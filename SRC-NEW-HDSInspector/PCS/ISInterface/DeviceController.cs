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

using System;
using DCS.PLC;
using System.Diagnostics;
using Common;
using DCS.Light;
using Common.DataBase;
using DCS.NiDaq;

namespace PCS
{
    public class DeviceController
    {
        public static event SendLogMessageEventHandler SendLogMessageEvent;

        public VisionInterface[] Vision { get; private set; }
        public bool[] VisionConnected { get; private set; }

        public VisionInterface TheVision { get; private set; }
        public bool TheVisionConnected { get; private set; }

        public bool IsOpenPlc { get; private set; }
        public bool IsOpenCounter { get; private set; }
        public bool IsOpenLight { get; private set; }

        public PLC_Address Address;

        public PLCDevice PlcDevice { get; private set; }
        public static LightDeviceManager LightDevice;
        private IntPtr CounterDevice1 = new IntPtr();   // Counter Device Handle. (Input)
        private IntPtr CounterDevice2 = new IntPtr();
        private IntPtr CounterDevice3 = new IntPtr();

        private DCS.SW_Trigger Trigger_CA;
        private DCS.SW_Trigger Trigger_BA;

        private SubSystems m_SettingDevice = null;
        private Jobs m_JobDevice = null;
        private string MachineName;

        public bool bInit = false;

        public DeviceController(SubSystems aSettingDevice, string aszMachineName, Jobs aJobDevice)
        {
            MachineName = aszMachineName;
            m_SettingDevice = aSettingDevice;
            m_JobDevice = aJobDevice;
            CreateVision();
            
            System.Threading.Thread thdInit = new System.Threading.Thread(InitDevice);
            thdInit.Start();
        }

        private void InitDevice()
        {
            #region Initialize Devices.
            // Initialize PLC
            if (m_SettingDevice.PLC.UsePLC)
            {
                try
                {
                    string file = System.IO.Directory.GetCurrentDirectory() + "\\..\\config\\PLC_Address.ini";
                    Address = new PLC_Address(file, MachineName);
                    PlcDevice = new PLCDevice(m_SettingDevice.PLC.PLCType, Address);
                    IsOpenPlc = PlcDevice.ConnectPLC(m_SettingDevice.PLC.IP, m_SettingDevice.PLC.Port);
                }
                catch { IsOpenPlc = false; }
            }
            else IsOpenPlc = false;

            // Initialize Counter
            if (m_SettingDevice.ENC.UseEncoder)
            {
                try
                {
                    if (m_SettingDevice.ENC.EncoderType == 1)
                    {
                        //성우 Counter
                        InitTrigger(m_SettingDevice.ENC.Port[0], m_SettingDevice.ENC.Port[1]);
                        Trigger_CA.InitTrigger(m_SettingDevice.ENC.Direction[0], m_SettingDevice.ENC.Count[0],
                            m_SettingDevice.ENC.Delay1[0], m_SettingDevice.ENC.Delay2[0], m_SettingDevice.ENC.Width[0]);
                        Trigger_BA.InitTrigger(m_SettingDevice.ENC.Direction[1], m_SettingDevice.ENC.Count[1],
                            m_SettingDevice.ENC.Delay1[1], m_SettingDevice.ENC.Delay2[1], m_SettingDevice.ENC.Width[1]);
                    }
                    //NI counter
                    else StartCount(5);

                    IsOpenCounter = true;
                }
                catch { IsOpenCounter = false; }
            }
            else IsOpenCounter = false;

            // Initialize Light
            if (m_SettingDevice.Light != null)
            {
                LightDevice = LightDeviceManager.Instance;

                IsOpenLight = LightDevice.InitializeLight(m_SettingDevice.Light.ComPort, m_SettingDevice.Light.LightType);
                if (IsOpenLight)
                {
                    LightDevice.LightOff();
                }
            }
            bInit = true;
            #endregion
        }

