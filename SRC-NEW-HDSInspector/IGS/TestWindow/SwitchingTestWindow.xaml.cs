using IGS.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IGS.TestWindow
{
    /// <summary>
    /// SwitchingTestWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SwitchingTestWindow : Window
    {
        //공정서버 이중화 Test Window
        //Client에서 지속적으로 요청을 보내는 중에 서버가 Switching될 때 Delay 확인용

        private Thread loopThd;
        private bool bLoopFlag = true;

        public SwitchingTestWindow()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.btnStart.Click += BtnStart_Click;
            this.btnStop.Click += BtnStop_Click;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (loopThd != null)
            {
                loopThd.Abort();
                loopThd = null;
            }

            bLoopFlag = true;
            loopThd = new Thread(TestThread);
            loopThd.Start();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            bLoopFlag = false;
        }

        private void TestThread()
        {
            string strUserID = "210238";
            string strLot = "105365533";
            ReportType[] reportArr = new ReportType[] { ReportType.GET_STOPCODE, ReportType.GET_CHANGEPOINT, ReportType.GET_LOTINFO, ReportType.GET_MODEL_HISTORY, ReportType.GET_USERINFO };

            Random rand = new Random();
            while (bLoopFlag)
            {
                try
                {
                    int nReport = rand.Next(0, 5);

                    ReportMenuCtrl.clientManager.ServerHeartbeat(1);

                    if (reportArr[nReport] == ReportType.GET_STOPCODE)
                    {
                        ReportMenuCtrl.clientManager.GetStopCodeList(strUserID, out List<string> codeList, out string errMsg);
                    }
                    else if (reportArr[nReport] == ReportType.GET_CHANGEPOINT)
                    {
                        ReportMenuCtrl.clientManager.GetChangePointInfo(strLot, out List<POP_CHANGE_POINT_OUTPUT_DATA> outList, out string errMsg);
                    }
                    else if (reportArr[nReport] == ReportType.GET_LOTINFO)
                    {
                        ReportMenuCtrl.clientManager.GetMESLotInfo(strLot, out POP_LOT_DATA outInfo);
                    }
                    else if (reportArr[nReport] == ReportType.GET_MODEL_HISTORY)
                    {
                        ReportMenuCtrl.clientManager.GetModelHistoryInfo(strLot, out List<POP_RECENT_HISTORY_DATA> outList, out string errMsg);
                    }
                    else if (reportArr[nReport] == ReportType.GET_USERINFO)
                    {
                        ReportMenuCtrl.clientManager.GetUserInfo(strUserID, out POP_USER_DATA userData, out string errMsg);
                    }
                }
                catch (Exception ex)
                {
                    string tmp = ex.Message;
                }

                Thread.Sleep(2000);
            }
        }
    }
}
