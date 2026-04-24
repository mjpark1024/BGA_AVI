using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common;
using System.Data;
using Common.DataBase;
using HDSInspector.SubWindow;


namespace HDSInspector
{
    /// <summary>
    /// SettingUser.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingUser : UserControl
    {
        private ObservableCollection<User> m_ableUserList = new ObservableCollection<User>();
        private ObservableCollection<User> m_disableUserList = new ObservableCollection<User>();
        private bool m_bChangedIndex = false;
        private bool m_bChangedIndex2 = false;
        CurrentUser m_partialUser = new CurrentUser();
        public SettingUser()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }
        private void InitializeEvent()
        {
            btnSendLeft.Click += new RoutedEventHandler(btnSendLeft_Click);
            btnSendRight.Click += new RoutedEventHandler(btnSendRight_Click);
            btnSelectAll.Click += new RoutedEventHandler(btnSelectAll_Click);
            btnUnSelectAll.Click += new RoutedEventHandler(btnUnSelectAll_Click);
            btnAddUser.Click += new RoutedEventHandler(btnAddUser_Click);
            btnDeleteUser.Click += new RoutedEventHandler(btnDeleteUser_Click);

            lbableGroup.SelectionChanged += lbableGroup_Selected;
            lbdisableGroup.SelectionChanged += lbdisableGroup_Selected;

            svableGroup.PreviewMouseWheel += new MouseWheelEventHandler(svableGroup_PreviewMouseWheel);
            svdisableGroup.PreviewMouseWheel += new MouseWheelEventHandler(svdisableGroup_PreviewMouseWheel);
        }


        /// <summary>   Initializes the dialog. </summary>
        private void InitializeDialog()
        {
            LoadUserGroup();
            grdCurrentUser.DataContext = MainWindow.CurrentUser_HDS;
            lbableGroup.ItemsSource = m_ableUserList;
            lbdisableGroup.ItemsSource = m_disableUserList;
        }
        private void LoadUserGroup()
        {
            string strQuery = string.Format("SELECT user_name, user_passwd, user_auth FROM user_info WHERE use_yn = 1");

            IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery(strQuery);

            if (dataReader != null)
            {
                while (dataReader.Read())
                {
                    User tmpUser = new User();

                    tmpUser.Name = dataReader.GetValue(0).ToString();
                    tmpUser.BusinessNumber = dataReader.GetValue(1).ToString();
                    tmpUser.Author = (dataReader.GetValue(2).ToString() == "0064") ? true : false;
                    if (tmpUser.Author)
                        m_ableUserList.Add(tmpUser);
                    else
                    {
                        if (Convert.ToInt32(dataReader.GetValue(2)) == 65)
                            m_disableUserList.Add(tmpUser);
                    }
                }
                dataReader.Close();
            }
        }
        /// <summary>
        /// button Event
        /// </summary>

