// http://www.codeproject.com/KB/WPF/WPF_DrawTools.aspx
// Author : Alex Fr
// This article, along with any associated source code and files, is licensed under The Code Project Open License (CPOL)

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;

// Commented by suoow2.

namespace Common.Drawing
{
    /// <summary>   Pointer tool. </summary>
    class ToolPointer : Tool
    {
        /// <summary>   Values that represent SelectionMode.  </summary>
        private enum SelectionMode
        {
            None = 0,
            Move = 1,           // object(s) are moved
            Size = 2,           // object is resized
            GroupSelection = 3
        }

        private SelectionMode selectMode = SelectionMode.None;
        private GraphicsBase resizedObject;
        private int resizedObjectHandle;
        private bool moveRequested = false;
        private bool resizeRequested = false; // 전체 영상에서 Resize 요청 유무 (2012-05-14, suoow2 Added).
        private Point resizedPoint = new Point(0, 0); // Resize되기 전 ROI의 Left Top을 의미함.
        private Size originSize = new Size(0, 0); // Resize되기 전 ROI의 Size.

        private Point lastPoint = new Point(0, 0);
        private CommandChangeState commandChangeState;
        private bool wasMove;

        #region Empty Ctor.
        public ToolPointer()
        {
            //
        }
        #endregion

        #region DarwingCanvas Mouse Event
        public override void OnMouseDown(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            selectMode = SelectionMode.None;

            Point point = e.GetPosition(drawingCanvas);
            GraphicsBase movedObject = null;
            commandChangeState = null;
            wasMove = false;
            int handleNumber;

            // Test for resizing (only if control is selected, cursor is on the handle)
            for (int i = drawingCanvas.GraphicsList.Count - 1; i >= 0; i--)
            {
                if (drawingCanvas[i].IsSelected)
                {
                    handleNumber = drawingCanvas[i].MakeHitTest(point);

                    if (handleNumber > 0)
                    {
                        selectMode = SelectionMode.Size;

                        // keep resized object in class member
                        resizedObject = drawingCanvas[i];
                        resizedObjectHandle = handleNumber;

                        // Since we want to resize only one object, unselect all other objects
                        HelperFunctions.UnselectAll(drawingCanvas);
                        drawingCanvas[i].IsSelected = true;

                        // 전체 영상 View에서는 History 관리를 하지 않는다.
                        if (!drawingCanvas.IsBasedCanvas)
                        {
                            commandChangeState = new CommandChangeState(drawingCanvas);
                        }

                        break;
                    }
                }
            }

            // Test for move (cursor is on the object)
            if (selectMode == SelectionMode.None)
            {
                for (int i = drawingCanvas.GraphicsList.Count - 1; i >= 0; i--)
                {
                    if (drawingCanvas[i].MakeHitTest(point) == 0)
                    {
                        movedObject = drawingCanvas[i];
                        break;
                    }
                }

                if (movedObject != null)
                {
                    selectMode = SelectionMode.Move;

                    // Unselect all if Ctrl is not pressed and clicked object is not selected yet
                    if (Keyboard.Modifiers != ModifierKeys.Control && Keyboard.Modifiers != ModifierKeys.Shift && !movedObject.IsSelected)
                    {
                        HelperFunctions.UnselectAll(drawingCanvas);
                    }
                    movedObject.IsSelected = true;

                    // 전체 영상 View에서는 History 관리를 하지 않는다.
                    if (!drawingCanvas.IsBasedCanvas)
                    {
                        commandChangeState = new CommandChangeState(drawingCanvas);
                    }
                }
            }

            // Click on background
            if (selectMode == SelectionMode.None)
            {
                // Unselect all if Ctrl is not pressed
                if (Keyboard.Modifiers != ModifierKeys.Control && Keyboard.Modifiers != ModifierKeys.Shift)
                {
                    HelperFunctions.UnselectAll(drawingCanvas);
                }

                // Group selection. Create selection rectangle.
                GraphicsSelectionRectangle r = new GraphicsSelectionRectangle(
                    point.X, point.Y,
                    point.X + 1, point.Y + 1,
                    drawingCanvas.ActualScale);

                r.Clip = new RectangleGeometry(new Rect(0, 0, drawingCanvas.ActualWidth, drawingCanvas.ActualHeight));
                drawingCanvas.GraphicsList.Add(r);
                selectMode = SelectionMode.GroupSelection;
            }

            lastPoint = point;

            // Capture mouse until MouseUp event is received
            drawingCanvas.CaptureMouse();
        }

