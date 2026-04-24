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

using Common.Drawing.InspectionInformation;
using RVS.Generic.Insp;
using System;
using System.Collections.Generic;
using System.Windows;


namespace PCS
{
    public static class VisionDefinition
    {
        public static readonly double GRAB_IMAGE_SCALE = 0.25; // 1/4 scale.

        ////////////////////////////////////////////

        public const int LINESCAN_NOGRAB = 0;
        public const int LINESCAN_FORWARD = 1;
        public const int LINESCAN_REVERSE = 2;

        public const int GRABBER_FILE = 0;
        public const int GRABBER_SAPERA = 1;
        public const int GRABBER_MULTICAM = 2;
        public const int GRABBER_MIL = 3;

        public const int LIVE_IMAGE_DEPTH = 8;
        public const int IMAGE_PACKET_SIZE = 1000000; // 10000000; (10MB)

        ////////////////////////////////////////////

        public const int PING = 1;

        public const int STATUS_LOG_1 = 11;
        public const int STATUS_LOG_2 = 12;
        public const int STATUS_LOG_3 = 13;
        public const int ERROR_LOG_1 = 21;
        public const int ERROR_LOG_2 = 22;
        public const int ERROR_LOG_3 = 23;

        public const int INIT_VISION = 31;
        public const int CLEAR_VISION = 32;
        public const int SET_STATUS_LOG_LEVEL = 33;
        public const int SET_ERROR_LOG_LEVEL = 34;
        public const int SET_PAGE_DELAY = 35;
        public const int REQ_RESULT_INFO = 36;
        public const int ACK_RESULT_INFO = 37;
        public const int SET_GAIN = 38;
        public const int SET_VERIFY_INFO = 39;
        public const int SET_STRENGTH = 40;
        public const int SET_GRAB_SIDE = 48;
        public const int SET_INSP_MODE = 49;

        public const int GRAB = 51;
        public const int INSPECT = 52;
        public const int REGISTER = 53;
        public const int CLEAR_INSP_ITEMS = 54;
        public const int CLEAR_INSP_RESULTS = 55;
        public const int GRAB_AND_INSPECT = 56;
        public const int GRAB_READY = 57;

        public const int ADD_SECTION = 61;   
        public const int REQ_SECTION = 62;
        public const int REQ_SECTION_COLOR = 621;
        public const int ACK_SECTION = 63;
        public const int ACK_SECTION_COLOR = 631;
        public const int ADD_ROI = 64;
        public const int ADD_INSP = 65;
        public const int SEND_REF_IMAGE = 66;
        public const int SEND_REF_IMAGE_COLOR = 661;
        public const int SEND_SEC_IMAGE = 67;

        public const int REQ_CENTER_LINE_INFO = 68;
        public const int ACK_CENTER_LINE_INFO = 69;
        public const int SEND_CENTER_LINE_INFO = 70;

        public const int GRAB_DONE = 71;
        public const int INSPECT_DONE = 72;
        public const int REGISTER_DONE = 73;
        public const int REGISTER_FAIL = 74;
        public const int REGISTER_PROGRESS = 75;
        public const int REQ_GRAB_IMAGE_RGB = 77;
        public const int ACK_GRAB_IMAGE_RGB = 78;

        public const int REQ_BLOB_IMAGE = 79;
        public const int ACK_BLOB_IMAGE = 80;

        public const int REQ_LIVE_IMAGE = 81;
        public const int ACK_LIVE_IMAGE = 82;
        public const int REQ_GRAB_IMAGE = 83;
        public const int ACK_GRAB_IMAGE = 84;
        public const int REQ_RESULT_COUNT = 85;
        public const int ACK_RESULT_COUNT = 86;
        public const int REQ_RESULT_DATA = 87;
        public const int ACK_RESULT_DATA = 88;
        public const int REQ_RESULT_IMAGE = 89;
        public const int ACK_RESULT_IMAGE = 90;
        public const int ACK_RESULT_IMAGE_COLOR = 900;

        public const int SEND_VERT_HISTO = 91;
        public const int ACK_VERT_HISTO = 92;
        public const int SEND_HORI_HISTO = 93;
        public const int ACK_HORI_HISTO = 94;

