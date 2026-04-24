using System;
using System.Windows.Controls;
using System.Windows.Input;
using Common.Drawing;
using Common.Drawing.InspectionTypeUI;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.Diagnostics;
using Common.Drawing.InspectionInformation;
using Common.Drawing.MarkingInformation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PCS.ModelTeaching;
using System.Windows;
using System.Linq;
using Common.Drawing.MarkingTypeUI;
using Common;

namespace HDSInspector
{
    /// <summary>
    /// MarkingTypeCtrl.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public delegate void SaveChangeEventHandler(GraphicsBase graphic, eMarkingType marktype);
    public delegate void MarkMoveEventHandler();
    public delegate void MarkResizeEventHandler();


    public partial class MarkingTypeCtrl : UserControl
    {
        public LaserWindow m_ptrLaserWindow;
        public MarkViewerCtrl m_ptrViewer;
        public GraphicsBase SelectedGraphic { get; set; }

        public event SaveChangeEventHandler SaveChanged;
        public event MarkMoveEventHandler MarkMoved;
        //public event MarkResizeEventHandler MarkChanged;
        #region Ctor
        public MarkingTypeCtrl()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }
        private void InitializeDialog()
        {

        }

        private void InitializeEvent()
        {
            DrawingCanvas.SelectedGraphicChangeEvent += DrawingCanvas_SelectedGraphicChangeEvent;

            this.btnSave.Click += (s, e) => SaveMarkingItem();
            this.btnSetPreview.Click += (s, e) => SetAsPreview();
            this.btnSetDefault.Click += (s, e) => SetAsDefault();
            this.PreviewKeyUp += (s, e) => { if (e.Key == Key.Enter) { SaveMarkingItem(); } };
        }

        #endregion

        #region EventHandler
        // 선택된 ROI 변경시 호출되는 이벤트.
        private void DrawingCanvas_SelectedGraphicChangeEvent(GraphicsBase newGraphic)
        {
            if (newGraphic != null)
            {
                if (((newGraphic.RegionType == GraphicsRegionType.MarkingUnit ||
                      newGraphic.RegionType == GraphicsRegionType.MarkingRail ||
                      newGraphic.RegionType == GraphicsRegionType.MarkingReject) && !newGraphic.Dummy) ||
                      newGraphic.RegionType == GraphicsRegionType.UnitGuide)
                {
                    ShortenAnimation();
                }
                else if (newGraphic.RegionType == GraphicsRegionType.MarkingRail && newGraphic.Dummy)
                {
                    if (MainWindow.CurrentModel.Marker.RailIrr)
                    {
                        ShortenAnimation();
                    }
                    else
                    {
                        if (newGraphic.Step == 0)
                        {
                            ShortenAnimation();
                        }
                        else
                        {
                            SetDisableStateAddInspectionItem();

                            this.SelectedGraphic = null;
                            return;
                        }
                    }
                }
                else
                {
                    SetDisableStateAddInspectionItem();

                    this.SelectedGraphic = null;
                    return;
                }
                this.SelectedGraphic = newGraphic;
                SelectionChanged();
            }
            else // newGraphic is null
            {
                SetDisableStateAddInspectionItem();
                this.SelectedGraphic = null;
            }
        }

