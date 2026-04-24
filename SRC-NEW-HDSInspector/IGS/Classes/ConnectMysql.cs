using System;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Data;

namespace IGS.Classes
{
    public class ConnectMysql : IDisposable
    {
        private String sCon = String.Empty;
        private MysqlConnector m_DBConnector = null;
        public static string errMsg = "";

        public MysqlConnector DBConnector()
        {
            return m_DBConnector;
        }

        public MysqlConnector CreateConnection(String strCon)
        {
            MysqlConnector Connection = null;
            sCon = strCon;

            try
            {
                Connection = new MysqlConnector();
                Connection.Connector(strCon);
                if (Connection.SqlConnection == null)
                {
                    errMsg = MysqlConnector.errmsg;
                    return null;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                Debug.WriteLine(ex.Message);
                return null;
            }

            m_DBConnector = Connection;
            return Connection;
        }

        public bool Open()
        {
            if (m_DBConnector == null) return false;

            m_DBConnector.Connector(sCon);
            if (m_DBConnector.SqlConnection == null)
                return false;

            return true;
        }

        public bool Close()
        {
            if (m_DBConnector == null) return false;
            if (m_DBConnector.SqlConnection != null)
                m_DBConnector.Close();

            return true;
        }

        public void Dispose()
        {
            Close();
        }
    }

    public class MysqlConnector
    {
        protected String m_connectionStr;
        protected MySqlDataReader m_DataReader;
        protected MySqlDataReader m_ProcDataReader;
        public static string errmsg;

        protected MySqlConnection m_Connection;
        public MySqlConnection SqlConnection
        {
            get { return m_Connection; }
            set { m_Connection = value; }
        }

        public void Connector(String connectStr = null)
        {
            String strConn = String.Empty;
            if (connectStr == null)
                strConn = m_connectionStr;
            else
                strConn = connectStr;

            try
            {
                m_Connection = new MySqlConnection(strConn);
                m_Connection.Open();
            }
            catch (MySqlException ex)
            {
                errmsg = ex.Message;
                Debug.WriteLine(ex.Message);
                m_Connection = null;
            }
        }
        
        public void Close()
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
                Debug.WriteLine("Exception occured in Close(IGS-ConnectMysql.cs)");
            }
        }

        ~MysqlConnector()
        {
            Close();
        }

        public int ExecuteScalarByInt(string strQuery)
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
                    sqlCommand.Dispose();

                Debug.WriteLine(ex.Message);
                Debug.WriteLine("Query : " + strQuery);

                return -1;
            }
        }

        public int Execute(String strQuery)
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
                    sqlCommand.Dispose();

                Debug.WriteLine(ex.Message);
                Debug.WriteLine("Query : " + strQuery);

                return -1;
            }

            return nRetRow;
        }

        public IDataReader ExecuteProcedure(ref MySqlCommand sqlCommand)
        {
            try
            {
                if (m_ProcDataReader != null)
                    m_ProcDataReader.Close();

                return sqlCommand.ExecuteReader();
            }
            catch(MySqlException ex)
            {
                if (m_ProcDataReader != null)
                    m_ProcDataReader.Close();

                if (sqlCommand != null)
                    sqlCommand.Dispose();

                Debug.WriteLine(ex.Message);

                return null;
            }
        }

        public IDataReader ExecuteQuery(String strQuery)
        {
            MySqlCommand sqlCommand = null;
            try
            {
                sqlCommand = new MySqlCommand(strQuery, m_Connection);
                if (m_DataReader != null)
                    m_DataReader.Close();

                return sqlCommand.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                if (m_DataReader != null)
                    m_DataReader.Close();

                if (sqlCommand != null)
                    sqlCommand.Dispose();

                Debug.WriteLine(ex.Message);
                Debug.WriteLine("Query : " + strQuery);

                return null;
            }
        }

        public int StartTrans()
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
                    sqlCommand.Dispose();

                Debug.WriteLine(ex.Message);
                return -1;
            }

            return nRetRow;
        }

        public int Commit()
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
                    sqlCommand.Dispose();

                Debug.WriteLine(ex.Message);
                return -1;
            }

            return nRetRow;
        }

        public int Rollback()
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
                    sqlCommand.Dispose();

                Debug.WriteLine(ex.Message);

                return -1;
            }

            return nRetRow;
        }
    }
}
