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
using Common;
using System.Diagnostics;
using System.Windows;

namespace RVS.Generic.Insp
{
    /// <summary>   Manager for lots.  </summary>
    public class LotManager
    {
        #region List Lot-Information.
        /// <summary>   Gets the information describing the list lot. </summary>
        /// <value> Information describing the list lot. </value>
        public ObservableCollection<LotInformation> listLotInformation
        {
            get
            {
                return m_listLotInformation;
            }
        }

        /// <summary> Information describing the list lot </summary>
        private ObservableCollection<LotInformation> m_listLotInformation = new ObservableCollection<LotInformation>();
        #endregion

        /********************************************************************
        *  검사 완료 후 DB 저장 Sequence
        *  1) AddResultData                 => InspectResult 를 구성 해야 함
        *  2) UpdateLotWorkCount            => Lot_work Table Count update
        *  3) UpdateLotCount                => Lot info Count Update
        * *******************************************************************/

        /// <summary>   Updates the lot count described by strLotNo. </summary>
        public int UpdateLotCount(string strLotNo)
        {
            // 검사완료 후 Lot Count Update
            int nRet = 0;
            LotInformation aLotInfo = LoadLotInfo(strLotNo);
            aLotInfo.LotCount += 1;
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("UPDATE lot_info SET lot_count = {0:d} ,end_time =now(), user_id ='{1}' " +
                                 " WHERE machine_code = '{2}' and lot_no = '{3}' "
                                 , aLotInfo.LotCount, UserManager.CurrentUser.ID, Settings.GetSettings().General.MachineCode, strLotNo);

                nRet = ConnectFactory.DBConnector().Execute(strQuery);
            }

            return nRet;
        }

        public int SyncGoodNGCount(string aszModelName, string aszLotNo, int anGoodCount, int anNGCount)
        {
            // 검사완료 후 Lot Count Update
            int nRet = 0;
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("UPDATE lot_info SET good_cnt ={0:d}, load5_cnt = {1:d} " +
                                 "WHERE machine_code = '{2}' and model_code = '{3}' and lot_no = '{4}' "
                                 , anGoodCount, anNGCount, Settings.GetSettings().General.MachineCode, aszModelName, aszLotNo);

                nRet = ConnectFactory.DBConnector().Execute(strQuery);
            }

