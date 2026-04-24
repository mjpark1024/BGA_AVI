using System;
using System.Windows;
using System.Windows.Controls;
using Common;
using Common.DataBase;
using System.Data;
using PCS.ModelTeaching;

namespace HDSInspector
{
    public partial class SettingGeneral : UserControl, ISetting
    {
        private MainWindow m_ptrMainWindow = (MainWindow)Application.Current.MainWindow;

        public SettingGeneral()
        {
            InitializeComponent();
            InitializeDialog();
        }
       
        private void InitializeDialog()
        {
            // 장비
            SetMachineList();
            cmbMachine.SelectedItem = MainWindow.Setting.General.MachineName;

            txtIP.IPAddress = MainWindow.Setting.General.MachineIP;
            txtDBIP.IPAddress = MainWindow.Setting.General.DBIP;
            txtVerifyIP.IPAddress = MainWindow.Setting.General.VRSDBIP;

            // Page Delay.
            txtDelay1.Text = MainWindow.Setting.SubSystem.IS.CamPageDelay[0].ToString();
            txtDelay2.Text = MainWindow.Setting.SubSystem.IS.CamPageDelay[1].ToString();
            txtDelay3.Text = MainWindow.Setting.SubSystem.IS.CamPageDelay[2].ToString();
            txtCamYPosition.Text = MainWindow.Setting.Job.Cam1Position.ToString();
           
            // PLC
            if (MainWindow.Setting.SubSystem.PLC.UsePLC)
                radUsePLCYes.IsChecked = true;
            else
                radUsePLCNo.IsChecked = true;
            txtPLCIP.IPAddress = MainWindow.Setting.SubSystem.PLC.IP;
            txtPLCPort.Text = MainWindow.Setting.SubSystem.PLC.Port.ToString();

            // Counter
            if (MainWindow.Setting.SubSystem.ENC.UseEncoder)
                radUseCounterYes.IsChecked = true;
            else
                radUseCounterNo.IsChecked = true;
            txtCounterLow1.Text = MainWindow.Setting.SubSystem.ENC.Low[0].ToString();
            txtCounterHigh1.Text = MainWindow.Setting.SubSystem.ENC.High[0].ToString();
            txtCounterLow2.Text = MainWindow.Setting.SubSystem.ENC.Low[1].ToString();
            txtCounterHigh2.Text = MainWindow.Setting.SubSystem.ENC.High[1].ToString();
            txtCounterLow3.Text = MainWindow.Setting.SubSystem.ENC.Low[2].ToString();
            txtCounterHigh3.Text = MainWindow.Setting.SubSystem.ENC.High[2].ToString();
            txtResolution.Text = MainWindow.Setting.SubSystem.Laser.CamResolution.ToString();
        }

        private void SetMachineList()
        {
            if (m_ptrMainWindow.DBStatus)
            {
                String strQuery = String.Format("Select machine_code, machine_name from machine_info where use_yn = 1");

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        String strCode = dataReader.GetValue(0).ToString();
                        String strName = dataReader.GetValue(1).ToString();
                        ComboBoxItem lbItem = new ComboBoxItem();
                        lbItem.Content = strName;
                        lbItem.Tag = strCode;
                        cmbMachine.Items.Add(lbItem);
                    }
                    dataReader.Close();
                }
                else
                {
                    cmbMachine.Items.Add(MainWindow.Setting.General.MachineName);
                    cmbMachine.SelectedIndex = 0;
                }
            }
            else
            {
                cmbMachine.Items.Add(MainWindow.Setting.General.MachineName);
                cmbMachine.SelectedIndex = 0;
            }


            for (int i = 0; i < cmbMachine.Items.Count; i++)
            {
                ComboBoxItem cmbItem = cmbMachine.Items.GetItemAt(i) as ComboBoxItem;
                if (cmbItem != null)
                {
                    if (cmbItem.Content.ToString() == MainWindow.Setting.General.MachineName)
                    {
                        cmbMachine.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        public bool IsValidate()
        {
            if (string.IsNullOrEmpty(txtIP.IPAddress))
            {
                MessageBox.Show("IP 주소를 확인 바랍니다.", "Information");
                txtIP.Focus();
                return false;
            }

            return true;
        }

        public bool CheckSave()
        {
            return false;
        }

        public void Save()
        {
            // 장비
            if (cmbMachine.SelectedItem != null)
            {
                ComboBoxItem lbItem = cmbMachine.SelectedItem as ComboBoxItem;
                if (lbItem != null)
                {
                    if (MainWindow.Setting.General.MachineName != lbItem.Content.ToString())
                    {
                        string aszNewMachineCode = string.Format("{0:D4}", Convert.ToInt32(lbItem.Content.ToString().Substring(3, 3)));
                        TeachingDataManager teachingDataManager = new TeachingDataManager();
                        teachingDataManager.CopySectionDataToMachine(MainWindow.Setting.General.MachineCode, aszNewMachineCode);
                        teachingDataManager.CopyROIDataToMachine(MainWindow.Setting.General.MachineCode, aszNewMachineCode);
                    }
                    MainWindow.Setting.General.MachineName = lbItem.Content.ToString();
                }
            }
            // Page Delay.
            MainWindow.Setting.SubSystem.IS.CamPageDelay[0] = Convert.ToInt32(txtDelay1.Text);
            MainWindow.Setting.SubSystem.IS.CamPageDelay[1] = Convert.ToInt32(txtDelay2.Text);
            MainWindow.Setting.SubSystem.IS.CamPageDelay[2] = Convert.ToInt32(txtDelay3.Text);
            MainWindow.Setting.Job.Cam1Position = Convert.ToDouble(txtCamYPosition.Text);
            MainWindow.Setting.SubSystem.Laser.CamResolution = Convert.ToDouble(txtResolution.Text);
            MainWindow.Setting.General.MachineIP = txtIP.IPAddress;
            MainWindow.Setting.General.DBIP = txtDBIP.IPAddress;
            MainWindow.Setting.General.VRSDBIP = txtVerifyIP.IPAddress;

            // PLC
            if (radUsePLCYes.IsChecked == true)
                MainWindow.Setting.SubSystem.PLC.UsePLC = true;
            else
                MainWindow.Setting.SubSystem.PLC.UsePLC = false;
            MainWindow.Setting.SubSystem.PLC.Port = Convert.ToInt32(txtPLCPort.Text);

            MainWindow.Setting.SubSystem.ENC.UseEncoder = (bool)radUseCounterYes.IsChecked;
            // Counter

            MainWindow.Setting.SubSystem.ENC.Low[0] = Convert.ToInt32(txtCounterLow1.Text);
            MainWindow.Setting.SubSystem.ENC.High[0] = Convert.ToInt32(txtCounterHigh1.Text);
            MainWindow.Setting.SubSystem.ENC.Low[1] = Convert.ToInt32(txtCounterLow2.Text);
            MainWindow.Setting.SubSystem.ENC.High[1] = Convert.ToInt32(txtCounterHigh2.Text);
            MainWindow.Setting.SubSystem.ENC.Low[2] = Convert.ToInt32(txtCounterLow3.Text);
            MainWindow.Setting.SubSystem.ENC.High[2] = Convert.ToInt32(txtCounterHigh3.Text);
        }

        public void TrySave()
        {
            if (IsValidate())
            {
                if (CheckSave())
                {
                    Save();
                }
            }
        }
    }
}
