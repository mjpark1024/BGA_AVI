using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace DCS.Picolo
{
    public struct t_SurfaceInfo
    {
        public int STRUCTSIZE;
        public int TYPE2;
        public IntPtr ADDRESS;
        public int SIZE;
        public int PITCH;
        public int UserAllocatedMemory;
    }
    class PicoloControl
    {
        public const UInt32 MULTI_ID1 = 0;
        public const UInt32 MULTI_ID2 = 1;
        public const UInt32 MULTI_ID3 = 2;
        public const UInt32 MULTI__YC = 3;
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
        public static extern int MultiCamSurfaceCreate(ref t_SurfaceInfo pSurfaceInfo);
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
        public void Delay(int ms)
        {
            int time = Environment.TickCount;

            do
            {
                if (Environment.TickCount - time >= ms)
                    return;
            } while (true);
        }
        public int init()
        {
            MultiCamSystemAcquisitionStop();
            Delay(200);
            MultiCamSystemTerminate();
            int s = MultiCamSystemInitialize();
            if (s != EC_OK)
            { return 0; }
            else { return 1; }
        }

        public int CreateChannel(UInt32 ch)
        {
            //BitmapImage m_bitmapImage = new BitmapImage();

            hChannel0 = MultiCamChannelCreate(null, BoardIdentificator, ch);

            if (hChannel0 <= 0)
            {
                MessageBox.Show("Cannot Create Channel to board");
                return 0;
            }
            int i = MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_ChannelState, 0);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_InitPause, 0);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_InitTrigger, 0);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_InitGrabSync, 0);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_InitGrabDelay, 0);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_RepeatPause, 0);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_RepeatTrigger, 0);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_RepeatGrabSync, 0);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_RepeatGrabDelay, 0);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_RepeatGrabCount, -1);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_TriggerMode, 0);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_AssemblerMask, 1);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_Standard, 3);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_Scanning, 0);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_Resolution, 0);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_FieldMode, 2);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_AcqColFmt, 0);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_VideoGain, 65536);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_VideoOffset, 0);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_VideoUGain, 65536);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_VideoVGain, 65536);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_VideoHue, 0);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_ImageFlipY, 0);
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_ImageFlipX, 0);
            IntPtr width = new IntPtr(0);
            IntPtr height = new IntPtr(0);
            MultiCamChannelGetParameterInt(hChannel0, EC_PARAM_ImageSizeX, out width);
            MultiCamChannelGetParameterInt(hChannel0, EC_PARAM_ImageSizeY, out height);

            SurfInfo.STRUCTSIZE = 10000;
            SurfInfo.TYPE2 = 1;

            SurfInfo.SIZE = 640 * 480;
            SurfInfo.PITCH = 640;
            SurfInfo.UserAllocatedMemory = 1;
            // m_bitmapImage.BeginInit();
            //  m_bitmapImage.UriSource = new Uri(
            //  m_bitmapImage.EndInit();
            byte[,] bmp = new byte[640, 480];

            SurfInfo.ADDRESS = new IntPtr(bmp[0, 0]);
            int hSurface = MultiCamSurfaceCreate(ref SurfInfo);
            if (hSurface < 0)
            {
                MessageBox.Show("Err hSurpace");
                return 0;
            }

            int Status = MultiCamChannelAddSurface(hChannel0, hSurface);
            if (Status != EC_OK)
            {
                MessageBox.Show("Add Surfac");
                return 0;
            }

            MultiCamSignalMask(EC_SIGNAL_SURFACE_AVAILABLE, 1);

            Status = MultiCamSystemAcquisitionStart();
            if (Status != EC_OK)
            {
                MessageBox.Show("Cannot enable acquisition");
                return 0;
            }
            return 1;
        }

        public int ChannelDestory(UInt32 ch)
        {
            MultiCamChannelSetParameterInt(hChannel0, EC_PARAM_ChannelState, 0);
            MultiCamSystemAcquisitionStop();
            Delay(200);
            MultiCamChannelDelete(hChannel0);
            MultiCamSurfaceDelete(hChannel0);
            return 1;

        }

        public int Grab(UInt32 ch)
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
            return 0;

        }

        ~PicoloControl()
        {
            MultiCamSystemTerminate();
        }

    }
}
