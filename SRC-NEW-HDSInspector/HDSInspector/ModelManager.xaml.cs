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
 * @file  ModelManager.xaml.cs
 * @brief 
 *  Interaction logic for ModelManager.xaml.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.08.19
 * @version : 1.1
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.10 First creation.
 * - 2011.08.19 Added detail validate function.
 * - 2012.02.09 Re-factoring & some functions added.
 * - 2015.10.19 BGA AVI Creation.
 */

using Common;
using Common.Control;
using Common.Drawing.MarkingTypeUI;
using HDSInspector.SubWindow;
using IGS;
using PCS;
using PCS.ELF.AVI;
using PCS.ModelTeaching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

namespace HDSInspector
{
    public delegate void ModelManager_eventHandler_Send_PSR_Shift_Margin(int PSR_Shift_Margin);


    public class AutoNG_CheckBox_Item : INotifyPropertyChanged
    {
        private bool _isChecked;
        private bool _isEnabled = true;
        public string Name { get; set; }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public partial class ModelManager : Window
    {
        public static event ModelManager_eventHandler_Send_PSR_Shift_Margin Send_PSR_Shift_Margin_Event;

        public static int PSR_Shift_Margin = 0;

        #region Selected Group, selected model.
        private Group m_SelectedGroup = null;
        private Group m_BeforeSelectedGroup = null;

        private ModelInformation m_SelectedModel = null;
        private ModelInformation m_BeforeSelectedModel = null;

        public Group SelectedGroup              { get { return m_SelectedGroup; } }
        public ModelInformation SelectedModel   { get { return m_SelectedModel; } }
        #endregion

        #region Member variables.
        // Controller.
        private PCS.ELF.AVI.ModelManager m_ModelManager = new PCS.ELF.AVI.ModelManager();

        private List<string> m_listStoredGroupName = new List<string>();
        private List<string> m_listStoredModelName = new List<string>();

        private ModelInformation m_NewModel = new ModelInformation();

        private bool? m_bCreateActionFinished = true;
        private bool? m_bModifyActionFinished = true;

        private string m_szCaptureGroupName = string.Empty;
        private string m_szCaptureModelName = string.Empty;
        private string m_szMachineType = string.Empty;
        private string m_szCaptureMark = string.Empty;

        private static bool m_bTryStarted = false; // '시작' 요청이 있었는가에 대한 여부, Vision PC 상태 확인을 위한 용도로 쓰임. 2012-03-06 suoow2 added.
        #endregion
        RadioButton[] rdID = new RadioButton[3];
        RadioButton[] rdUnit = new RadioButton[10];
        RadioButton[] rdRail = new RadioButton[5];
        RadioButton[] rdCountLoc = new RadioButton[2];
        RadioButton[] rdCount = new RadioButton[5];
        RadioButton[] rdCountRW = new RadioButton[5];
        RadioButton[] rdWeek = new RadioButton[3];
        RadioButton[] rdWeekLoc = new RadioButton[3];

        //CheckBox[] cbAutoNG = new CheckBox[50];
        public ObservableCollection<AutoNG_CheckBox_Item> cb_autoNG_Items {  get; set; }
        

        private List<int> m_Index = new List<int>();
        private int m_SearchIndex = 0;

        #region Constructor.%
        public ModelManager(bool bLaserEnable) : this()
        {
            // bLaserEnable에 대한 처리. (TBD)
        }

        public ModelManager()
        {
            this.DataContext = this;            
            InitializeComponent();
            InitializeEvent();
            InitializeDialog();
            CreateContextMenu(); // 우클릭 팝업 메뉴를 구성한다.            
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        #endregion

        #region Initialize Window.
        private void InitializeEvent()
        {
            MarkRadioInit();
            
            this.lbGroup.SelectionChanged += lbGroup_SelectionChanged;
            this.lbModel.SelectionChanged += lbModel_SelectionChanged;

            this.lbGroup.PreviewMouseWheel += (s, e) => DoWheelAction(svGroup, e.Delta);
            this.lbModel.PreviewMouseWheel += (s, e) => DoWheelAction(svModel, e.Delta);

            this.txtBlock.LostFocus += txtBlock_LostFocus;
            this.txtUnitX.LostFocus += txtUnitX_LostFocus;
            this.txtUnitY.LostFocus += txtUnitX_LostFocus;

            this.btnSave.Click += btnSave_Click;
            this.btnCancel.Click += btnCancel_Click;
            this.btnClose.Click += btnClose_Click;

            this.btnCreateGroup.Click += btnGroup_Click;
            this.btnModifyGroup.Click += btnGroup_Click;
            this.btnDeleteGroup.Click += btnGroup_Click;

            this.btnRenameModel.Click += btnModel_Click;
            this.btnCreateModel.Click += btnModel_Click;
            this.btnModifyModel.Click += btnModel_Click;
            this.btnDeleteModel.Click += btnModel_Click;

            this.btnPLC.Click += btnPLC_Click;
            this.btnStart.Click += btnStart_Click;
            this.btnGoldenModel.Click += btnGoldenModel_Click;
            this.btnMC.Click += new RoutedEventHandler(btnMC_Click);

            this.btnRerunVision1.Click += btnRerunVision_Click;
            this.btnRerunVision2.Click += btnRerunVision_Click;
            this.btnRerunVision3.Click += btnRerunVision_Click;
            this.btnRerunVision4.Click += btnRerunVision_Click;
            this.btnRerunVision5.Click += btnRerunVision_Click;
            //this.btnRerunVision22.Click += btnRerunVision_Click;

            this.btnCalc1.Click += btnCalc1_Click;
            this.btnCalc2.Click += btnCalc2_Click;
            this.btn_AddList.Click += btn_AddListClick;
            this.btn_DeleteList.Click += btn_DeleteListClick;

            #region Mark RadioButton Click
            this.rdUnit0.Click += rdUnit_Click;
            this.rdUnit1.Click += rdUnit_Click;
            this.rdUnit2.Click += rdUnit_Click;
            this.rdUnit3.Click += rdUnit_Click;
            this.rdUnit4.Click += rdUnit_Click;
            this.rdUnit5.Click += rdUnit_Click;
            this.rdUnit6.Click += rdUnit_Click;
            this.rdUnit7.Click += rdUnit_Click;
            this.rdUnit8.Click += rdUnit_Click;
            this.rdUnit9.Click += rdUnit_Click;
            this.rdRail0.Click += rdRail_Click;
            this.rdRail1.Click += rdRail_Click;
            this.rdRail2.Click += rdRail_Click;
            this.rdRail3.Click += rdRail_Click;
            this.rdRail4.Click += rdRail_Click;
            this.rdNumber0.Click += rdNumber_Click;
            this.rdNumber1.Click += rdNumber_Click;
            this.rdNumber2.Click += rdNumber_Click;
            this.rdNumber3.Click += rdNumber_Click;
            this.rdNumber4.Click += rdNumber_Click;
            this.rdNumberLeft.Click += rdNumberLoc_Click;
            this.rdNumberRight.Click += rdNumberLoc_Click;
            this.rdReMark0.Click += rdReMark_Click;
            this.rdReMark1.Click += rdReMark_Click;
            this.rdReMark2.Click += rdReMark_Click;
            this.rdReMark3.Click += rdReMark_Click;
            this.rdReMark4.Click += rdReMark_Click;
            this.rdWeek0.Click += rdWeek_Click;
            this.rdWeek1.Click += rdWeek_Click;
            this.rdWeek2.Click += rdWeek_Click;
            this.rdIDNone.Click += rdIDNone_Click;
            this.rdIDLeft.Click += rdIDNone_Click;
            this.rdIDRight.Click += rdIDNone_Click;
            this.rdWeekLeft.Click += rdWeekLeft_Click;
            this.rdWeekRight.Click += rdWeekLeft_Click;
            this.rdWeekCenter.Click += rdWeekLeft_Click;

            this.btnMakeMark.Click += btnMakeMark_Click;
            #endregion

            this.KeyDown += ModelManager_KeyDown;
            this.Loaded += ModelManager_Loaded;

            this.txtSearchModelCode.GotFocus += TxtSearchModelCode_GotFocus;
            this.txtSearchModelCode.PreviewKeyDown += TxtSearchModelCode_PreviewKeyDown;
            this.btnPrevCode.Click += new RoutedEventHandler(btnPrevCode_Click);
            this.btnNextCode.Click += new RoutedEventHandler(btnNextCode_Click);
        }

        #region Model_Code_search Event
        private void ClearIndex()
        {
            m_Index.Clear();
            btnPrevCode.IsEnabled = false;
            btnNextCode.IsEnabled = false;
            m_SearchIndex = 0;
        }

        void SearchSelectModel()
        {
            if (m_Index[m_SearchIndex] < 0) return;
            lbModel.SelectedIndex = m_Index[m_SearchIndex];
        }

        void btnNextCode_Click(object sender, RoutedEventArgs e)
        {
            m_SearchIndex++;
            if (m_SearchIndex >= m_Index.Count) m_SearchIndex = 0;
            SearchSelectModel();
        }

        void btnPrevCode_Click(object sender, RoutedEventArgs e)
        {
            m_SearchIndex--;
            if (m_SearchIndex < 0) m_SearchIndex = m_Index.Count - 1;
            SearchSelectModel();
        }

        private void TxtSearchModelCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string strCode = txtSearchModelCode.Text.Trim();
                ClearIndex();
                for (int i = 0; i < m_ModelManager.Models.Count; i++)
                {
                    if (m_ModelManager.Models[i].Description == strCode)
                    {
                        m_Index.Add(i);
                    }
                }
                if (m_Index.Count >= 1)
                {
                    m_SearchIndex = 0;
                    if (m_Index.Count > 1)
                    {
                        btnPrevCode.IsEnabled = true;
                        btnNextCode.IsEnabled = true;
                    }
                    SearchSelectModel();
                }
                e.Handled = true;
            }
        }

        private void TxtSearchModelCode_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSearchModelCode.Clear();
        }
        #endregion Model_Code_search Event

        #region Mark RadoButton Ckick event
        void rdIDNone_Click(object sender, RoutedEventArgs e)
        {
            int nTag = Convert.ToInt32(((RadioButton)sender).Tag);
            rdID[nTag].IsChecked = true;
        }

        void rdWeekLeft_Click(object sender, RoutedEventArgs e)
        {
            int nTag = Convert.ToInt32(((RadioButton)sender).Tag);
            rdWeekLoc[0].IsChecked = false;
            rdWeekLoc[1].IsChecked = false;
            rdWeekLoc[2].IsChecked = false;
            rdWeekLoc[nTag].IsChecked = true;
        }

        void rdWeek_Click(object sender, RoutedEventArgs e)
        {
            int nTag = Convert.ToInt32(((RadioButton)sender).Tag);
            for (int i = 0; i < 3; i++) rdWeek[i].IsChecked = false;
            rdWeek[nTag].IsChecked = true;
        }
        void rdReMark_Click(object sender, RoutedEventArgs e)
        {
            int nTag = Convert.ToInt32(((RadioButton)sender).Tag);
            for (int i = 0; i < 5; i++) rdCountRW[i].IsChecked = false;
            rdCountRW[nTag].IsChecked = true;
        }
        void rdNumberLoc_Click(object sender, RoutedEventArgs e)
        {
            int nTag = Convert.ToInt32(((RadioButton)sender).Tag);
            rdCountLoc[0].IsChecked = false;
            rdCountLoc[1].IsChecked = false;
            rdCountLoc[nTag].IsChecked = true;
        }
        void rdNumber_Click(object sender, RoutedEventArgs e)
        {
            int nTag = Convert.ToInt32(((RadioButton)sender).Tag);
            for (int i = 0; i < 5; i++) rdCount[i].IsChecked = false;
            rdCount[nTag].IsChecked = true;
        }

        void rdRail_Click(object sender, RoutedEventArgs e)
        {
            int nTag = Convert.ToInt32(((RadioButton)sender).Tag);
            for (int i = 0; i < 5; i++) rdRail[i].IsChecked = false;
            rdRail[nTag].IsChecked = true;
        }

        void rdUnit_Click(object sender, RoutedEventArgs e)
        {
            int nTag = Convert.ToInt32(((RadioButton)sender).Tag);
            for (int i = 0; i < 10; i++) rdUnit[i].IsChecked = false;
            rdUnit[nTag].IsChecked = true;
        }
        #endregion Mark RadoButton Ckick event

