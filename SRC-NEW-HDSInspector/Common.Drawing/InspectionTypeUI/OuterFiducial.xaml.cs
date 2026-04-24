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

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>   Interaction logic for Outer Fiducial.xaml. </summary>
    /// <remarks>   suoow2, 2020-02-20. </remarks>
    public partial class OuterFiducial : UserControl, IInspectionTypeUICommands
    {
        private OuterFiducialProperty m_previewValue;

        #region InspectType
        /// <summary>   Gets the type of the inspection. </summary>
        /// <value> The type of the inspection. </value>
        public eVisInspectType InspectType
        {
            get
            {
                return m_enumInspectType;
            }
        }
        private eVisInspectType m_enumInspectType;
        #endregion

 
        public OuterFiducial()
        {
            InitializeComponent();
        }


        public void SetDialog(string strCaption, eVisInspectType inspectType)
        {
            this.txtCaption.Text = strCaption;
            this.m_enumInspectType = inspectType;
        }

        // 검사 설정 저장.
        public void TrySave(GraphicsBase graphic, int anInspectID, ParametersLocking paradata)
        {
            try
            {
                int alignMarginX = Convert.ToInt32(this.txtAlignMarginX.Text);
                int alignMarginY = Convert.ToInt32(this.txtAlignMarginY.Text);
                int alignAcceptance = Convert.ToInt32(this.txtAlignAcceptance.Text);

                OuterFiducialProperty oldFiducialAlignValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.InspectionType != null && element.ID == anInspectID)
                    {
                        oldFiducialAlignValue = element.InspectionAlgorithm as OuterFiducialProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldFiducialAlignValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    OuterFiducialProperty fiducialAlignValue = new OuterFiducialProperty();
                    fiducialAlignValue.MarginX = alignMarginX;
                    fiducialAlignValue.MarginY = alignMarginY;
                    fiducialAlignValue.Acceptance = alignAcceptance;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = fiducialAlignValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldFiducialAlignValue.MarginX = alignMarginX;
                    oldFiducialAlignValue.MarginY = alignMarginY;
                    oldFiducialAlignValue.Acceptance = alignAcceptance;
                }

                if (m_previewValue == null)
                    m_previewValue = new OuterFiducialProperty();
                m_previewValue.MarginX = alignMarginX;
                m_previewValue.MarginY = alignMarginY;
                m_previewValue.Acceptance = alignAcceptance;
            }
            catch
            {
                MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
                txtAlignMarginX.Focus();
            }
        }

        // 검사 설정 저장.
        public void TryAdd(ref InspectList list, int anInspectID)
        {
            try
            {
                int alignMarginX = Convert.ToInt32(this.txtAlignMarginX.Text);
                int alignMarginY = Convert.ToInt32(this.txtAlignMarginY.Text);
                int alignAcceptance = Convert.ToInt32(this.txtAlignAcceptance.Text);

                OuterFiducialProperty oldFiducialAlignValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.InspectionType != null && element.ID == anInspectID)
                    {
                        oldFiducialAlignValue = element.InspectionAlgorithm as OuterFiducialProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldFiducialAlignValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    OuterFiducialProperty fiducialAlignValue = new OuterFiducialProperty();
                    fiducialAlignValue.MarginX = alignMarginX;
                    fiducialAlignValue.MarginY = alignMarginY;
                    fiducialAlignValue.Acceptance = alignAcceptance;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = fiducialAlignValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldFiducialAlignValue.MarginX = alignMarginX;
                    oldFiducialAlignValue.MarginY = alignMarginY;
                    oldFiducialAlignValue.Acceptance = alignAcceptance;
                }

                if (m_previewValue == null)
                    m_previewValue = new OuterFiducialProperty();
                m_previewValue.MarginX = alignMarginX;
                m_previewValue.MarginY = alignMarginY;
                m_previewValue.Acceptance = alignAcceptance;
            }
            catch
            {
                //   MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
                //   txtAlignMarginX.Focus();
            }
        }

        // 검사 설정 직전 값 표시.
        public void SetPreviewValue()
        {
            if (m_previewValue != null)
            {
                this.txtAlignMarginX.Text = m_previewValue.MarginX.ToString();
                this.txtAlignMarginY.Text = m_previewValue.MarginY.ToString();
                this.txtAlignAcceptance.Text = m_previewValue.Acceptance.ToString();
            }
        }

        // 검사 설정 기본 값 표시.
        public void SetDefaultValue()
        {
            if (!OuterFiducialDefaultValue.DefaultValueLoaded)
            {
                OuterFiducialDefaultValue.DefaultValueLoaded = true;
                OuterFiducialDefaultValue.LoadDefaultValue();
            }

            this.txtAlignMarginX.Text = OuterFiducialDefaultValue.MarginX.ToString();
            this.txtAlignMarginY.Text = OuterFiducialDefaultValue.MarginY.ToString();
            this.txtAlignAcceptance.Text = OuterFiducialDefaultValue.Acceptance.ToString();
        }

        // 검사 설정 표시.
        public void Display(InspectionItem inspectionItem, int MarginX, int MatginY)
        {
            // Data를 View에 보여준다.
            OuterFiducialProperty fiducialAlignValue = inspectionItem.InspectionAlgorithm as OuterFiducialProperty;
            if (fiducialAlignValue != null)
            {
                this.txtAlignMarginX.Text = fiducialAlignValue.MarginX.ToString();
                this.txtAlignMarginY.Text = fiducialAlignValue.MarginY.ToString();
                this.txtAlignAcceptance.Text = fiducialAlignValue.Acceptance.ToString();
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