        public const int ADD_STRIPALIGN = 96;
        public const int ACK_STRIPALIGN = 97;
        public const int REQ_SHIFT_DATA = 98;
        public const int ACK_SHIFT_DATA = 99;

        public const int ADD_IDMARK = 111;
        public const int ACK_IDMARK = 112;
        public const int REQ_IDMARK_RESULT = 113;
        public const int ACK_IDMARK_RESULT = 114;

        public const int REQ_BALL_INFO = 121;
        public const int ACK_BALL_INFO = 122;
        public const int SEND_BALL_INFO = 123;
        public const int REQ_CORNER_IMAGE = 124; // MJ : 260129 ICS-AVI 좌표계 연동 (영상 원점부터 10mm 이미지 전송 요청)
        public const int ACK_CORNER_IMAGE = 125; // MJ : 260129 ICS-AVI 좌표계 연동 (이미지 전송 실패/성공 Flag)

        public const int REQ_ICS_OFFSET = 132; // MJ : 260317 ICS Tilt 계산용 스트립 좌상단 & 하단 유닛 offset 회신
        public const int ACK_ICS_OFFSET = 133; // MJ : 260317 ICS Tilt 계산용 스트립 좌상단 & 하단 유닛 offset 회신
        public const int ADD_UNITS_ICS = 611;
        ////////////////////////////////////////////

        public const int SEC_TYPE_GLOBAL = 1;
        public const int SEC_TYPE_RAIL = 2;
        public const int SEC_TYPE_BLOCK = 3;
        public const int SEC_TYPE_UNIT = 4;

        ////////////////////////////////////////////

        public const int DRAW_TYPE_NONE = 0;		// 없음
        public const int DRAW_TYPE_RECT = 1;		// 사각형
        public const int DRAW_TYPE_ELLIPSE = 2;		// 원형
        public const int DRAW_TYPE_CROSS = 3;		// X 모양 (단순 Diplay용)
        public const int DRAW_TYPE_LINE = 4;		// 선형
        public const int DRAW_TYPE_POLY = 5;		// 다각형

        ////////////////////////////////////////////

        public const int BREAK_TYPE_CONTINUE = 0;
        public const int BREAK_TYPE_UNIT = 1;
        public const int BREAK_TYPE_STRIP = 2;

        ////////////////////////////////////////////

