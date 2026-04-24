using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using Common.DataBase;

namespace RMS
{
    public partial class MainWindow : Window
    {
        public static Logger MainLogger = Logger.GetLogger();

        public static Settings Setting
        {
            get { return Common.Settings.GetSettings(); }
        }

        public MainWindow()
        {
            Setting.Load();
            ConnectDb();
            InitializeComponent();            
            InitializeDialog();
            InitializeEvent();

            //RMSRemotingServer();
            CleanLog();

            Log("RMS", SeverityLevel.DEBUG, "프로그램이 시작되었습니다.");
        }

        private void InitializeDialog()
        {
            btn_MouseLeftButtonDown(this.btnUserManagement, null);
        }

        private void InitializeEvent()
        {
            this.btnUserManagement.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(btn_MouseLeftButtonDown);
            this.btnEquipmentManagement.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(btn_MouseLeftButtonDown);
            this.btnNGInformationManagement.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(btn_MouseLeftButtonDown);
            this.btnInspectTypeManagement.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(btn_MouseLeftButtonDown);
            this.btnResultDataManagement.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(btn_MouseLeftButtonDown);

            this.btnUserManagement.MouseEnter += new MouseEventHandler(btn_MouseEnter);
            this.btnUserManagement.MouseLeave += new MouseEventHandler(btn_MouseLeave);
            this.btnEquipmentManagement.MouseEnter += new MouseEventHandler(btn_MouseEnter);
            this.btnEquipmentManagement.MouseLeave += new MouseEventHandler(btn_MouseLeave);
            this.btnNGInformationManagement.MouseEnter += new MouseEventHandler(btn_MouseEnter);
            this.btnNGInformationManagement.MouseLeave += new MouseEventHandler(btn_MouseLeave);
            this.btnInspectTypeManagement.MouseEnter += new MouseEventHandler(btn_MouseEnter);
            this.btnInspectTypeManagement.MouseLeave += new MouseEventHandler(btn_MouseLeave);
            this.btnResultDataManagement.MouseEnter += new MouseEventHandler(btn_MouseEnter);
            this.btnResultDataManagement.MouseLeave += new MouseEventHandler(btn_MouseLeave);
        }

        public void ConnectDb()
        {
            String strDBIP = Settings.GetSettings().SubSystem.DBIP;
            String strDBPort = Settings.GetSettings().SubSystem.DBPort;
            String strCon = String.Format("server={0};user id={1}; password={2}; database=BGADB; port={3}; pooling=false", strDBIP, "root", "mysql", strDBPort);
            ConnectFactory.CreateConnection(strCon);
        }

