using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using Common.DataBase;
using System.Windows;
using Common;

namespace RMS.Generic
{
    /// <summary>   Marker informaion. f </summary>
    /// <remarks>   suoow2, 2014-09-26. </remarks>
    public class MarkerInformaion : NotifyPropertyChanged
    {
        /// <summary> The code </summary>
        public string Code = string.Empty; // CreateCodeByCheckbox 메서드를 통해 생성된다.

        #region Marker code parser.
        /// <summary>   Creates a code by checkbox. </summary>
        /// <remarks>   suoow2, 2014-09-26. </remarks>
        /// <returns>   Marker code string. </returns>
        public static string CreateCodeByCheckbox(MarkerInformaion info)
        {
            string val = "";
            val += info.UnitMark.ToString() + ";";
            val += info.RailMark.ToString() + ";";
            val += info.NumMark.ToString() + ";";
            val += info.ReMark.ToString() + ";";
            val += info.WeekMark.ToString() + ";";
            val += (info.RailIrr) ? "1;" : "0;";
            val += (info.FlipChip) ? "1;" : "0;";
            val += (info.NumLeft) ? "1;" : "0;";
            val += info.WeekPos.ToString() + ";";
            val += (info.ZeroMark) ? "1;" : "0;";
            val += info.IDMark.ToString() + ";";
            val += info.ID_Count.ToString() + ";";
            val += (info.ID_Text) ? "1;" : "0;";
            val += info.WeekMarkType.ToString() + ";";
            return val;
        }

        /// <summary>   Creates a bool value by code. </summary>
        /// <remarks>   suoow2, 2014-09-26. </remarks>
        /// <param name="strCode">  The string code. </param>
        public static MarkerInformaion CreateBoolValueByCode(string strCode, string mpos1, string mpos2, string pos1, string pos2, double boat1, double boat2, double Campos, int rate ,double boatAngle, double laserAngle)
        {
            int unitMark;
            int railMark;
            int numMark;
            int reMark;
            int weekMark;
            bool railIrr;
            bool flipChip;
            bool numLeft;
            int weekpos;
            bool zeroMark;
            int nIDMark;
            int nID_Count;
            bool bID_Text;
            int nWeekMarkType;
            string[] ar = strCode.Split(';');
            if (ar.Length >= 10)
            {
                try
                {
                    unitMark = Convert.ToInt32(ar[0]);
                    railMark = Convert.ToInt32(ar[1]);
                    numMark = Convert.ToInt32(ar[2]);
                    reMark = Convert.ToInt32(ar[3]);
                    weekMark = Convert.ToInt32(ar[4]);
                    railIrr = (ar[5]=="0") ? false : true;
                    flipChip = (ar[6] == "0") ? false : true;
                    numLeft = (ar[7] == "0") ? false : true;
                    weekpos = Convert.ToInt32(ar[8]);
                    zeroMark = (ar[9] == "0") ? false : true;
                    if (ar[13] == "") nWeekMarkType = 0;
                    else nWeekMarkType = Convert.ToInt32(ar[13]);

                    try
                    {
                        nIDMark = Convert.ToInt32(ar[10]);
                        nID_Count = Convert.ToInt32(ar[11]);
                        bID_Text = (ar[12] == "0") ? false : true;
                    }
                    catch { nIDMark = 0; nID_Count = 0; bID_Text = false; }
                }
                catch
                {
                    unitMark = 0;
                    railMark = 0;
                    numMark = 0;
                    reMark = 0;
                    weekMark = 0;
                    railIrr = false;
                    flipChip = false;
                    numLeft = true;
                    weekpos = 0;
                    zeroMark = false;
                    nIDMark = 0;
                    nID_Count = 0;
                    bID_Text = false;
                    nWeekMarkType = 0;
                }
            }
            else
            {
                unitMark = 0;
                railMark = 0;
                numMark = 0;
                reMark = 0;
                weekMark = 0;
                railIrr = false;
                flipChip = false;
                numLeft = true ;
                weekpos = 0;
                zeroMark = false;
                nIDMark = 0;
                nID_Count = 0;
                bID_Text = false;
                nWeekMarkType= 0;
            }

            int x1 = 0;
            int y1 = 0;
            int x2 = 0;
            int y2 = 0;
            if (pos1 != null && pos1.Length == 7)
            {
                string[] tmp = pos1.Split(',');
                if (tmp.Length == 2)
                {
                    x1 = Convert.ToInt32(tmp[0]);
                    y1 = Convert.ToInt32(tmp[1]);
                }
            }
            if (pos2 != null && pos2.Length == 7)
            {
                string[] tmp = pos2.Split(',');
                if (tmp.Length == 2)
                {
                    x2 = Convert.ToInt32(tmp[0]);
                    y2 = Convert.ToInt32(tmp[1]);
                }
            }

            double mx1 = 0;
            double my1 = 0;
            double mx2 = 0;
            double my2 = 0;
            if (mpos1 != null)
            {
                string[] tmp = mpos1.Split(',');
                if (tmp.Length >= 2)
                {
                    try
                    {
                        mx1 = Convert.ToDouble(tmp[0]);
                        my1 = Convert.ToDouble(tmp[1]);
                    }
                    catch { }
                }
            }
            if (mpos2 != null)
            {
                string[] tmp = mpos2.Split(',');
                if (tmp.Length >= 2)
                {
                    try
                    {
                        mx2 = Convert.ToDouble(tmp[0]);
                        my2 = Convert.ToDouble(tmp[1]);
                    }
                    catch { }
                }
            }

            return new MarkerInformaion()
            {
                Code = strCode,
                MarkStartPosX = mx1,
                MarkStartPosY = my1,
                UMarkStartPosX = mx2,
                UMarkStartPosY = my2,
                BoatAngle = boatAngle,
                LaserAngle = laserAngle,
                PosX1 = x1,
                PosY1 = y1,
                PosX2 = x2,
                PosY2 = y2,
                BoatPos1 = boat1,
                BoatPos2 = boat2,
                CamPosY = Campos,
                MatchRate = rate,
                UnitMark = unitMark,
                RailMark = railMark,
                NumMark = numMark,
                ReMark = reMark,
                WeekMark = weekMark,
                RailIrr = railIrr,
                FlipChip = flipChip,
                NumLeft = numLeft,
                WeekPos = weekpos,
                ZeroMark = zeroMark,
                IDMark = nIDMark,
                ID_Count = nID_Count,
                ID_Text = bID_Text,
                WeekMarkType = nWeekMarkType
            };
        }
        #endregion

