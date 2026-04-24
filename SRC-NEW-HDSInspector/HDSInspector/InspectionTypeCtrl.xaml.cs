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
 * @file  InspectionTypeCtrl.xaml.cs
 * @brief 
 *  선택된 ROI에 검사 설정을 부여한다.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.09.25
 * @version : 1.1
 * 
 * <b> Revision Histroy </b>
 * - 2011.09.25 First creation.
 */

using Common.Drawing;
using Common.Drawing.InspectionInformation;
using Common.Drawing.InspectionTypeUI;
using HDSInspector.SubWindow;
using PCS.ModelTeaching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace HDSInspector
{
    /// <summary>   InspectionTypeCtrl.xaml에 대한 상호 작용 논리. </summary>
    /// <remarks>   suoow2, 2014-09-25. </remarks>
    /// 
    public delegate void SaveChangedEventHandler(InspectionItem[] list, int index, string code);
    public delegate void InsptypeChangedEvent();

    public partial class InspectionTypeCtrl : UserControl, INotifyPropertyChanged
    {
        public TeachingWindow m_ptrTeachingWindow; // Teaching Window 포인터.
        public TeachingViewer m_ptrTeachingViewer; // Teaching Viewer 포인터.
        public GraphicsBase SelectedGraphic { get; set; }

        public event SaveChangedEventHandler SaveChanged;
        public static event InsptypeChangedEvent Event_MakeROIMulti16;
        #region Constructor & Initializer.
        public InspectionTypeCtrl()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }

        private void InitializeDialog()
        {
            cmbInspectionType.DisplayMemberPath = "Name";
            cmbInspectionType.SelectedValuePath = "SettingControl";
            cmbInspectionType.ItemsSource = InspectionType.GetInspectionTypeList();
            cmbInspectionType.SelectedIndex = 0;
        }

        private void InitializeEvent()
        {
            DrawingCanvas.ContextMenuChangeEvent += DrawingCanvas_ContextMenuChangeEvent;
            DrawingCanvas.SelectedGraphicChangeEvent += DrawingCanvas_SelectedGraphicChangeEvent;

            this.cmbInspectionType.SelectionChanged += cmbInspectionType_SelectionChanged;
            this.cmbInspectionType.PreviewMouseLeftButtonDown += cmbInspectionType_MouseLeftButtonDown;
            this.lbInspection.SelectionChanged += lbInspection_SelectionChanged;
            this.lbInspection.PreviewMouseWheel += lbInspection_PreviewMouseWheel;

            this.btnAddInspectionType.Click += (s, e) => AddInspectionItem(cmbInspectionType.SelectedItem as InspectionType);
            this.btnSave.Click += (s, e) => SaveInspectionItem();
            this.btnSetPreview.Click += (s, e) => SetAsPreview();
            this.btnSetDefault.Click += (s, e) => SetAsDefault();
            this.btnDelete.Click += (s, e) => DeleteInspectionItem();
            this.PreviewKeyUp += (s, e) => { if (e.Key == Key.Enter) { SaveInspectionItem(); } };
        }
        #endregion

        public void Remove_InspectType_PSRodd()
        {
            cmbInspectionType.ItemsSource = null;
            List<InspectionType> inspectionTypeList = new List<InspectionType>();
            inspectionTypeList = InspectionType.GetInspectionTypeList();
            inspectionTypeList.Remove(InspectionType.GetInspectionType("PSR이물 검사"));
            cmbInspectionType.ItemsSource = inspectionTypeList;
        }

        public void ChangeInspectionType(eSectionType aSectionType)
        {
            if (aSectionType == eSectionType.eSecTypeGlobal)
            {
                cmbInspectionType.ItemsSource = InspectionType.GetStripAlignInspectionTypeList();
                cmbInspectionType.SelectedIndex = 0;
            }
            else // Block, Unit, Rail... etc.
            {
                cmbInspectionType.ItemsSource = InspectionType.GetDefaultInspectionTypeList();
                cmbInspectionType.SelectedIndex = 0;
            }
        }

        private void lbInspection_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double offset = e.Delta / 3;

            if (offset > 0)
            {
                offset = (svInspectionList.VerticalOffset - offset < 0) ? svInspectionList.VerticalOffset : offset;
                this.svInspectionList.ScrollToVerticalOffset(svInspectionList.VerticalOffset - offset);
            }
            else
            {
                offset = (svInspectionList.VerticalOffset - offset > svInspectionList.ScrollableHeight) ? svInspectionList.VerticalOffset - svInspectionList.ScrollableHeight : offset;
                this.svInspectionList.ScrollToVerticalOffset(svInspectionList.VerticalOffset - offset);
            }
        }

        private void DrawingCanvas_ContextMenuChangeEvent(ContextMenuCommand selectedContextMenuCommand)
        {
            switch (selectedContextMenuCommand)
            {
                case ContextMenuCommand.UnSetExceptInspectionRegion:
                    DeleteExceptionalMaskItem();
                    break;
                case ContextMenuCommand.SetExceptInspectionRegion:
                    SetExeptionalMask();
                    SetEnableStateAddInspectionItem();
                    lbInspection.SelectedIndex = -1;
                    SaveInspectionItem();
                    break;
            }
        }

        #region Handling Inspection Item.
        // 검사 설정 추가.
        public void AddInspectionItem(InspectionType aSelectedItem)
        {
            lbInspection.SelectedIndex = -1;
            if (aSelectedItem != null && aSelectedItem.SettingControl != null)
            {
                if (aSelectedItem.InspectType == eVisInspectType.eInspTypeExceptionalMask)
                {
                    ExceptionalMask except = aSelectedItem.SettingControl as ExceptionalMask;
                    if (except != null)
                    {
                        List<int> columnList = new List<int>();
                        List<int> rowList = new List<int>();
                        foreach (SectionRegion region in m_ptrTeachingViewer.SelectedViewer.SelectedSection.SectionRegionList)
                        {
                            if (!columnList.Contains(region.RegionIndex.X))
                                columnList.Add(region.RegionIndex.X);
                            if (!rowList.Contains(region.RegionIndex.Y))
                                rowList.Add(region.RegionIndex.Y);
                        }
                        except.SetMaxValue(ref columnList, ref rowList);
                    }
                }
                
                ((IInspectionTypeUICommands)aSelectedItem.SettingControl).SetDialog(aSelectedItem.Name, aSelectedItem.InspectType);
                pnlInspectionSettings.Children.Clear();
                pnlInspectionSettings.Children.Add(aSelectedItem.SettingControl);
                btnSave.IsEnabled = true;
                btnSetPreview.IsEnabled = true;
                btnSetDefault.IsEnabled = true;
                btnDelete.IsEnabled = true;
                LengthenAnimation();

                SaveInspectionItem();
                if (aSelectedItem.InspectType == eVisInspectType.eInspTypePSROdd)
                {
                    Event_MakeROIMulti16();
                }
            }
        }

        // 검사 설정 저장.
        public void SaveInspectionItem()
        {
            if (pnlInspectionSettings.Children.Count == 1 && SelectedGraphic != null)
            {
                IInspectionTypeUICommands inspectionSetting = (IInspectionTypeUICommands)pnlInspectionSettings.Children[0];
                InspectionItem selectedItem = null;
                if (inspectionSetting is ExceptionalMask && SelectedGraphic.RegionType == GraphicsRegionType.Except)
                {
                    foreach (InspectionItem inspItem in SelectedGraphic.InspectionList)
                    {
                        if (inspItem.InspectionType.InspectType == eVisInspectType.eInspTypeExceptionalMask)
                        {
                            selectedItem = inspItem;
                            break;
                        }
                    }
                }
                else selectedItem = lbInspection.SelectedItem as InspectionItem;

                if (selectedItem == null)
                {
                    inspectionSetting.TrySave(SelectedGraphic, -1, MainWindow.LockingData);
                }
                else
                {
                    inspectionSetting.TrySave(SelectedGraphic, selectedItem.ID, MainWindow.LockingData);               
                }


                if (m_ptrTeachingViewer.SelectedViewer.SelectedSection.Type.Code == SectionTypeCode.RAW_REGION)
                {
                    bool bAlign = false;
                    foreach (InspectionItem inspItem in SelectedGraphic.InspectionList)
                    {
                        if ((inspItem.InspectionType.InspectType == eVisInspectType.eInspTypeUnitAlign &&
                            inspItem.InspectionAlgorithm is FiducialAlignProperty))
                        {
                            bAlign = true;
                            break;
                        }
                    }

                    if (!bAlign)
                    {

                        #region 원소재 검사 중복 체크

                        bool hasRawMaterial = SelectedGraphic.InspectionList.Any(x => x.InspectionType.InspectType == eVisInspectType.eInspTypeUnitRawMaterial);

                        bool hasOther = SelectedGraphic.InspectionList.Any(x => x.InspectionType.InspectType != eVisInspectType.eInspTypeUnitRawMaterial);

                        bool result = !(hasRawMaterial && hasOther);

                        if (!(hasRawMaterial && hasOther))
                        {
                            // true 경우
                        }
                        else
                        {
                            MessageBox.Show("다른 검사와 원소재 검사는 중복으로 처리할 수 없습니다.");
                            SelectedGraphic.InspectionList.Remove(SelectedGraphic.InspectionList[SelectedGraphic.InspectionList.Count-1]);
                            return;
                        }
                        #endregion

                        InspectionItem[] tmp = new InspectionItem[SelectedGraphic.InspectionList.Count];
                        SelectedGraphic.InspectionList.CopyTo(tmp, 0);



                        SaveChangedEventHandler er = SaveChanged;
                        if (er != null) er(tmp, lbInspection.SelectedIndex, SectionTypeCode.RAW_REGION);
                    }
                }

                if (m_ptrTeachingViewer.SelectedViewer.SelectedSection.Type.Code == SectionTypeCode.UNIT_REGION)
                {
                    bool bVia = false;
                    InspectionItem item = new InspectionItem();
                    foreach (InspectionItem inspItem in SelectedGraphic.InspectionList)
                    {
                        if (inspItem.InspectionType.InspectType == eVisInspectType.eInspTypeDownSet)
                        {
                            bVia = true;
                            item = inspItem;
                            break;
                        }
                    }
                    if (bVia)
                    {
                        SaveChangedEventHandler er = SaveChanged;
                        InspectionItem[] tmp = new InspectionItem[1];
                        tmp[0] = item;
                        if (er != null) er(tmp, lbInspection.SelectedIndex, SectionTypeCode.UNIT_REGION);
                    }
                }

                #region PSR 하지이물 파라미터 체인지
                //bool Modify_AllCircuitPartChange = false;
                //if (m_ptrTeachingViewer.SelectedViewer.SelectedSection.Type.Code == SectionTypeCode.PSR_REGION)
                //{
                //    GraphicsRectangle CurrentROI = m_ptrTeachingViewer.SelectedViewer.TeachingCanvas.SelectedGraphic as GraphicsRectangle;
                //    if (CurrentROI != null)
                //    {
                //        foreach (InspectionItem Insptype in CurrentROI.InspectionList)
                //        {
                //            if (Insptype.InspectionType == InspectionType.GetInspectionType(eVisInspectType.eInspTypePSROdd))
                //            {
                //                PSROddProperty item = Insptype.InspectionAlgorithm as PSROddProperty;
                //                if ((PSR_Inspection_Type)item.ThreshType == PSR_Inspection_Type.Circuit) /////현재 선택된  ROI가 회로부 일때
                //                {
                //                    Modify_AllCircuitPartChange = true;
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //}


                //if (m_ptrTeachingViewer.SelectedViewer.SelectedSection.Type.Code == SectionTypeCode.PSR_REGION && Modify_AllCircuitPartChange)
                //{
                //    foreach (SectionInformation section in m_ptrTeachingViewer.SelectedViewer.SectionManager.Sections)
                //    {
                //        for (int i = 0; i < 3; i++)
                //        {
                //            foreach (GraphicsBase graphic in section.ROICanvas[i].GraphicsList)
                //            {
                //                if (graphic == SelectedGraphic)
                //                    continue;
                //                foreach (InspectionItem InspectionItem in graphic.InspectionList)
                //                {
                //                    if (InspectionItem.InspectionType.InspectType == eVisInspectType.eInspTypePSROdd)
                //                    {
                //                        PSROddProperty item = InspectionItem.InspectionAlgorithm as PSROddProperty;
                //                        if ((PSR_Inspection_Type)item.ThreshType == PSR_Inspection_Type.Circuit)////회로부인 놈만
                //                        {
                //                            inspectionSetting.AllCircuitPartChange(graphic, InspectionItem.ID);
                //                            //item.MinWidthRatio = selectedItem.InspectionAlgorithm
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }

                //}

                PSR_Inspection_Type Inspection_Type = PSR_Inspection_Type.Non;

                if (m_ptrTeachingViewer.SelectedViewer.SelectedSection.Type.Code == SectionTypeCode.PSR_REGION)
                {
                    GraphicsRectangle CurrentROI = m_ptrTeachingViewer.SelectedViewer.TeachingCanvas.SelectedGraphic as GraphicsRectangle;
                    if (CurrentROI != null)
                    {
                        foreach (InspectionItem Insptype in CurrentROI.InspectionList)
                        {
                            if (Insptype.InspectionType == InspectionType.GetInspectionType(eVisInspectType.eInspTypePSROdd))
                            {
                                PSROddProperty item = Insptype.InspectionAlgorithm as PSROddProperty;
                                if ((PSR_Inspection_Type)item.ThreshType == PSR_Inspection_Type.Circuit) /////현재 선택된  ROI가 회로부 일때
                                {
                                    Inspection_Type = PSR_Inspection_Type.Circuit;
                                    break;
                                }
                                else
                                {
                                    Inspection_Type = PSR_Inspection_Type.Non_Circuit;
                                    break;
                                }
                            }
                        }
                    }
                }


                if (m_ptrTeachingViewer.SelectedViewer.SelectedSection.Type.Code == SectionTypeCode.PSR_REGION && Inspection_Type == PSR_Inspection_Type.Circuit)
                {
                    foreach (SectionInformation section in m_ptrTeachingViewer.SelectedViewer.SectionManager.Sections)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            foreach (GraphicsBase graphic in section.ROICanvas[i].GraphicsList)
                            {
                                if (graphic == SelectedGraphic)
                                    continue;
                                foreach (InspectionItem InspectionItem in graphic.InspectionList)
                                {
                                    if (InspectionItem.InspectionType.InspectType == eVisInspectType.eInspTypePSROdd)
                                    {
                                        PSROddProperty item = InspectionItem.InspectionAlgorithm as PSROddProperty;
                                        inspectionSetting.AllCircuitPartChange(graphic, InspectionItem.ID);
                                    }
                                }
                            }
                        }
                    }

                }

                if (m_ptrTeachingViewer.SelectedViewer.SelectedSection.Type.Code == SectionTypeCode.PSR_REGION && Inspection_Type == PSR_Inspection_Type.Non_Circuit)
                {
                    foreach (SectionInformation section in m_ptrTeachingViewer.SelectedViewer.SectionManager.Sections)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            foreach (GraphicsBase graphic in section.ROICanvas[i].GraphicsList)
                            {
                                if (graphic == SelectedGraphic)
                                    continue;
                                foreach (InspectionItem InspectionItem in graphic.InspectionList)
                                {
                                    if (InspectionItem.InspectionType.InspectType == eVisInspectType.eInspTypePSROdd)
                                    {
                                        PSROddProperty item = InspectionItem.InspectionAlgorithm as PSROddProperty;
                                        inspectionSetting.AllNonCircuitPartChange(graphic, InspectionItem.ID);
                                    }
                                }
                            }
                        }
                    }

                }

                #endregion
                if (inspectionSetting is FiducialAlign)
                {
                    #region Unit Align 일괄 적용.
                    foreach (InspectionItem inspItem in SelectedGraphic.InspectionList)
                    {
                        if (inspItem.InspectionType.InspectType == eVisInspectType.eInspTypeUnitAlign &&
                            inspItem.InspectionAlgorithm is FiducialAlignProperty)
                        {
                            TeachingAlignSettingWindow.DefaultUnitAlignProperty = inspItem.InspectionAlgorithm as FiducialAlignProperty;
                            SectionInformation selectedSection = m_ptrTeachingViewer.SelectedViewer.SelectedSection;
                            string szSectionType = string.Empty;
                            if (selectedSection != null)
                                szSectionType = selectedSection.Type.Name;
                            if (szSectionType != null)
                            {
                                foreach (SectionInformation section in m_ptrTeachingViewer.SelectedViewer.SectionManager.Sections)
                                {
                                    if (szSectionType == section.Type.Name)
                                    {
                                        for (int i = 0; i < 3; i++)
                                        {
                                            foreach (GraphicsBase graphic in section.ROICanvas[i].GraphicsList)
                                            {
                                                if (graphic == SelectedGraphic)
                                                    continue;

                                                if (graphic.RegionType == GraphicsRegionType.UnitAlign) // UnitAlign && FiducialAlignProperty = Unit Align ROI.
                                                {
                                                    foreach (InspectionItem unitAlignItem in graphic.InspectionList)
                                                    {
                                                        if (unitAlignItem.InspectionAlgorithm is FiducialAlignProperty)
                                                        {
                                                            unitAlignItem.InspectionAlgorithm = inspItem.InspectionAlgorithm as FiducialAlignProperty;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                    #endregion
                }
                else if (inspectionSetting is ExceptionalMask)
                {
                    SelectedGraphic.OriginObjectColor = SelectedGraphic.ObjectColor = GraphicsColors.Blue;
                }

                if (selectedItem == null)
                {
                    lbInspection.SelectedIndex = lbInspection.Items.Count - 1;
                    svInspectionList.ScrollToBottom();
                }
                m_ptrTeachingWindow.SetText("저장 완료", 1000);


            }
        }

        // 검사 설정 이전 값 복원.
        private void SetAsPreview()
        {
            if (this.pnlInspectionSettings.Children.Count == 1) // always 1
            {
                IInspectionTypeUICommands inspectionSetting = (IInspectionTypeUICommands)this.pnlInspectionSettings.Children[0];
                inspectionSetting.SetPreviewValue();
            }
        }

        // 검사 설정 초기화.
        private void SetAsDefault()
        {
            if (this.pnlInspectionSettings.Children.Count == 1) // always 1
            {
                IInspectionTypeUICommands inspectionSetting = (IInspectionTypeUICommands)this.pnlInspectionSettings.Children[0];
                inspectionSetting.SetDefaultValue();
            }
        }

        // 검사 제외 해제.
        private void DeleteExceptionalMaskItem()
        {
            try
            {
                ShortenAnimation();

                //지우려하는 아이템이 검사제외면, 일반 검사 ROI로 전환
                InspectionItem selectedItem = null;
                foreach (InspectionItem inspectionElement in SelectedGraphic.InspectionList)
                {
                    if (inspectionElement.InspectionType == InspectionType.GetInspectionType(eVisInspectType.eInspTypeExceptionalMask))
                    {
                        SelectedGraphic.Caption = "";
                        SelectedGraphic.OriginObjectColor = SelectedGraphic.ObjectColor = GraphicsColors.Green;
                        SelectedGraphic.RegionType = GraphicsRegionType.Inspection;

                        selectedItem = inspectionElement;
                        break;
                    }
                }
                SelectedGraphic.InspectionList.Remove(selectedItem);

                int nSequenceID = 1;
                foreach (InspectionItem inspectionElement in SelectedGraphic.InspectionList)
                {
                    inspectionElement.ID = nSequenceID++;
                }
                btnSave.IsEnabled = false;
                btnSetPreview.IsEnabled = false;
                btnSetDefault.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
            catch
            {
                Debug.WriteLine("Exception occured in btnDelete_Click(InspectionTypeCtrl.xaml.cs)");
            }
        }

        // 검사 설정 삭제.
        private void DeleteInspectionItem()
        {
            try
            {
                ShortenAnimation();

                //검사 제외가 들어가 있는 경우
                if (SelectedGraphic.RegionType == GraphicsRegionType.Except)
                {
                    //지우려하는 아이템이 검사제외면, 일반 검사 ROI로 전환
                    InspectionItem selectedItem = lbInspection.SelectedItem as InspectionItem;
                    if (selectedItem != null && selectedItem.InspectionType == InspectionType.GetInspectionType(eVisInspectType.eInspTypeExceptionalMask))
                    {
                        SelectedGraphic.Caption = "";
                        SelectedGraphic.OriginObjectColor = SelectedGraphic.ObjectColor = GraphicsColors.Green;
                        SelectedGraphic.RegionType = GraphicsRegionType.Inspection;
                    }
                }
                SelectedGraphic.InspectionList.RemoveAt(lbInspection.SelectedIndex);

                int nSequenceID = 1;
                foreach (InspectionItem inspectionElement in SelectedGraphic.InspectionList)
                {
                    inspectionElement.ID = nSequenceID++;
                }
                btnSave.IsEnabled = false;
                btnSetPreview.IsEnabled = false;
                btnSetDefault.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
            catch
            {
                Debug.WriteLine("Exception occured in btnDelete_Click(InspectionTypeCtrl.xaml.cs)");
            }
        }
        #endregion

        #region Add inspection item setting panel.
        private void SetEnableStateAddInspectionItem()
        {
            this.cmbInspectionType.IsEnabled = true; // 검사 방법 추가 가능한 상태
            this.btnAddInspectionType.IsEnabled = true;
            this.btnSave.IsEnabled = true;
            this.btnSetPreview.IsEnabled = true;
            this.btnSetDefault.IsEnabled = true;
            this.btnDelete.IsEnabled = true;
        }

        private void SetDisableStateAddInspectionItem()
        {
            this.cmbInspectionType.IsEnabled = false;
            this.btnAddInspectionType.IsEnabled = false;
            this.btnSave.IsEnabled = false;
            this.btnSetPreview.IsEnabled = false;
            this.btnSetDefault.IsEnabled = false;
            this.btnDelete.IsEnabled = false;
        }
        #endregion

        private void lbInspection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbInspection.SelectedIndex == -1)
            {
                ShortenAnimation();

                btnSave.IsEnabled = false;
                btnSetPreview.IsEnabled = false;
                btnSetDefault.IsEnabled = false;
                btnDelete.IsEnabled = false;

                if (SelectedGraphic != null)
                {
                    SelectedGraphic.LineSegments = null;
                    SelectedGraphic.RefreshDrawing();
                }
            }
            else if (SelectedGraphic == null)
            {
                SetDisableStateAddInspectionItem();
                ShortenAnimation();
            }
            else
            {
                try
                {
                    SetEnableStateAddInspectionItem();
                    if (SelectedGraphic.RegionType == GraphicsRegionType.StripAlign || SelectedGraphic.RegionType == GraphicsRegionType.IDMark)
                    {
                        this.cmbInspectionType.IsEnabled = false;
                        this.btnAddInspectionType.IsEnabled = false;
                    }

                    InspectionItem selectedItem = lbInspection.SelectedItem as InspectionItem;
                    if (selectedItem != null)
                    {
                        string inspectionName = selectedItem.InspectionType.Name;

                        // 검사 설정 아이템과 검사 항목을 조회하여 알맞은 UI컨트롤을 찾아 검사 설정 값을 Display 시킨다.
                        foreach (InspectionType inspectionElement in InspectionType.GetInspectionTypeList())
                        {
                            if (inspectionName == inspectionElement.Name)
                            {
                                if (inspectionElement.InspectType == eVisInspectType.eInspTypeExceptionalMask)
                                {
                                    ExceptionalMask except = inspectionElement.SettingControl as ExceptionalMask;
                                    if (except != null)
                                    {
                                        List<int> columnList = new List<int>();
                                        List<int> rowList = new List<int>();
                                        foreach (SectionRegion region in m_ptrTeachingViewer.SelectedViewer.SelectedSection.SectionRegionList)
                                        {
                                            if (!columnList.Contains(region.RegionIndex.X))
                                                columnList.Add(region.RegionIndex.X);
                                            if (!rowList.Contains(region.RegionIndex.Y))
                                                rowList.Add(region.RegionIndex.Y);
                                        }
                                        except.SetMaxValue(ref columnList, ref rowList);
                                    }
                                }

                                // Show Center Line.
                                SelectedGraphic.LineSegments = selectedItem.LineSegments;
                                SelectedGraphic.BallSegments = selectedItem.BallSegments;
                                SelectedGraphic.RefreshDrawing();

                                ((IInspectionTypeUICommands)inspectionElement.SettingControl).Display(selectedItem, MainWindow.CurrentModel.Strip.PSRMarginX, MainWindow.CurrentModel.Strip.PSRMarginY);
                                selectedItem.InspectionType.SettingControl = InspectionType.GetInspectionTypeSettingDialog(selectedItem.InspectionType.InspectType);
                                ((IInspectionTypeUICommands)selectedItem.InspectionType.SettingControl).SetDialog(selectedItem.InspectionType.Name, (eVisInspectType)selectedItem.InspectionType.InspectType);
                                pnlInspectionSettings.Children.Clear();
                                pnlInspectionSettings.Children.Add(inspectionElement.SettingControl);
                                LengthenAnimation();
                                break;
                            }
                        }
                        e.Handled = true;
                    }
                }
                catch
                {
                    Debug.WriteLine("Exception occured in lbInspection_SelectionChanged(InspectionTypeCtrl.xaml.cs)");
                }
            }
        }

        #region combobox events.
        private void cmbInspectionType_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedGraphic != null)
            {
                this.btnAddInspectionType.IsEnabled = true;
                this.lbInspection.SelectedIndex = -1;
            }
        }

        private void cmbInspectionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.pnlInspectionSettings.Children.Clear();
            // ShortenAnimation();
        }
        #endregion

        // 검사 예외 설정.
        private void SetExeptionalMask()
        {
            InspectionType selectedItem = null;

            foreach (InspectionType item in cmbInspectionType.Items)
            {
                if (item.InspectAlgorithm == eVisInspectAlgo.eInspAlgoExceptionalMask)
                {
                    selectedItem = item as InspectionType;
                    cmbInspectionType.SelectedIndex = cmbInspectionType.Items.IndexOf(item);
                    break;
                }
            }

            int bExceptionMaskIndex = -1;
            foreach (InspectionItem insp in lbInspection.Items)
            {
                if (insp.InspectionType.InspectType == eVisInspectType.eInspTypeExceptionalMask)
                {
                    lbInspection.SelectedItem = insp;
                    return;
                }
            }

            if (selectedItem != null && selectedItem.SettingControl != null)
            {
                if (bExceptionMaskIndex < 0)
                {
                    ExceptionalMask except = (ExceptionalMask)selectedItem.SettingControl;
                    
                    List<int> columnList = new List<int>();
                    List<int> rowList = new List<int>();
                    foreach (SectionRegion region in m_ptrTeachingViewer.SelectedViewer.SelectedSection.SectionRegionList)
                    {
                        if (!columnList.Contains(region.RegionIndex.X))
                            columnList.Add(region.RegionIndex.X);
                        if (!rowList.Contains(region.RegionIndex.Y))
                            rowList.Add(region.RegionIndex.Y);
                    }
                    except.SetMaxValue(ref columnList, ref rowList);
                    ((IInspectionTypeUICommands)selectedItem.SettingControl).SetDialog(selectedItem.Name, (eVisInspectType)selectedItem.InspectType);
                    this.pnlInspectionSettings.Children.Clear();
                    this.pnlInspectionSettings.Children.Add(selectedItem.SettingControl);
                    LengthenAnimation();
                }
            }
        }

        // 선택된 ROI 변경시 호출되는 이벤트.
        private void DrawingCanvas_SelectedGraphicChangeEvent(GraphicsBase newGraphic)
        {
            if (newGraphic != null)
            {
                if (newGraphic is GraphicsSelectionRectangle)
                {
                    ShortenAnimation();
                }

                this.lbInspection.DataContext = newGraphic.InspectionList;
                this.SelectedGraphic = newGraphic;

                if (newGraphic.InspectionList.Count > 0)
                {
                    this.lbInspection.SelectedIndex = newGraphic.InspectionList.Count - 1;
                }
                else
                {
                    this.lbInspection.SelectedIndex = -1;
                }

                if (newGraphic is GraphicsRectangleBase || newGraphic is GraphicsPolyLine)
                {
                    if (newGraphic.RegionType == GraphicsRegionType.StripAlign ||
                        newGraphic.RegionType == GraphicsRegionType.IDMark ||
                        newGraphic.RegionType == GraphicsRegionType.UnitRegion ||
                        newGraphic.RegionType == GraphicsRegionType.PSROdd ||
                        newGraphic.RegionType == GraphicsRegionType.LocalAlign ||
                        newGraphic.RegionType == GraphicsRegionType.OuterRegion ||
                        newGraphic.RegionType == GraphicsRegionType.Rawmetrial ||
                        newGraphic.RegionType == GraphicsRegionType.GuideLine ||
                        newGraphic.RegionType == GraphicsRegionType.TapeLoaction ||
                        newGraphic.RegionType == GraphicsRegionType.None)
                    {
                        SetDisableStateAddInspectionItem();
                    }
                    else if (newGraphic.RegionType == GraphicsRegionType.Except)
                    {
                        SetEnableStateAddInspectionItem();
                    }
                    else
                    {
                        this.cmbInspectionType.IsEnabled = true; // 검사 방법 추가 가능한 상태
                        this.btnAddInspectionType.IsEnabled = true;
                    }
                }
                else if (newGraphic is GraphicsLine)
                {
                    // 2014-10-13. Line에는 현재 검사방법을 지정할 수 없다.
                    SetDisableStateAddInspectionItem();
                    this.lbInspection.SelectedIndex = -1;
                }
            }
            else // newGraphic is null
            {
                this.cmbInspectionType.SelectedIndex = 0;
                this.pnlInspectionSettings.Children.Clear();
                SetDisableStateAddInspectionItem();

                this.SelectedGraphic = null;
                this.lbInspection.DataContext = null;
            }
        }

        #region Lengthen, Shorten Animation.
        private void LengthenAnimation()
        {
            Storyboard sb = (Storyboard)FindResource("LengthenAnimation");
            sb.Begin();
        }

        private void ShortenAnimation()
        {
            Storyboard sb = (Storyboard)FindResource("ShortenAnimation");
            sb.Begin();            
        }
        #endregion

        #region Implements INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        
        public void Notify(string strPropertyName)
        {
            PropertyChangedEventHandler p = PropertyChanged;
            if (p != null)
            {
                p(this, new PropertyChangedEventArgs(strPropertyName));
            }
        }
        #endregion
    }
}
