using Emgu.CV.CvEnum;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class RegistryReadWrite
    {
        RegistryKey m_Reg = null;
        public RegistryReadWrite(string Part = "Device")
        {
            m_Reg=  Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("HDSinspector").CreateSubKey(Part);
        }
        
        public void Write(string sSub, object obj)
        {
            if (m_Reg == null || obj == null) return;
            m_Reg.SetValue(sSub, obj);
        }

        public dynamic Read(string sSub, dynamic valueDefault)
        {
            if (m_Reg == null) return valueDefault;
            dynamic value = m_Reg.GetValue(sSub);
            if (value == null) return valueDefault;
            Type type = valueDefault.GetType();
            try
            {
                if (type == typeof(bool)) return (value.ToString() == true.ToString());
                if (type == typeof(int)) return Convert.ToInt32(value.ToString());
                if (type == typeof(long)) return Convert.ToInt64(value.ToString());
                if (type == typeof(double)) return Convert.ToDouble(value.ToString());
                if (type == typeof(string)) return value.ToString();
                return valueDefault;
            }
            catch (Exception) { return valueDefault; }
        }
    }
}
