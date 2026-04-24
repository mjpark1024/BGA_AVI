using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

using RMS.Generic;
using PCS.ELF.AVI;


namespace PCS
{
    public class LotInformation
    {
        private string m_strLotNo;
        private ModelInformation m_ModelInfo = new ModelInformation();
        private BaseCode m_WorkType = new BaseCode();

        private int m_SheetCount;
        private int m_StripCount;
        private int m_UnitCount;
        private DateTime m_StartTime;
        private DateTime m_EndTime;
        private Reject m_RejectInfo = new Reject();
        private MarkerInformaion m_MarkerInfo = new MarkerInformaion();

        private DateTime m_RegDate;
       
        public string LotNo
        {
            get { return m_strLotNo; }
            set { m_strLotNo = value; }
        }

        public ModelInformation ModelInfo
        {
            get { return m_ModelInfo; }
            set { m_ModelInfo = value; }
        }

        public BaseCode WorkType
        {
            get { return m_WorkType; }
            set { m_WorkType = value; }
        }

        public int SheetCount
        {
            get { return m_SheetCount; }
            set { m_SheetCount = value; }
        }

        public int StripCount
        {
            get { return m_StripCount; }
            set { m_StripCount = value; }
        }

        public int UnitCount
        {
            get { return m_UnitCount; }
            set { m_UnitCount = value; }
        }

        public DateTime StartTime
        {
            get { return m_StartTime; }
            set { m_StartTime = value; }
        }

        public DateTime EndTime
        {
            get { return m_EndTime; }
            set { m_EndTime = value; }
        }
        
        public MarkerInformaion MarkInfo
        {
            get { return m_MarkerInfo; }
            set { m_MarkerInfo = value; }
        }

        public Reject RejectInfo
        {
            get { return m_RejectInfo; }
            set { m_RejectInfo = value; }
        }

        public DateTime RegDate
        {
            get { return m_RegDate; }
            set { m_RegDate = value; }
        }


        #region implements INotifyPropertyChanged
        /// <summary> Event queue for all listeners interested in PropertyChanged events. </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>   Notifies. </summary>
        /// <remarks>   Minseok,Hwang, 2011-08-09. </remarks>
        /// <param name="strPropertyName">  Name of the property. </param>
        public void Notify(string strPropertyName)
        {
            PropertyChangedEventHandler p = PropertyChanged;
            if (p != null)
            {
                p(this, new PropertyChangedEventArgs(strPropertyName));
            }
        }
        #endregion



    }
}
