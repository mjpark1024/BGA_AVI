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
 * @file  ImageEditor.xaml.cs
 * @brief
 *  It supports editing image.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.10.03
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.10.03 First creation.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Common;
using System.Windows.Ink;
using System.Collections.Generic;

namespace HDSInspector
{
    /// <summary>   Editor for image.  </summary>
    /// <remarks>   suoow2, 2014-10-03. </remarks>
    public partial class ImageEditor : Window
    {
        #region Private member variables.
        private AntiAliasedInkCanvas TeachingImageCanvas = new AntiAliasedInkCanvas();
        
        private BitmapSource TeachingImage
        {
            get
            {
                ImageBrush backgroundImageBrush = TeachingImageCanvas.Background as ImageBrush;
                if (backgroundImageBrush != null)
                {
                    return (backgroundImageBrush.ImageSource as BitmapSource);
                }
                else return null;
            }
        }
        private BitmapSource m_CloneTeachingImage = null;

        private System.Windows.Point? m_ptLastContentMousePosition;
        private System.Windows.Point? m_ptLastCenterOfViewport;

        // Zoom To Fit 값.
        private double m_fZoomToFitScale;

        private TeachingViewerCtrl m_ptrParentCtrl;
        private System.Windows.Point? m_ptLastDragPoint;

        private int m_nPixelWidth;
        private int m_nPixelHeight;

        private byte[] m_pixels = null;
        private byte[,] m_pixelData = null;

        private List<int> m_brushList = new List<int>();

        private static int? m_nLastBrushColor;
        private static int? m_nLastBrushSize;
        #endregion

