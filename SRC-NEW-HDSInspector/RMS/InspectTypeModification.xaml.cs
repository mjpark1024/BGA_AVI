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
using RMS.Generic.InspectManagement;
using Common;
using RMS.Generic.UserManagement;

namespace ResultManagement
{
    /// <summary>
    /// Interaction logic for InspectTypeModification.xaml
    /// </summary>
    public partial class InspectTypeModification : Window
    {
        public InspectTypeInformation m_InspectTypeInformation;
        public InspectTypeManager m_InspectTypeManager = new InspectTypeManager();
        private bool m_bValidID = true;

        public InspectTypeModification(InspectTypeInformation aInspectionType)
        {
            InitializeComponent();
            InitializeDialog(aInspectionType);
            InitializeEvent();
        }

        private void InitializeDialog(InspectTypeInformation aInspectionType)
        {
            this.m_InspectTypeInformation = aInspectionType;

            //Inspection Name Setting
            this.txtName.Text = m_InspectTypeInformation.Name;  

            //CheckBox Setting
            if (m_InspectTypeInformation.IsUse.Equals(true)) { this.UseYes.IsChecked = true; }
            else { this.UseNo.IsChecked = true; }

            // V marks.
            this.imgValidName.Visibility = Visibility.Visible;
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
            // Check Name is valid.
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("I001"), "Information");
                txtName.Focus();
                return false;
            }
            else if (m_bValidID == false)
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("I002"), "Information");
                txtName.Focus();
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
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("I001"), "Information");
                txtName.Focus();
                return;
            }

            if (m_bValidID.Equals(false))
            {
                if (m_InspectTypeManager.SearchByName(txtName.Text).Equals(true))
                {
                    // 검사이름 중복 
                    this.imgValidName.Visibility = Visibility.Collapsed;
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("I004"), "Information");
                    m_bValidID = false;
                }
                else
                {
                    // 검사이름 비중복
                    this.imgValidName.Visibility = Visibility.Visible;
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("I005"), "Information");
                    m_bValidID = true;
                }
            }
        }

        #region OK & Cancel click events.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid())
            {
                m_InspectTypeInformation.Name = this.txtName.Text;
                m_InspectTypeInformation.IsUse = this.UseYes.IsChecked.Equals(true) ? true : false;
                m_InspectTypeInformation.RegistrationDate = DateTime.Now;
                m_InspectTypeInformation.RegistrationID = UserManager.CurrentUser.ID;

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
