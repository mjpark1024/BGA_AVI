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
/**
 * @file  ThumbnailViewer.xaml.cs
 * @brief 
 *  Interaction logic for ThumbnailViewer.xaml.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.08.22
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.22 First creation.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

namespace HDSInspector
{
    /// <summary>   Thumbnail Viewer Control.  </summary>
    /// <remarks>   suoow2, 2014-08-22. </remarks>
    public partial class ThumbnailViewer : UserControl
    {
        #region Member Variables.
        private BitmapSource m_ptrSourceImage;
        private ScrollViewer m_ptrTargetScrollViewer;
        private Slider m_ptrTargetSlider;
        private Rectangle m_rectNavigator;

        private bool m_bSourceLoaded = false;
        private double m_fSourceImageWidth;
        private double m_fSourceImageHeight;
        #endregion

        /// <summary>   Initializes a new instance of the UnitControl class. </summary>
        /// <remarks>   suoow2, 2014-08-22. </remarks>
        public ThumbnailViewer()
        {
            InitializeComponent();
            InitializeEvent();
            InitializeDialog();
        }

        /// <summary>   Initializes the dialog. </summary>
        /// <remarks>   suoow2, 2014-08-22. </remarks>
        private void InitializeDialog()
        {
            // Create Rectangle.
            m_rectNavigator = new Rectangle();
            m_rectNavigator.HorizontalAlignment = HorizontalAlignment.Left;
            m_rectNavigator.VerticalAlignment = VerticalAlignment.Top;
            m_rectNavigator.Stroke = new SolidColorBrush(Colors.Red);
            m_rectNavigator.StrokeThickness = 1.0;
            m_rectNavigator.Width = 0;
            m_rectNavigator.Height = 0;
            
            this.pnlFullImage.Children.Add(m_rectNavigator);
        }

        private void InitializeEvent()
        {
            this.pnlFullImage.MouseLeftButtonDown += pnlFullImage_MouseLeftButtonDown;
            this.pnlFullImage.MouseMove += pnlFullImage_MouseMove;
        }

        // ThumbnailViewer에 소스를 연결시켜준다.
        public void SetSourceImage(TeachingViewerCtrl aViewer)
        {
            if (aViewer.TeachingImageSource == null)
            {
                m_bSourceLoaded = false;
                this.imgFull.Source = null;
                this.m_rectNavigator.Visibility = Visibility.Collapsed;

                // 원본 이미지 없음. 컨트롤 무효화 수행.
                m_fSourceImageWidth = 0;
                m_fSourceImageHeight = 0;
            }
            else
            {
                m_bSourceLoaded = true;
                this.imgFull.Source = aViewer.TeachingImageSource;
                this.m_rectNavigator.Visibility = Visibility.Visible;

                // 원본 이미지 Width & Height
                m_fSourceImageWidth = aViewer.TeachingImageSource.Width;
                m_fSourceImageHeight = aViewer.TeachingImageSource.Height;

                // Pointing Controls.
                this.m_ptrSourceImage = aViewer.TeachingImageSource;
                this.m_ptrTargetScrollViewer = aViewer.svTeaching;
                this.m_ptrTargetSlider = aViewer.sldrScale;
            }
        }

        public void SetSourceImage(MarkViewerCtrl aViewer)
        {
            if (aViewer.TeachingImageSource == null)
            {
                m_bSourceLoaded = false;
                this.imgFull.Source = null;
                this.m_rectNavigator.Visibility = Visibility.Collapsed;

                // 원본 이미지 없음. 컨트롤 무효화 수행.
                m_fSourceImageWidth = 0;
                m_fSourceImageHeight = 0;
            }
            else
            {
                m_bSourceLoaded = true;
                this.imgFull.Source = aViewer.TeachingImageSource;
                this.m_rectNavigator.Visibility = Visibility.Visible;

                // 원본 이미지 Width & Height
                m_fSourceImageWidth = aViewer.TeachingImageSource.Width;
                m_fSourceImageHeight = aViewer.TeachingImageSource.Height;

                // Pointing Controls.
                this.m_ptrSourceImage = aViewer.TeachingImageSource;
                this.m_ptrTargetScrollViewer = aViewer.svTeaching;
                this.m_ptrTargetSlider = aViewer.sldrScale;
            }
        }

        /// <summary>   Updates the navigator. </summary>
        /// <remarks>   suoow2, 2014-08-22. </remarks>
        public void UpdateNavigator()
        {
            if (m_ptrTargetScrollViewer == null ||
                m_ptrTargetScrollViewer.ViewportWidth == 0 ||
                m_ptrTargetScrollViewer.ViewportHeight == 0)
            {
                return;
            }

            if (m_bSourceLoaded)
            {
                // ScrollViewer Width & Height (한 화면에서 표시할 수 있는 영역 획득)
                double fSourceViewportWidth = m_ptrTargetScrollViewer.ViewportWidth;
                double fSourceViewportHeight = m_ptrTargetScrollViewer.ViewportHeight;

                double fScale = m_ptrTargetSlider.Value;
                double fRatio = imgFull.ActualWidth / m_fSourceImageWidth;
                
                // ScrollViewer의 ViewportWidth 및 ViewportHeight 표시 비율
                double fWidthViewRatio = m_fSourceImageWidth * fScale / fSourceViewportWidth;
                double fHeightViewRatio = m_fSourceImageHeight * fScale / fSourceViewportHeight;

                // 네비게이터의 X, Y 시작좌표
                double fLeft = m_ptrTargetScrollViewer.HorizontalOffset / fScale * fRatio;
                double fTop = m_ptrTargetScrollViewer.VerticalOffset / fScale * fRatio;

                // 네비게이터의 Width & Height
                if (!double.IsInfinity(fSourceViewportWidth / fScale * fRatio))
                {
                    m_rectNavigator.Width = fSourceViewportWidth / fScale * fRatio;
                }
                else
                {
                    m_rectNavigator.Width = imgFull.ActualWidth;
                }

                if (!double.IsInfinity(fSourceViewportHeight / fScale * fRatio))
                {
                    m_rectNavigator.Height = fSourceViewportHeight / fScale * fRatio;
                }
                else
                {
                    m_rectNavigator.Height = imgFull.ActualHeight;
                }

                if (fWidthViewRatio < 1)
                {
                    m_rectNavigator.Width *= fWidthViewRatio;
                }
                if (fHeightViewRatio < 1)
                {
                    m_rectNavigator.Height *= fHeightViewRatio;
                }

                // Thumbnail 이미지 정렬에 대한 X, Y 시작좌표 보정
                if (m_fSourceImageHeight / this.ActualHeight > m_fSourceImageWidth / this.ActualWidth)
                {
                    fLeft += Math.Abs((pnlFullImage.ActualWidth - imgFull.ActualWidth) / 2);
                }
                else
                {
                    fTop += Math.Abs((pnlFullImage.ActualHeight - imgFull.ActualHeight) / 2);
                }

                if (!double.IsNaN(fLeft) || !double.IsNaN(fTop))
                {
                    m_rectNavigator.Margin = new Thickness(fLeft, fTop, 0, 0);
                }
            }
        }

        /// <summary>   Move navigator. </summary>
        /// <remarks>   suoow2, 2014-08-22. </remarks>
        /// <param name="clickedPoint"> The clicked point. </param>
        private void MoveNavigator(Point clickedPoint)
        {
            if (m_bSourceLoaded)
            {
                if (Math.Round(m_rectNavigator.Height) >= (int)imgFull.ActualHeight &&
                    Math.Round(m_rectNavigator.Width) >= (int)imgFull.ActualWidth)
                {
                    return;
                }

                double fNewLeft = clickedPoint.X - (m_rectNavigator.Width / 2);
                double fNewRight = clickedPoint.X + (m_rectNavigator.Width / 2);
                double fNewTop = clickedPoint.Y - (m_rectNavigator.Height / 2);
                double fNewBottom = clickedPoint.Y + (m_rectNavigator.Height / 2);

                if (m_fSourceImageHeight / this.ActualHeight > m_fSourceImageWidth / this.ActualWidth)
                {
                    fNewLeft = (fNewLeft < (pnlFullImage.ActualWidth - imgFull.ActualWidth) / 2) ? (pnlFullImage.ActualWidth - imgFull.ActualWidth) / 2 : fNewLeft;
                    fNewTop = (fNewTop < 0) ? 0 : fNewTop;

                    if (fNewRight > pnlFullImage.ActualWidth - (pnlFullImage.ActualWidth - imgFull.ActualWidth) / 2)
                    {
                        fNewLeft -= fNewRight - (pnlFullImage.ActualWidth - (pnlFullImage.ActualWidth - imgFull.ActualWidth) / 2);
                    }
                    if (fNewBottom > imgFull.ActualHeight)
                    {
                        fNewTop -= fNewBottom - imgFull.ActualHeight;
                    }
                }
                else // 상하 여백이 있고, 좌우 여백이 없는 형태로 Thumbnail이 출력된다.
                {
                    fNewLeft = (fNewLeft < 0) ? 0 : fNewLeft;
                    fNewTop = (fNewTop < (pnlFullImage.ActualHeight - imgFull.ActualHeight) / 2) ? (pnlFullImage.ActualHeight - imgFull.ActualHeight) / 2 : fNewTop;

                    if (fNewRight > imgFull.ActualWidth)
                    {
                        fNewLeft -= fNewRight - imgFull.ActualWidth;
                    }
                    if (fNewBottom > pnlFullImage.ActualHeight - (pnlFullImage.ActualHeight - imgFull.ActualHeight) / 2)
                    {
                        fNewTop -= fNewBottom - (pnlFullImage.ActualHeight - (pnlFullImage.ActualHeight - imgFull.ActualHeight) / 2);
                    }
                }
                m_rectNavigator.Margin = new Thickness(fNewLeft, fNewTop, 0, 0);

                // Target의 Scroll Offset 연산 재보정.
                double fScale = m_ptrTargetSlider.Value;
                double fRatio = imgFull.ActualWidth / m_ptrSourceImage.PixelWidth;
                if (m_fSourceImageHeight / this.ActualHeight > m_fSourceImageWidth / this.ActualWidth)
                {
                    if (m_fSourceImageHeight >= imgFull.ActualHeight)
                    {
                        fNewLeft -= Math.Abs((pnlFullImage.ActualHeight - imgFull.ActualHeight) / 2);
                    }
                    fNewLeft -= Math.Abs((pnlFullImage.ActualWidth - imgFull.ActualWidth) / 2);
                }
                else
                {
                    if (m_fSourceImageWidth >= imgFull.ActualWidth)
                    {
                        fNewTop -= Math.Abs((pnlFullImage.ActualWidth - imgFull.ActualWidth) / 2);
                    }
                    fNewTop -= Math.Abs((pnlFullImage.ActualHeight - imgFull.ActualHeight) / 2);
                }
                m_ptrTargetScrollViewer.ScrollToHorizontalOffset(fNewLeft * fScale / fRatio);
                m_ptrTargetScrollViewer.ScrollToVerticalOffset(fNewTop * fScale / fRatio);
            }
        }

        #region Mouse event handlers.
        private void pnlFullImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MoveNavigator(e.GetPosition(pnlFullImage));
            }
            catch
            {
                Debug.WriteLine("Exception occured in pnlFullImage_MouseLeftButtonDown(ThumbnailViewer.xaml.cs)");
            }
        }

        private void pnlFullImage_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    MoveNavigator(e.GetPosition(pnlFullImage));
                }
            }
            catch
            {
                Debug.WriteLine("Exception occured in pnlFullImage_MouseMove(ThumbnailViewer.xaml.cs)");
            }
        }
        #endregion
    }
}
