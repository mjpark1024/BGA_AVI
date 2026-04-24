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

namespace Common.Drawing.InspectionTypeUI
{
    public partial class Groove : UserControl, IInspectionTypeUICommands
    {
        private GrooveProperty m_previewValue;

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

        /// <summary> The default thresh type </summary>
        private static int DEFAULT_THRESH_TYPE = 2;

        public Groove()
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
                int threshType = DEFAULT_THRESH_TYPE;

                int lowerThresh = Convert.ToInt32(txtLowerThreshold.Text);
                int upperThresh = Convert.ToInt32(txtUpperThreshold.Text);

                if (lowerThresh > upperThresh)
                {
                    MessageBox.Show("임계 하한 값은 임계 상한 값을 초과할 수 없습니다.", "Information");
                    this.txtLowerThreshold.Focus();
                    return;
                }

                int applyAverDiff = (chkApplyAverDiff.IsChecked == true) ? 1 : 0;
                int averMinMargin = (applyAverDiff == 1) ? Convert.ToInt32(txtAverMinMargin.Text) : 0;
                int averMaxMargin = (applyAverDiff == 1) ? Convert.ToInt32(txtAverMaxMargin.Text) : 0;

                int minWidthRatio = Convert.ToInt32(txtMinWidthRatio.Text);
                int minWidthSize = Convert.ToInt32(txtMinWidthSize.Text);
                int maxWidthRatio = Convert.ToInt32(txtMaxWidthRatio.Text);
                int maxWidthSize = Convert.ToInt32(txtMaxWidthSize.Text);
                int minHeightSize = Convert.ToInt32(txtMinHeightSize.Text);
                int maxHeightRatio = Convert.ToInt32(txtMaxHeightRatio.Text);
                int maxHeightSize = Convert.ToInt32(txtMaxHeightSize.Text);
                int criterionSize = Convert.ToInt32(txtCriterionSize.Text);
                int minNormalRatio = Convert.ToInt32(txtMinNormalRatio.Text);
                int maxNormalRatio = Convert.ToInt32(txtMaxNormalRatio.Text);

                GrooveProperty oldGrooveValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldGrooveValue = element.InspectionAlgorithm as GrooveProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldGrooveValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    GrooveProperty grooveValue = new GrooveProperty();

                    grooveValue.ThreshType = threshType;
                    grooveValue.LowerThresh = lowerThresh;
                    grooveValue.UpperThresh = upperThresh;
                    grooveValue.ApplyAverDiff = applyAverDiff;
                    grooveValue.AverMinMargin = averMinMargin;
                    grooveValue.AverMaxMargin = averMaxMargin;
                    grooveValue.MinWidthRatio = minWidthRatio;
                    grooveValue.MinWidthSize = minWidthSize;
                    grooveValue.MaxWidthRatio = maxWidthRatio;
                    grooveValue.MaxWidthSize = maxWidthSize;
                    grooveValue.MinHeightSize = minHeightSize;
                    grooveValue.MaxHeightRatio = maxHeightRatio;
                    grooveValue.MaxHeightSize = maxHeightSize;
                    grooveValue.CriterionSize = criterionSize;
                    grooveValue.MinNormalRatio = minNormalRatio;
                    grooveValue.MaxNormalRatio = maxNormalRatio;


                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = grooveValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldGrooveValue.ThreshType = threshType;
                    oldGrooveValue.LowerThresh = lowerThresh;
                    oldGrooveValue.UpperThresh = upperThresh;
                    oldGrooveValue.ApplyAverDiff = applyAverDiff;
                    oldGrooveValue.AverMinMargin = averMinMargin;
                    oldGrooveValue.AverMaxMargin = averMaxMargin;
                    oldGrooveValue.MinWidthRatio = minWidthRatio;
                    oldGrooveValue.MinWidthSize = minWidthSize;
                    oldGrooveValue.MaxWidthRatio = maxWidthRatio;
                    oldGrooveValue.MaxWidthSize = maxWidthSize;
                    oldGrooveValue.MinHeightSize = minHeightSize;
                    oldGrooveValue.MaxHeightRatio = maxHeightRatio;
                    oldGrooveValue.MaxHeightSize = maxHeightSize;
                    oldGrooveValue.CriterionSize = criterionSize;
                    oldGrooveValue.MinNormalRatio = minNormalRatio;
                    oldGrooveValue.MaxNormalRatio = maxNormalRatio;
                }

                if (m_previewValue == null)
                    m_previewValue = new GrooveProperty();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;
                m_previewValue.ApplyAverDiff = applyAverDiff;
                m_previewValue.AverMinMargin = averMinMargin;
                m_previewValue.AverMaxMargin = averMaxMargin;
                m_previewValue.MinWidthRatio = minWidthRatio;
                m_previewValue.MinWidthSize = minWidthSize;
                m_previewValue.MaxWidthRatio = maxWidthRatio;
                m_previewValue.MaxWidthSize = maxWidthSize;
                m_previewValue.MinHeightSize = minHeightSize;
                m_previewValue.MaxHeightRatio = maxHeightRatio;
                m_previewValue.MaxHeightSize = maxHeightSize;
                m_previewValue.CriterionSize = criterionSize;
                m_previewValue.MinNormalRatio = minNormalRatio;
                m_previewValue.MaxNormalRatio = maxNormalRatio;
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
                int threshType = DEFAULT_THRESH_TYPE;

