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

using Common;
using Common.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace PCS.ModelTeaching
{
    internal class ROIInfo
    {
        public string Code { get; set; }
        public string OuterPoints { get; set; }
        public string InnerPoints { get; set; }
        public string LocalAlignPoints { get; set; }

        public List<ROIParam> InspectionItems = new List<ROIParam>();
    }

    internal class ROIInfo2
    {
        public string ModelCode { get; set; }
        public string WorkType { get; set; }
        public string SectionCode { get; set; }

        public string Code { get; set; }
        public string OuterPoints { get; set; }
        public string InnerPoints { get; set; }
        public string LocalAlignPoints { get; set; }

        public List<ROIParam> InspectionItems = new List<ROIParam>();
    }

    internal class ROIParam
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string ParamGroup { get; set; }
        public string ParamCode { get; set; }
        public int ParamValue { get; set; }
    }

    public class TeachingDataManager
    {
        public List<SectionInformation> Sections
        {
            get;
            private set;
        }

        public TeachingDataManager()
        {
            Sections = new List<SectionInformation>();
        }

        public static string GetTeachingAuthor(string aszMachineCode, string aszModelName)
        {
            string szAuthor = "Unknown User";

            if (ConnectFactory.DBConnector() != null)
            {
                string szQuery = string.Format("SELECT DISTINCT(A.user_id) FROM bgadb.roi_param A WHERE machine_code='{0}' AND model_code='{1}'", aszMachineCode, aszModelName);
                int nQueryResult = ConnectFactory.DBConnector().Execute(szQuery);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(szQuery);
                if (dataReader != null && dataReader.Read())
                {
                    szAuthor = dataReader.GetValue(0).ToString();
                }

                if (dataReader != null)
                    dataReader.Close();
            }
            return szAuthor;
        }

        public static bool DeleteTeachingData(string aszMachineCode, string aszModelName)
        {
            if (SectionManager.CheckBackupNeeded(aszMachineCode, aszModelName))
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();
                    try
                    {
                        // QUERY : 해당장비, 해당모델의 ROI 설정 값을 전부 삭제한다.
                        string szQuery = String.Format("DELETE FROM roi_param WHERE roi_param.machine_code = '{0}' AND roi_param.model_code = '{1}'", aszMachineCode, aszModelName);
                        int nQueryResult = ConnectFactory.DBConnector().Execute(szQuery);
                        if (nQueryResult >= 0)
                        {
                            // QUERY : 해당장비, 해당모델의 ROI를 전부 삭제한다.
                            szQuery = String.Format("DELETE FROM roi_info WHERE roi_info.machine_code = '{0}' AND roi_info.model_code = '{1}'", aszMachineCode, aszModelName);
                            nQueryResult = ConnectFactory.DBConnector().Execute(szQuery);

                            if (nQueryResult >= 0)
                            {
                                // QUERY : 해당장비, 해당모델의 섹션을 전부 삭제한다.
                                szQuery = String.Format("DELETE FROM section_info WHERE section_info.machine_code = '{0}' AND section_info.model_code = '{1}'", aszMachineCode, aszModelName);
                                nQueryResult = ConnectFactory.DBConnector().Execute(szQuery);
                            }
                        }
                        if (nQueryResult >= 0) // Query Success.
                        {
                            ConnectFactory.DBConnector().Commit();
                        }
                        else // Query Fail.
                        {
                            ConnectFactory.DBConnector().Rollback();
                            return false;
                        }
                    }
                    catch
                    {
                        ConnectFactory.DBConnector().Rollback();
                        Debug.WriteLine("Exception occured in TeachingViewerCtrl_SaveTeachDataEvent(Deletion process)");
                        return false;
                    }
                }
            }
            return true;
        }

        // DB의 Section 데이터를 복제한다 : A모델 -> B모델
        public void CopySectionDataToModel(string aszMachineCode, string aszOriginModelCode, string aszNewModelCode, string szWorkTypeCode)
        {
            bool bTransactionStarted = false;
            int nQueryResult = 0;
            IDataReader dataReader = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    // WorkType에 해당되는 Section을 조회한다.
                    string szQuery = string.Format("SELECT A.section_code, A.section_name, A.section_type, A.section_rect, A.region_data, A.rgb_color, " +
                                             "A.iterstart_x, A.iterstart_y, A.itercount_x, A.itercount_y, A.iterjump_x, A.iterjump_y, A.iterpitch_x, A.iterpitch_y, " +
                                             "A.blockcount_x, A.blockcount_y, A.blockpitch_x, A.blockpitch_y, A.blockcount, A.blockpitch, A.region_data2 " +
                                             "FROM bgadb.section_info A " +
                                             "WHERE A.machine_code = '{0}' AND A.model_code = '{1}' AND A.work_type = '{2}'", aszMachineCode, aszOriginModelCode, szWorkTypeCode);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(szQuery);
                    if (dataReader != null)
                    {
                        #region Read Section Data.
                        while (dataReader.Read())
                        {
                            SectionInformation section = new SectionInformation
                            {
                                Code = dataReader.GetValue(0).ToString(),
                                Name = dataReader.GetValue(1).ToString(),
                                Type = SectionManager.GetSectionType(dataReader.GetValue(2).ToString()),
                                Region = SectionManager.ParseRegionToRect(dataReader.GetValue(3).ToString().Split('X', 'Y', 'W', 'H')),
                                RGBColor = dataReader.GetValue(5).ToString(),
                                IterationStartX = Convert.ToInt32(dataReader.GetValue(6).ToString()),
                                IterationStartY = Convert.ToInt32(dataReader.GetValue(7).ToString()),
                                IterationCountX = Convert.ToInt32(dataReader.GetValue(8).ToString()),
                                IterationCountY = Convert.ToInt32(dataReader.GetValue(9).ToString()),
                                IterationJumpX = Convert.ToInt32(dataReader.GetValue(10).ToString()),
                                IterationJumpY = Convert.ToInt32(dataReader.GetValue(11).ToString()),
                                IterationPitchX = Convert.ToDouble(dataReader.GetValue(12).ToString()),
                                IterationPitchY = Convert.ToDouble(dataReader.GetValue(13).ToString()),
                                BlockCountX = Convert.ToInt32(dataReader.GetValue(14).ToString()),
                                BlockCountY = Convert.ToInt32(dataReader.GetValue(15).ToString()),
                                BlockPitchX = Convert.ToDouble(dataReader.GetValue(16).ToString()),
                                BlockPitchY = Convert.ToDouble(dataReader.GetValue(17).ToString()),
                                BlockCount = Convert.ToInt32(dataReader.GetValue(18).ToString()),
                                BlockPitch = Convert.ToDouble(dataReader.GetValue(19).ToString()),
                            };
                            Debug.WriteLine("### Read Section. WorkType:{0}, ID:{1}, Name:{2}", szWorkTypeCode, section.Code, section.Name);

                            #region DB에 기록된 문자열로부터 Region Data를 추출한다.
                            string szRegionData = dataReader.GetValue(4).ToString() + dataReader.GetValue(20).ToString();
                            if (!string.IsNullOrEmpty(szRegionData))
                            {
                                if (szRegionData.Contains('E'))
                                {
                                    foreach (string szRegionDatum in szRegionData.Split('X'))
                                    {
                                        if (!string.IsNullOrEmpty(szRegionDatum))
                                        {
                                            string[] szRegionTokens = szRegionDatum.Split('Y', 'I', 'J','E');
                                            if (szRegionTokens.Length == 5)
                                            {
                                                int nUnitX = Convert.ToInt32(szRegionTokens[0]);
                                                int nUnitY = Convert.ToInt32(szRegionTokens[1]);
                                                int nUnitXPosition = Convert.ToInt32(szRegionTokens[2]);
                                                int nUnitYPosition = Convert.ToInt32(szRegionTokens[3]);
                                                bool Is_Inspection = Convert.ToInt32(szRegionTokens[4]) == 1 ? true: false;

                                                if (section.Region.X == nUnitXPosition && section.Region.Y == nUnitYPosition)
                                                {
                                                    section.IterationXPosition = nUnitX;
                                                    section.IterationYPosition = nUnitY;
                                                    section.SectionRegionList.Add(new SectionRegion(nUnitX, nUnitY, nUnitXPosition, nUnitYPosition, Is_Inspection, true)); // 기준 영역
                                                }
                                                else
                                                {
                                                    section.SectionRegionList.Add(new SectionRegion(nUnitX, nUnitY, nUnitXPosition, nUnitYPosition, Is_Inspection)); // 일반 영역
                                                }
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    foreach (string szRegionDatum in szRegionData.Split('X'))
                                    {
                                        if (!string.IsNullOrEmpty(szRegionDatum))
                                        {
                                            string[] szRegionTokens = szRegionDatum.Split('Y', 'I', 'J');
                                            if (szRegionTokens.Length == 4)
                                            {
                                                int nUnitX = Convert.ToInt32(szRegionTokens[0]);
                                                int nUnitY = Convert.ToInt32(szRegionTokens[1]);
                                                int nUnitXPosition = Convert.ToInt32(szRegionTokens[2]);
                                                int nUnitYPosition = Convert.ToInt32(szRegionTokens[3]);
                                                if (section.Region.X == nUnitXPosition && section.Region.Y == nUnitYPosition)
                                                {
                                                    section.IterationXPosition = nUnitX;
                                                    section.IterationYPosition = nUnitY;
                                                    section.SectionRegionList.Add(new SectionRegion(nUnitX, nUnitY, nUnitXPosition, nUnitYPosition, true, true)); // 기준 영역
                                                }
                                                else
                                                {
                                                    section.SectionRegionList.Add(new SectionRegion(nUnitX, nUnitY, nUnitXPosition, nUnitYPosition, true)); // 일반 영역
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                            #endregion
                            section.SectionRegionList = section.SectionRegionList.OrderBy(x => x.ToString().Length).ThenBy(x => x.ToString()).ToList();
                            section.HashID = section.GetHashCode();

                            Sections.Add(section);
                        }
                        dataReader.Close();
                        #endregion
                    }

                    // 읽혀진 Section 데이터를 ModelCode만 변경하여 DB에 다시 기록한다.
                    if (Sections.Count > 0)
                    {
                        ConnectFactory.DBConnector().StartTrans();
                        bTransactionStarted = true;

                        StringBuilder sbRegionData = new StringBuilder();
                        foreach (SectionInformation section in Sections)
                        {
                            sbRegionData.Clear();
                            foreach (SectionRegion sectionRegion in section.SectionRegionList)
                            {
                                sbRegionData.Append(sectionRegion.ToString());
                            }
                            string str1, str2;
                            if (sbRegionData.Length > 50000)
                            {
                                char[] c1 = new char[50000];
                                sbRegionData.CopyTo(0, c1, 0, 50000);
                                str1 = new string(c1);
                                char[] c2 = new char[sbRegionData.Length - 50000];
                                sbRegionData.CopyTo(50000, c2, 0, sbRegionData.Length - 50000);
                                str2 = new string(c2);
                            }
                            else
                            {
                                char[] c1 = new char[sbRegionData.Length];
                                sbRegionData.CopyTo(0, c1, 0, sbRegionData.Length);
                                str1 = new string(c1);
                                str2 = "";
                            }
                            #region Write Section Data.
                            szQuery = String.Format("INSERT INTO bgadb.section_info(" +
                                    "machine_code," +                   // 0
                                    "model_code," +                     // 1
                                    "section_code," +                   // 2
                                    "work_type," +                      // 3
                                    "section_name," +                   // 4
                                    "section_type," +                   // 5
                                    "section_rect," +                   // 6
                                    "region_data," +                    // 7
                                    "rgb_color," +                      // 8
                                    "iterstart_x," +                    // 9
                                    "iterstart_y," +                    // 10
                                    "itercount_x," +                    // 11
                                    "itercount_y," +                    // 12
                                    "iterjump_x," +                     // 13
                                    "iterjump_y," +                     // 14
                                    "iterpitch_x," +                    // 15
                                    "iterpitch_y," +                    // 16
                                    "blockcount_x," +                   // 17
                                    "blockcount_y," +                   // 18
                                    "blockpitch_x," +                   // 19
                                    "blockpitch_y," +                   // 20
                                    "blockcount," +                   // 20
                                    "blockpitch," +                   // 20
                                    "send_yn," +
                                    "reg_date," +
                                    "region_data2)" +
                                    "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, 0, now(), '{23}')",
                                    aszMachineCode,                     // 0
                                    aszNewModelCode,                    // 1
                                    section.Code,                       // 2
                                    szWorkTypeCode,                     // 3
                                    section.Name,                       // 4
                                    section.Type.Code,                  // 5
                                    SectionManager.ParseRectToRegion(section.Region),  // 6
                                    //sbRegionData.ToString(),            // 7
                                    str1,
                                    section.RGBColor,                   // 8
                                    section.IterationStartX,            // 9
                                    section.IterationStartY,            // 10
                                    section.IterationCountX,            // 11
                                    section.IterationCountY,            // 12
                                    section.IterationJumpX,             // 13
                                    section.IterationJumpY,             // 14
                                    section.IterationPitchX,            // 15
                                    section.IterationPitchY,            // 16
                                    section.BlockCountX,                // 17
                                    section.BlockCountY,                // 18
                                    section.BlockPitchX,                // 19
                                    section.BlockPitchY,
                                    section.BlockCount,
                                    section.BlockPitch,
                                    str2);               // 20

                            nQueryResult = ConnectFactory.DBConnector().Execute(szQuery);
                            if (nQueryResult < 1) // INSERT INTO bgadb.section_info is fail
                            {
                                break;
                            }
                            #endregion
                        }

                        if (nQueryResult > 0) ConnectFactory.DBConnector().Commit();
                        else ConnectFactory.DBConnector().Rollback();
                    }
                }
            }
            catch
            {
                if (dataReader != null) dataReader.Close(); // data reader가 닫히지 않았을 때 닫아준다.
                if (bTransactionStarted) ConnectFactory.DBConnector().Rollback(); // transaction 시작 후 Exception이 발생하면 롤백 작업을 수행한다.

                Debug.WriteLine("Exception occured in CloneDBSectionData(TeachingDataManager.cs");
            }
        }

        // DB의 ROI, Param 데이터를 복제한다 : A모델 -> B모델
        public void CopyROIDataToModel(string aszMachineCode, string aszOriginModelCode, string aszNewModelCode, string aszWorkType, string aszSectionCode)
        {
            bool bTransactionStarted = false;
            List<ROIInfo> roiList = new List<ROIInfo>();

            IDataReader dataReader = null;
            try
            {
                #region Load ROI Data.
                string szQuery = string.Format("SELECT A.roi_code, A.outer_roi_rect, A.local_align_points FROM bgadb.roi_info A " +
                                                "WHERE A.machine_code = '{0}' AND A.model_code = '{1}' AND A.work_type = '{2}' AND A.section_code = '{3}' " +
                                                "ORDER BY A.section_code ASC, A.roi_code ASC", aszMachineCode, aszOriginModelCode, aszWorkType, aszSectionCode);

                dataReader = ConnectFactory.DBConnector().ExecuteQuery19(szQuery);
                if (dataReader != null)
                {
                    #region Read ROI Data.
                    while (dataReader.Read())
                    {
                        ROIInfo roi = new ROIInfo
                        {
                            Code = dataReader.GetValue(0).ToString(),
                            OuterPoints = dataReader.GetValue(1).ToString(),
                            InnerPoints = null,
                            LocalAlignPoints = dataReader.GetValue(2).ToString()
                        };
                        roiList.Add(roi);
                    }
                    dataReader.Close();
                    #endregion
                }
                #endregion

                #region Load ROI Param Data.
                int nROICount = roiList.Count;
                for (int i = 0; i < nROICount; i++)
                {
                    if (!string.IsNullOrEmpty(roiList[i].Code))
                    {
                        // 1. ROI에 포함된 inspect_code를 리스트로 가져온다.
                        szQuery = string.Format("SELECT A.Inspect_id, A.inspect_code, A.param_group, A.param_code, A.param_value " +
                                                 "FROM bgadb.roi_param A " +
                                                 "WHERE A.machine_code='{0}' " +
                                                 "AND A.model_code='{1}' " +
                                                 "AND A.work_type='{2}' " +
                                                 "AND A.section_code='{3}' " +
                                                 "AND A.roi_code='{4}'",
                                                 aszMachineCode, aszOriginModelCode, aszWorkType, aszSectionCode, roiList[i].Code);

                        dataReader = ConnectFactory.DBConnector().ExecuteQuery19(szQuery);
                        if (dataReader != null)
                        {
                            #region Read ROI Param Data.
                            while (dataReader.Read())
                            {
                                ROIParam roiParam = new ROIParam
                                {
                                    ID = dataReader.GetValue(0).ToString(),
                                    Code = dataReader.GetValue(1).ToString(),
                                    ParamGroup = dataReader.GetValue(2).ToString(),
                                    ParamCode = dataReader.GetValue(3).ToString(),
                                    ParamValue = Convert.ToInt32(dataReader.GetValue(4))
                                };

                                roiList[i].InspectionItems.Add(roiParam);
                            }
                            dataReader.Close();
                            #endregion
                        }
                    }
                }
                #endregion

                #region Save ROI Data.
                ConnectFactory.DBConnector().StartTrans();
                bTransactionStarted = true;

                int nQueryResult = 0;
                foreach (ROIInfo roi in roiList)
                {
                    szQuery = string.Format("INSERT INTO bgadb.roi_info(" +
                                             "machine_code," + // 0
                                             "model_code," + // 1
                                             "work_type," + // 2
                                             "section_code," + // 3
                                             "roi_code," + // 4
                                             "outer_roi_rect," + // 5
                        //"inner_roi_rect," +
                                             "local_align_points," + // 6
                                             "send_yn) " +
                                             "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', 0)",
                                             aszMachineCode, aszNewModelCode, aszWorkType, aszSectionCode, roi.Code, roi.OuterPoints, roi.LocalAlignPoints);

                    nQueryResult = ConnectFactory.DBConnector().Execute(szQuery);
                    if (nQueryResult < 1) break; // INSERT INTO bgadb.roi_info fail

                    foreach (ROIParam roiParam in roi.InspectionItems)
                    {
                        #region Save ROI Param Data.
                        szQuery = string.Format("INSERT INTO bgadb.roi_param(" +
                                                "machine_code, " + // 0
                                                "model_code, " + // 1
                                                "work_type, " + // 2
                                                "section_code, " + // 3
                                                "roi_code, " + // 4
                                                "inspect_id, " + // 5
                                                "inspect_code, " + // 6
                                                "param_group, " + // 7
                                                "param_code, " + // 8
                                                "param_value, " + // 9 : Integer.
                                                "reg_date, user_id, send_yn) " +
                                                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, now(), 'system', 0)",
                                                aszMachineCode, aszNewModelCode, aszWorkType, aszSectionCode,
                                                roi.Code, roiParam.ID, roiParam.Code, roiParam.ParamGroup, roiParam.ParamCode, roiParam.ParamValue);

                        nQueryResult = ConnectFactory.DBConnector().Execute(szQuery);
                        if (nQueryResult < 1) break; // INSERT INTO bgadb.roi_param fail
                        #endregion
                    }

                    // 검사 설정값 저장 실패시 모든 작업을 취소하도록 함.
                    if (nQueryResult < 1) break;
                }

                if (nQueryResult > 0) { ConnectFactory.DBConnector().Commit(); }
                else { ConnectFactory.DBConnector().Rollback(); }
                #endregion
            }
            catch
            {
                if (dataReader != null) dataReader.Close(); // data reader가 닫히지 않았을 때 닫아준다.
                if (bTransactionStarted) ConnectFactory.DBConnector().Rollback(); // transaction 시작 후 Exception이 발생하면 롤백 작업을 수행한다.

                Debug.WriteLine("Exception occured in CloneDBRoiData(TeachingDataManager.cs");
            }
        }

        // DB의 Section 데이터를 복제한다 : A장비 -> B장비
        public void CopySectionDataToMachine(string aszOldMachineCode, string aszNewMachineCode)
        {
            bool bTransactionStarted = false;
            int nQueryResult = 0;
            IDataReader dataReader = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    // WorkType에 해당되는 Section을 조회한다.
                    string szQuery = string.Format("SELECT A.machine_code, A.model_code, A.work_type, A.section_code, A.section_name, " + 
                                                   "A.section_type, A.section_rect, A.region_data, A.rgb_color, " +
                                                   "A.iterstart_x, A.iterstart_y, A.itercount_x, A.itercount_y, A.iterjump_x, A.iterjump_y, " +
                                                   "A.iterpitch_x, A.iterpitch_y, A.blockcount_x, A.blockcount_y, A.blockpitch_x, A.blockpitch_y, A.blockcount, A.blockpitch, A.region_data2 " +
                                                   "FROM bgadb.section_info A WHERE A.machine_code = '{0}'", aszOldMachineCode);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(szQuery);
                    if (dataReader != null)
                    {
                        #region Read Section Data.
                        while (dataReader.Read())
                        {
                            SectionInformation section = new SectionInformation
                            {
                                MachineCode = dataReader.GetValue(0).ToString(),
                                ModelCode = dataReader.GetValue(1).ToString(),
                                WorkType = dataReader.GetValue(2).ToString(),
                                Code = dataReader.GetValue(3).ToString(),
                                Name = dataReader.GetValue(4).ToString(),
                                Type = SectionManager.GetSectionType(dataReader.GetValue(5).ToString()),
                                Region = SectionManager.ParseRegionToRect(dataReader.GetValue(6).ToString().Split('X', 'Y', 'W', 'H')),
                                RGBColor = dataReader.GetValue(8).ToString(),
                                IterationStartX = Convert.ToInt32(dataReader.GetValue(9).ToString()),
                                IterationStartY = Convert.ToInt32(dataReader.GetValue(10).ToString()),
                                IterationCountX = Convert.ToInt32(dataReader.GetValue(11).ToString()),
                                IterationCountY = Convert.ToInt32(dataReader.GetValue(12).ToString()),
                                IterationJumpX = Convert.ToInt32(dataReader.GetValue(13).ToString()),
                                IterationJumpY = Convert.ToInt32(dataReader.GetValue(14).ToString()),
                                IterationPitchX = Convert.ToDouble(dataReader.GetValue(15).ToString()),
                                IterationPitchY = Convert.ToDouble(dataReader.GetValue(16).ToString()),
                                BlockCountX = Convert.ToInt32(dataReader.GetValue(17).ToString()),
                                BlockCountY = Convert.ToInt32(dataReader.GetValue(18).ToString()),
                                BlockPitchX = Convert.ToDouble(dataReader.GetValue(19).ToString()),
                                BlockPitchY = Convert.ToDouble(dataReader.GetValue(20).ToString()),
                                BlockCount = Convert.ToInt32(dataReader.GetValue(21).ToString()),
                                BlockPitch = Convert.ToDouble(dataReader.GetValue(22).ToString()),
                            };

                            #region DB에 기록된 문자열로부터 Region Data를 추출한다.
                            string szRegionData = dataReader.GetValue(7).ToString() + dataReader.GetValue(23).ToString();
                            if (!string.IsNullOrEmpty(szRegionData))
                            {
                                if(szRegionData.Contains('E'))
                                {
                                    foreach (string szRegionDatum in szRegionData.Split('X'))
                                    {
                                        if (!string.IsNullOrEmpty(szRegionDatum))
                                        {
                                            string[] szRegionTokens = szRegionDatum.Split('Y', 'I', 'J', 'E');
                                            if (szRegionTokens.Length == 5)
                                            {
                                                int nUnitX = Convert.ToInt32(szRegionTokens[0]);
                                                int nUnitY = Convert.ToInt32(szRegionTokens[1]);
                                                int nUnitXPosition = Convert.ToInt32(szRegionTokens[2]);
                                                int nUnitYPosition = Convert.ToInt32(szRegionTokens[3]);
                                                bool Is_Inspection = Convert.ToInt32(szRegionTokens[4]) == 1 ? true : false ;

                                                if (section.Region.X == nUnitXPosition && section.Region.Y == nUnitYPosition)
                                                {
                                                    section.IterationXPosition = nUnitX;
                                                    section.IterationYPosition = nUnitY;
                                                    section.SectionRegionList.Add(new SectionRegion(nUnitX, nUnitY, nUnitXPosition, nUnitYPosition, Is_Inspection, true)); // 기준 영역
                                                }
                                                else
                                                {
                                                    section.SectionRegionList.Add(new SectionRegion(nUnitX, nUnitY, nUnitXPosition, nUnitYPosition, Is_Inspection)); // 일반 영역
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (string szRegionDatum in szRegionData.Split('X'))
                                    {
                                        if (!string.IsNullOrEmpty(szRegionDatum))
                                        {
                                            string[] szRegionTokens = szRegionDatum.Split('Y', 'I', 'J');
                                            if (szRegionTokens.Length == 4)
                                            {
                                                int nUnitX = Convert.ToInt32(szRegionTokens[0]);
                                                int nUnitY = Convert.ToInt32(szRegionTokens[1]);
                                                int nUnitXPosition = Convert.ToInt32(szRegionTokens[2]);
                                                int nUnitYPosition = Convert.ToInt32(szRegionTokens[3]);
                                                if (section.Region.X == nUnitXPosition && section.Region.Y == nUnitYPosition)
                                                {
                                                    section.IterationXPosition = nUnitX;
                                                    section.IterationYPosition = nUnitY;
                                                    section.SectionRegionList.Add(new SectionRegion(nUnitX, nUnitY, nUnitXPosition, nUnitYPosition, true, true)); // 기준 영역
                                                }
                                                else
                                                {
                                                    section.SectionRegionList.Add(new SectionRegion(nUnitX, nUnitY, nUnitXPosition, nUnitYPosition, true)); // 일반 영역
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                            section.SectionRegionList = section.SectionRegionList.OrderBy(x => x.ToString().Length).ThenBy(x => x.ToString()).ToList();
                            section.HashID = section.GetHashCode();

                            Sections.Add(section);
                        }
                        dataReader.Close();
                        #endregion
                    }

                    // 읽혀진 Section 데이터를 Machine Code만 변경하여 DB에 다시 기록한다.
                    if (Sections.Count > 0)
                    {
                        ConnectFactory.DBConnector().StartTrans();
                        bTransactionStarted = true;

                        StringBuilder sbRegionData = new StringBuilder();
                        foreach (SectionInformation section in Sections)
                        {
                            sbRegionData.Clear();
                            foreach (SectionRegion sectionRegion in section.SectionRegionList)
                            {
                                sbRegionData.Append(sectionRegion.ToString());
                            }

                            string str1, str2;
                            if (sbRegionData.Length > 50000)
                            {
                                char[] c1 = new char[50000];
                                sbRegionData.CopyTo(0, c1, 0, 50000);
                                str1 = new string(c1);
                                char[] c2 = new char[sbRegionData.Length - 50000];
                                sbRegionData.CopyTo(50000, c2, 0, sbRegionData.Length - 50000);
                                str2 = new string(c2);
                            }
                            else
                            {
                                char[] c1 = new char[sbRegionData.Length];
                                sbRegionData.CopyTo(0, c1, 0, sbRegionData.Length);
                                str1 = new string(c1);
                                str2 = "";
                            }
                            #region Write Section Data.
                            szQuery = String.Format("INSERT INTO bgadb.section_info(" +
                                    "machine_code," +                   // 0
                                    "model_code," +                     // 1
                                    "section_code," +                   // 2
                                    "work_type," +                      // 3
                                    "section_name," +                   // 4
                                    "section_type," +                   // 5
                                    "section_rect," +                   // 6
                                    "region_data," +                    // 7
                                    "rgb_color," +                      // 8
                                    "iterstart_x," +                    // 9
                                    "iterstart_y," +                    // 10
                                    "itercount_x," +                    // 11
                                    "itercount_y," +                    // 12
                                    "iterjump_x," +                     // 13
                                    "iterjump_y," +                     // 14
                                    "iterpitch_x," +                    // 15
                                    "iterpitch_y," +                    // 16
                                    "blockcount_x," +                   // 17
                                    "blockcount_y," +                   // 18
                                    "blockpitch_x," +                   // 19
                                    "blockpitch_y," +                   // 20
                                    "blockcount," +                   // 20
                                    "blockpitch," +                   // 20
                                    "send_yn," +
                                    "reg_date," +
                                    "region_data2)" +
                                    "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, 0, now(), '{23}')",
                                    aszNewMachineCode,                  // 0
                                    section.ModelCode,                  // 1
                                    section.Code,                       // 2
                                    section.WorkType,                   // 3
                                    section.Name,                       // 4
                                    section.Type.Code,                  // 5
                                    SectionManager.ParseRectToRegion(section.Region),  // 6
                                    //sbRegionData.ToString(),            // 7
                                    str1,
                                    section.RGBColor,                   // 8
                                    section.IterationStartX,            // 9
                                    section.IterationStartY,            // 10
                                    section.IterationCountX,            // 11
                                    section.IterationCountY,            // 12
                                    section.IterationJumpX,             // 13
                                    section.IterationJumpY,             // 14
                                    section.IterationPitchX,            // 15
                                    section.IterationPitchY,            // 16
                                    section.BlockCountX,                // 17
                                    section.BlockCountY,                // 18
                                    section.BlockPitchX,                // 19
                                    section.BlockPitchY,
                                    section.BlockCount,
                                    section.BlockPitch,
                                    str2);               // 20

                            nQueryResult = ConnectFactory.DBConnector().Execute(szQuery);
                            if (nQueryResult < 1) // INSERT INTO bgadb.section_info is fail
                            {
                                break;
                            }
                            #endregion
                        }

                        if (nQueryResult > 0) ConnectFactory.DBConnector().Commit();
                        else ConnectFactory.DBConnector().Rollback();
                    }
                }
            }
            catch
            {
                if (dataReader != null) dataReader.Close(); // data reader가 닫히지 않았을 때 닫아준다.
                if (bTransactionStarted) ConnectFactory.DBConnector().Rollback(); // transaction 시작 후 Exception이 발생하면 롤백 작업을 수행한다.

                Debug.WriteLine("Exception occured in CopySectionDataToMachine(TeachingDataManager.cs");
            }
        }

        // DB의 ROI, Param 데이터를 복제한다 : A장비 -> B장비
        public void CopyROIDataToMachine(string aszOldMachineCode, string aszNewMachineCode)
        {
            bool bTransactionStarted = false;
            List<ROIInfo2> roiList = new List<ROIInfo2>();

            IDataReader dataReader = null;
            try
            {
                #region Load ROI Data.
                string szQuery = string.Format("SELECT A.model_code, A.work_type, A.section_code, A.roi_code, A.outer_roi_rect, A.local_align_points " +
                                               "FROM bgadb.roi_info A WHERE A.machine_code = '{0}'", aszOldMachineCode);
                dataReader = ConnectFactory.DBConnector().ExecuteQuery19(szQuery);
                if (dataReader != null)
                {
                    #region Read ROI Data.
                    while (dataReader.Read())
                    {
                        ROIInfo2 roi = new ROIInfo2
                        {
                            ModelCode = dataReader.GetValue(0).ToString(),
                            WorkType = dataReader.GetValue(1).ToString(),
                            SectionCode = dataReader.GetValue(2).ToString(),

                            Code = dataReader.GetValue(3).ToString(),
                            OuterPoints = dataReader.GetValue(4).ToString(),
                            InnerPoints = null,
                            LocalAlignPoints = dataReader.GetValue(5).ToString()
                        };
                        roiList.Add(roi);
                    }
                    dataReader.Close();
                    #endregion
                }
                #endregion

                #region Load ROI Param Data.
                int nROICount = roiList.Count;
                for (int i = 0; i < nROICount; i++)
                {
                    if (!string.IsNullOrEmpty(roiList[i].Code))
                    {
                        // 1. ROI에 포함된 inspect_code를 리스트로 가져온다.
                        szQuery = string.Format("SELECT A.Inspect_id, A.inspect_code, A.param_group, A.param_code, A.param_value " +
                                                 "FROM bgadb.roi_param A " +
                                                 "WHERE A.machine_code='{0}' " +
                                                 "AND A.model_code='{1}' " +
                                                 "AND A.work_type='{2}' " +
                                                 "AND A.section_code='{3}' " +
                                                 "AND A.roi_code='{4}'",
                                                 aszOldMachineCode, roiList[i].ModelCode, roiList[i].WorkType, roiList[i].SectionCode, roiList[i].Code);

                        dataReader = ConnectFactory.DBConnector().ExecuteQuery19(szQuery);
                        if (dataReader != null)
                        {
                            #region Read ROI Param Data.
                            while (dataReader.Read())
                            {
                                ROIParam roiParam = new ROIParam
                                {
                                    ID = dataReader.GetValue(0).ToString(),
                                    Code = dataReader.GetValue(1).ToString(),
                                    ParamGroup = dataReader.GetValue(2).ToString(),
                                    ParamCode = dataReader.GetValue(3).ToString(),
                                    ParamValue = Convert.ToInt32(dataReader.GetValue(4))
                                };

                                roiList[i].InspectionItems.Add(roiParam);
                            }
                            dataReader.Close();
                            #endregion
                        }
                    }
                }
                #endregion

                #region Save ROI Data.
                ConnectFactory.DBConnector().StartTrans();
                bTransactionStarted = true;

                int nQueryResult = 0;
                foreach (ROIInfo2 roi in roiList)
                {
                    szQuery = string.Format("INSERT INTO bgadb.roi_info(" +
                                             "machine_code," + // 0
                                             "model_code," + // 1
                                             "work_type," + // 2
                                             "section_code," + // 3
                                             "roi_code," + // 4
                                             "outer_roi_rect," + // 5
                                             //"inner_roi_rect," +
                                             "local_align_points," + // 6
                                             "send_yn) " +
                                             "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', 0)",
                                             aszNewMachineCode, roi.ModelCode, roi.WorkType, roi.SectionCode, roi.Code, roi.OuterPoints, roi.LocalAlignPoints);

                    nQueryResult = ConnectFactory.DBConnector().Execute(szQuery);
                    if (nQueryResult < 1) break; // INSERT INTO bgadb.roi_info fail

                    foreach (ROIParam roiParam in roi.InspectionItems)
                    {
                        #region Save ROI Param Data.
                        szQuery = string.Format("INSERT INTO bgadb.roi_param(" +
                                                "machine_code, " + // 0
                                                "model_code, " + // 1
                                                "work_type, " + // 2
                                                "section_code, " + // 3
                                                "roi_code, " + // 4
                                                "inspect_id, " + // 5
                                                "inspect_code, " + // 6
                                                "param_group, " + // 7
                                                "param_code, " + // 8
                                                "param_value, " + // 9 : Integer.
                                                "reg_date, user_id, send_yn) " +
                                                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, now(), 'system', 0)",
                                                aszNewMachineCode, roi.ModelCode, roi.WorkType, roi.SectionCode,
                                                roi.Code, roiParam.ID, roiParam.Code, roiParam.ParamGroup, roiParam.ParamCode, roiParam.ParamValue);

                        nQueryResult = ConnectFactory.DBConnector().Execute(szQuery);
                        if (nQueryResult < 1) break; // INSERT INTO bgadb.roi_param fail
                        #endregion
                    }

                    // 검사 설정값 저장 실패시 모든 작업을 취소하도록 함.
                    if (nQueryResult < 1) break;
                }

                if (nQueryResult > 0) { ConnectFactory.DBConnector().Commit(); }
                else { ConnectFactory.DBConnector().Rollback(); }
                #endregion
            }
            catch
            {
                if (dataReader != null) dataReader.Close(); // data reader가 닫히지 않았을 때 닫아준다.
                if (bTransactionStarted) ConnectFactory.DBConnector().Rollback(); // transaction 시작 후 Exception이 발생하면 롤백 작업을 수행한다.

                Debug.WriteLine("Exception occured in CopyROIDataToMachine(TeachingDataManager.cs");
            }
        }
    }
}
