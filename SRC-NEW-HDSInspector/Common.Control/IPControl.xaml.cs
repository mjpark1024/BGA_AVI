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
 * @file  IPControl.xaml.cs
 * @brief
 *  IP address user control.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.08.02
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.02 First creation.
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
    /// <summary>   Interaction logic for IPControl.xaml. </summary>
    /// <remarks>   suoow2, 2014-08-02. </remarks>
    public partial class IPControl : UserControl
    {
        /// <summary>   Initializes a new instance of the IPControl class. </summary>
        /// <remarks>   suoow2, 2014-08-02. </remarks>
        public IPControl()
        {
            InitializeComponent();
        }

        /// <summary>   Gets or sets the ip address. </summary>
        /// <value> The ip address. </value>
        public string IPAddress
        {
            get
            {
                if (string.IsNullOrEmpty(txtAClass.Text) || string.IsNullOrEmpty(txtBClass.Text) ||
                    string.IsNullOrEmpty(txtCClass.Text) || string.IsNullOrEmpty(txtDClass.Text))
                {
                    return string.Empty;
                }
                else
                {
                    return String.Format("{0}.{1}.{2}.{3}", txtAClass.Text, txtBClass.Text, txtCClass.Text, txtDClass.Text);
                }
            }
            set
            {
                if(value == "localhost")
                {
                    txtAClass.Text = "127";
                    txtBClass.Text = "0";
                    txtCClass.Text = "0";
                    txtDClass.Text = "1";
                }
                else
                {
                    string[] tokens = value.Split(new string[] { "." }, StringSplitOptions.None);
                    if (tokens.Length == 4)
                    {
                        txtAClass.Text = tokens[0];
                        txtBClass.Text = tokens[1];
                        txtCClass.Text = tokens[2];
                        txtDClass.Text = tokens[3];
                    }
                }                
            }
        }
    }
}
