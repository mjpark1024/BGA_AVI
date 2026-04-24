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
using Common;
using System.Data;
using Common.DataBase;

namespace PCS.ELF.AVI
{
    /// <summary>   Information about the lot.  </summary>
    public class LotInformation : NotifyPropertyChanged   
    {
        public static LotInformation LoadLotTableInfo(string strModelCode, string strLotNo)
        {
            LotInformation LotTableinfo = new LotInformation();
            IDataReader dataReader = null;

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = String.Format("SELECT lot_no, start_time, end_time, user_id " +
                                                    "FROM bgadb.lot_info " +
                                                    "WHERE model_code='{0}' AND lot_no = '{1}'", strModelCode, strLotNo);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            LotTableinfo.Lot_no = dataReader.GetValue(0).ToString();
                            LotTableinfo.StartTime = Convert.ToDateTime(dataReader.GetValue(1));
                            LotTableinfo.EndTime = Convert.ToDateTime(dataReader.GetValue(2));
                            LotTableinfo.UserID = dataReader.GetValue(3).ToString();
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

            return LotTableinfo;
        }

        #region Properties.
        public string Lot_no
        {
            get
            {
                return m_strLot_no;
            }
            set
            {
                m_strLot_no = value;
                Notify("Lot_no");
            }
        }

        public DateTime StartTime
        {
            get
            {
                return m_StartTime;
            }
            set
            {
                m_StartTime = value;
                Notify("StartTime");
            }
        }

        public DateTime EndTime
        {
            get
            {
                return m_EndTime;
            }
            set
            {
                m_EndTime = value;
                Notify("EndTime");
            }
        }

        public string UserID
        {
            get
            {
                return m_UserID;
            }
            set
            {
                m_UserID = value;
                Notify("UserID");
            }
        }
        #endregion

        #region Private member variables.
        private string m_strLot_no;
        private string m_UserID;

        private DateTime m_StartTime;
        private DateTime m_EndTime;
        #endregion
    }
}
