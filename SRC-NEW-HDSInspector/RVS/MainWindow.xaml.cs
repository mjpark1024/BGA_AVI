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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Collections;
using System.IO;
using Common;
using Common.DataBase;
using RVS.Generic;
using System.Windows.Media.Animation;
using RVS.Generic.Insp;
using RVS.Generic.Class;
using PCS.Interface;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

namespace RVS
{
    public partial class MainWindow : Window
    {
        public static Configs Setting;

        private RemotingRVSInterface[] m_PCSObjList = new RemotingRVSInterface[3];

        int m_CurrMC = 0;
        string[] imgfiles;
        List<IntPoint> lstUnit = new List<IntPoint>();
        int[,] BeforeMap;
        int[,] AfterMap;
        bool bStartUnit = false;
        IntPoint ptUnit;
        int CurrUnit = 0;
        int imgPage = 0;
        int CurrPage = 0;
        int CurrStrip = 0;
        StatusCtrl[] Status = new StatusCtrl[3];
        List<VerifyData> VerifyQueue = new List<VerifyData>();
        Model[] m_Model = new Model[3];
        bool m_bVerifying = false;
        Thread VerifyThd;

        public bool IsVerifying
        {
            get {return m_bVerifying;}
            set {m_bVerifying = value;}
        }

        public MainWindow()
        {
            InitializeComponent();
            Status[0] = Status1; Status[1] = Status2; Status[2] = Status3;
            Status1.ID = 0; Status2.ID = 1; Status3.ID = 2;
            m_Model[0] = new Model();
            m_Model[1] = new Model();
            m_Model[2] = new Model();
            Status[0].Visibility = Visibility.Hidden;
            Status[1].Visibility = Visibility.Hidden;
            Status[2].Visibility = Visibility.Hidden;
            VerifyQueue.Clear();
            grdButton.IsEnabled = false;
            bStartUnit = false;
            LoadSetting();
            InitEvent();
        }

        void InitEvent()
        {
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            this.imgList.Image_Click += imgList_Image_Click;
            this.btnPad0.Click += btnPad0_Click;
            this.btnPad1.Click += btnPad1_Click;
            this.btnPad3.Click += btnPad3_Click;
            RemotingPCSInterface.ConnectEquipEvent += RemotingPCSInterface_ConnectEquipEvent;
            RemotingPCSInterface.DisconnectEquipEvent += RemotingPCSInterface_DisconnectEquipEvent;
            RemotingPCSInterface.EnqueueVerifyDataEvent += RemotingPCSInterface_EnqueueVerifyDataEvent;
            RemotingPCSInterface.SetVerifyDataEvent += RemotingPCSInterface_SetVerifyDataEvent;
           // RemotingPCSInterface.EnqueueVerifyDataEvent += RemotingPCSInterface_ReqVerifyDataEvent;
        }

        void RemotingPCSInterface_SetVerifyDataEvent(string aEquipName, ResultData aData)
        {
            int nID = GetMachineID(aEquipName);
            Action a = delegate
            {
                Status[nID].SetResult(aData);
            }; this.Dispatcher.Invoke(a);

        }

        void btnPad3_Click(object sender, RoutedEventArgs e)
        {
            if (CurrUnit <= 0)
            {
                MessageBox.Show("첫번째 유닛입니다.");
                return;
            }
            grdButton.IsEnabled = false;
            CurrUnit--;
            Stripmap.SetColor(lstUnit[CurrUnit].X, lstUnit[CurrUnit].Y, BeforeMap[lstUnit[CurrUnit].X, lstUnit[CurrUnit].Y]);
            NextUnit();
            grdButton.IsEnabled = true;
        }

        void btnPad1_Click(object sender, RoutedEventArgs e)
        {
            grdButton.IsEnabled = false;
            //CurrPage++;
            //if (CurrPage >= imgPage)
            //{
                AfterMap[ptUnit.X, ptUnit.Y] = BeforeMap[ptUnit.X, ptUnit.Y];
                Stripmap.SetColor(ptUnit.X, ptUnit.Y, BeforeMap[ptUnit.X, ptUnit.Y]);
                bStartUnit = false;
                CurrUnit++;
                if (CurrUnit >= lstUnit.Count)
                {
                    m_PCSObjList[m_CurrMC].VerifyDoneMessageToPCS(VerifyResult.Done, AfterMap);
                    ClearStatus();
                    m_bVerifying = false;
                    grdButton.IsEnabled = false;
                }
                else
                {
                    NextUnit();
                    
                }
            //}
           // else
           // {
            //    NextImage();
           // }
            
        }

