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
    /// SUSPEND_CONFIRM_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SUSPEND_CONFIRM_Window : Window
    {
        public int nMode;       //0: 유지, 1:변경

        public SUSPEND_CONFIRM_Window()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.bdTitle.PreviewMouseLeftButtonDown += BdTitle_PreviewMouseLeftButtonDown;
            this.btnSame.Click += BtnSame_Click;
            this.btnChange.Click += BtnChange_Click;
            this.btnCancel.Click += BtnCancel_Click;
        }

        private void BdTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void BtnSame_Click(object sender, RoutedEventArgs e)
        {
            nMode = 0;
            this.DialogResult = true;
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            nMode = 1;
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
