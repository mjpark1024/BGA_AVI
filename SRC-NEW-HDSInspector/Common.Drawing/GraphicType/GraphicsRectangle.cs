// http://www.codeproject.com/KB/WPF/WPF_DrawTools.aspx
// Author : Alex Fr
// This article, along with any associated source code and files, is licensed under The Code Project Open License (CPOL)

using System;
using System.Windows;
using System.Windows.Media;
using Common.Drawing.InspectionInformation;
using System.Collections.Generic;
using System.Xml.Serialization;

// Modified & Commented by suoow2.

namespace Common.Drawing
{
    // Rectangle graphics object.
    public class GraphicsRectangle : GraphicsRectangleBase
    {
        #region Private member variables.
        protected IterationSymmetryInformation graphicsSymmetryValue;
        protected IterationInformation graphicsIterationValue;
        protected IterationInformation graphicsBlockIterationValue;
        protected GraphicsBase motherROI;
        protected int motherROIID = -1;
        protected int graphicsIterationXPosition;
        protected int graphicsIterationYPosition;
        protected bool isFiducialRegion = false;
        protected bool isInspection = true;
        public bool IsValidRegion = false;
        #endregion

        #region Constructors
        // Default constructor.
        public GraphicsRectangle() 
            : this(0.0, 0.0, 100.0, 100.0, 1.0, GraphicsRegionType.Inspection, Colors.Black, 1.0, new IterationSymmetryInformation(1, 1, 1, 1), new IterationInformation(1, 1, 0, 0), new IterationInformation(1, 1, 0, 0), false, null, string.Empty, null, 0, 0)
        {
        }

        // It called by DrawingCanvas
        public GraphicsRectangle(double left, double top, double right, double bottom, double lineWidth, GraphicsRegionType regionType, Color objectColor, double actualScale)
            : this(left, top, right, bottom, lineWidth, regionType, objectColor, actualScale, new IterationSymmetryInformation(1, 1, 1, 1), new IterationInformation(1, 1, 0, 0), new IterationInformation(1, 1, 0, 0), false, null, string.Empty, null, 0, 0)
        {
        }

        // It called by SectionManager
        public GraphicsRectangle(double left, double top, double right, double bottom, double lineWidth, GraphicsRegionType regionType, Color objectColor, double actualScale, IterationSymmetryInformation symmetryValue, IterationInformation iterationValue, IterationInformation blockIterationValue, bool isFiducialRegion, GraphicsBase motherROI)
            : this(left, top, right, bottom, lineWidth, regionType, objectColor, actualScale, symmetryValue, iterationValue, blockIterationValue, isFiducialRegion, motherROI, string.Empty, null, 0, 0)
        {
            
        }

        // It called by PropertiesGraphicRectangle
        public GraphicsRectangle(double left, double top, double right, double bottom,
                                 double lineWidth, 
                                 GraphicsRegionType regionType, 
                                 Color objectColor,
                                 double actualScale,
                                 IterationSymmetryInformation symmetryValue,
                                 IterationInformation iterationValue, 
                                 IterationInformation blockIterationValue, 
                                 bool isFiducialRegion, 
                                 GraphicsBase motherROI,
                                 string caption, List<InspectionItem> inspectionList, 
                                 int iterationXPosition, int iterationYPosition)
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
            this.graphicsRegionType = regionType;
            this.graphicsObjectColor = objectColor;
            this.OriginObjectColor = objectColor;
            this.graphicsActualScale = actualScale;

            this.graphicsSymmetryValue = symmetryValue;
            this.graphicsIterationValue = iterationValue;
            this.graphicsIterationXPosition = iterationXPosition;
            this.graphicsIterationYPosition = iterationYPosition;
            this.graphicsBlockIterationValue = blockIterationValue;

            this.isFiducialRegion = isFiducialRegion;
            this.motherROI = motherROI;
            this.motherROIID = (this.motherROI != null) ? this.motherROI.ID : -1;