        void SetMCstatus(int anMC)
        {
            try
            {
                lblMC.Content = Status[anMC].lblMCName.Content.ToString();
                lblModel.Content = Status[anMC].lblModel.Content.ToString();
                lblLot.Content = Status[anMC].lblLot.Content.ToString();
                lblBYield.Content = Status[anMC].lblBYield.Content.ToString();
                lblAYield.Content = Status[anMC].lblAYield.Content.ToString();
            }
            catch { }
        }

        void SetStrip()
        {
            lblStripID.Content = CurrStrip.ToString("000");
            lblUnitX.Content = (ptUnit.X+1).ToString("00");
            lblUnitY.Content = (ptUnit.Y+1).ToString("00");
        }

        void ClearStatus()
        {
            lblMC.Content = "";
            lblModel.Content = "";
            lblLot.Content = "";
            lblBYield.Content = "";
            lblAYield.Content = "";
            lblStripID.Content = "";
            lblUnitX.Content = "";
            lblUnitY.Content = "";
            Stripmap.ClearColor();
            grdButton.IsEnabled = false;
            imgList.Clear();
        }

        void btnPad0_Click(object sender, RoutedEventArgs e)
        {
            grdButton.IsEnabled = false;
            int inputValue = int.Parse(((Button)sender).Tag.ToString());
            CurrPage++;
            if (CurrPage >= imgPage)
            {
                AfterMap[ptUnit.X, ptUnit.Y] = 99;
                Stripmap.SetColor(ptUnit.X, ptUnit.Y, 0);
                bStartUnit = false;
                CurrUnit++;
                if (CurrUnit >= lstUnit.Count)
                {
                    try
                    {
                        m_PCSObjList[m_CurrMC].VerifyDoneMessageToPCS(VerifyResult.Done, AfterMap);
                    }
                    catch //(Exception ex) 
                    { 
                        
                    }
                    ClearStatus();
                    m_bVerifying = false;
                    grdButton.IsEnabled = false;
                    //Verify 결과를 보내 주자.
                    ////////////////////////
                    ////////////////////////
                }
                else NextUnit();
            }
            else
            {
                NextImage();
                grdButton.IsEnabled = true;
            }
            //grdButton.IsEnabled = true;
        }

        void imgList_Image_Click(int anTag)
        {
            if (imgList.Def[anTag].Source != null)
            {
                WideImage.Source = imgList.Def[anTag].Source;
            }
        }

        bool RemotingPCSInterface_EnqueueVerifyDataEvent(string aEquipName, int anStripID, int[,] aMap)
        {
            int nEquipIndex = 0;
            // DB 연결
            nEquipIndex = GetMachineID(aEquipName);
            if (nEquipIndex == -1)
                return false;
            VerifyData v = new Generic.Class.VerifyData(nEquipIndex, anStripID, aMap, m_Model[nEquipIndex].UnitX, m_Model[nEquipIndex].UnitY);
            lock (this)
            {
                VerifyQueue.Add(v);
            }
            return true;;
        }

        void Verify()
        {
            while(true)
            {
                Thread.Sleep(500);
                if (!m_bVerifying && VerifyQueue.Count > 0)
                {
                    VerifyData v = null;
                    lock (this)
                    {
                        v = VerifyQueue[0].CopyTo();
                        VerifyQueue.RemoveAt(0);
                    }
                    Action a = delegate
                    {
                        SetVerify(v);
                    }; this.Dispatcher.Invoke(a);
                }
            }
        }

        void SetVerify(VerifyData aData)
        {
            m_bVerifying = true;
            Stripmap.SetStripMap(aData.X, aData.Y);
            BeforeMap = aData.Map;
            AfterMap = new int[aData.X, aData.Y];
            Array.Copy(aData.Map, AfterMap, aData.Map.Length);
            Stripmap.SetStrip(BeforeMap);
            Stripmap.SetCurrPos(0, 0);
            m_CurrMC = aData.MCNo;
            CurrStrip = aData.StripID;
            bool bBad = false;
            lstUnit.Clear();
            for (int j = 0; j < m_Model[m_CurrMC].UnitY; j++)
            {
                for (int i = 0; i < m_Model[m_CurrMC].UnitX; i++)
                {
                    if (BeforeMap[i, j] > 1 && BeforeMap[i, j] < 100)
                    {
                        IntPoint tmp = new IntPoint(i, j);
                        lstUnit.Add(tmp);
                        bBad = true;
                    }
                }
            }
            if (bBad)
            {
                ptUnit = new IntPoint(lstUnit[0].X, lstUnit[0].Y);
                CurrUnit = 0;
            }
            SetMCstatus(m_CurrMC);
            LoadUnit();
            grdButton.IsEnabled = true;
        }

