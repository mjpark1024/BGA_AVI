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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RVS
{
    /// <summary>   Values that represent RVSMenuType.  </summary>
    public enum RVSMenuType
    {
        VERIFYMODE = 0,
        SETTINGS = 1,
        ABOUT = 2,
        NGIMAGE = 3,
        EXIT = 4
    }
    /// <summary>   Delegate for handling MenuItemChange events. </summary>
    /// <param name="selectedMenu"> The selected menu. </param>
    public delegate void MenuItemChangeEventHandler(RVSMenuType selectedMenu);

    /// <summary>   Main menu.  </summary>
    public partial class MainMenu : UserControl
    {
        /// <summary> Event queue for all listeners interested in MenuItemChange events. </summary>
        public static event MenuItemChangeEventHandler MenuItemChangeEvent;

        /// <summary>   Initializes a new instance of the MainMenu class. </summary>
        public MainMenu()
        {
            InitializeComponent();
        }

        /// <summary>   Menu item select. </summary>
        /// <param name="strTag">   The string tag. </param>
        private void MenuItemSelect(string strTag)
        {
            if (string.IsNullOrEmpty(strTag))
            {
                return;
            }

            MenuItemChangeEventHandler eventRunner = MenuItemChangeEvent;
            if (eventRunner == null)
            {
                return;
            }

            // switch by tag
            
        }

        /// <summary>   Event handler. Called by MenuItem for click events. </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;

            if (item != null)
            {
                MenuItemChangeEventHandler eventRunner = MenuItemChangeEvent;
                if (eventRunner == null)
                    return;
                
                switch (item.Name)
                {
                    case "Settings":
                        eventRunner(RVSMenuType.SETTINGS);
                        break;
                    case "About":
                        eventRunner(RVSMenuType.ABOUT);
                        break;
                    case "NGImage":
                        eventRunner(RVSMenuType.NGIMAGE);
                        break;
                    case "ExitApplication":
                        eventRunner(RVSMenuType.EXIT);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>   Event handler. Called by MenuItem for text input events. </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text composition event information. </param>
        private void MenuItem_TextInput(object sender, TextCompositionEventArgs e)
        {
            MenuItem item = sender as MenuItem;

            if (item != null)
            {
                MenuItemSelect(item.Name);
            }
        }
    }
}
