using IGS.Classes;
using IGS.SubWindow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace IGS
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Member Variables.
        public ClientManager clientManager;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            InitializeEvent();
            InitializeDialog();
        }

        private void InitializeEvent()
        {
            ClientManager.OnLog += LogView;
            ReportCtrl.OnAccept += ReportCtrl_OnAccept;
            ReportCtrl.OnRequest += IgsCtrl_OnRequest;
            userCtrl.OnRequest += IgsCtrl_OnRequest;

            this.btnStart.Click += BtnStart_Click;
            this.btnStop.Click += BtnStop_Click;
            this.btnTest.Click += BtnTest_Click;
            this.btnChange.Click += BtnChange_Click;
        }

        private void InitializeDialog()
        {
            //TEST SERVER: 55.60.234.128
            //BGA AOI SERVER: 55.60.232.177
            //BGA AVI SERVER: 55.60.233.161
            //LF AVI SERVER: 55.60.101.135
            //설비 적용 시, Setting 파일에서 설정할 수 있도록 적용바람

            //ex. new ClientManager(Setting.IGSServerIP, Setting.IGSServerDBPort, Setting.MachineCode, Setting.IGSSite)
            //MachineName은 MES에 등록되어 있는 명칭으로 해야함. ex. BAV07: O, CAI10: X (옛날 설비코드 사용하지 말 것)
            //IGSSite: "BGA", "LF" 중 택 1
            
            //clientManager = new ClientManager("192.168.30.228", 3306, "EAV05", "LF");
            //clientManager = new ClientManager("55.60.101.135", 3306, "EAV05", "LF");
            clientManager = new ClientManager("55.60.234.128", "", 3306, "BAV09", "BGA");
            //clientManager = new ClientManager("55.60.234.128", "", 3306, "EAV05", "LF");

            //Server와 네트워크가 끊겨도, 현장 POP PC를 사용하여 보고할 수 있기 때문에 Server에 의존적이면 안된다. 
            //POP 자동보고 없이도 설비가 가동될 수 있도록 조치 필요
            int nRes = clientManager.Initialize();
            if (nRes == -2)
                MessageBox.Show(string.Format("IGS Server DB Connection Fail. Error: {0}", ConnectMysql.errMsg));
            else if (nRes == -3)
            {
                //IGS Server 관리자에게 요청하여 상태 모니터링 테이블에 현재 설비 추가 조치 필요. (MES상 설비코드 등록 후)
                MessageBox.Show("IGS Server 상태 모니터링 테이블에 현재 설비 정보가 없습니다.");
            }

            ReportCtrl.SetManager(ref clientManager);
            //각 설비의 Loss 저장경로 전달. 완료보고 시, 자동으로 Loss폴더에서 해당 로트의 결과를 읽어옴.
            ReportCtrl.SetLossPath(@"D:/PERSONAL_SPACE/");

            //작업자 정보 설정
            userCtrl.SetUser("");
            //userCtrl.SetUser(MainWindow.Setting.LastUser);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            StartReport();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            int nMin = 5;

            string errMsg;
            if (!clientManager.SetPOPDefaultLoss(nMin, out errMsg))
                MessageBox.Show(string.Format("IGS - DefaultLoss Report Fail. ErrMsg: {0}", errMsg));
        }

        //시작보고는 다른 보고와 달리, 자동검사 시작 버튼 누를 시 Background로 보고되기 때문에
        //기존 자동검사 시작 화면 Behind Code에 아래 기능 추가 필요.
        private void StartReport()
        {
            StartTestWindow dlg = new StartTestWindow();
            if ((bool)dlg.ShowDialog())
            {
                POP_START_WINDOW_PARA para = new POP_START_WINDOW_PARA();
                para.strGroup = "양산";                       //MainWindow.CurrentGroup.Name (설비에서 사용하는 그룹명)
                para.strModel = "TESTMODEL";                  //MainWindow.CurrentModel.Name (설비에서 사용하는 모델명), 추후에 경로 조합하여 결과파일 가져가기 위함
                para.strLot = dlg.txtLot.Text.Trim();         //공백이 포함지 않도록 Trim 처리 필요
                para.strOperator = dlg.txtOperator.Text;
                para.strVerifyOper = dlg.tbVeriOperator.Text;
                para.ganji_spec = dlg.tboxGanjiSpec.Text;
                para.ganji_lot = dlg.tboxGanjiLot.Text;
                para.standModel = dlg.stdModel;

                string errMsg;

                //시작보고 날리기
                if (dlg.bNormalLot)
                {
                    if (clientManager.SetPOPStartReport(ref para, dlg.bRestart, out errMsg))
                    {
                        MessageBox.Show("시작보고 성공");
                    }
                    else
                    {
                        MessageBox.Show(string.Format("시작보고 실패. 사유: {0}", errMsg));
                    }
                }
                else
                    MessageBox.Show("MES상 존재하지 않는 오더번호 입니다.\n시작보고를 진행하지 않습니다.");
            }
        }

        //일정시간동안 비가동 시, Default 유실코드로 비가동 중지시킨다.
        //nMinute: 비가동 판단 기준 (ex. 5: 5분 미입력 시)
        //정확한 비가동 시간은 서버에서 (보고시간 - nMinute) 계산해서 보고한다.
        private void BtnLoss_Click(object sender, RoutedEventArgs e)
        {
            int nMin = 5;

            string errMsg;
            if(!clientManager.SetPOPDefaultLoss(nMin, out errMsg))
                ClientManager.Log(string.Format("IGS - DefaultLoss Report Fail. ErrMsg: {0}", errMsg));
            else
                ClientManager.Log(string.Format("IGS - POP Default Loss Report Success."));
        }

        private void LogView(string msg)
        {
            //설비 로컬 Log 파일에 추가하거나, 별도의 로그파일을 만들어 저장 필요

        }

        //보고가 성공적으로 끝났을 경우 Event 발생
        private void ReportCtrl_OnAccept()
        {
            ///필요한 곳 Flag 살려주기
        }

        private object IgsCtrl_OnRequest(EventType eventType, object param = null)
        {
            try
            {
                ///Main에서 관련 이벤트 처리하기
                if (eventType == EventType.GET_LASTTIME)
                {
                    //완료 시점을 어느 시점으로 판단할 것인가 - InspectProcess 내부에서 따로 체크, 마지막 Map 생성시간 등
                    return DateTime.Now;
                }
                else if (eventType == EventType.GET_INPUT_COUNT)
                {
                    //설비의 투입 수량 - 중단 보고 시, 전공정 완성수량이 아닌 설비에 현재까지 투입된 수량이 필요함
                    return 0;
                }
                else if (eventType == EventType.SET_LAST_USER)
                {
                    //마지막 작업자 정보를 메인의 Setting파일에 저장, 재실행 시 해당 기록을 불러옴
                    string lastUser = (string)param;
                    //MainWindow.Setting.LastUser = lastUser;
                }
                else if (eventType == EventType.GET_LAST_USER)
                {
                    //마지막 작업자 정보를 메인의 Setting파일에 저장, 재실행 시 해당 기록을 불러옴
                    //return MainWindow.Setting.LastUser;
                    return "";
                }

                return null;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("IGS - IgsCtrl_OnRequest Exception: {0}", ex.Message));
                return null;
            }
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            Test();
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            string strMC = tbMcCode.Text;
            if (strMC == "")
            {
                MessageBox.Show("변경하고자 하는 설비코드를 입력 바랍니다.");
                tbMcCode.Focus();
                return;
            }

            clientManager.strMachine = strMC;
            clientManager.UpdateState();
        }

        private void Test()
        {
            WARNING_POPUP_Window dlg = new WARNING_POPUP_Window();

            string strLot = "105379989";
            string strOp = "VI32";

            POP_WARNING_INFO winfo;
            List<POP_CHANGE_POINT_OUTPUT_DATA> points;
            string errMsg;
            clientManager.GetWarningInfo(strLot, strOp, out winfo, out errMsg);
            clientManager.GetChangePointInfo(strLot, out points, out errMsg);

            dlg.InitializeDialog(winfo, points);
            dlg.ShowDialog();

            //TestWindow.SwitchingTestWindow dlg = new TestWindow.SwitchingTestWindow();
            //dlg.ShowDialog();
        }
    }
}
