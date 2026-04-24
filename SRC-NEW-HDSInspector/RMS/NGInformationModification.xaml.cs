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
using RMS.Generic.InspectManagement;
using RMS.Generic;
using RMS.Generic.UserManagement;

using Common;

namespace ResultManagement
{
    /// <summary>
    /// Interaction logic for NGInformationModification.xaml
    /// </summary>
    public partial class NGInformationModification : Window
    {
        public NGInformation m_NGInformation;
        private NGManager m_NGManger = new NGManager();
        private bool m_bValidID = true;

        public NGInformationModification(NGInformation aNGInfo)
        {
            InitializeComponent();
            InitializeDialog(aNGInfo);
            InitializeEvent();
        }

        private void InitializeDialog(NGInformation aNGInfo)
        {
            m_NGInformation = aNGInfo;
                        
            ////불량그룹 셋팅
            //this.cmGroup.Items.Add("불량 1그룹");
            //this.cmGroup.Items.Add("불량 2그룹");
            //this.cmGroup.SelectedIndex = 0;

            ////불량타입 셋팅
            //this.cmType.Items.Add("검사 1타입");
            //this.cmType.Items.Add("검사 2타입");
            //this.cmType.SelectedIndex = 0;


            m_NGManger.LoadGroup();
            this.cmGroup.ItemsSource = m_NGManger.listGroup;
            this.cmGroup.DisplayMemberPath = "Name";
            this.cmGroup.SelectedValuePath = "Code";


            m_NGManger.LoadInspectType();
            this.cmType.ItemsSource = m_NGManger.listInspectType;
            this.cmType.DisplayMemberPath = "Name";
            this.cmType.SelectedValuePath = "Code"; 


            //Name Setting
            this.txtName.Text = m_NGInformation.Name;
            //comboBox Setting
            int selectedIndex = 0;
            foreach (BaseCode obj in cmGroup.Items.SourceCollection)
            {
                if (m_NGInformation.Group.Code == obj.Code)
                {
                    break;
                }
                selectedIndex++;
            }
            cmGroup.SelectedIndex = selectedIndex;

            selectedIndex = 0;
            foreach (InspectTypeInformation obj in cmType.Items.SourceCollection)
            {
                if (m_NGInformation.Type.Code == obj.Code)
                {
                    break;
                }
                selectedIndex++;
            }
            cmType.SelectedIndex = selectedIndex;

            //CheckBox Setting
            if (m_NGInformation.IsUse.Equals(true)) { this.UseYes.IsChecked = true; }
            else { this.UseNo.IsChecked = true; }

            if (m_NGInformation.IsAutoNG.Equals(true)) { this.AutoNGYes.IsChecked = true; }
            else { this.AutoNGNo.IsChecked = true; }

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
            if (txtName.Text.Equals(m_NGInformation.Name))
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

        private void btnCheckName_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("N001"), "Information");
                txtName.Focus();
                return;
            }
            if (m_bValidID.Equals(false))
            {
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
        }

        #region OK & Cancel click events.
        void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid())
            {
                //데이타 셋팅
                m_NGInformation.Name = this.txtName.Text;
                m_NGInformation.Group.Code = this.cmGroup.SelectedValue.ToString();
                m_NGInformation.Group.Name = this.cmGroup.Text;
                m_NGInformation.Type.Code = this.cmType.SelectedValue.ToString();
                m_NGInformation.Type.Name = this.cmType.Text;
                m_NGInformation.IsAutoNG = this.AutoNGNo.IsChecked.Equals(true) ? true : false;
                m_NGInformation.IsUse = this.UseYes.IsChecked.Equals(true) ? true : false;
                m_NGInformation.RegistrationDate = DateTime.Now;
                m_NGInformation.RegistrationID = UserManager.CurrentUser.ID;

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
