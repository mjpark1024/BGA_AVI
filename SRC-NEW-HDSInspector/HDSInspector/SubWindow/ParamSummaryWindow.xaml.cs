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
 * @file  ParamSummaryWindow.xaml.cs
 * @brief 
 *  검사 설정 Data를 화면에 Display한다.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2012.08.03
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2012.08.03 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using PCS.ModelTeaching;
using Common.Drawing;
using Common.Drawing.InspectionInformation;
using Common;

namespace HDSInspector.SubWindow
{
    public partial class ParamSummaryWindow : Window
    {
        private ObjectDataProvider ObjectDataProvider
        {
            get { return this.TryFindResource("InspItemData") as ObjectDataProvider; }
        }

        public ParamSummaryWindow(List<List<SectionInformation>> Sections)
        {
            InitializeComponent();

            if (ObjectDataProvider != null)
                ObjectDataProvider.ObjectInstance = InspItemStorage.Instance;
            this.DataContext = ObjectDataProvider;

            ShowParamSummary(Sections);
            InitializeEvent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        private void InitializeEvent()
        {
            radAll.Click += chkInspType_Click;
            radUnitAlign.Click += chkInspType_Click;
            radSurface.Click += chkInspType_Click;
            radSpace.Click += chkInspType_Click;
            radPlate.Click += chkInspType_Click;
            radTape.Click += chkInspType_Click;
            radDownset.Click += chkInspType_Click;
            radLeadShape.Click += chkInspType_Click;
            radLeadGap.Click += chkInspType_Click;
            radSpaceShape.Click += chkInspType_Click;

            this.KeyDown += (s, e) => { if (e.Key == Key.Escape) this.Close(); };
        }

        private void ShowParamSummary(List<List<SectionInformation>> Sections)
        {
            InspItemStorage.Instance.GenerateData(Sections);
            txtSectionCount.Text = string.Format("{0} 개", InspItemStorage.Instance.SectionCount.ToString());
            txtROICount.Text = string.Format("{0} 개", InspItemStorage.Instance.ROICount.ToString());
            txtInspCount.Text = string.Format("{0} 개", InspItemStorage.Instance.InspCount.ToString());
        }

        List<DataGridColumn> m_listAll = new List<DataGridColumn>();
        List<DataGridColumn> m_listAlign = new List<DataGridColumn>();

        private void chkInspType_Click(object sender, RoutedEventArgs e)
        {
            UpdateColumnVisibility();
        }

        // 검사 항목
        private enum InspType
        {
            All = 0,
            UnitAlign = 1,
            Surface = 2,
            Space = 3,
            Plate = 4,
            Tape = 5,
            Downset = 6,
            LeadShape = 7,
            LeadGap = 8,
            SpaceShape = 9
        }

        private void UpdateColumnVisibility()
        {
            if (radAll.IsChecked == true)
            {
                foreach (DataGridColumn column in paramGrid.Columns)
                    column.Visibility = Visibility.Visible;
                return;
            }

            #region Common 프로퍼티
            cThreshType.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cLowerThresh.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked || (bool)radDownset.IsChecked || (bool)radLeadShape.IsChecked || (bool)radLeadGap.IsChecked || (bool)radSpaceShape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cUpperThresh.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked || (bool)radDownset.IsChecked || (bool)radLeadShape.IsChecked || (bool)radLeadGap.IsChecked || (bool)radSpaceShape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;

            cMaskLowerThresh.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cMaskUpperThresh.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked) ? Visibility.Visible : Visibility.Collapsed;

            cApplyAverDiff.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked || (bool)radDownset.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cAverMinMargin.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked || (bool)radDownset.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cAverMaxMargin.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked || (bool)radDownset.IsChecked) ? Visibility.Visible : Visibility.Collapsed;

            cInspRange.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cMinMargin.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked || (bool)radDownset.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cMaxMargin.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked || (bool)radDownset.IsChecked) ? Visibility.Visible : Visibility.Collapsed;

            cErosionTrainIter.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked || (bool)radDownset.IsChecked || (bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cDilationTrainIter.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked || (bool)radDownset.IsChecked || (bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;

            cErosionInspIter.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked || (bool)radDownset.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cDilationInspIter.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked || (bool)radDownset.IsChecked) ? Visibility.Visible : Visibility.Collapsed;

            cMinDefectSize.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked || (bool)radDownset.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cMaxDefectSize.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked) ? Visibility.Visible : Visibility.Collapsed;

