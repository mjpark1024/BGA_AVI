using IGS.Classes;
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
    /// WARNING_POPUP_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WARNING_POPUP_Window : Window
    {
        public WARNING_POPUP_Window()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.btnOK.Click += BtnOK_Click;
        }

        public void InitializeDialog(POP_WARNING_INFO warning, List<POP_CHANGE_POINT_OUTPUT_DATA> changes)
        {
            if (warning != null)
                warningCtrl.InitializeDialog(warning);

            if (changes != null && changes.Count > 0)
                changeCtrl.InitializeDialog(changes);
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

    }
}
