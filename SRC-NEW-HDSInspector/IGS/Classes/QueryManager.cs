using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace IGS.Classes
{
    public class QueryManager
    {
        #region Member Variables.
        private string strDBName;
        private string strDBPort;
        public string strMysqlCon;             //Mysql DB Connection String

        public int TimeOutSec = 30;            //결과 요청 대기 TimeOut (초 단위)
        public int nSleepTime = 50;
        public int nLoop = 600;

        public static bool bConnected = false;      //IGS Server 연결상태
        #endregion

        //Server MysqlDB의 IP,Port 정보를 입력해야한다.
        public bool InitConnection(string mysqlIP, string mysqlPort, string site)
        {
            try
            {
                //Make Mysql DB Connection
                if (mysqlIP == "" || mysqlPort == "")
                {
                    Log("IGS-QueryManager.cs Need Input Information 'IP', 'Port'");
                    return false;
                }

                //Local DB에 접속할 때, '127.0.0.1'로 접속하면 Windows보안에 걸려 DB접속이 제한될 수 있다.
                //가급적이면 사내망이나 설비망 IP로 접속 권장
                strDBName = site == "BGA" ? "igsdb_bga" : "igsdb_lf";
                ClientManager.productType = ProductType.STRIP;
                if (site == "BGA_AOI")
                {
                    strDBName = "mesdb_bga";
                    ClientManager.productType = ProductType.SHOT;
                }

                strDBPort = mysqlPort;
                strMysqlCon = string.Format("server={0}; user id={1}; password={2}; database={3}; port={4}; Connection Timeout=3",
                    mysqlIP, "igsClient", "mysql", strDBName, mysqlPort);

                using (ConnectMysql mysqlCon = new ConnectMysql())
                {
                    if (mysqlCon.CreateConnection(strMysqlCon) == null)
                    {
                        Log("IGS Server Mysql DB Connect Fail.");
                        return false;
                    }
                }
                //Log("IGS Server Mysql DB Connect Success.");

                return true;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs InitConnection Exception: {0}", ex.Message));
                return false;
            }
        }

        //Server Database Connection Check
        public bool ConnectionCheck()
        {
            try
            {
                if (strMysqlCon == null)
                    return false;

                using (ConnectMysql mysqlCon = new ConnectMysql())
                {
                    if (mysqlCon.CreateConnection(strMysqlCon) == null)
                    {
                        Log("IGS Server Mysql DB Connect Fail.");
                        return false;
                    }
                }
                Log("IGS Server Mysql DB Connect Success.");

                return true;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs ConnectionCheck Exception: {0}", ex.Message));
                return false;
            }
        }

        //Connection String Change
        public void ChangeConnectionString(string strIP)
        {
            strMysqlCon = string.Format("server={0}; user id={1}; password={2}; database={3}; port={4}; Connection Timeout=3",
                    strIP, "igsClient", "mysql", strDBName, strDBPort);
        }

        //Server Database Reconnection Check
        public bool DBReconnectionCheck(string strIP, string strMachine)
        {
            try
            {
                bool bRes = false;
                string strConn = string.Format("server={0}; user id={1}; password={2}; database={3}; port={4}; Connection Timeout=1",
                    strIP, "igsClient", "mysql", strDBName, strDBPort);

                ConnectMysql mysqlCon = new ConnectMysql();
                if (mysqlCon.CreateConnection(strConn) == null)
                {
                    Log(string.Format("DBReconnectionCheck Server Reconnect Fail. IP: {0}", strIP));
                    return false;
                }

                if (CheckDemonFlag(ref mysqlCon))
                {
                    if (CheckSwitchHeart(ref mysqlCon, 1, strMachine))
                        bRes = true;
                }

                mysqlCon.Close();
                return bRes;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs DBReconnectionCheck Exception: {0}", ex.Message));
                return false;
            }
        }

        //Server Demon Enable Flag Check
        public bool CheckDemonFlag()
        {
            try
            {
                bool bRes = false;

                ConnectMysql mysql = new ConnectMysql();
                if (mysql.CreateConnection(strMysqlCon) == null)
                    return false;

                bRes = CheckDemonFlag(ref mysql);
                mysql.Close();

                return bRes;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs CheckDemonFlag Exception: {0}", ex.Message));
                return false;
            }
        }

        //Server Demon Enable Flag Check
        private bool CheckDemonFlag(ref ConnectMysql mysql)
        {
            try
            {
                bool bRes = false;

                String query = String.Format("SELECT DEMON_FLAG FROM {0}.server_info", strDBName);
                IDataReader dataReader = mysql.DBConnector().ExecuteQuery(query);
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        bRes = Convert.ToBoolean(dataReader.GetValue(0).ToString());
                        break;
                    }
                    dataReader.Close();
                }

                return bRes;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs CheckDemonFlag Exception: {0}", ex.Message));
                return false;
            }
        }

        //Server Demon Heartbeat Check
        public bool CheckSwitchHeart(ref ConnectMysql mysql, int nTimeout, string strMachine)
        {
            try
            {
                bool bRes = false;

                DateTime now = DateTime.Now;
                String query = String.Format("INSERT INTO {0}.request_info(REQ_STATE, MACHINE_CODE, REPORT_CODE, REQUEST_TIME) VALUES(0, '{1}', '{2}', '{3}')",
                    strDBName, strMachine, (int)ReportType.CHECK_HEARTBEAT, now.ToString("yyyy-MM-dd HH:mm:ss"));

                if (mysql.DBConnector().Execute(query) > 0)
                    bRes = true;

                if (bRes)
                {
                    query = String.Format("SELECT REQ_STATE FROM {0}.request_info WHERE MACHINE_CODE='{1}' AND REQUEST_TIME='{2}' AND REPORT_CODE='{3}' AND REQ_STATE>1",
                        strDBName, strMachine, now.ToString("yyyy-MM-dd HH:mm:ss"), (int)ReportType.CHECK_HEARTBEAT);

                    int nFlag = -1;
                    int nLoopCount = 0;
                    int nWaitLoop = nTimeout * 1000 / 50;

                    IDataReader dataReader;
                    while (nFlag == -1)
                    {
                        dataReader = mysql.DBConnector().ExecuteQuery(query);
                        if (dataReader != null)
                        {
                            while (dataReader.Read())
                            {
                                nFlag = Convert.ToInt32(dataReader.GetValue(0).ToString());
                                break;
                            }
                            dataReader.Close();
                        }

                        if (nFlag == -1)
                        {
                            if (nLoopCount > nWaitLoop)
                            {
                                Log(string.Format("IGS-QueryManager.cs CheckSwitchEnable Timeout {0}Sec", nTimeout));
                                break;
                            }

                            Thread.Sleep(50);
                            nLoopCount++;
                        }
                    }

                    query = String.Format("DELETE FROM {0}.request_info WHERE MACHINE_CODE='{1}' AND REPORT_CODE='{2}'", strDBName, strMachine, (int)ReportType.CHECK_HEARTBEAT);
                    mysql.DBConnector().Execute(query);

                    if (nFlag != -1)
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs CheckSwitchEnable Exception: {0}", ex.Message));
                return false;
            }
        }

        //입력받은 시간(초) 만큼 서버 응답을 대기한다. Timeout
        public void SetTimeOut(int nSecond)
        {
            TimeOutSec = nSecond;
            nLoop = TimeOutSec * 1000 / nSleepTime;
        }

        #region Client Interface
        //해당 검사기에서 마지막으로 진행한 모델명을 반환한다. 기종교체 여부 판단을 위함.
        public bool GetLastJobModel(string strMC, ref string strModel)
        {
            try
            {
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("SELECT STD_MODEL FROM {0}.job_info WHERE MACHINE='{1}' ORDER BY CREATE_TIME DESC LIMIT 1", strDBName, strMC);
                    IDataReader dataReader = mysql.DBConnector().ExecuteQuery(query);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            strModel = dataReader.GetValue(0).ToString();
                            break;
                        }
                        dataReader.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs GetLastJobModel Exception: {0}", ex.Message));
                return false;
            }
        }

        //Server의 MES 모델 기준정보에서 가지고 있는 고객사 코드, 이름 정보를 Dictionary구조로 반환한다.
        public bool GetCustMatchList(ref Dictionary<string, string> matchList)
        {
            try
            {
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("SELECT DISTINCT(CUST_CD), CUST_NM FROM {0}.model_info", strDBName);
                    IDataReader dataReader = mysql.DBConnector().ExecuteQuery(query);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            string strCd = dataReader.GetValue(0).ToString();
                            string strNm = dataReader.GetValue(1).ToString();

                            if (strNm.Contains("\'"))
                                strNm = strNm.Replace("\'", "\\\'");
                            if (strNm.Contains("\""))
                                strNm = strNm.Replace("\"", "\\\"");
                            if (strNm.Contains("\'"))
                                strNm = strNm.Replace("\'", "\\\'");
                            if (strNm.Contains("\""))
                                strNm = strNm.Replace("\"", "\\\"");

                            matchList.Add(strCd, strNm);
                        }
                        dataReader.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs GetCustMatchList Exception: {0}", ex.Message));
                return false;
            }
        }

        //Server에 있는 전체 모델 기준정보 리스트를 반환한다.
        public bool GetAllModelData(out List<MODEL_RECIPE_DATA> dataList)
        {
            dataList = null;
            try
            {
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("SELECT ITEM_GRP, ITEM_KIND_CD, ITEM_CD, ITEM_NM, SPEC_NO, REV_LVL, CUST_CD, CUST_NM, NEW_DIV, "
                        + "XOUT_YIELD, UPP, UNIT_PER_ROW, ARRAY, INLINE_MULTI_WK, ITEM_LEN, STRIP_WIDTH, PITCH, QM_MARK_DIV, QM_MARK_CUST, "
                        + "MOLD_GATE, STRIP_WAY, AOI_IN, AOI_OUT, BBT, AVI, MARKING_2D, IF_DTTM, PCS_STRIP, STRIP_BUNDLE, LEVEL4 FROM {0}.model_info", strDBName);

                    IDataReader dataReader = mysql.DBConnector().ExecuteQuery(query);
                    if (dataReader != null)
                    {
                        dataList = new List<MODEL_RECIPE_DATA>();

                        while (dataReader.Read())
                        {
                            MODEL_RECIPE_DATA data = new MODEL_RECIPE_DATA();

                            data.ITEM_GRP = dataReader.GetValue(0).ToString();
                            data.ITEM_KIND_CD = dataReader.GetValue(1).ToString();
                            data.ITEM_CD = dataReader.GetValue(2).ToString();
                            data.ITEM_NM = dataReader.GetValue(3).ToString();
                            data.SPEC_NO = dataReader.GetValue(4).ToString();
                            data.REV_LVL = dataReader.GetValue(5).ToString();
                            data.CUST_CD = dataReader.GetValue(6).ToString();
                            data.CUST_NM = dataReader.GetValue(7).ToString();
                            data.NEW_DIV = dataReader.GetValue(8).ToString();
                            data.XOUT_YIELD = dataReader.GetValue(9).ToString();

                            try
                            {
                                //MES상에 unit row, column 정보가 정확하지 않아 임의로 처리. 
                                //둘 중 큰 값을 Strip 길이방향 Unit수로 본다.
                                string upp = dataReader.GetValue(10).ToString();
                                string upr = dataReader.GetValue(11).ToString();

                                int n_upp = Convert.ToInt32(upp);
                                int n_upr = Convert.ToInt32(upr);

                                data.UPP = (n_upp > n_upr) ? n_upp.ToString() : n_upr.ToString();
                                data.UNIT_PER_ROW = (n_upp > n_upr) ? n_upr.ToString() : n_upp.ToString();
                            }
                            catch (Exception exc)
                            {
                                data.UPP = "0";
                                data.UNIT_PER_ROW = "0";

                                Log(string.Format("IGS-QueryManager.cs GetAllModelData Upp, Upr Calc Fail: {0}", exc.Message));
                            }

                            data.ARRAY = dataReader.GetValue(12).ToString();
                            data.INLINE_MULTI_WK = dataReader.GetValue(13).ToString();
                            data.ITEM_LEN = dataReader.GetValue(14).ToString();
                            data.STRIP_WIDTH = dataReader.GetValue(15).ToString();
                            data.PITCH = dataReader.GetValue(16).ToString();
                            data.QM_MARK_DIV = dataReader.GetValue(17).ToString();
                            data.QM_MARK_CUST = dataReader.GetValue(18).ToString();
                            data.MOLD_GATE = dataReader.GetValue(19).ToString();
                            data.STRIP_WAY = dataReader.GetValue(20).ToString();
                            data.AOI_IN = dataReader.GetValue(21).ToString();
                            data.AOI_OUT = dataReader.GetValue(22).ToString();
                            data.BBT = dataReader.GetValue(23).ToString();
                            data.AVI = dataReader.GetValue(24).ToString();
                            data.MARKING_2D = dataReader.GetValue(25).ToString();
                            data.IF_DTTM = Convert.ToDateTime(dataReader.GetValue(26).ToString());
                            data.PCS_STRIP = dataReader.GetValue(27).ToString();
                            data.STRIP_BUNDLE = dataReader.GetValue(28).ToString();
                            data.LEVEL4 = dataReader.GetValue(29).ToString();

                            dataList.Add(data.Clone());
                        }
                        dataReader.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs GetAllModelData Exception: {0}", ex.Message));
                return false;
            }
        }

        //GetModelData By ModelName
        //If Query Execute Fail or No data, return null
        //Unit 행, 열 개수 연산에 문제가 생길 경우 둘 다 0으로 반환한다.
        public bool GetModelData(string modelName, out List<MODEL_RECIPE_DATA> dataList)
        {
            dataList = null;
            try
            {
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("SELECT ITEM_GRP, ITEM_KIND_CD, ITEM_CD, ITEM_NM, SPEC_NO, REV_LVL, CUST_CD, CUST_NM, NEW_DIV, "
                        + "XOUT_YIELD, UPP, UNIT_PER_ROW, ARRAY, INLINE_MULTI_WK, ITEM_LEN, STRIP_WIDTH, PITCH, QM_MARK_DIV, QM_MARK_CUST, "
                        + "MOLD_GATE, STRIP_WAY, AOI_IN, AOI_OUT, BBT, AVI, MARKING_2D, IF_DTTM, PCS_STRIP, STRIP_BUNDLE, LEVEL4 FROM {0}.model_info "
                        + "WHERE ITEM_CD='{1}'", strDBName, modelName);

                    IDataReader dataReader = mysql.DBConnector().ExecuteQuery(query);
                    if (dataReader != null)
                    {
                        dataList = new List<MODEL_RECIPE_DATA>();

                        while (dataReader.Read())
                        {
                            MODEL_RECIPE_DATA data = new MODEL_RECIPE_DATA();

                            data.ITEM_GRP = dataReader.GetValue(0).ToString();
                            data.ITEM_KIND_CD = dataReader.GetValue(1).ToString();
                            data.ITEM_CD = dataReader.GetValue(2).ToString();
                            data.ITEM_NM = dataReader.GetValue(3).ToString();
                            data.SPEC_NO = dataReader.GetValue(4).ToString();
                            data.REV_LVL = dataReader.GetValue(5).ToString();
                            data.CUST_CD = dataReader.GetValue(6).ToString();
                            data.CUST_NM = dataReader.GetValue(7).ToString();
                            data.NEW_DIV = dataReader.GetValue(8).ToString();
                            data.XOUT_YIELD = dataReader.GetValue(9).ToString();

                            try
                            {
                                //MES상에 unit row, column 정보가 정확하지 않아 임의로 처리. 
                                //둘 중 큰 값을 Strip 길이방향 Unit수로 본다.
                                string upp = dataReader.GetValue(10).ToString();
                                string upr = dataReader.GetValue(11).ToString();

                                int n_upp = Convert.ToInt32(upp);
                                int n_upr = Convert.ToInt32(upr);

                                data.UPP = (n_upp > n_upr) ? n_upp.ToString() : n_upr.ToString();
                                data.UNIT_PER_ROW = (n_upp > n_upr) ? n_upr.ToString() : n_upp.ToString();
                            }
                            catch (Exception exc)
                            {
                                data.UPP = "0";
                                data.UNIT_PER_ROW = "0";

                                Log(string.Format("IGS-QueryManager.cs GetModelData Upp, Upr Calc Fail: {0}", exc.Message));
                            }

                            data.ARRAY = dataReader.GetValue(12).ToString();
                            data.INLINE_MULTI_WK = dataReader.GetValue(13).ToString();
                            data.ITEM_LEN = dataReader.GetValue(14).ToString();
                            data.STRIP_WIDTH = dataReader.GetValue(15).ToString();
                            data.PITCH = dataReader.GetValue(16).ToString();
                            data.QM_MARK_DIV = dataReader.GetValue(17).ToString();
                            data.QM_MARK_CUST = dataReader.GetValue(18).ToString();
                            data.MOLD_GATE = dataReader.GetValue(19).ToString();
                            data.STRIP_WAY = dataReader.GetValue(20).ToString();
                            data.AOI_IN = dataReader.GetValue(21).ToString();
                            data.AOI_OUT = dataReader.GetValue(22).ToString();
                            data.BBT = dataReader.GetValue(23).ToString();
                            data.AVI = dataReader.GetValue(24).ToString();
                            data.MARKING_2D = dataReader.GetValue(25).ToString();
                            data.IF_DTTM = Convert.ToDateTime(dataReader.GetValue(26).ToString());
                            data.PCS_STRIP = dataReader.GetValue(27).ToString();
                            data.STRIP_BUNDLE = dataReader.GetValue(28).ToString();
                            data.LEVEL4 = dataReader.GetValue(29).ToString();

                            dataList.Add(data.Clone());
                        }
                        dataReader.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs GetModelData Exception: {0}", ex.Message));
                return false;
            }
        }

        //GetModelData By ModelName, CustomCode
        //If Query Execute Fail or No data, return null
        public bool GetModelData(string modelName, string customCode, out MODEL_RECIPE_DATA data)
        {
            data = null;
            try
            {
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("SELECT ITEM_GRP, ITEM_KIND_CD, ITEM_CD, ITEM_NM, SPEC_NO, REV_LVL, CUST_CD, CUST_NM, NEW_DIV, "
                        + "XOUT_YIELD, UPP, UNIT_PER_ROW, ARRAY, INLINE_MULTI_WK, ITEM_LEN, STRIP_WIDTH, PITCH, QM_MARK_DIV, QM_MARK_CUST, "
                        + "MOLD_GATE, STRIP_WAY, AOI_IN, AOI_OUT, BBT, AVI, MARKING_2D, IF_DTTM, PCS_STRIP, STRIP_BUNDLE, LEVEL4 FROM {0}.model_info "
                        + "WHERE ITEM_CD='{1}' AND CUST_CD='{2}'", strDBName, modelName, customCode);

                    IDataReader dataReader = mysql.DBConnector().ExecuteQuery(query);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            data.ITEM_GRP = dataReader.GetValue(0).ToString();
                            data.ITEM_KIND_CD = dataReader.GetValue(1).ToString();
                            data.ITEM_CD = dataReader.GetValue(2).ToString();
                            data.ITEM_NM = dataReader.GetValue(3).ToString();
                            data.SPEC_NO = dataReader.GetValue(4).ToString();
                            data.REV_LVL = dataReader.GetValue(5).ToString();
                            data.CUST_CD = dataReader.GetValue(6).ToString();
                            data.CUST_NM = dataReader.GetValue(7).ToString();
                            data.NEW_DIV = dataReader.GetValue(8).ToString();
                            data.XOUT_YIELD = dataReader.GetValue(9).ToString();

                            try
                            {
                                //MES상에 unit row, column 정보가 정확하지 않아 임의로 처리. 
                                //둘 중 큰 값을 Strip 길이방향 Unit수로 본다.
                                string upp = dataReader.GetValue(10).ToString();
                                string upr = dataReader.GetValue(11).ToString();

                                int n_upp = Convert.ToInt32(upp);
                                int n_upr = Convert.ToInt32(upr);

                                data.UPP = (n_upp > n_upr) ? n_upp.ToString() : n_upr.ToString();
                                data.UNIT_PER_ROW = (n_upp > n_upr) ? n_upr.ToString() : n_upp.ToString();
                            }
                            catch (Exception exc)
                            {
                                data.UPP = "0";
                                data.UNIT_PER_ROW = "0";

                                Log(string.Format("IGS-QueryManager.cs GetModelData Upp, Upr Calc Fail: {0}", exc.Message));
                            }

                            data.ARRAY = dataReader.GetValue(12).ToString();
                            data.INLINE_MULTI_WK = dataReader.GetValue(13).ToString();
                            data.ITEM_LEN = dataReader.GetValue(14).ToString();
                            data.STRIP_WIDTH = dataReader.GetValue(15).ToString();
                            data.PITCH = dataReader.GetValue(16).ToString();
                            data.QM_MARK_DIV = dataReader.GetValue(17).ToString();
                            data.QM_MARK_CUST = dataReader.GetValue(18).ToString();
                            data.MOLD_GATE = dataReader.GetValue(19).ToString();
                            data.STRIP_WAY = dataReader.GetValue(20).ToString();
                            data.AOI_IN = dataReader.GetValue(21).ToString();
                            data.AOI_OUT = dataReader.GetValue(22).ToString();
                            data.BBT = dataReader.GetValue(23).ToString();
                            data.AVI = dataReader.GetValue(24).ToString();
                            data.MARKING_2D = dataReader.GetValue(25).ToString();
                            data.IF_DTTM = Convert.ToDateTime(dataReader.GetValue(26).ToString());
                            data.PCS_STRIP = dataReader.GetValue(27).ToString();
                            data.STRIP_BUNDLE = dataReader.GetValue(28).ToString();
                            data.LEVEL4 = dataReader.GetValue(29).ToString();

                            break;
                        }
                        dataReader.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs GetModelData Exception: {0}", ex.Message));
                return false;
            }
        }

        //JSON BASE - Server에 보고 후, 결과를 기다리지 않고 반환한다.
        public bool SetReportInfo(REQUEST_DATA data)
        {
            try
            {
                bool bRes = false;

                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("INSERT INTO {0}.request_info(REQ_STATE, MACHINE_CODE, REPORT_CODE, UP_MSG, REQUEST_TIME) VALUES(0, '{1}', '{2}', '{3}', now())",
                        strDBName, data.EQPT_CD, (int)data.REPORT_CODE, data.UP_MSG);

                    mysql.DBConnector().StartTrans();

                    if (mysql.DBConnector().Execute(query) > 0)
                    {
                        mysql.DBConnector().Commit();
                        bRes = true;
                    }
                    else
                        mysql.DBConnector().Rollback();
                }

                return bRes;
            }
            catch(Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs SetReportInfo Exception: {0}", ex.Message));
                return false;
            }
        }

        //서버의 Heartbeat 응답을 기다린다.
        public bool WaitHeartBeat(REQUEST_DATA data, int nTimeout)
        {
            try
            {
                bool bRes = false;

                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("SELECT REQ_STATE, DOWN_MSG FROM {0}.request_info WHERE MACHINE_CODE='{1}' AND REPORT_CODE='{2}' AND REQ_STATE>1",
                        strDBName, data.EQPT_CD, (int)data.REPORT_CODE);

                    int nFlag = -1;
                    int nLoopCount = 0;
                    int nWaitLoop = nTimeout * 1000 / nSleepTime;

                    IDataReader dataReader;
                    while (nFlag == -1)
                    {
                        dataReader = mysql.DBConnector().ExecuteQuery(query);
                        if (dataReader != null)
                        {
                            while (dataReader.Read())
                            {
                                nFlag = Convert.ToInt32(dataReader.GetValue(0).ToString());
                                bConnected = true;

                                break;
                            }
                            dataReader.Close();
                        }

                        if (nFlag == -1)
                        {
                            if (nLoopCount > nWaitLoop)
                            {
                                Log(string.Format("IGS-QueryManager.cs WaitHeartBeat Timeout {0}Sec", TimeOutSec));

                                bConnected = false;
                                break;
                            }

                            Thread.Sleep(nSleepTime);
                            nLoopCount++;
                        }
                    }
                    query = String.Format("DELETE FROM {0}.request_info WHERE MACHINE_CODE='{1}' AND REPORT_CODE='{2}'", strDBName, data.EQPT_CD, (int)data.REPORT_CODE);
                    mysql.DBConnector().Execute(query);

                    if (nFlag != -1)
                        bRes = true;
                }

                return bRes;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs WaitHeartBeat Exception: {0}", ex.Message));
                return false;
            }
        }

        //JSON BASE - 공통 Wait 함수
        public bool WaitReportInfo(REQUEST_DATA data, out string strDownMsg, out string errMsg)
        {
            strDownMsg = "";
            errMsg = "";

            try
            {
                bool bRes = false;

                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("SELECT REQ_STATE, DOWN_MSG FROM {0}.request_info WHERE MACHINE_CODE='{1}' AND REPORT_CODE='{2}' AND REQ_STATE>1",
                        strDBName, data.EQPT_CD, (int)data.REPORT_CODE);

                    int nFlag = -1;
                    int nLoopCount = 0;

                    IDataReader dataReader;
                    while (nFlag == -1)
                    {
                        dataReader = mysql.DBConnector().ExecuteQuery(query);
                        if (dataReader != null)
                        {
                            while (dataReader.Read())
                            {
                                nFlag = Convert.ToInt32(dataReader.GetValue(0).ToString());

                                if (nFlag == 2)
                                    strDownMsg = dataReader.GetValue(1).ToString();
                                else if (nFlag == 3)
                                    errMsg = dataReader.GetValue(1).ToString();

                                break;
                            }
                            dataReader.Close();
                        }

                        if (nFlag == -1)
                        {
                            if (nLoopCount > nLoop)
                            {
                                Log(string.Format("IGS-QueryManager.cs WaitReportInfo Timeout {0}Sec", TimeOutSec));
                                errMsg = "응답 대기시간 초과";

                                bConnected = false;
                                break;
                            }

                            Thread.Sleep(nSleepTime);
                            nLoopCount++;
                        }
                    }
                    query = String.Format("DELETE FROM {0}.request_info WHERE MACHINE_CODE='{1}' AND REPORT_CODE='{2}'", strDBName, data.EQPT_CD, (int)data.REPORT_CODE);
                    mysql.DBConnector().Execute(query);

                    if (nFlag == 2)
                        bRes = true;
                }

                return bRes;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs WaitReportInfo Exception: {0}", ex.Message));
                return false;
            }
        }

        //현재 설비 코드가 서버 상태 모니터링 테이블에 존재하는지 확인한다.
        //존재한다면 IGS Version을 업데이트한다.
        public bool CheckStateInfo_Client(string machineCode, string igsVersion)
        {
            try
            {
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("SELECT STATE FROM {0}.state_info WHERE MACHINE_CODE='{1}'", strDBName, machineCode);

                    bool bExist = false;
                    IDataReader dataReader = mysql.DBConnector().ExecuteQuery(query);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            bExist = true;
                            break;
                        }
                        dataReader.Close();
                    }

                    if (bExist)
                    {
                        query = String.Format("UPDATE {0}.state_info SET IGS_VERSION='{1}' WHERE MACHINE_CODE='{2}'", strDBName, igsVersion, machineCode);
                        mysql.DBConnector().StartTrans();

                        try
                        {
                            if (mysql.DBConnector().Execute(query) > 0)
                                mysql.DBConnector().Commit();
                            else
                                mysql.DBConnector().Rollback();
                        }
                        catch (Exception exc)
                        {
                            Log(string.Format("IGS-QueryManager.cs CheckStateInfo_Client Inner Exception: {0}", exc.Message));
                            if (mysql.DBConnector() != null)
                                mysql.DBConnector().Rollback();
                        }
                    }

                    return bExist;
                }
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs CheckStateInfo_Client Exception: {0}", ex.Message));
                return false;
            }
        }

        //서버에 등록되어있는 모든 설비의 상태정보 리스트를 반환한다.
        public bool GetStateInfo(out List<STATE_TABLE_DATA> stateList)
        {
            stateList = null;
            try
            {
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("SELECT MACHINE_CODE, bAUTO, STATE, LOT, OP_CODE, MODEL_CODE, CUST_CODE, EXC_CD, EXC_NM, EXC_START_TIME, LAST_REPORT_TIME, USER_NAME, USER_ID, USER_DEPT_CD, SHIFT_WORK, VERIFY_YN, VERIFY_USER, PARAM1 FROM {0}.state_info", strDBName);
                    IDataReader dataReader = mysql.DBConnector().ExecuteQuery(query);
                    if (dataReader != null)
                    {
                        stateList = new List<STATE_TABLE_DATA>();

                        while (dataReader.Read())
                        {
                            STATE_TABLE_DATA data = new STATE_TABLE_DATA();

                            data.MACHINE_CODE = dataReader.GetValue(0).ToString();
                            data.bAUTO = Convert.ToBoolean(dataReader.GetValue(1).ToString());
                            data.STATE = dataReader.GetValue(2).ToString();
                            data.ORDER = dataReader.GetValue(3).ToString();
                            data.OP_CODE = dataReader.GetValue(4).ToString();
                            data.MODEL_CODE = dataReader.GetValue(5).ToString();
                            data.CUST_CODE = dataReader.GetValue(6).ToString();
                            data.EXC_CD = dataReader.GetValue(7).ToString();
                            data.EXC_NM = dataReader.GetValue(8).ToString();
                            data.EXC_START_TIME = dataReader.GetValue(9).ToString();
                            data.LAST_REPORT_TIME = dataReader.GetValue(10).ToString();
                            data.USER_NAME = dataReader.GetValue(11).ToString();
                            data.USER_ID = dataReader.GetValue(12).ToString();
                            data.USER_DEPT_CD = dataReader.GetValue(13).ToString();
                            data.SHIFT_WORK = dataReader.GetValue(14).ToString();
                            data.VERIFY_YN = dataReader.GetValue(15).ToString();
                            data.VERIFY_USER = dataReader.GetValue(16).ToString();
                            data.PARAM1 = dataReader.GetValue(17).ToString();

                            stateList.Add(data);
                        }
                        dataReader.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs GetStateInfo Exception: {0}", ex.Message));
                return false;
            }
        }

        //입력된 설비코드에 해당하는 상태정보를 반환한다.
        public bool GetStateInfo(string machineCode, out STATE_TABLE_DATA data)
        {
            data = null;
            try
            {
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("SELECT MACHINE_CODE, bAUTO, STATE, LOT, OP_CODE, MODEL_CODE, CUST_CODE, EXC_CD, EXC_NM, EXC_START_TIME, LAST_REPORT_TIME, " +
                        "USER_NAME, USER_ID, USER_DEPT_CD, SHIFT_WORK, VERIFY_YN, VERIFY_USER, LAST_ITEM_CD, PARAM1 FROM {0}.state_info WHERE MACHINE_CODE='{1}'",
                        strDBName, machineCode);

                    IDataReader dataReader = mysql.DBConnector().ExecuteQuery(query);
                    if (dataReader != null)
                    {
                        data = new STATE_TABLE_DATA();

                        while (dataReader.Read())
                        {
                            data.MACHINE_CODE = dataReader.GetValue(0).ToString();
                            data.bAUTO = Convert.ToBoolean(dataReader.GetValue(1).ToString());
                            data.STATE = dataReader.GetValue(2).ToString();
                            data.ORDER = dataReader.GetValue(3).ToString();
                            data.OP_CODE = dataReader.GetValue(4).ToString();
                            data.MODEL_CODE = dataReader.GetValue(5).ToString();
                            data.CUST_CODE = dataReader.GetValue(6).ToString();
                            data.EXC_CD = dataReader.GetValue(7).ToString();
                            data.EXC_NM = dataReader.GetValue(8).ToString();
                            data.EXC_START_TIME = dataReader.GetValue(9).ToString();
                            data.LAST_REPORT_TIME = dataReader.GetValue(10).ToString();
                            data.USER_NAME = dataReader.GetValue(11).ToString();
                            data.USER_ID = dataReader.GetValue(12).ToString();
                            data.USER_DEPT_CD = dataReader.GetValue(13).ToString();
                            data.SHIFT_WORK = dataReader.GetValue(14).ToString();
                            data.VERIFY_YN = dataReader.GetValue(15).ToString();
                            data.VERIFY_USER = dataReader.GetValue(16).ToString();
                            data.LAST_ITEM_CD = dataReader.GetValue(17).ToString();
                            data.PARAM1 = dataReader.GetValue(18).ToString();
                        }
                        dataReader.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs GetStateInfo Exception: {0}", ex.Message));
                return false;
            }
        }

        //자동 유실 설정 리스트를 반환한다.
        public bool GetBreakTimeList(out List<BreakTimeDisplayData> timeList)
        {
            timeList = new List<BreakTimeDisplayData>();

            try
            {
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("SELECT Idx, FROM_TIME, TO_TIME, DETAIL, EXC_CD, EXC_NM, RUN_TIME, FROM_STD, TO_STD, STATE FROM {0}.breaktime_info", strDBName);

                    IDataReader dataReader = mysql.DBConnector().ExecuteQuery(query);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            BreakTimeDisplayData time = new BreakTimeDisplayData();

                            time.Idx = Convert.ToInt32(dataReader.GetValue(0).ToString());
                            time.from_time = TimeSpan.Parse(dataReader.GetValue(1).ToString()).ToString(@"hh\:mm");
                            time.to_time = TimeSpan.Parse(dataReader.GetValue(2).ToString()).ToString(@"hh\:mm");
                            time.detail = dataReader.GetValue(3).ToString();
                            time.exc_cd = dataReader.GetValue(4).ToString();
                            time.exc_nm = dataReader.GetValue(5).ToString();
                            time.run_time = Convert.ToInt32(dataReader.GetValue(6).ToString());
                            time.from_std = TimeSpan.Parse(dataReader.GetValue(7).ToString()).ToString(@"hh\:mm");
                            time.to_std = TimeSpan.Parse(dataReader.GetValue(8).ToString()).ToString(@"hh\:mm");
                            time.state = Convert.ToBoolean(dataReader.GetValue(9)) ? "승인" : "승인 대기";

                            timeList.Add(time);
                        }
                        dataReader.Close();
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs GetBreakTimeList Exception: {0}", ex.Message));
                return false;
            }
        }

        //자동 유실 설정을 추가한다.
        public bool SetBreakTime(BreakTimeDisplayData time, string userID)
        {
            try
            {
                bool bRet = false;
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("INSERT INTO {0}.breaktime_info(FROM_TIME, TO_TIME, DETAIL, EXC_CD, EXC_NM, RUN_TIME, FROM_STD, TO_STD, REGISTER) " +
                        "VALUES('{1}', '{2}', '{3}', '{4}', '{5}', {6}, '{7}', '{8}', '{9}')", strDBName, time.from_time, time.to_time,
                        time.detail, time.exc_cd, time.exc_nm, time.run_time, time.from_std, time.to_std, userID);

                    mysql.DBConnector().StartTrans();
                    try
                    {
                        if (mysql.DBConnector().Execute(query) > 0)
                        {
                            mysql.DBConnector().Commit();
                            bRet = true;
                        }
                        else
                            mysql.DBConnector().Rollback();
                    }
                    catch (Exception exc)
                    {
                        Log(string.Format("IGS-QueryManager.cs SetBreakTime Inner Exception: {0}", exc.Message));
                        if (mysql.DBConnector() != null)
                            mysql.DBConnector().Rollback();
                    }
                }

                return bRet;
            }
            catch(Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs SetBreakTime Exception: {0}", ex.Message));
                return false;
            }
        }

        //자동 유실 설정을 삭제한다.
        public bool DeleteBreakTimeInfo(int nIndex)
        {
            try
            {
                bool bRet = false;
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("DELETE FROM {0}.breaktime_info WHERE Idx={1}", strDBName, nIndex);

                    mysql.DBConnector().StartTrans();
                    try
                    {
                        if (mysql.DBConnector().Execute(query) > 0)
                        {
                            mysql.DBConnector().Commit();
                            bRet = true;
                        }
                        else
                            mysql.DBConnector().Rollback();
                    }
                    catch (Exception exc)
                    {
                        Log(string.Format("IGS-QueryManager.cs DeleteBreakTimeInfo Inner Exception: {0}", exc.Message));
                        if (mysql.DBConnector() != null)
                            mysql.DBConnector().Rollback();
                    }
                }

                return bRet;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs DeleteBreakTimeInfo Exception: {0}", ex.Message));
                return false;
            }
        }

        //불량코드별 책임공정(ex. E116 - PT) 리스트를 반환한다.
        public bool GetBadOpcodeInfo_Client(out Dictionary<string, List<string>> opList)
        {
            opList = new Dictionary<string, List<string>>();
            try
            {
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("SELECT BAD_CODE, OP_CODE FROM {0}.badcode_info WHERE OP_CODE IS NOT NULL", strDBName);

                    IDataReader dataReader = mysql.DBConnector().ExecuteQuery(query);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            string strCode = dataReader.GetValue(0).ToString();
                            string[] spt = dataReader.GetValue(1).ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            opList.Add(strCode, spt.ToList());
                        }
                        dataReader.Close();
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs GetBadOpcodeInfo_Client Exception: {0}", ex.Message));
                return false;
            }
        }

        //자동검사 공정코드를 반환한다.
        public bool GetInspOpcodeInfo_Client(out List<string> opList)
        {
            opList = new List<string>();
            try
            {
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("SELECT OP_CODE FROM {0}.opcode_info", strDBName);

                    IDataReader dataReader = mysql.DBConnector().ExecuteQuery(query);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            string strCode = dataReader.GetValue(0).ToString();
                            opList.Add(strCode);
                        }
                        dataReader.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs GetInspOpcodeInfo_Client Exception: {0}", ex.Message));
                return false;
            }
        }

        //불량코드(ex. B101 - AOI#1 불량) 리스트를 반환한다.
        public bool GetBadCodeInfo_Client(out Dictionary<string, string> codeList)
        {
            codeList = new Dictionary<string, string>();
            try
            {
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("SELECT BAD_CODE, DETAIL FROM {0}.badcode_info", strDBName);

                    IDataReader dataReader = mysql.DBConnector().ExecuteQuery(query);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            string strCode = dataReader.GetValue(0).ToString();
                            string strDetail = dataReader.GetValue(1).ToString();

                            codeList.Add(strCode, strDetail);
                        }
                        dataReader.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs GetBadCodeInfo_Client Exception: {0}", ex.Message));
                return false;
            }
        }

        //My설비 리스트를 불러온다.
        public bool GetMyMCList(string strID, out List<string> mcList)
        {
            mcList = new List<string>();
            try
            {
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("SELECT MC_LIST FROM {0}.user_mymc_info WHERE USER_ID='{1}'", strDBName, strID);

                    IDataReader dataReader = mysql.DBConnector().ExecuteQuery(query);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            string strList = dataReader.GetValue(0).ToString();
                            string[] spt = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string str in spt)
                                mcList.Add(str);

                            break;
                        }
                        dataReader.Close();
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs GetMyMCList Exception: {0}", ex.Message));
                return false;
            }
        }

        //My설비 리스트를 설정한다.
        public bool SetMyMCList(string strID, List<string> mcList)
        {
            bool bRet = false;

            try
            {
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    string strMC = "";
                    foreach (string mc in mcList)
                        strMC += mc + ",";

                    String query = String.Format("REPLACE INTO {0}.user_mymc_info (USER_ID, MC_LIST) VALUES ('{1}', '{2}')", strDBName, strID, strMC);

                    mysql.DBConnector().StartTrans();
                    try
                    {
                        if (mysql.DBConnector().Execute(query) > 0)
                        {
                            mysql.DBConnector().Commit();
                            bRet = true;
                        }
                        else
                            mysql.DBConnector().Rollback();
                    }
                    catch (Exception exc)
                    {
                        Log(string.Format("IGS-QueryManager.cs SetMyMCList Inner Exception: {0}", exc.Message));
                        if (mysql.DBConnector() != null)
                            mysql.DBConnector().Rollback();
                    }
                }

                return bRet;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs SetMyMCList Exception: {0}", ex.Message));
                return false;
            }
        }

        //설비 시작 시, 서버에 요청했던 잔여 기록을 삭제한다.
        public bool ClearMCHistory(string machineCode)
        {
            try
            {
                using (ConnectMysql mysql = new ConnectMysql())
                {
                    if (mysql.CreateConnection(strMysqlCon) == null)
                        return false;

                    String query = String.Format("DELETE FROM {0}.report_info WHERE MACHINE_CODE='{1}'; DELETE FROM {0}.lot_info WHERE MACHINE_CODE='{1}'; " +
                        "DELETE FROM {0}.request_info WHERE MACHINE_CODE='{1}'", strDBName, machineCode);
                    mysql.DBConnector().StartTrans();

                    try
                    {
                        if (mysql.DBConnector().Execute(query) > 0)
                            mysql.DBConnector().Commit();
                        else
                            mysql.DBConnector().Rollback();
                    }
                    catch (Exception exc)
                    {
                        Log(string.Format("IGS-QueryManager.cs ClearMCHistory Inner Exception: {0}", exc.Message));
                        if (mysql.DBConnector() != null)
                            mysql.DBConnector().Rollback();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log(string.Format("IGS-QueryManager.cs ClearMCHistory Exception: {0}", ex.Message));
                return false;
            }
        }
        #endregion

        public void Log(string msg)
        {
            ClientManager.Log(msg);
        }
    }
}
