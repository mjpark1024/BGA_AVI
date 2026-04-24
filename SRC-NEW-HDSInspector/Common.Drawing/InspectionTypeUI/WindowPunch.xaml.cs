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
// 2016.03.06 - 코드 정리 완료.
//////////////////////////////////////////////////////
// Window Punch 알고리즘 : 코드 3019
//////////////////////////////////////////////////////
// 인식키 검사 : 코드 0428
//////////////////////////////////////////////////////

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>
    /// WindowPunch.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WindowPunch : UserControl, IInspectionTypeUICommands
    {
        private WindowPunchProperty m_previewValue;

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
        public WindowPunch()
        {
            InitializeComponent();
        }

        public void SetDialog(string strCaption, eVisInspectType inspectType)
        {
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

                int useCrack = Convert.ToInt32((bool)chkCrack.IsChecked);
                int crackSize = Convert.ToInt32(txtCrackSize.Text);
                int crackMargin = Convert.ToInt32(txtCrackMarginY.Text);
                int useBurr = Convert.ToInt32((bool)chkBurr.IsChecked);
                int burrSize = Convert.ToInt32(txtBurrSize.Text);
                int burrMargin = Convert.ToInt32(txtBurrMarginY.Text);
                int erode = Convert.ToInt32(txtErode.Text);
                
                
                

                WindowPunchProperty oldWindowPunchValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        // 검사 파라미터 수정으로 간주하고 LineSegment를 삭제한다.
                        graphic.LineSegments = element.LineSegments = null;
                        graphic.BallSegments = element.BallSegments = null;
                        graphic.RefreshDrawing();

                        oldWindowPunchValue = element.InspectionAlgorithm as WindowPunchProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldWindowPunchValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    WindowPunchProperty windowPunchValue = new WindowPunchProperty();

                    windowPunchValue.ThreshType = threshType;
                    windowPunchValue.LowerThresh = lowerThresh;
                    windowPunchValue.UpperThresh = upperThresh;

                    windowPunchValue.UsePSRShift = useCrack;
                    windowPunchValue.PsrMarginX = crackMargin;
                    windowPunchValue.MinDefectSize = crackSize;
                    windowPunchValue.UsePunchShift = useBurr;
                    windowPunchValue.PsrMarginY = burrMargin;
                    windowPunchValue.MinHeightsize = burrSize;
                    windowPunchValue.ErosionTrainIter = erode;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = windowPunchValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldWindowPunchValue.ThreshType = threshType;
                    oldWindowPunchValue.LowerThresh = lowerThresh;
                    oldWindowPunchValue.UpperThresh = upperThresh;
                    oldWindowPunchValue.UsePSRShift = useCrack;
                    oldWindowPunchValue.PsrMarginX = crackMargin;
                    oldWindowPunchValue.MinDefectSize = crackSize;
                    oldWindowPunchValue.UsePunchShift = useBurr;
                    oldWindowPunchValue.PsrMarginY = burrMargin;
                    oldWindowPunchValue.MinHeightsize = burrSize;
                    oldWindowPunchValue.ErosionTrainIter = erode;

                }

                if (m_previewValue == null)
                    m_previewValue = new WindowPunchProperty();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;
                m_previewValue.UsePSRShift = useCrack;
                m_previewValue.PsrMarginX = crackMargin;
                m_previewValue.MinDefectSize = crackSize;
                m_previewValue.UsePunchShift = useBurr;
                m_previewValue.PsrMarginY = burrMargin;
                m_previewValue.MinHeightsize = burrSize;
                m_previewValue.ErosionTrainIter = erode;
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
                int upperThresh = 255;// Convert.ToInt32(txtUpperThreshold.Text);

                if (lowerThresh > upperThresh)
                {
                    MessageBox.Show("임계 하한 값은 임계 상한 값을 초과할 수 없습니다.", "Information");
                    this.txtLowerThreshold.Focus();
                    return;
                }

                int useCrack = Convert.ToInt32((bool)chkCrack.IsChecked);
                int crackSize = Convert.ToInt32(txtCrackSize.Text);
                int crackMargin = Convert.ToInt32(txtCrackMarginY.Text);
                int useBurr = Convert.ToInt32((bool)chkBurr.IsChecked);
                int burrSize = Convert.ToInt32(txtBurrSize.Text);
                int burrMargin = Convert.ToInt32(txtBurrMarginY.Text);
                int erode = Convert.ToInt32(txtErode.Text);




                WindowPunchProperty oldWindowPunchValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldWindowPunchValue = element.InspectionAlgorithm as WindowPunchProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldWindowPunchValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    WindowPunchProperty windowPunchValue = new WindowPunchProperty();

                    windowPunchValue.ThreshType = threshType;
                    windowPunchValue.LowerThresh = lowerThresh;
                    windowPunchValue.UpperThresh = upperThresh;

                    windowPunchValue.UsePSRShift = useCrack;
                    windowPunchValue.PsrMarginX = crackMargin;
                    windowPunchValue.MinDefectSize = crackSize;
                    windowPunchValue.UsePunchShift = useBurr;
                    windowPunchValue.PsrMarginY = burrMargin;
                    windowPunchValue.MinHeightsize = burrSize;
                    windowPunchValue.ErosionTrainIter = erode;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = windowPunchValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldWindowPunchValue.ThreshType = threshType;
                    oldWindowPunchValue.LowerThresh = lowerThresh;
                    oldWindowPunchValue.UpperThresh = upperThresh;
                    oldWindowPunchValue.UsePSRShift = useCrack;
                    oldWindowPunchValue.PsrMarginX = crackMargin;
                    oldWindowPunchValue.MinDefectSize = crackSize;
                    oldWindowPunchValue.UsePunchShift = useBurr;
                    oldWindowPunchValue.PsrMarginY = burrMargin;
                    oldWindowPunchValue.MinHeightsize = burrSize;
                    oldWindowPunchValue.ErosionTrainIter = erode;

                }

                if (m_previewValue == null)
                    m_previewValue = new WindowPunchProperty();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;
                m_previewValue.UsePSRShift = useCrack;
                m_previewValue.PsrMarginX = crackMargin;
                m_previewValue.MinDefectSize = crackSize;
                m_previewValue.UsePunchShift = useBurr;
                m_previewValue.PsrMarginY = burrMargin;
                m_previewValue.MinHeightsize = burrSize;
                m_previewValue.ErosionTrainIter = erode;
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

                this.chkBurr.IsChecked = Convert.ToBoolean(m_previewValue.UsePunchShift);
                this.chkCrack.IsChecked = Convert.ToBoolean(m_previewValue.UsePSRShift);

                this.txtCrackSize.Text = m_previewValue.MinDefectSize.ToString();

                this.txtBurrSize.Text = m_previewValue.MinHeightsize.ToString();

                this.txtCrackMarginY.Text = m_previewValue.PsrMarginX.ToString();
                this.txtBurrMarginY.Text = m_previewValue.PsrMarginY.ToString();
                this.txtErode.Text = m_previewValue.ErosionTrainIter.ToString();
            }
        }

        // 검사의 기본 값 표시.
        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeUnitCross)
            {
                if (!WindowPunchDefaultValue.DefaultValueLoaded)
                {
                    WindowPunchDefaultValue.DefaultValueLoaded = true;
                    WindowPunchDefaultValue.LoadDefaultValue();
                }

                this.txtLowerThreshold.Text = WindowPunchDefaultValue.LowerThresh.ToString();
                this.chkBurr.IsChecked = Convert.ToBoolean(WindowPunchDefaultValue.UsePunchShift);
                this.chkCrack.IsChecked = Convert.ToBoolean(WindowPunchDefaultValue.UsePSRShift);

                this.txtCrackSize.Text = WindowPunchDefaultValue.MinDefectSize.ToString();

                this.txtBurrSize.Text = WindowPunchDefaultValue.MinHeightsize.ToString();

                this.txtCrackMarginY.Text = WindowPunchDefaultValue.PsrMarginX.ToString();
                this.txtBurrMarginY.Text = WindowPunchDefaultValue.PsrMarginY.ToString();
                this.txtErode.Text = WindowPunchDefaultValue.ErosionTrainIter.ToString();
            }
        }

        // 검사 설정 표시.
        public void Display(InspectionItem settingValue, int MarginX, int MatginY)
        {
            WindowPunchProperty windowPunchValue = settingValue.InspectionAlgorithm as WindowPunchProperty;
            if (windowPunchValue != null)
            {
                this.txtLowerThreshold.Text = windowPunchValue.LowerThresh.ToString();
                this.chkBurr.IsChecked = Convert.ToBoolean(windowPunchValue.UsePunchShift);
                this.chkCrack.IsChecked = Convert.ToBoolean(windowPunchValue.UsePSRShift);

                this.txtCrackSize.Text = windowPunchValue.MinDefectSize.ToString();

                this.txtBurrSize.Text = windowPunchValue.MinHeightsize.ToString();

                this.txtCrackMarginY.Text = windowPunchValue.PsrMarginX.ToString();
                this.txtBurrMarginY.Text = windowPunchValue.PsrMarginY.ToString();
                this.txtErode.Text = windowPunchValue.ErosionTrainIter.ToString();
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
