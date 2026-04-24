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

namespace HDSInspector.SubWindow
{
    /// <summary>
    /// Password.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Password : Window
    {
        public Password()
        {
            InitializeComponent();
            txtPW.Focus();
            this.KeyDown += new KeyEventHandler(PitchSetWindow_KeyDown);
        }
        void PitchSetWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                CloseWithOK();
            }
            else if (e.Key == Key.Escape)
            {
                CloseWithCancel();
            }
        }

        private void CloseWithOK()
        {
            if (txtPW.Password.ToString() == "hds" || txtPW.Password.ToString() == "HDS")
            {
                DialogResult = true;
            }
            else
            {
                txtPW.Password = "";
                txtPW.Focus();
            }
        }
        private void CloseWithCancel()
        {
            if (DialogResult == null || !(bool)DialogResult)
            {
                DialogResult = false;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWithCancel();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            CloseWithOK();
        }
    }
}
