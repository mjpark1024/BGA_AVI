/* property of Haesung DS. This software may only be used in accordance with
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
 * @file  InspectionResultTypeHelper.cs
 * @brief 
 *  대표 결과, 하한 결과, 상한 결과를 설정해 준다.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2012.04.02
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2012.04.02 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common.Drawing.InspectionInformation
{
    public static class InspectionResultTypeHelper
    {
        public static void SetResultType(int anSurfaceID, InspectionItem aInspItem)
        {          
            aInspItem.InspectionType.BreakType = 0;
            switch(aInspItem.InspectionType.InspectType)
            {
                case eVisInspectType.eInspTypeUnitAlign: // Unit Align
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultUnitAlign;
                    break;

                case eVisInspectType.eInspTypeSurface: // 표면 검사
                        aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultContami; // 조명 이상
                        aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultContami; // 오염
                        aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultForeignBody; // 이물질
                    break;

                case eVisInspectType.eInspTypeDownSet: // Via 검사
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultDownset; // 조명 이상
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultDownset; // 오염
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultDownset; // 이물질
                    break;

                case eVisInspectType.eInspTypeDamBar: // Via 검사
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultMinDam; // 조명 이상
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultMinDam; // 오염
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultMaxDam; // 이물질
                    break;

                case eVisInspectType.eInspTypePunchBurr: // Burr 검사
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultPunchBurr; // 조명 이상
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultPunchBurr; // 오염
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultPunchBurr; // 이물질
                    break;

                case eVisInspectType.eInspTypeSpace: // Crack 검사
                    //if (anSurfaceID < 2) // 상부 반사, 하부 반사
                    //{
                    //    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultIllumiFault; // 조명 이상
                    //    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultIllumiFault; // 조명 이상
                    //    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultNonPlastic; // 미성형
                    //}
                    //else if (anSurfaceID == 2) // 상부 투과
                    //{
                        aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultCoreCrack; // 조명 이상
                        aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultCoreCrack; // 미성형
                        aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultCoreCrack; // 조명 이상
                    //}
                    break;

                case eVisInspectType.eInspTypePlate: // 도금 검사
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultPlateShift; // 도금 Shift
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultNonPlate; // 미도금
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultAbnormalPlate; // 도금 이상
                    break;

                case eVisInspectType.eInspTypePlateSurface: // 도금 검사
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultNonPlate; // 도금 Shift
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultNonPlate; // 미도금
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultNonPlate; // 도금 이상
                    break;

                case eVisInspectType.eInspTypeTape: // 테이프 검사
                case eVisInspectType.eInspTypeStateTape:
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultTapeShift; // Tape Shift
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultTapeContami; // Tape 오염
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultTapePinhole; // Tape 손상
                    break;

                case eVisInspectType.eInspTypePSR:
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultPSR; // Tape Shift
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultPSR; // Tape 오염
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultPSRPinHole; // Tape 손상
                    break;

                case eVisInspectType.eInspTypePSROdd:
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultPSROdd_Circuit;
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultPSROdd_Circuit;
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultPSROdd_Circuit; 
                    break;

                case eVisInspectType.eInspResultGV:
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultGV;
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultGV;
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultGV;
                    break;

                case eVisInspectType.eInspTypeBallPattern:
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultNonPlateInBall; // Tape Shift
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultPSRInBall; // Tape 오염
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultShapeInBall; // Tape 손상
                    break;

                case eVisInspectType.eInspTypeOuter:
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultContamiInRail; // Tape Shift
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultContamiInRail; // Tape 오염
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultNonPSRInRail; // 
                    break;

                case eVisInspectType.eInspTypeRailFiducial:
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultContamiInRailFidu; // Tape Shift
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultContamiInRailFidu; // Tape 오염
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultContamiInRailFidu; // 
                    break;

                case eVisInspectType.eInspTypeIDMark:
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultOther; // Tape Shift
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultOther; // Tape 오염
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultOther; // 
                    break;

                case eVisInspectType.eInspTypeUnitFidu:
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultOpenInUnitFidu; // Tape Shift
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultOpenInUnitFidu; // Tape 오염
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultOpenInUnitFidu; // Tape 손상
                    break;

                case eVisInspectType.eInspTypeUnitPattern:
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultUnitPattern; // Tape Shift
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultUnitPattern; // Tape 오염
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultUnitPattern; // Tape 손상
                    break;

                case eVisInspectType.eInspTypeVentHole:    // 벤트홀 검사
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultVentHole; // 벤트홀 불량
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultVentHole; // 벤트홀 불량
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultVentHole; // 벤트홀 불량              
                    break;

                case eVisInspectType.eInspTypeNoResizeVentHole: // 벤트홀 외곽검사              
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultNoReSizeVentHole; // 벤트홀2 불량
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultNoReSizeVentHole; // 벤트홀2 불량
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultNoReSizeVentHole; // 벤트홀2 불량  
                    break;

                case eVisInspectType.eInspTypeCoreCrack:
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultCoreCrack; // Tape Shift
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultCoreCrack; // Tape 오염
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultPunchBurr; // Tape 손상
                    break;

                case eVisInspectType.eInspTypePlateWithCL: // 도금 선폭 검사
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultPlateShift; // 도금 Shift
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultNonPlate; // 미도금
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultAbnormalPlate; // 도금 이상
                    break;

                case eVisInspectType.eInspTypeRailIntensity: // 다운셋 검사
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultNonPSRInRail; // Downset
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultPSRInRail; // 미도금
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultNonPSRInRail; // 도금 이상
                    
                    break;

                case eVisInspectType.eInspTypeLeadShapeWithCL: // 리드 형상 검사
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultLeadDeviation; // 리드 편차
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultOpen; // Open
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultShort; // Short
                    break;

                case eVisInspectType.eInspTypeSpaceShapeWithCL: // 공간 형상 검사
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultSpaceDeviation; // 공간 편차
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultShort; // Short
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultOpen; // Open
                    break;

                case eVisInspectType.eInspTypeLeadGap: // 리드 간격 검사
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultLeadGap; // 리드 간격
                    break;

                case eVisInspectType.eInspTypeGroove: // Groove 검사
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultNonGroove; // 전체 Groove
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultGrooveMB; // Groove MB
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultGrooveProtrusion; // Groove 돌출
                    break;
                case eVisInspectType.eInspTypeUnitRawMaterial: //  원소재 검사
                    aInspItem.InspectionType.ResultType = (int)eVisInspectResultType.eInspResultRawMaterial; // 원소재 불량
                    aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultRawMaterial; // 원소재 불량
                    aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultRawMaterial; // 원소재 불량
                    break;
                    
                case eVisInspectType.eInspTypeShapeHalfEtching: // 형상 Half Etching 검사                    
                    //aInspItem.InspectionType.ResultTypeMin = (int)eVisInspectResultType.eInspResultLocationShape; // TBD
                    //aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspectDarkArea; // TBD
                    break;

                case eVisInspectType.eInspTypeSurfaceHalfEtching: // 표면 Half Etching 검사
                    //aInspItem.InspectionType.ResultTypeMax = (int)eVisInspectResultType.eInspResultNoEtching; // TBD
                    break;

                case eVisInspectType.eInspTypeExceptionalMask: // 검사 제외
                    break;
            }
        }
    }
}
