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

namespace IGS
{
    /// <summary>
    /// POPStateCtrl_Horizontal.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class POPStateCtrl_Horizontal : UserControl
    {
        public POPStateCtrl_Horizontal()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.btnRefresh.Click += BtnRefresh_Click;
            ClientManager.OnState += UpdateState;
            UpdateState(false);
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateState(false);
        }

        public void UpdateState(bool bDelay)
        {
            if (ReportMenuCtrl.clientManager == null)
                return;

            if (!ReportMenuCtrl.clientManager.ServerConnectionCheck() && !ReportMenuCtrl.clientManager.ServerHeartbeat(1))
            {
                MessageBox.Show("SERVER와 연결할 수 없습니다.");
                return;
            }

            if (bDelay)
                Thread.Sleep(1000);

            STATE_TABLE_DATA data;
            if (ReportMenuCtrl.clientManager.GetCurStateInfo(out data))
            {
                Action action = delegate
                {
                    tbMachine.Text = data.MACHINE_CODE;
                    tbState.Text = data.STATE == "RUN" ? "가동중" : string.Format("{0}-{1}", data.EXC_CD, data.EXC_NM);
                    tbLot.Text = data.ORDER;
                    tbOpCode.Text = data.OP_CODE;
                    tbUserName.Text = data.USER_NAME;
                }; this.Dispatcher.Invoke(action);
            }
        }
    }
}
