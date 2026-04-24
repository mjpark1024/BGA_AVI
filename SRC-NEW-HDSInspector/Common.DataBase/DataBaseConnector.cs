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
 * @file  DataBaseConnector.cs
 * @brief
 *  Database connector.
 * 
 * @author : suoow <suoow.yeo@haesung.net>
 * @date : 2011.6.30
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.06.30 First creation.
 */

using System;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Data;

namespace Common.DataBase
{
    /// <summary>   Database connector.  </summary>
    public class DBConnector
    {
        protected MySqlConnection m_Connection;
        protected String m_connectionStr;
        protected MySqlDataReader m_DataReader;

        public MySqlConnection SqlConnection
        {
            get { return m_Connection; }
            set { m_Connection = value; }
        }

        public MySqlCommand mySqlCommand;

        public virtual void Connector(String connectStr = null) { }
        public virtual void Close() { }
        public virtual int Execute(String strQuery) { return 0; }
        public virtual int StartTrans() { return 0; }
        public virtual int Commit() { return 0; }
        public virtual int Rollback() { return 0; }
        public virtual IDataReader ExecuteQuery(String strQuery) { return null; }
        public virtual int ExecuteScalarByInt(string strQuery) { return -1; }
        public virtual IDataReader ExecuteQuery19(String strQuery) { return null; }
        public virtual void CloseCmd() { }
    }

    /// <summary>   Odbc connector.  </summary>
    public class ODBCConnector : DBConnector
    {
        public override void Connector(String connectStr = null) { }
        public override void Close() { }
        public override int Execute(String strQuery) { return 0; }
        public override int StartTrans() { return 0; }
        public override int Commit() { return 0; }
        public override int Rollback() { return 0; }
        public override IDataReader ExecuteQuery(String strQuery) { return null; }
        
        public override int ExecuteScalarByInt(string strQuery) { return -1; }
    }

    /// <summary>   Ado connector.  </summary>
    public class ADOConnector : DBConnector
    {
        public override void Connector(String connectStr = null)
        {
            String strConn = String.Empty;
            if (connectStr == null)
            {
                strConn = m_connectionStr;
            }
            else
            {
                strConn = connectStr;
            }

            try
            {
                m_Connection = new MySqlConnection(strConn);
                m_Connection.Open();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                m_Connection = null;
            }
        }

        public override void Close()
        {
            try
            {
                if (m_DataReader != null)
                {
                    m_DataReader.Close();
                    m_DataReader = null;
                }

                if (m_Connection != null)
                {
                    m_Connection.Close();
                    m_Connection = null;
                }
            }
            catch
            {
                Debug.WriteLine("Exception occured in Close(DataBaseConnector.cs)");
            }
        }

        ~ADOConnector()
        {
            Close();
        }

        public override int ExecuteScalarByInt(string strQuery)
        {
            MySqlCommand sqlCommand = null;

            try
            {
                sqlCommand = new MySqlCommand(strQuery, m_Connection);
                
                object result = sqlCommand.ExecuteScalar();
                
                sqlCommand.Dispose();
                sqlCommand = null;

                return Convert.ToInt32(result);
            }
            catch (MySqlException ex)
            {
                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }

                Debug.WriteLine(ex.Message);
                Debug.WriteLine("Query : " + strQuery);

                return -1;
            }
        }

        public override int Execute(String strQuery)
        {
            MySqlCommand sqlCommand = null;
            int nRetRow = -1;

            try
            {
                sqlCommand = new MySqlCommand(strQuery, m_Connection);
                
                nRetRow = sqlCommand.ExecuteNonQuery();
                
                sqlCommand.Dispose();
                sqlCommand = null;
            }
            catch (MySqlException ex)
            {
                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }

                Debug.WriteLine(ex.Message);
                Debug.WriteLine("Query : " + strQuery);

                return -1;
            }

            return nRetRow;
        }

        public override IDataReader ExecuteQuery(String strQuery)
        {
            MySqlCommand sqlCommand = null;
            try
            {
                // StartTrans();

                sqlCommand = new MySqlCommand(strQuery, m_Connection);
                if (m_DataReader != null)
                {
                    m_DataReader.Close();
                }

                m_DataReader = sqlCommand.ExecuteReader();
                
                sqlCommand.Dispose();
                sqlCommand = null;

                return m_DataReader;
            }
            catch (MySqlException ex)
            {
                if (m_DataReader != null)
                {
                    m_DataReader.Close();
                }
                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }

                Debug.WriteLine(ex.Message);
                Debug.WriteLine("Query : " + strQuery);

                return null;
            }
        }

        // Start Transaction.
        public override int StartTrans()
        {
            MySqlCommand sqlCommand = null;
            int nRetRow = -1;

            try
            {
                sqlCommand = new MySqlCommand("START TRANSACTION", m_Connection);
                
                nRetRow = sqlCommand.ExecuteNonQuery();

                sqlCommand.Dispose();
                sqlCommand = null;
            }
            catch (MySqlException ex)
            {
                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }

                Debug.WriteLine(ex.Message);
                return -1;
            }

            return nRetRow;
        }
        public override IDataReader ExecuteQuery19(String strQuery)
        {
            try
            {
                mySqlCommand = new MySqlCommand(strQuery, m_Connection);
                if (m_DataReader != null)
                {
                    m_DataReader.Close();
                }

                m_DataReader = mySqlCommand.ExecuteReader();

                return m_DataReader;
            }
            catch (MySqlException ex)
            {
                if (m_DataReader != null)
                {
                    m_DataReader.Close();
                }
                if (mySqlCommand != null)
                {
                    mySqlCommand.Dispose();
                }

                Debug.WriteLine(ex.Message);
                Debug.WriteLine("Query : " + strQuery);

                return null;
            }
        }

        // Commit command.
        public override int Commit()
        {
            MySqlCommand sqlCommand = null;
            int nRetRow = -1;

            try
            {
                sqlCommand = new MySqlCommand("COMMIT", m_Connection);

                nRetRow = sqlCommand.ExecuteNonQuery();

                sqlCommand.Dispose();
                sqlCommand = null;
            }
            catch (MySqlException ex)
            {
                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }

                Debug.WriteLine(ex.Message);
                return -1;
            }

            return nRetRow;
        }

        // Rollback command.
        public override int Rollback()
        {
            MySqlCommand sqlCommand = null;
            int nRetRow = -1;

            try
            {
                sqlCommand = new MySqlCommand("ROLLBACK", m_Connection);

                nRetRow = sqlCommand.ExecuteNonQuery();

                sqlCommand.Dispose();
                sqlCommand = null;
            }
            catch (MySqlException ex)
            {
                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }

                Debug.WriteLine(ex.Message);
                
                return -1;
            }

            return nRetRow;
        }
    }

    /// <summary>   Null connector.  </summary>
    public class NullConnector : DBConnector
    {
        public override void Connector(String connectStr = null) { }
        public override void Close() { }
        public override int Execute(String strQuery) { return 0; }
        public override IDataReader ExecuteQuery(String strQuery) { return null; }
        public override int ExecuteScalarByInt(string strQuery) { return -1; }
    }
}
