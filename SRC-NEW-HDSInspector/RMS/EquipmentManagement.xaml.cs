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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;
using RMS.Generic.EquipmentManagement;

namespace ResultManagement
{
    /// <summary>   Interaction logic for EquipmentManagement.xaml. </summary>
    public partial class EquipmentManagement : UserControl
    {
        /// <summary> Manager for equipment </summary>
        EquipmentManager m_EquipmentManager = new EquipmentManager();

        /// <summary>   Initializes a new instance of the EquipmentManagement class. </summary>
        public EquipmentManagement()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }

        /// <summary>   Initializes the dialog. </summary>
        private void InitializeDialog()
        {
            this.dgEquipmentTable.ItemsSource = m_EquipmentManager.Equipments;
            this.cmSearchOption.ItemsSource = m_EquipmentManager.listEquipmentType;
            this.cmSearchOption.DisplayMemberPath = "TypeName";
            this.cmSearchOption.SelectedValuePath = "TypeCode";
            this.cmSearchOption.SelectedIndex = 0;
        }

        /// <summary>   Initializes the event. </summary>
        private void InitializeEvent()
        {
            this.btnNew.Click += new RoutedEventHandler(btnNew_Click);
            this.btnModify.Click += new RoutedEventHandler(btnModify_Click);
            this.btnDelete.Click += new RoutedEventHandler(btnDelete_Click);            
            this.btnSearch.Click += new RoutedEventHandler(btnSearch_Click);
            this.btnSearch.MouseEnter += new MouseEventHandler(btn_MouseEnter);
            this.btnSearch.MouseLeave += new MouseEventHandler(btn_MouseLeave);
            this.dgEquipmentTable.MouseDoubleClick += new MouseButtonEventHandler(dgUserTable_MouseDoubleClick);

            this.txtNameSearch.GotFocus += new RoutedEventHandler(txtSearch_GotFocus);
            this.txtNameSearch.LostFocus += new RoutedEventHandler(txtSearch_LostFocus);

            this.KeyDown += new KeyEventHandler(EquipmentManagement_KeyDown);
            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(EquipmentManagement_IsVisibleChanged);
        }

        /// <summary>   Event handler. Called by EquipmentManagement for is visible changed events. </summary>
        private void EquipmentManagement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            m_EquipmentManager.LoadEquipmentType_SearchOption();
            this.cmSearchOption.SelectedIndex = 0;
        }

        /// <summary>   Event handler. Called by txtSearch for lost focus events. </summary>
        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtNameSearch.Text))
            {
                this.txtNameSearch.Foreground = new SolidColorBrush(Colors.Gray);
                this.txtNameSearch.Text = "전체 검색";
            }
        }

        /// <summary>   Event handler. Called by EquipmentManagement for key down events. </summary>
        private void EquipmentManagement_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnSearch_Click(btnSearch, null);
            }
        }

        /// <summary>   Event handler. Called by txtSearch for got focus events. </summary>
        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.txtNameSearch.Text.Equals("전체 검색"))
            {
                this.txtNameSearch.Clear();
            }
            this.txtNameSearch.Foreground = new SolidColorBrush(Colors.Black);
        }

        /// <summary>   Event handler. Called by btnSearch for click events. </summary>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            EquipmentType equipType = (EquipmentType)this.cmSearchOption.SelectedItem;

            //Type All, Name All
            if (equipType.TypeName.Equals("전체 검색")
                && (this.txtNameSearch.Text.Equals("전체 검색") || string.IsNullOrEmpty(this.txtNameSearch.Text)))
            {
                m_EquipmentManager.LoadEquipment();
            }
            //Type Select, Name All
            else if (!equipType.TypeName.Equals("전체 검색")
                && (this.txtNameSearch.Text.Equals("전체 검색") || string.IsNullOrEmpty(this.txtNameSearch.Text)))
            {
                m_EquipmentManager.SelectEquipByType(equipType.TypeCode);
            }
            //Type All, Name Select
            else if (equipType.TypeName.Equals("전체 검색")
                && !(this.txtNameSearch.Text.Equals("전체 검색") || string.IsNullOrEmpty(this.txtNameSearch.Text)))
            {
                m_EquipmentManager.SelectEquipByName(this.txtNameSearch.Text);
            }
            //Type Select, Name Select
            else if (!equipType.TypeName.Equals("전체 검색")
                && !(this.txtNameSearch.Text.Equals("전체 검색") || string.IsNullOrEmpty(this.txtNameSearch.Text)))
            {
                m_EquipmentManager.SelectEquipByName_Type(equipType.TypeCode, this.txtNameSearch.Text);
            }
        }

        /// <summary>   Event handler. Called by btn for mouse leave events. </summary>
        private void btn_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>   Event handler. Called by btn for mouse enter events. </summary>
        private void btn_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        /// <summary>   Event handler. Called by dgUserTable for mouse double click events. </summary>
        private void dgUserTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnModify_Click(sender, null);
        }

        /// <summary>   Event handler. Called by btnNew for click events. </summary>
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            EquipmentRegistration equipRegistration = new EquipmentRegistration();
            equipRegistration.Owner = Application.Current.MainWindow;
            if (equipRegistration.ShowDialog() == true)
            {
                m_EquipmentManager.AddEquipment(equipRegistration.m_EquipmentInformation);
            }
        }

        /// <summary>   Event handler. Called by btnModify for click events. </summary>
        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgEquipmentTable.SelectedIndex < 0)
            {
                return;
            }

            EquipmentInformation equipInfo = this.dgEquipmentTable.SelectedItem as EquipmentInformation;
            if (equipInfo != null)
            {
                EquipmentModification equipmentModification = new EquipmentModification(equipInfo);
                equipmentModification.Owner = Application.Current.MainWindow;
                if (equipmentModification.ShowDialog() == true)
                {
                    m_EquipmentManager.ModifyEquipment(equipInfo);
                }
            }
        }

        /// <summary>   Event handler. Called by btnDelete for click events. </summary>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgEquipmentTable.SelectedIndex < 0)
            {
                return;
            }

            if (MessageBox.Show(ResourceStringHelper.GetInformationMessage("E003"), "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<EquipmentInformation> equipmentList = new List<EquipmentInformation>();
                foreach (EquipmentInformation equip in this.dgEquipmentTable.SelectedItems)
                {
                    equipmentList.Add(equip);
                }
                foreach (EquipmentInformation equip in equipmentList)
                {
                    m_EquipmentManager.DeleteEquipment(equip);
                }
            }
        }
    }
}
