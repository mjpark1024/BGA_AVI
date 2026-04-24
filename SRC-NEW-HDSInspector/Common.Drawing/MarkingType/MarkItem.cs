using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using Common.Drawing.MarkingTypeUI;


namespace Common.Drawing.MarkingInformation
{
    /// <summary>   Inspection item.  </summary>
    /// <remarks>   suoow2, 2014-10-09. </remarks>
    [XmlInclude(typeof(RailCircleProperty))]
    [XmlInclude(typeof(RailRectProperty))]
    [XmlInclude(typeof(RailSpecialProperty))]
    [XmlInclude(typeof(RailTriProperty))]
    [XmlInclude(typeof(UnitCircleProperty))]
    [XmlInclude(typeof(UnitRectProperty))]
    [XmlInclude(typeof(UnitSpecialProperty))]
    [XmlInclude(typeof(UnitTriProperty))]
    [XmlInclude(typeof(NumberProperty))]
    [XmlInclude(typeof(WeekProperty))]

    public class MarkItem : NotifyPropertyChanged
    {
        [XmlElement(ElementName = "SequenceID")]
        public int ID
        {
            get
            {
                return m_nID;
            }
            set
            {
                m_nID = value;
                Notify("ID");
            }
        }
        private int m_nID = 0;

        [XmlIgnore]
        public int? SentID { get; set; }

        [XmlElement(ElementName = "Resolution")]
        public double Resolution
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Resolution")]


        [XmlElement(ElementName = "MarkType")]
        public MarkingType MarkType
        {
            get;
            set;
        }

        [XmlElement(ElementName = "MarkProperty")]
        public MarkInfo MarkInfo
        {
            get;
            set;
        }

        public MarkItem()
        {
        }

        public MarkItem(MarkingType markType, MarkInfo markInfo)
        {
            MarkType = markType;
            MarkInfo = markInfo.Clone();
        }

        public MarkItem(MarkingType markType, MarkInfo markInfo, int id)
        {
            m_nID = id;
            MarkType = markType;
            MarkInfo = markInfo;
        }

        //public int SaveMarkItem(string strMachineCode, string strModelCode, string strWorkType, string strRoiCode)
        //{
           // return MarkInfo.SaveProperty(strMachineCode, strModelCode, strWorkType, strRoiCode);
        //}

        //public MarkItem Clone()
        //{
        //    MarkItem mi = new MarkItem();
        //    mi.ID = this.ID;
        //    return mi;
        //}
    }


}
