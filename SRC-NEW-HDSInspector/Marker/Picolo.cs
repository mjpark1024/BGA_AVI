using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using System.Drawing;
using System.Threading;
using System.IO;

namespace Marker
{
    public struct t_SurfaceInfo
    {
        public int STRUCTSIZE;
        public int TYPE2;
        unsafe public void* ADDRESS;
        public int SIZE;
        public int PITCH;
        public int UserAllocatedMemory;
    }
    public class Picolo
    {
        public const int EC_OK = 0;
        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MultiCamSystemInitialize();
        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MultiCamSystemTerminate();

        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MultiCamSystemAcquisitionStart();
        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MultiCamSystemAcquisitionStop();
        public Picolo()
        {
            //Init();
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
            MultiCamSystemAcquisitionStop();
            Delay(200);
            MultiCamSystemTerminate();
            int s = MultiCamSystemInitialize();
            if (s != EC_OK)
            { return 0; }
            else return 1;
        }
    }
    public class PicoloChannel
    {
        public UInt32 MULTI_ID1 = 0;
        public UInt32 MULTI_ID2 = 1;
        public UInt32 MULTI_ID3 = 2;
        public UInt32 MULTI__YC = 3;
        public const string BoardIdentificator = "#0 - Picolo";
        public const int EC_OK = 0;
        public const int EC_ERROR_DEVICE_NOT_FOUND = -1;
        public const int EC_ERROR_INVALID_PARAMETER = -2;
        public const int EC_ERROR_TIMEOUT = -3;
        public const int EC_ERROR_INVALID_HANDLE = -4;
        public const int EC_ERROR_SURFACE_ERROR = -5;
        public const int EC_ERROR_CHANNEL_ERROR = -6;
        public const int EC_ERROR_SYSTEM_ERROR = -7;
        public const int EC_ERROR_UNSUCCESSFUL = -8;
        public const int EC_ERROR_NO_DATA = -9;
        public const int EC_ERROR_INVALID_PARAMETER_VALUE = -10;
        public const int EC_ERROR_IO_INDEX_ERROR = -11;
        public const int EC_ERROR_INVALID_IO_FUNCTION = -12;
        public const int EC_ERROR_INVALID_PID = -13;
        public const int EC_ERROR_NOT_IMPLEMENTED = -100;
        public const int EC_SIGNAL_CHANNEL_STATE = 0;
        public const int EC_SIGNAL_CHANNEL_ERROR = 1;
        public const int EC_SIGNAL_SURFACE_STATE = 2;
        public const int EC_SIGNAL_SURFACE_AVAILABLE = 3;
        public const int EC_SIGNAL_ASM_STATE = 4;
        public const int EC_SIGNAL_ASM_ERROR = 5;
        public const int EC_SIGNAL_SYSTEM_TRIGGER = 6;
        public const int EC_SIGNAL_IO_ACTIVITY = EC_SIGNAL_SYSTEM_TRIGGER;
        public const int EC_MAX_EVENTS = 7;

        public const int EC_STD_TIMEOUT = 1000;
        public const int EC_MAX_SOURCE = 30;
        public const int EC_MAX_BOARD = 30;

        public const int ECSOURCE_STATE_USED = 0;
        public const int ECSOURCE_STATE_AVAILABLE = -1;
        public const int ECSOURCE_STATE_NOT_AVAILABLE = -2;

        public const int EC_MAX_SURFACE = 16384;
        public const int EC_PAUSE_INITPAUSE = 1;
        public const int EC_PAUSE_REPEATPAUSE = 2;
        public const int EC_PAUSE_LAST = 16;

        public const int EC_PAUSE_LIVE = 32;

        public const int EC_TRIGGER_INIT = 1;
        public const int EC_TRIGGER_REPEAT = 2;
        public const int EC_TRIGGER_LAST = 16;


        public const int EC_PARAM_Standard = 13;

        public const int EC_PARAM_Scanning = 14;

        public const int EC_PARAM_Resolution = 15;

        public const int EC_PARAM_FieldMode = 18;

        public const int EC_PARAM_AcqColFmt = 146;


