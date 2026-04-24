using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using RVS.Generic;
using Common;
using System.Windows.Controls.DataVisualization.Charting;
using PCS.ELF.AVI;
using RVS.Generic.Insp;
using System.Collections.ObjectModel;
using PCS.ModelTeaching;
using RVS.Generic.Class;
using System.IO;
using System.Windows.Media;
using Common.DataBase;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HDSInspector
{
    public partial class InspectResultWindow : Window
    {
        #region Member Variables
        const int IMAGE_FRAME_COUNT = 60;

        private MainWindow m_ptrMainwindow;
        private InspectionResultDataControl m_TableData;

        private ObservableCollection<string> Groups = new ObservableCollection<string>();
        private ObservableCollection<string> Models = new ObservableCollection<string>();
        private ObservableCollection<string> Lots = new ObservableCollection<string>();

        private string Group;
        private string Model;
        private string Lot;

        private Image[] m_lstImage = new Image[60];
        private TextBlock[] m_lstLabel = new TextBlock[60];

        public List<NGImageData> m_ImageData = new List<NGImageData>();

        private int m_nPageCount = 0;
        private int m_nPageIndex = 0;
        private bool m_bShowAllImage = true;
        private string m_Path;
        #endregion

        #region Constructor
        public InspectResultWindow(InspectionResultDataControl aTableData, string strPath)
        {
            InitializeComponent();
            m_Path = strPath + "\\";
            InitializeDialog();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.lbGroup.SelectionChanged += lbGroup_SelectionChanged;
            this.lbModel.SelectionChanged += lbModel_SelectionChanged;
            this.lbLot.SelectionChanged += lbLot_SelectionChanged;

            this.svGroup.PreviewMouseWheel += svGroup_PreviewMouseWheel;
            this.svModel.PreviewMouseWheel += svModel_PreviewMouseWheel;
            this.svLot.PreviewMouseWheel += svLot_PreviewMouseWheel;

            this.btnOK.Click += btnOK_Click;
            this.btnApply.Click += btnApply_Click;
            this.sldPage.ValueChanged += sldPage_ValueChanged;
            this.KeyDown += InspectResultWindow_KeyDown;
            this.Loaded += InspectResultWindow_Loaded;
            this.Closing += new System.ComponentModel.CancelEventHandler(InspectResultWindow_Closing);

            this.btnPrev.Click += PagingButton_Click;
            this.btnNext.Click += PagingButton_Click;
            this.btnPrev10.Click += PagingButton_Click;
            this.btnNext10.Click += PagingButton_Click;
        }

        private void InitializeDialog()
        {
            rbAllImage.IsChecked = true;
            rbAllCamera.IsChecked = true;

            LoadGroups();
            lbGroup.DataContext = Groups;
            lbModel.DataContext = Models;
            lbLot.DataContext = Lots;

            if (MainWindow.Setting.General.MachineName != null)
                this.txtEquipName.Text = MainWindow.Setting.General.MachineName + "장비";

            SetControl();
        }
        #endregion

        #region Event Handler

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        void InspectResultWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            GC.Collect();
        }

        private void InspectResultWindow_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < lbGroup.Items.Count; i++)
            {
                Group group = lbGroup.Items[i] as Group;
                if (group != null && group.Name == MainWindow.CurrentGroup.Name)
                {
                    lbGroup.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < lbModel.Items.Count; i++)
            {
                ModelInformation model = lbModel.Items[i] as ModelInformation;
                if (model != null && model.Name == MainWindow.CurrentModel.Name)
                {
                    lbModel.SelectedIndex = i;
                    break;
                }
            }

            if (m_ptrMainwindow.InspectionMonitoringCtrl.ResultTable.txtLot != null && m_ptrMainwindow.InspectionMonitoringCtrl.ResultTable.txtLot.Text != "-")
            {
                for (int i = 0; i < lbLot.Items.Count; i++)
                {
                    string szLotName = lbLot.Items[i] as string;
                    if (!string.IsNullOrEmpty(szLotName) && szLotName == m_ptrMainwindow.InspectionMonitoringCtrl.ResultTable.txtLot.Text)
                    {
                        lbLot.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void InspectResultWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void svLot_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double offset = e.Delta / 3;

            if (offset > 0)
            {
                offset = (svLot.VerticalOffset - offset < 0) ? svLot.VerticalOffset : offset;
                this.svLot.ScrollToVerticalOffset(svLot.VerticalOffset - offset);
            }
            else
            {
                offset = (svLot.VerticalOffset - offset > svLot.ScrollableHeight) ? svLot.VerticalOffset - svLot.ScrollableHeight : offset;
                this.svLot.ScrollToVerticalOffset(svLot.VerticalOffset - offset);
            }
        }

        private void svModel_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double offset = e.Delta / 3;

            if (offset > 0)
            {
                offset = (svModel.VerticalOffset - offset < 0) ? svModel.VerticalOffset : offset;
                this.svModel.ScrollToVerticalOffset(svModel.VerticalOffset - offset);
            }
            else
            {
                offset = (svModel.VerticalOffset - offset > svModel.ScrollableHeight) ? svModel.VerticalOffset - svModel.ScrollableHeight : offset;
                this.svModel.ScrollToVerticalOffset(svModel.VerticalOffset - offset);
            }
        }

        private void svGroup_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double offset = e.Delta / 3;

            if (offset > 0)
            {
                offset = (svGroup.VerticalOffset - offset < 0) ? svGroup.VerticalOffset : offset;
                this.svGroup.ScrollToVerticalOffset(svGroup.VerticalOffset - offset);
            }
            else
            {
                offset = (svGroup.VerticalOffset - offset > svGroup.ScrollableHeight) ? svGroup.VerticalOffset - svGroup.ScrollableHeight : offset;
                this.svGroup.ScrollToVerticalOffset(svGroup.VerticalOffset - offset);
            }
        }

        private void lbGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Lots.Clear();
            Group = lbGroup.SelectedItem as string;
            if (Group != "")
            {
                LoadModels();
                lbModel.SelectedIndex = -1;

            }
        }

        private void lbModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model = lbModel.SelectedItem as string;
            if (Model != "")
            {
                LoadLots();
                lbLot.SelectedIndex = -1;
            }
        }

        private void lbLot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Lot = lbLot.SelectedItem as string;

        }

        private void sldPage_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_nPageCount == 0)
                return;

            int nPageIndex = Convert.ToInt32(sldPage.Value);
            m_nPageIndex = nPageIndex;
            ShowImage(nPageIndex, m_bShowAllImage);
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidSelectedIndex()) return;

            // progExport.Visibility = Visibility.Visible;
            LoadImage();
            // progExport.Visibility = Visibility.Hidden;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidSelectedIndex()) return;

            //progExport.Visibility = Visibility.Visible;
            // LoadLot();
            this.Cursor = Cursors.Wait;
            LoadData();
            this.Cursor = Cursors.Arrow;
            // progExport.Visibility = Visibility.Hidden;
        }

        private void PagingButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_nPageCount == 0)
                return;

            Button btn = sender as Button;
            int nPageIndex = m_nPageIndex;

            switch (btn.Name)
            {
                case "btnPrev":
                    nPageIndex--;
                    break;
                case "btnNext":
                    nPageIndex++;
                    break;
                case "btnPrev10":
                    nPageIndex -= 10;
                    break;
                case "btnNext10":
                    nPageIndex += 10;
                    break;
            }

            if (nPageIndex < 0)
            {
                nPageIndex = 0;
                m_nPageIndex = 0;
            }
            else if (nPageIndex >= m_nPageCount)
            {
                nPageIndex = m_nPageCount - 1;
                m_nPageIndex = m_nPageCount - 1;
            }

            sldPage.Value = nPageIndex;
        }
        #endregion

        #region Set Method
        public void SetMainwindow(MainWindow aMainwindow)
        {
            m_ptrMainwindow = aMainwindow;
        }

        public void SetDataContext(InspectionResultDataControl aTableData)
        {
            m_TableData = aTableData;
            this.DataContext = m_TableData;
            MainBar.DataContext = m_TableData;
        }
        #endregion

        #region Load Method
        public void LoadPieChartData(List<KeyValuePair<string, int>> aDefectDataList)
        {
            GraphGrid.Children.Remove(this.NGPieChart);
            GraphGrid.Children.Add(this.NGPieChart);
            ((PieSeries)NGPieChart.Series[0]).ItemsSource = aDefectDataList;
        }

        private void LoadData()
        {
            m_TableData = new InspectionResultDataControl(MainWindow.NG_Info);
            m_TableData.GroupName = Group;
            m_TableData.ModelName = Model;

            m_TableData = LoadLotData(MainWindow.Setting.General.ResultPath, Group, Model, Lot);

            m_TableData.LotNo = Lot;
            m_TableData.ModelName = Model;
            m_TableData.Operator = "";
            m_TableData.RunTime = m_TableData.EndTime.Subtract(m_TableData.StartTime);

            SetDataContext(m_TableData);
            LoadDefect(Model, m_TableData.LotNo);
            LoadImage();

            MainWindow.Log("PCS", SeverityLevel.INFO, String.Format("{0}의 Lot:{1} 결과 조회가 완료되었습니다.", Model, m_TableData.LotNo));
        }

        public void LoadDefect(string aszModelName, string aszLotNo)
        {
            //불량 갯수 테이블

            ObservableCollection<LotResultData> listDefectResultList = new ObservableCollection<LotResultData>();
            for (int n=1; n< m_TableData.Infos.Size; n++)
            {
                Bad_Info bad = m_TableData.Infos.GetItem(n);
                LotResultData data = new LotResultData();
                data.LotNo = aszLotNo;
                data.DefectName = bad.Name;
                data.DefectCount = m_TableData.UnitBad[n];
                listDefectResultList.Add(data);
            }
            #region
            /*
            if (m_TableData.Align > 0)
            {
                LotResultData data = new LotResultData();
                data.LotNo = aszLotNo;
                data.DefectName = "Align";
                data.DefectCount = m_TableData.Align;
                listDefectResultList.Add(data);

            }
            if (m_TableData.Raw > 0)
            {
                LotResultData data = new LotResultData();
                data.LotNo = aszLotNo;
                data.DefectName = "원소재";
                data.DefectCount = m_TableData.Raw;
                listDefectResultList.Add(data);

            }
            if (m_TableData.Open > 0)
            {
                LotResultData data = new LotResultData();
                data.LotNo = aszLotNo;
                data.DefectName = "Open";
                data.DefectCount = m_TableData.Open;
                listDefectResultList.Add(data);

            }
            if (m_TableData.Short > 0)
            {
                LotResultData data = new LotResultData();
                data.LotNo = aszLotNo;
                data.DefectName = "Short";
                data.DefectCount = m_TableData.Short;
                listDefectResultList.Add(data);

            }
            if (m_TableData.BP > 0)
            {
                LotResultData data = new LotResultData();
                data.LotNo = aszLotNo;
                data.DefectName = "본드패드";
                data.DefectCount = m_TableData.BP;
                listDefectResultList.Add(data);

            }
            if (m_TableData.Ball > 0)
            {
                LotResultData data = new LotResultData();
                data.LotNo = aszLotNo;
                data.DefectName = "Ball";
                data.DefectCount = m_TableData.Ball;
                listDefectResultList.Add(data);

            }
            if (m_TableData.PSR > 0)
            {
                LotResultData data = new LotResultData();
                data.LotNo = aszLotNo;
                data.DefectName = "PSR이물질";
                data.DefectCount = m_TableData.PSR;
                listDefectResultList.Add(data);

            }
            if (m_TableData.NonPSR > 0)
            {
                LotResultData data = new LotResultData();
                data.LotNo = aszLotNo;
                data.DefectName = "PSR핀홀";
                data.DefectCount = m_TableData.NonPSR;
                listDefectResultList.Add(data);

            }
            if (m_TableData.Crack > 0)
            {
                LotResultData data = new LotResultData();
                data.LotNo = aszLotNo;
                data.DefectName = "CoreCrack";
                data.DefectCount = m_TableData.Crack;
                listDefectResultList.Add(data);

            }
            if (m_TableData.Burr > 0)
            {
                LotResultData data = new LotResultData();
                data.LotNo = aszLotNo;
                data.DefectName = "Burr뭉침";
                data.DefectCount = m_TableData.Burr;
                listDefectResultList.Add(data);

            }
            if (m_TableData.Venthole > 0)
            {
                LotResultData data = new LotResultData();
                data.LotNo = aszLotNo;
                data.DefectName = "VentHole";
                data.DefectCount = m_TableData.Venthole;
                listDefectResultList.Add(data);

            }
            if (m_TableData.Via > 0)
            {
                LotResultData data = new LotResultData();
                data.LotNo = aszLotNo;
                data.DefectName = "Via 미충진";
                data.DefectCount = m_TableData.Via;
                listDefectResultList.Add(data);

            }
            */
            #endregion
            //파이 차트에 Set 해줄 데이터
            List<KeyValuePair<string, int>> aDefectDataList = new List<KeyValuePair<string, int>>();

            //불량 이미지 검색 체크박스 리스트
            NGCheckBoxListData NGcheckBox = new NGCheckBoxListData();
            NGcheckBox.IsNGChecked = true;
            NGcheckBox.NGName1 = "전체 불량";

            int nCount = listDefectResultList.Count;
            int i = 0;
            NGCountListData ngCount = null;
            foreach (LotResultData lotResultData in listDefectResultList)
            {
                if (i % 2 == 0)
                {
                    ngCount = new NGCountListData();
                    ngCount.NGName1 = lotResultData.DefectName;
                    ngCount.NGCnt1 = lotResultData.DefectCount;

                    aDefectDataList.Add(new KeyValuePair<string, int>(lotResultData.DefectName, lotResultData.DefectCount));

                    NGcheckBox = new NGCheckBoxListData();
                    NGcheckBox.IsNGChecked = false;
                    NGcheckBox.NGName1 = lotResultData.DefectName;
                }
                else
                {
                    if (ngCount != null)
                    {
                        ngCount.NGName2 = lotResultData.DefectName;
                        ngCount.NGCnt2 = lotResultData.DefectCount;

                        aDefectDataList.Add(new KeyValuePair<string, int>(lotResultData.DefectName, lotResultData.DefectCount));

                        NGcheckBox = new NGCheckBoxListData();
                        NGcheckBox.IsNGChecked = false;
                        NGcheckBox.NGName1 = lotResultData.DefectName;
                    }
                }

                i++;

            }

            LoadPieChartData(aDefectDataList);
        }

        private void LoadImage()
        {
            string szCameraType;
            string szBadType;

            #region Set CameraType
            if (rbBotSur1.IsChecked == true) szCameraType = "CAM11";
            else if (rbBotSur2.IsChecked == true) szCameraType = "CAM12";
            else if (rbBondPad1.IsChecked == true) szCameraType = "CAM21";
            else if (rbBondPad2.IsChecked == true) szCameraType = "CAM22";
            else if (rbTopSur1.IsChecked == true) szCameraType = "CAM31";
            else if (rbTopSur2.IsChecked == true) szCameraType = "CAM32";
            else szCameraType = "*";
            #endregion

            bool bRef = (bool)rbAllImage.IsChecked;

            #region Set NGType
            szBadType = "*";
            if ((bool)rbRaw.IsChecked) szBadType = "원소재";
            else if ((bool)rbBP.IsChecked) szBadType = "본드패드";
            else if ((bool)rbOpen.IsChecked) szBadType = "Open";
            else if ((bool)rbShort.IsChecked) szBadType = "Short";
            else if ((bool)rbBall.IsChecked) szBadType = "Ball";
            else if ((bool)rbNPSR.IsChecked) szBadType = "PSR핀홀";
            else if ((bool)rbPSR.IsChecked) szBadType = "PSR이물질";
            else if ((bool)rbBurr.IsChecked) szBadType = "Burr";
            else if ((bool)rbCrack.IsChecked) szBadType = "Crack";
            else if ((bool)rbVH.IsChecked) szBadType = "VentHole";
            else if ((bool)rbAlign.IsChecked) szBadType = "Align";
            else szBadType = "*";

            #endregion

            if (m_TableData != null)
                LoadImagePathAll(MainWindow.Setting.General.ResultPath, Group, m_TableData.ModelName, m_TableData.LotNo, szCameraType, szBadType);
            else
            {
                MessageBox.Show("작업 조회를 클릭해 주세요.");
            }
            if (rbAllImage.IsChecked == true)
                m_bShowAllImage = true;
            else
                m_bShowAllImage = false;

            #region Image Load
            GC.Collect();

            if (m_ImageData.Count > 0)
            {
                List<int> ImageIndexList = new List<int>();
                string szBeforePath = m_ImageData[0].NGImagePath;

                int nImageDataCount = m_ImageData.Count;
                if (m_bShowAllImage)
                    m_nPageCount = Convert.ToInt32(Math.Ceiling((double)nImageDataCount / (IMAGE_FRAME_COUNT / 2)));
                else
                    m_nPageCount = Convert.ToInt32(Math.Ceiling((double)nImageDataCount / (IMAGE_FRAME_COUNT)));

                sldPage.Maximum = m_nPageCount - 1;
                m_nPageIndex = 0;

                if (sldPage.Value != 0)
                    sldPage.Value = 0;
                ShowImage(m_nPageIndex, m_bShowAllImage);
            }
            else
            {
                tbPage.Text = "Page";
                m_nPageCount = 0;
                sldPage.Value = 0;
                ClearImage();
            }
            #endregion
        }
        #endregion

        #region Clear Method

        private void ClearImage()
        {
            for (int i = 0; i < IMAGE_FRAME_COUNT; i++)
            {
                m_lstImage[i].Source = null;
                m_lstLabel[i].Text = null;
            }
        }
        #endregion

        private void SetControl()
        {
            m_lstImage[0] = image01; m_lstImage[1] = image02; m_lstImage[2] = image03; m_lstImage[3] = image04; m_lstImage[4] = image05;
            m_lstImage[5] = image06; m_lstImage[6] = image07; m_lstImage[7] = image08; m_lstImage[8] = image09; m_lstImage[9] = image10;
            m_lstImage[10] = image11; m_lstImage[11] = image12; m_lstImage[12] = image13; m_lstImage[13] = image14; m_lstImage[14] = image15;
            m_lstImage[15] = image16; m_lstImage[16] = image17; m_lstImage[17] = image18; m_lstImage[18] = image19; m_lstImage[19] = image20;
            m_lstImage[20] = image21; m_lstImage[21] = image22; m_lstImage[22] = image23; m_lstImage[23] = image24; m_lstImage[24] = image25;
            m_lstImage[25] = image26; m_lstImage[26] = image27; m_lstImage[27] = image28; m_lstImage[28] = image29; m_lstImage[29] = image30;
            m_lstImage[30] = image31; m_lstImage[31] = image32; m_lstImage[32] = image33; m_lstImage[33] = image34; m_lstImage[34] = image35;
            m_lstImage[35] = image36; m_lstImage[36] = image37; m_lstImage[37] = image38; m_lstImage[38] = image39; m_lstImage[39] = image40;
            m_lstImage[40] = image41; m_lstImage[41] = image42; m_lstImage[42] = image43; m_lstImage[43] = image44; m_lstImage[44] = image45;
            m_lstImage[45] = image46; m_lstImage[46] = image47; m_lstImage[47] = image48; m_lstImage[48] = image49; m_lstImage[49] = image50;
            m_lstImage[50] = image51; m_lstImage[51] = image52; m_lstImage[52] = image53; m_lstImage[53] = image54; m_lstImage[54] = image55;
            m_lstImage[55] = image56; m_lstImage[56] = image57; m_lstImage[57] = image58; m_lstImage[58] = image59; m_lstImage[59] = image60;

            m_lstLabel[0] = label01; m_lstLabel[1] = label02; m_lstLabel[2] = label03; m_lstLabel[3] = label04; m_lstLabel[4] = label05;
            m_lstLabel[5] = label06; m_lstLabel[6] = label07; m_lstLabel[7] = label08; m_lstLabel[8] = label09; m_lstLabel[9] = label10;
            m_lstLabel[10] = label11; m_lstLabel[11] = label12; m_lstLabel[12] = label13; m_lstLabel[13] = label14; m_lstLabel[14] = label15;
            m_lstLabel[15] = label16; m_lstLabel[16] = label17; m_lstLabel[17] = label18; m_lstLabel[18] = label19; m_lstLabel[19] = label20;
            m_lstLabel[20] = label21; m_lstLabel[21] = label22; m_lstLabel[22] = label23; m_lstLabel[23] = label24; m_lstLabel[24] = label25;
            m_lstLabel[25] = label26; m_lstLabel[26] = label27; m_lstLabel[27] = label28; m_lstLabel[28] = label29; m_lstLabel[29] = label30;
            m_lstLabel[30] = label31; m_lstLabel[31] = label32; m_lstLabel[32] = label33; m_lstLabel[33] = label34; m_lstLabel[34] = label35;
            m_lstLabel[35] = label36; m_lstLabel[36] = label37; m_lstLabel[37] = label38; m_lstLabel[38] = label39; m_lstLabel[39] = label40;
            m_lstLabel[40] = label41; m_lstLabel[41] = label42; m_lstLabel[42] = label43; m_lstLabel[43] = label44; m_lstLabel[44] = label45;
            m_lstLabel[45] = label46; m_lstLabel[46] = label47; m_lstLabel[47] = label48; m_lstLabel[48] = label49; m_lstLabel[49] = label50;
            m_lstLabel[50] = label51; m_lstLabel[51] = label52; m_lstLabel[52] = label53; m_lstLabel[53] = label54; m_lstLabel[54] = label55;
            m_lstLabel[55] = label56; m_lstLabel[56] = label57; m_lstLabel[57] = label58; m_lstLabel[58] = label59; m_lstLabel[59] = label60;
        }

        private bool IsValidSelectedIndex()
        {
            if (lbModel.SelectedIndex < 0)
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("M066"), "Information");
                return false;
            }
            if (lbLot.SelectedIndex < 0)
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("M067"), "Information");
                return false;
            }

            return true;
        }

        private void ShowImage(int anPageIndex, bool abShowAllImage)
        {
            #region Page Set
            if (m_nPageCount == 0 || anPageIndex < 0 || anPageIndex >= m_nPageCount)
            {
                tbPage.Text = "Page";
                return;
            }
            tbPage.Text = String.Format("{0} / {1} Page", m_nPageIndex + 1, m_nPageCount);
            #endregion

            ClearImage();
            try
            {
                if (abShowAllImage)
                {
                    #region 불량 이미지 + 기준 이미지
                    #region Grid Setting
                    //ImageGrid.RowDefinitions[1].Height = new GridLength(1);
                    //ImageGrid.RowDefinitions[5].Height = new GridLength(3);
                    //ImageGrid.RowDefinitions[7].Height = new GridLength(1);

                    for (int i = 0; i < 10; i++)
                    {
                        m_lstLabel[i].Background = new SolidColorBrush(Colors.Gainsboro);
                        //  m_lstLabel[i].Height = 5.0;
                        //  m_lstLabel[i + 10].Height = 27.0;
                        m_lstLabel[i + 20].Background = new SolidColorBrush(Colors.Gainsboro);
                        //  m_lstLabel[i + 20].Height = 3.0;
                        //  m_lstLabel[i + 30].Height = 27.0;
                        m_lstLabel[i + 40].Background = new SolidColorBrush(Colors.Gainsboro);
                    }
                    #endregion

                    for (int i = 0; i < (IMAGE_FRAME_COUNT / 2); i++)
                    {
                        if ((i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))) >= m_ImageData.Count)
                            break;

                        #region 기준이미지
                        if (m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].StandardImagePath != "")
                        {
                            try
                            {
                                if (i < 10)
                                {
                                    m_lstImage[i].Source = LoadPng(m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].StandardImagePath);
                                    //m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].StandardImageSource;
                                    m_lstLabel[i].Text = "";
                                }
                                else if (i < 20)
                                {
                                    m_lstImage[i + 10].Source = LoadPng(m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].StandardImagePath);
                                    //m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].StandardImageSource;
                                    m_lstLabel[i + 10].Text = "";
                                }
                                else
                                {
                                    m_lstImage[i + 20].Source = LoadPng(m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].StandardImagePath);
                                    //m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].StandardImageSource;
                                    m_lstLabel[i + 20].Text = "";
                                }
                            }
                            catch
                            {
                                Debug.WriteLine("Out of Index: ShowImage() 기준이미지 Load 실패");
                                return;
                            }
                        }
                        #endregion

                        #region 불량이미지
                        if ((m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].NGImagePath != ""))
                        {
                            try
                            {
                                if (i < 10)
                                {
                                    m_lstImage[i + 10].Source = LoadPng(m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].NGImagePath);
                                    //m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].NGImageSource;
                                    m_lstLabel[i + 10].Text = GetSurface(m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].StripSurface) + "["
                                                            + QueryHelper.GetCode(m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].StripID + 1, 4) + "] x="
                                                            + (m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].UnitX + 1) + ", y="
                                                            + (m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].UnitY + 1) + " "
                                                            + m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].NGName + " size="
                                                            + m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].DefectSize;
                                }
                                else if (i < 20)
                                {
                                    m_lstImage[i + 20].Source = LoadPng(m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].NGImagePath);
                                    //m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].NGImageSource;
                                    m_lstLabel[i + 20].Text = GetSurface(m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].StripSurface) + "["
                                                            + QueryHelper.GetCode(m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].StripID + 1, 4) + "] x="
                                                            + (m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].UnitX + 1) + ", y="
                                                            + (m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].UnitY + 1) + " "
                                                            + m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].NGName + " size="
                                                            + m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].DefectSize;
                                }
                                else
                                {
                                    m_lstImage[i + 30].Source = LoadPng(m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].NGImagePath);
                                    //m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].NGImageSource;
                                    m_lstLabel[i + 30].Text = GetSurface(m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].StripSurface) + "["
                                                            + QueryHelper.GetCode(m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].StripID + 1, 4) + "] x="
                                                            + (m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].UnitX + 1) + ", y="
                                                            + (m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].UnitY + 1) + " "
                                                            + m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].NGName + " size="
                                                            + m_ImageData[i + (anPageIndex * (IMAGE_FRAME_COUNT / 2))].DefectSize;
                                }
                            }
                            catch
                            {
                                Debug.WriteLine("Out of Index: ShowImage() 불량이미지 Load 실패");
                                return;
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region 불량 이미지만 Show
                    #region Grid Setting
                    // ImageGrid.RowDefinitions[1].Height = new GridLength(27);
                    // ImageGrid.RowDefinitions[7].Height = new GridLength(3);
                    // ImageGrid.RowDefinitions[13].Height = new GridLength(27);

                    for (int i = 0; i < m_lstLabel.Length; i++)
                    {
                        m_lstLabel[i].Background = new SolidColorBrush(Color.FromRgb(71, 91, 126));
                        m_lstLabel[i].Height = 27.0;
                    }
                    #endregion

                    for (int i = 0; i < IMAGE_FRAME_COUNT; i++)
                    {
                        if ((i + (anPageIndex * IMAGE_FRAME_COUNT)) >= m_ImageData.Count)
                            break;

                        if (m_ImageData[i + (anPageIndex * IMAGE_FRAME_COUNT)].NGImagePath != "")
                        {
                            try
                            {
                                m_lstImage[i].Source = LoadPng(m_ImageData[i + (anPageIndex * IMAGE_FRAME_COUNT)].NGImagePath);
                                // m_ImageData[i + (anPageIndex * IMAGE_FRAME_COUNT)].NGImageSource;
                                m_lstLabel[i].Text = GetSurface(m_ImageData[i + (anPageIndex * IMAGE_FRAME_COUNT)].StripSurface) + "["
                                                            + QueryHelper.GetCode(m_ImageData[i + (anPageIndex * IMAGE_FRAME_COUNT)].StripID + 1, 4) + "] x="
                                                            + (m_ImageData[i + (anPageIndex * IMAGE_FRAME_COUNT)].UnitX + 1) + ", y="
                                                            + (m_ImageData[i + (anPageIndex * IMAGE_FRAME_COUNT)].UnitY + 1) + " "
                                                            + m_ImageData[i + (anPageIndex * IMAGE_FRAME_COUNT)].NGName + " size="
                                                            + m_ImageData[i + (anPageIndex * IMAGE_FRAME_COUNT)].DefectSize;
                            }
                            catch
                            {
                                Debug.WriteLine("Out of Index: ShowImage() 불량이미지 Load 실패");
                                return;
                            }
                        }
                    }
                    #endregion
                }
            }
            catch
            {
                Debug.WriteLine("검사결과 (불량 이미지 불러오기 실패)");
            }
        }

        private string GetSurface(Surface aSurface)
        {
            if (aSurface < Surface.CA1 && aSurface >= Surface.BP1) return "본드패드" + ((int)aSurface - (int)Surface.BP1 + 1);
            else if (aSurface < Surface.BA1 && aSurface >= Surface.CA1) return "CA외관" + ((int)aSurface - (int)Surface.CA1 + 1);
            else if (aSurface <= Surface.BA8 && aSurface >= Surface.BA1) return "BA외관" + ((int)aSurface - (int)Surface.BA1 + 1);
            else return "-";
        }

        private void LoadGroups()
        {
            Groups.Clear();
            string[] dirs = Directory.GetDirectories(m_Path);
            for (int i = 0; i < dirs.Length; i++)
            {
                Groups.Add(dirs[i].Remove(0, m_Path.Length));
            }

        }

        private void LoadModels()
        {
            Models.Clear();
            string path = m_Path + Group + "\\";
            string[] dirs = Directory.GetDirectories(path);
            for (int i = 0; i < dirs.Length; i++)
            {
                Models.Add(dirs[i].Remove(0, path.Length));
            }
        }

        private void LoadLots()
        {
            Lots.Clear();
            string path = m_Path + Group + "\\" + Model + "\\";
            string[] dirs = Directory.GetDirectories(path);
            for (int i = 0; i < dirs.Length; i++)
            {
                Lots.Add(dirs[i].Remove(0, path.Length));
            }
        }

        private InspectionResultDataControl LoadLotData(string strPath, string strGroup, string strModel, string strLotNo)
        {
            InspectionResultDataControl data = new InspectionResultDataControl(MainWindow.NG_Info);
            string strmodelPath = String.Format(@"{0}/{1}/{2}/{3}/", strPath, strGroup, strModel, strLotNo);
            if (Directory.Exists(strmodelPath))
            {
                string strFile = String.Format(@"{0}/{1}/{2}/{3}/Result.ini", strPath, strGroup, strModel, strLotNo);
                if (!File.Exists(strFile))
                {
                    System.Windows.MessageBox.Show("결과 파일을 찾을 수 없습니다.");
                    return null;
                }
                IniFile ini = new IniFile(strFile);
                data.StartTime = ini.ReadDateTime("결과정보", "시작시간", DateTime.Now);
                data.EndTime = ini.ReadDateTime("결과정보", "종료시간", DateTime.Now);
                data.InspectStrip = ini.ReadInteger("결과정보", "투입스트립", 0);
                data.TotalStrip = ini.ReadInteger("결과정보", "완성스트립", 0);
                data.GoodStrip = ini.ReadInteger("결과정보", "양품스트립", 0);
                data.BadStrip = ini.ReadInteger("결과정보", "불량스트립", 0);
                data.FailStrip = ini.ReadInteger("결과정보", "폐기스트립", 0);
                data.TotalUnits = ini.ReadInteger("결과정보", "전체유닛", 0);
                data.BadUnits = ini.ReadInteger("결과정보", "불량유닛", 0);

                data.CompletionUnits = ini.ReadInteger("결과정보", "양품유닛", 0);
      
                data.BeforeYield = ini.ReadDouble("결과정보", "Verify전수율", 0);
                data.Yield = ini.ReadDouble("결과정보", "Verify후수율", 0);

                for (int i = 1; i < MainWindow.NG_Info.Size; i++)
                {
                    Bad_Info bad = MainWindow.NG_Info.GetItem(i);
                    if (bad == null) continue;
                    data.UnitBad[i] = ini.ReadInteger("불량정보", bad.Name, 0);
                }
            }

            return data;
        }

        private BitmapSource LoadPng(string path)
        {
            if (File.Exists(path))
            {
                PngBitmapDecoder pngBitmapDecoder = new PngBitmapDecoder(new Uri(path), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                return pngBitmapDecoder.Frames[0];
            }
            return null;

        }

        public void LoadImagePathAll(string strPath, string strGroup, string strModel, string strLot, string strCam, string strBad)
        {
            string strmodelPath = String.Format(@"{0}/{1}/{2}/{3}/Image/", strPath, strGroup, strModel, strLot);
            // string[] files = Directory.GetDirectories(strmodelPath, null, SearchOption.TopDirectoryOnly);
            m_ImageData.Clear();
            int n = 0;
            foreach (string strFile in Directory.GetFiles(strmodelPath))
            {
                if (strFile.Contains(".png") && strFile.Contains("REF"))
                {

                    string[] arr = strFile.Remove(0, strmodelPath.Length).Split(' ');
                    string ng = arr[7].Replace(".png", "");
                    if (strCam != "*")
                        if (!arr[0].Contains(strCam))
                            continue;
                    if (strBad != "*")
                        if (strBad != ng) continue;
                    NGImageData ngData = new NGImageData();
                    ngData.NGName = ng;
                    ngData.UnitX = Convert.ToInt32(arr[2].Substring(2, 2));
                    ngData.UnitY = Convert.ToInt32(arr[3].Substring(2, 2));
                    ngData.DefectSize = arr[6].Replace("S=", "");
                    ngData.DefectCenterX = "0";
                    ngData.DefectCenterY = "0";
                    ngData.StripID = Convert.ToInt32(arr[1].Substring(3, 4));

                    ngData.StandardImagePath = strFile;
                    ngData.NGImagePath = strFile.Replace("REF", "DEF");
                    //if (File.Exists(ngData.StandardImagePath))
                    //{
                    //    PngBitmapDecoder pngBitmapDecoder = new PngBitmapDecoder(new Uri(ngData.StandardImagePath), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    //    ngData.StandardImageSource = pngBitmapDecoder.Frames[0];
                    //}
                    //if (File.Exists(ngData.NGImagePath))
                    //{
                    //    PngBitmapDecoder pngBitmapDecoder = new PngBitmapDecoder(new Uri(ngData.NGImagePath), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    //    ngData.NGImageSource = pngBitmapDecoder.Frames[0];
                    // }

                    if (arr[0] == "CAM11")
                    {
                        ngData.StripSurface = Surface.BA1;
                    }
                    else if (arr[0] == "CAM12")
                    {
                        ngData.StripSurface = Surface.BA2;
                    }
                    else if (arr[0] == "CAM21")
                    {
                        ngData.StripSurface = Surface.BP1;
                    }
                    else if (arr[0] == "CAM22")
                    {
                        ngData.StripSurface = Surface.BP2;
                    }
                    else if (arr[0] == "CAM31")
                    {
                        ngData.StripSurface = Surface.CA1;
                    }
                    else if (arr[0] == "CAM32")
                    {
                        ngData.StripSurface = Surface.CA2;
                    }
                    ngData.ImageIndex = n++;

                    m_ImageData.Add(ngData);
                }
            }
        }

    }
}
