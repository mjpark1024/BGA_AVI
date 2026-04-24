using IGS.Classes;
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

namespace IGS
{
    /// <summary>
    /// UserCtrl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserCtrl : UserControl
    {
        public static POP_USER_DATA curUser;
        private POP_USER_DATA searchUser;

        public event IgsToMainEventHandler OnRequest;

        public UserCtrl()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.btnChange.Click += BtnChange_Click;
            this.btnSearch.Click += BtnSearch_Click;
            this.btnOK.Click += BtnOK_Click;
            this.btnCancel.Click += BtnCancel_Click;

            this.tboxUserID.PreviewKeyDown += TboxUserID_PreviewKeyDown;            
        }

        public void SetUser(string userID)
        {
            if (string.IsNullOrEmpty(userID))
            {
                STATE_TABLE_DATA stateData;
                if (ReportMenuCtrl.clientManager.GetCurStateInfo(out stateData) && stateData != null && !string.IsNullOrEmpty(stateData.USER_ID))
                    userID = stateData.USER_ID;
                else
                    return;
            }

            POP_USER_DATA userData;
            string errMsg;

            if (ReportMenuCtrl.clientManager.GetUserInfo(userID, out userData, out errMsg) && userData != null)
            {
                curUser = userData;
                tbDeptName.Text = userData.DEPT_NM;
                tbUserName.Text = userData.USER_NM;
                tbPosName.Text = userData.POS_NM;                
            }
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            grdCurUser.Visibility = Visibility.Hidden;
            grdChange.Visibility = Visibility.Visible;

            tboxUserID.Text = "";
            tbSearchName.Text = "";
            btnOK.IsEnabled = false;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            tbSearchName.Text = "";

            if (tboxUserID.Text.Length != 6)
                return;

            string userID = tboxUserID.Text;
            POP_USER_DATA userData;
            string errMsg;

            if (ReportMenuCtrl.clientManager.GetUserInfo(userID, out userData, out errMsg) && userData != null)
            {
                tbSearchName.Text = string.Format("{0} {1}", userData.USER_NM, userData.POS_NM);
                btnOK.IsEnabled = true;
                searchUser = userData;
            }
            else
                MessageBox.Show(string.Format("입력된 사번으로 MES에서 정보를 조회할 수 없습니다.\n에러메세지: {0}", errMsg));
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (searchUser == null)
            {
                MessageBox.Show("사용자 정보가 올바르지 않습니다. 다시 검색바랍니다.");
                return;
            }

            curUser = searchUser;
            tbDeptName.Text = curUser.DEPT_NM;
            tbUserName.Text = curUser.USER_NM;
            tbPosName.Text = curUser.POS_NM;

            IgsToMainEventHandler itm = OnRequest;
            itm(EventType.SET_LAST_USER, curUser.USERID);

            grdChange.Visibility = Visibility.Hidden;
            grdCurUser.Visibility = Visibility.Visible;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            grdChange.Visibility = Visibility.Hidden;
            grdCurUser.Visibility = Visibility.Visible;
        }

        private void TboxUserID_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            btnOK.IsEnabled = false;
            tbSearchName.Text = "";

            if (e.Key == Key.Enter)
                BtnSearch_Click(null, null);
        }
    }
}
