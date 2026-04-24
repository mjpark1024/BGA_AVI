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
//
using RMS.Generic.NGInformationManagement;
using Common;
using RMS.Generic;
using RMS.Generic.InspectManagement;

namespace ResultManagement
{
    /// <summary>
    /// Interaction logic for NGInformationManagement.xaml
    /// </summary>
    public partial class NGInformationManagement : UserControl
    {
        NGManager m_NGInformationManager = new NGManager();

        public NGInformationManagement()
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
            this.btnSearch.Click += new RoutedEventHandler(btnSearch_Click);
            this.btnSearch.MouseEnter += new MouseEventHandler(btn_MouseEnter);
            this.btnSearch.MouseLeave += new MouseEventHandler(btn_MouseLeave);
            this.dgNGInformationTable.MouseDoubleClick += new MouseButtonEventHandler(dgNGInformationTable_MouseDoubleClick);

            this.txtNameSearch.GotFocus += new RoutedEventHandler(txtNameSearch_GotFocus);
            this.txtNameSearch.LostFocus += new RoutedEventHandler(txtNameSearch_LostFocus);

            this.KeyDown += new KeyEventHandler(NGInformationManagement_KeyDown);
            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(NGInformationManagement_IsVisibleChanged);
        }

        void NGInformationManagement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            m_NGInformationManager.LoadGroup_SearchOption();
            m_NGInformationManager.LoadInspectType_SearchOption();

            this.cmGroupSearch.SelectedIndex = 0;
            this.cmTypeSearch.SelectedIndex = 0;
        }
        
        void NGInformationManagement_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnSearch_Click(btnSearch, null);
            }
        }

        private void InitializeDialog()
        {
            this.dgNGInformationTable.ItemsSource = m_NGInformationManager.NGInformations;

            this.cmGroupSearch.ItemsSource = m_NGInformationManager.listGroup;
            this.cmGroupSearch.DisplayMemberPath = "Name";
            this.cmGroupSearch.SelectedValuePath = "Code";
            this.cmGroupSearch.SelectedIndex = 0;

            this.cmTypeSearch.ItemsSource = m_NGInformationManager.listInspectType;
            this.cmTypeSearch.DisplayMemberPath = "Name";
            this.cmTypeSearch.SelectedValuePath = "Code";
            this.cmTypeSearch.SelectedIndex = 0;
        }

        void txtNameSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtNameSearch.Text))
            {
                this.txtNameSearch.Foreground = new SolidColorBrush(Colors.Gray);
                this.txtNameSearch.Text = "전체 검색";
            }
        }

        void txtNameSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.txtNameSearch.Text.Equals("전체 검색"))
            {
                this.txtNameSearch.Clear();
            }
            this.txtNameSearch.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            BaseCode GroupInfo = (BaseCode)this.cmGroupSearch.SelectedItem;
            InspectTypeInformation InspType = (InspectTypeInformation)this.cmTypeSearch.SelectedItem;

            //Group _All, InspectionType _All, Name _All
            if (GroupInfo.Name.Equals("전체 검색")
                && InspType.Name.Equals("전체 검색")
                && (this.txtNameSearch.Text.Equals("전체 검색") || string.IsNullOrEmpty(this.txtNameSearch.Text)))
            {
                m_NGInformationManager.LoadNGInformation();
            }
            //Group Select, InspectionType _All, Name _All
            else if (!GroupInfo.Name.Equals("전체 검색")
                && InspType.Name.Equals("전체 검색")
                && (this.txtNameSearch.Text.Equals("전체 검색") || string.IsNullOrEmpty(this.txtNameSearch.Text)))
            {
                m_NGInformationManager.SelectByGroup(GroupInfo.Code);
            }
            //Group _All, InspectionType Select, Name _All
            else if (GroupInfo.Name.Equals("전체 검색")
                && !InspType.Name.Equals("전체 검색")
                && (this.txtNameSearch.Text.Equals("전체 검색") || string.IsNullOrEmpty(this.txtNameSearch.Text)))
            {
                m_NGInformationManager.SelectByInspectionType(InspType.Code);
            }
            //Group _All, InspectionType _All, Name Select
            else if (GroupInfo.Name.Equals("전체 검색")
                && InspType.Name.Equals("전체 검색")
                && !(this.txtNameSearch.Text.Equals("전체 검색") || string.IsNullOrEmpty(this.txtNameSearch.Text)))
            {
                m_NGInformationManager.SelectByName(this.txtNameSearch.Text);
            }
            //Group Select, InspectionType Select, Name _All
            else if (!GroupInfo.Name.Equals("전체 검색")
                && !InspType.Name.Equals("전체 검색")
                && (this.txtNameSearch.Text.Equals("전체 검색") || string.IsNullOrEmpty(this.txtNameSearch.Text)))
            {
                m_NGInformationManager.SelectByGroup_InspectionType(GroupInfo.Code, InspType.Code);
            }
            //Group Select, InspectionType _All, Name Select
            else if (!GroupInfo.Name.Equals("전체 검색")
                && InspType.Name.Equals("전체 검색")
                && !(this.txtNameSearch.Text.Equals("전체 검색") || string.IsNullOrEmpty(this.txtNameSearch.Text)))
            {
                m_NGInformationManager.SelectByGroup_Name(GroupInfo.Code, this.txtNameSearch.Text);
            }
            //Group _All, InspectionType Select, Name Select
            else if (GroupInfo.Name.Equals("전체 검색")
                && !InspType.Name.Equals("전체 검색")
                && !(this.txtNameSearch.Text.Equals("전체 검색") || string.IsNullOrEmpty(this.txtNameSearch.Text)))
            {
                m_NGInformationManager.SelectByInspectionType_Name(InspType.Code, this.txtNameSearch.Text);
            }
            //Group Select, InspectionType Select, Name Select
            else if (!GroupInfo.Name.Equals("전체 검색")
                && !InspType.Name.Equals("전체 검색")
                && !(this.txtNameSearch.Text.Equals("전체 검색") || string.IsNullOrEmpty(this.txtNameSearch.Text)))
            {
                m_NGInformationManager.SelectByGroup_InspectionType_Name(GroupInfo.Code, InspType.Code, this.txtNameSearch.Text);
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

        private void dgNGInformationTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnModify_Click(sender, null);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgNGInformationTable.SelectedIndex < 0)
            {
                return;
            }

            if (MessageBox.Show(ResourceStringHelper.GetInformationMessage("U007"), "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<NGInformation> NGInforList = new List<NGInformation>();
                foreach (NGInformation n in this.dgNGInformationTable.SelectedItems)
                {
                    NGInforList.Add(n);
                }
                foreach (NGInformation n in NGInforList)
                {
                    m_NGInformationManager.DeleteNGInformation(n);
                }
            }
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgNGInformationTable.SelectedIndex < 0)
            {
                return;
            }

            NGInformation NGInfo = this.dgNGInformationTable.SelectedItem as NGInformation;
            if (NGInfo != null)
            {
                NGInformationModification NGModification = new NGInformationModification(NGInfo);
                NGModification.Owner = Application.Current.MainWindow;
                if (NGModification.ShowDialog() == true)
                {
                    m_NGInformationManager.ModifyNGInformation(NGModification.m_NGInformation);
                }
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            NGInformationRegistration NGRegistration = new NGInformationRegistration();
            NGRegistration.Owner = Application.Current.MainWindow;
            if (NGRegistration.ShowDialog() == true)
            {
                m_NGInformationManager.AddNGInformation(NGRegistration.m_NGInformation);
            }
        }
    }
}
