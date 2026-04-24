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
using System.Xml;
using System.Xml.Serialization;
using RMS.Generic;

namespace RVS.Generic.Insp
{
    /// <summary>   Information about the lot.  </summary>
    public class LotInformation
    {
        /// <summary>   Gets or sets the lot no. </summary>
        /// <value> The lot no. </value>
        public string LotNo
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the model code. </summary>
        /// <value> The model code. </value>
        public string ModelCode
        {
            get;
            set;
        }

        #region Lot count.
        [DefaultValue(0)]
        public int LotCount
        {
            get;
            set;
        }

        [DefaultValue(0)]
        public int Load1Cnt
        {
            get;
            set;
        }

        [DefaultValue(0)]
        public int Load2Cnt
        {
            get;
            set;
        }

        [DefaultValue(0)]
        public int Load3Cnt
        {
            get;
            set;
        }

        [DefaultValue(0)]
        public int Load4Cnt
        {
            get;
            set;
        }

        [DefaultValue(0)]
        public int Load5Cnt
        {
            get;
            set;
        }
        #endregion

        /// <summary>   Gets or sets a value indicating whether this object use marking. </summary>
        /// <value> true if use marking, false if not. </value>
        [DefaultValue(false)]
        public bool UseMarking
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the marker code. </summary>
        /// <value> The marker code. </value>
        public string MarkerCode
        {
            get { return m_strMarkerCode; }
            set { m_strMarkerCode = value; }
        }
        private string m_strMarkerCode = "0000";

        /// <summary>   Gets or sets the reject code. </summary>
        /// <value> The reject code. </value>
        public string RejectCode
        {
            get { return m_strRejectCode; }
            set { m_strRejectCode = value; }
        }
        private string m_strRejectCode = "0000";

        /// <summary>   Gets or sets the time of the start. </summary>
        /// <value> The time of the start. </value>
        public DateTime StartTime
        {
            get { return m_StartTime; }
            set { m_StartTime = value; }
        }

        /// <summary>   Gets or sets the time of the end. </summary>
        /// <value> The time of the end. </value>
        public DateTime EndTime
        {
            get { return m_EndTime; }
            set { m_EndTime = value; }
        }

        /// <summary>   Gets or sets the date of the register. </summary>
        /// <value> The date of the register. </value>
        public DateTime RegDate
        {
            get { return m_RegDate; }
            set { m_RegDate = value; }
        }

        #region Private member variables.
        private DateTime m_StartTime = DateTime.Now;
        private DateTime m_EndTime = DateTime.Now;
        private DateTime m_RegDate = DateTime.Now;
        #endregion
    }

    /// <summary>   Lot work.  </summary>
    public class LotWork
    {
        public string LotNo
        {
            get;
            set;
        }

        public string WorkType
        {
            get;
            set;
        }

        public string ModelCode
        {
            get;
            set;
        }

        public int SheetInspCount
        {
            get;
            set;
        }

        public int StripInspCount
        {
            get;
            set;
        }

        public int UnitInspCount
        {
            get;
            set;
        }

        public int SheetDefectCount
        {
            get;
            set;
        }

        public int StripDefectCount
        {
            get;
            set;
        }

        public int UnitDefectCount
        {
            get;
            set;
        }
    }
}
