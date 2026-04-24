using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics;

namespace HDSInspector
{
    public partial class StripMap : UserControl
    {
        #region Member Variables
        private InspectionMonitoring m_InspectionMonitoring;
        private Rectangle m_RectOutLine = new Rectangle();
        private Rectangle[,] m_RectUnit;
        private bool[,] m_bUnit;

        private int m_nMargin = 15;
        private int m_nUnitX = 30;
        private int m_nUnitY = 6;

        private double m_fWidth;
        private double m_fHeight;

        private double m_fSizeX;
        private double m_fSizeY;

        private double m_fSpace = 2.0;

        private int nCount = 0;
        private System.Threading.Thread timer;

        public bool IsLabelVisible
        {
            get { return m_bIsLabelVisible; }
            set { m_bIsLabelVisible = value; }
        }
        private bool m_bIsLabelVisible = true;
        #endregion

        #region Constructor & Initializer
        public StripMap()
        {
            InitializeComponent();
            InitializeEvent();
            Paint();
        }

        private void InitializeEvent()
        {
            this.SizeChanged += StripMap_SizeChanged;
        }

        private void StripMap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            stripmap.RenderTransform = new ScaleTransform(this.ActualWidth / stripmap.Width, this.ActualHeight / stripmap.Height);
        }

        private void InitializeMapData()
        {
            if (m_nUnitX > 0 && m_nUnitY > 0)
            {
                m_RectUnit = new Rectangle[m_nUnitX, m_nUnitY];
                m_bUnit = new bool[m_nUnitX, m_nUnitY];
                for (int i = 0; i < m_nUnitX; i++)
                {
                    for (int j = 0; j < m_nUnitY; j++)
                    {
                        m_RectUnit[i, j] = new Rectangle();
                        m_bUnit[i, j] = true;
                    }
                }
            }
        }
        #endregion

        #region Set Stripmap Color.

        public void StartInspect()
        {
            nCount = 0;
            if (timer != null)
            {
                timer.Abort();
                System.Threading.Thread.Sleep(50);
                timer = null;
            }
            timer = new System.Threading.Thread(OnTimer);
            timer.Start();
        }

        /// <summary>   Executes the timer elapsed action. </summary>
        private void OnTimer()
        {
            while (nCount < 50)
            {
                Action action = delegate
                {
                    LinearGradientBrush gb = new LinearGradientBrush(Colors.White, Colors.Black, new Point(0.0, 0.5), new Point(0.04 * (double)nCount, 0.5));
                    m_RectOutLine.Fill = gb;

                }; m_InspectionMonitoring.Dispatcher.Invoke(action);
                nCount++;
                System.Threading.Thread.Sleep(30);
            }
        }

        public void SetColor(int x, int y, Color color, string SectionType)
        {
            if (SectionType == PCS.ModelTeaching.SectionTypeCode.OUTER_REGION ||
                SectionType == PCS.ModelTeaching.SectionTypeCode.ID_REGION ||
                SectionType == PCS.ModelTeaching.SectionTypeCode.STRIP_REGION ||
                SectionType == "")
            {
                m_RectOutLine.Stroke = new SolidColorBrush(Colors.Orange);
                m_RectOutLine.StrokeThickness = 6.0;
            }
            else
            {
                // Strip Map 특정 위치를 '특정' 색상으로 칠한다.
                if (m_bUnit[x, y])
                {
                    m_RectUnit[x, y].Fill = new SolidColorBrush(color);
                    m_bUnit[x, y] = false;
                }
            }
        }

        public void ClearColor()
        {
            if (m_RectUnit != null)
            {
                m_RectOutLine.Stroke = new SolidColorBrush(Colors.Gray);
                m_RectOutLine.StrokeThickness = 1.0;

                // Strip Map 전체를 GhostWhite 색상으로 칠한다.
                for (int i = 0; i < m_nUnitX; i++)
                {
                    for (int j = 0; j < m_nUnitY; j++)
                    {
                        m_RectUnit[i, j].Fill = new SolidColorBrush(Colors.GhostWhite);
                        m_bUnit[i, j] = true;
                    }
                }
            }
        }

