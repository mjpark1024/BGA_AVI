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
 * @file  ModelBase.cs
 * @brief 
 *  Defines model base class.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.09.11
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.09 First creation.
 * - 2011.09.11 Re-structuring
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using Common;

namespace PCS.ELF.AVI
{
    /// <summary>   Model base.  </summary>
    /// <remarks>   suoow2, 2014-08-09. </remarks>
    public class ProfileNotifyChanged : NotifyPropertyChanged
    {
        /// <summary>   Gets or sets the name. </summary>
        /// <value> The name. </value>
        [XmlElement(ElementName = "Code")]
        public string Code
        {
            get
            {
                return m_strCode;
            }
            set
            {
                m_strCode = value;
            }
        }

        /// <summary>   Gets or sets the name. </summary>
        /// <value> The name. </value>
        [XmlElement(ElementName = "Name")]
        public string Name
        {
            get
            {
                return m_strName;
            }
            set
            {
                m_strName = value;
                Notify("Name");
            }
        }

        #region Private member variables.
        private string m_strCode = string.Empty;
        private string m_strName = string.Empty;
        #endregion
    }
}
