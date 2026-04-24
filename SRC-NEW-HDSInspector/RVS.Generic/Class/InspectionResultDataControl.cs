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
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Documents;
using Common;

namespace RVS.Generic
{
    public enum VerifyResult
    {
        Done = 0,
        None = 99,
        Error = -1
    }

    [Serializable]
    public class ResultData
    {
        public int InspectStrip;
        public int TotalStrip;
        public int TotalUnits;
        public int BadUnits;
        public int GoodStrip;
        public int BadStrip;
        public int FailStrip;
        public double Yield;
        public double BeforeYield;
        public double FYield;
        public int FTotalUnits;
        public int FailUnits;

        public int[] UnitBad;
        public int[] FailBad;
        public int[] DnnBad;

        public int BeforeCount;
    }
    
    [Serializable]
    public class InspectionResultDataControl : NotifyPropertyChanged
    {
        public RegistryReadWrite m_2DRegistry = new RegistryReadWrite("MatrixCount");
        public NGInformationHelper Infos;
        // Ctor.
        public InspectionResultDataControl(NGInformationHelper info)
        {
            Infos = info;
            Init();
            p2DMarkCountBP = m_2DRegistry.Read("p2DMarkCountBP", p2DMarkCountBP);
            p2DMarkCountCA = m_2DRegistry.Read("p2DMarkCountCA", p2DMarkCountCA);
            p2DMarkCountBA = m_2DRegistry.Read("p2DMarkCountBA", p2DMarkCountBA);
        }

        // 데이터 초기화.
        public void Init()
        {
            TotalStrip = 0;
            InspectStrip = 0;
            BTotalStrip = 0;
            TotalUnits = 0;
            BadUnits = 0;
            CompletionUnits = 0;
            GoodStrip = 0;
            BadStrip = 0;
            FailStrip = 0;
            FTotalUnits = 0;
            FailUnits = 0;
            FYield = 100.0;
            BTotalUnits = 0;
            BBadUnits = 0;
            BGoodStrip = 0;
            BBadStrip = 0;
            BFailStrip = 0;
            BeforeYield = 100.0;
            Yield = 100.0;
            BeforeCount = 0;
            for (int i = 0; i < Infos.Size; i++)
            {
                if (UnitBad == null) break;
                this.UnitBad[i] = 0;
                this.FailBad[i] = 0;
                this.DnnBad[i] = 0;
                this.DnnTUnitTbl[i] = 0;
                this.DnnFUnitTbl[i] = 0;
                this.DnnTImageTbl[i] = 0;
                this.DnnFImageTbl[i] = 0;
            }
        }

        public bool ClearData()
        {
            p2DMarkCountCA = 0;
            p2DMarkCountBA = 0;
            p2DMarkCountBP = 0;
            Init();
            StartTime = DateTime.Now;
            try
            {
                if (!File.Exists(ResultFile))
                {
                    File.Delete(ResultFile);
                }
                FileStream fs = File.Create(ResultFile);
                fs.Close();
                if (!File.Exists(MapFile))
                {
                    File.Delete(MapFile);
                }
                fs = File.Create(MapFile);
                fs.Close();
                if (!File.Exists(FMapFile))
                {
                    File.Delete(FMapFile);
                }
                fs = File.Create(FMapFile);
                fs.Close();
                if (!File.Exists(DMapFile))
                {
                    File.Delete(DMapFile);
                }
                fs = File.Create(DMapFile);
                fs.Close();

                DirectoryManager.DeleteDirectory(ImagePath);
                DirectoryManager.DeleteDirectory(DataMatrixPath);
            }
            catch { return false; }

            return true;
        }

