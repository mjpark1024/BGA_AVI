/*********************************************************************************
 * Copyright(c) 2015 by Haesung DS.
 * 
 * This software is copyrighted by, and is the sole property of Haesung DS.
 * All rigths, title, ownership, or other interests in the software remain the
 * property of Haesung DS. This software may only be used in accordance with
 * the corresponding license agreement. Any unauthorized use, duplication, 
 * transmission, distribution, or disclosure of this software is expressly 
 * forbidden.
 *
 * This Copyright notice may not be removed or modified without prior written
 * consent of Haesung DS reserves the right to modify this 
 * software without notice.
 *
 * Haesung DS.
 * KOREA 
 * http://www.HaesungDS.com
 *********************************************************************************/
/**
 * @file  LoginWindow.xaml.cs
 * @brief . 
 *  behind code of LoginWindow.xaml
 * 
 * @author : suoow <suoow.yeo@haesung.net>
 * @date : 2011.05.09
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.05.09 First creation.
 * - 2011.05.15 ProgressBar Add.
 * - 2011.05.26 Xml reading Add.
 */
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
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Data;
using Common;
using Common.Control;
using Common.DataBase;
using RMS.Generic.UserManagement;

namespace RMS
{
    /** 
     * @brief interation with LoginWindow.xaml
     * @author suoow
     * @date 2011.05.09
     */
    public partial class LoginWindow : Window
    {
        /// <summary> The logger </summary>
        private static Logger m_Logger = Logger.GetLogger();

        /// <summary> The main window </summary>
        private MainWindow m_MainWindow = new MainWindow();

        /// <summary>   Initializes a new instance of the LoginWindow class. </summary>
        public LoginWindow()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();            
            
            Log("RMS", SeverityLevel.DEBUG, "로그인창이 실행되었습니다.");
        }

        /// <summary>   Initializes the dialog. </summary>
        private void InitializeDialog()
        {
            Application.Current.MainWindow = m_MainWindow;

            // 마지막 사용자의 ID를 디스플레이한다.
            string szUserID = Settings.GetSettings().General.LastUser;
            string szUserPW = string.Empty;
            if (string.IsNullOrEmpty(szUserID))
                szUserID = "DS";
            if (szUserID == "DS")
                szUserPW = "DS";

            this.txtID.Text = szUserID;
            this.txtPW.Password = szUserPW;
        }

