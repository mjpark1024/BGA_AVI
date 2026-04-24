using System;
using System.Windows;
using System.Windows.Controls;
using RMS.Generic.UserManagement;
using Common;
using System.Windows.Input;

namespace HDSInspector.SubWindow
{
    public partial class AddUserWindow : Window
    {
        private UserManager m_UserManager = new UserManager();
        public UserInformation NewUser = new UserInformation();

        private bool m_bValidID = false;

        #region Ctor.
        public AddUserWindow()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }
        #endregion

        #region Initializer.
        private void InitializeDialog()
        {
            m_UserManager.LoadUserAutority();
            this.cmbAuthority.ItemsSource = m_UserManager.UserAuthorityList;
            this.cmbAuthority.DisplayMemberPath = "AuthName";
            this.cmbAuthority.SelectedValuePath = "AuthCode";
            this.cmbAuthority.SelectedIndex = 0;
            this.imgValidID.Visibility = Visibility.Collapsed; // V marks.
        }

        private void InitializeEvent()
        {
            this.btnCheckID.Click += btnCheckID_Click;
            this.btnCancel.Click += btnCancel_Click;
            this.btnOK.Click += btnOK_Click;
            this.txtID.TextChanged += txtID_TextChanged;
            this.KeyDown += AddUserWindow_KeyDown;
        }
        #endregion

        private void txtID_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.imgValidID.Visibility = Visibility.Collapsed;
            this.m_bValidID = false;
        }

        private void btnCheckID_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("U001"), "Information");
                txtID.Focus();
                return;
            }

            txtID.Text = txtID.Text.TrimStart(' ');
            txtID.Text = txtID.Text.TrimEnd(' ');
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

        private void AddUserWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                CloseWithOK();
            }
            else if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
            }
        }

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

        #region OK & Cancel click events.
        private void CloseWithOK()
        {
            if (IsValid())
            {
                NewUser.ID = txtID.Text;

                string szName = txtName.Text.TrimStart(' ');
                szName = txtName.Text.TrimEnd(' ');
                NewUser.Name = szName;

                NewUser.Password = MD5Core.GetHashString(txtNewPassword.Password + MD5Core.GetHashString(txtID.Text));

                if (NewUser.Authority == null)
                    NewUser.Authority = new UserAuthority();
                NewUser.Authority.AuthCode = cmbAuthority.SelectedValue.ToString();
                NewUser.Authority.AuthName = cmbAuthority.Text;
                NewUser.RegistrationDate = DateTime.Now;

                m_UserManager.AddUser(NewUser);
                this.DialogResult = true;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            CloseWithOK();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        #endregion
    }
}