        void btnMakeMark_Click(object sender, RoutedEventArgs e)
        {
            m_SelectedModel = lbModel.SelectedItem as ModelInformation;
            if (SelectedModel == null) return;
            if (MessageBox.Show("기존 마크 파일을 삭제하고 재 생성 합니다. 계속 진행 하시겠습니까?", "경고", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Common.Drawing.MarkingInformation.MarkLogo logo = new Common.Drawing.MarkingInformation.MarkLogo();
                Common.Drawing.MarkingInformation.Mark.m_bDebug = !MainWindow.Setting.SubSystem.PLC.UsePLC;
                if (!Common.Drawing.MarkingInformation.MarkInformation.CreateNewModel(SelectedModel.CopyToMarkInfo(),MainWindow.Setting.SubSystem.Laser.IP, logo))
                        MessageBox.Show("Mark1 : Mark File 생성에 실패 하였습니다.");
            }
        }

        void btnCalc2_Click(object sender, RoutedEventArgs e)
        {
            if (txtUnitX.Text != "0" && !string.IsNullOrEmpty(txtUnitX.Text) && txtUnitPitchX.Text != "0" && !string.IsNullOrEmpty(txtUnitPitchX.Text))
            {
                m_NewModel.Strip.StepUnits = (int)Math.Ceiling(Convert.ToInt32(txtUnitX.Text) / (double)m_NewModel.Strip.MarkStep);
                m_NewModel.Strip.StepPitch = m_NewModel.Strip.StepUnits * m_NewModel.Strip.UnitWidth + m_NewModel.Strip.BlockGap;
            }
        }

        void btnCalc1_Click(object sender, RoutedEventArgs e)
        {
            if (txtUnitX.Text != "0" && !string.IsNullOrEmpty(txtUnitX.Text) && txtUnitY.Text != "0" && !string.IsNullOrEmpty(txtUnitY.Text))
            {
                int val = (int)Math.Ceiling(m_NewModel.Strip.UnitColumn * m_NewModel.Strip.UnitRow * (MainWindow.Setting.General.RejectRate/100.0));
                m_NewModel.AutoNG = val + 1;
               // txtAutoNGCnt.Text = val.ToString("00");
            }
        }

        void btn_AddListClick(object sender, RoutedEventArgs e)
        {
            if (txtAutoNGMaxtrixX.Text != "0" && !string.IsNullOrEmpty(txtAutoNGMaxtrixX.Text) && txtAutoNGMaxtrixY.Text != "0" && !string.IsNullOrEmpty(txtAutoNGMaxtrixY.Text))
            {
                for (int i = 0; i < m_NewModel.p_ListMatrix.Count; i++)
                {
                    string str = txtAutoNGMaxtrixX.Text + ", " + txtAutoNGMaxtrixY.Text;
                    if (m_NewModel.p_ListMatrix[i] == txtAutoNGMaxtrixX.Text + ", " + txtAutoNGMaxtrixY.Text)
                    { return; }
                }
                m_NewModel.p_ListMatrix.Add(txtAutoNGMaxtrixX.Text + ", " + txtAutoNGMaxtrixY.Text);
            }
        }

        void btn_DeleteListClick(object sender, RoutedEventArgs e)
        {
            if (m_NewModel.p_nSelectedList > -1)
            {
                m_NewModel.p_ListMatrix.RemoveAt(m_NewModel.p_nSelectedList);
            }

            //if (txtAutoNGMaxtrixX.Text != "0" && !string.IsNullOrEmpty(txtAutoNGMaxtrixX.Text) && txtAutoNGMaxtrixY.Text != "0" && !string.IsNullOrEmpty(txtAutoNGMaxtrixY.Text))
            //{
            //    for (int i = 0; i < m_NewModel.p_ListMatrix.Count; i++)
            //    {
            //        string str = txtAutoNGMaxtrixX.Text + ", " + txtAutoNGMaxtrixY.Text;
            //        if (m_NewModel.p_ListMatrix[i] == txtAutoNGMaxtrixX.Text + ", " + txtAutoNGMaxtrixY.Text)
            //        { m_NewModel.p_ListMatrix.Remove(txtAutoNGMaxtrixX.Text + ", " + txtAutoNGMaxtrixY.Text); }
            //    }                
            //}
        }

        

        void txtUnitX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtUnitX.Text != "0" && !string.IsNullOrEmpty(txtUnitX.Text))
            {
                m_NewModel.Strip.StepUnits = (int)Math.Ceiling(Convert.ToInt32(txtUnitX.Text) / 3.0);
                m_NewModel.Strip.StepPitch = m_NewModel.Strip.StepUnits * m_NewModel.Strip.UnitWidth;
            }
        }

        void btnMC_Click(object sender, RoutedEventArgs e)
        {
            MachineModel mcDlg = new MachineModel("\\\\" + MainWindow.Setting.General.RVSIP +"\\d$\\bin\\MachineInfo.ini");
            if ((bool)mcDlg.ShowDialog())
            {
                Group targetGroup = lbGroup.Items[lbGroup.SelectedIndex] as Group;
                if (targetGroup != null && mcDlg.SelectedModel != null)
                {
                    m_szCaptureGroupName = targetGroup.Name;
                    m_szCaptureModelName = mcDlg.SelectedModel.Name;

                    m_ModelManager.CaptureModel = PCS.ELF.AVI.ModelManager.CreateCloneModel(mcDlg.SelectedModel);
                    m_ModelManager.CaptureModel.Name = string.Empty;
                    m_ModelManager.CaptureModel.Code = string.Empty;
                    EnableButtons();
                }
            }
        }

        private void BadFailSave()
        {
            for (int i = 0; i < cb_autoNG_Items.Count; i++)
            {    
                m_NewModel.ScrabInfo[i] = (bool)cb_autoNG_Items[i].IsChecked;
            }
        }

        private void MarkSave()
        {
            for (int i = 0; i < 10; i++)
            {
                if ((bool)rdUnit[i].IsChecked)
                {
                    m_NewModel.Marker.UnitMark = i;
                    break;
                }

            }
            for (int i = 0; i < 5; i++)
            {
                if ((bool)rdRail[i].IsChecked)
                {
                    m_NewModel.Marker.RailMark = i;
                    break;
                }
            }
            for (int i = 0; i < 5; i++)
            {
                if ((bool)rdCount[i].IsChecked)
                {
                    m_NewModel.Marker.NumMark = i;
                    break;
                }
            }
            for (int i = 0; i < 5; i++)
            {
                if ((bool)rdCountRW[i].IsChecked)
                {
                    m_NewModel.Marker.ReMark = i + 1;
                    break;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if ((bool)rdWeek[i].IsChecked)
                {
                    m_NewModel.Marker.WeekMark = i;
                    break;
                }
            }
            m_NewModel.Marker.WeekMarkType = cmbWeekMark.SelectedIndex;
            if ((bool)rdCountLoc[0].IsChecked)
                m_NewModel.Marker.NumLeft = true;
            else m_NewModel.Marker.NumLeft = false;

            if ((bool)rdWeekLoc[0].IsChecked)
            { m_NewModel.Marker.WeekPos = 0; }
            else if ((bool)rdWeekLoc[1].IsChecked)
            { m_NewModel.Marker.WeekPos = 1; }
            else
            { m_NewModel.Marker.WeekPos = 2; }
            for (int i = 0; i < 3; i++)
            {
                if ((bool)rdID[i].IsChecked)
                {
                    m_NewModel.Marker.IDMark = i;
                    break;
                }
            }
            m_NewModel.Marker.ID_Count = cbIDCount.SelectedIndex;
            if (m_NewModel.Marker.ID_Count < 0) m_NewModel.Marker.ID_Count = 0;
            m_NewModel.Marker.ID_Text = (bool)chkText.IsChecked;
        }

        private void BadFailSelection(ModelInformation selectedModel)
        {
            if (selectedModel != null)
            {
                for (int i = 0; i < cb_autoNG_Items.Count; i++)
                {
                    if (i < selectedModel.ScrabInfo.Length)
                        cb_autoNG_Items[i].IsChecked = (selectedModel.ScrabInfo[i]) ? true : false;
                    else cb_autoNG_Items[i].IsChecked = false;
                }
            }
        }

        private void MarkSeletion(ModelInformation selectedModel)
        {
            if (selectedModel == null) return;
            for (int i = 0; i < 10; i++)
            {
                rdUnit[i].IsChecked = false;
            }
            rdUnit[selectedModel.Marker.UnitMark].IsChecked = true;
            for (int i = 0; i < 5; i++)
            {
                rdRail[i].IsChecked = false;
                rdCount[i].IsChecked = false;
            }
            rdRail[selectedModel.Marker.RailMark].IsChecked = true;
            rdCount[selectedModel.Marker.NumMark].IsChecked = true;
            for (int i = 0; i < 5; i++)
            {
                rdCountRW[i].IsChecked = false;
            }
            if (selectedModel.Marker.ReMark == 0)
                rdCountRW[0].IsChecked = true;
            else
                rdCountRW[selectedModel.Marker.ReMark - 1].IsChecked = true;
            for (int i = 0; i < 3; i++)
            {
                rdWeek[i].IsChecked = false;
                rdWeekLoc[i].IsChecked = false;
            }
            rdWeek[selectedModel.Marker.WeekMark].IsChecked = true;
            for (int i = 0; i < 2; i++)
            {
                rdCountLoc[i].IsChecked = false;
            }
            if (selectedModel.Marker.NumLeft)
                rdCountLoc[0].IsChecked = true;
            else
                rdCountLoc[1].IsChecked = true;
            if (selectedModel.Marker.WeekPos == 0)
                rdWeekLoc[0].IsChecked = true;
            else if (selectedModel.Marker.WeekPos == 1)
                rdWeekLoc[1].IsChecked = true;
            else
                rdWeekLoc[2].IsChecked = true;
            rdID[selectedModel.Marker.IDMark].IsChecked = true;
            cbIDCount.SelectedIndex = selectedModel.Marker.ID_Count;
            chkText.IsChecked = selectedModel.Marker.ID_Text;
            cmbWeekMark.SelectedIndex = selectedModel.Marker.WeekMarkType;
        }

        private void MarkRadioInit()
        {
            rdUnit[0] = rdUnit0; rdUnit[1] = rdUnit1; rdUnit[2] = rdUnit2; rdUnit[3] = rdUnit3; rdUnit[4] = rdUnit4; rdUnit[5] = rdUnit5; rdUnit[6] = rdUnit6; rdUnit[7] = rdUnit7; rdUnit[8] = rdUnit8; rdUnit[9] = rdUnit9;
            rdRail[0] = rdRail0; rdRail[1] = rdRail1; rdRail[2] = rdRail2; rdRail[3] = rdRail3; rdRail[4] = rdRail4;
            rdCountLoc[0] = rdNumberLeft; rdCountLoc[1] = rdNumberRight;
            rdCount[0] = rdNumber0; rdCount[1] = rdNumber1; rdCount[2] = rdNumber2; rdCount[3] = rdNumber3; rdCount[4] = rdNumber4;
            rdCountRW[0] = rdReMark0; rdCountRW[1] = rdReMark1; rdCountRW[2] = rdReMark2; rdCountRW[3] = rdReMark3; rdCountRW[4] = rdReMark4;
            rdWeek[0] = rdWeek0; rdWeek[1] = rdWeek1; rdWeek[2] = rdWeek2;
            rdWeekLoc[0] = rdWeekLeft; rdWeekLoc[1] = rdWeekRight; rdWeekLoc[2] = rdWeekCenter;
            rdID[0] = rdIDNone; rdID[1] = rdIDLeft; rdID[2] = rdIDRight;

            cb_autoNG_Items = new ObservableCollection<AutoNG_CheckBox_Item>();
            AutoNG_List.DataContext = cb_autoNG_Items;


            //cbAutoNG[0] = cbAutoNG00; cbAutoNG[10] = cbAutoNG10; cbAutoNG[20] = cbAutoNG20; cbAutoNG[30] = cbAutoNG30; cbAutoNG[40] = cbAutoNG40;
            //cbAutoNG[1] = cbAutoNG01; cbAutoNG[11] = cbAutoNG11; cbAutoNG[21] = cbAutoNG21; cbAutoNG[31] = cbAutoNG31; cbAutoNG[41] = cbAutoNG41;
            //cbAutoNG[2] = cbAutoNG02; cbAutoNG[12] = cbAutoNG12; cbAutoNG[22] = cbAutoNG22; cbAutoNG[32] = cbAutoNG32; cbAutoNG[42] = cbAutoNG42;
            //cbAutoNG[3] = cbAutoNG03; cbAutoNG[13] = cbAutoNG13; cbAutoNG[23] = cbAutoNG23; cbAutoNG[33] = cbAutoNG33; cbAutoNG[43] = cbAutoNG43;
            //cbAutoNG[4] = cbAutoNG04; cbAutoNG[14] = cbAutoNG14; cbAutoNG[24] = cbAutoNG24; cbAutoNG[34] = cbAutoNG34; cbAutoNG[44] = cbAutoNG44;
            //cbAutoNG[5] = cbAutoNG05; cbAutoNG[15] = cbAutoNG15; cbAutoNG[25] = cbAutoNG25; cbAutoNG[35] = cbAutoNG35; cbAutoNG[45] = cbAutoNG45;
            //cbAutoNG[6] = cbAutoNG06; cbAutoNG[16] = cbAutoNG16; cbAutoNG[26] = cbAutoNG26; cbAutoNG[36] = cbAutoNG36; cbAutoNG[46] = cbAutoNG46;
            //cbAutoNG[7] = cbAutoNG07; cbAutoNG[17] = cbAutoNG17; cbAutoNG[27] = cbAutoNG27; cbAutoNG[37] = cbAutoNG37; cbAutoNG[47] = cbAutoNG47;
            //cbAutoNG[8] = cbAutoNG08; cbAutoNG[18] = cbAutoNG18; cbAutoNG[28] = cbAutoNG28; cbAutoNG[38] = cbAutoNG38; cbAutoNG[48] = cbAutoNG48;
            //cbAutoNG[9] = cbAutoNG09; cbAutoNG[19] = cbAutoNG19; cbAutoNG[29] = cbAutoNG29; cbAutoNG[39] = cbAutoNG39; cbAutoNG[49] = cbAutoNG49;

            //for (int i = 0; i < cbAutoNG.Length; i++)
            //{
            //    if (i < MainWindow.NG_Info.Size-1)
            //    {
            //        string name = MainWindow.NG_Info.GetBadName(i + 1);
            //        cbAutoNG[i].Content = name;
            //        if (name == "고정불량" || name == "연속불량")
            //        {
            //            cbAutoNG[i].IsEnabled = false;
            //            cbAutoNG[i].IsChecked = false;
            //        }
            //    }
            //    else
            //    {
            //        cbAutoNG[i].Visibility = Visibility.Hidden;
            //        cbAutoNG[i].IsChecked = false;
            //    }
            //}       


            for (int i = 1; i< MainWindow.NG_Info.Size; i++ )
            {
                string name = MainWindow.NG_Info.GetBadName(i);
                bool isChecked = m_NewModel.ScrabInfo[i];
                bool isEnabled = true;
                
                if (name == "고정불량" || name == "연속불량")
                {
                    isChecked = false;
                    isEnabled = false;
                }

                cb_autoNG_Items.Add(new AutoNG_CheckBox_Item
                {
                    Name = name,
                    IsChecked = isChecked,
                    IsEnabled = isEnabled
                });
            }

        }
       
        private void InitializeDialog()
        {
            CreateCheckBoxes();
            m_szMachineType = PCS.ELF.AVI.ModelManager.GetMachineType(MainWindow.Setting.General.MachineCode);

            this.grdMark.IsEnabled = false;
            this.pnlModelSpec.IsEnabled = this.AutoNG_Control.IsEnabled= false;
            this.grdMark.DataContext = m_NewModel;
            this.pnlModelSpec.DataContext = m_NewModel;
            this.btnMakeMark.IsEnabled = true;
            this.UseDualLaser.IsEnabled = MainWindow.Setting.SubSystem.Laser.DualLaser;
            lbGroup.DataContext = m_ModelManager.Groups;
            lbModel.DataContext = m_ModelManager.Models;

            // PLC 사용여부에 따라 Cam Infomation을 표시하거나 표시하지않는다.
            // PLC 사용여부에 따라 Vision State Infomation을 표시하거나 표시하지않는다.
            if (MainWindow.Setting.SubSystem.PLC.UsePLC)
            {
                CamInfo.Visibility = Visibility.Hidden;
                VisionInfo.Visibility = Visibility.Visible;

                if (m_bTryStarted)
                {
                    MainWindow ptrMainWindow = Application.Current.MainWindow as MainWindow;
                    if (ptrMainWindow != null && ptrMainWindow.PCSInstance.Vision != null)
                    {
                        try
                        {
                            for(int i = 0; i< MainWindow.m_nTotalScanCount; i++)
                            {
                                MainWindow.IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(i);
                                if (IndexInfo.CategorySurface == CategorySurface.CA)
                                { if (!MainWindow.Setting.SubSystem.IS.UseCASlave && IndexInfo.Slave) continue; }
                                else if (IndexInfo.CategorySurface == CategorySurface.BA)
                                { if (!MainWindow.Setting.SubSystem.IS.UseBASlave && IndexInfo.Slave) continue; }
                                ptrMainWindow.PCSInstance.Vision[i].SendPacket(VisionDefinition.PING);
                            }
                        }
                        catch { }

                        ////현재 사용 안함
                        //his.circVision1State.Background = (ptrMainWindow.PCSInstance.Vision[VID.BP1].Connected) ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                        //his.circVision2State.Background = (ptrMainWindow.PCSInstance.Vision[VID.CA1].Connected) ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                        //his.circVision3State.Background = (ptrMainWindow.PCSInstance.Vision[VID.CA1 + 1].Connected) ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                        //his.circVision4State.Background = (ptrMainWindow.PCSInstance.Vision[VID.BA1].Connected) ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                        //his.circVision5State.Background = (ptrMainWindow.PCSInstance.Vision[VID.BA1 + 1].Connected) ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                        ////
                        //if (!MainWindow.Setting.SubSystem.IS.UseCASlave)
                        //{
                        //    this.circVision3State.Visibility = Visibility.Hidden;
                        //    this.btnRerunVision3.Visibility = Visibility.Hidden;
                        //}
                        //if (!MainWindow.Setting.SubSystem.IS.UseBASlave)
                        //{
                        //    this.circVision5State.Visibility = Visibility.Hidden;
                        //    this.btnRerunVision3.Visibility = Visibility.Hidden;
                        //}
                    }
                }
            }
            else
            {
                CamInfo.Visibility = Visibility.Visible;
                VisionInfo.Visibility = Visibility.Hidden;

                txtCamWidth.Text = MainWindow.Setting.SubSystem.IS.CameraWidth[0].ToString();
                txtCamHeight.Text = MainWindow.Setting.SubSystem.IS.CameraHeight[0].ToString();
                txtCamFile.Text = MainWindow.Setting.SubSystem.IS.CamFile[0];
                switch (MainWindow.Setting.SubSystem.IS.TestID)
                {
                    case 0:
                        rbBot.IsChecked = false;
                        rbTop.IsChecked = true;
                        rbTra.IsChecked = false;
                        break;
                    case 1:
                        rbBot.IsChecked = true;
                        rbTop.IsChecked = false;
                        rbTra.IsChecked = false;
                        break;
                    case 2:
                        rbBot.IsChecked = false;
                        rbTop.IsChecked = false;
                        rbTra.IsChecked = true;
                        break;
                }
            }
            InitializeGroupAndModel();
        }

        private void InitializeGroupAndModel()
        {
            // 0. 사용자의 중복 입력 방지를 위해 그룹과 모델이름 목록을 읽어들인다.
            PCS.ELF.AVI.ModelManager.LoadGroupNames(ref m_listStoredGroupName);
            PCS.ELF.AVI.ModelManager.LoadUsedModelNames(ref m_listStoredModelName);

            int nLastSelectedGroup = Math.Max(MainWindow.Setting.Job.LastGroup, 0);
            string nLastSelectedModel = MainWindow.Setting.Job.LastModel;

            // 1. 그룹 정보를 읽어들인다.
            m_ModelManager.LoadGroup();
            if (m_ModelManager.Groups.Count > 0)
            {
                foreach (Group group in m_ModelManager.Groups)
                {
                    // 2. 새로운 그룹에 대한 이미지 저장 경로 생성.
                    DirectoryManager.CreateDirectory(DirectoryManager.GetGroupImagePath(MainWindow.Setting.General.ModelPath, group.Name));
                }

                if (nLastSelectedGroup >= 0)
                {
                    if (nLastSelectedGroup > m_ModelManager.Groups.Count - 1)
                        nLastSelectedGroup = m_ModelManager.Groups.Count - 1;
                    lbGroup.SelectedIndex = nLastSelectedGroup;
                                        
                    // 3. 모델 정보를 읽어들인다.
                    m_ModelManager.LoadModel(m_ModelManager.Groups[nLastSelectedGroup].Name);
                    if (m_ModelManager.Models.Count > 0)
                    {
                        foreach (ModelInformation model in m_ModelManager.Models)
                        {
                            // 4. 새로운 모델에 대한 이미지 저장 경로를 생성한다.
                            DirectoryManager.CreateDirectory(DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, m_ModelManager.Groups[nLastSelectedGroup].Name, model.Name));
                        }

                        if (nLastSelectedModel != "")
                        {
                            foreach (ModelInformation model in m_ModelManager.Models)
                            {
                                if (model.Name == nLastSelectedModel)
                                {
                                    m_NewModel = model;
                                    lbModel.SelectedIndex = model.Index - 1;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            lbModel.SelectedIndex = -1;
                        }

                       // m_NewModel = lbModel.SelectedItem as ModelInformation;
                        
                        this.pnlModelSpec.DataContext = m_NewModel;
                        //this.pnlMarking.DataContext = m_NewModel;
                    }
                }
            }
        }
        private void CreateCheckBoxes()
        {
            // 열 정의 (XAML의 Grid.Column에 해당)
            for (int i = 0; i < MainWindow.Setting.Job.BPCount; i++) CheckBoxBPContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            for (int i = 0; i < MainWindow.Setting.Job.CACount; i++) CheckBoxCAContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            for (int i = 0; i < MainWindow.Setting.Job.BACount; i++) CheckBoxBAContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // BP 체크박스 생성
            List<MainWindow.CheckBoxData> listBPdataSource = new List<MainWindow.CheckBoxData>();
            List<CheckBox> listBPcheckBoxes = new List<CheckBox>();
            for (int i = 1; i <= MainWindow.Setting.Job.BPCount; i++)
            {
                listBPdataSource.Add(new MainWindow.CheckBoxData
                {
                    Name = CategorySurface.BP.ToString() + i,
                    Content = "본드패드" + i,
                    BindingPath = "UseBondPad" + i,
                    IsChecked = true,
                    IsEnable = true
                });
            }

            for (int i = 0; i < listBPdataSource.Count; i++)
            {
                var data = listBPdataSource[i];
                CheckBox checkBox = new CheckBox
                {
                    Name = data.Name,
                    Content = data.Content,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 5, 0, 0),
                    IsChecked = (i < VID.BP_PC_Count),
                    IsEnabled = (i < VID.BP_PC_Count) 
                };

                CheckBoxEvent(i, VID.BP_PC_Count, checkBox, listBPcheckBoxes);
                // 바인딩 설정
                Binding binding = new Binding(data.BindingPath)
                {
                    Mode = BindingMode.TwoWay
                };
                checkBox.SetBinding(CheckBox.IsCheckedProperty, binding);
                // Grid 위치 설정
                Grid.SetRow(checkBox, i);
                CheckBoxBPContainer.Children.Add(checkBox);
                listBPcheckBoxes.Add(checkBox);
            }

            // CA 체크박스 생성
            List<MainWindow.CheckBoxData> listCAdataSource = new List<MainWindow.CheckBoxData>();
            List<CheckBox> listCAcheckBoxes = new List<CheckBox>();
            for (int i = 1; i <= MainWindow.Setting.Job.CACount; i++)
            {
                listCAdataSource.Add(new MainWindow.CheckBoxData
                {
                    Name = CategorySurface.CA.ToString() + i,
                    Content = "CA외관" + i,
                    BindingPath = "UseTopSur" + i,
                    IsChecked = true,
                    IsEnable = true
                });
            }

            for (int i = 0; i < listCAdataSource.Count; i++)
            {
                var data = listCAdataSource[i];
                CheckBox checkBox = new CheckBox
                {
                    Name = data.Name,
                    Content = data.Content,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 5, 0, 0),
                    IsChecked = (i < VID.CA_PC_Count), // 첫번째 체크박스 활성화
                    IsEnabled = (i < VID.CA_PC_Count)  // 첫번째 체크박스 활성화
                };

                CheckBoxEvent(i, VID.CA_PC_Count, checkBox, listCAcheckBoxes);
                // 바인딩 설정
                Binding binding = new Binding(data.BindingPath)
                {
                    Mode = BindingMode.TwoWay
                };
                checkBox.SetBinding(CheckBox.IsCheckedProperty, binding);
                // Grid 위치 설정
                Grid.SetRow(checkBox, 0);
                Grid.SetColumn(checkBox, i);
                CheckBoxCAContainer.Children.Add(checkBox);
                listCAcheckBoxes.Add(checkBox);
            }

            // BA 체크박스 생성
            List<MainWindow.CheckBoxData> listBAdataSource = new List<MainWindow.CheckBoxData>();
            List<CheckBox> litsBAcheckBoxes = new List<CheckBox>();
            for (int i = 1; i <= MainWindow.Setting.Job.BACount; i++)
            {
                listBAdataSource.Add(new MainWindow.CheckBoxData
                {
                    Name = CategorySurface.BA.ToString() + i,
                    Content = "BA외관" + i,
                    BindingPath = "UseBotSur" + i,
                    IsChecked = true,
                    IsEnable = true
                });
            }

            for (int i = 0; i < listBAdataSource.Count; i++)
            {
                var data = listBAdataSource[i];
                CheckBox checkBox = new CheckBox
                {
                    Name = data.Name,
                    Content = data.Content,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 5, 0, 0),
                    IsChecked = (i < VID.BA_PC_Count), // 첫번째 체크박스 활성화
                    IsEnabled = (i < VID.BA_PC_Count)  // 첫번째 체크박스 활성화
                };

                CheckBoxEvent(i, VID.BA_PC_Count, checkBox, litsBAcheckBoxes);
                // 바인딩 설정
                Binding binding = new Binding(data.BindingPath)
                {
                    Mode = BindingMode.TwoWay
                };
                checkBox.SetBinding(CheckBox.IsCheckedProperty, binding);
   
                // Grid 위치 설정
                Grid.SetRow(checkBox, 0);
                Grid.SetColumn(checkBox, i);
                CheckBoxBAContainer.Children.Add(checkBox);
                litsBAcheckBoxes.Add(checkBox);
            }
        }

        private void CheckBoxEvent(int i, int nPCCount, CheckBox checkBox, List<CheckBox> ListCheckBox)
        {
            bool bClick = false;
            if (i >= 0)
            {
                // 마우스 클릭 이벤트로 상태 제어
                checkBox.PreviewMouseLeftButtonDown += (s, e) =>
                {
                    bClick = true;
                    if (i >= nPCCount)
                    {
                        CheckBox clickedCheckBox = s as CheckBox;
                        int index = ListCheckBox.IndexOf(clickedCheckBox);
                   
                        if (index > 0 && !(ListCheckBox[index - 1].IsChecked ?? false))
                        {
                            e.Handled = true; // 클릭 무시
                        }
                        else if (!(ListCheckBox[0].IsChecked ?? false))
                        {  e.Handled = true; }
                    }
                };            

                // 체크 상태 변경 이벤트 핸들러
                checkBox.Checked += (s, e) =>
                {
                    CheckBox checkedBox = s as CheckBox;
                    int index = ListCheckBox.IndexOf(checkedBox);
                    if (index < 0) return;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (index == 0)
                        {
                            if (index + 2 < ListCheckBox.Count)
                            {
                                if (ListCheckBox[index + 1].IsEnabled)
                                {
                                    ListCheckBox[index + 2].IsEnabled = true;
                                }
                            }
                        }
                        else
                        {
                            if (index + 1 < ListCheckBox.Count)
                            {
                                ListCheckBox[index + 1].IsEnabled = true;
                            }
                        }
                    }));
                };

                checkBox.Unchecked += (s, e) =>
                {
                    if (!bClick) return;
                    CheckBox uncheckedBox = s as CheckBox;
                    int index = ListCheckBox.IndexOf(uncheckedBox);
                    if(index < 0) return;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (index == 0)
                        {
                            while (index + 2 < ListCheckBox.Count)
                            {
                                ListCheckBox[index + 2].IsEnabled = false;
                                ListCheckBox[index + 2].IsChecked = false;
                                index++;
                            }
                        }

                        else
                        {
                            while (index + 1 < ListCheckBox.Count)
                            {
                                ListCheckBox[index + 1].IsEnabled = false;
                                ListCheckBox[index + 1].IsChecked = false;
                                index++;
                            }
                        }
                    }));
                };
            }
        }

        private void ModelManager_Loaded(object sender, RoutedEventArgs e)
        {
            m_ModelManager.CaptureModel = null;
            EnableButtons();

            // 기존의 선택된 그룹, 모델이 있다면 그것을 가리키도록 한다.
            InitializeSelectItem();
        }

        private void InitializeSelectItem()
        {
            MainWindow ptrMainWindow = Application.Current.MainWindow as MainWindow;
            if (ptrMainWindow != null)
            {
                this.lbGroup.SelectionChanged -= lbGroup_SelectionChanged;
                this.lbModel.SelectionChanged -= lbModel_SelectionChanged;

                int groupIndex = MainWindow.Setting.Job.LastGroup;
                string modeName = MainWindow.Setting.Job.LastModel;
                int modelIndex = -1;
                foreach (ModelInformation model in m_ModelManager.Models)
                {
                    if (model.Name == modeName)
                    {
                        modelIndex = model.Index - 1;
                        break;
                    }
                }

                if (groupIndex >= lbGroup.Items.Count)
                {
                    groupIndex = -1;
                }

                if (modelIndex >= lbModel.Items.Count)
                {
                    modelIndex = -1;
                }

                lbGroup.SelectedIndex = groupIndex;
                lbModel.SelectedIndex = modelIndex;

                if (groupIndex >= 0 && modelIndex >= 0)
                {
                    Group selectedGroup = lbGroup.SelectedItem as Group;
                    ModelInformation selectedModel = lbModel.SelectedItem as ModelInformation;
                    PSR_Shift_Margin = selectedModel.Strip.PSRMarginX;
                    

                    if (selectedGroup != null && selectedModel != null)
                    {
                        // 기준영상이 존재하는 경우 보여준다.
                        string imagePath = DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, selectedGroup.Name, selectedModel.Name);
                        TryShowBasedImage(imagePath);

                        this.txtModelName.Text = selectedModel.Name;
                        this.pnlModelSpec.DataContext = selectedModel;
                        this.grdMark.DataContext = selectedModel;
                        MarkSeletion(selectedModel);
                        BadFailSelection(selectedModel);
                    }
                    else
                    {
                        this.imgBase.Source = null;
                        this.txtModelName.Text = "선택된 모델이 없습니다.";
                        this.pnlModelSpec.DataContext = new ModelInformation();
                    }
                }
                else
                {
                    this.imgBase.Source = null;
                    this.txtModelName.Text = "선택된 모델이 없습니다.";
                    this.pnlModelSpec.DataContext = new ModelInformation();
                }

                this.lbGroup.SelectionChanged += lbGroup_SelectionChanged;
                this.lbModel.SelectionChanged += lbModel_SelectionChanged;
                //AddModelList(ref ptrMainWindow.InspectionMonitoringCtrl.ResultTable.ModelList.lstGM);

            }
        }

        private void ModelManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_bModifyActionFinished == false && e.Key == Key.Escape)
            {
                m_bCreateActionFinished = true;
                m_bModifyActionFinished = true;

                EnableButtons();
                ChangeModel();
            }

            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        private void AddModelList(ref List<SubControl.GroupAndModel> lstGM)
        {
            lstGM.Clear();
            foreach (Group group in m_ModelManager.Groups)
            {
                SubControl.GroupAndModel mg = new SubControl.GroupAndModel();
                mg.Group = group.Name;
                mg.Model = m_ModelManager.LoadModels(group.Name);
                lstGM.Add(mg);
            }
        }

        private void CreateContextMenu()
        {
            // 분류에 대한 ContextMenu를 생성.
            List<ContextMenuObject> listGroupContextMenuObject = new List<ContextMenuObject>();
            listGroupContextMenuObject.Add(new ContextMenuObject() { m_objTag = GroupOperation.Create, m_bHasEvent = true, m_nParentID = 0, m_strImageUri = "./Images/add_64_64.png", m_strName = "분류 추가" });
            listGroupContextMenuObject.Add(new ContextMenuObject() { m_objTag = GroupOperation.Modify, m_bHasEvent = true, m_nParentID = 0, m_strImageUri = "./Images/modify_128_128.png", m_strName = "분류 수정" });
            listGroupContextMenuObject.Add(new ContextMenuObject() { m_objTag = GroupOperation.Delete, m_bHasEvent = true, m_nParentID = 0, m_strImageUri = "./Images/remove_64_64.png", m_strName = "분류 삭제" });

            ContextMenuHelper groupContextMenu = new ContextMenuHelper(GroupContextMenuClicked);
            lbGroup.ContextMenu = groupContextMenu.CreateMenu(listGroupContextMenuObject);

            // 모델에 대한 ContextMenu를 생성.
            List<ContextMenuObject> listModelContextMenuObject = new List<ContextMenuObject>();
            listModelContextMenuObject.Add(new ContextMenuObject() { m_objTag = ModelOperation.Create, m_bHasEvent = true, m_nParentID = 0, m_strImageUri = "./Images/add_64_64.png", m_strName = "모델 추가" });
            listModelContextMenuObject.Add(new ContextMenuObject() { m_objTag = ModelOperation.Modify, m_bHasEvent = true, m_nParentID = 0, m_strImageUri = "./Images/modify_128_128.png", m_strName = "모델 수정" });
            listModelContextMenuObject.Add(new ContextMenuObject() { m_objTag = ModelOperation.Delete, m_bHasEvent = true, m_nParentID = 0, m_strImageUri = "./Images/remove_64_64.png", m_strName = "모델 삭제" });
            listModelContextMenuObject.Add(new ContextMenuObject() { m_objTag = ModelOperation.Copy, m_bHasEvent = true, m_nParentID = 0, m_strImageUri = null, m_strName = "모델 복사" });
            listModelContextMenuObject.Add(new ContextMenuObject() { m_objTag = ModelOperation.Paste, m_bHasEvent = true, m_nParentID = 0, m_strImageUri = null, m_strName = "모델 붙여넣기" });
            
            ContextMenuHelper modelContextMenu = new ContextMenuHelper(ModelContextMenuClicked);
            lbModel.ContextMenu = modelContextMenu.CreateMenu(listModelContextMenuObject);
        }
        #endregion

        #region Listbox selection changed events.
        private void lbGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_bCreateActionFinished == false || m_bModifyActionFinished == false)
            {
                m_bCreateActionFinished = true;
                m_bModifyActionFinished = true;

                EnableButtons();

                // M015 : 변경된 사항이 있습니다. 저장하고 다른 페이지를 탐색하시겠습니까?
                if (MessageBox.Show(ResourceStringHelper.GetInformationMessage("M015"), "Information", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SaveModel();
                }
            }

            Group selectedGroup = lbGroup.SelectedItem as Group;
            if (selectedGroup != null)
            {
                m_ModelManager.LoadModel(selectedGroup.Name);
                txtGroupName.Text = selectedGroup.Name;
                if (m_ModelManager.Models.Count > 0)
                {
                    string nLastSelectedModel = MainWindow.Setting.Job.LastModel;
                    this.lbModel.DataContext = m_ModelManager.Models;
                    if (nLastSelectedModel != "")
                    {
                        foreach (ModelInformation model in m_ModelManager.Models)
                        {
                            if (model.Name == nLastSelectedModel)
                            {
                                m_NewModel = model;
                                lbModel.SelectedIndex = model.Index - 1;
                                break;
                            }
                        }
                    }
                    else
                    {
                        lbModel.SelectedIndex = 0;
                    }
                   // this.lbModel.SelectedIndex = 0;
                }
            }
        }

        private void lbModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_bCreateActionFinished == false || m_bModifyActionFinished == false)
            {
                m_bCreateActionFinished = true;
                m_bModifyActionFinished = true;

                EnableButtons();

                // M015 : 변경된 사항이 있습니다. 저장하고 다른 페이지를 탐색하시겠습니까?
                if (MessageBox.Show(ResourceStringHelper.GetInformationMessage("M015"), "Information", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SaveModel();
                }
            }

            ModelInformation selectedModel = lbModel.SelectedItem as ModelInformation;
            if (selectedModel == null)
            {
                if (lbModel.HasItems)
                {
                    lbModel.SelectedIndex = 0;
                }
            }

            ChangeModel();
        }
        #endregion

        #region Event handlers.

        void btnGoldenModel_Click(object sender, RoutedEventArgs e)
        {
            GoldenLoadWindow golden = new GoldenLoadWindow(m_szMachineType);
            if ((bool)golden.ShowDialog())
            {
                InitializeGroupAndModel();

                string szGroupName = PCS.ELF.AVI.ModelManager.GetGroupName(golden.txtModelName.Text);

                foreach (Group group in m_ModelManager.Groups)
                {
                    if (szGroupName == group.Name)
                    {
                        lbGroup.SelectedIndex = group.Index - 1;
                        break;
                    }
                }

                m_ModelManager.LoadModel(szGroupName);
                foreach (ModelInformation model in m_ModelManager.Models)
                {
                    if (golden.txtModelName.Text == model.Name)
                    {
                        lbModel.SelectedIndex = model.Index - 1;
                        break;
                    }
                }
            }
        }

        private void btnPLC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                m_SelectedModel = lbModel.SelectedItem as ModelInformation;
                if (m_SelectedModel != null)
                {
                    MainWindow ptrMainWindow = Application.Current.MainWindow as MainWindow;
                    if(ptrMainWindow != null)
                    {
                        double cam1pitch = MainWindow.Setting.Job.Cam1Pitch + m_SelectedModel.XPitch;
                        if (m_SelectedModel.Strip.UnitRow % 2 == 1)
                        {
                            cam1pitch = MainWindow.Setting.Job.Cam1Pitch - (m_SelectedModel.Strip.UnitHeight / 2.0) + m_SelectedModel.XPitch;
                        }

                        if (MainWindow.Setting.SubSystem.PLC.MCType == 1)
                            cam1pitch += MainWindow.Setting.Job.Cam1Position;

                        int nLeft = -1;
                        if (MainWindow.Setting.General.UseIDReader)
                        {
                            nLeft = 0;
                            if (m_SelectedModel.ITS.UseITS)
                            {
                                if (m_SelectedModel.ITS.LeftID) nLeft = 1;
                                else nLeft = 2;
                            }
                        }
                        int nScanVelocity1 = MainWindow.Setting.Job.ScanVelocity1;
                        int nScanVelocity2 = MainWindow.Setting.Job.ScanVelocity2;
                        double thick = -0.1;

                        int TOP_COUNT = 0; int BOT_COUNT = 0;
                        //////// 택타임 이슈 발생시 사용하자.
                        CheckScanSkip(out TOP_COUNT, out BOT_COUNT);

                        //int TotalCount = 0;
                        //if (MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count > MainWindow.Setting.Job.CACount) TotalCount = MainWindow.Setting.Job.BPCount * 2;
                        //else TotalCount = MainWindow.Setting.Job.CACount;
                        //int TOP_COUNT = TotalCount;
                        //int BOT_COUNT = MainWindow.Setting.Job.BACount;
                        //////////
                        //현장적용 업데이트 후 삭제할 것
                        ////이후 Setting 으로 변경할 것, 오입력 시 PLC 원점복귀 안됨
                        //if(MainWindow.Setting.General.MachineName == "BAV11" || MainWindow.Setting.General.MachineName == "BAV12" || MainWindow.Setting.General.MachineName == "BAV13")
                        //{
                        //    BOT_COUNT = 1;
                        //}
                        if (MainWindow.Setting.SubSystem.PLC.MCType == 1)

                        {
                            nScanVelocity1 = nScanVelocity2 = 200;
                        }

                        if(MainWindow.Setting.SubSystem.IS.UseFocus)
                        {
                            thick = m_SelectedModel.Strip.Thickness;
                        }

                        MarkingBoatShift markingBoatShift = new MarkingBoatShift();
                        markingBoatShift.Offset1 = m_SelectedModel.Strip.MarkShift1;
                        markingBoatShift.Offset2 = m_SelectedModel.Strip.MarkShift2;

                        ptrMainWindow.SendModelInfo(m_SelectedModel.Strip.Height, m_SelectedModel.Strip.Width, nScanVelocity1, nScanVelocity2,
                                                      MainWindow.Setting.Job.Cam1Position, cam1pitch,
                                                      m_SelectedModel.Strip.MarkStep, m_SelectedModel.Strip.StepPitch,
                                                      m_SelectedModel.Marker.BoatPos1, m_SelectedModel.Marker.BoatPos2, m_SelectedModel.Marker.CamPosY, 0, thick, TOP_COUNT, BOT_COUNT, nLeft
                                                      , MainWindow.Setting.SubSystem.PLC.MCType, markingBoatShift);

                        ptrMainWindow.pTopScanCount = TOP_COUNT;
                        ptrMainWindow.pBotScanCount = BOT_COUNT;
                    }
                }
                else
                {
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M066"), "Information");
                }
            }
            catch
            {
                MessageBox.Show("PLC Error", "Error");
                MainWindow.Log("PCS", SeverityLevel.FATAL, "PLC로 모델 정보를 전송하는데 실패하였습니다.", true);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Send_PSR_Shift_Margin_Event?.Invoke(PSR_Shift_Margin);

            //////// 택타임 이슈 발생시 사용하자.
            int TempTop = 0; ; int Tempbot = 0;
            CheckScanSkip(out TempTop, out Tempbot);
            MainWindow ptrMainWindow = Application.Current.MainWindow as MainWindow;
            if ((ptrMainWindow.pTopScanCount != TempTop || ptrMainWindow.pBotScanCount != Tempbot) && MainWindow.Setting.SubSystem.PLC.UsePLC)
            {
                MessageBox.Show("PLC입력값과 검사Skip이 다름. 다시 PLC 전송하세요.", "Information");
                return;
            }

            if (m_bCreateActionFinished == false || m_bModifyActionFinished == false)
            {
                m_bCreateActionFinished = true;
                m_bModifyActionFinished = true;
                SaveModel();
            }

            m_SelectedGroup = lbGroup.SelectedItem as Group;
            m_SelectedModel = lbModel.SelectedItem as ModelInformation;

            //2D 데이터 수정
            if (!string.IsNullOrEmpty(tbHistoryID.Text))
                MainWindow.historyID = Convert.ToInt32(tbHistoryID.Text);

            Array.Copy(m_SelectedModel.ScrabInfo, MainWindow.AutoNG_Check, m_SelectedModel.ScrabInfo.Length);
            string s = "";
            for (int i = 0; i < m_SelectedModel.ScrabInfo.Length; i++)
            {
                if (MainWindow.NG_Info.GetBadName(i+1) == "외곽불량")
                    MainWindow.UseOuter = MainWindow.AutoNG_Check[i];
                if (MainWindow.AutoNG_Check[i]) s += "1,";
                else s += "0,";
            }
            MainWindow.Setting.Job.AutoNG = s;

            MainWindow.Setting.Job.LastGroup = lbGroup.SelectedIndex;
            if (m_SelectedModel != null)
                MainWindow.Setting.Job.LastModel = m_SelectedModel.Name;
            MainWindow.Setting.Job.Save();

            if (!MainWindow.Setting.SubSystem.PLC.UsePLC)
            {
                MainWindow.Setting.SubSystem.IS.CameraWidth[0] = Convert.ToInt32(txtCamWidth.Text);
                MainWindow.Setting.SubSystem.IS.CameraHeight[0] = Convert.ToInt32(txtCamHeight.Text);
                MainWindow.Setting.SubSystem.IS.CamFile[0] = txtCamFile.Text;
                if ((bool)rbTop.IsChecked) MainWindow.Setting.SubSystem.IS.TestID = VID.BP1;
                if ((bool)rbBot.IsChecked) MainWindow.Setting.SubSystem.IS.TestID = VID.CA1;
                if ((bool)rbTra.IsChecked) MainWindow.Setting.SubSystem.IS.TestID = VID.BA1;
                MainWindow.Setting.SubSystem.Save();
                
            }
            MainWindow.Setting.Load();

            if (m_SelectedModel != null && m_SelectedGroup != null)
            {
                PCS.ELF.AVI.ModelManager.SelectedGroup = m_SelectedGroup;
                PCS.ELF.AVI.ModelManager.SelectedModel = m_SelectedModel;
                m_bTryStarted = true;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                // M066 : 모델을 선택해 주시기 바랍니다.
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("M066"), "Information");
            }
        }
        private void txtBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtBlock.Text == "1" && !string.IsNullOrEmpty(txtWidth.Text))
            {
                txtBlockGap.Text = "0";
            }
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            m_bCreateActionFinished = true;
            m_bModifyActionFinished = true;

            EnableButtons();
            SaveModel();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            m_bCreateActionFinished = true;
            m_bModifyActionFinished = true;

            EnableButtons();

            ChangeModel();
        }
        #endregion

        #region Group & Model management functions.
        private void GroupManagement(GroupOperation operationType)
        {
            switch (operationType)
            {
                case GroupOperation.Create:
                    this.CreateGroup();
                    break;
                case GroupOperation.Modify:
                    //if (m_ModelManager.Models.Count > 0)
                    //{
                    //    MessageBox.Show("모델 정보가 있는 그룹은 수정할 수 없습니다.\n모델 정보 삭제 후 수정해 주십시오", "Information");
                    //    break;
                   // }
                    this.ModifyGroup();
                    break;
                case GroupOperation.Delete:
                    this.DeleteGroup();
                    break;
                default:
                    break;
            }
        }

        private void ModelManagement(ModelOperation operationType)
        {
            switch (operationType)
            {
                case ModelOperation.Create:
                    m_szCaptureGroupName = m_szCaptureModelName = string.Empty;
                    this.CreateModel();
                    break;
                case ModelOperation.Modify:
                    m_szCaptureGroupName = m_szCaptureModelName = string.Empty;
                    this.ModifyModel();
                    break;
                case ModelOperation.Delete:
                    m_szCaptureGroupName = m_szCaptureModelName = string.Empty;
                    this.DeleteModel();
                    break;
                case ModelOperation.Copy:
                    m_szCaptureGroupName = m_szCaptureModelName = string.Empty;
                    this.CopyModel();
                    break;
                case ModelOperation.Paste:
                    this.PasteModel();
                    break;
                case ModelOperation.Rename:
                    this.RenameModel();
                    break;
                default:
                    break;
            }
        }

        private void GroupContextMenuClicked(object sender, RoutedEventArgs e)
        {
            MenuItem clickedMenuItem = sender as MenuItem;
            if (clickedMenuItem == null)
            {
                return;
            }

            GroupManagement((GroupOperation)clickedMenuItem.Tag);
            e.Handled = true;
        }

        private void ModelContextMenuClicked(object sender, RoutedEventArgs e)
        {
            MenuItem clickedMenuItem = sender as MenuItem;
            if (clickedMenuItem == null)
            {
                return;
            }

            ModelManagement((ModelOperation)clickedMenuItem.Tag);
            e.Handled = true;
        }

        private void btnGroup_Click(object sender, RoutedEventArgs e)
        {
            Button selectedMenu = sender as Button;
            if (selectedMenu == null)
            {
                return;
            }

            GroupManagement((GroupOperation)Enum.Parse(typeof(GroupOperation), selectedMenu.Tag.ToString()));
        }

        private void btnModel_Click(object sender, RoutedEventArgs e)
        {
            Button selectedMenu = sender as Button;
            if (selectedMenu == null)
            {
                return;
            }

            ModelManagement((ModelOperation)Enum.Parse(typeof(ModelOperation), selectedMenu.Tag.ToString()));
        }
        #endregion

        #region Create & Modify & Delete Group.
        /// <summary>   Creates the group. </summary>
        /// <remarks>   suoow2, 2014-08-10. </remarks>
        private void CreateGroup()
        {
            string strGroupName = GetInputMessage("분류 추가", "새로운 분류 이름을 입력해 주십시오.",
                                                  m_listStoredGroupName);

            if (!string.IsNullOrEmpty(strGroupName))
            {
                if (m_ModelManager.CreateGroup(strGroupName))
                {
                    // 새로운 그룹에 대한 이미지 저장 경로 생성.
                    DirectoryManager.CreateDirectory(DirectoryManager.GetGroupImagePath(MainWindow.Setting.General.ModelPath, strGroupName));

                    m_listStoredGroupName.Add(strGroupName);
                    this.lbGroup.SelectedIndex = this.lbGroup.Items.Count - 1;
                    this.svGroup.ScrollToVerticalOffset(this.svGroup.MaxHeight);

                    if (imgBase.Source != null)
                    {
                        imgBase.Source = null;
                    }

                    // M027 : 분류가 생성되었습니다.
                    // MessageBox.Show(ResourceStringHelper.GetInformationMessage("M027"), "Information");
                }
                else
                {
                    // M053 : 분류 생성에 실패하였습니다. 다시 시도해 주시기 바랍니다.
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M053"), "Database Error");
                }
            }
        }

        /// <summary>   Modify group. </summary>
        /// <remarks>   suoow2, 2014-08-10. </remarks>
        private void ModifyGroup()
        {
            Group selectedGroup = lbGroup.SelectedItem as Group;
            if (selectedGroup == null)
            {
                // M023 : 수정하려는 분류를 선택해 주시기 바랍니다.
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("M025"), "Information");
                return;
            }
            

            int nSelectedGroupIndex = selectedGroup.Index;
            string strOldGroupName = selectedGroup.Name;
            string strNewGroupName = GetInputMessage("분류 수정", "변경할 분류 이름을 입력해 주십시오.",
                                                                m_listStoredGroupName, selectedGroup.Name);

            if (!string.IsNullOrEmpty(strNewGroupName))
            {
                m_listStoredGroupName.Remove(selectedGroup.Name);
                m_listStoredGroupName.Add(strNewGroupName);

                if (m_ModelManager.ModifyGroup(nSelectedGroupIndex, strNewGroupName))
                {
                    //if (
                    m_ModelManager.RenameGroup(strOldGroupName, strNewGroupName);//)
                    //{
                        // 분류명 변경에 따른 디렉토리 이동 작업을 수행한다.
                        DirectoryManager.MoveDirectory(DirectoryManager.GetGroupImagePath(MainWindow.Setting.General.ModelPath, strOldGroupName), DirectoryManager.GetGroupImagePath(MainWindow.Setting.General.ModelPath, strNewGroupName));

                        // M028 : 분류명이 변경되었습니다.
                        MessageBox.Show(ResourceStringHelper.GetInformationMessage("M028"), "Information");

                   // }
                }
                else
                {
                    // M054 : 분류 수정에 실패하였습니다. 다시 시도해 주시기 바랍니다.
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M054"), "Database Error");
                }
            }
        }

        /// <summary>   Deletes the group. </summary>
        /// <remarks>   suoow2, 2014-08-11. </remarks>
        private void DeleteGroup()
        {
            Group selectedGroup = lbGroup.SelectedItem as Group;
          
            if (selectedGroup == null)
            {
                // M026 : 삭제하려는 분류를 선택해 주시기 바랍니다.
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("M026"), "Information");
                return;
            }

            bool? IsDelete = null;

            if (m_ModelManager.Models.Count > 0)
            {
                ModelInformation currentModel = MainWindow.CurrentModel;
                if (currentModel != null)
                {
                    if (currentModel.Type.Code == selectedGroup.Name)
                    {
                        // M061 : 현재 선택된 모델이 속한 그룹은 삭제할 수 없습니다.
                        MessageBox.Show(ResourceStringHelper.GetInformationMessage("M061"), "Information");
                        return;
                    }
                }

                string strWarningMessage = selectedGroup.Name + "\n\n분류와 관계된 " +
                                           m_ModelManager.Models.Count + "개의 모델들도 삭제됩니다. 정말로 삭제하시겠습니까?";
                WarningMessageBox warningMessageBox = new WarningMessageBox(strWarningMessage, "그룹 삭제");
                warningMessageBox.Owner = this;
                if (warningMessageBox.ShowDialog() == true) // DialogResult true.
                {
                    IsDelete = true;
                }
                else // DialogResult false.
                {
                    IsDelete = false;
                }
            }
            else
            {
                string strWarningMessage = "분류명 " + selectedGroup.Name + "를 삭제하시겠습니까?";
                WarningMessageBox warningMessageBox = new WarningMessageBox(strWarningMessage, "그룹 삭제");
                warningMessageBox.Owner = this;
                if (warningMessageBox.ShowDialog() == true) // DialogResult true.
                {
                    IsDelete = true;
                }
                else // DialogResult false.
                {
                    IsDelete = false;
                }
            }

            if (IsDelete == true)
            {
                if (m_ModelManager.DeleteGroup(selectedGroup.Index)) // 뷴류 삭제 성공!
                {
                    // 분류 삭제.
                    DirectoryManager.DeleteDirectory(DirectoryManager.GetGroupImagePath(MainWindow.Setting.General.ModelPath, selectedGroup.Name));

                    m_listStoredGroupName.Remove(selectedGroup.Name);

                    if (m_ModelManager.Groups.Count == 0)
                    {
                        lbModel.DataContext = null;
                    }

                    // M029 : 선택된 분류가 삭제되었습니다.
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M029"), "Information");
                }
                else // 분류 삭제 실패.
                {

                    // M055 : 분류 삭제에 실패하였습니다. 다시 시도해 주시기 바랍니다.
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M055"), "Information");
                }
            }
        }
        #endregion

        #region Create & Modify & Save & Delete model.
        private void CreateModel()
        {
            Group selectedGroup = lbGroup.SelectedItem as Group;
            if (selectedGroup == null)
            {
                // M022 : 모델이 속할 그룹을 생성하거나 선택해 주시기 바랍니다.
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("M022"), "Information");
                return;
            }
            CreateModelWindow dlg = new CreateModelWindow(m_listStoredModelName, 0);
            if ((bool)dlg.ShowDialog())
            {
                int ScanCount = MainWindow.m_nTotalScanCount;
                m_NewModel = new ModelInformation(dlg.model);
                m_NewModel.Description = dlg.code;
                for (int i = 0; i < m_NewModel.ScrabInfo.Length; i++)
                    m_NewModel.ScrabInfo[i] = false;
                for (int i = 0; i < cb_autoNG_Items.Count; i++)
                {
                    if (MainWindow.NG_Info.GetGroupID(i + 1) == 1) // 1 = 페기
                    {
                        m_NewModel.ScrabInfo[i] = true;
                    }

                    //if (MainWindow.NG_Info.GetBadName(i+1) == "외곽불량" || MainWindow.NG_Info.GetBadName(i + 1) == "PSRShift" || MainWindow.NG_Info.GetBadName(i + 1) == "VentHole" || MainWindow.NG_Info.GetBadName(i + 1) == "ID미인식")
                    //{
                    //    m_NewModel.ScrabInfo[i] = true;
                    //}
                }
                m_NewModel.ScanVelocity1 = MainWindow.Setting.Job.ScanVelocity1;
                m_NewModel.ScanVelocity2 = MainWindow.Setting.Job.ScanVelocity2;
                if (MainWindow.Setting.General.UsePOP)
                {
                    MainWindow ptrMainWindow = Application.Current.MainWindow as MainWindow;
                    if (ptrMainWindow != null && m_NewModel.Description != "")
                    {
                        ptrMainWindow.GetModelInfo(m_NewModel.Description, ref m_NewModel);
                    }
                }
                if (!MainWindow.Setting.SubSystem.IS.UseCASlave)
                {
                    m_NewModel.UseTopSur2 = false;
                }
                if (!MainWindow.Setting.SubSystem.IS.UseBASlave)
                {
                    m_NewModel.UseBotSur2 = false;
                }
                this.txtModelName.Text = dlg.model;
                this.pnlModelSpec.DataContext = m_NewModel;
                this.pnlModelSpec.IsEnabled = this.AutoNG_Control.IsEnabled = true;
                this.grdMark.DataContext = m_NewModel;
                this.grdMark.IsEnabled = true;
                this.btnMakeMark.IsEnabled = true;
                this.txtHeight.Focus();

                m_bCreateActionFinished = false;

                DisableButtons();
            }
            else
            {
                m_bCreateActionFinished = true;

                EnableButtons();
            }

            m_BeforeSelectedGroup = selectedGroup;
           
        }

        private void ModifyModel()
        {
            Group selectedGroup = lbGroup.SelectedItem as Group;
            ModelInformation selectedModel = lbModel.SelectedItem as ModelInformation;
            if (selectedGroup == null || selectedModel == null)
            {
                // M023 : 수정하려는 모델을 선택해 주시기 바랍니다.
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("M023"), "Information");
                return;
            }

            m_NewModel = PCS.ELF.AVI.ModelManager.CreateCloneModel(selectedModel);

            this.txtModelName.Text = m_NewModel.Name;
            this.pnlModelSpec.DataContext = m_NewModel;
            this.pnlModelSpec.IsEnabled = this.AutoNG_Control.IsEnabled = true;
            this.grdMark.DataContext = m_NewModel;
            this.grdMark.IsEnabled = true;
            this.btnMakeMark.IsEnabled = true;
            this.txtHeight.Focus();

            m_BeforeSelectedGroup = selectedGroup;
            m_BeforeSelectedModel = selectedModel;
            m_bModifyActionFinished = false;

            DisableButtons();
        }

        private void RenameModel()
        {
            Group selectedGroup = lbGroup.SelectedItem as Group;
            ModelInformation selectedModel = lbModel.SelectedItem as ModelInformation;
            if (selectedGroup == null || selectedModel == null)
            {
                // M023 : 수정하려는 모델을 선택해 주시기 바랍니다.
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("M023"), "Information");
                return;
            }
            
            int nSelectedModelIndex = selectedModel.Index;
            string strOldModelName = selectedModel.Name;

            CreateModelWindow dlg = new CreateModelWindow(m_listStoredModelName, 1);
            dlg.SetOldModel(selectedModel.Name, selectedModel.Description);


            if ((bool)dlg.ShowDialog())
            {
                if (!string.IsNullOrEmpty(dlg.model))
                {
                    m_listStoredModelName.Remove(selectedModel.Name);
                    m_listStoredModelName.Add(dlg.model);

                    if (m_ModelManager.RenameModel(nSelectedModelIndex, strOldModelName, dlg.model, dlg.code))
                    {
                        // 분류명 변경에 따른 디렉토리 이동 작업을 수행한다.
                        DirectoryManager.MoveDirectory(DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, selectedGroup.Name, strOldModelName), DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, selectedGroup.Name, dlg.model));

                        // M028 : 분류명이 변경되었습니다.
                        MessageBox.Show("모델명이 변경 되었습니다.", "Information");

                    }
                    else
                    {
                        // M054 : 분류 수정에 실패하였습니다. 다시 시도해 주시기 바랍니다.
                        MessageBox.Show("모델명 수정에 실패하였습니다. 다시 시도해 주시기 바랍니다", "Database Error");
                    }
                }
            }
        }

        private void SaveModel()
        {
            m_NewModel.MergeAutoNGMatrixList();
            // 전제 조건 : 모델은 특정 그룹 내에 속해야만 한다.
            Group selectedGroup = m_BeforeSelectedGroup;
            ModelInformation selectedModel = m_BeforeSelectedModel;

            if (selectedGroup != null)
            {
                string szStripCode = PCS.ELF.AVI.ModelManager.CheckModelExist(m_NewModel.Name);
                MarkSave(); BadFailSave();
                // DB상에 모델이름이 존재하면 '수정' 작업에 해당된다.
                if (szStripCode != string.Empty)
                {
                    if (IsValidModel())
                    {
                        // 리스트에서 선택하여 수정하는 경우
                        if (selectedModel != null && selectedModel.Name == m_NewModel.Name)
                        {
                            if (m_ModelManager.ModifyModel(m_NewModel, selectedModel.Index))
                            {
                                this.pnlModelSpec.IsEnabled = this.AutoNG_Control.IsEnabled = false;                            
                                this.btnMakeMark.IsEnabled = true;
                                m_bModifyActionFinished = true;
                                m_NewModel.PassingAutoNGMatrixInfo(m_NewModel.AutoNGMatrixInfo);
                                // M018 : 모델 정보가 수정되었습니다.
                                //MessageBox.Show(ResourceStringHelper.GetInformationMessage("M018"), "Information");
                            }
                            else
                            {
                                this.pnlModelSpec.IsEnabled = this.AutoNG_Control.IsEnabled = true;
                                this.grdMark.IsEnabled = true;

                                // M057 : 모델 수정에 실패하였습니다. 다시 시도해 주시기 바랍니다.
                                MessageBox.Show(ResourceStringHelper.GetInformationMessage("M057"), "Database Error");
                            }
                        }
                        else // use_yn = 0 이어서 use_yn = 1 로 수정하는 경우 (사용자 측면에서는 모델생성과 같은 작업임)
                        {
                            m_NewModel.Strip.Code = szStripCode;
                            m_NewModel.Code = m_NewModel.Name;
                        
                            if (m_ModelManager.ModifyModel(m_NewModel))
                            {
                                string szNewModelPath = DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, selectedGroup.Name, m_NewModel.Name);
                                DirectoryManager.CreateDirectory(szNewModelPath);

                                m_listStoredModelName.Add(m_NewModel.Name);
                                m_bCreateActionFinished = true;

                                this.lbModel.SelectedIndex = lbModel.Items.Count - 1;
                                this.txtModelName.Text = m_NewModel.Name;
                                this.pnlModelSpec.DataContext = m_NewModel;
                                this.grdMark.DataContext = m_NewModel;
                                this.grdMark.IsEnabled = false;
                                this.pnlModelSpec.IsEnabled = this.AutoNG_Control.IsEnabled = false;
                                this.btnMakeMark.IsEnabled = true;
                                
                                // 복사 & 붙여넣기 모델인 경우 기존 모델의 티칭 데이터를 복사하여 준다.
                                ApplyPasteModel(szNewModelPath);
                            }
                            else
                            {
                                this.pnlModelSpec.IsEnabled = this.AutoNG_Control.IsEnabled = true;
                                this.grdMark.IsEnabled = true;
                                this.btnMakeMark.IsEnabled = true;
                                
                                // M056 : 모델 생성에 실패하였습니다. 다시 시도해 주시기 바랍니다.
                                MessageBox.Show(ResourceStringHelper.GetInformationMessage("M056"), "Database Error");
                            }
                        }
                    }
                }
                else // 새로운 모델을 등록하는 경우.
                {
                    if (IsValidModel())
                    {
                        m_NewModel.Type.Code = selectedGroup.Name;

                        if (m_ModelManager.CreateModel(m_NewModel))
                        {
                            PCS.ELF.AVI.ModelManager.UpdateModelMarkInfo(m_NewModel);

                            // 새로운 모델에 대한 이미지 저장 경로를 생성한다.
                            string szNewModelPath = DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, selectedGroup.Name, m_NewModel.Name);
                            DirectoryManager.CreateDirectory(szNewModelPath);

                            m_listStoredModelName.Add(m_NewModel.Name);
                            m_bCreateActionFinished = true;

                            this.lbModel.SelectedIndex = lbModel.Items.Count - 1;
                            this.txtModelName.Text = m_NewModel.Name;
                            this.pnlModelSpec.DataContext = m_NewModel;
                            this.pnlModelSpec.IsEnabled = this.AutoNG_Control.IsEnabled = false;
                            this.grdMark.DataContext = m_NewModel;
                            this.grdMark.IsEnabled = false;
                            this.btnMakeMark.IsEnabled = true;

                            // 복사 & 붙여넣기 모델인 경우 기존 모델의 티칭 데이터를 복사하여 준다.
                            ApplyPasteModel(szNewModelPath);
                            Common.Drawing.MarkingInformation.MarkLogo logo = new Common.Drawing.MarkingInformation.MarkLogo();
                            if (MessageBox.Show("마크 파일을 복사해서 사용 하시겠습니까?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
                            {
                                if (!Common.Drawing.MarkingInformation.MarkInformation.CreateNewModel(m_NewModel.CopyToMarkInfo(), MainWindow.Setting.SubSystem.Laser.IP, logo))
                                    MessageBox.Show("Mark1 : Mark File 생성에 실패 하였습니다.");
                            }
                            else
                            {
                                Common.Drawing.MarkingInformation.MarkInformation.CopyTo(m_szCaptureMark, m_NewModel.Name, MainWindow.Setting.SubSystem.Laser.IP, m_NewModel.CopyToMarkInfo(), logo);
                            }
                        }
                        else
                        {
                            // M056 : 모델 생성에 실패하였습니다. 다시 시도해 주시기 바랍니다.
                            MessageBox.Show(ResourceStringHelper.GetInformationMessage("M056"), "Database Error");

                            this.pnlModelSpec.IsEnabled = this.AutoNG_Control.IsEnabled = true;
                            this.grdMark.IsEnabled = true;
                            this.btnMakeMark.IsEnabled = true;
                        }
                    }
                }
            }
        }

        private void DeleteModel()
        {
            Group selectedGroup = lbGroup.SelectedItem as Group;
            ModelInformation selectedModel = lbModel.SelectedItem as ModelInformation;
            if (selectedGroup == null || selectedModel == null)
            {
                // M024 : 삭제하려는 모델을 선택해 주시기 바랍니다.
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("M024"), "Information");
                return;
            }

            bool? IsDelete = null;
            if (selectedModel!= null)
            {
                ModelInformation currentModel = MainWindow.CurrentModel;
                if (currentModel != null)
                {
                    if (selectedModel.Code == currentModel.Code)
                    {
                        MessageBox.Show(ResourceStringHelper.GetInformationMessage("M060"), "Information");
                        return;
                    }
                }

                string strWarningMessage = selectedModel.Name + "\n\n정말로 삭제하시겠습니까?";
                WarningMessageBox warningMessageBox = new WarningMessageBox(strWarningMessage, "모델 삭제") { Owner = this };
                if (warningMessageBox.ShowDialog() == true) // DialogResult true.
                    IsDelete = true;
                else // DialogResult false.
                    IsDelete = false;
            }
            else
            {
                IsDelete = true;
            }

            if (IsDelete == true)
            {
                if (m_ModelManager.DeleteModel(selectedModel))
                {
                    m_listStoredModelName.Remove(selectedModel.Name);
                    if (imgBase.Source != null)
                    {
                        imgBase.Source = null;
                        imgBase.Refresh();
                    }

                    // 2012-08-06 suoow2; 실제 Data는 삭제시키지 않도록 함.
                    // DirectoryManager.DeleteDirectory(DirectoryManager.GetModelImagePath(selectedGroup.Name, selectedModel.Name));
                    // TeachingDataManager.DeleteTeachingData(MainWindow.Setting.General.MachineCode, selectedModel.Name);
                    
                    // M059 : 선택된 모델이 삭제되었습니다.
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M059"), "Information");
                }
                else
                {
                    // M058 : 모델 삭제에 실패하였습니다. 다시 시도해 주시기 바랍니다.
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M058"), "Database Error");
                }
            }
        }

        private void CopyModel()
        {
            if (lbGroup.Items.Count <= 0 || lbModel.Items.Count <= 0 || lbGroup.SelectedIndex < 0 || lbModel.SelectedIndex < 0)
            {
                return;
            }

            try
            {
                Group targetGroup = lbGroup.Items[lbGroup.SelectedIndex] as Group;
                ModelInformation targetModel = lbModel.Items[lbModel.SelectedIndex] as ModelInformation;
                if (targetGroup != null && targetModel != null)
                {
                    m_szCaptureGroupName = targetGroup.Name;
                    m_szCaptureModelName = targetModel.Name;

                    m_ModelManager.CaptureModel = PCS.ELF.AVI.ModelManager.CreateCloneModel(targetModel);
                    m_ModelManager.CaptureModel.Name = string.Empty;
                    m_ModelManager.CaptureModel.Code = string.Empty;
                    EnableButtons();
                }
            }
            catch { }
        }

        private string CreatePasteModelName(string aszCaptureModelName)
        {
            try
            {
                int nDashIndex = -1;
                for (int nIndex = aszCaptureModelName.Length - 1; nIndex >= 0; nIndex--)
                {
                    if (aszCaptureModelName[nIndex] == '-')
                    {
                        nDashIndex = nIndex;
                        break;
                    }
                }

                if (nDashIndex >= 0)
                {
                    string szRevision = aszCaptureModelName.Substring(nDashIndex + 1, aszCaptureModelName.Length - nDashIndex - 1);
                    int nRevision = -1;
                    if (Int32.TryParse(szRevision, out nRevision))
                    {
                        if (nRevision > 0)
                        {
                            nRevision++;
                            aszCaptureModelName = aszCaptureModelName.Substring(0, nDashIndex + 1);
                            aszCaptureModelName += nRevision.ToString();
                        }
                    }
                    else aszCaptureModelName += "-1";
                }
                else aszCaptureModelName += "-1";
            }
            catch { }

            return aszCaptureModelName;
        }

        private void PasteModel()
        {
            if (m_ModelManager.CaptureModel != null)
            {
                Group selectedGroup = lbGroup.SelectedItem as Group;
                if (selectedGroup == null)
                {
                    // M022 : 모델이 속할 그룹을 생성하거나 선택해 주시기 바랍니다.
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M022"), "Information");
                    return;
                }

                string strModelName = GetInputMessage("모델 추가", "복사한 모델의 새 이름을 입력해 주십시오.", m_listStoredModelName, CreatePasteModelName(m_szCaptureModelName));
                if (!string.IsNullOrEmpty(strModelName))
                {
                    m_NewModel = PCS.ELF.AVI.ModelManager.CreateCloneModel(m_ModelManager.CaptureModel);
                    m_NewModel.Name = strModelName;
                    m_NewModel.Code = strModelName;
                    m_szCaptureMark = m_szCaptureModelName;
                    this.txtModelName.Text = strModelName;
                    this.pnlModelSpec.DataContext = m_NewModel;
                    this.pnlModelSpec.IsEnabled = this.AutoNG_Control.IsEnabled = true;
                    this.grdMark.DataContext = m_NewModel;
                    this.grdMark.IsEnabled = true;
                    this.btnMakeMark.IsEnabled = true;
                    this.txtHeight.Focus();
                    MarkSeletion(m_NewModel);
                    BadFailSelection(m_NewModel);
                    m_bCreateActionFinished = false;
                    m_ModelManager.CaptureModel = null;

                    DisableButtons();
                }
                else
                {
                    m_bCreateActionFinished = true;

                    EnableButtons();
                }

                m_BeforeSelectedGroup = selectedGroup;
            }
        }

        private void ApplyPasteModel(string aszNewModelPath)
        {
            if (!string.IsNullOrEmpty(m_szCaptureModelName) && !string.IsNullOrEmpty(m_szCaptureGroupName))
            {
                string szMachineCode = MainWindow.Setting.General.MachineCode;

                #region 기준 이미지 복사를 수행한다.
                string[] FILES = DirectoryManager.GetFileEntries(DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, m_szCaptureGroupName, m_szCaptureModelName));
                foreach (string FILE in FILES)
                {
                    FileInfo fileInfo = new FileInfo(FILE);
                    if (fileInfo.Extension == ".bmp")
                    {
                        if (fileInfo.Name[0] == '1' || fileInfo.Name[0] == '2' || fileInfo.Name[0] == '3')
                        {
                            FileSupport.FileCopy(fileInfo.FullName, String.Format("{0}/{1}", aszNewModelPath, fileInfo.Name));
                        }
                    }
                    if (fileInfo.Name == "align1.dat" || fileInfo.Name == "align2.dat")
                    {
                        FileSupport.FileCopy(fileInfo.FullName, String.Format("{0}/{1}", aszNewModelPath, fileInfo.Name));
                    }
                }
                #endregion

                ImageSource imageSource = null;
                if (SectionManager.SectionTypes.Count == 0)
                    SectionManager.LoadSectionType();

                // 기존에 존재하던 티칭 데이터가 있다면 삭제 후 복사를 수행한다.
                if (!TeachingDataManager.DeleteTeachingData(szMachineCode, m_NewModel.Name))
                {
                    Debug.WriteLine("모델 복사 수행 중 기존에 존재하던 티칭 데이터 삭제에 실패하였습니다.");
                    return;
                }

                foreach (Surface surface in ((Surface[])Enum.GetValues(typeof(Surface))))
                {
                    if (File.Exists(System.IO.Path.Combine(aszNewModelPath, (int)surface + "-Based.bmp")))
                    {
                        imageSource = BitmapImageLoader.LoadCachedBitmapImage(new Uri(System.IO.Path.Combine(aszNewModelPath, (int)surface + "-Based.bmp")));
                        if (imageSource != null) CloneTeachingData(szMachineCode, m_szCaptureModelName, surface);                        
                    }
                }                
               
                m_szCaptureGroupName = m_szCaptureModelName = string.Empty;

                if (imageSource != null)
                {
                    imgBase.Source = imageSource.Clone(); // can be null.
                }
            }
            else
            {
                imgBase.Source = null;
            }
        }
        #endregion

        private string GetInputMessage(string strTitle, string strCaption, List<string> listStoredName = null, string strInputText = null)
        {
            InputTextCheckBox inputTextCheckBox = new InputTextCheckBox(strTitle, strCaption, strInputText, listStoredName) { Owner = this };
            inputTextCheckBox.ShowDialog();

            if (inputTextCheckBox.DialogResult == true)
            {
                return inputTextCheckBox.m_strInputMessage;
            }
            else
            {
                return string.Empty;
            }
        }

        #region Validation.
        private bool IsValidModel()
        {
            try
            {
                #region Check null or empty value.
                if ((ValueValidator.IsValidTextBoxValue(txtWidth) && 
                    ValueValidator.IsValidTextBoxValue(txtHeight) &&
                    ValueValidator.IsValidTextBoxValue(txtThickness) &&
                    ValueValidator.IsValidTextBoxValue(txtScanVelocity1) &&
                    ValueValidator.IsValidTextBoxValue(txtScanVelocity2) && 
                    ValueValidator.IsValidTextBoxValue(txtUnitX) &&
                    ValueValidator.IsValidTextBoxValue(txtUnitY) &&
                    ValueValidator.IsValidTextBoxValue(txtUnitPitchX) &&
                    ValueValidator.IsValidTextBoxValue(txtUnitPitchY) && 
                    ValueValidator.IsValidTextBoxValue(txtBlock) && 
                    ValueValidator.IsValidTextBoxValue(txtConBad) &&
                    ValueValidator.IsValidTextBoxValue(txtBlockGap)
                   )
                    == false)
                {
                    return false;
                }
                #endregion

                #region Cast Text to value-type.
                double fLength = Convert.ToDouble(txtWidth.Text);
                double fWidth = Convert.ToDouble(txtHeight.Text);
                double fThickness = Convert.ToDouble(txtThickness.Text);
                int nBlock = Convert.ToInt32(txtBlock.Text);
                double fBlockGap = Convert.ToDouble(txtBlockGap.Text);
                int nUnitX = Convert.ToInt32(txtUnitX.Text);
                int nUnitY = Convert.ToInt32(txtUnitY.Text);
                double fUnitPitchX = Convert.ToDouble(txtUnitPitchX.Text);
                double fUnitPitchY = Convert.ToDouble(txtUnitPitchY.Text);
                #endregion

                #region Check model size.
                if (fLength > 300)
                {
                    // M068	제품 길이는 300보다 작아야 합니다.
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M068"), "Information");
                    txtWidth.Focus();
                    return false;
                }

                if (fLength <= 0)
                {
                    // M031 : 제품 길이는 0보다 커야 합니다.
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M031"), "Information");
                    txtWidth.Focus();
                    return false;
                }

                if (fWidth < 60)
                {
                    MessageBox.Show("제품 폭은 60mm 보다 커야 합니다.", "Information");
                    txtHeight.Focus();
                    return false;
                }
                else if (fWidth > 150)
                {
                    MessageBox.Show("제품 폭은 150mm 보다 작아야 합니다.", "Information");
                    txtHeight.Focus();
                    return false;
                }

                if (fThickness < 0.0)
                {
                    MessageBox.Show("제품 두께는 0mm 보다 커야 합니다.", "Information");
                    txtThickness.Focus();
                    return false;
                }
                else if (fThickness > 1.5)
                {
                    MessageBox.Show("제품 두께는 1.500mm 보다 작아야 합니다.", "Information");
                    txtThickness.Focus();
                    return false;
                }

                if (fWidth <= 0)
                {
                    // M030 : 제품 폭은 0보다 커야 합니다.
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M030"), "Information");
                    txtHeight.Focus();
                    return false;
                }
                #endregion

                #region Check block values.
                if (nBlock <= 0)
                {
                    m_NewModel.Strip.Block = 1;
                    txtBlock.Text = "1";
                    // M034 : Block은 최소 1개 이상이어야 합니다. (원맵 타입인 경우 1로 지정해 주십시오.)
                    //MessageBox.Show(ResourceStringHelper.GetInformationMessage("M034"), "Information");
                    //txtBlock.Focus();
                    //return false;
                }

                if (nBlock == 1)
                {
                    txtBlockGap.Text = "0";
                    m_NewModel.Strip.BlockGap = 0;
                    fBlockGap = 0;
                }
                else if (nBlock != 1 && fBlockGap <= 0.0)
                {
                    // M036 : Block의 Pitch 값은 0보다 커야 합니다.
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M036"), "Information");
                    txtBlockGap.Focus();
                    return false;
                }
                else if (fBlockGap > fLength)
                {
                    // M037 : Block의 X축 Pitch 값은 제품의 길이를 초과할 수 없습니다.
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M037"), "Information");
                    txtBlockGap.Focus();
                    return false;
                }

                #endregion

                #region Check unit values.

                if (nUnitX <= 0)
                {
                    // M039 : Unit은 최소 1개 이상이어야 합니다.
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M039"), "Information");
                    txtUnitX.Focus();
                    return false;
                }

                if (nUnitY <= 0)
                {
                    // M039 : Unit은 최소 1개 이상이어야 합니다.
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M039"), "Information");
                    txtUnitY.Focus();
                    return false;
                }

                //if (nTotalUnits < nUnitX * nUnitY)
                //{
                    // M039 : Unit은 최소 1개 이상이어야 합니다.
                //    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M070"), "Information");
                //    txtTotalUnits.Focus();
                //    return false;
               // }
                
                //if (fUnitPitchX <= 0.0)
                //{
                //    // M043 : Unit의 Pitch 값은 0보다 커야 합니다.
                //    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M043"), "Information");
                //    txtUnitPitchX.Focus();
                //    return false;
                //}
                //else 
                    if (fUnitPitchX > fLength)
                {
                    // M044 : Unit의 X축 Pitch 값은 제품의 길이를 초과할 수 없습니다.
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M044"), "Information");
                    txtUnitPitchX.Focus();
                    return false;
                }

                //if (fUnitPitchY <= 0.0)
                //{
                //    // M043 : Unit의 Pitch 값은 0보다 커야 합니다.
                //    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M043"), "Information");
                //    txtUnitPitchY.Focus();
                //    return false;
                //}
                //else
                    if (fUnitPitchY > fWidth)
                {
                    // M045 : Unit의 Y축 Pitch 값은 제품의 폭을 초과할 수 없습니다.
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M045"), "Information");
                    txtUnitPitchY.Focus();
                    return false;
                }
                #endregion

                #region Check X-Y arrays.
                // 2012-03-21 suoow2. : 제품 등록 스펙 완화. 
                //if ((double)(nUnitX * fUnitPitchX) > fLength)
                //{
                //    // M048 : X축 Unit의 배열이 제품의 길이를 초과하였습니다. 확인바랍니다.
                //    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M048"), "Information");
                //    txtUnitX.Focus();
                //    return false;
                //}

                //if ((double)(nUnitY * fUnitPitchY) > fWidth)
                //{
                //    // M049 : Y축 Unit의 배열이 제품의 폭을 초과하였습니다. 확인바랍니다.
                //    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M049"), "Information");
                //    txtUnitY.Focus();
                //    return false;
                //}

                //if ((double)(nBlockX * fBlockPitchX) > fLength)
                //{
                //    // M050 : X축 Block의 배열이 제품의 길이를 초과하였습니다. 확인바랍니다.
                //    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M050"), "Information");
                //    txtBlockX.Focus();
                //    return false;
                //}

                //if ((double)(nBlockY * fBlockPitchY) > fWidth)
                //{
                //    // M051 : Y축 Block의 배열이 제품의 폭을 초과하였습니다. 확인바랍니다.
                //    MessageBox.Show(ResourceStringHelper.GetInformationMessage("M051"), "Information");
                //    txtBlockY.Focus();
                //    return false;
                //}
                #endregion

                if ((bool)chkUseMarking.IsChecked)
                {
                    if (txtMarkStep.Text.Trim() == "")
                    {
                        MessageBox.Show("마킹 스텝을 입력 하세요.");
                        m_NewModel.Strip.BlockGap = 3;
                        txtMarkStep.Focus();
                        return false;
                    }
                    int nStep = 3;
                    try
                    {
                        nStep = Convert.ToInt32(txtMarkStep.Text);
                    }
                    catch
                    {
                        MessageBox.Show("마킹 스텝을 잘못 입력 하셨습니다.");
                        m_NewModel.Strip.BlockGap = 3;
                        txtMarkStep.Focus();
                        return false;
                    }
                    if (nStep > 3 || nStep < 2)
                    {
                        MessageBox.Show("마킹 스텝은 2회 혹은 3회로 입력 해 주세요.");
                        m_NewModel.Strip.BlockGap = 3;
                        txtMarkStep.Focus();
                        return false;
                    }

                }

                return true;
            }
            catch
            {
                return false;
            }
            
        }
        #endregion

        private void ChangeModel()
        {
            Group selectedGroup = lbGroup.SelectedItem as Group;
            ModelInformation selectedModel = lbModel.SelectedItem as ModelInformation;

            if (selectedGroup == null)
            {
                return;
            }

            if (selectedModel == null)
            {
                if (imgBase.Source != null)
                {
                    imgBase.Source = null;
                }

                this.txtModelName.Text = "선택된 모델이 없습니다.";
                this.pnlModelSpec.DataContext = new ModelInformation();
            }
            else
            {
                // 기준영상이 존재하는 경우 보여준다.
                string imagePath = DirectoryManager.GetModelImagePath(MainWindow.Setting.General.ModelPath, selectedGroup.Name, selectedModel.Name);
                TryShowBasedImage(imagePath);

                this.txtModelName.Text = selectedModel.Name;
                this.pnlModelSpec.DataContext = selectedModel;
                this.grdMark.DataContext = selectedModel;
                PSR_Shift_Margin = selectedModel.Strip.PSRMarginX;
            }
            MarkSeletion(selectedModel);
            BadFailSelection(selectedModel);
            this.pnlModelSpec.IsEnabled = this.AutoNG_Control.IsEnabled = false;
            this.grdMark.IsEnabled = false;
            this.btnMakeMark.IsEnabled = true;
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

        // 기준영상이 존재하는 경우 보여준다.
        private void TryShowBasedImage(string aszModelImagePath)
        {
            ImageSource imageSource = null;

            foreach (Surface surface in ((Surface[])Enum.GetValues(typeof(Surface))).Reverse())
            {
                if (File.Exists(System.IO.Path.Combine(aszModelImagePath, (int)surface + "-Based.bmp")))
                {
                    imageSource = BitmapImageLoader.LoadCachedBitmapImage(new Uri(System.IO.Path.Combine(aszModelImagePath, (int)surface + "-Based.bmp")));
                    if (imageSource != null)
                    {
                        imgBase.Source = imageSource;
                        return;
                    }
                }
            }

            imgBase.Source = null;
        }

        #region Handle context menu.
        private void EnableButtons()
        {
            ContextMenu groupContextMenu = lbGroup.ContextMenu;
            if (groupContextMenu != null)
            {
                foreach (MenuItem menuItem in groupContextMenu.Items)
                {
                    menuItem.IsEnabled = true;
                }
            }

            ContextMenu modelContextMenu = lbModel.ContextMenu;
            if (modelContextMenu != null)
            {
                foreach (MenuItem menuItem in modelContextMenu.Items)
                {
                    if ((ModelOperation)menuItem.Tag == ModelOperation.Paste)
                    {
                        if (m_ModelManager.CaptureModel != null) // 복사된 모델이 있는 경우 contextmenu 활성화.
                        {
                            menuItem.IsEnabled = true;
                        }
                        else
                        {
                            menuItem.IsEnabled = false;
                        }
                    }
                    else
                    {
                        menuItem.IsEnabled = true;
                    }
                }
            }

            this.btnCreateGroup.IsEnabled = true;
            this.btnModifyGroup.IsEnabled = true;
            this.btnDeleteGroup.IsEnabled = true;
            this.btnCreateModel.IsEnabled = true;
            this.btnModifyModel.IsEnabled = true;
            this.btnDeleteModel.IsEnabled = true;
        }

        private void DisableButtons()
        {
            ContextMenu groupContextMenu = lbGroup.ContextMenu;
            ContextMenu modelContextMenu = lbModel.ContextMenu;

            if (m_bCreateActionFinished == false)
            {
                this.btnCreateGroup.IsEnabled = false;
                this.btnModifyGroup.IsEnabled = false;
                this.btnDeleteGroup.IsEnabled = false;
                this.btnCreateModel.IsEnabled = true;
                this.btnModifyModel.IsEnabled = false;
                this.btnDeleteModel.IsEnabled = false;
                
                if (groupContextMenu != null)
                {
                    if (m_bCreateActionFinished == false || m_bModifyActionFinished == false)
                    {
                        foreach (MenuItem menuItem in groupContextMenu.Items)
                        {
                            menuItem.IsEnabled = false;
                        }
                    }
                }
                
                if (modelContextMenu != null)
                {
                    foreach (MenuItem menuItem in modelContextMenu.Items)
                    {
                        if ((ModelOperation)menuItem.Tag != ModelOperation.Create)
                        {
                            menuItem.IsEnabled = false;
                        }
                        else
                        {
                            menuItem.IsEnabled = true;
                        }
                    }
                }
            }
            else if(m_bModifyActionFinished == false)
            {
                this.btnCreateGroup.IsEnabled = false;
                this.btnModifyGroup.IsEnabled = false;
                this.btnDeleteGroup.IsEnabled = false;
                this.btnCreateModel.IsEnabled = false;
                this.btnModifyModel.IsEnabled = true;
                this.btnDeleteModel.IsEnabled = false;

                if (modelContextMenu != null)
                {
                    foreach (MenuItem menuItem in modelContextMenu.Items)
                    {
                        if ((ModelOperation)menuItem.Tag != ModelOperation.Modify)
                        {
                            menuItem.IsEnabled = false;
                        }
                        else
                        {
                            menuItem.IsEnabled = true;
                        }
                    }
                }
            }
        }
        #endregion

        private void btnRerunVision_Click(object sender, RoutedEventArgs e)
        {
            Button btnTarget = null;
            Border circTarget = null;
            int nTargetIndex = Convert.ToInt32(((Button)(sender)).Tag);

            switch(nTargetIndex)
            {
                case 0: btnTarget = btnRerunVision1; circTarget = circVision1State;  break;
                case 1: btnTarget = btnRerunVision2; circTarget = circVision2State; break;
                case 2: btnTarget = btnRerunVision3; circTarget = circVision3State; break;
                case 3: btnTarget = btnRerunVision4; circTarget = circVision4State; break;
                case 4: btnTarget = btnRerunVision5; circTarget = circVision5State; break;
            }

            MainWindow ptrMainWindow = Application.Current.MainWindow as MainWindow;
            if (ptrMainWindow == null || btnTarget == null || circTarget == null || nTargetIndex == -1)
                return;

            MainWindow.CommandCloseIS(ptrMainWindow.debug, nTargetIndex);
            System.Threading.Thread.Sleep(3000); // 종료시키기까지 충분한 시간 필요.
            MainWindow.CommandExecute(ptrMainWindow.debug, nTargetIndex);
            System.Threading.Thread.Sleep(3000); // 재실행시키기까지 충분한 시간 필요.
            MainWindow.CheckRunningState(ptrMainWindow.debug, nTargetIndex);

            if (!MainWindow.Setting.SubSystem.PLC.UsePLC)
            {
                ptrMainWindow.PCSInstance.ConnectVision(nTargetIndex, "127.0.0.1", 15000 + nTargetIndex);
            }
            else
            {
                ptrMainWindow.PCSInstance.ConnectVision(nTargetIndex, MainWindow.Setting.SubSystem.IS.IP[nTargetIndex], MainWindow.Setting.SubSystem.IS.Port[nTargetIndex]);
            }
            btnTarget.IsEnabled = false;
            circTarget.Background = new SolidColorBrush(Colors.Green);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void CheckScanSkip(out int TopCount, out int BotCount)
        {
            //////// 택타임 이슈 발생시 사용하자.
            ///      체크된 수 만큼 스캔 카운트 지정 프로그램
            int BPCount = 0; int BACount = 0; int CACount = 0;
            for (int i = 0; i < CheckBoxBPContainer.Children.Count; i++)
            {
                if (CheckBoxBPContainer.Children[i] is System.Windows.Controls.CheckBox checkBox)
                {
                    if (checkBox.Name.Contains(CategorySurface.BP.ToString()) && checkBox.IsChecked == true)
                    {
                        BPCount++;
                    }
                }
            }
            for (int i = 0; i < CheckBoxCAContainer.Children.Count; i++)
            {
                if (CheckBoxCAContainer.Children[i] is System.Windows.Controls.CheckBox checkBox)
                {
                    if (checkBox.IsChecked == true)
                    {
                        CACount++;
                    }
                }
            }
            for (int i = 0; i < CheckBoxBAContainer.Children.Count; i++)
            {
                if (CheckBoxBAContainer.Children[i] is System.Windows.Controls.CheckBox checkBox)
                {
                    if (checkBox.IsChecked == true)
                    {
                        BACount++;
                    }
                }
            }

            if (BPCount == 0 && CACount == 0)
            {
                TopCount = 1;
            }
            else
            {
                if (BPCount * 2 > CACount) { TopCount = BPCount * 2; }
                else TopCount = CACount;
            }
            if (BACount == 0) BACount = 1;
            BotCount = BACount;
            //
            /////이부분은 필요 없지 않을까.. 만들었지만 테스트 안해봄
            //m_ptrMainWindow.InspectionMonitoringCtrl.BP_Done = new bool[BPCount * 2, InspectionMonitoring.BUFFER];
            //m_ptrMainWindow.InspectionMonitoringCtrl.CA_Done = new bool[CACount, InspectionMonitoring.BUFFER];
            //m_ptrMainWindow.InspectionMonitoringCtrl.BA_Done = new bool[BACount, InspectionMonitoring.BUFFER];
            //
            //InspectProcess.BPInspectDone = new bool[BPCount * 2];
            //InspectProcess.BAInspectDone = new bool[BACount];
            //InspectProcess.CAInspectDone = new bool[CACount];
            //
            //InspectProcess.BPInspectSkip = new bool[BPCount];
            //InspectProcess.CAInspectSkip = new bool[CACount];
            //InspectProcess.BAInspectSkip = new bool[CACount];
        }
    }
}
