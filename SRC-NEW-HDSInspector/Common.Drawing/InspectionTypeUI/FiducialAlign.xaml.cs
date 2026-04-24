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
    /// <summary>   Interaction logic for StripAlign.xaml. </summary>
    /// <remarks>   suoow2, 2014-09-24. </remarks>
    public partial class FiducialAlign : UserControl, IInspectionTypeUICommands
    {
        private FiducialAlignProperty m_previewValue;

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

        public FiducialAlign()
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

                FiducialAlignProperty oldFiducialAlignValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.InspectionType != null && element.ID == anInspectID)
                    {
                        oldFiducialAlignValue = element.InspectionAlgorithm as FiducialAlignProperty; // 기존에 저장된 값이 있는 경우.
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
                    FiducialAlignProperty fiducialAlignValue = new FiducialAlignProperty();
                    fiducialAlignValue.AlignMarginX = alignMarginX;
                    fiducialAlignValue.AlignMarginY = alignMarginY;
                    fiducialAlignValue.AlignAcceptance = alignAcceptance;

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
                    oldFiducialAlignValue.AlignMarginX = alignMarginX;
                    oldFiducialAlignValue.AlignMarginY = alignMarginY;
                    oldFiducialAlignValue.AlignAcceptance = alignAcceptance;
                }

                if (m_previewValue == null)
                    m_previewValue = new FiducialAlignProperty();
                m_previewValue.AlignMarginX = alignMarginX;
                m_previewValue.AlignMarginY = alignMarginY;
                m_previewValue.AlignAcceptance = alignAcceptance;
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

                FiducialAlignProperty oldFiducialAlignValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.InspectionType != null && element.ID == anInspectID)
                    {
                        oldFiducialAlignValue = element.InspectionAlgorithm as FiducialAlignProperty; // 기존에 저장된 값이 있는 경우.
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
                    FiducialAlignProperty fiducialAlignValue = new FiducialAlignProperty();
                    fiducialAlignValue.AlignMarginX = alignMarginX;
                    fiducialAlignValue.AlignMarginY = alignMarginY;
                    fiducialAlignValue.AlignAcceptance = alignAcceptance;

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
                    oldFiducialAlignValue.AlignMarginX = alignMarginX;
                    oldFiducialAlignValue.AlignMarginY = alignMarginY;
                    oldFiducialAlignValue.AlignAcceptance = alignAcceptance;
                }

                if (m_previewValue == null)
                    m_previewValue = new FiducialAlignProperty();
                m_previewValue.AlignMarginX = alignMarginX;
                m_previewValue.AlignMarginY = alignMarginY;
                m_previewValue.AlignAcceptance = alignAcceptance;
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
                this.txtAlignMarginX.Text = m_previewValue.AlignMarginX.ToString();
                this.txtAlignMarginY.Text = m_previewValue.AlignMarginY.ToString();
                this.txtAlignAcceptance.Text = m_previewValue.AlignAcceptance.ToString();
            }
        }

        // 검사 설정 기본 값 표시.
        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeGlobalAlign)
            {
                if (!StripAlignDefaultValue.DefaultValueLoaded)
                {
                    StripAlignDefaultValue.DefaultValueLoaded = true;
                    StripAlignDefaultValue.LoadDefaultValue();
                }

                this.txtAlignMarginX.Text = StripAlignDefaultValue.AlignMarginX.ToString();
                this.txtAlignMarginY.Text = StripAlignDefaultValue.AlignMarginY.ToString();
                this.txtAlignAcceptance.Text = StripAlignDefaultValue.AlignAcceptance.ToString();
            }
            else //if(m_enumInspectType == eVisInspectType.eInspTypeUnitAlign)
            {
                if (!UnitAlignDefaultValue.DefaultValueLoaded)
                {
                    UnitAlignDefaultValue.LoadDefaultValue();
                }

                this.txtAlignMarginX.Text = UnitAlignDefaultValue.AlignMarginX.ToString();
                this.txtAlignMarginY.Text = UnitAlignDefaultValue.AlignMarginY.ToString();
                this.txtAlignAcceptance.Text = UnitAlignDefaultValue.AlignAcceptance.ToString();
            }
        }

        // 검사 설정 표시.
        public void Display(InspectionItem inspectionItem, int MarginX, int MatginY)
        {
            // Data를 View에 보여준다.
            FiducialAlignProperty fiducialAlignValue = inspectionItem.InspectionAlgorithm as FiducialAlignProperty;
            if (fiducialAlignValue != null)
            {
                this.txtAlignMarginX.Text = fiducialAlignValue.AlignMarginX.ToString();
                this.txtAlignMarginY.Text = fiducialAlignValue.AlignMarginY.ToString();
                this.txtAlignAcceptance.Text = fiducialAlignValue.AlignAcceptance.ToString();
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