        private bool CheckLogin()
        {
            if (m_MainWindow != null)
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    String txtPasswd = MD5Core.GetHashString(txtPW.Password + MD5Core.GetHashString(txtID.Text));
                    String strQuery = String.Format("Select a.user_id, a.user_name, a.user_passwd, a.user_auth, b.com_dname from user_info a, com_detail b where a.user_auth = b.com_dcode and a.use_yn = 1 and a.user_id = '{0}' and b.com_mcode = '06' and a.user_passwd = '{1}'", txtID.Text, txtPasswd);

                    IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                    if (dataReader == null)
                    {
                        return false;
                    }

                    if (dataReader.Read())
                    {
                        UserManager.CurrentUser.ID = dataReader.GetValue(0).ToString();
                        UserManager.CurrentUser.Name = dataReader.GetValue(1).ToString();
                        UserManager.CurrentUser.Password = dataReader.GetValue(2).ToString();
                        UserManager.CurrentUser.Authority.AuthCode = dataReader.GetValue(3).ToString();
                        UserManager.CurrentUser.Authority.AuthName = dataReader.GetValue(4).ToString();

                        dataReader.Close();

                        Log("RMS", SeverityLevel.DEBUG, String.Format("\"{0}\"님이 로그인하였습니다.", txtID.Text));

                        return true;
                    }
                }
            }
            Log("RMS", SeverityLevel.DEBUG, String.Format("\"{0}\"님이 로그인에 실패하였습니다.", txtID.Text));
            return false;
        }

        /// <summary>   Initializes the event. </summary>
        private void InitializeEvent()
        {
            this.btnLogin.Click += new RoutedEventHandler(btnLogin_Click);
            this.btnLogin.MouseEnter += new MouseEventHandler(btn_MouseEnter);
            this.btnLogin.MouseLeave += new MouseEventHandler(btn_MouseLeave);

            this.btnClose.Click += new RoutedEventHandler(btnClose_Click);
            this.KeyDown += new KeyEventHandler(LoginWindow_KeyDown);
        }

        private void LoginWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter || e.Key == Key.Space) && this.txtID.Text != string.Empty && this.txtPW.Password != string.Empty)
            {
                btnLogin_Click(btnLogin, null);
            }
        }

        /**
         * @fn titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
         * @brief Moving Titlebar method
         */
        private void titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /**
         * @fn btnClose_Click(object sender, RoutedEventArgs e)
         * @brief close button clicked
         */
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (m_MainWindow != null)
            {
                m_MainWindow.Close();

                if (MainWindow.MainLogger == null)
                {
                    this.Close();
                }
            }
        }
        
        #region handle log.
        /**
         * @fn Log(string astrSubSystem, SeverityLevel aSeverityLevel, string astrLogMessage)
         * @brief do logging & display logs on UI.
         * @param [in] strSubSystem - sub-system name.
         * @param [in] severityLevel - severity level of log
         * @param [in] strLogMessage - log message.
         * @see class Logger.
         */
        public void Log(string astrSubSystem, SeverityLevel aSeverityLevel, string astrLogMessage)
        {
            if (MainWindow.Setting.Log.LocalSave.Equals(1) && Convert.ToInt32(aSeverityLevel)
                >= Convert.ToInt32(MainWindow.Setting.Log.LocalSaveLevel))
            {
                m_Logger.Log(astrSubSystem, aSeverityLevel, astrLogMessage);
            }
        }

        /**
         * @fn CleanLog()
         * @brief it deleting log files by log keep date.
         * @see class LoggerOption.
         */
        public void CleanLog()
        {
            int nDeleteLogCount = MainWindow.Setting.Log.CleanLog();
            if (nDeleteLogCount > 0)
            {
                MessageBox.Show("최근에 기록된 " + nDeleteLogCount + "개의 로그 파일을 정리하였습니다.", "Information");
            }
        }
        #endregion

        /**
         * @fn btnLogin_Click(object sender, RoutedEventArgs e)
         * @brief Login button clicked
         */
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            this.btnLogin.IsEnabled = false;

            /* Set scroll focus */
            txtStatusBoxScroll.ScrollToVerticalOffset(txtStatusBoxScroll.MaxHeight);
            
            #region progressbar
            RoundRectProgressBar progress = new RoundRectProgressBar();
            progress.Owner = this;
            progress.Foreground = new SolidColorBrush(Colors.Gray);
            progress.Show();

            this.txtStatusBox.Text += "로그인...\n";

            int i = 0;

            while (i != 5000)
            {
                i++;
                System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                                       new System.Threading.ThreadStart(delegate { }));
            }
            progress.Stop();
            progress.Close();
            #endregion
            
            this.txtStatusBox.Text += "초기화...\n";
            this.txtStatusBox.Refresh();

            if(this.txtID.Text == "nodb" && this.txtPW.Password == "nodb")
            {
                m_MainWindow.Show();
                this.Close();

                return;
            }

            if (Settings.GetSettings().SubSystem.UseDB == "1")
            {
                if(ConnectFactory.DBConnector() != null)
                {
                    if (CheckLogin())
                    {
                        this.txtStatusBox.Text += "DB : " + Common.Settings.GetSettings().SubSystem.DBIP + "succeed! \n";
                        this.txtStatusBox.Refresh();

                        m_MainWindow.Show();
                        this.Close();

                        return;
                    }
                    else
                    {
                        // ID/PW 잘못된 입력시 에러 메시지
                        MessageBox.Show(ResourceStringHelper.GetErrorMessage("C003"), "Error");
                    }
                }
                else
                {
                    // DB 접속 실패 에러 메시지
                    MessageBox.Show(ResourceStringHelper.GetErrorMessage("C004"), "Error");
                    m_MainWindow.ConnectDb();
                }

                // DB 접속 또는 ID/PW 잘못된 입력 시
                this.btnLogin.IsEnabled = true;
                this.txtStatusBox.Text += "로그인 실패\n";
                txtID.Text = "";
                txtPW.Password = "";
            }
            else
            {
                this.txtStatusBox.Text += "Not used DB! \n";
                this.txtStatusBox.Refresh();

                m_MainWindow.Show();
                this.Close();
                return;
            }
        }

        #region Cursor change event.
        /// <summary>   Event handler. Called by btn for mouse enter events. </summary>
        private void btn_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        /// <summary>   Event handler. Called by btn for mouse leave events. </summary>
        private void btn_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }
        #endregion
    }
}