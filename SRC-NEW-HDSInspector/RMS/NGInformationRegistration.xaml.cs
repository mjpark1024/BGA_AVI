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
using RMS.Generic.NGInformationManagement;
using RMS.Generic.UserManagement;
using Common;

namespace ResultManagement
{
    /// <summary>
    /// Interaction logic for NGInformationRegistration.xaml
    /// </summary>
    public partial class NGInformationRegistration : Window
    {
        public NGInformation m_NGInformation = new NGInformation();
        private NGManager m_NGManger = new NGManager();

        private bool m_bValidID = false;

        public NGInformationRegistration()
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

            m_NGManger.LoadGroup();
            this.cmGroup.ItemsSource = m_NGManger.listGroup;
            this.cmGroup.DisplayMemberPath = "Name";
            this.cmGroup.SelectedValuePath = "Code";
            this.cmGroup.SelectedIndex = 0;

            m_NGManger.LoadInspectType();
            this.cmType.ItemsSource = m_NGManger.listInspectType;
            this.cmType.DisplayMemberPath = "Name";
            this.cmType.SelectedValuePath = "Code";
            this.cmType.SelectedIndex = 0;
        }

        private void InitializeEvent()
        {
            this.btnCheckName.Click += new RoutedEventHandler(btnCheckName_Click);
            this.btnCancel.Click += new RoutedEventHandler(btnCancel_Click);
            this.btnOK.Click += new RoutedEventHandler(btnOK_Click);

            this.txtName.TextChanged += new TextChangedEventHandler(txtName_TextChanged);
            this.cmGroup.SelectionChanged += new SelectionChangedEventHandler(cmGroup_SelectionChanged);
            this.cmType.SelectionChanged += new SelectionChangedEventHandler(cmType_SelectionChanged);
        }

        private void cmType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.imgValidName.Visibility = Visibility.Collapsed;
            m_bValidID = false;
        }

        private void cmGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.imgValidName.Visibility = Visibility.Collapsed;
            m_bValidID = false;
        }

        private bool IsValid()
        {
            // Check ID is valid.
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("N001"), "Information");
                txtName.Focus();
                return false;
            }
            else if (m_bValidID == false)
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("N002"), "Information");
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
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("N001"), "Information");
                txtName.Focus();
                return;
            }

            if (m_NGManger.SearchByName(txtName.Text, this.cmGroup.SelectedValue.ToString()).Equals(true))
            {
                // Name 중복
                this.imgValidName.Visibility = Visibility.Collapsed;
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("N004"), "Information");
                m_bValidID = false;
            }
            else
            {
                // Name 비중복
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
                m_NGInformation.Name = this.txtName.Text;
                m_NGInformation.Group.Code = this.cmGroup.SelectedValue.ToString();
                m_NGInformation.Group.Name = this.cmGroup.Text;
                m_NGInformation.Type.Code = this.cmType.SelectedValue.ToString();
                m_NGInformation.Type.Name = this.cmType.Text;
                m_NGInformation.IsAutoNG = this.AutoNGYes.IsChecked.Equals(true) ? true : false;
                m_NGInformation.IsUse = this.UseYes.IsChecked.Equals(true) ? true : false;
                m_NGInformation.RegistrationDate = DateTime.Now;
                m_NGInformation.RegistrationID = UserManager.CurrentUser.ID;

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
