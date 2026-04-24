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
using System.Timers;

namespace HDSInspector.SubWindow
{
    /// <summary>
    /// StatusProgWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StatusProgWindow : Window
    {
        /// <summary>   Void delegete. </summary>
        public delegate void VoidDelegete();

        /// <summary> The timer </summary>
        private Timer timer;

        public StatusProgWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        /// <summary>   Executes the loaded action. </summary>
        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            timer = new Timer(100);
            timer.Elapsed += OnTimerElapsed;
            timer.Start();
        }

        /// <summary>   Executes the timer elapsed action. </summary>
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            rotationCanvas.Dispatcher.Invoke
            (
                new VoidDelegete(
                    delegate
                    {
                        SpinnerRotate.Angle += 30;
                        if (SpinnerRotate.Angle == 360)
                        {
                            SpinnerRotate.Angle = 0;
                        }
                    }
                    ),
                null
            );
        }

        public void SetLabel(string astrStatus)
        {
            this.Dispatcher.Invoke(new Action(delegate
            {
            lblStatus.Content = astrStatus;
            }));
        }

        /// <summary>   Stops this object. </summary>
        public void Stop()
        {
            timer.Dispose();
        }
    }
}