            cMinSmallDefectSize.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cMaxSmallDefectSize.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked) ? Visibility.Visible : Visibility.Collapsed;

            cMinSmallDefectCount.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cMaxSmallDefectCount.Visibility = ((bool)radSurface.IsChecked || (bool)radSpace.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            #endregion Common 프로퍼티

            #region 도금, Tape Column Visibility.
            //// 도금, Tape 전용 프로퍼티
            //cGroundLowerThresh.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cGroundUpperThresh.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cShapeLowerThresh.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cShapeUpperThresh.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cErosionShapeIter.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cDilationShapeIter.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cShapeShiftMarginX.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cShapeShiftMarginY.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cShapeOffsetX.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cShapeOffsetY.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;

            //// 도금, Tape의 형상 영역 전용 프로퍼티
            //cMasterUse.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cMasterThreshType.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cMasterLowerThresh.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cMasterUpperThresh.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cMasterApplyAverDiff.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cMasterAverMinMargin.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cMasterAverMaxMargin.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cMasterInspRange.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cMasterMinMargin.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cMasterMaxMargin.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cMasterMinDefectSize.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cMasterMaxDefectSize.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;

            //// 도금, Tape의 표면 영역 전용 프로퍼티
            //cSlaveUse.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cSlaveThreshType.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cSlaveLowerThresh.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cSlaveUpperThresh.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cSlaveApplyAverDiff.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cSlaveAverMinMargin.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cSlaveAverMaxMargin.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cSlaveInspRange.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cSlaveMinMargin.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cSlaveMaxMargin.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cSlaveMinDefectSize.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //cSlaveMaxDefectSize.Visibility = ((bool)radPlate.IsChecked || (bool)radTape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            #endregion 도금, Tape 전용 프로퍼티

            #region 리드 형상, 간격 전용 프로퍼티
            cLeadMinNormalRatio.Visibility = ((bool)radLeadShape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cLeadMaxNormalRatio.Visibility = ((bool)radLeadShape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cLeadMinHeightSize.Visibility = ((bool)radLeadShape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cLeadMinWidthRatio.Visibility = ((bool)radLeadShape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cLeadMaxWidthRatio.Visibility = ((bool)radLeadShape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cLeadMinWidthSize.Visibility = ((bool)radLeadShape.IsChecked || (bool)radLeadGap.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cLeadMaxWidthSize.Visibility = ((bool)radLeadShape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            #endregion

            #region 공간 형상 검사 전용 프로퍼티
            cShapeMinNormalRatio.Visibility = ((bool)radSpaceShape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cShapeMaxNormalRatio.Visibility = ((bool)radSpaceShape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cShapeMinHeightSize.Visibility = ((bool)radSpaceShape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cShapeMinWidthRatio.Visibility = ((bool)radSpaceShape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cShapeMaxWidthRatio.Visibility = ((bool)radSpaceShape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cShapeMinWidthSize.Visibility = ((bool)radSpaceShape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cShapeMaxWidthSize.Visibility = ((bool)radSpaceShape.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            #endregion

            #region Unit Align 전용 프로퍼티
            cAlignAcceptance.Visibility = ((bool)radUnitAlign.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cAlignX.Visibility = ((bool)radUnitAlign.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            cAlignY.Visibility = ((bool)radUnitAlign.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            #endregion
        }
    }

    //public class InspItemStorage : INotifyPropertyChanged
    //{
    //    #region InspItem Collections
    //    public ObservableCollection<InspItem> InspItemList
    //    {
    //        get { return inspItemList; }
    //        set
    //        {
    //            inspItemList = value;
    //            NotifyPropertyChanged("InspItemList");
    //        }
    //    }
    //    private ObservableCollection<InspItem> inspItemList;
    //    #endregion

    //    #region Filter Collections
    //    public ObservableCollection<IdString> RoiCodeList { get; set; }
    //    public ObservableCollection<IdString> WorkTypeList { get; set; }
    //    public ObservableCollection<IdString> SectionNameList { get; set; }
    //    public ObservableCollection<IdString> InspNameList { get; set; }
    //    public ObservableCollection<IdString> ThreshTypeList { get; set; }
    //    public ObservableCollection<IdString> InspRangeList { get; set; }
    //    #endregion

    //    #region Singleton.
    //    static InspItemStorage()
    //    {
    //        _instance = null;
    //    }

    //    private static InspItemStorage _instance;
    //    public static InspItemStorage Instance
    //    {
    //        get
    //        {
    //            if (_instance == null)
    //                _instance = new InspItemStorage();
    //            return _instance;
    //        }
    //    }
    //    #endregion

    //    public int SectionCount;
    //    public int ROICount;
    //    public int InspCount;

    //    private InspItemStorage()
    //    {
    //        CreateFilterString();
    //        inspItemList = new ObservableCollection<InspItem>();
    //    }

    //    #region Data Handling.
    //    private void CreateFilterString()
    //    {
    //        RoiCodeList = new ObservableCollection<IdString>();

    //        WorkTypeList = new ObservableCollection<IdString>();
    //        WorkTypeList.Add(new IdString(0, "BP1"));
    //        WorkTypeList.Add(new IdString(1, "BP2"));
    //        WorkTypeList.Add(new IdString(2, "CA1"));
    //        WorkTypeList.Add(new IdString(3, "CA2"));
    //        WorkTypeList.Add(new IdString(4, "BA1"));
    //        WorkTypeList.Add(new IdString(5, "BA2"));

    //        SectionNameList = new ObservableCollection<IdString>();

    //        InspNameList = new ObservableCollection<IdString>();
    //        InspNameList.Add(new IdString(0, "Unit Align"));
    //        InspNameList.Add(new IdString(1, "표면 검사"));
    //        InspNameList.Add(new IdString(2, "Core Crack 검사"));
    //        InspNameList.Add(new IdString(3, "미도금 검사"));
    //        InspNameList.Add(new IdString(4, "PSR 검사"));
    //        InspNameList.Add(new IdString(5, "Downset 검사"));
    //        InspNameList.Add(new IdString(6, "리드 형상 검사"));
    //        InspNameList.Add(new IdString(7, "공간 형상 검사"));
    //        InspNameList.Add(new IdString(8, "Ball 검사"));
    //        InspNameList.Add(new IdString(9, "인식키 검사"));
    //        InspNameList.Add(new IdString(10, "Window Punch 검사"));
    //        InspNameList.Add(new IdString(11, "Vent Hole 검사"));
    //        InspNameList.Add(new IdString(12, "Burr 검사"));
    //        InspNameList.Add(new IdString(13, "Via 검사"));
    //        InspNameList.Add(new IdString(14, "Dam Size 검사"));
    //        InspNameList.Add(new IdString(15, "유닛인식키 검사"));
    //        InspNameList.Add(new IdString(16, "외곽 검사"));

    //        ThreshTypeList = new ObservableCollection<IdString>();
    //        ThreshTypeList.Add(new IdString(0, "임계 이상"));
    //        ThreshTypeList.Add(new IdString(1, "임계 이하"));
    //        ThreshTypeList.Add(new IdString(2, "임계 범위"));

    //        InspRangeList = new ObservableCollection<IdString>();
    //        InspRangeList.Add(new IdString(0, "-"));
    //        InspRangeList.Add(new IdString(1, "Min"));
    //        InspRangeList.Add(new IdString(2, "Max"));
    //        InspRangeList.Add(new IdString(3, "Min/Max"));
    //        InspRangeList.Add(new IdString(4, "In"));
    //        InspRangeList.Add(new IdString(5, "Min/In"));
    //        InspRangeList.Add(new IdString(6, "Max/In"));
    //        InspRangeList.Add(new IdString(7, "Min/Max/In"));
    //    }

    //    public void GenerateData(List<SectionInformation> sec11, List<SectionInformation> sec12, List<SectionInformation> sec21, 
    //        List<SectionInformation> sec22, List<SectionInformation> sec31, List<SectionInformation> sec32)
    //    {
    //        SectionNameList.Clear();
    //        AddSectionNameList(sec11);
    //        AddSectionNameList(sec12);
    //        AddSectionNameList(sec21);
    //        AddSectionNameList(sec22);
    //        AddSectionNameList(sec31);
    //        AddSectionNameList(sec32);

    //        List<InspItem> list = new List<InspItem>();

    //        if (sec11 != null)
    //        {
    //            GenerateInspItems(ref list, WorkTypeList[0], sec11);
    //        }
    //        if (sec12 != null)
    //        {
    //            GenerateInspItems(ref list, WorkTypeList[1], sec12);
    //        }
    //        if (sec21 != null)
    //        {
    //            GenerateInspItems(ref list, WorkTypeList[2], sec21);
    //        }
    //        if (sec22 != null)
    //        {
    //            GenerateInspItems(ref list, WorkTypeList[3], sec22);
    //        }
    //        if (sec31 != null)
    //        {
    //            GenerateInspItems(ref list, WorkTypeList[4], sec31);
    //        }
    //        if (sec32 != null)
    //        {
    //            GenerateInspItems(ref list, WorkTypeList[5], sec32);
    //        }
    //        InspItemList = new ObservableCollection<InspItem>(list);
    //    }

    //    public void AddSectionNameList(List<SectionInformation> sections)
    //    {
    //        if (sections == null) return;

    //        List<string> sectionNames = new List<string>();
    //        foreach (SectionInformation section in sections)
    //            sectionNames.Add(section.Name);

    //        bool bNewName = true;
    //        foreach (string sectionName in sectionNames)
    //        {
    //            foreach (IdString val in SectionNameList)
    //            {
    //                if (val.Name == sectionName)
    //                {
    //                    bNewName = false;
    //                    break;
    //                }
    //            }
    //            if (bNewName)
    //                SectionNameList.Add(new IdString(SectionNameList.Count, sectionName));
    //        }
    //    }

    //    private void GenerateInspItems(ref List<InspItem> inspItemList, IdString workType, List<SectionInformation> sections)
    //    {
    //        if (SectionNameList.Count == 0) // 작업이 필요하지 않은 상태
    //            return;
    //        IdString sectionValue = null;
    //        foreach (SectionInformation section in sections)
    //        {
    //            SectionCount++;
    //            foreach (IdString val in SectionNameList)
    //            {
    //                if (val.Name == section.Name)
    //                {
    //                    sectionValue = val;
    //                    break;
    //                }
    //            }

    //            foreach (GraphicsBase graphic in section.ROICanvas[0].GraphicsList)
    //            {
    //                ROICount++;
    //                if (graphic.InspectionList == null) continue;
    //                InspCount += graphic.InspectionList.Count;

    //                foreach (InspectionItem inspItem in graphic.InspectionList)
    //                {
    //                    InspItem item = new InspItem();
    //                    //item.RoiCode = inspItem.InspectionType.Code;
    //                    item.WorkType = workType;
    //                    if (sectionValue != null)
    //                        item.SectionName = sectionValue;

    //                    switch (inspItem.InspectionType.InspectType)
    //                    {
    //                        case eVisInspectType.eInspTypeUnitAlign: // Unit Align
    //                            FiducialAlignProperty alignProperty = inspItem.InspectionAlgorithm as FiducialAlignProperty;
    //                            if (alignProperty != null)
    //                            {
    //                                item.InspName = InspNameList[0];

    //                                item.AlignAcceptance = alignProperty.AlignAcceptance;
    //                                item.AlignMarginX = alignProperty.AlignMarginX;
    //                                item.AlignMarginY = alignProperty.AlignMarginY;
    //                            }
    //                            else continue;
    //                            break;
    //                        case eVisInspectType.eInspTypeSurface: // 표면 검사
    //                        case eVisInspectType.eInspTypeSpace: // 공간 검사
    //                        case eVisInspectType.eInspTypePSR: // PSR 검사
    //                        case eVisInspectType.eInspTypePunchBurr:
    //                        case eVisInspectType.eInspTypeDownSet:
    //                            StateAlignedMaskProperty stateAlignedProperty = inspItem.InspectionAlgorithm as StateAlignedMaskProperty;
    //                            if (stateAlignedProperty != null)
    //                            {
    //                                #region Allocate StateAlignedMaskProperty
    //                                if (inspItem.InspectionType.InspectType == eVisInspectType.eInspTypeSurface)
    //                                    item.InspName = InspNameList[1];
    //                                if (inspItem.InspectionType.InspectType == eVisInspectType.eInspTypeSpace)
    //                                    item.InspName = InspNameList[2];
    //                                if (inspItem.InspectionType.InspectType == eVisInspectType.eInspTypePSR)
    //                                    item.InspName = InspNameList[4];
    //                                if (inspItem.InspectionType.InspectType == eVisInspectType.eInspTypePunchBurr)
    //                                    item.InspName = InspNameList[12];
    //                                if (inspItem.InspectionType.InspectType == eVisInspectType.eInspTypeDownSet)
    //                                    item.InspName = InspNameList[13];

    //                                item.ThreshType = ThreshTypeList[stateAlignedProperty.ThreshType];
    //                                item.LowerThresh = stateAlignedProperty.LowerThresh;
    //                                item.UpperThresh = stateAlignedProperty.UpperThresh;
    //                                item.MaskLowerThresh = stateAlignedProperty.MaskLowerThresh;
    //                                item.MaskUpperThresh = stateAlignedProperty.MaskUpperThresh;
    //                                item.ApplyAverDiff = (stateAlignedProperty.ApplyAverDiff == 1) ? true : false;
    //                                item.AverMinMargin = stateAlignedProperty.AverMinMargin;
    //                                item.AverMaxMargin = stateAlignedProperty.AverMaxMargin;
    //                                item.InspRange = InspRangeList[stateAlignedProperty.InspRange];
    //                                item.MinMargin = stateAlignedProperty.MinMargin;
    //                                item.MaxMargin = stateAlignedProperty.MaxMargin;
    //                                item.ErosionTrainIter = stateAlignedProperty.ErosionTrainIter;
    //                                item.DilationTrainIter = stateAlignedProperty.DilationTrainIter;
    //                                item.ErosionInspIter = stateAlignedProperty.ErosionInspIter;
    //                                item.DilationInspIter = stateAlignedProperty.DilationInspIter;
    //                                item.MinDefectSize = stateAlignedProperty.MinDefectSize;
    //                                item.MaxDefectSize = stateAlignedProperty.MaxDefectSize;
    //                                item.MinSmallDefectSize = stateAlignedProperty.MinSmallDefectSize;
    //                                item.MaxSmallDefectSize = stateAlignedProperty.MaxSmallDefectSize;
    //                                item.MinSmallDefectCount = stateAlignedProperty.MinSmallDefectCount;
    //                                item.MaxSmallDefectCount = stateAlignedProperty.MaxSmallDefectCount;
    //                                #endregion
    //                            }
    //                            else continue;
    //                            break;

    //                        case eVisInspectType.eInspTypeDamBar: // Ball 검사
    //                            DamSizeProperty damProperty = inspItem.InspectionAlgorithm as DamSizeProperty;
    //                            if (damProperty != null)
    //                            {
    //                                #region Allocate BallPatternProperty
    //                                item.InspName = InspNameList[14];
    //                                item.ThreshType = ThreshTypeList[damProperty.ThreshType];
    //                                item.LowerThresh = damProperty.LowerThresh;
    //                                item.MinMargin = damProperty.MinMargin;
    //                                item.MinDefectSize = damProperty.MinDefectSize;
    //                                item.MaxDefectSize = damProperty.MaxDefectSize;
    //                                item.MinWidthSize = damProperty.MinWidth;
    //                                #endregion
    //                            }
    //                            else continue;
    //                            break;

    //                        case eVisInspectType.eInspTypeBallPattern: // Ball 검사
    //                            BallPatternProperty ballProperty = inspItem.InspectionAlgorithm as BallPatternProperty;
    //                            if (ballProperty != null)
    //                            {
    //                                #region Allocate BallPatternProperty
    //                                item.InspName = InspNameList[8];
    //                                item.ThreshType = ThreshTypeList[ballProperty.ThreshType];
    //                                item.LowerThresh = ballProperty.LowerThresh;
    //                                item.UpperThresh = ballProperty.UpperThresh;
    //                                item.MaskLowerThresh = ballProperty.MaskLowerThresh;
    //                                item.MaskUpperThresh = ballProperty.MaskUpperThresh;
    //                                item.ApplyAverDiff = (ballProperty.ApplyAverDiff == 1) ? true : false;
    //                                item.AverMinMargin = ballProperty.AverMinMargin;
    //                                item.AverMaxMargin = ballProperty.AverMaxMargin;
    //                                item.InspRange = InspRangeList[ballProperty.InspRange];
    //                                item.MinMargin = ballProperty.MinMargin;
    //                                item.MaxMargin = ballProperty.MaxMargin;
    //                                item.ErosionTrainIter = ballProperty.ErosionTrainIter;
    //                                item.DilationTrainIter = ballProperty.DilationTrainIter;
    //                                item.ErosionInspIter = ballProperty.ErosionInspIter;
    //                                item.DilationInspIter = ballProperty.DilationInspIter;
    //                                item.MinDefectSize = ballProperty.MinDefectSize;
    //                                item.MaxDefectSize = ballProperty.MaxDefectSize;
    //                                item.MinWidthSize = ballProperty.MinWidthSize;
    //                                item.MinHeightSize = ballProperty.MinHeightSize;
    //                                item.MaxHeightSize = ballProperty.MaxHeightSize;
    //                                item.MinSmallDefectSize = ballProperty.MinSmallDefectSize;
    //                                item.MaxSmallDefectSize = ballProperty.MaxSmallDefectSize;
    //                                item.MinSmallDefectCount = ballProperty.MinSmallDefectCount;
    //                                item.MaxSmallDefectCount = ballProperty.MaxSmallDefectCount;
    //                                item.UsePSRShift = ballProperty.UsePSRShift;
    //                                item.UsePunchShift = ballProperty.UsePunchShift;
    //                                #endregion
    //                            }
    //                            else continue;
    //                            break;

    //                        case eVisInspectType.eInspTypeRailAlignedMask: // Ball 검사
    //                            OuterProperty outerProperty = inspItem.InspectionAlgorithm as OuterProperty;
    //                            if (outerProperty != null)
    //                            {
    //                                #region Allocate BallPatternProperty
    //                                item.InspName = InspNameList[16];
    //                                item.LowerThresh = outerProperty.LowerThresh;
    //                                item.UpperThresh = outerProperty.UpperThresh;
    //                                item.MaskLowerThresh = outerProperty.MaskLowerThresh;
    //                                item.MaskUpperThresh = outerProperty.MaskUpperThresh;
    //                                item.ErosionTrainIter = outerProperty.ErosionTrainIter;
    //                                item.DilationTrainIter = outerProperty.DilationTrainIter;
    //                                item.MinDefectSize = outerProperty.MinDefectSize;
    //                                item.MaxDefectSize = outerProperty.MaxDefectSize;
    //                                #endregion
    //                            }
    //                            else continue;
    //                            break;

    //                        case eVisInspectType.eInspTypeUnitFidu: // 인식키 검사
    //                            CrossPatternProperty crossPatternProperty = inspItem.InspectionAlgorithm as CrossPatternProperty;
    //                            if (crossPatternProperty != null)
    //                            {
    //                                #region Allocate CrossPatternProperty
    //                                item.InspName = InspNameList[9];
    //                                item.ThreshType = ThreshTypeList[crossPatternProperty.ThreshType];
    //                                item.LowerThresh = crossPatternProperty.LowerThresh;
    //                                item.UpperThresh = crossPatternProperty.UpperThresh;
    //                                item.UsePSRShift = crossPatternProperty.UsePSRShift;
    //                                item.PsrMarginX = crossPatternProperty.PsrMarginX;
    //                                item.PsrMarginY = crossPatternProperty.PsrMarginY;
    //                                item.UsePunchShift = crossPatternProperty.UsePunchShift;
    //                                item.MinDefectSize = crossPatternProperty.MinDefectSize;
    //                                item.MinHeightSize = crossPatternProperty.MinHeightsize;
    //                                #endregion
    //                            }
    //                            else continue;
    //                            break;

    //                        case eVisInspectType.eInspTypeUnitPattern: // 
    //                            UnitPatternProperty unitPatternProperty = inspItem.InspectionAlgorithm as UnitPatternProperty;
    //                            if (unitPatternProperty != null)
    //                            {
    //                                #region Allocate unitPatternProperty
    //                                item.InspName = InspNameList[15];
    //                                item.ThreshType = ThreshTypeList[1];
    //                                item.LowerThresh = unitPatternProperty.LowerThresh;
    //                                item.MinDefectSize = unitPatternProperty.MinDefectSize;
    //                                #endregion
    //                            }
    //                            else continue;
    //                            break;

    //                        case eVisInspectType.eInspTypeVentHole: // VentHole 검사
    //                            VentHoleProperty ventHoleProperty = inspItem.InspectionAlgorithm as VentHoleProperty;
    //                            if (ventHoleProperty != null)
    //                            {
    //                                #region Allocate ventHoleProperty
    //                                item.InspName = InspNameList[11];
    //                                item.ThreshType = ThreshTypeList[ventHoleProperty.ThreshType];
    //                                item.LowerThresh = ventHoleProperty.LowerThresh;
    //                                item.MinDefectSize = ventHoleProperty.MinDefectSize;
    //                                #endregion
    //                            }
    //                            else continue;
    //                            break;

    //                        case eVisInspectType.eInspTypeCoreCrack: // WindowPunch 검사
    //                            WindowPunchProperty windowPunchProperty = inspItem.InspectionAlgorithm as WindowPunchProperty;
    //                            if (windowPunchProperty != null)
    //                            {
    //                                #region Allocate WindowPunchProperty
    //                                item.InspName = InspNameList[10];
    //                                item.ThreshType = ThreshTypeList[windowPunchProperty.ThreshType];
    //                                item.LowerThresh = windowPunchProperty.LowerThresh;
    //                                item.UpperThresh = windowPunchProperty.UpperThresh;
    //                                item.UsePSRShift = windowPunchProperty.UsePSRShift;
    //                                item.PsrMarginX = windowPunchProperty.PsrMarginX;
    //                                item.PsrMarginY = windowPunchProperty.PsrMarginY;
    //                                item.UsePunchShift = windowPunchProperty.UsePunchShift;
    //                                item.MinDefectSize = windowPunchProperty.MinDefectSize;
    //                                item.MinHeightSize = windowPunchProperty.MinHeightsize;
    //                                item.ErosionTrainIter = windowPunchProperty.ErosionTrainIter;
    //                                #endregion
    //                            }
    //                            else continue;
    //                            break;

    //                        case eVisInspectType.eInspTypePlate: // 도금 검사
    //                        case eVisInspectType.eInspTypeTape: // Tape 검사
    //                            ShapeShiftProperty shapeShiftProperty = inspItem.InspectionAlgorithm as ShapeShiftProperty;
    //                            if (shapeShiftProperty != null)
    //                            {
    //                                #region Allocate ShapeShiftProperty
    //                                if (inspItem.InspectionType.InspectType == eVisInspectType.eInspTypePlate)
    //                                    item.InspName = InspNameList[3];
    //                                if (inspItem.InspectionType.InspectType == eVisInspectType.eInspTypeTape)
    //                                    item.InspName = InspNameList[4];

    //                                item.GroundLowerThresh = shapeShiftProperty.GroundLowerThresh;
    //                                item.GroundUpperThresh = shapeShiftProperty.GroundUpperThresh;
    //                                item.ShapeLowerThresh = shapeShiftProperty.ShapeLowerThresh;
    //                                item.ShapeUpperThresh = shapeShiftProperty.ShapeUpperThresh;
    //                                item.ErosionShapeIter = shapeShiftProperty.ErosionShapeIter;
    //                                item.DilationShapeIter = shapeShiftProperty.DilationShapeIter;
    //                                item.ShapeShiftMarginX = shapeShiftProperty.ShapeShiftMarginX;
    //                                item.ShapeShiftMarginY = shapeShiftProperty.ShapeShiftMarginY;
    //                                item.ShapeOffsetX = shapeShiftProperty.ShapeOffsetX;
    //                                item.ShapeOffsetY = shapeShiftProperty.ShapeOffsetY;
    //                                item.ErosionTrainIter = shapeShiftProperty.ErosionTrainIter;
    //                                item.DilationTrainIter = shapeShiftProperty.DilationTrainIter;

    //                                item.MasterUse = (shapeShiftProperty.IsInspectMaster == 1) ? true : false;
    //                                item.MasterThreshType = ThreshTypeList[shapeShiftProperty.ThreshType];
    //                                item.MasterLowerThresh = shapeShiftProperty.LowerThresh;
    //                                item.MasterUpperThresh = shapeShiftProperty.UpperThresh;
    //                                item.MasterApplyAverDiff = (shapeShiftProperty.ApplyAverDiff == 1) ? true : false;
    //                                item.MasterAverMinMargin = shapeShiftProperty.AverMinMargin;
    //                                item.MasterAverMaxMargin = shapeShiftProperty.AverMaxMargin;
    //                                item.MasterInspRange = InspRangeList[shapeShiftProperty.InspRange];
    //                                item.MasterMinMargin = shapeShiftProperty.MinMargin;
    //                                item.MasterMaxMargin = shapeShiftProperty.MaxMargin;
    //                                item.MasterMinDefectSize = shapeShiftProperty.MinDefectSize;
    //                                item.MasterMaxDefectSize = shapeShiftProperty.MaxDefectSize;

    //                                item.SlaveUse = (shapeShiftProperty.IsInspectSlave == 1) ? true : false;
    //                                item.SlaveThreshType = ThreshTypeList[shapeShiftProperty.ThreshType2];
    //                                item.SlaveLowerThresh = shapeShiftProperty.InspLowerThresh2;
    //                                item.SlaveUpperThresh = shapeShiftProperty.InspUpperThresh2;
    //                                item.SlaveApplyAverDiff = (shapeShiftProperty.ApplyAverDiff2 == 1) ? true : false;
    //                                item.SlaveAverMinMargin = shapeShiftProperty.AverMinMargin2;
    //                                item.SlaveAverMaxMargin = shapeShiftProperty.AverMaxMargin2;
    //                                item.SlaveInspRange = InspRangeList[shapeShiftProperty.InspRange2];
    //                                item.SlaveMinMargin = shapeShiftProperty.InspMinMargin2;
    //                                item.SlaveMaxMargin = shapeShiftProperty.InspMaxMargin2;
    //                                item.SlaveMinDefectSize = shapeShiftProperty.MinDefectSize2;
    //                                item.SlaveMaxDefectSize = shapeShiftProperty.MaxDefectSize2;
    //                                #endregion
    //                            }
    //                            else continue;
    //                            break;
    //                        case eVisInspectType.eInspTypeRailIntensity: // Downset 검사
    //                            StateIntensityProperty railProperty = inspItem.InspectionAlgorithm as StateIntensityProperty;
    //                            if (railProperty != null)
    //                            {
    //                                #region Allocate StateIntensityProperty
    //                                item.InspName = InspNameList[5];

    //                                item.LowerThresh = railProperty.LowerThresh;
    //                                item.UpperThresh = railProperty.UpperThresh;
    //                                item.ApplyAverDiff = (railProperty.ApplyAverDiff == 1) ? true : false;
    //                                item.AverMinMargin = railProperty.AverMinMargin;
    //                                item.AverMaxMargin = railProperty.AverMaxMargin;
    //                                item.MinMargin = 0;// downsetProperty.MinMargin;
    //                                item.MaxMargin = 0;// downsetProperty.MaxMargin;
    //                                item.ErosionTrainIter = railProperty.ErosionTrainIter;
    //                                item.DilationTrainIter = 0;// downsetProperty.DilationTrainIter;
    //                                item.ErosionInspIter = 0;// downsetProperty.ErosionInspIter;
    //                                item.DilationInspIter = 0;// downsetProperty.DilationInspIter;
    //                                item.MinDefectSize = railProperty.MinDefectSize;
    //                                item.Invert = (railProperty.Invert == 1) ? true : false;
    //                                #endregion
    //                            }
    //                            else continue;
    //                            break;
    //                        case eVisInspectType.eInspTypeLeadShapeWithCL: // 리드 형상 검사
    //                            LeadShapeWithCenterLineProperty leadShapeProperty = inspItem.InspectionAlgorithm as LeadShapeWithCenterLineProperty;
    //                            if (leadShapeProperty != null)
    //                            {
    //                                #region Allocate LeadShapeWithCenterLineProperty
    //                                item.InspName = InspNameList[6];

    //                                item.LowerThresh = leadShapeProperty.LowerThresh;
    //                                item.UpperThresh = leadShapeProperty.UpperThresh;
    //                                item.MaskLowerThresh = leadShapeProperty.MaskLowerThresh;
    //                                item.MinDefectSize = leadShapeProperty.MinDefectSize;
    //                                item.LeadMinNormalRatio = leadShapeProperty.MinNormalRatio;
    //                                item.LeadMaxNormalRatio = leadShapeProperty.MaxNormalRatio;
    //                                item.LeadMinHeightSize = leadShapeProperty.MinHeightsize;
    //                                item.LeadMinWidthRatio = leadShapeProperty.MinWidthRatio;
    //                                item.LeadMaxWidthRatio = leadShapeProperty.MaxWidthRatio;
    //                                item.LeadMinWidthSize = leadShapeProperty.MinWidthSize;
    //                                item.LeadMaxWidthSize = leadShapeProperty.MaxWidthSize;
    //                                #endregion
    //                            }
    //                            else continue;
    //                            break;
    //                        case eVisInspectType.eInspTypeLeadGap: // 리드 간격 검사
    //                            LeadGapProperty leadGapProperty = inspItem.InspectionAlgorithm as LeadGapProperty;
    //                            if (leadGapProperty != null)
    //                            {
    //                                item.InspName = InspNameList[7];

    //                                item.LowerThresh = leadGapProperty.LowerThresh;
    //                                item.UpperThresh = leadGapProperty.UpperThresh;
    //                                item.LeadMinWidthSize = leadGapProperty.MinWidthSize;
    //                            }
    //                            else continue;
    //                            break;
    //                        case eVisInspectType.eInspTypeSpaceShapeWithCL: // 공간 형상 검사
    //                            SpaceShapeWithCenterLineProperty spaceShapeProperty = inspItem.InspectionAlgorithm as SpaceShapeWithCenterLineProperty;
    //                            if (spaceShapeProperty != null)
    //                            {
    //                                #region Allocate SpaceShapeWithCenterLineProperty
    //                                item.InspName = InspNameList[7];

    //                                item.LowerThresh = spaceShapeProperty.LowerThresh;
    //                                item.UpperThresh = spaceShapeProperty.UpperThresh;
    //                                item.ShapeMinNormalRatio = spaceShapeProperty.MinNormalRatio;
    //                                item.ShapeMaxNormalRatio = spaceShapeProperty.MaxNormalRatio;
    //                                item.ShapeMinHeightSize = spaceShapeProperty.MinHeightsize;
    //                                item.ShapeMinWidthRatio = spaceShapeProperty.MinWidthRatio;
    //                                item.ShapeMaxWidthRatio = spaceShapeProperty.MaxWidthRatio;
    //                                item.ShapeMinWidthSize = spaceShapeProperty.MinWidthSize;
    //                                item.ShapeMaxWidthSize = spaceShapeProperty.MaxWidthSize;
    //                                #endregion
    //                            }
    //                            else continue;
    //                            break;

    //                        default: // 그 외의 검사 Item인 경우 pass.
    //                            continue;
    //                    }
    //                    inspItemList.Add(item);
    //                }
    //            }
    //        }
    //    }
    //    #endregion Data Handling.

    //    #region INotifyPropertyChanged Members
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    public void NotifyPropertyChanged(string propertyName)
    //    {
    //        if (PropertyChanged != null)
    //        {
    //            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    //        }
    //    }
    //    #endregion
    //}

    //#region Data Model.
    ////For filtering.
    //public class IdString
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }

    //    public IdString(int id, string name)
    //    {
    //        Id = id;
    //        Name = name;
    //    }
    //}

    //public class InspItem
    //{
    //    //Header
    //    public string RoiCode { get; set; }
    //    public IdString WorkType { get; set; }
    //    public IdString SectionName { get; set; }
    //    public IdString InspName { get; set; }

    //    public IdString ThreshType { get; set; } // 임계 구분; 임계 이상 | 임계 이하 | 임계 범위
    //    public int LowerThresh { get; set; } // 하한 임계
    //    public int UpperThresh { get; set; } // 상한 임계

    //    public int MaskLowerThresh { get; set; } // 마스크 하한임계
    //    public int MaskUpperThresh { get; set; } // 마스크 상한임계

    //    public bool ApplyAverDiff { get; set; } // 평균값 적용
    //    public int AverMinMargin { get; set; } // 평균값 최소범위
    //    public int AverMaxMargin { get; set; } // 평균값 최대범위

    //    public IdString InspRange { get; set; } // 검사조건 범위; Min | Max | In
    //    public int MinMargin { get; set; } // 검사 최소 허용범위
    //    public int MaxMargin { get; set; } // 검사 최대 허용범위

    //    public int ErosionTrainIter { get; set; } // 마스크 축소
    //    public int DilationTrainIter { get; set; } // 마스크 확장

    //    public int ErosionInspIter { get; set; } // 결과 축소
    //    public int DilationInspIter { get; set; } // 결과 확장

    //    public int MinDefectSize { get; set; } // 최소 검출사이즈(Min)
    //    public int MaxDefectSize { get; set; } // 최소 검출사이즈(Max)

    //    public int MinWidthSize { get; set; } // 최소 검출사이즈(Max)
    //    public int MinHeightSize { get; set; } // 최소 검출사이즈(Max)
    //    public int MaxHeightSize { get; set; } // 최소 검출사이즈(Max)

    //    public int MinSmallDefectSize { get; set; } // 미세불량 최소 검출사이즈(Min)
    //    public int MaxSmallDefectSize { get; set; } // 미세불량 최소 검출사이즈(Max)

    //    public int MinSmallDefectCount { get; set; } // 미세불량 최소 검출개수(Min)
    //    public int MaxSmallDefectCount { get; set; } // 미세불량 최소 검출개수(Max)

    //    // 도금, Tape 전용 프로퍼티
    //    public int GroundLowerThresh { get; set; } // 전체 Mask 하한
    //    public int GroundUpperThresh { get; set; } // 전체 Mask 상한
    //    public int ShapeLowerThresh { get; set; } // 형상 Mask 하한
    //    public int ShapeUpperThresh { get; set; } // 형상 Mask 상한
    //    public int ErosionShapeIter { get; set; } // 형상 Mask 축소
    //    public int DilationShapeIter { get; set; } // 형상 Mask 확장
    //    public int ShapeShiftMarginX { get; set; } // 형상 Shift X
    //    public int ShapeShiftMarginY { get; set; } // 형상 Shift Y
    //    public int ShapeOffsetX { get; set; } // 형상 Offset X
    //    public int ShapeOffsetY { get; set; } // 형상 Offset Y

    //    // 도금, Tape의 형상 영역 전용 프로퍼티
    //    public bool MasterUse { get; set; } // 형상 영역 검사 적용
    //    public IdString MasterThreshType { get; set; } // 형상 영역 임계 구분
    //    public int MasterThreshold { get; set; } // 형상 영역 임계 값
    //    public int MasterLowerThresh { get; set; } // 형상 영역 하한 임계
    //    public int MasterUpperThresh { get; set; } // 형상 영역 상한 임계
    //    public bool MasterApplyAverDiff { get; set; } // 형상 영역 평균값 적용
    //    public int MasterAverMinMargin { get; set; } // 형상 영역 평균값 최소범위
    //    public int MasterAverMaxMargin { get; set; } // 형상 영역 평균값 최대범위
    //    public IdString MasterInspRange { get; set; } // 형상 영역 검사조건 범위; Min | Max | In
    //    public int MasterMinMargin { get; set; } // 형상 영역 검사최소 허용범위
    //    public int MasterMaxMargin { get; set; } // 형상 영역 검사최대 허용범위
    //    public int MasterMinDefectSize { get; set; } // 형상 영역 최소 검출사이즈(Min)
    //    public int MasterMaxDefectSize { get; set; } // 형상 영역 최소 검출사이즈(Max)

    //    // 도금, Tape의 표면 영역 전용 프로퍼티
    //    public bool SlaveUse { get; set; } // 표면 영역 검사 적용
    //    public IdString SlaveThreshType { get; set; } // 표면 영역 임계 구분
    //    public int SlaveThreshold { get; set; } // 표면 영역 임계 값
    //    public int SlaveLowerThresh { get; set; } // 표면 영역 하한 임계
    //    public int SlaveUpperThresh { get; set; } // 표면 영역 상한 임계
    //    public bool SlaveApplyAverDiff { get; set; } // 표면 영역 평균값 적용
    //    public int SlaveAverMinMargin { get; set; } // 표면 영역 평균값 최소범위
    //    public int SlaveAverMaxMargin { get; set; } // 표면 영역 평균값 최대범위
    //    public IdString SlaveInspRange { get; set; } // 표면 영역 검사조건 범위; Min | Max | In
    //    public int SlaveMinMargin { get; set; } // 표면 영역 검사최소 허용범위
    //    public int SlaveMaxMargin { get; set; } // 표면 영역 검사최대 허용범위
    //    public int SlaveMinDefectSize { get; set; } // 표면 영역 최소 검출사이즈(Min)
    //    public int SlaveMaxDefectSize { get; set; } // 표면 영역 최소 검출사이즈(Max)

    //    // 다운셋 전용 프로퍼티
    //    public bool Invert { get; set; } // 역 검사

    //    // 리드 형상, 간격 전용 프로퍼티
    //    public int LeadMinNormalRatio { get; set; } // 리드편차 최소 허용범위(%)
    //    public int LeadMaxNormalRatio { get; set; } // 리드편차 최대 허용범위(%)
    //    public int LeadMinHeightSize { get; set; } // 리드 연속 불량 개수
    //    public int LeadMinWidthRatio { get; set; } // 리드 최소 선폭범위(%)
    //    public int LeadMaxWidthRatio { get; set; } // 리드 최대 선폭범위(%)
    //    public int LeadMinWidthSize { get; set; } // 리드 최소 선폭(px) | 리드 최소선폭 허용범위(px)
    //    public int LeadMaxWidthSize { get; set; } // 리드 최대 선폭(px)

    //    // 공간 형상 검사 전용 프로퍼티
    //    public int ShapeMinNormalRatio { get; set; } // 공간편차 최소 허용범위(%)
    //    public int ShapeMaxNormalRatio { get; set; } // 공간편차 최대 허용범위(%)
    //    public int ShapeMinHeightSize { get; set; } // 공간 연속 불량 개수
    //    public int ShapeMinWidthRatio { get; set; } // 공간 최소 선폭범위(%)
    //    public int ShapeMaxWidthRatio { get; set; } // 공간 최대 선폭범위(%)
    //    public int ShapeMinWidthSize { get; set; } // 공간 최소 선폭(px)
    //    public int ShapeMaxWidthSize { get; set; } // 공간 최대 선폭(px)

    //    // Unit Align 전용 프로퍼티
    //    public int AlignMarginX { get; set; } // X Margin
    //    public int AlignMarginY { get; set; } // Y Margin
    //    public int AlignAcceptance { get; set; } // 일치율

    //    public int UsePSRShift { get; set; }
    //    public int UsePunchShift { get; set; }
    //    public int PsrMarginX { get; set; }
    //    public int PsrMarginY { get; set; }
    //}
    //#endregion
}
