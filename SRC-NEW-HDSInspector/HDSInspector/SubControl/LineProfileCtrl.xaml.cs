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
 * @file  LineProfileCtrl.xaml.cs
 * @brief 
 *  It draws Line profile.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.08.01
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.01 First creation.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HDSInspector
{
    /// <summary>   Interaction logic for LineProfileCtrl.xaml. </summary>
    /// <remarks>   suoow2, 2014-09-24. </remarks>
    public partial class LineProfileCtrl : UserControl
    {
        // 컨트롤이 다이얼로그에 올라가는 형식에 따라 Width, Height 값이 잘못 반영되는 경우가 발생하여
        // Width와 Height를 직접 지정하여 처리하였다.
        private readonly int CONTROL_WIDTH = 350;
        private readonly int CONTROL_HEIGHT = 180;
        //private readonly int MAX_SOURCE_WIDTH = 2000;
        private readonly int SAMPLING_RATE = 5; // n픽셀 단위로 한번씩 추출.

        #region Private member variables.
        private Point[] m_ptLineProfileDataList; // X,Y interval에 의해 측정된 Line Profile 좌표리스트
        private static Path m_LineProfilePath = new Path(); // 좌표리스트를 이용하여 화면에 Line Profile을 그려낸다.

        private double m_fIntervalX = 0.0;
        private double m_fIntervalY = 0.0;

        private static double m_fMarginX = 30;
        private static double m_fMarginY = 20;

        private int m_nPixelWidth = 0;
        #endregion

        #region Constructor & InitializeDialog
        public LineProfileCtrl()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.SizeChanged += (s, e) =>
            {
                LineProfile.RenderTransform = new ScaleTransform(this.ActualWidth / CONTROL_WIDTH, this.ActualHeight / CONTROL_HEIGHT);
            };
        }

        private void InitializeDialog()
        {
            int nDottedLineProfileGap = 50;
            int nDashLineCnt = (255 / nDottedLineProfileGap) + 1;
            double fLineProfileMaxHeight = (CONTROL_HEIGHT - m_fMarginY * 2) * 250.0 / 255.0;

            Line DotLine;
            TextBlock Label;
            for (int i = 0; i < nDashLineCnt - 1; i++)
            {
                DotLine = new Line();
                DotLine.X1 = m_fMarginX - 1;
                DotLine.X2 = CONTROL_WIDTH - 1;
                DotLine.Y1 = (CONTROL_HEIGHT - m_fMarginY) - (fLineProfileMaxHeight) * (i / ((double)nDashLineCnt - 1));
                DotLine.Y2 = (CONTROL_HEIGHT - m_fMarginY) - (fLineProfileMaxHeight) * (i / ((double)nDashLineCnt - 1));
                DotLine.StrokeThickness = 2;
                DotLine.StrokeDashArray = new DoubleCollection() { 1, 1 };
                DotLine.Stroke = new SolidColorBrush(Colors.DarkGray);
                LineProfile.Children.Add(DotLine);

                Label = new TextBlock();
                Label.Text = (nDottedLineProfileGap * i).ToString();
                Canvas.SetLeft(Label, 5);
                Canvas.SetTop(Label, (CONTROL_HEIGHT - m_fMarginY) - (fLineProfileMaxHeight) * (i / ((double)nDashLineCnt - 1)));
                LineProfile.Children.Add(Label);
            }

            DotLine = new Line();
            DotLine.X1 = m_fMarginX - 1;
            DotLine.X2 = CONTROL_WIDTH - 1;
            DotLine.Y1 = m_fMarginY;
            DotLine.Y2 = m_fMarginY;
            DotLine.StrokeThickness = 2;
            DotLine.StrokeDashArray = new DoubleCollection() { 1, 1 };
            DotLine.Stroke = new SolidColorBrush(Colors.DarkGray);
            LineProfile.Children.Add(DotLine);

            Label = new TextBlock();
            Label.Text = "255";
            Canvas.SetLeft(Label, 5);
            Canvas.SetTop(Label, m_fMarginY);
            LineProfile.Children.Add(Label);

            // Draw X-Axis line of LineProfile
            Line AxisX = new Line { X1 = m_fMarginX - 1, X2 = CONTROL_WIDTH - 1, 
                Y1 = CONTROL_HEIGHT - m_fMarginY, Y2 = CONTROL_HEIGHT - m_fMarginY, 
                StrokeThickness = 2, Stroke = new SolidColorBrush(Colors.Black) };
            LineProfile.Children.Add(AxisX);

            // Draw Y-Axis line of LineProfile
            Line AxisY = new Line { X1 = m_fMarginX - 1, X2 = m_fMarginX - 1, 
                Y1 = m_fMarginY - 1, Y2 = CONTROL_HEIGHT - m_fMarginY, 
                StrokeThickness = 2, Stroke = new SolidColorBrush(Colors.Black) };
            LineProfile.Children.Add(AxisY);
        }
        #endregion

        public void Refresh()
        {
            LineProfile.Children.Clear();
            InitializeDialog();
        }

        public void SetLineProfileSource(BitmapSource source)
        {
            if (source == null)
            {
                return;
            }

            // TBD : 샘플링을 통한 LineProfile 그리기.

            //if (source.PixelWidth < MAX_SOURCE_WIDTH)
            //{
                m_nPixelWidth = source.PixelWidth;
            //}
            //else
            //{
            //    m_nPixelWidth = source.PixelWidth / SAMPLING_RATE;
            //}

            m_ptLineProfileDataList = new Point[m_nPixelWidth];
            m_LineProfilePath.Fill = new SolidColorBrush(Color.FromArgb(255, 68, 68, 68));

            m_fIntervalX = 1.0 / m_nPixelWidth * (CONTROL_WIDTH - m_fMarginX);
            m_fIntervalY = 1.0 / 255.0 * (CONTROL_HEIGHT - m_fMarginY * 2);
        }

        public void DrawLineProfile(byte[] lineData)
        {
            if (lineData == null || m_ptLineProfileDataList == null)
            {
                return;
            }

            int nIndex = 0;
            int stepCount = 0;
            foreach (byte data in lineData)
            {
                if (stepCount % SAMPLING_RATE != 0)
                {
                    stepCount++;
                    continue;
                }

                try
                {
                    if (nIndex >= m_ptLineProfileDataList.Length)
                    {
                        break;
                    }
                    m_ptLineProfileDataList[nIndex].X = nIndex * m_fIntervalX + m_fMarginX;
                    m_ptLineProfileDataList[nIndex++].Y = CONTROL_HEIGHT - 1 - data * m_fIntervalY - m_fMarginY;

                    stepCount = (stepCount == 5) ? 0 : stepCount;
                }
                catch
                {

                }
            }

            StreamGeometry lineProfile = new StreamGeometry();
            using (StreamGeometryContext ctx = lineProfile.Open())
            {
                ctx.BeginFigure(new Point(m_ptLineProfileDataList[0].X, CONTROL_HEIGHT - m_fMarginY), true, true);
                for (int k = 0; k < m_nPixelWidth; k++)
                {
                    ctx.LineTo(m_ptLineProfileDataList[k], false, true);
                }
                ctx.LineTo(new Point(m_ptLineProfileDataList[m_nPixelWidth - 1].X, CONTROL_HEIGHT - m_fMarginY), true, true);
            }
            lineProfile.Freeze();

            this.LineProfile.Children.Remove(m_LineProfilePath);
            m_LineProfilePath.Data = lineProfile;
            this.LineProfile.Children.Add(m_LineProfilePath);
        }
    }
}
