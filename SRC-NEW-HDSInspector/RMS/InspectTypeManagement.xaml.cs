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
using RMS.Generic.InspectManagement;
using Common;

namespace ResultManagement
{
    /// <summary>
    /// Interaction logic for InspectTypeManagement.xaml
    /// </summary>
    public partial class InspectTypeManagement : UserControl
    {
        InspectTypeManager m_InspectTypeManager = new InspectTypeManager();

        public InspectTypeManagement()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.btnNew.Click += new RoutedEventHandler(btnNew_Click);
            this.btnModify.Click += new RoutedEventHandler(btnModify_Click);
            this.btnDelete.Click += new RoutedEventHandler(btnDelete_Click);

            this.txtSearch.GotFocus += new RoutedEventHandler(txtSearch_GotFocus);
            this.txtSearch.LostFocus += new RoutedEventHandler(txtSearch_LostFocus);
            this.btnSearch.Click += new RoutedEventHandler(btnSearch_Click);
            this.btnSearch.MouseEnter += new MouseEventHandler(btn_MouseEnter);
            this.btnSearch.MouseLeave += new MouseEventHandler(btn_MouseLeave);

            this.dgInspectTypeTable.MouseDoubleClick += new MouseButtonEventHandler(dgUserTable_MouseDoubleClick);
            this.KeyDown += new KeyEventHandler(EquipmentManagement_KeyDown);
        }

        void EquipmentManagement_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnSearch_Click(btnSearch, null);
            }
        }

        private void InitializeDialog()
        {
            this.dgInspectTypeTable.ItemsSource = m_InspectTypeManager.InspectTypeInformation;

            this.cmSearchOption.Items.Add("검사이름");
            this.cmSearchOption.SelectedIndex = 0;
        }

        void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtSearch.Text))
            {
                this.txtSearch.Foreground = new SolidColorBrush(Colors.Gray);
                this.txtSearch.Text = "전체 검색";
            }
        }

        void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.txtSearch.Text.Equals("전체 검색"))
            {
                this.txtSearch.Clear();
            }
            this.txtSearch.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //전체 검색
            if (this.txtSearch.Text.Equals("전체 검색") || string.IsNullOrEmpty(this.txtSearch.Text))
            {
                m_InspectTypeManager.LoadInspectType();
            }
            //검사이름으로 검색
            else 
            {
                m_InspectTypeManager.SelectInspectTypeByName(this.txtSearch.Text);
            }
        }

        private void btn_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void btn_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void dgUserTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnModify_Click(sender, null);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgInspectTypeTable.SelectedIndex < 0)
            {
                return;
            }

            if (MessageBox.Show(ResourceStringHelper.GetInformationMessage("E003"), "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<InspectTypeInformation> inspectTypeList = new List<InspectTypeInformation>();
                foreach (InspectTypeInformation inspType in this.dgInspectTypeTable.SelectedItems)
                {
                    inspectTypeList.Add(inspType);
                }
                foreach (InspectTypeInformation inspType in inspectTypeList)
                {
                    m_InspectTypeManager.DeleteInspectType(inspType);
                }
            }
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgInspectTypeTable.SelectedIndex < 0)
            {
                return;
            }

            InspectTypeInformation inspTypeInfo = this.dgInspectTypeTable.SelectedItem as InspectTypeInformation;

            if (inspTypeInfo != null)
            {
                InspectTypeModification inspTypeModification = new InspectTypeModification(inspTypeInfo);
                inspTypeModification.Owner = Application.Current.MainWindow;
                if (inspTypeModification.ShowDialog() == true)
                {
                    m_InspectTypeManager.ModifyInspectType(inspTypeInfo);
                }
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            InspectTypeRegistration inspTypeRegistration = new InspectTypeRegistration();
            inspTypeRegistration.Owner = Application.Current.MainWindow;
            if (inspTypeRegistration.ShowDialog() == true)
            {
                m_InspectTypeManager.AddInspectType(inspTypeRegistration.m_InspectTypeInformation);
            }
        }
    }
}