        public void LoadData()
        {
            if (!File.Exists(ResultFile))
            {
                FileStream fs = File.Create(ResultFile);
                fs.Close();
            }
            IniFile ini = new IniFile(ResultFile);
            StartTime = ini.ReadDateTime("결과정보", "시작시간", DateTime.Now);
            InspectStrip = ini.ReadInteger("결과정보", "투입스트립", 0);
            TotalStrip = ini.ReadInteger("결과정보", "완성스트립", 0);
            GoodStrip = ini.ReadInteger("결과정보", "양품스트립", 0);
            BadStrip = ini.ReadInteger("결과정보", "불량스트립", 0);
            TotalUnits = ini.ReadInteger("결과정보", "전체유닛", 0);
            BadUnits = ini.ReadInteger("결과정보", "불량유닛", 0);

            CompletionUnits = ini.ReadInteger("결과정보", "양품유닛", 0);

            BeforeYield = ini.ReadDouble("결과정보", "Verify전수율", 0);
            Yield = ini.ReadDouble("결과정보", "Verify후수율", 0);

            for(int i =1; i< Infos.Size; i++)
            {
                Bad_Info bad = Infos.GetItem(i);
                if (bad == null) continue;
                UnitBad[i] = ini.ReadInteger("불량정보", bad.Name, 0);
            }

            FailStrip = ini.ReadInteger("결과정보", "폐기 스트립", 0);
            FTotalUnits = ini.ReadInteger("결과정보", "폐기 전체유닛", 0);
            FailUnits = ini.ReadInteger("결과정보", "폐기 불량유닛", 0);
            FYield = ini.ReadDouble("결과정보", "폐기 수율", 0);

            for (int i = 1; i < Infos.Size; i++)
            {
                Bad_Info bad = Infos.GetItem(i);
                if (bad == null) continue;
                FailBad[i] = ini.ReadInteger("불량정보", "폐기 " + bad.Name, 0);
            }
            for (int i = 1; i < Infos.Size; i++)
            {
                Bad_Info bad = Infos.GetItem(i);
                if (bad == null) continue;
                DnnBad[i] = ini.ReadInteger("Dnn 불량정보", "Dnn " + bad.Name, 0);
            }
            for (int i = 1;i < Infos.Size; i++)
            {
                Bad_Info bad = Infos.GetItem(i);
                if (bad == null) continue;
                DnnTUnitTbl[i] = ini.ReadInteger("Dnn 양품정보(유닛)", "Dnn" + bad.Name, 0);
            }
            for (int i = 1;i< Infos.Size; i++)
            {
                Bad_Info bad = Infos.GetItem(i);
                if (bad == null) continue;
                DnnFUnitTbl[i] = ini.ReadInteger("Dnn 불량정보(유닛)", "Dnn" + bad.Name, 0);
            }
            for (int i=1;i< Infos.Size; i++)
            {
                Bad_Info bad = Infos.GetItem(i);
                if (bad == null) continue;
                DnnTImageTbl[i] = ini.ReadInteger("Dnn 양품정보(이미지)", "Dnn" + bad.Name, 0);
            }
            for (int i=1; i<Infos.Size; i++)
            {
                Bad_Info bad = Infos.GetItem(i);
                if (bad == null) continue;
                DnnFImageTbl[i] = ini.ReadInteger("Dnn 불량정보(이미지)", "Dnn" + bad.Name, 0);
            }

            BTotalStrip = ini.ReadInteger("결과정보", "이전완성스트립", 0);
            BGoodStrip = ini.ReadInteger("결과정보", "이전양품스트립", 0);
            BBadStrip = ini.ReadInteger("결과정보", "이전불량스트립", 0);
            BFailStrip = ini.ReadInteger("결과정보", "이전폐기스트립", 0);
            BTotalUnits = ini.ReadInteger("결과정보", "이전전체유닛", 0);
            BBadUnits = ini.ReadInteger("결과정보", "이전불량유닛", 0);
            BeforeCount = InspectStrip;
        }

