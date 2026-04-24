using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Common.Drawing.InspectionInformation;

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>   Shape shift with center line.  </summary>
    /// <remarks>   suoow2, 2012-09-12. </remarks>
    public partial class ShapeShiftWithCL : UserControl, IInspectionTypeUICommands
    {
        private ShapeShiftWithCLProperty m_previewValue;

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

        private readonly static int DEFAULT_CORRECT_METHOD = 1;
        private readonly static int SHAPE_SHIFT_TYPE = 0;

        public ShapeShiftWithCL()
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

                ////////////////////////////////////////////////////////////////////////////////////////////////

                // 형상 영역 중앙선 검사
                int isInspectCL = (chkCLUse.IsChecked == true) ? 1 : 0;
                int minWidthRatio = Convert.ToInt32(txtMinWidthRatio.Text);
                int minWidthSize = Convert.ToInt32(txtMinWidthSize.Text);
                int maxWidthRatio = Convert.ToInt32(txtMaxWidthRatio.Text);
                int maxWidthSize = Convert.ToInt32(txtMaxWidthSize.Text);
                int minHeightSize = Convert.ToInt32(txtMinHeightSize.Text);
                int minNormalRatio = Convert.ToInt32(txtMinNormalRatio.Text);
                int maxNormalRatio = Convert.ToInt32(txtMaxNormalRatio.Text);
                int leadThresh = Convert.ToInt32(txtLeadThresh.Text);

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

                ShapeShiftWithCLProperty oldShapeShiftWithCLValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        // 검사 파라미터 수정으로 간주하고 LineSegment를 삭제한다.
                        graphic.LineSegments = element.LineSegments = null;
                        graphic.BallSegments = element.BallSegments = null;
                        graphic.RefreshDrawing();

                        oldShapeShiftWithCLValue = element.InspectionAlgorithm as ShapeShiftWithCLProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldShapeShiftWithCLValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    ShapeShiftWithCLProperty shapeShiftWithCLValue = new ShapeShiftWithCLProperty();

                    shapeShiftWithCLValue.CorrectMethod = correctMethod;

                    shapeShiftWithCLValue.GroundLowerThresh = groundLowerThresh;
                    shapeShiftWithCLValue.GroundUpperThresh = groundUpperThresh;
                    shapeShiftWithCLValue.ShapeLowerThresh = shapeLowerThresh;
                    shapeShiftWithCLValue.ShapeUpperThresh = shapeUpperThresh;
                    shapeShiftWithCLValue.ErosionShapeIter = erosionShapeIter;
                    shapeShiftWithCLValue.DilationShapeIter = dilationShapeIter;
                    shapeShiftWithCLValue.ShapeShiftMarginX = shapeShiftMarginX;
                    shapeShiftWithCLValue.ShapeShiftMarginY = shapeShiftMarginY;
                    shapeShiftWithCLValue.ShapeOffsetX = shapeOffsetX;
                    shapeShiftWithCLValue.ShapeOffsetY = shapeOffsetY;
                    shapeShiftWithCLValue.ErosionTrainIter = erosionTrainIter;
                    shapeShiftWithCLValue.DilationTrainIter = dilationTrainIter;

                    // 2012-09-12 added.
                    shapeShiftWithCLValue.IsInspectCL = isInspectCL;
                    shapeShiftWithCLValue.MinWidthRatio = minWidthRatio;
                    shapeShiftWithCLValue.MinWidthSize = minWidthSize;
                    shapeShiftWithCLValue.MaxWidthRatio = maxWidthRatio;
                    shapeShiftWithCLValue.MaxWidthSize = maxWidthSize;
                    shapeShiftWithCLValue.MinHeightSize = minHeightSize;
                    shapeShiftWithCLValue.MinNormalRatio = minNormalRatio;
                    shapeShiftWithCLValue.MaxNormalRatio = maxNormalRatio;
                    shapeShiftWithCLValue.LeadThresh = leadThresh;

                    shapeShiftWithCLValue.IsInspectMaster = isInspectMaster;
                    shapeShiftWithCLValue.ThreshType = threshType;
                    shapeShiftWithCLValue.LowerThresh = lowerThresh;
                    shapeShiftWithCLValue.UpperThresh = upperThresh;
                    shapeShiftWithCLValue.ApplyAverDiff = applyAverDiff;
                    shapeShiftWithCLValue.InspRange = inspRange;
                    shapeShiftWithCLValue.AverMinMargin = averMinMargin;
                    shapeShiftWithCLValue.AverMaxMargin = averMaxMargin;
                    shapeShiftWithCLValue.MinMargin = minMargin;
                    shapeShiftWithCLValue.MaxMargin = maxMargin;
                    shapeShiftWithCLValue.MinDefectSize = minDefectSize;
                    shapeShiftWithCLValue.MaxDefectSize = maxDefectSize;

                    shapeShiftWithCLValue.IsInspectSlave = isInspectSlave;
                    shapeShiftWithCLValue.ThreshType2 = threshType2;
                    shapeShiftWithCLValue.InspLowerThresh2 = lowerThresh2;
                    shapeShiftWithCLValue.InspUpperThresh2 = upperThresh2;
                    shapeShiftWithCLValue.ApplyAverDiff2 = applyAverDiff2;
                    shapeShiftWithCLValue.InspRange2 = inspRange2;
                    shapeShiftWithCLValue.AverMinMargin2 = averMinMargin2;
                    shapeShiftWithCLValue.AverMaxMargin2 = averMaxMargin2;
                    shapeShiftWithCLValue.InspMinMargin2 = minMargin2;
                    shapeShiftWithCLValue.InspMaxMargin2 = maxMargin2;
                    shapeShiftWithCLValue.MinDefectSize2 = minDefectSize2;
                    shapeShiftWithCLValue.MaxDefectSize2 = maxDefectSize2;
                    
                    shapeShiftWithCLValue.ShapeShiftType = SHAPE_SHIFT_TYPE;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = shapeShiftWithCLValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldShapeShiftWithCLValue.CorrectMethod = correctMethod;

                    oldShapeShiftWithCLValue.GroundLowerThresh = groundLowerThresh;
                    oldShapeShiftWithCLValue.GroundUpperThresh = groundUpperThresh;
                    oldShapeShiftWithCLValue.ShapeLowerThresh = shapeLowerThresh;
                    oldShapeShiftWithCLValue.ShapeUpperThresh = shapeUpperThresh;
                    oldShapeShiftWithCLValue.ErosionShapeIter = erosionShapeIter;
                    oldShapeShiftWithCLValue.DilationShapeIter = dilationShapeIter;
                    oldShapeShiftWithCLValue.ShapeShiftMarginX = shapeShiftMarginX;
                    oldShapeShiftWithCLValue.ShapeShiftMarginY = shapeShiftMarginY;
                    oldShapeShiftWithCLValue.ShapeOffsetX = shapeOffsetX;
                    oldShapeShiftWithCLValue.ShapeOffsetY = shapeOffsetY;
                    oldShapeShiftWithCLValue.ErosionTrainIter = erosionTrainIter;
                    oldShapeShiftWithCLValue.DilationTrainIter = dilationTrainIter;

                    // 2012-09-12 added.
                    oldShapeShiftWithCLValue.IsInspectCL = isInspectCL;
                    oldShapeShiftWithCLValue.MinWidthRatio = minWidthRatio;
                    oldShapeShiftWithCLValue.MinWidthSize = minWidthSize;
                    oldShapeShiftWithCLValue.MaxWidthRatio = maxWidthRatio;
                    oldShapeShiftWithCLValue.MaxWidthSize = maxWidthSize;
                    oldShapeShiftWithCLValue.MinHeightSize = minHeightSize;
                    oldShapeShiftWithCLValue.MinNormalRatio = minNormalRatio;
                    oldShapeShiftWithCLValue.MaxNormalRatio = maxNormalRatio;
                    oldShapeShiftWithCLValue.LeadThresh = leadThresh;

                    oldShapeShiftWithCLValue.IsInspectMaster = isInspectMaster;
                    oldShapeShiftWithCLValue.ThreshType = threshType;
                    oldShapeShiftWithCLValue.LowerThresh = lowerThresh;
                    oldShapeShiftWithCLValue.UpperThresh = upperThresh;
                    oldShapeShiftWithCLValue.ApplyAverDiff = applyAverDiff;
                    oldShapeShiftWithCLValue.InspRange = inspRange;
                    oldShapeShiftWithCLValue.AverMinMargin = averMinMargin;
                    oldShapeShiftWithCLValue.AverMaxMargin = averMaxMargin;
                    oldShapeShiftWithCLValue.MinMargin = minMargin;
                    oldShapeShiftWithCLValue.MaxMargin = maxMargin;
                    oldShapeShiftWithCLValue.MinDefectSize = minDefectSize;
                    oldShapeShiftWithCLValue.MaxDefectSize = maxDefectSize;

                    oldShapeShiftWithCLValue.IsInspectSlave = isInspectSlave;
                    oldShapeShiftWithCLValue.ThreshType2 = threshType2;
                    oldShapeShiftWithCLValue.InspLowerThresh2 = lowerThresh2;
                    oldShapeShiftWithCLValue.InspUpperThresh2 = upperThresh2;
                    oldShapeShiftWithCLValue.ApplyAverDiff2 = applyAverDiff2;
                    oldShapeShiftWithCLValue.InspRange2 = inspRange2;
                    oldShapeShiftWithCLValue.AverMinMargin2 = averMinMargin2;
                    oldShapeShiftWithCLValue.AverMaxMargin2 = averMaxMargin2;
                    oldShapeShiftWithCLValue.InspMinMargin2 = minMargin2;
                    oldShapeShiftWithCLValue.InspMaxMargin2 = maxMargin2;
                    oldShapeShiftWithCLValue.MinDefectSize2 = minDefectSize2;
                    oldShapeShiftWithCLValue.MaxDefectSize2 = maxDefectSize2;

                    oldShapeShiftWithCLValue.ShapeShiftType = SHAPE_SHIFT_TYPE;
                }

                if (m_previewValue == null)
                    m_previewValue = new ShapeShiftWithCLProperty();
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

                // 2012-09-12 added.
                m_previewValue.IsInspectCL = isInspectCL;
                m_previewValue.MinWidthRatio = minWidthRatio;
                m_previewValue.MinWidthSize = minWidthSize;
                m_previewValue.MaxWidthRatio = maxWidthRatio;
                m_previewValue.MaxWidthSize = maxWidthSize;
                m_previewValue.MinHeightSize = minHeightSize;
                m_previewValue.MinNormalRatio = minNormalRatio;
                m_previewValue.MaxNormalRatio = maxNormalRatio;
                m_previewValue.LeadThresh = leadThresh;     

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

                m_previewValue.ShapeShiftType = SHAPE_SHIFT_TYPE;
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

                ////////////////////////////////////////////////////////////////////////////////////////////////

                // 형상 영역 중앙선 검사
                int isInspectCL = (chkCLUse.IsChecked == true) ? 1 : 0;
                int minWidthRatio = Convert.ToInt32(txtMinWidthRatio.Text);
                int minWidthSize = Convert.ToInt32(txtMinWidthSize.Text);
                int maxWidthRatio = Convert.ToInt32(txtMaxWidthRatio.Text);
                int maxWidthSize = Convert.ToInt32(txtMaxWidthSize.Text);
                int minHeightSize = Convert.ToInt32(txtMinHeightSize.Text);
                int minNormalRatio = Convert.ToInt32(txtMinNormalRatio.Text);
                int maxNormalRatio = Convert.ToInt32(txtMaxNormalRatio.Text);
                int leadThresh = Convert.ToInt32(txtLeadThresh.Text);

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

                ShapeShiftWithCLProperty oldShapeShiftWithCLValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {

                        oldShapeShiftWithCLValue = element.InspectionAlgorithm as ShapeShiftWithCLProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldShapeShiftWithCLValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    ShapeShiftWithCLProperty shapeShiftWithCLValue = new ShapeShiftWithCLProperty();

                    shapeShiftWithCLValue.CorrectMethod = correctMethod;

                    shapeShiftWithCLValue.GroundLowerThresh = groundLowerThresh;
                    shapeShiftWithCLValue.GroundUpperThresh = groundUpperThresh;
                    shapeShiftWithCLValue.ShapeLowerThresh = shapeLowerThresh;
                    shapeShiftWithCLValue.ShapeUpperThresh = shapeUpperThresh;
                    shapeShiftWithCLValue.ErosionShapeIter = erosionShapeIter;
                    shapeShiftWithCLValue.DilationShapeIter = dilationShapeIter;
                    shapeShiftWithCLValue.ShapeShiftMarginX = shapeShiftMarginX;
                    shapeShiftWithCLValue.ShapeShiftMarginY = shapeShiftMarginY;
                    shapeShiftWithCLValue.ShapeOffsetX = shapeOffsetX;
                    shapeShiftWithCLValue.ShapeOffsetY = shapeOffsetY;
                    shapeShiftWithCLValue.ErosionTrainIter = erosionTrainIter;
                    shapeShiftWithCLValue.DilationTrainIter = dilationTrainIter;

                    // 2012-09-12 added.
                    shapeShiftWithCLValue.IsInspectCL = isInspectCL;
                    shapeShiftWithCLValue.MinWidthRatio = minWidthRatio;
                    shapeShiftWithCLValue.MinWidthSize = minWidthSize;
                    shapeShiftWithCLValue.MaxWidthRatio = maxWidthRatio;
                    shapeShiftWithCLValue.MaxWidthSize = maxWidthSize;
                    shapeShiftWithCLValue.MinHeightSize = minHeightSize;
                    shapeShiftWithCLValue.MinNormalRatio = minNormalRatio;
                    shapeShiftWithCLValue.MaxNormalRatio = maxNormalRatio;
                    shapeShiftWithCLValue.LeadThresh = leadThresh;

                    shapeShiftWithCLValue.IsInspectMaster = isInspectMaster;
                    shapeShiftWithCLValue.ThreshType = threshType;
                    shapeShiftWithCLValue.LowerThresh = lowerThresh;
                    shapeShiftWithCLValue.UpperThresh = upperThresh;
                    shapeShiftWithCLValue.ApplyAverDiff = applyAverDiff;
                    shapeShiftWithCLValue.InspRange = inspRange;
                    shapeShiftWithCLValue.AverMinMargin = averMinMargin;
                    shapeShiftWithCLValue.AverMaxMargin = averMaxMargin;
                    shapeShiftWithCLValue.MinMargin = minMargin;
                    shapeShiftWithCLValue.MaxMargin = maxMargin;
                    shapeShiftWithCLValue.MinDefectSize = minDefectSize;
                    shapeShiftWithCLValue.MaxDefectSize = maxDefectSize;

                    shapeShiftWithCLValue.IsInspectSlave = isInspectSlave;
                    shapeShiftWithCLValue.ThreshType2 = threshType2;
                    shapeShiftWithCLValue.InspLowerThresh2 = lowerThresh2;
                    shapeShiftWithCLValue.InspUpperThresh2 = upperThresh2;
                    shapeShiftWithCLValue.ApplyAverDiff2 = applyAverDiff2;
                    shapeShiftWithCLValue.InspRange2 = inspRange2;
                    shapeShiftWithCLValue.AverMinMargin2 = averMinMargin2;
                    shapeShiftWithCLValue.AverMaxMargin2 = averMaxMargin2;
                    shapeShiftWithCLValue.InspMinMargin2 = minMargin2;
                    shapeShiftWithCLValue.InspMaxMargin2 = maxMargin2;
                    shapeShiftWithCLValue.MinDefectSize2 = minDefectSize2;
                    shapeShiftWithCLValue.MaxDefectSize2 = maxDefectSize2;

                    shapeShiftWithCLValue.ShapeShiftType = SHAPE_SHIFT_TYPE;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = shapeShiftWithCLValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldShapeShiftWithCLValue.CorrectMethod = correctMethod;

                    oldShapeShiftWithCLValue.GroundLowerThresh = groundLowerThresh;
                    oldShapeShiftWithCLValue.GroundUpperThresh = groundUpperThresh;
                    oldShapeShiftWithCLValue.ShapeLowerThresh = shapeLowerThresh;
                    oldShapeShiftWithCLValue.ShapeUpperThresh = shapeUpperThresh;
                    oldShapeShiftWithCLValue.ErosionShapeIter = erosionShapeIter;
                    oldShapeShiftWithCLValue.DilationShapeIter = dilationShapeIter;
                    oldShapeShiftWithCLValue.ShapeShiftMarginX = shapeShiftMarginX;
                    oldShapeShiftWithCLValue.ShapeShiftMarginY = shapeShiftMarginY;
                    oldShapeShiftWithCLValue.ShapeOffsetX = shapeOffsetX;
                    oldShapeShiftWithCLValue.ShapeOffsetY = shapeOffsetY;
                    oldShapeShiftWithCLValue.ErosionTrainIter = erosionTrainIter;
                    oldShapeShiftWithCLValue.DilationTrainIter = dilationTrainIter;

                    // 2012-09-12 added.
                    oldShapeShiftWithCLValue.IsInspectCL = isInspectCL;
                    oldShapeShiftWithCLValue.MinWidthRatio = minWidthRatio;
                    oldShapeShiftWithCLValue.MinWidthSize = minWidthSize;
                    oldShapeShiftWithCLValue.MaxWidthRatio = maxWidthRatio;
                    oldShapeShiftWithCLValue.MaxWidthSize = maxWidthSize;
                    oldShapeShiftWithCLValue.MinHeightSize = minHeightSize;
                    oldShapeShiftWithCLValue.MinNormalRatio = minNormalRatio;
                    oldShapeShiftWithCLValue.MaxNormalRatio = maxNormalRatio;
                    oldShapeShiftWithCLValue.LeadThresh = leadThresh;

                    oldShapeShiftWithCLValue.IsInspectMaster = isInspectMaster;
                    oldShapeShiftWithCLValue.ThreshType = threshType;
                    oldShapeShiftWithCLValue.LowerThresh = lowerThresh;
                    oldShapeShiftWithCLValue.UpperThresh = upperThresh;
                    oldShapeShiftWithCLValue.ApplyAverDiff = applyAverDiff;
                    oldShapeShiftWithCLValue.InspRange = inspRange;
                    oldShapeShiftWithCLValue.AverMinMargin = averMinMargin;
                    oldShapeShiftWithCLValue.AverMaxMargin = averMaxMargin;
                    oldShapeShiftWithCLValue.MinMargin = minMargin;
                    oldShapeShiftWithCLValue.MaxMargin = maxMargin;
                    oldShapeShiftWithCLValue.MinDefectSize = minDefectSize;
                    oldShapeShiftWithCLValue.MaxDefectSize = maxDefectSize;

                    oldShapeShiftWithCLValue.IsInspectSlave = isInspectSlave;
                    oldShapeShiftWithCLValue.ThreshType2 = threshType2;
                    oldShapeShiftWithCLValue.InspLowerThresh2 = lowerThresh2;
                    oldShapeShiftWithCLValue.InspUpperThresh2 = upperThresh2;
                    oldShapeShiftWithCLValue.ApplyAverDiff2 = applyAverDiff2;
                    oldShapeShiftWithCLValue.InspRange2 = inspRange2;
                    oldShapeShiftWithCLValue.AverMinMargin2 = averMinMargin2;
                    oldShapeShiftWithCLValue.AverMaxMargin2 = averMaxMargin2;
                    oldShapeShiftWithCLValue.InspMinMargin2 = minMargin2;
                    oldShapeShiftWithCLValue.InspMaxMargin2 = maxMargin2;
                    oldShapeShiftWithCLValue.MinDefectSize2 = minDefectSize2;
                    oldShapeShiftWithCLValue.MaxDefectSize2 = maxDefectSize2;

                    oldShapeShiftWithCLValue.ShapeShiftType = SHAPE_SHIFT_TYPE;
                }

                if (m_previewValue == null)
                    m_previewValue = new ShapeShiftWithCLProperty();
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

                // 2012-09-12 added.
                m_previewValue.IsInspectCL = isInspectCL;
                m_previewValue.MinWidthRatio = minWidthRatio;
                m_previewValue.MinWidthSize = minWidthSize;
                m_previewValue.MaxWidthRatio = maxWidthRatio;
                m_previewValue.MaxWidthSize = maxWidthSize;
                m_previewValue.MinHeightSize = minHeightSize;
                m_previewValue.MinNormalRatio = minNormalRatio;
                m_previewValue.MaxNormalRatio = maxNormalRatio;
                m_previewValue.LeadThresh = leadThresh;

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

                m_previewValue.ShapeShiftType = SHAPE_SHIFT_TYPE;
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

                // 형상 영역 중앙선 검사 2012-09-12
                this.chkCLUse.IsChecked = (m_previewValue.IsInspectCL == 1) ? true : false;
                this.txtMinWidthRatio.Text = m_previewValue.MinWidthRatio.ToString();
                this.txtMinWidthSize.Text = m_previewValue.MinWidthSize.ToString();
                this.txtMaxWidthRatio.Text = m_previewValue.MaxWidthRatio.ToString();
                this.txtMaxWidthSize.Text = m_previewValue.MaxWidthSize.ToString();
                this.txtMinHeightSize.Text = m_previewValue.MinHeightSize.ToString();
                this.txtMinNormalRatio.Text = m_previewValue.MinNormalRatio.ToString();
                this.txtMaxNormalRatio.Text = m_previewValue.MaxNormalRatio.ToString();
                this.txtLeadThresh.Text = m_previewValue.LeadThresh.ToString();     

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
            }
        }

        // 검사 설정 기본 값 표시.
        public void SetDefaultValue()
        {
            if (!PlateWithCLDefaultValue.DefaultValueLoaded)
            {
                PlateWithCLDefaultValue.DefaultValueLoaded = true;
                PlateWithCLDefaultValue.LoadDefaultValue();
            }
            // Upper panel.
            this.txtGroundLowerThresh.Text = PlateWithCLDefaultValue.GroundLowerThresh.ToString();
            this.txtGroundUpperThresh.Text = PlateWithCLDefaultValue.GroundUpperThresh.ToString();
            this.txtShapeLowerThresh.Text = PlateWithCLDefaultValue.ShapeLowerThresh.ToString();
            this.txtShapeUpperThresh.Text = PlateWithCLDefaultValue.ShapeUpperThresh.ToString();
            this.txtErosionShapeIter.Text = PlateWithCLDefaultValue.ErosionShapeIter.ToString();
            this.txtDilationShapeIter.Text = PlateWithCLDefaultValue.DilationShapeIter.ToString();
            this.txtShapeShiftMarginX.Text = PlateWithCLDefaultValue.ShapeShiftMarginX.ToString();
            this.txtShapeShiftMarginY.Text = PlateWithCLDefaultValue.ShapeShiftMarginY.ToString();
            this.txtErosionTrainIter.Text = PlateWithCLDefaultValue.ErosionTrainIter.ToString();
            this.txtDilationTrainIter.Text = PlateWithCLDefaultValue.DilationTrainIter.ToString();
            this.txtShapeOffsetX.Text = PlateWithCLDefaultValue.ShapeOffsetX.ToString();
            this.txtShapeOffsetY.Text = PlateWithCLDefaultValue.ShapeOffsetY.ToString();

            // 형상 영역 중앙선 검사 2012-09-12
            this.chkCLUse.IsChecked = (PlateWithCLDefaultValue.IsInspectCL == 1) ? true : false;
            this.txtMinWidthRatio.Text = PlateWithCLDefaultValue.MinWidthRatio.ToString();
            this.txtMinWidthSize.Text = PlateWithCLDefaultValue.MinWidthSize.ToString();
            this.txtMaxWidthRatio.Text = PlateWithCLDefaultValue.MaxWidthRatio.ToString();
            this.txtMaxWidthSize.Text = PlateWithCLDefaultValue.MaxWidthSize.ToString();
            this.txtMinHeightSize.Text = PlateWithCLDefaultValue.MinHeightSize.ToString();
            this.txtMinNormalRatio.Text = PlateWithCLDefaultValue.MinNormalRatio.ToString();
            this.txtMaxNormalRatio.Text = PlateWithCLDefaultValue.MaxNormalRatio.ToString();
            this.txtLeadThresh.Text = PlateWithCLDefaultValue.LeadThresh.ToString();  

            // 형상 영역 검사
            this.chkMasterUse.IsChecked = (PlateWithCLDefaultValue.IsInspectMaster == 1) ? true : false;
            #region Threshold.
            if (PlateWithCLDefaultValue.ThreshType == 0)
            {
                this.radThresholdUpper.IsChecked = true;
                this.chkMinRange.IsChecked = true;
                this.chkMaxRange.IsChecked = false;
                this.txtThreshold.Text = PlateWithCLDefaultValue.LowerThresh.ToString();
                this.txtLowerThreshold.Text = PlateWithCLDefaultValue.LowerThresh.ToString();
                this.txtUpperThreshold.Text = PlateWithCLDefaultValue.UpperThresh.ToString();
                SetPlatingRadioState1();
            }
            else if (PlateWithCLDefaultValue.ThreshType == 1)
            {
                this.radThresholdLower.IsChecked = true;
                this.chkMinRange.IsChecked = false;
                this.chkMaxRange.IsChecked = true;
                this.txtThreshold.Text = PlateWithCLDefaultValue.UpperThresh.ToString();
                this.txtLowerThreshold.Text = PlateWithCLDefaultValue.LowerThresh.ToString();
                this.txtUpperThreshold.Text = PlateWithCLDefaultValue.UpperThresh.ToString();
                SetPlatingRadioState1();
            }
            else
            {
                this.radThresholdRange.IsChecked = true;
                this.chkMinRange.IsChecked = true;
                this.chkMaxRange.IsChecked = true;
                this.txtLowerThreshold.Text = PlateWithCLDefaultValue.LowerThresh.ToString();
                this.txtUpperThreshold.Text = PlateWithCLDefaultValue.UpperThresh.ToString();
                this.txtThreshold.Text = PlateWithCLDefaultValue.LowerThresh.ToString();
                SetPlatingRadioState2();
            }
            #endregion
            this.chkApplyAverDiff.IsChecked = (PlateWithCLDefaultValue.ApplyAverDiff == 1) ? true : false;
            this.txtAverMinMargin.Text = (PlateWithCLDefaultValue.ApplyAverDiff == 1) ? PlateWithCLDefaultValue.AverMinMargin.ToString() : "0";
            this.txtAverMaxMargin.Text = (PlateWithCLDefaultValue.ApplyAverDiff == 1) ? PlateWithCLDefaultValue.AverMaxMargin.ToString() : "0";
            this.SetPlatingCheckBoxState(PlateWithCLDefaultValue.InspRange);
            this.txtMinMargin.Text = PlateWithCLDefaultValue.MinMargin.ToString();
            this.txtMaxMargin.Text = PlateWithCLDefaultValue.MaxMargin.ToString();
            this.txtMinDefectSize.Text = PlateWithCLDefaultValue.MinDefectSize.ToString();
            this.txtMaxDefectSize.Text = PlateWithCLDefaultValue.MaxDefectSize.ToString();

            // 표면 영역 검사
            this.chkSlaveUse.IsChecked = (PlateWithCLDefaultValue.IsInspectSlave == 1) ? true : false;
            #region Threshold.
            if (PlateWithCLDefaultValue.ThreshType2 == 0)
            {
                this.radThresholdUpper2.IsChecked = true;
                this.chkMinRange2.IsChecked = true;
                this.chkMaxRange2.IsChecked = false;
                this.txtThreshold2.Text = PlateWithCLDefaultValue.InspLowerThresh2.ToString();
                this.txtLowerThreshold2.Text = PlateWithCLDefaultValue.InspLowerThresh2.ToString();
                this.txtUpperThreshold2.Text = PlateWithCLDefaultValue.InspUpperThresh2.ToString();
                SetSurfaceRadioState1();
            }
            else if (PlateWithCLDefaultValue.ThreshType2 == 1)
            {
                this.radThresholdLower2.IsChecked = true;
                this.chkMinRange2.IsChecked = false;
                this.chkMaxRange2.IsChecked = true;
                this.txtThreshold2.Text = PlateWithCLDefaultValue.InspUpperThresh2.ToString();
                this.txtLowerThreshold2.Text = PlateWithCLDefaultValue.InspLowerThresh2.ToString();
                this.txtUpperThreshold2.Text = PlateWithCLDefaultValue.InspUpperThresh2.ToString();
                SetSurfaceRadioState1();
            }
            else
            {
                this.radThresholdRange2.IsChecked = true;
                this.chkMinRange2.IsChecked = true;
                this.chkMaxRange2.IsChecked = true;
                this.txtLowerThreshold2.Text = PlateWithCLDefaultValue.InspLowerThresh2.ToString();
                this.txtUpperThreshold2.Text = PlateWithCLDefaultValue.InspUpperThresh2.ToString();
                this.txtThreshold2.Text = PlateWithCLDefaultValue.InspLowerThresh2.ToString();
                SetSurfaceRadioState2();
            }
            #endregion
            this.chkApplyAverDiff2.IsChecked = (PlateWithCLDefaultValue.ApplyAverDiff2 == 1) ? true : false;
            this.txtAverMinMargin2.Text = (PlateWithCLDefaultValue.ApplyAverDiff2 == 1) ? PlateWithCLDefaultValue.AverMinMargin2.ToString() : "0";
            this.txtAverMaxMargin2.Text = (PlateWithCLDefaultValue.ApplyAverDiff2 == 1) ? PlateWithCLDefaultValue.AverMaxMargin2.ToString() : "0";
            this.SetSurfaceCheckBoxState(PlateWithCLDefaultValue.InspRange2);
            this.txtMinMargin2.Text = PlateWithCLDefaultValue.InspMinMargin2.ToString();
            this.txtMaxMargin2.Text = PlateWithCLDefaultValue.InspMaxMargin2.ToString();
            this.txtMinDefectSize2.Text = PlateWithCLDefaultValue.MinDefectSize2.ToString();
            this.txtMaxDefectSize2.Text = PlateWithCLDefaultValue.MaxDefectSize2.ToString();
        }

        // 검사 설정 표시.
        public void Display(InspectionItem settingValue, int MarginX, int MatginY)
        {
            ShapeShiftWithCLProperty shapeShiftWithCLProperty = settingValue.InspectionAlgorithm as ShapeShiftWithCLProperty;
            if (shapeShiftWithCLProperty != null)
            {
                // Upper panel.
                this.txtGroundLowerThresh.Text = shapeShiftWithCLProperty.GroundLowerThresh.ToString();
                this.txtGroundUpperThresh.Text = shapeShiftWithCLProperty.GroundUpperThresh.ToString();
                this.txtShapeLowerThresh.Text = shapeShiftWithCLProperty.ShapeLowerThresh.ToString();
                this.txtShapeUpperThresh.Text = shapeShiftWithCLProperty.ShapeUpperThresh.ToString();
                this.txtErosionShapeIter.Text = shapeShiftWithCLProperty.ErosionShapeIter.ToString();
                this.txtDilationShapeIter.Text = shapeShiftWithCLProperty.DilationShapeIter.ToString();
                this.txtShapeShiftMarginX.Text = shapeShiftWithCLProperty.ShapeShiftMarginX.ToString();
                this.txtShapeShiftMarginY.Text = shapeShiftWithCLProperty.ShapeShiftMarginY.ToString();
                this.txtShapeOffsetX.Text = shapeShiftWithCLProperty.ShapeOffsetX.ToString();
                this.txtShapeOffsetY.Text = shapeShiftWithCLProperty.ShapeOffsetY.ToString();
                this.txtErosionTrainIter.Text = shapeShiftWithCLProperty.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = shapeShiftWithCLProperty.DilationTrainIter.ToString();

                // 형상 영역 중앙선 검사 2012-09-12
                this.chkCLUse.IsChecked = (shapeShiftWithCLProperty.IsInspectCL == 1) ? true : false;
                this.txtMinWidthRatio.Text = shapeShiftWithCLProperty.MinWidthRatio.ToString();
                this.txtMinWidthSize.Text = shapeShiftWithCLProperty.MinWidthSize.ToString();
                this.txtMaxWidthRatio.Text = shapeShiftWithCLProperty.MaxWidthRatio.ToString();
                this.txtMaxWidthSize.Text = shapeShiftWithCLProperty.MaxWidthSize.ToString();
                this.txtMinHeightSize.Text = shapeShiftWithCLProperty.MinHeightSize.ToString();
                this.txtMinNormalRatio.Text = shapeShiftWithCLProperty.MinNormalRatio.ToString();
                this.txtMaxNormalRatio.Text = shapeShiftWithCLProperty.MaxNormalRatio.ToString();
                this.txtLeadThresh.Text = shapeShiftWithCLProperty.LeadThresh.ToString();     

                // 형상 영역 검사
                this.chkMasterUse.IsChecked = (shapeShiftWithCLProperty.IsInspectMaster == 1) ? true : false;
                #region Threshold.
                if (shapeShiftWithCLProperty.ThreshType == 0)
                {
                    this.radThresholdUpper.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = false;
                    this.txtThreshold.Text = shapeShiftWithCLProperty.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = shapeShiftWithCLProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = shapeShiftWithCLProperty.UpperThresh.ToString();
                    SetPlatingRadioState1();
                }
                else if (shapeShiftWithCLProperty.ThreshType == 1)
                {
                    this.radThresholdLower.IsChecked = true;
                    this.chkMinRange.IsChecked = false;
                    this.chkMaxRange.IsChecked = true;
                    this.txtThreshold.Text = shapeShiftWithCLProperty.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = shapeShiftWithCLProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = shapeShiftWithCLProperty.UpperThresh.ToString();
                    SetPlatingRadioState1();
                }
                else
                {
                    this.radThresholdRange.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = true;
                    this.txtLowerThreshold.Text = shapeShiftWithCLProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = shapeShiftWithCLProperty.UpperThresh.ToString();
                    this.txtThreshold.Text = shapeShiftWithCLProperty.LowerThresh.ToString();
                    SetPlatingRadioState2();
                }
                #endregion
                this.chkApplyAverDiff.IsChecked = (shapeShiftWithCLProperty.ApplyAverDiff == 1) ? true : false;
                this.txtAverMinMargin.Text = shapeShiftWithCLProperty.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = shapeShiftWithCLProperty.AverMaxMargin.ToString();
                this.SetPlatingCheckBoxState(shapeShiftWithCLProperty.InspRange);
                if (shapeShiftWithCLProperty.ApplyAverDiff == 0)
                {
                    shapeShiftWithCLProperty.AverMinMargin = 0;
                    shapeShiftWithCLProperty.AverMaxMargin = 0;
                }
                this.txtMinMargin.Text = shapeShiftWithCLProperty.MinMargin.ToString();
                this.txtMaxMargin.Text = shapeShiftWithCLProperty.MaxMargin.ToString();
                this.txtMinDefectSize.Text = shapeShiftWithCLProperty.MinDefectSize.ToString();
                this.txtMaxDefectSize.Text = shapeShiftWithCLProperty.MaxDefectSize.ToString();

                // 표면 영역 검사
                this.chkSlaveUse.IsChecked = (shapeShiftWithCLProperty.IsInspectSlave == 1) ? true : false;
                #region Threshold.
                if (shapeShiftWithCLProperty.ThreshType2 == 0)
                {
                    this.radThresholdUpper2.IsChecked = true;
                    this.chkMinRange2.IsChecked = true;
                    this.chkMaxRange2.IsChecked = false;
                    this.txtThreshold2.Text = shapeShiftWithCLProperty.InspLowerThresh2.ToString();
                    this.txtLowerThreshold2.Text = shapeShiftWithCLProperty.InspLowerThresh2.ToString();
                    this.txtUpperThreshold2.Text = shapeShiftWithCLProperty.InspUpperThresh2.ToString();
                    SetSurfaceRadioState1();
                }
                else if (shapeShiftWithCLProperty.ThreshType2 == 1)
                {
                    this.radThresholdLower2.IsChecked = true;
                    this.chkMinRange2.IsChecked = false;
                    this.chkMaxRange2.IsChecked = true;
                    this.txtThreshold2.Text = shapeShiftWithCLProperty.InspUpperThresh2.ToString();
                    this.txtLowerThreshold2.Text = shapeShiftWithCLProperty.InspLowerThresh2.ToString();
                    this.txtUpperThreshold2.Text = shapeShiftWithCLProperty.InspUpperThresh2.ToString();
                    SetSurfaceRadioState1();
                }
                else
                {
                    this.radThresholdRange2.IsChecked = true;
                    this.chkMinRange2.IsChecked = true;
                    this.chkMaxRange2.IsChecked = true;
                    this.txtLowerThreshold2.Text = shapeShiftWithCLProperty.InspLowerThresh2.ToString();
                    this.txtUpperThreshold2.Text = shapeShiftWithCLProperty.InspUpperThresh2.ToString();
                    this.txtThreshold2.Text = shapeShiftWithCLProperty.InspLowerThresh2.ToString();
                    SetSurfaceRadioState2();
                }
                #endregion
                this.chkApplyAverDiff2.IsChecked = (shapeShiftWithCLProperty.ApplyAverDiff2 == 1) ? true : false;
                this.txtAverMinMargin2.Text = shapeShiftWithCLProperty.AverMinMargin2.ToString();
                this.txtAverMaxMargin2.Text = shapeShiftWithCLProperty.AverMaxMargin2.ToString();
                this.SetSurfaceCheckBoxState(shapeShiftWithCLProperty.InspRange2);
                if (shapeShiftWithCLProperty.ApplyAverDiff2 == 0)
                {
                    shapeShiftWithCLProperty.AverMinMargin2 = 0;
                    shapeShiftWithCLProperty.AverMaxMargin2 = 0;
                }
                this.txtMinMargin2.Text = shapeShiftWithCLProperty.InspMinMargin2.ToString();
                this.txtMaxMargin2.Text = shapeShiftWithCLProperty.InspMaxMargin2.ToString();
                this.txtMinDefectSize2.Text = shapeShiftWithCLProperty.MinDefectSize2.ToString();
                this.txtMaxDefectSize2.Text = shapeShiftWithCLProperty.MaxDefectSize2.ToString();
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
