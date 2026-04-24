using System;
using System.Windows;
using System.Windows.Media;
using Common.Drawing.MarkingInformation;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Common.Drawing
{
    public class GraphicsTriangleMark : GraphicsRectangleBase
    {  
        #region Private member variables.
        public bool IsValidRegion = false;
        public bool white = false;
        private int rotate = 0;
        private int motherROIID;
        #endregion

        #region Constructors
        // Default constructor.
        public GraphicsTriangleMark() 
            : this(0.0, 0.0, 100.0, 100.0, 1.0, Colors.Black, 1.0, "", string.Empty, false, false, 0,  null, GraphicsRegionType.MarkingUnit)
        {
        }

        // It called by DrawingCanvas
        public GraphicsTriangleMark(double left, double top, double right, double bottom, double lineWidth, Color objectColor, double actualScale, bool bwhite, bool bdummy, string nid, int rotate, GraphicsRegionType regiontype)
            : this(left, top, right, bottom, lineWidth, objectColor, actualScale, nid, string.Empty, bwhite, bdummy, rotate,  null, regiontype)
        {
        }

        // It called by SectionManager
        public GraphicsTriangleMark(double left, double top, double right, double bottom, double lineWidth, GraphicsRegionType regionType, Color objectColor, double actualScale, bool bwhite, bool bdummy, string nid, int rotate, GraphicsRegionType regiontype)
            : this(left, top, right, bottom, lineWidth, objectColor, actualScale, nid, string.Empty, bwhite, bdummy, rotate, null, regiontype)
        {
            
        }

        // It called by PropertiesGraphicRectangle
        public GraphicsTriangleMark(double left, double top, double right, double bottom,
                                 double lineWidth, 
                                 Color objectColor,
                                 double actualScale,
                                 string motherid,
                                 string caption, 
                                 bool white,
                                 bool dummy,
                                 int rotate,
                                 MarkItem markItem, GraphicsRegionType regiontype)
        {
            this.startPoint = new Point(left, top);
            this.rectangleLeft = left;
            this.rectangleTop = top;
            this.rectangleRight = right;
            this.rectangleBottom = bottom;

            this.LeftProperty = (int)left;
            this.TopProperty = (int)top;
            this.WidthProperty = Math.Abs(rectangleRight - rectangleLeft);
            this.HeightProperty = Math.Abs(rectangleBottom - rectangleTop);

            this.graphicsLineWidth = lineWidth;
            this.graphicsRegionType = regiontype;
            this.graphicsObjectColor = objectColor;
            this.OriginObjectColor = objectColor;
            this.graphicsActualScale = actualScale;

            this.rotate = rotate;

            this.caption = "삼각 마킹";
            this.white = white;
            this.Dummy = dummy;
            this.MarkID = motherid;
            this.MarkInfo = markItem;
            if (Dummy)
            {
                this.graphicsObjectColor = Colors.YellowGreen;
                this.OriginObjectColor = Colors.YellowGreen;
                this.graphicsLineWidth = lineWidth / 2.0;
            }
            else
            {
                this.graphicsObjectColor = Colors.Yellow;
                this.OriginObjectColor = Colors.Yellow;
                this.graphicsLineWidth = lineWidth;
            }
            //RefreshDrawng();
        }
        #endregion Constructors

        #region Overrides
        // Rectangle을 화면에 출력한다.
        public override void Draw(DrawingContext drawingContext)
        {
            if (drawingContext == null)
            {
                throw new ArgumentNullException("drawingContext");
            }
            Brush br;
            if (white) br = new SolidColorBrush(Colors.White);
            else br = new SolidColorBrush(Colors.Black);
            br.Opacity = 0.5;
            // draw rect
            drawingContext.DrawRectangle(null, new Pen(new SolidColorBrush(ObjectColor), ActualLineWidth), Rectangle);
            PathFigure figure = new PathFigure { IsClosed = true };
            //IsClosed = true;
            LineSegment segment1, segment2, segment3;
            switch (rotate){
                case 180: 
                    figure.StartPoint = new Point(this.Left, Top);
                    segment2 = new LineSegment(new Point(Right, Bottom), true) { IsSmoothJoin = true };
                    figure.Segments.Add(segment2);
                    segment3 = new LineSegment(new Point(Left, Bottom), true) { IsSmoothJoin = true };
                    figure.Segments.Add(segment3);
                    segment1 = new LineSegment(new Point(Left, Top), true) { IsSmoothJoin = true };
                    figure.Segments.Add(segment1);
                    break;
                case 90:
                    figure.StartPoint = new Point(this.Right, Top);
                    segment2 = new LineSegment(new Point(Left, Bottom), true) { IsSmoothJoin = true };
                    figure.Segments.Add(segment2);
                    segment3 = new LineSegment(new Point(Left, Top), true) { IsSmoothJoin = true };
                    figure.Segments.Add(segment3);
                    segment1 = new LineSegment(new Point(Right, Top), true) { IsSmoothJoin = true };
                    figure.Segments.Add(segment1);
                    break;
                case 0:
                    figure.StartPoint = new Point(this.Right, Bottom);
                    segment2 = new LineSegment(new Point(Left, Top), true) { IsSmoothJoin = true };
                    figure.Segments.Add(segment2);
                    segment3 = new LineSegment(new Point(Right, Top), true) { IsSmoothJoin = true };
                    figure.Segments.Add(segment3);
                    segment1 = new LineSegment(new Point(Right, Bottom), true) { IsSmoothJoin = true };
                    figure.Segments.Add(segment1);
                    break;
                case 270:
                    figure.StartPoint = new Point(this.Left, Bottom);
                    segment2 = new LineSegment(new Point(Right, Top), true) { IsSmoothJoin = true };
                    figure.Segments.Add(segment2);
                    segment3 = new LineSegment(new Point(Right, Bottom), true) { IsSmoothJoin = true };
                    figure.Segments.Add(segment3);
                    segment1 = new LineSegment(new Point(Left, Bottom), true) { IsSmoothJoin = true };
                    figure.Segments.Add(segment1);
                    break;
            }
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(figure);
            drawingContext.DrawGeometry(br, new Pen(new SolidColorBrush(Colors.Red), 0.5), pathGeometry); 
            if (this.ActualScale >= 0.5)
            {
                // draw caption
                drawingContext.DrawText(CreateCaptionString(), new Point(startPoint.X, startPoint.Y - (15 / graphicsActualScale)));
            }
            base.Draw(drawingContext);
        }

        // Point가 Rectangle 내부에 위치했는가를 판정한다.
        public override bool Contains(Point point)
        {
            return this.Rectangle.Contains(point);
        }

        // Rectangle 이동
        public override void Move(double deltaX, double deltaY, double maxWidth, double maxHeight)
        {
            LineSegments = null;

            base.Move(deltaX, deltaY, maxWidth, maxHeight);
        }

        // 조절점을 통한 크기 변경
        public override void MoveHandleTo(Point point, int handleNumber)
        {
            LineSegments = null;

            base.MoveHandleTo(point, handleNumber);
        }

        // to XML
        public override PropertiesGraphicsBase CreateSerializedObject()
        {
            return new PropertiesGraphicsTriangleMark(this);
        }
        #endregion Overrides

        #region Properties.

        public bool White
        {
            get { return white; }
            set { white = value; }
        }

        public int Rotate
        {
            get { return rotate; }
            set { rotate = value; }
        }

        public int MotherROIID
        {
            get { return motherROIID; }
            set { motherROIID = value; }
        }
        #endregion Properties.
    }
}

