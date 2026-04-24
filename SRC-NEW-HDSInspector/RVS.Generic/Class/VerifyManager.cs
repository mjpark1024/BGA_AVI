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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;
using Common.DataBase;
using Common;
using RMS.Generic.UserManagement;
using RVS.Generic.Insp;

namespace RVS.Generic.Class
{
    public enum VerifyKey
    {
        Good = 0,
        Bad = 1,
        Prev = 2
    }

    public class IntPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public IntPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class VerifyData
    {
        public int MCNo;
        public int StripID;
        public int[,] Map;
        public int X;
        public int Y;
        public VerifyData(int anMC, int anStrip, int[,] aMap, int anUnitNumX, int anUnitNumY)
        {
            MCNo = anMC;
            StripID = anStrip;
            Map = new int[anUnitNumX, anUnitNumY];
            Array.Copy(aMap, Map, aMap.Length);
            X = anUnitNumX;
            Y = anUnitNumY;
        }
        public VerifyData CopyTo()
        {
            VerifyData tmp = new VerifyData(MCNo, StripID, Map, X, Y);
            return tmp;
        }
    }

    public class Configs
    {
        public string InfoPath { get; set; }
        public string DataPath { get; set; }

        public string LastLot;
        public bool bEnd;
        public bool bBad;

    }
    public class MapInfo
    {
        public bool Scrab { get; set; }
        public int ID;
        public byte[,] Map;
        public MapInfo()
        {
            Scrab = false;
            ID = 0;
            Map = null;
        }
        public MapInfo(bool scrab, int id, int width, int height)
        {
            Scrab = scrab;
            ID = id;
            Map = new byte[width, height];
        }
    }
    
    /// <summary>   Manager for verifies.  </summary>
    public class VerifyManager
    {
        /// <summary>   Updates the closed. </summary>
        public int UpdateClosed(string strMachineCode, string strModelCode)//, string strResultCode)
        {
            int nRet = 0;
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("UPDATE inspect_result SET closed_yn = 1, reg_date = now(), user_id = '{0}' ", UserManager.CurrentUser.ID);
                strQuery += String.Format(" WHERE machine_code = '{0}' and model_code = '{1}'", strMachineCode, strModelCode);

                nRet = ConnectFactory.DBConnector().Execute(strQuery);
            }

            return nRet;
        }

