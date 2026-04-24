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
using Common.DataBase;
using Common.Drawing;
using DCS.PLC;
using DNN;
using HDSInspector.SubWindow;
using IGS.Classes;
using Keyence.AR.Communication;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenCvSharp.Extensions;
using OpenCvSharp.Flann;
using PCS;
using PCS.Interface;
using PCS.ModelTeaching;
using PCS.VRS;
using RMS.Generic;
using RMS.Generic.UserManagement;
using RVS.Generic;
using RVS.Generic.Insp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static OpenCvSharp.LineIterator;
using static PCS.VRS.InfoControl;
using static System.Collections.Specialized.BitVector32;

namespace HDSInspector
{
    public delegate void LogEventHandler(Common.LogType log);
    public delegate void InspectErrorHandler(ErrorType err);

    public partial class MainWindow : Window
    {
        public bool debug = false;
        /// <summary>
        /// POP 재검 체크 타이머 사용
        /// </summary>
        public static bool UseTimer;
        public static bool bPOPInit = false;
        public ClientManager popManager;
        public static StandardManager stdManager;
        public static POP_LOT_DATA curLotData;
        public static JobHistoryInfo curJob;
        public POP_START_WINDOW_PARA POPStartPara = new POP_START_WINDOW_PARA();        //bRestart가 True일때, 자동 재시작 시 관련 파라 넣어줘야한다.

        public static string StartUpPath = Directory.GetCurrentDirectory();
        public static event LogEventHandler LogEvent;
        public static Logger MainLogger;
        public static NGInformationHelper NG_Info;

        private Thread timer;
        private LogSettingWindow m_LogSettingDlg;
        public ISProgressWindow ProgressWindow = new ISProgressWindow();

        public RemotingPCSInterface m_RVSObj;
        public ITS_Info_Control ITS_info_control = new ITS_Info_Control();

        public static bool bMCR_TEST_SAVE = false;

        public RegistryReadWrite m_Registry = new RegistryReadWrite("MCR_TEST");
        public RegistryReadWrite m_ScanSkipCountRegistry = new RegistryReadWrite("ScanSkipCount");

        public int m_nCA_EnableCnt = 0;
        public int m_nBA_EnableCnt = 0;
        public int m_nBP_EnableCnt = 0;
        private int m_nTopScanCount = 0;
        public int pTopScanCount
        {
            get { return m_nTopScanCount; }
            set
            {
                if (m_nTopScanCount != value)
                {
                    m_nTopScanCount = value;
                    m_ScanSkipCountRegistry.Write("pTopScanCount", value);
                }
            }
        }
        private int m_nBotScanCount = 0;
        public int pBotScanCount
        {
            get { return m_nBotScanCount; }
            set
            {
                if (m_nBotScanCount != value)
                {
                    m_nBotScanCount = value;
                    m_ScanSkipCountRegistry.Write("pBotScanCount", value);
                }
            }
        }

        public class CheckBoxData
        {
            public string Name { get; set; }
            public string Content { get; set; }
            public string BindingPath { get; set; }
            public bool IsChecked { get; set; }
            public CategorySurface Type { get; set; }
            public bool IsEnable { get; set; }
        } 

        public DeviceController PCSInstance
        {
            get;
            set;
        }

        private bool m_bTeachingDataLoaded;
        private bool m_bInspectionStarted;
        public bool InspectionStarted
        {
            get { return m_bInspectionStarted; }
            set { m_bInspectionStarted = value; }
        }

        public TeachingWindow TeachingDlg
        {
            get
            {
                return m_TeachingDlg;
            }
        }

        private TeachingWindow m_TeachingDlg;

        public static ParametersLocking m_LockingData = new ParametersLocking();

        public static ParametersLocking LockingData
        {
            get
            {
                return m_LockingData;
            }
            set
            {
                m_LockingData = value;
            }
        }
        public static Setting Setting;

        public static Networks Dnn_Net;

        public static PCS.ELF.AVI.Group CurrentGroup
        {
            get
            {
                return PCS.ELF.AVI.ModelManager.SelectedGroup;
            }
        }

        public static PCS.ELF.AVI.ModelInformation CurrentModel
        {
            get
            {
                return PCS.ELF.AVI.ModelManager.SelectedModel;
            }
        }

        public DateTime StartTime
        {
            get { return InspectionMonitoringCtrl.ResultTable.TableData.StartTime; }
            set { InspectionMonitoringCtrl.ResultTable.TableData.StartTime = value; }
        }

        public TimeSpan RunTime
        {
            get { return InspectionMonitoringCtrl.ResultTable.TableData.RunTime; }
            set { InspectionMonitoringCtrl.ResultTable.TableData.RunTime = value; }
        }

        public struct StructSectionPostion
        {
            public System.Windows.Point DefectSectionPosition { get; set; }
            public System.Windows.Point IDSectionPosition { get; set; }       
        }

        public string Week;
        public bool m_bPause = false;
        public bool m_bPause2 = false;

        public bool DBStatus = false;
        public static int m_CurrentRGBIndex = 0;
        public static StructSectionPostion[] SectionPosition = new StructSectionPostion[10];
        //public static System.Windows.Point[] SectionPosition = new System.Windows.Point[10];//최대 10 스캔

        public string currOrder;
        public static bool[] AutoNG_Check = new bool[100];
        public static bool UseOuter = false;
        public bool StaticMark = false;
        public bool SkipData = false;
        public static string IDString;
        public static int historyID = -1;
        public static int OldMarkID;
        public static string VRSMapPath;
        public static string VRSBinCodeTableName;
        public static List<PCS.VRS.MapConverter> lstConv = new List<PCS.VRS.MapConverter>();
        public static List<Result_Convert> lstRC = new List<Result_Convert>();
        public VerifyResult m_nVerifyDoneMassage = VerifyResult.None;
        public int[,] m_aVerifyResult;

        public int m_Customer;
        public string First_Mark;

        public static int m_nTotalTeachingViewCount = 3;
        public int nTotalTeachingViewCount = 3;
        public static int m_nTotalScanCount = 3;
        public int nTotalScanCount = 3;
        public int[] InspTypeStartIndex = new int[3];
        #region Ctor.
        public MainWindow(Logger logger)
        {
            Setting = new Setting(Directory.GetCurrentDirectory() + "\\..\\Config");
            Setting.Load();
            m_nTotalTeachingViewCount = Setting.Job.BPCount * VID.BP_ScanComplete_Count + Setting.Job.CACount + Setting.Job.BACount;
            nTotalTeachingViewCount = m_nTotalTeachingViewCount;
            m_nTotalScanCount = Setting.Job.BPCount + Setting.Job.CACount + Setting.Job.BACount;
            nTotalScanCount = m_nTotalScanCount;
            SectionPosition = new StructSectionPostion[nTotalScanCount];
            ConnectDb();
            VRSMapPath = MainWindow.Setting.General.VRSDBIP;
            VRSBinCodeTableName = MainWindow.Setting.General.VRSBinCodeTableName;
            string strVRSUSEDB = MainWindow.Setting.General.UseVRSDB ? "1" : "0";
            if (strVRSUSEDB == "1")
                if (!PCS.VRS.InfoControl.ReadMapConv(ref lstConv, VRSBinCodeTableName))
                    MessageBox.Show("VRS DB 연결 오류");

            InitializeComponent();
            InitializeEvent();
            InitializeDevice();  
            Load_Net(MainWindow.Setting.General.PathAIWeights);

            if (m_LogSettingDlg == null)
                m_LogSettingDlg = new LogSettingWindow(MainWindow.Setting.General);

            //결과 데이터 정보 서버 확인
            //ResultDataServerCheck();

            SetNG_Info();

            MainLogger = logger;
            CleanLog();
            bMCR_TEST_SAVE = m_Registry.Read("bMCR_TEST_SAVE", bMCR_TEST_SAVE);
            pTopScanCount = m_ScanSkipCountRegistry.Read("pTopScanCount", pTopScanCount);
            pBotScanCount = m_ScanSkipCountRegistry.Read("pBotScanCount", pBotScanCount);
            Log("PCS", SeverityLevel.DEBUG, "메인 프로그램이 시작되었습니다.");
        }
        #endregion
        public static List<List<List<SectionInfo>>> SectionData_ICS = new List<List<List<SectionInfo>>>();
        public static List<List<SectionInfo>> AlignOffset_ICS = new List<List<SectionInfo>>();
        public static System.Windows.Point BP2Section_info = new System.Windows.Point();
        public static Modelinformation ModelInfo = new Modelinformation();
        #region Initializer.       
        // DB 접속.
        public void ConnectDb()
        {
            string strDBIP = MainWindow.Setting.General.DBIP;
            string strDBPort = MainWindow.Setting.General.DBPort;

            string strCon = string.Format("server={0}; user id={1}; password={2}; database=bgadb; port={3}; pooling=false", strDBIP, "root", "mysql", strDBPort);
            ConnectFactory.CreateConnection(strCon);

            //VRS DB
            string strVRSDBIP = MainWindow.Setting.General.VRSDBIP;
            string strVRSDBPort = MainWindow.Setting.General.VRSDBPort;
            string strVRSUSEDB = MainWindow.Setting.General.UseVRSDB ? "1" : "0";
            string strITSSUSEDB = MainWindow.Setting.General.UseITSDB ? "1" : "0";
            string strITSDBIP = MainWindow.Setting.General.ITSDBIP;
            string strITSDBPort = MainWindow.Setting.General.ITSDBPort;

            // VRS, ITS 동일 DB 사용
            // 이력 기록을 위해 무조건 초기화 필요 - Harry
            PCS.VRS.InfoControl.CreateConnectDB(strVRSDBIP, strVRSDBPort, "igsdb_bga");
        }       
        /*
         * 불량 정보 DB 동기화
         */
        private void SetNG_Info()
        {
            //DB로 부터 읽어 오는 것 추가요
            //DB는 로컬 DB와 동기화 할 수 있도록
            ObservableCollection<Bad_Info> infos = PCS.ELF.AVI.ModelManager.Get_Result_Info();
            NG_Info = new NGInformationHelper(infos);
        }

        public void UpdatePriority(NGInformationHelper ng)
        {
            PCS.ELF.AVI.ModelManager.UpdateBadPriority(ng);
            MainWindow.NG_Info = ng;
        }

        private void InitializeEvent()
        {
            this.Loaded += MainWindow_Loaded;
            this.btnResetTimer.Click += btnResetTimer_Click;
            VisionInterface.SendLogMessageEvent += SendLogMessageEvent;
            DeviceController.SendLogMessageEvent += SendLogMessageEvent;
            MainToolBar.MenuChangeEvent += MenuChangeEvent;
            InspectProcess.InspectErrorEvent += InspectErrorEvent;
            this.conbad.sendstop += conbad_sendstop;
            this.EndInspect.sendevent += EndInspect_sendevent;
            this.Closing += MainWindow_Closing;
            this.btnServerRetry.Click += BtnServerRetry_Click;
        }

        private void BtnServerRetry_Click(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.Setting.General.UsePOP)
            {
                MessageBox.Show("설비 설정에서 POP 기능이 비활성화 되어 있습니다.");
                return;
            }

            if (bPOPInit)
            {
                if (popManager.ServerConnectionCheck() && popManager.ServerHeartbeat(1))
                    ServerStateChange(true);
                else
                    ServerStateChange(false);
            }
            else
            {
                int nRes = popManager.Initialize();
                if (nRes == -2)
                {
                    MessageBox.Show("POP 서버 DB 연결 실패");
                    ServerStateChange(false);
                }
                else if (nRes == -3)
                {
                    MessageBox.Show("POP 서버에 현재 설비 정보가 없습니다. 개발 담당자에게 의뢰바랍니다.");
                    ServerStateChange(false);
                }
                else
                {
                    InspectionMonitoringCtrl.ReportCtrl.SetManager(ref popManager);
                    InspectionMonitoringCtrl.ReportCtrl.SetLossPath(MainWindow.Setting.General.POP_Path);
                    InspectionMonitoringCtrl.userCtrl.SetUser(MainWindow.Setting.General.POP_LastUser);
                    ServerStateChange(true);
                    popStateCtrl.UpdateState(false);
                }
            }
        }

        private void ServerStateChange(bool bConnect)
        {
            if (bConnect)
            {
                tbServerOn.Visibility = Visibility.Visible;
                tbServerOff.Visibility = Visibility.Hidden;
            }
            else
            {
                tbServerOn.Visibility = Visibility.Hidden;
                tbServerOff.Visibility = Visibility.Visible;
            }
        }

