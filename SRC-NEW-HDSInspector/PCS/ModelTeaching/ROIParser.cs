/*********************************************************************************
 * Copyright(c) 2015 by Haesung DS.
 * 
 * This software is copyrighted by, and is the sole property of Haesung DS.
 * All rigths, title, ownership, or other interests in the software remain the
 * property of Haesung DS. This software may only be used in accordance with
 * the corresponding license agreement. Any unauthorized use, duplication, 
 * transmission, distribution, or disclosure of this software is expressly 
 * forbidden.
 *
 * This Copyright notice may not be removed or modified without prior written
 * consent of Haesung DS reserves the right to modify this 
 * software without notice.
 *
 * Haesung DS.
 * KOREA 
 * http://www.HaesungDS.com
 *********************************************************************************/
/**
 * @file  ROIParser.cs
 * @brief 
 *  It Parsed ROIInformation <-> GraphicsBase.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.09.16
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.09.16 First creation.
 */

using System;
using System.Collections.Generic;
using Common.Drawing;
using System.Windows;
using System.Windows.Media;

namespace PCS.ModelTeaching
{
    /// <summary>   Roi parser.  </summary>
    /// <remarks>   suoow2, 2014-09-29. </remarks>
    public static class ROIParser
    {
        public static string GraphicsToString(GraphicsBase graphic)
        {
            if (graphic is GraphicsGuideLine)
            {
                return GraphicsGuideLineToString(graphic as GraphicsGuideLine);
            }
            else if (graphic is GraphicsRectangleBase)
            {
                return GraphicsRectangleBaseToString(graphic as GraphicsRectangleBase);
            }
            else if (graphic is GraphicsPolyLine)
            {
                return GraphicsPolyLineToString(graphic as GraphicsPolyLine);
            }
            else if (graphic is GraphicsLine)
            {
                return GraphicsLineToString(graphic as GraphicsLine);
            }
            else return string.Empty;
        }

        public static string LocalAlignsToString(GraphicsBase graphic)
        {
            if (graphic.LocalAligns == null)
            {
                return string.Empty;
            }
            GraphicsRectangle[] localAligns = graphic.LocalAligns;
            string strLocalAlignPoints = string.Empty;
            
            for (int i = 0; i < localAligns.Length; i++)
            {
                if (localAligns[i] != null)
                {
                    strLocalAlignPoints += i + "," +
                        string.Format("{0:f4},", localAligns[i].Left + (localAligns[i].Right - localAligns[i].Left) / 2.0) +
                        string.Format("{0:f4},", localAligns[i].Top + (localAligns[i].Bottom - localAligns[i].Top) / 2.0);
                }
            }

            return strLocalAlignPoints;
        }

        #region Graphics to String.


        private static string GraphicsEllipseMarkToString(GraphicsEllipseMark graphic)
        {
            if (graphic == null)
            {
                return null;
            }

            string strOuterRoiRect = "M,E,";
            strOuterRoiRect += "L" + String.Format("{0:f4}", graphic.CircleInfo.Position.X);
            strOuterRoiRect += "T" + String.Format("{0:f4}", graphic.CircleInfo.Position.Y);
            strOuterRoiRect += "R" + String.Format("{0:f4}", graphic.CircleInfo.Radian);
            strOuterRoiRect += "B" + String.Format("{0:f4}", 0);
            strOuterRoiRect += "W" + String.Format("{0:D1}", (graphic.white) ? 0 : 1);
            strOuterRoiRect += "D" + String.Format("{0:D1}", (graphic.Dummy) ? 0 : 1);
            strOuterRoiRect += "I" + String.Format("{0}", graphic.MotherROIID);
            strOuterRoiRect += "S" + String.Format("{0}", 0);
            strOuterRoiRect += "Z" + String.Format("{0:d2};{1:d2};{2:d2}", graphic.Step, graphic.UnitColumn, graphic.UnitRow);

            return strOuterRoiRect;
        }

        private static string GraphicsRectangleMarkToString(GraphicsRectangleMark graphic)
        {
            if (graphic == null)
            {
                return null;
            }

            string strOuterRoiRect = "M,R,";
            strOuterRoiRect += "L" + String.Format("{0:f4}", graphic.Left);
            strOuterRoiRect += "T" + String.Format("{0:f4}", graphic.Top);
            strOuterRoiRect += "R" + String.Format("{0:f4}", graphic.Right);
            strOuterRoiRect += "B" + String.Format("{0:f4}", graphic.Bottom);
            strOuterRoiRect += "W" + String.Format("{0:D1}", (graphic.white) ? 0 : 1);
            strOuterRoiRect += "D" + String.Format("{0:D1}", (graphic.Dummy) ? 0 : 1);
            strOuterRoiRect += "I" + String.Format("{0}", graphic.MotherROIID);
            strOuterRoiRect += "S" + String.Format("{0}", 0);
            strOuterRoiRect += "Z" + String.Format("{0:d2};{1:d2};{2:d2}", graphic.Step, graphic.UnitColumn, graphic.UnitRow);

            return strOuterRoiRect;
        }

