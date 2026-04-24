/*********************************************************************************
 * Copyright(c) 2015 by Haesung DS.
 * 
 * This software is copyrighted by, and is the sole property of Haesung DS.
 * All rigths, title, ownership, or other interests in the software remain the
 * property of Haesung DS. This software may only be used in accordance with
 * the corresponding license agreement. Any unauthorized use, duplication, 
 * transmission, distribution, or disclosure of this software is expressly 
 * forbidden.
 *
 * This Copyright notice may not be removed or modified without prior written
 * consent of Haesung DS reserves the right to modify this 
 * software without notice.
 *
 * Haesung DS.
 * KOREA 
 * http://www.HaesungDS.com
 *********************************************************************************/
/**
 * @file  UserManagement.xaml.cs
 * @brief 
 *  Behind code of UserManagement.xaml
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.08.27
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.27 First creation.
 */

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RMS.Generic.UserManagement;
using Common;

namespace ResultManagement
{
    /// <summary>   User management.  </summary>
    public partial class UserManagement : UserControl
    {
        private UserManager m_UserManager = new UserManager();

        public UserManagement()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }

        private void InitializeDialog()
        {
            this.dgUserTable.ItemsSource = m_UserManager.UserList;
            this.cmSearchOption.Items.Add("이름");
            this.cmSearchOption.Items.Add("ID");
            this.cmSearchOption.SelectedIndex = 0;
        }

        private void InitializeEvent()
        {
            this.btnNew.Click += btnNew_Click;
            this.btnModify.Click += btnModify_Click;
            this.btnDelete.Click += btnDelete_Click;
            this.btnSearch.Click += btnSearch_Click;
            this.dgUserTable.MouseDoubleClick += dgUserTable_MouseDoubleClick;
            this.txtSearch.GotFocus += txtSearch_GotFocus;
            this.txtSearch.LostFocus += txtSearch_LostFocus;
            this.btnSearch.MouseEnter += btn_MouseEnter;
            this.btnSearch.MouseLeave += btn_MouseLeave;
            this.KeyDown += UserManagement_KeyDown;
        }

        private void UserManagement_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnSearch_Click(btnSearch, null);
            }
        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.txtSearch.Text.Equals("전체 검색"))
            {
                this.txtSearch.Clear();
            }
            this.txtSearch.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtSearch.Text))
            {
                this.txtSearch.Foreground = new SolidColorBrush(Colors.Gray);
                this.txtSearch.Text = "전체 검색";
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            // 검색 text box가 전체 검색 일때 작업
            if (this.txtSearch.Text.Equals("전체 검색") || string.IsNullOrEmpty(this.txtSearch.Text))
            {
                m_UserManager.LoadUser();
            }
            else if (this.cmSearchOption.SelectedItem.Equals("이름"))
            {
                m_UserManager.SelectUserByName(this.txtSearch.Text);
            }
            else if (this.cmSearchOption.SelectedItem.Equals("ID"))
            {
                m_UserManager.SelectUserByID(this.txtSearch.Text);
            }
        }

        private void btn_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void btn_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void dgUserTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnModify_Click(sender, null);
        }

        #region User CRUD operation.
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            UserRegistration userRegistration = new UserRegistration(m_UserManager) { Owner = Application.Current.MainWindow };
            if (userRegistration.ShowDialog() == true)
            {
                m_UserManager.AddUser(userRegistration.m_UserInformation);
            }
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgUserTable.SelectedIndex < 0)
            {
                return; // SelectedItem is null.
            }

            UserInformation userInfo = this.dgUserTable.SelectedItem as UserInformation;
            if (userInfo != null)
            {
                UserModification userModification = new UserModification(userInfo, m_UserManager) { Owner = Application.Current.MainWindow };
                if (userModification.ShowDialog() == true)
                {
                    m_UserManager.ModifyUser(userModification.m_UserInformation);
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgUserTable.SelectedIndex < 0)
            {
                return;
            }

            if (MessageBox.Show(ResourceStringHelper.GetInformationMessage("U007"), "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<UserInformation> userList = new List<UserInformation>(); // 삭제를 원하는 Item들의 컬렉션.
                foreach (UserInformation u in this.dgUserTable.SelectedItems)
                {
                    userList.Add(u);
                }

                foreach (UserInformation u in userList)
                {
                    m_UserManager.DeleteUser(u);
                }
            }
        }
        #endregion
    }
}
