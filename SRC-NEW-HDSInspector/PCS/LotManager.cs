using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

using System.Data;
using Common.DataBase;
using RMS.Generic.UserManagement;

namespace PCS
{
    public class LotManager
    {
        private ObservableCollection<LotInformation> m_listLotInformation = new ObservableCollection<LotInformation>();
       
        public ObservableCollection<LotInformation> listLotInformation
        {
            get
            {
                return m_listLotInformation;
            }
        }

        public void LoadLotInfo(string strLotCode)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("SELECT model_code, work_type, sheet_count, strip_count, unit_count, start_time, end_time, marker_code, reject_code, reg_date, user_id, send_yn ");
                strQuery += String.Format("FROM lot_info WHERE lot_no = '{0}'", strLotCode);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery(strQuery);

                if (dataReader != null)
                {
                    m_listLotInformation.Clear();
                    while (dataReader.Read())
                    {
                        LotInformation LotInfo = new LotInformation();
                        LotInfo.LotNo = strLotCode;
                        LotInfo.ModelInfo.Code = dataReader.GetValue(0).ToString();
                        LotInfo.WorkType.Code = dataReader.GetValue(1).ToString();
                        LotInfo.SheetCount = Convert.ToInt32(dataReader.GetValue(2).ToString());
                        LotInfo.StripCount = Convert.ToInt32(dataReader.GetValue(3).ToString());
                        LotInfo.UnitCount = Convert.ToInt32(dataReader.GetValue(4).ToString());
                        LotInfo.StartTime = Convert.ToDateTime(dataReader.GetValue(5).ToString());
                        LotInfo.EndTime = Convert.ToDateTime(dataReader.GetValue(6).ToString());
                        LotInfo.MarkInfo.Code = dataReader.GetValue(7).ToString();
                        LotInfo.RejectInfo.Code = dataReader.GetValue(8).ToString();
                        LotInfo.RegDate = Convert.ToDateTime(dataReader.GetValue(9).ToString());
                        
                        m_listLotInformation.Add(LotInfo);
                    }
                    dataReader.Close();
                }
            }

        }

        public void AddLotInfo(LotInformation aLotInformation)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = "INSERT INTO lot_info(lot_no,model_code, work_type, sheet_count, strip_count, unit_count, start_time, end_time, marker_code, reject_code, reg_date, user_id, send_yn) ";
                strQuery += String.Format("VALUES('{0}', '{1}', '{2}', {3:d}, {4:d}, {5:d}, '{6}', '{7}', '{8}', '{9}', now(), '{10}', 0) ",
                    aLotInformation.LotNo, aLotInformation.ModelInfo.Code, aLotInformation.WorkType.Code, aLotInformation.SheetCount, aLotInformation.StripCount, aLotInformation.UnitCount, aLotInformation.StartTime, aLotInformation.EndTime,
                    aLotInformation.MarkInfo.Code, aLotInformation.RejectInfo.Code, UserManager.Instance().CurUser.ID);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                    m_listLotInformation.Add(aLotInformation);
            }
        }

        public void ModifyLotInfo(LotInformation aLotInformation)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery =  String.Format( "UPDATE lot_info SET model_code = '{0}', work_type = '{1}', sheet_count = {2:d}, strip_count={3:d}, unit_count={4:d}, start_time ='{5}',"); 
                      strQuery += String.Format( " end_time ='{6}', marker_code = '{7}', reject_code ='{8}', reg_date = now(), user_id ='{9}' WHERE lot_no = '{10}' ", 
                                aLotInformation.ModelInfo.Code, aLotInformation.WorkType.Code, aLotInformation.SheetCount, aLotInformation.StripCount, aLotInformation.UnitCount, aLotInformation.StartTime, aLotInformation.EndTime,
                                aLotInformation.MarkInfo.Code, aLotInformation.RejectInfo.Code, UserManager.Instance().CurUser.ID,  aLotInformation.LotNo);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                    m_listLotInformation.Add(aLotInformation);
            }

        }

        public void DeleteLotInfo(LotInformation aLotInformation)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery =String.Format( "DELETE FROM lot_info  WHERE lot_no = '{0}' ", aLotInformation.LotNo);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                    m_listLotInformation.Remove(aLotInformation);
            }
        }
    }
}
