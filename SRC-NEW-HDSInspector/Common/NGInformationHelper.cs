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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Common
{
    /// <summary>   Values that represent Surface.  </summary>
    public enum Surface
    {
        BP1 = 11,             // BondPad1
        BP2 = 12,             // BondPad2
        BP3 = 13,             // BondPad3
        BP4 = 14,             // BondPad4
        BP5 = 15,             // BondPad5
        BP6 = 16,             // BondPad6
        BP7 = 17,             // BondPad7
        BP8 = 18,             // BondPad8
        CA1 = 21,             // Top pattern surface1
        CA2 = 22,             // Top pattern surface2
        CA3 = 23,             // Top pattern surface3
        CA4 = 24,             // Top pattern surface4
        CA5 = 25,             // Top pattern surface5
        CA6 = 26,             // Top pattern surface6
        CA7 = 27,             // Top pattern surface7
        CA8 = 28,             // Top pattern surface8
        BA1 = 31,             // Bottom pattern surface1
        BA2 = 32,             // Bottom pattern surface2
        BA3 = 33,             // Bottom pattern surface3
        BA4 = 34,             // Bottom pattern surface4
        BA5 = 35,             // Bottom pattern surface5
        BA6 = 36,             // Bottom pattern surface6
        BA7 = 37,             // Bottom pattern surface7
        BA8 = 38              // Bottom pattern surface8
    }

    public enum CategorySurface
    {
        BP = 0,             // BondPad
        CA = 1,
        BA = 2,
    }

    public static class SurfaceEnum
    {
        private static readonly Dictionary<string, int> _Type = new Dictionary<string, int>();

        public static void Add(string key, int value)
        {
            _Type[key] = value;
        }

        public static int Get(string key)
        {
            return _Type.TryGetValue(key, out int value) ? value : throw new KeyNotFoundException($"Key '{key}' not found.");
        }

        public static void Remove(string key)
        {
            _Type.Remove(key);
        }

        public static IEnumerable<string> Keys => _Type.Keys;
        public static IEnumerable<int> Type => _Type.Values;
    }

    public class Bad_Info : NotifyPropertyChanged
    {
        private int m_id;                    //불량 인덱스 0~
        private List<int> m_code = new List<int>();                  //비전 검사의 Defect Code
        private string m_name;               //불량 명 
        private string m_mes_code;           //MES 코드
        private string m_mes_fail;           //MES 폐기코드
        private string m_map;                //불량 맵 표시
        private int m_group_id;                //불량 그룹 ID
        private string m_group_name;           //불량 그룹 명
        private string m_proc_code;               //불량 요인 공정 코드
        private string m_proc_name;            //불량 요인 공정 명
        private System.Windows.Media.Brush m_brush;
        private int m_priority;

        public Bad_Info Clone()
        {
            Bad_Info tmp = new Bad_Info();
            tmp.ID = this.ID;
            tmp.Name = this.Name;
            tmp.MES_Code = this.MES_Code;
            tmp.MES_Fail = this.MES_Fail;
            tmp.Map = this.Map;
            tmp.GroupID = this.GroupID;
            tmp.GroupName = this.GroupName;
            tmp.ProcCode = this.ProcCode;
            tmp.ProcName = this.ProcName;
            tmp.Color = this.Color;
            tmp.Priority = this.Priority;
            tmp.Code = new List<int>();
            for (int i= 0; i< Code.Count; i++)
            {
                tmp.Code.Add(this.Code[i]);
            }
            return tmp;
        }
        public int ID
        {
            get { return m_id; }
            set
            {
                m_id = value;
                Notify("ID");
            }
        }
        public List<int> Code
        {
            get { return m_code; }
            set
            {
                m_code = value;
                Notify("Code");
            }
        }
        public string Name
        {
            get { return m_name; }
            set
            {
                m_name = value;
                Notify("Name");
            }
        }
        public string MES_Code
        {
            get { return m_mes_code; }
            set
            {
                m_mes_code = value;
                Notify("MES_Code");
            }
        }
        public string MES_Fail
        {
            get { return m_mes_fail; }
            set
            {
                m_mes_fail = value;
                Notify("MES_Fail");
            }
        }

        public string Map
        {
            get { return m_map; }
            set
            {
                m_map = value;
                Notify("Map");
            }
        }

        public System.Windows.Media.Brush Color
        {
            get { return m_brush; }
            set { m_brush = value; Notify("Color"); }
        }

        public int GroupID
        {
            get { return m_group_id; }
            set
            {
                m_group_id = value;
                Notify("GroupID");
            }
        }

        public string GroupName
        {
            get { return m_group_name; }
            set
            {
                m_group_name = value;
                Notify("GroupName");
            }
        }

        public string ProcCode
        {
            get { return m_proc_code; }
            set
            {
                m_proc_code = value;
                Notify("ProcCode");
            }
        }

        public string ProcName
        {
            get { return m_proc_name; }
            set
            {
                m_proc_name = value;
                Notify("ProcName");
            }
        }

        public int Priority
        {
            get { return m_priority; }
            set
            {
                m_priority = value;
                Notify("Priority");
            }
        }

    }

    /// <summary>   Information about the NG.  </summary>
    public class NGInformationHelper
    {
        ObservableCollection<Bad_Info> Bad_Infos = new ObservableCollection<Bad_Info>();
        public int Size
        {
            get { return Bad_Infos.Count; }
        }
        public NGInformationHelper(ObservableCollection<Bad_Info> infos)
        {
            Bad_Infos = infos;
        }

        public NGInformationHelper()
        {

        }

        public NGInformationHelper Clone()
        {
            ObservableCollection<Bad_Info> bads = new ObservableCollection<Bad_Info>();
            for (int i=0; i< this.Size; i++)
            {
                Bad_Info bad = new Bad_Info();
                bad = this.Bad_Infos[i].Clone();
                bads.Add(bad);
            }
            NGInformationHelper tmp = new NGInformationHelper(bads);
            return tmp;
        }

        public int[] Priority
        {
            get
            {
                int[] tmp = new int[Size];
                for (int i = 0; i < Size; i++)
                    tmp[i] = Bad_Infos[i].Priority;
                return tmp;
            }
        }

        public void SetValue(int nID, int nPriority, System.Windows.Media.Brush color)
        {
            Bad_Infos[nID].Priority = nPriority;
            Bad_Infos[nID].Color = color;
        }

        public Bad_Info GetItem(int anIndex)
        {
            if (anIndex < 0 || anIndex >= Size) return null;
            return Bad_Infos[anIndex];
        }

        public string GetMapString(int anIndex)
        {
            if (anIndex < 0 || anIndex >= Size) return null;
            return Bad_Infos[anIndex].Map;
        }

        public string GetBadName(int anIndex)
        {
            if (anIndex < 0 || anIndex >= Size) return null;
            return Bad_Infos[anIndex].Name;
        }
        public int GetGroupID(int anIndex)
        {
            if (anIndex < 0 || anIndex >= Size) return 0;
            return Bad_Infos[anIndex].GroupID;
        }

        public int GetPriority(int anIndex)
        {
            if (anIndex < 0 || anIndex >= Size) return -1;
            return Bad_Infos[anIndex].Priority;
        }

        public Bad_Info ResultTypeToBadInfo(int anType)
        {
            Bad_Info bad = Bad_Infos.First(x => x.Code.Contains(anType));
            return bad;
        }

        public int BadNameToID(string astrBadName)
        {
            try
            {
                Bad_Info bad = Bad_Infos.First(x => x.Name == astrBadName);
                if (bad != null)
                    return bad.ID;
                else return -1;
            }
            catch
            {
                return 13;
            }
        }

        public int ResultTypeToID(int anResult)
        {
            try
            {
                //bgadb.result_info
                Bad_Info bad = Bad_Infos.First(x => x.Code.Contains(anResult));
                if (bad != null)
                    return bad.ID;
                else return -1;
            }
            catch
            {
                return -1;
            }

        }

        public string ResultTypeToName(int anResult)
        {
            try
            {
                Bad_Info bad = Bad_Infos.First(x => x.Code.Contains(anResult));
                if (bad != null)
                    return bad.Name;
                else return "";
            }
            catch
            {
                return "";
            }
        }

        public System.Windows.Media.Brush IDToColor(int anID)
        {
            Bad_Info bad = Bad_Infos.First(x => x.ID == anID);
            if (bad != null)
                return bad.Color;
            else return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
        }

        public string IDtoMesCode(int anID)
        {
            Bad_Info bad = Bad_Infos.First(x => x.ID == anID);
            if (bad != null)
                return bad.MES_Code;
            else return "";
        }

    }
}