        private void LoadUnit()
        {
            Stripmap.SetCurrPos(ptUnit.X, ptUnit.Y);
            lblUnitX.Content = (ptUnit.X + 1).ToString("00");
            lblUnitY.Content = (ptUnit.Y + 1).ToString("00");
            if (!LoadImage())
            {
                AfterMap[ptUnit.X, ptUnit.Y] = BeforeMap[ptUnit.X, ptUnit.Y];
                Stripmap.SetColor(ptUnit.X, ptUnit.Y, BeforeMap[ptUnit.X, ptUnit.Y]);
                bStartUnit = false;
                CurrUnit++;
                if (CurrUnit >= lstUnit.Count)
                {
                    m_PCSObjList[m_CurrMC].VerifyDoneMessageToPCS(VerifyResult.Done, AfterMap);
                    ClearStatus();
                    m_bVerifying = false;
                    grdButton.IsEnabled = false;
                }
                else
                {
                    NextUnit();

                }
            }
            else SetStrip();
        }

        private void NextUnit()
        {
            ptUnit = new IntPoint(lstUnit[CurrUnit].X, lstUnit[CurrUnit].Y);
            Stripmap.SetCurrPos(ptUnit.X, ptUnit.Y);
            lblUnitX.Content = (ptUnit.X + 1).ToString("00");
            lblUnitY.Content = (ptUnit.Y + 1).ToString("00");
            if (!LoadImage())
            {
                AfterMap[ptUnit.X, ptUnit.Y] = BeforeMap[ptUnit.X, ptUnit.Y];
                Stripmap.SetColor(ptUnit.X, ptUnit.Y, BeforeMap[ptUnit.X, ptUnit.Y]);
                bStartUnit = false;
                CurrUnit++;
                if (CurrUnit >= lstUnit.Count)
                {
                    m_PCSObjList[m_CurrMC].VerifyDoneMessageToPCS(VerifyResult.Done, AfterMap);
                    ClearStatus();
                    m_bVerifying = false;
                    grdButton.IsEnabled = false;
                }
                else
                {
                    NextUnit();

                }
            }
            else
            {
                SetStrip();
                grdButton.IsEnabled = true;
            }
        }

        private bool LoadImage()
        {
            string path = m_Model[m_CurrMC].Path + "\\" + m_Model[m_CurrMC].Group + "\\" + m_Model[m_CurrMC].ModelName + "\\" + m_Model[m_CurrMC].Lot + "\\Image";
            if (!Directory.Exists(path)) return false;

            string ft = String.Format("*[{0:D4}]*X={1:D2} Y={2:D2}*DEF*png", CurrStrip, ptUnit.X + 1, ptUnit.Y + 1);
            imgfiles = Directory.GetFiles(path, ft);
            if (imgfiles.Length >= m_Model[m_CurrMC].UnitBad) return false;
            if (imgfiles.Length > 10)
            {
                imgList.Clear();
                string[] tmp = new string[10];
                //imgfiles.CopyTo(tmp, 0);
                Array.Copy(imgfiles, 0, tmp, 0, 10);
                imgList.ImageSet(tmp);
                imgPage = (int)Math.Ceiling((double)imgfiles.Length / 10.0);
            }
            else
            {
                imgList.Clear();
                imgList.ImageSet(imgfiles);
                imgPage = 1;
            }
            CurrPage = 0;
            bStartUnit = true;
            return true;
        }

        private void NextImage()
        {
            string path = m_Model[m_CurrMC].Path + "\\" + m_Model[m_CurrMC].Group + "\\" + m_Model[m_CurrMC].ModelName + "\\" + m_Model[m_CurrMC].Lot;
            if (!Directory.Exists(path)) return;
            if (imgPage > CurrPage + 1)
            {
                imgList.Clear();
                string[] tmp = new string[10];
                Array.Copy(imgfiles, CurrPage * 10, tmp, 0, 10);
                imgList.ImageSet(tmp);
            }
            else
            {
                imgList.Clear();
                string[] tmp = new string[imgfiles.Length - CurrPage * 10];
                Array.Copy(imgfiles, CurrPage * 10, tmp, 0, tmp.Length);
                imgList.ImageSet(tmp);
            }
        }

        int GetMachineID(string aEquipName)
        {
            for (int i = 0; i < 3; i++)
            {
                if (m_Model[i].MCName == aEquipName)
                    return i;
            }
            return -1;
        }

        public void ClearMCData(int anMC)
        {
            for (int i = 0; i < VerifyQueue.Count; i++)
            {
                if (VerifyQueue[i].MCNo == anMC)
                {
                    VerifyQueue.RemoveAt(i);
                    break;
                }
            }
            if (m_bVerifying && m_CurrMC == anMC)
            {
                ClearStatus();
                m_bVerifying = false;
            }
        }


