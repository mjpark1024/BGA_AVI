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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PCS.ModelTeaching.OfflineTeaching;
using System.Diagnostics;
using HDSInspector.SubWindow;
using System.Threading;

namespace HDSInspector.SubControl
{
    public partial class OffLineTeaching : UserControl
    {
        private readonly TeachingWindow m_ptrTeachingWindow;
        private readonly MTSManager m_ptrMTSManager;

        public OffLineTeaching(TeachingWindow aTeachingWindow)
        {
            InitializeComponent();
            InitializeEvent();

            m_ptrTeachingWindow = aTeachingWindow;
            m_ptrMTSManager = aTeachingWindow.MtsManager;
            if (m_ptrMTSManager != null)
            {
                this.txtVisionPath.Text = m_ptrMTSManager.VisionLocation;
            }
        }

        private void InitializeEvent()
        {
            this.btnFileOpen.Click += btnFileOpen_Click;
            this.btnConnectVision.Click += btnConnectVision_Click;
            this.btnRunVision.Click += btnRunVision_Click;
            this.btnKillVision.Click += btnKillVision_Click;

            this.btnChangeModel.Click += btnChangeModel_Click;
            this.btnSettingVision.Click += btnSettingVision_Click;
        }

        private void btnFileOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = "*.exe";
            dlg.Filter = "Execute files (*.exe)|*.exe";

            if ((bool)dlg.ShowDialog().GetValueOrDefault())
            {
                try
                {
                    txtVisionPath.Text = dlg.FileName.Replace(@"\", "/");
                    m_ptrMTSManager.VisionLocation = dlg.FileName.Replace(@"\", "/");
                }
                catch
                {
                    Debug.WriteLine("MTS:Exception occured in btnFileOpen_Click(OffLineTeaching.xaml.cs)");
                }
            }
        }

        private void btnConnectVision_Click(object sender, RoutedEventArgs e)
        {
            if (m_ptrTeachingWindow.TeachingViewer.SelectedViewer.ID != 3)
            {
                m_ptrMTSManager.Connect();
                Thread.Sleep(500);
                m_ptrMTSManager.InitVision(MainWindow.Setting.SubSystem.IS, m_ptrTeachingWindow.TeachingViewer.SelectedViewer.ID);
                m_ptrTeachingWindow.TeachingViewer.SelectedViewer.IsGrabDone = false;
            }
        }

        private void btnRunVision_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (System.IO.File.Exists(m_ptrMTSManager.VisionLocation))
                {
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = m_ptrMTSManager.VisionLocation;
                    si.UseShellExecute = false;
                    si.RedirectStandardInput = true;
                    Process run = new Process();
                    run.StartInfo = si;
                    run.Start();
                }
                else
                {
                    MessageBox.Show("파일이 존재하지 않습니다.", "Information");
                }
            }
            catch
            {
                Debug.WriteLine("MTS:Exception occured in btnRunVision_Click(OffLineTeaching.xaml.cs)");
            }
        }

        private void btnKillVision_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process run = new Process();
                ProcessStartInfo si = new ProcessStartInfo();
                si.FileName = "taskkill.exe";
                si.UseShellExecute = false;
                si.RedirectStandardInput = true;
                si.Arguments = "/f /im IS.exe";
                run.StartInfo = si;
                run.Start();
            }
            catch
            {
                MessageBox.Show("수동으로 비전 프로그램을 종료해주시기 바랍니다.", "Error");
                Debug.WriteLine("MTS:Exception occured in btnKillVision_Click(OffLineTeaching.xaml.cs)");
            }
        }

        private void btnChangeModel_Click(object sender, RoutedEventArgs e)
        {
            if (m_ptrTeachingWindow != null)
            {
                m_ptrTeachingWindow.SelectOfflineModel();
            }
        }

        private void btnSettingVision_Click(object sender, RoutedEventArgs e)
        {
            if (m_ptrTeachingWindow.TeachingViewer.SelectedViewer.ID != 3)
            {
                VisionSettingWindow visionSettingWindow = new VisionSettingWindow(m_ptrTeachingWindow);
                if ((bool)visionSettingWindow.ShowDialog())
                {
                    m_ptrMTSManager.InitVision(MainWindow.Setting.SubSystem.IS, m_ptrTeachingWindow.TeachingViewer.SelectedViewer.ID);
                    m_ptrTeachingWindow.TeachingViewer.SelectedViewer.IsGrabDone = false;
                }
            }
        }
    }
}