        #region Properties.
        [XmlElement(ElementName = "UnitMark")]
        public int UnitMark // 0 Marking
        {
            get
            {
                return m_nUnitMark;
            }
            set
            {
                m_nUnitMark = value;
                Notify("UnitMark");
            }
        }

        [XmlElement(ElementName = "RailMark")]
        public int RailMark // 0 Marking
        {
            get
            {
                return m_nRailMark;
            }
            set
            {
                m_nRailMark = value;
                Notify("RailMark");
            }
        }

        [XmlElement(ElementName = "NumMark")]
        public int NumMark // 0 Marking
        {
            get
            {
                return m_nNumMark;
            }
            set
            {
                m_nNumMark = value;
                Notify("NumMark");
            }
        }

        [XmlElement(ElementName = "ReMark")]
        public int ReMark // 0 Marking
        {
            get
            {
                return m_nReMark;
            }
            set
            {
                m_nReMark = value;
                Notify("ReMark");
            }
        }

        [XmlElement(ElementName = "WeekMark")]
        public int WeekMark // 0 Marking
        {
            get
            {
                return m_nWeekMark;
            }
            set
            {
                m_nWeekMark = value;
                Notify("WeekMark");
            }
        }

        [XmlElement(ElementName = "RailIrr")]
        public bool RailIrr // 0 Marking
        {
            get
            {
                return m_bRailIrr;
            }
            set
            {
                m_bRailIrr = value;
                Notify("RailIrr");
            }
        }

        [XmlElement(ElementName = "FlipChip")]
        public bool FlipChip // 0 Marking
        {
            get
            {
                return m_bFlipChip;
            }
            set
            {
                m_bFlipChip = value;
                Notify("FlipChip");
            }
        }

