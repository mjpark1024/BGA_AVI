using IGS.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace IGS.SubWindow
{
    /// <summary>
    /// BREAKTIME_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BREAKTIME_Window : Window
    {
        #region Member Variables.
        public ObservableCollection<BreakTimeDisplayData> breakTimes
        {
            get { return m_breakTimes; }
        }
        private ObservableCollection<BreakTimeDisplayData> m_breakTimes = new ObservableCollection<BreakTimeDisplayData>();

        private STATE_TABLE_DATA curState;
        #endregion

        public BREAKTIME_Window()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.btnOK.Click += BtnOK_Click;
            this.btnAdd.Click += BtnAdd_Click;

            this.tbStdStartHour.GotFocus += Tbbox_GotFocus;
            this.tbStdStartMin.GotFocus += Tbbox_GotFocus;
            this.tbStdEndHour.GotFocus += Tbbox_GotFocus;
            this.tbStdEndMin.GotFocus += Tbbox_GotFocus;
            this.tbCalcStartHour.GotFocus += Tbbox_GotFocus;
            this.tbCalcStartMin.GotFocus += Tbbox_GotFocus;
            this.tbCalcEndHour.GotFocus += Tbbox_GotFocus;
            this.tbCalcEndMin.GotFocus += Tbbox_GotFocus;
            this.tbDetail.GotFocus += Tbbox_GotFocus;
            this.tbRunTime.GotFocus += Tbbox_GotFocus;
            this.tboxLossCode.GotFocus += Tbbox_GotFocus;

            this.tbCalcStartHour.GotFocus += TbCalcStartHour_GotFocus;
            this.tbCalcEndHour.GotFocus += TbCalcEndHour_GotFocus;

            this.tboxLossCode.PreviewKeyUp += TboxLossCode_PreviewKeyUp;

            this.lbInfo.PreviewMouseWheel += LbInfo_PreviewMouseWheel;
            this.bdTitle.PreviewMouseLeftButtonDown += BdTitle_PreviewMouseLeftButtonDown;
        }

        private void InitializeDialog()
        {
            //GET STATE INFO
            if (!ReportMenuCtrl.clientManager.GetCurStateInfo(out curState) || curState == null)
            {
                MessageBox.Show("서버 정보를 불러오는데 실패하였습니다.\n설비 상태 확인 실패.");
                ClientManager.Log(string.Format("IGS - BREAKTIME_Window GetCurStateInfo Fail"));
                return;
            }

            //SET LOSSCODE
            List<string> codeList;
            string errMsg;
            if (!ReportMenuCtrl.clientManager.GetBreakCodeList(curState.USER_ID, out codeList, out errMsg))
            {
                MessageBox.Show(string.Format("유실 코드 정보를 불러오지 못했습니다.\nErrorCode: {0}", errMsg));
                ClientManager.Log(string.Format("IGS - BREAKTIME_Window GetBreakCodeList Fail. Error: {0}", errMsg));
                return;
            }

            LoadTimeInfo();

            cbLossCode.Items.Clear();
            foreach (string code in codeList)
                cbLossCode.Items.Add(code);
        }

        private void LoadTimeInfo()
        {
            //GET TIME INFO
            List<BreakTimeDisplayData> times;
            if (!ReportMenuCtrl.clientManager.GetBreakTimeList(out times))
            {
                MessageBox.Show(string.Format("자동 유실 설정 정보를 불러오지 못했습니다."));
                ClientManager.Log(string.Format("IGS - BREAKTIME_Window GetBreakTimeList Fail."));
                return;
            }

            breakTimes.Clear();
            if (times != null)
            {
                times.Sort((x, y) => TimeSpan.Parse(x.from_time).TotalMinutes.CompareTo(TimeSpan.Parse(y.from_time).TotalMinutes));

                foreach (BreakTimeDisplayData time in times)
                    breakTimes.Add(time);
            }

            lbInfo.DataContext = breakTimes;

            tbStdStartHour.Text = tbStdStartMin.Text = tbStdEndHour.Text = tbStdEndMin.Text = "";
            tbCalcStartHour.Text = tbCalcStartMin.Text = tbCalcEndHour.Text = tbCalcEndMin.Text = "";
            tbDetail.Text = "";
            tbRunTime.Text = "";
            tboxLossCode.Text = "";
            cbLossCode.SelectedIndex = -1;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tbStdStartHour.Text == "" || tbStdStartMin.Text == "" || tbStdEndHour.Text == "" || tbStdEndMin.Text == "")
            {
                MessageBox.Show("규정 시간을 정확하게 입력바랍니다.");
                return;
            }

            if (tbCalcStartHour.Text == "" || tbCalcStartMin.Text == "" || tbCalcEndHour.Text == "" || tbCalcEndMin.Text == "")
            {
                MessageBox.Show("판단 시간을 정확하게 입력바랍니다.");
                return;
            }

            if (tbDetail.Text == "")
            {
                MessageBox.Show("내용을 입력바랍니다.");
                tbDetail.Focus();
                return;
            }

            if (tbRunTime.Text == "")
            {
                MessageBox.Show("소요시간을 입력바랍니다.");
                tbRunTime.Focus();
                return;
            }

            if (cbLossCode.SelectedIndex == -1)
            {
                MessageBox.Show("유실 코드를 선택바랍니다.");
                return;
            }

            if (MessageBox.Show("입력된 정보로 기준 추가 요청을 하시겠습니까?", "확인", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            try
            {
                BreakTimeDisplayData time = new BreakTimeDisplayData();

                int nHour = Convert.ToInt32(tbStdStartHour.Text);
                int nMin = Convert.ToInt32(tbStdStartMin.Text);
                if (nHour < 0 || nHour > 23 || nMin < 0 || nMin > 60)
                {
                    MessageBox.Show("시간 정보가 올바르지 않습니다.");
                    return;
                }
                time.from_time = string.Format("{0:D2}:{1:D2}:00", nHour, nMin);

                nHour = Convert.ToInt32(tbStdEndHour.Text);
                nMin = Convert.ToInt32(tbStdEndMin.Text);
                if (nHour < 0 || nHour > 23 || nMin < 0 || nMin > 60)
                {
                    MessageBox.Show("시간 정보가 올바르지 않습니다.");
                    return;
                }
                time.to_time = string.Format("{0:D2}:{1:D2}:00", nHour, nMin);

                time.detail = tbDetail.Text;

                string[] spt = cbLossCode.SelectedItem.ToString().Split(',');
                time.exc_cd = spt[0];
                time.exc_nm = spt[1];

                time.run_time = Convert.ToInt32(tbRunTime.Text);

                nHour = Convert.ToInt32(tbCalcStartHour.Text);
                nMin = Convert.ToInt32(tbCalcStartMin.Text);
                if (nHour < 0 || nHour > 23 || nMin < 0 || nMin > 60)
                {
                    MessageBox.Show("시간 정보가 올바르지 않습니다.");
                    return;
                }
                time.from_std = string.Format("{0:D2}:{1:D2}:00", nHour, nMin);

                nHour = Convert.ToInt32(tbCalcEndHour.Text);
                nMin = Convert.ToInt32(tbCalcEndMin.Text);
                if (nHour < 0 || nHour > 23 || nMin < 0 || nMin > 60)
                {
                    MessageBox.Show("시간 정보가 올바르지 않습니다.");
                    return;
                }
                time.to_std = string.Format("{0:D2}:{1:D2}:00", nHour, nMin);

                if (!ReportMenuCtrl.clientManager.SetBreakTime(time, curState.USER_ID))
                {
                    MessageBox.Show("요청하는 과정에서 문제가 발생하였습니다.");
                    return;
                }

                MessageBox.Show("요청이 완료되었습니다.\n서버 관리자의 승인 후 적용됩니다.");

                LoadTimeInfo();
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format("저장하는 과정에서 문제가 발생하였습니다.\nError:{0}", ex.Message), "ERROR");
                ClientManager.Log(string.Format("IGS - BREAKTIME_Window Add Fail. Error:{0}", ex.Message));
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("선택한 정보를 정말로 삭제하시겠습니까?", "확인", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;

                int nIndex = Convert.ToInt32(((Button)sender).Tag.ToString());
                if (!ReportMenuCtrl.clientManager.DeleteBreakTime(nIndex))
                    MessageBox.Show("삭제하는 과정에서 문제가 발생하였습니다.");
                else
                    MessageBox.Show("삭제가 완료되었습니다.");
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format("삭제하는 과정에서 문제가 발생하였습니다.\nError:{0}", ex.Message), "ERROR");
                ClientManager.Log(string.Format("IGS - BREAKTIME_Window Delete Fail. Error:{0}", ex.Message));
            }
            finally
            {
                LoadTimeInfo();
            }
        }

        private void Tbbox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void TbCalcStartHour_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tbCalcStartHour.Text == "" && tbCalcStartMin.Text == "" && tbStdStartHour.Text != "" && tbStdStartMin.Text != "")
                {
                    int nMargin = 10;

                    int nStartHour = Convert.ToInt32(tbStdStartHour.Text);
                    int nStartMin = Convert.ToInt32(tbStdStartMin.Text);

                    string strStart = string.Format("{0:D2}:{1:D2}:00", nStartHour, nStartMin);
                    TimeSpan startSpan = TimeSpan.Parse(strStart);
                    TimeSpan subSpan = startSpan.Subtract(new TimeSpan(0, nMargin, 0));

                    if (subSpan.Minutes < 0)
                        subSpan = subSpan.Add(new TimeSpan(24, 0, 0));

                    tbCalcStartHour.Text = subSpan.Hours.ToString("D2");
                    tbCalcStartMin.Text = subSpan.Minutes.ToString("D2");
                }
            }
            catch { }
        }

        private void TbCalcEndHour_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tbCalcEndHour.Text == "" && tbCalcEndMin.Text == "" && tbStdEndHour.Text != "" && tbStdEndMin.Text != "")
                {
                    int nMargin = 10;

                    int nEndHour = Convert.ToInt32(tbStdEndHour.Text);
                    int nEndMin = Convert.ToInt32(tbStdEndMin.Text);

                    string strEnd = string.Format("{0:D2}:{1:D2}:00", nEndHour, nEndMin);
                    TimeSpan endSpan = TimeSpan.Parse(strEnd);

                    tbCalcEndHour.Text = endSpan.Add(new TimeSpan(0, nMargin, 0)).Hours.ToString("D2");
                    tbCalcEndMin.Text = endSpan.Add(new TimeSpan(0, nMargin, 0)).Minutes.ToString("D2");
                }
            }
            catch { }
        }

        private void TboxLossCode_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (tboxLossCode.Text == "") return;

            if (e.Key == Key.Enter)
                BtnAdd_Click(null, null);
            else
            {
                foreach (string code in cbLossCode.Items)
                {
                    if (code.Contains(tboxLossCode.Text.ToUpper()))
                    {
                        cbLossCode.SelectedItem = code;
                        break;
                    }
                }
            }
        }

        private void LbInfo_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroll = (ScrollViewer)svTimeInfo;
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
