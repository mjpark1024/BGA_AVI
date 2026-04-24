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
// History
// 2020.01.04 - 외곽 검사 간소화를 위해 생성
//////////////////////////////////////////////////////
// Outer 알고리즘 : 코드 3023
//////////////////////////////////////////////////////

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>
    /// OuterState.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OuterState : UserControl, IInspectionTypeUICommands
    {
        private OuterProperty m_previewValue;

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
        public OuterState()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
        }

        public void SetDialog(string strCaption, InspectionInformation.eVisInspectType inspectType)
        {
            this.m_enumInspectType = inspectType;
        }

        public void TrySave(GraphicsBase graphic, int anInspectID, ParametersLocking paradata)
        {
            try
            {
                #region Check 하한 / 상한 검출값
                int lowerThresh = Convert.ToInt32(txtInspThreshLower.Text);
                int upperThresh = Convert.ToInt32(txtInspThreshUpper.Text);
                
                #endregion

                #region Check 마스크 임계값
                int maskLowerThresh = Convert.ToInt32(txtMaskThreshLower.Text);
                int maskUpperThresh = Convert.ToInt32(txtMaskThreshUpper.Text);

                #endregion

                int erosionTrainIter = Convert.ToInt32(txtErosionTrainIter.Text);
                int dilationTrainIter = Convert.ToInt32(txtDilationTrainIter.Text);

                int minDefectSize =  Convert.ToInt32(txtMinDefectSize.Text);
                int maxDefectSize = Convert.ToInt32(txtMaxDefectSize.Text);

                OuterProperty oldOuterValue = null;

                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        graphic.BallSegments = element.BallSegments = null;
                        graphic.RefreshDrawing();
                        oldOuterValue = element.InspectionAlgorithm as OuterProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldOuterValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    OuterProperty outerValue = new OuterProperty();
                    outerValue.LowerThresh = lowerThresh;
                    outerValue.UpperThresh = upperThresh;

                    outerValue.MaskLowerThresh = maskLowerThresh;
                    outerValue.MaskUpperThresh = maskUpperThresh;

                    outerValue.ErosionTrainIter = erosionTrainIter;
                    outerValue.DilationTrainIter = dilationTrainIter;

                    outerValue.MinDefectSize = minDefectSize;
                    outerValue.MaxDefectSize = maxDefectSize;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = outerValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldOuterValue.LowerThresh = lowerThresh;
                    oldOuterValue.UpperThresh = upperThresh;
                    oldOuterValue.MaskLowerThresh = maskLowerThresh;
                    oldOuterValue.MaskUpperThresh = maskUpperThresh;
                    oldOuterValue.ErosionTrainIter = erosionTrainIter;
                    oldOuterValue.DilationTrainIter = dilationTrainIter;
                    oldOuterValue.MinDefectSize = minDefectSize;
                    oldOuterValue.MaxDefectSize = maxDefectSize;
                }

                if (m_previewValue == null)
                    m_previewValue = new OuterProperty();
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;
                m_previewValue.MaskLowerThresh = maskLowerThresh;
                m_previewValue.MaskUpperThresh = maskUpperThresh;
                m_previewValue.ErosionTrainIter = erosionTrainIter;
                m_previewValue.DilationTrainIter = dilationTrainIter;
                m_previewValue.MinDefectSize = minDefectSize;
                m_previewValue.MaxDefectSize = maxDefectSize;
            }
            catch
            {
                txtMaskThreshLower.Focus();
            }
        }

        // 검사 설정 저장.
        public void TryAdd(ref InspectList list, int anInspectID)
        {
            try
            {
                #region Check 하한 / 상한 검출값
                int lowerThresh = Convert.ToInt32(txtInspThreshLower.Text);
                int upperThresh = Convert.ToInt32(txtInspThreshUpper.Text);

                #endregion

                #region Check 마스크 임계값
                int maskLowerThresh = Convert.ToInt32(txtMaskThreshLower.Text);
                int maskUpperThresh = Convert.ToInt32(txtMaskThreshUpper.Text);

                #endregion

                int erosionTrainIter = Convert.ToInt32(txtErosionTrainIter.Text);
                int dilationTrainIter = Convert.ToInt32(txtDilationTrainIter.Text);

                int minDefectSize = Convert.ToInt32(txtMinDefectSize.Text);
                int maxDefectSize = Convert.ToInt32(txtMaxDefectSize.Text);

                OuterProperty oldOuterValue = null;

                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldOuterValue = element.InspectionAlgorithm as OuterProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldOuterValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    OuterProperty outerValue = new OuterProperty();
                    outerValue.LowerThresh = lowerThresh;
                    outerValue.UpperThresh = upperThresh;

                    outerValue.MaskLowerThresh = maskLowerThresh;
                    outerValue.MaskUpperThresh = maskUpperThresh;

                    outerValue.ErosionTrainIter = erosionTrainIter;
                    outerValue.DilationTrainIter = dilationTrainIter;

                    outerValue.MinDefectSize = minDefectSize;
                    outerValue.MaxDefectSize = maxDefectSize;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = outerValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldOuterValue.LowerThresh = lowerThresh;
                    oldOuterValue.UpperThresh = upperThresh;
                    oldOuterValue.MaskLowerThresh = maskLowerThresh;
                    oldOuterValue.MaskUpperThresh = maskUpperThresh;
                    oldOuterValue.ErosionTrainIter = erosionTrainIter;
                    oldOuterValue.DilationTrainIter = dilationTrainIter;
                    oldOuterValue.MinDefectSize = minDefectSize;
                    oldOuterValue.MaxDefectSize = maxDefectSize;
                }

                if (m_previewValue == null)
                    m_previewValue = new OuterProperty();
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;
                m_previewValue.MaskLowerThresh = maskLowerThresh;
                m_previewValue.MaskUpperThresh = maskUpperThresh;
                m_previewValue.ErosionTrainIter = erosionTrainIter;
                m_previewValue.DilationTrainIter = dilationTrainIter;
                m_previewValue.MinDefectSize = minDefectSize;
                m_previewValue.MaxDefectSize = maxDefectSize;
            }
            catch
            {
                //MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
                //if (txtThreshold.Visibility == Visibility.Visible)
                //{
                //    txtThreshold.Focus();
                //}
                //else
                //{
                //    txtLowerThreshold.Focus();
                //}
            }
        }

        // 검사의 기본 값 표시.
        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeOuter)
            {
                // 마스크 하한/상한 임계
                this.txtInspThreshLower.Text = OuterDefaultValue.LowerThresh.ToString();
                this.txtInspThreshUpper.Text = OuterDefaultValue.UpperThresh.ToString();
                this.txtMaskThreshLower.Text = OuterDefaultValue.MaskLowerThresh.ToString();
                this.txtMaskThreshUpper.Text = OuterDefaultValue.MaskUpperThresh.ToString();
                // 마스크 축소/확장, 결과 축소/확장.
                this.txtErosionTrainIter.Text = OuterDefaultValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = OuterDefaultValue.DilationTrainIter.ToString();
                // 최소 검출 사이즈
                this.txtMinDefectSize.Text = OuterDefaultValue.MinDefectSize.ToString();
                this.txtMaxDefectSize.Text = OuterDefaultValue.MaxDefectSize.ToString();
            }
        }

        public void SetPreviewValue()
        {
            if (m_previewValue != null)
            {
                this.txtInspThreshLower.Text = m_previewValue.LowerThresh.ToString();
                this.txtInspThreshUpper.Text = m_previewValue.UpperThresh.ToString();
                this.txtMaskThreshLower.Text = m_previewValue.MaskLowerThresh.ToString();
                this.txtMaskThreshUpper.Text = m_previewValue.MaskUpperThresh.ToString();
                this.txtErosionTrainIter.Text = m_previewValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = m_previewValue.DilationTrainIter.ToString();
                this.txtMinDefectSize.Text = m_previewValue.MinDefectSize.ToString();
                this.txtMaxDefectSize.Text = m_previewValue.MaxDefectSize.ToString();
            }
        }

        // 검사 설정 표시.
        public void Display(InspectionInformation.InspectionItem settingValue, int MarginX, int MatginY)
        {
            OuterProperty outerProperty = settingValue.InspectionAlgorithm as OuterProperty;
            if (outerProperty != null)
            {
                this.txtInspThreshLower.Text = outerProperty.LowerThresh.ToString();
                this.txtInspThreshUpper.Text = outerProperty.UpperThresh.ToString();
                this.txtMaskThreshLower.Text = outerProperty.MaskLowerThresh.ToString();
                this.txtMaskThreshUpper.Text = outerProperty.MaskUpperThresh.ToString();
                // 마스크 축소/확장, 결과 축소/확장
                this.txtErosionTrainIter.Text = outerProperty.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = outerProperty.DilationTrainIter.ToString();
                // 최소 검출 사이즈, 미세불량 검출 사이즈, 미세불량 검출 개수
                this.txtMinDefectSize.Text = outerProperty.MinDefectSize.ToString();
                this.txtMaxDefectSize.Text = outerProperty.MaxDefectSize.ToString();
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