        public override void OnMouseMove(DrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            // Exclude all cases except left button on/off.
            if (e.MiddleButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
                drawingCanvas.Cursor = Cursors.Arrow;
                return;
            }
            Point point = e.GetPosition(drawingCanvas);

            // Set cursor when left button is not pressed
            if (e.LeftButton == MouseButtonState.Released)
            {
                Cursor cursor = Cursors.Arrow;

                int handleNumber = 0;
                foreach(GraphicsBase graphic in drawingCanvas.GraphicsList)
                {
                    handleNumber = graphic.MakeHitTest(point);
                    if (handleNumber > 0)
                    {
                        cursor = graphic.GetHandleCursor(handleNumber);
                        break;
                    }
                }
                drawingCanvas.Cursor = cursor;
                return;
            }

            if (!drawingCanvas.IsMouseCaptured)
            {
                return;
            }
            wasMove = true;

            // Find difference between previous and current position
            double dx = point.X - lastPoint.X;
            double dy = point.Y - lastPoint.Y;

            if (point.X >= drawingCanvas.ActualWidth)
            {
                point.X = drawingCanvas.ActualWidth;
                dx = 0;
            }

            if (point.Y >= drawingCanvas.ActualHeight)
            {
                point.Y = drawingCanvas.ActualHeight;
                dy = 0;
            }

            lastPoint = point;

            if (selectMode == SelectionMode.Size) // ROI 리사이즈.
            {
                if (resizedObject != null)
                {
                    bool bCanResize = true;

                    if (drawingCanvas.IsBasedCanvas && !drawingCanvas.IsMarkCanvas)
                    {
                        if (drawingCanvas.SelectionCount == 1 && resizedObject != null)
                        {
                            GraphicsRectangle resizeGraphic = resizedObject as GraphicsRectangle;
                            if (resizeGraphic != null) // Memorize resize graphics's origin left, top.
                            {
                                resizedPoint.X = resizeGraphic.Left;
                                resizedPoint.Y = resizeGraphic.Top;
                                if (originSize.Width == 0.0 && originSize.Height == 0.0)
                                {
                                    originSize.Width = resizeGraphic.WidthProperty;
                                    originSize.Height = resizeGraphic.HeightProperty;
                                }
                            }
                            bCanResize = true;
                            resizeRequested = true;
                        }
                        else
                        {
                            bCanResize = false;
                            resizeRequested = false;
                        }
                    }
                    else if (drawingCanvas.IsBasedCanvas && drawingCanvas.IsMarkCanvas)
                    {
                        if (DrawingCanvas.FixedSectionROI)
                        {
                            bCanResize = false;
                            resizeRequested = false;
                        }
                        else
                        {
                            if (drawingCanvas.SelectionCount == 1 && resizedObject != null)
                            {
                                if ((resizedObject.RegionType == GraphicsRegionType.MarkingUnit ||
                                    resizedObject.RegionType == GraphicsRegionType.MarkingRail ||
                                    resizedObject.RegionType == GraphicsRegionType.MarkingReject)
                                    && resizedObject.Dummy)
                                {
                                    bCanResize = false;
                                    resizeRequested = false;
                                }
                                else
                                {
                                    bCanResize = true;
                                    resizeRequested = true;
                                }
                            }
                            else
                            {
                                bCanResize = false;
                                resizeRequested = false;
                            }
                        }
                    }
                    else if (resizedObject.RegionType == GraphicsRegionType.LocalAlign) // Local Align Resize 불허.
                        bCanResize = false;

                    if (bCanResize)
                    {
                        resizedObject.MoveHandleTo(point, resizedObjectHandle); // Resize
                    }
                    else
                    {
                        drawingCanvas.Cursor = Cursors.Arrow;
                    }
                }
            }
            else if (selectMode == SelectionMode.Move) // ROI 이동
            {
                if (Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    if (!drawingCanvas.IsBasedCanvas && !DrawingCanvas.FixedInspectROI)
                    {
                        foreach (GraphicsBase graphic in drawingCanvas.Selection)
                        {
                            graphic.Move(dx, dy, drawingCanvas.ActualWidth, drawingCanvas.ActualHeight);
                            // Section 영상에서는 Local Align이 존재할 수 있다.
                            //if (graphic.LocalAligns != null)
                            //{
                            //    for (int nIndex = 0; nIndex < graphic.LocalAligns.Length; nIndex++)
                            //    {
                            //        if (graphic.LocalAligns[nIndex] != null)
                            //        {
                            //            graphic.LocalAligns[nIndex].Move(dx, dy, drawingCanvas.ActualWidth, drawingCanvas.ActualHeight);
                            //        }
                            //    }
                            //}
                        }
                    }
                    else if (drawingCanvas.IsBasedCanvas && drawingCanvas.IsMarkCanvas && !DrawingCanvas.FixedInspectROI)
                    {

                        if (drawingCanvas.SelectionCount == 1)
                        {
                            if (drawingCanvas.SelectedGraphic != null)
                            {
                                if (drawingCanvas.SelectedGraphic.RegionType == GraphicsRegionType.MarkingUnit && drawingCanvas.SelectedGraphic.Dummy)
                                {
                                    moveRequested = false;
                                }
                                else
                                {
                                    foreach (GraphicsBase graphic in drawingCanvas.Selection)
                                    {
                                        graphic.Move(dx, dy, drawingCanvas.ActualWidth, drawingCanvas.ActualHeight);
                                    }
                                    moveRequested = true;
                                }
                            }
                        }
                        else
                        {
                            moveRequested = false;
                        }

                    }
                    else if (!DrawingCanvas.FixedSectionROI)
                    {
                        foreach (GraphicsRectangle graphic in drawingCanvas.SelectionRectangle)
                        {
                            graphic.Move(dx, dy, drawingCanvas.ActualWidth, drawingCanvas.ActualHeight);
                        }
                    }
                }
            }
            else if (selectMode == SelectionMode.GroupSelection)
            {
                // Resize selection rectangle
                drawingCanvas[drawingCanvas.Count - 1].MoveHandleTo(point, 5);
            }
        }

