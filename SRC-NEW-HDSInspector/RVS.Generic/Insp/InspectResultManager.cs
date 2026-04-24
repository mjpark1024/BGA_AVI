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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;
using Common.DataBase;
using RMS.Generic.UserManagement;
using RMS.Generic;
using Common;
using System.Diagnostics;

namespace RVS.Generic.Insp
{
    /// <summary>   Manager for inspect results.  </summary>
    public class InspectResultManager
    {
        /// <summary>   Adds an inspect result.  </summary>
        /// <param name="aInspectResult">   The inspect result. </param>
        private int AddInspectResult(InspectResult aInspectResult)
        {
            int nRet = 0;
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = "INSERT INTO inspect_result(machine_code, result_code, lot_no, model_code, strip_inspcount, unit_inspcount, strip_defectcount, unit_defectcount, defect_yn, verify_yn, reject_yn, slot_num, start_time, end_time, reg_date, user_id, send_yn, closed_yn, strip_id, auto_ng) ";
                strQuery += String.Format("VALUES('{0}', '{1}', '{2}', '{3}', {4:d}, {5:d}, {6:d}, {7:d}, {8:d}, {9:d}, {10:d}, {11:d}, '{12}', now(), now(), '{13}', 0, {14:d}, {15:d}, {16:d}) ",
                    Settings.GetSettings().General.MachineCode, aInspectResult.ResultCode, aInspectResult.LotInfo.LotNo, aInspectResult.ModelCode, aInspectResult.StripInspCount, aInspectResult.UnitInspCount, aInspectResult.StripDfectCount,
                    aInspectResult.UnitDefectCount, aInspectResult.IsDefect == true ? 1 : 0, aInspectResult.IsVerify == true ? 1 : 0, aInspectResult.IsReject == true ? 1 : 0, aInspectResult.SlotNum,
                    aInspectResult.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), UserManager.CurrentUser.ID, aInspectResult.IsClosed == true ? 1 : 0, aInspectResult.StripID, aInspectResult.IsAutoNG == true ? 1 : 0);

                nRet = ConnectFactory.DBConnector().Execute(strQuery);
            }

