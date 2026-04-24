using IGS.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

namespace IGS.SubWindow
{
    /// <summary>
    /// POP_EXCEPT_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class POP_EXCEPT_Window : Window
    {
        List<string> lossList = new List<string>();

        public POP_EXCEPT_Window()
        {
            InitializeComponent();
            InitializeEvent();
            InitializeDialog();
        }

        private void InitializeEvent()
        {
            this.btnCancel.Click += BtnCancel_Click;
            this.btnOK.Click += BtnOK_Click;
            this.btnBreakTime.Click += BtnBreakTime_Click;
            this.btnSearch.Click += BtnSearch_Click;

            this.cbLossCode.SelectionChanged += CbLossCode_SelectionChanged;
            this.tbLossCode.TextChanged += TbLossCode_TextChanged;
            this.tboxOper.PreviewKeyUp += TboxOper_PreviewKeyUp;

            this.tboxOper.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;
            this.tbLossCode.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;
            this.bdTitle.PreviewMouseLeftButtonDown += BdTitle_PreviewMouseLeftButtonDown;
        }

        private void InitializeDialog()
        {
            btnOK.IsEnabled = false;

            //GET STATE INFO
            STATE_TABLE_DATA stateData;
            if (!ReportMenuCtrl.clientManager.GetCurStateInfo(out stateData) || stateData == null)
            {
                MessageBox.Show("서버 정보를 불러오는데 실패하였습니다.\n설비 상태 확인 실패.");
                ClientManager.Log(string.Format("IGS - POP_EXCEPT_Window GetCurStateInfo Fail"));
                return;
            }

            //GET LOSS CODE
            string errMsg;
            if (!ReportMenuCtrl.clientManager.GetLossCodeList(stateData.USER_ID, out lossList, out errMsg))
            {
                MessageBox.Show(string.Format("유실 코드 정보를 불러오지 못했습니다.\nErrorCode: {0}", errMsg));
                ClientManager.Log(string.Format("IGS - POP_EXCEPT_Window GetLossCodeList Fail. Error: {0}", errMsg));
                return;
            }

            //TIME CHECK - Default코드일 경우, 1분컷으로 끊고 그 시간으로 비가동 보고, 그 외에는 현재시간으로 보고
            tbNextTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:00");
            if (stateData.EXC_START_TIME != "")
            {
                DateTime lastTime = DateTime.ParseExact(stateData.EXC_START_TIME, "yyyyMMddHHmmss", null);
                tbCurTime.Text = lastTime.ToString("yyyy-MM-dd HH:mm:ss");

                if (stateData.EXC_CD == "D99")
                    tbNextTime.Text = lastTime.AddMinutes(1).ToString("yyyy-MM-dd HH:mm:00");
            }
            else
                tbCurTime.Text = "";

            tbMC.Text = stateData.MACHINE_CODE;
            tbLot.Text = stateData.ORDER;
            if (tbLot.Text == "")
            {
                tbIssue.Text = "오더가 없는 상태에서는 \n특이사항을 입력할 수 없습니다.";
                tbIssue.IsEnabled = false;
            }

            if(stateData.EXC_CD == "")
            {
                tbCurDtl.Text = "가동중";
                tbCurDtl.Foreground = new SolidColorBrush(Colors.Tomato);
            }
            else
            {
                tbCurDtl.Text = string.Format("{0},{1}", stateData.EXC_CD, stateData.EXC_NM);
                tbCurDtl.Foreground = new SolidColorBrush(Colors.Black);
            }

            cbLossCode.Items.Clear();
            foreach (string loss in lossList)
                cbLossCode.Items.Add(loss);

            if (UserCtrl.curUser != null)
            {
                tboxOper.Text = UserCtrl.curUser.USERID;
                tbOperName.Text = UserCtrl.curUser.USER_NM;
            }
            else
            {
                tboxOper.Text = stateData.USER_ID;
                tbOperName.Text = stateData.USER_NAME;
            }

            tbLossCode.Focus();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (tbNextTime.Text == "")
            {
                MessageBox.Show("비가동 시작 시간을 입력바랍니다.");
                tbNextTime.Focus();
                return;
            }

            if (tbOperName.Text == "" || tboxOper.Text.Length != 6)
            {
                MessageBox.Show("담당자 정보가 올바르지 않습니다.");
                tboxOper.Focus();
                return;
            }

            if (!POP_Report())
                return;

            MessageBox.Show("정상적으로 보고가 완료되었습니다.");
            this.DialogResult = true;
        }

        private bool POP_Report()
        {
            try
            {
                DateTime repTime;
                try
                {
                    repTime = Convert.ToDateTime(tbNextTime.Text);
                }
                catch
                {
                    MessageBox.Show("비가동 시작 시간을 다시 확인 바랍니다.");
                    tbNextTime.Focus();
                    return false;
                }

                if (tbCurTime.Text != "" && Convert.ToDateTime(tbCurTime.Text) > repTime)
                {
                    MessageBox.Show("새로운 비가동 시작 시간은 이전 비가동 시간보다 빠를 수 없습니다.");
                    tbNextTime.Focus();
                    return false;
                }

                if (tboxOper.Text == "")
                {
                    MessageBox.Show("작업자 정보를 입력바랍니다.");
                    tboxOper.Focus();
                    return false;
                }

                string strIssue = "";

                if (tbLot.Text != "")
                    strIssue = tbIssue.Text.Replace("\\", "").Replace("\r\n", " ");

                if (strIssue != "")
                {
                    if (tbLot.Text == "")
                    {
                        MessageBox.Show("가동중인 오더가 없는 상태에서는 특이사항을 입력할 수 없습니다.");
                        return false;
                    }

                    int nByte = Encoding.Default.GetByteCount(strIssue);
                    if (nByte > 500)
                    {
                        MessageBox.Show("특이사항 내용이 너무 많습니다. 한글로 165자 이상 입력할 수 없습니다.");
                        return false;
                    }
                }

                if (MessageBox.Show(string.Format("입력된 정보로 비가동 보고를 진행하시겠습니까?"), "확인", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return false;

                string strDate = repTime.ToString("yyyyMMdd");
                string strTime = repTime.ToString("HHmm00");
                string code = cbLossCode.SelectedItem.ToString().Split(',')[0];
                
                JObject request = new JObject();
                request.Add("USER_ID", tboxOper.Text);
                request.Add("EQPT_CD", tbMC.Text);
                request.Add("WONO", tbLot.Text);
                request.Add("NEXT_DT", strDate);
                request.Add("NEXT_TM", strTime);
                request.Add("EXC_CD", code);
                request.Add("BREAK_DETAIL", tbBreakDetail.Text);

                string request_msg = JsonConvert.SerializeObject(request);
                string errMsg;

                if (!ReportMenuCtrl.clientManager.SetReportOneWay(ReportType.EXCEPT_REPORT, request_msg, out errMsg))
                {
                    MessageBox.Show(string.Format("보고에 실패하였습니다. 실패사유: {0}", errMsg));
                    ClientManager.Log(string.Format("IGS - POP_EXCEPT_Window.cs POP_Report Fail. Error:{0}", errMsg));
                    return false;
                }
                else
                    ClientManager.Log(string.Format("IGS - POP_EXCEPT_Window.cs POP_Report Success."));

                if (strIssue != "")
                {
                    request = new JObject();
                    request.Add("USER_ID", tboxOper.Text);
                    request.Add("ISSUE_DETAIL", strIssue);

                    request_msg = JsonConvert.SerializeObject(request);

                    if (!ReportMenuCtrl.clientManager.SetReportOneWay(ReportType.SET_REMK_MESSAGE, request_msg, out errMsg))
                    {
                        MessageBox.Show(string.Format("특이사항 입력에 실패하였습니다. 실패사유: {0}", errMsg));
                        ClientManager.Log(string.Format("IGS - POP_EXCEPT_Window.cs SET_REMK_MESSAGE Fail. Error:{0}", errMsg));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("IGS - POP_EXCEPT_Window.cs POP_Report Exception: {0}", ex.Message));
                MessageBox.Show(string.Format("보고 과정에 문제가 발생하였습니다.\nException: {0}", ex.Message));
                return false;
            }
        }

        private void BtnBreakTime_Click(object sender, RoutedEventArgs e)
        {
            SubWindow.PASSWORD_Window pass = new PASSWORD_Window();
            if ((bool)pass.ShowDialog())
            {
                SubWindow.BREAKTIME_Window dlg = new BREAKTIME_Window();
                dlg.ShowDialog();
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            SubWindow.CODE_SEARCH_Window dlg = new CODE_SEARCH_Window();
            dlg.InitializeDialog(lossList);
            if ((bool)dlg.ShowDialog())
            {
                cbLossCode.SelectedItem = dlg.selectedCode;
            }
        }

        private void TbLossCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbLossCode.Text == "") return;

            foreach (string loss in cbLossCode.Items)
            {
                if (loss.Contains(tbLossCode.Text.ToUpper()))
                    cbLossCode.SelectedItem = loss;
            }
        }

        private void TboxOper_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (tboxOper.Text.Length == 6)
            {
                POP_USER_DATA user;
                string errMsg;

                if (ReportMenuCtrl.clientManager.GetUserInfo(tboxOper.Text, out user, out errMsg))
                    tbOperName.Text = user.USER_NM;
                else
                    tbOperName.Text = "";
            }
            else
                tbOperName.Text = "";
        }

        private void Tbox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void CbLossCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbLossCode.SelectedIndex == -1)
            {
                tbNextDtl.Text = "";
                btnOK.IsEnabled = false;

                tbBreakDetail.Text = "";
                tbBreakDetail.IsEnabled = false;

                return;
            }

            if (cbLossCode.SelectedItem.ToString().StartsWith("D11"))
                tbBreakDetail.IsEnabled = true;
            else
            {
                tbBreakDetail.Text = "";
                tbBreakDetail.IsEnabled = false;
            }

            tbNextDtl.Text = cbLossCode.SelectedItem.ToString();
            btnOK.IsEnabled = true;
        }

        private void BdTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
