using System;
using System.Windows;
using System.Threading;

namespace ResultManagement
{
    public partial class App : Application
    {
        private Mutex m_Mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            string mutexName = "RMSMutex";

            try
            {
                m_Mutex = new Mutex(false, mutexName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }

            if (m_Mutex.WaitOne(0, false))
            {
                base.OnStartup(e);
            }
            else
            {
                MessageBox.Show("프로그램이 이미 실행중입니다.");
                Application.Current.Shutdown();
            }
        }
    }
}
