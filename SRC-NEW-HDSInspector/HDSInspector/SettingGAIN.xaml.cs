using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;
using Common.DataBase;

namespace HDSInspector
{
    /// <summary>
    /// SettingGAIN.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingGAIN : UserControl
    {
        public SettingGAIN()
        {
            InitializeComponent();
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            txtCam1RGain.Text = MainWindow.Setting.SubSystem.IS.R_Gain[1].ToString();
            txtCam1GGain.Text = MainWindow.Setting.SubSystem.IS.G_Gain[1].ToString();
            txtCam1BGain.Text = MainWindow.Setting.SubSystem.IS.B_Gain[1].ToString();

            txtCam2RGain.Text = MainWindow.Setting.SubSystem.IS.R_Gain[2].ToString();
            txtCam2GGain.Text = MainWindow.Setting.SubSystem.IS.G_Gain[2].ToString();
            txtCam2BGain.Text = MainWindow.Setting.SubSystem.IS.B_Gain[2].ToString();

            txtCamSRGain.Text = MainWindow.Setting.SubSystem.IS.Strenth[0].ToString();
        }

        public void Save()
        {
            try
            {
                MainWindow.Setting.SubSystem.IS.R_Gain[1] = (float)Convert.ToDouble(txtCam1RGain.Text);
                MainWindow.Setting.SubSystem.IS.G_Gain[1] = (float)Convert.ToDouble(txtCam1GGain.Text);
                MainWindow.Setting.SubSystem.IS.B_Gain[1] = (float)Convert.ToDouble(txtCam1BGain.Text);
                MainWindow.Setting.SubSystem.IS.R_Gain[2] = (float)Convert.ToDouble(txtCam2RGain.Text);
                MainWindow.Setting.SubSystem.IS.G_Gain[2] = (float)Convert.ToDouble(txtCam2GGain.Text);
                MainWindow.Setting.SubSystem.IS.B_Gain[2] = (float)Convert.ToDouble(txtCam2BGain.Text);
                MainWindow.Setting.SubSystem.IS.Strenth[0] = (float)Convert.ToDouble(txtCamSRGain.Text);

                MainWindow.Setting.SubSystem.Save();
            }
            catch
            {
                MessageBox.Show("Gain 값 입력 오류입니다.");
            }
        }
    }
}
