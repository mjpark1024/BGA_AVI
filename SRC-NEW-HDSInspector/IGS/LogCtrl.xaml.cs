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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IGS
{
    /// <summary>
    /// LogCtrl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LogCtrl : UserControl
    {
        public LogCtrl()
        {
            InitializeComponent();
            this.lbLog.PreviewMouseWheel += LbLog_PreviewMouseWheel;
        }

        private void LbLog_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double m_MouseScroll = this.svLogViewer.VerticalOffset;

            m_MouseScroll -= e.Delta / 3;
            // At Start Point
            if (m_MouseScroll < 0)
            {
                m_MouseScroll = 0;
                this.svLogViewer.ScrollToVerticalOffset(m_MouseScroll);
            }
            // At End Point
            else if (m_MouseScroll > this.svLogViewer.ScrollableHeight)
            {
                m_MouseScroll = this.svLogViewer.ScrollableHeight;
                this.svLogViewer.ScrollToEnd();
            }
            // Middle
            else
            {
                this.svLogViewer.ScrollToVerticalOffset(m_MouseScroll);
            }
        }

        public void AddLog(LogType log)
        {
            this.Dispatcher.Invoke(new Action(delegate
            {
                if (lbLog.Items.Count >= 300)
                {
                    for (int i = 0; i < 100; i++)
                        lbLog.Items.RemoveAt(0);
                }
                this.lbLog.Items.Add(log);
                this.svLogViewer.ScrollToVerticalOffset(svLogViewer.MaxHeight);
            }));
        }
    }

    public class LogType
    {
        public LogType(string aszMessage) : this(DateTime.Now, aszMessage) { }
        public LogType(DateTime aTime, string aszMessage)
        {
            this.Time = aTime;
            this.Message = aszMessage;
        }

        public DateTime Time { get; set; }
        public string Message { get; set; }
    }
}
