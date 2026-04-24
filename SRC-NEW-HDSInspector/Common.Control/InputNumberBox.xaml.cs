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
 * @file  InputNumberBox.xaml.cs
 * @brief
 *  behind code of InputNumberBox.xaml
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.09.05
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.09.05 First creation.
 */

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

namespace Common.Control
{
    public partial class InputNumberBox : Window
    {
        public string m_strResult = string.Empty;

        #region Constructors.
        public InputNumberBox()
            : this(string.Empty, string.Empty, string.Empty)
        {

        }

        public InputNumberBox(string strTitle, string strCaption, string strHelp)
        {
            InitializeComponent();
            InitializeDialog(strTitle, strCaption, strHelp);
            InitializeEvent();
        }
        #endregion

        #region Initializer.
        private void InitializeDialog(string strTitle, string strCaption, string strHelp)
        {
            this.Title = strTitle;
            this.txtCaption.Text = strCaption;
            this.txtHelp.Text = strHelp;

            this.txtUnitNumber.Focus();
        }

        private void InitializeEvent()
        {
            this.btnOK.Click += new RoutedEventHandler(btnOK_Click);
            this.btnCancel.Click += new RoutedEventHandler(btnCancel_Click);
        }
        #endregion

        #region Event handlers.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            CloseWindowWithResult();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }
        #endregion

        #region Close functions.
        private void CloseWindowWithResult()
        {
            m_strResult = this.txtUnitNumber.Text;
            this.DialogResult = true;
            this.Close();
        }

        private void CloseWindow()
        {
            this.DialogResult = false;
            this.Close();
        }
        #endregion

        public void SetMinValue(int nMinValue)
        {
            this.txtUnitNumber.Minimum = nMinValue;
        }

        public void SetMaxValue(int nMaxValue)
        {
            this.txtUnitNumber.Maximum = nMaxValue;
        }
    }
}
