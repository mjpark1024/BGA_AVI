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

namespace HDSInspector
{
    /// <summary>
    /// CamResultCtrl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CamResultCtrl : UserControl
    {
        public int ID;
        public List<InspectionResultImage> Image;
        public CamResultCtrl(int ResultimageCount)
        {
            InitializeComponent();
            Image = new List<InspectionResultImage>();
            // 반복문을 사용하여 Image를 생성
            for (int i = 0; i < ResultimageCount; i++)
            {
                GridResultimage.ColumnDefinitions.Add(new ColumnDefinition());
                InspectionResultImage TempImage = new InspectionResultImage
                {
                    Margin = new Thickness(2)
                };
                Image.Add(TempImage);
                Image[i].Name = $"Image_{i}";
                Grid.SetColumn(Image[i], i);
                Grid.SetRowSpan(Image[i], 3);
                GridResultimage.Children.Add(Image[i]);
            }
        }

        public void SetID(int anID)
        {
            ID = anID;
            btnLC.Tag = ID;
            btnSaveLight.Tag = ID;
            btnCancelLight.Tag = ID;
        }

        public void SetGV(double x, double y, int red, int green, int blue)
        {
            lblPosX.Content = "X:" + x.ToString("00");
            lblPosY.Content = "Y:" + y.ToString("00");
            lblBV.Content = red.ToString("000");
            lblGV.Content = green.ToString("000");
            lblRV.Content = blue.ToString("000");
        }

        public void SetSkip(bool bSkip)
        {
            bdrSkip.Visibility = bSkip ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
