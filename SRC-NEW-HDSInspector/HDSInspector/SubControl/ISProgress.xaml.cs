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
using System.Windows.Controls;
using Common;

namespace HDSInspector.SubControl
{
    /// <summary>   Is progress.  </summary>
    /// <remarks>   Cheol Min, Shin, 2014-11-19. </remarks>
    public partial class ISProgress : UserControl
    {
        public ISProgress(string astrProgessName)
        {
            InitializeComponent();
            InitializeDialog(astrProgessName);
        }

        private void InitializeDialog(string astrProgessName)
        {
            this.txtISName.Text = astrProgessName;
        }

        public void SetProgressValue(double afProgressValue)
        {
            this.Progressbar.Value = afProgressValue;
            this.Progressbar.Refresh();
        }
    }
}