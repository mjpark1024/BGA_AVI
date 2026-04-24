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
using System.Windows.Shapes;

namespace HDSInspector.SubWindow
{
    /// <summary>
    /// RangeSettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RangeSettingWindow : Window
    {

        private MainWindow m_ptrMainWindow;
        private int MAX_VALUE = 99999;
        string path = Settings.GetSettings().GetCommonPath().GetAppPath() + "\\parameters.ini";

        public void SetWindow()
        {
            m_ptrMainWindow = Application.Current.MainWindow as MainWindow;
        }

        public RangeSettingWindow()
        {
            InitializeComponent();
            initialize();
        }

        public void initialize()
        {
            btnSave.Click += SaveData;
            btnCancel.Click += btnCancel_Click;
            SetWindow();
            ReadData();
        }

        public void ReadData()
        {
            IniFile ini = new IniFile(path);
                        

            m_ptrMainWindow.TeachingDlg.min_vent = ini.ReadInteger("VentHole", "MIN VALUE", 0);
            min_vent.Text = m_ptrMainWindow.TeachingDlg.min_vent.ToString();

            m_ptrMainWindow.TeachingDlg.max_vent = ini.ReadInteger("VentHole", "MAX VALUE", MAX_VALUE);
            max_vent.Text = ini.ReadInteger("VentHole", "MAX VALUE", MAX_VALUE).ToString();

            m_ptrMainWindow.TeachingDlg.min_cross = ini.ReadInteger("Cross", "MIN VALUE", 0);
            min_cross.Text = ini.ReadInteger("Cross", "MIN VALUE", 0).ToString();
            m_ptrMainWindow.TeachingDlg.max_cross = ini.ReadInteger("Cross", "MAX VALUE", MAX_VALUE);
            max_cross.Text = ini.ReadInteger("Cross", "MAX VALUE", MAX_VALUE).ToString();

            m_ptrMainWindow.TeachingDlg.min_surface = ini.ReadInteger("Surface", "MIN VALUE", 0);
            min_surface.Text = ini.ReadInteger("Surface", "MIN VALUE", 0).ToString();
            m_ptrMainWindow.TeachingDlg.max_surface = ini.ReadInteger("Surface", "MAX VALUE", MAX_VALUE);
            max_surface.Text = ini.ReadInteger("Surface", "MAX VALUE", MAX_VALUE).ToString();

            m_ptrMainWindow.TeachingDlg.min_psr = ini.ReadInteger("Psr", "MIN VALUE", 0);
            min_psr.Text = ini.ReadInteger("Psr", "MIN VALUE", 0).ToString();
            m_ptrMainWindow.TeachingDlg.max_psr = ini.ReadInteger("Psr", "MAX VALUE", MAX_VALUE);
            max_psr.Text = ini.ReadInteger("Psr", "MAX VALUE", MAX_VALUE).ToString();

            m_ptrMainWindow.TeachingDlg.min_lead = ini.ReadInteger("Lead", "MIN VALUE", 0);
            min_lead.Text = ini.ReadInteger("Lead", "MIN VALUE", 0).ToString();
            m_ptrMainWindow.TeachingDlg.max_lead = ini.ReadInteger("Lead", "MAX VALUE", MAX_VALUE);
            max_lead.Text = ini.ReadInteger("Lead", "MAX VALUE", MAX_VALUE).ToString();

            m_ptrMainWindow.TeachingDlg.min_space = ini.ReadInteger("Space", "MIN VALUE", 0);
            min_space.Text = ini.ReadInteger("Space", "MIN VALUE", 0).ToString();
            m_ptrMainWindow.TeachingDlg.max_space = ini.ReadInteger("Space", "MAX VALUE", MAX_VALUE);
            max_space.Text = ini.ReadInteger("Space", "MAX VALUE", MAX_VALUE).ToString();

        }

        public void SaveData(object sender, RoutedEventArgs e)
        {
            IniFile ini = new IniFile(path);

            ini.WriteInteger("VentHole", "MIN VALUE", Convert.ToInt32(min_vent.Text));
            ini.WriteInteger("VentHole", "MAX VALUE", Convert.ToInt32(max_vent.Text));

            ini.WriteInteger("Cross", "MIN VALUE", Convert.ToInt32(min_cross.Text));
            ini.WriteInteger("Cross", "MAX VALUE", Convert.ToInt32(max_cross.Text));

            ini.WriteInteger("Surface", "MIN VALUE", Convert.ToInt32(min_surface.Text));
            ini.WriteInteger("Surface", "MAX VALUE", Convert.ToInt32(max_surface.Text));

            ini.WriteInteger("Psr", "MIN VALUE", Convert.ToInt32(min_psr.Text));
            ini.WriteInteger("Psr", "MAX VALUE", Convert.ToInt32(max_psr.Text));

            ini.WriteInteger("Lead", "MIN VALUE", Convert.ToInt32(min_lead.Text));
            ini.WriteInteger("Lead", "MAX VALUE", Convert.ToInt32(max_lead.Text));

            ini.WriteInteger("Space", "MIN VALUE", Convert.ToInt32(min_space.Text));
            ini.WriteInteger("Space", "MAX VALUE", Convert.ToInt32(max_space.Text));

            this.DialogResult = true;
        }
        void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
