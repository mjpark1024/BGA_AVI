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
/**
 * @file  ISProgressWindow.xaml.cs
 * @brief
 *  Container of IS-progress bar(s).
 * 
 * @author : suoow <suoow.yeo@haesung.net>
 * @date : 2011.11.19
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.11.19 First creation.
 */

using System;
using System.Windows;
using HDSInspector.SubControl;
using Common;
using System.Threading;

namespace HDSInspector.SubWindow
{
    /// <summary>   Form for viewing the is progress.  </summary>
    /// <remarks>   Cheol Min, Shin, 2014-11-19. </remarks>
    public partial class ISProgressWindow : Window
    {
        public ISProgressWindow()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            Loaded += ISProgressWindow_Loaded;
        }

        private void ISProgressWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (ISProgress progressbar in pnlProgress.Children)
            {
                progressbar.SetProgressValue(0);
            }
        }

        public void StartProgress(int anTargetVision)
        {
            if (anTargetVision < 0)
            {
                for (int i = 0; i < this.pnlProgress.Children.Count; i++)
                {
                    this.pnlProgress.Children[i].Visibility = Visibility.Visible;
                    SetProgressValue(i, 0, "준 비");
                }
            }
            else
            {
                for (int i = 0; i < this.pnlProgress.Children.Count; i++)
                {
                    if (i == anTargetVision)
                    {
                        this.pnlProgress.Children[i].Visibility = Visibility.Visible;
                        SetProgressValue(anTargetVision, 0, "준 비");
                    }
                    else
                    {
                        this.pnlProgress.Children[i].Visibility = Visibility.Collapsed;
                    }
                }
            }
            this.Show();
        }

        public void StopProgress(int anTargetVision)
        {
            if (anTargetVision < 0)
            {
                for (int i = 0; i < this.pnlProgress.Children.Count; i++)
                {
                    SetProgressValue(i, 100, "종 료");
                }
            }
            else
            {
                SetProgressValue(anTargetVision, 100, "종 료");
            }

            Thread.Sleep(100);
            this.Hide();
        }

        public void AddProgressBar(string astrProgName)
        {
            ISProgress isProgressBar = new ISProgress(astrProgName) { Visibility = Visibility.Collapsed };
            this.pnlProgress.Children.Add(isProgressBar);
        }

        public void SetVisibility(int anTargetVision, bool abIsVisible)
        {
            pnlProgress.Children[anTargetVision].Visibility = abIsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public void SetProgressValue(int anTargetVision, double afValue, string aszState)
        {
            ((ISProgress)pnlProgress.Children[anTargetVision]).Progressbar.Refresh();

            this.Dispatcher.Invoke(new Action(delegate
            {
                ((ISProgress)pnlProgress.Children[anTargetVision]).Progressbar.Value = afValue;
                ((ISProgress)pnlProgress.Children[anTargetVision]).txtISState.Text = aszState;
                ((ISProgress)pnlProgress.Children[anTargetVision]).Progressbar.Refresh();
            }));
        }
    }
}
