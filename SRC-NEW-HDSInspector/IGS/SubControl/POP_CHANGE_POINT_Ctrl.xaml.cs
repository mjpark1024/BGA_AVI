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
    /// POP_CHANGE_POINT_Ctrl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class POP_CHANGE_POINT_Ctrl : UserControl
    {
        public ObservableCollection<ChangepointDisplayData> Points
        {
            get { return m_points; }
        }
        private ObservableCollection<ChangepointDisplayData> m_points = new ObservableCollection<ChangepointDisplayData>();

        public POP_CHANGE_POINT_Ctrl()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            lbInfo.PreviewMouseWheel += LbInfo_PreviewMouseWheel;
            lbInfo.DataContext = m_points;
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
            m_points.Clear();
            ChangepointDisplayData data = new ChangepointDisplayData();
            data.chg_title = "MES 조회에 실패하였습니다.\n기존 POP 프로그램으로 조회바랍니다.";
            m_points.Add(data);
        }

        public void InitializeDialog(List<POP_CHANGE_POINT_OUTPUT_DATA> changes)
        {
            m_points.Clear();
            if (changes.Count == 0)
            {
                ChangepointDisplayData data = new ChangepointDisplayData();
                data.chg_title = "변경점 없음";
                m_points.Add(data);
            }
            else
            {
                for (int i = 0; i < changes.Count; i++)
                {
                    ChangepointDisplayData data = new ChangepointDisplayData();
                    data.chg_div = changes[i].CHG_DIV;
                    data.chg_title = changes[i].CHG_TITLE;
                    data.item_cd = changes[i].ITEM_CD;
                    data.op_cd = changes[i].OP_CD;
                    data.eqpt_cd = changes[i].EQPT_CD;
                    data.reg_dt = string.Format("{0}-{1}-{2}", changes[i].REG_DT.Substring(0, 4), changes[i].REG_DT.Substring(4, 2), changes[i].REG_DT.Substring(6, 2));
                    m_points.Add(data);
                }
            }
        }

        public void ListClear()
        {
            m_points.Clear();
        }
    }
}
