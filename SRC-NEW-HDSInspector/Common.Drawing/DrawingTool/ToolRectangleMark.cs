
using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using Common.Drawing.MarkingTypeUI;
using Common.Drawing.MarkingInformation;

namespace Common.Drawing
{
    class ToolRectangleMark : ToolRectangleBase
    {
        #region Ctor.
        public ToolRectangleMark()
        {
            using (MemoryStream stream = new MemoryStream(Properties.Resources.Rectangle))
            {
                ToolCursor = new Cursor(stream); // Set Cursor.
            }
        }
        #endregion

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

            GraphicsRectangleMark graphic =
                new GraphicsRectangleMark(p.X, p.Y, p.X + 1, p.Y + 1,
                                      drawingCanvas.LineWidth,
                                      drawingCanvas.ObjectColor,
                                      drawingCanvas.ActualScale, false, false, str, GraphicsRegionType.MarkingUnit);
            // Add this Rectangle to GraphicList of drawingCanvas.
            UnitRectProperty rc = new UnitRectProperty();
            graphic.MarkInfo = new MarkItem(MarkingType.GetMarkType(eMarkingType.eMarkingUnitRect), rc, 0);
            AddNewObject(drawingCanvas, graphic);
        }
    }
}
