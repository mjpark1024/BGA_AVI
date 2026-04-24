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
using System.Timers;

namespace Common.Control
{
    /// <summary>   Interaction logic for RoundRectProgressBar.xaml. </summary>
    public partial class RoundRectProgressBar : Window
    {
        /// <summary> The percents text </summary>
        private const string PERCENTS_TEXT = "{0}%";

        /// <summary>   Void delegete. </summary>
        public delegate void VoidDelegete();

        /// <summary> The timer </summary>
        private Timer timer;

        /// <summary>   Initializes a new instance of the RoundRectProgressBar class. </summary>
        public RoundRectProgressBar()
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

        /// <summary>   Stops this object. </summary>
        public void Stop()
        {
            timer.Dispose();
        }
    } 
}
