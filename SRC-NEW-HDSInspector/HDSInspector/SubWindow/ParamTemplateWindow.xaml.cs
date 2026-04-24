using Common.Drawing.InspectionInformation;
using Common.Drawing;
using PCS.ModelTeaching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common.DataBase;
using System.Data;
using Common;

namespace HDSInspector.SubWindow
{
    public partial class ParamTemplateWindow : Window
    {

        #region 생성자
        public ParamTemplateWindow(List<List<SectionInformation>> Sections)
        {
            InitializeComponent();

            if (ObjectDataProvider != null)
                ObjectDataProvider.ObjectInstance = InspItemStorage.Instance;
            this.DataContext = ObjectDataProvider;
            
            ShowTemplateSummary(Sections);
            InitializeEvent();

            currentModel = MainWindow.CurrentModel.Name;
        }
        #endregion

        #region 버튼 이벤트
        MatrixTransform originMatrix;
        private bool isCopied = false;
        BitmapSource TeachingImageSource;
        Image TeachingImage;
        private System.Windows.Point? m_ptLastContentMousePosition;
        private System.Windows.Point? m_ptLastCenterOfViewport;
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            if (InspItemStorage.Instance.ROICount != CountInspROI())
            {
                NoticeDlg noticeDlg = new NoticeDlg("ROI 개수 불일치, 개발팀 문의 바랍니다.");
                noticeDlg.ShowDialog();
                return;
            }

            if (paramGrid.SelectedItems.Count == 1)
            {
                InspItem selectedItem = (InspItem)paramGrid.SelectedItems[0];
                txtSelectedRoiCode.Text = selectedItem.RoiCode;
                copiedRoiCode = selectedItem.RoiCode;
                isCopied = true;
                cntCopiedRoiInsp = paramGrid.Items.Cast<InspItem>().Where(InspItem => InspItem.RoiCode == copiedRoiCode).Count();
                lstCopiedInspCode = paramGrid.Items.Cast<InspItem>().Where(InspItem => InspItem.RoiCode == copiedRoiCode).Select(InspItem => InspItem.InspName).ToList();
                lstCopiedRoi = paramGrid.Items.Cast<InspItem>().Where(InspItem => InspItem.RoiCode == copiedRoiCode).ToList();
                SetDataByCopiedRoicode();

                NoticeDlg noticeDlg = new NoticeDlg(string.Format(" {0} ROI가 복사 되었습니다.", selectedItem.RoiCode));
                noticeDlg.ShowDialog();
            }

            Mouse.OverrideCursor = null;
        }
        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            if (isCopied)
            {
                //붙여넣기 작업 수행
                pasteROI();

                //초기화
                InitData();

                NoticeDlg notice = new NoticeDlg("붙여넣기 완료 되었습니다."); notice.ShowDialog();
            }

