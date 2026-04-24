using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common;
using RMS.Generic.UserManagement;

namespace RVS
{
    public partial class SettingRVS : UserControl
    {
        private MainWindow m_MainWindow;

        public SettingRVS()
        {
            InitializeComponent();
            InitializeEvent();
        }

        public void SetMainWindow(MainWindow aMainWindow)
        {
            m_MainWindow = aMainWindow;
        }

        private void InitializeDialog()
        {
            string szIP;
            string[] words;

            if(Settings.GetSettings().SubSystem.RVSIP != null)
            {
                szIP = Settings.GetSettings().SubSystem.RVSIP;
                
                if(szIP == "localhost")
                {
                    txtIP0.Text = "127";
                    txtIP1.Text = "0";
                    txtIP2.Text = "0";
                    txtIP3.Text = "1";
                }
                else
                {
                    words = szIP.Split('.');
                    txtIP0.Text = words[0];
                    txtIP1.Text = words[1];
                    txtIP2.Text = words[2];
                    txtIP3.Text = words[3];
                }
            }

            if (Settings.GetSettings().SubSystem.EquipName != null && Settings.GetSettings().SubSystem.EquipIP != null)
            {
                for (int i = 0; i < Settings.GetSettings().SubSystem.ConnectionCount; i++)
                {
                    SettingRVSData data = new SettingRVSData();
                    data.Name = Settings.GetSettings().SubSystem.EquipName[i];
                    data.Number = (i + 1).ToString();

                    szIP = Settings.GetSettings().SubSystem.EquipIP[i];
                    if (szIP == "localhost")
                    {
                        data.IP_0 = "127";
                        data.IP_1 = "0";
                        data.IP_2 = "0";
                        data.IP_3 = "1";
                    }
                    else
                    {
                        words = szIP.Split('.');
                        try
                        {
                            data.IP_0 = words[0];
                            data.IP_1 = words[1];
                            data.IP_2 = words[2];
                            data.IP_3 = words[3];
                        }
                        catch 
                        {
                            data.IP_0 = "127";
                            data.IP_1 = "0";
                            data.IP_2 = "0";
                            data.IP_3 = "1";
                        }
                    }
                    lbEquipSettingList.Items.Add(data);
                }
            }

            if (Settings.GetSettings().SubSystem.KeyPadName != null)
            {
                for (int i = 0; i < 11; i++)
                {
                    SettingRVSData data = new SettingRVSData();
                    data.Name = Settings.GetSettings().SubSystem.KeyPadName[i];
                    data.Number = i.ToString();
                    if (i == 10)
                        data.Number = ".";

                    lbKeyPadNameList.Items.Add(data);
                }
            }
            
            txtInner.Text = Settings.GetSettings().SubSystem.InnerResolution.ToString();
            txtCenter.Text = Settings.GetSettings().SubSystem.CenterResolution.ToString();
            txtOuter.Text = Settings.GetSettings().SubSystem.OuterResolution.ToString();

            if (UserManager.CurrentUser.Authority.AuthCode == "0063")
            {
                EquipGrid.IsEnabled = false;
                KeypadGrid.IsEnabled = false;
            }
        }

        private void InitializeEvent()
        {
            this.Loaded += new RoutedEventHandler(SettingWindow_Loaded);
            this.btnAddEquip.Click += new RoutedEventHandler(btnAddEquip_Click);
            this.btnDeleteEquip.Click += new RoutedEventHandler(btnDeleteEquip_Click);

            this.lbEquipSettingList.PreviewMouseWheel += new MouseWheelEventHandler(lbEquipSettingList_MouseWheel);
            this.lbKeyPadNameList.PreviewMouseWheel += new MouseWheelEventHandler(lbKeyPadNameList_MouseWheel);
        }

        void lbKeyPadNameList_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double fMouseScroll = svDataNameViewer.VerticalOffset;

            fMouseScroll -= e.Delta / 3;

            if (fMouseScroll < 0)                                   
            {
                fMouseScroll = 0;
                svDataNameViewer.ScrollToVerticalOffset(fMouseScroll);
            }
            else if (fMouseScroll > svDataNameViewer.ScrollableHeight)   
            {
                fMouseScroll = svDataNameViewer.ScrollableHeight;
                svDataNameViewer.ScrollToEnd();
            }
            else                                                    
            {
                this.svDataNameViewer.ScrollToVerticalOffset(fMouseScroll);
            }
        }

        void lbEquipSettingList_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double fMouseScroll = svSettingIPViewer.VerticalOffset;

            fMouseScroll -= e.Delta / 3;

