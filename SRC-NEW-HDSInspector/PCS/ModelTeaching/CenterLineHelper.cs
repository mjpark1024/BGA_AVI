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
 * @file  CenterLineHelper.cs
 * @brief 
 *  Load / Save Center Line information.
 * 
 * @author : suoow2
 * @date : 2012.08.08
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.08 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Drawing;
using Common;
using System.IO;

namespace PCS.ModelTeaching
{
    public static class CenterLineHelper
    {
        public static void AppendCenterLineDatum(string aszFilePath, string aszSectionCode, string aszRoiCode, string aszInspCode, GraphicsSkeletonLine[] aLineSegments)
        {
            if (File.Exists(aszFilePath))
            {
                using (StreamWriter sw = new StreamWriter(aszFilePath, true))
                {
                    sw.WriteLine("%C");
                    sw.WriteLine(string.Format("Section={0}", aszSectionCode));
                    sw.WriteLine(string.Format("ROI={0}", aszRoiCode));
                    sw.WriteLine(string.Format("Insp={0}", aszInspCode));

                    foreach (GraphicsSkeletonLine lineSegment in aLineSegments)
                    {
                        sw.WriteLine("%L");
                        sw.WriteLine(string.Format("L={0}", lineSegment.LineDir));
                        sw.WriteLine(string.Format("B={0}", lineSegment.BoundaryRect.ToFile()));
                        sw.WriteLine(string.Format("M={0:F2}", lineSegment.MedianSize));
                        sw.WriteLine(string.Format("T={0}", lineSegment.Type));

                        foreach (SkeletonNode node in lineSegment.Nodes)
                        {
                            sw.Write("%N");
                            sw.Write(string.Format(" L={0}", node.LineDirection));
                            sw.Write(string.Format(" P={0}", node.Position.ToFile()));
                            sw.Write(string.Format(" M={0:F2}", node.MeasureSize));
                            sw.WriteLine(string.Format(" T={0}", node.Type));
                        }
                    }
                }
            }
        }

        public static void AppendBallDatum(string aszFilePath, string aszSectionCode, string aszRoiCode, string aszInspCode, GraphicsSkeletonBall[] aBallSegments)
        {
            if (File.Exists(aszFilePath))
            {
                using (StreamWriter sw = new StreamWriter(aszFilePath, true))
                {
                    sw.WriteLine("%C");
                    sw.WriteLine(string.Format("Section={0}", aszSectionCode));
                    sw.WriteLine(string.Format("ROI={0}", aszRoiCode));
                    sw.WriteLine(string.Format("Insp={0}", aszInspCode));

                    foreach (GraphicsSkeletonBall ballSegment in aBallSegments)
                    {
                        sw.WriteLine("%B");
                        sw.WriteLine(string.Format("R={0}", ballSegment.Radian));
                        sw.WriteLine(string.Format("P={0}", ballSegment.BoundaryRect.ToFile()));
                    }
                }
            }
        }

