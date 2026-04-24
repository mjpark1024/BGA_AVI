using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PylonC.NET;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Media;
using OpenCvSharp;
using System.Diagnostics;
using System.IO;
using Common;
using System.Windows.Forms;

namespace Marker
{

    public class PylonCam
    {
        const uint NUM_IMAGE_BUFFERS = 3;         /* Number of buffers used for grabbing. */

        PYLON_DEVICE_HANDLE hDev;
        PYLON_STREAMGRABBER_HANDLE hStreamGrabber;        /* Handle for the pylon stream grabber. */
        PYLON_WAITOBJECT_HANDLE hWaitStream;              /* Handle used for waiting for a grab to be finished. */

        Dictionary<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>> buffers = new Dictionary<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<byte>>(); /* Holds handles and buffers used for grabbing. */
        public bool IsOpenVision = false;
        uint numDevicesAvail;
        bool isAvail;
        public bool Grab_Ready = false;
        bool bLive;
        public int nGrabDone = 0;  //-1 Failure 0 Grabing 1 Done
        public bool bAuto = false;
        public BitmapSource bmp;
        private Thread grab_thread;
        public bool Grab_Done = false;
        public bool Inspect_Done;

        public BitmapSource AlignBmp1;
        public BitmapSource AlignBmp2;
        public BitmapSource MarkBmp;
        System.Windows.Media.PixelFormat pf;

        int m_image_idx = 0;
        private Mat m_image_buffer;

        public int Match;
        private bool grab_thread_continue = false;

        public byte[] result = new byte[1280 * 1024];
        byte[,] tempResult = new byte[1024, 1280];

        byte[,] Align1 = new byte[40, 40];
        byte[,] Align2 = new byte[40, 40];
        byte[,] Mark = new byte[28, 28];
        byte[,] AOI = new byte[300, 880];
        public byte[] align1 = new byte[40 * 40];
        public byte[] align2 = new byte[40 * 40];
        public byte[] mark = new byte[28 * 28];
        public byte[] aoi = new byte[300 * 880];
        private System.Drawing.Point AlignPos1;
        private System.Drawing.Point AlignPos2;
        private System.Drawing.Point MarkPos;
        private System.Drawing.Point AoiPos;

        System.Windows.Controls.Image image;
        System.Windows.Controls.Image image1;
        System.Windows.Controls.Image image2;

        public Mat images
        {
            get { return m_image_buffer; }
            private set { m_image_buffer = value; }
        }

        public int image_idx
        {
            get { return m_image_idx; }
            private set { m_image_idx = value; }
        }

        public PylonCam()
        {
            pf = PixelFormats.Gray8;

            hDev = new PYLON_DEVICE_HANDLE();

            pylon_init();
        }

        ~PylonCam()
        {

        }

        public bool pylon_init()
        {
            try
            {
                Environment.SetEnvironmentVariable("PYLON_GIGE_HEARTBEAT", "3000" /*ms*/);
                Pylon.Initialize();

                numDevicesAvail = Pylon.EnumerateDevices();
                hDev = Pylon.CreateDeviceByIndex(0);

                Pylon.DeviceOpen(hDev, Pylon.cPylonAccessModeControl | Pylon.cPylonAccessModeStream);

                hStreamGrabber = Pylon.DeviceGetStreamGrabber(hDev, 0);

                Pylon.StreamGrabberOpen(hStreamGrabber);
                hWaitStream = Pylon.StreamGrabberGetWaitObject(hStreamGrabber);

                Pylon.DeviceSetIntegerFeature(hDev, "Width", 1280);
                Pylon.DeviceSetIntegerFeature(hDev, "Height", 1024);
                Pylon.DeviceSetFloatFeature(hDev, "ExposureTimeAbs", 1000.0f);
                Pylon.DeviceSetIntegerFeature(hDev, "BlackLevelRaw", 0);

                isAvail = Pylon.DeviceFeatureIsAvailable(hDev, "AcquisitionFrameRateEnable");
                if (!isAvail)
                {
                    Pylon.DeviceSetBooleanFeature(hDev, "AcquisitionFrameRateEnable", true);
                }
                Pylon.DeviceSetFloatFeature(hDev, "AcquisitionFrameRateAbs", 20f);

                isAvail = Pylon.DeviceFeatureIsAvailable(hDev, "EnumEntry_PixelFormat_Mono8");
                if (!isAvail)
                {
                    throw new Exception("Device doesn't support the Mono8 pixel format.");
                }
                Pylon.DeviceFeatureFromString(hDev, "PixelFormat", "Mono8");

                isAvail = Pylon.DeviceFeatureIsAvailable(hDev, "EnumEntry_TriggerSelector_FrameStart");
                if (isAvail)
                {
                    Pylon.DeviceFeatureFromString(hDev, "TriggerSelector", "FrameStart");
                    Pylon.DeviceFeatureFromString(hDev, "TriggerMode", "Off");
                }
                Pylon.DeviceFeatureFromString(hDev, "AcquisitionMode", "Continuous");

                IsOpenVision = true;

                SetupGrab();
                live_threadStart();
            }
            catch (Exception e)
            {
                string msg = GenApi.GetLastErrorMessage() + "\n" + GenApi.GetLastErrorDetail();
                Console.WriteLine(e.Message);
                if (msg != "\n")
                {
                    Console.WriteLine("Last error message:");
                    Console.WriteLine(msg);
                }
                return false;
            }
            return true;
        }

