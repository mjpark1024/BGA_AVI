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
 * @file  ModelManager.cs
 * @brief 
 *  Do Model Create, Modify, Delete.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.08.09
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.09 First creation.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Common.DataBase;
using RMS.Generic.UserManagement;
using System.Diagnostics;
using RVS.Generic;
using Common;
using PCS.ModelTeaching;
using System.Windows.Media;
using System.Xml;

namespace PCS.ELF.AVI
{
    /// <summary>   Values that represent GroupOperation.  </summary>
    /// <remarks>   suoow2, 2014-08-20. </remarks>
    public enum GroupOperation
    {
        NONE = 0,
        Create = 1,
        Modify = 2,
        Delete = 3
    }

    /// <summary>   Values that represent ModelOperation.  </summary>
    /// <remarks>   suoow2, 2014-08-20. </remarks>
    public enum ModelOperation
    {
        NONE = 0,
        Create = 1,
        Modify = 2,
        Delete = 3,
        Copy = 4,
        Paste = 5,
        Rename = 6
    }

    /// <summary>   Manager for models.  </summary>
    /// <remarks>   suoow2, 2014-08-10. </remarks>
    public class ModelManager
    {
        #region Member variables.
        // 현재 선택된 Group.
        public static Group SelectedGroup { get; set; }
        
        // 현재 선택된 Model.
        public static ModelInformation SelectedModel { get; set; }

        public ModelInformation CaptureModel { get; set; }

        public int GroupIndex
        {
            get
            {
                return ++m_nGroupIndex;
            }
            set
            {
                m_nGroupIndex = value;
            }
        }
        private int m_nGroupIndex = 0;

        // 그룹 목록.
        public ObservableCollection<Group> Groups
        {
            get
            {
                return m_listGroup;
            }
        }
        private ObservableCollection<Group> m_listGroup = new ObservableCollection<Group>();

        // 모델 목록.
        public ObservableCollection<ModelInformation> Models
        {
            get
            {
                return m_listModel;
            }
        }
        private ObservableCollection<ModelInformation> m_listModel = new ObservableCollection<ModelInformation>();

        // 히스토리 목록
        public ObservableCollection<GoldenHistory> History
        {
            get
            {
                return m_listHistory;
            }
        }
        private ObservableCollection<GoldenHistory> m_listHistory = new ObservableCollection<GoldenHistory>();

        public ObservableCollection<string> LotNumber
        {
            get
            {
                return m_listLotNumber;
            }
        }
        private ObservableCollection<string> m_listLotNumber = new ObservableCollection<string>();
        #endregion

        #region Result Window Data
        // 기준 이미지
        public List<string> StandardImagePath
        {
            get
            {
                return m_StandardImagePath;
            }
        }
        private List<string> m_StandardImagePath = new List<string>();

        // 불량 이미지
        public List<string> NGImagePath
        {
            get
            {
                return m_NGImagePath;
            }
        }
        private List<string> m_NGImagePath = new List<string>();

        // 불량 이름
        public List<string> NGName
        {
            get
            {
                return m_NGName;
            }
        }
        private List<string> m_NGName = new List<string>();

        public List<NGImageData> m_ImageData = new List<NGImageData>();
        #endregion

        #region Load Group Names & Model names.
        public static string GetGroupName(string aszModelName)
        {
            IDataReader dataReader = null;

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = string.Format("SELECT a.com_dname FROM bgadb.com_detail a, bgadb.model_info b WHERE a.com_dname = b.model_type AND a.use_yn = 1 AND b.use_yn = 1 AND b.model_name='{0}'", aszModelName);
                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            string szGroupName = dataReader.GetValue(0).ToString();
                            dataReader.Close();

                            return szGroupName;
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
            }

            return "";
        }

