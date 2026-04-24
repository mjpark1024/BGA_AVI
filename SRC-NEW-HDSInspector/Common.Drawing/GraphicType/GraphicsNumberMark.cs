using System;
using System.Windows;
using System.Windows.Media;
using Common.Drawing.MarkingInformation;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Common.Drawing
{

    public class GraphicsNumberMark : GraphicsRectangleBase
    {
        #region Private member variables.
        protected double rotate;
        protected MarkLogo logo;
        public bool IsValidRegion = false;
        #endregion

        #region Constructors
        // Default constructor.
        public GraphicsNumberMark() 
            : this(0.0, 0.0, 100.0, 100.0, 1.0, Colors.Black, 1.0, 0, string.Empty, string.Empty, null, GraphicsRegionType.MarkingReject, null)
        {
        }

        // It called by DrawingCanvas
        public GraphicsNumberMark(double left, double top, double right, double bottom, double lineWidth, Color objectColor, double actualScale, double rotate, string mID, GraphicsRegionType regiontype, MarkLogo logo)
            : this(left, top, right, bottom, lineWidth, objectColor, actualScale, rotate, string.Empty, mID, null, regiontype, logo)
        {
        }

        // It called by PropertiesGraphicRectangle
        public GraphicsNumberMark(double left, double top, double right, double bottom,
                                 double lineWidth, 
                                 Color objectColor,
                                 double actualScale,
                                 double rotate,
                                 string caption,
                                 string markID,
                                 MarkItem markItem,
                                 GraphicsRegionType regiontype,
                                 MarkLogo logo)
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
            this.logo = logo;
                this.graphicsObjectColor = Colors.Yellow;
                this.OriginObjectColor = Colors.Yellow;

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

            //RefNum 따라 저으이 할것
            drawingContext.DrawRectangle(new SolidColorBrush(Colors.Transparent), new Pen(new SolidColorBrush(ObjectColor), ActualLineWidth), Rectangle);

            int refno = 0;
            if (logo != null)
            {
                if (logo.FontRemark.Count > 0)
                {
                    string[] str = (MarkInfo.MarkInfo as NumberProperty).FNTFile.Split('\\');
                    refno = logo.FontRemark.IndexOf(str[str.Length-1]) + 1;
                }
            }
            if (refno == -1) refno = 0;
            DrawingVisual drawingVisual = new DrawingVisual();
            
            using (DrawingContext drawContext = drawingVisual.RenderOpen())
            {
                
                Brush br = new SolidColorBrush(Colors.White);
                br.Opacity = 0.3;
                Rect rct;
                switch(refno)
                {
                    case 0:
                        drawContext.PushTransform(rt);    
                        FormattedText f = CreateNumberString(height);
                        drawContext.DrawText(f, new Point(rt.CenterX - (f.Width / 2.0), startPoint.Y));          
                    break;
                    case 1:
                        //drawContext.PushTransform(rt);
                        rct = new Rect(Rectangle.Left + 1, Rectangle.Top + 1, Rectangle.Width - 2, Rectangle.Height - 2);
                      //  rct = new Rect(Rectangle.Left + 2, Rectangle.Top + 2, Rectangle.Height - 4, ((MarkInfo.MarkInfo as NumberProperty).CapitalHeight*1000.0/12.5));
                        drawContext.DrawRectangle(br, new Pen(new SolidColorBrush(ObjectColor), ActualLineWidth), rct);
                    break;
                    case 2:
                        //rt.Angle -= 30.0;
                        //drawContext.PushTransform(rt);
                    drawingContext.DrawLine(new Pen(br, 5), new Point(Right, Top), new Point(Left, Bottom));
                        //rct = new Rect(Rectangle.Left + 2, Rectangle.Top + 2, Rectangle.Height - 4, (MarkInfo.MarkInfo as NumberProperty).CharHeight * 1000.0 / 12.5);
                        //drawContext.DrawRectangle(br, new Pen(new SolidColorBrush(ObjectColor), ActualLineWidth), rct);
                    break;
                    case 3:
                    //drawContext.PushTransform(rt);
                    drawingContext.DrawLine(new Pen(br, 5), new Point(Right, Top), new Point(Left, Bottom));
                    drawingContext.DrawLine(new Pen(br, 5), new Point(Left, Top), new Point(Right, Bottom));
                    break;
                    case 4:
                        //drawContext.PushTransform(rt);
                        rct = new Rect(Rectangle.Left + 1, Rectangle.Top + 1, Rectangle.Width - 2, Rectangle.Height - 2);
                        drawContext.DrawRectangle(br, new Pen(new SolidColorBrush(ObjectColor), ActualLineWidth), rct);
                    break;
                }
            }
            DrawingGroup dg = VisualTreeHelper.GetDrawing(drawingVisual);
            drawingContext.DrawDrawing(dg);
            //FormattedText f = CreateNumberString(height);
            //drawingContext.DrawText(f, new Point(rt.CenterX - (f.Width / 2.0), startPoint.Y));
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
            return new PropertiesGraphicsNumberMark(this);
        }
        #endregion Overrides

        public double Rotate
        {
            get { return rotate; }
            set { rotate = value; }
        }

        public MarkLogo Logo
        {
            get { return logo; }
        }

    }
}


