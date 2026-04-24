using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common;

namespace HDSInspector
{
    public partial class SettingSAP : UserControl
    {
        private MainWindow m_ptrMainWindow;

        public SettingSAP()
        {
            InitializeComponent();
            InitializeDialog();
            svDefectCodeViewer.PreviewMouseWheel += lbKeyPadNameList_MouseWheel;
        }

        void lbKeyPadNameList_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double fMouseScroll = svDefectCodeViewer.VerticalOffset;

            fMouseScroll -= e.Delta / 3;

            if (fMouseScroll < 0)
            {
                fMouseScroll = 0;
                svDefectCodeViewer.ScrollToVerticalOffset(fMouseScroll);
            }
            else if (fMouseScroll > svDefectCodeViewer.ScrollableHeight)
            {
                fMouseScroll = svDefectCodeViewer.ScrollableHeight;
                svDefectCodeViewer.ScrollToEnd();
            }
            else
            {
                this.svDefectCodeViewer.ScrollToVerticalOffset(fMouseScroll);
            }
        }


        private void InitializeDialog()
        {
            txtSAPPath.Text = MainWindow.Setting.General.POP_Path;
            txtProcessCode.Text = MainWindow.Setting.Job.ProcessCode;

            for (int i = 0; i < 9; i++ )
            {
                SettingSAPData data = new SettingSAPData();
                data.Number = (i + 1).ToString();
                data.Name = Settings.GetSettings().SubSystem.KeyPadName[i];
                data.Code = Settings.GetSettings().SubSystem.DefectCode[i];
                
                this.lbDefectCodeList.Items.Add(data);
            }
        }

        public void SetMainWindow(MainWindow aMainWindow)
        {
            m_ptrMainWindow = aMainWindow;
        }

        public void Save()
        {
            MainWindow.Setting.General.POP_Path = txtSAPPath.Text;
            MainWindow.Setting.Job.ProcessCode = txtProcessCode.Text;

            string[] KeyPadName = new string[lbDefectCodeList.Items.Count];
            string[] DefectCode = new string[lbDefectCodeList.Items.Count];
            for (int i = 0; i < lbDefectCodeList.Items.Count; i++)
            {
                SettingSAPData data = lbDefectCodeList.Items[i] as SettingSAPData;
                if (data != null)
                {
                    KeyPadName[i] = data.Name;
                    DefectCode[i] = data.Code;
                }
            }
            Settings.GetSettings().SubSystem.KeyPadName = KeyPadName;
            Settings.GetSettings().SubSystem.DefectCode = DefectCode;
        }

        private class SettingSAPData : NotifyPropertyChanged
        {
            private string m_Name = "-";
            private string m_Number = "-";
            private string m_Code = "-";

            public string Name { get { return m_Name; } set { m_Name = value; Notify("Name"); } }
            public string Number { get { return m_Number; } set { m_Number = value; Notify("Number"); } }
            public string Code { get { return m_Code; } set { m_Code = value; Notify("Code"); } }
        }
    }
}
