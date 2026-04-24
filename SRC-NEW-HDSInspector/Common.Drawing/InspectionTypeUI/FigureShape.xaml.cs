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
    public partial class FigureShape : UserControl, IInspectionTypeUICommands
    {
        private FigureShapeProperty m_previewValue;

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

        public FigureShape()
        {
            InitializeComponent();
        }

        public void SetDialog(string strCaption, InspectionInformation.eVisInspectType inspectType)
        {
            this.txtCaption.Text = strCaption;
            this.m_enumInspectType = inspectType;
        }

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

        // 검사 설정 저장.
        public void TrySave(GraphicsBase graphic, int anInspectID, ParametersLocking paradata)
        {
            try
            {
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
                int minWidthSize = Convert.ToInt32(txtMinWidthSize.Text);
                int minHeightSize = Convert.ToInt32(txtMinHeightSize.Text);
                int minNormalRatio = Convert.ToInt32(txtMinNormalRatio.Text);
                int maxNormalRatio = Convert.ToInt32(txtMaxNormalRatio.Text);
                int alignMaxDist = Convert.ToInt32(txtAlignMaxDist.Text);
                int darkAreaWidth = Convert.ToInt32(txtDarkAreaWidth.Text);

                FigureShapeProperty oldFigureShapeValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldFigureShapeValue = element.InspectionAlgorithm as FigureShapeProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldFigureShapeValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    FigureShapeProperty figureShapeValue = new FigureShapeProperty();

                    figureShapeValue.InspRange = inspRange;
                    figureShapeValue.AverMinMargin = averMinMargin;
                    figureShapeValue.AverMaxMargin = averMaxMargin;
                    figureShapeValue.MinMargin = minMargin;
                    figureShapeValue.MaxMargin = maxMargin;
                    figureShapeValue.ErosionTrainIter = erosionTrainIter;
                    figureShapeValue.DilationTrainIter = dilationTrainIter;
                    figureShapeValue.ErosionInspIter = erosionInspIter;
                    figureShapeValue.DilationInspIter = dilationInspIter;
                    figureShapeValue.MinWidthSize = minWidthSize;
                    figureShapeValue.MinHeightSize = minHeightSize;
                    figureShapeValue.MinNormalRatio = minNormalRatio;
                    figureShapeValue.MaxNormalRatio = maxNormalRatio;
                    figureShapeValue.AlignMaxDist = alignMaxDist;
                    figureShapeValue.DarkAreaWidth = darkAreaWidth;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = figureShapeValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldFigureShapeValue.InspRange = inspRange;
                    oldFigureShapeValue.AverMinMargin = averMinMargin;
                    oldFigureShapeValue.AverMaxMargin = averMaxMargin;
                    oldFigureShapeValue.MinMargin = minMargin;
                    oldFigureShapeValue.MaxMargin = maxMargin;
                    oldFigureShapeValue.ErosionTrainIter = erosionTrainIter;
                    oldFigureShapeValue.DilationTrainIter = dilationTrainIter;
                    oldFigureShapeValue.ErosionInspIter = erosionInspIter;
                    oldFigureShapeValue.DilationInspIter = dilationInspIter;
                    oldFigureShapeValue.MinWidthSize = minWidthSize;
                    oldFigureShapeValue.MinHeightSize = minHeightSize;
                    oldFigureShapeValue.MinNormalRatio = minNormalRatio;
                    oldFigureShapeValue.MaxNormalRatio = maxNormalRatio;
                    oldFigureShapeValue.AlignMaxDist = alignMaxDist;
                    oldFigureShapeValue.DarkAreaWidth = darkAreaWidth;
                }

                if (m_previewValue == null)
                    m_previewValue = new FigureShapeProperty();
                m_previewValue.InspRange = inspRange;
                m_previewValue.AverMinMargin = averMinMargin;
                m_previewValue.AverMaxMargin = averMaxMargin;
                m_previewValue.MinMargin = minMargin;
                m_previewValue.MaxMargin = maxMargin;
                m_previewValue.ErosionTrainIter = erosionTrainIter;
                m_previewValue.DilationTrainIter = dilationTrainIter;
                m_previewValue.ErosionInspIter = erosionInspIter;
                m_previewValue.DilationInspIter = dilationInspIter;
                m_previewValue.MinWidthSize = minWidthSize;
                m_previewValue.MinHeightSize = minHeightSize;
                m_previewValue.MinNormalRatio = minNormalRatio;
                m_previewValue.MaxNormalRatio = maxNormalRatio;
                m_previewValue.AlignMaxDist = alignMaxDist;
                m_previewValue.DarkAreaWidth = darkAreaWidth;
            }
            catch
            {
                MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
                txtAverMinMargin.Focus();
            }
        }

        // 검사 설정 저장.
        public void TryAdd(ref InspectList list, int anInspectID)
        {
            try
            {
                #region Check 평균값 최소 / 최대 허용범위
                int averMinMargin = Convert.ToInt32(txtAverMinMargin.Text);
                int averMaxMargin = Convert.ToInt32(txtAverMaxMargin.Text);

                #endregion

                #region Check 검사 최소 / 최대 허용범위
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

                int minMargin = Convert.ToInt32(txtMinMargin.Text);
                int maxMargin = Convert.ToInt32(txtMaxMargin.Text);

                #endregion

                int erosionTrainIter = Convert.ToInt32(txtErosionTrainIter.Text);
                int dilationTrainIter = Convert.ToInt32(txtDilationTrainIter.Text);
                int erosionInspIter = Convert.ToInt32(txtErosionInspIter.Text);
                int dilationInspIter = Convert.ToInt32(txtDilationInspIter.Text);
                int minWidthSize = Convert.ToInt32(txtMinWidthSize.Text);
                int minHeightSize = Convert.ToInt32(txtMinHeightSize.Text);
                int minNormalRatio = Convert.ToInt32(txtMinNormalRatio.Text);
                int maxNormalRatio = Convert.ToInt32(txtMaxNormalRatio.Text);
                int alignMaxDist = Convert.ToInt32(txtAlignMaxDist.Text);
                int darkAreaWidth = Convert.ToInt32(txtDarkAreaWidth.Text);

                FigureShapeProperty oldFigureShapeValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldFigureShapeValue = element.InspectionAlgorithm as FigureShapeProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldFigureShapeValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    FigureShapeProperty figureShapeValue = new FigureShapeProperty();

                    figureShapeValue.InspRange = inspRange;
                    figureShapeValue.AverMinMargin = averMinMargin;
                    figureShapeValue.AverMaxMargin = averMaxMargin;
                    figureShapeValue.MinMargin = minMargin;
                    figureShapeValue.MaxMargin = maxMargin;
                    figureShapeValue.ErosionTrainIter = erosionTrainIter;
                    figureShapeValue.DilationTrainIter = dilationTrainIter;
                    figureShapeValue.ErosionInspIter = erosionInspIter;
                    figureShapeValue.DilationInspIter = dilationInspIter;
                    figureShapeValue.MinWidthSize = minWidthSize;
                    figureShapeValue.MinHeightSize = minHeightSize;
                    figureShapeValue.MinNormalRatio = minNormalRatio;
                    figureShapeValue.MaxNormalRatio = maxNormalRatio;
                    figureShapeValue.AlignMaxDist = alignMaxDist;
                    figureShapeValue.DarkAreaWidth = darkAreaWidth;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = figureShapeValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldFigureShapeValue.InspRange = inspRange;
                    oldFigureShapeValue.AverMinMargin = averMinMargin;
                    oldFigureShapeValue.AverMaxMargin = averMaxMargin;
                    oldFigureShapeValue.MinMargin = minMargin;
                    oldFigureShapeValue.MaxMargin = maxMargin;
                    oldFigureShapeValue.ErosionTrainIter = erosionTrainIter;
                    oldFigureShapeValue.DilationTrainIter = dilationTrainIter;
                    oldFigureShapeValue.ErosionInspIter = erosionInspIter;
                    oldFigureShapeValue.DilationInspIter = dilationInspIter;
                    oldFigureShapeValue.MinWidthSize = minWidthSize;
                    oldFigureShapeValue.MinHeightSize = minHeightSize;
                    oldFigureShapeValue.MinNormalRatio = minNormalRatio;
                    oldFigureShapeValue.MaxNormalRatio = maxNormalRatio;
                    oldFigureShapeValue.AlignMaxDist = alignMaxDist;
                    oldFigureShapeValue.DarkAreaWidth = darkAreaWidth;
                }

                if (m_previewValue == null)
                    m_previewValue = new FigureShapeProperty();
                m_previewValue.InspRange = inspRange;
                m_previewValue.AverMinMargin = averMinMargin;
                m_previewValue.AverMaxMargin = averMaxMargin;
                m_previewValue.MinMargin = minMargin;
                m_previewValue.MaxMargin = maxMargin;
                m_previewValue.ErosionTrainIter = erosionTrainIter;
                m_previewValue.DilationTrainIter = dilationTrainIter;
                m_previewValue.ErosionInspIter = erosionInspIter;
                m_previewValue.DilationInspIter = dilationInspIter;
                m_previewValue.MinWidthSize = minWidthSize;
                m_previewValue.MinHeightSize = minHeightSize;
                m_previewValue.MinNormalRatio = minNormalRatio;
                m_previewValue.MaxNormalRatio = maxNormalRatio;
                m_previewValue.AlignMaxDist = alignMaxDist;
                m_previewValue.DarkAreaWidth = darkAreaWidth;
            }
            catch
            {
              //  MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
               // txtAverMinMargin.Focus();
            }
        }

        // 검사 설정 직전 값 표시.
        public void SetPreviewValue()
        {
            if (m_previewValue != null)
            {
                this.SetCheckBoxState(m_previewValue.InspRange);
                this.txtAverMinMargin.Text = m_previewValue.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = m_previewValue.AverMaxMargin.ToString();
                this.txtMinMargin.Text = m_previewValue.MinMargin.ToString();
                this.txtMaxMargin.Text = m_previewValue.MaxMargin.ToString();
                this.txtErosionTrainIter.Text = m_previewValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = m_previewValue.DilationTrainIter.ToString();
                this.txtErosionInspIter.Text = m_previewValue.ErosionInspIter.ToString();
                this.txtDilationInspIter.Text = m_previewValue.DilationInspIter.ToString();
                this.txtMinWidthSize.Text = m_previewValue.MinWidthSize.ToString();
                this.txtMinHeightSize.Text = m_previewValue.MinHeightSize.ToString();
                this.txtMinNormalRatio.Text = m_previewValue.MinNormalRatio.ToString();
                this.txtMaxNormalRatio.Text = m_previewValue.MaxNormalRatio.ToString();
                this.txtAlignMaxDist.Text = m_previewValue.AlignMaxDist.ToString();
                this.txtDarkAreaWidth.Text = m_previewValue.DarkAreaWidth.ToString();
            }
        }

        // 검사 설정 기본 값 표시.
        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeShapeHalfEtching)
            {
                if (!ShapeHalfEtchingDefaultValue.DefaultValueLoaded)
                {
                    ShapeHalfEtchingDefaultValue.DefaultValueLoaded = true;
                    ShapeHalfEtchingDefaultValue.LoadDefaultValue();
                }

                this.SetCheckBoxState(ShapeHalfEtchingDefaultValue.InspRange);
                this.txtAverMinMargin.Text = ShapeHalfEtchingDefaultValue.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = ShapeHalfEtchingDefaultValue.AverMaxMargin.ToString();
                this.txtMinMargin.Text = ShapeHalfEtchingDefaultValue.MinMargin.ToString();
                this.txtMaxMargin.Text = ShapeHalfEtchingDefaultValue.MaxMargin.ToString();
                this.txtErosionTrainIter.Text = ShapeHalfEtchingDefaultValue.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = ShapeHalfEtchingDefaultValue.DilationTrainIter.ToString();
                this.txtErosionInspIter.Text = ShapeHalfEtchingDefaultValue.ErosionInspIter.ToString();
                this.txtDilationInspIter.Text = ShapeHalfEtchingDefaultValue.DilationInspIter.ToString();
                this.txtMinWidthSize.Text = ShapeHalfEtchingDefaultValue.MinWidthSize.ToString();
                this.txtMinHeightSize.Text = ShapeHalfEtchingDefaultValue.MinHeightSize.ToString();
                this.txtMinNormalRatio.Text = ShapeHalfEtchingDefaultValue.MinNormalRatio.ToString();
                this.txtMaxNormalRatio.Text = ShapeHalfEtchingDefaultValue.MaxNormalRatio.ToString();
                this.txtAlignMaxDist.Text = ShapeHalfEtchingDefaultValue.AlignMaxDist.ToString();
                this.txtDarkAreaWidth.Text = ShapeHalfEtchingDefaultValue.DarkAreaWidth.ToString();
            }
        }

        // 검사 설정 표시.
        public void Display(InspectionItem settingValue, int MarginX, int MatginY)
        {
            FigureShapeProperty figureShapeProperty = settingValue.InspectionAlgorithm as FigureShapeProperty;
            if (figureShapeProperty != null)
            {
                this.SetCheckBoxState(figureShapeProperty.InspRange);
                this.txtAverMinMargin.Text = figureShapeProperty.AverMinMargin.ToString();
                this.txtAverMaxMargin.Text = figureShapeProperty.AverMaxMargin.ToString();
                this.txtMinMargin.Text = figureShapeProperty.MinMargin.ToString();
                this.txtMaxMargin.Text = figureShapeProperty.MaxMargin.ToString();
                this.txtErosionTrainIter.Text = figureShapeProperty.ErosionTrainIter.ToString();
                this.txtDilationTrainIter.Text = figureShapeProperty.DilationTrainIter.ToString();
                this.txtErosionInspIter.Text = figureShapeProperty.ErosionInspIter.ToString();
                this.txtDilationInspIter.Text = figureShapeProperty.DilationInspIter.ToString();
                this.txtMinWidthSize.Text = figureShapeProperty.MinWidthSize.ToString();
                this.txtMinHeightSize.Text = figureShapeProperty.MinHeightSize.ToString();
                this.txtMinNormalRatio.Text = figureShapeProperty.MinNormalRatio.ToString();
                this.txtMaxNormalRatio.Text = figureShapeProperty.MaxNormalRatio.ToString();
                this.txtAlignMaxDist.Text = figureShapeProperty.AlignMaxDist.ToString();
                this.txtDarkAreaWidth.Text = figureShapeProperty.DarkAreaWidth.ToString();
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
