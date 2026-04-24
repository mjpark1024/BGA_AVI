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

using OpenCvSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using Point = System.Windows.Point;

namespace Common
{
    public struct StructMatrix
    {
        public int V1 { get; set; }
        public int V2 { get; set; }
        public string StringUI { get; set; }
        public StructMatrix(int v1, int v2, string stringUI)
        {
            V1 = v1;
            V2 = v2;
            StringUI = stringUI;
        }
    }
    public struct MarkingBoatShift
    {
        public double Offset1;
        public double Offset2;

    }
    /// <summary>   Interface for setting.  </summary>
    /// <remarks>   suoow2, 2014-08-24. </remarks>
    public interface ISetting
    {
        bool IsValidate(); // Setting 값이 올바른지를 확인한다.
        bool CheckSave(); // Save가 필요한 경우인지 확인한다.
        void Save();
        void TrySave(); // 입력 값이 올바른 항목만 저장한다.
    }
    public class SaveQueue
    {
        public List<List<bool>> bWorkDone = new List<List<bool>>();
        public List<List<ConcurrentQueue<object>>> ImageSave_Queue = new List<List<ConcurrentQueue<object>>>();
        public List<List<ConcurrentQueue<object>>> ICS_ResultQueue = new List<List<ConcurrentQueue<object>>>();
    }
    /// <summary>   Settings.  </summary>
    public class Settings
    {
        #region Private member variables.
        private readonly static Settings _Instance = new Settings();
        private readonly static CommonPath m_Path = new CommonPath();
        private readonly static XmlSetting m_XmlSetting = new XmlSetting();
        private static SettingGeneral m_General;
        private static SettingSubSystem m_SubSystem;
        private static SettingDevice m_Device;
        private static SettingLog m_Log;
        private static SettingMTS m_MTS;
        //private static SettingMachines m_Machines; 
        #endregion

        /// <summary>   Gets the xml setting. </summary>
        /// <returns>   The xml setting. </returns>
        public XmlSetting GetXmlSetting()
        {
            return m_XmlSetting;
        }

        /// <summary>   Gets the common path. </summary>
        /// <returns>   The common path. </returns>
        public CommonPath GetCommonPath()
        {
            return m_Path;
        }

        public bool Load()
        {
            String xmlFile = GetCommonPath().GetConfigFileName();

            FileSupport.ForceDirectories(xmlFile);

            if (!m_XmlSetting.Initialize(xmlFile))
                return false;

            General.Load();
            Device.Load();
            SubSystem.Load();
            MTS.Load();

            return true;
        }

        public void Save()
        {
            General.Save();
            Device.Save();
            SubSystem.Save();
            MTS.Save();     
            m_XmlSetting.Flush();
        }

        public static Settings GetSettings()
        {
            return _Instance;
        }

        #region Properties.
        // general setting.
        public SettingGeneral General
        {
            get
            {
                if (m_General == null)
                {
                    m_General = new SettingGeneral(m_XmlSetting);
                }

                return m_General;
            }
            set
            {
                m_General = value;
            }
        }

        // subsystem setting.
        public SettingSubSystem SubSystem
        {
            get
            {
                if (m_SubSystem == null)
                {
                    m_SubSystem = new SettingSubSystem(m_XmlSetting);
                }

                return m_SubSystem;
            }
            set
            {
                m_SubSystem = value;
            }
        }

        // device setting.
        public SettingDevice Device
        {
            get
            {
                if (m_Device == null)
                {
                    m_Device = new SettingDevice(m_XmlSetting);
                }

                return m_Device;
            }
            set
            {
                m_Device = value;
            }
        }

        // log setting.
        public SettingLog Log
        {
            get
            {
                if (m_Log == null)
                {
                    m_Log = new SettingLog(m_XmlSetting);
                }

                return m_Log;
            }
            set
            {
                m_Log = value;
            }
        }

        // mts setting.
        public SettingMTS MTS
        {
            get
            {
                if (m_MTS == null)
                {
                    m_MTS = new SettingMTS(m_XmlSetting);
                }

                return m_MTS;
            }
            set
            {
                m_MTS = value;
            }
        }
        #endregion
    }

    public class InspectBuffer
    {
        int m_nSizeX;
        int m_nSizeY;
        int[,] m_badunit;
        int[,] m_buffer;
        int[,] m_dnn_buffer;
        int[,] m_PreSkipdatabuffer;

