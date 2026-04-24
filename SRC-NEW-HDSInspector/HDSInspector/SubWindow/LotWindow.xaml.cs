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

using Common;
using Common.Control;
using IGS;
using IGS.Classes;
using MySqlX.XDevAPI.Relational;
using OpenCvSharp.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Action = System.Action;

namespace HDSInspector
{
    public partial class LotWindow : Window
    {
        private MainWindow m_ptrMainWindow;
        private SimpleGageBar m_Gage; // for display disk space.

        private bool m_LaserEnable;
        private string m_strOperatorName;
        private int m_Customer;

        //private CheckBox[] cbAutoNG = new CheckBox[48];
        //private int nBad_Types;
        public ObservableCollection<AutoNG_CheckBox_Item> cb_autoNG_Items { get; set; }

       
        #region Automation Variables.
        private POP_WARNING_INFO pWarningInfo;
        private POP_LOT_DATA curLot;            //MES Lot 정보
        private string curOpcode = "";          //MES상 현재 공정코드
        private string errMsg = "";             //MES 조회 실패 메시지
        
        public string stdModel = "";            //표준모델명 - 기종교체 확인용 
        public bool bNormalLot = false;         //MES상 존재하는 Lot 번호인가
        public bool bRestart = false;           //재검 여부
        #endregion

        public LotWindow(bool LaserEnable, string strOperatorName, NGInformationHelper ng_info)
        {
            InitializeComponent();
            InitializeEvent();
            InitializeDialog(LaserEnable, strOperatorName, ng_info);
            CreateInspCheckBoxes();
        }

