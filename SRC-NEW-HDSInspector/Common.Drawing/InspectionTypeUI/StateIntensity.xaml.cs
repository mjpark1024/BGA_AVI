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
// StateIntensity 알고리즘 : 코드 3011
//////////////////////////////////////////////////////
// Downset 검사 : 코드 0419
//////////////////////////////////////////////////////

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>   State intensity.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public partial class StateIntensity : UserControl, IInspectionTypeUICommands
    {
        private StateIntensityProperty m_previewValue;

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

        public StateIntensity()
        {
            InitializeComponent();
            InitializeEvent();

            SetRadioState2();
        }

        private void InitializeEvent()
        {
            this.radThresholdLower.Click += radThreshold_Click;
            this.radThresholdUpper.Click += radThreshold_Click;
            this.radThresholdRange.Click += radThresholdRange_Click;
        }

        #region Radio button events.
        private void radThreshold_Click(object sender, RoutedEventArgs e)
        {
            if (sender == radThresholdUpper)
            {
                chkMinRange.IsChecked = true;
                chkMaxRange.IsChecked = false;
            }
            else
            {
                chkMinRange.IsChecked = false;
                chkMaxRange.IsChecked = true;
            }
            chkInRange.IsChecked = false;

            SetRadioState1();
            chkSameValue_Click(null, null);
            this.txtThreshold.Focus();
        }

        private void radThresholdRange_Click(object sender, RoutedEventArgs e)
        {
            SetRadioState2();
            chkSameValue_Click(null, null);
            this.txtLowerThreshold.Focus();
        }
        #endregion

        private void chkSameValue_Click(object sender, RoutedEventArgs e)
        {
            if (radThresholdUpper.IsChecked == true)
            {
                this.chkMinRange.IsChecked = true;
                this.chkMaxRange.IsChecked = false;
            }
            else if (radThresholdLower.IsChecked == true)
            {
                this.chkMinRange.IsChecked = false;
                this.chkMaxRange.IsChecked = true;
            }
            else //radThresholdRange.IsChecked == true
            {
                this.chkMinRange.IsChecked = false;
                this.chkMaxRange.IsChecked = false;
                this.chkInRange.IsChecked = true;
            }
        }

        private void SetRadioState1()
        {
            this.txtLowerGV.Visibility = Visibility.Collapsed;
            this.txtLowerThreshold.Visibility = Visibility.Collapsed;
            this.txtUpperGV.Visibility = Visibility.Collapsed;
            this.txtUpperThreshold.Visibility = Visibility.Collapsed;

            this.txtThreshold.Visibility = Visibility.Visible;
            this.txtGV.Visibility = Visibility.Visible;
        }

        private void SetRadioState2()
        {
            this.txtLowerGV.Visibility = Visibility.Visible;
            this.txtLowerThreshold.Visibility = Visibility.Visible;
            this.txtUpperGV.Visibility = Visibility.Visible;
            this.txtUpperThreshold.Visibility = Visibility.Visible;

            this.txtThreshold.Visibility = Visibility.Collapsed;
            this.txtGV.Visibility = Visibility.Collapsed;
        }

        private void SetCheckBoxState(int switchValue)
        {
            switch (switchValue)
            {
                case 1:
                    this.chkMinRange.IsChecked = true; // 1
                    this.chkMaxRange.IsChecked = false;
                    this.chkInRange.IsChecked = false;
                    break;
                case 2:
                    this.chkMinRange.IsChecked = false;
                    this.chkMaxRange.IsChecked = true; // 2
                    this.chkInRange.IsChecked = false;
                    break;
                case 3:
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = true; // 1 + 2
                    this.chkInRange.IsChecked = false;
                    break;
                case 4:
                    this.chkMinRange.IsChecked = false;
                    this.chkMaxRange.IsChecked = false;
                    this.chkInRange.IsChecked = true; // 4
                    break;
                case 5:
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = false;
                    this.chkInRange.IsChecked = true; // 1 + 4
                    break;
                case 6:
                    this.chkMinRange.IsChecked = false;
                    this.chkMaxRange.IsChecked = true;
                    this.chkInRange.IsChecked = true; // 2 + 4
                    break;
                case 7:
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = true;
                    this.chkInRange.IsChecked = true; // 1 + 2 + 4
                    break;
            }
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
                #region Check 하한 / 상한 검출값
                int threshType = 0;
                int lowerThresh = -1;
                int upperThresh = -1;
                if (radThresholdUpper.IsChecked == true) // 임계 이상
                {
                    threshType = 0;
                    lowerThresh = Convert.ToInt32(txtThreshold.Text);
                    upperThresh = 255;
                }
                else if (radThresholdLower.IsChecked == true) // 임계 이하
                {
                    threshType = 1;
                    lowerThresh = 0;
                    upperThresh = Convert.ToInt32(txtThreshold.Text);
                }
                else if (radThresholdRange.IsChecked == true) // 임계 범위
                {
                    threshType = 2;

                    lowerThresh = Convert.ToInt32(txtLowerThreshold.Text);
                    upperThresh = Convert.ToInt32(txtUpperThreshold.Text);

                    if (lowerThresh > upperThresh)
                    {
                        MessageBox.Show("임계 하한 값은 임계 상한 값을 초과할 수 없습니다.", "Information");
                        this.txtLowerThreshold.Focus();
                        return;
                    }
                }
                #endregion

                int applyAverDiff = (chkApplyAverDiff.IsChecked == true) ? 1 : 0;
                int averMinMargin = (applyAverDiff == 1) ? Convert.ToInt32(txtAverMinMargin.Text) : 0;
                int averMaxMargin = (applyAverDiff == 1) ? Convert.ToInt32(txtAverMaxMargin.Text) : 0;

                int inspRange = 0;
                int minMargin = 0;
                int maxMargin = 0;
                if (this.chkMinRange.IsChecked == true)
                {
                    minMargin = Convert.ToInt32(txtMinMargin.Text);
                    inspRange += 1;
                }
                if (this.chkMaxRange.IsChecked == true)
                {
                    maxMargin = Convert.ToInt32(txtMaxMargin.Text);
                    inspRange += 2;
                }
                if (this.chkInRange.IsChecked == true)
                {
                    minMargin = Convert.ToInt32(txtMinMargin.Text);
                    maxMargin = Convert.ToInt32(txtMaxMargin.Text);
                    inspRange += 4;
                }
                
                int erosionTrainIter = Convert.ToInt32(txtErosionTrainIter.Text);
                int dilationTrainIter = Convert.ToInt32(txtDilationTrainIter.Text);
                int erosionInspIter = Convert.ToInt32(txtErosionInspIter.Text);
                int dilationInspIter = Convert.ToInt32(txtDilationInspIter.Text);
                int minDefectSize = Convert.ToInt32(txtMinDefectSize.Text);
                int invert = (chkInvert.IsChecked == true) ? 1 : 0;

                StateIntensityProperty oldStateIntensityValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldStateIntensityValue = element.InspectionAlgorithm as StateIntensityProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldStateIntensityValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    StateIntensityProperty stateIntensityValue = new StateIntensityProperty();

                    stateIntensityValue.ThreshType = threshType;
                    stateIntensityValue.LowerThresh = lowerThresh;
                    stateIntensityValue.UpperThresh = upperThresh;

                    stateIntensityValue.ApplyAverDiff = applyAverDiff;
                    stateIntensityValue.AverMinMargin = averMinMargin;
                    stateIntensityValue.AverMaxMargin = averMaxMargin;

                    stateIntensityValue.InspRange = inspRange;
                    stateIntensityValue.MinMargin = minMargin;
                    stateIntensityValue.MaxMargin = maxMargin;
                    
                    stateIntensityValue.ErosionTrainIter = erosionTrainIter;
                    stateIntensityValue.DilationTrainIter = dilationTrainIter;
                    stateIntensityValue.ErosionInspIter = erosionInspIter;
                    stateIntensityValue.DilationInspIter = dilationInspIter;
                    stateIntensityValue.MinDefectSize = minDefectSize;
                    stateIntensityValue.Invert = invert;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = stateIntensityValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldStateIntensityValue.ThreshType = threshType;
                    oldStateIntensityValue.LowerThresh = lowerThresh;
                    oldStateIntensityValue.UpperThresh = upperThresh;

                    oldStateIntensityValue.ApplyAverDiff = applyAverDiff;
                    oldStateIntensityValue.AverMinMargin = averMinMargin;
                    oldStateIntensityValue.AverMaxMargin = averMaxMargin;

                    oldStateIntensityValue.InspRange = inspRange;
                    oldStateIntensityValue.MinMargin = minMargin;
                    oldStateIntensityValue.MaxMargin = maxMargin;

                    oldStateIntensityValue.ErosionTrainIter = erosionTrainIter;
                    oldStateIntensityValue.DilationTrainIter = dilationTrainIter;
                    oldStateIntensityValue.ErosionInspIter = erosionInspIter;
                    oldStateIntensityValue.DilationInspIter = dilationInspIter;
                    oldStateIntensityValue.MinDefectSize = minDefectSize;
                    oldStateIntensityValue.Invert = invert;
                }

                if (m_previewValue == null)
                    m_previewValue = new StateIntensityProperty();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;

                m_previewValue.ApplyAverDiff = applyAverDiff;
                m_previewValue.AverMinMargin = averMinMargin;
                m_previewValue.AverMaxMargin = averMaxMargin;

                m_previewValue.InspRange = inspRange;
                m_previewValue.MinMargin = minMargin;
                m_previewValue.MaxMargin = maxMargin;
                
                m_previewValue.ErosionTrainIter = erosionTrainIter;
                m_previewValue.DilationTrainIter = dilationTrainIter;
                m_previewValue.ErosionInspIter = erosionInspIter;
                m_previewValue.DilationInspIter = dilationInspIter;
                m_previewValue.MinDefectSize = minDefectSize;
                m_previewValue.Invert = invert;
            }
            catch
            {
                MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
                if (txtThreshold.Visibility == Visibility.Visible)
                {
                    txtThreshold.Focus();
                }
                else
                {
                    txtLowerThreshold.Focus();
                }
            }
        }

        // 검사 설정 저장.
        public void TryAdd(ref InspectList list, int anInspectID)
        {
            try
            {
                #region Check 하한 / 상한 검출값
                int threshType = 0;
                int lowerThresh = -1;
                int upperThresh = -1;
                if (radThresholdUpper.IsChecked == true) // 임계 이상
                {
                    threshType = 0;
                    lowerThresh = Convert.ToInt32(txtThreshold.Text);
                    upperThresh = 255;
                }
                else if (radThresholdLower.IsChecked == true) // 임계 이하
                {
                    threshType = 1;
                    lowerThresh = 0;
                    upperThresh = Convert.ToInt32(txtThreshold.Text);
                }
                else if (radThresholdRange.IsChecked == true) // 임계 범위
                {
                    threshType = 2;

                    lowerThresh = Convert.ToInt32(txtLowerThreshold.Text);
                    upperThresh = Convert.ToInt32(txtUpperThreshold.Text);

                    //if (lowerThresh > upperThresh)
                    //{
                    //    MessageBox.Show("임계 하한 값은 임계 상한 값을 초과할 수 없습니다.", "Information");
                    //    this.txtLowerThreshold.Focus();
                    //    return;
                    //}
                }
                #endregion

                int applyAverDiff = (chkApplyAverDiff.IsChecked == true) ? 1 : 0;
                int averMinMargin = (applyAverDiff == 1) ? Convert.ToInt32(txtAverMinMargin.Text) : 0;
                int averMaxMargin = (applyAverDiff == 1) ? Convert.ToInt32(txtAverMaxMargin.Text) : 0;

                int inspRange = 0;
                int minMargin = 0;
                int maxMargin = 0;
                if (this.chkMinRange.IsChecked == true)
                {
                    minMargin = Convert.ToInt32(txtMinMargin.Text);
                    inspRange += 1;
                }
                if (this.chkMaxRange.IsChecked == true)
                {
                    maxMargin = Convert.ToInt32(txtMaxMargin.Text);
                    inspRange += 2;
                }
                if (this.chkInRange.IsChecked == true)
                {
                    minMargin = Convert.ToInt32(txtMinMargin.Text);
                    maxMargin = Convert.ToInt32(txtMaxMargin.Text);
                    inspRange += 4;
                }

                int erosionTrainIter = Convert.ToInt32(txtErosionTrainIter.Text);
                int dilationTrainIter = Convert.ToInt32(txtDilationTrainIter.Text);
                int erosionInspIter = Convert.ToInt32(txtErosionInspIter.Text);
                int dilationInspIter = Convert.ToInt32(txtDilationInspIter.Text);
                int minDefectSize = Convert.ToInt32(txtMinDefectSize.Text);
                int invert = (chkInvert.IsChecked == true) ? 1 : 0;

                StateIntensityProperty oldStateIntensityValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldStateIntensityValue = element.InspectionAlgorithm as StateIntensityProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldStateIntensityValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    StateIntensityProperty stateIntensityValue = new StateIntensityProperty();

                    stateIntensityValue.ThreshType = threshType;
                    stateIntensityValue.LowerThresh = lowerThresh;
                    stateIntensityValue.UpperThresh = upperThresh;

                    stateIntensityValue.ApplyAverDiff = applyAverDiff;
                    stateIntensityValue.AverMinMargin = averMinMargin;
                    stateIntensityValue.AverMaxMargin = averMaxMargin;

                    stateIntensityValue.InspRange = inspRange;
                    stateIntensityValue.MinMargin = minMargin;
                    stateIntensityValue.MaxMargin = maxMargin;

                    stateIntensityValue.ErosionTrainIter = erosionTrainIter;
                    stateIntensityValue.DilationTrainIter = dilationTrainIter;
                    stateIntensityValue.ErosionInspIter = erosionInspIter;
                    stateIntensityValue.DilationInspIter = dilationInspIter;
                    stateIntensityValue.MinDefectSize = minDefectSize;
                    stateIntensityValue.Invert = invert;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = stateIntensityValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldStateIntensityValue.ThreshType = threshType;
                    oldStateIntensityValue.LowerThresh = lowerThresh;
                    oldStateIntensityValue.UpperThresh = upperThresh;

                    oldStateIntensityValue.ApplyAverDiff = applyAverDiff;
                    oldStateIntensityValue.AverMinMargin = averMinMargin;
                    oldStateIntensityValue.AverMaxMargin = averMaxMargin;

                    oldStateIntensityValue.InspRange = inspRange;
                    oldStateIntensityValue.MinMargin = minMargin;
                    oldStateIntensityValue.MaxMargin = maxMargin;

                    oldStateIntensityValue.ErosionTrainIter = erosionTrainIter;
                    oldStateIntensityValue.DilationTrainIter = dilationTrainIter;
                    oldStateIntensityValue.ErosionInspIter = erosionInspIter;
                    oldStateIntensityValue.DilationInspIter = dilationInspIter;
                    oldStateIntensityValue.MinDefectSize = minDefectSize;
                    oldStateIntensityValue.Invert = invert;
                }

                if (m_previewValue == null)
                    m_previewValue = new StateIntensityProperty();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;

                m_previewValue.ApplyAverDiff = applyAverDiff;
                m_previewValue.AverMinMargin = averMinMargin;
                m_previewValue.AverMaxMargin = averMaxMargin;

                m_previewValue.InspRange = inspRange;
                m_previewValue.MinMargin = minMargin;
                m_previewValue.MaxMargin = maxMargin;

                m_previewValue.ErosionTrainIter = erosionTrainIter;
                m_previewValue.DilationTrainIter = dilationTrainIter;
                m_previewValue.ErosionInspIter = erosionInspIter;
                m_previewValue.DilationInspIter = dilationInspIter;
                m_previewValue.MinDefectSize = minDefectSize;
                m_previewValue.Invert = invert;
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

        // 검사 설정 직전 값 표시.
        public void SetPreviewValue()
        {
            if (m_previewValue != null)
            {
                #region Threshold.
                if (m_previewValue.ThreshType == 0)
                {
                    this.radThresholdUpper.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = false;
                    this.txtThreshold.Text = m_previewValue.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = m_previewValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = m_previewValue.UpperThresh.ToString();
                    SetRadioState1();
                }
                else if (m_previewValue.ThreshType == 1)
                {
                    this.radThresholdLower.IsChecked = true;
                    this.chkMinRange.IsChecked = false;
                    this.chkMaxRange.IsChecked = true;
                    this.txtThreshold.Text = m_previewValue.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = m_previewValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = m_previewValue.UpperThresh.ToString();
                    SetRadioState1();
                }
                else
                {
                    this.radThresholdRange.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = true;
                    this.txtLowerThreshold.Text = m_previewValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = m_previewValue.UpperThresh.ToString();
                    this.txtThreshold.Text = m_previewValue.LowerThresh.ToString();
                    SetRadioState2();
                }
                #endregion Threshold.
                this.chkApplyAverDiff.IsChecked = (m_previewValue.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = m_previewValue.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = m_previewValue.AverMaxMargin.ToString();

                this.SetCheckBoxState(m_previewValue.InspRange);
                this.txtMinMargin.Text = m_previewValue.MinMargin.ToString();
                this.txtMaxMargin.Text = m_previewValue.MaxMargin.ToString();

                this.txtErosionTrainIter.Text = m_previewValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = m_previewValue.DilationTrainIter.ToString();
                this.txtErosionInspIter.Text = m_previewValue.ErosionInspIter.ToString();
                this.txtDilationInspIter.Text = m_previewValue.DilationInspIter.ToString();
                this.txtMinDefectSize.Text = m_previewValue.MinDefectSize.ToString();
                this.chkInvert.IsChecked = (m_previewValue.Invert == 1) ? true : false;
                
            }
        }

        // 검사 설정 기본 값 표시.
        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeRailIntensity)
            {
                if (!RailDefaultValue.DefaultValueLoaded)
                {
                    RailDefaultValue.DefaultValueLoaded = true;
                    RailDefaultValue.LoadDefaultValue();
                }

                if (RailDefaultValue.ThreshType == 0)
                {
                    this.radThresholdUpper.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = false;
                    this.txtThreshold.Text = RailDefaultValue.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = RailDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = RailDefaultValue.UpperThresh.ToString();
                    SetRadioState1();
                }
                else if (RailDefaultValue.ThreshType == 1)
                {
                    this.radThresholdLower.IsChecked = true;
                    this.chkMinRange.IsChecked = false;
                    this.chkMaxRange.IsChecked = true;
                    this.txtThreshold.Text = RailDefaultValue.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = RailDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = RailDefaultValue.UpperThresh.ToString();
                    SetRadioState1();
                }
                else
                {
                    this.radThresholdRange.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = true;
                    this.txtLowerThreshold.Text = RailDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = RailDefaultValue.UpperThresh.ToString();
                    this.txtThreshold.Text = RailDefaultValue.LowerThresh.ToString();
                    SetRadioState2();
                }
                this.chkApplyAverDiff.IsChecked = (RailDefaultValue.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = (RailDefaultValue.ApplyAverDiff == 1) ? RailDefaultValue.AverMinMargin.ToString() : "0";
                this.txtAverMaxMargin.Text = (RailDefaultValue.ApplyAverDiff == 1) ? RailDefaultValue.AverMaxMargin.ToString() : "0";

                this.SetCheckBoxState(RailDefaultValue.InspRange);
                this.txtMinMargin.Text = RailDefaultValue.MinMargin.ToString();
                this.txtMaxMargin.Text = RailDefaultValue.MaxMargin.ToString();

                this.txtErosionTrainIter.Text = RailDefaultValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = RailDefaultValue.DilationTrainIter.ToString();
                this.txtErosionInspIter.Text = RailDefaultValue.ErosionInspIter.ToString();
                this.txtDilationInspIter.Text = RailDefaultValue.DilationInspIter.ToString();
                this.txtMinDefectSize.Text = RailDefaultValue.MinDefectSize.ToString();
                this.chkInvert.IsChecked = (RailDefaultValue.Invert == 1) ? true : false;
            }
        }

        // 검사 설정 표시.
        public void Display(InspectionInformation.InspectionItem settingValue, int MarginX, int MatginY)
        {
            StateIntensityProperty stateIntensityProperty = settingValue.InspectionAlgorithm as StateIntensityProperty;
            if (stateIntensityProperty != null)
            {
                #region 임계 설정.
                if (stateIntensityProperty.ThreshType == 0) // 임계 이상
                {
                    this.radThresholdUpper.IsChecked = true;
                    this.txtThreshold.Text = stateIntensityProperty.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = stateIntensityProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = stateIntensityProperty.UpperThresh.ToString();
                    SetRadioState1();
                }
                else if (stateIntensityProperty.ThreshType == 1) // 임계 이하
                {
                    this.radThresholdLower.IsChecked = true;
                    this.txtThreshold.Text = stateIntensityProperty.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = stateIntensityProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = stateIntensityProperty.UpperThresh.ToString();
                    SetRadioState1();
                }
                else // 임계 범위
                {
                    this.radThresholdRange.IsChecked = true;
                    this.txtLowerThreshold.Text = stateIntensityProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = stateIntensityProperty.UpperThresh.ToString();
                    this.txtThreshold.Text = stateIntensityProperty.LowerThresh.ToString();
                    SetRadioState2();
                }
                #endregion
                this.chkApplyAverDiff.IsChecked = (stateIntensityProperty.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = stateIntensityProperty.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = stateIntensityProperty.AverMaxMargin.ToString();

                this.SetCheckBoxState(stateIntensityProperty.InspRange);
                this.txtMinMargin.Text = stateIntensityProperty.MinMargin.ToString();
                this.txtMaxMargin.Text = stateIntensityProperty.MaxMargin.ToString();

                this.txtErosionTrainIter.Text = stateIntensityProperty.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = stateIntensityProperty.DilationTrainIter.ToString();
                this.txtErosionInspIter.Text = stateIntensityProperty.ErosionInspIter.ToString();
                this.txtDilationInspIter.Text = stateIntensityProperty.DilationInspIter.ToString();
                this.txtMinDefectSize.Text = stateIntensityProperty.MinDefectSize.ToString();
                this.chkInvert.IsChecked = (stateIntensityProperty.Invert == 1) ? true : false;
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
