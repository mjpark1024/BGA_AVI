using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.DataBase;

namespace RMS.Generic.ResultDataManagement
{
    public class ResultDataManager
    {
        public List<string> m_StandardImageURI = new List<string>();
        public List<string> m_NGImageURI = new List<string>();
        public List<string> m_DefectSerial = new List<string>();
        
        public void DeleteResultImage(DateTime aStartTime, DateTime aEndTime)
        {
            if (ConnectFactory.DBConnector() != null)
            {
               ConnectFactory.DBConnector().StartTrans();

                /* 구현 예정 */
                String strQuery = String.Format("SELECT");

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
    }
}
