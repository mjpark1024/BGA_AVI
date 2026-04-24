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
    /// POPStateCtrl_Vertical.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class POPStateCtrl_Vertical : UserControl
    {
        public POPStateCtrl_Vertical()
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
            if (bDelay)
                Thread.Sleep(1000);

            STATE_TABLE_DATA data;
            if (ReportMenuCtrl.clientManager != null && ReportMenuCtrl.clientManager.GetCurStateInfo(out data))
            {
                tbMachine.Text = data.MACHINE_CODE;
                tbState.Text = data.STATE == "RUN" ? "가동중" : string.Format("{0}-{1}", data.EXC_CD, data.EXC_NM);
                tbLot.Text = data.ORDER;
                tbUserName.Text = data.USER_NAME;
            }
        }
    }
}
