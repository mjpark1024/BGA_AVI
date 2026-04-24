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
/**
 * @file  ModelInformation.cs
 * @brief 
 *  Definitions of model.
 * 
 * @author :  suoow2 <suoow.yeo@haesung.net>
 * @date : 2021.12.18
 * @version : 1.1
 * 
 * <b> Revision Histroy </b>
 * - 2021.12.18 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using RMS.Generic;
using PCS.ELF.AVI;

namespace PCS.ITS
{
    public class ITSModelInfo : ProfileNotifyChanged
    {
        public string ModelName
        {
            get
            {
                return m_sModelName;
            }
            set
            {
                m_sModelName = value;
                Notify("Name");
            }
        }

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
                return m_bSkipData;
            }
            set
            {
                m_bSkipData = value;
                Notify("SKIPDATA");
            }
        }

        public double Length
        {
            get
            {
                return m_dLength;
            }
            set
            {
                m_dLength = value;
                Notify("Length");
            }
        }

        public double Width
        {
            get
            {
                return m_dWidth;
            }
            set
            {
                m_dWidth = value;
                Notify("Width");
            }
        }

        public double PitchX
        {
            get
            {
                return m_dPitchX;
            }
            set
            {
                m_dPitchX = value;
                Notify("PitchX");
            }
        }

        public double PitchY
        {
            get
            {
                return m_dPitchY;
            }
            set
            {
                m_dPitchY = value;
                Notify("PitchY");
            }
        }

        public int UnitNumX
        {
            get
            {
                return m_nUnitNumX;
            }
            set
            {
                m_nUnitNumX = value;
                Notify("UnitNumX");
            }
        }

        public int UnitNumY
        {
            get
            {
                return m_nUnitNumY;
            }
            set
            {
                m_nUnitNumY = value;
                Notify("UnitNumY");
            }
        }

        private string m_sModelName;
        private bool m_bUseITS;
        private bool m_bLeftID;
        private bool m_bInnerAOI;
        private bool m_bOuterAOI;
        private bool m_bBBT;
        private bool m_bSkipData;
        private double m_dLength;
        private double m_dWidth;
        private double m_dPitchX;
        private double m_dPitchY;
        private int m_nUnitNumX;
        private int m_nUnitNumY;

    }
}