        public const int INSP_TYPE_GLOBAL_ALIGN = 101;	// Strip Align (Fiducial Insp.)
        public const int INSP_TYPE_GLOBAL_CROSS = 102;	// Strip Cross Pattern
        public const int INSP_TYPE_GLOBAL_PSR_SHIFT = 103;	// Strip Ball Psr Shift
        public const int INSP_TYPE_GLOBAL_BALL_PSR = 104;	// Strip Ball Psr Shift
        public const int INSP_TYPE_GLOBAL_ALIGNED_MASK = 111;	// Strip Aligned Mask
        public const int INSP_TYPE_GLOBAL_INTENSITY = 112;	// Strip Intensity
        public const int INSP_TYPE_RAIL_ALIGN = 201;	// Strip Align (Fiducial Insp.)
        public const int INSP_TYPE_RAIL_CROSS = 202;	// Strip Cross Pattern
        public const int INSP_TYPE_RAIL_PSR_SHIFT = 203;	// Strip Ball Psr Shift
        public const int INSP_TYPE_RAIL_BALL_PSR = 204;	// Strip Ball Psr Shift
        public const int INSP_TYPE_RAIL_ALIGNED_MASK = 211;	// Strip Aligned Mask
        public const int INSP_TYPE_RAIL_INTENSITY = 212;	// Strip Intensity
        public const int INSP_TYPE_BLOCK_ALIGN = 301;	// Strip Align (Fiducial Insp.)
        public const int INSP_TYPE_BLOCK_CROSS = 302;	// Strip Cross Pattern
        public const int INSP_TYPE_BLOCK_PSR_SHIFT = 303;	// Strip Ball Psr Shift
        public const int INSP_TYPE_BLOCK_BALL_PSR = 304;	// Strip Ball Psr Shift
        public const int INSP_TYPE_BLOCK_ALIGNED_MASK = 311;	// Strip Aligned Mask
        public const int INSP_TYPE_BLOCK_INTENSITY = 312;	// Strip Intensity
        public const int INSP_TYPE_UNIT_ALIGN = 401;	// Unit Align (Fiducial Insp.)
        public const int INSP_TYPE_UNIT_CROSS = 402;	// Unit 십자가 (Cross Pattern)
        public const int INSP_TYPE_UNIT_PSR_SHIFT = 403;	// PSR Shift (PSR Shift X, Y)
        public const int INSP_TYPE_UNIT_BALL_PSR = 404;	// Unit Ball PSR Shift
        public const int INSP_TYPE_UNIT_TOLERANCE = 405;	// 치수 공차 검사 (PSR Shift Y, Punch Shift X,Y)
        public const int INSP_TYPE_UNIT_PUNCH_SHIFT = 406;	// Punch Shift (Punch Shift X, Y)
        public const int INSP_TYPE_UNIT_FIDU = 411;	// 인식키 검사 (Aligned Mask)
        public const int INSP_TYPE_UNIT_RAW_NONPLATE = 412;	// 원소재 미도금 (Aligned Mask)
        public const int INSP_TYPE_UNIT_RAW_MARKING = 413;	// 원소재 검사 (Aligned Mask)
        public const int INSP_TYPE_UNIT_LEAD_SHAPE_DIR = 414;
        public const int INSP_TYPE_UNIT_LEAD_SHAPE_CL = 415;
        public const int INSP_TYPE_UNIT_GROOVE = 416;
        public const int INSP_TYPE_UNIT_SHAPE_HALF = 417;
        public const int INSP_TYPE_UNIT_SURFACE_HALF = 418;
        public const int INSP_TYPE_UNIT_DOWN_SET = 419;
        public const int INSP_TYPE_UNIT_TAPE = 420;
        public const int INSP_TYPE_UNIT_PLATE = 421;
        public const int INSP_TYPE_UNIT_SURFACE = 422;
        public const int INSP_TYPE_UNIT_SPACE = 423;
        public const int INSP_TYPE_UNIT_BOND_STATE = 424;	// Bond-Pad 외관 (Aligned Mask)
        public const int INSP_TYPE_UNIT_BOND_SHAPE = 425;	// Bond-Pad 패턴 (Shape with Dir)
        public const int INSP_TYPE_UNIT_BOND_LIFT = 426;	// Bond-Pad 리드 들림 (Aligned Mask)
        public const int INSP_TYPE_UNIT_BOND_INTERVAL = 427;	// Bond-Pad 리드 간격 (Interval)
        public const int INSP_TYPE_UNIT_CORE_CRACK = 428;	// Core Crack (Intensity)
        public const int INSP_TYPE_UNIT_BALL_PATTERN = 429;	// Ball Pattern (Ball Pattern)
        public const int INSP_TYPE_UNIT_BALL_LAND = 430;	// Ball 영역 (Aligned Mask)
        public const int INSP_TYPE_UNIT_PSR_ALIGNED_MASK = 431;	// PSR 영역 (Aligned Mask)
        public const int INSP_TYPE_UNIT_PUNCH_BURR = 432;	// Punch Burr (Intensity)
        public const int INSP_TYPE_UNIT_DAMBAR = 433;	// Dam Bar 검사(DamBar)
        public const int INSP_TYPE_UNIT_VENTHOLE = 434;	// Vent-Hole (Vent-Hole)
        public const int INSP_TYPE_UNIT_LEAD_GAP = 435;	// 리드 간격 검사
        public const int INSP_TYPE_EXCEPTIONAL = 999;	// Exceptional Mask

        ////////////////////////////////////////////

