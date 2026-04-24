using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Windows.Controls;

namespace Common.Drawing.MarkingInformation
{
    /// <summary>   Mark Information.  </summary>
    /// <remarks>   suoow2, 2014-10-09. </remarks>
    public abstract class MarkInfo
    {
        [XmlIgnore]
        protected string Code;

        public MarkInfo()
        {
            // for XML Serializing.
        }

        public abstract MarkInfo Clone();

        public abstract void LoadProperty(Strip strip, HPGL hpgl);
        public abstract int SaveProperty(ref HPGL hpgl);

        public abstract void LoadProperty(Strip strip, IDMARK text);
        public abstract int SaveProperty(ref IDMARK text);

        public abstract void LoadProperty(Strip strip, TEXT text);
        public abstract int SaveProperty(ref TEXT text);
        public abstract void LoadProperty(Strip strip, TEXT text, int loc);
    }

    /// <summary>   Mark type.  </summary>
    /// <remarks>   suoow2, 2014-10-09. </remarks>
    public class MarkingType
    {
        #region Properties.
        [XmlElement(ElementName = "Code")]
        public string Code { get; set; } // 검사 코드

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; } // 검사 이름

        [XmlElement(ElementName = "MarkType")]
        public eMarkingType MarkType; // 검사 항목

        [XmlElement(ElementName = "SecType")]
        public eMarkSectionType SecType; // 검사 알고리즘
        #endregion

        #region Constructors.
        // Ctor.
        public MarkingType()
        {
            if (SettingControl == null)
            {
                SettingControl = GetMarkTypeSettingDialog(MarkType);
            }
        }

        // Initialize.
        private MarkingType(string strCode, string strName, eMarkingType markType, eMarkSectionType secType, UserControl settingControl)
        {
            Code = strCode;
            Name = strName;

            MarkType = markType;
            SecType = secType;

            SettingControl = settingControl;
            if (SettingControl == null)
            {
                SettingControl = GetMarkTypeSettingDialog(markType);
            }
        }

        public MarkingType Clone()
        {
            MarkingType tmp = new MarkingType();
            tmp.Code = this.Code;
            tmp.Name = this.Name;
            tmp.MarkType = this.MarkType;
            tmp.SecType = this.SecType;
            tmp.SettingControl = GetMarkTypeSettingDialog(this.MarkType);
            return tmp;
        }
        #endregion

        #region Set Inspection types.
        // 검사 항목 리스트.
        private static List<MarkingType> m_markTypeList = new List<MarkingType>();

        /// <summary>   Gets the inspection type list. </summary>
        /// <remarks>   suoow2, 2014-10-09. </remarks>
        public static List<MarkingType> GetMarkTypeList()
        {
            return m_markTypeList;
        }

        // 일반 검사 영역에 대한 검사 설정 목록.
        public static List<MarkingType> GetDefaultMarkingTypeList()
        {
            List<MarkingType> defaultMaringTypeList = new List<MarkingType>();
            foreach (MarkingType markElement in m_markTypeList)
            {
                defaultMaringTypeList.Add(markElement);
            }
            return defaultMaringTypeList;
        }

