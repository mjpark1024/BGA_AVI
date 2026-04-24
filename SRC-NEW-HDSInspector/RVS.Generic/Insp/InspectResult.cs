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
using System.Collections.ObjectModel;

namespace RVS.Generic.Insp
{
    /// <summary>   Inspect result.  </summary>
    public class InspectResult
    {
        public string ModelCode { get; set; }
        public string ResultCode { get; set; }
        
        public int StripID { get; set; }
        public int SlotNum { get; set; }
        public int StripDfectCount { get; set; }
        public int UnitDefectCount { get; set; }
        public int StripInspCount { get; set; }
        public int UnitInspCount { get; set; }

        public bool IsReject { get; set; }
        public bool IsDefect { get; set; }
        public bool IsVerify { get; set; }
        public bool IsFornt { get; set; }
        public bool IsSend { get; set; }
        public bool IsClosed { get; set; }
        public bool IsAutoNG { get; set; }

        public LotInformation LotInfo
        {
            get
            {
                return m_LotInfo;
            }
            set
            {
                m_LotInfo = value;
            }
        }
        private LotInformation m_LotInfo = new LotInformation();

        public DateTime StartTime
        {
            get
            {
                return m_StartTime;
            }
            set
            {
                m_StartTime = value;
            }
        }
        private DateTime m_StartTime = DateTime.Now;

        public DateTime EndTime
        {
            get
            {
                return m_EndTime;
            }
            set
            {
                m_EndTime = value;
            }
        }
        private DateTime m_EndTime = DateTime.Now;

        public ObservableCollection<InspectResultDetail> listInspectResultDetail
        {
            get
            {
                return m_listInspectResultDetail;
            }
            set
            {
                m_listInspectResultDetail = value;
            }
        }
        private ObservableCollection<InspectResultDetail> m_listInspectResultDetail = new ObservableCollection<InspectResultDetail>();
    }
}
