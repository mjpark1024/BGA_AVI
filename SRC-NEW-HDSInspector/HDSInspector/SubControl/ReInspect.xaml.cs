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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HDSInspector.SubControl
{
    /// <summary>
    /// ReInspect.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public enum ConID
    {
        Stop, 
        Continue,
        Restart,
        FRestart
    }

    public delegate void SendEvent(ConID nID);
    public partial class ReInspect : UserControl
    {
        public event SendEvent sendevent;
        private Thread thdTimeCheck;
        private bool bButtonSelect = false;

        public ReInspect()
        {
            InitializeComponent();
            this.btnReset.Click += BtnReset_Click;
            this.btnStop.Click += BtnStop_Click;
            this.btnFReset.Click += BtnFReset_Click;
        }

        private void BtnFReset_Click(object sender, RoutedEventArgs e)
        {
            bButtonSelect = true;
            POPStateCheck();

            SendEvent er = sendevent;
            er(ConID.FRestart);
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            bButtonSelect = true;

            SendEvent er = sendevent;
            er(ConID.Stop);
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            bButtonSelect = true;
            POPStateCheck();

            SendEvent er = sendevent;
            er(ConID.Restart);
        }

        private void POPStateCheck()
        {
            if (!MainWindow.Setting.General.UsePOP)
                return;

            //비가동 상태 시, 재시작 처리
            MainWindow ptrMain = Application.Current.MainWindow as MainWindow;
            if(ptrMain.popManager.GetCurStateInfo(out STATE_TABLE_DATA curState) && curState != null)
            {
                if(curState.STATE == "STOP" && ptrMain.POPStartPara.strLot == curState.ORDER && curState.EXC_CD != "" )
                {
                    if (ptrMain.popManager.SetPOPStartReport(ref ptrMain.POPStartPara, ptrMain.POPStartPara.bRestart, out string errMsg))
                    {
                        ClientManager.Log(string.Format("POP Start Report Success. Lot: {0}", ptrMain.POPStartPara.strLot));
                    }
                    else
                    {
                        ClientManager.Log(string.Format("POP Start Report Fail. Lot: {0}, Error: {1}", ptrMain.POPStartPara.strLot, errMsg));
                    }
                }
            }
        }

        public void StartTimer()
        {
            if (!MainWindow.Setting.General.UsePOP)
                return;

            if (thdTimeCheck != null)
            {
                thdTimeCheck.Abort();
                thdTimeCheck = null;
            }

            thdTimeCheck = new Thread(TimeCheck);
            thdTimeCheck.Start();
        }

        private void TimeCheck()
        {
            int nStdTime = Environment.TickCount;
            Thread.Sleep(100);

            int nCurrTime = Environment.TickCount;
            while ((nCurrTime - nStdTime) / 1000.0 < 300)
            {
                if (bButtonSelect)
                    return;

                Thread.Sleep(1000);
            }

            //비가동
            MainWindow ptrMain = Application.Current.MainWindow as MainWindow;
            if (ptrMain.popManager.SetPOPDefaultLoss(5, out string errMsg))
            {
                ClientManager.Log(string.Format("ReInspect POP Default Loss Success."));
            }
            else
            {
                ClientManager.Log(string.Format("ReInspect POP Default Loss Fail, Error: {0}", errMsg));
            }
        }
    }
}
