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
 * @file  InspectionQueryHelper.cs
 * @brief 
 *  Inspection query helper class.
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
using System.Linq;
using RMS.Generic.UserManagement;
using Common.DataBase;
using System.Data;

namespace Common.Drawing.InspectionInformation
{
    /// <summary>   Inspection query helper.  </summary>
    /// <remarks>   suoow2, 2014-09-30. </remarks>
    public class InspectionQueryHelper
    {
        // 공통 Insert Query.
        public static string GetInsertQuery(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, 
                                            string strRoiCode, string strInspectID, string strInspectionCode, string strParamGroup, string strParamCode, int value)
        {
            return String.Format("INSERT INTO bgadb.roi_param(" +
                                 "machine_code," +  // 0
                                 "model_code," +    // 1
                                 "work_type," +     // 2
                                 "section_code," +  // 3
                                 "roi_code," +      // 4
                                 "inspect_id," +    // 5
                                 "inspect_code," +  // 6
                                 "param_group," +   // 7
                                 "param_code," +    // 8
                                 "param_value," +   // 9
                                 "reg_date," +      // now
                                 "user_id," +       // 10
                                 "send_yn) " +      // 0
                                 "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, now(), '{10}', 0)",
                                 strMachineCode, 
                                 strModelCode, 
                                 strWorkType,
                                 strSectionCode, 
                                 strRoiCode, 
                                 strInspectID,
                                 strInspectionCode,
                                 strParamGroup,
                                 strParamCode, 
                                 value, 
                                 UserManager.CurrentUser.ID);
        }

        // 공통 Update Query
        public static string GetUpdateQuery(string strModelCode, string strRoiCode, string strInspectID, string strParamCode, int value)
        {
            return String.Format("UPDATE bgadb.roi_param SET param_value = '{0}' WHERE " +
                                 "model_code = '{1}' and " +  
                                 "roi_code = '{2}' and " +    
                                 "inspect_id = '{3}' and " +     
                                 "param_code = '{4}'",  
                                 value,
                                 strModelCode,
                                 strRoiCode,
                                 strInspectID,
                                 strParamCode);
        }

        // 공통 Select Query.
        private static string GetSelectQuery(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode,
                                            string strRoiCode, string strInspctID, string strInspectionCode, string strParamGroup, string strParamCode)
        {
            return String.Format("SELECT roi_param.param_value " +
                                 "FROM bgadb.roi_param roi_param " +
                                 "WHERE roi_param.machine_code = '{0}' " +
                                 "AND roi_param.model_code = '{1}' " +
                                 "AND roi_param.work_type = '{2}' " + 
                                 "AND roi_param.section_code = '{3}' " +
                                 "AND roi_param.roi_code = '{4}' " +
                                 "AND roi_param.inspect_id = '{5}' " +
                                 "AND roi_param.inspect_code = '{6}' " +
                                 "AND roi_param.param_group = '{7}' " +
                                 "AND roi_param.param_code = '{8}'",
                                 strMachineCode,
                                 strModelCode,
                                 strWorkType,
                                 strSectionCode,
                                 strRoiCode,
                                 strInspctID,
                                 strInspectionCode,
                                 strParamGroup,
                                 strParamCode
                                );
        }

        // 검사 설정 파라미터의 개별 값을 DB로부터 가져온다.
        public static int GetParamValue(string strMachineCode, string strModelCode, string strWorkType, 
                                        string strSectionCode, string strRoiCode, string strInspectID,
                                        string strInspectionCode, string strParamGroup, string strParamCode)
                                         
        {
            string strQuery = GetSelectQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectionCode, strParamGroup, strParamCode);

            IDataReader dataReader = null;
            /// <summary> The parameter value </summary>
            int paramValue = 0; 
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            paramValue = Convert.ToInt32(dataReader.GetValue(0).ToString()); // 파라미터 값이 넘어온다.
                            dataReader.Close();
                        }
                    }
                }
                return paramValue; // Data를 가져오는데 실패하더라도 0 값을 반환하도록 한다.
            }
            catch
            {
                if (dataReader != null)
                {
                    dataReader.Close();
                }

                return 0; // Data를 가져오는데 실패하더라도 0 값을 반환하도록 한다.
            }
        }

        // 검사 설정 파라미터의 기본값을 DB로부터 가져온다.
        public static int GetDefaultParamValue(string strInspectCode, string strParamGroupCode, string strParamCode)
        {
            IDataReader dataReader = null;
            int paramValue = 0;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = string.Format("SELECT inspect_property.param_value " +
                                                    "FROM bgadb.inspect_property WHERE " +
                                                    "inspect_property.inspect_code = '{0}' AND " +
                                                    "inspect_property.param_group = '{1}' AND " +
                                                    "inspect_property.param_code = '{2}'", strInspectCode, strParamGroupCode, strParamCode);

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
    }
}
