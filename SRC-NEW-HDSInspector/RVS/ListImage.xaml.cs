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
using System.IO;

namespace RVS
{
    /// <summary>
    /// ListImage.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public delegate void ImageClick(int anTag);

    public partial class ListImage : UserControl
    {
        Image[] Ref = new Image[10];
        public Image[] Def = new Image[10];
        TextBlock[] label = new TextBlock[10];
        public event ImageClick Image_Click;

        public ListImage()
        {
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            Ref[0] = RefImage01; Ref[1] = RefImage02; Ref[2] = RefImage03; Ref[3] = RefImage04; Ref[4] = RefImage05;
            Ref[5] = RefImage06; Ref[6] = RefImage07; Ref[7] = RefImage08; Ref[8] = RefImage09; Ref[9] = RefImage10;
            Def[0] = DefImage01; Def[1] = DefImage02; Def[2] = DefImage03; Def[3] = DefImage04; Def[4] = DefImage05;
            Def[5] = DefImage06; Def[6] = DefImage07; Def[7] = DefImage08; Def[8] = DefImage09; Def[9] = DefImage10;
            for (int i = 0; i < 10; i++)
            {
                Def[i].MouseLeftButtonUp += ImageControl_MouseLeftButtonDown;
            }

            label[0] = txtNGName01; label[1] = txtNGName02; label[2] = txtNGName03; label[3] = txtNGName04; label[4] = txtNGName05;
            label[5] = txtNGName06; label[6] = txtNGName07; label[7] = txtNGName08; label[8] = txtNGName09; label[9] = txtNGName10;
        }

        void ImageControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int nTag = int.Parse(((Image)sender).Tag.ToString());
            ImageClick img = Image_Click;
            img(nTag);
        }

        public void Clear()
        {
            for (int i = 0; i < 10; i++)
            {
                Ref[i].Source = null;
                Def[i].Source = null;
                label[i].Text = "";
            }
        }

        public void ImageSet(string[] path)
        {
            string strRef, strDef;
            string[] tmp;
            for (int i = 0; i < path.Length; i++)
            {
                strDef = path[i];
                strRef = strDef.Replace("DEF", "REF");
                tmp = strDef.Split(' ', '.');
                if (File.Exists(strRef) && File.Exists(strDef))
                {
                    BitmapImage refImage = new BitmapImage();
                    BitmapImage defImage = new BitmapImage();

                    refImage.BeginInit();
                    refImage.UriSource = new Uri(strRef, UriKind.Absolute);
                    refImage.EndInit();
                    this.Ref[i].Source = refImage;

                    defImage.BeginInit();
                    defImage.UriSource = new Uri(strDef, UriKind.Absolute);
                    defImage.EndInit();
                    this.Def[i].Source = defImage;
                    this.label[i].Text = tmp[tmp.Length - 2];
                }
            }
        }
    }
}

