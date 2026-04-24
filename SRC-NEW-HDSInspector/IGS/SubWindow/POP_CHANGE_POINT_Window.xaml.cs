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
using IGS.Classes;

namespace IGS.SubWindow
{
    /// <summary>
    /// POP_CHANGE_POINT_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class POP_CHANGE_POINT_Window : Window
    {
        public POP_CHANGE_POINT_Window(List<POP_CHANGE_POINT_OUTPUT_DATA> points)
        {
            InitializeComponent();
            InitializeEvent();

            changeCtrl.InitializeDialog(points);
        }

        private void InitializeEvent()
        {
            this.btnOK.Click += BtnOK_Click;
            this.bdTitle.PreviewMouseLeftButtonDown += BdTitle_PreviewMouseLeftButtonDown;
        }

        private void BdTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