        public const int INSP_ALGO_FEATURE_ALIGN = 1;	// Strip Align Inspection
        public const int INSP_ALGO_BALL_PSR = 2;	// Ball PSR Shift 검사
        public const int INSP_ALGO_CROSS = 3;	// 십자가 인식키 전용 검사
        public const int INSP_ALGO_TOLERANCE = 4;	// 공차 치수 검사(PSR Y, Punch Y 검사)
        public const int INSP_ALGO_SHAPE_SHIFT = 5;
        public const int INSP_ALGO_LEAD_GAP = 6;	// 리드 간격 검사
        public const int INSP_ALGO_SHAPE_WITH_DIR = 7;	// Shape Inspection
        public const int INSP_ALGO_LEAD_WITH_CL = 8;
        public const int INSP_ALGO_GROOVE_WITH_CL = 9;
        public const int INSP_ALGO_SHAPE_HALF = 10;
        public const int INSP_ALGO_STATE_INTENSITY = 11;	// State Intensity
        public const int INSP_ALGO_STATE_ALIGNED_MASK = 12;	// State Inspection
        public const int INSP_ALGO_BALL_PATTERN = 13;	// Ball 패턴 전용 검사
        public const int INSP_ALGO_DAMBAR = 14;	// Dam Bar 검사
        public const int INSP_ALGO_VENTHOLE = 15;	// Via-Hole Inspection
        public const int INSP_ALGO_EXCEPTIONAL = 16;	// Exceptional Mask Area

        ////////////////////////////////////////////

        public const int PRIORITY_GLOBAL_ALIGN = 11;
        public const int PRIORITY_GLOBAL_PSR = 12;
        public const int PRIORITY_GLOBAL_FIDU = 13;
        public const int PRIORITY_GLOBAL_NORMAL = 14;
        public const int PRIORITY_RAIL_ALIGN = 21;
        public const int PRIORITY_RAIL_PSR = 22;
        public const int PRIORITY_RAIL_FIDU = 23;
        public const int PRIORITY_RAIL_NORMAL = 24;
        public const int PRIORITY_BLOCK_ALIGN = 31;
        public const int PRIORITY_BLOCK_PSR = 32;
        public const int PRIORITY_BLOCK_FIDU = 33;
        public const int PRIORITY_BLOCK_NORMAL = 34;
        public const int PRIORITY_UNIT_ALIGN = 41;
        public const int PRIORITY_UNIT_PSR = 42;
        public const int PRIORITY_UNIT_PUNCH = 43;
        public const int PRIORITY_UNIT_FIDU = 44;
        public const int PRIORITY_UNIT_PLATE = 45;
        public const int PRIORITY_UNIT_RAW = 46;
        public const int PRIORITY_UNIT_NORMAL = 47;

        ////////////////////////////////////////////

