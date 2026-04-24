// Vision Interface 기준 데이터.
namespace Common.Drawing.InspectionInformation
{
    // 검사 Type.
    public enum eVisInspectType
    {
        eInspTypeGlobalAlign = 101,	                                 // Strip Align
        eInspTypeIDMark = 113, //2D ID Mark

        eInspTypeRailFiducial = 205,	                                // 외곽 인식키
        eInspTypeRailAlignedMask = 211,	                    // 외곽 마스크 검사
        eInspTypeRailIntensity = 212,	                                // 외곽 밝기 검사

        eInspTypeUnitAlign = 401,	                                    // Unit Align
        eInspTypeUnitCross = 402,	                                    // Unit 십자가
        eInspTypeUnitPattern = 407,                                     // 유닛 인식키 폐기

        eInspTypeUnitFidu = 411,	                                    // 인식키 검사 (Aligned Mask)
        eInspTypeUnitRawMaterial = 413,	                                // 원소재 검사 (Aligned Mask)

        eInspTypeLeadShapeWithCL = 415,	                                // 리드 형상 검사
        eInspTypeGroove = 416,	                                        // Groove 전용 검사
        eInspTypeShapeHalfEtching = 417,	                            // 형상 Half-Etching
        eInspTypeSurfaceHalfEtching = 418,	                            // 표면 Half-Etching
        eInspTypeDownSet = 419,	                                        // Down Set 검사
        eInspTypeTape = 420,	                                        // Tape 검사
        eInspTypePlate = 421,	                                        // 도금 검사
        eInspTypeSurface = 422,	                                        // 표면 검사
        eInspTypeSpace = 423,	                                        // 공간 검사

        eInspTypeCoreCrack = 428,	                                    // Core Crack (Intensity)
        eInspTypeBallPattern = 429,	                                    // Ball Pattern (Ball Pattern)

        eInspTypePSR = 431,	                                            // PSR 영역 (Aligned Mask)
        eInspTypePunchBurr = 432,	                                    // Punch Burr (Intensity)
        eInspTypeDamBar = 433,	                                        // Dam Bar 검사(DamBar)
        eInspTypeVentHole = 434,                                        // Vent Hole 검사
        eInspTypeLeadGap = 435,	                                        // 리드 간격 검사
        eInspTypeSpaceShapeWithCL = 436,                                //공간 형상 검사
        eInspTypePlateWithCL = 437,                                     // 도금검사(CL)
        eInspTypePlateSurface = 438,                                    // 도금검사
        eInspTypeStateTape = 439,                                       // 테이프 검사
        eInspTypePSROdd = 450,                                          // PSR 하지이물 검사
        eInspResultGV = 490,                                            // GV 검사
        eInspTypeNoResizeVentHole = 441,                                // Vent Hole 외곽검사
        eInspTypeOuter = 500, // 외곽 표면 검사

        eInspTypeExceptionalMask = 999, // Exceptional Mask
    }

    // Section 타입.
    public enum eSectionType
    {
        eSecTypeNone = 0,
        eSecTypeGlobal = 1,
        eSecTypeRail = 2,
        eSecTypeBlock = 3,
        eSecTypeUnit = 4,
        eSecTypeRegionGlobal = 11,
        eSecTypeRegionOuter = 12,
        eSecTypeRegionBlock = 13,
        eSecTypeRegionUnit = 14,
        eSecTypeRegionRaw = 15,
        eSecTypeRegionID = 16,
        eSecTypeRegionPsr = 17,
    }