        private static string GraphicsTriangleMarkToString(GraphicsTriangleMark graphic)
        {
            if (graphic == null)
            {
                return null;
            }

            string strOuterRoiRect = "M,T,";
            strOuterRoiRect += "L" + String.Format("{0:f4}", graphic.Left);
            strOuterRoiRect += "T" + String.Format("{0:f4}", graphic.Top);
            strOuterRoiRect += "R" + String.Format("{0:f4}", graphic.Right);
            strOuterRoiRect += "B" + String.Format("{0:f4}", graphic.Bottom);
            strOuterRoiRect += "W" + String.Format("{0:D1}", (graphic.white) ? 0 : 1);
            strOuterRoiRect += "D" + String.Format("{0:D1}", (graphic.Dummy) ? 0 : 1);
            strOuterRoiRect += "I" + String.Format("{0}", 0);
            strOuterRoiRect += "S" + String.Format("{0}", graphic.Rotate);
            strOuterRoiRect += "Z" + String.Format("{0:d2};{1:d2};{2:d2}", graphic.Step, graphic.UnitColumn, graphic.UnitRow);

            return strOuterRoiRect;
        }

        private static string GraphicsSpecialMarkToString(GraphicsSpecialMark graphic)
        {
            if (graphic == null)
            {
                return null;
            }

            string strOuterRoiRect = "M,S,";
            strOuterRoiRect += "L" + String.Format("{0:f4}", graphic.Left);
            strOuterRoiRect += "T" + String.Format("{0:f4}", graphic.Top);
            strOuterRoiRect += "R" + String.Format("{0:f4}", graphic.Right);
            strOuterRoiRect += "B" + String.Format("{0:f4}", graphic.Bottom);
            strOuterRoiRect += "W" + String.Format("{0:D1}", (graphic.white) ? 0 : 1);
            strOuterRoiRect += "D" + String.Format("{0:D1}", (graphic.Dummy) ? 0 : 1);
            strOuterRoiRect += "I" + String.Format("{0}", graphic.MotherROIID);
            strOuterRoiRect += "S" + String.Format("{0}", graphic.Shape);
            strOuterRoiRect += "Z" + String.Format("{0:d2};{1:d2};{2:d2}", graphic.Step, graphic.UnitColumn, graphic.UnitRow);

            return strOuterRoiRect;
        }

        private static string GraphicsGuideLineToString(GraphicsGuideLine graphic)
        {
            if (graphic == null)
            {
                return null;
            }

            string strOuterRoiRect = "G,G,";
            strOuterRoiRect += "L" + String.Format("{0:f4}", graphic.Left);
            strOuterRoiRect += "T" + String.Format("{0:f4}", graphic.Top);
            strOuterRoiRect += "R" + String.Format("{0:f4}", graphic.Right);
            strOuterRoiRect += "B" + String.Format("{0:f4}", graphic.Bottom);

            return strOuterRoiRect;
        }

        public static string GraphicsStripAlignToString(GraphicsStripAlign graphic)
        {
            if (graphic == null)
            {
                return null;
            }

            string strOuterRoiRect = "S,A,";
            strOuterRoiRect += "X" + String.Format("{0:f4}", graphic.cpX);
            strOuterRoiRect += "Y" + String.Format("{0:f4}", graphic.cpY);
            strOuterRoiRect += "N" + String.Format("{0}", graphic.nID);
            return strOuterRoiRect;
        }

        private static string GraphicsStripGuideToString(GraphicsMarkGuide graphic)
        {
            if (graphic == null)
            {
                return null;
            }

            string strOuterRoiRect = "S,G,";
            strOuterRoiRect += "X" + String.Format("{0:f4}", graphic.cpX);
            strOuterRoiRect += "Y" + String.Format("{0:f4}", graphic.cpY);
            strOuterRoiRect += "N" + String.Format("{0}", 0);
            strOuterRoiRect += "N" + String.Format("{0}", 0);
            return strOuterRoiRect;
        }

        private static string GraphicsRectangleBaseToString(GraphicsRectangleBase graphic)
        {
            if (graphic == null)
            {
                return null;
            }

            string strOuterRoiRect = "R,";
            if (graphic is GraphicsEllipse)
            {
                strOuterRoiRect = "E,";
            }

            switch (graphic.RegionType)
            {
                case GraphicsRegionType.Except:
                    strOuterRoiRect += "E,";
                    break;
                case GraphicsRegionType.Inspection:
                    strOuterRoiRect += "I,";
                    break;
                case GraphicsRegionType.UnitAlign:
                    strOuterRoiRect += "U,";
                    break;
                case GraphicsRegionType.WPShift:
                    strOuterRoiRect += "W,";
                    break;
                case GraphicsRegionType.StripAlign:
                    strOuterRoiRect += "S,";
                    break;
                case GraphicsRegionType.IDMark:
                    strOuterRoiRect += "D,";
                    break;
                case GraphicsRegionType.StripOrigin:
                    strOuterRoiRect += "S,";
                    break;
                default:
                    strOuterRoiRect += "I,";
                    break;
            }

            strOuterRoiRect += "L" + String.Format("{0:f4}", graphic.Left);
            strOuterRoiRect += "T" + String.Format("{0:f4}", graphic.Top);
            strOuterRoiRect += "R" + String.Format("{0:f4}", graphic.Right);
            strOuterRoiRect += "B" + String.Format("{0:f4}", graphic.Bottom);
            strOuterRoiRect += "Z" + String.Format("{0:d2}", graphic.UnitRow);

            return strOuterRoiRect;
        }