            if (fMouseScroll < 0)                                   
            {
                fMouseScroll = 0;
                svSettingIPViewer.ScrollToVerticalOffset(fMouseScroll);
            }
            else if (fMouseScroll > svSettingIPViewer.ScrollableHeight)   
            {
                fMouseScroll = svSettingIPViewer.ScrollableHeight;
                svSettingIPViewer.ScrollToEnd();
            }
            else                                                   
            {
                this.svSettingIPViewer.ScrollToVerticalOffset(fMouseScroll);
            }
        }

        void SettingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeDialog();
        }

        void btnDeleteEquip_Click(object sender, RoutedEventArgs e)
        {
            if (m_MainWindow.IsVerifying == true)
            {
                MessageBox.Show("Verify 중에는 추가 또는 삭제 할수 없습니다.");
                return;
            }

            if (this.lbEquipSettingList.Items.Count == 0)
                return;
            else
            {
                this.lbEquipSettingList.Items.RemoveAt(this.lbEquipSettingList.Items.Count - 1);
            }

            svSettingIPViewer.ScrollToEnd();
        }

        void btnAddEquip_Click(object sender, RoutedEventArgs e)
        {
            if (m_MainWindow.IsVerifying == true)
            {
                MessageBox.Show("Verify 중에는 추가 또는 삭제 할수 없습니다.");
                return;
            }

            if (this.lbEquipSettingList.Items.Count < Settings.GetSettings().SubSystem.ConnectionCount)
            {
                SettingRVSData data = new SettingRVSData();
                data.Name = Settings.GetSettings().SubSystem.EquipName[this.lbEquipSettingList.Items.Count];
                data.Number = (this.lbEquipSettingList.Items.Count + 1).ToString();

                string szIP = Settings.GetSettings().SubSystem.EquipIP[this.lbEquipSettingList.Items.Count];
                string[] words = szIP.Split('.');
                data.IP_0 = words[0];
                data.IP_1 = words[1];
                data.IP_2 = words[2];
                data.IP_3 = words[3];

                this.lbEquipSettingList.Items.Add(data);
            }
            else
            {
                SettingRVSData data = new SettingRVSData();
                data.Name = "Equip " + (this.lbEquipSettingList.Items.Count + 1).ToString();
                data.Number = (this.lbEquipSettingList.Items.Count + 1).ToString();

                data.IP_0 = "127";
                data.IP_1 = "0";
                data.IP_2 = "0";
                data.IP_3 = "1";

                this.lbEquipSettingList.Items.Add(data);
            }
            svSettingIPViewer.ScrollToEnd();
        }
        
        public void SaveRVS()
        {
            Settings.GetSettings().SubSystem.RVSIP = txtIP0.Text + "." + txtIP1.Text + "." + txtIP2.Text + "." + txtIP3.Text;
            Settings.GetSettings().SubSystem.ConnectionCount = this.lbEquipSettingList.Items.Count;
            Settings.GetSettings().SubSystem.InnerResolution = txtInner.Text == "" ? 0 : Convert.ToInt32(txtInner.Text);
            Settings.GetSettings().SubSystem.CenterResolution = txtCenter.Text == "" ? 0 : Convert.ToInt32(txtCenter.Text);
            Settings.GetSettings().SubSystem.OuterResolution = txtOuter.Text == "" ? 0 : Convert.ToInt32(txtOuter.Text);

            string[] EquipName = new string[lbEquipSettingList.Items.Count];
            string[] EquipIP = new string[lbEquipSettingList.Items.Count];
            for (int i = 0; i < lbEquipSettingList.Items.Count; i++)
            {
                SettingRVSData data = lbEquipSettingList.Items[i] as SettingRVSData;
                if (data != null)
                {
                    EquipName[i] = data.Name;
                    EquipIP[i] = data.IP_0 + "." + data.IP_1 + "." + data.IP_2 + "." + data.IP_3;
                }
            }
            Settings.GetSettings().SubSystem.EquipName = EquipName;
            Settings.GetSettings().SubSystem.EquipIP = EquipIP;

            string[] KeyPadName = new string[lbKeyPadNameList.Items.Count];
            for (int i = 0; i < lbKeyPadNameList.Items.Count; i++)
            {
                SettingRVSData data = lbKeyPadNameList.Items[i] as SettingRVSData;
                if (data != null)
                {
                    KeyPadName[i] = data.Name;
                }
            }
            Settings.GetSettings().SubSystem.KeyPadName = KeyPadName;
            Settings.GetSettings().SubSystem.Save();
        }

        public bool CheckValidate()
        {
            int EquipCount = this.lbEquipSettingList.Items.Count;

            List<string> name = new List<string>();
            for (int i = 0; i < lbEquipSettingList.Items.Count; i++)
            {
                SettingRVSData data = lbEquipSettingList.Items[i] as SettingRVSData;
                if (data != null)
                {
                    name.Add(data.Name);
                }
            }
            IEnumerable<string> distinctName = name.Distinct();
            
            if( EquipCount > distinctName.Count<string>())
                return false;

            return true;
        }
    }

    public class SettingRVSData : NotifyPropertyChanged
    {
        private string m_Name = "-";
        private string m_Number = "0";
        private string m_IP_0 = "127";
        private string m_IP_1 = "0";
        private string m_IP_2 = "0";
        private string m_IP_3 = "1";

        public string Name { get { return m_Name; } set { m_Name = value; Notify("Name"); } }
        public string Number { get { return m_Number; } set { m_Number = value; Notify("Number"); } }
        public string IP_0 { get { return m_IP_0; } set { m_IP_0 = value; Notify("IP_0"); } }
        public string IP_1 { get { return m_IP_1; } set { m_IP_1 = value; Notify("IP_1"); } }
        public string IP_2 { get { return m_IP_2; } set { m_IP_2 = value; Notify("IP_2"); } }
        public string IP_3 { get { return m_IP_3; } set { m_IP_3 = value; Notify("IP_3"); } }
    }
}
