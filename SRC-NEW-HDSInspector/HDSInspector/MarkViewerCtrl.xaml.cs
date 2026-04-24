using Common;
using Common.DataBase;
using Common.Drawing;
using Common.Drawing.InspectionInformation;
using Common.Drawing.MarkingInformation;
using HDSInspector.SubWindow;
using PCS;
using PCS.ELF.AVI;
using PCS.ModelTeaching;
using RMS.Generic.UserManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    public partial class MarkViewerCtrl : UserControl
    {
        #region Public member variables.
        public LaserWindow m_ptrLaserWindow;
        public Point[] StripAlign = new Point[3];

        public static event ToolTypeChangeEventHandler ToolTypeChangeEvent;

        #endregion
        #region Private member variables.
        private double m_fReferenceImageScale = VisionDefinition.GRAB_IMAGE_SCALE; // 전체영상은 1/4 스케일로 받는다.
        private readonly SectionManager m_SectionManager = new SectionManager();

        private bool m_bFMDrawing = false;
        private bool m_bGLDrawing = false;
        private bool m_bSGDrawing = false;
        private eMarkingType m_nRailType = eMarkingType.eMarkingNone;

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
        public DrawingCanvas MarkROICanvas { get; set; }

        public MarkInformation Mark_Info { get; set; }

        public double ReferenceImageScale
        {
            get { return m_fReferenceImageScale; }
        }
        public double CamResolutionX { get; set; }
        public double CamResolutionY { get; set; }

        public double ViewerHeight { get; set; }
        public double ViewerWidth { get; set; }
        private bool markmode = true;

        public MarkParams markparam;

        public DrawingCanvas TeachingCanvas
        {
            get
            {
                return MarkROICanvas;
            }
        }

        public BitmapSource TeachingImageSource
        {
            get
            {
                return ReferenceImage;
            }
        }

        public Image TeachingImage
        {
            get
            {
                return BasedImage;
            }
        }

        public bool MarkMode
        {
            get { return markmode; }
            set { markmode = value; }
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

        private double ZoomValue
        {
            get { return sldrScale.Value; }
            set
            {
                sldrScale.Value = value;
                UpdateScale();
            }
        }

        int CurrParam = 0;
        #endregion

        public MarkViewerCtrl()
        {
            InitializeComponent();
            InitializeEvent();
            InitializeDialog();
        }

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

            MarkROICanvas = new DrawingCanvas(true, true)
            {
                // 상부 반사 - 0 / 하부 반사 - 1 / 상부 투과 - 2 고정값.
                MaxGraphicsCount = 1024, // 전체영상에서는 ROI(Section)을 64개까지 그릴 수 있다.
                Background = new SolidColorBrush(Colors.Transparent),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 0,
                Height = 0
            };

            ThumbnailViewer.SetSourceImage(this);

            this.IsShowPaintState = true;
            this.IsGrabDone = false;
            this.IsSentDone = false;

            ToolChange(ToolType.Pointer);

            MarkMode = true;
            MarkROICanvas.Visibility = Visibility.Visible;
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

            // Others.
            this.svTeaching.ScrollChanged += svTeaching_ScrollChanged;
            this.svTeaching.PreviewKeyDown += ViewerCtrlPreviewKeyDown;
            this.svTeaching.PreviewKeyUp += ViewerCtrlPreviewKeyUp;

            this.MarkSubMenuCtrl.TeachingToolChangeEvent += ToolChangeEvent;

            this.txtLength.TextChanged += txtLength_TextChanged;
            this.txtWidth.TextChanged += txtWidth_TextChanged;
            this.txtHeight.TextChanged += txtHeight_TextChanged;
            this.Loaded += MarkViewerCtrl_Loaded;
            DrawingCanvas.MarkGraphicChangeEvent += DrawingCanvas_MarkGraphicChangeEvent;
            DrawingCanvas.MarkGraphicMoveEvent += DrawingCanvas_MarkGraphicMoveEvent;
            DrawingCanvas.ContextMenuChangeEvent += DrawingCanvas_ContextMenuChangeEvent;
            this.btnSavePower.Click += btnSavePower_Click;
            this.btnSavePos.Click += btnSavePos_Click;

        }

        void DrawingCanvas_ContextMenuChangeEvent(ContextMenuCommand aSelectedContextMenuCommand)
        {
            switch (aSelectedContextMenuCommand)
            {
                case ContextMenuCommand.ReloadMark:
                    ReloadMark();
                    break;

                case ContextMenuCommand.CopyRailColumn:
                    RailMarkChange();
                    break;

                case ContextMenuCommand.CopyRailColumn2:
                    RailMarkChange2();
                    break;

                case ContextMenuCommand.CopyRailZero:
                    RailMarkGapYZero();
                    break;
            }
        }

        void btnSavePos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.CurrentModel.Marker.MarkStartPosX = Convert.ToDouble(txtMarkPosX.Text);
                MainWindow.CurrentModel.Marker.MarkStartPosY = Convert.ToDouble(txtMarkPosY.Text);
                MainWindow.CurrentModel.Marker.UMarkStartPosX = Convert.ToDouble(txtUMarkPosX.Text);
                MainWindow.CurrentModel.Marker.UMarkStartPosY = Convert.ToDouble(txtUMarkPosY.Text);
                MainWindow.CurrentModel.Marker.BoatAngle = Convert.ToDouble(txtBoatAngle.Text);
                MainWindow.CurrentModel.Marker.LaserAngle = Convert.ToDouble(txtLaserAngle.Text);
                PCS.ELF.AVI.ModelManager.UpdateModelMarkPos(new Point(MainWindow.CurrentModel.Marker.MarkStartPosX, MainWindow.CurrentModel.Marker.MarkStartPosY),
                                                            new Point(MainWindow.CurrentModel.Marker.UMarkStartPosX, MainWindow.CurrentModel.Marker.UMarkStartPosY),
                                                            MainWindow.CurrentModel.Marker.BoatAngle, MainWindow.CurrentModel.Marker.LaserAngle,
                                                            MainWindow.CurrentModel.Code);

                MessageBox.Show("Laser 값 변경 적용, 전체 저장시 적용 됩니다.");
            }
            catch { }
        }

        void btnSavePower_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mark_Info.pen[CurrParam].DrawStep = Convert.ToInt32(txtDrawStep.Text);
                Mark_Info.pen[CurrParam].JumpDelay = Convert.ToInt32(txtJumpDelay.Text);
                Mark_Info.pen[CurrParam].JumpStep = Convert.ToInt32(txtJumpStep.Text);
                Mark_Info.pen[CurrParam].LineDelay = Convert.ToInt32(txtLineDelay.Text);
                Mark_Info.pen[CurrParam].StepPeriod = Convert.ToInt32(txtStepPeriod.Text);
                Mark_Info.pen[CurrParam].LaserOnDelay = Convert.ToInt32(txtOnDelay.Text);
                Mark_Info.pen[CurrParam].LaserOffDelay = Convert.ToInt32(txtOffDelay.Text);
                Mark_Info.pen[CurrParam].CornerDelay = Convert.ToInt32(txtCornerDelay.Text);
                Mark_Info.pen[CurrParam].Frequency = Convert.ToInt32(txtFrequency.Text);
                Mark_Info.pen[CurrParam].LampCurrent = Convert.ToDouble(txtCurrent.Text);
                Mark_Info.pen[CurrParam].PulseDuty = Convert.ToInt32(txtDuty.Text);
                if (!Mark_Info.pen[CurrParam].Save())
                {
                    MessageBox.Show("Pen Parameter저장에 실패 하였습니다.");
                }
            }
            catch
            {
                MessageBox.Show("숫자 변환에 실패 하였습니다.");
            }
        }

        void DrawingCanvas_MarkGraphicMoveEvent(GraphicsBase aNewGraphic)
        {
            MarkGraphicChange(aNewGraphic);
        }

        void DrawingCanvas_MarkGraphicChangeEvent(GraphicsBase aNewGraphic)
        {
            MarkGraphicChange(aNewGraphic);
        }

        public void InitMark(bool bOpen)
        {
            if (MainWindow.Setting.SubSystem.Laser.UseLaser)
                LoadMarkingData(MainWindow.Setting.SubSystem.Laser.IP);

            markparam = MainWindow.CurrentModel.CopyToMarkParams();
            map.SetUnitNum(MainWindow.CurrentModel.Strip.UnitRow, MainWindow.CurrentModel.Strip.UnitColumn);            
        }

        public bool MarkExists()
        {
            bool exist = Common.Drawing.MarkingInformation.MarkInformation.ModelExists(MainWindow.CurrentModel.Name, MainWindow.Setting.SubSystem.Laser.IP);

            return exist;
        }

        void UnitGuideMove(GraphicsBase aNewGraphic, Point p)
        {
            double res = MainWindow.Setting.SubSystem.IS.CamResolutionX[CID.CA] * 4.0 / 1000.0;
            double ofsy = 2.0 * 1000.0 / MainWindow.Setting.SubSystem.IS.CamResolutionY[CID.CA] * 4.0;
            double top = 0;
            double right = 0;
            if (this.MarkROICanvas != null)
            {
                UnitGuideProperty ugproperty = aNewGraphic.MarkInfo.MarkInfo as UnitGuideProperty;
                top = Math.Min(((GraphicsRectangleBase)aNewGraphic).Top, ((GraphicsRectangleBase)aNewGraphic).Top);
                right = Math.Max(((GraphicsRectangleBase)aNewGraphic).Right, ((GraphicsRectangleBase)aNewGraphic).Right);
                ugproperty.StartX = (top - this.MarkROICanvas.StripGuidePoint.Y) * res - p.X;
                ugproperty.StartY = 60.0 - (right - this.MarkROICanvas.StripGuidePoint.X) * res - p.Y;
                Strip strip = this.Mark_Info.UnitMark.template.Strips[0];
                strip.FirstX = ugproperty.StartX;
                strip.FirstY = ugproperty.StartY;
                this.MarkingTypeCtrl.DisplayChange();
                this.MarkROICanvas.UnitGuidePoint = new Point(right, top);
                for (int i = 0; i < this.MarkROICanvas.GraphicsList.Count; i++)
                {
                    GraphicsBase ga = this.MarkROICanvas.GraphicsList[i] as GraphicsBase;
                    if (ga.RegionType == GraphicsRegionType.MarkingUnit && ga.Dummy)
                    {
                        this.MarkROICanvas.GraphicsList.RemoveAt(i);
                        i--;
                    }
                }
                List<GraphicsBase> vc = new List<GraphicsBase>();
                vc.Clear();
                double resX = CamResolutionX * 4.0;
                double resY = CamResolutionY * 4.0;
                foreach (GraphicsBase g in this.MarkROICanvas.GraphicsList)
                {
                    if (g.RegionType == GraphicsRegionType.MarkingUnit && !g.Dummy)
                    {
                        GraphicsEllipseBase ga1 = null;
                        GraphicsRectangleBase ga = null;
                        if (g.MarkInfo.MarkType.MarkType == eMarkingType.eMarkingUnitCircle)
                            ga1 = g as GraphicsEllipseBase;
                        else ga = g as GraphicsRectangleBase;
                        switch (g.MarkInfo.MarkType.MarkType)
                        {
                            case eMarkingType.eMarkingUnitCircle:
                                UnitCircleProperty unitcircleproperty = g.MarkInfo.MarkInfo as UnitCircleProperty;
                                ga1.Right = this.MarkROICanvas.UnitGuidePoint.X - unitcircleproperty.Top * 1000.0 / resX;
                                ga1.Top = this.MarkROICanvas.UnitGuidePoint.Y + unitcircleproperty.Left * 1000.0 / resY;
                                ga1.Left = ga1.Right - unitcircleproperty.Height * 1000.0 / resX;
                                ga1.Bottom = ga1.Top + unitcircleproperty.Width * 1000.0 / resY;
                                break;
                            case eMarkingType.eMarkingUnitRect:
                                UnitRectProperty unitrectproperty = g.MarkInfo.MarkInfo as UnitRectProperty;
                                ga.Right = this.MarkROICanvas.UnitGuidePoint.X - unitrectproperty.Top * 1000.0 / resX;
                                ga.Top = this.MarkROICanvas.UnitGuidePoint.Y + unitrectproperty.Left * 1000.0 / resY;
                                ga.Left = ga.Right - unitrectproperty.Height * 1000.0 / resX;
                                ga.Bottom = ga.Top + unitrectproperty.Width * 1000.0 / resY;
                                break;
                            case eMarkingType.eMarkingUnitTri:
                                UnitTriProperty unittriproperty = g.MarkInfo.MarkInfo as UnitTriProperty;
                                ga.Right = this.MarkROICanvas.UnitGuidePoint.X - unittriproperty.Top * 1000.0 / resX;
                                ga.Top = this.MarkROICanvas.UnitGuidePoint.Y + unittriproperty.Left * 1000.0 / resY;
                                ga.Left = ga.Right - unittriproperty.Height * 1000.0 / resX;
                                ga.Bottom = ga.Top + unittriproperty.Width * 1000.0 / resY;
                                break;
                            case eMarkingType.eMarkingUnitSpecial:
                                UnitSpecialProperty unitspecialproperty = g.MarkInfo.MarkInfo as UnitSpecialProperty;
                                ga.Right = this.MarkROICanvas.UnitGuidePoint.X - unitspecialproperty.Top * 1000.0 / resX;
                                ga.Top = this.MarkROICanvas.UnitGuidePoint.Y + unitspecialproperty.Left * 1000.0 / resY;
                                ga.Left = ga.Right - unitspecialproperty.Height * 1000.0 / resX;
                                ga.Bottom = ga.Top + unitspecialproperty.Width * 1000.0 / resY;
                                //unitspecialproperty.Shape = ((GraphicsSpecialMark)g).Shape;
                                break;
                        }
                        if (g.MarkInfo.MarkType.MarkType == eMarkingType.eMarkingUnitCircle)
                            ga1.RefreshDrawing();
                        else
                            ga.RefreshDrawing();
                        vc.Add(g);
                        //TeachingViewer.SelectedViewer.Mark_Info.AddUnitDummy(TeachingViewer.SelectedViewer.MarkROICanvas, g, TeachingViewer.SelectedViewer.StripAlign);
                    }
                }
                ModelMarkInfo model = new ModelMarkInfo(MainWindow.CurrentModel.Strip.MarkStep, MainWindow.CurrentModel.Strip.StepPitch, MainWindow.CurrentModel.Strip.StepUnits,
                                                                        MainWindow.CurrentModel.Strip.UnitRow, MainWindow.CurrentModel.Strip.UnitColumn, MainWindow.CurrentModel.Strip.UnitWidth, MainWindow.CurrentModel.Strip.UnitHeight,
                                                                        MainWindow.CurrentModel.Strip.Block, MainWindow.CurrentModel.Strip.BlockGap);
                for (int i = 0; i < vc.Count; i++)
                {
                    this.Mark_Info.AddUnitDummyReverse(this.MarkROICanvas, vc[i], this.StripAlign, model);

                }
            }
        }

        public void MarkGraphicChange(GraphicsBase aNewGraphic)
        {
            if (aNewGraphic.RegionType == GraphicsRegionType.UnitGuide)
            {
                Point p = new Point(MainWindow.CurrentModel.Marker.UMarkStartPosX, MainWindow.CurrentModel.Marker.UMarkStartPosY);

                UnitGuideMove(aNewGraphic, p);
                return;
            }
            double resX = CamResolutionX * 4.0 / 1000.0;
            double resY = CamResolutionY * 4.0 / 1000.0;
            ModelMarkInfo model = new ModelMarkInfo(MainWindow.CurrentModel.Strip.MarkStep, MainWindow.CurrentModel.Strip.StepPitch, MainWindow.CurrentModel.Strip.StepUnits,
                                                                        MainWindow.CurrentModel.Strip.UnitRow, MainWindow.CurrentModel.Strip.UnitColumn, MainWindow.CurrentModel.Strip.UnitWidth, MainWindow.CurrentModel.Strip.UnitHeight,
                                                                        MainWindow.CurrentModel.Strip.Block, MainWindow.CurrentModel.Strip.BlockGap);

            if (MarkROICanvas != null)
            {

                foreach (GraphicsBase g in MarkROICanvas.GraphicsList)
                {
                    if (g == aNewGraphic)
                    {
                        this.MarkingTypeCtrl.SelectedGraphic = g;
                        double top = 0;
                        double right = 0;
                        if (g.RegionType == GraphicsRegionType.MarkingRail)
                        {
                            Strip strip = Mark_Info.RailMark.template.Strips[0];
                            switch (g.MarkInfo.MarkType.MarkType)
                            {
                                case eMarkingType.eMarkingRailCircle:
                                    RailCircleProperty railcircleproperty = g.MarkInfo.MarkInfo as RailCircleProperty;
                                    top = Math.Min(((GraphicsEllipseMark)g).Top, ((GraphicsEllipseMark)g).Bottom);
                                    right = Math.Max(((GraphicsEllipseMark)g).Right, ((GraphicsEllipseMark)g).Left);
                                    if (!g.Dummy)
                                    {
                                        railcircleproperty.FirstX = Math.Round((top - MarkROICanvas.StripGuidePoint.Y) * resY, 3);
                                        railcircleproperty.FirstY = 60.0 - Math.Round((right - MarkROICanvas.StripGuidePoint.X) * resX, 3);
                                        strip.FirstX = railcircleproperty.FirstX;
                                        strip.FirstY = railcircleproperty.FirstY;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].DispLeft = 0.0;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].DispTop = 0.0;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].DispRight = railcircleproperty.Width;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].DispBottom = railcircleproperty.Height;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].RotateAngle = railcircleproperty.Rotate;
                                    }
                                    else
                                    {
                                        double ox = 0.0;
                                        double ax = 0.0;
                                        if (StripAlign[0].Y > 0 && StripAlign[1].Y > 0)
                                        {
                                            ox = (StripAlign[1].Y - StripAlign[0].Y) / (StripAlign[1].X - StripAlign[0].X);
                                            ax = ((top - MarkROICanvas.StripGuidePoint.Y) + (strip.FirstX * 1000.0) / (CamResolutionY * 4.0)) / ox;
                                            ax = ax * resY;
                                        }

                                        railcircleproperty.GapX = Math.Round((top - MarkROICanvas.StripGuidePoint.Y) * resY, 3) - strip.FirstX - g.UnitColumn * MainWindow.CurrentModel.Strip.UnitWidth;
                                        railcircleproperty.GapX -= g.Step * MainWindow.CurrentModel.Strip.StepPitch;
                                        railcircleproperty.GapY = (60.0 - Math.Round((right - MarkROICanvas.StripGuidePoint.X) * resX, 3)) - strip.FirstY + ax;
                                        Strip s = Mark_Info.RailMark.template.Strips[g.Step];
                                        s.OneChipOffset[g.UnitRow + (model.UnitRow * g.UnitColumn)].X = railcircleproperty.GapX;
                                        s.OneChipOffset[g.UnitRow + (model.UnitRow * g.UnitColumn)].Y = railcircleproperty.GapY;
                                    }
                                    //railcircleproperty.Width = Math.Round((((GraphicsEllipseMark)g).HeightProperty) * res, 3);
                                    //railcircleproperty.Height = Math.Round((((GraphicsEllipseMark)g).WidthProperty) * res, 3);
                                    break;
                                case eMarkingType.eMarkingRailRect:
                                    RailRectProperty railrectproperty = g.MarkInfo.MarkInfo as RailRectProperty;
                                    top = Math.Min(((GraphicsRectangleMark)g).Top, ((GraphicsRectangleMark)g).Bottom);
                                    right = Math.Max(((GraphicsRectangleMark)g).Right, ((GraphicsRectangleMark)g).Left);
                                    if (!g.Dummy)
                                    {
                                        railrectproperty.FirstX = Math.Round((top - MarkROICanvas.StripGuidePoint.Y) * resY, 3);
                                        railrectproperty.FirstY = 60.0 - Math.Round((right - MarkROICanvas.StripGuidePoint.X) * resX, 3);
                                        strip.FirstX = railrectproperty.FirstX;
                                        strip.FirstY = railrectproperty.FirstY;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].DispLeft = 0.0;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].DispTop = 0.0;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].DispRight = railrectproperty.Width;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].DispBottom = railrectproperty.Height;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].RotateAngle = railrectproperty.Rotate;
                                    }
                                    else
                                    {
                                        double ox = 0.0;
                                        double ax = 0.0;
                                        if (StripAlign[0].Y > 0 && StripAlign[1].Y > 0)
                                        {
                                            ox = (StripAlign[1].Y - StripAlign[0].Y) / (StripAlign[1].X - StripAlign[0].X);
                                            ax = ((top - MarkROICanvas.StripGuidePoint.Y) + (strip.FirstX * 1000.0) / (CamResolutionY * 4.0)) / ox;
                                            ax = ax * resY;
                                        }

                                        railrectproperty.GapX = Math.Round((top - MarkROICanvas.StripGuidePoint.Y) * resY, 3) - strip.FirstX - g.UnitColumn * strip.SpecialPitch[0];
                                        railrectproperty.GapX -= g.Step * MainWindow.CurrentModel.Strip.StepPitch;
                                        railrectproperty.GapY = (60.0 - Math.Round((right - MarkROICanvas.StripGuidePoint.X) * resX, 3)) - strip.FirstY + ax;
                                        Strip s = Mark_Info.RailMark.template.Strips[g.Step];
                                        s.OneChipOffset[g.UnitRow + (model.UnitRow * g.UnitColumn)].X = railrectproperty.GapX;
                                        s.OneChipOffset[g.UnitRow + (model.UnitRow * g.UnitColumn)].Y = railrectproperty.GapY;
                                    }
                                    break;
                                case eMarkingType.eMarkingRailTri:
                                    RailTriProperty railtriproperty = g.MarkInfo.MarkInfo as RailTriProperty;
                                    top = Math.Min(((GraphicsTriangleMark)g).Top, ((GraphicsTriangleMark)g).Bottom);
                                    right = Math.Max(((GraphicsTriangleMark)g).Right, ((GraphicsTriangleMark)g).Left);
                                    if (!g.Dummy)
                                    {
                                        railtriproperty.FirstX = Math.Round((top - MarkROICanvas.StripGuidePoint.Y) * resY, 3);
                                        railtriproperty.FirstY = 60.0 - Math.Round((right - MarkROICanvas.StripGuidePoint.X) * resX, 3);
                                        strip.FirstX = railtriproperty.FirstX;
                                        strip.FirstY = railtriproperty.FirstY;
                                        railtriproperty.Rotate = ((GraphicsTriangleMark)g).Rotate;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].DispLeft = 0.0;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].DispTop = 0.0;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].DispRight = railtriproperty.Width;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].DispBottom = railtriproperty.Height;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].RotateAngle = railtriproperty.Rotate;
                                    }
                                    else
                                    {
                                        double ox = 0.0;
                                        double ax = 0.0;
                                        if (StripAlign[0].Y > 0 && StripAlign[1].Y > 0)
                                        {
                                            ox = (StripAlign[1].Y - StripAlign[0].Y) / (StripAlign[1].X - StripAlign[0].X);
                                            ax = ((top - MarkROICanvas.StripGuidePoint.Y) + (strip.FirstX * 1000.0) / (CamResolutionY * 4.0)) / ox;
                                            ax = ax * resY;
                                        }

                                        railtriproperty.GapX = Math.Round((top - MarkROICanvas.StripGuidePoint.Y) * resY, 3) - strip.FirstX - g.UnitColumn * strip.SpecialPitch[0];
                                        railtriproperty.GapX -= g.Step * MainWindow.CurrentModel.Strip.StepPitch;
                                        railtriproperty.GapY = (60.0 - Math.Round((right - MarkROICanvas.StripGuidePoint.X) * resX, 3)) - strip.FirstY + ax;
                                        Strip s = Mark_Info.RailMark.template.Strips[g.Step];
                                        s.OneChipOffset[g.UnitRow + (model.UnitRow * g.UnitColumn)].X = railtriproperty.GapX;
                                        s.OneChipOffset[g.UnitRow + (model.UnitRow * g.UnitColumn)].Y = railtriproperty.GapY;
                                    }
                                    break;
                                case eMarkingType.eMarkingRailSpecial:
                                    RailSpecialProperty railspecialproperty = g.MarkInfo.MarkInfo as RailSpecialProperty;
                                    top = Math.Min(((GraphicsSpecialMark)g).Top, ((GraphicsSpecialMark)g).Bottom);
                                    right = Math.Max(((GraphicsSpecialMark)g).Right, ((GraphicsSpecialMark)g).Left);
                                    if (!g.Dummy)
                                    {
                                        railspecialproperty.FirstX = Math.Round((top - MarkROICanvas.StripGuidePoint.Y) * resY, 3);
                                        railspecialproperty.FirstY = 60.0 - Math.Round((right - MarkROICanvas.StripGuidePoint.X) * resX, 3);
                                        strip.FirstX = railspecialproperty.FirstX;
                                        strip.FirstY = railspecialproperty.FirstY;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].DispLeft = 0.0;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].DispTop = 0.0;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].DispRight = railspecialproperty.Width;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].DispBottom = railspecialproperty.Height;
                                        Mark_Info.RailMark.template.Sems.lstHPGL[g.MarkOBJID].RotateAngle = railspecialproperty.Rotate;
                                    }
                                    else
                                    {
                                        double ox = 0.0;
                                        double ax = 0.0;
                                        if (StripAlign[0].Y > 0 && StripAlign[1].Y > 0)
                                        {
                                            ox = (StripAlign[1].Y - StripAlign[0].Y) / (StripAlign[1].X - StripAlign[0].X);
                                            ax = ((top - MarkROICanvas.StripGuidePoint.Y) + (strip.FirstX * 1000.0) / (CamResolutionY * 4.0)) / ox;
                                            ax = ax * resY;
                                        }

                                        railspecialproperty.GapX = Math.Round((top - MarkROICanvas.StripGuidePoint.Y) * resY, 3) - strip.FirstX - g.UnitColumn * strip.SpecialPitch[0];
                                        railspecialproperty.GapX -= g.Step * MainWindow.CurrentModel.Strip.StepPitch;
                                        railspecialproperty.GapY = (60.0 - Math.Round((right - MarkROICanvas.StripGuidePoint.X) * resX, 3)) - strip.FirstY + ax;
                                        Strip s = Mark_Info.RailMark.template.Strips[g.Step];
                                        s.OneChipOffset[g.UnitRow + (model.UnitRow * g.UnitColumn)].X = railspecialproperty.GapX;
                                        s.OneChipOffset[g.UnitRow + (model.UnitRow * g.UnitColumn)].Y = railspecialproperty.GapY;
                                    }
                                    //unitspecialproperty.Shape = ((GraphicsSpecialMark)g).Shape;
                                    break;
                            }
                            this.MarkingTypeCtrl.DisplayChange();
                            if (!g.Dummy)
                            {
                                for (int i = 0; i < MarkROICanvas.GraphicsList.Count; i++)
                                {
                                    if (((GraphicsBase)MarkROICanvas.GraphicsList[i]).MarkID == MarkROICanvas.SelectedGraphic.MarkID)
                                    {
                                        if (((GraphicsBase)MarkROICanvas.GraphicsList[i]).Dummy)
                                        {
                                            MarkROICanvas.GraphicsList.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                }
                                Mark_Info.AddRailDummy(MarkROICanvas, MarkROICanvas.SelectedGraphic,
                                                                                StripAlign, strip, model);
                            }
                        }
                        else if (g.RegionType == GraphicsRegionType.UnitGuide)
                        {
                            Strip strip = Mark_Info.UnitMark.template.Strips[0];
                            GraphicsRectangleBase rr = g as GraphicsRectangleBase;
                            top = Math.Min(((GraphicsSpecialMark)g).Top, ((GraphicsSpecialMark)g).Bottom);
                            right = Math.Max(((GraphicsSpecialMark)g).Right, ((GraphicsSpecialMark)g).Left);
                            if (!g.Dummy)
                            {
                                strip.FirstX = Math.Round((top - MarkROICanvas.StripGuidePoint.Y) * resY, 3);
                                strip.FirstY = 60.0 - Math.Round((right - MarkROICanvas.StripGuidePoint.X) * resX, 3);
                            }
                        }
                        #region Unit
                        else if (g.RegionType == GraphicsRegionType.MarkingUnit)
                        {
                            Strip strip = Mark_Info.UnitMark.template.Strips[0];
                            switch (g.MarkInfo.MarkType.MarkType)
                            {
                                case eMarkingType.eMarkingUnitCircle:
                                    UnitCircleProperty unitcircleproperty = g.MarkInfo.MarkInfo as UnitCircleProperty;
                                    top = Math.Min(((GraphicsEllipseMark)g).Top, ((GraphicsEllipseMark)g).Bottom);
                                    right = Math.Max(((GraphicsEllipseMark)g).Right, ((GraphicsEllipseMark)g).Left);
                                    unitcircleproperty.Left = Math.Round((top - MarkROICanvas.UnitGuidePoint.Y) * resY, 3);
                                    unitcircleproperty.Top = Math.Round((MarkROICanvas.UnitGuidePoint.X - right) * resX, 3);
                                    unitcircleproperty.Width = Math.Round((((GraphicsEllipseMark)g).HeightProperty) * resY, 3);
                                    unitcircleproperty.Height = Math.Round((((GraphicsEllipseMark)g).WidthProperty) * resX, 3);
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].DispLeft = unitcircleproperty.Left;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].DispTop = unitcircleproperty.Top;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].DispRight = unitcircleproperty.Left + unitcircleproperty.Width;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].DispBottom = unitcircleproperty.Top + unitcircleproperty.Height;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].RotateAngle = unitcircleproperty.Rotate;

                                    break;
                                case eMarkingType.eMarkingUnitRect:
                                    UnitRectProperty unitrectproperty = g.MarkInfo.MarkInfo as UnitRectProperty;
                                    top = Math.Min(((GraphicsRectangleMark)g).Top, ((GraphicsRectangleMark)g).Bottom);
                                    right = Math.Max(((GraphicsRectangleMark)g).Right, ((GraphicsRectangleMark)g).Left);
                                    unitrectproperty.Left = Math.Round((top - MarkROICanvas.UnitGuidePoint.Y) * resY, 3);
                                    unitrectproperty.Top = Math.Round((MarkROICanvas.UnitGuidePoint.X - right) * resX, 3);
                                    unitrectproperty.Width = Math.Round((((GraphicsRectangleMark)g).HeightProperty) * resY, 3);
                                    unitrectproperty.Height = Math.Round((((GraphicsRectangleMark)g).WidthProperty) * resX, 3);
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].DispLeft = unitrectproperty.Left;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].DispTop = unitrectproperty.Top;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].DispRight = unitrectproperty.Left + unitrectproperty.Width;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].DispBottom = unitrectproperty.Top + unitrectproperty.Height;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].RotateAngle = unitrectproperty.Rotate;
                                    break;
                                case eMarkingType.eMarkingUnitTri:
                                    UnitTriProperty unittriproperty = g.MarkInfo.MarkInfo as UnitTriProperty;
                                    top = Math.Min(((GraphicsTriangleMark)g).Top, ((GraphicsTriangleMark)g).Bottom);
                                    right = Math.Max(((GraphicsTriangleMark)g).Right, ((GraphicsTriangleMark)g).Left);
                                    unittriproperty.Left = Math.Round((top - MarkROICanvas.UnitGuidePoint.Y) * resY, 3);
                                    unittriproperty.Top = Math.Round((MarkROICanvas.UnitGuidePoint.X - right) * resX, 3);
                                    unittriproperty.Width = Math.Round((((GraphicsTriangleMark)g).HeightProperty) * resY, 3);
                                    unittriproperty.Height = Math.Round((((GraphicsTriangleMark)g).WidthProperty) * resX, 3);
                                    unittriproperty.Rotate = ((GraphicsTriangleMark)g).Rotate;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].DispLeft = unittriproperty.Left;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].DispTop = unittriproperty.Top;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].DispRight = unittriproperty.Left + unittriproperty.Width;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].DispBottom = unittriproperty.Top + unittriproperty.Height;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].RotateAngle = unittriproperty.Rotate;
                                    break;
                                case eMarkingType.eMarkingUnitSpecial:
                                    UnitSpecialProperty unitspecialproperty = g.MarkInfo.MarkInfo as UnitSpecialProperty;
                                    top = Math.Min(((GraphicsSpecialMark)g).Top, ((GraphicsSpecialMark)g).Bottom);
                                    right = Math.Max(((GraphicsSpecialMark)g).Right, ((GraphicsSpecialMark)g).Left);
                                    unitspecialproperty.Left = Math.Round((top - MarkROICanvas.UnitGuidePoint.Y) * resY, 3);
                                    unitspecialproperty.Top = Math.Round((MarkROICanvas.UnitGuidePoint.X - right) * resX, 3);
                                    unitspecialproperty.Width = Math.Round((((GraphicsSpecialMark)g).HeightProperty) * resY, 3);
                                    unitspecialproperty.Height = Math.Round((((GraphicsSpecialMark)g).WidthProperty) * resX, 3);
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].DispLeft = unitspecialproperty.Left;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].DispTop = unitspecialproperty.Top;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].DispRight = unitspecialproperty.Left + unitspecialproperty.Width;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].DispBottom = unitspecialproperty.Top + unitspecialproperty.Height;
                                    Mark_Info.UnitMark.template.Sems.lstHPGL[g.MarkOBJID].RotateAngle = unitspecialproperty.Rotate;
                                    //unitspecialproperty.Shape = ((GraphicsSpecialMark)g).Shape;
                                    break;

                            }
                            this.MarkingTypeCtrl.DisplayChange();
                            if (g.RegionType == GraphicsRegionType.MarkingUnit)
                            {
                                for (int i = 0; i < MarkROICanvas.GraphicsList.Count; i++)
                                {
                                    if (((GraphicsBase)MarkROICanvas.GraphicsList[i]).MarkID == MarkROICanvas.SelectedGraphic.MarkID)
                                    {
                                        if (((GraphicsBase)MarkROICanvas.GraphicsList[i]).Dummy)
                                        {
                                            MarkROICanvas.GraphicsList.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                }
                                Mark_Info.AddUnitDummyReverse(MarkROICanvas, MarkROICanvas.SelectedGraphic, StripAlign, model);

                            }
                        }
                        #endregion
                        else if (g.RegionType == GraphicsRegionType.MarkingReject)
                        {
                            int ip = MainWindow.CurrentModel.Marker.IDMark;
                            switch (g.MarkInfo.MarkType.MarkType)
                            {
                                case eMarkingType.eMarkingTBD:
                                    TBDProperty tbdproperty = g.MarkInfo.MarkInfo as TBDProperty;
                                    top = Math.Min(((GraphicsTBD)g).Top, ((GraphicsTBD)g).Bottom);
                                    right = Math.Max(((GraphicsTBD)g).Right, ((GraphicsTBD)g).Left);
                                    tbdproperty.Left = Math.Round((top - MarkROICanvas.StripRejectGuidePoint[ip].Y) * resY, 3) - Mark_Info.RejMark.template.Strips[0].FirstX;
                                    tbdproperty.Top = 60.0 - Math.Round((right - MarkROICanvas.StripRejectGuidePoint[ip].X) * resX, 3) - Mark_Info.RejMark.template.Strips[0].FirstY;
                                    tbdproperty.Width = Math.Round((((GraphicsTBD)g).HeightProperty) * resY, 3);
                                    tbdproperty.Height = Math.Round((((GraphicsTBD)g).WidthProperty) * resX, 3);
                                    Mark_Info.RejMark.template.Sems.lstIDMark[g.MarkOBJID].DispLeft = tbdproperty.Left;
                                    Mark_Info.RejMark.template.Sems.lstIDMark[g.MarkOBJID].DispTop = tbdproperty.Top;
                                    Mark_Info.RejMark.template.Sems.lstIDMark[g.MarkOBJID].DispRight = tbdproperty.Left + tbdproperty.Width;
                                    Mark_Info.RejMark.template.Sems.lstIDMark[g.MarkOBJID].DispBottom = tbdproperty.Top + tbdproperty.Height;
                                    Mark_Info.RejMark.template.Sems.lstIDMark[g.MarkOBJID].RotateAngle = tbdproperty.Rotate;
                                    break;
                                case eMarkingType.eMarkingIDMark:
                                    IDMarkProperty idproperty = g.MarkInfo.MarkInfo as IDMarkProperty;
                                    top = Math.Min(((GraphicsIDMark)g).Top, ((GraphicsIDMark)g).Bottom);
                                    right = Math.Max(((GraphicsIDMark)g).Right, ((GraphicsIDMark)g).Left);

                                    idproperty.Left = Math.Round((top - MarkROICanvas.StripRejectGuidePoint[ip].Y) * resY, 3) - Mark_Info.RejMark.template.Strips[0].FirstX;
                                    idproperty.Top = 60.0 - Math.Round((right - MarkROICanvas.StripRejectGuidePoint[ip].X) * resX, 3) - Mark_Info.RejMark.template.Strips[0].FirstY;
                                    if (((GraphicsIDMark)g).Rotate == 90 || ((GraphicsIDMark)g).Rotate == 270)
                                    {
                                        idproperty.Width = Math.Round((((GraphicsIDMark)g).WidthProperty) * resY, 3);
                                        idproperty.Height = Math.Round((((GraphicsIDMark)g).HeightProperty) * resX, 3);
                                    }
                                    else
                                    {
                                        idproperty.Width = Math.Round((((GraphicsIDMark)g).HeightProperty) * resY, 3);
                                        idproperty.Height = Math.Round((((GraphicsIDMark)g).WidthProperty) * resX, 3);
                                    }
                                    idproperty.Rotate = ((GraphicsIDMark)g).Rotate;
                                    Mark_Info.RejMark.template.Sems.lstText[g.MarkOBJID].DispLeft = idproperty.Left;
                                    Mark_Info.RejMark.template.Sems.lstText[g.MarkOBJID].DispTop = idproperty.Top;
                                    Mark_Info.RejMark.template.Sems.lstText[g.MarkOBJID].DispRight = idproperty.Left + idproperty.Width;
                                    Mark_Info.RejMark.template.Sems.lstText[g.MarkOBJID].DispBottom = idproperty.Top + idproperty.Height;
                                    Mark_Info.RejMark.template.Sems.lstText[g.MarkOBJID].RotateAngle = idproperty.Rotate;
                                    break;
                                case eMarkingType.eMarkingWeek:
                                    int wp = MainWindow.CurrentModel.Marker.WeekPos;
                                    double sp = MainWindow.CurrentModel.Strip.StepPitch;
                                    WeekProperty weekproperty = g.MarkInfo.MarkInfo as WeekProperty;
                                    top = Math.Min(((GraphicsWeekMark)g).Top, ((GraphicsWeekMark)g).Bottom);
                                    right = Math.Max(((GraphicsWeekMark)g).Right, ((GraphicsWeekMark)g).Left);

                                    weekproperty.Left = Math.Round((top - MarkROICanvas.StripRejectGuidePoint[wp].Y) * resY, 3) - Mark_Info.RejMark.template.Strips[0].FirstX;
                                    weekproperty.Top = 60.0 - Math.Round((right - MarkROICanvas.StripRejectGuidePoint[wp].X) * resX, 3) - Mark_Info.RejMark.template.Strips[0].FirstY;
                                    if (((GraphicsWeekMark)g).Rotate == 90 || ((GraphicsWeekMark)g).Rotate == 270)
                                    {
                                        weekproperty.Width = Math.Round((((GraphicsWeekMark)g).WidthProperty) * resY, 3);
                                        weekproperty.Height = Math.Round((((GraphicsWeekMark)g).HeightProperty) * resX, 3);
                                    }
                                    else
                                    {
                                        weekproperty.Width = Math.Round((((GraphicsWeekMark)g).HeightProperty) * resY, 3);
                                        weekproperty.Height = Math.Round((((GraphicsWeekMark)g).WidthProperty) * resX, 3);
                                    }
                                    //if (MainWindow.CurrentModel.Strip.StepPitch < weekproperty.Left + weekproperty.Width || weekproperty.Left < 0)
                                    //{
                                    //    MessageBox.Show("레이져 범위를 넘어 섰습니다. 위치를 확인해주세요"); break;
                                    //}
                                    weekproperty.Rotate = ((GraphicsWeekMark)g).Rotate;
                                    Mark_Info.RejMark.template.Sems.lstText[g.MarkOBJID].DispLeft = weekproperty.Left;
                                    Mark_Info.RejMark.template.Sems.lstText[g.MarkOBJID].DispTop = weekproperty.Top;
                                    Mark_Info.RejMark.template.Sems.lstText[g.MarkOBJID].DispRight = weekproperty.Left + weekproperty.Width;
                                    Mark_Info.RejMark.template.Sems.lstText[g.MarkOBJID].DispBottom = weekproperty.Top + weekproperty.Height;
                                    Mark_Info.RejMark.template.Sems.lstText[g.MarkOBJID].RotateAngle = weekproperty.Rotate;
                                    break;
                                case eMarkingType.eMarkingNumber:
                                    int cp = MainWindow.CurrentModel.Marker.NumLeft ? 0 : 1;
                                    NumberProperty numberproperty = g.MarkInfo.MarkInfo as NumberProperty;
                                    if (((GraphicsNumberMark)g).Rotate == 90 || ((GraphicsNumberMark)g).Rotate == 270)
                                    {
                                        top = (Math.Min(((GraphicsNumberMark)g).Top, ((GraphicsNumberMark)g).Bottom));// -(((GraphicsNumberMark)g).HeightProperty - ((GraphicsNumberMark)g).WidthProperty) / 2.0;
                                        right = Math.Max(((GraphicsNumberMark)g).Right, ((GraphicsNumberMark)g).Left);// +(((GraphicsNumberMark)g).HeightProperty - ((GraphicsNumberMark)g).WidthProperty) / 2.0;
                                        numberproperty.Left = Math.Round((top - MarkROICanvas.StripRejectGuidePoint[cp].Y) * resY, 3) - Mark_Info.RejMark.template.Strips[0].FirstX;
                                        numberproperty.Top = 60.0 - Math.Round((right - MarkROICanvas.StripRejectGuidePoint[cp].X) * resX, 3) - Mark_Info.RejMark.template.Strips[0].FirstY;
                                        numberproperty.Height = Math.Round((((GraphicsNumberMark)g).HeightProperty) * resY, 3);
                                        numberproperty.Width = Math.Round((((GraphicsNumberMark)g).WidthProperty) * resX, 3);

                                    }
                                    else
                                    {
                                        top = Math.Min(((GraphicsNumberMark)g).Top, ((GraphicsNumberMark)g).Bottom);
                                        right = Math.Max(((GraphicsNumberMark)g).Right, ((GraphicsNumberMark)g).Left);
                                        numberproperty.Left = Math.Round((top - MarkROICanvas.StripRejectGuidePoint[cp].Y) * resY, 3) - Mark_Info.RejMark.template.Strips[0].FirstX;
                                        numberproperty.Top = 60.0 - Math.Round((right - MarkROICanvas.StripRejectGuidePoint[cp].X) * resX, 3) - Mark_Info.RejMark.template.Strips[0].FirstY;
                                        numberproperty.Height = Math.Round((((GraphicsNumberMark)g).WidthProperty) * resY, 3);
                                        numberproperty.Width = Math.Round((((GraphicsNumberMark)g).HeightProperty) * resX, 3);
                                    }

                                    numberproperty.Rotate = ((GraphicsNumberMark)g).Rotate;
                                    Mark_Info.RejMark.template.Sems.lstText[g.MarkOBJID].DispLeft = numberproperty.Left;
                                    Mark_Info.RejMark.template.Sems.lstText[g.MarkOBJID].DispTop = numberproperty.Top;
                                    Mark_Info.RejMark.template.Sems.lstText[g.MarkOBJID].DispRight = numberproperty.Left + numberproperty.Width;
                                    Mark_Info.RejMark.template.Sems.lstText[g.MarkOBJID].DispBottom = numberproperty.Top + numberproperty.Height;
                                    Mark_Info.RejMark.template.Sems.lstText[g.MarkOBJID].RotateAngle = numberproperty.Rotate;
                                    break;

                            }
                            this.MarkingTypeCtrl.DisplayChange();
                        }
                        break;
                    }
                }
            }
        }

        public void ReloadMark()
        {
            LoadMarkingData(MainWindow.Setting.SubSystem.Laser.IP);
        }

        public void RailMarkChange()
        {
            double resX = MainWindow.Setting.SubSystem.IS.CamResolutionX[CID.CA] * 4.0 / 1000.0;
            double resY = MainWindow.Setting.SubSystem.IS.CamResolutionY[CID.CA] * 4.0 / 1000.0;
            ModelMarkInfo model = new ModelMarkInfo(MainWindow.CurrentModel.Strip.MarkStep, MainWindow.CurrentModel.Strip.StepPitch, MainWindow.CurrentModel.Strip.StepUnits,
                                                                        MainWindow.CurrentModel.Strip.UnitRow, MainWindow.CurrentModel.Strip.UnitColumn, MainWindow.CurrentModel.Strip.UnitWidth, MainWindow.CurrentModel.Strip.UnitHeight,
                                                                        MainWindow.CurrentModel.Strip.Block, MainWindow.CurrentModel.Strip.BlockGap);
            bool railirr = MainWindow.CurrentModel.Marker.RailIrr;

            if (MarkROICanvas != null)
            {
                if (railirr)
                {
                    GraphicsBase g = null;
                    for (int n = 0; n < MainWindow.CurrentModel.Strip.MarkStep; n++)
                    {
                        Strip strip = Mark_Info.RailMark.template.Strips[n];

                        if (n == 0)
                        {
                            for (int i = MainWindow.CurrentModel.Strip.UnitRow; i < strip.OneChipOffset.Length; i++)
                            {
                                strip.OneChipOffset[i].X = strip.OneChipOffset[i % MainWindow.CurrentModel.Strip.UnitRow].X;
                                strip.OneChipOffset[i].Y = strip.OneChipOffset[i % MainWindow.CurrentModel.Strip.UnitRow].Y;
                            }
                        }
                        else
                        {
                            Strip tmp = Mark_Info.RailMark.template.Strips[0];
                            for (int i = 0; i < strip.OneChipOffset.Length; i++)
                            {
                                strip.OneChipOffset[i].X = tmp.OneChipOffset[i % MainWindow.CurrentModel.Strip.UnitRow].X;
                                strip.OneChipOffset[i].Y = tmp.OneChipOffset[i % MainWindow.CurrentModel.Strip.UnitRow].Y;
                            }
                        }
                        if (n == 0)
                        {
                            for (int i = 0; i < MarkROICanvas.GraphicsList.Count; i++)
                            {
                                if (((GraphicsBase)MarkROICanvas.GraphicsList[i]).RegionType == GraphicsRegionType.MarkingRail
                                    && ((GraphicsBase)MarkROICanvas.GraphicsList[i]).Dummy)
                                {
                                    MarkROICanvas.GraphicsList.RemoveAt(i);
                                    i--;
                                }
                                if (((GraphicsBase)MarkROICanvas.GraphicsList[i]).RegionType == GraphicsRegionType.MarkingRail
                                    && !((GraphicsBase)MarkROICanvas.GraphicsList[i]).Dummy)
                                {
                                    g = (GraphicsBase)MarkROICanvas.GraphicsList[i];
                                }
                            }
                        }
                        if (g != null)
                        {
                            Mark_Info.AddRailStepDummy(MarkROICanvas, g, StripAlign, strip, model, n);
                        }
                    }
                }
                else
                {
                    Strip strip = Mark_Info.RailMark.template.Strips[0];
                    strip.SpecialPitch[0] = MainWindow.CurrentModel.Strip.UnitWidth;
                    GraphicsBase g = null;
                    for (int i = MainWindow.CurrentModel.Strip.UnitRow; i < strip.OneChipOffset.Length; i++)
                    {
                        strip.OneChipOffset[i].X = strip.OneChipOffset[i % MainWindow.CurrentModel.Strip.UnitRow].X;
                        strip.OneChipOffset[i].Y = strip.OneChipOffset[i % MainWindow.CurrentModel.Strip.UnitRow].Y;
                    }
                    for (int i = 0; i < MarkROICanvas.GraphicsList.Count; i++)
                    {
                        if (((GraphicsBase)MarkROICanvas.GraphicsList[i]).RegionType == GraphicsRegionType.MarkingRail
                            && ((GraphicsBase)MarkROICanvas.GraphicsList[i]).Dummy)
                        {
                            MarkROICanvas.GraphicsList.RemoveAt(i);
                            i--;
                        }
                        if (((GraphicsBase)MarkROICanvas.GraphicsList[i]).RegionType == GraphicsRegionType.MarkingRail
                            && !((GraphicsBase)MarkROICanvas.GraphicsList[i]).Dummy)
                        {
                            g = (GraphicsBase)MarkROICanvas.GraphicsList[i];
                        }
                    }
                    if (g != null)
                    {
                        Mark_Info.AddRailDummy(MarkROICanvas, g, StripAlign, strip, model);
                    }
                }

            }
        }

        public void RailMarkChange2()
        {
            double resX = CamResolutionX * 4.0 / 1000.0;
            double resY = CamResolutionY * 4.0 / 1000.0;
            ModelMarkInfo model = new ModelMarkInfo(MainWindow.CurrentModel.Strip.MarkStep, MainWindow.CurrentModel.Strip.StepPitch, MainWindow.CurrentModel.Strip.StepUnits,
                                                                        MainWindow.CurrentModel.Strip.UnitRow, MainWindow.CurrentModel.Strip.UnitColumn, MainWindow.CurrentModel.Strip.UnitWidth, MainWindow.CurrentModel.Strip.UnitHeight,
                                                                        MainWindow.CurrentModel.Strip.Block, MainWindow.CurrentModel.Strip.BlockGap);
            bool railirr = MainWindow.CurrentModel.Marker.RailIrr;
            if (MarkROICanvas != null)
            {
                if (railirr)
                {
                    GraphicsBase g = null;
                    for (int n = 0; n < MainWindow.CurrentModel.Strip.MarkStep; n++)
                    {
                        Strip strip = Mark_Info.RailMark.template.Strips[n];

                        if (n == 0)
                        {
                            for (int i = (MainWindow.CurrentModel.Strip.UnitRow * 2); i < strip.OneChipOffset.Length; i++)
                            {
                                strip.OneChipOffset[i].X = strip.OneChipOffset[MainWindow.CurrentModel.Strip.UnitRow + (i % MainWindow.CurrentModel.Strip.UnitRow)].X;
                                strip.OneChipOffset[i].Y = strip.OneChipOffset[MainWindow.CurrentModel.Strip.UnitRow + (i % MainWindow.CurrentModel.Strip.UnitRow)].Y;
                            }
                        }
                        else
                        {
                            Strip tmp = Mark_Info.RailMark.template.Strips[0];
                            for (int i = 0; i < strip.OneChipOffset.Length; i++)
                            {
                                strip.OneChipOffset[i].X = tmp.OneChipOffset[MainWindow.CurrentModel.Strip.UnitRow + (i % MainWindow.CurrentModel.Strip.UnitRow)].X;
                                strip.OneChipOffset[i].Y = tmp.OneChipOffset[MainWindow.CurrentModel.Strip.UnitRow + (i % MainWindow.CurrentModel.Strip.UnitRow)].Y;
                            }
                        }
                        if (n == 0)
                        {
                            for (int i = 0; i < MarkROICanvas.GraphicsList.Count; i++)
                            {
                                if (((GraphicsBase)MarkROICanvas.GraphicsList[i]).RegionType == GraphicsRegionType.MarkingRail
                                    && ((GraphicsBase)MarkROICanvas.GraphicsList[i]).Dummy)
                                {
                                    MarkROICanvas.GraphicsList.RemoveAt(i);
                                    i--;
                                }
                                if (((GraphicsBase)MarkROICanvas.GraphicsList[i]).RegionType == GraphicsRegionType.MarkingRail
                                    && !((GraphicsBase)MarkROICanvas.GraphicsList[i]).Dummy)
                                {
                                    g = (GraphicsBase)MarkROICanvas.GraphicsList[i];
                                }
                            }
                        }
                        if (g != null)
                        {
                            Mark_Info.AddRailStepDummy(MarkROICanvas, g, StripAlign, strip, model, n);
                        }
                    }
                }
                else
                {
                    Strip strip = Mark_Info.RailMark.template.Strips[0];
                    strip.SpecialPitch[0] = MainWindow.CurrentModel.Strip.UnitWidth;
                    GraphicsBase g = null;
                    for (int i = MainWindow.CurrentModel.Strip.UnitRow; i < strip.OneChipOffset.Length; i++)
                    {
                        strip.OneChipOffset[i].X = strip.OneChipOffset[i % MainWindow.CurrentModel.Strip.UnitRow].X;
                        strip.OneChipOffset[i].Y = strip.OneChipOffset[i % MainWindow.CurrentModel.Strip.UnitRow].Y;
                    }
                    for (int i = 0; i < MarkROICanvas.GraphicsList.Count; i++)
                    {
                        if (((GraphicsBase)MarkROICanvas.GraphicsList[i]).RegionType == GraphicsRegionType.MarkingRail
                            && ((GraphicsBase)MarkROICanvas.GraphicsList[i]).Dummy)
                        {
                            MarkROICanvas.GraphicsList.RemoveAt(i);
                            i--;
                        }
                        if (((GraphicsBase)MarkROICanvas.GraphicsList[i]).RegionType == GraphicsRegionType.MarkingRail
                            && !((GraphicsBase)MarkROICanvas.GraphicsList[i]).Dummy)
                        {
                            g = (GraphicsBase)MarkROICanvas.GraphicsList[i];
                        }
                    }
                    if (g != null)
                    {
                        Mark_Info.AddRailDummy(MarkROICanvas, g, StripAlign, strip, model);
                    }
                }

            }
        }

        public void RailMarkGapYZero()
        {
            try
            {
                double resX = CamResolutionX * 4.0 / 1000.0;
                double resY = CamResolutionY * 4.0 / 1000.0;
                ModelMarkInfo model = new ModelMarkInfo(MainWindow.CurrentModel.Strip.MarkStep, MainWindow.CurrentModel.Strip.StepPitch, MainWindow.CurrentModel.Strip.StepUnits,
                                                                            MainWindow.CurrentModel.Strip.UnitRow, MainWindow.CurrentModel.Strip.UnitColumn, MainWindow.CurrentModel.Strip.UnitWidth, MainWindow.CurrentModel.Strip.UnitHeight,
                                                                            MainWindow.CurrentModel.Strip.Block, MainWindow.CurrentModel.Strip.BlockGap);
                bool railirr = MainWindow.CurrentModel.Marker.RailIrr;

                if (MarkROICanvas != null)
                {
                    if (railirr)
                    {
                        GraphicsBase g = null;
                        for (int n = 0; n < MainWindow.CurrentModel.Strip.MarkStep; n++)
                        {
                            Strip strip = Mark_Info.RailMark.template.Strips[n];

                            for (int i = 0; i < strip.OneChipOffset.Length; i++)
                                strip.OneChipOffset[i].Y = 0.00;

                            if (n == 0)
                            {
                                for (int i = 0; i < MarkROICanvas.GraphicsList.Count; i++)
                                {
                                    if (((GraphicsBase)MarkROICanvas.GraphicsList[i]).RegionType == GraphicsRegionType.MarkingRail
                                        && ((GraphicsBase)MarkROICanvas.GraphicsList[i]).Dummy)
                                    {
                                        MarkROICanvas.GraphicsList.RemoveAt(i);
                                        i--;
                                    }
                                    if (((GraphicsBase)MarkROICanvas.GraphicsList[i]).RegionType == GraphicsRegionType.MarkingRail
                                        && !((GraphicsBase)MarkROICanvas.GraphicsList[i]).Dummy)
                                    {
                                        g = (GraphicsBase)MarkROICanvas.GraphicsList[i];
                                    }
                                }
                            }
                            if (g != null)
                            {
                                Mark_Info.AddRailStepDummy(MarkROICanvas, g, StripAlign, strip, model, n);
                            }
                        }
                    }
                    else
                    {
                        Strip strip = Mark_Info.RailMark.template.Strips[0];
                        GraphicsBase g = null;
                        for (int i = 0; i < strip.OneChipOffset.Length; i++)
                        {
                            strip.OneChipOffset[i].Y = 0.0;
                        }
                        for (int i = 0; i < MarkROICanvas.GraphicsList.Count; i++)
                        {
                            if (((GraphicsBase)MarkROICanvas.GraphicsList[i]).RegionType == GraphicsRegionType.MarkingRail
                                && ((GraphicsBase)MarkROICanvas.GraphicsList[i]).Dummy)
                            {
                                MarkROICanvas.GraphicsList.RemoveAt(i);
                                i--;
                            }
                            if (((GraphicsBase)MarkROICanvas.GraphicsList[i]).RegionType == GraphicsRegionType.MarkingRail
                                && !((GraphicsBase)MarkROICanvas.GraphicsList[i]).Dummy)
                            {
                                g = (GraphicsBase)MarkROICanvas.GraphicsList[i];
                            }
                        }
                        if (g != null)
                        {
                            Mark_Info.AddRailDummy(MarkROICanvas, g, StripAlign, strip, model);
                        }
                    }

                }
            }
            catch
            {
                MessageBox.Show("선택된 요소가 없습니다.\nDB연결이 끊어졌거나, 모델이 선택되지 않았습니다.\n프로그램을 다시 시작해 주세요.");
            }
        }

        void MarkViewerCtrl_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewerWidth = svTeaching.ActualWidth;
            this.ViewerHeight = svTeaching.ActualHeight;
            this.CalculateZoomToFitScale();
            SetCameraResolution();
            MarkingTypeCtrl.m_ptrViewer = this;
        }


        #region Update Milimeter textboxes.
        private void txtLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtLength.Text))
            {
                this.txtLengthByResolution.Text = string.Format("({0:f2}mm)", Convert.ToDouble(this.txtLength.Text) * this.CamResolutionX / 1000 / this.ReferenceImageScale);
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
                this.txtWidthByResolution.Text = string.Format("({0:f2}mm)", Convert.ToDouble(this.txtWidth.Text) * this.CamResolutionX / 1000 / this.ReferenceImageScale);
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
                this.txtHeightByResolution.Text = string.Format("({0:f2}mm)", Convert.ToDouble(this.txtHeight.Text) * this.CamResolutionY / 1000 / this.ReferenceImageScale);
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

        public bool ChangeRefImage()
        {
            string szBasedImagePath = DirectoryManager.GetBasedImagePath(MainWindow.Setting.General.ModelPath, MainWindow.CurrentGroup.Name, MainWindow.CurrentModel.Name, Surface.CA1);
            string szMarkImagePath = DirectoryManager.GetMarkImagePath(MainWindow.Setting.General.ModelPath, MainWindow.CurrentGroup.Name, MainWindow.CurrentModel.Name, Surface.CA1);
            if (FileSupport.TryDeleteFile(szMarkImagePath))
            {
                FileSupport.FileCopy(szBasedImagePath, szMarkImagePath);
                SetByCurrentModel();
                InitMark(true);
                return true;
            }
            else return false;
        }

        public void SetByCurrentModel()
        {
            ReferenceImage = LoadBasedImage(); // 기존에 저장된 전체영상을 읽어들인다.
            if (ReferenceImage == null) // 기준 영상을 찾지 못하는 경우
            {
                m_SectionManager.Sections.Clear();
                InitializeSurfaceView();
                this.txtReferenceLabel.Visibility = Visibility.Hidden;
            }
            else // 기준 영상이 있는 경우.
            {
                InitializeSurfaceView();
                this.txtReferenceLabel.Visibility = Visibility.Visible;
            }
        }

        public void LoadMarkingData(string IP)
        {
            bool railirr = MainWindow.CurrentModel.Marker.RailIrr;

            Mark_Info = new MarkInformation(MainWindow.CurrentModel.Name, IP, MainWindow.Setting.SubSystem.IS.CamResolutionX[1] * 4.0, MainWindow.Setting.SubSystem.IS.CamResolutionY[1] * 4.0, railirr, MainWindow.CurrentModel.Strip.MarkStep, !MainWindow.Setting.SubSystem.PLC.UsePLC);
            System.Windows.Point pos = new Point(MainWindow.CurrentModel.Marker.MarkStartPosX, MainWindow.CurrentModel.Marker.MarkStartPosY);
            System.Windows.Point upos = new Point(MainWindow.CurrentModel.Marker.UMarkStartPosX, MainWindow.CurrentModel.Marker.UMarkStartPosY);

            string strMachineCode = MainWindow.Setting.General.MachineCode;
            string szModelCode = MainWindow.CurrentModel.Code;
            string szGroupName = MainWindow.CurrentGroup.Name;
            ROIManager.LoadMarkStripAlignROI(strMachineCode, szGroupName, szModelCode, MarkROICanvas);
            StripAlign[0] = new Point(10, 10);
            StripAlign[1] = new Point(10, 4000);
            StripAlign[2] = new Point(2000, 10);
            foreach (GraphicsBase g in MarkROICanvas.GraphicsList)
            {
                if (g.RegionType == GraphicsRegionType.StripAlign)
                {
                    GraphicsStripAlign gs = g as GraphicsStripAlign;
                    StripAlign[gs.nID] = new Point(gs.cpX, gs.cpY);
                }
            }

            if (!Mark_Info.LoadMarkFile(pos, upos))
            {
                MessageBox.Show("Warning : 마킹 파일 읽기 오류.");
            }
            else
            {
                ModelMarkInfo model = MainWindow.CurrentModel.CopyToMarkInfo();
                //ModelMarkInfo model = new ModelMarkInfo(MainWindow.CurrentModel.Mark.Step, MainWindow.CurrentModel.Mark.StepPitch, MainWindow.CurrentModel.Mark.StepUnits,
                //                                                        MainWindow.CurrentModel.Strip.UnitRow, MainWindow.CurrentModel.Strip.UnitColumn, MainWindow.CurrentModel.Strip.UnitWidth, MainWindow.CurrentModel.Strip.UnitHeight,
                //                                                        MainWindow.CurrentModel.Strip.Block, MainWindow.CurrentModel.Strip.BlockGap);
                double centerY = MainWindow.Setting.SubSystem.Laser.CenterY;
                MarkROICanvas.StripRejectGuidePoint.Clear();
                for (int i = 0; i < model.Step; i++) MarkROICanvas.StripRejectGuidePoint.Add(new System.Windows.Point(0, 0));
                Common.Drawing.MarkingInformation.MarkLogo logo = new Common.Drawing.MarkingInformation.MarkLogo();
                Mark_Info.LoadROI(MarkROICanvas, StripAlign, centerY, model, logo);
                MarkROICanvas.ClearHistory();
            }


        }

        public void InitializeSurfaceView()
        {
            if (ReferenceImage != null)
            {
                UpdateViewerSource(ReferenceImage);
                CalculateZoomToFitScale();
            }
            else // BasedImageSource == null
            {
                UpdateViewerSource(null);
                ZoomValue = 0.1;
            }
            MarkROICanvas.GraphicsList.Clear();
            MarkROICanvas.SelectedGraphic = null;
            //UpdateReferenceLabel();
            SetScrollViewerToHome();

            pnlInner.Children.Clear();
            pnlInner.Children.Add(BasedImage);      // Image Control
                                                    // pnlInner.Children.Add(BasedROICanvas);  // Drawing Canvas
            pnlInner.Children.Add(MarkROICanvas);  // Drawing Canvas

            ToolChange(ToolType.Pointer);
            MarkSubMenuCtrl.ChangeTeachingMode(TeachingType.Entire);
            ThumbnailViewer.SetSourceImage(this);
        }

        /// <summary>   Sets the scroll viewer to home. </summary>
        /// <remarks>   suoow2, 2016-12-12. </remarks>
        public void SetScrollViewerToHome()
        {
            svTeaching.ScrollToHorizontalOffset(0.0);
            svTeaching.ScrollToVerticalOffset(0.0);
        }
        #endregion

        #region Event Handlers.
        /// <summary>   Event handler. Called by TeachingScrollViewer for scroll changed events. </summary>
        /// <remarks>   suoow2, 2014-08-23. </remarks>
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

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY) || newOffsetX < 0 || newOffsetY < 0)
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
            double offsetX = (svTeaching.ExtentWidth) * (afPositionX / TeachingImageSource.PixelWidth);
            double offsetY = (svTeaching.ExtentHeight) * (afPositionY / TeachingImageSource.PixelHeight);

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

            }
            //if (Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Shift)
            //{
            //    if (MarkROICanvas != null)
            //    {
            //        switch (e.Key)
            //        {
            //            case Key.A:
            //                MarkROICanvas.SelectAll();
            //                break;
            //        }
            //    }
            //}
            //else if (e.Key == Key.Space)
            //{
            //    ToolChange(ToolType.Move);
            //}
            e.Handled = true;
        }

        private void pnlOuter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TeachingCanvas == null || TeachingImageSource == null || MarkROICanvas == null)
            {
                return;
            }
            else if ((MarkROICanvas.ActualWidth < e.GetPosition(MarkROICanvas).X || MarkROICanvas.ActualHeight < e.GetPosition(MarkROICanvas).Y))
            {
                return;
            }
            if ((e.RightButton == MouseButtonState.Pressed) && (this.Cursor == System.Windows.Input.Cursors.ScrollAll))
            {
                this.Cursor = Cursors.Arrow;
                ToolChange(ToolType.Pointer);
                return;
            }

            if ((Keyboard.IsKeyDown(Key.Space) || TeachingCanvas.Tool == ToolType.Move) && e.ChangedButton == MouseButton.Left)
            {
                m_ptLastDragPoint = e.GetPosition(svTeaching);
            }
            else if (IsShowPaintState)
            {
                if (MarkROICanvas.Tool == ToolType.SearchRail || MarkROICanvas.Tool == ToolType.RailCirMark ||
                    MarkROICanvas.Tool == ToolType.RailRectMark || MarkROICanvas.Tool == ToolType.RailSpecial || MarkROICanvas.Tool == ToolType.RailTriMark)
                {
                    switch (MarkROICanvas.Tool)
                    {
                        case ToolType.SearchRail: m_nRailType = eMarkingType.eMarkingNone; break;
                        case ToolType.RailCirMark: m_nRailType = eMarkingType.eMarkingRailCircle; break;
                        case ToolType.RailRectMark: m_nRailType = eMarkingType.eMarkingRailRect; break;
                        case ToolType.RailSpecial: m_nRailType = eMarkingType.eMarkingRailSpecial; break;
                        case ToolType.RailTriMark: m_nRailType = eMarkingType.eMarkingRailTri; break;
                    }
                    m_tmpPoint = Mouse.GetPosition(TeachingImage);
                    MarkROICanvas.Tool = ToolType.Pointer;
                    //m_bRailDrawing = true;
                }
                if (MarkROICanvas.Tool == ToolType.GuideLine)
                {
                    m_tmpPoint = Mouse.GetPosition(TeachingImage);
                    MarkROICanvas.Tool = ToolType.Pointer;
                    m_bGLDrawing = true;
                }
                if (MarkROICanvas.Tool == ToolType.StripGuide)
                {
                    m_tmpPoint = Mouse.GetPosition(TeachingImage);
                    MarkROICanvas.Tool = ToolType.Pointer;
                    m_bSGDrawing = true;
                }
                if (MarkROICanvas.Tool == ToolType.SetFirstMark)
                {
                    m_tmpPoint = Mouse.GetPosition(TeachingImage);
                    m_bFMDrawing = true;
                }

                //UpdateReferenceLabel();
                MarkROICanvas.DrawingCanvas_MouseDown(sender, e);

            }
        }

        private Point GetStripAlignDistanceVertical()
        {
            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, 0);
            bool S1 = false;
            bool S2 = false;
            foreach (GraphicsBase graphic in BasedROICanvas.GraphicsList)
            {
                if (graphic.RegionType == GraphicsRegionType.StripAlign)
                {
                    if (BasedROICanvas.CanDraw(graphic))
                    {
                        GraphicsStripAlign g = (GraphicsStripAlign)graphic;
                        if (g.nID == 0)
                        {
                            p1.X = g.cpX;
                            p1.Y = g.cpY;
                            S1 = true;
                        }
                        else if (g.nID == 1)
                        {
                            p2.X = g.cpX;
                            p2.Y = g.cpY;
                            S2 = true;
                        }
                    }
                }
            }
            if (!S1)
            {
                return new Point(0, 0);
            }
            if (!S2)
            {
                return new Point(0, 0);
            }
            return new Point((p2.X - p1.X) *CamResolutionX, (p2.Y - p1.Y) * CamResolutionY);
        }

        public Point GetStripAlignDistanceHorizontal()
        {
            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, 0);
            bool S1 = false;
            bool S2 = false;
            foreach (GraphicsBase graphic in BasedROICanvas.GraphicsList)
            {
                if (graphic.RegionType == GraphicsRegionType.StripAlign)
                {
                    if (BasedROICanvas.CanDraw(graphic))
                    {
                        GraphicsStripAlign g = (GraphicsStripAlign)graphic;
                        if (g.nID == 0)
                        {
                            p1.X = g.cpX;
                            p1.Y = g.cpY;
                            S1 = true;
                        }
                        else if (g.nID == 2)
                        {
                            p2.X = g.cpX;
                            p2.Y = g.cpY;
                            S2 = true;
                        }
                    }
                }
            }
            if (!S1)
            {
                return new Point(0, 0);
            }
            if (!S2)
            {
                return new Point(0, 0);
            }
            return new Point((p2.X - p1.X) * CamResolutionX / 1000.0, (p2.Y - p1.Y) * CamResolutionY / 1000.0);
        }

        public void ChangeDummyGraphics(GraphicsBase graphic, eMarkingType marktype, string strID, int mode, double dx, double dy)
        {
            // if (marktype != eMarkingType.eMarkingRailCircle && marktype != eMarkingType.eMarkingUnitCircle && mode == 2) return;
            foreach (GraphicsBase g in MarkROICanvas.GraphicsList)
            {
                if (g.MarkID == strID && g.Step == graphic.Step)
                {
                    if (marktype == eMarkingType.eMarkingRailCircle || marktype == eMarkingType.eMarkingUnitCircle)
                    {
                        (g as GraphicsEllipseMark).CircleInfo.Radian = (graphic as GraphicsEllipseMark).CircleInfo.Radian;
                        if (mode == 0)
                        {
                            (g as GraphicsEllipseMark).CircleInfo.Position.X += dx;
                            (g as GraphicsEllipseMark).CircleInfo.Position.Y += dy;
                        }
                        else if (mode == 1)
                        {
                            if (g.UnitRow == graphic.UnitRow)
                            {
                                (g as GraphicsEllipseMark).CircleInfo.Position.X += dx;
                                (g as GraphicsEllipseMark).CircleInfo.Position.Y += dy;
                            }
                        }
                        g.RefreshDrawing();
                    }
                    //if (marktype == eMarkingType.eMarkingRailRect)
                    else
                    {
                        (g as GraphicsRectangleBase).Right = (g as GraphicsRectangleBase).Left + (graphic as GraphicsRectangleBase).WidthProperty;
                        (g as GraphicsRectangleBase).Bottom = (g as GraphicsRectangleBase).Top + (graphic as GraphicsRectangleBase).HeightProperty;
                        (g as GraphicsRectangleBase).WidthProperty = (graphic as GraphicsRectangleBase).WidthProperty;
                        (g as GraphicsRectangleBase).HeightProperty = (graphic as GraphicsRectangleBase).HeightProperty;
                        if (mode == 0)
                        {
                            (g as GraphicsRectangleBase).Left += dx;
                            (g as GraphicsRectangleBase).Top += dy;


                        }
                        else if (mode == 1)
                        {
                            if (g.UnitRow == graphic.UnitRow)
                            {
                                (g as GraphicsRectangleBase).Left += dx;
                                (g as GraphicsRectangleBase).Top += dy;
                            }
                        }
                        g.RefreshDrawing();
                    }
                }
            }
            MarkROICanvas.Refresh();
        }

        public string GetMarkID()
        {
            int nid = 0;
            bool ok = false;
            for (int c = 1; c < 9999; c++)
            {
                ok = false;
                foreach (GraphicsBase g in MarkROICanvas.GraphicsList)
                {
                    nid = Convert.ToInt32(g.MarkID);
                    if (c == nid)
                    {
                        ok = true;
                        break;
                    }
                }
                if (!ok)
                {
                    return c.ToString("D4");
                }
            }
            return "0000";
        }

        private void GuideChanged()
        {
            foreach (GraphicsBase g in MarkROICanvas.GraphicsList)
            {
                if (g.RegionType == GraphicsRegionType.MarkingUnit)
                {
                    if (g.MarkInfo.MarkType.MarkType >= eMarkingType.eMarkingUnitCircle && g.MarkInfo.MarkType.MarkType <= eMarkingType.eMarkingUnitSpecial)
                    {

                    }
                }
            }
        }

        private void pnlOuter_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (TeachingCanvas == null) return;

            if (TeachingCanvas.Tool == ToolType.Move)
                m_ptLastDragPoint = null;
            else if (IsShowPaintState)
            {
                if (m_bFMDrawing)
                {
                    m_bFMDrawing = false;
                    Point p = Mouse.GetPosition(TeachingImage);

                    FirstMarkSetWindow FMWindow = new FirstMarkSetWindow();
                    m_tmpPoint.X = -(p.X - m_tmpPoint.X) * CamResolutionX / 1000.0;
                    m_tmpPoint.Y = (p.Y - m_tmpPoint.Y) * CamResolutionY / 1000.0;
                    FMWindow.SetValue(MainWindow.CurrentModel.Marker.MarkStartPosX, MainWindow.CurrentModel.Marker.MarkStartPosY, m_tmpPoint.Y, m_tmpPoint.X);
                    if ((bool)FMWindow.ShowDialog())
                    {
                        this.SaveFirstMarkPos(FMWindow.FMX, FMWindow.FMY);
                    }
                    // SetPosition.Y = (p.Y - SetPosition.Y) * this.ActualScale;
                }
                else if (m_bSGDrawing)
                {
                    m_bSGDrawing = false;
                    Point p = Mouse.GetPosition(TeachingImage);

                    foreach (GraphicsBase g in MarkROICanvas.GraphicsList)
                    {
                        if (g.RegionType == GraphicsRegionType.MarkGuide)
                        {
                            MarkROICanvas.GraphicsList.Remove(g);
                            break;
                        }
                    }

                    //  GraphicsMarkGuide graphic = new GraphicsMarkGuide(p.X, p.Y, MarkROICanvas.LineWidth, MarkROICanvas.ObjectColor, MarkROICanvas.ActualScale);
                    MarkROICanvas.StripGuidePoint = new Point(p.X, p.Y);

                    //graphic.RefreshDrawing();
                    //MarkROICanvas.GraphicsList.Add(graphic);
                    GuideChanged();
                }
                else if (m_bGLDrawing)
                {
                    m_bGLDrawing = false;
                    Point p = Mouse.GetPosition(TeachingImage);
                    double w = 0;
                    double h = 0;
                    if (this.Mark_Info.UnitMark != null)
                    {

                        w = Mark_Info.UnitMark.template.Strips[0].YPitch * 1000.0 / CamResolutionX;
                        h = Mark_Info.UnitMark.template.Strips[0].XPitch * 1000.0 / CamResolutionY;
                    }
                    else
                    {
                        w = MainWindow.CurrentModel.Strip.UnitHeight * 1000.0 / CamResolutionX;
                        h = MainWindow.CurrentModel.Strip.UnitWidth * 1000.0 / CamResolutionY;
                    }
                    if (p.X - w < 0) p.X = w + 1;
                    if (p.Y < 0) p.Y = 1;
                    if (p.X > 3000) p.Y = 3000;
                    if (p.Y + h > 1300) p.Y = 1;

                    foreach (GraphicsBase g in MarkROICanvas.GraphicsList)
                    {
                        if (g.RegionType == GraphicsRegionType.GuideLine)
                        {
                            MarkROICanvas.GraphicsList.Remove(g);
                            break;
                        }
                    }

                    GraphicsGuideLine graphic = new GraphicsGuideLine(p.X - w, p.Y, p.X, p.Y + h, MarkROICanvas.LineWidth, MarkROICanvas.ObjectColor, MarkROICanvas.ActualScale, new Point(0, 0));
                    MarkROICanvas.UnitGuidePoint = new Point(p.X, p.Y);

                    graphic.RefreshDrawing();
                    MarkROICanvas.GraphicsList.Add(graphic);
                    GuideChanged();
                }

                if (MarkROICanvas.DrawingFinished)
                {
                    ToolTypeChangeEventHandler eventRunner = ToolTypeChangeEvent;
                    if (eventRunner != null)
                    {
                        eventRunner(ToolType.Pointer);
                    }
                }
            }
        }

        private void pnlOuter_MouseMove(object sender, MouseEventArgs e)
        {
            if (TeachingImageSource == null || TeachingImage == null || MarkROICanvas == null)
                return;
            try
            {
                System.Windows.Point ptCurrentByViewer = Mouse.GetPosition(svTeaching);
                System.Windows.Point ptCurrentByImage = Mouse.GetPosition(TeachingImage);

                UpdateIndicator();

                if (TeachingImageSource != null)
                {
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

                    }
                    else
                    {
                        this.txtGVValue.Text = "0";
                    }
                }
                if ((MarkROICanvas.Tool == ToolType.Move || (Keyboard.IsKeyDown(Key.Space) && Mouse.LeftButton == MouseButtonState.Pressed)) && m_ptLastDragPoint != null)
                {
                    double fdeltaX = ptCurrentByViewer.X - m_ptLastDragPoint.Value.X;
                    double fdeltaY = ptCurrentByViewer.Y - m_ptLastDragPoint.Value.Y;

                    svTeaching.ScrollToHorizontalOffset(svTeaching.HorizontalOffset - fdeltaX);
                    svTeaching.ScrollToVerticalOffset(svTeaching.VerticalOffset - fdeltaY);

                    m_ptLastDragPoint = ptCurrentByViewer;
                }
                else
                {
                    MarkROICanvas.DrawingCanvas_MouseMove(sender, e);
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
                if (MarkROICanvas != null && MarkROICanvas.GraphicsList.Count > 2000)
                    ZoomValue *= 2.0;
                else
                    ZoomValue *= 1.5;

            }
            else if (deltaValue == 0)
            {
                ZoomValue = m_fZoomToFitScale;
            }
            else
            {
                if (MarkROICanvas != null && MarkROICanvas.GraphicsList.Count > 2000)
                    ZoomValue = (ZoomValue / 2.0 < m_fZoomToFitScale) ? m_fZoomToFitScale : ZoomValue / 2.0;
                else
                    ZoomValue = (ZoomValue / 1.5 < m_fZoomToFitScale) ? m_fZoomToFitScale : ZoomValue / 1.5;


            }
        }

        private void pnlOuter_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (MarkROICanvas == null)
            {
                return;
            }
            MarkROICanvas.DrawingCanvas_LostMouseCapture(sender, e);
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
            sldrScale.Minimum = (m_fZoomToFitScale > 0) ? m_fZoomToFitScale : 0.1;
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

            if (MarkROICanvas != null)
            {
                if (MarkROICanvas.Tool == ToolType.Move)
                {
                    this.Cursor = System.Windows.Input.Cursors.ScrollAll;
                    ToolChange(ToolType.Move);
                }
                else
                {
                    this.Cursor = MarkROICanvas.Cursor;
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

            if (MarkROICanvas != null)
            {
                MarkROICanvas.Visibility = Visibility.Visible;
            }
        }

        /// <summary>   Hidden paint. </summary>
        /// <remarks>   suoow2, 2014-08-22. </remarks>
        public void HiddenPaint()
        {
            IsShowPaintState = false;

            if (MarkROICanvas != null)
            {
                MarkROICanvas.Visibility = Visibility.Hidden;

                if (MarkROICanvas.Tool > ToolType.Pointer)
                {
                    ToolChange(ToolType.Pointer);
                }
            }
        }
        #endregion

        #region Other functions.
        private void UpdateScale()
        {
            if (MarkROICanvas != null)
                MarkROICanvas.ActualScale = ZoomValue;
        }

        public void UpdateViewerSource()
        {
            MarkROICanvas.Width = BasedImage.Width;
            MarkROICanvas.Height = BasedImage.Height;
        }

        public void UpdateViewerSource(BitmapSource aBitmapSource)
        {
            if (aBitmapSource != null)
            {
                MarkROICanvas.Width = BasedImage.Width = aBitmapSource.PixelWidth;
                MarkROICanvas.Height = BasedImage.Height = aBitmapSource.PixelHeight;
                //BasedROICanvas.Width = BasedImage.Width = aBitmapSource.PixelWidth;
                //BasedROICanvas.Height = BasedImage.Height = aBitmapSource.PixelHeight;
                BasedImage.Source = aBitmapSource;
                CalculateZoomToFitScale();
            }
            else
            {
                MarkROICanvas.Width = BasedImage.Width = 0;
                MarkROICanvas.Height = BasedImage.Height = 0;
                // BasedROICanvas.Width = BasedImage.Width = 0;
                // BasedROICanvas.Height = BasedImage.Height = 0;
                BasedImage.Source = null;
            }
        }

        public void ToolChange(ToolType newTool)
        {
            if (MarkROICanvas == null) return;
            this.m_ptLastDragPoint = null;


            MarkROICanvas.UnselectAll();
            MarkROICanvas.Tool = newTool;
            if (newTool == ToolType.GuideLine)
                MarkROICanvas.RegionType = GraphicsRegionType.GuideLine;
            if (newTool == ToolType.StripGuide)
                MarkROICanvas.RegionType = GraphicsRegionType.MarkGuide;
            else if (newTool == ToolType.RailSpecial || newTool == ToolType.RailCirMark || newTool == ToolType.RailRectMark || newTool == ToolType.RailTriMark)
                MarkROICanvas.RegionType = GraphicsRegionType.MarkingRail;
            else if (newTool == ToolType.UnitSpecial || newTool == ToolType.UnitCirMark || newTool == ToolType.UnitRectMark || newTool == ToolType.UnitTriMark)
                MarkROICanvas.RegionType = GraphicsRegionType.MarkingUnit;
            else if (newTool == ToolType.IDMark || newTool == ToolType.Number || newTool == ToolType.Week)
                MarkROICanvas.RegionType = GraphicsRegionType.MarkingReject;
            else
                MarkROICanvas.RegionType = GraphicsRegionType.Inspection;

            ToolTypeChangeEventHandler eventRunner = ToolTypeChangeEvent;
            if (eventRunner != null)
            {
                eventRunner(newTool);
            }
        }
        #endregion

        #region Display & Save Image.

        private void DisplayImage(string aszFileName)
        {
            ToolChange(ToolType.Pointer);

            try
            {
                BitmapSource bitmapSource = BitmapImageLoader.LoadCachedBitmapImage(new Uri(aszFileName)) as BitmapSource;
                if (bitmapSource != null)
                {
                    ReferenceImage = bitmapSource;
                    UpdateViewerSource(ReferenceImage);
                }
            }
            catch
            {
                MessageBox.Show(ResourceStringHelper.GetErrorMessage("I001"), "Error");
            }
        }
        #endregion

        #region File open & save
        public string GetTeachingFileName(string strMiddleName, string strExtention)
        {
            return String.Format("{0}-{3}-{1}.{2}", MainWindow.CurrentModel.Name, strMiddleName, strExtention, 21);
        }

        #endregion

        #region Load based image.
        private BitmapSource LoadBasedImage()
        {
            try
            {
                if (MainWindow.CurrentGroup != null && MainWindow.CurrentModel != null)
                {
                    string szBasedImagePath = string.Empty;
                    szBasedImagePath = DirectoryManager.GetMarkImagePath(MainWindow.Setting.General.ModelPath, MainWindow.CurrentGroup.Name, MainWindow.CurrentModel.Name, Surface.CA1);

                    if (File.Exists(szBasedImagePath))
                    {
                        this.txtReferenceLabel.Visibility = Visibility.Visible;

                        // 전체영상을 획득하여 반환한다.
                        return Common.Algo.LoadImage(szBasedImagePath);
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


        public void SeeEntireImage()
        {
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
                return;

            m_fReferenceImageScale = VisionDefinition.GRAB_IMAGE_SCALE;

            BitmapSource bitmapSource = (GrabImage != null) ? GrabImage : ReferenceImage;
            IsGrabView = (GrabImage != null);

            if (bitmapSource == null)
            {
                this.txtReferenceLabel.Visibility = Visibility.Hidden;
                MessageBox.Show("등록된 취득 영상이 없습니다.", "Information");
            }
            else
            {
                UpdateViewerSource(bitmapSource);
            }
        }

        public void SeeReferenceImage()
        {
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null) return;
            string szGroupName = MainWindow.CurrentGroup.Name;
            string szModelName = MainWindow.CurrentModel.Name;
            string szReferenceImagePath = string.Empty;

            szReferenceImagePath = DirectoryManager.GetMarkImagePath(MainWindow.Setting.General.ModelPath, MainWindow.CurrentGroup.Name, MainWindow.CurrentModel.Name, Surface.CA1);

            // result action.
            if (!string.IsNullOrEmpty(szReferenceImagePath) && File.Exists(szReferenceImagePath))
            {
                IsGrabView = false;
                DisplayImage(szReferenceImagePath);
                ThumbnailViewer.SetSourceImage(this);
                this.txtReferenceLabel.Visibility = Visibility.Visible;
            }
            else
            {
                try
                {
                    MessageBox.Show("등록된 기준영상이 없습니다.", "Information");
                }
                catch { }
            }

        }


        public void CloseDialog()
        {
            if (MessageBoxResult.Yes == MessageBox.Show("변경된 설정을 저장하시겠습니까?", "Confirm", MessageBoxButton.YesNo))
            {
                SaveTeachingData();
            }
        }

        // Teaching 데이터 저장.
        public void SaveTeachingData()
        {
            if (MainWindow.CurrentGroup == null || MainWindow.CurrentModel == null)
            {
                MessageBox.Show("모델을 선택해 주시기 바랍니다.", "Information");
                return;
            }
            this.Cursor = Cursors.Wait;

            string szMachineCode = MainWindow.Setting.General.MachineCode;
            string szGroupName = MainWindow.CurrentGroup.Name;
            string szModelName = MainWindow.CurrentModel.Name;

            bool saveResult = true;
            ROIManager.RoiCode = 0;
            ModifyWeekLocation();

            saveResult = ROIManager.SaveMarkStripAlignROI(szMachineCode, szGroupName, szModelName, MarkROICanvas);
            if (saveResult)
            {
                for (int i = 0; i < 3; i++) StripAlign[i] = new Point(0, 0);
                foreach (GraphicsBase g in MarkROICanvas.GraphicsList)
                {
                    if (g.RegionType == GraphicsRegionType.StripAlign)
                    {
                        GraphicsStripAlign gs = g as GraphicsStripAlign;
                        StripAlign[gs.nID] = new Point(gs.cpX, gs.cpY);
                    }
                }
            }
            if (Mark_Info != null)
            {
                // if (MainWindow.Setting.Device.PlcUsed == 0 )
                if (MainWindow.Setting.SubSystem.Laser.UseLaser)
                    saveResult = Mark_Info.SaveAll(MainWindow.CurrentModel.CopyToMarkInfo());
            }
            //saveResult = ROIManager.SaveStripGuideROI(szMachineCode, szGroupName, szModelName, MarkROICanvas);


            this.Cursor = Cursors.Arrow;
            if (!saveResult) // Step 3: 티칭 데이터 결과 리포트
            {
                MessageBox.Show(String.Format("마킹 데이터 저장 실패"));
            }
        }
        public bool ModifyWeekLocation()
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    ConnectFactory.DBConnector().StartTrans();
                    ModelInformation modifiedModel = new ModelInformation();
                        modifiedModel.Marker.Code = RMS.Generic.MarkerInformaion.CreateCodeByCheckbox(MainWindow.CurrentModel.Marker);
                    String strQuery = String.Format("UPDATE model_info SET marker_code = '{0}' WHERE model_code = '{1}'", modifiedModel.Marker.Code, MainWindow.CurrentModel.Code);

                    if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                    {
                        return true;
                    }
                    else // UPDATE model_info fail
                    {
                        ConnectFactory.DBConnector().Rollback();

                        return false;
                    }
                }
                else // DBConnector is null
                {
                    return false;
                }
            }
            catch
            {
                if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().Rollback();
                Debug.WriteLine("Exception occured in ModifyModel(ModelManager.cs");
                return false;
            }
        }
        public void SetCameraResolution()
        {
            this.CamResolutionX = MainWindow.Setting.SubSystem.IS.CamResolutionX[CID.CA];
            this.CamResolutionY = MainWindow.Setting.SubSystem.IS.CamResolutionY[CID.CA];
        }

        public void SetMarkPos()
        {
            txtMarkPosX.Text = MainWindow.CurrentModel.Marker.MarkStartPosX.ToString();
            txtMarkPosY.Text = MainWindow.CurrentModel.Marker.MarkStartPosY.ToString();
            txtUMarkPosX.Text = MainWindow.CurrentModel.Marker.UMarkStartPosX.ToString();
            txtUMarkPosY.Text = MainWindow.CurrentModel.Marker.UMarkStartPosY.ToString();
            txtBoatAngle.Text = MainWindow.CurrentModel.Marker.BoatAngle.ToString();
            txtLaserAngle.Text = MainWindow.CurrentModel.Marker.LaserAngle.ToString();
            //SeeReferenceImage();
            SetByCurrentModel();
            // SeeEntireImage();
        }

        public void SaveFirstMarkPos(double x, double y)
        {
            try
            {
                txtMarkPosX.Text = x.ToString();
                txtMarkPosY.Text = y.ToString();
                MainWindow.CurrentModel.Marker.MarkStartPosX = Convert.ToDouble(txtMarkPosX.Text);
                MainWindow.CurrentModel.Marker.MarkStartPosY = Convert.ToDouble(txtMarkPosY.Text);
                MainWindow.CurrentModel.Marker.UMarkStartPosX = Convert.ToDouble(txtUMarkPosX.Text);
                MainWindow.CurrentModel.Marker.UMarkStartPosY = Convert.ToDouble(txtUMarkPosY.Text);
                MainWindow.CurrentModel.Marker.BoatAngle = Convert.ToDouble(txtBoatAngle.Text);
                MainWindow.CurrentModel.Marker.LaserAngle = Convert.ToDouble(txtLaserAngle.Text);
                PCS.ELF.AVI.ModelManager.UpdateModelMarkPos(new Point(MainWindow.CurrentModel.Marker.MarkStartPosX, MainWindow.CurrentModel.Marker.MarkStartPosY),
                                                            new Point(MainWindow.CurrentModel.Marker.UMarkStartPosX, MainWindow.CurrentModel.Marker.UMarkStartPosY),
                                                            MainWindow.CurrentModel.Marker.BoatAngle,
                                                            MainWindow.CurrentModel.Marker.LaserAngle,
                                                            MainWindow.CurrentModel.Code);
            }
            catch
            {
                MessageBox.Show("잘못된 값을 입력 했습니다.");
            }
        }

        public void PowerParamChange(GraphicsBase graphic)
        {
            if (graphic.RegionType == GraphicsRegionType.MarkingRail ||
                graphic.RegionType == GraphicsRegionType.MarkingUnit ||
                graphic.RegionType == GraphicsRegionType.MarkingReject)
            {
                switch (graphic.MarkInfo.MarkType.MarkType)
                {
                    case eMarkingType.eMarkingRailCircle:
                        CurrParam = (graphic.MarkInfo.MarkInfo as RailCircleProperty).ParaNumber;
                        break;
                    case eMarkingType.eMarkingRailRect:
                        CurrParam = (graphic.MarkInfo.MarkInfo as RailRectProperty).ParaNumber;
                        break;
                    case eMarkingType.eMarkingRailSpecial:
                        CurrParam = (graphic.MarkInfo.MarkInfo as RailSpecialProperty).ParaNumber;
                        break;
                    case eMarkingType.eMarkingRailTri:
                        CurrParam = (graphic.MarkInfo.MarkInfo as RailTriProperty).ParaNumber;
                        break;
                    case eMarkingType.eMarkingUnitCircle:
                        CurrParam = (graphic.MarkInfo.MarkInfo as UnitCircleProperty).ParaNumber;
                        break;
                    case eMarkingType.eMarkingUnitRect:
                        CurrParam = (graphic.MarkInfo.MarkInfo as UnitRectProperty).ParaNumber;
                        break;
                    case eMarkingType.eMarkingUnitSpecial:
                        CurrParam = (graphic.MarkInfo.MarkInfo as UnitSpecialProperty).ParaNumber;
                        break;
                    case eMarkingType.eMarkingUnitTri:
                        CurrParam = (graphic.MarkInfo.MarkInfo as UnitTriProperty).ParaNumber;
                        break;
                    case eMarkingType.eMarkingNumber:
                        CurrParam = (graphic.MarkInfo.MarkInfo as NumberProperty).ParaNumber;
                        break;
                    case eMarkingType.eMarkingWeek:
                        CurrParam = (graphic.MarkInfo.MarkInfo as WeekProperty).ParaNumber;
                        break;
                    case eMarkingType.eMarkingIDMark:
                        CurrParam = (graphic.MarkInfo.MarkInfo as IDMarkProperty).ParaNumber;
                        break;
                    case eMarkingType.eMarkingTBD:
                        CurrParam = (graphic.MarkInfo.MarkInfo as TBDProperty).ParaNumber;
                        break;
                }
                PenParam pen = this.Mark_Info.pen[CurrParam];
                txtDrawStep.Text = pen.DrawStep.ToString();
                txtJumpDelay.Text = pen.JumpDelay.ToString();
                txtJumpStep.Text = pen.JumpStep.ToString();
                txtLineDelay.Text = pen.LineDelay.ToString();
                txtStepPeriod.Text = pen.StepPeriod.ToString();
                txtOnDelay.Text = pen.LaserOnDelay.ToString();
                txtOffDelay.Text = pen.LaserOffDelay.ToString();
                txtCornerDelay.Text = pen.CornerDelay.ToString();
                txtFrequency.Text = pen.Frequency.ToString();
                txtCurrent.Text = pen.LampCurrent.ToString();
                txtDuty.Text = pen.PulseDuty.ToString();
            }
        }

        public void PowerParamChange(int nParam)
        {
            if (this.MarkROICanvas.SelectedGraphic != null &&
                (this.MarkROICanvas.SelectedGraphic.RegionType == GraphicsRegionType.MarkingRail ||
                this.MarkROICanvas.SelectedGraphic.RegionType == GraphicsRegionType.MarkingUnit ||
                this.MarkROICanvas.SelectedGraphic.RegionType == GraphicsRegionType.MarkingReject))
            {
                CurrParam = nParam;
                PenParam pen = this.Mark_Info.pen[CurrParam];
                txtDrawStep.Text = pen.DrawStep.ToString();
                txtJumpDelay.Text = pen.JumpDelay.ToString();
                txtJumpStep.Text = pen.JumpStep.ToString();
                txtLineDelay.Text = pen.LineDelay.ToString();
                txtStepPeriod.Text = pen.StepPeriod.ToString();
                txtOnDelay.Text = pen.LaserOnDelay.ToString();
                txtOffDelay.Text = pen.LaserOffDelay.ToString();
                txtCornerDelay.Text = pen.CornerDelay.ToString();
                txtFrequency.Text = pen.Frequency.ToString();
                txtCurrent.Text = pen.LampCurrent.ToString();
                txtDuty.Text = pen.PulseDuty.ToString();
            }
        }
    }
}
