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
/**
 * @file  TeachingAlignSettingWindow.xaml.cs
 * @brief 
 *  Interaction logic for TeachingAlignSettingWindow.xaml.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.10.01
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.10.01 First creation.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common.Drawing;
using System.Diagnostics;
using Common.Drawing.InspectionInformation;
using Common.Drawing.InspectionTypeUI;
using Common;
using PCS.ModelTeaching;

namespace HDSInspector
{
    /// <summary>   Form for viewing the teaching align setting.  </summary>
    /// <remarks>   suoow2, 2014-10-01. </remarks>
    public partial class TeachingAlignSettingWindow : Window
    {
        private DrawingCanvas m_currentCanvas;
        private GraphicsRegionType m_roiRegionType; // Unit Align or Strip Align type.

        public GraphicsRectangle CurrentROI
        {
            get;
            private set;
        }

        public static FiducialAlignProperty DefaultStripAlignProperty = new FiducialAlignProperty();
        public static FiducialAlignProperty DefaultUnitAlignProperty = new FiducialAlignProperty();

        public TeachingAlignSettingWindow(DrawingCanvas currentCanvas, GraphicsRegionType regionType)
        {
            m_currentCanvas = currentCanvas;
            CurrentROI = currentCanvas.SelectedGraphic as GraphicsRectangle;
            m_roiRegionType = regionType;

            InitializeComponent();
            InitializeEvent();
            InitializeDialog(regionType);
        }

        public static void InitializeDefaultValue()
        {
            if (!StripAlignDefaultValue.DefaultValueLoaded)
            {
                StripAlignDefaultValue.DefaultValueLoaded = true;
                StripAlignDefaultValue.LoadDefaultValue();

                DefaultStripAlignProperty.AlignMarginX = StripAlignDefaultValue.AlignMarginX;
                DefaultStripAlignProperty.AlignMarginY = StripAlignDefaultValue.AlignMarginY;
                DefaultStripAlignProperty.AlignAcceptance = StripAlignDefaultValue.AlignAcceptance;
            }
            if (!UnitAlignDefaultValue.DefaultValueLoaded)
            {
                UnitAlignDefaultValue.DefaultValueLoaded = true;
                UnitAlignDefaultValue.LoadDefaultValue();

                DefaultUnitAlignProperty.AlignMarginX = UnitAlignDefaultValue.AlignMarginX;
                DefaultUnitAlignProperty.AlignMarginY = UnitAlignDefaultValue.AlignMarginY;
                DefaultUnitAlignProperty.AlignAcceptance = UnitAlignDefaultValue.AlignAcceptance;
            }
        }

        private void InitializeDialog(GraphicsRegionType regionType)
        {
            if (regionType == GraphicsRegionType.UnitAlign)
            {
                this.Title = "Unit Align 설정";
                this.txtAlignMarginX.Text = DefaultUnitAlignProperty.AlignMarginX.ToString();
                this.txtAlignMarginY.Text = DefaultUnitAlignProperty.AlignMarginY.ToString();
                this.txtAlignAcceptance.Text = DefaultUnitAlignProperty.AlignAcceptance.ToString();
            }
            else if (regionType == GraphicsRegionType.Rawmetrial)
            {
                this.Title = "원소재 Align 설정";
                this.txtAlignMarginX.Text = DefaultUnitAlignProperty.AlignMarginX.ToString();
                this.txtAlignMarginY.Text = DefaultUnitAlignProperty.AlignMarginY.ToString();
                this.txtAlignAcceptance.Text = DefaultUnitAlignProperty.AlignAcceptance.ToString();

            }
            else if (regionType == GraphicsRegionType.OuterRegion)
            {
                this.Title = "외곽 Align 설정";
                this.txtAlignMarginX.Text = DefaultUnitAlignProperty.AlignMarginX.ToString();
                this.txtAlignMarginY.Text = DefaultUnitAlignProperty.AlignMarginY.ToString();
                this.txtAlignAcceptance.Text = DefaultUnitAlignProperty.AlignAcceptance.ToString();
            }
            else if (regionType == GraphicsRegionType.IDRegion)
            {
                this.Title = "ID Align 설정";
                this.txtAlignMarginX.Text = DefaultUnitAlignProperty.AlignMarginX.ToString();
                this.txtAlignMarginY.Text = DefaultUnitAlignProperty.AlignMarginY.ToString();
                this.txtAlignAcceptance.Text = DefaultUnitAlignProperty.AlignAcceptance.ToString();
            }
            else // GraphicsBaseRegionType.StripAlign
            {
                this.Title = "Strip Align 설정";
                this.txtAlignMarginX.Text = DefaultStripAlignProperty.AlignMarginX.ToString();
                this.txtAlignMarginY.Text = DefaultStripAlignProperty.AlignMarginY.ToString();
                this.txtAlignAcceptance.Text = DefaultStripAlignProperty.AlignAcceptance.ToString();
            }
        }

        private void InitializeEvent()
        {
            this.KeyDown += TeachingAlignSettingWindow_KeyDown;
            this.btnOK.Click += (s, e) => CloseWithOK();
            this.btnCancel.Click += (s, e) => CloseWithCancel();
        }

        private void TeachingAlignSettingWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                CloseWithOK();
            }
            else if (e.Key == Key.Escape)
            {
                CloseWithCancel();
            }
        }

        private void CloseWithOK()
        {
            try
            {
                if (ValueValidator.IsValidTextBoxValue(txtAlignMarginX) == false ||
                    ValueValidator.IsValidTextBoxValue(txtAlignMarginY) == false ||
                    ValueValidator.IsValidTextBoxValue(txtAlignAcceptance) == false)
                {
                    return;
                }

                int alignMarginX = Convert.ToInt32(txtAlignMarginX.Text);
                int alignMarginY = Convert.ToInt32(txtAlignMarginY.Text);
                int alignAcceptance = Convert.ToInt32(txtAlignAcceptance.Text);

                #region Check values.
                if (alignMarginX < (int)txtAlignMarginX.MinValue || (int)txtAlignMarginX.MaxValue < alignMarginX)
                {
                    MessageBox.Show("잘못된 값을 입력하셨습니다. 확인 바랍니다.", "Information");
                    txtAlignMarginX.Focus();
                    return;
                }

                if (alignMarginY < (int)txtAlignMarginY.MinValue || (int)txtAlignMarginY.MaxValue < alignMarginY)
                {
                    MessageBox.Show("잘못된 값을 입력하셨습니다. 확인 바랍니다.", "Information");
                    txtAlignMarginY.Focus();
                    return;
                }

                if (alignAcceptance < (int)txtAlignAcceptance.MinValue || (int)txtAlignAcceptance.MaxValue < alignAcceptance)
                {
                    MessageBox.Show("잘못된 값을 입력하셨습니다. 확인 바랍니다.", "Information");
                    txtAlignAcceptance.Focus();
                    return;
                }
                #endregion

                FiducialAlignProperty alignProperty = new FiducialAlignProperty { AlignMarginX = alignMarginX, AlignMarginY = alignMarginY, AlignAcceptance = alignAcceptance };
                if (m_roiRegionType == GraphicsRegionType.UnitAlign || m_roiRegionType == GraphicsRegionType.IDRegion || m_roiRegionType == GraphicsRegionType.StripAlign || m_roiRegionType == GraphicsRegionType.OuterRegion || m_roiRegionType == GraphicsRegionType.Rawmetrial)
                {
                    DefaultUnitAlignProperty.AlignMarginX = alignProperty.AlignMarginX;
                    DefaultUnitAlignProperty.AlignMarginY = alignProperty.AlignMarginY;
                    DefaultUnitAlignProperty.AlignAcceptance = alignProperty.AlignAcceptance;

                    int nUnitAlignCount = 0;

                    // 1. Unit Align 일괄 적용.
                    CurrentROI.InspectionList.Add(new InspectionItem(InspectionType.GetInspectionType(eVisInspectType.eInspTypeUnitAlign), alignProperty, 1));
                    if (Owner != null && Owner is TeachingWindow)
                    {
                        TeachingWindow teachingWindow = Owner as TeachingWindow;
                        if (teachingWindow != null)
                        {
                            string szSectionType = string.Empty;
                            SectionInformation selectedSection = teachingWindow.TeachingViewer.SelectedViewer.SelectedSection;
                            
                            if (selectedSection != null)
                                szSectionType = selectedSection.Type.Name;
                            if (!string.IsNullOrEmpty(szSectionType))
                            {
                                foreach (SectionInformation section in teachingWindow.TeachingViewer.SelectedViewer.SectionManager.Sections)
                                {
                                    if (szSectionType == section.Type.Name)
                                    {
                                        for (int i = 0; i < 3; i++)
                                        {
                                            // 동일한 Section 타입의 Unit Align에만 동일 설정 값을 적용한다.
                                            foreach (GraphicsBase graphic in section.ROICanvas[i].GraphicsList)
                                            {
                                                if (graphic.RegionType == GraphicsRegionType.UnitAlign || graphic.RegionType == GraphicsRegionType.IDRegion || graphic.RegionType == GraphicsRegionType.OuterRegion || graphic.RegionType == GraphicsRegionType.Rawmetrial) // UnitAlign && FiducialAlignProperty = Unit Align ROI.
                                                {
                                                    if (section.ROICanvas[i] == m_currentCanvas) nUnitAlignCount++;
                                                    if (graphic != CurrentROI)
                                                    {
                                                        foreach (InspectionItem unitAlignItem in graphic.InspectionList)
                                                        {
                                                            if (unitAlignItem.InspectionAlgorithm is FiducialAlignProperty)
                                                            {
                                                                unitAlignItem.InspectionAlgorithm = alignProperty;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (m_roiRegionType == GraphicsRegionType.UnitAlign || m_roiRegionType == GraphicsRegionType.Rawmetrial || m_roiRegionType == GraphicsRegionType.OuterRegion)
                    {
                        // 2. Unit Align ROI가 Section에 대해 처음 그려지는 경우라면 대칭되는 위치에 쌍둥이 Unit Align을 그려준다.
                        if (nUnitAlignCount == 1)
                        {
                            GraphicsRectangle graphic = new GraphicsRectangle(m_currentCanvas.ActualWidth - CurrentROI.Right,
                                                                              m_currentCanvas.ActualHeight - CurrentROI.Bottom,
                                                                              m_currentCanvas.ActualWidth - CurrentROI.Left,
                                                                              m_currentCanvas.ActualHeight - CurrentROI.Top,
                                                                              m_currentCanvas.LineWidth,
                                                                              GraphicsRegionType.UnitAlign,
                                                                              GraphicsColors.Red,
                                                                              m_currentCanvas.ActualScale);

                            if (m_currentCanvas.CanDraw(graphic))
                            {
                                graphic.Caption = CaptionHelper.UnitAlignCaption;
                                graphic.InspectionList.Add(new InspectionItem(InspectionType.GetInspectionType(eVisInspectType.eInspTypeUnitAlign), alignProperty, 1));
                                m_currentCanvas.GraphicsList.Add(graphic);
                            }
                            if (m_roiRegionType == GraphicsRegionType.UnitAlign || m_roiRegionType == GraphicsRegionType.OuterRegion)
                            {
                                GraphicsRectangle graphic1 = new GraphicsRectangle(CurrentROI.Right,
                                                                                   m_currentCanvas.ActualHeight - CurrentROI.Bottom,
                                                                                   CurrentROI.Left,
                                                                                   m_currentCanvas.ActualHeight - CurrentROI.Top,
                                                                                   m_currentCanvas.LineWidth,
                                                                                   GraphicsRegionType.UnitAlign,
                                                                                   GraphicsColors.Red,
                                                                                   m_currentCanvas.ActualScale);
                            
                                if (m_currentCanvas.CanDraw(graphic1))
                                {
                                    graphic1.Caption = CaptionHelper.UnitAlignCaption;
                                    graphic1.InspectionList.Add(new InspectionItem(InspectionType.GetInspectionType(eVisInspectType.eInspTypeUnitAlign), alignProperty, 1));
                                    m_currentCanvas.GraphicsList.Add(graphic1);
                                }
                            }
                            if (Owner != null && Owner is TeachingWindow)/////PSR 하지 이물 검사 색션에서는 4점 Align 을 사용 한다.
                            {
                                TeachingWindow teachingWindow = Owner as TeachingWindow;
                                if (teachingWindow != null)
                                {
                                    string szSectionType = string.Empty;
                                    SectionInformation selectedSection = teachingWindow.TeachingViewer.SelectedViewer.SelectedSection;
                            
                                    if (selectedSection != null && selectedSection.Type.Code == SectionTypeCode.PSR_REGION)
                                    {
                                        if (m_roiRegionType == GraphicsRegionType.UnitAlign || m_roiRegionType == GraphicsRegionType.OuterRegion)
                                        {
                                            GraphicsRectangle graphic2 = new GraphicsRectangle(m_currentCanvas.ActualWidth - CurrentROI.Right,
                                                                                              CurrentROI.Bottom,
                                                                                              m_currentCanvas.ActualWidth - CurrentROI.Left,
                                                                                              CurrentROI.Top,
                                                                                              m_currentCanvas.LineWidth,
                                                                                              GraphicsRegionType.UnitAlign,
                                                                                              GraphicsColors.Red,
                                                                                              m_currentCanvas.ActualScale);
                            
                                            if (m_currentCanvas.CanDraw(graphic2))
                                            {
                                                graphic2.Caption = CaptionHelper.UnitAlignCaption;
                                                graphic2.InspectionList.Add(new InspectionItem(InspectionType.GetInspectionType(eVisInspectType.eInspTypeUnitAlign), alignProperty, 1));
                                                m_currentCanvas.GraphicsList.Add(graphic2);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //else if (m_roiRegionType == GraphicsRegionType.Rawmetrial)
                //{
                //    //DefaultUnitAlignProperty.AlignMarginX = alignProperty.AlignMarginX;
                //    //DefaultUnitAlignProperty.AlignMarginY = alignProperty.AlignMarginY;
                //    //DefaultUnitAlignProperty.AlignAcceptance = alignProperty.AlignAcceptance;

                //    //InspectionItem rawAlignItem = new InspectionItem(InspectionType.GetInspectionType(eVisInspectType.eInspTypeUnitAlign), alignProperty, CurrentROI.InspectionList.Count + 1);
                //    //CurrentROI.InspectionList.Add(rawAlignItem);
                //    if (nUnitAlignCount == 1)
                //    {
                //        GraphicsRectangle graphic = new GraphicsRectangle(m_currentCanvas.ActualWidth - CurrentROI.Right,
                //                                                          m_currentCanvas.ActualHeight - CurrentROI.Bottom,
                //                                                          m_currentCanvas.ActualWidth - CurrentROI.Left,
                //                                                          m_currentCanvas.ActualHeight - CurrentROI.Top,
                //                                                          m_currentCanvas.LineWidth,
                //                                                          GraphicsRegionType.UnitAlign,
                //                                                          GraphicsColors.Red,
                //                                                          m_currentCanvas.ActualScale);

                //        if (m_currentCanvas.CanDraw(graphic))
                //        {
                //            graphic.Caption = CaptionHelper.UnitAlignCaption;
                //            graphic.InspectionList.Add(new InspectionItem(InspectionType.GetInspectionType(eVisInspectType.eInspTypeUnitAlign), alignProperty, 1));
                //            m_currentCanvas.GraphicsList.Add(graphic);
                //        }
                //    }
                //}
                
                //else // GraphicsBaseRegionType.StripAlign
                //{
                //    DefaultStripAlignProperty.AlignMarginX = alignProperty.AlignMarginX;
                //    DefaultStripAlignProperty.AlignMarginY = alignProperty.AlignMarginY;
                //    DefaultStripAlignProperty.AlignAcceptance = alignProperty.AlignAcceptance;

                //    InspectionItem stripAlignItem = new InspectionItem(InspectionType.GetInspectionType(eVisInspectType.eInspTypeGlobalAlign), alignProperty, CurrentROI.InspectionList.Count + 1);
                //    CurrentROI.InspectionList.Add(stripAlignItem);
                //}

                this.DialogResult = true;
                this.Close();
            }
            catch
            {
                MessageBox.Show("잘못된 값을 입력하셨습니다. 확인 바랍니다.", "Information");
            }
        }

        private void CloseWithCancel()
        {
            if (CurrentROI != null && m_currentCanvas.GraphicsList.Contains(CurrentROI))
            {
                m_currentCanvas.GraphicsList.Remove(CurrentROI);
            }
            this.DialogResult = false;
            this.Close();
        }
    }
}
