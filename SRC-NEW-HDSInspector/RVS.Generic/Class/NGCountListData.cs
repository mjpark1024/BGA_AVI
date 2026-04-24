using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;

namespace HDSInspector
{
    public class NGCountListData : NotifyPropertyChanged
    {
        private string m_NGName1 = "None";
        private string m_NGName2 = "None";
        private int m_NGCnt1;
        private int m_NGCnt2;

        public string NGName1
        {
            get { return m_NGName1; }
            set
            {
                m_NGName1 = value;
                Notify("NGName1");
            }
        }

        public string NGName2
        {
            get { return m_NGName2; }
            set
            {
                m_NGName2 = value;
                Notify("NGName2");
            }
        }

        public int NGCnt1
        {
            get { return m_NGCnt1; }
            set
            {
                m_NGCnt1 = value;
                Notify("NGCnt1");
            }
        }

        public int NGCnt2
        {
            get { return m_NGCnt2; }
            set
            {
                m_NGCnt2 = value;
                Notify("NGCnt2");
            }
        }
    }
}
