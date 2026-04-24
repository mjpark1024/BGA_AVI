
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

using System.Data;
using Common.DataBase;
using RMS.Generic.UserManagement;

namespace RMS.Generic.InspectManagement
{
    public class InspectTypeManager
    {
        private ObservableCollection<InspectTypeInformation> m_listInspectTypeInformation = new ObservableCollection<InspectTypeInformation>();
        public ObservableCollection<InspectTypeInformation> InspectTypeInformation
        {
            get
            {
                return m_listInspectTypeInformation;
            }
        }

        /// <summary>   Loads the inspect type. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        public void LoadInspectType()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select inspect_code, inspect_name, use_yn, reg_date, user_id from inspect_info ");

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listInspectTypeInformation.Clear();
                    while (dataReader.Read())
                    {
                        InspectTypeInformation InspectType = new InspectTypeInformation();
                        InspectType.Code = dataReader.GetValue(0).ToString();
                        InspectType.Name = dataReader.GetValue(1).ToString();
                        InspectType.IsUse = Convert.ToBoolean(dataReader.GetValue(2).ToString());
                        InspectType.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(3).ToString());
                        InspectType.RegistrationID = dataReader.GetValue(4).ToString();
                        m_listInspectTypeInformation.Add(InspectType);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Select inspect type by name. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="astrName"> Name of the astr. </param>
        public void SelectInspectTypeByName(string astrName)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select inspect_code, inspect_name, use_yn, reg_date, user_id from inspect_info WHERE inspect_name LIKE '%{0}%'", astrName);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listInspectTypeInformation.Clear();
                    while (dataReader.Read())
                    {
                        InspectTypeInformation InspectType = new InspectTypeInformation();
                        InspectType.Code = dataReader.GetValue(0).ToString();
                        InspectType.Name = dataReader.GetValue(1).ToString();
                        InspectType.IsUse = Convert.ToBoolean(dataReader.GetValue(2).ToString());
                        InspectType.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(3).ToString());
                        InspectType.RegistrationID = dataReader.GetValue(4).ToString();
                        m_listInspectTypeInformation.Add(InspectType);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Searches for the first by name. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="astrName"> Name of the astr. </param>
        /// <returns>   true if it succeeds, false if it fails. </returns>
        public bool SearchByName(string astrName)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select inspect_name from inspect_info where inspect_name = '{0}'", astrName);

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

        /// <summary>   Adds an inspect type.  </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="aInspectType"> Type of the inspect. </param>
        public void AddInspectType(InspectTypeInformation aInspectType)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                ConnectFactory.DBConnector().StartTrans();

                //code 번호 획득
                aInspectType.Code = GenerateMachineCode();

                String strQuery = "INSERT INTO Inspect_info(inspect_code, inspect_name, use_yn, reg_date, user_id) ";
                strQuery += String.Format("VALUES('{0}', '{1}', {2:d}, now(), '{3}') ",
                    aInspectType.Code, aInspectType.Name, aInspectType.IsUse == true ? 1 : 0, UserManager.CurrentUser.ID);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                {
                    ConnectFactory.DBConnector().Commit();
                    m_listInspectTypeInformation.Add(aInspectType);
                }
                else
                {
                    ConnectFactory.DBConnector().Rollback();
                }
            }
        }

        /// <summary>   Modify inspect type. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="aInspectType"> Type of the inspect. </param>
        public void ModifyInspectType(InspectTypeInformation aInspectType)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                ConnectFactory.DBConnector().StartTrans();

                String strQuery = String.Format("UPDATE Inspect_info SET inspect_name ='{0}', use_yn ={1:d}, reg_date = now(), user_id='{2}' WHERE inspect_code = '{3}'",
                   aInspectType.Name, aInspectType.IsUse == true ? 1 : 0, UserManager.CurrentUser.ID, aInspectType.Code);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                {
                    ConnectFactory.DBConnector().Commit();
                    /* 쿼리 실행 Error Message */
                }
                else
                {
                    ConnectFactory.DBConnector().Rollback();
                }
            }

        }

        /// <summary>   Deletes the inspect type described by aInspectType. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="aInspectType"> Type of the inspect. </param>
        public void DeleteInspectType(InspectTypeInformation aInspectType)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                ConnectFactory.DBConnector().StartTrans();

                String strQuery = String.Format("DELETE FROM inspect_info WHERE inspect_code ='{0}' ", aInspectType.Code);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                {
                    ConnectFactory.DBConnector().Commit();
                    m_listInspectTypeInformation.Remove(aInspectType);
                }
                else
                {
                    ConnectFactory.DBConnector().Rollback();
                }
            }
        }

        /// <summary>   Generates a machine code. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-14. </remarks>
        /// <returns>   The machine code. </returns>
        public string GenerateMachineCode()
        {
            // 가장 큰 값의 Machine Index가 생성된다.
            string strQuery = "SELECT ifnull(max(inspect_code), 0) + 1 from inspect_info";

            int nMaxStripCode = ConnectFactory.DBConnector().ExecuteScalarByInt(strQuery);

            return QueryHelper.GetCode(nMaxStripCode, 4);
        }
    }
}