        private static string GraphicsPolyLineToString(GraphicsPolyLine graphic)
        {
            if (graphic == null)
            {
                return null;
            }

            string strOuterRoiRect = "P,";

            switch (graphic.RegionType)
            {
                case GraphicsRegionType.Except:
                    strOuterRoiRect += "E,";
                    break;
                case GraphicsRegionType.Inspection:
                    strOuterRoiRect += "I,";
                    break;
                case GraphicsRegionType.UnitAlign:
                    strOuterRoiRect += "U,";
                    break;
                case GraphicsRegionType.StripAlign:
                    strOuterRoiRect += "S,";
                    break;
                default:
                    strOuterRoiRect += "I,";
                    break;
            }

            int nPoints = graphic.Points.Length;
            for (int i = 0; i < nPoints; i++)
            {
                strOuterRoiRect += "X" + String.Format("{0:f4}", graphic.Points[i].X);
                strOuterRoiRect += "Y" + String.Format("{0:f4}", graphic.Points[i].Y);
            }

            return strOuterRoiRect;
        }

        private static string GraphicsLineToString(GraphicsLine graphic)
        {
            if (graphic == null)
            {
                return null;
            }

            string strOuterRoiRect = "L,";

            switch (graphic.RegionType)
            {
                case GraphicsRegionType.Except:
                    strOuterRoiRect += "E,";
                    break;
                case GraphicsRegionType.Inspection:
                    strOuterRoiRect += "I,";
                    break;
                case GraphicsRegionType.UnitAlign:
                    strOuterRoiRect += "U,";
                    break;
                case GraphicsRegionType.StripAlign:
                    strOuterRoiRect += "S,";
                    break;
                default:
                    strOuterRoiRect += "I,";
                    break;
            }

            strOuterRoiRect += "X" + String.Format("{0:f4}", graphic.Start.X);
            strOuterRoiRect += "Y" + String.Format("{0:f4}", graphic.Start.Y);
            strOuterRoiRect += "X" + String.Format("{0:f4}", graphic.End.X);
            strOuterRoiRect += "Y" + String.Format("{0:f4}", graphic.End.Y);

            return strOuterRoiRect;
        }
        #endregion

        #region String to Graphics.
        public static void CreateLocalAlignsToCanvas(string strLocalAlignInfo, GraphicsBase motherROI, DrawingCanvas roiCanvas)
        {
            if (strLocalAlignInfo.Length < 4)
            {
                return;
            }
            int index = 0;
            double fCenterX = 0.0;
            double fCenterY = 0.0;

            motherROI.LocalAligns = new GraphicsRectangle[Definitions.MAX_LOCAL_ALIGN_COUNT];

            string[] localAlignTokens = strLocalAlignInfo.Split(',');
            for(int i = 0; i < localAlignTokens.Length; i += 3)
            {
                if (i + 2 >= localAlignTokens.Length)
                {
                    break;
                }

                index = Convert.ToInt32(localAlignTokens[i]);
                fCenterX = Convert.ToDouble(localAlignTokens[i + 1]);
                fCenterY = Convert.ToDouble(localAlignTokens[i + 2]);

                motherROI.LocalAligns[index] = new GraphicsRectangle(
                        fCenterX - 15, // Left
                        fCenterY - 15, // Top
                        fCenterX + 15, // Right
                        fCenterY + 15, // Bottom
                        roiCanvas.LineWidth,
                        GraphicsRegionType.LocalAlign,
                        GraphicsColors.Red,
                        roiCanvas.ActualScale,
                        null, null, null, false, motherROI
                        );
                motherROI.LocalAligns[index].Caption = CaptionHelper.LocalAlignCaption;
                roiCanvas.GraphicsList.Add(motherROI.LocalAligns[index]);
            }
        }

        public static void SetRegionType(char strRoiRectInfo, out GraphicsRegionType regionType, out Color objectColor)
        {
            switch (strRoiRectInfo)
            {
                case 'E': // 검사 제외 영역
                    regionType = GraphicsRegionType.Except;
                    objectColor = GraphicsColors.Blue;
                    break;
                case 'I': // 검사 영역
                    regionType = GraphicsRegionType.Inspection;
                    objectColor = GraphicsColors.Green;
                    break;
                case 'U': // 유닛 Align 영역
                    regionType = GraphicsRegionType.UnitAlign;
                    objectColor = GraphicsColors.Red;
                    break;
                case 'W': // 유닛 Align 영역
                    regionType = GraphicsRegionType.WPShift;
                    objectColor = GraphicsColors.Red;
                    break;
                case 'S': // 유닛 Align 영역
                    regionType = GraphicsRegionType.StripAlign;
                    objectColor = GraphicsColors.Red;
                    break;
                case 'D': // 유닛 Align 영역
                    regionType = GraphicsRegionType.IDMark;
                    objectColor = GraphicsColors.Yellow;
                    break;
                case 'G': // 가이드 라인 영역
                    regionType = GraphicsRegionType.GuideLine;
                    objectColor = GraphicsColors.Yellow;
                    break;
                case 'X': // 가이드 라인 영역
                    regionType = GraphicsRegionType.TapeLoaction;
                    objectColor = GraphicsColors.Yellow;
                    break;
                default:
                    regionType = GraphicsRegionType.Inspection;
                    objectColor = GraphicsColors.Green;
                    break;
            }
        }