        void lbableGroup_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (!m_bChangedIndex)
            {
                m_bChangedIndex = true;
                lbdisableGroup.SelectedIndex = -1;
                m_bChangedIndex2 = false;
            }
        }
        void lbdisableGroup_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (!m_bChangedIndex2)
            {
                m_bChangedIndex2 = true;
                lbableGroup.SelectedIndex = -1;
                m_bChangedIndex = false;
            }
        }
        void svdisableGroup_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double fMouseScroll = svdisableGroup.VerticalOffset;

            fMouseScroll -= e.Delta / 3;

            if (fMouseScroll < 0)
            {
                fMouseScroll = 0;
                svdisableGroup.ScrollToVerticalOffset(fMouseScroll);
            }
            else if (fMouseScroll > svdisableGroup.ScrollableHeight)
            {
                fMouseScroll = svdisableGroup.ScrollableHeight;
                svdisableGroup.ScrollToEnd();
            }
            else
            {
                this.svdisableGroup.ScrollToVerticalOffset(fMouseScroll);
            }
        }

        void svableGroup_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double fMouseScroll = svableGroup.VerticalOffset;

            fMouseScroll -= e.Delta / 3;

            if (fMouseScroll < 0)
            {
                fMouseScroll = 0;
                svableGroup.ScrollToVerticalOffset(fMouseScroll);
            }
            else if (fMouseScroll > svableGroup.ScrollableHeight)
            {
                fMouseScroll = svableGroup.ScrollableHeight;
                svableGroup.ScrollToEnd();
            }
            else
            {
                this.svableGroup.ScrollToVerticalOffset(fMouseScroll);
            }
        }

        void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            AddUserHDS addUserHDS = new AddUserHDS();
            if ((bool)addUserHDS.ShowDialog())
            {
                User tmpuser = new User();

                tmpuser.BusinessNumber = addUserHDS.txtNumber.Text;
                tmpuser.Name = addUserHDS.txtName.Text;
                tmpuser.Author = (addUserHDS.cmbAuthority.SelectedIndex == 0) ? true : false;
                if (tmpuser.Author)
                    m_ableUserList.Add(tmpuser);
                else
                    m_disableUserList.Add(tmpuser);
            }
        }
        void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("삭제 하시겠습니까?", "Yes-No", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();

                    //현재 유저 선별
                    if (lbableGroup.SelectedIndex != -1)
                    {
                        for (int i = 0; i < lbableGroup.SelectedItems.Count; i++)
                        {
                            User tmpuser = lbableGroup.SelectedItems[i] as User;
                            for (int j = 0; j < m_ableUserList.Count; j++)
                            {
                                if (tmpuser.Name == m_ableUserList[j].Name)
                                {
                                    String strQuery = String.Format("DELETE FROM user_info WHERE user_name = '{0}'", tmpuser.Name);

                                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                                    {
                                        ConnectFactory.DBConnector().Commit();
                                        m_ableUserList.RemoveAt(j);
                                        i--;
                                    }
                                    else
                                    {
                                        ConnectFactory.DBConnector().Rollback();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < lbdisableGroup.SelectedItems.Count; i++)
                        {
                            User tmpuser = lbdisableGroup.SelectedItems[i] as User;
                            for (int j = 0; j < m_disableUserList.Count; j++)
                            {
                                if (tmpuser.Name == m_disableUserList[j].Name)
                                {
                                    String strQuery = String.Format("DELETE FROM user_info WHERE user_name = '{0}'", tmpuser.Name);

                                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                                    {
                                        ConnectFactory.DBConnector().Commit();
                                        m_disableUserList.RemoveAt(j);
                                        i--;
                                    }
                                    else
                                    {
                                        ConnectFactory.DBConnector().Rollback();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        void btnSendLeft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();
                    if (lbableGroup.SelectedIndex == -1)
                    {
                        for (int i = 0; i < lbdisableGroup.SelectedItems.Count; i++)
                        {
                            User tmpuser = lbdisableGroup.SelectedItems[i] as User;
                            for (int j = 0; j < m_disableUserList.Count; j++)
                            {
                                if (tmpuser.Name == m_disableUserList[j].Name)
                                {
                                    String strQuery = String.Format("update user_info set user_auth = '0064' where user_name = '{0}'", tmpuser.Name);

                                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                                    {
                                        ConnectFactory.DBConnector().Commit();
                                        m_disableUserList.RemoveAt(j);
                                        tmpuser.Author = true;
                                        m_ableUserList.Add(tmpuser);
                                        i--;
                                    }
                                    else
                                    {
                                        ConnectFactory.DBConnector().Rollback();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }

        }
        void btnSendRight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    if (lbdisableGroup.SelectedIndex == -1)
                    {
                        for (int i = 0; i < lbableGroup.SelectedItems.Count; i++)
                        {
                            User tmpuser = lbableGroup.SelectedItems[i] as User;
                            for (int j = 0; j < m_ableUserList.Count; j++)
                            {
                                if (tmpuser.Name == m_ableUserList[j].Name)
                                {
                                    String strQuery = String.Format("update user_info set user_auth = '0065' where user_name = '{0}'", tmpuser.Name);

                                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                                    {
                                        ConnectFactory.DBConnector().Commit();
                                        m_ableUserList.RemoveAt(j);
                                        tmpuser.Author = false;
                                        m_disableUserList.Add(tmpuser);
                                        i--;
                                    }
                                    else
                                    {
                                        ConnectFactory.DBConnector().Rollback();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }
        void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (lbableGroup.SelectedIndex != -1)
                lbableGroup.SelectAll();
            else
                lbdisableGroup.SelectAll();
        }
        void btnUnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (lbableGroup.SelectedIndex != -1)
                lbableGroup.UnselectAll();
            else
                lbdisableGroup.UnselectAll();
        }


        private class User : NotifyPropertyChanged
        {
            private string m_sName;
            private string m_sBusinessNumber;
            private bool m_bAuthor;

            public string Name { get { return m_sName; } set { m_sName = value; Notify("Name"); } }
            public string BusinessNumber { get { return m_sBusinessNumber; } set { m_sBusinessNumber = value; Notify("BusinessNumber"); } }
            public bool Author { get { return m_bAuthor; } set { m_bAuthor = value; Notify("Author"); } }
        }
        public class CurrentUser : NotifyPropertyChanged
        {
            private string m_sName;
            private string m_sBusinessNumber;
            private bool m_bChanged;
            private bool m_bAuthor;
            private string m_sDateTime;

            public string Name { get { return m_sName; } set { m_sName = value; Notify("Name"); } }
            public string BusinessNumber { get { return m_sBusinessNumber; } set { m_sBusinessNumber = value; Notify("BusinessNumber"); } }
            public bool Changed { get { return m_bChanged; } set { m_bChanged = value; Notify("Changed"); } }
            public bool Author { get { return m_bAuthor; } set { m_bAuthor = value; Notify("Author"); } }
            public string DateTime { get { return m_sDateTime; } set { m_sDateTime = value; Notify("DateTime"); } }
        }
    }
}
