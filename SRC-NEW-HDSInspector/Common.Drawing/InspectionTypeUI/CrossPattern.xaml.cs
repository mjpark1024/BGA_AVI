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
using Common.DataBase;

//////////////////////////////////////////////////////
// suoow2 Created.
//////////////////////////////////////////////////////
// History
// 2012.04.05 - 코드 정리 완료.
//////////////////////////////////////////////////////
// CrossPattern 알고리즘 : 코드 3003
//////////////////////////////////////////////////////
// 인식키 검사 : 코드 0402
//////////////////////////////////////////////////////

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>
    /// CrossPattern.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CrossPattern : UserControl, IInspectionTypeUICommands
    {
        private CrossPatternProperty m_previewValue;
        private System.Drawing.Point PSRMargin;
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

        public CrossPattern()
        {
            InitializeComponent();
        }

        public void SetDialog(string strCaption, eVisInspectType inspectType)
        {
            //this.txtCaption.Text = strCaption;
            this.m_enumInspectType = inspectType;
        }

        // 검사 설정 저장.
        public void TrySave(GraphicsBase graphic, int anInspectID, ParametersLocking paradata)
        {
            try
            {
                int threshType = 1;

                int lowerThresh = Convert.ToInt32(txtLowerThreshold.Text);
                int upperThresh = 255;// Convert.ToInt32(txtUpperThreshold.Text);

                if (lowerThresh > upperThresh)
                {
                    MessageBox.Show("임계 하한 값은 임계 상한 값을 초과할 수 없습니다.", "Information");
                    this.txtLowerThreshold.Focus();
                    return;
                }

                int usePsr = Convert.ToInt32((bool)chkPSRShift.IsChecked);
                int usePsrBA = Convert.ToInt32((bool)chkPSRShiftBA.IsChecked);
                int useInspect = Convert.ToInt32((bool)chkInspect.IsChecked);

                int minHeightSize = Convert.ToInt32(txtMinHeightSize.Text);
                int minDefectSize = Convert.ToInt32(txtMinDefectSize.Text);
                // int marginX = Convert.ToInt32(txtPSRMarginX.Text);
                // int marginY = Convert.ToInt32(txtPSRMarginY.Text);

                CrossPatternProperty oldCrossPatternValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        // 검사 파라미터 수정으로 간주하고 LineSegment를 삭제한다.
                        graphic.LineSegments = element.LineSegments = null;
                        graphic.BallSegments = element.BallSegments = null;
                        graphic.RefreshDrawing();

                        oldCrossPatternValue = element.InspectionAlgorithm as CrossPatternProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }
                if (minDefectSize > paradata.MaxCross)
                {
                    if (oldCrossPatternValue != null)
                    {
                        minDefectSize = oldCrossPatternValue.MinDefectSize;
                    }
                    else
                    {
                        minDefectSize = paradata.MinVent;
                    }
                    txtMinDefectSize.Text = minDefectSize.ToString();
                    txtMinDefectSize.Focus();
                    MessageBox.Show("파라매터 제한 값 확인 바랍니다.", "Information");
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldCrossPatternValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    CrossPatternProperty crossPatternValue = new CrossPatternProperty();

                    crossPatternValue.ThreshType = threshType;
                    crossPatternValue.LowerThresh = lowerThresh;
                    crossPatternValue.UpperThresh = upperThresh;

                    crossPatternValue.UsePSRShift = usePsr;
                    crossPatternValue.UsePSRShiftBA = usePsrBA;
                    //   crossPatternValue.PsrMarginX = marginX;// minNormalRatio;
                    //   crossPatternValue.PsrMarginY = marginY;// maxNormalRatio;

                    crossPatternValue.UsePunchShift = useInspect;
                    
                    crossPatternValue.MinDefectSize = minDefectSize;
                    crossPatternValue.MinHeightsize = minHeightSize;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = crossPatternValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldCrossPatternValue.ThreshType = threshType;
                    oldCrossPatternValue.LowerThresh = lowerThresh;
                    oldCrossPatternValue.UpperThresh = upperThresh;

                    oldCrossPatternValue.UsePSRShift = usePsr;
                    oldCrossPatternValue.UsePSRShiftBA = usePsrBA;
                    //   oldCrossPatternValue.PsrMarginX = marginX;// minNormalRatio;
                    //   oldCrossPatternValue.PsrMarginY = marginY;// maxNormalRatio;

                    oldCrossPatternValue.UsePunchShift = useInspect;

                    oldCrossPatternValue.MinDefectSize = minDefectSize;
                    oldCrossPatternValue.MinHeightsize = minHeightSize;

                }

                if (m_previewValue == null)
                    m_previewValue = new CrossPatternProperty();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;

                m_previewValue.UsePSRShift = usePsr;
                m_previewValue.UsePSRShiftBA = usePsrBA;
                m_previewValue.UsePunchShift = useInspect;
                //  m_previewValue.PsrMarginX = marginX;// minNormalRatio;
                // m_previewValue.PsrMarginY = marginY;// maxNormalRatio;

                m_previewValue.MinDefectSize = minDefectSize;
                m_previewValue.MinHeightsize = minHeightSize;
            }
            catch
            {
                MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
                txtLowerThreshold.Focus();
            }
        }

        // 검사 설정 저장.
        public void TryAdd(ref InspectList list, int anInspectID)
        {
            try
            {
                int threshType = 1;

                int lowerThresh = Convert.ToInt32(txtLowerThreshold.Text);
                int upperThresh = Convert.ToInt32(txtUpperThreshold.Text);
                int usePsr = Convert.ToInt32((bool)chkPSRShift.IsChecked);
                int usePsrBA = Convert.ToInt32((bool)chkPSRShiftBA.IsChecked);
                int useInspect = Convert.ToInt32((bool)chkInspect.IsChecked);

                int minHeightSize = Convert.ToInt32(txtMinHeightSize.Text);
                int minDefectSize = Convert.ToInt32(txtMinDefectSize.Text);
                //   int marginX = Convert.ToInt32(txtPSRMarginX.Text);
                //  int marginY = Convert.ToInt32(txtPSRMarginY.Text);

                CrossPatternProperty oldCrossPatternValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldCrossPatternValue = element.InspectionAlgorithm as CrossPatternProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldCrossPatternValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    CrossPatternProperty crossPatternValue = new CrossPatternProperty();

                    crossPatternValue.ThreshType = threshType;
                    crossPatternValue.LowerThresh = lowerThresh;
                    crossPatternValue.UpperThresh = upperThresh;
                    crossPatternValue.UsePSRShift = usePsr;
                    crossPatternValue.UsePSRShiftBA = usePsrBA;
                    //     crossPatternValue.PsrMarginX = marginX;// minNormalRatio;
                    //      crossPatternValue.PsrMarginY = marginY;// maxNormalRatio;
                    crossPatternValue.UsePunchShift = useInspect;

                    crossPatternValue.MinDefectSize = minDefectSize;

                    crossPatternValue.MinHeightsize = minHeightSize;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = crossPatternValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldCrossPatternValue.ThreshType = threshType;
                    oldCrossPatternValue.LowerThresh = lowerThresh;
                    oldCrossPatternValue.UpperThresh = upperThresh;
                    oldCrossPatternValue.UsePSRShift = usePsr;
                    oldCrossPatternValue.UsePSRShiftBA = usePsrBA;
                    //     oldCrossPatternValue.PsrMarginX = marginX;// minNormalRatio;
                    //     oldCrossPatternValue.PsrMarginY = marginY;// maxNormalRatio;
                    oldCrossPatternValue.UsePunchShift = useInspect;

                    oldCrossPatternValue.MinDefectSize = minDefectSize;

                    oldCrossPatternValue.MinHeightsize = minHeightSize;
                }

                if (m_previewValue == null)
                    m_previewValue = new CrossPatternProperty();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;
                m_previewValue.UsePSRShift = usePsr;
                m_previewValue.UsePSRShiftBA = usePsrBA;
                //    m_previewValue.PsrMarginX = marginX;// minNormalRatio;
                //    m_previewValue.PsrMarginY = marginY;// maxNormalRatio;
                m_previewValue.UsePunchShift = useInspect;

                m_previewValue.MinDefectSize = minDefectSize;

                m_previewValue.MinHeightsize = minHeightSize;
            }
            catch
            {
                //MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
                //txtLowerThreshold.Focus();
            }
        }

        // 검사의 직전 값 표시.
        public void SetPreviewValue()
        {
            if (m_previewValue != null)
            {
                this.txtLowerThreshold.Text = m_previewValue.LowerThresh.ToString();
                this.txtUpperThreshold.Text = m_previewValue.UpperThresh.ToString();

                this.chkInspect.IsChecked = Convert.ToBoolean(m_previewValue.UsePunchShift);
                this.chkPSRShift.IsChecked = Convert.ToBoolean(m_previewValue.UsePSRShift);
                this.chkPSRShiftBA.IsChecked = Convert.ToBoolean(m_previewValue.UsePSRShiftBA);

                this.txtMinDefectSize.Text = m_previewValue.MinDefectSize.ToString();
                this.txtMinHeightSize.Text = m_previewValue.MinHeightsize.ToString();
            }
        }

        // 검사의 기본 값 표시.
        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeUnitCross)
            {
                if (!CrossPatternDefaultValue.DefaultValueLoaded)
                {
                    CrossPatternDefaultValue.DefaultValueLoaded = true;
                    CrossPatternDefaultValue.LoadDefaultValue();
                }

                this.txtLowerThreshold.Text = CrossPatternDefaultValue.LowerThresh.ToString();
                this.txtUpperThreshold.Text = CrossPatternDefaultValue.UpperThresh.ToString();
                this.chkInspect.IsChecked = Convert.ToBoolean(CrossPatternDefaultValue.UsePunchShift);
                this.chkPSRShift.IsChecked = Convert.ToBoolean(CrossPatternDefaultValue.UsePSRShift);
                this.chkPSRShiftBA.IsChecked = Convert.ToBoolean(CrossPatternDefaultValue.UsePSRShiftBA);

                this.txtMinDefectSize.Text = CrossPatternDefaultValue.MinDefectSize.ToString();
                this.txtMinHeightSize.Text = CrossPatternDefaultValue.MinHeightsize.ToString();
                this.lblPSRMarginX.Text = PSRMargin.X.ToString();
                this.lblPSRMarginY.Text = PSRMargin.Y.ToString();
                //  this.txtPSRMarginX.Text = CrossPatternDefaultValue.PsrMarginX.ToString();
                //   this.txtPSRMarginY.Text = CrossPatternDefaultValue.PsrMarginY.ToString();
            }
        }

        // 검사 설정 표시.
        public void Display(InspectionItem settingValue, int MarginX, int MarginY)
        {
            PSRMargin = new System.Drawing.Point(MarginX, MarginY);
            CrossPatternProperty crossPatternValue = settingValue.InspectionAlgorithm as CrossPatternProperty;
            if (crossPatternValue != null)
            {
                this.txtLowerThreshold.Text = crossPatternValue.LowerThresh.ToString();
                this.txtUpperThreshold.Text = crossPatternValue.UpperThresh.ToString();

                this.chkInspect.IsChecked = Convert.ToBoolean(crossPatternValue.UsePunchShift);
                this.chkPSRShift.IsChecked = Convert.ToBoolean(crossPatternValue.UsePSRShift);
                this.chkPSRShiftBA.IsChecked = Convert.ToBoolean(crossPatternValue.UsePSRShiftBA);

                this.txtMinDefectSize.Text = crossPatternValue.MinDefectSize.ToString();
                this.txtMinHeightSize.Text = crossPatternValue.MinHeightsize.ToString();
                this.lblPSRMarginX.Text = PSRMargin.X.ToString();
                this.lblPSRMarginY.Text = PSRMargin.Y.ToString();
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
