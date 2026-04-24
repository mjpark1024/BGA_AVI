using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using IGS.Classes;

namespace IGS.SubWindow
{
    /// <summary>
    /// ATTEND_VERIFY_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ATTEND_VERIFY_Window : Window
    {
        #region Member Variables.
        private List<string> machineList;
        private string curID;

        public ObservableCollection<VerifyUserDisplayData> Users
        {
            get { return m_users; }
        }
        private ObservableCollection<VerifyUserDisplayData> m_users = new ObservableCollection<VerifyUserDisplayData>();
        #endregion

        public ATTEND_VERIFY_Window(List<string> mcList, string strID)
        {
            InitializeComponent();

            machineList = mcList;
            curID = strID;

            InitializeDialog();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.btnCancel.Click += BtnCancel_Click;
            this.btnOK.Click += BtnOK_Click;
            this.cbAll.Click += CbAll_Click;

            this.btnTotalSearch.Click += BtnTotalSearch_Click;
            this.btnTotalSet.Click += BtnTotalSet_Click;

            this.lbInfo.PreviewMouseWheel += LbInfo_PreviewMouseWheel;
            this.lbInfo.PreviewMouseDoubleClick += LbInfo_PreviewMouseDoubleClick;
            this.bdTitle.PreviewMouseLeftButtonDown += BdTitle_PreviewMouseLeftButtonDown;
        }

        private void InitializeDialog()
        {
            //GET VERIFY Y/N LIST
            Dictionary<string, string> ynList = new Dictionary<string, string>();
            List<STATE_TABLE_DATA> allList;
            if (ReportMenuCtrl.clientManager.GetStateInfo(out allList))
            {
                foreach (STATE_TABLE_DATA data in allList)
                    ynList.Add(data.MACHINE_CODE, data.VERIFY_YN);
            }
            else
            {
                MessageBox.Show("설비 정보를 가져오는데 실패하였습니다. 다시 시도바랍니다.");
                return;
            }

            //SET MACHINE LIST
            m_users.Clear();
            foreach (string mc in machineList)
            {
                VerifyUserDisplayData data = new VerifyUserDisplayData();

                data.mc_code = mc;
                data.verify_yn = ynList[mc];
                data.verify_user = "";

                m_users.Add(data.Clone());
            }

            lbInfo.DataContext = m_users;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            foreach (VerifyUserDisplayData user in m_users)
            {
                if (user.verify_yn == "Y" && user.verify_user == "")
                {
                    MessageBox.Show(string.Format("{0} 설비는 Verify 적용 설비입니다.\nVerify 작업자를 선택해주세요.", user.mc_code));
                    return;
                }
            }

            if (MessageBox.Show(string.Format("선택된 {0}개 설비에 대하여\n{1} 사번으로 출근 보고를 진행하시겠습니까?", machineList.Count, curID),
                    "확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                this.DialogResult = true;
        }

        private void CbAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (VerifyUserDisplayData data in m_users)
                data.bChecked = (bool)cbAll.IsChecked;
        }

        private void BtnTotalSearch_Click(object sender, RoutedEventArgs e)
        {
            SubWindow.VERIFY_USER_LIST_Window dlg = new VERIFY_USER_LIST_Window();
            if ((bool)dlg.ShowDialog())
            {
                string strUser = "";
                for (int i = 0; i < dlg.selectedUser.Count; i++)
                {
                    if (i > 0)
                        strUser += ",";
                    strUser += string.Format("{0}({1})", dlg.selectedUser[i].userName, dlg.selectedUser[i].userID);
                }
                tboxTotalUser.Text = strUser;

                foreach (VerifyUserDisplayData data in m_users)
                    data.bChecked = false;
            }
        }

        private void BtnTotalSet_Click(object sender, RoutedEventArgs e)
        {
            foreach (VerifyUserDisplayData user in m_users)
            {
                if (user.bChecked)
                    user.verify_user = tboxTotalUser.Text;
            }
        }

        private void LbInfo_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroll = (ScrollViewer)svInfo;
            if (e.Delta < 0)
            {
                if (scroll.VerticalOffset - e.Delta < scroll.ExtentHeight - scroll.ViewportHeight)
                    scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta);
                else
                    scroll.ScrollToBottom();
            }
            else
            {
                if (scroll.VerticalOffset + e.Delta > 0)
                    scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta);
                else
                    scroll.ScrollToTop();
            }
            e.Handled = true;
        }

        private void LbInfo_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnTotalSet_Click(null, null);
        }

        private void BdTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string strMC = ((Button)sender).Tag.ToString();
                m_users.First(u => u.mc_code == strMC).verify_user = "";
            }
            catch { }
        }
    }
}