        private void btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button menuButton = sender as Button;
            if (menuButton != null)
            {
                string tag = menuButton.Tag.ToString();
                switch (tag)
                {
                    case "U":
                        UserManagement.Visibility = Visibility.Visible;
                        EquipmentManagement.Visibility = Visibility.Collapsed;
                        NGInformationManagement.Visibility = Visibility.Collapsed;
                        InspectTypeManagement.Visibility = Visibility.Collapsed;
                        ResultDataManagement.Visibility = Visibility.Collapsed;

                        this.btnUserManagement.BorderThickness = new Thickness(0, 4, 0, 4);
                        this.btnEquipmentManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnNGInformationManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnInspectTypeManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnResultDataManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        break;
                    case "E":
                        UserManagement.Visibility = Visibility.Collapsed;
                        EquipmentManagement.Visibility = Visibility.Visible;
                        NGInformationManagement.Visibility = Visibility.Collapsed;
                        InspectTypeManagement.Visibility = Visibility.Collapsed;
                        ResultDataManagement.Visibility = Visibility.Collapsed;

                        this.btnUserManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnEquipmentManagement.BorderThickness = new Thickness(0, 4, 0, 4);
                        this.btnNGInformationManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnInspectTypeManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnResultDataManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        break;
                    case "N":
                        UserManagement.Visibility = Visibility.Collapsed;
                        EquipmentManagement.Visibility = Visibility.Collapsed;
                        NGInformationManagement.Visibility = Visibility.Visible;
                        InspectTypeManagement.Visibility = Visibility.Collapsed;
                        ResultDataManagement.Visibility = Visibility.Collapsed;

                        this.btnUserManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnEquipmentManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnNGInformationManagement.BorderThickness = new Thickness(0, 4, 0, 4);
                        this.btnInspectTypeManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnResultDataManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        break;
                    case "I":
                        UserManagement.Visibility = Visibility.Collapsed;
                        EquipmentManagement.Visibility = Visibility.Collapsed;
                        NGInformationManagement.Visibility = Visibility.Collapsed;
                        InspectTypeManagement.Visibility = Visibility.Visible;
                        ResultDataManagement.Visibility = Visibility.Collapsed;

                        this.btnUserManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnEquipmentManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnNGInformationManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnInspectTypeManagement.BorderThickness = new Thickness(0, 4, 0, 4);
                        this.btnResultDataManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        break;
                    case "D":
                        UserManagement.Visibility = Visibility.Collapsed;
                        EquipmentManagement.Visibility = Visibility.Collapsed;
                        NGInformationManagement.Visibility = Visibility.Collapsed;
                        InspectTypeManagement.Visibility = Visibility.Collapsed;
                        ResultDataManagement.Visibility = Visibility.Collapsed;

                        this.btnUserManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnEquipmentManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnNGInformationManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnInspectTypeManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnResultDataManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        break;
                    case "R":
                        UserManagement.Visibility = Visibility.Collapsed;
                        EquipmentManagement.Visibility = Visibility.Collapsed;
                        NGInformationManagement.Visibility = Visibility.Collapsed;
                        InspectTypeManagement.Visibility = Visibility.Collapsed;
                        ResultDataManagement.Visibility = Visibility.Visible;

                        this.btnUserManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnEquipmentManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnNGInformationManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnInspectTypeManagement.BorderThickness = new Thickness(0, 0, 0, 0);
                        this.btnResultDataManagement.BorderThickness = new Thickness(0, 4, 0, 4);
                        break;

                }
            }
        }

        private void btn_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void btn_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        #region .Net remoting
        [STAThread]
        private void RMSRemotingServer()
        {
            //Read configuration file, and apply the program
            //XML Parsing :get Port
            //ChannelServices.RegisterChannel(new TcpChannel(50002), false);
            //RemotingConfiguration.RegisterWellKnownServiceType(typeof(ResultManagement.CRMSAutoInspInterface), "RMSAutoInspUri", WellKnownObjectMode.Singleton);
        }

        private void RMSRemotingClient()
        {
            //  m_PCSInterface = (CPCSInterface)Activator.GetObject(typeof(ProcessControl.CPCSInterface), "tcp://localhost:50001/PCSUri");
        }
        #endregion

        #region Handle log.
        private static void Log(string astrSubSystem, SeverityLevel aSeverityLevel, string astrLogMessage)
        {
            if (Convert.ToInt32(aSeverityLevel) >= Convert.ToInt32(Setting.Log.UIDisplayLevel))
            {
                //txtStatusBox.Text += astrSubSystem + ":" + astrLogMessage + "\r\n";
            }

            if (Setting.Log.LocalSave.Equals(1) && Convert.ToInt32(aSeverityLevel) >= Convert.ToInt32(Setting.Log.LocalSaveLevel))
            {
                MainLogger.Log(astrSubSystem, aSeverityLevel, astrLogMessage);
            }

            if (Setting.Log.RemoteSave.Equals(1) && Convert.ToInt32(aSeverityLevel) >= Convert.ToInt32(Setting.Log.LocalSaveLevel))
            {
                try
                {
                    //
                }
                catch
                {
                    MessageBox.Show(ResourceStringHelper.GetErrorMessage("C002"), "Error");
                }
            }
        }

        private void CleanLog()
        {
            int nDeleteLogCount = Setting.Log.CleanLog();
            if (nDeleteLogCount > 0)
            {
                MessageBox.Show("최근에 기록된 " + nDeleteLogCount + "개의 로그 파일을 정리하였습니다.", "Information");
            }
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Log("RMS", SeverityLevel.DEBUG, "프로그램을 종료합니다.");
            MainLogger.Close();
            MainLogger = null;
        }
    }
}
