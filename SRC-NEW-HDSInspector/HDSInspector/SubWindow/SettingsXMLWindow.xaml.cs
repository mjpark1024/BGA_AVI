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
using System.IO;
using Common;
using System.Windows.Input;

namespace HDSInspector.SubWindow
{
    /// <summary>   Window for setting the tings xml.  </summary>
    public partial class SettingsXMLWindow : Window
    {
        /// <summary>   Initializes a new instance of the SettingsXMLWindow class. </summary>
        public SettingsXMLWindow()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();            
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        /// <summary>   Initializes the event. </summary>
        private void InitializeEvent()
        {
            this.btnSave.Click += btnSave_Click;
            this.btnClose.Click += btnClose_Click;
            this.KeyDown += SettingsXMLWindow_KeyDown;
        }

        private void SettingsXMLWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        /// <summary>   Initializes the dialog. </summary>
        private void InitializeDialog()
        {
            FileStream fileStream = null;
            StreamReader streamReader = null;
            try
            {
                fileStream = File.Open("../config/Settings.xml", FileMode.Open, FileAccess.ReadWrite);
                streamReader = new StreamReader(fileStream, Encoding.Default);
                txtSettingsXML.Text = streamReader.ReadToEnd();

                fileStream.Close();
                fileStream = null;
                streamReader.Close();
                streamReader = null;
            }
            catch
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }

                if (streamReader != null)
                {
                    streamReader.Close();
                }
            }
        }

        /// <summary>   Event handler. Called by btnSave for click events. </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            CloseWithOK();
        }

        /// <summary>   Event handler. Called by btnClose for click events. </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseWithCancel();
        }

        private void CloseWithOK()
        {
            MainWindow.Log("PCS", SeverityLevel.INFO, "환경 설정이 변경되었습니다.");
            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            try
            {
                fileStream = new FileStream("../config/Settings.xml", FileMode.Create, FileAccess.Write);
                streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(txtSettingsXML.Text);
                streamWriter.Flush();

                fileStream.Close();
                fileStream = null;
                streamWriter.Close();
                streamWriter = null;
            }
            catch
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }

                if (streamWriter != null)
                {
                    streamWriter.Close();
                }
            }

            MainWindow.Setting.Load();
            this.DialogResult = true;
        }

        private void CloseWithCancel()
        {
            this.DialogResult = false;
        }
    }
}
