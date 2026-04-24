using Common;
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

namespace HDSInspector
{
    /// <summary>
    /// SettingAlgorithm.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingAlgorithm : UserControl
    {
        public SettingAlgorithm()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }

        private void InitializeDialog()
        {
            if (MainWindow.Setting.Job.PSRShiftType == 0) rbPSRShiftType1.IsChecked = true;
            if (MainWindow.Setting.Job.PSRShiftType == 1) rbPSRShiftType2.IsChecked = true;
            if (MainWindow.Setting.Job.PSRShiftType == 2) rbPSRShiftType3.IsChecked = true;
            if (MainWindow.Setting.Job.PSRShiftType == 3) rbPSRShiftType4.IsChecked = true;
        }

        private void InitializeEvent()
        {
            rbPSRShiftType1.Click += btnRadio_Event;
            rbPSRShiftType2.Click += btnRadio_Event;
            rbPSRShiftType3.Click += btnRadio_Event;
            rbPSRShiftType4.Click += btnRadio_Event;
        }

        private void btnRadio_Event(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            if ((bool)rbPSRShiftType1.IsChecked) MainWindow.Setting.Job.PSRShiftType = 0;
            if ((bool)rbPSRShiftType2.IsChecked) MainWindow.Setting.Job.PSRShiftType = 1;
            if ((bool)rbPSRShiftType3.IsChecked) MainWindow.Setting.Job.PSRShiftType = 2;
            if ((bool)rbPSRShiftType4.IsChecked) MainWindow.Setting.Job.PSRShiftType = 3;
        }
    }
}