        public void AddData(InspectBuffer data, int ID, int UnitLimit, bool bUseITS)
        {
        
            InspectStrip++;
 
            //폐기
            if (data.AutoNG)
            {
                FailStrip++;
                int cnt = data.BadCount();
                FTotalUnits += (data.SizeX * data.SizeY);

                for (int i = 0; i < data.SizeX; i++)
                {
                    for (int j = 0; j < data.SizeY; j++)
                    {
                        if (data.Buffer[i, j] == 0) continue;
                        FailBad[data.Buffer[i, j]]++;
                        DnnFUnitTbl[data.DnnUnitBuff[i, j]]++;

                        foreach(var imageBad in data.DnnImageBuff.FindAll(x => x.Item1 == new Point(i, j)))  
                            DnnFImageTbl[imageBad.Item2]++;              
                    }
                }

                FailUnits += cnt;

                FYield = (1.0 - ((double)FailUnits / (double)FTotalUnits)) * 100.0; // 수율 계산
            }
            //양품
            else
            {
                int cnt = data.BadCount();
                TotalUnits += (data.SizeX * data.SizeY);
                TotalStrip++; 
                if (cnt == 0) GoodStrip++;
                else
                {
                    BadStrip++;
                    BadUnits += cnt;
                    for (int i = 0; i < data.SizeX; i++)
                    {
                        for (int j = 0; j < data.SizeY; j++)
                        {
                            if (data.Buffer[i, j] == 0) continue;
                            UnitBad[data.Buffer[i, j]]++;
                            DnnBad[data.Dnn_Buffer[i, j]]++;
                            DnnTUnitTbl[data.DnnUnitBuff[i, j]]++;

                            foreach(var imageBad in data.DnnImageBuff.FindAll(x => x.Item1 == new Point(i,j) ))
                                DnnTImageTbl[imageBad.Item2]++;
                        }
                    }
                }

                BeforeYield = (1.0 - ((double)BadUnits / (double)TotalUnits)) * 100.0;
                Yield = (1.0 - ((double)BadUnits / (double)TotalUnits)) * 100.0;
            }

            CompletionUnits = TotalUnits - BadUnits;

            SaveData();
            WriteMap(data, ID, bUseITS);
        }

        public void SaveStartTime(DateTime adtTime)
        {
            if (!File.Exists(ResultFile) && ResultFile != null)
            {
                FileStream fs = File.Create(ResultFile);
                fs.Close();
            }
            IniFile ini = new IniFile(ResultFile);
            ini.WriteDateTime("결과정보", "시작시간", adtTime);
        }

        public void SaveEndTime(DateTime adtTime)
        {
            if (!File.Exists(ResultFile) && ResultFile != null)
            {
                FileStream fs = File.Create(ResultFile);
                fs.Close();
            }
            IniFile ini = new IniFile(ResultFile);
            ini.WriteDateTime("결과정보", "종료시간", adtTime);
        }

