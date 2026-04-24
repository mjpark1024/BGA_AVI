using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using IGS.Classes;

namespace IGS
{
    /// <summary>
    /// ReportMenuCtrl.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public delegate void ReportMenuAcceptEventHandler();
    public delegate object IgsToMainEventHandler(EventType eventType, object param = null);

    public partial class ReportMenuCtrl : UserControl
    {
        public static ClientManager clientManager;
        public static string lossPath = "";
        public event ReportMenuAcceptEventHandler OnAccept;
        public event IgsToMainEventHandler OnRequest;

        public ReportMenuCtrl()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.btnAttend.Click += BtnAttend_Click;
            this.btnLeave.Click += BtnLeave_Click;
            this.btnExcept.Click += BtnExcept_Click;
            this.btnStop.Click += BtnStop_Click;
            this.btnComplete.Click += BtnComplete_Click;
            this.btnLotSearch.Click += BtnLotSearch_Click;
        }

        public void SetManager(ref ClientManager cManager)
        {
            clientManager = cManager;
            clientManager.UpdateState();
        }

        public void SetLossPath(string lossFolderPath)
        {
            lossPath = lossFolderPath;   
        }

        public void ButtonEnable(bool bEnable)
        {
            btnExcept.IsEnabled = bEnable;
            btnStop.IsEnabled = bEnable;
            btnComplete.IsEnabled = bEnable;
        }

        private void BtnAttend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!clientManager.ServerHeartbeat(1))
                {
                    MessageBox.Show("서버와 연결할 수 없습니다.\n기존 POP 프로그램으로 처리바랍니다.");
                    return;
                }

                SubWindow.POP_ATTEND_Window dlg = new SubWindow.POP_ATTEND_Window();
                if ((bool)dlg.ShowDialog())
                {
                    ReportMenuAcceptEventHandler er = OnAccept;
                    if (er != null)
                        er();
                }
            }
            catch { }
        }

        private void BtnLeave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!clientManager.ServerHeartbeat(1))
                {
                    MessageBox.Show("서버와 연결할 수 없습니다.\n기존 POP 프로그램으로 처리바랍니다.");
                    return;
                }

                SubWindow.POP_LEAV_Window dlg = new SubWindow.POP_LEAV_Window();
                if ((bool)dlg.ShowDialog())
                {
                    ReportMenuAcceptEventHandler er = OnAccept;
                    if (er != null)
                        er();
                }
            }
            catch { }
        }

        private void BtnExcept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!clientManager.ServerHeartbeat(1))
                {
                    MessageBox.Show("서버와 연결할 수 없습니다.\n기존 POP 프로그램으로 처리바랍니다.");
                    return;
                }

                SubWindow.POP_EXCEPT_Window dlg = new SubWindow.POP_EXCEPT_Window();
                if ((bool)dlg.ShowDialog())
                {
                    ReportMenuAcceptEventHandler er = OnAccept;
                    if (er != null)
                        er();
                }
            }
            catch { }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!clientManager.ServerHeartbeat(1))
                {
                    MessageBox.Show("서버와 연결할 수 없습니다.\n기존 POP 프로그램으로 처리바랍니다.");
                    return;
                }

                SubWindow.POP_SUSPEND_Window dlg = new SubWindow.POP_SUSPEND_Window();
                if (dlg.bInitialize && (bool)dlg.ShowDialog())
                {
                    ReportMenuAcceptEventHandler er = OnAccept;
                    if (er != null)
                        er();
                }
            }
            catch { }
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!clientManager.ServerHeartbeat(1))
                {
                    MessageBox.Show("서버와 연결할 수 없습니다.\n기존 POP 프로그램으로 처리바랍니다.");
                    return;
                }

                IgsToMainEventHandler itm = OnRequest;
                //if (itm != null)
                //    itm(EventType.MAKE_LOSS);

                DateTime lastWorkTime = (DateTime)itm(EventType.GET_LASTTIME);

                SubWindow.POP_COMPLETE_Window dlg = new SubWindow.POP_COMPLETE_Window(lastWorkTime);
                if (dlg.bInitialize && (bool)dlg.ShowDialog())
                {
                    ReportMenuAcceptEventHandler er = OnAccept;
                    if (er != null)
                        er();
                }
            }
            catch { }
        }

        private void BtnLotSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!clientManager.ServerHeartbeat(1))
                {
                    MessageBox.Show("서버와 연결할 수 없습니다.\n기존 POP 프로그램으로 처리바랍니다.");
                    return;
                }

                SubWindow.POP_LOT_SEARCH_Window dlg = new SubWindow.POP_LOT_SEARCH_Window();
                if ((bool)dlg.ShowDialog())
                {
                    ReportMenuAcceptEventHandler er = OnAccept;
                    if (er != null)
                        er();
                }
            }
            catch { }
        }
    }
}
