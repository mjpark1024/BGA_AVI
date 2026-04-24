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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Common.Drawing.InspectionInformation;
using Common.DataBase;

//////////////////////////////////////////////////////
// suoow2 Created.
//////////////////////////////////////////////////////
// History
// 2012.04.05 - 코드 정리 완료.
//////////////////////////////////////////////////////
// CrossPattern 알고리즘 : 코드 3003
//////////////////////////////////////////////////////
// 인식키 검사 : 코드 0402
//////////////////////////////////////////////////////

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>
    /// UnitPattern.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UnitPattern : UserControl, IInspectionTypeUICommands
    {
        private UnitPatternProperty m_previewValue;

        #region Inspect Type
        public eVisInspectType InspectType
        {
            get
            {
                return m_enumInspectType;
            }
        }
        private eVisInspectType m_enumInspectType;
        #endregion


        public UnitPattern()
        {
            InitializeComponent();
        }
        public void SetDialog(string strCaption, eVisInspectType inspectType)
        {
            //this.txtCaption.Text = strCaption;
            this.m_enumInspectType = inspectType;
        }

        // 검사 설정 저장.
        public void TrySave(GraphicsBase graphic, int anInspectID, ParametersLocking paradata)
        {
            try
            {
                int lowerThresh = Convert.ToInt32(txtThreshold.Text);
                int minDefectSize = Convert.ToInt32(txtMatch.Text);

                UnitPatternProperty oldUnitPatternValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        // 검사 파라미터 수정으로 간주하고 LineSegment를 삭제한다.
                        graphic.LineSegments = element.LineSegments = null;
                        graphic.BallSegments = element.BallSegments = null;
                        graphic.RefreshDrawing();

                        oldUnitPatternValue = element.InspectionAlgorithm as UnitPatternProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldUnitPatternValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    UnitPatternProperty unitPatternValue = new UnitPatternProperty();
                    unitPatternValue.LowerThresh = lowerThresh;
                    unitPatternValue.MinDefectSize = minDefectSize;
                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = unitPatternValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldUnitPatternValue.LowerThresh = lowerThresh;
                    oldUnitPatternValue.MinDefectSize = minDefectSize;
                }

                if (m_previewValue == null)
                    m_previewValue = new UnitPatternProperty();
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.MinDefectSize = minDefectSize;
            }
            catch
            {
                MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
                txtMatch.Focus();
            }
        }

        // 검사 설정 저장.
        public void TryAdd(ref InspectList list, int anInspectID)
        {
            try
            {
                int lowerThresh = Convert.ToInt32(txtThreshold.Text);
                int minDefectSize = Convert.ToInt32(txtMatch.Text);

                UnitPatternProperty oldUnitPatternValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldUnitPatternValue = element.InspectionAlgorithm as UnitPatternProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldUnitPatternValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    UnitPatternProperty unitPatternValue = new UnitPatternProperty();
                    unitPatternValue.LowerThresh = lowerThresh;
                    unitPatternValue.MinDefectSize = minDefectSize;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = unitPatternValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldUnitPatternValue.LowerThresh = lowerThresh;
                    oldUnitPatternValue.MinDefectSize = minDefectSize;
                }

                if (m_previewValue == null)
                    m_previewValue = new UnitPatternProperty();
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.MinDefectSize = minDefectSize;
            }
            catch
            {
                //MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
                //txtLowerThreshold.Focus();
            }
        }

        // 검사의 직전 값 표시.
        public void SetPreviewValue()
        {
            if (m_previewValue != null)
            {
                this.txtThreshold.Text = m_previewValue.LowerThresh.ToString();
                this.txtMatch.Text = m_previewValue.MinDefectSize.ToString();
            }
        }

        // 검사의 기본 값 표시.
        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeUnitPattern)
            {
                if (!UnitPatternDefaultValue.DefaultValueLoaded)
                {
                    UnitPatternDefaultValue.DefaultValueLoaded = true;
                    UnitPatternDefaultValue.LoadDefaultValue();
                }

                this.txtThreshold.Text = UnitPatternDefaultValue.LowerThresh.ToString();
                this.txtMatch.Text = UnitPatternDefaultValue.MinDefectSize.ToString();
            }
        }

        // 검사 설정 표시.
        public void Display(InspectionItem settingValue, int MarginX, int MatginY)
        {
            UnitPatternProperty unitPatternValue = settingValue.InspectionAlgorithm as UnitPatternProperty;
            if (unitPatternValue != null)
            {
                this.txtThreshold.Text = unitPatternValue.LowerThresh.ToString();
                this.txtMatch.Text = unitPatternValue.MinDefectSize.ToString();
            }
        }
        public void AllCircuitPartChange(GraphicsBase graphic, int anInspectID)
        {
        }
        public void AllNonCircuitPartChange(GraphicsBase graphic, int anInspectID)
        {
        }
    }
}