        [XmlIgnore]
        public UserControl SettingControl
        {
            get
            {
                if (m_ucSettingControl == null)
                {
                    m_ucSettingControl = MarkingType.GetMarkTypeSettingDialog(this.MarkType);
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
        public static MarkingType GetMarkType(string aszName)
        {
            foreach (MarkingType markingTypeElement in m_markTypeList)
            {
                if (markingTypeElement.Name == aszName)
                {
                    return markingTypeElement;
                }
            }
            return null;
        }

        // Enum 값에 의한 InspectionType 조회.
        public static MarkingType GetMarkType(eMarkingType markType)
        {
            foreach (MarkingType inspectionTypeElement in m_markTypeList)
            {
                if (inspectionTypeElement.MarkType == markType)
                {
                    return inspectionTypeElement;
                }
            }
            return null;
        }

        // Setting UI를 반환한다.
        public static UserControl GetMarkTypeSettingDialog(eMarkingType markType)
        {
            foreach (MarkingType markingTypeElement in m_markTypeList)
            {
                if (markingTypeElement.MarkType == markType)
                {
                    return markingTypeElement.SettingControl;
                }
            }
            return null;
        }

        // static ctor.
        static MarkingType()
        {
            // See InspectionResultTypeHelper.cs

            #region Rail Circle Item
            MarkingType railCircleItem = new MarkingType(
                "0101",
                "원형 (Rail Mark)",
                eMarkingType.eMarkingRailCircle,       // inspect type
                eMarkSectionType.eSecTypeRail,      // inspect algorithm type
                new MarkingTypeUI.RailCircle());
            m_markTypeList.Add(railCircleItem);
            #endregion

            #region Rail Rectangle Item
            MarkingType railRectItem = new MarkingType(
                "0102",
                "사각 (Rail Mark)",
                eMarkingType.eMarkingRailRect,       // inspect type
                eMarkSectionType.eSecTypeRail,      // inspect algorithm type
                new MarkingTypeUI.RailRect());
            m_markTypeList.Add(railRectItem);
            #endregion
            #region Rail Triangle Item
            MarkingType railTriItem = new MarkingType(
                "0103",
                "삼각 (Rail Mark)",
                eMarkingType.eMarkingRailTri,       // inspect type
                eMarkSectionType.eSecTypeRail,      // inspect algorithm type
                new MarkingTypeUI.RailTri());
            m_markTypeList.Add(railTriItem);
            #endregion
            #region Rail Special Item
            MarkingType railSpecialItem = new MarkingType(
                "0104",
                "특수 (Rail Mark)",
                eMarkingType.eMarkingRailSpecial,       // inspect type
                eMarkSectionType.eSecTypeRail,      // inspect algorithm type
                new MarkingTypeUI.RailSpecial());
            m_markTypeList.Add(railSpecialItem);
            #endregion
            #region Unit Circle Item
            MarkingType unitCircleItem = new MarkingType(
                "0201",
                "원형 (Unit Mark)",
                eMarkingType.eMarkingUnitCircle,       // inspect type
                eMarkSectionType.eSecTypeUnit,      // inspect algorithm type
                new MarkingTypeUI.UnitCircle());
            m_markTypeList.Add(unitCircleItem);
            #endregion

            #region Unit Rectangle Item
            MarkingType unitRectItem = new MarkingType(
                "0202",
                "사각 (Unit Mark)",
                eMarkingType.eMarkingUnitRect,       // inspect type
                eMarkSectionType.eSecTypeUnit,      // inspect algorithm type
                new MarkingTypeUI.UnitRect());
            m_markTypeList.Add(unitRectItem);
            #endregion
            #region Unit Triangle Item
            MarkingType unitTriItem = new MarkingType(
                "0203",
                "삼각 (Unit Mark)",
                eMarkingType.eMarkingUnitTri,       // inspect type
                eMarkSectionType.eSecTypeUnit,      // inspect algorithm type
                new MarkingTypeUI.UnitTri());
            m_markTypeList.Add(unitTriItem);
            #endregion
            #region Unit Special Item
            MarkingType unitSpecialItem = new MarkingType(
                "0204",
                "특수 (Unit Mark)",
                eMarkingType.eMarkingUnitSpecial,       // inspect type
                eMarkSectionType.eSecTypeUnit,      // inspect algorithm type
                new MarkingTypeUI.UnitSpecial());
            m_markTypeList.Add(unitSpecialItem);
            #endregion
            #region Number mark Item
            MarkingType numberItem = new MarkingType(
                "0301",
                "수량마킹",
                eMarkingType.eMarkingNumber,       // inspect type
                eMarkSectionType.eSecTypeNum,      // inspect algorithm type
                new MarkingTypeUI.Number());
            m_markTypeList.Add(numberItem);
            #endregion
            #region Week mark Item
            MarkingType weekItem = new MarkingType(
                "0302",
                "주차마킹",
                eMarkingType.eMarkingWeek,       // inspect type
                eMarkSectionType.eSecTypeWeek,      // inspect algorithm type
                new MarkingTypeUI.Week());
            m_markTypeList.Add(weekItem);
            #endregion
            #region ID mark Item
            MarkingType idItem = new MarkingType(
                "0303",
                "ID마킹",
                eMarkingType.eMarkingIDMark,       // inspect type
                eMarkSectionType.eSecTypeID,      // inspect algorithm type
                new MarkingTypeUI.IDMark());
            m_markTypeList.Add(idItem);
            #endregion
            #region ID mark Item
            MarkingType tbdItem = new MarkingType(
                "0304",
                "2D마킹",
                eMarkingType.eMarkingTBD,       // inspect type
                eMarkSectionType.eSecTypeID,      // inspect algorithm type
                new MarkingTypeUI.TBD());
            m_markTypeList.Add(tbdItem);
            #endregion
            #region Unit Rectangle Item
            MarkingType unitGuideItem = new MarkingType(
                "0202",
                "Unit Guide",
                eMarkingType.eUnitGuide,       // inspect type
                eMarkSectionType.eSecTypeUnit,      // inspect algorithm type
                new MarkingTypeUI.UnitGuide());
            m_markTypeList.Add(unitGuideItem);
            #endregion
        }
        #endregion
    }
}
