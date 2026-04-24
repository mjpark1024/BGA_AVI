using Common;
using Common.DataBase;
using Common.Drawing;
using Common.Drawing.InspectionInformation;
using Common.Drawing.InspectionTypeUI;
using Common.Drawing.MarkingInformation;
using HDSInspector.SubControl;
using HDSInspector.SubWindow;
using IGS;
using Marker;
using OpenCvSharp.Flann;
using PCS;
using PCS.ModelTeaching;
using PCS.ModelTeaching.OfflineTeaching;
using RMS;
using RVS.Generic.Insp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static HDSInspector.MainWindow;

namespace HDSInspector
{
    public partial class TeachingWindow : Window
    {
        private MainWindow m_ptrMainWindow;

        ParameterSetting parasetting;
        public static SymmetryType Symmetry = SymmetryType.Matrix;

        public TeachingViewerCtrl[] TeachingViewers = new TeachingViewerCtrl[MainWindow.m_nTotalTeachingViewCount];
        public bool ViewChanged { get; set; }

        // Offline Teaching 전용 변수.
        public bool IsOnLine = true; // OnLine : 장비, Simulation 상태 // OffLine : Offline Teaching 상태
        public MTSManager MtsManager { get; set; }
        public OffLineTeaching OffLineTeachingCtrl;
        public OfflineModelSelectWindow OffLineModelSelector;

        private Algo m_Algo = new Algo();
        private ResultItem[] Sorted_Results;

        public const int PSR_Saturation_Index = 4; // 노이즈가 제거된 채도 이미지
        public const int PSR_Color_Index = 5;
        public const int PSR_Blending_Index = 7;

        public const int Result_Saturation_Index = 6; // 노이즈가 제거가 되지 않은 채도 이미지
        public const int Result_Color_Index = 5;
        public const int Result_Blending_Index = 7;

        public int VRS_SectionIndex = 0;
        public int ChannelIndex
        {
            get
            {
                if ((bool)rdRed.IsChecked) return 0;
                else if ((bool)rdGreen.IsChecked) return 1;
                else if ((bool)rdBlue.IsChecked) return 2;
                else return 3;
            }
        }

        // 생성자
        public TeachingWindow()
        {
            InitializeComponent();
            InitializeDialogPointer();
            InitializeEvent();
        }

        public void SetOfflineMode()
        {
            if (!IsOnLine)
            {
                OffLineTeachingCtrl = new OffLineTeaching(this);
                Grid grid = pnlRight as Grid;
                if (grid != null)
                {
                    grid.Children.Add(OffLineTeachingCtrl);
                    grid.Children.Remove(pnlLight);

                    Grid.SetRow(OffLineTeachingCtrl, 0);
                    Grid.SetRow(pnlManualInspect, 1);

                    RowDefinitionCollection rowDefines = grid.RowDefinitions;
                    if (rowDefines.Count > 1)
                    {
                        rowDefines[0].Height = new GridLength(245);

                        GridLengthConverter gridLengthConverter = new GridLengthConverter();
                        rowDefines[1].Height = (GridLength)gridLengthConverter.ConvertFrom("*");
                    }
                }
            }
        }

        #region Initialiezer.
        // View간 연결성을 확보한다.
        private void InitializeDialogPointer()
        {
            this.InspectionTypectrl.m_ptrTeachingWindow = this;
            this.HistogramCtrl.m_ptrTeachingWindow = this;
            this.TeachingViewer.m_ptrTeachingWindow = this;
            this.InspectionTypectrl.m_ptrTeachingViewer = this.TeachingViewer;
            this.HistogramCtrl.m_ptrTeachingViewer = this.TeachingViewer;
            this.InspectionTypectrl.SaveChanged += InspectionTypectrl_SaveChanged;
           
            TeachingViewers = new TeachingViewerCtrl[MainWindow.m_nTotalTeachingViewCount];
            for(int i = 0; i < TeachingViewers.Length; i++)
            {
                TeachingViewers[i] = TeachingViewer.CamView[i];
                Surface type = Surface.BP1;
                if (i < MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count)
                    type = Surface.BP1 + i;
                else if (i >= MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count && i < MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count + MainWindow.Setting.Job.CACount)
                    type = Surface.CA1 + i;
                else
                    type = Surface.BA1 + i;
                TeachingViewers[i].SurfaceType = type;
            }

            for (int nIndex = 0; nIndex < TeachingViewers.Length; nIndex++)
            {
                TeachingViewers[nIndex].m_ptrTeachingWindow = this;
                TeachingViewers[nIndex].m_ptrTeachingViewer = this.TeachingViewer;
            }
        }

        void InspectionTypectrl_SaveChanged(InspectionItem[] list, int index, string code)
        {
            foreach (SectionInformation section in TeachingViewer.SelectedViewer.SectionManager.Sections)
            {
                if (section.Type.Code == code)
                {
                    if (code == SectionTypeCode.RAW_REGION)
                    {
                        foreach (GraphicsBase g in section.ROICanvas[(int)TeachingViewer.SelectedViewer.SelectedChannel].GraphicsList)  // ROI별 반복
                        {
                            if (g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine || g.RegionType == GraphicsRegionType.TapeLoaction || g.RegionType == GraphicsRegionType.UnitAlign)
                                continue;
                            if (g is GraphicsSelectionRectangle || g is GraphicsLine)
                                continue;

                            bool RawMaterial_check = false;

                            for (int n = 0; n< g.InspectionList.Count; n++)
                            {
                                if (g.InspectionList[n].InspectionType.InspectType != eVisInspectType.eInspTypeUnitRawMaterial)
                                    continue;
                                for (int m = 0; m < list.Length; m++)
                                {
                                    if (list[m].InspectionType.InspectType == eVisInspectType.eInspTypeUnitRawMaterial)
                                    {
                                        RawMaterial_check = true;
                                        break;
                                    }
                                }
                            }
 
                            if (RawMaterial_check)
                            {
                                InspectionItem[] tmp = new InspectionItem[list.Length];
                                list.CopyTo(tmp, 0);
                                int tmprow = 0;
                                if (g.InspectionList.Count == 0)
                                {
                                    tmprow = g.UnitRow;
                                }
                                else
                                {
                                    for (int i = 0; i < g.InspectionList.Count; i++)
                                    {
                                        RawMetrialProperty item = (RawMetrialProperty)g.InspectionList[i].InspectionAlgorithm;
                                        tmprow = item.MinSmallDefectSize;
                                    }
                                }
                                g.InspectionList.Clear();

                                for (int i = 0; i < list.Length; i++)
                                {
                                    InspectionItem itemtmp = new InspectionItem(tmp[i].InspectionType, tmp[i].InspectionAlgorithm, i);
                                    RawMetrialProperty item = (RawMetrialProperty)itemtmp.InspectionAlgorithm;
                                    item.MinSmallDefectSize = tmprow;
                                    g.InspectionList.Add(itemtmp);
                                }
                                g.RawRow = tmprow + 1;
                            }  
                        }
                    }
                    else if (SectionTypeCode.OUTER_REGION == code)// && section.RelatedROI.MotherROIID == TeachingViewer.SelectedViewer.SelectedSection.RelatedROI.MotherROIID)
                    {
                        if (TeachingViewer.SelectedViewer.SelectedSection == section)
                            continue;
                        foreach (GraphicsBase g in section.ROICanvas[(int)TeachingViewer.SelectedViewer.SelectedChannel].GraphicsList)  // ROI별 반복
                        {
                            if (g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine || g.RegionType == GraphicsRegionType.TapeLoaction || g.RegionType == GraphicsRegionType.UnitAlign || g.RegionType == GraphicsRegionType.Except)
                                continue;

                            InspectionItem[] tmp = new InspectionItem[list.Length];
                            list.CopyTo(tmp, 0);
                            g.InspectionList.Clear();
                            for (int i = 0; i < list.Length; i++)
                            {
                                g.InspectionList.Add(tmp[i]);
                            }
                        }
                    }
                }
            }
            if (SectionTypeCode.UNIT_REGION == code)
            {
                foreach (GraphicsBase g in TeachingViewer.SelectedViewer.SelectedSection.ROICanvas[(int)TeachingViewer.SelectedViewer.SelectedChannel].GraphicsList)  // ROI별 반복
                {
                    if (g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine || g.RegionType == GraphicsRegionType.TapeLoaction || g.RegionType == GraphicsRegionType.UnitAlign)
                        continue;
                    if (g is GraphicsSelectionRectangle || g is GraphicsLine)
                        continue;
                    InspectionItem[] tmp = new InspectionItem[list.Length];
                    list.CopyTo(tmp, 0);
                    foreach (InspectionItem inspItem in g.InspectionList)
                    {
                        if (inspItem.InspectionType.InspectType == eVisInspectType.eInspTypeDownSet)
                        {
                            inspItem.InspectionAlgorithm = tmp[0].InspectionAlgorithm;
                            inspItem.InspectionType = tmp[0].InspectionType;
                            break;
                        }
                    }
                }
            }
            if (SectionTypeCode.PSR_REGION == code)
            {
                foreach (GraphicsBase g in TeachingViewer.SelectedViewer.SelectedSection.ROICanvas[(int)TeachingViewer.SelectedViewer.SelectedChannel].GraphicsList)  // ROI별 반복
                {
                    if (g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine || g.RegionType == GraphicsRegionType.TapeLoaction || g.RegionType == GraphicsRegionType.UnitAlign)
                        continue;
                    if (g is GraphicsSelectionRectangle || g is GraphicsLine)
                        continue;
                    InspectionItem[] tmp = new InspectionItem[list.Length];
                    list.CopyTo(tmp, 0);
                    foreach (InspectionItem inspItem in g.InspectionList)
                    {
                        if (inspItem.InspectionType.InspectType == eVisInspectType.eInspTypePSROdd)
                        {
                            inspItem.InspectionAlgorithm = tmp[0].InspectionAlgorithm;
                            inspItem.InspectionType = tmp[0].InspectionType;
                            break;
                        }
                    }
                }
            }
            InspectionTypectrl.lbInspection.SelectedIndex = index;
        }

        // 화면 초기화
        public void InitializeDialog()
        {
            for (int nIndex = 0; nIndex < TeachingViewers.Length; nIndex++)
            {
                TeachingViewers[nIndex].InitializeDialog();
            }
            LineProfileCtrl.SetLineProfileSource(TeachingViewer.SelectedViewer.TeachingImageSource);
            InspectionTypectrl.SelectedGraphic = null;

            lvResult.Items.Clear();
            BitmapImage StandardImage = new BitmapImage();
            StandardImage.BeginInit();
            StandardImage.UriSource = new Uri(@"./Images/REF.png", UriKind.Relative);
            StandardImage.EndInit();
            ImgRef.Source = StandardImage;

            BitmapImage NGImage = new BitmapImage();
            NGImage.BeginInit();
            NGImage.UriSource = new Uri(@"./Images/DEF.png", UriKind.Relative);
            NGImage.EndInit();
            ImgDef.Source = NGImage;

            SetManualInspectState(false);
            HistogramCtrl.Refresh();
            LineProfileCtrl.Refresh();
            TeachingViewer.bVia = false;
            VRS_SectionIndex = 0;
            for (int nIndex = 0; nIndex < TeachingViewers.Length; nIndex++)
            {
                TeachingViewer.CamView[nIndex].SetByCurrentModel();
                TeachingViewer.CamView[nIndex].CreateContextMenu();
            }
            
            SetLight();

            grdCampitch.DataContext = MainWindow.CurrentModel;
        }

        public void Apply_Campitch(object sender, RoutedEventArgs e)
        {
            double temp_pitch = MainWindow.CurrentModel.XPitch;
            if (temp_pitch != Convert.ToDouble(txtXPitch.Text))
            {
                if (MessageBox.Show("카메라 이송 피치를 조절하시겠습니까?", "종료", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    MainWindow.CurrentModel.XPitch = Convert.ToDouble(txtXPitch.Text);
                    double cam1pitch = MainWindow.Setting.Job.Cam1Pitch + MainWindow.CurrentModel.XPitch;
                    if (MainWindow.CurrentModel.Strip.UnitRow % 2 == 1)
                    {
                        cam1pitch = MainWindow.Setting.Job.Cam1Pitch - (MainWindow.CurrentModel.Strip.UnitHeight / 2.0) + MainWindow.CurrentModel.XPitch;
                    }
                    cam1pitch += MainWindow.Setting.Job.Cam1Position;//cam position에 pitch를 더해서 전송

                    m_ptrMainWindow.SendPitch(cam1pitch);
                }
                else
                {
                    MainWindow.CurrentModel.XPitch = temp_pitch;
                    txtXPitch.Text = MainWindow.CurrentModel.XPitch.ToString();
                }
                PCS.ELF.AVI.ModelManager.UpdateXPitch(MainWindow.CurrentModel);
            }
        }

        public void SetLight()
        {
            IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(TeachingViewer.SelectedViewer.ID);
            LightInfo LightInfo = MainWindow.Convert_LightIndex(IndexInfo.CategorySurface, IndexInfo.Index);////////10개의 DB 버퍼공간을 활용하는 방식으로 변경
            if (this.lightcontrol.IsOpen && MainWindow.CurrentModel != null)
            {
                this.lightcontrol.LightNO = LightInfo.LightIndex;
                lightcontrol.SetValues(MainWindow.CurrentModel.LightValue, LightInfo.LightIndex, LightInfo.ValueIndex, MainWindow.Setting.SubSystem.Light.Channel);

                this.lightcontrol.LightOn(false);
                this.lightcontrol.IsEnabled = true;
                this.btnONOFF.IsEnabled = true;
                this.btnSave.IsEnabled = true;
                this.btnCancel.IsEnabled = true;
            }
            else
            {
                this.lightcontrol.IsEnabled = false;
                this.btnONOFF.IsEnabled = false;
                this.btnSave.IsEnabled = false;
                this.btnCancel.IsEnabled = false;
            }

        }

        // 이벤트 초기화
        private void InitializeEvent()
        {
            this.btnClose.Click += btnClose_Click;
            this.btnONOFF.Click += btnONOFF_Click;
            this.btnSave.Click += btnSave_Click;
            this.btnCancel.Click += btnCancel_Click;
            this.btnViewUnit.Click += btnViewUnit_Click;
            this.btnSendPara.Click += (s, e) => ReqSendVisionData();
            this.btnManualInspect.Click += (s, e) => RunManualInspect();
            this.btnPartialInspect.Click += (s, e) => RunPartialInspect();

            this.btnLoadImage.Click += btnLoadImage_Click;
            this.btnSaveImage.Click += btnSaveImage_Click;
            this.btnLoadXML.Click += btnLoadXML_Click;
            this.btnSaveXML.Click += btnSaveXML_Click;
            this.btnQuestion.Click += (s, e) =>
            {
                QuestionWindow question = new QuestionWindow();
                question.ShowDialog();
            };
            this.btnParaSetting.Click += (s, e) =>
            {
                if (MainWindow.Setting.General.UsePassword)
                {
                    Password password = new Password();
                    if (password.ShowDialog() != true)
                    {
                        return;
                    }
                }
                parasetting.ShowDialog();
            };
            this.btnGrabEntireImage.Click += (s, e) => GrabEntireImage(false);
            this.btnGrabFullImage.Click += (s, e) => GrabEntireImage(true);
            this.btnRegisterBasedImage.Click += (s, e) => RegisterBasedImage();
            this.btnSaveTeachingData.Click += (s, e) => SaveTeachingData();
            this.btnCreateSection.Click += btnCreateSection_Click;
            this.btnSaveGoldenModel.Click += btnSaveGoldenModel_Click;
            this.btn_Campitch.Click += Apply_Campitch;

            DrawingCanvas.NotifyConstraintEvent += DrawingCanvas_NotifyConstraintEvent;
            DrawingCanvas.ToolTypeChangeEvent += TeachingViewerCtrl_ToolTypeChangeEvent;
            TeachingViewerCtrl.ToolTypeChangeEvent += TeachingViewerCtrl_ToolTypeChangeEvent;
            DrawingCanvas.SectionSizeChangeEvent += DrawingCanvas_SectionSizeChangeEvent;
            VisionInterface.LiveImageUpdateEvent += VisionInterface_LiveImageUpdateEvent;

            this.Loaded += TeachingWindow_Loaded;
            this.Closing += TeachingWindow_Closing;
            this.lvResult.SelectionChanged += lvResult_SelectionChanged;
            this.lvResult.PreviewMouseWheel += lvResult_PreviewMouseWheel;
            this.lvResult.LostFocus += lvResult_LostFocus;

            this.PreviewKeyDown += TeachingWindow_PreviewKeyDown;

            this.sdRefOpacity.ValueChanged += sdRefOpacity_ValueChanged;

            this.rdRed.Checked += RGB_Checked;
            this.rdGreen.Checked += RGB_Checked;
            this.rdBlue.Checked += RGB_Checked;
            this.rdRGB.Checked += RGB_Checked;
            parasetting = new ParameterSetting();
        }



        private void RGB_Checked(object sender, RoutedEventArgs e)
        {
            RGBChange(ChannelIndex, TeachingViewer.SelectedViewer.ID);
        }

        void lvResult_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int val = e.Delta;
            if (val > 0)
            {
                if (lvResult.SelectedIndex > 0)
                {
                    lvResult.SelectedIndex--;
                }
            }
            else
            {
                if (lvResult.Items.Count - 1 > lvResult.SelectedIndex) lvResult.SelectedIndex++;
            }
        }

        private ScrollViewer FindScrollviewer(DependencyObject d)
        {
            if (d is ScrollViewer)
                return d as ScrollViewer;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var sv = FindScrollviewer(VisualTreeHelper.GetChild(d, i));
                return sv;
            }
            return null;
        }