        //Server -> Local DB
        private void ResultDataServerCheck()
        {
            try
            {
                if (!CheckNetworkConnection(MainWindow.Setting.General.POP_IP))
                    return;

                string strServerPath = string.Format(@"//{0}/CommonData", MainWindow.Setting.General.POP_IP);
                if (!Directory.Exists(strServerPath))
                    return;

                strServerPath = String.Format(@"{0}/ResultData.json", strServerPath);
                if (!File.Exists(strServerPath))
                    return;

                List<string[]> results = new List<string[]>();

                using (StreamReader reader = new StreamReader(strServerPath))
                {
                    string strJson = reader.ReadToEnd();
                    JArray jarray = JArray.Parse(strJson);

                    for (int i = 0; i < jarray.Count; i++)
                    {
                        JObject jobject = (JObject)jarray[i];
                        string[] resData = new string[13];

                        resData[0] = jobject["ID"].ToString();
                        resData[1] = jobject["INSP_TYPE"].ToString();
                        resData[2] = jobject["RES_NAME"].ToString();
                        resData[3] = jobject["CLASS_NAME"].ToString();
                        resData[4] = jobject["MES_CODE"].ToString();
                        resData[5] = jobject["MES_FAIL"].ToString();
                        resData[6] = jobject["MAP"].ToString();
                        resData[7] = jobject["COLOR"].ToString();
                        resData[8] = jobject["GROUP_ID"].ToString();
                        resData[9] = jobject["GROUP_NAME"].ToString();
                        resData[10] = jobject["PROC_CODE"].ToString();
                        resData[11] = jobject["PROC_NAME"].ToString();
                        resData[12] = jobject["PRIORITY"].ToString();

                        results.Add(resData);
                    }
                }

                if (results.Count == 0)
                    return;

                if (ConnectFactory.DBConnector() != null)
                {
                    foreach (string[] resData in results)
                    {
                        string strQuery = string.Format("REPLACE INTO result_info (id, insp_type, res_name, class_name, mes_code, mes_fail, map, color, " +
                            "group_id, group_name, proc_code, proc_name, priority) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}')",
                            resData[0], resData[1], resData[2], resData[3], resData[4], resData[5], resData[6], resData[7], resData[8], resData[9], resData[10], resData[11], resData[12]);

                        ConnectFactory.DBConnector().StartTrans();

                        if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                            ConnectFactory.DBConnector().Commit();
                        else
                            ConnectFactory.DBConnector().Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                Log("PCS", SeverityLevel.DEBUG, string.Format("ResultDataServerCheck Exception: {0}", ex.Message), true);
            }
        }

        private bool CheckNetworkConnection(string strIP)
        {
            try
            {
                bool networkUp = NetworkInterface.GetIsNetworkAvailable();
                bool pingRes = false;

                if (networkUp)
                {
                    Ping pingSender = new Ping();
                    PingReply reply = pingSender.Send(strIP, 500);
                    pingRes = reply.Status == IPStatus.Success;
                }

                return pingRes;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void EndInspect_sendevent(SubControl.ConID nID)
        {
            if (nID == SubControl.ConID.Stop)
            {
                //검사 종료
                EndInspect.Visibility = Visibility.Hidden;
                MainToolBarCtrl.IsEnabled = true;
                m_bInspectionStarted = false;
                MainWindow.Log("MainWindow", SeverityLevel.DEBUG, string.Format("검 사 종 료"), true);
                InspectionMonitoringCtrl.StopInspect(true);
                InspectionMonitoringCtrl.SetPOPButton(true);
                InspectionMonitoringCtrl.EndLoader = true;
                MainToolBarCtrl.InspectionStop(8);
            }
            else if (nID == SubControl.ConID.FRestart)
            {
                //폐기 재투입
                InspectionMonitoringCtrl.TableDataCtrl.ResetFail();
                EndInspect.Visibility = Visibility.Hidden;
                MainToolBarCtrl.IsEnabled = true;
                InspectionMonitoringCtrl.EndLoader = false;
                InspectProcess.Result_Done = false;
            }
            else
            {
                //제품 재투입
                EndInspect.Visibility = Visibility.Hidden;
                MainToolBarCtrl.IsEnabled = true;
                InspectionMonitoringCtrl.EndLoader = false;
                InspectProcess.Result_Done = false;
            }
        }

        void conbad_sendstop(int stop, int x, int y)
        {
            if (stop == 0)
            {
                if (MessageBox.Show("마킹이 진행 중일 수 있습니다. 종료 하시겠습니까?", "", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
                Log("PCS", SeverityLevel.DEBUG, "자동검사가 중지되었습니다.");

                if (DeviceController.LightDevice != null)
                    DeviceController.LightDevice.LightOff();

                this.MainToolBarCtrl.IsEnabled = true;
                this.MainToolBarCtrl.InspectionStop(7);
                InspectionMonitoringCtrl.StopInspect();
                m_bInspectionStarted = false;
                InspectionMonitoringCtrl.SetPOPButton(true);

                Border smokeBorder = new Border { Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(196, 196, 196)), Opacity = 0.5 };
                Grid.SetRowSpan(smokeBorder, 5);
                Grid.SetColumnSpan(smokeBorder, 10);
                this.pnlMain.Children.Add(smokeBorder);
                {
                    BusyWindow busy = new BusyWindow();
                    busy.ShowDialog();
                }
                this.pnlMain.Children.Remove(smokeBorder);
            }
            else
            {
                InspectionMonitoringCtrl.m_BadMap[x, y] = 0;
                this.MainToolBarCtrl.IsEnabled = true;
            }

            conbad.Visibility = Visibility.Hidden;
            InspectionMonitoringCtrl.m_BadStop = false;
        }

        // HW 초기화.
        private void InitializeDevice()
        {
            VID vision = new VID(Setting.Job.BPCount, Setting.Job.CACount, Setting.Job.BACount, Setting.Job.BP_PC_Count, Setting.Job.CA_PC_Count, Setting.Job.BA_PC_Count);
            Thread.Sleep(100);
            PCSInstance = new DeviceController(MainWindow.Setting.SubSystem, MainWindow.Setting.General.MachineName, Setting.Job);
            
            ProgressWindow.pnlProgress.Children.Clear();
            for (int nIndex = 0; nIndex < MainWindow.m_nTotalTeachingViewCount; nIndex++)
            {
                if (nIndex < Setting.Job.BPCount * 2)
                {
                    ProgressWindow.AddProgressBar(string.Format("Vision 본드패드" + (nIndex + 1)));
                }
                else if (Setting.Job.BPCount * 2 >= VID.CA1 && nIndex < Setting.Job.BPCount * 2  + Setting.Job.CACount)
                {
                    ProgressWindow.AddProgressBar(string.Format("Vision CA외관" + (nIndex - Setting.Job.BPCount * 2 + 1)));
                }
                else
                {
                    ProgressWindow.AddProgressBar(string.Format("Vision BA외관" + (nIndex - (Setting.Job.BPCount * 2 + Setting.Job.CACount) + 1)));
                }
            }
        }

        private void StartTimer()
        {
            timer = new Thread(Timer);
            timer.Start();
        }

        public void StopTimer()
        {
            if (timer != null)
                timer.Abort();
            timer = null;
        }

        private void btnResetTimer_Click(object sender, RoutedEventArgs e)
        {
            InspectionMonitoringCtrl.ResetCycleTime();
        }

        private void Timer()
        {
            while (true)
            {
                Action action = delegate
                {
                    // CIOpacity();
                    InspectionMonitoringCtrl.TableDataCtrl.RunTime = DateTime.Now - InspectionMonitoringCtrl.TableDataCtrl.StartTime;
                };
                this.Dispatcher.Invoke(action);
                Thread.Sleep(1000);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RemotingRVSInterface.VerifyDoneEvent += (DoneMessage, Result) => { m_nVerifyDoneMassage = DoneMessage; m_aVerifyResult = Result; };
            InspectionMonitoringCtrl.SetWindow(MainWindow.Setting.General.MachineType, MainWindow.Setting.SubSystem.IS.UseCASlave, MainWindow.Setting.SubSystem.IS.UseBASlave, MainWindow.Setting.SubSystem.Laser.DualLaser);
            if (MainWindow.Setting.General.UseIDReader || MainWindow.Setting.General.UseITS)
                InspectionMonitoringCtrl.grd2D.Visibility = Visibility.Visible;
            else
                InspectionMonitoringCtrl.grd2D.Visibility = Visibility.Hidden;
            if (MainWindow.Setting.SubSystem.PLC.UsePLC)
            {
                if (!PCSInstance.IsOpenPlc)
                {
                    InspectionMonitoringCtrl.lampPLC.Status = false;
                    MessageBox.Show("PLC와 연결하지 못하였습니다.", "Connection Error");
                }
                else InspectionMonitoringCtrl.lampPLC.Status = true;
            }
            if (MainWindow.Setting.SubSystem.ENC.UseEncoder)
            {
                if (!PCSInstance.IsOpenCounter)
                    MessageBox.Show("Counter를 초기화하지 못하였습니다.", "Init Error");
            }
            if (MainWindow.Setting.SubSystem.Light.UseLight)
            {
                if (!PCSInstance.IsOpenLight)
                    MessageBox.Show("조명과 연결하지 못하였습니다.", "Connection Error");
            }
            if (MainWindow.Setting.SubSystem.Laser.UseLaser)
            {
                InspectionMonitoringCtrl.LaserDlg = new LaserWindow(this);
            }
            if (MainWindow.Setting.General.UseIDReader)
            {
                bool isOpen = InspectionMonitoringCtrl.reader2D.Connect(MainWindow.Setting.General.IDReaderIP);
                if (!isOpen)
                    MessageBox.Show("2D 바코드리더와 연결하지 못하였습니다.", "Connection Error!");
            }

            if (MainWindow.Setting.General.UsePOP)
            {
                bPOPInit = CreatePOPManager(false);

                //if (!bPOPEnable)
                //    MessageBox.Show("공정 서버 연결 오류");
            }
            else
                bPOPInit = CreatePOPManager(true);

            ServerStateChange(bPOPInit);
            // stdManager = new StandardManager();

            InspectDataManager.SetDefectList(); // 불량 정보에 대한 <Code, Name> 쌍을 가져온다.
            m_TeachingDlg = new TeachingWindow();
            m_TeachingDlg.Owner = this;
            m_TeachingDlg.TeachingViewer.SetTabControl(MainWindow.Setting.General.MachineType, MainWindow.Setting.SubSystem.IS.UseCASlave, MainWindow.Setting.SubSystem.IS.UseBASlave);
            ChangeModel();

        }

        #endregion Initializer.

        private void Load_Net(string path)
        {
            if (!MainWindow.Setting.General.UseAI) return;
            string model = path + "\\BGA-MONO\\BGA-MONO.net";
            if (MainWindow.Setting.SubSystem.IS.FGType[VID.BA1] == 6)
                model = path + "\\BGA-COLOR\\BGA-COLOR.net";
            Dnn_Net = new Networks(model);
            Dnn_Net.Add_Fix_Bad_Name("Align");
            Dnn_Net.Add_Fix_Bad_Name("PSR Shift");
            if (Dnn_Net.IsCreate)
            {
                Log("DNN", SeverityLevel.INFO, "딥러닝이 로드 되었습니다.");
            }
            else
            {
                Log("DNN", SeverityLevel.ERROR, string.Format("딥러닝 로딩에 실패 하였습니다. {0}", path));
            }
        }

        public static YoloResult Predect_Yolo_Net(int anCamID, string astrInfo, System.Windows.Media.Imaging.BitmapSource rimg, System.Windows.Media.Imaging.BitmapSource dimg, int no)
        {
            YoloResult result = new YoloResult();
            result.ClassID = -1;
            result.ClassName = "None";
            result.BadName = "None";
            result.prob = 0.0;

            if (!Dnn_Net.IsCreate)
                return result;

            int nCamID = 0;
            if (anCamID == (int)Surface.BP1 || anCamID == (int)Surface.BP2)
                nCamID = 1;
            else if (anCamID == (int)Surface.CA1 || anCamID == (int)Surface.CA2)
                nCamID = 2;
            else
                nCamID = 3;

            string inspBad = "";
            result = Dnn_Net.Predict_Yolo_BGA(astrInfo, rimg, dimg, nCamID, no, out inspBad);
            if (result.isDetection)
            {
                if (result.ClassName == "None")
                {
                    result.BadName = "None";
                }
            }
            else
            {
                result.ClassID = -1;
                result.ClassName = "None";
                result.BadName = "None";
                result.prob = 0.0;
            }
            return result;
        }

        private void MenuChangeEvent(MenuType selectedMenu)
        {
            switch (selectedMenu)
            {
                case MenuType.MODELMANAGER:
                    ChangeModel();

                    break;
                case MenuType.MODELTEACHING:
                    if (MainWindow.Setting.General.UsePassword)
                    {
                        Password password = new Password();
                        if (password.ShowDialog() != true)
                        {
                            break;
                        }
                    }
                    Log("PCS", SeverityLevel.DEBUG, "모델 티칭 메뉴가 선택되었습니다.");
                    this.Cursor = Cursors.Wait;
                    for (int i = 0; i < MainWindow.CurrentModel.LightValue.GetLength(1); i++)/////최대 스캔횟수 4회 일때 10개의 조명 값이 필요
                    {
                        int[] val = new int[MainWindow.CurrentModel.LightValue.GetLength(1)];
                        for (int n = 0; n < MainWindow.CurrentModel.LightValue.GetLength(1); n++) val[n] = MainWindow.CurrentModel.LightValue[i, n];
                        DeviceController.SetLightValue(i, val);
                    }

                    for (int i = 0; i < VID.BP_PC_Count; i++)
                    {
                        PCSInstance.Vision[VID.BP1 + i].DefectCount = 0;
                        PCSInstance.Vision[VID.BP1 + i].AutoInspect = false;
                    }
                    for (int i = 0; i < VID.CA_PC_Count; i++)
                    {
                        PCSInstance.Vision[VID.CA1 + i].DefectCount = 0;
                        PCSInstance.Vision[VID.CA1 + i].AutoInspect = false;
                    }
                    for (int i = 0; i < VID.BA_PC_Count; i++)
                    {
                        PCSInstance.Vision[VID.BA1 + i].DefectCount = 0;
                        PCSInstance.Vision[VID.BA1 + i].AutoInspect = false;
                    }

                    m_TeachingDlg.lvResult.Items.Clear();

                    if (MainWindow.Setting.SubSystem.Light.UseLight)
                    {
                        m_TeachingDlg.lightcontrol.LoadDevice(DeviceController.LightDevice.LightDevices, MainWindow.Setting.SubSystem.Light.LightType, MainWindow.m_nTotalScanCount);
                        m_TeachingDlg.lightcontrol.LightOn(false);
                    }

                    if (!m_bTeachingDataLoaded)
                    {
                        m_bTeachingDataLoaded = true;

                        m_TeachingDlg.InitializeDialog();
                    }

                    m_TeachingDlg.SetLight();
                    m_TeachingDlg.Title = string.Format("모델 티칭   Group : {0}   Model : {1}   Setter : {2}", CurrentGroup.Name, CurrentModel.Name, UserManager.CurrentUser.ID);

                    try
                    {
                        m_TeachingDlg.ShowDialog();
                    }
                    catch (Exception e)
                    {
                        Log("PCS", SeverityLevel.FATAL, "모델 티칭 창이 예기치 못한 오류로 인해 강제 종료 되었습니다." + e.Message, true);
                        m_TeachingDlg = null;
                        m_TeachingDlg = new TeachingWindow() { Owner = this };
                        m_TeachingDlg.TeachingViewer.SetTabControl(MainWindow.Setting.General.MachineType, MainWindow.Setting.SubSystem.IS.UseCASlave, MainWindow.Setting.SubSystem.IS.UseBASlave);
                        m_bTeachingDataLoaded = false;
                    }

                    this.Cursor = Cursors.Arrow;

                    break;
                case MenuType.RUNINSPECT:
                    
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        string diretoryPath = Path.GetDirectoryName(openFileDialog.FileName);
                        DirectoryInfo FolderPathInfo = new DirectoryInfo(diretoryPath);
                        FileInfo[] Files = FolderPathInfo.GetFiles();
                        if (Files.Length == 0) return;
                        for (int i = 0; i < Files.Count(); i++)
                        {
                            string str = Files[i].Name.ToLower();
                            Bitmap bitmap = new Bitmap(diretoryPath + "\\"+ str);

                            List<string> id = new List<string>();

                            string teststr = MatrixCodeDLL.Native_Decode(bitmap);
                            //id = DataMatrixCode.Algo_Conv_Square_DataMatrix(bitmap, currOrder, "1", i);
                            //
                            //if (id != null && id.Count > 0 && id[0].Trim() != "")
                            //    ;// StripID = id[0];
                            //else
                            //{
                            //    System.IO.File.Copy(diretoryPath + "\\" + str, "C:\\Users\\admin\\Desktop\\양산\\실패" + "\\" + str, true);
                            //    ; //id = DataMatrixCode.GetDataMatrixMJ(bitmap, currOrder, "1", 0);
                            //}
                        }
                    }
                    return;
                    
                    InspectionMonitoringCtrl.EndLoader = false;             
                    if (MainWindow.Setting.General.UseIDReader)
                    {
                        int n = PCSInstance.PlcDevice.ReadIDUsed();
                        if (MainWindow.CurrentModel.ITS.UseITS && n <= 0)
                        {
                            MessageBox.Show("ID 마킹 리드 사용 여부가 틀립니다. PLC 전송 후 다시 시작 해 주세요.");
                            return;
                        }
                        if (!MainWindow.CurrentModel.ITS.UseITS && n > 0)
                        {
                            MessageBox.Show("ID 마킹 리드 사용 여부가 틀립니다. PLC 전송 후 다시 시작 해 주세요.");
                            return;
                        }
                        InspectionMonitoringCtrl.Init2DReaderUI();
                    }

                    Log("PCS", SeverityLevel.DEBUG, "자동검사가 시작되었습니다.");
                    MessageBox.Show("주요 티칭 영역 및 마스크 확인 후 작업바랍니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    this.MainToolBarCtrl.SetMainWindowPtr(this);

                    // 검사 유무 Reset.
                    for (int scan = 0; scan < InspectProcess.CAInspectSkip.Length; scan++) InspectProcess.CAInspectSkip[scan] = false;
                    for (int scan = 0; scan < InspectProcess.BAInspectSkip.Length; scan++) InspectProcess.BAInspectSkip[scan] = false;
                    for (int scan = 0; scan < InspectProcess.BPInspectSkip.Length; scan++) InspectProcess.BPInspectSkip[scan] = false;
                    LotWindow lotdlg = new LotWindow(MainWindow.Setting.SubSystem.Laser.UseLaser, UserManager.CurrentUser.Name, MainWindow.NG_Info);
                    lotdlg.SetMainWindowPtr(this);
                    lotdlg.Owner = this;
                    if ((bool)lotdlg.ShowDialog())
                    {
                        if (MainWindow.Setting.SubSystem.Laser.UseLaser && MainWindow.CurrentModel.UseMarking)
                        {
                            if (!InspectionMonitoringCtrl.LaserDlg.Connected)
                            {
                                MessageBox.Show("레이저 연결을 확인하세요.");
                                return;
                            }
                            InspectionMonitoringCtrl.LaserDlg.InitModelData();
                        }
                        if (MainWindow.Setting.SubSystem.Laser.UseLaser && MainWindow.CurrentModel.UseMarking && MainWindow.CurrentModel.Marker.WeekMark > 0)
                        {
                            InspectionMonitoringCtrl.LaserDlg.week = lotdlg.txtWeek.Text;
                            if (InspectionMonitoringCtrl.LaserDlg.week == "")
                            {
                                MessageBox.Show("제조주차를 확인 할수 없습니다.");
                                return;
                            }
                        }
                        else
                            if (MainWindow.Setting.SubSystem.Laser.UseLaser) InspectionMonitoringCtrl.LaserDlg.week = "";

                        // 검사 유무 Set/Unset.
                        int BPindexCount = 0; int BAindexCount = 0; int CAindexCount = 0;
                        for (int i = 0; i < lotdlg.CheckBoxInspContainer.Children.Count; i++)
                        {
                            if (lotdlg.CheckBoxInspContainer.Children[i] is CheckBox checkBox)
                            {
                                if (checkBox.Name.Contains(CategorySurface.BP.ToString()))
                                {
                                    if (InspectProcess.BPInspectSkip.Length > BPindexCount) InspectProcess.BPInspectSkip[BPindexCount++] = !(bool)checkBox.IsChecked;
                                }
                                else if (checkBox.Name.Contains(CategorySurface.CA.ToString()))
                                {
                                    if (InspectProcess.CAInspectSkip.Length > CAindexCount) InspectProcess.CAInspectSkip[CAindexCount++] = !(bool)checkBox.IsChecked;
                                }
                                else if (checkBox.Name.Contains(CategorySurface.BA.ToString()))
                                {
                                    if (InspectProcess.BAInspectSkip.Length > BAindexCount) InspectProcess.BAInspectSkip[BAindexCount++] = !(bool)checkBox.IsChecked;
                                }
                                InspectionMonitoringCtrl.SetSkipDisplay(i, !(bool)checkBox.IsChecked);
                            }
                        }

                        InspectionMonitoringCtrl.m_UseConBad = (bool)lotdlg.cbConBad.IsChecked;
                        MainWindow.CurrentModel.UseVerify = (bool)lotdlg.chkVerify.IsChecked;
                        MainWindow.CurrentModel.UseAI = (bool)lotdlg.chkUseAI.IsChecked;

                        this.StaticMark = (bool)lotdlg.chkStaticMark.IsChecked;
                        if (StaticMark)
                        {
                            InspectionMonitoringCtrl.StaticMarkResult.Init(MainWindow.CurrentModel.Strip.UnitColumn, MainWindow.CurrentModel.Strip.UnitRow, MainWindow.NG_Info.Priority);
                            int val = MainWindow.NG_Info.BadNameToID("고정불량");
                            for (int i = 0; i < MainWindow.CurrentModel.Strip.UnitColumn; i++)
                            {
                                for (int j = 0; j < MainWindow.CurrentModel.Strip.UnitRow; j++)
                                {
                                    if (lotdlg.map.GetValue(i, j))
                                    {
                                        InspectionMonitoringCtrl.StaticMarkResult.Set(i, j, val);
                                    }
                                }
                            }
                        }
                        Array.Copy(MainWindow.CurrentModel.ScrabInfo, AutoNG_Check, AutoNG_Check.Length);
                        SetCurrentInspectInfo(lotdlg.txtLot.Text.ToString(), lotdlg.txtOperator.Text.ToString(), lotdlg.cbLotType.Text.ToString(), lotdlg.txtCode.Text);
                        currOrder = lotdlg.txtLot.Text.ToString();
                        m_TeachingDlg.InitializeDialog();
                        m_bTeachingDataLoaded = true;
                        m_bInspectionStarted = true;
                        InspectionMonitoringCtrl.SetPOPButton(false);

                        /////결과 표준화
                        //stdManager.AllClear();

                        //stdManager.LOCAL_GROUP_NAME = MainWindow.CurrentGroup.Name;
                        //stdManager.LOCAL_MODEL_NAME = MainWindow.CurrentModel.Name;
                        //stdManager.STD_MODEL_NAME = curLotData.ITEM_CD;
                        //stdManager.STD_ORDER = curLotData.WONO;
                        //stdManager.ITS_ORDER = curLotData.ITS_ORDER;
                        //stdManager.LOCAL_ORDER = InspectionMonitoringCtrl.TableDataCtrl.Re_LotNo;
                        //stdManager.OP_CODE = lotdlg.txtCode.Text.Trim();
                        //stdManager.PROD_TYPE = "STRIP";
                        //stdManager.UNIT_X = MainWindow.CurrentModel.Strip.UnitColumn;
                        //stdManager.UNIT_Y = MainWindow.CurrentModel.Strip.UnitRow;
                        //stdManager.MOLD_SURFACE = "";
                        //stdManager.MOLD_LOCATION = "";
                        //stdManager.MAP_DIR_PATH = MainWindow.Setting.General.XMLMapPath;

                        if (!InspectionMonitoringCtrl.StartInspection())
                        {
                            this.MainToolBarCtrl.InspectionStop(1);
                            m_bInspectionStarted = false;
                            InspectionMonitoringCtrl.SetPOPButton(true);
                            this.ProgressWindow.StopProgress(-1);
                            return;
                        }

                        StartTimer();

                        if (MainWindow.CurrentModel.UseVerify)
                        {
                            #region VRS을 위한 로트 정보 Update
                            PCS.VRS.VRS_LOT_Info LotInfo = new PCS.VRS.VRS_LOT_Info();
                            LotInfo.Group = MainWindow.CurrentGroup.Name;
                            LotInfo.Model = MainWindow.CurrentModel.Name;
                            LotInfo.LotNo = currOrder;
                            LotInfo.ITS_LotNo = curLotData.ITS_ORDER;
                            LotInfo.RegDate = DateTime.Now;
                            LotInfo.ManagementCode = ITS_info_control.Management_Code;

                            //결과 경로를 네트워크 경로로 입력 할 것
                            // Map 저장 경로와 결과 저장 경로  
                            LotInfo.MC_Name = MainWindow.Setting.General.MachineName;
                            System.Windows.Point[] p = new System.Windows.Point[nTotalScanCount];
                            System.Windows.Point[] p1 = new System.Windows.Point[nTotalScanCount];
                            for (int i = 0; i < p.Length; i++)
                            {
                                p[i] = new System.Windows.Point(MainWindow.SectionPosition[i].DefectSectionPosition.X, MainWindow.SectionPosition[i].DefectSectionPosition.Y);
                                p1[i] = new System.Windows.Point(MainWindow.SectionPosition[i].IDSectionPosition.X, MainWindow.SectionPosition[i].IDSectionPosition.Y);
                            }
                            PCS.VRS.VRS_Model_Info ModelInfo = new PCS.VRS.VRS_Model_Info(MainWindow.CurrentModel, p, p1);

                            string TableName = MainWindow.Setting.General.VRSAVITableName;
                            string ScanCountInfo = MainWindow.Setting.Job.BPCount + "," + MainWindow.Setting.Job.CACount + "," + MainWindow.Setting.Job.BACount;
                            if (!PCS.VRS.InfoControl.CreateLotInfo(LotInfo, ModelInfo, TableName, ScanCountInfo))//false 일때 검사 종료
                                return;

                            string mapPath = @"e:\VRSBinCodeMap" + string.Format("\\{0}", MainWindow.CurrentGroup.Name);
                            if (!Directory.Exists(mapPath)) Directory.CreateDirectory(mapPath);
                            mapPath += "\\" + MainWindow.CurrentModel.Name;
                            if (!Directory.Exists(mapPath)) Directory.CreateDirectory(mapPath);
                            mapPath += "\\" + currOrder;
                            if (!Directory.Exists(mapPath)) Directory.CreateDirectory(mapPath);
                            #endregion
                        }

                        //압축된 섹션 이미지를 ResultPath로 Copy
                        CopyResizeSectionImage();

                        //검사 이력정보 서버 등록
                        try
                        {
                            JobHistoryInfo job = new JobHistoryInfo();
                            job.strProdOrder = curLotData.WONO;
                            job.strLocalOrder = InspectionMonitoringCtrl.TableDataCtrl.Re_LotNo;
                            job.strItsOrder = curLotData.ITS_ORDER;
                            job.strStdModel = curLotData.ITEM_CD;
                            job.strOpcode = curLotData.OP_CD;
                            job.strMachine = MainWindow.Setting.General.MachineName;
                            job.strLocalModel = MainWindow.CurrentModel.Name;
                            job.strLocalGroup = MainWindow.CurrentGroup.Name;

                            curJob = job;
                            InfoControl.UpdateJobInfoStart(job);
                        }
                        catch (Exception ex)
                        {
                            Log("PCS", SeverityLevel.DEBUG, string.Format("UpdateJobInfoStart Fail, Error: {0}", ex.Message), true);
                        }                        
                    }
                    break;
                case MenuType.STOPINSPECT:
                    Log("PCS", SeverityLevel.DEBUG, "자동검사가 중지되었습니다.");

                    if (DeviceController.LightDevice != null)
                    {
                        DeviceController.LightDevice.LightOff();
                    }

                    InspectionMonitoringCtrl.StopInspect();
                    m_bInspectionStarted = false;
                    InspectionMonitoringCtrl.SetPOPButton(true);
                    Border smokeBorder = new Border { Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(196, 196, 196)), Opacity = 0.5 };
                    Grid.SetRowSpan(smokeBorder, 5);
                    Grid.SetColumnSpan(smokeBorder, 10);
                    this.pnlMain.Children.Add(smokeBorder);
                    {
                        BusyWindow busy = new BusyWindow();
                        busy.ShowDialog();
                    }
                    this.pnlMain.Children.Remove(smokeBorder);
                    break;
                case MenuType.INSPECTIONRESULT:
                    Log("PCS", SeverityLevel.DEBUG, "검사결과 메뉴가 선택되었습니다.");
                    InspectResultWindow ResultDialog = new InspectResultWindow(InspectionMonitoringCtrl.ResultTable.TableData, MainWindow.Setting.General.ResultPath) { Owner = this };
                    ResultDialog.SetMainwindow(this);
                    ResultDialog.ShowDialog();
                    break;
                case MenuType.LASERMARKING:
                    try
                    {
                        if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null) { MessageBox.Show("모델을 선택하지 않았습니다."); return; }
                        InspectionMonitoringCtrl.LaserDlg.InitModel(false, InspectionMonitoringCtrl.imgAlign1, InspectionMonitoringCtrl.imgAlign2);
                        InspectionMonitoringCtrl.LaserDlg.InitModelData();
                        InspectionMonitoringCtrl.LaserDlg.ShowDialog();
                    }
                    catch
                    {
                        MessageBox.Show("MRK 파일 이상입니다. 마크 파일을 재생성 해 주세요.");
                        if (InspectionMonitoringCtrl.LaserDlg != null)
                        {
                            InspectionMonitoringCtrl.LaserDlg.UnInit();
                            InspectionMonitoringCtrl.LaserDlg = null;
                        }
                        InspectionMonitoringCtrl.LaserDlg = new LaserWindow(this);
                    }
                    break;
                case MenuType.SETTINGS:

                    if (UserManager.CurrentUser.Authority.AuthCode == "0061" ||
                        UserManager.CurrentUser.Authority.AuthCode == "0062")
                    {
                        Log("PCS", SeverityLevel.DEBUG, "환경 설정 메뉴가 선택되었습니다.");
                        SettingWindow SettingDialog = new SettingWindow(this) { Owner = this };
                        SettingDialog.ShowDialog();
                        m_TeachingDlg.SetCameraResolution();
                    }
                    else MessageBox.Show("관리자 이상 권한에서 접근할 수 있습니다.", "Information");

                    break;
                case MenuType.LOGSETTING:
                    if (UserManager.CurrentUser.Authority.AuthCode == "0061" ||
                        UserManager.CurrentUser.Authority.AuthCode == "0062")
                    {
                        Log("PCS", SeverityLevel.DEBUG, "로그 설정 메뉴가 선택되었습니다.");
                        if (m_LogSettingDlg == null)
                            m_LogSettingDlg = new LogSettingWindow(MainWindow.Setting.General);

                        try
                        {
                            m_LogSettingDlg.ShowDialog();
                            try
                            {
                                MainWindow.Setting.General.Save();
                            }
                            catch
                            {
                                Log("PCS", SeverityLevel.WARN, "변경된 로그 설정을 반영하는데 실패하였습니다.");
                            }
                            CleanLog();
                            Log("PCS", SeverityLevel.INFO, "로그 설정이 변경되었습니다.");
                        }
                        catch
                        {
                            Log("PCS", SeverityLevel.FATAL, "로그 설정 창이 예기치 못한 오류로 인해 강제 종료되었습니다.", true);
                        }
                    }
                    else MessageBox.Show("관리자 이상 권한에서 접근할 수 있습니다.", "Information");
                    break;
                case MenuType.CHANGEUSER:
                    Log("PCS", SeverityLevel.DEBUG, "작업자 변경 메뉴가 선택되었습니다.");
                    smokeBorder = new Border { Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(196, 196, 196)), Opacity = 0.5 };
                    Grid.SetRowSpan(smokeBorder, 5);
                    Grid.SetColumnSpan(smokeBorder, 10);
                    this.pnlMain.Children.Add(smokeBorder);
                    {
                        ChangeUserWindow changeUserWindow = new ChangeUserWindow();
                        if ((bool)changeUserWindow.ShowDialog())
                        {
                            Log("PCS", SeverityLevel.DEBUG, string.Format("작업자가 변경되었습니다. ID:{0}", UserManager.CurrentUser.Name));
                        }
                    }
                    this.pnlMain.Children.Remove(smokeBorder);
                    break;
                case MenuType.EXIT:
                    if (!m_bInspectionStarted)
                        ExitApplication();
                    break;
            }
        }

        private void SendLogMessageEvent(string aszSubsystem, SeverityLevel aSeverityLevel, string aszLogMessage)
        {
            Log(aszSubsystem, aSeverityLevel, aszLogMessage);
        }

        private void InspectErrorEvent(ErrorType aErrType)
        {
            switch (aErrType)
            {
                case ErrorType.TrainError:
                    Log("PCS", SeverityLevel.DEBUG, "Train Error, 자동검사가 중지되었습니다.");

                    if (DeviceController.LightDevice != null)
                        DeviceController.LightDevice.LightOff();

                    m_bInspectionStarted = false;
                    InspectionMonitoringCtrl.SetPOPButton(true);
                    InspectionMonitoringCtrl.StopInspect();
                    MessageBox.Show("Error에 의해 자동검사가 종료 되었습니다 : " + aErrType.ToString());
                    break;
                case ErrorType.VerifyConnectError:
                    MainWindow.Log("PCS", SeverityLevel.DEBUG, "Verify PC와 연결이 끊어졌습니다. (Verify 요청)");

                    if (DeviceController.LightDevice != null)
                        DeviceController.LightDevice.LightOff();

                    m_bInspectionStarted = false;
                    InspectionMonitoringCtrl.SetPOPButton(true);
                    InspectionMonitoringCtrl.StopInspect();
                    MessageBox.Show("Verify PC와 연결이 끊어졌습니다.");
                    break;
                default:
                    break;
            }
        }

        // 비전 접속
        private void SetFrame()
        {
            string szConnectionResult = "Vision 연결 상태를 확인 바랍니다.\n\n";
            bool bVisionConnected = true;
            int cntRE = 0;
            string sType;
            List<string> ConnectionResultList = new List<string>();
            for (int i = 0; i < PCSInstance.Vision.Length; i++)
            {
                if (i < Setting.Job.BPCount)
                    sType = "본드 패드" + (i + 1);
                else if (i >= Setting.Job.BPCount && i < Setting.Job.BPCount + Setting.Job.CACount)
                    sType = "CA 외관" + (i - Setting.Job.BPCount + 1);
                else
                    sType = "BA 외관" + (i - (Setting.Job.BPCount + Setting.Job.CACount) + 1);

                sType += " : 미확인\n";
                ConnectionResultList.Add(sType);
            }
            if (!MainWindow.Setting.SubSystem.PLC.UsePLC)
            {
                for (int nIndex = 0; nIndex < PCSInstance.Vision.Length; nIndex++)
                {
                    bVisionConnected = false;
                    if (nIndex == MainWindow.Setting.SubSystem.IS.TestID)
                    {
                        bVisionConnected = PCSInstance.ConnectVision(nIndex, MainWindow.Setting.SubSystem.IS.IP[nIndex], 15000);

                        if (!bVisionConnected)
                        {
                            szConnectionResult += "vision" +(nIndex + 1) + ": 연결 실패\n";
                            MessageBox.Show(szConnectionResult, "Connection Error");
                            return;
                        }
                        InitVision(nIndex);
                    }
                }
            }
            else
            {
                for (int nIndex = 0; nIndex < PCSInstance.Vision.Length; nIndex++)//////PC Count...5 
                {
                    int FrameGrabberIndex = VID.CalcIndex(nIndex);
                    if (!PCSInstance.Vision[nIndex].Connected)
                    {
                        if (nIndex >= VID.CA1 && nIndex < VID.BA1)
                        {
                            if (!MainWindow.Setting.SubSystem.IS.UseCASlave && (nIndex - (int)VID.CA1) % 2 == 1) continue;////짝수번째(Slave) Connect 확인안함
                        }
                        else if (nIndex >= VID.BA1)
                        {
                            if (!MainWindow.Setting.SubSystem.IS.UseBASlave && (nIndex - (int)VID.BA1) % 2 == 1) continue;////짝수번째(Slave) Connect 확인안함
                        }

                        bVisionConnected = false;

                        while (!bVisionConnected)
                        {
                            bVisionConnected = PCSInstance.ConnectVision(nIndex, MainWindow.Setting.SubSystem.IS.IP[FrameGrabberIndex], MainWindow.Setting.SubSystem.IS.Port[FrameGrabberIndex]);

                            if (!bVisionConnected)
                            {
                                RemoteVisionProcessKillSingle(nIndex);
                                RemoteVisionProcessStartSingle(nIndex);
                            }

                            cntRE++;
                            if (cntRE > 1)
                            {
                                break;
                            }
                        }
                        cntRE = 0;
                    
                        if (bVisionConnected)
                        {
                            string Modify = "연결 성공";
                            sType = "미확인";
                            ConnectionResultList[nIndex] = ConnectionResultList[nIndex].Replace(sType, Modify);
                        }
                        else
                        {
                            string Modify = "연결 실패";
                            sType = "미확인";
                            ConnectionResultList[nIndex] = ConnectionResultList[nIndex].Replace(sType, Modify);

                            for (int i = 0; i < ConnectionResultList.Count; i++)
                            {
                                szConnectionResult += ConnectionResultList[i];
                            }
                            MessageBox.Show(szConnectionResult, "Connection Error");
                            return;
                        }
                    }
                    else
                    {
                        if (!MainWindow.Setting.SubSystem.PLC.UsePLC)
                        {
                            PCSInstance.Vision[nIndex].DisConnect();
                            if (nIndex == MainWindow.Setting.SubSystem.IS.TestID)
                                bVisionConnected = PCSInstance.ConnectVision(nIndex, "127.0.0.1", 15000 + 0);//nIndex);
                            if (bVisionConnected)
                            {
                                string Modify = "연결 성공";
                                sType = "미확인";
                                ConnectionResultList[nIndex] = ConnectionResultList[nIndex].Replace(sType, Modify);
                            }
                            else
                            {
                                string Modify = "연결 실패";
                                sType = "미확인";
                                ConnectionResultList[nIndex] = ConnectionResultList[nIndex].Replace(sType, Modify);

                                for (int i = 0; i < ConnectionResultList.Count; i++)
                                {
                                    szConnectionResult += ConnectionResultList[i];
                                }
                                break;
                            }

                            if (!bVisionConnected)
                            {
                                MessageBox.Show(szConnectionResult, "Connection Error");
                                return;
                            }
                        }
                    }
                }
                InitVision();
            }      
        }

        // 비전 초기화
        public void InitVision(int anDesVision = -1)
        {
            int nGrabCount = m_nTotalScanCount;
            int nCropHeight = 500;
            if (CurrentModel != null)
            {
                int nUnits = CurrentModel.Strip.UnitColumn * CurrentModel.Strip.UnitRow;
                nGrabCount = (nUnits / nGrabCount <= 2) ? 1 : (nUnits / nGrabCount <= 4) ? 2 : 5;
            }

            int height = 0;
            //if (!MainWindow.Setting.SubSystem.PLC.UsePLC)
            //{
            // Height에 4mm를 더하여 연산하도록 함.

            for (int i = 0; i < m_nTotalScanCount; i++)
            {
                IndexInfo IndexInfo = Convert_CheckVisionSlave(i);
                if (IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave) continue;
                int FrameGrabberIndex = VID.CalcIndex(IndexInfo.VisionIndex);
                if ((!MainWindow.Setting.SubSystem.PLC.UsePLC && anDesVision == IndexInfo.VisionIndex) || anDesVision == -1)
                {
                    int nCID = 0;
                    int nDeviceType = 0;
                    double dMaster = 1.0;

                    string testpath = "";

                    if (IndexInfo.CategorySurface == CategorySurface.BP)
                    {
                        nCID = CID.BP; nDeviceType = 0;
                        testpath = "//BP";
                    }

                    else if (IndexInfo.CategorySurface == CategorySurface.CA)
                    {
                        nCID = CID.CA; nDeviceType = 1;
                        testpath = "//CA";
                    }
                    else
                    {
                        nCID = CID.BA; nDeviceType = 0;
                        testpath = "//BA";
                    }
                    ;
                
                    if (IndexInfo.CategorySurface == CategorySurface.BP )    ////BP에서만 PSRShift 설정해준다.
                    {
                        dMaster = MainWindow.Setting.SubSystem.IS.ReScale;
                        PCSInstance.Vision[IndexInfo.VisionIndex].SetInspectMode(MainWindow.Setting.Job.PSRShiftType, 2);
                        PCSInstance.Vision[IndexInfo.VisionIndex].SetGain(MainWindow.Setting.SubSystem.IS.R_Gain[nCID], MainWindow.Setting.SubSystem.IS.G_Gain[nCID], MainWindow.Setting.SubSystem.IS.B_Gain[nCID], MainWindow.Setting.SubSystem.IS.Strenth[nCID], false);
                    }
                    else
                    {
                        if (IndexInfo.Slave)//// CA, BA Slave 스캔일때
                        {
                            dMaster = 0.0;
                            if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] == GrabberType.CXP) dMaster = 1.0;
                        }
                        else//// CA, BA Master
                        {
                            dMaster = 1.0;
                            PCSInstance.Vision[IndexInfo.VisionIndex].SetGain(MainWindow.Setting.SubSystem.IS.R_Gain[nCID], MainWindow.Setting.SubSystem.IS.G_Gain[nCID], MainWindow.Setting.SubSystem.IS.B_Gain[nCID], MainWindow.Setting.SubSystem.IS.Strenth[nCID], true);
                        }
                    }

                    if (IndexInfo.SurfaceScanCount != 0) continue;
                    height = Convert.ToInt32((CurrentModel.Strip.Width + 4) * 1000 / MainWindow.Setting.SubSystem.IS.CamResolutionY[nCID]);
                    PCSInstance.Vision[IndexInfo.VisionIndex].InitVision(MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex], MainWindow.Setting.SubSystem.IS.CameraWidth[nCID], height, 8, nGrabCount, nDeviceType,////마지막 nDeviceType 파라메터는 PSR하지이물 검사만  CA, BA 구분용으로 사용
                                                           MainWindow.Setting.SubSystem.IS.DeviceName[FrameGrabberIndex], MainWindow.Setting.SubSystem.IS.CamFile[FrameGrabberIndex], nCropHeight, MainWindow.Setting.SubSystem.IS.CamPageDelay[nCID],
                                                           MainWindow.Setting.SubSystem.IS.CamResolutionX[nCID], dMaster, m_CurrentRGBIndex, MainWindow.Setting.General.MaxLimitDefect);

                    // 박경수 - 로컬 모드 코드 제작 중
                    //if (IndexInfo.SurfaceScanCount != 0) continue;
                    //height = Convert.ToInt32((CurrentModel.Strip.Width + 4) * 1000 / MainWindow.Setting.SubSystem.IS.CamResolutionY[nCID]);


                    //if (!Setting.SubSystem.PLC.UsePLC) // 수동모드
                    //{
                    //    PCSInstance.Vision[IndexInfo.VisionIndex].InitVision(MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex], MainWindow.Setting.SubSystem.IS.CameraWidth[nCID], height, 8, nGrabCount, nDeviceType,////마지막 nDeviceType 파라메터는 PSR하지이물 검사만  CA, BA 구분용으로 사용
                    //                   MainWindow.Setting.SubSystem.IS.DeviceName[FrameGrabberIndex], MainWindow.Setting.SubSystem.IS.CamFile[FrameGrabberIndex] + "//" + CurrentModel.Name + testpath + "//GrabImage_", nCropHeight, MainWindow.Setting.SubSystem.IS.CamPageDelay[nCID],
                    //                   MainWindow.Setting.SubSystem.IS.CamResolutionX[nCID], dMaster, m_CurrentRGBIndex, MainWindow.Setting.General.MaxLimitDefect);
                    //}
                    //else // 자동모드
                    //{IndexInfo
                    //    PCSInstance.Vision[.VisionIndex].InitVision(MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex], MainWindow.Setting.SubSystem.IS.CameraWidth[nCID], height, 8, nGrabCount, nDeviceType,////마지막 nDeviceType 파라메터는 PSR하지이물 검사만  CA, BA 구분용으로 사용
                    //                                           MainWindow.Setting.SubSystem.IS.DeviceName[FrameGrabberIndex], MainWindow.Setting.SubSystem.IS.CamFile[FrameGrabberIndex], nCropHeight, MainWindow.Setting.SubSystem.IS.CamPageDelay[nCID],
                    //                                           MainWindow.Setting.SubSystem.IS.CamResolutionX[nCID], dMaster, m_CurrentRGBIndex, MainWindow.Setting.General.MaxLimitDefect);
                    //}
                }
            }
            //}
            //else
            //{
            //    // Height에 4mm를 더하여 연산하도록 함.
            //    height = 0;
            //    if (anDesVision == -1 || anDesVision == VID.BA1)
            //    {
            //        PCSInstance.Vision[VID.BA1].SetGain(MainWindow.Setting.SubSystem.IS.R_Gain[CID.BA], MainWindow.Setting.SubSystem.IS.G_Gain[CID.BA], MainWindow.Setting.SubSystem.IS.B_Gain[CID.BA], MainWindow.Setting.SubSystem.IS.Strenth[CID.BA], true);
            //        height = Convert.ToInt32((CurrentModel.Strip.Width + 4) * 1000 / MainWindow.Setting.SubSystem.IS.CamResolutionY[CID.BA]);
            //        PCSInstance.Vision[VID.BA1].InitVision(MainWindow.Setting.SubSystem.IS.FGType[VID.BA1], MainWindow.Setting.SubSystem.IS.CameraWidth[CID.BA], height, 8, nGrabCount, 0,
            //                                               MainWindow.Setting.SubSystem.IS.DeviceName[VID.BA1], MainWindow.Setting.SubSystem.IS.CamFile[VID.BA1], nCropHeight, MainWindow.Setting.SubSystem.IS.CamPageDelay[CID.BA],
            //                                               MainWindow.Setting.SubSystem.IS.CamResolutionX[CID.BA], 1.0, m_CurrentRGBIndex, MainWindow.Setting.General.MaxLimitDefect);
            //
            //    }
            //    if (anDesVision == -1 || anDesVision == VID.BA1+1)
            //    {
            //        height = Convert.ToInt32((CurrentModel.Strip.Width + 4) * 1000 / MainWindow.Setting.SubSystem.IS.CamResolutionY[CID.BA]);
            //        double dMaster = 0.0;
            //        if (MainWindow.Setting.SubSystem.IS.FGType[CID.BA] == GrabberType.CXP) dMaster = 1.0;
            //        PCSInstance.Vision[VID.BA1 + 1].InitVision(MainWindow.Setting.SubSystem.IS.FGType[VID.BA1 + 1], MainWindow.Setting.SubSystem.IS.CameraWidth[CID.BA], height, 8, nGrabCount, 0,
            //                                               MainWindow.Setting.SubSystem.IS.DeviceName[VID.BA1 + 1], MainWindow.Setting.SubSystem.IS.CamFile[VID.BA1 + 1], nCropHeight, MainWindow.Setting.SubSystem.IS.CamPageDelay[CID.BA],
            //                                               MainWindow.Setting.SubSystem.IS.CamResolutionX[CID.BA], dMaster, m_CurrentRGBIndex, MainWindow.Setting.General.MaxLimitDefect);
            //    }
            //    if (anDesVision == -1 || anDesVision == VID.BP1)
            //    {
            //        PCSInstance.Vision[VID.BP1].SetInspectMode(MainWindow.Setting.Job.PSRShiftType, 2);
            //        PCSInstance.Vision[VID.BP1].SetGain(MainWindow.Setting.SubSystem.IS.R_Gain[CID.BP], MainWindow.Setting.SubSystem.IS.G_Gain[CID.BP], MainWindow.Setting.SubSystem.IS.B_Gain[CID.BP], MainWindow.Setting.SubSystem.IS.Strenth[CID.BP], false);
            //        height = Convert.ToInt32((CurrentModel.Strip.Width + 4) * 1000 / MainWindow.Setting.SubSystem.IS.CamResolutionY[CID.BP]);
            //        PCSInstance.Vision[VID.BP1].InitVision(MainWindow.Setting.SubSystem.IS.FGType[VID.BP1], MainWindow.Setting.SubSystem.IS.CameraWidth[CID.BP], height, 8, nGrabCount, 0,
            //                                               MainWindow.Setting.SubSystem.IS.DeviceName[VID.BP1], MainWindow.Setting.SubSystem.IS.CamFile[VID.BP1], nCropHeight, MainWindow.Setting.SubSystem.IS.CamPageDelay[CID.BP],
            //                                               MainWindow.Setting.SubSystem.IS.CamResolutionX[CID.BP], MainWindow.Setting.SubSystem.IS.ReScale, m_CurrentRGBIndex, MainWindow.Setting.General.MaxLimitDefect);
            //    }
            //    if (anDesVision == -1 || anDesVision == VID.CA1)
            //    {
            //        PCSInstance.Vision[VID.CA1].SetGain(MainWindow.Setting.SubSystem.IS.R_Gain[CID.CA], MainWindow.Setting.SubSystem.IS.G_Gain[CID.CA], MainWindow.Setting.SubSystem.IS.B_Gain[CID.CA], MainWindow.Setting.SubSystem.IS.Strenth[CID.CA], true);
            //        height = Convert.ToInt32((CurrentModel.Strip.Width + 4) * 1000 / MainWindow.Setting.SubSystem.IS.CamResolutionY[CID.CA]);
            //        PCSInstance.Vision[VID.CA1].InitVision(MainWindow.Setting.SubSystem.IS.FGType[VID.CA1], MainWindow.Setting.SubSystem.IS.CameraWidth[CID.CA], height, 8, nGrabCount, 1,////마지막 1 파라메터는 PSR하지이물 검사만  CA, BA 구분용으로 사용
            //                                               MainWindow.Setting.SubSystem.IS.DeviceName[VID.CA1], MainWindow.Setting.SubSystem.IS.CamFile[VID.CA1], nCropHeight, MainWindow.Setting.SubSystem.IS.CamPageDelay[CID.CA],
            //                                               MainWindow.Setting.SubSystem.IS.CamResolutionX[CID.CA], 1.0, m_CurrentRGBIndex, MainWindow.Setting.General.MaxLimitDefect);
            //    }
            //    if (anDesVision == -1 || anDesVision == VID.CA1+1)
            //    {
            //        double dMaster = 0.0;
            //        if (MainWindow.Setting.SubSystem.IS.FGType[CID.CA] == GrabberType.CXP) dMaster = 1.0;
            //        height = Convert.ToInt32((CurrentModel.Strip.Width + 4) * 1000 / MainWindow.Setting.SubSystem.IS.CamResolutionY[CID.CA]);
            //        PCSInstance.Vision[VID.CA1 + 1].InitVision(MainWindow.Setting.SubSystem.IS.FGType[VID.CA1 + 1], MainWindow.Setting.SubSystem.IS.CameraWidth[CID.CA], height, 8, nGrabCount, 1,////마지막 1 파라메터는 PSR하지이물 검사만  CA, BA 구분용으로 사용
            //                                               MainWindow.Setting.SubSystem.IS.DeviceName[VID.CA1 + 1], MainWindow.Setting.SubSystem.IS.CamFile[VID.CA1 + 1], nCropHeight, MainWindow.Setting.SubSystem.IS.CamPageDelay[CID.CA],
            //                                               MainWindow.Setting.SubSystem.IS.CamResolutionX[CID.CA], dMaster, m_CurrentRGBIndex, MainWindow.Setting.General.MaxLimitDefect);
            //    }
            //}
        }

