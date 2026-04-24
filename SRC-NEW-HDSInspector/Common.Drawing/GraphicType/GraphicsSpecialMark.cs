using System;
using System.Windows;
using System.Windows.Media;
using Common.Drawing.MarkingInformation;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace Common.Drawing
{
    public class GraphicsSpecialMark : GraphicsRectangleBase
    {
         #region Private member variables.
        public bool IsValidRegion = false;
        public bool white = false;
        private int shape = 0;
        protected int motherROIID = -1;
        #endregion

        #region Constructors
        // Default constructor.
        public GraphicsSpecialMark() 
            : this(0.0, 0.0, 100.0, 100.0, 1.0, Colors.Black, 1.0, "", string.Empty, false, false, 0,  null,GraphicsRegionType.MarkingUnit)
        {
        }

        // It called by DrawingCanvas
        public GraphicsSpecialMark(double left, double top, double right, double bottom, double lineWidth, Color objectColor, string nid, double actualScale, bool bwhite, bool bdummy, int shape, GraphicsRegionType regiontype)
            : this(left, top, right, bottom, lineWidth, objectColor, actualScale, nid, string.Empty, bwhite, bdummy, shape, null, regiontype)
        {
        }

        // It called by SectionManager
        public GraphicsSpecialMark(double left, double top, double right, double bottom, double lineWidth, GraphicsRegionType regionType, Color objectColor, string nid, double actualScale, bool bwhite, bool bdummy, int shape, GraphicsRegionType regiontype)
            : this(left, top, right, bottom, lineWidth, objectColor, actualScale, nid, string.Empty, bwhite,bdummy, shape, null, regiontype)
        {
            
        }

        // It called by PropertiesGraphicRectangle
        public GraphicsSpecialMark(double left, double top, double right, double bottom,
                                 double lineWidth, 
                                 Color objectColor,
                                 double actualScale,
                                 string motherROI,
                                 string caption, 
                                 bool white,
                                 bool dummy,
                                 int shape,
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

            this.shape = shape;

            this.caption = "특수 마킹";
            this.white = white;
            this.MarkInfo = markItem;
            this.Dummy = dummy;
            //this.motherROI = motherROI;
            this.MarkID = motherROI;
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
            // draw rect
            Brush br;
            if (white) br = new SolidColorBrush(Colors.Red);
            else br = new SolidColorBrush(Colors.Blue);
            if (Dummy) br.Opacity = 0.5;
            else br.Opacity = 0.8;
            drawingContext.DrawRectangle(null, new Pen(new SolidColorBrush(ObjectColor), ActualLineWidth), Rectangle);
            switch(shape)
            {
                case 0:
                    drawingContext.DrawLine(new Pen(br, 5), new Point(Right,Top), new Point(Left,Bottom));
                    break;
                case 1:
                    drawingContext.DrawLine(new Pen(br, 5), new Point(Left, Top), new Point(Right, Bottom));
                    break;
                case 2:
                    drawingContext.DrawLine(new Pen(br, 5), new Point(Right, Top), new Point(Left, Bottom));
                    drawingContext.DrawLine(new Pen(br, 5), new Point(Left, Top), new Point(Right, Bottom));
                    break;
            }
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
            return new PropertiesGraphicsSpecialMark(this);
        }
        #endregion Overrides

        #region Properties.

        public bool White
        {
            get { return white; }
            set { white = value; }
        }

        public int Shape
        {
            get { return shape; }
            set { shape = value; }
        }

        public int MotherROIID
        {
            get
            {
                return motherROIID;
            }
            set
            {
                motherROIID = value;
            }
        }
        #endregion Properties.
    }
}