        public void DisplayChange()
        {
            if(SelectedGraphic != null)
                ((IMarkingTypeUICommands)pnlMarkingSettings.Children[0]).Display(SelectedGraphic);
        }
        public void SelectionChanged()
        {
            try
            {
                SetEnableStateAddInspectionItem();
                MarkItem selectedItem = SelectedGraphic.MarkInfo;
                if (selectedItem != null)
                {
                    string markingName = selectedItem.MarkType.Name;
                    // 마킹 설정 아이템과 마킹 항목을 조회하여 알맞은 UI컨트롤을 찾아 검사 설정 값을 Display 시킨다.
                    foreach (MarkingType markElement in MarkingType.GetMarkTypeList())
                    {
                        if (markingName == markElement.Name)
                        {

                            selectedItem.MarkType.SettingControl = MarkingType.GetMarkTypeSettingDialog(selectedItem.MarkType.MarkType);
                            ((IMarkingTypeUICommands)selectedItem.MarkType.SettingControl).SetDialog(MainWindow.Setting.SubSystem.IS.CamResolutionX[2], selectedItem.MarkType.MarkType);

                            pnlMarkingSettings.Children.Clear();
                            pnlMarkingSettings.Children.Add(markElement.SettingControl);
                            bool bRadioButton = false;
                            if(selectedItem.MarkType.MarkType == eMarkingType.eMarkingWeek || selectedItem.MarkType.MarkType == eMarkingType.eMarkingNumber /*|| selectedItem.MarkType.MarkType == eMarkingType.eMarkingTBD || selectedItem.MarkType.MarkType == eMarkingType.eMarkingIDMark*/) bRadioButton = true;
                             ((IMarkingTypeUICommands)markElement.SettingControl).Display(SelectedGraphic, bRadioButton);
                            ((IMarkingTypeUICommands)markElement.SettingControl).MoveClick += MarkingTypeCtrl_MoveClick;
                            ((IMarkingTypeUICommands)markElement.SettingControl).MarkSizeChanged += MarkingTypeCtrl_SizeChanged;
                            ((IMarkingTypeUICommands)markElement.SettingControl).ParamChange += MarkingTypeCtrl_ParamChange;
                            ((IMarkingTypeUICommands)markElement.SettingControl).SaveChange += MarkingTypeCtrl_SaveChange;
                            ((IMarkingTypeUICommands)markElement.SettingControl).LocationChanged += LocationChange;
                            if (SelectedGraphic != null && m_ptrViewer != null) m_ptrViewer.PowerParamChange(SelectedGraphic);
                            LengthenAnimation();
                            break;
                        }
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Exception occured in lbInspection_SelectionChanged(InspectionTypeCtrl.xaml.cs)");
            }
        }

        void MarkingTypeCtrl_SaveChange()
        {
            if (SelectedGraphic == null) return;
            if (SelectedGraphic.RegionType == GraphicsRegionType.MarkingUnit)
            {
                switch (SelectedGraphic.MarkInfo.MarkType.MarkType)
                {
                    case eMarkingType.eMarkingUnitCircle:
                        UnitCircleProperty uc = SelectedGraphic.MarkInfo.MarkInfo as UnitCircleProperty;
                        m_ptrViewer.Mark_Info.UnitMark.template.Sems.lstHPGL[SelectedGraphic.MarkOBJID].FileName = uc.PLTFile;
                        m_ptrViewer.Mark_Info.UnitMark.template.Sems.lstHPGL[SelectedGraphic.MarkOBJID].ParaNumber = uc.ParaNumber;
                        break;
                    case eMarkingType.eMarkingUnitRect:
                        UnitRectProperty ur = SelectedGraphic.MarkInfo.MarkInfo as UnitRectProperty;
                        m_ptrViewer.Mark_Info.UnitMark.template.Sems.lstHPGL[SelectedGraphic.MarkOBJID].FileName = ur.PLTFile;
                        m_ptrViewer.Mark_Info.UnitMark.template.Sems.lstHPGL[SelectedGraphic.MarkOBJID].ParaNumber = ur.ParaNumber;
                        break;
                    case eMarkingType.eMarkingUnitTri:
                        UnitTriProperty ut = SelectedGraphic.MarkInfo.MarkInfo as UnitTriProperty;
                        m_ptrViewer.Mark_Info.UnitMark.template.Sems.lstHPGL[SelectedGraphic.MarkOBJID].FileName = ut.PLTFile;
                        m_ptrViewer.Mark_Info.UnitMark.template.Sems.lstHPGL[SelectedGraphic.MarkOBJID].ParaNumber = ut.ParaNumber;
                        break;
                    case eMarkingType.eMarkingUnitSpecial:
                        UnitSpecialProperty us = SelectedGraphic.MarkInfo.MarkInfo as UnitSpecialProperty;
                        m_ptrViewer.Mark_Info.UnitMark.template.Sems.lstHPGL[SelectedGraphic.MarkOBJID].FileName = us.PLTFile;
                        m_ptrViewer.Mark_Info.UnitMark.template.Sems.lstHPGL[SelectedGraphic.MarkOBJID].ParaNumber = us.ParaNumber;
                        break;
                }
            }
            else if (SelectedGraphic.RegionType == GraphicsRegionType.MarkingRail)
            {
                switch (SelectedGraphic.MarkInfo.MarkType.MarkType)
                {
                    case eMarkingType.eMarkingRailCircle:
                        RailCircleProperty uc = SelectedGraphic.MarkInfo.MarkInfo as RailCircleProperty;
                        m_ptrViewer.Mark_Info.RailMark.template.Sems.lstHPGL[SelectedGraphic.MarkOBJID].FileName = uc.PLTFile;
                        m_ptrViewer.Mark_Info.RailMark.template.Sems.lstHPGL[SelectedGraphic.MarkOBJID].ParaNumber = uc.ParaNumber;
                        break;
                    case eMarkingType.eMarkingRailRect:
                        RailRectProperty ur = SelectedGraphic.MarkInfo.MarkInfo as RailRectProperty;
                        m_ptrViewer.Mark_Info.RailMark.template.Sems.lstHPGL[SelectedGraphic.MarkOBJID].FileName = ur.PLTFile;
                        m_ptrViewer.Mark_Info.RailMark.template.Sems.lstHPGL[SelectedGraphic.MarkOBJID].ParaNumber = ur.ParaNumber;
                        break;
                    case eMarkingType.eMarkingRailTri:
                        RailTriProperty ut = SelectedGraphic.MarkInfo.MarkInfo as RailTriProperty;
                        m_ptrViewer.Mark_Info.RailMark.template.Sems.lstHPGL[SelectedGraphic.MarkOBJID].FileName = ut.PLTFile;
                        m_ptrViewer.Mark_Info.RailMark.template.Sems.lstHPGL[SelectedGraphic.MarkOBJID].ParaNumber = ut.ParaNumber;
                        break;
                    case eMarkingType.eMarkingRailSpecial:
                        RailSpecialProperty us = SelectedGraphic.MarkInfo.MarkInfo as RailSpecialProperty;
                        m_ptrViewer.Mark_Info.RailMark.template.Sems.lstHPGL[SelectedGraphic.MarkOBJID].FileName = us.PLTFile;
                        m_ptrViewer.Mark_Info.RailMark.template.Sems.lstHPGL[SelectedGraphic.MarkOBJID].ParaNumber = us.ParaNumber;
                        break;
                }
            }
            else if (SelectedGraphic.RegionType == GraphicsRegionType.MarkingReject)
            {
                if (SelectedGraphic.MarkInfo.MarkType.MarkType == eMarkingType.eMarkingNumber)
                {
                    NumberProperty n = SelectedGraphic.MarkInfo.MarkInfo as NumberProperty;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].FontName = n.FNTFile;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].ParaNumber = n.ParaNumber;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].CapitalHeight = n.CapitalHeight;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].CharGap = n.CharGap;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].Width = n.CharWidth;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].Height = n.CharHeight;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].SpaceSize = n.SpaceSize;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].ReferenceNumber1 = n.RefNumber;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].SpecialType = n.SpecialType;
                }
                else if (SelectedGraphic.MarkInfo.MarkType.MarkType == eMarkingType.eMarkingWeek)
                {
                    WeekProperty w = SelectedGraphic.MarkInfo.MarkInfo as WeekProperty;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].FontName = w.FNTFile;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].ParaNumber = w.ParaNumber;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].CapitalHeight = w.CapitalHeight;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].CharGap = w.CharGap;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].Width = w.CharWidth;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].Height = w.CharHeight;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].SpaceSize = w.SpaceSize;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].ReferenceNumber1 = w.RefNumber;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].SpecialType = w.SpecialType;
                }
                else if (SelectedGraphic.MarkInfo.MarkType.MarkType == eMarkingType.eMarkingIDMark)
                {
                    IDMarkProperty w = SelectedGraphic.MarkInfo.MarkInfo as IDMarkProperty;
                    //m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].FontName = w.FNTFile;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].ParaNumber = w.ParaNumber;
                    //m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].CapitalHeight = w.CapitalHeight;
                    //m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].CharGap = w.CharGap;
                    //m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].Width = w.CharWidth;
                    //m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].Height = w.CharHeight;
                    //m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].SpaceSize = w.SpaceSize;
                    //m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].ReferenceNumber1 = w.RefNumber;
                    //m_ptrViewer.Mark_Info.RejMark.template.Sems.lstText[SelectedGraphic.MarkOBJID].SpecialType = w.SpecialType;
                }
                else if (SelectedGraphic.MarkInfo.MarkType.MarkType == eMarkingType.eMarkingTBD)
                {
                    TBDProperty w = SelectedGraphic.MarkInfo.MarkInfo as TBDProperty;
                    m_ptrViewer.Mark_Info.RejMark.template.Sems.lstIDMark[SelectedGraphic.MarkOBJID].ParaNumber = w.ParaNumber;
                }
                SelectedGraphic.RefreshDrawing();
            }
        }

        void MarkingTypeCtrl_ParamChange(int nParam)
        {
            if (SelectedGraphic != null)
            {
                m_ptrViewer.PowerParamChange(nParam);
            }
        }



        void MarkingTypeCtrl_SizeChanged(double X, double Y, double Width, double Height, double Angle, eMarkingType type)
        {
            if (SelectedGraphic == null) return;
            double resX = MainWindow.Setting.SubSystem.IS.CamResolutionX[2] * 4.0;
            double resY = MainWindow.Setting.SubSystem.IS.CamResolutionY[2] * 4.0;
            System.Windows.Point p;
            System.Windows.Point p1 = new System.Windows.Point(0, 0);
            Strip strip;
            if (type == eMarkingType.eMarkingRailCircle)
            {
                strip = m_ptrViewer.Mark_Info.RailMark.template.Strips[0];
                p = new System.Windows.Point(m_ptrViewer.MarkROICanvas.StripGuidePoint.X, m_ptrViewer.MarkROICanvas.StripGuidePoint.Y);
                GraphicsEllipseMark g = SelectedGraphic as GraphicsEllipseMark;
                if (g.Dummy)
                {
                    double ox = 0.0;
                    double ax = 0.0;
                    double tmph = g.HeightProperty;
                    double step = g.Step * MainWindow.CurrentModel.Strip.StepPitch;
                    g.Top = p.Y + (strip.FirstX + g.UnitColumn * MainWindow.CurrentModel.Strip.UnitWidth + step + X) * 1000.0 / resY;
                    g.Right = p.X + (60.0 - strip.FirstY - Y) * 1000.0 / resX;
                    if (m_ptrViewer.StripAlign[0].Y > 0 && m_ptrViewer.StripAlign[1].Y > 0)
                    {
                        ox = (m_ptrViewer.StripAlign[1].Y - m_ptrViewer.StripAlign[0].Y) / (m_ptrViewer.StripAlign[1].X - m_ptrViewer.StripAlign[0].X);
                        ax = ((X + g.UnitColumn * MainWindow.CurrentModel.Strip.UnitWidth + step) * 1000 / resY) / ox;
                    }
                    g.Right += ax;
                    g.Left = g.Right - tmph;
                    g.Bottom = g.Top + tmph;
                    g.CircleInfo.Radian = g.WidthProperty / 2.0;
                    g.CircleInfo.Position.X = g.Left + g.CircleInfo.Radian;
                    g.CircleInfo.Position.Y = g.Top + g.CircleInfo.Radian;
                }
                else
                {
                    double tmpw = Width * 1000.0 / resX;
                    double tmph = g.HeightProperty;
                    g.Top = (X) * 1000.0 / resY + p.Y;
                    g.Bottom = g.Top + tmpw;
                    g.Right = p.X + (60.0 - Y) * 1000.0 / resX;
                    g.Left = g.Right - tmpw;
                    g.CircleInfo.Radian = g.WidthProperty / 2.0;
                    g.CircleInfo.Position.X = g.Left + g.CircleInfo.Radian;
                    g.CircleInfo.Position.Y = g.Top + g.CircleInfo.Radian;
                    RailCircleProperty rc = g.MarkInfo.MarkInfo as RailCircleProperty;
                    rc.Rotate = Angle;
                    rc.Width = rc.Height = Width;
                }
            }
            else if (type == eMarkingType.eMarkingUnitCircle)
            {
                p = new System.Windows.Point(m_ptrViewer.MarkROICanvas.UnitGuidePoint.X, m_ptrViewer.MarkROICanvas.UnitGuidePoint.Y);
                GraphicsEllipseMark g = SelectedGraphic as GraphicsEllipseMark;
                g.Top = X * 1000.0 / resY + p.Y;
                g.Left = p.X - Y * 1000.0 / resX;
                g.Bottom = (X + Width) * 1000.0 / resY + p.Y;
                g.Right = p.X - (Y + Height) * 1000.0 / resX;
                g.CircleInfo.Position.X = g.Left + g.CircleInfo.Radian;
                g.CircleInfo.Position.Y = g.Top + g.CircleInfo.Radian;
            }
            else if (type == eMarkingType.eUnitGuide)
            {
                p = new System.Windows.Point(m_ptrViewer.MarkROICanvas.StripGuidePoint.X, m_ptrViewer.MarkROICanvas.StripGuidePoint.Y);

                GraphicsRectangleBase g = SelectedGraphic as GraphicsRectangleBase;
                double tmpw = g.WidthProperty;
                double tmph = g.HeightProperty;
                g.Top = (X) * 1000.0 / resY + p.Y;
                g.Bottom = g.Top + tmph;
                g.Right = p.X + (60.0 - Y) * 1000.0 / resX;
                g.Left = g.Right - tmpw;

                m_ptrViewer.MarkROICanvas.UnitGuidePoint = new System.Windows.Point(g.Right, g.Top);
            }
            else if (type == eMarkingType.eMarkingNumber)
            {
                int cp = MainWindow.CurrentModel.Marker.NumLeft ? 0 : 1;
                p = new System.Windows.Point(m_ptrViewer.MarkROICanvas.StripRejectGuidePoint[cp].X, m_ptrViewer.MarkROICanvas.StripRejectGuidePoint[cp].Y);
                GraphicsNumberMark g = SelectedGraphic as GraphicsNumberMark;
                if (Angle == 0 || Angle == 180)
                {
                    g.Top = (X + m_ptrViewer.Mark_Info.RejMark.template.Strips[0].FirstX) * 1000.0 / resY + p.Y;
                    g.Left = p.X + (60.0 - (Y + Height) - m_ptrViewer.Mark_Info.RejMark.template.Strips[0].FirstY) * 1000.0 / resX;
                    g.Bottom = g.Top + (Width * 1000.0 / resY);
                    g.Right = g.Left + (Height * 1000.0 / resX);
                    g.Rotate = Angle;
                }
                else
                {
                    g.Top = (X + m_ptrViewer.Mark_Info.RejMark.template.Strips[0].FirstX) * 1000.0 / resY + p.Y;
                    g.Left = p.X + (60.0 - (Y + Width) - m_ptrViewer.Mark_Info.RejMark.template.Strips[0].FirstY) * 1000.0 / resX;
                    g.Bottom = g.Top + (Height * 1000.0 / resY);
                    g.Right = g.Left + (Width * 1000.0 / resX);
                    g.Rotate = Angle;
                }
                NumberProperty oldNum = g.MarkInfo.MarkInfo as NumberProperty;
                double scale = oldNum.CapitalHeight / oldNum.CharHeight;
                oldNum.CapitalHeight = Height;
                if (oldNum.FNTFile.Contains("sbga3"))
                {

                }
                else
                {
                    oldNum.CharWidth = Width;
                }
                double c = Height / scale * 10000.0;
                oldNum.CharHeight = Math.Round(c) / 10000.0;
            }
            else if (type == eMarkingType.eMarkingIDMark)
            {
                int cp = MainWindow.CurrentModel.Marker.IDMark;
                p = new System.Windows.Point(m_ptrViewer.MarkROICanvas.StripRejectGuidePoint[cp].X, m_ptrViewer.MarkROICanvas.StripRejectGuidePoint[cp].Y);
                GraphicsIDMark g = SelectedGraphic as GraphicsIDMark;
                if (Angle == 0 || Angle == 180)
                {
                    g.Top = (X + m_ptrViewer.Mark_Info.RejMark.template.Strips[0].FirstX) * 1000.0 / resY + p.Y;
                    g.Left = p.X + (60.0 - (Y + Height) - m_ptrViewer.Mark_Info.RejMark.template.Strips[0].FirstY) * 1000.0 / resX;
                    g.Bottom = g.Top + (Width * 1000.0 / resY);
                    g.Right = g.Left + (Height * 1000.0 / resX);
                    g.Rotate = Angle;
                }
                else
                {
                    g.Top = (X + m_ptrViewer.Mark_Info.RejMark.template.Strips[0].FirstX) * 1000.0 / resY + p.Y;
                    g.Left = p.X + (60.0 - (Y + Width) - m_ptrViewer.Mark_Info.RejMark.template.Strips[0].FirstY) * 1000.0 / resX;
                    g.Bottom = g.Top + (Height * 1000.0 / resY);
                    g.Right = g.Left + (Width * 1000.0 / resX);
                    g.Rotate = Angle;
                }
            }
            else if (type == eMarkingType.eMarkingTBD)
            {
                int cp = MainWindow.CurrentModel.Marker.IDMark;
                p = new System.Windows.Point(m_ptrViewer.MarkROICanvas.StripRejectGuidePoint[cp].X, m_ptrViewer.MarkROICanvas.StripRejectGuidePoint[cp].Y);
                GraphicsTBD g = SelectedGraphic as GraphicsTBD;
                g.Top = (X + m_ptrViewer.Mark_Info.RejMark.template.Strips[0].FirstX) * 1000.0 / resY + p.Y;
                g.Left = p.X + (60.0 - (Y + Width) - m_ptrViewer.Mark_Info.RejMark.template.Strips[0].FirstY) * 1000.0 / resX;
                g.Bottom = g.Top + (Height * 1000.0 / resY);
                g.Right = g.Left + (Width * 1000.0 / resX);
                g.Rotate = Angle;
            }
            else if (type == eMarkingType.eMarkingWeek)
            {
                int wp = MainWindow.CurrentModel.Marker.WeekPos;
                p = new System.Windows.Point(m_ptrViewer.MarkROICanvas.StripRejectGuidePoint[wp].X, m_ptrViewer.MarkROICanvas.StripRejectGuidePoint[wp].Y);
                GraphicsWeekMark g = SelectedGraphic as GraphicsWeekMark;
                if (Angle == 0 || Angle == 180)
                {
                    g.Top = (X + m_ptrViewer.Mark_Info.RejMark.template.Strips[0].FirstX) * 1000.0 / resY + p.Y;
                    g.Left = p.X + (60.0 - (Y + Height) - m_ptrViewer.Mark_Info.RejMark.template.Strips[0].FirstY) * 1000.0 / resX;
                    g.Bottom = g.Top + (Width * 1000.0 / resY);
                    g.Right = g.Left + (Height * 1000.0 / resX);
                    g.Rotate = Angle;
                }
                else
                {
                    g.Top = (X + m_ptrViewer.Mark_Info.RejMark.template.Strips[0].FirstX) * 1000.0 / resY + p.Y;
                    g.Left = p.X + (60.0 - (Y + Width) - m_ptrViewer.Mark_Info.RejMark.template.Strips[0].FirstY) * 1000.0 / resX;
                    g.Bottom = g.Top + (Height * 1000.0 / resY);
                    g.Right = g.Left + (Width * 1000.0 / resX);
                    g.Rotate = Angle;
                }

                WeekProperty oldNum = g.MarkInfo.MarkInfo as WeekProperty;
                double scale = oldNum.CapitalHeight / oldNum.CharHeight;
                oldNum.CapitalHeight = Height;
                double c = Height / scale * 10000.0;
                oldNum.CharHeight = Math.Round(c) / 10000.0;
            }
            else if (type == eMarkingType.eMarkingUnitTri)
            {
                p = new System.Windows.Point(m_ptrViewer.MarkROICanvas.UnitGuidePoint.X, m_ptrViewer.MarkROICanvas.UnitGuidePoint.Y);
                GraphicsTriangleMark g = SelectedGraphic as GraphicsTriangleMark;
                g.Top = X * 1000.0 / resY + p.Y;
                g.Left = p.X - Y * 1000.0 / resX;
                g.Bottom = (X + Width) * 1000.0 / resY + p.Y;
                g.Right = p.X - (Y + Height) * 1000.0 / resX;
                g.Rotate = (int)Angle;
            }
            else
            {
                if (type > eMarkingType.eMarkingUnitCircle)
                {
                    p = new System.Windows.Point(m_ptrViewer.MarkROICanvas.UnitGuidePoint.X, m_ptrViewer.MarkROICanvas.UnitGuidePoint.Y);
                    GraphicsRectangleBase g = SelectedGraphic as GraphicsRectangleBase;
                    g.Top = X * 1000.0 / resY + p.Y;
                    g.Left = p.X - Y * 1000.0 / resX;
                    g.Bottom = (X + Width) * 1000.0 / resY + p.Y;
                    g.Right = p.X - (Y + Height) * 1000.0 / resX;
                }
                else
                {
                    // strip = m_ptrViewer.Mark_Info.RailMark.template.Strips;
                    p = new System.Windows.Point(m_ptrViewer.MarkROICanvas.StripGuidePoint.X, m_ptrViewer.MarkROICanvas.StripGuidePoint.Y);
                    GraphicsRectangleBase g = SelectedGraphic as GraphicsRectangleBase;
                    g.Top = (X) * 1000.0 / resY + p.Y;
                    g.Left = p.X + (60.0 - (Y + Height)) * 1000.0 / resX;
                    g.Bottom = (X + Width) * 1000.0 / resY + p.Y;
                    g.Right = p.X + (60.0 - Y) * 1000.0 / resX;
                }
            }
            //m_ptrViewer.SaveTeachingData();

            SelectedGraphic.RefreshDrawing();
            MarkMoveEventHandler er = MarkMoved;
            m_ptrViewer.MarkGraphicChange(SelectedGraphic);
            if (er != null)
                er();

        }

        void MarkingTypeCtrl_MoveClick(int id, double pitch, eMarkingType type)
        {
            // left, right, up, down
            if (SelectedGraphic == null) return;
            double pixelmove = pitch * 1000.0 / MainWindow.Setting.SubSystem.IS.CamResolutionX[2] * 4.0;
            switch (id)
            {
                case 1:
                    if (type == eMarkingType.eMarkingRailCircle || type == eMarkingType.eMarkingUnitCircle)
                    {
                        GraphicsEllipseMark g = SelectedGraphic as GraphicsEllipseMark;
                        g.Left += pixelmove;
                        g.Right += pixelmove;
                        g.CircleInfo.Position.X = g.Left + g.CircleInfo.Radian;
                        g.CircleInfo.Position.Y = g.Top + g.CircleInfo.Radian;
                    }
                    else
                    {
                        GraphicsRectangleBase g = SelectedGraphic as GraphicsRectangleBase;
                        g.Left += pixelmove;
                        g.Right += pixelmove;
                    }
                    break;
                case 0:
                    if (type == eMarkingType.eMarkingRailCircle || type == eMarkingType.eMarkingUnitCircle)
                    {
                        GraphicsEllipseMark g = SelectedGraphic as GraphicsEllipseMark;
                        g.Left -= pixelmove;
                        g.Right -= pixelmove;
                        g.CircleInfo.Position.X = g.Left + g.CircleInfo.Radian;
                        g.CircleInfo.Position.Y = g.Top + g.CircleInfo.Radian;
                    }
                    else
                    {
                        GraphicsRectangleBase g = SelectedGraphic as GraphicsRectangleBase;
                        g.Left -= pixelmove;
                        g.Right -= pixelmove;
                    }
                    break;
                case 3:
                    if (type == eMarkingType.eMarkingRailCircle || type == eMarkingType.eMarkingUnitCircle)
                    {
                        GraphicsEllipseMark g = SelectedGraphic as GraphicsEllipseMark;
                        g.Top += pixelmove;
                        g.Bottom += pixelmove;
                        g.CircleInfo.Position.X = g.Left + g.CircleInfo.Radian;
                        g.CircleInfo.Position.Y = g.Top + g.CircleInfo.Radian;
                    }
                    else
                    {
                        GraphicsRectangleBase g = SelectedGraphic as GraphicsRectangleBase;
                        g.Top += pixelmove;
                        g.Bottom += pixelmove;
                    }
                    break;
                case 2:
                    if (type == eMarkingType.eMarkingRailCircle || type == eMarkingType.eMarkingUnitCircle)
                    {
                        GraphicsEllipseMark g = SelectedGraphic as GraphicsEllipseMark;
                        g.Top -= pixelmove;
                        g.Bottom -= pixelmove;
                        g.CircleInfo.Position.X = g.Left + g.CircleInfo.Radian;
                        g.CircleInfo.Position.Y = g.Top + g.CircleInfo.Radian;
                    }
                    else
                    {
                        GraphicsRectangleBase g = SelectedGraphic as GraphicsRectangleBase;
                        g.Top -= pixelmove;
                        g.Bottom -= pixelmove;
                    }
                    break;
            }
            SelectedGraphic.RefreshDrawing();
            MarkMoveEventHandler er = MarkMoved;
            m_ptrViewer.MarkGraphicChange(SelectedGraphic);
            if (er != null)
                er();
        }
        #endregion

        #region ItemControl

        // 검사 설정 저장.
        public void SaveMarkingItem()
        {
            if (pnlMarkingSettings.Children.Count == 1 && SelectedGraphic != null)
            {
                IMarkingTypeUICommands markSetting = (IMarkingTypeUICommands)pnlMarkingSettings.Children[0];
                MarkItem selectedItem = SelectedGraphic.MarkInfo;
                if (selectedItem != null)
                {
                    markSetting.TrySave(SelectedGraphic);
                    SaveChangeEventHandler er = SaveChanged;
                    if (er != null) er(SelectedGraphic, selectedItem.MarkType.MarkType);
                }

            }
        }

        // 검사 설정 이전 값 복원.
        private void SetAsPreview()
        {
            if (this.pnlMarkingSettings.Children.Count == 1) // always 1
            {
                IMarkingTypeUICommands markSetting = (IMarkingTypeUICommands)this.pnlMarkingSettings.Children[0];
                markSetting.SetPreviewValue();
            }
        }

        // 검사 설정 초기화.
        private void SetAsDefault()
        {
            if (this.pnlMarkingSettings.Children.Count == 1) // always 1
            {
                IMarkingTypeUICommands markSetting = (IMarkingTypeUICommands)this.pnlMarkingSettings.Children[0];
                markSetting.SetDefaultValue();
            }
        }
        #endregion

        #region Add marking item setting panel.
        private void SetEnableStateAddInspectionItem()
        {
            if (this.pnlMarkingSettings.Children.Count == 1) // always 1
            {
                IMarkingTypeUICommands markSetting = (IMarkingTypeUICommands)this.pnlMarkingSettings.Children[0];
                markSetting.LocationEnabled(true);
            }
            this.btnSave.IsEnabled = true;
            this.btnSetPreview.IsEnabled = true;
            this.btnSetDefault.IsEnabled = true;
            this.btnDelete.IsEnabled = true;
            this.pnlMarkingSettings.Children.Clear();
        }

        private void SetDisableStateAddInspectionItem()
        {
            if (this.pnlMarkingSettings.Children.Count == 1) // always 1
            {
                IMarkingTypeUICommands markSetting = (IMarkingTypeUICommands)this.pnlMarkingSettings.Children[0];
                markSetting.LocationEnabled(false);
            }
            this.btnSave.IsEnabled = false;
            this.btnSetPreview.IsEnabled = false;
            this.btnSetDefault.IsEnabled = false;
            this.btnDelete.IsEnabled = false;
            this.pnlMarkingSettings.Children.Clear();
        }
        #endregion

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
        void LocationChange(int Location)
        {
            MarkItem selectedItem = SelectedGraphic.MarkInfo;
            if (selectedItem.MarkType.MarkType == eMarkingType.eMarkingWeek)
                MainWindow.CurrentModel.Marker.WeekPos = Location;
            if (selectedItem.MarkType.MarkType == eMarkingType.eMarkingNumber)
                MainWindow.CurrentModel.Marker.NumLeft = Location == 0 ? true : false;
            //if (selectedItem.MarkType.MarkType == eMarkingType.eMarkingIDMark)///사용안함
            //    MainWindow.CurrentModel.Marker.IDMark = Location;               
            //if (selectedItem.MarkType.MarkType == eMarkingType.eMarkingTBD)
            //    MainWindow.CurrentModel.Marker.IDMark = Location;
        }
    }
}

