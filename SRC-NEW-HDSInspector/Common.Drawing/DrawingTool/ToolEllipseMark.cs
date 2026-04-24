using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using Common.Drawing.MarkingTypeUI;
using Common.Drawing.MarkingInformation;


namespace Common.Drawing
{
    class ToolEllipseMark : ToolEllipseBase
    {
        public ToolEllipseMark()
        {
            using (MemoryStream stream = new MemoryStream(Properties.Resources.Ellipse))
            {
                ToolCursor = new Cursor(stream);
            }
        }

        // Create new Ellipse.
        public override void OnMouseDown(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(drawingCanvas);
            if (p.X >= drawingCanvas.ActualWidth - 1 || p.Y >= drawingCanvas.ActualHeight - 1)
            {
                return;
            }

            if (p.X < 0)
                p.X = 0;
            if (p.Y < 0)
                p.Y = 0;

            SetGraphicsColor(drawingCanvas);
            string str = GetMarkID(drawingCanvas);
            GraphicsEllipseMark graphic = new GraphicsEllipseMark(new Circle(new Point(p.X, p.Y), 1),
                                    drawingCanvas.LineWidth,
                                    drawingCanvas.ActualScale, false, false, str, GraphicsRegionType.MarkingUnit);
            UnitCircleProperty rc = new UnitCircleProperty();
            graphic.MarkInfo = new MarkItem(MarkingType.GetMarkType(eMarkingType.eMarkingUnitCircle), rc, 0);
            AddNewObject(drawingCanvas, graphic);
        }
    }
}

