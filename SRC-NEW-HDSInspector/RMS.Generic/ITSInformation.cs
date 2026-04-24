using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Generic
{
    public class ITSInfo : NotifyPropertyChanged
    {
        public bool UseITS
        {
            get
            {
                return m_bUseITS;
            }
            set
            {
                m_bUseITS = value;
                Notify("UseITS");
            }
        }

        public bool LeftID
        {
            get
            {
                return m_bLeftID;
            }
            set
            {
                m_bLeftID = value;
                Notify("LeftID");
            }
        }

        public bool InnerAOI
        {
            get
            {
                return m_bInnerAOI;
            }
            set
            {
                m_bInnerAOI = value;
                Notify("InnerAOI");
            }
        }

        public bool OuterAOI
        {
            get
            {
                return m_bOuterAOI;
            }
            set
            {
                m_bOuterAOI = value;
                Notify("OuterAOI");
            }
        }

        public bool BBT
        {
            get
            {
                return m_bBBT;
            }
            set
            {
                m_bBBT = value;
                Notify("BBT");
            }
        }
        public bool SKIPDATA
        {
            get
            {
                return m_Skipdata;
            }
            set
            {
                m_Skipdata = value;
                Notify("SKIPDATA");
            }
        }
        private bool m_bUseITS;
        private bool m_bLeftID;
        private bool m_bInnerAOI;
        private bool m_bOuterAOI;
        private bool m_bBBT;
        private bool m_Skipdata;

        public string ITSInfoString()
        {
            string val = "";
            val += (UseITS) ? "1" : "0";
            val += (LeftID) ? "1" : "0";
            val += (InnerAOI) ? "1" : "0";
            val += (OuterAOI) ? "1" : "0";
            val += (BBT) ? "1" : "0";
            val += (SKIPDATA) ? "1" : "0";
            return val;
        }

        public void Set(string strVal)
        {
            if (strVal.Length == 6)
            {
                UseITS = (strVal.Substring(0, 1) == "1");
                LeftID = (strVal.Substring(1, 1) == "1");
                InnerAOI = (strVal.Substring(2, 1) == "1");
                OuterAOI = (strVal.Substring(3, 1) == "1");
                BBT = (strVal.Substring(4, 1) == "1");
                SKIPDATA = (strVal.Substring(5, 1) == "1");
            }
            if (strVal.Length == 5)
            {
                UseITS = (strVal.Substring(0, 1) == "1");
                LeftID = (strVal.Substring(1, 1) == "1");
                InnerAOI = (strVal.Substring(2, 1) == "1");
                OuterAOI = (strVal.Substring(3, 1) == "1");
                BBT = (strVal.Substring(4, 1) == "1");
            }
        }

        public ITSInfo Clone()
        {
            ITSInfo its = new ITSInfo();
            its.UseITS = this.UseITS;
            its.LeftID = this.LeftID;
            its.InnerAOI = this.InnerAOI;
            its.OuterAOI = this.OuterAOI;
            its.BBT = this.BBT;
            its.SKIPDATA = this.SKIPDATA;
            return its;
        }
    }
}
