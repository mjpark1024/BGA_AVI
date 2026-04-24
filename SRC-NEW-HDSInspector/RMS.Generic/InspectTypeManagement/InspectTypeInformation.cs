using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using Common;

namespace RMS.Generic.InspectManagement
{
    public class InspectTypeInformation : NotifyPropertyChanged
    {
        private string m_Code;
        private string m_Name;
        private bool m_bIsUse;
        private DateTime m_RegistrationDate;
        private string m_strRegistrationID;

        public string Code
        {
            get { return m_Code; }
            set { m_Code = value; Notify("Code"); }
        }

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; Notify("Name"); }
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
    }
}
