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
    /// NGCount_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NGCount_Window : Window
    {
        #region Member Variables.
        private List<string> curOpcodeList = new List<string>();
        private List<string> codeList = new List<string>();

        public ObservableCollection<BadCountData> Counts
        {
            get { return m_count; }
        }
        private ObservableCollection<BadCountData> m_count = new ObservableCollection<BadCountData>();
        #endregion

        public NGCount_Window(ref STATE_TABLE_DATA data, ref List<BadCountData> orgList)
        {
            InitializeComponent();

            InitializeEvent();
            InitializeDialog(ref data, ref orgList);
        }

        private void InitializeEvent()
        {
            this.btnCancel.Click += BtnCancel_Click;
            this.btnOK.Click += BtnOK_Click;
            this.btnAdd.Click += BtnAdd_Click;
            this.btnSearch.Click += BtnSearch_Click;

            this.tbBadCode.PreviewKeyUp += TbBadCode_PreviewKeyUp;
            this.tboxCount.TextChanged += TboxCount_TextChanged;
            this.tboxCount.PreviewKeyDown += TboxCount_PreviewKeyDown;
            this.tboxCount.PreviewMouseLeftButtonUp += TboxCount_PreviewMouseLeftButtonUp;
            this.tboxCount.PreviewMouseLeftButtonDown += TboxCount_PreviewMouseLeftButtonDown;

            this.lbInfo.PreviewMouseWheel += LbInfo_PreviewMouseWheel;
            this.bdTitle.PreviewMouseLeftButtonDown += BdTitle_PreviewMouseLeftButtonDown;
        }

        private void InitializeDialog(ref STATE_TABLE_DATA curData, ref List<BadCountData> orgList)
        {
            //GET BAD CODE
            cbBadCode.Items.Clear();
            foreach (string key in ClientManager.badcodeList.Keys)
            {
                string strCode = string.Format("{0}, {1}", key, ClientManager.badcodeList[key]);

                cbBadCode.Items.Add(strCode);
                codeList.Add(strCode);
            }

            cbBadCode.SelectedIndex = 0;

            //GET OPCODE
            string errMsg;
            if (ReportMenuCtrl.clientManager.GetOPCodeList(curData.ORDER, out curOpcodeList, out errMsg) && curOpcodeList != null)
            {
                string inspCode = "";

                cbOPCode.Items.Clear();
                foreach (string code in curOpcodeList)
                {
                    cbOPCode.Items.Add(code);

                    if (ReportMenuCtrl.clientManager.CheckInspectCode(code))
                        inspCode = code;
                }

                if (inspCode == "")
                    cbOPCode.SelectedIndex = cbOPCode.Items.Count - 1;
                else
                    cbOPCode.SelectedItem = inspCode;
            }
            else
            {
                MessageBox.Show(string.Format("책임공정 정보를 불러오지 못했습니다.\n실패 사유: {0}", errMsg));
                ClientManager.Log(string.Format("IGS - NGCount_Window GetOPCodeList Fail. Error: {0}", errMsg));
            }

            //SET ORIGIN LIST
            if (orgList.Count != 0)
            {
                foreach (BadCountData cnt in orgList)
                    m_count.Add(cnt);
            }

            btnAdd.IsEnabled = false;
            lbInfo.DataContext = Counts;

            tbBadCode.Focus();
        }

        private void TbBadCode_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (tbBadCode.Text == "")
                return;

            if (e.Key == Key.Enter)
            {
                BtnAdd_Click(null, null);
            }
            else
            {
                foreach (string item in cbBadCode.Items)
                {
                    if (item.Contains(tbBadCode.Text.ToUpper()))
                        cbBadCode.SelectedItem = item;
                }
            }
        }

        private void TboxCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tboxCount.Text == "" || tboxCount.Text == "0")
                btnAdd.IsEnabled = false;
            else
                btnAdd.IsEnabled = true;
        }

        private void TboxCount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BtnAdd_Click(null, null);
        }

        private void TboxCount_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ReportMenuCtrl.clientManager.strSite == "LF")
                tboxCount.SelectAll();
        }

        private void TboxCount_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ReportMenuCtrl.clientManager.strSite == "LF")
                tboxCount.SelectAll();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
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

            if(m_count.Where(m => m.bad_code == strCode && m.op_code == strOP).Count() != 0)
            {
                MessageBox.Show("이미 추가된 불량 코드입니다.");
                return;
            }

            BadCountData data = new BadCountData();
            data.bad_code = strCode;
            data.op_code = strOP;
            data.ngCount = nCount;

            m_count.Add(data);

            tbBadCode.Text = "";
            tbBadCode.Focus();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            SubWindow.CODE_SEARCH_Window dlg = new CODE_SEARCH_Window();
            dlg.InitializeDialog(codeList);
            if ((bool)dlg.ShowDialog())
                cbBadCode.SelectedItem = dlg.selectedCode;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string tag = ((Button)sender).Tag.ToString();
            try
            {
                m_count.Remove(m_count.First(m => m.bad_code == tag));
            }
            catch { }
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

        private void TextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ReportMenuCtrl.clientManager.strSite == "LF")
                (sender as TextBox).SelectAll();
        }
    }
}
