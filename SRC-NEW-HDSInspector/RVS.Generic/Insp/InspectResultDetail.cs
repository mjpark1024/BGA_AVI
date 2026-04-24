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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;
using Common.DataBase;
using RMS.Generic.UserManagement;

namespace RVS.Generic.Insp
{
    /// <summary>   Inspect result detail.  </summary>
    public class InspectResultDetail
    {
        /// <summary>   Gets or sets the roi code. </summary>
        /// <value> The roi code. </value>
        public string RoiCode
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the section code. </summary>
        /// <value> The section code. </value>
        public string SectionCode
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the type of the work. </summary>
        /// <value> The type of the work. </value>
        public string WorkType
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the strip col. </summary>
        /// <value> The strip col. </value>
        public int StripCol
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the strip row. </summary>
        /// <value> The strip row. </value>
        public int StripRow
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the unit row. </summary>
        /// <value> The unit row. </value>
        public int UnitRow
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the unit col. </summary>
        /// <value> The unit col. </value>
        public int UnitCol
        {
            get;
            set;
        }

        /// <summary>   Gets or sets a value indicating whether this object is defect. </summary>
        /// <value> true if this object is defect, false if not. </value>
        public bool IsDefect
        {
            get;
            set;
        }

        /// <summary>   Gets or sets a value indicating whether this object is verify. </summary>
        /// <value> true if this object is verify, false if not. </value>
        public bool IsVerify
        {
            get;
            set;
        }

        /// <summary>   Gets the list defect result. </summary>
        /// <value> The list defect result. </value>
        public ObservableCollection<DefectResult> listDefectResult
        {
            get
            {
                return m_listDefectResult;
            }
        }

        /// <summary> The list defect result </summary>
        private ObservableCollection<DefectResult> m_listDefectResult = new ObservableCollection<DefectResult>();
    }
}
