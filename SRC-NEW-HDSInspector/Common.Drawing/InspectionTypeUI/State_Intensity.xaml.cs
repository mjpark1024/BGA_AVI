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

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>   State intensity property.  </summary>
    /// <remarks>   Minseok, Hwang, 2011-10-20. </remarks>
    public class StateIntensityProperty : InspectionAlgorithm
    {
        public int ThreshType;              // 0000
        public int LowerThresh;             // 0001
        public int UpperThresh;             // 0002
        public int ApplyAverDiff;           // 0003
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
        public int Invert;                  // 0020

        public StateIntensityProperty()
        {
            Code = "3011";
        }

        /// <summary>   Makes a deep copy of this object. </summary>
        /// <remarks>   Minseok, Hwang, 2011-10-11. </remarks>
        /// <returns>   A copy of this object. </returns>
        public override InspectionAlgorithm Clone()
        {
            StateIntensityProperty cloneStateIntensity = new StateIntensityProperty();

            cloneStateIntensity.ThreshType = this.ThreshType;
            cloneStateIntensity.LowerThresh = this.LowerThresh;
            cloneStateIntensity.UpperThresh = this.UpperThresh;
            cloneStateIntensity.ApplyAverDiff = this.ApplyAverDiff;
            cloneStateIntensity.InspRange = this.InspRange;
            cloneStateIntensity.AverMinMargin = this.AverMinMargin;
            cloneStateIntensity.AverMaxMargin = this.AverMaxMargin;
            cloneStateIntensity.MinMargin = this.MinMargin;
            cloneStateIntensity.MaxMargin = this.MaxMargin;
            cloneStateIntensity.ErosionTrainIter = this.ErosionTrainIter;
            cloneStateIntensity.DilationTrainIter = this.DilationTrainIter;
            cloneStateIntensity.ErosionInspIter = this.ErosionInspIter;
            cloneStateIntensity.DilationInspIter = this.DilationInspIter;
            cloneStateIntensity.MinDefectSize = this.MinDefectSize;
            cloneStateIntensity.Invert = this.Invert;

            return cloneStateIntensity;
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
            Invert              = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0020");
        }

        /// <summary>   Saves a property. </summary>
        /// <remarks>   Minseok, Hwang, 2011-10-11. </remarks>
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    if (ThreshType == 1 || ThreshType == 2) // LowerThreshold 값은 임계이하, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    if (ThreshType == 0 || ThreshType == 2) // Upperthreshold 값은 임계이상, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002", UpperThresh);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0003", ApplyAverDiff);
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
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0020", Invert);
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

    /// <summary>   State intensity.  </summary>
    /// <remarks>   Minseok, Hwang, 2011-10-20. </remarks>
    public partial class State_Intensity : UserControl, IInspectionTypeUICommands
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

        public State_Intensity()
        {
            InitializeComponent();
            InitializeEvent();

            this.radThresholdRange.IsChecked = true;
            this.chkMinRange.IsChecked = true;
            this.chkMaxRange.IsChecked = true;
            SetRadioState2();
        }

        /// <summary>   Initializes the event. </summary>
        /// <remarks>   Minseok, Hwang, 2011-09-28. </remarks>
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
                this.chkMinRange.IsChecked = true;
                this.chkMaxRange.IsChecked = true;
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
                    stateIntensityValue.InspRange = inspRange;
                    stateIntensityValue.AverMinMargin = averMinMargin;
                    stateIntensityValue.AverMaxMargin = averMaxMargin;
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
                    oldStateIntensityValue.InspRange = inspRange;
                    oldStateIntensityValue.AverMinMargin = averMinMargin;
                    oldStateIntensityValue.AverMaxMargin = averMaxMargin;
                    oldStateIntensityValue.MinMargin = minMargin;
                    oldStateIntensityValue.MaxMargin = maxMargin;
                    oldStateIntensityValue.ErosionTrainIter = erosionTrainIter;
                    oldStateIntensityValue.DilationTrainIter = dilationTrainIter;
                    oldStateIntensityValue.ErosionInspIter = erosionInspIter;
                    oldStateIntensityValue.DilationInspIter = dilationInspIter;
                    oldStateIntensityValue.MinDefectSize = minDefectSize;
                    oldStateIntensityValue.Invert = invert;
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
            if (m_enumInspectType == eVisInspectType.eInspTypeDownSet)
            {
                if (!DownSetDefaultValue.DefaultValueLoaded)
                {
                    DownSetDefaultValue.DefaultValueLoaded = true;
                    DownSetDefaultValue.LoadDefaultValue();
                }

                if (DownSetDefaultValue.ThreshType == 0)
                {
                    SetRadioState1();
                    this.radThresholdUpper.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = false;
                    this.txtThreshold.Text = DownSetDefaultValue.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = DownSetDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = DownSetDefaultValue.UpperThresh.ToString();
                }
                else if (SurfaceHalfEtchingDefaultValue.ThreshType == 1)
                {
                    SetRadioState1();
                    this.radThresholdLower.IsChecked = true;
                    this.chkMinRange.IsChecked = false;
                    this.chkMaxRange.IsChecked = true;
                    this.txtThreshold.Text = DownSetDefaultValue.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = DownSetDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = DownSetDefaultValue.UpperThresh.ToString();
                }
                else
                {
                    this.radThresholdRange.IsChecked = true;
                    this.chkMinRange.IsChecked = true;
                    this.chkMaxRange.IsChecked = true;
                    this.txtLowerThreshold.Text = DownSetDefaultValue.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = DownSetDefaultValue.UpperThresh.ToString();
                    this.txtThreshold.Text = DownSetDefaultValue.LowerThresh.ToString();
                }
                this.chkApplyAverDiff.IsChecked = (DownSetDefaultValue.ApplyAverDiff == 1) ? true : false;
                this.SetCheckBoxState(DownSetDefaultValue.InspRange);
                this.txtAverMinMargin.Text = DownSetDefaultValue.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = DownSetDefaultValue.AverMaxMargin.ToString();
                this.txtMinMargin.Text = DownSetDefaultValue.MinMargin.ToString();
                this.txtMaxMargin.Text = DownSetDefaultValue.MaxMargin.ToString();
                this.txtErosionTrainIter.Text = DownSetDefaultValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = DownSetDefaultValue.DilationTrainIter.ToString();
                this.txtErosionInspIter.Text = DownSetDefaultValue.ErosionInspIter.ToString();
                this.txtDilationInspIter.Text = DownSetDefaultValue.DilationInspIter.ToString();
                this.txtMinDefectSize.Text = DownSetDefaultValue.MinDefectSize.ToString();
                this.chkInvert.IsChecked = (DownSetDefaultValue.Invert == 1) ? true : false;
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
            StateIntensityProperty stateIntensityProperty = settingValue.InspectionAlgorithm as StateIntensityProperty;
            if (stateIntensityProperty != null)
            {
                if (stateIntensityProperty.ThreshType == 0) // 임계 이상
                {
                    SetRadioState1();
                    this.radThresholdUpper.IsChecked = true;
                    this.txtThreshold.Text = stateIntensityProperty.LowerThresh.ToString();
                    this.txtLowerThreshold.Text = stateIntensityProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = stateIntensityProperty.UpperThresh.ToString();
                }
                else if (stateIntensityProperty.ThreshType == 1) // 임계 이하
                {
                    SetRadioState1();
                    this.radThresholdLower.IsChecked = true;
                    this.txtThreshold.Text = stateIntensityProperty.UpperThresh.ToString();
                    this.txtLowerThreshold.Text = stateIntensityProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = stateIntensityProperty.UpperThresh.ToString();
                }
                else // 임계 범위
                {
                    SetRadioState2();
                    this.radThresholdRange.IsChecked = true;
                    this.txtLowerThreshold.Text = stateIntensityProperty.LowerThresh.ToString();
                    this.txtUpperThreshold.Text = stateIntensityProperty.UpperThresh.ToString();
                    this.txtThreshold.Text = stateIntensityProperty.LowerThresh.ToString();
                }
                this.chkApplyAverDiff.IsChecked = (stateIntensityProperty.ApplyAverDiff == 1) ? true : false;
                this.SetCheckBoxState(stateIntensityProperty.InspRange);
                this.txtAverMinMargin.Text = stateIntensityProperty.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = stateIntensityProperty.AverMaxMargin.ToString();
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
    }
}
