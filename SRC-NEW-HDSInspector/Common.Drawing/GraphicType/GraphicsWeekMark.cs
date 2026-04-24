using System;
using System.Windows;
using System.Windows.Media;
using Common.Drawing.MarkingInformation;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Common.Drawing
{
    public class GraphicsWeekMark : GraphicsRectangleBase
    {
        #region Private member variables.
        protected double rotate;
        public bool IsValidRegion = false;
        #endregion
     #region Constructors
        // Default constructor.
        public GraphicsWeekMark() 
            : this(0.0, 0.0, 100.0, 100.0, 1.0, Colors.Black, 1.0, 0, string.Empty, string.Empty, null, GraphicsRegionType.MarkingReject)
        {
        }

        // It called by DrawingCanvas
        public GraphicsWeekMark(double left, double top, double right, double bottom, double lineWidth, Color objectColor, double actualScale, double rotate, string mID, GraphicsRegionType regiontype)
            : this(left, top, right, bottom, lineWidth, objectColor, actualScale, rotate, string.Empty, mID, null, regiontype)
        {
        }

        // It called by PropertiesGraphicRectangle
        public GraphicsWeekMark(double left, double top, double right, double bottom,
                                 double lineWidth, 
                                 Color objectColor,
                                 double actualScale,
                                 double rotate,
                                 string caption,
                                 string markID,
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
            this.MarkID = markID;
            this.caption = caption;
            this.MarkInfo = markItem;
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
            RotateTransform rt = new RotateTransform();

            rt.CenterX = startPoint.X + WidthProperty / 2.0;
            rt.CenterY = startPoint.Y + HeightProperty / 2.0;
            rt.Angle = this.Rotate + 90.0;
            //drawingContext.PushTransform(rt);
            double height = 1.0;
            if (MarkInfo.MarkType.MarkType == eMarkingType.eMarkingWeek)
            {
                height = (MarkInfo.MarkInfo as WeekProperty).CapitalHeight;
            }
            else if (MarkInfo.MarkType.MarkType == eMarkingType.eMarkingNumber)
            {
                height = (MarkInfo.MarkInfo as NumberProperty).CapitalHeight;
            }
            height = height * 1000 / MarkInfo.Resolution;
            drawingContext.DrawRectangle(new SolidColorBrush(Colors.Transparent), new Pen(new SolidColorBrush(ObjectColor), ActualLineWidth), Rectangle);
            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext drawContext = drawingVisual.RenderOpen())
            {

                Brush br = new SolidColorBrush(Colors.White);
                br.Opacity = 0.3;
                drawContext.PushTransform(rt);
                FormattedText f = CreateNumberString(height);
                drawContext.DrawText(f, new Point(rt.CenterX - (f.Width / 2.0), startPoint.Y));

            }
            DrawingGroup dg = VisualTreeHelper.GetDrawing(drawingVisual);
            drawingContext.DrawDrawing(dg);
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
            return new PropertiesGraphicsWeekMark(this);
        }
        #endregion Overrides

        public double Rotate
        {
            get { return rotate; }
            set { rotate = value; }
        }
    }
}