            return nRet;
        }

        /// <summary>   Adds an inspect result detail to 'aInspectResultDetail'. </summary>
        private int AddInspectResultDetail(InspectResult aInspectResult, InspectResultDetail aInspectResultDetail)
        {
            int nRet = 0;
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = "INSERT INTO inspect_result_detail(machine_code, model_code, result_code, section_code, roi_code, unit_row, unit_col, defect_yn, verify_yn, strip_row, strip_col,work_type, reg_date, user_id, send_yn) ";
                strQuery += String.Format("VALUES('{0}','{1}', '{2}', '{3}', '{4}', {5:d}, {6:d}, {7:d}, {8:d}, {9:d}, {10:d}, '{11}', now(), '{12}', 0) ",
                    Settings.GetSettings().General.MachineCode, aInspectResult.ModelCode, aInspectResult.ResultCode, aInspectResultDetail.SectionCode, aInspectResultDetail.RoiCode, aInspectResultDetail.UnitRow, aInspectResultDetail.UnitCol,
                    aInspectResultDetail.IsDefect == true ? 1 : 0, aInspectResultDetail.IsVerify == true ? 1 : 0, aInspectResultDetail.StripRow, aInspectResultDetail.StripCol, aInspectResultDetail.WorkType, UserManager.CurrentUser.ID);

                nRet = ConnectFactory.DBConnector().Execute(strQuery);
            }

            return nRet;
        }

        /// <summary>   Adds a defect result. </summary>
        private int AddDefectResult(InspectResult aInspectResult, InspectResultDetail aInspectResultDetail, DefectResult aDefectResult)
        {
            int nRet = 0;
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = "INSERT INTO defect_result(machine_code, model_code, defect_serial, result_code, section_code, roi_code, defect_code, default_image, defect_image, live_image, defect_boundary, defect_score, defect_centerx, defect_centery, defect_posx, defect_posy, defect_size, result_id, work_type ) ";
                strQuery += String.Format("VALUES('{0}','{1}', {2:d}, '{3}','{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', {11:f} , {12:f}, {13:f}, {14:f}, {15:f}, {16:f}, '{17}', '{18}') ",
                    Settings.GetSettings().General.MachineCode, aInspectResult.ModelCode, aDefectResult.Serial, aInspectResult.ResultCode, aInspectResultDetail.SectionCode, aInspectResultDetail.RoiCode, aDefectResult.DefectCode, aDefectResult.DefaultImagePath,
                    aDefectResult.DefectImagePath, aDefectResult.LiveImagePath, aDefectResult.DefectBoundary, aDefectResult.DefectScore, aDefectResult.DefectCenterX, aDefectResult.DefectCenterY, aDefectResult.DefectPosX, aDefectResult.DefectPosY, aDefectResult.DefectSize, aDefectResult.ResultID, aDefectResult.WorkType);

                nRet = ConnectFactory.DBConnector().Execute(strQuery);
            }

            return nRet;
        }

        /// <summary>   Adds a result data.  </summary>
        public int AddResultData(InspectResult aInspectResult)
        {
            //aInspectResult.ResultCode = GenerateResultCode(aInspectResult.ModelCode);
            //aInspectResult.StripID = GenerateStripID(aInspectResult.ModelCode, aInspectResult.LotInfo.LotNo);

            int nRet = AddInspectResult(aInspectResult);

            if (nRet > 0)
            {
                foreach (InspectResultDetail aInspectResultDetail in aInspectResult.listInspectResultDetail)
                {
                    nRet = AddInspectResultDetail(aInspectResult, aInspectResultDetail);

                    if (nRet < 0)
                    {
                        Debug.WriteLine("Result_Detail Table 데이터 중복 에러!");
                        nRet = 1;
                    }

                    foreach (DefectResult aDefectResult in aInspectResultDetail.listDefectResult)
                    {
                        aDefectResult.Serial = GenerateDefectSerial();

                        nRet = AddDefectResult(aInspectResult, aInspectResultDetail, aDefectResult);
                        if (nRet <= 0)
                        {
                            return nRet;
                        }
                    }
                }
            }

            return nRet;
        }

        /// <summary>   Deletes all result described by strResultCode. </summary>
        public static int DeleteAllResult(string aszModelCode, string strResultCode)
        {
            int nRet = 0;
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("DELETE FROM defect_result where machine_code = '{0}' and result_code = '{1}' and model_code = '{2}' ", Settings.GetSettings().General.MachineCode, strResultCode, aszModelCode);
                nRet = ConnectFactory.DBConnector().Execute(strQuery);

                if (nRet >= 0)
                {
                    strQuery = String.Format("DELETE FROM inspect_result_detail where machine_code = '{0}' and result_code = '{1}' and model_code = '{2}' ", Settings.GetSettings().General.MachineCode, strResultCode, aszModelCode);
                    nRet = ConnectFactory.DBConnector().Execute(strQuery);

                    if (nRet >= 0)
                    {
                        strQuery = String.Format("DELETE FROM inspect_result where machine_code = '{0}' and result_code = '{1}' and model_code = '{2}' ", Settings.GetSettings().General.MachineCode, strResultCode, aszModelCode);
                        nRet = ConnectFactory.DBConnector().Execute(strQuery);
                    }
                }
            }

            return nRet; // nRet >= 0을 정상 값으로 간주한다.
        }

        public static int DeleteAllResult(string aszModelCode, string strMinResultCode, string strMaxResultCode)
        {
            int nRet = 0;
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("DELETE FROM defect_result where machine_code = '{0}' and result_code >= '{1}' and result_code <= '{2}' and model_code = '{3}' ", Settings.GetSettings().General.MachineCode, strMinResultCode, strMaxResultCode, aszModelCode);
                nRet = ConnectFactory.DBConnector().Execute(strQuery);

                if (nRet >= 0)
                {
                    strQuery = String.Format("DELETE FROM inspect_result_detail where machine_code = '{0}' and result_code >= '{1}' and result_code <= '{2}' and model_code = '{3}' ", Settings.GetSettings().General.MachineCode, strMinResultCode, strMaxResultCode, aszModelCode);
                    nRet = ConnectFactory.DBConnector().Execute(strQuery);

                    if (nRet >= 0)
                    {
                        strQuery = String.Format("DELETE FROM inspect_result where machine_code = '{0}' and result_code >= '{1}' and result_code <= '{2}' and model_code = '{3}' ", Settings.GetSettings().General.MachineCode, strMinResultCode, strMaxResultCode, aszModelCode);
                        nRet = ConnectFactory.DBConnector().Execute(strQuery);
                    }
                }
            }

            return nRet; // nRet >= 0을 정상 값으로 간주한다.
        }

        /// <summary>   Gets a result code list by lot no. </summary>
        public static List<string> GetResultCodeListByLotNo(string strLotNo, string strModelCode)
        {
            IDataReader dataReader = null;
            List<string> listResultCode = new List<string>();

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = String.Format("Select result_code from inspect_result WHERE machine_code = '{0}' and model_code = '{1}' and lot_no = '{2}'", Settings.GetSettings().General.MachineCode, strModelCode, strLotNo);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            String strResultCode = String.Format("{0:D15}", Convert.ToInt32(dataReader.GetValue(0).ToString()));
                            listResultCode.Add(strResultCode);
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
                Debug.WriteLine("Exception occured in GetResultCodeListByLotNo(InspectResultManager.cs)");
            }

            return listResultCode;
        }

        /// <summary>   Query if 'strMachine' is load defect. </summary>
        /// <returns>   true if load defect, false if not. </returns>
        public bool IsLoadDefect(string strMachine, string strLot, string strResultCode)
        {
            IDataReader dataReader = null;
            bool bDefect = true;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = String.Format(" SELECT defect_yn " +
                           " FROM inspect_result WHERE machine_code = '{0}' and lot_no = '{1}' and result_code = '{2}'"
                           , strMachine, strLot, strResultCode);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            string strDefect = dataReader.GetValue(0).ToString();

                            bDefect = strDefect.Equals("False") ? false : true;
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
                Debug.WriteLine("Exception occured in IsLoadDefect(InspectResultManager.cs)");
            }

            return bDefect;
        }

        #region Helper Functions. (Generate Code)
        /// <summary>   Generates a result code. </summary>
        public string GenerateResultCode(string szModelCode)
        {
            // Machine 별 Model 별 ResultCode
            if (ConnectFactory.DBConnector() != null)
            {
                string strQuery = string.Format("Select ifnull(max(result_code), 0) + 1 from inspect_result WHERE machine_code = '{0}' and model_code = '{1}'", Settings.GetSettings().General.MachineCode, szModelCode);
                
                int nMaxResultCode = ConnectFactory.DBConnector().ExecuteScalarByInt(strQuery);
                if (nMaxResultCode < 0)
                {
                    nMaxResultCode = 1; // result Code is 1-base
                }
                return string.Format("{0:D15}", nMaxResultCode);
            }
            return string.Empty;
        }

        public int GenerateStripID(string szModelCode, string aszLotNumber)
        {
            // Machine 별 Model 별 ResultCode
            if (ConnectFactory.DBConnector() != null)
            {
                string strQuery = string.Format("Select ifnull(max(strip_id) + 1, 0) from inspect_result WHERE machine_code = '{0}' and model_code = '{1}' and lot_no = '{2}'", Settings.GetSettings().General.MachineCode, szModelCode, aszLotNumber);

                int nMaxStripID = ConnectFactory.DBConnector().ExecuteScalarByInt(strQuery);
                if (nMaxStripID < 0)
                {
                    nMaxStripID = 0;
                }
                return nMaxStripID;
            }
            return 0;
        }

        public static int GetStripCount(string szModelCode, string aszLotNumber)
        {
            // Machine 별 Model 별 ResultCode
            if (ConnectFactory.DBConnector() != null)
            {
                string strQuery = string.Format("Select COUNT(*) from inspect_result WHERE machine_code = '{0}' and model_code = '{1}' and lot_no = '{2}'", Settings.GetSettings().General.MachineCode, szModelCode, aszLotNumber);

                int nMaxStripID = ConnectFactory.DBConnector().ExecuteScalarByInt(strQuery);
                if (nMaxStripID < 0)
                {
                    nMaxStripID = 0;
                }
                return nMaxStripID;
            }
            return 0;
        }

        public static uint GetMaxResultCode(string szModelCode, string aszLotNumber)
        {
            // Machine 별 Model 별 ResultCode
            if (ConnectFactory.DBConnector() != null)
            {
                string strQuery = string.Format("Select ifnull(max(result_code), 0) from inspect_result WHERE machine_code = '{0}' and model_code = '{1}' and lot_no = '{2}' ", Settings.GetSettings().General.MachineCode, szModelCode, aszLotNumber);

                int nMaxResultCode = ConnectFactory.DBConnector().ExecuteScalarByInt(strQuery);
                if (nMaxResultCode < 0)
                {
                    nMaxResultCode = 0;
                }
                return Convert.ToUInt32(nMaxResultCode);
            }
            return 0;
        }

        public static uint GetMinResultCode(string szModelCode, string aszLotNumber)
        {
            // Machine 별 Model 별 ResultCode
            if (ConnectFactory.DBConnector() != null)
            {
                string strQuery = string.Format("Select ifnull(min(result_code), 0) from inspect_result WHERE machine_code = '{0}' and model_code = '{1}' and lot_no = '{2}' ", Settings.GetSettings().General.MachineCode, szModelCode, aszLotNumber);

                int nMinResultCode = ConnectFactory.DBConnector().ExecuteScalarByInt(strQuery);
                if (nMinResultCode < 0)
                {
                    nMinResultCode = 0;
                }
                return Convert.ToUInt32(nMinResultCode);
            }
            return 0;
        }

        /// <summary>   Generates a defect serial. </summary>
        // original code : private int GenerateDefectSerial(string strModelCode, string strResultCode, string strSectionCode, string strRoiCode)
        // 인자 중 실제 사용되고 있는 인자가 없으므로 아래와 같이 삭제함 (2012-04-23 suoow2.)
        private int GenerateDefectSerial()
        {
            int nSerial = 0;
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select ifnull(max(defect_serial), 0) + 1 from defect_result WHERE machine_code = '{0}' ", Settings.GetSettings().General.MachineCode);

                nSerial = ConnectFactory.DBConnector().ExecuteScalarByInt(strQuery);
                if (nSerial < 0)
                {
                    nSerial = 0;
                }
            }

            return nSerial;
        }
        #endregion
    }
}