        public const int INSP_RES_GOOD = 0;    // 양품
        public const int INSP_RES_StripAlign = 101;	// Strip Align 불량
        public const int INSP_RES_ContamiInStripFidu = 102;	// Strip 인식키 오염
        public const int INSP_RES_OpenInStripFidu = 103;	// Strip 인식키 단락
        public const int INSP_RES_NonPlateInStripFidu = 104;    // Strip 인식키 미도금
        public const int INSP_RES_RailAlign = 201;	// Rail Align 불량
        public const int INSP_RES_ContamiInRailFidu = 202;	// Rail 인식키 오염
        public const int INSP_RES_OpenInRailFidu = 203;	// Rail 인식키 단락
        public const int INSP_RES_NonPlateInRailFidu = 204;	// Rail 인식키 미도금
        public const int INSP_RES_PSRInRail = 211;	// Rail영역 PSR 불량
        public const int INSP_RES_NonPSRInRail = 212;	// Rail영역 PSR 미도포 불량
        public const int INSP_RES_ContamiInRail = 213;  // Rail영역 오염성 불량
        public const int INSP_RES_NonPlateInRail = 214;	// Rail영역 미도금 불량
        public const int INSP_RES_ScratchInRail = 215;	// Rail영역 스크래치 불량
        public const int INSP_RES_ForeignBodyInRail = 216;	// Rail영역 이물질 불량
        public const int INSP_RES_PSRShiftInRail = 217;	// Rail영역 PSR Shift 불량
        public const int INSP_RES_BlockAlign = 301;	// Block Align 불량
        public const int INSP_RES_ContamiInBlockFidu = 302;	// Block 인식키 오염
        public const int INSP_RES_OpenInBlockFidu = 303;	// Block 인식키 단락
        public const int INSP_RES_NonPlateInBlockFidu = 304;	// Block 인식키 미도금
        public const int INSP_RES_PSRInBlock = 311;	// Block영역 PSR 불량
        public const int INSP_RES_NonPSRInBlock = 312;	// Block영역 PSR 미도포 불량
        public const int INSP_RES_ContamiInBlock = 313;   // Block영역 오염성 불량
        public const int INSP_RES_NonPlateInBlock = 314;	// Block영역 미도금 불량
        public const int INSP_RES_ScratchInBlock = 315;	// Block영역 스크래치 불량
        public const int INSP_RES_ForeignBodyInBlock = 316;	// Block영역 이물질 불량
        public const int INSP_RES_PSRShiftInBlock = 317;	// Block영역 PSR Shift 불량
        public const int INSP_RES_UnitAlign = 401;	// Unit Align 불량
        public const int INSP_RES_ContamiInUnitFidu = 402;	// Unit 인식키 오염
        public const int INSP_RES_OpenInUnitFidu = 403;	// Unit 인식키 단락
        public const int INSP_RES_NonPlateInUnitFidu = 404;	// Unit 인식키 미도금
        public const int INSP_RES_NonPlateInRawMeterial = 411;	// Dummy 불량 (제품 없음 or 조명 이
        public const int INSP_RES_RawMaterial = 412;	// 원소재 불량 (1차 마킹 제품)
        public const int INSP_RES_Open = 413;	// Open성 불량
        public const int INSP_RES_Short = 414;	// Short성 불량
        public const int INSP_RES_MB = 415;	// MB성 불량
        public const int INSP_RES_Protrusion = 416;	// 미성형 불량
        public const int INSP_RES_LeadPinhole = 417;	// 리드 핀홀 불량
        public const int INSP_RES_Island = 418;	// 섬형 불량
        public const int INSP_RES_Pit = 419;	// Pit성 불량
        public const int INSP_RES_NoLead = 420;	// 리드 탈락
        public const int INSP_RES_CurvedLead = 421;	// 리드 휨
        public const int INSP_RES_ShortLead = 422;	// 리드 휨
        public const int INSP_RES_LargeSurface = 423;	// 광역 표면 불량
        public const int INSP_RES_LeadDeviation = 424;	// 리드 편차 불량
        public const int INSP_RES_LeadLocation = 425;	// 리드 위치
        public const int INSP_RES_MinWidth = 426;	// 최소 선폭 검사 이하 검출
        public const int INSP_RES_MaxWidth = 427;  // 최대 선폭 검사 이상 검출
        public const int INSP_RES_PSRInLead = 428;	// 리드 내 PSR 잔존
        public const int INSP_RES_Nodule = 429;	// 동돌출 불량
        public const int INSP_RES_BondPadInterval = 430;	// 리드 간격 불량
        public const int INSP_RES_SR = 431;	// SR 뭉침
        public const int INSP_RES_PSR = 432;	// PSR 뭉침
        public const int INSP_RES_NonSR = 433;	// SR 미도포
        public const int INSP_RES_NonPSR = 434;	// PSR 미도포
        public const int INSP_RES_SRPinHole = 435;	// SR Pinhole 불량
        public const int INSP_RES_PSRPinHole = 436;	// PSR Pinhole 불량
        public const int INSP_RES_NonPlate = 437;	// 미도금
        public const int INSP_RES_PlateVoid = 438;	// 도금 VOID (PSR 잔존)
        public const int INSP_RES_Scratch = 439;	// Scratch 불량
        public const int INSP_RES_Contami = 440;	// 오염성 불량
        public const int INSP_RES_ForeignBody = 441;	// 이물질 불량
        public const int INSP_RES_SRShfit = 442;	// SR Location
        public const int INSP_RES_PSRShift = 443;	// PSR Shift;
        public const int INSP_RES_PunchShift = 444;	// Windows Punch Shift
        public const int INSP_RES_IllumiFault = 445;	// 조명 이상
        public const int INSP_RES_ScratchInLead = 446;	// 리드 Scratch(Min)
        public const int INSP_RES_NonPlateInLead = 447;	// 리드 미도금(Max)
        public const int INSP_RES_LeadLift = 448;	// 리드 들뜸(Min)
        public const int INSP_RES_CoreCrack = 449;	// Core Crack(Max)
        public const int INSP_RES_PSRInBall = 450;	// Ball PSR잔존(Min)
        public const int INSP_RES_NonPlateInBall = 451;	// Ball 미도금(Max)
        public const int INSP_RES_PunchBurr = 452;	// Punch Burr(Min)
        public const int INSP_RES_ShapeInBall = 453;	// Ball 형상 이상
        public const int INSP_RES_SmallBall = 454;	// Ball 크기이상(작음)
        public const int INSP_RES_LargeBall = 455;	// Ball 크기이상(큼)
        public const int INSP_RES_DamBarLength = 456;	// Dam Bar 길이 불량
        public const int INSP_RES_VentHole = 457;	// Vent-Hole 불량
        public const int INSP_RES_ViaHole = 458;	// Via-Hole 불량
        public const int INSP_RES_Tape = 459;	// Tape 불량
        public const int INSP_RES_Downset = 460;	// Downset 불량
        public const int INSP_RES_LeadWidth = 461;	// 리드폭 불량
        public const int INSP_RES_LeadSpace = 462;	// 공간폭 불량
        public const int INSP_RES_PlateFlash = 463; // 도금 Flash
        public const int INSP_RES_Shape = 464;      // 형상 이상 불량
        public const int INSP_RES_LocationShape = 465;   // 형상 위치 불량
        public const int INSP_RES_LargeSize = 466;      // 형상 확대 불량
        public const int INSP_RES_SmallSize = 467;      // 형상 축소 불량
        public const int INSP_RES_GrooveNone = 468;	// 전체 Groove 미성형
        public const int INSP_RES_GroovePartial = 469;	// 부분 Groove 미성형
        public const int INSP_RES_GrooveMB = 470;	// 부분 Groove MB
        public const int INSP_RES_GrooveProtrusion = 471;	// 부분 Groove 돌출
        public const int INSP_RES_HalfEtchingShape = 472;	// 하프에칭 형상
        public const int INSP_RES_HalfEtchingIntensity = 473;	// 하프에칭 밝기
        public const int INSP_RES_LeadGap = 474;	// 리드 간격
        public const int INSP_RES_Other = 999;
        public const int INSP_RES_TRAIN_ERROR = -1;
        public const int INSP_RES_BUFFER_ERROR = -2;
    }

