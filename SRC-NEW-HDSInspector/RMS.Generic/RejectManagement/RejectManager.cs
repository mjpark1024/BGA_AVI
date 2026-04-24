using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using System.Data;
using Common.DataBase;
using RMS.Generic.UserManagement;


namespace RMS.Generic.RejectManagement
{
    public class RejectManager
    {
        /// <summary> Information describing the list reject </summary>
        private ObservableCollection<RejectInformation> m_listRejectInformation = new ObservableCollection<RejectInformation>();

        /// <summary>   Gets the rejects. </summary>
        /// <value> The rejects. </value>
        public ObservableCollection<RejectInformation> Rejects
        {
            get
            {
                return m_listRejectInformation;
            }
        }

        /// <summary>   Loads the reject. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        public void LoadReject()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select reject_code, reject_name, stripdefect_rate, unitdefect_rate, use_yn, reg_date, user_id from reject_info ");

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listRejectInformation.Clear();
                    while (dataReader.Read())
                    {
                        RejectInformation RejectInfo = new RejectInformation();
                        RejectInfo.Code = dataReader.GetValue(0).ToString();
                        RejectInfo.Name = dataReader.GetValue(1).ToString();
                        RejectInfo.StripDefectRate = Convert.ToDouble(dataReader.GetValue(2).ToString());
                        RejectInfo.UnitDefectRate = Convert.ToDouble(dataReader.GetValue(3).ToString());
                        RejectInfo.IsUse = Convert.ToBoolean(dataReader.GetValue(4).ToString());
                        RejectInfo.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(5).ToString());
                        RejectInfo.RegistrationID = dataReader.GetValue(6).ToString();
                        m_listRejectInformation.Add(RejectInfo);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Gets a code. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="codeNumber">   The code number. </param>
        /// <param name="codeLength">   Length of the code. </param>
        /// <returns>   The code. </returns>
        private string GetCode(int codeNumber, int codeLength)
        {
            if (codeNumber < 0)
            {
                return string.Empty;
            }

            string strCode = codeNumber.ToString();
            while (strCode.Length < codeLength)
            {
                strCode = "0" + strCode;
            }

            if (strCode.Length != codeLength)
            {
                return string.Empty;
            }

            return strCode;
        }

        /// <summary>   Generates a machine code. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-14. </remarks>
        /// <returns>   The machine code. </returns>
        public string GenerateRejectCode()
        {
            // 가장 큰 값의 Machine Index가 생성된다.
            string strQuery = "SELECT ifnull(max(reject_code), 0) + 1 from reject_info";

            int nMaxStripCode = ConnectFactory.DBConnector().ExecuteScalarByInt(strQuery);

            return QueryHelper.GetCode(nMaxStripCode, 4);
        }

        /// <summary>   Select reject by name. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="astrRejectName">   Name of the astr reject. </param>
        public void SelectRejectByName(string astrRejectName)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select reject_code, reject_name, stripdefect_rate, unitdefect_rate, use_yn, reg_date, user_id from reject_info where reject_name LIKE '%{0}%'", astrRejectName);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listRejectInformation.Clear();
                    while (dataReader.Read())
                    {
                        RejectInformation RejectInfo = new RejectInformation();
                        RejectInfo.Code = dataReader.GetValue(0).ToString();
                        RejectInfo.Name = dataReader.GetValue(1).ToString();
                        RejectInfo.StripDefectRate = Convert.ToDouble(dataReader.GetValue(2).ToString());
                        RejectInfo.UnitDefectRate = Convert.ToDouble(dataReader.GetValue(3).ToString());
                        RejectInfo.IsUse = Convert.ToBoolean(dataReader.GetValue(4).ToString());
                        RejectInfo.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(5).ToString());
                        RejectInfo.RegistrationID = dataReader.GetValue(6).ToString();
                        m_listRejectInformation.Add(RejectInfo);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Searches for the first by name. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="astrRejectName">   Name of the astr reject. </param>
        /// <returns>   true if it succeeds, false if it fails. </returns>
        public bool SearchByName(string astrRejectName)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("SELECT reject_name FROM reject_info WHERE reject_name = '{0}'", astrRejectName);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    if (dataReader.Read())
                    {
                        dataReader.Close();
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>   Adds a reject.  </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="aRejectInformation">   Information describing the reject. </param>
        public void AddReject(RejectInformation aRejectInformation)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                ConnectFactory.DBConnector().StartTrans();

                aRejectInformation.Code = GenerateRejectCode();
                aRejectInformation.RegistrationID = UserManager.CurrentUser.ID;

                String strQuery = "INSERT INTO reject_info(reject_code,reject_name, stripdefect_rate, unitdefect_rate, use_yn, reg_date, user_id) ";
                strQuery += String.Format("VALUES('{0}', '{1}', {2:f}, {3:f}, {4:d}, now(), '{5}') ",
                    aRejectInformation.Code, aRejectInformation.Name, aRejectInformation.StripDefectRate, aRejectInformation.UnitDefectRate, aRejectInformation.IsUse == true ? 1 : 0, aRejectInformation.RegistrationID);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                {
                    ConnectFactory.DBConnector().Commit();
                    m_listRejectInformation.Add(aRejectInformation);
                }
                else
                {
                    ConnectFactory.DBConnector().Rollback();
                }
            }
        }

        /// <summary>   Modify reject. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="aRejectInformation">   Information describing the reject. </param>
        public void ModifyReject(RejectInformation aRejectInformation)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                ConnectFactory.DBConnector().StartTrans();

                aRejectInformation.RegistrationID = UserManager.CurrentUser.ID;

                String strQuery = String.Format("UPDATE reject_info SET reject_name ='{0}',stripdefect_rate = {1:f}, unitdefect_rate ={2:f}, use_yn ={3:D}, reg_date = now(), user_id='{4}' WHERE reject_code = '{5}'",
                   aRejectInformation.Name, aRejectInformation.StripDefectRate, aRejectInformation.UnitDefectRate, aRejectInformation.IsUse == true ? 1 : 0, aRejectInformation.RegistrationID, aRejectInformation.Code);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                {
                    ConnectFactory.DBConnector().Commit();
                }
                else
                {
                    ConnectFactory.DBConnector().Rollback();
                }
            }
        }

        /// <summary>   Deletes the reject described by aRejectInformation. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="aRejectInformation">   Information describing the reject. </param>
        public void DeleteReject(RejectInformation aRejectInformation)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                ConnectFactory.DBConnector().StartTrans();

                String strQuery = String.Format("DELETE FROM reject_info WHERE reject_code ='{0}' ", aRejectInformation.Code);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                {
                    ConnectFactory.DBConnector().Commit();
                    m_listRejectInformation.Remove(aRejectInformation);
                }
                else
                {
                    ConnectFactory.DBConnector().Rollback();
                }
            }
        }
    }
}
