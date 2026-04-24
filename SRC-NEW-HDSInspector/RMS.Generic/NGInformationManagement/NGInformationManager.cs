using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

using System.Data;
using Common.DataBase;
using RMS.Generic.UserManagement;
using RMS.Generic.InspectManagement;


namespace RMS.Generic.NGInformationManagement
{
    public class NGManager
    {
        private ObservableCollection<NGInformation> m_listNGInformation = new ObservableCollection<NGInformation>();
        private ObservableCollection<BaseCode> m_listGroup = new ObservableCollection<BaseCode>();
        private ObservableCollection<InspectTypeInformation> m_listInspectType = new ObservableCollection<InspectTypeInformation>();

        public ObservableCollection<NGInformation> NGInformations
        {
            get
            {
                return m_listNGInformation;
            }
        }

        public ObservableCollection<BaseCode> listGroup
        {
            get
            {
                return m_listGroup;
            }
        }

        public ObservableCollection<InspectTypeInformation> listInspectType
        {
            get
            {
                return m_listInspectType;
            }
        }

        /// <summary>   Loads the group. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        public void LoadGroup()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select com_dcode, com_dname FROM com_detail where com_mcode = '20' and use_yn = 1");

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listGroup.Clear();
                    while (dataReader.Read())
                    {
                        BaseCode Group = new BaseCode();
                        Group.Code = dataReader.GetValue(0).ToString();
                        Group.Name = dataReader.GetValue(1).ToString();

                        m_listGroup.Add(Group);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Loads the group search option. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        public void LoadGroup_SearchOption()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select com_dcode, com_dname FROM com_detail where com_mcode = '20' and use_yn = 1");

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listGroup.Clear();

                    BaseCode Group = new BaseCode();
                    Group.Code = "-99";
                    Group.Name = "전체 검색";
                    m_listGroup.Add(Group);
                   
                    while (dataReader.Read())
                    {
                        Group = new BaseCode();
                        Group.Code = dataReader.GetValue(0).ToString();
                        Group.Name = dataReader.GetValue(1).ToString();
                        m_listGroup.Add(Group);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Loads the inspect type. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        public void LoadInspectType()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select inspect_code, inspect_name FROM inspect_info where use_yn = 1");

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listInspectType.Clear();
                    while (dataReader.Read())
                    {
                        InspectTypeInformation InspectType = new InspectTypeInformation();
                        InspectType.Code = dataReader.GetValue(0).ToString();
                        InspectType.Name = dataReader.GetValue(1).ToString();

                        m_listInspectType.Add(InspectType);
                    }
                    dataReader.Close();
                }
            }

        }

        /// <summary>   Loads the inspect type search option. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        public void LoadInspectType_SearchOption()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select inspect_code, inspect_name FROM inspect_info where use_yn = 1");

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listInspectType.Clear();

                    InspectTypeInformation InspectType = new InspectTypeInformation();
                    InspectType.Code = "-99";
                    InspectType.Name = "전체 검색";
                    m_listInspectType.Add(InspectType);

