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
    /// Location.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public delegate void MoveButtonClick(int nid, double pitch);
    public delegate void SetButtonClick();
    public delegate void LocationSetButtonClick(int Location);

    public partial class Location : UserControl
    {
        public event MoveButtonClick MoveClick;
        public event SetButtonClick SetClick;
        public event LocationSetButtonClick LocationSetClick;
        
        public Location()
        {
            InitializeComponent();
            this.btnLeft.Click += btnMove_Click;
            this.btnRight.Click += btnMove_Click;
            this.btnUp.Click += btnMove_Click;
            this.btnDown.Click += btnMove_Click;
            this.btnLoaction.Click += btnLoaction_Click;
            this.rdLeft.Click += btnWeekLocation_Click;
            this.rdRight.Click += btnWeekLocation_Click;
            this.rdCenter.Click += btnWeekLocation_Click;
        }

        void btnLoaction_Click(object sender, RoutedEventArgs e)
        {
            SetButtonClick er = SetClick;
            if (er != null) er();
        }

        void btnWeekLocation_Click(object sender, RoutedEventArgs e)
        {
            int Location = 0;
            if (rdLeft.IsChecked == true) Location = 0;
            else if (rdRight.IsChecked == true) Location = 1;
            else if (rdCenter.IsChecked == true) Location = 2;
            LocationSetButtonClick er = LocationSetClick;
            if (er != null) er(Location);
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

        public void SetParam(double X, double Y, double Width, double Height, double Rotate, int type, bool isReject = false, bool isWeek = false)
        {
            this.rdLeft.Visibility = Visibility.Hidden;
            this.lbLeft.Visibility = Visibility.Hidden;
            this.rdRight.Visibility = Visibility.Hidden;
            this.lbRight.Visibility = Visibility.Hidden;
            this.rdCenter.Visibility = Visibility.Hidden;
            this.lbCenter.Visibility = Visibility.Hidden;
            txtRotate.IsEnabled = false;
            if (type <= 4)
            {
                if (type == 1)
                {
                    this.lblWidth.Visibility = Visibility.Hidden;
                    this.lblHeight.Visibility = Visibility.Hidden;
                    this.txtWidth.Visibility = Visibility.Hidden;
                    this.txtHeight.Visibility = Visibility.Hidden;
                    this.lblRadian.Visibility = Visibility.Visible;
                    this.txtRadian.Visibility = Visibility.Visible;
                    txtLeft.Text = X.ToString("f3");
                    txtTop.Text = Y.ToString("f3");
                    txtRadian.Text = Width.ToString("f3");
                    txtRotate.Text = Rotate.ToString("f3");
                }
                else
                {
                    this.lblWidth.Visibility = Visibility.Visible;
                    this.lblHeight.Visibility = Visibility.Visible;
                    this.txtWidth.Visibility = Visibility.Visible;
                    this.txtHeight.Visibility = Visibility.Visible;
                    this.lblRadian.Visibility = Visibility.Hidden;
                    this.txtRadian.Visibility = Visibility.Hidden;
                    txtLeft.Text = X.ToString("f3");
                    txtTop.Text = Y.ToString("f3");
                    txtWidth.Text = Width.ToString("f3");
                    txtHeight.Text = Height.ToString("f3");
                    txtRotate.Text = Rotate.ToString("f3");
                }
            }
            else
            {
                if (type == 5)
                {
                    this.lblWidth.Visibility = Visibility.Hidden;
                    this.lblHeight.Visibility = Visibility.Hidden;
                    this.txtWidth.Visibility = Visibility.Hidden;
                    this.txtHeight.Visibility = Visibility.Hidden;
                    this.lblRadian.Visibility = Visibility.Visible;
                    this.txtRadian.Visibility = Visibility.Visible;
                    txtLeft.Text = X.ToString("f3");
                    txtTop.Text = Y.ToString("f3");
                    txtRadian.Text = Width.ToString("f3");
                    txtRotate.Text = Rotate.ToString("f3");
                }
                else
                {
                    this.lblWidth.Visibility = Visibility.Visible;
                    this.lblHeight.Visibility = Visibility.Visible;
                    this.txtWidth.Visibility = Visibility.Visible;
                    this.txtHeight.Visibility = Visibility.Visible;
                    this.lblRadian.Visibility = Visibility.Hidden;
                    this.txtRadian.Visibility = Visibility.Hidden;

                    if (isReject)
                    {
                        this.rdLeft.Visibility = Visibility.Visible;
                        this.lbLeft.Visibility = Visibility.Visible;
                        this.rdRight.Visibility = Visibility.Visible;
                        this.lbRight.Visibility = Visibility.Visible;
                        if (isWeek)
                        {
                            this.rdCenter.Visibility = Visibility.Visible;
                            this.lbCenter.Visibility = Visibility.Visible;
                        }
                    }
                    txtLeft.Text = X.ToString("f3");
                    txtTop.Text = Y.ToString("f3");
                    txtWidth.Text = Width.ToString("f3");
                    txtHeight.Text = Height.ToString("f3");
                    txtRotate.Text = Rotate.ToString("f3");
                    txtRotate.IsEnabled = true;
                }
            }
        }
    }
}
