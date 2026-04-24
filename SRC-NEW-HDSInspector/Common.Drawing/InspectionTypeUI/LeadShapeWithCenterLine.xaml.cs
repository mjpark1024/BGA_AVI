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
// LeadShapeWithCenterLine 알고리즘 : 코드 3008
//////////////////////////////////////////////////////
// 리드 형상 검사 : 코드 0415
//////////////////////////////////////////////////////

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>   Lead shape with center line.  </summary>
    /// <remarks>   suoow2, 2014-10-10. </remarks>
    public partial class LeadShapeWithCenterLine : UserControl, IInspectionTypeUICommands
    {
        private LeadShapeWithCenterLineProperty m_previewValue;

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

        public LeadShapeWithCenterLine()
        {
            InitializeComponent();
        }

        public void SetDialog(string strCaption, eVisInspectType inspectType)
        {
            this.txtCaption.Text = strCaption;
            this.m_enumInspectType = inspectType;
        }

        // 검사 설정 저장.
        public void TrySave(GraphicsBase graphic, int anInspectID, ParametersLocking paradata)
        {
            try
            {
                int threshType = 2;

                int lowerThresh = Convert.ToInt32(txtLowerThreshold.Text);
                int upperThresh = Convert.ToInt32(txtUpperThreshold.Text);

                if (lowerThresh > upperThresh)
                {
                    MessageBox.Show("임계 하한 값은 임계 상한 값을 초과할 수 없습니다.", "Information");
                    this.txtLowerThreshold.Focus();
                    return;
                }

                int masklowerThresh = Convert.ToInt32(txtThreshProject.Text);
                int minDefectSzie = Convert.ToInt32(txtProjectSize.Text);

                //int minNormalRatio = Convert.ToInt32(txtMinNormalRatio.Text);
                //int maxNormalRatio = Convert.ToInt32(txtMaxNormalRatio.Text);

                //if (minNormalRatio > maxNormalRatio)
                //{
                //    MessageBox.Show("리드편차 최소 허용범위는 리드편차 최대 허용범위를 초과할 수 없습니다.", "Information");
                //    this.txtMinNormalRatio.Focus();
                //    return;
                //}

                int minHeightSize = Convert.ToInt32(txtMinHeightSize.Text);

                int minWidthRatio = Convert.ToInt32(txtMinWidthRatio.Text);
                //int maxWidthRatio = Convert.ToInt32(txtMaxWidthRatio.Text);

                //if (minWidthRatio > maxWidthRatio)
                //{
                //    MessageBox.Show("리드 최소 선폭범위는 리드 최대 선폭범위를 초과할 수 없습니다.", "Information");
                //    this.txtMinWidthRatio.Focus();
                //    return;
                //}

                int minWidthSize = Convert.ToInt32(txtMinWidthSize.Text);
                int maxWidthSize = Convert.ToInt32(txtMaxWidthSize.Text);

                if (minWidthSize > maxWidthSize)
                {
                    MessageBox.Show("리드 최소 선폭은 리드 최대 선폭을 초과할 수 없습니다.", "Information");
                    this.txtMinWidthSize.Focus();
                    return;
                }

                int removetip = Convert.ToInt32(txtTipSize.Text);

                LeadShapeWithCenterLineProperty oldLeadWithCenterLineValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        // 검사 파라미터 수정으로 간주하고 LineSegment를 삭제한다.
                        graphic.LineSegments = element.LineSegments = null;
                        graphic.BallSegments = element.BallSegments = null;
                        graphic.RefreshDrawing();

                        oldLeadWithCenterLineValue = element.InspectionAlgorithm as LeadShapeWithCenterLineProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }
                if (minWidthRatio > paradata.MaxLead)
                {
                    if (oldLeadWithCenterLineValue != null)
                    {
                        minWidthRatio = oldLeadWithCenterLineValue.MinWidthRatio;
                    }
                    else
                    {
                        minWidthRatio = paradata.MaxLead;
                    }
                    txtMinWidthRatio.Text = minWidthRatio.ToString();
                    txtMinWidthRatio.Focus();
                    MessageBox.Show("파라매터 제한 값 확인 바랍니다.", "Information");
                }
                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldLeadWithCenterLineValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    LeadShapeWithCenterLineProperty leadWithCenterLineValue = new LeadShapeWithCenterLineProperty();

                    leadWithCenterLineValue.ThreshType = threshType;
                    leadWithCenterLineValue.LowerThresh = lowerThresh;
                    leadWithCenterLineValue.UpperThresh = upperThresh;

                    leadWithCenterLineValue.MaskLowerThresh = masklowerThresh;
                    leadWithCenterLineValue.MinDefectSize = minDefectSzie;

                    leadWithCenterLineValue.MinWidthRatio = minWidthRatio;
                    leadWithCenterLineValue.MaxWidthRatio = 100;// maxWidthRatio;

                    leadWithCenterLineValue.MinHeightsize = minHeightSize;

                    leadWithCenterLineValue.MinNormalRatio = 100;// minNormalRatio;
                    leadWithCenterLineValue.MaxNormalRatio = 100;// maxNormalRatio;

                    leadWithCenterLineValue.MinWidthSize = minWidthSize;
                    leadWithCenterLineValue.MaxWidthSize = maxWidthSize;
                    leadWithCenterLineValue.RemoveTipSize = removetip;
                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = leadWithCenterLineValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldLeadWithCenterLineValue.ThreshType = threshType;
                    oldLeadWithCenterLineValue.LowerThresh = lowerThresh;
                    oldLeadWithCenterLineValue.UpperThresh = upperThresh;

                    oldLeadWithCenterLineValue.MaskLowerThresh = masklowerThresh;
                    oldLeadWithCenterLineValue.MinDefectSize = minDefectSzie;

                    oldLeadWithCenterLineValue.MinWidthRatio = minWidthRatio;
                    oldLeadWithCenterLineValue.MaxWidthRatio = 100;// maxWidthRatio;

                    oldLeadWithCenterLineValue.MinHeightsize = minHeightSize;

                    oldLeadWithCenterLineValue.MinNormalRatio = 100;// minNormalRatio;
                    oldLeadWithCenterLineValue.MaxNormalRatio = 100;// maxNormalRatio;

                    oldLeadWithCenterLineValue.MinWidthSize = minWidthSize;
                    oldLeadWithCenterLineValue.MaxWidthSize = maxWidthSize;
                    oldLeadWithCenterLineValue.RemoveTipSize = removetip;
                }

                if (m_previewValue == null)
                    m_previewValue = new LeadShapeWithCenterLineProperty();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;

                m_previewValue.MaskLowerThresh = masklowerThresh;
                m_previewValue.MinDefectSize = minDefectSzie;

                m_previewValue.MinWidthRatio = minWidthRatio;
                m_previewValue.MaxWidthRatio = 100;// maxWidthRatio;

                m_previewValue.MinHeightsize = minHeightSize;

                m_previewValue.MinNormalRatio = 100;// minNormalRatio;
                m_previewValue.MaxNormalRatio = 100;// maxNormalRatio;

                m_previewValue.MinWidthSize = minWidthSize;
                m_previewValue.MaxWidthSize = maxWidthSize;
                m_previewValue.RemoveTipSize = removetip;
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
                int threshType = 2;

                int lowerThresh = Convert.ToInt32(txtLowerThreshold.Text);
                int upperThresh = Convert.ToInt32(txtUpperThreshold.Text);

                int masklowerThresh = Convert.ToInt32(txtThreshProject.Text);
                int minDefectSzie = Convert.ToInt32(txtProjectSize.Text);
               // if (lowerThresh > upperThresh)
               // {
               //     MessageBox.Show("임계 하한 값은 임계 상한 값을 초과할 수 없습니다.", "Information");
               //     this.txtLowerThreshold.Focus();
               //     return;
               // }
                int minHeightSize = Convert.ToInt32(txtMinHeightSize.Text);
                int minWidthRatio = Convert.ToInt32(txtMinWidthRatio.Text);
                int minWidthSize = Convert.ToInt32(txtMinWidthSize.Text);
                int maxWidthSize = Convert.ToInt32(txtMaxWidthSize.Text);
                int removetip = Convert.ToInt32(txtTipSize.Text);
                //if (minWidthSize > maxWidthSize)
                //{
                //    MessageBox.Show("리드 최소 선폭은 리드 최대 선폭을 초과할 수 없습니다.", "Information");
                //    this.txtMinWidthSize.Focus();
                //    return;
                //}

                LeadShapeWithCenterLineProperty oldLeadWithCenterLineValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldLeadWithCenterLineValue = element.InspectionAlgorithm as LeadShapeWithCenterLineProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldLeadWithCenterLineValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    LeadShapeWithCenterLineProperty leadWithCenterLineValue = new LeadShapeWithCenterLineProperty();

                    leadWithCenterLineValue.ThreshType = threshType;
                    leadWithCenterLineValue.LowerThresh = lowerThresh;
                    leadWithCenterLineValue.UpperThresh = upperThresh;

                    leadWithCenterLineValue.MaskLowerThresh = masklowerThresh;
                    leadWithCenterLineValue.MinDefectSize = minDefectSzie;

                    leadWithCenterLineValue.MinWidthRatio = minWidthRatio;
                    leadWithCenterLineValue.MaxWidthRatio = 100;// maxWidthRatio;

                    leadWithCenterLineValue.MinHeightsize = minHeightSize;

                    leadWithCenterLineValue.MinNormalRatio = 100;// minNormalRatio;
                    leadWithCenterLineValue.MaxNormalRatio = 100;// maxNormalRatio;

                    leadWithCenterLineValue.MinWidthSize = minWidthSize;
                    leadWithCenterLineValue.MaxWidthSize = maxWidthSize;
                    leadWithCenterLineValue.RemoveTipSize = removetip;
                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = leadWithCenterLineValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldLeadWithCenterLineValue.ThreshType = threshType;
                    oldLeadWithCenterLineValue.LowerThresh = lowerThresh;
                    oldLeadWithCenterLineValue.UpperThresh = upperThresh;

                    oldLeadWithCenterLineValue.MaskLowerThresh = masklowerThresh;
                    oldLeadWithCenterLineValue.MinDefectSize = minDefectSzie;

                    oldLeadWithCenterLineValue.MinWidthRatio = minWidthRatio;
                    oldLeadWithCenterLineValue.MaxWidthRatio = 100;// maxWidthRatio;

                    oldLeadWithCenterLineValue.MinHeightsize = minHeightSize;

                    oldLeadWithCenterLineValue.MinNormalRatio = 100;// minNormalRatio;
                    oldLeadWithCenterLineValue.MaxNormalRatio = 100;// maxNormalRatio;

                    oldLeadWithCenterLineValue.MinWidthSize = minWidthSize;
                    oldLeadWithCenterLineValue.MaxWidthSize = maxWidthSize;
                    oldLeadWithCenterLineValue.RemoveTipSize = removetip;
                }

                if (m_previewValue == null)
                    m_previewValue = new LeadShapeWithCenterLineProperty();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;

                m_previewValue.MaskLowerThresh = masklowerThresh;
                m_previewValue.MinDefectSize = minDefectSzie;

                m_previewValue.MinWidthRatio = minWidthRatio;
                m_previewValue.MaxWidthRatio = 100;// maxWidthRatio;

                m_previewValue.MinHeightsize = minHeightSize;

                m_previewValue.MinNormalRatio = 100;// minNormalRatio;
                m_previewValue.MaxNormalRatio = 100;// maxNormalRatio;

                m_previewValue.MinWidthSize = minWidthSize;
                m_previewValue.MaxWidthSize = maxWidthSize;
                m_previewValue.RemoveTipSize = removetip;
            }
            catch
            {
               // MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
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

                this.txtThreshProject.Text = m_previewValue.MaskLowerThresh.ToString();
                this.txtProjectSize.Text = m_previewValue.MinDefectSize.ToString();

                this.txtMinWidthRatio.Text = m_previewValue.MinWidthRatio.ToString();
                this.txtMaxWidthRatio.Text = m_previewValue.MaxWidthRatio.ToString();

                this.txtMinHeightSize.Text = m_previewValue.MinHeightsize.ToString();
                
                this.txtMinNormalRatio.Text = m_previewValue.MinNormalRatio.ToString();
                this.txtMaxNormalRatio.Text = m_previewValue.MaxNormalRatio.ToString();

                this.txtMinWidthSize.Text = m_previewValue.MinWidthSize.ToString();
                this.txtMaxWidthSize.Text = m_previewValue.MaxWidthSize.ToString();
                this.txtTipSize.Text = m_previewValue.RemoveTipSize.ToString();
            }
        }

        // 검사 설정 기본 값 표시.
        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeLeadShapeWithCL)
            {
                if (!LeadShapeWithCenterLineDefaultValue.DefaultValueLoaded)
                {
                    LeadShapeWithCenterLineDefaultValue.DefaultValueLoaded = true;
                    LeadShapeWithCenterLineDefaultValue.LoadDefaultValue();
                }

                this.txtLowerThreshold.Text = LeadShapeWithCenterLineDefaultValue.LowerThresh.ToString();
                this.txtUpperThreshold.Text = LeadShapeWithCenterLineDefaultValue.UpperThresh.ToString();

                this.txtThreshProject.Text = LeadShapeWithCenterLineDefaultValue.MaskLowerThresh.ToString();
                this.txtProjectSize.Text = LeadShapeWithCenterLineDefaultValue.MinDefectSize.ToString();

                this.txtMinWidthRatio.Text = LeadShapeWithCenterLineDefaultValue.MinWidthRatio.ToString();
                this.txtMaxWidthRatio.Text = LeadShapeWithCenterLineDefaultValue.MaxWidthRatio.ToString();

                this.txtMinHeightSize.Text = LeadShapeWithCenterLineDefaultValue.MinHeightsize.ToString();
                
                this.txtMinNormalRatio.Text = LeadShapeWithCenterLineDefaultValue.MinNormalRatio.ToString();
                this.txtMaxNormalRatio.Text = LeadShapeWithCenterLineDefaultValue.MaxNormalRatio.ToString();

                this.txtMinWidthSize.Text = LeadShapeWithCenterLineDefaultValue.MinWidthSize.ToString();
                this.txtMaxWidthSize.Text = LeadShapeWithCenterLineDefaultValue.MaxWidthSize.ToString();
                this.txtTipSize.Text = LeadShapeWithCenterLineDefaultValue.RemoveTipSize.ToString();
            }
        }

        // 검사 설정 표시.
        public void Display(InspectionItem settingValue, int MarginX, int MatginY)
        {
            LeadShapeWithCenterLineProperty leadWithCenterLineValue = settingValue.InspectionAlgorithm as LeadShapeWithCenterLineProperty;
            if (leadWithCenterLineValue != null)
            {
                this.txtLowerThreshold.Text = leadWithCenterLineValue.LowerThresh.ToString();
                this.txtUpperThreshold.Text = leadWithCenterLineValue.UpperThresh.ToString();

                this.txtThreshProject.Text = leadWithCenterLineValue.MaskLowerThresh.ToString();
                this.txtProjectSize.Text = leadWithCenterLineValue.MinDefectSize.ToString();

                // 공간편차 최소/최대 허용범위
                this.txtMinWidthRatio.Text = leadWithCenterLineValue.MinWidthRatio.ToString();
                this.txtMaxWidthRatio.Text = leadWithCenterLineValue.MaxWidthRatio.ToString();

                // 연속 불량 개수
                this.txtMinHeightSize.Text = leadWithCenterLineValue.MinHeightsize.ToString();
                
                // 리드 최소/최대 선폭범위
                this.txtMinNormalRatio.Text = leadWithCenterLineValue.MinNormalRatio.ToString();
                this.txtMaxNormalRatio.Text = leadWithCenterLineValue.MaxNormalRatio.ToString();

                // 리드 최소/최대 선폭
                this.txtMinWidthSize.Text = leadWithCenterLineValue.MinWidthSize.ToString();
                this.txtMaxWidthSize.Text = leadWithCenterLineValue.MaxWidthSize.ToString();
                this.txtTipSize.Text = leadWithCenterLineValue.RemoveTipSize.ToString();
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
