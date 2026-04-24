using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Common
{
    public class Parameters
    {
        private readonly XmlSetting m_XmlSetting;
        public XmlSetting GetXmlSetting()
        {
            return m_XmlSetting;
        }

        public Parameters(XmlSetting aXmlSetting)
        {
            if (aXmlSetting == null)
            {
                aXmlSetting = new XmlSetting();
                //Load();
            }

            m_XmlSetting = aXmlSetting;
        }

        public void Save(string work_type, string inspect_code,string roi_code,string param_code,int paramValue)
        {
            m_XmlSetting.SetSettingInt(work_type + "/" + inspect_code + "/" + roi_code, param_code, paramValue);

        }
    }
}
