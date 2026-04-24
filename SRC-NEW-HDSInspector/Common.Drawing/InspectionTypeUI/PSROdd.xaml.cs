using Common.Drawing.InspectionInformation;
using Common.Drawing.MarkingInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


//////////////////////////////////////////////////////
// PSROdd 알고리즘 : 코드 3026
//////////////////////////////////////////////////////
namespace Common.Drawing.InspectionTypeUI
{
    // PSR 하지이물 검사 타입
    public enum PSR_Inspection_Type
    {
        Non_Circuit = 0,
        Circuit = 1,

        Non = -1,
    }

    /// <summary>
    /// PSROdd.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PSROdd : System.Windows.Controls.UserControl, IInspectionTypeUICommands
    {
        private PSROddProperty m_previewValue;

        public eVisInspectType InspectType
        {
            get
            {
                return m_enumInspectType;
            }
        }
        private eVisInspectType m_enumInspectType;

        public PSR_Inspection_Type PSR_Type = PSR_Inspection_Type.Non_Circuit;  // 초기는 Non_Circuit

        public PSROdd()
        {
            InitializeComponent();
            InitializeEvent();
        }
        private void InitializeEvent()
        {
            this.rad_Non_Circuit.Click += radType_Click;
            this.radCircuit.Click += radType_Click;      
        }
        private void radType_Click(object sender, RoutedEventArgs e)
        {
            SetRadioState((PSR_Inspection_Type)Convert.ToInt32(((System.Windows.Controls.RadioButton)sender).Tag));
        }

        private void SetRadioState(PSR_Inspection_Type type)
        {

            switch (type)
            {
                case PSR_Inspection_Type.Non_Circuit:
                    rad_Non_Circuit.IsChecked = true;
                    radCircuit.IsChecked = false;

                    grd_Non_CircuitParam.Visibility = Visibility.Visible;
                    grd_Circuit_Param.Visibility = Visibility.Hidden;

                    PSR_Type = PSR_Inspection_Type.Non_Circuit;
                    break;

                case PSR_Inspection_Type.Circuit:
                    rad_Non_Circuit.IsChecked = false;
                    radCircuit.IsChecked = true;

                    grd_Non_CircuitParam.Visibility = Visibility.Hidden;
                    grd_Circuit_Param.Visibility = Visibility.Visible;

                    PSR_Type = PSR_Inspection_Type.Circuit;
                    break;
            }

        }

        private PSR_Inspection_Type GetRadioState(int anInspectID)
        {
            if (anInspectID != -1)
            {
                if (rad_Non_Circuit.IsChecked == true) return PSR_Inspection_Type.Non_Circuit;
                if (radCircuit.IsChecked == true) return PSR_Inspection_Type.Circuit;
            }

            return PSR_Inspection_Type.Non_Circuit;
        }

        public void SetDialog(string strCaption, eVisInspectType inspectType)
        {
            this.m_enumInspectType = inspectType;

        }

        public void Display(InspectionItem settingValue, int MarginX, int MatginY)
        {
            PSROddProperty psroddProperty  = settingValue.InspectionAlgorithm as PSROddProperty;
            if (psroddProperty != null)
            {
                SetRadioState((PSR_Inspection_Type)psroddProperty.ThreshType);

                #region Non Circuit

                //Core_RGB
                this.txt_Core_Threshold.Text = psroddProperty.Core_Threshold.ToString();
                this.txt_Core_ExceptionThreshold.Text = psroddProperty.Core_ExceptionThreshold.ToString();
                this.txt_Core_MinDefectSize.Text = psroddProperty.Core_MinDefectSize.ToString();

                //Metal_채도
                this.txt_Metal_LowerThreshold.Text = psroddProperty.Metal_LowerThreshold.ToString();
                this.txt_Metal_UpperThreshold.Text = psroddProperty.Metal_UpperThreshold.ToString();
                this.txtMetal_MinDefectSize.Text = psroddProperty.Metal_MinDefectSize.ToString();

                // Nomal
                this.txt_Summation_range.Text = psroddProperty.Summation_range.ToString();
                this.txt_Summation_detection_size.Text = psroddProperty.Summation_detection_size.ToString();
                this.txtMask_Threshold.Text = psroddProperty.Mask_Threshold.ToString();
                this.txtMask_Extension.Text = psroddProperty.Mask_Extension.ToString();
                this.txtStep_Threshold.Text = psroddProperty.Step_Threshold.ToString();
                this.txtStep_Expansion.Text = psroddProperty.Step_Expansion.ToString();

                // 필터
                this.txtHV_ratio_value.Text = psroddProperty.HV_ratio_value.ToString();
                this.txtMinRelative_size.Text = psroddProperty.Min_Relative_size.ToString();
                this.txtMaxRelative_size.Text = psroddProperty.Max_Relative_size.ToString();
                this.txtArea_Relative.Text = psroddProperty.Area_Relative.ToString();

                #endregion

                //Circuit
                this.txtCircuit_Threshold.Text = psroddProperty.Circuit_Threshold.ToString();
                this.txtCircuit_MinDefectSize.Text = psroddProperty.Circuit_MinDefectSize.ToString();

            }
        }

        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypePSROdd)
            {
                if (!PSROddDefaultValue.DefaultValueLoaded)
                {
                    PSROddDefaultValue.DefaultValueLoaded = true;
                    PSROddDefaultValue.LoadDefaultValue();
                }


                if((bool)this.radCircuit.IsChecked)
                {
                    //Circuit
                    this.txtCircuit_Threshold.Text = PSROddDefaultValue.Default_Circuit_Threshold.ToString();
                    this.txtCircuit_MinDefectSize.Text = PSROddDefaultValue.Default_Circuit_MinDefectSize.ToString();
                }

                if((bool)this.rad_Non_Circuit.IsChecked)
                {
                    //Core_RGB
                    this.txt_Core_Threshold.Text = PSROddDefaultValue.Default_Core_Threshold.ToString();
                    this.txt_Core_ExceptionThreshold.Text = PSROddDefaultValue.Default_Core_ExceptionThreshold.ToString();
                    this.txt_Core_MinDefectSize.Text = PSROddDefaultValue.Default_Core_MinDefectSize.ToString();

                    //Metal_채도
                    this.txt_Metal_LowerThreshold.Text = PSROddDefaultValue.Default_Metal_LowerThreshold.ToString();
                    this.txt_Metal_UpperThreshold.Text = PSROddDefaultValue.Default_Metal_UpperThreshold.ToString();
                    this.txtMetal_MinDefectSize.Text = PSROddDefaultValue.Default_Metal_MinDefectSize.ToString();

                    // Nomal
                    this.txt_Summation_range.Text = PSROddDefaultValue.Default_Summation_range.ToString();
                    this.txt_Summation_detection_size.Text = PSROddDefaultValue.Default_Summation_detection_size.ToString();
                    this.txtMask_Threshold.Text = PSROddDefaultValue.Default_Mask_Threshold.ToString();
                    this.txtMask_Extension.Text = PSROddDefaultValue.Default_Mask_Extension.ToString();
                    this.txtStep_Threshold.Text = PSROddDefaultValue.Default_Step_Threshold.ToString();
                    this.txtStep_Expansion.Text = PSROddDefaultValue.Default_Step_Expansion.ToString();

                    // 필터
                    this.txtHV_ratio_value.Text = PSROddDefaultValue.Default_HV_ratio_value.ToString();
                    this.txtMinRelative_size.Text = PSROddDefaultValue.Default_Min_Relative_size.ToString();
                    this.txtMaxRelative_size.Text = PSROddDefaultValue.Default_Max_Relative_size.ToString();
                    this.txtArea_Relative.Text = PSROddDefaultValue.Default_Area_Relative.ToString();
                }

            }
        }


        public void SetPreviewValue()
        {
            if (m_previewValue != null)
            {

                SetRadioState((PSR_Inspection_Type)m_previewValue.ThreshType);


                //Core_RGB
                this.txt_Core_Threshold.Text = m_previewValue.Core_Threshold.ToString();
                this.txt_Core_ExceptionThreshold.Text = m_previewValue.Core_ExceptionThreshold.ToString();
                this.txt_Core_MinDefectSize.Text = m_previewValue.Core_MinDefectSize.ToString();

                //Metal_채도
                this.txt_Metal_LowerThreshold.Text = m_previewValue.Metal_LowerThreshold.ToString();
                this.txt_Metal_UpperThreshold.Text = m_previewValue.Metal_UpperThreshold.ToString();
                this.txtMetal_MinDefectSize.Text = m_previewValue.Metal_MinDefectSize.ToString();

                // Nomal
                this.txt_Summation_range.Text = m_previewValue.Summation_range.ToString();
                this.txt_Summation_detection_size.Text = m_previewValue.Summation_detection_size.ToString();
                this.txtMask_Threshold.Text = m_previewValue.Mask_Threshold.ToString();
                this.txtMask_Extension.Text = m_previewValue.Mask_Extension.ToString();
                this.txtStep_Threshold.Text = m_previewValue.Step_Threshold.ToString();
                this.txtStep_Expansion.Text = m_previewValue.Step_Expansion.ToString();

                //필터
                this.txtHV_ratio_value.Text = m_previewValue.HV_ratio_value.ToString();
                this.txtMinRelative_size.Text = m_previewValue.Min_Relative_size.ToString();
                this.txtMaxRelative_size.Text = m_previewValue.Max_Relative_size.ToString();
                this.txtArea_Relative.Text = m_previewValue.Area_Relative.ToString();


                //Circuit
                this.txtCircuit_Threshold.Text = m_previewValue.Circuit_Threshold.ToString();
                this.txtCircuit_MinDefectSize.Text = m_previewValue.Circuit_MinDefectSize.ToString();
            }
        }

        public void TryAdd(ref InspectList list, int anInspectID)
        {
            try
            {
                //Common
                int threshType = (int)GetRadioState(anInspectID);

                //Core_RGB
                int Core_Threshold = Convert.ToInt32(txt_Core_Threshold.Text);
                int Core_ExceptionThreshold = Convert.ToInt32(txt_Core_ExceptionThreshold.Text);
                int Core_MinDefectSize = Convert.ToInt32(txt_Core_MinDefectSize.Text);

                //Metal_채도
                int Metal_LowerThreshold = Convert.ToInt32(txt_Metal_LowerThreshold.Text);
                int Metal_UpperThreshold = Convert.ToInt32(txt_Metal_UpperThreshold.Text);
                int Metal_MinDefectSize = Convert.ToInt32(txtMetal_MinDefectSize.Text);

                // Nomal
                int Summation_range = Convert.ToInt32(txt_Summation_range.Text);
                int Summation_detection_size = Convert.ToInt32(txt_Summation_detection_size.Text);
                int Mask_Threshold = Convert.ToInt32(txtMask_Threshold.Text);
                int Mask_Extension = Convert.ToInt32(txtMask_Extension.Text);
                int Step_Threshold = Convert.ToInt32(txtStep_Threshold.Text);
                int Step_Expansion = Convert.ToInt32(txtStep_Expansion.Text);

                //필터
                int HV_ratio_value = Convert.ToInt32(txtHV_ratio_value.Text);
                int Min_Relative_size = Convert.ToInt32(txtMinRelative_size.Text);
                int Max_Relative_size = Convert.ToInt32(txtMaxRelative_size.Text);
                int Area_Relative = Convert.ToInt32(txtArea_Relative.Text);

                //Circuit
                int Circuit_Threshold = Convert.ToInt32(txtCircuit_Threshold.Text);
                int Circuit_MinDefectSize = Convert.ToInt32(txtCircuit_MinDefectSize.Text);


                PSROddProperty psroddValue = null;

                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID) 
                    {
                        psroddValue =  element.InspectionAlgorithm as PSROddProperty;
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (psroddValue == null)
                {
                    PSROddProperty oddProperty = new PSROddProperty();

                    //Common
                    oddProperty.ThreshType = threshType;

                    //Core_RGB
                    oddProperty.Core_Threshold = Core_Threshold;
                    oddProperty.Core_ExceptionThreshold = Core_ExceptionThreshold;
                    oddProperty.Core_MinDefectSize = Core_MinDefectSize;

                    //Metal_채도
                    oddProperty.Metal_LowerThreshold = Metal_LowerThreshold;
                    oddProperty.Metal_UpperThreshold = Metal_UpperThreshold;
                    oddProperty.Metal_MinDefectSize = Metal_MinDefectSize;

                    // Nomal
                    oddProperty.Summation_range = Summation_range;
                    oddProperty.Summation_detection_size = Summation_detection_size;
                    oddProperty.Mask_Threshold = Mask_Threshold;
                    oddProperty.Mask_Extension = Mask_Extension;
                    oddProperty.Step_Threshold = Step_Threshold;
                    oddProperty.Step_Expansion = Step_Expansion;

                    //필터
                    oddProperty.HV_ratio_value = HV_ratio_value;
                    oddProperty.Min_Relative_size = Min_Relative_size;
                    oddProperty.Max_Relative_size = Max_Relative_size;
                    oddProperty.Area_Relative = Area_Relative;

                    //Circuit
                    oddProperty.Circuit_Threshold = Circuit_Threshold;
                    oddProperty.Circuit_MinDefectSize = Circuit_MinDefectSize;


                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionAlgorithm = oddProperty;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    //Common
                    psroddValue.ThreshType = threshType;

                    //Core_RGB
                    psroddValue.Core_Threshold = Core_Threshold;
                    psroddValue.Core_ExceptionThreshold = Core_ExceptionThreshold;
                    psroddValue.Core_MinDefectSize = Core_MinDefectSize;

                    //Metal_채도
                    psroddValue.Metal_LowerThreshold = Metal_LowerThreshold;
                    psroddValue.Metal_UpperThreshold = Metal_UpperThreshold;
                    psroddValue.Metal_MinDefectSize = Metal_MinDefectSize;

                    // Nomal
                    psroddValue.Summation_range = Summation_range;
                    psroddValue.Summation_detection_size = Summation_detection_size;
                    psroddValue.Mask_Threshold = Mask_Threshold;
                    psroddValue.Mask_Extension = Mask_Extension;
                    psroddValue.Step_Threshold = Step_Threshold;
                    psroddValue.Step_Expansion = Step_Expansion;

                    //필터
                    psroddValue.HV_ratio_value = HV_ratio_value;
                    psroddValue.Min_Relative_size = Min_Relative_size;
                    psroddValue.Max_Relative_size = Max_Relative_size;
                    psroddValue.Area_Relative = Area_Relative;

                    //Circuit
                    psroddValue.Circuit_Threshold = Circuit_Threshold;
                    psroddValue.Circuit_MinDefectSize = Circuit_MinDefectSize;

                }

                if (m_previewValue == null)
                    m_previewValue = new PSROddProperty();

                //Common
                m_previewValue.ThreshType = threshType;

                //Core_RGB
                m_previewValue.Core_Threshold = Core_Threshold;
                m_previewValue.Core_ExceptionThreshold = Core_ExceptionThreshold;
                m_previewValue.Core_MinDefectSize = Core_MinDefectSize;

                //Metal_채도
                m_previewValue.Metal_LowerThreshold = Metal_LowerThreshold;
                m_previewValue.Metal_UpperThreshold = Metal_UpperThreshold;
                m_previewValue.Metal_MinDefectSize = Metal_MinDefectSize;

                // Nomal
                m_previewValue.Summation_range = Summation_range;
                m_previewValue.Summation_detection_size = Summation_detection_size;
                m_previewValue.Mask_Threshold = Mask_Threshold;
                m_previewValue.Mask_Extension = Mask_Extension;
                m_previewValue.Step_Threshold = Step_Threshold;
                m_previewValue.Step_Expansion = Step_Expansion;

                //필터
                m_previewValue.HV_ratio_value = HV_ratio_value;
                m_previewValue.Min_Relative_size = Min_Relative_size;
                m_previewValue.Max_Relative_size = Max_Relative_size;
                m_previewValue.Area_Relative = Area_Relative;

                //Circuit
                m_previewValue.Circuit_Threshold = Circuit_Threshold;
                m_previewValue.Circuit_MinDefectSize = Circuit_MinDefectSize;
            }
            catch
            {

            }
        }

        public void TrySave(GraphicsBase graphic, int anInspectID, ParametersLocking paradata)
        {
            try
            {
                //Common
                int threshType = (int)GetRadioState(anInspectID);

                //Core_RGB
                int Core_Threshold = Convert.ToInt32(txt_Core_Threshold.Text);
                int Core_ExceptionThreshold = Convert.ToInt32(txt_Core_ExceptionThreshold.Text);
                int Core_MinDefectSize = Convert.ToInt32(txt_Core_MinDefectSize.Text);

                //Metal_채도
                int Metal_LowerThreshold = Convert.ToInt32(txt_Metal_LowerThreshold.Text);
                int Metal_UpperThreshold = Convert.ToInt32(txt_Metal_UpperThreshold.Text);
                int Metal_MinDefectSize = Convert.ToInt32(txtMetal_MinDefectSize.Text);

                // Nomal
                int Summation_range = Convert.ToInt32(txt_Summation_range.Text);
                int Summation_detection_size = Convert.ToInt32(txt_Summation_detection_size.Text);
                int Mask_Threshold = Convert.ToInt32(txtMask_Threshold.Text);
                int Mask_Extension = Convert.ToInt32(txtMask_Extension.Text);
                int Step_Threshold = Convert.ToInt32(txtStep_Threshold.Text);
                int Step_Expansion = Convert.ToInt32(txtStep_Expansion.Text);

                //필터
                int HV_ratio_value = Convert.ToInt32(txtHV_ratio_value.Text);
                int Min_Relative_size = Convert.ToInt32(txtMinRelative_size.Text);
                int Max_Relative_size = Convert.ToInt32(txtMaxRelative_size.Text);
                int Area_Relative = Convert.ToInt32(txtArea_Relative.Text);

                //Circuit
                int Circuit_Threshold = Convert.ToInt32(txtCircuit_Threshold.Text);
                int Circuit_MinDefectSize = Convert.ToInt32(txtCircuit_MinDefectSize.Text);

                PSROddProperty psroddValue = null;

                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        psroddValue = element.InspectionAlgorithm as PSROddProperty;
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (psroddValue == null)
                {
                    PSROddProperty oddProperty = new PSROddProperty();

                    //Common
                    oddProperty.ThreshType = threshType;

                    //Core_RGB
                    oddProperty.Core_Threshold = Core_Threshold;
                    oddProperty.Core_ExceptionThreshold = Core_ExceptionThreshold;
                    oddProperty.Core_MinDefectSize = Core_MinDefectSize;

                    //Metal_채도
                    oddProperty.Metal_LowerThreshold = Metal_LowerThreshold;
                    oddProperty.Metal_UpperThreshold = Metal_UpperThreshold;
                    oddProperty.Metal_MinDefectSize = Metal_MinDefectSize;

                    // Nomal
                    oddProperty.Summation_range = Summation_range;
                    oddProperty.Summation_detection_size = Summation_detection_size;
                    oddProperty.Mask_Threshold = Mask_Threshold;
                    oddProperty.Mask_Extension = Mask_Extension;
                    oddProperty.Step_Threshold = Step_Threshold;
                    oddProperty.Step_Expansion = Step_Expansion;

                    //필터
                    oddProperty.HV_ratio_value = HV_ratio_value;
                    oddProperty.Min_Relative_size = Min_Relative_size;
                    oddProperty.Max_Relative_size = Max_Relative_size;
                    oddProperty.Area_Relative = Area_Relative;

                    //Circuit
                    oddProperty.Circuit_Threshold = Circuit_Threshold;
                    oddProperty.Circuit_MinDefectSize = Circuit_MinDefectSize;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionAlgorithm = oddProperty;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    //Common
                    psroddValue.ThreshType = threshType;

                    //Core_RGB
                    psroddValue.Core_Threshold = Core_Threshold;
                    psroddValue.Core_ExceptionThreshold = Core_ExceptionThreshold;
                    psroddValue.Core_MinDefectSize = Core_MinDefectSize;

                    //Metal_채도
                    psroddValue.Metal_LowerThreshold = Metal_LowerThreshold;
                    psroddValue.Metal_UpperThreshold = Metal_UpperThreshold;
                    psroddValue.Metal_MinDefectSize = Metal_MinDefectSize;

                    // Nomal
                    psroddValue.Summation_range = Summation_range;
                    psroddValue.Summation_detection_size = Summation_detection_size;
                    psroddValue.Mask_Threshold = Mask_Threshold;
                    psroddValue.Mask_Extension = Mask_Extension;
                    psroddValue.Step_Threshold = Step_Threshold;
                    psroddValue.Step_Expansion = Step_Expansion;

                    //필터
                    psroddValue.HV_ratio_value = HV_ratio_value;
                    psroddValue.Min_Relative_size = Min_Relative_size;
                    psroddValue.Max_Relative_size = Max_Relative_size;
                    psroddValue.Area_Relative = Area_Relative;

                    //Circuit
                    psroddValue.Circuit_Threshold = Circuit_Threshold;
                    psroddValue.Circuit_MinDefectSize = Circuit_MinDefectSize;
                }

                if (m_previewValue == null)
                    m_previewValue = new PSROddProperty();

                //Common
                m_previewValue.ThreshType = threshType;

                //Core_RGB
                m_previewValue.Core_Threshold = Core_Threshold;
                m_previewValue.Core_ExceptionThreshold = Core_ExceptionThreshold;
                m_previewValue.Core_MinDefectSize = Core_MinDefectSize;

                //Metal_채도
                m_previewValue.Metal_LowerThreshold = Metal_LowerThreshold;
                m_previewValue.Metal_UpperThreshold = Metal_UpperThreshold;
                m_previewValue.Metal_MinDefectSize = Metal_MinDefectSize;

                // Nomal
                m_previewValue.Summation_range = Summation_range;
                m_previewValue.Summation_detection_size = Summation_detection_size;
                m_previewValue.Mask_Threshold = Mask_Threshold;
                m_previewValue.Mask_Extension = Mask_Extension;
                m_previewValue.Step_Threshold = Step_Threshold;
                m_previewValue.Step_Expansion = Step_Expansion;

                //필터
                m_previewValue.HV_ratio_value = HV_ratio_value;
                m_previewValue.Min_Relative_size = Min_Relative_size;
                m_previewValue.Max_Relative_size = Max_Relative_size;
                m_previewValue.Area_Relative = Area_Relative;

                //Circuit
                m_previewValue.Circuit_Threshold = Circuit_Threshold;
                m_previewValue.Circuit_MinDefectSize = Circuit_MinDefectSize;
            }
            catch
            {

            }
        }

        public void AllCircuitPartChange(GraphicsBase graphic, int anInspectID)
        {
            try
            {
                int Circuit_Threshold = Convert.ToInt32(txtCircuit_Threshold.Text);
                int Circuit_MinDefectSize = Convert.ToInt32(txtCircuit_MinDefectSize.Text);

                PSROddProperty psroddValue = null;

                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        psroddValue = element.InspectionAlgorithm as PSROddProperty;
                        break;
                    }
                }

                if (psroddValue != null)
                {
                    psroddValue.Circuit_Threshold = Circuit_Threshold;
                    psroddValue.Circuit_MinDefectSize = Circuit_MinDefectSize;
                }

                if (m_previewValue == null)
                    m_previewValue = new PSROddProperty();

                m_previewValue.Circuit_Threshold = Circuit_Threshold;
                m_previewValue.Circuit_MinDefectSize = Circuit_MinDefectSize;

            }
            catch
            {

            }
        }
    

        public void AllNonCircuitPartChange(GraphicsBase graphic, int anInspectID)
        {
            try
            {

                //Core_RGB
                int Core_Threshold = Convert.ToInt32(txt_Core_Threshold.Text);
                int Core_ExceptionThreshold = Convert.ToInt32(txt_Core_ExceptionThreshold.Text);
                int Core_MinDefectSize = Convert.ToInt32(txt_Core_MinDefectSize.Text);

                //Metal_채도
                int Metal_LowerThreshold = Convert.ToInt32(txt_Metal_LowerThreshold.Text);
                int Metal_UpperThreshold = Convert.ToInt32(txt_Metal_UpperThreshold.Text);
                int Metal_MinDefectSize = Convert.ToInt32(txtMetal_MinDefectSize.Text);

                // Nomal
                int Summation_range = Convert.ToInt32(txt_Summation_range.Text);
                int Summation_detection_size = Convert.ToInt32(txt_Summation_detection_size.Text);
                int Mask_Threshold = Convert.ToInt32(txtMask_Threshold.Text);
                int Mask_Extension = Convert.ToInt32(txtMask_Extension.Text);
                int Step_Threshold = Convert.ToInt32(txtStep_Threshold.Text);
                int Step_Expansion = Convert.ToInt32(txtStep_Expansion.Text);

                //필터
                int HV_ratio_value = Convert.ToInt32(txtHV_ratio_value.Text);
                int Min_Relative_size = Convert.ToInt32(txtMinRelative_size.Text);
                int Max_Relative_size = Convert.ToInt32(txtMaxRelative_size.Text);
                int Area_Relative = Convert.ToInt32(txtArea_Relative.Text);
        

                PSROddProperty psroddValue = null;

                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        psroddValue = element.InspectionAlgorithm as PSROddProperty;
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (psroddValue == null)
                {
                    PSROddProperty oddProperty = new PSROddProperty();

                    //Core_RGB
                    oddProperty.Core_Threshold = Core_Threshold;
                    oddProperty.Core_ExceptionThreshold = Core_ExceptionThreshold;
                    oddProperty.Core_MinDefectSize = Core_MinDefectSize;

                    //Metal_채도
                    oddProperty.Metal_LowerThreshold = Metal_LowerThreshold;
                    oddProperty.Metal_UpperThreshold = Metal_UpperThreshold;
                    oddProperty.Metal_MinDefectSize = Metal_MinDefectSize;

                    // Nomal
                    oddProperty.Summation_range = Summation_range;
                    oddProperty.Summation_detection_size = Summation_detection_size;
                    oddProperty.Mask_Threshold = Mask_Threshold;
                    oddProperty.Mask_Extension = Mask_Extension;
                    oddProperty.Step_Threshold = Step_Threshold;
                    oddProperty.Step_Expansion = Step_Expansion;

                    //필터
                    oddProperty.HV_ratio_value = HV_ratio_value;
                    oddProperty.Min_Relative_size = Min_Relative_size;
                    oddProperty.Max_Relative_size = Max_Relative_size;
                    oddProperty.Area_Relative = Area_Relative;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionAlgorithm = oddProperty;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    //Core_RGB
                    psroddValue.Core_Threshold = Core_Threshold;
                    psroddValue.Core_ExceptionThreshold = Core_ExceptionThreshold;
                    psroddValue.Core_MinDefectSize = Core_MinDefectSize;

                    //Metal_채도
                    psroddValue.Metal_LowerThreshold = Metal_LowerThreshold;
                    psroddValue.Metal_UpperThreshold = Metal_UpperThreshold;
                    psroddValue.Metal_MinDefectSize = Metal_MinDefectSize;

                    // Nomal
                    psroddValue.Summation_range = Summation_range;
                    psroddValue.Summation_detection_size = Summation_detection_size;
                    psroddValue.Mask_Threshold = Mask_Threshold;
                    psroddValue.Mask_Extension = Mask_Extension;
                    psroddValue.Step_Threshold = Step_Threshold;
                    psroddValue.Step_Expansion = Step_Expansion;


                    //필터
                    psroddValue.HV_ratio_value = HV_ratio_value;
                    psroddValue.Min_Relative_size = Min_Relative_size;
                    psroddValue.Max_Relative_size = Max_Relative_size;
                    psroddValue.Area_Relative = Area_Relative;
                }

                if (m_previewValue == null)
                    m_previewValue = new PSROddProperty();

                //Core_RGB
                m_previewValue.Core_Threshold = Core_Threshold;
                m_previewValue.Core_ExceptionThreshold = Core_ExceptionThreshold;
                m_previewValue.Core_MinDefectSize = Core_MinDefectSize;

                //Metal_채도
                m_previewValue.Metal_LowerThreshold = Metal_LowerThreshold;
                m_previewValue.Metal_UpperThreshold = Metal_UpperThreshold;
                m_previewValue.Metal_MinDefectSize = Metal_MinDefectSize;

                // Nomal
                m_previewValue.Summation_range = Summation_range;
                m_previewValue.Summation_detection_size = Summation_detection_size;
                m_previewValue.Mask_Threshold = Mask_Threshold;
                m_previewValue.Mask_Extension = Mask_Extension;
                m_previewValue.Step_Threshold = Step_Threshold;
                m_previewValue.Step_Expansion = Step_Expansion;

                //필터
                m_previewValue.HV_ratio_value = HV_ratio_value;
                m_previewValue.Min_Relative_size = Min_Relative_size;
                m_previewValue.Max_Relative_size = Max_Relative_size;
                m_previewValue.Area_Relative = Area_Relative;
            }
            catch
            {

            }
        }
    }
}
