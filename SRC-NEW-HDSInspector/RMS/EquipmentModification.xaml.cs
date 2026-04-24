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
using Common;

namespace ResultManagement
{
    /// <summary>   Equipment modification.  </summary>
    public partial class EquipmentModification : Window
    {
        public EquipmentInformation m_EquipmentInformation;
        private EquipmentManager m_EquipmentManger = new EquipmentManager();
        private bool m_bValidID = true;

        public EquipmentModification(EquipmentInformation aEquipInfo)
        {
            InitializeComponent();
            InitializeDialog(aEquipInfo);
            InitializeEvent();
        }

        private void InitializeDialog(EquipmentInformation aEquipInfo)
        {
            m_EquipmentManger.LoadEquipmentType();
            this.cmType.ItemsSource = m_EquipmentManger.listEquipmentType;
            this.cmType.DisplayMemberPath = "TypeName";
            this.cmType.SelectedValuePath = "TypeCode"; 
            
            this.m_EquipmentInformation = aEquipInfo;
            this.txtName.Text = m_EquipmentInformation.Name;  

            // ComboBox Setting
            int selectedIndex = 0;
            foreach(EquipmentType obj in cmType.Items.SourceCollection)
            {
                if(m_EquipmentInformation.EquipmentType.TypeCode == obj.TypeCode)
                {
                    break;
                }
                selectedIndex++;
            }
            cmType.SelectedIndex = selectedIndex;

            // CheckBox Setting
            if (m_EquipmentInformation.IsUse.Equals(true))
            {
                this.UseYes.IsChecked = true;
            }
            else
            {
                this.UseNo.IsChecked = true;
            }            

            // V marks.
            this.imgValidName.Visibility = Visibility.Visible;
        }

        /// <summary>   Initializes the event. </summary>
        private void InitializeEvent()
        {
            this.btnCheckName.Click += new RoutedEventHandler(btnCheckName_Click);
            this.btnOK.Click += new RoutedEventHandler(btnOK_Click);
            this.btnCancel.Click += new RoutedEventHandler(btnCancel_Click);
            this.txtName.TextChanged += new TextChangedEventHandler(txtName_TextChanged);
        }

        /// <summary>   Event handler. Called by txtName for text changed events. </summary>
        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtName.Text.Equals(m_EquipmentInformation.Name))
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

        /// <summary>   Query if this object is valid. </summary>
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

            return true; // Valid id.
        }

        /// <summary>   Event handler. Called by btnCheckName for click events. </summary>
        private void btnCheckName_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("E001"), "Information");
                txtName.Focus();
                return;
            }

            if (m_bValidID.Equals(false))
            {
                if (m_EquipmentManger.SearchByName(txtName.Text, this.cmType.SelectedValue.ToString()).Equals(true)) // 장비이름 중복
                {
                    this.imgValidName.Visibility = Visibility.Collapsed;
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("E004"), "Information");
                    m_bValidID = false;
                }
                else
                {
                    this.imgValidName.Visibility = Visibility.Visible;
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("E005"), "Information");
                    m_bValidID = true;
                }
            }
        }

        #region OK & Cancel click events.
        /// <summary>   Event handler. Called by btnOK for click events. </summary>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid())
            {
                // 데이타 셋팅
                m_EquipmentInformation.Name = this.txtName.Text;
                m_EquipmentInformation.EquipmentType.TypeCode = cmType.SelectedValue.ToString();
                m_EquipmentInformation.EquipmentType.TypeName = cmType.Text;          
                m_EquipmentInformation.IsUse = this.UseYes.IsChecked.Equals(true) ? true : false;

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