        public static GraphicsBase CreateGuideGraphicsToCanvas(string strRoiRectInfo, DrawingCanvas roiCanvas)
        {
            if (strRoiRectInfo.Length < 4)
            {
                return null;
            }
            GraphicsBase graphic = null;
            GraphicsRegionType graphicRegionType = GraphicsRegionType.None;
            Color graphicObjectColor = Colors.Transparent;

            SetRegionType(strRoiRectInfo[2], out graphicRegionType, out graphicObjectColor);

            switch (strRoiRectInfo[0])
            {
                case 'G': // Guide Line
                    string[] guideLineTokens = strRoiRectInfo.Split(',', 'L', 'T', 'R', 'B');
                    if (guideLineTokens.Length >= 7)
                    {
                        double left = Convert.ToDouble(guideLineTokens[3]);
                        double top = Convert.ToDouble(guideLineTokens[4]);
                        double right = Convert.ToDouble(guideLineTokens[5]);
                        double bottom = Convert.ToDouble(guideLineTokens[6]);
                        int bp = 0;
                        Point p = new Point(1, 1);
                        GraphicsGuideLine graphicsGuideLine = new GraphicsGuideLine(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale, p);
                        graphicsGuideLine.SetBasePoint(bp, p);
                        roiCanvas.UnitGuidePoint = new Point(right, top);
                        graphic = graphicsGuideLine;
                    }
                    break;
                case 'S': // Guide Line
                    string[] stripGuideTokens = strRoiRectInfo.Split(',', 'A', 'X', 'Y', 'N');
                    if (stripGuideTokens.Length >= 7)
                    {
                        double cpx = Convert.ToDouble(stripGuideTokens[3]);
                        double cpy = Convert.ToDouble(stripGuideTokens[4]);
                        // GraphicsMarkGuide graphicsStripGuide = new GraphicsMarkGuide(cpx,cpy, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale);
                        roiCanvas.StripGuidePoint = new Point(cpx, cpy);
                        //  graphic = graphicsStripGuide;
                    }
                    break;
            }
            if (graphic != null)
            {
                roiCanvas.GraphicsList.Add(graphic);
                graphic.RefreshDrawing();
                return graphic;
            }
            return null;
        }

