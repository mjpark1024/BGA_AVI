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

namespace HDSInspector.SubWindow
{
    /// <summary>
    /// Interaction logic for QuestionWindow.xaml
    /// </summary>
    public partial class QuestionWindow : Window
    {
        public QuestionWindow()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }

        private void InitializeDialog()
        {
            page1.Visibility = Visibility.Visible;
            page2.Visibility = Visibility.Hidden;
            page3.Visibility = Visibility.Hidden;
        }

        private void InitializeEvent()
        {
            btnPrev.Click += btnPrev_Click;
            btnNext.Click += btnNext_Click;
            KeyDown += QuestionWindow_KeyDown;
        }

        void QuestionWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if(page1.Visibility == Visibility.Visible)
            {
                page1.Visibility = Visibility.Hidden;
                page2.Visibility = Visibility.Visible;
                txtCurrentPage.Text = "2";
            }
            else if (page2.Visibility == Visibility.Visible)
            {
                page2.Visibility = Visibility.Hidden;
                page3.Visibility = Visibility.Visible;
                txtCurrentPage.Text = "3";
            }
        }

        void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (page3.Visibility == Visibility.Visible)
            {
                page3.Visibility = Visibility.Hidden;
                page2.Visibility = Visibility.Visible;
                txtCurrentPage.Text = "2";
            }
            else if (page2.Visibility == Visibility.Visible)
            {
                page2.Visibility = Visibility.Hidden;
                page1.Visibility = Visibility.Visible;
                txtCurrentPage.Text = "1";
            }
        }
    }
}
