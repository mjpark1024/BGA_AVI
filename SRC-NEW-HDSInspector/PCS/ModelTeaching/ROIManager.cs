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
 * @file  ROIManager.cs
 * @brief 
 *  It manages ROIs.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.09.30
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.09.30 First creation.
 */

using System;
using System.Collections.Generic;
using Common.DataBase;
using Common.Drawing;
using System.Data;
using Common.Drawing.InspectionInformation;
using System.Diagnostics;
using Common;

namespace PCS.ModelTeaching
{
    /// <summary>   Manager for rois.  </summary>
    /// <remarks>   suoow2, 2014-09-30. </remarks>
    public class ROIManager
    {
        public static void LoadMarkStripAlignROI(string aszMachineCode, string aszGroupName, string aszModelCode, DrawingCanvas anRoiCanvas)
        {
             anRoiCanvas.GraphicsList.Clear();

            IDataReader dataReader = null;
            List<string> roiCodeList = new List<string>();
            List<GraphicsBase> roiList = new List<GraphicsBase>();
            GraphicsBase graphic = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    // 기록된 ROI를 Drawing Canvas에 그린다.
                    string strQuery = string.Format("SELECT roi_code, roi_rect FROM bgadb.mark_strip_align " +
                                                    "WHERE machine_code = '{0}' AND model_code = '{1}' " +
                                                    "ORDER BY roi_code ASC", aszMachineCode, aszModelCode);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            roiCodeList.Add(dataReader.GetValue(0).ToString());
                            graphic = ROIParser.CreateMarkStripAlignGraphicsToCanvas(dataReader.GetValue(1).ToString(), anRoiCanvas); // Drawing Canvas에 ROI를 그린다.
                            if (graphic != null)
                            {
                                roiList.Add(graphic); // ROI에 검사 Parameter를 부여하기 위해 roiList에 graphic을 추가시킨다.
                            }
                        }
                        dataReader.Close();
                    }
                }
            }
            catch
            {
                if (dataReader != null)
                {
                    dataReader.Close();
                }
            }
        }

        // ROI 읽기.
        public static void LoadStripAlignROI(string aszMachineCode, string aszGroupName, string aszModelCode, DrawingCanvas anRoiCanvas, Surface aSurfaceType)
        {
           // anRoiCanvas.GraphicsList.Clear();

            IDataReader dataReader = null;
            List<string> roiCodeList = new List<string>();
            List<GraphicsBase> roiList = new List<GraphicsBase>();
            GraphicsBase graphic = null;
            string szWorkType = WorkTypeCode.GetWorkTypeCode(aSurfaceType);
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    // 기록된 ROI를 Drawing Canvas에 그린다.
                    string strQuery = string.Format("SELECT roi_code, roi_rect FROM bgadb.strip_align " +
                                                    "WHERE machine_code = '{0}' AND model_code = '{1}' AND work_type = '{2}' " +
                                                    "ORDER BY roi_code ASC", aszMachineCode, aszModelCode, szWorkType);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            roiCodeList.Add(dataReader.GetValue(0).ToString());
                            graphic = ROIParser.CreateStripAlignGraphicsToCanvas(dataReader.GetValue(1).ToString(), anRoiCanvas); // Drawing Canvas에 ROI를 그린다.
                            if (graphic != null)
                            {
                                roiList.Add(graphic); // ROI에 검사 Parameter를 부여하기 위해 roiList에 graphic을 추가시킨다.
                            }
                        }
                        dataReader.Close();
                    }
                }
            }
            catch
            {
                if (dataReader != null)
                {
                    dataReader.Close();
                }
            }
        }
        public static void LoadIDMarkROI(string aszMachineCode, string aszGroupName, string aszModelCode, DrawingCanvas anRoiCanvas, Surface aSurfaceType)
        {
            // anRoiCanvas.GraphicsList.Clear();

            IDataReader dataReader = null;
            List<string> roiCodeList = new List<string>();
            List<GraphicsBase> roiList = new List<GraphicsBase>();
            GraphicsBase graphic = null;
            string szWorkType = WorkTypeCode.GetWorkTypeCode(aSurfaceType);
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    // 기록된 ROI를 Drawing Canvas에 그린다.
                    string strQuery = string.Format("SELECT roi_code, roi_rect FROM bgadb.id_mark " +
                                                    "WHERE machine_code = '{0}' AND model_code = '{1}' AND work_type = '{2}' " +
                                                    "ORDER BY roi_code ASC", aszMachineCode, aszModelCode, szWorkType);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            roiCodeList.Add(dataReader.GetValue(0).ToString());
                            graphic = ROIParser.CreateIDMarkGraphicsToCanvas(dataReader.GetValue(1).ToString(), anRoiCanvas); // Drawing Canvas에 ROI를 그린다.
                            if (graphic != null)
                            {
                                roiList.Add(graphic); // ROI에 검사 Parameter를 부여하기 위해 roiList에 graphic을 추가시킨다.
                            }
                        }
                        dataReader.Close();
                    }
                }
            }
            catch
            {
                if (dataReader != null)
                {
                    dataReader.Close();
                }
                //anRoiCanvas.GraphicsList.Clear();
            }
        }
        // ROI 읽기.
        public static void LoadROI(string aszMachineCode, string aszGroupName, string aszModelCode, string aszSectionCode, DrawingCanvas anRoiCanvas, Surface aSurfaceType, int anChannel, string aszModePath)
        {
            anRoiCanvas.GraphicsList.Clear();

            IDataReader dataReader = null;
            List<string> roiCodeList = new List<string>();
            List<GraphicsBase> roiList = new List<GraphicsBase>();
            GraphicsBase graphic = null;
            string szWorkType = WorkTypeCode.GetWorkTypeCode(aSurfaceType);
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    // 기록된 ROI를 Drawing Canvas에 그린다.
                    string strQuery = string.Format("SELECT roi_info.roi_code, roi_info.outer_roi_rect, roi_info.local_align_points FROM bgadb.roi_info roi_info " +
                                                    "WHERE roi_info.machine_code = '{0}' AND roi_info.model_code = '{1}' AND roi_info.work_type = '{2}' AND roi_info.section_code = '{3}' AND roi_info.channel = {4} " +
                                                    "ORDER BY roi_info.section_code ASC, roi_info.roi_code ASC", aszMachineCode, aszModelCode, szWorkType, aszSectionCode,anChannel);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            roiCodeList.Add(dataReader.GetValue(0).ToString());
                            graphic = ROIParser.CreateGraphicsToCanvas(dataReader.GetValue(1).ToString(), anRoiCanvas); // Drawing Canvas에 ROI를 그린다.
                            graphic.roiCode = Convert.ToInt32(dataReader.GetValue(0)).ToString("0000");
                            if (graphic != null)
                            {
                                ROIParser.CreateLocalAlignsToCanvas(dataReader.GetValue(2).ToString(), graphic, anRoiCanvas);
                                roiList.Add(graphic); // ROI에 검사 Parameter를 부여하기 위해 roiList에 graphic을 추가시킨다.
                            }
                        }
                        dataReader.Close();
                    }

                    // Load ROI Param.
                    int roiCodeLength = roiCodeList.Count;
                    for (int i = 0; i < roiCodeLength; i++)
                    {
                        if (!string.IsNullOrEmpty(roiCodeList[i]) && roiList[i] != null)
                        {
                            // 1. ROI에 포함된
                            // 를 리스트로 가져온다.
                            strQuery = string.Format("SELECT distinct roi_param.Inspect_id, roi_param.inspect_code FROM bgadb.roi_param roi_param " +
                                                     "WHERE roi_param.machine_code='{0}' " +
                                                     "AND roi_param.model_code='{1}' " +
                                                     "AND roi_param.work_type='{2}' " +
                                                     "AND roi_param.section_code='{3}' " +
                                                     "AND roi_param.roi_code='{4}'",
                                                     aszMachineCode, aszModelCode, szWorkType, aszSectionCode, roiCodeList[i]);

                            if (dataReader != null)
                            {
                                dataReader.Close();
                            }

                            dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                            if (dataReader != null)
                            {
                                List<string> strInspectIDList = new List<string>();
                                List<string> strInspectCodeList = new List<string>();
                                while (dataReader.Read())
                                {
                                    strInspectIDList.Add(dataReader.GetValue(0).ToString());
                                    strInspectCodeList.Add(dataReader.GetValue(1).ToString());
                                }
                                if (dataReader != null)
                                {
                                    dataReader.Close();
                                }

                                // 2. inspect_code와 매칭된 클래스 타입으로부터 instance를 가져와 roiGraphic의 inspectionlist에 추가시킨다.
                                for (int j = 0; j < strInspectIDList.Count; j++)
                                {
                                    InspectionItem inspectionItem = InspectionItemHelper.GetInspectionItem(aszMachineCode, aszModelCode, szWorkType, aszSectionCode, roiCodeList[i], strInspectIDList[j], strInspectCodeList[j]);
                                    if (inspectionItem != null)
                                    {
                                        if (inspectionItem.InspectionType.InspectType == eVisInspectType.eInspTypeLeadShapeWithCL ||
                                            inspectionItem.InspectionType.InspectType == eVisInspectType.eInspTypeSpaceShapeWithCL ||
                                            inspectionItem.InspectionType.InspectType == eVisInspectType.eInspTypePlateWithCL)
                                        {
                                            // Load CenterLine Datum.
                                            string szCenterLineDataPath = DirectoryManager.GetCenterLineDataPath(aszModePath, aszGroupName, aszModelCode, aSurfaceType);
                                            inspectionItem.LineSegments = CenterLineHelper.GetCenterLineDatum(szCenterLineDataPath, aszSectionCode, roiCodeList[i], strInspectIDList[j]);
                                        }

                                        if (inspectionItem.InspectionType.InspectType == eVisInspectType.eInspTypeBallPattern)
                                        {
                                            // Load CenterLine Datum.
                                            string szBallDataPath = DirectoryManager.GetBallDataPath(aszModePath, aszGroupName, aszModelCode, aSurfaceType);
                                            inspectionItem.BallSegments = CenterLineHelper.GetBallDatum(szBallDataPath, aszSectionCode, roiCodeList[i], strInspectIDList[j]);
                                        }
                                        if (inspectionItem.InspectionType.InspectType == eVisInspectType.eInspTypeUnitRawMaterial)
                                        {
                                            RawMetrialProperty raw = inspectionItem.InspectionAlgorithm as RawMetrialProperty;
                                            roiList[i].RawRow = raw.MinSmallDefectSize + 1;
                                        }
                                        roiList[i].InspectionList.Add(inspectionItem);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                if (dataReader != null)
                {
                    dataReader.Close();
                }
                anRoiCanvas.GraphicsList.Clear();
            }
        }

        public static void LoadStripGuideROI(string aszMachineCode, string aszGroupName, string aszModelCode, DrawingCanvas anRoiCanvas, Surface aSurfaceType)
        {
            // anRoiCanvas.GraphicsList.Clear();

            IDataReader dataReader = null;
            List<string> roiCodeList = new List<string>();
            List<GraphicsBase> roiList = new List<GraphicsBase>();
            GraphicsBase graphic = null;
            string szWorkType = WorkTypeCode.GetWorkTypeCode(aSurfaceType);
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    // 기록된 ROI를 Drawing Canvas에 그린다.
                    string strQuery = string.Format("SELECT roi_code, roi_rect FROM markerdb.strip_guide " +
                                                    "WHERE machine_code = '{0}' AND model_code = '{1}' AND work_type = '{2}' " +
                                                    "ORDER BY roi_code ASC", aszMachineCode, aszModelCode, szWorkType);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            roiCodeList.Add(dataReader.GetValue(0).ToString());
                            graphic = ROIParser.CreateGuideGraphicsToCanvas(dataReader.GetValue(1).ToString(), anRoiCanvas); // Drawing Canvas에 ROI를 그린다.
                            if (graphic != null)
                            {
                                roiList.Add(graphic); // ROI에 검사 Parameter를 부여하기 위해 roiList에 graphic을 추가시킨다.
                            }
                        }
                        dataReader.Close();
                    }
                }
            }
            catch
            {
                if (dataReader != null)
                {
                    dataReader.Close();
                }
                //anRoiCanvas.GraphicsList.Clear();
            }
        }

        public static bool SaveStripGuideROI(string aszMachineCode, string aszGroupName, string aszModelCode, DrawingCanvas anRoiCanvas)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();

                    string strQuery = string.Empty;
                    string strOuterRoiRect = string.Empty;
                    string strRoiCode = string.Empty;
                    int queryResult = 1;
                    strQuery = String.Format("set sql_safe_updates  = 0; Delete from markerdb.strip_guide where " +
                                                 "machine_code = '{0}' and " + // 0
                                                 "model_code = '{1}'; set sql_safe_updates  = 1;",
                                                 aszMachineCode, aszModelCode);

                    queryResult = ConnectFactory.DBConnector().Execute(strQuery);
                    if (queryResult >= 0)
                    {
                        ConnectFactory.DBConnector().Commit();
                    }
                    else
                    {
                        ConnectFactory.DBConnector().Rollback();
                        return false;
                    }

                    strOuterRoiRect = "";
                    foreach (GraphicsBase graphic in anRoiCanvas.GraphicsList)
                    {
                        if (graphic.RegionType == GraphicsRegionType.GuideLine)
                        {
                            strOuterRoiRect = ROIParser.GraphicsToString(graphic);
                            strRoiCode = QueryHelper.GetCode(RoiCode++, 4);
                            strQuery = String.Format("INSERT INTO markerdb.strip_guide(" +
                                                     "machine_code," + // 0
                                                     "model_code," + // 1
                                                     "roi_code," + // 3
                                                     "roi_rect," + // 4
                                                     "send_yn) " +
                                                     "VALUES ('{0}', '{1}', '{2}', '{3}', 0)",
                                                     aszMachineCode, aszModelCode, strRoiCode, strOuterRoiRect);

                            queryResult = ConnectFactory.DBConnector().Execute(strQuery);

                            if (queryResult < 1) // INSERT INTO markerdb.roi_info fail
                            {
                                break;
                            }
                        }
                        if (graphic.RegionType == GraphicsRegionType.MarkGuide)
                        {
                            strOuterRoiRect = ROIParser.GraphicsToString(graphic);
                            strRoiCode = QueryHelper.GetCode(RoiCode++, 4);
                            strQuery = String.Format("INSERT INTO markerdb.strip_guide(" +
                                                     "machine_code," + // 0
                                                     "model_code," + // 1
                                                     "roi_code," + // 3
                                                     "roi_rect," + // 4
                                                     "send_yn) " +
                                                     "VALUES ('{0}', '{1}', '{2}', '{3}', 0)",
                                                     aszMachineCode, aszModelCode, strRoiCode, strOuterRoiRect);

                            queryResult = ConnectFactory.DBConnector().Execute(strQuery);

                            if (queryResult < 1) // INSERT INTO markerdb.roi_info fail
                            {
                                break;
                            }
                        }
                        if (string.IsNullOrEmpty(strOuterRoiRect))
                        {
                            continue;
                        }


                    }

                    if (queryResult > 0)
                    {
                        ConnectFactory.DBConnector().Commit();
                        return true;
                    }
                    else
                    {
                        ConnectFactory.DBConnector().Rollback();
                        return false;
                    }
                }
                else
                {
                    return false; // DBConnector is null, 저장 실패
                }
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in SaveStripAlignROI(ROIManager.cs)");
                return false; // 저장 실패
            }
        }

        public static bool SaveMarkStripAlignROI(string aszMachineCode, string aszGroupName, string aszModelCode, DrawingCanvas anRoiCanvas)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();
                    string strQuery = string.Empty;
                    string strOuterRoiRect = string.Empty;
                    string strRoiCode = string.Empty;
                    int queryResult = 1;
                    strQuery = String.Format("set sql_safe_updates  = 0; Delete from bgadb.mark_strip_align where " +
                                                 "machine_code = '{0}' and " + // 0
                                                 "model_code = '{1}'; set sql_safe_updates  = 1;",
                                                 aszMachineCode, aszModelCode);

                    queryResult = ConnectFactory.DBConnector().Execute(strQuery);
                    if (queryResult >= 0)
                    {
                        ConnectFactory.DBConnector().Commit();
                    }
                    else
                    {
                        ConnectFactory.DBConnector().Rollback();
                        return false;
                    }
                    int cnt = 0;
                    string tmpstr = "";
                    for (int i = 0; i < anRoiCanvas.GraphicsList.Count; i++)
                    {
                        GraphicsBase graphic = (GraphicsBase)anRoiCanvas.GraphicsList[i];
                        if (graphic.RegionType == GraphicsRegionType.StripAlign)
                        {
                            if (cnt == 0) tmpstr = ROIParser.GraphicsToString(graphic);
                            else
                            {

                                strOuterRoiRect = ROIParser.GraphicsToString(graphic);
                                if (tmpstr.Equals(strOuterRoiRect))
                                {
                                    anRoiCanvas.GraphicsList.RemoveAt(i);
                                    i--;
                                }
                            }
                            cnt++;
                        }
                        if (string.IsNullOrEmpty(strOuterRoiRect))
                        {
                            continue;
                        }
                    }
                    tmpstr = "";
                    cnt = 0;
                    strOuterRoiRect = "";
                    for (int i = 0; i < anRoiCanvas.GraphicsList.Count; i++)
                    {
                        GraphicsBase graphic = (GraphicsBase)anRoiCanvas.GraphicsList[i];
                        if (graphic.RegionType == GraphicsRegionType.StripAlign)
                        {
                            if (cnt == 1) tmpstr = ROIParser.GraphicsToString(graphic);
                            else
                            {

                                strOuterRoiRect = ROIParser.GraphicsToString(graphic);
                                if (tmpstr.Equals(strOuterRoiRect))
                                {
                                    anRoiCanvas.GraphicsList.RemoveAt(i);
                                    i--;
                                }
                            }
                            cnt++;
                        }
                        if (string.IsNullOrEmpty(strOuterRoiRect))
                        {
                            continue;
                        }
                    }
                    strOuterRoiRect = "";
                    foreach (GraphicsBase graphic in anRoiCanvas.GraphicsList)
                    {
                        if (graphic.RegionType == GraphicsRegionType.StripAlign)
                        {
                            strOuterRoiRect = ROIParser.GraphicsStripAlignToString((GraphicsStripAlign)graphic);
                            strRoiCode = QueryHelper.GetCode(RoiCode++, 4);
                            strQuery = String.Format("INSERT INTO bgadb.mark_strip_align(" +
                                                     "machine_code," + // 0
                                                     "model_code," + // 1
                                                     "roi_code," + // 2
                                                     "roi_rect," + // 3
                                                     "send_yn) " +
                                                     "VALUES ('{0}', '{1}', '{2}', '{3}', 0)",
                                                     aszMachineCode, aszModelCode, strRoiCode, strOuterRoiRect);

                            queryResult = ConnectFactory.DBConnector().Execute(strQuery);

                            if (queryResult < 1) // INSERT INTO markerdb.roi_info fail
                            {
                                break;
                            }
                        }
                        if (string.IsNullOrEmpty(strOuterRoiRect))
                        {
                            continue;
                        }


                    }

                    if (queryResult > 0)
                    {
                        ConnectFactory.DBConnector().Commit();
                        return true;
                    }
                    else
                    {
                        ConnectFactory.DBConnector().Rollback();
                        return false;
                    }
                }
                else
                {
                    return false; // DBConnector is null, 저장 실패
                }
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in SaveMarkStripAlignROI(ROIManager.cs)");
                return false; // 저장 실패
            }
        }


        public static bool SaveStripAlignROI(string aszMachineCode, string aszGroupName, string aszModelCode, string aszSectionCode, DrawingCanvas anRoiCanvas, Surface aSurfaceType)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();

                    string strWorkType = WorkTypeCode.GetWorkTypeCode(aSurfaceType);
                    string strQuery = string.Empty;
                    string strOuterRoiRect = string.Empty;
                    string strRoiCode = string.Empty;
                    int queryResult = 1;
                    strQuery = String.Format("set sql_safe_updates  = 0; Delete from bgadb.strip_align where " +
                                                 "machine_code = '{0}' and " + // 0
                                                 "model_code = '{1}' and " + // 1
                                                 "work_type = '{2}'; set sql_safe_updates  = 1;",
                                                 aszMachineCode, aszModelCode, strWorkType);

                    queryResult = ConnectFactory.DBConnector().Execute(strQuery);
                    if (queryResult >= 0)
                    {
                        ConnectFactory.DBConnector().Commit();
                    }
                    else
                    {
                        ConnectFactory.DBConnector().Rollback();
                        return false;
                    }
                    int cnt = 0;
                    string tmpstr = "";
                    for (int i = 0; i < anRoiCanvas.GraphicsList.Count; i++)
                    {
                        GraphicsBase graphic = (GraphicsBase)anRoiCanvas.GraphicsList[i];
                        if (graphic.RegionType == GraphicsRegionType.StripOrigin)
                        {
                            if (cnt == 0) tmpstr = ROIParser.GraphicsToString(graphic);
                            else
                            {

                                strOuterRoiRect = ROIParser.GraphicsToString(graphic);
                                if (tmpstr.Equals(strOuterRoiRect))
                                {
                                    anRoiCanvas.GraphicsList.RemoveAt(i);
                                    i--;
                                }
                            }
                            cnt++;
                        }
                        if (string.IsNullOrEmpty(strOuterRoiRect))
                        {
                            continue;
                        }
                    }
                    tmpstr = "";
                    cnt = 0;
                    strOuterRoiRect = "";
                    for (int i = 0; i < anRoiCanvas.GraphicsList.Count; i++)
                    {
                        GraphicsBase graphic = (GraphicsBase)anRoiCanvas.GraphicsList[i];
                        if (graphic.RegionType == GraphicsRegionType.StripOrigin)
                        {
                            if (cnt == 1) tmpstr = ROIParser.GraphicsToString(graphic);
                            else
                            {

                                strOuterRoiRect = ROIParser.GraphicsToString(graphic);
                                if (tmpstr.Equals(strOuterRoiRect))
                                {
                                    anRoiCanvas.GraphicsList.RemoveAt(i);
                                    i--;
                                }
                            }
                            cnt++;
                        }
                        if (string.IsNullOrEmpty(strOuterRoiRect))
                        {
                            continue;
                        }
                    }
                    strOuterRoiRect = "";
                    foreach (GraphicsBase graphic in anRoiCanvas.GraphicsList)
                    {
                        if (graphic.RegionType == GraphicsRegionType.StripOrigin)
                        {
                            strOuterRoiRect = ROIParser.GraphicsToString(graphic);
                            strRoiCode = QueryHelper.GetCode(RoiCode++, 4);
                            strQuery = String.Format("INSERT INTO bgadb.strip_align(" +
                                                     "machine_code," + // 0
                                                     "model_code," + // 1
                                                     "work_type," + // 2
                                                     "roi_code," + // 3
                                                     "roi_rect," + // 4
                                                     "send_yn) " +
                                                     "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', 0)",
                                                     aszMachineCode, aszModelCode, strWorkType, strRoiCode, strOuterRoiRect);

                            queryResult = ConnectFactory.DBConnector().Execute(strQuery);

                            if (queryResult < 1) // INSERT INTO bgadb.roi_info fail
                            {
                                break;
                            }
                        }
                        if (string.IsNullOrEmpty(strOuterRoiRect))
                        {
                            continue;
                        }

                        
                    }

                    if (queryResult > 0)
                    {
                        ConnectFactory.DBConnector().Commit();
                        return true;
                    }
                    else
                    {
                        ConnectFactory.DBConnector().Rollback();
                        return false;
                    }
                }
                else
                {
                    return false; // DBConnector is null, 저장 실패
                }
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in SaveStripAlignROI(ROIManager.cs)");
                return false; // 저장 실패
            }
        }

        public static bool SaveIDMarkROI(string aszMachineCode, string aszGroupName, string aszModelCode, string aszSectionCode, DrawingCanvas anRoiCanvas, Surface aSurfaceType)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();

                    string strWorkType = WorkTypeCode.GetWorkTypeCode(aSurfaceType);
                    string strQuery = string.Empty;
                    string strOuterRoiRect = string.Empty;
                    string strRoiCode = string.Empty;
                    int queryResult = 1;
                    strQuery = String.Format("set sql_safe_updates  = 0; Delete from bgadb.id_mark where " +
                                                 "machine_code = '{0}' and " + // 0
                                                 "model_code = '{1}' and " + // 1
                                                 "work_type = '{2}'; set sql_safe_updates  = 1;",
                                                 aszMachineCode, aszModelCode, strWorkType);

                    queryResult = ConnectFactory.DBConnector().Execute(strQuery);
                    if (queryResult >= 0)
                    {
                        ConnectFactory.DBConnector().Commit();
                    }
                    else
                    {
                        ConnectFactory.DBConnector().Rollback();
                        return false;
                    }
                    int cnt = 0;
                    string tmpstr = "";
                    for (int i = 0; i < anRoiCanvas.GraphicsList.Count; i++)
                    {
                        GraphicsBase graphic = (GraphicsBase)anRoiCanvas.GraphicsList[i];
                        if (graphic.RegionType == GraphicsRegionType.IDMark)
                        {
                            if (cnt == 0) tmpstr = ROIParser.GraphicsToString(graphic);
                            else
                            {

                                strOuterRoiRect = ROIParser.GraphicsToString(graphic);
                                if (tmpstr.Equals(strOuterRoiRect))
                                {
                                    anRoiCanvas.GraphicsList.RemoveAt(i);
                                    i--;
                                }
                            }
                            cnt++;
                        }
                        if (string.IsNullOrEmpty(strOuterRoiRect))
                        {
                            continue;
                        }
                    }
                    if (cnt == 0)
                    {
                        return true;
                    }
                    foreach (GraphicsBase graphic in anRoiCanvas.GraphicsList)
                    {
                        if (graphic.RegionType == GraphicsRegionType.IDMark)
                        {
                            strOuterRoiRect = ROIParser.GraphicsToString(graphic);
                            strRoiCode = QueryHelper.GetCode(RoiCode++, 4);
                            strQuery = String.Format("INSERT INTO bgadb.id_mark(" +
                                                     "machine_code," + // 0
                                                     "model_code," + // 1
                                                     "work_type," + // 2
                                                     "roi_code," + // 3
                                                     "roi_rect," + // 4
                                                     "send_yn) " +
                                                     "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', 0)",
                                                     aszMachineCode, aszModelCode, strWorkType, strRoiCode, strOuterRoiRect);

                            queryResult = ConnectFactory.DBConnector().Execute(strQuery);

                            if (queryResult < 1) // INSERT INTO bgadb.roi_info fail
                            {
                                break;
                            }
                        }
                        if (string.IsNullOrEmpty(strOuterRoiRect))
                        {
                            continue;
                        }


                    }

                    if (queryResult > 0)
                    {
                        ConnectFactory.DBConnector().Commit();
                        return true;
                    }
                    else
                    {
                        ConnectFactory.DBConnector().Rollback();
                        return false;
                    }
                }
                else
                {
                    return false; // DBConnector is null, 저장 실패
                }
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in SaveIDMarkROI(ROIManager.cs)");
                return false; // 저장 실패
            }
        }

        // ROI 쓰기.
        public static int RoiCode = 0;
        public static bool SaveROI(string aszMachineCode, string aszGroupName, string aszModelCode, string aszSectionCode, DrawingCanvas anRoiCanvas, Surface aSurfaceType,int anChannel, string aszModePath)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();

                    string strWorkType = WorkTypeCode.GetWorkTypeCode(aSurfaceType);
                    string strQuery = string.Empty;
                    string strOuterRoiRect = string.Empty;
                    string strLocalAlignPoints = string.Empty;
                    string strRoiCode = string.Empty;
                    int queryResult = 1;
                    foreach (GraphicsBase graphic in anRoiCanvas.GraphicsList)
                    {
                        if (graphic.RegionType == GraphicsRegionType.LocalAlign)
                            continue; // LocalAlign ROI는 개별적으로 저장하지 않고 부모 ROI와 연관시켜 저장하도록 한다.
                        if (graphic is GraphicsSelectionRectangle || graphic is GraphicsLine || graphic is GraphicsSkeletonLine)
                            continue; // 실제 ROI가 아닌 타입은 저장하지 않도록 한다.

                        strOuterRoiRect = ROIParser.GraphicsToString(graphic);
                        strLocalAlignPoints = ROIParser.LocalAlignsToString(graphic);
                        if (string.IsNullOrEmpty(strOuterRoiRect))
                        {
                            continue;
                        }

                        strRoiCode = QueryHelper.GetCode(RoiCode++, 4);
                        strQuery = String.Format("INSERT INTO bgadb.roi_info(" +
                                                 "machine_code," + // 0
                                                 "model_code," + // 1
                                                 "work_type," + // 2
                                                 "section_code," + // 3
                                                 "channel," +
                                                 "roi_code," + // 4
                                                 "outer_roi_rect," + // 5
                                                 "local_align_points," + // 6
                                                 "send_yn) " +
                                                 "VALUES ('{0}', '{1}', '{2}', '{3}',{4}, '{5}', '{6}', '{7}', 0)",
                                                 aszMachineCode, aszModelCode, strWorkType, aszSectionCode, anChannel, strRoiCode, strOuterRoiRect, strLocalAlignPoints);

                        queryResult = ConnectFactory.DBConnector().Execute(strQuery);
                        //ConnectFactory.DBConnector().Commit();
                        if (queryResult < 1) // INSERT INTO bgadb.roi_info fail
                        {
                            break;
                        }

                        // Save ROI Param
                        foreach (InspectionItem inspectionItem in graphic.InspectionList)
                        {
                            string strInspCode = QueryHelper.GetCode(inspectionItem.ID, 4);

                            if (inspectionItem.InspectionType.InspectType == eVisInspectType.eInspTypeLeadShapeWithCL ||
                                inspectionItem.InspectionType.InspectType == eVisInspectType.eInspTypeSpaceShapeWithCL ||
                                inspectionItem.InspectionType.InspectType == eVisInspectType.eInspTypePlateWithCL)
                            {
                                if (inspectionItem.LineSegments != null)
                                {
                                    // Save CenterLine Datum.
                                    string szCenterLineDataPath = DirectoryManager.GetCenterLineDataPath(aszModePath, aszGroupName, aszModelCode, aSurfaceType);
                                    CenterLineHelper.AppendCenterLineDatum(szCenterLineDataPath, aszSectionCode, strRoiCode, strInspCode, inspectionItem.LineSegments);
                                }
                            }

                            if (inspectionItem.InspectionType.InspectType == eVisInspectType.eInspTypeBallPattern )
                            {
                                if (inspectionItem.BallSegments != null)
                                {
                                    // Save CenterLine Datum.
                                    string szBallDataPath = DirectoryManager.GetBallDataPath(aszModePath, aszGroupName, aszModelCode, aSurfaceType);
                                    CenterLineHelper.AppendBallDatum(szBallDataPath, aszSectionCode, strRoiCode, strInspCode, inspectionItem.BallSegments);
                                }
                            }

                            queryResult = inspectionItem.SaveInspectItem(aszMachineCode, aszModelCode, strWorkType, aszSectionCode, strRoiCode, strInspCode);
                            if (queryResult < 1) // INSERT ROI parameter fail
                            {
                                break;
                            }
                        }
                        if (queryResult < 1) // Save ROI Param foreach문에서 queryResult < 1인 경우에 대한 올바른 처리.
                        {
                            break;
                        }
                    }

                    if (queryResult > 0)
                    {
                        ConnectFactory.DBConnector().Commit();
                        return true;
                    }
                    else
                    {
                        ConnectFactory.DBConnector().Rollback();
                        return false;
                    }
                }
                else
                {
                    return false; // DBConnector is null, 저장 실패
                }
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in SaveROI(ROIManager.cs)");
                return false; // 저장 실패
            }
        }
    }
}
