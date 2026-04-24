using IGS.Classes;
using System;
using System.Collections.Generic;
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

namespace IGS
{
    /// <summary>
    /// StartTestWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StartTestWindow : Window
    {
        //설비의 자동검사 시작 시뮬레이션 Window
        //기능 테스트용이며, 실제 양산 적용 시 설비의 LotWindow 또는 적절한 위치에 구현해야한다.

        private string curOpcode = "";          //MES상 현재 공정코드

        public bool bNormalLot = false;         //MES상 존재하는 Lot번호인가
        public bool bRestart = false;           //재검여부
        public string stdModel = "";            //표준모델명 - 기종교체 판단용

        public StartTestWindow()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }
        
        private void InitializeDialog()
        {
            STATE_TABLE_DATA data;
            if (ReportMenuCtrl.clientManager.GetCurStateInfo(out data))
            {
                txtOperator.Text = data.USER_ID;
                tbOperatorName.Text = data.USER_NAME;

                if (data.USER_ID != "")
                    TxtOperator_KeyUp(null, null);

                tbVeriOperator.Text = data.VERIFY_USER;
                txtVerifyYN.Text = data.VERIFY_YN;

                if (data.VERIFY_YN == "N")
                    grdVerifyInfo.IsEnabled = false;
            }
        }

        private void InitializeEvent()
        {
            this.txtLot.KeyUp += TxtLot_KeyUp;
            this.txtOperator.KeyUp += TxtOperator_KeyUp;

            this.btnVerifyDel.Click += BtnVerifyDel_Click;
            this.btnVerifyChange.Click += BtnVerifyChange_Click;
            this.btnOK.Click += BtnOK_Click;
            this.btnCancel.Click += BtnCancel_Click;
        }

        private void TxtOperator_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtOperator.Text.Length == 6)
            {
                string strID = txtOperator.Text;
                Regex regex = new Regex(@"[0-9]");
                if (!regex.IsMatch(strID))
                {
                    MessageBox.Show("Operator 정보는 사번만 입력 가능합니다.");
                    return;
                }

                POP_USER_DATA user;
                string errMsg = "";
                if (ReportMenuCtrl.clientManager.GetUserInfo(strID, out user, out errMsg))
                    tbOperatorName.Text = user.USER_NM;
                else
                    tbOperatorName.Text = "";
            }
            else
                tbOperatorName.Text = "";
        }

        private void TxtLot_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtLot.Text.Length == 9)
            {
                POP_LOT_DATA lotData;
                if (ReportMenuCtrl.clientManager.GetMESLotInfo(txtLot.Text, out lotData) && lotData != null)
                {
                    curOpcode = lotData.OP_CD;
                    stdModel = lotData.ITEM_CD;

                    spMESState.Visibility = Visibility.Visible;
                    tbOpState.Text = string.Format("공정 - {0}, 상태 - {1}", lotData.OP_CD, lotData.OP_STAT);

                    bNormalLot = true;

                    if (lotData.OP_CD.Contains("VI"))
                        txtOpcode.Text = lotData.OP_CD;
                    else
                        txtOpcode.Text = "";

                    txtGanjiYN.Text = lotData.SP_YN;
                    if (lotData.SP_YN == "Y")
                    {
                        txtPlate.Text = lotData.PLATE;
                        txtGanjiSpec.Text = lotData.GANJI_SPEC;
                    }
                    else
                    {
                        txtPlate.Text = "";
                        txtGanjiSpec.Text = "";
                    }
                }
                else
                {
                    curOpcode = "";
                    spMESState.Visibility = Visibility.Hidden;
                    bNormalLot = false;
                }
            }
            else
                bNormalLot = false;
        }

        private void BtnVerifyDel_Click(object sender, RoutedEventArgs e)
        {
            tbVeriOperator.Text = "";
        }

        private void BtnVerifyChange_Click(object sender, RoutedEventArgs e)
        {
            SubWindow.VERIFY_USER_LIST_Window dlg = new SubWindow.VERIFY_USER_LIST_Window();
            if ((bool)dlg.ShowDialog())
            {
                string strUser = "";
                for (int i = 0; i < dlg.selectedUser.Count; i++)
                {
                    if (i > 0)
                        strUser += ",";
                    strUser += string.Format("{0}({1})", dlg.selectedUser[i].userName, dlg.selectedUser[i].userID);
                }

                tbVeriOperator.Text = strUser;
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            //조건 검사
            if (txtLot.Text == "")
            {
                MessageBox.Show("LOT 번호를 입력 바랍니다.");
                return;
            }

            if (txtOpcode.Text == "")
            {
                MessageBox.Show("공정코드를 입력 바랍니다.");
                return;
            }

            if (txtOperator.Text == "" || txtOperator.Text.Length != 6 || tbOperatorName.Text == "")
            {
                MessageBox.Show("작업자 정보를 확인해주세요.");
                txtOperator.Focus();
                return;
            }

            if (txtVerifyYN.Text == "Y" && tbVeriOperator.Text == "")
            {
                MessageBox.Show("Verify 작업자 정보를 입력 바랍니다.");                
                return;
            }

            if (txtGanjiYN.Text == "Y")
            {
                if (tboxGanjiSpec.Text == "")
                {
                    MessageBox.Show("간지 사양을 입력 바랍니다.");
                    tboxGanjiSpec.Focus();
                    return;
                }

                if (tboxGanjiLot.Text == "")
                {
                    MessageBox.Show("간지 LOT를 입력 바랍니다.");
                    tboxGanjiLot.Focus();
                    return;
                }

                if (txtGanjiSpec.Text != tboxGanjiSpec.Text.Trim())
                {
                    MessageBox.Show("간지 기준사양과 입력사양이 다릅니다.");
                    return;
                }
            }

            if (bNormalLot && curOpcode != "" && !ReportMenuCtrl.clientManager.CheckInspectCode(curOpcode))
            {
                if (ReportMenuCtrl.clientManager.GetOpStateCheck(txtLot.Text, out POP_START_STATE_OUTPUT_DATA info, out string errMsg))
                {
                    if (info.CHECK_CD == "NG")
                    {
                        if (MessageBox.Show(string.Format("{0}\n'D58-재작업 및 재검' 유실코드로 비가동 처리하시겠습니까?", info.CHECK_MSG), "확인", MessageBoxButton.YesNo) == MessageBoxResult.No)
                            return;

                        ClientManager.Log(String.Format("{0}. D58 유실코드로 비가동 진행", info.CHECK_MSG));
                        bRestart = true;
                    }
                }
                else
                    ClientManager.Log(string.Format("GetOpStateCheck Fail. Error: {0}", errMsg));
            }

            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
