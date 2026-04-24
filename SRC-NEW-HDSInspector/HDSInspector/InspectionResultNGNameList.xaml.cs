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
using System.Windows.Controls;
using System.Windows.Input;
using Common;

namespace HDSInspector
{
    /// <summary>   Interaction logic for InspectionResultNGNameList.xaml. </summary>
    public partial class InspectionResultNGNameList : UserControl
    {
        /// <summary>   Initializes a new instance of the InspectionResultNGNameList class. </summary>
        public InspectionResultNGNameList()
        {
            InitializeComponent();
            lbNGCheckBoxList.PreviewMouseWheel += new MouseWheelEventHandler(lbNGCheckBoxList_PreviewMouseWheel);
        }

        /// <summary>
        /// Event handler. Called by lbNGCheckBoxList for preview mouse wheel events.
        /// </summary>
        private void lbNGCheckBoxList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double m_MouseScroll = this.NGContListScrollViewer.VerticalOffset;
            m_MouseScroll -= e.Delta / 8;

            // At Start Point
            if (m_MouseScroll < 0)
            {
                this.NGContListScrollViewer.ScrollToHome();
            }
            // At End Point
            else if (m_MouseScroll > this.NGContListScrollViewer.ScrollableHeight)
            {
                this.NGContListScrollViewer.ScrollToEnd();
            }
            // Middle
            else
            {
                this.NGContListScrollViewer.ScrollToVerticalOffset(m_MouseScroll);
            }
        }

        /// <summary>   Adds an item.  </summary>
        public void AddItem(NGCheckBoxListData aNGCheckBoxList)
        {
            this.lbNGCheckBoxList.Dispatcher.Invoke(new Action(delegate
            {
                lbNGCheckBoxList.Items.Add(aNGCheckBoxList);
            }));
        }

        /// <summary>   Clears this object to its blank/initial state. </summary>
        public void Clear()
        {
            this.lbNGCheckBoxList.Dispatcher.Invoke(new Action(delegate
            {
                if (lbNGCheckBoxList.Items.Count > 0)
                {
                    lbNGCheckBoxList.Items.Clear();
                }
            }));
        }
    }

    /// <summary>   Ng check box list data.  </summary>
    public class NGCheckBoxListData : NotifyPropertyChanged
    {
        /// <summary>   Gets or sets a value indicating whether this object is ng checked. </summary>
        /// <value> true if this object is ng checked, false if not. </value>
        public bool IsNGChecked
        {
            get 
            { 
                return m_bIsNGChecked; 
            }
            set 
            { 
                m_bIsNGChecked = value; 
                Notify("IsNGChecked"); 
            }
        }

        /// <summary>   Gets or sets the ng name 1. </summary>
        /// <value> The ng name 1. </value>
        public string NGName1
        {
            get 
            { 
                return m_strNGName1; 
            }
            set 
            { 
                m_strNGName1 = value; 
                Notify("NGName1"); 
            }
        }

        private bool m_bIsNGChecked = false;
        private string m_strNGName1 = "None";
    }
}