        public static GraphicsBase CreateGraphicsToCanvas(string strRoiRectInfo, DrawingCanvas roiCanvas)
        {
            if (strRoiRectInfo.Length < 4)
            {
                return null;
            }

            GraphicsBase graphic = null;
            GraphicsRegionType graphicRegionType = GraphicsRegionType.None;
            Color graphicObjectColor = Colors.Transparent;

            SetRegionType(strRoiRectInfo[2], out graphicRegionType, out graphicObjectColor);
            #region Parsing ROI.
            switch (strRoiRectInfo[0])
            {
                case 'R': // Rectangle
                    string[] rectangleTokens = strRoiRectInfo.Substring(4).Split('L', 'T', 'R', 'B', 'Z');
                    if (rectangleTokens.Length == 5)
                    {
                        double left = Convert.ToDouble(rectangleTokens[1]);
                        double top = Convert.ToDouble(rectangleTokens[2]);
                        double right = Convert.ToDouble(rectangleTokens[3]);
                        double bottom = Convert.ToDouble(rectangleTokens[4]);

                        GraphicsRectangle graphicsRectangle = new GraphicsRectangle(
                            left,
                            top,
                            right,
                            bottom,
                            roiCanvas.LineWidth,
                            graphicRegionType,
                            graphicObjectColor,
                            roiCanvas.ActualScale,
                            new IterationSymmetryInformation(1, 1, 1, 1), // ROI에는 실제로 IterationSymmetryInformation이 필요하지 않다. // 필요시 수정 요망. HMS
                            new IterationInformation(1, 1, right - left, bottom - top), // ROI에는 실제로 IterationInformation이 필요하지 않다.
                            new IterationInformation(1, 1, 0, 0), // ROI에는 실제로 IterationInformation이 필요하지 않다.
                            false,
                            null);

                        graphic = graphicsRectangle;
                        graphic.UnitRow = 0;
                    }
                    else if (rectangleTokens.Length == 6)
                    {
                        double left = Convert.ToDouble(rectangleTokens[1]);
                        double top = Convert.ToDouble(rectangleTokens[2]);
                        double right = Convert.ToDouble(rectangleTokens[3]);
                        double bottom = Convert.ToDouble(rectangleTokens[4]);

                        GraphicsRectangle graphicsRectangle = new GraphicsRectangle(
                            left,
                            top,
                            right,
                            bottom,
                            roiCanvas.LineWidth,
                            graphicRegionType,
                            graphicObjectColor,
                            roiCanvas.ActualScale,
                            new IterationSymmetryInformation(1, 1, 1, 1), // ROI에는 실제로 IterationSymmetryInformation이 필요하지 않다. // 필요시 수정 요망. HMS
                            new IterationInformation(1, 1, right - left, bottom - top), // ROI에는 실제로 IterationInformation이 필요하지 않다.
                            new IterationInformation(1, 1, 0, 0), // ROI에는 실제로 IterationInformation이 필요하지 않다.
                            false,
                            null);

                        graphic = graphicsRectangle;
                        graphic.UnitRow = Convert.ToInt32(rectangleTokens[5]);
                    }
                    break;
                case 'E': // Ellipse
                    string[] ellipseTokens = strRoiRectInfo.Substring(4).Split('L', 'T', 'R', 'B', 'Z');
                    if (ellipseTokens.Length == 5)
                    {
                        double left = Convert.ToDouble(ellipseTokens[1]);
                        double top = Convert.ToDouble(ellipseTokens[2]);
                        double right = Convert.ToDouble(ellipseTokens[3]);
                        double bottom = Convert.ToDouble(ellipseTokens[4]);

                        GraphicsEllipse graphicsEllipse = new GraphicsEllipse(
                            left,
                            top,
                            right,
                            bottom,
                            roiCanvas.LineWidth,
                            graphicRegionType,
                            graphicObjectColor,
                            roiCanvas.ActualScale);

                        graphic = graphicsEllipse;
                    }
                    else if (ellipseTokens.Length == 6)
                    {
                        double left = Convert.ToDouble(ellipseTokens[1]);
                        double top = Convert.ToDouble(ellipseTokens[2]);
                        double right = Convert.ToDouble(ellipseTokens[3]);
                        double bottom = Convert.ToDouble(ellipseTokens[4]);

                        GraphicsEllipse graphicsEllipse = new GraphicsEllipse(
                            left,
                            top,
                            right,
                            bottom,
                            roiCanvas.LineWidth,
                            graphicRegionType,
                            graphicObjectColor,
                            roiCanvas.ActualScale);

                        graphic = graphicsEllipse;
                        graphic.UnitRow = Convert.ToInt32(ellipseTokens[5]);
                    }
                    break;
                case 'P': // PolyLine
                    string[] polyLineTokens = strRoiRectInfo.Substring(4).Split('X', 'Y');
                    if (polyLineTokens.Length > 5)
                    {
                        int tokenIndex = 1;
                        int nPoints = (polyLineTokens.Length - 1) / 2;

                        List<Point> points = new List<Point>();
                        for (int i = 0; i < nPoints; i++)
                        {
                            points.Add(new Point(Convert.ToDouble(polyLineTokens[tokenIndex++]), Convert.ToDouble(polyLineTokens[tokenIndex++])));
                        }

                        GraphicsPolyLine graphicsPolyLine = new GraphicsPolyLine(
                            points,
                            roiCanvas.LineWidth,
                            graphicRegionType,
                            graphicObjectColor,
                            roiCanvas.ActualScale);

                        graphic = graphicsPolyLine;
                    }
                    break;
                case 'L': // Line
                    string[] lineTokens = strRoiRectInfo.Substring(4).Split('X', 'Y');
                    if (lineTokens.Length == 5)
                    {
                        double startX = Convert.ToDouble(lineTokens[1]);
                        double startY = Convert.ToDouble(lineTokens[2]);
                        double endX = Convert.ToDouble(lineTokens[3]);
                        double endY = Convert.ToDouble(lineTokens[4]);

                        GraphicsLine graphicsLine = new GraphicsLine(
                            new Point(startX, startY),
                            new Point(endX, endY),
                            roiCanvas.LineWidth,
                            graphicRegionType,
                            graphicObjectColor,
                            roiCanvas.ActualScale);

                        graphic = graphicsLine;
                    }
                    break;
            }
            #endregion Parsing ROI.

            if (graphic != null)
            {
                roiCanvas.GraphicsList.Add(graphic);
                graphic.RefreshDrawing();
                return graphic;
            }
            return null;
        }

