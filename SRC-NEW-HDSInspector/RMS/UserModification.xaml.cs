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

using System;
using System.Windows;
using RMS.Generic.UserManagement;
using Common;

namespace ResultManagement
{
    /// <summary>   User modification.  </summary>
    public partial class UserModification : Window
    {
        /// <summary> Information describing the user </summary>
        public UserInformation m_UserInformation = null;

        /// <summary> Manager for user </summary>
        private UserManager m_UserManager = null;

        /// <summary>   Initializes a new instance of the UserModification class. </summary>
        public UserModification(UserInformation aUserInfo, UserManager aUserManager)
        {
            InitializeComponent();
            InitializeDialog(aUserInfo, aUserManager);
            InitializeEvent();
        }

        /// <summary>   Initializes the dialog. </summary>
        private void InitializeDialog(UserInformation aUserInfo, UserManager aUserManager)
        {
            m_UserInformation = aUserInfo;
            m_UserManager = aUserManager;

            m_UserManager.LoadUserAutority();
            this.cmbAuthority.ItemsSource = m_UserManager.UserAuthorityList;
            this.cmbAuthority.DisplayMemberPath = "AuthName";
            this.cmbAuthority.SelectedValuePath = "AuthCode"; 

            this.txtID.Text = m_UserInformation.ID;
            this.txtName.Text = m_UserInformation.Name;
            int selectedIndex = 0;
            foreach (object obj in cmbAuthority.Items.SourceCollection)
            {
                if (m_UserInformation.Authority.AuthCode == ((UserAuthority)obj).AuthCode)
                {
                    break;
                }
                selectedIndex++;
            }
            cmbAuthority.SelectedIndex= selectedIndex;
        }

        /// <summary>   Initializes the event. </summary>
        private void InitializeEvent()
        {
            this.btnOK.Click += new RoutedEventHandler(btnOK_Click);
            this.btnCancel.Click += new RoutedEventHandler(btnCancel_Click);
        }

        /// <summary>   Query if this object is valid. </summary>
        private bool IsValid()
        {
            if (!string.IsNullOrEmpty(txtNewPassword.Password) && string.IsNullOrEmpty(txtCheckNewPassword.Password)) // Check Password is valid.
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("U003"), "Information");
                txtCheckNewPassword.Focus();

                return false;
            }
            else if (string.IsNullOrEmpty(txtNewPassword.Password) && !string.IsNullOrEmpty(txtCheckNewPassword.Password))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("U003"), "Information");
                txtNewPassword.Focus();

                return false;
            }
            else if (string.IsNullOrEmpty(txtNewPassword.Password) && string.IsNullOrEmpty(txtCheckNewPassword.Password))
            {
                m_UserInformation.Authority.AuthCode = cmbAuthority.SelectedValue.ToString();
                m_UserInformation.Authority.AuthName = cmbAuthority.Text;

                return true;
            }
            else if (!string.IsNullOrEmpty(txtNewPassword.Password) && !string.IsNullOrEmpty(txtCheckNewPassword.Password))
            {
                // 새로운 '비밀번호'와 '비밀번호 확인' 내용이 같은지 체크한다.
                if (txtNewPassword.Password.Equals(txtCheckNewPassword.Password))
                {
                    m_UserInformation.Password = MD5Core.GetHashString(txtNewPassword.Password + MD5Core.GetHashString(txtID.Text));
                    m_UserInformation.Authority.AuthCode = cmbAuthority.SelectedValue.ToString();
                    m_UserInformation.Authority.AuthName = cmbAuthority.Text;

                    return true;
                }
                else
                {
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("U005"), "Information");
                    return false;
                }
            }
            return true;
        }

        #region OK & Cancel click events.
        /// <summary>   Event handler. Called by btnOK for click events. </summary>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid())
            {
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