        public static ObservableCollection<IndexedName> LoadGroupNames(string strIP, string strPort)
        {
            ObservableCollection<IndexedName> listStoredGroupName = new ObservableCollection<IndexedName>();
            String strDBPort = strPort;
            String strCon = String.Format("server={0};user id={1}; password={2}; database=BGADB; port={3}; pooling=false", strIP, "root", "mysql", strDBPort);
            try
            {
                DBConnector Connection = new ADOConnector();
                Connection.Connector(strCon);
                if (Connection.SqlConnection == null)
                {
                }
                else
                {
                    IDataReader dataReader = null;
                    listStoredGroupName.Clear();
                    try
                    {
                        string strQuery = "SELECT com_dname FROM com_detail WHERE com_mcode = '10' AND use_yn = 1 ";
                        dataReader = Connection.ExecuteQuery19(strQuery);

                        if (dataReader != null)
                        {
                            while (dataReader.Read())
                            {
                                IndexedName n = new IndexedName(0, dataReader.GetValue(0).ToString());
                                listStoredGroupName.Add(n);
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
                    }
                }
            }
            catch { }
            return listStoredGroupName;
        }

        public static ObservableCollection<IndexedName> LoadUsedModelNames(string strGroup, string strIP, string strPort)
        {
            ObservableCollection<IndexedName> listStoredModelName = new ObservableCollection<IndexedName>();
            String strDBPort = strPort;
            String strCon = String.Format("server={0};user id={1}; password={2}; database=BGADB; port={3}; pooling=false", strIP, "root", "mysql", strDBPort);
            try
            {
                DBConnector Connection = new ADOConnector();
                Connection.Connector(strCon);
                if (Connection.SqlConnection == null)
                {
                }
                else
                {
                    IDataReader dataReader = null;
                    listStoredModelName.Clear();
                    try
                    {
                        string strQuery = String.Format("SELECT model_code FROM model_info WHERE use_yn = 1 AND model_type='{0}'", strGroup);
                        dataReader = Connection.ExecuteQuery19(strQuery);

                        if (dataReader != null)
                        {
                            while (dataReader.Read())
                            {
                                IndexedName n = new IndexedName(0, dataReader.GetValue(0).ToString());
                                listStoredModelName.Add(n);
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
                    }
                }
            }
            catch { }
            return listStoredModelName;
        }

        public static void LightConversion(string aszMCName, string ip, string port)
        {
            IDataReader dataReader = null;


             String strCon = String.Format("server={0};user id={1}; password={2}; database=BGADB; port={3}; pooling=false", ip, "root", "mysql", port);
             try
             {
                 DBConnector Connection = new ADOConnector();
                 Connection.Connector(strCon);
                 if (Connection.SqlConnection == null)
                 {
                 }
                 else
                 {
                     string strQuery = String.Format("SELECT light_value, model_code  FROM bgadb.model_info");
                     dataReader = Connection.ExecuteQuery19(strQuery);
                    List<string[]> ls = new List<string[]>();
                    try
                    {
                        if (dataReader != null)
                        {
                            while (dataReader.Read())
                            {
                                string szLight = dataReader.GetValue(0).ToString();
                                string code = dataReader.GetValue(1).ToString();
                                ls.Add(new string[] { szLight, code });
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
                    }
                    if(ls.Count > 0)
                    { 
                        foreach(string[] arr in ls)
                        {
                            string szLight = arr[0];
                            string code = arr[1];
                             int[,] tmp = new int[5, 8];
                             for (int i = 0; i < 3; i++)
                             {
                                 for (int j = 0; j < 8; j++)
                                     tmp[i, j] = 0;
                             }
                             #region aaa
                             if (aszMCName == "BAV04" || aszMCName == "BAV19" || aszMCName == "BAV20" || aszMCName == "BAV21" || aszMCName =="BAV22" || aszMCName == "BAV23" ||
                                aszMCName == "BAV24")
                             {
                                 if (szLight.Length >= 120)
                                 {
                                     for (int i = 0; i < 6; i++)
                                         tmp[3, i] = Convert.ToInt32(szLight.Substring((i * 4), 3));
                                     for (int i = 0; i < 6; i++)
                                         tmp[4, i] = Convert.ToInt32(szLight.Substring(((i + 6) * 4), 3));
                                     for (int i = 0; i < 6; i++)
                                         tmp[0, i] = Convert.ToInt32(szLight.Substring(((i + 12) * 4), 3));
                                     for (int i = 0; i < 6; i++)
                                         tmp[1, i] = Convert.ToInt32(szLight.Substring(((i + 18) * 4), 3));
                                     for (int i = 0; i < 6; i++)
                                         tmp[2, i] = Convert.ToInt32(szLight.Substring(((i + 24) * 4), 3));

                                 }
                             }
                             else if (aszMCName == "BAV01")
                             {
                                 if (szLight.Length >= 96)
                                 {
                                     for (int i = 0; i < 6; i++)
                                         tmp[0, i] = Convert.ToInt32(szLight.Substring((i * 4), 3));
                                     for (int i = 0; i < 6; i++)
                                         tmp[1, i] = Convert.ToInt32(szLight.Substring(((i + 6) * 4), 3));
                                     for (int i = 0; i < 6; i++)
                                         tmp[3, i] = Convert.ToInt32(szLight.Substring(((i + 12) * 4), 3));
                                     for (int i = 0; i < 6; i++)
                                         tmp[4, i] = Convert.ToInt32(szLight.Substring(((i + 18) * 4), 3));
                                 }
                             }
                             else if (aszMCName == "BAV03" || aszMCName == "BAV05" || aszMCName == "BAV06" || aszMCName == "BAV11" || aszMCName == "BAV12" || aszMCName == "BAV13")
                             {
                                 if (szLight.Length >= 96)
                                 {
                                     for (int i = 0; i < 8; i++)
                                         tmp[0, i] = Convert.ToInt32(szLight.Substring((i * 4), 3));
                                     for (int i = 0; i < 8; i++)
                                         tmp[1, i] = Convert.ToInt32(szLight.Substring(((i + 8) * 4), 3));
                                     for (int i = 0; i < 8; i++)
                                         tmp[3, i] = Convert.ToInt32(szLight.Substring(((i + 16) * 4), 3));
                                 }
                             }
                             else if (aszMCName == "BAV07" || aszMCName == "BAV08")
                             {
                                 if (szLight.Length >= 120)
                                 {
                                     for (int i = 0; i < 6; i++)
                                         tmp[0, i] = Convert.ToInt32(szLight.Substring((i * 4), 3));
                                     for (int i = 0; i < 6; i++)
                                         tmp[1, i] = Convert.ToInt32(szLight.Substring(((i + 6) * 4), 3));
                                     for (int i = 0; i < 6; i++)
                                         tmp[2, i] = Convert.ToInt32(szLight.Substring(((i + 12) * 4), 3));
                                     for (int i = 0; i < 6; i++)
                                         tmp[3, i] = Convert.ToInt32(szLight.Substring(((i + 18) * 4), 3));
                                     for (int i = 0; i < 6; i++)
                                         tmp[4, i] = Convert.ToInt32(szLight.Substring(((i + 24) * 4), 3));

                                 }
                             }
                             #endregion
                             Connection.StartTrans();
                             #region bbb
                                try
                                {
                                    string szLightValue = "";
                                    for (int i = 0; i < 5; i++)
                                    {
                                        for (int j = 0; j < 8; j++)
                                            szLightValue += tmp[i, j].ToString("D3") + ";";
                                    }
                                    strQuery = string.Format("UPDATE model_info SET light_value = '{0}' WHERE model_code = '{1}'", szLightValue, code);
                                    if (Connection.Execute(strQuery) > 0)
                                    {
                                        Connection.Commit();
                                    }
                                    else // UPDATE model_info fail
                                    {
                                        Connection.Rollback();
                                    }
                                }
                                catch
                                {
                                    if (Connection != null) Connection.Rollback();
                                    Debug.WriteLine("Exception occured in SaveLightValue(DeviceController.cs");
                                }
                                #endregion
                         }
                     }

                 }
             }
             catch
             {

             }   
        }

        public static ModelInformation LoadModel(string strGroupName, string strModel ,string strIP, string strPort, Jobs job)
        {
            String strDBPort = strPort;
            String strCon = String.Format("server={0};user id={1}; password={2}; database=BGADB; port={3}; pooling=false", strIP, "root", "mysql", strDBPort);
            ModelInformation newModel = new ModelInformation();
            try
            {
                DBConnector Connection = new ADOConnector();
                Connection.Connector(strCon);
                if (Connection.SqlConnection == null)
                {
                }
                else
                {
                    IDataReader dataReader = null;

                    try
                    {
                        if (ConnectFactory.DBConnector() != null)
                        {
                            string strQuery = String.Format("SELECT a.model_code, a.model_name, a.model_desc, a.model_type, a.scan_velocity1, a.scan_velocity2, " +
                                                    "a.use_marking, a.marker_code, a.marking_filepath, a.strip_code, b.strip_width, b.strip_height, " +
                                                    "b.unit_row, b.unit_col, b.unit_width, b.unit_height, a.light_value, b.block_num, b.block_gap, " +
                                                    "b.step_units, b.step_pitch, b.mark_step, b.mark_step, b.psr_marginx, b.psr_marginy, b.wp_marginx, b.wp_marginy, a.boatpos1, a.boatpos2, a.camposy, a.alignpos1, a.alignpos2, a.align_rate " +
                                                    "a.inspectmode, a.AutoNG, a.XPitch, a.boat_angle, a.laser_angle, a.insp_option, a.rail_option, a.mark_start_pos, a.umark_start_pos, a.AutoNGX, a.AutoNGY, a.conbad, a.verify, a.IDMark, a.AutoNGBlock, a.scrab_info, b.strip_thickness, b.stip_scancount, a.its_info, a.AutoNGOuterY, a,AutoNGOuterYMode, a.AutoNGOuterDivY, " +
                                                    "a.AutoNGMatrixInfo, b.strip_shift1, b.strip_shift2, a.UseAI" +
                                                    " FROM bgadb.model_info a, bgadb.strip_info b WHERE a.model_type='{0}' AND a.strip_code = b.strip_code AND a.use_yn = 1", strGroupName);
                            dataReader = Connection.ExecuteQuery19(strQuery);
                            if (dataReader != null)
                            {
                                while (dataReader.Read())
                                {
                                    newModel.Index = 0;

                                    newModel.Code = dataReader.GetValue(0).ToString();
                                    newModel.Name = dataReader.GetValue(1).ToString();
                                    newModel.Description = dataReader.GetValue(2).ToString();
                                    newModel.Type.Code = dataReader.GetValue(3).ToString();
                                    newModel.ScanVelocity1 = Convert.ToInt32(dataReader.GetValue(4).ToString());
                                    newModel.ScanVelocity2 = Convert.ToInt32(dataReader.GetValue(5).ToString());
                                    newModel.InspectMode = Convert.ToInt32(dataReader.GetValue(32).ToString());
                                    newModel.AutoNG = Convert.ToInt32(dataReader.GetValue(33).ToString());
                                    newModel.AutoNGBlock = Convert.ToInt32(dataReader.GetValue(46).ToString());
                                    newModel.AutoNGX = Convert.ToInt32(dataReader.GetValue(41).ToString());
                                    newModel.AutoNGY = Convert.ToInt32(dataReader.GetValue(42).ToString());
                                    newModel.UseVerify = (dataReader.GetValue(44).ToString() == "0") ? false : true;
                                    newModel.UseIDMark = (dataReader.GetValue(45).ToString() == "0") ? false : true;
                                    string[] tmpScrab = dataReader.GetValue(47).ToString().Split(',');
                                    if (tmpScrab.Length < 100)
                                    {
                                        for (int i = 0; i < 100; i++) newModel.ScrabInfo[i] = false;
                                        newModel.ScrabInfo[0] = true;
                                        newModel.ScrabInfo[1] = true;
                                        newModel.ScrabInfo[2] = true;
                                        newModel.ScrabInfo[3] = true;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < 100; i++) newModel.ScrabInfo[i] = false;
                                        int min = Math.Min(tmpScrab.Length, newModel.ScrabInfo.Length);
                                        for (int i = 0; i < min; i++) newModel.ScrabInfo[i] = tmpScrab[i] == "0" ? false : true;
                                    }
                                    string[] tmpBad = dataReader.GetValue(43).ToString().Split(',');
                                    if (tmpBad.Length == 2)
                                    {
                                        newModel.UseConBad = (tmpBad[0] == "0") ? false : true;
                                        newModel.ConBad = Convert.ToInt32(tmpBad[1]);
                                    }
                                    newModel.XPitch = Convert.ToDouble(dataReader.GetValue(34).ToString());
                                    newModel.UseMarking = (dataReader.GetValue(6).ToString() == "True") ? true : false;
                                    newModel.Marker.Code = dataReader.GetValue(7).ToString();

                                    double boatangle = -0.07;
                                    double laserangle = 0.25;
                                    if (dataReader.GetValue(35).ToString() != "")
                                    {
                                        boatangle = Convert.ToDouble(dataReader.GetValue(35).ToString());
                                    }
                                    if (dataReader.GetValue(36).ToString() != "")
                                    {
                                        laserangle = Convert.ToDouble(dataReader.GetValue(36).ToString());
                                    }

                                    if (dataReader.GetValue(37).ToString() != "" && dataReader.GetValue(37).ToString().Split(';').Length == 12)
                                    {
                                        string[] tmp = dataReader.GetValue(37).ToString().Split(';');
                                        newModel.UseBondPad1 = (tmp[0] == "0") ? false : true;
                                        newModel.UseBondPad2 = (tmp[1] == "0") ? false : true;
                                        newModel.UseBotSur1 = (tmp[2] == "0") ? false : true;
                                        newModel.UseBotSur2 = (tmp[3] == "0") ? false : true;
                                        newModel.UseBotSur3 = (tmp[4] == "0") ? false : true;
                                        newModel.UseBotSur4 = (tmp[5] == "0") ? false : true;
                                        newModel.UseBotSur5 = (tmp[6] == "0") ? false : true;
                                        newModel.UseTopSur1 = (tmp[7] == "0") ? false : true;
                                        newModel.UseTopSur2 = (tmp[8] == "0") ? false : true;
                                        newModel.UseTopSur3 = (tmp[9] == "0") ? false : true;
                                        newModel.UseTopSur4 = (tmp[10] == "0") ? false : true;
                                        newModel.UseTopSur5 = (tmp[11] == "0") ? false : true;
                                    }
                                    else
                                    {
                                        newModel.UseBondPad1 = true;
                                        newModel.UseBondPad2 = true;
                                        newModel.UseBotSur1 = true;
                                        newModel.UseBotSur2 = true;
                                        newModel.UseBotSur3 = true;
                                        newModel.UseBotSur4 = true;
                                        newModel.UseBotSur5 = true;
                                        newModel.UseTopSur1 = true;
                                        newModel.UseTopSur2 = true;
                                        newModel.UseTopSur3 = true;
                                        newModel.UseTopSur4 = true;
                                        newModel.UseTopSur5 = true;
                                    }
                                    newModel.Marker = RMS.Generic.MarkerInformaion.CreateBoolValueByCode(newModel.Marker.Code, dataReader.GetValue(39).ToString(), dataReader.GetValue(40).ToString(),
                                                                                                         dataReader.GetValue(29).ToString(), dataReader.GetValue(30).ToString(),
                                                                                                         Convert.ToDouble(dataReader.GetValue(26).ToString()), Convert.ToDouble(dataReader.GetValue(27).ToString()),
                                                                                                         Convert.ToDouble(dataReader.GetValue(28).ToString()), Convert.ToInt32(dataReader.GetValue(31).ToString()),
                                                                                                         boatangle, laserangle);
                                    newModel.UseRailOption = (dataReader.GetValue(38).ToString() == "True") ? true : false;
                                    newModel.Strip.Code = dataReader.GetValue(9).ToString();
                                    newModel.Strip.Name = string.Empty;
                                    newModel.Strip.Width = Convert.ToDouble(dataReader.GetValue(10).ToString());
                                    newModel.Strip.Height = Convert.ToDouble(dataReader.GetValue(11).ToString());
                                    newModel.Strip.Thickness = Convert.ToDouble(dataReader.GetValue(48).ToString());
                                    newModel.Strip.UnitRow = Convert.ToInt32(dataReader.GetValue(12).ToString());
                                    newModel.Strip.UnitColumn = Convert.ToInt32(dataReader.GetValue(13).ToString());
                                    newModel.Strip.UnitWidth = Convert.ToDouble(dataReader.GetValue(14).ToString());
                                    newModel.Strip.UnitHeight = Convert.ToDouble(dataReader.GetValue(15).ToString());
                                    newModel.Strip.Block = Convert.ToInt32(dataReader.GetValue(17).ToString());
                                    newModel.Strip.BlockGap = Convert.ToDouble(dataReader.GetValue(18).ToString());
                                    newModel.Strip.StepUnits = Convert.ToInt32(dataReader.GetValue(19).ToString());
                                    newModel.Strip.StepPitch = Convert.ToDouble(dataReader.GetValue(20).ToString());
                                    newModel.Strip.MarkStep = Convert.ToInt32(dataReader.GetValue(21).ToString());
                                    newModel.Strip.PSRMarginX = Convert.ToInt32(dataReader.GetValue(22).ToString());
                                    newModel.Strip.PSRMarginY = Convert.ToInt32(dataReader.GetValue(23).ToString());
                                    newModel.Strip.WPMarginX = Convert.ToInt32(dataReader.GetValue(24).ToString());
                                    newModel.Strip.WPMarginY = Convert.ToInt32(dataReader.GetValue(25).ToString());
                                    newModel.ITS.Set(dataReader.GetValue(49).ToString());
                                    newModel.AutoNGOuterY = Convert.ToInt32(dataReader.GetValue(50).ToString());
                                    newModel.AutoNGOuterYMode = Convert.ToInt32(dataReader.GetValue(51).ToString());
                                    newModel.AutoNGOuterDivY = Convert.ToInt32(dataReader.GetValue(52).ToString());
                                    newModel.AutoNGMatrixInfo = dataReader.GetValue(53).ToString();
                                    newModel.PassingAutoNGMatrixInfo(newModel.AutoNGMatrixInfo);
                                    newModel.Strip.MarkShift1 = Convert.ToDouble(dataReader.GetValue(54).ToString());
                                    newModel.Strip.MarkShift2 = Convert.ToDouble(dataReader.GetValue(55).ToString());
                                    newModel.UseAI = (dataReader.GetValue(56).ToString() == "1") ? true : false;
                         
                                    string szLight = dataReader.GetValue(16).ToString();
                                    if (szLight.Length >= 10 * 32)/////최대 4스캔
                                    {
                                        try
                                        {
                                            for (int i = 0; i < newModel.LightValue.GetLength(0); i++)
                                            {
                                                for (int j = 0; j < newModel.LightValue.GetLength(1); j++)
                                                {
                                                    newModel.LightValue[i, j] = Convert.ToInt32(szLight.Substring((j + (8 * i)) * 4, 3));
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            ;
                                        }
                                        //for (int i = 0; i < 8; i++)
                                        //    newModel.LightValue[0,i] = Convert.ToInt32(szLight.Substring(((i)     * 4), 3));
                                        //for (int i = 0; i < 8; i++)
                                        //    newModel.LightValue[1,i] = Convert.ToInt32(szLight.Substring(((i + 8) * 4), 3));
                                        //for (int i = 0; i < 8; i++)
                                        //    newModel.LightValue[2,i] = Convert.ToInt32(szLight.Substring(((i + 16) * 4), 3));
                                        //for (int i = 0; i < 8; i++)
                                        //    newModel.LightValue[3,i] = Convert.ToInt32(szLight.Substring(((i + 24) * 4), 3));
                                        //for (int i = 0; i < 8; i++)
                                        //    newModel.LightValue[4,i] = Convert.ToInt32(szLight.Substring(((i + 32) * 4), 3));

                                    }
                                    break;
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
            }
            catch { }
            return newModel;
        }

        public static void LoadGroupNames(ref List<string> listStoredGroupName)
        {
            listStoredGroupName.Clear();
            IDataReader dataReader = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = "SELECT com_dname FROM com_detail WHERE com_mcode = '10' AND use_yn = 1 ";
                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            listStoredGroupName.Add(dataReader.GetValue(0).ToString());
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

        // 모델 이름 중복을 피하기 위해 use_yn = 1 인 모델 목록의 이름을 List로 가져온다.
        public static void LoadUsedModelNames(ref List<string> listStoredModelName)
        {
            listStoredModelName.Clear();
            IDataReader dataReader = null;

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = "SELECT model_code FROM model_info WHERE use_yn = 1";
                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            listStoredModelName.Add(dataReader.GetValue(0).ToString());
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

        public static bool SetGroupNameUse(string aszGroupName)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();

                    //String strQuery = String.Format("DELETE FROM strip_info where strip_code = '{0}'", selectedModel.Strip.Code);
                    String strQuery = String.Format("UPDATE bgadb.com_detail SET use_yn = 1 WHERE com_dname = '{0}'", aszGroupName);

                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
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
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in SetGroupNameUse(ModelManager.cs)");
                return false;
            }
            return false;
        }

        // 모델 정보 수정작업인지 생성작업인지 판단을 위해 모델이름을 가져온다.
        public static string CheckModelExist(string aszModelName)
        {
            IDataReader dataReader = null;

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = string.Format("SELECT strip_code FROM model_info WHERE model_code = '{0}'", aszModelName);
                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            string szStripCode = dataReader.GetValue(0).ToString();
                            dataReader.Close();
                            return szStripCode;
                        }
                        dataReader.Close();
                        return string.Empty;
                    }
                }
            }
            catch
            {
                if (dataReader != null)
                {
                    dataReader.Close();
                }
                return string.Empty;
            }
            return string.Empty;
        }
        #endregion

        #region Load Group & Model.
        // 사용 중인 Group 목록을 조회한다.
        public void LoadGroup()
        {
            IDataReader dataReader = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    // com_mcode = '10' : Group
                    String strQuery = "SELECT com_dcode, com_dname FROM com_detail WHERE com_mcode = '10' and use_yn = 1";
                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                    if (dataReader != null)
                    {
                        m_listGroup.Clear();
                        m_nGroupIndex = 0;
                        while (dataReader.Read())
                        {
                            string strCode = dataReader.GetValue(0).ToString();
                            string strName = dataReader.GetValue(1).ToString();

                            m_listGroup.Add(new Group(++m_nGroupIndex, strCode, strName));
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

        public List<string> LoadModels(string strGroup)
        {
            List<string> lstModel = new List<string>();
            IDataReader dataReader = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = String.Format("SELECT model_name FROM bgadb.model_info WHERE model_type='{0}' AND use_yn = 1", strGroup);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        lstModel.Clear();
                        while (dataReader.Read())
                        {
                            ModelInformation newModel = new ModelInformation();

                            string name = dataReader.GetValue(0).ToString();

                            lstModel.Add(name);
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
            return lstModel;
        }

        // Group에 속하는 Model 정보를 조회한다.
        public void LoadModel(string strGroupName)
        {
            IDataReader dataReader = null;

            try
            {
                m_listModel.Clear();
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = String.Format("SELECT a.model_code, a.model_name, a.model_desc, a.model_type, a.scan_velocity1, a.scan_velocity2, " +
                                                    "a.use_marking, a.marker_code, a.marking_filepath, a.strip_code, b.strip_width, b.strip_height, " +
                                                    "b.unit_row, b.unit_col, b.unit_width, b.unit_height, a.light_value, b.block_num, b.block_gap, " +
                                                    "b.step_units, b.step_pitch, b.mark_step, b.psr_marginx, b.psr_marginy, b.wp_marginx, b.wp_marginy, a.boatpos1, a.boatpos2, a.camposy, a.alignpos1, a.alignpos2, a.align_rate, " +
                                                    "a.inspectmode, a.AutoNG, a.XPitch, a.boat_angle, a.laser_angle, a.insp_option, a.rail_option, a.mark_start_pos, a.umark_start_pos, a.AutoNGX, a.AutoNGY, a.conbad, a.verify, a.IDMark, a.AutoNGBlock, a.scrab_info, b.strip_thickness, a.its_info, a.AutoNGOuterY, a.AutoNGOuterYMode, a.AutoNGOuterDivY, " +
                                                    "a.AutoNGMatrixInfo, b.strip_shift1, b.strip_shift2, a.UseAI FROM bgadb.model_info a, bgadb.strip_info b " +
                                                    "WHERE a.model_type='{0}' AND a.strip_code = b.strip_code AND a.use_yn = 1", strGroupName);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        int nIndex = 0;
                        while (dataReader.Read())
                        {
                            //try
                            {
                                ModelInformation newModel = new ModelInformation();

                                newModel.Index = ++nIndex; // sequence : 1, 2, 3, 4, ...

                                newModel.Code = dataReader.GetValue(0).ToString();
                                newModel.Name = dataReader.GetValue(1).ToString();
                                newModel.Description = dataReader.GetValue(2).ToString();
                                newModel.Type.Code = dataReader.GetValue(3).ToString();
                                newModel.ScanVelocity1 = Convert.ToInt32(dataReader.GetValue(4).ToString());
                                newModel.ScanVelocity2 = Convert.ToInt32(dataReader.GetValue(5).ToString());
                                newModel.InspectMode = Convert.ToInt32(dataReader.GetValue(32).ToString());
                                newModel.AutoNG = Convert.ToInt32(dataReader.GetValue(33).ToString());
                                newModel.AutoNGBlock = Convert.ToInt32(dataReader.GetValue(46).ToString());
                                newModel.AutoNGX = Convert.ToInt32(dataReader.GetValue(41).ToString());
                                newModel.AutoNGY = Convert.ToInt32(dataReader.GetValue(42).ToString());
                                newModel.UseVerify = (dataReader.GetValue(44).ToString() == "0") ? false : true;
                                newModel.UseIDMark = (dataReader.GetValue(45).ToString() == "0") ? false : true;
                                string[] tmpScrab = dataReader.GetValue(47).ToString().Split(',');
                                if (tmpScrab.Length < 100)
                                {
                                    for (int i = 0; i < 100; i++) newModel.ScrabInfo[i] = false;
                                    newModel.ScrabInfo[0] = true;
                                    newModel.ScrabInfo[1] = true;
                                    newModel.ScrabInfo[2] = true;
                                    newModel.ScrabInfo[3] = true;
                                }
                                else
                                {
                                    for (int i = 0; i < 100; i++) newModel.ScrabInfo[i] = false;
                                    int min = Math.Min(tmpScrab.Length, newModel.ScrabInfo.Length);
                                    for (int i = 0; i < min; i++) newModel.ScrabInfo[i] = tmpScrab[i] == "0" ? false : true;
                                }
                                string[] tmpBad = dataReader.GetValue(43).ToString().Split(',');
                                if (tmpBad.Length == 2)
                                {
                                    newModel.UseConBad = (tmpBad[0] == "0") ? false : true;
                                    newModel.ConBad = Convert.ToInt32(tmpBad[1]);
                                }
                                newModel.XPitch = Convert.ToDouble(dataReader.GetValue(34).ToString());
                                newModel.UseMarking = (dataReader.GetValue(6).ToString() == "True") ? true : false;
                                newModel.Marker.Code = dataReader.GetValue(7).ToString();

                                double boatangle = -0.07;
                                double laserangle = 0.25;
                                if (dataReader.GetValue(35).ToString() != "")
                                {
                                    boatangle = Convert.ToDouble(dataReader.GetValue(35).ToString());
                                }
                                if (dataReader.GetValue(36).ToString() != "")
                                {
                                    laserangle = Convert.ToDouble(dataReader.GetValue(36).ToString());
                                }
                                if (dataReader.GetValue(37).ToString() != "" && dataReader.GetValue(37).ToString().Split(';').Length == 12)
                                {
                                    string[] tmp = dataReader.GetValue(37).ToString().Split(';');
                                    newModel.UseBondPad1 = (tmp[0] == "0") ? false : true;
                                    newModel.UseBondPad2 = (tmp[1] == "0") ? false : true;
                                    newModel.UseBotSur1 = (tmp[2] == "0") ? false : true;
                                    newModel.UseBotSur2 = (tmp[3] == "0") ? false : true;
                                    newModel.UseBotSur3 = (tmp[4] == "0") ? false : true;
                                    newModel.UseBotSur4 = (tmp[5] == "0") ? false : true;
                                    newModel.UseBotSur5 = (tmp[6] == "0") ? false : true;
                                    newModel.UseTopSur1 = (tmp[7] == "0") ? false : true;
                                    newModel.UseTopSur2 = (tmp[8] == "0") ? false : true;
                                    newModel.UseTopSur3 = (tmp[9] == "0") ? false : true;
                                    newModel.UseTopSur4 = (tmp[10] == "0") ? false : true;
                                    newModel.UseTopSur5 = (tmp[11] == "0") ? false : true;
                                }
                                else if(dataReader.GetValue(37).ToString() != "" && dataReader.GetValue(37).ToString().Split(';').Length == 5)
                                {
                                    string[] tmp = dataReader.GetValue(37).ToString().Split(';');
                                    newModel.UseBondPad1 = (tmp[2] == "0") ? false : true;
                                    newModel.UseBondPad2 = false;
                                    newModel.UseBotSur1 = (tmp[0] == "0") ? false : true;
                                    newModel.UseBotSur2 = (tmp[1] == "0") ? false : true;
                                    newModel.UseBotSur3 = false;
                                    newModel.UseBotSur4 = false;
                                    newModel.UseBotSur5 = false;
                                    newModel.UseTopSur1 = (tmp[3] == "0") ? false : true;
                                    newModel.UseTopSur2 = (tmp[4] == "0") ? false : true;
                                    newModel.UseTopSur3 = false;
                                    newModel.UseTopSur4 = false;
                                    newModel.UseTopSur5 = false;
                                }
                                else if (dataReader.GetValue(37).ToString() != "" && dataReader.GetValue(37).ToString().Split(';').Length == 6)
                                {
                                    string[] tmp = dataReader.GetValue(37).ToString().Split(';');
                                    newModel.UseBondPad1 = (tmp[2] == "0") ? false : true;
                                    newModel.UseBondPad2 = false;
                                    newModel.UseBotSur1 = (tmp[0] == "0") ? false : true;
                                    newModel.UseBotSur2 = (tmp[1] == "0") ? false : true;
                                    newModel.UseBotSur3 = false;
                                    newModel.UseBotSur4 = false;
                                    newModel.UseBotSur5 = false;
                                    newModel.UseTopSur1 = (tmp[3] == "0") ? false : true;
                                    newModel.UseTopSur2 = (tmp[4] == "0") ? false : true;
                                    newModel.UseTopSur3 = false;
                                    newModel.UseTopSur4 = false;
                                    newModel.UseTopSur5 = false;
                                }
                                newModel.Marker = RMS.Generic.MarkerInformaion.CreateBoolValueByCode(newModel.Marker.Code, dataReader.GetValue(39).ToString(), dataReader.GetValue(40).ToString(),
                                                                                                     dataReader.GetValue(29).ToString(), dataReader.GetValue(30).ToString(),
                                                                                                     Convert.ToDouble(dataReader.GetValue(26).ToString()), Convert.ToDouble(dataReader.GetValue(27).ToString()),
                                                                                                     Convert.ToDouble(dataReader.GetValue(28).ToString()), Convert.ToInt32(dataReader.GetValue(31).ToString()),
                                                                                                     boatangle, laserangle);

                                newModel.UseRailOption = (dataReader.GetValue(38).ToString() == "True") ? true : false;

                                newModel.Strip.Code = dataReader.GetValue(9).ToString();
                                newModel.Strip.Name = string.Empty;
                                newModel.Strip.Width = Convert.ToDouble(dataReader.GetValue(10).ToString());
                                newModel.Strip.Height = Convert.ToDouble(dataReader.GetValue(11).ToString());
                                if (dataReader.GetValue(48).ToString() == "") newModel.Strip.Thickness = 0.2;
                                else newModel.Strip.Thickness = Convert.ToDouble(dataReader.GetValue(48).ToString());
                                newModel.Strip.UnitRow = Convert.ToInt32(dataReader.GetValue(12).ToString());
                                newModel.Strip.UnitColumn = Convert.ToInt32(dataReader.GetValue(13).ToString());
                                newModel.Strip.UnitWidth = Convert.ToDouble(dataReader.GetValue(14).ToString());
                                newModel.Strip.UnitHeight = Convert.ToDouble(dataReader.GetValue(15).ToString());
                                newModel.Strip.Block = Convert.ToInt32(dataReader.GetValue(17).ToString());
                                newModel.Strip.BlockGap = Convert.ToDouble(dataReader.GetValue(18).ToString());
                                newModel.Strip.StepUnits = Convert.ToInt32(dataReader.GetValue(19).ToString());
                                newModel.Strip.StepPitch = Convert.ToDouble(dataReader.GetValue(20).ToString());
                                newModel.Strip.MarkStep = Convert.ToInt32(dataReader.GetValue(21).ToString());
                                newModel.Strip.PSRMarginX = Convert.ToInt32(dataReader.GetValue(22).ToString());
                                newModel.Strip.PSRMarginY = Convert.ToInt32(dataReader.GetValue(23).ToString());
                                newModel.Strip.WPMarginX = Convert.ToInt32(dataReader.GetValue(24).ToString());
                                newModel.Strip.WPMarginY = Convert.ToInt32(dataReader.GetValue(25).ToString());
                                newModel.ITS.Set(dataReader.GetValue(49).ToString());
                                newModel.AutoNGOuterY = Convert.ToInt32(dataReader.GetValue(50).ToString());
                                newModel.AutoNGOuterYMode = Convert.ToInt32(dataReader.GetValue(51).ToString());
                                newModel.AutoNGOuterDivY = Convert.ToInt32(dataReader.GetValue(52).ToString());
                                newModel.AutoNGMatrixInfo = dataReader.GetValue(53).ToString();
                                newModel.PassingAutoNGMatrixInfo(newModel.AutoNGMatrixInfo);
                                newModel.Strip.MarkShift1 = Convert.ToDouble(dataReader.GetValue(54).ToString());
                                newModel.Strip.MarkShift2 = Convert.ToDouble(dataReader.GetValue(55).ToString());
                                newModel.UseAI = (dataReader.GetValue(56).ToString() == "1") ? true : false;
                                string szLight = dataReader.GetValue(16).ToString();
                                if (szLight.Length >= newModel.LightValue.GetLength(0) * newModel.LightValue.GetLength(1) * 4)/////최대 4스캔 10개 조명 파라미터 BP 2, BA 4, CA 4
                                {
                                    try
                                    {
                                        for (int i = 0; i < newModel.LightValue.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < newModel.LightValue.GetLength(1); j++)
                                                newModel.LightValue[i, j] = Convert.ToInt32(szLight.Substring((j + (8 * i)) * 4, 3));
                                        }
                                    }
                                    catch
                                    {
                                        ;
                                    }
                                }
                                else if (szLight.Length >= 5 * newModel.LightValue.GetLength(1) * 4)  ////////////////////////   설비 DB업데이트 후 지울것
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        for (int j = 0; j < newModel.LightValue.GetLength(1); j++)
                                        {
                                            if (i == 0) newModel.LightValue[0, j] = Convert.ToInt32(szLight.Substring((j + (8 * i)) * 4, 3));
                                            else if (i == 1) newModel.LightValue[2, j] = Convert.ToInt32(szLight.Substring((j + (8 * i)) * 4, 3));
                                            else if (i == 2) newModel.LightValue[3, j] = Convert.ToInt32(szLight.Substring((j + (8 * i)) * 4, 3));
                                            else if (i == 3) newModel.LightValue[6, j] = Convert.ToInt32(szLight.Substring((j + (8 * i)) * 4, 3));
                                            else if (i == 4) newModel.LightValue[7, j] = Convert.ToInt32(szLight.Substring((j + (8 * i)) * 4, 3));
                                        }
                                    }
                                }
                                m_listModel.Add(newModel);
                            }
                        //    catch
                            {
                                ;
                            }
                        }
                        dataReader.Close();
                    }
                    ////////////////////////   설비 DB업데이트 후 지울것
                    
                    for (int i = 0; i < m_listModel.Count; i++)
                    {
                        ModelInformation newModel = new ModelInformation();
                        newModel.Code = m_listModel[i].Name;
                        string tmp = Convert.ToInt32(m_listModel[i].UseBondPad1).ToString() + ";" +
                                     Convert.ToInt32(m_listModel[i].UseBondPad2).ToString() + ";" +
                                     Convert.ToInt32(m_listModel[i].UseBotSur1).ToString() + ";" +
                                     Convert.ToInt32(m_listModel[i].UseBotSur2).ToString() + ";" +
                                     Convert.ToInt32(m_listModel[i].UseBotSur3).ToString() + ";" +
                                     Convert.ToInt32(m_listModel[i].UseBotSur4).ToString() + ";" +
                                     Convert.ToInt32(m_listModel[i].UseBotSur5).ToString() + ";" +
                                     Convert.ToInt32(m_listModel[i].UseTopSur1).ToString() + ";" +
                                     Convert.ToInt32(m_listModel[i].UseTopSur2).ToString() + ";" +
                                     Convert.ToInt32(m_listModel[i].UseTopSur3).ToString() + ";" +
                                     Convert.ToInt32(m_listModel[i].UseTopSur4).ToString() + ";" +
                                     Convert.ToInt32(m_listModel[i].UseTopSur5).ToString();
                        if (!string.IsNullOrEmpty(newModel.Code))
                        {
                            if (ConnectFactory.DBConnector() != null)
                            {
                                ConnectFactory.DBConnector().StartTrans();
                                try
                                {
                                    strQuery = string.Format("UPDATE `bgadb`.`model_info` SET `insp_option` = '{0}' WHERE model_code = '{1}'", tmp, newModel.Code);
                                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                                    {
                                        ConnectFactory.DBConnector().Commit();
                                    }
                                    else // UPDATE model_info fail
                                    {
                                        ConnectFactory.DBConnector().Rollback();
                                    }
                                }
                                catch
                                {
                                    if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                                    Debug.WriteLine("Exception occured in SaveLightValue(DeviceController.cs");
                                }
                            }
                        }
                        newModel.LightValue = m_listModel[i].LightValue;
                        DeviceController.SaveLightValue(newModel.Code, newModel.LightValue);
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



        // Model과 관련된 Lot 번호를 조회한다.
        public void LoadLotNumber(string strModelCode)
        {
            IDataReader dataReader = null;

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = String.Format("SELECT lot_no FROM bgadb.lot_info " +
                                                    "WHERE machine_code='{0}' AND model_code='{1}'", Common.Settings.GetSettings().General.MachineCode, strModelCode);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                    if (dataReader != null)
                    {
                        m_listLotNumber.Clear();

                        while (dataReader.Read())
                        {
                            m_listLotNumber.Add(dataReader.GetValue(0).ToString());
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

        public void LoadImagePathNG(string strModelCode, string strWorkType, string strLotNo, string strDefectName)
        {
            IDataReader dataReader = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = String.Format("SELECT c.defect_image, d.defect_name " +
                        "FROM inspect_result a, inspect_result_detail b, defect_result c, defect_info d " +
                        "WHERE a.machine_code = b.machine_code " +
                        "AND a.machine_code = c.machine_code " +
                        "AND a.result_code = b.result_code " +
                        "AND a.result_code = c.result_code " +
                        "AND b.work_type = c.work_type " +
                        "AND b.section_code = c.section_code " +
                        "AND b.roi_code = c.roi_code " +
                        "AND c.defect_code = d.defect_code " +
                        "AND a.model_code = '{0}' " +
                        "AND a.lot_no = '{1}' " +
                        "AND (b.work_type = '{2}' or '{2}' = '*') " +
                        "AND (d.defect_name = '{3}' or '{3}' = '*')" +
                        "AND a.machine_code = '{4}'", strModelCode, strLotNo, strWorkType, strDefectName, Common.Settings.GetSettings().General.MachineCode);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        m_StandardImagePath.Clear();
                        m_NGImagePath.Clear();
                        m_NGName.Clear();

                        while (dataReader.Read())
                        {
                            m_NGImagePath.Add(dataReader.GetValue(0).ToString());
                            m_NGName.Add(dataReader.GetValue(1).ToString());
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

        public void LoadImagePathAll(string strModelCode, string strWorkType, string strLotNo, string strDefectGroup)
        {
            IDataReader dataReader = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = String.Format("SELECT c.default_image, c.defect_image, c.work_type, c.result_id, d.defect_name, c.defect_posx, c.defect_posy, c.defect_size, c.defect_centerx, c.defect_centery, a.strip_id " +
                        "FROM inspect_result a, inspect_result_detail b, defect_result c, defect_info d " +
                        "WHERE a.machine_code = b.machine_code " +
                        "AND a.machine_code = c.machine_code " +
                        "AND a.result_code = b.result_code " +
                        "AND a.result_code = c.result_code " +
                        "AND a.closed_yn = 1 " +
                        "AND b.work_type = c.work_type " +
                        "AND b.section_code = c.section_code " +
                        "AND a.model_code = b.model_code AND b.model_code = c.model_code " +
                        "AND b.roi_code = c.roi_code " +
                        "AND c.defect_code = d.defect_code " +
                        "AND a.model_code = '{0}' " +
                        "AND a.lot_no = '{1}' " +
                        "AND (b.work_type = '{2}' or '{2}' = '*') " +
                        "AND (d.defect_group = '{3}' or '{3}' = '*') " +
                        "AND a.machine_code = '{4}' ORDER BY c.defect_image, c.result_id ASC", strModelCode, strLotNo, strWorkType, strDefectGroup, Common.Settings.GetSettings().General.MachineCode);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        m_ImageData.Clear();

                        while (dataReader.Read())
                        {
                            NGImageData ngData = new NGImageData();
                            ngData.StandardImagePath = dataReader.GetValue(0).ToString();
                            ngData.NGImagePath = dataReader.GetValue(1).ToString();
                            string surface = dataReader.GetValue(2).ToString();
                            if (surface == WorkTypeCode.WORK_BOTSUR1)
                            {
                                ngData.StripSurface = Surface.BA1;
                            }
                            else if (surface == WorkTypeCode.WORK_BOTSUR2)
                            {
                                ngData.StripSurface = Surface.BA2;
                            }
                            else if (surface == WorkTypeCode.WORK_BONDPAD1)
                            {
                                ngData.StripSurface = Surface.BP1;
                            }
                            else if(surface == WorkTypeCode.WORK_BONDPAD2)
                            {
                                ngData.StripSurface = Surface.BP2;
                            }
                            else if (surface == WorkTypeCode.WORK_TOPSUR1)
                            {
                                ngData.StripSurface = Surface.CA1;
                            }
                            else if (surface == WorkTypeCode.WORK_TOPSUR2)
                            {
                                ngData.StripSurface = Surface.CA2;
                            }

                            ngData.ImageIndex = Convert.ToInt32(dataReader.GetValue(3).ToString());
                            ngData.NGName = dataReader.GetValue(4).ToString();
                            ngData.UnitX = Convert.ToInt32(dataReader.GetValue(5).ToString());
                            ngData.UnitY = Convert.ToInt32(dataReader.GetValue(6).ToString());
                            ngData.DefectSize = dataReader.GetValue(7).ToString();
                            ngData.DefectCenterX = dataReader.GetValue(8).ToString();
                            ngData.DefectCenterY = dataReader.GetValue(9).ToString();
                            ngData.StripID = Convert.ToInt32(dataReader.GetValue(10).ToString());

                            m_ImageData.Add(ngData);
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
        #endregion

        #region CRUD group.
        public bool CreateGroup(string strGroupName)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();

                    string strGroupCode = GetMaxGroupCode();

                    string strQuery = "INSERT INTO com_detail(com_dcode, com_dname, com_desc, use_yn, reg_date, com_mcode, user_id) ";
                    strQuery += String.Format("VALUES('{0}', '{1}', 'model group', 1,  now(), '10', '{2}')", strGroupCode, strGroupName, UserManager.CurrentUser.ID);

                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                    {
                        ConnectFactory.DBConnector().Commit();

                        m_listGroup.Add(new Group(m_listGroup.Count + 1, strGroupCode, strGroupName));
                        return true;
                    }
                    else
                    {
                        ConnectFactory.DBConnector().Rollback();
                        return false;
                    }
                }
                else // DBConnector is null
                {
                    return false;
                }
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in CreateGroup(ModelManager.cs");
                return false;
            }
        }

        public bool ModifyGroup(int nIndex, string strNewGroupName)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();

                    Group selectedGroup = m_listGroup[nIndex - 1];

                    String strQuery = String.Format("UPDATE com_detail SET com_dname = '{0}', reg_date = now(), user_id = '{1}' WHERE com_dcode = '{2}'", strNewGroupName, UserManager.CurrentUser.ID, selectedGroup.Code);

                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                    {
                        ConnectFactory.DBConnector().Commit();
                        m_listGroup[nIndex - 1].Name = strNewGroupName;

                        return true;
                    }
                    else
                    {
                        ConnectFactory.DBConnector().Rollback();

                        return false;
                    }
                }
                else // DBConnector is null
                {
                    return false;
                }
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in ModifyGroup(ModelManager.cs");
                return false;
            }
        }

        public bool RenameGroup(string srcName, string newName)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();

                    String strQuery = String.Format("UPDATE model_info SET model_type = '{0}' WHERE model_type = '{1}'", newName, srcName);

                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
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
                else // DBConnector is null
                {
                    return false;
                }
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in ModifyGroup(ModelManager.cs");
                return false;
            }
        }

        public bool DeleteGroup(int nIndex)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    Group selectedGroup = m_listGroup[nIndex - 1];

                    // 그룹 내 포함된 모델을 삭제하여 준다.
                    // 그룹 내 일부 모델 삭제 실패시 false 반환.
                    int nCount = m_listModel.Count;
                    for (int i = 0; i < nCount; i++)
                    {
                        if (m_listModel[i].Type.Code == selectedGroup.Name)
                        {
                            if (DeleteModel(m_listModel[i]) == false)
                            {
                                return false;
                            }

                            i--;
                            nCount--;
                        }
                    }
                    ConnectFactory.DBConnector().StartTrans();

                    //String strQuery = String.Format("DELETE FROM com_detail where com_dcode = '{0}'", selectedGroup.Code);
                    String strQuery = String.Format("UPDATE com_detail SET use_yn = 0 WHERE com_dcode = '{0}'", selectedGroup.Code);

                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                    {
                        ConnectFactory.DBConnector().Commit();

                        m_listGroup.RemoveAt(nIndex - 1);

                        GroupIndex = 0;
                        foreach (Group group in Groups)
                        {
                            group.Index = GroupIndex;
                        }

                        return true;
                    }
                    else
                    {
                        ConnectFactory.DBConnector().Rollback();

                        return false;
                    }
                }
                else // DBConnector is null
                {
                    return false;
                }
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in DeleteGroup(ModelManager.cs");
                return false;
            }
        }
        #endregion

        #region CRUD model.
        public bool CreateModel(ModelInformation newModel)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();

                    // Step 1: insert into strip_info
                    newModel.Strip.Code = GetMaxStripCode();
                    String strQuery = "INSERT INTO strip_info(strip_code, strip_name, strip_width, strip_height, " +
                                      "unit_row, unit_col, unit_width, unit_height, use_yn, " +
                                      "user_id, reg_date, send_yn, block_num, block_gap, step_units, step_pitch, mark_step, psr_marginx, psr_marginy, wp_marginx, wp_marginy, strip_thickness, strip_shift1, strip_shift2) ";
                    strQuery += String.Format("VALUES('{0}', '{1}', {2:f}, {3:f}, {4:D}, {5:D}, {6:f}, {7:f}, 1, '{8}', now(), 0, {9:D}, {10:f}, {11:D}, {12:f}, {13:D},{14},{15},{16},{17},{18},{19:f},{20:f}) ",
                                            newModel.Strip.Code, newModel.Strip.Name, newModel.Strip.Width, newModel.Strip.Height,
                                            newModel.Strip.UnitRow, newModel.Strip.UnitColumn, newModel.Strip.UnitWidth, newModel.Strip.UnitHeight,
                                            UserManager.CurrentUser.ID, newModel.Strip.Block, newModel.Strip.BlockGap, newModel.Strip.StepUnits, newModel.Strip.StepPitch, newModel.Strip.MarkStep,
                                            newModel.Strip.PSRMarginX, newModel.Strip.PSRMarginY, newModel.Strip.WPMarginX, newModel.Strip.WPMarginY, newModel.Strip.Thickness, newModel.Strip.MarkShift1, newModel.Strip.MarkShift2);

                    // Step 2: insert into model_info
                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                    {
                        newModel.Code = newModel.Name;
                        newModel.Marker.Code = RMS.Generic.MarkerInformaion.CreateCodeByCheckbox(newModel.Marker);
                        //newModel.MarkingFilePath.Replace(@"\", "/");
                        string tmpConBad = (newModel.UseConBad ? "1" : "0") + "," + newModel.ConBad.ToString();
                        string tmp = Convert.ToInt32(newModel.UseBondPad1).ToString() + ";" +
                                     Convert.ToInt32(newModel.UseBondPad2).ToString() + ";" +
                                     Convert.ToInt32(newModel.UseBotSur1).ToString() + ";" +
                                     Convert.ToInt32(newModel.UseBotSur2).ToString() + ";" +
                                     Convert.ToInt32(newModel.UseBotSur3).ToString() + ";" +
                                     Convert.ToInt32(newModel.UseBotSur4).ToString() + ";" +
                                     Convert.ToInt32(newModel.UseBotSur5).ToString() + ";" +
                                     Convert.ToInt32(newModel.UseTopSur1).ToString() + ";" +
                                     Convert.ToInt32(newModel.UseTopSur2).ToString() + ";" +
                                     Convert.ToInt32(newModel.UseTopSur3).ToString() + ";" +
                                     Convert.ToInt32(newModel.UseTopSur4).ToString() + ";" +
                                     Convert.ToInt32(newModel.UseTopSur5).ToString();
                        string tmpScrab = "";
                        for (int i = 0; i < newModel.ScrabInfo.Length; i++)
                        {
                            tmpScrab += (newModel.ScrabInfo[i] ? "1" : "0") + ",";
                        }
                        strQuery = "INSERT INTO model_info(model_code, model_name, model_type, model_desc, strip_code, marker_code, " +
                                                          "default_reject_code, scan_velocity1, scan_velocity2, use_marking, marking_filepath, " +
                                                          "send_yn, use_yn, reg_date,  user_id, inspectmode, AutoNG, AutoNGX, AutoNGY, XPitch, rail_option, " +
                                                          "marking_filepath2, marking_filepath3, conbad, verify, IDMark, AutoNGBlock, insp_option, " +
                                                          "mark_start_pos, umark_start_pos, scrab_info, its_info, AutoNGOuterY, AutoNGOuterYMode, AutoNGOuterDivY, AutoNGMatrixInfo, UseAI) ";
                        strQuery += String.Format("VALUES('{0}', '{1}', '{2}','', '{3}', '{4}', '0000', {5}, {6}, {7}, '{8}', 0, 1, now(), '{9}', " +
                                                  "{10}, {11}, {12}, {13}, {14}, {15}, '', '', '{16}', {17}, {18}, {19}, '{20}', '{21}', '{22}', '{23}', '{24}', {25}, {26}, {27}, '{28}', {29})",
                                        newModel.Code, // 0
                                        newModel.Name, // 1
                                        newModel.Type.Code, // 2
                                        newModel.Strip.Code, // 3
                                        newModel.Marker.Code, // 4
                                        newModel.ScanVelocity1, // 5
                                        newModel.ScanVelocity2, // 6
                                        newModel.UseMarking, // 7
                                        "", // 8
                                        UserManager.CurrentUser.ID,
                                        newModel.InspectMode,
                                        newModel.AutoNG, newModel.AutoNGX, newModel.AutoNGY, newModel.XPitch, false, tmpConBad
                                        , Convert.ToInt32(newModel.UseVerify), Convert.ToInt32(newModel.UseIDMark), newModel.AutoNGBlock,
                                        tmp,
                                        newModel.Marker.MarkStartPosX.ToString() + "," + newModel.Marker.MarkStartPosY.ToString(),
                                        newModel.Marker.UMarkStartPosX.ToString() + "," + newModel.Marker.UMarkStartPosY.ToString(), tmpScrab, newModel.ITS.ITSInfoString(),
                                        newModel.AutoNGOuterY, newModel.AutoNGOuterYMode, newModel.AutoNGOuterDivY, newModel.AutoNGMatrixInfo, newModel.UseAI); // 13

                        if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                        {
                            ConnectFactory.DBConnector().Commit();
                            DeviceController.SaveLightValue(newModel.Code, newModel.LightValue); // 기록된 조명 값을 반영한다.

                            newModel.Index = m_listModel.Count + 1;
                            m_listModel.Add(newModel);
                            return true;
                        }
                        else // INSERT INTO model_info fail
                        {
                            ConnectFactory.DBConnector().Rollback();
                            return false;
                        }
                    }
                    else // INSERT INTO strip_info fail
                    {
                        ConnectFactory.DBConnector().Rollback();
                        return false;
                    }
                }
                else // DBConnector is null
                {
                    return false;
                }
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in CreateModel(ModelManager.cs)");
                return false;
            }
        }

        public bool ModifyModel(ModelInformation modifiedModel, int nModelIndex = -1)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();

                    String strQuery = String.Format("UPDATE strip_info SET strip_name='{0}', strip_width = {1:f}, strip_height = {2:f}, " +
                                                    "unit_row = {3:d}, unit_col = {4:d}, unit_width = {5:f}, unit_height = {6:f}, " +
                                                    "block_num ={7:d}, block_gap={8:f}, step_units={9}, step_pitch={10:f}, mark_step={11}, " +
                                                    "psr_marginx={12}, psr_marginy={13}, wp_marginx={14}, wp_marginy={15}, strip_thickness={18}, strip_shift1={19:f}, strip_shift2={20:f}, " +
                                                    "user_id = '{16}', reg_date = now(), use_yn = 1 " +
                                                    "WHERE strip_code = '{17}'",
                                                    modifiedModel.Strip.Name, modifiedModel.Strip.Width, modifiedModel.Strip.Height,
                                                    modifiedModel.Strip.UnitRow, modifiedModel.Strip.UnitColumn, modifiedModel.Strip.UnitWidth, modifiedModel.Strip.UnitHeight,
                                                    modifiedModel.Strip.Block, modifiedModel.Strip.BlockGap, modifiedModel.Strip.StepUnits, modifiedModel.Strip.StepPitch, modifiedModel.Strip.MarkStep,
                                                    modifiedModel.Strip.PSRMarginX, modifiedModel.Strip.PSRMarginY, modifiedModel.Strip.WPMarginX, modifiedModel.Strip.WPMarginY,
                                                    UserManager.CurrentUser.ID, modifiedModel.Strip.Code, modifiedModel.Strip.Thickness, modifiedModel.Strip.MarkShift1, modifiedModel.Strip.MarkShift2);

                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                    {
                        modifiedModel.Marker.Code = RMS.Generic.MarkerInformaion.CreateCodeByCheckbox(modifiedModel.Marker);

                        string tmp = Convert.ToInt32(modifiedModel.UseBondPad1).ToString() + ";" +
                                     Convert.ToInt32(modifiedModel.UseBondPad2).ToString() + ";" +
                                     Convert.ToInt32(modifiedModel.UseBotSur1).ToString() + ";" +
                                     Convert.ToInt32(modifiedModel.UseBotSur2).ToString() + ";" +
                                     Convert.ToInt32(modifiedModel.UseBotSur3).ToString() + ";" +
                                     Convert.ToInt32(modifiedModel.UseBotSur4).ToString() + ";" +
                                     Convert.ToInt32(modifiedModel.UseBotSur5).ToString() + ";" +
                                     Convert.ToInt32(modifiedModel.UseTopSur1).ToString() + ";" +
                                     Convert.ToInt32(modifiedModel.UseTopSur2).ToString() + ";" +
                                     Convert.ToInt32(modifiedModel.UseTopSur3).ToString() + ";" +
                                     Convert.ToInt32(modifiedModel.UseTopSur4).ToString() + ";" +
                                     Convert.ToInt32(modifiedModel.UseTopSur5).ToString();

                        string tmpConBad = (modifiedModel.UseConBad ? "1" : "0") + "," + modifiedModel.ConBad.ToString();
                        string tmpScrab = "";
                        for (int i = 0; i < modifiedModel.ScrabInfo.Length; i++)
                        {
                            tmpScrab += (modifiedModel.ScrabInfo[i] ? "1" : "0") + ",";
                        }
                        strQuery = String.Format("UPDATE model_info SET model_name='{0}', model_desc = '{1}', scan_velocity1 = {2}, scan_velocity2 = {3}, marker_code = '{4}', " +
                                                 "use_marking = {5}, marking_filepath='{6}', inspectmode = {7}, AutoNG = {10}, AutoNGX = {16}, AutoNGY = {17}, user_id = '{8}', reg_date = now(), use_yn = 1, XPitch = {11}, insp_option = '{12}', " +
                                                 "rail_option = {13}, mark_start_pos='{14}', umark_start_pos='{15}', conbad='{18}', verify={19} , IDMark={20}, AutoNGBlock={21}, scrab_info='{22}', its_info='{23}', AutoNGOuterY={24}, AutoNGOuterYMode={25}, AutoNGOuterDivY={26}, " +
                                                 "AutoNGMatrixInfo = '{27}', UseAI = {28} WHERE model_code = '{9}'",
                                                 modifiedModel.Name, // 0
                                                 modifiedModel.Description, // 1
                                                 modifiedModel.ScanVelocity1, // 2
                                                 modifiedModel.ScanVelocity2, // 3
                                                 modifiedModel.Marker.Code, // 4
                                                 modifiedModel.UseMarking, // 5
                                                 "", // 6
                                                 modifiedModel.InspectMode, // 7
                                                 UserManager.CurrentUser.ID, // 11
                                                 modifiedModel.Code,
                                                 modifiedModel.AutoNG, modifiedModel.XPitch, tmp,
                                                 modifiedModel.UseRailOption,
                                                 modifiedModel.Marker.MarkStartPosX.ToString() + "," + modifiedModel.Marker.MarkStartPosY.ToString(),
                                                 modifiedModel.Marker.UMarkStartPosX.ToString() + "," + modifiedModel.Marker.UMarkStartPosY.ToString(),
                                                 modifiedModel.AutoNGX, modifiedModel.AutoNGY, tmpConBad
                                                 , Convert.ToInt32(modifiedModel.UseVerify), Convert.ToInt32(modifiedModel.UseIDMark), modifiedModel.AutoNGBlock, tmpScrab, modifiedModel.ITS.ITSInfoString(), modifiedModel.AutoNGOuterY, modifiedModel.AutoNGOuterDivY, modifiedModel.AutoNGOuterYMode, modifiedModel.AutoNGMatrixInfo
                                                 , modifiedModel.UseAI);//12

                        if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                        {
                            ConnectFactory.DBConnector().Commit();

                            if (nModelIndex < 0)
                            {
                                modifiedModel.Index = m_listModel.Count + 1;
                                m_listModel.Add(modifiedModel);
                            }
                            else
                                m_listModel[nModelIndex - 1] = modifiedModel;

                            return true;
                        }
                        else // UPDATE model_info fail
                        {
                            ConnectFactory.DBConnector().Rollback();

                            return false;
                        }
                    }
                    else // UPDATE strip_info fail
                    {
                        ConnectFactory.DBConnector().Rollback();

                        return false;
                    }
                }
                else // DBConnector is null
                {
                    return false;
                }
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in ModifyModel(ModelManager.cs");
                return false;
            }
        }

        public bool RenameModel(int nIndex, string strSrcName, string strNewName, string strNewDescription)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();

                    ModelInformation selectedModel = m_listModel[nIndex - 1];

                    String strQuery = "set sql_safe_updates=0;SET FOREIGN_KEY_CHECKS = 0;";
                    strQuery += String.Format("UPDATE model_info SET model_code = '{0}', model_name = '{0}', model_desc = '{2}' WHERE model_code = '{1}';", strNewName, strSrcName, strNewDescription);
                    strQuery += String.Format("UPDATE section_info SET model_code = '{0}' WHERE model_code = '{1}';", strNewName, strSrcName);
                    //strQuery += String.Format("UPDATE defect_result SET model_code = '{0}' WHERE model_code = '{1}';", strNewName, strSrcName);
                    //strQuery += String.Format("UPDATE inspect_result_detail SET model_code = '{0}' WHERE model_code = '{1}';", strNewName, strSrcName);
                    strQuery += String.Format("UPDATE roi_info SET model_code = '{0}' WHERE model_code = '{1}';", strNewName, strSrcName);
                    strQuery += String.Format("UPDATE roi_param SET model_code = '{0}' WHERE model_code = '{1}';", strNewName, strSrcName);
                    strQuery += "set sql_safe_updates=1;SET FOREIGN_KEY_CHECKS = 1;";

                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                    {
                        ConnectFactory.DBConnector().Commit();
                        m_listModel[nIndex - 1].Name = strNewName;

                        return true;
                    }
                    else
                    {
                        ConnectFactory.DBConnector().Rollback();

                        return false;
                    }
                }
                else // DBConnector is null
                {
                    return false;
                }
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in ModifyGroup(ModelManager.cs");
                return false;
            }
        }

        public bool DeleteModel(ModelInformation selectedModel)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();

                    // 2014-09-26 : Delete Model 요청시 실제 데이터 삭제가 아니라, use_yn을 바꾸어 주는 것으로 대체한다.

                    //String strQuery = String.Format("DELETE FROM strip_info where strip_code = '{0}'", selectedModel.Strip.Code);
                    String strQuery = String.Format("UPDATE strip_info SET use_yn = 0 WHERE strip_code = '{0}'", selectedModel.Strip.Code);

                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                    {
                        //strQuery = String.Format("DELETE FROM model_info  where model_code = '{0}'", selectedModel.Code);
                        strQuery = String.Format("UPDATE model_info SET use_yn = 0 WHERE model_code = '{0}'", selectedModel.Code);

                        if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                        {
                            ConnectFactory.DBConnector().Commit();
                            m_listModel.Remove(selectedModel);

                            return true;
                        }
                        else // UPDATE model_info fail
                        {
                            ConnectFactory.DBConnector().Rollback();
                            return false;
                        }
                    }
                    else // Update strip_info fail
                    {
                        ConnectFactory.DBConnector().Rollback();
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in DeleteModel(ModelManager.cs)");
                return false;
            }
        }

        public static bool UpdateXPitch(ModelInformation model)
        {
            string strQuery = String.Format("UPDATE model_info SET XPitch ={0} WHERE model_code = '{1}'",
                                                 model.XPitch, model.Code);

            if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
            {
                ConnectFactory.DBConnector().Commit();

                return true;
            }
            else // UPDATE model_info fail
            {
                ConnectFactory.DBConnector().Rollback();

                return false;
            }
        }

        public static bool UpdateScrabInfo(ModelInformation newModel)
        {
            string str = "";
            for (int i = 0; i < newModel.ScrabInfo.Length; i++)
            {
                str += (newModel.ScrabInfo[i]) ? "1" : "0";
                str += ",";
            }
            string strQuery = String.Format("UPDATE model_info SET scrab_info='{0}' WHERE model_code = '{1}'",
                                                 str,
                                                 newModel.Code); // 6

            if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
            {
                ConnectFactory.DBConnector().Commit();

                return true;
            }
            else // UPDATE model_info fail
            {
                ConnectFactory.DBConnector().Rollback();

                return false;
            }
        }

        public static bool UpdateModelMarkPos(System.Windows.Point pos, System.Windows.Point upos, double boatangle, double laserangle, string modelcode)
        {
            //return true;
            string strQuery = String.Format("UPDATE model_info SET mark_start_pos='{0}', umark_start_pos='{1}', boat_angle={2}, laser_angle={3} " +
                                                 "WHERE model_code = '{4}'",
                                                 pos.X.ToString("00.000") + "," + pos.Y.ToString("00.000"),
                                                 upos.X.ToString("00.000") + "," + upos.Y.ToString("00.000"),
                                                 boatangle, laserangle,
                                                 modelcode); // 6

            if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
            {
                ConnectFactory.DBConnector().Commit();

                return true;
            }
            else // UPDATE model_info fail
            {
                ConnectFactory.DBConnector().Rollback();

                return false;
            }
        }

        public static bool UpdateModelMarkInfo(ModelInformation model)
        {
            string strQuery = String.Format("UPDATE model_info SET boatpos1={0}, boatpos2 = {1}, camposy = {2}, alignpos1 = '{3}', alignpos2 = '{4}', align_rate = {5}, boat_angle = {6}, laser_angle = {7} " +
                                                 "WHERE model_code = '{8}'",
                                                 model.Marker.BoatPos1, // 0
                                                 model.Marker.BoatPos2, // 1
                                                 model.Marker.CamPosY, // 2
                                                 model.Marker.PosX1.ToString() + "," + model.Marker.PosY1.ToString(), // 3
                                                 model.Marker.PosX2.ToString() + "," + model.Marker.PosY2.ToString(), // 4
                                                 model.Marker.MatchRate, // 5
                                                 model.Marker.BoatAngle,
                                                 model.Marker.LaserAngle,
                                                 model.Code); // 6

            if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
            {
                ConnectFactory.DBConnector().Commit();

                return true;
            }
            else // UPDATE model_info fail
            {
                ConnectFactory.DBConnector().Rollback();

                return false;
            }
        }

        // 반복 설정 창에서 피치 값 수정이 들어오면 DB상 Model Spec에 반영되도록 한다.
        // 2012/02/06, suoow2.
        public static bool UpdateModelPitch(ModelInformation modifiedModel)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();

                    string strQuery = string.Format("UPDATE strip_info SET unit_width = {0:f}, unit_height = {1:f}, block_gap = {2:f}, user_id = '{3}', reg_date = now() " +
                                                    "WHERE strip_code = '{4}'",
                                                    modifiedModel.Strip.UnitWidth,
                                                    modifiedModel.Strip.UnitHeight,
                                                    modifiedModel.Strip.BlockGap,
                                                    UserManager.CurrentUser.ID,
                                                    modifiedModel.Strip.Code);

                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
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
                else return false;
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in UpdateModelPitch(ModelManager.cs");
                return false;
            }
        }
        #endregion

        #region Load Golden Group & Golden Model
        public void LoadGoldenGroup()
        {
            IDataReader dataReader = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = "SELECT DISTINCT bgadb_svr_link.model_info.model_type FROM bgadb_svr_link.model_info ";
                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                    if (dataReader != null)
                    {
                        int nIndex = 0;
                        while (dataReader.Read())
                        {
                            string szGroupName = dataReader.GetValue(0).ToString();
                            m_listGroup.Add(new Group(++nIndex, szGroupName, szGroupName));
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

        //public void LoadGoldenModel(string aszGroupName)
        //{
        //    IDataReader dataReader = null;

        //    try
        //    {
        //        if (ConnectFactory.DBConnector() != null)
        //        {
        //            string strQuery = String.Format("SELECT a.model_code, a.model_name, a.model_desc, a.model_type, a.scan_velocity, a.load_goodcount, a.use_marking, a.marker_code, a.marking_filepath, a.slot_1, a.slot_2, a.slot_3, a.slot_4, a.strip_code, " +
        //                                            "c.strip_width, c.strip_height, c.strip_align, c.unit_row, c.unit_col, c.unit_width, c.unit_height, c.block_row, c.block_col, c.block_width, c.block_height, " +
        //                                            "a.light_value, c.block_num, c.block_gap, c.total_units " +
        //                                            "FROM bgadb_svr_link.model_info a, bgadb_svr_link.strip_info c " +
        //                                            "WHERE a.model_type='{0}' AND a.strip_code = c.strip_code AND a.use_yn = 1", aszGroupName);

        //            dataReader = ConnectFactory.DBConnector().ExecuteQuery(strQuery);
        //            if (dataReader != null)
        //            {
        //                m_listModel.Clear();
        //                int nIndex = 0;
        //                while (dataReader.Read())
        //                {
        //                    ModelInformation newModel = new ModelInformation();

        //                    newModel.Index = ++nIndex; // sequence : 1, 2, 3, 4, ...

        //                    newModel.Code = dataReader.GetValue(0).ToString();
        //                    newModel.Name = dataReader.GetValue(1).ToString();
        //                    newModel.Description = dataReader.GetValue(2).ToString();
        //                    newModel.Type.Code = dataReader.GetValue(3).ToString();
        //                    newModel.ScanVelocity1 = Convert.ToInt32(dataReader.GetValue(4).ToString());
        //                    newModel.ScanVelocity2 = Convert.ToInt32(dataReader.GetValue(4).ToString());

        //                    newModel.UseMarking = (dataReader.GetValue(6).ToString() == "True") ? true : false;
        //                    newModel.Marker.Code = dataReader.GetValue(7).ToString();
        //                    newModel.Marker = RMS.Generic.MarkerInformaion.CreateBoolValueByCode(newModel.Marker.Code);

        //                    newModel.MarkingFilePath = dataReader.GetValue(8).ToString();

        //                    newModel.Strip.Code = dataReader.GetValue(13).ToString();
        //                    newModel.Strip.Name = string.Empty;
        //                    newModel.Strip.Width = Convert.ToDouble(dataReader.GetValue(14).ToString());
        //                    newModel.Strip.Height = Convert.ToDouble(dataReader.GetValue(15).ToString());
        //                    newModel.Strip.Align = Convert.ToDouble(dataReader.GetValue(16).ToString());

        //                    newModel.Strip.UnitRow = Convert.ToInt32(dataReader.GetValue(17).ToString());
        //                    newModel.Strip.UnitColumn = Convert.ToInt32(dataReader.GetValue(18).ToString());
        //                    newModel.Strip.UnitWidth = Convert.ToDouble(dataReader.GetValue(19).ToString());
        //                    newModel.Strip.UnitHeight = Convert.ToDouble(dataReader.GetValue(20).ToString());


        //                    newModel.Strip.Block = Convert.ToInt32(dataReader.GetValue(26).ToString());
        //                    newModel.Strip.BlockGap = Convert.ToDouble(dataReader.GetValue(27).ToString());

        //                    string szLight = dataReader.GetValue(25).ToString();
        //                    if (szLight.Length >= 64)
        //                    {
        //                        for (int i = 0; i < 16; i++)
        //                        {
        //                            newModel.LightValue[i] = Convert.ToInt32(szLight.Substring((i * 4), 3));

        //                        }
        //                    }
        //                    m_listModel.Add(newModel);
        //                }
        //                dataReader.Close();
        //            }                    
        //        }
        //    }
        //    catch
        //    {
        //        if (dataReader != null)
        //        {
        //            dataReader.Close();
        //        }
        //    }
        //}

        public void LoadGoldenHistory(string aszMachineType, string aszModelName)
        {
            IDataReader dataReader = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    dataReader = null;
                    string strQuery = String.Format("SELECT a.history_date, a.comment " +
                                                    "FROM bgadb_svr_link.section_info_history a " +
                                                    "WHERE a.machine_type='{0}' AND a.model_code='{1}' GROUP BY a.history_date ASC", aszMachineType, aszModelName);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        m_listHistory.Clear();
                        while (dataReader.Read())
                        {
                            GoldenHistory history = new GoldenHistory();
                            history.Date = Convert.ToDateTime(dataReader.GetValue(0).ToString());

                            string szAuthorAndComment = dataReader.GetValue(1).ToString();
                            string[] split = szAuthorAndComment.Split('|');
                            history.Author = split[0];
                            history.Comment = split[1];

                            m_listHistory.Add(history);
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
        #endregion

        public static bool UpdateBadPriority(NGInformationHelper Infos)
        {
            try
            {
                if (ConnectITS.Open())
                {
                    for (int i = 1; i < Infos.Size; i++)
                    {
                        ConnectITS.DBConnector().StartTrans();
                        Bad_Info bad = Infos.GetItem(i);
                        string strQuery = string.Format("UPDATE igsdb_bga.result_info SET priority = {0}, color = '{1}' " +
                                                        "WHERE id = {2}",
                                                        bad.Priority,
                                                        bad.Color.ToString(),
                                                        bad.ID);
  

                        if (ConnectITS.DBConnector().Execute(strQuery) > 0)
                        {
                            ConnectITS.DBConnector().Commit();
                            //return true;
                        }
                        else
                        {
                            ConnectITS.DBConnector().Rollback();
                            //return false;
                        }
                    }
                    return true;
                }
                else return false;
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in UpdateModelPitch(ModelManager.cs");
                return false;
            }
        }

        public static ObservableCollection<Bad_Info> Get_Result_Info()
        {
            ObservableCollection<Bad_Info> res = new ObservableCollection<Bad_Info>();

            IDataReader dataReader = null;
            try
            {
                if (ConnectITS.Open())
                {
                    string strQuery = string.Format("SELECT id, insp_type, res_name, mes_code, mes_fail, map, color, group_id, group_name, proc_code, proc_name, priority FROM igsdb_bga.result_info");
                    dataReader = ConnectITS.DBConnector().ExecuteQuery19(strQuery);

                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            Bad_Info bad = new Bad_Info();
                            bad.ID = Convert.ToInt32(dataReader.GetValue(0).ToString());
                            string[] tmp = dataReader.GetValue(1).ToString().Split(';');
                            for (int i = 0; i < tmp.Length; i++)
                            {
                                if(tmp[i].Trim() != "")
                                    bad.Code.Add(Convert.ToInt32(tmp[i]));
                            }
                            bad.Name = dataReader.GetValue(2).ToString();
                            bad.MES_Code = dataReader.GetValue(3).ToString();
                            bad.MES_Fail = dataReader.GetValue(4).ToString();
                            bad.Map = dataReader.GetValue(5).ToString();
                            bad.Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(dataReader.GetValue(6).ToString()));
                            bad.GroupID = Convert.ToInt32(dataReader.GetValue(7).ToString());
                            bad.GroupName = dataReader.GetValue(8).ToString();
                            bad.ProcCode = dataReader.GetValue(9).ToString();
                            bad.ProcName = dataReader.GetValue(10).ToString();
                            bad.Priority = Convert.ToInt32(dataReader.GetValue(11).ToString());
                            res.Add(bad);
                        }
                        dataReader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                if (dataReader != null)
                {
                    dataReader.Close();
                }
            }
            return res;
        }

        public static string GetMachineType(string aszMachineCode)
        {
            IDataReader dataReader = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = string.Format("SELECT machine_type FROM bgadb.machine_info WHERE machine_code='{0}' AND use_yn = 1", aszMachineCode);
                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            string szMachineType = dataReader.GetValue(0).ToString();
                            dataReader.Close();

                            return szMachineType;
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
            }
            return "";
        }

        public static ModelInformation CreateCloneModel(ModelInformation sourceModel)
        {
            ModelInformation cloneModel = new ModelInformation();

            cloneModel.Code = sourceModel.Code;
            cloneModel.Name = sourceModel.Name;
            cloneModel.Description = sourceModel.Description;
            cloneModel.Index = sourceModel.Index;
            cloneModel.ScanVelocity1 = sourceModel.ScanVelocity1;
            cloneModel.ScanVelocity2 = sourceModel.ScanVelocity2;
            cloneModel.InspectMode = sourceModel.InspectMode;
            cloneModel.AutoNG = sourceModel.AutoNG;
            cloneModel.AutoNGBlock = sourceModel.AutoNGBlock;
            cloneModel.AutoNGX = sourceModel.AutoNGX;
            cloneModel.AutoNGY = sourceModel.AutoNGY;
            cloneModel.AutoNGOuterY = sourceModel.AutoNGOuterY; 
            cloneModel.AutoNGOuterYMode = sourceModel.AutoNGOuterYMode;
            cloneModel.AutoNGOuterDivY = sourceModel.AutoNGOuterDivY;        
            cloneModel.AutoNGMatrixInfo = sourceModel.AutoNGMatrixInfo;
            cloneModel.PassingAutoNGMatrixInfo(cloneModel.AutoNGMatrixInfo);
            cloneModel.XPitch = sourceModel.XPitch;
            cloneModel.ConBad = sourceModel.ConBad;
            cloneModel.UseConBad = sourceModel.UseConBad;
            cloneModel.UseVerify = sourceModel.UseVerify;
            cloneModel.UseIDMark = sourceModel.UseIDMark;
            cloneModel.UseBondPad1 = sourceModel.UseBondPad1;
            cloneModel.UseBondPad2 = sourceModel.UseBondPad2;
            cloneModel.UseBotSur1 = sourceModel.UseBotSur1;
            cloneModel.UseBotSur2 = sourceModel.UseBotSur2;
            cloneModel.UseBotSur3 = sourceModel.UseBotSur3;
            cloneModel.UseBotSur4 = sourceModel.UseBotSur4;
            cloneModel.UseBotSur5 = sourceModel.UseBotSur5;
            cloneModel.UseTopSur1 = sourceModel.UseTopSur1;
            cloneModel.UseTopSur2 = sourceModel.UseTopSur2;
            cloneModel.UseTopSur3 = sourceModel.UseTopSur3;
            cloneModel.UseTopSur4 = sourceModel.UseTopSur4;
            cloneModel.UseTopSur5 = sourceModel.UseTopSur5;

            cloneModel.ScrabInfo = new bool[sourceModel.ScrabInfo.Length];
            for (int i = 0; i < cloneModel.ScrabInfo.Length; i++)
                cloneModel.ScrabInfo[i] = sourceModel.ScrabInfo[i];

            for (int i = 0; i < cloneModel.LightValue.GetLength(0); i++)
            {
                for (int j = 0; j < cloneModel.LightValue.GetLength(1); j++)
                {
                    cloneModel.LightValue[i,j] = sourceModel.LightValue[i,j];
                }
            }
            
            cloneModel.UseMarking = sourceModel.UseMarking;
            cloneModel.UseRailOption = sourceModel.UseRailOption;

            // Shallow-Copy시 Datacontext가 틀어지는 경우가 존재하므로 Deep-Copy를 수행하는 Clone 메서드를 만들어
            // 독립된 메모리 공간을 갖게끔 한다.
            cloneModel.Marker = sourceModel.Marker.Clone();
            cloneModel.Reject = sourceModel.Reject.Clone();
            cloneModel.Strip = sourceModel.Strip.Clone();
            cloneModel.Type = sourceModel.Type.Clone();
            cloneModel.ITS = sourceModel.ITS.Clone();
            cloneModel.UseAI = sourceModel.UseAI;

            return cloneModel;
        }

        public static bool IsModified(ModelInformation sourceModel, ModelInformation comparedModel)
        {
            if (sourceModel.Name != comparedModel.Name)
            {
                return true;
            }

            //if (sourceModel.Description != comparedModel.Description)
            //{
            //    return true;
            //}

            #region Load informations.
            if (sourceModel.InspectMode != comparedModel.InspectMode)
            {
                return true;
            }

            if (sourceModel.AutoNG != comparedModel.AutoNG)
            {
                return true;
            }
            if (sourceModel.AutoNGBlock != comparedModel.AutoNGBlock)
            {
                return true;
            }
            if (sourceModel.AutoNGX != comparedModel.AutoNGX)
            {
                return true;
            }
            if (sourceModel.AutoNGY != comparedModel.AutoNGY)
            {
                return true;
            }

            if (sourceModel.XPitch != comparedModel.XPitch)
            {
                return true;
            }

            if (sourceModel.UseConBad != comparedModel.UseConBad) return true;
            if (sourceModel.ConBad != comparedModel.ConBad) return true;

            #endregion

            if (sourceModel.UseMarking != comparedModel.UseMarking)
            {
                return true;
            }

            #region Marker information.
            if (sourceModel.Marker.UnitMark != comparedModel.Marker.UnitMark ||
                sourceModel.Marker.RailMark != comparedModel.Marker.RailMark ||
                sourceModel.Marker.NumMark != comparedModel.Marker.NumMark ||
                sourceModel.Marker.ReMark != comparedModel.Marker.ReMark ||
                sourceModel.Marker.WeekMark != comparedModel.Marker.WeekMark)
            {
                return true;
            }

            if (sourceModel.Marker.RailIrr != comparedModel.Marker.RailIrr ||
               sourceModel.Marker.FlipChip != comparedModel.Marker.FlipChip ||
               sourceModel.Marker.NumLeft != comparedModel.Marker.NumLeft ||
              sourceModel.Marker.WeekPos != comparedModel.Marker.WeekPos ||
               sourceModel.Marker.ZeroMark != comparedModel.Marker.ZeroMark)
            {
                return true;
            }

            if (sourceModel.ITS.UseITS != comparedModel.ITS.UseITS ||
                sourceModel.ITS.LeftID != comparedModel.ITS.LeftID ||
                sourceModel.ITS.InnerAOI != comparedModel.ITS.InnerAOI ||
                sourceModel.ITS.OuterAOI != comparedModel.ITS.OuterAOI ||
                sourceModel.ITS.BBT != comparedModel.ITS.BBT)
            {
                return true;
            }
            #endregion

            #region Check Strip information.
            if (sourceModel.Strip.Height != comparedModel.Strip.Height)
            {
                return true;
            }

            if (sourceModel.Strip.Width != comparedModel.Strip.Width)
            {
                return true;
            }

            if (sourceModel.Strip.Thickness != comparedModel.Strip.Thickness)
            {
                return true;
            }

            if (sourceModel.Strip.Align != comparedModel.Strip.Align)
            {
                return true;
            }

            if (sourceModel.Strip.UnitColumn != comparedModel.Strip.UnitColumn)
            {
                return true;
            }

            if (sourceModel.Strip.UnitRow != comparedModel.Strip.UnitRow)
            {
                return true;
            }

            if (sourceModel.Strip.UnitWidth != comparedModel.Strip.UnitWidth)
            {
                return true;
            }

            if (sourceModel.Strip.UnitHeight != comparedModel.Strip.UnitHeight)
            {
                return true;
            }

            if (sourceModel.Strip.Block != comparedModel.Strip.Block)
            {
                return true;
            }


            if (sourceModel.Strip.BlockGap != comparedModel.Strip.BlockGap)
            {
                return true;
            }
            #endregion

            return false;
        }

        #region Generate code. (Helper functions.)
        public static String GetMaxGroupCode()
        {
            // 가장 큰 값의 Group Index가 생성된다.
            int nMaxGroupCode = ConnectFactory.DBConnector().ExecuteScalarByInt("SELECT ifnull(max(com_dcode), 0) + 1 FROM com_detail WHERE com_mcode='10'");

            // Group Index는 1000번대를 사용하도록 함.
            if (nMaxGroupCode < 1000)
            {
                nMaxGroupCode += 1000;
            }

            return QueryHelper.GetCode(nMaxGroupCode, 4);
        }

        public static String GetMaxStripCode()
        {
            // 가장 큰 값의 Group Index가 생성된다.
            int nMaxStripCode = ConnectFactory.DBConnector().ExecuteScalarByInt("SELECT ifnull(max(strip_code), 0) + 1 from strip_info");

            return QueryHelper.GetCode(nMaxStripCode, 4);
        }
        #endregion
    }
}

