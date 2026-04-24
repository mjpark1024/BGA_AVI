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
// 2015.12.21 - 코드 정리 완료. (검사 방법에 따라 초기 설정 값을 달리 가져갈 필요가 있다.)
//////////////////////////////////////////////////////
// Dam Size 알고리즘 : 코드 3030
//////////////////////////////////////////////////////

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>
    /// DamSize.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DamSize : UserControl, IInspectionTypeUICommands
    {
        private DamSizeProperty m_previewValue;

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
        public DamSize()
        {
            InitializeComponent();
            InitializeEvent();
        }
        private void InitializeEvent()
        {
            this.radLH.Click += radType_Click;
            this.radHH.Click += radType_Click;
        }


        #region Radio button events.
        private void radType_Click(object sender, RoutedEventArgs e)
        {
            SetRadioState1();
        }

        #endregion

        private void SetRadioState1()
        {
            if ((bool)radLH.IsChecked)
            {
                radHH.IsChecked = false;
            }
            else
            {
                radHH.IsChecked = true;
            }
        }

        public void SetDialog(string strCaption, InspectionInformation.eVisInspectType inspectType)
        {

            this.m_enumInspectType = inspectType;
        }

        // 검사 설정 저장.
        public void TrySave(GraphicsBase graphic, int anInspectID, ParametersLocking paradata)
        {
            try
            {
                #region Check 하한 / 상한 검출값
                int threshType = 0;
                int lowerThresh = Convert.ToInt32(txtThreshold.Text);
                int upperThresh = Convert.ToInt32(txtUpperThres.Text);
                int minmargin = Convert.ToInt32(txtMargin.Text);
                int mindefectsize = Convert.ToInt32(txtMinSize.Text);
                int maxdefectsize = Convert.ToInt32(txtMaxSize.Text);
                int minwidth = Convert.ToInt32(txtMinWidth.Text);
                if (radLH.IsChecked == true) // 임계 이상
                {
                    threshType = 0;
                }
                else if (radHH.IsChecked == true) // 임계 이하
                {
                    threshType = 1;
                }
                #endregion

                DamSizeProperty oldDamSizeValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldDamSizeValue = element.InspectionAlgorithm as DamSizeProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldDamSizeValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    DamSizeProperty damsizeValue = new DamSizeProperty();

                    damsizeValue.ThreshType = threshType;
                    damsizeValue.LowerThresh = lowerThresh;
                    damsizeValue.UpperThresh = upperThresh;
                    damsizeValue.MinMargin = minmargin;
                    damsizeValue.MinDefectSize = mindefectsize;
                    damsizeValue.MaxDefectSize = maxdefectsize;
                    damsizeValue.MinWidth = minwidth;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = damsizeValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldDamSizeValue.ThreshType = threshType;
                    oldDamSizeValue.LowerThresh = lowerThresh;
                    oldDamSizeValue.UpperThresh = upperThresh;
                    oldDamSizeValue.MinMargin = minmargin;
                    oldDamSizeValue.MinDefectSize = mindefectsize;
                    oldDamSizeValue.MaxDefectSize = maxdefectsize;
                    oldDamSizeValue.MinWidth = minwidth;
                }

                if (m_previewValue == null)
                    m_previewValue = new DamSizeProperty();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;
                m_previewValue.MinMargin = minmargin;
                m_previewValue.MinDefectSize = mindefectsize;
                m_previewValue.MaxDefectSize = maxdefectsize;
                m_previewValue.MinWidth = minwidth;
            }
            catch
            {
                MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
                txtThreshold.Focus();
            }
        }

        // 검사 설정 저장.
        public void TryAdd(ref InspectList list, int anInspectID)
        {
            try
            {
                #region Check 하한 / 상한 검출값
                int threshType = 0;
                int lowerThresh = Convert.ToInt32(txtThreshold.Text);
                int upperThresh = Convert.ToInt32(txtUpperThres.Text);
                int minmargin = Convert.ToInt32(txtMargin.Text);
                int mindefectsize = Convert.ToInt32(txtMinSize.Text);
                int maxdefectsize = Convert.ToInt32(txtMaxSize.Text);
                int minwidth = Convert.ToInt32(txtMinWidth.Text);
                if (radLH.IsChecked == true) // 임계 이상
                {
                    threshType = 0;
                }
                else if (radHH.IsChecked == true) // 임계 이하
                {
                    threshType = 1;
                }
                #endregion

                DamSizeProperty oldDamSizeValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldDamSizeValue = element.InspectionAlgorithm as DamSizeProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldDamSizeValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    DamSizeProperty damsizeValue = new DamSizeProperty();

                    damsizeValue.ThreshType = threshType;
                    damsizeValue.LowerThresh = lowerThresh;
                    damsizeValue.UpperThresh = upperThresh;
                    damsizeValue.MinMargin = minmargin;
                    damsizeValue.MinDefectSize = mindefectsize;
                    damsizeValue.MaxDefectSize = maxdefectsize;
                    damsizeValue.MinWidth = minwidth;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = damsizeValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldDamSizeValue.ThreshType = threshType;
                    oldDamSizeValue.LowerThresh = lowerThresh;
                    oldDamSizeValue.UpperThresh = upperThresh;
                    oldDamSizeValue.MinMargin = minmargin;
                    oldDamSizeValue.MinDefectSize = mindefectsize;
                    oldDamSizeValue.MaxDefectSize = maxdefectsize;
                    oldDamSizeValue.MinWidth = minwidth;
                }

                if (m_previewValue == null)
                    m_previewValue = new DamSizeProperty();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;
                m_previewValue.MinMargin = minmargin;
                m_previewValue.MinDefectSize = mindefectsize;
                m_previewValue.MaxDefectSize = maxdefectsize;
                m_previewValue.MinWidth = minwidth;
            }
            catch
            {
                //MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");
                //if (txtThreshold.Visibility == Visibility.Visible)
                //{
                //    txtThreshold.Focus();
                //}
                //else
                //{
                //    txtLowerThreshold.Focus();
                //}
            }
        }

        // 검사의 기본 값 표시.
        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeDamBar)
            {
                #region DamSizeDefaultValue
                if (!DamSizeDefaultValue.DefaultValueLoaded)
                {
                    DamSizeDefaultValue.DefaultValueLoaded = true;
                    DamSizeDefaultValue.LoadDefaultValue();
                }

                if (DamSizeDefaultValue.ThreshType == 0)
                {
                    this.radLH.IsChecked = true;
                    SetRadioState1();
                }
                else if (BallPatternDefaultValue.ThreshType == 1)
                {
                    this.radHH.IsChecked = true;
                    SetRadioState1();
                }
                txtThreshold.Text = DamSizeDefaultValue.LowerThresh.ToString();
                txtUpperThres.Text = DamSizeDefaultValue.UpperThresh.ToString();
                txtMargin.Text = DamSizeDefaultValue.MinMargin.ToString();
                txtMinSize.Text = DamSizeDefaultValue.MinDefectSize.ToString();
                txtMaxSize.Text = DamSizeDefaultValue.MaxDefectSize.ToString();
                txtMinWidth.Text = DamSizeDefaultValue.MinWidth.ToString();
                #endregion
            }
        }

        public void SetPreviewValue()
        {
            if (m_previewValue != null)
            {
                if (m_previewValue.ThreshType == 0)
                {
                    this.radLH.IsChecked = true;
                    SetRadioState1();
                }
                else if (m_previewValue.ThreshType == 1)
                {
                    this.radHH.IsChecked = true;
                    SetRadioState1();
                }
                txtThreshold.Text = m_previewValue.LowerThresh.ToString();
                txtUpperThres.Text = m_previewValue.UpperThresh.ToString();
                txtMargin.Text = m_previewValue.MinMargin.ToString();
                txtMinSize.Text = m_previewValue.MinDefectSize.ToString();
                txtMaxSize.Text = m_previewValue.MaxDefectSize.ToString();
                txtMinWidth.Text = m_previewValue.MinWidth.ToString();
            }
        }

        // 검사 설정 표시.
        public void Display(InspectionInformation.InspectionItem settingValue, int MarginX, int MatginY)
        {
            DamSizeProperty damsizeProperty = settingValue.InspectionAlgorithm as DamSizeProperty;
            if (damsizeProperty != null)
            {
                #region 임계 설정.
                if (damsizeProperty.ThreshType == 0) // 임계 이상
                {
                    this.radLH.IsChecked = true;
                    SetRadioState1();
                }
                else if (damsizeProperty.ThreshType == 1) // 임계 이하
                {
                    this.radHH.IsChecked = true;
                    SetRadioState1();
                }

                #endregion
                txtThreshold.Text = damsizeProperty.LowerThresh.ToString();
                txtUpperThres.Text = damsizeProperty.UpperThresh.ToString();
                txtMargin.Text = damsizeProperty.MinMargin.ToString();
                txtMinSize.Text = damsizeProperty.MinDefectSize.ToString();
                txtMaxSize.Text = damsizeProperty.MaxDefectSize.ToString();
                txtMinWidth.Text = damsizeProperty.MinWidth.ToString();
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