        public const int EC_STATUS_PARITY_UP = 128;
        public const int EC_ERROR_DELAY = 256;
        public const int EC_ERROR_FIFO_RESYNC = 512;
        public const int EC_ERROR_MISMATCH = 1024;
        public const int EC_ERROR_RESTART = 2048;
        public const int EC_ERROR_FIFO = 4096;
        public const int EC_ERROR_SKIP = 8192;
        public const int EC_ERROR_NOSIG = 16384;
        public const int EC_ERROR_DMA = 32768;
        public const int EC_ERROR_FLUSH = 65280;
        public const int EC_ERROR_CHANNEL_TIMEOUT = 65536;

        // Channel Param.
        public const int EC_PARAM_ChannelState = 163;
        public const int EC_PARAM_ChannelSurfaceCount = 164;
        public const int EC_PARAM_ChannelTimeCode = 165;
        public const int EC_PARAM_ChannelContext = 120;
        public const int EC_PARAM_ChannelOperation = 69;

        // Channel Acquisition Sequence
        public const int EC_PARAM_InitPause = 149;
        public const int EC_PARAM_InitTrigger = 150;
        public const int EC_PARAM_InitGrabSync = 151;
        public const int EC_PARAM_InitGrabDelay = 152;
        public const int EC_PARAM_RepeatPause = 153;
        public const int EC_PARAM_RepeatTrigger = 154;
        public const int EC_PARAM_RepeatGrabSync = 155;
        public const int EC_PARAM_RepeatGrabDelay = 156;
        public const int EC_PARAM_RepeatGrabCount = 157;


        // Channel Trigger Param }
        public const int EC_PARAM_TriggerMode = 158;
        public const int EC_PARAM_TriggerMask = 159;
        public const int EC_PARAM_AssemblerMask = 160;

        public const int EC_PARAM_VideoGain = 141; // Gain}
        public const int EC_PARAM_VideoOffset = 142; // Offset}
        public const int EC_PARAM_VideoUGain = 143; // U Gain}
        public const int EC_PARAM_VideoVGain = 144; // V Gain}
        public const int EC_PARAM_VideoHue = 145; // This parameter is kept for compatibility issue only. Please use EC_PARAM_VideoNtscHue.}
        public const int EC_PARAM_VideoNtscHue = 119; // Hue}
        public const int EC_PARAM_AcqMinVal = 147; // Acquisition range minimum value}
        public const int EC_PARAM_AcqMaxVal = 148; // Acquisition range maximum value}