                int lowerThresh = Convert.ToInt32(txtLowerThreshold.Text);
                int upperThresh = Convert.ToInt32(txtUpperThreshold.Text);


                int applyAverDiff = (chkApplyAverDiff.IsChecked == true) ? 1 : 0;
                int averMinMargin = (applyAverDiff == 1) ? Convert.ToInt32(txtAverMinMargin.Text) : 0;
                int averMaxMargin = (applyAverDiff == 1) ? Convert.ToInt32(txtAverMaxMargin.Text) : 0;

                int minWidthRatio = Convert.ToInt32(txtMinWidthRatio.Text);
                int minWidthSize = Convert.ToInt32(txtMinWidthSize.Text);
                int maxWidthRatio = Convert.ToInt32(txtMaxWidthRatio.Text);
                int maxWidthSize = Convert.ToInt32(txtMaxWidthSize.Text);
                int minHeightSize = Convert.ToInt32(txtMinHeightSize.Text);
                int maxHeightRatio = Convert.ToInt32(txtMaxHeightRatio.Text);
                int maxHeightSize = Convert.ToInt32(txtMaxHeightSize.Text);
                int criterionSize = Convert.ToInt32(txtCriterionSize.Text);
                int minNormalRatio = Convert.ToInt32(txtMinNormalRatio.Text);
                int maxNormalRatio = Convert.ToInt32(txtMaxNormalRatio.Text);

                GrooveProperty oldGrooveValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldGrooveValue = element.InspectionAlgorithm as GrooveProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldGrooveValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    GrooveProperty grooveValue = new GrooveProperty();

