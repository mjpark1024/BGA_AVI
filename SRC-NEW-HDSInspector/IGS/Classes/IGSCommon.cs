using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;

namespace IGS.Classes
{
    #region ENUM INFO
    public enum EventType
    {
        MAKE_LOSS = 0,
        GET_LASTTIME = 1,
        GET_INPUT_COUNT = 2,
        SET_LAST_USER = 3,
        GET_LAST_USER = 4
    }

    public enum ProductType
    {
        STRIP = 0,
        SHOT = 1
    }

    public enum ReportType
    {
        CHECK_HEARTBEAT = 100,          //서버 가동상태 확인

        START_REPORT = 101,             //시작보고
        EXCEPT_REPORT = 102,            //비가동보고
        COMPLETE_REPORT = 103,          //완료보고
        SUSPEND_REPORT = 104,           //중단보고
        DEFAULT_EXCEPT_REPORT = 112,    //자동 비가동 보고 (일정시간 비가동)

        WORK_ON_REPORT = 105,           //출근보고
        WORK_OFF_REPORT = 106,          //퇴근보고

        GET_SCRAPCODE = 107,            //불량코드(SAP) 정보 요청
        GET_USERINFO = 108,             //작업자 조회 요청
        GET_OPCODE = 109,               //책임공정 리스트 요청
        GET_LOSSCODE = 110,             //유실코드 정보 요청
        GET_STOPCODE = 111,             //중단사유코드 정보 요청
        GET_WARNING = 113,              //Warning&Holding 정보 요청
        GET_CHANGEPOINT = 114,          //변경점 정보 요청
        GET_MODEL_HISTORY = 115,        //동일 모델 최근 가동 리스트 요청
        GET_VERIFY_USER_INFO = 116,     //Verify 작업자 정보(1사람) 요청 - 사용하려면 기능 구현 필요
        GET_VERIFY_USER_LIST = 117,     //Verify 작업자 정보(리스트) 요청
        GET_START_STATE_CHECK = 118,    //시작보고 가능여부 확인 요청
        GET_LOTINFO = 119,              //LOT정보 요청
        GET_BREAKTIME_CODE = 120,       //자동 유실 코드 요청
        SET_REMK_MESSAGE = 121,         //특이사항 입력 요청
        SET_AI_CLASSIFICATION = 122,    //AI 분류 결과 입력 요청
        GET_OP_STATE_CHECK = 123        //현재 공정상태가 가동 가능상태인지 확인 요청
    }

    public enum AUTO_STOP_CODE
    {
        DEFAULT = 0,
        SCAN_WAIT = 1,
        VERIFY_WAIT = 2,
        SW_ERROR = 3
    }
    #endregion

    #region POP PROCEDURE PARAMETERS - COMMON
    public class POP_START_STATE_INPUT_DATA
    {
        public string WONO;
        public string OPCD;
        public string EQPT_CD;

        public POP_START_STATE_INPUT_DATA Clone()
        {
            POP_START_STATE_INPUT_DATA data = new POP_START_STATE_INPUT_DATA();

            data.WONO = this.WONO;
            data.OPCD = this.OPCD;
            data.EQPT_CD = this.EQPT_CD;

            return data;
        }
    }

    public class POP_USER_DATA
    {
        public string USERID;                   //작업자 사번
        public string USER_NM;                  //작업자명
        public string DEPT_CD;                  //부서코드
        public string DEPT_NM;                  //부서명
        public string POS_CD;                   //직급코드
        public string POS_NM;                   //직급명
        public string JIKGUN_CD;                //직군코드
        public string JIKGUN_NM;                //직군코드명
        public string WORK_TP;                  //근무형태코드
        public string WORK_TP_NM;               //근무형태명

        public POP_USER_DATA() { }

        public POP_USER_DATA(string strPara) : this(JObject.Parse(strPara)) { }

        public POP_USER_DATA(JObject info)
        {
            this.USERID = info["USER_ID"].ToString();
            this.USER_NM = info["USER_NM"].ToString();
            this.DEPT_CD = info["DEPT_CD"].ToString();
            this.DEPT_NM = info["DEPT_NM"].ToString();
            this.POS_CD = info["POS_CD"].ToString();
            this.POS_NM = info["POS_NM"].ToString();
            this.JIKGUN_CD = info["JIKGUN_CD"].ToString();
            this.JIKGUN_NM = info["JIKGUN_NM"].ToString();
            this.WORK_TP = info["WORK_TP"].ToString();
            this.WORK_TP_NM = info["WORK_TP_NM"].ToString();
        }

        public string ToPara()
        {
            JObject info = ToJObject();
            return JsonConvert.SerializeObject(info);
        }

        public JObject ToJObject()
        {
            JObject info = new JObject();

            info.Add("USER_ID", this.USERID);
            info.Add("USER_NM", this.USER_NM);
            info.Add("DEPT_CD", this.DEPT_CD);
            info.Add("DEPT_NM", this.DEPT_NM);
            info.Add("POS_CD", this.POS_CD);
            info.Add("POS_NM", this.POS_NM);
            info.Add("JIKGUN_CD", this.JIKGUN_CD);
            info.Add("JIKGUN_NM", this.JIKGUN_NM);
            info.Add("WORK_TP", this.WORK_TP);
            info.Add("WORK_TP_NM", this.WORK_TP_NM);

            return info;
        }