    public class StripAlignInfo
    {
        public int RoiID;
        public Int32Rect BndRect;
        public int SearchMarginX;
        public int SearchMarginY;
        public int Match;
    }

    public class IDMarkInfo
    {
        public int RoiID;
        public Int32Rect BndRect;
        public int Threshold;
    }

    // Vision PC에서 인식가능한 ROI 자료구조.
    public class RoiInfo
    {
        public int RoiID;							    // ROI ID
        public int SectionID;						    // 연결 Section ID
        public int UnitRow;
        int Channel;                                    //Color Channel

        public Int32Rect BndRect;					    // ROI 외곽 Rectangle (SetInspItems에서 계산)
        public int OuterDrawType;					    // 외곽 ROI 종류
        public int InnerDrawType;					    // 내부 ROI 종류
        public int OuterPointsCount;				    // 외곽 정점 개수
        public int InnerPointsCount;                    // 내부 정점 개수

        public System.Drawing.Point[] OuterPoints;	    // 외곽 정점 (Section 내 상대 좌표)
        public System.Drawing.Point[] InnerPoints;	    // 내부 정점 (Section 내 상대 좌표)

        public List<System.Drawing.Point> LocalAlignPoints
            = new List<System.Drawing.Point>();         // 로컬 Align 좌표
    }

    // Vision PC에서 수신되는 결과 구조.
    public class ResultInfo
    {
        private const int MAX_RESULT_ITEM_NUM = 1024;

        public int UnitInspFailureCount;				// 불량 유닛 개수
        public int ResultItemCount;					    // 전체 불량 개수

