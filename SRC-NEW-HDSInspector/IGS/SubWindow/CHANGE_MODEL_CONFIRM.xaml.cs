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
    /// CHANGE_MODEL_CONFIRM.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CHANGE_MODEL_CONFIRM : Window
    {
        public int nMode;           //0: ok, 1: no

        public CHANGE_MODEL_CONFIRM()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.bdTitle.PreviewMouseLeftButtonDown += BdTitle_PreviewMouseLeftButtonDown;
            this.btnOK.Click += BtnOK_Click;
            this.btnNo.Click += BtnNo_Click;
        }

        private void BdTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            nMode = 0;
            this.DialogResult = true;
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            nMode = 1;
            this.DialogResult = true;
        }
    }
}
