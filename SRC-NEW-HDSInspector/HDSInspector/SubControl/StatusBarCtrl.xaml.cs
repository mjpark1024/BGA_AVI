using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Common;
using RVS.Generic;

namespace HDSInspector
{
    public partial class StatusBarCtrl : UserControl
    {
        private MainWindow m_ptrMainWindow;
        public bool DBStatus { get; set; }

        public StatusBarCtrl()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }

        public void SetMainWindowPtr(MainWindow mainWindow)
        {
            this.m_ptrMainWindow = mainWindow;
        }

        private void InitializeEvent()
        {

        }

        private void InitializeDialog()
        {
            this.chkVerify.IsChecked = false;
            this.chkVerify.IsEnabled = false;
            this.txtVerify.Foreground = new SolidColorBrush(Colors.LightGray);

        }
    }
}
