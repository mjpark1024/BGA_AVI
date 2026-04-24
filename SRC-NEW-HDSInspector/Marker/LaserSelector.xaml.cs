using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Marker
{
    /// <summary>
    /// LaserSelector.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LaserSelector : UserControl
    {
        #region Field
        Size m_ApparentImageSize = new Size(0, 0);
        Point m_Origin = new Point(0, 0);
        Rectangle rectBound = new Rectangle();
        int sp = 50;
        int m_UnitNumX = 10;
        int m_UnitNumY = 4;
        double m_nWidth;
        double m_nHeight;
        double Size_X;
        double Size_Y;
        int space;
        Color[,] unitColor;
        Color[] HorColor;
        Color[] VerColor;
        bool[,] unitvalue;
        bool[] horvalue;
        bool[] vervalue;
        bool m_bAll = false;
        Rect unitrect;
        Rect horrect;
        Rect verrect;
        Rect allrect;
        Pen DrawPen = new Pen(new SolidColorBrush(Colors.Gray), 1);// 그리는 펜
        #endregion

        #region Property


        public Point Origin
        {
            get { return m_Origin; }
            set
            {
                m_Origin = value;
            }
        }

        public int UnitNumX
        {
            get { return m_UnitNumX; }
            set
            {
                m_UnitNumX = value;
                Init();
            }
        }

        public int UnitNumY
        {
            get { return m_UnitNumY; }
            set
            {
                m_UnitNumY = value;
                Init();
            }
        }

        public Size ApparentImageSize
        {
            get { return m_ApparentImageSize; }
        }
        #endregion
        public LaserSelector()
        {
            InitializeComponent();
            Init();
        }
        private void CalcSize()
        {
            m_nWidth = this.LaserMap.Width - 10 - sp;
            m_nHeight = this.LaserMap.Height - 10 - sp;
            space = 3;
            Size_X = (m_nWidth - (space * 2)) / m_UnitNumX;
            Size_Y = (m_nHeight - (space * 2)) / m_UnitNumY;
        }

        private void setValue(int X, int Y)
        {
            //int i = (Y) + (X * m_UnitNumY) + 1;
            //MessageBox.Show(i.ToString());
            //Rectangle rect = (Rectangle)this.LaserMap.Children[i];
            if (unitvalue[X, Y])
            {
                //rect.Fill = new SolidColorBrush(Colors.GhostWhite);
                unitColor[X, Y] = Colors.GhostWhite;
                unitvalue[X, Y] = false;
            }
            else
            {
                //rect.Fill = new SolidColorBrush(Colors.Blue);
                unitColor[X, Y] = Colors.Blue;
                unitvalue[X, Y] = true;
            }
            paint();
        }

        private void setHorValue(int X)
        {
            Color c;
            //MessageBox.Show(X.ToString());
            if (horvalue[X])
            {
                horvalue[X] = false;
                c = Colors.GhostWhite;
            }
            else
            {
                horvalue[X] = true;
                c = Colors.Blue;
            }
            //MessageBox.Show("bb");
            for (int i = 0; i < m_UnitNumY; i++)
            {
                unitColor[X, i] = c;
                //Rectangle rect = (Rectangle)this.LaserMap.Children[(i) + (X * m_UnitNumY) +1];
                //rect.Fill = new SolidColorBrush(c);
            }
            paint();
        }

        private void SetAll()
        {
            Color c;
            if (m_bAll)
            {
                m_bAll = false;
                c = Colors.GhostWhite;
            }
            else
            {
                m_bAll = true;
                c = Colors.Blue;
            }
            for (int i = 0; i < m_UnitNumX; i++)
            {
                for (int j = 0; j < m_UnitNumY; j++)
                {
                    unitColor[i, j] = c;
                }
            }
            paint();
        }

        private void setVerValue(int Y)
        {
            Color c;
            if (vervalue[Y])
            {
                vervalue[Y] = false;
                c = Colors.GhostWhite;
            }
            else
            {
                vervalue[Y] = true;
                c = Colors.Blue;
            }
            for (int i = 0; i < m_UnitNumX; i++)
            {
                unitColor[i, Y] = c;
                //Rectangle rect = (Rectangle)this.LaserMap.Children[(Y) + (i * m_UnitNumY) +1];
                //rect.Fill = new SolidColorBrush(c);
            }
            paint();
        }

        public void Init()
        {
            unitColor = new Color[m_UnitNumX, m_UnitNumY];
            VerColor = new Color[m_UnitNumY];
            HorColor = new Color[m_UnitNumX];
            unitvalue = new bool[m_UnitNumX, m_UnitNumY];
            vervalue = new bool[m_UnitNumY];
            horvalue = new bool[m_UnitNumX];
            for (int i = 0; i < m_UnitNumX; i++)
            {
                for (int j = 0; j < m_UnitNumY; j++)
                {
                    unitColor[i, j] = Colors.GhostWhite;
                    unitvalue[i, j] = false;
                }
            }
            for (int i = 0; i < m_UnitNumY; i++)
            {
                VerColor[i] = Colors.GhostWhite;
                vervalue[i] = false;
            }
            for (int i = 0; i < m_UnitNumX; i++)
            {
                HorColor[i] = Colors.GhostWhite;
                horvalue[i] = false;
            }
            m_bAll = false;
            paint();
        }

        private void paintVer()
        {
            try
            {
                Rectangle Bound = new Rectangle();
                Bound.Width = sp - 5;
                Bound.Height = this.LaserMap.Height - 5 - sp;
                Bound.Stroke = new SolidColorBrush(Colors.Gray);
                Bound.Fill = new SolidColorBrush(Colors.Gainsboro);
                Bound.Opacity = 0.8;
                Bound.RadiusX = 5;
                Bound.RadiusY = 5;
                Canvas.SetLeft(Bound, 1);
                Canvas.SetTop(Bound, sp);
                this.LaserMap.Children.Add(Bound);

                Point a = new Point(15, space + sp + 5);
                Point b = new Point(15 + sp - 24, (m_UnitNumY * Size_Y) + sp + 5);
                verrect = new Rect(a, b);

                double y;
                for (int i = 0; i < m_UnitNumY; i++)
                {
                    y = space + (i * Size_Y) + sp + 5;
                    Rectangle rect = new Rectangle();
                    rect.Width = (sp - 24);
                    rect.Height = (Size_Y - space);
                    rect.Stroke = new SolidColorBrush(Colors.Gray);
                    rect.Fill = new SolidColorBrush(VerColor[i]);
                    rect.Opacity = 0.8;
                    rect.RadiusX = 3;
                    rect.RadiusY = 3;
                    Canvas.SetLeft(rect, 15);
                    Canvas.SetTop(rect, y);
                    this.LaserMap.Children.Add(rect);
                    y = space + (i * Size_Y) + 2 + sp + 5;
                    TextBlock Labeltxt1 = new TextBlock();
                    Labeltxt1.Margin = new Thickness(3, y, 11, y + 8);
                    Labeltxt1.Text = (i + 1).ToString();
                    Labeltxt1.FontSize = 10;
                    this.LaserMap.Children.Add(Labeltxt1);

                }
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void paintHor()
        {
            try
            {
                Rectangle Bound = new Rectangle();
                Bound.Width = this.LaserMap.Width - 5 - sp;
                Bound.Height = sp - 5;
                Bound.Stroke = new SolidColorBrush(Colors.Gray);
                Bound.Fill = new SolidColorBrush(Colors.Gainsboro);
                Bound.Opacity = 0.8;
                Bound.RadiusX = 5;
                Bound.RadiusY = 5;
                Canvas.SetLeft(Bound, sp);
                Canvas.SetTop(Bound, 1);
                this.LaserMap.Children.Add(Bound);
                double x;

                Point a = new Point(space + sp + 5, 15);
                Point b = new Point((m_UnitNumX * Size_X) + sp + 5, 15 + sp - 24);
                horrect = new Rect(a, b);

                for (int i = 0; i < m_UnitNumX; i++)
                {
                    x = space + (i * Size_X) + sp + 5;
                    Rectangle rect = new Rectangle();
                    rect.Width = (Size_X - space);
                    rect.Height = (sp - 24);
                    rect.Stroke = new SolidColorBrush(Colors.Gray);
                    rect.Fill = new SolidColorBrush(HorColor[i]);
                    rect.Opacity = 0.8;
                    rect.RadiusX = 3;
                    rect.RadiusY = 3;
                    Canvas.SetLeft(rect, x);
                    Canvas.SetTop(rect, 15);
                    this.LaserMap.Children.Add(rect);
                    if (m_UnitNumX > 50)
                    {
                        if ((i % 5) == 0)
                        {
                            x = space + (i * Size_X) + 2 + sp + 5;
                            TextBlock Labeltxt1 = new TextBlock();
                            Labeltxt1.Margin = new Thickness(x, 3, x + 8, 11);
                            Labeltxt1.Text = (i + 1).ToString();
                            Labeltxt1.FontSize = 10;
                            this.LaserMap.Children.Add(Labeltxt1);
                        }
                    }
                    else
                    {
                        x = space + (i * Size_X) + 2 + sp + 5;
                        TextBlock Labeltxt1 = new TextBlock();
                        Labeltxt1.Margin = new Thickness(x, 3, x + 8, 11);
                        Labeltxt1.Text = (i + 1).ToString();
                        Labeltxt1.FontSize = 10;
                        this.LaserMap.Children.Add(Labeltxt1);
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void paintAll()
        {
            try
            {
                Rectangle rect = new Rectangle();
                rect.Width = sp - 3;
                rect.Height = sp - 3;
                rect.Stroke = new SolidColorBrush(Colors.Gray);
                rect.Fill = new SolidColorBrush(Colors.Gainsboro);
                rect.Opacity = 0.8;
                rect.RadiusX = 5;
                rect.RadiusY = 5;
                Canvas.SetLeft(rect, 1);
                Canvas.SetTop(rect, 1);
                this.LaserMap.Children.Add(rect);
                Point a = new Point(1, 1);
                Point b = new Point(47, 47);
                allrect = new Rect(a, b);
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void paintMap()
        {
            try
            {
                rectBound.Width = this.LaserMap.Width - 3 - sp;
                rectBound.Height = this.LaserMap.Height - 3 - sp;
                rectBound.Stroke = new SolidColorBrush(Colors.Gray);
                rectBound.Fill = new SolidColorBrush(Colors.Gainsboro);
                rectBound.Opacity = 0.8;
                rectBound.RadiusX = 5;
                rectBound.RadiusY = 5;
                Canvas.SetLeft(rectBound, sp);
                Canvas.SetTop(rectBound, sp);
                this.LaserMap.Children.Add(rectBound);
                double x;
                double y;
                Point a = new Point(space + sp + 5, space + sp + 5);
                Point b = new Point((m_UnitNumX * Size_X) + sp + 5, (m_UnitNumY * Size_Y) + sp + 5);
                unitrect = new Rect(a, b);
                for (int i = 0; i < m_UnitNumX; i++)
                {

                    for (int j = 0; j < m_UnitNumY; j++)
                    {
                        x = space + (i * Size_X) + sp + 5;
                        y = space + (j * Size_Y) + sp + 5;
                        AddUnit(x, y, i, j);
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void paint()
        {
            CalcSize();
            this.LaserMap.Children.Clear();
            paintMap();
            paintHor();
            paintVer();
            paintAll();
        }

        private void AddUnit(double x, double y, int i, int j)
        {
            Rectangle rect = new Rectangle();
            rect.Width = (Size_X - space);
            rect.Height = (Size_Y - space);
            rect.Stroke = new SolidColorBrush(Colors.Gray);
            rect.Fill = new SolidColorBrush(unitColor[i, j]);
            rect.Opacity = 0.8;
            rect.RadiusX = 3;
            rect.RadiusY = 3;
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);

            this.LaserMap.Children.Add(rect);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LaserMap.Width = this.Width - 10;
            LaserMap.Height = this.Height - 10;
            Init();
            //SetUnitValue(new Point(400, 200));
            //SetHor(new Point(400,0));
            //SetVer(new Point(0, 200));
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(this.LaserMap);
            switch (PointBoundCheck(pos))
            {
                case 0:
                    SetHor(pos);
                    break;
                case 1:
                    SetVer(pos);
                    break;
                case 2:
                    SetUnitValue(pos);
                    break;
                case 3:
                    SetAll();
                    break;
            }

        }

        private void SetUnitValue(Point pos)
        {
            int x = System.Convert.ToInt32(Math.Ceiling((pos.X - unitrect.Left) / (Size_X + space)));
            int y = System.Convert.ToInt32(Math.Ceiling((pos.Y - unitrect.Top) / (Size_Y + space)));
            setValue(x, y);
        }

        private void SetHor(Point pos)
        {
            int x = System.Convert.ToInt32(Math.Ceiling((pos.X - unitrect.Left) / (Size_X + space)));
            setHorValue(x);
        }

        private void SetVer(Point pos)
        {
            int y = System.Convert.ToInt32(Math.Ceiling((pos.Y - unitrect.Top) / (Size_Y + space)));
            setVerValue(y);
        }

        private int PointBoundCheck(Point pos)
        {
            if ((pos.X > horrect.Left) && (pos.X < horrect.Right) && (pos.Y > horrect.Top) && (pos.Y < horrect.Bottom))
                return 0;
            if ((pos.X > verrect.Left) && (pos.X < verrect.Right) && (pos.Y > verrect.Top) && (pos.Y < verrect.Bottom))
                return 1;
            if ((pos.X > unitrect.Left) && (pos.X < unitrect.Right) && (pos.Y > unitrect.Top) && (pos.Y < unitrect.Bottom))
                return 2;
            if ((pos.X > allrect.Left) && (pos.X < allrect.Right) && (pos.Y > allrect.Top) && (pos.Y < allrect.Bottom))
                return 2;

            return -1;
        }

        public string GetLaserString(out int cnt)
        {
            string str = "";
            cnt = 0;
            for (int i = 0; i < m_UnitNumX; i++)
            {
                for (int j = 0; j < m_UnitNumY; j++)
                {
                    if (unitvalue[i, j])
                    {
                        str = str + "1;";
                        cnt++;
                    }
                    else str = str + "0";
                }
            }
            return str;
        }
    }
}
