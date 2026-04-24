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
 * @file  SectionManager.cs
 * @brief 
 *  Section Manager class.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.09.09
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.09.09 First creation.
 * - 2011.11.10 Re-factoring.
 */

using Common;
using Common.DataBase;
using Common.Drawing;
using Common.Drawing.InspectionInformation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common.Drawing.InspectionInformation;
using System.Diagnostics.Eventing.Reader;

namespace PCS.ModelTeaching
{
    /// <summary>   Manager for sections.  </summary>
    /// <remarks>   suoow2, 2014-09-09. </remarks>
    public class SectionManager
    {
        public ObservableCollection<SectionInformation> Sections
        {
            get
            {
                return m_listSectionInformation;
            }
            set
            {
                m_listSectionInformation = value;
            }
        }
        private ObservableCollection<SectionInformation> m_listSectionInformation = new ObservableCollection<SectionInformation>();

        public static bool CheckBackupNeeded(string strMachineCode, string strModelCode)
        {
            string strQuery = string.Format("SELECT count(section_info.section_code) FROM bgadb.section_info section_info " +
                                            "WHERE section_info.machine_code = '{0}' " +
                                            "AND section_info.model_code = '{1}'", strMachineCode, strModelCode);
            if (ConnectFactory.DBConnector().ExecuteScalarByInt(strQuery) > 0)
            {
                return true; // Need backup.
            }
            else
            {
                return false;
            }
        }

        public bool SaveSection(string strMachineCode, string strModelCode, Surface surface)
        {
            try
            {
                if (m_listSectionInformation.Count == 0)
                {
                    return true; // 섹션이 존재하지 않는 경우, 저장에 성공했다고 간주한다.
                }

                if (ConnectFactory.DBConnector() != null)
                {
                    int nSecCode = 0;
                    ConnectFactory.DBConnector().StartTrans();

                    int queryResult = 1;
                    string strQuery = string.Empty;
                    string strWorkType = WorkTypeCode.GetWorkTypeCode(surface);

                    StringBuilder sbRegionData = new StringBuilder();
                    foreach (SectionInformation section in m_listSectionInformation)
                    {
                        string copyid = section.RelatedROI.MotherROIID.ToString(); 
                        foreach (SectionRegion sectionRegion in section.SectionRegionList)
                        {
                            //if (section.Type.Code == SectionTypeCode.OUTER_REGION)
                            //{
                            //    SectionRegion tmp = new SectionRegion(sectionRegion.RegionIndex.X, sectionRegion.RegionIndex.Y, sectionRegion.RegionPosition.X * 4, sectionRegion.RegionPosition.Y * 4);
                            //    sbRegionData.Append(tmp.ToString());
                            //}
                            //else
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

                        string region = "";
                        if(section.Type.Code == SectionTypeCode.OUTER_REGION)
                        {
                            Int32Rect rct = new Int32Rect(section.Region.X, section.Region.Y, section.Region.Width, section.Region.Height);
                            region = ParseRectToRegion(rct);
                        }
                        else  region = ParseRectToRegion(section.Region);

                        section.Code = QueryHelper.GetCode(nSecCode++, 4);
                        strQuery = String.Format("INSERT INTO bgadb.section_info(" +
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
                                "region_data2, copy_id)" +
                                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, 0, now(),'{23}', '{24}')",
                                strMachineCode,                     // 0
                                strModelCode,                       // 1
                                section.Code,                       // 2
                                strWorkType,                        // 3
                                section.Name,                       // 4
                                section.Type.Code,                  // 5
                                region,
                                //ParseRectToRegion(section.Region),  // 6
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
                                str2, copyid);               // 20

                        queryResult = ConnectFactory.DBConnector().Execute(strQuery);
                        if (queryResult < 1) // INSERT INTO bgadb.section_info is fail
                        {
                            break;
                        }
                        sbRegionData.Clear();
                    }

                    if (queryResult > 0)
                    {
                        ConnectFactory.DBConnector().Commit();
                        Debug.WriteLine("Save section success(Commit) in  SaveSection(SectionManager.cs)");
                        return true;
                    }
                    else
                    {
                        ConnectFactory.DBConnector().Rollback();
                        Debug.WriteLine("Save section fail(Rollback) in  SaveSection(SectionManager.cs)");
                        return false;
                    }
                }
                else // DBConnector is null
                {
                    return false; // 저장 실패
                }
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in SaveSection(SectionManager.cs)");
                return false; // 저장 실패
            }
        }

        public void LoadSurfaceData(string aszMachineName, string aszMachineCode, string aszModelCode, Surface aSurface, string aszModePath, bool bMono)
        {
            if (m_listSectionType.Count == 0)
            {
                LoadSectionType();
            }
            m_listSectionInformation.Clear(); // Section 불러오기가 초기화될 때마다 리스트를 초기화시켜준다.

            IDataReader dataReader = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    // Section Image를 File System으로부터 가져오기 위해 사용된다.
                    string strGroupName = string.Empty;
                    string strModelName = aszModelCode;
                    string strWorkType = WorkTypeCode.GetWorkTypeCode(aSurface);

                    // Model Code로부터 분류 이름을 가져온다.
                    string strQuery = String.Format("SELECT com_detail.com_dname " +
                                                    "FROM bgadb.model_info model_info, bgadb.com_detail com_detail " +
                                                    "WHERE model_info.model_type = com_detail.com_dname AND model_info.model_code = '{0}'", aszModelCode);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            strGroupName = dataReader.GetValue(0).ToString();
                        }
                        dataReader.Close();
                    }