        public POP_USER_DATA Clone()
        {
            POP_USER_DATA data = new POP_USER_DATA();

            data.USERID = this.USERID;
            data.USER_NM = this.USER_NM;
            data.DEPT_CD = this.DEPT_CD;
            data.DEPT_NM = this.DEPT_NM;
            data.POS_CD = this.POS_CD;
            data.POS_NM = this.POS_NM;
            data.JIKGUN_CD = this.JIKGUN_CD;
            data.JIKGUN_NM = this.JIKGUN_NM;
            data.WORK_TP = this.WORK_TP;
            data.WORK_TP_NM = this.WORK_TP_NM;

            return data;
        }
    }

    public class POP_LOT_DATA
    {
        public string WONO;                     //오더번호
        public string ITEM_CD;                  //모델명
        public string CUST_CD;                  //고객코드
        public string OP_CD;                    //공정코드
        public string WK_CNT;                   //작업횟수
        public string OP_STAT;                  //공정상태
        public string ITS_ORDER;                //ITS 오더번호
        public string PRE_OUT_CNT;              //전공정 완성수량
        public string SP_YN;                    //간지입력여부 (Y/N)
        public string PLATE;                    //포장기준정보
        public string GANJI_SPEC;               //간지사양

        public POP_LOT_DATA() { }

        public POP_LOT_DATA(string strPara)
        {
            JObject info = JObject.Parse(strPara);

            this.WONO = info["WONO"].ToString();
            this.ITEM_CD = info["ITEM_CD"].ToString();
            this.CUST_CD = info["CUST_CD"].ToString();
            this.OP_CD = info["OP_CD"].ToString();
            this.WK_CNT = info["WK_CNT"].ToString();
            this.OP_STAT = info["OP_STAT"].ToString();
            this.ITS_ORDER = info["ITS_ORDER"].ToString();
            this.PRE_OUT_CNT = info["PRE_OUT_CNT"].ToString();
            this.SP_YN = info["SP_YN"].ToString();
            this.PLATE = info["PLATE"].ToString();
            this.GANJI_SPEC = info["GANJI_SPEC"].ToString();
        }

        public POP_LOT_DATA Clone()
        {
            POP_LOT_DATA data = new POP_LOT_DATA();

            data.WONO = this.WONO;
            data.ITEM_CD = this.ITEM_CD;
            data.CUST_CD = this.CUST_CD;
            data.OP_CD = this.OP_CD;
            data.WK_CNT = this.WK_CNT;
            data.OP_STAT = this.OP_STAT;
            data.ITS_ORDER = this.ITS_ORDER;
            data.PRE_OUT_CNT = this.PRE_OUT_CNT;
            data.SP_YN = this.SP_YN;
            data.PLATE = this.PLATE;
            data.GANJI_SPEC = this.GANJI_SPEC;

            return data;
        }

        public JObject ToJObject()
        {
            JObject obj = new JObject();
            obj.Add("WONO", this.WONO);
            obj.Add("ITEM_CD", this.ITEM_CD);
            obj.Add("CUST_CD", this.CUST_CD);
            obj.Add("OP_CD", this.OP_CD);
            obj.Add("WK_CNT", this.WK_CNT);
            obj.Add("OP_STAT", this.OP_STAT);
            obj.Add("ITS_ORDER", this.ITS_ORDER);
            obj.Add("PRE_OUT_CNT", this.PRE_OUT_CNT);
            obj.Add("SP_YN", this.SP_YN);
            obj.Add("PLATE", this.PLATE);
            obj.Add("GANJI_SPEC", this.GANJI_SPEC);

            return obj;
        }
    }

    public class POP_RECENT_HISTORY_DATA
    {
        public string ITEM_CD;
        public string EQPT_CD;
        public string PROD_DT;
        public int NUM;

        public POP_RECENT_HISTORY_DATA() { }

        public POP_RECENT_HISTORY_DATA(JObject obj)
        {
            this.ITEM_CD = obj["ITEM_CD"].ToString();
            this.EQPT_CD = obj["EQPT_CD"].ToString();
            this.PROD_DT = obj["PROD_DT"].ToString();
            this.NUM = Convert.ToInt32(obj["NUM"].ToString());
        }

        public POP_RECENT_HISTORY_DATA Clone()
        {
            POP_RECENT_HISTORY_DATA data = new POP_RECENT_HISTORY_DATA();

            data.ITEM_CD = this.ITEM_CD;
            data.EQPT_CD = this.EQPT_CD;
            data.PROD_DT = this.PROD_DT;
            data.NUM = this.NUM;

            return data;
        }

        public JObject ToJObject()
        {
            JObject obj = new JObject();
            obj.Add("ITEM_CD", this.ITEM_CD);
            obj.Add("EQPT_CD", this.EQPT_CD);
            obj.Add("PROD_DT", this.PROD_DT);
            obj.Add("NUM", this.NUM);

            return obj;
        }
    }

    public class POP_START_STATE_OUTPUT_DATA
    {
        public string CHECK_CD = "";
        public string CHECK_MSG = "";

        public POP_START_STATE_OUTPUT_DATA() { }

        public POP_START_STATE_OUTPUT_DATA(string strPara)
        {
            JObject obj = JObject.Parse(strPara);

            this.CHECK_CD = obj["CHECK_CD"].ToString();
            this.CHECK_MSG = obj["CHECK_MSG"].ToString();
        }

        public POP_START_STATE_OUTPUT_DATA Clone()
        {
            POP_START_STATE_OUTPUT_DATA data = new POP_START_STATE_OUTPUT_DATA();

            data.CHECK_CD = this.CHECK_CD;
            data.CHECK_MSG = this.CHECK_MSG;

            return data;
        }

