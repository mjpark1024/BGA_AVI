using System;
using System.Windows;
using System.Windows.Media;
using Common.Drawing.InspectionInformation;
using System.Collections.Generic;

namespace Common.Drawing
{
    public class GraphicsMarkGuide : GraphicsRectangleBase
    {
        public double cpX;
        public double cpY;
        private int id = 0;
        private Color[] colors = { Colors.Yellow, Colors.Blue, Colors.Red, Colors.Violet, Colors.Tomato };

        #region Constructors
        public GraphicsMarkGuide(double top, double bottom, double cpx, double lineWidth, Color objectColor, double actualScale, int id)
        {
            this.startPoint = new Point(0, top);
            this.rectangleLeft = 0;
            this.rectangleTop = top;
            this.rectangleRight = 4090;
            this.rectangleBottom = bottom;

            this.LeftProperty = (int)rectangleLeft;
            this.TopProperty = (int)rectangleTop;
            this.WidthProperty = Math.Abs(rectangleRight - rectangleLeft);
            this.HeightProperty = Math.Abs(rectangleBottom - rectangleTop);
            this.id = id;
            this.cpX = cpx;
            this.cpY = rectangleTop + (rectangleBottom - rectangleTop) / 2.0;

            this.graphicsLineWidth = lineWidth;
            this.graphicsObjectColor = objectColor;
            this.OriginObjectColor = objectColor;
            this.graphicsActualScale = actualScale;

            // Guide Line.
            this.graphicsRegionType = GraphicsRegionType.MarkGuide;
            this.caption = CaptionHelper.MarkGuideCaption;
            RefreshDrawing();
        }
        #endregion Constructors

        #region Overrides
        /// <summary>
        /// Draw object
        /// </summary>
        public override void Draw(DrawingContext drawingContext)
        {
            if (drawingContext == null)
            {
                throw new ArgumentNullException("drawingContext");
            }

            drawingContext.DrawRectangle(
                null,
                new Pen(new SolidColorBrush(colors[id]), 5),
                Rectangle);

            if (this.ActualScale >= 0.5)
            {
                drawingContext.DrawText(
                    CreateCaptionString(),
                    new Point(startPoint.X, startPoint.Y - (15 / graphicsActualScale)));
            }
            drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.Red), 2), new Point(rectangleLeft, cpY), new Point(rectangleRight, cpY));
            drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.Red), 2), new Point(cpX, rectangleTop), new Point(cpX, rectangleBottom));
            base.Draw(drawingContext);
        }

        /// <summary>
        /// Test whether object contains point
        /// </summary>
        public override bool Contains(Point point)
        {
            return this.Rectangle.Contains(point);
        }

        public override void Move(double deltaX, double deltaY, double maxWidth, double maxHeight)
        {
            //double farLeft = maxWidth;
            //double farTop = maxHeight;
            //double farRight = 0;
            //double farBottom = 0;

            //bool bCanMove = false;
            //farLeft = Math.Min(rectangleLeft, farLeft);
            //farTop = Math.Min(rectangleTop, farTop);
            //farRight = Math.Max(rectangleRight, farRight);
            //farBottom = Math.Max(rectangleBottom, farBottom);

            //bCanMove = (farLeft + deltaX > 0) && (farTop + deltaY > 0) && (farRight + deltaX < maxWidth) && (farBottom + deltaY < maxHeight);
            //if (bCanMove)
            //{

            //    base.Move(deltaX, deltaY, maxWidth, maxHeight);

            //}
        }

        public override void MoveHandleTo(Point point, int handleNumber)
        {
            return;
            /*
            point.X = (point.X < 0) ? 0 : point.X;
            point.Y = (point.Y < 0) ? 0 : point.Y;

            switch (handleNumber)
            {
                case 1:
                    rectangleLeft = point.X;
                    rectangleTop = point.Y;
                    break;
                case 2:
                    rectangleTop = point.Y;
                    break;
                case 3:
                    rectangleRight = point.X;
                    rectangleTop = point.Y;
                    break;
                case 4:
                    rectangleRight = point.X;
                    break;
                case 5:
                    rectangleRight = point.X;
                    rectangleBottom = point.Y;
                    break;
                case 6:
                    rectangleBottom = point.Y;
                    break;
                case 7:
                    rectangleLeft = point.X;
                    rectangleBottom = point.Y;
                    break;
                case 8:
                    rectangleLeft = point.X;  
                    break;
            }
            this.cpX = this.rectangleLeft + (Math.Abs(rectangleRight - rectangleLeft) / 2);
            this.cpY = this.rectangleTop + (Math.Abs(rectangleBottom - rectangleTop) / 2);
            CalcBoundaryRect();
            RefreshDrawing();
            this.cpX = this.rectangleLeft + (Math.Abs(rectangleRight - rectangleLeft) / 2);
            this.cpY = this.rectangleTop + (Math.Abs(rectangleBottom - rectangleTop) / 2);
            */
        }

        /// <summary>
        /// Serialization support
        /// </summary>
        public override PropertiesGraphicsBase CreateSerializedObject()
        {
            return new PropertiesGraphicsMarkGuide(this);
        }
        #endregion Overrides

    }
}
