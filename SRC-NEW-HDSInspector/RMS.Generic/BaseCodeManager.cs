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
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using System.Data;
using Common;
using Common.DataBase;

namespace RMS.Generic
{
    /// <summary>   Base code.  </summary>
    public class BaseCode : NotifyPropertyChanged
    {
        /// <summary>   Gets or sets the code. </summary>
        /// <value> The code. </value>
        public string Code
        {
            get
            {
                return m_BaseCode;
            }
            set
            {
                m_BaseCode = value;
                Notify("Code");
            }
        }
        private string m_BaseCode;

        /// <summary>   Gets or sets the name. </summary>
        /// <value> The name. </value>
        public string Name
        {
            get
            {
                return m_BaseName;
            }
            set
            {
                m_BaseName = value;
                Notify("Name");
            }
        }
        private string m_BaseName;

        /// <summary>   Gets or sets the master code. </summary>
        /// <value> The master code. </value>
        public string MasterCode  // parent code
        {
            get
            {
                return m_MasterCode;
            }
            set
            {
                m_MasterCode = value;
                Notify("MasterCode");
            }
        }
        private string m_MasterCode;
    }
}