        #region handle log.
        public static void Log(string strSubSystem, SeverityLevel enumSeverityLevel, string strLogMessage, bool IsDirectLog = false)
        {
            if (MainWindow.Setting.General.LogSave && (Convert.ToInt32(enumSeverityLevel) >= MainWindow.Setting.General.LogLevel))
            {
                if (MainLogger != null)
                    MainLogger.Log(strSubSystem, enumSeverityLevel, strLogMessage, IsDirectLog);
            }
        }

        public static void CleanLog()
        {
            int nDeleteLogCount = Logger.CleanLog(MainWindow.Setting.General.LogKeepDate);
            if (nDeleteLogCount > 0)
            {
                MessageBox.Show(String.Format("최근에 기록된 {0}개의 로그 파일을 정리하였습니다.", nDeleteLogCount), "Information");
            }
        }
        #endregion

        public void SetCurrentInspectInfo(string LotNo, string Operator, string reInspect, string procCode)
        {
            InspectionMonitoringCtrl.TableDataCtrl.Operator = Operator;
            InspectionMonitoringCtrl.TableDataCtrl.LotNo = LotNo;
            InspectionMonitoringCtrl.TableDataCtrl.ProcCode = procCode;
            if (reInspect != "")
                InspectionMonitoringCtrl.TableDataCtrl.Re_LotNo = LotNo + reInspect;
            else
                InspectionMonitoringCtrl.TableDataCtrl.Re_LotNo = LotNo;

            InspectionMonitoringCtrl.TableDataCtrl.ModelName = CurrentModel.Name;
            InspectionMonitoringCtrl.TableDataCtrl.GroupName = CurrentGroup.Name;
            string strPath = MainWindow.Setting.General.ResultPath;
            string path = String.Format(@"{0}/{1}/{2}/{3}/", strPath, CurrentGroup.Name, CurrentModel.Name, InspectionMonitoringCtrl.TableDataCtrl.Re_LotNo);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (!Directory.Exists(path + "Image/")) Directory.CreateDirectory(path + "Image/");
            InspectionMonitoringCtrl.TableDataCtrl.ResultFile = String.Format(@"{0}/{1}/{2}/{3}/Result.ini", strPath, CurrentGroup.Name, CurrentModel.Name, InspectionMonitoringCtrl.TableDataCtrl.Re_LotNo);
            InspectionMonitoringCtrl.TableDataCtrl.MapFile = String.Format(@"{0}/{1}/{2}/{3}/Map.txt", strPath, CurrentGroup.Name, CurrentModel.Name, InspectionMonitoringCtrl.TableDataCtrl.Re_LotNo);
            InspectionMonitoringCtrl.TableDataCtrl.FMapFile = String.Format(@"{0}/{1}/{2}/{3}/FMap.txt", strPath, CurrentGroup.Name, CurrentModel.Name, InspectionMonitoringCtrl.TableDataCtrl.Re_LotNo);
            InspectionMonitoringCtrl.TableDataCtrl.DMapFile = String.Format(@"{0}/{1}/{2}/{3}/DMap.txt", strPath, CurrentGroup.Name, CurrentModel.Name, InspectionMonitoringCtrl.TableDataCtrl.Re_LotNo);
            InspectionMonitoringCtrl.TableDataCtrl.ImagePath = String.Format(@"{0}/{1}/{2}/{3}/Image/", strPath, CurrentGroup.Name, CurrentModel.Name, InspectionMonitoringCtrl.TableDataCtrl.Re_LotNo);
            InspectionMonitoringCtrl.TableDataCtrl.DataMatrixPath = String.Format(@"{0}/{1}/{2}/{3}/2D Mark log/", strPath, CurrentGroup.Name, CurrentModel.Name, InspectionMonitoringCtrl.TableDataCtrl.Re_LotNo);
            InspectionMonitoringCtrl.ResultTable.txtModel.Text = CurrentModel.Name;
            InspectionMonitoringCtrl.ResultTable.txtLot.Text = LotNo;
        }
        // 모델 변경
        private void ChangeModel()
        {
            Log("PCS", SeverityLevel.DEBUG, "모델 관리 메뉴가 선택되었습니다.");
            ModelManager modelCreatorDlg = new ModelManager(MainWindow.Setting.SubSystem.Laser.UseLaser) { Owner = this };
            if ((bool)modelCreatorDlg.ShowDialog())
            {
                if (modelCreatorDlg.SelectedModel != null)
                {
                    Log("PCS", SeverityLevel.DEBUG, String.Format("모델명 {0}이(가) 선택되었습니다.", modelCreatorDlg.SelectedModel.Name));
                    InspectionMonitoringCtrl.ResultTable.txtModel.Text = modelCreatorDlg.SelectedModel.Name;
                    InspectionMonitoringCtrl.m_UseConBad = (bool)modelCreatorDlg.cbConBad.IsChecked;
                    m_bTeachingDataLoaded = false;
                    SectionSettingStorage.OriginSetting.ClearSetting(); // 섹션 설정 저장소 초기화.
                    InspectionMonitoringCtrl.InitDisplay();
                    SetFrame();

                    for(int i = 0; i < m_TeachingDlg.TeachingViewer.CamView.Count; i++)
                    {
                        m_TeachingDlg.TeachingViewer.CamView[i].GrabImage = null;
                        m_TeachingDlg.TeachingViewer.CamView[i].IsSentDone = false;
                        m_TeachingDlg.TeachingViewer.CamView[i].IsGrabView = false;
                        m_TeachingDlg.TeachingViewer.CamView[i].IsGrabDone = false;
                    }

                    List<bool> ListUseBondPad = new List<bool>();
                    List<bool> ListUseTopSur = new List<bool>();
                    List<bool> ListUseBotSur = new List<bool>();
                    ListUseBondPad.Add(CurrentModel.UseBondPad1);
                    ListUseBondPad.Add(CurrentModel.UseBondPad2);
                    ListUseTopSur.Add(CurrentModel.UseTopSur1);
                    ListUseTopSur.Add(CurrentModel.UseTopSur2);
                    ListUseTopSur.Add(CurrentModel.UseTopSur3);
                    ListUseTopSur.Add(CurrentModel.UseTopSur4);
                    ListUseTopSur.Add(CurrentModel.UseTopSur5);
                    ListUseBotSur.Add(CurrentModel.UseBotSur1);
                    ListUseBotSur.Add(CurrentModel.UseBotSur2);
                    ListUseBotSur.Add(CurrentModel.UseBotSur3);
                    ListUseBotSur.Add(CurrentModel.UseBotSur4);
                    ListUseBotSur.Add(CurrentModel.UseBotSur5);

                    for (int i = 0; i < Setting.Job.BPCount; i++) { MainWindow.CurrentModel.ListALLUseSur.Add(ListUseBondPad[i]); if (ListUseTopSur[i]) m_nBP_EnableCnt++; }
                    for (int i = 0; i < Setting.Job.CACount; i++) { MainWindow.CurrentModel.ListALLUseSur.Add(ListUseTopSur[i]); if (ListUseTopSur[i]) m_nCA_EnableCnt++; }
                    for (int i = 0; i < Setting.Job.BACount; i++) { MainWindow.CurrentModel.ListALLUseSur.Add(ListUseBotSur[i]); if (ListUseBotSur[i]) m_nBA_EnableCnt++; }
                    for (int i = 0; i < m_nTotalScanCount; i++) 
                        InspectionMonitoringCtrl.SetSkipDisplay(i, !MainWindow.CurrentModel.ListALLUseSur[i]);
                }
            }
        }

