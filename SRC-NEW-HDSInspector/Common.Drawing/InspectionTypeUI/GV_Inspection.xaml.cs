using Common.Drawing.InspectionInformation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>
    /// GV_Inspection.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GV_Inspection : UserControl, IInspectionTypeUICommands
    {
        private GV_Inspection_Property m_previewValue;

        private eVisInspectType m_enumInspectType;
        public eVisInspectType InspectType
        {
            get
            {
                return m_enumInspectType;
            }
        }


        public GV_Inspection()
        {
            InitializeComponent();
        }

        public void SetDialog(string strCaption, eVisInspectType inspectType)
        {
            this.txtCaption.Text = strCaption;
            this.m_enumInspectType = inspectType;
        }

        public void TrySave(GraphicsBase graphic, int anInspectID, ParametersLocking paradata)
        {
            try
            {
                int Ball_Thresh = Convert.ToInt32(txtBall_Thresh.Text);
                int Core_Thresh = Convert.ToInt32(txtCore_Thresh.Text);
                int Mask = Convert.ToInt32(txtMask.Text);
                int Taget_GV = Convert.ToInt32(txtTaget_GV.Text);
                int GV_Margin = Convert.ToInt32(txtGV_Margin.Text);


                GV_Inspection_Property old_gv_Inspection = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        old_gv_Inspection = element.InspectionAlgorithm as GV_Inspection_Property; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (old_gv_Inspection == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    GV_Inspection_Property gv_Inspection = new GV_Inspection_Property();

                    gv_Inspection.Ball_Thresh = Ball_Thresh;
                    gv_Inspection.Core_Thresh = Core_Thresh;
                    gv_Inspection.Mask = Mask;
                    gv_Inspection.Taget_GV = Taget_GV;
                    gv_Inspection.GV_Margin = GV_Margin;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = gv_Inspection;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    old_gv_Inspection.Ball_Thresh = Ball_Thresh;
                    old_gv_Inspection.Core_Thresh = Core_Thresh;
                    old_gv_Inspection.Mask = Mask;
                    old_gv_Inspection.Taget_GV = Taget_GV;
                    old_gv_Inspection.GV_Margin = GV_Margin;

                }

                if (m_previewValue == null)
                    m_previewValue = new GV_Inspection_Property();

                m_previewValue.Ball_Thresh = Ball_Thresh;
                m_previewValue.Core_Thresh = Core_Thresh;
                m_previewValue.Mask = Mask;
                m_previewValue.Taget_GV = Taget_GV;
                m_previewValue.GV_Margin = GV_Margin;
            }
            catch
            {
                MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
            }
        }

        // 검사 설정 저장.
        public void TryAdd(ref InspectList list, int anInspectID)
        {
            try
            {

                int Ball_Thresh = Convert.ToInt32(txtBall_Thresh.Text);
                int Core_Thresh = Convert.ToInt32(txtCore_Thresh.Text);
                int Mask = Convert.ToInt32(txtMask.Text);
                int Taget_GV = Convert.ToInt32(txtTaget_GV.Text);
                int GV_Margin = Convert.ToInt32(txtGV_Margin.Text);


                GV_Inspection_Property old_gv_Inspection = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        old_gv_Inspection = element.InspectionAlgorithm as GV_Inspection_Property; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (old_gv_Inspection == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    GV_Inspection_Property gv_Inspection = new GV_Inspection_Property();

                    gv_Inspection.Ball_Thresh = Ball_Thresh;
                    gv_Inspection.Core_Thresh = Core_Thresh;
                    gv_Inspection.Mask = Mask;
                    gv_Inspection.Taget_GV = Taget_GV;
                    gv_Inspection.GV_Margin = GV_Margin;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = gv_Inspection;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    old_gv_Inspection.Ball_Thresh = Ball_Thresh;
                    old_gv_Inspection.Core_Thresh = Core_Thresh;
                    old_gv_Inspection.Mask = Mask;
                    old_gv_Inspection.Taget_GV = Taget_GV;
                    old_gv_Inspection.GV_Margin = GV_Margin;

                }

                if (m_previewValue == null)
                    m_previewValue = new GV_Inspection_Property();

                m_previewValue.Ball_Thresh = Ball_Thresh;
                m_previewValue.Core_Thresh = Core_Thresh;
                m_previewValue.Mask = Mask;
                m_previewValue.Taget_GV = Taget_GV;
                m_previewValue.GV_Margin = GV_Margin;
            }

            catch
            {
                MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
            }

        }

        // 검사 설정 직전 값 표시.
        public void SetPreviewValue()
        {
            this.txtBall_Thresh.Text = m_previewValue.Ball_Thresh.ToString();
            this.txtCore_Thresh.Text = m_previewValue.Core_Thresh.ToString();
            this.txtMask.Text = m_previewValue.Mask.ToString();
            this.txtTaget_GV.Text = m_previewValue.Taget_GV.ToString();
            this.txtGV_Margin.Text = m_previewValue.GV_Margin.ToString();
        }

        // 검사 설정 기본 값 표시.
        public void SetDefaultValue()
        {

            if (!GV_Inspection_DefaultValue.DefaultValueLoaded)
            {
                GV_Inspection_DefaultValue.DefaultValueLoaded = true;
                GV_Inspection_DefaultValue.LoadDefaultValue();
            }

            this.txtBall_Thresh.Text = GV_Inspection_DefaultValue.Ball_Thresh.ToString();
            this.txtCore_Thresh.Text = GV_Inspection_DefaultValue.Core_Thresh.ToString();
            this.txtMask.Text = GV_Inspection_DefaultValue.Mask.ToString();
            this.txtTaget_GV.Text = GV_Inspection_DefaultValue.Taget_GV.ToString();
            this.txtGV_Margin.Text = GV_Inspection_DefaultValue.GV_Margin.ToString();


        }

        // 검사 설정 표시.
        public void Display(InspectionItem inspectionItem, int MarginX, int MatginY)
        {
            GV_Inspection_Property Property = inspectionItem.InspectionAlgorithm as GV_Inspection_Property;
            if (Property != null)
            {
                this.txtBall_Thresh.Text = Property.Ball_Thresh.ToString();
                this.txtCore_Thresh.Text = Property.Core_Thresh.ToString();
                this.txtMask.Text = Property.Mask.ToString();
                this.txtTaget_GV.Text = Property.Taget_GV.ToString();
                this.txtGV_Margin.Text = Property.GV_Margin.ToString();

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