    // 알고리즘 타입.
    public enum eVisInspectAlgo
    {
        eInspAlgoFeatureAlign = 1,	                    // coefficient of correlation Align
        eInspAlgoBallPsrShift = 2,	                    // Ball PSR Shift 검사
        eInspAlgoCrossPattern = 3,	                    // 십자가 인식키 전용 검사
        eInspAlgoTolerance = 4,	                        // 공차 치수 검사(PSR Y, Punch Y 검사)
        eInspAlgoShapeShift = 5,	                    // Shape Shfit 검사
        eInspAlgoLeadGap = 6,	                        // 리드 간격 검사
        eInspAlgoShapeWithDir = 7,	                    // Shape Inspection
        eInspAlgoLeadWithCenterLine = 8,	            // 중앙선 검사
        eInspAlgoGrooveWithCenterLine = 9,	            // Groove 검사 with 중앙선
        eInspAlgoShapeHalfEtching = 10,	                // 특이 형상 Half Etching 검사
        eInspAlgoStateIntensity = 11,	                // State Intensity
        eInspAlgoStateAlignedMask = 12,	                // State Inspection
        eInspAlgoBallPattern = 13,	                    // Ball 패턴 전용 검사
        eInspAlgoDamBar = 14,	                        // Dam Bar 검사
        eInspAlgoVentHole = 15,	                        // Via-Hole Inspection
        eInspAlgoShapeShiftWithCL = 16,                 // Shape Shift With CL 검사
        eInspAlgoPlateSurface = 17,                     // Shape Shift With CL 검사
        eInspAlgoStripAlign = 18,	                    // Shape Shift With CL 검사
        eInspAlgoWindowPunch = 19,
        eInspAlgoRawMaterial = 20,
        eInspAlgoDamSize = 21,
        eInspAlgoUnitPattern = 22,
        eInspAlgoOuter = 23,
        eInspAlgoID = 24,
        eInspAlgoOuterFidu = 25,
        eInspAlgoGV = 26,
        eInspAlgoNoResizeVentHole = 27,
        eInspAlgoPSROdd = 30,
        eInspAlgoExceptionalMask = 99,	                // Exceptional Mask Area
    }

    // 알고리즘 우선순위
    public enum eInspectPriority
    {
        eInspPriorityNone = 0,

        eInspPriorityGlobalAlign = 11,
        eInspPriorityGlobalPsr = 12,
        eInspPriorityGlobalFidu = 13,
        eInspPriorityGlobalNormal = 14,

        eInspPriorityRailAlign = 21,
        eInspPriorityRailPsr = 22,
        eInspPriorityRailFidu = 23,
        eInspPriorityRailNormal = 24,

        eInspPriorityBlockAlign = 31,
        eInspPriorityBlockPsr = 32,
        eInspPriorityBlockFidu = 33,
        eInspPriorityBlockNormal = 34,

        eInspPriorityUnitAlign = 41,
        eInspPriorityUnitPsr = 42,
        eInspPriorityUnitPunch = 43,
        eInspPriorityUnitFidu = 44,
        eInspPriorityUnitPlate = 45,
        eInspPriorityUnitRaw = 46,
        eInspPriorityUnitNormal = 47
    }

    // 알고리즘 우선순위와 연계되어 처리됨.
    public static class DefineBreakType
    {
        public const int FINISH_NORMAL = 0;
        public const int FINISH_UNIT = 1;
        public const int FINISH_STRIP = 2;
    }

    // Vision 결과 리포팅 목록
    public enum eVisInspectResultType
    {
        eInspectGood = 0,                                   // 양품

        // Strip 영역 불량
        eInspResultStripAlign = 101,	                    // Strip Align 불량
        eInspResultContamiInStripFidu = 102,	            // Strip 인식키 오염
        eInspResultOpenInStripFidu = 103,	                // Strip 인식키 단락
        eInspResultNonPlateInStripFidu = 104,	            // Strip 인식키 미도금

        // 외곽 영역 불량
        eInspResultRailAlign = 201,	                        // Rail Align 불량
        eInspResultContamiInRailFidu = 202,	                // Rail 인식키 오염
        eInspResultOpenInRailFidu = 203,	                // Rail 인식키 단락
        eInspResultNonPlateInRailFidu = 204,	            // Rail 인식키 미도금

        eInspResultPSRInRail = 211,	                        // Rail영역 PSR 불량
        eInspResultNonPSRInRail = 212,	                    // Rail영역 PSR 미도포 불량
        eInspResultContamiInRail = 213,                     // Rail영역 오염성 불량
        eInspResultNonPlateInRail = 214,	                // Rail영역 미도금 불량
        eInspResultScratchInRail = 215,	                    // Rail영역 스크래치 불량
        eInspResultForeignBodyInRail = 216,	                // Rail영역 이물질 불량
        eInspResultPSRShiftInRail = 217,	                // Rail영역 PSR Shift 불량