        public int[,] DnnUnitBuff;
        public List<Tuple<Point,int>> DnnImageBuff;

        public int SizeX { get { return m_nSizeX; } }
        public int SizeY { get { return m_nSizeY; } }
        public bool AutoNG;
        public bool OuterNG;
        public int[,] BadUnit
        {
            get { return m_badunit; }
            set { m_badunit = value; }
        }
        public int[,] Buffer
        {
            get { return m_buffer; }
        }

        public int[,] Dnn_Buffer
        {
            get { return m_dnn_buffer; }
        }

        public int[,] PreSkipdatabuffer
        {
            get { return m_PreSkipdatabuffer; }
        }

        public int ID;
        public string IDString { get; set; }
        public int[] PriorityNG;

        // 1) Outer, PSRShift의 경우 2D ID 결과 무시하고 폐기 == 전체 유닛 불량 처리
        // 2) 티칭에러 2D ID 결과 무시하고 폐기
        public bool IsNGStrip()
        {
            int cnt = 0;
            for (int i = 0; i < m_nSizeX; i++)
            {
                for (int j = 0; j < m_nSizeY; j++)
                {
                    if (m_buffer[i, j] != 0) cnt++;
                    if (m_buffer[i,j] == 47) return true;
                }
            }
            if (m_nSizeX * m_nSizeY == cnt) return true;
            else return false;
        }

        public bool IsAutoNGStrip(bool[] AutoNG_Check)
        {
            for (int i = 0; i < AutoNG_Check.Length; i++)
            {
                if (AutoNG_Check[i] && i != 12) //OuterNG 는 별도로 체크, 이부분 정리 필요, 이렇게되면 Dam 검출되면 AutoNG로 빠짐, 기존에도 동일 확인
                {
                    for (int j = 0; j < m_nSizeX; j++)
                    {
                        for (int z = 0; z < m_nSizeY; z++)
                        {
                            if (m_buffer[j, z] == i+1) return true;
                        }
                    }
                }
            }

            return false;
        }
   
