using System;
using System.Windows;
using System.Windows.Media;
using Common.Drawing.InspectionInformation;
using System.Collections.Generic;
namespace Common.Drawing
{
    public class GraphicsStripOrigin : GraphicsRectangleBase
    {
        public const double SIZE = 50.0;
        public double cpX;
        public double cpY;
        public int nID;

        #region Constructors
        public GraphicsStripOrigin(double cpx, double cpy, int id, double lineWidth, Color objectColor, double actualScale)
        {
            double fSize = SIZE;
            nID = id;
            this.startPoint = new Point(cpx - fSize, cpy - fSize);
            this.rectangleLeft = cpx - fSize;
            this.rectangleTop = cpy - fSize;
            this.rectangleRight = cpx + fSize;
            this.rectangleBottom = cpy + fSize;

            this.LeftProperty = (int)rectangleLeft;
            this.TopProperty = (int)rectangleTop;
            this.WidthProperty = Math.Abs(rectangleRight - rectangleLeft);
            this.HeightProperty = Math.Abs(rectangleBottom - rectangleTop);

            this.cpX = cpx;
            this.cpY = cpy;

            this.graphicsLineWidth = lineWidth;
            this.graphicsObjectColor = objectColor;
            this.OriginObjectColor = objectColor;
            this.graphicsActualScale = actualScale;

            // Guide Line.
            this.graphicsRegionType = GraphicsRegionType.StripOrigin;
            this.caption = CaptionHelper.StripOriginCaption;
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
                new Pen(new SolidColorBrush(Colors.Yellow), 0.5),
                Rectangle);

            if (this.ActualScale >= 0.5)
            {
                drawingContext.DrawText(
                    CreateCaptionString(),
                    new Point(startPoint.X, startPoint.Y - (15 / graphicsActualScale)));
            }
            cpX = rectangleLeft + SIZE;
            cpY = rectangleTop + SIZE;
            drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.Red), 0.5), new Point(rectangleLeft, cpY), new Point(rectangleRight, cpY));
            drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.Red), 0.5), new Point(cpX, rectangleTop), new Point(cpX, rectangleBottom));
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
            double farLeft = maxWidth;
            double farTop = maxHeight;
            double farRight = 0;
            double farBottom = 0;

            bool bCanMove = false;
            farLeft = Math.Min(rectangleLeft, farLeft);
            farTop = Math.Min(rectangleTop, farTop);
            farRight = Math.Max(rectangleRight, farRight);
            farBottom = Math.Max(rectangleBottom, farBottom);

            bCanMove = (farLeft + deltaX > 0) && (farTop + deltaY > 0) && (farRight + deltaX < maxWidth) && (farBottom + deltaY < maxHeight);
            if (bCanMove)
            {

                base.Move(deltaX, deltaY, maxWidth, maxHeight);

            }
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
            return new PropertiesGraphicsStripOrigin(this);
        }
        #endregion Overrides

    }
}