        public void GoodColor()
        {
            if (m_RectUnit != null)
            {
                m_RectOutLine.Stroke = new SolidColorBrush(Colors.Gray);
                m_RectOutLine.StrokeThickness = 1.0;
                // Strip Map 전체를 Lime 색상으로 칠한다.
                for (int i = 0; i < m_nUnitX; i++)
                {
                    for (int j = 0; j < m_nUnitY; j++)
                    {
                        m_RectUnit[i, j].Fill = new SolidColorBrush(Color.FromRgb(0x05, 0xB4, 0x05));
                        m_bUnit[i, j] = true;
                    }
                }
            }
        }

        public void GoodColor(int nYPos, int nEndY)
        {
            if (m_RectUnit != null)
            {
                // Strip Map 전체를 Lime 색상으로 칠한다.
                for (int i = nYPos; i <= nEndY; i++)
                {
                    for (int j = 0; j < m_nUnitX; j++)
                    {
                        if (m_bUnit[j,i])
                        m_RectUnit[j, i].Fill = new SolidColorBrush(Color.FromRgb(0x05, 0xB4, 0x05));
                    }
                }
            }
        }
        #endregion

        #region Paint strip map.
        private void CalcSize()
        {
            if (m_nUnitX > 0 && m_nUnitY > 0)
            {
                m_fWidth = this.stripmap.Width - (m_nMargin * 2);
                m_fHeight = this.stripmap.Height - (m_nMargin * 2);

                m_fSizeX = (m_fWidth - (m_fSpace * 2)) / m_nUnitX;
                m_fSizeY = (m_fHeight - (m_fSpace * 2)) / m_nUnitY;
            }
        }

        private void Paint()
        {
            if (this.ActualWidth == 0.0 || this.ActualHeight == 0.0)
            {
                this.Measure(new Size(this.stripmap.Width, this.stripmap.Height));
                this.Arrange(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height));
            }

            this.stripmap.Width = this.ActualWidth;
            this.stripmap.Height = this.ActualHeight;
            this.stripmap.Children.Clear();

            InitializeMapData();
            CalcSize();
            PaintStrip();
            
            if (m_bIsLabelVisible)
                PaintLabel();

