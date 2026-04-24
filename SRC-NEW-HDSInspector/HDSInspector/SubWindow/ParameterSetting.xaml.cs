using Common;
using Common.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HDSInspector.SubWindow
{
    /// <summary>
    /// 파라미터 제한 값 설정
    /// </summary>
    public partial class ParameterSetting : Window
    {
        public ParameterSetting()
        {
            InitializeComponent();
            Initevent();
        }

        private void Initevent()
        {
            txtMaxVent.IsEnabled = false;
            txtMaxVent2.IsEnabled = false;
            txtMinCross.IsEnabled = false;
            txtMinSurface.IsEnabled = false;
            txtMinPSR.IsEnabled = false;
            txtMinLead.IsEnabled = false;
            txtMinSpace.IsEnabled = false;
            LoadIniFile();
        }

        private void LoadIniFile()
        {
            try
            {
                string iniPath = String.Format(@"{0}/Parameters.ini", Settings.GetSettings().GetCommonPath().GetConfigPath());

                if (File.Exists(iniPath))
                {
                    IniFile ini = new IniFile(iniPath);
                    ParametersLocking temp = new ParametersLocking();

                    temp.MinVent = ini.ReadInteger("VentHole", "Min", 0);
                    temp.MaxVent = ini.ReadInteger("VentHole", "Max", 0);
                    temp.MinVent2 = ini.ReadInteger("VentHole2", "Min", 0);
                    temp.MaxVent2 = ini.ReadInteger("VentHole2", "Max", 0);
                    temp.MinCross = ini.ReadInteger("Cross", "Min", 0);
                    temp.MaxCross = ini.ReadInteger("Cross", "Max", 0);
                    temp.MinSurface = ini.ReadInteger("Surface", "Min", 0);
                    temp.MaxSurface = ini.ReadInteger("Surface", "Max", 0);
                    temp.MinPSR = ini.ReadInteger("PSR", "Min", 0);
                    temp.MaxPSR = ini.ReadInteger("PSR", "Max", 0);
                    temp.MinLead = ini.ReadInteger("Lead", "Min", 0);
                    temp.MaxLead = ini.ReadInteger("Lead", "Max", 0);
                    temp.MinSpace = ini.ReadInteger("Space", "Min", 0);
                    temp.MaxSpace = ini.ReadInteger("Space", "Max", 0);

                    txtMinVent.Text = temp.MinVent.ToString();
                    txtMaxVent.Text = temp.MaxVent.ToString();
                    txtMinVent2.Text = temp.MinVent2.ToString();
                    txtMaxVent2.Text = temp.MaxVent2.ToString();
                    txtMinCross.Text = temp.MinCross.ToString();
                    txtMaxCross.Text = temp.MaxCross.ToString();
                    txtMinSurface.Text = temp.MinSurface.ToString();
                    txtMaxSurface.Text = temp.MaxSurface.ToString();
                    txtMinPSR.Text = temp.MinPSR.ToString();
                    txtMaxPSR.Text = temp.MaxPSR.ToString();
                    txtMinLead.Text = temp.MinLead.ToString();
                    txtMaxLead.Text = temp.MaxLead.ToString();
                    txtMinSpace.Text = temp.MinSpace.ToString();
                    txtMaxSpace.Text = temp.MaxSpace.ToString();

                    MainWindow.LockingData = temp;
                }
                else
                {
                    IniFile ini = new IniFile(iniPath);
                    ini.WriteInteger("VentHole", "Min", 200);
                    ini.WriteInteger("VentHole", "Max", 0);
                    ini.WriteInteger("VentHole2", "Min", 40);
                    ini.WriteInteger("VentHole2", "Max", 0);
                    ini.WriteInteger("Cross", "Min", 0);
                    ini.WriteInteger("Cross", "Max", 50);
                    ini.WriteInteger("Surface", "Min", 0);
                    ini.WriteInteger("Surface", "Max", 500);
                    ini.WriteInteger("PSR", "Min", 0);
                    ini.WriteInteger("PSR", "Max", 500);
                    ini.WriteInteger("Lead", "Min", 0);
                    ini.WriteInteger("Lead", "Max", 80);
                    ini.WriteInteger("Space", "Min", 0);
                    ini.WriteInteger("Space", "Max", 80);
                    ParametersLocking temp = new ParametersLocking();

                    temp.MinVent = ini.ReadInteger("VentHole", "Min", 0);
                    temp.MaxVent = ini.ReadInteger("VentHole", "Max", 0);
                    temp.MinCross = ini.ReadInteger("Cross", "Min", 0);
                    temp.MaxCross = ini.ReadInteger("Cross", "Max", 0);
                    temp.MinSurface = ini.ReadInteger("Surface", "Min", 0);
                    temp.MaxSurface = ini.ReadInteger("Surface", "Max", 0);
                    temp.MinPSR = ini.ReadInteger("PSR", "Min", 0);
                    temp.MaxPSR = ini.ReadInteger("PSR", "Max", 0);
                    temp.MinLead = ini.ReadInteger("Lead", "Min", 0);
                    temp.MaxLead = ini.ReadInteger("Lead", "Max", 0);
                    temp.MinSpace = ini.ReadInteger("Space", "Min", 0);
                    temp.MaxSpace = ini.ReadInteger("Space", "Max", 0);

                    txtMinVent.Text = temp.MinVent.ToString();
                    txtMaxVent.Text = temp.MaxVent.ToString();
                    txtMinCross.Text = temp.MinCross.ToString();
                    txtMaxCross.Text = temp.MaxCross.ToString();
                    txtMinSurface.Text = temp.MinSurface.ToString();
                    txtMaxSurface.Text = temp.MaxSurface.ToString();
                    txtMinPSR.Text = temp.MinPSR.ToString();
                    txtMaxPSR.Text = temp.MaxPSR.ToString();
                    txtMinLead.Text = temp.MinLead.ToString();
                    txtMaxLead.Text = temp.MaxLead.ToString();
                    txtMinSpace.Text = temp.MinSpace.ToString();
                    txtMaxSpace.Text = temp.MaxSpace.ToString();

                    MainWindow.LockingData = temp;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Parameter Locking 정보를 불러오지 못했습니다.");
                MainWindow.Log("PCS", SeverityLevel.DEBUG, "Parameter Locking 정보를 불러오지 못했습니다." + ex.Message, false);
            }
        }

        private void SaveIniFile()
        {
            try
            {
                string iniPath = String.Format(@"{0}/Parameters.ini", Settings.GetSettings().GetCommonPath().GetConfigPath());
                IniFile ini = new IniFile(iniPath);

                ParametersLocking temp = new ParametersLocking();

                temp.MinVent = Convert.ToInt32(txtMinVent.Text);
                ini.WriteInteger("VentHole", "Min", temp.MinVent);
                temp.MaxVent = Convert.ToInt32(txtMaxVent.Text);
                ini.WriteInteger("VentHole", "Max", temp.MaxVent);

                temp.MinVent2 = Convert.ToInt32(txtMinVent2.Text);
                ini.WriteInteger("VentHole2", "Min", temp.MinVent2);
                temp.MaxVent2 = Convert.ToInt32(txtMaxVent2.Text);
                ini.WriteInteger("VentHole2", "Max", temp.MaxVent2);

                temp.MinCross = Convert.ToInt32(txtMinCross.Text);
                ini.WriteInteger("Cross", "Min", temp.MinCross);
                temp.MaxCross = Convert.ToInt32(txtMaxCross.Text);
                ini.WriteInteger("Cross", "Max", temp.MaxCross);

                temp.MinSurface = Convert.ToInt32(txtMinSurface.Text);
                ini.WriteInteger("Surface", "Min", temp.MinSurface);
                temp.MaxSurface = Convert.ToInt32(txtMaxSurface.Text);
                ini.WriteInteger("Surface", "Max", temp.MaxSurface);

                temp.MinPSR = Convert.ToInt32(txtMinPSR.Text);
                ini.WriteInteger("PSR", "Min", temp.MinPSR);
                temp.MaxPSR = Convert.ToInt32(txtMaxPSR.Text);
                ini.WriteInteger("PSR", "Max", temp.MaxPSR);

                temp.MinLead = Convert.ToInt32(txtMinLead.Text);
                ini.WriteInteger("Lead", "Min", temp.MinLead);
                temp.MaxLead = Convert.ToInt32(txtMaxLead.Text);
                ini.WriteInteger("Lead", "Max", temp.MaxLead);

                temp.MinSpace = Convert.ToInt32(txtMinSpace.Text);
                ini.WriteInteger("Space", "Min", temp.MinSpace);
                temp.MaxSpace = Convert.ToInt32(txtMaxSpace.Text);
                ini.WriteInteger("Space", "Max", temp.MaxSpace);

                MainWindow.LockingData = temp;
                MainWindow.Log("PCS", SeverityLevel.DEBUG, "파라매터 locking 저장되었습니다.", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Parameter Locking 정보를 저장하지 못했습니다.");
                MainWindow.Log("PCS", SeverityLevel.DEBUG, "Parameter Locking 정보를 저장하지 못했습니다." + ex.Message, false);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveIniFile();
            this.Hide();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