        #region Handling Counter.
        private void InitTrigger(string portName1, string portName2)
        {
            Trigger_CA = new DCS.SW_Trigger(portName1);
            Trigger_BA = new DCS.SW_Trigger(portName2);
        }

        public void ResetTrigger(int id)
        {
            if (id == 0)
                Trigger_CA.ResetTrigger();
            else
                Trigger_BA.ResetTrigger();
        }

        public void StartCount(int no)
        {
            if (m_SettingDevice != null)
            {
                try
                {
                    if (no == 5)
                    {
                        InitCounter(1, m_SettingDevice.ENC.Low[0], m_SettingDevice.ENC.High[0],
                                     1000, m_SettingDevice.ENC.FilterTime[0],
                                     (m_SettingDevice.ENC.UseFilter[0] == 1) ? true : false, 1);
                        InitCounter(1, m_SettingDevice.ENC.Low[1], m_SettingDevice.ENC.High[1],
                                     1000, m_SettingDevice.ENC.FilterTime[1],
                                     (m_SettingDevice.ENC.UseFilter[1] == 1) ? true : false, 2);
                        InitCounter(1, m_SettingDevice.ENC.Low[2], m_SettingDevice.ENC.High[2],
                                     1000, m_SettingDevice.ENC.FilterTime[2],
                                     (m_SettingDevice.ENC.UseFilter[2] == 1) ? true : false, 3);
                    }
                    else
                    {
                        if (no == 0)
                        {
                            InitCounter(1, m_SettingDevice.ENC.Low[0], m_SettingDevice.ENC.High[0],
                                     1000, m_SettingDevice.ENC.FilterTime[0],
                                     (m_SettingDevice.ENC.UseFilter[0] == 1) ? true : false, 1);
                            InitCounter(1, m_SettingDevice.ENC.Low[1], m_SettingDevice.ENC.High[1],
                                     1000, m_SettingDevice.ENC.FilterTime[1],
                                     (m_SettingDevice.ENC.UseFilter[1] == 1) ? true : false, 2);
                        }
                        else
                        {
                            InitCounter(1, m_SettingDevice.ENC.Low[2], m_SettingDevice.ENC.High[2],
                                     1000, m_SettingDevice.ENC.FilterTime[2],
                                     (m_SettingDevice.ENC.UseFilter[2] == 1) ? true : false, 3);
                        }
                    }

                    IsOpenCounter = true;
                }
                catch
                {
                    IsOpenCounter = false;
                }
            }
        }

        private void InitCounter(long anNIDAQmxUsed, long anLowValue, long anHighValue, long alSampleTime, double alMinPulse, bool abFilterEnable, int no)
        {
            // 2012-05-15 suoow2.
            // 21호기 횡전개간 Trigger 노이즈가 발생하였음. 19호기와 달리 배선상태가 불안정한 것이 원인으로 보임.
            // 노이즈 필터링 코드를 추가하려 하였으나 관련 레퍼런스를 찾기 어려워 NIDAQmx Library로 해결할 수 없었음.
            // 따라서, 21호기에서는 NIDAQmx Library 대신 nidaq32.dll(NI Traditional)을 import하여 Legacy 방식대로 노이즈를 필터링하도록 조치함.
            try
            {
                if (anNIDAQmxUsed == 0)
                    NIDAQ.StartCounter((uint)anLowValue, (uint)anHighValue);
                else
                {
                    if (no == 1)
                    {
                        //BAV05
                        CounterDevice1 = InitCounter(CounterDevice1, "Dev1/ctr0", "/Dev1/PFI39", 1, Convert.ToInt32(anLowValue), Convert.ToInt32(anHighValue), alSampleTime, alMinPulse, abFilterEnable, 0);

                    }
                    else if (no == 2)
                    {
                        //BAV05 NI보드 사용
                        CounterDevice2 = InitCounter(CounterDevice2, "Dev1/ctr1", "/Dev1/PFI35", 1, Convert.ToInt32(anLowValue), Convert.ToInt32(anHighValue), alSampleTime, alMinPulse, abFilterEnable, 0);
                    }
                    else
                    {
                        //BAV05 NI보드 사용
                        CounterDevice3 = InitCounter(CounterDevice3, "Dev1/ctr2", "/Dev1/PFI31", 1, Convert.ToInt32(anLowValue), Convert.ToInt32(anHighValue), alSampleTime, alMinPulse, abFilterEnable, 0);
                    }
                }
            }
            catch
            {
                SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
                if (sendRunner != null)
                    sendRunner("PCS", SeverityLevel.FATAL, "Counter를 시작하는데 실패하였습니다.");
            }
        }

