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
 * @file  StripInformation.cs
 * @brief 
 *  Definitions of strip.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.09.11
 * @version : 1.1
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.10 First creation.
 * - 2011.09.11 Re-definition.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace PCS.ELF.AVI
{
    /// <summary>   Information about the strip.  </summary>
    /// <remarks>   suoow2, 2014-09-11. </remarks>
    public class StripInformation: ProfileNotifyChanged
    {
        /// <summary>   Gets or sets the width. </summary>
        /// <value> The width. </value>
        [XmlElement(ElementName = "Width")]
        public double Width // 스트립 가로 길이
        {
            get
            {
                return m_fWidth;
            }
            set
            {
                m_fWidth = value;
                Notify("Width");
            }
        }

        /// <summary>   Gets or sets the height. </summary>
        /// <value> The height. </value>
        [XmlElement(ElementName = "Height")]
        public double Height // 스트립 세로 길이
        {
            get
            {
                return m_fHeight;
            }
            set
            {
                m_fHeight = value;
                Notify("Height");
            }
        }

        /// <summary>   Gets or sets the thickness. </summary>
        /// <value> The thickness. </value>
        [XmlElement(ElementName = "Thickness")]
        public double Thickness // 스트립 세로 길이
        {
            get
            {
                return m_fThickness;
            }
            set
            {
                m_fThickness = value;
                Notify("Thickness");
            }

        }
        /// <summary>   Gets or sets the align. </summary>
        /// <value> The align. </value>
        [XmlElement(ElementName = "Align")]
        public double Align
        {
            get
            {
                return m_fAlign;
            }
            set
            {
                m_fAlign = value;
                Notify("Align");
            }
        }


        [XmlElement(ElementName = "UnitRow")]
        public int UnitRow // 유닛 행 수
        {
            get
            {
                return m_nUnitRow;
            }
            set
            {
                m_nUnitRow = value;
                Notify("UnitRow");
            }
        }

        [XmlElement(ElementName = "UnitColumn")]
        public int UnitColumn // 유닛 열 수
        {
            get
            {
                return m_nUnitColumn;
            }
            set
            {
                m_nUnitColumn = value;
                Notify("UnitColumn");
            }
        }

        /// <summary>   Gets or sets the width of the unit. </summary>
        /// <value> The width of the unit. </value>
        [XmlElement(ElementName = "UnitWidth")]
        public double UnitWidth // 유닛 가로 길이
        {
            get
            {
                return m_fUnitWidth;
            }
            set
            {
                m_fUnitWidth = value;
                Notify("UnitWidth");
            }
        }

        /// <summary>   Gets or sets the height of the unit. </summary>
        /// <value> The height of the unit. </value>
        [XmlElement(ElementName = "UnitHeight")]
        public double UnitHeight // 유닛 세로 길이
        {
            get
            {
                return m_fUnitHeight;
            }
            set
            {
                m_fUnitHeight = value;
                Notify("UnitHeight");
            }
        }

        [XmlElement(ElementName = "PSRMarginX")]
        public int PSRMarginX // 블록 행 수
        {
            get
            {
                return m_nPSRMarginX;
            }
            set
            {
                m_nPSRMarginX = value;
                Notify("PSRMarginX");
            }
        }

        [XmlElement(ElementName = "PSRMarginY")]
        public int PSRMarginY // 블록 행 수
        {
            get
            {
                return m_nPSRMarginY;
            }
            set
            {
                m_nPSRMarginY = value;
                Notify("PSRMarginY");
            }
        }

        [XmlElement(ElementName = "WPMarginX")]
        public int WPMarginX // 블록 행 수
        {
            get
            {
                return m_nWPMarginX;
            }
            set
            {
                m_nWPMarginX = value;
                Notify("WPMarginX");
            }
        }

        [XmlElement(ElementName = "WPMarginY")]
        public int WPMarginY // 블록 행 수
        {
            get
            {
                return m_nWPMarginY;
            }
            set
            {
                m_nWPMarginY = value;
                Notify("WPMarginY");
            }
        }

        /// <summary>   Gets or sets the block count. </summary>
        /// <value> The block row. </value>
        [XmlElement(ElementName = "Block")]
        public int Block // 블록 행 수
        {
            get
            {
                return m_nBlock;
            }
            set
            {
                m_nBlock = value;
                Notify("Block");
            }
        }

        /// <summary>   Gets or sets the gap of the block. </summary>
        /// <value> The Gap of the block. </value>
        [XmlElement(ElementName = "BlockGap")]
        public double BlockGap
        {
            get
            {
                return m_fBlockGap;
            }
            set
            {
                m_fBlockGap = value;
                Notify("BlockGap");
            }
        }

        [XmlElement(ElementName = "AutoNGCnt")]
        public int AutoNGCnt // 유닛 열 수
        {
            get
            {
                return m_nAutoNGCnt;
            }
            set
            {
                m_nAutoNGCnt = value;
                Notify("AutoNGCnt");
            }
        }

        [XmlElement(ElementName = "MarkStep")]
        public int MarkStep // 유닛 열 수
        {
            get
            {
                return m_nMarkStep;
            }
            set
            {
                m_nMarkStep = value;
                Notify("MarkStep");
            }
        }

        [XmlElement(ElementName = "MarkShift")]
        public double MarkShift1
        {
            get
            {
                return m_MarkShift1;
            }
            set
            {
                m_MarkShift1 = value;
                Notify("MarkShift1");
            }
        }
        public double MarkShift2
        {
            get
            {
                return m_MarkShift2;
            }
            set
            {
                m_MarkShift2 = value;
                Notify("MarkShift2");
            }
        }        

        [XmlElement(ElementName = "StepUnits")]
        public int StepUnits // 유닛 열 수
        {
            get
            {
                return m_nStepUnits;
            }
            set
            {
                m_nStepUnits = value;
                Notify("StepUnits");
            }
        }

        /// <summary>   Gets or sets the gap of the block. </summary>
        /// <value> The Gap of the block. </value>
        [XmlElement(ElementName = "StepPitch")]
        public double StepPitch
        {
            get
            {
                return m_fStepPitch;
            }
            set
            {
                m_fStepPitch = value;
                Notify("StepPitch");
            }
        }       

        #region Private member variables.
        private double m_fWidth;
        private double m_fHeight;
        private double m_fThickness;
        private double m_fAlign;

        private int m_nUnitRow;
        private int m_nUnitColumn;

        private double m_fUnitWidth;
        private double m_fUnitHeight;

        private int m_nPSRMarginX;
        private int m_nPSRMarginY;
        private int m_nWPMarginX;
        private int m_nWPMarginY;

        private int m_nBlock;
        private double m_fBlockGap;

        private int m_nAutoNGCnt;
        private int m_nMarkStep;
        private int m_nStepUnits;
        private double m_fStepPitch;
        private double m_MarkShift1;
        private double m_MarkShift2;
        #endregion

        /// <summary>   Makes a deep copy of this object. </summary>
        /// <remarks>   suoow2, 2014-09-14. </remarks>
        /// <returns>   A copy of this object. </returns>
        public StripInformation Clone()
        {
            StripInformation strip = new StripInformation();

            strip.Code = this.Code;
            strip.Name = this.Name;
            strip.m_fWidth = this.m_fWidth;
            strip.m_fHeight = this.m_fHeight;
            strip.m_fThickness = this.m_fThickness;
            strip.m_fAlign = this.m_fAlign;
            strip.m_nUnitRow = this.m_nUnitRow;
            strip.m_nUnitColumn = this.m_nUnitColumn;
            strip.m_fUnitWidth = this.m_fUnitWidth;
            strip.m_fUnitHeight = this.m_fUnitHeight;
            strip.m_nBlock = this.m_nBlock;
            strip.m_fBlockGap = this.m_fBlockGap;
            strip.m_fStepPitch = this.m_fStepPitch;
            strip.m_nMarkStep = this.m_nMarkStep;
            strip.m_nStepUnits = this.m_nStepUnits;
            strip.m_nAutoNGCnt = this.m_nAutoNGCnt;
            strip.m_nPSRMarginX = this.m_nPSRMarginX;
            strip.m_nPSRMarginY = this.m_nPSRMarginY;
            strip.m_nWPMarginX = this.m_nWPMarginX;
            strip.m_nWPMarginY = this.m_nWPMarginY;
            strip.m_MarkShift1 = this.m_MarkShift1;
            strip.m_MarkShift2 = this.m_MarkShift2;
            return strip;
        }
    }

    /// <summary>   Information about the Laser Mark.  </summary>
    /// <remarks>   suoow2, 2014-09-11. </remarks>
    public class LaserInfomation : ProfileNotifyChanged
    {
        [XmlElement(ElementName = "Step")]
        public int Step // 유닛 행 수
        {
            get
            {
                return m_nStep;
            }
            set
            {
                m_nStep = value;
                Notify("Step");
            }
        }

        [XmlElement(ElementName = "StepUnits")]
        public int StepUnits // 유닛 열 수
        {
            get
            {
                return m_nStepUnits;
            }
            set
            {
                m_nStepUnits = value;
                Notify("StepUnits");
            }
        }

        [XmlElement(ElementName = "StepPitch")]
        public double StepPitch // 유닛 가로 길이
        {
            get
            {
                return m_fStepPitch;
            }
            set
            {
                m_fStepPitch = value;
                Notify("StepPitch");
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

        #region Private member variables.
        private int m_nStep;
        private int m_nStepUnits;
        private double m_fStepPitch;
        private bool m_bZeroMark;
        private bool m_bRailIrr;
        private bool m_bFlipChip;
        public System.Windows.Point MarkStartPos;
        public System.Windows.Point UMarkStartPos;

        public double BoatAngle;
        public double LaserAngle;

        public int Unit;
        public int Rail;
        public int Week;
        public int WeekLoc;
        public int Count;
        public int CountLoc;
        public int CountRW;

        #endregion

        /// <summary>   Makes a deep copy of this object. </summary>
        /// <remarks>   suoow2, 2014-09-14. </remarks>
        /// <returns>   A copy of this object. </returns>
        public LaserInfomation Clone()
        {
            LaserInfomation mark = new LaserInfomation();
            mark.m_nStep = this.m_nStep;
            mark.m_nStepUnits = this.m_nStepUnits;
            mark.m_fStepPitch = this.m_fStepPitch;
            mark.RailIrr = this.RailIrr;
            mark.FlipChip = this.FlipChip;
            mark.Unit = this.Unit;
            mark.Rail = this.Rail;
            mark.Week = this.Week;
            mark.WeekLoc = this.WeekLoc;
            mark.Count = this.Count;
            mark.CountLoc = this.CountLoc;
            mark.CountRW = this.CountRW;      
            mark.MarkStartPos = new System.Windows.Point(this.MarkStartPos.X, this.MarkStartPos.Y);
            mark.UMarkStartPos = new System.Windows.Point(this.UMarkStartPos.X, this.UMarkStartPos.Y);
            return mark;
        }
    }
}


