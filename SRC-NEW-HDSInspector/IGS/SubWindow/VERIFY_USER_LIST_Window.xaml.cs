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
    /// VERIFY_USER_LIST_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VERIFY_USER_LIST_Window : Window
    {
        public List<UserSelectDisplayData> selectedUser;
        public ObservableCollection<UserSelectDisplayData> Users
        {
            get { return m_users; }
        }
        private ObservableCollection<UserSelectDisplayData> m_users = new ObservableCollection<UserSelectDisplayData>();

        public VERIFY_USER_LIST_Window()
        {
            InitializeComponent();
            InitializeEvent();
            InitializeDialog();
        }

        private void InitializeEvent()
        {
            this.btnOK.Click += BtnOK_Click;
            this.btnCancel.Click += BtnCancel_Click;

            this.tboxName.TextChanged += TboxName_TextChanged;
            this.tboxID.TextChanged += TboxID_TextChanged;

            this.tboxName.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;
            this.tboxID.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;

            this.bdTitle.PreviewMouseLeftButtonDown += BdTitle_PreviewMouseLeftButtonDown;
            this.lbInfo.PreviewMouseWheel += LbInfo_PreviewMouseWheel;

            this.lbInfo.PreviewMouseDoubleClick += LbInfo_PreviewMouseDoubleClick;
        }

        private void InitializeDialog()
        {
            //GET VERIFY USER LIST
            List<POP_USER_DATA> userList;
            string errMsg = "";
            if (ReportMenuCtrl.clientManager == null || !ReportMenuCtrl.clientManager.GetVerifyUserList(out userList, out errMsg))
            {
                MessageBox.Show("작업자 정보를 불러오지 못했습니다.");
                return;
            }

            //SET USER LIST
            m_users.Clear();
            foreach (POP_USER_DATA user in userList)
            {
                UserSelectDisplayData data = new UserSelectDisplayData();
                data.userID = user.USERID;
                data.userName = user.USER_NM;
                m_users.Add(data.Clone());
            }

            lbInfo.DataContext = m_users;
        }

        private void BdTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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
            BtnOK_Click(null, null);
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (lbInfo.SelectedItem == null)
            {
                MessageBox.Show("작업자를 선택해주세요.");
                return;
            }

            selectedUser = new List<UserSelectDisplayData>();
            foreach(UserSelectDisplayData user in Users)
            {
                if (user.bChecked)
                    selectedUser.Add(user.Clone());
            }

            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void TboxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strName = tboxName.Text;
            if (strName == "")
                return;

            if (m_users.Any(m => m.userName.Contains(strName)))
            {
                UserSelectDisplayData find = m_users.First(m => m.userName.Contains(strName));
                lbInfo.SelectedItem = m_users.First(m => m.userName == find.userName);

                lbInfo.ScrollIntoView(lbInfo.SelectedItem);
            }
        }

        private void TboxID_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strID = tboxID.Text;
            if (strID == "")
                return;

            if (m_users.Any(m => m.userID.Contains(strID)))
            {
                UserSelectDisplayData find = m_users.First(m => m.userID.Contains(strID));
                lbInfo.SelectedItem = m_users.First(m => m.userID == find.userID);

                lbInfo.ScrollIntoView(lbInfo.SelectedItem);
            }
        }

        private void Tbox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
    }
}
