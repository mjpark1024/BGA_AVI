using System;
using System.Windows;
using System.Windows.Media;
using Common.Drawing.MarkingInformation;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Common.Drawing
{
    public class GraphicsRectangleMark : GraphicsRectangleBase
    {
        #region Private member variables.
        protected IterationSymmetryInformation graphicsSymmetryValue;
        protected IterationInformation graphicsIterationValue;
        protected IterationInformation graphicsBlockIterationValue;
        //protected GraphicsBase motherROI;
        protected int motherROIID = -1;
        protected int graphicsIterationXPosition;
        protected int graphicsIterationYPosition;
        protected bool isFiducialRegion = false;
        public bool IsValidRegion = false;
        public bool white = false;
        #endregion

        #region Constructors
        // Default constructor.
        public GraphicsRectangleMark() 
            : this(0.0, 0.0, 100.0, 100.0, 1.0, Colors.Black, 1.0, new IterationSymmetryInformation(1, 1, 1, 1), new IterationInformation(1, 1, 0, 0), new IterationInformation(1, 1, 0, 0), false, "", string.Empty,false, false, null, 0, 0, GraphicsRegionType.MarkingUnit)
        {
        }

        // It called by DrawingCanvas
        public GraphicsRectangleMark(double left, double top, double right, double bottom, double lineWidth, Color objectColor, double actualScale, bool bwhite, bool bdummy, string id, GraphicsRegionType regiontype)
            : this(left, top, right, bottom, lineWidth, objectColor, actualScale, new IterationSymmetryInformation(1, 1, 1, 1), new IterationInformation(1, 1, 0, 0), new IterationInformation(1, 1, 0, 0), false, id, string.Empty, bwhite, bdummy, null, 0, 0, regiontype)
        {
        }

        // It called by SectionManager
        public GraphicsRectangleMark(double left, double top, double right, double bottom, double lineWidth, GraphicsRegionType regionType, Color objectColor, double actualScale, bool bwhite, bool bdummy, IterationSymmetryInformation symmetryValue, IterationInformation iterationValue, IterationInformation blockIterationValue, bool isFiducialRegion, string motherROI, GraphicsRegionType regiontype)
            : this(left, top, right, bottom, lineWidth, objectColor, actualScale, symmetryValue, iterationValue, blockIterationValue, isFiducialRegion, motherROI, string.Empty, bwhite, bdummy, null, 0, 0, regiontype)
        {
            
        }

        // It called by PropertiesGraphicRectangle
        public GraphicsRectangleMark(double left, double top, double right, double bottom,
                                 double lineWidth, 
                                 Color objectColor,
                                 double actualScale,
                                 IterationSymmetryInformation symmetryValue,
                                 IterationInformation iterationValue, 
                                 IterationInformation blockIterationValue, 
                                 bool isFiducialRegion, 
                                 string motherROIID,
                                 string caption, 
                                 bool white,
                                  bool dummy,
                                 MarkItem markItem,
                                 int iterationXPosition, int iterationYPosition, GraphicsRegionType regiontype)
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

            this.graphicsSymmetryValue = symmetryValue;
            this.graphicsIterationValue = iterationValue;
            this.graphicsIterationXPosition = iterationXPosition;
            this.graphicsIterationYPosition = iterationYPosition;
            this.graphicsBlockIterationValue = blockIterationValue;

            this.isFiducialRegion = isFiducialRegion;
         //   this.motherROI = motherROI;
            this.MarkID = motherROIID;

            this.caption = "사각 마킹";
            this.white = white;
            this.Dummy = dummy;
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
            if (Dummy) br.Opacity = 0.3;
            else br.Opacity = 0.5;
            // draw rect
            drawingContext.DrawRectangle(br, new Pen(new SolidColorBrush(ObjectColor), ActualLineWidth), Rectangle);
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
            return new PropertiesGraphicsRectangleMark(this);
        }
        #endregion Overrides

        #region Properties.
        public IterationSymmetryInformation SymmetryValue
        {
            get { return graphicsSymmetryValue; }
            set { graphicsSymmetryValue = value; }
        }

        public IterationInformation IterationValue
        {
            get { return graphicsIterationValue; }
            set { graphicsIterationValue = value; }
        }

        public int IterationXPosition
        {
            get { return graphicsIterationXPosition; }
            set { graphicsIterationXPosition = value; }
        }

        public int IterationYPosition
        {
            get { return graphicsIterationYPosition; }
            set { graphicsIterationYPosition = value; }
        }

        public IterationInformation BlockIterationValue
        {
            get { return graphicsBlockIterationValue; }
            set { graphicsBlockIterationValue = value; }
        }

        public bool IsFiducialRegion
        {
            get { return isFiducialRegion; }
            set { isFiducialRegion = value; }
        }

        public bool White
        {
            get { return white; }
            set { white = value; }
        }

        public int MotherROIID
        {
            get { return motherROIID; }
            set { motherROIID = value; }
        }
        #endregion Properties.
    }
}

