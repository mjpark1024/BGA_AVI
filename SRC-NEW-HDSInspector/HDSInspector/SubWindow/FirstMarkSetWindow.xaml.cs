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
    /// FirstMarkSetWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FirstMarkSetWindow : Window
    {
        public double FMX;
        public double FMY;
        public FirstMarkSetWindow()
        {
            InitializeComponent();
            this.btnSave.Click += btnSave_Click;
            this.btnClose.Click += btnClose_Click;
        }

        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        public void SetValue(double fmx, double fmy, double ofsx, double ofsy)
        {
            lblFMX.Content = fmx.ToString();
            lblFMY.Content = fmy.ToString();
            lblOSX.Content = ofsx.ToString("0.000");
            lblOSY.Content = ofsy.ToString("0.000");
            FMX = fmx + ofsx;
            FMY = fmy + ofsy;
            lblCMX.Content = FMX.ToString("0.000");
            lblCMY.Content = FMY.ToString("0.000");
            FMX = Convert.ToDouble(lblCMX.Content);
            FMY = Convert.ToDouble(lblCMY.Content);
        }
    }
}