        public static GraphicsSkeletonLine[] GetCenterLineDatum(string aszFilePath, string aszSectionCode, string aszRoiCode, string aszInspCode)
        {
            if (!File.Exists(aszFilePath)) return null;
            try
            {
                List<GraphicsSkeletonLine> lineSegments = new List<GraphicsSkeletonLine>();
                string szLine = string.Empty;
                using (StreamReader sr = new StreamReader(aszFilePath))
                {
                    while (!string.IsNullOrEmpty(szLine = sr.ReadLine()))
                    {
                        if (szLine.StartsWith("%C"))
                        {
                            string szSectionCode = sr.ReadLine();
                            string szRoiCode = sr.ReadLine();
                            string szInspCode = sr.ReadLine();

                            // Insp Item 추적.
                            if (szSectionCode.IndexOf(aszSectionCode) > 0 &&
                                szRoiCode.IndexOf(aszRoiCode) > 0 &&
                                szInspCode.IndexOf(aszInspCode) > 0)
                            {
                                string szSubLine = sr.ReadLine();
                                while (!string.IsNullOrEmpty(szSubLine) && szSubLine.StartsWith("%L"))
                                {
                                    GraphicsSkeletonLine lineSegment = new GraphicsSkeletonLine();

                                    #region Parsing Line Segment.
                                    // Line Direction
                                    szSubLine = sr.ReadLine();
                                    if (szSubLine == null)
                                        break;

                                    lineSegment.LineDir = Convert.ToByte(szSubLine.Substring(2, szSubLine.Length - 2));

                                    // Boundary Rectangle
                                    szSubLine = sr.ReadLine();
                                    string szBoundaryRect = szSubLine.Substring(2, szSubLine.Length - 2);
                                    string[] szTokens = szBoundaryRect.Split('X', 'Y', 'W', 'H');
                                    if (szTokens.Length != 5)
                                        return null;
                                    lineSegment.BoundaryRect = new Int16Rect();
                                    lineSegment.BoundaryRect.X = Convert.ToInt16(szTokens[1]);
                                    lineSegment.BoundaryRect.Y = Convert.ToInt16(szTokens[2]);
                                    lineSegment.BoundaryRect.Width = Convert.ToInt16(szTokens[3]);
                                    lineSegment.BoundaryRect.Height = Convert.ToInt16(szTokens[4]);

                                    // Median Size
                                    szSubLine = sr.ReadLine();
                                    lineSegment.MedianSize = Convert.ToSingle(szSubLine.Substring(2, szSubLine.Length - 2));

                                    // Type
                                    szSubLine = sr.ReadLine();
                                    lineSegment.Type = Convert.ToByte(szSubLine.Substring(2, szSubLine.Length - 2));
                                    #endregion

                                    #region Parsing Node List.
                                    List<SkeletonNode> nodeList = new List<SkeletonNode>();
                                    while (!string.IsNullOrEmpty(szSubLine = sr.ReadLine()) && szSubLine.StartsWith("%N"))
                                    {
                                        SkeletonNode node = new SkeletonNode();
                                        szTokens = szSubLine.Split(' ');

                                        // Line Direction
                                        node.LineDirection = Convert.ToByte(szTokens[1].Substring(2, szTokens[1].Length - 2));

                                        // Position
                                        szSubLine = szTokens[2].Substring(2, szTokens[2].Length - 2);
                                        string[] szPoints = szSubLine.Split('X', 'Y');
                                        node.Position = new Int16Point();
                                        node.Position.X = Convert.ToInt16(szPoints[1]);
                                        node.Position.Y = Convert.ToInt16(szPoints[2]);

                                        // Measure Size & Type
                                        node.MeasureSize = Convert.ToSingle(szTokens[3].Substring(2, szTokens[3].Length - 2));
                                        node.Type = Convert.ToByte(szTokens[4].Substring(2, szTokens[4].Length - 2));

                                        nodeList.Add(node);
                                    }
                                    if (nodeList.Count > 0) 
                                        lineSegment.Nodes = nodeList.ToArray();
                                    #endregion

                                    if (lineSegment.Nodes != null && lineSegment.Nodes.Length > 0)
                                        lineSegments.Add(lineSegment);
                                }
                            }
                        }
                    }
                }

                if (lineSegments.Count == 0)
                    return null;
                else
                    return lineSegments.ToArray();
            }
            catch
            {
                return null; // Parse Error.
            }
        }

        public static GraphicsSkeletonBall[] GetBallDatum(string aszFilePath, string aszSectionCode, string aszRoiCode, string aszInspCode)
        {
            if (!File.Exists(aszFilePath)) return null;
            try
            {
                List<GraphicsSkeletonBall> ballSegments = new List<GraphicsSkeletonBall>();
                string szLine = string.Empty;
                using (StreamReader sr = new StreamReader(aszFilePath))
                {
                    while (!string.IsNullOrEmpty(szLine = sr.ReadLine()))
                    {
                        if (szLine.StartsWith("%C"))
                        {
                            string szSectionCode = sr.ReadLine();
                            string szRoiCode = sr.ReadLine();
                            string szInspCode = sr.ReadLine();

                            // Insp Item 추적.
                            if (szSectionCode.IndexOf(aszSectionCode) > 0 &&
                                szRoiCode.IndexOf(aszRoiCode) > 0 &&
                                szInspCode.IndexOf(aszInspCode) > 0)
                            {
                                string szSubLine = sr.ReadLine();
                                while (!string.IsNullOrEmpty(szSubLine) && szSubLine.StartsWith("%B"))
                                {
                                    GraphicsSkeletonBall ballSegment = new GraphicsSkeletonBall();

                                    #region Parsing Ball Segment.
                                    // Line Direction
                                    szSubLine = sr.ReadLine();
                                    if (szSubLine == null)
                                        break;

                                    ballSegment.Radian = Convert.ToSingle(szSubLine.Substring(2, szSubLine.Length - 2));

                                    // Boundary Rectangle
                                    szSubLine = sr.ReadLine();
                                    string szBoundaryRect = szSubLine.Substring(2, szSubLine.Length - 2);
                                    string[] szTokens = szBoundaryRect.Split('X', 'Y', 'W', 'H');
                                    if (szTokens.Length != 5)
                                        return null;
                                    ballSegment.BoundaryRect = new Int16Rect();
                                    ballSegment.BoundaryRect.X = Convert.ToInt16(szTokens[1]);
                                    ballSegment.BoundaryRect.Y = Convert.ToInt16(szTokens[2]);
                                    ballSegment.BoundaryRect.Width = Convert.ToInt16(szTokens[3]);
                                    ballSegment.BoundaryRect.Height = Convert.ToInt16(szTokens[4]);

                                    #endregion
                                    ballSegments.Add(ballSegment);
                                    szSubLine = sr.ReadLine();
                                }
                            }
                        }
                    }
                }

                if (ballSegments.Count == 0)
                    return null;
                else
                    return ballSegments.ToArray();
            }
            catch
            {
                return null; // Parse Error.
            }
        }
    }
}