        public static GraphicsBase CreateMarkGraphicsToCanvas(string strRoiRectInfo, DrawingCanvas roiCanvas)
        {
            if (strRoiRectInfo.Length < 4)
            {
                return null;
            }

            GraphicsBase graphic = null;
            // GraphicsRegionType graphicRegionType = GraphicsRegionType.Marking;
            Color graphicObjectColor = Colors.Yellow;
            string[] rectangleTokens = strRoiRectInfo.Substring(4).Split('L', 'T', 'R', 'B', 'W', 'D', 'I', 'S', 'Z');
            if (rectangleTokens.Length == 10)
            {
                double left = Convert.ToDouble(rectangleTokens[1]);
                double top = Convert.ToDouble(rectangleTokens[2]);
                double right = Convert.ToDouble(rectangleTokens[3]);
                double bottom = Convert.ToDouble(rectangleTokens[4]);
                bool white = (rectangleTokens[5] == "0") ? false : true;
                bool dummy = (rectangleTokens[6] == "0") ? false : true;
                string id = rectangleTokens[7];
                int sp = Convert.ToInt32(rectangleTokens[8]);
                string[] unit = rectangleTokens[9].Split(';');
                switch (strRoiRectInfo[2])
                {
                    case 'E':
                        GraphicsEllipseMark graphicse = new GraphicsEllipseMark(new Circle(new Point(left, top), right), roiCanvas.LineWidth, roiCanvas.ActualScale, white, dummy, id, GraphicsRegionType.MarkingRail);
                        graphic = graphicse;
                        graphic.Step = Convert.ToInt32(unit[0]);
                        graphic.UnitColumn = Convert.ToInt32(unit[1]);
                        graphic.UnitRow = Convert.ToInt32(unit[2]);
                        break;
                    case 'R':
                        GraphicsRectangleMark graphicsr = new GraphicsRectangleMark(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale, white, dummy, id, GraphicsRegionType.MarkingUnit);
                        graphic = graphicsr;
                        graphic.Step = Convert.ToInt32(unit[0]);
                        graphic.UnitColumn = Convert.ToInt32(unit[1]);
                        graphic.UnitRow = Convert.ToInt32(unit[2]);
                        break;
                    case 'T':
                        GraphicsTriangleMark graphicst = new GraphicsTriangleMark(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale, white, dummy, id, sp, GraphicsRegionType.MarkingUnit);
                        graphic = graphicst;
                        graphic.Step = Convert.ToInt32(unit[0]);
                        graphic.UnitColumn = Convert.ToInt32(unit[1]);
                        graphic.UnitRow = Convert.ToInt32(unit[2]);
                        break;
                    case 'S':
                        GraphicsSpecialMark graphicss = new GraphicsSpecialMark(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, id, roiCanvas.ActualScale, white, dummy, sp, GraphicsRegionType.MarkingUnit);
                        graphic = graphicss;
                        graphic.Step = Convert.ToInt32(unit[0]);
                        graphic.UnitColumn = Convert.ToInt32(unit[1]);
                        graphic.UnitRow = Convert.ToInt32(unit[2]);
                        break;
                }

            }

            if (graphic != null)
            {
                graphic.RegionType = GraphicsRegionType.MarkingUnit;
                roiCanvas.GraphicsList.Add(graphic);
                graphic.RefreshDrawing();
                return graphic;
            }
            return null;
        }

        public static GraphicsBase CreateMarkStripAlignGraphicsToCanvas(string strRoiRectInfo, DrawingCanvas roiCanvas)
        {
            if (strRoiRectInfo.Length < 4)
            {
                return null;
            }

            GraphicsBase graphic = null;
            GraphicsRegionType graphicRegionType = GraphicsRegionType.None;
            Color graphicObjectColor = Colors.Transparent;

            SetRegionType(strRoiRectInfo[2], out graphicRegionType, out graphicObjectColor);
            graphicRegionType = GraphicsRegionType.StripAlign;
            graphicObjectColor = Colors.Red;
            #region Parsing ROI.
            switch (strRoiRectInfo[0])
            {


                case 'S': // Guide Line
                    string[] stripalignTokens = strRoiRectInfo.Split(',', 'A', 'X', 'Y', 'N');
                    if (stripalignTokens.Length >= 7)
                    {
                        double cpx = Convert.ToDouble(stripalignTokens[4]);
                        double cpy = Convert.ToDouble(stripalignTokens[5]);
                        int n = Convert.ToInt32(stripalignTokens[6]);
                        GraphicsStripAlign graphicsstripalign = new GraphicsStripAlign(cpx, cpy, n, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale);
                        graphic = graphicsstripalign;
                    }
                    if (stripalignTokens.Length == 6)
                    {
                        double cpx = Convert.ToDouble(stripalignTokens[4]);
                        double cpy = Convert.ToDouble(stripalignTokens[5]);
                        int n = 0;
                        GraphicsStripAlign graphicsstripalign = new GraphicsStripAlign(cpx, cpy, n, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale);
                        graphic = graphicsstripalign;
                    }
                    break;
            }
            #endregion Parsing ROI.

            if (graphic != null)
            {
                roiCanvas.GraphicsList.Add(graphic);
                graphic.RefreshDrawing();
                return graphic;
            }
            return null;
        }


        public static GraphicsBase CreateStripAlignGraphicsToCanvas(string strRoiRectInfo, DrawingCanvas roiCanvas)
        {
            if (strRoiRectInfo.Length < 4)
            {
                return null;
            }

            GraphicsBase graphic = null;
            GraphicsRegionType graphicRegionType = GraphicsRegionType.None;
            Color graphicObjectColor = Colors.Transparent;

            SetRegionType(strRoiRectInfo[2], out graphicRegionType, out graphicObjectColor);
            graphicRegionType = GraphicsRegionType.StripOrigin;
            graphicObjectColor = Colors.Red;
            #region Parsing ROI.
            switch (strRoiRectInfo[0])
            {

                case 'R':
                    string[] stripalignTokens = strRoiRectInfo.Split(',', 'R', 'S', 'L', 'T', 'B');
                    if (stripalignTokens.Length >= 7)
                    {
                        double cpx = Convert.ToDouble(stripalignTokens[5]);
                        double cpy = Convert.ToDouble(stripalignTokens[6]);
                        //int n = Convert.ToInt32(stripalignTokens[6]);
                        int n = 0;
                        GraphicsStripOrigin graphicsstripalign = new GraphicsStripOrigin(cpx + 50, cpy + 50, n, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale);
                        graphic = graphicsstripalign;
                    }
                    if (stripalignTokens.Length == 6)
                    {
                        double cpx = Convert.ToDouble(stripalignTokens[4]);
                        double cpy = Convert.ToDouble(stripalignTokens[5]);
                        int n = 0;
                        GraphicsStripOrigin graphicsstripalign = new GraphicsStripOrigin(cpx, cpy, n, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale);
                        graphic = graphicsstripalign;
                    }
                    break;
            }
        
            #endregion Parsing ROI.

            if (graphic != null)
            {
                roiCanvas.GraphicsList.Add(graphic);
                graphic.RefreshDrawing();
                return graphic;
            }
            return null;
        }

