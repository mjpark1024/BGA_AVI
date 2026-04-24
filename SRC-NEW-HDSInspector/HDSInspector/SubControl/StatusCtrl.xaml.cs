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
 * @file  StatusCtrl.xaml.cs
 * @brief . 
 *  behind code of StatusCtrl.xaml
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.05.09
 * @version : 1.1
 * 
 * <b> Revision Histroy </b>
 * - 2011.05.09 First creation.
 */

using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace HDSInspector
{
    /// <summary>   Status control.  </summary>
    public partial class StatusCtrl : UserControl
    {
        public StatusCtrl()
        {
            InitializeComponent();
        }

        public void SetText(Color color, string text)
        {
            txtStatus.Text = text;
            txtStatus.Foreground = new SolidColorBrush(color);
        }
    }
}