        void sdRefOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.ImgRefTrans.Opacity = this.sdRefOpacity.Value;
        }
        #endregion

        #region Event Handlers.
        private void btnSaveGoldenModel_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
            {
                MessageBox.Show("모델을 선택해 주시기 바랍니다.", "Information");
                return;
            }

            SaveTeachingData(); 

            GoldenSaveWindow golden = new GoldenSaveWindow();
            if ((bool)golden.ShowDialog())
            {
                if (golden.SaveOK)
                {
                    GoldenHistoryWindow history = new GoldenHistoryWindow();
                    history.ShowDialog();
                }
            }
        }

        private static void DrawingCanvas_NotifyConstraintEvent(string strMessage)
        {
            MessageBox.Show(strMessage, "Information");
        }

        // Section Region Size 변경시 호출됨.
        private void DrawingCanvas_SectionSizeChangeEvent(GraphicsRectangle aNewGraphic, double afDeltaX, double afDeltaY)
        {
            TeachingViewerCtrl selectedViewer = TeachingViewer.SelectedViewer;
            if (selectedViewer != null)
            {
                if (selectedViewer.ReferenceImage == null || !selectedViewer.IsGrabDone)
                {
                    MessageBox.Show("영상 취득 후 섹션 설정을 변경해 주시기 바랍니다.", "Information");
                    selectedViewer.BasedROICanvas.RevertResize();
                    return;
                }

                if (aNewGraphic.RegionType == GraphicsRegionType.UnitRegion || aNewGraphic.RegionType == GraphicsRegionType.PSROdd)
                {
                    foreach (GraphicsRectangle g in selectedViewer.BasedROICanvas.GraphicsRectangleList)
                    {
                        if (g == aNewGraphic) continue;
                        if (g.RegionType == GraphicsRegionType.UnitRegion || g.RegionType == GraphicsRegionType.PSROdd)
                        {
                            g.Right = g.Right + afDeltaX;
                            g.Bottom = g.Bottom + afDeltaY;
                            g.CalcBoundaryRect();
                            g.RefreshDrawing();
                        }
                    }
                }

                // Section을 재생성한다.
                if (CreateSection())
                    MessageBox.Show("섹션 크기 변경으로 인해 검사 영역이 수정되었습니다.\n섹션 내 검사 영역을 재배치 바랍니다.", "Confirm");
            }
        }

        private void TeachingWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                DrawingCanvas teachingCanvas = TeachingViewer.SelectedViewer.TeachingCanvas;
                TeachingViewerCtrl teachingViewer = TeachingViewer.SelectedViewer;
                TeachingViewer.bVia = false;
                if (Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    #region DrawingCanvas에 대한 단축키.
                    if (teachingCanvas != null)
                    {
                        switch (e.Key)
                        {
                            case Key.A:
                                if (!teachingCanvas.IsBasedCanvas)
                                {
                                    teachingCanvas.SelectedGraphic = null;
                                }
                                teachingCanvas.SelectAll();
                                break;
                            case Key.V:
                                teachingCanvas.PasteGraphics();
                                break;
                            case Key.C:
                                if (!teachingCanvas.IsBasedCanvas)
                                {
                                    if (teachingCanvas.SelectionCount > 0)
                                        teachingCanvas.CopyGraphics();
                                }
                                else
                                {
                                    if (teachingCanvas.SelectionCount > 0)
                                    {
                                        foreach (GraphicsBase g in teachingCanvas.Selection)
                                        {
                                            if (g.RegionType != GraphicsRegionType.OuterRegion || g.RegionType != GraphicsRegionType.Rawmetrial)
                                            {
                                                MessageBox.Show("외곽 영역만 복사 & 붙여넣기가 가능합니다.", "Information");
                                                return;
                                            }
                                        }
                                        teachingCanvas.CopyGraphics();
                                    }
                                }
                                break;
                            case Key.Y:
                                if (!teachingCanvas.IsBasedCanvas)
                                    teachingCanvas.Redo();
                                break;
                            case Key.Z:
                                if (!teachingCanvas.IsBasedCanvas)
                                    teachingCanvas.Undo();
                                break;
                            case Key.Left:
                                if (teachingCanvas != null && teachingViewer != null && !(bool)teachingViewer.chkFixedROI.IsChecked)
                                {
                                    if (teachingCanvas.IsFocused)
                                        teachingCanvas.MoveGraphics(-1, 0);
                                }
                                break;
                            case Key.Right:
                                if (teachingCanvas != null && teachingViewer != null && !(bool)teachingViewer.chkFixedROI.IsChecked)
                                {
                                    if (teachingCanvas.IsFocused)
                                        teachingCanvas.MoveGraphics(1, 0);
                                }
                                break;
                            case Key.Up:
                                if (teachingCanvas != null && teachingViewer != null && !(bool)teachingViewer.chkFixedROI.IsChecked)
                                {
                                    if (teachingCanvas.IsFocused)
                                        teachingCanvas.MoveGraphics(0, -1);
                                }
                                break;
                            case Key.Down:
                                if (teachingCanvas != null && teachingViewer != null && !(bool)teachingViewer.chkFixedROI.IsChecked)
                                {
                                    if (teachingCanvas.IsFocused)
                                        teachingCanvas.MoveGraphics(0, 1);
                                }
                                break;
                        }
                    }
                    #endregion
                }
                else
                {
                    switch (e.Key)
                    {
                        case Key.F2:
                            if (teachingViewer != null)
                            {
                                if (teachingViewer.lbSection.Items.Count > 0)
                                {
                                    if (teachingViewer.lbSection.SelectedIndex + 1 >= teachingViewer.lbSection.Items.Count)
                                        teachingViewer.lbSection.SelectedIndex = -1;
                                    else
                                        teachingViewer.lbSection.SelectedIndex++;
                                }
                            }
                            break;
                        case Key.Escape:
                            if (teachingCanvas != null)
                            {
                                if (teachingCanvas.IsMouseCaptured)
                                    teachingCanvas.CancelCurrentOperation();
                                else
                                    teachingCanvas.UnselectAll();
                            }
                            break;
                        case Key.F3:
                            if (teachingViewer != null)
                                teachingViewer.SeeEntireImage();
                            break;
                        case Key.F4:
                            if (teachingViewer != null)
                                teachingViewer.SeeReferenceImage();
                            break;
                        case Key.F5:
                            if (btnGrabEntireImage.IsEnabled)
                                GrabEntireImage(false);
                            break;
                        case Key.F6:
                            if (btnSendPara.IsEnabled)
                            {
                                Stopwatch sw = new Stopwatch();
                                sw.Start();
                                ReqSendVisionData();
                                sw.Stop();
                                Console.WriteLine(sw.ElapsedMilliseconds);
                            }
                            break;
                        case Key.F7:
                            if (btnPartialInspect.IsEnabled)
                                RunPartialInspect();
                            break;
                        case Key.F8:
                            if (btnManualInspect.IsEnabled)
                                RunManualInspect();
                            break;
                        case Key.Delete:
                            if (teachingCanvas != null)
                                teachingCanvas.Delete();
                            break;
                        case Key.Left:
                            if (teachingCanvas != null && teachingViewer != null && !(bool)teachingViewer.chkFixedROI.IsChecked)
                            {
                                if (teachingCanvas.IsFocused)
                                    teachingCanvas.MoveGraphics(-10, 0);
                            }
                            break;
                        case Key.Right:
                            if (teachingCanvas != null && teachingViewer != null && !(bool)teachingViewer.chkFixedROI.IsChecked)
                            {
                                if (teachingCanvas.IsFocused)
                                    teachingCanvas.MoveGraphics(10, 0);
                            }
                            break;
                        case Key.Up:
                            if (teachingCanvas != null && teachingViewer != null && !(bool)teachingViewer.chkFixedROI.IsChecked)
                            {
                                if (teachingCanvas.IsFocused)
                                    teachingCanvas.MoveGraphics(0, -10);
                            }
                            break;
                        case Key.Down:
                            if (teachingCanvas != null && teachingViewer != null && !(bool)teachingViewer.chkFixedROI.IsChecked)
                            {
                                if (teachingCanvas.IsFocused)
                                    teachingCanvas.MoveGraphics(0, 10);
                            }
                            break;
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Exception occured in DrawingCanvas_KeyDown(DrawingCanvas.cs)");
            }
        }

        private void TeachingViewerCtrl_ToolTypeChangeEvent(Common.Drawing.ToolType newToolType)
        {
            TeachingViewer.SelectedViewer.TeachingSubMenuCtrl.ToolTypeChanged(newToolType);
        }
        #endregion Event Handlers.

        #region Load & Close.
        private void TeachingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            m_ptrMainWindow = Application.Current.MainWindow as MainWindow;
            Debug.Assert(m_ptrMainWindow != null);
            TeachingViewer.bVia = false;
            SectionManager.LoadSectionType();
            TeachingAlignSettingWindow.InitializeDefaultValue();

            TeachingViewer.SetViewSize(1083, 840);// TeachingViewer.SelectedViewer.svTeaching.ActualWidth, TeachingViewer.SelectedViewer.svTeaching.ActualHeight);
            TeachingViewer.SelectedViewer.CalculateZoomToFitScale();
            if (!IsOnLine)
            {
                if (MtsManager != null)
                {
                    OffLineModelSelector = new OfflineModelSelectWindow(MtsManager);
                    OffLineModelSelector.SetMachineList(ref MtsManager.MachineList);
                }
                SelectOfflineModel();
            }

            SetCameraResolution();
            if (this.lightcontrol.IsOpen && MainWindow.CurrentModel != null)
            {
                IndexInfo IndexInfo = m_ptrMainWindow.Convert_ViewIndexToVisionIndex(TeachingViewer.SelectedViewer.ID);
                LightInfo LightInfo = MainWindow.Convert_LightIndex(IndexInfo.CategorySurface, IndexInfo.Index);////////10개의 DB 버퍼공간을 활용하는 방식으로 변경
                this.lightcontrol.LightNO = LightInfo.LightIndex;
                lightcontrol.SetValues(MainWindow.CurrentModel.LightValue, LightInfo.LightIndex, LightInfo.ValueIndex, MainWindow.Setting.SubSystem.Light.Channel);
                this.lightcontrol.LightOn(false);

                TeachingViewer.set_rgbmenu();
            }
            else
            {
                this.lightcontrol.IsEnabled = false;
                this.btnONOFF.IsEnabled = false;
                this.btnSave.IsEnabled = false;
                this.btnCancel.IsEnabled = false;
            }
        }

        public void SelectOfflineModel()
        {
            bool bMachineChanged = false;
            bool bModelChanged = false;

            OffLineModelSelector.ShowDialog();
            if (MainWindow.CurrentGroup != null && MainWindow.CurrentModel != null)
            {
                MachineInformation machine = OffLineModelSelector.cmbMachineList.SelectedItem as MachineInformation;
                if (machine != null && OffLineTeachingCtrl.txtMachine.Text != machine.Name)
                {
                    MtsManager.SelectedMachine = machine;
                    if (MtsManager.SelectedMachine != null)
                    {
                        MainWindow.Setting.General.MachineCode = MtsManager.SelectedMachine.Code;
                        MainWindow.Setting.General.MachineName = MtsManager.SelectedMachine.Name;
                    }

                    OffLineTeachingCtrl.txtMachine.Text = machine.Name;
                    bMachineChanged = true;

                    // 변경된 설비 정보로 Camera 해상도를 수정한다.
                    //TeachingViewer.TopView.CamResolutionX = machine.CamResolutionX;
                    //TeachingViewer.TopView.CamResolutionY = machine.CamResolutionY;
                    //TeachingViewer.BottomView.CamResolutionX = machine.CamResolutionX;
                    //TeachingViewer.BottomView.CamResolutionY = machine.CamResolutionY;
                    //TeachingViewer.TransmissionView.CamResolutionX = machine.CamResolutionX;
                    //TeachingViewer.TransmissionView.CamResolutionY = machine.CamResolutionY;

                    MtsManager.InitVision(MainWindow.Setting.SubSystem.IS, TeachingViewer.SelectedViewer.ID);
                    TeachingViewer.SelectedViewer.IsGrabDone = false;
                }
                if (OffLineTeachingCtrl.txtModel.Text != MainWindow.CurrentModel.Name)
                {
                    OffLineTeachingCtrl.txtModel.Text = MainWindow.CurrentModel.Name;
                    bModelChanged = true;
                }

                if (bMachineChanged || bModelChanged)
                {
                    InitializeDialog();
                }
            }
        }

        private void TeachingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseDialog();
            if (IsOnLine)
            {
                e.Cancel = true;
            }
            else // !IsOnLine
            {
                if (MessageBoxResult.Yes != MessageBox.Show("프로그램을 종료하시겠습니까?", "Confirm", MessageBoxButton.YesNo))
                {
                    e.Cancel = true;
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog();
        }

        private void CloseDialog()
        {
            int nSections = 0;
            foreach (TeachingViewerCtrl viewer in TeachingViewers)
            {
                nSections += viewer.SectionManager.Sections.Count;
            }
            if (nSections > 0)
            {
                if (MessageBoxResult.Yes == MessageBox.Show("변경된 설정을 저장하시겠습니까?", "Confirm", MessageBoxButton.YesNo))
                {
                    SaveTeachingData();
                }
            }

            for(int i = 0; i < MainWindow.m_nTotalScanCount; i++)
            {
                SaveReferenceEntireImage(i, true);
                SaveReferenceSectionImages(i, true);
            }
            if (IsOnLine)
            {
                if (MainWindow.Setting.SubSystem.Light.UseLight)
                {
                    lightcontrol.LightOn(false);
                }
                this.Hide();
            }
            else // Offline Mode.
            {
                this.DialogResult = true;
            }
        }
        #endregion

        public void RGBChange(int anChannel, int anVisionID)
        {
            try
            {
                IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(anVisionID);
                int tempDesVision = IndexInfo.VisionIndex;
                int FrameGrabberIndex = VID.CalcIndex(tempDesVision);
                if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] >= 6)
                //if (tempDesVision != 0)
                {
                    if (anChannel == 0)
                        TeachingViewer.SelectedViewer.GrabImage = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].BasedImageR;
                    else if (anChannel == 1)
                        TeachingViewer.SelectedViewer.GrabImage = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].BasedImageG;
                    else if (anChannel == 2)
                        TeachingViewer.SelectedViewer.GrabImage = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].BasedImageB;
                    else
                        TeachingViewer.SelectedViewer.GrabImage = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].BasedImageRGB;
                    {
                        TeachingViewer.SelectedViewer.IsGrabView = true;
                        TeachingViewer.SelectedViewer.UpdateViewerSource(TeachingViewer.SelectedViewer.GrabImage);
                        TeachingViewer.SelectedViewer.pnlInner.Children.Clear();
                        TeachingViewer.SelectedViewer.pnlInner.Children.Add(TeachingViewer.SelectedViewer.BasedImage); // Image Control
                        TeachingViewer.SelectedViewer.pnlInner.Children.Add(TeachingViewer.SelectedViewer.BasedROICanvas); // Drawing Canvas

                        TeachingViewer.SelectedViewer.ToolChange(ToolType.Pointer);

                        bool bManu_PSRodd = true;
                        if (IndexInfo.CategorySurface == CategorySurface.BP) 
                            bManu_PSRodd = false;//BP에서는 PSR ODD 사용하면 안된다
                        TeachingViewer.SelectedViewer.TeachingSubMenuCtrl.ChangeTeachingMode(TeachingType.Entire, bManu_PSRodd);
                        TeachingViewer.SelectedViewer.ThumbnailViewer.SetSourceImage(TeachingViewer.SelectedViewer);

                        if (TeachingViewer.SelectedViewer.BasedROICanvas.GraphicsList.Count == 0)
                        {
                            TeachingViewer.SelectedViewer.LoadTeachingData(); // Teaching Data를 읽어들인다.
                            if (TeachingViewer.SelectedViewer.BasedROICanvas.GraphicsList.Count > 0)
                            {
                                MessageBox.Show("티칭 정보가 자동 갱신되었습니다.", "Teaching Data Manager");
                            }
                            TeachingViewer.SelectedViewer.UpdateViewerSource(TeachingViewer.SelectedViewer.GrabImage);
                        }
                        TeachingViewer.SelectedViewer.lbSection.SelectedIndex = -1;

                        HistogramCtrl.Refresh();
                        LineProfileCtrl.SetLineProfileSource(TeachingViewer.SelectedViewer.GrabImage);
                        LineProfileCtrl.Refresh();
                        HistogramCtrl.ImageChanged(TeachingViewer.SelectedViewer.ReferenceImage, TeachingViewer.SelectedViewer.GrabImage);
                        TeachingViewer.SelectedViewer.txtReferenceLabel.Visibility = Visibility.Hidden;
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Exception occured in btnGrabEntireImage_Click(TeachingWindow.xaml.cs");
            }
        }
        // 카메라 해상도를 설정파일로부터 읽어들인다.
        public void SetCameraResolution()
        {
            int CamType = 0;
            if (IsOnLine)
            {
                for(int i = 0; i < MainWindow.m_nTotalTeachingViewCount; i++)
                {
                    CamType = (int)MainWindow.Convert_ViewIndexToVisionIndexs(i).CategorySurface;
                    TeachingViewer.CamView[i].CamResolutionX = MainWindow.Setting.SubSystem.IS.CamResolutionX[CamType];
                    TeachingViewer.CamView[i].CamResolutionY = MainWindow.Setting.SubSystem.IS.CamResolutionY[CamType];
                }
            }
        }

        // 현재 Surface에 지정된 섹션을 생성한다.
        private void btnCreateSection_Click(object sender, RoutedEventArgs e)
        {
            TeachingViewerCtrl viewer = TeachingViewer.SelectedViewer;
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
            {
                MessageBox.Show("모델을 선택해 주시기 바랍니다.", "Information");
                return;
            }
            if (TeachingViewer.SelectedViewer.ReferenceImage == null)
            {
                MessageBox.Show("기준 영상 등록 후 섹션을 생성해 주시기 바랍니다.", "Information");
                return;
            }

            if (CreateSection())
            {
                SaveReferenceCornerImage(viewer.ID);
                SetText("섹션 생성완료", 1500);
            }
               
            else
                SetText("섹션 생성실패", 1500);
        }

        private bool CreateSection()
        {
            bool bRefImageExists = false;
            int nDesVision = TeachingViewer.SelectedViewer.ID;
            int nSectionCount = TeachingViewer.SelectedViewer.BasedROICanvas.FiduGraphicsList.Count;

            string szGroupName = MainWindow.CurrentGroup.Name;
            string szModelName = MainWindow.CurrentModel.Name;

            MainWindow ptrMainWindow = Application.Current.MainWindow as MainWindow;
            DrawingCanvas roiCanvas = TeachingViewer.SelectedViewer.BasedROICanvas;
            SectionManager sectionManager = TeachingViewer.SelectedViewer.SectionManager;
            Surface surfaceType = TeachingViewer.SelectedViewer.SurfaceType;

            IndexInfo IndexInfo = ptrMainWindow.Convert_ViewIndexToVisionIndex(nDesVision);
            int tempDesvision = IndexInfo.VisionIndex;
            int FrameGrabberIndex = VID.CalcIndex(tempDesvision);

            if (nSectionCount <= 0)
            {
                sectionManager.Sections.Clear();
                // MT007 : 섹션 영역을 생성해 주시기 바랍니다.
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("MT007"), "Information");
                return false;
            }

            if (!(IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave))///////BP의 Slave를 제외
            {
                foreach (SectionInformation section in sectionManager.Sections)
                {
                    if ((IndexInfo.CategorySurface == CategorySurface.BP) ////BP 이면서 Master인 경우
                        || MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] < 6)
                    {
                        string szSectionImagePath = string.Empty;
                        if (!IsOnLine && MtsManager.SelectedMachine != null)
                        {
                            szSectionImagePath = DirectoryManager.GetOfflineSectionImagePath(MainWindow.Setting.General.ModelPath, MtsManager.SelectedMachine.Name, szGroupName, szModelName, section.Name, surfaceType);
                        }
                        else
                        {
                            szSectionImagePath = DirectoryManager.GetSectionImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName, section.Name, surfaceType, -1);
                        }

                        if (File.Exists(szSectionImagePath))
                        {
                            bRefImageExists = true;
                            break;
                        }
                    }
                    else///////////CA, BA 경우
                    {
                        string[] szSectionImagePath = new string[3];// string.Empty;
                        if (!IsOnLine && MtsManager.SelectedMachine != null)
                        {
                            szSectionImagePath = DirectoryManager.GetOfflineSectionRGBImagePath(MainWindow.Setting.General.ModelPath, MtsManager.SelectedMachine.Name, szGroupName, szModelName, section.Name, surfaceType);
                        }
                        else
                        {
                            szSectionImagePath = DirectoryManager.GetSectionRGBImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName, section.Name, surfaceType);
                        }

                        if (File.Exists(szSectionImagePath[0]))
                        {
                            bRefImageExists = true;
                            break;
                        }
                    }
                }

                if (bRefImageExists)
                {
                    if (MessageBoxResult.No == MessageBox.Show("기준 영상이 존재합니다.\n기존의 기준 영상을 삭제 후 새로운 섹션을 생성하시겠습니까?", "Information", MessageBoxButton.YesNo))
                    {
                        return false;
                    }
                    // else
                    foreach (SectionInformation section in sectionManager.Sections)
                    {
                        string[] szSectionImagePath = new string[3];
                        if (surfaceType == Surface.BP1 || MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] < 6)
                        {
                            if (!IsOnLine && MtsManager.SelectedMachine != null)
                            {
                                szSectionImagePath[0] = DirectoryManager.GetOfflineSectionImagePath(MainWindow.Setting.General.ModelPath, MtsManager.SelectedMachine.Name, szGroupName, szModelName, section.Name, surfaceType);
                            }
                            else
                            {
                                szSectionImagePath[0] = DirectoryManager.GetSectionImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName, section.Name, surfaceType, -1);
                            }

                            if (File.Exists(szSectionImagePath[0]))
                            {
                                section.Image[0].Source = null;
                                System.GC.Collect();
                                FileSupport.TryDeleteFile(szSectionImagePath[0]);
                            }
                        }
                        else
                        {
                            if (!IsOnLine && MtsManager.SelectedMachine != null)
                            {
                                szSectionImagePath = DirectoryManager.GetOfflineSectionRGBImagePath(MainWindow.Setting.General.ModelPath, MtsManager.SelectedMachine.Name, szGroupName, szModelName, section.Name, surfaceType);
                            }
                            else
                            {
                                szSectionImagePath = DirectoryManager.GetSectionRGBImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName, section.Name, surfaceType);
                            }
                            for (int i = 0; i < 3; i++)
                            {
                                if (File.Exists(szSectionImagePath[i]))
                                {
                                    section.Image[i].Source = null;
                                    System.GC.Collect();
                                    FileSupport.TryDeleteFile(szSectionImagePath[i]);
                                }
                            }
                        }
                    }
                }
            }


            if (ptrMainWindow != null && ptrMainWindow.PCSInstance.Vision[tempDesvision].Connected)
            {
                lvResult.Items.Clear();
                ptrMainWindow.PCSInstance.Vision[tempDesvision].DefectCount = 0;

                List<SectionInformation> newSections = new List<SectionInformation>();
                int nUnitRegionCount = 1;
                int nPsrRegionCount = 1;
                int nOuterRegionCount = 1;
                int nRawRegionCount = 1;
                int nIDRegionCount = 1;

                #region Check Region Validation.
                bool bNeedNotify = false;
                roiCanvas.UnselectAll();
                foreach (GraphicsRectangle g in roiCanvas.GraphicsRectangleList)
                {
                    if (!g.IsValidRegion && g.RegionType != GraphicsRegionType.StripAlign && g.RegionType != GraphicsRegionType.IDMark)
                    {
                        g.IsSelected = true;
                        bNeedNotify = true;
                    }
                }
                if (bNeedNotify)
                {
                    TeachingViewer.SelectedViewer.SetZoomToFit();
                    MessageBox.Show("섹션 설정이 되어있지 않은 영역에 대한 섹션 설정이 필요합니다.\n선택된 영역을 확인바랍니다.", "Information");
                    return false;
                }
                #endregion

                #region 1. Create sections 원소재.

                int sectionCode = 0;
                for (int nIndex = 0; nIndex < nSectionCount; nIndex++)
                {
                    GraphicsRectangle graphic = roiCanvas.FiduGraphicsList[nIndex] as GraphicsRectangle;
                    if (graphic == null ||
                        graphic.IsFiducialRegion == false ||
                        !roiCanvas.GraphicsList.Contains(graphic))
                    {
                        continue; // 잘못된 Section ROI.
                    }

                    if (graphic.RegionType != GraphicsRegionType.Rawmetrial) continue;

                    SectionInformation section = SectionManager.GraphicsToSection(graphic, roiCanvas, MainWindow.Setting.General.MachineCode, szModelName, sectionCode++);
                    if (section != null)
                    {
                        section.Type = SectionManager.GetSectionType(SectionTypeCode.RAW_REGION);
                        section.Name = "원소재 Section " + nRawRegionCount++;
                        section.RelatedROI = graphic;
                        newSections.Add(section);

                    }
                }
                #endregion 1. Create sections 원소재.

                #region 2. Create sections Unit.
                for (int nIndex = 0; nIndex < nSectionCount; nIndex++)
                {
                    GraphicsRectangle graphic = roiCanvas.FiduGraphicsList[nIndex] as GraphicsRectangle;
                    if (graphic == null ||
                        graphic.IsFiducialRegion == false ||
                        !roiCanvas.GraphicsList.Contains(graphic))
                    {
                        continue; // 잘못된 Section ROI.
                    }

                    if (graphic.RegionType != GraphicsRegionType.UnitRegion ) continue;

                    SectionInformation section = SectionManager.GraphicsToSection(graphic, roiCanvas, MainWindow.Setting.General.MachineCode, szModelName, sectionCode++);
                    if (section != null)
                    {
                        section.Type = SectionManager.GetSectionType(SectionTypeCode.UNIT_REGION);
                        section.Name = "Unit Section " + nUnitRegionCount++;
                        section.RelatedROI = graphic;
                        newSections.Add(section);
                    }
                }
                #endregion 2. Create sections Unit.

                #region 3. Create sections PSR.
                for (int nIndex = 0; nIndex < nSectionCount; nIndex++)
                {
                    GraphicsRectangle graphic = roiCanvas.FiduGraphicsList[nIndex] as GraphicsRectangle;
                    if (graphic == null ||
                        graphic.IsFiducialRegion == false ||
                        !roiCanvas.GraphicsList.Contains(graphic))
                    {
                        continue; // 잘못된 Section ROI.
                    }

                    if (graphic.RegionType != GraphicsRegionType.PSROdd) continue;

                    SectionInformation section = SectionManager.GraphicsToSection(graphic, roiCanvas, MainWindow.Setting.General.MachineCode, szModelName, sectionCode++);
                    if (section != null)
                    {
                        section.Type = SectionManager.GetSectionType(SectionTypeCode.PSR_REGION);
                        section.Name = "PSR Section " + nPsrRegionCount++;
                        section.RelatedROI = graphic;
                        newSections.Add(section);
                    }
                }
                #endregion 3. Create sections PSR.

                #region 1. Create sections Outer if MotherID is -1.
                for (int nIndex = 0; nIndex < nSectionCount; nIndex++)
                {
                    GraphicsRectangle graphic = roiCanvas.FiduGraphicsList[nIndex] as GraphicsRectangle;
                    if (graphic == null ||
                        graphic.IsFiducialRegion == false ||
                        !roiCanvas.GraphicsList.Contains(graphic))
                    {
                        continue; // 잘못된 Section ROI.
                    }

                    if (graphic.RegionType != GraphicsRegionType.OuterRegion) continue;

                    if (graphic.MotherROIID == -1)
                    {
                        SectionInformation section = SectionManager.GraphicsToSection(graphic, roiCanvas, MainWindow.Setting.General.MachineCode, szModelName, sectionCode++);
                        if (section != null)
                        {
                            section.Type = SectionManager.GetSectionType(SectionTypeCode.OUTER_REGION);
                            section.Name = "Outer Section " + nOuterRegionCount++;
                            section.RelatedROI = graphic;
                            newSections.Add(section);
                        }
                    }
                }
                #endregion 1. Create sections Outer.

                #region 1. Create sections Outer if MotherID is -1.
                for (int nIndex = 0; nIndex < nSectionCount; nIndex++)
                {
                    GraphicsRectangle graphic = roiCanvas.FiduGraphicsList[nIndex] as GraphicsRectangle;
                    if (graphic == null ||
                        graphic.IsFiducialRegion == false ||
                        !roiCanvas.GraphicsList.Contains(graphic))
                    {
                        continue; // 잘못된 Section ROI.
                    }

                    if (graphic.RegionType != GraphicsRegionType.IDRegion) continue;

                    if (graphic.MotherROIID == -1)
                    {
                        SectionInformation section = SectionManager.GraphicsToSection(graphic, roiCanvas, MainWindow.Setting.General.MachineCode, szModelName, sectionCode++);
                        if (section != null)
                        {
                            section.Type = SectionManager.GetSectionType(SectionTypeCode.ID_REGION);
                            section.Name = "ID Section " + nIDRegionCount++;
                            section.RelatedROI = graphic;
                            newSections.Add(section);
                        }
                    }
                }
                #endregion 1. Create sections Outer.

                #region 3. Send section informations to vision.

                ptrMainWindow.PCSInstance.Vision[tempDesvision].SendGrabSide(IndexInfo.SurfaceScanCount);

                if (!(IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave))///////BP의 Slave 제외
                {
                    // 2. Clear InspInfo
                    ptrMainWindow.PCSInstance.Vision[tempDesvision].ClearModelInfo();

                    int nSecCode = 0;
                    foreach (SectionInformation section in newSections)
                    {
                        section.Code = QueryHelper.GetCode(nSecCode++, 4);
                        if (ptrMainWindow.PCSInstance.Vision[tempDesvision].Connected && (section.BitmapSource[0] != null || section.BitmapSource[1] != null || section.BitmapSource[2] != null))
                        {
                            //if (Math.Abs((section.BitmapSource.PixelWidth / 4) - section.Region.Width) == 1 ||
                            //    Math.Abs((section.BitmapSource.PixelHeight / 4) - section.Region.Height) == 1)
                            if (Math.Abs((section.BitmapSource[0].PixelWidth * VisionDefinition.GRAB_IMAGE_SCALE) - section.Region.Width) != 0 ||
                                Math.Abs((section.BitmapSource[0].PixelHeight * VisionDefinition.GRAB_IMAGE_SCALE) - section.Region.Height) != 0)
                            {
                                section.Region = new Int32Rect(section.Region.X, section.Region.Y, (int)((double)section.BitmapSource[0].PixelWidth * VisionDefinition.GRAB_IMAGE_SCALE), (int)((double)section.BitmapSource[0].PixelHeight * VisionDefinition.GRAB_IMAGE_SCALE));
                            }
                        }
                        else
                        {
                            if (section.Type.Code == SectionTypeCode.OUTER_REGION)
                            {
                                int w = (section.Region.Width / 4);
                                int h = (section.Region.Height / 4);
                                section.Region = new Int32Rect(section.Region.X, section.Region.Y, w * 4, h * 4);
                            }
                        }

                        ptrMainWindow.PCSInstance.Vision[tempDesvision].AddSection(section);
                        Thread.Sleep(5);
                    }
                    #endregion 3. Send section informations to vision.

                #region 4. Request section Images.
                    foreach (SectionInformation section in newSections)
                    {
                        ///////위 조건문에서 BP Slave를 제외하였지만 다시 BP Master만 제한
                        if ((IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave == false) || (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] < 6))
                        {
                            ptrMainWindow.PCSInstance.Vision[tempDesvision].ReqSectionRGB(section.IntCode, section.IterationXPosition, section.IterationYPosition, 0, 0, IndexInfo.SurfaceScanCount);

                            Thread.Sleep(900);

                            section.LastUnitX = section.IterationXPosition;
                            section.LastUnitY = section.IterationYPosition;

                            int sleepCount = 0;
                            while (!ptrMainWindow.PCSInstance.Vision[tempDesvision].Grab_Done)
                            {
                                if (!m_ptrMainWindow.PCSInstance.Vision[tempDesvision].Connected || sleepCount >= 100) // Timeout : 3 sec.
                                {
                                    // MT003 : Section 이미지를 획득하는데 실패하였습니다. Vision과의 연결 상태를 확인 후 다시 시도해 주시기 바랍니다.
                                    MessageBox.Show(ResourceStringHelper.GetErrorMessage("MT003"), "Error");
                                    return false;
                                }
                                System.Threading.Thread.Sleep(100);
                                sleepCount++;
                            }

                            section.SetTempBitmapSource(ptrMainWindow.PCSInstance.Vision[tempDesvision].SectionImage, 0);
                        }                        
                        else ///////BP를 제외한 CA, BA
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                if (section.Type.Code == SectionTypeCode.PSR_REGION )
                                {
                                    switch(i)
                                    {
                                        case 0: // Saturation image
                                            ptrMainWindow.PCSInstance.Vision[tempDesvision].ReqSectionRGB(section.IntCode, section.IterationXPosition, section.IterationYPosition, 0, PSR_Saturation_Index, IndexInfo.SurfaceScanCount);
                                            break;

                                        case 1: // Color_image
                                            ptrMainWindow.PCSInstance.Vision[tempDesvision].ReqSectionRGB(section.IntCode, section.IterationXPosition, section.IterationYPosition, 0, PSR_Color_Index, IndexInfo.SurfaceScanCount);
                                            break;

                                        case 2: // Blending_image
                                            ptrMainWindow.PCSInstance.Vision[tempDesvision].ReqSectionRGB(section.IntCode, section.IterationXPosition, section.IterationYPosition, 0, PSR_Blending_Index, IndexInfo.SurfaceScanCount);
                                            break;
                                    }
                                }

                                 
                                if (section.Type.Code != SectionTypeCode.PSR_REGION)    
                                    ptrMainWindow.PCSInstance.Vision[tempDesvision].ReqSectionRGB(section.IntCode, section.IterationXPosition, section.IterationYPosition, 0, i, IndexInfo.SurfaceScanCount);

                                Thread.Sleep(900);

                                section.LastUnitX = section.IterationXPosition;
                                section.LastUnitY = section.IterationYPosition;

                                int sleepCount = 0;
                                while (!ptrMainWindow.PCSInstance.Vision[tempDesvision].Grab_Done)
                                {
                                    if (!m_ptrMainWindow.PCSInstance.Vision[tempDesvision].Connected || sleepCount >= 100) // Timeout : 3 sec.
                                    {
                                        // MT003 : Section 이미지를 획득하는데 실패하였습니다. Vision과의 연결 상태를 확인 후 다시 시도해 주시기 바랍니다.
                                        MessageBox.Show(ResourceStringHelper.GetErrorMessage("MT003"), "Error");
                                        return false;
                                    }
                                    System.Threading.Thread.Sleep(100);
                                    sleepCount++;
                                }

                                //Color Section
                                if(section.Type.Code == SectionTypeCode.PSR_REGION)
                                {
                                    switch (i)
                                    {
                                        case 0: // Saturation image
                                            section.SetTempBitmapSource(ptrMainWindow.PCSInstance.Vision[tempDesvision].SectionImage, i);
                                            break;

                                        case 1: // Color_image
                                            section.SetTempBitmapSource(ptrMainWindow.PCSInstance.Vision[tempDesvision].SectionImage_Color, i);
                                            break;

                                        case 2: // Blending_image
                                            section.SetTempBitmapSource(ptrMainWindow.PCSInstance.Vision[tempDesvision].SectionImage, i);
                                            break;
                                    }
                                }
                                else
                                {
                                    section.SetTempBitmapSource(ptrMainWindow.PCSInstance.Vision[tempDesvision].SectionImage, i);
                                }
                            }
                        }
                    }
                }
                #endregion 4. Request section Images.

                if (newSections.Count > 0)
                {
                    foreach (SectionInformation newSection in newSections)
                    {
                        foreach (SectionInformation oldSection in sectionManager.Sections)
                        {
                            //if (newSection.Type.Name.Contains("Outer Section"))
                            //    continue;
                            if (newSection.Name == oldSection.Name) // 기존에 존재했었던 Section이라면, ROI를 복사하여 준다.
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    foreach (GraphicsBase b in oldSection.ROICanvas[i].GraphicsList)
                                    {
                                        if (b.RegionType == GraphicsRegionType.LocalAlign)// || b.RegionType == GraphicsRegionType.GuideLine || b.RegionType == GraphicsRegionType.TapeLoaction)
                                            continue;

                                        GraphicsBase graphic = b.CreateSerializedObject().CreateGraphics();
                                        graphic.OriginObjectColor = graphic.ObjectColor = b.OriginObjectColor;
                                        graphic.ID = graphic.GetHashCode();
                                        graphic.ActualScale = newSection.ROICanvas[i].ActualScale;

                                        double deltaScale = 1.0;
                                        deltaScale = Math.Min(deltaScale, (double)newSection.Region.Width / (double)oldSection.Region.Width);
                                        deltaScale = Math.Min(deltaScale, (double)newSection.Region.Height / (double)oldSection.Region.Height);
                                        if (newSection.ROICanvas[i].CanDraw(graphic, deltaScale))
                                        {
                                            newSection.ROICanvas[i].GraphicsList.Add(graphic);
                                            if (b.LocalAligns != null) // b에 존재하던 Local Align을 새로 생성되는 객체 graphic에 입혀준다.
                                            {
                                                graphic.LocalAligns = new GraphicsRectangle[b.LocalAligns.Length];
                                                for (int nIndex = 0; nIndex < b.LocalAligns.Length; nIndex++)
                                                {
                                                    if (b.LocalAligns[nIndex] != null)
                                                    {
                                                        // Create Local Align.
                                                        GraphicsRectangle localAlign = newSection.ROICanvas[i].CreateLocalAlign(graphic, nIndex, b.LocalAligns[nIndex].boundaryRect);
                                                        localAlign.MotherROI = graphic;
                                                        graphic.LocalAligns[nIndex] = localAlign;
                                                        newSection.ROICanvas[i].GraphicsList.Add(localAlign);
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
                sectionManager.Sections.Clear();
                foreach (SectionInformation section in newSections)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (section.ROICanvas[i] != null)
                        {
                            section.ROICanvas[i].Refresh();
                        }

                    }
                    sectionManager.Sections.Add(section);
                }


                TeachingViewer.SelectedViewer.lbSection.ItemsSource = sectionManager.Sections;
                //TeachingViewer.SelectedViewer.lbSection.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                TeachingViewer.SelectedViewer.lbSection.SelectedIndex = -1;

                // IS에서 전달받은 불량에 대한 Unit X, Y의 실제 값을 연산하기 위해 IS_Interface에 Section 리스트에 대한 포인터를 전달한다.
                if (!(IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave))///////BP의 Slave를 제외
                    ptrMainWindow.PCSInstance.Vision[tempDesvision].Sections = sectionManager.Sections;

                // Refresh Context Menu.
                for(int i = 0; i < m_ptrMainWindow.nTotalTeachingViewCount; i++)
                {
                    TeachingViewer.CamView[i].CreateContextMenu();
                }
            }
            else
            {
                // MT005 : Vision과의 연결이 끊어졌습니다. 문제가 지속되면 Vision을 다시 시작해 주시기 바랍니다.                
                MessageBox.Show(ResourceStringHelper.GetErrorMessage("MT005"), "Error");
                return false;
            }

            return true;
        }

        // 개별 유닛 보기 요청.
        private void btnViewUnit_Click(object sender, RoutedEventArgs e)
        {
            // 수행 요건 : 선택된 Section이 있어야 한다.
            if (TeachingViewer.SelectedViewer.SelectedSection == null)
                return;

            int nSecID = TeachingViewer.SelectedViewer.lbSection.SelectedIndex;
            string[] arrTokens = cmbRegionList.SelectedItem.ToString().Split(new char[] { ' ', ':' });

            int nUnitX = Convert.ToInt32(arrTokens[1]) - 1;
            int nUnitY = Convert.ToInt32(arrTokens[4]) - 1;

            if ((nSecID < 0) || (nUnitX < 0) || (nUnitY < 0))
            {
                return;
            }

            int nDesVision = TeachingViewer.SelectedViewer.ID;
            IndexInfo IndexInfo = m_ptrMainWindow.Convert_ViewIndexToVisionIndex(nDesVision);

            if (TeachingViewer.SelectedViewer.SelectedSection.Type.Code == SectionTypeCode.UNIT_REGION ||
                TeachingViewer.SelectedViewer.SelectedSection.Type.Code == SectionTypeCode.PSR_REGION)
            {

                if (IndexInfo.CategorySurface == CategorySurface.BP) // BP는 절반만 봐야함
                {

                    if((int)IndexInfo.Surface%2 == 1) //BP1
                    {
                        nUnitX = MainWindow.CurrentModel.Strip.UnitRow - nUnitX - 1;
                    }
                    else // BP2
                    {
                        nUnitX = (int)Math.Ceiling(MainWindow.CurrentModel.Strip.UnitRow / 2.0) - nUnitX - 1;
                    }

                }
                else if (IndexInfo.CategorySurface == CategorySurface.CA) // 뒤집어서 봐야함
                    nUnitX = MainWindow.CurrentModel.Strip.UnitRow - nUnitX - 1;
            }

            int tempDesVision = IndexInfo.VisionIndex;
            int FrameGrabberIndex = VID.CalcIndex(tempDesVision);
            int nChannel = (int)TeachingViewer.SelectedViewer.SelectedChannel;
            if (tempDesVision == VID.BP1 || MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] < 6)
            {
                nChannel = 0;
            }

            if (TeachingViewer.SelectedViewer.SelectedSection.Type.Code == SectionTypeCode.PSR_REGION)
            {
                for (int i = 0; i < 3; i++)
                {
                    switch (i)
                    {
                        case 0:
                            m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(nSecID, nUnitX, nUnitY, 1, Result_Saturation_Index, IndexInfo.SurfaceScanCount);
                            Wait_Grab_Done(tempDesVision);
                            TeachingViewer.SelectedViewer.SelectedSection.SetTempBitmapSource(m_ptrMainWindow.PCSInstance.Vision[tempDesVision].SectionImage, i);
                            break;

                        case 1:
                            m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(nSecID, nUnitX, nUnitY, 1, Result_Color_Index, IndexInfo.SurfaceScanCount);
                            Wait_Grab_Done(tempDesVision);
                            TeachingViewer.SelectedViewer.SelectedSection.SetTempBitmapSource(m_ptrMainWindow.PCSInstance.Vision[tempDesVision].SectionImage_Color, i);
                            break;

                        case 2:
                            m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(nSecID, nUnitX, nUnitY, 1, Result_Blending_Index, IndexInfo.SurfaceScanCount);
                            Wait_Grab_Done(tempDesVision);
                            TeachingViewer.SelectedViewer.SelectedSection.SetTempBitmapSource(m_ptrMainWindow.PCSInstance.Vision[tempDesVision].SectionImage, i);
                            break;
                    }
                }

                TeachingViewer.SelectedViewer.SelectedSection.SetTempBitmapSource_channel(nChannel);
            }
            else
            {
                if (TeachingViewer.SelectedViewer.RGB)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(nSecID, nUnitX, nUnitY, 1, i, IndexInfo.SurfaceScanCount);
                        Wait_Grab_Done(tempDesVision);
                        TeachingViewer.SelectedViewer.SelectedSection.SetTempBitmapSource(m_ptrMainWindow.PCSInstance.Vision[tempDesVision].SectionImage, i);
                    }

                    TeachingViewer.SelectedViewer.SelectedSection.SetTempBitmapSource_channel(nChannel);
                }
                else
                {
                    m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(nSecID, nUnitX, nUnitY, 1, nChannel, IndexInfo.SurfaceScanCount);
                    Wait_Grab_Done(tempDesVision);
                    TeachingViewer.SelectedViewer.SelectedSection.SetTempBitmapSource(m_ptrMainWindow.PCSInstance.Vision[tempDesVision].SectionImage, nChannel);
                }
            }



            // 수동 검사할 때 LastUnit x, y 로 수동검사함
            TeachingViewer.SelectedViewer.SelectedSection.LastUnitX = nUnitX;
            TeachingViewer.SelectedViewer.SelectedSection.LastUnitY = nUnitY;



            TeachingViewer.SelectedViewer.ThumbnailViewer.SetSourceImage(TeachingViewer.SelectedViewer);
            TeachingViewer.SelectedViewer.txtReferenceLabel.Visibility = Visibility.Hidden;
        }

        public void Wait_Grab_Done(int tempDesVision)
        {

            if (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Connected)
            {
                // MT003 : Section 이미지를 획득하는데 실패하였습니다. Vision과의 연결 상태를 확인 후 다시 시도해 주시기 바랍니다.
                MessageBox.Show(ResourceStringHelper.GetErrorMessage("MT003"), "Error");
                return;
            }

            int sleepCount = 0;
            while (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Grab_Done)
            {
                if (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Connected || sleepCount >= 300) // Timeout : 3 sec.
                {
                    // MT003 : Section 이미지를 획득하는데 실패하였습니다. Vision과의 연결 상태를 확인 후 다시 시도해 주시기 바랍니다.
                    MessageBox.Show(ResourceStringHelper.GetErrorMessage("MT003"), "Error");
                    return;
                }
                Thread.Sleep(10);
                sleepCount++;
            }
        }

        public void SaveTeachingData()
        {
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
            {
                MessageBox.Show("모델을 선택해 주시기 바랍니다.", "Information");
                return;
            }
            if (!CheckAlign_MerterialSection()) return;
            this.Cursor = Cursors.Wait;
            TeachingViewer.SelectedViewer.Cursor = Cursors.Wait;
            m_ptrMainWindow.ProgressWindow.Owner = this;
            m_ptrMainWindow.ProgressWindow.StartProgress(0);

            string szMachineCode = MainWindow.Setting.General.MachineCode;
            string szGroupName = MainWindow.CurrentGroup.Name;
            string szModelName = MainWindow.CurrentModel.Name;        
            if (!TeachingDataManager.DeleteTeachingData(szMachineCode, szModelName))
            {
                this.Cursor = Cursors.Arrow;
                TeachingViewer.SelectedViewer.Cursor = Cursors.Arrow;
                m_ptrMainWindow.ProgressWindow.StopProgress(0);
                MessageBox.Show("기존 Teaching 데이터를 초기화 시키는데 실패하였습니다.\n잠시 후 다시 수행해 주시기 바랍니다.", "Information");
            }
            m_ptrMainWindow.ProgressWindow.SetProgressValue(0, 15, "기존 데이터 삭제");
            CreateCenterLineDataFile();
            CreateBallDataFile();
            m_ptrMainWindow.ProgressWindow.SetProgressValue(0, 20, "중앙선 생성");
            // 상부, 하부, 투과에 대한 티칭 데이터를 DB에 저장한다.

            bool[] saveResult = Enumerable.Repeat(true, MainWindow.m_nTotalTeachingViewCount).ToArray();
            ROIManager.RoiCode = 0;

            bool[] so_check = new bool[m_nTotalScanCount];
            int Count = 0;
            for (int nIndex = 0; nIndex < TeachingViewers.Length; nIndex++)
            {
                IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(nIndex); // hs - BP, CA, BA 인덱스 배분
                int tempnIndex = IndexInfo.VisionIndex;
                int FrameGrabberIndex = VID.CalcIndex(tempnIndex);

                if (IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave) // hs - BP 영역
                {
                    saveResult[nIndex] = TeachingViewers[nIndex].SaveSection(szMachineCode, szModelName);
                }
                else
                {
                    saveResult[nIndex] = ROIManager.SaveStripAlignROI(szMachineCode, szGroupName, szModelName, tempnIndex.ToString(), TeachingViewers[nIndex].BasedROICanvas, TeachingViewers[nIndex].SurfaceType);
                    TeachingViewers[nIndex].StripAlign = new Point(50, 50);
                    int soCount = 0;
                    foreach (GraphicsBase g in TeachingViewers[nIndex].BasedROICanvas.GraphicsList)
                    {
                        if (g.RegionType == GraphicsRegionType.StripOrigin)
                        {
                            GraphicsStripOrigin gs = g as GraphicsStripOrigin;
                            TeachingViewers[nIndex].StripAlign = new Point(gs.cpX, gs.cpY);
                            soCount++;
                        }
                    }
                    if (TeachingViewers[nIndex].BasedROICanvas.GraphicsList.Count == 0) soCount++;

                    saveResult[nIndex] = TeachingViewers[nIndex].SaveSection(szMachineCode, szModelName);

                    if (soCount == 0)
                        so_check[Count] = false;
                    else
                    {
                        so_check[Count] = true;
                        soCount = 0;
                        foreach (SectionInformation sec in TeachingViewers[nIndex].SectionManager.Sections)
                        {
                            if (sec.Type.Code == SectionTypeCode.UNIT_REGION || sec.Type.Code == SectionTypeCode.PSR_REGION)
                            {
                                foreach (SectionRegion sr in sec.SectionRegionList)
                                {
                                    if (sr.RegionIndex.X == 0 && sr.RegionIndex.Y == 0)
                                    {
                                        TeachingViewers[nIndex].Section = new Point(sr.RegionPosition.X, sr.RegionPosition.Y);
                                        soCount++;

                                        break;
                                    }
                                }
                            }
                        }
                        System.Windows.Point tempSectionPos = new System.Windows.Point();
                        if (soCount > 0)
                        {
                            tempSectionPos = new Point(TeachingViewers[nIndex].Section.X - TeachingViewers[nIndex].StripAlign.X, TeachingViewers[nIndex].Section.Y - TeachingViewers[nIndex].StripAlign.Y);
                        }
                        else
                        {
                            tempSectionPos = new Point(TeachingViewers[nIndex].StripAlign.X, TeachingViewers[nIndex].StripAlign.Y);
                        }
                        tempSectionPos.X /= VisionDefinition.GRAB_IMAGE_SCALE;
                        tempSectionPos.Y /= VisionDefinition.GRAB_IMAGE_SCALE;
                        tempSectionPos.X *= (MainWindow.Setting.SubSystem.IS.CamResolutionX[(int)IndexInfo.CategorySurface] / 1000.0);
                        tempSectionPos.Y *= (MainWindow.Setting.SubSystem.IS.CamResolutionY[(int)IndexInfo.CategorySurface] / 1000.0);
                        MainWindow.SectionPosition[Count].DefectSectionPosition = tempSectionPos;

                        Point MarkSectionPos = new Point();
                        System.Windows.Point tempIDSectionPos = new System.Windows.Point();              
                        foreach (SectionInformation sec in TeachingViewers[nIndex].SectionManager.Sections)
                        {
                            if (sec.Type.Code == SectionTypeCode.ID_REGION)
                            {
                                foreach (SectionRegion sr in sec.SectionRegionList)
                                {
                                    if (sr.RegionIndex.X == 0 && sr.RegionIndex.Y == 0)
                                    {
                                        MarkSectionPos = new Point(sr.RegionPosition.X - TeachingViewers[nIndex].StripAlign.X, sr.RegionPosition.Y - TeachingViewers[nIndex].StripAlign.Y);
                                        break;
                                    }
                                }
                                for (int i = 0; i < 3; i++)
                                {
                                    foreach (GraphicsBase Roi in sec.ROICanvas[i].GraphicsList)
                                    {
                                        if (Roi.RegionType == GraphicsRegionType.IDMark)
                                        {
                                            GraphicsRectangle Rect = Roi as GraphicsRectangle;
                                            tempIDSectionPos = new Point((Rect.Left + Rect.Right) / 2 , (Rect.Top + Rect.Bottom) / 2);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        MarkSectionPos.X /= VisionDefinition.GRAB_IMAGE_SCALE;////원본 이미지의 Scale
                        MarkSectionPos.Y /= VisionDefinition.GRAB_IMAGE_SCALE;
                        tempIDSectionPos.X += MarkSectionPos.X;
                        tempIDSectionPos.Y += MarkSectionPos.Y;
                        tempIDSectionPos.X *= (MainWindow.Setting.SubSystem.IS.CamResolutionX[(int)IndexInfo.CategorySurface] / 1000.0);
                        tempIDSectionPos.Y *= (MainWindow.Setting.SubSystem.IS.CamResolutionY[(int)IndexInfo.CategorySurface] / 1000.0);
                        MainWindow.SectionPosition[Count].IDSectionPosition = tempIDSectionPos;
                        Count++;
                    }
                }
                m_ptrMainWindow.ProgressWindow.SetProgressValue(nIndex, 20 + (nIndex * 15) + 4, "섹션 저장 " + tempnIndex.ToString());

                if (saveResult[nIndex]) // Step 1: Section 저장에 성공한 상태.
                {
                    foreach (SectionInformation section in TeachingViewers[nIndex].SectionManager.Sections)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if ((MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] < 6) && i == 1)
                                break;
                            saveResult[nIndex] = ROIManager.SaveROI(szMachineCode, szGroupName, szModelName, section.Code, section.ROICanvas[i], TeachingViewers[nIndex].SurfaceType, i, MainWindow.Setting.General.ModelPath);
                            if (!saveResult[nIndex])
                                break;
                        }
                    }
                    m_ptrMainWindow.ProgressWindow.SetProgressValue(nIndex, 20 + (nIndex * 15) + 6, "ROI 저장 " + nIndex.ToString());

                    if (saveResult[nIndex]) // Step 2: ROI, 검사 설정 정보 저장이 성공한 상태.
                    {
                        // Step 2.1: 전체 영상 저장. (파일 경로에 존재하지 않는 것에 대해서만.)
                        SaveReferenceEntireImage(nIndex, true);

                        // Step 2.2: 섹션 영상 저장. (파일 경로에 존재하지 않는 것에 대해서만.)
                        if (TeachingViewers[nIndex].SectionManager.Sections.Count > 0)
                        {
                            if (nIndex != 1)
                                SaveReferenceSectionImages(nIndex, true);
                        }
                    }
                    m_ptrMainWindow.ProgressWindow.SetProgressValue(nIndex, 20 + (nIndex * 15) + 15, "기준 영상 저장 " + nIndex.ToString());
                }
            }
            string str = "";
            if (MainWindow.CurrentModel.UseVerify)
            {
                bool result = true;
                for (int nIndex = 0; nIndex < m_nTotalScanCount; nIndex++)
                {
                    result = result && so_check[nIndex];
                    string Type = "";
                    string so = so_check[nIndex] ? "성공" : "실패";
                    m_ptrMainWindow.ProgressWindow.StopProgress(0);
                    Type = MainWindow.Convert_CheckVisionSlave(nIndex).CategorySurface.ToString();
                    str += "Cam" + Type + (nIndex + 1) + " Strip 원점 저장" + so + " ";
                }
                if(!result) MessageBox.Show(str, "Information");
            }

            DeleteEmptyCenterLineDataFile();
            DeleteEmptyBallDataFile();
            m_ptrMainWindow.ProgressWindow.SetProgressValue(0, 85, "중앙선 정리 ");

            this.Cursor = Cursors.Arrow;
            TeachingViewer.SelectedViewer.Cursor = Cursors.Arrow;
            bool bsave = true;
            for (int nIndex = 0; nIndex < TeachingViewers.Length; nIndex++)
            {
                string Type = "";
                string strSave = saveResult[nIndex] ? "성공" : "실패";
                bsave = bsave && saveResult[nIndex];
                m_ptrMainWindow.ProgressWindow.StopProgress(0);
                Type = MainWindow.Convert_ViewIndexToVisionIndexs(nIndex).Surface.ToString();
                str += "Cam" + Type + " Date 저장" + strSave + " ";
            }
            if(!bsave) MessageBox.Show(str, "Information");
            m_ptrMainWindow.ProgressWindow.SetProgressValue(0, 100, "완료 ");
            this.Cursor = Cursors.Arrow;
            TeachingViewer.SelectedViewer.Cursor = Cursors.Arrow;
            TeachingViewer.SelectedViewer.UpdateReferenceLabel();
            m_ptrMainWindow.ProgressWindow.StopProgress(0);
            SetText("저장 완료", 1500);
        }

        #region 영상 취득 및 저장
        // 현재 Surface에 대한 전체영상 취득.
        private void GrabEntireImage(bool full)
        {
            if (full)
            {
                if (MessageBox.Show("테스트 목적이 아닌 경우 사용하지 마세요.\n  시간이 오래 걸릴 수도 있습니다. 계속 진행 하시겠습니까?", "경고 : 전체 영상 취득", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) return;
            }
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
            {
                MessageBox.Show("모델을 선택해 주시기 바랍니다.", "Information");
                return;
            }
            int nDesVision = TeachingViewer.SelectedViewer.ID;
            IndexInfo indexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(nDesVision);
            int tempDesVision = indexInfo.VisionIndex;
            int FrameGrabberIndex = VID.CalcIndex(tempDesVision);////Corner Image Size를 100단위로 정렬
            System.Windows.Size CornerImageSize = new System.Windows.Size(
                 Math.Ceiling((MainWindow.Setting.General.CornerImageSizeX_mm * 1000 / MainWindow.Setting.SubSystem.IS.CamResolutionX[(int)indexInfo.CategorySurface]) / 100) * 100,
                 Math.Ceiling((MainWindow.Setting.General.CornerImageSizeY_mm * 1000 / MainWindow.Setting.SubSystem.IS.CamResolutionY[(int)indexInfo.CategorySurface]) / 100) * 100);
            if(!(indexInfo.CategorySurface == CategorySurface.BP && indexInfo.Slave))
                m_ptrMainWindow.PCSInstance.Vision[tempDesVision].SetBaseCornerImageInfo(CornerImageSize);
            if (m_ptrMainWindow.PCSInstance.Vision[tempDesVision] == null || !m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Connected)
            {
                // MT005 : Vision과의 연결이 끊어졌습니다. 문제가 지속되면 Vision을 다시 시작해 주시기 바랍니다.
                MessageBox.Show(ResourceStringHelper.GetErrorMessage("MT005"), "Error");
                return;
            }

            try
            {
                #region 장비에서 영상 취득시.
                if (MainWindow.Setting.SubSystem.PLC.UsePLC)
                {
                    //  m_ptrMainWindow.PCSInstance.SetPageDelay(MainWindow.Setting.General.PageDealy);
                    bool rboat = true;
                    int BoatIndex = 0;
                    try
                    {
                        if (indexInfo.CategorySurface == CategorySurface.BP) BoatIndex = (int)(indexInfo.Surface - Surface.BP1) % VID.BP_ScanComplete_Count;
                        else if (indexInfo.CategorySurface == CategorySurface.CA) BoatIndex = 2;
                        else if (indexInfo.CategorySurface == CategorySurface.BA) BoatIndex = 4;
                        rboat = m_ptrMainWindow.PCSInstance.PlcDevice.RequestBoat(BoatIndex, MainWindow.Setting.SubSystem.PLC.MCType);//추후에 ID별 보트 반환 감시
                    }
                    catch
                    {
                    }
                    if (!rboat)
                    {
                        MessageBox.Show("검사요구 및 위치를 확인 하세요");
                        return;
                    }
                    if (rboat)
                    {
                        lightcontrol.LightOn(true);
                        int sleepCount = 0;

                        if (indexInfo.CategorySurface == CategorySurface.BP)
                        {
                            m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Grab_Ready = false;
                            m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Grab(VisionDefinition.LINESCAN_FORWARD, full, indexInfo.SurfaceScanCount);
                        }

                        if(indexInfo.CategorySurface == CategorySurface.CA || indexInfo.CategorySurface == CategorySurface.BA)
                        {
                            if(indexInfo.Slave == false)//BA, CA - master
                            {
                                //////////////////////// 기존코드에는 SlavePC에 Nograb을 안줌. 추가한부분
                                m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Grab_Ready = false;
                                //m_ptrMainWindow.PCSInstance.Vision[tempDesVision + 1].Grab_Ready = false;                        
                                //m_ptrMainWindow.PCSInstance.Vision[tempDesVision + 1].Grab(VisionDefinition.LINESCAN_NOGRAB, false, indexInfo.SurfaceScanCount);
                                //sleepCount = 0;
                                //while (!(m_ptrMainWindow.PCSInstance.Vision[tempDesVision + 1].Grab_Ready))
                                //{
                                //    if (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision + 1].Connected || sleepCount >= 1700) // Timeout : 17 sec.
                                //    {
                                //        MessageBox.Show(ResourceStringHelper.GetErrorMessage("MT004"), "Error");
                                //        MainWindow.Log("PCS", SeverityLevel.ERROR, "영상 획득에 실패하였습니다.");
                                //        TeachingViewer.SelectedViewer.IsGrabDone = false;
                                //        return;
                                //    }
                                //    Thread.Sleep(10);
                                //    sleepCount++;
                                //}
                                ///////////////////////
                                m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Grab(VisionDefinition.LINESCAN_FORWARD, full, indexInfo.SurfaceScanCount);
                            }
                            else//BA , CA - slave
                            {
                                m_ptrMainWindow.PCSInstance.Vision[tempDesVision - 1].Grab_Ready = false;
                                m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Grab_Ready = false;
                                m_ptrMainWindow.PCSInstance.Vision[tempDesVision - 1].Grab(VisionDefinition.LINESCAN_NOGRAB, false, indexInfo.SurfaceScanCount);
                                sleepCount = 0;
                                while (!(m_ptrMainWindow.PCSInstance.Vision[tempDesVision - 1].Grab_Ready))
                                {
                                    if (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision - 1].Connected || sleepCount >= 1700) // Timeout : 17 sec.
                                    {
                                        MessageBox.Show(ResourceStringHelper.GetErrorMessage("MT004"), "Error");
                                        MainWindow.Log("PCS", SeverityLevel.ERROR, "영상 획득에 실패하였습니다.");
                                        TeachingViewer.SelectedViewer.IsGrabDone = false;
                                        return;
                                    }
                                    Thread.Sleep(10);
                                    sleepCount++;
                                }
                                m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Grab(VisionDefinition.LINESCAN_FORWARD, full, indexInfo.SurfaceScanCount);
                            }
                        }

                        while (!(m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Grab_Ready))
                        {
                            if (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Connected || sleepCount >= 1700) // Timeout : 17 sec.
                            {
                                MessageBox.Show(ResourceStringHelper.GetErrorMessage("MT004"), "Error");
                                MainWindow.Log("PCS", SeverityLevel.ERROR, "영상 획득에 실패하였습니다.");
                                TeachingViewer.SelectedViewer.IsGrabDone = false;

                                return;
                            }
                            Thread.Sleep(10);
                            sleepCount++;
                        }

                        try
                        {
                            Thread.Sleep(100);
                            if (MainWindow.Setting.SubSystem.ENC.EncoderType == 1)
                            {
                                if (indexInfo.CategorySurface != CategorySurface.BA)
                                    m_ptrMainWindow.PCSInstance.ResetTrigger(0);
                                else
                                    m_ptrMainWindow.PCSInstance.ResetTrigger(1);
                            }

                            if (indexInfo.CategorySurface != CategorySurface.BA)
                                m_ptrMainWindow.PCSInstance.PlcDevice.PassBoat(0);
                            else
                                m_ptrMainWindow.PCSInstance.PlcDevice.PassBoat(1);
                        }
                        catch
                        {
                        }
                        TeachingViewer.SelectedViewer.IsGrabDone = true;

                        sleepCount = 0;

                        while (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Grab_Done)
                        {
                            if (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Connected || sleepCount >= 150) // Timeout : 7 sec.
                            {
                                lightcontrol.LightOn(false);
                                // MT004 : 영상 획득에 실패하였습니다. 다시 시도해 주시기 바랍니다.
                                MessageBox.Show(ResourceStringHelper.GetErrorMessage("MT004"), "Error");
                                MainWindow.Log("PCS", SeverityLevel.ERROR, "영상 획득에 실패하였습니다.");
                                TeachingViewer.SelectedViewer.IsGrabDone = false;

                                return;
                            }
                            if (full) Thread.Sleep(800);
                            else Thread.Sleep(200);
                            sleepCount++;
                        }

                        lightcontrol.LightOn(false);
                    }
                    else
                    {
                        MessageBox.Show("영상획득 준비가 되지 않았습니다", "Information");
                        //  m_ptrMainWindow.PCSInstance.SCACounter(nDesVision);
                        lightcontrol.LightOn(false);
                    }
                }
                #endregion

                #region 로컬 환경에서 영상 취득시.
                else
                {
                    m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Grab_Ready = false;
                    m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Grab(VisionDefinition.LINESCAN_FORWARD, false, indexInfo.SurfaceScanCount);
                    int sleepCount = 0;
                    while (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Grab_Ready)
                    {

                        if (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Connected || sleepCount >= 1500) // Timeout : 17 sec.
                        {
                            // MT004 : 영상 획득에 실패하였습니다. 다시 시도해 주시기 바랍니다.
                            MessageBox.Show(ResourceStringHelper.GetErrorMessage("MT004"), "Error");
                            MainWindow.Log("PCS", SeverityLevel.ERROR, "영상 획득에 실패하였습니다.");
                            TeachingViewer.SelectedViewer.IsGrabDone = false;

                            return;
                        }
                        Thread.Sleep(10);
                        sleepCount++;
                    }
                    TeachingViewer.SelectedViewer.IsGrabDone = true;

                    sleepCount = 0;
                    while (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Grab_Done)
                    {
                        if (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Connected || sleepCount >= 12000) // Timeout : 10 sec.
                        {
                            // MT004 : 영상 획득에 실패하였습니다. 다시 시도해 주시기 바랍니다.
                            MessageBox.Show(ResourceStringHelper.GetErrorMessage("MT004"), "Error");
                            MainWindow.Log("PCS", SeverityLevel.ERROR, "영상 획득에 실패하였습니다.");
                            TeachingViewer.SelectedViewer.IsGrabDone = false;

                            return;
                        }
                        Thread.Sleep(100);
                        sleepCount++;
                    }
                    SetText("영상 취득 완료", 2000);
                }
                #endregion
                TeachingViewer.SelectedViewer.CornerImage = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].BaseCornerImage;////컬러가 필요하면 바꾸자
                if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] >= 6)
                    RGBChange(ChannelIndex, nDesVision);
                else
                {
                    TeachingViewer.SelectedViewer.GrabImage = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].BasedImage;
                    if (TeachingViewer.SelectedViewer.GrabImage != null)
                    {
                        TeachingViewer.SelectedViewer.IsGrabView = true;
                        TeachingViewer.SelectedViewer.UpdateViewerSource(TeachingViewer.SelectedViewer.GrabImage);
                        TeachingViewer.SelectedViewer.pnlInner.Children.Clear();
                        TeachingViewer.SelectedViewer.pnlInner.Children.Add(TeachingViewer.SelectedViewer.BasedImage); // Image Control
                        TeachingViewer.SelectedViewer.pnlInner.Children.Add(TeachingViewer.SelectedViewer.BasedROICanvas); // Drawing Canvas

                        TeachingViewer.SelectedViewer.ToolChange(ToolType.Pointer);

                        bool bManu_PSRodd = true;
                        if (indexInfo.CategorySurface == CategorySurface.BP) 
                            bManu_PSRodd = false;//BP에서는 PSR ODD 사용하면 안된다
                        TeachingViewer.SelectedViewer.TeachingSubMenuCtrl.ChangeTeachingMode(TeachingType.Entire, bManu_PSRodd);
                        TeachingViewer.SelectedViewer.ThumbnailViewer.SetSourceImage(TeachingViewer.SelectedViewer);

                        if (TeachingViewer.SelectedViewer.BasedROICanvas.GraphicsList.Count == 0)
                        {
                            TeachingViewer.SelectedViewer.LoadTeachingData(); // Teaching Data를 읽어들인다.
                            if (TeachingViewer.SelectedViewer.BasedROICanvas.GraphicsList.Count > 0)
                            {
                                MessageBox.Show("티칭 정보가 자동 갱신되었습니다.", "Teaching Data Manager");
                            }
                            TeachingViewer.SelectedViewer.UpdateViewerSource(TeachingViewer.SelectedViewer.GrabImage);
                        }
                        TeachingViewer.SelectedViewer.lbSection.SelectedIndex = -1;

                        HistogramCtrl.Refresh();
                        LineProfileCtrl.SetLineProfileSource(TeachingViewer.SelectedViewer.GrabImage);
                        LineProfileCtrl.Refresh();
                        HistogramCtrl.ImageChanged(TeachingViewer.SelectedViewer.ReferenceImage, TeachingViewer.SelectedViewer.GrabImage);
                        TeachingViewer.SelectedViewer.txtReferenceLabel.Visibility = Visibility.Hidden;
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Exception occured in btnGrabEntireImage_Click(TeachingWindow.xaml.cs");
                if (MainWindow.Setting.SubSystem.PLC.UsePLC) lightcontrol.LightOn(false);
            }
        }

        private void VisionInterface_LiveImageUpdateEvent(int anIndex)
        {
            IndexInfo indexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(anIndex - 1);
            int anVision = indexInfo.VisionIndex;
            if (m_ptrMainWindow.PCSInstance.Vision[anVision].BasedImage != null)
            {
                Action action = () =>
                {
                    TeachingViewers[anIndex].GrabImage = BitmapSourceHelper.CloneBitmapSource(m_ptrMainWindow.PCSInstance.Vision[anVision].BasedImage);
                    if (TeachingViewers[anIndex].GrabImage != null)
                    {
                        TeachingViewers[anIndex].IsGrabView = true;
                        TeachingViewers[anIndex].UpdateViewerSource(TeachingViewers[anIndex].GrabImage);

                        TeachingViewers[anIndex].pnlInner.Children.Clear();
                        TeachingViewers[anIndex].pnlInner.Children.Add(TeachingViewers[anIndex].BasedImage);
                        TeachingViewers[anIndex].pnlInner.Children.Add(TeachingViewers[anIndex].BasedROICanvas);

                        TeachingViewers[anIndex].ToolChange(ToolType.Pointer);
                        bool bManu_PSRodd = true;
                        if (indexInfo.CategorySurface == CategorySurface.BP)
                            bManu_PSRodd = false;//BP에서는 PSR ODD 사용하면 안된다
                        TeachingViewers[anIndex].TeachingSubMenuCtrl.ChangeTeachingMode(TeachingType.Entire, bManu_PSRodd);

                        TeachingViewers[anIndex].ThumbnailViewer.SetSourceImage(TeachingViewers[anIndex]);
                        TeachingViewers[anIndex].lbSection.SelectedIndex = -1;
                        TeachingViewers[anIndex].txtReferenceLabel.Visibility = Visibility.Hidden;

                        if (TeachingViewer.SelectedViewer == TeachingViewers[anIndex])
                        {
                            HistogramCtrl.Refresh();
                            LineProfileCtrl.SetLineProfileSource(TeachingViewers[anIndex].GrabImage);
                            LineProfileCtrl.Refresh();
                        }
                    }
                };
                m_ptrMainWindow.Dispatcher.Invoke(action);
            }
        }

        // 기준 영상 등록.
        private void RegisterBasedImage()
        {
            TeachingViewerCtrl viewer = TeachingViewer.SelectedViewer;
            IndexInfo indexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(viewer.ID);
            if (viewer == null) return;

            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
            {
                MessageBox.Show("모델을 선택해 주시기 바랍니다.", "Information");
                return;
            }

            if (viewer.GrabImage != null && viewer.BasedImage.Source == viewer.GrabImage)
            {
                viewer.ReferenceImage = viewer.GrabImage;
                viewer.GrabImage = null;
                viewer.BasedImage.Source = viewer.ReferenceImage;
                viewer.SeeEntireImage();
            }

            if (viewer.ReferenceImage != null)
            {
                if (indexInfo.CategorySurface == CategorySurface.BP && indexInfo.Slave)//BP Slave
                {
                    bool bUnitRegionExists = false;
                    GraphicsRectangle CurrentROI = null;
                    foreach (GraphicsRectangle rectGraphic in TeachingViewers[0].BasedROICanvas.FiduGraphicsList)
                    {
                        if (rectGraphic.RegionType == GraphicsRegionType.UnitRegion || rectGraphic.RegionType == GraphicsRegionType.PSROdd)
                        {
                            CurrentROI = rectGraphic;

                            bUnitRegionExists = true;
                            break;
                        }
                    }
                    if (!bUnitRegionExists)
                    {
                        MessageBox.Show("BondPad1의 유닛 섹션이 없습니다. BondPad1의 섹션 등록 후 진행 해 주세요.");
                        return;
                    }
                    viewer.BasedROICanvas.GraphicsList.Clear();
                    bUnitRegionExists = false;
                    BitmapSource bmp = null;

                    foreach (SectionInformation sec in TeachingViewers[0].SectionManager.Sections)
                    {
                        if (sec.Type.Code == SectionTypeCode.UNIT_REGION || sec.Type.Code == SectionTypeCode.PSR_REGION)
                        {
                            bmp = sec.GetBitmapSource(0);
                            bUnitRegionExists = true;
                            break;
                        }
                    }
                    if (!bUnitRegionExists)
                    {
                        MessageBox.Show("BondPad1 의 유닛 섹션이 없습니다. BondPad1에 섹션 등록 후 진행 해 주세요.");
                        return;
                    }

                    if (!viewer.SearchROIPatternSub(CurrentROI, 0, bmp))
                    {
                        MessageBox.Show("섹션 생성에 실패 하였습니다.");
                        return;
                    }
                    // teachingHelperWindow.CurrentROI.IsSelected = true;
                    // TeachingWindow.Symmetry = teachingHelperWindow.SectionSymmetryType;
                    // }
                    SaveReferenceEntireImage(viewer.ID);
                    CreateSection();
                }
                else
                {
                    SaveReferenceEntireImage(viewer.ID);
                    SaveReferenceSectionImages(viewer.ID);
                }

            }
            SetText("기준 영상 등록", 2000);
        }

        private void CreateBallDataFile()
        {
            if (!IsOnLine) // Blocking offline mode.
                return;
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
                return;

            string szGroupName = MainWindow.CurrentGroup.Name;
            string szModelName = MainWindow.CurrentModel.Name;

            if (!Directory.Exists(Path.Combine(DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName), "Ball")))
                Directory.CreateDirectory(Path.Combine(DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName), "Ball"));

            for (int nIndex = 0; nIndex < TeachingViewers.Length; nIndex++)
            {
                string szBallDataPath = DirectoryManager.GetBallDataPath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName, TeachingViewers[nIndex].SurfaceType);
                if (!string.IsNullOrEmpty(szBallDataPath))
                {
                    FileSupport.TryDeleteFile(szBallDataPath);
                    using (StreamWriter sw = new StreamWriter(szBallDataPath)) { /* just create file. */ }
                }
            }
        }

        private void DeleteEmptyBallDataFile()
        {
            if (!IsOnLine) // Blocking offline mode.
                return;
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
                return;

            string szGroupName = MainWindow.CurrentGroup.Name;
            string szModelName = MainWindow.CurrentModel.Name;

            if (!Directory.Exists(Path.Combine(DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName), "Ball")))
                return;

            for (int nIndex = 0; nIndex < TeachingViewers.Length; nIndex++)
            {
                string szBallDataPath = DirectoryManager.GetBallDataPath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName, TeachingViewers[nIndex].SurfaceType);
                if (!string.IsNullOrEmpty(szBallDataPath))
                {
                    FileInfo fileInfo = new FileInfo(szBallDataPath);
                    if (fileInfo.Length == 0)
                        FileSupport.TryDeleteFile(szBallDataPath);
                }
            }

            // Directory Center Line Dir.
            try
            {
                Directory.Delete(Path.Combine(DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName), "Ball"));
            }
            catch { }
        }

        private void CreateCenterLineDataFile()
        {
            if (!IsOnLine) // Blocking offline mode.
                return;
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
                return;

            string szGroupName = MainWindow.CurrentGroup.Name;
            string szModelName = MainWindow.CurrentModel.Name;

            if (!Directory.Exists(Path.Combine(DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName), "CenterLine")))
                Directory.CreateDirectory(Path.Combine(DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName), "CenterLine"));

            for (int nIndex = 0; nIndex < TeachingViewers.Length; nIndex++)
            {
                string szCenterLineDataPath = DirectoryManager.GetCenterLineDataPath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName, TeachingViewers[nIndex].SurfaceType);
                if (!string.IsNullOrEmpty(szCenterLineDataPath))
                {
                    FileSupport.TryDeleteFile(szCenterLineDataPath);
                    using (StreamWriter sw = new StreamWriter(szCenterLineDataPath)) { /* just create file. */ }
                }
            }
        }

        private void DeleteEmptyCenterLineDataFile()
        {
            if (!IsOnLine) // Blocking offline mode.
                return;
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
                return;

            string szGroupName = MainWindow.CurrentGroup.Name;
            string szModelName = MainWindow.CurrentModel.Name;

            if (!Directory.Exists(Path.Combine(DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName), "CenterLine")))
                return;

            for (int nIndex = 0; nIndex < TeachingViewers.Length; nIndex++)
            {
                string szCenterLineDataPath = DirectoryManager.GetCenterLineDataPath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName, TeachingViewers[nIndex].SurfaceType);
                if (!string.IsNullOrEmpty(szCenterLineDataPath))
                {
                    FileInfo fileInfo = new FileInfo(szCenterLineDataPath);
                    if (fileInfo.Length == 0)
                        FileSupport.TryDeleteFile(szCenterLineDataPath);
                }
            }

            // Directory Center Line Dir.
            try
            {
                Directory.Delete(Path.Combine(DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName), "CenterLine"));
            }
            catch { }
        }

        // 전체 영상 저장.
        public void SaveReferenceEntireImage(int anDesVision, bool abIsQuickMode = false)
        {
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
                return;

            string szGroupName = MainWindow.CurrentGroup.Name;
            string szModelName = MainWindow.CurrentModel.Name;
            string szBasedImagePath = string.Empty;
            string szMarkImagePath = string.Empty;
            if (!IsOnLine && MtsManager.SelectedMachine != null)
            {
                szBasedImagePath = DirectoryManager.GetOfflineBasedImagePath(MainWindow.Setting.General.ModelPath, MtsManager.SelectedMachine.Name, szGroupName, szModelName, TeachingViewers[anDesVision].SurfaceType);
            }
            else
            {
                szBasedImagePath = DirectoryManager.GetBasedImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName, TeachingViewers[anDesVision].SurfaceType);
                szMarkImagePath = DirectoryManager.GetMarkImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName, TeachingViewers[anDesVision].SurfaceType);
            }

            if (abIsQuickMode)
            {
                if (File.Exists(szBasedImagePath))
                    return;
            }

            if (!string.IsNullOrEmpty(szBasedImagePath))
            {
                if (TeachingViewers[anDesVision].ReferenceImage != null)
                {
                    TeachingViewers[anDesVision].BasedImage.Source = null;
                    System.GC.Collect();
                    if (FileSupport.TryDeleteFile(szBasedImagePath))
                    {
                        FileSupport.SaveImageFile(szBasedImagePath, TeachingViewers[anDesVision].ReferenceImage);
                    }
                    TeachingViewers[anDesVision].BasedImage.Source = TeachingViewers[anDesVision].ReferenceImage;
                    TeachingViewers[anDesVision].txtReferenceLabel.Visibility = Visibility.Visible;
                }
            }
        }

        // 섹션 영상 저장.
        public void SaveReferenceSectionImages(int anDesVision, bool abIsQuickMode = false)
        {
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
                return;
            IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(anDesVision);
            Surface surface = TeachingViewers[anDesVision].SurfaceType;
            string szGroupName = MainWindow.CurrentGroup.Name;
            string szModelName = MainWindow.CurrentModel.Name;
            foreach (SectionInformation section in TeachingViewers[anDesVision].SectionManager.Sections)
            {
                string[] szSectionImagePath = new string[3];
                if (MainWindow.Setting.SubSystem.IS.FGType[Math.Max(0, VID.CalcIndex(IndexInfo.VisionIndex))] < 6)
                {
                    if (!IsOnLine && MtsManager.SelectedMachine != null)
                    {
                        szSectionImagePath[0] = DirectoryManager.GetOfflineSectionImagePath(MainWindow.Setting.General.ModelPath, MtsManager.SelectedMachine.Name, szGroupName, szModelName, section.Name, surface);
                    }
                    else
                    {
                        szSectionImagePath[0] = DirectoryManager.GetSectionImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName, section.Name, surface, -1);
                    }

                    if (!string.IsNullOrEmpty(szSectionImagePath[0]))
                    {
                        if (section.BitmapSource[0] == null)
                            section.BitmapSource[0] = section.GetBitmapSource(0);

                        if (abIsQuickMode)
                        {
                            if (File.Exists(szSectionImagePath[0]))
                                continue;
                        }
                        section.Image[0].Source = null;

                        System.GC.Collect();
                        if (FileSupport.TryDeleteFile(szSectionImagePath[0]))
                        {
                            FileSupport.SaveImageFile(szSectionImagePath[0], section.BitmapSource[0]);
                        }

                        section.Image[0].Source = section.BitmapSource[0];
                        TeachingViewer.SelectedViewer.txtReferenceLabel.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (!IsOnLine && MtsManager.SelectedMachine != null)
                    {
                        szSectionImagePath = DirectoryManager.GetOfflineSectionRGBImagePath(MainWindow.Setting.General.ModelPath, MtsManager.SelectedMachine.Name, szGroupName, szModelName, section.Name, surface);
                    }
                    else
                    {
                        szSectionImagePath = DirectoryManager.GetSectionRGBImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName, section.Name, surface);
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        if (!string.IsNullOrEmpty(szSectionImagePath[i]))
                        {
                            if (section.BitmapSource[i] == null)
                                section.SetBitmapSource(section.GetBitmapSource(i), i);

                            if (abIsQuickMode)
                            {
                                if (File.Exists(szSectionImagePath[i]))
                                    continue;
                            }
                            section.Image[i].Source = null;

                            System.GC.Collect();
                            if (FileSupport.TryDeleteFile(szSectionImagePath[i]))
                            {
                                FileSupport.SaveImageFile(szSectionImagePath[i], section.BitmapSource[i]);
                            }

                            section.Image[i].Source = section.BitmapSource[i];
                            TeachingViewer.SelectedViewer.txtReferenceLabel.Visibility = Visibility.Visible;
                        }
                    }
                }

                m_ptrMainWindow.SaveResizeSectionImage(szSectionImagePath[0]);
            }
        }

        public void SaveReferenceCornerImage(int anDesVision, bool abIsQuickMode = false)
        {
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
                return;

            string szGroupName = MainWindow.CurrentGroup.Name;
            string szModelName = MainWindow.CurrentModel.Name;
            string szCornerImagePath = string.Empty;
            if (!IsOnLine && MtsManager.SelectedMachine != null)
            {
                szCornerImagePath = DirectoryManager.GetOfflineCornerImagePath(MainWindow.Setting.General.ModelPath, MtsManager.SelectedMachine.Name, szGroupName, szModelName, TeachingViewers[anDesVision].SurfaceType);
            }
            else
            {
                szCornerImagePath = DirectoryManager.GetCornerImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName, TeachingViewers[anDesVision].SurfaceType);
            }

            //if (abIsQuickMode)
            //{
            //    if (File.Exists(szCornerImagePath))
            //        return;
            //}
            if (FileSupport.TryDeleteFile(szCornerImagePath))
            {
                FileSupport.SaveImageFile(szCornerImagePath, TeachingViewers[anDesVision].CornerImage);
            }
        }
        #endregion 영상 취득 및 저장

        #region 비전 정보 전송
        // 비전 정보 전송 요청.
        private void ReqSendVisionData()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
            {
                MessageBox.Show("모델을 선택해 주시기 바랍니다.", "Information");
                return;
            }
            if (TeachingViewer.SelectedViewer.lbSection.Items.Count == 0)
            {
                MessageBox.Show("섹션 데이터를 생성 후 Vision 정보를 전송 바랍니다.", "Information");
                return;
            }
            IndexInfo info = MainWindow.Convert_ViewIndexToVisionIndexs(TeachingViewer.SelectedViewer.ID);
            this.Cursor = Cursors.Wait;
            btnSendPara.IsEnabled = btnPartialInspect.IsEnabled = btnManualInspect.IsEnabled = false;
            btnSendPara.Refresh();
            if (!SendVisionData(false, true, 1, TeachingViewer.SelectedViewer))
            {
                this.Cursor = Cursors.Arrow;
                this.btnSendPara.IsEnabled = true;
                this.btnPartialInspect.IsEnabled = false;
                this.btnManualInspect.IsEnabled = false;

                if (info.CategorySurface == CategorySurface.BP)
                    m_ptrMainWindow.ProgressWindow.StopProgress(0);
                else
                    m_ptrMainWindow.ProgressWindow.StopProgress(info.VisionIndex);
                MainWindow.Log("PCS", SeverityLevel.ERROR, "Vision 정보 전송에 실패하였습니다.", true);
                return;
            }
   
            TeachingViewer.SelectedViewer.IsSentDone = true;
            UpdateLvResult();
            SetManualInspectState(true);

            this.Cursor = Cursors.Arrow;
            btnSendPara.IsEnabled = btnPartialInspect.IsEnabled = btnManualInspect.IsEnabled = true;
            if (TeachingViewer.SelectedViewer.lbSection.SelectedIndex == -1)
            {
                btnPartialInspect.IsEnabled = false;
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            Thread.Sleep(100);
        }

        // 자동검사 및 수동검사 비전 정보 전송
        public bool SendVisionData(bool abIsAutoInspection, bool aGetLiveImage, int anTryCount = 2, TeachingViewerCtrl anDesVision = null)//viewer id
        {
            int[] sendedCount = new int[MainWindow.m_nTotalTeachingViewCount];
            MainWindow.lstRC.Clear();
            if (abIsAutoInspection)
            {
                #region 자동검사 비전 전송
                if (m_ptrMainWindow == null)
                    m_ptrMainWindow = Application.Current.MainWindow as MainWindow;

                m_ptrMainWindow.ProgressWindow.Owner = m_ptrMainWindow;
                m_ptrMainWindow.ProgressWindow.StartProgress(-1);   // 자동 -1, 수동 TargetVision

                // SendParameter
                for (int nCurrVision = 0; nCurrVision < TeachingViewers.Length; nCurrVision++)
                {
                    IndexInfo IndexInfo = m_ptrMainWindow.Convert_ViewIndexToVisionIndex(nCurrVision);
                    int tempCurrvision = IndexInfo.VisionIndex;

                    bool bIsSimulationMode = !m_ptrMainWindow.PCSInstance.IsOpenPlc && !m_ptrMainWindow.PCSInstance.Vision[tempCurrvision].Connected;
                    bool bIsSkip = false;

                    if (IndexInfo.CategorySurface == CategorySurface.BP)
                    {
                        if (nCurrVision / VID.BP_ScanComplete_Count >= InspectProcess.BPInspectSkip.Length) bIsSkip = true;
                        else bIsSkip = InspectProcess.BPInspectSkip[nCurrVision / VID.BP_ScanComplete_Count];
                    }
                    else if (IndexInfo.CategorySurface == CategorySurface.CA)
                    {
                        if (nCurrVision - IndexInfo.SurfaceBeginIndex >= InspectProcess.CAInspectSkip.Length) bIsSkip = true;
                        else bIsSkip = InspectProcess.CAInspectSkip[nCurrVision - IndexInfo.SurfaceBeginIndex];
                    }                        
                    else
                    {
                        if (nCurrVision - IndexInfo.SurfaceBeginIndex >= InspectProcess.BAInspectSkip.Length) bIsSkip = true;
                        else bIsSkip = InspectProcess.BAInspectSkip[nCurrVision - IndexInfo.SurfaceBeginIndex];
                    }
                    // 시뮬레이션 환경이거나 검사 제외 적용된 경우 Vision 전송을 생략한다.
                    if (bIsSkip || bIsSimulationMode)
                    {
                        m_ptrMainWindow.PCSInstance.Vision[tempCurrvision].Register_Done = true;
                        m_ptrMainWindow.PCSInstance.Vision[tempCurrvision].Register_Fail = false;
                        continue;
                    }

                    m_ptrMainWindow.PCSInstance.Vision[tempCurrvision].GetLiveImage = aGetLiveImage;
                    do
                    {
                        sendedCount[nCurrVision] = SendVisionData(nCurrVision, abIsAutoInspection, MainWindow.UseOuter);
                        if (!m_ptrMainWindow.PCSInstance.Vision[tempCurrvision].Connected)
                        {
                            Debug.WriteLine("#### Retry Send Vision Data");
                            if (!m_ptrMainWindow.PCSInstance.Vision[tempCurrvision].RecoveryConnection()) // 소켓 복구 실패!!
                                return false;
                        }
                    }
                    while (--anTryCount > 0 && (sendedCount[nCurrVision] == -1));
                    if (sendedCount[nCurrVision] == -1)
                    {
                        MainWindow.Log("PCS", SeverityLevel.FATAL, string.Format("Vision 정보 전송에 실패하였습니다. 식별자: {0}, 복구 시도 회수: {1}.", nCurrVision + 1, anTryCount), true);
                        Debug.WriteLine("#### Send Vision Data Fail.");
                        return false;
                    }

                    m_ptrMainWindow.PCSInstance.Vision[tempCurrvision].SendPacket(VisionDefinition.PING);
                    Thread.Sleep(500);
                    //if (IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave)
                    //{
                    //    m_ptrMainWindow.PCSInstance.Vision[tempCurrvision].Register_Done = true;
                    //    m_ptrMainWindow.PCSInstance.Vision[tempCurrvision].Register_Fail = false;
                    //    m_ptrMainWindow.PCSInstance.Vision[tempCurrvision].RegisterDone();
                    //}
                    //else
                    //{
                        if (sendedCount[nCurrVision] > 0)
                        {
                            m_ptrMainWindow.PCSInstance.Vision[tempCurrvision].Register_Done = false;
                            m_ptrMainWindow.PCSInstance.Vision[tempCurrvision].Register_Fail = false;
                            m_ptrMainWindow.PCSInstance.Vision[tempCurrvision].RegisterDone();
                        }
                        else
                        {
                            m_ptrMainWindow.PCSInstance.Vision[tempCurrvision].Register_Done = true;
                            m_ptrMainWindow.PCSInstance.Vision[tempCurrvision].Register_Fail = false;
                        }
                    //}
                    Thread.Sleep(1000);
                }

                // Wait & Progressbar Set
                int nTimeOutCount = 300; // 전체 timeout 300초.
                int nSleepCount = 0;
                bool Check_Register_Done = false;
                while (!Check_Register_Done)
                {
                    for (int n = 0; n < m_ptrMainWindow.PCSInstance.Vision.Length; n++)////////모든 Vision Register_Done이면 While문 탈출
                    {
                        if (!m_ptrMainWindow.PCSInstance.Vision[n].Register_Done) { break; }
                        if (n == m_ptrMainWindow.PCSInstance.Vision.Length - 1) { Check_Register_Done = true; continue; }
                    }

                    bool Check_Register_Fail = false;
                    for (int n = 0; n < m_ptrMainWindow.PCSInstance.Vision.Length; n++)////////전체 Vision 중 Register_Fail 이 하나라도 있으면 Error 출력을 위한 Check
                    {
                        if (m_ptrMainWindow.PCSInstance.Vision[n].Register_Fail) { Check_Register_Fail = true; }
                    }
                    if (nSleepCount >= nTimeOutCount || Check_Register_Fail)
                    {
                        string code = Get_Error_Algo(m_ptrMainWindow.PCSInstance.Vision);

                        m_ptrMainWindow.ProgressWindow.StopProgress(-1);
                        if (nSleepCount >= nTimeOutCount)
                            MessageBox.Show("Vision 정보를 전송하지 못하였습니다.(Code:Time Out)", "Error");
                        else
                            MessageBox.Show("Vision 정보를 전송하지 못하였습니다.(Code : " + code + ")", "Error");
                        return false;
                    }
                    Thread.Sleep(1000);
                    nSleepCount++;                 
                }

                m_ptrMainWindow.ProgressWindow.StopProgress(-1);
                #endregion
            }
            else
            {
                #region 수동검사 비전 전송
                SetText("Vision 정보 전송", 3000);

                IndexInfo IndexInfo = m_ptrMainWindow.Convert_ViewIndexToVisionIndex(anDesVision.ID);
                int tempDesVision = IndexInfo.VisionIndex;

                VisionInterface desVision = m_ptrMainWindow.PCSInstance.Vision[tempDesVision];
                m_ptrMainWindow.ProgressWindow.Owner = this;
                m_ptrMainWindow.ProgressWindow.StartProgress(anDesVision.ID);   // 자동 -1, 수동 TargetVision
                m_ptrMainWindow.PCSInstance.Vision[tempDesVision].GetLiveImage = aGetLiveImage;
                do
                {
                    sendedCount[tempDesVision] = SendVisionData(anDesVision.ID, abIsAutoInspection, true); // Send Parameter

                    if (!desVision.Connected)
                    {
                        Debug.WriteLine("#### Retry Send Vision Data");
                        if (!desVision.RecoveryConnection()) // 소켓 복구 실패!!
                            return false;
                    }
                }
                while (--anTryCount > 0 && (sendedCount[anDesVision.ID] == -1));
                if (sendedCount[anDesVision.ID] == -1)
                {
                    MainWindow.Log("PCS", SeverityLevel.FATAL, string.Format("Vision 정보 전송에 실패하였습니다. 복구 시도 회수: {0}.", anTryCount), true);
                    Debug.WriteLine("#### Send Vision Data Fail.");
                    return false;
                }

                #region Register Done.
                desVision.SendPacket(VisionDefinition.PING);
                Thread.Sleep(500);
                //if (IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave == true)
                //{
                //    desVision.Register_Done = true;
                //    desVision.Register_Fail = false;
                //    desVision.RegisterDone();
                //}
                //else
                //{
                    if (sendedCount[tempDesVision] > 0)
                    {
                        desVision.Register_Done = false;
                        desVision.Register_Fail = false;
                        desVision.RegisterDone();
                    }
                    else
                    {
                        desVision.Register_Done = true;
                        desVision.Register_Fail = false;
                    }
                //}
                #endregion

                // Wait & Progressbar Set
                int nTimeOutCount = 240; // timeout 240초.
                int nSleepCount = 0;
                while (!desVision.Register_Done)
                {
                    if (nSleepCount >= nTimeOutCount || desVision.Register_Fail)
                    {
                        string code = Get_Error_Algo(m_ptrMainWindow.PCSInstance.Vision);

                        m_ptrMainWindow.ProgressWindow.StopProgress(-1);
                        if (nSleepCount >= nTimeOutCount)
                            MessageBox.Show("Vision 정보를 전송하지 못하였습니다.(Code:Time Out)", "Error");
                        else
                            MessageBox.Show("Vision 정보를 전송하지 못하였습니다.(Code : " + code + ")", "Error");
                        return false;
                    }
                    Thread.Sleep(1000);
                    nSleepCount++;
                }
                if (!(IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave))////BP, Slave 제외
                {
                    RequestCenterLineInfo(anDesVision.ID);
                    RequestBallInfo(anDesVision.ID);
                }
                m_ptrMainWindow.ProgressWindow.StopProgress(anDesVision.ID);
                #endregion
            }
            return true;
        }

        private string Get_Error_Algo(VisionInterface[] vision)
        {
            string code = "";
            for (int i = 0; i < vision.Length; i++)
            {
                if (vision[i].Register_Fail)
                {
                    switch (vision[i].Register_Fail_Algo)
                    {
                        case (int)eVisInspectType.eInspTypeOuter:
                            code = "외곽 검사";
                            break;
                        case (int)eVisInspectType.eInspTypeRailFiducial:
                            code = "외곽 인식키";
                            break;
                        case (int)eVisInspectType.eInspTypeIDMark:
                            code = "ID Mark";
                            break;
                        case (int)eVisInspectType.eInspTypeUnitAlign:
                            code = "Unit Align";
                            break;
                        case (int)eVisInspectType.eInspTypeDamBar:
                            code = "Dam Size 검사";
                            break;
                        case (int)eVisInspectType.eInspTypeSurface:
                            code = "표면 검사";
                            break;
                        case (int)eVisInspectType.eInspTypeDownSet:
                            code = "Via 검사";
                            break;
                        case (int)eVisInspectType.eInspTypeUnitRawMaterial:
                            code = "원소재 검사";
                            break;
                        case (int)eVisInspectType.eInspTypePSR:
                            code = "PSR 검사";
                            break;
                        case (int)eVisInspectType.eInspTypeSpace:
                            code = "Core Crack 검사";
                            break;
                        case (int)eVisInspectType.eInspTypePunchBurr:
                            code = "Burr 검사";
                            break;
                        case (int)eVisInspectType.eInspTypeBallPattern:
                            code = "Ball 검사";
                            break;
                        case (int)eVisInspectType.eInspTypeVentHole:
                            code = "Vent Hole 검사";
                            break;
                        case (int)eVisInspectType.eInspTypeNoResizeVentHole:
                            code = "Vent Hole 외곽검사";
                            break;
                        case (int)eVisInspectType.eInspTypeUnitFidu:
                            code = "인식키 검사";
                            break;
                        case (int)eVisInspectType.eInspTypeCoreCrack:
                            code = "Window Punch 검사";
                            break;
                        case (int)eVisInspectType.eInspTypeRailIntensity:
                            code = "외곽 검사";
                            break;
                        case (int)eVisInspectType.eInspTypeLeadShapeWithCL:
                            code = "리드 형상 검사";
                            break;
                        case (int)eVisInspectType.eInspTypeSpaceShapeWithCL:
                            code = "공간 형상 검사";
                            break;
                        case (int)eVisInspectType.eInspTypeExceptionalMask:
                            code = "검사 제외";
                            break;
                        case (int)eVisInspectType.eInspTypePSROdd:
                            code = "PSR이물 검사";
                            break;
                        case (int)eVisInspectType.eInspResultGV:
                            code = "GV 검사";
                            break;
                        case -1:
                            code = "Init-Error";
                            break;
                        default:
                            code = "Buff-Fail";
                            break;
                    }
                }
            }
            return code;
        }

        // 2012-08-06 suoow2 Added; 중앙선 데이터를 요청한다.
        private bool RequestCenterLineInfo(int anDesVision)//viewers 0-5
        {
            IndexInfo IndexInfo = m_ptrMainWindow.Convert_ViewIndexToVisionIndex(anDesVision);
            int tempDesVision = IndexInfo.VisionIndex;
            int FrameGrabberIndex = VID.CalcIndex(tempDesVision);

            TeachingViewerCtrl viewer = TeachingViewers[anDesVision];
            VisionInterface desVision = m_ptrMainWindow.PCSInstance.Vision[tempDesVision];
            if (viewer == null || desVision == null) return false;

            int nRoiCode = 0;
            foreach (SectionInformation section in viewer.SectionManager.Sections)
            {
                for (int i = 0; i < 3; i++)
                {
                    if ((MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] < 6) && i == 1) break;
                    foreach (GraphicsBase g in section.ROICanvas[i].GraphicsList)  // ROI별 반복
                    {
                        if (g.RegionType == GraphicsRegionType.IDMark || g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine || g.RegionType == GraphicsRegionType.TapeLoaction)
                            continue;
                        if (g is GraphicsSelectionRectangle || g is GraphicsLine)
                            continue;

                        foreach (InspectionItem inspItem in g.InspectionList)
                        {
                            if (inspItem.InspectionType.InspectType == eVisInspectType.eInspTypeLeadShapeWithCL ||
                                inspItem.InspectionType.InspectType == eVisInspectType.eInspTypeSpaceShapeWithCL ||
                                inspItem.InspectionType.InspectType == eVisInspectType.eInspTypePlateWithCL)
                            {
                                //if (inspItem.LineSegments != null)
                                //    continue;

                                desVision.ReqCenterLineInfo(inspItem.SentID.Value);
                                int nSleepCount = 0;
                                while (!desVision.Recv_CenterLine_Done)
                                {
                                    if (nSleepCount >= 10000) // Center Line 수신 Time Out : 10 sec.
                                    {
                                        m_ptrMainWindow.ProgressWindow.StopProgress(tempDesVision);
                                        this.Cursor = Cursors.Arrow;
                                        MessageBox.Show("Vision 정보를 전송하지 못하였습니다. (Code:Time Out)", "Error");
                                        MainWindow.Log("PCS", SeverityLevel.FATAL, "Vision 정보 전송 실패 (Center Line 수신 실패)", true);
                                        return false;
                                    }
                                    Thread.Sleep(10);
                                    nSleepCount++;
                                }
                                inspItem.LineSegments = desVision.LineSegments;
                                g.LineSegments = desVision.LineSegments;
                                g.RefreshDrawing();
                                desVision.LineSegments = null;
                            }
                            Thread.Sleep(5);
                        }
                        nRoiCode++;
                    }
                }
            }
            return true;
        }

        // 2012-08-06 suoow2 Added; 중앙선 데이터를 요청한다.
        private bool RequestBallInfo(int anDesVision)
        {
            IndexInfo IndexInfo = m_ptrMainWindow.Convert_ViewIndexToVisionIndex(anDesVision);
            int tempDesVision = IndexInfo.VisionIndex;
            int FrameGrabberIndex = VID.CalcIndex(tempDesVision);

            TeachingViewerCtrl viewer = TeachingViewers[anDesVision];
            VisionInterface desVision = m_ptrMainWindow.PCSInstance.Vision[tempDesVision];
            if (viewer == null || desVision == null) return false;

            int nRoiCode = 0;
            foreach (SectionInformation section in viewer.SectionManager.Sections)
            {
                for (int i = 0; i < 3; i++)
                {
                    if ((MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] < 6) && i == 1) break;
                    foreach (GraphicsBase g in section.ROICanvas[i].GraphicsList)  // ROI별 반복
                    {
                        if (g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine || g.RegionType == GraphicsRegionType.TapeLoaction)
                            continue;
                        if (g is GraphicsSelectionRectangle || g is GraphicsLine)
                            continue;

                        foreach (InspectionItem inspItem in g.InspectionList)
                        {
                            if (inspItem.InspectionType.InspectType == eVisInspectType.eInspTypeBallPattern)
                            {
                                if (inspItem.BallSegments != null)
                                    inspItem.BallSegments = null;

                                desVision.ReqBallInfo(inspItem.SentID.Value);
                                int nSleepCount = 0;
                                while (!desVision.Recv_Ball_Done)
                                {
                                    if (nSleepCount >= 10000) // Center Line 수신 Time Out : 10 sec.
                                    {
                                        m_ptrMainWindow.ProgressWindow.StopProgress(tempDesVision);
                                        this.Cursor = Cursors.Arrow;
                                        MessageBox.Show("Vision 정보를 전송하지 못하였습니다. (Code:Time Out)", "Error");
                                        MainWindow.Log("PCS", SeverityLevel.FATAL, "Vision 정보 전송 실패 (Center Line 수신 실패)", true);
                                        return false;
                                    }
                                    Thread.Sleep(10);
                                    nSleepCount++;
                                }
                                inspItem.BallSegments = desVision.BallSegments;
                                g.BallSegments = desVision.BallSegments;
                                g.RefreshDrawing();
                                desVision.BallSegments = null;
                            }
                            Thread.Sleep(5);
                        }
                        nRoiCode++;
                    }
                }
            }
            return true;
        }

        // 개별 Surface에 대한 비전 정보 전송
        private int SendVisionData(int anDesVision, bool bAuto, bool bRail)
        {
            int nSendedItemCount = 0;
            string szGroupName = MainWindow.CurrentGroup.Name;
            string szModelName = MainWindow.CurrentModel.Name;
            Point psrmargin = new Point(MainWindow.CurrentModel.Strip.PSRMarginX, MainWindow.CurrentModel.Strip.PSRMarginY);
            Point wpmargin = new Point(MainWindow.CurrentModel.Strip.WPMarginX, MainWindow.CurrentModel.Strip.WPMarginY);

            try
            {
                IndexInfo indexInfo = m_ptrMainWindow.Convert_ViewIndexToVisionIndex(anDesVision);
                int tempDesVision = indexInfo.VisionIndex;
                int FrameGrabberIndex = VID.CalcIndex(tempDesVision);
                SectionManager currSectionManager = TeachingViewers[anDesVision].SectionManager;
                #region 잘못된 Section 정보에 대한 처리.
                if (currSectionManager == null || currSectionManager.Sections.Count < 1) // Section이 존재하지 않는 경우.
                {
                    this.Cursor = Cursors.Arrow;
                    m_ptrMainWindow.ProgressWindow.StopProgress(tempDesVision);

                    string str = "";
                    if (indexInfo.CategorySurface == CategorySurface.BP)
                        str = "[본드패드" + (indexInfo.Index + 1) + "]에 생성된 섹션이 없습니다.\n섹션 설정을 진행해 주시기 바랍니다.";
                    else if (indexInfo.CategorySurface == CategorySurface.CA)
                        str = "[CA외관" + (indexInfo.Index + 1) + "]에 생성된 섹션이 없습니다.\n섹션 설정을 진행해 주시기 바랍니다.";
                    else
                        str = "[BA외관" + (indexInfo.Index + 1) + "]에 생성된 섹션이 없습니다.\n섹션 설정을 진행해 주시기 바랍니다.";
                    MessageBox.Show(str, "Information");
                    return -1;
                }

                foreach (SectionInformation section in currSectionManager.Sections)
                {
                    string szSectionImagePath = string.Empty;
                    if (!IsOnLine && MtsManager.SelectedMachine != null)
                    {
                        szSectionImagePath = String.Format("{0}/{1}1-{2}.bmp", DirectoryManager.GetOfflineModelImagePath(MainWindow.Setting.General.ModelPath, MtsManager.SelectedMachine.Name, szGroupName, szModelName), anDesVision + 1, section.Name);
                    }
                    else
                    {
                        string str = "";
                        if (TeachingViewers[anDesVision].RGB) str = "{0}/{1}-{2}-R.bmp";
                        else str = "{0}/{1}-{2}.bmp";

                        int surfaceindex;
                        if (indexInfo.CategorySurface == CategorySurface.BP && indexInfo.Slave == true)//// BP Slave 경우 BP1의 Section을 따라하기 때문에 같은 값을 쓰자
                            surfaceindex = (int)indexInfo.Surface - indexInfo.Index;
                        else surfaceindex = (int)indexInfo.Surface;
                        szSectionImagePath = String.Format(str, DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName), surfaceindex, section.Name);
                    }

                    if (!File.Exists(szSectionImagePath)) // 기준 영상이 존재하지 않는 경우.
                    {
                        this.Cursor = Cursors.Arrow;
                        m_ptrMainWindow.ProgressWindow.StopProgress(tempDesVision);string str = "";
                        if (indexInfo.CategorySurface == CategorySurface.BP)
                        {
                            str = "[본드패드" + (indexInfo.Index + 1) + "]의 기준 영상이 등록되어 있지 않습니다.\n기준 영상을 등록해 주시기 바랍니다.";
                        }
                        else if (indexInfo.CategorySurface == CategorySurface.CA)
                        {
                            str = "[CA외관" + (indexInfo.Index + 1) + "]의 기준 영상이 등록되어 있지 않습니다.\n기준 영상을 등록해 주시기 바랍니다.";
                        }
                        else
                        {
                            str = "[BA외관" + (indexInfo.Index + 1) + "]의 기준 영상이 등록되어 있지 않습니다.\n기준 영상을 등록해 주시기 바랍니다.";
                        }
                        MessageBox.Show(str, "Information");
                        return -1;
                    }
                    else // 기준 영상이 존재하는 경우.
                    {
                        section.SetBitmapSource(BitmapImageLoader.LoadCachedBitmapImage(new Uri(szSectionImagePath, UriKind.RelativeOrAbsolute)) as BitmapSource, 0);
                        section.Image[0].Refresh();
                    }
                }
                #endregion

                VisionInterface desVision = m_ptrMainWindow.PCSInstance.Vision[tempDesVision];
                desVision.Set_UseAI(MainWindow.CurrentModel.UseAI);
                if (!desVision.Connected)
                {
                    this.Cursor = Cursors.Arrow;
                    m_ptrMainWindow.ProgressWindow.StopProgress(anDesVision);
                    return -1;
                }
                      
                desVision.m_iTrainError = 0;
                desVision.Register_Done = false;
                if(bAuto)//자동일때는 각 PC별 첫번째에 모두 클리어 하기 위한 시퀀스 비전에 맞춰주기 위함
                {
                    if (indexInfo.SurfaceScanCount == 0)//각 PC의 두번째 스캔부터는 초기화하면 안됨 비전에 맞춰주기 위함
                    {
                        for (int Refeat = 0; Refeat < indexInfo.SurfaceTotalCount; Refeat++)
                        {
                            desVision.SendGrabSide(Refeat);
                            desVision.SendPacket(VisionDefinition.CLEAR_INSP_ITEMS);
                        }
                    }
                    desVision.SendGrabSide(indexInfo.SurfaceScanCount);
                }
                else//수동일때는 선택된 뷰에 따라 날림 
                {
                    desVision.SendGrabSide(indexInfo.SurfaceScanCount);
                    desVision.SendPacket(VisionDefinition.CLEAR_INSP_ITEMS);
                }
  
                desVision.SendPacket(VisionDefinition.CLEAR_INSP_RESULTS);
                desVision.DefectCount = 0;
                desVision.SendVerifyInfo(MainWindow.CurrentModel.UseVerify ? 1 : 0, MainWindow.Setting.General.VRSNGUnitLimit);


                string szBasedImagePath = string.Empty;
                if (!IsOnLine && MtsManager.SelectedMachine != null)
                {
                    szBasedImagePath = DirectoryManager.GetOfflineBasedImagePath(MainWindow.Setting.General.ModelPath, MtsManager.SelectedMachine.Name, szGroupName, szModelName, TeachingViewers[anDesVision].SurfaceType);
                }
                else
                {
                    szBasedImagePath = DirectoryManager.GetBasedImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName, TeachingViewers[anDesVision].SurfaceType);
                }
                //BitmapSource basedImageSource = null;
                if (File.Exists(szBasedImagePath))
                {
                    // Code 채번 Rule.
                    // 섹션 : WorkType별 0번부터 채번.
                    // ROI : WorkType별 0번부터 채번 (섹션 구분 없음)
                    // 검사설정 : WorkType별 0번부터 채번 (섹션, ROI 구분 없음)
                    int nSecCode = 0;
                    int nRoiCode = 0;
                    int nInspCode = 0;
                    bool bExceptionSkip;
                    lock (this)
                    {
                        #region 2. Send Section Data.
                        foreach (SectionInformation section in TeachingViewers[anDesVision].SectionManager.Sections)
                        {
                            section.Code = QueryHelper.GetCode(nSecCode++, 4);
                            if (bAuto && !bRail)
                            {
                                if (section.Type.Code == SectionTypeCode.OUTER_REGION)
                                    continue;
                            }
                            if (section.BitmapSource != null)
                            {
                                if (section.Type.Code == SectionTypeCode.OUTER_REGION)
                                {
                                    if (Math.Abs((section.BitmapSource[0].PixelWidth) - section.Region.Width) != 0 ||
                                        Math.Abs((section.BitmapSource[0].PixelHeight) - section.Region.Height) != 0)
                                    {
                                        section.Region = new Int32Rect(section.Region.X, section.Region.Y, section.BitmapSource[0].PixelWidth, section.BitmapSource[0].PixelHeight);
                                    }
                                }
                                else
                                {
                                    if (Math.Abs((section.BitmapSource[0].PixelWidth * VisionDefinition.GRAB_IMAGE_SCALE) - section.Region.Width) != 0 ||
                                        Math.Abs((section.BitmapSource[0].PixelHeight * VisionDefinition.GRAB_IMAGE_SCALE) - section.Region.Height) != 0)
                                    {
                                        section.Region = new Int32Rect(section.Region.X, section.Region.Y, (int)(section.BitmapSource[0].PixelWidth * VisionDefinition.GRAB_IMAGE_SCALE), (int)(section.BitmapSource[0].PixelHeight * VisionDefinition.GRAB_IMAGE_SCALE));
                                    }
                                }
                            }

                            if (indexInfo.CategorySurface == CategorySurface.BP && indexInfo.Slave)////BP, Slave 일때
                                section.BondPadSide = 1;
                            else
                                section.BondPadSide = 0;

                            desVision.AddSection(section);
                            Thread.Sleep(5);
                        }
                        #region 2. Send Section Left Top, Left Bottom ICS Data.
                        if (!(indexInfo.CategorySurface == CategorySurface.BP && indexInfo.Slave))
                        {
                            foreach (SectionInformation section in TeachingViewers[anDesVision].SectionManager.Sections)
                            {
                                if (section.Type.Code == SectionTypeCode.UNIT_REGION || section.Type.Code == SectionTypeCode.PSR_REGION)
                                {
                                    List<System.Windows.Point> UnitPos = new List<System.Windows.Point>();
                                    foreach (SectionRegion region in section.SectionRegionList)
                                    {
                                        if ((region.RegionIndex.X == 0 && region.RegionIndex.Y == 0) || (region.RegionIndex.X == 0 && region.RegionIndex.Y == section.IterationCountY - 1))
                                        {
                                            UnitPos.Add(new System.Windows.Point(region.RegionPosition.X, region.RegionPosition.Y));
                                        }
                                    }
                                    desVision.SendtoICS_UnitPos(UnitPos, indexInfo.SurfaceScanCount);
                                    Thread.Sleep(5);
                                }
                            }
                        }
                        #endregion
                        m_ptrMainWindow.ProgressWindow.SetProgressValue(anDesVision, 40, "섹션 정보");
                        #endregion

                        #region 3. Send Reference Image.
                        int tmpcode = 0;
                        foreach (SectionInformation section in currSectionManager.Sections)
                        {
                            if (bAuto && !bRail)
                            {
                                if (section.Type.Code == SectionTypeCode.OUTER_REGION)
                                    continue;
                            }
                            for (int i = 0; i < 3; i++)
                            {
                                //if (indexInfo.CategorySurface == CategorySurface.BP && indexInfo.Slave) continue;////BP, Slave 일때
                                if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] < 6)
                                {
                                    if (i == 1)
                                        break;
                                    if (section.BitmapSource[i] != null)
                                    {
                                        m_ptrMainWindow.PCSInstance.Vision[tempDesVision].SendImageData(VisionDefinition.SEND_REF_IMAGE, section.IntCode, section.BitmapSource[i], -1, -1, i);
                                        tmpcode = section.IntCode;
                                        Thread.Sleep(5);
                                    }
                                }
                                else
                                {

                                    //Saturation, Mask
                                    if (section.BitmapSource[i] != null)
                                    {
                                        if (section.Type.Code == SectionTypeCode.PSR_REGION)
                                        {

                                            switch(i)
                                            {
                                                case 0:
                                                    m_ptrMainWindow.PCSInstance.Vision[tempDesVision].SendImageData(VisionDefinition.SEND_REF_IMAGE, section.IntCode, section.BitmapSource[i], -1, -1, PSR_Saturation_Index);
                                                    break;
                                                case 1:
                                                    m_ptrMainWindow.PCSInstance.Vision[tempDesVision].SendImageData(VisionDefinition.SEND_REF_IMAGE_COLOR, section.IntCode, section.BitmapSource[i], -1, -1, PSR_Color_Index);
                                                    break;
                                                case 2:
                                                    m_ptrMainWindow.PCSInstance.Vision[tempDesVision].SendImageData(VisionDefinition.SEND_REF_IMAGE, section.IntCode, section.BitmapSource[i], -1, -1, PSR_Blending_Index);
                                                    break;
                                            }

                                        }
                                        else
                                            m_ptrMainWindow.PCSInstance.Vision[tempDesVision].SendImageData(VisionDefinition.SEND_REF_IMAGE, section.IntCode, section.BitmapSource[i], -1, -1, i);
                                   
                                        tmpcode = section.IntCode;
                                        Thread.Sleep(5);
                                    }
                                }

                            }
                        }
                        m_ptrMainWindow.ProgressWindow.SetProgressValue(anDesVision, 60, "기준 영상");
                        #endregion

                        #region 4. Send ROI Data.

                        nRoiCode = 0;
                        if (indexInfo.CategorySurface == CategorySurface.BP && indexInfo.Slave == true)//BondPad2
                        {
                            foreach (SectionInformation section in TeachingViewers[indexInfo.SurfaceBeginIndex].SectionManager.Sections)
                            {
                                if (section.Type.Code == SectionTypeCode.UNIT_REGION)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] < 6)
                                            if (i == 1)
                                                break;
                                        foreach (GraphicsBase g in section.ROICanvas[i].GraphicsList)
                                        {
                                            if (g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine || g.RegionType == GraphicsRegionType.TapeLoaction)
                                                continue;
                                            if (g is GraphicsSelectionRectangle || g is GraphicsLine || g is GraphicsSkeletonLine || g is GraphicsSkeletonBall)
                                                continue;

                                            if (section.ROICanvas[i].CanDraw(g))
                                            {
                                                g.SentID = nRoiCode;
                                                desVision.AddROI(ROIParser.GraphicsToIS(nRoiCode++, 0, g), 0);
                                                Thread.Sleep(5);
                                            }
                                            else
                                            {
                                                MessageBox.Show(string.Format("섹션을 벗어나는 검사 영역이 존재합니다.\n섹션명:{0}", section.Name));
                                                return -1;
                                            }
                                        }                                     
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (SectionInformation section in currSectionManager.Sections)
                            {
                                if (bAuto && !bRail)
                                {
                                    if (section.Type.Code == SectionTypeCode.OUTER_REGION)
                                        continue;
                                }
                                for (int i = 0; i < 3; i++)
                                {
                                    if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] < 6)
                                        if (i == 1)
                                            break;
                                    foreach (GraphicsBase g in section.ROICanvas[i].GraphicsList)  // ROI별 반복
                                    {
                                        if (g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine || g.RegionType == GraphicsRegionType.TapeLoaction)
                                            continue;
                                        if (g is GraphicsSelectionRectangle || g is GraphicsLine || g is GraphicsSkeletonLine || g is GraphicsSkeletonBall)
                                            continue;

                                        if (section.ROICanvas[i].CanDraw(g))
                                        {
                                            g.SentID = nRoiCode;
                                            desVision.AddROI(ROIParser.GraphicsToIS(nRoiCode++, section.IntCode, g), i);
                                            Thread.Sleep(5);
                                        }
                                        else
                                        {
                                            MessageBox.Show(string.Format("섹션을 벗어나는 검사 영역이 존재합니다.\n섹션명:{0}", section.Name));
                                            return -1;
                                        }
                                    }
                                }
                            }
                        }

                        m_ptrMainWindow.ProgressWindow.SetProgressValue(anDesVision, 80, "검사 영역");
                        #endregion

                        #region 5. Send Inspection Item.

                        double resolutionX = 0;

                        if (indexInfo.CategorySurface == CategorySurface.BP)
                            resolutionX = MainWindow.Setting.SubSystem.IS.CamResolutionX[0];
                        else if (indexInfo.CategorySurface == CategorySurface.CA)
                            resolutionX = MainWindow.Setting.SubSystem.IS.CamResolutionX[1];
                        else if (indexInfo.CategorySurface == CategorySurface.BA)
                            resolutionX = MainWindow.Setting.SubSystem.IS.CamResolutionX[2];

                        nRoiCode = 0;
                        nInspCode = 0;
                        if (indexInfo.CategorySurface == CategorySurface.BP && indexInfo.Slave == true)
                        {
                            foreach (SectionInformation section in TeachingViewers[indexInfo.SurfaceBeginIndex].SectionManager.Sections)
                            {
                                if (section.Type.Code == SectionTypeCode.UNIT_REGION)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] < 6)
                                            if (i == 1)
                                                break;
                                        bExceptionSkip = false;
                                        foreach (GraphicsBase g in section.ROICanvas[i].GraphicsList)  // ROI별 반복
                                        {
                                            if (g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine || g.RegionType == GraphicsRegionType.TapeLoaction)
                                                continue;
                                            if (g is GraphicsSelectionRectangle || g is GraphicsLine)
                                                continue;

                                            foreach (InspectionItem inspectionElement in g.InspectionList)
                                            {
                                                if (inspectionElement.InspectionType.InspectAlgorithm == eVisInspectAlgo.eInspAlgoExceptionalMask)
                                                {
                                                    bExceptionSkip = true;
                                                    break;
                                                }
                                            }
                                            foreach (InspectionItem inspectionElement in g.InspectionList)
                                            {
                                                inspectionElement.IsExceptionSkip = (bExceptionSkip) ? 1 : 0;
                                                inspectionElement.SentID = nInspCode;
                                                desVision.SendPacket(VisionDefinition.ADD_INSP, VisionParameter.InspSize, VisionParameter.InspItemToBytes(anDesVision, nInspCode++, 0, nRoiCode, inspectionElement, resolutionX, psrmargin, wpmargin, i));
                                                
                                                
                                                Result_Convert ret = new Result_Convert();
                                                ret.Target_Viewer = anDesVision;
                                                ret.SectionID = 0;  // section.IntCode; //BP2 의 경우 Unit Section 1개만 생성하기 때문에 복사해온 BP1에 SectionID를 사용하지 않고 SectionID  0으로 고정
                                                ret.RoiID = nRoiCode;
                                                ret.InspID = nInspCode - 1;
                                                ret.Channel = i;
                                                ret.Name = inspectionElement.InspectionType.Name;
                                                ret.InspectType = (int)inspectionElement.InspectionType.InspectType;
                                                switch (inspectionElement.InspectionType.InspectType)
                                                {
                                                    case eVisInspectType.eInspTypeUnitAlign: // Unit Align
                                                        ret.LowerThresh = 0;
                                                        ret.UpperThresh = 0;
                                                        ret.Mode = -1;
                                                        break;
                                                    case eVisInspectType.eInspTypeSurface: // 표면 검사
                                                    case eVisInspectType.eInspTypeSpace: // 공간 검사
                                                    case eVisInspectType.eInspTypePSR: // 공간 검사
                                                    case eVisInspectType.eInspTypePunchBurr:
                                                    case eVisInspectType.eInspTypeDownSet:
                                                        StateAlignedMaskProperty stateAlignedProperty = inspectionElement.InspectionAlgorithm as StateAlignedMaskProperty;
                                                        if (stateAlignedProperty != null)
                                                        {
                                                            ret.Mode = stateAlignedProperty.ThreshType;
                                                            ret.LowerThresh = stateAlignedProperty.LowerThresh;
                                                            ret.UpperThresh = stateAlignedProperty.UpperThresh;
                                                        }
                                                        else
                                                        {
                                                            ret.Mode = -1;
                                                            ret.LowerThresh = 0;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        break;

                                                    case eVisInspectType.eInspTypeDamBar: // Ball 검사
                                                        DamSizeProperty damProperty = inspectionElement.InspectionAlgorithm as DamSizeProperty;
                                                        if (damProperty != null)
                                                        {
                                                            ret.Mode = damProperty.ThreshType;
                                                            ret.LowerThresh = damProperty.LowerThresh;
                                                            ret.UpperThresh = damProperty.UpperThresh;
                                                        }
                                                        else
                                                        {
                                                            ret.Mode = -1;
                                                            ret.LowerThresh = 0;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        break;

                                                    case eVisInspectType.eInspTypeBallPattern: // Ball 검사
                                                        BallPatternProperty ballProperty = inspectionElement.InspectionAlgorithm as BallPatternProperty;
                                                        if (ballProperty != null)
                                                        {
                                                            ret.Mode = ballProperty.ThreshType;
                                                            ret.LowerThresh = ballProperty.LowerThresh;
                                                            ret.UpperThresh = ballProperty.UpperThresh;
                                                        }
                                                        else
                                                        {
                                                            ret.Mode = -1;
                                                            ret.LowerThresh = 0;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        break;

                                                    case eVisInspectType.eInspTypeOuter: // 외곽 검사
                                                        OuterProperty outerProperty = inspectionElement.InspectionAlgorithm as OuterProperty;
                                                        if (outerProperty != null)
                                                        {
                                                            ret.Mode = 2;
                                                            ret.LowerThresh = outerProperty.LowerThresh;
                                                            ret.UpperThresh = outerProperty.UpperThresh;
                                                        }
                                                        else
                                                        {
                                                            ret.Mode = -1;
                                                            ret.LowerThresh = 0;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        break;

                                                    case eVisInspectType.eInspTypeUnitFidu: // 인식키 검사
                                                        CrossPatternProperty crossPatternProperty = inspectionElement.InspectionAlgorithm as CrossPatternProperty;
                                                        if (crossPatternProperty != null)
                                                        {
                                                            ret.Mode = crossPatternProperty.ThreshType;
                                                            ret.LowerThresh = crossPatternProperty.LowerThresh;
                                                            ret.UpperThresh = crossPatternProperty.UpperThresh;
                                                        }
                                                        else
                                                        {
                                                            ret.Mode = -1;
                                                            ret.LowerThresh = 0;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        break;

                                                    case eVisInspectType.eInspTypeUnitPattern: // 
                                                        UnitPatternProperty unitPatternProperty = inspectionElement.InspectionAlgorithm as UnitPatternProperty;
                                                        if (unitPatternProperty != null)
                                                        {
                                                            ret.Mode = 0;
                                                            ret.LowerThresh = unitPatternProperty.LowerThresh;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        else
                                                        {
                                                            ret.Mode = -1;
                                                            ret.LowerThresh = 0;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        break;

                                                    case eVisInspectType.eInspTypeVentHole: // VentHole 검사
                                                        VentHoleProperty ventHoleProperty = inspectionElement.InspectionAlgorithm as VentHoleProperty;
                                                        if (ventHoleProperty != null)
                                                        {
                                                            ret.Mode = 0;
                                                            ret.LowerThresh = ventHoleProperty.LowerThresh;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        else
                                                        {
                                                            ret.Mode = -1;
                                                            ret.LowerThresh = 0;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        break;


                                                    case eVisInspectType.eInspTypeNoResizeVentHole: // NoReSize Vent Hole 검사
                                                        VentHoleProperty2 ventHoleProperty2 = inspectionElement.InspectionAlgorithm as VentHoleProperty2;
                                                        if (ventHoleProperty2 != null)
                                                        {
                                                            ret.Mode = 0;
                                                            ret.LowerThresh = ventHoleProperty2.LowerThresh;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        else
                                                        {
                                                            ret.Mode = -1;
                                                            ret.LowerThresh = 0;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        break;

                                                    case eVisInspectType.eInspTypeCoreCrack: // WindowPunch 검사
                                                        WindowPunchProperty windowPunchProperty = inspectionElement.InspectionAlgorithm as WindowPunchProperty;
                                                        if (windowPunchProperty != null)
                                                        {
                                                            ret.Mode = windowPunchProperty.ThreshType;
                                                            ret.LowerThresh = windowPunchProperty.LowerThresh;
                                                            ret.UpperThresh = windowPunchProperty.UpperThresh;
                                                        }
                                                        else
                                                        {
                                                            ret.Mode = -1;
                                                            ret.LowerThresh = 0;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        break;

                                                    case eVisInspectType.eInspTypePlate: // 도금 검사
                                                    case eVisInspectType.eInspTypeTape: // Tape 검사
                                                        ShapeShiftProperty shapeShiftProperty = inspectionElement.InspectionAlgorithm as ShapeShiftProperty;
                                                        if (shapeShiftProperty != null)
                                                        {
                                                            ret.Mode = shapeShiftProperty.ThreshType;
                                                            ret.LowerThresh = shapeShiftProperty.LowerThresh;
                                                            ret.UpperThresh = shapeShiftProperty.UpperThresh;
                                                        }
                                                        else
                                                        {
                                                            ret.Mode = -1;
                                                            ret.LowerThresh = 0;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        break;
                                                    case eVisInspectType.eInspTypeRailIntensity: // Downset 검사
                                                        StateIntensityProperty railProperty = inspectionElement.InspectionAlgorithm as StateIntensityProperty;
                                                        if (railProperty != null)
                                                        {
                                                            ret.Mode = railProperty.ThreshType;
                                                            ret.LowerThresh = railProperty.LowerThresh;
                                                            ret.UpperThresh = railProperty.UpperThresh;
                                                        }
                                                        else
                                                        {
                                                            ret.Mode = -1;
                                                            ret.LowerThresh = 0;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        break;
                                                    case eVisInspectType.eInspTypeLeadShapeWithCL: // 리드 형상 검사
                                                        LeadShapeWithCenterLineProperty leadShapeProperty = inspectionElement.InspectionAlgorithm as LeadShapeWithCenterLineProperty;
                                                        if (leadShapeProperty != null)
                                                        {
                                                            ret.Mode = leadShapeProperty.ThreshType;
                                                            ret.LowerThresh = leadShapeProperty.LowerThresh;
                                                            ret.UpperThresh = leadShapeProperty.UpperThresh;
                                                        }
                                                        else
                                                        {
                                                            ret.Mode = -1;
                                                            ret.LowerThresh = 0;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        break;
                                                    case eVisInspectType.eInspTypeLeadGap: // 리드 간격 검사
                                                        LeadGapProperty leadGapProperty = inspectionElement.InspectionAlgorithm as LeadGapProperty;
                                                        if (leadGapProperty != null)
                                                        {
                                                            ret.Mode = 2;
                                                            ret.LowerThresh = leadGapProperty.LowerThresh;
                                                            ret.UpperThresh = leadGapProperty.UpperThresh;
                                                        }
                                                        else
                                                        {
                                                            ret.Mode = -1;
                                                            ret.LowerThresh = 0;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        break;
                                                    case eVisInspectType.eInspTypeSpaceShapeWithCL: // 공간 형상 검사
                                                        SpaceShapeWithCenterLineProperty spaceShapeProperty = inspectionElement.InspectionAlgorithm as SpaceShapeWithCenterLineProperty;
                                                        if (spaceShapeProperty != null)
                                                        {
                                                            ret.Mode = spaceShapeProperty.ThreshType;
                                                            ret.LowerThresh = spaceShapeProperty.LowerThresh;
                                                            ret.UpperThresh = spaceShapeProperty.UpperThresh;
                                                        }
                                                        else
                                                        {
                                                            ret.Mode = -1;
                                                            ret.LowerThresh = 0;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        break;
                                                    case eVisInspectType.eInspTypeUnitRawMaterial:          // 테이프 검사
                                                        RawMetrialProperty rawmetrialProperty = inspectionElement.InspectionAlgorithm as RawMetrialProperty;
                                                        if (rawmetrialProperty != null)
                                                        {
                                                            ret.Mode = rawmetrialProperty.ThreshType;
                                                            ret.LowerThresh = rawmetrialProperty.LowerThresh;
                                                            ret.UpperThresh = rawmetrialProperty.UpperThresh;
                                                        }
                                                        else
                                                        {
                                                            ret.Mode = -1;
                                                            ret.LowerThresh = 0;
                                                            ret.UpperThresh = 0;
                                                        }
                                                        break;
                                                    case eVisInspectType.eInspTypePSROdd:
                                                        PSROddProperty psrOddProperty = inspectionElement.InspectionAlgorithm as PSROddProperty;
                                                        if (psrOddProperty != null)
                                                        {
                                                            ret.Mode = 2;
                                                            ret.LowerThresh = psrOddProperty.Metal_LowerThreshold;
                                                            ret.UpperThresh = psrOddProperty.Metal_UpperThreshold;
                                                        }
                                                        break;
                                                    case eVisInspectType.eInspResultGV:
                                                        GV_Inspection_Property gv_Inspection_Property = inspectionElement.InspectionAlgorithm as GV_Inspection_Property;
                                                        if (gv_Inspection_Property != null)
                                                        {
                                                            ret.Mode = 2;
                                                            ret.LowerThresh = gv_Inspection_Property.Ball_Thresh;
                                                            ret.UpperThresh = gv_Inspection_Property.Core_Thresh;
                                                        }
                                                        break;
                                                }
                                                MainWindow.lstRC.Add(ret);
                                                Thread.Sleep(5);
                                            }
                                            nSendedItemCount++;
                                            nRoiCode++;
                                        }
                                       
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (SectionInformation section in currSectionManager.Sections)
                            {
                                if (bAuto && !bRail)
                                {
                                    if (section.Type.Code == SectionTypeCode.OUTER_REGION)
                                        continue;
                                }
                                bExceptionSkip = false;
                                for (int i = 0; i < 3; i++)
                                {
                                    if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] < 6)
                                        if (i == 1)
                                            break;
                                    foreach (GraphicsBase g in section.ROICanvas[i].GraphicsList)  // ROI별 반복
                                    {
                                        if (g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine || g.RegionType == GraphicsRegionType.TapeLoaction)
                                            continue;
                                        if (g is GraphicsSelectionRectangle || g is GraphicsLine)
                                            continue;

                                        foreach (InspectionItem inspectionElement in g.InspectionList)
                                        {
                                            if (inspectionElement.InspectionType.InspectAlgorithm == eVisInspectAlgo.eInspAlgoExceptionalMask)
                                            {
                                                bExceptionSkip = true;
                                                break;
                                            }
                                        }
                                        foreach (InspectionItem inspectionElement in g.InspectionList)
                                        {
                                            inspectionElement.IsExceptionSkip = (bExceptionSkip) ? 1 : 0;
                                            inspectionElement.SentID = nInspCode;
                                            desVision.SendPacket(VisionDefinition.ADD_INSP, VisionParameter.InspSize, VisionParameter.InspItemToBytes(anDesVision, nInspCode++, section.IntCode, nRoiCode, inspectionElement, resolutionX, psrmargin, wpmargin, i));
                                            Result_Convert ret = new Result_Convert();
                                            ret.Target_Viewer = anDesVision; // anDesVision > 1 ? anDesVision - 1 : anDesVision;
                                            ret.SectionID = section.IntCode;
                                            ret.RoiID = nRoiCode;
                                            ret.InspID = nInspCode - 1;
                                            ret.Channel = i;
                                            ret.Name = inspectionElement.InspectionType.Name;
                                            ret.InspectType = (int)inspectionElement.InspectionType.InspectType;
                                            switch (inspectionElement.InspectionType.InspectType)
                                            {
                                                case eVisInspectType.eInspTypeUnitAlign: // Unit Align
                                                    ret.LowerThresh = 0;
                                                    ret.UpperThresh = 0;
                                                    ret.Mode = -1;
                                                    break;
                                                case eVisInspectType.eInspTypeSurface: // 표면 검사
                                                case eVisInspectType.eInspTypeSpace: // 공간 검사
                                                case eVisInspectType.eInspTypePSR: // 공간 검사
                                                case eVisInspectType.eInspTypePunchBurr:
                                                case eVisInspectType.eInspTypeDownSet:
                                                    StateAlignedMaskProperty stateAlignedProperty = inspectionElement.InspectionAlgorithm as StateAlignedMaskProperty;
                                                    if (stateAlignedProperty != null)
                                                    {
                                                        ret.Mode = stateAlignedProperty.ThreshType;
                                                        ret.LowerThresh = stateAlignedProperty.LowerThresh;
                                                        ret.UpperThresh = stateAlignedProperty.UpperThresh;
                                                    }
                                                    else
                                                    {
                                                        ret.Mode = -1;
                                                        ret.LowerThresh = 0;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    break;

                                                case eVisInspectType.eInspTypeDamBar: // Ball 검사
                                                    DamSizeProperty damProperty = inspectionElement.InspectionAlgorithm as DamSizeProperty;
                                                    if (damProperty != null)
                                                    {
                                                        ret.Mode = damProperty.ThreshType;
                                                        ret.LowerThresh = damProperty.LowerThresh;
                                                        ret.UpperThresh = damProperty.UpperThresh;
                                                    }
                                                    else
                                                    {
                                                        ret.Mode = -1;
                                                        ret.LowerThresh = 0;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    break;

                                                case eVisInspectType.eInspTypeBallPattern: // Ball 검사
                                                    BallPatternProperty ballProperty = inspectionElement.InspectionAlgorithm as BallPatternProperty;
                                                    if (ballProperty != null)
                                                    {
                                                        ret.Mode = ballProperty.ThreshType;
                                                        ret.LowerThresh = ballProperty.LowerThresh;
                                                        ret.UpperThresh = ballProperty.UpperThresh;
                                                    }
                                                    else
                                                    {
                                                        ret.Mode = -1;
                                                        ret.LowerThresh = 0;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    break;

                                                case eVisInspectType.eInspTypeOuter: // 외곽 검사
                                                    OuterProperty outerProperty = inspectionElement.InspectionAlgorithm as OuterProperty;
                                                    if (outerProperty != null)
                                                    {
                                                        ret.Mode = 2;
                                                        ret.LowerThresh = outerProperty.LowerThresh;
                                                        ret.UpperThresh = outerProperty.UpperThresh;
                                                    }
                                                    else
                                                    {
                                                        ret.Mode = -1;
                                                        ret.LowerThresh = 0;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    break;

                                                case eVisInspectType.eInspTypeUnitFidu: // 인식키 검사
                                                    CrossPatternProperty crossPatternProperty = inspectionElement.InspectionAlgorithm as CrossPatternProperty;
                                                    if (crossPatternProperty != null)
                                                    {
                                                        ret.Mode = crossPatternProperty.ThreshType;
                                                        ret.LowerThresh = crossPatternProperty.LowerThresh;
                                                        ret.UpperThresh = crossPatternProperty.UpperThresh;
                                                    }
                                                    else
                                                    {
                                                        ret.Mode = -1;
                                                        ret.LowerThresh = 0;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    break;

                                                case eVisInspectType.eInspTypeUnitPattern: // 
                                                    UnitPatternProperty unitPatternProperty = inspectionElement.InspectionAlgorithm as UnitPatternProperty;
                                                    if (unitPatternProperty != null)
                                                    {
                                                        ret.Mode = 0;
                                                        ret.LowerThresh = unitPatternProperty.LowerThresh;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    else
                                                    {
                                                        ret.Mode = -1;
                                                        ret.LowerThresh = 0;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    break;

                                                case eVisInspectType.eInspTypeVentHole: // VentHole 검사
                                                    VentHoleProperty ventHoleProperty = inspectionElement.InspectionAlgorithm as VentHoleProperty;
                                                    if (ventHoleProperty != null)
                                                    {
                                                        ret.Mode = 0;
                                                        ret.LowerThresh = ventHoleProperty.LowerThresh;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    else
                                                    {
                                                        ret.Mode = -1;
                                                        ret.LowerThresh = 0;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    break;

                                                case eVisInspectType.eInspTypeNoResizeVentHole: // NoReSize Vent Hole 검사
                                                    VentHoleProperty2 ventHoleProperty2 = inspectionElement.InspectionAlgorithm as VentHoleProperty2;
                                                    if (ventHoleProperty2 != null)
                                                    {
                                                        ret.Mode = 0;
                                                        ret.LowerThresh = ventHoleProperty2.LowerThresh;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    else
                                                    {
                                                        ret.Mode = -1;
                                                        ret.LowerThresh = 0;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    break;

                                                case eVisInspectType.eInspTypeCoreCrack: // WindowPunch 검사
                                                    WindowPunchProperty windowPunchProperty = inspectionElement.InspectionAlgorithm as WindowPunchProperty;
                                                    if (windowPunchProperty != null)
                                                    {
                                                        ret.Mode = windowPunchProperty.ThreshType;
                                                        ret.LowerThresh = windowPunchProperty.LowerThresh;
                                                        ret.UpperThresh = windowPunchProperty.UpperThresh;
                                                    }
                                                    else
                                                    {
                                                        ret.Mode = -1;
                                                        ret.LowerThresh = 0;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    break;

                                                case eVisInspectType.eInspTypePlate: // 도금 검사
                                                case eVisInspectType.eInspTypeTape: // Tape 검사
                                                    ShapeShiftProperty shapeShiftProperty = inspectionElement.InspectionAlgorithm as ShapeShiftProperty;
                                                    if (shapeShiftProperty != null)
                                                    {
                                                        ret.Mode = shapeShiftProperty.ThreshType;
                                                        ret.LowerThresh = shapeShiftProperty.LowerThresh;
                                                        ret.UpperThresh = shapeShiftProperty.UpperThresh;
                                                    }
                                                    else
                                                    {
                                                        ret.Mode = -1;
                                                        ret.LowerThresh = 0;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    break;
                                                case eVisInspectType.eInspTypeRailIntensity: // Downset 검사
                                                    StateIntensityProperty railProperty = inspectionElement.InspectionAlgorithm as StateIntensityProperty;
                                                    if (railProperty != null)
                                                    {
                                                        ret.Mode = railProperty.ThreshType;
                                                        ret.LowerThresh = railProperty.LowerThresh;
                                                        ret.UpperThresh = railProperty.UpperThresh;
                                                    }
                                                    else
                                                    {
                                                        ret.Mode = -1;
                                                        ret.LowerThresh = 0;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    break;
                                                case eVisInspectType.eInspTypeLeadShapeWithCL: // 리드 형상 검사
                                                    LeadShapeWithCenterLineProperty leadShapeProperty = inspectionElement.InspectionAlgorithm as LeadShapeWithCenterLineProperty;
                                                    if (leadShapeProperty != null)
                                                    {
                                                        ret.Mode = leadShapeProperty.ThreshType;
                                                        ret.LowerThresh = leadShapeProperty.LowerThresh;
                                                        ret.UpperThresh = leadShapeProperty.UpperThresh;
                                                    }
                                                    else
                                                    {
                                                        ret.Mode = -1;
                                                        ret.LowerThresh = 0;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    break;
                                                case eVisInspectType.eInspTypeLeadGap: // 리드 간격 검사
                                                    LeadGapProperty leadGapProperty = inspectionElement.InspectionAlgorithm as LeadGapProperty;
                                                    if (leadGapProperty != null)
                                                    {
                                                        ret.Mode = 2;
                                                        ret.LowerThresh = leadGapProperty.LowerThresh;
                                                        ret.UpperThresh = leadGapProperty.UpperThresh;
                                                    }
                                                    else
                                                    {
                                                        ret.Mode = -1;
                                                        ret.LowerThresh = 0;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    break;
                                                case eVisInspectType.eInspTypeSpaceShapeWithCL: // 공간 형상 검사
                                                    SpaceShapeWithCenterLineProperty spaceShapeProperty = inspectionElement.InspectionAlgorithm as SpaceShapeWithCenterLineProperty;
                                                    if (spaceShapeProperty != null)
                                                    {
                                                        ret.Mode = spaceShapeProperty.ThreshType;
                                                        ret.LowerThresh = spaceShapeProperty.LowerThresh;
                                                        ret.UpperThresh = spaceShapeProperty.UpperThresh;
                                                    }
                                                    else
                                                    {
                                                        ret.Mode = -1;
                                                        ret.LowerThresh = 0;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    break;
                                                case eVisInspectType.eInspTypeUnitRawMaterial:          // 테이프 검사
                                                    RawMetrialProperty rawmetrialProperty = inspectionElement.InspectionAlgorithm as RawMetrialProperty;
                                                    if (rawmetrialProperty != null)
                                                    {
                                                        ret.Mode = rawmetrialProperty.ThreshType;
                                                        ret.LowerThresh = rawmetrialProperty.LowerThresh;
                                                        ret.UpperThresh = rawmetrialProperty.UpperThresh;
                                                    }
                                                    else
                                                    {
                                                        ret.Mode = -1;
                                                        ret.LowerThresh = 0;
                                                        ret.UpperThresh = 0;
                                                    }
                                                    break;
                                                case eVisInspectType.eInspTypePSROdd:
                                                    PSROddProperty psrOddProperty = inspectionElement.InspectionAlgorithm as PSROddProperty;
                                                    if (psrOddProperty != null)
                                                    {
                                                        ret.Mode = 2;
                                                        ret.LowerThresh = psrOddProperty.Metal_LowerThreshold;
                                                        ret.UpperThresh = psrOddProperty.Metal_UpperThreshold;
                                                    }
                                                    break;

                                                case eVisInspectType.eInspResultGV:
                                                    GV_Inspection_Property gv_Inspection_Property = inspectionElement.InspectionAlgorithm as GV_Inspection_Property;
                                                    if (gv_Inspection_Property != null)
                                                    {
                                                        ret.Mode = 2;
                                                        ret.LowerThresh = gv_Inspection_Property.Ball_Thresh;
                                                        ret.UpperThresh = gv_Inspection_Property.Core_Thresh;
                                                    }
                                                    break;
                                            }
                                            MainWindow.lstRC.Add(ret);
                                            Thread.Sleep(5);
                                        }
                                        nSendedItemCount++;
                                        nRoiCode++;
                                    }
                                }
                            }
                        }

                        m_ptrMainWindow.ProgressWindow.SetProgressValue(anDesVision, 100, "Training");
                        #endregion

                        #region 6. Send Center Line Info.
                        if (bAuto)
                        {
                            nRoiCode = 0;
                            nInspCode = 0;

                            if (indexInfo.CategorySurface == CategorySurface.BP && indexInfo.Slave == true)
                            {
                                foreach (SectionInformation section in TeachingViewers[indexInfo.SurfaceBeginIndex].SectionManager.Sections)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        if (section.ROICanvas[i].ActualHeight > 0 && section.ROICanvas[i].ActualWidth > 0)////BP 경우 흑백이므로 1개의 ROI Canvas를 제외한 다른 것들은 Size가 없음, 추후 Color 적용 할 수 도 있으므로 조건 추가
                                        {
                                            if (section.Type.Code == SectionTypeCode.UNIT_REGION || section.Type.Code == SectionTypeCode.PSR_REGION)
                                            {
                                                foreach (GraphicsBase g in section.ROICanvas[indexInfo.SurfaceBeginIndex].GraphicsList)  // ROI별 반복
                                                {
                                                    if (g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine || g.RegionType == GraphicsRegionType.TapeLoaction)
                                                        continue;
                                                    if (g is GraphicsSelectionRectangle || g is GraphicsLine)
                                                        continue;

                                                    foreach (InspectionItem inspItem in g.InspectionList)
                                                    {
                                                        if (inspItem.InspectionType.InspectType == eVisInspectType.eInspTypeLeadShapeWithCL ||
                                                            inspItem.InspectionType.InspectType == eVisInspectType.eInspTypeSpaceShapeWithCL ||
                                                            inspItem.InspectionType.InspectType == eVisInspectType.eInspTypePlateWithCL)
                                                        {
                                                            if (inspItem.LineSegments != null)
                                                                desVision.SendCenterLineInfo(nInspCode, inspItem.LineSegments, 0);
                                                        }
                                                        nInspCode++;
                                                        Thread.Sleep(5);
                                                    }
                                                    nRoiCode++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (SectionInformation section in currSectionManager.Sections)
                                {
                                    if (bAuto && !bRail)
                                    {
                                        if (section.Type.Code == SectionTypeCode.OUTER_REGION)
                                            continue;
                                    }
                                    for (int i = 0; i < 3; i++)
                                    {
                                        if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] < 6)
                                            if (i == 1)
                                                break;
                                        foreach (GraphicsBase g in section.ROICanvas[i].GraphicsList)  // ROI별 반복
                                        {
                                            if (g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine || g.RegionType == GraphicsRegionType.TapeLoaction)
                                                continue;
                                            if (g is GraphicsSelectionRectangle || g is GraphicsLine)
                                                continue;

                                            foreach (InspectionItem inspItem in g.InspectionList)
                                            {
                                                if (inspItem.InspectionType.InspectType == eVisInspectType.eInspTypeLeadShapeWithCL ||
                                                    inspItem.InspectionType.InspectType == eVisInspectType.eInspTypeSpaceShapeWithCL ||
                                                    inspItem.InspectionType.InspectType == eVisInspectType.eInspTypePlateWithCL)
                                                {
                                                    if (inspItem.LineSegments != null)
                                                        desVision.SendCenterLineInfo(nInspCode, inspItem.LineSegments, i);
                                                }
                                                nInspCode++;
                                                Thread.Sleep(5);
                                            }
                                            nRoiCode++;
                                        }
                                    }
                                }
                            }
                            #endregion                       
                            Thread.Sleep(250);
                        }
                    }
                    if (!desVision.Connected)
                    {
                        // MT005 : 
                        MainWindow.Log("PCS", SeverityLevel.FATAL, string.Format("Vision과의 연결이 끊어졌습니다. 문제가 지속되면 Vision을 다시 시작해 주시기 바랍니다."));
                    }
                }
            }
            catch (Exception e)
            {
                MainWindow.Log("PCS", SeverityLevel.FATAL, string.Format("Vision 정보 전송 실패 {0}", e.ToString()));
                return -1;
            }
            return nSendedItemCount;
        }
        #endregion 비전 정보 전송

        #region 수동 검사
        // 전체 수동 검사.
        private void RunManualInspect()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            btnSendPara.IsEnabled = btnManualInspect.IsEnabled = btnPartialInspect.IsEnabled = false;
            btnManualInspect.Refresh();

            int nDesVision = TeachingViewer.SelectedViewer.ID;
            IndexInfo IndexInfo = m_ptrMainWindow.Convert_ViewIndexToVisionIndex(nDesVision);
            int tempDesVision = IndexInfo.VisionIndex;

            if (m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Connected)
            {
                this.Cursor = Cursors.Wait;

                m_ptrMainWindow.PCSInstance.Vision[tempDesVision].SendGrabSide(IndexInfo.SurfaceScanCount);
                m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ManualInspection(-1, -1, -1, IndexInfo.SurfaceScanCount, (bool)this.ck_Remove_duplicates.IsChecked);

                int sleepCount = 0;
                while (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Inspect_Done)
                {
                    if (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Connected || sleepCount >= 3000) // Timeout : 30 sec.
                    {
                        this.Cursor = Cursors.Arrow;
                        MessageBox.Show("수동 검사에 실패하였습니다.\nTimeout : 30 sec", "Error");
                        MainWindow.Log("PCS", SeverityLevel.DEBUG, "시간 초과로 수동 검사에 실패하였습니다.");
                        btnSendPara.IsEnabled = btnManualInspect.IsEnabled = btnPartialInspect.IsEnabled = true;
                        break;
                    }
                    Thread.Sleep(10);
                    sleepCount++;
                }
                this.Cursor = Cursors.Arrow;

                UpdateLvResult();
                txtNoNG.Visibility = (lvResult.Items.Count == 0) ? Visibility.Visible : Visibility.Hidden;

                // 마지막으로 선택된 유닛 좌표 초기화 작업
                // 불량리스트에서 불량 선택시 같은 유닛좌표의 섹션이미지는 요청하지 않으므로 초기화 필요
                SectionInformation section = TeachingViewer.SelectedViewer.SelectedSection;
                if (section != null)
                {
                    section.LastUnitX = -1;
                    section.LastUnitY = -1;
                }
            }
            btnSendPara.IsEnabled = btnManualInspect.IsEnabled = btnPartialInspect.IsEnabled = true;

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            SetText("검사 완료", 1500);
        }

        // 부분 수동 검사.
        private void RunPartialInspect()
        {
            btnSendPara.IsEnabled = btnManualInspect.IsEnabled = btnPartialInspect.IsEnabled = false;
            btnPartialInspect.Refresh();

            int nDesVision = TeachingViewer.SelectedViewer.ID;
            IndexInfo IndexInfo = m_ptrMainWindow.Convert_ViewIndexToVisionIndex(nDesVision);
            int tempDesVision = IndexInfo.VisionIndex;


            SectionInformation section = TeachingViewer.SelectedViewer.SelectedSection;
            if (section != null)
            {
                int nSecID = section.IntCode;
                int nUnitX = 0;
                int nUnitY = 0;

                if (section.LastUnitX >= 0 && section.LastUnitY >= 0)
                {
                    nUnitX = section.LastUnitX;
                    nUnitY = section.LastUnitY;                 
                }

                if (m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Connected)
                {
                    m_ptrMainWindow.PCSInstance.Vision[tempDesVision].SendImageData(VisionDefinition.SEND_SEC_IMAGE, nSecID, section.GetBitmapSource2((int)TeachingViewer.SelectedViewer.SelectedChannel), nUnitX, nUnitY, (int)TeachingViewer.SelectedViewer.SelectedChannel);
                }
                else
                {
                    lvResult.Items.Clear();
                    m_ptrMainWindow.PCSInstance.Vision[tempDesVision].DefectCount = 0;
                }
            }
            btnSendPara.IsEnabled = btnManualInspect.IsEnabled = btnPartialInspect.IsEnabled = true;
            SetText("전송 완료", 1500);
        }

        // 수동 검사 Enable / Disable
        public void SetManualInspectState(bool abIsEnabled)
        {
            this.btnViewUnit.IsEnabled = abIsEnabled;
            this.btnPartialInspect.IsEnabled = abIsEnabled;
            this.btnManualInspect.IsEnabled = abIsEnabled;
        }

        // 유닛 보기 Enable / Disable
        public void DisableViewUnitFunction()
        {
            this.cmbRegionList.Items.Clear();
            this.cmbRegionList.IsEnabled = false;
            this.btnPartialInspect.IsEnabled = false;
            this.btnViewUnit.IsEnabled = false;
        }

        private void lvResult_LostFocus(object sender, RoutedEventArgs e)
        {
            SectionInformation selectedSection = TeachingViewer.SelectedViewer.SelectedSection;
            if (selectedSection != null)
            {
                if (TeachingViewer.SelectedViewer.pnlInner.Children.Contains(selectedSection.BadRectCanvas[0])) TeachingViewer.SelectedViewer.pnlInner.Children.Remove(selectedSection.BadRectCanvas[0]);
                if (TeachingViewer.SelectedViewer.pnlInner.Children.Contains(selectedSection.BadRectCanvas[1])) TeachingViewer.SelectedViewer.pnlInner.Children.Remove(selectedSection.BadRectCanvas[1]);
                if (TeachingViewer.SelectedViewer.pnlInner.Children.Contains(selectedSection.BadRectCanvas[2])) TeachingViewer.SelectedViewer.pnlInner.Children.Remove(selectedSection.BadRectCanvas[2]);
                this.lvResult.UnselectAll();
            }
        }

        private void lvResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int nDesVision = TeachingViewer.SelectedViewer.ID;

            IndexInfo IndexInfo = m_ptrMainWindow.Convert_ViewIndexToVisionIndex(nDesVision);
            int tempDesVision = IndexInfo.VisionIndex;
            int FrameGrabberIndex = VID.CalcIndex(tempDesVision);

            m_ptrMainWindow.PCSInstance.Vision[tempDesVision].RefImageReady = false;
            m_ptrMainWindow.PCSInstance.Vision[tempDesVision].DefImageReady = false;

            if (lvResult.SelectedIndex >= 0 && lvResult.SelectedIndex < lvResult.Items.Count)
            {
                var sv = FindScrollviewer(lvResult);
                if (sv != null)
                {
                    int n = (int)(Math.Floor((double)lvResult.SelectedIndex / 13.0)) * 13;
                    if (n > 0) sv.ScrollToVerticalOffset(n);
                    else sv.ScrollToVerticalOffset(0);
                }

                lvResult.IsEnabled = false;

                //int nSelIdx = lvResult.SelectedIndex;

                int nSelIdx = Sorted_Results[lvResult.SelectedIndex].RealIndex;


                byte[] arrParam = new byte[sizeof(int) * 2];
                BitConverter.GetBytes(nSelIdx).CopyTo(arrParam, 0 * sizeof(int));

                // Request Ref Image
                BitConverter.GetBytes((int)0).CopyTo(arrParam, 1 * sizeof(int));
                m_ptrMainWindow.PCSInstance.Vision[tempDesVision].SendPacket(VisionDefinition.REQ_RESULT_IMAGE, 8, arrParam);

                int sleepCount = 0;
                while (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].RefImageReady)
                {
                    if (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Connected || sleepCount >= 1000) // Timeout : 7 sec.
                    {
                        lvResult.IsEnabled = true;
                        MessageBox.Show("이미지 취득에 실패하였습니다.\nTimeout : 10 sec", "Error");
                        return;
                    }
                    Thread.Sleep(10);
                    sleepCount++;
                }
                if (m_ptrMainWindow.PCSInstance.Vision[tempDesVision].RefImageReady)
                {
                    if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] >= 6)
                    {
                        ImgRef.Source = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].RefImage_Color;
                        ImgRefTrans.Source = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].RefImage_Color;
                    }
                    else
                    {
                        ImgRef.Source = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].RefImage;
                        ImgRefTrans.Source = ImgRef.Source;
                    }
                }

                // Request Def Image
                BitConverter.GetBytes((int)1).CopyTo(arrParam, 1 * sizeof(int));
                m_ptrMainWindow.PCSInstance.Vision[tempDesVision].SendPacket(VisionDefinition.REQ_RESULT_IMAGE, 8, arrParam);

                sleepCount = 0;
                while (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].DefImageReady)
                {
                    if (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Connected || sleepCount >= 1000) // Timeout : 7 sec.
                    {
                        lvResult.IsEnabled = true;
                        MessageBox.Show("이미지 취득에 실패하였습니다.\nTimeout : 10 sec", "Error");
                        return;
                    }
                    Thread.Sleep(10);
                    sleepCount++;
                }

                if (m_ptrMainWindow.PCSInstance.Vision[tempDesVision].DefImageReady)
                {
                    if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] >= 6)
                        ImgDef.Source = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].DefImage_Color;
                    else
                        ImgDef.Source = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].DefImage;
                }

                ResultItem tmpResult = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].VisionResult.Results[nSelIdx];

                if (tmpResult.SectionID < 0)
                {
                    return;
                }
                // 수동검사할때 LastUnit x, y 로 수동검사함
                int tmpSectionID = tmpResult.SectionID;
                SectionInformation tmpSection = TeachingViewer.SelectedViewer.SelectedSection;
                if ((IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave) && tmpResult.SectionID >= TeachingViewer.SelectedViewer.SectionManager.Sections.Count)
                {
                    TeachingViewer.SelectedViewer.lbSection.SelectedIndex = 0;
       
                    tmpSectionID = 0;
                }
                else TeachingViewer.SelectedViewer.lbSection.SelectedIndex = tmpResult.SectionID;

              
                SectionInformation selectedSection = TeachingViewer.SelectedViewer.SelectedSection;

                if (tmpSection != null)
                {
                    if (selectedSection.Type.Code == SectionTypeCode.UNIT_REGION)
                    {
                        //if ((tmpSection.LastUnitX != tmpResult.UnitPos.X || tmpSection.LastUnitY != tmpResult.UnitPos.Y) || (selectedSection.Type.Code != tmpSection.Type.Code))
                            m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(tmpResult.SectionID, tmpResult.UnitPos.X, tmpResult.UnitPos.Y, 1, tmpResult.Channel, IndexInfo.SurfaceScanCount);
                    }
                    else if (selectedSection.Type.Code == SectionTypeCode.PSR_REGION)
                    {
                        if (tmpResult.ResultType == (int)eVisInspectResultType.eInspResultPSROdd_Core)
                            m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(tmpResult.SectionID, tmpResult.UnitPos.X, tmpResult.UnitPos.Y, 1, Result_Blending_Index, IndexInfo.SurfaceScanCount);
                        else
                            m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(tmpResult.SectionID, tmpResult.UnitPos.X, tmpResult.UnitPos.Y, 1, Result_Saturation_Index, IndexInfo.SurfaceScanCount);
                    }
                    else if (selectedSection.Type.Code == SectionTypeCode.RAW_REGION)
                    {
                        m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(tmpResult.SectionID, 0, tmpResult.UnitPos.Y, 1, tmpResult.Channel, IndexInfo.SurfaceScanCount);
                    }
                    else if (selectedSection.Type.Code == SectionTypeCode.OUTER_REGION)
                    {
                        m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(tmpResult.SectionID, tmpResult.UnitPos.X, tmpResult.UnitPos.Y, 1, tmpResult.Channel, IndexInfo.SurfaceScanCount);
                    }
                    else
                    {
                        m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(tmpResult.SectionID, 0, 0, 1, tmpResult.Channel, IndexInfo.SurfaceScanCount);
                    }
                }
                else
                {
                    if (selectedSection.Type.Code == SectionTypeCode.UNIT_REGION)
                    {
                        m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(tmpResult.SectionID, tmpResult.UnitPos.X, tmpResult.UnitPos.Y, 1, tmpResult.Channel, IndexInfo.SurfaceScanCount);
                    }
                    else if (selectedSection.Type.Code == SectionTypeCode.PSR_REGION)
                    {
                        if (tmpResult.ResultType == (int)eVisInspectResultType.eInspResultPSROdd_Core)
                            m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(tmpResult.SectionID, tmpResult.UnitPos.X, tmpResult.UnitPos.Y, 1, Result_Blending_Index, IndexInfo.SurfaceScanCount);
                        else
                            m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(tmpResult.SectionID, tmpResult.UnitPos.X, tmpResult.UnitPos.Y, 1, Result_Saturation_Index, IndexInfo.SurfaceScanCount);
                    }
                    else if (selectedSection.Type.Code == SectionTypeCode.RAW_REGION)
                    {
                        m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(tmpResult.SectionID, 0, tmpResult.UnitPos.Y, 1, tmpResult.Channel, IndexInfo.SurfaceScanCount);
                    }
                    else if (selectedSection.Type.Code == SectionTypeCode.OUTER_REGION)
                    {
                        m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(tmpResult.SectionID, tmpResult.UnitPos.X, tmpResult.UnitPos.Y, 1, tmpResult.Channel, IndexInfo.SurfaceScanCount);
                    }
                    else
                    {
                        m_ptrMainWindow.PCSInstance.Vision[tempDesVision].ReqSectionRGB(tmpResult.SectionID, 0, 0, 1, tmpResult.Channel, IndexInfo.SurfaceScanCount);
                    }
                }
             
                //TeachingViewer.SelectedViewer.lbSection.SelectedIndex = tmpResult.SectionID;
                //selectedSection = TeachingViewer.SelectedViewer.SelectedSection;
                selectedSection.LastUnitX = tmpResult.UnitPos.X;
                selectedSection.LastUnitY = tmpResult.UnitPos.Y;

                sleepCount = 0;
                while (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Grab_Done)
                {
                    if (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Connected || sleepCount >= 30) // Timeout : 3 sec.
                    {
                        lvResult.IsEnabled = true;
                        // MT003 : Section 이미지를 획득하는데 실패하였습니다. Vision과의 연결 상태를 확인 후 다시 시도해 주시기 바랍니다.
                        MessageBox.Show(ResourceStringHelper.GetErrorMessage("MT003"), "Error");
                        return;
                    }
                    Thread.Sleep(100);
                    sleepCount++;
                }

                if (TeachingViewer.SelectedViewer.lbSection.SelectedIndex != tmpSectionID)
                {
                    ViewChanged = false;
                    TeachingViewer.SelectedViewer.lbSection.SelectedIndex = tmpSectionID;

                    sleepCount = 0;
                    while (!ViewChanged)
                    {
                        if (!m_ptrMainWindow.PCSInstance.Vision[tempDesVision].Connected || sleepCount >= 300) // Timeout : 3sec.
                        {
                            lvResult.IsEnabled = true;
                            return;
                        }
                        Thread.Sleep(10);
                        sleepCount++;
                    };
                }

                switch (tmpResult.Channel)
                {
                    case 0: // R
                        TeachingViewer.SelectedViewer.rdR.IsChecked = true;
                        break;

                    case 1: // G
                        TeachingViewer.SelectedViewer.rdG.IsChecked = true;
                        break;

                    case 2: // B
                        TeachingViewer.SelectedViewer.rdB.IsChecked = true;
                        break;
                }               
                //if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] >= 6)
                //{

                //    if (tmpResult.Channel == 0)
                //    {
                //        TeachingViewer.SelectedViewer.rdR.IsChecked = true;
                //    }
                //    else if (tmpResult.Channel == 1)
                //    {
                //        TeachingViewer.SelectedViewer.rdG.IsChecked = true;
                //    }
                //    else
                //        TeachingViewer.SelectedViewer.rdB.IsChecked = true;
                //}
                TeachingViewer.SelectedViewer.SetRGB();

                selectedSection.SetTempBitmapSource(m_ptrMainWindow.PCSInstance.Vision[tempDesVision].SectionImage, tmpResult.Channel);
                TeachingViewer.SelectedViewer.ThumbnailViewer.SetSourceImage(TeachingViewer.SelectedViewer);
                TeachingViewer.SelectedViewer.pnlInner.Children.Remove(selectedSection.BadRectCanvas[tmpResult.Channel]);

                // 불량 위치를 표시한다.
                if (tmpResult.RelativeDefectRect.Width <= 64.0 && tmpResult.RelativeDefectRect.Height <= 64.0) // 불량 크기가 너무 작은 경우 고정값 사용. (기준 64*64)
                {
                    selectedSection.DrawBadRect(tmpResult.Channel, 64.0, 64.0, tmpResult.RelativeDefectCenter.X - 32.0, tmpResult.RelativeDefectCenter.Y - 32.0);
                }
                else
                {
                    selectedSection.DrawBadRect(tmpResult.Channel, tmpResult.RelativeDefectRect.Width, tmpResult.RelativeDefectRect.Height, tmpResult.RelativeDefectRect.X, tmpResult.RelativeDefectRect.Y);
                }

                selectedSection.ROICanvas[tmpResult.Channel].UnselectAll();
                if (selectedSection.IntCode == tmpResult.SectionID)
                {
                    foreach (GraphicsBase g in selectedSection.ROICanvas[tmpResult.Channel].GraphicsList)
                    {
                        if (g.InspectionList.Count > 0)
                        {
                            if (g.InspectionList[0].InspectionType.InspectType == eVisInspectType.eInspTypePSROdd)
                            {
                                //PSR 회로검사
                                PSROddProperty item = g.InspectionList[0].InspectionAlgorithm as PSROddProperty;

                                if ((PSR_Inspection_Type)item.ThreshType == PSR_Inspection_Type.Circuit)
                                {
                                    foreach (InspectionItem inspectionElement in g.InspectionList)
                                    {
                                        if (tmpResult.RoiID == -10)
                                        {
                                            selectedSection.ROICanvas[tmpResult.Channel].SelectedGraphic = g;
                                            InspectionTypectrl.lbInspection.SelectedItem = inspectionElement;

                                            g.IsSelected = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (g.SentID != null && (int)g.SentID == tmpResult.RoiID)
                        {
                            foreach (InspectionItem inspectionElement in g.InspectionList)
                            {
                                if (inspectionElement.SentID != null && (int)inspectionElement.SentID == tmpResult.InspID)
                                {
                                    selectedSection.ROICanvas[tmpResult.Channel].SelectedGraphic = g;
                                    InspectionTypectrl.lbInspection.SelectedItem = inspectionElement;

                                    g.IsSelected = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                TeachingViewer.SelectedViewer.CalculateZoomToFitScale();
                TeachingViewer.SelectedViewer.pnlInner.Children.Add(selectedSection.BadRectCanvas[tmpResult.Channel]);
                TeachingViewer.SelectedViewer.ViewDefectPosition(tmpResult.RelativeDefectCenter.X, tmpResult.RelativeDefectCenter.Y);
                TeachingViewer.SelectedViewer.txtReferenceLabel.Visibility = Visibility.Hidden;

                lvResult.IsEnabled = true;
            }
        }

        public bool check_datamatrix(ref List<string> data_list)
        {
            Regex regex = new Regex("^[a-zA-Z0-9\\s]*$");

            try
            {
                for (int i = 0; i < data_list.Count; i++)
                {
                    if (data_list[i].Contains("\t") || data_list[i].Contains("\v") || !regex.IsMatch(data_list[i]) || data_list[i] == "")
                    {
                        data_list.RemoveAt(i);
                        i--;
                    }
                }
                if (data_list.Count < 1)
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 수동 검사 결과 Update.
        public void UpdateLvResult()
        {
            if (m_ptrMainWindow != null && m_ptrMainWindow.PCSInstance != null)
            {
                BitmapImage StandardImage = new BitmapImage();
                StandardImage.BeginInit();
                StandardImage.UriSource = new Uri(@"./Images/REF.png", UriKind.Relative);
                StandardImage.EndInit();
                ImgRef.Source = StandardImage;

                BitmapImage NGImage = new BitmapImage();
                NGImage.BeginInit();
                NGImage.UriSource = new Uri(@"./Images/DEF.png", UriKind.Relative);
                NGImage.EndInit();
                ImgDef.Source = NGImage;

                txtNoNG.Visibility = Visibility.Hidden;
                lvResult.Items.Clear();

                int nDesVision = TeachingViewer.SelectedViewer.ID;
                IndexInfo IndexInfo = m_ptrMainWindow.Convert_ViewIndexToVisionIndex(nDesVision);
                int tempDesVision = IndexInfo.VisionIndex;

                int nDefectCount = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].DefectCount;


                // 박경수
                for(int i = 0; i < m_ptrMainWindow.PCSInstance.Vision[tempDesVision].VisionResult.ResultItemCount; i++)
                {
                    int ResultType = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].VisionResult.Results[i].ResultType;
                    m_ptrMainWindow.PCSInstance.Vision[tempDesVision].VisionResult.Results[i].ResultName = MainWindow.NG_Info.ResultTypeToBadInfo(ResultType).Name;

                    if (ResultType == (int)eVisInspectResultType.eInspResultPSROdd_Circuit)
                        m_ptrMainWindow.PCSInstance.Vision[tempDesVision].VisionResult.Results[i].ResultName = "PSR-회로부";

                    if (ResultType == (int)eVisInspectResultType.eInspResultPSROdd_Core)
                        m_ptrMainWindow.PCSInstance.Vision[tempDesVision].VisionResult.Results[i].ResultName = "PSR-Core";

                    if (ResultType == (int)eVisInspectResultType.eInspResultPSROdd_Metal)
                        m_ptrMainWindow.PCSInstance.Vision[tempDesVision].VisionResult.Results[i].ResultName = "PSR-Metal";

                    if (ResultType == (int)eVisInspectResultType.eInspResultPSROdd_Common)
                        m_ptrMainWindow.PCSInstance.Vision[tempDesVision].VisionResult.Results[i].ResultName = "PSR-공통";
                }

                ResultItem[] ResultItems = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].VisionResult.Results;
                PSRResults PSRResults = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].PSRResult;

                if (MainWindow.CurrentModel.ITS.UseITS && m_ptrMainWindow.PCSInstance.Vision[tempDesVision].VisionResult.IDMark.Status == 1)
                {
                    try
                    {
                        IDMarkResultInfo idmark = m_ptrMainWindow.PCSInstance.Vision[tempDesVision].VisionResult.IDMark;
                        string filter = "";
                        List<string> id = new List<string>();
                        id = DataMatrixCode.Algo_Conv_Square_DataMatrix2(idmark.Image, "Teaching", filter, 0);
                        if (id != null)
                        {
                            if (id.Count > 0)
                                lblStripID.Content = id[0];
                            else
                            {
                                id = DataMatrixCode.Algo_Conv_Square_DataMatrix(idmark.Image, "Teaching", filter, 0);
                                if (id != null)
                                {
                                    if (id.Count > 0)
                                        lblStripID.Content = id[0];
                                    else
                                    {
                                        id = DataMatrixCode.GetDataMatrixMJ(idmark.Image, "Teaching", filter, 0);
                                        if (id.Count > 0)
                                            lblStripID.Content = id[0];
                                        else
                                            lblStripID.Content = "인식 실패";
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Setting.xml 파일의 ITS USE = 1 입니다.\n 정상인지 확인해 주세요.");
                    }
                }

                Sorted_Results = new ResultItem[nDefectCount];

                if (nDefectCount > 0)
                {
                    lvResult.IsEnabled = true;
                    for (int i = 0; i < nDefectCount; i++)
                    {
                        if (ResultItems[i].Grabside != IndexInfo.SurfaceScanCount) 
                            continue;

                        ResultItems[i].InspType = GetInspTypeString(nDesVision, ResultItems[i].SectionID, ResultItems[i].RoiID, ResultItems[i].InspID);
                        int y = 0;

                        if (ResultItems[i].UnitPos.X >= 0 && ResultItems[i].UnitPos.Y >= 0)
                        {              
                            if (ResultItems[i].SectionType == PCS.ModelTeaching.SectionTypeCode.UNIT_REGION ||
                            ResultItems[i].SectionType == PCS.ModelTeaching.SectionTypeCode.PSR_REGION)
                            {
                                if (IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave == true)
                                    y = (int)Math.Ceiling(MainWindow.CurrentModel.Strip.UnitRow / 2.0) - ResultItems[i].UnitPos.X - 1;
                                else
                                {
                                    if (IndexInfo.CategorySurface == CategorySurface.BA)
                                        y = ResultItems[i].UnitPos.X;
                                    else
                                        y = MainWindow.CurrentModel.Strip.UnitRow - ResultItems[i].UnitPos.X - 1;
                                }


                                ResultItems[i].ResPosition = string.Format("X:{0},Y:{1}", y + 1, ResultItems[i].UnitPos.Y + 1);
                            }
                            else
                            { 
                               ResultItems[i].ResPosition = string.Format("X:{0},Y:{1}", ResultItems[i].UnitPos.X + 1, ResultItems[i].UnitPos.Y + 1);              
                            }
                        }


                        Sorted_Results[i] = ResultItems[i];
                        Sorted_Results[i].RealIndex = i;
                    }

                    if (Sorted_Results[0] != null)
                    {
                        Array.Sort(Sorted_Results, (a, b) =>
                         {
                             if (a.UnitPos.X == b.UnitPos.X)
                             {
                                 if (a.UnitPos.Y < b.UnitPos.Y)
                                     return -1;
                                 else
                                     return 1;
                             }
                             else
                             {
                                 if (a.UnitPos.X < b.UnitPos.X)
                                     return 1;
                                 else
                                     return -1;
                             }
                         });
                    
                        for (int i = 0; i < Sorted_Results.Length; i++)
                        {
                            Sorted_Results[i].SortedIndex = i;
                            lvResult.Items.Add(Sorted_Results[i]);
                        }
                    
                    }
                    
                    if (PSRResults != null)
                    {
                        if (PSRResults.ResultItemCount > 0)
                        {
                            Save_PSRresults(PSRResults, nDesVision);
                        }
                    }
                }

            }
        }

        private void Save_PSRresults(PSRResults results, int nDesVision)
        {
            string szGroupName = MainWindow.CurrentGroup.Name;
            string szModelName = MainWindow.CurrentModel.Name;
            string path = DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName);

            for (int i = 0; i < results.ResultItemCount; i++)
            {
                int X = MainWindow.CurrentModel.Strip.UnitRow - results.Results[i].UnitPos.X;
                int Y = results.Results[i].UnitPos.Y + 1;

                if (nDesVision == 1)
                    X = (int)Math.Ceiling(MainWindow.CurrentModel.Strip.UnitRow / 2.0) - results.Results[i].UnitPos.X - 1;

                results.Results[i].UnitPos = new System.Drawing.Point(X, Y);
            }
            //sort
            var v = results.Results.OrderBy(x => x.UnitPos.Y).ThenBy(x => x.UnitPos.X).ThenBy(x => x.Position).ToList();

            string strFile = path + "\\psrshift.xml";
            if (File.Exists(strFile))
            {
                File.Delete(strFile);
            }
            FileStream fs = File.Create(strFile);
            fs.Close();
            List<string> lstLines = new List<string>();

            lstLines.Add("<PSR Shift data>");

            int position = 0;
            string str = "";
            int j = 0;
            for (int i = 0; i < v.Count; i++)
            {
                if (position == 0)
                    str += "<" + v[i].UnitPos.X.ToString() + ", " + v[i].UnitPos.Y.ToString() + ">";
                i += j;
                if (position != v[i].Position)
                {
                    str += "\t\t0, 0";
                    j = -1;
                }

                else
                {
                    str += "\t\t" + v[i].Offset.X.ToString() + ", " + v[i].Offset.Y.ToString();
                    j = 0;
                }

                position++;
                if (position > 3)
                {
                    position = 0;
                    lstLines.Add(str);
                    str = "";
                }
            }

            File.WriteAllLines(strFile, lstLines);
        }

        // 수동 검사 결과 아이템의 검사 방법을 반환한다.
        private string GetInspTypeString(int anDesVision, int anSectionID, int anRoiID, int anInspID)
        {
            IndexInfo IndexInfo = m_ptrMainWindow.Convert_ViewIndexToVisionIndex(anDesVision);

            if (IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave == true)
            {
                foreach (SectionInformation section in TeachingViewers[IndexInfo.VisionIndex].SectionManager.Sections)
                {
                    if (section.Name.Contains("Unit Section"))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (!TeachingViewers[anDesVision].RGB && i == 1)
                                break;
                            foreach (GraphicsBase g in section.ROICanvas[i].GraphicsList)
                            {
                                if (g.SentID != null && (int)g.SentID == anRoiID)
                                {
                                    foreach (InspectionItem inspectionElement in g.InspectionList)
                                    {
                                        if (inspectionElement.SentID != null && (int)inspectionElement.SentID == anInspID)
                                        {
                                            return inspectionElement.InspectionType.Name;
                                        }
                                    }
                                }

                                if (anRoiID == -10)
                                {
                                    return "PSR이물 검사";
                                }

                            }
                        }
                    }
                }
            }
            else
            {
                foreach (SectionInformation section in TeachingViewers[anDesVision].SectionManager.Sections)
                {
                    if (section.IntCode == anSectionID)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (!TeachingViewers[anDesVision].RGB && i == 1)
                                break;
                            foreach (GraphicsBase g in section.ROICanvas[i].GraphicsList)
                            {
                                if (g.SentID != null && (int)g.SentID == anRoiID)
                                {
                                    foreach (InspectionItem inspectionElement in g.InspectionList)
                                    {
                                        if (inspectionElement.SentID != null && (int)inspectionElement.SentID == anInspID)
                                        {
                                            return inspectionElement.InspectionType.Name;
                                        }
                                    }
                                }

                                if (anRoiID == -10)
                                {
                                    return "PSR이물 검사";
                                }

                            }
                        }
                    }
                }
            }
            return "-";
        }
        #endregion 수동 검사

        #region Why do not use?????? 
        private bool SendStripAlign(Algo algo, int anDesVision, GraphicsRectangleBase g, int id, string AlignPath, bool bAuto)
        {
            if (algo != null && m_ptrMainWindow.PCSInstance.Vision[anDesVision].Connected)
            {
                if (m_ptrMainWindow.PCSInstance.Vision[anDesVision].VisionInfo.GrabCount < 1)
                    m_ptrMainWindow.PCSInstance.Vision[anDesVision].VisionInfo.GrabCount = 1;
                int w = (int)g.Rectangle.Width;
                int h = (int)g.Rectangle.Height;
                //System.Drawing.Rectangle rectSearch = new System.Drawing.Rectangle((int)fiducialGraphic.Left, (int)fiducialGraphic.Top,
                //                                                                   (int)fiducialGraphic.WidthProperty, (int)fiducialGraphic.HeightProperty);
                byte[] b;
                if (bAuto)
                {
                    try
                    {
                        FileStream input = new FileStream(AlignPath, FileMode.Open);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            input.CopyTo(ms);
                            b = ms.ToArray();
                        }
                        input.Close();
                    }
                    catch
                    {
                        b = null;
                    }
                }
                else
                {
                    try
                    {
                        b = m_Algo.GetAlignImage(new System.Drawing.Rectangle((int)g.Left, (int)g.Top, (int)g.WidthProperty, (int)g.HeightProperty));
                        if (File.Exists(AlignPath)) File.Delete(AlignPath);
                        FileStream output = new FileStream(AlignPath, FileMode.CreateNew);
                        output.Write(b, 0, b.Length);
                        output.Close();
                    }
                    catch
                    {
                        b = null;
                    }

                }
                if (b == null)
                    return false;

                StripAlignInfo si = new StripAlignInfo();
                si.RoiID = id;
                si.BndRect.X = (int)g.Left;
                si.BndRect.Y = (int)g.Top;
                si.BndRect.Width = w;
                si.BndRect.Height = h;

                if (id < 2)
                {
                    si.SearchMarginX = 500;
                    si.SearchMarginY = 500;
                }
                else
                {
                    si.SearchMarginX = 250;
                    si.SearchMarginY = 250;
                }
                si.Match = 80;

                m_ptrMainWindow.PCSInstance.Vision[anDesVision].SendStripAlign(VisionDefinition.ADD_STRIPALIGN, b, si, id);
                return true;
            }
            return false;
        }


        private bool SendIDMark(int anDesVision, GraphicsRectangleBase g, int id)
        {
            if (m_ptrMainWindow.PCSInstance.Vision[anDesVision].Connected)
            {
                if (m_ptrMainWindow.PCSInstance.Vision[anDesVision].VisionInfo.GrabCount < 1)
                    m_ptrMainWindow.PCSInstance.Vision[anDesVision].VisionInfo.GrabCount = 1;
                int w = (int)g.Rectangle.Width;
                int h = (int)g.Rectangle.Height;

                IDMarkInfo si = new IDMarkInfo();
                si.RoiID = id;
                si.BndRect.X = (int)g.Left;
                si.BndRect.Y = (int)g.Top;
                si.BndRect.Width = w;
                si.BndRect.Height = h;
                si.Threshold = 0;

                m_ptrMainWindow.PCSInstance.Vision[anDesVision].SendIDMark(VisionDefinition.ADD_IDMARK, si, id);
                return true;
            }
            return false;
        }

        #endregion

        #region 조명 컨트롤 제어
        private void btnONOFF_Click(object sender, RoutedEventArgs e)
        {
            if (lightcontrol.IsOpen)
            {
                Button button = sender as Button;
                if (button != null)
                {
                    button.Content = (button.Content.ToString() == "ON") ? "OFF" : "ON";

                    if (button.Content.ToString() == "ON")
                    {
                        lightcontrol.LightOn(false);
                    }
                    else
                    {
                        lightcontrol.LightOn(true);
                    }
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (lightcontrol.IsOpen && MainWindow.CurrentModel != null)
            {
                IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(TeachingViewer.SelectedViewer.ID);
                LightInfo LightInfo = MainWindow.Convert_LightIndex(IndexInfo.CategorySurface, IndexInfo.Index);////////10개의 DB 버퍼공간을 활용하는 방식으로 변경
                int[] Value = new int[MainWindow.CurrentModel.LightValue.GetLength(1)];
                Value = lightcontrol.GetValues();
                for (int i = 0; i < MainWindow.CurrentModel.LightValue.GetLength(1); i++) MainWindow.CurrentModel.LightValue[LightInfo.ValueIndex, i] = Value[i];
                DeviceController.SetLightValue(LightInfo.ValueIndex, Value);
                
                DeviceController.SaveLightValue(MainWindow.CurrentModel.Code, MainWindow.CurrentModel.LightValue);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (lightcontrol.IsOpen && MainWindow.CurrentModel != null)
            {
                IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(TeachingViewer.SelectedViewer.ID);
                LightInfo LightInfo = MainWindow.Convert_LightIndex(IndexInfo.CategorySurface, IndexInfo.Index);////////10개의 DB 버퍼공간을 활용하는 방식으로 변경
                lightcontrol.SetValues(MainWindow.CurrentModel.LightValue, LightInfo.LightIndex, LightInfo.ValueIndex, MainWindow.Setting.SubSystem.Light.Channel);
            }
        }
        #endregion 조명 컨트롤 제어

        #region 유틸리티
        // 기준 이미지 변경
        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
            {
                MessageBox.Show("모델을 선택해 주시기 바랍니다.", "Information");
                return;
            }
            if (TeachingViewer.SelectedViewer.lbSection.SelectedIndex < 0)
            {
                MessageBox.Show("기준 이미지를 변경하려는 Section을 선택 바랍니다.", "Information");
                return;
            }
            TeachingViewer.SelectedViewer.LoadImage();
        }

        // 현재 뷰어의 이미지 저장
        private void btnSaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
            {
                MessageBox.Show("모델을 선택해 주시기 바랍니다.", "Information");
                return;
            }

            // Save source as *.BMP
            TeachingViewer.SelectedViewer.SaveImage();
        }

        // 티칭 데이터 Import
        private void btnLoadXML_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
            {
                MessageBox.Show("모델을 선택해 주시기 바랍니다.", "Information");
                return;
            }

            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string strDirectoryPath = dlg.SelectedPath + @"\";
                try
                {
                    this.Cursor = Cursors.Wait;
                    this.TeachingViewer.SelectedViewer.Cursor = Cursors.Wait;

                    // 티칭 데이터를 복원한다.
                    int Result = 0;
                    bool[] bViewImported = new bool[MainWindow.m_nTotalTeachingViewCount];
                    for (int i = 0; i < MainWindow.m_nTotalTeachingViewCount; i++)
                        bViewImported[i] = TeachingViewer.CamView[i].ImportTeachingData(strDirectoryPath);

                    for (int i = 0; i < MainWindow.m_nTotalTeachingViewCount; i++)
                        Result += Convert.ToInt32(bViewImported[i]);
                    
                    if (Result == MainWindow.m_nTotalTeachingViewCount)
                    {
                        MessageBox.Show("데이터 복원을 완료하였습니다.", "Information");
                    }
                    else if (Result == 0)
                    {
                        MessageBox.Show("현재 모델과 일치하는 데이터를 찾지 못하였습니다.", "Information");
                    }
                    else
                    {
                        string szErrorMessage = "복원 결과\n\n";
                        
                        for (int i = 0; i < MainWindow.m_nTotalTeachingViewCount; i++)
                        {
                            IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(i);
                            string str = "";
                            if (IndexInfo.CategorySurface == CategorySurface.BP)
                            {
                                str = "본드패드" + IndexInfo.Index + 1;
                                szErrorMessage += str + ": 일치하는 데이터를 찾지 못하였습니다.\n";
                            }
                            else if (IndexInfo.CategorySurface == CategorySurface.CA)
                            {
                                str = "CA외관" + IndexInfo.Index + 1;
                                szErrorMessage += str + ": 일치하는 데이터를 찾지 못하였습니다.\n";
                            }
                            else
                            {
                                str = "BA외관" + IndexInfo.Index + 1;
                                szErrorMessage += str + ": 일치하는 데이터를 찾지 못하였습니다.\n";
                            }
                        }                       
                        MessageBox.Show(szErrorMessage, "Information");
                    }
                }
                catch
                {
                    this.Cursor = Cursors.Arrow;
                    this.TeachingViewer.SelectedViewer.Cursor = Cursors.Arrow;

                    MainWindow.Log("PCS", SeverityLevel.ERROR, "Teaching 데이터를 복원하는데 실패하였습니다.");
                    Debug.WriteLine("Exception occured in btnLoadXML_Click(Teachingwindow.xaml.cs)");
                }
                this.Cursor = Cursors.Arrow;
                this.TeachingViewer.SelectedViewer.Cursor = Cursors.Arrow;

                for (int i = 0; i < MainWindow.m_nTotalTeachingViewCount; i++) TeachingViewer.CamView[i].CreateContextMenu();
            }
        }

        // 티칭 데이터 Export
        private void btnSaveXML_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
            {
                MessageBox.Show("모델을 선택해 주시기 바랍니다.", "Information");
                return;
            }

            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string strDirectoryPath = dlg.SelectedPath + @"\";
                try
                {
                    this.Cursor = Cursors.Wait;
                    this.TeachingViewer.SelectedViewer.Cursor = Cursors.Wait;

                    // 티칭 데이터를 추출한다.
                    for(int i = 0; i < MainWindow.m_nTotalTeachingViewCount; i++)
                    {
                        IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(i);
                        if (IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave) continue;////BP, Slave 제외
                        TeachingViewer.CamView[i].ExportTeachingData(strDirectoryPath);
                    }
                    //TeachingViewer.CamBP1View.ExportTeachingData(strDirectoryPath);
                    ////TeachingViewer.CamBP2View.ExportTeachingData(strDirectoryPath);
                    //TeachingViewer.CamCA1View.ExportTeachingData(strDirectoryPath);
                    //TeachingViewer.CamCA2View.ExportTeachingData(strDirectoryPath);
                    //TeachingViewer.CamBA1View.ExportTeachingData(strDirectoryPath);
                    //TeachingViewer.CamBA2View.ExportTeachingData(strDirectoryPath);

                    this.Cursor = Cursors.Arrow;
                    this.TeachingViewer.SelectedViewer.Cursor = Cursors.Arrow;

                    // 탐색기를 실행한다.
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = "explorer.exe";
                    si.UseShellExecute = false;
                    si.RedirectStandardInput = true;
                    Process run = new Process();
                    si.Arguments = strDirectoryPath;
                    run.StartInfo = si;
                    run.Start();
                }
                catch
                {
                    this.Cursor = Cursors.Arrow;
                    this.TeachingViewer.SelectedViewer.Cursor = Cursors.Arrow;

                    MainWindow.Log("PCS", SeverityLevel.ERROR, "Teaching 데이터를 추출하는데 실패하였습니다.");
                    Debug.WriteLine("Exception occured in btnSaveXML_Click(Teachingwindow.xaml.cs)");
                }
            }
        }

        private DispatcherTimer m_timer = new DispatcherTimer();
        public void SetText(string aszText, int anInterval = 0)
        {
            txtStatus.Text = aszText;
            if (anInterval > 0)
            {
                m_timer.Interval = TimeSpan.FromMilliseconds(anInterval);
                m_timer.Tick += m_timer_Tick;
                m_timer.Start();
            }
        }

        private void m_timer_Tick(object sender, EventArgs e)
        {
            txtStatus.Text = "";
            m_timer.Tick -= m_timer_Tick;
            m_timer.Stop();
        }
        #endregion 유틸리티
        System.Windows.Point BP1Maxpoint = new Point();
        System.Windows.Point BPUnitPitch = new Point();
        public void MakeSectionDataICS(int Index, ref List<List<List<SectionInfo>>> SectionData, ref System.Windows.Point BP2SectionData)
        {
            MainWindow.IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(Index);
            foreach (GraphicsRectangle g in TeachingViewers[Index].TeachingCanvas.GraphicsRectangleList)
            {
                if (g.RegionType == GraphicsRegionType.UnitRegion || g.RegionType == GraphicsRegionType.PSROdd)
                {   /////Image File 기준 X가 긴 Scan 방향이라서 티칭과 X,Y가 뒤집힌다
                    int x = g.IterationYPosition;
                    int y = g.IterationXPosition;
                    int UnitPosX = Convert.ToInt32(g.boundaryRect.Y / VisionDefinition.GRAB_IMAGE_SCALE);
                    int UnitPosY = Convert.ToInt32(g.boundaryRect.X / VisionDefinition.GRAB_IMAGE_SCALE);
                    if (IndexInfo.CategorySurface != CategorySurface.BA)
                    {
                        y = MainWindow.CurrentModel.Strip.UnitRow - g.IterationXPosition - 1;
                        if (IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave)
                        {
                            y = (int)Math.Ceiling(MainWindow.CurrentModel.Strip.UnitRow / 2.0) - g.IterationXPosition - 1;

                            double TempCamPitch = (MainWindow.Setting.Job.Cam1Pitch + MainWindow.CurrentModel.XPitch) * 1000;
                            if (MainWindow.CurrentModel.Strip.UnitRow % 2 == 1)
                            {
                                TempCamPitch = MainWindow.Setting.Job.Cam1Pitch - (MainWindow.CurrentModel.Strip.UnitHeight / 2.0) + MainWindow.CurrentModel.XPitch;
                                TempCamPitch = TempCamPitch * 1000;
                            }
                            BP2SectionData.X = Convert.ToInt32(TempCamPitch / MainWindow.Setting.SubSystem.IS.CamResolutionX[(int)IndexInfo.CategorySurface]);
                            BP2SectionData.Y = 0;

                            UnitPosX += Convert.ToInt32(BP2SectionData.Y);
                            UnitPosY += Convert.ToInt32(BP2SectionData.X);
                        }
                    }
                    SectionInfo Sectioninfo = new SectionInfo();
                    Sectioninfo.UnitX = x;
                    Sectioninfo.UnitY = y;
                    Sectioninfo.UnitPosX = UnitPosX;
                    Sectioninfo.UnitPosY = UnitPosY;
                    SectionData[IndexInfo.ScanIndex][(int)SectionArea.Unit].Add(Sectioninfo);
                }
                else if (g.RegionType == GraphicsRegionType.OuterRegion)
                {   /////Image File 기준 X가 긴 Scan 방향이라서 티칭과 X,Y가 뒤집힌다

                    int x = g.IterationYPosition;
                    int y = g.IterationXPosition;
                    int UnitPosX = Convert.ToInt32(g.boundaryRect.Y / VisionDefinition.GRAB_IMAGE_SCALE);
                    int UnitPosY = Convert.ToInt32(g.boundaryRect.X / VisionDefinition.GRAB_IMAGE_SCALE);
                    SectionInfo Sectioninfo = new SectionInfo();
                    Sectioninfo.UnitX = x;
                    Sectioninfo.UnitY = y;
                    Sectioninfo.UnitPosX = UnitPosX;
                    Sectioninfo.UnitPosY = UnitPosY;
                    SectionData[IndexInfo.ScanIndex][(int)SectionArea.Outer].Add(Sectioninfo);
                }

                else if (g.RegionType == GraphicsRegionType.Rawmetrial)
                {   /////Image File 기준 X가 긴 Scan 방향이라서 티칭과 X,Y가 뒤집힌다
                    int x = g.IterationYPosition;
                    int y = g.IterationXPosition;
                    int UnitPosX = Convert.ToInt32(g.boundaryRect.Y / VisionDefinition.GRAB_IMAGE_SCALE);
                    int UnitPosY = Convert.ToInt32(g.boundaryRect.X / VisionDefinition.GRAB_IMAGE_SCALE);
                    SectionInfo Sectioninfo = new SectionInfo();
                    Sectioninfo.UnitX = x;
                    Sectioninfo.UnitY = y;
                    Sectioninfo.UnitPosX = UnitPosX;
                    Sectioninfo.UnitPosY = UnitPosY;
                    SectionData[IndexInfo.ScanIndex][(int)SectionArea.Material].Add(Sectioninfo);
                }
            }
        }
        public bool CheckAlign_MerterialSection()
        {
            for (int nIndex = 0; nIndex < TeachingViewers.Length; nIndex++)
            {       
                IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(nIndex); // hs - BP, CA, BA 인덱스 배분
                foreach (SectionInformation sec in TeachingViewers[nIndex].SectionManager.Sections)
                {
                    if (sec.Type.Code == SectionTypeCode.RAW_REGION)
                    {
                        int RawAlignRoiCount = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            int temp = 0;
                            foreach (GraphicsBase Roi in sec.ROICanvas[i].GraphicsList)
                            {
                                if (Roi.RegionType == GraphicsRegionType.UnitAlign)
                                    temp++;
                            }
                            RawAlignRoiCount = Math.Max(RawAlignRoiCount, temp);
                        }
                        if (RawAlignRoiCount < 3)
                        {
                            WarningDlg warningDlg = new WarningDlg(IndexInfo.Surface + "원소재 섹션 Align 최소 3개 이상 있어야 합니다. 저장 실패");
                            warningDlg.ShowDialog();
                            m_ptrMainWindow.ProgressWindow.StopProgress(0);
                            SetText("저장 실패", 1500);
                            return false;
                        }
                    }                 
                }                      
            }
            return true;
        }
    }
}
