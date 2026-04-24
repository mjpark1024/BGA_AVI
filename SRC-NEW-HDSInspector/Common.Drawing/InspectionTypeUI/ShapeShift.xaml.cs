using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Common.Drawing.InspectionInformation;

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>   Shape shift.  </summary>
    /// <remarks>   suoow2, 2014-10-10. </remarks>
    public partial class ShapeShift : UserControl, IInspectionTypeUICommands
    {
        private ShapeShiftProperty m_previewValue;

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

        /// <summary> The default correct method </summary>
        private readonly static int DEFAULT_CORRECT_METHOD = 0;
        private int m_nShapeShiftType = 0;

        public ShapeShift()
        {
            InitializeComponent();
            InitializeEvent();

            SetPlatingRadioState1();
            SetSurfaceRadioState2();

            svViewer.ScrollToBottom();
        }

        private void InitializeEvent()
        {
            this.radThresholdLower.Click += radThreshold_Click;
            this.radThresholdUpper.Click += radThreshold_Click;
            this.radThresholdRange.Click += radThresholdRange_Click;

            this.radThresholdLower2.Click += radThreshold2_Click;
            this.radThresholdUpper2.Click += radThreshold2_Click;
            this.radThresholdRange2.Click += radThresholdRange2_Click;
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

            SetPlatingRadioState1();
            this.txtThreshold.Focus();
        }

        private void radThresholdRange_Click(object sender, RoutedEventArgs e)
        {
            SetPlatingRadioState2();
            this.txtLowerThreshold.Focus();
        }

        private void radThreshold2_Click(object sender, RoutedEventArgs e)
        {
            if (sender == radThresholdUpper2)
            {
                chkMinRange2.IsChecked = true;
                chkMaxRange2.IsChecked = false;
            }
            else
            {
                chkMinRange2.IsChecked = false;
                chkMaxRange2.IsChecked = true;
            }
            chkInRange2.IsChecked = false;

            SetSurfaceRadioState1();
            this.txtThreshold2.Focus();
        }
        
        private void radThresholdRange2_Click(object sender, RoutedEventArgs e)
        {
            SetSurfaceRadioState2();
            this.txtLowerThreshold2.Focus();
        }

        private void SetPlatingRadioState1()
        {
            if ((bool)radThresholdUpper.IsChecked)
            {
                this.txtMinDefectSize.IsEnabled = true;
                this.txtMaxDefectSize.IsEnabled = false;
            }
            else
            {
                this.txtMinDefectSize.IsEnabled = false;
                this.txtMaxDefectSize.IsEnabled = true;
            }
            this.txtLowerGV.Visibility = Visibility.Collapsed;
            this.txtLowerThreshold.Visibility = Visibility.Collapsed;
            this.txtUpperGV.Visibility = Visibility.Collapsed;
            this.txtUpperThreshold.Visibility = Visibility.Collapsed;
            this.txtThreshold.Visibility = Visibility.Visible;
            this.txtGV.Visibility = Visibility.Visible;
        }

        private void SetPlatingRadioState2()
        {
            this.txtMinDefectSize.IsEnabled = true;
            this.txtMaxDefectSize.IsEnabled = true;

            this.chkMinRange.IsChecked = true;
            this.chkMaxRange.IsChecked = true;
            this.chkInRange.IsChecked = false;

            this.txtLowerGV.Visibility = Visibility.Visible;
            this.txtLowerThreshold.Visibility = Visibility.Visible;
            this.txtUpperGV.Visibility = Visibility.Visible;
            this.txtUpperThreshold.Visibility = Visibility.Visible;
            this.txtThreshold.Visibility = Visibility.Collapsed;
            this.txtGV.Visibility = Visibility.Collapsed;
        }

        private void SetSurfaceRadioState1()
        {
            if ((bool)radThresholdUpper2.IsChecked)
            {
                this.txtMinDefectSize2.IsEnabled = true;
                this.txtMaxDefectSize2.IsEnabled = false;
            }
            else
            {
                this.txtMinDefectSize2.IsEnabled = false;
                this.txtMaxDefectSize2.IsEnabled = true;
            }
            this.txtLowerGV2.Visibility = Visibility.Collapsed;
            this.txtLowerThreshold2.Visibility = Visibility.Collapsed;
            this.txtUpperGV2.Visibility = Visibility.Collapsed;
            this.txtUpperThreshold2.Visibility = Visibility.Collapsed;
            this.txtThreshold2.Visibility = Visibility.Visible;
            this.txtGV2.Visibility = Visibility.Visible;
        }

        private void SetSurfaceRadioState2()
        {
            this.txtMinDefectSize2.IsEnabled = true;
            this.txtMaxDefectSize2.IsEnabled = true;

            this.chkMinRange2.IsChecked = true;
            this.chkMaxRange2.IsChecked = true;
            this.chkInRange2.IsChecked = false;

            this.txtLowerGV2.Visibility = Visibility.Visible;
            this.txtLowerThreshold2.Visibility = Visibility.Visible;
            this.txtUpperGV2.Visibility = Visibility.Visible;
            this.txtUpperThreshold2.Visibility = Visibility.Visible;
            this.txtThreshold2.Visibility = Visibility.Collapsed;
            this.txtGV2.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region 검사 조건 범위에 따른 CheckBox 상태 표시.
        private void SetPlatingCheckBoxState(int switchValue)
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

        private void SetSurfaceCheckBoxState(int switchValue)
        {
            switch (switchValue)
            {
                case 1:
                    this.chkMinRange2.IsChecked = true; // 1
                    this.chkMaxRange2.IsChecked = false;
                    this.chkInRange2.IsChecked = false;
                    break;
                case 2:
                    this.chkMinRange2.IsChecked = false;
                    this.chkMaxRange2.IsChecked = true; // 2
                    this.chkInRange2.IsChecked = false;
                    break;
                case 3:
                    this.chkMinRange2.IsChecked = true;
                    this.chkMaxRange2.IsChecked = true; // 1 + 2
                    this.chkInRange2.IsChecked = false;
                    break;
                case 4:
                    this.chkMinRange2.IsChecked = false;
                    this.chkMaxRange2.IsChecked = false;
                    this.chkInRange2.IsChecked = true; // 4
                    break;
                case 5:
                    this.chkMinRange2.IsChecked = true;
                    this.chkMaxRange2.IsChecked = false;
                    this.chkInRange2.IsChecked = true; // 1 + 4
                    break;
                case 6:
                    this.chkMinRange2.IsChecked = false;
                    this.chkMaxRange2.IsChecked = true;
                    this.chkInRange2.IsChecked = true; // 2 + 4
                    break;
                case 7:
                    this.chkMinRange2.IsChecked = true;
                    this.chkMaxRange2.IsChecked = true;
                    this.chkInRange2.IsChecked = true; // 1 + 2 + 4
                    break;
            }
        }
        #endregion

        public void SetDialog(string strCaption, InspectionInformation.eVisInspectType inspectType)
        {
            this.txtCaption.Text = strCaption;
            this.m_enumInspectType = inspectType;
            if (m_enumInspectType == eVisInspectType.eInspTypeTape)
                m_nShapeShiftType = 1;
            else if (m_enumInspectType == eVisInspectType.eInspTypePlate)
                m_nShapeShiftType = 0;
        }

        // 검사 설정 저장.
        public void TrySave(GraphicsBase graphic, int anInspectID, ParametersLocking paradata)
        {
            try
            {
                #region Save Logic.
                int correctMethod = DEFAULT_CORRECT_METHOD;

                #region Check 전체 Mask 하한 / 상한
                int groundLowerThresh = Convert.ToInt32(txtGroundLowerThresh.Text);
                int groundUpperThresh = Convert.ToInt32(txtGroundUpperThresh.Text);
                if (groundLowerThresh > groundUpperThresh)
                {
                    MessageBox.Show("전체 Mask 하한 값은 전체 Mask 상한 값을 초과할 수 없습니다.", "Information");
                    this.txtGroundLowerThresh.Focus();
                    return;
                }
                #endregion

                #region Check 형상 Mask 하한 / 상한
                int shapeLowerThresh = Convert.ToInt32(txtShapeLowerThresh.Text);
                int shapeUpperThresh = Convert.ToInt32(txtShapeUpperThresh.Text);
                if (shapeLowerThresh > shapeUpperThresh)
                {
                    MessageBox.Show("형상 Mask 하한 값은 형상 Mask 상한 값을 초과할 수 없습니다.", "Information");
                    this.txtShapeLowerThresh.Focus();
                    return;
                }
                #endregion

                // 형상 Mask 축소 / 확장
                int erosionShapeIter = Convert.ToInt32(txtErosionShapeIter.Text);
                int dilationShapeIter = Convert.ToInt32(txtDilationShapeIter.Text);

                // 형상 Shift X / Y
                int shapeShiftMarginX = Convert.ToInt32(txtShapeShiftMarginX.Text);
                int shapeShiftMarginY = Convert.ToInt32(txtShapeShiftMarginY.Text);

                // 형상 Offset X / Y
                int shapeOffsetX = Convert.ToInt32(txtShapeOffsetX.Text);
                int shapeOffsetY = Convert.ToInt32(txtShapeOffsetY.Text);

                // Mask 축소 / 확장
                int erosionTrainIter = Convert.ToInt32(txtErosionTrainIter.Text);
                int dilationTrainIter = Convert.ToInt32(txtDilationTrainIter.Text);

                // 형상 영역 검사 적용
                int isInspectMaster = (chkMasterUse.IsChecked == true) ? 1 : 0;
                
                #region Check 도금 영역 검사 하한 / 상한 검출값
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

                // 도금 영역 검사 평균값 적용 여부
                int applyAverDiff = (chkApplyAverDiff.IsChecked == true) ? 1 : 0;

                // 도금 영역 평균값 최소 / 최대 허용 범위
                int averMinMargin = (applyAverDiff == 1) ? Convert.ToInt32(txtAverMinMargin.Text) : 0;
                int averMaxMargin = (applyAverDiff == 1) ? Convert.ToInt32(txtAverMaxMargin.Text) : 0;

                // 도금 영역 검사 최소 / 최대 허용범위
                int minMargin = Convert.ToInt32(txtMinMargin.Text);
                int maxMargin = Convert.ToInt32(txtMaxMargin.Text);

                #region 도금 영역 검사 검사 조건 범위
                int inspRange = 0;
                if (this.chkMinRange.IsChecked == true)
                {
                    inspRange += 1;
                }
                if (this.chkMaxRange.IsChecked == true)
                {
                    inspRange += 2;
                }
                if (this.chkInRange.IsChecked == true)
                {
                    inspRange += 4;
                }
                #endregion

                // 도금 영역 최소 검출 사이즈
                int minDefectSize = Convert.ToInt32(txtMinDefectSize.Text);
                int maxDefectSize = Convert.ToInt32(txtMaxDefectSize.Text);

                //////////////////////////////////////////////////////////////////////////////////////////////////////

                // 표면 영역 검사 적용
                int isInspectSlave = (chkSlaveUse.IsChecked == true) ? 1 : 0;

                #region Check 표면 영역 검사 하한 / 상한 검출값
                int threshType2 = 0;
                int lowerThresh2 = -1;
                int upperThresh2 = -1;
                if (radThresholdUpper2.IsChecked == true) // 임계 이상
                {
                    threshType2 = 0;
                    lowerThresh2 = Convert.ToInt32(txtThreshold2.Text);
                    upperThresh2 = 255;
                }
                else if (radThresholdLower2.IsChecked == true) // 임계 이하
                {
                    threshType2 = 1;
                    lowerThresh2 = 0;
                    upperThresh2 = Convert.ToInt32(txtThreshold2.Text);
                }
                else if (radThresholdRange2.IsChecked == true) // 임계 범위
                {
                    threshType2 = 2;
                    lowerThresh2 = Convert.ToInt32(txtLowerThreshold2.Text);
                    upperThresh2 = Convert.ToInt32(txtUpperThreshold2.Text);

                    if (lowerThresh2 > upperThresh2)
                    {
                        MessageBox.Show("임계 하한 값은 임계 상한 값을 초과할 수 없습니다.", "Information");
                        this.txtLowerThreshold2.Focus();
                        return;
                    }
                }
                #endregion
                
                // 표면 영역 검사 평균값 적용 여부
                int applyAverDiff2 = (chkApplyAverDiff2.IsChecked == true) ? 1 : 0;

                // 표면 영역 평균값 최소 / 최대 허용 범위
                int averMinMargin2 = (applyAverDiff2 == 1) ? Convert.ToInt32(txtAverMinMargin2.Text) : 0;
                int averMaxMargin2 = (applyAverDiff2 == 1) ? Convert.ToInt32(txtAverMaxMargin2.Text) : 0;

                #region 표면 영역 검사 검사 조건 범위
                int inspRange2 = 0;
                if (this.chkMinRange2.IsChecked == true)
                {
                    inspRange2 += 1;
                }
                if (this.chkMaxRange2.IsChecked == true)
                {
                    inspRange2 += 2;
                }
                if (this.chkInRange2.IsChecked == true)
                {
                    inspRange2 += 4;
                }
                #endregion

                // 표면 영역 검사 최소 / 최대 허용범위
                int minMargin2 = Convert.ToInt32(txtMinMargin2.Text);
                int maxMargin2 = Convert.ToInt32(txtMaxMargin2.Text);
                
                // 표면 영역 최소 검출 사이즈
                int minDefectSize2 = Convert.ToInt32(txtMinDefectSize2.Text);
                int maxDefectSize2 = Convert.ToInt32(txtMaxDefectSize2.Text);
                #endregion

                ShapeShiftProperty oldShapeShiftValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldShapeShiftValue = element.InspectionAlgorithm as ShapeShiftProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldShapeShiftValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    ShapeShiftProperty shapeShiftValue = new ShapeShiftProperty();

                    shapeShiftValue.CorrectMethod = correctMethod;

                    shapeShiftValue.GroundLowerThresh = groundLowerThresh;
                    shapeShiftValue.GroundUpperThresh = groundUpperThresh;
                    shapeShiftValue.ShapeLowerThresh = shapeLowerThresh;
                    shapeShiftValue.ShapeUpperThresh = shapeUpperThresh;
                    shapeShiftValue.ErosionShapeIter = erosionShapeIter;
                    shapeShiftValue.DilationShapeIter = dilationShapeIter;
                    shapeShiftValue.ShapeShiftMarginX = shapeShiftMarginX;
                    shapeShiftValue.ShapeShiftMarginY = shapeShiftMarginY;
                    shapeShiftValue.ShapeOffsetX = shapeOffsetX;
                    shapeShiftValue.ShapeOffsetY = shapeOffsetY;
                    shapeShiftValue.ErosionTrainIter = erosionTrainIter;
                    shapeShiftValue.DilationTrainIter = dilationTrainIter;

                    shapeShiftValue.IsInspectMaster = isInspectMaster;
                    shapeShiftValue.ThreshType = threshType;
                    shapeShiftValue.LowerThresh = lowerThresh;
                    shapeShiftValue.UpperThresh = upperThresh;
                    shapeShiftValue.ApplyAverDiff = applyAverDiff;
                    shapeShiftValue.InspRange = inspRange;
                    shapeShiftValue.AverMinMargin = averMinMargin;
                    shapeShiftValue.AverMaxMargin = averMaxMargin;
                    shapeShiftValue.MinMargin = minMargin;
                    shapeShiftValue.MaxMargin = maxMargin;
                    shapeShiftValue.MinDefectSize = minDefectSize;
                    shapeShiftValue.MaxDefectSize = maxDefectSize;

                    shapeShiftValue.IsInspectSlave = isInspectSlave;
                    shapeShiftValue.ThreshType2 = threshType2;
                    shapeShiftValue.InspLowerThresh2 = lowerThresh2;
                    shapeShiftValue.InspUpperThresh2 = upperThresh2;
                    shapeShiftValue.ApplyAverDiff2 = applyAverDiff2;
                    shapeShiftValue.InspRange2 = inspRange2;
                    shapeShiftValue.AverMinMargin2 = averMinMargin2;
                    shapeShiftValue.AverMaxMargin2 = averMaxMargin2;
                    shapeShiftValue.InspMinMargin2 = minMargin2;
                    shapeShiftValue.InspMaxMargin2 = maxMargin2;
                    shapeShiftValue.MinDefectSize2 = minDefectSize2;
                    shapeShiftValue.MaxDefectSize2 = maxDefectSize2;
                    
                    shapeShiftValue.ShapeShiftType = m_nShapeShiftType;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = shapeShiftValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldShapeShiftValue.CorrectMethod = correctMethod;

                    oldShapeShiftValue.GroundLowerThresh = groundLowerThresh;
                    oldShapeShiftValue.GroundUpperThresh = groundUpperThresh;
                    oldShapeShiftValue.ShapeLowerThresh = shapeLowerThresh;
                    oldShapeShiftValue.ShapeUpperThresh = shapeUpperThresh;
                    oldShapeShiftValue.ErosionShapeIter = erosionShapeIter;
                    oldShapeShiftValue.DilationShapeIter = dilationShapeIter;
                    oldShapeShiftValue.ShapeShiftMarginX = shapeShiftMarginX;
                    oldShapeShiftValue.ShapeShiftMarginY = shapeShiftMarginY;
                    oldShapeShiftValue.ShapeOffsetX = shapeOffsetX;
                    oldShapeShiftValue.ShapeOffsetY = shapeOffsetY;
                    oldShapeShiftValue.ErosionTrainIter = erosionTrainIter;
                    oldShapeShiftValue.DilationTrainIter = dilationTrainIter;

                    oldShapeShiftValue.IsInspectMaster = isInspectMaster;
                    oldShapeShiftValue.ThreshType = threshType;
                    oldShapeShiftValue.LowerThresh = lowerThresh;
                    oldShapeShiftValue.UpperThresh = upperThresh;
                    oldShapeShiftValue.ApplyAverDiff = applyAverDiff;
                    oldShapeShiftValue.InspRange = inspRange;
                    oldShapeShiftValue.AverMinMargin = averMinMargin;
                    oldShapeShiftValue.AverMaxMargin = averMaxMargin;
                    oldShapeShiftValue.MinMargin = minMargin;
                    oldShapeShiftValue.MaxMargin = maxMargin;
                    oldShapeShiftValue.MinDefectSize = minDefectSize;
                    oldShapeShiftValue.MaxDefectSize = maxDefectSize;

                    oldShapeShiftValue.IsInspectSlave = isInspectSlave;
                    oldShapeShiftValue.ThreshType2 = threshType2;
                    oldShapeShiftValue.InspLowerThresh2 = lowerThresh2;
                    oldShapeShiftValue.InspUpperThresh2 = upperThresh2;
                    oldShapeShiftValue.ApplyAverDiff2 = applyAverDiff2;
                    oldShapeShiftValue.InspRange2 = inspRange2;
                    oldShapeShiftValue.AverMinMargin2 = averMinMargin2;
                    oldShapeShiftValue.AverMaxMargin2 = averMaxMargin2;
                    oldShapeShiftValue.InspMinMargin2 = minMargin2;
                    oldShapeShiftValue.InspMaxMargin2 = maxMargin2;
                    oldShapeShiftValue.MinDefectSize2 = minDefectSize2;
                    oldShapeShiftValue.MaxDefectSize2 = maxDefectSize2;

                    oldShapeShiftValue.ShapeShiftType = m_nShapeShiftType;
                }

                if (m_previewValue == null)
                    m_previewValue = new ShapeShiftProperty();
                m_previewValue.CorrectMethod = correctMethod;

                m_previewValue.GroundLowerThresh = groundLowerThresh;
                m_previewValue.GroundUpperThresh = groundUpperThresh;
                m_previewValue.ShapeLowerThresh = shapeLowerThresh;
                m_previewValue.ShapeUpperThresh = shapeUpperThresh;
                m_previewValue.ErosionShapeIter = erosionShapeIter;
                m_previewValue.DilationShapeIter = dilationShapeIter;
                m_previewValue.ShapeShiftMarginX = shapeShiftMarginX;
                m_previewValue.ShapeShiftMarginY = shapeShiftMarginY;
                m_previewValue.ShapeOffsetX = shapeOffsetX;
                m_previewValue.ShapeOffsetY = shapeOffsetY;
                m_previewValue.ErosionTrainIter = erosionTrainIter;
                m_previewValue.DilationTrainIter = dilationTrainIter;

                m_previewValue.IsInspectMaster = isInspectMaster;
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;
                m_previewValue.ApplyAverDiff = applyAverDiff;
                m_previewValue.InspRange = inspRange;
                m_previewValue.AverMinMargin = averMinMargin;
                m_previewValue.AverMaxMargin = averMaxMargin;
                m_previewValue.MinMargin = minMargin;
                m_previewValue.MaxMargin = maxMargin;
                m_previewValue.MinDefectSize = minDefectSize;
                m_previewValue.MaxDefectSize = maxDefectSize;

                m_previewValue.IsInspectSlave = isInspectSlave;
                m_previewValue.ThreshType2 = threshType2;
                m_previewValue.InspLowerThresh2 = lowerThresh2;
                m_previewValue.InspUpperThresh2 = upperThresh2;
                m_previewValue.ApplyAverDiff2 = applyAverDiff2;
                m_previewValue.InspRange2 = inspRange2;
                m_previewValue.AverMinMargin2 = averMinMargin2;
                m_previewValue.AverMaxMargin2 = averMaxMargin2;
                m_previewValue.InspMinMargin2 = minMargin2;
                m_previewValue.InspMaxMargin2 = maxMargin2;
                m_previewValue.MinDefectSize2 = minDefectSize2;
                m_previewValue.MaxDefectSize2 = maxDefectSize2;

                m_previewValue.ShapeShiftType = m_nShapeShiftType;
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
                #region Save Logic.
                int correctMethod = DEFAULT_CORRECT_METHOD;

                #region Check 전체 Mask 하한 / 상한
                int groundLowerThresh = Convert.ToInt32(txtGroundLowerThresh.Text);
                int groundUpperThresh = Convert.ToInt32(txtGroundUpperThresh.Text);
                //if (groundLowerThresh > groundUpperThresh)
                //{
                //    MessageBox.Show("전체 Mask 하한 값은 전체 Mask 상한 값을 초과할 수 없습니다.", "Information");
                //    this.txtGroundLowerThresh.Focus();
                //    return;
                //}
                #endregion

                #region Check 형상 Mask 하한 / 상한
                int shapeLowerThresh = Convert.ToInt32(txtShapeLowerThresh.Text);
                int shapeUpperThresh = Convert.ToInt32(txtShapeUpperThresh.Text);
                //if (shapeLowerThresh > shapeUpperThresh)
                //{
                //    MessageBox.Show("형상 Mask 하한 값은 형상 Mask 상한 값을 초과할 수 없습니다.", "Information");
                //    this.txtShapeLowerThresh.Focus();
                //    return;
                //}
                #endregion

                // 형상 Mask 축소 / 확장
                int erosionShapeIter = Convert.ToInt32(txtErosionShapeIter.Text);
                int dilationShapeIter = Convert.ToInt32(txtDilationShapeIter.Text);

                // 형상 Shift X / Y
                int shapeShiftMarginX = Convert.ToInt32(txtShapeShiftMarginX.Text);
                int shapeShiftMarginY = Convert.ToInt32(txtShapeShiftMarginY.Text);

                // 형상 Offset X / Y
                int shapeOffsetX = Convert.ToInt32(txtShapeOffsetX.Text);
                int shapeOffsetY = Convert.ToInt32(txtShapeOffsetY.Text);

                // Mask 축소 / 확장
                int erosionTrainIter = Convert.ToInt32(txtErosionTrainIter.Text);
                int dilationTrainIter = Convert.ToInt32(txtDilationTrainIter.Text);

                // 형상 영역 검사 적용
                int isInspectMaster = (chkMasterUse.IsChecked == true) ? 1 : 0;

                #region Check 도금 영역 검사 하한 / 상한 검출값
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

                // 도금 영역 검사 평균값 적용 여부
                int applyAverDiff = (chkApplyAverDiff.IsChecked == true) ? 1 : 0;

                // 도금 영역 평균값 최소 / 최대 허용 범위
                int averMinMargin = (applyAverDiff == 1) ? Convert.ToInt32(txtAverMinMargin.Text) : 0;
                int averMaxMargin = (applyAverDiff == 1) ? Convert.ToInt32(txtAverMaxMargin.Text) : 0;

                // 도금 영역 검사 최소 / 최대 허용범위
                int minMargin = Convert.ToInt32(txtMinMargin.Text);
                int maxMargin = Convert.ToInt32(txtMaxMargin.Text);

                #region 도금 영역 검사 검사 조건 범위
                int inspRange = 0;
                if (this.chkMinRange.IsChecked == true)
                {
                    inspRange += 1;
                }
                if (this.chkMaxRange.IsChecked == true)
                {
                    inspRange += 2;
                }
                if (this.chkInRange.IsChecked == true)
                {
                    inspRange += 4;
                }
                #endregion

                // 도금 영역 최소 검출 사이즈
                int minDefectSize = Convert.ToInt32(txtMinDefectSize.Text);
                int maxDefectSize = Convert.ToInt32(txtMaxDefectSize.Text);

                //////////////////////////////////////////////////////////////////////////////////////////////////////

                // 표면 영역 검사 적용
                int isInspectSlave = (chkSlaveUse.IsChecked == true) ? 1 : 0;

                #region Check 표면 영역 검사 하한 / 상한 검출값
                int threshType2 = 0;
                int lowerThresh2 = -1;
                int upperThresh2 = -1;
                if (radThresholdUpper2.IsChecked == true) // 임계 이상
                {
                    threshType2 = 0;
                    lowerThresh2 = Convert.ToInt32(txtThreshold2.Text);
                    upperThresh2 = 255;
                }
                else if (radThresholdLower2.IsChecked == true) // 임계 이하
                {
                    threshType2 = 1;
                    lowerThresh2 = 0;
                    upperThresh2 = Convert.ToInt32(txtThreshold2.Text);
                }
                else if (radThresholdRange2.IsChecked == true) // 임계 범위
                {
                    threshType2 = 2;
                    lowerThresh2 = Convert.ToInt32(txtLowerThreshold2.Text);
                    upperThresh2 = Convert.ToInt32(txtUpperThreshold2.Text);

                    //if (lowerThresh2 > upperThresh2)
                    //{
                    //    MessageBox.Show("임계 하한 값은 임계 상한 값을 초과할 수 없습니다.", "Information");
                    //    this.txtLowerThreshold2.Focus();
                    //    return;
                    //}
                }
                #endregion

                // 표면 영역 검사 평균값 적용 여부
                int applyAverDiff2 = (chkApplyAverDiff2.IsChecked == true) ? 1 : 0;

                // 표면 영역 평균값 최소 / 최대 허용 범위
                int averMinMargin2 = (applyAverDiff2 == 1) ? Convert.ToInt32(txtAverMinMargin2.Text) : 0;
                int averMaxMargin2 = (applyAverDiff2 == 1) ? Convert.ToInt32(txtAverMaxMargin2.Text) : 0;

                #region 표면 영역 검사 검사 조건 범위
                int inspRange2 = 0;
                if (this.chkMinRange2.IsChecked == true)
                {
                    inspRange2 += 1;
                }
                if (this.chkMaxRange2.IsChecked == true)
                {
                    inspRange2 += 2;
                }
                if (this.chkInRange2.IsChecked == true)
                {
                    inspRange2 += 4;
                }
                #endregion

                // 표면 영역 검사 최소 / 최대 허용범위
                int minMargin2 = Convert.ToInt32(txtMinMargin2.Text);
                int maxMargin2 = Convert.ToInt32(txtMaxMargin2.Text);

                // 표면 영역 최소 검출 사이즈
                int minDefectSize2 = Convert.ToInt32(txtMinDefectSize2.Text);
                int maxDefectSize2 = Convert.ToInt32(txtMaxDefectSize2.Text);
                #endregion

                ShapeShiftProperty oldShapeShiftValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldShapeShiftValue = element.InspectionAlgorithm as ShapeShiftProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldShapeShiftValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    ShapeShiftProperty shapeShiftValue = new ShapeShiftProperty();

                    shapeShiftValue.CorrectMethod = correctMethod;

                    shapeShiftValue.GroundLowerThresh = groundLowerThresh;
                    shapeShiftValue.GroundUpperThresh = groundUpperThresh;
                    shapeShiftValue.ShapeLowerThresh = shapeLowerThresh;
                    shapeShiftValue.ShapeUpperThresh = shapeUpperThresh;
                    shapeShiftValue.ErosionShapeIter = erosionShapeIter;
                    shapeShiftValue.DilationShapeIter = dilationShapeIter;
                    shapeShiftValue.ShapeShiftMarginX = shapeShiftMarginX;
                    shapeShiftValue.ShapeShiftMarginY = shapeShiftMarginY;
                    shapeShiftValue.ShapeOffsetX = shapeOffsetX;
                    shapeShiftValue.ShapeOffsetY = shapeOffsetY;
                    shapeShiftValue.ErosionTrainIter = erosionTrainIter;
                    shapeShiftValue.DilationTrainIter = dilationTrainIter;

                    shapeShiftValue.IsInspectMaster = isInspectMaster;
                    shapeShiftValue.ThreshType = threshType;
                    shapeShiftValue.LowerThresh = lowerThresh;
                    shapeShiftValue.UpperThresh = upperThresh;
                    shapeShiftValue.ApplyAverDiff = applyAverDiff;
                    shapeShiftValue.InspRange = inspRange;
                    shapeShiftValue.AverMinMargin = averMinMargin;
                    shapeShiftValue.AverMaxMargin = averMaxMargin;
                    shapeShiftValue.MinMargin = minMargin;
                    shapeShiftValue.MaxMargin = maxMargin;
                    shapeShiftValue.MinDefectSize = minDefectSize;
                    shapeShiftValue.MaxDefectSize = maxDefectSize;

                    shapeShiftValue.IsInspectSlave = isInspectSlave;
                    shapeShiftValue.ThreshType2 = threshType2;
                    shapeShiftValue.InspLowerThresh2 = lowerThresh2;
                    shapeShiftValue.InspUpperThresh2 = upperThresh2;
                    shapeShiftValue.ApplyAverDiff2 = applyAverDiff2;
                    shapeShiftValue.InspRange2 = inspRange2;
                    shapeShiftValue.AverMinMargin2 = averMinMargin2;
                    shapeShiftValue.AverMaxMargin2 = averMaxMargin2;
                    shapeShiftValue.InspMinMargin2 = minMargin2;
                    shapeShiftValue.InspMaxMargin2 = maxMargin2;
                    shapeShiftValue.MinDefectSize2 = minDefectSize2;
                    shapeShiftValue.MaxDefectSize2 = maxDefectSize2;

                    shapeShiftValue.ShapeShiftType = m_nShapeShiftType;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = shapeShiftValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldShapeShiftValue.CorrectMethod = correctMethod;

                    oldShapeShiftValue.GroundLowerThresh = groundLowerThresh;
                    oldShapeShiftValue.GroundUpperThresh = groundUpperThresh;
                    oldShapeShiftValue.ShapeLowerThresh = shapeLowerThresh;
                    oldShapeShiftValue.ShapeUpperThresh = shapeUpperThresh;
                    oldShapeShiftValue.ErosionShapeIter = erosionShapeIter;
                    oldShapeShiftValue.DilationShapeIter = dilationShapeIter;
                    oldShapeShiftValue.ShapeShiftMarginX = shapeShiftMarginX;
                    oldShapeShiftValue.ShapeShiftMarginY = shapeShiftMarginY;
                    oldShapeShiftValue.ShapeOffsetX = shapeOffsetX;
                    oldShapeShiftValue.ShapeOffsetY = shapeOffsetY;
                    oldShapeShiftValue.ErosionTrainIter = erosionTrainIter;
                    oldShapeShiftValue.DilationTrainIter = dilationTrainIter;

                    oldShapeShiftValue.IsInspectMaster = isInspectMaster;
                    oldShapeShiftValue.ThreshType = threshType;
                    oldShapeShiftValue.LowerThresh = lowerThresh;
                    oldShapeShiftValue.UpperThresh = upperThresh;
                    oldShapeShiftValue.ApplyAverDiff = applyAverDiff;
                    oldShapeShiftValue.InspRange = inspRange;
                    oldShapeShiftValue.AverMinMargin = averMinMargin;
                    oldShapeShiftValue.AverMaxMargin = averMaxMargin;
                    oldShapeShiftValue.MinMargin = minMargin;
                    oldShapeShiftValue.MaxMargin = maxMargin;
                    oldShapeShiftValue.MinDefectSize = minDefectSize;
                    oldShapeShiftValue.MaxDefectSize = maxDefectSize;

                    oldShapeShiftValue.IsInspectSlave = isInspectSlave;
                    oldShapeShiftValue.ThreshType2 = threshType2;
                    oldShapeShiftValue.InspLowerThresh2 = lowerThresh2;
                    oldShapeShiftValue.InspUpperThresh2 = upperThresh2;
                    oldShapeShiftValue.ApplyAverDiff2 = applyAverDiff2;
                    oldShapeShiftValue.InspRange2 = inspRange2;
                    oldShapeShiftValue.AverMinMargin2 = averMinMargin2;
                    oldShapeShiftValue.AverMaxMargin2 = averMaxMargin2;
                    oldShapeShiftValue.InspMinMargin2 = minMargin2;
                    oldShapeShiftValue.InspMaxMargin2 = maxMargin2;
                    oldShapeShiftValue.MinDefectSize2 = minDefectSize2;
                    oldShapeShiftValue.MaxDefectSize2 = maxDefectSize2;

                    oldShapeShiftValue.ShapeShiftType = m_nShapeShiftType;
                }

                if (m_previewValue == null)
                    m_previewValue = new ShapeShiftProperty();
                m_previewValue.CorrectMethod = correctMethod;

                m_previewValue.GroundLowerThresh = groundLowerThresh;
                m_previewValue.GroundUpperThresh = groundUpperThresh;
                m_previewValue.ShapeLowerThresh = shapeLowerThresh;
                m_previewValue.ShapeUpperThresh = shapeUpperThresh;
                m_previewValue.ErosionShapeIter = erosionShapeIter;
                m_previewValue.DilationShapeIter = dilationShapeIter;
                m_previewValue.ShapeShiftMarginX = shapeShiftMarginX;
                m_previewValue.ShapeShiftMarginY = shapeShiftMarginY;
                m_previewValue.ShapeOffsetX = shapeOffsetX;
                m_previewValue.ShapeOffsetY = shapeOffsetY;
                m_previewValue.ErosionTrainIter = erosionTrainIter;
                m_previewValue.DilationTrainIter = dilationTrainIter;

                m_previewValue.IsInspectMaster = isInspectMaster;
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;
                m_previewValue.ApplyAverDiff = applyAverDiff;
                m_previewValue.InspRange = inspRange;
                m_previewValue.AverMinMargin = averMinMargin;
                m_previewValue.AverMaxMargin = averMaxMargin;
                m_previewValue.MinMargin = minMargin;
                m_previewValue.MaxMargin = maxMargin;
                m_previewValue.MinDefectSize = minDefectSize;
                m_previewValue.MaxDefectSize = maxDefectSize;

                m_previewValue.IsInspectSlave = isInspectSlave;
                m_previewValue.ThreshType2 = threshType2;
                m_previewValue.InspLowerThresh2 = lowerThresh2;
                m_previewValue.InspUpperThresh2 = upperThresh2;
                m_previewValue.ApplyAverDiff2 = applyAverDiff2;
                m_previewValue.InspRange2 = inspRange2;
                m_previewValue.AverMinMargin2 = averMinMargin2;
                m_previewValue.AverMaxMargin2 = averMaxMargin2;
                m_previewValue.InspMinMargin2 = minMargin2;
                m_previewValue.InspMaxMargin2 = maxMargin2;
                m_previewValue.MinDefectSize2 = minDefectSize2;
                m_previewValue.MaxDefectSize2 = maxDefectSize2;

                m_previewValue.ShapeShiftType = m_nShapeShiftType;
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
                // Upper pannel.
                this.txtGroundLowerThresh.Text = m_previewValue.GroundLowerThresh.ToString();
                this.txtGroundUpperThresh.Text = m_previewValue.GroundUpperThresh.ToString();
                this.txtShapeLowerThresh.Text = m_previewValue.ShapeLowerThresh.ToString();
                this.txtShapeUpperThresh.Text = m_previewValue.ShapeUpperThresh.ToString();
                this.txtErosionShapeIter.Text = m_previewValue.ErosionShapeIter.ToString();
                this.txtDilationShapeIter.Text = m_previewValue.DilationShapeIter.ToString();
                this.txtShapeShiftMarginX.Text = m_previewValue.ShapeShiftMarginX.ToString();
                this.txtShapeShiftMarginY.Text = m_previewValue.ShapeShiftMarginY.ToString();
                this.txtShapeOffsetX.Text = m_previewValue.ShapeOffsetX.ToString();
                this.txtShapeOffsetY.Text = m_previewValue.ShapeOffsetY.ToString();
                this.txtErosionTrainIter.Text = m_previewValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = m_previewValue.DilationTrainIter.ToString();

                // 형상 영역 검사
                this.chkMasterUse.IsChecked = (m_previewValue.IsInspectMaster == 1) ? true : false;
                #region Threshold.
                if (m_previewValue.ThreshType == 0)
                {
                    this.radThresholdUpper.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = false;
                    this.txtThreshold.Text = m_previewValue.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = m_previewValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = m_previewValue.UpperThresh.ToString();
                    SetPlatingRadioState1();
                }
                else if (m_previewValue.ThreshType == 1)
                {
                    this.radThresholdLower.IsChecked = true;
                    this.chkMinRange.IsChecked = false;
                    this.chkMaxRange.IsChecked = true;
                    this.txtThreshold.Text = m_previewValue.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = m_previewValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = m_previewValue.UpperThresh.ToString();
                    SetPlatingRadioState1();
                }
                else
                {
                    this.radThresholdRange.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = true;
                    this.txtLowerThreshold.Text = m_previewValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = m_previewValue.UpperThresh.ToString();
                    this.txtThreshold.Text = m_previewValue.LowerThresh.ToString();
                    SetPlatingRadioState2();
                }
                #endregion
                this.chkApplyAverDiff.IsChecked = (m_previewValue.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = m_previewValue.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = m_previewValue.AverMaxMargin.ToString();
                this.SetPlatingCheckBoxState(m_previewValue.InspRange);
                this.txtMinMargin.Text = m_previewValue.MinMargin.ToString();
                this.txtMaxMargin.Text = m_previewValue.MaxMargin.ToString();
                this.txtMinDefectSize.Text = m_previewValue.MinDefectSize.ToString();
                this.txtMaxDefectSize.Text = m_previewValue.MaxDefectSize.ToString();

                // 표면 영역 검사
                this.chkSlaveUse.IsChecked = (m_previewValue.IsInspectSlave == 1) ? true : false;
                #region Threshold.
                if (m_previewValue.ThreshType2 == 0)
                {
                    this.radThresholdUpper2.IsChecked = true;
                    this.chkMinRange2.IsChecked = true;
                    this.chkMaxRange2.IsChecked = false;
                    this.txtThreshold2.Text = m_previewValue.InspLowerThresh2.ToString();
                    this.txtLowerThreshold2.Text = m_previewValue.InspLowerThresh2.ToString();
                    this.txtUpperThreshold2.Text = m_previewValue.InspUpperThresh2.ToString();
                    SetSurfaceRadioState1();
                }
                else if (m_previewValue.ThreshType2 == 1)
                {
                    this.radThresholdLower2.IsChecked = true;
                    this.chkMinRange2.IsChecked = false;
                    this.chkMaxRange2.IsChecked = true;
                    this.txtThreshold2.Text = m_previewValue.InspUpperThresh2.ToString();
                    this.txtLowerThreshold2.Text = m_previewValue.InspLowerThresh2.ToString();
                    this.txtUpperThreshold2.Text = m_previewValue.InspUpperThresh2.ToString();
                    SetSurfaceRadioState1();
                }
                else
                {
                    this.radThresholdRange2.IsChecked = true;
                    this.chkMinRange2.IsChecked = true;
                    this.chkMaxRange2.IsChecked = true;
                    this.txtLowerThreshold2.Text = m_previewValue.InspLowerThresh2.ToString();
                    this.txtUpperThreshold2.Text = m_previewValue.InspUpperThresh2.ToString();
                    this.txtThreshold2.Text = m_previewValue.InspLowerThresh2.ToString();
                    SetSurfaceRadioState2();
                }
                #endregion
                this.chkApplyAverDiff2.IsChecked = (m_previewValue.ApplyAverDiff2 == 1) ? true : false;
                this.txtAverMinMargin2.Text = m_previewValue.AverMinMargin2.ToString();
                this.txtAverMaxMargin2.Text = m_previewValue.AverMaxMargin2.ToString();
                this.SetSurfaceCheckBoxState(m_previewValue.InspRange2);
                this.txtMinMargin2.Text = m_previewValue.InspMinMargin2.ToString();
                this.txtMaxMargin2.Text = m_previewValue.InspMaxMargin2.ToString();
                this.txtMinDefectSize2.Text = m_previewValue.MinDefectSize2.ToString();
                this.txtMaxDefectSize2.Text = m_previewValue.MaxDefectSize2.ToString();

                this.m_nShapeShiftType = m_previewValue.ShapeShiftType;
            }
        }

        // 검사 설정 기본 값 표시.
        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeTape)
            {
                if (!TapeDefaultValue.DefaultValueLoaded)
                {
                    TapeDefaultValue.DefaultValueLoaded = true;
                    TapeDefaultValue.LoadDefaultValue();
                }
                // Upper panel.
                this.txtGroundLowerThresh.Text = TapeDefaultValue.GroundLowerThresh.ToString();
                this.txtGroundUpperThresh.Text = TapeDefaultValue.GroundUpperThresh.ToString();
                this.txtShapeLowerThresh.Text = TapeDefaultValue.ShapeLowerThresh.ToString();
                this.txtShapeUpperThresh.Text = TapeDefaultValue.ShapeUpperThresh.ToString();
                this.txtErosionShapeIter.Text = TapeDefaultValue.ErosionShapeIter.ToString();
                this.txtDilationShapeIter.Text = TapeDefaultValue.DilationShapeIter.ToString();
                this.txtShapeShiftMarginX.Text = TapeDefaultValue.ShapeShiftMarginX.ToString();
                this.txtShapeShiftMarginY.Text = TapeDefaultValue.ShapeShiftMarginY.ToString();
                this.txtErosionTrainIter.Text = TapeDefaultValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = TapeDefaultValue.DilationTrainIter.ToString();
                this.txtShapeOffsetX.Text = TapeDefaultValue.ShapeOffsetX.ToString();
                this.txtShapeOffsetY.Text = TapeDefaultValue.ShapeOffsetY.ToString();

                // 형상 영역 검사
                this.chkMasterUse.IsChecked = (TapeDefaultValue.IsInspectMaster == 1) ? true : false;
                #region Threshold.
                if (TapeDefaultValue.ThreshType == 0)
                {
                    this.radThresholdUpper.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = false;
                    this.txtThreshold.Text = TapeDefaultValue.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = TapeDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = TapeDefaultValue.UpperThresh.ToString();
                    SetPlatingRadioState1();
                }
                else if (TapeDefaultValue.ThreshType == 1)
                {
                    this.radThresholdLower.IsChecked = true;
                    this.chkMinRange.IsChecked = false;
                    this.chkMaxRange.IsChecked = true;
                    this.txtThreshold.Text = TapeDefaultValue.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = TapeDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = TapeDefaultValue.UpperThresh.ToString();
                    SetPlatingRadioState1();
                }
                else
                {
                    this.radThresholdRange.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = true;
                    this.txtLowerThreshold.Text = TapeDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = TapeDefaultValue.UpperThresh.ToString();
                    this.txtThreshold.Text = TapeDefaultValue.LowerThresh.ToString();
                    SetPlatingRadioState2();
                }
                #endregion
                this.chkApplyAverDiff.IsChecked = (TapeDefaultValue.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = (TapeDefaultValue.ApplyAverDiff == 1) ? TapeDefaultValue.AverMinMargin.ToString() : "0";
                this.txtAverMaxMargin.Text = (TapeDefaultValue.ApplyAverDiff == 1) ? TapeDefaultValue.AverMaxMargin.ToString() : "0";
                this.SetPlatingCheckBoxState(TapeDefaultValue.InspRange);
                this.txtMinMargin.Text = TapeDefaultValue.MinMargin.ToString();
                this.txtMaxMargin.Text = TapeDefaultValue.MaxMargin.ToString();
                this.txtMinDefectSize.Text = TapeDefaultValue.MinDefectSize.ToString();
                this.txtMaxDefectSize.Text = TapeDefaultValue.MaxDefectSize.ToString();

                // 표면 영역 검사
                this.chkSlaveUse.IsChecked = (TapeDefaultValue.IsInspectSlave == 1) ? true : false;
                #region Threshold.
                if (TapeDefaultValue.ThreshType2 == 0)
                {
                    this.radThresholdUpper2.IsChecked = true;
                    this.chkMinRange2.IsChecked = true;
                    this.chkMaxRange2.IsChecked = false;
                    this.txtThreshold2.Text = TapeDefaultValue.InspLowerThresh2.ToString();
                    this.txtLowerThreshold2.Text = TapeDefaultValue.InspLowerThresh2.ToString();
                    this.txtUpperThreshold2.Text = TapeDefaultValue.InspUpperThresh2.ToString();
                    SetSurfaceRadioState1();
                }
                else if (TapeDefaultValue.ThreshType2 == 1)
                {
                    this.radThresholdLower2.IsChecked = true;
                    this.chkMinRange2.IsChecked = false;
                    this.chkMaxRange2.IsChecked = true;
                    this.txtThreshold2.Text = TapeDefaultValue.InspUpperThresh2.ToString();
                    this.txtLowerThreshold2.Text = TapeDefaultValue.InspLowerThresh2.ToString();
                    this.txtUpperThreshold2.Text = TapeDefaultValue.InspUpperThresh2.ToString();
                    SetSurfaceRadioState1();
                }
                else
                {
                    this.radThresholdRange2.IsChecked = true;
                    this.chkMinRange2.IsChecked = true;
                    this.chkMaxRange2.IsChecked = true;
                    this.txtLowerThreshold2.Text = TapeDefaultValue.InspLowerThresh2.ToString();
                    this.txtUpperThreshold2.Text = TapeDefaultValue.InspUpperThresh2.ToString();
                    this.txtThreshold2.Text = TapeDefaultValue.InspLowerThresh2.ToString();
                    SetSurfaceRadioState2();
                }
                #endregion
                this.chkApplyAverDiff2.IsChecked = (TapeDefaultValue.ApplyAverDiff2 == 1) ? true : false;
                this.txtAverMinMargin2.Text = (TapeDefaultValue.ApplyAverDiff2 == 1) ? TapeDefaultValue.AverMinMargin2.ToString() : "0";
                this.txtAverMaxMargin2.Text = (TapeDefaultValue.ApplyAverDiff2 == 1) ? TapeDefaultValue.AverMaxMargin2.ToString() : "0";
                this.SetSurfaceCheckBoxState(TapeDefaultValue.InspRange2);
                this.txtMinMargin2.Text = TapeDefaultValue.InspMinMargin2.ToString();
                this.txtMaxMargin2.Text = TapeDefaultValue.InspMaxMargin2.ToString();
                this.txtMinDefectSize2.Text = TapeDefaultValue.MinDefectSize2.ToString();
                this.txtMaxDefectSize2.Text = TapeDefaultValue.MaxDefectSize2.ToString();

                this.m_nShapeShiftType = TapeDefaultValue.ShapeShiftType;
            }
            else if (m_enumInspectType == eVisInspectType.eInspTypePlate)
            {
                if (!PlateDefaultValue.DefaultValueLoaded)
                {
                    PlateDefaultValue.DefaultValueLoaded = true;
                    PlateDefaultValue.LoadDefaultValue();
                }
                // Upper panel.
                this.txtGroundLowerThresh.Text = PlateDefaultValue.GroundLowerThresh.ToString();
                this.txtGroundUpperThresh.Text = PlateDefaultValue.GroundUpperThresh.ToString();
                this.txtShapeLowerThresh.Text = PlateDefaultValue.ShapeLowerThresh.ToString();
                this.txtShapeUpperThresh.Text = PlateDefaultValue.ShapeUpperThresh.ToString();
                this.txtErosionShapeIter.Text = PlateDefaultValue.ErosionShapeIter.ToString();
                this.txtDilationShapeIter.Text = PlateDefaultValue.DilationShapeIter.ToString();
                this.txtShapeShiftMarginX.Text = PlateDefaultValue.ShapeShiftMarginX.ToString();
                this.txtShapeShiftMarginY.Text = PlateDefaultValue.ShapeShiftMarginY.ToString();
                this.txtErosionTrainIter.Text = PlateDefaultValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = PlateDefaultValue.DilationTrainIter.ToString();
                this.txtShapeOffsetX.Text = PlateDefaultValue.ShapeOffsetX.ToString();
                this.txtShapeOffsetY.Text = PlateDefaultValue.ShapeOffsetY.ToString();

                // 형상 영역 검사
                this.chkMasterUse.IsChecked = (PlateDefaultValue.IsInspectMaster == 1) ? true : false;
                #region Threshold.
                if (PlateDefaultValue.ThreshType == 0)
                {
                    this.radThresholdUpper.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = false;
                    this.txtThreshold.Text = PlateDefaultValue.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = PlateDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = PlateDefaultValue.UpperThresh.ToString();
                    SetPlatingRadioState1();
                }
                else if (PlateDefaultValue.ThreshType == 1)
                {
                    this.radThresholdLower.IsChecked = true;
                    this.chkMinRange.IsChecked = false;
                    this.chkMaxRange.IsChecked = true;
                    this.txtThreshold.Text = PlateDefaultValue.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = PlateDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = PlateDefaultValue.UpperThresh.ToString();
                    SetPlatingRadioState1();
                }
                else
                {
                    this.radThresholdRange.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = true;
                    this.txtLowerThreshold.Text = PlateDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = PlateDefaultValue.UpperThresh.ToString();
                    this.txtThreshold.Text = PlateDefaultValue.LowerThresh.ToString();
                    SetPlatingRadioState2();
                }
                #endregion
                this.chkApplyAverDiff.IsChecked = (PlateDefaultValue.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = (PlateDefaultValue.ApplyAverDiff == 1) ? PlateDefaultValue.AverMinMargin.ToString() : "0";
                this.txtAverMaxMargin.Text = (PlateDefaultValue.ApplyAverDiff == 1) ? PlateDefaultValue.AverMaxMargin.ToString() : "0";
                this.SetPlatingCheckBoxState(PlateDefaultValue.InspRange);
                this.txtMinMargin.Text = PlateDefaultValue.MinMargin.ToString();
                this.txtMaxMargin.Text = PlateDefaultValue.MaxMargin.ToString();
                this.txtMinDefectSize.Text = PlateDefaultValue.MinDefectSize.ToString();
                this.txtMaxDefectSize.Text = PlateDefaultValue.MaxDefectSize.ToString();

                // 표면 영역 검사
                this.chkSlaveUse.IsChecked = (PlateDefaultValue.IsInspectSlave == 1) ? true : false;
                #region Threshold.
                if (PlateDefaultValue.ThreshType2 == 0)
                {
                    this.radThresholdUpper2.IsChecked = true;
                    this.chkMinRange2.IsChecked = true;
                    this.chkMaxRange2.IsChecked = false;
                    this.txtThreshold2.Text = PlateDefaultValue.InspLowerThresh2.ToString();
                    this.txtLowerThreshold2.Text = PlateDefaultValue.InspLowerThresh2.ToString();
                    this.txtUpperThreshold2.Text = PlateDefaultValue.InspUpperThresh2.ToString();
                    SetSurfaceRadioState1();
                }
                else if (PlateDefaultValue.ThreshType2 == 1)
                {
                    this.radThresholdLower2.IsChecked = true;
                    this.chkMinRange2.IsChecked = false;
                    this.chkMaxRange2.IsChecked = true;
                    this.txtThreshold2.Text = PlateDefaultValue.InspUpperThresh2.ToString();
                    this.txtLowerThreshold2.Text = PlateDefaultValue.InspLowerThresh2.ToString();
                    this.txtUpperThreshold2.Text = PlateDefaultValue.InspUpperThresh2.ToString();
                    SetSurfaceRadioState1();
                }
                else
                {
                    this.radThresholdRange2.IsChecked = true;
                    this.chkMinRange2.IsChecked = true;
                    this.chkMaxRange2.IsChecked = true;
                    this.txtLowerThreshold2.Text = PlateDefaultValue.InspLowerThresh2.ToString();
                    this.txtUpperThreshold2.Text = PlateDefaultValue.InspUpperThresh2.ToString();
                    this.txtThreshold2.Text = PlateDefaultValue.InspLowerThresh2.ToString();
                    SetSurfaceRadioState2();
                }
                #endregion
                this.chkApplyAverDiff2.IsChecked = (PlateDefaultValue.ApplyAverDiff2 == 1) ? true : false;
                this.txtAverMinMargin2.Text = (PlateDefaultValue.ApplyAverDiff2 == 1) ? PlateDefaultValue.AverMinMargin2.ToString() : "0";
                this.txtAverMaxMargin2.Text = (PlateDefaultValue.ApplyAverDiff2 == 1) ? PlateDefaultValue.AverMaxMargin2.ToString() : "0";
                this.SetSurfaceCheckBoxState(PlateDefaultValue.InspRange2);
                this.txtMinMargin2.Text = PlateDefaultValue.InspMinMargin2.ToString();
                this.txtMaxMargin2.Text = PlateDefaultValue.InspMaxMargin2.ToString();
                this.txtMinDefectSize2.Text = PlateDefaultValue.MinDefectSize2.ToString();
                this.txtMaxDefectSize2.Text = PlateDefaultValue.MaxDefectSize2.ToString();

                this.m_nShapeShiftType = PlateDefaultValue.ShapeShiftType;
            }
        }

        // 검사 설정 표시.
        public void Display(InspectionItem settingValue, int MarginX, int MatginY)
        {
            ShapeShiftProperty shapeShiftProperty = settingValue.InspectionAlgorithm as ShapeShiftProperty;
            if (shapeShiftProperty != null)
            {
                // Upper panel.
                this.txtGroundLowerThresh.Text = shapeShiftProperty.GroundLowerThresh.ToString();
                this.txtGroundUpperThresh.Text = shapeShiftProperty.GroundUpperThresh.ToString();
                this.txtShapeLowerThresh.Text = shapeShiftProperty.ShapeLowerThresh.ToString();
                this.txtShapeUpperThresh.Text = shapeShiftProperty.ShapeUpperThresh.ToString();
                this.txtErosionShapeIter.Text = shapeShiftProperty.ErosionShapeIter.ToString();
                this.txtDilationShapeIter.Text = shapeShiftProperty.DilationShapeIter.ToString();
                this.txtShapeShiftMarginX.Text = shapeShiftProperty.ShapeShiftMarginX.ToString();
                this.txtShapeShiftMarginY.Text = shapeShiftProperty.ShapeShiftMarginY.ToString();
                this.txtShapeOffsetX.Text = shapeShiftProperty.ShapeOffsetX.ToString();
                this.txtShapeOffsetY.Text = shapeShiftProperty.ShapeOffsetY.ToString();
                this.txtErosionTrainIter.Text = shapeShiftProperty.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = shapeShiftProperty.DilationTrainIter.ToString();

                // 형상 영역 검사
                this.chkMasterUse.IsChecked = (shapeShiftProperty.IsInspectMaster == 1) ? true : false;
                #region Threshold.
                if (shapeShiftProperty.ThreshType == 0)
                {
                    this.radThresholdUpper.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = false;
                    this.txtThreshold.Text = shapeShiftProperty.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = shapeShiftProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = shapeShiftProperty.UpperThresh.ToString();
                    SetPlatingRadioState1();
                }
                else if (shapeShiftProperty.ThreshType == 1)
                {
                    this.radThresholdLower.IsChecked = true;
                    this.chkMinRange.IsChecked = false;
                    this.chkMaxRange.IsChecked = true;
                    this.txtThreshold.Text = shapeShiftProperty.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = shapeShiftProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = shapeShiftProperty.UpperThresh.ToString();
                    SetPlatingRadioState1();
                }
                else
                {
                    this.radThresholdRange.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = true;
                    this.txtLowerThreshold.Text = shapeShiftProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = shapeShiftProperty.UpperThresh.ToString();
                    this.txtThreshold.Text = shapeShiftProperty.LowerThresh.ToString();
                    SetPlatingRadioState2();
                }
                #endregion
                this.chkApplyAverDiff.IsChecked = (shapeShiftProperty.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = shapeShiftProperty.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = shapeShiftProperty.AverMaxMargin.ToString();
                this.SetPlatingCheckBoxState(shapeShiftProperty.InspRange);
                if (shapeShiftProperty.ApplyAverDiff == 0)
                {
                    shapeShiftProperty.AverMinMargin = 0;
                    shapeShiftProperty.AverMaxMargin = 0;
                }
                this.txtMinMargin.Text = shapeShiftProperty.MinMargin.ToString();
                this.txtMaxMargin.Text = shapeShiftProperty.MaxMargin.ToString();
                this.txtMinDefectSize.Text = shapeShiftProperty.MinDefectSize.ToString();
                this.txtMaxDefectSize.Text = shapeShiftProperty.MaxDefectSize.ToString();

                // 표면 영역 검사
                this.chkSlaveUse.IsChecked = (shapeShiftProperty.IsInspectSlave == 1) ? true : false;
                #region Threshold.
                if (shapeShiftProperty.ThreshType2 == 0)
                {
                    this.radThresholdUpper2.IsChecked = true;
                    this.chkMinRange2.IsChecked = true;
                    this.chkMaxRange2.IsChecked = false;
                    this.txtThreshold2.Text = shapeShiftProperty.InspLowerThresh2.ToString();
                    this.txtLowerThreshold2.Text = shapeShiftProperty.InspLowerThresh2.ToString();
                    this.txtUpperThreshold2.Text = shapeShiftProperty.InspUpperThresh2.ToString();
                    SetSurfaceRadioState1();
                }
                else if (shapeShiftProperty.ThreshType2 == 1)
                {
                    this.radThresholdLower2.IsChecked = true;
                    this.chkMinRange2.IsChecked = false;
                    this.chkMaxRange2.IsChecked = true;
                    this.txtThreshold2.Text = shapeShiftProperty.InspUpperThresh2.ToString();
                    this.txtLowerThreshold2.Text = shapeShiftProperty.InspLowerThresh2.ToString();
                    this.txtUpperThreshold2.Text = shapeShiftProperty.InspUpperThresh2.ToString();
                    SetSurfaceRadioState1();
                }
                else
                {
                    this.radThresholdRange2.IsChecked = true;
                    this.chkMinRange2.IsChecked = true;
                    this.chkMaxRange2.IsChecked = true;
                    this.txtLowerThreshold2.Text = shapeShiftProperty.InspLowerThresh2.ToString();
                    this.txtUpperThreshold2.Text = shapeShiftProperty.InspUpperThresh2.ToString();
                    this.txtThreshold2.Text = shapeShiftProperty.InspLowerThresh2.ToString();
                    SetSurfaceRadioState2();
                }
                #endregion
                this.chkApplyAverDiff2.IsChecked = (shapeShiftProperty.ApplyAverDiff2 == 1) ? true : false;
                this.txtAverMinMargin2.Text = shapeShiftProperty.AverMinMargin2.ToString();
                this.txtAverMaxMargin2.Text = shapeShiftProperty.AverMaxMargin2.ToString();
                this.SetSurfaceCheckBoxState(shapeShiftProperty.InspRange2);
                if (shapeShiftProperty.ApplyAverDiff2 == 0)
                {
                    shapeShiftProperty.AverMinMargin2 = 0;
                    shapeShiftProperty.AverMaxMargin2 = 0;
                }
                this.txtMinMargin2.Text = shapeShiftProperty.InspMinMargin2.ToString();
                this.txtMaxMargin2.Text = shapeShiftProperty.InspMaxMargin2.ToString();
                this.txtMinDefectSize2.Text = shapeShiftProperty.MinDefectSize2.ToString();
                this.txtMaxDefectSize2.Text = shapeShiftProperty.MaxDefectSize2.ToString();

                this.m_nShapeShiftType = shapeShiftProperty.ShapeShiftType;
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