        public string ToPara()
        {
            JObject obj = new JObject();
            obj.Add("CHECK_CD", this.CHECK_CD);
            obj.Add("CHECK_MSG", this.CHECK_MSG);

            return JsonConvert.SerializeObject(obj);
        }
    }

    public class POP_CHANGE_POINT_OUTPUT_DATA
    {
        public string CHG_DIV;
        public string CHG_TITLE;
        public string ITEM_CD;
        public string OP_CD;
        public string WONO;
        public string EQPT_CD;
        public string REG_DT;

        public POP_CHANGE_POINT_OUTPUT_DATA() { }

        public POP_CHANGE_POINT_OUTPUT_DATA(string strPara)
        {
            string[] spt = strPara.Split(';');

            this.CHG_DIV = spt[0];
            this.CHG_TITLE = spt[1];
            this.ITEM_CD = spt[2];
            this.OP_CD = spt[3];
            this.WONO = spt[4];
            this.EQPT_CD = spt[5];
            this.REG_DT = spt[6];
        }

        public POP_CHANGE_POINT_OUTPUT_DATA(JObject obj)
        {
            this.CHG_DIV = obj["CHG_DIV"].ToString();
            this.CHG_TITLE = obj["CHG_TITLE"].ToString();
            this.ITEM_CD = obj["ITEM_CD"].ToString();
            this.OP_CD = obj["OP_CD"].ToString();
            this.WONO = obj["WONO"].ToString();
            this.EQPT_CD = obj["EQPT_CD"].ToString();
            this.REG_DT = obj["REG_DT"].ToString();
        }

        public POP_CHANGE_POINT_OUTPUT_DATA Clone()
        {
            POP_CHANGE_POINT_OUTPUT_DATA data = new POP_CHANGE_POINT_OUTPUT_DATA();

            data.CHG_DIV = this.CHG_DIV;
            data.CHG_TITLE = this.CHG_TITLE;
            data.ITEM_CD = this.ITEM_CD;
            data.OP_CD = this.OP_CD;
            data.WONO = this.WONO;
            data.EQPT_CD = this.EQPT_CD;
            data.REG_DT = this.REG_DT;

            return data;
        }

        public JObject ToJObject()
        {
            JObject obj = new JObject();
            obj.Add("CHG_DIV", this.CHG_DIV);
            obj.Add("CHG_TITLE", this.CHG_TITLE);
            obj.Add("ITEM_CD", this.ITEM_CD);
            obj.Add("OP_CD", this.OP_CD);
            obj.Add("WONO", this.WONO);
            obj.Add("EQPT_CD", this.EQPT_CD);
            obj.Add("REG_DT", this.REG_DT);

            return obj;
        }
    }

    public class POP_WARNING_INFO
    {
        public string RESULT_CODE;
        public string RESULT_MSG;
        public string RESULT_DTL;
        public List<POP_WARNING_MSG> WARNING_INFO = new List<POP_WARNING_MSG>();

        public void AddWarningMsg(string msg)
        {
            string[] spt = msg.Split(new string[] { "^>" }, StringSplitOptions.RemoveEmptyEntries);

            WARNING_INFO = new List<POP_WARNING_MSG>();
            for (int i = 0; i < spt.Length; i++)
                WARNING_INFO.Add(new POP_WARNING_MSG(spt[i]));
        }

        public string ToPara()
        {
            JObject obj = new JObject();
            obj.Add("RESULT_CODE", this.RESULT_CODE == "0000" ? "G" : RESULT_CODE);
            obj.Add("RESULT_MSG", this.RESULT_MSG);
            obj.Add("RESULT_DTL", this.RESULT_DTL);

            JArray msgArr = new JArray();
            for (int i = 0; i < WARNING_INFO.Count; i++)
                msgArr.Add(WARNING_INFO[i].ToJObject());
            obj.Add("INFO", msgArr);

            return JsonConvert.SerializeObject(obj);
        }

        public void ConvertInfo(string str)
        {
            JObject obj = JObject.Parse(str);

            this.RESULT_CODE = obj["RESULT_CODE"].ToString();
            this.RESULT_MSG = obj["RESULT_MSG"].ToString();
            this.RESULT_DTL = obj["RESULT_DTL"].ToString();

            JArray msgArr = (JArray)obj["INFO"];
            foreach (JObject info in msgArr)
            {
                POP_WARNING_MSG msg = new POP_WARNING_MSG(info);
                WARNING_INFO.Add(msg.Clone());
            }
        }

        public POP_WARNING_INFO Clone()
        {
            POP_WARNING_INFO data = new POP_WARNING_INFO();

            data.RESULT_CODE = this.RESULT_CODE;
            data.RESULT_MSG = this.RESULT_MSG;
            data.RESULT_DTL = this.RESULT_DTL;
            data.WARNING_INFO = this.WARNING_INFO.ConvertAll(c => c);

            return data;
        }
    }

    public class POP_WARNING_MSG
    {
        public string RULE;             //공정진행 (WARNING / HOLDING)
        public string CONTROL_NUM;      //관리번호 (11자리)
        public string REGIST;           //등록자
        public List<string> MSG;        //세부내용

        public POP_WARNING_MSG() { }

        public POP_WARNING_MSG(string str)
        {
            str = str.Replace("^>", "");
            str = str.Replace("\t", "");
            string[] spt = str.Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries);