                    while (dataReader.Read())
                    {
                        InspectType = new InspectTypeInformation();
                        InspectType.Code = dataReader.GetValue(0).ToString();
                        InspectType.Name = dataReader.GetValue(1).ToString();
                        m_listInspectType.Add(InspectType);
                    }
                    dataReader.Close();
                }
            }

        }

        /// <summary>   Loads the ng information. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        public void LoadNGInformation()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("SELECT defect_info.defect_code, defect_info.defect_name, defect_info.defect_group, com_detail.com_dname, defect_info.inspect_code, inspect_info.inspect_name, defect_info.use_yn, defect_info.reg_date, defect_info.user_id ");
                strQuery += string.Format("FROM bgadb.com_detail com_detail, bgadb.inspect_info inspect_info, bgadb.defect_info defect_info ");
                strQuery += string.Format("WHERE defect_info.defect_group = com_detail.com_dcode AND defect_info.inspect_code = inspect_info.inspect_code");
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listNGInformation.Clear();
                    while (dataReader.Read())
                    {
                        NGInformation NGInfo = new NGInformation();
                        NGInfo.Code = dataReader.GetValue(0).ToString();
                        NGInfo.Name = dataReader.GetValue(1).ToString();
                        NGInfo.Group.Code = dataReader.GetValue(2).ToString();
                        NGInfo.Group.Name = dataReader.GetValue(3).ToString();
                        NGInfo.Type.Code = dataReader.GetValue(4).ToString();
                        NGInfo.Type.Name = dataReader.GetValue(5).ToString();
                        NGInfo.IsUse = Convert.ToBoolean(dataReader.GetValue(6).ToString());
                        NGInfo.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(7));
                        NGInfo.RegistrationID = dataReader.GetValue(8).ToString();
                        m_listNGInformation.Add(NGInfo);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Select by group. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="astrGroupCode">    The astr group code. </param>
        public void SelectByGroup(string astrGroupCode)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("SELECT defect_info.defect_code, defect_info.defect_name, defect_info.defect_group, com_detail.com_dname, defect_info.inspect_code, inspect_info.inspect_name, defect_info.use_yn, defect_info.reg_date, defect_info.user_id ");
                strQuery += string.Format("FROM bgadb.com_detail com_detail, bgadb.inspect_info inspect_info, bgadb.defect_info defect_info ");
                strQuery += string.Format("WHERE defect_info.defect_group = com_detail.com_dcode AND defect_info.inspect_code = inspect_info.inspect_code AND defect_info.defect_group = '{0}'", astrGroupCode);
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listNGInformation.Clear();
                    while (dataReader.Read())
                    {
                        NGInformation NGInfo = new NGInformation();
                        NGInfo.Code = dataReader.GetValue(0).ToString();
                        NGInfo.Name = dataReader.GetValue(1).ToString();
                        NGInfo.Group.Code = dataReader.GetValue(2).ToString();
                        NGInfo.Group.Name = dataReader.GetValue(3).ToString();
                        NGInfo.Type.Code = dataReader.GetValue(4).ToString();
                        NGInfo.Type.Name = dataReader.GetValue(5).ToString();
                        NGInfo.IsUse = Convert.ToBoolean(dataReader.GetValue(6).ToString());
                        NGInfo.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(7));
                        NGInfo.RegistrationID = dataReader.GetValue(8).ToString();
                        m_listNGInformation.Add(NGInfo);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Select by inspection type. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="astrInspType"> Type of the astr insp. </param>
        public void SelectByInspectionType(string astrInspType)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("SELECT defect_info.defect_code, defect_info.defect_name, defect_info.defect_group, com_detail.com_dname, defect_info.inspect_code, inspect_info.inspect_name, defect_info.use_yn, defect_info.reg_date, defect_info.user_id ");
                strQuery += string.Format("FROM bgadb.com_detail com_detail, bgadb.inspect_info inspect_info, bgadb.defect_info defect_info ");
                strQuery += string.Format("WHERE defect_info.defect_group = com_detail.com_dcode AND defect_info.inspect_code = inspect_info.inspect_code AND defect_info.inspect_code = '{0}'", astrInspType);
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listNGInformation.Clear();
                    while (dataReader.Read())
                    {
                        NGInformation NGInfo = new NGInformation();
                        NGInfo.Code = dataReader.GetValue(0).ToString();
                        NGInfo.Name = dataReader.GetValue(1).ToString();
                        NGInfo.Group.Code = dataReader.GetValue(2).ToString();
                        NGInfo.Group.Name = dataReader.GetValue(3).ToString();
                        NGInfo.Type.Code = dataReader.GetValue(4).ToString();
                        NGInfo.Type.Name = dataReader.GetValue(5).ToString();
                        NGInfo.IsUse = Convert.ToBoolean(dataReader.GetValue(6).ToString());
                        NGInfo.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(7));
                        NGInfo.RegistrationID = dataReader.GetValue(8).ToString();
                        m_listNGInformation.Add(NGInfo);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Select by name. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="astrNGName">   Name of the astr ng. </param>
        public void SelectByName(string astrNGName)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("SELECT defect_info.defect_code, defect_info.defect_name, defect_info.defect_group, com_detail.com_dname, defect_info.inspect_code, inspect_info.inspect_name, defect_info.use_yn, defect_info.reg_date, defect_info.user_id ");
                strQuery += string.Format("FROM bgadb.com_detail com_detail, bgadb.inspect_info inspect_info, bgadb.defect_info defect_info ");
                strQuery += string.Format("WHERE defect_info.defect_group = com_detail.com_dcode AND defect_info.inspect_code = inspect_info.inspect_code AND defect_info.defect_name LIKE '%{0}%'", astrNGName);
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listNGInformation.Clear();
                    while (dataReader.Read())
                    {
                        NGInformation NGInfo = new NGInformation();
                        NGInfo.Code = dataReader.GetValue(0).ToString();
                        NGInfo.Name = dataReader.GetValue(1).ToString();
                        NGInfo.Group.Code = dataReader.GetValue(2).ToString();
                        NGInfo.Group.Name = dataReader.GetValue(3).ToString();
                        NGInfo.Type.Code = dataReader.GetValue(4).ToString();
                        NGInfo.Type.Name = dataReader.GetValue(5).ToString();
                        NGInfo.IsUse = Convert.ToBoolean(dataReader.GetValue(6).ToString());
                        NGInfo.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(7));
                        NGInfo.RegistrationID = dataReader.GetValue(8).ToString();
                        m_listNGInformation.Add(NGInfo);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Select by group inspection type. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="astrGroupCode">    The astr group code. </param>
        /// <param name="astrInspType">     Type of the astr insp. </param>
        public void SelectByGroup_InspectionType(string astrGroupCode, string astrInspType)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("SELECT defect_info.defect_code, defect_info.defect_name, defect_info.defect_group, com_detail.com_dname, defect_info.inspect_code, inspect_info.inspect_name, defect_info.use_yn, defect_info.reg_date, defect_info.user_id ");
                strQuery += string.Format("FROM bgadb.com_detail com_detail, bgadb.inspect_info inspect_info, bgadb.defect_info defect_info ");
                strQuery += string.Format("WHERE defect_info.defect_group = com_detail.com_dcode AND defect_info.inspect_code = inspect_info.inspect_code AND defect_info.defect_group = '{0}' AND defect_info.inspect_code = '{1}'", astrGroupCode, astrInspType);
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listNGInformation.Clear();
                    while (dataReader.Read())
                    {
                        NGInformation NGInfo = new NGInformation();
                        NGInfo.Code = dataReader.GetValue(0).ToString();
                        NGInfo.Name = dataReader.GetValue(1).ToString();
                        NGInfo.Group.Code = dataReader.GetValue(2).ToString();
                        NGInfo.Group.Name = dataReader.GetValue(3).ToString();
                        NGInfo.Type.Code = dataReader.GetValue(4).ToString();
                        NGInfo.Type.Name = dataReader.GetValue(5).ToString();
                        NGInfo.IsUse = Convert.ToBoolean(dataReader.GetValue(6).ToString());
                        NGInfo.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(7));
                        NGInfo.RegistrationID = dataReader.GetValue(8).ToString();
                        m_listNGInformation.Add(NGInfo);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Select by group name. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="astrGroupCode">    The astr group code. </param>
        /// <param name="astrNGName">       Name of the astr ng. </param>
        public void SelectByGroup_Name(string astrGroupCode, string astrNGName)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("SELECT defect_info.defect_code, defect_info.defect_name, defect_info.defect_group, com_detail.com_dname, defect_info.inspect_code, inspect_info.inspect_name, defect_info.use_yn, defect_info.reg_date, defect_info.user_id ");
                strQuery += string.Format("FROM bgadb.com_detail com_detail, bgadb.inspect_info inspect_info, bgadb.defect_info defect_info ");
                strQuery += string.Format("WHERE defect_info.defect_group = com_detail.com_dcode AND defect_info.inspect_code = inspect_info.inspect_code AND defect_info.defect_group = '{0}' AND defect_info.defect_name LIKE '%{1}%'", astrGroupCode, astrNGName);
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listNGInformation.Clear();
                    while (dataReader.Read())
                    {
                        NGInformation NGInfo = new NGInformation();
                        NGInfo.Code = dataReader.GetValue(0).ToString();
                        NGInfo.Name = dataReader.GetValue(1).ToString();
                        NGInfo.Group.Code = dataReader.GetValue(2).ToString();
                        NGInfo.Group.Name = dataReader.GetValue(3).ToString();
                        NGInfo.Type.Code = dataReader.GetValue(4).ToString();
                        NGInfo.Type.Name = dataReader.GetValue(5).ToString();
                        NGInfo.IsUse = Convert.ToBoolean(dataReader.GetValue(6).ToString());
                        NGInfo.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(7));
                        NGInfo.RegistrationID = dataReader.GetValue(8).ToString();
                        m_listNGInformation.Add(NGInfo);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Select by inspection type name. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="astrInspType"> Type of the astr insp. </param>
        /// <param name="astrNGName">   Name of the astr ng. </param>
        public void SelectByInspectionType_Name(string astrInspType, string astrNGName)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("SELECT defect_info.defect_code, defect_info.defect_name, defect_info.defect_group, com_detail.com_dname, defect_info.inspect_code, inspect_info.inspect_name, defect_info.use_yn, defect_info.reg_date, defect_info.user_id ");
                strQuery += string.Format("FROM bgadb.com_detail com_detail, bgadb.inspect_info inspect_info, bgadb.defect_info defect_info ");
                strQuery += string.Format("WHERE defect_info.defect_group = com_detail.com_dcode AND defect_info.inspect_code = inspect_info.inspect_code AND defect_info.inspect_code = '{0}' AND defect_info.defect_name LIKE '%{1}%'", astrInspType, astrNGName);
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listNGInformation.Clear();
                    while (dataReader.Read())
                    {
                        NGInformation NGInfo = new NGInformation();
                        NGInfo.Code = dataReader.GetValue(0).ToString();
                        NGInfo.Name = dataReader.GetValue(1).ToString();
                        NGInfo.Group.Code = dataReader.GetValue(2).ToString();
                        NGInfo.Group.Name = dataReader.GetValue(3).ToString();
                        NGInfo.Type.Code = dataReader.GetValue(4).ToString();
                        NGInfo.Type.Name = dataReader.GetValue(5).ToString();
                        NGInfo.IsUse = Convert.ToBoolean(dataReader.GetValue(6).ToString());
                        NGInfo.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(7));
                        NGInfo.RegistrationID = dataReader.GetValue(8).ToString();
                        m_listNGInformation.Add(NGInfo);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Select by group inspection type name. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="astrGroupCode">    The astr group code. </param>
        /// <param name="astrInspType">     Type of the astr insp. </param>
        /// <param name="astrNGName">       Name of the astr ng. </param>
        public void SelectByGroup_InspectionType_Name(string astrGroupCode, string astrInspType, string astrNGName)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("SELECT defect_info.defect_code, defect_info.defect_name, defect_info.defect_group, com_detail.com_dname, defect_info.inspect_code, inspect_info.inspect_name, defect_info.use_yn, defect_info.reg_date, defect_info.user_id ");
                strQuery += string.Format("FROM bgadb.com_detail com_detail, bgadb.inspect_info inspect_info, bgadb.defect_info defect_info ");
                strQuery += string.Format("WHERE defect_info.defect_group = com_detail.com_dcode AND defect_info.inspect_code = inspect_info.inspect_code AND defect_info.defect_group = '{0}' AND defect_info.inspect_code = '{1}' AND defect_info.defect_name LIKE '%{2}%'", astrGroupCode, astrInspType, astrNGName);
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listNGInformation.Clear();
                    while (dataReader.Read())
                    {
                        NGInformation NGInfo = new NGInformation();
                        NGInfo.Code = dataReader.GetValue(0).ToString();
                        NGInfo.Name = dataReader.GetValue(1).ToString();
                        NGInfo.Group.Code = dataReader.GetValue(2).ToString();
                        NGInfo.Group.Name = dataReader.GetValue(3).ToString();
                        NGInfo.Type.Code = dataReader.GetValue(4).ToString();
                        NGInfo.Type.Name = dataReader.GetValue(5).ToString();
                        NGInfo.IsUse = Convert.ToBoolean(dataReader.GetValue(6).ToString());
                        NGInfo.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(7));
                        NGInfo.RegistrationID = dataReader.GetValue(8).ToString();
                        m_listNGInformation.Add(NGInfo);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Searches for the first by name. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="astrNGName">   Name of the astr ng. </param>
        /// <param name="astrGroup">    Group the astr belongs to. </param>
        /// <returns>   true if it succeeds, false if it fails. </returns>
        public bool SearchByName(string astrNGName, string astrGroup)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select defect_name from defect_info where defect_name = '{0}' AND defect_group = '{1}'", astrNGName, astrGroup);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    if (dataReader.Read())
                    {
                        dataReader.Close();
                        return true;
                    }
                    dataReader.Close();
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>   Generates a machine code. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-14. </remarks>
        /// <returns>   The machine code. </returns>
        public string GenerateDefectCode()
        {
            // 가장 큰 값의 Machine Index가 생성된다.
            string strQuery = "SELECT ifnull(max(defect_code), 0) + 1 from defect_info";

            int nMaxStripCode = ConnectFactory.DBConnector().ExecuteScalarByInt(strQuery);

            return QueryHelper.GetCode(nMaxStripCode, 4);
        }

        /// <summary>   Adds a ng information.  </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="aNGInformation">   Information describing the ng. </param>
        public void AddNGInformation(NGInformation aNGInformation)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                ConnectFactory.DBConnector().StartTrans();

                aNGInformation.Code = GenerateDefectCode();

                String strQuery = "INSERT INTO defect_info(defect_code, defect_name, defect_group, inspect_code, use_yn, autong_yn, reg_date, user_id) ";
                strQuery += String.Format("VALUES('{0}', '{1}', '{2}', '{3}', {4:d}, {5:d}, now(), '{6}') ",
                    aNGInformation.Code, aNGInformation.Name, aNGInformation.Group.Code, aNGInformation.Type.Code, aNGInformation.IsUse == true ? 1 : 0, aNGInformation.IsAutoNG == true ? 1 : 0, UserManager.CurrentUser.ID);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                {
                    ConnectFactory.DBConnector().Commit();
                    m_listNGInformation.Add(aNGInformation);
                }
                else
                {
                    ConnectFactory.DBConnector().Rollback();
                }
            }
            
        }

        /// <summary>   Modify ng information. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="aNGInformation">   Information describing the ng. </param>
        public void ModifyNGInformation(NGInformation aNGInformation)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                ConnectFactory.DBConnector().StartTrans();

                String strQuery = String.Format("UPDATE defect_info SET defect_name ='{0}', defect_group= '{1}', inspect_code = '{2}', use_yn ={3:d}, reg_date = now(), user_id='{4}' WHERE defect_code = '{5}'",
                   aNGInformation.Name, aNGInformation.Group.Code, aNGInformation.Type.Code, aNGInformation.IsUse == true ? 1 : 0, UserManager.CurrentUser.ID, aNGInformation.Code);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                {
                    ConnectFactory.DBConnector().Commit();
                    /* 쿼리 Error Message */
                }
                else
                {
                    ConnectFactory.DBConnector().Rollback();
                }
            }
        }

        /// <summary>   Deletes the ng information described by aNGInformation. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="aNGInformation">   Information describing the ng. </param>
        public void DeleteNGInformation(NGInformation aNGInformation)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                ConnectFactory.DBConnector().StartTrans();

                String strQuery = String.Format("DELETE FROM defect_info WHERE defect_code ='{0}' ", aNGInformation.Code);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                {
                    ConnectFactory.DBConnector().Commit();
                    m_listNGInformation.Remove(aNGInformation);
                }
                else
                {
                    ConnectFactory.DBConnector().Rollback();
                }
            }
    
        }
    }
}
