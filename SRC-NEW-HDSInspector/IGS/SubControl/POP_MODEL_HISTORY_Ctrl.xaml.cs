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
using System.Windows.Navigation;
using System.Windows.Shapes;
using IGS.Classes;

namespace IGS.SubControl
{
    /// <summary>
    /// POP_MODEL_HISTORY_Ctrl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class POP_MODEL_HISTORY_Ctrl : UserControl
    {
        public ObservableCollection<ModelHistoryDisplayData> Hists
        {
            get { return m_hists; }
        }
        private ObservableCollection<ModelHistoryDisplayData> m_hists = new ObservableCollection<ModelHistoryDisplayData>();

        public POP_MODEL_HISTORY_Ctrl()
        {
            InitializeComponent();
            InitializeEvent();

            lbInfo.DataContext = m_hists;
        }

        private void InitializeEvent()
        {
            lbInfo.PreviewMouseWheel += LbInfo_PreviewMouseWheel;
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

        public void InitializeFail()
        {
            m_hists.Clear();
            bdFail.Visibility = Visibility.Visible;
        }

        public void InitializeDialog(List<POP_RECENT_HISTORY_DATA> list)
        {
            m_hists.Clear();
            bdFail.Visibility = Visibility.Hidden;

            for (int i = 0; i < list.Count; i++)
            {
                ModelHistoryDisplayData data = new ModelHistoryDisplayData();
                data.idx = list[i].NUM;
                data.model_code = list[i].ITEM_CD;
                data.mc_code = list[i].EQPT_CD;
                string strDate = list[i].PROD_DT;
                data.prod_dt = string.Format("{0}-{1}-{2}", strDate.Substring(0, 4), strDate.Substring(4, 2), strDate.Substring(6, 2));

                m_hists.Add(data.Clone());
            }
        }
        
        public void ListClear()
        {
            m_hists.Clear();
        }
    }
}
