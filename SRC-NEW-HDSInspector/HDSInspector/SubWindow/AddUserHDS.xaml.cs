using System;
using System.Windows;
using System.Windows.Controls;
using RMS.Generic.UserManagement;
using Common;
using System.Windows.Input;

namespace HDSInspector.SubWindow
{
    /// <summary>
    /// AddUserHDS.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddUserHDS : Window
    {
        private UserManager m_UserManager = new UserManager();
        public UserInformation NewUser = new UserInformation();

        private bool m_bValidID = false;

        #region Ctor.
        public AddUserHDS()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }
        #endregion

        #region Initializer.
        private void InitializeDialog()
        {
            m_UserManager.LoadUserAutorityForHDS();
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
            this.txtNumber.TextChanged += txtNumber_TextChanged;
            this.KeyDown += AddUserWindow_KeyDown;
        }
        #endregion

        private void txtNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.imgValidID.Visibility = Visibility.Collapsed;
            this.m_bValidID = false;
        }

        private void btnCheckID_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNumber.Text))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("U001"), "Information");
                txtNumber.Focus();
                return;
            }

            txtNumber.Text = txtNumber.Text.TrimStart(' ');
            txtNumber.Text = txtNumber.Text.TrimEnd(' ');
            if (m_UserManager.SearchByID(txtNumber.Text).Equals(true))
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
            if (string.IsNullOrEmpty(txtNumber.Text))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("U001"), "Information");
                txtNumber.Focus();
                return false;
            }
            else if (m_bValidID == false)
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("U004"), "Information");
                txtNumber.Focus();
                return false;
            }

            // Check Name is valid.
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("U002"), "Information");
                txtName.Focus();
                return false;
            }
            // Valid User Information.
            return true;
        }

        #region OK & Cancel click events.
        private void CloseWithOK()
        {
            if (IsValid())
            {
                NewUser.ID = txtNumber.Text;

                string szName = txtName.Text.TrimStart(' ');
                szName = txtName.Text.TrimEnd(' ');
                NewUser.Name = szName;

                NewUser.Password = txtNumber.Text;

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
