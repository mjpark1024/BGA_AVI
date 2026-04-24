using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Common.Drawing
{
    public class Circle
    {
        public Point Position;
        public double Radian;
        public Circle()
        {
            Position = new Point();
            Radian = 0;
        }
        public Circle(Point pos, double radian)
        {
            Position = new Point(pos.X, pos.Y);
            Radian = radian;
        }
    }
    
    public abstract class  GraphicsEllipseBase : GraphicsBase
    {
        #region Class Members
        protected Circle circle;
        protected double rectangleLeft;
        protected double rectangleTop;
        protected double rectangleRight;
        protected double rectangleBottom;
        //protected bool isDummy;
        protected int motherID;
        #endregion Class Members

        public void CalcBoundaryRect()
        {
            boundaryRect.X = Math.Min(rectangleLeft, rectangleRight);
            boundaryRect.Y = Math.Min(rectangleTop, rectangleBottom);
            //boundaryRect.Width = Math.Abs(rectangleLeft - rectangleRight) + 1;
            //boundaryRect.Height = Math.Abs(rectangleTop - rectangleBottom) + 1;
            boundaryRect.Width = Math.Abs(rectangleLeft - rectangleRight);
            boundaryRect.Height = Math.Abs(rectangleTop - rectangleBottom);

            startPoint = boundaryRect.TopLeft;

            LeftProperty = (int)boundaryRect.Left;
            TopProperty = (int)boundaryRect.Top;
            WidthProperty = boundaryRect.Width;
            HeightProperty = boundaryRect.Height;
        }

        public void UpdateRect(Circle circle)
        {
            CircleInfo = new Circle(new Point(circle.Position.X, circle.Position.Y), circle.Radian); ;
            //rectangleLeft = afLeft;
            //rectangleTop = afTop;
            //rectangleRight = afRight;
            //rectangleBottom = afBottom;

            //this.CalcBoundaryRect();
        }

        #region Properties
        //public bool Dummy
       // {
       //     get { return isDummy; }
      //      set { isDummy = value; }
      //  }

        public int MotherROIID
        {
            get { return motherID; }
            set { motherID = value; }
        }

        public Circle CircleInfo
        {
            get { return circle; }
            set { 
                circle = value;
                rectangleLeft = circle.Position.X - circle.Radian;
                rectangleTop = circle.Position.Y - circle.Radian;
                rectangleRight = circle.Position.X + circle.Radian;
                rectangleBottom = circle.Position.Y + circle.Radian;
                CalcBoundaryRect();
            }
        }

        public Rect Rectangle
        {
            get
            {
                CalcBoundaryRect();

                return boundaryRect;
            }
        }

        public double Left
        {
            get
            {
                return rectangleLeft;
            }
            set
            {
                rectangleLeft = value;
                CalcBoundaryRect();
            }
        }

        public double Top
        {
            get
            {
                return rectangleTop;
            }
            set
            {
                rectangleTop = value;
                CalcBoundaryRect();
            }
        }

        public double Right
        {
            get
            {
                return rectangleRight;
            }
            set
            {
                rectangleRight = value;
                CalcBoundaryRect();
            }
        }

        public double Bottom
        {
            get
            {
                return rectangleBottom;
            }
            set
            {
                rectangleBottom = value;
                CalcBoundaryRect();
            }
        }
        #endregion Properties

        #region Overrides
        /// <summary>
        /// Get number of handles
        /// </summary>
        public override int HandleCount
        {
            get
            {
                return 8;
            }
        }

        /// <summary>
        /// Get handle point by 1-based number
        /// </summary>
        public override Point GetHandle(int handleNumber)
        {
            double x, y, xCenter, yCenter;
            
            xCenter = (rectangleRight + rectangleLeft) / 2;
            yCenter = (rectangleBottom + rectangleTop) / 2;
            x = rectangleLeft;
            y = rectangleTop;

            switch (handleNumber)
            {
                case 1:
                    x = rectangleLeft;
                    y = rectangleTop;
                    break;
                case 2:
                    x = xCenter;
                    y = rectangleTop;
                    break;
                case 3:
                    x = rectangleRight;
                    y = rectangleTop;
                    break;
                case 4:
                    x = rectangleRight;
                    y = yCenter;
                    break;
                case 5:
                    x = rectangleRight;
                    y = rectangleBottom;
                    break;
                case 6:
                    x = xCenter;
                    y = rectangleBottom;
                    break;
                case 7:
                    x = rectangleLeft;
                    y = rectangleBottom;
                    break;
                case 8:
                    x = rectangleLeft;
                    y = yCenter;
                    break;
            }

            return new Point(x, y);
        }

        /// <summary>
        /// Hit test.
        /// Return value: -1 - no hit
        ///                0 - hit anywhere
        ///                > 1 - handle number
        /// </summary>
        public override int MakeHitTest(Point point)
        {
            if (IsSelected)
            {
                for (int i = 1; i <= HandleCount; i++)
                {
                    if (GetHandleRectangle(i).Contains(point))
                        return i;
                }
            }

            if (Contains(point))
                return 0;

            return -1;
        }



        /// <summary>
        /// Get cursor for the handle
        /// </summary>
        public override Cursor GetHandleCursor(int handleNumber)
        {
            switch (handleNumber)
            {
                case 1:
                    return Cursors.SizeNWSE;
                case 2:
                    return Cursors.SizeNS;
                case 3:
                    return Cursors.SizeNESW;
                case 4:
                    return Cursors.SizeWE;
                case 5:
                    return Cursors.SizeNWSE;
                case 6:
                    return Cursors.SizeNS;
                case 7:
                    return Cursors.SizeNESW;
                case 8:
                    return Cursors.SizeWE;
                default:
                    return Cursors.Arrow;
            }
        }

        /// <summary>
        /// Move handle to new point (resizing)
        /// </summary>
        public override void MoveHandleTo(Point point, int handleNumber)
        {
            if (handleNumber > -1) return;
            if (Dummy) return;
            point.X = (point.X < 0) ? 0 : point.X;
            point.Y = (point.Y < 0) ? 0 : point.Y;

            double r = Math.Max(Math.Abs(point.X - CircleInfo.Position.X), Math.Abs(point.Y - CircleInfo.Position.Y));
            CircleInfo = new Circle(new Point(CircleInfo.Position.X, CircleInfo.Position.Y), r);
            //switch (handleNumber)
            //{
            //    case 1:
            //        rectangleLeft = point.X;
            //        rectangleTop = point.Y;
            //        break;
            //    case 2:
            //        rectangleTop = point.Y;
            //        break;
            //    case 3:
            //        rectangleRight = point.X;
            //        rectangleTop = point.Y;
            //        break;
            //    case 4:
            //        rectangleRight = point.X;
            //        break;
            //    case 5:
            //        rectangleRight = point.X;
            //        rectangleBottom = point.Y;
            //        break;
            //    case 6:
            //        rectangleBottom = point.Y;
            //        break;
            //    case 7:
            //        rectangleLeft = point.X;
            //        rectangleBottom = point.Y;
            //        break;
            //    case 8:
            //        rectangleLeft = point.X;
            //        break;
            //}
            //CalcBoundaryRect();
            RefreshDrawing();
        }

        /// <summary>
        /// Test whether object intersects with rectangle
        /// </summary>
        public override bool IntersectsWith(Rect rectangle)
        {
            return Rectangle.IntersectsWith(rectangle);
        }

        public override void Move(double deltaX, double deltaY, double maxWidth, double maxHeight)
        {
            if ((boundaryRect.Left + deltaX) < 0)
                deltaX = -boundaryRect.Left;
            if ((boundaryRect.Top + deltaY) < 0)
                deltaY = -boundaryRect.Top;
            if ((boundaryRect.Right + deltaX) >= maxWidth)
                deltaX = (maxWidth - 1) - boundaryRect.Right;
            if ((boundaryRect.Bottom + deltaY) >= maxHeight)
                deltaY = (maxHeight - 1) - boundaryRect.Bottom;
            
            rectangleLeft += deltaX;
            rectangleRight += deltaX;

            rectangleTop += deltaY;
            rectangleBottom += deltaY;

            CircleInfo = new Circle(new Point(rectangleLeft + CircleInfo.Radian, rectangleTop + CircleInfo.Radian), CircleInfo.Radian);

           // CalcBoundaryRect();
            RefreshDrawing();
        }

        /// <summary>
        /// Normalize rectangle
        /// </summary>
        public override void Normalize()
        {
            if (rectangleLeft > rectangleRight)
            {
                double tmp = rectangleLeft;
                rectangleLeft = rectangleRight;
                rectangleRight = tmp;
            }

            if (rectangleTop > rectangleBottom)
            {
                double tmp = rectangleTop;
                rectangleTop = rectangleBottom;
                rectangleBottom = tmp;
            }

            CalcBoundaryRect();
        }
        #endregion Overrides

        #region Binding Properties
        private int left;
        private int top;
        private double width;
        private double height;

        [XmlIgnore]
        public int LeftProperty
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
                Notify("LeftProperty");
            }
        }

        [XmlIgnore]
        public int TopProperty
        {
            get
            {
                return top;
            }
            set
            {
                top = value;
                Notify("TopProperty");
            }
        }

        [XmlIgnore]
        public double WidthProperty
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                Notify("WidthProperty");
            }
        }

        [XmlIgnore]
        public double HeightProperty
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                Notify("HeightProperty");
            }
        }
        #endregion
    }
}
    
