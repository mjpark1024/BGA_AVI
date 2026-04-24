using System;
using System.Windows;
using System.Windows.Media;
using Common.Drawing.InspectionInformation;
using System.Collections.Generic;

namespace Common.Drawing
{
    public class GraphicsUnitGuide : GraphicsRectangleBase
    {
       #region Constructors
        public GraphicsUnitGuide(double left, double top, double right, double bottom, double lineWidth, Color objectColor, double actualScale)
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
            this.graphicsObjectColor = objectColor;
            this.OriginObjectColor = objectColor;
            this.graphicsActualScale = actualScale;

            // Guide Line.
            this.graphicsRegionType = GraphicsRegionType.UnitGuide;
            this.caption = CaptionHelper.UnitGuideCaption;
            this.MarkID = "";
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
                new Pen(new SolidColorBrush(Colors.Red), 2),
                Rectangle);

            if (this.ActualScale >= 0.5)
            {
                drawingContext.DrawText(
                    CreateCaptionString(),
                    new Point(startPoint.X, startPoint.Y - (15 / graphicsActualScale)));
            }
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
            return new PropertiesGraphicsUnitGuide(this);
        }
        #endregion Overrides

    }
}

