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

using Common;
using Common.DataBase;
using Common.Drawing;
using Common.Drawing.InspectionInformation;
using HDSInspector.SubWindow;
using PCS;
using PCS.ModelTeaching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HDSInspector
{
    public enum ChannelType
    {
        RED = 0,                // Top surface
        GREEN = 1,             // Bottom surface
        BLUE = 2,               // Both surface
        GRAY = 3,        // Transmission
    }

    public delegate void ToolTypeChangeEventHandler(ToolType newToolType);


    public delegate void InspectionChangedEventHandler();


    public partial class TeachingViewerCtrl : UserControl
    {
        #region Dependency Property (Index)
        public static readonly DependencyProperty IndexProperty = DependencyProperty.Register("Index", typeof(int), typeof(TeachingViewerCtrl), new PropertyMetadata(0));
        public int ID
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        public static readonly DependencyProperty nSurfaceProperty = DependencyProperty.Register("nSurface", typeof(int), typeof(TeachingViewerCtrl), new PropertyMetadata(0));
        public int nSurface
        {
            get { return (int)GetValue(nSurfaceProperty); }
            set { SetValue(nSurfaceProperty, value); }
        }

        public static readonly DependencyProperty sSurfaceProperty = DependencyProperty.Register("sSurface", typeof(string), typeof(TeachingViewerCtrl), new PropertyMetadata(""));
        public string sSurface
        {
            get { return (string)GetValue(sSurfaceProperty); }
            set { SetValue(sSurfaceProperty, value); }
        }

        public bool RGB = false;

        #endregion Dependency Property (ID)

        #region Public member variables.
        public Point StripAlign = new Point();
        public Point Section = new Point();
        public TeachingWindow m_ptrTeachingWindow;
        public TeachingViewer m_ptrTeachingViewer;

        public static event ToolTypeChangeEventHandler ToolTypeChangeEvent;

        #endregion

        #region Private member variables.
        private double m_fReferenceImageScale = VisionDefinition.GRAB_IMAGE_SCALE; // 전체영상은 1/4 스케일로 받는다.
        private readonly SectionManager m_SectionManager = new SectionManager();

        private bool m_bFiduRegionDrawing;
        private bool m_bIDMarkRegionDrawing;
        private bool m_bIsUnitAlignDrawing;
        private bool m_bIsWPShiftDrawing;
        private bool m_blsRectangleDrawing = false;
        private bool m_bPitchDrawing = false;
        private bool m_bBlockDrawing = false;

        private bool m_bCut = false;
        private bool m_bPaste = false;
        private Image cutImage;
        private BitmapSource cutBS;

        Point m_MousePoint = new Point(0, 0);//// 이전 마우스 포인터 위치 기억을 위해

        public bool IsGrabDone;
        public bool IsSentDone;
        public bool IsShowPaintState = true;
        public bool IsGrabView { get; set; }
        public bool IsReferenceView
        {
            get { return (BasedImage != null && ReferenceImage != null && !IsGrabView); }
        }

        // Zoom To Fit value.
        private double m_fZoomToFitScale = 1.0;

        private System.Windows.Point? m_ptLastDragPoint;
        private System.Windows.Point? m_ptLastContentMousePosition;
        private System.Windows.Point? m_ptLastCenterOfViewport;

        private Point m_tmpPoint;

        private Algo m_Algo = new Algo();

        #endregion

        #region Properties.
        public BitmapSource GrabImage { get; set; }
        public BitmapSource ReferenceImage { get; set; }

        public AntiAliasedImage BasedImage { get; set; }
        public DrawingCanvas BasedROICanvas { get; set; }

        public SectionManager SectionManager
        {
            get { return m_SectionManager; }
        }
        public BitmapSource CornerImage { get; set; }
        // 선택된 SectionInformation을 반환.
        public SectionInformation SelectedSection
        {
            get
            {
                if (lbSection.SelectedIndex == -1)
                    return null;
                else
                    return m_SectionManager.Sections[lbSection.SelectedIndex];
            }
        }

        public ChannelType SelectedChannel
        {
            get
            {
                if ((bool)rdG.IsChecked)
                    return ChannelType.GREEN;
                else if ((bool)rdB.IsChecked)
                    return ChannelType.BLUE;
                else
                    return ChannelType.RED;
            }
        }

        public double ReferenceImageScale
        {
            get { return m_fReferenceImageScale; }
        }

        // 상, 하, 투 구분.
        public Surface SurfaceType { get; set; }

        // X,Y축 카메라 해상도
        public double CamResolutionX { get; set; }
        public double CamResolutionY { get; set; }

        public double ViewerHeight { get; set; }
        public double ViewerWidth { get; set; }

        //public Point SectionToAlign
        //{
        //    get
        //    {
        //        int minX = 1000000;
        //        int minY = 10000000;
        //        foreach (SectionInformation section in m_SectionManager.Sections)
        //        {
        //            if (section.Type.Code == SectionTypeCode.UNIT_REGION)
        //            {
        //                for (int i = 0; i < section.SectionRegionList.Count; i++)
        //                {
        //                    if (section.SectionRegionList[i].RegionPosition.X < minX)
        //                        minX = section.SectionRegionList[i].RegionPosition.X;
        //                    if (section.SectionRegionList[i].RegionPosition.Y < minY)
        //                        minY = section.SectionRegionList[i].RegionPosition.Y;
        //                }
        //            }
        //        }
        //        return new Point(minX, minY);
        //    }
        //}

        public DrawingCanvas TeachingCanvas
        {
            get
            {
                if (lbSection.SelectedIndex == -1)
                    return BasedROICanvas;
                else if (lbSection.Items.Count > 0 && lbSection.SelectedIndex > -1)
                {
                    if (!RGB)
                        return m_SectionManager.Sections[lbSection.SelectedIndex].ROICanvas[0];
                    else
                        return m_SectionManager.Sections[lbSection.SelectedIndex].ROICanvas[(int)SelectedChannel];
                }
                else
                    return null;
            }
        }

        public BitmapSource TeachingImageSource
        {
            get
            {
                if (lbSection.SelectedIndex == -1)
                {
                    if (IsGrabView)
                        return GrabImage;
                    else
                        return ReferenceImage;
                }
                else if (lbSection.Items.Count > 0 && lbSection.SelectedIndex > -1)
                {
                    if (!RGB)
                        return m_SectionManager.Sections[lbSection.SelectedIndex].GetBitmapSource(0);
                    else
                        return m_SectionManager.Sections[lbSection.SelectedIndex].GetBitmapSource((int)SelectedChannel);
                }
                else
                {
                    return null;
                }
            }
        }

        // 티칭 영상 이미지 컨트롤.
        public Image TeachingImage
        {
            get
            {
                if (lbSection.SelectedIndex == -1)
                    return BasedImage;
                if (lbSection.Items.Count > 0 && lbSection.SelectedIndex > -1)
                {
                    if (!RGB)
                        return m_SectionManager.Sections[lbSection.SelectedIndex].Image[0];
                    else
                        return m_SectionManager.Sections[lbSection.SelectedIndex].Image[(int)this.SelectedChannel];
                }
                else
                    return null;
            }
        }

        public ObservableCollection<SectionInformation> SectionList
        {
            get
            {
                return m_SectionManager.Sections;
            }
        }

        public ToolType Tool
        {
            get
            {
                if (TeachingCanvas != null)
                    return TeachingCanvas.Tool;
                else
                    return ToolType.Pointer;
            }
        }

        private int SourceHeight
        {
            get
            {
                if (TeachingImageSource != null)
                    return TeachingImageSource.PixelHeight;
                else
                    return -1;
            }
        }

        private int SourceWidth
        {
            get
            {
                if (TeachingImageSource != null)
                    return TeachingImageSource.PixelWidth;
                else
                    return -1;
            }
        }

        // Scale 값.
        private double ZoomValue
        {
            get { return sldrScale.Value; }
            set
            {
                sldrScale.Value = value;
                UpdateScale();
            }
        }
        #endregion

        #region Constructor.
        public TeachingViewerCtrl()
        {
            InitializeComponent();
            InitializeEvent();
        }
        #endregion

        #region Initializers.
        public void InitializeDialog()
        {
            ReferenceImage = null;
            BasedImage = new AntiAliasedImage()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 0,
                Height = 0
            };

            BasedROICanvas = new DrawingCanvas(true, false)
            {
                ID = this.ID,       // 상부 반사 - 0 / 하부 반사 - 1 / 상부 투과 - 2 고정값.
                MaxGraphicsCount = 64, // 전체영상에서는 ROI(Section)을 64개까지 그릴 수 있다.
                Background = new SolidColorBrush(Colors.Transparent),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 0,
                Height = 0
            };

            ThumbnailViewer.SetSourceImage(this);

            MainWindow.IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(this.ID);
            SurfaceType = IndexInfo.Surface;
        
            this.btnEditImage.IsEnabled = false;
            this.lbSection.DataContext = null;
            this.IsShowPaintState = true;
            this.IsGrabDone = false;
            this.IsSentDone = false;

            ToolChange(ToolType.Pointer);

            pnlMultiBinarization.Visibility = Visibility.Hidden;
            radSingleThreshold.IsChecked = true;

            chkBinarization.IsChecked = false;
            chkDisableROI.IsChecked = false;

            sldrLowerThreshold.Value = 100;
            sldrUpperThreshold.Value = 200;
            if (MainWindow.Setting.SubSystem.IS.FGType[1] < 6) m_ptrTeachingWindow.InspectionTypectrl.Remove_InspectType_PSRodd();/////컬러 장비가 아니면 해당 검사 기능 Disable을 위해 Combobox 내용 수정 기능
        }

        private void InitializeEvent()
        {
            // Zoom events.
            this.btnZoomIn.Click += zoomBtn_Click;
            this.btnZoomOut.Click += zoomBtn_Click;
            this.btnZoomToFit.Click += zoomBtn_Click;
            this.sldrScale.ValueChanged += sldrScale_ValueChanged;

            // Mouse events.
            this.pnlOuter.MouseDown += pnlOuter_MouseDown;
            this.pnlOuter.MouseUp += pnlOuter_MouseUp;
            this.pnlOuter.MouseMove += pnlOuter_MouseMove;
            this.pnlOuter.MouseWheel += pnlOuter_MouseWheel;
            this.pnlOuter.LostMouseCapture += pnlOuter_LostMouseCapture;

            this.cvsCross.MouseEnter += CrossCanvas_MouseEnter;
            this.cvsCross.MouseLeave += CrossCanvas_MouseLeave;

            // Side button events.
            this.btnSeeReferenceImage.Click += (s, e) => SeeReferenceImage();
            this.btnSeeEntireImage.Click += (s, e) => SeeEntireImage();
            this.btnEditImage.Click += btnEditImage_Click;
            this.btnDeleteROI.Click += btnDeleteROI_Click;
            this.btnConfirmParam.Click += btnConfirmParam_Click;

            // Others.
            this.svTeaching.ScrollChanged += svTeaching_ScrollChanged;
            this.lbSection.SelectionChanged += lbSection_SelectionChanged;
            this.svTeaching.PreviewKeyDown += ViewerCtrlPreviewKeyDown;
            this.svTeaching.PreviewKeyUp += ViewerCtrlPreviewKeyUp;

            // Teaching Tool Change event.
            this.TeachingSubMenuCtrl.TeachingToolChangeEvent += ToolChangeEvent;

            this.txtLength.TextChanged += txtLength_TextChanged;
            this.txtWidth.TextChanged += txtWidth_TextChanged;
            this.txtHeight.TextChanged += txtHeight_TextChanged;

            this.rdR.Checked += RdR_Checked;
            this.rdG.Checked += RdR_Checked;
            this.rdB.Checked += RdR_Checked;

            InspectionTypeCtrl.Event_MakeROIMulti16 += new InsptypeChangedEvent(this.Make_ROI_Width_Multiple16);

            #region About binarization.
            this.chkDisableROI.Click += chkDisableROI_Click;
            this.chkBinarization.Click += chkBinarization_Click;

            this.sldrLowerThreshold.ValueChanged += sldrLowerThreshold_ValueChanged;
            this.sldrUpperThreshold.ValueChanged += sldrUpperThreshold_ValueChanged;
            this.sldrThreshold.ValueChanged += sldrThreshold_ValueChanged;
            this.sldrErosionIter.ValueChanged += sldrErosionIter_ValueChanged;
            this.sldrDilationIter.ValueChanged += sldrDilationIter_ValueChanged;

            this.sldrLowerThreshold.PreviewMouseUp += sldrProcessing_MouseUp;
            this.sldrUpperThreshold.PreviewMouseUp += sldrProcessing_MouseUp;
            this.sldrThreshold.PreviewMouseUp += sldrProcessing_MouseUp;
            this.sldrErosionIter.PreviewMouseUp += sldrProcessing_MouseUp;
            this.sldrDilationIter.PreviewMouseUp += sldrProcessing_MouseUp;

            this.txtLowerThreshold.LostFocus += txtLowerThreshold_LostFocus;
            this.txtUpperThreshold.LostFocus += txtUpperThreshold_LostFocus;
            this.txtErosionIter.LostFocus += txtErosionIter_LostFocus;
            this.txtDialtionIter.LostFocus += txtDialtionIter_LostFocus;
            this.radMultiThreshold.Checked += radThreshold_Checked;
            this.radSingleThreshold.Checked += radThreshold_Checked;
            #endregion

            this.chkFixedROI.Click += chkFixedROI_Click;
        }

        private void RdR_Checked(object sender, RoutedEventArgs e)
        {
            SetRGB();
        }

        public void SetRGB()
        {
            #region Set teaching type.
            TeachingType teachingType = TeachingType.NONE;

            int nSelectedIndex = lbSection.SelectedIndex;

            bool bManu_PSRodd = true;
            MainWindow.IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(ID);

            if (nSelectedIndex != -1)
            {
                chkFixedROI.IsChecked = DrawingCanvas.FixedInspectROI; // ROI 고정 여부 판단.
                btnEditImage.IsEnabled = true;
                m_fReferenceImageScale = 1.0;
                int nChannel = (int)SelectedChannel;
                if (!RGB)
                    nChannel = 0;


                SectionInformation section = lbSection.SelectedItem as SectionInformation;

                //sort
                section.SectionRegionList = section.SectionRegionList.OrderBy(x => x.RegionIndex.X).ThenBy(x => x.RegionIndex.Y).ToList();

                if (section != null)
                {
                    foreach (GraphicsBase g in section.ROICanvas[nChannel].GraphicsList)
                    {
                        // Actual Scale이 틀어지는 경우가 발생하여 동기화 시켜주도록 조치함.
                        if (g.ActualScale != section.ROICanvas[nChannel].ActualScale)
                            g.ActualScale = section.ROICanvas[nChannel].ActualScale;
                    }

                    if (section.Type.Code == SectionTypeCode.STRIP_REGION)
                    {
                        #region STRIP_REGION
                        teachingType = TeachingType.StripAlign;
                        m_ptrTeachingWindow.InspectionTypectrl.ChangeInspectionType(Common.Drawing.InspectionInformation.eSectionType.eSecTypeGlobal);
                        m_ptrTeachingWindow.DisableViewUnitFunction();

                        m_ptrTeachingWindow.cmbRegionList.Items.Clear();
                        m_ptrTeachingWindow.cmbRegionList.IsEnabled = false;

                        m_ptrTeachingWindow.btnPartialInspect.IsEnabled = IsSentDone;
                        m_ptrTeachingWindow.btnViewUnit.IsEnabled = false;
                        #endregion
                    }
                    else if (section.Type.Code == SectionTypeCode.UNIT_REGION)
                    {
                        #region UNIT_REGION
                        teachingType = TeachingType.UnitRegion;
                        m_ptrTeachingWindow.InspectionTypectrl.ChangeInspectionType(Common.Drawing.InspectionInformation.eSectionType.eSecTypeUnit);


                        int cmbRegionList_SeletedIndex = 0;
                        if (m_ptrTeachingWindow.cmbRegionList.Items.Count != 0)
                        {
                            cmbRegionList_SeletedIndex = m_ptrTeachingWindow.cmbRegionList.SelectedIndex;
                        }

                        m_ptrTeachingWindow.cmbRegionList.Items.Clear();
                        m_ptrTeachingWindow.cmbRegionList.IsEnabled = true;

                        foreach (SectionRegion region in section.SectionRegionList)
                        {
                            if (region.IsInspection)
                            {
                                // 수동 검사에서 개별 유닛 보기에 사용되는 Combobox Item을 set 하여줌.
                                if (this.SelectedSection.Type.Code == SectionTypeCode.UNIT_REGION)
                                {
                                    if (IndexInfo.CategorySurface == CategorySurface.BP)
                                    {
                                        if((int)IndexInfo.Surface%2 == 1) // BP1
                                        {
                                            m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", MainWindow.CurrentModel.Strip.UnitRow - region.RegionIndex.X, region.RegionIndex.Y + 1));
                                        }
                                        else // BP2
                                        {
                                            m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));
                                        }
                                     
                                    }
                                    else if (IndexInfo.CategorySurface == CategorySurface.CA)
                                    {
                                        m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", MainWindow.CurrentModel.Strip.UnitRow - region.RegionIndex.X, region.RegionIndex.Y + 1));
                                    }
                                    else  //BA
                                    {
                                        m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));
                                    }                  
                                }
                                else
                                {
                                    m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));
                                }
                            }
                        }
                        m_ptrTeachingWindow.cmbRegionList.SelectedIndex = cmbRegionList_SeletedIndex;

                        m_ptrTeachingWindow.btnPartialInspect.IsEnabled = IsSentDone;
                        m_ptrTeachingWindow.btnViewUnit.IsEnabled = IsSentDone;
                        #endregion
                    }
                    else if (section.Type.Code == SectionTypeCode.PSR_REGION)
                    {
                        #region UNIT_REGION
                        teachingType = TeachingType.PsrRegion;
                        m_ptrTeachingWindow.InspectionTypectrl.ChangeInspectionType(Common.Drawing.InspectionInformation.eSectionType.eSecTypeRegionPsr);

                        int cmbRegionList_SeletedIndex = 0;
                        if (m_ptrTeachingWindow.cmbRegionList.Items.Count != 0)
                        {
                            cmbRegionList_SeletedIndex = m_ptrTeachingWindow.cmbRegionList.SelectedIndex;
                        }

                        m_ptrTeachingWindow.cmbRegionList.Items.Clear();
                        m_ptrTeachingWindow.cmbRegionList.IsEnabled = true;

                        foreach (SectionRegion region in section.SectionRegionList)
                        {
                            if (region.IsInspection)
                            {
                                // 수동 검사에서 개별 유닛 보기에 사용되는 Combobox Item을 set 하여줌.
                                if (this.SelectedSection.Type.Code == SectionTypeCode.PSR_REGION)
                                {
                                    if (IndexInfo.CategorySurface == CategorySurface.BP)
                                    {
                                        if ((int)IndexInfo.Surface % 2 == 1) // BP1
                                        {
                                            m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", MainWindow.CurrentModel.Strip.UnitRow - region.RegionIndex.X, region.RegionIndex.Y + 1));
                                        }
                                        else // BP2
                                        {
                                            m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));
                                        }

                                    }
                                    else if (IndexInfo.CategorySurface == CategorySurface.CA)
                                    {
                                        m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", MainWindow.CurrentModel.Strip.UnitRow - region.RegionIndex.X, region.RegionIndex.Y + 1));
                                    }
                                    else  //BA
                                    {
                                        m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));
                                    }
                                }
                                else
                                {
                                    m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));
                                }
                            }
                        }
                        m_ptrTeachingWindow.cmbRegionList.SelectedIndex = cmbRegionList_SeletedIndex;

                        m_ptrTeachingWindow.btnPartialInspect.IsEnabled = IsSentDone;
                        m_ptrTeachingWindow.btnViewUnit.IsEnabled = IsSentDone;
                        #endregion
                    }
                    else if (section.Type.Code == SectionTypeCode.RAW_REGION)
                    {
                        #region RAW_REGION
                        teachingType = TeachingType.RawRegion;
                        m_ptrTeachingWindow.InspectionTypectrl.ChangeInspectionType(Common.Drawing.InspectionInformation.eSectionType.eSecTypeRail);
                        m_ptrTeachingWindow.DisableViewUnitFunction();


                        int cmbRegionList_SeletedIndex = 0;
                        if (m_ptrTeachingWindow.cmbRegionList.Items.Count != 0)
                        {
                            cmbRegionList_SeletedIndex = m_ptrTeachingWindow.cmbRegionList.SelectedIndex;

                        }
                        m_ptrTeachingWindow.cmbRegionList.Items.Clear();
                        m_ptrTeachingWindow.cmbRegionList.IsEnabled = true;

                        foreach (SectionRegion region in section.SectionRegionList)
                        {
                            // 수동 검사에서 개별 유닛 보기에 사용되는 Combobox Item을 set 하여줌.
                            m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));
                        }
                        m_ptrTeachingWindow.cmbRegionList.SelectedIndex = cmbRegionList_SeletedIndex;

                        m_ptrTeachingWindow.btnPartialInspect.IsEnabled = IsSentDone;
                        m_ptrTeachingWindow.btnViewUnit.IsEnabled = IsSentDone;
                        #endregion
                    }
                    else // section.Type.Code == SectionTypeCode.OUTER_REGION
                    {
                        #region OUTER_REGION
                        teachingType = TeachingType.OuterRegion;
                        m_ptrTeachingWindow.InspectionTypectrl.ChangeInspectionType(Common.Drawing.InspectionInformation.eSectionType.eSecTypeRegionOuter);
                        m_ptrTeachingWindow.DisableViewUnitFunction();


                        int cmbRegionList_SeletedIndex = 0;
                        if (m_ptrTeachingWindow.cmbRegionList.Items.Count != 0)
                        {
                            cmbRegionList_SeletedIndex = m_ptrTeachingWindow.cmbRegionList.SelectedIndex;

                        }
                        m_ptrTeachingWindow.cmbRegionList.Items.Clear();
                        m_ptrTeachingWindow.cmbRegionList.IsEnabled = true;

                        foreach (SectionRegion region in section.SectionRegionList)
                        {
                            // 수동 검사에서 개별 유닛 보기에 사용되는 Combobox Item을 set 하여줌.
                            m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));
                        }
                        m_ptrTeachingWindow.cmbRegionList.SelectedIndex = cmbRegionList_SeletedIndex;

                        m_ptrTeachingWindow.btnPartialInspect.IsEnabled = IsSentDone;
                        m_ptrTeachingWindow.btnViewUnit.IsEnabled = IsSentDone;
                        #endregion
                    }
                    txtReferenceLabel.Visibility = (!section.IsTempView[nChannel]) ? Visibility.Visible : Visibility.Hidden;
                }
            }
            else // 전체 영상
            {
                DrawingCanvas.FixedSectionROI = true;
                chkFixedROI.IsChecked = DrawingCanvas.FixedSectionROI; // 전체 영상에서는 ROI 고정 checkbox가 선택되도록 함.
                btnEditImage.IsEnabled = false;
                m_ptrTeachingWindow.DisableViewUnitFunction();
                m_fReferenceImageScale = VisionDefinition.GRAB_IMAGE_SCALE;
                teachingType = TeachingType.Entire;
            }
            #endregion
            pnlInner.Children.Clear();
            pnlInner.Children.Add(TeachingImage); // Image Control
            pnlInner.Children.Add(TeachingCanvas); // Drawing Canvas

            TeachingCanvas.UnselectAll();
            TeachingCanvas.SelectedGraphic = null;
            //TeachingCanvas.Focus();

            if (m_ptrTeachingWindow != null)
            {
                SetROIDrawingMode(!(bool)chkDisableROI.IsChecked);
                if (chkBinarization.IsChecked == true)
                    Binarization();
                else
                    m_ptrTeachingWindow.HistogramCtrl.HideThresholdGuideLine();
            }
            ToolChange(ToolType.Pointer);
            SetScrollViewerToHome();


            if (IndexInfo.CategorySurface == CategorySurface.BP)
                bManu_PSRodd = false;//BP에서는 PSR ODD 사용하면 안된다
            TeachingSubMenuCtrl.ChangeTeachingMode(teachingType, bManu_PSRodd);
            ThumbnailViewer.SetSourceImage(this);

            m_ptrTeachingWindow.ViewChanged = true;
            if (TeachingImage.Source != null)
                m_ptrTeachingWindow.LineProfileCtrl.SetLineProfileSource(TeachingImage.Source as BitmapSource);

            CalculateZoomToFitScale();
            UpdateScale();
        }

        private void sldrProcessing_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Binarization();
        }

        private void sldrDilationIter_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_ptrTeachingWindow == null)
                return;

            if (Math.Abs(e.OldValue - e.NewValue) == 1.0)
                Binarization();
            else
            {
                // 영상 크기가 1500 * 1500 이하인 경우 UI를 즉각 반영하도록 한다.
                BitmapSource source = TeachingImageSource;
                if (source != null && source.PixelWidth * source.PixelHeight < 1500 * 1500)
                    Binarization();
            }
        }

        private void sldrErosionIter_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_ptrTeachingWindow == null)
                return;

            if (Math.Abs(e.OldValue - e.NewValue) == 1.0)
                Binarization();
            else
            {
                // 영상 크기가 1500 * 1500 이하인 경우 UI를 즉각 반영하도록 한다.
                BitmapSource source = TeachingImageSource;
                if (source != null && source.PixelWidth * source.PixelHeight < 1500 * 1500)
                    Binarization();
            }
        }

        private void txtDialtionIter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtDialtionIter.Text == "")
                txtDialtionIter.Text = "0";
        }

        private void txtErosionIter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtErosionIter.Text == "")
                txtErosionIter.Text = "0";
        }

        private void chkFixedROI_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)chkFixedROI.IsChecked)
            {
                if (lbSection.SelectedIndex == -1)
                    DrawingCanvas.FixedSectionROI = true;
                else
                    DrawingCanvas.FixedInspectROI = true;
            }
            else
            {
                if (lbSection.SelectedIndex == -1)
                    DrawingCanvas.FixedSectionROI = false;
                else
                    DrawingCanvas.FixedInspectROI = false;
            }
        }

        #region Binarization-Controller Event Handler.
        private void radThreshold_Checked(object sender, RoutedEventArgs e)
        {
            if (m_ptrTeachingWindow == null)
                return;

            if (this.radSingleThreshold.IsChecked == true)
            {
                this.pnlMultiBinarization.Visibility = Visibility.Hidden;
                this.pnlSingleBinarization.Visibility = Visibility.Visible;
            }
            else
            {
                this.pnlMultiBinarization.Visibility = Visibility.Visible;
                this.pnlSingleBinarization.Visibility = Visibility.Hidden;
            }
            m_ptrTeachingWindow.HistogramCtrl.HideThresholdGuideLine();
            chkBinarization_Click(null, null);
        }

        private void txtUpperThreshold_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtUpperThreshold.Text == "")
                txtUpperThreshold.Text = "0";
        }

        private void txtLowerThreshold_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtLowerThreshold.Text == "")
                txtLowerThreshold.Text = "0";
        }

        private void chkDisableROI_Click(object sender, RoutedEventArgs e)
        {
            if (chkDisableROI.IsChecked == true)
            {
                HiddenPaint();
            }
            else
            {
                ShowPaint();
            }
        }

        private void chkBinarization_Click(object sender, RoutedEventArgs e)
        {
            if (m_ptrTeachingWindow == null) return;
            if (chkBinarization.IsChecked == true)
            {
                //단일 스레시
                if (radSingleThreshold.IsChecked == true)
                {
                    try
                    {
                        int threshold = Convert.ToInt32(txtThreshold.Text);
                        if (threshold >= 0 && threshold <= 255)
                        {
                            m_ptrTeachingWindow.HistogramCtrl.EnableBinarization(threshold, threshold, true);
                            Binarization();
                        }
                    }
                    catch
                    {
                    }
                }
                else //멀티 스레시
                {
                    try
                    {
                        int lowerThreshold = Convert.ToInt32(txtLowerThreshold.Text);
                        int upperThreshold = Convert.ToInt32(txtUpperThreshold.Text);

                        if (lowerThreshold <= upperThreshold &&
                            lowerThreshold >= 0 &&
                            upperThreshold <= 255)
                        {
                            m_ptrTeachingWindow.HistogramCtrl.EnableBinarization(lowerThreshold, upperThreshold, false);
                            Binarization();
                        }
                    }
                    catch
                    {
                    }
                }
            }
            else m_ptrTeachingWindow.HistogramCtrl.HideThresholdGuideLine();
        }

        private void sldrLowerThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_ptrTeachingWindow == null)
                return;

            if (sldrLowerThreshold.Value >= 0 && sldrLowerThreshold.Value <= sldrUpperThreshold.Value)
            {
                m_ptrTeachingWindow.HistogramCtrl.EnableBinarization((int)sldrLowerThreshold.Value, (int)sldrUpperThreshold.Value, false);

                if (Math.Abs(e.OldValue - e.NewValue) == 1.0)
                    Binarization();
                else
                {
                    // 영상 크기가 1500 * 1500 이하인 경우 UI를 즉각 반영하도록 한다.
                    BitmapSource source = TeachingImageSource;
                    if (source != null && source.PixelWidth * source.PixelHeight < 1500 * 1500)
                        Binarization();
                }
            }
        }

        private void sldrUpperThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_ptrTeachingWindow == null)
                return;

            if (sldrUpperThreshold.Value >= sldrLowerThreshold.Value && sldrUpperThreshold.Value <= 255)
            {
                m_ptrTeachingWindow.HistogramCtrl.EnableBinarization((int)sldrLowerThreshold.Value, (int)sldrUpperThreshold.Value, false);

                if (Math.Abs(e.OldValue - e.NewValue) == 1.0)
                    Binarization();
                else
                {
                    // 영상 크기가 1500 * 1500 이하인 경우 UI를 즉각 반영하도록 한다.
                    BitmapSource source = TeachingImageSource;
                    if (source != null && source.PixelWidth * source.PixelHeight < 1500 * 1500)
                        Binarization();
                }
            }
        }

        private void sldrThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_ptrTeachingWindow == null)
                return;

            if (sldrThreshold.Value >= 0 && sldrThreshold.Value <= 255)
            {
                m_ptrTeachingWindow.HistogramCtrl.EnableBinarization((int)sldrThreshold.Value, (int)sldrThreshold.Value, true);

                if (Math.Abs(e.OldValue - e.NewValue) == 1.0)
                    Binarization();
                else
                {
                    // 영상 크기가 1500 * 1500 이하인 경우 UI를 즉각 반영하도록 한다.
                    BitmapSource source = TeachingImageSource;
                    if (source != null && source.PixelWidth * source.PixelHeight < 1500 * 1500)
                        Binarization();
                }
            }
        }
        #endregion

        #region Update Milimeter textboxes.
        private void txtLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtLength.Text))
            {
                this.txtLengthByResolution.Text = string.Format("({0:f2}mm)", Convert.ToDouble(this.txtLength.Text) * m_ptrTeachingViewer.SelectedViewer.CamResolutionX / 1000 / m_ptrTeachingViewer.SelectedViewer.ReferenceImageScale);
            }
            else
            {
                this.txtLengthByResolution.Text = string.Empty;
            }
        }

        private void txtWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtWidth.Text))
            {
                this.txtWidthByResolution.Text = string.Format("({0:f2}mm)", Convert.ToDouble(this.txtWidth.Text) * m_ptrTeachingViewer.SelectedViewer.CamResolutionX / 1000 / m_ptrTeachingViewer.SelectedViewer.ReferenceImageScale);
            }
            else
            {
                this.txtWidthByResolution.Text = string.Empty;
            }
        }

        private void txtHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtHeight.Text))
            {
                this.txtHeightByResolution.Text = string.Format("({0:f2}mm)", Convert.ToDouble(this.txtHeight.Text) * m_ptrTeachingViewer.SelectedViewer.CamResolutionY / 1000 / m_ptrTeachingViewer.SelectedViewer.ReferenceImageScale);
            }
            else
            {
                this.txtHeightByResolution.Text = string.Empty;
            }
        }
        #endregion

        private void ToolChangeEvent(string aszToolType)
        {
            // Do actions when sub-teaching tool changed.
            if (!string.IsNullOrEmpty(aszToolType))
            {
                switch (aszToolType)
                {
                    default:
                        ToolChange((ToolType)Enum.Parse(typeof(ToolType), aszToolType));
                        break;
                }
            }
        }

        public void SetByCurrentModel()
        {
            ReferenceImage = LoadBasedImage(SurfaceType); // 기존에 저장된 전체영상을 읽어들인다.
            if (ReferenceImage == null) // 기준 영상을 찾지 못하는 경우
            {
                m_SectionManager.Sections.Clear();
                InitializeSurfaceView(SurfaceType);
                this.txtReferenceLabel.Visibility = Visibility.Hidden;
            }
            else // 기준 영상이 있는 경우.
            {
                LoadTeachingData(); // Teaching Data를 읽어들인다.
                this.txtReferenceLabel.Visibility = Visibility.Visible;
            }
        }

        public void LoadTeachingData()
        {
            MainWindow ptrMainWindow = Application.Current.MainWindow as MainWindow;
            if (ptrMainWindow != null && MainWindow.CurrentModel != null)
            {
                string strMachineCode = MainWindow.Setting.General.MachineCode;
                string szModelCode = MainWindow.CurrentModel.Code;
                string szGroupName = MainWindow.CurrentGroup.Name;

                MainWindow.IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(ID);
                int temp_ID = IndexInfo.VisionIndex;
 
                if (!(IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave))//////BP이면서 Slave가 아닐때
                    ptrMainWindow.PCSInstance.Vision[temp_ID].Sections = m_SectionManager.Sections;
                InitializeSurfaceView(SurfaceType);

                // Section이 정상 생성된 경우 ROI를 Section의 캔버스에 그려낸다.
                ROIManager.LoadStripAlignROI(strMachineCode, szGroupName, szModelCode, BasedROICanvas, SurfaceType);
                StripAlign = new Point(10, 10);
                foreach (GraphicsBase g in BasedROICanvas.GraphicsList)
                {
                    if (g.RegionType == GraphicsRegionType.StripOrigin)
                    {
                        GraphicsStripOrigin gs = g as GraphicsStripOrigin;
                        StripAlign = new Point(gs.cpX, gs.cpY);
                    }
                }

                // 기존에 저장된 Section 데이터를 읽어들인다.
                if (!m_ptrTeachingWindow.IsOnLine && m_ptrTeachingWindow.MtsManager.SelectedMachine != null)
                {
                    m_SectionManager.LoadSurfaceData(m_ptrTeachingWindow.MtsManager.SelectedMachine.Name, strMachineCode, szModelCode, SurfaceType, MainWindow.Setting.General.ModelPath,!RGB);
                }
                else
                {
                    m_SectionManager.LoadSurfaceData(null, strMachineCode, szModelCode, SurfaceType, MainWindow.Setting.General.ModelPath,!RGB);
                }
                Section = new Point(StripAlign.X, StripAlign.Y);
                foreach (SectionInformation sec in m_SectionManager.Sections)
                {
                    if (sec.Type.Code == SectionTypeCode.UNIT_REGION || sec.Type.Code == SectionTypeCode.PSR_REGION)
                    {
                        foreach (SectionRegion sr in sec.SectionRegionList)
                        {
                            if (sr.RegionIndex.X == 0 && sr.RegionIndex.Y == 0)
                            {
                                Section = new Point(sr.RegionPosition.X, sr.RegionPosition.Y);
                                break;
                            }
                        }
                    }
                }      

                lbSection.ItemsSource = m_SectionManager.Sections;
                lbSection.SelectedIndex = -1;
                if (lbSection.Items.Count > 0)
                {
                    foreach (SectionInformation section in m_SectionManager.Sections)
                    {
                        // Section ROI를 기준 영상에 포함시킨다.
                        SectionManager.AddSectionToROICanvas(BasedROICanvas, section);

                        for (int i = 0; i < 3; i++)
                        {
                            if (!RGB && i == 1) break;

                            if (section.ROICanvas[i] != null)
                            {
                                // Section이 정상 생성된 경우 ROI를 Section의 캔버스에 그려낸다.
                                ROIManager.LoadROI(strMachineCode, szGroupName, szModelCode, section.Code, section.ROICanvas[i], SurfaceType, i, MainWindow.Setting.General.ModelPath);
                                ToolChange(ToolType.Pointer);

                                for (int nIndex = 0; nIndex < section.ROICanvas[i].GraphicsList.Count; nIndex++)
                                {
                                    if (!section.ROICanvas[i].CanDraw(section.ROICanvas[i][nIndex]))
                                    {
                                        section.ROICanvas[i].GraphicsList.RemoveAt(nIndex);
                                        nIndex--;
                                    }
                                }
                            }
                        }
                    }
                }

                System.Windows.Point tempSectionPos = new System.Windows.Point();
                tempSectionPos = new Point(Section.X - StripAlign.X, Section.Y - StripAlign.Y);
                tempSectionPos.X /= VisionDefinition.GRAB_IMAGE_SCALE;
                tempSectionPos.Y /= VisionDefinition.GRAB_IMAGE_SCALE;
                tempSectionPos.X *= MainWindow.Setting.SubSystem.IS.CamResolutionX[(int)IndexInfo.CategorySurface] / 1000.0;
                tempSectionPos.Y *= MainWindow.Setting.SubSystem.IS.CamResolutionY[(int)IndexInfo.CategorySurface] / 1000.0;
                MainWindow.SectionPosition[m_ptrTeachingWindow.VRS_SectionIndex].DefectSectionPosition = tempSectionPos;

                if (!(IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave))
                {
                    Point MarkSectionPos = new Point();
                    System.Windows.Point tempIDSectionPos = new System.Windows.Point();
                    foreach (SectionInformation sec in m_SectionManager.Sections)
                    {
                        if (sec.Type.Code == SectionTypeCode.ID_REGION)
                        {
                            foreach (SectionRegion sr in sec.SectionRegionList)
                            {
                                if (sr.RegionIndex.X == 0 && sr.RegionIndex.Y == 0)
                                {
                                    MarkSectionPos = new Point(sr.RegionPosition.X - StripAlign.X, sr.RegionPosition.Y - StripAlign.Y);
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
                                        tempIDSectionPos = new Point((Rect.Left + Rect.Right) / 2, (Rect.Top + Rect.Bottom) / 2);
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
                    MainWindow.SectionPosition[m_ptrTeachingWindow.VRS_SectionIndex].IDSectionPosition = tempIDSectionPos;
                    m_ptrTeachingWindow.VRS_SectionIndex++;
                }
                BasedROICanvas.ClearHistory();
            }
        }

        public void InitializeSurfaceView(Surface surface)
        {
            if (ReferenceImage != null)
            {
                UpdateViewerSource(ReferenceImage);
                CalculateZoomToFitScale();
                m_ptrTeachingWindow.LineProfileCtrl.SetLineProfileSource(ReferenceImage);
            }
            else // BasedImageSource == null
            {
                UpdateViewerSource(null);
                ZoomValue = 0.1;
            }
            BasedROICanvas.GraphicsList.Clear();
            BasedROICanvas.SelectedGraphic = null;
            UpdateReferenceLabel();
            SetScrollViewerToHome();

            pnlInner.Children.Clear();
            pnlInner.Children.Add(BasedImage);      // Image Control
            pnlInner.Children.Add(BasedROICanvas);  // Drawing Canvas

            ToolChange(ToolType.Pointer);
            bool bManu_PSRodd = true;
            if (surface.ToString().Contains(CategorySurface.BP.ToString())) 
                bManu_PSRodd = false;//BP에서는 PSR ODD 사용하면 안된다
            TeachingSubMenuCtrl.ChangeTeachingMode(TeachingType.Entire, bManu_PSRodd);
            ThumbnailViewer.SetSourceImage(this);
            m_ptrTeachingWindow.ViewChanged = true;
        }

        /// <summary>   Sets the scroll viewer to home. </summary>
        /// <remarks>   suoow2, 2014-11-10. </remarks>
        public void SetScrollViewerToHome()
        {
            svTeaching.ScrollToHorizontalOffset(0.0);
            svTeaching.ScrollToVerticalOffset(0.0);
        }
        #endregion

        #region Button event handlers.
        private void btnConfirmParam_Click(object sender, RoutedEventArgs e)
        {
            if (m_ptrTeachingViewer == null) return;

            List<List<SectionInformation>> ListSection = new List<List<SectionInformation>>();
            List<List<SectionInformation>> Temp = new List<List<SectionInformation>>();
            for (int i = 0; i < MainWindow.m_nTotalTeachingViewCount; i++)
            {
                List<SectionInformation> sections = m_ptrTeachingViewer.CamView[i].SectionManager.Sections.ToList();
                Temp.Add(sections);
            }
            ListSection = Temp.ToList();
            //List<SectionInformation> section11 = null;
            //if (m_ptrTeachingViewer.CamBP1View.SectionManager.Sections.Count > 0)
            //    section11 = m_ptrTeachingViewer.CamBP1View.SectionManager.Sections.ToList();
            //List<SectionInformation> section12 = null;
            //if (m_ptrTeachingViewer.CamBP2View.SectionManager.Sections.Count > 0)
            //    section12 = m_ptrTeachingViewer.CamBP2View.SectionManager.Sections.ToList();
            //List<SectionInformation> section21 = null;
            //if (m_ptrTeachingViewer.CamCA1View.SectionManager.Sections.Count > 0)
            //    section21 = m_ptrTeachingViewer.CamCA1View.SectionManager.Sections.ToList();
            //List<SectionInformation> section22 = null;
            //if (m_ptrTeachingViewer.CamCA2View.SectionManager.Sections.Count > 0)
            //    section22 = m_ptrTeachingViewer.CamCA2View.SectionManager.Sections.ToList();
            //List<SectionInformation> section31 = null;
            //if (m_ptrTeachingViewer.CamBA1View.SectionManager.Sections.Count > 0)
            //    section31 = m_ptrTeachingViewer.CamBA1View.SectionManager.Sections.ToList();
            //List<SectionInformation> section32 = null;
            //if (m_ptrTeachingViewer.CamBA2View.SectionManager.Sections.Count > 0)
            //    section32 = m_ptrTeachingViewer.CamBA2View.SectionManager.Sections.ToList();

            ParamTemplateWindow paramTemplateWindow = new ParamTemplateWindow(ListSection);
            paramTemplateWindow.SetTeacingWindowPtr(m_ptrTeachingViewer);
            paramTemplateWindow.ShowDialog();
            
            //ParamTemplateWindow paramTemplateWindow = new ParamTemplateWindow(section11, section12, section21 ,section22, section31, section32);
            //paramTemplateWindow.SetTeacingWindowPtr(m_ptrTeachingViewer);
            //paramTemplateWindow.ShowDialog();
        }

        private void btnDeleteROI_Click(object sender, RoutedEventArgs e)
        {
            if (TeachingCanvas != null)
                TeachingCanvas.DeleteAll();
        }

        public void CreateContextMenu()
        {
            TeachingViewerCtrl[] teachingViewers = m_ptrTeachingWindow.TeachingViewers;
            List<SectionInformation> sections = null;
            MenuItem menuItem;
            if (teachingViewers != null)
            {
                for (int nIndex = 0; nIndex < teachingViewers.Length; nIndex++)
                {                    
                    foreach (SectionInformation section in teachingViewers[nIndex].SectionManager.Sections)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            foreach (object item in section.ROICanvas[i].contextMenu.Items)
                            {
                                if (item is MenuItem)
                                {
                                    menuItem = item as MenuItem;
                                    if (menuItem != null)
                                    {
                                        bool bFind = false;
                                        if (!bFind)
                                        {
                                            for (int j = 0; j < MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count; j++)
                                            {
                                                if ((ContextMenuCommand)menuItem.Tag == ContextMenuCommand.CopyROIToBP1 + j)
                                                {
                                                    sections = teachingViewers[j].SectionManager.Sections.ToList();
                                                    bFind = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (!bFind)
                                        {
                                            for (int j = 0; j < MainWindow.Setting.Job.CACount; j++)
                                            {
                                                if ((ContextMenuCommand)menuItem.Tag == ContextMenuCommand.CopyROIToCA1 + j)
                                                {
                                                    sections = teachingViewers[j + (MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count)].SectionManager.Sections.ToList();
                                                    bFind = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (!bFind)
                                        {
                                            for (int j = 0; j < MainWindow.Setting.Job.BACount; j++)
                                            {
                                                if ((ContextMenuCommand)menuItem.Tag == ContextMenuCommand.CopyROIToBA1 + j)
                                                {
                                                    sections = teachingViewers[j + (MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count) + MainWindow.Setting.Job.CACount].SectionManager.Sections.ToList();
                                                    bFind = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (sections != null)
                                        {
                                            menuItem.Items.Clear();
                                            foreach (SectionInformation _section in sections)
                                            {
                                                MenuItem subItem = new MenuItem();
                                                subItem.Header = _section.Name;
                                                subItem.Tag = String.Format("{0}|{1}", menuItem.Tag, _section.HashID);
                                                subItem.Click += CopySectionROI;
                                                menuItem.Items.Add(subItem);
                                            }
                                            if (sections.Count > 1)
                                            {
                                                menuItem.Items.Add(new Separator());
                                                MenuItem subItem = new MenuItem();
                                                subItem.Header = "전체 적용";
                                                subItem.Foreground = new SolidColorBrush(Colors.Blue);
                                                subItem.FontWeight = FontWeights.Bold;
                                                subItem.Tag = String.Format("{0}|{1}", menuItem.Tag, -1);
                                                subItem.Click += CopySectionROI;
                                                menuItem.Items.Add(subItem);
                                            }
                                        }
                                        sections = null;
                                    }
                                }
                            }
                        } // end of inner foreach
                    } // end of outer foreach
                } // end of for.
            }
        }

        /// <summary>   Copies the section roi. </summary>
        /// <remarks>   suoow2, 2012-03-09. </remarks>
        private void CopySectionROI(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = sender as MenuItem;
                if (item == null || m_ptrTeachingWindow == null)
                {
                    return;
                }

                // Parsing Item.Tag
                string strTag = item.Tag.ToString();
                string strContextMenuCommand = strTag.Substring(0, strTag.IndexOf('|'));
                ContextMenuCommand command = (ContextMenuCommand)Enum.Parse(typeof(ContextMenuCommand), strContextMenuCommand);
                int sectionHashID = Convert.ToInt32(strTag.Substring(strTag.IndexOf('|') + 1, (strTag.Length - 1) - strTag.IndexOf('|')));

                if (sectionHashID > 0) // 단일 섹션으로의 복사 수행.
                {
                    // 복사 대상이 될 Section을 찾는다.
                    SectionInformation targetSection = null;
                    int nSelectView = 0;
                    for (int i = 0; i < MainWindow.m_nTotalTeachingViewCount; i++)
                    {
                        if (command - ContextMenuCommand.CalcCommandNum == m_ptrTeachingViewer.CamView[i].nSurface)
                        {
                            nSelectView = i;
                            break;
                        }
                    }

                    foreach (SectionInformation section in m_ptrTeachingViewer.CamView[nSelectView].SectionManager.Sections)
                    {
                        if (sectionHashID == section.HashID)
                        {
                            targetSection = section;
                            break;
                        }
                    }
                    //switch (command)
                    //{
                    //    case ContextMenuCommand.CopyROIToBA1:
                    //        foreach (SectionInformation section in m_ptrTeachingViewer.CamBA1View.SectionManager.Sections)
                    //        {
                    //            if (sectionHashID == section.HashID)
                    //            {
                    //                targetSection = section;
                    //                break;
                    //            }
                    //        }
                    //        break;
                    //    case ContextMenuCommand.CopyROIToBA2:
                    //        foreach (SectionInformation section in m_ptrTeachingViewer.CamBA2View.SectionManager.Sections)
                    //        {
                    //            if (sectionHashID == section.HashID)
                    //            {
                    //                targetSection = section;
                    //                break;
                    //            }
                    //        }
                    //        break;
                    //    case ContextMenuCommand.CopyROIToBP1:
                    //        foreach (SectionInformation section in m_ptrTeachingViewer.CamBP1View.SectionManager.Sections)
                    //        {
                    //            if (sectionHashID == section.HashID)
                    //            {
                    //                targetSection = section;
                    //                break;
                    //            }
                    //        }
                    //        break;
                    //    case ContextMenuCommand.CopyROIToBP2:
                    //        foreach (SectionInformation section in m_ptrTeachingViewer.CamBP2View.SectionManager.Sections)
                    //        {
                    //            if (sectionHashID == section.HashID)
                    //            {
                    //                targetSection = section;
                    //                break;
                    //            }
                    //        }
                    //        break;
                    //    case ContextMenuCommand.CopyROIToCA1:
                    //        foreach (SectionInformation section in m_ptrTeachingViewer.CamCA1View.SectionManager.Sections)
                    //        {
                    //            if (sectionHashID == section.HashID)
                    //            {
                    //                targetSection = section;
                    //                break;
                    //            }
                    //        }
                    //        break;
                    //    case ContextMenuCommand.CopyROIToCA2:
                    //        foreach (SectionInformation section in m_ptrTeachingViewer.CamCA2View.SectionManager.Sections)
                    //        {
                    //            if (sectionHashID == section.HashID)
                    //            {
                    //                targetSection = section;
                    //                break;
                    //            }
                    //        }
                    //        break;
                    //}           

                    // ROI를 복사한다.
                    SectionInformation selectedSection = m_ptrTeachingWindow.TeachingViewer.SelectedViewer.SelectedSection;
                    if (selectedSection != null && targetSection != null && selectedSection != targetSection)
                    {
                        string szSelectedSectionType = selectedSection.Type.Name;
                        if (szSelectedSectionType == targetSection.Type.Name)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                foreach (GraphicsBase g in selectedSection.ROICanvas[i].Selection)
                                {
                                    if (g.RegionType == GraphicsRegionType.LocalAlign)// || g.RegionType == GraphicsRegionType.GuideLine)
                                        continue;
                                    if (g is GraphicsSelectionRectangle || g is GraphicsLine)
                                        continue;

                                    if (targetSection.ROICanvas[i].CanDraw(g))
                                    {
                                        GraphicsBase graphic = g.CreateSerializedObject().CreateGraphics();
                                        graphic.OriginObjectColor = graphic.ObjectColor = g.OriginObjectColor;
                                        targetSection.ROICanvas[i].GraphicsList.Add(graphic);

                                        if (g.LocalAligns != null)
                                        {
                                            graphic.LocalAligns = new GraphicsRectangle[g.LocalAligns.Length];
                                            for (int nIndex = 0; nIndex < g.LocalAligns.Length; nIndex++)
                                            {
                                                if (g.LocalAligns[nIndex] != null)
                                                {
                                                    // Create Local Align.
                                                    GraphicsRectangle localAlign = targetSection.ROICanvas[i].CreateLocalAlign(graphic, nIndex, g.LocalAligns[nIndex].boundaryRect);
                                                    localAlign.MotherROI = graphic;
                                                    graphic.LocalAligns[nIndex] = localAlign;
                                                    targetSection.ROICanvas[i].GraphicsList.Add(localAlign);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else // 전체 적용
                {
                    int nSelectView = 0;
                    List<SectionInformation> sections = null;

                    for (int i = 0; i < MainWindow.m_nTotalTeachingViewCount; i++)
                    {
                        if (command - ContextMenuCommand.CalcCommandNum == m_ptrTeachingViewer.CamView[i].nSurface)
                        {
                            nSelectView = i;
                            break;
                        }
                    }
                    sections = m_ptrTeachingViewer.CamView[nSelectView].SectionManager.Sections.ToList();

                    //switch (command)
                    //{
                    //    case ContextMenuCommand.CopyROIToBA1:
                    //        sections = m_ptrTeachingViewer.CamBA1View.SectionManager.Sections.ToList();
                    //        break;
                    //    case ContextMenuCommand.CopyROIToBA2:
                    //        sections = m_ptrTeachingViewer.CamBA2View.SectionManager.Sections.ToList();
                    //        break;
                    //    case ContextMenuCommand.CopyROIToBP1:
                    //        sections = m_ptrTeachingViewer.CamBP1View.SectionManager.Sections.ToList();
                    //        break;
                    //    case ContextMenuCommand.CopyROIToBP2:
                    //        sections = m_ptrTeachingViewer.CamBP2View.SectionManager.Sections.ToList();
                    //        break;
                    //    case ContextMenuCommand.CopyROIToCA1:
                    //        sections = m_ptrTeachingViewer.CamCA1View.SectionManager.Sections.ToList();
                    //        break;
                    //    case ContextMenuCommand.CopyROIToCA2:
                    //        sections = m_ptrTeachingViewer.CamCA2View.SectionManager.Sections.ToList();
                    //        break;
                    //}

                    SectionInformation selectedSection = m_ptrTeachingWindow.TeachingViewer.SelectedViewer.SelectedSection;
                    if (selectedSection != null && sections != null)
                    {
                        string szSelectedSectionType = selectedSection.Type.Name;
                        for (int i = 0; i < 3; i++)
                        {
                            foreach (SectionInformation section in sections)
                            {
                                if (selectedSection == section)
                                    continue;
                                else if (szSelectedSectionType != section.Type.Name)
                                    continue;
                                section.ROICanvas[i].GraphicsList.Clear();
                                //section.GraphicsList.Clear();
                            }

                            foreach (GraphicsBase g in selectedSection.ROICanvas[i].Selection)
                            {
                                if (g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine || g.RegionType == GraphicsRegionType.TapeLoaction)
                                    continue;
                                if (g is GraphicsSelectionRectangle || g is GraphicsLine)
                                    continue;

                                foreach (SectionInformation section in sections)
                                {
                                    if (selectedSection == section)
                                        continue;
                                    else if (szSelectedSectionType != section.Type.Name)
                                        continue;

                                    if (section.ROICanvas[i].CanDraw(g))
                                    {
                                        GraphicsBase graphic = g.CreateSerializedObject().CreateGraphics();
                                        graphic.OriginObjectColor = graphic.ObjectColor = g.OriginObjectColor;
                                        section.ROICanvas[i].GraphicsList.Add(graphic);

                                        if (g.LocalAligns != null)
                                        {
                                            graphic.LocalAligns = new GraphicsRectangle[g.LocalAligns.Length];
                                            for (int nIndex = 0; nIndex < g.LocalAligns.Length; nIndex++)
                                            {
                                                if (g.LocalAligns[nIndex] != null)
                                                {
                                                    // Create Local Align.
                                                    GraphicsRectangle localAlign = section.ROICanvas[i].CreateLocalAlign(graphic, nIndex, g.LocalAligns[nIndex].boundaryRect);
                                                    localAlign.MotherROI = graphic;
                                                    graphic.LocalAligns[nIndex] = localAlign;
                                                    section.ROICanvas[i].GraphicsList.Add(localAlign);
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
            catch
            {
                MainWindow.Log("PCS", SeverityLevel.ERROR, "섹션 ROI 복사에 실패하였습니다.");
            }
        }

        private void btnEditImage_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)chkBinarization.IsChecked)
            {
                TeachingImage.Source = m_ptrTeachingViewer.SelectedViewer.TeachingImageSource;
            }
            pnlInner.Children.Clear();

            Border smokeBorder = new Border { Background = new SolidColorBrush(Color.FromArgb(255, 196, 196, 196)), Opacity = 0.5 };
            Grid.SetRowSpan(smokeBorder, 4);
            Grid.SetColumnSpan(smokeBorder, 3);
            m_ptrTeachingWindow.pnlMain.Children.Add(smokeBorder);

            ImageEditor imageEditor = new ImageEditor(m_ptrTeachingViewer.SelectedViewer.TeachingImageSource, m_fZoomToFitScale, this) { Owner = m_ptrTeachingWindow };
            imageEditor.ShowDialog();

            m_ptrTeachingWindow.pnlMain.Children.Remove(smokeBorder);

            pnlInner.Children.Add(TeachingImage); // Image Control
            pnlInner.Children.Add(TeachingCanvas); // Drawing Canvas

            if ((bool)chkBinarization.IsChecked)
            {
                TeachingCanvas.UnselectAll();
            }
        }
        #endregion

        #region Event Handlers.
        private void svTeaching_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (TeachingImageSource == null)
            {
                return;
            }

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

                    double newOffsetX = svTeaching.HorizontalOffset - fDeltaXInTargetPixels * fMultiplicatorX;
                    double newOffsetY = svTeaching.VerticalOffset - fDeltaYInTargetPixels * fMultiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {
                        return;
                    }

                    svTeaching.ScrollToHorizontalOffset(newOffsetX);
                    svTeaching.ScrollToVerticalOffset(newOffsetY);
                }
            }
            else
            {
                m_ptLastCenterOfViewport = GetCenterOfViewport();
            }

            ThumbnailViewer.UpdateNavigator();
        }

        public void ViewDefectPosition(double afPositionX, double afPositionY)
        {
            if (TeachingImageSource == null)
            {
                return;
            }
            double offsetX = svTeaching.ExtentWidth * (afPositionX / TeachingImageSource.PixelWidth);
            double offsetY = svTeaching.ExtentHeight * (afPositionY / TeachingImageSource.PixelHeight);

            double margin = (svTeaching.ExtentWidth - svTeaching.ScrollableWidth) / 2;
            offsetX -= margin;
            offsetY -= margin;

            svTeaching.ScrollToHorizontalOffset(offsetX);
            svTeaching.ScrollToVerticalOffset(offsetY);
        }

        private void ViewerCtrlPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                this.Cursor = Cursors.Arrow;
                ToolChange(ToolType.Pointer);
                e.Handled = true;
            }
        }

        // Keyboard 입력에 대한 처리.
        private void ViewerCtrlPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Down || e.Key == Key.Up)
            {
                if (txtLowerThreshold.Text == "")
                    txtLowerThreshold.Text = "0";
                else if (txtUpperThreshold.Text == "")
                    txtUpperThreshold.Text = "255";
            }
            //else if (e.Key == Key.Space)
            //{
            //    ToolChange(ToolType.Move);
            //}
            e.Handled = true;
        }

        private void pnlOuter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TeachingCanvas == null || TeachingImageSource == null)
            {
                return;
            }
            else if (TeachingCanvas.ActualWidth < e.GetPosition(TeachingCanvas).X || TeachingCanvas.ActualHeight < e.GetPosition(TeachingCanvas).Y)
            {
                return;
            }

            if ((e.RightButton == MouseButtonState.Pressed) && (this.Cursor == System.Windows.Input.Cursors.ScrollAll))
            {
                this.Cursor = Cursors.Arrow;
                ToolChange(ToolType.Pointer);
                return;
            }

            if (m_bCut)
            {
                m_ptLastDragPoint = e.GetPosition(TeachingCanvas);
            }

            if ((Keyboard.IsKeyDown(Key.Space) || TeachingCanvas.Tool == ToolType.Move) && e.ChangedButton == MouseButton.Left)
            {
                m_ptLastDragPoint = e.GetPosition(svTeaching);
            }
            else if (IsShowPaintState)
            {
                // 전체영상에 그리기를 시도하는 경우, 섹션 생성 요청 작업에 해당된다.
                // Mouse Up 이벤트 시점에서 bool 값을 통해 처리하여 준다.
                if (lbSection.SelectedIndex < 0)
                {
                    if (BasedROICanvas.Tool == ToolType.Rectangle)
                    {
                        TeachingCanvas.RegionType = GraphicsRegionType.UnitRegion;
                        m_bFiduRegionDrawing = true;
                    }
                    else if (BasedROICanvas.Tool == ToolType.PSROdd)
                    {
                        TeachingCanvas.RegionType = GraphicsRegionType.PSROdd;
                        m_bFiduRegionDrawing = true;
                    }
                    else if (BasedROICanvas.Tool == ToolType.Outer)
                    {
                        TeachingCanvas.RegionType = GraphicsRegionType.OuterRegion;
                        m_bFiduRegionDrawing = true;
                    }
                    else if (BasedROICanvas.Tool == ToolType.Rawmetrial)
                    {
                        TeachingCanvas.RegionType = GraphicsRegionType.Rawmetrial;
                        m_bFiduRegionDrawing = true;
                    }
                    else if (BasedROICanvas.Tool == ToolType.StripOrigin)
                    {
                        TeachingCanvas.RegionType = GraphicsRegionType.StripOrigin;
                    }
                    else if (BasedROICanvas.Tool == ToolType.IDMark)
                    {
                        TeachingCanvas.RegionType = GraphicsRegionType.IDRegion;
                        m_bFiduRegionDrawing = true;
                    }
                    else if (BasedROICanvas.Tool == ToolType.UnitPitch)
                    {
                        m_tmpPoint = Mouse.GetPosition(TeachingImage);
                        m_bPitchDrawing = true;
                    }
                    else if (BasedROICanvas.Tool == ToolType.BlockGap)
                    {
                        m_tmpPoint = Mouse.GetPosition(TeachingImage);
                        m_bBlockDrawing = true;
                    }
                    UpdateReferenceLabel();
                }
                else if (lbSection.SelectedIndex != -1)
                {
                    SectionInformation selectedSection = lbSection.SelectedItem as SectionInformation;
                    int nChannel = 0;
                    if (RGB)
                        nChannel = (int)SelectedChannel;
                    if (selectedSection != null)
                    {
                        if (!selectedSection.IsTempView[nChannel])
                            txtReferenceLabel.Visibility = Visibility.Visible;
                        else
                            txtReferenceLabel.Visibility = Visibility.Hidden;

                        if (TeachingCanvas.Tool == ToolType.AlignPattern)
                        {
                            TeachingCanvas.RegionType = GraphicsRegionType.UnitAlign;
                            m_bIsUnitAlignDrawing = true;
                        }

                        if (TeachingCanvas.Tool == ToolType.IDMark)
                        {
                            TeachingCanvas.RegionType = GraphicsRegionType.IDMark;
                            m_bIDMarkRegionDrawing = true;
                        }

                        if (TeachingCanvas.Tool == ToolType.WPShift)
                        {
                            TeachingCanvas.RegionType = GraphicsRegionType.WPShift;
                            m_bIsWPShiftDrawing = true;
                        }

                        if (TeachingCanvas.Tool == ToolType.Rectangle)
                        {
                            m_blsRectangleDrawing = true;
                        }
                       
                        if (TeachingCanvas.Tool == ToolType.Pointer)
                        {
                            m_blsRectangleDrawing = true;
                        }
                    }
                }

                CreateInspectContextMenu(); // 우클릭 팝업 검사 설정 항목 생성.
                TeachingCanvas.DrawingCanvas_MouseDown(sender, e);
            }
        }

        private void CreateInspectContextMenu()
        {
            SectionInformation selectedSection = SelectedSection;
            if (selectedSection != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    foreach (object item in selectedSection.ROICanvas[i].contextMenu.Items)
                    {
                        if (item is MenuItem)
                        {
                            MenuItem menuItem = item as MenuItem;
                            if (menuItem != null && (ContextMenuCommand)menuItem.Tag == ContextMenuCommand.AddInspectItem)
                            {
                                menuItem.Items.Clear();

                                List<InspectionType> inspectionTypeList;
                                if (selectedSection.Type.Code == SectionTypeCode.STRIP_REGION)
                                {
                                    inspectionTypeList = InspectionType.GetStripAlignInspectionTypeList();
                                }
                                else
                                {
                                    inspectionTypeList = InspectionType.GetDefaultInspectionTypeList();
                                }

                                foreach (InspectionType inspectionType in inspectionTypeList)
                                {
                                    MenuItem subMenuItem = new MenuItem() { Header = inspectionType.Name, Tag = inspectionType.Name };
                                    subMenuItem.Click += AddInspectItem;
                                    menuItem.Items.Add(subMenuItem);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddInspectItem(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                InspectionType selectedItem = InspectionType.GetInspectionType(menuItem.Tag.ToString());
                m_ptrTeachingWindow.InspectionTypectrl.cmbInspectionType.SelectedItem = selectedItem;
                m_ptrTeachingWindow.InspectionTypectrl.lbInspection.SelectedIndex = -1;
                m_ptrTeachingWindow.InspectionTypectrl.AddInspectionItem(selectedItem);
            }
        }

        private void pnlOuter_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (TeachingCanvas == null) return;
            int nChannel = (int)SelectedChannel;

            m_MousePoint = Mouse.GetPosition(this);
            if (!RGB) nChannel = 0;
            if (m_bPaste)
            {
                Point p = e.GetPosition(TeachingCanvas);
                System.Drawing.Rectangle rct = new System.Drawing.Rectangle((int)p.X, (int)p.Y, (int)cutBS.Width, (int)cutBS.Height);

                Algo a = new Algo();

                BitmapSource bs = a.SetROIImage(m_SectionManager.Sections[lbSection.SelectedIndex].GetBitmapSource(nChannel), cutBS, rct);
                cvsCross.Children.Remove(cutImage);
                m_SectionManager.Sections[lbSection.SelectedIndex].SetTempBitmapSource(bs, nChannel);// = bs;
                this.ThumbnailViewer.SetSourceImage(this);
                this.txtReferenceLabel.Visibility = Visibility.Hidden;
                m_bCut = false;
                m_bPaste = false;
                ShowPaint();
            }
            if (m_bCut)
            {
                Point p = e.GetPosition(TeachingCanvas);
                System.Windows.Rect rct = new Rect((Point)m_ptLastDragPoint, p);
                Algo a = new Algo();
                cutBS = null;
                cutBS = a.GetROIImage(m_SectionManager.Sections[lbSection.SelectedIndex].GetBitmapSource(nChannel), new System.Drawing.Rectangle((int)(rct.X), (int)(rct.Y),
                                                                                             (int)(rct.Width), (int)(rct.Height)));
                cutImage = new Image();
                cutImage.Stretch = Stretch.Fill;
                cutImage.Width = rct.Width * this.TeachingCanvas.ActualScale;
                cutImage.Height = rct.Height * this.TeachingCanvas.ActualScale;
                cutImage.Opacity = 0.7;
                cutImage.Source = cutBS;
                Point pp = e.GetPosition(cvsCross);
                Canvas.SetLeft(cutImage, pp.X);
                Canvas.SetTop(cutImage, pp.Y);
                cvsCross.Children.Add(cutImage);
                m_bCut = false;
                m_bPaste = true;
            }
            if (TeachingCanvas.Tool == ToolType.Move) m_ptLastDragPoint = null;
            else if (IsShowPaintState)
            {
                // Apply Image Processing
                #region 이진화, 확장, 축소 처리.
                if (chkBinarization.IsChecked == true)
                {
                    if (TeachingCanvas.SelectedGraphic != null)
                    {
                        if (radSingleThreshold.IsChecked == true)
                        {
                            m_ptrTeachingWindow.HistogramCtrl.EnableBinarization((int)sldrThreshold.Value, (int)sldrThreshold.Value, true);
                            Binarization();
                        }
                        else
                        {
                            m_ptrTeachingWindow.HistogramCtrl.EnableBinarization((int)sldrLowerThreshold.Value, (int)sldrUpperThreshold.Value, false);
                            Binarization();
                        }
                    }
                    else
                        TeachingImage.Source = TeachingImageSource;
                }
                else
                {
                    m_ptrTeachingWindow.HistogramCtrl.HideThresholdGuideLine();
                }
                #endregion
                // End of Apply Image Processing

                // TeachingCanvas.DrawingCanvas_MouseUp(sender, e);
                if (m_bFiduRegionDrawing) // 기준 Section을 그린 경우 사이즈 값, 반복 값을 지정해준다.
                {
                    if (BasedROICanvas.SelectedGraphic != null && BasedROICanvas.FiduGraphicsList.Count <= BasedROICanvas.MaxGraphicsCount)
                    {
                        GraphicsRectangle sectionROI = BasedROICanvas.SelectedGraphic as GraphicsRectangle;

                        if (sectionROI != null)
                        {
                            //PSR이물검사 ROI 16배수 보정
                            if (sectionROI.RegionType == GraphicsRegionType.PSROdd)
                            {
                                int x = (int)sectionROI.Rectangle.Right;
                                if (x%16 > 8)
                                {
                                    x = x - (16 - (x % 16));
                                } 
                                else
                                {
                                    x = x - x % 16;
                                }
                                sectionROI.UpdateRect(sectionROI.Rectangle.Left, sectionROI.Rectangle.Top, x, sectionROI.Rectangle.Bottom);
                            }

                            if (sectionROI.RegionType == GraphicsRegionType.UnitRegion || sectionROI.RegionType == GraphicsRegionType.PSROdd) // Unit Region
                            {
                                // Unit Section 설정은 중복으로 이루어질 수 없다.
                                bool bUnitRegionExists = false;
                                foreach (GraphicsRectangle rectGraphic in BasedROICanvas.FiduGraphicsList)
                                {
                                    if (rectGraphic.RegionType == GraphicsRegionType.UnitRegion || rectGraphic.RegionType == GraphicsRegionType.PSROdd)
                                    {
                                        bUnitRegionExists = true;
                                        break;
                                    }
                                }

                                if (!bUnitRegionExists)
                                {
                                    TeachingHelperWindow teachingHelperWindow = new TeachingHelperWindow(this, MainWindow.CurrentModel, BasedROICanvas, this.ID) { Owner = m_ptrTeachingWindow };
                                    if ((bool)teachingHelperWindow.ShowDialog())
                                    {
                                        SearchROIPattern(teachingHelperWindow.CurrentROI, teachingHelperWindow.SectionSymmetryType, teachingHelperWindow.Align);
                                        teachingHelperWindow.CurrentROI.IsSelected = true;
                                        TeachingWindow.Symmetry = teachingHelperWindow.SectionSymmetryType;
                                    }
                                }
                                else BasedROICanvas.GraphicsList.Remove(sectionROI);
                            }
                            else if (sectionROI.RegionType == GraphicsRegionType.IDRegion) // Outer Region
                            {
                                sectionROI.RegionType = GraphicsRegionType.IDRegion;
                                sectionROI.IsFiducialRegion = true;
                                sectionROI.IsValidRegion = true;
                                sectionROI.IterationXPosition = 0;
                                sectionROI.IterationYPosition = 0;
                                sectionROI.OriginObjectColor = sectionROI.ObjectColor = GraphicsColors.Green;
                                sectionROI.Caption = CaptionHelper.FiducialIDRegionCaption;
                                sectionROI.MotherROIID = -1;
                                BasedROICanvas.FiduGraphicsList.Add(sectionROI);
                            }
                            else if (sectionROI.RegionType == GraphicsRegionType.OuterRegion) // Outer Region
                            { 
                                MainWindow.IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(ID);
                                OuterSectionWindow outerSectionWindow = new OuterSectionWindow(this, MainWindow.CurrentModel, BasedROICanvas, IndexInfo.CategorySurface) { Owner = m_ptrTeachingWindow };
                                if ((bool)outerSectionWindow.ShowDialog())
                                {
                                    if(outerSectionWindow.nType == 0)
                                    {
                                        sectionROI.RegionType = GraphicsRegionType.OuterRegion;
                                        sectionROI.IsFiducialRegion = true;
                                        sectionROI.IsValidRegion = true;
                                        sectionROI.IterationXPosition = 0;
                                        sectionROI.IterationYPosition = 0;
                                        sectionROI.OriginObjectColor = sectionROI.ObjectColor = GraphicsColors.Green;
                                        sectionROI.Caption = CaptionHelper.FiducialOuterRegionCaption;
                                        sectionROI.MotherROIID = -1;
                                        BasedROICanvas.FiduGraphicsList.Add(sectionROI);
                                    }
                                    else
                                    {
                                        SearchROIOuterPattern(outerSectionWindow.CurrentROI, outerSectionWindow.nType);
                                        outerSectionWindow.CurrentROI.IsSelected = true;
                                    }
     

                                }


                            }
                            else if (sectionROI.RegionType == GraphicsRegionType.Rawmetrial) // Outer Region
                            {
                                sectionROI.RegionType = GraphicsRegionType.Rawmetrial;
                                GraphicsRectangle CurrentROI = null;
                                if (BasedROICanvas != null && BasedROICanvas.SelectedGraphic != null)
                                {
                                    CurrentROI = BasedROICanvas.SelectedGraphic as GraphicsRectangle;
                                    double fBlockgap = MainWindow.CurrentModel.Strip.BlockGap / CamResolutionY * 1000 * ReferenceImageScale;
                                    double fUnitXPitch = MainWindow.CurrentModel.Strip.UnitHeight / CamResolutionX * 1000 * ReferenceImageScale;
                                    double fUnitYPitch = MainWindow.CurrentModel.Strip.UnitWidth / CamResolutionY * 1000 * ReferenceImageScale;
                                    CurrentROI.BlockIterationValue = new IterationInformation(MainWindow.CurrentModel.Strip.Block, 1, 1, fBlockgap, 0, 0);
                                    CurrentROI.IterationValue = new IterationInformation(MainWindow.CurrentModel.Strip.UnitRow, MainWindow.CurrentModel.Strip.UnitColumn, fUnitXPitch, fUnitYPitch);
                                    CurrentROI.IsValidRegion = true;
                                    CurrentROI.IsFiducialRegion = true;
                                    CurrentROI.IterationXPosition = 0;
                                    CurrentROI.IterationYPosition = 0;
                                    CurrentROI.SymmetryValue.StartX = 1;
                                    CurrentROI.SymmetryValue.StartY = 1;
                                    CurrentROI.SymmetryValue.JumpX = 0;
                                    CurrentROI.SymmetryValue.JumpY = 1;
                                    SearchROIPattern(CurrentROI, 2);
                                    CurrentROI.IsSelected = true;
                                }
                            }
                        }
                    }
                    m_bFiduRegionDrawing = false;
                }
                else if (m_bIsUnitAlignDrawing)
                {
                    SectionInformation selectedSection = lbSection.SelectedItem as SectionInformation;
                    GraphicsRegionType regionType = GraphicsRegionType.UnitAlign;
                    if (selectedSection != null)
                    {
                        if (selectedSection.Type.Code == SectionTypeCode.OUTER_REGION)
                            regionType = GraphicsRegionType.OuterRegion;
                        else if (selectedSection.Type.Code == SectionTypeCode.RAW_REGION)
                            regionType = GraphicsRegionType.Rawmetrial;
                        else if (selectedSection.Type.Code == SectionTypeCode.STRIP_REGION)
                            regionType = GraphicsRegionType.StripAlign;
                        else if (selectedSection.Type.Code == SectionTypeCode.ID_REGION)
                            regionType = GraphicsRegionType.IDRegion;
                        else regionType = GraphicsRegionType.UnitAlign;
                    }


                    TeachingAlignSettingWindow teachingAlignSettingWindow = new TeachingAlignSettingWindow(TeachingCanvas, regionType) { Owner = m_ptrTeachingWindow };
                    teachingAlignSettingWindow.ShowDialog();

                    if (m_bIsUnitAlignDrawing)
                        m_bIsUnitAlignDrawing = false;
                }
                else if (m_bIDMarkRegionDrawing)
                {
                    SectionInformation selectedSection = lbSection.SelectedItem as SectionInformation;
                    GraphicsRectangle CurrentROI = TeachingCanvas.SelectedGraphic as GraphicsRectangle;
                    IDAreaProperty idproperty = new IDAreaProperty { ThreshType = 0 };
                    CurrentROI.InspectionList.Add(new InspectionItem(InspectionType.GetInspectionType(eVisInspectType.eInspTypeIDMark), idproperty, 1));
                    if (m_bIDMarkRegionDrawing)
                        m_bIDMarkRegionDrawing = false;
                }
                else if (m_bIsWPShiftDrawing)
                {
                    SectionInformation selectedSection = lbSection.SelectedItem as SectionInformation;
                    GraphicsRegionType regionType = GraphicsRegionType.WPShift;

                    TeachingAlignSettingWindow teachingAlignSettingWindow = new TeachingAlignSettingWindow(TeachingCanvas, regionType) { Owner = m_ptrTeachingWindow };
                    teachingAlignSettingWindow.ShowDialog();

                    if (m_bIsWPShiftDrawing)
                        m_bIsWPShiftDrawing = false;
                }

                else if (m_bPitchDrawing)
                {
                    m_bPitchDrawing = false;
                    Point p = Mouse.GetPosition(TeachingImage);
                    m_tmpPoint.X = (p.X - m_tmpPoint.X);
                    m_tmpPoint.Y = (p.Y - m_tmpPoint.Y);
                    PitchSetWindow pitchWindow = new PitchSetWindow(this, MainWindow.CurrentModel, m_tmpPoint, true, this.ID);
                    if ((bool)pitchWindow.ShowDialog())
                    {
                        //
                    }
                    // SetPosition.Y = (p.Y - SetPosition.Y) * this.ActualScale;
                }
                else if (m_bBlockDrawing)
                {
                    m_bBlockDrawing = false;
                    Point p = Mouse.GetPosition(TeachingImage);
                    m_tmpPoint.X = (p.X - m_tmpPoint.X);
                    m_tmpPoint.Y = (p.Y - m_tmpPoint.Y);
                    PitchSetWindow pitchWindow = new PitchSetWindow(this, MainWindow.CurrentModel, m_tmpPoint, false, this.ID);
                    if ((bool)pitchWindow.ShowDialog())
                    {
                        //
                    }
                    // SetPosition.Y = (p.Y - SetPosition.Y) * this.ActualScale;
                }
                //PSR이물검사 ROI 16배수 보정
                else if (m_blsRectangleDrawing)
                {
                    SectionInformation selectedSection = lbSection.SelectedItem as SectionInformation;

                    if (selectedSection.Type.Code == SectionTypeCode.PSR_REGION)
                    {
                        Make_ROI_Width_Multiple16();
                    }
                    if (m_blsRectangleDrawing)
                        m_blsRectangleDrawing = false;
                }

                if (TeachingCanvas.DrawingFinished)
                {
                    ToolTypeChangeEventHandler eventRunner = ToolTypeChangeEvent;
                    if (eventRunner != null)
                    {
                        eventRunner(ToolType.Pointer);
                    }
                }
                TeachingCanvas.UpdateSelectedGraphic();
            }
        }

        //private Rect CalcPSROddRect(Rect rect)
        //{
        //    TeachingHelperWindow
        //}

        private void pnlOuter_MouseMove(object sender, MouseEventArgs e)
        {
            if (TeachingImageSource == null || TeachingCanvas == null || TeachingImage == null)
                return;
            try
            {
                System.Windows.Point ptCurrentByViewer = Mouse.GetPosition(svTeaching);
                System.Windows.Point ptCurrentByImage = Mouse.GetPosition(TeachingImage);

                UpdateIndicator();

                if (TeachingImageSource != null)
                {
                    if (m_bPaste)
                    {
                        Point pp = e.GetPosition(cvsCross);
                        Canvas.SetLeft(cutImage, pp.X);
                        Canvas.SetTop(cutImage, pp.Y);
                    }

                    // GV 표시는 등록된 이미지위 범위 내에서만 동작하도록 한다.
                    if (!(ptCurrentByImage.X > TeachingImage.ActualWidth) &&
                        !(ptCurrentByImage.Y > TeachingImage.ActualHeight))
                    {
                        // Calculate GV Value.
                        byte[] pixel = new byte[1];

                        TeachingImageSource.CopyPixels(new Int32Rect((int)ptCurrentByImage.X, (int)ptCurrentByImage.Y, 1, 1),
                                                          pixel, SourceWidth, 0);

                        // Update X, Y, GV
                        txtGVValue.Text = pixel[0].ToString();
                        txtXPosition.Text = Convert.ToInt32(ptCurrentByImage.X).ToString();
                        txtYPosition.Text = Convert.ToInt32(ptCurrentByImage.Y).ToString();

                        #region Unused Code. (Update X, Y by (mm))
                        //if (ptCurrentByImage.X != 0)
                        //{
                        //    txtXPositionMM.Text = string.Format("{0:f2}", Convert.ToDouble(ptCurrentByImage.X * CamResolutionX / 1000 / m_fReferenceImageScale));
                        //}
                        //if (ptCurrentByImage.Y != 0)
                        //{
                        //    txtYPositionMM.Text = string.Format("{0:f2}", Convert.ToDouble(ptCurrentByImage.Y * CamResolutionY / 1000 / m_fReferenceImageScale));
                        //}
                        #endregion

                        // Draw Line profile.
                        if (TeachingCanvas.Tool == ToolType.Pointer && Mouse.LeftButton == MouseButtonState.Released)
                        {
                            double fScale = SourceHeight / TeachingImage.ActualHeight;
                            m_ptrTeachingWindow.LineProfileCtrl.DrawLineProfile(BitmapSourceHelper.GetLinePixels(TeachingImageSource, Convert.ToInt32(ptCurrentByImage.Y * fScale)));
                        }
                    }
                    else
                    {
                        this.txtGVValue.Text = "0";
                    }
                }

                if ((TeachingCanvas.Tool == ToolType.Move || (Keyboard.IsKeyDown(Key.Space) && Mouse.LeftButton == MouseButtonState.Pressed)) && m_ptLastDragPoint != null)
                {
                    double fdeltaX = ptCurrentByViewer.X - m_ptLastDragPoint.Value.X;
                    double fdeltaY = ptCurrentByViewer.Y - m_ptLastDragPoint.Value.Y;

                    svTeaching.ScrollToHorizontalOffset(svTeaching.HorizontalOffset - fdeltaX);
                    svTeaching.ScrollToVerticalOffset(svTeaching.VerticalOffset - fdeltaY);

                    m_ptLastDragPoint = ptCurrentByViewer;
                }
                else
                {
                    TeachingCanvas.DrawingCanvas_MouseMove(sender, e);
                }
            }
            catch { }
        }

        private void pnlOuter_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.Space))
            {
                m_ptLastContentMousePosition = GetContentMousePosition();
                if (e.Delta != 0)
                    Zoom(e.Delta);
                m_ptLastCenterOfViewport = GetCenterOfViewport();
                ThumbnailViewer.UpdateNavigator();

                e.Handled = true; // blocking wheel event of svImageViewer control.
            }
        }

        private void Zoom(double deltaValue)
        {
            // ROI가 많을 경우 속도 저하 발생하므로, Zoom Scale Frequency를 임의로 키운다.
            if (deltaValue > 0)
            {
                if (TeachingCanvas != null && TeachingCanvas.GraphicsList.Count > 2000)
                    ZoomValue *= 2.0;
                else
                    ZoomValue *= 1.1;
            }
            else if (deltaValue == 0)
            {
                ZoomValue = m_fZoomToFitScale;
            }
            else
            {
                if (TeachingCanvas != null && TeachingCanvas.GraphicsList.Count > 2000)
                    ZoomValue = (ZoomValue / 2.0 < m_fZoomToFitScale) ? m_fZoomToFitScale : ZoomValue / 2.0;
                else
                    ZoomValue = (ZoomValue / 1.1 < m_fZoomToFitScale) ? m_fZoomToFitScale : ZoomValue / 1.1;
            }
        }

        private void pnlOuter_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (TeachingCanvas == null)
            {
                return;
            }

            TeachingCanvas.DrawingCanvas_LostMouseCapture(sender, e);
        }
        #endregion

        #region Zooming function.
        private void sldrScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ZoomValue = sldrScale.Value;
        }

        private void zoomBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender as Button == null)
            {
                return;
            }

            string strTag = (sender as Button).Tag.ToString();

            if (!string.IsNullOrEmpty(strTag))
            {
                if (strTag == "IN")
                {
                    Zoom(100);
                }
                else if (strTag == "OUT")
                {
                    Zoom(-100);
                }
                else// ZOOM_TO_FIT
                {
                    Zoom(0);
                }
            }
        }

        public void CalculateZoomToFitScale()
        {
            double fnumerator = 1.0;
            double fdenominator = 1.0;
          //  ViewerWidth = this.svTeaching.ActualWidth;
           // ViewerHeight = this.svTeaching.ActualHeight;
            if (SourceHeight / ViewerHeight > SourceWidth / ViewerWidth)
            {
                fnumerator = ViewerHeight;
                fdenominator = SourceHeight;
            }
            else
            {
                fnumerator = ViewerWidth;
                fdenominator = SourceWidth;
            }
            m_fZoomToFitScale = fnumerator / fdenominator * 0.975;

            ZoomValue = m_fZoomToFitScale;
            sldrScale.Minimum = (m_fZoomToFitScale > 0) ? m_fZoomToFitScale : 0.05;
        }

        public void SetZoomToFit()
        {
            ZoomValue = m_fZoomToFitScale;
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

        private System.Windows.Point GetCenterOfViewport()
        {
            if (TeachingImage == null)
            {
                return new System.Windows.Point(0, 0);
            }
            else
            {
               // ViewerWidth = this.svTeaching.ActualWidth;
              //  ViewerHeight = this.svTeaching.ActualHeight;
                System.Windows.Point ptCenterOfViewport = new System.Windows.Point(ViewerWidth / 2, ViewerHeight / 2);
                System.Windows.Point ptTranslatedCenterOfViewport = svTeaching.TranslatePoint(ptCenterOfViewport, TeachingImage);

                return ptTranslatedCenterOfViewport;
            }
        }
        #endregion

        #region Crosscanvas events.
        private static Line m_horizontalLine = new Line() { Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 1.0 };
        private static Line m_verticalLine = new Line() { Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 1.0 };

        private void InitializeIndicator()
        {
            m_horizontalLine.X1 = 0;
            m_horizontalLine.Y1 = -1;
            m_horizontalLine.X2 = cvsCross.ActualWidth;
            m_horizontalLine.Y2 = -1;

            m_verticalLine.X1 = -1;
            m_verticalLine.Y1 = 0;
            m_verticalLine.X2 = -1;
            m_verticalLine.Y2 = cvsCross.ActualHeight;
        }

        private void UpdateIndicator()
        {
            System.Windows.Point ptCrossCanvas = Mouse.GetPosition(cvsCross);

            m_verticalLine.X1 = ptCrossCanvas.X;
            m_verticalLine.X2 = ptCrossCanvas.X;

            m_horizontalLine.Y1 = ptCrossCanvas.Y;
            m_horizontalLine.Y2 = ptCrossCanvas.Y;

            m_horizontalLine.X2 = cvsCross.ActualWidth;
            m_verticalLine.Y2 = cvsCross.ActualHeight;
        }

        private void CrossCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            InitializeIndicator();

            cvsCross.Children.Add(m_horizontalLine);
            cvsCross.Children.Add(m_verticalLine);

            if (TeachingCanvas != null)
            {
                if (TeachingCanvas.Tool == ToolType.Move)
                {
                    this.Cursor = System.Windows.Input.Cursors.ScrollAll;
                    ToolChange(ToolType.Move);
                }
                else
                {
                    this.Cursor = TeachingCanvas.Cursor;
                }
            }
        }

        private void CrossCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            cvsCross.Children.Remove(m_horizontalLine);
            cvsCross.Children.Remove(m_verticalLine);

            this.Cursor = Cursors.Arrow;
        }
        #endregion

        #region Show & Hidden ROI.
        private void SetROIDrawingMode(bool showPaint)
        {
            if (showPaint)
            {
                ShowPaint();
            }
            else
            {
                HiddenPaint();
            }
        }

        /// <summary>   Shows the paint. </summary>
        /// <remarks>   suoow2, 2014-08-22. </remarks>
        public void ShowPaint()
        {
            IsShowPaintState = true;

            if (TeachingCanvas != null)
            {
                TeachingCanvas.Visibility = Visibility.Visible;
            }
        }

        /// <summary>   Hidden paint. </summary>
        /// <remarks>   suoow2, 2014-08-22. </remarks>
        public void HiddenPaint()
        {
            IsShowPaintState = false;

            if (TeachingCanvas != null)
            {
                TeachingCanvas.Visibility = Visibility.Hidden;

                if (TeachingCanvas.Tool > ToolType.Pointer)
                {
                    ToolChange(ToolType.Pointer);
                }
            }
        }
        #endregion

        #region Other functions.
        private void UpdateScale()
        {
            DrawingCanvas drawingCanvas = TeachingCanvas;
            if (drawingCanvas != null)
                drawingCanvas.ActualScale = ZoomValue;
        }

        // 기준 영상 라벨 표시 업데이트.
        public void UpdateReferenceLabel()
        {
            if (lbSection.SelectedIndex == -1)
            {
                if (IsReferenceView)
                    txtReferenceLabel.Visibility = Visibility.Visible;
                else
                    txtReferenceLabel.Visibility = Visibility.Hidden;
            }
            else
            {
                SectionInformation section = SelectedSection;
                if (section != null)
                    txtReferenceLabel.Visibility = (!section.IsTempView[(int)SelectedChannel]) ? Visibility.Visible : Visibility.Hidden;
                else
                    txtReferenceLabel.Visibility = Visibility.Hidden;
            }
        }

        public void UpdateViewerSource(BitmapSource aBitmapSource)
        {
            if (aBitmapSource != null)
            {
                BasedROICanvas.Width = BasedImage.Width = aBitmapSource.PixelWidth;
                BasedROICanvas.Height = BasedImage.Height = aBitmapSource.PixelHeight;
                BasedImage.Source = aBitmapSource;
                CalculateZoomToFitScale();
            }
            else
            {
                BasedROICanvas.Width = BasedImage.Width = 0;
                BasedROICanvas.Height = BasedImage.Height = 0;
                BasedImage.Source = null;
            }
        }

        private void lbSection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            #region Set teaching type.
            TeachingType teachingType = TeachingType.NONE;

            int nSelectedIndex = lbSection.SelectedIndex;

            bool bManu_PSRodd = true;
            MainWindow.IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(ID);

            if (nSelectedIndex != -1)
            {
                chkFixedROI.IsChecked = DrawingCanvas.FixedInspectROI; // ROI 고정 여부 판단.
                btnEditImage.IsEnabled = true;
                m_fReferenceImageScale = 1.0;
                int nChannel = (int)SelectedChannel;
                if (!RGB)
                    nChannel = 0;


                SectionInformation section = lbSection.SelectedItem as SectionInformation;

                //sort
                section.SectionRegionList = section.SectionRegionList.OrderBy(x => x.RegionIndex.X).ThenBy(x => x.RegionIndex.Y).ToList();

                if (section != null)
                {
                    foreach (GraphicsBase g in section.ROICanvas[nChannel].GraphicsList)
                    {
                        // 2012-03-25 suoow2.
                        // Actual Scale이 틀어지는 경우가 발생하여 동기화 시켜주도록 조치함.
                        if (g.ActualScale != section.ROICanvas[nChannel].ActualScale)
                            g.ActualScale = section.ROICanvas[nChannel].ActualScale;
                    }

                    if (section.Type.Code == SectionTypeCode.STRIP_REGION)
                    {
                        #region STRIP_REGION
                        teachingType = TeachingType.StripAlign;
                        m_ptrTeachingWindow.InspectionTypectrl.ChangeInspectionType(Common.Drawing.InspectionInformation.eSectionType.eSecTypeGlobal);
                        m_ptrTeachingWindow.DisableViewUnitFunction();

                        m_ptrTeachingWindow.cmbRegionList.Items.Clear();
                        m_ptrTeachingWindow.cmbRegionList.IsEnabled = false;

                        m_ptrTeachingWindow.btnPartialInspect.IsEnabled = IsSentDone;
                        m_ptrTeachingWindow.btnViewUnit.IsEnabled = false;
                        #endregion
                    }
                    else if (section.Type.Code == SectionTypeCode.UNIT_REGION)
                    {
                        #region UNIT_REGION
                        teachingType = TeachingType.UnitRegion;
                        m_ptrTeachingWindow.InspectionTypectrl.ChangeInspectionType(Common.Drawing.InspectionInformation.eSectionType.eSecTypeUnit);

                        m_ptrTeachingWindow.cmbRegionList.Items.Clear();
                        m_ptrTeachingWindow.cmbRegionList.IsEnabled = true;

                        foreach (SectionRegion region in section.SectionRegionList)
                        {
                            // 수동 검사에서 개별 유닛 보기에 사용되는 Combobox Item을 set 하여줌.
                            if (this.SelectedSection.Type.Code == SectionTypeCode.UNIT_REGION)
                            {
                                if (IndexInfo.CategorySurface == CategorySurface.BP)
                                {
                                    if ((int)IndexInfo.Surface % 2 == 1) // BP1
                                    {
                                        m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", MainWindow.CurrentModel.Strip.UnitRow - region.RegionIndex.X, region.RegionIndex.Y + 1));
                                    }
                                    else // BP2
                                    {
                                        m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));
                                    }

                                }
                                else if (IndexInfo.CategorySurface == CategorySurface.CA)
                                {
                                    m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", MainWindow.CurrentModel.Strip.UnitRow - region.RegionIndex.X, region.RegionIndex.Y + 1));
                                }
                                else  //BA
                                {
                                    m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));
                                }
                            }
                            else
                            {
                                m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));
                            }
                        }
                        m_ptrTeachingWindow.cmbRegionList.SelectedIndex = 0;

                        m_ptrTeachingWindow.btnPartialInspect.IsEnabled = IsSentDone;
                        m_ptrTeachingWindow.btnViewUnit.IsEnabled = IsSentDone;
                        #endregion
                    }
                    else if (section.Type.Code == SectionTypeCode.PSR_REGION)
                    {
                        #region UNIT_REGION
                        teachingType = TeachingType.PsrRegion;
                        m_ptrTeachingWindow.InspectionTypectrl.ChangeInspectionType(Common.Drawing.InspectionInformation.eSectionType.eSecTypeRegionPsr);

                        m_ptrTeachingWindow.cmbRegionList.Items.Clear();
                        m_ptrTeachingWindow.cmbRegionList.IsEnabled = true;

                        foreach (SectionRegion region in section.SectionRegionList)
                        {
                            // 수동 검사에서 개별 유닛 보기에 사용되는 Combobox Item을 set 하여줌.
                            if (this.SelectedSection.Type.Code == SectionTypeCode.PSR_REGION)
                            {
                                //m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));

                                if (IndexInfo.CategorySurface == CategorySurface.BP)
                                {
                                    if ((int)IndexInfo.Surface % 2 == 1) // BP1
                                    {
                                        m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", MainWindow.CurrentModel.Strip.UnitRow - region.RegionIndex.X, region.RegionIndex.Y + 1));
                                    }
                                    else // BP2
                                    {
                                        m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));
                                    }

                                }
                                else if (IndexInfo.CategorySurface == CategorySurface.CA)
                                {
                                    m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", MainWindow.CurrentModel.Strip.UnitRow - region.RegionIndex.X, region.RegionIndex.Y + 1));
                                }
                                else  //BA
                                {
                                    m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));
                                }
                            }
                            else
                            {
                                m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));
                            }
                        }
                        m_ptrTeachingWindow.cmbRegionList.SelectedIndex = 0;

                        m_ptrTeachingWindow.btnPartialInspect.IsEnabled = IsSentDone;
                        m_ptrTeachingWindow.btnViewUnit.IsEnabled = IsSentDone;
                        #endregion
                    }
                    else if(section.Type.Code == SectionTypeCode.RAW_REGION) //RAW_REGION. OUTER_REGION
                    {
                        #region RAW_REGION
                        teachingType = TeachingType.RawRegion;
                        m_ptrTeachingWindow.InspectionTypectrl.ChangeInspectionType(Common.Drawing.InspectionInformation.eSectionType.eSecTypeRail);
                        m_ptrTeachingWindow.DisableViewUnitFunction();

                        m_ptrTeachingWindow.cmbRegionList.Items.Clear();
                        m_ptrTeachingWindow.cmbRegionList.IsEnabled = true;


                        foreach (SectionRegion region in section.SectionRegionList)
                        {
                            // 수동 검사에서 개별 유닛 보기에 사용되는 Combobox Item을 set 하여줌.
                            m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));
                        }
                        m_ptrTeachingWindow.cmbRegionList.SelectedIndex = 0;

                        m_ptrTeachingWindow.btnPartialInspect.IsEnabled = IsSentDone;
                        m_ptrTeachingWindow.btnViewUnit.IsEnabled = IsSentDone;
                        #endregion
                    }
                    else if(section.Type.Code == SectionTypeCode.OUTER_REGION)
                    {
                        #region OUTER_REGION
                        teachingType = TeachingType.RawRegion;
                        m_ptrTeachingWindow.InspectionTypectrl.ChangeInspectionType(Common.Drawing.InspectionInformation.eSectionType.eSecTypeRegionOuter);
                        m_ptrTeachingWindow.DisableViewUnitFunction();

                        m_ptrTeachingWindow.cmbRegionList.Items.Clear();
                        m_ptrTeachingWindow.cmbRegionList.IsEnabled = true;


                        foreach (SectionRegion region in section.SectionRegionList)
                        {
                            // 수동 검사에서 개별 유닛 보기에 사용되는 Combobox Item을 set 하여줌.
                            m_ptrTeachingWindow.cmbRegionList.Items.Add(string.Format("X:{0}  Y:{1}", region.RegionIndex.X + 1, region.RegionIndex.Y + 1));
                        }
                        m_ptrTeachingWindow.cmbRegionList.SelectedIndex = 0;

                        m_ptrTeachingWindow.btnPartialInspect.IsEnabled = IsSentDone;
                        m_ptrTeachingWindow.btnViewUnit.IsEnabled = IsSentDone;
                        #endregion
                    }

                    txtReferenceLabel.Visibility = (!section.IsTempView[nChannel]) ? Visibility.Visible : Visibility.Hidden;
                }
            }
            else // 전체 영상
            {
                DrawingCanvas.FixedSectionROI = true;
                chkFixedROI.IsChecked = DrawingCanvas.FixedSectionROI; // 전체 영상에서는 ROI 고정 checkbox가 선택되도록 함.
                btnEditImage.IsEnabled = false;
                m_ptrTeachingWindow.DisableViewUnitFunction();
                m_fReferenceImageScale = VisionDefinition.GRAB_IMAGE_SCALE;
                teachingType = TeachingType.Entire;
            }
            #endregion
            pnlInner.Children.Clear();
            pnlInner.Children.Add(TeachingImage); // Image Control
            pnlInner.Children.Add(TeachingCanvas); // Drawing Canvas

            TeachingCanvas.UnselectAll();
            TeachingCanvas.SelectedGraphic = null;
            TeachingCanvas.Focus();

            if (m_ptrTeachingWindow != null)
            {
                SetROIDrawingMode(!(bool)chkDisableROI.IsChecked);
                if (chkBinarization.IsChecked == true)
                    Binarization();
                else
                    m_ptrTeachingWindow.HistogramCtrl.HideThresholdGuideLine();
            }
            ToolChange(ToolType.Pointer);
            SetScrollViewerToHome();

            if (IndexInfo.CategorySurface == CategorySurface.BP)
                bManu_PSRodd = false;//BP에서는 PSR ODD 사용하면 안된다
            TeachingSubMenuCtrl.ChangeTeachingMode(teachingType, bManu_PSRodd);
            ThumbnailViewer.SetSourceImage(this);

            m_ptrTeachingWindow.ViewChanged = true;
            if (TeachingImage.Source != null)
                m_ptrTeachingWindow.LineProfileCtrl.SetLineProfileSource(TeachingImage.Source as BitmapSource);

            CalculateZoomToFitScale();
            UpdateScale();
            e.Handled = true;
        }

        #region Binarization by selected ROI.
        private void ClipPos(ref Int32Rect arcTarget, Size anBoundary)
        {
            if (arcTarget.X < 0)
                arcTarget.X = 0;
            if (arcTarget.X >= anBoundary.Width)
                arcTarget.X = (int)anBoundary.Width - 1;

            if (arcTarget.Y < 0)
                arcTarget.Y = 0;
            if (arcTarget.Y >= anBoundary.Height)
                arcTarget.Y = (int)anBoundary.Height - 1;

            if (arcTarget.X + arcTarget.Width >= anBoundary.Width)
                arcTarget.Width = (int)anBoundary.Width - arcTarget.X;
            if (arcTarget.Y + arcTarget.Height >= anBoundary.Height)
                arcTarget.Height = (int)anBoundary.Height - arcTarget.X;
        }

        public void Binarization(BitmapSource bitmapSource, int anLowerThreshold, int anUpperThreshold, int anErosionIter, int anDilationIter)
        {
            try
            {
                if (bitmapSource == null) return;
                int width = bitmapSource.PixelWidth;
                int height = bitmapSource.PixelHeight;

                GraphicsBase graphic = TeachingCanvas.SelectedGraphic;
                if (graphic == null)
                    return;

                Int32Rect region = new Int32Rect();
                if (graphic is GraphicsRectangleBase)
                {
                    region.X = (int)Math.Round(((GraphicsRectangleBase)graphic).Left);
                    region.Y = (int)Math.Round(((GraphicsRectangleBase)graphic).Top);
                    region.Width = (int)Math.Round(((GraphicsRectangleBase)graphic).Right - ((GraphicsRectangleBase)graphic).Left);
                    region.Height = (int)Math.Round(((GraphicsRectangleBase)graphic).Bottom - ((GraphicsRectangleBase)graphic).Top);

                    ClipPos(ref region, new Size(width, height));
                }
                else if (graphic is GraphicsPolyLine)
                {
                    region.X = ((GraphicsPolyLine)graphic).LeftProperty;
                    region.Y = ((GraphicsPolyLine)graphic).TopProperty;
                    region.Width = (int)Math.Round(((GraphicsPolyLine)graphic).WidthProperty) - 1;
                    region.Height = (int)Math.Round(((GraphicsPolyLine)graphic).HeightProperty) - 1;

                    ClipPos(ref region, new Size(width, height));
                }

                if (region.Width <= 0 || region.Height <= 0) // check region.
                    return;
                m_Algo.SetImage(BitmapHelper.BitmapSource2Bitmap(bitmapSource));
                m_Algo.SetImageROI(new System.Drawing.Rectangle(region.X, region.Y, region.Width, region.Height));
                m_Algo.DoProcessing(anLowerThreshold, anUpperThreshold, anErosionIter, anDilationIter);
                m_Algo.ResetImageROI();
                TeachingImage.Source = m_Algo.GetImage();
            }
            catch
            {
                Debug.WriteLine("Exception ocured in Binarization(TeachingViewerCtrl.xaml.cs)");
            }
        }

        public void Binarization()
        {
            int nLowerThreshold, nUpperThreshold, nErosionIter, nDilationIter;

            if ((bool)radSingleThreshold.IsChecked)
            {
                nLowerThreshold = (int)sldrThreshold.Value;
                nUpperThreshold = 256;
            }
            else
            {
                nLowerThreshold = (int)sldrLowerThreshold.Value;
                nUpperThreshold = (int)sldrUpperThreshold.Value;
            }

            nErosionIter = (int)sldrErosionIter.Value;
            nDilationIter = (int)sldrDilationIter.Value;

            try
            {
                // 기준 영상이 아닌, Section 이미지에 대한 이진화 요청일 시 아래 코드로 분기하도록 함.
                if (lbSection.SelectedIndex != -1)
                {
                    Binarization(TeachingImageSource, nLowerThreshold, nUpperThreshold, nErosionIter, nDilationIter);
                }
                else
                {
                    // CHEKCK : 전체 영상일 경우 처리 시간이 이미지 로딩에 오래걸림
                    //          개선 방향은 Algo 클래스 구조에서 이중 버퍼 형태를 취해야 하며
                    //          원본을 처리하여 결과 이미지 버퍼에 쓰는 구조로 바꿔야 함
                    //          위와 같은 구조에서 매번 이미지 셋팅이 하는 것이 아니라 원 이미지
                    //          변경이 필요할 경우에만 셋팅해야 함으로 로딩 시간 단축 가능함
                    Binarization(TeachingImageSource, nLowerThreshold, nUpperThreshold, nErosionIter, nDilationIter);
                }
            }
            catch
            {
                Debug.WriteLine("Exception occured in Binarization(TeachingViewerCtrl.xaml.cs)");
            }
        }
        #endregion

        public void ToolChange(ToolType newTool)
        {
            m_bCut = false;
            m_bPaste = false;
            if (TeachingCanvas == null) return;
            this.m_ptLastDragPoint = null;

            if (chkDisableROI.IsChecked == true)
            {
                if (newTool == ToolType.Rectangle ||
                    newTool == ToolType.Ellipse ||
                    newTool == ToolType.PolyLine ||
                    newTool == ToolType.AlignPattern ||
                    newTool == ToolType.WPShift ||
                    newTool == ToolType.StripAlign ||
                    newTool == ToolType.StripOrigin ||
                    newTool == ToolType.IDMark ||
                    newTool == ToolType.Outer ||
                    newTool == ToolType.Rawmetrial ||
                    newTool == ToolType.PSROdd)
                {
                    chkDisableROI.IsChecked = false;
                    ShowPaint();
                }
            }
            TeachingCanvas.UnselectAll();
            TeachingCanvas.Tool = newTool;

            if (newTool == ToolType.AlignPattern)
            {
                if (lbSection.SelectedIndex < 0) // 전체 영상
                {
                    TeachingCanvas.RegionType = GraphicsRegionType.StripAlign;
                }
                else
                {
                    SectionInformation selectedSection = lbSection.SelectedItem as SectionInformation;
                    if (selectedSection != null)
                    {
                        if (selectedSection.Type.Code == SectionTypeCode.STRIP_REGION)
                            TeachingCanvas.RegionType = GraphicsRegionType.StripAlign;
                        else if (selectedSection.Type.Code == SectionTypeCode.UNIT_REGION)
                            TeachingCanvas.RegionType = GraphicsRegionType.UnitAlign;
                        else if (selectedSection.Type.Code == SectionTypeCode.PSR_REGION)
                            TeachingCanvas.RegionType = GraphicsRegionType.PSROdd;
                        else if (selectedSection.Type.Code == SectionTypeCode.OUTER_REGION)
                            TeachingCanvas.RegionType = GraphicsRegionType.OuterRegion;
                        else if (selectedSection.Type.Code == SectionTypeCode.RAW_REGION)
                            TeachingCanvas.RegionType = GraphicsRegionType.Rawmetrial;
                        else if (selectedSection.Type.Code == SectionTypeCode.ID_REGION)
                            TeachingCanvas.RegionType = GraphicsRegionType.IDRegion;
                    }
                }
            }
            else if (newTool == ToolType.CopyAndPaste)
            {
                HiddenPaint();
                TeachingCanvas.RegionType = GraphicsRegionType.None;
                TeachingCanvas.Tool = ToolType.Pointer;
                m_bCut = true;
                m_bPaste = false;
            }
            else if (newTool == ToolType.WPShift)
            {
                SectionInformation selectedSection = lbSection.SelectedItem as SectionInformation;
                if (selectedSection != null)
                {
                    if (selectedSection.Type.Code == SectionTypeCode.UNIT_REGION)
                        TeachingCanvas.RegionType = GraphicsRegionType.WPShift;
                }
            }
            else if (newTool == ToolType.GuideLine)
                TeachingCanvas.RegionType = GraphicsRegionType.GuideLine;
            else if (newTool == ToolType.TapeLocation)
                TeachingCanvas.RegionType = GraphicsRegionType.TapeLoaction;
            else
                TeachingCanvas.RegionType = GraphicsRegionType.Inspection;

            ToolTypeChangeEventHandler eventRunner = ToolTypeChangeEvent;
            if (eventRunner != null)
            {
                eventRunner(newTool);
            }
        }
        #endregion

        #region Display & Save Image.
        /// <summary>   Loads the image. </summary>
        /// <remarks>   suoow2, 2014-08-15. </remarks>
        public void LoadImage()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".bmp";
            dlg.Filter = "Bitmap Images (.bmp) | *.bmp";

            // Save Initial directory.
            string strOldInitialDirectory = dlg.InitialDirectory;

            string strParentPath = DirectoryManager.GetParentPath(MainWindow.StartUpPath);
            dlg.InitialDirectory = DirectoryManager.GetCombinedPathName(strParentPath, @"\Temp\BasedImage\");

            if ((bool)dlg.ShowDialog())
            {
                DisplayImage(dlg.FileName, (int)SelectedChannel);
            }
             
            // Restore Initial directory.
            dlg.InitialDirectory = strOldInitialDirectory;
        }

        private void DisplayImage(string aszFileName, int nChannel)
        {
            ToolChange(ToolType.Pointer);

            try
            {
                SectionInformation selectedSection = SelectedSection;

                if (!RGB) nChannel = 0;
                if (selectedSection != null)
                {
                    BitmapSource oldSectionImage = selectedSection.BitmapSource[nChannel];
                    BitmapSource bitmapSource = BitmapImageLoader.LoadCachedBitmapImage(new Uri(aszFileName)) as BitmapSource;
                    if (bitmapSource != null)
                    {
                        if (oldSectionImage != null &&
                            oldSectionImage.PixelWidth == bitmapSource.PixelWidth && oldSectionImage.PixelHeight == bitmapSource.PixelHeight)
                        {
                            selectedSection.SetBitmapSource(bitmapSource, nChannel);

                        }
                        else if (bitmapSource.PixelWidth == selectedSection.Region.Width && bitmapSource.PixelHeight == selectedSection.Region.Height)
                        {
                            selectedSection.SetBitmapSource(bitmapSource, nChannel);

                        }
                        else
                        {
                            MessageBox.Show("기존 Section의 크기와 영상의 크기가 다릅니다.\n영상을 여는데 실패하였습니다.", "Information");
                        }
                    }
                    else
                    {
                        MessageBox.Show(ResourceStringHelper.GetErrorMessage("I001"), "Error");
                    }
                }
                else // 전체 영상
                {
                    BitmapSource bitmapSource = BitmapImageLoader.LoadCachedBitmapImage(new Uri(aszFileName)) as BitmapSource;
                    if (bitmapSource != null)
                    {
                        ReferenceImage = bitmapSource;
                        UpdateViewerSource(ReferenceImage);
                    }
                }
            }
            catch
            {
                MessageBox.Show(ResourceStringHelper.GetErrorMessage("I001"), "Error");
            }
        }

        public void SaveImage()
        {
            string fileName = string.Empty;
            if (lbSection.SelectedIndex == -1)
            {
                fileName = GetTeachingFileName("Based", "bmp");
            }
            else if (SelectedSection != null)
            {
                fileName = GetTeachingFileName(SelectedSection.Name, "bmp");
            }

            if (TeachingImageSource != null)
            {
                ImageSave(TeachingImageSource, fileName);
            }
        }

        private void ImageSave(BitmapSource source, string fileName)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".bmp";
            dlg.Filter = "Bitmap Images (.bmp) | *.bmp";
            dlg.FileName = fileName;

            // Save Initial directory.
            string strOldInitialDirectory = dlg.InitialDirectory;
            string strParentPath = DirectoryManager.GetParentPath(MainWindow.StartUpPath);

            dlg.InitialDirectory = DirectoryManager.GetCombinedPathName(strParentPath, @"\Temp\BasedImage\");

            if ((bool)dlg.ShowDialog())
                FileSupport.SaveImageFile(dlg.FileName, source);

            // Restore Initial directory.
            dlg.InitialDirectory = strOldInitialDirectory;
        }
        #endregion

        #region File open & save
        public string GetSelectedSectionName()
        {
            SectionInformation selectedSection = lbSection.SelectedItem as SectionInformation;
            if (selectedSection != null)
            {
                return selectedSection.Name;
            }
            else
            {
                for(int i =0; i < MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count; i++)
                {
                    if (SurfaceType == Surface.BA1 + i) return "BondPad" + (i + 1) + "Based Region";
                }
                for (int i = 0; i < MainWindow.Setting.Job.CACount; i++)
                {
                    if (SurfaceType == Surface.CA1 + i) return "Top Surface" + (i + 1) + "Based Region";
                }
                for (int i = 0; i < MainWindow.Setting.Job.BACount; i++)
                {
                    if (SurfaceType == Surface.BA1 + i) return "Bottom Surface" + (i + 1) + "Based Region";
                }
                return "";
            }
        }

        public string GetTeachingFileName(string strMiddleName, string strExtention)
        {
            return String.Format("{0}-{3}-{1}.{2}", MainWindow.CurrentModel.Name, strMiddleName, strExtention, (int)SurfaceType);
        }

        /// <summary>   Export teaching data. </summary>
        /// <remarks>   suoow2, 2014-11-25. </remarks>
        public void ExportTeachingData(string strDirectoryPath, bool abNeedSaveImage = true)
        {
            if (ReferenceImage == null)
                return;
            DirectoryManager.CreateDirectory(strDirectoryPath);

            // 1. 전체 이미지 저장
            if (abNeedSaveImage)
            {
                FileSupport.SaveImageFile(System.IO.Path.Combine(strDirectoryPath, GetTeachingFileName("Based", "bmp")), ReferenceImage);
            }
            // 2. 전체 Teaching 데이터 저장
            BasedROICanvas.Save(System.IO.Path.Combine(strDirectoryPath, GetTeachingFileName("Based", "xml")), MainWindow.CurrentModel.Name,
                                GetSelectedSectionName(), ReferenceImage.PixelWidth, ReferenceImage.PixelHeight);

            foreach (SectionInformation section in SectionManager.Sections)
            {
                // 3. 섹션 이미지 저장
                if (abNeedSaveImage)
                {
                    FileSupport.SaveImageFile(System.IO.Path.Combine(strDirectoryPath, GetTeachingFileName(section.Name, "bmp")), section.GetBitmapSource(0));
                }
                // 4. 섹션 Teaching 데이터 저장
                if (section.ROICanvas[0].GraphicsList.Count > 0)
                {
                    section.ROICanvas[0].Save(System.IO.Path.Combine(strDirectoryPath, GetTeachingFileName(section.Name, "xml")), MainWindow.CurrentModel.Name,
                        GetSelectedSectionName(), section.GetBitmapSource(0).PixelWidth, section.GetBitmapSource(0).PixelHeight);
                }
            }
        }

        /// <summary>   Import teaching data. </summary>
        /// <remarks>   suoow2, 2014-11-30. </remarks>
        public bool ImportTeachingData(string strDirectoryPath)
        {
            // 티칭 데이터를 추출한다.
            bool BasedReady = TryImportBasedTeachingData(strDirectoryPath);
            if (!BasedReady)
            {
                return false;
            }
            BasedROICanvas.ClearHistory(); // suoow2 Added. 2014-11-30

            if (BasedReady && BasedROICanvas.GraphicsList.Count > 0)
            {
                TryImportSectionTeachingData(strDirectoryPath);
            }

            return true; // 최소 기준 영상만 읽히더라도 Import 성공된 것으로 간주한다.
        }

        /// <summary>   Try import section teaching data. </summary>
        /// <remarks>   suoow2, 2014-11-29. </remarks>
        private void TryImportSectionTeachingData(string strDirectoryPath)
        {
            if (!Directory.Exists(strDirectoryPath))
            {
                return;
            }
            MainWindow ptrMainWindow = Application.Current.MainWindow as MainWindow;
            if (ptrMainWindow == null)
            {
                return;
            }

            List<SectionInformation> sections = new List<SectionInformation>(); // 임시 Section 저장소
            GraphicsRectangle graphic;

            int nStripAlignCount = 1;
            int nUnitRegionCount = 1;
            int nOuterRegionCount = 1;
            int nPsrRegionCount = 1;

            int nSectionCount = BasedROICanvas.FiduGraphicsList.Count;
            // Section 복원을 시도한다.
            for (int nIndex = 0; nIndex < nSectionCount; nIndex++)
            {
                graphic = BasedROICanvas.FiduGraphicsList[nIndex];

                // Section의 Code 설정은 이미지와 ROI를 복원시킨 후 부여하도록 함.
                SectionInformation section = SectionManager.GraphicsToSection(graphic, BasedROICanvas, MainWindow.Setting.General.MachineCode, MainWindow.CurrentModel.Code);
                if (section != null)
                {
                    if (graphic.RegionType == GraphicsRegionType.StripAlign)
                    {
                        section.Type = SectionManager.GetSectionType(SectionTypeCode.STRIP_REGION);
                        section.Name = "StripAlign Section " + nStripAlignCount++;
                    }
                    else if (graphic.RegionType == GraphicsRegionType.UnitRegion)
                    {
                        section.Type = SectionManager.GetSectionType(SectionTypeCode.UNIT_REGION);
                        section.Name = "Unit Section " + nUnitRegionCount++;
                    }
                    else if (graphic.RegionType == GraphicsRegionType.PSROdd)
                    {
                        section.Type = SectionManager.GetSectionType(SectionTypeCode.PSR_REGION);
                        section.Name = "PSR Section " + nPsrRegionCount++;
                    }
                    else if (graphic.RegionType == GraphicsRegionType.IDRegion)
                    {
                        section.Type = SectionManager.GetSectionType(SectionTypeCode.ID_REGION);
                        section.Name = "ID Section " + nUnitRegionCount++;
                    }
                    else // OuterRegion
                    {
                        section.Type = SectionManager.GetSectionType(SectionTypeCode.OUTER_REGION);
                        section.Name = "Outer Section " + nOuterRegionCount++;
                    }
                    section.RelatedROI = graphic;
                    sections.Add(section);
                }
            }

            bool bFoundFiles = false;
            string[] strTokens;
            string strSelectedModelName = MainWindow.CurrentModel.Name;
            FileInfo fileInfo;
            m_SectionManager.Sections.Clear();
            foreach (SectionInformation section in sections)
            {
                // Set Image.
                foreach (string strFile in Directory.GetFiles(strDirectoryPath))
                {
                    fileInfo = new FileInfo(strFile);
                    if (fileInfo.Name.StartsWith(strSelectedModelName))
                    {
                        strTokens = fileInfo.Name.Substring(strSelectedModelName.Length + 1, fileInfo.Name.Length - strSelectedModelName.Length - 1).Split('-', '.');

                        if (strTokens.Length == 3 && strTokens[1].ToLower() == section.Name.ToLower() && strTokens[2].ToLower() == "bmp") // Section 기준 이미지에 해당되는 조건식
                        {
                            bool check = false;
                            foreach(Surface nSurface in Enum.GetValues(typeof(Surface)))
                            {
                                if(SurfaceType == nSurface && strTokens[0] == nSurface.ToString())
                                {
                                    check = true;
                                    break;
                                }
                            }
                            if (check)
                            {
                                bFoundFiles = true;
                                section.BitmapSource[0] = BitmapImageLoader.LoadCachedBitmapImage(new Uri(strFile)) as BitmapSource;
                                if (section.BitmapSource[0] != null)
                                {
                                    m_ptrTeachingWindow.SaveReferenceSectionImages(this.ID); // 2012-03-22, suoow2 : Import 된 이미지를 기준 이미지로 등록함.
                                    m_SectionManager.Sections.Add(section);
                                    // Set XML.
                                    if (File.Exists(strFile.Substring(0, strFile.Length - 3) + "xml"))
                                    {
                                        section.ROICanvas[0].Measure(new Size(section.ROICanvas[0].Width, section.ROICanvas[0].Height));
                                        section.ROICanvas[0].Arrange(new Rect(0, 0, section.ROICanvas[0].DesiredSize.Width, section.ROICanvas[0].DesiredSize.Height));
                                        section.ROICanvas[0].Refresh();
                                        section.ROICanvas[0].Load(strFile.Substring(0, strFile.Length - 3) + "xml", strSelectedModelName, section.Name, section.BitmapSource[0].PixelWidth, section.BitmapSource[0].PixelHeight);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!bFoundFiles) // 일치하는 파일을 찾지 못한 경우 마구잡이로 파일을 조회한다. (현재 모델과 Import 파일명이 다른 경우 발생할 수 있는 사용상 불편함 해소)
            {
                foreach (SectionInformation section in sections)
                {
                    foreach (string strFile in Directory.GetFiles(strDirectoryPath))
                    {
                        fileInfo = new FileInfo(strFile);
                        strTokens = fileInfo.Name.Split('-', '.');
                        for (int nIndex = 0; nIndex < strTokens.Length; nIndex++)
                        {
                            strTokens[nIndex] = strTokens[nIndex].ToLower();
                        }

                        if (strTokens.Length >= 3 && strTokens[strTokens.Length - 2] == section.Name.ToLower() && strTokens[strTokens.Length - 1] == "bmp")
                        {
                            bool check = false;
                            foreach (Surface nSurface in Enum.GetValues(typeof(Surface)))
                            {
                                if (SurfaceType == nSurface && strTokens[0] == nSurface.ToString())
                                {
                                    check = true;
                                    break;
                                }
                            }
                            if (check)
                            {
                                bFoundFiles = true;
                                section.BitmapSource[0] = BitmapImageLoader.LoadCachedBitmapImage(new Uri(strFile)) as BitmapSource;
                                if (section.BitmapSource[0] != null)
                                {
                                    m_ptrTeachingWindow.SaveReferenceSectionImages(this.ID);
                                    m_SectionManager.Sections.Add(section);
                                    if (File.Exists(strFile.Substring(0, strFile.Length - 3) + "xml"))
                                    {
                                        section.ROICanvas[0].Measure(new Size(section.ROICanvas[0].Width, section.ROICanvas[0].Height));
                                        section.ROICanvas[0].Arrange(new Rect(0, 0, section.ROICanvas[0].DesiredSize.Width, section.ROICanvas[0].DesiredSize.Height));
                                        section.ROICanvas[0].Refresh();
                                        section.ROICanvas[0].Load(strFile.Substring(0, strFile.Length - 3) + "xml", strSelectedModelName, section.Name, section.BitmapSource[0].PixelWidth, section.BitmapSource[0].PixelHeight);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            List<SectionInformation> temp = m_SectionManager.Sections.ToList();
            temp.Sort((x, y) => string.Compare(x.Name, y.Name));

            m_SectionManager.Sections = new ObservableCollection<SectionInformation>(temp);

            int nSecCode = 0;
            foreach (SectionInformation section in m_SectionManager.Sections)
            {
                section.Code = QueryHelper.GetCode(nSecCode++, 4);
            }

            int temp_ID = ID - 1;
            if (ID <= 2)
                temp_ID = 0;

            // Vision PC와 연결되어있는 상태라면, Section 정보를 Vision PC에 등록시킨다.
            if (m_ptrTeachingWindow.IsOnLine)
            {
                if (ptrMainWindow.PCSInstance.Vision[temp_ID].Connected)
                {
                    ptrMainWindow.PCSInstance.Vision[temp_ID].ClearModelInfo();
                    foreach (SectionInformation section in sections)
                    {
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
                        ptrMainWindow.PCSInstance.Vision[temp_ID].AddSection(section);
                        Thread.Sleep(5);
                    }
                }
                m_ptrTeachingWindow.SaveReferenceSectionImages(this.ID); // 2012-03-22, suoow2 : Import 된 이미지를 기준 이미지로 등록함.
            }

            lbSection.ItemsSource = m_SectionManager.Sections;
            lbSection.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            lbSection.SelectedIndex = -1;

            // IS에서 전달받은 불량에 대한 Unit X, Y의 실제 값을 연산하기 위해 IS_Interface에 Section 리스트에 대한 포인터를 전달한다.
            if (ID != 1)
                ptrMainWindow.PCSInstance.Vision[temp_ID].Sections = SectionManager.Sections;
        }

        /// <summary>   Try import based teaching data. </summary>
        /// <remarks>   suoow2, 2014-11-29. </remarks>
        private bool TryImportBasedTeachingData(string strDirectoryPath)
        {
            if (!Directory.Exists(strDirectoryPath))
            {
                return false;
            }
            string[] strTokens;
            string strSelectedModelName = MainWindow.CurrentModel.Name;
            FileInfo fileInfo;

            try
            {
                bool bFoundFiles = false;

                // 1. 기준면 정보를 구성한다.
                foreach (string strFile in Directory.GetFiles(strDirectoryPath))
                {
                    fileInfo = new FileInfo(strFile);
                    if (fileInfo.Name.StartsWith(strSelectedModelName)) // 현재 선택된 모델에 해당되는 데이터인지 확인한다.
                    {
                        // 예를 들어, '201111-19-Sample Model-11-Based.bmp' 문자열이 '11-Based.bmp'로 잘려진다.
                        // 그 후 ('-', '.') 구분자를 통해 {"11", "Based", "bmp"}과 같은 토큰 배열에 저장된다.
                        strTokens = fileInfo.Name.Substring(strSelectedModelName.Length + 1, fileInfo.Name.Length - strSelectedModelName.Length - 1).Split('-', '.');

                        // 기준면에 대한 영상이 없다면 복원 자체에 의미가 없다.
                        // 따라서 기준면 이미지를 복원시킨 후에 개별 Section 데이터를 복원하는 것이 옳다.
                        if (strTokens.Length == 3 && strTokens[1].ToLower() == "based" && strTokens[2].ToLower() == "bmp") // 기준면 이미지에 해당되는 조건식.
                        {
                            bool check = false;
                            foreach (Surface nSurface in Enum.GetValues(typeof(Surface)))
                            {
                                if (SurfaceType == nSurface && strTokens[0] == nSurface.ToString())
                                {
                                    check = true;
                                    break;
                                }
                            }
                            if (check)
                            {
                                bFoundFiles = true;
                                ReferenceImage = BitmapImageLoader.LoadCachedBitmapImage(new Uri(strFile)) as BitmapSource;
                                InitializeSurfaceView(SurfaceType); // Teaching Ctrl을 초기화 시킨다.
                                if (ReferenceImage != null)
                                {
                                    IsGrabView = false;
                                    m_ptrTeachingWindow.SaveReferenceEntireImage(this.ID); // 2012-03-22, suoow2 : Import 된 이미지를 기준 이미지로 등록함.
                                    if (File.Exists(strFile.Substring(0, strFile.Length - 3) + "xml"))
                                    {
                                        BasedROICanvas.Measure(new Size(BasedROICanvas.Width, BasedROICanvas.Height));
                                        BasedROICanvas.Arrange(new Rect(0, 0, BasedROICanvas.DesiredSize.Width, BasedROICanvas.DesiredSize.Height));
                                        BasedROICanvas.Refresh();
                                        BasedROICanvas.Load(strFile.Substring(0, strFile.Length - 3) + "xml", strSelectedModelName, GetSelectedSectionName(), ReferenceImage.PixelWidth, ReferenceImage.PixelHeight);
                                    }
                                }

                                return true; // 기준면 이미지만 불려지더라도 성공한 것으로 간주한다.
                            }
                        }
                    }
                }

                if (!bFoundFiles) // 일치하는 파일을 찾지 못한 경우 마구잡이로 파일을 조회한다. (현재 모델과 Import 파일명이 다른 경우 발생할 수 있는 사용상 불편함 해소)
                {
                    foreach (string strFile in Directory.GetFiles(strDirectoryPath))
                    {
                        fileInfo = new FileInfo(strFile);
                        strTokens = fileInfo.Name.Split('-', '.');
                        for (int nIndex = 0; nIndex < strTokens.Length; nIndex++)
                        {
                            strTokens[nIndex] = strTokens[nIndex].ToLower();
                        }
                        if (strTokens.Length >= 3 && strTokens[strTokens.Length - 2] == "based" && strTokens[strTokens.Length - 1] == "bmp")
                        {
                            bool check = false;
                            foreach (Surface nSurface in Enum.GetValues(typeof(Surface)))
                            {
                                if (SurfaceType == nSurface && strTokens[0] == nSurface.ToString())
                                {
                                    check = true;
                                    break;
                                }
                            }
                            if (check)
                            {
                                bFoundFiles = true;
                                ReferenceImage = BitmapImageLoader.LoadCachedBitmapImage(new Uri(strFile)) as BitmapSource;
                                InitializeSurfaceView(SurfaceType);
                                if (ReferenceImage != null)
                                {
                                    m_ptrTeachingWindow.SaveReferenceEntireImage(this.ID);
                                    if (File.Exists(strFile.Substring(0, strFile.Length - 3) + "xml"))
                                    {
                                        BasedROICanvas.Measure(new Size(BasedROICanvas.Width, BasedROICanvas.Height));
                                        BasedROICanvas.Arrange(new Rect(0, 0, BasedROICanvas.DesiredSize.Width, BasedROICanvas.DesiredSize.Height));
                                        BasedROICanvas.Refresh();
                                        BasedROICanvas.Load(strFile.Substring(0, strFile.Length - 3) + "xml", strSelectedModelName, GetSelectedSectionName(), ReferenceImage.PixelWidth, ReferenceImage.PixelHeight);
                                    }
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {
                MainWindow.Log("MTS", SeverityLevel.ERROR, "Teaching 데이터를 복원하는 중 오류가 발생하였습니다.");
                Debug.WriteLine("Exception occured in TryImportBasedTeachingData(TeachingViewerCtrl.xaml.cs)");
            }
            return false;
        }
        #endregion

        #region Save Section.
        public bool SaveSection(string strMachineCode, string strModelCode)
        {

            return m_SectionManager.SaveSection(strMachineCode, strModelCode, SurfaceType);
        }
        #endregion

        #region Load based image.
        private BitmapSource LoadBasedImage(Surface aSurface)
        {
            try
            {
                if (MainWindow.CurrentGroup != null && MainWindow.CurrentModel != null)
                {
                    string szBasedImagePath = string.Empty;
                    if (!m_ptrTeachingWindow.IsOnLine && m_ptrTeachingWindow.MtsManager.SelectedMachine != null)
                    {
                        szBasedImagePath = DirectoryManager.GetOfflineBasedImagePath(MainWindow.Setting.General.ModelPath, m_ptrTeachingWindow.MtsManager.SelectedMachine.Name, MainWindow.CurrentGroup.Name,
                                                                              MainWindow.CurrentModel.Name, aSurface);
                    }
                    else
                    {
                        szBasedImagePath = DirectoryManager.GetBasedImagePath(MainWindow.Setting.General.ModelPath, MainWindow.CurrentGroup.Name, MainWindow.CurrentModel.Name, aSurface);
                    }

                    if (File.Exists(szBasedImagePath))
                    {
                        this.txtReferenceLabel.Visibility = Visibility.Visible;

                        // 전체영상을 획득하여 반환한다.
                        return BitmapImageLoader.LoadCachedBitmapImage(new Uri(szBasedImagePath)) as BitmapSource;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        // Section Region 재탐색
        public void RetrySearchROIPattern()
        {
            if (lbSection.SelectedIndex != -1)
                return;

            if (!m_Algo.SetImage(BitmapHelper.BitmapSource2Bitmap(BasedImage.Source as BitmapSource)))
                return;

            foreach (GraphicsRectangle fiducialGraphic in BasedROICanvas.SelectionRectangle)
            {
                if (fiducialGraphic.IsFiducialRegion)
                {
                    foreach (GraphicsRectangle childGraphic in BasedROICanvas.GraphicsRectangleList)
                    {
                        if (childGraphic.MotherROI == fiducialGraphic)
                        {
                            childGraphic.IsSelected = false;
                        }
                    }
                }
            }

            foreach (GraphicsRectangle selectedGraphic in BasedROICanvas.SelectionRectangle)
            {
                GraphicsRectangle fiducialGraphic = selectedGraphic;
                List<GraphicsRectangle> childGraphics = new List<GraphicsRectangle>();

                foreach (GraphicsRectangle g in BasedROICanvas.GraphicsRectangleList)
                {
                    if (g.MotherROI == fiducialGraphic)
                        childGraphics.Add(g);
                }
                if (childGraphics.Count == 0)
                {
                    if (fiducialGraphic.MotherROI != null)
                    {
                        childGraphics.Add(fiducialGraphic);
                        fiducialGraphic = fiducialGraphic.MotherROI as GraphicsRectangle;

                    }
                    if (childGraphics.Count == 0 || fiducialGraphic == null)
                        continue;
                }

                // Retry Search Margin은 최대 100px or ROI 크기의 50% 수준.
                int nMarginWidth = (fiducialGraphic.WidthProperty / 2 > 100) ? 100 : (int)fiducialGraphic.WidthProperty / 2;
                int nMarginHeight = (fiducialGraphic.HeightProperty / 2 > 100) ? 100 : (int)fiducialGraphic.HeightProperty / 2;
                System.Drawing.Point sizeSearchMargin = new System.Drawing.Point(nMarginWidth, nMarginHeight);

                System.Drawing.Point ptNext = new System.Drawing.Point();
                System.Drawing.Rectangle rectSearch = new System.Drawing.Rectangle((int)fiducialGraphic.Left, (int)fiducialGraphic.Top,
                                                                                   (int)fiducialGraphic.WidthProperty, (int)fiducialGraphic.HeightProperty);
                m_Algo.SetTemplateImage(rectSearch);

                bool bResult = false;
                foreach (GraphicsRectangle g in childGraphics)
                {
                    ptNext.X = Convert.ToInt32(g.Left);
                    ptNext.Y = Convert.ToInt32(g.Top);
                    bResult = m_Algo.SearchTemplateImage(ptNext, sizeSearchMargin, 0.75); // 재탐색의 경우 Search %를 65-> 75

                    if (bResult)
                    {
                        double fLeft = ptNext.X + m_Algo.Offset.X;
                        double fTop = ptNext.Y + m_Algo.Offset.Y;
                        if (fLeft != g.Left || fTop != g.Top)
                        {
                            if (g.MotherROI != null)
                            {
                                g.OriginObjectColor = g.ObjectColor = g.MotherROI.OriginObjectColor;
                                g.IsValidRegion = true;
                            }
                            g.UpdateRect(fLeft, fTop, fLeft + fiducialGraphic.WidthProperty, fTop + fiducialGraphic.HeightProperty);
                        }
                    }
                    else
                    {
                        double fLeft = ptNext.X;
                        double fTop = ptNext.Y;
                        if (fLeft != g.Left || fTop != g.Top)
                        {
                            g.OriginObjectColor = g.ObjectColor = GraphicsColors.Blue;
                            g.IsValidRegion = false;
                            g.UpdateRect(fLeft, fTop, fLeft + fiducialGraphic.WidthProperty, fTop + fiducialGraphic.HeightProperty);
                        }
                    }
                    g.RefreshDrawing();
                }
            }
        }

        private void SearchRawmetrialROI(GraphicsRectangle aFiducialGraphic)
        {
            if (aFiducialGraphic == null) return;
            m_Algo.SetImage(BitmapHelper.BitmapSource2Bitmap(BasedImage.Source as BitmapSource));

        }

        private void SearchROIPattern(GraphicsRectangle aFiducialGraphic, int nSearchType)
        {
            if (aFiducialGraphic == null || lbSection.SelectedIndex != -1)
                return;
            // Template ROI는 현재 DrawingCanvas 위에 얹어진 ROI여야 한다.
            if (BasedROICanvas.GraphicsList.Contains(aFiducialGraphic))
            {
                #region Set Local Variables.
                int unitColumns = aFiducialGraphic.IterationValue.Column;
                int unitRows = aFiducialGraphic.IterationValue.Row;
                double unitXPitch = aFiducialGraphic.IterationValue.XPitch;
                double unitYPitch = aFiducialGraphic.IterationValue.YPitch;

                int blocknum = aFiducialGraphic.BlockIterationValue.Block;
                int blockColumns = aFiducialGraphic.BlockIterationValue.Column;
                int blockRows = aFiducialGraphic.BlockIterationValue.Row;
                double blockgap = aFiducialGraphic.BlockIterationValue.Gap;
                double blockXPitch = aFiducialGraphic.BlockIterationValue.XPitch;
                double blockYPitch = aFiducialGraphic.BlockIterationValue.YPitch;

                double rowSpace = 0;
                double columnSpace = 0;

                int unitRowPerBlockNum = ((double)unitRows / (double)blocknum > unitRows / blocknum) ? unitRows / blocknum + 1 : unitRows / blocknum;
                int unitRowPerBlockRow = ((double)unitRowPerBlockNum / (double)blockRows > unitRowPerBlockNum / blockRows) ? unitRowPerBlockNum / blockRows + 1 : unitRowPerBlockNum / blockRows;
                int unitColumnPerBlockColumn = ((double)unitColumns / (double)blockColumns > unitColumns / blockColumns) ? unitColumns / blockColumns + 1 : unitColumns / blockColumns;
                #endregion

                m_Algo.SetImage(BitmapHelper.BitmapSource2Bitmap(BasedImage.Source as BitmapSource));
                m_Algo.XOffsetSetZero();
                m_Algo.YOffsetSetZero();

                System.Drawing.Point ptNext = new System.Drawing.Point();
                System.Drawing.Rectangle rectSearch = new System.Drawing.Rectangle((int)aFiducialGraphic.Left, (int)aFiducialGraphic.Top,
                                                                                   (int)aFiducialGraphic.WidthProperty, (int)aFiducialGraphic.HeightProperty);
                // Search Margin은 최대 30px or ROI 크기의 25% 수준.
                int nMarginWidth = (aFiducialGraphic.WidthProperty / 4 > 50) ? 50 : (int)aFiducialGraphic.WidthProperty / 4;
                int nMarginHeight = (aFiducialGraphic.HeightProperty / 4 > 50) ? 50 : (int)aFiducialGraphic.HeightProperty / 4;
                if (RGB)
                {
                    nMarginWidth = (aFiducialGraphic.WidthProperty / 4 > 100) ? 100 : (int)aFiducialGraphic.WidthProperty / 4;
                    nMarginHeight = (aFiducialGraphic.HeightProperty / 4 > 100) ? 100 : (int)aFiducialGraphic.HeightProperty / 4;
                }
                System.Drawing.Point sizeSearchMargin = new System.Drawing.Point(nMarginWidth, nMarginHeight);
                if (nSearchType == 2)
                {
                    rectSearch = new System.Drawing.Rectangle((int)aFiducialGraphic.Left + (int)aFiducialGraphic.WidthProperty / 2, (int)aFiducialGraphic.Top,
                                                                                       (int)aFiducialGraphic.WidthProperty / 2, (int)aFiducialGraphic.HeightProperty);

                    nMarginHeight = (aFiducialGraphic.HeightProperty / 4 > 40) ? 40 : (int)aFiducialGraphic.HeightProperty / 4;
                }
                m_Algo.SetTemplateImage(rectSearch);
                bool bResult = false;
                // int m_nColorIndex = 1;
                if (nSearchType == 2)
                {

                    m_Algo.XOffsetSetZero();
                    sizeSearchMargin.X = nMarginWidth;
                    sizeSearchMargin.Y = nMarginHeight;
                    columnSpace = 0;
                    System.Drawing.Point ppt = new System.Drawing.Point(0);
                    //Color sectionColor = GraphicsColors.GetNextColor(2);
                    for (int row = 0; row < unitRows; row++)
                    {
                        if (row == 0)
                        {
                            rowSpace += unitYPitch;
                            continue;
                        }
                        if (row % unitRowPerBlockRow == 0 && row >= 1)
                        {
                            if (row >= 1 && row % unitRowPerBlockNum == 0)
                            {
                                rowSpace += blockgap;
                            }
                            else rowSpace += blockYPitch;
                        }
                        ptNext.X = (int)aFiducialGraphic.Left;
                        ppt.X = (int)aFiducialGraphic.Left + (int)aFiducialGraphic.WidthProperty / 2;
                        ppt.Y = ptNext.Y = (int)aFiducialGraphic.Top + (int)rowSpace;
                        GraphicsRectangle graphic;
                        bResult = m_Algo.SearchTemplateImage(ppt, sizeSearchMargin, 0.75, m_Algo.Offset);
                        if (bResult)
                        {
                            Color roiColor;
                            roiColor = GraphicsColors.Green;
                            double fLeft = ptNext.X + m_Algo.Offset.X;
                            double fTop = ptNext.Y + m_Algo.Offset.Y;
                            graphic = new GraphicsRectangle(
                                fLeft, fTop, fLeft + aFiducialGraphic.WidthProperty, fTop + aFiducialGraphic.HeightProperty,
                                aFiducialGraphic.LineWidth, aFiducialGraphic.RegionType, roiColor, aFiducialGraphic.ActualScale);
                            graphic.IsValidRegion = true;
                            graphic.IterationXPosition = 0;
                            graphic.IterationYPosition = row;
                            graphic.BlockIterationValue = aFiducialGraphic.BlockIterationValue;
                            graphic.IterationValue = aFiducialGraphic.IterationValue;
                            graphic.Caption = CaptionHelper.GetRegionCaption(graphic);

                        }
                        else
                        {
                            graphic = new GraphicsRectangle(
                                ptNext.X, ptNext.Y, ptNext.X + aFiducialGraphic.WidthProperty, ptNext.Y + aFiducialGraphic.HeightProperty,
                                aFiducialGraphic.LineWidth, aFiducialGraphic.RegionType, GraphicsColors.Blue, aFiducialGraphic.ActualScale);
                            graphic.IsValidRegion = false;
                            graphic.IterationXPosition = 0;
                            graphic.IterationYPosition = row;
                            graphic.BlockIterationValue = aFiducialGraphic.BlockIterationValue;
                            graphic.IterationValue = aFiducialGraphic.IterationValue;
                            graphic.Caption = CaptionHelper.GetRegionCaption(graphic);
                        }
                        graphic.RegionType = aFiducialGraphic.RegionType;// GraphicsRegionType.UnitRegion;
                        graphic.MotherROI = aFiducialGraphic;
                        graphic.RefreshDrawing();
                        if (aFiducialGraphic.RegionType == GraphicsRegionType.OuterRegion)
                        {
                            graphic.IsFiducialRegion = true;


                            if (graphic != null)
                            {
                                graphic.IsFiducialRegion = true;
                                graphic.IsValidRegion = true;
                                graphic.MotherROI = null;
                                aFiducialGraphic.MotherROIID = aFiducialGraphic.ID;
                                graphic.MotherROIID = aFiducialGraphic.MotherROIID;
                                graphic.Caption = CaptionHelper.FiducialOuterRegionCaption;


                                // graphic.OriginObjectColor = graphic.ObjectColor = sectionColor;
                                BasedROICanvas.GraphicsList.Add(graphic);
                                BasedROICanvas.FiduGraphicsList.Add(graphic);
                            }
                        }
                        else
                        {
                            BasedROICanvas.GraphicsList.Add(graphic);
                        }
                        rowSpace += unitYPitch;
                    }
                }
                else
                {
                    m_Algo.XOffsetSetZero();
                    sizeSearchMargin.X = nMarginWidth;
                    sizeSearchMargin.Y = nMarginHeight;
                    columnSpace = 0;
                    rowSpace = 0;
                    for (int row = 0; row < unitRows; row++)
                    {
                        if ((row % unitRowPerBlockRow == 0 && row >= 1) || row == 0)
                        {
                            if (row == 0) rowSpace = 0;
                            else
                            {
                                if (row >= 1 && row % unitRowPerBlockNum == 0)
                                {
                                    rowSpace += blockgap;
                                }
                                else rowSpace += blockYPitch;
                            }
                        }
                        else
                        {
                            rowSpace += unitYPitch;
                            continue;
                        }
                        for (int column = 0; column < unitColumns; column++)
                        {
                            if (column == 0 && row == 0)
                            {
                                continue;
                            }
                            if (column == 0)
                            {
                                m_Algo.XOffsetSetZero();
                                sizeSearchMargin.X = nMarginWidth;
                                sizeSearchMargin.Y = nMarginHeight;
                                columnSpace = 0;
                            }
                            else
                            {
                                sizeSearchMargin.X = nMarginWidth / 2;
                                sizeSearchMargin.Y = nMarginHeight / 2;
                                columnSpace += unitXPitch;
                            }

                            if (column % unitColumnPerBlockColumn == 0 && column >= 1)
                                columnSpace += blockXPitch;
                            ptNext.X = (int)aFiducialGraphic.Left + (int)columnSpace;
                            ptNext.Y = (int)aFiducialGraphic.Top + (int)rowSpace;

                            GraphicsRectangle graphic;
                            bResult = m_Algo.SearchTemplateImage(ptNext, sizeSearchMargin, 0.85, m_Algo.Offset);
                            if (bResult)
                            {
                                Color roiColor;
                                roiColor = GraphicsColors.Green;

                                double fLeft = ptNext.X + m_Algo.Offset.X;
                                double fTop = ptNext.Y + m_Algo.Offset.Y;
                                graphic = new GraphicsRectangle(
                                    fLeft, fTop, fLeft + aFiducialGraphic.WidthProperty, fTop + aFiducialGraphic.HeightProperty,
                                    aFiducialGraphic.LineWidth, aFiducialGraphic.RegionType, roiColor, aFiducialGraphic.ActualScale);
                                graphic.IsValidRegion = true;
                                graphic.IterationXPosition = column;
                                graphic.IterationYPosition = row;
                                graphic.BlockIterationValue = aFiducialGraphic.BlockIterationValue;
                                graphic.IterationValue = aFiducialGraphic.IterationValue;
                                graphic.Caption = CaptionHelper.GetRegionCaption(graphic);
                            }
                            else
                            {
                                graphic = new GraphicsRectangle(
                                    ptNext.X, ptNext.Y, ptNext.X + aFiducialGraphic.WidthProperty, ptNext.Y + aFiducialGraphic.HeightProperty,
                                    aFiducialGraphic.LineWidth, aFiducialGraphic.RegionType, GraphicsColors.Blue, aFiducialGraphic.ActualScale);
                                graphic.IsValidRegion = false;
                                graphic.IterationXPosition = column;
                                graphic.IterationYPosition = row;
                                graphic.BlockIterationValue = aFiducialGraphic.BlockIterationValue;
                                graphic.IterationValue = aFiducialGraphic.IterationValue;
                                graphic.Caption = CaptionHelper.GetRegionCaption(graphic);
                            }
                            graphic.RegionType = aFiducialGraphic.RegionType;// GraphicsRegionType.UnitRegion;
                            graphic.MotherROI = aFiducialGraphic;
                            graphic.RefreshDrawing();
                            BasedROICanvas.GraphicsList.Add(graphic);
                        }
                        rowSpace += unitYPitch;
                    }
                }
            }
            //aFiducialGraphic.Caption = CaptionHelper.FiducialUnitRegionCaption;
            //aFiducialGraphic.OriginObjectColor = GraphicsColors.Green;
            //aFiducialGraphic.RegionType = GraphicsRegionType.UnitRegion;
            //aFiducialGraphic.IsFiducialRegion = true;
            //aFiducialGraphic.RefreshDrawing();
            BasedROICanvas.FiduGraphicsList.Add(aFiducialGraphic);
            BasedROICanvas.Refresh();
        }

        private void SearchROIOuterPattern(GraphicsRectangle aFiducialGraphic, int nSearchType)
        {
            if (aFiducialGraphic == null || lbSection.SelectedIndex != -1)
                return;
            // Template ROI는 현재 DrawingCanvas 위에 얹어진 ROI여야 한다.
            if (BasedROICanvas.GraphicsList.Contains(aFiducialGraphic))
            {
                #region Set Local Variables.
                int unitColumns = aFiducialGraphic.IterationValue.Column;
                int unitRows = aFiducialGraphic.IterationValue.Row;
                double unitXPitch = aFiducialGraphic.IterationValue.XPitch;
                double unitYPitch = aFiducialGraphic.IterationValue.YPitch;

                int blocknum = aFiducialGraphic.BlockIterationValue.Block;
                int blockColumns = aFiducialGraphic.BlockIterationValue.Column;
                int blockRows = aFiducialGraphic.BlockIterationValue.Row;
                double blockgap = aFiducialGraphic.BlockIterationValue.Gap;
                double blockXPitch = aFiducialGraphic.BlockIterationValue.XPitch;
                double blockYPitch = aFiducialGraphic.BlockIterationValue.YPitch;

                double rowSpace = 0;
                double columnSpace = 0;

                int unitRowPerBlockNum = ((double)unitRows / (double)blocknum > unitRows / blocknum) ? unitRows / blocknum + 1 : unitRows / blocknum;
                int unitRowPerBlockRow = ((double)unitRowPerBlockNum / (double)blockRows > unitRowPerBlockNum / blockRows) ? unitRowPerBlockNum / blockRows + 1 : unitRowPerBlockNum / blockRows;
                int unitColumnPerBlockColumn = ((double)unitColumns / (double)blockColumns > unitColumns / blockColumns) ? unitColumns / blockColumns + 1 : unitColumns / blockColumns;
                #endregion

                m_Algo.SetImage(BitmapHelper.BitmapSource2Bitmap(BasedImage.Source as BitmapSource));
                m_Algo.XOffsetSetZero();
                m_Algo.YOffsetSetZero();

                System.Drawing.Point ptNext = new System.Drawing.Point();
                System.Drawing.Rectangle rectSearch = new System.Drawing.Rectangle((int)aFiducialGraphic.Left, (int)aFiducialGraphic.Top,
                                                                                   (int)aFiducialGraphic.WidthProperty, (int)aFiducialGraphic.HeightProperty);
                // Search Margin은 최대 30px or ROI 크기의 25% 수준.
                int nMarginWidth = (aFiducialGraphic.WidthProperty / 2 > 100) ? 100 : (int)aFiducialGraphic.WidthProperty / 2;
                int nMarginHeight = (aFiducialGraphic.HeightProperty / 2 > 100) ? 100 : (int)aFiducialGraphic.HeightProperty / 2;
                System.Drawing.Point sizeSearchMargin = new System.Drawing.Point(nMarginWidth, nMarginHeight);

                m_Algo.SetTemplateImage(rectSearch);
                bool bResult = false;

                // int m_nColorIndex = 1;
                if (nSearchType == 2)
                {
                    m_Algo.XOffsetSetZero();
                    sizeSearchMargin.X = nMarginWidth;
                    sizeSearchMargin.Y = nMarginHeight;
                    columnSpace = 0;
                    for (int row = 0; row < unitRows; row++)
                    {
                        if (row == 0)
                        {
                            rowSpace += unitYPitch;
                            continue;
                        }
                        if (row % unitRowPerBlockRow == 0 && row >= 1)
                        {
                            if (row >= 1 && row % unitRowPerBlockNum == 0)
                            {
                                rowSpace += blockgap;
                            }
                            else rowSpace += blockYPitch;
                        }
                        ptNext.X = (int)aFiducialGraphic.Left;
                        ptNext.Y = (int)aFiducialGraphic.Top + (int)rowSpace;
                        GraphicsRectangle graphic;
                        bResult = m_Algo.SearchTemplateImage(ptNext, sizeSearchMargin, 0.65, m_Algo.Offset);
                        if (bResult)
                        {
                            Color roiColor;
                            roiColor = GraphicsColors.Green;
                            double fLeft = ptNext.X + m_Algo.Offset.X;
                            double fTop = ptNext.Y + m_Algo.Offset.Y;
                            graphic = new GraphicsRectangle(
                                fLeft, fTop, fLeft + aFiducialGraphic.WidthProperty, fTop + aFiducialGraphic.HeightProperty,
                                aFiducialGraphic.LineWidth, aFiducialGraphic.RegionType, roiColor, aFiducialGraphic.ActualScale);
                            graphic.IsValidRegion = true;
                            graphic.IterationXPosition = 0;
                            graphic.IterationYPosition = row;
                            graphic.BlockIterationValue = aFiducialGraphic.BlockIterationValue;
                            graphic.IterationValue = aFiducialGraphic.IterationValue;
                            graphic.Caption = CaptionHelper.GetRegionCaption(graphic);
                        }
                        else
                        {
                            graphic = new GraphicsRectangle(
                                ptNext.X, ptNext.Y, ptNext.X + aFiducialGraphic.WidthProperty, ptNext.Y + aFiducialGraphic.HeightProperty,
                                aFiducialGraphic.LineWidth, aFiducialGraphic.RegionType, GraphicsColors.Blue, aFiducialGraphic.ActualScale);
                            graphic.IsValidRegion = false;
                            graphic.IterationXPosition = 0;
                            graphic.IterationYPosition = row;
                            graphic.BlockIterationValue = aFiducialGraphic.BlockIterationValue;
                            graphic.IterationValue = aFiducialGraphic.IterationValue;
                            graphic.Caption = CaptionHelper.GetRegionCaption(graphic);
                        }
                        graphic.RegionType = GraphicsRegionType.OuterRegion;
                        graphic.MotherROI = aFiducialGraphic;
                        graphic.RefreshDrawing();
                        BasedROICanvas.GraphicsList.Add(graphic);
                        rowSpace += unitYPitch;
                    }
                }
                else
                {
                    m_Algo.XOffsetSetZero();
                    sizeSearchMargin.X = nMarginWidth;
                    sizeSearchMargin.Y = nMarginHeight;
                    columnSpace = 0;
                    rowSpace = 0;
                    for (int row = 0; row < unitRows; row++)
                    {
                        if ((row % unitRowPerBlockRow == 0 && row >= 1) || row == 0)
                        {
                            if (row == 0) rowSpace = 0;
                            else
                            {
                                if (row >= 1 && row % unitRowPerBlockNum == 0)
                                {
                                    rowSpace += blockgap;
                                }
                                else rowSpace += blockYPitch;
                            }
                        }
                        else
                        {
                            rowSpace += unitYPitch;
                            continue;
                        }
                        for (int column = 0; column < unitColumns; column++)
                        {
                            if (column == 0 && row == 0)
                            {
                                continue;
                            }
                            if (column == 0)
                            {
                                m_Algo.XOffsetSetZero();
                                sizeSearchMargin.X = nMarginWidth;
                                sizeSearchMargin.Y = nMarginHeight;
                                columnSpace = 0;
                            }
                            else
                            {
                                sizeSearchMargin.X = nMarginWidth / 2;
                                sizeSearchMargin.Y = nMarginHeight / 2;
                                columnSpace += unitXPitch;
                            }

                            if (column % unitColumnPerBlockColumn == 0 && column >= 1)
                                columnSpace += blockXPitch;
                            ptNext.X = (int)aFiducialGraphic.Left + (int)columnSpace;
                            ptNext.Y = (int)aFiducialGraphic.Top + (int)rowSpace;

                            GraphicsRectangle graphic;
                            bResult = m_Algo.SearchTemplateImage(ptNext, sizeSearchMargin, 0.65, m_Algo.Offset);
                            if (bResult)
                            {
                                Color roiColor;
                                roiColor = GraphicsColors.Green;

                                double fLeft = ptNext.X + m_Algo.Offset.X;
                                double fTop = ptNext.Y + m_Algo.Offset.Y;
                                graphic = new GraphicsRectangle(
                                    fLeft, fTop, fLeft + aFiducialGraphic.WidthProperty, fTop + aFiducialGraphic.HeightProperty,
                                    aFiducialGraphic.LineWidth, aFiducialGraphic.RegionType, roiColor, aFiducialGraphic.ActualScale);
                                graphic.IsValidRegion = true;
                                graphic.IterationXPosition = column;
                                graphic.IterationYPosition = row;
                                graphic.BlockIterationValue = aFiducialGraphic.BlockIterationValue;
                                graphic.IterationValue = aFiducialGraphic.IterationValue;
                                graphic.Caption = CaptionHelper.GetRegionCaption(graphic);
                            }
                            else
                            {
                                graphic = new GraphicsRectangle(
                                    ptNext.X, ptNext.Y, ptNext.X + aFiducialGraphic.WidthProperty, ptNext.Y + aFiducialGraphic.HeightProperty,
                                    aFiducialGraphic.LineWidth, aFiducialGraphic.RegionType, GraphicsColors.Blue, aFiducialGraphic.ActualScale);
                                graphic.IsValidRegion = false;
                                graphic.IterationXPosition = column;
                                graphic.IterationYPosition = row;
                                graphic.BlockIterationValue = aFiducialGraphic.BlockIterationValue;
                                graphic.IterationValue = aFiducialGraphic.IterationValue;
                                graphic.Caption = CaptionHelper.GetRegionCaption(graphic);
                            }
                            graphic.RegionType = GraphicsRegionType.OuterRegion;
                            graphic.MotherROI = aFiducialGraphic;
                            graphic.RefreshDrawing();
                            BasedROICanvas.GraphicsList.Add(graphic);
                        }
                        rowSpace += unitYPitch;
                    }
                }
            }
            //aFiducialGraphic.Caption = CaptionHelper.FiducialUnitRegionCaption;
            //aFiducialGraphic.OriginObjectColor = GraphicsColors.Green;
            //aFiducialGraphic.RegionType = GraphicsRegionType.UnitRegion;
            //aFiducialGraphic.IsFiducialRegion = true;
            //aFiducialGraphic.RefreshDrawing();
            BasedROICanvas.FiduGraphicsList.Add(aFiducialGraphic);
            BasedROICanvas.Refresh();
        }

        private void SearchROIPattern(GraphicsRectangle aFiducialGraphic, SymmetryType aSymmetryType, bool bAlign)
        {
            if (aFiducialGraphic == null || lbSection.SelectedIndex != -1)
                return;
            GraphicsRectangle pairFiducialGraphic = null; // 비대칭 모델인 경우의 또 하나의 기준 ROI

            // Template ROI는 현재 DrawingCanvas 위에 얹어진 ROI여야 한다.
            if (BasedROICanvas.GraphicsList.Contains(aFiducialGraphic))
            {
                #region Set Local Variables.
                int unitColumns = aFiducialGraphic.IterationValue.Column;
                int unitRows = aFiducialGraphic.IterationValue.Row;
                double unitXPitch = aFiducialGraphic.IterationValue.XPitch;
                double unitYPitch = aFiducialGraphic.IterationValue.YPitch;

                int blocknum = aFiducialGraphic.BlockIterationValue.Block;
                int blockColumns = aFiducialGraphic.BlockIterationValue.Column;
                int blockRows = aFiducialGraphic.BlockIterationValue.Row;
                double blockgap = aFiducialGraphic.BlockIterationValue.Gap;
                double blockXPitch = aFiducialGraphic.BlockIterationValue.XPitch;
                double blockYPitch = aFiducialGraphic.BlockIterationValue.YPitch;

                double rowSpace = 0;
                double columnSpace = 0;

                int unitRowPerBlockNum = ((double)unitRows / (double)blocknum > unitRows / blocknum) ? unitRows / blocknum + 1 : unitRows / blocknum;
                int unitRowPerBlockRow = ((double)unitRowPerBlockNum / (double)blockRows > unitRowPerBlockNum / blockRows) ? unitRowPerBlockNum / blockRows + 1 : unitRowPerBlockNum / blockRows;
                int unitColumnPerBlockColumn = ((double)unitColumns / (double)blockColumns > unitColumns / blockColumns) ? unitColumns / blockColumns + 1 : unitColumns / blockColumns;
                #endregion

                m_Algo.SetImage(BitmapHelper.BitmapSource2Bitmap(BasedImage.Source as BitmapSource));
                m_Algo.XOffsetSetZero();
                m_Algo.YOffsetSetZero();

                System.Drawing.Point ptNext = new System.Drawing.Point();
                System.Drawing.Rectangle rectSearch = new System.Drawing.Rectangle((int)aFiducialGraphic.Left, (int)aFiducialGraphic.Top,
                                                                                   (int)aFiducialGraphic.WidthProperty, (int)aFiducialGraphic.HeightProperty);
                // Search Margin은 최대 30px or ROI 크기의 25% 수준.
                int nMarginWidth = (aFiducialGraphic.WidthProperty / 4 > 40) ? 40 : (int)aFiducialGraphic.WidthProperty / 4;
                int nMarginHeight = (aFiducialGraphic.HeightProperty / 4 > 40) ? 40 : (int)aFiducialGraphic.HeightProperty / 4;
                if (RGB)
                {
                    nMarginWidth = (aFiducialGraphic.WidthProperty / 4 > 20) ? 20 : (int)aFiducialGraphic.WidthProperty / 4;
                    nMarginHeight = (aFiducialGraphic.HeightProperty / 4 > 20) ? 20 : (int)aFiducialGraphic.HeightProperty / 4;
                }
                System.Drawing.Point sizeSearchMargin = new System.Drawing.Point(nMarginWidth, nMarginHeight);

                m_Algo.SetTemplateImage(rectSearch);
                System.Drawing.Point prevOffset = new System.Drawing.Point(0, 0);
                System.Drawing.Point prevrowOffset = new System.Drawing.Point(0, 0);
                bool bResult = false;
                for (int row = 0; row < unitRows; row++)
                {
                    if (row % unitRowPerBlockRow == 0 && row >= 1)
                    {
                        if (row >= 1 && row % unitRowPerBlockNum == 0)
                        {
                            rowSpace += blockgap;
                        }
                        else rowSpace += blockYPitch;
                    }

                    for (int column = 0; column < unitColumns; column++)
                    {
                        if (column == 0 && row == 0) continue;

                        if (column == 0)
                        {
                            m_Algo.XOffsetSetZero();
                            sizeSearchMargin.X = nMarginWidth;
                            sizeSearchMargin.Y = nMarginHeight;
                            columnSpace = prevrowOffset.X;
                            prevOffset = new System.Drawing.Point(0, 0);
                        }
                        else
                        {
                            sizeSearchMargin.X = nMarginWidth / 2;
                            sizeSearchMargin.Y = nMarginHeight / 2;
                            columnSpace += (unitXPitch);// + prevOffset.X);
                        }

                        if (column % unitColumnPerBlockColumn == 0 && column >= 1)

                            columnSpace += blockXPitch;



                        ptNext.X = (int)aFiducialGraphic.Left + (int)columnSpace;
                        ptNext.Y = (int)aFiducialGraphic.Top + (int)rowSpace;// +prevOffset.Y;

                        GraphicsRectangle graphic;
                        // m_Algo.Offset = new System.Drawing.Point(0, 0);
                        if (bAlign) bResult = m_Algo.SearchTemplateImage(ptNext, sizeSearchMargin, 0.65, m_Algo.Offset);
                        else
                        {
                            bResult = true;
                            m_Algo.Offset = new System.Drawing.Point(0, 0);
                        }
                        if (bResult)
                        {
                            #region Set Object, Text color of ROI.
                            Color roiColor;
                            switch (aSymmetryType)
                            {
                                case SymmetryType.Matrix:
                                    roiColor = GraphicsColors.Green;
                                    break;
                                case SymmetryType.XFlip:
                                    if (column % 2 == 0)
                                        roiColor = GraphicsColors.YellowGreen;
                                    else
                                        roiColor = GraphicsColors.DodgerBlue;
                                    break;
                                case SymmetryType.YFlip:
                                    if (row % 2 == 0)
                                        roiColor = GraphicsColors.YellowGreen;
                                    else
                                        roiColor = GraphicsColors.DodgerBlue;
                                    break;
                                case SymmetryType.XYFlip:
                                    if ((row + column) % 2 == 0)
                                        roiColor = GraphicsColors.YellowGreen;
                                    else
                                        roiColor = GraphicsColors.DodgerBlue;
                                    break;
                                case SymmetryType.Unknown:
                                default:
                                    roiColor = GraphicsColors.Purple;
                                    break;
                            }
                            #endregion

                            double fLeft = ptNext.X + m_Algo.Offset.X;
                            double fTop = ptNext.Y + m_Algo.Offset.Y;
                            prevOffset = new System.Drawing.Point(m_Algo.Offset.X, m_Algo.Offset.Y);
                            if (column == 0)
                                prevrowOffset = new System.Drawing.Point(m_Algo.Offset.X, m_Algo.Offset.Y);
                            graphic = new GraphicsRectangle(
                                fLeft, fTop, fLeft + aFiducialGraphic.WidthProperty, fTop + aFiducialGraphic.HeightProperty,
                                aFiducialGraphic.LineWidth, aFiducialGraphic.RegionType, roiColor, aFiducialGraphic.ActualScale);
                            graphic.IsValidRegion = true;
                            graphic.IterationXPosition = column;
                            graphic.IterationYPosition = row;
                            graphic.BlockIterationValue = aFiducialGraphic.BlockIterationValue;
                            graphic.IterationValue = aFiducialGraphic.IterationValue;
                            graphic.Caption = CaptionHelper.GetRegionCaption(graphic);
                        }
                        else
                        {
                            graphic = new GraphicsRectangle(
                                ptNext.X, ptNext.Y, ptNext.X + aFiducialGraphic.WidthProperty, ptNext.Y + aFiducialGraphic.HeightProperty,
                                aFiducialGraphic.LineWidth, aFiducialGraphic.RegionType, GraphicsColors.Blue, aFiducialGraphic.ActualScale);
                            graphic.IsValidRegion = false;
                            graphic.IterationXPosition = column;
                            graphic.IterationYPosition = row;
                            graphic.BlockIterationValue = aFiducialGraphic.BlockIterationValue;
                            graphic.IterationValue = aFiducialGraphic.IterationValue;
                            graphic.Caption = CaptionHelper.GetRegionCaption(graphic);
                        }

                        if (aSymmetryType == SymmetryType.Matrix)
                        {
                            graphic.MotherROI = aFiducialGraphic;
                        }
                        else
                        {
                            switch (aSymmetryType)
                            {
                                case SymmetryType.XFlip:
                                    if (column % 2 == 0)
                                        graphic.MotherROI = aFiducialGraphic;
                                    else
                                        graphic.MotherROI = pairFiducialGraphic;
                                    break;
                                case SymmetryType.YFlip:
                                    if (row % 2 == 0)
                                        graphic.MotherROI = aFiducialGraphic;
                                    else
                                        graphic.MotherROI = pairFiducialGraphic;
                                    break;
                                case SymmetryType.XYFlip:
                                    if ((row + column) % 2 == 0)
                                        graphic.MotherROI = aFiducialGraphic;
                                    else
                                        graphic.MotherROI = pairFiducialGraphic;
                                    break;
                            }
                        }

                        // 비대칭 모델인 경우 기준 ROI에 대응되는 또 하나의 기준 ROI를 식별함.
                        if (column + row == 1)
                        {
                            if (column == 1 && (aSymmetryType == SymmetryType.XFlip || aSymmetryType == SymmetryType.XYFlip))
                            {
                                // column = 1, row = 0 case.
                                graphic.IsValidRegion = true;
                                graphic.IsFiducialRegion = true;
                                graphic.SymmetryValue.StartX = graphic.IterationXPosition + 1;
                                graphic.SymmetryValue.StartY = graphic.IterationYPosition + 1;
                                graphic.SymmetryValue.JumpX = 2;
                                graphic.SymmetryValue.JumpY = (aSymmetryType == SymmetryType.XFlip) ? 1 : 2;
                                if (graphic.RegionType == GraphicsRegionType.UnitRegion)
                                    graphic.Caption = CaptionHelper.FiducialUnitRegionCaption;
                                else if (graphic.RegionType == GraphicsRegionType.PSROdd)
                                    graphic.Caption = CaptionHelper.FiducialPsrRegionCaption;
                                graphic.MotherROI = null; // 기준 ROI는 MotherROI가 null이다.
                                pairFiducialGraphic = graphic;
                            }
                            else if (row == 1 && aSymmetryType == SymmetryType.YFlip)
                            {
                                // column = 0, row = 1 case.
                                graphic.IsValidRegion = true;
                                graphic.IsFiducialRegion = true;
                                graphic.SymmetryValue.StartX = graphic.IterationXPosition + 1;
                                graphic.SymmetryValue.StartY = graphic.IterationYPosition + 1;
                                graphic.SymmetryValue.JumpX = 1;
                                graphic.SymmetryValue.JumpY = 2;
                                if (graphic.RegionType == GraphicsRegionType.UnitRegion)
                                    graphic.Caption = CaptionHelper.FiducialUnitRegionCaption;
                                else if (graphic.RegionType == GraphicsRegionType.PSROdd)
                                    graphic.Caption = CaptionHelper.FiducialPsrRegionCaption;
                                graphic.MotherROI = null; // 기준 ROI는 MotherROI가 null이다.
                                pairFiducialGraphic = graphic;
                            }
                        }
                        graphic.RefreshDrawing();
                        BasedROICanvas.GraphicsList.Add(graphic);
                    }
                    columnSpace = 0;
                    rowSpace += (unitYPitch + prevrowOffset.Y);
                    prevOffset = new System.Drawing.Point(0, 0);
                    m_Algo.Offset = new System.Drawing.Point(0, 0);
                }
            }

            switch (aSymmetryType)
            {
                case SymmetryType.Matrix:
                    aFiducialGraphic.OriginObjectColor = GraphicsColors.Green;
                    break;
                case SymmetryType.XFlip:
                case SymmetryType.YFlip:
                case SymmetryType.XYFlip:
                    aFiducialGraphic.OriginObjectColor = GraphicsColors.YellowGreen;
                    break;
                case SymmetryType.Unknown:
                    aFiducialGraphic.OriginObjectColor = GraphicsColors.Purple;
                    aFiducialGraphic.IsValidRegion = false;
                    aFiducialGraphic.IsFiducialRegion = false;
                    aFiducialGraphic.Caption = CaptionHelper.GetRegionCaption(aFiducialGraphic);

                    // 식별 불가한 모델 배치인 경우 모든 ROI의 Valid를 false로 전환시킨다.
                    foreach (GraphicsRectangle g in BasedROICanvas.GraphicsRectangleList)
                    {
                        g.MotherROI = null;
                        g.MotherROIID = -1;
                        g.IsValidRegion = false;
                    }
                    break;
            }
            aFiducialGraphic.RefreshDrawing();

            // Update Fiducial Graphic List.
            if (aSymmetryType != SymmetryType.Unknown)
            {
                BasedROICanvas.FiduGraphicsList.Add(aFiducialGraphic);
                if (pairFiducialGraphic != null)
                    BasedROICanvas.FiduGraphicsList.Add(pairFiducialGraphic);
            }
            BasedROICanvas.Refresh();
        }

        public bool SearchROIPatternSub(GraphicsRectangle aGraphic, SymmetryType aSymmetryType, BitmapSource absSrc)
        {
            if (aGraphic == null || lbSection.SelectedIndex != -1)
                return false;
            GraphicsRectangle pairFiducialGraphic = null; // 비대칭 모델인 경우의 또 하나의 기준 ROI

            m_Algo.SetImage(BitmapHelper.BitmapSource2Bitmap(BasedImage.Source as BitmapSource));
            m_Algo.XOffsetSetZero();
            m_Algo.YOffsetSetZero();

            m_Algo.SetTemplateImage(absSrc, VisionDefinition.GRAB_IMAGE_SCALE);

            int nMarginWidth = (aGraphic.WidthProperty / 4 > 300) ? 300 : (int)aGraphic.WidthProperty / 4;
            int nMarginHeight = (aGraphic.HeightProperty / 4 > 150) ? 150 : (int)aGraphic.HeightProperty / 4;
            System.Drawing.Point SearchMargin = new System.Drawing.Point(nMarginWidth, nMarginHeight);
            int top = (int)aGraphic.Top - (int)(aGraphic.IterationYPosition * aGraphic.IterationValue.YPitch);// + aGraphic.BlockIterationValue.Row * aGraphic.BlockIterationValue.Gap);
            System.Drawing.Point ptStart = new System.Drawing.Point(nMarginWidth, top);
            System.Drawing.Point pt = new System.Drawing.Point(0, 0);
            if (!m_Algo.SearchTemplateImage(ptStart, SearchMargin, 0.65, pt))
            {
                return false;
            }
            double diff = aGraphic.Left - m_Algo.m_ptStartOffset.X;
            GraphicsRectangle aFiducialGraphic = new GraphicsRectangle(
                                m_Algo.m_ptStartOffset.X, top + m_Algo.m_ptStartOffset.Y, m_Algo.m_ptStartOffset.X + aGraphic.WidthProperty, top + m_Algo.m_ptStartOffset.Y + aGraphic.HeightProperty,
                                aGraphic.LineWidth, aGraphic.RegionType, GraphicsColors.YellowGreen, aGraphic.ActualScale);
            aFiducialGraphic.IsValidRegion = true;
            aFiducialGraphic.IterationXPosition = 0;
            aFiducialGraphic.IterationYPosition = 0;
            aFiducialGraphic.BlockIterationValue = aFiducialGraphic.BlockIterationValue;
            aFiducialGraphic.IterationValue = aFiducialGraphic.IterationValue;
            aFiducialGraphic.Caption = CaptionHelper.GetRegionCaption(aFiducialGraphic);
            SetROIIteration(aFiducialGraphic);
            aFiducialGraphic.RefreshDrawing();
            BasedROICanvas.GraphicsList.Add(aFiducialGraphic);

            // Template ROI는 현재 DrawingCanvas 위에 얹어진 ROI여야 한다.
            if (BasedROICanvas.GraphicsList.Contains(aFiducialGraphic))
            {
                #region Set Local Variables.
                int unitColumns = aFiducialGraphic.IterationValue.Column;
                int unitRows = aFiducialGraphic.IterationValue.Row;
                double unitXPitch = aFiducialGraphic.IterationValue.XPitch;
                double unitYPitch = aFiducialGraphic.IterationValue.YPitch;

                int blocknum = aFiducialGraphic.BlockIterationValue.Block;
                int blockColumns = aFiducialGraphic.BlockIterationValue.Column;
                int blockRows = aFiducialGraphic.BlockIterationValue.Row;
                double blockgap = aFiducialGraphic.BlockIterationValue.Gap;
                double blockXPitch = aFiducialGraphic.BlockIterationValue.XPitch;
                double blockYPitch = aFiducialGraphic.BlockIterationValue.YPitch;

                double rowSpace = 0;
                double columnSpace = 0;

                int unitRowPerBlockNum = ((double)unitRows / (double)blocknum > unitRows / blocknum) ? unitRows / blocknum + 1 : unitRows / blocknum;
                int unitRowPerBlockRow = ((double)unitRowPerBlockNum / (double)blockRows > unitRowPerBlockNum / blockRows) ? unitRowPerBlockNum / blockRows + 1 : unitRowPerBlockNum / blockRows;
                int unitColumnPerBlockColumn = ((double)unitColumns / (double)blockColumns > unitColumns / blockColumns) ? unitColumns / blockColumns + 1 : unitColumns / blockColumns;
                #endregion

                //m_Algo.SetImage(BitmapHelper.BitmapSource2Bitmap(BasedImage.Source as BitmapSource));
                m_Algo.XOffsetSetZero();
                m_Algo.YOffsetSetZero();

                System.Drawing.Point ptNext = new System.Drawing.Point();
                System.Drawing.Rectangle rectSearch = new System.Drawing.Rectangle((int)aFiducialGraphic.Left, (int)aFiducialGraphic.Top,
                                                                                   (int)aFiducialGraphic.WidthProperty, (int)aFiducialGraphic.HeightProperty);
                // Search Margin은 최대 30px or ROI 크기의 25% 수준.
                nMarginWidth = (aFiducialGraphic.WidthProperty / 4 > 20) ? 20 : (int)aFiducialGraphic.WidthProperty / 4;
                nMarginHeight = (aFiducialGraphic.HeightProperty / 4 > 20) ? 20 : (int)aFiducialGraphic.HeightProperty / 4;
                System.Drawing.Point sizeSearchMargin = new System.Drawing.Point(nMarginWidth, nMarginHeight);

                // m_Algo.SetTemplateImage(rectSearch);
                System.Drawing.Point prevOffset = new System.Drawing.Point(0, 0);
                System.Drawing.Point prevrowOffset = new System.Drawing.Point(0, 0);
                bool bResult = false;
                for (int row = 0; row < unitRows; row++)
                {
                    if (row % unitRowPerBlockRow == 0 && row >= 1)
                    {
                        if (row >= 1 && row % unitRowPerBlockNum == 0)
                        {
                            rowSpace += blockgap;
                        }
                        else rowSpace += blockYPitch;
                    }

                    for (int column = 0; column < unitColumns; column++)
                    {
                        if (column == 0 && row == 0) continue;

                        if (column == 0)
                        {
                            m_Algo.XOffsetSetZero();
                            sizeSearchMargin.X = nMarginWidth;
                            sizeSearchMargin.Y = nMarginHeight;
                            columnSpace = prevrowOffset.X;
                            prevOffset = new System.Drawing.Point(0, 0);
                        }
                        else
                        {
                            sizeSearchMargin.X = nMarginWidth / 2;
                            sizeSearchMargin.Y = nMarginHeight / 2;
                            columnSpace += (unitXPitch);// + prevOffset.X);
                        }

                        if (column % unitColumnPerBlockColumn == 0 && column >= 1)

                            columnSpace += blockXPitch;



                        ptNext.X = (int)aFiducialGraphic.Left + (int)columnSpace;
                        ptNext.Y = (int)aFiducialGraphic.Top + (int)rowSpace;// +prevOffset.Y;

                        GraphicsRectangle graphic;
                        // m_Algo.Offset = new System.Drawing.Point(0, 0);
                        bResult = m_Algo.SearchTemplateImage(ptNext, sizeSearchMargin, 0.65, m_Algo.Offset);
                        if (bResult)
                        {
                            #region Set Object, Text color of ROI.
                            Color roiColor;
                            switch (aSymmetryType)
                            {
                                case SymmetryType.Matrix:
                                    roiColor = GraphicsColors.Green;
                                    break;
                                case SymmetryType.XFlip:
                                    if (column % 2 == 0)
                                        roiColor = GraphicsColors.YellowGreen;
                                    else
                                        roiColor = GraphicsColors.DodgerBlue;
                                    break;
                                case SymmetryType.YFlip:
                                    if (row % 2 == 0)
                                        roiColor = GraphicsColors.YellowGreen;
                                    else
                                        roiColor = GraphicsColors.DodgerBlue;
                                    break;
                                case SymmetryType.XYFlip:
                                    if ((row + column) % 2 == 0)
                                        roiColor = GraphicsColors.YellowGreen;
                                    else
                                        roiColor = GraphicsColors.DodgerBlue;
                                    break;
                                case SymmetryType.Unknown:
                                default:
                                    roiColor = GraphicsColors.Purple;
                                    break;
                            }
                            #endregion

                            double fLeft = ptNext.X + m_Algo.Offset.X;
                            double fTop = ptNext.Y + m_Algo.Offset.Y;
                            prevOffset = new System.Drawing.Point(m_Algo.Offset.X, m_Algo.Offset.Y);
                            if (column == 0)
                                prevrowOffset = new System.Drawing.Point(m_Algo.Offset.X, m_Algo.Offset.Y);
                            graphic = new GraphicsRectangle(
                                fLeft, fTop, fLeft + aFiducialGraphic.WidthProperty, fTop + aFiducialGraphic.HeightProperty,
                                aFiducialGraphic.LineWidth, aFiducialGraphic.RegionType, roiColor, aFiducialGraphic.ActualScale);
                            graphic.IsValidRegion = true;
                            graphic.IterationXPosition = column;
                            graphic.IterationYPosition = row;
                            graphic.BlockIterationValue = aFiducialGraphic.BlockIterationValue;
                            graphic.IterationValue = aFiducialGraphic.IterationValue;
                            graphic.Caption = CaptionHelper.GetRegionCaption(graphic);
                        }
                        else
                        {
                            graphic = new GraphicsRectangle(
                                ptNext.X, ptNext.Y, ptNext.X + aFiducialGraphic.WidthProperty, ptNext.Y + aFiducialGraphic.HeightProperty,
                                aFiducialGraphic.LineWidth, aFiducialGraphic.RegionType, GraphicsColors.Blue, aFiducialGraphic.ActualScale);
                            graphic.IsValidRegion = false;
                            graphic.IterationXPosition = column;
                            graphic.IterationYPosition = row;
                            graphic.BlockIterationValue = aFiducialGraphic.BlockIterationValue;
                            graphic.IterationValue = aFiducialGraphic.IterationValue;
                            graphic.Caption = CaptionHelper.GetRegionCaption(graphic);
                        }

                        if (aSymmetryType == SymmetryType.Matrix)
                        {
                            graphic.MotherROI = aFiducialGraphic;
                        }
                        else
                        {
                            switch (aSymmetryType)
                            {
                                case SymmetryType.XFlip:
                                    if (column % 2 == 0)
                                        graphic.MotherROI = aFiducialGraphic;
                                    else
                                        graphic.MotherROI = pairFiducialGraphic;
                                    break;
                                case SymmetryType.YFlip:
                                    if (row % 2 == 0)
                                        graphic.MotherROI = aFiducialGraphic;
                                    else
                                        graphic.MotherROI = pairFiducialGraphic;
                                    break;
                                case SymmetryType.XYFlip:
                                    if ((row + column) % 2 == 0)
                                        graphic.MotherROI = aFiducialGraphic;
                                    else
                                        graphic.MotherROI = pairFiducialGraphic;
                                    break;
                            }
                        }

                        // 비대칭 모델인 경우 기준 ROI에 대응되는 또 하나의 기준 ROI를 식별함.
                        if (column + row == 1)
                        {
                            if (column == 1 && (aSymmetryType == SymmetryType.XFlip || aSymmetryType == SymmetryType.XYFlip))
                            {
                                // column = 1, row = 0 case.
                                graphic.IsValidRegion = true;
                                graphic.IsFiducialRegion = true;
                                graphic.SymmetryValue.StartX = graphic.IterationXPosition + 1;
                                graphic.SymmetryValue.StartY = graphic.IterationYPosition + 1;
                                graphic.SymmetryValue.JumpX = 2;
                                graphic.SymmetryValue.JumpY = (aSymmetryType == SymmetryType.XFlip) ? 1 : 2;
                                if (graphic.RegionType == GraphicsRegionType.UnitRegion)
                                    graphic.Caption = CaptionHelper.FiducialUnitRegionCaption;
                                else if (graphic.RegionType == GraphicsRegionType.PSROdd)
                                    graphic.Caption = CaptionHelper.FiducialPsrRegionCaption;
                                graphic.MotherROI = null; // 기준 ROI는 MotherROI가 null이다.
                                pairFiducialGraphic = graphic;
                            }
                            else if (row == 1 && aSymmetryType == SymmetryType.YFlip)
                            {
                                // column = 0, row = 1 case.
                                graphic.IsValidRegion = true;
                                graphic.IsFiducialRegion = true;
                                graphic.SymmetryValue.StartX = graphic.IterationXPosition + 1;
                                graphic.SymmetryValue.StartY = graphic.IterationYPosition + 1;
                                graphic.SymmetryValue.JumpX = 1;
                                graphic.SymmetryValue.JumpY = 2;
                                if (graphic.RegionType == GraphicsRegionType.UnitRegion)
                                    graphic.Caption = CaptionHelper.FiducialUnitRegionCaption;
                                else if (graphic.RegionType == GraphicsRegionType.PSROdd)
                                    graphic.Caption = CaptionHelper.FiducialPsrRegionCaption;
                                graphic.MotherROI = null; // 기준 ROI는 MotherROI가 null이다.
                                pairFiducialGraphic = graphic;
                            }
                        }
                        graphic.RefreshDrawing();
                        BasedROICanvas.GraphicsList.Add(graphic);
                    }
                    columnSpace = 0;
                    rowSpace += (unitYPitch + prevrowOffset.Y);
                    prevOffset = new System.Drawing.Point(0, 0);
                    m_Algo.Offset = new System.Drawing.Point(0, 0);
                }
            }

            switch (aSymmetryType)
            {
                case SymmetryType.Matrix:
                    aFiducialGraphic.OriginObjectColor = GraphicsColors.Green;
                    break;
                case SymmetryType.XFlip:
                case SymmetryType.YFlip:
                case SymmetryType.XYFlip:
                    aFiducialGraphic.OriginObjectColor = GraphicsColors.YellowGreen;
                    break;
                case SymmetryType.Unknown:
                    aFiducialGraphic.OriginObjectColor = GraphicsColors.Purple;
                    aFiducialGraphic.IsValidRegion = false;
                    aFiducialGraphic.IsFiducialRegion = false;
                    aFiducialGraphic.Caption = CaptionHelper.GetRegionCaption(aFiducialGraphic);

                    // 식별 불가한 모델 배치인 경우 모든 ROI의 Valid를 false로 전환시킨다.
                    foreach (GraphicsRectangle g in BasedROICanvas.GraphicsRectangleList)
                    {
                        g.MotherROI = null;
                        g.MotherROIID = -1;
                        g.IsValidRegion = false;
                    }
                    break;
            }
            aFiducialGraphic.RefreshDrawing();

            // Update Fiducial Graphic List.
            if (aSymmetryType != SymmetryType.Unknown)
            {
                BasedROICanvas.FiduGraphicsList.Add(aFiducialGraphic);
                if (pairFiducialGraphic != null)
                    BasedROICanvas.FiduGraphicsList.Add(pairFiducialGraphic);
            }
            BasedROICanvas.Refresh();
            return true;
        }

        private void SetROIIteration(GraphicsRectangle aRoi)
        {
            double fBlockgap = MainWindow.CurrentModel.Strip.BlockGap / CamResolutionY * 1000 * ReferenceImageScale;
            double fUnitXPitch = MainWindow.CurrentModel.Strip.UnitHeight / CamResolutionX * 1000 * ReferenceImageScale;
            double fUnitYPitch = MainWindow.CurrentModel.Strip.UnitWidth / CamResolutionY * 1000 * ReferenceImageScale;
            aRoi.BlockIterationValue = new IterationInformation(MainWindow.CurrentModel.Strip.Block, 1, 1, fBlockgap, 0, 0);

            int nRow = (int)Math.Floor(MainWindow.CurrentModel.Strip.UnitRow / 2.0);
            if (ID == 1)
            {
                if (MainWindow.CurrentModel.Strip.UnitRow % 2 == 1)
                    nRow++;
            }
            else
                nRow = MainWindow.CurrentModel.Strip.UnitRow;

            aRoi.IterationValue = new IterationInformation(nRow, MainWindow.CurrentModel.Strip.UnitColumn, fUnitXPitch, fUnitYPitch);
            aRoi.IsValidRegion = true;
            aRoi.IsFiducialRegion = true;
            aRoi.IterationXPosition = 0;
            aRoi.IterationYPosition = 0;
            aRoi.SymmetryValue.StartX = 1;
            aRoi.SymmetryValue.StartY = 1;
            aRoi.SymmetryValue.JumpX = 1;
            aRoi.SymmetryValue.JumpY = 1;
            TeachingWindow.Symmetry = SymmetryType.Matrix;
        }

        public void SeeEntireImage()
        {
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
                return;

            m_fReferenceImageScale = VisionDefinition.GRAB_IMAGE_SCALE;
            TeachingSubMenuCtrl.btnGuideLine.IsEnabled = false;
            m_ptrTeachingWindow.DisableViewUnitFunction();
            btnEditImage.IsEnabled = false;

            BitmapSource bitmapSource = (GrabImage != null) ? GrabImage : ReferenceImage;
            IsGrabView = (GrabImage != null);

            if (bitmapSource == null)
            {
                this.txtReferenceLabel.Visibility = Visibility.Hidden;
                MessageBox.Show("등록된 취득 영상이 없습니다.", "Information");
            }
            else
            {
                lbSection.SelectedIndex = -1;

                UpdateViewerSource(bitmapSource);
                if (bitmapSource != null)
                    m_ptrTeachingWindow.LineProfileCtrl.SetLineProfileSource(bitmapSource);
                UpdateReferenceLabel();
            }
        }

        public void SeeReferenceImage()
        {
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null) return;
            string szGroupName = MainWindow.CurrentGroup.Name;
            string szModelName = MainWindow.CurrentModel.Name;
            string szReferenceImagePath = string.Empty;
            int nChannel = (int)SelectedChannel;

            if (lbSection.SelectedIndex == -1)
            {
                if (!m_ptrTeachingWindow.IsOnLine && m_ptrTeachingWindow.MtsManager.SelectedMachine != null)
                    szReferenceImagePath = DirectoryManager.GetOfflineBasedImagePath(MainWindow.Setting.General.ModelPath,
                        m_ptrTeachingWindow.MtsManager.SelectedMachine.Name, MainWindow.CurrentGroup.Name, MainWindow.CurrentModel.Name, SurfaceType);
                else
                    szReferenceImagePath = DirectoryManager.GetBasedImagePath(MainWindow.Setting.General.ModelPath, MainWindow.CurrentGroup.Name, MainWindow.CurrentModel.Name, SurfaceType);

                if(!Load_Standard_Image(szReferenceImagePath, nChannel))return;
            }
            else
            {
                SectionInformation section = lbSection.SelectedItem as SectionInformation;
               
                int tmp = this.ID - 1; if (this.ID <= 1) tmp = 0;
                if (!RGB) nChannel = -1;
                if (section != null)
                {
                    if (!m_ptrTeachingWindow.IsOnLine && m_ptrTeachingWindow.MtsManager.SelectedMachine != null)
                    {
                        szReferenceImagePath = DirectoryManager.GetOfflineSectionImagePath(MainWindow.Setting.General.ModelPath,
                            m_ptrTeachingWindow.MtsManager.SelectedMachine.Name, szGroupName, szModelName, section.Name, SurfaceType);

                        if (!Load_Standard_Image(szReferenceImagePath, nChannel)) return;
                    }
                    else
                    {
                        if (nChannel != -1)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                szReferenceImagePath = DirectoryManager.GetSectionImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName, section.Name, SurfaceType, i);
                                if (!Load_Standard_Image(szReferenceImagePath, i)) return;
                            }
                        }
                        else
                        {
                            MainWindow.IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(this.ID);
                            Surface surface;
                            surface = SurfaceType;
                            if (IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave)
                            {
                                surface = (Surface)((int)SurfaceType - IndexInfo.SurfaceScanCount);                             
                            }                           
                            szReferenceImagePath = DirectoryManager.GetSectionImagePath(MainWindow.Setting.General.ModelPath, szGroupName, szModelName, section.Name, surface, nChannel);                
                            if (!Load_Standard_Image(szReferenceImagePath, nChannel)) return;
                        }
                    }
                }
            }

            //// result action.
            //if (!string.IsNullOrEmpty(szReferenceImagePath) && File.Exists(szReferenceImagePath))
            //{
            //    IsGrabView = false;
            //    DisplayImage(szReferenceImagePath, nChannel);
            //    ThumbnailViewer.SetSourceImage(this);
            //    this.txtReferenceLabel.Visibility = Visibility.Visible;
            //    //  m_ptrTeachingWindow.HistogramCtrl.SetRefImage(this.ReferenceImage);
            //}
            //else MessageBox.Show("등록된 기준영상이 없습니다.", "Information");
        }

        public bool Load_Standard_Image(string szReferenceImagePath, int nChannel)
        {
            // result action.
            if (!string.IsNullOrEmpty(szReferenceImagePath) && File.Exists(szReferenceImagePath))
            {
                IsGrabView = false;
                DisplayImage(szReferenceImagePath, nChannel);
                ThumbnailViewer.SetSourceImage(this);
                this.txtReferenceLabel.Visibility = Visibility.Visible;
                //  m_ptrTeachingWindow.HistogramCtrl.SetRefImage(this.ReferenceImage);
                return true;
            }
            else
            { 
                MessageBox.Show("등록된 기준영상이 없습니다.", "Information");
                return false;
            }
        }

        public void Make_ROI_Width_Multiple16()
        {
            GraphicsRectangle CurrentROI = null;
            CurrentROI = TeachingCanvas.SelectedGraphic as GraphicsRectangle;
            bool bUse_PSRODD_Modify = false;
            if (CurrentROI != null)
            {
                foreach (InspectionItem Insptype in CurrentROI.InspectionList)
                {
                    if (Insptype.InspectionType == InspectionType.GetInspectionType(eVisInspectType.eInspTypePSROdd))
                    {
                        if(CurrentROI.Rectangle.Width % 16 != 0) bUse_PSRODD_Modify = true;
                        break;
                    }
                }
                if (bUse_PSRODD_Modify)
                {
                    int right = (int)CurrentROI.Rectangle.Right;
                    if (right % 16 > 8)
                    {
                        right = right + (16 - (right % 16));
                    }
                    else
                    {
                        right = right - right % 16;
                    }
                    int left = (int)CurrentROI.Rectangle.Left;
                    if (left % 16 > 8)
                    {
                        left = left + (16 - (left % 16));
                    }
                    else
                    {
                        left = left - left % 16;
                    }
                    CurrentROI.UpdateRect(left, CurrentROI.Rectangle.Top, right, CurrentROI.Rectangle.Bottom);
                }
            }
        }
    }
}