            string[] inner = spt[0].Split(',');
            for (int i = 0; i < inner.Length; i++)
            {
                if (inner[i].Contains("공정진행"))
                    this.RULE = inner[i].Split('-')[1];
                else if (inner[i].Contains("관리번호"))
                    this.CONTROL_NUM = inner[i].Split(':')[1];
                else if (inner[i].Contains("등록자"))
                    this.REGIST = inner[i].Split(':')[1];
            }

            MSG = new List<string>();
            for (int i = 1; i < spt.Length; i++)
                MSG.Add(spt[i]);
        }

        public POP_WARNING_MSG(JObject obj)
        {
            this.RULE = obj["RULE"].ToString();
            this.CONTROL_NUM = obj["CONTROL_NUM"].ToString();
            this.REGIST = obj["REGISTER"].ToString();

            string strMsg = obj["MSG"].ToString();
            this.MSG = strMsg.Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public JObject ToJObject()
        {
            JObject obj = new JObject();
            obj.Add("RULE", this.RULE);
            obj.Add("CONTROL_NUM", this.CONTROL_NUM);
            obj.Add("REGISTER", this.REGIST);

            string strMsg = "";
            for (int i = 0; i < MSG.Count; i++)
            {
                if (i != 0)
                    strMsg += "^";

                strMsg += MSG[i];
            }
            obj.Add("MSG", strMsg);

            return obj;
        }

        public POP_WARNING_MSG Clone()
        {
            POP_WARNING_MSG data = new POP_WARNING_MSG();

            data.RULE = this.RULE;
            data.CONTROL_NUM = this.CONTROL_NUM;
            data.REGIST = this.REGIST;
            data.MSG = this.MSG;

            return data;
        }

    }



    //Oracle, Mysql model_info Table Data
    public class MODEL_RECIPE_DATA
    {
        public string ITEM_GRP;         //제품군
        public string ITEM_KIND_CD;     //제품류
        public string ITEM_CD;          //제품코드
        public string ITEM_NM;          //제품명
        public string SPEC_NO;          //SPEC번호
        public string REV_LVL;          //REVISION
        public string CUST_CD;          //고객코드
        public string CUST_NM;          //고객명
        public string NEW_DIV;          //SPEC최신구분(N최신)
        public string XOUT_YIELD;       //XOUT 수율
        public string UPP;              //열 (Strip 길이방향 Unit수)
        public string UNIT_PER_ROW;     //행 (Strip 폭방향 Unit수)
        public string ARRAY;            //배열
        public string INLINE_MULTI_WK;  //생산라인수
        public string ITEM_LEN;         //제품길이
        public string STRIP_WIDTH;      //제품폭
        public string PITCH;            //피치
        public string QM_MARK_DIV;      //2D 마킹적용여부
        public string QM_MARK_CUST;     //2D 고객 Mapping 생성
        public string MOLD_GATE;        //몰드게이트 방향
        public string STRIP_WAY;        //2D Strip 방향
        public string AOI_IN;           //AOI 내층
        public string AOI_OUT;          //AOI 외층
        public string BBT;              //BBT 여부
        public string SKIPDATA;         //SkipData
        public string AVI;              //AVI 여부
        public string MARKING_2D;       //2D Marking 여부
        public DateTime IF_DTTM;        //데이터 IF 일자
        public string PCS_STRIP;        //유닛/스트립
        public string STRIP_BUNDLE;     //스트립/번들
        public string LEVEL4;           //LF 제품 타입 (ex.PPF)

        public MODEL_RECIPE_DATA Clone()
        {
            MODEL_RECIPE_DATA data = new MODEL_RECIPE_DATA();

            data.ITEM_GRP = this.ITEM_GRP;
            data.ITEM_KIND_CD = this.ITEM_KIND_CD;
            data.ITEM_CD = this.ITEM_CD;
            data.ITEM_NM = this.ITEM_NM;
            data.SPEC_NO = this.SPEC_NO;
            data.REV_LVL = this.REV_LVL;
            data.CUST_CD = this.CUST_CD;
            data.CUST_NM = this.CUST_NM;
            data.NEW_DIV = this.NEW_DIV;
            data.XOUT_YIELD = this.XOUT_YIELD;
            data.UPP = this.UPP;
            data.UNIT_PER_ROW = this.UNIT_PER_ROW;
            data.ARRAY = this.ARRAY;
            data.INLINE_MULTI_WK = this.INLINE_MULTI_WK;
            data.ITEM_LEN = this.ITEM_LEN;
            data.STRIP_WIDTH = this.STRIP_WIDTH;
            data.PITCH = this.PITCH;
            data.QM_MARK_DIV = this.QM_MARK_DIV;
            data.QM_MARK_CUST = this.QM_MARK_CUST;
            data.MOLD_GATE = this.MOLD_GATE;
            data.STRIP_WAY = this.STRIP_WAY;
            data.AOI_IN = this.AOI_IN;
            data.AOI_OUT = this.AOI_OUT;
            data.BBT = this.BBT;
            data.AVI = this.AVI;
            data.MARKING_2D = this.MARKING_2D;
            data.IF_DTTM = this.IF_DTTM;
            data.PCS_STRIP = this.PCS_STRIP;
            data.STRIP_BUNDLE = this.STRIP_BUNDLE;
            data.LEVEL4 = this.LEVEL4;

            return data;
        }
    }

