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
    /// PASSWORD_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PASSWORD_Window : Window
    {
        public PASSWORD_Window()
        {
            InitializeComponent();
            InitializeEvent();

            txtPW.Focus();
        }

        private void InitializeEvent()
        {
            this.btnOK.Click += BtnOK_Click;
            this.btnCancel.Click += BtnCancel_Click;

            this.KeyUp += PASSWORD_Window_KeyUp;
        }

        private void PASSWORD_Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BtnOK_Click(null, null);
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (txtPW.Password == "")
                return;

            if (txtPW.Password == "hds")
                this.DialogResult = true;
            else
            {
                MessageBox.Show("패스워드를 잘못 입력하였습니다.");
                txtPW.Password = "";
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