            Mouse.OverrideCursor = null;
        }
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
           Mouse.OverrideCursor = Cursors.Wait;

            ptrTeachingViewer.CamView[0].LoadTeachingData();

            Mouse.OverrideCursor = null;
        }
        private void btnCopyCancel_Click(object sender, RoutedEventArgs e)
        {
            InitData();
        }
        private void Close_Event(object sender, EventArgs e)
        {
            InspItemStorage.Instance = null;
        }
        private void paramGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid != null && e.ClickCount == 1)
                SelectionChangeEvent();
        }
        private void ParamTemplateWindow_KeyUp(object sender, KeyEventArgs e)
        {
            SelectionChangeEvent();
        }
        private void ParamTemplateWindow_KeyDown(object sender, KeyEventArgs e)
        {
            SelectionChangeEvent();
        }
        private void paramGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                if (e.Delta > 0)
                {
                    // 마우스 위로 내릴 때 스크롤을 조정합니다.
                    ScrollViewer scrollViewer = GetScrollViewer(dataGrid);
                    if (scrollViewer != null)
                    {
                        double verticalOffset = scrollViewer.VerticalOffset - 10;
                        scrollViewer.ScrollToVerticalOffset(verticalOffset);
                    }
                }
                else
                {
                    // 아래로 스크롤할 때
                    ScrollViewer scrollViewer = GetScrollViewer(dataGrid);
                    if (scrollViewer != null)
                    {
                        double verticalOffset = scrollViewer.VerticalOffset + 10;
                        scrollViewer.ScrollToVerticalOffset(verticalOffset);
                    }
                }
                e.Handled = true;
            }
        }
        private void pnlOuter_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.Space))
            {
                m_ptLastContentMousePosition = GetContentMousePosition();
                if (e.Delta != 0)
                    Zoom(e.Delta);
                m_ptLastCenterOfViewport = GetCenterOfViewport();

                e.Handled = true; // blocking wheel event of svImageViewer control.
            }
        }
        private void Zoom(double deltaValue)
        {
            if (deltaValue > 0)
            {
                    ZoomValue *= 1.1;
            }
            else if (deltaValue == 0)
            {
                ZoomValue = scaleToFit;
            }
            else
            {
                ZoomValue = (ZoomValue / 1.1 < scaleToFit) ? scaleToFit : ZoomValue / 1.1;
            }
        }
        private System.Windows.Point GetContentMousePosition()
        {
            if (TeachingImage == null)
            {
                return new System.Windows.Point(0, 0);
            }
            else
            {
                System.Windows.Point ptContentMousePosition = Mouse.GetPosition(TeachingImage);

                return ptContentMousePosition;
            }
        }
        private void svTeaching_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ViewportWidthChange != 0 || e.ViewportHeightChange != 0)
            {
                m_ptLastCenterOfViewport = GetCenterOfViewport();
            }

            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                System.Windows.Point? ptBefore = null;
                System.Windows.Point? ptCurrent = null;

                if (!m_ptLastContentMousePosition.HasValue && m_ptLastCenterOfViewport.HasValue)
                {
                    ptBefore = m_ptLastCenterOfViewport;
                    ptCurrent = GetCenterOfViewport();
                }
                else
                {
                    ptBefore = m_ptLastContentMousePosition;
                    ptCurrent = GetContentMousePosition();

                    m_ptLastContentMousePosition = null;
                }

                if (ptBefore.HasValue)
                {
                    double fDeltaXInTargetPixels = ptCurrent.Value.X - ptBefore.Value.X;
                    double fDeltaYInTargetPixels = ptCurrent.Value.Y - ptBefore.Value.Y;

                    double fMultiplicatorX = e.ExtentWidth / TeachingImageSource.PixelWidth;
                    double fMultiplicatorY = e.ExtentHeight / TeachingImageSource.PixelHeight;

                    double newOffsetX = svROIViewer.HorizontalOffset - fDeltaXInTargetPixels * fMultiplicatorX;
                    double newOffsetY = svROIViewer.VerticalOffset - fDeltaYInTargetPixels * fMultiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {
                        return;
                    }

                    svROIViewer.ScrollToHorizontalOffset(newOffsetX);
                    svROIViewer.ScrollToVerticalOffset(newOffsetY);
                }
            }
            else
            {
                m_ptLastCenterOfViewport = GetCenterOfViewport();
            }
        }
        private ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            // DataGrid 내부의 ScrollViewer를 찾아 반환합니다.
            if (depObj is ScrollViewer)
            {
                return depObj as ScrollViewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                ScrollViewer scrollViewer = GetScrollViewer(child);
                if (scrollViewer != null)
                {
                    return scrollViewer;
                }
            }

            return null;
        }
        private System.Windows.Point GetCenterOfViewport()
        {
            if (TeachingImage == null)
            {
                return new System.Windows.Point(0, 0);
            }
            else
            {
                System.Windows.Point ptCenterOfViewport = new System.Windows.Point(svROIViewer.ActualWidth / 2, svROIViewer.ActualHeight / 2);
                System.Windows.Point ptTranslatedCenterOfViewport = svROIViewer.TranslatePoint(ptCenterOfViewport, TeachingImage);

                return ptTranslatedCenterOfViewport;
            }
        }
        #endregion

        #region 티칭 데이터 연동
        string currentModel = "";
        TeachingViewer ptrTeachingViewer;
        TeachingViewerCtrl selectedViewerCtrl;
        string copiedRoiCode = "";
        string selectedRoiCode = "";
        string selectedWorkType = "";
        string selectedSectionName = "";
        int sectionImgPixelHeight = 0;
        int sectionImgPixelWidth = 0;
        double viewerHeight = 0;
        double viewerWidth = 0;
        double scaleToFit = 0;
        int cntCopiedRoiInsp = 0;
        List<IdString> lstCopiedInspCode;
        List<InspItem> lstCopiedRoi;
        public void SetTeacingWindowPtr(TeachingViewer teachingViewer)
        {
            ptrTeachingViewer = teachingViewer;
        }
        private void SetViewerCtrl(string workType)
        {
            for (int i = 0; i < MainWindow.m_nTotalTeachingViewCount; i++)
            {
                if (workType == ptrTeachingViewer.CamView[i].sSurface)
                    selectedViewerCtrl = ptrTeachingViewer.CamView[i];
            }

            //switch (workType)
            //{
            //    case "BP1":
            //        selectedViewerCtrl = ptrTeachingViewer.CamBP1View;
            //        break;
            //    case "BP2":
            //        selectedViewerCtrl = ptrTeachingViewer.CamBP2View;
            //        break;
            //    case "CA1":
            //        selectedViewerCtrl = ptrTeachingViewer.CamCA1View;
            //        break;
            //    case "CA2":
            //        selectedViewerCtrl = ptrTeachingViewer.CamCA2View;
            //        break;
            //    case "BA1":
            //        selectedViewerCtrl = ptrTeachingViewer.CamBA1View;
            //        break;
            //    case "BA2":
            //        selectedViewerCtrl = ptrTeachingViewer.CamBA2View;
            //        break;
            //}
        }
        private void SetSectionImage(string sectionName)
        {
            pnlInner.Children.Clear();
            foreach (SectionInformation section in selectedViewerCtrl.SectionList)
            {
                if (section.Name == sectionName)
                {
                    Image image = new Image();
                    image.VerticalAlignment = VerticalAlignment.Top;
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    image.Source = section.Image[0].Source.Clone();
                    pnlInner.Children.Add(image);
                    TeachingImage = image;
                    sectionImgPixelHeight = section.GetBitmapSource(0).PixelHeight;
                    sectionImgPixelWidth = section.GetBitmapSource(0).PixelWidth;
                    TeachingImageSource = section.GetBitmapSource(0);
                    viewerHeight = svROIViewer.ActualHeight;
                    viewerWidth = svROIViewer.ActualWidth;
                }
            }
        }
        private void SetFitScale()
        {
            double fnumerator = 1.0;
            double fdenominator = 1.0;

            if (sectionImgPixelHeight / viewerHeight > sectionImgPixelWidth / viewerWidth)
            {
                fnumerator = viewerHeight;
                fdenominator = sectionImgPixelHeight;
            }
            else
            {
                fnumerator = viewerWidth;
                fdenominator = sectionImgPixelWidth;
            }
            scaleToFit = fnumerator / fdenominator * 0.975;

            ZoomValue = scaleToFit;
            sldrScale.Minimum = (scaleToFit > 0) ? scaleToFit : 0.05;
        }
        private void SetROIData(string sectionName)
        {
            foreach (SectionInformation section in selectedViewerCtrl.SectionList)
            {
                if (section.Name == sectionName)
                {
                    DrawingCanvas drawingCanvas = new DrawingCanvas(false,false);
                    drawingCanvas.HorizontalAlignment = HorizontalAlignment.Left;
                    drawingCanvas.VerticalAlignment = VerticalAlignment.Top;
                    HighLightSelectedROI(drawingCanvas, section);
                    pnlInner.Children.Add(drawingCanvas);
                }
            }
        }
        private void HighLightSelectedROI(DrawingCanvas drawingCanvas, SectionInformation selectedSection)
        {
            foreach (GraphicsBase g in selectedSection.ROICanvas[0].GraphicsList)
            {
                if (g.roiCode == selectedRoiCode)
                {
                    //g.boundaryRect.Top
                    GraphicsRectangle rctHightLightROI = new GraphicsRectangle(g.boundaryRect.Left, g.boundaryRect.Top, g.boundaryRect.Right, g.boundaryRect.Bottom,
                                                     1, GraphicsRegionType.LocalAlign, Colors.Red, scaleToFit);

                    GraphicsBase selectedROI = g.CreateSerializedObject().CreateGraphics();
                    selectedROI.ActualScale = scaleToFit;

                    drawingCanvas.GraphicsList.Add(selectedROI);
                }
            }
        }
        private double ZoomValue
        {
            get { return sldrScale.Value; }
            set
            {
                sldrScale.Value = value;
            }
        }
        private void SelectionChangeEvent()
        {
            if (paramGrid.SelectedItems.Count > 1 && isCopied == false)
            {
                paramGrid.SelectedItem = null;
                NoticeDlg noticeDlg = new NoticeDlg("복사 대상 ROI는 1개만 선택할 수 있습니다.");
                noticeDlg.ShowDialog();
                return;
            }

            if (paramGrid.SelectedItems.Count > 0)
            {
                InspItem selectedItem = (InspItem)paramGrid.SelectedItems[0];
                selectedRoiCode = selectedItem.RoiCode;
                selectedWorkType = selectedItem.WorkType.Name;
                selectedSectionName = selectedItem.SectionName.Name;

                //Set Section Image
                SetViewerCtrl(selectedWorkType);
                SetSectionImage(selectedSectionName);

                SetFitScale();

                //Set Roi Data
                SetROIData(selectedSectionName);
            }
        }
        private void pasteROI()
        {
            NoticeDlg notice;
            List<IdString> lstUsedSectionType = new List<IdString>();

            //1. 붙여넣기 할 선택된 ROI Code List
            List<string> lstSelectedRoi = new List<string>();
            foreach(InspItem item in paramGrid.SelectedItems)
                if (!lstSelectedRoi.Contains(item.RoiCode)) lstSelectedRoi.Add(item.RoiCode);

            //2. 선택 ROI 유효성 체크
            foreach(var citem in lstCopiedRoi)
            {
                foreach (var selectedRoi in lstSelectedRoi)
                {
                    if (paramGrid.Items.Cast<InspItem>()
                         .Where(InspItem => InspItem.RoiCode == selectedRoi)
                         .Where(InspItem => InspItem.InspName == citem.InspName)
                         .Where(InspItem => InspItem.ThreshType == citem.ThreshType).ToList().Count == 0)
                    {
                        notice = new NoticeDlg("다른 종류의 ROI는 붙여넣기 할 수 없습니다.\nROI 간의 상호 검사 종류가 동일해야 합니다."); notice.ShowDialog();
                        return;
                    }
                }

                if (!lstUsedSectionType.Contains(citem.SectionName)) lstUsedSectionType.Add(citem.SectionName);
            }

            //3. Datagrid InspItem instance 업데이트
            foreach (var citem in lstCopiedRoi)
            { 
                foreach (var selectedRoi in lstSelectedRoi)
                {
                    InspItem item = InspItemStorage.Instance.InspItemList.Cast<InspItem>()
                        .Where(InspItem => InspItem.RoiCode == citem.RoiCode)
                        .Where(InspItem => InspItem.InspName == citem.InspName)
                        .First(InspItem => InspItem.ThreshType == citem.ThreshType);

                    InspItemStorage.Instance.InspItemList.Cast<InspItem>()
                        .Where(InspItem => InspItem.RoiCode == selectedRoi)
                        .Where(InspItem => InspItem.InspName == citem.InspName)
                        .First(InspItem => InspItem.ThreshType == citem.ThreshType).Paste(item);
                }
            }

            //4. DB, 티칭데이터 업데이트
            foreach (var citem in lstCopiedRoi)
            {
                foreach (var selectedRoi in lstSelectedRoi)
                {
                    InspItem item = InspItemStorage.Instance.InspItemList.Cast<InspItem>()
                        .Where(InspItem => InspItem.RoiCode == selectedRoi)
                        .Where(InspItem => InspItem.InspName == citem.InspName)
                        .First(InspItem => InspItem.ThreshType == citem.ThreshType);

                    UpdateDB(item);
                }
            }

            //5. 사용한 Section에 대해서만 TeachingWindow Reload
            List<string> lstSelectedSection = new List<string>();
            foreach (InspItem item in paramGrid.SelectedItems)
                if (!lstSelectedSection.Contains(item.WorkType.Name)) lstSelectedSection.Add(item.WorkType.Name);

            foreach (var section in lstSelectedSection)
            {
                for (int i = 0; i < MainWindow.m_nTotalTeachingViewCount; i++)
                {
                    if (section == ptrTeachingViewer.CamView[i].sSurface)
                        ptrTeachingViewer.CamView[i].LoadTeachingData();
                }
            
                //if (section == "BA1") ptrTeachingViewer.CamBA1View.LoadTeachingData();
                //else if (section == "BA2") ptrTeachingViewer.CamBA2View.LoadTeachingData();
                //else if (section == "BP1") ptrTeachingViewer.CamBP1View.LoadTeachingData();
                //else if (section == "BP2") ptrTeachingViewer.CamBP2View.LoadTeachingData();
                //else if (section == "CA1") ptrTeachingViewer.CamCA1View.LoadTeachingData();
                //else if (section == "CA2") ptrTeachingViewer.CamCA2View.LoadTeachingData();
            }
        }
        private void SetDataByCopiedRoicode()
        {
            int cntCopiedRoiInsp = paramGrid.Items.Cast<InspItem>().Where(InspItem => InspItem.RoiCode == copiedRoiCode).Count();

            var filteredInspitems = paramGrid.Items.Cast<InspItem>()
                .Where(InspItem => InspItem.RoiCode != copiedRoiCode)
                .GroupBy(x => x.RoiCode)
                .Where(x => x.Count() == cntCopiedRoiInsp)
                .SelectMany(x => x.ToList());

            paramGrid.ItemsSource = filteredInspitems.ToList();
        }
        private void InitData()
        {
            paramGrid.SelectedItem = null;
            isCopied = false;
            txtSelectedRoiCode.Text = "없음";
            copiedRoiCode = "";
            cntCopiedRoiInsp = 0;
            paramGrid.ItemsSource = InspItemStorage.Instance.InspItemList;
            lstCopiedInspCode = null;
            lstCopiedRoi = null;
        }
        private void UpdateDB(InspItem item)
        {
            switch (item.InspectionType.InspectType)
            {
                case eVisInspectType.eInspTypeUnitAlign: // Unit Align
                    FiducialAlignProperty alignProperty = item.InspectionAlgorithm as FiducialAlignProperty;
                    if (alignProperty != null)
                    {
                        alignProperty.AlignAcceptance = item.AlignAcceptance;
                        alignProperty.AlignMarginX = item.AlignMarginX;
                        alignProperty.AlignMarginY = item.AlignMarginY;
                        item.InspectionAlgorithm = alignProperty;
                    }
                    break;
                case eVisInspectType.eInspTypeUnitFidu:
                    CrossPatternProperty crossPatternProperty = item.InspectionAlgorithm as CrossPatternProperty;
                    if (crossPatternProperty != null)
                    {
                        #region Allocate CrossPatternProperty
                        crossPatternProperty.LowerThresh = item.LowerThresh;
                        crossPatternProperty.UpperThresh = item.UpperThresh;
                        crossPatternProperty.UsePSRShift = item.UsePSRShift;
                        crossPatternProperty.UsePSRShiftBA = item.UsePSRShiftBA;
                        crossPatternProperty.PsrMarginX = item.PsrMarginX;
                        crossPatternProperty.PsrMarginY = item.PsrMarginY;
                        crossPatternProperty.UsePunchShift = item.UsePunchShift;
                        crossPatternProperty.MinDefectSize = item.MinDefectSize;
                        crossPatternProperty.MinHeightsize = item.MinHeightSize;
                        item.InspectionAlgorithm = crossPatternProperty;
                        #endregion
                    }
                    break;
                case eVisInspectType.eInspTypeSurface: // 표면 검사
                case eVisInspectType.eInspTypePSR: // PSR 검사
                case eVisInspectType.eInspTypeDownSet: // Via 검사
                    StateAlignedMaskProperty stateAlignedProperty = item.InspectionAlgorithm as StateAlignedMaskProperty;
                    if (stateAlignedProperty != null)
                    {
                        #region Allocate StateAlignedMaskProperty
                        stateAlignedProperty.LowerThresh = item.LowerThresh;
                        stateAlignedProperty.UpperThresh = item.UpperThresh;
                        stateAlignedProperty.MaskLowerThresh = item.MaskLowerThresh;
                        stateAlignedProperty.MaskUpperThresh = item.MaskUpperThresh;
                        stateAlignedProperty.ApplyAverDiff = (item.ApplyAverDiff == true) ? 1 : 0;
                        stateAlignedProperty.AverMinMargin = item.AverMinMargin;
                        stateAlignedProperty.AverMaxMargin = item.AverMaxMargin;
                        stateAlignedProperty.MinMargin = item.MinMargin;
                        stateAlignedProperty.MaxMargin = item.MaxMargin;
                        stateAlignedProperty.ErosionTrainIter = item.ErosionTrainIter;
                        stateAlignedProperty.DilationTrainIter = item.DilationTrainIter;
                        stateAlignedProperty.ErosionInspIter = item.ErosionInspIter;
                        stateAlignedProperty.DilationInspIter = item.DilationInspIter;
                        stateAlignedProperty.MinDefectSize = item.MinDefectSize;
                        stateAlignedProperty.MaxDefectSize = item.MaxDefectSize;
                        stateAlignedProperty.MinSmallDefectSize = item.MinSmallDefectSize;
                        stateAlignedProperty.MaxSmallDefectSize = item.MaxSmallDefectSize;
                        stateAlignedProperty.MinSmallDefectCount = item.MinSmallDefectCount;
                        stateAlignedProperty.MaxSmallDefectCount = item.MaxSmallDefectCount;
                        item.InspectionAlgorithm = stateAlignedProperty;
                        #endregion
                    }
                    break;
                case eVisInspectType.eInspTypeLeadShapeWithCL: // 리드 형상 검사
                    LeadShapeWithCenterLineProperty leadShapeProperty = item.InspectionAlgorithm as LeadShapeWithCenterLineProperty;
                    if (leadShapeProperty != null)
                    {
                        #region Allocate LeadShapeWithCenterLineProperty
                        leadShapeProperty.LowerThresh = item.LowerThresh;
                        leadShapeProperty.UpperThresh = item.UpperThresh;
                        leadShapeProperty.MaskLowerThresh = item.MaskLowerThresh;
                        leadShapeProperty.MinDefectSize = item.MinDefectSize;

                        leadShapeProperty.MinNormalRatio = item.LeadMinNormalRatio; 
                        leadShapeProperty.MaxNormalRatio = item.LeadMaxNormalRatio; 
                        leadShapeProperty.MinHeightsize = item.LeadMinHeightSize;
                        leadShapeProperty.MinWidthRatio = item.LeadMinWidthRatio;
                        leadShapeProperty.MaxWidthRatio = item.LeadMaxWidthRatio;
                        leadShapeProperty.MinWidthSize = item.LeadMinWidthSize;
                        leadShapeProperty.MaxWidthSize = item.LeadMaxWidthSize;
                        item.InspectionAlgorithm = leadShapeProperty;
                        #endregion
                    }
                    break;
                case eVisInspectType.eInspTypeSpaceShapeWithCL: // 공간 형상 검사
                    SpaceShapeWithCenterLineProperty spaceShapeProperty = item.InspectionAlgorithm as SpaceShapeWithCenterLineProperty;
                    if (spaceShapeProperty != null)
                    {
                        #region Allocate SpaceShapeWithCenterLineProperty
                       spaceShapeProperty.LowerThresh = item.LowerThresh;
                       spaceShapeProperty.UpperThresh = item.UpperThresh;
                       spaceShapeProperty.MinNormalRatio = item.ShapeMinNormalRatio;
                       spaceShapeProperty.MaxNormalRatio = item.ShapeMaxNormalRatio; 
                       spaceShapeProperty.MinHeightsize = item.ShapeMinHeightSize;
                       spaceShapeProperty.MinWidthRatio = item.ShapeMinWidthRatio;
                       spaceShapeProperty.MaxWidthRatio = item.ShapeMaxWidthRatio;
                       spaceShapeProperty.MinWidthSize = item.ShapeMinWidthSize;
                        spaceShapeProperty.MaxWidthSize = item.ShapeMaxWidthSize;
                        item.InspectionAlgorithm = spaceShapeProperty;
                        #endregion
                    }
                    break;
                case eVisInspectType.eInspTypeVentHole: // VentHole 검사
                    VentHoleProperty ventHoleProperty = item.InspectionAlgorithm as VentHoleProperty;
                    if (ventHoleProperty != null)
                    {
                        #region Allocate ventHoleProperty
                        ventHoleProperty.LowerThresh = item.LowerThresh;
                        ventHoleProperty.MinDefectSize = item.MinDefectSize;
                        item.InspectionAlgorithm = ventHoleProperty;
                        #endregion
                    }
                    break;

                case eVisInspectType.eInspTypeNoResizeVentHole: // NoReSize Vent Hole 검사
                    VentHoleProperty2 ventHoleProperty2 = item.InspectionAlgorithm as VentHoleProperty2;
                    if (ventHoleProperty2 != null)
                    {
                        #region Allocate ventHoleProperty
                        ventHoleProperty2.LowerThresh = item.LowerThresh;
                        ventHoleProperty2.MinDefectSize = item.MinDefectSize;
                        item.InspectionAlgorithm = ventHoleProperty2;
                        #endregion
                    }
                    break;
                case eVisInspectType.eInspTypeUnitRawMaterial: // 원소재 검사
                    RawMetrialProperty rawMaterialProperty = item.InspectionAlgorithm as RawMetrialProperty;
                    if (rawMaterialProperty != null)
                    {
                        #region Allocate RawMetrialProperty
                        rawMaterialProperty.LowerThresh = item.LowerThresh;
                        rawMaterialProperty.UpperThresh = item.UpperThresh;
                        rawMaterialProperty.ApplyAverDiff = item.ApplyAverDiff == true ? 1 : 0;
                        rawMaterialProperty.MaskLowerThresh = item.MaskLowerThresh;
                        rawMaterialProperty.MaskUpperThresh = item.MaskUpperThresh;
                        rawMaterialProperty.AverMinMargin = item.AverMinMargin;
                        rawMaterialProperty.AverMaxMargin = item.AverMaxMargin;
                        rawMaterialProperty.MinMargin = item.MinMargin;
                        rawMaterialProperty.MaxMargin = item.MaxMargin;
                        rawMaterialProperty.ErosionTrainIter = item.ErosionTrainIter;
                        rawMaterialProperty.DilationTrainIter = item.DilationTrainIter;
                        rawMaterialProperty.ErosionInspIter = item.ErosionInspIter;
                        rawMaterialProperty.DilationInspIter = item.DilationInspIter;
                        rawMaterialProperty.MinDefectSize = item.MinDefectSize;
                        rawMaterialProperty.MinSmallDefectSize = item.MinSmallDefectSize;
                        rawMaterialProperty.MinSmallDefectCount = item.MinSmallDefectCount;
                        rawMaterialProperty.MaxDefectSize = item.MaxDefectSize;
                        rawMaterialProperty.MaxSmallDefectSize = item.MaxDefectSize;
                        rawMaterialProperty.MaxSmallDefectCount = item.MaxSmallDefectSize;
                        item.InspectionAlgorithm = rawMaterialProperty;
                        #endregion
                    }
                    break;
                case eVisInspectType.eInspTypeOuter: // 외곽 검사
                    OuterProperty outerProperty = item.InspectionAlgorithm as OuterProperty;
                    if (outerProperty != null)
                    {
                        #region Allocate OuterProperty
                        outerProperty.LowerThresh = item.LowerThresh;
                        outerProperty.UpperThresh = item.UpperThresh;
                        outerProperty.MaskLowerThresh = item.MaskLowerThresh;
                        outerProperty.MaskUpperThresh = item.MaskUpperThresh;
                        outerProperty.ErosionTrainIter = item.ErosionTrainIter;
                        outerProperty.DilationTrainIter = item.DilationTrainIter;
                        outerProperty.MinDefectSize = item.MinDefectSize;
                        outerProperty.MaxDefectSize = item.MaxDefectSize;
                        item.InspectionAlgorithm = outerProperty;
                        #endregion
                    }
                    break;
                case eVisInspectType.eInspTypePSROdd:
                    PSROddProperty psrOddProperty = item.InspectionAlgorithm as PSROddProperty;
                    if (psrOddProperty != null)
                    {
                        #region Allocate psrOddProperty

                        //Common
                        psrOddProperty.ThreshType = item.PSR_ThreshType;
                        //Core_RGB
                        psrOddProperty.Core_Threshold = item.Core_Threshold;
                        psrOddProperty.Core_ExceptionThreshold = item.Core_ExceptionThreshold;
                        psrOddProperty.Core_MinDefectSize = item.Core_MinDefectSize;
                        //Metal_채도
                        psrOddProperty.Metal_LowerThreshold = item.Metal_LowerThreshold;
                        psrOddProperty.Metal_UpperThreshold = item.Metal_UpperThreshold;
                        psrOddProperty.Metal_MinDefectSize = item.Metal_MinDefectSize;
                        //Circuit
                        psrOddProperty.Circuit_Threshold = item.Circuit_Threshold;
                        psrOddProperty.Circuit_MinDefectSize = item.Circuit_MinDefectSize;
                        // Nomal
                        psrOddProperty.Summation_range = item.Summation_range;
                        psrOddProperty.Summation_detection_size = item.Summation_detection_size;
                        psrOddProperty.Mask_Threshold = item.Mask_Threshold;
                        psrOddProperty.Mask_Extension = item.Mask_Extension;
                        psrOddProperty.Step_Threshold = item.Step_Threshold;
                        psrOddProperty.Step_Expansion = item.Step_Expansion;

                        //필터
                        psrOddProperty.HV_ratio_value = item.HV_ratio_value;
                        psrOddProperty.Min_Relative_size = item.Min_Relative_size;
                        psrOddProperty.Max_Relative_size = item.Max_Relative_size;
                        psrOddProperty.Area_Relative = item.Area_Relative;

                        item.InspectionAlgorithm = psrOddProperty;
                        #endregion
                    }
                    break;
                case eVisInspectType.eInspResultGV:
                    GV_Inspection_Property gv_Inspection_Property = item.InspectionAlgorithm as GV_Inspection_Property;
                    if (gv_Inspection_Property != null)
                    {
                        #region Allocate GV_Inspection_Property
                        gv_Inspection_Property.Ball_Thresh = item.GV_Ball_Thresh;
                        gv_Inspection_Property.Core_Thresh = item.GV_Core_Thresh;
                        gv_Inspection_Property.Mask = item.GV_Mask;
                        gv_Inspection_Property.Taget_GV = item.GV_Taget;
                        gv_Inspection_Property.GV_Margin = item.GV_Margin;

                        item.InspectionAlgorithm = gv_Inspection_Property;
                        #endregion
                    }
                    break;
                case eVisInspectType.eInspTypeExceptionalMask: // 검사 제외
                    ExceptionalMaskProperty exceptionalMaskProperty = item.InspectionAlgorithm as ExceptionalMaskProperty;
                    if (exceptionalMaskProperty != null)
                    {
                        #region Allocate unitPatternProperty
                        exceptionalMaskProperty.ExceptionX = item.ExceptionX;
                        exceptionalMaskProperty.ExceptionY = item.ExceptionY;
                        exceptionalMaskProperty.UseShapeShift = item.UseShapeShift;
                        item.InspectionAlgorithm = exceptionalMaskProperty;
                        #endregion
                    }
                    break;
                case eVisInspectType.eInspTypeIDMark: // ID Mark
                    IDAreaProperty iDMarkProperty = item.InspectionAlgorithm as IDAreaProperty;
                    if (iDMarkProperty != null)
                    {
                        #region ID Mark Property
                        item.InspectionAlgorithm = iDMarkProperty;
                        #endregion
                    }
                    break;
                default: // 그 외의 검사 Item인 경우 pass.
                    break;
            }

            item.InspectionAlgorithm.UpdateProperty(currentModel, item.RoiCode, item.InspectionID);
        }
        #endregion

        #region 데이터그리드 구성
        List<DataGridColumn> m_listAll = new List<DataGridColumn>();
        List<DataGridColumn> m_listAlign = new List<DataGridColumn>();
        private ObjectDataProvider ObjectDataProvider
        {
            get { return this.TryFindResource("InspItemData") as ObjectDataProvider; }
        }
        private void InitializeEvent()
        {
            radAll.Click += chkInspType_Click;
            radUnitAlign.Click += chkInspType_Click;
            radFiducialMark.Click += chkInspType_Click;
            radSurface.Click += chkInspType_Click;
            radPSR.Click += chkInspType_Click;
            radLeadShape.Click += chkInspType_Click;
            radSpaceShape.Click += chkInspType_Click;
            radVentHole.Click += chkInspType_Click;
            radRaw.Click += chkInspType_Click;
            radOuter.Click += chkInspType_Click;
            radVia.Click += chkInspType_Click;
            radExcept.Click += chkInspType_Click;

            this.KeyDown += ParamTemplateWindow_KeyDown;
            this.KeyUp += ParamTemplateWindow_KeyUp;
        }
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        private void ShowTemplateSummary(List<List<SectionInformation>> Sections)
        {
            InspItemStorage.Instance.GenerateData(Sections);
            txtSectionCount.Text = string.Format("{0} 개", InspItemStorage.Instance.SectionCount.ToString());
            txtROICount.Text = string.Format("{0} 개", InspItemStorage.Instance.ROICount.ToString());
            txtInspCount.Text = string.Format("{0} 개", InspItemStorage.Instance.InspCount.ToString());
        }
        private void chkInspType_Click(object sender, RoutedEventArgs e)
        {
            UpdateColumnVisibility();
        } 
        private void UpdateColumnVisibility()
        {
            #region default 값 초기화
            cMinDefectSize.Header = "최소검출사이즈(Min)";
            #endregion

            if (radAll.IsChecked.Value)
            {
                foreach (DataGridColumn column in paramGrid.Columns)
                    column.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                foreach (DataGridColumn column in paramGrid.Columns)
                    column.Visibility = Visibility.Collapsed;

                cWorkType.Visibility = Visibility.Visible;
                cRoiCode.Visibility = Visibility.Visible;
                cInspName.Visibility = Visibility.Visible;
                cSectionName.Visibility = Visibility.Visible;
            }

            #region Unit Align 전용 프로퍼티
            if (radUnitAlign.IsChecked.Value)
            {
                cAlignAcceptance.Visibility = Visibility.Visible;
                cAlignX.Visibility = Visibility.Visible;
                cAlignY.Visibility = Visibility.Visible;
            }
            #endregion

            #region PSRShift 전용 프로퍼티
            if (radFiducialMark.IsChecked.Value)
            {
                cThreshType.Visibility = Visibility.Visible;
                cLowerThresh.Visibility = Visibility.Visible;
                cUsePSRShift.Visibility = Visibility.Visible;     
                cUsePSRShiftBA.Visibility = Visibility.Visible;     //jiwon PSRShiftBA 
                cMinDefectSize.Visibility = Visibility.Visible;
                cMinDefectSize.Header = "두께(px)";
                cPSROpenSize.Visibility = Visibility.Visible;
                cUseFiduInsp.Visibility = Visibility.Visible;
            }
            #endregion

            #region 리드 형상, 간격 전용 프로퍼티
            if (radLeadShape.IsChecked.Value)
            {
                cLowerThresh.Visibility = Visibility.Visible;
                cUpperThresh.Visibility = Visibility.Visible;
                cLeadMinNormalRatio.Visibility = Visibility.Visible;
                cLeadMaxNormalRatio.Visibility = Visibility.Visible;
                cLeadMinHeightSize.Visibility = Visibility.Visible;
                cLeadMinWidthRatio.Visibility = Visibility.Visible;
                cLeadMaxWidthRatio.Visibility = Visibility.Visible;
                cLeadMinWidthSize.Visibility = Visibility.Visible;
                cLeadMaxWidthSize.Visibility = Visibility.Visible;
            }
            #endregion

            #region 공간 형상 검사 전용 프로퍼티
            if (radSpaceShape.IsChecked.Value)
            {
                cLowerThresh.Visibility = Visibility.Visible;
                cUpperThresh.Visibility = Visibility.Visible;
                cShapeMinNormalRatio.Visibility = Visibility.Visible;
                cShapeMaxNormalRatio.Visibility = Visibility.Visible;
                cShapeMinHeightSize.Visibility = Visibility.Visible;
                cShapeMinWidthRatio.Visibility = Visibility.Visible;
                cShapeMaxWidthRatio.Visibility = Visibility.Visible;
                cShapeMinWidthSize.Visibility = Visibility.Visible;
                cShapeMaxWidthSize.Visibility = Visibility.Visible;
            }
            #endregion

            #region 표면 검사, PSR 검사, Via 검사, 원소재 검사 전용 프로퍼티
            if (radSurface.IsChecked.Value || radPSR.IsChecked.Value || radVia.IsChecked.Value || radRaw.IsChecked.Value)
            {
                cThreshType.Visibility = Visibility.Visible;
                cLowerThresh.Visibility = Visibility.Visible;
                cUpperThresh.Visibility = Visibility.Visible;
                cMaskLowerThresh.Visibility = Visibility.Visible;
                cMaskUpperThresh.Visibility = Visibility.Visible;
                cApplyAverDiff.Visibility = Visibility.Visible;
                cAverMinMargin.Visibility = Visibility.Visible;
                cAverMaxMargin.Visibility = Visibility.Visible;
                cInspRange.Visibility = Visibility.Visible;
                cMinMargin.Visibility = Visibility.Visible;
                cMaxMargin.Visibility = Visibility.Visible;
                cErosionTrainIter.Visibility = Visibility.Visible;
                cDilationTrainIter.Visibility = Visibility.Visible;
                cErosionInspIter.Visibility = Visibility.Visible;
                cDilationInspIter.Visibility = Visibility.Visible;
                cMinDefectSize.Visibility = Visibility.Visible;
                cMaxDefectSize.Visibility = Visibility.Visible;
                //item.MinSmallDefectSize = stateAlignedProperty.MinSmallDefectSize;
                //item.MaxSmallDefectSize = stateAlignedProperty.MaxSmallDefectSize;
                //item.MinSmallDefectCount = stateAlignedProperty.MinSmallDefectCount;
                //item.MaxSmallDefectCount = stateAlignedProperty.MaxSmallDefectCount;
            }
            #endregion

            #region VentHole 검사
            if (radVentHole.IsChecked.Value)
            {
                cThreshType.Visibility = Visibility.Visible;
                cLowerThresh.Visibility = Visibility.Visible;
                cMinDefectSize.Visibility = Visibility.Visible;
            }
            #endregion

            #region 외곽 검사
            if (radOuter.IsChecked.Value)
            {
                cLowerThresh.Visibility = Visibility.Visible;
                cUpperThresh.Visibility = Visibility.Visible;
                cMaskLowerThresh.Visibility = Visibility.Visible;
                cMaskUpperThresh.Visibility = Visibility.Visible;
                cErosionTrainIter.Visibility = Visibility.Visible;
                cDilationTrainIter.Visibility = Visibility.Visible;
                cMinDefectSize.Visibility = Visibility.Visible;
                cMaxDefectSize.Visibility = Visibility.Visible;
            }
            #endregion

            #region 검사 제외
            if (radExcept.IsChecked.Value)
            {
                //item.ExceptionX = exceptionalMaskProperty.ExceptionX;
                //item.ExceptionY = exceptionalMaskProperty.ExceptionY;
                //item.UseShapeShift = exceptionalMaskProperty.UseShapeShift;
            }
            #endregion
        }
        //DB
        public int CountInspROI()
        {
            IDataReader dataReader = null;
            int count = 0;
            
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    // 기록된 ROI를 Drawing Canvas에 그린다.
                    string strQuery = string.Format("SELECT COUNT(roi_code) FROM bgadb.roi_info" + " " +
                                                    "WHERE model_code = '{0}'", MainWindow.CurrentModel.Name);

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            count = dataReader.GetInt32(0);
                        }
                        dataReader.Close();
                        return count;
                    }
                }
            }
            catch
            {
                if (dataReader != null)
                {
                    dataReader.Close();
                }
            }
            return 0;
        }
        #endregion


    }

    #region Filter Structure
    public class InspItemStorage : INotifyPropertyChanged
    {
        //InspItem Collections
        public ObservableCollection<InspItem> InspItemList
        {
            get { return inspItemList; }
            set
            {
                inspItemList = value;
                NotifyPropertyChanged("InspItemList");
            }
        }
        private ObservableCollection<InspItem> inspItemList;

        //Filter Collection
        public ObservableCollection<IdString> WorkTypeList { get; set; }
        public ObservableCollection<IdString> SectionNameList { get; set; }
        public ObservableCollection<IdString> InspNameList { get; set; }
        public ObservableCollection<IdString> ThreshTypeList { get; set; }
        public ObservableCollection<IdString> InspRangeList { get; set; }

        static InspItemStorage()
        {
            _instance = null;
        }
        private static InspItemStorage _instance;
        public static InspItemStorage Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new InspItemStorage();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public int SectionCount;
        public int ROICount;
        public int InspCount;

        private InspItemStorage()
        {
            CreateFilterString();
            inspItemList = new ObservableCollection<InspItem>();
        }
        private void CreateFilterString()
        {
            WorkTypeList = new ObservableCollection<IdString>();
            string str = "";
            int BP_Count = 1; int CA_Count = 1; int BA_Count = 1;
            for (int i = 0; i <  MainWindow.m_nTotalTeachingViewCount; i++)
            {
                MainWindow.IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(i);
                if (IndexInfo.CategorySurface == CategorySurface.BP)
                { str = "BP" + BP_Count++; }
                else if (IndexInfo.CategorySurface == CategorySurface.CA)
                { str = "CA" + CA_Count++; }
                else
                { str = "BA" + BA_Count++; }

                WorkTypeList.Add(new IdString(i, str));
            }

            //WorkTypeList.Add(new IdString(0, "BP1"));
            //WorkTypeList.Add(new IdString(1, "BP2"));
            //WorkTypeList.Add(new IdString(2, "CA1"));
            //WorkTypeList.Add(new IdString(3, "CA2"));
            //WorkTypeList.Add(new IdString(4, "BA1"));
            //WorkTypeList.Add(new IdString(5, "BA2"));

            SectionNameList = new ObservableCollection<IdString>();

            InspNameList = new ObservableCollection<IdString>();
            InspNameList.Add(new IdString(0, "Unit Align"));
            InspNameList.Add(new IdString(1, "인식키 검사"));
            InspNameList.Add(new IdString(2, "표면 검사"));
            InspNameList.Add(new IdString(3, "PSR 검사"));
            InspNameList.Add(new IdString(4, "리드 형상 검사"));
            InspNameList.Add(new IdString(5, "공간 형상 검사"));
            InspNameList.Add(new IdString(6, "Vent Hole 검사"));
            InspNameList.Add(new IdString(7, "원소재 검사"));
            InspNameList.Add(new IdString(8, "외곽 검사"));
            InspNameList.Add(new IdString(9, "Via 검사"));
            InspNameList.Add(new IdString(10, "검사 제외"));
            InspNameList.Add(new IdString(11, "ID Mark"));
            InspNameList.Add(new IdString(12, "PSR이물 검사"));

            ThreshTypeList = new ObservableCollection<IdString>();
            ThreshTypeList.Add(new IdString(0, "임계 이상"));
            ThreshTypeList.Add(new IdString(1, "임계 이하"));
            ThreshTypeList.Add(new IdString(2, "임계 범위"));

            InspRangeList = new ObservableCollection<IdString>();
            InspRangeList.Add(new IdString(0, "-"));
            InspRangeList.Add(new IdString(1, "Min"));
            InspRangeList.Add(new IdString(2, "Max"));
            InspRangeList.Add(new IdString(3, "Min/Max"));
            InspRangeList.Add(new IdString(4, "In"));
            InspRangeList.Add(new IdString(5, "Min/In"));
            InspRangeList.Add(new IdString(6, "Max/In"));
            InspRangeList.Add(new IdString(7, "Min/Max/In"));
        }
        public void GenerateData(List<List<SectionInformation>> Sections)
        {
            List<InspItem> list = new List<InspItem>();
            SectionNameList.Clear();
            for(int i = 0; i < Sections.Count; i ++)
            {
                AddSectionNameList(Sections[i]);
                if (Sections[i] != null)
                {
                    GenerateInspItems(ref list, WorkTypeList[i], Sections[i]);
                }
            }
            InspItemList = new ObservableCollection<InspItem>(list);
        }
        public void AddSectionNameList(List<SectionInformation> sections)
        {
            if (sections == null) return;

            List<string> sectionNames = new List<string>();
            foreach (SectionInformation section in sections)
                sectionNames.Add(section.Name);

            bool bNewName = true;
            foreach (string sectionName in sectionNames)
            {
                foreach (IdString val in SectionNameList)
                {
                    if (val.Name == sectionName)
                    {
                        bNewName = false;
                        break;
                    }
                }
                if (bNewName)
                    SectionNameList.Add(new IdString(SectionNameList.Count, sectionName));
            }
        }
        private void GenerateInspItems(ref List<InspItem> inspItemList, IdString workType, List<SectionInformation> sections)
        {
            if (SectionNameList.Count == 0) // 작업이 필요하지 않은 상태
                return;
            IdString sectionValue = null;
            foreach (SectionInformation section in sections)
            {
                SectionCount++;
                foreach (IdString val in SectionNameList)
                {
                    if (val.Name == section.Name)
                    {
                        sectionValue = val;
                        break;
                    }
                }

                foreach (GraphicsBase graphic in section.ROICanvas[0].GraphicsList)
                {
                    if (graphic.InspectionList == null || graphic.InspectionList.Count == 0)
                        continue;
                    else
                        ROICount++;
                    
                    foreach (InspectionItem inspItem in graphic.InspectionList)
                    {
                        InspItem item = new InspItem();
                        item.RoiCode = QueryHelper.GetCode(ROICount - 1, 4);
                        item.WorkType = workType;
                        
                        if (sectionValue != null)
                            item.SectionName = sectionValue;

                        //전체 검사 개수
                        InspCount++;

                        //검사 ID 설정
                        item.InspectionID = QueryHelper.GetCode(inspItem.ID,4);
                        item.InspectionType = inspItem.InspectionType;

                        switch (inspItem.InspectionType.InspectType)
                        {
                            case eVisInspectType.eInspTypeUnitAlign: // Unit Align
                                FiducialAlignProperty alignProperty = inspItem.InspectionAlgorithm as FiducialAlignProperty;
                                if (alignProperty != null)
                                {
                                    item.InspName = InspNameList[0];
                                    item.AlignAcceptance = alignProperty.AlignAcceptance;
                                    item.AlignMarginX = alignProperty.AlignMarginX;
                                    item.AlignMarginY = alignProperty.AlignMarginY;
                                    item.InspectionAlgorithm = alignProperty;
                                }
                                else continue;
                                break;
                            case eVisInspectType.eInspTypeUnitFidu:
                                CrossPatternProperty crossPatternProperty = inspItem.InspectionAlgorithm as CrossPatternProperty;
                                if (crossPatternProperty != null)
                                {
                                    #region Allocate CrossPatternProperty
                                    item.InspName = InspNameList[1];
                                    item.ThreshType = ThreshTypeList[crossPatternProperty.ThreshType];
                                    item.LowerThresh = crossPatternProperty.LowerThresh;
                                    item.UpperThresh = crossPatternProperty.UpperThresh;
                                    item.UsePSRShift = crossPatternProperty.UsePSRShift;
                                    item.UsePSRShiftBA = crossPatternProperty.UsePSRShiftBA;
                                    item.PsrMarginX = crossPatternProperty.PsrMarginX;
                                    item.PsrMarginY = crossPatternProperty.PsrMarginY;
                                    item.UsePunchShift = crossPatternProperty.UsePunchShift;
                                    item.MinDefectSize = crossPatternProperty.MinDefectSize;
                                    item.MinHeightSize = crossPatternProperty.MinHeightsize;
                                    item.InspectionAlgorithm = crossPatternProperty;
                                    #endregion
                                }
                                else continue;
                                break;
                            case eVisInspectType.eInspTypeSurface: // 표면 검사
                            case eVisInspectType.eInspTypePSR: // PSR 검사
                            case eVisInspectType.eInspTypeDownSet: // Via 검사
                                StateAlignedMaskProperty stateAlignedProperty = inspItem.InspectionAlgorithm as StateAlignedMaskProperty;
                                if (stateAlignedProperty != null)
                                {
                                    #region Allocate StateAlignedMaskProperty
                                    if (eVisInspectType.eInspTypeSurface == inspItem.InspectionType.InspectType) item.InspName = InspNameList[2];
                                    if (eVisInspectType.eInspTypePSR == inspItem.InspectionType.InspectType) item.InspName = InspNameList[3];
                                    if (eVisInspectType.eInspTypeDownSet == inspItem.InspectionType.InspectType) item.InspName = InspNameList[9];

                                    item.ThreshType = ThreshTypeList[stateAlignedProperty.ThreshType];
                                    item.LowerThresh = stateAlignedProperty.LowerThresh;
                                    item.UpperThresh = stateAlignedProperty.UpperThresh;
                                    item.MaskLowerThresh = stateAlignedProperty.MaskLowerThresh;
                                    item.MaskUpperThresh = stateAlignedProperty.MaskUpperThresh;
                                    item.ApplyAverDiff = (stateAlignedProperty.ApplyAverDiff == 1) ? true : false;
                                    item.AverMinMargin = stateAlignedProperty.AverMinMargin;
                                    item.AverMaxMargin = stateAlignedProperty.AverMaxMargin;
                                    item.InspRange = InspRangeList[stateAlignedProperty.InspRange];
                                    item.MinMargin = stateAlignedProperty.MinMargin;
                                    item.MaxMargin = stateAlignedProperty.MaxMargin;
                                    item.ErosionTrainIter = stateAlignedProperty.ErosionTrainIter;
                                    item.DilationTrainIter = stateAlignedProperty.DilationTrainIter;
                                    item.ErosionInspIter = stateAlignedProperty.ErosionInspIter;
                                    item.DilationInspIter = stateAlignedProperty.DilationInspIter;
                                    item.MinDefectSize = stateAlignedProperty.MinDefectSize;
                                    item.MaxDefectSize = stateAlignedProperty.MaxDefectSize;
                                    item.MinSmallDefectSize = stateAlignedProperty.MinSmallDefectSize;
                                    item.MaxSmallDefectSize = stateAlignedProperty.MaxSmallDefectSize;
                                    item.MinSmallDefectCount = stateAlignedProperty.MinSmallDefectCount;
                                    item.MaxSmallDefectCount = stateAlignedProperty.MaxSmallDefectCount;
                                    item.InspectionAlgorithm = stateAlignedProperty;
                                    #endregion
                                }
                                else continue;
                                break;
                            case eVisInspectType.eInspTypeLeadShapeWithCL: // 리드 형상 검사
                                LeadShapeWithCenterLineProperty leadShapeProperty = inspItem.InspectionAlgorithm as LeadShapeWithCenterLineProperty;
                                if (leadShapeProperty != null)
                                {
                                    #region Allocate LeadShapeWithCenterLineProperty
                                    item.InspName = InspNameList[4];
                                    item.LowerThresh = leadShapeProperty.LowerThresh;
                                    item.UpperThresh = leadShapeProperty.UpperThresh;
                                    item.MaskLowerThresh = leadShapeProperty.MaskLowerThresh;
                                    item.MinDefectSize = leadShapeProperty.MinDefectSize;
                                    item.LeadMinNormalRatio = leadShapeProperty.MinNormalRatio;
                                    item.LeadMaxNormalRatio = leadShapeProperty.MaxNormalRatio;
                                    item.LeadMinHeightSize = leadShapeProperty.MinHeightsize;
                                    item.LeadMinWidthRatio = leadShapeProperty.MinWidthRatio;
                                    item.LeadMaxWidthRatio = leadShapeProperty.MaxWidthRatio;
                                    item.LeadMinWidthSize = leadShapeProperty.MinWidthSize;
                                    item.LeadMaxWidthSize = leadShapeProperty.MaxWidthSize;
                                    item.InspectionAlgorithm = leadShapeProperty;
                                    #endregion
                                }
                                else continue;
                                break;
                            case eVisInspectType.eInspTypeSpaceShapeWithCL: // 공간 형상 검사
                                SpaceShapeWithCenterLineProperty spaceShapeProperty = inspItem.InspectionAlgorithm as SpaceShapeWithCenterLineProperty;
                                if (spaceShapeProperty != null)
                                {
                                    #region Allocate SpaceShapeWithCenterLineProperty
                                    item.InspName = InspNameList[5];

                                    item.LowerThresh = spaceShapeProperty.LowerThresh;
                                    item.UpperThresh = spaceShapeProperty.UpperThresh;
                                    item.ShapeMinNormalRatio = spaceShapeProperty.MinNormalRatio;
                                    item.ShapeMaxNormalRatio = spaceShapeProperty.MaxNormalRatio;
                                    item.ShapeMinHeightSize = spaceShapeProperty.MinHeightsize;
                                    item.ShapeMinWidthRatio = spaceShapeProperty.MinWidthRatio;
                                    item.ShapeMaxWidthRatio = spaceShapeProperty.MaxWidthRatio;
                                    item.ShapeMinWidthSize = spaceShapeProperty.MinWidthSize;
                                    item.ShapeMaxWidthSize = spaceShapeProperty.MaxWidthSize;
                                    item.InspectionAlgorithm = spaceShapeProperty;
                                    #endregion
                                }
                                else continue;
                                break;


                            case eVisInspectType.eInspTypeVentHole: // VentHole 검사
                                VentHoleProperty ventHoleProperty = inspItem.InspectionAlgorithm as VentHoleProperty;
                                if (ventHoleProperty != null)
                                {
                                    #region Allocate ventHoleProperty
                                    item.InspName = InspNameList[6];
                                    item.ThreshType = ThreshTypeList[ventHoleProperty.ThreshType];
                                    item.LowerThresh = ventHoleProperty.LowerThresh;
                                    item.MinDefectSize = ventHoleProperty.MinDefectSize;
                                    item.InspectionAlgorithm = ventHoleProperty;
                                    #endregion
                                }
                                else continue;
                                break;


                            case eVisInspectType.eInspTypeNoResizeVentHole: // VentHole 외곽검사
                                VentHoleProperty2 ventHoleProperty2 = inspItem.InspectionAlgorithm as VentHoleProperty2;
                                if (ventHoleProperty2 != null)
                                {
                                    #region Allocate ventHoleProperty
                                    item.InspName = InspNameList[6];
                                    item.ThreshType = ThreshTypeList[ventHoleProperty2.ThreshType];
                                    item.LowerThresh = ventHoleProperty2.LowerThresh;
                                    item.MinDefectSize = ventHoleProperty2.MinDefectSize;
                                    item.InspectionAlgorithm = ventHoleProperty2;
                                    #endregion
                                }
                                else continue;
                                break;

                            case eVisInspectType.eInspTypeUnitRawMaterial: // 원소재 검사
                                RawMetrialProperty rawMaterialProperty = inspItem.InspectionAlgorithm as RawMetrialProperty;
                                if (rawMaterialProperty != null)
                                {
                                    #region Allocate RawMetrialProperty
                                    item.InspName = InspNameList[7];
                                    item.ThreshType = ThreshTypeList[rawMaterialProperty.ThreshType];
                                    item.LowerThresh = rawMaterialProperty.LowerThresh;
                                    item.UpperThresh = rawMaterialProperty.UpperThresh;
                                    item.ApplyAverDiff = rawMaterialProperty.ApplyAverDiff == 0 ? false : true;
                                    item.MaskLowerThresh = rawMaterialProperty.MaskLowerThresh;
                                    item.MaskUpperThresh = rawMaterialProperty.MaskUpperThresh;
                                    item.InspRange = InspRangeList[rawMaterialProperty.InspRange];
                                    item.AverMinMargin = rawMaterialProperty.AverMinMargin;
                                    item.AverMaxMargin = rawMaterialProperty.AverMaxMargin;
                                    item.MinMargin = rawMaterialProperty.MinMargin;
                                    item.MaxMargin = rawMaterialProperty.MaxMargin;               
                                    item.ErosionTrainIter = rawMaterialProperty.ErosionTrainIter;
                                    item.DilationTrainIter = rawMaterialProperty.DilationTrainIter;
                                    item.ErosionInspIter = rawMaterialProperty.ErosionInspIter;
                                    item.DilationInspIter = rawMaterialProperty.DilationInspIter;
                                    item.MinDefectSize = rawMaterialProperty.MinDefectSize;
                                    item.MinSmallDefectSize = rawMaterialProperty.MinSmallDefectSize;
                                    item.MinSmallDefectCount = rawMaterialProperty.MinSmallDefectCount;
                                    item.MaxDefectSize = rawMaterialProperty.MaxDefectSize;
                                    item.MaxSmallDefectSize = rawMaterialProperty.MaxDefectSize;
                                    item.MaxSmallDefectCount = rawMaterialProperty.MaxSmallDefectSize;
                                    item.InspectionAlgorithm = rawMaterialProperty;
                                    //item.SumDefectSize = rawMaterialProperty.SumDefectSize;
                                    //item.SameValue = rawMaterialProperty.SameValue;
                                    #endregion
                                 }
                                else continue;
                                break;
                            case eVisInspectType.eInspTypeOuter: // 외곽 검사
                                OuterProperty outerProperty = inspItem.InspectionAlgorithm as OuterProperty;
                                if (outerProperty != null)
                                {
                                    #region Allocate OuterProperty
                                    item.InspName = InspNameList[8];
                                    item.LowerThresh = outerProperty.LowerThresh;
                                    item.UpperThresh = outerProperty.UpperThresh;
                                    item.MaskLowerThresh = outerProperty.MaskLowerThresh;
                                    item.MaskUpperThresh = outerProperty.MaskUpperThresh;
                                    item.ErosionTrainIter = outerProperty.ErosionTrainIter;
                                    item.DilationTrainIter = outerProperty.DilationTrainIter;
                                    item.MinDefectSize = outerProperty.MinDefectSize;
                                    item.MaxDefectSize = outerProperty.MaxDefectSize;
                                    item.InspectionAlgorithm = outerProperty;
                                    #endregion
                                }
                                else continue;
                                break;
                            case eVisInspectType.eInspTypeExceptionalMask: // 검사 제외
                                ExceptionalMaskProperty exceptionalMaskProperty = inspItem.InspectionAlgorithm as ExceptionalMaskProperty;
                                if (exceptionalMaskProperty != null)
                                {
                                    #region Allocate unitPatternProperty
                                    item.InspName = InspNameList[10];
                                    item.ExceptionX = exceptionalMaskProperty.ExceptionX;
                                    item.ExceptionY = exceptionalMaskProperty.ExceptionY;
                                    item.UseShapeShift = exceptionalMaskProperty.UseShapeShift;
                                    item.InspectionAlgorithm = exceptionalMaskProperty;
                                    #endregion
                                }
                                else continue;
                                break;
                            case eVisInspectType.eInspTypeIDMark: // ID Mark
                                IDAreaProperty iDMarkProperty = inspItem.InspectionAlgorithm as IDAreaProperty;
                                if (iDMarkProperty != null)
                                {
                                    #region ID Mark Property
                                    item.InspName = InspNameList[11];
                                    item.ThreshType = ThreshTypeList[iDMarkProperty.ThreshType];
                                    item.InspectionAlgorithm = iDMarkProperty;
                                    #endregion
                                }
                                else continue;
                                break;
                            case eVisInspectType.eInspTypePSROdd:
                                PSROddProperty psrOddProperty = inspItem.InspectionAlgorithm as PSROddProperty;
                                if (psrOddProperty != null)
                                {
                                    #region Allocate OuterProperty
                                    item.InspName = InspNameList[11];
                                    //Common
                                    item.PSR_ThreshType = psrOddProperty.ThreshType;
                                    //Core_RGB
                                    item.Core_Threshold = psrOddProperty.Core_Threshold;
                                    item.Core_ExceptionThreshold = psrOddProperty.Core_ExceptionThreshold;
                                    item.Core_MinDefectSize = psrOddProperty.Core_MinDefectSize;
                                    //Metal_채도
                                    item.Metal_LowerThreshold = psrOddProperty.Metal_LowerThreshold;
                                    item.Metal_UpperThreshold = psrOddProperty.Metal_UpperThreshold;
                                    item.Metal_MinDefectSize = psrOddProperty.Metal_MinDefectSize;
                                    //Circuit
                                    item.Circuit_Threshold = psrOddProperty.Circuit_Threshold;
                                    item.Circuit_MinDefectSize = psrOddProperty.Circuit_MinDefectSize;
                                    // Nomal
                                    item.Summation_range = psrOddProperty.Summation_range;
                                    item.Summation_detection_size = psrOddProperty.Summation_detection_size;
                                    item.Mask_Threshold = psrOddProperty.Mask_Threshold;
                                    item.Mask_Extension = psrOddProperty.Mask_Extension;
                                    item.Step_Threshold = psrOddProperty.Step_Threshold;
                                    item.Step_Expansion = psrOddProperty.Step_Expansion;
                                    //필터
                                    item.HV_ratio_value = psrOddProperty.HV_ratio_value;
                                    item.Min_Relative_size = psrOddProperty.Min_Relative_size;
                                    item.Max_Relative_size = psrOddProperty.Max_Relative_size;
                                    item.Area_Relative = psrOddProperty.Area_Relative;

                                    #endregion
                                }
                                else continue;
                                break;
                            case eVisInspectType.eInspResultGV:
                                GV_Inspection_Property gv_Inspection_Property = inspItem.InspectionAlgorithm as GV_Inspection_Property;
                                if (gv_Inspection_Property != null)
                                {
                                    #region Allocate OuterProperty
                                    item.InspName = InspNameList[11];             
                                    item.GV_Ball_Thresh = gv_Inspection_Property.Ball_Thresh;
                                    item.GV_Core_Thresh = gv_Inspection_Property.Core_Thresh;
                                    item.GV_Mask = gv_Inspection_Property.Mask;
                                    item.GV_Taget = gv_Inspection_Property.Taget_GV;
                                    item.GV_Margin = gv_Inspection_Property.GV_Margin;
                                    #endregion
                                }
                                else continue;
                                break;
                            default: // 그 외의 검사 Item인 경우 pass.
                                continue;
                        }
                        inspItemList.Add(item);
                    }
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class IdString
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IdString(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
    public class InspItem
    {
        //Header
        public InspectionType InspectionType { get; set; }
        public InspectionAlgorithm InspectionAlgorithm { get; set; }
        public string InspectionID { get; set; }
        public string RoiCode { get; set; }
        public IdString WorkType { get; set; }
        public IdString SectionName { get; set; }
        public IdString InspName { get; set; }

        public IdString ThreshType { get; set; } // 임계 구분; 임계 이상 | 임계 이하 | 임계 범위
        public int LowerThresh { get; set; } // 하한 임계
        public int UpperThresh { get; set; } // 상한 임계
        public int SumDefectSize { get; set; } // 불량 Merge Size

        public int MaskLowerThresh { get; set; } // 마스크 하한임계
        public int MaskUpperThresh { get; set; } // 마스크 상한임계

        public bool ApplyAverDiff { get; set; } // 평균값 적용
        public int AverMinMargin { get; set; } // 평균값 최소범위
        public int AverMaxMargin { get; set; } // 평균값 최대범위

        public IdString InspRange { get; set; } // 검사조건 범위; Min | Max | In
        public int MinMargin { get; set; } // 검사 최소 허용범위
        public int MaxMargin { get; set; } // 검사 최대 허용범위

        public int ErosionTrainIter { get; set; } // 마스크 축소
        public int DilationTrainIter { get; set; } // 마스크 확장

        public int ErosionInspIter { get; set; } // 결과 축소
        public int DilationInspIter { get; set; } // 결과 확장

        public int MinDefectSize { get; set; } // 최소 검출사이즈(Min)
        public int MaxDefectSize { get; set; } // 최소 검출사이즈(Max)

        public int MinWidthSize { get; set; } // 최소 검출사이즈(Max)
        public int MinHeightSize { get; set; } // 최소 검출사이즈(Max)
        public int MaxHeightSize { get; set; } // 최소 검출사이즈(Max)

        public int MinSmallDefectSize { get; set; } // 미세불량 최소 검출사이즈(Min)
        public int MaxSmallDefectSize { get; set; } // 미세불량 최소 검출사이즈(Max)

        public int MinSmallDefectCount { get; set; } // 미세불량 최소 검출개수(Min)
        public int MaxSmallDefectCount { get; set; } // 미세불량 최소 검출개수(Max)

        // 리드 형상, 간격 전용 프로퍼티
        public int LeadMinNormalRatio { get; set; } // 리드편차 최소 허용범위(%)
        public int LeadMaxNormalRatio { get; set; } // 리드편차 최대 허용범위(%)
        public int LeadMinHeightSize { get; set; } // 리드 연속 불량 개수
        public int LeadMinWidthRatio { get; set; } // 리드 최소 선폭범위(%)
        public int LeadMaxWidthRatio { get; set; } // 리드 최대 선폭범위(%)
        public int LeadMinWidthSize { get; set; } // 리드 최소 선폭(px) | 리드 최소선폭 허용범위(px)
        public int LeadMaxWidthSize { get; set; } // 리드 최대 선폭(px)

        // 공간 형상 검사 전용 프로퍼티
        public int ShapeMinNormalRatio { get; set; } // 공간편차 최소 허용범위(%)
        public int ShapeMaxNormalRatio { get; set; } // 공간편차 최대 허용범위(%)
        public int ShapeMinHeightSize { get; set; } // 공간 연속 불량 개수
        public int ShapeMinWidthRatio { get; set; } // 공간 최소 선폭범위(%)
        public int ShapeMaxWidthRatio { get; set; } // 공간 최대 선폭범위(%)
        public int ShapeMinWidthSize { get; set; } // 공간 최소 선폭(px)
        public int ShapeMaxWidthSize { get; set; } // 공간 최대 선폭(px)

        // Unit Align 전용 프로퍼티
        public int AlignMarginX { get; set; } // X Margin
        public int AlignMarginY { get; set; } // Y Margin
        public int AlignAcceptance { get; set; } // 일치율

        public int UsePSRShift { get; set; }
        public int UsePSRShiftBA { get; set; }
        public int UsePunchShift { get; set; } 
        public int PsrMarginX { get; set; }
        public int PsrMarginY { get; set; }

        public int ExceptionX { get; set; }
        public int ExceptionY { get; set; }
        public int UseShapeShift { get; set; }

        // GV_Inspection 전용 프로퍼티
        public int GV_Ball_Thresh { get; set; }
        public int GV_Core_Thresh { get; set; }
        public int GV_Mask { get; set; }
        public int GV_Taget { get; set; }
        public int GV_Margin { get; set; }




        // PSR 하지이물 전용 프로퍼티
        //Common
        public int PSR_ThreshType { get; set; }
       
        //Core_RGB
        public int Core_Threshold { get; set; }
        public int Core_ExceptionThreshold { get; set; }
        public int Core_MinDefectSize { get; set; }
        
        //Metal_채도
        public int Metal_LowerThreshold { get; set; }
        public int Metal_UpperThreshold { get; set; }
        public int Metal_MinDefectSize { get; set; }
       
        //Circuit
        public int Circuit_Threshold { get; set; }
        public int Circuit_MinDefectSize { get; set; }
       
        // Nomal
        public int Summation_range { get; set; }
        public int Summation_detection_size { get; set; }
        public int Mask_Threshold { get; set; }
        public int Mask_Extension { get; set; }
        public int Step_Threshold { get; set; }
        public int Step_Expansion { get; set; }
        
        //필터
        public int HV_ratio_value { get; set; }
        public int Min_Relative_size { get; set; }
        public int Max_Relative_size { get; set; }
        public int Area_Relative {  get; set; }

        public void Paste(InspItem item)
        {
            this.ThreshType = item.ThreshType;
            this.LowerThresh = item.LowerThresh;
            this.UpperThresh = item.UpperThresh;

            this.MaskLowerThresh = item.MaskLowerThresh;
            this.MaskUpperThresh = item.MaskUpperThresh;

            this.ApplyAverDiff = item.ApplyAverDiff;
            this.AverMinMargin = item.AverMinMargin;
            this.AverMaxMargin = item.AverMaxMargin;

            this.InspRange = item.InspRange;
            this.MinMargin = item.MinMargin;
            this.MaxMargin = item.MaxMargin;

            this.ErosionTrainIter = item.ErosionTrainIter;
            this.DilationTrainIter = item.DilationTrainIter;

            this.ErosionInspIter = item.ErosionInspIter;
            this.DilationInspIter = item.DilationInspIter;

            this.MinDefectSize = item.MinDefectSize;
            this.MaxDefectSize = item.MaxDefectSize;

            this.MinWidthSize = item.MinWidthSize;
            this.MinHeightSize = item.MinHeightSize;
            this.MaxHeightSize = item.MaxHeightSize;

            this.MinSmallDefectSize = item.MinSmallDefectSize;
            this.MaxSmallDefectSize = item.MaxSmallDefectSize;

            this.MinSmallDefectCount = item.MinSmallDefectCount;
            this.MaxSmallDefectCount = item.MaxSmallDefectCount;

            this.LeadMinNormalRatio = item.LeadMinNormalRatio;
            this.LeadMaxNormalRatio = item.LeadMaxNormalRatio;
            this.LeadMinHeightSize = item.LeadMinHeightSize;
            this.LeadMinWidthRatio = item.LeadMinWidthRatio;
            this.LeadMaxWidthRatio = item.LeadMaxWidthRatio;
            this.LeadMinWidthSize = item.LeadMinWidthSize;
            this.LeadMaxWidthSize = item.LeadMaxWidthSize;

            // 공간 형상 검사 전용 프로퍼티
            this.ShapeMinNormalRatio = item.ShapeMinNormalRatio;
            this.ShapeMaxNormalRatio = item.ShapeMaxNormalRatio;
            this.ShapeMinHeightSize = item.ShapeMinHeightSize;
            this.ShapeMinWidthRatio = item.ShapeMinWidthRatio;
            this.ShapeMaxWidthRatio = item.ShapeMaxWidthRatio;
            this.ShapeMinWidthSize = item.ShapeMinWidthSize;
            this.ShapeMaxWidthSize = item.ShapeMaxWidthSize;

            // Unit Align 전용 프로퍼티
            this.AlignMarginX = item.AlignMarginX;
            this.AlignMarginY = item.AlignMarginY;
            this.AlignAcceptance = item.AlignAcceptance;

            this.UsePSRShift = item.UsePSRShift;
            this.UsePSRShiftBA = item.UsePSRShiftBA;
            this.UsePunchShift = item.UsePunchShift;
            this.PsrMarginX = item.PsrMarginX;
            this.PsrMarginY = item.PsrMarginY;

            this.ExceptionX = item.ExceptionX;
            this.ExceptionY = item.ExceptionY;
            this.UseShapeShift = item.UseShapeShift;
    }
        //// 도금, Tape 전용 프로퍼티
        //public int GroundLowerThresh { get; set; } // 전체 Mask 하한
        //public int GroundUpperThresh { get; set; } // 전체 Mask 상한
        //public int ShapeLowerThresh { get; set; } // 형상 Mask 하한
        //public int ShapeUpperThresh { get; set; } // 형상 Mask 상한
        //public int ErosionShapeIter { get; set; } // 형상 Mask 축소
        //public int DilationShapeIter { get; set; } // 형상 Mask 확장
        //public int ShapeShiftMarginX { get; set; } // 형상 Shift X
        //public int ShapeShiftMarginY { get; set; } // 형상 Shift Y
        //public int ShapeOffsetX { get; set; } // 형상 Offset X
        //public int ShapeOffsetY { get; set; } // 형상 Offset Y

        //// 도금, Tape의 형상 영역 전용 프로퍼티
        //public bool MasterUse { get; set; } // 형상 영역 검사 적용
        //public IdString MasterThreshType { get; set; } // 형상 영역 임계 구분
        //public int MasterThreshold { get; set; } // 형상 영역 임계 값
        //public int MasterLowerThresh { get; set; } // 형상 영역 하한 임계
        //public int MasterUpperThresh { get; set; } // 형상 영역 상한 임계
        //public bool MasterApplyAverDiff { get; set; } // 형상 영역 평균값 적용
        //public int MasterAverMinMargin { get; set; } // 형상 영역 평균값 최소범위
        //public int MasterAverMaxMargin { get; set; } // 형상 영역 평균값 최대범위
        //public IdString MasterInspRange { get; set; } // 형상 영역 검사조건 범위; Min | Max | In
        //public int MasterMinMargin { get; set; } // 형상 영역 검사최소 허용범위
        //public int MasterMaxMargin { get; set; } // 형상 영역 검사최대 허용범위
        //public int MasterMinDefectSize { get; set; } // 형상 영역 최소 검출사이즈(Min)
        //public int MasterMaxDefectSize { get; set; } // 형상 영역 최소 검출사이즈(Max)

        //// 도금, Tape의 표면 영역 전용 프로퍼티
        //public bool SlaveUse { get; set; } // 표면 영역 검사 적용
        //public IdString SlaveThreshType { get; set; } // 표면 영역 임계 구분
        //public int SlaveThreshold { get; set; } // 표면 영역 임계 값
        //public int SlaveLowerThresh { get; set; } // 표면 영역 하한 임계
        //public int SlaveUpperThresh { get; set; } // 표면 영역 상한 임계
        //public bool SlaveApplyAverDiff { get; set; } // 표면 영역 평균값 적용
        //public int SlaveAverMinMargin { get; set; } // 표면 영역 평균값 최소범위
        //public int SlaveAverMaxMargin { get; set; } // 표면 영역 평균값 최대범위
        //public IdString SlaveInspRange { get; set; } // 표면 영역 검사조건 범위; Min | Max | In
        //public int SlaveMinMargin { get; set; } // 표면 영역 검사최소 허용범위
        //public int SlaveMaxMargin { get; set; } // 표면 영역 검사최대 허용범위
        //public int SlaveMinDefectSize { get; set; } // 표면 영역 최소 검출사이즈(Min)
        //public int SlaveMaxDefectSize { get; set; } // 표면 영역 최소 검출사이즈(Max)

        //// 다운셋 전용 프로퍼티
        //public bool Invert { get; set; } // 역 검사
    }
    #endregion
}
