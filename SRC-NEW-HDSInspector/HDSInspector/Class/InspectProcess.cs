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
using IGS.Classes;
using PCS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using static HDSInspector.MainWindow;

namespace HDSInspector
{
    public enum ErrorType
    {
        Unknown = 0,
        TrainError = 1,
        BufferError = 2,
        ParamError = 3,
        ImageCuttingError = 4,
        VerifyConnectError = 5
    }

    /// <summary>   Inspect process.  </summary>
    class InspectProcess
    {
        #region Inspect Flag
        // 검사 skip 유무
        /// <summary>
        /// 임시 부활
        /// </summary>
        //public static bool BA1InspectSkip = false;
        //public static bool BA2InspectSkip = false;
        //public static bool BPInspectSkip = false;
        //public static bool CA1InspectSkip = false;
        //public static bool CA2InspectSkip = false;

        public static bool[] BPInspectSkip = new bool[1] { false };
        public static bool[] BAInspectSkip = new bool[1] { false };
        public static bool[] CAInspectSkip = new bool[1] { false };

        public static bool IDSkip = false;

        // 검사 완료 유무
        public static bool[] BPInspectDone = new bool[1] { false };
        public static bool[] BAInspectDone = new bool[1] { false };
        public static bool[] CAInspectDone = new bool[1] { false };

        public static int globalStripID = 0;
        public static bool Laser1_Done = true; // hs - 초기값 true 
        public static bool Laser2_Done = true; // hs - Laser1, 2 모두 Done 상태여야 오더 종료
        public static bool Loader_Done = false;
        public static bool Result_Done = false;

        // UI 뷰 완료 유무
        public static bool View_Done = false;
        public static bool ImageSave_Done = false;
        #endregion

        #region Member variables.
        private readonly MainWindow m_ptrMainWindow;
        private readonly InspectionMonitoring InspectionMonitoringCtrl;
        private readonly DeviceController PCSInstance;
        private readonly bool m_bSimulationMode;

        private int m_TactTimeStart;
        private int m_TactTimeEnd;
        private int m_TactTimeStart2;
        private int m_TactTimeEnd2;
        private double m_TotalTime1;
        private double m_TotalTime2;
        private int m_Count1;
        private int m_Count2;
        private bool Use2DReader;
        private int MachineType;

        Thread InspThreadCA;
        Thread InspThreadBA;
        Thread ResultThread;

        InspectBuffer VerifyBuffer = new InspectBuffer();

        string MachineName;
        public static string Current_ID;
        int[] LightType;
        int BeforeCount;
        public static bool m_bStripEnd = false;

        public static event InspectErrorHandler InspectErrorEvent;

        private int m_CheckTime;
        public static int nRetryCount = 0;
        public static DateTime pop_LastWorkTime = DateTime.Now;
        #endregion


        #region Ctor & Initializer.
        public InspectProcess(bool abTestMode)
        {
            m_bSimulationMode = abTestMode;
            m_ptrMainWindow = Application.Current.MainWindow as MainWindow;
            InspectionMonitoringCtrl = m_ptrMainWindow.InspectionMonitoringCtrl;
            BeforeCount = InspectionMonitoringCtrl.TableDataCtrl.BeforeCount;
            PCSInstance = m_ptrMainWindow.PCSInstance;
            LightType = MainWindow.Setting.SubSystem.Light.LightType;
            IDSkip = !MainWindow.CurrentModel.ITS.UseITS;
            Use2DReader = MainWindow.Setting.General.UseIDReader;
            MachineType = MainWindow.Setting.General.MachineType;
        }

        private void Reinspect()
        {
            m_ptrMainWindow.InspectionMonitoringCtrl.EndLoader = true;
            Action action = delegate
            {
                m_ptrMainWindow.EndInspect.Visibility = Visibility.Visible;
                m_ptrMainWindow.EndInspect.StartTimer();
                m_ptrMainWindow.MainToolBarCtrl.IsEnabled = false;
            };
            m_ptrMainWindow.Dispatcher.Invoke(action);
            while (m_ptrMainWindow.InspectionMonitoringCtrl.EndLoader)
            {
                Thread.Sleep(100);
                if (!m_ptrMainWindow.InspectionMonitoringCtrl.InspectionStarted)
                {
                    MainWindow.Log("PCS", SeverityLevel.DEBUG, "자동 검사가 종료되었습니다. (제품 이송)");
                    m_ptrMainWindow.InspectionMonitoringCtrl.InspectionThreadStarted = false;
                    InspectProcess.LightOff(3);
                    InspectProcess.Loader_Done = false;
                    return;
                }
            }
            InspectProcess.Loader_Done = false;
        }

        public void ClearThread()
        {
            if (InspThreadCA != null)
            {
                InspThreadCA.Abort();
                Thread.Sleep(500);
                InspThreadCA = null;
            }
            if (InspThreadBA != null)
            {
                InspThreadBA.Abort();
                Thread.Sleep(500);
                InspThreadBA = null;
            }
            if (ResultThread != null)
            {
                ResultThread.Abort();
                Thread.Sleep(500);
                ResultThread = null;
            }
        }

        public void ResetTimer()
        {
            m_TotalTime1 = 0;
            m_TotalTime2 = 0;
            m_Count1 = 0;
            m_Count2 = 0;
            Action a = delegate
            {
                m_ptrMainWindow.lblCACycle.Content = string.Format("{0:F2}Sec", 0);
                m_ptrMainWindow.lblCAAverg.Content = string.Format("{0:F2}Sec", 0);
                m_ptrMainWindow.lblBACycle.Content = string.Format("{0:F2}Sec", 0);
                m_ptrMainWindow.lblBAAverg.Content = string.Format("{0:F2}Sec", 0);
                InspectionMonitoringCtrl.ResultTable.txtXOut.Text = "0.00";

            }; m_ptrMainWindow.Dispatcher.Invoke(a);
        }

        // 검사 스레드를 동작시킨다.
        public void Inspection(string aszMachineName)
        {
            MachineName = aszMachineName;
            m_ptrMainWindow.MainToolBarCtrl.SetLabelText("검 사 시 작");
            ResetTimer();

            InitializeInspection();
            if (InspThreadBA != null)
            {
                InspThreadBA.Abort();
                Thread.Sleep(500);
                InspThreadBA = null;
            }
            InspThreadBA = new Thread(InspectProcBA);
            InspThreadBA.Start();

            if (InspThreadCA != null)
            {
                InspThreadCA.Abort();
                Thread.Sleep(500);
                InspThreadCA = null;
            }
            InspThreadCA = new Thread(InspectProcCA);
            InspThreadCA.Start();

            if (ResultThread != null)
            {
                ResultThread.Abort();
                Thread.Sleep(500);
                ResultThread = null;
            }
            ResultThread = new Thread(ResultProc); 
            ResultThread.Start();

            Loader_Done = false;
            Result_Done = false;
        }

        private void InitializeInspection()
        {
            if (!m_bSimulationMode)
            {
                // 조명 값 설정.
                for (int i = 0; i < MainWindow.CurrentModel.LightValue.GetLength(0); i++)/////최대 스캔횟수 4회 일때 10개의 조명 값이 필요
                {
                    int[] val = new int[MainWindow.CurrentModel.LightValue.GetLength(1)];
                    for (int j = 0; j < MainWindow.CurrentModel.LightValue.GetLength(1); j++)
                    {
                        val[j] = MainWindow.CurrentModel.LightValue[i, j];
                    }
                    DeviceController.SetLightValue(i, val);
                }
            }
            PCSInstance.SetAutoInspection(true);
            m_TactTimeStart = Environment.TickCount;
            m_TotalTime1 = 0;
            m_TotalTime2 = 0;

            // PCS 자동 검사 관련 데이터 초기화.
            for (int i = 0; i < VID.BP_PC_Count; i++)
            {
                PCSInstance.Vision[VID.BP1 + i].ClearVisionData();
            }
            for (int i = 0; i < VID.CA_PC_Count; i++)
            {
                if (MainWindow.Setting.SubSystem.IS.UseCASlave && 0 != (VID.CA1 + i) % VID.CA_PC_Count) continue;
                PCSInstance.Vision[VID.CA1 + i].ClearVisionData();
            }
            for (int i = 0; i < VID.BA_PC_Count; i++)
            {
                if (MainWindow.Setting.SubSystem.IS.UseBASlave && 0 != (VID.BA1 + i) % VID.BA_PC_Count) continue;
                PCSInstance.Vision[VID.BA1 + i].ClearVisionData();
            }
        }
        #endregion

        #region Light On / Off
        public static void LightOff(Object obj)
        {
            if (DeviceController.LightDevice != null)
                DeviceController.LightDevice.LightOn((int)obj, false);
        }

        public static void LightOnEx(Object obj)
        {
            if (DeviceController.LightDevice != null)
                DeviceController.LightDevice.LightOn((int)obj, true);
        }

        public static void SetLight(Object Type, Object Index)
        {
            MainWindow.LightInfo LightInfo = MainWindow.Convert_LightIndex((CategorySurface)Type, (int)Index);////////10개의 DB 버퍼공간을 활용하는 방식으로 변경

            if (DeviceController.LightDevice != null)
                DeviceController.LightDevice.SetLight(LightInfo.LightIndex, LightInfo.ValueIndex);
        }
        #endregion