        public const int EC_PARAM_ImageFlipX = 65; // Flip X direction}
        public const int EC_PARAM_ImageFlipY = 66; // Flip Y direction}
        public const int EC_PARAM_ImageSizeX = 60;
        public const int EC_PARAM_ImageSizeY = 63;


        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MultiCamSystemInitialize();
        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MultiCamSystemTerminate();

        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MultiCamChannelSetParameterInt(int hChannel, int ParamID, int Value);
        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MultiCamChannelGetParameterInt(int hChannel, int ParamID, out IntPtr pValue);
        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MultiCamSignalWait(UInt32 EventID);

        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MultiCamChannelCreate(string ChannelConfigurationFile, string AsmIdentification, UInt32 SourceID);
        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MultiCamChannelDelete(int hChannel);

        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        unsafe public static extern int MultiCamSurfaceCreate(t_SurfaceInfo* pSurfaceInfo);
        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MultiCamSurfaceDelete(int handle);

        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MultiCamChannelAddSurface(int hChannel, int hSurface);
        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MultiCamSignalMask(UInt32 EventID, UInt32 Mask);

        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MultiCamSystemAcquisitionStart();
        [DllImport("MultiCam32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MultiCamSystemAcquisitionStop();

        public t_SurfaceInfo SurfInfo = new t_SurfaceInfo();
        int hChannel0 = 0;
        public byte[] result = new byte[640 * 480];
        public byte[] align = new byte[100 * 100];
        byte[,] Align = new byte[100, 100];
        byte[,] tempResult = new byte[480, 640];
        public BitmapSource AlignBmp;
        System.Windows.Controls.Image image;
        public bool bLive = false;
        public UInt32 channel;
        PixelFormat pf;
        BitmapSource bmp;
        SynchronizationContext sc;
        public int LiveInterval = 50;

        public PicoloChannel()
        {
            pf = PixelFormats.Gray8;
        }


        public int init(UInt32 ch, System.Windows.Controls.Image i)
        {
            int s = CreateChannel(ch);
            channel = ch;
            image = i;
            return s;
        }

        public void LiveStart()
        {
            bLive = true;
            sc = SynchronizationContext.Current;
            Thread thd = new Thread(new ThreadStart(Live));
            thd.SetApartmentState(ApartmentState.STA);
            thd.IsBackground = true;
            thd.Start();
        }

        public void LiveStop()
        {
            bLive = false;
        }

        private void Live()
        {
            while (bLive)
            {
                int i = Grab();
                if (i != 0) bLive = false;
                else
                {
                    /* int r = (640 * pf.BitsPerPixel + 7) / 8;
                     bmp = BitmapSource.Create(640, 480, 96, 96, pf, null, result, r);
                     bmp.Freeze();
                     Action action = delegate { image.Source = bmp; };
                     image.Dispatcher.Invoke(action);
 */
                    Thread.Sleep(LiveInterval);
                }
            }
        }
        private int CreateChannel(UInt32 ch)
        {

            hChannel0 = MultiCamChannelCreate(null, BoardIdentificator, ch);

            if (hChannel0 <= 0)
            {
                return 0;
            }
            int i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_ChannelState, 0);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_InitPause, 0);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_InitTrigger, 0);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_InitGrabSync, 0);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_InitGrabDelay, 0);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_RepeatPause, 0);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_RepeatTrigger, 0);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_RepeatGrabSync, 0);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_RepeatGrabDelay, 0);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_RepeatGrabCount, -1);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_TriggerMode, 0);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_AssemblerMask, 1);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_Standard, 3);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_Scanning, 0);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_Resolution, 0);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_FieldMode, 2);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_AcqColFmt, 0);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_VideoGain, 65536);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_VideoOffset, 0);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_VideoUGain, 65536);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_VideoVGain, 65536);
            i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_VideoHue, 0);
            //i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_ImageFlipY, 0);
            //i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_ImageFlipX, 0);
            IntPtr width = new IntPtr(0);
            IntPtr height = new IntPtr(0);
            MultiCamChannelGetParameterInt(hChannel0, EC_PARAM_ImageSizeX, out width);
            MultiCamChannelGetParameterInt(hChannel0, EC_PARAM_ImageSizeY, out height);

            SurfInfo.TYPE2 = 1;

            SurfInfo.SIZE = 640 * 480;
            SurfInfo.PITCH = 640;
            SurfInfo.UserAllocatedMemory = 1;
            int hSurface;
            //result = new byte[640 * 480];
            unsafe
            {
                fixed (byte* p = &result[0])
                {
                    SurfInfo.ADDRESS = p;
                }


            }
            int size = (4 * 5) + result.Length;
            SurfInfo.STRUCTSIZE = size;
            unsafe
            {

                fixed (t_SurfaceInfo* ptr = &SurfInfo)
                {
                    hSurface = MultiCamSurfaceCreate(ptr);

                    if (hSurface <= 0)
                    {
                        return 0;
                    }
                };

                int Status = MultiCamChannelAddSurface(hChannel0, hSurface);
                if (Status != EC_OK)
                {
                    return 0;
                }


                MultiCamSignalMask(EC_SIGNAL_SURFACE_AVAILABLE, 1);

                Status = MultiCamSystemAcquisitionStart();
                if (Status != EC_OK)
                {
                    return 0;
                }
            }
            return 1;
        }

        public int ChannelDestory()
        {
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_ChannelState, 0);
            MultiCamSystemAcquisitionStop();
            Thread.Sleep(200);
            MultiCamChannelDelete(hChannel0);
            MultiCamSurfaceDelete(hChannel0);
            return 1;

        }

        public int Grab()
        {
            int status = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_ChannelState, 0);
            if (status != EC_OK)
            {
                return 1;
            }
            status = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_RepeatGrabCount, 0);
            if (status != EC_OK)
            {
                return 2;
            }
            status = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_ChannelState, 1);
            if (status != EC_OK)
            {
                return 3;
            }
            status = MultiCamSignalWait(EC_SIGNAL_SURFACE_AVAILABLE);
            if (status != EC_OK)
            {
                return 4;
            }
            int r = (640 * pf.BitsPerPixel + 7) / 8;
            bmp = BitmapSource.Create(640, 480, 96, 96, pf, null, result, r);
            bmp.Freeze();
            Action action = delegate { image.Source = bmp; };
            image.Dispatcher.Invoke(action);
            return 0;

        }

        public void resultToAlign(string filename)
        {
            int sx = 270;
            int sy = 190;
            byte[,] tmp = new byte[480, 640];
            for (int j = 0; j < 480; j++)
            {
                for (int i = 0; i < 640; i++)
                {
                    tmp[j, i] = result[i + (j * 640)];
                }
            }
            int x = 0;
            for (int j = sy; j < (sy + 100); j++)
            {
                for (int i = sx; i < (sx + 100); i++)
                {
                    Align[j - sy, i - sx] = tmp[j, i];
                    align[x] = Align[j - sy, i - sx];
                    x++;
                }
            }
            int r = (100 * pf.BitsPerPixel + 7) / 8;
            AlignBmp = BitmapSource.Create(100, 100, 96, 96, pf, null, align, r);
            FileStream stream = new FileStream(filename, FileMode.Create);
            stream.Write(align, 0, align.Length);
            stream.Close();
        }
        public bool ImageToAlign(string filename)
        {
            FileInfo f = new FileInfo(filename);
            if (!f.Exists)
            {
                return false;
            }
            FileStream stream = new FileStream(filename, FileMode.Open);
            stream.Read(align, 0, align.Length);
            stream.Close();
            int r = (100 * pf.BitsPerPixel + 7) / 8;
            AlignBmp = BitmapSource.Create(100, 100, 96, 96, pf, null, align, r);
            for (int j = 0; j < 100; j++)
            {
                for (int i = 0; i < 100; i++)
                {
                    Align[j, i] = align[i + (j * 100)];
                }
            }
            return true;
        }

        public int SearchAlign(out int x, out int y, out int match)
        {
            int STEP = 3;
            int stpx = 400 / STEP;
            int stpy = 300 / STEP;
            int nMin = 1000000000;
            int resX = 0;
            int resY = 0;
            for (int j = 0; j < 480; j++)
            {
                for (int i = 0; i < 640; i++)
                {
                    tempResult[j, i] = result[i + (j * 640)];
                }
            }
            int nSum;
            for (int j = 0; j < stpy; j++)
            {
                for (int i = 0; i < stpx; i++)
                {
                    int a = (j * STEP) + 120;
                    int b = (i * STEP) + 40;
                    nSum = Correation((i * STEP), (j * STEP), STEP);
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
                    nSum = Correation(i, j, STEP);
                    if (nSum < nMin)
                    {
                        resX = i;
                        resY = j;
                        nMin = nSum;
                    }
                }
            }
            match = NCorreation(resX, resY);
            x = resX - 270;
            y = resY - 190;
            return 0;
        }
        private int Correation(int cx, int cy, int step)
        {
            int Coff = 0;
            int size = (640 * 480) / (step * step);
            int tCoff = size * 40;
            int w = 100 / step;
            int h = 100 / step;
            byte src;
            byte des;
            for (int j = 0; j < w; j++)
            {

                for (int i = 0; i < h; i++)
                {
                    src = Align[j * step, i * step];
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
        private int NCorreation(int x, int y)
        {
            int Num = 1;// 000;
            byte src;
            byte des;
            int a = 0;
            int b = 0;
            int c = 0;
            int d = 0;
            int e = 0;
            for (int j = 0; j < 100; j++)
            {
                for (int i = 0; i < 100; i++)
                {
                    src = Align[j, i];
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
