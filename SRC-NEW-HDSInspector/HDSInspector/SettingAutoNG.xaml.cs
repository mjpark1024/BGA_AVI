using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common;
using System.Data;
using Common.DataBase;

namespace HDSInspector
{
    /// <summary>
    /// Interaction logic for SettingAutoNG.xaml
    /// </summary>
    public partial class SettingAutoNG : UserControl
    {
        public SettingAutoNG()
        {
            InitializeComponent();
            InitializeDialog();
        }

        /// <summary>   Initializes the dialog. </summary>
        private void InitializeDialog()
        {
            txtRate.Text = MainWindow.Setting.General.RejectRate.ToString();

            string str = MainWindow.Setting.Job.AutoNG;
            string[] s = str.Split(',');
            if (s.Length < 11)
            {
                return;
            }
            cbAlign.IsChecked   = (s[0] == "1") ? true : false;
            cbRaw.IsChecked     = (s[1] == "1") ? true : false;  
            cbOpen.IsChecked    = (s[2] == "1") ? true : false;
            cbShort.IsChecked   = (s[3] == "1") ? true : false;
            cbBP.IsChecked      = (s[4] == "1") ? true : false;
            cbBall.IsChecked    = (s[5] == "1") ? true : false;
            cbPinhole.IsChecked = (s[6] == "1") ? true : false;
            cbPSR.IsChecked     = (s[7] == "1") ? true : false;
            cbBurr.IsChecked    = (s[8] == "1") ? true : false;
            cbCrack.IsChecked   = (s[9] == "1") ? true : false;
            cbVH.IsChecked      = (s[10] == "1") ? true : false;
            cbVia.IsChecked     = (s[11] == "1") ? true : false;
            if (s.Length == 13)
                cbOuter.IsChecked = (s[12] == "1") ? true : false;
            else
                cbOuter.IsChecked = true;
        }
    }
}