            #region set caption.
            this.caption = caption;
            if (string.IsNullOrEmpty(this.caption))
            {
                switch (regionType)
                {
                    case GraphicsRegionType.StripAlign:     this.caption = CaptionHelper.StripAlignCaption;             break;
                    case GraphicsRegionType.IDMark:         this.caption = CaptionHelper.IDMarkCaption;                 break;
                    case GraphicsRegionType.UnitAlign:      this.caption = CaptionHelper.UnitAlignCaption;              break;
                    case GraphicsRegionType.WPShift:        this.caption = CaptionHelper.WPShiftCaption;                break;
                    case GraphicsRegionType.UnitRegion:     this.caption = CaptionHelper.FiducialUnitRegionCaption;     break;
                    case GraphicsRegionType.PSROdd:         this.caption = CaptionHelper.FiducialPsrRegionCaption;      break;
                    case GraphicsRegionType.OuterRegion:    this.caption = CaptionHelper.FiducialOuterRegionCaption;    break;
                    case GraphicsRegionType.IDRegion:       this.caption = CaptionHelper.FiducialIDRegionCaption;       break;
                    case GraphicsRegionType.Rawmetrial:     this.caption = CaptionHelper.FiducialRawRegionCaption;      break;
                    case GraphicsRegionType.Except:         this.caption = CaptionHelper.ExceptionalMaskCaption;        break;
                }
            }
            #endregion

            if (inspectionList != null)
            {
                int nSequenceID = 1;
                foreach (InspectionItem inspectionElement in inspectionList)
                {
                    inspectionElement.ID = nSequenceID++;
                    this.InspectionList.Add(inspectionElement);
                }
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
            drawingContext.DrawRectangle(null, new Pen(new SolidColorBrush(ObjectColor), ActualLineWidth), Rectangle);
            if (this.ActualScale >= 0.5)
            {
                // draw caption
                drawingContext.DrawText(CreateCaptionString(), new Point(startPoint.X, startPoint.Y - (15 / graphicsActualScale)));
            }

            if (IsSelected)
            {
                if (LineSegments != null && LineSegments.Length > 0)
                {
                    foreach (GraphicsSkeletonLine lineSegment in LineSegments)
                        lineSegment.Draw(drawingContext, new Point(rectangleLeft, rectangleTop), this.ActualScale);
                }
                if (BallSegments != null && BallSegments.Length > 0)
                {
                    foreach (GraphicsSkeletonBall ballSegment in BallSegments)
                        ballSegment.Draw(drawingContext, new Point(rectangleLeft, rectangleTop), this.ActualScale);
                }
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
            foreach (InspectionItem inspItem in InspectionList)
            {
                if (inspItem.LineSegments != null)
                    inspItem.LineSegments = null;
                if (inspItem.BallSegments != null)
                    inspItem.BallSegments = null;
            }
            LineSegments = null;
            BallSegments = null;

            base.Move(deltaX, deltaY, maxWidth, maxHeight);
        }

        // 조절점을 통한 크기 변경
        public override void MoveHandleTo(Point point, int handleNumber)
        {
            foreach (InspectionItem inspItem in InspectionList)
            {
                if (inspItem.LineSegments != null)
                    inspItem.LineSegments = null;
                if (inspItem.BallSegments != null)
                    inspItem.BallSegments = null;
            }
            LineSegments = null;
            BallSegments = null;

            base.MoveHandleTo(point, handleNumber);
        }

        // to XML
        public override PropertiesGraphicsBase CreateSerializedObject()
        {
            return new PropertiesGraphicsRectangle(this);
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

        public bool IsInspection
        {
            get { return isInspection; }
            set { isInspection = value; }
        }


        public GraphicsBase MotherROI
        {
            get
            {
                return motherROI;
            }
            set
            {
                motherROI = value;
                if (motherROI != null)
                {
                    motherROIID = motherROI.ID;
                }
                else
                {
                    motherROIID = -1;
                }
            }
        }

        public int MotherROIID
        {
            get { return motherROIID; }
            set { motherROIID = value; }
        }
        #endregion
    }
}