        public void SetImage(System.Windows.Controls.Image i)
        {
            image = i;
        }

        public void SetAuto(bool abAuto, System.Windows.Controls.Image i1, System.Windows.Controls.Image i2)
        {
            bAuto = abAuto;
            image1 = i1;
            image2 = i2;
        }

        public void Change_BlackValue(int value)
        {
            Pylon.DeviceSetIntegerFeature(hDev, "BlackLevelRaw", value);
        }

        public void Change_Exposure(int value)
        {
            Pylon.DeviceSetFloatFeature(hDev, "ExposureTimeAbs", value);
        }

        private void SetupGrab()
        {
            Grab_Ready = true;
            image_idx = 0;

            foreach (KeyValuePair<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>> pair in buffers)
            {
                pair.Value.Dispose();
            }
            buffers.Clear();

            Pylon.StreamGrabberSetMaxNumBuffer(hStreamGrabber, NUM_IMAGE_BUFFERS);

            uint payloadSize = checked((uint)Pylon.DeviceGetIntegerFeature(hDev, "PayloadSize"));
            Pylon.StreamGrabberSetMaxBufferSize(hStreamGrabber, payloadSize);
            Pylon.StreamGrabberPrepareGrab(hStreamGrabber);

            for (uint i = 0; i < NUM_IMAGE_BUFFERS; ++i)
            {
                PylonBuffer<Byte> buffer = new PylonBuffer<byte>(payloadSize, true);
                PYLON_STREAMBUFFER_HANDLE handle = Pylon.StreamGrabberRegisterBuffer(hStreamGrabber, ref buffer);
                buffers.Add(handle, buffer);
            }
            foreach (KeyValuePair<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>> pair in buffers)
            {
                Pylon.StreamGrabberQueueBuffer(hStreamGrabber, pair.Key, 0);
            }
        }

        public void Grab()
        {
            isAvail = Pylon.DeviceFeatureIsAvailable(hDev, "EnumEntry_TriggerSelector_AcquisitionStart");
            if (isAvail)
            {
                Pylon.DeviceExecuteCommandFeature(hDev, "AcquisitionStart");
            }
        }

