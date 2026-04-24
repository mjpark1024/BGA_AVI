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
using System.Diagnostics;
using PCS.ModelTeaching.OfflineTeaching;

namespace HDSInspector.SubWindow
{
    public partial class VisionSettingWindow : Window
    {
        private readonly TeachingWindow m_ptrTeachingWindow;
        private readonly MTSManager m_ptrMTSManager;

        public VisionSettingWindow(TeachingWindow aTeachingWindow)
        {
            InitializeComponent();
            InitializeEvent();

            m_ptrTeachingWindow = aTeachingWindow;
            m_ptrMTSManager = aTeachingWindow.MtsManager;
            if (m_ptrMTSManager != null)
            {
                txtVision1ImagePath.Text = m_ptrMTSManager.Vision1ImagePath;
                txtVision2ImagePath.Text = m_ptrMTSManager.Vision2ImagePath;
                txtVision3ImagePath.Text = m_ptrMTSManager.Vision3ImagePath;
            }
            txtCamWidth.Text = MainWindow.Setting.SubSystem.IS.CameraWidth[0].ToString();
            txtCamHeight.Text = MainWindow.Setting.SubSystem.IS.CameraHeight[0].ToString();
        }

        private void InitializeEvent()
        {
            this.btnFileOpen1.Click += btnFileOpen_Click;
            this.btnFileOpen2.Click += btnFileOpen_Click;
            this.btnFileOpen3.Click += btnFileOpen_Click;

            this.btnOK.Click += (s, e) => CloseWithOK();
            this.btnCancel.Click += (s, e) => { this.DialogResult = false; this.Close(); };
            this.KeyDown += VisionSettingWindow_KeyDown;
        }

        private void VisionSettingWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
            if (e.Key == Key.Return)
            {
                CloseWithOK();
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        private void btnFileOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = "*.bmp";
            dlg.Filter = "Reference Image files (*.bmp)|*.bmp";

            if ((bool)dlg.ShowDialog().GetValueOrDefault())
            {
                try
                {
                    if (sender == btnFileOpen1)
                    {
                        txtVision1ImagePath.Text = dlg.FileName.Replace(@"\", "/");
                        return;
                    }
                    else if (sender == btnFileOpen2)
                    {
                        txtVision2ImagePath.Text = dlg.FileName.Replace(@"\", "/");
                        return;
                    }
                    else if (sender == btnFileOpen3)
                    {
                        txtVision3ImagePath.Text = dlg.FileName.Replace(@"\", "/");
                        return;
                    }
                }
                catch
                {
                    Debug.WriteLine("MTS:Exception occured in btnFileOpen_Click(VisionSettingWindow.xaml.cs)");
                }
            }
        }

        private void CloseWithOK()
        {
            m_ptrMTSManager.Vision1ImagePath = txtVision1ImagePath.Text;
            m_ptrMTSManager.Vision2ImagePath = txtVision2ImagePath.Text;
            m_ptrMTSManager.Vision3ImagePath = txtVision3ImagePath.Text;

            MainWindow.Setting.SubSystem.IS.CameraWidth[0] = Convert.ToInt32(txtCamWidth.Text);
            MainWindow.Setting.SubSystem.IS.CameraHeight[0] = Convert.ToInt32(txtCamHeight.Text);
            MainWindow.Setting.SubSystem.Save();

            this.DialogResult = true;
            this.Close();
        }
    }
}