        [XmlElement(ElementName = "NumLeft")]
        public bool NumLeft // 0 Marking
        {
            get
            {
                return m_bNumLeft;
            }
            set
            {
                m_bNumLeft = value;
                Notify("NumLeft");
            }
        }

        [XmlElement(ElementName = "WeekPos")]
        public int WeekPos // 0 Marking
        {
            get
            {
                return m_nWeekPos;
            }
            set
            {
                m_nWeekPos = value;
                Notify("WeekPos");
            }
        }

        [XmlElement(ElementName = "ZeroMark")]
        public bool ZeroMark // 0 Marking
        {
            get
            {
                return m_bZeroMark;
            }
            set
            {
                m_bZeroMark = value;
                Notify("ZeroMark");
            }
        }

        [XmlElement(ElementName = "IDMark")]
        public int IDMark // 0 Marking
        {
            get
            {
                return m_nIDMark;
            }
            set
            {
                m_nIDMark = value;
                Notify("IDMark");
            }
        }

        public int ID_Count;
        public bool ID_Text;

        #endregion
        public double MarkStartPosX;
        public double MarkStartPosY;
        public double UMarkStartPosX;
        public double UMarkStartPosY;
        public int PosX1;
        public int PosY1;
        public int PosX2;
        public int PosY2; 
        public double BoatPos1;      // align 마진
        public double BoatPos2;
        public double BoatAngle;      // align 마진
        public double LaserAngle;
        public double CamPosY;
        public int MatchRate;    // 일치율
        public int WeekMarkType;

        /// <summary>   Makes a deep copy of this object. </summary>
        /// <remarks>   suoow2, 2014-09-14. </remarks>
        /// <returns>   A copy of this object. </returns>
        public MarkerInformaion Clone()
        {
            MarkerInformaion marker = new MarkerInformaion();
            marker.MarkStartPosX = this.MarkStartPosX;
            marker.MarkStartPosY = this.MarkStartPosY;
            marker.UMarkStartPosX = this.UMarkStartPosX;
            marker.UMarkStartPosY = this.UMarkStartPosY;
            marker.PosX1 = this.PosX1;
            marker.PosY1 = this.PosY1;
            marker.PosX2 = this.PosX2;
            marker.PosY2 = this.PosY2;
            marker.BoatPos1 = this.BoatPos1;
            marker.BoatPos2 = this.BoatPos2;
            marker.BoatAngle = this.BoatAngle;
            marker.LaserAngle = this.LaserAngle;
            marker.CamPosY = this.CamPosY;
            marker.MatchRate = this.MatchRate;
            marker.UnitMark = this.UnitMark;
            marker.RailMark = this.RailMark;
            marker.NumMark = this.NumMark;
            marker.WeekMark = this.WeekMark;
            marker.ReMark = this.ReMark;
            marker.RailIrr = this.RailIrr;
            marker.FlipChip = this.FlipChip;
            marker.NumLeft = this.NumLeft;
            marker.m_nWeekPos = this.m_nWeekPos;
            marker.ZeroMark = this.ZeroMark;
            marker.IDMark = this.IDMark;
            marker.ID_Count = this.ID_Count;
            marker.ID_Text = this.ID_Text;
            marker.WeekMarkType = this.WeekMarkType;
            marker.PosX1 = this.PosX1;
            marker.PosY1 = this.PosY1;
            marker.PosX2 = this.PosX2;
            marker.PosY2 = this.PosY2;
            marker.BoatPos1 = this.BoatPos1;
            marker.BoatPos2 = this.BoatPos2;
            marker.BoatAngle = this.BoatAngle;
            marker.LaserAngle = this.LaserAngle;
            marker.CamPosY = this.CamPosY;
            marker.MatchRate = this.MatchRate;

            return marker;
        }

        #region Private Member variables.
        private int m_nUnitMark;
        private int m_nRailMark;
        private int m_nNumMark;
        private int m_nReMark;
        private int m_nWeekMark;
        private bool m_bRailIrr;
        private bool m_bFlipChip;
        private bool m_bNumLeft;
        private int m_nWeekPos;
        private bool m_bZeroMark;
        private int m_nIDMark;
        #endregion
    }
}
