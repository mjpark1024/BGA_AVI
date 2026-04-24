using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.IO;


namespace Common.Drawing.DrawingTool
{
    class ToolStripGuide : ToolRectangleBase
    {
         #region Ctor.
        public ToolStripGuide()
        {
            // Cursor는 Rectangle 타입을 사용하도록 한다.
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
            foreach (GraphicsBase graphic in drawingCanvas.GraphicsList)
            {
                if (graphic.RegionType == GraphicsRegionType.MarkGuide)
                {
                    drawingCanvas.GraphicsList.Remove(graphic);
                    break;
                }
            }
            SetGraphicsColor(drawingCanvas);

            // Add this Rectangle to GraphicList of drawingCanvas.
            //AddNewObject(drawingCanvas, new GraphicsMarkGuide(p.X, p.Y, drawingCanvas.LineWidth, drawingCanvas.ObjectColor, drawingCanvas.ActualScale));
        }
    }
}
