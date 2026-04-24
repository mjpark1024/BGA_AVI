using IGS.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IGS.SubWindow
{
    /// <summary>
    /// POP_ATEND_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class POP_ATTEND_Window : Window
    {
        #region Member Variables.
        public ObservableCollection<StateDisplayData> States
        {
            get { return m_state; }
        }
        private ObservableCollection<StateDisplayData> m_state = new ObservableCollection<StateDisplayData>();

        private List<string> myMCList = new List<string>();

        private Thread updateThd;
        private bool bUpdate = true;
        private bool bPause = false;

        private POP_USER_DATA curUser;
        private List<STATE_TABLE_DATA> stateList = new List<STATE_TABLE_DATA>();
        #endregion

        public POP_ATTEND_Window()
        {
            InitializeComponent();
            InitializeEvent();
            InitializeDialog();
        }

        private void InitializeEvent()
        {
            this.btnSearch.Click += BtnSearch_Click;
            this.btnCancel.Click += BtnCancel_Click;
            this.btnOK.Click += BtnOK_Click;
            this.btnMyMc.Click += BtnMyMc_Click;

            this.lbInfo.PreviewMouseWheel += LbInfo_PreviewMouseWheel;
            this.bdTitle.PreviewMouseLeftButtonDown += BdTitle_PreviewMouseLeftButtonDown;
            this.tbUserID.PreviewKeyDown += TbUserID_PreviewKeyDown;

            this.tbUserID.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;
            this.tbHour.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;
            this.tbMinute.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;

            this.cbAll.Click += CbAll_Click;
            this.Closed += POP_ATEND_Window_Closed;
        }

        private void InitializeDialog()
        {
            this.btnOK.IsEnabled = false;
            lbInfo.IsEnabled = false;

            InitializeState();
            lbInfo.DataContext = m_state;

            DateTime now = DateTime.Now;
            dpDate.SelectedDate = now;
            TimeSpan span = now.TimeOfDay;

            if (span >= new TimeSpan(6, 30, 0) && span <= new TimeSpan(7, 30, 0))
            {
                tbHour.Text = "07";
                tbMinute.Text = "00";
            }
            else if (span >= new TimeSpan(14, 30, 0) && span <= new TimeSpan(15, 30, 0))
            {
                tbHour.Text = "15";
                tbMinute.Text = "00";
            }
            else if (span >= new TimeSpan(18, 30, 0) && span <= new TimeSpan(19, 30, 0))
            {
                tbHour.Text = "19";
                tbMinute.Text = "00";
            }
            else if (span >= new TimeSpan(22, 30, 0) && span <= new TimeSpan(23, 30, 0))
            {
                tbHour.Text = "23";
                tbMinute.Text = "00";
            }
            else
            {
                tbHour.Text = now.Hour.ToString("D2");
                tbMinute.Text = now.Minute.ToString("D2");
            }

            updateThd = new Thread(UpdateInfo);
            updateThd.Start();

            tbUserID.Focus();

            if (UserCtrl.curUser != null)
            {
                tbUserID.Text = UserCtrl.curUser.USERID;
                BtnSearch_Click(null, null);
            }
        }

        private void InitializeState()
        {
            m_state.Clear();
            if (ReportMenuCtrl.clientManager.GetStateInfo(out stateList) && stateList != null)
            {
                foreach (STATE_TABLE_DATA data in stateList)
                {
                    StateDisplayData disp = new StateDisplayData();

                    disp.bChecked = false;
                    disp.mc_code = data.MACHINE_CODE;
                    if (data.STATE == "RUN")
                    {
                        disp.state = "가동중";
                        disp.imagePath = "../Images/state_green.png";
                    }
                    else
                    {
                        disp.state = "비가동";
                        disp.imagePath = "../Images/state_yellow.png";
                    }

                    disp.order = data.ORDER;
                    disp.exc_code = data.EXC_CD;
                    disp.exc_name = data.EXC_NM;
                    disp.exc_time = data.EXC_START_TIME;
                    disp.user_name = data.USER_NAME;

                    m_state.Add(disp.Clone());
                }
            }
            else
            {
                MessageBox.Show("설비 리스트를 불러오는데 문제가 발생하였습니다.");
                ClientManager.Log(string.Format("IGS - POP_ATTEND_Window.cs GetStateInfo Fail"));
            }
        }

        private void UpdateInfo()
        {
            try
            {
                Action action;
                List<STATE_TABLE_DATA> thdList = new List<STATE_TABLE_DATA>();

                while (bUpdate)
                {
                    if (bPause)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    try
                    {
                        if (ReportMenuCtrl.clientManager.GetStateInfo(out thdList))
                        {
                            foreach (STATE_TABLE_DATA data in thdList)
                            {
                                StateDisplayData disp = new StateDisplayData();

                                disp.mc_code = data.MACHINE_CODE;
                                if (data.STATE == "RUN")
                                {
                                    disp.state = "가동중";
                                    disp.imagePath = "../Images/state_green.png";
                                }
                                else
                                {
                                    disp.state = "비가동";
                                    disp.imagePath = "../Images/state_yellow.png";
                                }

                                disp.order = data.ORDER;
                                disp.exc_code = data.EXC_CD;
                                disp.exc_name = data.EXC_NM;
                                disp.exc_time = data.EXC_START_TIME;
                                disp.user_name = data.USER_NAME;

                                if (m_state.Any(m => m.mc_code == data.MACHINE_CODE))
                                {
                                    StateDisplayData now = m_state.First(m => m.mc_code == data.MACHINE_CODE);

                                    action = delegate
                                    {
                                        if (myMCList.Count != 0 && myMCList.IndexOf(data.MACHINE_CODE) == -1)
                                        {
                                            m_state.Remove(now);
                                            lbInfo.DataContext = m_state;
                                        }
                                        else if (!now.Compare(disp))
                                            now.Replace(disp);
                                    }; this.Dispatcher.Invoke(action);
                                }
                                else if (myMCList.Count == 0 || myMCList.IndexOf(data.MACHINE_CODE) != -1)
                                {
                                    action = delegate
                                    {
                                        m_state.Add(disp.Clone());
                                        m_state = new ObservableCollection<StateDisplayData>(m_state.OrderBy(m => m.mc_code));
                                        lbInfo.DataContext = m_state;
                                    }; this.Dispatcher.Invoke(action);
                                }
                            }

                            Thread.Sleep(100);
                        }
                    }
                    catch(Exception exc) { }
                }
            }
            catch(Exception ex)
            {
                string tmp = ex.Message;
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string strID = tbUserID.Text.Trim();
                if (strID == string.Empty || strID == "")
                {
                    MessageBox.Show("조회하고자 하는 사번을 입력해주세요.");
                    return;
                }

                if (strID.Length != 6)
                {
                    MessageBox.Show("사번은 6자리입니다. 다시 확인바랍니다.");
                    return;
                }
                tbUserName.Text = "";

                POP_USER_DATA data;
                string errMsg;
                if (ReportMenuCtrl.clientManager.GetUserInfo(strID, out data, out errMsg) && data != null)
                {
                    curUser = data;
                    tbUserName.Text = data.USER_NM;

                    lbInfo.IsEnabled = true;
                    btnOK.IsEnabled = true;

                    ReportMenuCtrl.clientManager.GetMyMCList(strID, out myMCList);                    
                }
                else
                    MessageBox.Show(string.Format("입력된 사번으로 MES에서 정보를 조회할 수 없습니다.\n에러메세지: {0}", errMsg));
            }
            catch(Exception ex)
            {
                ClientManager.Log(string.Format("IGS - POP_ATTEND_Window.cs BtnSearch_Click Exception: {0}", ex.Message));
            }
        }

        private void TbUserID_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BtnSearch_Click(null, null);
        }

        private void Tbox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void CbAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (StateDisplayData data in m_state)
                data.bChecked = (bool)cbAll.IsChecked;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!POP_Report())
                return;

            MessageBox.Show("정상적으로 보고가 완료되었습니다.");
        }

        private void BtnMyMc_Click(object sender, RoutedEventArgs e)
        {
            SubWindow.MY_MACHINE_Window dlg = new MY_MACHINE_Window();
            dlg.InitializeDialog(curUser);
            if ((bool)dlg.ShowDialog())
            {
                if (curUser != null)
                    ReportMenuCtrl.clientManager.GetMyMCList(curUser.USERID, out myMCList);
            }
        }

        private bool POP_Report()
        {
            try
            {
                List<string> mcList = new List<string>();
                foreach (StateDisplayData data in m_state)
                {
                    if (data.bChecked)
                        mcList.Add(data.mc_code);
                }

                if (mcList.Count == 0)
                {
                    MessageBox.Show("출근하고자 하는 설비를 적어도 하나 이상 선택해야 합니다.");
                    return false;
                }

                if (dpDate.SelectedDate == null || tbHour.Text == "" || tbMinute.Text == "")
                {
                    MessageBox.Show("출근 시간을 정확히 입력바랍니다.");
                    return false;
                }

                int nHour = 0;
                int nMin = 0;
                try
                {
                    nHour = Convert.ToInt32(tbHour.Text);
                    nMin = Convert.ToInt32(tbMinute.Text);

                    if (nHour < 0 || nHour > 23)
                    {
                        MessageBox.Show("시간 정보가 올바르지 않습니다.");
                        tbHour.Focus();
                        return false;
                    }
                    else if (nMin < 0 || nMin > 60)
                    {
                        MessageBox.Show("시간 정보가 올바르지 않습니다.");
                        tbMinute.Focus();
                        return false;
                    }
                }
                catch
                {
                    MessageBox.Show("시간은 숫자만 입력할 수 있습니다.\n다시 입력바랍니다.");
                    return false;
                }

                string strDate = ((DateTime)dpDate.SelectedDate).ToString("yyyyMMdd");
                string strTime = nHour.ToString("D2") + nMin.ToString("D2") + "00";

                foreach (StateDisplayData data in m_state)
                {
                    if (mcList.IndexOf(data.mc_code) == -1)
                        continue;

                    bool bTimeIn = false;
                    STATE_TABLE_DATA curState = stateList.First(s => s.MACHINE_CODE == data.mc_code);
                    DateTime reportTime = DateTime.ParseExact((strDate + strTime), "yyyyMMddHHmmss", null);
                    TimeSpan span = reportTime.TimeOfDay;

                    //3교대 (07, 15, 21시)
                    if (curState.SHIFT_WORK == "A0")
                    {
                        if (span >= new TimeSpan(6, 40, 0) && span <= new TimeSpan(7, 20, 0))
                            bTimeIn = true;
                        else if (span >= new TimeSpan(14, 40, 0) && span <= new TimeSpan(15, 20, 0))
                            bTimeIn = true;
                        else if (span >= new TimeSpan(20, 40, 0) && span >= new TimeSpan(21, 20, 0))
                            bTimeIn = true;
                    }
                    //2교대 (07, 19시)
                    else if (curState.SHIFT_WORK == "A2" || curState.SHIFT_WORK == "A3")
                    {
                        if (span >= new TimeSpan(06, 30, 0) && span <= new TimeSpan(7, 30, 0))
                            bTimeIn = true;
                        else if (span >= new TimeSpan(18, 30, 0) && span <= new TimeSpan(19, 30, 0))
                            bTimeIn = true;
                    }

                    if (!bTimeIn)
                    {
                        if (MessageBox.Show(string.Format("{0} 설비의 지정 교대시간이 아닙니다. 계속 진행하시겠습니까?", data.mc_code), "확인", MessageBoxButton.YesNo) == MessageBoxResult.No)
                            return false;
                        else
                            break;
                    }
                }

                JArray attendMC = new JArray();

                //CHECK VERIFY YN
                if (stateList != null && stateList.Any(s => mcList.IndexOf(s.MACHINE_CODE) != -1 && s.VERIFY_YN == "Y"))
                {
                    ATTEND_VERIFY_Window dlg = new ATTEND_VERIFY_Window(mcList, curUser.USERID);
                    if ((bool)dlg.ShowDialog())
                    {
                        foreach (VerifyUserDisplayData user in dlg.Users)
                        {
                            JObject mcInfo = new JObject();
                            mcInfo.Add("MC_CODE", user.mc_code);
                            mcInfo.Add("VERIFY_USER", user.verify_user);

                            attendMC.Add(mcInfo);
                        }
                    }
                    else
                        return false;
                }
                else
                {
                    foreach (string mc in mcList)
                    {
                        JObject mcInfo = new JObject();
                        mcInfo.Add("MC_CODE", mc);
                        mcInfo.Add("VERIFY_USER", "");

                        attendMC.Add(mcInfo);
                    }
                }

                JObject request = new JObject();
                request.Add("USER_ID", curUser.USERID);
                request.Add("ATTEND_DT", strDate);
                request.Add("ATTEND_TM", strTime);
                request.Add("MC_LIST", attendMC);

                string request_msg = JsonConvert.SerializeObject(request);
                string errMsg;

                if (!ReportMenuCtrl.clientManager.SetReportOneWay(ReportType.WORK_ON_REPORT, request_msg, out errMsg))
                {
                    MessageBox.Show(string.Format("보고에 실패하였습니다. 실패사유: {0}", errMsg));
                    ClientManager.Log(string.Format("IGS - POP_ATTEND_Window.cs POP_Report Fail. Error:{0}", errMsg));
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("IGS - POP_ATTEND_Window.cs POP_Report Exception: {0}", ex.Message));
                MessageBox.Show(string.Format("보고 과정에 문제가 발생하였습니다.\nException: {0}", ex.Message));
                return false;
            }
        }

        private void POP_ATEND_Window_Closed(object sender, EventArgs e)
        {
            bUpdate = false;
            updateThd.Join(2000);
            updateThd.Abort();
        }

        private void LbInfo_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroll = (ScrollViewer)svInfo;
            if (e.Delta < 0)
            {
                if (scroll.VerticalOffset - e.Delta < scroll.ExtentHeight - scroll.ViewportHeight)
                    scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta);
                else
                    scroll.ScrollToBottom();
            }
            else
            {
                if (scroll.VerticalOffset + e.Delta > 0)
                    scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta);
                else
                    scroll.ScrollToTop();
            }
            e.Handled = true;
        }

        private void BdTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
