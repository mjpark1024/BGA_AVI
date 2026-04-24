using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Euresys.MultiCam;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.IO;
using Common;

namespace Marker.Class
{
    public class Picolo64
    {
        public Picolo64()
        {
            //Init();
        }

        ~Picolo64()
        {

        }

        public void UnInit()
        {
            try
            {
                // Close MultiCam driver
                MC.CloseDriver();
            }
            catch (Euresys.MultiCamException exc)
            {
                MessageBox.Show(exc.Message, "MultiCam Exception");
            }
        }

        public void Delay(int ms)
        {
            int time = Environment.TickCount;

            do
            {
                if (Environment.TickCount - time >= ms)
                    return;
            } while (true);
        }
        public int Init()
        {
            int val = MC.OpenDriver();

            // Enable error logging
            if (val == 0) MC.SetParam(MC.CONFIGURATION, "ErrorLog", "error.log");

            return val;
        }

    }
    public class Picolo64Ch
    {
       // private MarkControl m_mc = null;
        private static Mutex imageMutex = new Mutex();
        UInt32 channel;
        private UInt32 currentSurface;
        private string m_Connector;
        private volatile bool channelactive;

        private System.Drawing.Point AlignPos1;
        private System.Drawing.Point AlignPos2; 
        // MC.CALLBACK multiCamCallback;

        public byte[] result = new byte[640 * 480];
        public byte[] align1 = new byte[40 * 40];
        byte[,] Align1 = new byte[40, 40];
        public byte[] align2 = new byte[40 * 40];
        byte[,] Align2 = new byte[40, 40];
        byte[,] tempResult = new byte[480, 640];
        public int MarginX1;
        public int MarginX2;
        public int MarginY1;
        public int MarginY2;
        public int Match;
        public bool UseThres = false;
        public int Threshold = 80;
        public BitmapSource AlignBmp1;
        public BitmapSource AlignBmp2;
        System.Windows.Controls.Image image;
        System.Windows.Controls.Image image1;
        System.Windows.Controls.Image image2;
        public bool bGrab = false;
        bool bLive;
        public int nGrabDone = 0;  //-1 Failure 0 Grabing 1 Done
        public bool bAuto = false;
        PixelFormat pf;
        public BitmapSource bmp;
        int GrabID = 0;
        Thread liveThread;
        public Picolo64Ch(string connector)//MarkControl mc)
        {
            pf = PixelFormats.Gray8;
            init(connector);
          //  m_mc = mc;
        }

        ~Picolo64Ch()
        {

        }

        public void Uninit()
        {
            try
            {
                LiveStop();
                // Whait that the channel has finished the last acquisition
                while (channelactive == true)
                {
                    Thread.Sleep(10);
                    if (channel != 0)
                        MC.SetParam(channel, "ChannelState", "IDLE");
                }

                // Delete the channel
                if (channel != 0)
                {
                    //MC.Delete(channel);
                    channel = 0;
                }
            }
            catch (Euresys.MultiCamException exc)
            {
                MessageBox.Show(exc.Message, "MultiCam Exception");
            }
        }

