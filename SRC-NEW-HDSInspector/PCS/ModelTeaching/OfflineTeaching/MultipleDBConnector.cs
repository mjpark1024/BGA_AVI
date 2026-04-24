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
 * @file  MultipleDBConnector.cs
 * @brief 
 *  각 설비로의 DB 접근을 담당한다.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2012.04.03
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2012.04.03 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.DataBase;
using System.Diagnostics;

namespace PCS.ModelTeaching.OfflineTeaching
{
    public class MultipleDBConnector
    {
        private DBConnector m_DBConnector = null;
        private KeyValuePair<string, string>[] m_ConnectStringList;

        #region Set Connect Strings of Machines.
        public void SetConnectStrings(ref List<MachineInformation> listMachines)
        {
            int nMachines = listMachines.Count;
            if (nMachines > 0)
            {
                m_ConnectStringList = new KeyValuePair<string, string>[nMachines];
                for (int nIndex = 0; nIndex < nMachines; nIndex++)
                {
                    m_ConnectStringList[nIndex] = new KeyValuePair<string, string>(listMachines[nIndex].Name, 
                                                                                   GetConnectionString(listMachines[nIndex].IP, ConnectFactory.MYSQL_PORT));
                }
            }
        }
        
        // My SQL port : 3306
        private string GetConnectionString(string aszIP, int anPort)
        {
            return string.Format("server={0}; user id={1}; password={2}; database=BGADB; port={3}; pooling=false", aszIP, "root", "mysql", anPort);
        }
        #endregion Set Connect Strings of Machines.

        // 선택한 장비로 Connection을 변경시킨다.
        // 새로 맺어진 연결은 ConnectFactory의 전역 변수로 Set 된다.
        public bool ChangeConnection(string aszConnectKey)
        {
            foreach (KeyValuePair<string, string> connection in m_ConnectStringList)
            {
                if (connection.Key == aszConnectKey)
                {
                    return CloseAndConnect(connection.Value); // Connect Success
                }
            }
            return false; // Connect Fail.
        }

        // 2012-04-04 suoow2 Added.
        // 현재 Connection을 종료하고 새로운 Connection을 생성한다.
        // 생성된 Connection은 전역 DBConnector에 반영되도록 한다.
        public bool CloseAndConnect(string aszConnectString)
        {
            try
            {
                if (m_DBConnector != null)
                {
                    m_DBConnector.Close();
                }

                if (Connect(aszConnectString))
                {
                    ConnectFactory.SetDBConnector(m_DBConnector);
                    return true;
                }
            }
            catch
            {
                Debug.WriteLine("Exception occured in CloseAndConnect(MultipleDBConnector.cs)");
            }
            return false;
        }

        private bool Connect(string aszConnectString)
        {
            DBConnector dbConnector = null;
            try
            {
                dbConnector = new ADOConnector(); // Default로 ADO_CONNECTION을 사용한다.
                dbConnector.Connector(aszConnectString);

                if (dbConnector.SqlConnection == null)
                {
                    dbConnector.Close();
                    dbConnector = null;
                    m_DBConnector = null;
                }
                m_DBConnector = dbConnector;
                Debug.Assert(m_DBConnector != null);
            }
            catch
            {
                if (dbConnector != null)
                {
                    dbConnector.Close();
                    m_DBConnector = null;
                }
                Debug.WriteLine("Exception occured in CreateConnection(MultipleDBConnector.cs)");
                return false;
            }
            return true;
        }

        #region Finalizer.
        public void Dispose()
        {
            if (m_DBConnector != null)
            {
                m_DBConnector.Close();
                m_DBConnector = null;
                GC.SuppressFinalize(this);
            }
        }

        ~MultipleDBConnector()
        {
            Dispose();
        }
        #endregion
    }
}
