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
using RMS.Generic.RejectManagement;
using Common;

namespace ResultManagement
{
    /// <summary>
    /// Interaction logic for DiscardManagement.xaml
    /// </summary>
    public partial class RejectManagement : UserControl
    {
        RejectManager m_RejectManager = new RejectManager();

        public RejectManagement()
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
            this.btnSearch.Click +=new RoutedEventHandler(btnSearch_Click);
            this.dgRejectTable.MouseDoubleClick += new MouseButtonEventHandler(dgRejectTable_MouseDoubleClick);

            this.txtSearch.GotFocus += new RoutedEventHandler(txtSearch_GotFocus);
            this.txtSearch.LostFocus += new RoutedEventHandler(txtSearch_LostFocus);
            this.btnSearch.Click += new RoutedEventHandler(btnSearch_Click);
            this.btnSearch.MouseEnter += new MouseEventHandler(btn_MouseEnter);
            this.btnSearch.MouseLeave += new MouseEventHandler(btn_MouseLeave);

            this.KeyDown += new KeyEventHandler(RejectManagement_KeyDown);
        }

        void RejectManagement_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                btnSearch_Click(btnSearch, null);
            }
        }

        private void InitializeDialog()
        {
            this.dgRejectTable.ItemsSource = m_RejectManager.Rejects;

            this.cmSearchOption.Items.Add("폐기이름");
            this.cmSearchOption.SelectedIndex = 0;

            //#region Test Code
            //RejectInformation test = new RejectInformation();
            //test.Name = "EAV001";
            //test.Type = "ELF AVI";
            //m_RejectManager.AddReject(test);
            //#endregion
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
            if(this.txtSearch.Text.Equals("전체 검색") || string.IsNullOrEmpty(this.txtSearch.Text))
            {
                m_RejectManager.LoadReject();
            }
            else
            {
                m_RejectManager.SelectRejectByName(this.txtSearch.Text);
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

        private void dgRejectTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnModify_Click(sender, null);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgRejectTable.SelectedIndex < 0)
            {
                return;
            }
            //Yes Or No
            if (MessageBox.Show(ResourceStringHelper.GetInformationMessage("R003"), "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<RejectInformation> rejectList = new List<RejectInformation>();
                foreach (RejectInformation r in this.dgRejectTable.SelectedItems)
                {
                    rejectList.Add(r);
                }
                foreach (RejectInformation r in rejectList)
                {
                    m_RejectManager.DeleteReject(r);
                }
            }
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgRejectTable.SelectedIndex < 0)
            {
                return;
            }

            RejectInformation rejectInfo = this.dgRejectTable.SelectedItem as RejectInformation;
            if(rejectInfo != null)
            {
                RejectModification rejectModification = new RejectModification(rejectInfo);
                rejectModification.Owner = Application.Current.MainWindow;
                if (rejectModification.ShowDialog() == true)
                {
                    m_RejectManager.ModifyReject(rejectInfo);
                }
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            RejectRegistration rejectRegistration = new RejectRegistration();
            rejectRegistration.Owner = Application.Current.MainWindow;
            if (rejectRegistration.ShowDialog() == true)
            {
                m_RejectManager.AddReject(rejectRegistration.m_RejectInformation);
            }
        }
    }
}