                    grooveValue.ThreshType = threshType;
                    grooveValue.LowerThresh = lowerThresh;
                    grooveValue.UpperThresh = upperThresh;
                    grooveValue.ApplyAverDiff = applyAverDiff;
                    grooveValue.AverMinMargin = averMinMargin;
                    grooveValue.AverMaxMargin = averMaxMargin;
                    grooveValue.MinWidthRatio = minWidthRatio;
                    grooveValue.MinWidthSize = minWidthSize;
                    grooveValue.MaxWidthRatio = maxWidthRatio;
                    grooveValue.MaxWidthSize = maxWidthSize;
                    grooveValue.MinHeightSize = minHeightSize;
                    grooveValue.MaxHeightRatio = maxHeightRatio;
                    grooveValue.MaxHeightSize = maxHeightSize;
                    grooveValue.CriterionSize = criterionSize;
                    grooveValue.MinNormalRatio = minNormalRatio;
                    grooveValue.MaxNormalRatio = maxNormalRatio;


                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = grooveValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldGrooveValue.ThreshType = threshType;
                    oldGrooveValue.LowerThresh = lowerThresh;
                    oldGrooveValue.UpperThresh = upperThresh;
                    oldGrooveValue.ApplyAverDiff = applyAverDiff;
                    oldGrooveValue.AverMinMargin = averMinMargin;
                    oldGrooveValue.AverMaxMargin = averMaxMargin;
                    oldGrooveValue.MinWidthRatio = minWidthRatio;
                    oldGrooveValue.MinWidthSize = minWidthSize;
                    oldGrooveValue.MaxWidthRatio = maxWidthRatio;
                    oldGrooveValue.MaxWidthSize = maxWidthSize;
                    oldGrooveValue.MinHeightSize = minHeightSize;
                    oldGrooveValue.MaxHeightRatio = maxHeightRatio;
                    oldGrooveValue.MaxHeightSize = maxHeightSize;
                    oldGrooveValue.CriterionSize = criterionSize;
                    oldGrooveValue.MinNormalRatio = minNormalRatio;
                    oldGrooveValue.MaxNormalRatio = maxNormalRatio;
                }

                if (m_previewValue == null)
                    m_previewValue = new GrooveProperty();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;
                m_previewValue.ApplyAverDiff = applyAverDiff;
                m_previewValue.AverMinMargin = averMinMargin;
                m_previewValue.AverMaxMargin = averMaxMargin;
                m_previewValue.MinWidthRatio = minWidthRatio;
                m_previewValue.MinWidthSize = minWidthSize;
                m_previewValue.MaxWidthRatio = maxWidthRatio;
                m_previewValue.MaxWidthSize = maxWidthSize;
                m_previewValue.MinHeightSize = minHeightSize;
                m_previewValue.MaxHeightRatio = maxHeightRatio;
                m_previewValue.MaxHeightSize = maxHeightSize;
                m_previewValue.CriterionSize = criterionSize;
                m_previewValue.MinNormalRatio = minNormalRatio;
                m_previewValue.MaxNormalRatio = maxNormalRatio;
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
                this.chkApplyAverDiff.IsChecked = (m_previewValue.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = m_previewValue.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = m_previewValue.AverMaxMargin.ToString();
                this.txtMinWidthRatio.Text = m_previewValue.MinWidthRatio.ToString();
                this.txtMinWidthSize.Text = m_previewValue.MinWidthSize.ToString();
                this.txtMaxWidthRatio.Text = m_previewValue.MaxWidthRatio.ToString();
                this.txtMaxWidthSize.Text = m_previewValue.MaxWidthSize.ToString();
                this.txtMinHeightSize.Text = m_previewValue.MinHeightSize.ToString();
                this.txtMaxHeightRatio.Text = m_previewValue.MaxHeightRatio.ToString();
                this.txtMaxHeightSize.Text = m_previewValue.MaxHeightSize.ToString();
                this.txtCriterionSize.Text = m_previewValue.CriterionSize.ToString();
                this.txtMinNormalRatio.Text = m_previewValue.MinNormalRatio.ToString();
                this.txtMaxNormalRatio.Text = m_previewValue.MaxNormalRatio.ToString();
            }
        }

        // 검사 설정 기본 값 표시.
        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeGroove)
            {
                if (!GrooveDefaultValue.DefaultValueLoaded)
                {
                    GrooveDefaultValue.DefaultValueLoaded = true;
                    GrooveDefaultValue.LoadDefaultValue();
                }
                this.txtLowerThreshold.Text = GrooveDefaultValue.LowerThresh.ToString();
                this.txtUpperThreshold.Text = GrooveDefaultValue.UpperThresh.ToString();
                
                this.chkApplyAverDiff.IsChecked = (GrooveDefaultValue.ApplyAverDiff == 1) ? true : false;
                if (GrooveDefaultValue.ApplyAverDiff != 1) // chkApplyAverDiff.IsChecked == false
                {
                    this.txtAverMinMargin.Text = "0";
                    this.txtAverMaxMargin.Text = "0";
                }
                else
                {
                    this.txtAverMinMargin.Text = GrooveDefaultValue.AverMinMargin.ToString();
                    this.txtAverMaxMargin.Text = GrooveDefaultValue.AverMaxMargin.ToString();
                }

                this.txtMinWidthRatio.Text = GrooveDefaultValue.MinWidthRatio.ToString();
                this.txtMinWidthSize.Text = GrooveDefaultValue.MinWidthSize.ToString();
                this.txtMaxWidthRatio.Text = GrooveDefaultValue.MaxWidthRatio.ToString();
                this.txtMaxWidthSize.Text = GrooveDefaultValue.MaxWidthSize.ToString();
                this.txtMinHeightSize.Text = GrooveDefaultValue.MinHeightSize.ToString();
                this.txtMaxHeightRatio.Text = GrooveDefaultValue.MaxHeightRatio.ToString();
                this.txtMaxHeightSize.Text = GrooveDefaultValue.MaxHeightSize.ToString();
                this.txtCriterionSize.Text = GrooveDefaultValue.CriterionSize.ToString();
                this.txtMinNormalRatio.Text = GrooveDefaultValue.MinNormalRatio.ToString();
                this.txtMaxNormalRatio.Text = GrooveDefaultValue.MaxNormalRatio.ToString();
            }
        }

        // 검사 설정 표시.
        public void Display(InspectionItem settingValue, int MarginX, int MatginY)
        {
            GrooveProperty grooveProperty = settingValue.InspectionAlgorithm as GrooveProperty;
            if (grooveProperty != null)
            {
                this.txtLowerThreshold.Text = grooveProperty.LowerThresh.ToString();
                this.txtUpperThreshold.Text = grooveProperty.UpperThresh.ToString();
                
                this.chkApplyAverDiff.IsChecked = (grooveProperty.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = grooveProperty.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = grooveProperty.AverMaxMargin.ToString();

                this.txtMinWidthRatio.Text = grooveProperty.MinWidthRatio.ToString();
                this.txtMinWidthSize.Text = grooveProperty.MinWidthSize.ToString();
                this.txtMaxWidthRatio.Text = grooveProperty.MaxWidthRatio.ToString();
                this.txtMaxWidthSize.Text = grooveProperty.MaxWidthSize.ToString();
                this.txtMinHeightSize.Text = grooveProperty.MinHeightSize.ToString();
                this.txtMaxHeightRatio.Text = grooveProperty.MaxHeightRatio.ToString();
                this.txtMaxHeightSize.Text = grooveProperty.MaxHeightSize.ToString();
                this.txtCriterionSize.Text = grooveProperty.CriterionSize.ToString();
                this.txtMinNormalRatio.Text = grooveProperty.MinNormalRatio.ToString();
                this.txtMaxNormalRatio.Text = grooveProperty.MaxNormalRatio.ToString();
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
