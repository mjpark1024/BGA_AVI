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
 * @file  Teachingviewer.xaml.cs
 * @brief 
 *  Interaction logic for Teachingviewer.xaml.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.08.23
 * @version : 1.1
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.12 First creation.
 * - 2011.08.23 Multi layer handling.
 */

using Common;
using Common.Control;
using Common.Drawing;
using Common.Drawing.InspectionInformation;
using HDSInspector.SubWindow;
using OpenCvSharp.Flann;
using PCS;
using PCS.ModelTeaching;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

// Note.
// Offline mode 대응 완료.

namespace HDSInspector
{
    public partial class TeachingViewer : UserControl
    {
        public TeachingWindow m_ptrTeachingWindow;
        public TeachingViewerCtrl SelectedViewer { get; set; }

        public List<TeachingViewerCtrl> CamView = new List<TeachingViewerCtrl>();
        public List<TabItem> tabCam = new List<TabItem>();
        public bool bVia = false;

        // public int CamID = 0;
        #region ctor & Initializer.
        public TeachingViewer()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }

        private void InitializeDialog()
        {
            bVia = false;
            int BP_Count = 0; int CA_Count = 0; int BA_Count = 0;
            for (int i = 0; i < MainWindow.m_nTotalTeachingViewCount; i++)
            {
                TeachingViewerCtrl viewtemp = new TeachingViewerCtrl();
                CamView.Add(viewtemp);
                this.SelectedViewer = CamView[0];
                CamView[i].ID = i;

                TabItem tabtemp = new TabItem();
                tabCam.Add(tabtemp);

                MainWindow.IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(i);
                string tabName = "본드패드1"; int nSurface = 11; string sSurface = "BP1";
                if (IndexInfo.CategorySurface == CategorySurface.BP)
                {
                    nSurface = (int)Surface.BP1 + BP_Count;
                    sSurface = CategorySurface.BP.ToString() + (IndexInfo.Index + 1);
                    tabName = "본드패드" + (IndexInfo.Index + 1);
                }
                else if (IndexInfo.CategorySurface == CategorySurface.CA)
                {
                    nSurface = (int)Surface.CA1 + CA_Count;
                    sSurface = CategorySurface.CA.ToString() + (IndexInfo.Index + 1);
                    tabName = CategorySurface.CA.ToString() + " 외관" + (IndexInfo.Index + 1);
                }
                else
                {
                    nSurface = (int)Surface.BA1 + BA_Count;
                    sSurface = CategorySurface.BA.ToString() + (IndexInfo.Index + 1);
                    tabName = CategorySurface.BA.ToString() + " 외관" + (IndexInfo.Index + 1);
                }
                CamView[i].nSurface = nSurface;
                CamView[i].sSurface = sSurface;
                tabCam[i].Header = new TextBlock()
                {
                    Width = 58,
                    Height = 22,
                    TextAlignment = TextAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 13,
                    FontWeight = FontWeights.Bold,
                    Cursor = Cursors.Hand,
                    Text = tabName
                };
                Style style = this.FindResource("BlendTabStyle") as Style;
                tabCam[i].Style = style;
                tabCam[i].Content = CamView[i];
            }

            //CamBP1View = new TeachingViewerCtrl();
            //CamBP2View = new TeachingViewerCtrl();
            //CamCA1View = new TeachingViewerCtrl();
            //CamCA2View = new TeachingViewerCtrl();
            //CamBA1View = new TeachingViewerCtrl();
            //CamBA2View = new TeachingViewerCtrl();
            //
            //this.SelectedViewer = CamBP1View;
            //CamBP1View.ID = 0;
            //CamBP2View.ID = 1;
            //CamCA1View.ID = 2;
            //CamCA2View.ID = 3;
            //CamBA1View.ID = 4;
            //CamBA2View.ID = 5;


            //tabCamBP1 = new TabItem();
            //tabCamBP1.Header = new TextBlock()
            //{
            //    Width = 90,
            //    Height = 22,
            //    TextAlignment = TextAlignment.Center,
            //    Foreground = new SolidColorBrush(Colors.White),
            //    FontSize = 13,
            //    FontWeight = FontWeights.Bold,
            //    Cursor = Cursors.Hand,
            //    Text = "본드패드1"
            //};
            //Style style = this.FindResource("BlendTabStyle") as Style;
            //tabCamBP1.Style = style;
            //tabCamBP1.Content = CamBP1View;
            //
            //tabCamBP2 = new TabItem();
            //tabCamBP2.Header = new TextBlock()
            //{
            //    Width = 90,
            //    Height = 22,
            //    TextAlignment = TextAlignment.Center,
            //    Foreground = new SolidColorBrush(Colors.White),
            //    FontSize = 13,
            //    FontWeight = FontWeights.Bold,
            //    Cursor = Cursors.Hand,
            //    Text = "본드패드2"
            //};
            //tabCamBP2.Style = style;
            //tabCamBP2.Content = CamBP2View;
            //
            //tabCamCA1 = new TabItem();
            //tabCamCA1.Header = new TextBlock()
            //{
            //    Width = 90,
            //    Height = 22,
            //    TextAlignment = TextAlignment.Center,
            //    Foreground = new SolidColorBrush(Colors.White),
            //    FontSize = 13,
            //    FontWeight = FontWeights.Bold,
            //    Cursor = Cursors.Hand,
            //    Text = "CA 외관1"
            //};
            //tabCamCA1.Style = style;
            //tabCamCA1.Content = CamCA1View;
            //
            //tabCamCA2 = new TabItem();
            //tabCamCA2.Header = new TextBlock()
            //{
            //    Width = 90,
            //    Height = 22,
            //    TextAlignment = TextAlignment.Center,
            //    Foreground = new SolidColorBrush(Colors.White),
            //    FontSize = 13,
            //    FontWeight = FontWeights.Bold,
            //    Cursor = Cursors.Hand,
            //    Text = "CA 외관2"
            //};
            //tabCamCA2.Style = style;
            //tabCamCA2.Content = CamCA2View;
            //
            //tabCamBA1 = new TabItem();
            //tabCamBA1.Header = new TextBlock()
            //{
            //    Width = 90,
            //    Height = 22,
            //    TextAlignment = TextAlignment.Center,
            //    Foreground = new SolidColorBrush(Colors.White),
            //    FontSize = 13,
            //    FontWeight = FontWeights.Bold,
            //    Cursor = Cursors.Hand,
            //    Text = "BA 외관1"
            //};
            //tabCamBA1.Style = style;
            //tabCamBA1.Content = CamBA1View;
            //
            //tabCamBA2 = new TabItem();
            //tabCamBA2.Header = new TextBlock()
            //{
            //    Width = 90,
            //    Height = 22,
            //    TextAlignment = TextAlignment.Center,
            //    Foreground = new SolidColorBrush(Colors.White),
            //    FontSize = 13,
            //    FontWeight = FontWeights.Bold,
            //    Cursor = Cursors.Hand,
            //    Text = "BA 외관2"
            //};
            //tabCamBA2.Style = style;
            //tabCamBA2.Content = CamBA2View;
            //tabTeaching.SelectedIndex = 0;
            //SelectedViewer = CamBP1View;
            // CamID = 0;
        }

