using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace Common.Drawing
{
    class ToolStripAlign : ToolRectangleBase
    {
        #region Ctor.

        public ToolStripAlign()
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

            if (p.X < 0)
                p.X = 0;
            if (p.Y < 0)
                p.Y = 0;
            int n = 0;
            if (drawingCanvas.ID == 0)
            {
                if (p.Y > 1500) n = 1;
                else if (p.X > 1500) n = 2;
            }
            else
            {
                if (drawingCanvas.bRotate)
                {
                    n = 2;
                    if (p.Y > 1500) n = 1;
                    else if (p.X > 1500) n = 0;
                }
                else
                {
                    if (p.Y > 1500) n = 1;
                    else if (p.X > 1500) n = 2;
                }
            }
            foreach (GraphicsBase graphic in drawingCanvas.GraphicsList)
            {
                if (graphic.RegionType == GraphicsRegionType.StripAlign)
                {
                    if (((GraphicsStripAlign)graphic).nID == n)
                    {
                        drawingCanvas.GraphicsList.Remove(graphic);
                        break;
                    }
                }
            }
            SetGraphicsColor(drawingCanvas);

            // Add this Rectangle to GraphicList of drawingCanvas.
            AddNewObject(drawingCanvas, new GraphicsStripAlign(p.X, p.Y, n, drawingCanvas.LineWidth, drawingCanvas.ObjectColor, drawingCanvas.ActualScale));
        }
    }

    class ToolStripOrigin : ToolRectangleBase
    {
        #region Ctor.

        public ToolStripOrigin()
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

            if (p.X < 0)
                p.X = 0;
            if (p.Y < 0)
                p.Y = 0;
            int n = 0;
           
            foreach (GraphicsBase graphic in drawingCanvas.GraphicsList)
            {
                if (graphic.RegionType == GraphicsRegionType.StripOrigin)
                {
                        drawingCanvas.GraphicsList.Remove(graphic);
                        break;
                }
            }
            SetGraphicsColor(drawingCanvas);

            // Add this Rectangle to GraphicList of drawingCanvas.
            AddNewObject(drawingCanvas, new GraphicsStripOrigin(p.X, p.Y, n, drawingCanvas.LineWidth, drawingCanvas.ObjectColor, drawingCanvas.ActualScale));
        }
    }
}
