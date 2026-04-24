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
    /// POP_WARNING_Ctrl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class POP_WARNING_Ctrl : UserControl
    {
        POP_WARNING_INFO curInfo;

        public ObservableCollection<WarningDisplayData> Warnings
        {
            get { return m_warnings; }
        }
        private ObservableCollection<WarningDisplayData> m_warnings = new ObservableCollection<WarningDisplayData>();

        public POP_WARNING_Ctrl()
        {
            InitializeComponent();
            InitializeEvent();
        }
        
        private void InitializeEvent()
        {
            lbInfo.PreviewMouseWheel += LbInfo_PreviewMouseWheel;
            lbInfo.DataContext = m_warnings;
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
            curInfo = null;

            m_warnings.Clear();
            WarningDisplayData data = new WarningDisplayData();
            data.msg = "MES 조회에 실패하였습니다.\n기존 POP 프로그램으로 조회바랍니다.";
            m_warnings.Add(data);
        }

        public void InitializeDialog(POP_WARNING_INFO warning)
        {
            curInfo = warning;

            m_warnings.Clear();
            if (warning.WARNING_INFO.Count == 0)
            {
                tbResultMsg.Text = "";

                WarningDisplayData data = new WarningDisplayData();
                data.msg = "정상 오더입니다.";
                m_warnings.Add(data);
            }
            else
            {
                tbResultMsg.Text = warning.RESULT_MSG;

                for (int i = 0; i < warning.WARNING_INFO.Count; i++)
                {
                    POP_WARNING_MSG msg = warning.WARNING_INFO[i];

                    WarningDisplayData data = new WarningDisplayData();
                    data.control_num = msg.CONTROL_NUM;
                    data.rule = msg.RULE;
                    if (data.rule == "HOLDING")
                        data.color = new SolidColorBrush(Colors.Red);
                    data.regist = msg.REGIST;
                    for (int j = 0; j < msg.MSG.Count; j++)
                    {
                        if (j != 0) data.msg += "\n";
                        data.msg += msg.MSG[j];
                    }
                    m_warnings.Add(data.Clone());
                }
            }
        }

        public void ListClear()
        {
            m_warnings.Clear();
        }
    }
}
