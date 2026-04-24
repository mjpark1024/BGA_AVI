using System;
using System.Windows;
using System.Windows.Input;
using Common;
using HDSInspector.SubControl;
using HDSInspector.SubWindow;
using Microsoft.Win32;

namespace HDSInspector
{
    public partial class SettingWindow : Window
    {
        #region Member variables.
        private MainWindow m_ptrMainWindow;

        private SettingGeneral m_SettingGeneral = new SettingGeneral();
        private SettingAutoNG m_SettingAutoNG = new SettingAutoNG();
        private SettingGAIN m_SettingGain = new SettingGAIN();
        private SettingAlgorithm m_SettingAlgorithm = new SettingAlgorithm();
        private SettingPriority m_settingPriority;// = new SettingPriority();
        #endregion

        public SettingWindow(MainWindow aMainWindow)
        {
            m_ptrMainWindow = aMainWindow;
            m_settingPriority = new SettingPriority(MainWindow.NG_Info);
            InitializeComponent();
            InitialDialog();
            InitializeEvent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        #region Initialize
        public void InitialDialog()
        {
            tabGeneral.Content = m_SettingGeneral;
            tabAutoNG.Content = m_SettingAutoNG;
            tabRGB.Content = m_SettingGain;
            tabAlgorithm.Content = m_SettingAlgorithm;
            tabPriority.Content = m_settingPriority;
            this.tbSetting.SelectedIndex = 0;
            if (MainWindow.bMCR_TEST_SAVE) { radUseMCRtestSaveYes.IsChecked = true; radUseMCRtestSaveNo.IsChecked = false; }
            else { radUseMCRtestSaveYes.IsChecked = false; radUseMCRtestSaveNo.IsChecked = true; }         
        }

        private void InitializeEvent()
        {
            this.btnSave.Click += btnSave_Click;
            this.btnClose.Click += btnClose_Click;
            this.btnDetailXML.Click += btnDetailXML_Click;
            this.KeyDown += SettingWindow_KeyDown;
            this.radUseMCRtestSaveYes.Click += radUseMCRtestSave_Checked;
            this.radUseMCRtestSaveNo.Click += radUseMCRtestSave_Checked;
        }
        #endregion

        private void SettingWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                CloseWithOK();
            }
            else if (e.Key == Key.Escape)
            {
                CloseWithCancel();
            }
        }
        private void radUseMCRtestSave_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == radUseMCRtestSaveYes)
            {
                MainWindow.bMCR_TEST_SAVE = true;
            }
            else
            {
                MainWindow.bMCR_TEST_SAVE = false;
            }
            m_ptrMainWindow.m_Registry.Write("bMCR_TEST_SAVE", MainWindow.bMCR_TEST_SAVE);
        }

        #region Event handlers.
        private void btnDetailXML_Click(object sender, RoutedEventArgs e)
        {
            SettingsXMLWindow xmlWindow = new SettingsXMLWindow();
            xmlWindow.ShowDialog();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            CloseWithOK();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseWithCancel();
        }
        #endregion

        private void CloseWithOK()
        {
            m_SettingGain.Save();
            m_SettingGeneral.Save();
            m_settingPriority.Save();
            m_ptrMainWindow.UpdatePriority(m_settingPriority.NG);
            MainWindow.Setting.Save();

            this.DialogResult = true;
        }

        private void CloseWithCancel()
        {
            this.DialogResult = false;
        }
    }
}