        public void SaveData()
        {
            if (!File.Exists(ResultFile) && ResultFile != null)
            {
                FileStream fs = File.Create(ResultFile);
                fs.Close();
            }
            IniFile ini = new IniFile(ResultFile);
            ini.WriteInteger("결과정보", "투입스트립", InspectStrip);
            ini.WriteInteger("결과정보", "완성스트립", TotalStrip);
            ini.WriteInteger("결과정보", "양품스트립", GoodStrip);
            ini.WriteInteger("결과정보", "불량스트립", BadStrip);

            ini.WriteInteger("결과정보", "전체유닛", TotalUnits);
            ini.WriteInteger("결과정보", "불량유닛", BadUnits);

            ini.WriteInteger("결과정보", "양품유닛", CompletionUnits);

            ini.WriteDouble("결과정보", "Verify전수율", BeforeYield);
            ini.WriteDouble("결과정보", "Verify후수율", Yield);

            for (int i = 1; i < Infos.Size; i++)
            {
                Bad_Info bad = Infos.GetItem(i);
                if (bad == null) continue;
                ini.WriteInteger("불량정보", bad.Name, UnitBad[i]);
            }
            ini.WriteInteger("결과정보", "폐기 스트립", FailStrip);
            ini.WriteInteger("결과정보", "폐기 전체유닛", FTotalUnits);
            ini.WriteInteger("결과정보", "폐기 불량유닛", FailUnits);
            ini.WriteDouble("결과정보", "폐기 수율", FYield);

            for (int i = 1; i < Infos.Size; i++)
            {
                Bad_Info bad = Infos.GetItem(i);
                if (bad == null) continue;
                ini.WriteInteger("불량정보", "폐기 " + bad.Name, FailBad[i]);
            }
            for (int i = 1; i < Infos.Size; i++)
            {
                Bad_Info bad = Infos.GetItem(i);
                if (bad == null) continue;
                ini.WriteInteger("Dnn 불량정보", "Dnn " + bad.Name, DnnBad[i]);
            }
            for (int i = 1; i<Infos.Size; i++)
            {
                Bad_Info bad = Infos.GetItem(i);
                if (bad == null) continue;
                ini.WriteInteger("Dnn 양품정보(유닛)", "Dnn" + bad.Name, DnnTUnitTbl[i]);
            }
            for (int i =1; i<Infos.Size;i++)
            {
                Bad_Info bad = Infos.GetItem(i);
                if (bad == null) continue;
                ini.WriteInteger("Dnn 불량정보(유닛)", "Dnn" + bad.Name, DnnFUnitTbl[i]);
            }
            for (int i = 1;i<Infos.Size;i++)
            {
                Bad_Info bad = Infos.GetItem(i);
                 if (bad == null) continue;
                 ini.WriteInteger("Dnn 양품정보(이미지)", "Dnn" + bad.Name, DnnTImageTbl[i]);
            }
            for (int i = 1; i<Infos.Size; i++)
            {
                Bad_Info bad = Infos.GetItem(i);
                if (bad == null) continue;
                ini.WriteInteger("Dnn 불량정보(이미지)", "Dnn" + bad.Name, DnnFImageTbl[i]);
            }

            ini.WriteInteger("결과정보", "이전완성스트립", BTotalStrip);
            ini.WriteInteger("결과정보", "이전양품스트립", BGoodStrip);
            ini.WriteInteger("결과정보", "이전불량스트립", BBadStrip);
            ini.WriteInteger("결과정보", "이전폐기스트립", BFailStrip);
            ini.WriteInteger("결과정보", "이전전체유닛", BTotalUnits);
            ini.WriteInteger("결과정보", "이전불량유닛", BBadUnits);
        }

        public void WriteMap(InspectBuffer data, int ID, bool bUseITS)
        {
            string text = "";
            string dtext = "";

            if (bUseITS)
            {
                text = "[" + data.IDString + "]";
                dtext = "[" + data.IDString + "]";
                //string[] temp = data.IDString.Split(' ');
                //if (temp.Length == 2)
                //{ nID = Convert.ToInt32(temp[temp.Length - 1]); }
            }
            else
            {
                int nID = BeforeCount + ID;
                string.Format("[{0:D4}:{1}", nID, data.AutoNG ? 0 : 1);            
                text = "[" + nID.ToString("0000") + "]";
                dtext = "[" + nID.ToString("0000") + "]";
            }            

            for (int i = 0; i < data.SizeY; i++)
            {
                text += " ";
                dtext += " ";
                for (int j = 0; j < data.SizeX; j++)
                {
                    text += Infos.GetMapString(data.Buffer[j, i]);
                    dtext += Infos.GetMapString(data.Dnn_Buffer[j, i]);
                }
            }
            try
            {
                if (data.AutoNG)
                {
                    if (!File.Exists(FMapFile))
                    {
                        FileStream fs = File.Create(FMapFile);
                        fs.Close();
                    }
                    System.IO.File.AppendAllText(FMapFile, text + Environment.NewLine);
                }
                else
                {
                    if (!File.Exists(MapFile))
                    {
                        FileStream fs = File.Create(MapFile);
                        fs.Close();
                    }
                    System.IO.File.AppendAllText(MapFile, text + Environment.NewLine);
                    if (!File.Exists(DMapFile))
                    {
                        FileStream fs = File.Create(DMapFile);
                        fs.Close();
                    }
                    System.IO.File.AppendAllText(DMapFile, dtext + Environment.NewLine);
                }
            }
            catch
            {

            }
        }