        public int init(string connector)
        {
            m_Connector = connector;
            int s = CreateChannel();
            //image = i;
            return 0;
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

        public void LiveThreadProc()
        {

            MC.SIGNALINFO sigInfo = new MC.SIGNALINFO();
            Microsoft.Win32.SafeHandles.SafeWaitHandle pHandle;
            int signalledHandle;

            // Define an array with 3 ManualResetEvent WaitHandles.
            WaitHandle[] waitHandles = new WaitHandle[]
            {
                new ManualResetEvent(false),
                new ManualResetEvent(false),
                new ManualResetEvent(false)
            };

            // Assignment of WaitHandles
            MC.GetParam(channel, MC.SignalEvent + MC.SIG_SURFACE_FILLED, out pHandle);
            waitHandles[0].SafeWaitHandle = pHandle;

            MC.GetParam(channel, MC.SignalEvent + MC.SIG_ACQUISITION_FAILURE, out pHandle);
            waitHandles[1].SafeWaitHandle = pHandle;

            MC.GetParam(channel, MC.SignalEvent + MC.SIG_END_CHANNEL_ACTIVITY, out pHandle);
            waitHandles[2].SafeWaitHandle = pHandle;

            try
            {
                // Wait for any of the 3 signal
                signalledHandle = WaitHandle.WaitAny(waitHandles, 1000);

                // Get the signal information
                switch (signalledHandle)
                {
                    case 0:
                        MC.GetSignalInfo(channel, MC.SIG_SURFACE_FILLED, out sigInfo);
                        break;
                    case 1:
                        MC.GetSignalInfo(channel, MC.SIG_ACQUISITION_FAILURE, out sigInfo);
                        break;

                    case 2:
                        MC.GetSignalInfo(channel, MC.SIG_END_CHANNEL_ACTIVITY, out sigInfo);
                        break;

                    case WaitHandle.WaitTimeout:
                        throw new Euresys.MultiCamException("Timeout");


                    default:
                        throw new Euresys.MultiCamException("Unknown signal");
                }

                HandleSignal(ref sigInfo);
            }
            catch //(Euresys.MultiCamException exc)
            {
                nGrabDone = -1;
            }
        }

        private void HandleSignal(ref MC.SIGNALINFO signalInfo)
        {
            switch (signalInfo.Signal)
            {
                // Handles captured image 
                case MC.SIG_SURFACE_FILLED:
                    ProcessingCallback(signalInfo);
                   
                    break;

                // Handle Acquisition errors
                case MC.SIG_ACQUISITION_FAILURE:
                    AcqFailureCallback(signalInfo);
                    break;

                // Terminate live thread gracefully 
                case MC.SIG_END_CHANNEL_ACTIVITY:
                    nGrabDone = -1;
                    channelactive = false;
                    break;

                default:
                    throw new Euresys.MultiCamException("Unknown signal");
            }
        }

        private void ProcessingCallback(MC.SIGNALINFO signalInfo)
        {
            UInt32 currentChannel = (UInt32)signalInfo.Instance;
            currentSurface = signalInfo.SignalInfo;

            try
            {
                // Update the image with the acquired image buffer data 
                Int32 width, height, bufferPitch;
                IntPtr bufferAddress;
                MC.GetParam(currentChannel, "ImageSizeX", out width);
                MC.GetParam(currentChannel, "ImageSizeY", out height);
                MC.GetParam(currentChannel, "BufferPitch", out bufferPitch);
                MC.GetParam(currentSurface, "SurfaceAddr", out bufferAddress);

                try
                {
                    // imageMutex.WaitOne();
                    //if (bGrab || bLive)
                    // {
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                                       new System.Threading.ThreadStart(delegate
                                       { }));

                        Marshal.Copy(bufferAddress, result, 0, (width * height));// Marshal.SizeOf(bufferAddress));

                       
                        if (bAuto)
                        {
                            //if (GrabID == 0)
                            //{
                            //    Action action = delegate
                            //    {
                            //         image1.Source = bmp;
                            //    }; pMain.Dispatcher.Invoke(action);
                            //}
                            //else
                            //{
                            //    Action action = delegate
                            //    {
                            //        image2.Source = bmp;
                            //    }; pMain.Dispatcher.Invoke(action);
                            //}
                        }
                        else
                        {
                            int r = (640 * pf.BitsPerPixel + 7) / 8;
                            bmp = BitmapSource.Create(640, 480, 96, 96, pf, null, result, r);
                            bmp.Freeze();
                            Action action = delegate
                            {
                                image.Source = bmp;
                            }; image.Dispatcher.Invoke(action);  
                        }

                        nGrabDone = 1;
                }
                catch
                {
                    nGrabDone = -1;
                }


            }
            catch (Euresys.MultiCamException)
            {
                nGrabDone = -1;
            }
            catch
            {
                nGrabDone = -1;
            }

        }

        private void AcqFailureCallback(MC.SIGNALINFO signalInfo)
        {

            
            UInt32 currentChannel = (UInt32)signalInfo.Context;

            // + PicoloVideo Sample Program

            try
            {
                if (!bAuto)
                {
                    int r = (640 * pf.BitsPerPixel + 7) / 8;
                    bmp = BitmapSource.Create(640, 480, 96, 96, pf, null, result, r);
                    bmp.Freeze();
                    Action action = delegate { image.Source = bmp; };
                }
                //m_mc.Dispatcher.Invoke(action);
            }
            catch (System.Exception exc)
            {
                MessageBox.Show(exc.Message, "System Exception");
            }
            nGrabDone = -1;

            // - PicoloVideo Sample Program
        }

