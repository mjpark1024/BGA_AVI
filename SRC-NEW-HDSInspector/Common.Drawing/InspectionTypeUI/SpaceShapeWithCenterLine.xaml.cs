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
// SpaceShapeWithCenterLine 알고리즘 : 코드 3008
//////////////////////////////////////////////////////
// 공간 형상 검사 : 코드 0436
//////////////////////////////////////////////////////

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>   Space shape with center line.  </summary>
    /// <remarks>   suoow2, 2014-10-10. </remarks>
    public partial class SpaceShapeWithCenterLine : UserControl, IInspectionTypeUICommands
    {
        private SpaceShapeWithCenterLineProperty m_previewValue;

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

        public SpaceShapeWithCenterLine()
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
                int threshType = 2;

                int lowerThresh = Convert.ToInt32(txtLowerThreshold.Text);
                int upperThresh = Convert.ToInt32(txtUpperThreshold.Text);

                if (lowerThresh > upperThresh)
                {
                    MessageBox.Show("임계 하한 값은 임계 상한 값을 초과할 수 없습니다.", "Information");
                    this.txtLowerThreshold.Focus();
                    return;
                }

                //int minNormalRatio = Convert.ToInt32(txtMinNormalRatio.Text);
                //int maxNormalRatio = Convert.ToInt32(txtMaxNormalRatio.Text);

                //if (minNormalRatio > maxNormalRatio)
                //{
                //    MessageBox.Show("리드편차 최소 허용범위는 리드편차 최대 허용범위를 초과할 수 없습니다.", "Information");
                //    this.txtMinNormalRatio.Focus();
                //    return;
                //}

                int minHeightSize = Convert.ToInt32(txtMinHeightSize.Text);

                int minWidthRatio = Convert.ToInt32(txtMinWidthRatio.Text);
                //int maxWidthRatio = Convert.ToInt32(txtMaxWidthRatio.Text);

                //if (minWidthRatio > maxWidthRatio)
                //{
                //    MessageBox.Show("리드 최소 선폭범위는 리드 최대 선폭범위를 초과할 수 없습니다.", "Information");
                //    this.txtMinWidthRatio.Focus();
                //    return;
                //}

                int minWidthSize = Convert.ToInt32(txtMinWidthSize.Text);
                int maxWidthSize = Convert.ToInt32(txtMaxWidthSize.Text);

                if (minWidthSize > maxWidthSize)
                {
                    MessageBox.Show("리드 최소 선폭은 리드 최대 선폭을 초과할 수 없습니다.", "Information");
                    this.txtMinWidthSize.Focus();
                    return;
                }

                int tipSize = Convert.ToInt32(txtTipSize.Text);

                SpaceShapeWithCenterLineProperty oldShapeShiftWithCLValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        // 검사 파라미터 수정으로 간주하고 LineSegment를 삭제한다.
                        graphic.LineSegments = element.LineSegments = null;
                        graphic.BallSegments = element.BallSegments = null;
                        graphic.RefreshDrawing();

                        oldShapeShiftWithCLValue = element.InspectionAlgorithm as SpaceShapeWithCenterLineProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }
                if (minWidthRatio > paradata.MaxSpace)
                {
                    if (oldShapeShiftWithCLValue != null)
                    {
                        minWidthRatio = oldShapeShiftWithCLValue.MinWidthRatio;
                    }
                    else
                    {
                        minWidthRatio = paradata.MaxSpace;
                    }
                    txtMinWidthRatio.Text = minWidthRatio.ToString();
                    txtMinWidthRatio.Focus();
                    MessageBox.Show("파라매터 제한 값 확인 바랍니다.", "Information");
                }
                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldShapeShiftWithCLValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    SpaceShapeWithCenterLineProperty shapeShiftWithCLValue = new SpaceShapeWithCenterLineProperty();

                    shapeShiftWithCLValue.ThreshType = threshType;
                    shapeShiftWithCLValue.LowerThresh = lowerThresh;
                    shapeShiftWithCLValue.UpperThresh = upperThresh;

                    shapeShiftWithCLValue.MinWidthRatio = minWidthRatio;
                    shapeShiftWithCLValue.MaxWidthRatio = 100;// maxWidthRatio;

                    shapeShiftWithCLValue.MinHeightsize = minHeightSize;

                    shapeShiftWithCLValue.MinNormalRatio = 100;// minNormalRatio;
                    shapeShiftWithCLValue.MaxNormalRatio = 100;// maxNormalRatio;

                    shapeShiftWithCLValue.MinWidthSize = minWidthSize;
                    shapeShiftWithCLValue.MaxWidthSize = maxWidthSize;
                    shapeShiftWithCLValue.TipSearchSize = tipSize;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = shapeShiftWithCLValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldShapeShiftWithCLValue.ThreshType = threshType;
                    oldShapeShiftWithCLValue.LowerThresh = lowerThresh;
                    oldShapeShiftWithCLValue.UpperThresh = upperThresh;

                    oldShapeShiftWithCLValue.MinWidthRatio = minWidthRatio;
                    oldShapeShiftWithCLValue.MaxWidthRatio = 100;// maxWidthRatio;

                    oldShapeShiftWithCLValue.MinHeightsize = minHeightSize;

                    oldShapeShiftWithCLValue.MinNormalRatio = 100;// minNormalRatio;
                    oldShapeShiftWithCLValue.MaxNormalRatio = 100;// maxNormalRatio;

                    oldShapeShiftWithCLValue.MinWidthSize = minWidthSize;
                    oldShapeShiftWithCLValue.MaxWidthSize = maxWidthSize;
                    oldShapeShiftWithCLValue.TipSearchSize = tipSize;
                }

                if (m_previewValue == null)
                    m_previewValue = new SpaceShapeWithCenterLineProperty();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;

                m_previewValue.MinWidthRatio = minWidthRatio;
                m_previewValue.MaxWidthRatio = 100;// maxWidthRatio;

                m_previewValue.MinHeightsize = minHeightSize;

                m_previewValue.MinNormalRatio = 100;// minNormalRatio;
                m_previewValue.MaxNormalRatio = 100;// maxNormalRatio;

                m_previewValue.MinWidthSize = minWidthSize;
                m_previewValue.MaxWidthSize = maxWidthSize;
                m_previewValue.TipSearchSize = tipSize;
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
                int threshType = 2;

                int lowerThresh = Convert.ToInt32(txtLowerThreshold.Text);
                int upperThresh = Convert.ToInt32(txtUpperThreshold.Text);

                //if (lowerThresh > upperThresh)
                //{
                //    MessageBox.Show("임계 하한 값은 임계 상한 값을 초과할 수 없습니다.", "Information");
                //    this.txtLowerThreshold.Focus();
                //    return;
                //}

                int minHeightSize = Convert.ToInt32(txtMinHeightSize.Text);

                int minWidthRatio = Convert.ToInt32(txtMinWidthRatio.Text);

                int minWidthSize = Convert.ToInt32(txtMinWidthSize.Text);
                int maxWidthSize = Convert.ToInt32(txtMaxWidthSize.Text);

                if (minWidthSize > maxWidthSize)
                {
                    MessageBox.Show("리드 최소 선폭은 리드 최대 선폭을 초과할 수 없습니다.", "Information");
                    this.txtMinWidthSize.Focus();
                    return;
                }

                int tipSize = Convert.ToInt32(txtTipSize.Text);

                SpaceShapeWithCenterLineProperty oldShapeShiftWithCLValue = null;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (element.ID == anInspectID)
                    {
                        oldShapeShiftWithCLValue = element.InspectionAlgorithm as SpaceShapeWithCenterLineProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }

                int nMaxInspectID = -1;
                foreach (InspectionItem element in list.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }

                if (oldShapeShiftWithCLValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    SpaceShapeWithCenterLineProperty shapeShiftWithCLValue = new SpaceShapeWithCenterLineProperty();

                    shapeShiftWithCLValue.ThreshType = threshType;
                    shapeShiftWithCLValue.LowerThresh = lowerThresh;
                    shapeShiftWithCLValue.UpperThresh = upperThresh;

                    shapeShiftWithCLValue.MinWidthRatio = minWidthRatio;
                    shapeShiftWithCLValue.MaxWidthRatio = 100;// maxWidthRatio;

                    shapeShiftWithCLValue.MinHeightsize = minHeightSize;

                    shapeShiftWithCLValue.MinNormalRatio = 100;// minNormalRatio;
                    shapeShiftWithCLValue.MaxNormalRatio = 100;// maxNormalRatio;

                    shapeShiftWithCLValue.MinWidthSize = minWidthSize;
                    shapeShiftWithCLValue.MaxWidthSize = maxWidthSize;
                    shapeShiftWithCLValue.TipSearchSize = tipSize;

                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = shapeShiftWithCLValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        list.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldShapeShiftWithCLValue.ThreshType = threshType;
                    oldShapeShiftWithCLValue.LowerThresh = lowerThresh;
                    oldShapeShiftWithCLValue.UpperThresh = upperThresh;

                    oldShapeShiftWithCLValue.MinWidthRatio = minWidthRatio;
                    oldShapeShiftWithCLValue.MaxWidthRatio = 100;// maxWidthRatio;

                    oldShapeShiftWithCLValue.MinHeightsize = minHeightSize;

                    oldShapeShiftWithCLValue.MinNormalRatio = 100;// minNormalRatio;
                    oldShapeShiftWithCLValue.MaxNormalRatio = 100;// maxNormalRatio;

                    oldShapeShiftWithCLValue.MinWidthSize = minWidthSize;
                    oldShapeShiftWithCLValue.MaxWidthSize = maxWidthSize;
                    oldShapeShiftWithCLValue.TipSearchSize = tipSize;
                }

                if (m_previewValue == null)
                    m_previewValue = new SpaceShapeWithCenterLineProperty();
                m_previewValue.ThreshType = threshType;
                m_previewValue.LowerThresh = lowerThresh;
                m_previewValue.UpperThresh = upperThresh;

                m_previewValue.MinWidthRatio = minWidthRatio;
                m_previewValue.MaxWidthRatio = 100;// maxWidthRatio;

                m_previewValue.MinHeightsize = minHeightSize;

                m_previewValue.MinNormalRatio = 100;// minNormalRatio;
                m_previewValue.MaxNormalRatio = 100;// maxNormalRatio;

                m_previewValue.MinWidthSize = minWidthSize;
                m_previewValue.MaxWidthSize = maxWidthSize;
                m_previewValue.TipSearchSize = tipSize;
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

                this.txtMinWidthRatio.Text = m_previewValue.MinWidthRatio.ToString();
                this.txtMaxWidthRatio.Text = m_previewValue.MaxWidthRatio.ToString();

                this.txtMinHeightSize.Text = m_previewValue.MinHeightsize.ToString();
                
                this.txtMinNormalRatio.Text = m_previewValue.MinNormalRatio.ToString();
                this.txtMaxNormalRatio.Text = m_previewValue.MaxNormalRatio.ToString();

                this.txtMinWidthSize.Text = m_previewValue.MinWidthSize.ToString();
                this.txtMaxWidthSize.Text = m_previewValue.MaxWidthSize.ToString();
                this.txtTipSize.Text = m_previewValue.TipSearchSize.ToString();
            }
        }

        // 검사의 기본 값 표시.
        public void SetDefaultValue()
        {
            if (m_enumInspectType == eVisInspectType.eInspTypeSpaceShapeWithCL)
            {
                if (!SpaceShapeWithCenterLineDefaultValue.DefaultValueLoaded)
                {
                    SpaceShapeWithCenterLineDefaultValue.DefaultValueLoaded = true;
                    SpaceShapeWithCenterLineDefaultValue.LoadDefaultValue();
                }

                this.txtLowerThreshold.Text = SpaceShapeWithCenterLineDefaultValue.LowerThresh.ToString();
                this.txtUpperThreshold.Text = SpaceShapeWithCenterLineDefaultValue.UpperThresh.ToString();

                this.txtMinWidthRatio.Text = SpaceShapeWithCenterLineDefaultValue.MinWidthRatio.ToString();
                this.txtMaxWidthRatio.Text = SpaceShapeWithCenterLineDefaultValue.MaxWidthRatio.ToString();

                this.txtMinHeightSize.Text = SpaceShapeWithCenterLineDefaultValue.MinHeightsize.ToString();
                
                this.txtMinNormalRatio.Text = SpaceShapeWithCenterLineDefaultValue.MinNormalRatio.ToString();
                this.txtMaxNormalRatio.Text = SpaceShapeWithCenterLineDefaultValue.MaxNormalRatio.ToString();

                this.txtMinWidthSize.Text = SpaceShapeWithCenterLineDefaultValue.MinWidthSize.ToString();
                this.txtMaxWidthSize.Text = SpaceShapeWithCenterLineDefaultValue.MaxWidthSize.ToString();
                this.txtTipSize.Text = SpaceShapeWithCenterLineDefaultValue.TipSearchSize.ToString();
            }
        }

        // 검사 설정 표시.
        public void Display(InspectionItem settingValue, int MarginX, int MatginY)
        {
            SpaceShapeWithCenterLineProperty spaceShiftWithCenterLineValue = settingValue.InspectionAlgorithm as SpaceShapeWithCenterLineProperty;
            if (spaceShiftWithCenterLineValue != null)
            {
                this.txtLowerThreshold.Text = spaceShiftWithCenterLineValue.LowerThresh.ToString();
                this.txtUpperThreshold.Text = spaceShiftWithCenterLineValue.UpperThresh.ToString();

                // 공간편차 최소/최대 허용범위
                this.txtMinWidthRatio.Text = spaceShiftWithCenterLineValue.MinWidthRatio.ToString();
                this.txtMaxWidthRatio.Text = spaceShiftWithCenterLineValue.MaxWidthRatio.ToString();

                // 연속 불량 개수
                this.txtMinHeightSize.Text = spaceShiftWithCenterLineValue.MinHeightsize.ToString();
                
                // 공간 최소/최대 선폭범위
                this.txtMinNormalRatio.Text = spaceShiftWithCenterLineValue.MinNormalRatio.ToString();
                this.txtMaxNormalRatio.Text = spaceShiftWithCenterLineValue.MaxNormalRatio.ToString();

                // 공간 최소/최대 선폭
                this.txtMinWidthSize.Text = spaceShiftWithCenterLineValue.MinWidthSize.ToString();
                this.txtMaxWidthSize.Text = spaceShiftWithCenterLineValue.MaxWidthSize.ToString();
                this.txtTipSize.Text = spaceShiftWithCenterLineValue.TipSearchSize.ToString();
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