        public void InspectProcCA()
        {
            bool bIsLotStart = true;
            int nLastScanVision_Number = -1;
            MainWindow.Log("PCS", SeverityLevel.DEBUG, "CA면 자동 검사 스레드가 시작되었습니다.");
            #region Declare Local Variables.
            int nSleepCount = 0;

            // hs - Tact time
            Action tackTimeChecker = delegate
            {
                double t = (m_TactTimeEnd - m_TactTimeStart) / (double)1000;
                m_Count1++;
                m_ptrMainWindow.lblCACycle.Content = string.Format("{0:F2}Sec", t);
                m_TotalTime1 += t;
                m_ptrMainWindow.lblCAAverg.Content = string.Format("{0:F2}Sec", m_TotalTime1 / m_Count1);
            };

            Action action = null;
            #endregion Declare Local Variables.
            InspectionMonitoringCtrl.InspectionThreadStarted = true;
            int index = 1;///CA조명 Index
            for (int i = 0; i < InspectionMonitoringCtrl.m_BP_ID.Length; i++) InspectionMonitoringCtrl.m_BP_ID[i] = 0;
            for (int i = 0; i < InspectionMonitoringCtrl.m_CA_ID.Length; i++) InspectionMonitoringCtrl.m_CA_ID[i] = 0;
            while (InspectionMonitoringCtrl.InspectionStarted)
            {
                int plcID = 0;
                m_TactTimeStart = Environment.TickCount;
                int TotalCount = 0;

                if (MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count > MainWindow.Setting.Job.CACount) TotalCount = MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count;
                else TotalCount = MainWindow.Setting.Job.CACount;
                // hs - Strip 검사 loop
                for (int Count = 0; Count < /*m_ptrMainWindow.pTopScanCount*/TotalCount; Count++) 
                {
                    int Scan = Count;                    
                    #region Request Boat
                    int iPos = 0;
                    int camPos = 0;
                    bool BP_Pass = false; bool CA_Pass = false;
                    if (BPInspectSkip.Length != 0 && BPInspectSkip.Length > (int)Math.Round((double)Scan / VID.BP_ScanComplete_Count))
                    {
                        if (!BPInspectSkip[(int)Math.Truncate((double)Scan / VID.BP_ScanComplete_Count)])
                        { BP_Pass = true; SetLight(CategorySurface.BP, Scan / VID.BP_ScanComplete_Count); }
                    }
                    if (CAInspectSkip.Length != 0 && CAInspectSkip.Length > Scan)
                    {
                        if (!CAInspectSkip[Scan])
                        { CA_Pass = true; SetLight(CategorySurface.CA, Scan); }
                    }

                    if (BP_Pass || CA_Pass || (!BP_Pass && !CA_Pass && (Scan == 0)))
                    {
                        while (iPos != Scan + 1 || camPos != BPCampos(Scan) + 1)
                        {
                            iPos = PCSInstance.PlcDevice.RequestBoatCA();
                            if (MainWindow.Setting.SubSystem.PLC.MCType == 1)
                                camPos = PCSInstance.PlcDevice.ReadBoat2CamPos();
                            else
                            {
                                if (iPos != 0) camPos = iPos;
                            }

                            if (!InspectionMonitoringCtrl.InspectionStarted)
                            {
                                MainWindow.Log("PCS", SeverityLevel.DEBUG, "자동 검사가 종료되었습니다. (제품 이송)");
                                InspectionMonitoringCtrl.InspectionThreadStarted = false;
                                LightOff(Scan / 2);
                                LightOff((Scan % 2) + 1);
                                return;
                            }
                            Thread.Sleep(100);
                            #region 공급 완료
                            if (Scan == 0) 
                            {
                                if (MachineType == 0) 
                                {
                                    if (Loader_Done) 
                                    {
                                        while (InspectionMonitoringCtrl.EndLoader)
                                        {
                                            Thread.Sleep(100);
                                            if (!InspectionMonitoringCtrl.InspectionStarted)
                                            {

                                                MainWindow.Log("PCS", SeverityLevel.DEBUG, "자동 검사 CA 가 종료되었습니다. (제품 이송)");
                                                InspectionMonitoringCtrl.InspectionThreadStarted = false;
                                                LightOff(3);
                                                return;
                                            }
                                        }
                                    }
                                    if (!Loader_Done)
                                    {
                                        if (PCSInstance.PlcDevice.RequestDone())
                                        {
                                            //Thread.Sleep(11000); // Loader Done과 Insp ID로 Result done을 체크하는데, insp ID가 증가되기 전에 Loader Done이 먼저 true가 되버림. 따라서 딜레이를 줌.
                                            Loader_Done = true;
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                        if (Scan == 0)
                        {
                            if (MachineType == 0)
                            {
                                Loader_Done = false;
                            }

                            plcID = InspectionMonitoringCtrl.m_BP_ID[Scan];
                            if (MachineType == 0)
                            {
                                InspectionMonitoringCtrl.StripID = "";
                                PCSInstance.PlcDevice.SendID(InspectionMonitoringCtrl.m_BP_ID[Scan]);
                            }
                            else
                                plcID = PCSInstance.PlcDevice.ReadBoat2ID();
                            action = delegate { InspectionMonitoringCtrl.lblTopID.Content = InspectionMonitoringCtrl.m_BP_ID[Scan].ToString("0000"); };
                            m_ptrMainWindow.Dispatcher.Invoke(action);
                            if (plcID != InspectionMonitoringCtrl.m_BP_ID[Scan])
                            {
                                MessageBox.Show(string.Format("InspectionID={0}, PLCID={1}, PLC와 PC ID가 일치 하지 않습니다.\n원점복귀 후 다시 시작해 주시기 바랍니다.", InspectionMonitoringCtrl.m_BP_ID[Scan], plcID));
                                MainWindow.Log("PCS", SeverityLevel.DEBUG, "자동 검사가 종료되었습니다. (Real ID Dismatch)");
                                InspectionMonitoringCtrl.InspectionThreadStarted = false;
                                if (BPInspectSkip.Length != 0 && BPInspectSkip.Length > (int)Math.Round((double)Scan / VID.BP_ScanComplete_Count))
                                    LightOff(Scan / VID.BP_ScanComplete_Count);
                                LightOff((Scan + 1) % 2);
                                return;
                            }              
                        }
                    }
                    
                    if (BPInspectSkip.Length != 0 && BPInspectSkip.Length > (int)Math.Round((double)Scan / VID.BP_ScanComplete_Count))
                    {
                        if (!BPInspectSkip[(int)Math.Truncate((double)Scan / VID.BP_ScanComplete_Count)])
                            LightOnEx(Scan / VID.BP_ScanComplete_Count);
                    }
                    if (CAInspectSkip.Length != 0 && CAInspectSkip.Length > Scan)
                    {
                        if (!CAInspectSkip[Scan])
                            LightOnEx(index + (Scan % 2));
                    }
                    #endregion Request Boat

                    #region 검사시작
                    if (BPInspectSkip.Length != 0 && BPInspectSkip.Length > (int)Math.Round((double)Scan / VID.BP_ScanComplete_Count))
                    {
                        PCSInstance.Vision[VID.BP1 + (int)(Scan / VID.BP_ScanComplete_Count) % VID.BP_PC_Count].Grab_Done = false;
                        PCSInstance.Vision[VID.BP1 + (int)(Scan / VID.BP_ScanComplete_Count) % VID.BP_PC_Count].Inspect_Done = false;
                        PCSInstance.Vision[VID.BP1 + (int)(Scan / VID.BP_ScanComplete_Count) % VID.BP_PC_Count].Grab_Ready = false;
                        BPInspectDone[Scan] = false;
                        if (!BPInspectSkip[(int)Math.Truncate((double)Scan / VID.BP_ScanComplete_Count)])
                            PCSInstance.Vision[VID.BP1 + (int)(Scan / VID.BP_ScanComplete_Count) % VID.BP_PC_Count].GrabAndInspect(Scan % VID.BP_ScanComplete_Count);
                        else
                        {
                            PCSInstance.Vision[VID.BP1 + (int)(Scan / VID.BP_ScanComplete_Count) % VID.BP_PC_Count].Grab_Ready = true;
                            PCSInstance.Vision[VID.BP1 + (int)(Scan / VID.BP_ScanComplete_Count) % VID.BP_PC_Count].Grab_Done = true;
                            PCSInstance.Vision[VID.BP1 + (int)(Scan / VID.BP_ScanComplete_Count) % VID.BP_PC_Count].Inspect_Done = true;
                            BPInspectDone[Scan] = true;
                            InspectionMonitoringCtrl.BP_Done[Scan, InspectionMonitoringCtrl.m_BP_ID[Scan] % 20] = true;
                        }
                    }
                    else
                    {

                        PCSInstance.Vision[VID.BP1 + (int)(Scan / VID.BP_ScanComplete_Count) % VID.BP_PC_Count].Grab_Ready = true;
                        PCSInstance.Vision[VID.BP1 + (int)(Scan / VID.BP_ScanComplete_Count) % VID.BP_PC_Count].Grab_Done = true;
                        PCSInstance.Vision[VID.BP1 + (int)(Scan / VID.BP_ScanComplete_Count) % VID.BP_PC_Count].Inspect_Done = true;
                        
                    }

                    if (CAInspectSkip.Length != 0 && CAInspectSkip.Length > Scan)
                    {
                        PCSInstance.Vision[VID.CA1 + (Scan % VID.CA_PC_Count)].Grab_Done = false;                        
                        PCSInstance.Vision[VID.CA1 + (Scan % VID.CA_PC_Count)].Grab_Ready = false;
                        CAInspectDone[Scan] = false;
                        if (!CAInspectSkip[Scan])
                        {
                            if (Scan == 0)////Lot의 첫번째가 아닌 다음 Strip의 첫번째 Scan은 이전 Strip의 마지막 Scan Inspect Done을 기다려야한다.
                            {
                                if (!bIsLotStart && nLastScanVision_Number >= 0 && nLastScanVision_Number < PCSInstance.Vision.Length && (nLastScanVision_Number == VID.CA1 + (Scan % VID.CA_PC_Count)))
                                {
                                    nSleepCount = 0;
                                    while (!PCSInstance.Vision[nLastScanVision_Number].Inspect_Done)
                                    {
                                        nSleepCount++;
                                        if (nSleepCount > 12000) // 시간 수정하면 안됨, Vision 결과 최대 반환 시간
                                        {
                                            break;
                                        }
                                        Thread.Sleep(10);
                                    }
                                }
                            }
                            if (Scan >= VID.CA_PC_Count)
                            {
                                nSleepCount = 0;
                                while (!PCSInstance.Vision[VID.CA1 + (Scan % VID.CA_PC_Count)].Inspect_Done)
                                {
                                    nSleepCount++;
                                    if (nSleepCount > 12000) // 시간 수정하면 안됨, Vision 결과 최대 반환 시간
                                    {
                                        break;
                                    }
                                    Thread.Sleep(10);
                                }
                            }
                            if (Scan % 2 != 0)
                            {
                                PCSInstance.Vision[VID.CA1].Grab_Ready = false;
                                PCSInstance.Vision[VID.CA1].Grab(VisionDefinition.LINESCAN_NOGRAB, false, (int)(Scan / VID.CA_PC_Count));
                                while (!PCSInstance.Vision[VID.CA1 + ((Scan - 1) % VID.CA_PC_Count)].Grab_Ready)
                                {
                                    if (nSleepCount >= 1500)
                                    {
                                        MainWindow.Log("PCS", SeverityLevel.FATAL, "CA1 Grab 대기 상태가 강제로 통과되었습니다. (Timeout 15sec)", true);
                                        PCSInstance.Vision[VID.CA1].Grab_Ready = true;
                                    }

                                    if (nSleepCount == 300)
                                    {
                                        MainWindow.Log("PCS", SeverityLevel.FATAL, "CA1 Grab 대기 3초 (Timeout 3sec)", true);
                                    }

                                    Thread.Sleep(10);
                                    nSleepCount++;
                                }
                            }
                            PCSInstance.Vision[VID.CA1 + (Scan % VID.CA_PC_Count)].Inspect_Done = false;
                            PCSInstance.Vision[VID.CA1 + (Scan % VID.CA_PC_Count)].GrabAndInspect(Scan / VID.CA_PC_Count);
                            //MainWindow.Log("IS", SeverityLevel.DEBUG, string.Format("InpProcess CA GrabSide : {0}", (int)(Scan / VID.CA_PC_Count)), true);
                        }
                        else
                        {
                            PCSInstance.Vision[VID.CA1 + (Scan % VID.CA_PC_Count)].Grab_Ready = true;
                            PCSInstance.Vision[VID.CA1 + (Scan % VID.CA_PC_Count)].Grab_Done = true;
                            PCSInstance.Vision[VID.CA1 + (Scan % VID.CA_PC_Count)].Inspect_Done = true;
                            CAInspectDone[Scan] = true;
                            InspectionMonitoringCtrl.CA_Done[Scan, InspectionMonitoringCtrl.m_CA_ID[Scan] % 20] = true;
                        }
                    }
                    else
                    {
                        PCSInstance.Vision[VID.CA1 + (Scan % VID.CA_PC_Count)].Grab_Ready = true;
                        PCSInstance.Vision[VID.CA1 + (Scan % VID.CA_PC_Count)].Grab_Done = true;
                        PCSInstance.Vision[VID.CA1 + (Scan % VID.CA_PC_Count)].Inspect_Done = true;
                    }

                    #region Map UI 초기화
                    ////검사 결과가 나올때 Map을 초기화해 주는데, 비동기 UI처리로 인해 검사 결과가 나온 후 Map 처리가 남아있으면, UI표시에 이전 검사 항목이 남는 현상 생김,
                    ////해당 현상을 다음 검사 시작시에 초기화 하도록 하여 없앰,
                    ////하지만 이 방법을 하더라도 다음 검사를 시작할 때 이전 불량 정보에 대한 UI처리가 끝나지 않으면 동일한 문제 발생
                    ////여기서 다음 검사가 UI를 기다리도록 하면 검사 Delay 발생.
                    //if (Scan == 0)
                    //{
                    //    action = delegate
                    //    {
                    //        if (BPInspectSkip.Length != 0 && BPInspectSkip.Length > (int)Math.Round((double)Scan / VID.BP_ScanComplete_Count))
                    //        {
                    //            for (int i = 0; i < BPInspectSkip.Length; i++) m_ptrMainWindow.InspectionMonitoringCtrl.CamCtrl[i].Map.GoodColor();
                    //        }
                    //        for (int i = 0; i < CAInspectSkip.Length; i++) m_ptrMainWindow.InspectionMonitoringCtrl.CamCtrl[i + MainWindow.Setting.Job.BPCount].Map.GoodColor();
                    //    };
                    //    m_ptrMainWindow.Dispatcher.Invoke(action);
                    //}
                    #endregion
                    nSleepCount = 0;
                    while (!(PCSInstance.Vision[VID.BP1 + (int)(Scan / VID.BP_ScanComplete_Count) % VID.BP_PC_Count].Grab_Ready && PCSInstance.Vision[VID.CA1 + (Scan % VID.CA_PC_Count)].Grab_Ready))
                    {
                        if (nSleepCount >= 1500)
                        {
                            MainWindow.Log("PCS", SeverityLevel.FATAL, "BP1 Grab 대기 상태가 강제로 통과되었습니다. (Timeout 15sec)", true);
                            PCSInstance.Vision[VID.BP1 + (int)(Scan / VID.BP_ScanComplete_Count) % VID.BP_PC_Count].Grab_Ready = PCSInstance.Vision[VID.CA1 + (Scan % VID.CA_PC_Count)].Grab_Ready = true;
                        }

                        if (nSleepCount == 300)
                        {
                            MainWindow.Log("PCS", SeverityLevel.FATAL, "BP1 Grab 대기 3초 (Timeout 3sec)", true);
                        }

                        Thread.Sleep(10);
                        nSleepCount++;
                    }

                    if (BP_Pass || CA_Pass || (!BP_Pass && !CA_Pass && (Scan == 0)))
                    {
                        if (MainWindow.Setting.SubSystem.ENC.EncoderType == 1) m_ptrMainWindow.PCSInstance.ResetTrigger(0);
                        PCSInstance.PlcDevice.PassBoatCA();
                    }
                    #endregion 검사시작
                    if (BPInspectSkip.Length != 0 && BPInspectSkip.Length > (int)Math.Round((double)Scan / VID.BP_ScanComplete_Count))
                    {
                        if (!BPInspectSkip[(int)Math.Truncate((double)Scan / VID.BP_ScanComplete_Count)])
                            InspectionMonitoringCtrl.StartScan(Scan / VID.BP_ScanComplete_Count);
                    }
                    if (CAInspectSkip.Length != 0 && CAInspectSkip.Length > Scan)
                    {
                        if (!CAInspectSkip[Scan]) InspectionMonitoringCtrl.StartScan(MainWindow.Setting.Job.BPCount + Scan);
                        nLastScanVision_Number = VID.CA1 + (Scan % VID.CA_PC_Count);
                    }

                    #region Grab done
                    nSleepCount = 0;
                    bool CheckDoneBP = false; bool CheckDoneCA = false;           
                    while (!(CheckDoneBP && CheckDoneCA))
                    {
                        if (BPInspectSkip.Length != 0 && BPInspectSkip.Length > (int)Math.Round((double)Scan / VID.BP_ScanComplete_Count))
                        {
                            CheckDoneBP = PCSInstance.Vision[VID.BP1 + (int)(Scan / VID.BP_ScanComplete_Count) % VID.BP_PC_Count].Grab_Done;
                        }
                        else CheckDoneBP = true;
                        if (CAInspectSkip.Length != 0 && CAInspectSkip.Length > Scan)
                        {
                            CheckDoneCA = PCSInstance.Vision[VID.CA1 + (Scan % VID.CA_PC_Count)].Grab_Done;
                        }
                        else CheckDoneCA = true;
                        bool bp = PCSInstance.Vision[VID.BP1 + (int)(Scan / VID.BP_ScanComplete_Count) % VID.BP_PC_Count].Grab_Done;
                        bool ca = PCSInstance.Vision[VID.CA1 + (Scan % VID.CA_PC_Count)].Grab_Done;
                        nSleepCount++;
                        if (nSleepCount > 1200) // 시간 수정하면 안됨, Vision 결과 최대 반환 시간
                        {
                            MainWindow.Log("VISION", SeverityLevel.DEBUG, "[" + MainWindow.Setting.General.MachineName + "]" + "[InspectProcCA] 영상획득 실패 BP" + ((Scan / VID.BP_ScanComplete_Count) + 1) + " = " + bp + " CA" + (Scan % VID.CA_PC_Count) + 1 + " = " + ca
                                + "BP" + (Scan / VID.BP_ScanComplete_Count + 1) + "연결=" + PCSInstance.Vision[VID.BP1].Connected + "CA" + (Scan % VID.CA_PC_Count + 1) + "연결=" + PCSInstance.Vision[VID.CA1 + (Scan % VID.CA_PC_Count)].Connected);
                            MessageBox.Show("영상획득에 실패하였습니다.\n비전 상태를 확인 후 자동 검사를 재개해 주시기 바랍니다.");
                            InspectionMonitoringCtrl.InspectionThreadStarted = false;
                            if (BPInspectSkip.Length >= (int)Math.Round((double)Scan / VID.BP_ScanComplete_Count))
                                LightOff(Scan / 2);
                            LightOff((Scan % 2) + 1);
                            break;
                        }
                        Thread.Sleep(100);
                    }
                    bIsLotStart = false;
                    #endregion
                    if (BPInspectSkip.Length >= (int)Math.Round((double)Scan / VID.BP_ScanComplete_Count))
                        LightOff(Scan / VID.BP_ScanComplete_Count);
                    LightOff((Scan % 2) + 1);
                    if (BPInspectSkip.Length != 0 && BPInspectSkip.Length > (int)Math.Round((double)Scan / VID.BP_ScanComplete_Count))
                    {
                        if (!BPInspectSkip[(int)Math.Truncate((double)Scan / VID.BP_ScanComplete_Count)])////////////////// BP 검사 완료 기다림
                        {
                            int count = 0;
                            while (!BPInspectDone[Scan])
                            {
                                string mc = MainWindow.Setting.General.MachineName;

                                if (count > 14000)
                                {
                                    MainWindow.Log("VISION", SeverityLevel.DEBUG, "[" + mc + "]" + "[InspectProcCA] 검사결과확인 실패 BP" + (Scan / VID.BP_ScanComplete_Count + 1) + " = " + BPInspectDone[Scan]
                                        + "BP" + (Scan / VID.BP_ScanComplete_Count + 1) + "연결=" + PCSInstance.Vision[VID.BP1].Connected);
                                    MessageBox.Show("상부 검사 결과를 확인 할 수 없습니다. (Timeout 30sec)");
                                    InspectionMonitoringCtrl.InspectionThreadStarted = false;
                                    LightOff(Scan / VID.BP_ScanComplete_Count);
                                    LightOff((Scan % 2) + 1);
                                    return;
                                }
                                if (!InspectionMonitoringCtrl.InspectionStarted) return;
                                Thread.Sleep(10);
                                count++;
                            }
                        }
                    }
                    #region 기존 코드에는 있지만 필요없는 코드
                    //if (!bFirst)
                    //{
                    //    int ncnt = 0;
                    //    while ((!BPInspectDone[TotalCount - 1 -  Scan]) || (!CAInspectDone[TotalCount -1 - Scan]))////이전 검사가 끝났는지 체크?
                    //    {
                    //        string mc = MainWindow.Setting.General.MachineName;
                    //
                    //        if (!InspectionMonitoringCtrl.InspectionStarted || ncnt > 14000)
                    //        {
                    //            MainWindow.Log("VISION", SeverityLevel.DEBUG, "[" + mc + "]" + "[InspectProcCA] 검사결과확인 실패 BP"+ (TotalCount - 1 - Scan) + " = " + BPInspectDone[TotalCount - 1 - Scan] + " CA" + (TotalCount - 1 - Scan) + " = " + CAInspectDone[TotalCount - 1 - Scan]
                    //                + "BP" + (TotalCount - 1 - Scan) + " 연결=" + PCSInstance.Vision[VID.BP1 + (TotalCount - 1 - Scan) / VID.BP_ScanComplete_Count].Connected + "CA"+ (TotalCount - 1 - Scan) +"연결=" + PCSInstance.Vision[VID.CA1 + TotalCount - 1 - Scan].Connected);
                    //            MessageBox.Show("검사 결과를 확인 할 수 없습니다. (Timeout 30sec)");
                    //            InspectionMonitoringCtrl.InspectionThreadStarted = false;
                    //            LightOff((TotalCount - 1 - Scan) / 2);
                    //            LightOff(((TotalCount - 1 - Scan) % 2) + 1);
                    //            return;
                    //        }
                    //        Thread.Sleep(10);
                    //        ncnt++;
                    //    }
                    //}
                    //
                    //bFirst = false;
                    #endregion
                }

                #region Request ID
                if (Use2DReader)
                {
                    if (!IDSkip)
                    {
                        bool reqID = false;
                        while (!reqID)
                        {
                            reqID = PCSInstance.PlcDevice.RequestID();
                            if (!InspectionMonitoringCtrl.InspectionStarted)
                            {
                                MainWindow.Log("PCS", SeverityLevel.DEBUG, "자동 검사가 종료되었습니다. (제품 이송)");
                                InspectionMonitoringCtrl.InspectionThreadStarted = false;
                                return;
                            }
                            Thread.Sleep(10);
                        }
                        Current_ID = InspectionMonitoringCtrl.ReadID();
                        InspectionMonitoringCtrl.m_CurrentID = Current_ID;
                        PCSInstance.PlcDevice.PassID();
                        Thread.Sleep(50);
                    }
                }
                #endregion

                #region GetInspect Result
                int cnt = 0;
                bool Finish = false;
                while (!Finish )
                {
                    bool CADone = true; bool BPDone = true;
                    foreach (var done in CAInspectDone) { CADone = CADone && done; }
                    foreach (var done in BPInspectDone) { BPDone = BPDone && done; }
                    Finish = CADone && BPDone;
                    if (cnt > 14000)
                    {
                        string strCA = ""; string strBP = "";
                        for (int i = 0; i < BPInspectDone.Length; i++) { strBP = " BP" + i + " =" + Convert.ToBoolean(BPInspectDone[i]); }
                        for (int i = 0; i < CAInspectDone.Length; i++) { strCA = " CA" + i + " =" + Convert.ToBoolean(CAInspectDone[i]); }
                        MainWindow.Log("VISION", SeverityLevel.DEBUG, "[" + MainWindow.Setting.General.MachineName + "]" + "[InspectProcCA] 검사결과확인 실패 " + strBP + strCA);
                        MessageBox.Show("상부검사 결과를 확인 할 수 없습니다. (Timeout 30sec)");
                        InspectionMonitoringCtrl.InspectionThreadStarted = false;
                        LightOff(0);
                        LightOff(1);
                        return;
                    }
                    if (!InspectionMonitoringCtrl.InspectionStarted) return;
                    Thread.Sleep(10);
                    cnt++;
                }
                m_TactTimeEnd = Environment.TickCount;
                m_ptrMainWindow.Dispatcher.Invoke(tackTimeChecker);
                if(action != null) m_ptrMainWindow.Dispatcher.Invoke(action);
                for (int i = 0; i < InspectionMonitoringCtrl.m_BP_ID.Length; i++) InspectionMonitoringCtrl.m_BP_ID[i]++;
                for (int i = 0; i < InspectionMonitoringCtrl.m_CA_ID.Length; i++) InspectionMonitoringCtrl.m_CA_ID[i]++;
                #endregion GetInspect Result
            }

            #region 누적 불량 초과시 루틴
            if (MachineType == 1) // hs - BA → CA 검사 끝난 후 누적 불량 초과시
            {
                if (InspectionMonitoringCtrl.m_BadStop)
                {
                    action = delegate
                    {
                        m_ptrMainWindow.conbad.Visibility = Visibility.Visible;
                        m_ptrMainWindow.conbad.SetMap(MainWindow.CurrentModel.Strip.UnitColumn, MainWindow.CurrentModel.Strip.UnitRow, InspectionMonitoringCtrl.m_BadMap, InspectionMonitoringCtrl.m_BP_ID[0]);
                        m_ptrMainWindow.conbad.SetBadPoint(InspectionMonitoringCtrl.BadX, InspectionMonitoringCtrl.BadY);
                        m_ptrMainWindow.MainToolBarCtrl.IsEnabled = false;
                    };
                    m_ptrMainWindow.Dispatcher.Invoke(action);
                    while (InspectionMonitoringCtrl.m_BadStop)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            #endregion

            InspectionMonitoringCtrl.InspectionThreadStarted = false;
            if (DeviceController.LightDevice != null)
                DeviceController.LightDevice.LightOff();

            MainWindow.Log("PCS", SeverityLevel.DEBUG, "자동 검사가 종료되었습니다. (완료)");
        }

        public void InspectProcBA()
        {
            // hs - 검사 loop 실행 전 LotStart 체크
            bool bIsLotStart = true;
            int nLastScanVision_Number = -1;
            MainWindow.Log("PCS", SeverityLevel.DEBUG, "BA면 자동 검사 스레드가 시작되었습니다.");

            #region Declare Local Variables.
            string szMachineName = MainWindow.Setting.General.MachineName;
            string szModelName = InspectionMonitoringCtrl.TableDataCtrl.ModelName;
            string szLotNumber = InspectionMonitoringCtrl.TableDataCtrl.LotNo;
            int nSleepCount = 0;
            // hs - Tact time
            Action tackTimeChecker = delegate
            {
                double t = (m_TactTimeEnd2 - m_TactTimeStart2) / (double)1000;
                m_Count2++;
                m_ptrMainWindow.lblBACycle.Content = string.Format("{0:F2}Sec", t);
                m_TotalTime2 += t;
                m_ptrMainWindow.lblBAAverg.Content = string.Format("{0:F2}Sec", m_TotalTime2 / m_Count2);
            };
           
            Action action = null;     
            int ngSlotNumber = MainWindow.Setting.SubSystem.PLC.NGID;
            int index = 3;///BA 조명 
            #endregion Declare Local Variables.

            
            for (int i = 0; i < InspectionMonitoringCtrl.m_BA_ID.Length; i++) InspectionMonitoringCtrl.m_BA_ID[i] = 0;
            // hs - 전체 검사 loop
            while (InspectionMonitoringCtrl.InspectionStarted)
            {
                int plcID = 0;

                m_TactTimeStart2 = Environment.TickCount;
                // hs - Strip 각 한장에 대한 검사 loop
                for (int Count = 0; Count < MainWindow.Setting.Job.BACount; /*m_ptrMainWindow.pBotScanCount;*/ Count++)
                {
                    int Scan = Count;
                    // hs - 조명 설정
                    if (!BAInspectSkip[Scan]) 
                        SetLight(CategorySurface.BA, Scan);

                    #region Request Boat
                    int iPos = 0;
                    if (!BAInspectSkip[Scan] || (BAInspectSkip[Scan] && Scan == 0))
                    {
                        while (iPos != Scan + 1) // hs - 보트 위치 기다림, 스캔횟수만큼 While문 반복
                        {
                            // hs - 스캔 횟수가 반환된다면(BACount 값 반환)
                            iPos = PCSInstance.PlcDevice.RequestBoatBA(); // Request 하면 스캔 횟수가 반환 되서 오는구나 1,2,1,2 가아니라

                            // hs - 검사 종료 조건(비정상 종료)
                            if (!InspectionMonitoringCtrl.InspectionStarted)
                            {
                                MainWindow.Log("PCS", SeverityLevel.DEBUG, "자동 검사가 종료되었습니다. (제품 이송)");
                                InspectionMonitoringCtrl.InspectionThreadStarted = false;
                                LightOff(index + Scan);
                                return;
                            }
                            Thread.Sleep(100);

                            // hs - 제품을 BA Boat에 공급하고 확인하는 코드
                            #region 공급 완료 
                            if (Scan == 0)
                            {
                                if (MachineType == 1) // hs - 8호기 MachineType = 1(BA면 우선검사, 턴테이블), 11호기 MachineType = 0(CA,BP면 우선검사, 흡착테이블)
                                {
                                    if (Loader_Done) 
                                    {
                                        // hs - EndLoader = true는 Reinspect에서 된다. (완전 끝나면 EndLoader  = true) 
                                        while (InspectionMonitoringCtrl.EndLoader)
                                        {
                                            Thread.Sleep(100);
                                            if (!InspectionMonitoringCtrl.InspectionStarted)
                                            {

                                                MainWindow.Log("PCS", SeverityLevel.DEBUG, "자동 검사 BA 가 종료되었습니다. (제품 이송)");
                                                InspectionMonitoringCtrl.InspectionThreadStarted = false;
                                                LightOff(index + Scan);
                                                return;
                                            } 
                                        }
                                    }
                                    if (!Loader_Done)
                                    {
                                        if (PCSInstance.PlcDevice.RequestDone()) // hs - 로더 완료 신호 요청
                                        {
                                            Loader_Done = true;
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }

                    // hs - plcID와 BAID 동일한지 확인(왜 하는거지?)
                    if (Scan == 0) 
                    {
                        if (MachineType == 1) // hs - 8호기 MachineType = 1, 11호기 MachineType = 0
                        {
                            Loader_Done = false;  // hs - 첫 스캔 Loader_Done = true를 해주는데 다음 Strip을 받기위해 Loader_Done을 초기화 해주는것.
                        }

                        plcID = InspectionMonitoringCtrl.m_BA_ID[Scan];
                        if (MachineType == 1)
                        {
                            InspectionMonitoringCtrl.StripID = "";
                            PCSInstance.PlcDevice.SendID(InspectionMonitoringCtrl.m_BA_ID[Scan]);
                        }
                        else
                            plcID = PCSInstance.PlcDevice.ReadBoat2ID();

                        action = delegate { InspectionMonitoringCtrl.lblBotID.Content = InspectionMonitoringCtrl.m_BA_ID[Scan].ToString("0000"); };
                        m_ptrMainWindow.Dispatcher.Invoke(action);
                        if (plcID != InspectionMonitoringCtrl.m_BA_ID[Scan])
                        {
                            MessageBox.Show(string.Format("InspectionID={0}, PLCID={1}, PLC와 PC ID가 일치 하지 않습니다.\n원점복귀 후 다시 시작해 주시기 바랍니다.", InspectionMonitoringCtrl.m_BA_ID[Scan], plcID));
                            MainWindow.Log("PCS", SeverityLevel.DEBUG, "자동 검사가 종료되었습니다. (Real ID Dismatch)");
                            InspectionMonitoringCtrl.InspectionThreadStarted = false;
                            LightOff(index + Scan);
                            return;
                        }
                    }
                    
                    // hs - 조명 On
                    if (!BAInspectSkip[Scan])
                        LightOnEx(index /*index = 3*/ + Scan % 2);
                    else
                        LightOff(index + Scan % 2);
                    #endregion Request Boat      
                    
                    // hs - 조명 설정 → 제품 공급 → plcID확인 → 조명 On
                    #region 검사시작
                    if (!BAInspectSkip[Scan])
                    {
                        PCSInstance.Vision[VID.BA1 + (Scan % VID.BA_PC_Count)].Grab_Done = false;
                        PCSInstance.Vision[VID.BA1 + (Scan % VID.BA_PC_Count)].Grab_Ready = false;
                        BAInspectDone[Scan] = false;
                        if (Scan == 0)////Lot의 첫번째가 아닌 다음 Strip의 첫번째 Scan은 이전 Strip의 마지막 Scan Inspect Done을 기다려야한다.
                        {
                            // hs - Lot의 첫번째 x, 이후 Strip의 첫번째 Scan은 이전 Strip의 마지막 Inspect_Done을 기다림. (이전 오더의 맨 마지막 Strip과 현재 오더의 맨 첫번째 Strip 검사가 꼬이지 않게 하는건가?)
                            if (!bIsLotStart && nLastScanVision_Number >= 0 && nLastScanVision_Number < PCSInstance.Vision.Length && (nLastScanVision_Number == VID.BA1 + (Scan % VID.BA_PC_Count))) 
                            {
                                nSleepCount = 0;
                                // hs - 이전 strip의 Inspect_Done을 기다린다
                                while (!PCSInstance.Vision[nLastScanVision_Number].Inspect_Done) 
                                {
                                    nSleepCount++;
                                    if (nSleepCount > 12000) // 시간 수정하면 안됨, Vision 결과 최대 반환 시간
                                    {
                                        break;
                                    }
                                    Thread.Sleep(10);
                                }
                            }
                        }
                        if (Scan >= VID.BA_PC_Count)
                        {
                            nSleepCount = 0;
                            while (!PCSInstance.Vision[VID.BA1 + (Scan % VID.BA_PC_Count)].Inspect_Done)
                            {
                                nSleepCount++;
                                if (nSleepCount > 12000) // 시간 수정하면 안됨, Vision 결과 최대 반환 시간
                                {
                                    break;
                                }
                                Thread.Sleep(10);
                            }
                        }
                        if (Scan % 2 != 0) // hs - 홀수 스캔
                        {
                            PCSInstance.Vision[VID.BA1 + (Scan % VID.BA_PC_Count == 1 ? 0 : 0)].Grab_Ready = false;
                            PCSInstance.Vision[VID.BA1].Grab(VisionDefinition.LINESCAN_NOGRAB, false, (int)(Scan / VID.BA_PC_Count));
                            nSleepCount = 0;
                            while (!PCSInstance.Vision[VID.BA1].Grab_Ready)
                            {
                                if (nSleepCount >= 1500) // Grab_Ready가 3초 동안 Set이 안되는 경우 강제로 Set하여 준다. (검사가 진행되도록)
                                {
                                    MainWindow.Log("PCS", SeverityLevel.FATAL, "BA2 Grab 대기 상태가 강제로 통과되었습니다. (Timeout 15sec)", true);
                                    PCSInstance.Vision[VID.BA1].Grab_Ready = true;
                                }

                                if (nSleepCount == 300)
                                {
                                    MainWindow.Log("PCS", SeverityLevel.FATAL, "BA2 Grab 대기 3초 (Timeout 3sec)", true);
                                }

                                Thread.Sleep(10);
                                nSleepCount++;
                            }
                        }

                        PCSInstance.Vision[VID.BA1 + (Scan % VID.BA_PC_Count)].Inspect_Done = false;
                        PCSInstance.Vision[VID.BA1 + (Scan % VID.BA_PC_Count)].GrabAndInspect(Scan / VID.BA_PC_Count);
                        //MainWindow.Log("IS", SeverityLevel.DEBUG, string.Format("InpProcess BA GrabSide : {0}", (int)(Scan / VID.BA_PC_Count)), true);
                    }
                    else 
                    {
                        PCSInstance.Vision[VID.BA1 + (Scan % VID.BA_PC_Count)].Grab_Ready = true;
                        PCSInstance.Vision[VID.BA1 + (Scan % VID.BA_PC_Count)].Grab_Done = true;
                        PCSInstance.Vision[VID.BA1 + (Scan % VID.BA_PC_Count)].Inspect_Done = true;
                        BAInspectDone[Scan] = true;
                        InspectionMonitoringCtrl.BA_Done[Scan, InspectionMonitoringCtrl.m_BA_ID[Scan] % 20] = true;
                    }
                    #region Map UI 초기화
                    //검사 결과가 나올때 Map을 초기화해 주는데, 비동기 UI처리로 인해 검사 결과가 나온 후 Map 처리가 남아있으면, UI표시에 이전 검사 항목이 남는 현상 생김,
                    //해당 현상을 다음 검사 시작시에 초기화 하도록 하여 없앰,
                    //하지만 이 방법을 하더라도 다음 검사를 시작할 때 이전 불량 정보에 대한 UI처리가 끝나지 않으면 동일한 문제 발생
                    //여기서 다음 검사가 UI를 기다리도록 하면 검사 Delay 발생.
                    //if (Scan == 0)
                    //{
                    //    action = delegate
                    //    {
                    //        for (int i = 0; i < BAInspectSkip.Length; i++) m_ptrMainWindow.InspectionMonitoringCtrl.CamCtrl[i + MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount].Map.GoodColor();
                    //        m_ptrMainWindow.InspectionMonitoringCtrl.CamCtrl[Scan + MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount].Map.GoodColor();
                    //    };
                    //    m_ptrMainWindow.Dispatcher.Invoke(action);
                    //}           
                    #endregion
                    nSleepCount = 0;
                    while (!PCSInstance.Vision[VID.BA1 + (Scan % VID.BA_PC_Count)].Grab_Ready)
                    {
                        if (nSleepCount >= 1500)
                        {
                            MainWindow.Log("PCS", SeverityLevel.FATAL, "BA1 Grab 대기 상태가 강제로 통과되었습니다. (Timeout 15sec)", true);
                            PCSInstance.Vision[VID.BA1 + (Scan % VID.BA_PC_Count)].Grab_Ready = true;
                        }

                        if (nSleepCount == 300)
                        {
                            MainWindow.Log("PCS", SeverityLevel.FATAL, "BA1 Grab 대기 3초 (Timeout 3sec)", true);
                        }

                        Thread.Sleep(10);
                        nSleepCount++;
                    }

                    #endregion 검사시작
                    // hs - BA 검사 Skip
                    if (BAInspectSkip[Scan] && Scan == 0)
                    {
                        // hs - EncoderType NI = 0, SW = 1
                        if (MainWindow.Setting.SubSystem.ENC.EncoderType == 1) m_ptrMainWindow.PCSInstance.ResetTrigger(1);
                        PCSInstance.PlcDevice.PassBoatBA();
                    }

                    if (!BAInspectSkip[Scan])
                    {
                        if (MainWindow.Setting.SubSystem.ENC.EncoderType == 1) m_ptrMainWindow.PCSInstance.ResetTrigger(1);
                        PCSInstance.PlcDevice.PassBoatBA();
                        // hs - 여기는 왜 BPCount, CACount를 더하지?
                        InspectionMonitoringCtrl.StartScan(MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount + Scan); 
                        nLastScanVision_Number = VID.BA1 + (Scan % VID.BA_PC_Count);
                    }

                    #region Grab done
                    if (!BAInspectSkip[Scan])
                    {
                        nSleepCount = 0;
                        while (!PCSInstance.Vision[VID.BA1 + (Scan % VID.BA_PC_Count)].Grab_Done)
                        {
                            bool ba = PCSInstance.Vision[VID.BA1 + (Scan % VID.BA_PC_Count)].Grab_Done;
                            nSleepCount++;
                            if (nSleepCount > 12000) // 시간 수정하면 안됨, Vision 결과 최대 반환 시간
                            {
                                MainWindow.Log("VISION", SeverityLevel.DEBUG, "[" + MainWindow.Setting.General.MachineName + "]" + "[InspectProcCA] 영상획득 실패 BA" + (Scan % VID.BA_PC_Count) + 1 + " = " + ba
                                    + "BA1연결=" + PCSInstance.Vision[VID.BA1 + (Scan % VID.BA_PC_Count)].Connected);
                                MessageBox.Show("영상획득에 실패하였습니다.\n비전 상태를 확인 후 자동 검사를 재개해 주시기 바랍니다.");
                                InspectionMonitoringCtrl.InspectionThreadStarted = false;
                                LightOff(3);
                                break;
                            }
                            Thread.Sleep(10);
                        }
                    }
                    
                    LightOff(index + Scan);
                    // hs - LotStart 체크
                    bIsLotStart = false;
                    if (!InspectionMonitoringCtrl.InspectionStarted) break;
                    #endregion grab done
                }

                #region GetInspect Result
                int cnt = 0;
                bool Finish = false;
                while (!Finish)
                {
                    bool BADone = true;
                    foreach (var done in BAInspectDone) { BADone = BADone && done; }
                    Finish = BADone;
                    if (cnt > 14000)
                    {
                        string strBA = "";
                        for (int i = 0; i < BAInspectDone.Length; i++) { strBA = " BA" + i + " =" + Convert.ToBoolean(BAInspectDone[i]); }
                        MainWindow.Log("VISION", SeverityLevel.DEBUG, "[" + MainWindow.Setting.General.MachineName + "]" + "[InspectProcBA] 검사결과확인 실패 " + strBA);
                        MessageBox.Show("하부검사 결과를 확인 할 수 없습니다. (Timeout 30sec)");
                        InspectionMonitoringCtrl.InspectionThreadStarted = false;
                        LightOff(0);
                        return;
                    }
                    if (!InspectionMonitoringCtrl.InspectionStarted) return;
                    Thread.Sleep(10);
                    cnt++;
                }
                m_TactTimeEnd2 = Environment.TickCount;
                m_ptrMainWindow.Dispatcher.Invoke(tackTimeChecker);
                if(action != null) m_ptrMainWindow.Dispatcher.Invoke(action);                
                for(int i = 0; i < InspectionMonitoringCtrl.m_BA_ID.Length; i++) InspectionMonitoringCtrl.m_BA_ID[i]++; // hs - BA에 들어온 Strip 개수
                #endregion GetInspect Result  

                #region 누적 불량 초과시 루틴
                if (MachineType == 0) // hs - CA → BA 검사 끝난 후 누적 불량 초과시
                {
                    if (InspectionMonitoringCtrl.m_BadStop)
                    {
                        action = delegate
                        {
                            m_ptrMainWindow.conbad.Visibility = Visibility.Visible;
                            m_ptrMainWindow.conbad.SetMap(MainWindow.CurrentModel.Strip.UnitColumn, MainWindow.CurrentModel.Strip.UnitRow, InspectionMonitoringCtrl.m_BadMap, InspectionMonitoringCtrl.m_BA_ID[0]);
                            m_ptrMainWindow.conbad.SetBadPoint(InspectionMonitoringCtrl.BadX, InspectionMonitoringCtrl.BadY);
                            m_ptrMainWindow.MainToolBarCtrl.IsEnabled = false;
                        };
                        m_ptrMainWindow.Dispatcher.Invoke(action);
                        while (InspectionMonitoringCtrl.m_BadStop)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                #endregion
            }

            InspectionMonitoringCtrl.InspectionThreadStarted = false;
            if (DeviceController.LightDevice != null)
                DeviceController.LightDevice.LightOff();

            MainWindow.Log("PCS", SeverityLevel.DEBUG, "자동 검사가 종료되었습니다. (완료)");
        }

        private void ResultProc()
        {
            #region Request Result
            bool bIsTrue = false;
            Action action = null;
            int nSleepCount = 0;
            int ResID = 0;
            int InspID;
            int val = 0;

            bool bPausePOP = false;
            m_CheckTime = 0;

            while (InspectionMonitoringCtrl.InspectionStarted)
            {
                nSleepCount = 0;
                bIsTrue = false;

                m_CheckTime = Environment.TickCount;

                while (!bIsTrue) 
                {
                    //POP Time Check
                    if (!bPausePOP && MainWindow.Setting.General.UsePOP)
                        bPausePOP = CheckRunningStateForPOP(bPausePOP, m_CheckTime, 0);

                    nSleepCount++;
                    bIsTrue = PCSInstance.PlcDevice.RequestResult(); // 결과 요구
                    if (!InspectionMonitoringCtrl.InspectionStarted)
                        return;
                    Thread.Sleep(100);
                }

                //POP Restart
                if (bPausePOP && MainWindow.Setting.General.UsePOP)
                    bPausePOP = CheckRunningStateForPOP(bPausePOP, m_CheckTime, 0);

                // hs - 각 검사(BA/CA/BP) true 확인, 검사가 다 완료될때까지 반복
                bIsTrue = false;
                while (!bIsTrue) 
                {
                    bIsTrue = true;
                    for (int Scan = 0; Scan < InspectionMonitoringCtrl.BP_Done.GetLength(0); Scan++)
                    {
                        bIsTrue = bIsTrue && InspectionMonitoringCtrl.BP_Done[Scan, ResID % 20]; 
                    }
                    for (int Scan = 0; Scan < InspectionMonitoringCtrl.CA_Done.GetLength(0); Scan++)
                    {
                        bIsTrue = bIsTrue && InspectionMonitoringCtrl.CA_Done[Scan, ResID % 20];
                    }
                    for (int Scan = 0; Scan < InspectionMonitoringCtrl.BA_Done.GetLength(0); Scan++)
                    {
                        bIsTrue = bIsTrue && InspectionMonitoringCtrl.BA_Done[Scan, ResID % 20];
                    }
                    if (!InspectionMonitoringCtrl.InspectionStarted) break;
                    Thread.Sleep(50);
                }

                action = delegate { m_ptrMainWindow.MainToolBarCtrl.SetLabelText("검 사 중"); };
                m_ptrMainWindow.Dispatcher.Invoke(action);

                int plcID1 = PCSInstance.PlcDevice.ReadResultID();

                action = delegate { InspectionMonitoringCtrl.lblResID.Content = plcID1.ToString("0000"); };
                m_ptrMainWindow.Dispatcher.Invoke(action);
                if (plcID1 != ResID)
                {
                    action = delegate
                    {
                        InspectionMonitoringCtrl.InspectionStarted = false;
                        InspectionMonitoringCtrl.InspectionThreadStarted = false;
                        m_ptrMainWindow.MainToolBarCtrl.InspectionStop(3);
                        m_ptrMainWindow.InspectionStarted = false;
                    };
                    m_ptrMainWindow.Dispatcher.Invoke(action);
                    MainWindow.Log("PCS", SeverityLevel.FATAL, "자동 검사가 강제 종료되었습니다. (Result ID Error)", true);
                    MessageBox.Show("PLC와 PC ID가 일치 하지 않습니다.\n원점복귀 후 다시 시작해 주시기 바랍니다.");                 
                }

                if (bIsTrue) // hs - 각 검사 Done flag 초기화
                {
                    for (int Scan = 0; Scan < InspectionMonitoringCtrl.BP_Done.GetLength(0); Scan++)
                    {
                        InspectionMonitoringCtrl.BP_Done[Scan, ResID % 20] = false;
                    }
                    for (int Scan = 0; Scan < InspectionMonitoringCtrl.CA_Done.GetLength(0); Scan++)
                    {
                        InspectionMonitoringCtrl.CA_Done[Scan, ResID % 20] = false;
                    }
                    for (int Scan = 0; Scan < InspectionMonitoringCtrl.BA_Done.GetLength(0); Scan++)
                    {
                        InspectionMonitoringCtrl.BA_Done[Scan, ResID % 20] = false;
                    }

                    int nBad = 0;
                    if (IDSkip) Current_ID = null;

                    //2D 리더기 인식결과 로그 먼저 남김(2D 알고리즘 결과 반영전 리더기 결과를 남기기 위함)
                    if (Current_ID != null && MainWindow.Setting.General.UseIDReader) Current_ID = InspectionMonitoringCtrl.Log2DMark(Current_ID, ResID);

                    //(2D 리더기 인식 실패할 경우("") or 2D 리더기 미사용(null))의 경우 알고리즘 인식결과 가져옴
                    if (Current_ID == "" || Current_ID == null || !MainWindow.Setting.General.UseIDReader)
                    {
                        Current_ID = InspectionMonitoringCtrl.m_LastResult[ResID % InspectionMonitoring.BUFFER].IDString;
                    }
               
                    //두 결과 합하여 판독
                    val = InspectionMonitoringCtrl.GetResultNum(ResID, Current_ID, out nBad);
                   
                    //상부 고객, 하부 고객, 상부 사내 2D 코드 일치여부 판단 및 저장
                    bool isSame = false;
                    if (MainWindow.CurrentModel.Marker.IDMark > 0)
                    {
                        if (!string.IsNullOrEmpty(InspectionMonitoringCtrl.m_TopResult[ResID % InspectionMonitoring.BUFFER].IDString))
                            MainWindow.IDString = InspectionMonitoringCtrl.m_TopResult[ResID % InspectionMonitoring.BUFFER].IDString;
                        if (!string.IsNullOrEmpty(InspectionMonitoringCtrl.m_BotResult[ResID % InspectionMonitoring.BUFFER].IDString))
                            MainWindow.IDString = InspectionMonitoringCtrl.m_BotResult[ResID % InspectionMonitoring.BUFFER].IDString;

                        //인식시에만 일치여부 판단
                        if (MainWindow.CurrentModel.Name.IndexOf("인식") > -1)
                            isSame = CheckMarkedID(InspectionMonitoringCtrl.m_TopResult[ResID % InspectionMonitoring.BUFFER].IDString, InspectionMonitoringCtrl.m_BotResult[ResID % InspectionMonitoring.BUFFER].IDString);
                        else
                            isSame = true;

                        if (!isSame) val = 2;
                    }

                    if (Current_ID != null)
                    {
                        string strImageStripFolderPath = String.Format(@"{0}/{1}/{2}/{3}/Image/{4}/",
                            MainWindow.Setting.General.ResultPath,
                            InspectionMonitoringCtrl.TableDataCtrl.GroupName,
                            InspectionMonitoringCtrl.TableDataCtrl.ModelName,
                            InspectionMonitoringCtrl.TableDataCtrl.Re_LotNo,
                            Current_ID
                            );
                        if (!Directory.Exists(strImageStripFolderPath)) Common.DirectoryManager.CreateDirectory(strImageStripFolderPath);
                    }
                    if (MainWindow.CurrentModel.UseVerify || MainWindow.CurrentModel.UseAI)
                    {
                        if (Current_ID != null)
                        {
                            if (Current_ID != "" && Current_ID.Length > 4)
                            {
                                string[] Splitid = new string[2];
                                Splitid = Current_ID.Split(' ');
                                string curID = Splitid[0];
                                if (curID == MainWindow.curLotData.ITS_ORDER && val == 2)
                                {
                                    val = 0;
                                }
                            }
                        }
                        if (MainWindow.CurrentModel.UseVerify)
                        {
                            int nRet = InspectionMonitoringCtrl.CreateStripMap(ResID);
                            if (nRet != 0)
                            {
                                action = delegate
                                {
                                    InspectionMonitoringCtrl.InspectionStarted = false;
                                    InspectionMonitoringCtrl.InspectionThreadStarted = false;
                                    m_ptrMainWindow.MainToolBarCtrl.InspectionStop(4);
                                    m_ptrMainWindow.InspectionStarted = false;
                                };
                                m_ptrMainWindow.Dispatcher.Invoke(action);
                                MainWindow.Log("PCS", SeverityLevel.FATAL, string.Format("자동 검사가 강제 종료되었습니다. (VRS Map Error {0} ID: {1})", nRet, Current_ID), true);
                                MessageBox.Show("VRS 맵을 생성 할 수 없습니다. 경로 설정을 확인 해 주세요.");
                            }
                        }
                    }

                    if ((MainWindow.CurrentModel.ITS.UseITS && val != 2) && !MainWindow.CurrentModel.UseAI)
                    {
                        if (!InspectionMonitoringCtrl.CreateResultFile(ResID, Current_ID))//result map 만들기
                        {
                            //LOT 불일치 경우 전체 스트립 폐기 카운트, 안해줄 시 카운트 양품으로 됨
                            InspectionMonitoringCtrl.m_LastResult[ResID % InspectionMonitoring.BUFFER].AutoNG = true;
                            InspectionMonitoringCtrl.m_LastResult[ResID % InspectionMonitoring.BUFFER].SetAll(MainWindow.NG_Info.BadNameToID("ID미인식"));
                            val = 2;
                        }
                    }
                 
                    if (val == 2)  // 폐기 조건
                    {
                        if (MainWindow.Setting.SubSystem.PLC.MCType == 0)
                            PCSInstance.PlcDevice.SendResult(5);
                        else PCSInstance.PlcDevice.SendResult(2);
                        action = delegate { InspectionMonitoringCtrl.lblResult.Content = "폐기"; };
                        m_ptrMainWindow.Dispatcher.Invoke(action);
                    }
                    else // 마킹 조건
                    {
                        PCSInstance.PlcDevice.SendResult(1); // hs - LaserBoat 전 핸들러 픽업
                        if (val == 0)
                        {
                            action = delegate { InspectionMonitoringCtrl.lblResult.Content = "양품"; };
                            m_ptrMainWindow.Dispatcher.Invoke(action);
                        }
                        else
                        {
                            action = delegate { InspectionMonitoringCtrl.lblResult.Content = "불량" + nBad.ToString("00"); };
                            m_ptrMainWindow.Dispatcher.Invoke(action);
                        }
                    }

                    if (val != 2)
                    {
                        lock (this)
                        {
                            InspectionMonitoringCtrl.CopyLaserResult(ResID); // hs - 폐기빼고는 전부 LaserResult.ID에 카운트 된댜
                        }
                    }

                    lock (this)
                    {
                        InspectionMonitoringCtrl.AddFailResult(ResID);
                    }
                    m_bStripEnd = true;
                    pop_LastWorkTime = DateTime.Now;
                }
                else
                {
                    action = delegate
                    {
                        InspectionMonitoringCtrl.InspectionStarted = false;
                        InspectionMonitoringCtrl.InspectionThreadStarted = false;
                        m_ptrMainWindow.MainToolBarCtrl.InspectionStop(5);
                        m_ptrMainWindow.InspectionStarted = false;
                    };
                    m_ptrMainWindow.Dispatcher.Invoke(action);
                    PCSInstance.PlcDevice.SetResult();
                    MainWindow.Log("PCS", SeverityLevel.FATAL, "자동 검사가 강제 종료되었습니다. (Sequence Error)", true);
                    MessageBox.Show("자동 검사가 강제 종료되었습니다.\n원점복귀 후 다시 시작해 주시기 바랍니다.");                    
                }


                bIsTrue = true;
                m_CheckTime = Environment.TickCount;
                while (bIsTrue) // 결과 처리가 완료 되서 끝남.
                {
                    //POP Time Check
                    if (!bPausePOP && MainWindow.Setting.General.UsePOP)
                        bPausePOP = CheckRunningStateForPOP(bPausePOP, m_CheckTime, 0);

                    nSleepCount++;
                    bIsTrue = PCSInstance.PlcDevice.RequestResult(); // 결과 요구
                    if (!InspectionMonitoringCtrl.InspectionStarted)
                    {
                        PCSInstance.PlcDevice.SetResult();
                        return;
                    }
                    Thread.Sleep(100);
                }

                //POP Restart
                if (bPausePOP && MainWindow.Setting.General.UsePOP)
                    bPausePOP = CheckRunningStateForPOP(bPausePOP, m_CheckTime, 0);

                PCSInstance.PlcDevice.SetResult();

                if (MachineType == 1) // hs - type 에 따라 BA 시작, CA 시작 다르다
                    InspID = InspectionMonitoringCtrl.m_BA_ID[0]; 
                else
                    InspID = InspectionMonitoringCtrl.m_CA_ID[0];
                
                
                
                if (Loader_Done && ResID + 1 == InspID) // hs - 1이 의심된다고 하셨다, 근데 코드적으로는 맞다.. / InspID : 들어온 Strip 개수가 들어있다.
                {
                    Result_Done = true;
                }

                // hs
                    globalStripID = ResID;
                    ResID++;

                // 마지막 스트립과 결과 스트립이 동일할때 : Result_Done true, 로더 적재 없을 때 : Loader_Done true, 마지막 스트립이 폐기일때
                if (Result_Done && Loader_Done && val == 2) // hs - 폐기일때
                {
                    // hs - 모든 레이저 끝났는지 확인
                    while ((!Laser1_Done || !Laser2_Done))
                    {
                        Thread.Sleep(1000); 
                    }
                    Reinspect(); // hs - 오더 완료창
                }
                else if (Result_Done && Loader_Done && val != 2) // hs - 폐기가 아닐때
                {
                    // hs - 모든 레이저 끝났는지 확인, globlalStripID와 레이저마킹하고 있는 StripID가 동일한지 확인
                    while ((!Laser1_Done || !Laser2_Done) || 
                        (globalStripID != 
                        LaserProcess.PtrInspectionMonitoringCtrl.LaserResult[(LaserProcess.PtrInspectionMonitoringCtrl.m_LaserPos - 1) % 20].ID))
                    {
                        Thread.Sleep(1000);
                    }
                    Reinspect(); // hs - 오더 완료창
                }
                
                 //globalStripID = ResID;
                 //ResID++;
            }
            #endregion Request Result
        }

        private bool CheckRunningStateForPOP(bool bStop, int stdTime, int nState)
        {
            bool bPause = false;        //설비가 비가동 상태인가

            if (MainWindow.UseTimer)
            {
                if (!bStop)
                {
                    //전산상 설비가 가동중인 상태 - 비가동 보고 시도
                    try
                    {
                        int m_CurTime = Environment.TickCount;
                        double m_Margin = (m_CurTime - stdTime) / (double)1000.0;

                        //5분 이상 경과 시, 비가동 보고 시도
                        if (m_Margin > 300 && nRetryCount < 2)
                        {
                            string errMsg, logMsg;
                            switch (nState)
                            {
                                case 0:
                                    logMsg = "루틴: 검사 결과 대기 중 발생";
                                    break;
                                default:
                                    logMsg = "루틴: 정의되지 않은 경우";
                                    break;
                            }

                            if (m_ptrMainWindow.popManager.SetPOPDefaultLoss((int)(m_Margin / 60), out errMsg))
                            {
                                ClientManager.Log(string.Format("POP Default Loss Report Success."));
                                ClientManager.Log(logMsg);
                                bPause = true;
                            }
                            else
                            {
                                ClientManager.Log(string.Format("POP Default Loss Report Fail, ErrMsg: {0}", errMsg));
                                bPause = false;

                                Thread.Sleep(1000);
                                nRetryCount++;
                                if (m_ptrMainWindow.popManager.SetPOPDefaultLoss((int)(m_Margin / 60), out errMsg))
                                {
                                    ClientManager.Log(string.Format("POP Default Loss Report Success."));
                                    ClientManager.Log(logMsg);
                                    bPause = true;
                                }
                                else
                                    nRetryCount++;
                            }
                        }

                        return bPause;
                    }
                    catch { return bPause; }
                }
                else
                {
                    //전산상 설비가 비가동인 상태 - 시작 보고(비가동 종료) 시도
                    try
                    {
                        bPause = true;

                        if (m_ptrMainWindow.popManager.SetPOPStartReport(ref m_ptrMainWindow.POPStartPara, m_ptrMainWindow.POPStartPara.bRestart, out string errMsg))
                        {
                            ClientManager.Log(string.Format("POP Start Report Success. Lot: {0}", m_ptrMainWindow.POPStartPara.strLot));

                            nRetryCount = 0;
                            bPause = false;
                            switch (nState)
                            {
                                case 0:
                                    ClientManager.Log("루틴: 결과 수신 완료");
                                    break;
                            }
                        }
                        else
                        {
                            ClientManager.Log(string.Format("POP Start Report Fail. ErrMsg: {0}", errMsg));
                            bPause = true;
                        }

                        return bPause;
                    }
                    catch { return bPause; }
                }
            }
            else
                return false;
        }

        private bool CheckMarkedID(string cust1, string cust2)
        {
            bool isOK = false;
            string info = "";
            if (string.IsNullOrEmpty(cust1) || string.IsNullOrEmpty(cust2))
            {
                info += "[미인식] ";
            }
            else
            {
                if (cust1 == cust2)
                {
                    info += "[일치] ";
                    isOK = true;
                }
                else
                    info += "[불일치] ";
            }

            info += string.Format("고객1 : {0} 고객2 : {1} 오더 : {2}", cust1, cust2, m_ptrMainWindow.currOrder);

            string folderPath = MainWindow.Setting.General.ResultPath + "/" + "CheckID" + "/" + MainWindow.CurrentModel.Name;

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string filePath = string.Format(@"{0}/{1}.txt", folderPath, m_ptrMainWindow.currOrder);

            List<string> fileLines = new List<string>();

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                fileLines.AddRange(lines);
            }

            fileLines.Add(info);

            using (StreamWriter stream = new StreamWriter(filePath))
            {
                foreach (string line in fileLines)
                {
                    stream.WriteLine(line);
                }
            }

            return isOK;
        }

        public  int BPCampos(int Scan)
        {
            if (MainWindow.Setting.SubSystem.PLC.MCType == 1)
                return Scan % VID.BP_ScanComplete_Count;
            else
                return Scan;
        }

    }

    public class Result_Convert
    {
        public int Target_Viewer;
        public int SectionID;
        public int RoiID;
        public int InspID;
        public int Channel;
        public int LowerThresh;
        public int UpperThresh;
        public int Mode;
        public string Name;
        public int InspectType;
    }
}
