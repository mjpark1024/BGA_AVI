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
 * @file  InspectionItem.cs
 * @brief 
 *  검사 설정 아이템 관리 Base Class.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.10.09
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.10.09 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Common.Drawing.InspectionTypeUI;

namespace Common.Drawing.InspectionInformation
{
    /// <summary>   Inspection item.  </summary>
    [XmlInclude(typeof(FiducialAlignProperty))]
    [XmlInclude(typeof(LeadShapeWithCenterLineProperty))]
    [XmlInclude(typeof(SpaceShapeWithCenterLineProperty))]
    [XmlInclude(typeof(LeadGapProperty))]
    [XmlInclude(typeof(GrooveProperty))]
    [XmlInclude(typeof(FigureShapeProperty))]
    [XmlInclude(typeof(StateIntensityProperty))]
    [XmlInclude(typeof(StateAlignedMaskProperty))]
    [XmlInclude(typeof(ShapeShiftProperty))]
    [XmlInclude(typeof(ShapeShiftWithCLProperty))]
    [XmlInclude(typeof(ExceptionalMaskProperty))]
    public class InspectionItem : NotifyPropertyChanged
    {
        [XmlElement(ElementName = "SequenceID")]
        public int ID
        {
            get
            {
                return m_nID;
            }
            set
            {
                m_nID = value;
                Notify("ID");
            }
        }
        private int m_nID = 0;

        [XmlIgnore]
        public int? SentID { get; set; }

        [XmlElement(ElementName = "IsExceptionSkip")]
        public int IsExceptionSkip
        {
            get
            {
                return m_IsExceptionSkip;
            }
            set
            {
                m_IsExceptionSkip = value;
                Notify("IsExceptionSkip");
            }
        }
        private int m_IsExceptionSkip = 0;

        [XmlElement(ElementName = "InspectionType")]
        public InspectionType InspectionType
        {
            get;
            set;
        }

        [XmlElement(ElementName = "InspectionProperty")]
        public InspectionAlgorithm InspectionAlgorithm
        {
            get;
            set;
        }
        public bool is_visible
        {
            get;set;
        }
        [XmlElement(ElementName = "LineSegment")]
        public GraphicsSkeletonLine[] LineSegments
        {
            get { return lineSegments; }
            set { lineSegments = value; }
        }
        private GraphicsSkeletonLine[] lineSegments;

        [XmlElement(ElementName = "BallSegment")]
        public GraphicsSkeletonBall[] BallSegments
        {
            get { return ballSegments; }
            set { ballSegments = value; }
        }
        private GraphicsSkeletonBall[] ballSegments;

        #region Constructors.
        public InspectionItem()
        {
            // For XML Serializing.
        }
        #endregion

        public InspectionItem(InspectionType inspectionType, InspectionAlgorithm inspectionAlgorithm)
        {
            InspectionType = inspectionType;
            InspectionAlgorithm tmp = inspectionAlgorithm.Clone();
            InspectionAlgorithm = tmp;
        }

        public InspectionItem(InspectionType inspectionType, InspectionAlgorithm inspectionAlgorithm, int id)
        {
            m_nID = id;
            InspectionType = inspectionType;
            InspectionAlgorithm = inspectionAlgorithm.Clone();
        }

        // To Database.
        public int SaveInspectItem(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID)
        {
            return InspectionAlgorithm.SaveProperty(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, InspectionType.Code);
        }
    }
}
