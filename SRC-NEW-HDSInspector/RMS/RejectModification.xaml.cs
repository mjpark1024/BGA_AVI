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
    /// Interaction logic for RejectModification.xaml
    /// </summary>
    public partial class RejectModification : Window
    {
        public RejectInformation m_RejectInformation;
        private RejectManager m_RejectManager = new RejectManager();
        private bool m_bValidID = true;

        public RejectModification(RejectInformation aRejectInfo)
        {
            InitializeComponent();
            InitializeDialog(aRejectInfo);
            InitializeEvent();
        }

        private void InitializeDialog(RejectInformation aRejectInfo)
        {
            this.m_RejectInformation = aRejectInfo;

            this.txtName.Text = m_RejectInformation.Name;
            this.txtStripRate.Text = m_RejectInformation.StripDefectRate.ToString();
            this.txtUnitRate.Text = m_RejectInformation.UnitDefectRate.ToString();

            //CheckBox Setting
            if (m_RejectInformation.IsUse.Equals(true)) { this.UseYes.IsChecked = true; }
            else { this.UseNo.IsChecked = true; }

            // V marks.
            this.imgValidName.Visibility = Visibility.Visible;
        }

        private void InitializeEvent()
        {
            this.btnCheckName.Click += new RoutedEventHandler(btnCheckName_Click);
            this.btnOK.Click += new RoutedEventHandler(btnOK_Click);
            this.btnCancel.Click += new RoutedEventHandler(btnCancel_Click);

            this.txtName.TextChanged += new TextChangedEventHandler(txtName_TextChanged);
        }

        void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtName.Text.Equals(m_RejectInformation.Name))
            {
                this.imgValidName.Visibility = Visibility.Visible;
                m_bValidID = true;
            }
            else
            {
                this.imgValidName.Visibility = Visibility.Collapsed;
                m_bValidID = false;
            }
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

            return true;
        }

        private void btnCheckName_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("R001"), "Information");
                txtName.Focus();
                return;
            }

            if(m_bValidID.Equals(false))
            {
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
        }

        #region OK & Cancel click events.
        void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid())
            {
                //데이타 셋팅
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

        void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        #endregion
    }
}