        /// <summary>   Initializes a new instance of the ImageEditor class. </summary>
        /// <remarks>   suoow2, 2014-10-03. </remarks>
        public ImageEditor(BitmapSource aTeachingImage, double afZoomToFitScale, TeachingViewerCtrl aParentCtrl)
        {
            InitializeComponent();
            InitializeDialog(aTeachingImage, afZoomToFitScale, aParentCtrl);
            InitializeEvent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        /// <summary>   Initializes the dialog. </summary>
        /// <remarks>   suoow2, 2014-10-03. </remarks>
        private void InitializeDialog(BitmapSource aTeachingImage, double afZoomToFitScale, TeachingViewerCtrl aParentCtrl)
        {
            m_ptrParentCtrl = aParentCtrl;

            if (m_nLastBrushColor != null)
            {
                this.txtBrushColor.Text = m_nLastBrushColor.ToString();
            }
            if (m_nLastBrushSize != null)
            {
                this.txtBrushSize.Text = m_nLastBrushSize.ToString();
            }

            TeachingImageCanvas.HorizontalAlignment = HorizontalAlignment.Left;
            TeachingImageCanvas.VerticalAlignment = VerticalAlignment.Top;
            TeachingImageCanvas.Background = new ImageBrush(aTeachingImage);
            TeachingImageCanvas.Width = aTeachingImage.PixelWidth;
            TeachingImageCanvas.Height = aTeachingImage.PixelHeight;
            TeachingImageCanvas.DefaultDrawingAttributes.StylusTip = StylusTip.Rectangle;
            TeachingImageCanvas.DefaultDrawingAttributes.Color = Color.FromArgb(255, Convert.ToByte(txtBrushColor.Text), Convert.ToByte(txtBrushColor.Text), Convert.ToByte(txtBrushColor.Text));
            TeachingImageCanvas.DefaultDrawingAttributes.Width = Convert.ToDouble(txtBrushSize.Text) * 2 - 1;
            TeachingImageCanvas.DefaultDrawingAttributes.Height = Convert.ToDouble(txtBrushSize.Text) * 2 - 1;
            TeachingImageCanvas.EditingMode = InkCanvasEditingMode.Ink;

            this.btnFreeDrawing.IsChecked = true;

            m_nPixelWidth = aTeachingImage.PixelWidth;
            m_nPixelHeight = aTeachingImage.PixelHeight;
            m_pixelData = new byte[m_nPixelHeight, m_nPixelWidth];
            m_pixels = new byte[m_nPixelWidth * m_nPixelHeight];                    
            aTeachingImage.CopyPixels(m_pixels, m_nPixelWidth, 0);
            for (int r = 0; r < m_nPixelHeight; r++)
            {
                for (int w = 0; w < m_nPixelWidth; w++)
                {
                    m_pixelData[r, w] = m_pixels[r * aTeachingImage.PixelWidth + w];
                }
            }
            this.m_CloneTeachingImage = BitmapSourceHelper.CloneBitmapSource(aTeachingImage);
            
            m_fZoomToFitScale = afZoomToFitScale;

            this.pnlInner.Children.Add(TeachingImageCanvas);
            this.sldrScale.Minimum = m_fZoomToFitScale;
            this.sldrScale.Value = m_fZoomToFitScale;
        }

        /// <summary>   Initializes the event. </summary>
        /// <remarks>   suoow2, 2014-10-03. </remarks>
        private void InitializeEvent()
        {
            this.btnColorPicker.Click += btnColorPicker_Click;
            this.btnFreeDrawing.Click += btnFreeDrawing_Click;
            this.btnEraser.Click += btnEraser_Click;
            this.btnInitImage.Click += btnInitImage_Click;
            this.btnOK.Click += btnOK_Click;
            this.btnCancel.Click += btnCancel_Click;

            this.pnlInner.PreviewMouseMove += pnlContainer_MouseMove;
            this.pnlInner.PreviewMouseWheel += pnlContainer_MouseWheel;
            this.pnlInner.MouseUp += pnlInner_MouseUp;
            this.btnZoomIn.Click += zoomBtn_Click;
            this.btnZoomOut.Click += zoomBtn_Click;
            this.btnZoomToFit.Click += zoomBtn_Click;

            this.txtBrushSize.ValueChanged += txtBrushSize_ValueChanged;
            this.txtBrushSize.LostFocus += txtBrushSize_LostFocus;
            this.txtBrushColor.ValueChanged += txtBrushColor_ValueChanged;
            this.txtBrushColor.LostFocus += txtBrushColor_LostFocus;
            this.sldrScale.ValueChanged += sldrScale_ValueChanged;

            this.TeachingImageCanvas.PreviewMouseDown += TeachingImageCanvas_PreviewMouseDown;
            this.TeachingImageCanvas.PreviewMouseMove += TeachingImageCanvas_PreviewMouseMove;
            this.TeachingImageCanvas.MouseEnter += cvsCross_MouseEnter;
            this.TeachingImageCanvas.MouseLeave += cvsCross_MouseLeave;

            this.PreviewKeyDown += ImageEditor_PreviewKeyDown;
            this.PreviewKeyUp += ImageEditor_PreviewKeyUp;
            this.svEditImage.ScrollChanged += svEditImage_ScrollChanged;

            this.Closing += (s, e) =>
            {
                try
                {
                    m_nLastBrushColor = Convert.ToInt32(txtBrushColor.Text);
                    m_nLastBrushSize = Convert.ToInt32(txtBrushSize.Text);
                }
                catch
                {
                    m_nLastBrushColor = 128;
                    m_nLastBrushSize = 5;
                }
            };
        }

        private void ImageEditor_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                TeachingImageCanvas.UseCustomCursor = false;
                if (btnEraser.IsChecked == true)
                {
                    TeachingImageCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                }
                else if(btnColorPicker.IsChecked == true)
                {
                    TeachingImageCanvas.EditingMode = InkCanvasEditingMode.None;
                }
                else
                {
                    TeachingImageCanvas.EditingMode = InkCanvasEditingMode.Ink;
                }
            }
        }