        private void InitializeEvent()
        {
            this.tabTeaching.SelectionChanged += tabTeaching_SelectionChanged;
            DrawingCanvas.ContextMenuChangeEvent += DrawingCanvas_ContextMenuChangeEvent;
            DrawingCanvas.SelectedGraphicChangeEvent += DrawingCanvas_SelectedGraphicChangeEvent;
            this.PreviewMouseLeftButtonUp += TeachingViewer_PreviewMouseLeftButtonUp;
            this.PreviewMouseRightButtonUp += TeachingViewer_PreviewMouseRightButtonUp;
        }

        void TeachingViewer_PreviewMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (bVia) bVia = false;
        }

        void TeachingViewer_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (bVia)
            {
                Point p = e.GetPosition(this.SelectedViewer.BasedROICanvas);
                SelectedViewer.TeachingCanvas.PasteViaGraphics(p);
            }
        }
        #endregion ctor & Initializer.

        public void SetTabControl(int MCType, bool CASlave, bool BASlave)
        {
            int Index = 0;
            if (MainWindow.Setting.SubSystem.IS.FGType[0] >= 6)
            {
                for(int i = 0; i < MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count; i ++) CamView[Index++].RGB = true;
            }
            else
            {
                for (int i = 0; i < MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count; i++) CamView[Index++].RGB = false;
            }
            if (MainWindow.Setting.SubSystem.IS.FGType[1] >= 6)////////SettingINI CA Index 1
            {
                for (int i = 0 ; i < MainWindow.Setting.Job.CACount; i++) CamView[Index++].RGB = true;
            }
            else
            {
                for (int i = 0; i < MainWindow.Setting.Job.CACount; i++) CamView[Index++].RGB = false;
            }
            if (MainWindow.Setting.SubSystem.IS.FGType[3] >= 6)////////SettingINI BA Index 1
            {
                for (int i = 0; i < MainWindow.Setting.Job.BACount; i++) CamView[Index++].RGB = true;
            }
            else
            {
                for (int i = 0; i < MainWindow.Setting.Job.BACount; i++) CamView[Index++].RGB = false;
            }
            tabTeaching.Items.Clear();
            
            for (int i = 0; i < MainWindow.m_nTotalTeachingViewCount; i++)
            {
                MainWindow.IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(i);
                if (IndexInfo.CategorySurface == CategorySurface.CA)
                {
                    if (IndexInfo.Slave && !CASlave) continue;
                }
                else if (IndexInfo.CategorySurface == CategorySurface.BA)
                {
                    if (IndexInfo.Slave && !BASlave) continue;
                }
                tabTeaching.Items.Add(tabCam[i]);
            }
        }

        public void SetViewSize(double w, double h)
        {
            for (int i = 0; i < MainWindow.m_nTotalTeachingViewCount; i++)
            {
                CamView[i].ViewerWidth = w;
                CamView[i].ViewerHeight = h;
            }
        }

        private void DrawingCanvas_ContextMenuChangeEvent(ContextMenuCommand selectedContextMenuCommand)
        {
            switch (selectedContextMenuCommand)
            {
                // 기준 영역 지정
                case ContextMenuCommand.SetFiducialRegion:
                    #region SetFiducialRegion.
                    GraphicsRectangle newFiducialRegion = SelectedViewer.BasedROICanvas.SelectedGraphic as GraphicsRectangle;
                    if (newFiducialRegion != null)
                    {
                        if (newFiducialRegion.IsInspection)
                        {
                            foreach (SectionInformation section in SelectedViewer.SectionManager.Sections)
                            {
                                foreach (SectionRegion region in section.SectionRegionList)
                                {
                                    if (region.RegionIndex.X == newFiducialRegion.IterationXPosition && region.RegionIndex.Y == newFiducialRegion.IterationYPosition)
                                    {
                                        section.RelatedROI = newFiducialRegion;
                                        return;
                                    }
                                }
                            }
                        }
                        else newFiducialRegion.Caption = CaptionHelper.GetRegionCaption(newFiducialRegion);

                    }
                    break;
                #endregion SetFiducialRegion.

                // Local Align 추가
                case ContextMenuCommand.LocalAlign:
                    SelectedViewer.SelectedSection.ROICanvas[(int)SelectedViewer.SelectedChannel].AddLocalAlign();
                    break;

                // 섹션 영역 재탐색
                case ContextMenuCommand.RetrySearchRegion:
                    SelectedViewer.RetrySearchROIPattern();
                    break;

                case ContextMenuCommand.RawSetFWD:
                case ContextMenuCommand.RawSetRWD:
                    if (SelectedViewer.SelectedSection.Type.Code != SectionTypeCode.RAW_REGION) return;
                    List<double> fTop = new List<double>();
                    int n = 0;
                    foreach (GraphicsBase g in SelectedViewer.SelectedSection.ROICanvas[(int)SelectedViewer.SelectedChannel].GraphicsList)
                    {
                        if (g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine ||
                            g.RegionType == GraphicsRegionType.TapeLoaction || g.RegionType == GraphicsRegionType.UnitAlign)
                        {
                            continue;
                        }

                        // 다른 검사들은 배열 하지 않는다
                        if(!other_inspection_check(g))
                            fTop.Add(g.boundaryRect.Top);
                    }
                    if (fTop.Count > 0)
                    {
                        double[] ftTop = new double[fTop.Count];
                        fTop.CopyTo(ftTop);
                        Array.Sort(ftTop);
                        n = 0;
                        if (ContextMenuCommand.RawSetFWD == selectedContextMenuCommand)
                        {
                            foreach (GraphicsBase g in SelectedViewer.SelectedSection.ROICanvas[(int)SelectedViewer.SelectedChannel].GraphicsList)
                            {
                                if (g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine ||
                                    g.RegionType == GraphicsRegionType.TapeLoaction || g.RegionType == GraphicsRegionType.UnitAlign)
                                {
                                    continue;
                                }

                                if (!other_inspection_check(g))
                                {
                                    g.RawRow = Array.IndexOf(ftTop, fTop[n]) + 1;// fTop.IndexOf(ftTop[n]) + 1;
                                    for (int i = 0; i < g.InspectionList.Count; i++)
                                    {
                                        if (g.InspectionList[i].InspectionType.InspectType == eVisInspectType.eInspTypeUnitRawMaterial)
                                        {
                                            RawMetrialProperty item = (RawMetrialProperty)g.InspectionList[i].InspectionAlgorithm;
                                            item.MinSmallDefectSize = g.RawRow - 1;
                                        }
                                    }
                                    n++;
                                }
                            }
                        }
                        else
                        {
                            Array.Reverse(ftTop);
                            foreach (GraphicsBase g in SelectedViewer.SelectedSection.ROICanvas[(int)SelectedViewer.SelectedChannel].GraphicsList)
                            {
                                if (g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine ||
                                    g.RegionType == GraphicsRegionType.TapeLoaction || g.RegionType == GraphicsRegionType.UnitAlign)
                                {
                                    continue;
                                }


                                if (!other_inspection_check(g))
                                {
                                    g.RawRow = Array.IndexOf(ftTop, fTop[n]) + 1;
                                    for (int i = 0; i < g.InspectionList.Count; i++)
                                    {
                                        if (g.InspectionList[i].InspectionType.InspectType == eVisInspectType.eInspTypeUnitRawMaterial)
                                        {
                                            RawMetrialProperty item = (RawMetrialProperty)g.InspectionList[i].InspectionAlgorithm;
                                            item.MinSmallDefectSize = g.RawRow - 1;
                                        }
                                    }
                                    n++;
                                }
                            }
                        }
                    }
                    break;

                case ContextMenuCommand.Templete:
                    m_ptrTeachingWindow.InspectionTypectrl.cmbInspectionType.SelectedIndex = -1;
                    m_ptrTeachingWindow.InspectionTypectrl.pnlInspectionSettings.Children.Clear();
                    TemplateWindow temWindow = new TemplateWindow(m_ptrTeachingWindow, SelectedViewer.TeachingCanvas.SelectedGraphic, SelectedViewer.ID);
                    try
                    {
                        if ((bool)temWindow.ShowDialog())
                        {
                        }
                    }
                    catch (Exception e) { MessageBox.Show(e.Message); };
                    break;

                case ContextMenuCommand.CopyRailROI:
                    try
                    {
                        SectionInformation selectedSection = SelectedViewer.SelectedSection;
                        if (selectedSection != null && SelectedViewer.SectionManager.Sections != null)
                        {
                            string szSelectedSectionType = selectedSection.Type.Name;
                            foreach (SectionInformation section in SelectedViewer.SectionManager.Sections)
                            {
                                if (selectedSection == section)
                                    continue;
                                else if (szSelectedSectionType != section.Type.Name)
                                    continue;

                                section.ROICanvas[(int)SelectedViewer.SelectedChannel].GraphicsList.Clear();
                            }
                            foreach (SectionInformation section in SelectedViewer.SectionManager.Sections)
                            {
                                if (selectedSection == section)
                                    continue;
                                else if (szSelectedSectionType != section.Type.Name)
                                    continue;
                                //int nAlignCnt = 0;
                                foreach (GraphicsBase g in selectedSection.ROICanvas[(int)SelectedViewer.SelectedChannel].Selection)
                                {
                                    if (g.RegionType == GraphicsRegionType.LocalAlign || g.RegionType == GraphicsRegionType.GuideLine || g.RegionType == GraphicsRegionType.TapeLoaction)
                                        continue;
                                    if (g is GraphicsSelectionRectangle || g is GraphicsLine)
                                        continue;

                                    if (GraphicsRegionType.Inspection == g.RegionType)
                                    {
                                        GraphicsRectangle graphic = new GraphicsRectangle(100,
                                                                                  100,
                                                                                  section.GetBitmapSource((int)SelectedViewer.SelectedChannel).PixelWidth - 100,
                                                                                  section.GetBitmapSource((int)SelectedViewer.SelectedChannel).PixelHeight - 100,
                                                                                  section.ROICanvas[(int)SelectedViewer.SelectedChannel].LineWidth,
                                                                                  GraphicsRegionType.Inspection,
                                                                                  GraphicsColors.YellowGreen,
                                                                                  section.ROICanvas[(int)SelectedViewer.SelectedChannel].ActualScale);
                                        if (section.ROICanvas[(int)SelectedViewer.SelectedChannel].CanDraw(graphic))
                                        {
                                            graphic.Caption = CaptionHelper.GetRegionCaption(graphic);
                                            for (int i = 0; i < g.InspectionList.Count; i++)
                                            {
                                                InspectionType it = g.InspectionList[i].InspectionType;
                                                InspectionAlgorithm ia = g.InspectionList[i].InspectionAlgorithm.Clone();
                                                graphic.InspectionList.Add(new InspectionItem(it, ia, i));
                                            }
                                            section.ROICanvas[(int)SelectedViewer.SelectedChannel].GraphicsList.Add(graphic);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch { }

                    break;
                case ContextMenuCommand.SetVia:
                    {
                        DrawingCanvas teachingCanvas = SelectedViewer.TeachingCanvas;
                        if (!teachingCanvas.IsBasedCanvas)
                        {
                            if (teachingCanvas.SelectionCount > 0)
                            {
                                teachingCanvas.CopyGraphics();
                                bVia = true;
                            }
                        }
                    }
                    break;

                case ContextMenuCommand.RawmetrialSetting://원소재 찾기
                    try
                    {
                        if (SelectedViewer.SelectedSection.BitmapSource != null)
                        {
                            //Algo m_Algo = new Algo();
                            //int cnt = m_Algo.SearchRawmeterial(SelectedViewer.SelectedSection.BitmapSource[(int)SelectedViewer.SelectedChannel], ref p, ref s);
                            List<Point> points = new List<Point>();
                            List<int> sizes = new List<int>();

                            New_Algo m_algo = new New_Algo();
                            BitmapSource bmp = null;
                            if (SelectedViewer.SelectedSection.BitmapSource[(int)SelectedViewer.SelectedChannel] == null)
                            {
                                if(SelectedViewer.SelectedSection.TempBitmapSource[(int)SelectedViewer.SelectedChannel] !=null)
                                  bmp = SelectedViewer.SelectedSection.TempBitmapSource[(int)SelectedViewer.SelectedChannel];
                            }
                            else
                            {
                                bmp = SelectedViewer.SelectedSection.BitmapSource[(int)SelectedViewer.SelectedChannel];
                            }

                            int cnt = m_algo.SearchRawmeterial(bmp, ref points, ref sizes);

                            RawSet(ref sizes, ref points);
                            //s네 대해서 많이 나오는 값을 찾자
                            //그리고 그값을 기준으로 해당되는 원만 그리자.
                            cnt = Math.Min(MainWindow.CurrentModel.Strip.UnitRow, cnt);

                            List<GraphicsBase> Copy_GraphicsList = new List<GraphicsBase>();

                            //SelectedViewer.TeachingCanvas.GraphicsList.CopyTo(Copy_GraphicsList, 0);
                        

                            for (int g_index = 0; g_index < SelectedViewer.TeachingCanvas.GraphicsList.Count; g_index++)
                            {
                                GraphicsBase graphics = (GraphicsBase)SelectedViewer.TeachingCanvas.GraphicsList[g_index];

                               for (int Insp_index = 0; Insp_index < graphics.InspectionList.Count; Insp_index++)
                               {
                                    if (graphics.InspectionList[Insp_index].InspectionType.InspectType == eVisInspectType.eInspTypeUnitRawMaterial)
                                    {
                                        graphics.InspectionList.Remove(graphics.InspectionList[Insp_index]);
                                        Insp_index--;
                                    }
                               }

                               if(graphics.InspectionList.Count != 0)
                                 Copy_GraphicsList.Add(graphics);
                            }

                            SelectedViewer.TeachingCanvas.GraphicsList.Clear();

                            for (int i = 0; i < Copy_GraphicsList.Count; i++)
                            {
                                Copy_GraphicsList[i].UnitRow = 1;
                                SelectedViewer.TeachingCanvas.GraphicsList.Add(Copy_GraphicsList[i]);
                            }


                            for (int i = 0; i < cnt; i++)
                            {
                                GraphicsEllipse graphic = new GraphicsEllipse(points[i].X - sizes[i] - 3, points[i].Y - sizes[i] - 3, points[i].X + sizes[i] + 7, points[i].Y + sizes[i] + 7,
                                                                                  SelectedViewer.TeachingCanvas.LineWidth,
                                                                                  GraphicsRegionType.Inspection,
                                                                                  GraphicsColors.Red,
                                                                                  SelectedViewer.TeachingCanvas.ActualScale);

                                {
                                    graphic.RefreshDrawing();
                                    if (MainWindow.CurrentModel.Marker.RailMark != 2)
                                        graphic.UnitRow = i;
                                    else graphic.UnitRow = cnt - i - 1;

                                    List<InspectionType> inspectionTypes = new List<InspectionType>();
                                    inspectionTypes = InspectionType.GetInspectionTypeList();

                                    InspectionType Rawmeterial_InspectionType = new InspectionType();
                                    foreach (InspectionType item in inspectionTypes)
                                    {
                                        if (item.InspectType == eVisInspectType.eInspTypeUnitRawMaterial)
                                        {
                                            Rawmeterial_InspectionType = item;
                                            break;
                                        }
                                    }

                                    SelectedViewer.TeachingCanvas.GraphicsList.Add(graphic);
                                    SelectedViewer.TeachingCanvas.SelectedGraphic = (GraphicsBase)SelectedViewer.TeachingCanvas.GraphicsList[SelectedViewer.TeachingCanvas.GraphicsList.Count - 1];
                                    m_ptrTeachingWindow.InspectionTypectrl.AddInspectionItem(Rawmeterial_InspectionType);
                                }
                            }
                            SelectedViewer.TeachingCanvas.Refresh();
                        }
                        else
                        {
                            MessageBox.Show("등록된 기준 영상이 없습니다.");
                        }
                    }
                    catch 
                    {
                        MessageBox.Show("등록된 영상이 없습니다.");
                    }

                    break;

                // ROI 90도 회전
                case ContextMenuCommand.RotateROI90:
                    Point ptRotate90 = new Point
                    {
                        X = SelectedViewer.SelectedSection.GetBitmapSource((int)SelectedViewer.SelectedChannel).PixelWidth / 2,
                        Y = SelectedViewer.SelectedSection.GetBitmapSource((int)SelectedViewer.SelectedChannel).PixelHeight / 2
                    };
                    SelectedViewer.SelectedSection.ROICanvas[(int)SelectedViewer.SelectedChannel].RotateGraphic(ptRotate90, 90.0);
                    break;

                // ROI 180도 회전
                case ContextMenuCommand.RotateROI180:
                    Point ptRotate180 = new Point
                    {
                        X = SelectedViewer.SelectedSection.GetBitmapSource((int)SelectedViewer.SelectedChannel).PixelWidth / 2,
                        Y = SelectedViewer.SelectedSection.GetBitmapSource((int)SelectedViewer.SelectedChannel).PixelHeight / 2
                    };
                    SelectedViewer.SelectedSection.ROICanvas[(int)SelectedViewer.SelectedChannel].RotateGraphic(ptRotate180, 180.0);
                    break;

                // Y축 Flip
                case ContextMenuCommand.SymmetryROIUpDown:
                    Point ptYFlip = new Point
                    {
                        X = SelectedViewer.SelectedSection.GetBitmapSource((int)SelectedViewer.SelectedChannel).PixelWidth,
                        Y = SelectedViewer.SelectedSection.GetBitmapSource((int)SelectedViewer.SelectedChannel).PixelHeight
                    };
                    SelectedViewer.SelectedSection.ROICanvas[(int)SelectedViewer.SelectedChannel].SymmetryGraphic(ptYFlip, FilpType.UPDOWN);
                    break;

                // X축 Flip
                case ContextMenuCommand.SymmetryROILeftRight:
                    Point ptXFlip = new Point
                    {
                        X = SelectedViewer.SelectedSection.GetBitmapSource((int)SelectedViewer.SelectedChannel).PixelWidth,
                        Y = SelectedViewer.SelectedSection.GetBitmapSource((int)SelectedViewer.SelectedChannel).PixelHeight
                    };
                    SelectedViewer.SelectedSection.ROICanvas[(int)SelectedViewer.SelectedChannel].SymmetryGraphic(ptXFlip, FilpType.LEFTRIGHT);
                    break;
                // Y축 Flip
                case ContextMenuCommand.CopyROIUpDown:
                    Point ptYFlip2 = new Point
                    {
                        X = SelectedViewer.SelectedSection.GetBitmapSource((int)SelectedViewer.SelectedChannel).PixelWidth,
                        Y = SelectedViewer.SelectedSection.GetBitmapSource((int)SelectedViewer.SelectedChannel).PixelHeight
                    };
                    SelectedViewer.SelectedSection.ROICanvas[(int)SelectedViewer.SelectedChannel].CopytoGraphic(ptYFlip2, FilpType.UPDOWN);
                    break;

                // X축 Flip
                case ContextMenuCommand.CopyROILeftRight:
                    Point ptXFlip2 = new Point
                    {
                        X = SelectedViewer.SelectedSection.GetBitmapSource((int)SelectedViewer.SelectedChannel).PixelWidth,
                        Y = SelectedViewer.SelectedSection.GetBitmapSource((int)SelectedViewer.SelectedChannel).PixelHeight
                    };
                    SelectedViewer.SelectedSection.ROICanvas[(int)SelectedViewer.SelectedChannel].CopytoGraphic(ptXFlip2, FilpType.LEFTRIGHT);
                    break;
            }
        }

        private void RawSet(ref List<int> size, ref List<Point> point)
        {
            if (size.Count < 4)
                return;
            int[] tmpsize = size.ToArray();
            Point[] tmppoint = point.ToArray();
            int sum = 0;
            int min = 1000;
            int max = 0;
            for (int i = 0; i < size.Count; i++)
            {
                sum += tmpsize[i];
                min = Math.Min(min, tmpsize[i]);
                max = Math.Max(max, tmpsize[i]);
            }
            sum = sum - min - max;
            int avg = sum / (size.Count - 2);
            for (int i = size.Count - 1; i >= 0; i--)
            {
                if (Math.Abs(size[i] - avg) > 20)
                {
                    size.RemoveAt(i);
                    point.RemoveAt(i);
                }
            }
        }

        private void DrawingCanvas_SelectedGraphicChangeEvent(GraphicsBase newGraphic)
        {
            if (SelectedViewer == null) return;
            if (newGraphic != null)
            {
                if (newGraphic is GraphicsRectangleBase || newGraphic is GraphicsPolyLine)
                {
                    SelectedViewer.lblWidth.Visibility = Visibility.Visible;
                    SelectedViewer.lblHeight.Visibility = Visibility.Visible;
                    SelectedViewer.lblLength.Visibility = Visibility.Collapsed;
                    SelectedViewer.txtWidth.Visibility = Visibility.Visible;
                    SelectedViewer.txtHeight.Visibility = Visibility.Visible;
                    SelectedViewer.txtLength.Visibility = Visibility.Collapsed;
                    SelectedViewer.txtWidthByResolution.Visibility = Visibility.Visible;
                    SelectedViewer.txtHeightByResolution.Visibility = Visibility.Visible;
                    SelectedViewer.txtLengthByResolution.Visibility = Visibility.Collapsed;
                    SelectedViewer.txtWidth.DataContext = newGraphic;
                    SelectedViewer.txtHeight.DataContext = newGraphic;
                }
                else if (newGraphic is GraphicsLine)
                {
                    SelectedViewer.lblWidth.Visibility = Visibility.Collapsed;
                    SelectedViewer.lblHeight.Visibility = Visibility.Collapsed;
                    SelectedViewer.lblLength.Visibility = Visibility.Visible;
                    SelectedViewer.txtWidth.Visibility = Visibility.Collapsed;
                    SelectedViewer.txtHeight.Visibility = Visibility.Collapsed;
                    SelectedViewer.txtLength.Visibility = Visibility.Visible;
                    SelectedViewer.txtWidthByResolution.Visibility = Visibility.Collapsed;
                    SelectedViewer.txtHeightByResolution.Visibility = Visibility.Collapsed;
                    SelectedViewer.txtLengthByResolution.Visibility = Visibility.Visible;
                    SelectedViewer.txtLength.DataContext = newGraphic;
                }
            }
            else // newGraphic is null
            {
                SelectedViewer.txtLength.DataContext = null;
                SelectedViewer.txtWidth.DataContext = null;
                SelectedViewer.txtHeight.DataContext = null;
            }
        }

        private void tabTeaching_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bVia = false;
            #region Set SelectedViewer Property.
            for(int i =0; i < CamView.Count; i++)
            {
                if (CamView[i].TeachingCanvas != null) CamView[i].TeachingCanvas.SelectedGraphic = null;
                if (tabTeaching.SelectedItem == tabCam[i]) SelectedViewer = CamView[i];
            }
            #endregion

            if (!m_ptrTeachingWindow.IsOnLine)
            {
                m_ptrTeachingWindow.MtsManager.InitVision(MainWindow.Setting.SubSystem.IS, m_ptrTeachingWindow.TeachingViewer.SelectedViewer.ID);
                m_ptrTeachingWindow.TeachingViewer.SelectedViewer.IsGrabDone = false;
            }

            if (SelectedViewer != null)
            {
                if (!m_ptrTeachingWindow.IsOnLine)
                {
                    SelectedViewer.IsSentDone = false;
                    SelectedViewer.IsGrabDone = false;
                }

                if (SelectedViewer.lbSection.SelectedIndex == -1)
                {
                    DrawingCanvas.FixedSectionROI = true;
                    SelectedViewer.chkFixedROI.IsChecked = DrawingCanvas.FixedSectionROI; // 전체 영상에서는 ROI 고정 checkbox가 선택되도록 함.
                }
                else
                    SelectedViewer.chkFixedROI.IsChecked = DrawingCanvas.FixedInspectROI; // ROI 고정 여부 판단.

                SelectedViewer.ToolChange(ToolType.Pointer);

                // Set Hide/Show ROI.
                if (SelectedViewer.chkDisableROI.IsChecked == true)
                    SelectedViewer.HiddenPaint();
                else
                    SelectedViewer.ShowPaint();

                // Set Binarization Mode.
                if (SelectedViewer.chkBinarization.IsChecked == true)
                    SelectedViewer.Binarization();
                else
                    m_ptrTeachingWindow.HistogramCtrl.HideThresholdGuideLine();

                SelectedViewer.TeachingCanvas.UnselectAll();
                SelectedViewer.CalculateZoomToFitScale();
                SelectedViewer.TeachingSubMenuCtrl.ToolTypeChanged(SelectedViewer.Tool);
                SelectedViewer.ThumbnailViewer.SetSourceImage(SelectedViewer);

                m_ptrTeachingWindow.LineProfileCtrl.SetLineProfileSource(SelectedViewer.TeachingImageSource);
                m_ptrTeachingWindow.UpdateLvResult();
                m_ptrTeachingWindow.SetManualInspectState(SelectedViewer.IsSentDone); // Vision 정보 전송이 이루어진 상태에서 유닛 보기 및 수동 검사가 가능하다.

                MainWindow.IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(SelectedViewer.ID);
                MainWindow.LightInfo LightInfo = MainWindow.Convert_LightIndex(IndexInfo.CategorySurface, IndexInfo.Index);////////10개의 DB 버퍼공간을 활용하는 방식으로 변경

                if (m_ptrTeachingWindow.IsOnLine && MainWindow.CurrentModel != null)
                {
                    m_ptrTeachingWindow.lightcontrol.SetValues(MainWindow.CurrentModel.LightValue, LightInfo.LightIndex, LightInfo.ValueIndex, MainWindow.Setting.SubSystem.Light.Channel);
                    
                    m_ptrTeachingWindow.lightcontrol.LightOn(false);
                }

                set_rgbmenu();
            }
        }

        public void set_rgbmenu()
        {
            MainWindow.IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(SelectedViewer.ID);
            int FrameGrabberIndex = VID.CalcIndex(IndexInfo.VisionIndex);
            if (IndexInfo.CategorySurface == CategorySurface.BP)
            {
                m_ptrTeachingWindow.grdSelectRGB.Visibility = Visibility.Hidden;
                SelectedViewer.spRGB.Visibility = Visibility.Hidden;
                m_ptrTeachingWindow.grdCampitch.Visibility = Visibility.Visible;
            }
            else
            {
                if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] >= 6)
                {
                    m_ptrTeachingWindow.grdSelectRGB.Visibility = Visibility.Visible;
                    SelectedViewer.spRGB.Visibility = Visibility.Visible;
                }
                else
                {
                    m_ptrTeachingWindow.grdSelectRGB.Visibility = Visibility.Hidden;
                    SelectedViewer.spRGB.Visibility = Visibility.Hidden;
                }
                m_ptrTeachingWindow.grdCampitch.Visibility = Visibility.Hidden;
            }
        }

        public bool other_inspection_check(GraphicsBase g)
        {
            #region 다른 검사들은 배열 하지 않는다
            bool other_inspection_check = false;
            for (int i = 0; i < g.InspectionList.Count; i++)
            {
                if (g.InspectionList[i].InspectionType.InspectType != eVisInspectType.eInspTypeUnitRawMaterial)
                {
                    other_inspection_check = true;
                    break;
                }
            }
            return other_inspection_check;

            #endregion
        }
    }
}
