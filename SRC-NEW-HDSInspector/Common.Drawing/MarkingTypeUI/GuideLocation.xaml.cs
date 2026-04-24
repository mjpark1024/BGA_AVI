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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Common.Drawing.MarkingTypeUI
{
    /// <summary>
    /// GuideLocation.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GuideLocation : UserControl
    {
        public event MoveButtonClick MoveClick;
        public event SetButtonClick SetClick;
        public GuideLocation()
        {
            InitializeComponent();
            this.btnLeft.Click += btnMove_Click;
            this.btnRight.Click += btnMove_Click;
            this.btnUp.Click += btnMove_Click;
            this.btnDown.Click += btnMove_Click;
            this.btnLoaction.Click += btnLoaction_Click;
        }
        void btnLoaction_Click(object sender, RoutedEventArgs e)
        {
            SetButtonClick er = SetClick;
            if (er != null) er();
        }

        void btnMove_Click(object sender, RoutedEventArgs e)
        {
            MoveButtonClick er = MoveClick;
            double pitch = Convert.ToDouble(txtPitch.Text);
            if (er != null)
            {
                switch ((((Button)sender).Tag).ToString())
                {
                    case "left": er(0, pitch); break;
                    case "right": er(1, pitch); break;
                    case "up": er(2, pitch); break;
                    case "down": er(3, pitch); break;
                }
            }
        }

        public void Set(double X, double Y)
        {
            txtX.Text = X.ToString("f3");
            txtY.Text = Y.ToString("f3");
        }
    }
}
