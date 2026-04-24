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
    /// PasswordWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PasswordWindow : Window
    {
        public PasswordWindow()
        {
            InitializeComponent();
            this.btnOK.Click += btnOK_Click;
            this.btnCancel.Click += btnCancel_Click;
        }

        void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password == "hds" || txtPassword.Password == "HDS")
            {
                this.DialogResult = true;
            }
            else
            {
                txtPassword.Password = "";
                txtPassword.Focus();
            }
        }
    }
}
