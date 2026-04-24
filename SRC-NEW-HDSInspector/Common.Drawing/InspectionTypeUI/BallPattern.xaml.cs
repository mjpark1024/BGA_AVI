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
// 2015.12.21 - 코드 정리 완료. (검사 방법에 따라 초기 설정 값을 달리 가져갈 필요가 있다.)
//////////////////////////////////////////////////////
// Ball 알고리즘 : 코드 3013
//////////////////////////////////////////////////////

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>
    /// BallPattern.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BallPattern : UserControl, IInspectionTypeUICommands
    {
        private BallPatternProperty m_previewValue;

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

        public BallPattern()
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
            this.txtThreshold.TextChanged += txtThreshold_TextChanged;
            this.txtLowerThreshold.TextChanged += txtLowerThreshold_TextChanged;
            this.txtUpperThreshold.TextChanged += txtUpperThreshold_TextChanged;
            this.txtMaskLowerThresh.TextChanged += txtMaskLowerThresh_TextChanged;
            this.txtMaskUpperThresh.TextChanged += txtMaskUpperThresh_TextChanged;

        }

        #region Textbox text changed events.
        private void txtThreshold_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (chkSameValue.IsChecked == true)
            //{
            //    if (radThresholdLower.IsChecked == true)
            //    {
            //        txtMaskLowerThresh.Text = "0";
            //        txtMaskUpperThresh.Text = txtThreshold.Text;

            //        //txtUpperThreshold.Text = txtThreshold.Text;
            //    }
            //    else if (radThresholdUpper.IsChecked == true)
            //    {
            //        txtMaskLowerThresh.Text = txtThreshold.Text;
            //        txtMaskUpperThresh.Text = "255";

            //        //txtLowerThreshold.Text = txtThreshold.Text;
            //    }
            //}
        }

        private void txtUpperThreshold_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (chkSameValue.IsChecked == true)
            //{
            //    txtMaskUpperThresh.Text = txtUpperThreshold.Text;
            //}
        }

        private void txtLowerThreshold_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (chkSameValue.IsChecked == true)
            //{
            //    txtMaskLowerThresh.Text = txtLowerThreshold.Text;
            //}
        }

        private void txtMaskUpperThresh_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (chkSameValue.IsChecked == true)
            //{
            //    txtUpperThreshold.Text = txtMaskUpperThresh.Text;
            //}
        }

        private void txtMaskLowerThresh_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (chkSameValue.IsChecked == true)
            //{
            //    txtLowerThreshold.Text = txtMaskLowerThresh.Text;
            //}
        }
        #endregion

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
            SetSameValue();
            this.txtThreshold.Focus();
        }

        private void radThresholdRange_Click(object sender, RoutedEventArgs e)
        {
            SetRadioState2();
            SetSameValue();
            this.txtLowerThreshold.Focus();
        }
        #endregion

        private void SetSameValue()
        {
            if (radThresholdUpper.IsChecked == true)
            {
                this.chkMinRange.IsChecked = true;
                this.chkMaxRange.IsChecked = false;
                //if (chkSameValue.IsChecked == true)
                //{
                //     this.txtMaskLowerThresh.Text = this.txtThreshold.Text;
                this.txtMaskUpperThresh.Text = "255";
                // }
            }
            else if (radThresholdLower.IsChecked == true)
            {
                this.chkMinRange.IsChecked = false;
                this.chkMaxRange.IsChecked = true;
                // if (chkSameValue.IsChecked == true)
                //  {
                this.txtMaskLowerThresh.Text = "0";
                //     this.txtMaskUpperThresh.Text = this.txtThreshold.Text;
                // }
            }
            else //radThresholdRange.IsChecked == true
            {
                this.chkMinRange.IsChecked = true;
                this.chkMaxRange.IsChecked = true;
                // if (chkSameValue.IsChecked == true)
                // {
                //      this.txtMaskLowerThresh.Text = this.txtLowerThreshold.Text;
                //     this.txtMaskUpperThresh.Text = this.txtUpperThreshold.Text;
                // }
            }
        }


        private void SetRadioState1()
        {
            if ((bool)radThresholdUpper.IsChecked)
            {
                //this.txtMinDefectSize.IsEnabled = true;
                //this.txtMinSmallDefectCount.IsEnabled = true;
                //this.txtMinSmallDefectSize.IsEnabled = true;

                //this.txtMaxDefectSize.IsEnabled = false;
                //this.txtMaxSmallDefectCount.IsEnabled = false;
                //this.txtMaxSmallDefectSize.IsEnabled = false;

                this.txtMinMargin.IsEnabled = true;
                this.txtMaxMargin.IsEnabled = false;
            }
            else
            {
                //this.txtMinDefectSize.IsEnabled = false;
                //this.txtMinSmallDefectCount.IsEnabled = false;
                //this.txtMinSmallDefectSize.IsEnabled = false;

                //this.txtMaxDefectSize.IsEnabled = true;
                //this.txtMaxSmallDefectCount.IsEnabled = true;
                //this.txtMaxSmallDefectSize.IsEnabled = true;

                this.txtMinMargin.IsEnabled = false;
                this.txtMaxMargin.IsEnabled = true;
            }
            this.txtLowerGV.Visibility = Visibility.Collapsed;
            this.txtLowerThreshold.Visibility = Visibility.Collapsed;
            this.txtUpperGV.Visibility = Visibility.Collapsed;
            this.txtUpperThreshold.Visibility = Visibility.Collapsed;
            this.txtThreshold.Visibility = Visibility.Visible;
            this.txtGV.Visibility = Visibility.Visible;
        }

        private void SetRadioState2()
        {
            //this.txtMinDefectSize.IsEnabled = true;
            //this.txtMinSmallDefectCount.IsEnabled = true;
            //this.txtMinSmallDefectSize.IsEnabled = true;
            //this.txtMaxDefectSize.IsEnabled = true;
            //this.txtMaxSmallDefectCount.IsEnabled = true;
            //this.txtMaxSmallDefectSize.IsEnabled = true;
            this.txtMinMargin.IsEnabled = true;
            this.txtMaxMargin.IsEnabled = true;

            this.txtLowerGV.Visibility = Visibility.Visible;
            this.txtLowerThreshold.Visibility = Visibility.Visible;
            this.txtUpperGV.Visibility = Visibility.Visible;
            this.txtUpperThreshold.Visibility = Visibility.Visible;
            this.txtThreshold.Visibility = Visibility.Collapsed;
            this.txtGV.Visibility = Visibility.Collapsed;
        }

        public void SetDialog(string strCaption, InspectionInformation.eVisInspectType inspectType)
        {

            this.m_enumInspectType = inspectType;
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

                #region Check 마스크 임계값
                int maskLowerThresh = Convert.ToInt32(txtMaskLowerThresh.Text);
                int maskUpperThresh = Convert.ToInt32(txtMaskUpperThresh.Text);

                if (maskLowerThresh > maskUpperThresh)
                {
                    MessageBox.Show("마스크 임계하한 값은 마스크 임계상한 값을 초과할 수 없습니다.", "Information");
                    this.txtMaskLowerThresh.Focus();
                    return;
                }
                #endregion

                int applyAverDiff = (chkApplyAverDiff.IsChecked == true) ? 1 : 0;
                int averMinMargin = (chkApplyAverDiff.IsChecked == true) ? Convert.ToInt32(txtAverMinMargin.Text) : 0;
                int averMaxMargin = (chkApplyAverDiff.IsChecked == true) ? Convert.ToInt32(txtAverMaxMargin.Text) : 0;
                int samevalue = 0;
                int inspRange = 0;
                int minMargin = 0;
                int maxMargin = 0;
                if (chkMinRange.IsChecked == true)
                {
                    minMargin = Convert.ToInt32(txtMinMargin.Text);
                    inspRange += 1;
                }
                if (chkMaxRange.IsChecked == true)
                {
                    maxMargin = Convert.ToInt32(txtMaxMargin.Text);
                    inspRange += 2;
                }
                if (chkInRange.IsChecked == true)
                {
                    minMargin = Convert.ToInt32(txtMinMargin.Text);
                    maxMargin = Convert.ToInt32(txtMaxMargin.Text);
                    inspRange += 4;
                }

                int erosionTrainIter = Convert.ToInt32(txtErosionTrainIter.Text);
                int dilationTrainIter = Convert.ToInt32(txtDilationTrainIter.Text);
                //int erosionInspIter = Convert.ToInt32(txtErosionInspIter.Text);
                //int dilationInspIter = Convert.ToInt32(txtDilationInspIter.Text);
                int erosionInspIter = 0;
                int dilationInspIter = 0;

                //int minDefectSize = (chkMinRange.IsChecked == true) ? Convert.ToInt32(txtMinDefectSize.Text) : 0;
                //int minSmallDefectSize = (chkMinRange.IsChecked == true) ? Convert.ToInt32(txtMinSmallDefectSize.Text) : 0;
                //int minSmallDefectCount = (chkMinRange.IsChecked == true) ? Convert.ToInt32(txtMinSmallDefectCount.Text) : 0;

                int minDefectSize = 0;
                int minSmallDefectSize = 0;
                int minSmallDefectCount = 0;

                int MinWidthSize = Convert.ToInt32(txtMinWidthSize.Text);
                int MinHeightSize = Convert.ToInt32(txtMinHeightSize.Text);
                int MaxHeightSize = Convert.ToInt32(txtMaxHeightSize.Text);

                //int maxDefectSize = (chkMaxRange.IsChecked == true) ? Convert.ToInt32(txtMaxDefectSize.Text) : 0;
                //int maxSmallDefectSize = (chkMaxRange.IsChecked == true) ? Convert.ToInt32(txtMaxSmallDefectSize.Text) : 0;
                //int maxSmallDefectCount = (chkMaxRange.IsChecked == true) ? Convert.ToInt32(txtMaxSmallDefectCount.Text) : 0;

                int maxDefectSize = 0;
                int maxSmallDefectSize = 0;
                int maxSmallDefectCount = 0;

                int UsePSR = (chkPSRShift.IsChecked == true) ? 1 : 0;
                //int UseState = (chkState.IsChecked == true) ? 1 : 0;
                int UseState = 0;

                BallPatternProperty oldStateAlignedMaskValue = null;

                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        graphic.BallSegments = element.BallSegments = null;
                        graphic.RefreshDrawing();
                        oldStateAlignedMaskValue = element.InspectionAlgorithm as BallPatternProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldStateAlignedMaskValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    BallPatternProperty stateAlignedMaskValue = new BallPatternProperty();

                    stateAlignedMaskValue.ThreshType = threshType;
                    stateAlignedMaskValue.LowerThresh = lowerThresh;
                    stateAlignedMaskValue.UpperThresh = upperThresh;
                    stateAlignedMaskValue.SameValue = samevalue;

                    stateAlignedMaskValue.MaskLowerThresh = maskLowerThresh;
                    stateAlignedMaskValue.MaskUpperThresh = maskUpperThresh;

                    stateAlignedMaskValue.ApplyAverDiff = applyAverDiff;
                    stateAlignedMaskValue.AverMinMargin = averMinMargin;
                    stateAlignedMaskValue.AverMaxMargin = averMaxMargin;

                    stateAlignedMaskValue.InspRange = inspRange;
                    stateAlignedMaskValue.MinMargin = minMargin;
                    stateAlignedMaskValue.MaxMargin = maxMargin;

                    stateAlignedMaskValue.ErosionTrainIter = erosionTrainIter;
                    stateAlignedMaskValue.DilationTrainIter = dilationTrainIter;
                    stateAlignedMaskValue.ErosionInspIter = erosionInspIter;
                    stateAlignedMaskValue.DilationInspIter = dilationInspIter;

                    stateAlignedMaskValue.MinDefectSize = minDefectSize;
                    stateAlignedMaskValue.MinSmallDefectSize = minSmallDefectSize;
                    stateAlignedMaskValue.MinSmallDefectCount = minSmallDefectCount;

                    stateAlignedMaskValue.MinWidthSize = MinWidthSize;
                    stateAlignedMaskValue.MinHeightSize = MinHeightSize;
                    stateAlignedMaskValue.MaxHeightSize = MaxHeightSize;

                    stateAlignedMaskValue.MaxDefectSize = maxDefectSize;
                    stateAlignedMaskValue.MaxSmallDefectSize = maxSmallDefectSize;
                    stateAlignedMaskValue.MaxSmallDefectCount = maxSmallDefectCount;
                    stateAlignedMaskValue.UsePSRShift = UsePSR;
                    stateAlignedMaskValue.UsePunchShift = UseState;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = stateAlignedMaskValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldStateAlignedMaskValue.ThreshType = threshType;
                    oldStateAlignedMaskValue.LowerThresh = lowerThresh;
                    oldStateAlignedMaskValue.UpperThresh = upperThresh;
                    oldStateAlignedMaskValue.SameValue = samevalue;
                    oldStateAlignedMaskValue.MaskLowerThresh = maskLowerThresh;
                    oldStateAlignedMaskValue.MaskUpperThresh = maskUpperThresh;

                    oldStateAlignedMaskValue.ApplyAverDiff = applyAverDiff;
                    oldStateAlignedMaskValue.AverMinMargin = averMinMargin;
                    oldStateAlignedMaskValue.AverMaxMargin = averMaxMargin;

                    oldStateAlignedMaskValue.InspRange = inspRange;
                    oldStateAlignedMaskValue.MinMargin = minMargin;
                    oldStateAlignedMaskValue.MaxMargin = maxMargin;

                    oldStateAlignedMaskValue.ErosionTrainIter = erosionTrainIter;
                    oldStateAlignedMaskValue.DilationTrainIter = dilationTrainIter;
                    oldStateAlignedMaskValue.ErosionInspIter = erosionInspIter;
                    oldStateAlignedMaskValue.DilationInspIter = dilationInspIter;

                    oldStateAlignedMaskValue.MinDefectSize = minDefectSize;
                    oldStateAlignedMaskValue.MinSmallDefectSize = minSmallDefectSize;
                    oldStateAlignedMaskValue.MinSmallDefectCount = minSmallDefectCount;

                    oldStateAlignedMaskValue.MinWidthSize = MinWidthSize;
                    oldStateAlignedMaskValue.MinHeightSize = MinHeightSize;
                    oldStateAlignedMaskValue.MaxHeightSize = MaxHeightSize;

                    oldStateAlignedMaskValue.MaxDefectSize = maxDefectSize;
                    oldStateAlignedMaskValue.MaxSmallDefectSize = maxSmallDefectSize;
                    oldStateAlignedMaskValue.MaxSmallDefectCount = maxSmallDefectCount;
                    oldStateAlignedMaskValue.UsePSRShift = UsePSR;
                    oldStateAlignedMaskValue.UsePunchShift = UseState;
                }

                if (m_previewValue == null)
                    m_previewValue = new BallPatternProperty();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;
                m_previewValue.MaskLowerThresh = maskLowerThresh;
                m_previewValue.MaskUpperThresh = maskUpperThresh;
                m_previewValue.SameValue = samevalue;
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
                m_previewValue.MinSmallDefectSize = minSmallDefectSize;
                m_previewValue.MinSmallDefectCount = minSmallDefectCount;

                m_previewValue.MinWidthSize = MinWidthSize;
                m_previewValue.MinHeightSize = MinHeightSize;
                m_previewValue.MaxHeightSize = MaxHeightSize;

                m_previewValue.MaxDefectSize = maxDefectSize;
                m_previewValue.MaxSmallDefectSize = maxSmallDefectSize;
                m_previewValue.MaxSmallDefectCount = maxSmallDefectCount;
                m_previewValue.UsePSRShift = UsePSR;
                m_previewValue.UsePunchShift = UseState;
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

                #region Check 마스크 임계값
                int maskLowerThresh = Convert.ToInt32(txtMaskLowerThresh.Text);
                int maskUpperThresh = Convert.ToInt32(txtMaskUpperThresh.Text);

                //if (maskLowerThresh > maskUpperThresh)
                //{
                //    MessageBox.Show("마스크 임계하한 값은 마스크 임계상한 값을 초과할 수 없습니다.", "Information");
                //    this.txtMaskLowerThresh.Focus();
                //    return;
                //}
                #endregion

                int applyAverDiff = (chkApplyAverDiff.IsChecked == true) ? 1 : 0;
                int averMinMargin = (chkApplyAverDiff.IsChecked == true) ? Convert.ToInt32(txtAverMinMargin.Text) : 0;
                int averMaxMargin = (chkApplyAverDiff.IsChecked == true) ? Convert.ToInt32(txtAverMaxMargin.Text) : 0;
                int samevalue = 0;
                int inspRange = 0;
                int minMargin = 0;
                int maxMargin = 0;
                if (chkMinRange.IsChecked == true)
                {
                    minMargin = Convert.ToInt32(txtMinMargin.Text);
                    inspRange += 1;
                }
                if (chkMaxRange.IsChecked == true)
                {
                    maxMargin = Convert.ToInt32(txtMaxMargin.Text);
                    inspRange += 2;
                }
                if (chkInRange.IsChecked == true)
                {
                    minMargin = Convert.ToInt32(txtMinMargin.Text);
                    maxMargin = Convert.ToInt32(txtMaxMargin.Text);
                    inspRange += 4;
                }

                int erosionTrainIter = Convert.ToInt32(txtErosionTrainIter.Text);
                int dilationTrainIter = Convert.ToInt32(txtDilationTrainIter.Text);
                //int erosionInspIter = Convert.ToInt32(txtErosionInspIter.Text);
                //int dilationInspIter = Convert.ToInt32(txtDilationInspIter.Text);
                int erosionInspIter = 0;
                int dilationInspIter = 0;

                int MinWidthSize = Convert.ToInt32(txtMinWidthSize.Text);
                int MinHeightSize = Convert.ToInt32(txtMinHeightSize.Text);
                int MaxHeightSize = Convert.ToInt32(txtMaxHeightSize.Text);

                //int minDefectSize = (chkMinRange.IsChecked == true) ? Convert.ToInt32(txtMinDefectSize.Text) : 0;
                //int minSmallDefectSize = (chkMinRange.IsChecked == true) ? Convert.ToInt32(txtMinSmallDefectSize.Text) : 0;
                //int minSmallDefectCount = (chkMinRange.IsChecked == true) ? Convert.ToInt32(txtMinSmallDefectCount.Text) : 0;
                //int maxDefectSize = (chkMaxRange.IsChecked == true) ? Convert.ToInt32(txtMaxDefectSize.Text) : 0;
                //int maxSmallDefectSize = (chkMaxRange.IsChecked == true) ? Convert.ToInt32(txtMaxSmallDefectSize.Text) : 0;
                //int maxSmallDefectCount = (chkMaxRange.IsChecked == true) ? Convert.ToInt32(txtMaxSmallDefectCount.Text) : 0;

                int minDefectSize =  0;
                int minSmallDefectSize =  0;
                int minSmallDefectCount = 0;
                int maxDefectSize =  0;
                int maxSmallDefectSize =  0;
                int maxSmallDefectCount =  0;

                int UsePSR = (chkPSRShift.IsChecked == true) ? 1 : 0;

                //int UseState = (chkState.IsChecked == true) ? 1 : 0;
                int UseState = 0;

                BallPatternProperty oldStateAlignedMaskValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldStateAlignedMaskValue = element.InspectionAlgorithm as BallPatternProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldStateAlignedMaskValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    BallPatternProperty stateAlignedMaskValue = new BallPatternProperty();

                    stateAlignedMaskValue.ThreshType = threshType;
                    stateAlignedMaskValue.LowerThresh = lowerThresh;
                    stateAlignedMaskValue.UpperThresh = upperThresh;
                    stateAlignedMaskValue.SameValue = samevalue;
                    stateAlignedMaskValue.MaskLowerThresh = maskLowerThresh;
                    stateAlignedMaskValue.MaskUpperThresh = maskUpperThresh;

                    stateAlignedMaskValue.ApplyAverDiff = applyAverDiff;
                    stateAlignedMaskValue.AverMinMargin = averMinMargin;
                    stateAlignedMaskValue.AverMaxMargin = averMaxMargin;

                    stateAlignedMaskValue.InspRange = inspRange;
                    stateAlignedMaskValue.MinMargin = minMargin;
                    stateAlignedMaskValue.MaxMargin = maxMargin;

                    stateAlignedMaskValue.ErosionTrainIter = erosionTrainIter;
                    stateAlignedMaskValue.DilationTrainIter = dilationTrainIter;
                    stateAlignedMaskValue.ErosionInspIter = erosionInspIter;
                    stateAlignedMaskValue.DilationInspIter = dilationInspIter;

                    stateAlignedMaskValue.MinWidthSize = MinWidthSize;
                    stateAlignedMaskValue.MinHeightSize = MinHeightSize;
                    stateAlignedMaskValue.MaxHeightSize = MaxHeightSize;

                    stateAlignedMaskValue.MinDefectSize = minDefectSize;
                    stateAlignedMaskValue.MinSmallDefectSize = minSmallDefectSize;
                    stateAlignedMaskValue.MinSmallDefectCount = minSmallDefectCount;
                    stateAlignedMaskValue.MaxDefectSize = maxDefectSize;
                    stateAlignedMaskValue.MaxSmallDefectSize = maxSmallDefectSize;
                    stateAlignedMaskValue.MaxSmallDefectCount = maxSmallDefectCount;
                    stateAlignedMaskValue.UsePSRShift = UsePSR;
                    stateAlignedMaskValue.UsePunchShift = UseState;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = stateAlignedMaskValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldStateAlignedMaskValue.ThreshType = threshType;
                    oldStateAlignedMaskValue.LowerThresh = lowerThresh;
                    oldStateAlignedMaskValue.UpperThresh = upperThresh;
                    oldStateAlignedMaskValue.SameValue = samevalue;
                    oldStateAlignedMaskValue.MaskLowerThresh = maskLowerThresh;
                    oldStateAlignedMaskValue.MaskUpperThresh = maskUpperThresh;

                    oldStateAlignedMaskValue.ApplyAverDiff = applyAverDiff;
                    oldStateAlignedMaskValue.AverMinMargin = averMinMargin;
                    oldStateAlignedMaskValue.AverMaxMargin = averMaxMargin;

                    oldStateAlignedMaskValue.InspRange = inspRange;
                    oldStateAlignedMaskValue.MinMargin = minMargin;
                    oldStateAlignedMaskValue.MaxMargin = maxMargin;

                    oldStateAlignedMaskValue.ErosionTrainIter = erosionTrainIter;
                    oldStateAlignedMaskValue.DilationTrainIter = dilationTrainIter;
                    oldStateAlignedMaskValue.ErosionInspIter = erosionInspIter;
                    oldStateAlignedMaskValue.DilationInspIter = dilationInspIter;

                    oldStateAlignedMaskValue.MinWidthSize = MinWidthSize;
                    oldStateAlignedMaskValue.MinHeightSize = MinHeightSize;
                    oldStateAlignedMaskValue.MaxHeightSize = MaxHeightSize;

                    oldStateAlignedMaskValue.MinDefectSize = minDefectSize;
                    oldStateAlignedMaskValue.MinSmallDefectSize = minSmallDefectSize;
                    oldStateAlignedMaskValue.MinSmallDefectCount = minSmallDefectCount;
                    oldStateAlignedMaskValue.MaxDefectSize = maxDefectSize;
                    oldStateAlignedMaskValue.MaxSmallDefectSize = maxSmallDefectSize;
                    oldStateAlignedMaskValue.MaxSmallDefectCount = maxSmallDefectCount;
                    oldStateAlignedMaskValue.UsePSRShift = UsePSR;
                    oldStateAlignedMaskValue.UsePunchShift = UseState;
                }

                if (m_previewValue == null)
                    m_previewValue = new BallPatternProperty();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;
                m_previewValue.MaskLowerThresh = maskLowerThresh;
                m_previewValue.MaskUpperThresh = maskUpperThresh;
                m_previewValue.SameValue = samevalue;
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

                m_previewValue.MinWidthSize = MinWidthSize;
                m_previewValue.MinHeightSize = MinHeightSize;
                m_previewValue.MaxHeightSize = MaxHeightSize;

                m_previewValue.MinDefectSize = minDefectSize;
                m_previewValue.MinSmallDefectSize = minSmallDefectSize;
                m_previewValue.MinSmallDefectCount = minSmallDefectCount;
                m_previewValue.MaxDefectSize = maxDefectSize;
                m_previewValue.MaxSmallDefectSize = maxSmallDefectSize;
                m_previewValue.MaxSmallDefectCount = maxSmallDefectCount;
                m_previewValue.UsePSRShift = UsePSR;
                m_previewValue.UsePunchShift = UseState;
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
            if (m_enumInspectType == eVisInspectType.eInspTypeBallPattern)
            {
                #region SurfaceHalfEtchingDefaultValue
                if (!BallPatternDefaultValue.DefaultValueLoaded)
                {
                    BallPatternDefaultValue.DefaultValueLoaded = true;
                    BallPatternDefaultValue.LoadDefaultValue();
                }

                #region 임계 설정.
                if (BallPatternDefaultValue.ThreshType == 0)
                {
                    this.radThresholdUpper.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = false;
                    this.txtThreshold.Text = BallPatternDefaultValue.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = BallPatternDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = BallPatternDefaultValue.UpperThresh.ToString();
                    SetRadioState1();
                }
                else if (BallPatternDefaultValue.ThreshType == 1)
                {
                    this.radThresholdLower.IsChecked = true;
                    this.chkMinRange.IsChecked = false;
                    this.chkMaxRange.IsChecked = true;
                    this.txtThreshold.Text = BallPatternDefaultValue.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = BallPatternDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = BallPatternDefaultValue.UpperThresh.ToString();
                    SetRadioState1();
                }
                else
                {
                    this.radThresholdRange.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = true;
                    this.txtLowerThreshold.Text = BallPatternDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = BallPatternDefaultValue.UpperThresh.ToString();
                    this.txtThreshold.Text = BallPatternDefaultValue.LowerThresh.ToString();
                    SetRadioState2();
                }
                #endregion

                // 마스크 하한/상한 임계
                this.txtMaskLowerThresh.Text = BallPatternDefaultValue.MaskLowerThresh.ToString();
                this.txtMaskUpperThresh.Text = BallPatternDefaultValue.MaskUpperThresh.ToString();

                // this.chkSameValue.IsChecked = (BallPatternDefaultValue.SameValue == 1) ? true : false;

                // 평균값 적용.
                this.chkApplyAverDiff.IsChecked = (BallPatternDefaultValue.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = BallPatternDefaultValue.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = BallPatternDefaultValue.AverMaxMargin.ToString();

                // 검사조건 범위
                this.SetCheckBoxState(BallPatternDefaultValue.InspRange);
                this.txtMinMargin.Text = BallPatternDefaultValue.MinMargin.ToString();
                this.txtMaxMargin.Text = BallPatternDefaultValue.MaxMargin.ToString();

                // 마스크 축소/확장, 결과 축소/확장.
                this.txtErosionTrainIter.Text = BallPatternDefaultValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = BallPatternDefaultValue.DilationTrainIter.ToString();
                //this.txtErosionInspIter.Text = BallPatternDefaultValue.ErosionInspIter.ToString();
                //this.txtDilationInspIter.Text = BallPatternDefaultValue.DilationInspIter.ToString();

                //// 최소 검출 사이즈, 미세불량 최소 검출 사이즈, 검출 개수.
                //this.txtMinDefectSize.Text = BallPatternDefaultValue.MinDefectSize.ToString();
                //this.txtMinSmallDefectSize.Text = BallPatternDefaultValue.MinSmallDefectSize.ToString();
                //this.txtMinSmallDefectCount.Text = BallPatternDefaultValue.MinSmallDefectCount.ToString();

                this.txtMinWidthSize.Text = BallPatternDefaultValue.MinWidthSize.ToString();
                this.txtMinHeightSize.Text = BallPatternDefaultValue.MinHeightSize.ToString();
                this.txtMaxHeightSize.Text = BallPatternDefaultValue.MaxHeightSize.ToString();

                //this.txtMaxDefectSize.Text = BallPatternDefaultValue.MaxDefectSize.ToString();
                //this.txtMaxSmallDefectSize.Text = BallPatternDefaultValue.MaxSmallDefectSize.ToString();
                //this.txtMaxSmallDefectCount.Text = BallPatternDefaultValue.MaxSmallDefectCount.ToString();

                chkPSRShift.IsChecked = (BallPatternDefaultValue.UsePSRShift == 1) ? true : false;
                //chkState.IsChecked = (BallPatternDefaultValue.UsePunchShift == 1) ? true : false;
                #endregion
            }
        }

        public void SetPreviewValue()
        {
            if (m_previewValue != null)
            {
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
                }

                this.txtMaskLowerThresh.Text = m_previewValue.MaskLowerThresh.ToString();
                this.txtMaskUpperThresh.Text = m_previewValue.MaskUpperThresh.ToString();

                this.chkApplyAverDiff.IsChecked = (m_previewValue.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = m_previewValue.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = m_previewValue.AverMaxMargin.ToString();

                this.SetCheckBoxState(m_previewValue.InspRange);
                this.txtMinMargin.Text = m_previewValue.MinMargin.ToString();
                this.txtMaxMargin.Text = m_previewValue.MaxMargin.ToString();

                this.txtErosionTrainIter.Text = m_previewValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = m_previewValue.DilationTrainIter.ToString();
                //this.txtErosionInspIter.Text = m_previewValue.ErosionInspIter.ToString();
                //this.txtDilationInspIter.Text = m_previewValue.DilationInspIter.ToString();

                //this.txtMinDefectSize.Text = m_previewValue.MinDefectSize.ToString();
                //this.txtMinSmallDefectSize.Text = m_previewValue.MinSmallDefectSize.ToString();
                //this.txtMinSmallDefectCount.Text = m_previewValue.MinSmallDefectCount.ToString();

                this.txtMinWidthSize.Text = m_previewValue.MinWidthSize.ToString();
                this.txtMinHeightSize.Text = m_previewValue.MinHeightSize.ToString();
                this.txtMaxHeightSize.Text = m_previewValue.MaxHeightSize.ToString();

                //this.txtMaxDefectSize.Text = m_previewValue.MaxDefectSize.ToString();
                //this.txtMaxSmallDefectSize.Text = m_previewValue.MaxSmallDefectSize.ToString();
                //this.txtMaxSmallDefectCount.Text = m_previewValue.MaxSmallDefectCount.ToString();
                chkPSRShift.IsChecked = (m_previewValue.UsePSRShift == 1) ? true : false;
                //chkState.IsChecked = (m_previewValue.UsePunchShift == 1) ? true : false;
                //  chkSameValue.IsChecked = (m_previewValue.SameValue == 1) ? true : false;
            }
        }

        // 검사 설정 표시.
        public void Display(InspectionInformation.InspectionItem settingValue, int MarginX, int MatginY)
        {
            BallPatternProperty stateAlignedMaskProperty = settingValue.InspectionAlgorithm as BallPatternProperty;
            if (stateAlignedMaskProperty != null)
            {
                #region 임계 설정.
                if (stateAlignedMaskProperty.ThreshType == 0) // 임계 이상
                {
                    this.radThresholdUpper.IsChecked = true;
                    this.txtThreshold.Text = stateAlignedMaskProperty.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = stateAlignedMaskProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = stateAlignedMaskProperty.UpperThresh.ToString();
                    SetRadioState1();
                }
                else if (stateAlignedMaskProperty.ThreshType == 1) // 임계 이하
                {
                    this.radThresholdLower.IsChecked = true;
                    this.txtThreshold.Text = stateAlignedMaskProperty.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = stateAlignedMaskProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = stateAlignedMaskProperty.UpperThresh.ToString();
                    SetRadioState1();
                }
                else // 임계 범위
                {
                    this.radThresholdRange.IsChecked = true;
                    this.txtLowerThreshold.Text = stateAlignedMaskProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = stateAlignedMaskProperty.UpperThresh.ToString();
                    this.txtThreshold.Text = stateAlignedMaskProperty.LowerThresh.ToString();
                    SetRadioState2();
                }
                #endregion

                // 동일값 적용.
                //  this.chkSameValue.IsChecked = (stateAlignedMaskProperty.SameValue == 1) ? true : false;
                this.txtMaskLowerThresh.Text = stateAlignedMaskProperty.MaskLowerThresh.ToString();
                this.txtMaskUpperThresh.Text = stateAlignedMaskProperty.MaskUpperThresh.ToString();

                // 평균값 적용
                this.chkApplyAverDiff.IsChecked = (stateAlignedMaskProperty.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = stateAlignedMaskProperty.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = stateAlignedMaskProperty.AverMaxMargin.ToString();

                // 검사조건 범위
                this.SetCheckBoxState(stateAlignedMaskProperty.InspRange);
                this.txtMinMargin.Text = stateAlignedMaskProperty.MinMargin.ToString();
                this.txtMaxMargin.Text = stateAlignedMaskProperty.MaxMargin.ToString();

                // 마스크 축소/확장, 결과 축소/확장
                this.txtErosionTrainIter.Text = stateAlignedMaskProperty.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = stateAlignedMaskProperty.DilationTrainIter.ToString();
                //this.txtErosionInspIter.Text = stateAlignedMaskProperty.ErosionInspIter.ToString();
                //this.txtDilationInspIter.Text = stateAlignedMaskProperty.DilationInspIter.ToString();

                // 최소 검출 사이즈, 미세불량 검출 사이즈, 미세불량 검출 개수
                //this.txtMinDefectSize.Text = stateAlignedMaskProperty.MinDefectSize.ToString();
                //this.txtMinSmallDefectSize.Text = stateAlignedMaskProperty.MinSmallDefectSize.ToString();
                //this.txtMinSmallDefectCount.Text = stateAlignedMaskProperty.MinSmallDefectCount.ToString();

                this.txtMinWidthSize.Text = stateAlignedMaskProperty.MinWidthSize.ToString();
                this.txtMinHeightSize.Text = stateAlignedMaskProperty.MinHeightSize.ToString();
                this.txtMaxHeightSize.Text = stateAlignedMaskProperty.MaxHeightSize.ToString();

                //this.txtMaxDefectSize.Text = stateAlignedMaskProperty.MaxDefectSize.ToString();
                //this.txtMaxSmallDefectSize.Text = stateAlignedMaskProperty.MaxSmallDefectSize.ToString();
                //this.txtMaxSmallDefectCount.Text = stateAlignedMaskProperty.MaxSmallDefectCount.ToString();
                chkPSRShift.IsChecked = (stateAlignedMaskProperty.UsePSRShift == 1) ? true : false;
                //chkState.IsChecked = (stateAlignedMaskProperty.UsePunchShift == 1) ? true : false;
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

