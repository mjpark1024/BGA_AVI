using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;

namespace Common.Control
{
    /// <summary>
    /// Interaction logic for BasicProgressbar.xaml
    /// </summary>
    public partial class BasicProgressbar : UserControl
    {
        private bool Canceled = false;
        public bool m_Canceled
        {
            get
            {
                return Canceled;
            }
        }

        private double Maximum;
        public double m_Maximum
        {
            get
            {
                return Maximum;
            }
            set
            {
                Maximum = value;
                this.Progress.Maximum = Maximum;
            }
        }

        private double Minimum;
        public double m_Minimum
        {
            get
            {
                return Minimum;
            }
            set
            {
                Minimum = value;
                this.Progress.Minimum = Minimum;
            }
        }

        public BasicProgressbar()
        {
            InitializeComponent();
        }

        public void CancelProgress()
        {
            Canceled = true;
        }

        public void UpdateProgress(double progress)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background,
               (SendOrPostCallback)delegate { Progress.Value = progress; }, null);
        }

        public void UpdateStatus(string status)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background,
               (SendOrPostCallback)delegate { txtStatus.Text = status; }, null);
        }

        public void Finish()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background,
               (SendOrPostCallback)delegate { Progress.IsEnabled = false; }, null);
        }
    }
}
