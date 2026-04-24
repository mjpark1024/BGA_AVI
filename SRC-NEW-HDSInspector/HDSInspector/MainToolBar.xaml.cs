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

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;

namespace HDSInspector
{
    public delegate void MenuChangeEventHandler(MenuType selectedMenu);

    public enum MenuType
    {
        NONE = 0,
        MODELMANAGER = 1,       // 모델관리
        MODELTEACHING = 2,      // 모델티칭
        INSPECTIONRESULT = 3,   // 검사결과
        LOGSETTING = 4,         // 로그세팅
        SETTINGS = 5,           // 환경설정
        LASERMARKING = 6,       // 레이저마킹
        RUNINSPECT = 7,         // 검사시작
        STOPINSPECT = 8,        // 검사정지
        PAUSEINSPECT = 9,       // 검사일시정지
        CHANGEUSER = 10,        // 작업자변경
        SCRAP = 11,             // 폐기 확인
        EXIT = 99               // 종료
    }

    public partial class MainToolBar : UserControl
    {
        private bool m_bLaserEnable = false;
        private bool m_bInspect = false;
        private MainWindow m_ptrMainWindow = null;

        public static event MenuChangeEventHandler MenuChangeEvent;

        /// <summary>   Initializes a new instance of the MainToolBar class. </summary>
        public MainToolBar()
        {
            InitializeComponent();
            InitializeEvent();
        }

        /// <summary>   Sets a main window pointer. </summary>
        public void SetMainWindowPtr(MainWindow mainWindow)
        {
            this.m_ptrMainWindow = mainWindow;
        }

        /// <summary>   Initializes the event. </summary>
        private void InitializeEvent()
        {
            toolButton1.Click += ToolButtonClick;
            toolButton2.Click += ToolButtonClick;
            toolButton3.Click += ToolButtonClick;
            toolButton4.Click += ToolButtonClick;
            toolButton5.Click += ToolButtonClick;
            toolButton6.Click += ToolButtonClick;
            toolButton7.Click += ToolButtonClick;
            toolButton8.Click += ToolButtonClick;
            toolButton9.Click += ToolButtonClick;
            toolButton10.Click += ToolButtonClick;
        }

        /// <summary>   Tool button click. </summary>
        private void ToolButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
            {
                return;
            }

            MenuChangeEventHandler eventRunner = MenuChangeEvent;
            if (eventRunner == null)
            {
                return;
            }

            // switch by tag.
            string strTag = button.Tag.ToString();
            switch (strTag)
            {
                case "ModelManager":
                    eventRunner(MenuType.MODELMANAGER);
                    break;
                case "ModelTeaching":
                    if (MainWindow.CurrentModel == null)
                    {
                        // 모델이 선택되지 않으면 티칭을 진행할 수 없다.
                        MessageBox.Show(ResourceStringHelper.GetInformationMessage("MT005"), "Information");
                        break;
                    }
                    eventRunner(MenuType.MODELTEACHING);
                    break;
                case "InspectionResult":
                    eventRunner(MenuType.INSPECTIONRESULT);
                    break;
                case "SettingLog":
                    eventRunner(MenuType.LOGSETTING);
                    break;
                case "Configure":
                    eventRunner(MenuType.SETTINGS);
                    break;
                case "MarkingLaser":
                    eventRunner(MenuType.LASERMARKING);
                    break;
                case "Inspection":
                    if (MainWindow.CurrentModel == null)
                    {
                        // 모델이 선택되지 않으면 검사를 진행할 수 없다.
                        MessageBox.Show(ResourceStringHelper.GetInformationMessage("IS001"), "Information");
                        break;
                    }

                    if (m_bInspect)
                    {
                        InspectionStop(2);
                        eventRunner(MenuType.STOPINSPECT);
                    }
                    else
                    {                        
                        eventRunner(MenuType.RUNINSPECT);
                    }
                    break;
                case "ChangeUser":
                    eventRunner(MenuType.CHANGEUSER);
                    break;
                case "Close":
                    eventRunner(MenuType.EXIT);
                    break;
                case "Scrap":
                    eventRunner(MenuType.SCRAP);
                    break;
                default:
                    eventRunner(MenuType.NONE);
                    break;
            }
        }

        /// <summary>   Sets a laser enable. </summary>
        /// <param name="value">    true to value. </param>
        public void SetLaserEnable(bool value)
        {
            m_bLaserEnable = value;
            toolButton4.IsEnabled = value;
            if (m_bLaserEnable)
            {
                toolButton4.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        /// <summary>   Sets a label text. </summary>
        /// <param name="anLabelType">    Label type </param>
        public void SetLabelText(string aszStatus)
        {
            lblStatus.Content = aszStatus;            
        }

        /// <summary>   Inspection start. </summary>
        public void InspectionStart()
        {
            m_bInspect = true;

            BitmapImage Img = new BitmapImage();
            Img.BeginInit();
            Img.UriSource = new Uri("/HDSInspector;component/Images/stop2.png", UriKind.Relative);
            Img.EndInit();
            this.ImgInspect.Source = Img;

            toolButton1.IsEnabled = false;  // 모델관리
            toolButton2.IsEnabled = false;  // 모델티칭
            toolButton4.IsEnabled = false;  // 레이저마킹
            toolButton5.IsEnabled = false;  // 로그설정
            toolButton6.IsEnabled = false;  // 환경설정
            toolButton8.IsEnabled = false;  // 작업자변경
            toolButton9.IsEnabled = false;  // 닫기
            toolButton1.Foreground = new SolidColorBrush(Colors.White);
            toolButton2.Foreground = new SolidColorBrush(Colors.White);
            toolButton4.Foreground = new SolidColorBrush(Colors.White);
            toolButton5.Foreground = new SolidColorBrush(Colors.White);
            toolButton6.Foreground = new SolidColorBrush(Colors.White);
            toolButton8.Foreground = new SolidColorBrush(Colors.White);
            toolButton9.Foreground = new SolidColorBrush(Colors.White);
        }

        /// <summary>   Inspection stop. </summary>
        public void InspectionStop(int nStopState)
        {
            m_bInspect = false;

            BitmapImage Img = new BitmapImage();
            Img.BeginInit();
            Img.UriSource = new Uri("/HDSInspector;component/Images/start2.png", UriKind.Relative);
            Img.EndInit();
            this.ImgInspect.Source = Img;

            toolButton1.IsEnabled = true;
            toolButton2.IsEnabled = true;
            toolButton3.IsEnabled = true;
            toolButton4.IsEnabled = true;
            toolButton5.IsEnabled = true;
            toolButton6.IsEnabled = true;
            toolButton8.IsEnabled = true;
            toolButton9.IsEnabled = true;
            toolButton1.Foreground = new SolidColorBrush(Colors.Black);
            toolButton2.Foreground = new SolidColorBrush(Colors.Black);
            toolButton3.Foreground = new SolidColorBrush(Colors.Black);
            toolButton4.Foreground = new SolidColorBrush(Colors.Black); // 레이저 마킹
            toolButton5.Foreground = new SolidColorBrush(Colors.Black);
            toolButton6.Foreground = new SolidColorBrush(Colors.Black);
            toolButton8.Foreground = new SolidColorBrush(Colors.Black);
            toolButton9.Foreground = new SolidColorBrush(Colors.Black);

            m_ptrMainWindow.POPStopReport(nStopState);
        }
    }
}