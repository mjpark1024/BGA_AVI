/*********************************************************************************
 * Copyright(c) 2015 by Haesung DS.
 * 
 * This software is copyrighted by, and is the sole property of Haesung DS.
 * All rigths, title, ownership, or other interests in the software remain the
 * property of Haesung DS. This software may only be used in accordance with
 * the corresponding license agreement. Any unauthorized use, duplication, 
 * transmission, distribution, or disclosure of this software is expressly 
 * forbidden.
 *
 * This Copyright notice may not be removed or modified without prior written
 * consent of Haesung DS reserves the right to modify this 
 * software without notice.
 * 
 * Haesung DS.
 * KOREA 
 * http://www.HaesungDS.com
 *********************************************************************************/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using RVS.Generic;
using Common;

namespace HDSInspector
{
    public class AntiAliasedImage : System.Windows.Controls.Image
    {
        protected override void OnRender(DrawingContext dc)
        {
            this.VisualBitmapScalingMode = System.Windows.Media.BitmapScalingMode.NearestNeighbor;
            base.OnRender(dc);
        }
    }

    public delegate void ImageMouseMove(int Index, double x, double y, byte[] gv);//////수정


    /// <summary>   Inspection result image.  </summary>
    public partial class InspectionResultImage : UserControl
    {
        #region Member Variables.
#pragma warning disable CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
        public event ImageMouseMove MouseMove;
#pragma warning restore CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
        private NGImageData m_NGImageData = new NGImageData();

        //public RectFill FillColor
        //{
        //    get
        //    {
        //        return m_NGImageData.NGType;
        //    }
        //    set
        //    {
        //        m_NGImageData.NGType = value;
        //        this.NGColor.Background = new SolidColorBrush(ColorPalette.GetColor((int)m_NGImageData.NGType));
        //    }
        //}

        public int NG_ID
        {
            get
            {
                return m_NGImageData.NG_ID;
            }
            set
            {
                m_NGImageData.NG_ID = value;
            }
        }

        public Brush NG_Color
        {
            get
            {
                return m_NGImageData.NG_Color;
            }
            set
            {
                m_NGImageData.NG_Color = value;
                this.NGColor.Background = value;
            }
        }

        public string NGName
        {
            get
            {
                return m_NGImageData.NGName;
            }
            set
            {
                m_NGImageData.NGName = value;
                this.txtNGName.Text = m_NGImageData.NGName;
            }
        }

        public int Row { get; set; }
        public int Col { get; set; }
        #endregion

        public InspectionResultImage()
        {
            InitializeComponent();
            this.NGColor.Opacity = 0.8;
            this.DefImage.PreviewMouseMove += DefImage_PreviewMouseMove;
            this.RefImage.PreviewMouseMove += DefImage_PreviewMouseMove;
        }

        void DefImage_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender == null) return;
            BitmapSource bs;
            System.Windows.Point ptCurrentByImage;
            if ((sender as Common.AntiAliasedImage).Tag.ToString() == "ref")
            {
                if (RefImage.Source == null) return;
                bs = RefImage.Source as BitmapSource;
                ptCurrentByImage = e.GetPosition(RefImage);
            }
            else
            {
                if (DefImage.Source == null) return;
                bs = DefImage.Source as BitmapSource;
                ptCurrentByImage = e.GetPosition(DefImage);
            }
            double scx = DefImage.ActualWidth / 96.0;
            double scy = DefImage.ActualHeight / 96.0;
            ptCurrentByImage.X /= scx;
            ptCurrentByImage.Y /= scy;
            ImageMouseMove er = MouseMove;
            if (!((int)ptCurrentByImage.X >= 96) &&
                        !((int)ptCurrentByImage.Y >= 96))
            {
                if (bs.Format != PixelFormats.Bgr24)
                {

                    // Calculate GV Value.
                    byte[] pixel = new byte[1];

                    bs.CopyPixels(new Int32Rect((int)ptCurrentByImage.X, (int)ptCurrentByImage.Y, 1, 1),
                                                      pixel, 96, 0);
                    if (er != null)
                        er(0, ptCurrentByImage.X, ptCurrentByImage.Y, pixel);
                }
                else
                {
                    byte[] pixel = new byte[3];
                    bs.CopyPixels(new Int32Rect((int)ptCurrentByImage.X, (int)ptCurrentByImage.Y, 1, 1),
                                                     pixel, 96, 0);
                    if (er != null)
                        er(0, ptCurrentByImage.X, ptCurrentByImage.Y, pixel);
                }
            }
            else
            {
                //if (er != null) er(0, 0, new byte[1] { 0 });

            }
        }

        public void ChangeImageSet(int anIndex)
        {
            BitmapImage refImage = new BitmapImage();
            BitmapImage defImage = new BitmapImage();

            string filePath = FileSupport.GetFile(m_NGImageData.StandardImagePath, anIndex);
            if (File.Exists(filePath))
            {
                refImage.BeginInit();
                refImage.UriSource = new Uri(filePath, UriKind.Absolute);
                refImage.EndInit();
                this.RefImage.Source = refImage;
            }

            filePath = FileSupport.GetFile(m_NGImageData.NGImagePath, anIndex);
            if (File.Exists(filePath))
            {
                defImage.BeginInit();
                defImage.UriSource = new Uri(filePath, UriKind.Absolute);
                defImage.EndInit();
                this.DefImage.Source = defImage;
            }
        }

        public void ChangeImageSet(String aszRefImagePath, String aszDefImagePath)
        {
            if (File.Exists(aszRefImagePath) && File.Exists(aszDefImagePath))
            {
                BitmapImage refImage = new BitmapImage();
                BitmapImage defImage = new BitmapImage();

                refImage.BeginInit();
                refImage.UriSource = new Uri(aszRefImagePath, UriKind.Absolute);
                refImage.EndInit();
                this.RefImage.Source = refImage;

                defImage.BeginInit();
                defImage.UriSource = new Uri(aszDefImagePath, UriKind.Absolute);
                defImage.EndInit();
                this.DefImage.Source = defImage;
            }
        }

        public void ChangeImageSet(BitmapSource aRefImage, BitmapSource aDefImage, bool abIsLastDefect)
        {
            RefImage.Source = aRefImage;
            DefImage.Source = aDefImage;

            if (abIsLastDefect)
                LastDefImage.Visibility = Visibility.Visible;
        }

        public void ClearImage()
        {
            RefImage.Source = null;
            DefImage.Source = null;
            NG_ID = -1;
            NG_Color = new SolidColorBrush(Colors.White);
            //FillColor = RectFill.INIT;

            NGName = "";
            txtDefectInfo.Text = "";
            txtUnitPos.Text = "";
            LastDefImage.Visibility = Visibility.Hidden;
        }
    }
}