        public int BadCount()
        {
            int cnt = 0;
            for (int i = 0; i < m_nSizeX; i++)
            {
                for (int j = 0; j < m_nSizeY; j++)
                {
                    if (m_buffer[i, j] != 0) cnt++;
                }
            }
            return cnt;
        }
        public InspectBuffer Clone()
        {
            InspectBuffer tmp = new InspectBuffer();
            tmp.Init(m_nSizeX, m_nSizeY, PriorityNG);
            tmp.ID = this.ID;
            tmp.IDString = this.IDString;
            tmp.AutoNG = this.AutoNG;
            Array.Copy(m_badunit, tmp.m_badunit, m_nSizeX * m_nSizeY);
            Array.Copy(m_buffer, tmp.m_buffer, m_nSizeX * m_nSizeY);
            Array.Copy(m_dnn_buffer, tmp.m_dnn_buffer, m_nSizeX * m_nSizeY);
            Array.Copy(m_PreSkipdatabuffer, tmp.m_PreSkipdatabuffer, m_nSizeX * m_nSizeY);
            return tmp;
        }
        public void CopyToPreSkipdata()
        {
            Array.Copy(m_buffer, m_PreSkipdatabuffer, m_nSizeX * m_nSizeY);   
        }
        public int CountAutoNG(int StripXOut, int XConbad, int YConbad, int BlockXOut, int BlockCount, int ShiftID, int StaticID, int OuterID, int AutoNGOuterY, int AutoNGOuterYMode, int AutoNGOuterDivY, List<StructMatrix> AutoNGMatrixInfo)
        {
            #region Strip XOut 초과 판단
            int cnt = 0;
            
            if (StripXOut > 0)
            {
                for (int i = 0; i < m_nSizeX; i++)
                {
                    for (int j = 0; j < m_nSizeY; j++)
                    {
                        if (m_buffer[i, j] != ShiftID && m_buffer[i,j] != 0)
                            if (m_buffer[i, j] != 0) cnt++;
                    }
                }
                if (StripXOut <= cnt) return 1;
            }
            #endregion

            #region 블록폐기
            if (BlockXOut > 0)
            {
                int size = m_nSizeX / BlockCount;
                for (int k = 0; k < BlockCount; k++)
                {
                    cnt = 0;
                    int s = size * k;
                    for (int i = s; i < s + size; i++)
                    {
                        for (int j = 0; j < m_nSizeY; j++)
                        {
                            if (m_buffer[i, j] != 0 && m_buffer[i, j] != ShiftID) cnt++;
                        }
                    }
                    if (cnt >= BlockXOut) return 2;
                }
            }
            #endregion

            #region 배열불량
            for (int z = 0; z < AutoNGMatrixInfo.Count; z++)
            {
                int nBlockCountX = 1;
                int nBlockCountY = 1;
                if (BlockCount > 0) nBlockCountX = BlockCount;
                //if (BlockCount > 0) nBlockCountY = 1;/////추후에 Block Y 가 추가되면 쓰자

                int DevideX = m_nSizeX / nBlockCountX;
                int DevideY = m_nSizeY / nBlockCountY;/////추후에 Block Y 가 추가되면 쓰자
                for (int x = 0; x < nBlockCountX; x++)
                {
                    int TempCols = DevideX * x;
                    for (int y = 0; y < nBlockCountY; y++)
                    {
                        int TempRows = DevideY * y;
                        for (int i = TempCols; i <= TempCols + DevideX - AutoNGMatrixInfo[z].V1; i++)
                        {
                            for (int j = TempRows; j <= TempRows + DevideY - AutoNGMatrixInfo[z].V2; j++)
                            {
                                bool found = false;
                                if (m_buffer[i, j] != 0)
                                {
                                    found = true;
                                    for (int k = 0; k < AutoNGMatrixInfo[z].V1; k++)
                                    {
                                        for (int l = 0; l < AutoNGMatrixInfo[z].V2; l++)
                                        {
                                            if (m_buffer[i + k, j + l] == 0)
                                            {
                                                found = false;
                                                break;
                                            }
                                        }
                                        if (!found) break;
                                    }
                                    if (!found) break;
                                }
                                if (found) return 2;
                            }
                        }
                    }
                }
            }
            #endregion

            #region 연속불량
            cnt = 0;
            if (XConbad > 0)
            {
                for (int i = 0; i < m_nSizeX; i++)
                {
                    cnt = 0;
                    for (int j = 0; j < m_nSizeY; j++)
                    {
                        if (m_buffer[i, j] != 0 && m_buffer[i, j] != ShiftID) cnt++;
                        else cnt = 0;
                        if (cnt >= XConbad) return 2;
                    }
                }
            }
            cnt = 0;
            if (YConbad > 0)
            {
                for (int i = 0; i < m_nSizeY; i++)
                {
                    cnt = 0;
                    for (int j = 0; j < m_nSizeX; j++)
                    {
                        if (m_buffer[j, i] != 0 && m_buffer[j, i] != ShiftID) cnt++;
                        else cnt = 0;
                        if (cnt >= YConbad) return 2;
                    }
                }
            }
            #endregion

            #region 외각 행 XOut 폐기
            //외각 행 폐기
            int cntXOutFirstY = 0;
            int cntXOutEndY = 0;
            for (int i = 0; i < m_nSizeY; i++) // 4
            {
                for (int j = 0; j < m_nSizeX; j++) // 30
                {
                    if (m_buffer[j, i] != 0 && i == 0 && AutoNGOuterYMode != 1) cntXOutFirstY++;
                    if (m_buffer[j, i] != 0 && i == m_nSizeY - 1 && AutoNGOuterYMode != 2) cntXOutEndY++;
                }
            }
            if (AutoNGOuterY > 0)
            {
                if (cntXOutFirstY >= AutoNGOuterY || cntXOutEndY >= AutoNGOuterY)
                {
                    return 2;
                }
            }

            //외각 분할 행 폐기
            int cntXOutAY = 0;
            int cntXOutBY = 0;
            int cntXOutCY = 0;
            int cntXOutDY = 0;
            for (int i = 0; i < m_nSizeY; i++) // 4
            {
                for (int j = 0; j < m_nSizeX; j++) // 30
                {
                    if (m_buffer[j, i] != 0 && i == 0)
                    {
                        if (j < m_nSizeX / 2) cntXOutAY++;
                        if (j >= m_nSizeX / 2) cntXOutBY++;
                    }
                    if (m_buffer[j, i] != 0 && i == m_nSizeY - 1)
                    {
                        if (j < m_nSizeX / 2) cntXOutCY++;
                        if (j >= m_nSizeX / 2) cntXOutDY++;
                    }
                }
            }
            if (AutoNGOuterDivY > 0)
            {
                if (cntXOutAY >= AutoNGOuterDivY || cntXOutBY >= AutoNGOuterDivY
                    || cntXOutCY >= AutoNGOuterDivY || cntXOutDY >= AutoNGOuterDivY)
                {
                    return 2;
                }
            }
            #endregion
            return 0;
        }
        public void Init(int x, int y, int[] priorityNG)
        {
            m_nSizeX = x;
            m_nSizeY = y;
            m_buffer = new int[m_nSizeX, m_nSizeY];
            m_dnn_buffer = new int[m_nSizeX, m_nSizeY];
            m_badunit = new int[m_nSizeX, m_nSizeY];
            m_PreSkipdatabuffer = new int[m_nSizeX, m_nSizeY];

            DnnUnitBuff = new int[m_nSizeX, m_nSizeY];
            DnnImageBuff = new List<Tuple<Point,int>>();

            AutoNG = false;
            IDString = "";
            PriorityNG = priorityNG;
            Clear();
        }
        public void Clear()
        {
            for (int i = 0; i < m_nSizeX; i++)
            {
                for (int j = 0; j < m_nSizeY; j++)
                {
                    m_buffer[i, j] = 0;
                    m_dnn_buffer[i, j] = 0;
                    m_badunit[i, j] = 0;
                    DnnUnitBuff[i, j] = 0;
                    m_PreSkipdatabuffer[i, j] = 0;
                }
            }
            DnnImageBuff.Clear();
            IDString = "";
            AutoNG = false;
        }
        public void Set(int px, int py, int val)
        {
            if(val > 0) m_badunit[px, py]++;
            if(m_buffer[px, py] == 0) m_buffer[px, py] = val;
            else if (PriorityNG[m_buffer[px, py]] > PriorityNG[val] && val > 0) m_buffer[px, py] = val;
        }
        public void SetDnn(int px, int py, int val, bool isSet)
        {
            if (m_dnn_buffer[px, py] == 0) m_dnn_buffer[px, py] = val;
            else if (PriorityNG[m_dnn_buffer[px, py]] > PriorityNG[val] && val > 0) m_dnn_buffer[px, py] = val;

            //Loss Table Buffer
            if (DnnUnitBuff[px, py] == 0) DnnUnitBuff[px, py] = val;
            else if (PriorityNG[DnnUnitBuff[px, py]] > PriorityNG[val] && val > 0) DnnUnitBuff[px, py] = val;

            if (isSet)
            {
                Tuple<Point, int> item = new Tuple<Point, int>(new Point(px,py), val);
                DnnImageBuff.Add(item);
            }
        }
        public bool SetAll(int val)
        {
            for (int i = 0; i < m_nSizeX; i++)
            {
                for (int j = 0; j < m_nSizeY; j++)
                {
                    if (m_buffer[i, j] == 0) m_buffer[i, j] = val;
                    else if(PriorityNG[m_buffer[i, j]] > PriorityNG[val] && val > 0) m_buffer[i, j] = val;
                    m_badunit[i, j] = 1;
                }
            }
            return true;
        }
        public int Get(int px, int py)
        {
            return m_buffer[px, py];
        }
        public int GetDnn(int px, int py)
        {
            return m_dnn_buffer[px, py];
        }
        public static InspectBuffer Sum(InspectBuffer a, InspectBuffer b)
        {
            InspectBuffer ib = new InspectBuffer();
            ib.Init(a.SizeX, a.SizeY, ib.PriorityNG);
            int tmp = 0;
            for (int i = 0; i < a.SizeX; i++)
            {
                for (int j = 0; j < a.SizeY; j++)
                {
                    tmp = a.BadUnit[i, j] + b.BadUnit[i, j];
                    ib.Set(i, j, Math.Max(a.Get(i, j), b.Get(i, j)));
                    ib.BadUnit[i, j] = tmp;
                }
            }

            if (a.AutoNG || b.AutoNG) ib.AutoNG = true;
            return ib;
        }
        public static void Add(ref InspectBuffer src, InspectBuffer des)
        {
            int tmp = 0;
            for (int i = 0; i < src.SizeX; i++)
            {
                for (int j = 0; j < src.SizeY; j++)
                {
                    tmp = src.BadUnit[i, j] + des.BadUnit[i, j];
                    src.Set(i, j, des.Get(i, j));
                    src.SetDnn(i, j, des.GetDnn(i, j), false);
                    src.BadUnit[i, j] = tmp;
                }
            }

            foreach (var item in des.DnnImageBuff)
            {
                src.DnnImageBuff.Add(item);
            }

            if (des.IDString != "") src.IDString = des.IDString;
            if (des.AutoNG) src.AutoNG = true;
        }

