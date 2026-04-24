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
    /// <summary>   Exceptional mask property.  </summary>
    /// <remarks>   suoow2, 2014-10-09. </remarks>
    public partial class ExceptionalMask : UserControl, IInspectionTypeUICommands
    {
        private ExceptionalMaskProperty m_previewValue;

        public ExceptionalMask()
        {
            InitializeComponent();
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            this.cbUseOffset.Items.Clear();
            this.cbUseOffset.Items.Add("사용안함");
            this.cbUseOffset.Items.Add("Shape Shift");
            this.cbUseOffset.SelectedIndex = 0;
        }

        public void SetDialog(string strCaption, eVisInspectType inspectType)
        {
            this.txtCaption.Text = strCaption;
        }

        public void SetMaxValue(ref List<int> columnList, ref List<int> rowList)
        {
            this.cbExceptionY.Items.Clear();
            this.cbExceptionY.Items.Add("전체");
            foreach (int value in rowList)
            {
                this.cbExceptionY.Items.Add((value + 1).ToString());
            }
            this.cbExceptionY.SelectedIndex = 0;

            this.cbExceptionX.Items.Clear();
            this.cbExceptionX.Items.Add("전체");
            foreach (int value in columnList)
            {
                this.cbExceptionX.Items.Add((value + 1).ToString());
            }
            this.cbExceptionX.SelectedIndex = 0;
        }

        // 검사 설정 저장.
        public void TrySave(GraphicsBase graphic, int anInspectID, ParametersLocking paradata)
        {
            try
            {
                int applyExceptionY = Convert.ToInt32(this.cbExceptionY.SelectedIndex) - 1;
                int applyExceptionX = Convert.ToInt32(this.cbExceptionX.SelectedIndex) - 1;
                int applyUseShapeShift;
                switch (cbUseOffset.SelectedIndex)
                {
                    case 0:
                        applyUseShapeShift = 0;
                        // 다른 Offset 사용 여부
                        break;
                    case 1:
                        applyUseShapeShift = 3;
                        break;
                    default:
                        throw (new Exception());
                }

                ExceptionalMaskProperty oldExceptionalMaskValue = null;
                bool newItem = true;

                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.InspectionType != null)//element.InspectionType.Name == this.lblCaption.Content.ToString())
                    {
                        if (element.ID == anInspectID)
                        {
                            newItem = false;
                            oldExceptionalMaskValue = element.InspectionAlgorithm as ExceptionalMaskProperty;
                            break;
                        }

                        if (element.InspectionType.Name == this.txtCaption.Text.ToString())
                        {
                            oldExceptionalMaskValue = element.InspectionAlgorithm as ExceptionalMaskProperty; // 기존에 저장된 값이 있는 경우.
                        }
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (newItem) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    ExceptionalMaskProperty ExceptionalMaskValue = new ExceptionalMaskProperty();
                    ExceptionalMaskValue.ExceptionX = applyExceptionX;
                    ExceptionalMaskValue.ExceptionY = applyExceptionY;
                    ExceptionalMaskValue.UseShapeShift = applyUseShapeShift;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(eVisInspectType.eInspTypeExceptionalMask);
                    inspectionItem.InspectionAlgorithm = ExceptionalMaskValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldExceptionalMaskValue.ExceptionX = applyExceptionX;
                    oldExceptionalMaskValue.ExceptionY = applyExceptionY;
                    oldExceptionalMaskValue.UseShapeShift = applyUseShapeShift;
                }

                if (m_previewValue == null)
                    m_previewValue = new ExceptionalMaskProperty();
                m_previewValue.ExceptionX = applyExceptionX;
                m_previewValue.ExceptionY = applyExceptionY;
                m_previewValue.UseShapeShift = applyUseShapeShift;

                graphic.Caption = CaptionHelper.ExceptionalMaskCaption;
                graphic.ObjectColor = GraphicsColors.Blue;
                graphic.RegionType = GraphicsRegionType.Except;
            }
            catch
            {
                MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
                cbExceptionX.Focus();
            }
        }

        // 검사 설정 저장.
        public void TryAdd(ref InspectList list, int anInspectID)
        {
            try
            {
                int applyExceptionY = Convert.ToInt32(this.cbExceptionY.SelectedIndex) - 1;
                int applyExceptionX = Convert.ToInt32(this.cbExceptionX.SelectedIndex) - 1;
                int applyUseShapeShift;
                switch (cbUseOffset.SelectedIndex)
                {
                    case 0:
                        applyUseShapeShift = 0;
                        // 다른 Offset 사용 여부
                        break;
                    case 1:
                        applyUseShapeShift = 3;
                        break;
                    default:
                        throw (new Exception());
                }

                ExceptionalMaskProperty oldExceptionalMaskValue = null;
                bool newItem = true;

                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.InspectionType != null)//element.InspectionType.Name == this.lblCaption.Content.ToString())
                    {
                        if (element.ID == anInspectID)
                        {
                            newItem = false;
                            oldExceptionalMaskValue = element.InspectionAlgorithm as ExceptionalMaskProperty;
                            break;
                        }

                        if (element.InspectionType.Name == this.txtCaption.Text.ToString())
                        {
                            oldExceptionalMaskValue = element.InspectionAlgorithm as ExceptionalMaskProperty; // 기존에 저장된 값이 있는 경우.
                        }
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (newItem) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    ExceptionalMaskProperty ExceptionalMaskValue = new ExceptionalMaskProperty();
                    ExceptionalMaskValue.ExceptionX = applyExceptionX;
                    ExceptionalMaskValue.ExceptionY = applyExceptionY;
                    ExceptionalMaskValue.UseShapeShift = applyUseShapeShift;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(eVisInspectType.eInspTypeExceptionalMask);
                    inspectionItem.InspectionAlgorithm = ExceptionalMaskValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldExceptionalMaskValue.ExceptionX = applyExceptionX;
                    oldExceptionalMaskValue.ExceptionY = applyExceptionY;
                    oldExceptionalMaskValue.UseShapeShift = applyUseShapeShift;
                }

                if (m_previewValue == null)
                    m_previewValue = new ExceptionalMaskProperty();
                m_previewValue.ExceptionX = applyExceptionX;
                m_previewValue.ExceptionY = applyExceptionY;
                m_previewValue.UseShapeShift = applyUseShapeShift;

            }
            catch
            {
              //  MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
              //  cbExceptionX.Focus();
            }
        }

        // 검사 설정 직전 값 표시.
        public void SetPreviewValue()
        {
            if (!ExceptionalMaskDefaultValue.DefaultValueLoaded)
            {
                ExceptionalMaskDefaultValue.DefaultValueLoaded = true;
                ExceptionalMaskDefaultValue.LoadDefaultValue();
            }

            if (m_previewValue != null)
            {
                this.cbExceptionX.SelectedIndex = (m_previewValue.ExceptionX + 1);
                this.cbExceptionY.SelectedIndex = (m_previewValue.ExceptionY + 1);
            }

            if (ExceptionalMaskDefaultValue.UseShapeShift != 0)
            {
                cbUseOffset.SelectedIndex = 1;
            }
        }

        // 검사 설정 기본 값 표시.
        public void SetDefaultValue()
        {
            if (!ExceptionalMaskDefaultValue.DefaultValueLoaded)
            {
                ExceptionalMaskDefaultValue.DefaultValueLoaded = true;
                ExceptionalMaskDefaultValue.LoadDefaultValue();
            }

            this.cbExceptionX.SelectedIndex = (ExceptionalMaskDefaultValue.ExceptionX + 1);
            this.cbExceptionY.SelectedIndex = (ExceptionalMaskDefaultValue.ExceptionY + 1);

            if (ExceptionalMaskDefaultValue.UseShapeShift != 0)
            {
                cbUseOffset.SelectedIndex = 1;
            }
        }

        // 검사 설정 표시.
        public void Display(InspectionItem inspectionItem, int MarginX, int MatginY)
        {
            // Data를 View에 보여준다.
            ExceptionalMaskProperty exceptionalMaskValue = inspectionItem.InspectionAlgorithm as ExceptionalMaskProperty;
            if (exceptionalMaskValue != null)
            {
                this.cbExceptionX.SelectedIndex = (exceptionalMaskValue.ExceptionX + 1);
                this.cbExceptionY.SelectedIndex = (exceptionalMaskValue.ExceptionY + 1);
                if (exceptionalMaskValue.UseShapeShift != 0)
                    this.cbUseOffset.SelectedIndex = 1;
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
