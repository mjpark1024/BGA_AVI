using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RMS.Generic.RejectManagement;
using Common;
using RMS.Generic.UserManagement;

namespace ResultManagement
{
    /// <summary>
    /// Interaction logic for Reject.xaml
    /// </summary>
    public partial class RejectRegistration : Window
    {
        public RejectInformation m_RejectInformation = new RejectInformation();
        private RejectManager m_RejectManager = new RejectManager();
        private bool m_bValidID = false;

        public RejectRegistration()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }

        private void InitializeDialog()
        {
            // V marks.
            this.imgValidName.Visibility = Visibility.Collapsed;

            // Is Use
            this.UseYes.IsChecked = true;
        }

        private void InitializeEvent()
        {
            this.btnCheckName.Click += new RoutedEventHandler(btnCheckName_Click);
            this.btnCancel.Click += new RoutedEventHandler(btnCancel_Click);
            this.btnOK.Click += new RoutedEventHandler(btnOK_Click);

            this.txtName.TextChanged += new TextChangedEventHandler(txtName_TextChanged);
        }

        private bool IsValid()
        {
            // Check ID is valid.
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("R001"), "Information");
                txtName.Focus();
                return false;
            }
            else if (m_bValidID == false)
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("R002"), "Information");
                txtName.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(this.txtStripRate.Text))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("R005"), "Information");
                txtStripRate.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(this.txtUnitRate.Text))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("R006"), "Information");
                txtUnitRate.Focus();
                return false;
            }

            return true;
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.imgValidName.Visibility = Visibility.Collapsed;
            m_bValidID = false;
        }

        private void btnCheckName_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("R001"), "Information");
                txtName.Focus();
                return;
            }

            if (m_RejectManager.SearchByName(txtName.Text).Equals(true))
            {
                // 폐기이름 중복 
                this.imgValidName.Visibility = Visibility.Collapsed;
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("R004"), "Information");
                m_bValidID = false;
            }
            else
            {
                // 폐기이름 비중복
                this.imgValidName.Visibility = Visibility.Visible;
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("E005"), "Information");
                m_bValidID = true;
            }
        }

        #region OK & Cancel click events.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid())
            {
                m_RejectInformation.Name = this.txtName.Text;
                m_RejectInformation.StripDefectRate = Convert.ToDouble(this.txtStripRate.Text);
                m_RejectInformation.UnitDefectRate = Convert.ToDouble(this.txtUnitRate.Text);
                m_RejectInformation.IsUse = this.UseYes.IsChecked.Equals(true) ? true : false;
                m_RejectInformation.RegistrationDate = DateTime.Now;
                m_RejectInformation.RegistrationID = UserManager.CurrentUser.ID;

                this.DialogResult = true;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        #endregion
    }
}