    public class REQUEST_DATA
    {
        public int REQ_STATE;                   //Request 상태 (0: 대기, 1: Running, 2: Done, 3: Fail)
        public string EQPT_CD;                  //설비 코드
        public ReportType REPORT_CODE;          //Report Type
        public string UP_MSG = "";              //Client->Server JSON Message
        public string DOWN_MSG = "";            //Server->Client JSON Message

        public REQUEST_DATA Clone()
        {
            REQUEST_DATA data = new REQUEST_DATA();

            data.REQ_STATE = this.REQ_STATE;
            data.EQPT_CD = this.EQPT_CD;
            data.REPORT_CODE = this.REPORT_CODE;
            data.UP_MSG = this.UP_MSG;
            data.DOWN_MSG = this.DOWN_MSG;

            return data;
        }
    }

    public class STATE_TABLE_DATA
    {
        public string MACHINE_CODE;             //설비 코드
        public bool bAUTO;                      //POP 자동화 연동 여부
        public string STATE;                    //설비 상태
        public string ORDER;                    //오더 번호
        public string OP_CODE;                  //공정 코드
        public string MODEL_CODE;               //모델 코드
        public string CUST_CODE;                //고객 코드
        public string EXC_CD;                   //유실 코드
        public string EXC_NM;                   //유실 코드 명칭
        public string EXC_START_TIME;           //유실 보고 시작일시
        public string LAST_REPORT_TIME;         //최근 보고일시
        public string USER_NAME;                //작업자 이름
        public string USER_ID;                  //작업자 사번
        public string VERIFY_USER;              //Verify 작업자 정보
        public string USER_DEPT_CD;             //작업자 부서코드
        public string SHIFT_WORK;               //현재 설비의 교대조 정보 (A0: 3교대, A1: 주간, A2: 2교대, A3: 맞교대)        
        public string VERIFY_YN;                //Verify 적용 설비 여부 Y/N
        public string LAST_ITEM_CD;             //마지막 작업 모델
        public string IGS_VERSION;              //IGS Version
        public string PARAM1;                   //Reserved.

        public STATE_TABLE_DATA Clone()
        {
            STATE_TABLE_DATA data = new STATE_TABLE_DATA();

            data.MACHINE_CODE = this.MACHINE_CODE;
            data.bAUTO = this.bAUTO;
            data.STATE = this.STATE;
            data.ORDER = this.ORDER;
            data.OP_CODE = this.OP_CODE;
            data.MODEL_CODE = this.MODEL_CODE;
            data.CUST_CODE = this.CUST_CODE;
            data.EXC_CD = this.EXC_CD;
            data.EXC_NM = this.EXC_NM;
            data.EXC_START_TIME = this.EXC_START_TIME;
            data.LAST_REPORT_TIME = this.LAST_REPORT_TIME;
            data.USER_NAME = this.USER_NAME;
            data.USER_ID = this.USER_ID;
            data.VERIFY_USER = this.VERIFY_USER;
            data.USER_DEPT_CD = this.USER_DEPT_CD;
            data.SHIFT_WORK = this.SHIFT_WORK;            
            data.VERIFY_YN = this.VERIFY_YN;
            data.LAST_ITEM_CD = this.LAST_ITEM_CD;
            data.IGS_VERSION = this.IGS_VERSION;
            data.PARAM1 = this.PARAM1;

            return data;
        }
    }

    #endregion POP PROCEDURE PARAMETERS - COMMON

    #region POP PROCEDURE PARAMETERS - CLIENT ONLY
    public class POP_COMPLETE_WINDOW_PARA
    {
        public int nInput;                      //투입수량
        public int nOutput;                     //완성수량

        //불량 수량 리스트 (SAP Code, 수량) ex. B123, 111
        public Dictionary<string, int> badList = new Dictionary<string, int>();
    }

    public class POP_START_WINDOW_PARA
    {
        public string strGroup;                 //검사기 메인프로그램에 등록된 그룹명
        public string strModel;                 //검사기 메인프로그램에 등록된 모델명 (서버에서 그룹, 모델명을 조합하여 결과폴더 찾아가기 위함)
        public string strLocalLot;              //검사기 내부 경로 상 사용하는 전체 오더명 (ex.123456789J01, 1234567891회차)
        public string strLot;                   //오더번호
        public string strOperator;              //작업자 사번
        public string strVerifyOper;            //Verify 작업자 사번
        public string ganji_spec;               //간지 사양코드
        public string ganji_lot;                //간지 오더번호
        public string standModel;               //표준모델명 - 기종교체 확인용
        public string changeModelYN;            //기종교체여부 입력값
        public bool bNormalLot;                 //정상적인 Lot인지 확인
        public bool bRestart;                   //재시작하는 Lot인지 확인
    }

    #endregion POP PROCEDURE PARAMETERS - CLIENT ONLY

    #region IGS UI PARAMETERS - BINDING (CLIENT ONLY)
    public class BadCountData : NotifyPropertyChanged
    {
        public string bad_code
        {
            get { return m_badcode; }
            set
            {
                m_badcode = value;
                Notify("bad_code");
            }
        }

        public string bad_name
        {
            get { return m_badname; }
            set
            {
                m_badname = value;
                Notify("bad_name");
            }
        }

        public string op_code
        {
            get { return m_opcode; }
            set
            {
                m_opcode = value;
                Notify("op_code");
            }
        }

        public int ngCount
        {
            get { return m_count; }
            set
            {
                m_count = value;
                Notify("ngCount");
            }
        }

        public List<string> codeList
        {
            get { return m_codeList; }
            set
            {
                m_codeList = value;
                Notify("codeList");
            }
        }

