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
 * @file  ConnectFactory.cs
 * @brief
 *  Database Connect Factory class.
 * 
 * @author : suoow <suoow.yeo@haesung.net>
 * @date : 2011.6.30
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.06.30 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Common.DataBase
{
    /// <summary>   Connect factory.  </summary>
    public class ConnectFactory
    {
        public static int MYSQL_PORT = 3306;
        private const int ADO_CONNECTION = 1;
        private const int ODBC_CONNECTION = 2;

        private static DBConnector m_DBConnector = null;
        public static DBConnector DBConnector()
        {
            return m_DBConnector;
        }

        // DB Changer.
        public static void SetDBConnector(DBConnector aNewDBConnector)
        {
            if (aNewDBConnector != null)
            {
                if (m_DBConnector != null)
                {
                    m_DBConnector.Close();
                }
                m_DBConnector = aNewDBConnector;
            }
        }

        /// <summary>   Creates a connection. </summary>
        public static DBConnector CreateConnection(String strCon)
        {
            int connectionType = 0;
            DBConnector Connection = null;
            String strConnection = strCon;

            if (strConnection.IndexOf("server") > -1)
            {
                connectionType = ADO_CONNECTION;
            }
            else if (strConnection.IndexOf("DSN") > -1)
            {
                connectionType = ODBC_CONNECTION;
            }

            try
            {
                switch (connectionType)
                {
                    case ADO_CONNECTION:
                        Connection = new ADOConnector();
                        break;
                    case ODBC_CONNECTION:
                        Connection = new ODBCConnector();
                        break;
                    default:
                        Connection = new NullConnector();
                        break;
                }

                Connection.Connector(strCon);
                if (Connection.SqlConnection == null)
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
            m_DBConnector = Connection;
            return Connection;
        }
    }
}
