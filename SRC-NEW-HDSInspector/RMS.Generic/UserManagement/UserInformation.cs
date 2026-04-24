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
 * @file  UserInformation.cs
 * @brief 
 *  User Information class.
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
    /// <summary>   Information about the user.  </summary>
    /// <remarks>   suoow2, 2014-08-27. </remarks>
    public class UserInformation : NotifyPropertyChanged
    {
        #region Constructors.
        /// <summary>   Initializes a new instance of the UserInformation class. </summary>
        /// <remarks>   suoow2, 2014-09-09. </remarks>
        public UserInformation()
        {

        }

        public UserInformation(string id, string name, string password, UserAuthority authority, DateTime registrationDate)
        {
            m_strID = id;
            m_strName = name;
            m_strPassword = password;
            m_enumAuthority = authority;
            m_RegistrationDate = registrationDate;
        }
        #endregion

        /// <summary>   Gets or sets the identifier. </summary>
        /// <value> The identifier. </value>
        [XmlElement(ElementName = "ID")]
        public string ID
        {
            get
            {
                return m_strID;
            }
            set
            {
                m_strID = value;
                Notify("ID");
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

        /// <summary>   Gets or sets the password. </summary>
        /// <value> The password. </value>
        [XmlElement(ElementName = "Password")]
        public string Password
        {
            get
            {
                return m_strPassword;
            }
            set
            {
                m_strPassword = value;
                Notify("Password");
            }
        }

        /// <summary>   Gets or sets the authority. </summary>
        /// <value> The authority. </value>
        [XmlElement(ElementName = "Authority")]
        public UserAuthority Authority
        {
            get
            {
                return m_enumAuthority;
            }
            set
            {
                m_enumAuthority = value;
                Notify("Authority");
            }
        }

        /// <summary>   Gets or sets the date of the registration. </summary>
        /// <value> The date of the registration. </value>
        [XmlElement(ElementName = "RegistrationDate")]
        public DateTime RegistrationDate
        {
            get
            {
                return m_RegistrationDate;
            }
            set
            {
                m_RegistrationDate = value;
                Notify("RegistrationDate");
            }
        }

        #region Private member variables.
        private string m_strID;
        private string m_strName;
        private string m_strPassword;
        private UserAuthority m_enumAuthority = new UserAuthority();
        private DateTime m_RegistrationDate;
        #endregion
    }
}
