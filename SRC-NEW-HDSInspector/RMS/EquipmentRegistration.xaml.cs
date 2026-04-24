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
using RMS.Generic.EquipmentManagement;
using RMS.Generic.UserManagement;
using Common;

namespace ResultManagement
{
    public partial class EquipmentRegistration : Window
    {
        public EquipmentInformation m_EquipmentInformation = new EquipmentInformation();
        private EquipmentManager m_EquipmentManger = new EquipmentManager();
        private bool m_bValidID = false;

        public EquipmentRegistration()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }

        private void InitializeDialog()
        {
            // V marks.
            this.imgValidName.Visibility = Visibility.Collapsed;

            //IsUse
            this.UseYes.IsChecked = true;

            //장비타입 셋팅
            m_EquipmentManger.LoadEquipmentType();
            this.cmType.ItemsSource = m_EquipmentManger.listEquipmentType;
            this.cmType.DisplayMemberPath = "TypeName";
            this.cmType.SelectedValuePath = "TypeCode"; 
            this.cmType.SelectedIndex = 0;
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
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("E001"), "Information");
                txtName.Focus();
                return false;
            }
            else if (m_bValidID == false)
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("E002"), "Information");
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
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("E001"), "Information");
                txtName.Focus();
                return;
            }

            if (m_EquipmentManger.SearchByName(txtName.Text, this.cmType.SelectedValue.ToString()).Equals(true))
            {
                // 장비이름 중복 
                this.imgValidName.Visibility = Visibility.Collapsed;
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("E004"), "Information");
                m_bValidID = false;       
            }
            else
            {
                // 장비이름 비중복
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
                m_EquipmentInformation.Name = this.txtName.Text;
                m_EquipmentInformation.EquipmentType.TypeCode = this.cmType.SelectedValue.ToString();
                m_EquipmentInformation.EquipmentType.TypeName = this.cmType.Text;
                m_EquipmentInformation.IsUse = this.UseYes.IsChecked.Equals(true) ? true : false;
                m_EquipmentInformation.RegistrationDate = DateTime.Now;
                m_EquipmentInformation.RegistrationID = UserManager.CurrentUser.ID;

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
