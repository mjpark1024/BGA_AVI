using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace HDSInspector.SubWindow
{
    public partial class BusyWindow : Window
    {
        private int m_nTickCount = 0;
        private readonly MainWindow m_ptrMainWindow;
        private readonly DispatcherTimer m_timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000) };

        public BusyWindow()
        {
            InitializeComponent();
            m_ptrMainWindow = Application.Current.MainWindow as MainWindow;
            BusyBar.IsBusy = true;
            this.Loaded += BusyWindow_Loaded;
        }

        private void BusyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            m_timer.Tick += timer_Tick;
            m_timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!m_ptrMainWindow.InspectionMonitoringCtrl.InspectionThreadStarted || m_nTickCount > 20)
            {
                m_timer.Stop();
                this.Close();
            }
            m_nTickCount++;
        }
    }
}
