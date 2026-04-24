using System;
using System.Windows;
using System.Windows.Controls;
using Common.Drawing.InspectionInformation;

//////////////////////////////////////////////////////
// History
// 2012.04.05 - 코드 정리 완료. (검사 방법에 따라 초기 설정 값을 달리 가져갈 필요가 있다.)
//////////////////////////////////////////////////////
// StateAlignedMask 알고리즘 : 코드 3012
//////////////////////////////////////////////////////
// 표면 HalfEtching 검사 : 코드 0418
// 표면 검사 : 코드 0422
// 공간 검사 : 코드 0423
// PSR 검사 : 코드 0431
//////////////////////////////////////////////////////

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>   State aligned mask.  </summary>
    public partial class StateAlignedMask : UserControl, IInspectionTypeUICommands
    {
        private StateAlignedMaskProperty m_previewValue;

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

        public StateAlignedMask()
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

        private void SetSameValue()
        {
            if (radThresholdUpper.IsChecked == true)
            {
                this.chkMinRange.IsChecked = true;
                this.chkMaxRange.IsChecked = false;
               // if (chkSameValue.IsChecked == true)
               // {
               //     this.txtMaskLowerThresh.Text = this.txtThreshold.Text;
                    this.txtMaskUpperThresh.Text = "255";
              //  }
            }
            else if (radThresholdLower.IsChecked == true)
            {
                this.chkMinRange.IsChecked = false;
                this.chkMaxRange.IsChecked = true;
               // if (chkSameValue.IsChecked == true)
               // {
                    this.txtMaskLowerThresh.Text = "0";
                //    this.txtMaskUpperThresh.Text = this.txtThreshold.Text;
               // }
            }
            else //radThresholdRange.IsChecked == true
            {
                this.chkMinRange.IsChecked = true;
                this.chkMaxRange.IsChecked = true;
               // if (chkSameValue.IsChecked == true)
               // {
               //     this.txtMaskLowerThresh.Text = this.txtLowerThreshold.Text;
               //     this.txtMaskUpperThresh.Text = this.txtUpperThreshold.Text;
               // }
            }
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

        private void SetRadioState1()
        {
            if ((bool)radThresholdUpper.IsChecked)
            {
                this.txtMinDefectSize.IsEnabled = true;
                this.txtMinSmallDefectCount.IsEnabled = true;
                this.txtMinSmallDefectSize.IsEnabled = true;
                
                this.txtMaxDefectSize.IsEnabled = false;
                this.txtMaxSmallDefectCount.IsEnabled = false;
                this.txtMaxSmallDefectSize.IsEnabled = false;
            }
            else
            {
                this.txtMinDefectSize.IsEnabled = false;
                this.txtMinSmallDefectCount.IsEnabled = false;
                this.txtMinSmallDefectSize.IsEnabled = false;
                
                this.txtMaxDefectSize.IsEnabled = true;
                this.txtMaxSmallDefectCount.IsEnabled = true;
                this.txtMaxSmallDefectSize.IsEnabled = true;
                
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
            this.txtMinDefectSize.IsEnabled = true;
            this.txtMinSmallDefectCount.IsEnabled = true;
            this.txtMinSmallDefectSize.IsEnabled = true;
            this.txtMaxDefectSize.IsEnabled = true;
            this.txtMaxSmallDefectCount.IsEnabled = true;
            this.txtMaxSmallDefectSize.IsEnabled = true;

            this.txtLowerGV.Visibility = Visibility.Visible;
            this.txtLowerThreshold.Visibility = Visibility.Visible;
            this.txtUpperGV.Visibility = Visibility.Visible;
            this.txtUpperThreshold.Visibility = Visibility.Visible;
            this.txtThreshold.Visibility = Visibility.Collapsed;
            this.txtGV.Visibility = Visibility.Collapsed;
        }

        public void SetDialog(string strCaption, InspectionInformation.eVisInspectType inspectType)
        {
            this.txtCaption.Text = strCaption;
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
                int samevalue =  0;

                int inspRange = 0;
                int minMargin = 0;
                int maxMargin = 0;
                if (chkMinRange.IsChecked == true)
                {
                    minMargin = 0;
                    inspRange += 1;
                }
                if (chkMaxRange.IsChecked == true)
                {
                    maxMargin = 0;
                    inspRange += 2;
                }
                if (chkInRange.IsChecked == true)
                {
                    minMargin = 0;
                    maxMargin = 0;
                    inspRange += 4;
                }
                
                int erosionTrainIter = Convert.ToInt32(txtErosionTrainIter.Text);
                int dilationTrainIter = Convert.ToInt32(txtDilationTrainIter.Text);
                int erosionInspIter = Convert.ToInt32(txtErosionInspIter.Text);
                int dilationInspIter = Convert.ToInt32(txtDilationInspIter.Text);
                
                int minDefectSize = (chkMinRange.IsChecked == true) ? Convert.ToInt32(txtMinDefectSize.Text) : 0;
                int minSmallDefectSize = (chkMinRange.IsChecked == true) ? Convert.ToInt32(txtMinSmallDefectSize.Text) : 0;
                int minSmallDefectCount = (chkMinRange.IsChecked == true) ? Convert.ToInt32(txtMinSmallDefectCount.Text) : 0;
                int maxDefectSize = (chkMaxRange.IsChecked == true) ? Convert.ToInt32(txtMaxDefectSize.Text) : 0;
                int maxSmallDefectSize = (chkMaxRange.IsChecked == true) ? Convert.ToInt32(txtMaxSmallDefectSize.Text) : 0;
                int maxSmallDefectCount = (chkMaxRange.IsChecked == true) ? Convert.ToInt32(txtMaxSmallDefectCount.Text) : 0;

                StateAlignedMaskProperty oldStateAlignedMaskValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldStateAlignedMaskValue = element.InspectionAlgorithm as StateAlignedMaskProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }
                if (m_enumInspectType == eVisInspectType.eInspTypeSurface)
                {
                    if (minDefectSize > paradata.MaxSurface)
                    {
                        if (oldStateAlignedMaskValue != null)
                        {
                            minDefectSize = oldStateAlignedMaskValue.MinDefectSize;
                        }
                        else
                        {
                            minDefectSize = paradata.MaxSurface;
                        }
                        txtMinDefectSize.Text = minDefectSize.ToString();
                        txtMinDefectSize.Focus();
                        MessageBox.Show("파라매터 제한 값 확인 바랍니다.", "Information");
                    }
                    if (maxDefectSize > paradata.MaxSurface)
                    {
                        if (oldStateAlignedMaskValue != null)
                        {
                            maxDefectSize = oldStateAlignedMaskValue.MaxDefectSize;
                        }
                        else
                        {
                            maxDefectSize = paradata.MaxSurface;
                        }
                        txtMaxDefectSize.Text = maxDefectSize.ToString();
                        txtMaxDefectSize.Focus();
                        MessageBox.Show("파라매터 제한 값 확인 바랍니다.", "Information");
                    }
                }
                else if (m_enumInspectType == eVisInspectType.eInspTypePSR)
                {
                    if (minDefectSize > paradata.MaxSurface)
                    {
                        if (oldStateAlignedMaskValue != null)
                        {
                            minDefectSize = oldStateAlignedMaskValue.MinDefectSize;
                        }
                        else
                        {
                            minDefectSize = paradata.MaxPSR;
                        }
                        txtMinDefectSize.Text = minDefectSize.ToString();
                        txtMinDefectSize.Focus();
                        MessageBox.Show("파라매터 제한 값 확인 바랍니다.", "Information");
                    }
                    if (maxDefectSize > paradata.MaxPSR)
                    {
                        if (oldStateAlignedMaskValue != null)
                        {
                            maxDefectSize = oldStateAlignedMaskValue.MaxDefectSize;
                        }
                        else
                        {
                            maxDefectSize = paradata.MaxPSR;
                        }
                        txtMaxDefectSize.Text = maxDefectSize.ToString();
                        txtMaxDefectSize.Focus();
                        MessageBox.Show("파라매터 제한 값 확인 바랍니다.", "Information");
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
                    StateAlignedMaskProperty stateAlignedMaskValue = new StateAlignedMaskProperty();

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
                    stateAlignedMaskValue.MaxDefectSize = maxDefectSize;
                    stateAlignedMaskValue.MaxSmallDefectSize = maxSmallDefectSize;
                    stateAlignedMaskValue.MaxSmallDefectCount = maxSmallDefectCount;

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
                    oldStateAlignedMaskValue.MaxDefectSize = maxDefectSize;
                    oldStateAlignedMaskValue.MaxSmallDefectSize = maxSmallDefectSize;
                    oldStateAlignedMaskValue.MaxSmallDefectCount = maxSmallDefectCount;
                }

                if (m_previewValue == null)
                    m_previewValue = new StateAlignedMaskProperty();
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
                m_previewValue.MaxDefectSize = maxDefectSize;
                m_previewValue.MaxSmallDefectSize = maxSmallDefectSize;
                m_previewValue.MaxSmallDefectCount = maxSmallDefectCount;
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
                    minMargin = 0;
                    inspRange += 1;
                }
                if (chkMaxRange.IsChecked == true)
                {
                    maxMargin = 0;
                    inspRange += 2;
                }
                if (chkInRange.IsChecked == true)
                {
                    minMargin = 0;
                    maxMargin = 0;
                    inspRange += 4;
                }

                int erosionTrainIter = Convert.ToInt32(txtErosionTrainIter.Text);
                int dilationTrainIter = Convert.ToInt32(txtDilationTrainIter.Text);
                int erosionInspIter = Convert.ToInt32(txtErosionInspIter.Text);
                int dilationInspIter = Convert.ToInt32(txtDilationInspIter.Text);

                int minDefectSize = (chkMinRange.IsChecked == true) ? Convert.ToInt32(txtMinDefectSize.Text) : 0;
                int minSmallDefectSize = (chkMinRange.IsChecked == true) ? Convert.ToInt32(txtMinSmallDefectSize.Text) : 0;
                int minSmallDefectCount = (chkMinRange.IsChecked == true) ? Convert.ToInt32(txtMinSmallDefectCount.Text) : 0;
                int maxDefectSize = (chkMaxRange.IsChecked == true) ? Convert.ToInt32(txtMaxDefectSize.Text) : 0;
                int maxSmallDefectSize = (chkMaxRange.IsChecked == true) ? Convert.ToInt32(txtMaxSmallDefectSize.Text) : 0;
                int maxSmallDefectCount = (chkMaxRange.IsChecked == true) ? Convert.ToInt32(txtMaxSmallDefectCount.Text) : 0;

                StateAlignedMaskProperty oldStateAlignedMaskValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldStateAlignedMaskValue = element.InspectionAlgorithm as StateAlignedMaskProperty; // 기존에 저장된 값이 있는 경우.
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
                    StateAlignedMaskProperty stateAlignedMaskValue = new StateAlignedMaskProperty();

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
                    stateAlignedMaskValue.MaxDefectSize = maxDefectSize;
                    stateAlignedMaskValue.MaxSmallDefectSize = maxSmallDefectSize;
                    stateAlignedMaskValue.MaxSmallDefectCount = maxSmallDefectCount;

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

                    oldStateAlignedMaskValue.MinDefectSize = minDefectSize;
                    oldStateAlignedMaskValue.MinSmallDefectSize = minSmallDefectSize;
                    oldStateAlignedMaskValue.MinSmallDefectCount = minSmallDefectCount;
                    oldStateAlignedMaskValue.MaxDefectSize = maxDefectSize;
                    oldStateAlignedMaskValue.MaxSmallDefectSize = maxSmallDefectSize;
                    oldStateAlignedMaskValue.MaxSmallDefectCount = maxSmallDefectCount;
                }

                if (m_previewValue == null)
                    m_previewValue = new StateAlignedMaskProperty();
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
                m_previewValue.MaxDefectSize = maxDefectSize;
                m_previewValue.MaxSmallDefectSize = maxSmallDefectSize;
                m_previewValue.MaxSmallDefectCount = maxSmallDefectCount;
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
        // 검사의 직전 값 표시.
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
                //this.chkSameValue.IsChecked = (m_previewValue.SameValue == 1) ? true : false;
                this.chkApplyAverDiff.IsChecked = (m_previewValue.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = m_previewValue.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = m_previewValue.AverMaxMargin.ToString();

                this.SetCheckBoxState(m_previewValue.InspRange);

                this.txtErosionTrainIter.Text = m_previewValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = m_previewValue.DilationTrainIter.ToString();
                this.txtErosionInspIter.Text = m_previewValue.ErosionInspIter.ToString();
                this.txtDilationInspIter.Text = m_previewValue.DilationInspIter.ToString();

                this.txtMinDefectSize.Text = m_previewValue.MinDefectSize.ToString();
                this.txtMinSmallDefectSize.Text = m_previewValue.MinSmallDefectSize.ToString();
                this.txtMinSmallDefectCount.Text = m_previewValue.MinSmallDefectCount.ToString();
                this.txtMaxDefectSize.Text = m_previewValue.MaxDefectSize.ToString();
                this.txtMaxSmallDefectSize.Text = m_previewValue.MaxSmallDefectSize.ToString();
                this.txtMaxSmallDefectCount.Text = m_previewValue.MaxSmallDefectCount.ToString();
            }
        }

        // 검사의 기본 값 표시.
        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeSurfaceHalfEtching)
            {
                #region SurfaceHalfEtchingDefaultValue
                if (!SurfaceHalfEtchingDefaultValue.DefaultValueLoaded)
                {
                    SurfaceHalfEtchingDefaultValue.DefaultValueLoaded = true;
                    SurfaceHalfEtchingDefaultValue.LoadDefaultValue();
                }

                #region 임계 설정.
                if (SurfaceHalfEtchingDefaultValue.ThreshType == 0)
                {
                    this.radThresholdUpper.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = false;
                    this.txtThreshold.Text = SurfaceHalfEtchingDefaultValue.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = SurfaceHalfEtchingDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SurfaceHalfEtchingDefaultValue.UpperThresh.ToString();
                    SetRadioState1();
                }
                else if (SurfaceHalfEtchingDefaultValue.ThreshType == 1)
                {
                    this.radThresholdLower.IsChecked = true;
                    this.chkMinRange.IsChecked = false;
                    this.chkMaxRange.IsChecked = true;
                    this.txtThreshold.Text = SurfaceHalfEtchingDefaultValue.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = SurfaceHalfEtchingDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SurfaceHalfEtchingDefaultValue.UpperThresh.ToString();
                    SetRadioState1();
                }
                else
                {
                    this.radThresholdRange.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = true;
                    this.txtLowerThreshold.Text = SurfaceHalfEtchingDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SurfaceHalfEtchingDefaultValue.UpperThresh.ToString();
                    this.txtThreshold.Text = SurfaceHalfEtchingDefaultValue.LowerThresh.ToString();
                    SetRadioState2();
                }
                #endregion

                // 마스크 하한/상한 임계
                this.txtMaskLowerThresh.Text = SurfaceHalfEtchingDefaultValue.MaskLowerThresh.ToString();
                this.txtMaskUpperThresh.Text = SurfaceHalfEtchingDefaultValue.MaskUpperThresh.ToString();
              //  this.chkSameValue.IsChecked = (SurfaceHalfEtchingDefaultValue.SameValue == 1) ? true : false;

                // 평균값 적용.
                this.chkApplyAverDiff.IsChecked = (SurfaceHalfEtchingDefaultValue.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = SurfaceHalfEtchingDefaultValue.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = SurfaceHalfEtchingDefaultValue.AverMaxMargin.ToString();
                
                // 검사조건 범위
                this.SetCheckBoxState(SurfaceHalfEtchingDefaultValue.InspRange);

                // 마스크 축소/확장, 결과 축소/확장.
                this.txtErosionTrainIter.Text = SurfaceHalfEtchingDefaultValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = SurfaceHalfEtchingDefaultValue.DilationTrainIter.ToString();
                this.txtErosionInspIter.Text = SurfaceHalfEtchingDefaultValue.ErosionInspIter.ToString();
                this.txtDilationInspIter.Text = SurfaceHalfEtchingDefaultValue.DilationInspIter.ToString();
                
                // 최소 검출 사이즈, 미세불량 최소 검출 사이즈, 검출 개수.
                this.txtMinDefectSize.Text = SurfaceHalfEtchingDefaultValue.MinDefectSize.ToString();
                this.txtMinSmallDefectSize.Text = SurfaceHalfEtchingDefaultValue.MinSmallDefectSize.ToString();
                this.txtMinSmallDefectCount.Text = SurfaceHalfEtchingDefaultValue.MinSmallDefectCount.ToString();
                this.txtMaxDefectSize.Text = SurfaceHalfEtchingDefaultValue.MaxDefectSize.ToString();
                this.txtMaxSmallDefectSize.Text = SurfaceHalfEtchingDefaultValue.MaxSmallDefectSize.ToString();
                this.txtMaxSmallDefectCount.Text = SurfaceHalfEtchingDefaultValue.MaxSmallDefectCount.ToString();
                #endregion
            }
            else if (m_enumInspectType == eVisInspectType.eInspTypeSurface || m_enumInspectType == eVisInspectType.eInspTypeDownSet)
            {
                #region SurfaceDefaultValue
                if (!SurfaceDefaultValue.DefaultValueLoaded)
                {
                    SurfaceDefaultValue.DefaultValueLoaded = true;
                    SurfaceDefaultValue.LoadDefaultValue();
                }

                #region 임계 설정.
                if (SurfaceDefaultValue.ThreshType == 0)
                {
                    this.radThresholdUpper.IsChecked = true;
                    this.txtThreshold.Text = SurfaceDefaultValue.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = SurfaceDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SurfaceDefaultValue.UpperThresh.ToString();
                    SetRadioState1();
                }
                else if (SurfaceDefaultValue.ThreshType == 1)
                {
                    this.radThresholdLower.IsChecked = true;
                    this.txtThreshold.Text = SurfaceDefaultValue.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = SurfaceDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SurfaceDefaultValue.UpperThresh.ToString();
                    SetRadioState1();
                }
                else
                {
                    this.radThresholdRange.IsChecked = true;
                    this.txtLowerThreshold.Text = SurfaceDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SurfaceDefaultValue.UpperThresh.ToString();
                    this.txtThreshold.Text = SurfaceDefaultValue.LowerThresh.ToString();
                    SetRadioState2();
                }
                #endregion

                // 마스크 하한/상한 임계.
                this.txtMaskLowerThresh.Text = SurfaceDefaultValue.MaskLowerThresh.ToString();
                this.txtMaskUpperThresh.Text = SurfaceDefaultValue.MaskUpperThresh.ToString();

               // this.chkSameValue.IsChecked = (SurfaceDefaultValue.SameValue == 1) ? true : false;

                // 평균값 적용.
                this.chkApplyAverDiff.IsChecked = (SurfaceDefaultValue.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = SurfaceDefaultValue.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = SurfaceDefaultValue.AverMaxMargin.ToString();

                // 검사조건 범위.
                this.SetCheckBoxState(SurfaceDefaultValue.InspRange);
                
                // 마스크 축소/확장, 결과 축소/확장.
                this.txtErosionTrainIter.Text = SurfaceDefaultValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = SurfaceDefaultValue.DilationTrainIter.ToString();
                this.txtErosionInspIter.Text = SurfaceDefaultValue.ErosionInspIter.ToString();
                this.txtDilationInspIter.Text = SurfaceDefaultValue.DilationInspIter.ToString();
                
                // 최소 검출 사이즈.
                this.txtMinDefectSize.Text = SurfaceDefaultValue.MinDefectSize.ToString();
                this.txtMinSmallDefectSize.Text = SurfaceDefaultValue.MinSmallDefectSize.ToString();
                this.txtMinSmallDefectCount.Text = SurfaceDefaultValue.MinSmallDefectCount.ToString();
                this.txtMaxDefectSize.Text = SurfaceDefaultValue.MaxDefectSize.ToString();
                this.txtMaxSmallDefectSize.Text = SurfaceDefaultValue.MaxSmallDefectSize.ToString();
                this.txtMaxSmallDefectCount.Text = SurfaceDefaultValue.MaxSmallDefectCount.ToString();
                #endregion
            }
            else if (m_enumInspectType == eVisInspectType.eInspTypeSpace)
            {
                #region SpaceDefaultValue
                if (!SpaceDefaultValue.DefaultValueLoaded)
                {
                    SpaceDefaultValue.DefaultValueLoaded = true;
                    SpaceDefaultValue.LoadDefaultValue();
                }

                #region 임계 설정.
                if (SpaceDefaultValue.ThreshType == 0)
                {
                    this.radThresholdUpper.IsChecked = true;
                    this.txtThreshold.Text = SpaceDefaultValue.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = SpaceDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SpaceDefaultValue.UpperThresh.ToString();
                    SetRadioState1();
                }
                else if (SpaceDefaultValue.ThreshType == 1)
                {
                    this.radThresholdLower.IsChecked = true;
                    this.txtThreshold.Text = SpaceDefaultValue.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = SpaceDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SpaceDefaultValue.UpperThresh.ToString();
                    SetRadioState1();
                }
                else
                {
                    this.radThresholdRange.IsChecked = true;
                    this.txtLowerThreshold.Text = SpaceDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SpaceDefaultValue.UpperThresh.ToString();
                    this.txtThreshold.Text = SpaceDefaultValue.LowerThresh.ToString();
                    SetRadioState2();
                }
                #endregion

                // 마스크 하한/상한.
                this.txtMaskLowerThresh.Text = SpaceDefaultValue.MaskLowerThresh.ToString();
                this.txtMaskUpperThresh.Text = SpaceDefaultValue.MaskUpperThresh.ToString();

              //  this.chkSameValue.IsChecked = (SpaceDefaultValue.SameValue == 1) ? true : false;

                // 평균값 적용.
                this.chkApplyAverDiff.IsChecked = (SpaceDefaultValue.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = SpaceDefaultValue.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = SpaceDefaultValue.AverMaxMargin.ToString();

                // 검사조건 범위.
                this.SetCheckBoxState(SpaceDefaultValue.InspRange);
                
                // 마스크 축소/확장, 결과 축소/확장.
                this.txtErosionTrainIter.Text = SpaceDefaultValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = SpaceDefaultValue.DilationTrainIter.ToString();
                this.txtErosionInspIter.Text = SpaceDefaultValue.ErosionInspIter.ToString();
                this.txtDilationInspIter.Text = SpaceDefaultValue.DilationInspIter.ToString();
                
                // 최소 검출 사이즈.
                this.txtMinDefectSize.Text = SpaceDefaultValue.MinDefectSize.ToString();
                this.txtMinSmallDefectSize.Text = SpaceDefaultValue.MinSmallDefectSize.ToString();
                this.txtMinSmallDefectCount.Text = SpaceDefaultValue.MinSmallDefectCount.ToString();
                this.txtMaxDefectSize.Text = SpaceDefaultValue.MaxDefectSize.ToString();
                this.txtMaxSmallDefectSize.Text = SpaceDefaultValue.MaxSmallDefectSize.ToString();
                this.txtMaxSmallDefectCount.Text = SpaceDefaultValue.MaxSmallDefectCount.ToString();
                #endregion
            }
        }

        // 검사 설정 표시.
        public void Display(InspectionInformation.InspectionItem settingValue, int MarginX, int MatginY)
        {
            StateAlignedMaskProperty stateAlignedMaskProperty = settingValue.InspectionAlgorithm as StateAlignedMaskProperty;
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
                //this.chkSameValue.IsChecked = true;
                this.txtMaskLowerThresh.Text = stateAlignedMaskProperty.MaskLowerThresh.ToString();
                this.txtMaskUpperThresh.Text = stateAlignedMaskProperty.MaskUpperThresh.ToString();

                // 평균값 적용
                this.chkApplyAverDiff.IsChecked = (stateAlignedMaskProperty.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = stateAlignedMaskProperty.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = stateAlignedMaskProperty.AverMaxMargin.ToString();

                // 검사조건 범위
                this.SetCheckBoxState(stateAlignedMaskProperty.InspRange);

                // 마스크 축소/확장, 결과 축소/확장
                this.txtErosionTrainIter.Text = stateAlignedMaskProperty.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = stateAlignedMaskProperty.DilationTrainIter.ToString();
                this.txtErosionInspIter.Text = stateAlignedMaskProperty.ErosionInspIter.ToString();
                this.txtDilationInspIter.Text = stateAlignedMaskProperty.DilationInspIter.ToString();

                // 최소 검출 사이즈, 미세불량 검출 사이즈, 미세불량 검출 개수
                this.txtMinDefectSize.Text = stateAlignedMaskProperty.MinDefectSize.ToString();
                this.txtMinSmallDefectSize.Text = stateAlignedMaskProperty.MinSmallDefectSize.ToString();
                this.txtMinSmallDefectCount.Text = stateAlignedMaskProperty.MinSmallDefectCount.ToString();
                this.txtMaxDefectSize.Text = stateAlignedMaskProperty.MaxDefectSize.ToString();
                this.txtMaxSmallDefectSize.Text = stateAlignedMaskProperty.MaxSmallDefectSize.ToString();
                this.txtMaxSmallDefectCount.Text = stateAlignedMaskProperty.MaxSmallDefectCount.ToString();
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