        private string m_badcode;
        private string m_badname;
        private string m_opcode;
        private int m_count;
        private List<string> m_codeList = new List<string>();

        public BadCountData Clone()
        {
            BadCountData data = new BadCountData();

            data.bad_code = this.bad_code;
            data.bad_name = this.bad_name;
            data.op_code = this.op_code;
            data.ngCount = this.ngCount;

            foreach (string str in data.codeList)
                data.codeList.Add(str);

            return data;
        }
    }

    public class CountDisplayData : NotifyPropertyChanged
    {
        public string mc_code
        {
            get { return m_mc_code; }
            set
            {
                m_mc_code = value;
                Notify("mc_code");
            }
        }

        public int InputCount
        {
            get { return m_inputCnt; }
            set
            {
                m_inputCnt = value;
                GoodCount = InputCount - NGTotalCount;
                Notify("InputCount");
            }
        }

        public int GoodCount
        {
            get { return m_goodCnt; }
            set
            {
                m_goodCnt = value;
                Notify("GoodCount");
            }
        }

        public int NGTotalCount
        {
            get { return m_ngCnt; }
            set
            {
                m_ngCnt = value;
                GoodCount = InputCount - NGTotalCount;
                Notify("NGTotalCount");
            }
        }

        public string lossCode
        {
            get { return m_lossCode; }
            set
            {
                m_lossCode = value;
                Notify("lossCode");
            }
        }

        public List<string> lossItems
        {
            get { return m_items; }
            set
            {
                m_items = value;
                Notify("lossItems");
            }
        }

        public List<BadCountData> badCounts = new List<BadCountData>();

        private string m_mc_code;
        private int m_inputCnt;
        private int m_goodCnt;
        private int m_ngCnt;
        private string m_lossCode;
        private List<string> m_items = new List<string>();

        public CountDisplayData Clone()
        {
            CountDisplayData data = new CountDisplayData();

            data.mc_code = this.mc_code;
            data.InputCount = this.InputCount;
            data.GoodCount = this.GoodCount;
            data.NGTotalCount = this.NGTotalCount;
            data.lossCode = this.lossCode;

            data.badCounts = new List<BadCountData>();
            foreach (BadCountData count in this.badCounts)
                data.badCounts.Add(count);

            foreach (string loss in this.lossItems)
                data.lossItems.Add(loss);

            return data;
        }
    }

    public class ModelHistoryDisplayData : NotifyPropertyChanged
    {
        public int idx
        {
            get { return m_idx; }
            set
            {
                m_idx = value;
                Notify("idx");
            }
        }

        public string model_code
        {
            get { return m_modelCode; }
            set
            {
                m_modelCode = value;
                Notify("model_code");
            }
        }

        public string mc_code
        {
            get { return m_mcCode; }
            set
            {
                m_mcCode = value;
                Notify("mc_code");
            }
        }

        public string prod_dt
        {
            get { return m_prodDt; }
            set
            {
                m_prodDt = value;
                Notify("prod_dt");
            }
        }

        private int m_idx;
        private string m_modelCode;
        private string m_mcCode;
        private string m_prodDt;

        public ModelHistoryDisplayData Clone()
        {
            ModelHistoryDisplayData data = new ModelHistoryDisplayData();

            data.idx = this.idx;
            data.model_code = this.model_code;
            data.mc_code = this.mc_code;
            data.prod_dt = this.prod_dt;

            return data;
        }
    }

    public class ChangepointDisplayData : NotifyPropertyChanged
    {
        public string chg_div
        {
            get { return m_chg_div; }
            set
            {
                m_chg_div = value;
                Notify("chg_div");
            }
        }

        public string chg_title
        {
            get { return m_chg_title; }
            set
            {
                m_chg_title = value;
                Notify("chg_title");
            }
        }

        public string item_cd
        {
            get { return m_item_cd; }
            set
            {
                m_item_cd = value;
                Notify("item_cd");
            }
        }

        public string op_cd
        {
            get { return m_op_cd; }
            set
            {
                m_op_cd = value;
                Notify("op_cd");
            }
        }

        public string eqpt_cd
        {
            get { return m_eqpt_cd; }
            set
            {
                m_eqpt_cd = value;
                Notify("eqpt_cd");
            }
        }

        public string reg_dt
        {
            get { return m_reg_dt; }
            set
            {
                m_reg_dt = value;
                Notify("reg_dt");
            }
        }

        private string m_chg_div;
        private string m_chg_title;
        private string m_item_cd;
        private string m_op_cd;
        private string m_eqpt_cd;
        private string m_reg_dt;

        public ChangepointDisplayData Clone()
        {
            ChangepointDisplayData data = new ChangepointDisplayData();

            data.chg_div = this.chg_div;
            data.chg_title = this.chg_title;
            data.item_cd = this.item_cd;
            data.op_cd = this.op_cd;
            data.eqpt_cd = this.eqpt_cd;
            data.reg_dt = this.reg_dt;

            return data;
        }
    }

    public class WarningDisplayData : NotifyPropertyChanged
    {
        public string control_num
        {
            get { return m_control_num; }
            set
            {
                m_control_num = value;
                Notify("control_num");
            }
        }

        public string rule
        {
            get { return m_rule; }
            set
            {
                m_rule = value;
                Notify("rule");
            }
        }

        public string regist
        {
            get { return m_regist; }
            set
            {
                m_regist = value;
                Notify("regist");
            }
        }

