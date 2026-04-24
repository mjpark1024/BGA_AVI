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
    /// LEAV_Count_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LEAV_Count_Window : Window
    {
        #region Member Variables.
        private List<string> machineList;
        private string curID;

        public ObservableCollection<CountDisplayData> Counts
        {
            get { return m_count; }
        }
        private ObservableCollection<CountDisplayData> m_count = new ObservableCollection<CountDisplayData>();

        private bool bOnCombo = false;
        #endregion

        public LEAV_Count_Window(List<string> mcList, string strID)
        {
            InitializeComponent();

            machineList = mcList;
            curID = strID;

            InitializeEvent();
            InitializeDialog();
        }

        private void InitializeEvent()
        {
            this.btnCancel.Click += BtnCancel_Click;
            this.btnOK.Click += BtnOK_Click;
            
            this.lbInfo.PreviewMouseWheel += LbInfo_PreviewMouseWheel;
            this.bdTitle.PreviewMouseLeftButtonDown += BdTitle_PreviewMouseLeftButtonDown;
        }

        private void InitializeDialog()
        {
            //GET LOSS CODE LIST
            List<string> lossList = new List<string>();
            string errMsg;
            if (!ReportMenuCtrl.clientManager.GetLossCodeList(curID, out lossList, out errMsg))
            {
                MessageBox.Show(string.Format("유실 코드 정보를 불러오지 못했습니다.\n실패 사유: {0}", errMsg));
                ClientManager.Log(string.Format("IGS - LEAV_Count_Window GetLossCodeList Fail. Error: {0}", errMsg));
                return;
            }
            string defaultLoss = lossList.First(x => x.Contains("D54"));

            //SET COUNT LIST
            m_count.Clear();
            foreach (string mc in machineList)
            {
                CountDisplayData data = new CountDisplayData();

                data.mc_code = mc;

                data.lossItems = new List<string>();
                foreach (string loss in lossList)
                    data.lossItems.Add(loss);

                data.lossCode = defaultLoss;
                m_count.Add(data.Clone());
            }

            lbInfo.DataContext = m_count;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format("선택된 {0}개 설비에 대하여\n{1} 사번으로 퇴근 보고를 진행하시겠습니까?", machineList.Count, curID),
                   "확인", MessageBoxButton.YesNo) == MessageBoxResult.No)
                this.DialogResult = false;
            else
                this.DialogResult = true;
        }

        private void LbInfo_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!bOnCombo)
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
        }

        private void BdTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnNG_Click(object sender, RoutedEventArgs e)
        {
            string strMC = ((Button)sender).Tag.ToString();

            STATE_TABLE_DATA data;
            if(!ReportMenuCtrl.clientManager.GetStateInfo(strMC, out data))
            {
                MessageBox.Show("서버에서 필요한 정보를 가져오지 못했습니다.");
                ClientManager.Log(string.Format("IGS - LEAV_Count_Window GetStateInfo Fail"));
                return;
            }

            CountDisplayData curMC = m_count.First(m => m.mc_code == strMC);
            SubWindow.NGCount_Window dlg = new NGCount_Window(ref data, ref curMC.badCounts);
            if((bool)dlg.ShowDialog())
            {
                try
                {
                    int nTotal = 0;
                    curMC.badCounts.Clear();
                    foreach (BadCountData cnt in dlg.Counts)
                    {
                        nTotal += cnt.ngCount;
                        curMC.badCounts.Add(cnt);
                    }
                    curMC.NGTotalCount = nTotal;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(string.Format("불량 정보를 집계하는데 문제가 발생하였습니다.\nException: {0}", ex.Message));
                    ClientManager.Log(string.Format("IGS - LEAV_Count_Window 불량 정보를 집계하는데 문제가 발생하였습니다. Exception: {0}", ex.Message));
                }
            }
        }

        private void cbLossCode_MouseEnter(object sender, MouseEventArgs e)
        {
            bOnCombo = true;
        }

        private void cbLossCode_MouseLeave(object sender, MouseEventArgs e)
        {
            bOnCombo = false;
        }

        private void TextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ReportMenuCtrl.clientManager.strSite == "LF")
                (sender as TextBox).SelectAll();
        }
    }
}