        // Block 영역 불량
        eInspResultBlockAlign = 301,	                    // Block Align 불량
        eInspResultContamiInBlockFidu = 302,	            // Block 인식키 오염
        eInspResultOpenInBlockFidu = 303,	                // Block 인식키 단락
        eInspResultNonPlateInBlockFidu = 304,	            // Block 인식키 미도금

        eInspResultPSRInBlock = 311,	                    // Block영역 PSR 불량
        eInspResultNonPSRInBlock = 312,	                    // Block영역 PSR 미도포 불량
        eInspResultContamiInBlock = 313,                    // Block영역 오염성 불량
        eInspResultNonPlateInBlock = 314,	                // Block영역 미도금 불량
        eInspResultScratchInBlock = 315,	                // Block영역 스크래치 불량
        eInspResultForeignBodyInBlock = 316,	            // Block영역 이물질 불량
        eInspResultPSRShiftInBlock = 317,	                // Block영역 PSR Shift 불량

        // Unit 영역 불량
        eInspResultUnitAlign = 401,	                        // Unit Align 불량
        eInspResultContamiInUnitFidu = 402,	                // Unit 인식키 오염
        eInspResultOpenInUnitFidu = 403,	                // Unit 인식키 단락
        eInspResultNonPlateInUnitFidu = 404,	            // Unit 인식키 미도금
        eInspResultNonPlateInRawMeterial = 411,	            // Dummy 불량 (제품 없음 or 조명 이상)
        eInspResultRawMaterial = 412,	                    // 원소재 불량 (1차 마킹 제품)
        eInspResultOpen = 413,	                            // Open성 불량
        eInspResultShort = 414,	                            // Short성 불량
        eInspResultMB = 415,	                            // MB성 불량
        eInspResultProtrusion = 416,	                    // 돌출 불량
        eInspResultLeadPinhole = 417,	                    // 리드 핀홀 불량
        eInspResultIsland = 418,	                        // 섬형 불량
        eInspResultPit = 419,	                            // Pit성 불량
        eInspResultNoLead = 420,	                        // 리드 탈락
        eInspResultCurvedLead = 421,	                    // 리드 휨
        eInspResultShortLead = 422,	                        // 리드 휨
        eInspResultLargeSurface = 423,	                    // 광역 표면 불량
        eInspResultLeadDeviation = 424,	                    // 리드 편차 불량
        eInspResultLeadLocation = 425,	                    // 리드 위치
        eInspResultMinLeadWidth = 426,	                    // 최소 선폭 불량
        eInspResultMaxLeadWidth = 427,                      // 최대 선폭 불량
        eInspResultPSRInLead = 428,	                        // 리드 내 PSR 잔존
        eInspResultNodule = 429,	                        // 동돌출 불량
        eInspResultBondPadInterval = 430,	                // 리드 간격 불량
        eInspResultSR = 431,	                            // SR 뭉침
        eInspResultPSR = 432,	                            // PSR 뭉침
        eInspResultNonSR = 433,	                            // SR 미도포
        eInspResultNonPSR = 434,	                        // PSR 미도포
        eInspResultSRPinHole = 435,	                        // SR Pinhole 불량
        eInspResultPSRPinHole = 436,	                    // PSR Pinhole 불량
        eInspResultNonPlate = 437,	                        // 미도금
        eInspResultPlateVoid = 438,	                        // 도금 VOID (PSR 잔존)
        eInspResultScratch = 439,	                        // Scratch 불량
        eInspResultContami = 440,	                        // 오염성 불량
        eInspResultForeignBody = 441,	                    // 이물질 불량
        eInspResultSRShfit = 442,	                        // SR Location
        eInspResultPSRShift = 443,	                        // PSR Shift;
        eInspResultPunchShift = 444,	                    // Windows Punch Shift
        eInspResultIllumiFault = 445,	                    // 조명 이상
        eInspResultScratchInLead = 446,	                    // 리드 Scratch(Min)
        eInspResultNonPlateInLead = 447,	                // 리드 미도금(Max)
        eInspResultLeadLift = 448,	                        // 리드 들뜸(Min)
        eInspResultCoreCrack = 449,	                        // Core Crack(Max)
        eInspResultPSRInBall = 450,	                        // Ball PSR잔존(Min)
        eInspResultNonPlateInBall = 451,	                // Ball 미도금(Max)
        eInspResultPunchBurr = 452,	                        // Punch Burr(Min)
        eInspResultShapeInBall = 453,	                    // Ball 형상 이상
        eInspResultSmallBall = 454,	                        // Ball 크기이상(작음)
        eInspResultLargeBall = 455,	                        // Ball 크기이상(큼)
        eInspResultDamBarLength = 456,	                    // Dam Bar 길이 불량
        eInspResultVentHole = 457,	                        // Vent-Hole 불량
        eInspResultViaHole = 458,	                        // Via-Hole 불량
        eInspResultTape = 459,	                            // Tape 불량
        eInspResultDownset = 460,	                        // Downset 불량
        eInspResultLeadWidth = 461,	                        // 리드폭 불량
        eInspResultLeadSpace = 462,	                        // 공간폭 불량
        eInspResultPlateFlash = 463,	                    // 도금 Flash
        eInspResultShape = 464,                             // 형상 이상 불량
        eInspResultLocationShape = 465,                     // 형상 위치 불량
        eInspResultLargeSize = 466,                         // 형상 확대 불량
        eInspResultSmallSize = 467,                         // 형상 축소 불량
        eInspResultNonGroove = 468,	                        // 전체 Groove 미성형
        eInspResultPartialGroove = 469,	                    // 부분 Groove 미성형
        eInspResultGrooveMB = 470,	                        // 부분 Groove MB
        eInspResultGrooveProtrusion = 471,	                // 부분 Groove 돌출
        eInspResultHalfEtchingShape = 472,	                // 하프에칭 형상
        eInspResultHalfEtchingIntensity = 473,	            // 하프에칭 밝기
        eInspResultLeadGap = 474,	                        // 리드 간격
        eInspResultGrooveDeviation = 475,	                // Groove 폭 편차
        eInspResultTapeShift = 476,	                        // Tape Shift
        eInspResultTapeBurr = 477,	                        // Tape Burr
        eInspResultTapePinhole = 478,	                    // Tape 손상
        eInspResultPlateShift = 479,	                    // Plate Shift
        eInspResultOverEtching = 480,	                    // Over Etching
        eInspResultNonPlastic = 481,	                    // 미성형(돌출 유사)
        eInspResultAbnormalPlate = 482,	                    // 도금 이상
        eInspResultTapeContami = 483,	                    // Tape 오염
        eInspResultSpaceDeviation = 484,	                // 공간 편차 불량
        eInspResultMinSpaceWidth = 485,	                    // 최소 공간 폭 불량
        eInspResultMaxSpaceWidth = 486,                     // 최대 공간 폭 불량

        eInspResultMinDam = 487,                            // Min Dam
        eInspResultMaxDam = 488,                            // Max Dam

        eInspResultUnitPattern = 489,
        eInspResultGV = 490,                                // GV불량
        eInspResultNoReSizeVentHole = 491,                  // Outer Vent-Hole 불량

        eInspResultLocalAlign = 501,	                    // Local Align 불량

        eInspResultPSROdd_Circuit = 600,                    // PSR 회로부
        eInspResultPSROdd_Core = 601,                       // PSR-Core
        eInspResultPSROdd_Metal = 602,                      // PSR-Metal
        eInspResultPSROdd_Common = 603,                     // PSR-공통

        eInspResultOther = 999,

        // Error
        eInspResultTrainError = 901,
        eInspResultBufferError = 902,
        eInspResultInputParamError = 903,
        eInspResultSectionROIError = 904
    }

}