        void RemotingPCSInterface_DisconnectEquipEvent(string aEquipName)
        {
            int nEquipIndex = 0;
            // DB 연결
            nEquipIndex = GetMachineID(aEquipName);
            if (nEquipIndex == -1)
                return;
            Action a = delegate
            {
                Status[nEquipIndex].Visibility = Visibility.Hidden;
                ClearMCData(nEquipIndex);
                if (m_CurrMC == nEquipIndex && m_bVerifying)
                {
                    ClearStatus();
                    m_bVerifying = false;
                }
            }; this.Dispatcher.Invoke(a);
            
        }

        bool RemotingPCSInterface_ConnectEquipEvent(InspectionEquipDataControl aEquip, int UnitBadCount)
        {
            int nEquipIndex = 0;
            // DB 연결
            nEquipIndex = GetMachineID(aEquip.EquipName);
            if (nEquipIndex == -1)
                return false;
            Action a = delegate
            {
                Status[nEquipIndex].Visibility = Visibility.Visible;
                Status[nEquipIndex].SetModelInfo(aEquip);
                m_Model[nEquipIndex].UnitX = aEquip.UnitX;
                m_Model[nEquipIndex].UnitY = aEquip.UnitY;
                m_Model[nEquipIndex].Group = aEquip.GroupName;
                m_Model[nEquipIndex].ModelName = aEquip.ModelName;
                m_Model[nEquipIndex].Lot = aEquip.LotNum;
                m_Model[nEquipIndex].UnitBad = UnitBadCount;
                ClearMCData(nEquipIndex);
            }; this.Dispatcher.Invoke(a);

            return true;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (m_bVerifying)
            {
                MessageBox.Show("Verify 중에는 종료할 수 없습니다.");
                e.Cancel = true;
            }
            else
            {
                if (!CloseExcute())
                    e.Cancel = true;
            }
        }

        bool CloseExcute()
        {
            if (MessageBox.Show("현재 작업 진행 중입니다. 작업 종료 시 연결이 끊어집니다. 계속 진행 하시겠습니까?", "", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return false;
            }
            else
            {
                if (VerifyThd != null)
                {
                    VerifyThd.Abort();
                }

                Environment.Exit(0);
                return true;
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ClearStatus();
            Setting = new Configs();
            LoadSetting();
            RVSRemotingServer();
            RVSRemotingClient();
            VerifyThd = new Thread(Verify);
            VerifyThd.Start();
        }

        private void LoadSetting()
        {
            string path = Environment.CurrentDirectory + "\\..\\config\\Setting.ini";
            IniFile ini = null;
            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Close();
                ini = new IniFile(path);
                for (int i = 0; i < 3; i++)
                {
                    m_Model[i].MCName = "CAI0" + (4+i).ToString();
                    m_Model[i].Path = "E:\\INSPECT_TEST\\BGA_AVI\\ResultPath\\";
                    m_Model[i].IP = "55.60.103.177";
                    ini.WriteString("INFO", "MC" + (i + 1).ToString(), m_Model[i].MCName);
                    ini.WriteString("INFO", "PATH" + (i + 1).ToString(), m_Model[i].Path);
                    ini.WriteString("INFO", "IP" + (i + 1).ToString(), m_Model[i].IP);
                }
                return;
            }

            ini = new IniFile(path);
            for (int i = 0; i < 3; i++)
            {
                m_Model[i].MCName = ini.ReadString("INFO", "MC" + (i + 1).ToString(), "");
                m_Model[i].Path = ini.ReadString("INFO", "PATH" + (i + 1).ToString(), "");
                m_Model[i].IP = ini.ReadString("INFO", "IP" + (i + 1).ToString(), "55.60.103.17" + (7+i).ToString());
            }
        }

        [STAThread]
        private void RVSRemotingServer()
        {
            BinaryClientFormatterSinkProvider clientProvider = null;
            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();

            serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            IDictionary props = new Hashtable();
            props["port"] = 50006;
            props["typeFilterLevel"] = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            TcpChannel tcpChannel = new TcpChannel(props, clientProvider, serverProvider);

            ChannelServices.RegisterChannel(tcpChannel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RVS.Generic.RemotingPCSInterface), "RVSUri", WellKnownObjectMode.Singleton);
        }

        private void RVSRemotingClient()
        {
            #region 장비별 리모팅 Object 생성

            for (int i = 0; i < 3; i++)
            {
                RemotingRVSInterface m_PCSObj = new RemotingRVSInterface();
                m_PCSObj = (RemotingRVSInterface)Activator.GetObject(typeof(RemotingRVSInterface), "tcp://" + m_Model[i].IP + ":50001/PCSUri");
                m_PCSObjList[i] = m_PCSObj;
            }

            #endregion
        }
    }

    public class Model
    {
        public string IP;
        public string MCName;
        public string Path;
        public int UnitX;
        public int UnitY;
        public string Group;
        public string ModelName;
        public string Lot;
        public int UnitBad;
    }
}