        public ResultData CopyTo()
        {
            ResultData tmp = new ResultData();
            tmp.InspectStrip = this.InspectStrip;
            tmp.TotalStrip = this.TotalStrip;
            tmp.TotalUnits= this.TotalUnits;
            tmp.BadUnits= this.BadUnits;
            tmp.GoodStrip= this.GoodStrip;
            tmp.BadStrip= this.BadStrip;
            tmp.FailStrip= this.FailStrip;
            tmp.Yield= this.Yield;
            tmp.FYield = this.FYield;
            tmp.FTotalUnits = this.FTotalUnits;
            tmp.FailUnits = this.FailUnits;
            tmp.BeforeYield = this.BeforeYield;
            tmp.UnitBad = new int[Infos.Size];
            tmp.FailBad = new int[Infos.Size];
            tmp.DnnBad = new int[Infos.Size];
            Array.Copy(this.UnitBad, tmp.UnitBad, Infos.Size);
            Array.Copy(this.FailBad, tmp.FailBad, Infos.Size);
            Array.Copy(this.DnnBad, tmp.DnnBad, Infos.Size);

            tmp.BeforeCount = this.BeforeCount;
            return tmp;
        }

        public int InspectStrip
        {
            get { return m_nInspectStrip; }
            set
            {
                m_nInspectStrip = value;
                Notify("InspectStrip");
            }
        }

        public int TotalStrip
        {
            get { return m_nTotalStrip; }
            set
            {
                m_nTotalStrip = value;
                Notify("TotalStrip");
            }
        }

        public int BTotalStrip { get; set; }

        public int BeforeCount
        {
            get { return m_nBeforeCount; }
            set
            {
                m_nBeforeCount = value;
                Notify("BeforeCount");
            }
        }

        #region Unit Inspection Count (Top, Bottom, Trans)
        public int TotalUnits
        {
            get { return m_nTotalUnits; }
            set
            {
                m_nTotalUnits = value;
                Notify("TotalUnits");
            }
        }

        public int BTotalUnits { get; set; }

        public int BadUnits
        {
            get { return m_nBadUnits; }
            set
            {
                m_nBadUnits = value;
                Notify("BadUnits");
            }
        }



        public int CompletionUnits
        {
            get { return m_nCompletionUnits; }
            set
            {
                m_nCompletionUnits = value;
                Notify("CompletionUnits");
            }
        }

        public int PSR_Shift
        {
            get { return m_nPSR_Shift; }
            set
            {
                m_nPSR_Shift = value;
                Notify("PSR_Shift");
            }
        }

        public int BBadUnits { get; set; }

        public int GoodStrip
        {
            get { return m_nGoodStrip; }
            set
            {
                m_nGoodStrip = value;
                Notify("GoodStrip");
            }
        }

        public int BGoodStrip { get; set; }

        public int BadStrip
        {
            get
            {
                return m_nBadStrip;
            }
            set
            {
                m_nBadStrip = value;
                Notify("BadStrip");
            }
        }

        public int BBadStrip { get; set; }

        public int FailStrip
        {
            get
            {
                return m_nFailStrip;
            }
            set
            {
                m_nFailStrip = value;
                Notify("FailStrip");
            }
        }

        public int BFailStrip { get; set; }