        private IntPtr InitCounter(IntPtr CounterDevice, String aszDevice, String aszChannel, long anNIDAQmxUsed, long anLowValue, long anHighValue, long alSampleTime, double alMinPulse, bool abFilterEnable, int nDelay)
        {
            // 2012-05-15 suoow2.
            // 21호기 횡전개간 Trigger 노이즈가 발생하였음. 19호기와 달리 배선상태가 불안정한 것이 원인으로 보임.
            // 노이즈 필터링 코드를 추가하려 하였으나 관련 레퍼런스를 찾기 어려워 NIDAQmx Library로 해결할 수 없었음.
            // 따라서, 21호기에서는 NIDAQmx Library 대신 nidaq32.dll(NI Traditional)을 import하여 Legacy 방식대로 노이즈를 필터링하도록 조치함.
            IntPtr pHandle = new IntPtr();
            try
            {
                pHandle = DCS.NIDAQmx.StartCount(CounterDevice, aszDevice, aszChannel, Convert.ToInt32(anLowValue), Convert.ToInt32(anHighValue), alSampleTime, alMinPulse, abFilterEnable, nDelay);

                if (anNIDAQmxUsed == 1)
                {
                    if (pHandle == (IntPtr)0)
                    {
                        Debug.WriteLine("************************** Counter Error ******************************");
                    }
                }
            }
            catch
            {
                SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
                if (sendRunner != null)
                    sendRunner("PCS", SeverityLevel.FATAL, "Counter를 시작하는데 실패하였습니다.");
            }

            return pHandle;
        }

        public void SCACounter(int no)
        {
            if (m_SettingDevice != null)
            {
                try
                {
                    if (no == 5)
                    {
                        DCS.NIDAQmx.SCACount(CounterDevice1);
                        DCS.NIDAQmx.SCACount(CounterDevice2);
                        DCS.NIDAQmx.SCACount(CounterDevice3);
                    }
                    else
                    {
                        if (no == 0)
                        {
                            DCS.NIDAQmx.SCACount(CounterDevice1);
                            DCS.NIDAQmx.SCACount(CounterDevice2);
                        }

                        else
                        {
                            DCS.NIDAQmx.SCACount(CounterDevice3);
                        }
                    }                       
                }
                catch
                {
                    SendLogMessageEventHandler sendRunner = SendLogMessageEvent;
                    if (sendRunner != null)
                        sendRunner("PCS", SeverityLevel.FATAL, "Counter를 종료하는데 실패하였습니다.");
                }
            }
        }
        #endregion Handling Counter.

        #region Handling Light.
        // Set Light Value.
        public static void SetLightValue(int no, int[] arrLightValue)
        {
            LightDevice.SetLightValue(no, arrLightValue);
        }

        public static void ConvertLightValue5to10Index(string aszModelCode, int[,] val)
        {
        }

