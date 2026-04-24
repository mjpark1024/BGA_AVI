using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using Common.Drawing.MarkingInformation;
using System.Xml.Serialization;

//2017.01.03 suoow.yeo new initialize

namespace Common.Drawing
{

    public class GraphicsEllipseMark : GraphicsEllipseBase
    {
        public bool white = false;
        //public bool Dummy = false;
        //public Circle circle;
        //protected int motherROIID = -1;
        #region Constructors
        public GraphicsEllipseMark(Circle circle,
            double lineWidth, double actualScale, bool bwhite, bool dummy, string motherid, GraphicsRegionType regiontype)
            : this(circle, lineWidth, actualScale, bwhite, dummy, string.Empty, null, motherid, regiontype)
        {

        }

        public GraphicsEllipseMark(Circle circle,
            double lineWidth,double actualScale, bool bwhite, bool dummy, string caption, MarkItem markItem, string motherID, GraphicsRegionType regiontype)
        {
            this.CircleInfo = new Circle(new Point(circle.Position.X, circle.Position.Y), circle.Radian);
            //this.CircleInfo = circle;
            this.startPoint = new Point(circle.Position.X - circle.Radian, circle.Position.Y - circle.Radian);
           // this.rectangleLeft = circle.Position.X - circle.Radian;
           // this.rectangleTop = circle.Position.Y - circle.Radian;
           // this.rectangleRight = circle.Position.X + circle.Radian;
           // this.rectangleBottom = circle.Position.Y + circle.Radian;
            this.white = bwhite;
            this.Dummy = dummy;
            this.MarkID = motherID;

            this.graphicsRegionType = regiontype;
            if (Dummy)
            {
                this.graphicsObjectColor = Colors.YellowGreen;
                this.OriginObjectColor = Colors.YellowGreen;
                this.graphicsLineWidth = lineWidth/2.0;
            }
            else
            {
                this.graphicsObjectColor = Colors.Yellow;
                this.OriginObjectColor = Colors.Yellow;
                this.graphicsLineWidth = lineWidth;
            }
            this.graphicsActualScale = actualScale;
            this.caption = "원형 마킹";

            this.LeftProperty = (int)this.rectangleLeft;
            this.TopProperty = (int)this.rectangleTop;
            this.WidthProperty = Math.Abs(rectangleRight - rectangleLeft);
            this.HeightProperty = Math.Abs(rectangleBottom - rectangleTop);

            InspectionList.Clear();
            MarkInfo = markItem;

            //RefreshDrawng();
        }

        public GraphicsEllipseMark()
            : this(new Circle(new Point(50,50), 50), 1.0, 1.0, false, false, "", GraphicsRegionType.MarkingUnit)
        {
        }

        #endregion Constructors

        #region Overrides
        // Ellipse를 화면에 출력한다.
        public override void Draw(DrawingContext drawingContext)
        {
            if (drawingContext == null)
            {
                throw new ArgumentNullException("drawingContext");
            }

            Rect r = Rectangle;
            Point center = new Point((r.Left + r.Right) / 2.0, (r.Top + r.Bottom) / 2.0);

            double radiusX = (r.Right - r.Left) / 2.0;
            double radiusY = (r.Bottom - r.Top) / 2.0;

            Brush br;
            if (white) br = new SolidColorBrush(Colors.White);
            else br = new SolidColorBrush(Colors.Black);
            if (Dummy) br.Opacity = 0.3;
            else br.Opacity = 0.5;
            drawingContext.DrawEllipse(
                br,
                new Pen(new SolidColorBrush(ObjectColor), ActualLineWidth),
                CircleInfo.Position, CircleInfo.Radian, CircleInfo.Radian);

            if (this.ActualScale >= 0.5)
            {
                drawingContext.DrawText(
                    CreateCaptionString(),
                    new Point(startPoint.X, startPoint.Y - (15 / graphicsActualScale)));
            }
            base.Draw(drawingContext);
        }

        public override bool Contains(Point point)
        {
            if (IsSelected)
            {
                return this.Rectangle.Contains(point);
            }
            else
            {
                EllipseGeometry g = new EllipseGeometry(Rectangle);

                return g.FillContains(point) || g.StrokeContains(new Pen(Brushes.Black, ActualLineWidth), point);
            }
        }

        public override PropertiesGraphicsBase CreateSerializedObject()
        {
            return new PropertiesGraphicsEllipseMark(this);
        }

        public override void Move(double deltaX, double deltaY, double maxWidth, double maxHeight)
        {
            LineSegments = null;

            base.Move(deltaX, deltaY, maxWidth, maxHeight);
        }

        public override void MoveHandleTo(Point point, int handleNumber)
        {
            LineSegments = null;

            base.MoveHandleTo(point, handleNumber);
        }
        #endregion Overrides
    }
}
