using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using Common;

namespace RMS.Generic.RejectManagement
{
    public class RejectInformation : NotifyPropertyChanged
    {
        #region Constructors.
        public RejectInformation()
            : this(string.Empty, 0.0, 0.0, true, DateTime.Now, string.Empty)
        {

        }

        public RejectInformation(string name, double stripRate, double UnitRate, 
                                  bool isUse, DateTime registrationDate, string registrationID)
        {
            m_fStripDefectRate = stripRate;
            m_fUnitDefectRate = UnitRate;
            m_strName = name;
            m_bIsUse = isUse;
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

        [XmlElement(ElementName = "StripDefectRate")]
        public double StripDefectRate
        {
            get
            {
                return m_fStripDefectRate;
            }
            set
            {
                m_fStripDefectRate = value;
                Notify("StripDefectRate");
            }
        }

        [XmlElement(ElementName = "UnitDefectRate")]
        public double UnitDefectRate
        {
            get
            {
                return m_fUnitDefectRate;
            }
            set
            {
                m_fUnitDefectRate = value;
                Notify("UnitDefectRate");
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
        private string m_strCode;
        private string m_strName;
        private double m_fStripDefectRate;
        private double m_fUnitDefectRate;
        private bool m_bIsUse;
        private DateTime m_RegistrationDate;
        private string m_strRegistrationID;
        #endregion
    }
}
