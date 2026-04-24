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
 * @file  Definitions.cs
 * @brief 
 *  Definitions of User Management module.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.08.27
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.27 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using Common;

namespace RMS.Generic.UserManagement
{
    /// <summary>   User authority.  </summary>
    /// <remarks>   Sungbok, Hong, 2014-08-28. </remarks>
    public class UserAuthority : NotifyPropertyChanged
    {
        /// <summary> The authorisation code </summary>
        private string m_AuthCode;

        /// <summary> Name of the authorisation </summary>
        private string m_AuthName;

        /// <summary>   Gets or sets the authorisation code. </summary>
        /// <value> The authorisation code. </value>
        public string AuthCode
        {
            get { return m_AuthCode; }
            set { m_AuthCode = value; }
        }

        /// <summary>   Gets or sets the name of the authorisation. </summary>
        /// <value> The name of the authorisation. </value>
        public string AuthName
        {
            get { return m_AuthName; }
            set { m_AuthName = value;}
        }
    }
}
