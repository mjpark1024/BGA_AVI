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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using Common.DataBase;

namespace RMS.Generic
{
    /// <summary>   Machine.  </summary>
    public class Machine
    {
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public int UseYn { get; set; }

        public BaseCode TypeInfo
        {
            get
            {
                if (m_TypeInfo == null)
                {
                    m_TypeInfo = new BaseCode();
                }
                return m_TypeInfo;
            }
            set
            {
                m_TypeInfo = value;
            }
        }
        private BaseCode m_TypeInfo;
    }

    /// <summary>   Manager for machines.  </summary>
    public class MachineManager
    {
        /// <summary>   Select machine information. </summary>
        public Machine SelectMachineInfo(string strCode)
        {
            Machine MachineIns = new Machine();

            if (ConnectFactory.DBConnector() != null)
            {
                string strQuery = string.Format("SELECT a.machine_code, a.machine_name, a.machine_type, b.com_dname FROM machine_info a, com_detail b WHERE a.use_yn = 1 and a.machine_code = '{0}' and b.com_mcode = '01'", strCode);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        MachineIns.MachineCode = dataReader.GetValue(0).ToString();
                        MachineIns.MachineName = dataReader.GetValue(1).ToString();
                        MachineIns.TypeInfo.Code = dataReader.GetValue(2).ToString();
                        MachineIns.TypeInfo.Name = dataReader.GetValue(3).ToString();
                    }
                    dataReader.Close();
                }
            }

            return MachineIns;
        }
    }
}