        // 현재 조명 값을 DB에 Update.
        public static void SaveLightValue(string aszModelCode, int[,] val)
        {
            if (!string.IsNullOrEmpty(aszModelCode))
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string szLightValue = string.Empty;
                    for (int n = 0; n < val.GetLength(0); n++)
                    {
                        for (int i = 0; i < val.GetLength(1); i++)
                        {
                            szLightValue += val[n, i].ToString("D3") + ";";
                        }
                    }
                    
                    ConnectFactory.DBConnector().StartTrans();
                    try
                    {
                        string strQuery = string.Format("UPDATE model_info SET light_value = '{0}' WHERE model_code = '{1}'", szLightValue, aszModelCode);
                        if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                        {
                            ConnectFactory.DBConnector().Commit();
                        }
                        else // UPDATE model_info fail
                        {
                            ConnectFactory.DBConnector().Rollback();
                        }
                    }
                    catch
                    {
                        if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                        Debug.WriteLine("Exception occured in SaveLightValue(DeviceController.cs");
                    }
                }
            }
        }
        #endregion Handling Light.

        #region Handling Visions.
        public void CreateVision()
        {
            if (Vision == null)
            {
                Vision = new VisionInterface[VID.ALL];
                VisionConnected = new bool[VID.ALL];
            }

            for (int nIndex = 0; nIndex < Vision.Length; nIndex++)
            {
                int FrameGrabberIndex = VID.CalcIndex(nIndex);
                if (nIndex < VID.CA1)
                {
                    if (m_SettingDevice.IS.FGType[FrameGrabberIndex] >= 6)
                        Vision[nIndex] = new VisionInterface(nIndex, true);
                    else
                        Vision[nIndex] = new VisionInterface(nIndex, false);
                }
                else if (nIndex >= VID.CA1 && nIndex < VID.BA1)
                {
                    if (m_SettingDevice.IS.FGType[FrameGrabberIndex] >= 6)
                        Vision[nIndex] = new VisionInterface(nIndex, true);
                    else
                        Vision[nIndex] = new VisionInterface(nIndex, false);
                }
                else
                {
                    if (m_SettingDevice.IS.FGType[FrameGrabberIndex] >= 6)
                        Vision[nIndex] = new VisionInterface(nIndex, true);
                    else
                        Vision[nIndex] = new VisionInterface(nIndex, false);
                }
            }
        }

        public bool ConnectVision(int anTargetVision, string aszIP, int anPort)
        {
            if (anTargetVision < 0 || anTargetVision > Vision.Length)
            {
                return false;
            }
            else
            {
                if (Vision[anTargetVision] == null) return false;
                return Vision[anTargetVision].Connect(aszIP, anPort);
            }
        }

        public void SetAutoInspection(bool abIsAutoInspection)
        {
            if (TheVision != null) // 비정상 케이스.
                return;

            if (Vision == null)
            {
                Vision = new VisionInterface[VID.ALL];
                VisionConnected = new bool[VID.ALL];
            }
            for (int nIndex = 0; nIndex < Vision.Length; nIndex++)
            {
                Vision[nIndex].AutoInspect = abIsAutoInspection;
            }
        }

        // Vision Program과의 연결을 종료한다.
        public void ReleaseVision()
        {
            if (Vision == null) return;
            for (int nIndex = 0; nIndex < Vision.Length; nIndex++)
            {
                Vision[nIndex].DisConnect();
            }
        }
        #endregion Handling Visions.

        #region Finalizer.
        // Online에서 장비 사용을 중단한다.
        public void ReleaseDevice()
        {
            if (IsOpenPlc)
            {
                PlcDevice = null;
                IsOpenPlc = false;
            }
            if (IsOpenCounter)
            {
                if (m_SettingDevice.ENC.EncoderType == 0)
                    SCACounter(5);
                IsOpenCounter = false;
            }
            if (IsOpenLight)
            {
                LightDevice.Dispose();
                IsOpenLight = false;
            }
        }

        // 소멸자.
        ~DeviceController()
        {
            ReleaseVision();
            ReleaseDevice();
        }
        #endregion Finalizer.
    }
}
