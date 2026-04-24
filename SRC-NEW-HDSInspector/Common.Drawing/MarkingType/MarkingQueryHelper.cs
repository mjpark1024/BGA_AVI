using System;
using System.Collections.Generic;
using System.Linq;
using Common.DataBase;
using System.Data;

namespace Common.Drawing.MarkingInformation
{
    public class MarkingQueryHelper
    {
        // 공통 Insert Query.
        public static string GetInsertQuery(string strMachineCode, string strModelCode, string strWorkType, string strRoiCode, string strParam)
        {
            return String.Format("INSERT INTO markerdb.mark_roi_param(" +
                                 "machine_code," +  // 0
                                 "model_code," +    // 1
                                 "work_type," +     // 2
                                 "roi_code," +      // 3
                                 "roi_param," +     // 4
                                 "reg_date," +      // now
                                 "user_id," +       // 5
                                 "send_yn) " +      // 0
                                 "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', now(), '{5}', 0)",
                                 strMachineCode,
                                 strModelCode,
                                 strWorkType,
                                 strRoiCode,
                                 strParam,
                                 "1");
        }

        // 공통 Select Query.
        private static string GetSelectQuery(string strMachineCode, string strModelCode, string strWorkType, string strRoiCode)
        {
            return String.Format("SELECT mark_roi_param.roi_param " +
                                 "FROM markerdb.mark_roi_param mark_roi_param" +
                                 "WHERE mark_roi_param.machine_code = '{0}' " +
                                 "AND mark_roi_param.model_code = '{1}' " +
                                 "AND mark_roi_param.work_type = '{2}' " +
                                 "AND mark_roi_param.roi_code = '{3}'",
                                 strMachineCode,
                                 strModelCode,
                                 strWorkType,
                                 strRoiCode
                                );
        }

        // 검사 설정 파라미터의 개별 값을 DB로부터 가져온다.
        public static string[] GetParamValue(string strMachineCode, string strModelCode, string strWorkType, string strRoiCode)
        {
            string strQuery = GetSelectQuery(strMachineCode, strModelCode, strWorkType, strRoiCode);

            IDataReader dataReader = null;
            /// <summary> The parameter value </summary>
            string[] paramValue = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            string tmp = dataReader.GetValue(0).ToString(); // 파라미터 값이 넘어온다.
                            paramValue = tmp.Split(';');
                            dataReader.Close();
                        }
                    }
                }
                if (paramValue != null && paramValue.Length >= 12)
                    return paramValue; // Data를 가져오는데 실패하더라도 0 값을 반환하도록 한다.
                else return null;
            }
            catch
            {
                if (dataReader != null)
                {
                    dataReader.Close();
                }

                return null; // Data를 가져오는데 실패하더라도 0 값을 반환하도록 한다.
            }
        }

        // 검사 설정 파라미터의 기본값을 DB로부터 가져온다.
        public static int GetDefaultParamValueInt(string strMarkCode, string strParamGroupCode, string strParamCode)
        {
            IDataReader dataReader = null;
            int paramValue = 0;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = string.Format("SELECT mark_property.param_value " +
                                                    "FROM markerdb.mark_property WHERE " +
                                                    "mark_property.mark_code = '{0}' AND " +
                                                    "mark_property.param_group = '{1}' AND " +
                                                    "mark_property.param_code = '{2}'", strMarkCode, strParamGroupCode, strParamCode);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            paramValue = Convert.ToInt32(dataReader.GetValue(0).ToString());
                            dataReader.Close();
                        }
                    }
                }
                return paramValue;
            }
            catch
            {
                if (dataReader != null)
                {
                    dataReader.Close();
                }
                return 0;
            }
        }
        // 검사 설정 파라미터의 기본값을 DB로부터 가져온다.
        public static string GetDefaultParamValueStr(string strMarkCode, string strParamGroupCode, string strParamCode)
        {
            IDataReader dataReader = null;
            string paramValue = "";
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = string.Format("SELECT mark_property.param_value " +
                                                    "FROM markerdb.mark_property WHERE " +
                                                    "mark_property.mark_code = '{0}' AND " +
                                                    "mark_property.param_group = '{1}' AND " +
                                                    "mark_property.param_code = '{2}'", strMarkCode, strParamGroupCode, strParamCode);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            paramValue = dataReader.GetValue(0).ToString();
                            dataReader.Close();
                        }
                    }
                }
                return paramValue;
            }
            catch
            {
                if (dataReader != null)
                {
                    dataReader.Close();
                }
                return "";
            }
        }
    }
}
