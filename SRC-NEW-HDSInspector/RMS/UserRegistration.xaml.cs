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
 * @file  UserRegistration.xaml.cs
 * @brief 
 *  Behind code of UserRegistration.xaml
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.08.27
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.27 First creation.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using RMS.Generic.UserManagement;
using Common;

namespace ResultManagement
{
    /// <summary>   User registration.  </summary>
    public partial class UserRegistration : Window
    {
        /// <summary> Information describing the user </summary>
        public UserInformation m_UserInformation = new UserInformation();

        /// <summary> Manager for user </summary>
        private UserManager m_UserManager = null;

        /// <summary> true to b valid identifier </summary>
        private bool m_bValidID = false;

        /// <summary>   Initializes a new instance of the UserRegistration class. </summary>
        public UserRegistration(UserManager aUserManager)
        {
            m_UserManager = aUserManager;

            InitializeComponent();
            InitializeEvent();
            InitializeDialog();
        }

        /// <summary>   Initializes the dialog. </summary>
        private void InitializeDialog()
        {
            m_UserManager.LoadUserAutority();
            this.cmbAuthority.ItemsSource = m_UserManager.UserAuthorityList;
            this.cmbAuthority.DisplayMemberPath = "AuthName";
            this.cmbAuthority.SelectedValuePath = "AuthCode"; 
            this.cmbAuthority.SelectedIndex = 0;
            this.imgValidID.Visibility = Visibility.Collapsed; // V marks.
        }

        /// <summary>   Initializes the event. </summary>
        private void InitializeEvent()
        {
            this.btnCheckID.Click += btnCheckID_Click;
            this.btnCancel.Click += btnCancel_Click;
            this.btnOK.Click += btnOK_Click;
            this.txtID.TextChanged += txtID_TextChanged;
        }

        /// <summary>   Query if this object is valid. </summary>
        private bool IsValid()
        {
            // Check ID is valid.
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("U001"), "Information");
                txtID.Focus();
                return false;
            }
            else if (m_bValidID == false)
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("U004"), "Information");
                txtID.Focus();
                return false;
            }

            // Check Name is valid.
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("U002"), "Information");
                txtName.Focus();
                return false;
            }

            // Compare NewPassword to CheckNewPassword.
            if (string.IsNullOrEmpty(txtNewPassword.Password) && string.IsNullOrEmpty(txtCheckNewPassword.Password))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("U003"), "Information");
                txtNewPassword.Focus();
                return false;
            }
            else if (!string.IsNullOrEmpty(txtNewPassword.Password) && string.IsNullOrEmpty(txtCheckNewPassword.Password))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("U008"), "Information");
                txtCheckNewPassword.Focus();
                return false;
            }
            else if (string.IsNullOrEmpty(txtNewPassword.Password) && !string.IsNullOrEmpty(txtCheckNewPassword.Password))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("U003"), "Information");
                txtNewPassword.Focus();
                return false;
            }
            else if (!string.IsNullOrEmpty(txtNewPassword.Password) && !string.IsNullOrEmpty(txtCheckNewPassword.Password))
            {
                // 새로운 '비밀번호'와 '비밀번호 확인' 내용이 같은지 체크한다.
                if (txtNewPassword.Password.Equals(txtCheckNewPassword.Password))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("U009"), "Information");
                    return false;
                }
            }        

            // Valid User Information.
            return true;
        }

        /// <summary>   Event handler. Called by txtID for text changed events. </summary>
        private void txtID_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.imgValidID.Visibility = Visibility.Collapsed;
            this.m_bValidID = false;
        }

        /// <summary>   Event handler. Called by btnCheckID for click events. </summary>
        private void btnCheckID_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("U001"), "Information");
                txtID.Focus();
                return;
            }

            if (m_UserManager.SearchByID(txtID.Text).Equals(true))
            {
                // ID 중복
                this.imgValidID.Visibility = Visibility.Collapsed;
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("U010"), "Information");
                m_bValidID = false;               
            }
            else
            {
                // ID 비중복
                this.imgValidID.Visibility = Visibility.Visible;
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("U011"), "Information");
                m_bValidID = true;
            }
        }

        #region OK & Cancel click events.
        /// <summary>   Event handler. Called by btnOK for click events. </summary>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid())
            {
                m_UserInformation.ID = this.txtID.Text;
                m_UserInformation.Name = this.txtName.Text;
                m_UserInformation.Password = MD5Core.GetHashString(txtNewPassword.Password + MD5Core.GetHashString(txtID.Text));

                m_UserInformation.Authority.AuthCode = cmbAuthority.SelectedValue.ToString();
                m_UserInformation.Authority.AuthName = cmbAuthority.Text;
                m_UserInformation.RegistrationDate = DateTime.Now;

                this.DialogResult = true;
                this.Close();
            }
        }

        /// <summary>   Event handler. Called by btnCancel for click events. </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        #endregion
    }
}
