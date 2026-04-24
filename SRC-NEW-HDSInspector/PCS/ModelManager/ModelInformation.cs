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
 * @date : 2011.09.11
 * @version : 1.1
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.10 First creation.
 * - 2011.09.11 Re-definition.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using Common;
using Common.Drawing.MarkingInformation;
using RMS.Generic;
using System.ComponentModel;
using System.Xml;
using System.Runtime.CompilerServices;
using System.Net.NetworkInformation;
using System.Xml.Schema;
using System.IO;

namespace PCS.ELF.AVI
{
    /// <summary>   Information about the model.  </summary>
    /// <remarks>   suoow2, 2014-09-11. </remarks>
    public class ModelInformation : ProfileNotifyChanged
    {
        #region Constructors.
        /// <summary>   Initializes a new instance of the ModelSpec class. </summary>
        /// <remarks>   suoow2, 2014-08-11. </remarks>
        /// 
        public ModelInformation()
            : this(-1, string.Empty)
        {
        }

        /// <summary>   Initializes a new instance of the ModelSpec class. </summary>
        /// <remarks>   suoow2, 2014-08-11. </remarks>
        /// <param name="strName">  Name of the group. </param>
        public ModelInformation(string strName)
            : this(-1, strName)
        {
        }

        /// <summary>   Initializes a new instance of the ModelSpec class. </summary>
        /// <remarks>   suoow2, 2014-08-11. </remarks>
        /// <param name="nIndex">   Zero-based index of the group. </param>
        /// <param name="strName">  Name of the group. </param>
        public ModelInformation(int nIndex, string strName)
        {
            this.Index = nIndex;
            this.Name = strName;
        }
        #endregion
       
        #region Properties.
        /// <summary>   Gets or sets the identifier. </summary>
        /// <value> The identifier. </value>
        [XmlIgnore]
        public int Index
        {
            get
            {
                return m_nIndex;
            }
            set
            {
                m_nIndex = value;
                Notify("Index");
            }
        }

        /// <summary>   Gets or sets the description. </summary>
        /// <value> The description. </value>
        [XmlElement(ElementName = "Description")]
        public string Description
        {
            get
            {
                return m_strDescription;
            }
            set
            {
                m_strDescription = value;
            }
        }

