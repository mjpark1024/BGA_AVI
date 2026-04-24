using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RMS.Generic.InspectManagement;
using Common;

namespace RMS.Generic.NGInformationManagement
{
    public class NGInformation : NotifyPropertyChanged
    {
        #region Constructors.
        public NGInformation()
        {

        }

        public NGInformation(string name, InspectTypeInformation aType, BaseCode group, 
                                  bool isUse, bool isAutoNG, DateTime registrationDate, string registrationID)
        {
            m_strType = aType;
            m_Group = group;
            m_strName = name;
            m_bIsUse = isUse;
            m_bIsAutoNG = isAutoNG;
            m_RegistrationDate = registrationDate;
            m_strRegistrationID = registrationID;
        }
        #endregion

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

        [XmlElement(ElementName = "Type")]
        public InspectTypeInformation Type
        {
            get
            {
                return m_strType;
            }
            set
            {
                m_strType = value;
                Notify("Type");
            }
        }

        [XmlElement(ElementName = "Group")]
        public BaseCode Group
        {
            get
            {
                return m_Group;
            }
            set
            {
                m_Group = value;
                Notify("Group");
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

        [XmlElement(ElementName = "IsAutoNG")]
        public bool IsAutoNG
        {
            get
            {
                return m_bIsAutoNG;
            }
            set
            {
                m_bIsAutoNG = value;
                Notify("IsAutoNG");
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
        private string m_strCode;
        private string m_strName;
        private InspectTypeInformation m_strType = new InspectTypeInformation();
        private BaseCode m_Group = new BaseCode();        
        private bool m_bIsUse;
        private DateTime m_RegistrationDate;
        private string m_strRegistrationID;
        private bool m_bIsAutoNG = false;
        #endregion
    }
}
