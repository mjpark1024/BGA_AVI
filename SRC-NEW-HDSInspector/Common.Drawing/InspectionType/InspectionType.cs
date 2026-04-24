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
 * @file  InspectyionType.cs
 * @brief 
 *  검사 설정 아이템들이 정의되어 있다.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.09.23
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.09.23 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Windows.Controls;

// TBD
// 표면 타입에대한 리스트 생성 구분 필요.
// Surface ID.

namespace Common.Drawing.InspectionInformation
{
    /// <summary>   Inspection algorithm.  </summary>
    /// <remarks>   suoow2, 2014-10-09. </remarks>
    public abstract class InspectionAlgorithm
    {
        [XmlIgnore]
        protected string Code;

        public InspectionAlgorithm()
        {
            // for XML Serializing.
        }

        public abstract InspectionAlgorithm Clone();

        public abstract void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode);
        public abstract int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode);
        public virtual int UpdateProperty(string strModelCode, string strRoiCode, string strInspectID) { return 0; }
    }

    /// <summary>   Inspection type.  </summary>
    /// <remarks>   suoow2, 2014-10-09. </remarks>
    public class InspectionType
    {
        #region Properties.
        [XmlElement(ElementName = "Code")]
        public string Code { get; set; } // 검사 코드

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; } // 검사 이름

        [XmlElement(ElementName = "Priority")]
        public eInspectPriority Priority; // 검사 우선순위

        [XmlElement(ElementName = "InspectionType")]
        public eVisInspectType InspectType; // 검사 항목

        [XmlElement(ElementName = "InspectionAlgorithm")]
        public eVisInspectAlgo InspectAlgorithm; // 검사 알고리즘

        [XmlIgnore] // [XmlElement(ElementName = "ResultType")]
        public int ResultType; // 대표 불량 리포트 항목

        [XmlIgnore] // [XmlElement(ElementName = "ResultTypeMin")]
        public int ResultTypeMin; // 불량 리포트 항목 1 (Min 검사)

        [XmlIgnore] // [XmlElement(ElementName = "ResultTypeMax")]
        public int ResultTypeMax; // 불량 리포트 항목 2 (Max 검사)

        [XmlIgnore] // [XmlElement(ElementName = "BreakType")]
        public int BreakType;
        #endregion

        #region Constructors.
        // Ctor.
        public InspectionType()
        {
            if (SettingControl == null)
            {
                SettingControl = GetInspectionTypeSettingDialog(InspectType);
            }
        }

        // Initialize.
        private InspectionType(string strCode, string strName, eInspectPriority priority, eVisInspectType inspectType, eVisInspectAlgo inspectAlgorithm, int breakType, UserControl settingControl)
        {
            Code = strCode;
            Name = strName;

            Priority = priority;

            InspectType = inspectType;
            InspectAlgorithm = inspectAlgorithm;

            SettingControl = settingControl;
            if (SettingControl == null)
            {
                SettingControl = GetInspectionTypeSettingDialog(InspectType);
            }
            BreakType = breakType;
        }
        #endregion

        #region Set Inspection types.
        // 검사 항목 리스트.
        private static List<InspectionType> m_inspectionTypeList = new List<InspectionType>();

        /// <summary>   Gets the inspection type list. </summary>
        /// <remarks>   suoow2, 2014-10-09. </remarks>
        public static List<InspectionType> GetInspectionTypeList()
        {
            return m_inspectionTypeList;
        }

        // Strip Align에 대한 검사 설정 목록.
        public static List<InspectionType> GetStripAlignInspectionTypeList()
        {
            List<InspectionType> stripInspectionTypeList = new List<InspectionType>();
            foreach (InspectionType inspectionElement in m_inspectionTypeList)
            {
                if (inspectionElement.InspectType == eVisInspectType.eInspTypeGlobalAlign)
                {
                    stripInspectionTypeList.Add(inspectionElement);
                    break;
                }
            }
            return stripInspectionTypeList;
        }

        // Unit Align에 대한 검사 설정 목록.
        public static List<InspectionType> GetUnitAlignInspectionTypeList()
        {
            List<InspectionType> unitInspectionTypeList = new List<InspectionType>();
            foreach (InspectionType inspectionElement in m_inspectionTypeList)
            {
                if (inspectionElement.InspectType == eVisInspectType.eInspTypeUnitAlign)
                {
                    unitInspectionTypeList.Add(inspectionElement);
                }
                if (inspectionElement.InspectType == eVisInspectType.eInspTypeExceptionalMask)
                {
                    unitInspectionTypeList.Add(inspectionElement);
                }
            }
            return unitInspectionTypeList;
        }

        // 일반 검사 영역에 대한 검사 설정 목록.
        public static List<InspectionType> GetDefaultInspectionTypeList()
        {
            List<InspectionType> defaultInspectionTypeList = new List<InspectionType>();
            foreach (InspectionType inspectionElement in m_inspectionTypeList)
            {
                if (inspectionElement.InspectType != eVisInspectType.eInspTypeGlobalAlign &&
                    inspectionElement.InspectType != eVisInspectType.eInspTypeUnitAlign)
                {
                    defaultInspectionTypeList.Add(inspectionElement);
                }
            }
            return defaultInspectionTypeList;
        }

        [XmlIgnore]
        public UserControl SettingControl
        {
            get
            {
                if (m_ucSettingControl == null)
                {
                    m_ucSettingControl = InspectionType.GetInspectionTypeSettingDialog(this.InspectType);
                }
                return m_ucSettingControl;
            }
            set
            {
                m_ucSettingControl = value;
            }
        }
        private UserControl m_ucSettingControl;

        // Name에 의한 InspectionType 조회.
        public static InspectionType GetInspectionType(string aszName)
        {
            foreach (InspectionType inspectionTypeElement in m_inspectionTypeList)
            {
                if (inspectionTypeElement.Name == aszName)
                {
                    return inspectionTypeElement;
                }
            }
            return null;
        }

        // Enum 값에 의한 InspectionType 조회.
        public static InspectionType GetInspectionType(eVisInspectType inspectType)
        {
            foreach (InspectionType inspectionTypeElement in m_inspectionTypeList)
            {
                if (inspectionTypeElement.InspectType == inspectType)
                {
                    return inspectionTypeElement;
                }
            }
            return null;
        }

        // Setting UI를 반환한다.
        public static UserControl GetInspectionTypeSettingDialog(eVisInspectType inspectType)
        {
            foreach (InspectionType inspectionTypeElement in m_inspectionTypeList)
            {
                if (inspectionTypeElement.InspectType == inspectType)
                {
                    return inspectionTypeElement.SettingControl;
                }
            }
            return null;
        }

        // static ctor.
        static InspectionType()
        {
            // See InspectionResultTypeHelper.cs

            #region PSR이물 검사
            InspectionType PSROddItem = new InspectionType(
                "0450",
                "PSR이물 검사",
                eInspectPriority.eInspPriorityUnitPsr,   // priority
                eVisInspectType.eInspTypePSROdd,             // inspect type
                eVisInspectAlgo.eInspAlgoPSROdd,  // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.PSROdd());
            m_inspectionTypeList.Add(PSROddItem);
            #endregion

            #region Strip Align Item
            InspectionType stripAlignItem = new InspectionType(
                "0101",
                "Strip Align",
                eInspectPriority.eInspPriorityGlobalAlign,  // priority
                eVisInspectType.eInspTypeGlobalAlign,       // inspect type
                eVisInspectAlgo.eInspAlgoFeatureAlign,      // inspect algorithm type
                DefineBreakType.FINISH_STRIP,
                new InspectionTypeUI.FiducialAlign());
            m_inspectionTypeList.Add(stripAlignItem);
            #endregion

            #region 외곽 표면 검사
            InspectionType OuterItem = new InspectionType(
                "0211",
                "외곽 검사",
                eInspectPriority.eInspPriorityRailNormal,   // priority
                eVisInspectType.eInspTypeOuter,             // inspect type
                eVisInspectAlgo.eInspAlgoOuter,  // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.OuterState());
            m_inspectionTypeList.Add(OuterItem);
            #endregion

            #region 외곽 인식키
            InspectionType OuterFiducialItem = new InspectionType(
                "0205",
                "외곽 인식키",
                eInspectPriority.eInspPriorityRailNormal,   // priority
                eVisInspectType.eInspTypeRailFiducial,             // inspect type
                eVisInspectAlgo.eInspAlgoOuterFidu,  // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.OuterFiducial());
            m_inspectionTypeList.Add(OuterFiducialItem);
            #endregion

            #region ID Mark Item
            InspectionType IDItem = new InspectionType(
                "0113",
                "ID Mark",
                eInspectPriority.eInspPriorityGlobalNormal,   // priority
                eVisInspectType.eInspTypeIDMark,             // inspect type
                eVisInspectAlgo.eInspAlgoID,  // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.IDMark());
            m_inspectionTypeList.Add(IDItem);
            #endregion

            #region Unit Align Item
            InspectionType unitAlignItem = new InspectionType(
                "0401",
                "Unit Align",
                eInspectPriority.eInspPriorityUnitAlign,    // priority
                eVisInspectType.eInspTypeUnitAlign,         // inspect type
                eVisInspectAlgo.eInspAlgoFeatureAlign,      // inspect algorithm type
                DefineBreakType.FINISH_UNIT,
                new InspectionTypeUI.FiducialAlign());
            m_inspectionTypeList.Add(unitAlignItem);
            #endregion

            #region Dam Size Item
            InspectionType damItem = new InspectionType(
                "0433",
                "Dam Size 검사",
                eInspectPriority.eInspPriorityUnitNormal,   // priority
                eVisInspectType.eInspTypeDamBar,           // inspect type
                eVisInspectAlgo.eInspAlgoDamSize,  // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.DamSize());
            m_inspectionTypeList.Add(damItem);
            #endregion

            #region Surface Item
            InspectionType surfaceItem = new InspectionType(
                "0422",
                "표면 검사",
                eInspectPriority.eInspPriorityUnitNormal,   // priority
                eVisInspectType.eInspTypeSurface,           // inspect type
                eVisInspectAlgo.eInspAlgoStateAlignedMask,  // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.StateAlignedMask());
            m_inspectionTypeList.Add(surfaceItem);
            #endregion

            #region Via Item
            InspectionType viaItem = new InspectionType(
                "0460",
                "Via 검사",
                eInspectPriority.eInspPriorityUnitNormal,   // priority
                eVisInspectType.eInspTypeDownSet,           // inspect type
                eVisInspectAlgo.eInspAlgoStateAlignedMask,  // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.StateAlignedMask());
            m_inspectionTypeList.Add(viaItem);
            #endregion

            #region RawMetrial Item
            InspectionType rawmetrialItem = new InspectionType(
                "0440",
                "원소재 검사",
                eInspectPriority.eInspPriorityUnitRaw,   // priority
                eVisInspectType.eInspTypeUnitRawMaterial,           // inspect type
                eVisInspectAlgo.eInspAlgoStateAlignedMask,  // inspect algorithm type
                DefineBreakType.FINISH_UNIT,
                new InspectionTypeUI.RawMetrial());
            m_inspectionTypeList.Add(rawmetrialItem);
            #endregion

            #region Surface Item
            InspectionType psrItem = new InspectionType(
                "0431",
                "PSR 검사",
                eInspectPriority.eInspPriorityUnitNormal,   // priority
                eVisInspectType.eInspTypePSR,           // inspect type
                eVisInspectAlgo.eInspAlgoStateAlignedMask,  // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.StateAlignedMask());
            m_inspectionTypeList.Add(psrItem);
            #endregion

            #region Space Item
            InspectionType spaceItem = new InspectionType(
                "0423",
                "Core Crack 검사",
                eInspectPriority.eInspPriorityUnitNormal,   // priority
                eVisInspectType.eInspTypeSpace,             // inspect type
                eVisInspectAlgo.eInspAlgoStateAlignedMask,  // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.StateAlignedMask());
            m_inspectionTypeList.Add(spaceItem);
            #endregion

            #region Space Item
            InspectionType burrItem = new InspectionType(
                "0432",
                "Burr 검사",
                eInspectPriority.eInspPriorityUnitNormal,   // priority
                eVisInspectType.eInspTypePunchBurr,             // inspect type
                eVisInspectAlgo.eInspAlgoStateAlignedMask,  // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.StateAlignedMask());
            m_inspectionTypeList.Add(burrItem);
            #endregion

            #region Ball Pattern Item
            InspectionType ballItem = new InspectionType(
                "0429",
                "Ball 검사",
                eInspectPriority.eInspPriorityUnitNormal,   // priority
                eVisInspectType.eInspTypeBallPattern,             // inspect type
                eVisInspectAlgo.eInspAlgoBallPattern,  // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.BallPattern());
            m_inspectionTypeList.Add(ballItem);
            #endregion

            #region Vent Hole Item
            InspectionType VentHoleItem = new InspectionType(
                "0434",
                "Vent Hole 검사",
                eInspectPriority.eInspPriorityUnitNormal,   // priority
                eVisInspectType.eInspTypeVentHole,             // inspect type
                eVisInspectAlgo.eInspAlgoVentHole,  // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.VentHole());
            m_inspectionTypeList.Add(VentHoleItem);
            #endregion

            #region Vent Hole Item
            InspectionType OuterVentHoleItem = new InspectionType(
                "0441",
                "Vent Hole 검사2",
                eInspectPriority.eInspPriorityRailNormal,   // priority
                eVisInspectType.eInspTypeNoResizeVentHole,             // inspect type
                eVisInspectAlgo.eInspAlgoNoResizeVentHole,  // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.VentHole2());
            m_inspectionTypeList.Add(OuterVentHoleItem);
            #endregion

            #region Cross Pattern Item
            InspectionType CrossPatternItem = new InspectionType(
                "0411",
                "인식키 검사",
                eInspectPriority.eInspPriorityUnitNormal,   // priority
                eVisInspectType.eInspTypeUnitFidu,             // inspect type
                eVisInspectAlgo.eInspAlgoCrossPattern,  // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.CrossPattern());
            m_inspectionTypeList.Add(CrossPatternItem);
            #endregion

            #region Cross Pattern Item
            InspectionType UnitPatternItem = new InspectionType(
                "0407",
                "유닛인식키 검사",
                eInspectPriority.eInspPriorityUnitNormal,   // priority
                eVisInspectType.eInspTypeUnitPattern,             // inspect type
                eVisInspectAlgo.eInspAlgoUnitPattern,  // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.UnitPattern());
            m_inspectionTypeList.Add(UnitPatternItem);
            #endregion

            #region Window Punch Item
            InspectionType WindowPunchItem = new InspectionType(
                "0428",
                "Window Punch 검사",
                eInspectPriority.eInspPriorityUnitNormal,   // priority
                eVisInspectType.eInspTypeCoreCrack,             // inspect type
                eVisInspectAlgo.eInspAlgoWindowPunch,  // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.WindowPunch());
            m_inspectionTypeList.Add(WindowPunchItem);
            #endregion

            //#region Outer Rail Item
            //InspectionType railItem = new InspectionType(
            //    "0419",
            //    "외곽 검사",
            //    eInspectPriority.eInspPriorityRailNormal,   // priority
            //    eVisInspectType.eInspTypeRailIntensity,     // inspect type
            //    eVisInspectAlgo.eInspAlgoStateIntensity,    // inspect algorithm type
            //    DefineBreakType.FINISH_STRIP,
            //    new InspectionTypeUI.StateIntensity());
            //m_inspectionTypeList.Add(railItem);
            //#endregion

            #region Lead Shape With Center Line Item
            InspectionType leadShapeWithCLItem = new InspectionType(
                "0415",
                "리드 형상 검사",
                eInspectPriority.eInspPriorityUnitNormal,       // priority
                eVisInspectType.eInspTypeLeadShapeWithCL,       // inspect type
                eVisInspectAlgo.eInspAlgoLeadWithCenterLine,    // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.LeadShapeWithCenterLine());
            m_inspectionTypeList.Add(leadShapeWithCLItem);
            #endregion

            #region Space Shape With Center Line Item
            InspectionType spaceShiftCLItem = new InspectionType(
                "0436",
                "공간 형상 검사",
                eInspectPriority.eInspPriorityUnitNormal,       // priority
                eVisInspectType.eInspTypeSpaceShapeWithCL,      // inspect type
                eVisInspectAlgo.eInspAlgoLeadWithCenterLine,    // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.SpaceShapeWithCenterLine());
            m_inspectionTypeList.Add(spaceShiftCLItem);
            #endregion

            #region Groove Item
            //InspectionType grooveItem = new InspectionType(
            //    "0416",
            //    "Groove 검사",
            //    eInspectPriority.eInspPriorityUnitNormal,       // priority
            //    eVisInspectType.eInspTypeGroove,                // inspect type
            //    eVisInspectAlgo.eInspAlgoGrooveWithCenterLine,  // inspect algorithm type
            //    DefineBreakType.FINISH_NORMAL,
            //    new InspectionTypeUI.Groove());
            //m_inspectionTypeList.Add(grooveItem);
            #endregion

            #region Shape Half Etching
            //InspectionType shapeHalfEtchingItem = new InspectionType(
            //    "0417",
            //    "형상 Half Etching 검사",
            //    eInspectPriority.eInspPriorityUnitNormal,       // priority
            //    eVisInspectType.eInspTypeShapeHalfEtching,      // inspect type
            //    eVisInspectAlgo.eInspAlgoShapeHalfEtching,      // inspect algorithm type
            //    DefineBreakType.FINISH_NORMAL,
            //    new InspectionTypeUI.FigureShape());
            //m_inspectionTypeList.Add(shapeHalfEtchingItem);
            #endregion

            #region Surface Half Etching Item
            //InspectionType surfaceHalfEtchingItem = new InspectionType(
            //    "0418",
            //    "표면 Half Etching 검사",
            //    eInspectPriority.eInspPriorityUnitNormal,       // priority
            //    eVisInspectType.eInspTypeSurfaceHalfEtching,    // inspect type
            //    eVisInspectAlgo.eInspAlgoStateAlignedMask,      // inspect algorithm type
            //    DefineBreakType.FINISH_NORMAL,
            //    new InspectionTypeUI.StateAlignedMask());
            //m_inspectionTypeList.Add(surfaceHalfEtchingItem);
            #endregion

            #region PSR이물 검사
            InspectionType GV_Inspection = new InspectionType(
                "0490",
                "GV 검사",
                eInspectPriority.eInspPriorityUnitNormal,   // priority
                eVisInspectType.eInspResultGV,              // inspect type
                eVisInspectAlgo.eInspAlgoGV,                // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.GV_Inspection());
            m_inspectionTypeList.Add(GV_Inspection);
            #endregion

            #region Exceptional Mask
            InspectionType exceptionalMaskItem = new InspectionType(
                "0999",
                "검사 제외",
                eInspectPriority.eInspPriorityUnitNormal,       // priority
                eVisInspectType.eInspTypeExceptionalMask,       // inspect type
                eVisInspectAlgo.eInspAlgoExceptionalMask,       // inspect algorithm type
                DefineBreakType.FINISH_NORMAL,
                new InspectionTypeUI.ExceptionalMask());
            m_inspectionTypeList.Add(exceptionalMaskItem);
            #endregion
        }
        #endregion
    }
}