        public ResultItem[] Results = new ResultItem[MAX_RESULT_ITEM_NUM];
        public IDMarkResultInfo IDMark = new IDMarkResultInfo();
        #region Ctor.
        public ResultInfo()
        {
            for (int i = 0; i < Results.Length; i++)
            {
                Results[i] = new ResultItem();
            }           
            IDMark = new IDMarkResultInfo();
        }
        #endregion
    }
    public class PSRResults
    {
        private const int MAX_RESULT_ITEM_NUM = 1024;
        public int ResultItemCount;					    // 전체 불량 개수
        public PSRResultItem[] Results;
        public PSRResults(int resultcount)
        {
            ResultItemCount = resultcount;
            Results = new PSRResultItem[ResultItemCount];

            for (int i = 0; i < Results.Length; i++)
            {
                Results[i] = new PSRResultItem();
            }
        }
        public PSRResults()
        {
        }
    }

    public class IDMarkResultInfo
    {
        public int Status;
        public int Width;
        public int Height;
        public System.Drawing.Bitmap Image;
        public IDMarkResultInfo()
        {
            Width = Height = 0;
            Image = null;
        }
    }

    //PSR Shift Result
    public class PSRResultItem
    {
        public bool CommRet { get; set; }
        public System.Drawing.Point UnitPos { get; set; }
        public int Position { get; set; }//TL,TR,BL,BR
        public System.Drawing.PointF Offset { get; set; }
    }

    // Vision PC에서 수신되는 개별 결과 아이템.
    public class ResultItem
    {
        public int ResultID { get; set; }			        // 결과 ID (0~MAX_RESULT_IMAGE_NUM-1)
        public int InspID;								    // 검사 아이템 ID
        public int SectionID;							    // Image Section ID
        public int RoiID;				                    // 연결 ROI ID
        public System.Drawing.Point UnitPos;                // 유닛 위치 (0, 0) based, Display 시점에 +1, +1 해주어야 한다.

        public int BreakType;							    // 검사 종료 결과(ACTION_COUNTINUE, ACTION_FINISH_STRIP,ACTION_FINISH_UNIT)
        public int ResultType;							    // 불량 결과 항목 (DB: defect_result 테이블 defect_code와 매칭됨)

        public Int32Rect AbsoluteDefectRect;                // 불량 위치 범위 (기준 영상 기준, 절대 좌표)
        public System.Drawing.Point AbsoluteDefectCenter;   // 불량 중심 좌표 

        public Int32Rect RelativeDefectRect;                // 불량 위치 범위 (섹션 영상 기준, 상대 좌표)
        public System.Drawing.Point RelativeDefectCenter;   // 불량 중심 좌표 

        public double DefectSize { get; set; }				// 불량 크기
        public double DefectScore { get; set; }				// 불량 판정 점수

        public int Channel { get; set; }
        public int Grabside { get; set; }
        // 검사 방법.
        public string InspType { get; set; }

        // 검사 방법.
        public string ResPosition { get; set; }

        // 불량 좌표.

        public string defectRegion;
        public string DefectRegion { get { return (new Int32Rect(AbsoluteDefectRect.X, AbsoluteDefectRect.Y, AbsoluteDefectRect.Width, AbsoluteDefectRect.Height)).ToString(); } set { defectRegion = value; } }

        // 불량 중심. (Strip 절대 좌표)
        public string stripDefectCenter;
        public string StripDefectCenter {get { return String.Format("{0:F0},{1:F0}", AbsoluteDefectCenter.X, AbsoluteDefectCenter.Y); } set { sectionDefectCenter = value; } }

        // 불량 중심. (Section 상대 좌표)
        public string sectionDefectCenter;
        public string SectionDefectCenter { get { return string.Format("{0:F0},{1:F0}", RelativeDefectCenter.X, RelativeDefectCenter.Y); } set { sectionDefectCenter = value; } }

        // 불량 이름.
        public string resultName;
        public string ResultName { get { return resultName; } set { resultName = value; } }
        //public string ResultName { get { return InspectDataManager.GetNGName(ResultType); } set { resultName = value; } }



