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
using Common;
using PCS.ModelTeaching.OfflineTeaching;
using System.Diagnostics;
using PCS.ELF.AVI;

namespace HDSInspector.SubWindow
{
    public partial class OfflineModelSelectWindow : Window
    {
        private readonly MTSManager m_ptrMTSManager;
        private PCS.ELF.AVI.ModelManager m_ModelManager = new PCS.ELF.AVI.ModelManager();

        public Group SelectedGroup { get; private set; }
        public ModelInformation SelectedModel { get; private set; }

        #region Ctor & Initializer.
        public OfflineModelSelectWindow(MTSManager aMTSManager)
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
            this.m_ptrMTSManager = aMTSManager;
        }

        private void InitializeDialog()
        {
            lbModel.DataContext = m_ModelManager.Models;
            lbGroup.DataContext = m_ModelManager.Groups;
            lbModel.PreviewMouseWheel += (s, e) => DoWheelAction(svModel, e.Delta);
            lbGroup.PreviewMouseWheel += (s, e) => DoWheelAction(svGroup, e.Delta);
            lbModel.SelectionChanged += lbModel_SelectionChanged;
            lbGroup.SelectionChanged += lbGroup_SelectionChanged;
            btnGoldenModel.Click += btnGoldenModel_Click;
        }

        private void InitializeEvent()
        {
            this.Closing += OfflineModelSelectWindow_Closing;
            this.btnStart.Click += btnStart_Click;
            this.KeyDown += (s, e) => { if (e.Key == Key.Escape) this.Close(); };
            this.cmbMachineList.SelectionChanged += cmbMachineList_SelectionChanged;
        }

        private void OfflineModelSelectWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        #endregion

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

        public void SetMachineList(ref List<MachineInformation> aMachines)
        {
            cmbMachineList.ItemsSource = aMachines;
            cmbMachineList.DisplayMemberPath = "Name";
            cmbMachineList.SelectedValuePath = "IP";
            cmbMachineList.SelectedIndex = 0;

            if (cmbMachineList.Items.Count - 1 >= m_ptrMTSManager.LastSelectedMachine &&
                aMachines[m_ptrMTSManager.LastSelectedMachine] != null)
            {
                cmbMachineList.SelectedIndex = m_ptrMTSManager.LastSelectedMachine;
                m_ptrMTSManager.ConnectToSelectedMachine(aMachines[m_ptrMTSManager.LastSelectedMachine].Name);

                m_ModelManager.Groups.Clear();
                m_ModelManager.Models.Clear();

                m_ModelManager.LoadGroup();
                lbGroup.ItemsSource = m_ModelManager.Groups;
                if (lbGroup.Items.Count - 1 >= m_ptrMTSManager.LastSelectedGroup)
                {
                    lbGroup.SelectedIndex = m_ptrMTSManager.LastSelectedGroup;
                    Group selectedGroup = lbGroup.SelectedItem as Group;
                    if (selectedGroup != null)
                    {
                        m_ModelManager.LoadModel(selectedGroup.Name);
                        lbModel.ItemsSource = m_ModelManager.Models;
                        if (lbModel.Items.Count - 1 >= m_ptrMTSManager.LastSelectedModel)
                        {
                            lbModel.SelectedIndex = m_ptrMTSManager.LastSelectedModel;
                        }
                    }
                }
                else cmbMachineList.SelectedIndex = 0;
            }
        }

        #region Group, Model 선택.
        private void lbGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Group selectedGroup = lbGroup.SelectedItem as Group;
            if (selectedGroup != null)
            {
                m_ModelManager.LoadModel(selectedGroup.Name);
                if (m_ModelManager.Models.Count > 0)
                {
                    this.lbModel.SelectedIndex = 0;
                }
            }
        }

        private void lbModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Group selectedGroup = lbGroup.SelectedItem as Group;
            ModelInformation selectedModel = lbModel.SelectedItem as ModelInformation;
            if (selectedModel == null && lbModel.HasItems)
            {
                lbModel.SelectedIndex = 0;
                return;
            }

            if (selectedGroup != null && selectedModel != null)
            {
                txtModelName.Text = selectedModel.Name;
                pnlModelSpec.DataContext = selectedModel;
            }
        }
        #endregion Group, Model 선택.

        // 해당 장비의 그룹과 모델을 조회한다.
        private void cmbMachineList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MachineInformation machine = cmbMachineList.SelectedItem as MachineInformation;
            if (machine != null && m_ptrMTSManager != null)
            {
                pnlIP.Visibility = Visibility.Visible;
                pnlResolution.Visibility = Visibility.Visible;
                txtDBIP.IPAddress = machine.IP;
                txtResolution.Text = machine.CamResolutionX.ToString();

                txtModelName.Text = "-";
                pnlModelSpec.DataContext = null;

                m_ptrMTSManager.ConnectToSelectedMachine(machine.Name);
                m_ModelManager.Groups.Clear();
                m_ModelManager.Models.Clear();
                m_ModelManager.LoadGroup();
            }
        }

        private void btnGoldenModel_Click(object sender, RoutedEventArgs e)
        {
            MachineInformation machine = cmbMachineList.SelectedItem as MachineInformation;
            if (machine != null)
            {
                m_ptrMTSManager.SelectedMachine = machine;
                MainWindow.Setting.General.MachineCode = machine.Code;
                MainWindow.Setting.General.MachineName = machine.Name;

                GoldenLoadWindow golden = new GoldenLoadWindow(machine.Type);
                if ((bool)golden.ShowDialog())
                {
                    m_ModelManager.LoadGroup();
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
        }

        // 선택된 장비의 모델로 작업을 시작한다.
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            SelectedGroup = lbGroup.SelectedItem as Group;
            SelectedModel = lbModel.SelectedItem as ModelInformation;
            
            m_ptrMTSManager.LastSelectedMachine = cmbMachineList.SelectedIndex;
            m_ptrMTSManager.LastSelectedGroup = lbGroup.SelectedIndex;
            m_ptrMTSManager.LastSelectedModel = lbModel.SelectedIndex;
            
            if (SelectedGroup != null && SelectedModel != null)
            {
                PCS.ELF.AVI.ModelManager.SelectedGroup = SelectedGroup;
                PCS.ELF.AVI.ModelManager.SelectedModel = SelectedModel;
                m_ptrMTSManager.SelectedMachine = cmbMachineList.SelectedItem as MachineInformation;
                if (m_ptrMTSManager.SelectedMachine != null)
                {
                    MainWindow.Setting.General.MachineCode = m_ptrMTSManager.SelectedMachine.Code;
                    MainWindow.Setting.General.MachineName = m_ptrMTSManager.SelectedMachine.Name;
                }
                this.Close();
            }
            else
            {
                // M066 : 모델을 선택해 주시기 바랍니다.
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("M066"), "Information");
            }
        }
    }
}
