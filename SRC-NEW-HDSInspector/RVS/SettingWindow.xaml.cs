using System;
using System.Windows;

namespace RVS
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        private MainWindow m_MainWindow;

        public SettingWindow(MainWindow aMainWindow)
        {
            m_MainWindow = aMainWindow;
            InitializeComponent();
            InitializeEvent();

            tabRVS.SetMainWindow(aMainWindow);
        }

        private void InitializeEvent()
        {
            btnCancel.Click += btnCancel_Click;
            btnOK.Click += btnOK_Click;
        }

        void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (m_MainWindow.IsVerifying == true)
            {
                MessageBox.Show("Verify 중에는 설정을 변경할 수 없습니다.");
                return;
            }

            if (!tabRVS.CheckValidate())
            {
                MessageBox.Show("장비이름은 중복될 수 없습니다.");
                return;
            }

            tabRVS.SaveRVS();
          //  m_MainWindow.ResetEquipment(MainWindow.Setting.SubSystem.EquipName.Length);
            this.Close();
        }

        void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
