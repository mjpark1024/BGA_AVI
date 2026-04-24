using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RVS.Generic.Class
{
    public class NGCountData
    {
        private string m_NGName;
        private int m_NGCount;

        public string NGName
        {
            get { return m_NGName; }
            set { m_NGName = value; }
        }
        public int NGCount
        {
            get { return m_NGCount; }
            set { m_NGCount = value; }
        }
    }
}