        public static GraphicsBase CreateIDMarkGraphicsToCanvas(string strRoiRectInfo, DrawingCanvas roiCanvas)
        {
            if (strRoiRectInfo.Length < 4)
            {
                return null;
            }

            GraphicsBase graphic = null;
            GraphicsRegionType graphicRegionType = GraphicsRegionType.None;
            Color graphicObjectColor = Colors.Transparent;

            SetRegionType(strRoiRectInfo[2], out graphicRegionType, out graphicObjectColor);
            graphicRegionType = GraphicsRegionType.IDMark;
            graphicObjectColor = Colors.Yellow;
            #region Parsing ROI.
            switch (strRoiRectInfo[0])
            {
                case 'R': // Rectangle
                    string[] rectangleTokens = strRoiRectInfo.Substring(4).Split('L', 'T', 'R', 'B', 'Z');
                    if (rectangleTokens.Length == 5 || rectangleTokens.Length == 6)
                    {
                        double left = Convert.ToDouble(rectangleTokens[1]);
                        double top = Convert.ToDouble(rectangleTokens[2]);
                        double right = Convert.ToDouble(rectangleTokens[3]);
                        double bottom = Convert.ToDouble(rectangleTokens[4]);

                        GraphicsRectangle graphicsRectangle = new GraphicsRectangle(
                            left,
                            top,
                            right,
                            bottom,
                            roiCanvas.LineWidth,
                            graphicRegionType,
                            graphicObjectColor,
                            roiCanvas.ActualScale,
                            new IterationSymmetryInformation(1, 1, 1, 1), // ROI에는 실제로 IterationSymmetryInformation이 필요하지 않다. // 필요시 수정 요망. HMS
                            new IterationInformation(1, 1, right - left, bottom - top), // ROI에는 실제로 IterationInformation이 필요하지 않다.
                            new IterationInformation(1, 1, 0, 0), // ROI에는 실제로 IterationInformation이 필요하지 않다.
                            false,
                            null);

                        graphic = graphicsRectangle;
                        graphic.UnitRow = 0;
                    }
                    break;
                case 'E': // Ellipse
                    string[] ellipseTokens = strRoiRectInfo.Substring(4).Split('L', 'T', 'R', 'B', 'Z');
                    if (ellipseTokens.Length == 5 || ellipseTokens.Length == 6)
                    {
                        double left = Convert.ToDouble(ellipseTokens[1]);
                        double top = Convert.ToDouble(ellipseTokens[2]);
                        double right = Convert.ToDouble(ellipseTokens[3]);
                        double bottom = Convert.ToDouble(ellipseTokens[4]);

                        GraphicsEllipse graphicsEllipse = new GraphicsEllipse(
                            left,
                            top,
                            right,
                            bottom,
                            roiCanvas.LineWidth,
                            graphicRegionType,
                            graphicObjectColor,
                            roiCanvas.ActualScale);

                        graphic = graphicsEllipse;
                        graphic.UnitRow = 0;
                    }
                    break;
                case 'P': // PolyLine
                    string[] polyLineTokens = strRoiRectInfo.Substring(4).Split('X', 'Y');
                    if (polyLineTokens.Length > 5)
                    {
                        int tokenIndex = 1;
                        int nPoints = (polyLineTokens.Length - 1) / 2;

                        List<Point> points = new List<Point>();
                        for (int i = 0; i < nPoints; i++)
                        {
                            points.Add(new Point(Convert.ToDouble(polyLineTokens[tokenIndex++]), Convert.ToDouble(polyLineTokens[tokenIndex++])));
                        }

                        GraphicsPolyLine graphicsPolyLine = new GraphicsPolyLine(
                            points,
                            roiCanvas.LineWidth,
                            graphicRegionType,
                            graphicObjectColor,
                            roiCanvas.ActualScale);

                        graphic = graphicsPolyLine;
                    }
                    break;
                case 'L': // Line
                    string[] lineTokens = strRoiRectInfo.Substring(4).Split('X', 'Y');
                    if (lineTokens.Length == 5)
                    {
                        double startX = Convert.ToDouble(lineTokens[1]);
                        double startY = Convert.ToDouble(lineTokens[2]);
                        double endX = Convert.ToDouble(lineTokens[3]);
                        double endY = Convert.ToDouble(lineTokens[4]);

                        GraphicsLine graphicsLine = new GraphicsLine(
                            new Point(startX, startY),
                            new Point(endX, endY),
                            roiCanvas.LineWidth,
                            graphicRegionType,
                            graphicObjectColor,
                            roiCanvas.ActualScale);

                        graphic = graphicsLine;
                    }
                    break;
            }
            #endregion Parsing ROI.

            if (graphic != null)
            {
                roiCanvas.GraphicsList.Add(graphic);
                graphic.RefreshDrawing();
                return graphic;
            }
            return null;
        }
        #endregion