        private void MultiCamCallback(ref MC.SIGNALINFO signalInfo)
        {
            switch (signalInfo.Signal)
            {
                case MC.SIG_SURFACE_FILLED:
                    ProcessingCallback(signalInfo);
                    break;
                case MC.SIG_ACQUISITION_FAILURE:
                    AcqFailureCallback(signalInfo);
                    break;
                case MC.SIG_END_CHANNEL_ACTIVITY:
                    channelactive = false;
                    break;
                default:
                    throw new Euresys.MultiCamException("Unknown signal");
            }
        }

        private int CreateChannel()
        {
            try
            {
                // Create a channel and associate it with the first connector on the first board
                MC.Create("CHANNEL", out channel);
                MC.SetParam(channel, "DriverIndex", 0);
                MC.SetParam(channel, "Connector", m_Connector);

                // Choose the video standard
                MC.SetParam(channel, "CamFile", "NTSC");
                // Choose the pixel color format
                MC.SetParam(channel, "ColorFormat", "Y8");

                // Choose the acquisition mode
                MC.SetParam(channel, "AcquisitionMode", "VIDEO");
                // Choose the way the first acquisition is triggered
                MC.SetParam(channel, "TrigMode", "IMMEDIATE");
                // Choose the triggering mode for subsequent acquisitions
                MC.SetParam(channel, "NextTrigMode", "SAME");
                // Choose the number of images to acquire
                MC.SetParam(channel, "SeqLength_Fr", 1);

                // Enable the signals corresponding to the callback functions
                MC.SetParam(channel, MC.SignalEnable + MC.SIG_SURFACE_FILLED, "ON");
                MC.SetParam(channel, MC.SignalEnable + MC.SIG_ACQUISITION_FAILURE, "ON");
                MC.SetParam(channel, MC.SignalEnable + MC.SIG_END_CHANNEL_ACTIVITY, "ON");

                // Enable Multicam Signal Using Windows Event
                MC.SetParam(channel, MC.SignalHandling + MC.SIG_SURFACE_FILLED, MC.SIGNALHANDLING_OS_EVENT_SIGNALING);
                MC.SetParam(channel, MC.SignalHandling + MC.SIG_ACQUISITION_FAILURE, MC.SIGNALHANDLING_OS_EVENT_SIGNALING);
                MC.SetParam(channel, MC.SignalHandling + MC.SIG_END_CHANNEL_ACTIVITY, MC.SIGNALHANDLING_OS_EVENT_SIGNALING);
                channelactive = false;

                // Prepare the channel in order to minimize the acquisition sequence startup latency
                MC.SetParam(channel, "ChannelState", "READY");

            }
            catch (Euresys.MultiCamException exc)
            {
                // An exception has occurred in the try {...} block. 
                // Retrieve its description and display it in a message box.
                MessageBox.Show(exc.Message, "MultiCam Exception");
                return 0;
            }
            return 1;
        }

        public void Live()
        {
            while (bLive)
            {
                Grab(0);
                Thread.Sleep(200);
            }
        }

        public void LiveStart()
        {
            bLive = true;
            if (liveThread != null)
            {
                liveThread.Abort();
                Thread.Sleep(100);
                liveThread = null;
            }
            liveThread = new Thread(Live);
            liveThread.Start();
        }

        public void LiveStop()
        {
            bLive = false;
            if (liveThread != null)
            {
                liveThread.Abort();
                Thread.Sleep(100);
                liveThread = null;
            }
        }

        private bool GrabDoneCheck()
        {
            return bGrab;
        }

        public int Grab(int anID)
        {
            nGrabDone = 0;
            GrabID = anID;
            String channelState;
            MC.GetParam(channel, "ChannelState", out channelState);
            if (channelState != "IDLE")
            {
                MC.SetParam(channel, "ChannelState", "IDLE");
                MC.GetParam(channel, "ChannelState", out channelState);
            }
            if (channelState != "ACTIVE")
                MC.SetParam(channel, "ChannelState", "ACTIVE");
            channelactive = true;
            LiveThreadProc();
            return 0;
        }

        public BitmapSource CropImage(int width, int height)
        {
            int sx = 320 - (width / 2);
            int sy = 240 - (height / 2);
            byte[] tmp = new byte[height * width];
            int x = 0;
            for (int j = sy; j < (sy + height); j++)
                for (int i = sx; i < (sx + width); i++)
                {
                    tmp[x] = result[i + (j * 640)];
                    x++;
                }
            int r = (width * pf.BitsPerPixel + 7) / 8;
            return BitmapSource.Create(width, height, 96, 96, pf, null, tmp, r);
        }

