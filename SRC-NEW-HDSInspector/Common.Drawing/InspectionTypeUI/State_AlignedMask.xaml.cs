using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common.Drawing.InspectionInformation;
using Common.DataBase;
using System.Diagnostics;

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>   State aligned mask property.  </summary>
    /// <remarks>   Minseok, Hwang, 2011-10-11. </remarks>
    public class StateAlignedMaskProperty : InspectionAlgorithm
    {
        public int ThreshType;              // 0000
        public int LowerThresh;             // 0001
        public int UpperThresh;             // 0002
        public int ApplyAverDiff;           // 0003
        public int MaskLowerThresh;         // 0004
        public int MaskUpperThresh;         // 0005
        public int InspRange;               // 0006
        public int AverMinMargin;           // 0007
        public int AverMaxMargin;           // 0008
        public int MinMargin;               // 0009
        public int MaxMargin;               // 0010
        public int ErosionTrainIter = 0;    // 0011
        public int DilationTrainIter = 0;   // 0012
        public int ErosionInspIter = 0;     // 0013
        public int DilationInspIter = 0;    // 0014
        public int MinDefectSize;           // 0015
        public int MinSmallDefectSize;      // 0016
        public int MinSmallDefectCount;     // 0017

        public StateAlignedMaskProperty()
        {
            Code = "3012";
        }

        /// <summary>   Makes a deep copy of this object. </summary>
        /// <remarks>   Minseok, Hwang, 2011-10-11. </remarks>
        /// <returns>   A copy of this object. </returns>
        public override InspectionAlgorithm Clone()
        {
            StateAlignedMaskProperty cloneStateAlignedMask = new StateAlignedMaskProperty();

            cloneStateAlignedMask.ThreshType = this.ThreshType;
            cloneStateAlignedMask.LowerThresh = this.LowerThresh;
            cloneStateAlignedMask.UpperThresh = this.UpperThresh;
            cloneStateAlignedMask.ApplyAverDiff = this.ApplyAverDiff;
            cloneStateAlignedMask.MaskLowerThresh = this.MaskLowerThresh;
            cloneStateAlignedMask.MaskUpperThresh = this.MaskUpperThresh;
            cloneStateAlignedMask.InspRange = this.InspRange;
            cloneStateAlignedMask.AverMinMargin = this.AverMinMargin;
            cloneStateAlignedMask.AverMaxMargin = this.AverMaxMargin;
            cloneStateAlignedMask.MinMargin = this.MinMargin;
            cloneStateAlignedMask.MaxMargin = this.MaxMargin;
            cloneStateAlignedMask.ErosionTrainIter = this.ErosionTrainIter;
            cloneStateAlignedMask.DilationTrainIter = this.DilationTrainIter;
            cloneStateAlignedMask.ErosionInspIter = this.ErosionInspIter;
            cloneStateAlignedMask.DilationInspIter = this.DilationInspIter;
            cloneStateAlignedMask.MinDefectSize = this.MinDefectSize;
            cloneStateAlignedMask.MinSmallDefectSize = this.MinSmallDefectSize;
            cloneStateAlignedMask.MinSmallDefectCount = this.MinSmallDefectCount;

            return cloneStateAlignedMask;
        }

        #region Load & Save Properties.
        /// <summary>   Loads a property. </summary>
        /// <remarks>   Minseok, Hwang, 2011-10-11. </remarks>
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType          = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            LowerThresh         = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UpperThresh         = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            ApplyAverDiff       = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0003");
            MaskLowerThresh     = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0004");
            MaskUpperThresh     = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0005");
            InspRange           = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0006");
            AverMinMargin       = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0007");
            AverMaxMargin       = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0008");
            MinMargin           = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0009");
            MaxMargin           = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0010");
            ErosionTrainIter    = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0011");
            DilationTrainIter   = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0012");
            ErosionInspIter     = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0013");
            DilationInspIter    = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0014");
            MinDefectSize       = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0015");
            MinSmallDefectSize  = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0016");
            MinSmallDefectCount = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0017");
        }

        /// <summary>   Saves a property. </summary>
        /// <remarks>   Minseok, Hwang, 2011-10-11. </remarks>
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    if (ThreshType == 0 || ThreshType == 2) // LowerThreshold 값은 임계이상, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    if (ThreshType == 1 || ThreshType == 2) // Upperthreshold 값은 임계이하, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002", UpperThresh);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0003", ApplyAverDiff);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;                                                    
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0004", MaskLowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;                                                   
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0005", MaskUpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;                                                    
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0006", InspRange);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;                                                    
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0007", AverMinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;                                                   
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0008", AverMaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;                                                    
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0009", MinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;                                                     
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0010", MaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;                                                    
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0011", ErosionTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;                                                    
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0012", DilationTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;                                                   
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0013", ErosionInspIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;                                                    
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0014", DilationInspIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;                                                    
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0015", MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;                                                     
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0016", MinSmallDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;                                                   
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0017", MinSmallDefectCount);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion
    }

    /// <summary>   State aligned mask.  </summary>
    /// <remarks>   Minseok, Hwang, 2011-10-11. </remarks>
    public partial class State_AlignedMask : UserControl, IInspectionTypeUICommands
    {
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

        public State_AlignedMask()
        {
            InitializeComponent();
            InitializeEvent();

            this.radThresholdRange.IsChecked = true;
            this.chkMinRange.IsChecked = true;
            this.chkMaxRange.IsChecked = true;
            this.chkSameValue.IsChecked = true;
            SetRadioState2();
        }

        /// <summary>   Initializes the event. </summary>
        /// <remarks>   Minseok, Hwang, 2011-09-28. </remarks>
        private void InitializeEvent()
        {
            this.radThresholdLower.Click += radThreshold_Click;
            this.radThresholdUpper.Click += radThreshold_Click;
            this.radThresholdRange.Click += new RoutedEventHandler(radThresholdRange_Click);
            this.chkSameValue.Click += new RoutedEventHandler(chkSameValue_Click);
            this.txtThreshold.TextChanged += new TextChangedEventHandler(txtThreshold_TextChanged);
            this.txtLowerThreshold.TextChanged += new TextChangedEventHandler(txtLowerThreshold_TextChanged);
            this.txtUpperThreshold.TextChanged += new TextChangedEventHandler(txtUpperThreshold_TextChanged);
            this.txtMaskLowerThresh.TextChanged += new TextChangedEventHandler(txtMaskLowerThresh_TextChanged);
            this.txtMaskUpperThresh.TextChanged += new TextChangedEventHandler(txtMaskUpperThresh_TextChanged);
        }

        #region Textbox text changed events.
        private void txtThreshold_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (chkSameValue.IsChecked == true)
            {
                if (radThresholdLower.IsChecked == true)
                {
                    txtMaskLowerThresh.Text = "0";
                    txtMaskUpperThresh.Text = txtThreshold.Text;

                    //txtUpperThreshold.Text = txtThreshold.Text;
                }
                else if (radThresholdUpper.IsChecked == true)
                {
                    txtMaskLowerThresh.Text = txtThreshold.Text;
                    txtMaskUpperThresh.Text = "255";

                    //txtLowerThreshold.Text = txtThreshold.Text;
                }
            }
        }

        private void txtUpperThreshold_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (chkSameValue.IsChecked == true)
            {
                txtMaskUpperThresh.Text = txtUpperThreshold.Text;
            }
        }

        private void txtLowerThreshold_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (chkSameValue.IsChecked == true)
            {
                txtMaskLowerThresh.Text = txtLowerThreshold.Text;
            }
        }

        private void txtMaskUpperThresh_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (chkSameValue.IsChecked == true)
            {
                txtUpperThreshold.Text = txtMaskUpperThresh.Text;
            }
        }

        private void txtMaskLowerThresh_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (chkSameValue.IsChecked == true)
            {
                txtLowerThreshold.Text = txtMaskLowerThresh.Text;
            }
        }
        #endregion

        private void chkSameValue_Click(object sender, RoutedEventArgs e)
        {
            if (radThresholdUpper.IsChecked == true)
            {
                this.chkMinRange.IsChecked = true;
                this.chkMaxRange.IsChecked = false;
                if (chkSameValue.IsChecked == true)
                {
                    this.txtMaskLowerThresh.Text = this.txtThreshold.Text;
                    this.txtMaskUpperThresh.Text = "255";
                }
            }
            else if (radThresholdLower.IsChecked == true)
            {
                this.chkMinRange.IsChecked = false;
                this.chkMaxRange.IsChecked = true;
                if (chkSameValue.IsChecked == true)
                {
                    this.txtMaskLowerThresh.Text = "0";
                    this.txtMaskUpperThresh.Text = this.txtThreshold.Text;
                }
            }
            else //radThresholdRange.IsChecked == true
            {
                this.chkMinRange.IsChecked = true;
                this.chkMaxRange.IsChecked = true;
                if (chkSameValue.IsChecked == true)
                {
                    this.txtMaskLowerThresh.Text = this.txtLowerThreshold.Text;
                    this.txtMaskUpperThresh.Text = this.txtUpperThreshold.Text;
                }
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

        public void SetDialog(string strCaption, InspectionInformation.eVisInspectType inspectType)
        {
            this.txtCaption.Text = strCaption;
            this.m_enumInspectType = inspectType;
        }

        public void TrySave(GraphicsBase graphic, int anInspectID)
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

                #region Check 평균값 최소 / 최대 허용범위
                int averMinMargin = Convert.ToInt32(txtAverMinMargin.Text);
                int averMaxMargin = Convert.ToInt32(txtAverMaxMargin.Text);

                //if (averMinMargin > averMaxMargin)
                //{
                //    MessageBox.Show("평균값 최소범위는 평균값 최대범위를 초과할 수 없습니다.", "Information");
                //    this.txtAverMinMargin.Focus();
                //    return;
                //}
                #endregion

                #region Check 검사 최소 / 최대 허용범위
                int minMargin = Convert.ToInt32(txtMinMargin.Text);
                int maxMargin = Convert.ToInt32(txtMaxMargin.Text);

                //if (minMargin > maxMargin)
                //{
                //    MessageBox.Show("검사 최소 허용범위는 검사 최대 허용범위를 초과할 수 없습니다.", "Information");
                //    this.txtMinMargin.Focus();
                //    return;
                //}
                #endregion

                int erosionTrainIter = Convert.ToInt32(txtErosionTrainIter.Text);
                int dilationTrainIter = Convert.ToInt32(txtDilationTrainIter.Text);
                int erosionInspIter = Convert.ToInt32(txtErosionInspIter.Text);
                int dilationInspIter = Convert.ToInt32(txtDilationInspIter.Text);
                int minDefectSize = Convert.ToInt32(txtMinDefectSize.Text);
                int minSmallDefectSize = Convert.ToInt32(txtMinSmallDefectCount.Text);
                int minSmallDefectCount = Convert.ToInt32(txtMinSmallDefectCount.Text);

                StateAlignedMaskProperty oldStateAlignedMaskValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldStateAlignedMaskValue = element.InspectionAlgorithm as StateAlignedMaskProperty; // 기존에 저장된 값이 있는 경우.
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
                    StateAlignedMaskProperty stateAlignedMaskValue = new StateAlignedMaskProperty();

                    stateAlignedMaskValue.ThreshType = threshType;
                    stateAlignedMaskValue.LowerThresh = lowerThresh;
                    stateAlignedMaskValue.UpperThresh = upperThresh;
                    stateAlignedMaskValue.ApplyAverDiff = applyAverDiff;
                    stateAlignedMaskValue.MaskLowerThresh = maskLowerThresh;
                    stateAlignedMaskValue.MaskUpperThresh = maskUpperThresh;
                    stateAlignedMaskValue.InspRange = inspRange;
                    stateAlignedMaskValue.AverMinMargin = averMinMargin;
                    stateAlignedMaskValue.AverMaxMargin = averMaxMargin;
                    stateAlignedMaskValue.MinMargin = minMargin;
                    stateAlignedMaskValue.MaxMargin = maxMargin;
                    stateAlignedMaskValue.ErosionTrainIter = erosionTrainIter;
                    stateAlignedMaskValue.DilationTrainIter = dilationTrainIter;
                    stateAlignedMaskValue.ErosionInspIter = erosionInspIter;
                    stateAlignedMaskValue.DilationInspIter = dilationInspIter;
                    stateAlignedMaskValue.MinDefectSize = minDefectSize;
                    stateAlignedMaskValue.MinSmallDefectSize = minSmallDefectSize;
                    stateAlignedMaskValue.MinSmallDefectCount = minSmallDefectCount;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = stateAlignedMaskValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }

                    Debug.WriteLine("L: " + stateAlignedMaskValue.LowerThresh.ToString() + "H: " + stateAlignedMaskValue.UpperThresh.ToString());
                    Debug.WriteLine("ML: " + stateAlignedMaskValue.MaskLowerThresh.ToString() + "MH: " + stateAlignedMaskValue.MaskUpperThresh.ToString());
                }
                else
                {
                    oldStateAlignedMaskValue.ThreshType = threshType;
                    oldStateAlignedMaskValue.LowerThresh = lowerThresh;
                    oldStateAlignedMaskValue.UpperThresh = upperThresh;
                    oldStateAlignedMaskValue.ApplyAverDiff = applyAverDiff;
                    oldStateAlignedMaskValue.MaskLowerThresh = maskLowerThresh;
                    oldStateAlignedMaskValue.MaskUpperThresh = maskUpperThresh;
                    oldStateAlignedMaskValue.InspRange = inspRange;
                    oldStateAlignedMaskValue.AverMinMargin = averMinMargin;
                    oldStateAlignedMaskValue.AverMaxMargin = averMaxMargin;
                    oldStateAlignedMaskValue.MinMargin = minMargin;
                    oldStateAlignedMaskValue.MaxMargin = maxMargin;
                    oldStateAlignedMaskValue.ErosionTrainIter = erosionTrainIter;
                    oldStateAlignedMaskValue.DilationTrainIter = dilationTrainIter;
                    oldStateAlignedMaskValue.ErosionInspIter = erosionInspIter;
                    oldStateAlignedMaskValue.DilationInspIter = dilationInspIter;
                    oldStateAlignedMaskValue.MinDefectSize = minDefectSize;
                    oldStateAlignedMaskValue.MinSmallDefectSize = minSmallDefectSize;
                    oldStateAlignedMaskValue.MinSmallDefectCount = minSmallDefectCount;
                }
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

        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeSurfaceHalfEtching)
            {
                if (!SurfaceHalfEtchingDefaultValue.DefaultValueLoaded)
                {
                    SurfaceHalfEtchingDefaultValue.DefaultValueLoaded = true;
                    SurfaceHalfEtchingDefaultValue.LoadDefaultValue();
                }

                if (SurfaceHalfEtchingDefaultValue.ThreshType == 0)
                {
                    SetRadioState1();
                    this.radThresholdUpper.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = false;
                    this.txtThreshold.Text = SurfaceHalfEtchingDefaultValue.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = SurfaceHalfEtchingDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SurfaceHalfEtchingDefaultValue.UpperThresh.ToString();
                }
                else if (SurfaceHalfEtchingDefaultValue.ThreshType == 1)
                {
                    SetRadioState1();
                    this.radThresholdLower.IsChecked = true;
                    this.chkMinRange.IsChecked = false;
                    this.chkMaxRange.IsChecked = true;
                    this.txtThreshold.Text = SurfaceHalfEtchingDefaultValue.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = SurfaceHalfEtchingDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SurfaceHalfEtchingDefaultValue.UpperThresh.ToString();
                }
                else
                {
                    this.radThresholdRange.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = true;
                    this.txtLowerThreshold.Text = SurfaceHalfEtchingDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SurfaceHalfEtchingDefaultValue.UpperThresh.ToString();
                    this.txtThreshold.Text = SurfaceHalfEtchingDefaultValue.LowerThresh.ToString();
                }
                this.chkApplyAverDiff.IsChecked = (SurfaceHalfEtchingDefaultValue.ApplyAverDiff == 1) ? true : false;
                this.txtMaskLowerThresh.Text = SurfaceHalfEtchingDefaultValue.MaskLowerThresh.ToString();
                this.txtMaskUpperThresh.Text = SurfaceHalfEtchingDefaultValue.MaskUpperThresh.ToString();
                this.SetCheckBoxState(SurfaceHalfEtchingDefaultValue.InspRange);
                this.txtAverMinMargin.Text = SurfaceHalfEtchingDefaultValue.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = SurfaceHalfEtchingDefaultValue.AverMaxMargin.ToString();
                this.txtMinMargin.Text = SurfaceHalfEtchingDefaultValue.MinMargin.ToString();
                this.txtMaxMargin.Text = SurfaceHalfEtchingDefaultValue.MaxMargin.ToString();
                this.txtErosionTrainIter.Text = SurfaceHalfEtchingDefaultValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = SurfaceHalfEtchingDefaultValue.DilationTrainIter.ToString();
                this.txtErosionInspIter.Text = SurfaceHalfEtchingDefaultValue.ErosionInspIter.ToString();
                this.txtDilationInspIter.Text = SurfaceHalfEtchingDefaultValue.DilationInspIter.ToString();
                this.txtMinDefectSize.Text = SurfaceHalfEtchingDefaultValue.MinDefectSize.ToString();
                this.txtMinSmallDefectSize.Text = SurfaceHalfEtchingDefaultValue.MinSmallDefectSize.ToString();
                this.txtMinSmallDefectCount.Text = SurfaceHalfEtchingDefaultValue.MinSmallDefectCount.ToString();
            }
            else if (m_enumInspectType == eVisInspectType.eInspTypeSurface)
            {
                if (!SurfaceDefaultValue.DefaultValueLoaded)
                {
                    SurfaceDefaultValue.DefaultValueLoaded = true;
                    SurfaceDefaultValue.LoadDefaultValue();
                }

                if (SurfaceDefaultValue.ThreshType == 0)
                {
                    SetRadioState1();
                    this.radThresholdUpper.IsChecked = true;
                    this.txtThreshold.Text = SurfaceDefaultValue.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = SurfaceDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SurfaceDefaultValue.UpperThresh.ToString();
                }
                else if (SurfaceDefaultValue.ThreshType == 1)
                {
                    SetRadioState1();
                    this.radThresholdLower.IsChecked = true;
                    this.txtThreshold.Text = SurfaceDefaultValue.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = SurfaceDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SurfaceDefaultValue.UpperThresh.ToString();
                }
                else
                {
                    this.radThresholdRange.IsChecked = true;
                    this.txtLowerThreshold.Text = SurfaceDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SurfaceDefaultValue.UpperThresh.ToString();
                    this.txtThreshold.Text = SurfaceDefaultValue.LowerThresh.ToString();
                }
                this.chkApplyAverDiff.IsChecked = (SurfaceDefaultValue.ApplyAverDiff == 1) ? true : false;
                this.txtMaskLowerThresh.Text = SurfaceDefaultValue.MaskLowerThresh.ToString();
                this.txtMaskUpperThresh.Text = SurfaceDefaultValue.MaskUpperThresh.ToString();
                this.SetCheckBoxState(SurfaceDefaultValue.InspRange);
                this.txtAverMinMargin.Text = SurfaceDefaultValue.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = SurfaceDefaultValue.AverMaxMargin.ToString();
                this.txtMinMargin.Text = SurfaceDefaultValue.MinMargin.ToString();
                this.txtMaxMargin.Text = SurfaceDefaultValue.MaxMargin.ToString();
                this.txtErosionTrainIter.Text = SurfaceDefaultValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = SurfaceDefaultValue.DilationTrainIter.ToString();
                this.txtErosionInspIter.Text = SurfaceDefaultValue.ErosionInspIter.ToString();
                this.txtDilationInspIter.Text = SurfaceDefaultValue.DilationInspIter.ToString();
                this.txtMinDefectSize.Text = SurfaceDefaultValue.MinDefectSize.ToString();
                this.txtMinSmallDefectSize.Text = SurfaceDefaultValue.MinSmallDefectSize.ToString();
                this.txtMinSmallDefectCount.Text = SurfaceDefaultValue.MinSmallDefectCount.ToString();
            }
            else if (m_enumInspectType == eVisInspectType.eInspTypeSpace)
            {
                if (!SpaceDefaultValue.DefaultValueLoaded)
                {
                    SpaceDefaultValue.DefaultValueLoaded = true;
                    SpaceDefaultValue.LoadDefaultValue();
                }

                if (SpaceDefaultValue.ThreshType == 0)
                {
                    SetRadioState1();
                    this.radThresholdUpper.IsChecked = true;
                    this.txtThreshold.Text = SpaceDefaultValue.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = SpaceDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SpaceDefaultValue.UpperThresh.ToString();
                }
                else if (SpaceDefaultValue.ThreshType == 1)
                {
                    SetRadioState1();
                    this.radThresholdLower.IsChecked = true;
                    this.txtThreshold.Text = SpaceDefaultValue.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = SpaceDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SpaceDefaultValue.UpperThresh.ToString();
                }
                else
                {
                    this.radThresholdRange.IsChecked = true;
                    this.txtLowerThreshold.Text = SpaceDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = SpaceDefaultValue.UpperThresh.ToString();
                    this.txtThreshold.Text = SpaceDefaultValue.LowerThresh.ToString();
                }
                this.chkApplyAverDiff.IsChecked = (SpaceDefaultValue.ApplyAverDiff == 1) ? true : false;
                this.txtMaskLowerThresh.Text = SpaceDefaultValue.MaskLowerThresh.ToString();
                this.txtMaskUpperThresh.Text = SpaceDefaultValue.MaskUpperThresh.ToString();
                this.SetCheckBoxState(SpaceDefaultValue.InspRange);
                this.txtAverMinMargin.Text = SpaceDefaultValue.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = SpaceDefaultValue.AverMaxMargin.ToString();
                this.txtMinMargin.Text = SpaceDefaultValue.MinMargin.ToString();
                this.txtMaxMargin.Text = SpaceDefaultValue.MaxMargin.ToString();
                this.txtErosionTrainIter.Text = SpaceDefaultValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = SpaceDefaultValue.DilationTrainIter.ToString();
                this.txtErosionInspIter.Text = SpaceDefaultValue.ErosionInspIter.ToString();
                this.txtDilationInspIter.Text = SpaceDefaultValue.DilationInspIter.ToString();
                this.txtMinDefectSize.Text = SpaceDefaultValue.MinDefectSize.ToString();
                this.txtMinSmallDefectSize.Text = SpaceDefaultValue.MinSmallDefectSize.ToString();
                this.txtMinSmallDefectCount.Text = SpaceDefaultValue.MinSmallDefectCount.ToString();
            }
        }

        /// <summary>   Sets a check box state. </summary>
        /// <remarks>   Minseok, Hwang, 2011-10-19. </remarks>
        /// <param name="switchValue">  The switch value. </param>
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

        public void Display(InspectionInformation.InspectionItem settingValue)
        {
            StateAlignedMaskProperty stateAlignedMaskProperty = settingValue.InspectionAlgorithm as StateAlignedMaskProperty;
            if (stateAlignedMaskProperty != null)
            {
                if (stateAlignedMaskProperty.ThreshType == 0) // 임계 이상
                {
                    SetRadioState1();
                    this.radThresholdUpper.IsChecked = true;
                    this.txtThreshold.Text = stateAlignedMaskProperty.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = stateAlignedMaskProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = stateAlignedMaskProperty.UpperThresh.ToString();
                }
                else if (stateAlignedMaskProperty.ThreshType == 1) // 임계 이하
                {
                    SetRadioState1();
                    this.radThresholdLower.IsChecked = true;
                    this.txtThreshold.Text = stateAlignedMaskProperty.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = stateAlignedMaskProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = stateAlignedMaskProperty.UpperThresh.ToString();
                }
                else // 임계 범위
                {
                    SetRadioState2();
                    this.radThresholdRange.IsChecked = true;
                    this.txtLowerThreshold.Text = stateAlignedMaskProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = stateAlignedMaskProperty.UpperThresh.ToString();
                    this.txtThreshold.Text = stateAlignedMaskProperty.LowerThresh.ToString();
                }
                this.chkApplyAverDiff.IsChecked = (stateAlignedMaskProperty.ApplyAverDiff == 1) ? true : false;
                this.chkSameValue.IsChecked = true;
                this.txtMaskLowerThresh.Text = stateAlignedMaskProperty.MaskLowerThresh.ToString();
                this.txtMaskUpperThresh.Text = stateAlignedMaskProperty.MaskUpperThresh.ToString();
                this.SetCheckBoxState(stateAlignedMaskProperty.InspRange);
                this.txtAverMinMargin.Text = stateAlignedMaskProperty.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = stateAlignedMaskProperty.AverMaxMargin.ToString();
                this.txtMinMargin.Text = stateAlignedMaskProperty.MinMargin.ToString();
                this.txtMaxMargin.Text = stateAlignedMaskProperty.MaxMargin.ToString();
                this.txtErosionTrainIter.Text = stateAlignedMaskProperty.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = stateAlignedMaskProperty.DilationTrainIter.ToString();
                this.txtErosionInspIter.Text = stateAlignedMaskProperty.ErosionInspIter.ToString();
                this.txtDilationInspIter.Text = stateAlignedMaskProperty.DilationInspIter.ToString();
                this.txtMinDefectSize.Text = stateAlignedMaskProperty.MinDefectSize.ToString();
                this.txtMinSmallDefectSize.Text = stateAlignedMaskProperty.MinSmallDefectSize.ToString();
                this.txtMinSmallDefectCount.Text = stateAlignedMaskProperty.MinSmallDefectCount.ToString();
            }
        }
    }
}
