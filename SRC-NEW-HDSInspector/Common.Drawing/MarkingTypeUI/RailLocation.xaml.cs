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
    /// RailLocation.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RailLocation : UserControl
    {
        public event MoveButtonClick MoveClick;
        public event SetButtonClick SetClick;

        public RailLocation()
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

        public void SetParamDummy(double GapX, double GapY)
        {
            lblGX.Visibility = Visibility.Visible;
            lblGY.Visibility = Visibility.Visible;
            txtGX.Visibility = Visibility.Visible;
            txtGY.Visibility = Visibility.Visible;
            lblFX.Visibility = Visibility.Hidden;
            lblFY.Visibility = Visibility.Hidden;
            txtFX.Visibility = Visibility.Hidden;
            txtFY.Visibility = Visibility.Hidden;
            lblWidth.Visibility = Visibility.Hidden;
            lblHeight.Visibility = Visibility.Hidden;
            txtWidth.Visibility = Visibility.Hidden;
            txtHeight.Visibility = Visibility.Hidden;
            lblRadian.Visibility = Visibility.Hidden;
            lblRotate.Visibility = Visibility.Hidden;
            txtRotate.Visibility = Visibility.Hidden;
            txtGX.Text = GapX.ToString("f3");
            txtGY.Text = GapY.ToString("f3");
        }

        public void SetParamFirst(double fx, double fy, double width, double height, double rotate, bool bCircle)
        {
                lblGX.Visibility = Visibility.Hidden;
                lblGY.Visibility = Visibility.Hidden;
                txtGX.Visibility = Visibility.Hidden;
                txtGY.Visibility = Visibility.Hidden;
                lblFX.Visibility = Visibility.Visible;
                lblFY.Visibility = Visibility.Visible;
                txtFX.Visibility = Visibility.Visible;
                txtFY.Visibility = Visibility.Visible;
                lblRotate.Visibility = Visibility.Visible;
                txtRotate.Visibility = Visibility.Visible;
                if (bCircle)
                {
                    this.lblWidth.Visibility = Visibility.Hidden;
                    this.lblHeight.Visibility = Visibility.Hidden;
                    this.txtWidth.Visibility = Visibility.Visible;
                    this.txtHeight.Visibility = Visibility.Hidden;
                    this.lblRadian.Visibility = Visibility.Visible;
                }
                else
                {
                    this.lblWidth.Visibility = Visibility.Visible;
                    this.lblHeight.Visibility = Visibility.Visible;
                    this.txtWidth.Visibility = Visibility.Visible;
                    this.txtHeight.Visibility = Visibility.Visible;
                    this.lblRadian.Visibility = Visibility.Hidden;
                }
                txtFX.Text = fx.ToString("f3");
                txtFY.Text = fy.ToString("f3");
                txtWidth.Text = width.ToString("f3");
                txtHeight.Text = height.ToString("f3");
                txtRotate.Text = rotate.ToString("f3");
        }
    }
}
