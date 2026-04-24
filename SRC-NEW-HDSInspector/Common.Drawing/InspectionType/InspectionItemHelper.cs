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
 * @file  InspectionItemHelper.cs
 * @brief 
 *  Helper class.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.10.10
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.10.10 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Drawing.InspectionTypeUI;

namespace Common.Drawing.InspectionInformation
{
    /// <summary>   Inspection item helper.  </summary>
    /// <remarks>   suoow2, 2014-10-10. </remarks>
    public static class InspectionItemHelper
    {
        // 검사 설정 복사
        public static InspectionItem CopyInspectionItem(InspectionItem inspectionItem)
        {
            if (inspectionItem != null && inspectionItem.InspectionAlgorithm != null && inspectionItem.InspectionType != null)
            {
                InspectionItem copiedInspectionItem = new InspectionItem()
                {
                    ID = inspectionItem.ID,
                    InspectionType = inspectionItem.InspectionType,
                    InspectionAlgorithm = inspectionItem.InspectionAlgorithm.Clone() // 검사 설정 값은 독자적으로 관리되어야 하기 때문에 Deep copy를 수행하여준다.
                };
                copiedInspectionItem.LineSegments = inspectionItem.LineSegments; // 중앙선 Training Data Pointer.
                copiedInspectionItem.BallSegments = inspectionItem.BallSegments;
                return copiedInspectionItem;
            }
            else
            {
                return inspectionItem;
            }
        }

        // 요구된 검사 설정 아이템 반환.
        public static InspectionItem GetInspectionItem(string aszMachineCode, string aszModelCode, string aszWorkType, string aszSectionCode, 
                                                       string aszRoiCode, string aszInspectID, string aszInspectCode)
        {
            InspectionType inspectionType = null;
            List<InspectionType> inspectionTypeList = InspectionType.GetInspectionTypeList();
            if (inspectionTypeList != null)
            {
                foreach (InspectionType inspectionTypeElement in inspectionTypeList)
                {
                    if (aszInspectCode == inspectionTypeElement.Code)
                    {
                        inspectionType = inspectionTypeElement;
                        break;
                    }
                }
            }
            if (inspectionType == null)
                return null;

            switch (inspectionType.InspectType)
            {
                case eVisInspectType.eInspTypeGlobalAlign: // Strip Align
                case eVisInspectType.eInspTypeUnitAlign: // Unit Align
                    FiducialAlignProperty fiducialAlignProperty = new FiducialAlignProperty();
                    fiducialAlignProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, fiducialAlignProperty, int.Parse(aszInspectID));
                //break;

                case eVisInspectType.eInspTypeLeadShapeWithCL: // 리드 형상 검사
                    LeadShapeWithCenterLineProperty leadShapeWithCenterLineProperty = new LeadShapeWithCenterLineProperty();
                    leadShapeWithCenterLineProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, leadShapeWithCenterLineProperty, int.Parse(aszInspectID));
                //break;

                case eVisInspectType.eInspTypeSpaceShapeWithCL: // 공간 형상 검사
                    SpaceShapeWithCenterLineProperty spaceShapeWithCenterLineProperty = new SpaceShapeWithCenterLineProperty();
                    spaceShapeWithCenterLineProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, spaceShapeWithCenterLineProperty, int.Parse(aszInspectID));
                //break;

