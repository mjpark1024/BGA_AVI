using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Common;
using Common.Control;
using PCS.ELF.AVI;
using System.IO;
using PCS.ModelTeaching;
using PCS;
using HDSInspector.SubWindow;
using System.Diagnostics;

namespace HDSInspector.SubWindow
{
    /// <summary>
    /// MachineModel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MachineModel : Window
    {
        #region Selected Group, selected model.
        private Group m_SelectedGroup = null;

        private ModelInformation m_SelectedModel = null;

        public Group SelectedGroup { get { return m_SelectedGroup; } }
        public ModelInformation SelectedModel { get { return m_SelectedModel; } }
        #endregion


        #region Member variables.
        // Controller.
        private PCS.ELF.AVI.ModelManager m_ModelManager = new PCS.ELF.AVI.ModelManager();

        //public List<string> StoredGroupName { get { return m_listStoredGroupName; } }
        //public List<string> StoredModelName { get { return m_listStoredModelName; } }
        public ObservableCollection<IndexedName> StoredGroupName = new ObservableCollection<IndexedName>();
        public ObservableCollection<IndexedName> StoredModelName = new ObservableCollection<IndexedName>();
        private ObservableCollection<Common.MachineDBInfo> m_listStoredMachine = new ObservableCollection<Common.MachineDBInfo>();

        private ModelInformation m_NewModel = new ModelInformation();

        private string m_szCaptureGroupName = string.Empty;
        private string m_szCaptureModelName = string.Empty;
        private string m_szMachineType = string.Empty;

        public Common.MachineDBInfo SelectedMachine;
        public string SelectedGroupStr;
        private string SelectedModelStr;
        #endregion


        public MachineModel(string path)
        {
            InitializeComponent();
            InitializeDialog(path);
            InitializeEvent();
            
        }

        private void InitializeEvent()
        {
            this.lbGroup.SelectionChanged += lbGroup_SelectionChanged;
            this.lbModel.SelectionChanged += lbModel_SelectionChanged;
            this.lbMachine.SelectionChanged += lbMachine_SelectionChanged;

            this.lbGroup.PreviewMouseWheel += (s, e) => DoWheelAction(svGroup, e.Delta);
            this.lbModel.PreviewMouseWheel += (s, e) => DoWheelAction(svModel, e.Delta);
            this.lbMachine.PreviewMouseWheel += (s, e) => DoWheelAction(svModel, e.Delta);

            this.btnMC.Click += new RoutedEventHandler(btnMC_Click);
            this.btnClose.Click += new RoutedEventHandler(btnClose_Click);

            this.Loaded += new RoutedEventHandler(MachineModel_Loaded);
        }

        void btnMC_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        void lbMachine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedMachine = lbMachine.SelectedItem as Common.MachineDBInfo;
            if (SelectedMachine != null)
            {
                InitializeGroup(SelectedMachine.IP);
                lbGroup.DataContext = StoredGroupName;
                lbGroup.SelectedIndex = 0;
              //  lbGroup.Refresh();
            }
        }

        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        void lbModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedModelStr = (lbModel.SelectedItem as IndexedName).Name;
            if (SelectedModelStr != null)
            {
                txtModelName.Text = SelectedModelStr;
                LoadModel(SelectedMachine.IP, SelectedGroupStr, SelectedModelStr);
            }

        }

        void lbGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedGroupStr = (lbGroup.SelectedItem as IndexedName).Name;
            if (SelectedGroupStr != null)
            {
                InitializeModel(SelectedMachine.IP, SelectedGroupStr);
                lbModel.DataContext = StoredModelName;
                lbModel.SelectedIndex = 0;
               // lbModel.Refresh();
            }
        }

        void MachineModel_Loaded(object sender, RoutedEventArgs e)
        {
            m_ModelManager.CaptureModel = null;

            // 기존의 선택된 그룹, 모델이 있다면 그것을 가리키도록 한다.
           // InitializeSelectItem();
        }

        private void InitializeDialog(string path)
        {
            this.pnlModelSpec.IsEnabled = false;
            this.pnlModelSpec.DataContext = m_SelectedModel;

            lbGroup.DataContext = StoredGroupName;
            lbModel.DataContext = StoredModelName;
            InitializeMachines(path);
        }

        private void InitializeMachines(string path)
        {
            SettingMachines setting = new SettingMachines(path);
            m_listStoredMachine = setting.MachineInfo;
            lbMachine.DataContext = m_listStoredMachine;
        }

        private void InitializeGroup(string IP)
        {
            // 0. 사용자의 중복 입력 방지를 위해 그룹과 모델이름 목록을 읽어들인다.
           // StoredGroupName.Clear();
            StoredGroupName = PCS.ELF.AVI.ModelManager.LoadGroupNames(IP, MainWindow.Setting.General.DBIP);
            //
        }

        private void InitializeModel(string IP, string strGroup)
        {
          //  StoredModelName.Clear();
            StoredModelName = PCS.ELF.AVI.ModelManager.LoadUsedModelNames(strGroup, IP, MainWindow.Setting.General.DBIP);
            //
        }

        private void LoadModel(string IP, string strGroup, string strModel)
        {
            // 1. 그룹 정보를 읽어들인다.
            m_SelectedModel = PCS.ELF.AVI.ModelManager.LoadModel(strGroup, strModel, IP, MainWindow.Setting.General.DBIP, MainWindow.Setting.Job);
            this.pnlModelSpec.DataContext = m_SelectedModel;
        }

        private void DoWheelAction(ScrollViewer aScrollViewer, int anDeltaMovedValue)
        {
            double offset = anDeltaMovedValue / 3;

            if (offset > 0)
            {
                offset = (aScrollViewer.VerticalOffset - offset < 0) ? aScrollViewer.VerticalOffset : offset;
                aScrollViewer.ScrollToVerticalOffset(aScrollViewer.VerticalOffset - offset);
            }
            else
            {
                offset = (aScrollViewer.VerticalOffset - offset > aScrollViewer.ScrollableHeight) ? aScrollViewer.VerticalOffset - aScrollViewer.ScrollableHeight : offset;
                aScrollViewer.ScrollToVerticalOffset(aScrollViewer.VerticalOffset - offset);
            }
        }

        // 복사 붙여넣기 수행시 Teaching 데이터 또한 복제되도록 한다.
        private void CloneTeachingData(string aszMachineCode, string aszOriginModelName, Surface aSurface)
        {
            string szWorkTypeCode = WorkTypeCode.GetWorkTypeCode(aSurface);

            TeachingDataManager teachingDataManager = new TeachingDataManager();
            teachingDataManager.CopySectionDataToModel(aszMachineCode, aszOriginModelName, m_NewModel.Name, szWorkTypeCode);
            foreach (SectionInformation section in teachingDataManager.Sections)
            {
                teachingDataManager.CopyROIDataToModel(aszMachineCode, aszOriginModelName, m_NewModel.Name, szWorkTypeCode, section.Code);
            }
        }
    }
}
