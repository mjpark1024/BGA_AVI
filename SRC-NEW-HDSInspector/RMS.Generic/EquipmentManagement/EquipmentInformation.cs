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
 * @file  EquipmentInformation.cs
 * @brief 
 *  Equipment Information class.
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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using Common;

namespace RMS.Generic.EquipmentManagement
{
    // 2014-08-27, suoow2
    public class EquipmentInformation : NotifyPropertyChanged
    {
        [XmlElement(ElementName = "EquipmentType")]
        public EquipmentType EquipmentType
        {
            get
            {
                return m_EquipmentType;
            }
            set
            {
                m_EquipmentType = value;
                Notify("EquipmentType");
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

        #region Private member variables.
        private EquipmentType m_EquipmentType = new EquipmentType();
        private string m_strCode;
        private string m_strName;
        private bool m_bIsUse;
        private DateTime m_RegistrationDate;
        private string m_strRegistrationID;
        #endregion
    }
}
