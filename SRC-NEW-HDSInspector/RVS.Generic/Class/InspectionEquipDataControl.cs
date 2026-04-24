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
using System.ComponentModel;

namespace RVS.Generic
{
    /// <summary>   Inspection equip data control.  </summary>
    [Serializable]
    public class InspectionEquipDataControl : NotifyPropertyChanged
    {
        /// <summary>   Gets or sets the name of the equip. </summary>
        /// <value> The name of the equip. </value>
        public string EquipName
        {
            get { return m_szEquipName; }
            set
            {
                m_szEquipName = value;
                Notify("EquipName");
            }
        }

        /// <summary>   Gets or sets the lot number. </summary>
        /// <value> The lot number. </value>
        public string LotNum
        {
            get { return m_szLotNum; }
            set
            {
                m_szLotNum = value;
                Notify("LotNum");
            }
        }

        /// <summary>   Gets or sets the before yield. </summary>
        /// <value> The before yield. </value>
        public string BeforeYield
        {
            get { return m_szBeforeYield; }
            set
            {
                m_szBeforeYield = value;
                Notify("BeforeYield");
            }
        }

        /// <summary>   Gets or sets the after yield. </summary>
        /// <value> The after yield. </value>
        public string AfterYield
        {
            get { return m_szAfterYield; }
            set
            {
                m_szAfterYield = value;
                Notify("AfterYield");
            }
        }

        /// <summary>   Gets or sets the model code. </summary>
        /// <value> The model code. </value>
        public string ModelName
        {
            get { return m_szModelName; }
            set
            {
                m_szModelName = value;
                Notify("ModelName");
            }
        }

        /// <summary>   Gets or sets the model code. </summary>
        /// <value> The model code. </value>
        public string GroupName
        {
            get { return m_szGroupName; }
            set
            {
                m_szGroupName = value;
                Notify("GroupName");
            }
        }

        public int FailStrip
        {
            get { return m_nFailStrip; }
            set
            {
                m_nFailStrip = value;
                Notify("FailStrip");
            }
        }

        public int UnitX
        {
            get { return m_nUnitX; }
            set
            {
                m_nUnitX = value;
                Notify("UnitX");
            }
        }
        public int UnitY
        {
            get { return m_nUnitY; }
            set
            {
                m_nUnitY = value;
                Notify("UnitY");
            }
        }

        #region Private member variables.

        private string m_szEquipName = "-";
        private string m_szLotNum = "-";
        private string m_szBeforeYield = "-";
        private string m_szAfterYield = "-";
        private string m_szModelName = "-";
        private string m_szGroupName = "-";
        private int m_nFailStrip = 0;
        private int m_nUnitX = 30;
        private int m_nUnitY = 6;

        #endregion
    }
}