        public string msg
        {
            get { return m_msg; }
            set
            {
                m_msg = value;
                Notify("msg");
            }
        }

        public SolidColorBrush color
        {
            get { return m_color; }
            set
            {
                m_color = value;
                Notify("color");
            }
        }

        private string m_control_num;
        private string m_rule;
        private string m_regist;
        private string m_msg;
        private SolidColorBrush m_color = new SolidColorBrush(Colors.Black);

        public WarningDisplayData Clone()
        {
            WarningDisplayData data = new WarningDisplayData();

            data.control_num = this.control_num;
            data.rule = this.rule;
            data.regist = this.regist;
            data.msg = this.msg;
            data.color = this.color;

            return data;
        }
    }

    public class UserSelectDisplayData : NotifyPropertyChanged
    {
        public bool bChecked
        {
            get { return m_bChecked; }
            set
            {
                m_bChecked = value;
                Notify("bChecked");
            }
        }

        public string userID
        {
            get { return m_userID; }
            set
            {
                m_userID = value;
                Notify("userID");
            }
        }

        public string userName
        {
            get { return m_userName; }
            set
            {
                m_userName = value;
                Notify("userName");
            }
        }

        private bool m_bChecked;
        private string m_userID;
        private string m_userName;

        public UserSelectDisplayData Clone()
        {
            UserSelectDisplayData data = new UserSelectDisplayData();

            data.bChecked = this.bChecked;
            data.userID = this.userID;
            data.userName = this.userName;

            return data;
        }
    }

    public class VerifyUserDisplayData : NotifyPropertyChanged
    {
        public bool bChecked
        {
            get { return m_bChecked; }
            set
            {
                m_bChecked = value;
                Notify("bChecked");
            }
        }

        public string mc_code
        {
            get { return m_mc_code; }
            set
            {
                m_mc_code = value;
                Notify("mc_code");
            }
        }

        public string verify_yn
        {
            get { return m_verify_yn; }
            set
            {
                m_verify_yn = value;
                Notify("verify_yn");
            }
        }

        public string verify_user
        {
            get { return m_verify_user; }
            set
            {
                m_verify_user = value;
                Notify("verify_user");
            }
        }

        private bool m_bChecked;
        private string m_mc_code;
        private string m_verify_yn;
        private string m_verify_user;

        public VerifyUserDisplayData Clone()
        {
            VerifyUserDisplayData data = new VerifyUserDisplayData();

            data.bChecked = this.bChecked;
            data.mc_code = this.mc_code;
            data.verify_yn = this.verify_yn;
            data.verify_user = this.verify_user;

            return data;
        }
    }

    public class BreakTimeDisplayData : NotifyPropertyChanged
    {
        public int Idx
        {
            get { return m_nIndex; }
            set
            {
                m_nIndex = value;
                Notify("Idx");
            }
        }

        public string from_time
        {
            get { return m_fromTime; }
            set
            {
                m_fromTime = value;
                Notify("from_time");
            }
        }

        public string to_time
        {
            get { return m_toTime; }
            set
            {
                m_toTime = value;
                Notify("to_time");
            }
        }

        public string detail
        {
            get { return m_detail; }
            set
            {
                m_detail = value;
                Notify("detail");
            }
        }

        public string exc_cd
        {
            get { return m_exc_cd; }
            set
            {
                m_exc_cd = value;
                Notify("exc_cd");
            }
        }

        public string exc_nm
        {
            get { return m_exc_nm; }
            set
            {
                m_exc_nm = value;
                Notify("exc_nm");
            }
        }

        public int run_time
        {
            get { return m_runTime; }
            set
            {
                m_runTime = value;
                Notify("run_time");
                Notify("run_time_str");
            }
        }

        public string run_time_str
        {
            get { return string.Format("{0}분", m_runTime); }
        }

        public string from_std
        {
            get { return m_fromStd; }
            set
            {
                m_fromStd = value;
                Notify("from_std");
                Notify("calc_time");
            }
        }

        public string to_std
        {
            get { return m_toStd; }
            set
            {
                m_toStd = value;
                Notify("to_std");
                Notify("calc_time");
            }
        }

        public string calc_time
        {
            get { return string.Format("{0} ~ {1}", from_std, to_std); }
        }

        public string state
        {
            get { return m_state; }
            set
            {
                m_state = value;
                Notify("state");
            }
        }

        private int m_nIndex;
        private string m_fromTime;
        private string m_toTime;
        private string m_detail;
        private string m_exc_cd;
        private string m_exc_nm;
        private int m_runTime;
        private string m_fromStd;
        private string m_toStd;
        private string m_state;

        public BreakTimeDisplayData Clone()
        {
            BreakTimeDisplayData data = new BreakTimeDisplayData();

            data.Idx = this.Idx;
            data.from_time = this.from_time;
            data.to_time = this.to_time;
            data.detail = this.detail;
            data.exc_cd = this.exc_cd;
            data.exc_nm = this.exc_nm;
            data.run_time = this.run_time;
            data.from_std = this.from_std;
            data.to_std = this.to_std;
            data.state = this.state;

            return data;
        }
    }

    public class MyMCDisplayData : NotifyPropertyChanged
    {
        public bool bChecked
        {
            get { return m_bChecked; }
            set
            {
                m_bChecked = value;
                Notify("bChecked");
            }
        }

        public string mc_code
        {
            get { return m_mc_code; }
            set
            {
                m_mc_code = value;
                Notify("mc_code");
            }
        }