        // 모델정보전송
        public void SendModelInfo(double Height, double Width, int ScanVelocity1, int ScanVelocity2, double Cam1Pos, double CamPitch, int LaserStep, double LaserPitch, double AlignPosX1, double AlignPosX2, double AlignPosY, double LaserCorr, double Thickness, int CACount, int BACount, int LeftID, int mcType, MarkingBoatShift MarkingShift)
        {
            PCSInstance.PlcDevice.SendModelData(Height, Width, ScanVelocity1, ScanVelocity2, Cam1Pos, CamPitch, LaserStep, LaserPitch, AlignPosX1, AlignPosX2, AlignPosY, LaserCorr, Thickness, CACount, BACount, LeftID, mcType, MarkingShift);
        }

        public void SendPitch(double cam1pitch)
        {
            if (PCSInstance.PlcDevice != null)
                PCSInstance.PlcDevice.SendPitchData(cam1pitch);
        }

        public void CheckMainUpdate()
        {
            try
            {
                string tempDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\", "temp_update"));

                if (Directory.Exists(tempDir))
                {
                    string updaterPath = Path.Combine(tempDir, "updater.exe");

                    if (File.Exists(updaterPath))
                    {
                        // updater.exe를 실행
                        Process.Start(updaterPath);
                        Log("Patch", SeverityLevel.INFO, "Patch P/G run");
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Patch", SeverityLevel.INFO, "Patch Error");
            }
        }


        #region Exit Application.
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (m_bInspectionStarted)
            {
                e.Cancel = true;
            }
            else
            {
                if (!ExitApplication())
                    e.Cancel = true;
            }
        }

        public bool ExitApplication()
        {
            if (m_TeachingDlg != null && !m_TeachingDlg.IsOnLine)
            {
                Setting.Save();

                Log("PCS", SeverityLevel.INFO, "프로그램을 종료합니다.");
                MainLogger.Close();
                MainLogger = null;
                Environment.Exit(0);

                return true;
            }
            else
            {
                if (MessageBox.Show("프로그램을 종료하시겠습니까?", "종료", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (PCSInstance != null)
                    {
                        PCSInstance.ReleaseDevice(); // HW 연결 해제
                        for (int nIndex = 0; nIndex < PCSInstance.Vision.Length; nIndex++)
                        {
                            // 수신 스레드가 종료되도록 한다.
                            if (PCSInstance.Vision[nIndex] != null && PCSInstance.Vision[nIndex].SyncSocket != null)
                                PCSInstance.Vision[nIndex].SyncSocket.RecvThread = false;
                        }
                    }

                    RemoteVisionProcessKill();

                    if (InspectionMonitoringCtrl.LaserDlg != null)
                    {
                        if (MainWindow.Setting.SubSystem.Laser.CamType == 1)
                            InspectionMonitoringCtrl.LaserDlg.close_vision();
                    }

                    Setting.Save();
                    if (InspectionMonitoringCtrl.LaserDlg != null)
                    {
                        InspectionMonitoringCtrl.LaserDlg.UnInit();
                    }
                    Log("PCS", SeverityLevel.INFO, "프로그램을 종료합니다.");
                    MainLogger.Close();
                    MainLogger = null; // MainLogger가 null 인지 체크하여 로그인 윈도우에서 종료할지 결정함

                    #region AutoPatch-Updater 실행
                    CheckMainUpdate(); // 패치 있을 경우 진행
                    MainWindow.CommandExcuteISUpdate(); // IS업데이트 실행
                    #endregion

                    Environment.Exit(0);

                    return true;
                }
                return false;
            }
        }
        #endregion
    }

    // Command Executor.
    public partial class MainWindow : Window
    {
        #region AutoPatch
        public static async Task CommandExcuteISUpdate()
        {
            string checkPath = AppContext.BaseDirectory;
            checkPath = Path.GetFullPath(Path.Combine(checkPath, @"..\..\isupdate.flag"));

            List<string> targetIps = new List<string>();

            if (File.Exists(checkPath))
            {
                // IS 주소 목록                
                for (int i = 0; i < Setting.SubSystem.IS.IP.Length; i++)
                {
                    targetIps.Add(Setting.SubSystem.IS.IP[i]);
                }

                List<Task> patchTasks = new List<Task>();
                for (int i = 0; i < Setting.SubSystem.IS.IP.Length; i++)
                {
                    // await semaphore.WaitAsync(); // 동시 작업 수 제한 시 사용

                    patchTasks.Add(ExecuteRemoteUpdateTask(targetIps[i]));
                }

                await Task.WhenAll(patchTasks);
                File.Delete(checkPath);
            }
        }

        /// <summary>
        /// 지정된 IP에 대해 원격으로 업데이트 작업을 생성하고 즉시 실행
        /// </summary>
        /// <param name="ip">대상 IP 주소</param>
        public static async Task ExecuteRemoteUpdateTask(string ip)
        {
            try
            {
                using (Process process = new Process())
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "schtasks.exe",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    // 1. 작업 생성
                    startInfo.Arguments = $"/create /s {ip} /u bga /p vision /tn isupdater /tr \"D:\\vision\\temp_update\\Updater.exe\" /sc ONCE /sd 2000/01/01 /st 00:00 /f";
                    process.StartInfo = startInfo;
                    process.Start();
                    //process.WaitForExit(); // 작업 생성이 끝날 때까지 대기

                    // 2. 작업 실행
                    startInfo.Arguments = $"/run /tn isupdater /s {ip}";
                    process.StartInfo = startInfo;
                    process.Start();
                    //process.WaitForExit(); // 작업 실행이 끝날 때까지 대기

                    await WaitForProcessToExitAsync(ip, "Updater.exe", 120); // 수정된 메서드 호출

                }
            }
            catch (Exception ex)
            {

            }
        }

        async static Task WaitForProcessToExitAsync(string ip, string processName, int timeoutSeconds) //
        {
            int waitedSeconds = 0; //
                                   // IsProgramRunning은 WMI를 사용하므로 블로킹 호출일 수 있음. Task.Run으로 감싸는 것을 고려할 수 있으나,
                                   // 여기서는 빈번한 호출이 아니므로 일단 그대로 둠.
            while (IsProgramRunning(ip, processName)) //
            {
                if (waitedSeconds >= timeoutSeconds) //
                {
                    break;
                }
                await Task.Delay(100); // 비동기 지연
                waitedSeconds++; //
            }
        }

        static bool IsProgramRunning(string ip, string processName)
        {
            try
            {
                string remoteName = @"\\" + ip + @"\root\cimv2";

                ConnectionOptions con = new ConnectionOptions();

                con.Username = "bga";
                con.Password = "vision";

                ManagementScope managementScope = new ManagementScope(remoteName, con);
                managementScope.Options.Authentication = AuthenticationLevel.PacketPrivacy;
                managementScope.Connect();
                ObjectQuery objectQuery = new ObjectQuery($"SELECT * FROM Win32_Process Where Name = '{processName}'");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(managementScope, objectQuery);
                ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
                if (managementObjectCollection.Count > 0) return true;
                else return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Remote Control Vision(IS) 실행/종료
        // 프로세스의 실행 상태를 확인한다.
        public static void CheckRunningState(bool debug, int anDesVision = -1)
        {
            string szPCName = Setting.General.MachineName;

            ProcessStartInfo si = new ProcessStartInfo();
            si.FileName = "tasklist.exe";
            si.UseShellExecute = false;
            si.WindowStyle = ProcessWindowStyle.Hidden;
            si.CreateNoWindow = true;
            si.RedirectStandardInput = true;
            si.RedirectStandardOutput = true;
            Process run = new Process();

            bool bOK = false;
            int nTryCount = 3;
            if ((anDesVision == -1 || anDesVision == 0) && !debug) // Check Vision 1
            {
                si.Arguments = string.Format("/s {0}V1a /fi \"IMAGENAME eq IS.exe\"", szPCName);
                run.StartInfo = si;
                while (run.Start())
                {
                    Thread.Sleep(250);
                    using (StreamReader sr = run.StandardOutput)
                    {
                        string szLine = string.Empty;
                        while ((szLine = sr.ReadLine()) != null)
                        {
                            if (szLine.Contains("IS.exe"))
                            {
                                bOK = true;
                                break;
                            }
                        }
                    }
                    if (--nTryCount == 0 || bOK) break;
                    Thread.Sleep(1000);
                }
            }
            nTryCount = 3;
            if (anDesVision == -1 || anDesVision == 1) // Check Vision 2
            {
                si.Arguments = string.Format("/s {0}V1b /fi \"IMAGENAME eq IS.exe\"", szPCName);
                run.StartInfo = si;
                bOK = false;
                while (run.Start())
                {
                    Thread.Sleep(250);
                    using (StreamReader sr = run.StandardOutput)
                    {
                        string szLine = string.Empty;
                        while ((szLine = sr.ReadLine()) != null)
                        {
                            if (szLine.Contains("IS.exe"))
                            {
                                bOK = true;
                                break;
                            }
                        }
                    }
                    if (--nTryCount == 0 || bOK) break;
                    Thread.Sleep(1000);
                }
            }
            nTryCount = 3;
            if (anDesVision == -1 || anDesVision == 2) // Check Vision 3
            {
                si.Arguments = string.Format("/s {0}V2 /fi \"IMAGENAME eq IS.exe\"", szPCName);
                run.StartInfo = si;
                bOK = false;
                while (run.Start())
                {
                    Thread.Sleep(250);
                    using (StreamReader sr = run.StandardOutput)
                    {
                        string szLine = string.Empty;
                        while ((szLine = sr.ReadLine()) != null)
                        {
                            if (szLine.Contains("IS.exe"))
                            {
                                bOK = true;
                                break;
                            }
                        }
                    }
                    if (--nTryCount == 0 || bOK) break;
                    Thread.Sleep(1000);
                }
            }

            nTryCount = 3;
            if (anDesVision == -1 || anDesVision == 3) // Check Vision 4
            {
                si.Arguments = string.Format("/s {0}V3a /fi \"IMAGENAME eq IS.exe\"", szPCName);
                run.StartInfo = si;
                bOK = false;
                while (run.Start())
                {
                    Thread.Sleep(250);
                    using (StreamReader sr = run.StandardOutput)
                    {
                        string szLine = string.Empty;
                        while ((szLine = sr.ReadLine()) != null)
                        {
                            if (szLine.Contains("IS.exe"))
                            {
                                Thread.Sleep(250);
                                bOK = true;
                                break;
                            }
                        }
                    }
                    if (--nTryCount == 0 || bOK) break;
                    Thread.Sleep(1000);
                }
            }

            nTryCount = 3;
            if (anDesVision == -1 || anDesVision == 4) // Check Vision 5
            {
                si.Arguments = string.Format("/s {0}V3b /fi \"IMAGENAME eq IS.exe\"", szPCName);
                run.StartInfo = si;
                bOK = false;
                while (run.Start())
                {
                    Thread.Sleep(250);
                    using (StreamReader sr = run.StandardOutput)
                    {
                        string szLine = string.Empty;
                        while ((szLine = sr.ReadLine()) != null)
                        {
                            if (szLine.Contains("IS.exe"))
                            {
                                Thread.Sleep(250);
                                bOK = true;
                                break;
                            }
                        }
                    }
                    if (--nTryCount == 0 || bOK) break;
                    Thread.Sleep(1000);
                }
            }
        }

        public static void CommandExecuteSingle(int anDesVision = -1)
        {
            string szPCName = "";

            ProcessStartInfo si = new ProcessStartInfo();
            si.FileName = "schtasks.exe";
            si.UseShellExecute = false;
            si.RedirectStandardInput = true;
            Process run = new Process();

            if ((anDesVision == -1 || anDesVision == 0))  // Run Vision 1
            {
                szPCName = MainWindow.Setting.SubSystem.IS.IP[0];
                si.Arguments = string.Format("/run /tn IS /s {0}", szPCName);
                run.StartInfo = si;
                run.Start();
            }

            if (anDesVision == -1 || anDesVision == 1)  // Run Vision 2
            {
                szPCName = MainWindow.Setting.SubSystem.IS.IP[1];
                si.Arguments = string.Format("/run /tn IS /s {0}", szPCName);
                run.StartInfo = si;
                run.Start();
            }

            if (anDesVision == -1 || anDesVision == 2)  // Run Vision 3
            {
                szPCName = MainWindow.Setting.SubSystem.IS.IP[2];
                si.Arguments = string.Format("/run /tn IS /s {0}", szPCName);
                run.StartInfo = si;
                run.Start();
            }
            if (anDesVision == -1 || anDesVision == 3)  // Run Vision 4
            {
                szPCName = MainWindow.Setting.SubSystem.IS.IP[3];
                si.Arguments = string.Format("/run /tn IS /s {0}", szPCName);
                run.StartInfo = si;
                run.Start();
            }
            if (anDesVision == -1 || anDesVision == 4)  // Run Vision 5
            {
                szPCName = MainWindow.Setting.SubSystem.IS.IP[4];
                si.Arguments = string.Format("/run /tn IS /s {0}", szPCName);
                run.StartInfo = si;
                run.Start();
            }
        }

        public static void CommandExecute(bool debug, int anDesVision = -1)
        {
            string szPCName = "";

            ProcessStartInfo si = new ProcessStartInfo();
            si.FileName = "schtasks.exe";
            si.UseShellExecute = false;
            si.WindowStyle = ProcessWindowStyle.Hidden;
            si.CreateNoWindow = true;
            si.RedirectStandardInput = true;
            Process run = new Process();

            if ((anDesVision == -1 || anDesVision == 0) && !debug)  // Run Vision 1
            {
                szPCName = MainWindow.Setting.SubSystem.IS.IP[0];
                si.Arguments = string.Format("/run /tn IS /s {0}", szPCName);
                run.StartInfo = si;
                run.Start();
            }

            if (anDesVision == -1 || anDesVision == 1)  // Run Vision 2
            {
                szPCName = MainWindow.Setting.SubSystem.IS.IP[1];
                si.Arguments = string.Format("/run /tn IS /s {0}", szPCName);
                run.StartInfo = si;
                run.Start();
            }

            if (anDesVision == -1 || anDesVision == 2)  // Run Vision 3
            {
                szPCName = MainWindow.Setting.SubSystem.IS.IP[2];
                si.Arguments = string.Format("/run /tn IS /s {0}", szPCName);
                run.StartInfo = si;
                run.Start();
            }
            if (anDesVision == -1 || anDesVision == 3)  // Run Vision 4
            {
                szPCName = MainWindow.Setting.SubSystem.IS.IP[3];
                si.Arguments = string.Format("/run /tn IS /s {0}", szPCName);
                run.StartInfo = si;
                run.Start();
            }
            if (anDesVision == -1 || anDesVision == 4)  // Run Vision 5
            {
                szPCName = MainWindow.Setting.SubSystem.IS.IP[4];
                si.Arguments = string.Format("/run /tn IS /s {0}", szPCName);
                run.StartInfo = si;
                run.Start();
            }
        }

        public static void CommandCloseISSingle(int anDesVision = -1)
        {
            string szPCName = "";

            ProcessStartInfo si = new ProcessStartInfo();
            si.FileName = "tasklist.exe";
            si.UseShellExecute = false;
            si.RedirectStandardInput = true;
            si.RedirectStandardOutput = true;
            Process run = new Process();

            if (MainWindow.Setting.SubSystem.PLC.UsePLC)
            {
                if ((anDesVision == -1 || anDesVision == 0))
                {
                    szPCName = MainWindow.Setting.SubSystem.IS.IP[0];
                    si.Arguments = string.Format("/s {0} /f /im IS.exe", szPCName);
                    run.StartInfo = si;

                    run.Start();
                }
                if (anDesVision == -1 || anDesVision == 1)
                {
                    szPCName = MainWindow.Setting.SubSystem.IS.IP[1];
                    si.Arguments = string.Format("/s {0} /f /im IS.exe", szPCName);
                    run.StartInfo = si;
                    run.Start();
                }

                if (anDesVision == -1 || anDesVision == 2)
                {
                    szPCName = MainWindow.Setting.SubSystem.IS.IP[2];
                    si.Arguments = string.Format("/s {0} /f /im IS.exe", szPCName);
                    run.StartInfo = si;
                    run.Start();
                }
                if (anDesVision == -1 || anDesVision == 3)
                {
                    szPCName = MainWindow.Setting.SubSystem.IS.IP[3];
                    si.Arguments = string.Format("/s {0} /f /im IS.exe", szPCName);
                    run.StartInfo = si;
                    run.Start();
                }
                if (anDesVision == -1 || anDesVision == 4)
                {
                    szPCName = MainWindow.Setting.SubSystem.IS.IP[4];
                    si.Arguments = string.Format("/s {0} /f /im IS.exe", szPCName);
                    run.StartInfo = si;
                    run.Start();
                }

            }
        }

        public static void RemoteVisionProcessKill()
        {
            try
            {
                for (int i = 0; i < MainWindow.Setting.SubSystem.IS.IP.Length; i++)
                {
                    if (!MainWindow.Setting.SubSystem.IS.UseBASlave && i == 4) continue;/////SettingINI Program Index라서 수정 불필요
                    if (!MainWindow.Setting.SubSystem.IS.UseCASlave && i == 2) continue;/////SettingINI Program Index라서 수정 불필요
                    string remoteName = @"\\" + MainWindow.Setting.SubSystem.IS.IP[i] + @"\root\cimv2";

                    ConnectionOptions con = new ConnectionOptions();
                    con.Username = "bga";
                    con.Password = "vision";

                    ManagementScope managementScope = new ManagementScope(remoteName, con);
                    managementScope.Options.Authentication = AuthenticationLevel.PacketPrivacy;
                    managementScope.Connect();
                    ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_Process Where Name = 'IS.exe'");
                    ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(managementScope, objectQuery);
                    ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
                    foreach (ManagementObject managementObject in managementObjectCollection)
                    {
                        managementObject.InvokeMethod("Terminate", null);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Vision 원격 종료 실패");
            }
        }
        public static void RemoteVisionProcessStart()
        {
            try
            {
                for (int i = 0; i < MainWindow.Setting.SubSystem.IS.IP.Length; i++)
                {
                    if ((!MainWindow.Setting.SubSystem.IS.UseBASlave && i == 4) || (!MainWindow.Setting.SubSystem.IS.UseCASlave && i == 2)) continue;/////SettingINI Program Index라서 수정 불필요
                    string szPCName = MainWindow.Setting.SubSystem.IS.IP[i];

                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = "schtasks.exe";
                    si.UseShellExecute = false;
                    si.WindowStyle = ProcessWindowStyle.Hidden;
                    si.CreateNoWindow = true;
                    si.RedirectStandardInput = true;
                    Process run = new Process();

                    si.Arguments = string.Format("/run /tn IS /s {0} /u {1} /p {2}", szPCName, "bga", "vision");
                    run.StartInfo = si;
                    run.Start();
                    Thread.Sleep(300);
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Vision 원격 종료 실패, 서비스 Remote Registy Service 'On' 을 확인하세요.");
            }
        }
        public static void RemoteVisionProcessKillSingle(int anDesVision)
        {
            try
            {
                IndexInfo IndexInfo = Convert_CheckVisionSlave(anDesVision);
                int FrameGrabberIndex = VID.CalcIndex(anDesVision);
                if (!MainWindow.Setting.SubSystem.IS.UseBASlave && IndexInfo.Slave) return;/////SettingINI Program Index라서 수정 불필요
                if (!MainWindow.Setting.SubSystem.IS.UseCASlave && IndexInfo.Slave) return;/////SettingINI Program Index라서 수정 불필요

                string remoteName = @"\\" + MainWindow.Setting.SubSystem.IS.IP[FrameGrabberIndex] + @"\root\cimv2";

                ConnectionOptions con = new ConnectionOptions();
                con.Username = "bga";
                con.Password = "vision";

                ManagementScope managementScope = new ManagementScope(remoteName, con);
                managementScope.Options.Authentication = AuthenticationLevel.PacketPrivacy;
                managementScope.Connect();
                ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_Process Where Name = 'IS.exe'");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(managementScope, objectQuery);
                ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
                foreach (ManagementObject managementObject in managementObjectCollection)
                {
                    managementObject.InvokeMethod("Terminate", null);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Vision 원격 종료 실패");
            }
        }
        public static void RemoteVisionProcessStartSingle(int anDesVision)
        {
            try
            {
                if (anDesVision >= MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount)
                {
                    if (!MainWindow.Setting.SubSystem.IS.UseBASlave && (anDesVision - (MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount) % 2 == 1)) return;
                }
                if (anDesVision >= MainWindow.Setting.Job.BPCount)
                { 
                    if (!MainWindow.Setting.SubSystem.IS.UseCASlave && (anDesVision - MainWindow.Setting.Job.BPCount) % 2 == 1) return;
                }
                string szPCName = MainWindow.Setting.SubSystem.IS.IP[anDesVision];

                ProcessStartInfo si = new ProcessStartInfo();
                si.FileName = "schtasks.exe";
                si.UseShellExecute = false;
                si.WindowStyle = ProcessWindowStyle.Hidden;
                si.CreateNoWindow = true;
                si.RedirectStandardInput = true;
                Process run = new Process();

                si.Arguments = string.Format("/run /tn IS /s {0} /u {1} /p {2}", szPCName, "bga", "vision");
                run.StartInfo = si;
                run.Start();
                Thread.Sleep(300);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Vision 원격 종료 실패, 서비스 Remote Registy Service 'On' 을 확인하세요.");
            }
        }

        public static void CommandCloseIS(bool debug, int anDesVision = -1)
        {
            string szPCName = Setting.General.MachineName;

            Process run = new Process();

            ProcessStartInfo si = new ProcessStartInfo();
            si.FileName = "taskkill.exe";
            si.WindowStyle = ProcessWindowStyle.Hidden;
            si.CreateNoWindow = true;
            si.UseShellExecute = false;
            si.RedirectStandardInput = true;

            if (MainWindow.Setting.SubSystem.PLC.UsePLC)
            {
                if ((anDesVision == -1 || anDesVision == 0) && !debug)
                {
                    szPCName = MainWindow.Setting.SubSystem.IS.IP[0];
                    si.Arguments = string.Format("/s {0} /f /im IS.exe", szPCName);
                    run.StartInfo = si;

                    run.Start();
                }
                if (anDesVision == -1 || anDesVision == 1)
                {
                    szPCName = MainWindow.Setting.SubSystem.IS.IP[1];
                    si.Arguments = string.Format("/s {0} /f /im IS.exe", szPCName);
                    run.StartInfo = si;
                    run.Start();
                }

                if (anDesVision == -1 || anDesVision == 2)
                {
                    szPCName = MainWindow.Setting.SubSystem.IS.IP[2];
                    si.Arguments = string.Format("/s {0} /f /im IS.exe", szPCName);
                    run.StartInfo = si;
                    run.Start();
                }
                if (anDesVision == -1 || anDesVision == 3)
                {
                    szPCName = MainWindow.Setting.SubSystem.IS.IP[3];
                    si.Arguments = string.Format("/s {0} /f /im IS.exe", szPCName);
                    run.StartInfo = si;
                    run.Start();
                }
                if (anDesVision == -1 || anDesVision == 4)
                {
                    szPCName = MainWindow.Setting.SubSystem.IS.IP[4];
                    si.Arguments = string.Format("/s {0} /f /im IS.exe", szPCName);
                    run.StartInfo = si;
                    run.Start();
                }
                System.Threading.Thread.Sleep(2000);
            }
        }
        #endregion

        #region Report MES POP
        public bool CreatePOPManager(bool bLog)
        {
            //TEST SERVER: 55.60.234.128
            //BGA AOI SERVER: 55.60.232.177
            //BGA AVI SERVER: 55.60.233.148
            //LF AVI SERVER: 55.60.101.135
            //설비 적용 시, Setting 파일에서 설정할 수 있도록 적용바람

            //ex. new ClientManager(Setting.IGSServerIP, Setting.IGSServerDBPort, Setting.MachineCode, Setting.IGSSite)
            //MachineName은 MES에 등록되어 있는 명칭으로 해야함. ex. BAV07: O, CAI10: X (옛날 설비코드 사용하지 말 것)
            //IGSSite: "BGA", "LF" 중 택 1
            popManager = new ClientManager(MainWindow.Setting.General.POP_IP, MainWindow.Setting.General.POP_BK_IP, 3306, MainWindow.Setting.General.MachineName, "BGA");
            ClientManager.OnLog += LogView;
            //Server와 네트워크가 끊겨도, 현장 POP PC를 사용하여 보고할 수 있기 때문에 Server에 의존적이면 안된다. 
            //POP 자동보고 없이도 설비가 가동될 수 있도록 조치 필요
            int nRes = popManager.Initialize();
            if (nRes == -2)
            {
                ClientManager.Log("IGS Server DB Connection Fail.");

                if (!bLog)
                    MessageBox.Show("POP 서버 DB 연결 실패");

                return false;
            }
            else if (nRes == -3)
            {
                //IGS Server 관리자에게 요청하여 상태 모니터링 테이블에 현재 설비 추가 조치 필요. (MES상 설비코드 등록 후)
                ClientManager.Log("POP 서버에 현재 설비 정보가 없습니다. 개발 담당자에게 의뢰바랍니다.");

                if (!bLog)
                    MessageBox.Show("POP 서버에 현재 설비 정보가 없습니다. 개발 담당자에게 의뢰바랍니다.");

                return false;
            }

            InspectionMonitoringCtrl.ReportCtrl.SetManager(ref popManager);
            //각 설비의 Loss 저장경로 전달. 완료보고 시, 자동으로 Loss폴더에서 해당 로트의 결과를 읽어옴.
            InspectionMonitoringCtrl.ReportCtrl.SetLossPath(MainWindow.Setting.General.POP_Path);

            //작업자 정보 설정
            InspectionMonitoringCtrl.userCtrl.SetUser(MainWindow.Setting.General.POP_LastUser);

            return true;
        }

        public bool POPStartReport()
        {
            if (popManager != null && bPOPInit)
            {
                string errMsg;
                if (POPStartPara.bNormalLot)
                {
                    if (!popManager.SetPOPStartReport(ref POPStartPara, POPStartPara.bRestart, out errMsg))
                    {
                        if (popManager.ServerHeartbeat())
                        {
                            MessageBox.Show(string.Format("시작 보고를 하지 못하였습니다.\n실패 사유: {0}", errMsg));
                            ClientManager.Log(string.Format("StartReport Fail, ErrMsg: {0}", errMsg));
                            return false;
                        }
                        else
                        {
                            MessageBox.Show("서버 이상으로 시작보고에 실패하였습니다. 수동으로 보고 바랍니다.");
                            ClientManager.Log(string.Format("서버 이상으로 시작보고에 실패하였습니다. 수동으로 보고 바랍니다."));
                            return true;
                        }
                    }

                    //재검 작업의 경우 Timer 체크를 하지 않는다.
                    if (POPStartPara.bRestart)
                        UseTimer = false;
                    else
                        UseTimer = true;

                    return true;
                }
                else
                {
                    //테스트 오더의 경우 Timer 체크를 하지 않는다.
                    UseTimer = false;
                    return true;
                }
            }
            else
            {
                MessageBox.Show("서버 이상으로 POP 자동화가 비활성화 되어 있습니다. 수동으로 보고 바랍니다.");
                ClientManager.Log(string.Format("서버 이상으로 POP 자동화가 비활성화 되어 있습니다. 수동으로 보고 바랍니다."));
                return true;
            }            
        }

        public void POPStopReport(int nStopState)
        {
            if (MainWindow.Setting.General.UsePOP && bPOPInit)
            {
                switch (nStopState)
                {
                    case 0:
                        ClientManager.Log(string.Format("루틴: 제품 이송 중 정지"));
                        break;
                    case 1:
                        ClientManager.Log(string.Format("루틴: 검사 시작 실패로 인한 정지"));
                        break;
                    case 2:
                        ClientManager.Log(string.Format("루틴: 검사 중지 버튼 클릭"));
                        break;
                    case 3:
                        ClientManager.Log(string.Format("루틴: PLC와 PC ID 미일치로 인한 정지"));
                        break;
                    case 4:
                        ClientManager.Log(string.Format("루틴: VRS 맵 생성 실패로 인한 정지"));
                        break;
                    case 5:
                        ClientManager.Log(string.Format("루틴: 강제종료로 인한 정지"));
                        break;
                    case 6:
                        ClientManager.Log(string.Format("루틴: PLC 마킹 ID 비정상으로 인한 정지"));
                        break;
                    case 7:
                        ClientManager.Log(string.Format("루틴: 누적불량 발생으로 인한 정지"));
                        break;
                    case 8:
                        ClientManager.Log(string.Format("루틴: 검사 완료로 인한 정지"));
                        break;
                }

                string errMsg;
                bool bComplete = false;
                if (nStopState == 8)
                {
                    //완료 보고
                    DateTime lastWorkTime = (DateTime)InspectionMonitoringCtrl.IgsCtrl_OnRequest(EventType.GET_LASTTIME);
                    IGS.SubWindow.POP_COMPLETE_Window dlg = new IGS.SubWindow.POP_COMPLETE_Window(lastWorkTime);
                    if (dlg.bInitialize && (bool)dlg.ShowDialog())
                    {
                        ClientManager.Log(string.Format("실제 마지막 스트립 작업 시간: {0}", lastWorkTime.ToString("yyyyMMdd HH:mm:ss")));
                        bComplete = true;

                        //statectrl update
                        popStateCtrl.UpdateState(false);
                    }
                }

                if (!bComplete)
                {
                    //자동 비가동 보고
                    if (popManager.SetPOPDefaultLoss(0, out errMsg))
                        ClientManager.Log(string.Format("DefaultLoss Report Success."));
                    else
                        ClientManager.Log(string.Format("DefaultLoss Report Fail, Error: {0}", errMsg));
                }
            }
        }

        public bool GetModelInfo(string modelName, ref PCS.ELF.AVI.ModelInformation model)
        {
            if (!MainWindow.Setting.General.UsePOP) return false;
            List<MODEL_RECIPE_DATA> dataList = new List<MODEL_RECIPE_DATA>();
            if (popManager.GetMESModelInfo(modelName, out dataList))
            {
                if (dataList.Count > 0)
                {
                    try
                    {
                        model.Strip.UnitColumn = Convert.ToInt32(dataList[0].UPP);
                        model.Strip.UnitRow = Convert.ToInt32(dataList[0].UNIT_PER_ROW);
                        model.Strip.Block = Convert.ToInt32(dataList[0].ARRAY);
                        model.Strip.Width = Convert.ToDouble(dataList[0].STRIP_WIDTH);
                        model.Strip.Height = Convert.ToDouble(dataList[0].ITEM_LEN);
                        // model.Strip.UnitWidth = Convert.ToDouble(dataList[0].ITEM_LEN);  //pitch를 파싱 할것
                        //  model.Strip.UnitHeight = Convert.ToDouble(dataList[0].ITEM_LEN);
                        model.ITS.BBT = dataList[0].BBT == "1" ? true : false;
                        model.ITS.InnerAOI = dataList[0].AOI_IN == "1" ? true : false;
                        model.ITS.LeftID = dataList[0].STRIP_WAY == "1" ? true : false;
                        model.ITS.OuterAOI = dataList[0].AOI_OUT == "1" ? true : false;
                        model.ITS.SKIPDATA = dataList[0].SKIPDATA == "1" ? true : false;
                        model.ITS.UseITS = dataList[0].MARKING_2D == "1" ? true : false;  //marking_2d
                        model.AutoNG = (model.Strip.UnitColumn * model.Strip.UnitRow) * Convert.ToInt32(dataList[0].XOUT_YIELD);
                    }
                    catch { }
                }
            }

            return true;
        }

        private void LogView(string msg)
        {
            //설비 로컬 Log 파일에 추가하거나, 별도의 로그파일을 만들어 저장 필요
            MainWindow.Log("POP", SeverityLevel.DEBUG, msg, true);
        }

        #endregion

        #region save resize image
        //자동검사 시작 시, Resize Section Image를 ResultPath로 Copy
        public void CopyResizeSectionImage(int nSize = 4)
        {
            try
            {
                string orgModelPath = string.Format(@"{0}/{1}/{2}/", MainWindow.Setting.General.ModelPath, MainWindow.CurrentGroup.Name, MainWindow.CurrentModel.Name);
                string resizeModelPath = string.Format(@"{0}/{1}/{2}/Resize_{3}", MainWindow.Setting.General.ModelPath, MainWindow.CurrentGroup.Name, MainWindow.CurrentModel.Name, nSize);

                List<string> orgSectionList = new List<string>();

                string[] files = Directory.GetFiles(orgModelPath, "*Unit Section 1.bmp");
                foreach (string file in files)
                    orgSectionList.Add(file);

                files = Directory.GetFiles(orgModelPath, "*Unit Section 1-R.bmp");
                foreach (string file in files)
                    orgSectionList.Add(file);

                if (!Directory.Exists(resizeModelPath) || Directory.GetFiles(resizeModelPath).Length != orgSectionList.Count)
                {
                    foreach (string file in orgSectionList)
                        SaveResizeSectionImage(file, nSize);
                }

                string resultDir = string.Format(@"{0}/{1}/{2}/{3}", MainWindow.Setting.General.ResultPath, MainWindow.CurrentGroup.Name, MainWindow.CurrentModel.Name, 
                    InspectionMonitoringCtrl.TableDataCtrl.Re_LotNo);
                if (!Directory.Exists(resultDir))
                    Directory.CreateDirectory(resultDir);

                resultDir = string.Format(@"{0}/Resize_{1}", resultDir, nSize);
                if (!Directory.Exists(resultDir))
                    Directory.CreateDirectory(resultDir);

                string[] sections = Directory.GetFiles(resizeModelPath);
                foreach (string section in sections)
                {
                    FileInfo fInfo = new FileInfo(section);
                    string fileName = fInfo.Name;
                    string destPath = string.Format(@"{0}/{1}", resultDir, fileName);

                    File.Copy(section, destPath, true);
                }
            }
            catch (Exception ex)
            {
                Log("PCS", SeverityLevel.DEBUG, string.Format("CopyResizeSectionImage Exception: {0}", ex.Message), true);
            }
        }

        //분석용 Resize Image 저장
        public void SaveResizeSectionImage(string originFile, int nSize = 4)
        {
            try
            {
                FileInfo fInfo = new FileInfo(originFile);

                string dirPath = fInfo.DirectoryName;
                string fileName = fInfo.Name;
                string resizeDir = string.Format(@"{0}/Resize_{1}", dirPath, nSize);

                if (!Directory.Exists(resizeDir))
                    Directory.CreateDirectory(resizeDir);

                string resizeFile = string.Format(@"{0}/{1}", resizeDir, fileName);
                GetResizeSectionImg(originFile, nSize).Save(resizeFile);
            }
            catch (Exception ex)
            {
                Log("PCS", SeverityLevel.DEBUG, string.Format("SaveResizeSectionImage Exception: {0}", ex.Message), true);
            }
        }

        private void SaveVRSTeachingSectionImage()
        {
            string srcPath = Setting.General.ModelPath + MainWindow.CurrentGroup.Name + "\\" + MainWindow.CurrentModel.Name + "\\";
            string dstPath = Setting.General.ResultPath + "\\" + MainWindow.CurrentGroup.Name + "\\" + MainWindow.CurrentModel.Name + "\\" + currOrder + "\\" + "VRS" + "\\";

            if (!Directory.Exists(dstPath)) Directory.CreateDirectory(dstPath);

            for(int i = 0; i < MainWindow.Setting.Job.BPCount; i++)
            {
                if (File.Exists(srcPath + (Surface.BP1 + i).ToString() + "-Unit Section 1.bmp")) GetResizeSectionImg(srcPath + (Surface.BP1 + i).ToString() + "-Unit Section 1.bmp").Save(dstPath + (Surface.BP1 + i).ToString() + "-Unit Section 1.bmp");
                if (File.Exists(srcPath + (Surface.BP1 + i).ToString() + "-Unit Section 1-R.bmp")) GetResizeSectionImg(srcPath + (Surface.BP1 + i).ToString() + "-Unit Section 1-R.bmp").Save(dstPath + (Surface.BP1 + i).ToString() + "-Unit Section 1.bmp");
            }
            for (int i = 0; i < MainWindow.Setting.Job.CACount; i++)
            {
                if (File.Exists(srcPath + (Surface.CA1 + i).ToString() + "-Unit Section 1.bmp")) GetResizeSectionImg(srcPath + (Surface.CA1 + i).ToString() + "-Unit Section 1.bmp").Save(dstPath + (Surface.CA1 + i).ToString() + "-Unit Section 1.bmp");
                if (File.Exists(srcPath + (Surface.CA1 + i).ToString() + "-Unit Section 1-R.bmp")) GetResizeSectionImg(srcPath + (Surface.CA1 + i).ToString() + "-Unit Section 1-R.bmp").Save(dstPath + (Surface.CA1 + i).ToString() + "-Unit Section 1.bmp");
            }
            for (int i = 0; i < MainWindow.Setting.Job.BACount; i++)
            {
                if (File.Exists(srcPath + (Surface.BA1 + i).ToString() + "-Unit Section 1.bmp")) GetResizeSectionImg(srcPath + (Surface.BA1 + i).ToString() + "-Unit Section 1.bmp").Save(dstPath + (Surface.BA1 + i).ToString() + "-Unit Section 1.bmp");
                if (File.Exists(srcPath + (Surface.BA1 + i).ToString() + "-Unit Section 1-R.bmp")) GetResizeSectionImg(srcPath + (Surface.BA1 + i).ToString() + "-Unit Section 1-R.bmp").Save(dstPath + (Surface.BA1 + i).ToString() + "-Unit Section 1.bmp");
            }            
        }

        private System.Drawing.Bitmap GetResizeSectionImg(string path, int nSize = 4)
        {
            // 원본 이미지
            System.Drawing.Bitmap sourceImage = new System.Drawing.Bitmap(path);

            // 사이즈가 변경된 이미지(1/nSize로 축소)
            int width = sourceImage.Width / nSize;
            int height = sourceImage.Height / nSize;

            return ResizeBitmap(sourceImage, width, height);
        }

        public System.Drawing.Bitmap ResizeBitmap(System.Drawing.Bitmap bmp, int width, int height)
        {
            System.Drawing.Bitmap result = new System.Drawing.Bitmap(width, height);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }

            return result;
        }
        #endregion

        #region Convert_ViewIndexToVisionIndex
        public struct IndexInfo
        {
            public int VisionIndex;                     // Viewer의 Vision 번호
            public int SurfaceScanCount;                // Viewer의 Master:Vision별 Scan 번호
            public CategorySurface CategorySurface;     // Viewer의 Surface 대분류
            public Surface Surface;                     // Viewer의 Surface 세분화
            public int Index;                           // Viewer의 Surface 내 Index 
            public int SurfaceBeginIndex;               // Viewer의 Surface 시작 번호 ex) BA1: CA:1 BP:1의 번호
            public bool Slave;                          // Viewer가 Master PC인지 아닌지
            public int SurfaceTotalCount;               // Surface 별 Maximum Scan Count
            public int ScanIndex;                       // Scan Count 
        }
        public IndexInfo Convert_ViewIndexToVisionIndex(int nIndex)
        {
            IndexInfo structindex = new IndexInfo();
            if (nIndex < MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count)
            {
                structindex.SurfaceBeginIndex = 0;
                structindex.Index = nIndex;
                structindex.VisionIndex = (int)Math.Truncate((double)(nIndex / VID.BP_ScanComplete_Count));//////// 검사 2개 이상 할때 PC를 추가하겠지..?
                structindex.SurfaceScanCount = nIndex % VID.BP_ScanComplete_Count;
                structindex.CategorySurface = CategorySurface.BP;
                structindex.Surface = Surface.BP1 + structindex.Index;
                structindex.Slave = structindex.Index % VID.BP_ScanComplete_Count != 0 ? true : false;
                structindex.SurfaceTotalCount = MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count;
                structindex.ScanIndex = structindex.Index / VID.BP_ScanComplete_Count;
            }
            else if (nIndex >= MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count && nIndex < MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count + MainWindow.Setting.Job.CACount)
            {
                structindex.SurfaceBeginIndex = MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count;
                structindex.Index = nIndex - MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count;
                structindex.VisionIndex = VID.BP_PC_Count + structindex.Index % VID.CA_PC_Count;/////PC  2대를 홀수 짝수 운영 하기 때문에
                structindex.SurfaceScanCount = (int)(structindex.Index / VID.CA_PC_Count);
                structindex.CategorySurface = CategorySurface.CA;
                structindex.Surface = Surface.CA1 + structindex.Index;
                structindex.Slave = structindex.Index % VID.CA_PC_Count != 0 ? true : false;
                int[] CountArray = new int[VID.CA_PC_Count];
                for (int count = 1; count <= MainWindow.Setting.Job.CACount; count++)
                {
                    int ArrayIndex = count % VID.CA_PC_Count;
                    CountArray[ArrayIndex]++;
                }
                structindex.SurfaceTotalCount = CountArray[VID.CA_PC_Count - 1 - structindex.Index % VID.CA_PC_Count];
                structindex.ScanIndex = structindex.Index + MainWindow.Setting.Job.BPCount;
            }
            else
            {
                structindex.SurfaceBeginIndex = MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count + MainWindow.Setting.Job.CACount;
                structindex.Index = nIndex - (MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count + MainWindow.Setting.Job.CACount);
                structindex.VisionIndex = VID.BP_PC_Count + VID.CA_PC_Count + structindex.Index % VID.BA_PC_Count;/////PC  2대를 홀수 짝수 운영 하기 때문에 
                structindex.SurfaceScanCount = (int)(structindex.Index / VID.BA_PC_Count);
                structindex.CategorySurface = CategorySurface.BA;
                structindex.Surface = Surface.BA1 + structindex.Index;
                structindex.Slave = structindex.Index % VID.BA_PC_Count != 0 ? true : false;
                int[] CountArray = new int[VID.BA_PC_Count];
                for (int count = 1; count <= MainWindow.Setting.Job.BACount; count++)
                {
                    int ArrayIndex = count % VID.BA_PC_Count;
                    CountArray[ArrayIndex]++;
                }
                structindex.SurfaceTotalCount = CountArray[VID.BA_PC_Count - 1 - structindex.Index % VID.BA_PC_Count];
                structindex.ScanIndex = structindex.Index + MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount;
            }
            return structindex;
        }
        public static IndexInfo Convert_ViewIndexToVisionIndexs(int nIndex)
        {
            IndexInfo structindex = new IndexInfo();
            if (nIndex < MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count) // hs - BP 영역
            {
                structindex.SurfaceBeginIndex = 0;
                structindex.Index = nIndex; // hs - 현재 인덱스
                structindex.VisionIndex = (int)Math.Truncate((double)(nIndex / VID.BP_ScanComplete_Count));//////// 검사 2개 이상 할때 PC를 추가하겠지..?
                structindex.SurfaceScanCount = nIndex % VID.BP_ScanComplete_Count;
                structindex.CategorySurface = CategorySurface.BP;
                structindex.Surface = Surface.BP1 + structindex.Index;
                structindex.Slave = structindex.Index % VID.BP_ScanComplete_Count != 0 ? true : false;
                structindex.SurfaceTotalCount = MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count;
                structindex.ScanIndex = structindex.Index / VID.BP_ScanComplete_Count;
            }
            else if (nIndex >= MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count && nIndex < MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count + MainWindow.Setting.Job.CACount) // hs - BP 영역 벗어난후 CA 영역
            {
                structindex.SurfaceBeginIndex = MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count; // hs - BP영역 끝값
                structindex.Index = nIndex - MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count; // hs - CA영역에서의 인덱스 값
                structindex.VisionIndex = VID.BP_PC_Count + structindex.Index % VID.CA_PC_Count;/////PC  2대를 홀수 짝수 운영 하기 때문에
                structindex.SurfaceScanCount = (int)(structindex.Index / VID.CA_PC_Count);
                structindex.CategorySurface = CategorySurface.CA;
                structindex.Surface = Surface.CA1 + structindex.Index;
                structindex.Slave = structindex.Index % VID.CA_PC_Count != 0 ? true : false;
                int[] CountArray = new int[VID.CA_PC_Count];
                for (int count = 1; count <= MainWindow.Setting.Job.CACount; count++)
                {
                    int ArrayIndex = count % VID.CA_PC_Count;
                    CountArray[ArrayIndex]++;
                }
                structindex.SurfaceTotalCount = CountArray[VID.CA_PC_Count - 1 - structindex.Index % VID.CA_PC_Count];
                structindex.ScanIndex = structindex.Index + MainWindow.Setting.Job.BPCount;
            }
            else
            {
                structindex.SurfaceBeginIndex = MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count + MainWindow.Setting.Job.CACount; // hs - BA영역 (BP, CA 영역 끝값)
                structindex.Index = nIndex - (MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count + MainWindow.Setting.Job.CACount);
                structindex.VisionIndex = VID.BP_PC_Count + VID.CA_PC_Count + structindex.Index % VID.BA_PC_Count;/////PC  2대를 홀수 짝수 운영 하기 때문에 
                structindex.SurfaceScanCount = (int)(structindex.Index / VID.BA_PC_Count);
                structindex.CategorySurface = CategorySurface.BA;
                structindex.Surface = Surface.BA1 + structindex.Index;
                structindex.Slave = structindex.Index % VID.BA_PC_Count != 0 ? true : false;
                int[] CountArray = new int[VID.BA_PC_Count];
                for (int count = 1; count <= MainWindow.Setting.Job.BACount; count++)
                {
                    int ArrayIndex = count % VID.BA_PC_Count;
                    CountArray[ArrayIndex]++;
                }
                structindex.SurfaceTotalCount = CountArray[VID.BA_PC_Count - 1 - structindex.Index % VID.BA_PC_Count];
                structindex.ScanIndex = structindex.Index + MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount;
            }
            return structindex;
        }

        public static IndexInfo Convert_CheckVisionSlave(int nIndex)
        {
            IndexInfo structindex = new IndexInfo();
            if (nIndex < MainWindow.Setting.Job.BPCount)
            {
                structindex.SurfaceBeginIndex = 0;
                structindex.Index = nIndex;
                structindex.VisionIndex = (int)Math.Truncate((double)(nIndex / VID.BP_ScanComplete_Count));//////// 검사 2개 이상 할때 PC를 추가하겠지..?
                structindex.SurfaceScanCount = nIndex % VID.BP_ScanComplete_Count;
                structindex.CategorySurface = CategorySurface.BP;
                structindex.Surface = Surface.BP1 + structindex.Index;
                structindex.Slave = structindex.Index % VID.BP_ScanComplete_Count != 0 ? true : false;
                structindex.SurfaceTotalCount = MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count;
                structindex.ScanIndex = structindex.Index / VID.BP_ScanComplete_Count;
            }
            else if (nIndex >= MainWindow.Setting.Job.BPCount && nIndex < MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount)
            {
                structindex.SurfaceBeginIndex = MainWindow.Setting.Job.BPCount;
                structindex.Index = nIndex - MainWindow.Setting.Job.BPCount;
                structindex.VisionIndex = VID.BP_PC_Count + structindex.Index % VID.CA_PC_Count;/////PC  2대를 홀수 짝수 운영 하기 때문에
                structindex.SurfaceScanCount = (int)(structindex.Index / VID.CA_PC_Count);
                structindex.CategorySurface = CategorySurface.CA;
                structindex.Surface = Surface.CA1 + structindex.Index;
                structindex.Slave = structindex.Index % VID.CA_PC_Count != 0 ? true : false;
                int[] CountArray = new int[VID.CA_PC_Count];
                for (int count = 1; count <= MainWindow.Setting.Job.CACount; count++)
                {
                    int ArrayIndex = count % VID.CA_PC_Count;
                    CountArray[ArrayIndex]++;
                }
                structindex.SurfaceTotalCount = CountArray[VID.CA_PC_Count - 1 - structindex.Index % VID.CA_PC_Count];
                structindex.ScanIndex = structindex.Index + MainWindow.Setting.Job.BPCount;
            }
            else
            {
                structindex.SurfaceBeginIndex = MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount;
                structindex.Index = nIndex - (MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount);
                structindex.VisionIndex = VID.BP_PC_Count + VID.CA_PC_Count + structindex.Index % VID.BA_PC_Count;/////PC  2대를 홀수 짝수 운영 하기 때문에 
                structindex.SurfaceScanCount = (int)(structindex.Index / VID.BA_PC_Count);
                structindex.CategorySurface = CategorySurface.BA;
                structindex.Surface = Surface.BA1 + structindex.Index;
                structindex.Slave = structindex.Index % VID.BA_PC_Count != 0 ? true : false;
                int[] CountArray = new int[VID.BA_PC_Count];
                for (int count = 1; count <= MainWindow.Setting.Job.BACount; count++)
                {
                    int ArrayIndex = count % VID.BA_PC_Count;
                    CountArray[ArrayIndex]++;
                }
                structindex.SurfaceTotalCount = CountArray[VID.BA_PC_Count - 1 - structindex.Index % VID.BA_PC_Count];
                structindex.ScanIndex = structindex.Index + MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount;
            }
            return structindex;
        }

        public static IndexInfo Convert_Vision(CategorySurface categorySurface, int VisionNum, int nIndex)
        {
            IndexInfo structindex = new IndexInfo();
            if (categorySurface == CategorySurface.BP)
            {
                structindex.SurfaceBeginIndex = 0;
                structindex.Index = (VisionNum - VID.BP1) * VID.BP_ScanComplete_Count + nIndex;
                structindex.VisionIndex = VisionNum;
                structindex.SurfaceScanCount = nIndex;
                structindex.Surface = Surface.BP1 + structindex.Index;
                structindex.Slave = nIndex % VID.BP_ScanComplete_Count != 0 ? true : false;
                structindex.CategorySurface = CategorySurface.BP;
                structindex.SurfaceTotalCount = MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count;
                structindex.ScanIndex = structindex.Index / VID.BP_ScanComplete_Count;
            }
            else if (categorySurface == CategorySurface.CA)
            {
                structindex.SurfaceBeginIndex = MainWindow.Setting.Job.BPCount;
                structindex.Index = (VisionNum - VID.CA1) + nIndex * VID.CA_PC_Count;
                structindex.VisionIndex = VisionNum;
                structindex.SurfaceScanCount = (int)(structindex.Index / VID.CA_PC_Count);
                structindex.Surface = Surface.CA1 + structindex.Index;
                structindex.Slave = nIndex % VID.CA_PC_Count != 0 ? true : false;
                structindex.CategorySurface = CategorySurface.CA;
                int[] CountArray = new int[VID.CA_PC_Count];
                for (int count = 1; count <= MainWindow.Setting.Job.CACount; count++)
                {
                    int ArrayIndex = count % VID.CA_PC_Count;
                    CountArray[ArrayIndex]++;
                }
                structindex.SurfaceTotalCount = CountArray[VID.CA_PC_Count - 1 - structindex.Index % VID.CA_PC_Count];
                structindex.ScanIndex = structindex.Index + MainWindow.Setting.Job.BPCount;
            }
            else if (categorySurface == CategorySurface.BA)
            {
                structindex.SurfaceBeginIndex = MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount;
                structindex.Index = (VisionNum - VID.BA1) + nIndex * VID.BA_PC_Count;
                structindex.VisionIndex = VisionNum;
                structindex.SurfaceScanCount = (int)(structindex.Index / VID.BA_PC_Count);
                structindex.Surface = Surface.BA1 + structindex.Index;
                structindex.Slave = nIndex % VID.BA_PC_Count != 0 ? true : false;
                structindex.CategorySurface = CategorySurface.BA;
                int[] CountArray = new int[VID.BA_PC_Count];
                for (int count = 1; count <= MainWindow.Setting.Job.BACount; count++)
                {
                    int ArrayIndex = count % VID.BA_PC_Count;
                    CountArray[ArrayIndex]++;
                }
                structindex.SurfaceTotalCount = CountArray[VID.BA_PC_Count - 1 - structindex.Index % VID.BA_PC_Count];
                structindex.ScanIndex = structindex.Index + MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount;
            }
            return structindex;
        }
        public struct LightInfo
        {
            public int LightIndex;                     // LightIndex Vision 번호
            public CategorySurface CategorySurface;     // Viewer의 Surface 대분류
            public int ValueIndex;
        }
        public static LightInfo Convert_LightIndex(CategorySurface surface, int Index)
        {
            LightInfo LightInfo = new LightInfo();
            if (surface == CategorySurface.BP)
            {
                LightInfo.LightIndex = Index / 2 + 0;//// BP는 2스캔이 1스캔 전체 이므로, Controller Index
                LightInfo.ValueIndex = Index / VID.BP_ScanComplete_Count + 0;////BP : 0
            }
            else if (surface == CategorySurface.CA)
            {
                LightInfo.LightIndex = Index % 2 + 1;//// Controller Count, Controller Index
                LightInfo.ValueIndex = Index + 2;////CA : 2
            }
            else
            {
                LightInfo.LightIndex = Index % 2 + 3;//// Controller Count, Controller Index
                LightInfo.ValueIndex = Index + 6;////BA : 6
            }
            return LightInfo;
        }
        #endregion        
    }
}