            return nRet;
        }

        public int SyncLotWorkInspCount(string aszModelName, string aszLotNo, int anInspStripCount, int anInspUnitCount)
        {
            // lot_work의 strip 검사갯수, unit 검사갯수 동기화
            int nRet = 0;
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("UPDATE lot_work SET strip_inspcount = {0:d}, unit_inspcount = {1:d} " +
                                                "WHERE model_code = '{2}' AND machine_code = '{3}' and lot_no = '{4}' "
                                 , anInspStripCount, anInspUnitCount, aszModelName, Settings.GetSettings().General.MachineCode, aszLotNo);

                nRet = ConnectFactory.DBConnector().Execute(strQuery);
            }

            return nRet;
        }

        /// <summary>   Updates the lot work count. </summary>
        public int UpdateLotWorkCount(string strLotNo, string strModelCode, string strWorkType, int StripInspCount, int StripDefectCount, int UnitInspCount, int UnitDefectCount, bool bDefect)
        {
            int nRet = 0;
            if (ConnectFactory.DBConnector() != null)
            {
                LotWork aLotWork = LoadLotWork(strLotNo, strWorkType);
                //aLotWork.SheetInspCount += SheetInspCount;
                //aLotWork.SheetDefectCount += SheetDefectCount;
                aLotWork.UnitInspCount += UnitInspCount;
                aLotWork.StripInspCount += StripInspCount;

                if (bDefect)
                {
                    aLotWork.StripDefectCount += StripDefectCount;
                    aLotWork.UnitDefectCount += UnitDefectCount;

                }
                Debug.WriteLine("Load WorkType = {0} Inspect Count = {1} Defect Count = {2}", strWorkType, StripInspCount, StripDefectCount);

                if (string.IsNullOrEmpty(aLotWork.LotNo))
                {
                    aLotWork.LotNo = strLotNo;
                    aLotWork.ModelCode = strModelCode;
                    aLotWork.WorkType = strWorkType;

                    nRet = AddLotWork(aLotWork);
                }
                else
                {
                    nRet = UpdateLotWork(aLotWork);
                }
            }

            return nRet;
        }

        /// <summary>   Loads a lot information. </summary>
        public LotInformation LoadLotInfo(string strLotCode)
        {
            IDataReader dataReader = null;
            LotInformation LotInfo = new LotInformation();

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = String.Format("SELECT model_code, lot_count, start_time, end_time, marker_code, reject_code, reg_date, user_id, send_yn ");
                    strQuery += String.Format("FROM lot_info WHERE lot_no = '{0}' and machine_code = '{1}'", strLotCode, Settings.GetSettings().General.MachineCode);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        m_listLotInformation.Clear();
                        if (dataReader.Read())
                        {
                            LotInfo.LotNo = strLotCode;
                            LotInfo.ModelCode = dataReader.GetValue(0).ToString();
                            LotInfo.LotCount = Convert.ToInt32(dataReader.GetValue(1).ToString());
                            LotInfo.StartTime = Convert.ToDateTime(dataReader.GetValue(2).ToString());
                            LotInfo.EndTime = Convert.ToDateTime(dataReader.GetValue(3).ToString());
                            LotInfo.MarkerCode = dataReader.GetValue(4).ToString();
                            LotInfo.RejectCode = dataReader.GetValue(5).ToString();
                            LotInfo.RegDate = Convert.ToDateTime(dataReader.GetValue(6).ToString());

                            m_listLotInformation.Add(LotInfo);
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
                Debug.WriteLine("Exception occured in LoadLotInfo(LotManager.cs)");
            }

            return LotInfo;
        }

        /// <summary>   Query if 'strLotNo' is lot infomation. </summary>
        public bool IsLotInfomation(string strLotNo)
        {
            bool bRet = false;
            int nCount = -1;
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("SELECT lot_count FROM lot_info WHERE lot_no = '{0}' and machine_code = '{1}'",
                                                strLotNo, Settings.GetSettings().General.MachineCode);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                if (dataReader != null)
                {
                    if (dataReader.Read())
                    {
                        nCount = Convert.ToInt32(dataReader.GetValue(0).ToString());
                        bRet = (nCount > 0) ? true : false;
                    }
                    dataReader.Close();
                }

                if (nCount == 0) // Lot count가 0인 것은 삭제한다.
                {
                    DeleteLotInfo(strLotNo);
                }
            }
            return bRet;
        }

        /// <summary>   Adds a lot information.  </summary>
        public bool AddLotInfo(LotInformation aLotInformation)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = "INSERT INTO lot_info(machine_code, lot_no, model_code, lot_count,start_time, end_time, marker_code, reject_code, use_marking, reg_date, user_id, send_yn) ";
                strQuery += String.Format("VALUES('{0}', '{1}', '{2}', {3:d},  '{4}', '{5}', '{6}', '{7}', {8:d}, now(), '{9}', 0) ",
                   Settings.GetSettings().General.MachineCode, aLotInformation.LotNo, aLotInformation.ModelCode, aLotInformation.LotCount, aLotInformation.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), aLotInformation.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                   aLotInformation.MarkerCode, aLotInformation.RejectCode, aLotInformation.UseMarking == true ? 1 : 0, UserManager.CurrentUser.ID);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                {
                    m_listLotInformation.Add(aLotInformation);

                    return true; // Lot 등록 성공
                }
            }
            return false; // Lot 등록 실패
        }

        /// <summary>   Deletes the lot information described by aLotInformation. </summary>
        private void DeleteLotInfo(string strLotNo)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = String.Format("DELETE FROM lot_info  WHERE machine_code = '{0}' and lot_no = '{1}' ",
                                                    Settings.GetSettings().General.MachineCode, strLotNo);

                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                    {
                        int nCount = m_listLotInformation.Count;
                        for (int nIndex = 0; nIndex < nCount; nIndex++)
                        {
                            if (m_listLotInformation[nIndex].LotNo == strLotNo)
                            {
                                // Delete Lot from list.
                                m_listLotInformation.RemoveAt(nIndex);
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Exception occured in DeleteLotInfo(LotManager.cs)");
            }
        }

        /// <summary>   Adds a lot work.  </summary>
        public int AddLotWork(LotWork aLotWork)
        {
            int nRet = 0;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = "INSERT INTO lot_work(machine_code, lot_no, model_code, work_type,  sheet_inspcount, strip_inspcount, unit_inspcount, sheet_defectcount, strip_defectcount, unit_defectcount) ";
                    strQuery += String.Format("VALUES('{0}', '{1}', '{2}', '{3}', {4:d}, {5:d}, {6:d}, {7:d}, {8:d}, {9:d}) ",
                       Settings.GetSettings().General.MachineCode, aLotWork.LotNo, aLotWork.ModelCode, aLotWork.WorkType, aLotWork.SheetInspCount, aLotWork.StripInspCount, aLotWork.UnitInspCount, aLotWork.SheetDefectCount, aLotWork.StripDefectCount, aLotWork.UnitDefectCount);

                    nRet = ConnectFactory.DBConnector().Execute(strQuery);
                    Debug.WriteLine("INSERT  WorkType,= {0} Inspect Count ={1} Defect Count = {2} nRet = {3}", aLotWork.WorkType, aLotWork.StripInspCount, aLotWork.StripDefectCount, nRet);
                }
            }
            catch
            {
                Debug.WriteLine("Exception occured in AddLotWork(LotManager.cs)");
            }

            return nRet;
        }

        /// <summary>   Updates the lot work described by aLotWork. </summary>
        public int UpdateLotWork(LotWork aLotWork)
        {
            int nRet = 0;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = String.Format("UPDATE lot_work SET sheet_inspcount = {0:d}, strip_inspcount={1:d}, unit_inspcount={2:d}, " +
                                                    " sheet_defectcount = {3:d}, strip_defectcount={4:d}, unit_defectcount={5:d} " +
                                                    " WHERE machine_code = '{6}' and lot_no = '{7}' and work_type = '{8}' ",
                                                    aLotWork.SheetInspCount, aLotWork.StripInspCount, aLotWork.UnitInspCount,
                                                    aLotWork.SheetDefectCount, aLotWork.StripDefectCount, aLotWork.UnitDefectCount,
                                                    Settings.GetSettings().General.MachineCode, aLotWork.LotNo, aLotWork.WorkType);

                    nRet = ConnectFactory.DBConnector().Execute(strQuery);

                    Debug.WriteLine("UPDATE  WorkType,= {0} Inspect Count ={1} Defect Count = {2} nRet = {3}", aLotWork.WorkType, aLotWork.StripInspCount, aLotWork.StripDefectCount, nRet);
                    return nRet;
                }
            }
            catch
            {
                Debug.WriteLine("Exception occured in UpdateLotWork(LotManager.cs)");
            }

            return nRet;
        }

        /// <summary>   Loads a lot work. </summary>
        public LotWork LoadLotWork(string strLotNo, string strWorkType)
        {
            IDataReader dataReader = null;
            LotWork retLotWork = new LotWork();

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = String.Format("SELECT model_code, sheet_inspcount, strip_inspcount, unit_inspcount, sheet_defectcount, strip_defectcount, unit_defectcount ");
                    strQuery += String.Format("FROM lot_work WHERE machine_code = '{0}' and lot_no = '{1}' and work_type = '{2}' ", Settings.GetSettings().General.MachineCode, strLotNo, strWorkType);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            retLotWork.LotNo = strLotNo;
                            retLotWork.ModelCode = dataReader.GetValue(0).ToString();
                            retLotWork.WorkType = strWorkType;
                            retLotWork.SheetInspCount = Convert.ToInt32(dataReader.GetValue(1).ToString());
                            retLotWork.StripInspCount = Convert.ToInt32(dataReader.GetValue(2).ToString());
                            retLotWork.UnitInspCount = Convert.ToInt32(dataReader.GetValue(3).ToString());
                            retLotWork.SheetDefectCount = Convert.ToInt32(dataReader.GetValue(4).ToString());
                            retLotWork.StripDefectCount = Convert.ToInt32(dataReader.GetValue(5).ToString());
                            retLotWork.UnitDefectCount = Convert.ToInt32(dataReader.GetValue(6).ToString());
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
                Debug.WriteLine("Exception occured in LoadLotWork(LotManager.cs)");
            }

            return retLotWork;
        }

        /// <summary>   Deletes all lot. </summary>
        public int DeleteAllLot(string strLotNo, string strModelCode)
        {
            int nRet = 0;
            if (ConnectFactory.DBConnector() != null)
            {
                int StripCount = InspectResultManager.GetStripCount(strModelCode, strLotNo);
                uint MinResultCode = InspectResultManager.GetMinResultCode(strModelCode, strLotNo);
                uint MaxResultCode = InspectResultManager.GetMaxResultCode(strModelCode, strLotNo);

                if (StripCount != 0)
                {
                    // 로트의 ResultCode 가 연속된 번호이면 빠른 Delete 수행
                    if (StripCount == (MaxResultCode - MinResultCode + 1))
                    {
                        InspectResultManager.DeleteAllResult(strModelCode, string.Format("{0:D15}", MinResultCode), string.Format("{0:D15}", MaxResultCode));
                    }
                    else // Naive Delete (예외 상황으로 인해 Sequential 한 ResultCode를 갖지 못 한 경우 하나씩 지워줌)
                    {
                        MessageBox.Show("DB의 데이터를 삭제하는데 오랜시간이 소요될 수 있습니다.");
                        List<string> listResultCode = InspectResultManager.GetResultCodeListByLotNo(strLotNo, strModelCode);
                        if (listResultCode != null && listResultCode.Count > 0)
                        {
                            foreach (string strResultCode in listResultCode)
                            {
                                nRet = InspectResultManager.DeleteAllResult(strModelCode, strResultCode);
                                if (nRet < 0) // Failed.
                                {
                                    return nRet;
                                }
                            }
                        }
                    }
                }
                                
                string strQuery = String.Format("DELETE FROM lot_work where machine_code = '{0}' and lot_no = '{1}'", Settings.GetSettings().General.MachineCode, strLotNo);

                nRet = ConnectFactory.DBConnector().Execute(strQuery);
                if (nRet >= 0) // lot_work는 없을 수도 있으므로 ">= 0"을 정상 값으로 간주한다.
                {
                    strQuery = String.Format("DELETE FROM lot_info where machine_code = '{0}' and lot_no = '{1}'", Settings.GetSettings().General.MachineCode, strLotNo);
                    nRet = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }

            return nRet;
        }

        /// <summary>   Updates the use marking. </summary>
        public int UpdateUseMarking(string strLotNo, string strModelCode, bool bUseMarking, string strMarkCode)
        {
            int nRet = 0;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = String.Format("UPDATE model_info SET use_marking = {0:d} WHERE model_code ='{1}' ", bUseMarking == true ? 1 : 0, strModelCode);

                    nRet = ConnectFactory.DBConnector().Execute(strQuery);
                    if (nRet > 0)
                    {
                        strQuery = String.Format("UPDATE lot_info SET use_marking = {0:d}, marker_code ='{1} WHERE lot_no ='{2}' ", bUseMarking == true ? 1 : 0, strMarkCode, strLotNo);

                        nRet = ConnectFactory.DBConnector().Execute(strQuery);
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Exception occured in UpdateUseMarking(LotManager.cs)");
            }

            return nRet;
        }

        /// <summary>   Updates the load count. </summary>
        public int UpdateLoadCnt(string strLotNo, int nBadStrip, int nGoodStrip, int nFailStrip)
        {
            int nRet = 0;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String strQuery = String.Format("UPDATE lot_info SET bad_strip = {0:d} ,good_strip = {1:d}, fail_strip = {2:d} " +
                                                    " WHERE machine_code = '{3}' and lot_no = '{4}' ",
                                                    nBadStrip, nGoodStrip, nFailStrip, Settings.GetSettings().General.MachineCode, strLotNo);

                    nRet = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }
            catch
            {
                Debug.WriteLine("Exception occured in UpdateLoadCnt(LotManager.cs)");
            }

            return nRet;
        }
    }
}