        /// <summary>   Gets or sets the type. </summary>
        /// <value> The type. </value>
        [XmlElement(ElementName = "Type")]
        public Group Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
            }
        }

        /// <summary>   Gets or sets the ScanVelocity1. </summary>
        /// <value> The ScanVelocity1. </value>
        [XmlElement(ElementName = "ScanVelocity1")]
        public int ScanVelocity1
        {
            get
            {
                return m_nScanVelocity1;
            }
            set
            {
                m_nScanVelocity1 = value;
                Notify("ScanVelocity1");
            }
        }

        /// <summary>   Gets or sets the ScanVelocity2. </summary>
        /// <value> The ScanVelocity2. </value>
        [XmlElement(ElementName = "ScanVelocity2")]
        public int ScanVelocity2
        {
            get
            {
                return m_nScanVelocity2;
            }
            set
            {
                m_nScanVelocity2 = value;
                Notify("ScanVelocity2");
            }
        }

        /// <summary>   Gets or sets the XPitch. </summary>
        /// <value> The ScanVelocity2. </value>
        [XmlElement(ElementName = "XPitch")]
        public double XPitch
        {
            get
            {
                return m_dXPitch;
            }
            set
            {
                m_dXPitch = value;
                Notify("XPitch");
            }
        }

        /// <summary>   Gets or sets the ScanVelocity2. </summary>
        /// <value> The ScanVelocity2. </value>
        [XmlElement(ElementName = "InspectMode")]
        public int InspectMode
        {
            get
            {
                return m_nInspectMode;
            }
            set
            {
                m_nInspectMode = value;
                Notify("InspectMode");
            }
        }

        /// <summary>   Gets or sets a value indicating whether this object use marking. </summary>
        /// <value> true if use marking, false if not. </value>
        [XmlElement(ElementName = "UseConBad")]
        public bool UseConBad
        {
            get
            {
                return m_bUseConBad;
            }
            set
            {
                m_bUseConBad = value;
                Notify("UseConBad");
            }
        }

        /// <summary>   Gets or sets a value indicating whether this object use marking. </summary>
        /// <value> true if use marking, false if not. </value>
        [XmlElement(ElementName = "UseVerify")]
        public bool UseVerify
        {
            get
            {
                return m_bUseVerify;
            }
            set
            {
                if (m_bUseVerify == value) return;
                m_bUseVerify = value;
                if (value == true) { UseMarking = false; }
                Notify("UseVerify");
            }
        }

        /// <summary>   Gets or sets a value indicating whether this object use marking. </summary>
        /// <value> true if use marking, false if not. </value>
        [XmlElement(ElementName = "UseIDMark")]
        public bool UseIDMark
        {
            get
            {
                return m_bUseIDMark;
            }
            set
            {
                m_bUseIDMark = value;
                Notify("UseIDMark");
            }
        }

        /// <summary>   Gets or sets a value indicating whether this object use marking. </summary>
        /// <value> true if use marking, false if not. </value>
        [XmlElement(ElementName = "ConBad")]
        public int ConBad
        {
            get
            {
                return m_nConBad;
            }
            set
            {
                m_nConBad = value;
                Notify("ConBad");
            }
        }


        /// <summary>   Gets or sets a value indicating whether this object use marking. </summary>
        /// <value> true if use marking, false if not. </value>
        [XmlElement(ElementName = "UseBotSur1")]
        public bool UseBotSur1
        {
            get
            {
                return m_bUseBotSur1;
            }
            set
            {
                m_bUseBotSur1 = value;
                Notify("UseBotSur1");
            }
        }
        
        /// <summary>   Gets or sets a value indicating whether this object use marking. </summary>
        /// <value> true if use marking, false if not. </value>
        [XmlElement(ElementName = "UseBotSur2")]
        public bool UseBotSur2
        {
            get
            {
                return m_bUseBotSur2;
            }
            set
            {
                m_bUseBotSur2 = value;
                Notify("UseBotSur2");
            }
        }
        
        [XmlElement(ElementName = "UseBotSur3")]
        public bool UseBotSur3
        {
            get
            {
                return m_bUseBotSur3;
            }
            set
            {
                m_bUseBotSur3 = value;
                Notify("UseBotSur3");
            }
        }

        [XmlElement(ElementName = "UseBotSur4")]
        public bool UseBotSur4
        {
            get
            {
                return m_bUseBotSur4;
            }
            set
            {
                m_bUseBotSur4 = value;
                Notify("UseBotSur4");
            }
        }
        [XmlElement(ElementName = "UseBotSur5")]
        public bool UseBotSur5
        {
            get
            {
                return m_bUseBotSur5;
            }
            set
            {
                m_bUseBotSur5 = value;
                Notify("UseBotSur5");
            }
        }


        /// <summary>   Gets or sets a value indicating whether this object use marking. </summary>
        /// <value> true if use marking, false if not. </value>
        [XmlElement(ElementName = "UseBondPad1")]
        public bool UseBondPad1
        {
            get
            {
                return m_bUseBondPad1;
            }
            set
            {
                m_bUseBondPad1 = value;
                Notify("UseBondPad1");
            }
        }

        /// <summary>   Gets or sets a value indicating whether this object use marking. </summary>
        /// <value> true if use marking, false if not. </value>
        [XmlElement(ElementName = "UseBondPad2")]
        public bool UseBondPad2
        {
            get
            {
                return m_bUseBondPad2;
            }
            set
            {
                m_bUseBondPad2 = value;
                Notify("UseBondPad2");
            }
        }


        /// <summary>   Gets or sets a value indicating whether this object use marking. </summary>
        /// <value> true if use marking, false if not. </value>
        [XmlElement(ElementName = "UseTopSur1")]
        public bool UseTopSur1
        {
            get
            {
                return m_bUseTopSur1;
            }
            set
            {
                m_bUseTopSur1 = value;
                Notify("UseTopSur1");
            }
        }
        
        /// <summary>   Gets or sets a value indicating whether this object use marking. </summary>
        /// <value> true if use marking, false if not. </value>
        [XmlElement(ElementName = "UseTopSur2")]
        public bool UseTopSur2
        {
            get
            {
                return m_bUseTopSur2;
            }
            set
            {
                m_bUseTopSur2 = value;
                Notify("UseTopSur2");
            }
        }

        [XmlElement(ElementName = "UseTopSur3")]
        public bool UseTopSur3
        {
            get
            {
                return m_bUseTopSur3;
            }
            set
            {
                m_bUseTopSur3 = value;
                Notify("UseTopSur3");
            }
        }

        [XmlElement(ElementName = "UseTopSur4")]
        public bool UseTopSur4
        {
            get
            {
                return m_bUseTopSur4;
            }
            set
            {
                m_bUseTopSur4 = value;
                Notify("UseTopSur4");
            }
        }

        [XmlElement(ElementName = "UseTopSur5")]
        public bool UseTopSur5
        {
            get
            {
                return m_bUseTopSur5;
            }
            set
            {
                m_bUseTopSur5 = value;
                Notify("UseTopSur5");
            }
        }

        /// <summary>   Gets or sets the Auto NG Count. </summary>
        /// <value> The Auto NG Count. </value>
        [XmlElement(ElementName = "AutoNG")]
        public int AutoNG
        {
            get
            {
                return m_nAutoNG;
            }
            set
            {
                m_nAutoNG = value;
                Notify("AutoNG");
            }
        }

        [XmlElement(ElementName = "AutoNGBlock")]
        public int AutoNGBlock
        {
            get
            {
                return m_nAutoNGBlock;
            }
            set
            {
                m_nAutoNGBlock = value;
                Notify("AutoNGBlock");
            }
        }

        /// <summary>   Gets or sets the Auto NG X Unit Count. </summary>
        /// <value> The Auto NG X Unit Count. </value>
        [XmlElement(ElementName = "AutoNGX")]
        public int AutoNGX
        {
            get
            {
                return m_nAutoNGX;
            }
            set
            {
                m_nAutoNGX = value;
                Notify("AutoNGX");
            }
        }

        /// <summary>   Gets or sets the Auto Y Unit Count. </summary>
        /// <value> The Auto NG Y Unit Count. </value>
        [XmlElement(ElementName = "AutoNGY")]
        public int AutoNGY
        {
            get
            {
                return m_nAutoNGY;
            }
            set
            {
                m_nAutoNGY = value;
                Notify("AutoNGY");
            }
        }

        [XmlElement(ElementName = "AutoNGOuterY")]
        public int AutoNGOuterY
        {
            get
            {
                return m_nAutoNGOuterY;
            }
            set
            {
                m_nAutoNGOuterY = value;
                Notify("AutoNGOuterY");
            }
        }

        [XmlElement(ElementName = "AutoNGOuterYMode")]
        public int AutoNGOuterYMode
        {
            get
            {
                return m_nAutoNGOuterYMode;
            }
            set
            {
                m_nAutoNGOuterYMode = value;
                Notify("AutoNGOuterYMode");
            }
        }
        

        [XmlElement(ElementName = "AutoNGOuterDivY")]
        public int AutoNGOuterDivY
        {
            get
            {
                return m_nAutoNGOuterDivY;
            }
            set
            {
                m_nAutoNGOuterDivY = value;
                Notify("AutoNGOuterDivY");
            }
        }

        private int m_nSelectedList = 0;
        public int p_nSelectedList
        {
            get { return m_nSelectedList; }
            set { m_nSelectedList = value; }
        }

        [XmlElement(ElementName = "ListMatrix")]
        private ObservableCollection<string> m_ListMatrix = new ObservableCollection<string>();
        public ObservableCollection<string> p_ListMatrix
        {
            get
            {
                return m_ListMatrix;
            }
            set
            {
                m_ListMatrix = value;
                Notify("m_ListMatrix");
            }
        }

        public List<StructMatrix> AutoNGMatrixdata = new List<StructMatrix>();
        public void PassingAutoNGMatrixInfo(string value)
        {
            if (value == "") return;
            string[] MatrixString = value.Split('|');

            if(AutoNGMatrixdata.Count > 0) { AutoNGMatrixdata.Clear(); }
            if (p_ListMatrix.Count > 0) { p_ListMatrix.Clear(); }
            
            for (int i = 0; i< MatrixString.Length; i++)
            {
                string[] cooldinates = MatrixString[i].Split(',');
                int x = int.Parse(cooldinates[0]);
                int y = int.Parse(cooldinates[1]);

                AutoNGMatrixdata.Add(new StructMatrix(x, y, x.ToString() + ", " + y.ToString()));
                p_ListMatrix.Add(AutoNGMatrixdata[i].StringUI);
            }
        }
      
        public string AutoNGMatrixInfo = "";
        public void MergeAutoNGMatrixList()
        {
            string tempMatrixString = "";
            for (int i = 0; i < p_ListMatrix.Count; i++)
            {
                tempMatrixString += getRemoveWhiteSpaces(p_ListMatrix[i]);
                if(i < p_ListMatrix.Count - 1) tempMatrixString += "|";
            }
            AutoNGMatrixInfo = tempMatrixString;
        }
        
        public static string getRemoveWhiteSpaces(string str)
        {
            return string.Concat(str.Where(c => !Char.IsWhiteSpace(c)));
        }
        public int[,] LightValue = new int[10, 8];

        //[XmlElement(ElementName = "LightValue1")]
        //public int[] LightValue1
        //{
        //    get
        //    {
        //        return m_arrLightValue1;
        //    }
        //    set
        //    {
        //        m_arrLightValue1 = value;
        //        Notify("LightValue1");
        //    }
        //}

        //[XmlElement(ElementName = "LightValue2")]
        //public int[] LightValue2
        //{
        //    get
        //    {
        //        return m_arrLightValue2;
        //    }
        //    set
        //    {
        //        m_arrLightValue2 = value;
        //        Notify("LightValue2");
        //    }
        //}

        //[XmlElement(ElementName = "LightValue3")]
        //public int[] LightValue3
        //{
        //    get
        //    {
        //        return m_arrLightValue3;
        //    }
        //    set
        //    {
        //        m_arrLightValue3 = value;
        //        Notify("LightValue3");
        //    }
        //}
        //[XmlElement(ElementName = "LightValue4")]
        //public int[] LightValue4
        //{
        //    get
        //    {
        //        return m_arrLightValue4;
        //    }
        //    set
        //    {
        //        m_arrLightValue4 = value;
        //        Notify("LightValue4");
        //    }
        //}
        //[XmlElement(ElementName = "LightValue5")]
        //public int[] LightValue5
        //{
        //    get
        //    {
        //        return m_arrLightValue5;
        //    }
        //    set
        //    {
        //        m_arrLightValue5 = value;
        //        Notify("LightValue5");
        //    }
        //}

        /// <summary>   Gets or sets the strip. </summary>
        /// <value> The strip. </value>
        [XmlElement(ElementName = "Strip")]
        public StripInformation Strip
        {
            get
            {
                return m_Strip;
            }
            set
            {
                m_Strip = value;
            }
        }

        /// <summary>   Gets or sets a value indicating whether this object use marking. </summary>
        /// <value> true if use marking, false if not. </value>
        [XmlElement(ElementName = "UseMarking")]
        public bool UseMarking
        {
            get
            {
                return m_bUseMarking;
            }
            set
            {
                m_bUseMarking = value;
                if (value == true) { UseAI = false; UseVerify = false; }
                Notify("UseMarking");
            }
        }

        /// <summary>   Gets or sets a value indicating whether this object use marking. </summary>
        /// <value> true if use marking, false if not. </value>
        [XmlElement(ElementName = "UseRailOption")]
        public bool UseRailOption
        {
            get
            {
                return m_bUseRailOption;
            }
            set
            {
                m_bUseRailOption = value;
                Notify("UseRailOption");
            }
        }

        /// <summary>   Gets or sets the marker. </summary>
        /// <value> The marker. </value>
        [XmlElement(ElementName = "Marker")]
        public MarkerInformaion Marker
        {
            get
            {
                return m_Marker;
            }
            set
            {
                m_Marker = value;
            }
        }


        /// <summary>   Gets or sets the reject. </summary>
        /// <value> The reject. </value>
        [XmlElement(ElementName = "Reject")]
        public Reject Reject
        {
            get
            {
                return m_Reject;
            }
            set
            {
                m_Reject = value;
            }
        }

        /// <summary>   Gets or sets the ITS. </summary>
        /// <value> The ITS. </value>
        [XmlElement(ElementName = "Reject")]
        public ITSInfo ITS
        {
            get
            {
                return m_ITS;
            }
            set
            {
                m_ITS = value;
            }
        }

        /// <summary>   Gets or sets the ScrabInfo. </summary>
        /// <value> The Scrab. </value>
        [XmlElement(ElementName = "ScrabInfo")]
        public bool[] ScrabInfo
        {
            get
            {
                return m_ScrabInfo;
            }
            set
            {
                m_ScrabInfo = value;
            }
        }

        List<bool>m_ListALLUseSur= new List<bool>();
        public List<bool> ListALLUseSur
        {
            get
            {
                return m_ListALLUseSur;
            }
            set
            {
                m_ListALLUseSur = value;
            }
        }
        [XmlElement(ElementName = "UseAI")]
        public bool UseAI
        {
            get
            {
                return m_bUseAI;
            }
            set
            {
                if (m_bUseAI == value) return;
                m_bUseAI = value;
                if (value == true) { ITS.UseITS = true; UseMarking = false; }
                Notify("UseAI");
            }
        }
        
        #endregion

        #region Private member variables.
        private int m_nIndex = -1;
        // private bool m_bHasGoldenModel;
        private string m_strDescription;
        private int m_nScanVelocity1;
        private int m_nScanVelocity2;
        private int m_nInspectMode;
        private int m_nAutoNG;
        private int m_nAutoNGBlock;
        private int m_nAutoNGX;
        private int m_nAutoNGY;
        private int m_nAutoNGOuterY;
        private int m_nAutoNGOuterDivY;
        private int m_nAutoNGOuterYMode;

        private int m_nConBad;

        private double m_dXPitch;

        private bool[] m_ScrabInfo     = new bool[100];
        //private int[] m_arrLightValue1 = new int[6];
        //private int[] m_arrLightValue2 = new int[6];
        //private int[] m_arrLightValue3 = new int[6];
        //private int[] m_arrLightValue4 = new int[6];
        //private int[] m_arrLightValue5 = new int[6];

        private Group m_Type = new Group();
        private StripInformation m_Strip = new StripInformation();

        private bool m_bUseMarking = false;
        private bool m_bUseRailOption = false;
        private bool m_bUseVerify = false;
        private bool m_bUseIDMark = false;
        private bool m_bUseConBad = false;
        private bool m_bUseBotSur1 = false;
        private bool m_bUseBotSur2 = false;
        private bool m_bUseBotSur3 = false;
        private bool m_bUseBotSur4 = false;
        private bool m_bUseBotSur5 = false;
        private bool m_bUseBondPad1 = false;
        private bool m_bUseBondPad2 = false;
        private bool m_bUseTopSur1 = false;
        private bool m_bUseTopSur2 = false;
        private bool m_bUseTopSur3 = false;
        private bool m_bUseTopSur4 = false;
        private bool m_bUseTopSur5 = false;
        private MarkerInformaion m_Marker = new MarkerInformaion();
        private Reject m_Reject = new Reject();
        private ITSInfo m_ITS = new ITSInfo();
        private bool m_bUseAI = false;
        #endregion

        public MarkParams CopyToMarkParams()
        {
            MarkParams param = new MarkParams();

            if (this.Marker.RailIrr)
                 param.filename = this.Name + "_1STEP" + ".mrk";
            else
                 param.filename = this.Name + ".mrk";
            param.Mode = this.InspectMode;
            param.Step = this.Strip.MarkStep;
            param.ZeroMark = this.Marker.ZeroMark;
            param.StepPitch = this.Strip.StepPitch;
            param.StepUnits = this.Strip.StepUnits;
            param.UnitRow = this.Strip.UnitRow;
            param.UnitColumn = this.Strip.UnitColumn;
            param.UnitWidth = this.Strip.UnitWidth;
            param.UnitHeight = this.Strip.UnitHeight;
            param.Block = this.Strip.Block;
            param.BlockGap = this.Strip.BlockGap;
            param.Unit = this.Marker.UnitMark;
            param.Rail = this.Marker.RailMark;
            param.RailIrr = this.Marker.RailIrr;
            param.FlipChip = this.Marker.FlipChip;
            param.Week = this.Marker.WeekMark;
            param.WeekLoc = this.Marker.WeekPos;
            param.Count = this.Marker.NumMark;
            param.CountLoc = (this.Marker.NumLeft) ? 0 : 1;
            param.CountRW = this.Marker.ReMark;
            param.ID_Count = this.Marker.ID_Count;
            param.ID_Text = this.Marker.ID_Text;
            param.WeekMarkType = this.Marker.WeekMarkType;

            return param;
        }

        public ModelMarkInfo CopyToMarkInfo()
        {
            ModelMarkInfo minfo = new ModelMarkInfo();
            minfo.ID = 0;
            minfo.ModelName = this.Name;
            minfo.Mode = this.InspectMode;
            minfo.RailIrr = this.Marker.RailIrr;
            minfo.FlipChip = this.Marker.FlipChip;
            minfo.Step = this.Strip.MarkStep;
            minfo.StepPitch = this.Strip.StepPitch;
            minfo.StepUnits = this.Strip.StepUnits;
            minfo.UnitRow = this.Strip.UnitRow;
            minfo.UnitColumn = this.Strip.UnitColumn;
            minfo.UnitWidth = this.Strip.UnitWidth;
            minfo.UnitHeight = this.Strip.UnitHeight;
            minfo.Block = this.Strip.Block;
            minfo.BlockGap = this.Strip.BlockGap;           

            minfo.Unit = this.Marker.UnitMark;
            minfo.Rail = this.Marker.RailMark;
            minfo.Week = this.Marker.WeekMark;
            minfo.WeekLoc = this.Marker.WeekPos;
            minfo.Count = this.Marker.NumMark;
            minfo.CountLoc = (this.Marker.NumLeft) ? 0 : 1;
            minfo.CountRW = this.Marker.ReMark;
            minfo.IDMark = this.Marker.IDMark;
            minfo.ID_Count = this.Marker.ID_Count;
            minfo.WeekMarkType = this.Marker.WeekMarkType;
            minfo.ID_Text = this.Marker.ID_Text;
            minfo.Pos = new System.Windows.Point(this.Marker.MarkStartPosX, this.Marker.MarkStartPosY);
            minfo.UPos = new System.Windows.Point(this.Marker.UMarkStartPosX, this.Marker.UMarkStartPosY);

            return minfo;
        }
    }
}


