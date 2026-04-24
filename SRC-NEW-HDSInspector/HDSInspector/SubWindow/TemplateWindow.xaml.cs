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
 * @file  TemplateWindow.xaml.cs
 * @brief
 * Inspect Method Templete Window 
 * 
 * @author : suoow <suoow.yeo@haesung.net>
 * @date : 2015.05.20
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2015.05.10 First creation.
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
using PCS.ModelTeaching;
using Common.Drawing.InspectionInformation;
using Common.Drawing;
using Common.Drawing.InspectionTypeUI;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.IO;

namespace HDSInspector.SubWindow
{
    /// <summary>
    /// TemplateWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public partial class TemplateWindow : Window, INotifyPropertyChanged
    {
        #region Constructor & Initializer.

        ObservableCollection<InspectList> Inspect = new ObservableCollection<InspectList>();

        public InspectList SelectedItem;

        GraphicsBase SelectedGraphic;
        
        TeachingWindow m_ptrTeachingWindow;

        int nID = 0;

        public bool OK = false;

        string m_Path = System.IO.Directory.GetCurrentDirectory() + "\\..\\Config\\";//Templete.cfg";

        public TemplateWindow(TeachingWindow tw, GraphicsBase newGraphic, int id)
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
            m_ptrTeachingWindow = tw;
            SelectedGraphic = newGraphic;
            nID = id;
            m_Path+= "Templete" + nID.ToString() + ".cfg";
            LoadFromFile();
        }
        
        private void Dispose()
        {
            cmbInspectionType.ItemsSource = null;
            this.pnlInspectionSettings.Children.Clear();
        }

        private void InitializeDialog()
        {
            cmbInspectionType.DisplayMemberPath = "Name";
            cmbInspectionType.SelectedValuePath = "SettingControl";
            cmbInspectionType.ItemsSource = InspectionType.GetDefaultInspectionTypeList();
            cmbInspectionType.SelectedIndex = 0;
            lbName.DataContext = Inspect;
            grdInput.Visibility = System.Windows.Visibility.Hidden;
            grdName.IsEnabled = true;
            grdInsepct.IsEnabled = true;
        }

        private void InitializeEvent()
        {
          //  this.Loaded += new RoutedEventHandler(TemplateWindow_Loaded);
            this.lbName.SelectionChanged += new SelectionChangedEventHandler(lbName_SelectionChanged);     
            
            this.cmbInspectionType.SelectionChanged += cmbInspectionType_SelectionChanged;
            this.cmbInspectionType.PreviewMouseLeftButtonDown += cmbInspectionType_MouseLeftButtonDown;
            this.lbInspection.SelectionChanged += lbInspection_SelectionChanged;
            this.lbInspection.PreviewMouseWheel += lbInspection_PreviewMouseWheel;

            this.btnSaveAll.Click += new RoutedEventHandler(btnSaveAll_Click);

            this.btnAddName.Click += new RoutedEventHandler(btnAddName_Click);
            this.btnRename.Click += new RoutedEventHandler(btnRename_Click);
            this.btnRenameOK.Click += new RoutedEventHandler(btnRenameOK_Click);
            this.btnRenameCancel.Click += new RoutedEventHandler(btnRenameCancel_Click);
            this.btnDeleteName.Click += new RoutedEventHandler(btnDeleteName_Click);
            this.btnSelect.Click += new RoutedEventHandler(btnSelect_Click);
            this.btnAddInspectionType.Click += new RoutedEventHandler(btnAddInspectionType_Click);
            this.btnSave.Click += (s, e) => SaveInspectionItem();
            this.btnSetPreview.Click += (s, e) => SetAsPreview();
            this.btnClose.Click += new RoutedEventHandler(btnClose_Click);
            this.btnDelete.Click += (s, e) => DeleteInspectionItem();
            this.PreviewKeyUp += new KeyEventHandler(TemplateWindow_PreviewKeyUp);
            
        }

        void btnSaveAll_Click(object sender, RoutedEventArgs e)
        {
            if (!SaveToFile()) MessageBox.Show("저장 할 수 없습니다.");
        }

        #endregion

