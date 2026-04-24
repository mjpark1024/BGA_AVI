using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IGS.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IGS.SubWindow
{
    /// <summary>
    /// POP_SUSPEND_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class POP_SUSPEND_Window : Window
    {        
        public bool bInitialize = false;
        private List<BadCountData> badList = new List<BadCountData>();
        private List<string> codeList = new List<string>();

        //nInput: 투입수량, nGood: 완성수량, nNG: 불량수량
        public POP_SUSPEND_Window()
        {
            InitializeComponent();
            InitializeEvent();
            InitializeDialog();
        }

        private void InitializeEvent()
        {
            this.cbStopCode.SelectionChanged += CbStopCode_SelectionChanged;

            this.bdTitle.PreviewMouseLeftButtonDown += BdTitle_PreviewMouseLeftButtonDown;

            this.btnOK.Click += BtnOK_Click;
            this.btnCancel.Click += BtnCancel_Click;
            this.btnAddNG.Click += BtnAddNG_Click;
            this.btnSearch.Click += BtnSearch_Click;

            this.tboxUserID.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;
            this.tbHour.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;
            this.tbMinute.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;
            this.tboxStopCode.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;
            this.tbInput.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;

            this.tboxUserID.PreviewKeyUp += TboxUserID_PreviewKeyUp;
            this.tboxStopCode.PreviewKeyUp += TboxStopCode_PreviewKeyUp;
            this.tbInput.TextChanged += TbInput_TextChanged;
            this.tbInput.LostFocus += TbInput_LostFocus;
        }

        private void InitializeDialog()
        {
            //GET STATE INFO
            STATE_TABLE_DATA stateData;
            if(!ReportMenuCtrl.clientManager.GetCurStateInfo(out stateData) || stateData == null)
            {
                MessageBox.Show("서버 정보를 불러오는데 실패하였습니다.");
                ClientManager.Log(string.Format("IGS - POP_SUSPEND_Window GetCurStateInfo Fail"));
                return;
            }

            if (stateData.ORDER == "")
            {
                ReportMenuCtrl.clientManager.UpdateState();
                MessageBox.Show("오더정보가 없는 비가동 상태입니다.\n설비에서 중단 보고를 할 수 없으니,\n기존 POP 프로그램에서 중단보고 바랍니다.");
                ClientManager.Log(string.Format("IGS - POP_SUSPEND_Window.cs 오더정보가 없는 비가동 상태입니다."));
                //this.DialogResult = false;
                this.Close();
                return;
            }

            //GET STOP REASON CODE
            cbStopCode.Items.Clear();
            string errMsg;
            if (ReportMenuCtrl.clientManager.GetStopCodeList(stateData.USER_ID, out codeList, out errMsg) && codeList != null)
            {
                foreach (string code in codeList)
                    cbStopCode.Items.Add(code);
            }
            else
            {
                MessageBox.Show(string.Format("중단사유 코드 정보를 불러오지 못했습니다.\nErrorCode: {0}", errMsg));
                ClientManager.Log(string.Format("IGS - POP_SUSPEND_Window GetLossCodeList Fail. Error: {0}", errMsg));
                return;
            }

            tbMachineName.Text = stateData.MACHINE_CODE;            
            tbLot.Text = stateData.ORDER;
            tbOPCode.Text = stateData.OP_CODE;

            if (UserCtrl.curUser != null)
            {
                tboxUserID.Text = UserCtrl.curUser.USERID;
                tbUserNM.Text = UserCtrl.curUser.USER_NM;
            }
            else
            {
                tboxUserID.Text = stateData.USER_ID;
                tbUserNM.Text = stateData.USER_NAME;
            }

            dpDate.SelectedDate = DateTime.Now;
            tbHour.Text = DateTime.Now.Hour.ToString("D2");
            tbMinute.Text = DateTime.Now.Minute.ToString("D2");

            tbInput.Text = "1";
            tbOutput.Text = "1";
            tbNGCount.Text = "0";

            //READ LOSS FILE
            //LF의 경우 MES 중단 이력을 확인해봤을 때, 중단보고 시 불량 수량을 입력하지 않는 것으로 보임
            //추후 LF에 대한 Loss 연동 기능 추가 시, 불량항목에 대한 책임공정 수정 기능이 필요한지 검토해야함.
            //BGA의 경우 책임공정을 VI91로 입력하지만, LF의 경우 불량별 책임공정을 별도로 지정
            //if (ReportMenuCtrl.clientManager.strSite == "BGA" && !string.IsNullOrEmpty(ReportMenuCtrl.lossPath))
            //{
            //    string[] files = Directory.GetFiles(ReportMenuCtrl.lossPath, string.Format("{0}_{1}_*.txt", stateData.ORDER, stateData.OP_CODE));
            //    if (files.Count() == 1)
            //    {
            //        //GET OP CODE LIST
            //        List<string> opList;
            //        ReportMenuCtrl.clientManager.GetOPCodeList(stateData.ORDER, out opList, out errMsg);

            //        string inspCode = "";
            //        foreach(string op in opList)
            //        {
            //            if (ReportMenuCtrl.clientManager.CheckInspectCode(op))
            //                inspCode = op;
            //        }

            //        int nInputCount = 0;
            //        int nTotalBad = 0;
            //        string[] lines = File.ReadAllLines(files[0]);
            //        for (int i = 0; i < lines.Length; i++)
            //        {
            //            if (ClientManager.productType == ProductType.STRIP)
            //            {
            //                if (lines[i].Contains("1Q,X"))
            //                    break;

            //                if (lines[i].Contains("1Q,O"))
            //                {
            //                    string[] spt = lines[i + 1].Split(',');
            //                    nInputCount = Convert.ToInt32(spt[0]);
            //                }

            //                if (Regex.IsMatch(lines[i], @"^[a-zA-Z]"))
            //                {
            //                    try
            //                    {
            //                        string[] spt = lines[i].Split(',');
            //                        if (spt.Length != 2)
            //                            continue;

            //                        //B900 불량은 집계하지 않는다(22.07.11). 추후 집계 필요시 아래의 코드 수정 필요.
            //                        if (spt[0] == "B900")
            //                            continue;

            //                        if (!ClientManager.badcodeList.Keys.Contains(spt[0]))
            //                            continue;

            //                        int nBadCount = Convert.ToInt32(spt[1]);
            //                        if (nBadCount == 0)
            //                            continue;

            //                        nTotalBad += nBadCount;

            //                        if (badList.Any(b => b.bad_code == spt[0]))
            //                            badList.First(b => b.bad_code == spt[0]).ngCount += nBadCount;
            //                        else
            //                        {
            //                            BadCountData cnt = new BadCountData();

            //                            cnt.bad_code = spt[0];
            //                            cnt.bad_name = ClientManager.badcodeList[spt[0]];
            //                            cnt.codeList = opList == null ? null : opList.ConvertAll(s => s);
            //                            cnt.op_code = ReportMenuCtrl.clientManager.GetDefaultOPCode(opList, spt[0], inspCode);
            //                            cnt.ngCount = nBadCount;

            //                            badList.Add(cnt);
            //                        }
            //                    }
            //                    catch { }
            //                }
            //            }
            //        }

            //        //tbInput.Text = string.Format("{0:#,0}", nInputCount);
            //        //tbNGCount.Text = string.Format("{0:#,0}", nTotalBad);
            //        //tbOutput.Text = string.Format("{0:#,0}", nInputCount - nTotalBad);

            //        tbInput.Text = string.Format("{0:#,0}", 0);
            //        tbNGCount.Text = string.Format("{0:#,0}", 0);
            //        tbOutput.Text = string.Format("{0:#,0}", 0);
            //    }
            //}

            tbInput.Text = string.Format("{0:#,0}", 0);
            tbNGCount.Text = string.Format("{0:#,0}", 0);
            tbOutput.Text = string.Format("{0:#,0}", 0);
            btnOK.IsEnabled = false;
            bInitialize = true;
        }

        private void CbStopCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbStopCode.SelectedIndex == -1)
                btnOK.IsEnabled = false;
            else
                btnOK.IsEnabled = true;
        }

        private void TbInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int nInput = string.IsNullOrEmpty(tbInput.Text) ? 0 : Convert.ToInt32(tbInput.Text.Replace(",", ""));
                int nNG = string.IsNullOrEmpty(tbNGCount.Text) ? 0 : Convert.ToInt32(tbNGCount.Text.Replace(",", ""));

                tbOutput.Text = string.Format("{0:#,0}", nInput - nNG);
            }
            catch { }
        }

        private void TbInput_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int nInput = string.IsNullOrEmpty(tbInput.Text) ? 0 : Convert.ToInt32(tbInput.Text.Replace(",", ""));
                tbInput.Text = string.Format("{0:#,0}", nInput);
            }
            catch { }
        }

        private void TboxUserID_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (tboxUserID.Text.Length == 6)
            {
                POP_USER_DATA user;
                string errMsg;
                if (ReportMenuCtrl.clientManager.GetUserInfo(tboxUserID.Text, out user, out errMsg))
                    tbUserNM.Text = user.USER_NM;
                else
                    tbUserNM.Text = "";
            }
            else
                tbUserNM.Text = "";
        }

        private void TboxStopCode_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (tboxStopCode.Text == "") return;

            foreach (string code in cbStopCode.Items)
            {
                if (code.Contains(tboxStopCode.Text.ToUpper()))
                    cbStopCode.SelectedItem = code;
            }
        }

        private void TbNGCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int nInput = Convert.ToInt32(tbInput.Text.Replace(",", ""));
                int nNG = Convert.ToInt32(tbNGCount.Text.Replace(",", ""));

                int nOutput = nInput - nNG;
                tbOutput.Text = string.Format("{0:#,0}", nOutput);
            }
            catch { }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!POP_Report())
                return;

            MessageBox.Show("정상적으로 보고가 완료되었습니다.");
            this.DialogResult = true;
        }

        private bool POP_Report()
        {
            try
            {
                if (tboxUserID.Text == "" || tbUserNM.Text == "")
                {
                    MessageBox.Show("담당자 정보를 다시 확인바랍니다.");
                    return false;
                }

                if (dpDate.SelectedDate == null || tbHour.Text == "" || tbMinute.Text == "")
                {
                    MessageBox.Show("중단 시간을 정확히 입력바랍니다.");
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

                int nInput = 0;
                int nOutput = 0;
                int nNG = 0;
                try
                {
                    nInput = Convert.ToInt32(tbInput.Text.Replace(",", ""));
                    nOutput = Convert.ToInt32(tbOutput.Text.Replace(",", ""));
                    nNG = Convert.ToInt32(tbNGCount.Text.Replace(",", ""));

                    if (nInput != nOutput + nNG)
                    {
                        MessageBox.Show("투입수량이 완성수량과 불량수량의 합계와 맞지 않습니다.");
                        return false;
                    }

                    if (nInput < 0 || nOutput < 0 || nNG < 0)
                    {
                        MessageBox.Show("수량은 음수가 될 수 없습니다.");
                        return false;
                    }
                }
                catch
                {
                    MessageBox.Show("중단 수량은 숫자만 입력할 수 있습니다.\n다시 입력바랍니다.");
                    return false;
                }

                if (tbLot.Text == "" || tbOPCode.Text == "")
                {
                    MessageBox.Show("오더번호나 공정코드가 확인되지 않으면 보고를 진행할 수 없습니다.");
                    return false;
                }

                int nChangeMode;
                SubWindow.SUSPEND_CONFIRM_Window sDlg = new SUSPEND_CONFIRM_Window();
                if ((bool)sDlg.ShowDialog())
                    nChangeMode = sDlg.nMode;
                else
                    return false;

                string strStopCode = cbStopCode.SelectedItem.ToString().Split(',')[0];

                JObject request = new JObject();
                request.Add("USER_ID", tboxUserID.Text);
                request.Add("EQPT_CD", tbMachineName.Text);
                request.Add("WONO", tbLot.Text);
                request.Add("STOP_DT", strDate);
                request.Add("STOP_TM", strTime);
                request.Add("STOP_CD", strStopCode);
                request.Add("STOP_DTL", tbDetail.Text.Replace("\\", "").Replace("\r\n", " "));
                request.Add("CHANGE_YN", nChangeMode == 0 ? "N" : "Y");
                request.Add("C_WONO", tbLot.Text);
                request.Add("IN_QTY", nInput);
                request.Add("OUT_QTY", nOutput);
                request.Add("SCR_QTY", nNG);

                JArray badCount = new JArray();
                foreach (BadCountData cnt in badList)
                {
                    JObject badInfo = new JObject();
                    badInfo.Add("SCR_CD", cnt.bad_code);
                    badInfo.Add("SCR_QTY", cnt.ngCount);
                    badInfo.Add("SCR_OPCD", cnt.op_code);

                    badCount.Add(badInfo);
                }
                request.Add("BAD_CNT", badCount);

                string request_msg = JsonConvert.SerializeObject(request);
                string errMsg;

                if (!ReportMenuCtrl.clientManager.SetReportOneWay(ReportType.SUSPEND_REPORT, request_msg, out errMsg))
                {
                    MessageBox.Show(string.Format("보고에 실패하였습니다.{0}", errMsg == "" ? "" : "실패사유: " + errMsg));
                    ClientManager.Log(string.Format("IGS - POP_SUSPEND_Window.cs POP_Report Fail. Error:{0}", errMsg));
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("IGS - POP_SUSPEND_Window.cs POP_Report Exception: {0}", ex.Message));
                MessageBox.Show(string.Format("보고 과정에 문제가 발생하였습니다.\nException: {0}", ex.Message));
                return false;
            }
        }

        private void BtnAddNG_Click(object sender, RoutedEventArgs e)
        {
            STATE_TABLE_DATA data;
            if (!ReportMenuCtrl.clientManager.GetCurStateInfo(out data))
            {
                MessageBox.Show("서버에서 필요한 정보를 가져오지 못했습니다.");
                ClientManager.Log(string.Format("IGS - POP_SUSPEND_Window GetCurStateInfo Fail."));
                return;
            }

            SubWindow.NGCount_Window dlg = new NGCount_Window(ref data, ref badList);
            if ((bool)dlg.ShowDialog())
            {
                try
                {
                    int nTotal = 0;
                    badList.Clear();
                    foreach (BadCountData cnt in dlg.Counts)
                    {
                        nTotal += cnt.ngCount;
                        badList.Add(cnt);
                    }
                    tbNGCount.Text = string.Format("{0:#,0}", nTotal);
                    try
                    {
                        int nInput = Convert.ToInt32(tbInput.Text.Replace(",", ""));
                        if (nInput < nTotal)
                        {
                            MessageBox.Show("투입 수량보다 불량 수량이 많을 수 없습니다.");
                            tbInput.Text = string.Format("{0:#,0}", nTotal);
                        }
                        else
                            tbOutput.Text = string.Format("{0:#,0}", nInput - nTotal);
                    }
                    catch { }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(string.Format("불량 정보를 집계하는데 문제가 발생하였습니다.\nException: {0}", ex.Message));
                    ClientManager.Log(string.Format("IGS - POP_SUSPEND_Window 불량 정보를 집계하는데 문제가 발생하였습니다. Exception: {0}", ex.Message));
                }
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            SubWindow.CODE_SEARCH_Window dlg = new CODE_SEARCH_Window();
            dlg.InitializeDialog(codeList);
            if ((bool)dlg.ShowDialog())
            {
                cbStopCode.SelectedItem = dlg.selectedCode;
            }
        }

        private void Tbox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ReportMenuCtrl.clientManager.strSite == "LF")
                (sender as TextBox).SelectAll();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BdTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

    }
}