        public void resultToAlign1(string filename, System.Windows.Point pos)
        {
            AlignPos1.X = (int)(pos.X);
            AlignPos1.Y = (int)(pos.Y);
            int sx = (int)pos.X-20;
            int sy = (int)pos.Y-20;
            byte[,] tmp = new byte[480, 640];
            for (int j = 0; j < 480; j++)
            {
                for (int i = 0; i < 640; i++)
                {
                    tmp[j, i] = result[i + (j * 640)];
                }
            }
            int x = 0;
            for (int j = sy; j < (sy + 40); j++)
            {
                for (int i = sx; i < (sx + 40); i++)
                {
                    Align1[j - sy, i - sx] = tmp[j, i];
                    align1[x] = Align1[j - sy, i - sx];
                    if (Align1[j - sy, i - sx] > Threshold)
                        Align1[j - sy, i - sx] = 255;
                    else Align1[j - sy, i - sx] = 0;
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

        public void resultToAlign2(string filename, System.Windows.Point pos)
        {
            AlignPos2.X = (int)(pos.X);
            AlignPos2.Y = (int)(pos.Y);
            int sx = (int)pos.X - 20;
            int sy = (int)pos.Y - 20;
            byte[,] tmp = new byte[480, 640];
            for (int j = 0; j < 480; j++)
            {
                for (int i = 0; i < 640; i++)
                {
                    tmp[j, i] = result[i + (j * 640)];
                }
            }
            int x = 0;
            for (int j = sy; j < (sy + 40); j++)
            {
                for (int i = sx; i < (sx + 40); i++)
                {
                    Align2[j - sy, i - sx] = tmp[j, i];
                    align2[x] = Align2[j - sy, i - sx];
                    if (Align2[j - sy, i - sx] > Threshold)
                        Align2[j - sy, i - sx] = 255;
                    else Align2[j - sy, i - sx] = 0;
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

        public void BitmapSourceToArray(int Index)
        {
            if (Index == 0)
            {
                int width = AlignBmp1.PixelWidth;
                int height = AlignBmp1.PixelHeight;
                int stride = width * ((AlignBmp1.Format.BitsPerPixel + 7) / 8);
                AlignBmp1.CopyPixels(align1, stride, 0);

                for (int j = 0; j < 100; j++)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Align1[j, i] = align1[i + (j * 100)];
                        if (Align1[j, i] > Threshold)
                            Align1[j, i] = 255;
                        else Align1[j, i] = 0;
                    }
                }
            }
            else
            {
                int width = AlignBmp2.PixelWidth;
                int height = AlignBmp2.PixelHeight;
                int stride = width * ((AlignBmp2.Format.BitsPerPixel + 7) / 8);
                AlignBmp2.CopyPixels(align2, stride, 0);

                for (int j = 0; j < 100; j++)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Align2[j, i] = align2[i + (j * 100)];
                        if (Align2[j, i] > Threshold)
                            Align2[j, i] = 255;
                        else Align2[j, i] = 0;
                    }
                }
            }
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
                    if (Align1[j, i] > Threshold)
                        Align1[j, i] = 255;
                    else Align1[j, i] = 0;
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
                    if (Align2[j, i] > Threshold)
                        Align2[j, i] = 255;
                    else Align2[j, i] = 0;
                }
            }
            return true;
        }