        private static Logger m_Logger = Logger.GetLogger();
        //재검마킹유무 결정
        public string GetLaserString(out int cnt, int step, int units, int total, int mode, int ModelStep, bool bVerify, int BoatID, ref int Allmarkingcount, ref int origincount, ref int SkipRemarkCnt)
        {
            //if(step == 0) m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("=========== Strip_id = {0}", IDString));
            int[,] m_ChcekMarkingbuffer = new int[m_nSizeX, m_nSizeY];
            string str = "";
            cnt = 0;
            int sc = units * (step);
            int ec = units * (step + 1);
            //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("Origin"));
            string mes = "";
            for (int j = 0; j < m_nSizeY; j++)
            {
                mes = "";
                for (int i = sc; i < ec; i++)
                {
                    if ((step == ModelStep - 1) && (i > total - 1))
                    {
                        ;
                    }
                    else
                    {
                        if (m_PreSkipdatabuffer[i, j] == 9 || m_PreSkipdatabuffer[i, j] == 10) origincount++;
                        mes += String.Format("{0} ", m_PreSkipdatabuffer[i, j]);
                    }

                }
                //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, mes);
            }
          
            //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("AfterSkipdata"));
            for (int j = 0; j < m_nSizeY; j++)
            {
                mes = "";
                for (int i = sc; i < ec; i++)
                {
                    if ((step == ModelStep - 1) && (i > total - 1))
                    {
                        ;
                    }
                    else
                        mes += String.Format("{0} ", m_buffer[i, j]);
                        
                }
                //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, mes);
            }

            for (int j = 0; j < m_nSizeY; j++)
            {
                mes = "";
                for (int i = sc; i < ec; i++)
                {
                    if ((step == ModelStep - 1) && (i > total - 1))
                    {
                        str += "0";
                    }
                    else
                    {
                        if (m_buffer[i, j] != 0)
                        {
                            if (bVerify)
                            {
                                if (mode > 0 && m_buffer[i, j] == 3) str += "1";// 이거 ID 미인식일때 마킹 할려는거 같은데 100은 Insptype이고 ID를 넣어야할거 같은데
                                else str += "0";                                // 문제는 ID 미인식이면 그냥 다 불량으로 빠져서 이부분을 쓸 이유가 없음.
                            }
                            else
                            {

                                if (mode > 0 && (m_buffer[i, j] == 9 || m_buffer[i, j] == 10))
                                {
                                    str += "0";// 재검 원소재 검사 마킹 X
                                    m_ChcekMarkingbuffer[i, j] = 2;
                                    //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("{0},{1} = X Skipdata 아님", i, j));
                                }
                                else if (mode > 0 && m_buffer[i, j] == 7)// 재검 Skipdata 원소재 마킹 유무
                                {
                                    Allmarkingcount++;
                                    if (m_PreSkipdatabuffer[i, j] == 9 || m_PreSkipdatabuffer[i, j] == 10)
                                    {
                                        str += "0";
                                        m_ChcekMarkingbuffer[i, j] = 0;
                                        //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("{0} => {1}", m_buffer[i, j], m_PreSkipdatabuffer[i, j]));
                                        //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("{0},{1} = X", i, j));
                                    }
                                    else
                                    {
                                        str += "1";
                                        SkipRemarkCnt++;
                                        cnt++;
                                        m_ChcekMarkingbuffer[i, j] = 1;
                                        //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("{0} => {1}", m_buffer[i, j], m_PreSkipdatabuffer[i, j]));
                                        //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("{0},{1} = O", i, j));
                                    }
                                }
                                else
                                {
                                    str += "1";
                                    cnt++;
                                    m_ChcekMarkingbuffer[i, j] = 3;
                                    //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("{0},{1} = O Another", i, j));
                                }
                            }
                        }
                        else
                        {
                            str += "0";
                            m_ChcekMarkingbuffer[i, j] = 0;
                        }
                    }
                    if ((i == (ec - 1)) && (j == m_nSizeY - 1)) break;
                    str += ",";
                }
            }
            //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("==========================={0}==========================", step));
            for (int j = 0; j < m_nSizeY; j++)
            {
                mes = "";
                for (int i = sc; i < ec; i++)
                {
                    if ((step == ModelStep - 1) && (i > total - 1))
                    {
                        ;
                    }
                    else
                        mes += String.Format("{0} ", m_ChcekMarkingbuffer[i, j]);
                }
                //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, mes);
            }
            return str;
        }
        //Y방향으로 뒤집어서 보낸다.
        public string GetLaserStringRev(out int cnt, int step, int units, int total, int mode, int ModelStep, bool bVerify, int BoatID, ref int Allmarkingcount, ref int origincount, ref int nSkipRemarkCnt)
        {
            //if (step == 0) m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("================== Strip_id = {0}==================", IDString));
            int[,] m_ChcekMarkingbuffer = new int[m_nSizeX, m_nSizeY];
            string str = "";
            cnt = 0;
            int sc = units * (step);
            int ec = units * (step + 1);            
            //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("Origin"));
            string mes = "";
            for (int j = 0; j < m_nSizeY; j++)
            {
                mes = "";
                for (int i = sc; i < ec; i++)
                {
                    if ((step == ModelStep - 1) && (i > total - 1))
                    {
                        ;
                    }
                    else
                    {
                        if (m_PreSkipdatabuffer[i, j] == 9 || m_PreSkipdatabuffer[i, j] == 10) origincount++;
                            mes += String.Format("{0} ", m_PreSkipdatabuffer[i, j]);
                    }
                        
                }
                //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, mes);
            }

            //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("AfterSkipdata"));
            for (int j = 0; j < m_nSizeY; j++)
            {
                mes = "";
                for (int i = sc; i < ec; i++)
                {
                    if ((step == ModelStep - 1) && (i > total - 1))
                    {
                        ;
                    }
                    else
                        mes += String.Format("{0} ", m_buffer[i, j]);
                }
                //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, mes);
            }
            for (int j = m_nSizeY - 1; j >= 0; j--)
            {
                for (int i = sc; i < ec; i++)
                {
                    if ((step == ModelStep-1) && (i > total - 1))
                    {
                        str += "0";
                    }
                    else
                    {
                        if (m_buffer[i, j] != 0)
                        {
                            if (bVerify)
                            {
                                if (mode > 0 && m_buffer[i, j] == 3) str += "1";
                                else str += "0";
                            }
                            else
                            {
                                if (mode > 0 && (m_buffer[i, j] == 9 || m_buffer[i, j] == 10))
                                {
                                    str += "0";// 재검 원소재 검사 마킹 X
                                    m_ChcekMarkingbuffer[i, j] = 2;
                                    //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("{0},{1} = X Skipdata 아님", i, j));
                                }
                                else if (mode > 0 && m_buffer[i, j] == 7)// 재검 Skipdata 원소재 마킹 유무
                                {
                                    Allmarkingcount++;
                                    if (m_PreSkipdatabuffer[i, j] == 9 || m_PreSkipdatabuffer[i, j] == 10)
                                    {
                                        str += "0";
                                        m_ChcekMarkingbuffer[i, j] = 0;
                                        //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("{0} => {1}", m_buffer[i, j], m_PreSkipdatabuffer[i, j]));
                                        //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("{0},{1} = X", i, j));
                                    }
                                    else
                                    {
                                        str += "1";
                                        cnt++;
                                        nSkipRemarkCnt++;
                                        m_ChcekMarkingbuffer[i, j] = 1;
                                        //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("{0} => {1}", m_buffer[i, j], m_PreSkipdatabuffer[i, j]));
                                        //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("{0},{1} = O", i, j));
                                    }
                                }
                                else
                                {
                                    str += "1";
                                    cnt++;
                                    m_ChcekMarkingbuffer[i, j] = 3;
                                    //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("{0},{1} = X Another", i, j));
                                }
                            }
                        }
                        else
                        {
                            str += "0";
                            m_ChcekMarkingbuffer[i, j] = 0;
                        }
                    }
                    if ((i == (ec - 1)) && (j == 0)) break;
                    str += ",";
                }
            }
            //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, String.Format("==========================={0}==========================", step));
            for (int j = 0; j < m_nSizeY; j++)
            {
                mes = "";
                for (int i = sc; i < ec; i++)
                {
                    if ((step == ModelStep - 1) && (i > total - 1))
                    {
                        ;
                    }
                    else
                        mes += String.Format("{0} ", m_buffer[i, j]);
                }
                //m_Logger.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, mes);
            }
            return str;
        }
    }
}
