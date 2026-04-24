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
using System.Linq;
using System.Text;
using Common;

namespace RVS.Generic
{
    /// <summary>   Inspection common information control.  </summary>
    /// <remarks>   suoow2, 2014-11-16. </remarks>
    public class InspectionCommonInfoControl : NotifyPropertyChanged
    {
        /// <summary>   Gets or sets the time of the start. </summary>
        /// <value> The time of the start. </value>
        public DateTime StartTime
        {
            get { return m_StartTime; }
            set
            {
                m_StartTime = value;
                Notify("StartTime");
            }
        }

        /// <summary>   Gets or sets the time of the run. </summary>
        /// <value> The time of the run. </value>
        public TimeSpan RunTime
        {
            get { return m_RunTime; }
            set
            {
                m_RunTime = value;
                Notify("RunTime");
            }
        }

        /// <summary>   Gets or sets the name of the user. </summary>
        /// <value> The name of the user. </value>
        public string UserName
        {
            get { return m_strUserName; }
            set
            {
                m_strUserName = value;
                Notify("UserName");
            }
        }

        /// <summary>   Gets or sets the current equip name. </summary>
        /// <value> The name of the current equip. </value>
        public string CurrentEquipName
        {
            get { return m_strCurrentEquipName; }
            set
            {
                m_strCurrentEquipName = value;
                Notify("CurrentEquipName");
            }
        }

        /// <summary>   Gets or sets the lot number. </summary>
        /// <value> The lot number. </value>
        public string ModelName
        {
            get { return m_strModelName; }
            set
            {
                m_strModelName = value;
                Notify("ModelName");
            }
        }

        /// <summary>   Gets or sets the lot number. </summary>
        /// <value> The lot number. </value>
        public string LotNum
        {
            get { return m_strLotNum; }
            set
            {
                m_strLotNum = value;
                Notify("LotNum");
            }
        }
        
        /// <summary>   Gets or sets the before yield. </summary>
        /// <value> The before yield. </value>
        public string BeforeYield
        {
            get { return m_strBeforeYield; }
            set
            {
                m_strBeforeYield = value;
                Notify("BeforeYield");
            }
        }

        /// <summary>   Gets or sets the after yield. </summary>
        /// <value> The after yield. </value>
        public string AfterYield
        {
            get { return m_strAfterYield; }
            set
            {
                m_strAfterYield = value;
                Notify("AfterYield");
            }
        }

        /// <summary>   Gets or sets the number of connected equips. </summary>
        /// <value> The number of connected equips. </value>
        public int ConnectedEquipCnt
        {
            get { return m_nConnectedEquipCnt; }
            set
            {
                m_nConnectedEquipCnt = value;
                Notify("ConnectedEquipCnt");
            }
        }

        #region Private member variables.

        private string m_strUserName = "정보없음";
        private string m_strCurrentEquipName = "-";
        private string m_strModelName = "-";
        private string m_strLotNum = "-";
        private string m_strBeforeYield = "-";
        private string m_strAfterYield = "-";
        private int m_nConnectedEquipCnt = 0;
        private DateTime m_StartTime;
        private TimeSpan m_RunTime;

        #endregion
    }
}
