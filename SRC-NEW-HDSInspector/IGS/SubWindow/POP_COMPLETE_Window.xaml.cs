using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// POP_COMPLETE_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class POP_COMPLETE_Window : Window
    {
        #region Member Variables.
        public ObservableCollection<BadCountData> Counts
        {
            get { return m_counts; }
        }
        private ObservableCollection<BadCountData> m_counts = new ObservableCollection<BadCountData>();

        private POP_COMPLETE_WINDOW_PARA curPara;
        private List<string> opList = new List<string>();
        private List<string> codeList = new List<string>();

        private DateTime lastTime;

        public bool bInitialize = false;
        #endregion

        public POP_COMPLETE_Window(DateTime lastWorkTime)
        {
            InitializeComponent();
            WindowPosition();

            lastTime = lastWorkTime;
            InitializeEvent();
            InitailizeDialog();
        }

        private void WindowPosition()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2) - 80;
        }

        private void InitializeEvent()
        {
            this.btnModify.Click += BtnModify_Click;
            this.btnSave.Click += BtnSave_Click;
            this.btnAdd.Click += BtnAdd_Click;
            this.btnCancel.Click += BtnCancel_Click;
            this.btnOK.Click += BtnOK_Click;
            this.btnSearch.Click += BtnSearch_Click;

            this.tboxCount.TextChanged += TboxCount_TextChanged;
            this.tbMCOutput.TextChanged += TbMCOutput_TextChanged;
            this.tbMCOutput.LostFocus += TbMCOutput_LostFocus;
            this.tboxCount.PreviewKeyDown += TboxCount_PreviewKeyDown;
            this.tboxUserID.PreviewKeyUp += TboxUserID_PreviewKeyUp;
            this.tboxBadCode.KeyUp += TboxBadCode_KeyUp;

            this.tboxUserID.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;
            this.tbHour.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;
            this.tbMinute.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;
            this.tbInput.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;

            this.lbInfo.PreviewMouseWheel += LbInfo_PreviewMouseWheel;
            this.bdTitle.PreviewMouseLeftButtonDown += BdTitle_PreviewMouseLeftButtonDown;
        }

        private void InitailizeDialog()
        {
            grdNGInfoHeader.IsEnabled = false;
            lbInfo.IsEnabled = false;
            btnSave.IsEnabled = false;

            //GET STATE INFO
            STATE_TABLE_DATA stateData;
            if (!ReportMenuCtrl.clientManager.GetCurStateInfo(out stateData) || stateData == null)
            {
                MessageBox.Show("서버 정보를 불러오는데 실패하였습니다.\n설비 상태 확인 실패.");
                ClientManager.Log(string.Format("IGS - POP_COMPLETE_Window.cs GetCurStateInfo Fail"));
                return;
            }

            if (stateData.ORDER == "")
            {
                MessageBox.Show("오더정보가 없는 비가동 상태입니다.\n설비에서 완료 보고를 할 수 없으니,\n기존 POP 프로그램에서 완료보고 바랍니다.");
                ClientManager.Log(string.Format("IGS - POP_COMPLETE_Window.cs 오더정보가 없는 비가동 상태입니다. 완료보고 불가"));
                //this.DialogResult = false;
                this.Close();
                return;
            }

            //READ LOSS FILE
            curPara = new POP_COMPLETE_WINDOW_PARA();
            curPara.nInput = 1;
            curPara.nOutput = 1;

            if (ReportMenuCtrl.lossPath == "")
            {
                if (MessageBox.Show("LOSS 경로가 등록되지 않았습니다.\n수동으로 입력하시겠습니까?", "확인", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    this.Close();
                    return;
                }
            }
            else
            {
                string lossPath = ReportMenuCtrl.lossPath;
                string[] files = Directory.GetFiles(lossPath, string.Format("{0}_{1}_*.txt", stateData.ORDER, stateData.OP_CODE));
                if (files.Count() == 0)
                {
                    if (MessageBox.Show(string.Format("{0} Lot에 대한 Loss파일을 찾을 수 없습니다.\n수동으로 입력하시겠습니까?", stateData.ORDER),
                        "확인", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        //this.DialogResult = false;
                        bInitialize = false;
                        this.Close();
                        return;
                    }
                }
                else if (files.Count() > 1)
                {
                    MessageBox.Show(string.Format("{0} Lot에 대한 Loss파일이 여러개입니다.\n수동으로 입력바랍니다.", stateData.ORDER));
                }
                else
                {
                    string[] lines = File.ReadAllLines(files[0]);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        ////Strip 제품 기준 Loss파일로 작성됨.
                        if (ClientManager.productType == ProductType.STRIP)
                        {
                            if (lines[i].Contains("1Q,O"))
                            {
                                string[] spt = lines[i + 1].Split(',');
                                curPara.nInput = Convert.ToInt32(spt[0]);
                                curPara.nOutput = Convert.ToInt32(spt[1]);
                            }

                            if (Regex.IsMatch(lines[i], @"^[a-zA-Z]"))
                            {
                                try
                                {
                                    string[] spt = lines[i].Split(',');
                                    if (spt.Length != 2)
                                        continue;

                                    //B900 불량은 집계하지 않는다(22.07.11). 추후 집계 필요시 아래의 코드 수정 필요.
                                    if (spt[0] == "B900")
                                        continue;

                                    if (!ClientManager.badcodeList.Keys.Contains(spt[0]))
                                        continue;

                                    int nBadCount = Convert.ToInt32(spt[1]);
                                    if (nBadCount == 0)
                                        continue;

                                    if (curPara.badList.ContainsKey(spt[0]))
                                        curPara.badList[spt[0]] += nBadCount;
                                    else
                                        curPara.badList.Add(spt[0], nBadCount);
                                }
                                catch { }
                            }
                        }
                        else if (ClientManager.productType == ProductType.SHOT)
                        {
                            ////AOI와 같은 SHOT 공정은 Loss파일 읽는 부분 재작성 필요.

                        }
                    }
                }
            }

            //GET BAD CODE
            cbBadCode.Items.Clear();
            foreach (string key in ClientManager.badcodeList.Keys)
            {
                string strCode = string.Format("{0}, {1}", key, ClientManager.badcodeList[key]);

                cbBadCode.Items.Add(strCode);
                codeList.Add(strCode);
            }

            cbBadCode.SelectedIndex = 0;

            //GET OP CODE LIST
            string errMsg;
            if (ReportMenuCtrl.clientManager.GetOPCodeList(stateData.ORDER, out opList, out errMsg) && opList == null)
            {
                MessageBox.Show("서버 정보를 불러오는데 실패하였습니다.\n책임 공정 리스트 획득 실패.");
                ClientManager.Log(string.Format("IGS - POP_COMPLETE_Window.cs GetOPCodeList Fail. Error:{0}", errMsg));
                return;
            }

            string inspCode = "";
            foreach (string op in opList)
            {
                cbOPCode.Items.Add(op);

                if (ReportMenuCtrl.clientManager.CheckInspectCode(op))
                    inspCode = op;
            }

            if (inspCode == "")
                cbOPCode.SelectedIndex = opList.Count - 1;
            else
                cbOPCode.SelectedItem = inspCode;

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

            dpDate.SelectedDate = lastTime;
            tbHour.Text = lastTime.Hour.ToString("D2");
            tbMinute.Text = lastTime.Minute.ToString("D2");

            //SET BAD COUNT LIST
            m_counts.Clear();
            foreach (string key in curPara.badList.Keys)
            {
                BadCountData cnt = new BadCountData();

                cnt.bad_code = key;
                cnt.bad_name = ClientManager.badcodeList[key];
                cnt.codeList = opList.ConvertAll(s => s);                                               //Deep Copy
                cnt.op_code = ReportMenuCtrl.clientManager.GetDefaultOPCode(opList, key, inspCode);     //opList[opList.Count - 1];
                cnt.ngCount = curPara.badList[key];

                m_counts.Add(cnt);
            }

            //전공정 완성수량 확인
            POP_LOT_DATA lotData = new POP_LOT_DATA();
            if (ReportMenuCtrl.clientManager.GetMESLotInfo(stateData.ORDER, out lotData))
            {
                if (lotData.PRE_OUT_CNT != "")
                {
                    curPara.nInput = Convert.ToInt32(lotData.PRE_OUT_CNT);
                }
                else
                {
                    tbInput.IsReadOnly = false;
                    curPara.nInput = 0;
                }
            }

            tbInput.Text = string.Format("{0:#,0}", curPara.nInput);
            tbMCOutput.Text = string.Format("{0:#,0}", curPara.nOutput);

            CalcYield();
            lbInfo.DataContext = Counts;

            dpDate.SelectedDate = DateTime.Now;
            tbHour.Text = DateTime.Now.Hour.ToString("D2");
            tbMinute.Text = DateTime.Now.Minute.ToString("D2");

            bInitialize = true;
        }

        private bool CalcYield()
        {
            try
            {
                int nTotal = 0;
                try
                {
                    nTotal = Convert.ToInt32(tbInput.Text.Replace(",", ""));
                    if (nTotal < 1)
                    {
                        MessageBox.Show("투입수량은 0보다 커야합니다.");
                        return false;
                    }
                }
                catch
                {
                    MessageBox.Show("수량은 숫자만 입력가능합니다.");
                    tbInput.Text = "";
                    return false;
                }

                int nNGCount = 0;
                foreach (BadCountData cnt in m_counts)
                    nNGCount += cnt.ngCount;

                if (nNGCount > nTotal)
                {
                    MessageBox.Show("불량수량이 투입수량보다 많을 수 없습니다.");
                    return false;
                }

                double dYield = (double)(nTotal - nNGCount) / (double)nTotal * 100.0;

                tbYield.Text = dYield.ToString("F2");
                tbNGCount.Text = string.Format("{0:#,0}", nNGCount);
                tbOutput.Text = string.Format("{0:#,0}", nTotal - nNGCount);

                //수량 차이 계산
                try
                {
                    int nOut = Convert.ToInt32(tbOutput.Text.Replace(",", ""));
                    int nMC = Convert.ToInt32(tbMCOutput.Text.Replace(",", ""));

                    tbCntDiff.Text = string.Format("{0:#,0}", nOut - nMC);
                }
                catch
                {
                    MessageBox.Show("완성 수량은 숫자만 입력 가능합니다.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("IGS - POP_COMPLETE_Window.cs CalcYield Exception: {0}", ex.Message));
                return false;
            }
        }

        private void BtnModify_Click(object sender, RoutedEventArgs e)
        {
            grdNGInfoHeader.IsEnabled = true;
            lbInfo.IsEnabled = true;
            btnOK.IsEnabled = false;

            btnModify.IsEnabled = false;
            btnSave.IsEnabled = true;
            tboxBadCode.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CalcYield())
            {
                grdNGInfoHeader.IsEnabled = false;
                lbInfo.IsEnabled = false;
                btnOK.IsEnabled = true;

                btnModify.IsEnabled = true;
                btnSave.IsEnabled = false;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int nCount = 0;
                try
                {
                    if (tboxCount.Text == "")
                        nCount = 0;
                    else
                        nCount = Convert.ToInt32(tboxCount.Text);
                }
                catch
                {
                    MessageBox.Show("수량 입력값이 올바르지 않습니다.");
                    return;
                }

                if (cbBadCode.SelectedItem == null)
                {
                    MessageBox.Show("불량 코드를 선택해주시기 바랍니다.");
                    return;
                }

                if (cbOPCode.SelectedItem == null)
                {
                    MessageBox.Show("책임공정을 선택해주시기 바랍니다.");
                    return;
                }

                string strCode = cbBadCode.SelectedItem.ToString().Substring(0, 4);
                string strOP = cbOPCode.SelectedItem.ToString();

                if (m_counts.Where(m => m.bad_code == strCode && m.op_code == strOP).Count() != 0)
                {
                    MessageBox.Show("이미 추가된 불량 코드입니다.");
                    return;
                }

                BadCountData data = new BadCountData();
                data.bad_code = strCode;
                data.bad_name = ClientManager.badcodeList[strCode];
                data.codeList = opList.ConvertAll(s => s);
                data.op_code = strOP;
                data.ngCount = nCount;

                m_counts.Add(data);

                tboxBadCode.Text = "";
                tboxBadCode.Focus();
            }
            catch { }
            finally
            {
                tboxBadCode.Focus();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!CalcYield())
                return;

            if (!POP_Report())
                return;

            this.DialogResult = true;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            SubWindow.CODE_SEARCH_Window dlg = new CODE_SEARCH_Window();
            dlg.InitializeDialog(codeList);
            if ((bool)dlg.ShowDialog())
                cbBadCode.SelectedItem = dlg.selectedCode;
        }

        private bool POP_Report()
        {
            try
            {
                if (tboxUserID.Text == "" || tboxUserID.Text.Length != 6 || tbUserNM.Text == "")
                {
                    MessageBox.Show("담당자 정보를 다시 확인바랍니다.");
                    return false;
                }

                if (dpDate.SelectedDate == null || tbHour.Text == "" || tbMinute.Text == "")
                {
                    MessageBox.Show("완료 시간을 정확히 입력바랍니다.");
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
                    MessageBox.Show("수량은 숫자만 입력할 수 있습니다.\n다시 입력바랍니다.");
                    return false;
                }

                if (tbLot.Text == "" || tbOPCode.Text == "")
                {
                    MessageBox.Show("오더번호나 공정코드가 확인되지 않으면 보고를 진행할 수 없습니다.");
                    return false;
                }

                if (!ReportMenuCtrl.clientManager.CheckInspectCode(tbOPCode.Text))
                {
                    MessageBox.Show("공정코드가 검사공정에 있지 않습니다.");
                    return false;
                }

                string strIssue = tbIssue.Text.Replace("\\", "").Replace("\r\n", " ");
                if (strIssue != "")
                {
                    int nByte = Encoding.Default.GetByteCount(strIssue);
                    if (nByte > 500)
                    {
                        MessageBox.Show("특이사항 내용이 너무 많습니다. 한글로 165자 이상 입력할 수 없습니다.");
                        return false;
                    }
                }

                int nRetestMode;
                if (ReportMenuCtrl.clientManager.strSite == "LF")
                {
                    SubWindow.COMPLETE_CONFIRM_Window cDlg = new COMPLETE_CONFIRM_Window();
                    if ((bool)cDlg.ShowDialog())
                        nRetestMode = cDlg.nMode;
                    else
                        return false;
                }
                else
                    nRetestMode = 1;

                JObject request = new JObject();
                request.Add("USER_ID", tboxUserID.Text);
                request.Add("EQPT_CD", tbMachineName.Text);
                request.Add("WONO", tbLot.Text);
                request.Add("END_DT", strDate);
                request.Add("END_TM", strTime);
                request.Add("C_WONO", tbLot.Text);
                request.Add("YIELD", tbYield.Text);
                request.Add("SPEED", 17.0);
                request.Add("RETEST_YN", nRetestMode == 0 ? "Y" : "N");
                request.Add("IN_QTY", nInput);
                request.Add("OUT_QTY", nOutput);
                request.Add("SCR_QTY", nNG);

                JArray badCount = new JArray();
                foreach (BadCountData cnt in m_counts)
                {
                    JObject badInfo = new JObject();
                    badInfo.Add("SCR_CD", cnt.bad_code);
                    badInfo.Add("SCR_QTY", cnt.ngCount);
                    badInfo.Add("SCR_OPCD", cnt.op_code);

                    badCount.Add(badInfo);
                }
                request.Add("BAD_CNT", badCount);

                string request_msg = JsonConvert.SerializeObject(request);
                string errMsg, downMsg;

                if (!ReportMenuCtrl.clientManager.SetReportTwoWay(ReportType.COMPLETE_REPORT, request_msg, out errMsg, out downMsg))
                {
                    MessageBox.Show(string.Format("보고에 실패하였습니다. 실패사유: {0}", errMsg));
                    ClientManager.Log(string.Format("IGS - POP_COMPLETE_Window.cs POP_Report Fail. Error:{0}", errMsg));
                    return false;
                }
                else
                {
                    ClientManager.Log(string.Format("IGS - POP_COMPLETE_Window.cs POP_Report Success."));

                    if (ReportMenuCtrl.clientManager.strSite == "BGA")
                    {
                        //AI 분류 결과 전송
                        JObject aiRequest;
                        if (GetAIClassification(out aiRequest) && aiRequest != null)
                        {
                            request_msg = JsonConvert.SerializeObject(aiRequest);
                            if (!ReportMenuCtrl.clientManager.SetReportOneWay(ReportType.SET_AI_CLASSIFICATION, request_msg, out errMsg))
                                ClientManager.Log(string.Format("IGS - POP_COMPLETE_Window.cs SET_AI_CLASSIFICATION Fail. Error:{0}", errMsg));
                        }
                    }
                }

                if (strIssue != "")
                {
                    request = new JObject();
                    request.Add("USER_ID", tboxUserID.Text);
                    request.Add("ISSUE_DETAIL", strIssue);

                    request_msg = JsonConvert.SerializeObject(request);

                    if (!ReportMenuCtrl.clientManager.SetReportOneWay(ReportType.SET_REMK_MESSAGE, request_msg, out errMsg))
                    {
                        MessageBox.Show(string.Format("특이사항 입력에 실패하였습니다. 실패사유: {0}", errMsg));
                        ClientManager.Log(string.Format("IGS - POP_COMPLETE_Window.cs SET_REMK_MESSAGE Fail. Error:{0}", errMsg));
                    }
                }

                if (!string.IsNullOrEmpty(downMsg))
                {
                    ClientManager.Log(string.Format("IGS - POP_COMPLETE_Window.cs Warning Msg,\n{0}", downMsg));
                    downMsg = downMsg.Replace("^>", "\n");
                    MessageBox.Show(string.Format("정상적으로 보고가 완료되었습니다.\n특이사항 : {0}", downMsg));
                }
                else
                {
                    MessageBox.Show("정상적으로 보고가 완료되었습니다.");
                }

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("IGS - POP_COMPLETE_Window.cs POP_Report Exception: {0}", ex.Message));
                MessageBox.Show(string.Format("보고 과정에 문제가 발생하였습니다.\nException: {0}", ex.Message));
                return false;
            }
        }

        private bool GetAIClassification(out JObject request)
        {
            request = new JObject();

            try
            {
                string[] files = Directory.GetFiles(ReportMenuCtrl.lossPath, string.Format("A_{0}_{1}_*.txt", tbLot.Text, tbOPCode.Text));
                if (files.Length == 0)
                {
                    ClientManager.Log(string.Format("IGS - POP_COMPLETE_Window.cs A_{0}_{1} 파일이 없습니다.", tbLot.Text, tbOPCode.Text));
                    return false;
                }

                Dictionary<string, int> unitCnt = new Dictionary<string, int>();
                Dictionary<string, int> imgCnt = new Dictionary<string, int>();

                string badType = "U";

                string[] lines = File.ReadAllLines(files[0]);
                foreach (string line in lines)
                {
                    if (line.Contains("U,"))
                        badType = "U";
                    else if (line.Contains("I,"))
                        badType = "I";
                    else
                    {
                        if (Regex.IsMatch(line, @"^[a-zA-Z]"))
                        {
                            try
                            {
                                string[] spt = line.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if (spt.Length != 2)
                                    continue;

                                //B900 불량은 집계하지 않는다(22.07.11). 추후 집계 필요시 아래의 코드 수정 필요.
                                if (spt[0] == "B900")
                                    continue;

                                if (!ClientManager.badcodeList.Keys.Contains(spt[0]))
                                    continue;

                                if (badType == "U")
                                {
                                    if (unitCnt.ContainsKey(spt[0]))
                                        unitCnt[spt[0]] += Convert.ToInt32(spt[1]);
                                    else
                                        unitCnt.Add(spt[0], Convert.ToInt32(spt[1]));
                                }
                                else
                                {
                                    if (imgCnt.ContainsKey(spt[0]))
                                        imgCnt[spt[0]] += Convert.ToInt32(spt[1]);
                                    else
                                        imgCnt.Add(spt[0], Convert.ToInt32(spt[1]));
                                }
                            }
                            catch { }
                        }
                    }
                }

                request.Add("WONO", tbLot.Text);
                request.Add("OP_CD", tbOPCode.Text);
                request.Add("EQPT_CD", tbMachineName.Text);

                JArray badCount = new JArray();
                if (unitCnt.Count > 0)
                {
                    foreach (KeyValuePair<string, int> pair in unitCnt)
                    {
                        JObject badInfo = new JObject();
                        badInfo.Add("SCR_CD", pair.Key);
                        badInfo.Add("SCR_DIV", "U");
                        badInfo.Add("SCR_QTY", pair.Value);

                        badCount.Add(badInfo);
                    }
                }

                if (imgCnt.Count > 0)
                {
                    foreach (KeyValuePair<string, int> pair in imgCnt)
                    {
                        JObject badInfo = new JObject();
                        badInfo.Add("SCR_CD", pair.Key);
                        badInfo.Add("SCR_DIV", "I");
                        badInfo.Add("SCR_QTY", pair.Value);

                        badCount.Add(badInfo);
                    }
                }

                if (badCount.Count == 0)
                {
                    ClientManager.Log(string.Format("IGS - POP_COMPLETE_Window.cs 불량 정보가 없습니다"));
                    return false;
                }

                request.Add("BAD_CNT", badCount);
                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("IGS - POP_COMPLETE_Window.cs GetAIClassification Exception: {0}", ex.Message));
                return false;
            }
        }

        private void TboxCount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BtnAdd_Click(null, null);
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

        private void TboxBadCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (tboxBadCode.Text == "") return;

            if (e.Key == Key.Enter)
            {
                BtnAdd_Click(null, null);
            }
            else
            {
                foreach (string code in cbBadCode.Items)
                {
                    if (code.Contains(tboxBadCode.Text.ToUpper()))
                        cbBadCode.SelectedItem = code;
                }
            }
        }

        private void Tbox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void TboxCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tboxCount.Text == "" || tboxCount.Text == "0")
                btnAdd.IsEnabled = false;
            else
                btnAdd.IsEnabled = true;
        }

        private void TbMCOutput_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int nMCCount = string.IsNullOrEmpty(tbMCOutput.Text) ? 0 : Convert.ToInt32(tbMCOutput.Text.Replace(",", ""));
                int nOutput = string.IsNullOrEmpty(tbOutput.Text) ? 0 : Convert.ToInt32(tbOutput.Text.Replace(",", ""));

                tbCntDiff.Text = string.Format("{0:#,0}", nOutput - nMCCount);
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("IGS - POP_COMPLETE_Window.cs TbMCOutput_TextChanged Exception: {0}", ex.Message));
            }
        }

        private void TbMCOutput_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int nMCCount = string.IsNullOrEmpty(tbMCOutput.Text) ? 0 : Convert.ToInt32(tbMCOutput.Text.Replace(",", ""));
                tbMCOutput.Text = string.Format("{0:#,0}", nMCCount);
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("IGS - POP_COMPLETE_Window.cs TbMCOutput_LostFocus Exception: {0}", ex.Message));
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string tag = ((Button)sender).Tag.ToString();
            try
            {
                m_counts.Remove(m_counts.First(m => m.bad_code == tag));
            }
            catch { }
        }

        private void LbInfo_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroll = (ScrollViewer)svNGInfo;
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