        // 유닛 위치.
        public string unitPosition;
        public string UnitPosition 
        { 
            get { return String.Format("X:{0},Y:{1}", UnitPos.X + 1, UnitPos.Y + 1);} 
            set { unitPosition = value; } 
        }

        public int RealIndex { get; set; }
        public int SortedIndex { get; set; }

        public int AreaID { get; set; }

        public int Priority { get; set; }
        public double LongDegree { get; set; }
        public double ShortDegree { get; set; }
        public double AvgRed { get; set; }
        public double AvgGreen { get; set; }
        public double AvgBlue { get; set; }
        public double AvgGV { get; set; }
        public int AvgS { get; set; }
        public int AvgH { get; set; }
        public int UnitAlignOffset_X { get; set; } // MJ : 260129 ICS-AVI 좌표계 연동 (Unit Align X좌표 보정치)
        public int UnitAlignOffset_Y { get; set; } // MJ : 260129 ICS-AVI 좌표계 연동 (Unit Align Y좌표 보정치)
        public string SectionType { get; set; }//  260129 ICS-AVI 좌표계 연동 (불량 Section Type)

        public int DefectPosX { get; set; } // MJ : 260129 ICS-AVI 좌표계 연동 (Unit Align Y좌표 보정치)
        public int DefectPosY { get; set; } // MJ : 260129 ICS-AVI 좌표계 연동 (Unit Align Y좌표 보정치)

        private string channelString;
        public string ChannelString
        {
            get
            {
                return this.channelString;
            }
            set
            {
                if (Channel == 0) this.channelString = "R";
                else if (Channel == 1) this.channelString = "G";
                else this.channelString = "B";
            }
        }

        public ResultItem Clone()
        {
            ResultItem tmp = new ResultItem();
            tmp.ResultID = this.ResultID;
            tmp.InspID = this.InspID;
            tmp.SectionID = this.SectionID;
            tmp.InspID = this.InspID;
            tmp.RoiID = this.RoiID;
            tmp.UnitPos = this.UnitPos;
            tmp.BreakType = this.BreakType;

            tmp.ResultType = this.ResultType;
            tmp.AbsoluteDefectRect = this.AbsoluteDefectRect;
            tmp.AbsoluteDefectCenter = this.AbsoluteDefectCenter;
            tmp.RelativeDefectRect = this.RelativeDefectRect;
            tmp.RelativeDefectCenter = this.RelativeDefectCenter;
            tmp.DefectSize = this.DefectSize;
            tmp.DefectScore = this.DefectScore;
            tmp.Channel = this.Channel;

            tmp.Grabside = this.Grabside;
            tmp.InspType = this.InspType;
            tmp.ResPosition = this.ResPosition;
            tmp.DefectRegion = this.DefectRegion;
            tmp.StripDefectCenter = this.StripDefectCenter;
            tmp.SectionDefectCenter = this.SectionDefectCenter;
            tmp.ResultName = this.ResultName;
            tmp.UnitPosition = this.UnitPosition;
            tmp.SectionType = this.SectionType;

            tmp.RealIndex = this.RealIndex;
            tmp.SortedIndex = this.SortedIndex;
            tmp.ChannelString = this.ChannelString;
            tmp.SectionType = this.SectionType;

            tmp.UnitAlignOffset_X = this.UnitAlignOffset_X;
            tmp.UnitAlignOffset_Y = this.UnitAlignOffset_Y;

            tmp.DefectPosX = this.DefectPosX;
            tmp.DefectPosY = this.DefectPosY;

            return tmp;
        }
    }
    //public class UnitResultItem
    //{        
    //    public int UnitAlignOffset_X { get; set; } // MJ : 260129 ICS-AVI 좌표계 연동 (Unit Align X좌표 보정치)
    //    public int UnitAlignOffset_Y { get; set; } // MJ : 260129 ICS-AVI 좌표계 연동 (Unit Align Y좌표 보정치)
    //    public UnitResultItem Clone()
    //    {
    //        UnitResultItem tmp = new UnitResultItem();           
    //        tmp.UnitAlignOffset_X = this.UnitAlignOffset_X;
    //        tmp.UnitAlignOffset_Y = this.UnitAlignOffset_Y;
    //        return tmp;
    //    }
    //}
}