        #region Item Event
        void btnAddName_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text.ToString().Trim() == "")
            {
                MessageBox.Show("영역을 입력 하세요.");
                return;
            }
            if (!AddItem(txtName.Text.ToString().Trim()))
            {
                MessageBox.Show("이미 등록된 영역 입니다.");
            }
            lbName.SelectedIndex = Inspect.Count - 1;
        }

        void btnDeleteName_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("선택된 영역이 없습니다.");
            }
            if (MessageBox.Show("정말 삭제 하시겠습니까?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (!DeleteItem())
                {

                    MessageBox.Show("삭제 할 수 없습니다.");
                }
            }
        }

        void btnRename_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                grdInput.Visibility = System.Windows.Visibility.Visible;
                grdName.IsEnabled = false;
                grdInsepct.IsEnabled = false;
                txtRename.Focus();
                txtRename.Text = SelectedItem.Name;
            }
            else
            {
                MessageBox.Show("선택된 영역이 없습니다.");
            }
        }

        void btnRenameCancel_Click(object sender, RoutedEventArgs e)
        {
            
            grdInput.Visibility = System.Windows.Visibility.Hidden;
            grdName.IsEnabled = true;
            grdInsepct.IsEnabled = true;
            
        }

        void btnRenameOK_Click(object sender, RoutedEventArgs e)
        {
            if (!RenameItem(txtRename.Text.ToString().Trim()))
            {
                MessageBox.Show("이미 등록된 영역입니다.");
                txtRename.Focus();
                txtRename.Text = SelectedItem.Name;
                return;
            }
            grdInput.Visibility = System.Windows.Visibility.Hidden;
            grdName.IsEnabled = true;
            grdInsepct.IsEnabled = true;
            lbName.DataContext = Inspect;
        }
        #endregion

        #region Event
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

        private void lbInspection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbInspection.SelectedIndex == -1)
            {
               

                btnSave.IsEnabled = false;
                btnSetPreview.IsEnabled = false;
                btnDelete.IsEnabled = false;

            }
            else if (SelectedItem == null)
            {
                SetDisableStateAddInspectionItem();
                
            }
            else
            {
                try
                {
                    SetEnableStateAddInspectionItem();

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

                                }

                                ((IInspectionTypeUICommands)inspectionElement.SettingControl).Display(selectedItem, MainWindow.CurrentModel.Strip.PSRMarginX, MainWindow.CurrentModel.Strip.PSRMarginY);
                                selectedItem.InspectionType.SettingControl = InspectionType.GetInspectionTypeSettingDialog(selectedItem.InspectionType.InspectType);
                                ((IInspectionTypeUICommands)selectedItem.InspectionType.SettingControl).SetDialog(selectedItem.InspectionType.Name, (eVisInspectType)selectedItem.InspectionType.InspectType);
                                pnlInspectionSettings.Children.Clear();
                                pnlInspectionSettings.Children.Add(inspectionElement.SettingControl);
                                
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

        void TemplateWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        { if (e.Key == Key.Enter) { SaveInspectionItem(); } }

        void btnAddInspectionType_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("선택된 영역이 없습니다.");
                return;
            }
            AddInspectionItem(cmbInspectionType.SelectedItem as InspectionType);
        }

        void TemplateWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //  InitializeDialog();
            this.pnlInspectionSettings.Children.Clear();
            cmbInspectionType.SelectedIndex = 0;
            cmbInspectionType.Focus();

        }

        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.cmbInspectionType.SelectedIndex = -1;
            this.DialogResult = false;

        }

        void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            int n = SelectedGraphic.InspectionList.Count;
            this.cmbInspectionType.SelectedIndex = -1;
            try
            {
                foreach (InspectionItem selectedItem in SelectedItem.InspectionList)
                {
                    m_ptrTeachingWindow.InspectionTypectrl.cmbInspectionType.SelectedItem = selectedItem.InspectionType;
                    m_ptrTeachingWindow.InspectionTypectrl.lbInspection.SelectedIndex = -1;
                    m_ptrTeachingWindow.InspectionTypectrl.AddInspectionItem(selectedItem.InspectionType);
                    SelectedGraphic.InspectionList[n].InspectionAlgorithm = selectedItem.InspectionAlgorithm.Clone();
                    n++;
                }
                SelectedGraphic.RefreshDrawing();
                DialogResult = true;

            }
            catch
            {
                MessageBox.Show("검사 방법 추가에 실패 하였습니다.");
                DialogResult = false;
            }
        }

        void lbName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItem = lbName.SelectedItem as InspectList;
            if (SelectedItem == null) return;
            tbName.Text = "[" + SelectedItem.Name + "]";

            this.lbInspection.DataContext = SelectedItem.InspectionList;
            if (SelectedItem.InspectionList.Count > 0)
            {
                this.lbInspection.SelectedIndex = SelectedItem.InspectionList.Count - 1;
            }
            else
            {
                this.lbInspection.SelectedIndex = -1;
            }

            this.cmbInspectionType.IsEnabled = true; // 검사 방법 추가 가능한 상태
            this.btnAddInspectionType.IsEnabled = true;
        }      
        #endregion

        #region Add inspection item setting panel.
        private void SetEnableStateAddInspectionItem()
        {
            this.cmbInspectionType.IsEnabled = true; // 검사 방법 추가 가능한 상태
            this.btnAddInspectionType.IsEnabled = true;
            this.btnSave.IsEnabled = true;
            this.btnSetPreview.IsEnabled = true;
            this.btnDelete.IsEnabled = true;
        }

        private void SetDisableStateAddInspectionItem()
        {
            this.cmbInspectionType.IsEnabled = false;
            this.btnAddInspectionType.IsEnabled = false;
            this.btnSave.IsEnabled = false;
            this.btnSetPreview.IsEnabled = false;
            this.btnDelete.IsEnabled = false;
        }
        #endregion

        #region combobox events.
        private void cmbInspectionType_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedItem != null)
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

        #region Name Handling

        private bool AddItem(string name)
        {
            bool ret = false;
            if (IsValidName(name))
            {
                InspectList il = new InspectList(name, Inspect.Count+1);
                Inspect.Add(il);
                ret = true;
            }
            return ret;
        }

        private bool RenameItem(string name)
        {
            bool ret = false;
            if (SelectedItem != null)
            {
                if (IsValidName(name))
                {
                    SelectedItem.Name = name;
                    ret = true;
                }
            }
            return ret;
        }

        private bool DeleteItem()
        {
            bool ret = false;
            if (SelectedItem != null)
            {
                ret = Inspect.Remove(SelectedItem);
                if (ret) SelectedItem = null;
            }
            return ret;
        }

        private bool IsValidName(string name)
        {
            foreach (InspectList a in Inspect)
            {
                if (a.Name == name) return false;
            }
            return true;
        }

        #endregion

        #region File

        private bool LoadFromFile()
        {
            bool ret = false;
            Inspect.Clear();
            if (File.Exists(m_Path))
            {
                int p = 0;
                string[] tmpList = File.ReadAllLines(m_Path);
                for (int i=0; i< tmpList.Length; i++)
                {
                    if (tmpList[i].Contains("[END]"))
                    {
                        string[] tmp = new string[i-p];
                        Array.Copy(tmpList, p, tmp, 0, tmp.Length);
                        p = i + 1;
                        InspectList il = StringListToInspectList(tmp, Inspect.Count+1);
                        Inspect.Add(il);
                    }
                }
                ret = true;
            }
            return ret;
        }

        private bool SaveToFile()
        {
            bool ret = false;
            if (File.Exists(m_Path))
            {
                File.Delete(m_Path);
            }
            if (!File.Exists(m_Path))
            {
               StreamWriter sw = File.CreateText(m_Path);
               sw.Close();
            }
            try
            {
                if (Inspect.Count < 1) return false;
                string appendText= "";
                foreach (InspectList il in Inspect)
                {
                    if (il.InspectionList.Count < 1) continue;
                    appendText = "[" + il.Name + "]";
                    File.AppendAllText(m_Path, appendText+ Environment.NewLine);
                    foreach (InspectionItem it in il.InspectionList)
                    {
                        appendText = InspectItemToString(it);
                        File.AppendAllText(m_Path, appendText + Environment.NewLine);
                    }
                    appendText = "[END]";
                    File.AppendAllText(m_Path, appendText + Environment.NewLine);
                }
                ret = true;
            }
            catch { }
            return ret;
        }

        private InspectList StringListToInspectList(string[] strList, int id)
        {
            InspectList il = new InspectList(strList[0].Substring(1,strList[0].Length-2), id);
            for (int i = 1; i < strList.Length; i++)
            {
                InspectionItem it = StringToInspectItem(strList[i], i);
                il.InspectionList.Add(it);
            }
            return il;
        }

        private InspectionItem StringToInspectItem(string str, int id)
        {
            string[] sl = str.Split(',');
            InspectionItem it;
            switch (sl[0])
            {
                case "검사 제외":
                    ExceptionalMaskProperty e = new ExceptionalMaskProperty();
                    e.ExceptionX = Convert.ToInt32(sl[1]);
                    e.ExceptionY = Convert.ToInt32(sl[2]);
                    e.UseShapeShift = Convert.ToInt32(sl[3]);
                    it = new InspectionItem(InspectionType.GetInspectionType(sl[0]), e, id);
                    break;
                case "표면 검사":
                case "Core Crack 검사":                
                case "Burr 검사":
                
                    StateAlignedMaskProperty p = new StateAlignedMaskProperty();
                    p.ThreshType = Convert.ToInt32(sl[1]);
                    p.LowerThresh = Convert.ToInt32(sl[2]);
                    p.UpperThresh = Convert.ToInt32(sl[3]);
                    p.ApplyAverDiff = Convert.ToInt32(sl[4]);
                    p.MaskLowerThresh = Convert.ToInt32(sl[5]);
                    p.MaskUpperThresh = Convert.ToInt32(sl[6]);
                    p.InspRange = Convert.ToInt32(sl[7]);
                    p.AverMinMargin = Convert.ToInt32(sl[8]);
                    p.AverMaxMargin = Convert.ToInt32(sl[9]);
                    p.MinMargin = Convert.ToInt32(sl[10]);
                    p.MaxMargin = Convert.ToInt32(sl[11]);
                    p.ErosionTrainIter = Convert.ToInt32(sl[12]);
                    p.DilationTrainIter = Convert.ToInt32(sl[13]);
                    p.ErosionInspIter = Convert.ToInt32(sl[14]);
                    p.DilationInspIter = Convert.ToInt32(sl[15]);
                    p.MinDefectSize = Convert.ToInt32(sl[16]);
                    p.MinSmallDefectSize = Convert.ToInt32(sl[17]);
                    p.MinSmallDefectCount = Convert.ToInt32(sl[18]);
                    p.MaxDefectSize = Convert.ToInt32(sl[19]);
                    p.MaxSmallDefectSize = Convert.ToInt32(sl[20]);
                    p.MaxSmallDefectCount = Convert.ToInt32(sl[21]);
                    it = new InspectionItem(InspectionType.GetInspectionType(sl[0]), p, id);
                    break;
                case "원소재 검사":
                    RawMetrialProperty rm = new RawMetrialProperty();
                    rm.ThreshType = Convert.ToInt32(sl[1]);             
                    rm.LowerThresh = Convert.ToInt32(sl[2]);            
                    rm.UpperThresh = Convert.ToInt32(sl[3]);            
                    rm.ApplyAverDiff = Convert.ToInt32(sl[4]);          
                    rm.MaskLowerThresh = Convert.ToInt32(sl[5]);        
                    rm.MaskUpperThresh = Convert.ToInt32(sl[6]);        
                    rm.InspRange = Convert.ToInt32(sl[7]);              
                    rm.AverMinMargin = Convert.ToInt32(sl[8]);          
                    rm.AverMaxMargin = Convert.ToInt32(sl[9]);          
                    rm.MinMargin = Convert.ToInt32(sl[10]);              
                    rm.MaxMargin = Convert.ToInt32(sl[11]);              
                    rm.ErosionTrainIter = Convert.ToInt32(sl[12]);   
                    rm.DilationTrainIter = Convert.ToInt32(sl[13]);
                    rm.ErosionInspIter = Convert.ToInt32(sl[14]);
                    rm.DilationInspIter = Convert.ToInt32(sl[15]);
                    rm.MinDefectSize = Convert.ToInt32(sl[16]);
                    rm.MinSmallDefectSize = Convert.ToInt32(sl[17]);
                    rm.MinSmallDefectCount = Convert.ToInt32(sl[18]);
                    rm.MaxDefectSize = Convert.ToInt32(sl[19]);
                    rm.MaxSmallDefectSize = Convert.ToInt32(sl[20]);
                    rm.MaxSmallDefectCount = Convert.ToInt32(sl[21]);
                    it = new InspectionItem(InspectionType.GetInspectionType(sl[0]), rm, id);
                    break;
                case "리드 형상 검사":
                    LeadShapeWithCenterLineProperty l = new LeadShapeWithCenterLineProperty();
                    l.ThreshType = Convert.ToInt32(sl[1]);
                    l.LowerThresh = Convert.ToInt32(sl[2]);
                    l.UpperThresh = Convert.ToInt32(sl[3]);
                    l.MinWidthRatio = Convert.ToInt32(sl[4]);
                    l.MinWidthSize = Convert.ToInt32(sl[5]);
                    l.MaxWidthRatio = Convert.ToInt32(sl[6]);
                    l.MaxWidthSize = Convert.ToInt32(sl[7]);
                    l.MinHeightsize = Convert.ToInt32(sl[8]);
                    l.MinNormalRatio = Convert.ToInt32(sl[9]);
                    l.MaxNormalRatio = Convert.ToInt32(sl[10]);
                    if (sl.Length < 12)
                        l.RemoveTipSize = 0;
                    else
                        l.RemoveTipSize = Convert.ToInt32(sl[11]);

                    it = new InspectionItem(InspectionType.GetInspectionType(sl[0]), l, id);
                    break;
                case "공간 형상 검사":
                    SpaceShapeWithCenterLineProperty s =new SpaceShapeWithCenterLineProperty();
                    s.ThreshType = Convert.ToInt32(sl[1]);
                    s.LowerThresh = Convert.ToInt32(sl[2]);
                    s.UpperThresh = Convert.ToInt32(sl[3]);
                    s.MinWidthRatio = Convert.ToInt32(sl[4]);
                    s.MinWidthSize = Convert.ToInt32(sl[5]);
                    s.MaxWidthRatio = Convert.ToInt32(sl[6]);
                    s.MaxWidthSize = Convert.ToInt32(sl[7]);
                    s.MinHeightsize = Convert.ToInt32(sl[8]);
                    s.MinNormalRatio = Convert.ToInt32(sl[9]);
                    s.MaxNormalRatio = Convert.ToInt32(sl[10]);
                    s.TipSearchSize = Convert.ToInt32(sl[11]);
                    it = new InspectionItem(InspectionType.GetInspectionType(sl[0]), s, id);
                    break;
                case "Window Punch 검사" :
                    WindowPunchProperty wp = new WindowPunchProperty();
                    wp.ThreshType = Convert.ToInt32(sl[1]);
                    wp.LowerThresh = Convert.ToInt32(sl[2]);
                    wp.UpperThresh = Convert.ToInt32(sl[3]);
                    wp.PsrMarginX = Convert.ToInt32(sl[4]);
                    wp.PsrMarginY = Convert.ToInt32(sl[5]);
                    wp.MinDefectSize = Convert.ToInt32(sl[6]);
                    wp.MinHeightsize = Convert.ToInt32(sl[7]);
                    wp.UsePSRShift = Convert.ToInt32(sl[8]);
                    wp.UsePunchShift = Convert.ToInt32(sl[9]);
                    wp.ErosionTrainIter = Convert.ToInt32(sl[10]);
                    it = new InspectionItem(InspectionType.GetInspectionType(sl[0]), wp, id);
                    break;
                case "인식키 검사":
                    CrossPatternProperty cp = new CrossPatternProperty();
                    cp.ThreshType = Convert.ToInt32(sl[1]);
                    cp.LowerThresh = Convert.ToInt32(sl[2]);
                    cp.UpperThresh = Convert.ToInt32(sl[3]);
                    cp.PsrMarginX = Convert.ToInt32(sl[4]);
                    cp.PsrMarginY = Convert.ToInt32(sl[5]);
                    cp.MinDefectSize = Convert.ToInt32(sl[6]);
                    cp.MinHeightsize = Convert.ToInt32(sl[7]);
                    cp.UsePSRShift = Convert.ToInt32(sl[8]);
                    cp.UsePSRShiftBA = Convert.ToInt32(sl[9]);
                    cp.UsePunchShift = Convert.ToInt32(sl[10]);
                    it = new InspectionItem(InspectionType.GetInspectionType(sl[0]), cp, id);
                    break;
                case "Vent Hole 검사" :
                    VentHoleProperty vh = new VentHoleProperty();
                    vh.ThreshType = Convert.ToInt32(sl[1]);
                    vh.LowerThresh = Convert.ToInt32(sl[2]);
                    vh.MinDefectSize = Convert.ToInt32(sl[3]);
                    it = new InspectionItem(InspectionType.GetInspectionType(sl[0]), vh, id);
                    break;

                case "Vent Hole 검사2":
                    VentHoleProperty2 vh2 = new VentHoleProperty2();
                    vh2.ThreshType = Convert.ToInt32(sl[1]);
                    vh2.LowerThresh = Convert.ToInt32(sl[2]);
                    vh2.MinDefectSize = Convert.ToInt32(sl[3]);
                    it = new InspectionItem(InspectionType.GetInspectionType(sl[0]), vh2, id);
                    break;

                case "Ball 검사":
                    BallPatternProperty si = new BallPatternProperty();
                    si.ThreshType = Convert.ToInt32(sl[1]);
                    si.LowerThresh = Convert.ToInt32(sl[2]);
                    si.UpperThresh = Convert.ToInt32(sl[3]);
                    si.MaskLowerThresh = Convert.ToInt32(sl[4]);
                    si.MaskUpperThresh = Convert.ToInt32(sl[5]);
                    si.InspRange = Convert.ToInt32(sl[6]);
                    si.ApplyAverDiff = Convert.ToInt32(sl[7]);
                    si.AverMinMargin = Convert.ToInt32(sl[8]);
                    si.AverMaxMargin = Convert.ToInt32(sl[9]);
                    si.MinMargin = Convert.ToInt32(sl[10]);
                    si.MaxMargin = Convert.ToInt32(sl[11]);
                    
                    si.ErosionTrainIter = Convert.ToInt32(sl[12]);
                    si.DilationTrainIter = Convert.ToInt32(sl[13]);
                    si.ErosionInspIter = Convert.ToInt32(sl[14]);
                    si.DilationInspIter = Convert.ToInt32(sl[15]);
                    si.MinDefectSize = Convert.ToInt32(sl[16]);

                    si.MinSmallDefectSize = Convert.ToInt32(sl[17]);
                    si.MinSmallDefectCount = Convert.ToInt32(sl[18]);
                    si.MinWidthSize = Convert.ToInt32(sl[19]);
                    si.MinHeightSize = Convert.ToInt32(sl[20]);
                    si.MaxHeightSize = Convert.ToInt32(sl[21]);
                    si.MaxDefectSize = Convert.ToInt32(sl[22]);
                    si.MaxSmallDefectSize = Convert.ToInt32(sl[23]);
                    si.MaxSmallDefectCount = Convert.ToInt32(sl[24]);
                    si.UsePSRShift = Convert.ToInt32(sl[25]);
                    si.UsePunchShift = Convert.ToInt32(sl[26]);
                    it = new InspectionItem(InspectionType.GetInspectionType(sl[0]), si,id);
                    break;

                case "PSR 검사":
                    PSROddProperty psr = new PSROddProperty();


                    //Common
                    psr.ThreshType = Convert.ToInt32(sl[1]);

                    //Core_RGB
                    psr.Core_Threshold = Convert.ToInt32(sl[2]);              
                    psr.Core_ExceptionThreshold = Convert.ToInt32(sl[3]);       
                    psr.Core_MinDefectSize = Convert.ToInt32(sl[4]);             

                    //Metal_채도
                    psr.Metal_LowerThreshold = Convert.ToInt32(sl[5]);          
                    psr.Metal_UpperThreshold = Convert.ToInt32(sl[6]);           
                    psr.Metal_MinDefectSize = Convert.ToInt32(sl[7]);         

                    //Circuit
                    psr.Circuit_Threshold = Convert.ToInt32(sl[8]);     
                    psr.Circuit_MinDefectSize = Convert.ToInt32(sl[9]);       

                    // Nomal
                    psr.Summation_range = Convert.ToInt32(sl[10]);             
                    psr.Summation_detection_size = Convert.ToInt32(sl[11]);    
                    psr.Mask_Threshold = Convert.ToInt32(sl[12]);              
                    psr.Mask_Extension = Convert.ToInt32(sl[13]);           
                    psr.Step_Threshold = Convert.ToInt32(sl[14]);             
                    psr.Step_Expansion = Convert.ToInt32(sl[15]);                 

                    //필터
                    psr.HV_ratio_value = Convert.ToInt32(sl[16]);          
                    psr.Min_Relative_size = Convert.ToInt32(sl[17]);         
                    psr.Max_Relative_size = Convert.ToInt32(sl[18]);
                    psr.Area_Relative = Convert.ToInt32(sl[19]);

                    it = new InspectionItem(InspectionType.GetInspectionType(sl[0]), psr, id);
                    break;


                case "GV 검사":
                    GV_Inspection_Property GV = new GV_Inspection_Property();

                    GV.Ball_Thresh = Convert.ToInt32(sl[1]);
                    GV.Core_Thresh = Convert.ToInt32(sl[2]);
                    GV.Mask = Convert.ToInt32(sl[3]);
                    GV.Taget_GV = Convert.ToInt32(sl[4]);
                    GV.GV_Margin = Convert.ToInt32(sl[5]);

                    it = new InspectionItem(InspectionType.GetInspectionType(sl[0]), GV, id);
                    break;

                default:
                    it = new InspectionItem();
                    break;
            }
            return it;
        }

        private string InspectItemToString(InspectionItem item)
        {
            string ret = item.InspectionType.Name + ",";
            StateAlignedMaskProperty p;
            switch (item.InspectionType.Name)
            {
                case "검사 제외":
                    ExceptionalMaskProperty e = item.InspectionAlgorithm as ExceptionalMaskProperty;
                    ret += e.ExceptionX.ToString() + ",";
                    ret += e.ExceptionY.ToString() + ",";
                    ret += e.UseShapeShift.ToString();
                    break;
                case "표면 검사":
                case "Core Crack 검사":               
                case "Burr 검사":
                    p = item.InspectionAlgorithm as StateAlignedMaskProperty;
                    ret += p.ThreshType.ToString() + ",";
                    ret += p.LowerThresh.ToString() + ",";
                    ret += p.UpperThresh.ToString() + ",";
                    ret += p.ApplyAverDiff.ToString() + ",";
                    ret += p.MaskLowerThresh.ToString() + ",";
                    ret += p.MaskUpperThresh.ToString() + ",";
                    ret += p.InspRange.ToString() + ",";
                    ret += p.AverMinMargin.ToString() + ",";
                    ret += p.AverMaxMargin.ToString() + ",";
                    ret += p.MinMargin.ToString() + ",";
                    ret += p.MaxMargin.ToString() + ",";
                    ret += p.ErosionTrainIter.ToString() + ",";
                    ret += p.DilationTrainIter.ToString() + ",";
                    ret += p.ErosionInspIter.ToString() + ",";
                    ret += p.DilationInspIter.ToString() + ",";
                    ret += p.MinDefectSize.ToString() + ",";
                    ret += p.MinSmallDefectSize.ToString() + ",";
                    ret += p.MinSmallDefectCount.ToString() + ",";
                    ret += p.MaxDefectSize.ToString() + ",";
                    ret += p.MaxSmallDefectSize.ToString() + ",";
                    ret += p.MaxSmallDefectCount.ToString();
                    break;
                case "원소재 검사":
                    RawMetrialProperty rm = item.InspectionAlgorithm as RawMetrialProperty;
                    ret += rm.ThreshType.ToString() + ",";
                    ret += rm.LowerThresh.ToString() + ",";
                    ret += rm.UpperThresh.ToString() + ",";
                    ret += rm.ApplyAverDiff.ToString() + ",";
                    ret += rm.MaskLowerThresh.ToString() + ",";
                    ret += rm.MaskUpperThresh.ToString() + ",";
                    ret += rm.InspRange.ToString() + ",";
                    ret += rm.AverMinMargin.ToString() + ",";
                    ret += rm.AverMaxMargin.ToString() + ",";
                    ret += rm.MinMargin.ToString() + ",";
                    ret += rm.MaxMargin.ToString() + ",";
                    ret += rm.ErosionTrainIter.ToString() + ",";
                    ret += rm.DilationTrainIter.ToString() + ",";
                    ret += rm.ErosionInspIter.ToString() + ",";
                    ret += rm.DilationInspIter.ToString() + ",";
                    ret += rm.MinDefectSize.ToString() + ",";
                    ret += rm.MinSmallDefectSize.ToString() + ",";
                    ret += rm.MinSmallDefectCount.ToString() + ",";
                    ret += rm.MaxDefectSize.ToString() + ",";
                    ret += rm.MaxSmallDefectSize.ToString() + ",";
                    ret += rm.MaxSmallDefectCount.ToString();
                    break;
                case "리드 형상 검사":
                    LeadShapeWithCenterLineProperty l = item.InspectionAlgorithm as LeadShapeWithCenterLineProperty;
                    ret += l.ThreshType.ToString() + ",";
                    ret += l.LowerThresh.ToString() + ",";
                    ret += l.UpperThresh.ToString() + ",";
                    ret += l.MinWidthRatio.ToString() + ",";
                    ret += l.MinWidthSize.ToString() + ",";
                    ret += l.MaxWidthRatio.ToString() + ",";
                    ret += l.MaxWidthSize.ToString() + ",";
                    ret += l.MinHeightsize.ToString() + ",";
                    ret += l.MinNormalRatio.ToString() + ",";
                    ret += l.MaxNormalRatio.ToString() + ",";
                    ret += l.RemoveTipSize.ToString();
                    break;
                case "공간 형상 검사":
                    SpaceShapeWithCenterLineProperty s = item.InspectionAlgorithm as SpaceShapeWithCenterLineProperty;
                    ret += s.ThreshType.ToString() + ",";
                    ret += s.LowerThresh.ToString() + ",";
                    ret += s.UpperThresh.ToString() + ",";
                    ret += s.MinWidthRatio.ToString() + ",";
                    ret += s.MinWidthSize.ToString() + ",";
                    ret += s.MaxWidthRatio.ToString() + ",";
                    ret += s.MaxWidthSize.ToString() + ",";
                    ret += s.MinHeightsize.ToString() + ",";
                    ret += s.MinNormalRatio.ToString() + ",";
                    ret += s.MaxNormalRatio.ToString() + ",";
                    ret += s.TipSearchSize.ToString();
                    break;
                case "Window Punch 검사":
                    WindowPunchProperty wp = item.InspectionAlgorithm as WindowPunchProperty;
                    ret += wp.ThreshType.ToString() + ",";
                    ret += wp.LowerThresh.ToString() + ",";
                    ret += wp.UpperThresh.ToString() + ",";
                    ret += wp.PsrMarginX.ToString() + ",";
                    ret += wp.PsrMarginY.ToString() + ",";
                    ret += wp.MinDefectSize.ToString() + ",";
                    ret += wp.MinHeightsize.ToString() + ",";
                    ret += wp.UsePSRShift.ToString() + ",";
                    ret += wp.UsePunchShift.ToString() + ",";
                    ret += wp.ErosionTrainIter.ToString();
                    break;
                case "인식키 검사" :
                    CrossPatternProperty cp = item.InspectionAlgorithm as CrossPatternProperty;
                    ret += cp.ThreshType.ToString() + ",";      
                    ret += cp.LowerThresh.ToString() + ",";    
                    ret += cp.UpperThresh.ToString() + ",";       
                    ret += cp.PsrMarginX.ToString() + ",";               
                    ret += cp.PsrMarginY.ToString() + ",";               
                    ret += cp.MinDefectSize.ToString() + ",";   
                    ret += cp.MinHeightsize.ToString() + ",";   
                    ret += cp.UsePSRShift.ToString() + ",";       
                    ret += cp.UsePSRShiftBA.ToString() + ",";       
                    ret += cp.UsePunchShift.ToString();       
                    break;
                case "Vent Hole 검사":
                    VentHoleProperty vh = item.InspectionAlgorithm as VentHoleProperty;
                    ret += vh.ThreshType.ToString() + ",";
                    ret += vh.LowerThresh.ToString() + ",";
                    ret += vh.MinDefectSize.ToString();
                    break;

                case "Vent Hole 검사2":
                    VentHoleProperty2 vh2 = item.InspectionAlgorithm as VentHoleProperty2;
                    ret += vh2.ThreshType.ToString() + ",";
                    ret += vh2.LowerThresh.ToString() + ",";
                    ret += vh2.MinDefectSize.ToString();
                    break;

                case "Ball 검사":
                    BallPatternProperty si = item.InspectionAlgorithm as BallPatternProperty;
                    ret += si.ThreshType.ToString() + ",";
                    ret += si.LowerThresh.ToString() + ",";
                    ret += si.UpperThresh.ToString() + ",";
                    ret += si.MaskLowerThresh.ToString() + ",";
                    ret += si.MaskUpperThresh.ToString() + ",";
                    ret += si.InspRange.ToString() + ",";
                    ret += si.ApplyAverDiff.ToString() + ",";
                    ret += si.AverMinMargin.ToString() + ",";
                    ret += si.AverMaxMargin.ToString() + ",";
                    ret += si.MinMargin.ToString() + ",";
                    ret += si.MaxMargin.ToString() + ",";

                    ret += si.ErosionTrainIter.ToString() + ",";
                    ret += si.DilationTrainIter.ToString() + ",";
                    ret += si.ErosionInspIter.ToString() + ",";
                    ret += si.DilationInspIter.ToString() + ",";
                    ret += si.MinDefectSize.ToString() + ",";

                    ret += si.MinSmallDefectSize.ToString() + ",";
                    ret += si.MinSmallDefectCount.ToString() + ",";
                    ret += si.MinWidthSize.ToString() + ",";
                    ret += si.MinHeightSize.ToString() + ",";
                    ret += si.MaxHeightSize.ToString() + ",";
                    ret += si.MaxDefectSize.ToString() + ",";
                    ret += si.MaxSmallDefectSize.ToString() + ",";
                    ret += si.MaxSmallDefectCount.ToString() + ",";
                    ret += si.UsePSRShift.ToString() + ",";
                    ret += si.UsePunchShift.ToString() + ",";
                    break;

                case "PSR 검사":
                    PSROddProperty psr = item.InspectionAlgorithm as PSROddProperty;

                    ret += psr.ThreshType.ToString() + ",";

                    //Core_RGB
                    ret += psr.Core_Threshold.ToString() + ",";
                    ret += psr.Core_ExceptionThreshold.ToString() + ",";
                    ret += psr.Core_MinDefectSize.ToString() + ",";

                    //Metal_채도
                    ret += psr.Metal_LowerThreshold.ToString() + ",";
                    ret += psr.Metal_UpperThreshold.ToString() + ",";
                    ret += psr.Metal_MinDefectSize.ToString() + ",";

                    //Circuit
                    ret += psr.Circuit_Threshold.ToString() + ",";
                    ret += psr.Circuit_MinDefectSize.ToString() + ",";

                    // Nomal
                    ret += psr.Summation_range.ToString() + ",";
                    ret += psr.Summation_detection_size.ToString() + ",";
                    ret += psr.Mask_Threshold.ToString() + ",";
                    ret += psr.Mask_Extension.ToString() + ",";
                    ret += psr.Step_Threshold.ToString() + ",";
                    ret += psr.Step_Expansion.ToString() + ",";

                    //필터
                    ret += psr.HV_ratio_value.ToString() + ",";
                    ret += psr.Min_Relative_size.ToString() + ",";
                    ret += psr.Max_Relative_size.ToString() + ",";
                    ret += psr.Area_Relative.ToString() + ",";
                    break;

                case "GV 검사":
                    GV_Inspection_Property GV = item.InspectionAlgorithm as GV_Inspection_Property;
                    ret += GV.Ball_Thresh.ToString() + ",";
                    ret += GV.Core_Thresh.ToString() + ",";
                    ret += GV.Mask.ToString() + ",";
                    ret += GV.Taget_GV.ToString() + ",";
                    ret += GV.GV_Margin.ToString();
                    break;
            }
            return ret;
        }
        #endregion

        #region Inspection Item Handling
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

        public void AddInspectionItem(InspectionType aSelectedItem)
        {
            
            lbInspection.SelectedIndex = -1;
            if (aSelectedItem != null && aSelectedItem.SettingControl != null)
            {
                if (aSelectedItem.InspectType == eVisInspectType.eInspTypeExceptionalMask)
                {
                    ExceptionalMask except = aSelectedItem.SettingControl as ExceptionalMask;
                }
                ((IInspectionTypeUICommands)aSelectedItem.SettingControl).SetDialog(aSelectedItem.Name, aSelectedItem.InspectType);
                try
                {
                    pnlInspectionSettings.Children.Clear();
                    pnlInspectionSettings.Children.Add(aSelectedItem.SettingControl);
                    btnSave.IsEnabled = true;
                    btnSetPreview.IsEnabled = true;
                    btnDelete.IsEnabled = true;
                    SaveInspectionItem();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        // 검사 설정 저장.
        public void SaveInspectionItem()
        {
            if (pnlInspectionSettings.Children.Count == 1)
            {
                IInspectionTypeUICommands inspectionSetting = (IInspectionTypeUICommands)pnlInspectionSettings.Children[0];
                InspectionItem selectedItem = null;
                selectedItem = lbInspection.SelectedItem as InspectionItem;

                if (selectedItem == null)
                    inspectionSetting.TryAdd(ref SelectedItem, -1);
                else
                    inspectionSetting.TryAdd(ref SelectedItem, selectedItem.ID);

                if (selectedItem == null)
                {
                    lbInspection.SelectedIndex = lbInspection.Items.Count - 1;
                    svInspectionList.ScrollToBottom();
                }
               // else SelectedItem.InspectionList.Add(selectedItem);
            }
        }

        // 검사 설정 삭제.
        private void DeleteInspectionItem()
        {
            try
            {
                SelectedItem.InspectionList.RemoveAt(lbInspection.SelectedIndex);

                int nSequenceID = 1;
                foreach (InspectionItem inspectionElement in SelectedItem.InspectionList)
                {
                    inspectionElement.ID = nSequenceID++;
                }
                btnSave.IsEnabled = false;
                btnSetPreview.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
            catch
            {
                Debug.WriteLine("Exception occured in btnDelete_Click(InspectionTypeCtrl.xaml.cs)");
            }
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