        #region GraphicsBase to RoiInfo(IS SW)
        private enum DrawType
        {
            NONE = 0,		// 없음
            Rectangle = 1,	// 사각형
            Ellipse = 2,	// 원형
            Cross = 3,		// X 모양 (단순 Diplay용)
            Line = 4,		// 선형
            PolyLine = 5	// 다각형
        }

        public static StripAlignInfo GraphicsToIS(int roiID, GraphicsBase graphic)
        {
            StripAlignInfo si = new StripAlignInfo();
            si.RoiID = roiID;
            GraphicsRectangleBase graphicsRectangleBase = (GraphicsRectangleBase)graphic;
            si.BndRect.X = (int)graphicsRectangleBase.Left;
            si.BndRect.Y = (int)graphicsRectangleBase.Top;
            si.BndRect.Width = Math.Abs((int)graphicsRectangleBase.Left - (int)graphicsRectangleBase.Right);
            si.BndRect.Height = Math.Abs((int)graphicsRectangleBase.Top - (int)graphicsRectangleBase.Bottom);
            si.SearchMarginX = 300;
            si.SearchMarginY = 300;
            si.Match = 70;
            return si;
        }

        public static RoiInfo GraphicsToIS(int roiID, int sectionID, GraphicsBase graphic)
        {
            RoiInfo roiInformation = new RoiInfo();
            roiInformation.RoiID = roiID;
            roiInformation.UnitRow = graphic.UnitRow;
            roiInformation.SectionID = sectionID;
            roiInformation.InnerDrawType = (int)DrawType.NONE;
            roiInformation.InnerPointsCount = 0;
            roiInformation.InnerPoints = null;

            if (graphic.LocalAligns != null)
            {
                for (int i = 0; i < graphic.LocalAligns.Length; i++)
                {
                    if (graphic.LocalAligns[i] != null)
                    {
                        roiInformation.LocalAlignPoints.Add(new System.Drawing.Point(
                            (int)Math.Round(graphic.LocalAligns[i].Left + (graphic.LocalAligns[i].Right - graphic.LocalAligns[i].Left) / 2.0),
                            (int)Math.Round(graphic.LocalAligns[i].Top + (graphic.LocalAligns[i].Bottom - graphic.LocalAligns[i].Top) / 2.0)
                            ));
                    }
                }
            }

            if (!(graphic is GraphicsPolyLine))
            {
                // graphic is line / rectangle / ellipse.
                roiInformation.OuterPointsCount = 2;
                roiInformation.OuterPoints = new System.Drawing.Point[2];

                if (graphic is GraphicsLine)
                {
                    GraphicsLine graphicsLine = (GraphicsLine)graphic;

                    roiInformation.OuterDrawType = (int)DrawType.Line;
                    roiInformation.OuterPoints[0].X = (int)Math.Round(graphicsLine.Start.X); // 시작점
                    roiInformation.OuterPoints[0].Y = (int)Math.Round(graphicsLine.Start.Y);
                    roiInformation.OuterPoints[1].X = (int)Math.Round(graphicsLine.End.X);   // 끝점
                    roiInformation.OuterPoints[1].Y = (int)Math.Round(graphicsLine.End.Y);
                }
                else if (graphic is GraphicsRectangleBase)
                {
                    GraphicsRectangleBase graphicsRectangleBase = (GraphicsRectangleBase)graphic;

                    if (graphicsRectangleBase is GraphicsEllipse)
                    {
                        roiInformation.OuterDrawType = (int)DrawType.Ellipse;
                    }
                    else
                    {
                        roiInformation.OuterDrawType = (int)DrawType.Rectangle;
                    }
                    roiInformation.OuterPoints[0].X = (int)Math.Round(graphicsRectangleBase.Left);   // 좌상귀
                    roiInformation.OuterPoints[0].Y = (int)Math.Round(graphicsRectangleBase.Top);
                    roiInformation.OuterPoints[1].X = (int)Math.Round(graphicsRectangleBase.Right);  // 우하귀
                    roiInformation.OuterPoints[1].Y = (int)Math.Round(graphicsRectangleBase.Bottom);
                }
            }
            else
            {
                GraphicsPolyLine graphicsPolyLine = (GraphicsPolyLine)graphic;

                roiInformation.OuterDrawType = (int)DrawType.PolyLine;
                roiInformation.OuterPointsCount = graphicsPolyLine.Points.Length;

                // System.Drawing.Point = int x, int y (8 bytes)
                roiInformation.OuterPoints = new System.Drawing.Point[graphicsPolyLine.Points.Length];
                int nIndex = 0;
                foreach (Point point in graphicsPolyLine.Points)
                {
                    roiInformation.OuterPoints[nIndex].X = (int)Math.Round(point.X);
                    roiInformation.OuterPoints[nIndex].Y = (int)Math.Round(point.Y);
                    nIndex++;
                }
            }

            return roiInformation;
        }
        #endregion
    }
}
