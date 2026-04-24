/*********************************************************************************
 * Copyright(c) 2015 by Haesung DS.
 * 
 * This software is copyrighted by, and is the sole property of Haesung DS.
 * All rigths, title, ownership, or other interests in the software remain the
 * property of Haesung DS. This software may only be used in accordance with
 * the corresponding license agreement. Any unauthorized use, duplication, 
 * transmission, distribution, or disclosure of this software is expressly 
 * forbidden.
 *
 * This Copyright notice may not be removed or modified without prior written
 * consent of Haesung DS reserves the right to modify this 
 * software without notice.
 *
 * Haesung DS.
 * KOREA 
 * http://www.HaesungDS.com
 *********************************************************************************/

using System;
using System.Windows.Controls;
using System.Windows.Input;
using Common;

namespace HDSInspector
{
    /// <summary>   Log control.  </summary>
    /// <remarks>   suoow2, 2014-09-17. </remarks>
    public partial class LogCtrl : UserControl
    {
        /// <summary>   Initializes a new instance of the LogCtrl class. </summary>
        /// <remarks>   suoow2, 2014-09-17. </remarks>
        public LogCtrl()
        {
            InitializeComponent();

            MainWindow.LogEvent += new LogEventHandler(MainWindow_LogEvent);
            this.lbLog.PreviewMouseWheel += new MouseWheelEventHandler(lbLog_PreviewMouseWheel);
        }

        /// <summary>   Event handler. Called by lbLog for preview mouse wheel events. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-10-08. </remarks>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Mouse wheel event information. </param>
        void lbLog_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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

        /// <summary>   Main window log event. </summary>
        /// <remarks>   suoow2, 2014-09-17. </remarks>
        /// <param name="log">  The log. </param>
        private void MainWindow_LogEvent(LogType log)
        {
            try
            {
                Action a = delegate
                {
                    this.lbLog.Items.Add(log);
                    this.svLogViewer.ScrollToVerticalOffset(svLogViewer.MaxHeight);
                }; this.Dispatcher.Invoke(a);
            }
            catch{}
            //this.Dispatcher.Invoke(new Action(delegate {
            //    this.lbLog.Items.Add(log);
            //    this.svLogViewer.ScrollToVerticalOffset(svLogViewer.MaxHeight);
            //}));
        }
    }
}
