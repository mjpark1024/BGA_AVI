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
using IGS.Classes;

namespace IGS.SubWindow
{
    /// <summary>
    /// POP_LOT_SEARCH_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class POP_LOT_SEARCH_Window : Window
    {
        public POP_LOT_SEARCH_Window()
        {
            InitializeComponent();
            InitializeEvent();
            InitializeDialog();
        }

        private void InitializeEvent()
        {
            this.btnSearch.Click += BtnSearch_Click;
            this.KeyDown += POP_LOT_SEARCH_Window_KeyDown;

            this.tbLot.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;
            this.tbOpcode.PreviewMouseLeftButtonUp += Tbox_PreviewMouseLeftButtonUp;
        }

        private void InitializeDialog()
        {
            STATE_TABLE_DATA state;
            if (ReportMenuCtrl.clientManager.GetCurStateInfo(out state))
            {
                tbMachine.Text = state.MACHINE_CODE;
                tbLot.Text = state.ORDER;
                tbOpcode.Text = state.OP_CODE;
            }
        }

        private void POP_LOT_SEARCH_Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BtnSearch_Click(null, null);
        }

        private void Tbox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string strLot = tbLot.Text;
            if (strLot == "" || strLot.Length != 9)
            {
                MessageBox.Show("LOT 번호가 올바르지 않습니다.");
                return;
            }

            string strOpcode = tbOpcode.Text;
            if (strOpcode == "" || strOpcode.Length != 4)
            {
                MessageBox.Show("공정코드가 올바르지 않습니다.");
                return;
            }

            LotSearch(strLot, strOpcode);
        }

        private void LotSearch(string strLot, string strOpcode)
        {
            try
            {
                string errMsg = "";

                this.Cursor = Cursors.Wait;

                //CLEAR UI
                historyCtrl.ListClear();
                warningCtrl.ListClear();
                changeCtrl.ListClear();
                tbStartCheckCode.Text = "";
                tbStartCheckMsg.Text = "";

                //GET START STATE CHECK
                POP_START_STATE_OUTPUT_DATA output;
                POP_START_STATE_INPUT_DATA start = new POP_START_STATE_INPUT_DATA();
                start.WONO = tbLot.Text.Trim();
                start.OPCD = tbOpcode.Text.Trim();
                start.EQPT_CD = tbMachine.Text;
                if (ReportMenuCtrl.clientManager.GetStartStateCheck(start, out output, out errMsg))
                {
                    if (output.CHECK_CD == "OK")
                        tbStartCheckCode.Foreground = new SolidColorBrush(Colors.Green);
                    else
                        tbStartCheckCode.Foreground = new SolidColorBrush(Colors.Tomato);

                    tbStartCheckCode.Text = output.CHECK_CD;
                    tbStartCheckMsg.Text = output.CHECK_MSG;
                }
                else
                {
                    tbStartCheckCode.Text = "";
                    tbStartCheckMsg.Text = "MES 조회에 실패하였습니다.";
                }

                //GET RECENT MODEL HISTORY
                List<POP_RECENT_HISTORY_DATA> history;
                if (ReportMenuCtrl.clientManager.GetModelHistoryInfo(strLot, out history, out errMsg))
                    historyCtrl.InitializeDialog(history);
                else
                    historyCtrl.InitializeFail();

                //GET WARNING HOLDING INFO
                POP_WARNING_INFO warning;
                if (ReportMenuCtrl.clientManager.GetWarningInfo(strLot, strOpcode, out warning, out errMsg))
                    warningCtrl.InitializeDialog(warning);
                else
                    warningCtrl.InitializeFail();

                //GET CHANGE POINT INFO
                List<POP_CHANGE_POINT_OUTPUT_DATA> change;
                if (ReportMenuCtrl.clientManager.GetChangePointInfo(strLot, out change, out errMsg))
                    changeCtrl.InitializeDialog(change);
                else
                    changeCtrl.InitializeFail();
            }
            catch(Exception ex)
            {
                MessageBox.Show(String.Format("LOT 정보를 검색하는데 문제가 발생하였습니다.\nError:{0}", ex.Message));
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }
    }
}