        /// <summary>   Initializes the dialog. </summary>
        private void InitializeDialog(bool LaserEnable, string strOperatorName, NGInformationHelper ng_info)
        {
            this.m_strOperatorName = strOperatorName;
            this.m_LaserEnable = LaserEnable;

            #region Auto NG CheckBox
            cb_autoNG_Items = new ObservableCollection<AutoNG_CheckBox_Item>();
            AutoNG_List.DataContext = cb_autoNG_Items;

            //cbAutoNG[0] = cbAutoNG00; cbAutoNG[10] = cbAutoNG10; cbAutoNG[20] = cbAutoNG20; cbAutoNG[30] = cbAutoNG30; cbAutoNG[40] = cbAutoNG40;
            //cbAutoNG[1] = cbAutoNG01; cbAutoNG[11] = cbAutoNG11; cbAutoNG[21] = cbAutoNG21; cbAutoNG[31] = cbAutoNG31; cbAutoNG[41] = cbAutoNG41;
            //cbAutoNG[2] = cbAutoNG02; cbAutoNG[12] = cbAutoNG12; cbAutoNG[22] = cbAutoNG22; cbAutoNG[32] = cbAutoNG32; cbAutoNG[42] = cbAutoNG42;
            //cbAutoNG[3] = cbAutoNG03; cbAutoNG[13] = cbAutoNG13; cbAutoNG[23] = cbAutoNG23; cbAutoNG[33] = cbAutoNG33; cbAutoNG[43] = cbAutoNG43;
            //cbAutoNG[4] = cbAutoNG04; cbAutoNG[14] = cbAutoNG14; cbAutoNG[24] = cbAutoNG24; cbAutoNG[34] = cbAutoNG34; cbAutoNG[44] = cbAutoNG44;
            //cbAutoNG[5] = cbAutoNG05; cbAutoNG[15] = cbAutoNG15; cbAutoNG[25] = cbAutoNG25; cbAutoNG[35] = cbAutoNG35; cbAutoNG[45] = cbAutoNG45;
            //cbAutoNG[6] = cbAutoNG06; cbAutoNG[16] = cbAutoNG16; cbAutoNG[26] = cbAutoNG26; cbAutoNG[36] = cbAutoNG36; cbAutoNG[46] = cbAutoNG46;
            //cbAutoNG[7] = cbAutoNG07; cbAutoNG[17] = cbAutoNG17; cbAutoNG[27] = cbAutoNG27; cbAutoNG[37] = cbAutoNG37; cbAutoNG[47] = cbAutoNG47;
            //cbAutoNG[8] = cbAutoNG08; cbAutoNG[18] = cbAutoNG18; cbAutoNG[28] = cbAutoNG28; cbAutoNG[38] = cbAutoNG38; 
            //cbAutoNG[9] = cbAutoNG09; cbAutoNG[19] = cbAutoNG19; cbAutoNG[29] = cbAutoNG29; cbAutoNG[39] = cbAutoNG39;

            //nBad_Types = ng_info.Size -1;
            //for (int i = 0; i< 48; i++)
            //{
            //    if (i < nBad_Types)
            //    {
            //        string name = ng_info.GetBadName(i+1);
            //        cbAutoNG[i].Content = name;
            //        if (name == "고정불량" || name == "연속불량")
            //        {
            //            cbAutoNG[i].IsEnabled = false;
            //            cbAutoNG[i].IsChecked = false;
            //        }
            //    }
            //    else
            //    {
            //        cbAutoNG[i].Visibility = Visibility.Hidden;
            //        cbAutoNG[i].IsChecked = false;
            //    }
            //}

            for (int i = 1; i < MainWindow.NG_Info.Size; i++)
            {
                string name = MainWindow.NG_Info.GetBadName(i);
                bool isChecked = true ;
                bool isEnabled = true;

                if (name == "고정불량" || name == "연속불량")
                {
                    isChecked = false;
                    isEnabled = false;
                }

                cb_autoNG_Items.Add(new AutoNG_CheckBox_Item
                {
                    Name = name,
                    IsChecked = isChecked,
                    IsEnabled = isEnabled
                });
            }
            #endregion

            #region Drive Infomation
            string szDrive = MainWindow.Setting.General.ResultPath;
            szDrive = szDrive.Substring(0, 1);
            DriveInfo drv = new DriveInfo(szDrive);

            m_Gage = new SimpleGageBar(drv.TotalSize, drv.TotalSize - drv.TotalFreeSpace);
            m_Gage.Margin = new Thickness(110, 0, 10, 0);
            m_Gage.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetColumn(m_Gage, 2);
            Grid.SetColumnSpan(m_Gage, 2);
            Grid.SetRow(m_Gage, 5);
            UpperGrid.Children.Add(m_Gage);

            txtGageLabel.Text = String.Format("디스크 사용량 ({0}:)", szDrive);
            #endregion

            #region Lot Type 정의
            cbLotType.Items.Clear();
            cbLotType.Items.Add("");
            cbLotType.Items.Add("1회차");
            cbLotType.Items.Add("2회차");
            cbLotType.Items.Add("3회차");
            cbLotType.SelectedIndex = 0;
            #endregion

            if (LaserEnable)
            {
                cbMarkEnable.IsEnabled = true;
            }
            else
            {
                cbMarkEnable.IsEnabled = false;
            }

            //this.SizeToContent = SizeToContent.Height;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        public void SetMainWindowPtr(MainWindow mainWindow)
        {
            this.m_ptrMainWindow = mainWindow;
        }

        private void InitializeEvent()
        {
            this.Loaded += LotWindow_Loaded;
            this.KeyDown += LotWindow_KeyDown;
            this.btnOK.Click += btnOK_Click;
            this.btnCancel.Click += btnCancel_Click;
            this.txtLot.KeyUp += new KeyEventHandler(txtLot_KeyDown);
            this.txtCode.KeyDown += new KeyEventHandler(txtCode_KeyDown);
            this.chkStaticMark.Click += chkLiveImage_Click;
            this.txtOperator.KeyUp += TxtOperator_KeyUp;
            //this.txtLot.TextChanged += textBox_TextChaned;
            //this.txtDate.TextChanged += txtDate_TextChanged;
        }


        private void TxtOperator_KeyUp(object sender, KeyEventArgs e)
        {
            if (e == null)
                return;

            if (txtOperator.Text.Length == 6 && e.Key != Key.Enter)
            {
                string strID = txtOperator.Text;
                Regex regex = new Regex(@"[0-9]");
                if (!regex.IsMatch(strID))
                {
                    MessageBox.Show("Operator 정보는 사번만 입력 가능합니다.");
                    return;
                }

                if (m_ptrMainWindow.popManager.GetUserInfo(strID, out POP_USER_DATA user, out errMsg))
                    tbOperName.Text = user.USER_NM;
                else
                    tbOperName.Text = "";
            }
            else
                tbOperName.Text = "";
        }

        void chkLiveImage_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)chkStaticMark.IsChecked)
                this.map.IsEnabled = true;
            else
            {
                this.map.Init();
                this.map.IsEnabled = false;
            }
        }

        void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.B || e.Key == Key.Return)
                e.Handled = true;
        }

        void txtLot_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A || e.Key == Key.Return)
                e.Handled = true;
            else
                SearchMESInfo(txtLot.Text.Trim());
        }

        private void SearchMESInfo(string strLot)
        {
            if (strLot.Length == 9 && MainWindow.Setting.General.UsePOP)
            {
                if (!m_ptrMainWindow.popManager.GetMESLotInfo(strLot, out curLot) || curLot == null)
                {
                    //비정상 로트번호
                    ClientManager.Log(string.Format("Lot 조회 실패, Lot: {0}", txtLot.Text));
                    curOpcode = "";
                    txtITSLot.Text = "";
                    tbPreCnt.Text = "";
                    tbOpState.Text = "MES 조회 불가";
                    bNormalLot = false;
                }
                else
                {
                    curOpcode = curLot.OP_CD;
                    stdModel = curLot.ITEM_CD;

                    txtITSLot.Text = curLot.ITS_ORDER;
                    tbOpState.Text = string.Format("공정 - {0}, 상태 - {1}", curLot.OP_CD, curLot.OP_STAT);

                    bNormalLot = true;
                    if (curLot.PRE_OUT_CNT == "")
                        curLot.PRE_OUT_CNT = "0";
                    tbPreCnt.Text = string.Format("{0:#,##0}", Convert.ToInt32(curLot.PRE_OUT_CNT));

                    //공정 코드 확인
                    if (m_ptrMainWindow.popManager.CheckInspectCode(curLot.OP_CD))
                        txtCode.Text = curLot.OP_CD;
                    else
                        txtCode.Text = "";

                    //Warning & Holding 정보 조회
                    if (m_ptrMainWindow.popManager.GetWarningInfo(strLot, "VI91", out pWarningInfo, out errMsg))
                    {
                        igsWarningInfo.InitializeDialog(pWarningInfo);
                        m_ptrMainWindow.InspectionMonitoringCtrl.igsWarning.InitializeDialog(pWarningInfo);
                    }
                    else
                    {
                        ClientManager.Log(string.Format("Warning 정보 조회 실패, Error: {0}", errMsg));
                        pWarningInfo = null;
                        igsWarningInfo.InitializeFail();
                        m_ptrMainWindow.InspectionMonitoringCtrl.igsWarning.InitializeFail();
                    }

                    //변경점 조회
                    if (m_ptrMainWindow.popManager.GetChangePointInfo(strLot, out List<POP_CHANGE_POINT_OUTPUT_DATA> points, out errMsg))
                    {
                        igsChangeInfo.InitializeDialog(points);
                    }
                    else
                    {
                        ClientManager.Log(string.Format("변경점 정보 조회 실패, Error: {0}", errMsg));
                        igsChangeInfo.InitializeFail();
                    }

                    if ((pWarningInfo != null && pWarningInfo.WARNING_INFO.Count > 0)|| points.Count > 0)
                    {
                        IGS.SubWindow.WARNING_POPUP_Window pDlg = new IGS.SubWindow.WARNING_POPUP_Window();
                        pDlg.InitializeDialog(pWarningInfo, points);
                        pDlg.ShowDialog();
                    }
                }

                textBox_TextChaned(null, null);
            }
            else
            {
                curLot = null;
                pWarningInfo = null;
                bNormalLot = false;
                curOpcode = "";
                txtITSLot.Text = "";
                tbPreCnt.Text = "";
                tbOpState.Text = "";
            }
        }

        private string MonthTo()
        {
            string str = "1";
            int m = DateTime.Now.Month;
            if (m < 10)
                str = m.ToString();
            else
            {
                if (m == 10) str = "A";
                if (m == 11) str = "B";
                if (m == 12) str = "C";
            }
            return str;
        }

        public List<MainWindow.CheckBoxData> checkBoxData = new List<MainWindow.CheckBoxData>();
        private void LotWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_ptrMainWindow.StaticMark)
            {
                this.chkStaticMark.IsChecked = true;
                this.map.IsEnabled = true;
            }
            else
            {
                this.chkStaticMark.IsChecked = false;
                this.map.IsEnabled = false;
            }

            string[] s = MainWindow.Setting.Job.AutoNG.Split(',');
            for (int i = 0; i < MainWindow.AutoNG_Check.Length; i++)
            {
                MainWindow.AutoNG_Check[i] = (s[i] == "1") ? true : false;
                if (cb_autoNG_Items.Count > i)
                    cb_autoNG_Items[i].IsChecked = (MainWindow.AutoNG_Check[i]) ? true : false;
            }

            map.SetUnitNum(MainWindow.CurrentModel.Strip.UnitColumn, MainWindow.CurrentModel.Strip.UnitRow);
            if (m_ptrMainWindow.InspectionMonitoringCtrl.StaticMarkResult.SizeX == MainWindow.CurrentModel.Strip.UnitColumn &&
                m_ptrMainWindow.InspectionMonitoringCtrl.StaticMarkResult.SizeY == MainWindow.CurrentModel.Strip.UnitRow)
            {
                for (int i = 0; i < MainWindow.CurrentModel.Strip.UnitColumn; i++)
                {
                    for (int j = 0; j < MainWindow.CurrentModel.Strip.UnitRow; j++)
                    {
                        if (m_ptrMainWindow.InspectionMonitoringCtrl.StaticMarkResult.Get(i, j) != 0)
                        {
                            map.SetValue(i, j);
                        }
                    }
                }
            }

            txtLot.Focus();
            cbConBad.IsChecked = MainWindow.CurrentModel.UseConBad;
            cbConBad.Content = string.Format("누적 Unit 불량 {0}Strip 시 정지 사용", MainWindow.CurrentModel.ConBad);
            chkVerify.IsChecked = MainWindow.CurrentModel.UseVerify;
            chkUseAI.IsChecked = MainWindow.CurrentModel.UseAI;
            //cbIDMark.IsChecked = MainWindow.CurrentModel.UseIDMark;

            //customer
            m_Customer = MainWindow.Setting.Job.Customer;
            //Customer_Combo.SelectedIndex = m_Customer;
            m_ptrMainWindow.m_Customer = m_Customer;
            //customer 
            PCS.ELF.AVI.ModelInformation model = MainWindow.CurrentModel;
            if (m_LaserEnable)
            {
                // Sets Checkboxes.
                cbMarkEnable.IsChecked = model.UseMarking;
                cbLotType.SelectedIndex = model.InspectMode;
            }
            if (checkBoxData != null)
            {
                for (int i = 0; i < CheckBoxInspContainer.Children.Count; i++)
                {
                    MainWindow.IndexInfo IndexInfo = MainWindow.Convert_CheckVisionSlave(i);
                    //checkBoxData[i].IsChecked = model.ListALLUseSur[i];
                    if (CheckBoxInspContainer.Children[i] is CheckBox checkBox)
                    {
                        if (IndexInfo.CategorySurface == CategorySurface.CA && IndexInfo.Slave)
                        {
                            //checkBox.IsEnabled = MainWindow.Setting.SubSystem.IS.UseCASlave;
                            if (!MainWindow.Setting.SubSystem.IS.UseCASlave)
                            {
                                checkBox.IsChecked = false;
                                model.ListALLUseSur[i] = false;
                            }
                        }
                        else if (IndexInfo.CategorySurface == CategorySurface.BA && IndexInfo.Slave)
                        {
                            //checkBox.IsEnabled = MainWindow.Setting.SubSystem.IS.UseBASlave;
                            if (!MainWindow.Setting.SubSystem.IS.UseBASlave)
                            {
                                checkBox.IsChecked = false;
                                model.ListALLUseSur[i] = false;
                            }
                        }
                        checkBox.IsChecked = model.ListALLUseSur[i];
                    }
                }
            }

            // Lot, Operator Display.
            #region Last Lot Name 체크
            String szLotName = MainWindow.Setting.Job.LastLot;

            //string sLot = "0000";
            //if (m_Customer == 0)
            //{
            //    if (txtLot.Text.Length > 5)
            //        sLot = txtLot.Text.Substring(txtLot.Text.Length - 5, 5);
            //    else
            //        sLot = txtLot.Text;

            //    txtDate.Text = "H" + (DateTime.Now.Year % 100).ToString("00") + MonthTo();

            //    txtIDMark.Text = /*"H" + (DateTime.Now.Year % 100).ToString("00") + MonthTo() +*/ sLot;
            //    txtFirstMark.Text = txtDate.Text + txtIDMark.Text + "00000";
            //}
            //else
            //{
            //    if (txtLot.Text.Length > 9)
            //        sLot = txtLot.Text.Substring(txtLot.Text.Length - 9, 9);
            //    else
            //        sLot = txtLot.Text;


            //    txtDate.Text = "0";

            //    txtIDMark.Text = /*"0" +*/ sLot + "W" + "000";
            //    txtFirstMark.Text = txtDate.Text + txtIDMark.Text + "000";
            //}
            #endregion

            //설비 상태 정보 조회
            if (MainWindow.Setting.General.UsePOP && m_ptrMainWindow.popManager.GetCurStateInfo(out STATE_TABLE_DATA curState) && curState != null)
            {
                if (UserCtrl.curUser != null)
                {
                    txtOperator.Text = UserCtrl.curUser.USERID;
                    tbOperName.Text = UserCtrl.curUser.USER_NM;
                }
                else
                {
                    txtOperator.Text = curState.USER_ID;
                    tbOperName.Text = curState.USER_NAME;
                }

                if (curState.ORDER != "" && curState.ORDER != szLotName)
                    szLotName = curState.ORDER;
            }
            else
            {
                txtOperator.Text = m_strOperatorName;
                tbOperName.Text = "";
            }

            //오더 정보 조회
            if (MainWindow.Setting.General.UsePOP)
                SearchMESInfo(szLotName);

            txtLot.Text = szLotName;
            if (!bNormalLot)
            {
                txtITSLot.Text = MainWindow.Setting.Job.LastItsLot;
                txtCode.Text = MainWindow.Setting.Job.ProcessCode;
            }

            //제조주차
            if (MainWindow.CurrentModel.Marker.WeekMark > 0)
            {
                string week = "";
                week = (MainWindow.CurrentModel.Marker.WeekMarkType == 0) ?
                    WeekOfDay(DateTime.Now).ToString("00") + (DateTime.Now.Year % 100).ToString("00") :
                    (DateTime.Now.Year % 100).ToString("00") + WeekOfDay(DateTime.Now).ToString("00");                
             
                txtWeek.Text = week;
                if (week == "")
                {
                    txtWeek.Text = "1316";
                }
            }
            else
            {
                txtWeek.Text = "";
            }
        }
        private void CreateInspCheckBoxes()
        {
            // 열 정의 (XAML의 Grid.Column에 해당)
            int Column = MainWindow.Setting.Job.CACount;
            if (MainWindow.Setting.Job.CACount < MainWindow.Setting.Job.BACount) Column = MainWindow.Setting.Job.BACount;
            for (int i = 0; i < Column + MainWindow.Setting.Job.BPCount; i++)
            {
                CheckBoxInspContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }
            for (int i = 0; i < 2; i++)
            {
                CheckBoxInspContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            
            List<MainWindow.CheckBoxData> dataSource = new List<MainWindow.CheckBoxData>();
            for (int i = 1; i < MainWindow.Setting.Job.BPCount + 1; i++)
            {
                string str =  "UseBondPad" + i;
                MainWindow.CheckBoxData temp = new MainWindow.CheckBoxData { Name = CategorySurface.BP.ToString() + i, Content = "본드 패드" + i, BindingPath = str, IsChecked = true, IsEnable = false };
                dataSource.Add(temp);
            }
            for (int i = 1; i < MainWindow.Setting.Job.CACount + 1; i++)
            {
                string str = "UseTopSur" + i;
                MainWindow.CheckBoxData temp = new MainWindow.CheckBoxData { Name = CategorySurface.CA.ToString() + i, Content = "CA 표면" + i, BindingPath = str, IsChecked = true, IsEnable = false };
                dataSource.Add(temp);
            }
            for (int i = 1; i < MainWindow.Setting.Job.BACount + 1; i++)
            {
                string str = "UseBotSur" + i;
                MainWindow.CheckBoxData temp = new MainWindow.CheckBoxData { Name = CategorySurface.BA.ToString() + i, Content = "BA 표면" + i, BindingPath = str, IsChecked = true, IsEnable = false };
                dataSource.Add(temp);
            }

            foreach (var data in dataSource)
            {
                checkBoxData.Add(new MainWindow.CheckBoxData
                {
                    Name = data.Name,
                    Content = data.Content,
                    BindingPath = data.BindingPath,
                    IsChecked = data.IsChecked,
                    IsEnable = data.IsEnable
                });
            }

            int column = 0;
            int row = 0;
            // 동적 CheckBox 추가
            for (int i = 0; i < checkBoxData.Count; i++)
            {
                var data = checkBoxData[i];
                CheckBox checkBox = new CheckBox
                {
                    Name = data.Name,
                    Content = data.Content,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    IsChecked = data.IsChecked,
                    IsEnabled = data.IsEnable,
                    Margin = new Thickness(0, 13, 10, 0),
                };

                // 바인딩 설정
                //Binding binding = new Binding(data.BindingPath)
                //{
                //    Mode = BindingMode.TwoWay
                //};
                //checkBox.SetBinding(CheckBox.IsCheckedProperty, binding);
                if (i == MainWindow.Setting.Job.CACount + MainWindow.Setting.Job.BPCount) column = 1;
                if (i == MainWindow.Setting.Job.BPCount) column = 1;
                if (i < MainWindow.Setting.Job.BPCount) { Grid.SetRow(checkBox, row++); Grid.SetColumn(checkBox, 0); }
                else if (MainWindow.Setting.Job.BPCount <= i && i < MainWindow.Setting.Job.CACount + MainWindow.Setting.Job.BPCount) { Grid.SetRow(checkBox, 0);}
                else Grid.SetRow(checkBox, 1);
                if (i >= MainWindow.Setting.Job.BPCount) Grid.SetColumn(checkBox, column++);
                CheckBoxInspContainer.Children.Add(checkBox);
            }
        }
        private int WeekOfDay(DateTime time)
        {
            CultureInfo ci = CultureInfo.InvariantCulture;
            System.Globalization.Calendar cal = ci.Calendar;
            CalendarWeekRule rule = CalendarWeekRule.FirstFourDayWeek;
            DayOfWeek firstDayOfWeek = DayOfWeek.Monday;

            int weekOfYear = cal.GetWeekOfYear(time, rule, firstDayOfWeek);//월요일을 시작으로 4일이 남는지에 따라 1년 53인 년도에 대응한다

            return weekOfYear;
        }

        private void LotWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && this.txtLot.Text != string.Empty && this.txtOperator.Text != string.Empty && this.txtCode.Text != string.Empty && !txtLot.IsFocused)
            {
                CloseWithOK();
            }
            else if (e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            CloseWithOK();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void CloseWithOK()
        {
            if (m_ptrMainWindow == null)
            {
                m_ptrMainWindow = Application.Current.MainWindow as MainWindow;
                if (m_ptrMainWindow == null)
                    return;
            }

            #region 로트 번호 입력 유무.
            if (string.IsNullOrEmpty(txtLot.Text))
            {
                MessageBox.Show("로트 번호를 입력 하십시오", "Information");
                txtLot.Focus();
                return;
            }
            else
            {
                txtLot.Text = txtLot.Text.Trim();
            }

            if (string.IsNullOrEmpty(txtITSLot.Text))
            {
                MessageBox.Show("ITS 번호를 입력 하십시오", "Information");
                txtITSLot.Focus();
                return;
            }
            else
                txtITSLot.Text = txtITSLot.Text.Trim();

            if (pWarningInfo != null && pWarningInfo.RESULT_CODE == "H")
            {
                MessageBox.Show("HOLD 자재입니다. 검사를 시작할 수 없습니다.", "POP Information", MessageBoxButton.OK, MessageBoxImage.Error);
                ClientManager.Log(string.Format("Hold 자재 시작요구, Lot: {0}", txtLot.Text));
                return;
            }
            #endregion 로트 번호 입력 유무.

            #region 공정 코드 입력 유무.
            if (string.IsNullOrEmpty(txtCode.Text))
            {
                MessageBox.Show("공정 코드를 입력 하십시오", "Information");
                txtCode.Focus();
                return;
            }
            #endregion 공정 코드 입력 유무.

            #region POP 시작보고 필수 정보 판단
            if (MainWindow.Setting.General.UsePOP && bNormalLot)
            {
                if (m_ptrMainWindow.popManager.ServerHeartbeat())
                {
                    if (txtOperator.Text == "" || txtOperator.Text.Length != 6 || tbOperName.Text == "")
                    {
                        MessageBox.Show("작업자 정보가 올바르지 않습니다.", "Information");
                        txtOperator.Focus();
                        return;
                    }

                    if (curOpcode != "")
                    {
                        if (m_ptrMainWindow.popManager.GetOpStateCheck(txtLot.Text, out POP_START_STATE_OUTPUT_DATA output, out errMsg) && output != null)
                        {
                            if (output.CHECK_CD == "NG")
                            {
                                if (MessageBox.Show(string.Format("{0}\n'D58-재작업 및 재검' 유실코드로 비가동 처리하시겠습니까?", output.CHECK_MSG), "확인", MessageBoxButton.YesNo) == MessageBoxResult.No)
                                    return;

                                ClientManager.Log(string.Format("{0}. D58 유실코드로 비가동 진행", output.CHECK_MSG));
                                bRestart = true;
                            }
                        }
                        else
                            ClientManager.Log(string.Format("IGS - GetOpStateCheck Fail. Error: {0}", errMsg));
                    }
                }
            }
            else
            {
                bNormalLot = false;
                ClientManager.Log(string.Format("POP 미사용 or 테스트 오더로 검사 진행, Lot: {0}", txtLot.Text));
            }
            #endregion

            #region Verify 적용 여부.
            if ((bool)chkVerify.IsChecked)
            {
                MainWindow.Log("PCS", SeverityLevel.INFO, "Verify 작업을 수행합니다.");
                MainWindow.Setting.General.UseRVS = true;
            }
            else
            {
                MainWindow.Log("PCS", SeverityLevel.INFO, "Verify 작업을 수행하지 않습니다.");
                // m_ptrMainWindow.InspectionMonitoringCtrl.ResultTable.VerifySkipBorder.Visibility = Visibility.Visible;
                MainWindow.Setting.General.UseRVS = false;
            }
            #endregion Verify 적용 여부.
            #region AI 적용 여부.
            if ((bool)chkUseAI.IsChecked)
            {
                MainWindow.Log("PCS", SeverityLevel.INFO, "ICS 작업을 수행합니다.");
            }
            else
            {
                MainWindow.Log("PCS", SeverityLevel.INFO, "ICS 작업을 수행하지 않습니다.");
                // m_ptrMainWindow.InspectionMonitoringCtrl.ResultTable.VerifySkipBorder.Visibility = Visibility.Visible;
            }
            #endregion Verify 적용 여부.

            #region 검사 유무 판단.
            // 작업자 요구사항 : 상부, 하부, 투과에 대해 선택적으로 검사를 수행할 수 있게 변경해 주세요.
            // 2012-04-30 suoow2.

            bool[] bInspCheckButton = new bool[CheckBoxInspContainer.Children.Count];
            int nEnableBP = 0; int nEnableCA = 0; int nEnableBA = 0;
            for (int i = 0; i < CheckBoxInspContainer.Children.Count; i++)
            {
                if (CheckBoxInspContainer.Children[i] is CheckBox checkBox)
                {
                    bInspCheckButton[i] = checkBox.IsChecked ?? false;
                }
            }
            if (bInspCheckButton.All(c => c == false))
            {
                MessageBox.Show("최소 1면 이상은 검사에 포함시켜야 합니다.", "Information");
                return;
            }
            else
            {
                string szWarnMessage = string.Empty;         
                for (int i = 0; i < CheckBoxInspContainer.Children.Count; i++)
                {
                    if (CheckBoxInspContainer.Children[i] is CheckBox checkBox)
                    {
                        if(checkBox.IsChecked == false)
                        {
                            szWarnMessage += checkBox.Content.ToString() + " : 검사 제외\n";
                        }
                        else
                        {
                            if (checkBox.Name.Contains(CategorySurface.BP.ToString())) nEnableBP++;
                            else if (checkBox.Name.Contains(CategorySurface.CA.ToString())) nEnableCA++;
                            else nEnableBA++;
                        }
                    }
                }

                szWarnMessage += "\n현재 설정으로 진행하시겠습니까?";

                if (MessageBox.Show(szWarnMessage, "검사 제외 확인", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;
            }

            //customer
            //m_Customer = Customer_Combo.SelectedIndex;
            //MainWindow.Setting.Job.Customer = m_Customer;
            //m_ptrMainWindow.m_Customer = m_Customer;
            //customer
            //  else szInspect = "1,1,1,1";
            #endregion 검사 유무 판단.

            MainWindow.CurrentModel.UseMarking = (bool)cbMarkEnable.IsChecked;
            //마킹 옵션 추가 설정 할것.
            string s = "";


            for (int i= 0; i< cb_autoNG_Items.Count; i++)
            {
                 MainWindow.AutoNG_Check[i] = (bool)cb_autoNG_Items[i].IsChecked;
            }

            for (int i = 0; i < 100; i++)
            {
                if (MainWindow.AutoNG_Check[i]) s += "1,";
                else s += "0,";
            }

            MainWindow.Setting.Job.AutoNG = s;
            MainWindow.Setting.Job.LastLot = txtLot.Text;
            MainWindow.Setting.Job.LastItsLot = txtITSLot.Text;
            MainWindow.Setting.Job.ProcessCode = txtCode.Text;
            MainWindow.CurrentModel.UseVerify = (bool)chkVerify.IsChecked;
            MainWindow.CurrentModel.UseAI = (bool)chkUseAI.IsChecked;
            MainWindow.Setting.Job.Save();
            MainWindow.Setting.General.Save();
            MainWindow.Setting.SubSystem.Save();

            if (curLot != null && curLot.WONO == txtLot.Text.Trim())
                MainWindow.curLotData = curLot;
            else
            {
                MainWindow.curLotData = new POP_LOT_DATA();
                MainWindow.curLotData.WONO = txtLot.Text.Trim();
                MainWindow.curLotData.ITS_ORDER = txtITSLot.Text.Trim();
            }

            m_ptrMainWindow.POPStartPara.strGroup = MainWindow.CurrentGroup.Name;
            m_ptrMainWindow.POPStartPara.strModel = MainWindow.CurrentModel.Name;
            m_ptrMainWindow.POPStartPara.strLocalLot = String.Format("{0}{1}", txtLot.Text.Trim(), cbLotType.Text);
            m_ptrMainWindow.POPStartPara.strLot = txtLot.Text.Trim();
            m_ptrMainWindow.POPStartPara.strOperator = txtOperator.Text;
            m_ptrMainWindow.POPStartPara.standModel = stdModel;
            m_ptrMainWindow.POPStartPara.bNormalLot = bNormalLot;
            m_ptrMainWindow.POPStartPara.bRestart = bRestart;

            //if (lot_valid)
            //{
            //    string temp = "";
            //    string sLot = "0000";

            //    int index = txtLot.Text.IndexOf("10");
            //    temp = txtLot.Text.Substring(index, txtLot.Text.Length - index);

            //    if (temp.Length >= 9)
            //        temp = temp.Substring(0, 9);

            //    if (m_Customer == 0)
            //    {
            //        if (temp.Length > 5)
            //            sLot = temp.Substring(temp.Length - 5, 5);
            //        else
            //            sLot = temp;

            //        txtIDMark.Text = sLot;
            //        txtFirstMark.Text = txtDate.Text + txtIDMark.Text;
            //    }
            //    else
            //    {
            //        if (temp.Length > 9)
            //            sLot = temp.Substring(temp.Length - 9, 9);
            //        else
            //            sLot = temp;
            //        txtIDMark.Text = /*"0" +*/ sLot + "W" + "000";
            //        txtFirstMark.Text = txtDate.Text + txtIDMark.Text /*+ "000"*/;
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Lot번호 오류.");
            //    return;
            //}

            m_ptrMainWindow.StartTime = DateTime.Now;
            
            m_ptrMainWindow.InspectionMonitoringCtrl.TableDataCtrl.RunTime = new TimeSpan(0, 0, 0);
            m_ptrMainWindow.Week = txtWeek.Text;
            if(MainWindow.Setting.SubSystem.PLC.UsePLC) m_ptrMainWindow.MainToolBarCtrl.InspectionStart();
            m_ptrMainWindow.InspectionMonitoringCtrl.ResultTable.TableData.Init();
            m_ptrMainWindow.InspectionMonitoringCtrl.GetLiveImage = false; // chkLiveImage.IsChecked == true ? true : false;

            //MainWindow.IDString = txtFirstMark.Text;
            if (MainWindow.CurrentModel.InspectMode == 4)
            {
                string strFolderPath = String.Format(@"{0}/{1}/", MainWindow.Setting.General.IDMarkPath, txtLot.Text);
                if (Directory.Exists(strFolderPath))
                {
                    string[] files = Directory.GetFiles(strFolderPath);
                    foreach (string f in files)
                    {
                        if (f.Contains(".xml"))
                        {
                            FileInfo fi = new FileInfo(f);
                            string[] ss = fi.Name.Split('.');
                            if (m_Customer == 0)//break;
                                MainWindow.IDString = ss[0].Substring(0, ss[0].Length - 5);
                            else
                                MainWindow.IDString = ss[0].Substring(0, ss[0].Length - 3);
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("상부 검사 이력 없음.");
                    return;
                }
            }
            //m_ptrMainWindow.First_Mark = txtFirstMark.Text;

            this.DialogResult = true;
            this.Close();
        }

        void txtDate_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (m_Customer == 0)
            //{
            //    txtFirstMark.Text = txtDate.Text + txtIDMark.Text + "00000";
            //}
            //else
            //{
            //    txtFirstMark.Text = txtDate.Text + txtIDMark.Text + "000";
            //}
        }

        void textBox_TextChaned(object sender, TextChangedEventArgs e)
        {
            //string sLot = "0000";
            //string temp = "";
            //Action action = null;
            //lot_valid = false;
            //try
            //{
            //    int index = txtLot.Text.IndexOf("10");//10으로 시작
            //    temp = txtLot.Text.Substring(index, txtLot.Text.Length - index);

            //    if (temp.Length >= 9)
            //    {
            //        temp = temp.Substring(0, 9);
            //        lot_valid = true;
            //    }
            //}
            //catch
            //{
            //    temp = txtLot.Text;
            //    lot_valid = false;
            //}

            //try
            //{
            //    if (m_Customer == 0)
            //    {
            //        if (temp.Length > 5)
            //            sLot = temp.Substring(temp.Length - 5, 5);
            //        else
            //            sLot = temp;

            //        txtIDMark.Text = /*"H" + (DateTime.Now.Year % 100).ToString("00") + MonthTo() +*/ sLot;
            //        txtFirstMark.Text = txtDate.Text + txtIDMark.Text + "00000";
            //    }
            //    else
            //    {
            //        if (temp.Length > 9)
            //            sLot = temp.Substring(temp.Length - 9, 9);
            //        else
            //            sLot = temp;

            //        txtIDMark.Text = /*"0" +*/ sLot + "W" + "000";
            //        txtFirstMark.Text = txtDate.Text + txtIDMark.Text + "000";
            //    }
            //}
            //catch (Exception)
            //{
            //};

            //if (lot_valid)
            //{
            //    action = delegate
            //    {
            //        txtIDMark.Foreground = System.Windows.Media.Brushes.Black;
            //    }; this.Dispatcher.Invoke(action);
            //}
            //else
            //{
            //    action = delegate
            //    {
            //        txtIDMark.Foreground = System.Windows.Media.Brushes.Red;
            //    }; this.Dispatcher.Invoke(action);
            //}
        }

        private void Customer_Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string temp = "";
            //string sLot = "";
            //bool lot_enable = false;
            //if (txtLot.Text != "")
            //{
            //    int index = txtLot.Text.IndexOf("10");//10으로 시작
            //    temp = txtLot.Text.Substring(index, txtLot.Text.Length - index);
            //    sLot = "";

            //    if (temp.Length >= 9)
            //    {
            //        temp = temp.Substring(0, 9);
            //    }
            //    lot_enable = true;
            //}

            //if (Customer_Combo.SelectedIndex == 0)
            //{
            //    m_Customer = 0;
            //    txtDate.Text = "H" + (DateTime.Now.Year % 100).ToString("00") + MonthTo();
            //    MainWindow.IDString = txtDate.Text + "00000";

            //    if (lot_enable)
            //    {
            //        if (temp.Length > 5)
            //            sLot = temp.Substring(temp.Length - 5, 5);
            //        else
            //            sLot = temp;

            //        txtIDMark.Text = sLot;
            //    }
            //    else
            //        txtIDMark.Text = "00000";
            //    txtFirstMark.Text = txtDate.Text + txtIDMark.Text + "00000";
            //}
            //else
            //{
            //    m_Customer = 1;
            //    txtDate.Text = "0";
            //    MainWindow.IDString = txtDate.Text + "000000000" + "W" + "000";

            //    if (lot_enable)
            //    {
            //        if (temp.Length > 9)
            //            sLot = temp.Substring(temp.Length - 9, 9);
            //        else
            //            sLot = temp;

            //        txtIDMark.Text = /*"0" +*/ sLot + "W" + "000";
            //    }
            //    else
            //        txtIDMark.Text = "000000000W000";
            //    txtFirstMark.Text = txtDate.Text + txtIDMark.Text + "000";
            //}
        }
    }
}
