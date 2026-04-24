using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Ink;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace IGS.Classes
{
    public delegate void ClientLogDelegate(string msg);
    public delegate void StateUpdateDelegate(bool bDelay);

    public class ClientManager
    {
        #region Member Variables.
        private QueryManager qManager;              //QueryManager  (Execute All Query)

        private string strActiveIP;                 //Active Server IP String
        private string strStandbyIP;                //Standby Server IP String
        private int m_mysqlPort;                    //Server Mysql DB Connection port
        public string strMachine;                   //Client Machine Name
        public string strSite;                      //Server Site (LF/BGA/BGA_AOI)

        public static ProductType productType;      //Strip / Shot
        public static string IgsVersion = "3.7";

        //private ClientSocket cSocket;             //Client Socket (Connection With Server Socket)
        //private int m_socketPort;                 //Server socket listening port

        public static ClientLogDelegate OnLog;      //Client Log Delegate Event
        public static StateUpdateDelegate OnState;  //State Update Delegate Event

        public static Dictionary<string, string> badcodeList = new Dictionary<string, string>();                //SAP BAD Code List (ex. Key:B101, Value:AOI #1)        
        public static Dictionary<string, List<string>> badOpList = new Dictionary<string, List<string>>();      //SAP CODE별 책임공정 기본값
        public static List<string> inspOpList = new List<string>();                                             //Site별 자동검사 공정코드 리스트

        public static string passWD = "hds";

        public bool bLocalTest = false;             //Local Simulation Flag
        #endregion

        #region Caching
        private static Hashtable hash = new Hashtable();        //Caching Data
        private static int nCacheTime = 60;                     //Caching 기능 유효 Interval
        #endregion

        public ClientManager(string serverIP, string backupIP, int serverMysqlPort, string machineName, string site)
        {
            this.strActiveIP = serverIP;
            this.strStandbyIP = backupIP;
            this.m_mysqlPort = serverMysqlPort;
            this.strMachine = machineName;
            this.strSite = site;

            if (strActiveIP.ToLower() == "localhost" || strActiveIP == "127.0.0.1")
                bLocalTest = true;
        }

        //Success: return 0
        //Socket Connection Fail: return -1
        //DB Connection Fail: return -2
        //No Data On MES: return -3
        public int Initialize()
        {
            //cSocket = new ClientSocket(strServerIP, m_socketPort);
            //if (!cSocket.Connect())
            //    return -1;

            qManager = new QueryManager();
            qManager.InitConnection(strActiveIP, m_mysqlPort.ToString(), strSite);

            if (bLocalTest)
                return 0;

            if (!ServerHeartbeat())
                return -2;

            if (!qManager.CheckStateInfo_Client(strMachine, IgsVersion))
                return -3;

            qManager.ClearMCHistory(strMachine);
            GetBadCodeInfo();
            GetBadOpcodeInfo();
            GetInspOpcodeInfo();

            StateCheck();

            return 0;
        }

        private void StateCheck()
        {
            //강제종료 확인
            STATE_TABLE_DATA curState;
            string errMsg;

            if (GetCurStateInfo(out curState) && curState != null)
            {
                //프로그램이 비가동 처리를 하지 못하고 강제 종료된 경우, 재시작할 때 비가동 처리를 해준다.
                if (curState.STATE == "RUN")
                    SetPOPDefaultLoss(0, out errMsg, AUTO_STOP_CODE.SW_ERROR);
            }
        }

        //Server의 Mysql Database 연결 상태를 확인한다.
        public bool DBConnectionCheck()
        {
            return qManager.ConnectionCheck();
        }

        //Server의 Demon Program 연결 상태를 확인한다.
        public bool ServerConnectionCheck()
        {
            return QueryManager.bConnected;
        }

        //입력된 IP에 대한 Network 연결 상태를 확인한다.
        public bool NetworkConnectionCheck(string strIP)
        {
            try
            {
                bool networkUp = NetworkInterface.GetIsNetworkAvailable();
                bool pingRes = false;

                if (networkUp)
                {
                    Ping pingSender = new Ping();
                    PingReply reply = pingSender.Send(strIP, 300);
                    pingRes = reply.Status == IPStatus.Success;
                }

                return pingRes;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //지정된 시간(초) 만큼 서버 응답을 대기한다. (Timeout)
        public void SetTimeout(int nSecond)
        {
            qManager.SetTimeOut(nSecond);
        }

        //지정된 시간(초) 만큼 Cache 기능이 유효하다. (ex. 60초 이내 동일한 요청에 대하여만 Caching 기능 사용)
        public void SetCachingTime(int nSecond)
        {
            nCacheTime = nSecond;
        }

        //Server의 응답 가능 상태를 확인한다. (nTimeout: Second)
        public bool ServerHeartbeat(int nTimeout = 10)
        {
            if (bLocalTest)
                return true;

            //현재 Connection이 Active인가
            bool bCurActive = qManager.strMysqlCon.Contains(strActiveIP);

            //복구 시도
            if (!bCurActive)
            {
                if (NetworkConnectionCheck(strActiveIP) && qManager.DBReconnectionCheck(strActiveIP, strMachine))
                {
                    //복구 성공
                    qManager.ChangeConnectionString(strActiveIP);
                    return true;
                }
            }

            if (!NetworkConnectionCheck(bCurActive ? strActiveIP : strStandbyIP) || !qManager.CheckDemonFlag())
            {
                //Auto Switching
                if (bCurActive)
                {
                    if (NetworkConnectionCheck(strStandbyIP) && qManager.DBReconnectionCheck(strStandbyIP, strMachine))
                    {
                        //Switching 성공
                        qManager.ChangeConnectionString(strStandbyIP);
                        return true;
                    }
                }

                return false;
            }

            REQUEST_DATA data = new REQUEST_DATA();
            data.EQPT_CD = strMachine;
            data.REPORT_CODE = ReportType.CHECK_HEARTBEAT;

            if (!qManager.SetReportInfo(data))
                return false;

            if (!qManager.WaitHeartBeat(data, nTimeout))
            {
                if (bCurActive)
                {
                    //Auto Switching
                    if (NetworkConnectionCheck(strStandbyIP) && qManager.DBReconnectionCheck(strStandbyIP, strMachine))
                    {
                        //Switching 성공
                        qManager.ChangeConnectionString(strStandbyIP);
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        //MES상 설비 상태를 UI에 업데이트한다.
        public void UpdateState()
        {
            if (bLocalTest)
                return;

            StateUpdateDelegate er = OnState;
            if (er != null)
                er(false);
        }

        //입력된 공정코드가 자동검사 공정인지 판단
        public bool CheckInspectCode(string processCode)
        {
            if (inspOpList.IndexOf(processCode) != -1)
                return true;

            return false;
        }

        //1분 이내 응답했던 동일 요청에 대한 Caching 기능
        private bool GetCacheInfo(object key, ref object value)
        {
            try
            {
                if (!QueryManager.bConnected)
                    return false;

                REQUEST_DATA req = (REQUEST_DATA)key;
                if (!hash.ContainsKey(req.REPORT_CODE))
                    return false;

                Tuple<REQUEST_DATA, object, DateTime> tup = (Tuple<REQUEST_DATA, object, DateTime>)hash[req.REPORT_CODE];
                if (tup.Item1.UP_MSG == req.UP_MSG && TimeSpanCheck(DateTime.Now, tup.Item3))
                {
                    value = tup.Item2;
                    return true;
                }

                return false;
            }
            catch { return false; }
        }

        //응답완료 후 Cache 데이터 갱신
        private void SetCacheInfo(object key, object value)
        {
            REQUEST_DATA req = (REQUEST_DATA)key;
            ReportType type = req.REPORT_CODE;

            if (hash.ContainsKey(type))
                hash[type] = new Tuple<REQUEST_DATA, object, DateTime>(req, value, DateTime.Now);
            else
                hash.Add(type, new Tuple<REQUEST_DATA, object, DateTime>(req, value, DateTime.Now));
        }

        //두 시간의 차이가 nSecond 이내인가
        private bool TimeSpanCheck(DateTime time1, DateTime time2)
        {
            if (Math.Abs(time1.Subtract(time2).TotalSeconds) < nCacheTime)
                return true;

            return false;
        }

        //MES model_info Table의 Primary Key는 (ModelName, CustomCode)이다.
        //ModelName으로만 조회할 경우, 각 고객사별 Model 기준정보 List를 반환한다.
        //Query를 실행하지 못했거나, 데이터가 없는 경우 out Parameter값은 null이다.
        public bool GetMESModelInfo(string modelName, out List<MODEL_RECIPE_DATA> infoList)
        {
            return qManager.GetModelData(modelName, out infoList);
        }

        //MES model_info Table의 Primary Key는 (ModelName, CustomCode)이다.
        //(ModelName, CustomCode)로 조회할 경우, 해당 고객의 Model 기준정보를 반환한다.
        //Query를 실행하지 못했거나, 데이터가 없는 경우 out Parameter값은 null이다.
        public bool GetMESModelInfo(string modelName, string customCode, out MODEL_RECIPE_DATA info)
        {
            return qManager.GetModelData(modelName, customCode, out info);
        }

        //서버에 요청을 보내고 응답을 기다린다.
        private bool GetServerAnswer(REQUEST_DATA data, out string strDownMsg, out string errMsg)
        {
            strDownMsg = "";
            errMsg = "";

            if (!qManager.SetReportInfo(data))
                return false;

            if (!qManager.WaitReportInfo(data, out strDownMsg, out errMsg))
                return false;

            return true;
        }

        //JSON 데이터를 string List로 변환
        private List<string> JsonToCodeList(string strDownMsg)
        {
            if (strDownMsg == "")
                return null;

            List<string> codeList = new List<string>();

            JArray codeArr = JArray.Parse(strDownMsg);
            for (int i = 0; i < codeArr.Count; i++)
                codeList.Add(codeArr[i].ToString());

            return codeList;
        }

        //조회를 시도하는 시점에 MES에 등록되어진 모델명, 고객코드 등의 정보를 반환한다.
        //Procedure를 실행하지 못했거나, 데이터가 없는 경우 out Parameter값은 null이다.
        //초기 버전은 Socket으로 Procedure를 실행하는 구조였으나, DB 통신으로 변경하고 Function구조를 변경함.
        public bool GetMESLotInfo(string lotNumber, out POP_LOT_DATA info)
        {
            info = null;

            if (bLocalTest)
                return false;

            lotNumber = lotNumber.Trim();

            REQUEST_DATA data = new REQUEST_DATA();
            data.EQPT_CD = strMachine;
            data.REPORT_CODE = ReportType.GET_LOTINFO;

            JObject request = new JObject();
            request.Add("WONO", lotNumber);

            string request_msg = JsonConvert.SerializeObject(request);
            data.UP_MSG = request_msg;

            object cache = new POP_LOT_DATA();
            if (GetCacheInfo(data, ref cache))
            {
                info = ((POP_LOT_DATA)cache).Clone();
                return true;
            }

            if (!ServerHeartbeat(2))
                return false;

            string strDownMsg;
            string errMsg;

            if (!GetServerAnswer(data, out strDownMsg, out errMsg))
                return false;

            if (strDownMsg != "")
            {
                info = new POP_LOT_DATA(strDownMsg);
                SetCacheInfo(data, info);
            }

            return true;
        }

        //Server에 공통 코드 요청 및 POP을 보고한다. - 보고 성공여부 및 Down Message 확인용
        //완료 보고 시, 보고에 성공하더라도 저수율(SBL)발생하면 Warning 정보가 있다. 해당 정보를 UI로 보여줘야 한다.
        public bool SetReportTwoWay(ReportType type, string requestMsg, out string errMsg, out string downMsg)
        {
            errMsg = "";
            downMsg = "";

            if (bLocalTest)
                return true;

            if (!ServerHeartbeat(2))
            {
                errMsg = "SERVER와 연결할 수 없습니다.";
                return false;
            }

            try
            {
                REQUEST_DATA data = new REQUEST_DATA();
                data.EQPT_CD = strMachine;
                data.REPORT_CODE = type;
                data.UP_MSG = requestMsg;

                return GetServerAnswer(data, out downMsg, out errMsg);
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("IGS - ClientManager.cs SetReportTwoWay Exception: {0}", ex.Message));
                return false;
            }
            finally
            {
                StateUpdateDelegate er = OnState;
                if (er != null)
                    er(true);
            }
        }

        //Server에 공통 코드 요청 및 POP을 보고한다. - 조회값을 따로 받지 않고 Flag만 확인하는 함수
        public bool SetReportOneWay(ReportType type, string requestMsg, out string errMsg)
        {
            errMsg = "";

            if (bLocalTest)
                return true;

            if (!ServerHeartbeat(2))
            {
                errMsg = "SERVER와 연결할 수 없습니다.";
                return false;
            }

            try
            {
                REQUEST_DATA data = new REQUEST_DATA();
                data.EQPT_CD = strMachine;
                data.REPORT_CODE = type;
                data.UP_MSG = requestMsg;

                string strDownMsg;
                return GetServerAnswer(data, out strDownMsg, out errMsg);
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("IGS - ClientManager.cs SetReportOneWay Exception: {0}", ex.Message));
                return false;
            }
            finally
            {
                StateUpdateDelegate er = OnState;
                if (er != null)
                    er(true);
            }
        }

        //Error Case
        /////Machine Error
        //1: Default(정의되지 않음), 2: Function Exception, 3: Server Disconnect
        /////Server Error
        //1: 설비 상태조회 실패, 2: MES Lot 조회 실패, 3: 비가동 종료보고 실패, 4: 시작보고 실패
        //5: 비가동 종료보고 실패(MES상 현재 설비가 다른 오더로 비가동 처리되어있음. 다른 오더를 중단보고 처리한 후 시작 해야함.-기존 POP 프로그램에서 처리)
        //6: 재작업 시 자동 비가동 보고 실패
        public bool SetPOPStartReport(ref POP_START_WINDOW_PARA para, bool bRestart, out string errMsg)
        {
            errMsg = "";

            if (bLocalTest)
                return true;

            if (!QueryManager.bConnected)
            {
                errMsg = "SERVER와 연결할 수 없습니다.";
                return false;
            }

            try
            {
                //기종교체 여부 확인
                CheckChangeModel(ref para);

                //JSON BASE
                REQUEST_DATA data = new REQUEST_DATA();
                data.EQPT_CD = strMachine;
                data.REPORT_CODE = ReportType.START_REPORT;

                JObject request = new JObject();
                request.Add("USER_ID", para.strOperator);
                request.Add("VERIFY_USER", para.strVerifyOper);             //2명 이상일 경우 ,로 구분한다 ex.홍길동(222222),김해성(111111)
                request.Add("WONO", para.strLot);
                request.Add("RESTART_YN", bRestart ? "Y" : "N");
                request.Add("CHANGE_YN", para.changeModelYN);
                request.Add("LOCAL_GRP", para.strGroup);
                request.Add("LOCAL_MODEL", para.strModel);
                request.Add("LOCAL_LOT", para.strLocalLot);
                request.Add("G_SPEC", para.ganji_spec);
                request.Add("G_LOT", para.ganji_lot);

                string request_msg = JsonConvert.SerializeObject(request);
                data.UP_MSG = request_msg;

                string strDownMsg;
                return GetServerAnswer(data, out strDownMsg, out errMsg);
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("IGS - ClientManager.cs SetPOPStartReport Exception: {0}", ex.Message));
                return false;
            }
            finally
            {
                StateUpdateDelegate er = OnState;
                if (er != null)
                    er(true);
            }
        }

        //기종교체 여부 팝업 처리
        private void CheckChangeModel(ref POP_START_WINDOW_PARA para)
        {
            try
            {
                para.changeModelYN = "N";       //Default

                //로트 변경 여부 확인
                STATE_TABLE_DATA state;
                if (GetCurStateInfo(out state) && state != null)
                {
                    if (para.strLot == state.ORDER)
                        return;

                    string lastModel = "";
                    if (state.LAST_ITEM_CD != "")
                        lastModel = state.LAST_ITEM_CD;
                    else
                        qManager.GetLastJobModel(strMachine, ref lastModel);

                    if (lastModel != "" && para.standModel != lastModel)
                    {
                        //기종교체 여부 확인
                        SubWindow.CHANGE_MODEL_CONFIRM dlg = new SubWindow.CHANGE_MODEL_CONFIRM();
                        dlg.ShowDialog();

                        if (dlg.nMode == 0)
                            para.changeModelYN = "Y";
                        else
                            para.changeModelYN = "N";
                    }
                }
            }
            catch { }
        }

        //일정시간동안 비가동 시, Default 유실코드로 비가동 중지시킨다.
        //nMinute: 비가동 판단 기준 (ex. 5: 5분 미입력 시)
        public bool SetPOPDefaultLoss(int nMinute, out string errMsg, AUTO_STOP_CODE stopCode = AUTO_STOP_CODE.DEFAULT)
        {
            errMsg = "";

            if (bLocalTest)
                return true;

            if (!QueryManager.bConnected)
            {
                errMsg = "SERVER와 연결할 수 없습니다.";
                return false;
            }

            try
            {
                REQUEST_DATA data = new REQUEST_DATA();
                data.EQPT_CD = strMachine;
                data.REPORT_CODE = ReportType.DEFAULT_EXCEPT_REPORT;

                JObject request = new JObject();
                request.Add("MINUTE", nMinute);
                request.Add("STOP_CODE", (int)stopCode);

                string request_msg = JsonConvert.SerializeObject(request);
                data.UP_MSG = request_msg;

                string strDownMsg;
                return GetServerAnswer(data, out strDownMsg, out errMsg);
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("IGS - ClientManager.cs SetPOPDefaultLoss Exception: {0}", ex.Message));
                return false;
            }
            finally
            {
                StateUpdateDelegate er = OnState;
                if (er != null)
                    er(true);
            }
        }

        //입력된 사번에 해당하는 사용자 정보를 반환한다.
        public bool GetUserInfo(string strID, out POP_USER_DATA userData, out string errMsg)
        {
            userData = null;
            errMsg = "";

            if (bLocalTest)
                return false;

            REQUEST_DATA data = new REQUEST_DATA();
            data.EQPT_CD = strMachine;
            data.REPORT_CODE = ReportType.GET_USERINFO;

            JObject request = new JObject();
            request.Add("USER_ID", strID);

            string request_msg = JsonConvert.SerializeObject(request);
            data.UP_MSG = request_msg;

            object cache = new POP_USER_DATA();
            if (GetCacheInfo(data, ref cache))
            {
                userData = ((POP_USER_DATA)cache).Clone();
                return true;
            }

            if (!QueryManager.bConnected && !ServerHeartbeat(2))
            {
                errMsg = "SERVER와 연결할 수 없습니다.";
                return false;
            }

            string strDownMsg;
            if (!GetServerAnswer(data, out strDownMsg, out errMsg))
                return false;

            if (strDownMsg != "")
            {
                userData = new POP_USER_DATA(strDownMsg);
                SetCacheInfo(data, userData);
            }

            return true;
        }

        //LF Verify 작업자 리스트를 반환한다.
        public bool GetVerifyUserList(out List<POP_USER_DATA> userList, out string errMsg)
        {
            userList = null;
            errMsg = "";

            if (bLocalTest)
                return false;

            REQUEST_DATA data = new REQUEST_DATA();
            data.EQPT_CD = strMachine;
            data.REPORT_CODE = ReportType.GET_VERIFY_USER_LIST;

            object cache = new List<POP_USER_DATA>();
            if (GetCacheInfo(data, ref cache))
            {
                userList = ((List<POP_USER_DATA>)cache).ConvertAll(u => u);
                return true;
            }

            if (!QueryManager.bConnected && !ServerHeartbeat(2))
            {
                errMsg = "SERVER와 연결할 수 없습니다.";
                return false;
            }

            string strDownMsg;
            if (!GetServerAnswer(data, out strDownMsg, out errMsg))
                return false;

            if (strDownMsg != "")
            {
                userList = new List<POP_USER_DATA>();

                JArray infoArr = JArray.Parse(strDownMsg);
                for (int i = 0; i < infoArr.Count; i++)
                    userList.Add(new POP_USER_DATA((JObject)infoArr[i]));

                SetCacheInfo(data, userList.ConvertAll(u => u));
            }

            return true;
        }

        //입력된 정보에 해당하는 시작보고 가능 여부 정보를 반환한다.
        public bool GetStartStateCheck(POP_START_STATE_INPUT_DATA input, out POP_START_STATE_OUTPUT_DATA output, out string errMsg)
        {
            output = null;
            errMsg = "";

            if (bLocalTest)
                return false;

            REQUEST_DATA data = new REQUEST_DATA();
            data.EQPT_CD = strMachine;
            data.REPORT_CODE = ReportType.GET_START_STATE_CHECK;

            JObject request = new JObject();
            request.Add("WONO", input.WONO);
            request.Add("OP_CODE", input.OPCD);
            request.Add("MC_CODE", input.EQPT_CD);

            string request_msg = JsonConvert.SerializeObject(request);
            data.UP_MSG = request_msg;

            object cache = new POP_START_STATE_OUTPUT_DATA();
            if (GetCacheInfo(data, ref cache))
            {
                output = ((POP_START_STATE_OUTPUT_DATA)cache).Clone();
                return true;
            }

            if (!QueryManager.bConnected && !ServerHeartbeat(2))
            {
                errMsg = "SERVER와 연결할 수 없습니다.";
                return false;
            }

            string strDownMsg;
            if (!GetServerAnswer(data, out strDownMsg, out errMsg))
                return false;

            if (strDownMsg != "")
            {
                output = new POP_START_STATE_OUTPUT_DATA(strDownMsg);
                SetCacheInfo(data, output);
            }

            return true;
        }

        //입력된 정보에 해당하는 시작보고 가능 여부 정보를 반환한다.
        //GetStartStateCheck 함수는 MES에서 판단하는 정보이며,GetOpStateCheck는 공정서버에서 판단하는 정보이다.
        //공정서버에서 시작 가능 여부를 판단하는 기준은 변경될 수 있기에, 별도의 함수로 분리한다.
        public bool GetOpStateCheck(string lotNumber, out POP_START_STATE_OUTPUT_DATA info, out string errMsg)
        {
            info = null;
            errMsg = "";

            if (bLocalTest)
                return true;

            lotNumber = lotNumber.Trim();

            REQUEST_DATA data = new REQUEST_DATA();
            data.EQPT_CD = strMachine;
            data.REPORT_CODE = ReportType.GET_OP_STATE_CHECK;

            JObject request = new JObject();
            request.Add("WONO", lotNumber);

            string request_msg = JsonConvert.SerializeObject(request);
            data.UP_MSG = request_msg;

            object cache = new POP_START_STATE_OUTPUT_DATA();
            if (GetCacheInfo(data, ref cache))
            {
                info = ((POP_START_STATE_OUTPUT_DATA)cache).Clone();
                return true;
            }

            if (!QueryManager.bConnected && !ServerHeartbeat(2))
            {
                errMsg = "SERVER와 연결할 수 없습니다.";
                return false;
            }

            string strDownMsg;
            if (!GetServerAnswer(data, out strDownMsg, out errMsg))
                return false;

            if (strDownMsg != "")
            {
                info = new POP_START_STATE_OUTPUT_DATA(strDownMsg);
                SetCacheInfo(data, info);
            }

            return true;
        }

        //입력된 Lot에 해당하는 동일 모델 최근 가동 리스트를 반환한다.
        public bool GetModelHistoryInfo(string strLot, out List<POP_RECENT_HISTORY_DATA> info, out string errMsg)
        {
            info = null;
            errMsg = "";

            if (bLocalTest)
                return false;

            REQUEST_DATA data = new REQUEST_DATA();
            data.EQPT_CD = strMachine;
            data.REPORT_CODE = ReportType.GET_MODEL_HISTORY;

            JObject request = new JObject();
            request.Add("WONO", strLot);

            string request_msg = JsonConvert.SerializeObject(request);
            data.UP_MSG = request_msg;

            object cache = new List<POP_RECENT_HISTORY_DATA>();
            if (GetCacheInfo(data, ref cache))
            {
                info = ((List<POP_RECENT_HISTORY_DATA>)cache).ConvertAll(d => d);
                return true;
            }

            if (!QueryManager.bConnected && !ServerHeartbeat(2))
            {
                errMsg = "SERVER와 연결할 수 없습니다.";
                return false;
            }

            string strDownMsg;
            if (!GetServerAnswer(data, out strDownMsg, out errMsg))
                return false;

            if (strDownMsg != "")
            {
                info = new List<POP_RECENT_HISTORY_DATA>();

                JArray infoArr = JArray.Parse(strDownMsg);
                for (int i = 0; i < infoArr.Count; i++)
                    info.Add(new POP_RECENT_HISTORY_DATA((JObject)infoArr[i]));

                SetCacheInfo(data, info.ConvertAll(d => d));
            }

            return true;
        }

        //입력된 Lot에 해당하는 변경점 정보를 반환한다.
        public bool GetChangePointInfo(string strLot, out List<POP_CHANGE_POINT_OUTPUT_DATA> info, out string errMsg)
        {
            info = null;
            errMsg = "";

            if (bLocalTest)
                return false;

            REQUEST_DATA data = new REQUEST_DATA();
            data.EQPT_CD = strMachine;
            data.REPORT_CODE = ReportType.GET_CHANGEPOINT;

            JObject request = new JObject();
            request.Add("WONO", strLot);

            string request_msg = JsonConvert.SerializeObject(request);
            data.UP_MSG = request_msg;

            object cache = new List<POP_CHANGE_POINT_OUTPUT_DATA>();
            if (GetCacheInfo(data, ref cache))
            {
                info = ((List<POP_CHANGE_POINT_OUTPUT_DATA>)cache).ConvertAll(d => d);
                return true;
            }

            if (!QueryManager.bConnected && !ServerHeartbeat(2))
            {
                errMsg = "SERVER와 연결할 수 없습니다.";
                return false;
            }

            string strDownMsg;
            if (!GetServerAnswer(data, out strDownMsg, out errMsg))
                return false;

            if (strDownMsg != "")
            {
                info = new List<POP_CHANGE_POINT_OUTPUT_DATA>();

                JArray infoArr = JArray.Parse(strDownMsg);
                for (int i = 0; i < infoArr.Count; i++)
                    info.Add(new POP_CHANGE_POINT_OUTPUT_DATA((JObject)infoArr[i]));

                SetCacheInfo(data, info.ConvertAll(d => d));
            }

            return true;
        }

        //입력된 Lot에 해당하는 Warning&Holding 정보를 반환한다. (현재 공정이 검사공정에 있지 않는 경우, 검사 공정 코드를 수동으로 입력받아야 한다.)
        public bool GetWarningInfo(string strLot, string strOPCode, out POP_WARNING_INFO info, out string errMsg)
        {
            info = null;
            errMsg = "";

            if (bLocalTest)
                return false;

            REQUEST_DATA data = new REQUEST_DATA();
            data.EQPT_CD = strMachine;
            data.REPORT_CODE = ReportType.GET_WARNING;

            JObject request = new JObject();
            request.Add("WONO", strLot);
            request.Add("OP_CODE", strOPCode);

            string request_msg = JsonConvert.SerializeObject(request);
            data.UP_MSG = request_msg;

            object cache = new POP_WARNING_INFO();
            if (GetCacheInfo(data, ref cache))
            {
                info = ((POP_WARNING_INFO)cache).Clone();
                return true;
            }

            if (!QueryManager.bConnected && !ServerHeartbeat(2))
            {
                errMsg = "SERVER와 연결할 수 없습니다.";
                return false;
            }

            string strDownMsg;
            if (!GetServerAnswer(data, out strDownMsg, out errMsg))
                return false;

            if (strDownMsg != "")
            {
                info = new POP_WARNING_INFO();
                info.ConvertInfo(strDownMsg);

                SetCacheInfo(data, info);
            }

            return true;
        }

        //입력된 Lot에 해당하는 책임공정 리스트를 반환한다.
        public bool GetOPCodeList(string strLot, out List<string> codeList, out string errMsg)
        {
            codeList = null;
            errMsg = "";

            if (bLocalTest)
                return false;

            REQUEST_DATA data = new REQUEST_DATA();
            data.EQPT_CD = strMachine;
            data.REPORT_CODE = ReportType.GET_OPCODE;

            JObject request = new JObject();
            request.Add("WONO", strLot);

            string request_msg = JsonConvert.SerializeObject(request);
            data.UP_MSG = request_msg;

            string strDownMsg;
            if (!GetServerAnswer(data, out strDownMsg, out errMsg))
                return false;

            codeList = JsonToCodeList(strDownMsg);
            return true;
        }

        //Sapcode별 책임공정 기본값을 확인한다. 지정된 값이 없을 경우 라우팅 가장 마지막 공정을 반환한다.
        public string GetDefaultOPCode(List<string> routing, string sapCode, string inspCode)
        {
            string defaultCode = inspCode == "" ? routing[routing.Count - 1] : inspCode;

            try
            {
                if (badOpList.ContainsKey(sapCode))
                {
                    int nIndex = -1;

                    for (int i = 0; i < badOpList[sapCode].Count; i++)
                    {
                        string curDefault = badOpList[sapCode][i];

                        if (curDefault.StartsWith("VI"))
                        {
                            string autoVICode = "";
                            foreach (string op in routing)
                            {
                                if (ReportMenuCtrl.clientManager.CheckInspectCode(op))
                                    autoVICode = op;
                            }

                            if (autoVICode == "")
                                continue;

                            int nCurIdx = routing.IndexOf(autoVICode);
                            if (curDefault.Contains("-"))
                                nCurIdx = nCurIdx == 0 ? 0 : nCurIdx - 1;

                            nIndex = Math.Max(nIndex, nCurIdx);
                        }
                        else
                        {
                            int nCurIdx = routing.IndexOf(routing.LastOrDefault(c => c.StartsWith(curDefault)));
                            nIndex = Math.Max(nIndex, nCurIdx);
                        }
                    }

                    if (nIndex == -1)
                        return defaultCode;
                    else
                        return routing[nIndex];
                }
                else
                    return defaultCode;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("IGS - ClientManager.cs GetDefaultOPCode Exception: {0}", ex.Message));
                return defaultCode;
            }
        }

        //자동 유실 설정이 가능한 유실코드 리스트를 반환한다. (Ex. 식사/휴식 등 B, C타입 코드)
        public bool GetBreakCodeList(string strID, out List<string> codeList, out string errMsg)
        {
            codeList = null;
            errMsg = "";

            if (bLocalTest)
                return false;

            REQUEST_DATA data = new REQUEST_DATA();
            data.EQPT_CD = strMachine;
            data.REPORT_CODE = ReportType.GET_BREAKTIME_CODE;

            JObject request = new JObject();
            request.Add("USER_ID", strID);

            string request_msg = JsonConvert.SerializeObject(request);
            data.UP_MSG = request_msg;

            string strDownMsg;
            if (!GetServerAnswer(data, out strDownMsg, out errMsg))
                return false;

            codeList = JsonToCodeList(strDownMsg);
            return true;
        }

        //유실코드 리스트를 반환한다.
        public bool GetLossCodeList(string strID, out List<string> codeList, out string errMsg)
        {
            codeList = null;
            errMsg = "";

            if (bLocalTest)
                return false;

            REQUEST_DATA data = new REQUEST_DATA();
            data.EQPT_CD = strMachine;
            data.REPORT_CODE = ReportType.GET_LOSSCODE;

            JObject request = new JObject();
            request.Add("USER_ID", strID);

            string request_msg = JsonConvert.SerializeObject(request);
            data.UP_MSG = request_msg;

            string strDownMsg;
            if (!GetServerAnswer(data, out strDownMsg, out errMsg))
                return false;

            codeList = JsonToCodeList(strDownMsg);
            return true;
        }

        //중단 사유 코드를 반환한다.
        public bool GetStopCodeList(string strID, out List<string> codeList, out string errMsg)
        {
            codeList = null;
            errMsg = "";

            if (bLocalTest)
                return false;

            REQUEST_DATA data = new REQUEST_DATA();
            data.EQPT_CD = strMachine;
            data.REPORT_CODE = ReportType.GET_STOPCODE;

            JObject request = new JObject();
            request.Add("USER_ID", strID);

            string request_msg = JsonConvert.SerializeObject(request);
            data.UP_MSG = request_msg;

            string strDownMsg;
            if (!GetServerAnswer(data, out strDownMsg, out errMsg))
                return false;

            codeList = JsonToCodeList(strDownMsg);
            return true;
        }

        //자동 유실 설정 리스트를 반환한다.
        public bool GetBreakTimeList(out List<BreakTimeDisplayData> timeList)
        {
            return qManager.GetBreakTimeList(out timeList);
        }

        //자동 유실 설정을 추가한다.
        public bool SetBreakTime(BreakTimeDisplayData time, string userID)
        {
            return qManager.SetBreakTime(time, userID);
        }

        //자동 유실 설정을 삭제한다.
        public bool DeleteBreakTime(int nIndex)
        {
            return qManager.DeleteBreakTimeInfo(nIndex);
        }

        //현재 설비의 상태정보를 반환한다.
        public bool GetCurStateInfo(out STATE_TABLE_DATA data)
        {
            return qManager.GetStateInfo(strMachine, out data);
        }

        //입력된 설비코드에 해당하는 상태정보를 반환한다.
        public bool GetStateInfo(string strMC, out STATE_TABLE_DATA data)
        {
            return qManager.GetStateInfo(strMC, out data);
        }

        //서버에 등록되어있는 모든 설비의 상태정보 리스트를 반환한다.
        public bool GetStateInfo(out List<STATE_TABLE_DATA> data)
        {
            return qManager.GetStateInfo(out data);
        }

        //불량코드(SAP 코드-ex.B123) 리스트를 읽어온다. 결과를 반환하지 않고, ClientManager badcodeList에 업데이트한다.
        public bool GetBadCodeInfo()
        {
            return qManager.GetBadCodeInfo_Client(out badcodeList);
        }

        //불량코드별 책임공정(SAP 코드-ex.PT) 리스트를 읽어온다. 결과를 반환하지 않고, ClientManager badOpList에 업데이트한다.
        public bool GetBadOpcodeInfo()
        {
            return qManager.GetBadOpcodeInfo_Client(out badOpList);
        }

        //자동검사 공정코드 리스트를 읽어온다. 결과를 반환하지 않고, ClientManager inspOpList 업데이트한다.
        public bool GetInspOpcodeInfo()
        {
            return qManager.GetInspOpcodeInfo_Client(out inspOpList);
        }

        //My설비 리스트를 불러온다.
        public bool GetMyMCList(string strID, out List<string> mcList)
        {
            return qManager.GetMyMCList(strID, out mcList);
        }

        //My설비 리스트를 설정한다.
        public bool SetMyMCList(string strID, List<string> mcList)
        {
            return qManager.SetMyMCList(strID, mcList);
        }

        public static void Log(string msg)
        {
            ClientLogDelegate er = OnLog;
            er(msg);
        }
    }
}