        public void live_threadStart()
        {
            if (IsOpen())
            {
                try
                {
                    if (grab_thread == null)
                    {
                        grab_thread = new Thread(GrabThreadFunction);
                        grab_thread.Start();
                    }
                }
                catch (Exception ex)
                {
                    CleanupGrab();
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        public void LiveStart(bool live)
        {
            bLive = live;
        }

        public void LiveStop()
        {
            bLive = false;
        }

        public byte[] grab()
        {
            Grab_Done = false;
            int cnt = 0;

            while (!Grab_Done)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(delegate { }));
                Thread.Sleep(100);
                cnt++;
                if (cnt > 30)
                    return null;
            }
            byte[] image = new byte[result.Length];
            Array.Copy(result, image, result.Length);

            return image;
        }

       

        private void GrabThreadFunction()
        {
            grab_thread_continue = true;

            try
            {
                while (grab_thread_continue)
                {
                    if (!Pylon.WaitObjectWait(hWaitStream, 200))
                    {                       
                        continue;
                    }

                    PylonGrabResult_t pylon_grab_result;

                    if (!Pylon.StreamGrabberRetrieveResult(hStreamGrabber, out pylon_grab_result))
                    {
                        throw new Exception("Failed to retrieve a grab result.");
                    }

                    switch (pylon_grab_result.Status)
                    {
                        case EPylonGrabStatus.Grabbed:
                            // lock (this)
                            //{
                            PylonBuffer<Byte> buffer = null;

                            if (!buffers.TryGetValue(pylon_grab_result.hBuffer, out buffer))
                                throw new Exception("Failed to find the buffer associated with the handle returned in grab result.");

                            if (buffer.Array.Length > 0)
                            {
                                if (grab_thread_continue)
                                    SetDisplay(buffer.Array);
                            }

                            Pylon.StreamGrabberQueueBuffer(hStreamGrabber, pylon_grab_result.hBuffer, 0);
                            Grab_Done = true;
                            // }
                            break;
                    }
                }
            }
#pragma warning disable CS0168 // 변수가 선언되었지만 사용되지 않았습니다.
            catch (Exception ex)
#pragma warning restore CS0168 // 변수가 선언되었지만 사용되지 않았습니다.
            {
                
            }
            finally
            {
                CleanupGrab();
            }

            grab_thread_continue = false;
        }

        private void CleanupGrab()
        {
            try
            {
                if (IsOpen())
                {
                    Pylon.StreamGrabberCancelGrab(hStreamGrabber);

                    bool isReady; /* Used as an output parameter. */
                    do
                    {
                        PylonGrabResult_t grabResult;  /* Stores the result of a grab operation. */
                        isReady = Pylon.StreamGrabberRetrieveResult(hStreamGrabber, out grabResult);

                    } while (isReady);

                    foreach (KeyValuePair<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>> pair in buffers)
                    {
                        Pylon.StreamGrabberDeregisterBuffer(hStreamGrabber, pair.Key);
                    }
                    foreach (KeyValuePair<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>> pair in buffers)
                    {
                        pair.Value.Dispose();
                    }
                    buffers.Clear();
                    Pylon.StreamGrabberFinishGrab(hStreamGrabber);
                }
            }
#pragma warning disable CS0168 // 변수가 선언되었지만 사용되지 않았습니다.
            catch (Exception ex)
#pragma warning restore CS0168 // 변수가 선언되었지만 사용되지 않았습니다.
            {
            }
        }

        private void SetDisplay(byte[] img)
        {
            try
            {
                images = new Mat(1024, 1280, MatType.CV_8UC1, img);
                lock (this)
                {
                    result = img;
                }

                if (bLive)
                {
                    
                    BitmapSource bs = OpenCvSharp.Extensions.BitmapSourceConverter.ToBitmapSource(images);
                    bs.Freeze();

                    Action action = delegate
                    {
                        image.Source = bs;
                    }; image.Dispatcher.Invoke(action);

                    bs = null;
                }

                image_idx++;
                nGrabDone = 1;
            }
            catch
            {
                nGrabDone = -1;
            }
        }

        public void resultToAlign1(string filename, System.Windows.Point pos)
        {
            AlignPos1.X = (int)pos.X;
            AlignPos1.Y = (int)pos.Y;

            int sx = (int)pos.X - 20;
            int sy = (int)pos.Y - 20;

            byte[,] tmp = new byte[1024, 1280];
            for (int j = 0; j < 1024; j++)
            {
                for (int i = 0; i < 1280; i++)
                {
                    tmp[j, i] = result[i + (j * 1280)];
                }
            }
            int x = 0;
            for (int j = sy; j < (sy + 40); j++)
            {
                for (int i = sx; i < (sx + 40); i++)
                {
                    Align1[j - sy, i - sx] = tmp[j, i];
                    align1[x] = Align1[j - sy, i - sx];
                    x++;
                }
            }
            int r = (40 * pf.BitsPerPixel + 7) / 8;
            AlignBmp1 = BitmapSource.Create(40, 40, 96, 96, pf, null, align1, r);
            if (filename == "") return;
            FileStream stream = new FileStream(filename, FileMode.Create);
            stream.Write(align1, 0, align1.Length);
            stream.Close();
        }

        public int SearchAlign1(out float x, out float y, out int match, ref byte[] temp)
        {
            for (int i = 0; i < 3; i++)
            {
                temp = grab();
                if (temp == null)
                {
                    x = 0; y = 0; match = 0;
                    Console.WriteLine("Grab is null!");
                    return -1;
                }
                //Array.Copy(result, temp, result.Length);
                if (!BitmapHelper.SearchAlign(temp, align1, 150, out x, out y, out match, 1))
                    continue;
                x += 640 - AlignPos1.X;
                y += 512 - AlignPos1.Y;

                if (Math.Abs(x) > 90)
                    continue;
                if (Math.Abs(y) > 90)
                    continue;
                if (match < Match)
                    continue;
                return 0;
            }
            x = 0; y = 0; match = 0;
            return -1;
        }

        private int Correation1(int cx, int cy, int step)
        {
            int Coff = 0;
            int size = (1280 * 1024) / (step * step);
            int tCoff = size * 40;
            int w = 40 / step;
            int h = 40 / step;
            byte src;
            byte des;
            for (int j = 0; j < w; j++)
            {

                for (int i = 0; i < h; i++)
                {
                    src = Align1[j * step, i * step];
                    des = tempResult[cy + (j * step), cx + (i * step)];
                    Coff = Coff + System.Math.Abs(des - src);
                }
                if (tCoff < Coff)
                {
                    return 999999999;
                }
            }
            return Coff;
        }

        private int NCorreation1(int x, int y)
        {
            int Num = 1;// 000;
            byte src;
            byte des;
            int a = 0;
            int b = 0;
            int c = 0;
            int d = 0;
            int e = 0;
            for (int j = 0; j < 40; j++)
            {
                for (int i = 0; i < 40; i++)
                {
                    src = Align1[j, i];
                    des = tempResult[y + j, x + i];
                    a += des;
                    b += src;
                    c += (des * src);
                    d += (des * des);
                    e += (src * src);
                }
            }
            a = a / 10000;
            b = b / 10000;
            c = c / 10000;
            d = d / 10000;
            e = e / 10000;

            double h = (Num * c) - (a * b);
            int z = (Num * d) - (a * a);
            int left = (int)System.Math.Sqrt(z);
            z = (Num * e) - (b * b);
            int right = (int)System.Math.Sqrt(z);
            int l = (left * right);
            double fCoff;
            if ((h == 0) || (l == 0))
            {
                fCoff = 0;
            }
            else
                fCoff = h / l;
            int m = (int)(fCoff * fCoff * 100);
            if (m > 100)
                return 100;
            else
                return m;
        }

        public int SearchAlign2(out float x, out float y, out int match, ref byte[] temp)
        {
            for (int i = 0; i < 4; i++)
            {
                temp = grab();
                if (temp == null)
                {
                    x = 0; y = 0; match = 0;

                    if (i == 3) { return -1; }

                    continue;
                }

                if (!BitmapHelper.SearchAlign(temp, align2, 150, out x, out y, out match, 1))
                    continue;

                x += 640 - AlignPos2.X;
                y += 512 - AlignPos2.Y;

                if (Math.Abs(x) > 90)
                    continue;
                if (Math.Abs(y) > 90)
                    continue;
                if (match < Match)
                    continue;
                return 0;
            }
            x = 0; y = 0; match = 0;
            return -1;
        }

        private int Correation2(int cx, int cy, int step)
        {
            int Coff = 0;
            int size = (1280 * 1024) / (step * step);
            int tCoff = size * 40;
            int w = 40 / step;
            int h = 40 / step;
            byte src;
            byte des;
            for (int j = 0; j < w; j++)
            {

                for (int i = 0; i < h; i++)
                {
                    src = Align2[j * step, i * step];
                    des = tempResult[cy + (j * step), cx + (i * step)];
                    Coff = Coff + System.Math.Abs(des - src);
                }
                if (tCoff < Coff)
                {
                    return 999999999;
                }
            }
            return Coff;
        }

        private int NCorreation2(int x, int y)
        {
            int Num = 1;// 000;
            byte src;
            byte des;
            int a = 0;
            int b = 0;
            int c = 0;
            int d = 0;
            int e = 0;
            for (int j = 0; j < 40; j++)
            {
                for (int i = 0; i < 40; i++)
                {
                    src = Align2[j, i];
                    des = tempResult[y + j, x + i];
                    a += des;
                    b += src;
                    c += (des * src);
                    d += (des * des);
                    e += (src * src);
                }
            }
            a = a / 10000;
            b = b / 10000;
            c = c / 10000;
            d = d / 10000;
            e = e / 10000;

            double h = (Num * c) - (a * b);
            int z = (Num * d) - (a * a);
            int left = (int)System.Math.Sqrt(z);
            z = (Num * e) - (b * b);
            int right = (int)System.Math.Sqrt(z);
            int l = (left * right);
            double fCoff;
            if ((h == 0) || (l == 0))
                fCoff = 0;
            else
                fCoff = h / l;
            int m = (int)(fCoff * fCoff * 100);
            if (m > 100)
                return 100;
            else
                return m;
        }


        public void resultToAlign2(string filename, System.Windows.Point pos)
        {
            AlignPos2.X = (int)(pos.X);
            AlignPos2.Y = (int)(pos.Y);
            int sx = (int)pos.X - 20;
            int sy = (int)pos.Y - 20;
            byte[,] tmp = new byte[1024, 1280];
            for (int j = 0; j < 1024; j++)
            {
                for (int i = 0; i < 1280; i++)
                {
                    tmp[j, i] = result[i + (j * 1280)];
                }
            }
            int x = 0;
            for (int j = sy; j < (sy + 40); j++)
            {
                for (int i = sx; i < (sx + 40); i++)
                {
                    Align2[j - sy, i - sx] = tmp[j, i];
                    align2[x] = Align2[j - sy, i - sx];
                    x++;
                }
            }
            int r = (40 * pf.BitsPerPixel + 7) / 8;
            AlignBmp2 = BitmapSource.Create(40, 40, 96, 96, pf, null, align2, r);
            if (filename == "") return;
            FileStream stream = new FileStream(filename, FileMode.Create);
            stream.Write(align2, 0, align2.Length);
            stream.Close();
        }

        public void resultToMarkRoi(string filename, System.Windows.Point pos)
        {
            MarkPos.X = (int)(pos.X);
            MarkPos.Y = (int)(pos.Y);
            int sx = (int)pos.X - 14;
            int sy = (int)pos.Y - 14;
            byte[,] tmp = new byte[1024, 1280];
            for (int j = 0; j < 1024; j++)
            {
                for (int i = 0; i < 1280; i++)
                {
                    tmp[j, i] = result[i + (j * 1280)];
                }
            }
            int x = 0;
            for (int j = sy; j < (sy + 28); j++)
            {
                for (int i = sx; i < (sx + 28); i++)
                {
                    Mark[j - sy, i - sx] = tmp[j, i];
                    mark[x] = Mark[j - sy, i - sx];
                    x++;
                }
            }
            int r = (28 * pf.BitsPerPixel + 7) / 8;
            MarkBmp = BitmapSource.Create(28, 28, 96, 96, pf, null, mark, r);
            if (filename == "") return;
            FileStream stream = new FileStream(filename, FileMode.Create);
            stream.Write(mark, 0, mark.Length);
            stream.Close();
        }

        public void resultToAoiRoi(string filename, System.Windows.Point pos)
        {
            AoiPos.X = (int)(pos.X);
            AoiPos.Y = (int)(pos.Y);
            int sx = (int)pos.X - 440;
            int sy = (int)pos.Y - 150;
            byte[,] tmp = new byte[1024, 1280];
            for (int j = 0; j < 1024; j++)
            {
                for (int i = 0; i < 1280; i++)
                {
                    tmp[j, i] = result[i + (j * 1280)];
                }
            }
            int x = 0;
            for (int j = sy; j < (sy + 300); j++)
            {
                for (int i = sx; i < (sx + 880); i++)
                {
                    AOI[j - sy, i - sx] = tmp[j, i];
                    aoi[x] = AOI[j - sy, i - sx];
                    x++;
                }
            }
            int r = (880 * pf.BitsPerPixel + 7) / 8;
            MarkBmp = BitmapSource.Create(880, 300, 96, 96, pf, null, aoi, r);
            if (filename == "") return;
            FileStream stream = new FileStream(filename, FileMode.Create);
            stream.Write(aoi, 0, aoi.Length);
            stream.Close();
        }

        public BitmapSource getAoiRoi(System.Windows.Point pos)
        {
            int sx = (int)pos.X - 440;
            int sy = (int)pos.Y - 150;

            byte[,] tmp = new byte[1024, 1280];
            byte[,] b_AOI = new byte[300, 880];
            byte[] b_aoi = new byte[300 * 880];
            BitmapSource b_markbmp;

            for (int j = 0; j < 1024; j++)
            {
                for (int i = 0; i < 1280; i++)
                {
                    tmp[j, i] = result[i + (j * 1280)];
                }
            }
            int x = 0;
            for (int j = sy; j < (sy + 300); j++)
            {
                for (int i = sx; i < (sx + 880); i++)
                {
                    b_AOI[j - sy, i - sx] = tmp[j, i];
                    b_aoi[x] = AOI[j - sy, i - sx];
                    x++;
                }
            }
            int r = (880 * pf.BitsPerPixel + 7) / 8;
            b_markbmp = BitmapSource.Create(880, 300, 96, 96, pf, null, b_aoi, r);

            return b_markbmp;
        }

        public bool ImageToAlign1(string filename, int x, int y)
        {
            AlignPos1.X = x;
            AlignPos1.Y = y;
            FileInfo f = new FileInfo(filename);
            if (!f.Exists)
            {
                return false;
            }
            FileStream stream = new FileStream(filename, FileMode.Open);
            stream.Read(align1, 0, align1.Length);
            stream.Close();
            int r = (40 * pf.BitsPerPixel + 7) / 8;
            AlignBmp1 = BitmapSource.Create(40, 40, 96, 96, pf, null, align1, r);
            for (int j = 0; j < 40; j++)
            {
                for (int i = 0; i < 40; i++)
                {
                    Align1[j, i] = align1[i + (j * 40)];
                }
            }
            return true;
        }

        public bool ImageToAlign2(string filename, int x, int y)
        {
            AlignPos2.X = x;
            AlignPos2.Y = y;
            FileInfo f = new FileInfo(filename);
            if (!f.Exists)
            {
                return false;
            }
            FileStream stream = new FileStream(filename, FileMode.Open);
            stream.Read(align2, 0, align2.Length);
            stream.Close();
            int r = (40 * pf.BitsPerPixel + 7) / 8;
            AlignBmp2 = BitmapSource.Create(40, 40, 96, 96, pf, null, align2, r);
            for (int j = 0; j < 40; j++)
            {
                for (int i = 0; i < 40; i++)
                {
                    Align2[j, i] = align2[i + (j * 40)];
                }
            }
            return true;
        }

        public bool ImageToMark(string filename, int x, int y)
        {
            MarkPos.X = x;
            MarkPos.Y = y;
            FileInfo f = new FileInfo(filename);
            if (!f.Exists)
            {
                return false;
            }
            FileStream stream = new FileStream(filename, FileMode.Open);
            stream.Read(mark, 0, mark.Length);
            stream.Close();
            int r = (28 * pf.BitsPerPixel + 7) / 8;
            MarkBmp = BitmapSource.Create(28, 28, 96, 96, pf, null, mark, r);
            for (int j = 0; j < 28; j++)
            {
                for (int i = 0; i < 28; i++)
                {
                    Mark[j, i] = mark[i + (j * 28)];
                }
            }

            return true;
        }

        public bool IsOpen()
        {
            bool res = false;

            if (hStreamGrabber != null && hStreamGrabber.IsValid)
            {
                try
                {
                    res = Pylon.StreamGrabberIsOpen(hStreamGrabber);
                }
#pragma warning disable CS0168 // 변수가 선언되었지만 사용되지 않았습니다.
                catch (Exception ex)
#pragma warning restore CS0168 // 변수가 선언되었지만 사용되지 않았습니다.
                {
                }
            }
            return res;
        }



        public void ClearVisionData()
        {
            Grab_Done = false;
            Grab_Ready = false;
            Inspect_Done = false;
        }

        public void close_vision()
        {
            if (IsOpen())
            {
                try
                {
                    Pylon.StreamGrabberClose(hStreamGrabber);
                    hStreamGrabber.SetInvalid();

                    Pylon.DeviceClose(hDev);

                    Pylon.DestroyDevice(hDev);
                    hDev.SetInvalid();
                    hDev = null;
                }
#pragma warning disable CS0168 // 변수가 선언되었지만 사용되지 않았습니다.
                catch (Exception ex)
#pragma warning restore CS0168 // 변수가 선언되었지만 사용되지 않았습니다.
                {
                }
            }
        }
    }
}
