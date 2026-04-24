/*********************************************************************************
 * Copyright(c) 2011,2012,2013 by Samsung Techwin.
 * 
 * This software is copyrighted by, and is the sole property of Samsung Techwin.
 * All rigths, title, ownership, or other interests in the software remain the
 * property of Samsung Techwin. This software may only be used in accordance with
 * the corresponding license agreement. Any unauthorized use, duplication, 
 * transmission, distribution, or disclosure of this software is expressly 
 * forbidden.
 *
 * This Copyright notice may not be removed or modified without prior written
 * consent of Samsung Techwin.Samsung Techwin reserves the right to modify this 
 * software without notice.
 *
 * Samsung Techwin.
 * KOREA 
 * http://www.samsungtechwin.co.kr
 *********************************************************************************/
/**
 * @file  MachineInformation.cs
 * @brief 
 *  Machine Information class.
 * 
 * @author : Minseok Hwang <h.min-suck@samsung.com>
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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace RMS.Generic.MachineManagement
{
    // 2011-08-27, Minseok Hwang
    public class MachineInformation : INotifyPropertyChanged
    {
        #region Constructors.
        public MachineInformation()
       //     : this(string.Empty, string.Empty, string.Empty, true, DateTime.Now, string.Empty)
        {

        }

        public MachineInformation(BaseCode category, string code, string name, 
                                  bool isUse, DateTime registrationDate, string registrationID)
        {
            m_strCategory = category;
            m_strCode = code;
            m_strName = name;
            m_bIsUse = isUse;
            m_RegistrationDate = registrationDate;
            m_strRegistrationID = registrationID;
        }
        #endregion

        [XmlElement(ElementName = "Category")]
        public BaseCode Category
        {
            get
            {
                return m_strCategory;
            }
            set
            {
                m_strCategory = value;
                Notify("Category");
            }
        }

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
                Notify("Code");
            }
        }

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

        [XmlElement(ElementName = "IsUse")]
        public bool IsUse
        {
            get
            {
                return m_bIsUse;
            }
            set
            {
                m_bIsUse = value;
                Notify("IsUse");
            }
        }

        [XmlElement(ElementName = "IsSend")]
        public bool IsSend
        {
            get
            {
                return m_bIsSend;
            }
            set
            {
                m_bIsUse = value;
                Notify("IsSend");
            }
        }



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
        
        [XmlElement(ElementName = "RegistrationID")]
        public string RegistrationID
        {
            get
            {
                return m_strRegistrationID;
            }
            set
            {
                m_strRegistrationID = value;
                Notify("RegistrationID");
            }
        }

        #region implements INotifyPropertyChanged
        /// <summary> Event queue for all listeners interested in PropertyChanged events. </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>   Notifies. </summary>
        /// <remarks>   Minseok,Hwang, 2011-08-09. </remarks>
        /// <param name="strPropertyName">  Name of the property. </param>
        public void Notify(string strPropertyName)
        {
            PropertyChangedEventHandler p = PropertyChanged;
            if (p != null)
            {
                p(this, new PropertyChangedEventArgs(strPropertyName));
            }
        }
        #endregion

        #region Private member variables.
        private BaseCode m_strCategory = new BaseCode();
        private string m_strCode;
        private string m_strName;
        private bool m_bIsUse;
        private bool m_bIsSend;
        private DateTime m_RegistrationDate;
        private string m_strRegistrationID;
        #endregion
    }
}