                    // WorkType에 해당되는 Section을 조회한다.
                    strQuery = String.Format("SELECT A.section_code, A.section_name, A.section_type, A.section_rect, A.region_data, A.rgb_color, " +
                                             "A.iterstart_x, A.iterstart_y, A.itercount_x, A.itercount_y, A.iterjump_x, A.iterjump_y, A.iterpitch_x, A.iterpitch_y, " +
                                             "A.blockcount_x, A.blockcount_y, A.blockpitch_x, A.blockpitch_y, A.blockcount, A.blockpitch, A.region_data2, A.copy_id " +
                                             "FROM bgadb.section_info A " +
                                             "WHERE A.machine_code = '{0}' AND A.model_code = '{1}' AND A.work_type = '{2}'",
                                             aszMachineCode, aszModelCode, strWorkType);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        #region Read Section Data.
                        while (dataReader.Read())
                        {
                            SectionInformation section = new SectionInformation
                            {
                                Code = dataReader.GetValue(0).ToString(),
                                Name = dataReader.GetValue(1).ToString(),
                                Type = GetSectionType(dataReader.GetValue(2).ToString()),
                                Region = ParseRegionToRect(dataReader.GetValue(3).ToString().Split('X', 'Y', 'W', 'H')),
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

                            if (dataReader.GetValue(21).ToString() != "")
                            {
                                if (section.RelatedROI == null)
                                {
                                    section.RelatedROI = new GraphicsRectangle();
                                }
                                section.RelatedROI.MotherROIID = Convert.ToInt32(dataReader.GetValue(21).ToString());
                            }
                            // DB에 기록된 문자열로부터 Region Data를 추출한다.
                            string szRegionData = dataReader.GetValue(4).ToString() + dataReader.GetValue(20).ToString();
                            if (!string.IsNullOrEmpty(szRegionData))
                            {
                                if(szRegionData.Contains('E'))
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
                                                    section.SectionRegionList.Add(new SectionRegion(nUnitX, nUnitY, nUnitXPosition, nUnitYPosition,true, true)); // 기준 영역
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
                            section.SectionRegionList = section.SectionRegionList.OrderBy(x => x.ToString().Length).ThenBy(x => x.ToString()).ToList();
                            section.HashID = section.GetHashCode();

                            // Image 경로 조회하여 이미지 파일이 있는지 확인 후 셋해준다.
                            if (strGroupName != string.Empty)
                            {
                                string[] szSectionImagePath = new string[3];
                                if (aSurface == Surface.BP1||aSurface==Surface.BP2 || bMono)
                                {
                                    if (!string.IsNullOrEmpty(aszMachineName))
                                    {
                                        szSectionImagePath[0] = DirectoryManager.GetOfflineSectionImagePath(aszModePath, aszMachineName, strGroupName, strModelName, section.Name, aSurface);
                                    }
                                    else
                                    {
                                        if(aSurface==Surface.BP2)
                                            szSectionImagePath[0] = DirectoryManager.GetSectionImagePath(aszModePath, strGroupName, strModelName, section.Name, Surface.BP1, -1);
                                        else
                                            szSectionImagePath[0] = DirectoryManager.GetSectionImagePath(aszModePath, strGroupName, strModelName, section.Name, aSurface, -1);
                                    }

                                    if (File.Exists(szSectionImagePath[0]))
                                    {
                                        section.SetBitmapSource(BitmapImageLoader.LoadCachedBitmapImage(new Uri(szSectionImagePath[0])) as BitmapSource, 0);
                                    }
                                    else
                                    {
                                        int nWidth = (int)(section.Region.Width / VisionDefinition.GRAB_IMAGE_SCALE);
                                        int nHeight = (int)(section.Region.Height / VisionDefinition.GRAB_IMAGE_SCALE);

                                        byte[] arrPixels = new byte[nWidth * nHeight];
                                        for (int nIndex = 0; nIndex < arrPixels.Length; nIndex++)
                                            arrPixels[nIndex] = 128;

                                        section.SetTempBitmapSource(BitmapSource.Create(section.Region.Width * 4, section.Region.Height * 4, 96, 96, PixelFormats.Indexed8, BitmapPalettes.Gray256, arrPixels, nWidth), 0);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(aszMachineName))
                                    {
                                        szSectionImagePath = DirectoryManager.GetOfflineSectionRGBImagePath(aszModePath, aszMachineName, strGroupName, strModelName, section.Name, aSurface);
                                    }
                                    else
                                    {
                                        szSectionImagePath = DirectoryManager.GetSectionRGBImagePath(aszModePath, strGroupName, strModelName, section.Name, aSurface);
                                    }
                                    for (int i = 0; i < 3; i++)
                                    {
                                        if (File.Exists(szSectionImagePath[i]))
                                        {
                                            if (section.Type.Code == SectionTypeCode.PSR_REGION && i == 1)
                                                section.SetBitmapSource(BitmapImageLoader.LoadCachedBitmapImageColor(new Uri(szSectionImagePath[i])) as BitmapSource, i);
                                            else
                                                section.SetBitmapSource(BitmapImageLoader.LoadCachedBitmapImage(new Uri(szSectionImagePath[i])) as BitmapSource, i);                                            
                                        }
                                        else
                                        {
                                            int nWidth = (int)(section.Region.Width / VisionDefinition.GRAB_IMAGE_SCALE);
                                            int nHeight = (int)(section.Region.Height / VisionDefinition.GRAB_IMAGE_SCALE);

                                            byte[] arrPixels = new byte[nWidth * nHeight];
                                            for (int nIndex = 0; nIndex < arrPixels.Length; nIndex++)
                                                arrPixels[nIndex] = 128;
                                            section.SetTempBitmapSource(BitmapSource.Create(section.Region.Width * 4, section.Region.Height * 4, 96, 96, PixelFormats.Indexed8, BitmapPalettes.Gray256, arrPixels, nWidth), i);                                           
                                        }
                                    }
                                }
                            }

                            if (section.GetBitmapSource(0) != null)
                            {
                                // Section 정보와 Image 데이터가 존재하면 Section 리스트에 포함시킨다. 
                                m_listSectionInformation.Add(section);
                            }
                        }
                        dataReader.Close();
                        #endregion
                    }
                }
            }
            catch(Exception e)
            {
                if (dataReader != null) dataReader.Close();
                Debug.WriteLine("Exception occured in LoadSection(SectionManager.cs) " + e.ToString());
            }
        }

        #region Section types.
        /// <summary>   Gets or sets a list of types of the sections. </summary>
        /// <value> A list of types of the sections. </value>
        public static List<SectionType> SectionTypes
        {
            get
            {
                return m_listSectionType;
            }
            set
            {
                m_listSectionType = value;
            }
        }

        /// <summary> Type of the list section </summary>
        private static List<SectionType> m_listSectionType = new List<SectionType>();

        /// <summary>   Loads the section type. </summary>
        /// <remarks>   suoow2, 2014-09-09. </remarks>
        public static void LoadSectionType()
        {
            if (m_listSectionType.Count > 0)
            {
                return;
            }

            if (ConnectFactory.DBConnector() != null)
            {
                // SectionType 리스트를 구성한다.
                // SectionType List는 Application 실행 시간에 변하지 않는다.
                String strQuery = "SELECT com_detail.com_dcode, com_detail.com_dname FROM bgadb.com_detail com_detail WHERE com_detail.com_mcode = '08' and com_detail.use_yn = 1";

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        string strCode = dataReader.GetValue(0).ToString();
                        string strName = dataReader.GetValue(1).ToString();

                        m_listSectionType.Add(new SectionType(strCode, strName));
                    }

                    dataReader.Close();
                }
            }
        }
        #endregion

        #region Helper functions.
        public static SectionType GetSectionType(string strCode)
        {
            if (m_listSectionType.Count == 0) LoadSectionType();
            foreach (SectionType sectionType in m_listSectionType)
            {
                if (sectionType.Code == strCode)
                {
                    return sectionType;
                }
            }

            return null;
        }

        public static string GetSectionType(eSectionType strCode)
        {
            string sSectionType;
            if (strCode == eSectionType.eSecTypeRegionGlobal) // Strip Align
            {
                sSectionType = SectionTypeCode.STRIP_REGION;
            }
            else if (strCode == eSectionType.eSecTypeRegionUnit) // Unit Region
            {
                sSectionType = SectionTypeCode.UNIT_REGION;
            }
            else if (strCode == eSectionType.eSecTypeRegionPsr) // Psr Region
            {
                sSectionType = SectionTypeCode.PSR_REGION;
            }
            else if (strCode == eSectionType.eSecTypeRegionRaw) // Raw Region
            {
                sSectionType = SectionTypeCode.RAW_REGION;
            }
            else if (strCode == eSectionType.eSecTypeRegionID) // ID Region
            {
                sSectionType = SectionTypeCode.ID_REGION;
            }
            else if (strCode == eSectionType.eSecTypeRegionOuter)
            {
                sSectionType = SectionTypeCode.OUTER_REGION;
            }
            else
                sSectionType = "";
            return sSectionType;
        }

        public static Int32Rect ParseRegionToRect(string[] strTokens)
        {
            if (strTokens.Length == 5)
            {
                return new Int32Rect(Convert.ToInt32(strTokens[1]), Convert.ToInt32(strTokens[2]), Convert.ToInt32(strTokens[3]), Convert.ToInt32(strTokens[4]));
            }
            else
            {
                return new Int32Rect(0, 0, 0, 0);
            }
        }

        public static string ParseRectToRegion(Int32Rect rect)
        {
            return string.Format("X{0}Y{1}W{2}H{3}", rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static void AddSectionToROICanvas(DrawingCanvas roiCanvas, SectionInformation section)
        {
            if (section.Type.Code == SectionTypeCode.STRIP_REGION)
            {
                #region Strip Align Section ROI.
                GraphicsRectangle stripAlignGraphic = new GraphicsRectangle(
                    section.Region.X,
                    section.Region.Y,
                    section.Region.X + section.Region.Width - 1,
                    section.Region.Y + section.Region.Height - 1,
                    roiCanvas.LineWidth,
                    GraphicsRegionType.StripAlign,
                    GraphicsColors.Red,
                    roiCanvas.ActualScale,
                    new IterationSymmetryInformation(section.IterationStartX, section.IterationStartY, section.IterationJumpX, section.IterationJumpY),
                    new IterationInformation(section.IterationCountX, section.IterationCountY, section.IterationPitchX, section.IterationPitchY),
                    new IterationInformation(section.BlockCountX, section.BlockCountY, section.BlockPitchX, section.BlockPitchY),
                    true,
                    null);
                stripAlignGraphic.IsValidRegion = true;
                stripAlignGraphic.IterationXPosition = 0;
                stripAlignGraphic.IterationYPosition = 0;
                stripAlignGraphic.Caption = CaptionHelper.StripAlignCaption;

                roiCanvas.GraphicsList.Add(stripAlignGraphic);
                roiCanvas.FiduGraphicsList.Add(stripAlignGraphic);
                section.RelatedROI = stripAlignGraphic;
                #endregion
            }
            else if (section.Type.Code == SectionTypeCode.UNIT_REGION)
            {
                Color ObjectColor = GraphicsColors.Green;


                if (!string.IsNullOrEmpty(section.RGBColor))
                {
                    ObjectColor = Color.FromArgb(255, Convert.ToByte(int.Parse(section.RGBColor.Substring(0, 2), NumberStyles.AllowHexSpecifier)),
                                                      Convert.ToByte(int.Parse(section.RGBColor.Substring(2, 2), NumberStyles.AllowHexSpecifier)),
                                                      Convert.ToByte(int.Parse(section.RGBColor.Substring(4, 2), NumberStyles.AllowHexSpecifier)));
                }

                // Section Region의 공통 속성.
                int nWidth = section.Region.Width;
                int nHeight = section.Region.Height;
                IterationSymmetryInformation symmetryInformation = new IterationSymmetryInformation(section.IterationStartX, section.IterationStartY, section.IterationJumpX, section.IterationJumpY);
                IterationInformation iterationInformation = new IterationInformation(section.IterationCountX, section.IterationCountY, section.IterationPitchX, section.IterationPitchY);
                IterationInformation blockIterationInformation = new IterationInformation(section.BlockCount, section.BlockCountX, section.BlockCountY, section.BlockPitch, section.BlockPitchX, section.BlockPitchY);
                GraphicsRectangle fiduUnitGraphic = null;

                foreach (SectionRegion region in section.SectionRegionList)
                {
                    if (region.RegionPosition.X == section.Region.X && // 기준 영역에 해당된다.
                        region.RegionPosition.Y == section.Region.Y)
                    {
                        fiduUnitGraphic = new GraphicsRectangle(
                            region.RegionPosition.X,
                            region.RegionPosition.Y,
                            region.RegionPosition.X + nWidth - 1,
                            region.RegionPosition.Y + nHeight - 1,
                            roiCanvas.LineWidth,
                            GraphicsRegionType.UnitRegion,
                            ObjectColor,
                            roiCanvas.ActualScale,
                            symmetryInformation, iterationInformation, blockIterationInformation,
                            true,
                            null);
                        fiduUnitGraphic.IsValidRegion = true;
                        
                        fiduUnitGraphic.IterationXPosition = region.RegionIndex.X;
                        fiduUnitGraphic.IterationYPosition = region.RegionIndex.Y;
                        fiduUnitGraphic.IsInspection = true;

                        fiduUnitGraphic.BlockIterationValue = blockIterationInformation;
                        fiduUnitGraphic.IterationValue = iterationInformation;
              
                        fiduUnitGraphic.Caption = CaptionHelper.FiducialUnitRegionCaption;

                        roiCanvas.GraphicsList.Add(fiduUnitGraphic);
                        roiCanvas.FiduGraphicsList.Add(fiduUnitGraphic);
                        section.RelatedROI = fiduUnitGraphic;
                        break;
                    }
                }

                Debug.Assert(fiduUnitGraphic != null);

                foreach (SectionRegion region in section.SectionRegionList)
                {
                    if (region.RegionPosition.X != section.Region.X || // 기준 영역이 아닌 경우에 해당됨.
                        region.RegionPosition.Y != section.Region.Y)
                    {
                        GraphicsRectangle regionGraphic = new GraphicsRectangle(
                            region.RegionPosition.X,
                            region.RegionPosition.Y,
                            region.RegionPosition.X + nWidth - 1,
                            region.RegionPosition.Y + nHeight - 1,
                            roiCanvas.LineWidth,
                            GraphicsRegionType.UnitRegion,
                            region.IsInspection == true ? ObjectColor : GraphicsColors.Blue,
                            roiCanvas.ActualScale,
                            symmetryInformation, iterationInformation, blockIterationInformation,
                            false,
                            fiduUnitGraphic);

                        regionGraphic.OriginObjectColor = (region.IsInspection == true) ? ObjectColor : GraphicsColors.Blue;
                        regionGraphic.IsValidRegion = true;
                        regionGraphic.IterationXPosition = region.RegionIndex.X;
                        regionGraphic.IterationYPosition = region.RegionIndex.Y;
                        regionGraphic.IsInspection = region.IsInspection;

                        fiduUnitGraphic.BlockIterationValue = blockIterationInformation;
                        fiduUnitGraphic.IterationValue = iterationInformation;

                        if (region.IsInspection) regionGraphic.Caption = CaptionHelper.GetRegionCaption(regionGraphic);
                        else regionGraphic.Caption = CaptionHelper.ExceptionalMaskCaption;

                        roiCanvas.GraphicsList.Add(regionGraphic);
                    }
                }
            }
            else if (section.Type.Code == SectionTypeCode.PSR_REGION)
            {
                Color ObjectColor = GraphicsColors.Green;
                if (!string.IsNullOrEmpty(section.RGBColor))
                {
                    ObjectColor = Color.FromArgb(255, Convert.ToByte(int.Parse(section.RGBColor.Substring(0, 2), NumberStyles.AllowHexSpecifier)),
                                                      Convert.ToByte(int.Parse(section.RGBColor.Substring(2, 2), NumberStyles.AllowHexSpecifier)),
                                                      Convert.ToByte(int.Parse(section.RGBColor.Substring(4, 2), NumberStyles.AllowHexSpecifier)));
                }

                // Section Region의 공통 속성.
                int nWidth = section.Region.Width;
                int nHeight = section.Region.Height;
                IterationSymmetryInformation symmetryInformation = new IterationSymmetryInformation(section.IterationStartX, section.IterationStartY, section.IterationJumpX, section.IterationJumpY);
                IterationInformation iterationInformation = new IterationInformation(section.IterationCountX, section.IterationCountY, section.IterationPitchX, section.IterationPitchY);
                IterationInformation blockIterationInformation = new IterationInformation(section.BlockCount, section.BlockCountX, section.BlockCountY, section.BlockPitch, section.BlockPitchX, section.BlockPitchY);
                GraphicsRectangle fiduUnitGraphic = null;

                foreach (SectionRegion region in section.SectionRegionList)
                {
                    if (region.RegionPosition.X == section.Region.X && // 기준 영역에 해당된다.
                        region.RegionPosition.Y == section.Region.Y)
                    {
                        fiduUnitGraphic = new GraphicsRectangle(
                            region.RegionPosition.X,
                            region.RegionPosition.Y,
                            region.RegionPosition.X + nWidth - 1,
                            region.RegionPosition.Y + nHeight - 1,
                            roiCanvas.LineWidth,
                            GraphicsRegionType.PSROdd,
                            ObjectColor,
                            roiCanvas.ActualScale,
                            symmetryInformation, iterationInformation, blockIterationInformation,
                            true,
                            null);
                        fiduUnitGraphic.IsValidRegion = true;

                        fiduUnitGraphic.IterationXPosition = region.RegionIndex.X;
                        fiduUnitGraphic.IterationYPosition = region.RegionIndex.Y;
                        fiduUnitGraphic.BlockIterationValue = blockIterationInformation;
                        fiduUnitGraphic.IterationValue = iterationInformation;
                        fiduUnitGraphic.Caption = CaptionHelper.FiducialPsrRegionCaption;

                        roiCanvas.GraphicsList.Add(fiduUnitGraphic);
                        roiCanvas.FiduGraphicsList.Add(fiduUnitGraphic);
                        section.RelatedROI = fiduUnitGraphic;
                        break;
                    }
                }

                Debug.Assert(fiduUnitGraphic != null);

                foreach (SectionRegion region in section.SectionRegionList)
                {
                    if (region.RegionPosition.X != section.Region.X || // 기준 영역이 아닌 경우에 해당됨.
                        region.RegionPosition.Y != section.Region.Y)
                    {
                        GraphicsRectangle regionGraphic = new GraphicsRectangle(
                            region.RegionPosition.X,
                            region.RegionPosition.Y,
                            region.RegionPosition.X + nWidth - 1,
                            region.RegionPosition.Y + nHeight - 1,
                            roiCanvas.LineWidth,
                            GraphicsRegionType.PSROdd,
                            region.IsInspection == true ? ObjectColor : GraphicsColors.Blue,
                            roiCanvas.ActualScale,
                            symmetryInformation, iterationInformation, blockIterationInformation,
                            false,
                            fiduUnitGraphic);
                        regionGraphic.OriginObjectColor = (region.IsInspection == true) ? ObjectColor : GraphicsColors.Blue;
                        regionGraphic.IsValidRegion = true;
                        regionGraphic.IterationXPosition = region.RegionIndex.X;
                        regionGraphic.IterationYPosition = region.RegionIndex.Y;
                        regionGraphic.IsInspection = region.IsInspection;

                        fiduUnitGraphic.BlockIterationValue = blockIterationInformation;
                        fiduUnitGraphic.IterationValue = iterationInformation;

                        if (region.IsInspection) regionGraphic.Caption = CaptionHelper.GetRegionCaption(regionGraphic);
                        else regionGraphic.Caption = CaptionHelper.ExceptionalMaskCaption;

                        roiCanvas.GraphicsList.Add(regionGraphic);
                    }
                }
            }
            else if (section.Type.Code == SectionTypeCode.ID_REGION)
            {
                // Section Region의 공통 속성.
                int nWidth = section.Region.Width;
                int nHeight = section.Region.Height;
                IterationSymmetryInformation symmetryInformation = new IterationSymmetryInformation(section.IterationStartX, section.IterationStartY, section.IterationJumpX, section.IterationJumpY);
                IterationInformation iterationInformation = new IterationInformation(section.IterationCountX, section.IterationCountY, section.IterationPitchX, section.IterationPitchY);
                IterationInformation blockIterationInformation = new IterationInformation(section.BlockCount, section.BlockCountX, section.BlockCountY, section.BlockPitch, section.BlockPitchX, section.BlockPitchY);
                GraphicsRectangle fiduUnitGraphic = null;

                foreach (SectionRegion region in section.SectionRegionList)
                {
                    if (region.RegionPosition.X == section.Region.X && // 기준 영역에 해당된다.
                        region.RegionPosition.Y == section.Region.Y)
                    {
                        fiduUnitGraphic = new GraphicsRectangle(
                            region.RegionPosition.X,
                            region.RegionPosition.Y,
                            region.RegionPosition.X + nWidth - 1,
                            region.RegionPosition.Y + nHeight - 1,
                            roiCanvas.LineWidth,
                            GraphicsRegionType.IDRegion,
                            GraphicsColors.Green,
                            roiCanvas.ActualScale,
                            symmetryInformation, iterationInformation, blockIterationInformation,
                            true,
                            null);
                        fiduUnitGraphic.IsValidRegion = true;
                        fiduUnitGraphic.IterationXPosition = region.RegionIndex.X;
                        fiduUnitGraphic.IterationYPosition = region.RegionIndex.Y;
                        fiduUnitGraphic.BlockIterationValue = blockIterationInformation;
                        fiduUnitGraphic.IterationValue = iterationInformation;
                        fiduUnitGraphic.Caption = CaptionHelper.FiducialIDRegionCaption;

                        if (section.RelatedROI != null)
                        {
                            fiduUnitGraphic.MotherROIID = section.RelatedROI.MotherROIID;
                        }

                        roiCanvas.GraphicsList.Add(fiduUnitGraphic);
                        roiCanvas.FiduGraphicsList.Add(fiduUnitGraphic);
                        section.RelatedROI = fiduUnitGraphic;
                        break;
                    }
                }

                Debug.Assert(fiduUnitGraphic != null);

                foreach (SectionRegion region in section.SectionRegionList)
                {
                    if (region.RegionPosition.X != section.Region.X || // 기준 영역이 아닌 경우에 해당됨.
                        region.RegionPosition.Y != section.Region.Y)
                    {
                        GraphicsRectangle regionGraphic = new GraphicsRectangle(
                            region.RegionPosition.X,
                            region.RegionPosition.Y,
                            region.RegionPosition.X + nWidth - 1,
                            region.RegionPosition.Y + nHeight - 1,
                            roiCanvas.LineWidth,
                            GraphicsRegionType.IDRegion,
                            GraphicsColors.Green,
                            roiCanvas.ActualScale,
                            symmetryInformation, iterationInformation, blockIterationInformation,
                            false,
                            fiduUnitGraphic);
                        regionGraphic.OriginObjectColor = GraphicsColors.Green;
                        regionGraphic.IsValidRegion = true;
                        regionGraphic.IterationXPosition = region.RegionIndex.X;
                        regionGraphic.IterationYPosition = region.RegionIndex.Y;
                        fiduUnitGraphic.BlockIterationValue = blockIterationInformation;
                        fiduUnitGraphic.IterationValue = iterationInformation;
                        regionGraphic.Caption = CaptionHelper.GetRegionCaption(regionGraphic);

                        roiCanvas.GraphicsList.Add(regionGraphic);
                    }
                }
            }
            else if (section.Type.Code == SectionTypeCode.OUTER_REGION)
            {
                // Section Region의 공통 속성.
                int nWidth = section.Region.Width;
                int nHeight = section.Region.Height;
                IterationSymmetryInformation symmetryInformation = new IterationSymmetryInformation(section.IterationStartX, section.IterationStartY, section.IterationJumpX, section.IterationJumpY);
                IterationInformation iterationInformation = new IterationInformation(section.IterationCountX, section.IterationCountY, section.IterationPitchX, section.IterationPitchY);
                IterationInformation blockIterationInformation = new IterationInformation(section.BlockCount, section.BlockCountX, section.BlockCountY, section.BlockPitch, section.BlockPitchX, section.BlockPitchY);
                GraphicsRectangle fiduUnitGraphic = null;

                foreach (SectionRegion region in section.SectionRegionList)
                {
                    if (region.RegionPosition.X == section.Region.X && // 기준 영역에 해당된다.
                        region.RegionPosition.Y == section.Region.Y)
                    {
                        fiduUnitGraphic = new GraphicsRectangle(
                            region.RegionPosition.X,
                            region.RegionPosition.Y,
                            region.RegionPosition.X + nWidth - 1,
                            region.RegionPosition.Y + nHeight - 1,
                            roiCanvas.LineWidth,
                            GraphicsRegionType.OuterRegion,
                            GraphicsColors.Green,
                            roiCanvas.ActualScale,
                            symmetryInformation, iterationInformation, blockIterationInformation,
                            true,
                            null);
                        fiduUnitGraphic.IsValidRegion = true;
                        fiduUnitGraphic.IterationXPosition = region.RegionIndex.X;
                        fiduUnitGraphic.IterationYPosition = region.RegionIndex.Y;
                        fiduUnitGraphic.BlockIterationValue = blockIterationInformation;
                        fiduUnitGraphic.IterationValue = iterationInformation;
                        fiduUnitGraphic.Caption = CaptionHelper.FiducialOuterRegionCaption;

                        if (section.RelatedROI != null)
                        {
                            fiduUnitGraphic.MotherROIID = section.RelatedROI.MotherROIID;
                        }

                        roiCanvas.GraphicsList.Add(fiduUnitGraphic);
                        roiCanvas.FiduGraphicsList.Add(fiduUnitGraphic);
                        section.RelatedROI = fiduUnitGraphic;
                        break;
                    }
                }

                Debug.Assert(fiduUnitGraphic != null);

                foreach (SectionRegion region in section.SectionRegionList)
                {
                    if (region.RegionPosition.X != section.Region.X || // 기준 영역이 아닌 경우에 해당됨.
                        region.RegionPosition.Y != section.Region.Y)
                    {
                        GraphicsRectangle regionGraphic = new GraphicsRectangle(
                            region.RegionPosition.X,
                            region.RegionPosition.Y,
                            region.RegionPosition.X + nWidth - 1,
                            region.RegionPosition.Y + nHeight - 1,
                            roiCanvas.LineWidth,
                            GraphicsRegionType.OuterRegion,
                            GraphicsColors.Green,
                            roiCanvas.ActualScale,
                            symmetryInformation, iterationInformation, blockIterationInformation,
                            false,
                            fiduUnitGraphic);
                        regionGraphic.OriginObjectColor = GraphicsColors.Green;
                        regionGraphic.IsValidRegion = true;
                        regionGraphic.IterationXPosition = region.RegionIndex.X;
                        regionGraphic.IterationYPosition = region.RegionIndex.Y;
                        fiduUnitGraphic.BlockIterationValue = blockIterationInformation;
                        fiduUnitGraphic.IterationValue = iterationInformation;
                        regionGraphic.Caption = CaptionHelper.GetRegionCaption(regionGraphic);

                        roiCanvas.GraphicsList.Add(regionGraphic);
                    }
                }
            }
            else if (section.Type.Code == SectionTypeCode.RAW_REGION)
            {
               
                int nWidth = section.Region.Width;
                int nHeight = section.Region.Height;
                IterationSymmetryInformation symmetryInformation = new IterationSymmetryInformation(section.IterationStartX, section.IterationStartY, section.IterationJumpX, section.IterationJumpY);
                IterationInformation iterationInformation = new IterationInformation(section.IterationCountX, section.IterationCountY, section.IterationPitchX, section.IterationPitchY);
                IterationInformation blockIterationInformation = new IterationInformation(section.BlockCount, section.BlockCountX, section.BlockCountY, section.BlockPitch, section.BlockPitchX, section.BlockPitchY);
                GraphicsRectangle fiduUnitGraphic = null;

                foreach (SectionRegion region in section.SectionRegionList)
                {
                    if (region.RegionPosition.X == section.Region.X && // 기준 영역에 해당된다.
                        region.RegionPosition.Y == section.Region.Y)
                    {
                        fiduUnitGraphic = new GraphicsRectangle(
                            region.RegionPosition.X,
                            region.RegionPosition.Y,
                            region.RegionPosition.X + nWidth - 1,
                            region.RegionPosition.Y + nHeight - 1,
                            roiCanvas.LineWidth,
                            GraphicsRegionType.Rawmetrial,
                            GraphicsColors.Green,
                            roiCanvas.ActualScale,
                            symmetryInformation, iterationInformation, blockIterationInformation,
                            true,
                            null);
                        fiduUnitGraphic.IsValidRegion = true;
                        fiduUnitGraphic.IterationXPosition = region.RegionIndex.X;
                        fiduUnitGraphic.IterationYPosition = region.RegionIndex.Y;
                        fiduUnitGraphic.BlockIterationValue = blockIterationInformation;
                        fiduUnitGraphic.IterationValue = iterationInformation;
                        fiduUnitGraphic.Caption = CaptionHelper.FiducialRawRegionCaption;

                        roiCanvas.GraphicsList.Add(fiduUnitGraphic);
                        roiCanvas.FiduGraphicsList.Add(fiduUnitGraphic);
                        section.RelatedROI = fiduUnitGraphic;
                        break;
                    }
                }

                Debug.Assert(fiduUnitGraphic != null);

                foreach (SectionRegion region in section.SectionRegionList)
                {
                    if (region.RegionPosition.X != section.Region.X || // 기준 영역이 아닌 경우에 해당됨.
                        region.RegionPosition.Y != section.Region.Y)
                    {
                        GraphicsRectangle regionGraphic = new GraphicsRectangle(
                            region.RegionPosition.X,
                            region.RegionPosition.Y,
                            region.RegionPosition.X + nWidth - 1,
                            region.RegionPosition.Y + nHeight - 1,
                            roiCanvas.LineWidth,
                            GraphicsRegionType.Rawmetrial,
                            GraphicsColors.Green,
                            roiCanvas.ActualScale,
                            symmetryInformation, iterationInformation, blockIterationInformation,
                            false,
                            fiduUnitGraphic);
                        regionGraphic.OriginObjectColor = GraphicsColors.Green;
                        regionGraphic.IsValidRegion = true;
                        regionGraphic.IterationXPosition = region.RegionIndex.X;
                        regionGraphic.IterationYPosition = region.RegionIndex.Y;
                        fiduUnitGraphic.BlockIterationValue = blockIterationInformation;
                        fiduUnitGraphic.IterationValue = iterationInformation;
                        regionGraphic.Caption = CaptionHelper.GetRegionCaption(regionGraphic);

                        roiCanvas.GraphicsList.Add(regionGraphic);
                    }
                }
            }
        }

        /// <summary>   Graphics to section. </summary>
        /// <remarks>   suoow2, 2014-10-12. </remarks>
        public static SectionInformation GraphicsToSection(GraphicsRectangle aFiducialGraphic, DrawingCanvas aROICanvas, string aszMachineCode, string aszModelCode, int anSectionCode = 0)
        {
            SectionInformation section = new SectionInformation
            {
                HashID = aFiducialGraphic.ID,
                Code = QueryHelper.GetCode(anSectionCode, 4),
                MachineCode = aszMachineCode,
                ModelCode = aszModelCode,
                RGBColor = aFiducialGraphic.OriginObjectColor.ToString().Substring(3, 6),
                IterationXPosition = aFiducialGraphic.IterationXPosition,
                IterationYPosition = aFiducialGraphic.IterationYPosition,
                IterationStartX = aFiducialGraphic.SymmetryValue.StartX,
                IterationStartY = aFiducialGraphic.SymmetryValue.StartY,
                IterationJumpX = aFiducialGraphic.SymmetryValue.JumpX,
                IterationJumpY = aFiducialGraphic.SymmetryValue.JumpY,
                IterationCountX = aFiducialGraphic.IterationValue.Column,
                IterationCountY = aFiducialGraphic.IterationValue.Row,
                IterationPitchX = aFiducialGraphic.IterationValue.XPitch,
                IterationPitchY = aFiducialGraphic.IterationValue.YPitch,
                BlockCountX = aFiducialGraphic.BlockIterationValue.Column,
                BlockCountY = aFiducialGraphic.BlockIterationValue.Row,
                BlockPitchX = aFiducialGraphic.BlockIterationValue.XPitch,
                BlockPitchY = aFiducialGraphic.BlockIterationValue.YPitch,
                BlockCount = aFiducialGraphic.BlockIterationValue.Block,
                BlockPitch = aFiducialGraphic.BlockIterationValue.Gap
            };
            section.Region = new Int32Rect((int)Math.Round(aFiducialGraphic.Left),
                                           (int)Math.Round(aFiducialGraphic.Top),
                                           (int)Math.Round(aFiducialGraphic.WidthProperty),
                                           (int)Math.Round(aFiducialGraphic.HeightProperty));

            // Sets SectionRegionList
            section.SectionRegionList.Add(new SectionRegion(aFiducialGraphic.IterationXPosition, aFiducialGraphic.IterationYPosition,
                                                                (int)Math.Round(aFiducialGraphic.Left), (int)Math.Round(aFiducialGraphic.Top), aFiducialGraphic.IsInspection, true));
            foreach (GraphicsRectangle g in aROICanvas.GraphicsRectangleList)
            {
                if (g == aFiducialGraphic)
                    continue;
                if (g.MotherROI == aFiducialGraphic)
                    section.SectionRegionList.Add(new SectionRegion(g.IterationXPosition, g.IterationYPosition, (int)Math.Round(g.Left), (int)Math.Round(g.Top), g.IsInspection));
            }
            section.SectionRegionList = section.SectionRegionList.OrderBy(x => x.ToString().Length).ThenBy(x => x.ToString()).ToList();
            return section;
        }
        #endregion
    }
}
