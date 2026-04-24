using IGS.Classes;
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

namespace IGS.SubWindow
{
    /// <summary>
    /// MY_MACHINE_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MY_MACHINE_Window : Window
    {
        #region Member Variables.
        public ObservableCollection<MyMCDisplayData> LeftMC
        {
            get { return m_leftMC; }
        }
        public ObservableCollection<MyMCDisplayData> RightMC
        {
            get { return m_rightMC; }
        }
        private ObservableCollection<MyMCDisplayData> m_leftMC = new ObservableCollection<MyMCDisplayData>();
        private ObservableCollection<MyMCDisplayData> m_rightMC = new ObservableCollection<MyMCDisplayData>();

        private POP_USER_DATA curUser;
        #endregion

        public MY_MACHINE_Window()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.btnSearch.Click += BtnSearch_Click;
            this.btnCancel.Click += BtnCancel_Click;
            this.btnOK.Click += BtnOK_Click;

            this.lbRightInfo.PreviewMouseWheel += LbRightInfo_PreviewMouseWheel;
            this.lbLeftInfo.PreviewMouseWheel += LbLeftInfo_PreviewMouseWheel;
            this.bdTitle.PreviewMouseLeftButtonDown += BdTitle_PreviewMouseLeftButtonDown;
            this.tbUserID.PreviewKeyDown += TbUserID_PreviewKeyDown;
            this.tbUserID.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;
        }

        public void InitializeDialog(POP_USER_DATA user)
        {
            curUser = user;

            List<STATE_TABLE_DATA> stateList;
            ReportMenuCtrl.clientManager.GetStateInfo(out stateList);

            List<string> mcList = null;
            if (user != null)
            {
                tbUserID.Text = user.USERID;
                tbUserName.Text = user.USER_NM;

                ReportMenuCtrl.clientManager.GetMyMCList(user.USERID, out mcList);
            }

            if (stateList != null)
            {
                int nHalf = stateList.Count / 2;

                for (int i = 0; i < stateList.Count; i++)
                {
                    MyMCDisplayData mc = new MyMCDisplayData();
                    mc.mc_code = stateList[i].MACHINE_CODE;

                    if (mcList != null && mcList.IndexOf(mc.mc_code) != -1)
                        mc.bChecked = true;

                    if (i < nHalf)
                        LeftMC.Add(mc.Clone());
                    else
                        RightMC.Add(mc.Clone());
                }

                lbLeftInfo.DataContext = LeftMC;
                lbRightInfo.DataContext = RightMC;
            }

            if (curUser == null)
            {
                lbLeftInfo.IsEnabled = false;
                lbRightInfo.IsEnabled = false;
            } 
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string strID = tbUserID.Text.Trim();
                if (strID == string.Empty || strID == "")
                {
                    MessageBox.Show("조회하고자 하는 사번을 입력해주세요.");
                    return;
                }

                if (strID.Length != 6)
                {
                    MessageBox.Show("사번은 6자리입니다. 다시 확인바랍니다.");
                    return;
                }
                tbUserName.Text = "";

                POP_USER_DATA data;
                string errMsg;
                if (ReportMenuCtrl.clientManager.GetUserInfo(strID, out data, out errMsg) && data != null)
                {
                    curUser = data;
                    tbUserName.Text = data.USER_NM;

                    lbLeftInfo.IsEnabled = true;
                    lbRightInfo.IsEnabled = true;

                    List<string> mcList = new List<string>();
                    ReportMenuCtrl.clientManager.GetMyMCList(data.USERID, out mcList);

                    foreach (MyMCDisplayData mc in LeftMC)
                    {
                        if (mcList.IndexOf(mc.mc_code) == -1)
                            mc.bChecked = false;
                        else
                            mc.bChecked = true;
                    }

                    foreach (MyMCDisplayData mc in RightMC)
                    {
                        if (mcList.IndexOf(mc.mc_code) == -1)
                            mc.bChecked = false;
                        else
                            mc.bChecked = true;
                    }
                }
                else
                    MessageBox.Show(string.Format("입력된 사번으로 MES에서 정보를 조회할 수 없습니다.\n에러메세지: {0}", errMsg));
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("IGS - MY_MACHINE_Window.cs BtnSearch_Click Exception: {0}", ex.Message));
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            List<string> mcList = new List<string>();

            foreach (MyMCDisplayData mc in LeftMC)
            {
                if (mc.bChecked)
                    mcList.Add(mc.mc_code);
            }

            foreach(MyMCDisplayData mc in RightMC)
            {
                if (mc.bChecked)
                    mcList.Add(mc.mc_code);
            }

            if (ReportMenuCtrl.clientManager.SetMyMCList(curUser.USERID, mcList))
                MessageBox.Show("저장되었습니다.");
            else
                MessageBox.Show("저장에 실패하였습니다.");

            this.DialogResult = true;
        }

        private void LbRightInfo_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroll = (ScrollViewer)svRightInfo;
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

        private void LbLeftInfo_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroll = (ScrollViewer)svLeftInfo;
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

        private void BdTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void TbUserID_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BtnSearch_Click(null, null);
        }

        private void Tbox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
    }
}