        private bool m_bChecked;
        private string m_mc_code;

        public MyMCDisplayData Clone()
        {
            MyMCDisplayData data = new MyMCDisplayData();

            data.bChecked = this.bChecked;
            data.mc_code = this.mc_code;

            return data;
        }
    }

    public class StateDisplayData : NotifyPropertyChanged
    {
        public bool bChecked
        {
            get { return m_bChecked; }
            set
            {
                m_bChecked = value;
                Notify("bChecked");
            }
        }

        public string mc_code
        {
            get { return m_mc_code; }
            set
            {
                m_mc_code = value;
                Notify("mc_code");
            }
        }

        public string imagePath
        {
            get { return m_imagePath; }
            set
            {
                m_imagePath = value;
                Notify("imagePath");
            }
        }

        public string state
        {
            get { return m_state; }
            set
            {
                m_state = value;
                Notify("state");
            }
        }

        public string order
        {
            get { return m_order; }
            set
            {
                m_order = value;
                Notify("order");
            }
        }

        public string exc_code
        {
            get { return m_exc_code; }
            set
            {
                m_exc_code = value;
                Notify("exc_code");
            }
        }

        public string exc_name
        {
            get { return m_exc_name; }
            set
            {
                m_exc_name = value;
                Notify("exc_name");
            }
        }

        public string exc_time
        {
            get { return m_exc_time; }
            set
            {
                m_exc_time = value;
                Notify("exc_time");
            }
        }

        public string user_name
        {
            get { return m_user_name; }
            set
            {
                m_user_name = value;
                Notify("user_name");
            }
        }

        private bool m_bChecked;
        private string m_mc_code;
        private string m_imagePath;
        private string m_state;
        private string m_order;
        private string m_exc_code;
        private string m_exc_name;
        private string m_exc_time;
        private string m_user_name;

        public StateDisplayData Clone()
        {
            StateDisplayData data = new StateDisplayData();

            data.bChecked = this.bChecked;
            data.mc_code = this.mc_code;
            data.imagePath = this.imagePath;
            data.state = this.state;
            data.order = this.order;
            data.exc_code = this.exc_code;
            data.exc_name = this.exc_name;
            data.m_exc_time = this.m_exc_time;
            data.user_name = this.user_name;

            return data;
        }

        public bool Compare(StateDisplayData data)
        {
            if (this.imagePath != data.imagePath || this.state != data.state || this.order != data.order || this.exc_code != data.exc_code ||
                this.exc_name != data.exc_name || this.exc_time != data.exc_time || this.user_name != data.user_name)
                return false;

            return true;
        }

        public void Replace(StateDisplayData data)
        {
            this.imagePath = data.imagePath;
            this.state = data.state;
            this.order = data.order;
            this.exc_code = data.exc_code;
            this.exc_name = data.exc_name;
            this.exc_time = data.exc_time;
            this.user_name = data.user_name;
        }
    }
    #endregion IGS UI PARAMETERS - BINDING (CLIENT ONLY)

    #region SOCKET DEFINITION
    public static class ProtocolDefine
    {
        public const int PIN = 1;
        public const int REQ = 2;
        public const int ACK = 3;
        public const int NAK = 4;
        public const int CLOSE = 5;

        public const int REQ_LOT_PROC = 101;

        public const int ACK_LOT_PROC = 201;
    }

    public class ServerSocketReceiveEventArgs : EventArgs
    {
        public int socketID
        {
            get { return m_socketID; }
        }

        public Socket socket
        {
            get { return m_socket; }
        }

        public QueryManager qManager
        {
            get { return m_qManager; }
        }

        public int ReceivedBytes
        {
            get { return receivedBytes; }
        }

        public byte[] ReceivedData
        {
            get { return receivedData; }
        }

        private readonly int m_socketID;
        private readonly Socket m_socket;
        private readonly QueryManager m_qManager;
        private readonly int receivedBytes;
        private readonly byte[] receivedData;

        public ServerSocketReceiveEventArgs(int socketID, Socket socket, QueryManager qManager, int receivedBytes, byte[] receivedData)
        {
            this.m_socketID = socketID;
            this.m_socket = socket;
            this.m_qManager = qManager;
            this.receivedBytes = receivedBytes;
            this.receivedData = receivedData;
        }
    }

    public class ClientSocketReceiveEventArgs : EventArgs
    {
        private readonly int receiveBytes;
        private readonly byte[] receivedData;

        public ClientSocketReceiveEventArgs(int areceivedBytes, byte[] areceivedData)
        {
            this.receiveBytes = areceivedBytes;
            this.receivedData = areceivedData;
        }

        public int ReceivedBytes
        {
            get { return receiveBytes; }
        }

        public byte[] ReceivedData
        {
            get { return receivedData; }
        }
    }
    #endregion  SOCKET DEFINITION

    public class TimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strOrigin = value.ToString();

            if (strOrigin == "") return "";

            string strRes = string.Format("{0}.{1:D2}.{2:D2} {3:D2}:{4:D2}:{5:D2}", strOrigin.Substring(0, 4), strOrigin.Substring(4, 2), strOrigin.Substring(6, 2),
                strOrigin.Substring(8, 2), strOrigin.Substring(10, 2), strOrigin.Substring(12, 2));
            return strRes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string strPropertyName)
        {
            PropertyChangedEventHandler p = PropertyChanged;
            if (p != null)
            {
                p(this, new PropertyChangedEventArgs(strPropertyName));
            }
        }
    }

}
