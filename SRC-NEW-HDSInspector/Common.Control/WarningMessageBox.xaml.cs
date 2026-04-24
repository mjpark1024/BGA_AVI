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
 * @file  WarningMessageBox.xaml.cs
 * @brief
 *  behind code of WarningMessageBox.xaml
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.08.11
 * @version : 1.1
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.11 First creation.
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
    /// <summary>   Interaction logic for WarningMessageBox.xaml. </summary>
    /// <remarks>   suoow2, 2014-08-11. </remarks>
    public partial class WarningMessageBox : Window
    {
        #region Constructors.
        /// <summary>   Initializes a new instance of the WarningMessageBox class. </summary>
        /// <remarks>   suoow2, 2014-08-11. </remarks>
        public WarningMessageBox()
            : this(string.Empty, string.Empty)
        {
            
        }

        /// <summary>   Initializes a new instance of the WarningMessageBox class. </summary>
        /// <remarks>   suoow2, 2014-08-11. </remarks>
        /// <param name="strWarningMessage">    Message describing the string warning. </param>
        public WarningMessageBox(string strWarningMessage)
            : this(strWarningMessage, string.Empty)
        {
        
        }

        /// <summary>   Initializes a new instance of the WarningMessageBox class. </summary>
        /// <remarks>   suoow2, 2014-08-11. </remarks>
        /// <param name="strWarningMessage">    Message describing the string warning. </param>
        /// <param name="strTitle">             The string title. </param>
        public WarningMessageBox(string strWarningMessage, string strTitle)
        {
            InitializeComponent();
            InitializeEvent();

            if (!string.IsNullOrEmpty(strWarningMessage))
            {
                this.txtMessage.Text = strWarningMessage;
            }
            if (!string.IsNullOrEmpty(strTitle))
            {
                this.Title = strTitle;
            }
        }
        #endregion

        /// <summary>   Initializes the event. </summary>
        /// <remarks>   suoow2, 2014-08-11. </remarks>
        private void InitializeEvent()
        {
            this.btnOK.Click += new RoutedEventHandler(btnOK_Click);
            this.btnCancel.Click += new RoutedEventHandler(btnCancel_Click);
            this.KeyDown += new KeyEventHandler(WarningMessageBox_KeyDown);
        }

        #region Event handlers.
        /// <summary>   Event handler. Called by WarningMessageBox for key down events. </summary>
        /// <remarks>   suoow2, 2014-08-11. </remarks>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Key event information. </param>
        private void WarningMessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                CloseWindowWithInputMessage();
            }
            else if (e.Key == Key.Escape)
            {
                CloseWindow();
            }
        }

        /// <summary>   Event handler. Called by btnOK for click events. </summary>
        /// <remarks>   suoow2, 2014-08-11. </remarks>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            CloseWindowWithInputMessage();
        }

        /// <summary>   Event handler. Called by btnCancel for click events. </summary>
        /// <remarks>   suoow2, 2014-08-11. </remarks>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }
        #endregion

        #region Close functions.
        /// <summary>   Closes the window with input message. </summary>
        /// <remarks>   suoow2, 2014-08-10. </remarks>
        private void CloseWindowWithInputMessage()
        {
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>   Closes the window. </summary>
        /// <remarks>   suoow2, 2014-08-10. </remarks>
        private void CloseWindow()
        {
            this.DialogResult = false;
            this.Close();
        }
        #endregion
    }
}
