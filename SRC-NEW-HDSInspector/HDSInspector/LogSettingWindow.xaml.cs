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
 * @file  LogSetting.xaml.cs
 * @brief . 
 *  behind code of LogSetting.xaml
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.05.09
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.05.09 First creation.
 */

using System;
using System.Windows;
using System.Windows.Input;
using Common;
using System.IO;
using System.Diagnostics;

namespace HDSInspector
{
    /** 
     * @brief interation with LogSetting.xaml
     * @author suoow2
     * @date 2011.05.09
     */
    public partial class LogSettingWindow : Window
    {
        /**
         * @var m_LogSetting
         * @brief log setting value.
        */
        private Generals m_LogSetting;

        /**
         * @fn LoggerOption(LoggerOption logSetting)
         * @param [in] logSetting - log setting value.
         * @brief default constructor.
         */
        public LogSettingWindow(Generals logSetting)
        {
            m_LogSetting = logSetting;

            InitializeComponent();
            InitializeComboBox();
            InitializeDialog();
            InitializeEvent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        /**
         * @fn InitializeEvent()
         * @brief initialize events.
         */
        private void InitializeEvent()
        {
            this.Closing += LogSettingWindow_Closing;
            this.btnOK.Click += btnOK_Click;
            this.btnCancel.Click += btnCancel_Click;
            this.btnLogDialog.Click += btnLogDialog_Click;
            this.radSaveLogYes.Click += radSaveLogYes_Click;
            this.radSaveLogNo.Click += radSaveLogNo_Click;

            this.KeyDown += LogSettingWindow_KeyDown;
        }

        void btnLogDialog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Directory.GetCurrentDirectory() + "\\..\\log\\");
            }
            catch { }
        }

        private void LogSettingWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnOK_Click(btnOK, new RoutedEventArgs());
            }
            else if (e.Key == Key.Escape)
            {
                btnCancel_Click(btnCancel, new RoutedEventArgs());
            }
        }

        void LogSettingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        #region radio button event handlers.
        /**
         * @fn radSaveLogYes_Click(object sender, RoutedEventArgs e)
         * @brief it changes txtSaveLogHelper tip.
         */
        private void radSaveLogYes_Click(object sender, RoutedEventArgs e)
        {
            this.txtSaveLogHelper.Text = ResourceStringHelper.GetInformationMessage("L001");
            
            this.txtKeepDate.IsEnabled = true;
            this.cbLocalSaveLevelSelector.IsEnabled = true;
        }

        /**
         * @fn radSaveLogNo_Click(object sender, RoutedEventArgs e)
         * @brief it changes txtSaveLogHelper tip.
         */
        private void radSaveLogNo_Click(object sender, RoutedEventArgs e)
        {
            this.txtSaveLogHelper.Text = ResourceStringHelper.GetInformationMessage("L002");

            this.txtKeepDate.IsEnabled = false;
            this.cbLocalSaveLevelSelector.IsEnabled = false;
        }
        #endregion

        #region ok & cancel button.
        /**
         * @fn btnOK_Click(object sender, RoutedEventArgs e)
         * @brief ok button click event handler.
         */
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            int integerValue;
            if(!Int32.TryParse(txtKeepDate.Text, out integerValue))
            {
                MessageBox.Show(ResourceStringHelper.GetErrorMessage("L001"), "Error");
                txtKeepDate.Text = m_LogSetting.LogKeepDate.ToString();
                return;
            }
                        
            this.DialogResult = true;
            this.Hide();
        }

        /**
         * @fn btnCancel_Click(object sender, RoutedEventArgs e)
         * @brief cancel button click event handler.
         */
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
        #endregion

        #region initialize dialog.
        /**
         * @fn InitializeDialog()
         * @brief initialize logsetting dialog's ui components.
         */
        private void InitializeDialog()
        {
            if(m_LogSetting.LogSave)
            {
                this.radSaveLogYes.IsChecked = true;
            }
            else
            {
                this.radSaveLogNo.IsChecked = true;
                this.txtSaveLogHelper.Text = ResourceStringHelper.GetInformationMessage("L002");

                this.txtKeepDate.IsEnabled = false;
                this.cbLocalSaveLevelSelector.IsEnabled = false;
            }

            this.txtKeepDate.Text = m_LogSetting.LogKeepDate.ToString();
            this.cbLocalSaveLevelSelector.SelectedIndex = (int) m_LogSetting.LogLevel;
            this.cbDisplayLevelSelector.SelectedIndex = (int) m_LogSetting.LogDPLevel;
        }

        /**
         * @fn InitializeComboBox()
         * @brief initialize comboboxes.
         */
        private void InitializeComboBox()
        {
            this.cbLocalSaveLevelSelector.ItemsSource = Enum.GetNames(typeof(SeverityLevel));
            this.cbDisplayLevelSelector.ItemsSource = Enum.GetNames(typeof(SeverityLevel));
        }
        #endregion
    }
}
