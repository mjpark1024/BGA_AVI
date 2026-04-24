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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Common.DataBase;
using Common;
using System.Diagnostics;
using RVS.Generic.Class;
using System.Windows.Media.Imaging;

namespace RVS.Generic.Insp
{
    /// <summary>   Lot result data.  </summary>
    public class LotResultData
    {
        public string LotNo { get; set; }
        public string WorkType { get; set; }
        public string DefectName { get; set; }
        public int DefectCount { get; set; }
        public int StripInspectCount { get; set; }
        public int StripDefectCount { get; set; }
        public int UnitInspectCount { get; set; }
        public int UnitDefectCount { get; set; }
    }

    public class InspectDataManager
    {
        #region Set Defect Name List & Get Defect Name.
        // 결함 코드, 이름을 리스트로 보관한다.
        public static List<KeyValuePair<int, string>> DefectNameList = new List<KeyValuePair<int, string>>();

        public static void SetDefectList()
        {
            DefectNameList.Clear();
            IDataReader dataReader = null;
            try
            {
                if(ConnectFactory.DBConnector() != null)
                {
                    string szQuery = "SELECT A.defect_code, A.defect_name FROM bgadb.defect_info A";

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(szQuery);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            int nCode = Convert.ToInt32(dataReader.GetValue(0));
                            string szName = dataReader.GetValue(1).ToString();

                            DefectNameList.Add(new KeyValuePair<int, string>(nCode, szName));
                        }
                        dataReader.Close();
                    }
                }
            }
            catch
            {
                if (dataReader != null) dataReader.Close();
                Debug.WriteLine("Exception occured in SetDefectList(InspectDataManager.cs)");
            }
        }

        public static string GetNGName(int aResultType)
        {
            string szNGName = "-";
            if (DefectNameList != null)
            {
                foreach (var element in DefectNameList)
                {
                    if (element.Key == aResultType)
                    {
                        szNGName = element.Value;
                        break;
                    }
                }
            }
            return szNGName;
        }
        #endregion
        
        public List<bool> LoadLotNGStatus(string aszModelCode, string aszLotNo)
        {
            IDataReader dataReader = null;
            List<bool> lotNGStatusList = new List<bool>();
           
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = "SELECT defect_yn FROM bgadb.inspect_result";
                    strQuery += String.Format(" WHERE machine_code = '{0}' and model_code = '{1}' and lot_no = '{2}'",
                                                Settings.GetSettings().General.MachineCode, aszModelCode, aszLotNo);
                    strQuery += "     and closed_yn = 1";
                    strQuery += "     ORDER BY strip_id ASC";

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            try
                            {
                                lotNGStatusList.Add(Convert.ToBoolean(dataReader.GetValue(0).ToString()));
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.ToString());
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
                Debug.WriteLine("Exception occured in LoadLotDefectData(InspectDataManager.cs)");
            }

            return lotNGStatusList;
        }
        
        public ObservableCollection<NGCountData> LoadVerifyInfo(string aszModelCode, string aszLotNo)
        {
            IDataReader dataReader = null;
            ObservableCollection<NGCountData> listData = new ObservableCollection<NGCountData>();
            
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = " SELECT c.verify_defect_code, COUNT(*) FROM (";
                    strQuery += "     SELECT b.verify_defect_code FROM bgadb.inspect_result a, bgadb.defect_result b";
                    strQuery += String.Format(" WHERE a.machine_code = '{0}' and a.model_code = '{1}' and a.lot_no = '{2}'", 
                                                        Settings.GetSettings().General.MachineCode, aszModelCode, aszLotNo);
                    strQuery += "     and a.machine_code = b.machine_code";
                    strQuery += "     and a.result_code = b.result_code";
                    strQuery += "     and a.model_code = b.model_code";
                    strQuery += "     GROUP BY a.result_code) c";
                    strQuery += "     GROUP BY c.verify_defect_code";

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            NGCountData data = new NGCountData();

                            data.NGName = dataReader.GetValue(0).ToString();
                            data.NGCount = Convert.ToInt32(dataReader.GetValue(1).ToString());
                            listData.Add(data);
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
                Debug.WriteLine("Exception occured in LoadLotDefectData(InspectDataManager.cs)");
            }

            return listData;
        }

        public ObservableCollection<LotResultData> LoadLotDefectData(string aszModelName, string aszLotNo)
        {
            IDataReader dataReader = null;
            ObservableCollection<LotResultData> listData = new ObservableCollection<LotResultData>();
            if (aszLotNo == null)
            {
                aszLotNo = "*";
            }

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = " SELECT  x.DefectName, ifnull(sum(x.DefectCount),0) ";
                    strQuery += "     FROM ";
                    strQuery += "    (SELECT e.com_dname DefectName,  Count(d.defect_code) DefectCount ";
                    strQuery += "     FROM inspect_result a, inspect_result_detail b, defect_result c, defect_info d, com_detail e";
                    strQuery += "     WHERE a.machine_code = b.machine_code ";
                    strQuery += String.Format(" and a.machine_code = '{0}' and a.model_code = '{1}' and a.lot_no = '{2}'", Settings.GetSettings().General.MachineCode, aszModelName, aszLotNo);
                    strQuery += "     and a.model_code = b.model_code";
                    strQuery += "     and a.result_code = b.result_code";
                    strQuery += "     and b.machine_code = c.machine_code";
                    strQuery += "     and b.model_code = c.model_code";
                    strQuery += "     and b.result_code = c.result_code";
                    strQuery += "     and b.roi_code = c.roi_code";
                    strQuery += "     and b.section_code = c.section_code";
                    strQuery += "     and b.work_type = c.work_type";
                    strQuery += "     and c.defect_code = d.defect_code";
                    strQuery += "     and d.defect_group = e.com_dcode";
                    strQuery += "     and e.com_mcode = '20'";
                    strQuery += "     and e.use_yn = 1";
                    strQuery += "     and a.defect_yn = 1";
                    strQuery += "     and b.defect_yn = 1";
                    strQuery += "     and d.use_yn = 1";                    
                    strQuery += "     group by e.com_dname ";
                    strQuery += "     union all ";
                    strQuery += "     SELECT com_dname DefectName, 0 DefectCount ";
                    strQuery += "     FROM com_detail WHERE com_mcode = '20' and use_yn = 1) x  group by x.defectName";

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            LotResultData LotData = new LotResultData();

                            LotData.LotNo = aszLotNo;
                            LotData.DefectName = dataReader.GetValue(0).ToString();
                            LotData.DefectCount = Convert.ToInt32(dataReader.GetValue(1).ToString());

                            listData.Add(LotData);
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
                Debug.WriteLine("Exception occured in LoadLotDefectData(InspectDataManager.cs)");
            }

            return listData;
        }

        public static ObservableCollection<LotResultData> LoadLotDefectDataDetail(string strLotNo)
        {
            IDataReader dataReader = null;
            ObservableCollection<LotResultData> listData = new ObservableCollection<LotResultData>();
            if (strLotNo == null)
            {
                strLotNo = "*";
            }

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = " SELECT  x.DefectName, ifnull(sum(x.DefectCount),0) ";
                    strQuery += "     FROM ";
                    strQuery += "    (SELECT d.defect_name DefectName,  Count(d.defect_code) DefectCount ";
                    strQuery += "     FROM inspect_result a, inspect_result_detail b, defect_result c, defect_info d";
                    strQuery += "     WHERE a.machine_code = b.machine_code ";
                    strQuery += "     and a.model_code = b.model_code";
                    strQuery += "     and a.result_code = b.result_code";
                    strQuery += "     and b.machine_code = c.machine_code";
                    strQuery += "     and b.model_code = c.model_code";
                    strQuery += "     and b.result_code = c.result_code";
                    strQuery += "     and b.roi_code = c.roi_code";
                    strQuery += "     and b.section_code = c.section_code";
                    strQuery += "     and b.work_type = c.work_type";
                    strQuery += "     and c.defect_code = d.defect_code";
                    strQuery += "     and a.defect_yn = 1";
                    strQuery += "     and b.defect_yn = 1";
                    strQuery += "     and d.use_yn = 1";
                    strQuery += String.Format("     and a.machine_code = '{0}' and a.lot_no = '{1}'", Settings.GetSettings().General.MachineCode, strLotNo);
                    strQuery += "     group by d.defect_name ";
                    strQuery += "     union all ";
                    strQuery += "     SELECT defect_name DefectName, ";
                    strQuery += "             0 DefectCount ";
                    strQuery += "     FROM defect_info WHERE use_yn = 1) x  group by x.defectName";

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            LotResultData LotData = new LotResultData();

                            LotData.LotNo = strLotNo;
                            LotData.DefectName = dataReader.GetValue(0).ToString();
                            LotData.DefectCount = Convert.ToInt32(dataReader.GetValue(1).ToString());
                            //LotData.StripDefectCount = Convert.ToInt32(dataReader.GetValue(3).ToString());
                            //LotData.UnitInspectCount = Convert.ToInt32(dataReader.GetValue(4).ToString());
                            //LotData.UnitDefectCount = Convert.ToInt32(dataReader.GetValue(5).ToString());
                            //LotData.TopDefectCount = Convert.ToInt32(dataReader.GetValue(6).ToString());
                            //LotData.BottomDefectCount = Convert.ToInt32(dataReader.GetValue(7).ToString());
                            //LotData.TransDefectCount = Convert.ToInt32(dataReader.GetValue(8).ToString());

                            listData.Add(LotData);
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
                Debug.WriteLine("Exception occured in LoadLotDefectDataDetail(InspectDataManager.cs)");
            }

            return listData;
        }

        public ObservableCollection<LotResultData> LoadLotWorkTypeDefectData(string strLotNo)
        {
            IDataReader dataReader = null;
            ObservableCollection<LotResultData> listData = new ObservableCollection<LotResultData>();

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = " SELECT  x.DefectName,  x.WorkType, sum(x.DefectCount) ";
                    strQuery += "     FROM ";
                    strQuery += "    (SELECT d.defect_name DefectName, b.work_type WorkType, Count(d.defect_code) DefectCount ";
                    strQuery += "     FROM inspect_result a, inspect_result_detail b, defect_result c, defect_info d";
                    strQuery += "     WHERE a.machine_code = b.machine_code ";
                    strQuery += "     and a.model_code = b.model_code";
                    strQuery += "     and a.result_code = b.result_code";
                    strQuery += "     and b.machine_code = c.machine_code";
                    strQuery += "     and b.model_code = c.model_code";
                    strQuery += "     and b.result_code = c.result_code";
                    strQuery += "     and b.roi_code = c.roi_code";
                    strQuery += "     and b.section_code = c.section_code";
                    strQuery += "     and b.work_type = c.work_type";
                    strQuery += "     and c.defect_code = d.defect_code";
                    strQuery += "     and a.defect_yn = 1";
                    strQuery += "     and b.defect_yn = 1";
                    strQuery += "     and d.use_yn = 1";
                    strQuery += String.Format("     and a.machine_code = '{0}' and a.lot_no = '{1}'", Settings.GetSettings().General.MachineCode, strLotNo);
                    strQuery += "     group by d.defect_name ,  b.work_type ";
                    strQuery += "     union all ";
                    strQuery += "     SELECT defect_name DefectName,'' WorkType,  ";
                    strQuery += "             0 DefectCount ";
                    strQuery += "     FROM defect_info WHERE use_yn = 1) x group by x.defectName, x.worktype";

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            LotResultData LotData = new LotResultData();

                            LotData.LotNo = strLotNo;
                            LotData.WorkType = dataReader.GetValue(0).ToString();
                            LotData.DefectName = dataReader.GetValue(1).ToString();
                            LotData.DefectCount = Convert.ToInt32(dataReader.GetValue(2).ToString());
                            //LotData.StripDefectCount = Convert.ToInt32(dataReader.GetValue(3).ToString());
                            //LotData.UnitInspectCount = Convert.ToInt32(dataReader.GetValue(4).ToString());
                            //LotData.UnitDefectCount = Convert.ToInt32(dataReader.GetValue(5).ToString());
                            //LotData.TopDefectCount = Convert.ToInt32(dataReader.GetValue(6).ToString());
                            //LotData.BottomDefectCount = Convert.ToInt32(dataReader.GetValue(7).ToString());
                            //LotData.TransDefectCount = Convert.ToInt32(dataReader.GetValue(8).ToString());

                            listData.Add(LotData);
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
                Debug.WriteLine("Exception occured in LoadLotWorkTypeDefectData(InspectDataManager.cs)");
            }

            return listData;
        }

        public List<LotResultData> LoadLotWorkTypeData(string strLotNo, string strWorkType)
        {
            IDataReader dataReader = null;
            List<LotResultData> listData = new List<LotResultData>();

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = " SELECT  x.DefectName,  x.WorkType, sum(x.DefectCount) ";
                    strQuery += "     FROM ";
                    strQuery += "    (SELECT d.defect_name DefectName, b.work_type WorkType, Count(d.defect_code) DefectCount ";
                    strQuery += "     FROM inspect_result a, inspect_result_detail b, defect_result c, defect_info d";
                    strQuery += "     WHERE a.machine_code = b.machine_code ";
                    strQuery += "     and a.model_code = b.model_code";
                    strQuery += "     and a.result_code = b.result_code";
                    strQuery += "     and b.machine_code = c.machine_code";
                    strQuery += "     and b.model_code = c.model_code";
                    strQuery += "     and b.result_code = c.result_code";
                    strQuery += "     and b.roi_code = c.roi_code";
                    strQuery += "     and b.section_code = c.section_code";
                    strQuery += "     and b.work_type = c.work_type";
                    strQuery += "     and c.defect_code = d.defect_code";
                    strQuery += "     and a.defect_yn = 1";
                    strQuery += "     and b.defect_yn = 1";
                    strQuery += "     and d.use_yn = 1";
                    strQuery += String.Format("     and a.machine_code = '{0}' and a.lot_no = '{1}' and (b.work_type = '{2}' or '{2}' = '*')", Settings.GetSettings().General.MachineCode, strLotNo, strWorkType);
                    strQuery += "     group by d.defect_name ,  b.work_type ";
                    strQuery += "     union all ";
                    strQuery += "     SELECT defect_name DefectName,'' WorkType,  ";
                    strQuery += "             0 DefectCount ";
                    strQuery += "     FROM defect_info WHERE use_yn = 1) x group by x.DefectName, x.WorkType";

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            LotResultData LotData = new LotResultData();

                            LotData.LotNo = strLotNo;
                            LotData.WorkType = dataReader.GetValue(0).ToString();
                            LotData.DefectName = dataReader.GetValue(1).ToString();
                            LotData.DefectCount = Convert.ToInt32(dataReader.GetValue(2).ToString());

                            //LotData.StripInspectCount = Convert.ToInt32(dataReader.GetValue(2).ToString());
                            //LotData.StripDefectCount = Convert.ToInt32(dataReader.GetValue(3).ToString());
                            //LotData.UnitInspectCount = Convert.ToInt32(dataReader.GetValue(4).ToString());
                            //LotData.UnitDefectCount = Convert.ToInt32(dataReader.GetValue(5).ToString());
                            //LotData.TopDefectCount = Convert.ToInt32(dataReader.GetValue(6).ToString());
                            //LotData.BottomDefectCount = Convert.ToInt32(dataReader.GetValue(7).ToString());
                            //LotData.TransDefectCount = Convert.ToInt32(dataReader.GetValue(8).ToString());

                            listData.Add(LotData);
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

            return listData;
        }

        #region Get Strip Count Used in RVS

        public List<NGImageData> LoadNGStripData(DBConnector aDBConnector, string astrLotNo, string EquipCode, string ModelCode)
        {
            if (aDBConnector == null)
                return null;

            IDataReader dataReader = null;
            List<NGImageData> ngImageList = new List<NGImageData>();

            string szSaveImageDisk = Settings.GetSettings().General.ResultPath;
            szSaveImageDisk = szSaveImageDisk.Substring(0, 1);

            try
            {
                string strQuery = "SELECT defect_result.result_code, defect_result.work_type, defect_result.default_image, defect_result.defect_image, defect_info.defect_name, defect_result.result_id,";
                strQuery += " inspect_result.strip_id, defect_result.defect_size, defect_result.defect_posx, defect_result.defect_posy, defect_result.defect_centerx, defect_result.defect_centery";
                strQuery += " FROM bgadb.inspect_result inspect_result, bgadb.inspect_result_detail inspect_result_detail, bgadb.defect_result defect_result, bgadb.defect_info defect_info";
                strQuery += " WHERE inspect_result.verify_yn = 0";
                strQuery += " and inspect_result.defect_yn = 1";
                strQuery += " and inspect_result.closed_yn = 0";
                
                strQuery += " and inspect_result.machine_code = inspect_result_detail.machine_code";
                strQuery += " and inspect_result.machine_code = defect_result.machine_code";
                strQuery += " and inspect_result.model_code = inspect_result_detail.model_code";
                strQuery += " and inspect_result.model_code = defect_result.model_code";
                strQuery += " and inspect_result.result_code = inspect_result_detail.result_code";
                strQuery += " and inspect_result.result_code = defect_result.result_code";

                strQuery += " and inspect_result_detail.section_code = defect_result.section_code";
                strQuery += " and inspect_result_detail.roi_code = defect_result.roi_code";
                strQuery += " and inspect_result_detail.work_type = defect_result.work_type";
                
                strQuery += " and defect_result.defect_code = defect_info.defect_code";
                strQuery += String.Format(" and inspect_result.machine_code = '{0}' and inspect_result.lot_no = '{1}' and inspect_result.model_code='{2}'", EquipCode, astrLotNo, ModelCode);
                strQuery += " GROUP BY defect_result.defect_serial";
                strQuery += " ORDER BY defect_result.defect_image, defect_result.result_id ASC";

                dataReader = aDBConnector.ExecuteQuery19(strQuery);
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        NGImageData ngImageData = new NGImageData();
                        ngImageData.ResultCode = dataReader.GetValue(0).ToString();

                        string surface = dataReader.GetValue(1).ToString();
                        if (surface == "9011") { ngImageData.StripSurface = Surface.BP1; }
                        else if (surface == "9021") { ngImageData.StripSurface = Surface.CA1; }
                        else if (surface == "9022") { ngImageData.StripSurface = Surface.CA2; }
                        else if (surface == "9031") { ngImageData.StripSurface = Surface.BA1; }
                        else if (surface == "9032") { ngImageData.StripSurface = Surface.BA2; }                        

                        string szStandardImagePath = dataReader.GetValue(2).ToString();
                        szStandardImagePath = szSaveImageDisk + szStandardImagePath.Substring(1, szStandardImagePath.Length - 1);
                        ngImageData.StandardImagePath = szStandardImagePath;

                        string szNGImagePath = dataReader.GetValue(3).ToString();
                        szNGImagePath = szSaveImageDisk + szNGImagePath.Substring(1, szNGImagePath.Length - 1);
                        ngImageData.NGImagePath = szNGImagePath;

                        ngImageData.NGName = dataReader.GetValue(4).ToString();
                       // ngImageData.NGType = Common.NGInformationHelper.GetNGEnumName(ngImageData.NGName);
                        ngImageData.ImageIndex = Convert.ToInt32(dataReader.GetValue(5).ToString());
                        ngImageData.StripID = Convert.ToInt32(dataReader.GetValue(6).ToString());
                        ngImageData.DefectSize = dataReader.GetValue(7).ToString();
                        ngImageData.UnitX = Convert.ToInt32(dataReader.GetValue(8).ToString());
                        ngImageData.UnitY = Convert.ToInt32(dataReader.GetValue(9).ToString());
                        ngImageData.DefectCenterX = dataReader.GetValue(10).ToString();
                        ngImageData.DefectCenterY = dataReader.GetValue(11).ToString();                        
                        ngImageList.Add(ngImageData);
                    }
                    dataReader.Close();
                }
            }
            catch
            {
                if (dataReader != null)
                {
                    dataReader.Close();
                }
                Debug.WriteLine("Exception occured in LoadNGStripData(InspectDataManager.cs");
            }

            return ngImageList;
        }

        public int LoadStripInspCount(DBConnector aDBConnector, string strLotNo, string EquipCode, string ModelCode)
        {
            if (aDBConnector == null)
                return -1;

            IDataReader dataReader = null;
            int stripInspCount = 0;

            try
            {
                String strQuery = "SELECT ifnull(sum(strip_inspcount), 0)";
                strQuery += " FROM inspect_result";
                strQuery += String.Format(" WHERE machine_code = '{0}' and lot_no = '{1}' and model_code = '{2}'", EquipCode, strLotNo, ModelCode);

                dataReader = aDBConnector.ExecuteQuery19(strQuery);
                if (dataReader != null)
                {
                    if (dataReader.Read())
                        stripInspCount = Convert.ToInt32(dataReader.GetValue(0).ToString());

                    dataReader.Close();
                }
            }
            catch
            {
                if (dataReader != null)
                    dataReader.Close();
                Debug.WriteLine("Exception occured in LoadStripInspCount(InspectDataManager.cs)");
            }

            return stripInspCount;
        }

        public int LoadStripNGCount(DBConnector aDBConnector, string strLotNo, string EquipCode, string ModelCode)
        {
            if (aDBConnector == null)
                return -1;

            IDataReader dataReader = null;
            int stripNGCount = 0;

            try
            {
                String strQuery = "SELECT ifnull(sum(strip_defectcount),0)";
                strQuery += " FROM inspect_result";
                strQuery += " WHERE defect_yn = 1";
                strQuery += String.Format(" and machine_code = '{0}' and lot_no = '{1}' and model_code = '{2}'", EquipCode, strLotNo, ModelCode);

                dataReader = aDBConnector.ExecuteQuery19(strQuery);
                if (dataReader != null)
                {
                    if (dataReader.Read())
                        stripNGCount = Convert.ToInt32(dataReader.GetValue(0).ToString());

                    dataReader.Close();
                }
            }
            catch
            {
                if (dataReader != null)
                    dataReader.Close();
                Debug.WriteLine("Exception occured in LoadStripNGCount(InspectDataManager)");
            }

            return stripNGCount;
        }

        public int LoadVerifiedGoodStripCount(DBConnector aDBConnector, string strLotNo, string EquipCode, string ModelCode)
        {
            if (aDBConnector == null)
                return -1;

            IDataReader dataReader = null;
            int verifyGoodstripCount = 0;

            try
            {
                String strQuery = "SELECT ifnull(sum(strip_inspcount),0)";
                strQuery += " FROM inspect_result";
                strQuery += " WHERE defect_yn = 0";
                strQuery += " and verify_yn = 1";
                strQuery += String.Format(" and machine_code = '{0}' and lot_no = '{1}' and model_code = '{2}'", EquipCode, strLotNo, ModelCode);

                dataReader = aDBConnector.ExecuteQuery19(strQuery);
                if (dataReader != null)
                {
                    if (dataReader.Read())
                        verifyGoodstripCount = Convert.ToInt32(dataReader.GetValue(0).ToString());

                    dataReader.Close();
                }
            }
            catch
            {
                if (dataReader != null)
                    dataReader.Close();
                Debug.WriteLine("Exception occured in LoadVerifiedGoodStripCount(InspectDataManager.cs)");
            }

            return verifyGoodstripCount;
        }

        public int LoadVerifyStripID(DBConnector aDBConnector, string EquipCode, string ModelCode, string strLotNo)
        {
            if (aDBConnector == null)
                return -1;

            IDataReader dataReader = null;
            int nStripID = 0;

            try
            {
                String strQuery = "SELECT ifnull(strip_id, 0) ";
                strQuery += " FROM inspect_result ";
                strQuery += " WHERE closed_yn = 0 and defect_yn = 1 and verify_yn = 0 and auto_ng = 0 ";
                strQuery += String.Format(" and machine_code = '{0}' and lot_no = '{1}' and model_code = '{2}'", EquipCode, strLotNo, ModelCode);

                dataReader = aDBConnector.ExecuteQuery19(strQuery);
                if (dataReader != null)
                {
                    if (dataReader.Read())
                        nStripID = Convert.ToInt32(dataReader.GetValue(0).ToString());

                    dataReader.Close();
                }
            }
            catch
            {
                if (dataReader != null)
                    dataReader.Close();
                Debug.WriteLine("Exception occured in LoadVerifyStripID(InspectDataManager.cs)");
            }

            return nStripID;
        }

        #endregion

        #region Get Strip Count Used in PCS
        public int LoadStripInspCount(string strLotNo, string EquipCode, string ModelCode)
        {
            IDataReader dataReader = null;
            int stripInspCount = 0;

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = "SELECT COUNT(*) ";
                    strQuery += " FROM inspect_result";
                    strQuery += String.Format(" WHERE machine_code = '{0}' and lot_no = '{1}' and model_code = '{2}'", EquipCode, strLotNo, ModelCode);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                            stripInspCount = Convert.ToInt32(dataReader.GetValue(0).ToString());

                        dataReader.Close();
                    }
                }
            }
            catch
            {
                if (dataReader != null)
                    dataReader.Close();
                Debug.WriteLine("Exception occured in LoadStripInspCount(InspectDataManager.cs)");
            }

            return stripInspCount;
        }

        public int LoadAutoNGCount(string strLotNo, string EquipCode, string ModelCode)
        {
            IDataReader dataReader = null;
            int nAutoNGCount = 0;

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = "SELECT COUNT(*)";
                    strQuery += " FROM inspect_result";
                    strQuery += String.Format(" WHERE machine_code = '{0}' and lot_no = '{1}' and model_code = '{2}' and auto_ng = 1", EquipCode, strLotNo, ModelCode);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                            nAutoNGCount = Convert.ToInt32(dataReader.GetValue(0).ToString());

                        dataReader.Close();
                    }
                }
            }
            catch
            {
                if (dataReader != null)
                    dataReader.Close();
                Debug.WriteLine("Exception occured in LoadStripInspCount(InspectDataManager.cs)");
            }

            return nAutoNGCount;
        }

        public int LoadStripGoodCount(string strLotNo, string EquipCode, string ModelCode)
        {
            IDataReader dataReader = null;
            int stripGoodCount = 0;

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = "SELECT COUNT(*) ";
                    strQuery += " FROM inspect_result";
                    strQuery += " WHERE defect_yn = 0";
                    strQuery += String.Format(" and machine_code = '{0}' and lot_no = '{1}' and model_code = '{2}'", EquipCode, strLotNo, ModelCode);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                            stripGoodCount = Convert.ToInt32(dataReader.GetValue(0).ToString());

                        dataReader.Close();
                    }
                }
            }
            catch
            {
                if (dataReader != null)
                    dataReader.Close();
                Debug.WriteLine("Exception occured in LoadStripGoodCount(InspectDataManager)");
            }

            return stripGoodCount;
        }

        public int LoadStripNGCount(string strLotNo, string EquipCode, string ModelCode)
        {
            IDataReader dataReader = null;
            int stripNGCount = 0;

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = "SELECT COUNT(*) ";
                    strQuery += " FROM inspect_result";
                    strQuery += " WHERE defect_yn = 1";
                    strQuery += String.Format(" and machine_code = '{0}' and lot_no = '{1}' and model_code = '{2}'", EquipCode, strLotNo, ModelCode);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                            stripNGCount = Convert.ToInt32(dataReader.GetValue(0).ToString());

                        dataReader.Close();
                    }
                }
            }
            catch
            {
                if (dataReader != null)
                    dataReader.Close();
                Debug.WriteLine("Exception occured in LoadStripNGCount(InspectDataManager)");
            }

            return stripNGCount;
        }

        public int LoadVerifiedGoodStripCount(string strLotNo, string EquipCode, string ModelCode)
        {
            IDataReader dataReader = null;
            int verifyGoodstripCount = 0;

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = "SELECT ifnull(sum(strip_inspcount),0)";
                    strQuery += " FROM inspect_result";
                    strQuery += " WHERE defect_yn = 0";
                    strQuery += " and verify_yn = 1";
                    strQuery += String.Format(" and machine_code = '{0}' and lot_no = '{1}' and model_code = '{2}'", EquipCode, strLotNo, ModelCode);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                            verifyGoodstripCount = Convert.ToInt32(dataReader.GetValue(0).ToString());

                        dataReader.Close();
                    }
                }
            }
            catch
            {
                if (dataReader != null)
                    dataReader.Close();
                Debug.WriteLine("Exception occured in LoadVerifiedGoodStripCount(InspectDataManager.cs)");
            }

            return verifyGoodstripCount;
        }
        #endregion

        public void LoadWorkTypeData(string strLotNo, string strWorkType, ref int nInspCount, ref int nDefectCount, ref int nUnitInspCount, ref int nUnitDefectCount)
        {
            IDataReader dataReader = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = String.Format("Select ifnull(sum(strip_inspcount),0), ifnull(sum(strip_defectcount),0), ifnull(sum(unit_inspcount),0), ifnull(sum(unit_defectcount),0) " +
                                                 " from lot_work WHERE machine_code = '{0}' and lot_no = '{1}' and (work_type = '{2}' or '{2}' = '*')", Settings.GetSettings().General.MachineCode, strLotNo, strWorkType);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            nInspCount = Convert.ToInt32(dataReader.GetValue(0).ToString());
                            nDefectCount = Convert.ToInt32(dataReader.GetValue(1).ToString());
                            nUnitInspCount = Convert.ToInt32(dataReader.GetValue(2).ToString());
                            nUnitDefectCount = Convert.ToInt32(dataReader.GetValue(3).ToString());
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
                Debug.WriteLine("Exception occured in LoadWorkTypeData(InspectDataManager.cs)");
            }
        }

        public void LoadLotCount(string strLotNo, ref int nGoodCnt, ref int nLoad1Cnt, ref int nLoad2Cnt, ref int nLoad3Cnt, ref int nLoad4Cnt, ref int nLoad5Cnt)
        {
            IDataReader dataReader = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = String.Format("Select ifnull(sum(load1_cnt),0), ifnull(sum(load2_cnt),0),ifnull(sum(load3_cnt),0),ifnull(sum(load4_cnt),0),ifnull(sum(load5_cnt),0) , ifnull(sum(good_cnt), 0)" +
                                                " from lot_info WHERE machine_code = '{0}' and lot_no = '{1}' ", Settings.GetSettings().General.MachineCode, strLotNo);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            nLoad1Cnt = Convert.ToInt32(dataReader.GetValue(0).ToString());
                            nLoad2Cnt = Convert.ToInt32(dataReader.GetValue(1).ToString());
                            nLoad3Cnt = Convert.ToInt32(dataReader.GetValue(2).ToString());
                            nLoad4Cnt = Convert.ToInt32(dataReader.GetValue(3).ToString());
                            nLoad5Cnt = Convert.ToInt32(dataReader.GetValue(4).ToString());

                            nGoodCnt = Convert.ToInt32(dataReader.GetValue(5).ToString());
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
                Debug.WriteLine("Exception occured in LoadLotCount(InspectDataManager.cs)");
            }
        }

        public string GetNGGroupCode(string astrNGGroupName)
        {
            IDataReader dataReader = null;
            string groupCode = "";

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = String.Format("Select com_dcode FROM bgadb.com_detail WHERE com_dname='{0}'", astrNGGroupName);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            groupCode = dataReader.GetValue(0).ToString();
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
                Debug.WriteLine("Exception occured in GetNGGroupCode(InspectDataManager.cs)");
            }

            return groupCode;
        }

        public static bool IsAutoNG(string strDefectCode)
        {
            IDataReader dataReader = null;
            bool bAutoNg = false;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = String.Format("Select ifnull(autong_yn, 0) " +
                                                    " from defect_info WHERE defect_code = '{0}'", strDefectCode);
                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            bAutoNg = Convert.ToInt32(dataReader.GetValue(0).ToString()) == 1 ? true : false;
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
                Debug.WriteLine("Exception occured in IsAutoNG(InspectDataManager.cs)");
            }

            return bAutoNg; // TRUE / FALSE
        }

        public static string GetNGGroupName(int aResultType)
        {
            IDataReader dataReader = null;
            string groupName = "-";

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = String.Format("Select b.com_dname FROM bgadb.defect_info a, bgadb.com_detail b WHERE a.defect_group = b.com_dcode AND a.defect_code='{0}'", QueryHelper.GetCode(aResultType, 4));

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            groupName = dataReader.GetValue(0).ToString();
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
                Debug.WriteLine("Exception occured in GetNGGroupName(InspectDataManager.cs)");
            }

            return groupName;
        }
    }
}
