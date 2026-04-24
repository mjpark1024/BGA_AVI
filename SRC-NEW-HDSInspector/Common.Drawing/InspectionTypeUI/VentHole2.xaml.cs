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
// 2016.03.10 - 코드 정리 완료.
//////////////////////////////////////////////////////
// VentHole 알고리즘 : 코드 3015
//////////////////////////////////////////////////////
// VentHole 검사 : 코드 0434
//////////////////////////////////////////////////////

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>
    /// VentHole.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VentHole2 : UserControl, IInspectionTypeUICommands
    {
        private VentHoleProperty2 m_previewValue;
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

        public VentHole2()
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
                int threshType = 0;

                int lowerThresh = Convert.ToInt32(txtThreshold.Text);

                if (lowerThresh < 1 && lowerThresh > 255)
                {
                    MessageBox.Show("임계 값은 1~255까지 입니다.", "Information");
                    this.txtThreshold.Focus();
                    return;
                }
                int minDefectSize = Convert.ToInt32(txtMinDefectSize.Text);

                VentHoleProperty2 oldVentHoleValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        // 검사 파라미터 수정으로 간주하고 LineSegment를 삭제한다.
                        graphic.LineSegments = element.LineSegments = null;
                        graphic.BallSegments = element.BallSegments = null;
                        graphic.RefreshDrawing();

                        oldVentHoleValue = element.InspectionAlgorithm as VentHoleProperty2; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }
                if (minDefectSize < paradata.MinVent2)
                {
                    if (oldVentHoleValue != null)
                    {
                        minDefectSize = oldVentHoleValue.MinDefectSize;
                    }
                    else
                    {
                        minDefectSize = paradata.MinVent2;
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

                if (oldVentHoleValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    VentHoleProperty2 ventHoleValue = new VentHoleProperty2();

                    ventHoleValue.ThreshType = threshType;
                    ventHoleValue.LowerThresh = lowerThresh;
                    ventHoleValue.MinDefectSize = minDefectSize;
                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = ventHoleValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldVentHoleValue.ThreshType = threshType;
                    oldVentHoleValue.LowerThresh = lowerThresh;
                    oldVentHoleValue.MinDefectSize = minDefectSize;

                }

                if (m_previewValue == null)
                    m_previewValue = new VentHoleProperty2();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.MinDefectSize = minDefectSize;
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
                int threshType = 0;
                int lowerThresh = Convert.ToInt32(txtThreshold.Text);
                int minDefectSize = Convert.ToInt32(txtMinDefectSize.Text);

                VentHoleProperty2 oldVentHoleValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldVentHoleValue = element.InspectionAlgorithm as VentHoleProperty2; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldVentHoleValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    VentHoleProperty2 ventHoleValue = new VentHoleProperty2();

                    ventHoleValue.ThreshType = threshType;
                    ventHoleValue.LowerThresh = lowerThresh;
                    ventHoleValue.MinDefectSize = minDefectSize;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = ventHoleValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldVentHoleValue.ThreshType = threshType;
                    oldVentHoleValue.LowerThresh = lowerThresh;
                    oldVentHoleValue.MinDefectSize = minDefectSize;
                }

                if (m_previewValue == null)
                    m_previewValue = new VentHoleProperty2();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.MinDefectSize = minDefectSize;
            }
            catch
            {

            }
        }

        // 검사의 직전 값 표시.
        public void SetPreviewValue()
        {
            if (m_previewValue != null)
            {
                this.txtThreshold.Text = m_previewValue.LowerThresh.ToString();
                this.txtMinDefectSize.Text = m_previewValue.MinDefectSize.ToString();
            }
        }

        // 검사의 기본 값 표시.
        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeNoResizeVentHole)
            {
                if (!ReSizeVentHoleDefaultValue.DefaultValueLoaded)
                {
                    ReSizeVentHoleDefaultValue.DefaultValueLoaded = true;
                    ReSizeVentHoleDefaultValue.LoadDefaultValue();
                }

                this.txtThreshold.Text = ReSizeVentHoleDefaultValue.LowerThresh.ToString();
                this.txtMinDefectSize.Text = ReSizeVentHoleDefaultValue.MinDefectSize.ToString();
             }
        }

        // 검사 설정 표시.
        public void Display(InspectionItem settingValue, int MarginX, int MatginY)
        {
            VentHoleProperty2 ventHoleValue = settingValue.InspectionAlgorithm as VentHoleProperty2;
            if (ventHoleValue != null)
            {
                this.txtThreshold.Text = ventHoleValue.LowerThresh.ToString();
                this.txtMinDefectSize.Text = ventHoleValue.MinDefectSize.ToString();
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