        public double BeforeYield
        {
            get
            {
                return m_fBeforeYield;
            }
            set
            {
                m_fBeforeYield = value;
                Notify("BeforeYield");
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

        public double FYield
        {
            get
            {
                return m_ffYield;
            }
            set
            {
                m_ffYield = value;
                Notify("FYield");
            }
        }

        public int FTotalUnits
        {
            get { return m_nfTotalUnits; }
            set
            {
                m_nfTotalUnits = value;
                Notify("FTotalUnits");
            }
        }

        public int FailUnits
        {
            get { return m_nFailUnits; }
            set
            {
                m_nFailUnits = value;
                Notify("FailUnits");
            }
        }
        #endregion

        public BindableArray<int> UnitBad
        {
            get { return m_UnitBad; }
            set
            {
                m_UnitBad = value;
                Notify("UnitBad");
            }
        }

        public BindableArray<int> FailBad
        {
            get { return m_FailBad; }
            set
            {
                m_FailBad = value;
                Notify("FailBad");
            }
        }

        public BindableArray<int> DnnBad
        {
            get { return m_DnnBad; }
            set
            {
                m_DnnBad = value;
                Notify("DnnBad");
            }
        }

        public BindableArray<int> DnnTUnitTbl
        {
            get { return m_DnnTUnitTbl; }
            set
            {
                m_DnnTUnitTbl = value;
                Notify("DnnTUnitTbl");
            }
        }

        public BindableArray<int> DnnFUnitTbl
        {
            get { return m_DnnFUnitTbl; }
            set
            {
                m_DnnFUnitTbl = value;
                Notify("DnnFUnitTbl");
            }
        }

        public BindableArray<int> DnnTImageTbl
        {
            get { return m_DnnTImageTbl; }
            set
            {
                m_DnnTImageTbl = value;
                Notify("DnnTImageTbl");
            }
        }

        public BindableArray<int> DnnFImageTbl
        {
            get { return m_DnnFImageTbl; }
            set
            {
                m_DnnFImageTbl = value;
                Notify("DnnFImageTbl");
            }
        }

        // 불량 개수 초기화.
        public void InitNGInfo()
        {
            this.m_UnitBad = new BindableArray<int>(Infos.Size);
            this.m_FailBad = new BindableArray<int>(Infos.Size);
            this.m_DnnBad = new BindableArray<int>(Infos.Size);
            this.m_DnnTUnitTbl = new BindableArray<int>(Infos.Size);
            this.m_DnnFUnitTbl = new BindableArray<int>(Infos.Size);
            this.m_DnnTImageTbl = new BindableArray<int>(Infos.Size);
            this.m_DnnFImageTbl = new BindableArray<int>(Infos.Size);
            for (int i=0; i< Infos.Size; i++)
            {
                this.UnitBad[i] = 0;
                this.FailBad[i] = 0;
                this.DnnBad[i] = 0;
                this.DnnTUnitTbl[i] = 0;
                this.DnnFUnitTbl[i] = 0;
                this.DnnTImageTbl[i] = 0;
                this.DnnFImageTbl[i] = 0;
            }
        }

        public void ResetFail()
        {
            FailStrip = 0;
            FTotalUnits = 0;
            FailUnits = 0;
            FYield = 100;

            for (int i = 0; i < Infos.Size; i++)
            {
                this.FailBad[i] = 0;
                this.DnnFUnitTbl[i] = 0;
                this.DnnFImageTbl[i] = 0;
            }

            try
            {
                if (!File.Exists(FMapFile))
                {
                    File.Delete(FMapFile);
                }
                FileStream fs = File.Create(FMapFile);
                fs.Close();
            }
            catch { }
        }

        #region Time information.
        public DateTime StartTime
        {
            get { return m_StartTime; }
            set
            {
                m_StartTime = value;
                Notify("StartTime");
            }
        }

        public TimeSpan RunTime
        {
            get { return m_RunTime; }
            set
            {
                m_strRunTime = String.Format("{0}일 {1}시간 {2}분 {3}초", value.Days, value.Hours, value.Minutes, value.Seconds);
                m_RunTime = value;
                Notify("RunTime");
            }
        }

        /// <summary>   Gets or sets the time of the now. </summary>
        /// <value> The time of the now. </value>
        public DateTime NowTime
        {
            get { return m_NowTime; }
            set
            {
                m_NowTime = value;
                Notify("NowTime");
            }
        }

        /// <summary>   Gets or sets the time of the end. </summary>
        /// <value> The time of the end. </value>
        public DateTime EndTime
        {
            get { return m_EndTime; }
            set
            {
                m_EndTime = value;
                Notify("EndTime");
            }
        }
        #endregion

        #region Model Info
        public string Operator
        {
            get { return m_strOperator; }
            set
            {
                m_strOperator = value;
                Notify("Operator");
            }
        }

        public string ModelName
        {
            get { return m_strModelName; }
            set
            {
                m_strModelName = value;
                Notify("ModelName");
            }
        }

        public string GroupName
        {
            get { return m_strGroupName; }
            set
            {
                m_strGroupName = value;
                Notify("GroupName");
            }
        }

        public string LotNo
        {
            get { return m_strLotNo; }
            set
            {
                m_strLotNo = value;
                Notify("LotNo");
            }
        }

        public string ProcCode
        {
            get { return m_strProcCode; }
            set
            {
                m_strProcCode = value;
                Notify("ProcCode");
            }
        }

        public string Re_LotNo
        {
            get { return m_strReLotNo; }
            set
            {
                m_strReLotNo = value;
                Notify("ReLotNo");
            }
        }

        public string ImagePath
        {
            get { return m_strImagePath; }
            set
            {
                m_strImagePath = value;
                Notify("ImagePath");
            }
        }

        public string ResultFile
        {
            get { return m_strResultFile; }
            set
            {
                m_strResultFile = value;
                Notify("ResultFile");
            }
        }

        public string MapFile
        {
            get { return m_strMapFile; }
            set
            {
                m_strMapFile = value;
                Notify("MapFile");
            }
        }

        public string FMapFile
        {
            get { return m_strFMapFile; }
            set
            {
                m_strFMapFile = value;
                Notify("FMapFile");
            }
        }

        public string DMapFile
        {
            get; set;
        }

        public string DataMatrixPath
        {
            get; set;
        }
        #endregion

        private int m_2DMarkCountCA = 0;
        private int m_2DMarkCountBA = 0;
        private int m_2DMarkCountBP = 0;
        public int p2DMarkCountCA
        {
            get { return m_2DMarkCountCA; }
            set
            {
                m_2DMarkCountCA = value;
                m_2DRegistry.Write("p2DMarkCountCA", value);
            }
        }
        public int p2DMarkCountBA
        {
            get { return m_2DMarkCountBA; }
            set
            {
                m_2DMarkCountBA = value;
                m_2DRegistry.Write("p2DMarkCountBA", value);
            }
        }
        public int p2DMarkCountBP
        {
            get { return m_2DMarkCountBP; }
            set
            {
                m_2DMarkCountBP = value;
                m_2DRegistry.Write("p2DMarkCountBP", value);
            }
        }

        #region Private member variables.

        private int m_nInspectStrip;
        private int m_nTotalStrip;
        private int m_nTotalUnits;
        private int m_nBadUnits;
        private int m_nCompletionUnits;
        private int m_nPSR_Shift;
        private int m_nGoodStrip;
        private int m_nBadStrip;
        private int m_nFailStrip;
        private double m_fYield;
        private double m_fBeforeYield;
        private int m_nBeforeCount;

        private double m_ffYield;
        private int m_nfTotalUnits;
        private int m_nFailUnits;

        private BindableArray<int> m_UnitBad;
        private BindableArray<int> m_FailBad;
        private BindableArray<int> m_DnnBad;
        private BindableArray<int> m_DnnTUnitTbl;
        private BindableArray<int> m_DnnFUnitTbl;
        private BindableArray<int> m_DnnTImageTbl;
        private BindableArray<int> m_DnnFImageTbl;

        private string m_strOperator;
        private string m_strModelName;
        private string m_strLotNo;
        private string m_strProcCode;
        private string m_strReLotNo;
        private string m_strGroupName;
        private string m_strImagePath;
        private string m_strResultFile;
        private string m_strMapFile;
        private string m_strFMapFile;

        private DateTime m_StartTime;
        private TimeSpan m_RunTime;
        private DateTime m_NowTime;
        private DateTime m_EndTime;
        public string m_strRunTime;
        #endregion
    }
}