                case eVisInspectType.eInspTypeGroove: // 그루브 검사
                    GrooveProperty grooveProperty = new GrooveProperty();
                    grooveProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, grooveProperty, int.Parse(aszInspectID));
                //break;

                case eVisInspectType.eInspTypeShapeHalfEtching: // 형상 Half Etching 검사
                    FigureShapeProperty figureShapeProperty = new FigureShapeProperty();
                    figureShapeProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, figureShapeProperty, int.Parse(aszInspectID));
                //break;

                case eVisInspectType.eInspTypeRailIntensity: // 다운셋 검사
                    StateIntensityProperty stateIntensityProperty = new StateIntensityProperty();
                    stateIntensityProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, stateIntensityProperty, int.Parse(aszInspectID));
                //break;

                case eVisInspectType.eInspTypeTape: // 테이프 검사
                case eVisInspectType.eInspTypePlate: // 도금 검사
                    ShapeShiftProperty shapeShiftProperty = new ShapeShiftProperty();
                    shapeShiftProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, shapeShiftProperty, int.Parse(aszInspectID));
                //break;

                case eVisInspectType.eInspTypePlateWithCL: // 도금 선폭 검사
                    ShapeShiftWithCLProperty shapeShiftWithCLProperty = new ShapeShiftWithCLProperty();
                    shapeShiftWithCLProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, shapeShiftWithCLProperty, int.Parse(aszInspectID));
                // break;

                case eVisInspectType.eInspTypeSurfaceHalfEtching: // 표면 Half Etching 검사
                case eVisInspectType.eInspTypeSurface: // 표면 검사
                case eVisInspectType.eInspTypeSpace: // 공간 검사
                case eVisInspectType.eInspTypePSR:
                case eVisInspectType.eInspTypePunchBurr:
                case eVisInspectType.eInspTypeDownSet: // 표면 검사
                    StateAlignedMaskProperty stateAlignedMaskProperty = new StateAlignedMaskProperty();
                    stateAlignedMaskProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, stateAlignedMaskProperty, int.Parse(aszInspectID));
                //break;

                case eVisInspectType.eInspTypeDamBar:
                    DamSizeProperty damProperty = new DamSizeProperty();
                    damProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, damProperty, int.Parse(aszInspectID));

                case eVisInspectType.eInspTypeUnitRawMaterial:
                    RawMetrialProperty rawmetrialProperty = new RawMetrialProperty();
                    rawmetrialProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, rawmetrialProperty, int.Parse(aszInspectID));

                case eVisInspectType.eInspTypeBallPattern:
                    BallPatternProperty ballProperty = new BallPatternProperty();
                    ballProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, ballProperty, int.Parse(aszInspectID));

                case eVisInspectType.eInspTypeOuter:
                    OuterProperty outerProperty = new OuterProperty();
                    outerProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, outerProperty, int.Parse(aszInspectID));

                case eVisInspectType.eInspTypeRailFiducial:
                    OuterFiducialProperty outerFiduProperty = new OuterFiducialProperty();
                    outerFiduProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, outerFiduProperty, int.Parse(aszInspectID));

                case eVisInspectType.eInspTypeIDMark:
                    IDAreaProperty idProperty = new IDAreaProperty();
                    idProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, idProperty, int.Parse(aszInspectID));
                case eVisInspectType.eInspTypeUnitFidu:                    
                    CrossPatternProperty crossPatternProperty = new CrossPatternProperty();
                    crossPatternProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, crossPatternProperty, int.Parse(aszInspectID));

                case eVisInspectType.eInspTypeUnitPattern:
                    UnitPatternProperty unitPatternProperty = new UnitPatternProperty();
                    unitPatternProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, unitPatternProperty, int.Parse(aszInspectID));

                case eVisInspectType.eInspTypeVentHole:
                    VentHoleProperty ventHoleProperty = new VentHoleProperty();
                    ventHoleProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, ventHoleProperty, int.Parse(aszInspectID));

                case eVisInspectType.eInspTypeNoResizeVentHole:
                    VentHoleProperty2 ventHoleProperty2 = new VentHoleProperty2();
                    ventHoleProperty2.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, ventHoleProperty2, int.Parse(aszInspectID));

                case eVisInspectType.eInspTypeCoreCrack:
                    WindowPunchProperty windowPunchProperty = new WindowPunchProperty();
                    windowPunchProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, windowPunchProperty, int.Parse(aszInspectID));

                case eVisInspectType.eInspTypeLeadGap: // 공간 검사
                    LeadGapProperty leadGapProperty = new LeadGapProperty();
                    leadGapProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, leadGapProperty, int.Parse(aszInspectID));
                //break;

                case eVisInspectType.eInspTypeExceptionalMask: // 검사 예외
                    ExceptionalMaskProperty exceptionalMaskProperty = new ExceptionalMaskProperty();
                    exceptionalMaskProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, exceptionalMaskProperty, int.Parse(aszInspectID));
                //break;

                case eVisInspectType.eInspTypePSROdd: // PSR 하지이물
                    PSROddProperty psrOddProperty = new PSROddProperty();
                    psrOddProperty.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, psrOddProperty, int.Parse(aszInspectID));

                case eVisInspectType.eInspResultGV: // GV검사
                    GV_Inspection_Property gv_Inspection_Property = new GV_Inspection_Property();
                    gv_Inspection_Property.LoadProperty(aszMachineCode, aszModelCode, aszWorkType, aszSectionCode, aszRoiCode, aszInspectID, inspectionType.Code);
                    return new InspectionItem(inspectionType, gv_Inspection_Property, int.Parse(aszInspectID));

                default:
                    return null;
            }
        }
    }
}
