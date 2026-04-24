using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IGS.SubWindow
{
    /// <summary>
    /// CODE_SEARCH_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CODE_SEARCH_Window : Window
    {
        private List<string> orgList;
        public string selectedCode;

        public CODE_SEARCH_Window()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.btnOK.Click += BtnOK_Click;
            this.btnCancel.Click += BtnCancel_Click;

            this.tboxCode.TextChanged += TboxCode_TextChanged;
            this.tboxName.TextChanged += TboxName_TextChanged;

            this.bdTitle.PreviewMouseLeftButtonDown += BdTitle_PreviewMouseLeftButtonDown;
            this.lbInfo.PreviewMouseWheel += LbInfo_PreviewMouseWheel;
        }

        public void InitializeDialog(List<string> codeList)
        {
            orgList = new List<string>();
            foreach (string code in codeList)
                orgList.Add(code);

            lbInfo.DataContext = codeList;

            if (codeList.Count > 0)
                lbInfo.SelectedIndex = 0;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (lbInfo.SelectedItem == null)
            {
                MessageBox.Show("선택된 코드가 없습니다.");
                return;
            }

            selectedCode = lbInfo.SelectedItem.ToString();
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void TboxCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<string> searchList = new List<string>();

            if (tboxCode.Text == "")
            {
                lbInfo.DataContext = orgList;
                lbInfo.SelectedIndex = -1;
            }
            else
            {
                foreach (string code in orgList)
                {
                    if (code.StartsWith(tboxCode.Text.ToUpper()))
                        searchList.Add(code);
                }

                lbInfo.DataContext = searchList;
            }
        }

        private void TboxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<string> searchList = new List<string>();

            if (tboxName.Text == "")
            {
                lbInfo.DataContext = orgList;
                lbInfo.SelectedIndex = -1;
            }
            else
            {
                foreach (string code in orgList)
                {
                    if (code.ToLower().Contains(tboxName.Text.ToLower()))
                        searchList.Add(code);
                }

                lbInfo.DataContext = searchList;
            }
        }

        private void BdTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void LbInfo_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroll = (ScrollViewer)svInfo;
            if (e.Delta < 0)
            {
                if (scroll.VerticalOffset - e.Delta < scroll.ExtentHeight - scroll.ViewportHeight)
                    scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta);
                else
                    scroll.ScrollToBottom();
            }
            else
            {
                if (scroll.VerticalOffset + e.Delta > 0)
                    scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta);
                else
                    scroll.ScrollToTop();
            }
            e.Handled = true;
        }
    }
}
