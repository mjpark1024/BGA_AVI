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
using Common.DataBase;
using Common.Drawing.InspectionInformation;

namespace Common.Drawing.InspectionTypeUI
{
    public class LeadGapProperty : InspectionAlgorithm
    {
        public int LowerThresh;             // 0001
        public int UpperThresh;             // 0002
        public int MinWidthSize;            // 0031

        public LeadGapProperty()
        {
            Code = "3017";
        }

        public override InspectionAlgorithm Clone()
        {
            LeadGapProperty cloneLeadGap = new LeadGapProperty();

            cloneLeadGap.LowerThresh = this.LowerThresh;
            cloneLeadGap.UpperThresh = this.UpperThresh;
            cloneLeadGap.MinWidthSize = this.MinWidthSize;

            return cloneLeadGap;
        }

        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            LowerThresh  = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UpperThresh  = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            MinWidthSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0031");
        }

        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001 ", LowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002 ", UpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0031 ", MinWidthSize);
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
    }
    /// <summary>
    /// Interaction logic for Lead_Gap.xaml
    /// </summary>
    public partial class Lead_Gap : UserControl, IInspectionTypeUICommands
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

        public Lead_Gap()
        {
            InitializeComponent();
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
                int lowerThresh = Convert.ToInt32(txtLowerThreshold.Text);
                int upperThresh = Convert.ToInt32(txtUpperThreshold.Text);

                if (lowerThresh > upperThresh)
                {
                    MessageBox.Show("임계 하한 값은 임계 상한 값을 초과할 수 없습니다.", "Information");
                    this.txtLowerThreshold.Focus();
                    return;
                }

                int minWidthSize = Convert.ToInt32(txtMinWidthSize.Text);

                LeadGapProperty oldLeadGapValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldLeadGapValue = element.InspectionAlgorithm as LeadGapProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldLeadGapValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    LeadGapProperty leadGapValue = new LeadGapProperty();

                    leadGapValue.LowerThresh = lowerThresh;
                    leadGapValue.UpperThresh = upperThresh;
                    leadGapValue.MinWidthSize = minWidthSize;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = leadGapValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldLeadGapValue.LowerThresh = lowerThresh;
                    oldLeadGapValue.UpperThresh = upperThresh;
                    oldLeadGapValue.MinWidthSize = minWidthSize;
                }
            }
            catch
            {
                MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
                txtLowerThreshold.Focus();
            }
        }

        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeLeadGap)
            {
                if (!LeadGapDefaultValue.DefaultValueLoaded)
                {
                    LeadGapDefaultValue.DefaultValueLoaded = true;
                    LeadGapDefaultValue.LoadDefaultValue();
                }

                this.txtLowerThreshold.Text = LeadGapDefaultValue.LowerThresh.ToString();
                this.txtUpperThreshold.Text = LeadGapDefaultValue.UpperThresh.ToString();
                this.txtMinWidthSize.Text = LeadGapDefaultValue.MinWidthSize.ToString();
            }
        }

        public void Display(InspectionItem settingValue)
        {
            LeadGapProperty leadGapProperty = settingValue.InspectionAlgorithm as LeadGapProperty;
            if (leadGapProperty != null)
            {
                this.txtLowerThreshold.Text = leadGapProperty.LowerThresh.ToString();
                this.txtUpperThreshold.Text = leadGapProperty.UpperThresh.ToString();
                this.txtMinWidthSize.Text = leadGapProperty.MinWidthSize.ToString();
            }
        }
    }
}