        /// <summary>   Updates the verify. </summary>
        public int UpdateVerify(DBConnector aDBConnector, string aszMachineCode, string aszModelCode, string aszResultCode, string aszVerifyResultCode, bool aIsGood)
        {
            if (aDBConnector == null)
                return -1;
            
            int nRet = 0;
            
            aDBConnector.StartTrans();

            String strQuery = String.Empty;
            if (aIsGood)
            {
                strQuery = String.Format("UPDATE inspect_result SET strip_defectcount = 0, unit_defectcount= 0, defect_yn = 0, verify_yn = 1, closed_yn = 1, reg_date = now(), user_id = '{0}' ", UserManager.CurrentUser.ID);
                strQuery += String.Format(" WHERE machine_code = '{0}' and model_code = '{1}' and result_code = '{2}'",
                    aszMachineCode, aszModelCode, aszResultCode);

                nRet = aDBConnector.Execute(strQuery);
            }
            else
            {
                strQuery = String.Format("UPDATE inspect_result SET verify_yn = 1, closed_yn = 1, reg_date = now(), user_id = '{0}' ", UserManager.CurrentUser.ID);
                strQuery += String.Format(" WHERE machine_code = '{0}' and model_code = '{1}' and result_code = '{2}'",
                    aszMachineCode, aszModelCode, aszResultCode);

                nRet = aDBConnector.Execute(strQuery);
            }

            if (nRet > 0)
            {
                if (aIsGood)
                {
                    strQuery = String.Format("UPDATE inspect_result_detail SET defect_yn = 0, verify_yn = 1, reg_date = now()");
                    strQuery += String.Format(" WHERE machine_code = '{0}' and model_code = '{1}' and result_code = '{2}'",
                        aszMachineCode, aszModelCode, aszResultCode);

                    nRet = aDBConnector.Execute(strQuery);
                }
                else
                {
                    strQuery = String.Format("UPDATE inspect_result_detail SET verify_yn = 1, reg_date = now()");
                    strQuery += String.Format(" WHERE machine_code = '{0}' and model_code = '{1}' and result_code = '{2}'",
                        aszMachineCode, aszModelCode, aszResultCode);

                    nRet = aDBConnector.Execute(strQuery);
                }
            }

            if (nRet > 0 && !aIsGood)
            {
                strQuery = String.Format("UPDATE defect_result SET overkill_yn = 1, verify_defect_code = '{0}'", aszVerifyResultCode);
                strQuery += String.Format(" WHERE machine_code = '{0}' and result_code = '{1}' and model_code = '{2}'",
                    aszMachineCode, aszResultCode, aszModelCode);

                nRet = aDBConnector.Execute(strQuery);
            }
            else if (nRet > 0 && aIsGood)
            {
                strQuery = String.Format("UPDATE defect_result SET verify_defect_code = '{0}'", aszVerifyResultCode);
                strQuery += String.Format(" WHERE machine_code = '{0}' and result_code = '{1}' and model_code = '{2}'",
                    aszMachineCode, aszResultCode, aszModelCode);

                nRet = aDBConnector.Execute(strQuery);
            }

            if (nRet > 0)
            {
                aDBConnector.Commit();
            }
            else
            {
                aDBConnector.Rollback();
            }

            return nRet;
        }
    }

    public class OrderInfo : NotifyPropertyChanged
    {
        #region Constructors.
        /// <summary>   Initializes a new instance of the ModelSpec class. </summary>
        /// <remarks>   suoow2, 2014-08-11. </remarks>
        public OrderInfo()
        {
        }

        /// <summary>   Initializes a new instance of the ModelSpec class. </summary>
        /// <remarks>   suoow2, 2014-08-11. </remarks>
        /// <param name="nIndex">   Zero-based index of the group. </param>
        /// <param name="strName">  Name of the group. </param>
        public OrderInfo(int nIndex, string strName)
        {
            this.Index = nIndex;
            this.Name = strName;
        }
        #endregion

        //  #region Properties.
        /// <summary>   Gets or sets the identifier. </summary>
        /// <value> The identifier. </value>
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
        public string Name
        {
            get
            {
                return m_strName;
            }
            set
            {
                m_strName = value;
            }
        }

        /// <summary>   Gets or sets the description. </summary>
        /// <value> The description. </value>
        public string Model
        {
            get
            {
                return m_strModel;
            }
            set
            {
                m_strModel = value;
            }
        }

        public string Machine
        {
            get
            {
                return m_strMC;
            }
            set
            {
                m_strMC = value;
            }
        }

        /// <summary>   Gets or sets the description. </summary>
        /// <value> The description. </value>
        public string Status
        {
            get
            {
                return m_strStatus;
            }
            set
            {
                m_strStatus = value;
                Notify("Status");
            }
        }

        /// <summary>   Gets or sets the ScanVelocity1. </summary>
        /// <value> The ScanVelocity1. </value>
        public int InspStrip
        {
            get
            {
                return m_nInspStrip;
            }
            set
            {
                m_nInspStrip = value;
                Notify("InspStrip");
            }
        }

        public int VeriStrip
        {
            get
            {
                return m_nVeriStrip;
            }
            set
            {
                m_nVeriStrip = value;
                Notify("VeriStrip");
            }
        }

        public int StripID
        {
            get
            {
                return m_nStripID;
            }
            set
            {
                m_nStripID = value;
                Notify("StripID");
            }
        }

        /// <summary>   Gets or sets the ScanVelocity2. </summary>
        /// <value> The ScanVelocity2. </value>
        public double InspYield
        {
            get
            {
                return m_fInspYield;
            }
            set
            {
                m_fInspYield = value;
                Notify("InspYield");
            }
        }
        public double VeriYield
        {
            get
            {
                return m_fVeriYield;
            }
            set
            {
                m_fVeriYield = value;
                Notify("VeriYield");
            }
        }

        /// <summary>   Gets or sets the ScanVelocity2. </summary>
        /// <value> The ScanVelocity2. </value>
        public int TotalUnits
        {
            get
            {
                return m_nUnitNumX * m_nUnitNumY;
            }
        }

        /// <summary>   Gets or sets the ScanVelocity2. </summary>
        /// <value> The ScanVelocity2. </value>
        public int BadUnits
        {
            get
            {
                return TotalUnits - (int)((double)TotalUnits * (InspYield / 100));
            }
        }

        /// <summary>   Gets or sets the ScanVelocity2. </summary>
        /// <value> The ScanVelocity2. </value>
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

        /// <summary>   Gets or sets the ScanVelocity2. </summary>
        /// <value> The ScanVelocity2. </value>
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

        /// <summary>   Gets or sets the ScanVelocity2. </summary>
        /// <value> The ScanVelocity2. </value>
        public DateTime DoneTime
        {
            get
            {
                return m_DoneTime;
            }
            set
            {
                m_DoneTime = value;
                Notify("DoneTime");
            }
        }

        /// <summary>   Gets or sets the ScanVelocity2. </summary>
        /// <value> The ScanVelocity2. </value>
        public DateTime StartTime
        {
            get
            {
                return m_StartTime;
            }
            set
            {
                m_StartTime = value;
                Notify("StartTime");
            }
        }

        public string ImagePath;
        public string MapPath;
        public string VerifyPath;
        public int NGCount;
        #region Private member variables.
        private int m_nIndex = -1;
        private string m_strName;
        private string m_strModel;
        private string m_strStatus;
        private string m_strMC;
        private int m_nInspStrip;
        private int m_nVeriStrip;
        private double m_fInspYield;
        private double m_fVeriYield;
        private int m_nUnitNumX;
        private int m_nUnitNumY;
        private int m_nStripID;
        private DateTime m_DoneTime;
        private DateTime m_StartTime;
        #endregion
    }

    public class VerifyInfo : NotifyPropertyChanged
    {
        #region Constructors.
        /// <summary>   Initializes a new instance of the ModelSpec class. </summary>
        /// <remarks>   suoow2, 2014-08-11. </remarks>
        public VerifyInfo()
        {
        }

        /// <summary>   Initializes a new instance of the ModelSpec class. </summary>
        /// <remarks>   suoow2, 2014-08-11. </remarks>
        /// <param name="nIndex">   Zero-based index of the group. </param>
        /// <param name="strName">  Name of the group. </param>
        public VerifyInfo(string strName, int unitx, int unity, double yield)
        {
            this.Name = strName;
            this.m_TotalMap = new int[unitx, unity];
            m_nStrips = 0;
            m_fYield = yield;
            m_fRate = 0.0;
            m_nTotalUnits = 0;
            m_nBadUnits = 0;
            m_nFailStrips = 0;
            m_nAlign = 0;
            m_nRaw = 0;
            m_nBP = 0;
            m_nOpen = 0;
            m_nShort = 0;
            m_nBall = 0;
            m_nVH = 0;
            m_nPin = 0;
            m_nPSR = 0;
            m_nVia = 0;
            m_nCC = 0;
            m_nBurr = 0;
        }
        #endregion

        /// <summary>   Gets or sets the description. </summary>
        /// <value> The description. </value>
        public string Name
        {
            get
            {
                return m_strName;
            }
            set
            {
                m_strName = value;
            }
        }

        public double Yield
        {
            get
            {
                return m_fYield;
            }
            set
            {
                m_fYield = value;
                Notify("Yield");
            }
        }

        public double Rate
        {
            get
            {
                return m_fRate;
            }
            set
            {
                m_fRate = value;
                Notify("Rate");
            }
        }

        public int Strips
        {
            get
            {
                return m_nStrips;
            }
            set
            {
                m_nStrips = value;
                Notify("Strips");
            }
        }

        public int TotalUnit
        {
            get
            {
                return m_nTotalUnits;
            }
            set
            {
                m_nTotalUnits = value;
                Notify("TotalUnit");
            }
        }

        public int BadUnits
        {
            get
            {
                return m_nBadUnits;
            }
            set
            {
                m_nBadUnits = value;
                Notify("BadUnits");
            }
        }

        public int FailStrips
        {
            get
            {
                return m_nFailStrips;
            }
            set
            {
                m_nFailStrips = value;
                Notify("FailStrips");
            }
        }

        public int Align
        {
            get
            {
                return m_nAlign;
            }
            set
            {
                m_nAlign = value;
                Notify("Align");
            }
        }

        public int Raw
        {
            get
            {
                return m_nRaw;
            }
            set
            {
                m_nRaw = value;
                Notify("Raw");
            }
        }


        public int BP
        {
            get
            {
                return m_nBP;
            }
            set
            {
                m_nBP = value;
                Notify("BP");
            }
        }

        public int Open
        {
            get
            {
                return m_nOpen;
            }
            set
            {
                m_nOpen = value;
                Notify("Open");
            }
        }

        public int Short
        {
            get
            {
                return m_nShort;
            }
            set
            {
                m_nShort = value;
                Notify("Short");
            }
        }

        public int Ball
        {
            get
            {
                return m_nBall;
            }
            set
            {
                m_nBall = value;
                Notify("Ball");
            }
        }

        public int VH
        {
            get
            {
                return m_nVH;
            }
            set
            {
                m_nVH = value;
                Notify("VH");
            }
        }

        public int Pin
        {
            get
            {
                return m_nPin;
            }
            set
            {
                m_nPin = value;
                Notify("Pin");
            }
        }

        public int PSR
        {
            get
            {
                return m_nPSR;
            }
            set
            {
                m_nPSR = value;
                Notify("PSR");
            }
        }

        public int Via
        {
            get
            {
                return m_nVia;
            }
            set
            {
                m_nVia = value;
                Notify("Via");
            }
        }

        public int CC
        {
            get
            {
                return m_nCC;
            }
            set
            {
                m_nCC = value;
                Notify("CC");
            }
        }

        public int Burr
        {
            get
            {
                return m_nBurr;
            }
            set
            {
                m_nBurr = value;
                Notify("Burr");
            }
        }

        public int[,] TotalMap
        {
            get
            {
                return m_TotalMap;
            }
            set
            {
                m_TotalMap = value;
                Notify("TotalMap");
            }
        }

        #region Private member variables.
        private string m_strName;
        private double m_fYield;
        private double m_fRate;
        private int m_nStrips;
        private int m_nTotalUnits;
        private int m_nBadUnits;
        private int m_nFailStrips;
        private int m_nAlign;
        private int m_nRaw;
        private int m_nBP;
        private int m_nOpen;
        private int m_nShort;
        private int m_nBall;
        private int m_nVH;
        private int m_nPin;
        private int m_nPSR;
        private int m_nVia;
        private int m_nCC;
        private int m_nBurr;
        private int[,] m_TotalMap;
        #endregion
    }
}
