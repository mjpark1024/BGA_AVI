/*********************************************************************************
 * Copyright(c) 2015 by Haesung DS.
 * 
 * This software is copyrighted by, and is the sole property of Haesung DS.
 * All rigths, title, ownership, or other interests in the software remain the
 * property of Haesung DS. This software may only be used in accordance with
 * the corresponding license agreement. Any unauthorized use, duplication, 
 * transmission, distribution, or disclosure of this software is expressly 
 * forbidden.
 *
 * This Copyright notice may not be removed or modified without prior written
 * consent of Haesung DS reserves the right to modify this 
 * software without notice.
 *
 * Haesung DS.
 * KOREA 
 * http://www.HaesungDS.com
 *********************************************************************************/

using Common;
using Common.Drawing;
using Common.Drawing.InspectionInformation;
using PCS.ModelTeaching;
using Sockets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace PCS
{
    public delegate void SendLogMessageEventHandler(string aszSubSystem, SeverityLevel aSeverityLevel, string aszLogMessage);
    public delegate void LiveImageUpdateHandler(int anIndex);
    public delegate void InspectDoneEventHandler(int ID, int side);
    public delegate void ReceiveUnitOffsetEventHandler(int ID, int side, List<System.Windows.Point> Offset);

    public class VID
    {
        public static int BP1 = 0;
        public static int CA1 = 1;     
        public static int BA1 = 3;
        public static int BP_Count = 1;
        public static int CA_Count = 2;
        public static int BA_Count = 2;

        public static int BP_ScanComplete_Count = 2; // hs - 1Scan 2SWOT
        public static int BP_PC_Count = 1; // hs - PC 대수
        public static int CA_PC_Count = 2;
        public static int BA_PC_Count = 2;

        public static int ALL = 5;
        public VID(int BP, int CA, int BA, int BP_PC_COUNT, int CA_PC_COUNT, int BA_PC_COUNT)
        {
            BP_Count = BP;
            CA_Count = CA;
            BA_Count = BA;
            BP_PC_Count = BP_PC_COUNT;
            CA_PC_Count = CA_PC_COUNT;
            BA_PC_Count = BA_PC_COUNT;
            BP1 = 0;
            CA1 = BP_PC_Count;
            BA1 = BP_PC_Count + CA_PC_Count;
            if (BP_PC_Count > BP_Count) BP_PC_Count = BP_Count;
            if (CA_PC_Count > CA_Count) CA_PC_Count = CA_Count;
            if (BA_PC_Count > BA_Count) BA_PC_Count = BA_Count; 
            ALL = BP_PC_COUNT + CA_PC_COUNT + BA_PC_COUNT;
        }

        public static int CalcIndex(int nIndex)
        {
            if (nIndex < 0) return -1;
            int FrameGrabberIndex = 0;
            if (nIndex < VID.CA1)
            {
                FrameGrabberIndex = (nIndex - VID.BP1) % 2 + 0;////////Setting INI BP 시작 Index 0.
            }
            else if (nIndex >= VID.CA1 && nIndex < VID.BA1)
            {
                FrameGrabberIndex = (nIndex - VID.CA1) % 2 + 1;////////Setting INI CA 시작 Index 1.
            }
            else
            {
                FrameGrabberIndex = (nIndex - VID.BA1) % 2 + 3;////////Setting INI BA 시작 Index 2.
            }
            return FrameGrabberIndex;
        }
    }
    public class CID
    {
        public const int BP = 0;
        public const int CA = 1;
        public const int BA = 2;
        public const int ALL = 3;
    }

    public class GrabberType
    {
        public const int File = 0;
        public const int Sapera = 1;
        public const int Multicam = 2;
        public const int Mil = 3;
        public const int CXP = 4;
        public const int CLHS = 5;
        public const int CLHS_RGB = 6;
        public const int File_RGB = 7;
    }

    /// <summary>   Is interface.  </summary>
    public class VisionInterface
    {
        public static event SendLogMessageEventHandler SendLogMessageEvent;
        public static event LiveImageUpdateHandler LiveImageUpdateEvent;
        public static event InspectDoneEventHandler InspectDone;
        public static event ReceiveUnitOffsetEventHandler ReceiveUnitOffset;
        public ClientSyncSocket SyncSocket;
        public bool Connected { get { return SyncSocket.IsConnected; } }

        public ResultInfo VisionResult = new ResultInfo();
        public PSRResults PSRResult;
        public VisionInformation VisionInfo = new VisionInformation();

        public int ID { get; set; }
        public bool UseRGB { get; set; }
        private int m_nReceiveDataWidth;
        private int m_nReceiveDataHeight;
        private int m_nSectionReceiveDataWidth;
        private int m_nSectionReceiveDataHeight;

        private int m_nReceiveDataWidth_COLOR;
        private int m_nReceiveDataHeight_COLOR;

        private int m_nReceiveCornerDataWidth;
        private int m_nReceiveCornerDataHeight;

        public bool AutoInspect;
        public bool GetLiveImage = false;
        public bool Grab_Done;
        // public bool Light_Done;
        public bool Register_Done;
        public bool Register_Fail;
        public int Register_Fail_Algo;
        public bool Grab_Ready;
        public bool Inspect_Done;
        public bool Recv_CenterLine_Done;
        public bool Recv_Ball_Done;

        public int m_iTrainError;
        public int DefectCount;
        public int PSRShiftCount;

        // 전송률 표시 (현재 제거된 인터페이스)
        // public int ProgressRate;

        public int m_nReqSecID;      // 검사시 INSPECT_DONE 확인을 위한 INSPECT 요청값
        public int m_nReqUnitX;
        public int m_nReqUnitY;
        public int m_nLastSecID;     // 최근에 Vision으로부터 요청한 Sec ID, X, Y
        public int m_nLastUnitX;
        public int m_nLastUnitY;

        public int m_nMergedImageWidth;
        public int m_nMergedImageHeight;
        public int m_nMergedImageHorResCnt;
        public int m_nMergedImageLimitCnt;

        private int m_nResultImageWidth;
        private int m_nResultImageHeight;
        private int m_nHistoLength;

        public bool MergedImageReady;
        public bool MergedImageInfoReady;
        public bool RefImageReady;
        public bool DefImageReady;
        public bool ProfileSent;

        private byte[] m_recvImagePixels; // 전체영상 or 섹션영상 buffer.
        private byte[] m_mergedImagePixels; // 불량머지영상 buffer.
        private byte[] m_refImagePixels; // 기준영상 buffer.
        private byte[] m_defImagePixels; // 불량영상 buffer.
        private byte[] m_recvImagePixelsR; // 전체영상 or 섹션영상 buffer.
        private byte[] m_recvImagePixelsG; // 전체영상 or 섹션영상 buffer.
        private byte[] m_recvImagePixelsB; // 전체영상 or 섹션영상 buffer.
        private byte[] m_recvCornerImagePixels; // Corner영상 buffer.

        private byte[] m_recvImagePixels_Color; // 전체영상 or 섹션영상 buffer.
        private byte[] m_mergedImagePixels_Color; // 불량머지영상 buffer. - Color
        private byte[] m_refImagePixels_Color; // 기준영상 buffer. - Color
        private byte[] m_defImagePixels_Color; // 불량영상 buffer. - Color
        private byte[] m_recvCornerImagePixels_Color; // Corner영상 buffer.


        private string m_strIP;
        private int m_nPort;
        private bool full = false;

        public GraphicsSkeletonLine[] LineSegments; // 중앙선

        public GraphicsSkeletonBall[] BallSegments; // 중앙선

        // for calculate real X, Y
        public ObservableCollection<SectionInformation> Sections { get; set; }

        private int m_nCornerImageWidth;
        private int m_nCornerImageHeight;

        public bool m_bUseAI = false;

        public VisionInterface(int anID, bool abUseRGB)
        {
            ID = anID;
            UseRGB = abUseRGB;
            SyncSocket = new ClientSyncSocket(ID);
            SyncSocket.OnConnect += OnConnect;
            SyncSocket.OnClose += OnClose;
            SyncSocket.OnSend += OnSend;
            SyncSocket.OnReceive += OnReceive;
            SyncSocket.OnError += OnError;
            SyncSocket.OnDisconnect += OnDisconnect;
        }

        #region Image getter
        public BitmapSource BasedImage
        {
            get
            {
                return GetIndexed8BitmapSource(m_recvImagePixels, m_nReceiveDataWidth, m_nReceiveDataHeight);
            }
        }

        public BitmapSource BasedImageR
        {
            get
            {
                return GetIndexed8BitmapSource(m_recvImagePixelsR, m_nReceiveDataWidth, m_nReceiveDataHeight);
            }
        }

        public BitmapSource BasedImageG
        {
            get
            {
                return GetIndexed8BitmapSource(m_recvImagePixelsG, m_nReceiveDataWidth, m_nReceiveDataHeight);
            }
        }
        public BitmapSource BasedImageB
        {
            get
            {
                return GetIndexed8BitmapSource(m_recvImagePixelsB, m_nReceiveDataWidth, m_nReceiveDataHeight);
            }
        }

        public BitmapSource BasedImageRGB
        {
            get
            {
                return GetRGB24BitmapSource(m_recvImagePixelsR, m_recvImagePixelsG, m_recvImagePixelsB, m_nReceiveDataWidth, m_nReceiveDataHeight);
            }
        }

        public BitmapSource SectionImage
        {
            get
            {
                return GetIndexed8BitmapSource(m_recvImagePixels, m_nSectionReceiveDataWidth, m_nSectionReceiveDataHeight);
            }
        }

        public BitmapSource MergedImage
        {
            get
            {
                return GetIndexed8BitmapSource(m_mergedImagePixels, m_nResultImageWidth, m_nResultImageHeight);
            }
        }

        public BitmapSource RefImage
        {
            get
            {
                return GetIndexed8BitmapSource(m_refImagePixels, m_nResultImageWidth, m_nResultImageHeight);
            }
        }

        public BitmapSource DefImage
        {
            get
            {
                return GetIndexed8BitmapSource(m_defImagePixels, m_nResultImageWidth, m_nResultImageHeight);
            }
        }

        public BitmapSource SectionImage_Color
        {
            get
            {
                return GetIndexed32BitmapSource(m_recvImagePixels_Color, m_nReceiveDataWidth_COLOR, m_nReceiveDataHeight_COLOR);
            }
        }

        public BitmapSource MergedImage_Color
        {
            get
            {
                return GetIndexed32BitmapSource(m_mergedImagePixels_Color, m_nResultImageWidth, m_nResultImageHeight);
            }
        }

        public BitmapSource RefImage_Color
        {
            get
            {
                return GetIndexed32BitmapSource(m_refImagePixels_Color, m_nResultImageWidth, m_nResultImageHeight);
            }
        }

        public BitmapSource DefImage_Color
        {
            get
            {
                return GetIndexed32BitmapSource(m_defImagePixels_Color, m_nResultImageWidth, m_nResultImageHeight);
            }
        }

        public BitmapSource BaseCornerImage
        {
            get
            {
                return GetIndexed8BitmapSource(m_recvCornerImagePixels, m_nReceiveCornerDataWidth, m_nReceiveCornerDataHeight);
            }
        }
        public BitmapSource BaseCornerImage_Color
        {
            get
            {
                return GetIndexed32BitmapSource(m_recvCornerImagePixels_Color, m_nReceiveCornerDataWidth, m_nReceiveCornerDataHeight);
            }
        }

        public static BitmapSource GetRGB24BitmapSource(byte[] pixelsR, byte[] pixelsG, byte[] pixelsB, int pixelWidth, int pixelHeight)
        {
            if (pixelsR == null || pixelsG == null || pixelsG == null)
                return null;
            byte[] pixels = new byte[pixelWidth * pixelHeight * 3];

            for (int i = 0; i < pixelWidth * pixelHeight; i++)
            {
                pixels[i * 3 + 0] = pixelsR[i];
                pixels[i * 3 + 1] = pixelsG[i];
                pixels[i * 3 + 2] = pixelsB[i];
            }
            System.Windows.Media.PixelFormat pf = System.Windows.Media.PixelFormats.Rgb24;
            int stride = (pixelWidth * pf.BitsPerPixel + 7) / 8;
            List<System.Windows.Media.Color> Colors = new List<System.Windows.Media.Color>();
            Colors.Add(System.Windows.Media.Colors.Red);
            Colors.Add(System.Windows.Media.Colors.Green);
            Colors.Add(System.Windows.Media.Colors.Blue);
            BitmapPalette palette = new BitmapPalette(Colors);
            BitmapSource bitmapSource = BitmapSource.Create(pixelWidth, pixelHeight, 96, 96, pf, palette, pixels, stride);

            if (bitmapSource != null)
            {
                return bitmapSource;
            }
            else
            {
                return null;
            }
        }

        public static BitmapSource GetIndexed8BitmapSource(byte[] pixels, int pixelWidth, int pixelHeight)
        {
            if (pixels == null)
                return null;

            System.Windows.Media.PixelFormat pf = System.Windows.Media.PixelFormats.Indexed8;
            BitmapPalette palette = BitmapPalettes.Gray256;
            BitmapSource bitmapSource = BitmapSource.Create(pixelWidth, pixelHeight, 96, 96, pf, palette, pixels, pixelWidth);

            
            if (bitmapSource != null)
            {
                return bitmapSource;
            }
            else
            {
                return null;
            }
        }

        public static BitmapSource GetIndexed32BitmapSource(byte[] pixels, int pixelWidth, int pixelHeight)
        {
            if (pixels == null)
                return null;

            System.Windows.Media.PixelFormat pf = System.Windows.Media.PixelFormats.Bgr24;

            BitmapSource bitmapSource = BitmapSource.Create(pixelWidth, pixelHeight, 96, 96, pf, null, pixels, pixelWidth * 3);

            if (bitmapSource != null)
            {
                return bitmapSource;
            }
            else
            {
                return null;
            }
        }

        public static System.Drawing.Bitmap ByteArrayToBitmap(byte[] pixels, int pixelWidth, int pixelHeight)
        {
            if (pixels == null)
                return null;

            System.Windows.Media.PixelFormat pf = System.Windows.Media.PixelFormats.Indexed8;
            BitmapPalette palette = BitmapPalettes.Gray256;
            BitmapSource bitmapSource = BitmapSource.Create(pixelWidth, pixelHeight, 96, 96, pf, palette, pixels, pixelWidth);


            if (bitmapSource != null)
            {
                return BitmapHelper.BitmapSource2Bitmap(bitmapSource, 1);
            }
            else
            {
                return null;
            }
        }

        public static System.Drawing.Bitmap GetBitmap(BitmapSource source)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(
              source.PixelWidth,
              source.PixelHeight,
              System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(
              new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), bmp.Size),
              System.Drawing.Imaging.ImageLockMode.WriteOnly,
              System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            bmp.Save("d:\\aaaaaa.bmp");
            return bmp;
        }
        #endregion

        #region OnSend, OnClose, Connect, DisConnect
        private void OnSend(object sender, SocketSendEventArgs e)
        {
            Debug.WriteLine(String.Format("###Debug### {1} Bytes sent. Send ID:{0}. (OnSend called.)", e.ID, e.SendBytes));
        }

        private void OnError(object sender, SocketErrorEventArgs e)
        {
            SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            if (sendRunner != null)
                sendRunner("IS", SeverityLevel.WARN, string.Format("Vision 전송 오류. 오류 메시지 {0}", e.AsyncSocketException));

            Debug.WriteLine(String.Format("###Warning### SocketException occured. Error ID:{0}, Error Message:{1}. (OnError called.)", e.ID, e.AsyncSocketException));
        }

        private void OnClose(object sender, SocketConnectionEventArgs e)
        {
            SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            if (sendRunner != null)
                sendRunner("IS", SeverityLevel.DEBUG, string.Format("Vision 연결이 종료되었습니다. IP:{0} PORT:{1}", m_strIP, m_nPort));

            Debug.WriteLine(String.Format("###Warning### Socket Closed. Closed ID:{0}. (OnClose called.)", e.ID));
        }

        private void OnConnect(object sender, SocketConnectionEventArgs e)
        {
            SendPacket(VisionDefinition.PING);
        }

        public bool Connect(string IP, int port)
        {
            try
            {
                m_strIP = IP;
                m_nPort = port;
                SyncSocket.ID = port;

                if (!SyncSocket.Connect(IP, port))
                {
                    SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
                    if (sendRunner != null)
                        sendRunner("IS", SeverityLevel.WARN, string.Format("Vision Connection 실패하였습니다. IP:{0} PORT:{1}", IP, port));
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void OnDisconnect(object sender, SocketConnectionEventArgs e)
        {
            SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            if (sendRunner != null)
                sendRunner("IS", SeverityLevel.WARN, string.Format("Vision 연결이 끊어졌습니다. IP:{0} PORT:{1}", m_strIP, m_nPort));

            Debug.WriteLine(String.Format("###Warning### Socket Disconnected. Disconnected IP:{0} PORT:{1}. (OnDisconnect called.)", m_strIP, m_nPort));
            DisConnect();
        }

        public void DisConnect()
        {
            SyncSocket.Close();
        }
        #endregion

        #region Vision.

        public void InitVision(int Type, int Width, int Height, int Depth, int GrabCount, int DeviceNum, string DeviceName, string CamFile, int CropHeight, int PageDelay, double Resolution, double Master, int rgbindex = 0, int maxLimitDefect = 64)
        {
            if (Type < 0) Type = 0;
            if (Width < 0) Width = 8192;
            if (Height < 0) Height = 20000;
            if (GrabCount < 1) GrabCount = 4;
            if (DeviceNum < 0) DeviceNum = 0;
            if (string.IsNullOrEmpty(DeviceName)) DeviceName = "System";
            if (string.IsNullOrEmpty(CamFile)) CamFile = "C:\\";
            if (CropHeight < 0) CropHeight = 0;
            if (PageDelay < 0) PageDelay = 0;
            VisionInfo.SetVisionInfo(Type, Width, Height, Depth, GrabCount, DeviceNum, DeviceName, CamFile, CropHeight, PageDelay, Resolution, Master, rgbindex, maxLimitDefect);

            if (Connected)
            {
                SendPacket(VisionDefinition.INIT_VISION, VisionParameter.VisionInfoSize, VisionInfo.GetVisionInfo());
            }
        }

        public void SetInspectMode(int anPSRShiftType, int anGrabType)
        {
            byte[] pPacket = new byte[sizeof(int) * 2];

            byte[] psrType = BitConverter.GetBytes(anPSRShiftType);
            byte[] grabType = BitConverter.GetBytes(anGrabType);

            psrType.CopyTo(pPacket, sizeof(int) * 0);
            grabType.CopyTo(pPacket, sizeof(int) * 1);

            SendPacket(VisionDefinition.SET_INSP_MODE, sizeof(int) * 2, pPacket);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("SetInspectMode"));
        }

        public void SetGain(float afRed, float afGreen, float afBlue, float afStrength, bool color)
        {
           // if (color)
            {
                byte[] pPacket = new byte[sizeof(float) * 3];

                byte[] arrR = BitConverter.GetBytes(afRed);
                byte[] arrG = BitConverter.GetBytes(afGreen);
                byte[] arrB = BitConverter.GetBytes(afBlue);

                arrR.CopyTo(pPacket, sizeof(float) * 0);
                arrG.CopyTo(pPacket, sizeof(float) * 1);
                arrB.CopyTo(pPacket, sizeof(float) * 2);
                SendPacket(VisionDefinition.SET_GAIN, sizeof(float) * 3, pPacket);
            }
           // else
            {
                byte[] pPacket = new byte[sizeof(float)];

                byte[] arrSR = BitConverter.GetBytes(afStrength);

                arrSR.CopyTo(pPacket, sizeof(float) * 0);
                SendPacket(VisionDefinition.SET_STRENGTH, sizeof(float) * 1, pPacket);
            }
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("SetGain"));
        }

        public void ClearVisionData()
        {
            Grab_Done = false;
            Grab_Ready = false;
            Register_Done = false;
            Inspect_Done = false;

            DefectCount = 0;
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("ClearVisionData"));
        }

        public void ClearVision()
        {
            SendPacket(VisionDefinition.CLEAR_VISION);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("ClearVision"));
        }

        public void AddSection(SectionInformation secInfo)
        {
            // Section Type 0,1,2 : Global Fidu, Unit, Rail
            SendPacket(VisionDefinition.ADD_SECTION, VisionParameter.SectionSize, VisionParameter.SectionToBytes(secInfo));
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("AddSection"));
        }

        public void ReqREQ_ICS_OFFSET(int GrabSide)
        {
            if ((this.ID < VID.BP_PC_Count && GrabSide == 1) || !m_bUseAI) return;
            byte[] arrResult = new Byte[4];
            BitConverter.GetBytes(GrabSide).CopyTo(arrResult, 0);
            SendPacket(VisionDefinition.REQ_ICS_OFFSET, arrResult.Length, arrResult);
        }
        public void SendtoICS_UnitPos(List<Point> UnitPos, int GrabSide)
        {
            SendPacket(VisionDefinition.ADD_UNITS_ICS, VisionParameter.ICS_UnitPos, VisionParameter.ICS_UNITToBytes(UnitPos, GrabSide));
        }
        public void ReqSection(int anSecID, int anSecX, int anSecY, int anUseOffset, bool isrgb = false)
        {
            byte[] pPacket = new byte[sizeof(int) * 4];

            byte[] arrSecID = BitConverter.GetBytes(anSecID);
            byte[] arrSecX = BitConverter.GetBytes(anSecX);
            byte[] arrSecY = BitConverter.GetBytes(anSecY);
            byte[] arrUseOffset = BitConverter.GetBytes(anUseOffset);

            arrSecID.CopyTo(pPacket, sizeof(int) * 0);
            arrSecX.CopyTo(pPacket, sizeof(int) * 1);
            arrSecY.CopyTo(pPacket, sizeof(int) * 2);
            arrUseOffset.CopyTo(pPacket, sizeof(int) * 3);

            Grab_Done = false;

            if (isrgb)
                SendPacket(VisionDefinition.REQ_SECTION_COLOR, sizeof(int) * 4, pPacket);
            else
                SendPacket(VisionDefinition.REQ_SECTION, sizeof(int) * 4, pPacket);

            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //     sendRunner("IS", SeverityLevel.WARN, string.Format("ReqSection"));
        }

        public void ReqSectionRGB(int anSecID, int anSecX, int anSecY, int anUseOffset, int anChannel, int nDesvision = 0)
        {
            int grabside = nDesvision;
            //if (nDesvision == (VID.BP1 + 1))
            //    grabside = 1;

            byte[] pPacket = new byte[sizeof(int) * 6];

            byte[] arrSecID = BitConverter.GetBytes(anSecID);
            byte[] arrSecX = BitConverter.GetBytes(anSecX);
            byte[] arrSecY = BitConverter.GetBytes(anSecY);
            byte[] arrUseOffset = BitConverter.GetBytes(anUseOffset);
            byte[] arrChannel = BitConverter.GetBytes(anChannel);
            byte[] arrgrabside = BitConverter.GetBytes(grabside);

            arrSecID.CopyTo(pPacket, sizeof(int) * 0);
            arrSecX.CopyTo(pPacket, sizeof(int) * 1);
            arrSecY.CopyTo(pPacket, sizeof(int) * 2);
            arrUseOffset.CopyTo(pPacket, sizeof(int) * 3);
            arrChannel.CopyTo(pPacket, sizeof(int) * 4);
            arrgrabside.CopyTo(pPacket, sizeof(int) * 5);

            Grab_Done = false;
            SendPacket(VisionDefinition.REQ_SECTION, sizeof(int) * 6, pPacket);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("ReqSectionRGB"));
        }

        public void SetStatusLogLevel(int nLevel)
        {
            byte[] aLevel = BitConverter.GetBytes(nLevel);
            SendPacket(VisionDefinition.SET_STATUS_LOG_LEVEL, 1, aLevel);
        }

        public void SetErrorLogLevel(int nLevel)
        {
            byte[] aLevel = BitConverter.GetBytes(nLevel);
            SendPacket(VisionDefinition.SET_ERROR_LOG_LEVEL, 1, aLevel);
        }

        public void SetPageDelay(int nDelay)
        {
            byte[] aDelay = BitConverter.GetBytes(nDelay);
            SendPacket(VisionDefinition.SET_PAGE_DELAY, 1, aDelay);
        }

        public void Grab(int nGrab, bool full, int grabside = 0)
        {
            this.full = full;
           
            byte[] arrParam = new byte[sizeof(int) * 2];
            BitConverter.GetBytes(nGrab).CopyTo(arrParam, 0 * sizeof(int));
            BitConverter.GetBytes(grabside).CopyTo(arrParam, 1 * sizeof(int));
            Grab_Done = false;
            SendPacket(VisionDefinition.GRAB, sizeof(int) * 2, arrParam);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("Vision 전송 Grab {0}", grabside));
        }

        public void ReqGrabImage()
        {
            double scale = VisionDefinition.GRAB_IMAGE_SCALE;
            if (full) scale = 1.0;
            byte[] arrParam = new byte[sizeof(int) * 1 + sizeof(double) * 1];
            BitConverter.GetBytes(scale).CopyTo(arrParam, 1 * sizeof(int));
            Grab_Done = false;
            SendPacket(VisionDefinition.REQ_GRAB_IMAGE, (sizeof(int) * 1 + sizeof(double) * 1), arrParam);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("ReqGrabImage"));
        }

        public void ReqGrabImageRGB()
        {
            double scale = VisionDefinition.GRAB_IMAGE_SCALE;
            if (full) scale = 1.0;
            byte[] arrParam = new byte[sizeof(int) * 1 + sizeof(double) * 1];
            BitConverter.GetBytes(scale).CopyTo(arrParam, 1 * sizeof(int));
            Grab_Done = false;
            SendPacket(VisionDefinition.REQ_GRAB_IMAGE_RGB, (sizeof(int) * 1 + sizeof(double) * 1), arrParam);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("ReqGrabImageRGB"));
        }

        public void SetBaseCornerImageInfo(Size Size)
        {
            m_nCornerImageWidth = Convert.ToInt32(Size.Width);
            m_nCornerImageHeight = Convert.ToInt32(Size.Height);
        }
        public void ReqCornerGrabImage()
        {
            if (!m_bUseAI) return;
            int Width = m_nCornerImageWidth;
            int Height = m_nCornerImageHeight;
            byte[] arrParam = new byte[sizeof(int) * 1 + sizeof(int) * 2];
            BitConverter.GetBytes(Width).CopyTo(arrParam, 1 * sizeof(int));
            BitConverter.GetBytes(Height).CopyTo(arrParam, 2 * sizeof(int));
            Grab_Done = false;
            //if(LeftRight == 0)
            //    SendPacket(VisionDefinition.REQ_CORNER_IMAGE_Left, (sizeof(int) * 1 + sizeof(int) * 2), arrParam);
            //else
            //    SendPacket(VisionDefinition.REQ_CORNER_IMAGE_Right, (sizeof(int) * 1 + sizeof(int) * 2), arrParam);
            SendPacket(VisionDefinition.REQ_CORNER_IMAGE, (sizeof(int) * 1 + sizeof(int) * 2), arrParam);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("ReqGrabImage"));
        }
        //public void ReqCornerGrabImageRGB()
        //{
        //    int Width = m_nCornerImageWidth;
        //    int Height = m_nCornerImageHeight;
        //    byte[] arrParam = new byte[sizeof(int) * 1 + sizeof(int) * 2];
        //    BitConverter.GetBytes(Width).CopyTo(arrParam, 1 * sizeof(int));
        //    BitConverter.GetBytes(Height).CopyTo(arrParam, 2 * sizeof(int));
        //    Grab_Done = false;
        //    SendPacket(VisionDefinition.REQ_CORNER_IMAGE, (sizeof(int) * 1 + sizeof(int) * 2), arrParam);
        //    //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
        //    //if (sendRunner != null)
        //    //    sendRunner("IS", SeverityLevel.WARN, string.Format("ReqGrabImageRGB"));
        //}

        public void ClearModelInfo()
        {
            SendPacket(VisionDefinition.CLEAR_INSP_ITEMS);
        }

        public void SendImageData(int anSendCode, int anSectionID, BitmapSource abmpSectionImage, int anUnitX = -1, int anUnitY = -1, int colorindex = 0)
        {
            if ((anSendCode == VisionDefinition.SEND_SEC_IMAGE) && ((anUnitX < 0) || (anUnitY < 0)))
                return;

            int nParamSize = (anSendCode == VisionDefinition.SEND_SEC_IMAGE) ? 8 : 6;
            int nWidth = abmpSectionImage.PixelWidth;
            int nHeight = abmpSectionImage.PixelHeight;

            int nTotalSize = nWidth * nHeight;

            if (anSendCode == VisionDefinition.SEND_REF_IMAGE_COLOR)
                nTotalSize *= 3;

            byte[] arrTotalImage = new byte[nTotalSize];
            byte[] arrSendData = new byte[sizeof(int) * nParamSize + VisionDefinition.IMAGE_PACKET_SIZE];

            if (anSendCode != VisionDefinition.SEND_REF_IMAGE_COLOR)
                abmpSectionImage.CopyPixels(arrTotalImage, abmpSectionImage.PixelWidth, 0);
            else
                abmpSectionImage.CopyPixels(arrTotalImage, abmpSectionImage.PixelWidth * 3, 0);

            BitConverter.GetBytes(anSectionID).CopyTo(arrSendData, 0);
            if (anSendCode == VisionDefinition.SEND_SEC_IMAGE)
            {
                BitConverter.GetBytes(anUnitX).CopyTo(arrSendData, sizeof(int) * (nParamSize - 7));
                BitConverter.GetBytes(anUnitY).CopyTo(arrSendData, sizeof(int) * (nParamSize - 6));
            }

            BitConverter.GetBytes(nWidth).CopyTo(arrSendData, sizeof(int) * (nParamSize - 5));
            BitConverter.GetBytes(nHeight).CopyTo(arrSendData, sizeof(int) * (nParamSize - 4));

            int nCount = nTotalSize / VisionDefinition.IMAGE_PACKET_SIZE;
            int nRemainder = nTotalSize % VisionDefinition.IMAGE_PACKET_SIZE;

            for (int i = 0; i < nCount; i++)
            {
                BitConverter.GetBytes(colorindex).CopyTo(arrSendData, sizeof(int) * (nParamSize - 3));//color index
                BitConverter.GetBytes(i * VisionDefinition.IMAGE_PACKET_SIZE).CopyTo(arrSendData, sizeof(int) * (nParamSize - 2));
                BitConverter.GetBytes((i + 1) * VisionDefinition.IMAGE_PACKET_SIZE).CopyTo(arrSendData, sizeof(int) * (nParamSize - 1));
                Array.Copy(arrTotalImage, i * VisionDefinition.IMAGE_PACKET_SIZE, arrSendData, sizeof(int) * nParamSize, VisionDefinition.IMAGE_PACKET_SIZE);
                SendPacket(anSendCode, sizeof(int) * nParamSize + VisionDefinition.IMAGE_PACKET_SIZE, arrSendData);
                Thread.Sleep(50);
            }
            if (nRemainder != 0)
            {
                BitConverter.GetBytes(colorindex).CopyTo(arrSendData, sizeof(int) * (nParamSize - 3));//color index
                BitConverter.GetBytes(nCount * VisionDefinition.IMAGE_PACKET_SIZE).CopyTo(arrSendData, sizeof(int) * (nParamSize - 2));
                BitConverter.GetBytes(nCount * VisionDefinition.IMAGE_PACKET_SIZE + nRemainder).CopyTo(arrSendData, sizeof(int) * (nParamSize - 1));
                Array.Copy(arrTotalImage, nCount * VisionDefinition.IMAGE_PACKET_SIZE, arrSendData, sizeof(int) * nParamSize, nRemainder);
                SendPacket(anSendCode, sizeof(int) * nParamSize + nRemainder, arrSendData);
                Thread.Sleep(50);
            }
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("SendImageData"));
        }

        public void AddROI(RoiInfo roiinfo, int anChannel)
        {
            SendPacket(VisionDefinition.ADD_ROI, VisionParameter.RoiSize, VisionParameter.RoiToBytes(roiinfo, anChannel));
        }

        public void AddStripAlign(StripAlignInfo info)
        {
            SendPacket(VisionDefinition.ADD_STRIPALIGN, VisionParameter.StripAlignSize, VisionParameter.StripAlignToBytes(info));
        }

        // 2012-08-06 suoow2 Added; 중앙선 데이터 요청
        public void ReqCenterLineInfo(int anInspID)
        {
            byte[] arrParam = new byte[4];
            BitConverter.GetBytes(anInspID).CopyTo(arrParam, 0);

            Recv_CenterLine_Done = false;

            SendPacket(VisionDefinition.REQ_CENTER_LINE_INFO, arrParam.Length, arrParam);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("ReqCenterLineInfo"));
        }

        // 2017-07-26 suoow2 Added; Ball 데이터 요청
        public void ReqBallInfo(int anInspID)
        {
            byte[] arrParam = new byte[4];
            BitConverter.GetBytes(anInspID).CopyTo(arrParam, 0);

            Recv_Ball_Done = false;

            SendPacket(VisionDefinition.REQ_BALL_INFO, arrParam.Length, arrParam);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("ReqBallInfo"));
        }

        // 2012-08-06 suoow2 Added; 중앙선 전송
        public void SendCenterLineInfo(int anInspID, GraphicsSkeletonLine[] lineSegments, int anChannel)
        {
            // sizeof(CenterPoint) : 16 bytes
            // sizeof(LineSegment) : 65536 bytes

            //Packet Size 문제로 계산하여 보냄 수정
            //2018.04.12

            if (lineSegments == null) return;
            int sizeOfLineSegment = 0;
            int lineSegmentCount = lineSegments.Length;
            for (int i = 0; i < lineSegmentCount; i++)
            {
                sizeOfLineSegment += 17;
                if (lineSegments[i].Nodes != null)
                {
                    sizeOfLineSegment += 4;
                    for (int n = 0; n < lineSegments[i].Nodes.Length; n++)
                    {
                        sizeOfLineSegment += 10;
                    }
                }
            }


            byte[] arrParam = new byte[8 + sizeOfLineSegment + 10];
            BitConverter.GetBytes(anInspID).CopyTo(arrParam, 0);
            BitConverter.GetBytes(lineSegmentCount).CopyTo(arrParam, 4);

            int nPacketPos = 8;
            for (int i = 0; i < lineSegmentCount; i++)
            {
                BitConverter.GetBytes(i).CopyTo(arrParam, nPacketPos);
                BitConverter.GetBytes(lineSegments[i].LineDir).CopyTo(arrParam, nPacketPos + 4);
                BitConverter.GetBytes(lineSegments[i].MedianSize).CopyTo(arrParam, nPacketPos + 5);
                BitConverter.GetBytes(lineSegments[i].BoundaryRect.X).CopyTo(arrParam, nPacketPos + 9);
                BitConverter.GetBytes(lineSegments[i].BoundaryRect.Y).CopyTo(arrParam, nPacketPos + 11);
                BitConverter.GetBytes(lineSegments[i].BoundaryRect.Width).CopyTo(arrParam, nPacketPos + 13);
                BitConverter.GetBytes(lineSegments[i].BoundaryRect.Height).CopyTo(arrParam, nPacketPos + 15);
                if (lineSegments[i].Nodes != null)
                {
                    BitConverter.GetBytes(lineSegments[i].Nodes.Length).CopyTo(arrParam, nPacketPos + 17);

                    nPacketPos += 21;
                    for (int n = 0; n < lineSegments[i].Nodes.Length; n++)
                    {
                        BitConverter.GetBytes(lineSegments[i].Nodes[n].LineDirection).CopyTo(arrParam, nPacketPos);
                        BitConverter.GetBytes(lineSegments[i].Nodes[n].Position.X).CopyTo(arrParam, nPacketPos + 1);
                        BitConverter.GetBytes(lineSegments[i].Nodes[n].Position.Y).CopyTo(arrParam, nPacketPos + 3);
                        BitConverter.GetBytes(lineSegments[i].Nodes[n].MeasureSize).CopyTo(arrParam, nPacketPos + 5);
                        BitConverter.GetBytes(lineSegments[i].Nodes[n].Type).CopyTo(arrParam, nPacketPos + 9);

                        nPacketPos += 10;
                    }
                }
            }
            SendPacket(VisionDefinition.SEND_CENTER_LINE_INFO, arrParam.Length, arrParam);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("SendCenterLineInfo"));
        }

        // suoow2, 2014-10-12. : 수동 검사
        public void ManualInspection(int reqSecID, int reqUnitX, int reqUnitY, int grabside, bool Merge)
        {
            Inspect_Done = false;

            SendPacket(VisionDefinition.CLEAR_INSP_RESULTS);

            m_nReqSecID = reqSecID;
            m_nReqUnitX = reqUnitX;
            m_nReqUnitY = reqUnitY;
            byte[] arrParam = new byte[sizeof(int) * 5];

            BitConverter.GetBytes(m_nReqSecID).CopyTo(arrParam, 0 * sizeof(int));
            BitConverter.GetBytes(m_nReqUnitX).CopyTo(arrParam, 1 * sizeof(int));
            BitConverter.GetBytes(m_nReqUnitY).CopyTo(arrParam, 2 * sizeof(int));
            BitConverter.GetBytes(grabside).CopyTo(arrParam, 3 * sizeof(int));
            BitConverter.GetBytes(Merge?1:0).CopyTo(arrParam, 4 * sizeof(int));

            //Merge는 PSR 하지이물에 대해서만 동작하며, PSR 불량의 중복을 제거하는 역할이다 - PKS

            SendPacket(VisionDefinition.INSPECT, sizeof(int) * 5, arrParam);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("Vision 전송 MAnualInspection {0}", grabside));
        }

        public void RegisterDone()
        {
            SendPacket(VisionDefinition.REGISTER);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //   sendRunner("IS", SeverityLevel.WARN, string.Format("RegisterDone"));
        }

        public BitmapSource GetMergedImage()
        {
            MergedImageReady = false;
            MergedImageInfoReady = false;

            byte[] arrParam = new byte[sizeof(int) * 2];
            BitConverter.GetBytes(-1).CopyTo(arrParam, 0 * sizeof(int));
            BitConverter.GetBytes(-1).CopyTo(arrParam, 1 * sizeof(int));
            SendPacket(VisionDefinition.REQ_RESULT_IMAGE, 8, arrParam);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("GetMergedImage"));
            while (!MergedImageReady || !MergedImageInfoReady)
            {
                Thread.Sleep(10);
            }
            return MergedImage;
        }

        public BitmapSource GetMergedImage_Color()
        {
            MergedImageReady = false;
            MergedImageInfoReady = false;

            byte[] arrParam = new byte[sizeof(int) * 2];
            BitConverter.GetBytes(-1).CopyTo(arrParam, 0 * sizeof(int));
            BitConverter.GetBytes(-1).CopyTo(arrParam, 1 * sizeof(int));
            SendPacket(VisionDefinition.REQ_RESULT_IMAGE, 8, arrParam);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("GetMergedImage_Color"));
            while (!MergedImageReady || !MergedImageInfoReady)
            {
                Thread.Sleep(10);
            }

            return MergedImage_Color;
        }

        public BitmapSource GetInspectResultImage(int nRequestCount)
        {
            byte[] arrParam = new byte[sizeof(int) * 2];
            BitConverter.GetBytes(0).CopyTo(arrParam, 0 * sizeof(int));
            BitConverter.GetBytes((int)1).CopyTo(arrParam, 1 * sizeof(int));

            DefImageReady = false;
            SendPacket(VisionDefinition.REQ_RESULT_IMAGE, 8, arrParam);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("GetInspectResultImage"));

            while (!DefImageReady)
            {
                Thread.Sleep(10);
            }
            return DefImage;
        }
        public BitmapSource GetInspectResultImage_Color(int nRequestCount)
        {
            byte[] arrParam = new byte[sizeof(int) * 2];
            BitConverter.GetBytes(0).CopyTo(arrParam, 0 * sizeof(int));
            BitConverter.GetBytes((int)1).CopyTo(arrParam, 1 * sizeof(int));

            DefImageReady = false;
            SendPacket(VisionDefinition.REQ_RESULT_IMAGE, 8, arrParam);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("GetInspectResultImage_Color"));

            while (!DefImageReady)
            {
                Thread.Sleep(10);
            }
            return DefImage_Color;//DefImage;
        }

        public void GrabAndInspect(int grabside = 0)
        {
            m_nReqSecID = -1;
            m_nReqUnitX = -1;
            m_nReqUnitY = -1;

            Grab_Ready = false;
            Grab_Done = false;
            Inspect_Done = false;

            byte[] arrParam = new byte[sizeof(int) * 4];

            BitConverter.GetBytes(VisionDefinition.LINESCAN_FORWARD).CopyTo(arrParam, 0 * sizeof(int));
            BitConverter.GetBytes((int)-1).CopyTo(arrParam, 1 * sizeof(int));
            BitConverter.GetBytes((int)-1).CopyTo(arrParam, 2 * sizeof(int));
            BitConverter.GetBytes(grabside).CopyTo(arrParam, 3 * sizeof(int));

            SendPacket(VisionDefinition.GRAB_AND_INSPECT, sizeof(int) * 4, arrParam);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("GrabAndInspect 전송 Grab {0}", grabside));
        }

        public void SendVerifyInfo(int anVerify, int anUnitBadCount)
        {
            byte[] arrParam = new byte[(sizeof(int) * 2)];
            BitConverter.GetBytes(anVerify).CopyTo(arrParam, 0);
            BitConverter.GetBytes(anUnitBadCount).CopyTo(arrParam, sizeof(int));
            SendPacket(VisionDefinition.SET_VERIFY_INFO, sizeof(int) * 2, arrParam);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("Vision 전송 SendVerifyInfo"));
        }

        public void SendGrabSide(int side)
        {
            byte[] arrParam = new byte[(sizeof(int))];
            BitConverter.GetBytes(side).CopyTo(arrParam, 0);
            SendPacket(VisionDefinition.SET_GRAB_SIDE, sizeof(int), arrParam);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("Vision 전송 SendGrabSide {0}", side));
        }

        public void SendStripAlign(int anSendCode, byte[] arrData, StripAlignInfo si, int id)
        {
            if (arrData != null && arrData.Length > 0)
            {
                int m_nLength = arrData.Length;//si.BndRect.Width * si.BndRect.Height;
                byte[] arrParam = new byte[(sizeof(byte) * m_nLength) + (sizeof(int) * 9)];
                //BitConverter.GetBytes(VisionDefinition.GRAB_IMAGE_SCALE).CopyTo(arrParam, 0);
                BitConverter.GetBytes(id).CopyTo(arrParam, 0);//sizeof(double));
                BitConverter.GetBytes(si.BndRect.X).CopyTo(arrParam, sizeof(int));
                BitConverter.GetBytes(si.BndRect.Y).CopyTo(arrParam, sizeof(int) * 2);
                BitConverter.GetBytes(si.BndRect.Width).CopyTo(arrParam, sizeof(int) * 3);
                BitConverter.GetBytes(si.BndRect.Height).CopyTo(arrParam, sizeof(int) * 4);
                BitConverter.GetBytes(si.SearchMarginX).CopyTo(arrParam, sizeof(int) * 5);
                BitConverter.GetBytes(si.SearchMarginY).CopyTo(arrParam, sizeof(int) * 6);
                BitConverter.GetBytes(si.Match).CopyTo(arrParam, sizeof(int) * 7);
                BitConverter.GetBytes(m_nLength).CopyTo(arrParam, sizeof(int) * 8);
                int nStartIndex = sizeof(int) * 9;

                Array.Copy(arrData, 0, arrParam, nStartIndex, arrData.Length);
                // for (int nIndex = 0; nIndex < m_nHistoLength-1; nIndex++)
                // {
                //     BitConverter.GetBytes(arrData[nIndex]).CopyTo(arrParam, (nStartIndex + (nIndex * sizeof(byte))));
                // }

                SendPacket(anSendCode, arrParam.Length, arrParam);

                //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
                //if (sendRunner != null)
                //    sendRunner("IS", SeverityLevel.WARN, string.Format("SendStripAlign"));
            }
        }

        public void SendIDMark(int anSendCode, IDMarkInfo si, int id)
        {
            byte[] arrParam = new byte[(sizeof(int) * 9)];

            BitConverter.GetBytes(id).CopyTo(arrParam, 0);//sizeof(double));
            BitConverter.GetBytes(si.BndRect.X).CopyTo(arrParam, sizeof(int));
            BitConverter.GetBytes(si.BndRect.Y).CopyTo(arrParam, sizeof(int) * 2);
            BitConverter.GetBytes(si.BndRect.Width).CopyTo(arrParam, sizeof(int) * 3);
            BitConverter.GetBytes(si.BndRect.Height).CopyTo(arrParam, sizeof(int) * 4);
            BitConverter.GetBytes(si.Threshold).CopyTo(arrParam, sizeof(int) * 5);

            SendPacket(anSendCode, arrParam.Length, arrParam);
            //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            //if (sendRunner != null)
            //    sendRunner("IS", SeverityLevel.WARN, string.Format("SendIDMark"));
        }

        public void SendHistogram(int anSendCode, float[] arrHistogram)
        {
            // SEND_VERT_HISTO : Vertical Histogram 전송.
            // SEND_HORI_HISTO : Horizontal Histogram 전송.
            if (anSendCode == VisionDefinition.SEND_VERT_HISTO || anSendCode == VisionDefinition.SEND_HORI_HISTO)
            {
                if (arrHistogram != null && arrHistogram.Length > 0)
                {
                    m_nHistoLength = arrHistogram.Length;
                    byte[] arrParam = new byte[sizeof(double) + sizeof(int) + (sizeof(float) * m_nHistoLength)];

                    BitConverter.GetBytes(VisionDefinition.GRAB_IMAGE_SCALE).CopyTo(arrParam, 0);
                    BitConverter.GetBytes(m_nHistoLength).CopyTo(arrParam, sizeof(double));

                    int nStartIndex = sizeof(double) + sizeof(int);
                    for (int nIndex = 0; nIndex < m_nHistoLength; nIndex++)
                    {
                        BitConverter.GetBytes(arrHistogram[nIndex]).CopyTo(arrParam, (nStartIndex + (nIndex * sizeof(float))));
                    }

                    SendPacket(anSendCode, arrParam.Length, arrParam);
                    //SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
                    //if (sendRunner != null)
                    //    sendRunner("IS", SeverityLevel.WARN, string.Format("SendHistogram"));
                }
            }
        }
        #endregion

        public void SendPacket(int nSendCode, int nSendSize = 0, byte[] pSendData = null)
        {
            lock (this)
            {
                byte[] pPacket = new byte[sizeof(int) * 2 + nSendSize];

                byte[] arrSendCode = BitConverter.GetBytes(nSendCode);
                byte[] arrSendSize = BitConverter.GetBytes(nSendSize);

                arrSendCode.CopyTo(pPacket, sizeof(int) * 0);
                arrSendSize.CopyTo(pPacket, sizeof(int) * 1);

                if ((nSendSize) != 0)
                    Array.Copy(pSendData, 0, pPacket, sizeof(int) * 2, nSendSize);

                int nLoop = 0;
                bool bSend;
                do
                {
                    bSend = SyncSocket.Send(pPacket);
                    if (!bSend && !string.IsNullOrEmpty(m_strIP) && m_nPort > 0)
                    {
                        // Do Recovery.
                        RecoveryConnection();
                    }
                    if (nLoop++ == 2) // 복구는 3회까지 시도하도록 한다.
                    {
                        break;
                    }
                } while (!bSend);
            }
        }

        // 2012-03-23 suoow2 Added.
        // Socket 연결을 복구한다.
        public bool RecoveryConnection()
        {
            Debug.WriteLine("#### Socket Recover Code Called.");
            try
            {
                SyncSocket.ShutDown();

                SyncSocket = new ClientSyncSocket(ID);
                SyncSocket.OnConnect += OnConnect;
                SyncSocket.OnClose += OnClose;
                SyncSocket.OnSend += OnSend;
                SyncSocket.OnReceive += OnReceive;
                SyncSocket.OnError += OnError;
                SyncSocket.OnDisconnect += OnDisconnect;

                if (!Connect(m_strIP, m_nPort))
                    return false;
                GC.Collect();
                Thread.Sleep(250);

                // SendPacket(VisionDefinition.INIT_VISION, VisionParameter.VisionInfoSize, VisionInfo.GetVisionInfo());
            }
            catch
            {
                Debug.WriteLine("#### Socket Recover Code Failed.");
                return false;
            }

            return true;
        }

        private void OnReceive(object sender, SocketReceiveEventArgs e)
        {
            SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
            try
            {
                int nCode = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 0);
                int nSize = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 1);
                int nCount = 0;
                int nIndex = 0;
                int nResultID = 0;
                int nImageType = 0;
                int nStartPos = 0;
                int nEndPos = 0;
                int grabside = 0;

                Debug.WriteLine(nCode);

                switch (nCode)
                {
                    case VisionDefinition.PING:
                        break;
                    case VisionDefinition.ERROR_LOG_1:
                    case VisionDefinition.ERROR_LOG_2:
                    case VisionDefinition.ERROR_LOG_3:
                        // TO DO : 로그에 대한 처리
                        break;
                    case VisionDefinition.STATUS_LOG_1:
                    case VisionDefinition.STATUS_LOG_2:
                    case VisionDefinition.STATUS_LOG_3:
                        // TO DO : 로그에 대한 처리
                        break;
                    case VisionDefinition.GRAB_READY:
                        Grab_Ready = true;
                        break;
                    case VisionDefinition.GRAB_DONE:
                        nIndex = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        if (nIndex == -1)
                        {
                            if (AutoInspect)
                            {
                                if (GetLiveImage)
                                {
                                    ReqCornerGrabImage();////GrabDone Flag에 관여하지 않기 때문에 전체영상 요청보다 반듯이 먼저한다
                                    ReqGrabImage();                                
                                }
                                Grab_Done = true;
                            }
                            else
                            {
                                ReqCornerGrabImage();////GrabDone Flag에 관여하지 않기 때문에 전체영상 요청보다 반듯이 먼저한다
                                if (!this.UseRGB)
                                {
                                    ReqGrabImage();
                                }
                                else
                                {
                                    ReqGrabImageRGB();                                
                                    //ReqICSGrabImageRGB();//모노 영상으로도 충분하지 않을까
                                }                          
                            }
                        }
                        break;
                    case VisionDefinition.REGISTER_DONE:
                        Register_Done = true;
                        break;
                    case VisionDefinition.REGISTER_FAIL:
                        Register_Fail = true;
                        Register_Fail_Algo = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        if (sendRunner != null)
                            sendRunner("IS", SeverityLevel.ERROR, string.Format("{2}검사 아이템 등록에 실패하였습니다. IP:{0}, Port:{1}, Code:REGISTER_FAIL", m_strIP, m_nPort, Register_Fail_Algo));

                        break;
                    case VisionDefinition.REGISTER_PROGRESS:
                        break;
                    case VisionDefinition.ACK_GRAB_IMAGE_RGB:
                        #region ACK_GRAB_IMAGE, ACK_LIVE_IMAGE, ACK_SECTION
                        nIndex = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        m_nReceiveDataWidth = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 3);
                        m_nReceiveDataHeight = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 4);
                        nStartPos = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 5);
                        nEndPos = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 6);

                        if (nStartPos == 0)
                        {
                            if (m_recvImagePixelsR == null || m_recvImagePixelsR.Length != m_nReceiveDataWidth * m_nReceiveDataHeight)
                            {
                                m_recvImagePixelsR = new byte[m_nReceiveDataWidth * m_nReceiveDataHeight];
                            }
                            if (m_recvImagePixelsG == null || m_recvImagePixelsG.Length != m_nReceiveDataWidth * m_nReceiveDataHeight)
                            {
                                m_recvImagePixelsG = new byte[m_nReceiveDataWidth * m_nReceiveDataHeight];
                            }
                            if (m_recvImagePixelsB == null || m_recvImagePixelsB.Length != m_nReceiveDataWidth * m_nReceiveDataHeight)
                            {
                                m_recvImagePixelsB = new byte[m_nReceiveDataWidth * m_nReceiveDataHeight];
                            }
                        }
                        try
                        {
                            if (nStartPos >= (m_nReceiveDataWidth * m_nReceiveDataHeight * 2))
                            {
                                Array.Copy(e.ReceiveData, sizeof(int) * 7, m_recvImagePixelsB, nStartPos - (m_nReceiveDataWidth * m_nReceiveDataHeight * 2), (nEndPos - nStartPos));
                            }
                            else
                           if (nStartPos >= (m_nReceiveDataWidth * m_nReceiveDataHeight))
                            {
                                if (nEndPos >= (m_nReceiveDataWidth * m_nReceiveDataHeight * 2))
                                {
                                    int pos = nEndPos - (m_nReceiveDataWidth * m_nReceiveDataHeight * 2);
                                    Array.Copy(e.ReceiveData, sizeof(int) * 7, m_recvImagePixelsG, nStartPos - (m_nReceiveDataWidth * m_nReceiveDataHeight), (nEndPos - nStartPos) - pos);
                                    Array.Copy(e.ReceiveData, sizeof(int) * 7 + ((nEndPos - nStartPos) - pos), m_recvImagePixelsB, 0, pos);

                                }
                                else
                                    Array.Copy(e.ReceiveData, sizeof(int) * 7, m_recvImagePixelsG, nStartPos - (m_nReceiveDataWidth * m_nReceiveDataHeight), (nEndPos - nStartPos));
                            }
                            else
                            {
                                if (nEndPos >= (m_nReceiveDataWidth * m_nReceiveDataHeight))
                                {
                                    int pos = nEndPos - (m_nReceiveDataWidth * m_nReceiveDataHeight);
                                    Array.Copy(e.ReceiveData, sizeof(int) * 7, m_recvImagePixelsR, nStartPos, (nEndPos - nStartPos) - pos);
                                    Array.Copy(e.ReceiveData, sizeof(int) * 7 + ((nEndPos - nStartPos) - pos), m_recvImagePixelsG, 0, pos);

                                }
                                else
                                    Array.Copy(e.ReceiveData, sizeof(int) * 7, m_recvImagePixelsR, nStartPos, nEndPos - nStartPos);
                            }

                            if (nEndPos >= (m_nReceiveDataWidth * m_nReceiveDataHeight * 3))
                            {
                                Grab_Done = true;
                            }
                        }
                        catch
                        {
                            if (sendRunner != null)
                                sendRunner("IS", SeverityLevel.FATAL, string.Format("이미지 수신에 실패하였습니다. IP:{0}, Port:{1}, Code:{2}", m_strIP, m_nPort, nCode));
                        }

                        #endregion ACK_GRAB_IMAGE, ACK_LIVE_IMAGE, ACK_SECTION
                        break;
                    case VisionDefinition.ACK_GRAB_IMAGE:
                    case VisionDefinition.ACK_LIVE_IMAGE:
                        #region ACK_GRAB_IMAGE, ACK_LIVE_IMAGE, ACK_SECTION
                        nIndex = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        m_nReceiveDataWidth = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 3);
                        m_nReceiveDataHeight = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 4);
                        nStartPos = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 5);
                        nEndPos = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 6);

                        if (nStartPos == 0)
                        {
                            if (m_recvImagePixels == null || m_recvImagePixels.Length != m_nReceiveDataWidth * m_nReceiveDataHeight)
                            {
                                m_recvImagePixels = new byte[m_nReceiveDataWidth * m_nReceiveDataHeight];
                            }
                        }
                        try
                        {
                            Array.Copy(e.ReceiveData, sizeof(int) * 7, m_recvImagePixels, nStartPos, nEndPos - nStartPos);
                            if (nEndPos >= (m_nReceiveDataWidth * m_nReceiveDataHeight))
                            {
                                if (AutoInspect)
                                {
                                    if (GetLiveImage)
                                    {
                                        // Grab Image Get!! Event
                                        LiveImageUpdateHandler updateRunner = LiveImageUpdateEvent;
                                        if (updateRunner != null)
                                            updateRunner(this.ID);
                                    }
                                }
                                Grab_Done = true;
                            }
                        }
                        catch
                        {
                            if (sendRunner != null)
                                sendRunner("IS", SeverityLevel.FATAL, string.Format("이미지 수신에 실패하였습니다. IP:{0}, Port:{1}, Code:{2}", m_strIP, m_nPort, nCode));
                        }
                        break;
                    #endregion ACK_GRAB_IMAGE, ACK_LIVE_IMAGE, ACK_SECTION
                    case VisionDefinition.ACK_SECTION:
                        #region ACK_SECTION
                        nIndex = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        m_nSectionReceiveDataWidth = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 3);
                        m_nSectionReceiveDataHeight = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 4);
                        nStartPos = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 5);
                        nEndPos = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 6);

                        if (nStartPos == 0)
                        {
                            if (m_recvImagePixels == null || m_recvImagePixels.Length != m_nSectionReceiveDataWidth * m_nSectionReceiveDataHeight)
                            {
                                m_recvImagePixels = new byte[m_nSectionReceiveDataWidth * m_nSectionReceiveDataHeight];
                            }
                        }
                        try
                        {
                            Array.Copy(e.ReceiveData, sizeof(int) * 7, m_recvImagePixels, nStartPos, nEndPos - nStartPos);
                            if (nEndPos >= (m_nSectionReceiveDataWidth * m_nSectionReceiveDataHeight))
                            {
                                if (AutoInspect)
                                {
                                    if (GetLiveImage)
                                    {
                                        // Grab Image Get!! Event
                                        LiveImageUpdateHandler updateRunner = LiveImageUpdateEvent;
                                        if (updateRunner != null)
                                            updateRunner(this.ID);
                                    }
                                }
                                Grab_Done = true;
                            }
                        }
                        catch
                        {
                            if (sendRunner != null)
                                sendRunner("IS", SeverityLevel.FATAL, string.Format("이미지 수신에 실패하였습니다. IP:{0}, Port:{1}, Code:{2}", m_strIP, m_nPort, nCode));
                        }
                        break;
                    #endregion ACK_SECTION
                    case VisionDefinition.ACK_SECTION_COLOR:
                        nIndex = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        m_nReceiveDataWidth_COLOR = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 3);
                        m_nReceiveDataHeight_COLOR = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 4);
                        nStartPos = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 5);
                        nEndPos = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 6);

                        if (nStartPos == 0)
                        {
                            if (m_recvImagePixels_Color == null || m_recvImagePixels_Color.Length != m_nReceiveDataWidth_COLOR * m_nReceiveDataHeight_COLOR * 3)
                            {
                                m_recvImagePixels_Color = new byte[m_nReceiveDataWidth_COLOR * m_nReceiveDataHeight_COLOR * 3];
                            }
                        }
                        try
                        {
                            Array.Copy(e.ReceiveData, sizeof(int) * 7, m_recvImagePixels_Color, nStartPos, nEndPos - nStartPos);
                            if (nEndPos >= (m_nReceiveDataWidth_COLOR * m_nReceiveDataHeight_COLOR * 3))
                            {
                                if (AutoInspect)
                                {
                                    if (GetLiveImage)
                                    {
                                        // Grab Image Get!! Event
                                        LiveImageUpdateHandler updateRunner = LiveImageUpdateEvent;
                                        if (updateRunner != null)
                                            updateRunner(this.ID);
                                    }
                                }
                                Grab_Done = true;
                            }
                        }
                        catch
                        {
                            if (sendRunner != null)
                                sendRunner("IS", SeverityLevel.FATAL, string.Format("이미지 수신에 실패하였습니다. IP:{0}, Port:{1}, Code:{2}", m_strIP, m_nPort, nCode));
                        }
                        break;
                    case VisionDefinition.ACK_RESULT_COUNT:
                        #region ACK_RESULT_COUNT
                        nCount = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        VisionResult.IDMark = new IDMarkResultInfo();

                        VisionResult.IDMark.Status = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 3);
                        VisionResult.IDMark.Width = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 4);
                        VisionResult.IDMark.Height = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 5);
                        grabside = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 6);
                        ReqREQ_ICS_OFFSET(grabside);
                        Thread.Sleep(50);
                        if (VisionResult.IDMark.Status == 1)
                        {
                            byte[] res = new byte[VisionResult.IDMark.Width * VisionResult.IDMark.Height];
                            Array.Copy(e.ReceiveData, sizeof(int) * 7, res, 0, VisionResult.IDMark.Width * VisionResult.IDMark.Height);
                            VisionResult.IDMark.Image = ByteArrayToBitmap(res, VisionResult.IDMark.Width, VisionResult.IDMark.Height);
                        }
                        m_iTrainError = 0;
                        VisionResult.ResultItemCount = nCount;
                        if (nCount > 0)
                        {
                            SendPacket(VisionDefinition.REQ_RESULT_DATA);
                        }
                        else
                        {
                            DefectCount = 0;
                            InspectDoneEventHandler er = InspectDone;
                            if (er != null) er(ID, grabside);
                            Inspect_Done = true;
                        }
                        break;
                    #endregion ACK_RESULT_COUNT
                    case VisionDefinition.ACK_SHIFT_DATA:
                        #region ACK_SHIFT_DATA
                        nCount = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        PSRResult = new PSRResults();
                        if (nCount > 0)
                        {
                            PSRShiftCount = nCount;
                            PSRResult = new PSRResults(nCount);

                            int nResultPacketSize = sizeof(int) * 6;
                            int nStep = 0;
                            try
                            {
                                for (int i = 0; i < nCount; i++)
                                {
                                    nStep = nResultPacketSize * i;
                                    PSRResult.Results[i].CommRet = BitConverter.ToBoolean(e.ReceiveData, sizeof(int) * 3 + nStep);
                                    PSRResult.Results[i].UnitPos = new System.Drawing.Point(BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 4 + nStep),
                                                                                               BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 5 + nStep));
                                    PSRResult.Results[i].Position = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 6 + nStep);
                                    PSRResult.Results[i].Offset = new System.Drawing.PointF(BitConverter.ToSingle(e.ReceiveData, sizeof(int) * 7 + nStep),
                                                                                               BitConverter.ToSingle(e.ReceiveData, sizeof(int) * 8 + nStep));
                                }
                            }
                            catch
                            {
                                if (sendRunner != null)
                                    sendRunner("IS", SeverityLevel.FATAL, string.Format("결과 수신에 실패하였습니다. IP:{0}, Port:{1}, Code:ACK_SHIFT_DATA", m_strIP, m_nPort));
                            }
                        }
                        #endregion
                        break;
                    case VisionDefinition.ACK_RESULT_DATA:
                        #region ACK_RESULT_DATA
                        nCount = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        grabside = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 3);
                        if (nCount > 0)
                        {
                            if (e.ReceiveData.Length - 16 != nCount * sizeof(int) * 48)   //28
                            {
                                SendPacket(VisionDefinition.REQ_RESULT_DATA);
                                DefectCount = 0;
                            }
                            DefectCount = nCount;

                            int nResultPacketSize = sizeof(int) * 48;  //28
                            int nStep = 0;
                            try
                            {
                                for (int i = 0; i < nCount; i++)
                                {
                                    nStep = nResultPacketSize * i;
                                    VisionResult.Results[i].ResultID = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 4 + nStep);
                                    VisionResult.Results[i].InspID = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 5 + nStep);
                                    VisionResult.Results[i].SectionID = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 6 + nStep);
                                    VisionResult.Results[i].RoiID = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 7 + nStep);

                                    VisionResult.Results[i].UnitPos = new System.Drawing.Point(BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 8 + nStep),
                                                                                               BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 9 + nStep));
                                    //if (AutoInspect && VisionResult.Results[i].UnitPos.X == 0 && VisionResult.Results[i].UnitPos.Y == 0)
                                    //{
                                    //    try
                                    //    {
                                    //        if (VisionResult.Results[i].SectionTypeCode == SectionTypeCode.OUTER_REGION)
                                    //        {
                                    //            VisionResult.Results[i].UnitPos.X = -1;
                                    //            VisionResult.Results[i].UnitPos.Y = -1;
                                    //        }
                                    //    }
                                    //    catch { }
                                    //}

                                    VisionResult.Results[i].BreakType = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 10 + nStep);
                                    VisionResult.Results[i].ResultType = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 11 + nStep);

                                    VisionResult.Results[i].AbsoluteDefectRect = new Int32Rect(BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 12 + nStep),
                                                                                               BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 13 + nStep),
                                                                                               BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 14 + nStep),
                                                                                               BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 15 + nStep));

                                    VisionResult.Results[i].AbsoluteDefectCenter = new System.Drawing.Point(BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 16 + nStep),
                                                                                                            BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 17 + nStep));
                                    VisionResult.Results[i].RelativeDefectRect = new Int32Rect(BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 18 + nStep),
                                                                                               BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 19 + nStep),
                                                                                               BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 20 + nStep),
                                                                                               BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 21 + nStep));

                                    VisionResult.Results[i].RelativeDefectCenter = new System.Drawing.Point(BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 22 + nStep),
                                                                                                            BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 23 + nStep));
                                    VisionResult.Results[i].DefectSize = BitConverter.ToDouble(e.ReceiveData, sizeof(int) * 24 + nStep);
                                    VisionResult.Results[i].DefectScore = BitConverter.ToDouble(e.ReceiveData, sizeof(int) * 26 + nStep);

                                    int ngid = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 28 + nStep);
                                    VisionResult.Results[i].Channel = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 29 + nStep);

                                    VisionResult.Results[i].Grabside = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 30 + nStep);

                                    #region new result para
                                    VisionResult.Results[i].AreaID = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 31 + nStep);
                                    VisionResult.Results[i].Priority = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 32 + nStep);
                                    VisionResult.Results[i].LongDegree = BitConverter.ToDouble(e.ReceiveData, sizeof(int) * 33 + nStep);
                                    VisionResult.Results[i].ShortDegree = BitConverter.ToDouble(e.ReceiveData, sizeof(int) * 35 + nStep);
                                    VisionResult.Results[i].AvgRed = BitConverter.ToDouble(e.ReceiveData, sizeof(int) * 37 + nStep);
                                    VisionResult.Results[i].AvgGreen = BitConverter.ToDouble(e.ReceiveData, sizeof(int) * 39 + nStep); 
                                    VisionResult.Results[i].AvgBlue = BitConverter.ToDouble(e.ReceiveData, sizeof(int) * 41 + nStep);
                                    VisionResult.Results[i].AvgGV = BitConverter.ToDouble(e.ReceiveData, sizeof(int) * 43 + nStep);
                                    VisionResult.Results[i].AvgS = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 45 + nStep);
                                    VisionResult.Results[i].AvgH = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 46 + nStep);
                                    VisionResult.Results[i].UnitAlignOffset_X = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 47 + nStep);
                                    VisionResult.Results[i].UnitAlignOffset_Y = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 48 + nStep);                        
                                    VisionResult.Results[i].DefectPosX = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 49 + nStep);
                                    VisionResult.Results[i].DefectPosY = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 50 + nStep);
                                    eSectionType tempSectionType = (eSectionType)BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 51 + nStep);
                                    VisionResult.Results[i].SectionType = SectionManager.GetSectionType(tempSectionType);
                                    #endregion

                                    if (VisionResult.Results[i].ResultType == (int)eVisInspectResultType.eInspResultTrainError)
                                        m_iTrainError = -1; // Train Error
                                    else if (VisionResult.Results[i].ResultType == (int)eVisInspectResultType.eInspResultBufferError)
                                        m_iTrainError = -2; // Buffer Error
                                    else if (VisionResult.Results[i].ResultType == (int)eVisInspectResultType.eInspResultInputParamError)
                                        m_iTrainError = -3; // Param Error
                                }
                                if (AutoInspect == false) Inspect_Done = true;
                            }
                            catch
                            {
                                if (sendRunner != null)
                                    sendRunner("IS", SeverityLevel.FATAL, string.Format("결과 수신에 실패하였습니다. IP:{0}, Port:{1}, Code:ACK_RESULT_DATA", m_strIP, m_nPort));
                            }
                        }
                        else 
                            Inspect_Done = true;
                        if (sendRunner != null)
                            sendRunner("IS", SeverityLevel.DEBUG, string.Format("결과 수신 완료, InspectDone 수행, ID = {0}", ID));

                        InspectDoneEventHandler er1 = InspectDone;
                        if (er1 != null) er1(ID, grabside);
              
                        break;
                    #endregion ACK_RESULT_DATA
                    case VisionDefinition.ACK_RESULT_IMAGE:
                        #region ACK_RESULT_IMAGE
                        nResultID = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        nImageType = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 3);
                        m_nResultImageWidth = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 4);
                        m_nResultImageHeight = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 5);
                        nStartPos = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 6);
                        nEndPos = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 7);

                        //전체이미지
                        if (nImageType == -1)
                        {
                            // Init Buffer.
                            if (nStartPos == 0)
                            {
                                if (m_mergedImagePixels == null || m_mergedImagePixels.Length != m_nResultImageWidth * m_nResultImageHeight)
                                {
                                    m_mergedImagePixels = new byte[m_nResultImageWidth * m_nResultImageHeight];
                                }
                            }
                            try
                            {
                                Array.Copy(e.ReceiveData, sizeof(int) * 8, m_mergedImagePixels, nStartPos, nEndPos - nStartPos);
                                if (nEndPos >= (m_nResultImageWidth * m_nResultImageHeight))
                                {
                                    MergedImageReady = true;
                                    SendPacket(VisionDefinition.REQ_RESULT_INFO);
                                }
                            }
                            catch
                            {
                                if (sendRunner != null)
                                    sendRunner("IS", SeverityLevel.FATAL, string.Format("비정상 전체 이미지가 수신되었습니다. IP:{0}, Port:{1}, Code:ACK_RESULT_IMAGE", m_strIP, m_nPort));
                            }
                        }
                        //기준 이미지
                        else if (nImageType == 0)
                        {
                            // Init Buffer.
                            if (nStartPos == 0)
                            {
                                if (m_refImagePixels == null || m_refImagePixels.Length != m_nResultImageWidth * m_nResultImageHeight)
                                {
                                    m_refImagePixels = new byte[m_nResultImageWidth * m_nResultImageHeight];
                                }
                            }
                            try
                            {
                                Array.Copy(e.ReceiveData, sizeof(int) * 8, m_refImagePixels, nStartPos, nEndPos - nStartPos);
                                if (nEndPos >= (m_nResultImageWidth * m_nResultImageHeight))
                                {
                                    RefImageReady = true;
                                }
                            }
                            catch
                            {
                                if (sendRunner != null)
                                    sendRunner("IS", SeverityLevel.FATAL, string.Format("비정상 기준 이미지가 수신되었습니다. IP:{0}, Port:{1}, Code:ACK_RESULT_IMAGE", m_strIP, m_nPort));
                            }
                        }
                        //불량이미지
                        else
                        {
                            // Init Buffer.
                            if (nStartPos == 0)
                            {
                                if (m_defImagePixels == null || m_defImagePixels.Length != m_nResultImageWidth * m_nResultImageHeight)
                                {
                                    m_defImagePixels = new byte[m_nResultImageWidth * m_nResultImageHeight];
                                }
                            }
                            try
                            {
                                Array.Copy(e.ReceiveData, sizeof(int) * 8, m_defImagePixels, nStartPos, nEndPos - nStartPos);
                                if (nEndPos >= (m_nResultImageWidth * m_nResultImageHeight))
                                {
                                    DefImageReady = true;
                                }
                            }
                            catch
                            {
                                if (sendRunner != null)
                                    sendRunner("IS", SeverityLevel.FATAL, string.Format("비정상 불량 이미지가 수신되었습니다. IP:{0}, Port:{1}, Code:ACK_RESULT_IMAGE", m_strIP, m_nPort));
                            }
                        }
                        Inspect_Done = true;
                        break;
                    #endregion ACK_RESULT_IMAGE
                    case VisionDefinition.ACK_RESULT_IMAGE_COLOR:
                        #region ACK_RESULT_IMAGE_COLOR
                        nResultID = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        nImageType = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 3);
                        m_nResultImageWidth = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 4);
                        m_nResultImageHeight = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 5);
                        nStartPos = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 6);
                        nEndPos = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 7);

                        //전체이미지
                        if (nImageType == -1)
                        {
                            // Init Buffer.
                            if (nStartPos == 0)
                            {
                                if (m_mergedImagePixels_Color == null || m_mergedImagePixels_Color.Length != m_nResultImageWidth * m_nResultImageHeight * 3)
                                {
                                    m_mergedImagePixels_Color = new byte[m_nResultImageWidth * m_nResultImageHeight * 3];
                                }
                            }
                            try
                            {
                                Array.Copy(e.ReceiveData, sizeof(int) * 8, m_mergedImagePixels_Color, nStartPos, nEndPos - nStartPos);
                                if (nEndPos >= (m_nResultImageWidth * m_nResultImageHeight * 3))
                                {
                                    MergedImageReady = true;
                                    SendPacket(VisionDefinition.REQ_RESULT_INFO);
                                }
                            }
                            catch
                            {
                                if (sendRunner != null)
                                    sendRunner("IS", SeverityLevel.FATAL, string.Format("비정상 전체 이미지가 수신되었습니다. IP:{0}, Port:{1}, Code:ACK_RESULT_IMAGE", m_strIP, m_nPort));
                            }
                        }
                        //기준 이미지
                        else if (nImageType == 0)
                        {
                            // Init Buffer.
                            if (nStartPos == 0)
                            {
                                if (m_refImagePixels_Color == null || m_refImagePixels_Color.Length != m_nResultImageWidth * m_nResultImageHeight * 3)
                                {
                                    m_refImagePixels_Color = new byte[m_nResultImageWidth * m_nResultImageHeight * 3];
                                }
                            }
                            try
                            {
                                Array.Copy(e.ReceiveData, sizeof(int) * 8, m_refImagePixels_Color, nStartPos, nEndPos - nStartPos);
                                if (nEndPos >= (m_nResultImageWidth * m_nResultImageHeight * 3))
                                {
                                    RefImageReady = true;
                                }
                            }
                            catch
                            {
                                if (sendRunner != null)
                                    sendRunner("IS", SeverityLevel.FATAL, string.Format("비정상 기준 이미지가 수신되었습니다. IP:{0}, Port:{1}, Code:ACK_RESULT_IMAGE", m_strIP, m_nPort));
                            }
                        }
                        //불량이미지
                        else
                        {
                            // Init Buffer.
                            if (nStartPos == 0)
                            {
                                if (m_defImagePixels_Color == null || m_defImagePixels_Color.Length != m_nResultImageWidth * m_nResultImageHeight * 3)
                                {
                                    m_defImagePixels_Color = new byte[m_nResultImageWidth * m_nResultImageHeight * 3];
                                }
                            }
                            try
                            {
                                Array.Copy(e.ReceiveData, sizeof(int) * 8, m_defImagePixels_Color, nStartPos, nEndPos - nStartPos);
                                if (nEndPos >= (m_nResultImageWidth * m_nResultImageHeight * 3))
                                {
                                    DefImageReady = true;
                                }
                            }
                            catch
                            {
                                if (sendRunner != null)
                                    sendRunner("IS", SeverityLevel.FATAL, string.Format("비정상 불량 이미지가 수신되었습니다. IP:{0}, Port:{1}, Code:ACK_RESULT_IMAGE", m_strIP, m_nPort));
                            }
                        }
                        Inspect_Done = true;
                        break;
                    #endregion
                    case VisionDefinition.ACK_RESULT_INFO:
                        #region ACK_RESULT_INFO
                        m_nMergedImageWidth = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        m_nMergedImageHeight = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 3);
                        m_nMergedImageHorResCnt = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 4);
                        m_nMergedImageLimitCnt = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 5);

                        MergedImageInfoReady = true;
                        break;
                    #endregion ACK_RESULT_INFO
                    case VisionDefinition.INSPECT_DONE:
                        #region INSPECT_DONE
                        int nSecID = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        int nUnitX = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 3);
                        int nUnitY = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 4);
                        int nInspDoneIndex = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 5);
                        if (nInspDoneIndex == -1 && nSecID == m_nReqSecID && nUnitX == m_nReqUnitX && nUnitY == m_nReqUnitY)
                        {
                            SendPacket(VisionDefinition.REQ_RESULT_COUNT);
                        }
                        if (sendRunner != null)
                            sendRunner("IS", SeverityLevel.FATAL, string.Format("Receive INSPECT_DONE, Send REQ_RESULT_COUNT, ID = {0}", ID));
                        break;
                    #endregion INSPECT_DONE
                    case VisionDefinition.ACK_VERT_HISTO:
                    case VisionDefinition.ACK_HORI_HISTO:
                        #region ACK_VERT_HISTO, ACK_HORI_HISTO
                        double fScale = BitConverter.ToDouble(e.ReceiveData, sizeof(int) * 2);
                        int nHistoSize = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2 + sizeof(double));

                        if (fScale == VisionDefinition.GRAB_IMAGE_SCALE && nHistoSize == m_nHistoLength)
                            ProfileSent = true;
                        else
                            ProfileSent = false;
                        break;
                    #endregion ACK_VERT_HISTO, ACK_HORI_HISTO
                    case VisionDefinition.ACK_STRIPALIGN:
                    case VisionDefinition.ACK_IDMARK:
                        ProfileSent = true;
                        break;
                    case VisionDefinition.ACK_CENTER_LINE_INFO:
                        #region ACK_CENTER_LINE_INFO
                        int nInspID = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        int nLineSegmentCount = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 3);

                        LineSegments = new GraphicsSkeletonLine[nLineSegmentCount];
                        for (int i = 0; i < nLineSegmentCount; i++)
                        {
                            LineSegments[i] = new GraphicsSkeletonLine();
                        }

                        int nPacketPos = 16; // sizeof(int) * 4
                        for (int i = 0; i < nLineSegmentCount; i++)
                        {
                            LineSegments[i].LineDir = e.ReceiveData[nPacketPos + 4];
                            LineSegments[i].MedianSize = BitConverter.ToSingle(e.ReceiveData, nPacketPos + 5);
                            LineSegments[i].BoundaryRect = new Int16Rect();
                            LineSegments[i].BoundaryRect.X = BitConverter.ToInt16(e.ReceiveData, nPacketPos + 9);
                            LineSegments[i].BoundaryRect.Y = BitConverter.ToInt16(e.ReceiveData, nPacketPos + 11);
                            LineSegments[i].BoundaryRect.Width = BitConverter.ToInt16(e.ReceiveData, nPacketPos + 13);
                            LineSegments[i].BoundaryRect.Height = BitConverter.ToInt16(e.ReceiveData, nPacketPos + 15);

                            int nPointsCnt = BitConverter.ToInt32(e.ReceiveData, nPacketPos + 17);
                            nPacketPos += 21;

                            LineSegments[i].Nodes = new SkeletonNode[nPointsCnt];
                            for (int n = 0; n < nPointsCnt; n++)
                            {
                                LineSegments[i].Nodes[n] = new SkeletonNode();
                                LineSegments[i].Nodes[n].LineDirection = e.ReceiveData[nPacketPos];
                                LineSegments[i].Nodes[n].Position = new Int16Point();
                                LineSegments[i].Nodes[n].Position.X = BitConverter.ToInt16(e.ReceiveData, nPacketPos + 1);
                                LineSegments[i].Nodes[n].Position.Y = BitConverter.ToInt16(e.ReceiveData, nPacketPos + 3);
                                LineSegments[i].Nodes[n].MeasureSize = BitConverter.ToSingle(e.ReceiveData, nPacketPos + 5);
                                LineSegments[i].Nodes[n].Type = e.ReceiveData[nPacketPos + 9];

                                nPacketPos += 10;
                            }
                        }
                        Recv_CenterLine_Done = true;
                        #endregion ACK_CENTER_LINE_INFO
                        break;
                    case VisionDefinition.ACK_BALL_INFO:
                        #region ACK_BALL_INFO
                        int nInspID1 = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        int nBallSegmentCount = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 3);

                        BallSegments = new GraphicsSkeletonBall[nBallSegmentCount];
                        for (int i = 0; i < nBallSegmentCount; i++)
                        {
                            BallSegments[i] = new GraphicsSkeletonBall();
                        }

                        int nPacketPos1 = 16; // sizeof(int) * 4
                        for (int i = 0; i < nBallSegmentCount; i++)
                        {
                            BallSegments[i].Radian = BitConverter.ToSingle(e.ReceiveData, nPacketPos1 + 4);
                            BallSegments[i].BoundaryRect = new Int16Rect();
                            BallSegments[i].BoundaryRect.X = BitConverter.ToInt16(e.ReceiveData, nPacketPos1 + 8);
                            BallSegments[i].BoundaryRect.Y = BitConverter.ToInt16(e.ReceiveData, nPacketPos1 + 10);
                            BallSegments[i].BoundaryRect.Width = BitConverter.ToInt16(e.ReceiveData, nPacketPos1 + 12);
                            BallSegments[i].BoundaryRect.Height = BitConverter.ToInt16(e.ReceiveData, nPacketPos1 + 14);
                            nPacketPos1 += 16;
                        }
                        Recv_Ball_Done = true;
                        #endregion ACK_BALL_INFO
                        break;
                    case VisionDefinition.ACK_CORNER_IMAGE:
                        #region ACK_CORNER_IMAGE
                        nIndex = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        m_nReceiveCornerDataWidth = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 3);
                        m_nReceiveCornerDataHeight = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 4);
                        nStartPos = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 5);
                        nEndPos = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 6);

                        if (nStartPos == 0)
                        {
                            if (m_recvCornerImagePixels == null || m_recvCornerImagePixels.Length != m_nReceiveCornerDataWidth * m_nReceiveCornerDataHeight)
                            {
                                m_recvCornerImagePixels = new byte[m_nReceiveCornerDataWidth * m_nReceiveCornerDataHeight];
                            }
                        }
                        try
                        {
                            Array.Copy(e.ReceiveData, sizeof(int) * 7, m_recvCornerImagePixels, nStartPos, nEndPos - nStartPos);
                            //if (nEndPos >= (m_nReceiveCornerDataWidth * m_nReceiveCornerDataHeight))
                            //{
                            //    if (AutoInspect)
                            //    {
                            //        if (GetLiveImage)
                            //        {
                            //            // Grab Image Get!! Event
                            //            LiveImageUpdateHandler updateRunner = LiveImageUpdateEvent;
                            //            if (updateRunner != null)
                            //                updateRunner(this.ID);
                            //        }
                            //    }
                            //}
                        }
                        catch
                        {
                            if (sendRunner != null)
                                sendRunner("IS", SeverityLevel.FATAL, string.Format("Corner 이미지 수신에 실패하였습니다. IP:{0}, Port:{1}, Code:{2}", m_strIP, m_nPort, nCode));
                        }
                        #endregion ACK_CORNER_IMAGE
                        break;

                    case VisionDefinition.ACK_ICS_OFFSET:
                        #region ACK_UNIT_OFFSET
                        /////////////////////////
                        grabside = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 2);
                        try
                        {                       
                            int UnitOffsetTopX = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 3);
                            int UnitOffsetTopY = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 4);
                            int UnitOffsetBottomX = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 5);
                            int UnitOffsetBottomY = BitConverter.ToInt32(e.ReceiveData, sizeof(int) * 6);

                            List<System.Windows.Point> UnitOffset = new List<System.Windows.Point>();
                            UnitOffset.Add(new System.Windows.Point(UnitOffsetTopX, UnitOffsetTopY));
                            UnitOffset.Add(new System.Windows.Point(UnitOffsetBottomX, UnitOffsetBottomY));

                            ReceiveUnitOffsetEventHandler Event = ReceiveUnitOffset;
                            if (Event != null) Event(ID, grabside, UnitOffset);
                        }
                        catch
                        {
                            if (sendRunner != null)
                                sendRunner("IS", SeverityLevel.FATAL, string.Format("Unit Offset 수신에 실패했습니다 IP:{0}, Port:{1}, Code:ACK_RESULT_IMAGE", m_strIP, m_nPort));
                        }
                   
                        ///////////////////////////
                        #endregion ACK_UNIT_OFFSET
                        break;
                    default:
                        if (sendRunner != null)
                            sendRunner("IS", SeverityLevel.FATAL, string.Format("잘못된 코드의 패킷이 수신되었습니다. IP:{0}, Port:{1}, Code:{2}", m_strIP, m_nPort, nCode));
                        break;
                }
            }
            catch
            {
                if (sendRunner != null)
                    sendRunner("IS", SeverityLevel.FATAL, string.Format("데이터 수신 도중에 치명 오류가 발생하였습니다. IP:{0}, Port:{1}", m_strIP, m_nPort));
            }
        }

        public void Set_UseAI(bool Enable)
        {
            m_bUseAI = Enable;
        }
    }
}