            stripmap.RenderTransform = new ScaleTransform(this.ActualWidth / stripmap.Width, this.ActualHeight / stripmap.Height);
        }

        private void PaintLabel()
        {
            try
            {
                double x = 0;
                double y = 0;
                double unitWidthHalf = m_fSizeX / 5 * 2;
                double unitHeightHalf = m_fSizeY / 3;
                double unitWidth = 0;
                double unitHeight = 0;

                #region X 축
                for (int i = 0; i < m_nUnitX; i++)
                {
                    // X축 유닛 갯수 30개 이상
                    if (m_nUnitX > 30)
                    {
                        //5 단위로 라벨 표시
                        if ((i % 5) == 0)
                        {
                            unitWidth = i * m_fSizeX;

                            #region Top X-axis Label
                            x = m_fSpace + unitWidth + m_nMargin + 2.5;
                            y = 5;

                            //X축 보정치
                            if (i >= 9)
                            {
                                x -= 1;

                                if (m_nUnitX > 50 && m_nUnitX < 56)
                                {
                                    x -= 1;
                                }
                                else if (m_nUnitX >= 56 && m_nUnitX < 86)
                                {
                                    x -= 2;
                                }
                                else if (m_nUnitX >= 86)
                                {
                                    x -= 3;
                                }
                            }

                            TextBlock Labeltxt1 = new TextBlock();
                            Labeltxt1.Margin = new Thickness(x, y, x + 8, y + 8);
                            Labeltxt1.Text = (i + 1).ToString();
                            Labeltxt1.FontSize = 8;

                            this.stripmap.Children.Add(Labeltxt1);
                            #endregion

                            #region Bottom X-axis Label
                            x = m_fSpace + unitWidth + m_nMargin + 2.5;
                            y = stripmap.Height - m_nMargin - 5;

                            // X 보정치
                            if (i >= 9)
                            {
                                x -= 1;

                                if (m_nUnitX > 50 && m_nUnitX < 56)
                                {
                                    x -= 1;
                                }
                                else if (m_nUnitX >= 56 && m_nUnitX < 86)
                                {
                                    x -= 2;
                                }
                                else if (m_nUnitX >= 86)
                                {
                                    x -= 3;
                                }
                            }

                            TextBlock Labeltxt2 = new TextBlock();
                            Labeltxt2.Margin = new Thickness(x, y, x + 8, y + 8);
                            Labeltxt2.Text = (i + 1).ToString();
                            Labeltxt2.FontSize = 8;

                            this.stripmap.Children.Add(Labeltxt2);
                            #endregion
                        }
                    }
                    else
                    {
                        unitWidth = i * m_fSizeX;

                        #region Top X-axis Label
                        x = m_fSpace + unitWidthHalf + unitWidth + m_nMargin;
                        y = 4;

                        // X 보정치
                        if (i >= 9)
                        {
                            x -= 2;

                            if (m_nUnitX > 20)
                            {
                                x -= 2;
                            }
                        }

                        if (m_nUnitX < 8)
                        {
                            unitWidthHalf = m_fSizeX / 2;
                            x = m_fSpace + unitWidthHalf + unitWidth + m_nMargin;
                        }

                        TextBlock Labeltxt1 = new TextBlock();
                        Labeltxt1.Margin = new Thickness(x, y, x + 8, y + 8);
                        Labeltxt1.Text = (i + 1).ToString();
                        Labeltxt1.FontSize = 8;

                        this.stripmap.Children.Add(Labeltxt1);
                        #endregion

                        #region Bottom X-axis Label
                        x = m_fSpace + unitWidthHalf + unitWidth + m_nMargin;
                        y = stripmap.Height - m_nMargin - 5;

                        // X 보정치
                        if (i >= 9)
                        {
                            x -= 2;

                            if (m_nUnitX > 20)
                            {
                                x -= 2;
                            }
                        }

                        if (m_nUnitX < 8)
                        {
                            unitWidthHalf = m_fSizeX / 2;
                            x = m_fSpace + unitWidthHalf + unitWidth + m_nMargin;
                        }
                        TextBlock Labeltxt2 = new TextBlock();
                        Labeltxt2.Margin = new Thickness(x, y, x + 8, y + 8);
                        Labeltxt2.Text = (i + 1).ToString();
                        Labeltxt2.FontSize = 8;

                        this.stripmap.Children.Add(Labeltxt2);
                        #endregion
                    }
                }
                #endregion

                #region Y 축
                for (int i = 0; i < m_nUnitY; i++)
                {
                    // X축 유닛 갯수 30개 이상
                    if (m_nUnitY > 30)
                    {
                        if ((i % 5) == 0)
                        {
                            unitHeight = i * m_fSizeY;

                            #region Left Y-axis Label
                            y = m_fSpace + unitHeight + m_nMargin;
                            x = 8.5;

                            // X 보정치
                            if (i >= 9)
                            {
                                x -= 3;
                            }
                            // Y 보정치
                            y -= 5.2;
                            
                            TextBlock Labeltxt1 = new TextBlock();
                            Labeltxt1.Margin = new Thickness(x, y, x + 8, y + 8);
                            Labeltxt1.Text = (i+1).ToString();
                            Labeltxt1.FontSize = 8;

                            this.stripmap.Children.Add(Labeltxt1);
                            #endregion

                            #region Right Y-axis label
                            y = m_fSpace + unitHeight + m_nMargin;
                            x = stripmap.Width - m_nMargin;

                            // X 보정치
                            if (i >= 9)
                            {
                                x -= 2;
                            }
                            // Y 보정치
                            y -= 5.2;

                            TextBlock Labeltxt2 = new TextBlock();
                            Labeltxt2.Margin = new Thickness(x, y, x + 8, y + 8);
                            Labeltxt2.Text = (i+1).ToString();
                            Labeltxt2.FontSize = 8;

                            this.stripmap.Children.Add(Labeltxt2);
                            #endregion
                        }
                    }
                    else
                    {
                        unitHeight = (i * m_fSizeY);

                        #region Left Y-axis Label
                        y = m_fSpace + unitHeight + m_nMargin;
                        x = 7;

                        // X 보정치
                        if (i >= 9)
                        {
                            x -= 2;
                        }
                        // Y 보정치
                        if (m_nUnitY <= 5)
                        {
                            y += unitHeightHalf;
                        }
                        else if (m_nUnitY >= 6 && m_nUnitY <= 9)
                        {
                            y += 3;
                        }
                        else if (m_nUnitY >= 12 && m_nUnitY <= 14)
                        {
                            y -= 1;
                        }
                        else if (m_nUnitY >= 15)
                        {
                            y -= 3.5;
                        }
                        TextBlock Labeltxt1 = new TextBlock();
                        Labeltxt1.Margin = new Thickness(x, y, x + 8, y + 8);
                        Labeltxt1.Text = (i+1).ToString();
                        Labeltxt1.FontSize = 8;

                        this.stripmap.Children.Add(Labeltxt1);
                        #endregion

                        #region Right Y-axis label
                        y = m_fSpace + unitHeight + m_nMargin;
                        x = stripmap.Width - m_nMargin;

                        // X 보정치
                        if (i >= 9)
                        {
                            x -= 2;
                        }
                        // Y 보정치
                        if (m_nUnitY <= 5)
                        {
                            y += unitHeightHalf;
                        }
                        else if (m_nUnitY >= 6 && m_nUnitY <= 9)
                        {
                            y += 3;
                        }
                        else if (m_nUnitY >= 12 && m_nUnitY <= 14)
                        {
                            y -= 1;
                        }
                        else if (m_nUnitY >= 15)
                        {
                            y -= 3.5;
                        }
                        TextBlock Labeltxt2 = new TextBlock();
                        Labeltxt2.Margin = new Thickness(x, y, x + 8, y + 8);
                        Labeltxt2.Text = (i+1).ToString();
                        Labeltxt2.FontSize = 8;

                        this.stripmap.Children.Add(Labeltxt2);
                        #endregion
                    }
                    

                }
                #endregion

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void PaintStrip()
        {
            try
            {
                #region Paint Out Line
                m_RectOutLine.Width = this.stripmap.Width - 4;
                m_RectOutLine.Height = this.stripmap.Height - 4;
                m_RectOutLine.Stroke = new SolidColorBrush(Colors.Gray);
                m_RectOutLine.Fill = new SolidColorBrush(Colors.Gainsboro);
                m_RectOutLine.Opacity = 0.8;
                m_RectOutLine.RadiusX = 5;
                m_RectOutLine.RadiusY = 5;
                Canvas.SetLeft(m_RectOutLine, 2);
                Canvas.SetTop(m_RectOutLine, 2);
                this.stripmap.Children.Add(m_RectOutLine);
                #endregion

                #region Paint Unit
                double fLeft;
                double fTop;

                for (int i = 0; i < m_nUnitX; i++)
                {
                    for (int j = 0; j < m_nUnitY; j++)
                    {
                        fLeft = m_fSpace + (i * m_fSizeX) + m_nMargin;
                        fTop = m_fSpace + (j * m_fSizeY) + m_nMargin;
                        AddUnit(fLeft, fTop, i, j);
                    }
                }
                #endregion
            }
            catch
            {
            }
        }

        private void AddUnit(double fLeft, double fTop, int i, int j)
        {
            m_RectUnit[i, j].Width = (m_fSizeX - m_fSpace);
            m_RectUnit[i, j].Height = (m_fSizeY - m_fSpace);
            m_RectUnit[i, j].Stroke = new SolidColorBrush(Colors.Gray);
            m_RectUnit[i, j].Fill = new SolidColorBrush(Colors.GhostWhite);
            m_RectUnit[i, j].Opacity = 0.8;
            m_RectUnit[i, j].RadiusX = 2;
            m_RectUnit[i, j].RadiusY = 2;

            Canvas.SetLeft(m_RectUnit[i, j], fLeft);
            Canvas.SetTop(m_RectUnit[i, j], fTop);

            this.stripmap.Children.Add(m_RectUnit[i, j]);
        }

        public void SetStripMap(int anUnitX, int anUnitY, InspectionMonitoring ptr)
        {
            m_InspectionMonitoring = ptr;
            m_nUnitX = anUnitX;
            m_nUnitY = anUnitY;

            Paint();
        }
        #endregion
    }
}
