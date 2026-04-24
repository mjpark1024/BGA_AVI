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

//////////////////////////////////////////////////////
// suoow2 Created.
//////////////////////////////////////////////////////
// History
// 2012.04.05 - 코드 정리 완료.
//////////////////////////////////////////////////////
// LeadGap 알고리즘 : 코드 3017
//////////////////////////////////////////////////////
// 리드 간격 검사 : 코드 0435
//////////////////////////////////////////////////////

namespace Common.Drawing.InspectionTypeUI
{
    public partial class LeadGap : UserControl, IInspectionTypeUICommands
    {
        private LeadGapProperty m_previewValue;

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

        public LeadGap()
        {
            InitializeComponent();
        }

        public void SetDialog(string strCaption, InspectionInformation.eVisInspectType inspectType)
        {
            this.txtCaption.Text = strCaption;
            this.m_enumInspectType = inspectType;
        }



        // 검사 설정 저장.
        public void TrySave(GraphicsBase graphic, int anInspectID, ParametersLocking paradata)
        {
            try
            {
                int lowerThresh = Convert.ToInt32(txtLowerThreshold.Text);
                int upperThresh = Convert.ToInt32(txtUpperThreshold.Text);

                if (lowerThresh > upperThresh)
                {
                    MessageBox.Show("임계 하한 값은 임계 상한 값을 초과할 수 없습니다.", "Information");
                    this.txtLowerThreshold.Focus();
                    return;
                }

                int minWidthSize = Convert.ToInt32(txtMinWidthSize.Text);

                LeadGapProperty oldLeadGapValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldLeadGapValue = element.InspectionAlgorithm as LeadGapProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldLeadGapValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    LeadGapProperty leadGapValue = new LeadGapProperty();

                    leadGapValue.LowerThresh = lowerThresh;
                    leadGapValue.UpperThresh = upperThresh;
                    leadGapValue.MinWidthSize = minWidthSize;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = leadGapValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldLeadGapValue.LowerThresh = lowerThresh;
                    oldLeadGapValue.UpperThresh = upperThresh;
                    oldLeadGapValue.MinWidthSize = minWidthSize;
                }

                if (m_previewValue == null)
                    m_previewValue = new LeadGapProperty();
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;
                m_previewValue.MinWidthSize = minWidthSize;
            }
            catch
            {
                MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
                txtLowerThreshold.Focus();
            }
        }

        // 검사 설정 저장.
        public void TryAdd(ref InspectList list, int anInspectID)
        {
            try
            {
                int lowerThresh = Convert.ToInt32(txtLowerThreshold.Text);
                int upperThresh = Convert.ToInt32(txtUpperThreshold.Text);

               // if (lowerThresh > upperThresh)
               // {
               //     MessageBox.Show("임계 하한 값은 임계 상한 값을 초과할 수 없습니다.", "Information");
               //     this.txtLowerThreshold.Focus();
               //     return;
               // }

                int minWidthSize = Convert.ToInt32(txtMinWidthSize.Text);

                LeadGapProperty oldLeadGapValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldLeadGapValue = element.InspectionAlgorithm as LeadGapProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldLeadGapValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    LeadGapProperty leadGapValue = new LeadGapProperty();

                    leadGapValue.LowerThresh = lowerThresh;
                    leadGapValue.UpperThresh = upperThresh;
                    leadGapValue.MinWidthSize = minWidthSize;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = leadGapValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldLeadGapValue.LowerThresh = lowerThresh;
                    oldLeadGapValue.UpperThresh = upperThresh;
                    oldLeadGapValue.MinWidthSize = minWidthSize;
                }

                if (m_previewValue == null)
                    m_previewValue = new LeadGapProperty();
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;
                m_previewValue.MinWidthSize = minWidthSize;
            }
            catch
            {
              //  MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
              //  txtLowerThreshold.Focus();
            }
        }

        // 검사 설정 직전 값 표시.
        public void SetPreviewValue()
        {
            if (m_previewValue != null)
            {
                this.txtLowerThreshold.Text = m_previewValue.LowerThresh.ToString();
                this.txtUpperThreshold.Text = m_previewValue.UpperThresh.ToString();
                this.txtMinWidthSize.Text = m_previewValue.MinWidthSize.ToString();
            }
        }

        // 검사 설정 기본 값 표시.
        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeLeadGap)
            {
                if (!LeadGapDefaultValue.DefaultValueLoaded)
                {
                    LeadGapDefaultValue.DefaultValueLoaded = true;
                    LeadGapDefaultValue.LoadDefaultValue();
                }

                this.txtLowerThreshold.Text = LeadGapDefaultValue.LowerThresh.ToString();
                this.txtUpperThreshold.Text = LeadGapDefaultValue.UpperThresh.ToString();
                this.txtMinWidthSize.Text = LeadGapDefaultValue.MinWidthSize.ToString();
            }
        }

        // 검사 설정 표시.
        public void Display(InspectionItem settingValue, int MarginX, int MatginY)
        {
            LeadGapProperty leadGapProperty = settingValue.InspectionAlgorithm as LeadGapProperty;
            if (leadGapProperty != null)
            {
                this.txtLowerThreshold.Text = leadGapProperty.LowerThresh.ToString();
                this.txtUpperThreshold.Text = leadGapProperty.UpperThresh.ToString();
                this.txtMinWidthSize.Text = leadGapProperty.MinWidthSize.ToString();
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