        private void ImageEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseWithCancel();
            }

            if (e.Key == Key.Space)
            {
                TeachingImageCanvas.Focus();                
                TeachingImageCanvas.EditingMode = InkCanvasEditingMode.None;
                TeachingImageCanvas.UseCustomCursor = true;
                TeachingImageCanvas.Cursor = System.Windows.Input.Cursors.ScrollAll;
            }
        }

        private void txtBrushSize_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBrushSize.Text))
            {
                txtBrushSize.Text = "5";
                this.TeachingImageCanvas.DefaultDrawingAttributes.Width = 9.0;
                this.TeachingImageCanvas.DefaultDrawingAttributes.Height = 9.0;
            }
        }

        private void txtBrushColor_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBrushColor.Text))
            {
                txtBrushColor.Text = "128";
                TeachingImageCanvas.DefaultDrawingAttributes.Color = Color.FromArgb(255, Convert.ToByte(128), Convert.ToByte(128), Convert.ToByte(128));
            }
        }

        #region Mouse Indicator.
        // Scale이 변경되어도 지시선의 두께는 유지되도록 한다.
        private void sldrScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_horizontalLine.StrokeThickness = 1.0 / sldrScale.Value;
            m_verticalLine.StrokeThickness = 1.0 / sldrScale.Value;
        }

        private static Line m_horizontalLine = new Line() { Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 1.0 };
        private static Line m_verticalLine = new Line() { Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 1.0 };

        private void cvsCross_MouseEnter(object sender, MouseEventArgs e)
        {
            InitializeIndicator();

            TeachingImageCanvas.Children.Add(m_horizontalLine);
            TeachingImageCanvas.Children.Add(m_verticalLine);
        }

        private void cvsCross_MouseLeave(object sender, MouseEventArgs e)
        {
            TeachingImageCanvas.Children.Remove(m_horizontalLine);
            TeachingImageCanvas.Children.Remove(m_verticalLine);
        }
        
        private void InitializeIndicator()
        {
            m_horizontalLine.X1 = 0;
            m_horizontalLine.Y1 = -1;
            m_horizontalLine.X2 = TeachingImageCanvas.ActualWidth;
            m_horizontalLine.Y2 = -1;

            m_verticalLine.X1 = -1;
            m_verticalLine.Y1 = 0;
            m_verticalLine.X2 = -1;
            m_verticalLine.Y2 = TeachingImageCanvas.ActualHeight;
        }

        private void UpdateIndicator()
        {
            System.Windows.Point ptCrossCanvas = Mouse.GetPosition(TeachingImageCanvas);

            m_verticalLine.X1 = ptCrossCanvas.X;
            m_verticalLine.X2 = ptCrossCanvas.X;

            m_horizontalLine.Y1 = ptCrossCanvas.Y;
            m_horizontalLine.Y2 = ptCrossCanvas.Y;

            m_horizontalLine.X2 = TeachingImageCanvas.ActualWidth;
            m_verticalLine.Y2 = TeachingImageCanvas.ActualHeight;
        }
        #endregion

        #region Init & Apply Image.
        private void ApplyImage()
        {
            System.Drawing.Point start;
            System.Drawing.Point end;

            int brushThickness = 1;
            byte color = 0;
            int listLength = 0;
            foreach (Stroke stroke in TeachingImageCanvas.Strokes)
            {
                if (m_brushList.Count > 0)
                {
                    brushThickness = m_brushList[0];
                    m_brushList.RemoveAt(0);
                }
                else
                    brushThickness = (int)TeachingImageCanvas.DefaultDrawingAttributes.Width / 2;

                listLength = stroke.StylusPoints.Count;
                color = stroke.DrawingAttributes.Color.R; // R == G == B

                if (listLength == 1)
                {
                    SetPixels((int)stroke.StylusPoints[0].X, (int)stroke.StylusPoints[0].Y, color, brushThickness);
                }
                else
                {
                    for (int nIndex = 0; nIndex < listLength - 1; nIndex++)
                    {
                        start = new System.Drawing.Point((int)stroke.StylusPoints[nIndex].X, (int)stroke.StylusPoints[nIndex].Y);
                        end = new System.Drawing.Point((int)stroke.StylusPoints[nIndex + 1].X, (int)stroke.StylusPoints[nIndex + 1].Y);

                        start.X = (start.X < m_nPixelWidth) ? start.X : m_nPixelWidth - 1;
                        end.X = (end.X < m_nPixelWidth) ? end.X : m_nPixelWidth - 1;
                        start.Y = (start.Y < m_nPixelHeight) ? start.Y : m_nPixelHeight - 1;
                        end.Y = (end.Y < m_nPixelHeight) ? end.Y : m_nPixelHeight - 1;

                        int cnt = 1;
                        int changeStep; // 행 또는 열을 바꾸어 주어야 하는 단위.
                        int width = Math.Abs(end.X - start.X) + 1;
                        int height = Math.Abs(end.Y - start.Y) + 1;
                        #region Simple cases.
                        if (start.X == end.X && start.Y == end.Y)
                        {
                            SetPixels(start.X, start.Y, color, brushThickness);
                        }
                        else if (start.X == end.X)
                        {
                            // 점과 점 사이에 세로 직선을 그어준다.
                            int minY = (start.Y < end.Y) ? start.Y : end.Y;
                            int maxY = (start.Y < end.Y) ? end.Y : start.Y;
                            for (int y = minY; y < maxY; y++)
                            {
                                SetPixels(start.X, y, color, brushThickness);
                            }
                        }
                        else if (start.Y == end.Y)
                        {
                            // 점과 점 사이에 가로 직선을 그어준다.
                            int minX = (start.X < end.X) ? start.X : end.X;
                            int maxX = (start.X < end.X) ? end.X : start.X;
                            for (int x = minX; x < maxX; x++)
                            {
                                SetPixels(x, start.Y, color, brushThickness);
                            }
                        }
                        #endregion
                        #region start - 4 / end - 2
                        else if (start.X < end.X && start.Y < end.Y)
                        {
                            // 시점 - 4사분면 / 끝점 - 2사분면
                            if (width > height)
                            {
                                changeStep = width / height;
                                for (int y = start.Y; y <= end.Y; y++)
                                {
                                    for (int x = start.X; x <= end.X; x++)
                                    {
                                        SetPixels(x, y, color, brushThickness);
                                        if (cnt++ == changeStep && y != end.Y)
                                        {
                                            SetPixels(x, y + 1, color, brushThickness);
                                            start.X = x + 1;
                                            break;
                                        }
                                    }
                                    cnt = 1;
                                }
                            }
                            else
                            {
                                changeStep = height / width;
                                for (int x = start.X; x <= end.X; x++)
                                {
                                    for (int y = start.Y; y <= end.Y; y++)
                                    {
                                        SetPixels(x, y, color, brushThickness);
                                        if (cnt++ == changeStep && x != end.X)
                                        {
                                            SetPixels(x + 1, y, color, brushThickness);
                                            start.Y = y + 1;
                                            break;
                                        }
                                    }
                                    cnt = 1;
                                }
                            }
                        }
                        #endregion
                        #region start - 1 / end - 3
                        else if (start.X > end.X && start.Y < end.Y)
                        {
                            // 시점 - 1사분면 / 끝점 - 3사분면
                            if (width > height)
                            {
                                changeStep = width / height;
                                for (int y = start.Y; y <= end.Y; y++)
                                {
                                    for (int x = start.X; x >= end.X; x--)
                                    {
                                        SetPixels(x, y, color, brushThickness);
                                        if (cnt++ == changeStep && y != end.Y)
                                        {
                                            SetPixels(x, y + 1, color, brushThickness);
                                            start.X = x - 1;
                                            break;
                                        }
                                    }
                                    cnt = 1;
                                }
                            }
                            else
                            {
                                changeStep = height / width;
                                for (int x = start.X; x >= end.X; x--)
                                {
                                    for (int y = start.Y; y <= end.Y; y++)
                                    {
                                        SetPixels(x, y, color, brushThickness);
                                        if (cnt++ == changeStep && x != end.X)
                                        {
                                            SetPixels(x - 1, y, color, brushThickness);
                                            start.Y = y + 1;
                                            break;
                                        }
                                    }
                                    cnt = 1;
                                }
                            }
                        }
                        #endregion
                        #region start - 2 / end - 4
                        else if (start.X > end.X && start.Y > end.Y)
                        {
                            // 시점 - 2사분면 / 끝점 - 4사분면
                            if (width > height)
                            {
                                changeStep = width / height;
                                for (int y = start.Y; y >= end.Y; y--)
                                {
                                    for (int x = start.X; x >= end.X; x--)
                                    {
                                        SetPixels(x, y, color, brushThickness);
                                        m_pixelData[y, x] = color;
                                        if (cnt++ == changeStep && y != end.Y)
                                        {
                                            SetPixels(x, y - 1, color, brushThickness);
                                            start.X = x - 1;
                                            break;
                                        }
                                    }
                                    cnt = 1;
                                }
                            }
                            else
                            {
                                changeStep = height / width;
                                for (int x = start.X; x >= end.X; x--)
                                {
                                    for (int y = start.Y; y >= end.Y; y--)
                                    {
                                        SetPixels(x, y, color, brushThickness);
                                        m_pixelData[y, x] = color;
                                        if (cnt++ == changeStep && x != end.X)
                                        {
                                            SetPixels(x - 1, y, color, brushThickness);
                                            m_pixelData[y, x - 1] = color;
                                            start.Y = y - 1;
                                            break;
                                        }
                                    }
                                    cnt = 1;
                                }
                            }
                        }
                        #endregion
                        #region start - 3 / end - 1
                        else //if(start.X < end.X && start.Y > end.Y)
                        {
                            // 시점 - 3사분면 / 끝점 - 1사분면
                            if (width > height)
                            {
                                changeStep = width / height;
                                for (int y = start.Y; y >= end.Y; y--)
                                {
                                    for (int x = start.X; x <= end.X; x++)
                                    {
                                        SetPixels(x, y, color, brushThickness);
                                        if (cnt++ == changeStep && y != end.Y)
                                        {
                                            SetPixels(x, y - 1, color, brushThickness);
                                            start.X = x + 1;
                                            break;
                                        }
                                    }
                                    cnt = 1;
                                }
                            }
                            else
                            {
                                changeStep = height / width;
                                for (int x = start.X; x <= end.X; x++)
                                {
                                    for (int y = start.Y; y >= end.Y; y--)
                                    {
                                        SetPixels(x, y, color, brushThickness);
                                        if (cnt++ == changeStep && x != end.X)
                                        {
                                            SetPixels(x + 1, y, color, brushThickness);
                                            start.Y = y - 1;
                                            break;
                                        }

                                    }
                                    cnt = 1;
                                }
                            }
                        }
                        #endregion
                    }
                }
            }

            for (int r = 0; r < m_nPixelHeight; r++)
            {
                for (int w = 0; w < m_nPixelWidth; w++)
                {
                    m_pixels[r * m_nPixelWidth + w] = m_pixelData[r, w];
                }
            }
            TeachingImageCanvas.Strokes.Clear();
            TeachingImageCanvas.Children.Clear();
            TeachingImageCanvas.Background = new ImageBrush(BitmapSource.Create(m_nPixelWidth, m_nPixelHeight, 96, 96, PixelFormats.Indexed8, BitmapPalettes.Gray256, m_pixels, m_nPixelWidth));

            m_brushList.Clear();
            MainWindow.Log("PCS", SeverityLevel.DEBUG, "편집된 영상이 현재 Section 이미지에 적용되었습니다.");
        }

        private void btnInitImage_Click(object sender, RoutedEventArgs e)
        {
            TeachingImageCanvas.Strokes.Clear();
            TeachingImageCanvas.Children.Clear();
            TeachingImageCanvas.Background = new ImageBrush(m_CloneTeachingImage);

            m_CloneTeachingImage.CopyPixels(m_pixels, m_nPixelWidth, 0);
            for (int r = 0; r < m_nPixelHeight; r++)
            {
                for (int w = 0; w < m_nPixelWidth; w++)
                {
                    m_pixelData[r, w] = m_pixels[r * m_nPixelWidth + w];
                }
            }

            btnInitImage.IsChecked = false;
            m_brushList.Clear();
            MainWindow.Log("PCS", SeverityLevel.DEBUG, "영상 편집 이미지를 초기화시켰습니다.");
        }

        private void SetPixels(int x, int y, byte color, int brushThickness)
        {
            if (brushThickness == 0)
            {
                if (y > 0 && x > 0 && y < m_pixelData.GetLength(0) && x < m_pixelData.GetLength(1))
                {
                    m_pixelData[y, x] = color;
                }
            }
            else
            {
                // 두께를 적용하여 x, y 좌표를 칠한다.
                for (int by = y - brushThickness; by <= y + brushThickness; by++)
                {
                    for (int bx = x - brushThickness; bx <= x + brushThickness; bx++)
                    {
                        if (by > 0 && bx > 0 && by < m_pixelData.GetLength(0) && bx < m_pixelData.GetLength(1))
                        {
                            m_pixelData[by, bx] = color;
                        }
                    }
                }
            }
        }
        #endregion

        #region Set brush color & size.
        private void txtBrushColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (string.IsNullOrEmpty(txtBrushColor.Text))
            {
                rectFill.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                TeachingImageCanvas.DefaultDrawingAttributes.Color = Color.FromArgb(255, 0, 0, 0);
            }
            else
            {
                try
                {
                    rectFill.Fill = new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(txtBrushColor.Text), Convert.ToByte(txtBrushColor.Text), Convert.ToByte(txtBrushColor.Text)));
                    TeachingImageCanvas.DefaultDrawingAttributes.Color = Color.FromArgb(255, Convert.ToByte(txtBrushColor.Text), Convert.ToByte(txtBrushColor.Text), Convert.ToByte(txtBrushColor.Text));
                }
                catch
                {
                    rectFill.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    TeachingImageCanvas.DefaultDrawingAttributes.Color = Color.FromRgb(255, 255, 255);
                }
            }
        }

        private void txtBrushSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                this.TeachingImageCanvas.DefaultDrawingAttributes.Width = Convert.ToDouble(e.NewValue) * 2 - 1;
                this.TeachingImageCanvas.DefaultDrawingAttributes.Height = Convert.ToDouble(e.NewValue) * 2 - 1;
            }
            catch
            {
                if (e.OldValue != null)
                {
                    this.TeachingImageCanvas.DefaultDrawingAttributes.Width = Convert.ToDouble(e.OldValue) * 2 - 1;
                    this.TeachingImageCanvas.DefaultDrawingAttributes.Height = Convert.ToDouble(e.OldValue) * 2 - 1;
                }
                else
                {
                    this.TeachingImageCanvas.DefaultDrawingAttributes.Width = 9.0;
                    this.TeachingImageCanvas.DefaultDrawingAttributes.Height = 9.0;
                }
            }
        }
        #endregion

        #region Mouse event handlers.
        private void pnlInner_MouseUp(object sender, MouseButtonEventArgs e)
        {
            m_ptLastDragPoint = null;
            if (TeachingImageCanvas.EditingMode == InkCanvasEditingMode.Ink)
            {
                m_brushList.Add((int)TeachingImageCanvas.DefaultDrawingAttributes.Width / 2);
            }
        }

        private void TeachingImageCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Space))
            {
                m_ptLastDragPoint = e.GetPosition(svEditImage);
            }
            else if (btnColorPicker.IsChecked == true)
            {
                UpdateSelectedColor();
            }            
        }

        private void TeachingImageCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.Space) && btnColorPicker.IsChecked == true && e.LeftButton == MouseButtonState.Pressed)
            {
                UpdateSelectedColor();
            }
        }

        private void UpdateSelectedColor()
        {
            TeachingImageCanvas.EditingMode = InkCanvasEditingMode.None;

            rectFill.Fill = new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(txtGVValue.Text), Convert.ToByte(txtGVValue.Text), Convert.ToByte(txtGVValue.Text)));
            TeachingImageCanvas.DefaultDrawingAttributes.Color = Color.FromArgb(255, Convert.ToByte(txtGVValue.Text), Convert.ToByte(txtGVValue.Text), Convert.ToByte(txtGVValue.Text));
            txtBrushColor.Text = txtGVValue.Text;
        }

        private void pnlContainer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            m_ptLastContentMousePosition = GetContentMousePosition();
            Zoom(e.Delta);
            m_ptLastCenterOfViewport = GetCenterOfViewport();

            e.Handled = true; // blocking wheel event of svImageViewer control.
        }

        private void Zoom(double deltaValue)
        {
            if (deltaValue > 0)
            {
                sldrScale.Value *= 1.1;
            }
            else if (deltaValue == 0)
            {
                sldrScale.Value = m_fZoomToFitScale;
            }
            else
            {
                sldrScale.Value = (sldrScale.Value / 1.1 < m_fZoomToFitScale) ? m_fZoomToFitScale : sldrScale.Value / 1.1;
            }
        }

        private void pnlContainer_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (Keyboard.IsKeyDown(Key.Space) && Mouse.LeftButton == MouseButtonState.Pressed && m_ptLastDragPoint != null)
                {
                    System.Windows.Point ptCurrentByViewer = Mouse.GetPosition(svEditImage);
                    double fdeltaX = ptCurrentByViewer.X - m_ptLastDragPoint.Value.X;
                    double fdeltaY = ptCurrentByViewer.Y - m_ptLastDragPoint.Value.Y;

                    svEditImage.ScrollToHorizontalOffset(svEditImage.HorizontalOffset - fdeltaX);
                    svEditImage.ScrollToVerticalOffset(svEditImage.VerticalOffset - fdeltaY);

                    m_ptLastDragPoint = ptCurrentByViewer;
                }
                else
                {
                    System.Windows.Point ptCurrentByImage = Mouse.GetPosition(TeachingImageCanvas);
                    UpdateIndicator();
                                 
                    // GV 표시는 등록된 이미지위 범위 내에서만 동작하도록 한다.
                    if (!(ptCurrentByImage.X > TeachingImageCanvas.ActualWidth) &&
                        !(ptCurrentByImage.Y > TeachingImageCanvas.ActualHeight))
                    {
                        // Calculate GV Value.
                        byte[] pixel = new byte[1];

                        BitmapSource teachingImage = TeachingImage;
                        if (teachingImage != null)
                        {
                            teachingImage.CopyPixels(new Int32Rect((int)ptCurrentByImage.X, (int)ptCurrentByImage.Y, 1, 1), pixel, teachingImage.PixelWidth, 0);
                        }

                        //// Update X, Y, GV
                        txtGVValue.Text = pixel[0].ToString();
                        txtXPosition.Text = Convert.ToInt32(ptCurrentByImage.X).ToString();
                        txtYPosition.Text = Convert.ToInt32(ptCurrentByImage.Y).ToString();
                    }
                    else
                    {
                        this.txtGVValue.Text = "0";
                    }

                    if (btnFreeDrawing.IsChecked == true && e.LeftButton == MouseButtonState.Pressed)
                    {
                        Point currentPoint = e.GetPosition(TeachingImageCanvas);

                        if (currentPoint.X > TeachingImageCanvas.ActualWidth - 1 ||
                            currentPoint.Y > TeachingImageCanvas.ActualHeight - 1)
                        {
                            TeachingImageCanvas.EditingMode = InkCanvasEditingMode.None;
                            TeachingImageCanvas.Refresh();
                            TeachingImageCanvas.EditingMode = InkCanvasEditingMode.Ink;
                        }
                    }
                    else if (TeachingImageCanvas.EditingMode == InkCanvasEditingMode.EraseByStroke &&
                        e.LeftButton == MouseButtonState.Pressed)
                    {
                        Point p = e.GetPosition(TeachingImageCanvas);

                        foreach (UIElement element in TeachingImageCanvas.Children)
                        {
                            Line line = element as Line;
                            if (line != null)
                            {
                                IInputElement inputElementResult = line.InputHitTest(p);

                                if (inputElementResult != null)
                                {
                                    TeachingImageCanvas.Children.Remove(line);
                                    break;
                                }
                            }
                        }
                    }
                }                
            }
            catch
            {

            }
        }
        #endregion

        #region Button event handlers.
        private void btnColorPicker_Click(object sender, RoutedEventArgs e)
        {
            if (btnColorPicker.IsChecked == true)
            {
                txtReferenceLabel.Foreground = new SolidColorBrush(Colors.Black);
                txtReferenceLabel.Text = "색상 추출";

                this.btnFreeDrawing.IsChecked = false;
                this.btnEraser.IsChecked = false;

                TeachingImageCanvas.EditingMode = InkCanvasEditingMode.None;
            }
            else
            {
                txtReferenceLabel.Foreground = new SolidColorBrush(Colors.Black);
                txtReferenceLabel.Text = "브 러 시";

                this.btnColorPicker.IsChecked = false;
                this.btnFreeDrawing.IsChecked = true;
                this.btnEraser.IsChecked = false;

                TeachingImageCanvas.EditingMode = InkCanvasEditingMode.Ink;
            }
        }

        private void btnFreeDrawing_Click(object sender, RoutedEventArgs e)
        {
            txtReferenceLabel.Foreground = new SolidColorBrush(Colors.Black);
            txtReferenceLabel.Text = "브 러 시";

            if (btnFreeDrawing.IsChecked == true)
            {
                this.btnColorPicker.IsChecked = false;
                this.btnEraser.IsChecked = false;

                TeachingImageCanvas.EditingMode = InkCanvasEditingMode.Ink;
            }
            else
            {
                this.btnColorPicker.IsChecked = false;
                this.btnFreeDrawing.IsChecked = true;
                this.btnEraser.IsChecked = false;

                TeachingImageCanvas.EditingMode = InkCanvasEditingMode.Ink;
            }
        }

        private void btnEraser_Click(object sender, RoutedEventArgs e)
        {
            if (btnEraser.IsChecked == true)
            {
                txtReferenceLabel.Foreground = new SolidColorBrush(Colors.Red);
                txtReferenceLabel.Text = "지 우 개";

                this.btnColorPicker.IsChecked = false;
                this.btnFreeDrawing.IsChecked = false;

                TeachingImageCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
            }
            else
            {
                txtReferenceLabel.Foreground = new SolidColorBrush(Colors.Black);
                txtReferenceLabel.Text = "브 러 시";

                this.btnColorPicker.IsChecked = false;
                this.btnFreeDrawing.IsChecked = true;
                this.btnEraser.IsChecked = false;

                TeachingImageCanvas.EditingMode = InkCanvasEditingMode.Ink;
            }
        }
        #endregion

        #region OK & Cancel button events.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            ApplyImage();
            BitmapSource editedSource = TeachingImage;
            if (editedSource != null)
            {
                m_ptrParentCtrl.ThumbnailViewer.SetSourceImage(m_ptrParentCtrl);
                if (m_ptrParentCtrl.SelectedSection != null)
                {
                    if (!m_ptrParentCtrl.SelectedSection.IsTempView[(int)m_ptrParentCtrl.SelectedChannel])
                        m_ptrParentCtrl.SelectedSection.SetBitmapSource(editedSource, (int)m_ptrParentCtrl.SelectedChannel);
                    else
                        m_ptrParentCtrl.SelectedSection.SetTempBitmapSource(editedSource, (int)m_ptrParentCtrl.SelectedChannel);
                }
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWithCancel();
        }

        private void CloseWithCancel()
        {
            TeachingImageCanvas.Background = new ImageBrush(m_CloneTeachingImage);
            this.Close();
        }
        #endregion

        #region Zooming function.
        private void zoomBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender as Button == null)
            {
                return;
            }

            string strTag = (sender as Button).Tag.ToString();

            if (!string.IsNullOrEmpty(strTag))
            {
                if (strTag == "IN")
                {
                    Zoom(100);
                }
                else if (strTag == "OUT")
                {
                    Zoom(-100);
                }
                else // ZOOM_TO_FIT
                {
                    Zoom(0);
                }
            }
        }

        private System.Windows.Point GetContentMousePosition()
        {
            if (TeachingImageCanvas == null)
            {
                return new System.Windows.Point(0, 0);
            }
            else
            {
                System.Windows.Point ptContentMousePosition = Mouse.GetPosition(TeachingImageCanvas);

                return ptContentMousePosition;
            }
        }

        private System.Windows.Point GetCenterOfViewport()
        {
            if (TeachingImageCanvas == null)
            {
                return new System.Windows.Point(0, 0);
            }
            else
            {
                System.Windows.Point ptCenterOfViewport = new System.Windows.Point(svEditImage.ViewportWidth / 2, svEditImage.ViewportHeight / 2);
                System.Windows.Point ptTranslatedCenterOfViewport = svEditImage.TranslatePoint(ptCenterOfViewport, TeachingImageCanvas);

                return ptTranslatedCenterOfViewport;
            }
        }

        private void svEditImage_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (TeachingImageCanvas == null) return;
            
            if (e.ViewportWidthChange != 0 || e.ViewportHeightChange != 0)
            {
                m_ptLastCenterOfViewport = GetCenterOfViewport();
            }

            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                System.Windows.Point? ptBefore = null;
                System.Windows.Point? ptCurrent = null;

                if (!m_ptLastContentMousePosition.HasValue && m_ptLastCenterOfViewport.HasValue)
                {
                    ptBefore = m_ptLastCenterOfViewport;
                    ptCurrent = GetCenterOfViewport();
                }
                else
                {
                    ptBefore = m_ptLastContentMousePosition;
                    ptCurrent = GetContentMousePosition();

                    m_ptLastContentMousePosition = null;
                }

                if (ptBefore.HasValue)
                {
                    double fDeltaXInTargetPixels = ptCurrent.Value.X - ptBefore.Value.X;
                    double fDeltaYInTargetPixels = ptCurrent.Value.Y - ptBefore.Value.Y;

                    double fMultiplicatorX = e.ExtentWidth / TeachingImageCanvas.ActualWidth;
                    double fMultiplicatorY = e.ExtentHeight / TeachingImageCanvas.ActualHeight;

                    double newOffsetX = svEditImage.HorizontalOffset - fDeltaXInTargetPixels * fMultiplicatorX;
                    double newOffsetY = svEditImage.VerticalOffset - fDeltaYInTargetPixels * fMultiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY)) return;
                    
                    svEditImage.ScrollToHorizontalOffset(newOffsetX);
                    svEditImage.ScrollToVerticalOffset(newOffsetY);
                }
            }
            else
            {
                m_ptLastCenterOfViewport = GetCenterOfViewport();
            }
        }
        #endregion
    }
}