        public override void OnMouseUp(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            if (!drawingCanvas.IsMouseCaptured)
            {
                drawingCanvas.Cursor = Cursors.Arrow;
                selectMode = SelectionMode.None;
                return;
            }

            if (moveRequested)
            {
                if (drawingCanvas.SelectionCount == 1)
                {

                    if (drawingCanvas.SelectedGraphic.RegionType == GraphicsRegionType.MarkingUnit ||
                        drawingCanvas.SelectedGraphic.RegionType == GraphicsRegionType.MarkingRail ||
                        drawingCanvas.SelectedGraphic.RegionType == GraphicsRegionType.MarkingReject ||
                        drawingCanvas.SelectedGraphic.RegionType == GraphicsRegionType.UnitGuide)
                    {
                        // if (resizedObject.MarkInfo.MarkType.MarkType == MarkingInformation.eMarkingType.eMarkingUnitRect)
                        // {
                        // GraphicsRectangleMark resizeGraphic = resizedObject as GraphicsRectangleMark;
                        // if (resizeGraphic == null) return;
                        drawingCanvas.NotifyMarkMoved(drawingCanvas.SelectedGraphic);
                        // }
                    }
                }
                moveRequested = false;
            }

            if (resizedObject != null)
            {
                if (resizeRequested && drawingCanvas.IsBasedCanvas && drawingCanvas.IsMarkCanvas)
                {
                    if (resizedObject != null && drawingCanvas.SelectionCount == 1)
                    {
                        if (resizedObject.RegionType == GraphicsRegionType.MarkingUnit ||
                            resizedObject.RegionType == GraphicsRegionType.MarkingRail ||
                            resizedObject.RegionType == GraphicsRegionType.MarkingReject)
                        {
                            drawingCanvas.NotifyMarkSizeChanged(resizedObject);
                            //if (resizedObject.MarkInfo.MarkType.MarkType == MarkingInformation.eMarkingType.eMarkingUnitRect)
                            // {
                            //     GraphicsRectangleMark resizeGraphic = resizedObject as GraphicsRectangleMark;
                            //     if (resizeGraphic == null) return;
                            //     drawingCanvas.NotifyMarkSizeChanged(resizedObject);
                            // }
                        }
                    }
                }

                if (resizeRequested && drawingCanvas.IsBasedCanvas && !drawingCanvas.IsMarkCanvas)
                {
                    if (resizedObject != null && drawingCanvas.SelectionCount == 1)
                    {
                        GraphicsRectangle resizeGraphic = resizedObject as GraphicsRectangle;
                        if (resizeGraphic.RegionType == GraphicsRegionType.StripAlign || resizeGraphic.RegionType == GraphicsRegionType.IDMark)
                        {
                            drawingCanvas.ReleaseMouseCapture();
                            drawingCanvas.Cursor = Cursors.Arrow;
                            selectMode = SelectionMode.None;

                            // 전체 영상 View에서는 History 관리를 하지 않는다.
                            if (!drawingCanvas.IsBasedCanvas)
                            {
                                AddChangeToHistory(drawingCanvas);
                            }
                            return;
                        }

                        

                        if (resizeGraphic != null)
                        {
                            bool bCanEnlarge = true; // 확대가능한 사이즈 유무.

                            Size newSize = new Size(resizeGraphic.WidthProperty, resizeGraphic.HeightProperty);
                            double fDeltaWidth = newSize.Width - originSize.Width;
                            double fDeltaHeight = newSize.Height - originSize.Height;
                            
                            if (fDeltaWidth != 0 || fDeltaHeight != 0) // 크기가 변경되었다면 로직 수행.
                            {
                                // 외곽 영역이라면 무조건 사이즈 변경이 가능하다.
                                if (resizeGraphic.RegionType == GraphicsRegionType.UnitRegion || resizeGraphic.RegionType == GraphicsRegionType.PSROdd)
                                {
                                    foreach (GraphicsRectangle g in drawingCanvas.GraphicsRectangleList)
                                    {
                                        if (g == resizedObject) continue;
                                        if (g.RegionType == GraphicsRegionType.UnitRegion || g.RegionType == GraphicsRegionType.PSROdd)
                                        {
                                            if ((g.Right + fDeltaWidth > drawingCanvas.Width - 5) ||
                                                (g.Bottom + fDeltaHeight > drawingCanvas.Height - 5))
                                            {
                                                bCanEnlarge = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                               
                                
                                if (bCanEnlarge)
                                {
                                    if (resizeGraphic.RegionType == GraphicsRegionType.OuterRegion)
                                    {
                                        bCanEnlarge = false;
                                        //drawingCanvas.NotifySectionSizeChanged(resizeGraphic, fDeltaWidth, fDeltaHeight);
                                    }
                                    else
                                    {
                                        if (MessageBoxResult.Yes ==
                                            MessageBox.Show("섹션 크기가 변경되었습니다. 적용하시겠습니까?\n(섹션 내부의 ROI가 올바르게 변경되었는지 확인바랍니다.)", "Confirm", MessageBoxButton.YesNo))
                                        {
                                            drawingCanvas.NotifySectionSizeChanged(resizeGraphic, fDeltaWidth, fDeltaHeight);
                                        }
                                        else
                                        {
                                            RevertResize();
                                        }
                                    }
                                    originSize.Width = originSize.Height = 0.0;
                                }
                                else // !bCanEnlarge
                                {
                                    RevertResize();
                                }
                            }
                        }
                    }
                }

                // 전체 ResizedObject에 적용.
                // after resizing
                resizedObject.Normalize();
                resizedObject = null;
            }

            if (selectMode == SelectionMode.GroupSelection && drawingCanvas[drawingCanvas.Count - 1] is GraphicsSelectionRectangle)
            {
                GraphicsSelectionRectangle graphicsSelectionRectangle = (GraphicsSelectionRectangle)drawingCanvas[drawingCanvas.Count - 1];
                graphicsSelectionRectangle.Normalize();

                Rect rect = graphicsSelectionRectangle.Rectangle;
                drawingCanvas.GraphicsList.Remove(graphicsSelectionRectangle);

                foreach (GraphicsBase g in drawingCanvas.GraphicsList)
                {
                    if (g.IntersectsWith(rect))
                    {
                        g.IsSelected = (g.IsSelected) ? false : true;
                        if (g.RegionType == GraphicsRegionType.LocalAlign) g.IsSelected = false;
                    }
                }

                if (drawingCanvas.SelectionCount == 1)
                {
                    foreach(GraphicsBase selectedGraphic in drawingCanvas.Selection)
                    {
                        drawingCanvas.SelectedGraphic = selectedGraphic;
                    }
                }
            }

            drawingCanvas.ReleaseMouseCapture();
            drawingCanvas.Cursor = Cursors.Arrow;
            selectMode = SelectionMode.None;

            // 전체 영상 View에서는 History 관리를 하지 않는다.
            if (!drawingCanvas.IsBasedCanvas)
            {
                AddChangeToHistory(drawingCanvas);
            }
        }

        #endregion DarwingCanvas Mouse Event


        public override void SetCursor(DrawingCanvas drawingCanvas)
        {
            drawingCanvas.Cursor = Cursors.Arrow;
        }

        public void AddChangeToHistory(DrawingCanvas drawingCanvas)
        {
            if (commandChangeState != null && wasMove)
            {
                // Keep state after moving / resizing and add command to history
                commandChangeState.NewState(drawingCanvas);
                drawingCanvas.AddCommandToHistory(commandChangeState);

                commandChangeState = null;
            }
        }

        public void RevertResize()
        {
            if (resizedObject != null)
            {
                GraphicsRectangle resizeGraphic = resizedObject as GraphicsRectangle;
                if (resizeGraphic != null)
                {
                    resizeGraphic.Left = resizedPoint.X;
                    resizeGraphic.Top = resizedPoint.Y;
                    resizeGraphic.Right = resizeGraphic.Left + originSize.Width;
                    resizeGraphic.Bottom = resizeGraphic.Top + originSize.Height;

                    resizeGraphic.CalcBoundaryRect();
                    resizeGraphic.RefreshDrawing();

                    resizedPoint.X = resizedPoint.Y = 0.0;
                    originSize.Width = originSize.Height = 0.0;
                }
            }
        }
    }
}