        public int SearchAlign1(out float x, out float y, out int match)
        {
            if (!BitmapHelper.SearchAlign(result, align1, 150, out x, out y, out match, 0))
                return -1;
            x += 320 - AlignPos1.X;
            y += 240 - AlignPos1.Y;

            if (Math.Abs(x) > 100) return -1;
            if (Math.Abs(y) > 60) return -1;
            if (match < Match) return -1;

            return 0;
            
            int STEP = 3;
            int stpx = 400 / STEP;
            int stpy = 300 / STEP;
            int nMin = 1000000000;
            int resX = 0;
            int resY = 0;
            try
            {
                for (int j = 0; j < 480; j++)
                {
                    for (int i = 0; i < 640; i++)
                    {
                        if (UseThres)
                        {
                            if (result[i + (j * 640)] > Threshold)
                                tempResult[j, i] = 255;
                            else tempResult[j, i] = 0;
                        }
                        else tempResult[j, i] = result[i + (j * 640)];
                    }
                }
                int nSum;
                for (int j = 0; j < stpy; j++)
                {
                    for (int i = 0; i < stpx; i++)
                    {
                        int a = (j * STEP) + 120;
                        int b = (i * STEP) + 40;
                        nSum = Correation1((i * STEP), (j * STEP), STEP);
                        if (nSum < nMin)
                        {
                            resX = (i * STEP);
                            resY = (j * STEP);
                            nMin = nSum;
                        }
                    }
                }
                int tmpx = resX;
                int tmpy = resY;
                for (int j = (tmpy - STEP); j < (tmpy + STEP); j++)
                {
                    for (int i = (tmpx - STEP); i < (tmpx + STEP); i++)
                    {
                        nSum = Correation1(i, j, STEP);
                        if (nSum < nMin)
                        {
                            resX = i;
                            resY = j;
                            nMin = nSum;
                        }
                    }
                }
                match = NCorreation1(resX, resY);
                x = resX - AlignPos1.X + 20;
                y = resY - AlignPos1.Y + 20;
               // x /= 2;
               // y /= 2;
                if (Math.Abs(x) > 100) return -1;
                if (Math.Abs(y) > 60) return -1; 
                if (match < Match) return -1;
                else return 0;
            }
            catch
            {
                x = 0;
                y = 0;
                match = 0;
                return -1;
            }
        }

        public int SearchAlign2(out float x, out float y, out int match)
        {
            if (!BitmapHelper.SearchAlign(result, align2, 150, out x, out y, out match, 0))
                return -1;

            x += 320 - AlignPos2.X;
            y += 240 - AlignPos2.Y;

            if (Math.Abs(x) > 100) return -1;
            if (Math.Abs(y) > 60) return -1;
            if (match < Match) return -1;
            return 0;

            int STEP = 3;
            int stpx = 400 / STEP;
            int stpy = 300 / STEP;
            int nMin = 1000000000;
            int resX = 0;
            int resY = 0;
            try
            {
                for (int j = 0; j < 480; j++)
                {
                    for (int i = 0; i < 640; i++)
                    {
                        if (UseThres)
                        {
                            if (result[i + (j * 640)] > Threshold)
                                tempResult[j, i] = 255;
                            else tempResult[j, i] = 0;
                        }
                        else tempResult[j, i] = result[i + (j * 640)];
                    }
                }
                int nSum;
                for (int j = 0; j < stpy; j++)
                {
                    for (int i = 0; i < stpx; i++)
                    {
                        int a = (j * STEP) + 120;
                        int b = (i * STEP) + 40;
                        nSum = Correation2((i * STEP), (j * STEP), STEP);
                        if (nSum < nMin)
                        {
                            resX = (i * STEP);
                            resY = (j * STEP);
                            nMin = nSum;
                        }
                    }
                }
                int tmpx = resX;
                int tmpy = resY;
                for (int j = (tmpy - STEP); j < (tmpy + STEP); j++)
                {
                    for (int i = (tmpx - STEP); i < (tmpx + STEP); i++)
                    {
                        nSum = Correation2(i, j, STEP);
                        if (nSum < nMin)
                        {
                            resX = i;
                            resY = j;
                            nMin = nSum;
                        }
                    }
                }
                match = NCorreation2(resX, resY);
                x = resX - AlignPos2.X + 20;
                y = resY - AlignPos2.Y + 20;
               // x /= 2;
               // y /= 2;
                if (Math.Abs(x) > 100) return -1;
                if (Math.Abs(y) > 60) return -1; 
                if (match < Match) return -1;
                else return 0;
            }
            catch
            {
                x = 0;
                y = 0;
                match = 0;
                return -1;
            }
        }

        private int Correation1(int cx, int cy, int step)
        {
            int Coff = 0;
            int size = (640 * 480) / (step * step);
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

        private int Correation2(int cx, int cy, int step)
        {
            int Coff = 0;
            int size = (640 * 480) / (step * step);
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
            if (m > 100) return 100;
            else return m;
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
            {
                fCoff = 0;
            }
            else
                fCoff = h / l;
            int m = (int)(fCoff * fCoff * 100);
            if (m > 100) return 100;
            else return m;
        }

    }
}
