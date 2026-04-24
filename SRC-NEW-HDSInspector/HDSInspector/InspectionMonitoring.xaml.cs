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

using Common;
using Common.DataBase;
using Common.Drawing;
using Common.Drawing.InspectionInformation;
using HDSInspector.SubWindow;
using IGS;
using IGS.Classes;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Flann;
using PCS;
using PCS.ModelTeaching;
using RVS.Generic;
using RVS.Generic.Class;
using RVS.Generic.Insp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static HDSInspector.InspectProcess;
using static HDSInspector.MainWindow;
using static System.Collections.Specialized.BitVector32;

namespace HDSInspector
{

    /// <summary>   Inspection monitoring.  </summary>
    public partial class InspectionMonitoring : UserControl
    {
        #region Member variables.
        public const int BUFFER = 20;
        private MainWindow m_ptrMainWindow;
        public InspectionResultDataControl TableDataCtrl;
        public LaserWindow LaserDlg;

        private LotManager m_LotManager = new LotManager();
        private VerifyManager m_VerifyManager = new VerifyManager();
        private InspectDataManager m_InspectDataManager = new InspectDataManager();
        public InspectDataManager InspDataManager
        {
            get
            {
                return m_InspectDataManager;
            }
        }
        private InspectResultManager m_InspectResultManager = new InspectResultManager();

        public bool InspectionStarted;
        public bool InspectionThreadStarted;

        private InspectProcess m_InspectThread;
        private LaserProcess m_LaserThread;

        // NG Image Slot
        private List<List<InspectionResultImage>> m_BPNGImage = new List<List<InspectionResultImage>>();
        private List<List<InspectionResultImage>> m_BANGImage = new List<List<InspectionResultImage>>();
        private List<List<InspectionResultImage>> m_CANGImage = new List<List<InspectionResultImage>>();

        private int m_nAllNGCount = 0;

        // NG Image List Index
        private int[] m_BPImageSetIndex = new int[1];
        private int[] m_CAImageSetIndex = new int[1];
        private int[] m_BAImageSetIndex = new int[1];

        private int m_nUnitCountPerShot = 0; //한Shot의 유닛 수

        public InspectBuffer[] m_BAResult = new InspectBuffer[1];
        public InspectBuffer[] m_CAResult = new InspectBuffer[1];
        public InspectBuffer[] m_BPResult = new InspectBuffer[2];

        public InspectBuffer[] m_LastResult = new InspectBuffer[BUFFER];
        public InspectBuffer[] m_TopResult = new InspectBuffer[BUFFER];
        public InspectBuffer[] m_BotResult = new InspectBuffer[BUFFER];
        public InspectBuffer[] LaserResult = new InspectBuffer[BUFFER];
        public int[,] m_BadMap;

        public bool EndLoader=false;
        public bool m_BadStop;
        public int BadX, BadY;
        public bool m_UseConBad;

        public InspectBuffer StaticMarkResult = new InspectBuffer();

        /// <summary>
        public bool[,] BA_Done = new bool[1, BUFFER]; 
        public bool[,] CA_Done = new bool[1, BUFFER];
        public bool[,] BP_Done = new bool[2, BUFFER];

        public string[] m_LaserStr = new string[5];
        public int m_LaserPos = 0;
        public int m_LaserStrPos = 0;
        public int m_LastID = 0;
        
        /// <summary>
        public int[] m_BA_ID = new int[1] { 0 };
        public int[] m_BP_ID = new int[1] { 0 };
        public int[] m_CA_ID = new int[1] { 0 };

        public int m_Mrk_ID = 0;

        public string StripID;
        public string m_CurrentID;
        private InspectResult m_InspectResult = new InspectResult();

        private bool m_bAutoNG = false;

        public string ICS_LotFolderPath = "";

        public bool IsAutoNG
        {
            get { return m_bAutoNG; }
            set { m_bAutoNG = value; }
        }

        public bool IsClosed
        {
            get { return m_bClosed; }
        }
        private bool m_bClosed = false;

        public bool GetLiveImage = false;
        #endregion

        public CamResultCtrl[] CamCtrl = new CamResultCtrl[1];
        public InspectionResultTable ResultTable = new InspectionResultTable();
        #region Constructor & Initializer.
        /// <summary>   Initializes a new instance of the InspectionMonitoring class. </summary>
        public InspectionMonitoring()
        {
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level
                = System.Diagnostics.SourceLevels.Critical;
            InitializeComponent();
            InitializeResultImageSet();
            InitializeEvent();

            TableDataCtrl = ResultTable.TableData;
            for (int i = 0; i < BUFFER; i++)
            {
                m_LastResult[i] = new InspectBuffer();
                LaserResult[i] = new InspectBuffer();
                m_TopResult[i] = new InspectBuffer();
                m_BotResult[i] = new InspectBuffer();
            }
            VisionInterface.InspectDone += VisionInterface_InspectDone;
            VisionInterface.ReceiveUnitOffset += VisionInterface_ReceiveUnitOffset;
            lampMark.Status = false;
            lampPLC.Status = false;

            BP_Done = new bool[MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count, BUFFER];
            m_BPResult = new InspectBuffer[MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count];//2스캔
            m_BP_ID = new int[MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count];

            BA_Done = new bool[MainWindow.Setting.Job.BACount, BUFFER];
            m_BAResult = new InspectBuffer[MainWindow.Setting.Job.BACount];
            m_BA_ID = new int[MainWindow.Setting.Job.BACount];

            CA_Done = new bool[MainWindow.Setting.Job.CACount, BUFFER];
            m_CAResult = new InspectBuffer[MainWindow.Setting.Job.CACount];
            m_CA_ID = new int[MainWindow.Setting.Job.CACount];

            for (int i = 0; i < MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count; i++)
            {
                m_BPResult[i] = new InspectBuffer();
                m_BP_ID[i] = new int();
            }
            for (int i = 0; i < MainWindow.Setting.Job.BACount; i++)
            {
                m_BAResult[i] = new InspectBuffer();
                m_BA_ID[i] = new int();
            }
            for (int i = 0; i < MainWindow.Setting.Job.CACount; i++)
            {
                m_CAResult[i] = new InspectBuffer();
                m_CA_ID[i] = new int();
            }

            BPInspectSkip = new bool[MainWindow.Setting.Job.BPCount];
            BAInspectSkip = new bool[MainWindow.Setting.Job.BACount];
            CAInspectSkip = new bool[MainWindow.Setting.Job.CACount];

            BPInspectDone = new bool[MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count];
            BAInspectDone = new bool[MainWindow.Setting.Job.BACount];
            CAInspectDone = new bool[MainWindow.Setting.Job.CACount];

            for (int i = 0; i < MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount + MainWindow.Setting.Job.BACount; i++)
            {
                MainWindow.AlignOffset_ICS.Add(new List<SectionInfo>());
            }
            for (int i = 0; i < MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count + MainWindow.Setting.Job.CACount + MainWindow.Setting.Job.BACount; i++)
            {
                ConcurrentQueue<object> Queue1 = new ConcurrentQueue<object>();
                ResultQueue.Add(Queue1);
            }
            for (int g = 0; g < QueueBuffer; g++)
            {
                List<bool> bQueue = new List<bool>();
                List<ConcurrentQueue<object>> GruopQueue = new List<ConcurrentQueue<object>>();
                List<ConcurrentQueue<object>> GruopQueue1 = new List<ConcurrentQueue<object>>();
                for (int i = 0; i < MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count + MainWindow.Setting.Job.CACount + MainWindow.Setting.Job.BACount; i++)
                {
                    bQueue.Add(false);
                    GruopQueue.Add(new ConcurrentQueue<object>());
                    GruopQueue1.Add(new ConcurrentQueue<object>());
                }
                Savequeue.bWorkDone.Add(bQueue);
                Savequeue.ImageSave_Queue.Add(GruopQueue);
                Savequeue.ICS_ResultQueue.Add(GruopQueue1);
            }
        }

        private void SetCamControl(int anType, bool UseCA, bool UseBA, bool DualLaser)
        {
            for (int i = 0; i < 2; i++) GridInsResult.RowDefinitions.Add(new RowDefinition());

            for (int i = 0; i < MainWindow.Setting.Job.BPCount; i++) GridInsResult.ColumnDefinitions.Add(new ColumnDefinition());
            /////////////////////////////BP & Result//////////////////////////            
            if (anType == 0)
            {
                for (int i = 0; i < MainWindow.Setting.Job.BPCount; i++)
                {
                    Grid.SetRow(CamCtrl[i], 0);
                    Grid.SetColumn(CamCtrl[i], i);
                    GridInsResult.Children.Add(CamCtrl[i]);
                }
                Grid.SetRow(ResultTable, 1);
                Grid.SetColumn(ResultTable, 0);
                Grid.SetColumnSpan(ResultTable, MainWindow.Setting.Job.BPCount);
                GridInsResult.Children.Add(ResultTable);
            }
            else
            {
                for (int i = 0; i < MainWindow.Setting.Job.BPCount; i++)
                {
                    Grid.SetRow(CamCtrl[i], 1);
                    Grid.SetColumn(CamCtrl[i], i);
                    GridInsResult.Children.Add(CamCtrl[i]);
                }
                Grid.SetRow(ResultTable, 0);
                Grid.SetColumn(ResultTable, 0);
                Grid.SetColumnSpan(ResultTable, MainWindow.Setting.Job.BPCount);
                GridInsResult.Children.Add(ResultTable);
            }
            /////////////////////////////BA & CA//////////////////////////
            if (anType == 0)
            {
                for (int i = 0; i < MainWindow.Setting.Job.CACount; i++) GridInsTypeUI1.ColumnDefinitions.Add(new ColumnDefinition());
                for (int i = 0; i < MainWindow.Setting.Job.BACount; i++) GridInsTypeUI2.ColumnDefinitions.Add(new ColumnDefinition());
                
                for (int j = 0; j < MainWindow.Setting.Job.CACount; j++)
                {
                    if (!UseCA && j % 2 == 1)
                    {
                        System.Windows.Controls.Image imgCA = new System.Windows.Controls.Image();
                        BitmapImage bi3 = new BitmapImage();
                        bi3.BeginInit();
                        bi3.UriSource = new Uri("/Images/CA_USE2.png", UriKind.Relative);
                        bi3.EndInit();
                        imgCA.Source = bi3;
                        imgCA.Stretch = System.Windows.Media.Stretch.Fill;
                        Grid.SetColumn(imgCA, j);
                        GridInsTypeUI1.Children.Add(imgCA);
                    }
                    else
                    {
                        Grid.SetColumn(CamCtrl[j + MainWindow.Setting.Job.BPCount], j);
                        GridInsTypeUI1.Children.Add(CamCtrl[j + MainWindow.Setting.Job.BPCount]);
                    }
                    if (!UseCA && j % 2 == 1) CamCtrl[MainWindow.Setting.Job.BPCount + j].Visibility = Visibility.Hidden;
                }
                for (int j = 0; j < MainWindow.Setting.Job.BACount; j++)
                { 
                    if (!UseBA && j % 2 == 1)
                    {
                        System.Windows.Controls.Image imgBA = new System.Windows.Controls.Image();
                        BitmapImage bi3 = new BitmapImage();
                        bi3.BeginInit();
                        bi3.UriSource = new Uri("/Images/BA_USE2.png", UriKind.Relative);
                        bi3.EndInit();
                        imgBA.Source = bi3;
                        imgBA.Stretch = System.Windows.Media.Stretch.Fill;
                        Grid.SetColumn(imgBA, j);
                        GridInsTypeUI2.Children.Add(imgBA);
                    }
                    else
                    {
                        Grid.SetColumn(CamCtrl[j + MainWindow.Setting.Job.CACount + MainWindow.Setting.Job.BPCount], j);
                        GridInsTypeUI2.Children.Add(CamCtrl[j + MainWindow.Setting.Job.CACount + MainWindow.Setting.Job.BPCount]);
                    }
                    if (!UseBA && j % 2 == 1) CamCtrl[MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount + j].Visibility = Visibility.Hidden;
                }
            }
            else
            {
                for (int i = 0; i < MainWindow.Setting.Job.BACount; i++) GridInsTypeUI1.ColumnDefinitions.Add(new ColumnDefinition());
                for (int i = 0; i < MainWindow.Setting.Job.CACount; i++) GridInsTypeUI2.ColumnDefinitions.Add(new ColumnDefinition());

                for (int j = 0; j < MainWindow.Setting.Job.CACount; j++)
                {
                    if (!UseCA && j % 2 == 1)
                    {
                        System.Windows.Controls.Image imgCA = new System.Windows.Controls.Image();
                        BitmapImage bi3 = new BitmapImage();
                        bi3.BeginInit();
                        bi3.UriSource = new Uri("/Images/CA_USE2.png", UriKind.Relative);
                        bi3.EndInit();
                        imgCA.Source = bi3;
                        imgCA.Stretch = System.Windows.Media.Stretch.Fill;
                        Grid.SetColumn(imgCA, j);
                        GridInsTypeUI2.Children.Add(imgCA);
                    }
                    else
                    {
                        Grid.SetColumn(CamCtrl[j + MainWindow.Setting.Job.BPCount], j);
                        GridInsTypeUI2.Children.Add(CamCtrl[j + MainWindow.Setting.Job.BPCount]);
                    }
                    if (!UseCA && j % 2 == 1) CamCtrl[MainWindow.Setting.Job.BPCount + j].Visibility = Visibility.Hidden;
                }
                for (int j = 0; j < MainWindow.Setting.Job.BACount; j++)
                {
                    if (!UseBA && j % 2 == 1)
                    {
                        System.Windows.Controls.Image imgBA = new System.Windows.Controls.Image();
                        BitmapImage bi3 = new BitmapImage();
                        bi3.BeginInit();
                        bi3.UriSource = new Uri("/Images/BA_USE2.png", UriKind.Relative);
                        bi3.EndInit();
                        imgBA.Source = bi3;
                        imgBA.Stretch = System.Windows.Media.Stretch.Fill;
                        Grid.SetColumn(imgBA, j);
                        GridInsTypeUI1.Children.Add(imgBA);
                    }
                    else
                    {
                        Grid.SetColumn(CamCtrl[j + MainWindow.Setting.Job.CACount + MainWindow.Setting.Job.BPCount], j);
                        GridInsTypeUI1.Children.Add(CamCtrl[j + MainWindow.Setting.Job.CACount + MainWindow.Setting.Job.BPCount]);
                    }
                    if (!UseBA && j % 2 == 1) CamCtrl[MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount + j].Visibility = Visibility.Hidden;
                }
            }        
            
    
       
            if (DualLaser) imgLaserUse.Visibility = Visibility.Hidden;
        }

        private void InitializeEvent()
        {
            for (int i = 0; i < MainWindow.Setting.Job.CACount + MainWindow.Setting.Job.BACount + MainWindow.Setting.Job.BPCount; i++)
            {
                int camIndex = i; // 클로저 문제 방지
                for (int j = 0; j < CamCtrl[camIndex].Image.Count; j++)
                {
                    int ImageIndex = j;
                    CamCtrl[camIndex].Image[ImageIndex].MouseMove += (index, x, y, gv) => Image_MouseMove(camIndex, x, y, gv);
                }
                CamCtrl[camIndex].btnLC.Click += BtnLC_Click;
                CamCtrl[camIndex].btnSaveLight.Click += BtnSave_Click;
                CamCtrl[camIndex].btnCancelLight.Click += BtnCancel_Click;
            }

            this.btnRead2D.Click += BtnRead2D_Click;
            this.ReportCtrl.OnAccept += ReportCtrl_OnAccept;
            this.ReportCtrl.OnRequest += IgsCtrl_OnRequest;
            this.userCtrl.OnRequest += IgsCtrl_OnRequest;

            this.btnPOPMenu.Click += BtnPOPMenu_Click;
            this.btnIssue.Click += BtnIssue_Click;
        }

        public object IgsCtrl_OnRequest(EventType eventType, object param = null)
        {
            try
            {
                if (eventType == EventType.MAKE_LOSS)
                    ResultTable.WriteResultLoss(MainWindow.CurrentModel.UseVerify);
                else if (eventType == EventType.GET_LASTTIME)
                {
                    if (m_ptrMainWindow.popManager.GetCurStateInfo(out STATE_TABLE_DATA curState) && curState != null)
                    {
                        string curOrder = m_ptrMainWindow.POPStartPara.strLot;

                        DateTime returnTime;
                        if (curState.ORDER == curOrder && curState.STATE == "STOP" && curState.EXC_CD != "")
                            returnTime = DateTime.ParseExact(curState.EXC_START_TIME, "yyyyMMddHHmmss", null);
                        else
                            returnTime = InspectProcess.pop_LastWorkTime == null ? DateTime.Now : InspectProcess.pop_LastWorkTime;

                        return returnTime;
                    }
                    else
                        return InspectProcess.pop_LastWorkTime == null ? DateTime.Now : InspectProcess.pop_LastWorkTime;
                }
                else if (eventType == EventType.GET_INPUT_COUNT)
                {
                    return TableDataCtrl == null ? 0 : TableDataCtrl.TotalUnits;
                }
                else if (eventType == EventType.SET_LAST_USER)
                {
                    string lastUser = (string)param;
                    MainWindow.Setting.General.POP_LastUser = lastUser;
                    MainWindow.Setting.General.Save();
                }
                else if (eventType == EventType.GET_LAST_USER)
                {
                    return MainWindow.Setting.General.POP_LastUser;
                }

                return null;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("IGS - IgsCtrl_OnRequest Exception: {0}", ex.Message));
                return null;
            }
        }

        private void ReportCtrl_OnAccept()
        {
            m_ptrMainWindow.popStateCtrl.UpdateState(true);
        }

        private void BtnPOPMenu_Click(object sender, RoutedEventArgs e)
        {
            btnPOPMenu.Background = new SolidColorBrush(Colors.RoyalBlue);
            btnIssue.Background = new SolidColorBrush(Colors.Gray);

            grdPOP.Visibility = Visibility.Visible;
            grdWarning.Visibility = Visibility.Hidden;
        }

        private void BtnIssue_Click(object sender, RoutedEventArgs e)
        {
            btnPOPMenu.Background = new SolidColorBrush(Colors.Gray);
            btnIssue.Background = new SolidColorBrush(Colors.RoyalBlue);

            grdPOP.Visibility = Visibility.Hidden;
            grdWarning.Visibility = Visibility.Visible;
        }

        private void BtnTune2D_Click(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.Setting.General.UseIDReader) return;

            this.Cursor = System.Windows.Input.Cursors.Wait;

            if (!reader2D.Tune())
            {
                this.Cursor = System.Windows.Input.Cursors.Arrow;
                MessageBox.Show("Tunning 실패");
                return;
            }
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void BtnGrab2D_Click(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.Setting.General.UseIDReader) return;

            this.Cursor = System.Windows.Input.Cursors.Wait;

            if (!reader2D.Grab())
            {
                this.Cursor = System.Windows.Input.Cursors.Arrow;
                MessageBox.Show("Grab 실패");
                return;
            }
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void BtnRead2D_Click(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.Setting.General.UseIDReader) return;

            if (MainWindow.Setting.SubSystem.PLC.UsePLC)
            {
                if (!m_ptrMainWindow.PCSInstance.PlcDevice.RequestID())
                {
                    MessageBox.Show("ID 확인 모드가 아닙니다.");
                    return;
                }
            }
            this.Cursor = System.Windows.Input.Cursors.Wait;

            string ID = reader2D.ReadID();
            lbl2D.Content = ID;

            this.Cursor = System.Windows.Input.Cursors.Arrow;
            this.Focus();
        }

        public void Init2DReaderUI()
        {
            lbl2D.Content = "";
            this.reader2D.Grab();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            int nTag = Convert.ToInt32(((Button)sender).Tag.ToString());
            CamCtrl[nTag].grdLC.Visibility = Visibility.Hidden;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            int nTag = Convert.ToInt32(((Button)sender).Tag.ToString());
            int[] val = new int[MainWindow.CurrentModel.LightValue.GetLength(1)];
            CamCtrl[nTag].grdLC.Visibility = Visibility.Hidden;
            val = CamCtrl[nTag].lc.GetValues();

            MainWindow.IndexInfo IndexInfo = MainWindow.Convert_CheckVisionSlave(nTag);
            MainWindow.LightInfo LightInfo = MainWindow.Convert_LightIndex(IndexInfo.CategorySurface, IndexInfo.Index);////////10개의 DB 버퍼공간을 활용하는 방식으로 변경

            for (int i = 0; i < MainWindow.CurrentModel.LightValue.GetLength(1); i++) MainWindow.CurrentModel.LightValue[LightInfo.ValueIndex, i] = val[i];
            DeviceController.SetLightValue(LightInfo.ValueIndex, val);
            DeviceController.LightDevice.SetLight(LightInfo.LightIndex, LightInfo.ValueIndex);
            DeviceController.SaveLightValue(MainWindow.CurrentModel.Code, MainWindow.CurrentModel.LightValue);
        }

        private void BtnLC_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentModel == null) return;
            int nTag = Convert.ToInt32(((Button)sender).Tag.ToString());           
            CamCtrl[nTag].grdLC.Visibility = Visibility.Visible;

            MainWindow.IndexInfo IndexInfo = MainWindow.Convert_CheckVisionSlave(nTag);
            MainWindow.LightInfo LightInfo = MainWindow.Convert_LightIndex(IndexInfo.CategorySurface, IndexInfo.Index);////////10개의 DB 버퍼공간을 활용하는 방식으로 변경

            CamCtrl[nTag].lc.LoadLightData(LightInfo.LightIndex, LightInfo.ValueIndex, MainWindow.CurrentModel.LightValue, MainWindow.Setting.SubSystem.Light.Channel);
        }

        public string ReadID()
        {
            string val = reader2D.ReadID();
            val = val.Replace("\r", "");
            Action a = delegate
            {
                lbl2D.Content = val;
            }; this.Dispatcher.Invoke(a);
            return val;
        }

        public string ReadID(int plcID)
        {
            string val = reader2D.ReadID();
            val = val.Replace("\r", "");
            Action a = delegate
            {
                lbl2D.Content = val;
            }; this.Dispatcher.Invoke(a);

            return val;
        }

        public string Log2DMark(string id, int plcID)
        {
            string path = MainWindow.Setting.General.ResultPath + "\\" + MainWindow.CurrentGroup.Name + "\\"
                + MainWindow.CurrentModel.Name + "\\" + m_ptrMainWindow.currOrder + "\\2D Mark log(temp)\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            reader2D.DownloadRecentImg(m_ptrMainWindow.currOrder, id);

            if (!DataMatrixCode.checkAndLogReaderData(reader2D.GetRecentMatImage(), id, m_ptrMainWindow.currOrder, plcID)) id = "";

            return id;
        }

        public void ResetCycleTime()
        {
            if (m_InspectThread != null)
                m_InspectThread.ResetTimer();

            if (m_LaserThread != null)
                m_LaserThread.ResetTimer();
        }

        //void ImageBA1_MouseMove(double x, double y, byte[] gv)
        //{
        //    if (MainWindow.Setting.SubSystem.IS.FGType[3] == 6)
        //        CamCtrl[3].SetGV(x, y, gv[0], gv[1], gv[2]);
        //    else
        //        CamCtrl[3].SetGV(x, y, gv[0], 0, 0);
        //}
        //
        //void ImageBA2_MouseMove(double x, double y, byte[] gv)
        //{
        //    if (MainWindow.Setting.SubSystem.IS.FGType[4] == 6)
        //        CamCtrl[4].SetGV(x, y, gv[0], gv[1], gv[2]);
        //    else
        //        CamCtrl[4].SetGV(x, y, gv[0], 0, 0);
        //}
        //
        //void ImageBondPad_MouseMove(double x, double y, byte[] gv)
        //{
        //    if (MainWindow.Setting.SubSystem.IS.FGType[0] == 6)
        //        CamCtrl[0].SetGV(x, y, gv[0], gv[1], gv[2]);
        //    else
        //        CamCtrl[0].SetGV(x, y, gv[0], 0, 0);
        //}
        //
        //void ImageCA1_MouseMove(double x, double y, byte[] gv)
        //{
        //    if (MainWindow.Setting.SubSystem.IS.FGType[1] == 6)
        //        CamCtrl[1].SetGV(x, y, gv[0], gv[1], gv[2]);
        //    else
        //        CamCtrl[1].SetGV(x, y, gv[0], 0, 0);
        //}
        //void ImageCA2_MouseMove(double x, double y, byte[] gv)
        //{
        //    if (MainWindow.Setting.SubSystem.IS.FGType[2] == 6)
        //        CamCtrl[2].SetGV(x, y, gv[0], gv[1], gv[2]);
        //    else
        //        CamCtrl[2].SetGV(x, y, gv[0], 0, 0);
        //}

        ////////////////////////////////////////
        void Image_MouseMove(int nIndex, double x, double y, byte[] gv)
        {
            int FrameGrabberIndex = VID.CalcIndex(nIndex);
            if (FrameGrabberIndex < 0) return;
            if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] >= 6)
                CamCtrl[nIndex].SetGV(x, y, gv[0], gv[1], gv[2]);
            else
                CamCtrl[nIndex].SetGV(x, y, gv[0], 0, 0);
        }
        /// <summary>
        /// ////////////////////////////////////////
        /// </summary>
        /// <param name="obj"></param>
        void GetInspectResult(object obj)
        {
            object[] param = (object[])obj;
            int ID = (int)param[0];
            int grabside = (int)param[1];

            MainWindow.Log("IS", SeverityLevel.DEBUG, string.Format("ID : {0} GetInspectResult 수행", ID), true);
            if (0 <= ID && ID < VID.CA1)
            {
                GetInspectResult_BP(ID, grabside);
                ShotChangeBP((ID - VID.BP1) * 2 + grabside);
                InspectProcess.BPInspectDone[(ID - VID.BP1) * 2 + grabside] = true;
                BP_Done[(ID - VID.BP1) * 2 + grabside, m_BP_ID[(ID - VID.BP1) * 2 + grabside] % 20] = true;
            }
            else if (ID >= VID.CA1 && ID < VID.BA1)
            {
                GetInspectResult_CA(ID, grabside);
                ShotChagneCA(ID - VID.CA1 + 2 * grabside);
                InspectProcess.CAInspectDone[ID - VID.CA1 + 2 * grabside] = true;
                CA_Done[ID - VID.CA1 + 2 * grabside, m_CA_ID[ID - VID.CA1 + 2 * grabside] % 20] = true;
            }
            else
            {
                GetInspectResult_BA(ID, grabside);
                ShotChangeBA(ID - VID.BA1 + 2 * grabside);
                InspectProcess.BAInspectDone[ID - VID.BA1 + 2 * grabside] = true;
                BA_Done[ID - VID.BA1 + 2 * grabside, m_BA_ID[ID - VID.BA1 + 2 * grabside] % 20] = true;
            }
        }

        void VisionInterface_InspectDone(int ID, int grabside)
        {
            if (!InspectionThreadStarted) return;
            object[] objects = new object[] { ID, grabside };
            Task taskInspectResult = new Task(GetInspectResult, objects);
            taskInspectResult.Start();
        }

        void btnClosePause_Click(object sender, RoutedEventArgs e)
        {
            m_ptrMainWindow.m_bPause2 = false;
        }

        /// <summary>   Initializes the result image set. </summary>
        private void InitializeResultImageSet()
        {
            m_BPImageSetIndex = new int[MainWindow.Setting.Job.BPCount];
            m_CAImageSetIndex = new int[MainWindow.Setting.Job.CACount];
            m_BAImageSetIndex = new int[MainWindow.Setting.Job.BACount];
            CamCtrl = new CamResultCtrl[MainWindow.Setting.Job.CACount + MainWindow.Setting.Job.BACount + MainWindow.Setting.Job.BPCount];
            ////BP는 분리 후에 맨 앞번호로 할지 뒤로 할지 고민하자
            //CamCtrl[0] = new CamResultCtrl(MainWindow.Setting.Job.BPResultImageCount);
            //CamCtrl[0].SetID(0);
            //ResultTable = new InspectionResultTable();
            //CamCtrl[0].txtTitle.Text = "본드패드";
            //
            //for (int j = 0; j < MainWindow.Setting.Job.BPResultImageCount; j++)
            //{
            //    List<InspectionResultImage> TempResultimages = new List<InspectionResultImage>();
            //    m_BPNGImage.Add(TempResultimages);
            //}
            ////BP
            //for (int i = 0; i < 1/*MainWindow.Setting.Job.BPCount*/; i++)/////////우선 하나만 쓰자.
            //{
            //    for (int j = 0; j < CamCtrl[i].GridResultimage.Children.Count; j++)
            //    {
            //        InspectionResultImage image = (InspectionResultImage)CamCtrl[i].GridResultimage.Children[j];
            //        m_BPNGImage[i].Add(image);
            //    }
            //}

            int IndexBP = 1;
            int IndexCA = 1;
            int IndexBA = 1;
            //CA
            for (int i = 0; i < MainWindow.Setting.Job.CACount + MainWindow.Setting.Job.BACount + MainWindow.Setting.Job.BPCount; i++)
            {
                string str = "";
                int ResultImagecount = 6;
                if (MainWindow.Setting.Job.BPCount > i)
                {
                    ResultImagecount = MainWindow.Setting.Job.BPResultImageCount;
                    str = "본드패드" + (IndexBP++);
                }
                else if (MainWindow.Setting.Job.BPCount <= i && MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount > i)
                {
                    ResultImagecount = MainWindow.Setting.Job.CAResultImageCount;
                    str = "CA 외관" + (IndexCA++);
                }
                else
                {
                    ResultImagecount = MainWindow.Setting.Job.BAResultImageCount;
                    str = "BA 외관" + (IndexBA++);
                }

                CamCtrl[i] = new CamResultCtrl(ResultImagecount);
                CamCtrl[i].SetID(i);
                CamCtrl[i].txtTitle.Text = str;
            }

            for (int j = 0; j < MainWindow.Setting.Job.BPCount; j++)
            {
                List<InspectionResultImage> TempResultimages = new List<InspectionResultImage>();
                m_BPNGImage.Add(TempResultimages);
            }

            for (int j = 0; j < MainWindow.Setting.Job.CACount; j++)
            {
                List<InspectionResultImage> TempResultimages = new List<InspectionResultImage>();
                m_CANGImage.Add(TempResultimages);
            }

            for (int j = 0; j < MainWindow.Setting.Job.BACount; j++)
            {
                List<InspectionResultImage> TempResultimages = new List<InspectionResultImage>();
                m_BANGImage.Add(TempResultimages);
            }

            //BP
            for (int i = 0; i < MainWindow.Setting.Job.BPCount; i++)
            {
                for (int j = 0; j < CamCtrl[i].GridResultimage.Children.Count; j++)
                {
                    InspectionResultImage image = (InspectionResultImage)CamCtrl[i].GridResultimage.Children[j];
                    m_BPNGImage[i].Add(image);
                }
            }
            //CA
            for (int i = 0; i < MainWindow.Setting.Job.CACount; i++)
            {
                for (int j = 0; j < CamCtrl[i + MainWindow.Setting.Job.BPCount].GridResultimage.Children.Count; j++)
                {
                    InspectionResultImage image = (InspectionResultImage)CamCtrl[i + MainWindow.Setting.Job.BPCount].GridResultimage.Children[j];
                    m_CANGImage[i].Add(image);
                }
            }
            //BA
            for (int i = 0; i < MainWindow.Setting.Job.BACount; i++)
            {
                for (int j = 0; j < CamCtrl[i + MainWindow.Setting.Job.CACount + MainWindow.Setting.Job.BPCount].GridResultimage.Children.Count; j++)
                {
                    InspectionResultImage image = (InspectionResultImage)CamCtrl[i + MainWindow.Setting.Job.CACount + MainWindow.Setting.Job.BPCount].GridResultimage.Children[j];
                    m_BANGImage[i].Add(image);
                }
            }
        }
        #endregion

        public void SetWindow(int anType, bool UseCA2, bool UseBA2, bool DualLaser)
        {
            SetCamControl(anType, UseCA2, UseBA2, DualLaser);
            m_ptrMainWindow = Application.Current.MainWindow as MainWindow;
            ResultTable.Init();
            TableDataCtrl = ResultTable.TableData;
            //if (!MainWindow.Setting.General.UsePOP)
            //    grdPOP.Visibility = Visibility.Hidden;
        }

        public bool StartInspection(bool test = false)
        {
            if (!CheckLotExist())
            {
                return false;
            }

            if (MainWindow.Setting.General.UsePOP && !m_ptrMainWindow.POPStartReport())
                return false;

            m_BadStop = false;

            TableDataCtrl.SaveStartTime(TableDataCtrl.StartTime);
            TableDataCtrl.RunTime = DateTime.Now - TableDataCtrl.StartTime;

            bool bIsSimulationMode = !m_ptrMainWindow.PCSInstance.IsOpenPlc; // hs - plc 연결 여부에 따라 Simulation 모드 결정

            if (MainWindow.Setting.SubSystem.Laser.UseLaser && MainWindow.CurrentModel.UseMarking)
            {
                LaserDlg.InitModel(true, imgAlign1, imgAlign2);
            }
            if (MainWindow.CurrentModel.UseAI)
            {
                if (!Make_ICSData_Information()) return false;
            }
            m_ptrMainWindow.SkipData = false;
            if (MainWindow.CurrentModel.ITS.UseITS && MainWindow.CurrentModel.ITS.SKIPDATA)
            {
                if (!ReadSkipData())
                {
                    MessageBox.Show("SkipData를 읽을 수 없습니다. 검사 시작 실패.");
                    MainWindow.Log("PCS", SeverityLevel.ERROR, "SkipData를 읽을 수 없습니다. 검사 시작 실패.", true);
                    return false;
                }
            }            
            if (!m_ptrMainWindow.TeachingDlg.SendVisionData(true, GetLiveImage, 2))
            {
                m_ptrMainWindow.ProgressWindow.StopProgress(-1);
                MessageBox.Show("Vision 정보 전송이 실패하였습니다.", "Error");
                MainWindow.Log("VISION", SeverityLevel.ERROR, "Vision 정보 전송에 실패하였습니다.", true);

                return false;
            }           
            InitDisplay();      
            return StartInspectThread(bIsSimulationMode);
        }

        public bool Make_ICSData_Information()
        {
            XoutInfo XOutInfo = new XoutInfo();
            XOutInfo.NGCount = MainWindow.CurrentModel.AutoNG;
            XOutInfo.NGContinueX = MainWindow.CurrentModel.AutoNGY;
            XOutInfo.NGContinueY = MainWindow.CurrentModel.AutoNGX;
            XOutInfo.NGBlockCount = MainWindow.CurrentModel.AutoNGBlock;
            XOutInfo.NGOutUnitCount = MainWindow.CurrentModel.AutoNGOuterY;
            XOutInfo.NGOutUnitXMode = MainWindow.CurrentModel.AutoNGOuterYMode;
            XOutInfo.NGOutDivideUnitCount = MainWindow.CurrentModel.AutoNGOuterDivY;
            XOutInfo.NGMatrix = new List<System.Windows.Point>();
            foreach (var item in MainWindow.CurrentModel.AutoNGMatrixdata)
                XOutInfo.NGMatrix.Add(new System.Windows.Point(item.V2,item.V1));

            List<AutoNGInformation> AutoNGInfo = new List<AutoNGInformation>();
            for (int i = 1; i < MainWindow.NG_Info.Size; i++)
            {
                AutoNGInfo.Add(new AutoNGInformation(MainWindow.NG_Info.GetItem(i).Name, MainWindow.NG_Info.GetItem(i).MES_Fail, MainWindow.CurrentModel.ScrabInfo[i - 1]));
            }

            MainWindow.ModelInfo = new Modelinformation
            {
                UnitCountX = MainWindow.CurrentModel.Strip.UnitColumn,//UnitCountX,
                UnitCountY = MainWindow.CurrentModel.Strip.UnitRow,//UnitCountY,
                UnitPitchX = MainWindow.CurrentModel.Strip.UnitWidth,
                UnitPitchY = MainWindow.CurrentModel.Strip.UnitHeight,
                BlockCountX = MainWindow.CurrentModel.Strip.Block,//BlockCountX,
                BlockCountY = 1,//BlockCountY,
                BlockDistX = MainWindow.CurrentModel.Strip.BlockGap,//BlockPitchX,
                BlockDistY = 0,//BlockPitchY,
                StripSizeX = Convert.ToInt32(MainWindow.CurrentModel.Strip.Width * 1000),
                StripSizeY = Convert.ToInt32(MainWindow.CurrentModel.Strip.Height * 1000),
                XOutInfo = XOutInfo,
                AutoNGInfo = AutoNGInfo
            };
            MainWindow.SectionData_ICS.Clear();
            for (int i = 0; i < MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount + MainWindow.Setting.Job.BACount; i++)
            {
                List<List<SectionInfo>> GruopSectionInfo = new List<List<SectionInfo>>();
                for (int j = 0; j < Enum.GetValues(typeof(SectionArea)).Length; j++)
                {
                    GruopSectionInfo.Add(new List<SectionInfo>());
                }
                MainWindow.SectionData_ICS.Add(GruopSectionInfo);
            }

            ICS_LotFolderPath = String.Format(@"{0}/{1}/{2}/{3}/", MainWindow.Setting.General.ICSFilePath, TableDataCtrl.GroupName, TableDataCtrl.ModelName, TableDataCtrl.LotNo);
            for (int nCurrVision = 0; nCurrVision < m_ptrMainWindow.TeachingDlg.TeachingViewers.Length; nCurrVision++)
            {
                IndexInfo IndexInfo = m_ptrMainWindow.Convert_ViewIndexToVisionIndex(nCurrVision);
                m_ptrMainWindow.TeachingDlg.MakeSectionDataICS(nCurrVision, ref MainWindow.SectionData_ICS, ref MainWindow.BP2Section_info);
                if (IndexInfo.CategorySurface == CategorySurface.BP)
                {              
                    if (BPInspectSkip[IndexInfo.Index / VID.BP_ScanComplete_Count]) continue;
                }
                else if (IndexInfo.CategorySurface == CategorySurface.CA)
                {
                    if (CAInspectSkip[IndexInfo.Index]) continue;
                }
                else if (IndexInfo.CategorySurface == CategorySurface.BA)
                {
                    if (BAInspectSkip[IndexInfo.Index]) continue;
                }
       
                if (!(IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave))
                {
                    string CornerFilePath = String.Format(@"{0}{1}/{2}/{3}-Corner.bmp", MainWindow.Setting.General.ModelPath, TableDataCtrl.GroupName, TableDataCtrl.ModelName, (int)IndexInfo.Surface);
                    if (!Directory.Exists(ICS_LotFolderPath)) Common.DirectoryManager.CreateDirectory(ICS_LotFolderPath);
                    if (File.Exists(CornerFilePath))
                    {
                        if (File.Exists(ICS_LotFolderPath + IndexInfo.Surface + ".bmp"))
                        {
                            File.Delete(ICS_LotFolderPath + IndexInfo.Surface + ".bmp");
                        }
                        File.Copy(CornerFilePath, ICS_LotFolderPath + IndexInfo.Surface + ".bmp");
                    }
                    else
                    {
                        MessageBox.Show(string.Format("{0}의 Corner.bmp 영상파일이 없습니다. 검사 종료", IndexInfo.Surface));
                        return false;
                    }
                }

                bool TEST = false;
                if (TEST)
                {
                    if (IndexInfo.CategorySurface == CategorySurface.BP && !IndexInfo.Slave) continue;
                    string SIDE = "TOP";
                    if (IndexInfo.CategorySurface == CategorySurface.BA) SIDE = "BOTTOM";
                    var Data = new ICS_Data
                    {
                        ModelName = TableDataCtrl.ModelName,
                        LOT_NO = TableDataCtrl.LotNo,
                        Side = SIDE,
                        MachineInfo = new MachineInfo
                        {
                            Name = MainWindow.Setting.General.MachineName,//MachinName,
                            ResX = MainWindow.Setting.SubSystem.IS.CamResolutionY[(int)IndexInfo.CategorySurface],//ResX,
                            ResY = MainWindow.Setting.SubSystem.IS.CamResolutionX[(int)IndexInfo.CategorySurface],//ResY,
                        },
                        ModelInfo = new Modelinformation(),
                        AlignOffset = new List<SectionInfo>(),
                        UnitInfo = new List<SectionInfo>(),
                        OuterInfo = new List<SectionInfo>(),
                        MaterialInfo = new List<SectionInfo>(),
                        UnitDefInfo = new List<DefInfo>(),
                        OuterDefInfo = new List<DefInfo>(),
                        MaterialDefInfo = new List<DefInfo>(),
                    };
                    Data.ModelInfo = MainWindow.ModelInfo;
                    Data.UnitInfo = MainWindow.SectionData_ICS[IndexInfo.ScanIndex][(int)SectionArea.Unit].ToList();
                    Data.OuterInfo = MainWindow.SectionData_ICS[IndexInfo.ScanIndex][(int)SectionArea.Outer].ToList();
                    Data.MaterialInfo = MainWindow.SectionData_ICS[IndexInfo.ScanIndex][(int)SectionArea.Material].ToList();
  
                    string strStripFolderPath = String.Format(@"{0}/{1}/", ICS_LotFolderPath, 0000);
                    if (!Directory.Exists(strStripFolderPath)) Common.DirectoryManager.CreateDirectory(strStripFolderPath);
                
                    var option = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(
                           UnicodeRanges.BasicLatin, // 일반 영숫자
                           UnicodeRanges.HangulCompatibilityJamo, // 이하 한글 관련 문자
                           UnicodeRanges.HangulJamo,
                           UnicodeRanges.HangulJamoExtendedA,
                           UnicodeRanges.HangulJamoExtendedB,
                           UnicodeRanges.HangulSyllables
                           ),
                        WriteIndented = true
                    };
                    string json = System.Text.Json.JsonSerializer.Serialize(Data, option);
                    File.WriteAllText(strStripFolderPath + IndexInfo.Surface + ".json", json);
                }
            }
            return true;
        }
        //public static CancellationTokenSource _imageSaveCancellationTokenSource;
        public bool StartInspectThread(bool abTestMode)
        {
            bool ret = true; ;
            InspectionStarted = true;
            InspectionThreadStarted = true;
            if (m_InspectThread != null)
            {
                m_InspectThread.ClearThread();
                m_InspectThread = null;
            }

            for (int i = 0; i < ResultQueue.Count; i++)
            {
                while (ResultQueue[i].TryDequeue(out _)) { } // 큐를 안전하게 비움
            }

            #region Save_Queue 초기화
            for (int g = 0; g < QueueBuffer; g++)
            {
                var groupQueue = Savequeue.ImageSave_Queue[g];            
                for (int i = 0; i < groupQueue.Count; i++)
                {
                    while (groupQueue[i].TryDequeue(out _)) { } // 각 하위 큐를 안전하게 비움
                }
                var groupQueue1 = Savequeue.ICS_ResultQueue[g];
                for (int i = 0; i < groupQueue1.Count; i++)
                {
                    while (groupQueue1[i].TryDequeue(out _)) { } // 각 하위 큐를 안전하게 비움
                }
                
                for (int i = 0; i < Savequeue.bWorkDone[g].Count; i++)
                {
                    MainWindow.IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(i);
                    if (IndexInfo.CategorySurface == CategorySurface.BP)
                    {
                        if(BPInspectSkip[IndexInfo.Index/ VID.BP_ScanComplete_Count]) Savequeue.bWorkDone[g][i] = true;
                        else Savequeue.bWorkDone[g][i] = false;
                    }
                    else if (IndexInfo.CategorySurface == CategorySurface.CA)
                    {
                        if (CAInspectSkip[IndexInfo.Index]) Savequeue.bWorkDone[g][i] = true;
                        else Savequeue.bWorkDone[g][i] = false;
                    }
                    else if (IndexInfo.CategorySurface == CategorySurface.BA)
                    {
                        if (BAInspectSkip[IndexInfo.Index]) Savequeue.bWorkDone[g][i] = true;
                        else Savequeue.bWorkDone[g][i] = false;
                    }
                }
            }
            #endregion
            m_InspectThread = new InspectProcess(abTestMode);
            m_InspectThread.Inspection(MainWindow.Setting.General.MachineName);

            if (MainWindow.Setting.SubSystem.Laser.UseLaser)
            {
                ret = StartLaser(abTestMode);
            }

            if (!bThreadView)
            {
                if (threadView != null)
                {
                    threadView.Abort();
                    Thread.Sleep(500);
                    threadView = null;
                }

                InspectProcess.View_Done = false;
                bThreadView = true;
                threadView = new Thread(ThreadView);
                threadView.Start();
            }

            if (!bThreadSave)
            {
                if (threadSave != null)
                {
                    threadSave.Abort();
                    Thread.Sleep(500);
                    threadSave = null;
                }

                InspectProcess.ImageSave_Done = false;

                // 기존 작업이 있다면 취소
                //_imageSaveCancellationTokenSource?.Cancel();
                //_imageSaveCancellationTokenSource = new CancellationTokenSource();

                //bThreadSave = true;
                //_ = ImageSaveQueueAsync(_imageSaveCancellationTokenSource.Token);

                bThreadSave = true;
                threadSave = new Thread(ImageSaveQueue);
                threadSave.Start();
            }
            return ret;
        }

        public void StartScan(int nIndex)
        {
            CamCtrl[nIndex].Map.StartInspect();
        }

        public bool CreateResultFile(int id, string IDString)
        {
            if (!m_ptrMainWindow.ITS_info_control.SaveResultFile(MainWindow.Setting.General.ITSPath3, m_TopResult[id % BUFFER], m_BotResult[id % BUFFER], MainWindow.curLotData.ITS_ORDER, MainWindow.NG_Info, IDString))
                return false;

            return true;
        }

        public int CreateStripMap(int id)
        {
            PCS.VRS.VRS_MAP_Info info = new PCS.VRS.VRS_MAP_Info();
            info.StripRow = 1;
            info.SubstrateType = "Strip";
            info.Orientation = 0;
            info.UnitNumX = MainWindow.CurrentModel.Strip.UnitColumn;
            info.UnitNumY = MainWindow.CurrentModel.Strip.UnitRow;
            PCS.VRS.VRS_MAP_Body map = new PCS.VRS.VRS_MAP_Body();
            map.SubstrateID = m_LastResult[id % BUFFER].IDString;
            int InspectIndex = id;       
            if (MainWindow.CurrentModel.ITS.UseITS)
            {
                string[] temp = map.SubstrateID.Split(' ');
                if (temp.Length == 2)
                { InspectIndex = Convert.ToInt32(temp[temp.Length - 1]); }
            }
            map.InspectIndex = InspectIndex;
            map.Side = "Bot";
            map.Attach = "BA";
            map.MapName = "BinCodeMap";
            map.MapVersion = "1.0";
            map.BinType = "HexaDecimal";
            map.NullBin = "00";

            string mapPath = @"e:\VRSBinCodeMap" + string.Format("\\{0}\\{1}\\{2}", MainWindow.CurrentGroup.Name, MainWindow.CurrentModel.Name, this.TableDataCtrl.LotNo);
            Directory.CreateDirectory(mapPath);
            string filename;
            List<string> strmap = new List<string>();

            ////Bot
            strmap.Clear();
            filename = map.SubstrateID + "_BA";
            PCS.VRS.MapControl.MapConvITSL(m_BotResult[id % BUFFER].Buffer, ref strmap, MainWindow.NG_Info, false, true);
            map.Map = strmap.ToArray();
            int nRet = PCS.VRS.MapControl.WriteMap(info, map, mapPath, filename);
            if (nRet != 0) return nRet;

            ///Top       
            strmap.Clear();
            filename = map.SubstrateID + "_CA";
            map.Side = "Top";
            map.Attach = "CA";
            PCS.VRS.MapControl.MapConvITSL(m_TopResult[id % BUFFER].Buffer, ref strmap, MainWindow.NG_Info);
            map.Map = strmap.ToArray();
            nRet = PCS.VRS.MapControl.WriteMap(info, map, mapPath, filename);
            if (nRet != 0) return nRet;

            ////All
            map.Side = "All";
            map.Attach = "AL";
            filename = map.SubstrateID + "_ALL";
            strmap.Clear();
            PCS.VRS.MapControl.MapConvITSL(m_LastResult[id % BUFFER].Buffer, ref strmap, MainWindow.NG_Info);
            map.Map = strmap.ToArray();
            nRet = PCS.VRS.MapControl.WriteMap(info, map, mapPath, filename);
            if (nRet != 0) return nRet;
            return 0;
        }

        public int GetResultNum(int id, string sid, out int nBad)
        {
            bool isNGAndIDOK = false; 

            if (m_UseConBad)
            {
                for (int i = 0; i < m_LastResult[id % BUFFER].SizeX; i++)
                    for (int j = 0; j < m_LastResult[id % BUFFER].SizeY; j++)
                    {
                        if (m_LastResult[id % BUFFER].Buffer[i, j] > 0)
                        {
                            m_BadMap[i, j]++;
                            if (m_BadMap[i, j] >= MainWindow.CurrentModel.ConBad)
                            {
                                BadX = i;
                                BadY = j;
                                m_BadStop = true;
                            }
                        }
                    }
            }

            if (MainWindow.CurrentModel.ITS.UseITS)
            {
               
                if (m_BotResult[id % BUFFER].IDString == "" && m_TopResult[id % BUFFER].IDString == "" && sid == "")
                    m_LastResult[id % BUFFER].AutoNG = true;

                if (sid != "")
                {
                    m_LastResult[id % BUFFER].IDString = sid;
                    //ID 마킹기 정상 인식, 알고리즘 불량 인식의 경우 알고리즘 결과 값 삭제, but AutoNG 불량은 또 불량처리해야함.
                    if (m_LastResult[id % BUFFER].AutoNG)
                    {
                        if (!m_LastResult[id % BUFFER].IsNGStrip())
                        {
                            if (m_LastResult[id % BUFFER].IsAutoNGStrip(MainWindow.AutoNG_Check))
                                isNGAndIDOK = true;
                            else
                                m_LastResult[id % BUFFER].AutoNG = false;
                        }
                        else isNGAndIDOK = true;
                    }
                }
                else
                {
                    if (m_TopResult[id % BUFFER].IDString != "") m_LastResult[id % BUFFER].IDString = m_TopResult[id % BUFFER].IDString;
                    else
                    {
                        if (m_BotResult[id % BUFFER].IDString != "") m_LastResult[id % BUFFER].IDString = m_BotResult[id % BUFFER].IDString;
                    }
                }
                // 2D ID가 ITS 와 다르거나 , 인식되지 않았을 경우 미인식처리
                if (!sid.StartsWith(MainWindow.curLotData.ITS_ORDER) || sid == "")
                {
                    m_LastResult[id % BUFFER].AutoNG = true;
                    MainWindow.Log("PCS", SeverityLevel.WARN, string.Format("2D Mark 불일치. (Current ID: {0} ITS ID: {1})", sid, MainWindow.curLotData.ITS_ORDER), true);
                }

                //2D 인식 방법 ( 1. 알고리즘, 2. 2D 리더기(있고/없고 설비별다름) ) 2가지 모두 실패시 여기서 최종판단!
                //여기서 판단하지 않고 검사 프로세스에서 개별판단시 알고리즘인식불가/리더기 인식의 경우 다른 불량정보 사라짐.
                //폐기의 경우에만 모든 유닛 불량처리.
                if (m_LastResult[id % BUFFER].AutoNG)
                {
                    if(!isNGAndIDOK) m_LastResult[id % BUFFER].SetAll(MainWindow.NG_Info.BadNameToID("ID미인식"));
                    nBad = m_LastResult[id % BUFFER].BadCount();
                    return 2;
                }
            }

            if (m_ptrMainWindow.StaticMark)
            {
                InspectBuffer.Add(ref m_LastResult[id % BUFFER], StaticMarkResult);
                InspectBuffer.Add(ref m_TopResult[id % BUFFER], StaticMarkResult);
                InspectBuffer.Add(ref m_BotResult[id % BUFFER], StaticMarkResult);
            }

            //ITS skip data
            if (m_ptrMainWindow.SkipData)
            {
                //ID string 비교
                int index = m_ptrMainWindow.ITS_info_control.skip_datas.FindIndex(x => x.ID == m_LastResult[id % BUFFER].IDString);
                if (index >= 0)
                {
                    m_LastResult[id % BUFFER].CopyToPreSkipdata();
                    InspectBuffer.Add(ref m_LastResult[id % BUFFER], m_ptrMainWindow.ITS_info_control.skip_datas[index].result);
                }
            }

            nBad = m_LastResult[id % BUFFER].BadCount();

            if (!MainWindow.CurrentModel.UseVerify)
            {
                int shi = MainWindow.NG_Info.BadNameToID("PSRShift");
                int sta = MainWindow.NG_Info.BadNameToID("고정불량");
                int our = MainWindow.NG_Info.BadNameToID("외곽불량");
                int result = m_LastResult[id % BUFFER].CountAutoNG(MainWindow.CurrentModel.AutoNG, MainWindow.CurrentModel.AutoNGX, MainWindow.CurrentModel.AutoNGY,
                    MainWindow.CurrentModel.AutoNGBlock, MainWindow.CurrentModel.Strip.Block, shi, sta, our, MainWindow.CurrentModel.AutoNGOuterY, MainWindow.CurrentModel.AutoNGOuterYMode, MainWindow.CurrentModel.AutoNGOuterDivY, MainWindow.CurrentModel.AutoNGMatrixdata);

                if (result == 1 || result == 2)
                {
                    int val = MainWindow.NG_Info.BadNameToID("연속불량");
                    if (result == 2) m_LastResult[id % BUFFER].SetAll(val);
                    m_LastResult[id % BUFFER].AutoNG = true;
                    return 2;
                }

                if (m_LastResult[id % BUFFER].AutoNG)
                    return 2;
            }
            else
            {
                if (MainWindow.CurrentModel.ITS.UseITS)
                {
                    if (m_LastResult[id % BUFFER].IDString == null || m_LastResult[id % BUFFER].IDString == "")
                    {
                        m_LastResult[id % BUFFER].IDString = "Fail read ID";
                        return 2; // Verify 사용의 경우 2D 미인식 시 Strip 폐기 처리.
                    }
                }
                else
                {
                    m_LastResult[id % BUFFER].IDString = string.Format("{0} {1}", TableDataCtrl.LotNo, string.Format("{0:D4}", (id + TableDataCtrl.BeforeCount)));
                    //m_LastResult[id % BUFFER].IDString = string.Format("{0:D4}", (id + TableDataCtrl.BeforeCount));
                }
            }


            if (m_LastResult[id % BUFFER].BadCount() == 0)
                return 0;
            else
                return 1;
        }

        public void CopyLaserResult(int id)
        {
            LaserResult[m_LastID % BUFFER].Clear();    
            LaserResult[m_LastID % BUFFER] = m_LastResult[id % BUFFER].Clone();
            LaserResult[m_LastID % BUFFER].ID = id;

            m_LastID++;
        }

        public void AddFailResult(int id)
        {          
            TableDataCtrl.AddData(m_LastResult[id % BUFFER], id, MainWindow.Setting.General.VRSNGUnitLimit, MainWindow.CurrentModel.ITS.UseITS);
            m_LastResult[id % BUFFER].Clear();
            m_TopResult[id % BUFFER].Clear();
            m_BotResult[id % BUFFER].Clear();
        }

        public bool CreateIDMarkMap(InspectBuffer buffer, int anID)
        {
            try
            {
                if (MainWindow.CurrentModel.Marker.IDMark == 0 || MainWindow.CurrentModel.InspectMode == 4)
                    return true;

                string strID = null;

                if (MainWindow.Setting.Job.Customer == 0)
                    strID = MainWindow.IDString + anID.ToString("00000");
                else
                    strID = MainWindow.IDString + anID.ToString("000");

                try
                {
                    if (MainWindow.CurrentModel.InspectMode > 0)
                    {
                        if (MainWindow.Setting.Job.Customer == 0)
                            strID = MainWindow.IDString + string.Format("{0:D5}", Convert.ToInt32(buffer.IDString));
                        else
                            strID = MainWindow.IDString + string.Format("{0:D3}", Convert.ToInt32(buffer.IDString));
                    }
                }
                catch
                {
                    strID = "";
                }

                string strFolderPath = String.Format(@"{0}/{1}/", MainWindow.Setting.General.IDMarkPath, TableDataCtrl.LotNo);
                if (!Directory.Exists(strFolderPath))
                    Directory.CreateDirectory(strFolderPath);

                string strFile = strFolderPath + strID + ".xml";
                if (File.Exists(strFile))
                {
                    return false;
                }

                FileStream fs = File.Create(strFile);
                fs.Close();
                List<string> lstLines = new List<string>();
                lstLines.Add(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
                lstLines.Add(@"<MapData xmlns=""urn:semi-org:xsd.E142-1.V1005.SubstrateMap"">");
                lstLines.Add("\t<SubstrateMaps>");
                lstLines.Add("\t\t" + @"<SubstrateMap SubstrateType=""Strip"" SubstrateId=""" + strID + @""" LayoutSpecifier="""" SubstrateSide=""TopSide"" Orientation=""0"" " +
                               @"OriginLocation=""UpperRight"" AxisDirection=""DownLeft"">");
                lstLines.Add("\t\t\t" + @"<Overlay MapName=""BinCodeMap"" MapVersion=""0"">");
                lstLines.Add("\t\t\t\t" + @"<BinCodeMap BinType=""HexaDecimal"" NullBin=""FF"">");
                lstLines.Add("\t\t\t\t\t<BinDefinitions>");
                lstLines.Add("\t\t\t\t\t</BinDefinitions>");
                for (int i = buffer.SizeY - 1; i >= 0; i--)
                {
                    lstLines.Add("\t\t\t\t\t<BinCode>");
                    string ss = "";
                    for (int j = buffer.SizeX - 1; j >= 0; j--)
                    {
                        if (buffer.Get(j, i) == 0)
                            ss += "FF";
                        else
                            ss += "81";
                    }
                    lstLines.Add("\t\t\t\t\t\t<![CDATA[" + ss + "]]>");
                    lstLines.Add("\t\t\t\t\t</BinCode>");
                }

                lstLines.Add("\t\t\t\t</BinCodeMap>");
                lstLines.Add("\t\t\t</Overlay>");
                lstLines.Add("\t\t</SubstrateMap>");
                lstLines.Add("\t</SubstrateMaps>");
                lstLines.Add("</MapData>");
                File.WriteAllLines(strFile, lstLines);

            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>   Starts a laser. </summary>
        public bool StartLaser(bool testMode)
        {
            m_LaserPos = 0;
            m_LaserStrPos = 0;
            m_LastID = 0;
            for (int i = 0; i < 5; i++)
            {
                m_LaserStr[i] = "";
            }
            int resultValue = -1;
            if (!MainWindow.Setting.SubSystem.Laser.UseLaser) return true;

            if (m_LaserThread != null)
            {
                m_LaserThread.Stop();
                m_LaserThread = null;
            }
            m_LaserThread = new LaserProcess(out resultValue, MainWindow.CurrentModel.UseMarking);
            if (resultValue != 0) return false;
            m_LaserThread.Marking();
            return true;
        }

        /// <summary>   Initialises the display. </summary>
        public void InitDisplay()
        {
            InitializeReelmap();
            ResultTable.lblFailUnit.Text = MainWindow.CurrentModel.AutoNG.ToString();
            ResultTable.lblFailBlock.Text = MainWindow.CurrentModel.AutoNGBlock.ToString();
            ResultTable.lblFailUnitX.Text = MainWindow.CurrentModel.AutoNGX.ToString();
            ResultTable.lblFailUnitY.Text = MainWindow.CurrentModel.AutoNGY.ToString();
            if (m_UseConBad)
            {
                ResultTable.lblUseConBad.Text = "정지 사용";
                ResultTable.spUseConBad.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                ResultTable.lblConBad.Text = MainWindow.CurrentModel.ConBad.ToString() + "Strip 누적";
            }
            else
            {
                ResultTable.lblUseConBad.Text = "정지 미사용";
                ResultTable.spUseConBad.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
                ResultTable.lblConBad.Text = "";
            }
            string AutoNGMatrixInof = "";
            if (MainWindow.CurrentModel.AutoNGMatrixdata != null)
            {
                for (int item = 0; item < MainWindow.CurrentModel.AutoNGMatrixdata.Count; item++)
                    AutoNGMatrixInof += MainWindow.CurrentModel.AutoNGMatrixdata[item].StringUI + "|";
            }
            ResultTable.lblFailArrayInfo.Text = AutoNGMatrixInof;
            ClearImage();
        }

        public void InitializeMakingMap(int MarkingMapNumber)
        {
            int nUnitColumn;
            int nUnitRow;

            if (MainWindow.CurrentModel != null)
            {
                nUnitColumn = MainWindow.CurrentModel.Strip.UnitColumn;
                nUnitRow = MainWindow.CurrentModel.Strip.UnitRow;
            }
            else
            {
                nUnitColumn = 5;
                nUnitRow = 20;
            }

            if (MarkingMapNumber == 0)
            {
                Mark1Map.SetStripMap(nUnitColumn, nUnitRow, MainWindow.CurrentModel.Strip.MarkStep, MainWindow.CurrentModel.Strip.StepUnits);
                Mark1Map.GoodColor();
            }
            else if (MarkingMapNumber == 1)
            {
                Mark2Map.SetStripMap(nUnitColumn, nUnitRow, MainWindow.CurrentModel.Strip.MarkStep, MainWindow.CurrentModel.Strip.StepUnits);
                Mark2Map.GoodColor();
            }
            else
            {
                ;
            }
        }

        /// <summary>   Initializes the reelmap. </summary>
        private void InitializeReelmap()
        {
            int nUnitColumn;
            int nUnitRow;

            if (MainWindow.CurrentModel != null)
            {
                nUnitColumn = MainWindow.CurrentModel.Strip.UnitColumn;
                nUnitRow = MainWindow.CurrentModel.Strip.UnitRow;
            }
            else
            {
                nUnitColumn = 5;
                nUnitRow = 20;
            }

            m_nUnitCountPerShot = nUnitColumn * nUnitRow;

            for (int i = 0; i < MainWindow.Setting.Job.CACount + MainWindow.Setting.Job.BACount + MainWindow.Setting.Job.BPCount; i++)
            {
                CamCtrl[i].Map.SetStripMap(nUnitColumn, nUnitRow, this);
                CamCtrl[i].Map.GoodColor();
            }
            Mark1Map.SetStripMap(nUnitColumn, nUnitRow, MainWindow.CurrentModel.Strip.MarkStep, MainWindow.CurrentModel.Strip.StepUnits);
            Mark2Map.SetStripMap(nUnitColumn, nUnitRow, MainWindow.CurrentModel.Strip.MarkStep, MainWindow.CurrentModel.Strip.StepUnits);
            m_BadMap = new int[nUnitColumn, nUnitRow];

            for (int i = 0; i < nUnitColumn; i++)
            {
                for (int j = 0; j < nUnitRow; j++)
                    m_BadMap[i, j] = 0;
            }

            Mark1Map.GoodColor();
            Mark2Map.GoodColor();

            for (int i = 0; i < m_BPResult.Length; i++) m_BPResult[i].Init(nUnitColumn, nUnitRow, MainWindow.NG_Info.Priority);
            for (int i = 0; i < m_CAResult.Length; i++) m_CAResult[i].Init(nUnitColumn, nUnitRow, MainWindow.NG_Info.Priority);
            for (int i = 0; i < m_BAResult.Length; i++) m_BAResult[i].Init(nUnitColumn, nUnitRow, MainWindow.NG_Info.Priority);
            
            for (int i = 0; i < BUFFER; i++)
            {
                m_LastResult[i].Init(nUnitColumn, nUnitRow, MainWindow.NG_Info.Priority);
                m_TopResult[i].Init(nUnitColumn, nUnitRow, MainWindow.NG_Info.Priority);
                m_BotResult[i].Init(nUnitColumn, nUnitRow, MainWindow.NG_Info.Priority);
                LaserResult[i].Init(nUnitColumn, nUnitRow, MainWindow.NG_Info.Priority);
                for (int scan = 0; scan < BP_Done.GetLength(0); scan++) BP_Done[scan, i] = false;
                for (int scan = 0; scan < CA_Done.GetLength(0); scan++) CA_Done[scan, i] = false;
                for (int scan = 0; scan < BA_Done.GetLength(0); scan++) BA_Done[scan, i] = false;
            }
        }

        /// <summary>   Shot change elf. </summary>
        public void ShotChangeBA(int ScanNum)
        {
            InspectBuffer.Add(ref m_LastResult[m_BA_ID[ScanNum] % BUFFER], m_BAResult[ScanNum]);
            InspectBuffer.Add(ref m_BotResult[m_BA_ID[ScanNum] % BUFFER], m_BAResult[ScanNum]);
            m_BAResult[ScanNum].Clear();
        }
        public void ShotChagneCA(int ScanNum)
        {
            InspectBuffer.Add(ref m_LastResult[m_CA_ID[ScanNum] % BUFFER], m_CAResult[ScanNum]);
            InspectBuffer.Add(ref m_TopResult[m_CA_ID[ScanNum] % BUFFER], m_CAResult[ScanNum]);
            m_CAResult[ScanNum].Clear();
        }

        /// <summary>   Shot change elf. </summary>
        public void ShotChangeBP(int ScanNum)
        {
            InspectBuffer.Add(ref m_LastResult[m_BP_ID[ScanNum] % BUFFER], m_BPResult[ScanNum]);
            InspectBuffer.Add(ref m_TopResult[m_BP_ID[ScanNum] % BUFFER], m_BPResult[ScanNum]);
            m_BPResult[ScanNum].Clear();
        }

        /// <summary>   Clears the image. </summary>
        private void ClearImage()
        {
            for (int Scancount = 0; Scancount < MainWindow.Setting.Job.BPCount; Scancount++)
            {
                foreach (InspectionResultImage imgSet in m_BPNGImage[Scancount])
                {
                    imgSet.ClearImage();
                }
                m_BPImageSetIndex[Scancount] = 0;
            }
            for(int Scancount = 0; Scancount < MainWindow.Setting.Job.CACount; Scancount++)
            {
                foreach (InspectionResultImage imgSet in m_CANGImage[Scancount])
                {
                    imgSet.ClearImage();
                }
                m_CAImageSetIndex[Scancount] = 0;
            }
            for (int Scancount = 0; Scancount < MainWindow.Setting.Job.BACount; Scancount++)
            {
                foreach (InspectionResultImage imgSet in m_BANGImage[Scancount])
                {
                    imgSet.ClearImage();
                }
                m_BAImageSetIndex[Scancount] = 0;
            }
        }

        public bool ReadSkipData()
        {
            string path = MainWindow.Setting.General.ITSPath1;
            if (!m_ptrMainWindow.ITS_info_control.ReadSkipData_AVI(path, MainWindow.curLotData.ITS_ORDER, MainWindow.CurrentModel.Strip.UnitColumn, MainWindow.CurrentModel.Strip.UnitRow))
                return m_ptrMainWindow.SkipData = false;
            else
                return m_ptrMainWindow.SkipData = true;
        }

        public void WriteITSFile()
        {
            if (!m_ptrMainWindow.ITS_info_control.WriteOutputFile_AVI(MainWindow.Setting.General.ITSPath3, MainWindow.Setting.General.ITSPath2, MainWindow.curLotData.ITS_ORDER, TableDataCtrl.Operator, MainWindow.CurrentModel.AutoNG, TableDataCtrl.EndTime))
            {
                MessageBox.Show("ITS File 쓰기 오류");
            }
        }

        public void WriteITSDB()
        {
            var enable = MainWindow.Setting.General.UseITSDB;
            if (enable)
            {
                DateTime StartTime = TableDataCtrl.StartTime;
                DateTime EndTime = TableDataCtrl.EndTime;

                string MCCode = MainWindow.Setting.General.MachineName;
                int Customer = m_ptrMainWindow.m_Customer;
                string TableName = MainWindow.Setting.General.ITSTableName;

                string strDBIP = MainWindow.Setting.General.ITSDBIP;
                string strDBPort = MainWindow.Setting.General.ITSDBPort;
                if (!m_ptrMainWindow.ITS_info_control.ConnectServerDB(strDBIP, strDBPort))
                {
                    MessageBox.Show("ITS DB 연결 오류");
                    return;
                }

                m_ptrMainWindow.ITS_info_control.CloseITSDB();
            }
        }

        public void StopInspect(bool bDone = false)
        {
            if (bThreadView)
            {
                bThreadView = false;
                while (!InspectProcess.View_Done) Thread.Sleep(100); // 모든 UI 처리가 끝날 때 까지 대기
            }
            if(bThreadSave)
            {
                bThreadSave = false;
                //_imageSaveCancellationTokenSource?.Cancel();
                //_imageSaveCancellationTokenSource = null;
                while (!InspectProcess.ImageSave_Done) Thread.Sleep(100); // 모든 UI 처리가 끝날 때 까지 대기
            }

            m_ptrMainWindow.StopTimer();
            TableDataCtrl.EndTime = DateTime.Now;
            TableDataCtrl.SaveEndTime(TableDataCtrl.EndTime);

            if (MainWindow.Setting.General.UseITSDB) WriteITSDB();

            // VRS 사용일 경우 loss 파일 다른 경로에 생성
            ResultTable.WriteResultLoss(MainWindow.CurrentModel.UseVerify);

            // 검사 이력정보 관리
            try
            {
                if (bDone && MainWindow.curJob != null)
                {
                    MainWindow.curJob.dYield = ResultTable.TableData.Yield;
                    PCS.VRS.InfoControl.UpdateJobInfoEnd(MainWindow.curJob);
                }
            }
            catch(Exception ex)
            {
                MainWindow.Log("PCS", SeverityLevel.DEBUG, string.Format("UpdateJobInfoEnd Fail, Error: {0}", ex.Message), true);
            }

            if (MainWindow.CurrentModel.ITS.UseITS && !MainWindow.CurrentModel.UseVerify)
                WriteITSFile();

            m_ptrMainWindow.MainToolBarCtrl.SetLabelText("검 사 대 기");            
            InspectionStarted = false;
        }

        private void ClearLastDefectRectangle(CategorySurface aSurface, int Scancount)
        {
            if (aSurface.Equals(CategorySurface.BA))
            {
                for (int i = 0; i < m_BANGImage[Scancount].Count; i++)
                    m_BANGImage[Scancount][i].LastDefImage.Visibility = Visibility.Hidden;
            }

            else if (aSurface.Equals(CategorySurface.BP))
            {
                for (int i = 0; i < m_BPNGImage[Scancount].Count; i++)
                    m_BPNGImage[Scancount][i].LastDefImage.Visibility = Visibility.Hidden;
            }
           
            else if (aSurface.Equals(CategorySurface.CA))
            {
                for (int i = 0; i < m_CANGImage[Scancount].Count; i++)
                    m_CANGImage[Scancount][i].LastDefImage.Visibility = Visibility.Hidden;
            }
        }

        public void RoundRobinImgSet(CategorySurface aMode, int ScanCount, ResultItem aResultItem, BitmapSource imgRef, BitmapSource imgDef, int anX, int anY, bool abIsLastDefect, string SectionType)
        {
            //RectFill fillColor = ConvertResultTypeToEnumNumber(aResultItem.ResultType);
            Bad_Info bad = MainWindow.NG_Info.ResultTypeToBadInfo(aResultItem.ResultType);
            //string szNGName = InspectDataManager.GetNGName(aResultItem.ResultType);

            if (aMode == CategorySurface.BA)
            {
                #region Bottom Sur
                m_BANGImage[ScanCount][m_BAImageSetIndex[ScanCount]].NGName = bad.Name;
                m_BANGImage[ScanCount][m_BAImageSetIndex[ScanCount]].txtUnitPos.Text = string.Format(" (X:{0},Y:{1})", anX + 1, anY + 1);
                                 
                m_BANGImage[ScanCount][m_BAImageSetIndex[ScanCount]].txtDefectInfo.Text = String.Format("불량크기:{1:F0}", aResultItem.SectionDefectCenter, aResultItem.DefectSize);
                //m_BANGImage[ScanCount][m_BAImageSetIndex[ScanCount]].FillColor = fillColor;
                m_BANGImage[ScanCount][m_BAImageSetIndex[ScanCount]].NG_ID = bad.ID;
                m_BANGImage[ScanCount][m_BAImageSetIndex[ScanCount]].NG_Color = bad.Color;
                m_BANGImage[ScanCount][m_BAImageSetIndex[ScanCount]].Row = anX;
                m_BANGImage[ScanCount][m_BAImageSetIndex[ScanCount]].Col = anY;
                m_BANGImage[ScanCount][m_BAImageSetIndex[ScanCount]].ChangeImageSet(imgRef, imgDef, abIsLastDefect);
                m_BANGImage[ScanCount][m_BAImageSetIndex[ScanCount]].Visibility = Visibility.Visible;

                CamCtrl[MainWindow.Setting.Job.BPCount+ MainWindow.Setting.Job.CACount + ScanCount].Map.SetColor(anX, anY, (bad.Color as System.Windows.Media.SolidColorBrush).Color, SectionType);
                if (m_BAImageSetIndex[ScanCount] >= m_BANGImage[ScanCount].Count - 1)
                {
                    m_BAImageSetIndex[ScanCount] = 0;
                }
                else
                {
                    m_BAImageSetIndex[ScanCount]++;
                }
                #endregion
            }
            
            else if (aMode == CategorySurface.BP)
            {
                #region BondPad
                m_BPNGImage[ScanCount][m_BPImageSetIndex[ScanCount]].NGName = bad.Name;
                m_BPNGImage[ScanCount][m_BPImageSetIndex[ScanCount]].txtUnitPos.Text = string.Format(" (X:{0},Y:{1})", anX + 1, anY + 1);

                m_BPNGImage[ScanCount][m_BPImageSetIndex[ScanCount]].txtDefectInfo.Text = String.Format("불량크기:{1:F0}", aResultItem.SectionDefectCenter, aResultItem.DefectSize);
                //m_BPNGImage[ScanCount][m_BPImageSetIndex].FillColor = fillColor;
                m_BPNGImage[ScanCount][m_BPImageSetIndex[ScanCount]].NG_ID = bad.ID;
                m_BPNGImage[ScanCount][m_BPImageSetIndex[ScanCount]].NG_Color = bad.Color;
                m_BPNGImage[ScanCount][m_BPImageSetIndex[ScanCount]].Row = anX;
                m_BPNGImage[ScanCount][m_BPImageSetIndex[ScanCount]].Col = anY;
                m_BPNGImage[ScanCount][m_BPImageSetIndex[ScanCount]].ChangeImageSet(imgRef, imgDef, abIsLastDefect);
                m_BPNGImage[ScanCount][m_BPImageSetIndex[ScanCount]].Visibility = Visibility.Visible;

                CamCtrl[0 + ScanCount].Map.SetColor(anX, anY, (bad.Color as System.Windows.Media.SolidColorBrush).Color, SectionType);
                if (m_BPImageSetIndex[ScanCount] >= m_BPNGImage[ScanCount].Count - 1)
                {
                    m_BPImageSetIndex[ScanCount] = 0;
                }
                else
                {
                    m_BPImageSetIndex[ScanCount]++;
                }
                #endregion
            }

            else if (aMode == CategorySurface.CA)
            {
                #region Top Sur
                m_CANGImage[ScanCount][m_CAImageSetIndex[ScanCount]].NGName = bad.Name;
                m_CANGImage[ScanCount][m_CAImageSetIndex[ScanCount]].txtUnitPos.Text = string.Format(" (X:{0},Y:{1})", anX + 1, anY + 1);

                m_CANGImage[ScanCount][m_CAImageSetIndex[ScanCount]].txtDefectInfo.Text = String.Format("불량크기:{1:F0}", aResultItem.SectionDefectCenter, aResultItem.DefectSize);
                //m_CANGImage[m_CAImageSetIndex].FillColor = fillColor;
                m_CANGImage[ScanCount][m_CAImageSetIndex[ScanCount]].NG_ID = bad.ID;
                m_CANGImage[ScanCount][m_CAImageSetIndex[ScanCount]].NG_Color = bad.Color;
                m_CANGImage[ScanCount][m_CAImageSetIndex[ScanCount]].Row = anX;
                m_CANGImage[ScanCount][m_CAImageSetIndex[ScanCount]].Col = anY;
                m_CANGImage[ScanCount][m_CAImageSetIndex[ScanCount]].ChangeImageSet(imgRef, imgDef, abIsLastDefect);
                m_CANGImage[ScanCount][m_CAImageSetIndex[ScanCount]].Visibility = Visibility.Visible;

                CamCtrl[MainWindow.Setting.Job.BPCount + ScanCount].Map.SetColor(anX, anY, (bad.Color as System.Windows.Media.SolidColorBrush).Color, SectionType);
                if (m_CAImageSetIndex[ScanCount] >= m_CANGImage[ScanCount].Count - 1)
                {
                    m_CAImageSetIndex[ScanCount] = 0;
                }
                else
                {
                    m_CAImageSetIndex[ScanCount]++;
                }
                #endregion
            }
        }

        public void TrainErrorStop(int anErrorValue, string aszMessage = "")
        {
            InspectionStarted = false;
            switch (anErrorValue)
            {
                case -1:
                    MainWindow.Log("PCS", SeverityLevel.FATAL, "Train Error", true);
                    break;
                case -2:
                    MainWindow.Log("PCS", SeverityLevel.FATAL, "Buffer Error", true);
                    break;
                case -3:
                    MainWindow.Log("PCS", SeverityLevel.FATAL, "Param Error", true);
                    break;
                case -4:
                    MainWindow.Log("PCS", SeverityLevel.FATAL, String.Format("Image Cutting Error (Count : {0})", aszMessage), true);
                    break;
                case -5:
                    MainWindow.Log("PCS", SeverityLevel.FATAL, aszMessage, true);
                    MessageBox.Show(aszMessage);
                    break;
            }
        }

        #region Get inspection result top, bottom, transmission surfaces.

        public void SetSkipDisplay(int index, bool bSkip)
        {
            CamCtrl[index].SetSkip(bSkip);
        }

        private Thread threadView;
        bool bThreadView = false;

        List<ConcurrentQueue<object>> ResultQueue = new List<ConcurrentQueue<object>>();

        private Thread threadSave;
        bool bThreadSave = false;
        const int QueueBuffer = 20;
        //static List<List<ConcurrentQueue<object>>> ImageSave_Queue = new List<List<ConcurrentQueue<object>>>();
        static public SaveQueue Savequeue = new SaveQueue();

        public void GetInspectResult_BA(int nNum, int nGrabSide)
        {
            MainWindow.IndexInfo IndexInfo = MainWindow.Convert_Vision(CategorySurface.BA, nNum, nGrabSide);
            //MainWindow.Log("IS", SeverityLevel.DEBUG, string.Format("ID : {0} , GrabSide : {1}, BA : Index {2}", nNum, nGrabSide, IndexInfo.Index), true);
            DeviceController PCSInstance = m_ptrMainWindow.PCSInstance;
            int FrameGrabberIndex = VID.CalcIndex(IndexInfo.VisionIndex);
            bool bAutoNG = false;

            int VisionNum = IndexInfo.VisionIndex;
            List<List<DefInfo>> SectionTypeDefect = new List<List<DefInfo>>();
            for (int j = 0; j < Enum.GetValues(typeof(SectionArea)).Length; j++) SectionTypeDefect.Add(new List<DefInfo>());
            int Surfaceindex = IndexInfo.Index + MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count + MainWindow.Setting.Job.CACount;
            if (PCSInstance == null)
                return;
            Action action = delegate
            {
                CamCtrl[MainWindow.Setting.Job.BPCount + MainWindow.Setting.Job.CACount + IndexInfo.Index].Map.GoodColor();/////// CA Scan Count + BP 1
            }; this.Dispatcher.Invoke(action);

            ResultInfo Result = PCSInstance.Vision[VisionNum].VisionResult;

            Surface surface;
            int nID = m_BA_ID[IndexInfo.Index];
            surface = Surface.BA1 + IndexInfo.Index;

            if (MainWindow.CurrentModel.ITS.UseITS && PCSInstance.Vision[VisionNum].VisionResult.IDMark.Status == 1)
            {
                try
                {
                    IDMarkResultInfo idmark = PCSInstance.Vision[VisionNum].VisionResult.IDMark;
                    string filter = "";
                    List<string> id = new List<string>();
                    id = DataMatrixCode.Algo_Conv_Square_DataMatrix2(idmark.Image, m_ptrMainWindow.currOrder, filter, m_ptrMainWindow.InspectionMonitoringCtrl.TableDataCtrl.p2DMarkCountBA);
                    if (id != null && id.Count > 0 && id[0].Trim() != "") StripID = id[0];
                    else
                    {
                        id = DataMatrixCode.Algo_Conv_Square_DataMatrix(idmark.Image, m_ptrMainWindow.currOrder, filter, m_ptrMainWindow.InspectionMonitoringCtrl.TableDataCtrl.p2DMarkCountBA);

                        if (id != null && id.Count > 0 && id[0].Trim() != "") StripID = id[0];
                        else
                        {
                            id = DataMatrixCode.GetDataMatrixMJ(idmark.Image, m_ptrMainWindow.currOrder, filter, m_ptrMainWindow.InspectionMonitoringCtrl.TableDataCtrl.p2DMarkCountBA);
                            if (id != null && id.Count > 0 && id[0].Trim() != "") StripID = id[0];
                            else
                            {
                                StripID = "";
                                bAutoNG = true;
                            }
                        }
                    }
                    m_ptrMainWindow.InspectionMonitoringCtrl.TableDataCtrl.p2DMarkCountBA++;
                    View2DMark(idmark.Image, id);
                    m_BAResult[IndexInfo.Index].IDString = StripID;
                    m_BAResult[IndexInfo.Index].AutoNG = bAutoNG;
                }
                catch
                {
                    MessageBox.Show("Setting.xml 파일의 ITS USE = 1 입니다.\n 정상인지 확인해 주세요.");
                }
            }

            if (Result != null)
            {
                int x = 0, y = 0;
                bool bColor = false;
                int resultCount = Result.ResultItemCount;
                if (resultCount == 0) Savequeue.bWorkDone[nID % QueueBuffer][IndexInfo.Index + MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count + MainWindow.Setting.Job.CACount] = true;
                if (PCSInstance.Vision[VisionNum].m_iTrainError < 0)
                {
                    bAutoNG = true;
                }
                Result.UnitInspFailureCount = 0;
                if (resultCount > 0)
                {
                    m_nAllNGCount += resultCount;
                    BitmapSource MergedImage;

                    int ImageSize = 96;                                                                                                                                                                                          
                    if (MainWindow.Setting.General.ResultImageSizeType == false)
                        ImageSize = (MainWindow.Setting.General.ResultImageSize1 >= ImageSize) ? MainWindow.Setting.General.ResultImageSize1 : 96;
                    else
                        ImageSize = (MainWindow.Setting.General.ResultImageSize2 >= ImageSize) ? MainWindow.Setting.General.ResultImageSize2 : 300;
                    ImageCutter imageCutter = new ImageCutter(ImageSize);
                    if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] >= 6)
                    {
                        MergedImage = PCSInstance.Vision[VisionNum].GetMergedImage_Color();
                        bColor = true;
                        if (!imageCutter.CuttingImage_Color(MergedImage, 0, resultCount))
                            TrainErrorStop(-4, resultCount.ToString());
                    }
                    else
                    {
                        MergedImage = PCSInstance.Vision[VisionNum].GetMergedImage();
                        bColor = false;
                        if (!imageCutter.CuttingImage(MergedImage, 0, resultCount))
                            TrainErrorStop(-4, resultCount.ToString());
                    }
                    action = delegate
                    {
                        ClearLastDefectRectangle(IndexInfo.CategorySurface, IndexInfo.Index);
                    }; this.Dispatcher.Invoke(action);

                    int resValue = 0;          
                    for (int i = 0; i < resultCount; i++)
                    {
                        if (Result.Results[i].BreakType == 2)
                        {
                            bAutoNG = true;
                        }

                        y = Result.Results[i].UnitPos.X;
                        x = Result.Results[i].UnitPos.Y;

                        if (Result.Results[i].SectionType == PCS.ModelTeaching.SectionTypeCode.UNIT_REGION ||
                            Result.Results[i].SectionType == PCS.ModelTeaching.SectionTypeCode.PSR_REGION ||
                            Result.Results[i].SectionType == PCS.ModelTeaching.SectionTypeCode.RAW_REGION
                            )
                        {
                            resValue = MainWindow.NG_Info.ResultTypeToID(Result.Results[i].ResultType);
                            if (resValue < 0)
                            {
                                TrainErrorStop(-5, "NG_Info POP DB 에러");
                                return;
                            }

                            if (Result.Results[i].ResultType == (int)Common.Drawing.InspectionInformation.eVisInspectResultType.eInspResultUnitPattern ||
                                Result.Results[i].ResultType == (int)Common.Drawing.InspectionInformation.eVisInspectResultType.eInspResultBufferError || resValue == 47)
                                m_BAResult[IndexInfo.Index].AutoNG = true;

                            m_BAResult[IndexInfo.Index].Set(x, y, resValue);
                            if (!MainWindow.CurrentModel.UseVerify)
                            {
                                if (MainWindow.AutoNG_Check[resValue - 1])
                                {
                                    m_BAResult[IndexInfo.Index].AutoNG = true;
                                }
                            }
                        }
                        else // 외곽 Section, ID Section, Strip Section 
                        {
                            int val = 0;
                            if (Result.Results[i].ResultType == (int)eVisInspectResultType.eInspResultNoReSizeVentHole)             
                                val = MainWindow.NG_Info.ResultTypeToID(Result.Results[i].ResultType);                      
                            else
                                val = MainWindow.NG_Info.ResultTypeToID(1); // ResultType=1 외곽불량

                            if (val < 0)
                            {
                                TrainErrorStop(-5, "NG_Info POP DB 에러");
                                return;
                            }

                            m_BAResult[IndexInfo.Index].OuterNG = true;
                            m_BAResult[IndexInfo.Index].SetAll(val);

                            m_BAResult[IndexInfo.Index].AutoNG = true;
                        }

                        if (i < imageCutter.RefImage.Count && i < imageCutter.DefImage.Count)
                        {
                            BitmapSource refClone = imageCutter.RefImage[i].Clone();
                            BitmapSource defClone = imageCutter.DefImage[i].Clone();
                            refClone.Freeze(); defClone.Freeze();
                            ResultItem resultItem = Result.Results[i].Clone();

                            #region 23.03.13 suoow 수정

                            Result_Convert rc = InspResultAt(IndexInfo.Index + MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count + MainWindow.Setting.Job.CACount, resultItem.SectionID, resultItem.RoiID, resultItem.InspID, resultItem.Channel);

                            string Insp = string.Format("{0}_{1}_{2}_{3}", rc.Name.Trim().Replace(" ", ""), rc.Mode, rc.LowerThresh, rc.UpperThresh);
                            string strResult = MainWindow.NG_Info.ResultTypeToName(resultItem.ResultType);
                            if (strResult == "")
                            {
                                TrainErrorStop(-5, "NG_Info POP DB 에러");
                                return;
                            }

                            if (Result.Results[i].SectionType == PCS.ModelTeaching.SectionTypeCode.OUTER_REGION)
                            {
                                strResult += "_외곽";
                            }
                            string FileName = String.Format("CAM{0} ID[{1:D4}] X={2:D2} Y={3:D2} DEF P={4:D5},{5:D5} S={6:F2} {7} {8} {9} {10} {11}",
                                                  (int)surface, nID + TableDataCtrl.BeforeCount, x + 1, y + 1, resultItem.RelativeDefectCenter.X, resultItem.RelativeDefectCenter.Y,
                                                  resultItem.DefectSize, strResult.Trim().Replace(" ", ""), CategorySurface.BA.ToString(), IndexInfo.Index, resultItem.ChannelString, Insp);

                            string dnn_ret = "";
                            if (MainWindow.Setting.General.UseAI)
                            {
                                ///////////////// Dnn Classification
                                if (strResult.Contains("Align") || strResult == "PSRShift")
                                {
                                    m_BAResult[IndexInfo.Index].SetDnn(x, y, MainWindow.NG_Info.BadNameToID(strResult), true);
                                    dnn_ret = string.Format("{0} {1} {2} {3} {4} {5:F2}.png", -1, strResult, strResult, -1, 0, 0.0);
                                }
                                else
                                {
                                    DNN.YoloResult res = MainWindow.Predect_Yolo_Net((int)surface, FileName, refClone, defClone, i);
                                    if (dnn_ret == "")
                                    {
                                        //////////////// 저장 이미지명은 결과를 반영
                                        dnn_ret = string.Format("{0} {1} {2} {3} {4} {5:F2}.png", res.ClassID, res.ClassName.Trim().Replace(" ", ""), res.BadName.Trim().Replace(" ", ""), res.isDetection ? 1 : 0, 0, res.prob);
                                        if (res.BadName == "None") res.BadName = strResult;
                                        if (!strResult.Contains("_외곽"))
                                        {
                                            m_BAResult[IndexInfo.Index].SetDnn(x, y, MainWindow.NG_Info.BadNameToID(res.BadName), true);
                                        }
                                    }
                                    else
                                    {
                                        dnn_ret = string.Format("{0} {1} {2} {3} {4} {5:F2}.png", -1, "-", "-", 0, 0, 0.0);
                                        m_BAResult[IndexInfo.Index].SetDnn(x, y, MainWindow.NG_Info.BadNameToID(strResult), true);
                                    }
                                }
                            }
                            else
                                dnn_ret = string.Format("{0} {1} {2} {3} {4} {5:F2}.png", -1, "-", "-", 0, 0, 0.0);

                            FileName += " "; 
                            FileName += dnn_ret;
                            //MainWindow.Log("PCS", SeverityLevel.DEBUG, String.Format("{0}", FileName), true);
                            Object obj = new object[] { i, x, y, resultCount, surface, refClone, defClone, resultItem, nID, FileName, bColor, Result.Results[i].SectionType};
                            #endregion
                            ResultQueue[Surfaceindex].Enqueue(obj);
                            if (MainWindow.CurrentModel.UseAI)      
                                Set_ICS_Def_Data(x, y, resultItem.RelativeDefectCenter.X, resultItem.RelativeDefectCenter.Y, resultItem.UnitAlignOffset_X, resultItem.UnitAlignOffset_Y, FileName, 
                                    resultItem.DefectSize, resultItem.SectionType, resultItem.DefectPosX, resultItem.DefectPosY, IndexInfo, BP2Section_info, 
                                    MainWindow.Setting.General.ICS_OFFSET_BA_X, MainWindow.Setting.General.ICS_OFFSET_BA_Y, ref SectionTypeDefect);
                        }
                    }
                    imageCutter.Clear();
                    if (bAutoNG) { m_BAResult[IndexInfo.Index].AutoNG = true; }
                }
                if (MainWindow.CurrentModel.UseAI)
                {
                    Object obj_ICS = new object[] { Surfaceindex, ICS_LotFolderPath, MainWindow.ModelInfo, SectionData_ICS[IndexInfo.ScanIndex].ToList(), SectionTypeDefect.ToList(), MainWindow.AlignOffset_ICS[IndexInfo.ScanIndex].ToList() };
                    Savequeue.ICS_ResultQueue[nID % QueueBuffer][Surfaceindex].Enqueue(obj_ICS);
                    SectionTypeDefect.Clear();
                    bool test = false;
                    if (test)
                    {
                        object obj1;
                        Savequeue.ICS_ResultQueue[nID % QueueBuffer][Surfaceindex].TryDequeue(out obj1);
                        Write_ICS_Data(obj1);
                    }
                }
            }
        }

        public void GetInspectResult_BP(int nNum, int nGrabSide)
        {
            MainWindow.IndexInfo IndexInfo = MainWindow.Convert_Vision(CategorySurface.BP, nNum, nGrabSide);
            //MainWindow.Log("IS", SeverityLevel.DEBUG, string.Format("ID : {0} , GrabSide : {1}, BP : Index {2}", nNum, nGrabSide, IndexInfo.Index), true);
            DeviceController PCSInstance = m_ptrMainWindow.PCSInstance;
            int FrameGrabberIndex = VID.CalcIndex(IndexInfo.VisionIndex);
            if (PCSInstance == null)
                return;

            bool bAutoNG = false;
            int VisionNum = IndexInfo.VisionIndex;
            List<List<DefInfo>> SectionTypeDefect = new List<List<DefInfo>>();
            for (int j = 0; j < Enum.GetValues(typeof(SectionArea)).Length; j++) SectionTypeDefect.Add(new List<DefInfo>());
            int Surfaceindex = IndexInfo.Index;
            int sx, ex;
            if (IndexInfo.Index == 1)
            {
                sx = 0;
                ex = (int)Math.Ceiling(MainWindow.CurrentModel.Strip.UnitRow / 2.0) - 1;
            }
            else
            {
                sx = (int)Math.Ceiling(MainWindow.CurrentModel.Strip.UnitRow / 2.0);
                ex = MainWindow.CurrentModel.Strip.UnitRow - 1;
            }
            Action action = delegate
            {
                if (IndexInfo.Slave == false)
                    CamCtrl[IndexInfo.Index / VID.BP_ScanComplete_Count].Map.GoodColor();
                else
                    CamCtrl[IndexInfo.Index / VID.BP_ScanComplete_Count].Map.GoodColor(sx, ex);
            }; this.Dispatcher.Invoke(action);
            
            ResultInfo Result = PCSInstance.Vision[VisionNum].VisionResult;

            Surface surface;
            int nID = m_BP_ID[IndexInfo.Index];
            surface = Surface.BP1 + IndexInfo.Index;

            if (MainWindow.CurrentModel.ITS.UseITS && PCSInstance.Vision[VisionNum].VisionResult.IDMark.Status == 1)
            {
                try
                {
                    IDMarkResultInfo idmark = PCSInstance.Vision[VisionNum].VisionResult.IDMark;
                    string filter = "";
                    List<string> id = new List<string>();

                    id = DataMatrixCode.Algo_Conv_Square_DataMatrix2(idmark.Image, m_ptrMainWindow.currOrder, filter, m_ptrMainWindow.InspectionMonitoringCtrl.TableDataCtrl.p2DMarkCountBP);
                    if (id != null && id.Count > 0 && id[0].Trim() != "") StripID = id[0];
                    else
                    {
                        id = DataMatrixCode.Algo_Conv_Square_DataMatrix(idmark.Image, m_ptrMainWindow.currOrder, filter, m_ptrMainWindow.InspectionMonitoringCtrl.TableDataCtrl.p2DMarkCountBP);
                        if (id != null && id.Count > 0 && id[0].Trim() != "") StripID = id[0];
                        else
                        {
                            id = DataMatrixCode.GetDataMatrixMJ(idmark.Image, m_ptrMainWindow.currOrder, filter, m_ptrMainWindow.InspectionMonitoringCtrl.TableDataCtrl.p2DMarkCountBP);
                            if (id != null && id.Count > 0 && id[0].Trim() != "") StripID = id[0];
                            else
                            {
                                StripID = "";
                                bAutoNG = true;
                            }
                        }
                    }
                    m_ptrMainWindow.InspectionMonitoringCtrl.TableDataCtrl.p2DMarkCountBP++;
                    View2DMark(idmark.Image, id);
                    m_BPResult[IndexInfo.Index].IDString = StripID;

                    if (bAutoNG) { m_BPResult[IndexInfo.Index].AutoNG = true; }
                }
                catch
                {
                    MessageBox.Show("Setting.xml 파일의 ITS USE = 1 입니다.\n 정상인지 확인해 주세요.");
                }
            }

            if (Result != null)
            {
                int x = 0, y = 0;
                bool bColor = false;
                int resultCount = Result.ResultItemCount;
                if (resultCount == 0) Savequeue.bWorkDone[nID % QueueBuffer][IndexInfo.Index] = true;
                if (PCSInstance.Vision[VisionNum].m_iTrainError < 0)
                {
                    bAutoNG = true;
                }
                Result.UnitInspFailureCount = 0;
                if (resultCount > 0)
                {
                    m_nAllNGCount += resultCount;
                    BitmapSource MergedImage;

                    int ImageSize = 96;
                    if (MainWindow.Setting.General.ResultImageSizeType == false)
                        ImageSize = (MainWindow.Setting.General.ResultImageSize1 >= ImageSize) ? MainWindow.Setting.General.ResultImageSize1 : 96;
                    else
                        ImageSize = (MainWindow.Setting.General.ResultImageSize2 >= ImageSize) ? MainWindow.Setting.General.ResultImageSize2 : 300;
                    ImageCutter imageCutter = new ImageCutter(ImageSize);
                    if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] >= 6)
                    {
                        MergedImage = PCSInstance.Vision[VisionNum].GetMergedImage_Color();
                        bColor = true;
                        if (!imageCutter.CuttingImage_Color(MergedImage, 0, resultCount))
                            TrainErrorStop(-4, resultCount.ToString());
                    }
                    else
                    {
                        MergedImage = PCSInstance.Vision[VisionNum].GetMergedImage();
                        bColor = false;
                        if (!imageCutter.CuttingImage(MergedImage, 0, resultCount))
                            TrainErrorStop(-4, resultCount.ToString());
                    }

                    if (!imageCutter.CuttingImage(MergedImage, 0, resultCount))
                        TrainErrorStop(-4, resultCount.ToString());

                    action = delegate
                    {
                        ClearLastDefectRectangle(IndexInfo.CategorySurface, IndexInfo.Index);
                    }; this.Dispatcher.Invoke(action);

                    int resValue = 0;           
                    for (int i = 0; i < resultCount; i++)
                    {
                        if (Result.Results[i].BreakType == 2)
                        {
                            bAutoNG = true;
                        }
                        x = Result.Results[i].UnitPos.Y;
                      
                        if (Result.Results[i].SectionType == PCS.ModelTeaching.SectionTypeCode.UNIT_REGION ||
                          Result.Results[i].SectionType == PCS.ModelTeaching.SectionTypeCode.PSR_REGION)
                        { 
                            if (IndexInfo.Slave == false)
                            {
                                y = MainWindow.CurrentModel.Strip.UnitRow - Result.Results[i].UnitPos.X - 1; // 잘못된 코드 아님
                            }
                            else
                            {
                                y = (int)Math.Ceiling(MainWindow.CurrentModel.Strip.UnitRow / 2.0) - Result.Results[i].UnitPos.X - 1;
                            }

                            if (Result.Results[i].ResultType == (int)Common.Drawing.InspectionInformation.eVisInspectResultType.eInspResultPSRShift)
                            {
                                int val = MainWindow.NG_Info.BadNameToID("PSRShift");
                                m_BPResult[IndexInfo.Index].SetAll(val);

                                double res = MainWindow.Setting.SubSystem.IS.CamResolutionX[CID.BP];                             
                                if (Result.Results[i].DefectSize > 200000)
                                {
                                    Result.Results[i].DefectSize = (double)(Result.Results[i].DefectSize - 200000);
                                    Result.Results[i].DefectSize += 2000;
                                }
                                else 
                                {
                                    Result.Results[i].DefectSize = (double)(Result.Results[i].DefectSize - 100000);
                                    Result.Results[i].DefectSize += 1000;
                                }
                                m_BPResult[IndexInfo.Index].AutoNG = true;
                            }
                        }
                        else
                        {
                            y = Result.Results[i].UnitPos.X;
                        }

                    if (Result.Results[i].SectionType == PCS.ModelTeaching.SectionTypeCode.UNIT_REGION ||
                            Result.Results[i].SectionType == PCS.ModelTeaching.SectionTypeCode.PSR_REGION ||
                            Result.Results[i].SectionType == PCS.ModelTeaching.SectionTypeCode.RAW_REGION
                            )
                        {
                            //resValue = (int)ConvertResultTypeToEnumNumber(Result.Results[i].ResultType);
                            resValue = MainWindow.NG_Info.ResultTypeToID(Result.Results[i].ResultType);
                            if (resValue < 0)
                            {
                                TrainErrorStop(-5, "NG_Info POP DB 에러");
                                return;
                            }

                            if (Result.Results[i].ResultType == (int)Common.Drawing.InspectionInformation.eVisInspectResultType.eInspResultUnitPattern ||
                                Result.Results[i].ResultType == (int)Common.Drawing.InspectionInformation.eVisInspectResultType.eInspResultBufferError)
                                m_BPResult[IndexInfo.Index].AutoNG = true;

                            m_BPResult[IndexInfo.Index].Set(x, y, resValue);
                            if (!MainWindow.CurrentModel.UseVerify)
                            {
                                if (MainWindow.AutoNG_Check[resValue - 1])
                                {
                                    m_BPResult[IndexInfo.Index].AutoNG = true;
                                }
                            }

                        }
                        else // 외곽 Section, ID Section, Strip Section 
                        {
                            int val = 0;
                            if (Result.Results[i].ResultType == (int)eVisInspectResultType.eInspResultNoReSizeVentHole)
                                val = MainWindow.NG_Info.ResultTypeToID(Result.Results[i].ResultType);
                            else
                                val = MainWindow.NG_Info.ResultTypeToID(1); // ResultType=1 외곽불량

                            if (val < 0)
                            {
                                TrainErrorStop(-5, "NG_Info POP DB 에러");
                                return;
                            }

                            m_BPResult[IndexInfo.Index].OuterNG = true;
                            m_BPResult[IndexInfo.Index].SetAll(val);
                            
                            m_BPResult[IndexInfo.Index].AutoNG = true;
                        }

                        if (i < imageCutter.RefImage.Count && i < imageCutter.DefImage.Count)
                        {
                            BitmapSource refClone = imageCutter.RefImage[i].Clone();
                            BitmapSource defClone = imageCutter.DefImage[i].Clone();
                            refClone.Freeze(); defClone.Freeze();
                            ResultItem resultItem = Result.Results[i].Clone();

                            #region 23.03.13 suoow 수정
                            Result_Convert rc = InspResultAt(IndexInfo.Index, resultItem.SectionID, resultItem.RoiID, resultItem.InspID, resultItem.Channel);

                            string Insp = string.Format("{0}_{1}_{2}_{3}", rc.Name.Trim().Replace(" ", ""), rc.Mode, rc.LowerThresh, rc.UpperThresh);
                            string strResult = MainWindow.NG_Info.ResultTypeToName(resultItem.ResultType);
                            if (strResult == "")
                            {
                                TrainErrorStop(-5, "NG_Info POP DB 에러");
                                return;
                            }

                            if (Result.Results[i].SectionType == PCS.ModelTeaching.SectionTypeCode.OUTER_REGION)
                            {
                                strResult += "_외곽";
                            }
                            string FileName = String.Format("CAM{0} ID[{1:D4}] X={2:D2} Y={3:D2} DEF P={4:D5},{5:D5} S={6:F2} {7} {8} {9} {10} {11}",
                                                  (int)surface, nID + TableDataCtrl.BeforeCount, x + 1, y + 1, resultItem.RelativeDefectCenter.X, resultItem.RelativeDefectCenter.Y,
                                                  resultItem.DefectSize, strResult.Trim().Replace(" ",""), CategorySurface.BP.ToString(), IndexInfo.Index, resultItem.ChannelString, Insp);

                            string dnn_ret = "";
                            if (MainWindow.Setting.General.UseAI)
                            {
                                ///////////////// Dnn Classification
                                if (strResult.Contains("Align") || strResult == "PSRShift")
                                {
                                    m_BPResult[IndexInfo.Index].SetDnn(x, y, MainWindow.NG_Info.BadNameToID(strResult), true);
                                    dnn_ret = string.Format("{0} {1} {2} {3} {4} {5:F2}.png", -1, strResult, strResult, -1, 0, 0.0);
                                }
                                else
                                {
                                    DNN.YoloResult res = MainWindow.Predect_Yolo_Net((int)surface, FileName, refClone, defClone, i);

                                    if (dnn_ret == "")
                                    {
                                        //////////////// 저장 이미지명은 결과를 반영
                                        if(res.BadName =="BPOpen" || res.BadName == "BPShort")
                                        {
                                            if(strResult.Contains("BP"))
                                                res.BadName = strResult;
                                        }
                                        dnn_ret = string.Format("{0} {1} {2} {3} {4} {5:F2}.png", res.ClassID, res.ClassName.Trim().Replace(" ", ""), res.BadName.Trim().Replace(" ", ""), res.isDetection ? 1 : 0, 0, res.prob);
                                        if (res.BadName == "None") res.BadName = strResult;
                                        if (!strResult.Contains("_외곽"))
                                        {
                                            m_BPResult[IndexInfo.Index].SetDnn(x, y, MainWindow.NG_Info.BadNameToID(res.BadName), true);
                                        }
                                    }
                                    else
                                    {
                                        m_BPResult[IndexInfo.Index].SetDnn(x, y, MainWindow.NG_Info.BadNameToID(res.BadName), true);
                                    }

                                    if (dnn_ret == "")
                                        //////////////// 저장 이미지명은 결과를 반영
                                        dnn_ret = string.Format("{0} {1} {2} {3} {4} {5:F2}.png", res.ClassID, res.ClassName.Trim().Replace(" ", ""), res.BadName.Trim().Replace(" ", ""), res.isDetection ? 1 : 0, 0, res.prob);
                                }
                            }
                            else
                                dnn_ret = string.Format("{0} {1} {2} {3} {4} {5:F2}.png", -1, "-", "-", 0, 0, 0.0);

                            FileName += " ";
                            FileName += dnn_ret;
                            //MainWindow.Log("PCS", SeverityLevel.DEBUG, String.Format("{0}", FileName), true);
                            Object obj = new object[] { i, x, y, resultCount, surface, refClone, defClone, resultItem, nID, FileName, bColor, Result.Results[i].SectionType };
                            #endregion
                            ResultQueue[Surfaceindex].Enqueue(obj);
                            if (MainWindow.CurrentModel.UseAI)
                                Set_ICS_Def_Data(x, y, resultItem.RelativeDefectCenter.X, resultItem.RelativeDefectCenter.Y, resultItem.UnitAlignOffset_X, resultItem.UnitAlignOffset_Y, FileName,
                                    resultItem.DefectSize, resultItem.SectionType, resultItem.DefectPosX, resultItem.DefectPosY, IndexInfo, BP2Section_info,
                                    MainWindow.Setting.General.ICS_OFFSET_BP_X, MainWindow.Setting.General.ICS_OFFSET_BP_Y, ref SectionTypeDefect);
                        }
                    }
                    imageCutter.Clear();                   
                    if (bAutoNG) { m_BPResult[IndexInfo.Index].AutoNG = true; }
                }
                if (MainWindow.CurrentModel.UseAI)
                {
                    Object obj_ICS = new object[] { Surfaceindex, ICS_LotFolderPath, MainWindow.ModelInfo, SectionData_ICS[IndexInfo.ScanIndex].ToList(), SectionTypeDefect.ToList(), MainWindow.AlignOffset_ICS[IndexInfo.ScanIndex].ToList() };
                    Savequeue.ICS_ResultQueue[nID % QueueBuffer][Surfaceindex].Enqueue(obj_ICS);
                    SectionTypeDefect.Clear();
                    bool test = false;
                    if (test)
                    {
                        object obj1;
                        Savequeue.ICS_ResultQueue[nID % QueueBuffer][Surfaceindex].TryDequeue(out obj1);
                        Write_ICS_Data(obj1);
                    }
                }
            }
        }

        public void GetInspectResult_CA(int nNum, int nGrabSide)
        {
            MainWindow.IndexInfo IndexInfo = MainWindow.Convert_Vision(CategorySurface.CA, nNum, nGrabSide);
            //MainWindow.Log("IS", SeverityLevel.DEBUG, string.Format("ID : {0} , GrabSide : {1}, CA : Index {2}", nNum, nGrabSide, IndexInfo.Index), true);
            DeviceController PCSInstance = m_ptrMainWindow.PCSInstance;
            int FrameGrabberIndex = VID.CalcIndex(IndexInfo.VisionIndex);
            bool bAutoNG = false;
            int VisionNum = IndexInfo.VisionIndex;
            List<List<DefInfo>> SectionTypeDefect = new List<List<DefInfo>>();
            for (int j = 0; j < Enum.GetValues(typeof(SectionArea)).Length; j++) SectionTypeDefect.Add(new List<DefInfo>());
            int Surfaceindex = IndexInfo.Index + MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count;
            if (PCSInstance == null)
                return;
            Action action = delegate
            {
                CamCtrl[MainWindow.Setting.Job.BPCount + IndexInfo.Index].Map.GoodColor();
            }; this.Dispatcher.Invoke(action);


            ResultInfo Result = PCSInstance.Vision[VisionNum].VisionResult;

            Surface surface;
            int nID = m_CA_ID[IndexInfo.Index];
            surface = Surface.CA1 + IndexInfo.Index;

            if (MainWindow.CurrentModel.ITS.UseITS && PCSInstance.Vision[VisionNum].VisionResult.IDMark.Status == 1)
            {
                try
                {
                    IDMarkResultInfo idmark = PCSInstance.Vision[VisionNum].VisionResult.IDMark;
                    string filter = "";
                    List<string> id = new List<string>();
                    id = DataMatrixCode.Algo_Conv_Square_DataMatrix2(idmark.Image, m_ptrMainWindow.currOrder, filter, m_ptrMainWindow.InspectionMonitoringCtrl.TableDataCtrl.p2DMarkCountCA);
                    if (id != null && id.Count > 0 && id[0].Trim() != "") StripID = id[0];
                    else
                    {
                        id = DataMatrixCode.Algo_Conv_Square_DataMatrix(idmark.Image, m_ptrMainWindow.currOrder, filter, m_ptrMainWindow.InspectionMonitoringCtrl.TableDataCtrl.p2DMarkCountCA);
                        if (id != null && id.Count > 0 && id[0].Trim() != "") StripID = id[0];
                        else
                        {
                            id = DataMatrixCode.GetDataMatrixMJ(idmark.Image, m_ptrMainWindow.currOrder, filter, m_ptrMainWindow.InspectionMonitoringCtrl.TableDataCtrl.p2DMarkCountCA);
                            if (id != null && id.Count > 0 && id[0].Trim() != "") StripID = id[0];
                            else
                            {
                                StripID = "";
                                bAutoNG = true;
                            }
                        }
                    }
                    m_ptrMainWindow.InspectionMonitoringCtrl.TableDataCtrl.p2DMarkCountCA++;
                    View2DMark(idmark.Image, id);
                    m_CAResult[IndexInfo.Index].IDString = StripID;

                    if (bAutoNG) { m_CAResult[IndexInfo.Index].AutoNG = true;}
                }
                catch
                {
                    MessageBox.Show("Setting.xml 파일의 ITS USE = 1 입니다.\n 정상인지 확인해 주세요.");
                }
            }

            if (Result != null)
            {
                int x = 0, y = 0;
                bool bColor = false;
                int resultCount = Result.ResultItemCount;
                if (resultCount == 0) Savequeue.bWorkDone[nID % QueueBuffer][IndexInfo.Index + MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count] = true;
                if (PCSInstance.Vision[VisionNum].m_iTrainError < 0)
                {
                    bAutoNG = true;
                }
                Result.UnitInspFailureCount = 0;
                if (resultCount > 0)
                {
                    m_nAllNGCount += resultCount;
                    BitmapSource MergedImage;

                    int ImageSize = 96;
                    if (MainWindow.Setting.General.ResultImageSizeType == false)
                        ImageSize = (MainWindow.Setting.General.ResultImageSize1 >= ImageSize) ? MainWindow.Setting.General.ResultImageSize1 : 96;
                    else
                        ImageSize = (MainWindow.Setting.General.ResultImageSize2 >= ImageSize) ? MainWindow.Setting.General.ResultImageSize2 : 300;
                    ImageCutter imageCutter = new ImageCutter(ImageSize);
                    if (MainWindow.Setting.SubSystem.IS.FGType[FrameGrabberIndex] >= 6)
                    {
                        MergedImage = PCSInstance.Vision[VisionNum].GetMergedImage_Color();
                        bColor = true;
                        if (!imageCutter.CuttingImage_Color(MergedImage, 0, resultCount))
                            TrainErrorStop(-4, resultCount.ToString());
                    }
                    else
                    {
                        MergedImage = PCSInstance.Vision[VisionNum].GetMergedImage();
                        bColor = false;
                        if (!imageCutter.CuttingImage(MergedImage, 0, resultCount))
                            TrainErrorStop(-4, resultCount.ToString());
                    }
                    action = delegate
                    {
                        ClearLastDefectRectangle(IndexInfo.CategorySurface, IndexInfo.Index);
                    }; this.Dispatcher.Invoke(action);

                    int resValue = 0;            
                    for (int i = 0; i < resultCount; i++)
                    {
                        if (Result.Results[i].BreakType == 2)
                        {
                            bAutoNG = true;
                        }
                        x = Result.Results[i].UnitPos.Y;

                        if (Result.Results[i].SectionType == PCS.ModelTeaching.SectionTypeCode.UNIT_REGION ||
                            Result.Results[i].SectionType == PCS.ModelTeaching.SectionTypeCode.PSR_REGION)
                        {
                            y = MainWindow.CurrentModel.Strip.UnitRow - Result.Results[i].UnitPos.X - 1;
                        }
                        else
                        {
                            y = Result.Results[i].UnitPos.X;
                        }

                        if (Result.Results[i].SectionType == PCS.ModelTeaching.SectionTypeCode.UNIT_REGION ||
                            Result.Results[i].SectionType == PCS.ModelTeaching.SectionTypeCode.PSR_REGION ||
                            Result.Results[i].SectionType == PCS.ModelTeaching.SectionTypeCode.RAW_REGION)
                        {
                            resValue = MainWindow.NG_Info.ResultTypeToID(Result.Results[i].ResultType);
                            if (resValue < 0)
                            {
                                TrainErrorStop(-5, "NG_Info POP DB 에러");
                                return;
                            }

                            if (Result.Results[i].ResultType == (int)Common.Drawing.InspectionInformation.eVisInspectResultType.eInspResultUnitPattern ||
                                Result.Results[i].ResultType == (int)Common.Drawing.InspectionInformation.eVisInspectResultType.eInspResultBufferError)
                                m_CAResult[IndexInfo.Index].AutoNG = true;

                            m_CAResult[IndexInfo.Index].Set(x, y, resValue);
                            if (!MainWindow.CurrentModel.UseVerify)
                            {
                                if (MainWindow.AutoNG_Check[resValue - 1])
                                {
                                    m_CAResult[IndexInfo.Index].AutoNG = true;
                                }
                            }
                        }
                        else // 외곽 Section, ID Section, Strip Section 
                        {
                            int val = 0;
                            if (Result.Results[i].ResultType == (int)eVisInspectResultType.eInspResultNoReSizeVentHole)
                                val = MainWindow.NG_Info.ResultTypeToID(Result.Results[i].ResultType);
                            else
                                val = MainWindow.NG_Info.ResultTypeToID(1); // ResultType=1 외곽불량

                            if (val < 0)
                            {
                                TrainErrorStop(-5, "NG_Info POP DB 에러");
                                return;
                            }

                            m_CAResult[IndexInfo.Index].OuterNG = true;
                            m_CAResult[IndexInfo.Index].SetAll(val);

                            m_CAResult[IndexInfo.Index].AutoNG = true;
                        }

                        if (i < imageCutter.RefImage.Count && i < imageCutter.DefImage.Count)
                        {
                            BitmapSource refClone = imageCutter.RefImage[i].Clone();
                            BitmapSource defClone = imageCutter.DefImage[i].Clone();
                            refClone.Freeze(); defClone.Freeze();
                            ResultItem resultItem = Result.Results[i].Clone();

                            #region 23.03.13 suoow 수정

                            Result_Convert rc = InspResultAt(IndexInfo.Index + MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count, resultItem.SectionID, resultItem.RoiID, resultItem.InspID, resultItem.Channel);

                            string Insp = string.Format("{0}_{1}_{2}_{3}", rc.Name.Trim().Replace(" ", ""), rc.Mode, rc.LowerThresh, rc.UpperThresh);
                            string strResult = MainWindow.NG_Info.ResultTypeToName(resultItem.ResultType);
                            if (strResult == "")
                            {
                                TrainErrorStop(-5, "NG_Info POP DB 에러");
                                return;
                            }
                            if (Result.Results[i].SectionType == PCS.ModelTeaching.SectionTypeCode.OUTER_REGION)
                            {
                                strResult += "_외곽";
                            }
                            string FileName = String.Format("CAM{0} ID[{1:D4}] X={2:D2} Y={3:D2} DEF P={4:D5},{5:D5} S={6:F2} {7} {8} {9} {10} {11}",
                                                  (int)surface, nID + TableDataCtrl.BeforeCount, x + 1, y + 1, resultItem.RelativeDefectCenter.X, resultItem.RelativeDefectCenter.Y,
                                                  resultItem.DefectSize, strResult.Trim().Replace(" ", ""), CategorySurface.CA.ToString(), IndexInfo.Index, resultItem.ChannelString, Insp);

                            string dnn_ret = "";
                            if (MainWindow.Setting.General.UseAI)
                            {
                                ///////////////// Dnn Classification
                                if (strResult.Contains("Align") || strResult == "PSRShift")
                                {
                                    m_CAResult[IndexInfo.Index].SetDnn(x, y, MainWindow.NG_Info.BadNameToID(strResult), true);
                                    dnn_ret = string.Format("{0} {1} {2} {3} {4} {5:F2}.png", -1, strResult, strResult, -1, 0, 0.0);
                                }
                                else
                                {
                                    DNN.YoloResult res = MainWindow.Predect_Yolo_Net((int)surface, FileName, refClone, defClone, i);
                                    if (dnn_ret == "")
                                    {
                                        //////////////// 저장 이미지명은 결과를 반영
                                        dnn_ret = string.Format("{0} {1} {2} {3} {4} {5:F2}.png", res.ClassID, res.ClassName.Trim().Replace(" ", ""), res.BadName.Trim().Replace(" ", ""), res.isDetection ? 1 : 0, 0, res.prob);
                                        if (res.BadName == "None") res.BadName = strResult;
                                        if (!strResult.Contains("_외곽"))
                                        {
                                            m_CAResult[IndexInfo.Index].SetDnn(x, y, MainWindow.NG_Info.BadNameToID(res.BadName), true);
                                        }
                                    }
                                    else
                                    {
                                        m_CAResult[IndexInfo.Index].SetDnn(x, y, MainWindow.NG_Info.BadNameToID(res.BadName), true);
                                    }
                                }
                            }
                            else
                                dnn_ret = string.Format("{0} {1} {2} {3} {4} {5:F2}.png", -1, "-", "-", 0, 0, 0.0);

                            FileName += " ";
                            FileName += dnn_ret;
                            //MainWindow.Log("PCS", SeverityLevel.DEBUG, String.Format("{0}", FileName), true);
                            Object obj = new object[] { i, x, y, resultCount, surface, refClone, defClone, resultItem, nID, FileName, bColor, Result.Results[i].SectionType };
                            #endregion
                            ResultQueue[Surfaceindex].Enqueue(obj);
                            if (MainWindow.CurrentModel.UseAI)
                                Set_ICS_Def_Data(x, y, resultItem.RelativeDefectCenter.X, resultItem.RelativeDefectCenter.Y, resultItem.UnitAlignOffset_X, resultItem.UnitAlignOffset_Y, FileName, 
                                    resultItem.DefectSize, resultItem.SectionType, resultItem.DefectPosX, resultItem.DefectPosY, IndexInfo, BP2Section_info,
                                    MainWindow.Setting.General.ICS_OFFSET_CA_X, MainWindow.Setting.General.ICS_OFFSET_CA_Y, ref SectionTypeDefect);
                        }
                    }
                    imageCutter.Clear();                 
                    if (bAutoNG) { m_CAResult[IndexInfo.Index].AutoNG = true; }
                }
                if (MainWindow.CurrentModel.UseAI)
                {
                    Object obj_ICS = new object[] { Surfaceindex, ICS_LotFolderPath, MainWindow.ModelInfo, SectionData_ICS[IndexInfo.ScanIndex].ToList(), SectionTypeDefect.ToList(), MainWindow.AlignOffset_ICS[IndexInfo.ScanIndex].ToList() };
                    Savequeue.ICS_ResultQueue[nID % QueueBuffer][Surfaceindex].Enqueue(obj_ICS);
                    SectionTypeDefect.Clear();
                    bool test = false;
                    if (test)
                    {
                        object obj1;
                        Savequeue.ICS_ResultQueue[nID % QueueBuffer][Surfaceindex].TryDequeue(out obj1);
                        Write_ICS_Data(obj1);
                    }
                }
            }           
        }

        public Result_Convert InspResultAt(int anViewer_index, int anSectionID, int anRoiID, int anInspID, int anchannel)
        {
            Result_Convert ret = MainWindow.lstRC.Find(st => (st.Target_Viewer == anViewer_index) && (st.SectionID == anSectionID) && (st.RoiID == anRoiID) && (st.InspID == anInspID) && (st.Channel == anchannel));
            if (ret == null)
            {
                ret = new Result_Convert();
                ret.Name = "-";
                ret.InspectType = 0;
                ret.Mode = -1;
                ret.LowerThresh = 0;
                ret.UpperThresh = 0;
            }
            return ret;

        }
        private void ThreadView()
        {
            while (bThreadView)
            {
                object obj;
                Parallel.For(0, ResultQueue.Count, i =>
                {
                    if (ResultQueue[i].Count > 0)
                    {
                        ResultQueue[i].TryDequeue(out obj); SaveAndView(obj);
                    }
                });       
                Thread.Sleep(15);
            }
            InspectProcess.View_Done = true;
        }

        private void ImageSaveQueue()
        {
            int Index = 0;
            int QueueCount = MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count +
                             MainWindow.Setting.Job.CACount +
                             MainWindow.Setting.Job.BACount;
            int currentIndex = 0;
            while (bThreadSave)
            {
                if (InspectProcess.m_bStripEnd)
                {
                    currentIndex = Index % QueueBuffer;
                    while (true)
                    {
                        bool finish = true;
                        for (int z = 0; z < QueueCount; z++)
                        {
                            finish = finish && Savequeue.bWorkDone[currentIndex][z];
                            //MainWindow.Log("PCS", SeverityLevel.ERROR, string.Format("bWorkDone[{0}][{1}] = {2}", currentIndex, z, Savequeue.bWorkDone[currentIndex][z]));
                        }
                        if (finish) break;
                        //MainWindow.Log("PCS", SeverityLevel.ERROR, string.Format("==========================Wait Finish================================="));
                        Thread.Sleep(500);
                    }
                    InspectProcess.m_bStripEnd = false;
                    //MainWindow.Log("PCS", SeverityLevel.ERROR, string.Format("==========================Save Start================================="));             
                    try
                    {
                        Parallel.For(0, QueueCount, i =>
                        {
                            object obj;
                            while (Savequeue.ImageSave_Queue[currentIndex][i].TryDequeue(out obj))
                            {
                                SaveImage(obj);
                            }
                        });
                        if (MainWindow.CurrentModel.UseAI)
                        {
                            Parallel.For(0, QueueCount, i =>
                            {
                                object obj1;
                                while (Savequeue.ICS_ResultQueue[currentIndex][i].TryDequeue(out obj1))
                                {
                                    Write_ICS_Data(obj1);
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        MainWindow.Log("PCS", SeverityLevel.ERROR, String.Format("Parallel.For Error : {0}", ex, true));
                    }              
                    for (int z = 0; z < QueueCount; z++)
                    {
                        MainWindow.IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(z);
                        if (IndexInfo.CategorySurface == CategorySurface.BP)
                        {
                            if (BPInspectSkip[IndexInfo.Index / VID.BP_ScanComplete_Count]) Savequeue.bWorkDone[currentIndex][z] = true;
                            else Savequeue.bWorkDone[currentIndex][z] = false;
                        }
                        else if (IndexInfo.CategorySurface == CategorySurface.CA)
                        {
                            if (CAInspectSkip[IndexInfo.Index]) Savequeue.bWorkDone[currentIndex][z] = true;
                            else Savequeue.bWorkDone[currentIndex][z] = false;
                        }
                        else if (IndexInfo.CategorySurface == CategorySurface.BA)
                        {
                            if (BAInspectSkip[IndexInfo.Index]) Savequeue.bWorkDone[currentIndex][z] = true;
                            else Savequeue.bWorkDone[currentIndex][z] = false;
                        }
                    }
                    Index++;
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
            InspectProcess.ImageSave_Done = true;
        }
        
        private void SaveAndView(object obj)
        {
            object[] param = (object[])obj;
            int i = (int)param[0];
            int x = (int)param[1];
            int y = (int)param[2];
            int resultCount = (int)param[3];
            Surface surface = (Surface)param[4];
            BitmapSource refImage = ((BitmapSource)param[5]);
            BitmapSource defImage = ((BitmapSource)param[6]);
            ResultItem resultItem = (ResultItem)param[7];
            int nID = (int)param[8];
            string filename = (string)param[9];
            bool isColor = Convert.ToBoolean(param[10]);
            string SectionType = (string)param[11];

            int ScanCount = 0;
            CategorySurface CategorySurface = new CategorySurface();
            int QueueIndex = -1;
            bool Check = false;
            if (surface.ToString().Contains("BP")) 
            { 
                CategorySurface = CategorySurface.BP; 
                ScanCount = (int)((surface - Surface.BP1) / VID.BP_ScanComplete_Count); 
                QueueIndex = (int)(surface - Surface.BP1);
                Check = true;
            }
            else if (surface.ToString().Contains("CA")) 
            { 
                CategorySurface = CategorySurface.CA;
                ScanCount = (int)(surface - Surface.CA1); 
                QueueIndex = MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count + (int)(surface - Surface.CA1);
                Check = true;
            }
            else if (surface.ToString().Contains("BA"))
            {
                CategorySurface = CategorySurface.BA;
                ScanCount = (int)(surface - Surface.BA1);
                QueueIndex = MainWindow.Setting.Job.BPCount * VID.BP_ScanComplete_Count + MainWindow.Setting.Job.CACount + (int)(surface - Surface.BA1);
                Check = true;
            }
            if (!Check) { MainWindow.Log("PCS", SeverityLevel.ERROR, "Queue가 이상함 ", true); return; }
            Object obj_Save = new object[] { refImage, defImage, surface, filename };
            Savequeue.ImageSave_Queue[nID % QueueBuffer][QueueIndex].Enqueue(obj_Save);
            if (Savequeue.ImageSave_Queue[nID % QueueBuffer][QueueIndex].Count == resultCount)
                Savequeue.bWorkDone[nID % QueueBuffer][QueueIndex] = true;


            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                BitmapSource UIrefImage;
                BitmapSource UIdefImage; 

                if (MainWindow.Setting.General.ResultImageSizeType == true)
                {
                    int imgSize = MainWindow.Setting.General.ResultImageSize1;
                    ImageCutter cutter = new ImageCutter(imgSize);
                    UIrefImage = cutter.CropImage(refImage.Clone(), imgSize, imgSize, isColor);
                    UIdefImage = cutter.CropImage(defImage.Clone(), imgSize, imgSize, isColor);
                }
                else
                {
                    UIrefImage = refImage.Clone();
                    UIdefImage = defImage.Clone();
                }
                //if (UIrefImage.CanFreeze) UIrefImage.Freeze();
                //if (UIdefImage.CanFreeze) UIdefImage.Freeze();
                if (refImage != null && defImage != null)
                {
                    if (i < (resultCount - 1))
                        RoundRobinImgSet(CategorySurface, ScanCount, resultItem, UIrefImage, UIdefImage, x, y, false, SectionType);
                    else
                        RoundRobinImgSet(CategorySurface, ScanCount, resultItem, UIrefImage, UIdefImage, x, y, true, SectionType);
                }
            }));
        }
        #endregion


        #region Result DB Interface

        public void UpdateClosedVerifyInfo()
        {
            if (ConnectFactory.DBConnector() != null) ConnectFactory.DBConnector().StartTrans();
            if (m_VerifyManager.UpdateClosed(MainWindow.Setting.General.MachineCode, TableDataCtrl.ModelName) >= 0)
            {
                ConnectFactory.DBConnector().Commit();
            }
            else
            {
                ConnectFactory.DBConnector().Rollback();
            }
        }

        public bool CheckLotExist()
        {
            if (LotExist())
            {
                MessageBoxResult msgBoxResult = MessageBox.Show("로트 검사정보가 존재 합니다. \n이어서 시작하려면 '예' \n처음부터 시작하려면 '아니오'를 선택해 주세요.",
                                                    "자동검사",
                                                    MessageBoxButton.YesNoCancel,
                                                    MessageBoxImage.Question);
                if (msgBoxResult == MessageBoxResult.No)
                {
                    if (!TableDataCtrl.ClearData())
                    {

                        MessageBox.Show("Lot 정보를 등록할 수 없습니다.", "Error");
                        MainWindow.Log("PCS", SeverityLevel.ERROR, "Lot 정보를 등록하지 못하였습니다.", true);
                        return false;
                    }
                    if (MainWindow.CurrentModel.InspectMode != 4)
                    {
                        string strFolerPath = String.Format("{0}{1}\\", MainWindow.Setting.General.IDMarkPath, TableDataCtrl.LotNo);
                        DirectoryManager.DeleteDirectory(strFolerPath);
                    }
                    MainWindow.OldMarkID = 0;

                    //VRS 사용시 서버에 저장된 Map 지움
                    if (MainWindow.CurrentModel.UseVerify)
                    {
                        string mapPath = @"e:\VRSBinCodeMap" + string.Format("\\{0}\\{1}\\{2}", MainWindow.CurrentGroup.Name, MainWindow.CurrentModel.Name, this.TableDataCtrl.LotNo);
                        DirectoryManager.DeleteDirectory(mapPath);
                    }

                    m_ptrMainWindow.ITS_info_control.DeleteResultFiles(MainWindow.Setting.General.ITSPath2, m_ptrMainWindow.currOrder);
                    m_ptrMainWindow.ITS_info_control.DeleteResultFiles(MainWindow.Setting.General.ITSPath3, m_ptrMainWindow.currOrder);

                    //if (MainWindow.Setting.General.UseXML)
                    //    MainWindow.stdManager.DirectoryClear();
                }
                else if (msgBoxResult == MessageBoxResult.Cancel)
                {
                    return false;
                }
                else
                {
                    MessageBoxResult msgBoxFailResult = MessageBox.Show("폐기 불량 정보를 리셋 하시겠습니까?", "자동검사",
                        MessageBoxButton.YesNo);
                    if (msgBoxFailResult == MessageBoxResult.Yes)
                    {
                        TableDataCtrl.ResetFail();
                    }
                }
            }
            else
            {
                if (!TableDataCtrl.ClearData())
                {
                    MessageBox.Show("Lot 정보를 등록할 수 없습니다.", "Error");
                    MainWindow.Log("PCS", SeverityLevel.ERROR, "Lot 정보를 등록하지 못하였습니다.", true);
                    return false;
                }
                if (MainWindow.CurrentModel.InspectMode != 4)
                {
                    string strFolerPath = String.Format("{0}{1}\\", MainWindow.Setting.General.IDMarkPath, TableDataCtrl.LotNo);
                    DirectoryManager.DeleteDirectory(strFolerPath);
                }
                MainWindow.OldMarkID = 0;
            }
            MainWindow.OldMarkID = GetMarkID();
            return true;
        }

        public int GetMarkID()
        {
            int nID = 0;
            try
            {
                string strFolerPath = String.Format("{0}{1}\\", MainWindow.Setting.General.IDMarkPath, TableDataCtrl.LotNo);
                if (!Directory.Exists(strFolerPath))
                    Directory.CreateDirectory(strFolerPath);
                string[] files = Directory.GetFiles(strFolerPath);
                foreach (string file in files)
                {
                    if (file.Length > 15)
                    {
                        if (file.Contains(".xml"))
                        {

                            int pos = file.IndexOf(".xml");
                            string n = file.Substring(pos - 4, 4);
                            try
                            {
                                nID = Math.Max(nID, Convert.ToInt32(n));
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch { }
            return nID;
        }

        private void SaveImage(BitmapSource refImg, BitmapSource defImg, int anCamID, string FileName)// int anStripID, ResultItem result, int x, int y, int index)
        {
            if (refImg == null || defImg == null) return;

            string strPath = MainWindow.Setting.General.ResultPath;
            string strImageFolerPath = String.Format(@"{0}/{1}/{2}/{3}/Image/", strPath, TableDataCtrl.GroupName, TableDataCtrl.ModelName, TableDataCtrl.Re_LotNo);
            Common.DirectoryManager.CreateDirectory(strImageFolerPath);
            string strBadInfoFile = strImageFolerPath + String.Format("Bad_Info_CAM{0}.txt", anCamID);
            if (!File.Exists(strBadInfoFile))
            {
                FileStream fs = File.Create(strBadInfoFile);
                fs.Close();
            }
           
            string strREFImagePath = strImageFolerPath + FileName.Replace("DEF", "REF");
            string strDEFImagePath = strImageFolerPath + FileName;

            BitmapSource RefImage = refImg;
            try
            {
                File.AppendAllText(strBadInfoFile, strDEFImagePath + "\r\n");
                using (FileStream stream = new FileStream(strREFImagePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    stream.Flush();
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(RefImage));
                    encoder.Save(stream);
                    encoder.Frames.Clear();
                    stream.Dispose();
                }
            }
            catch { }
            BitmapSource DefImage = defImg;
            try
            {
                using (FileStream stream = new FileStream(strDEFImagePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    stream.Flush();
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(DefImage));
                    encoder.Save(stream);
                    encoder.Frames.Clear();
                    stream.Dispose();
                }
            }
            catch { }
        }

        private void SaveImage(object obj)
        {
            try
            {
                object[] param = (object[])obj;
                BitmapSource refImg = ((BitmapSource)param[0]);
                BitmapSource defImg = ((BitmapSource)param[1]);
                Surface surface = (Surface)param[2];
                int anCamID = (int)surface;
                string fileName = (string)param[3];
                if (refImg == null || defImg == null) return;
                // 멀티스레드 대응용
                refImg.Freeze();
                defImg.Freeze();

                string strPath = MainWindow.Setting.General.ResultPath;
                string strImageFolderPath = String.Format(@"{0}/{1}/{2}/{3}/Image/", strPath, TableDataCtrl.GroupName, TableDataCtrl.ModelName, TableDataCtrl.Re_LotNo);
     
                string curID = Current_ID ?? "";
                string StripName = "";
                if (curID != "")///////ITS가 있을때
                {
                    fileName = Regex.Replace(fileName, @"\[\d{4}\]", "[" + curID + "]");
                    StripName = curID;
                }
                else//////PLC ID
                {
                    //Match match = Regex.Match(fileName, @"\[(\d+)\]");
                    //StripName = match.Groups[1].Value;

                    Match match = Regex.Match(fileName, @"\[(\d+)\]");
                    fileName = Regex.Replace(fileName, @"\[\d{4}\]", "[" + TableDataCtrl.Re_LotNo + " " + match.Groups[1].Value + "]");
                    StripName = TableDataCtrl.Re_LotNo + " " + match.Groups[1].Value;
                }
                
                string strImageStripFolderPath = String.Format(@"{0}/{1}/", strImageFolderPath, StripName);                
                if (!Directory.Exists(strImageStripFolderPath)) Common.DirectoryManager.CreateDirectory(strImageStripFolderPath);

                string strBadInfoFile = strImageFolderPath + String.Format("Bad_Info_CAM{0}.txt", anCamID);
                if (!File.Exists(strBadInfoFile))
                {
                    FileStream fs = File.Create(strBadInfoFile);
                    fs.Close();
                }

                string strREFImagePath = Path.Combine(strImageStripFolderPath, fileName.Replace("DEF", "REF"));
                string strDEFImagePath = Path.Combine(strImageStripFolderPath, fileName);

                File.AppendAllText(strBadInfoFile, strDEFImagePath + "\r\n");

                SaveBitmapSource(refImg, strREFImagePath);
                SaveBitmapSource(defImg, strDEFImagePath);
            }
            catch (Exception ex)
            {
                MainWindow.Log("PCS", SeverityLevel.ERROR, "[SaveImage 예외]", true);
            }
        }
        private void SaveBitmapSource(BitmapSource image, string path)
        {
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(stream);
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log("PCS", SeverityLevel.DEBUG, String.Format("[SaveBitmapSource 예외]path={0} ex={1}", path, ex), true);
            }
        }
        public DefectResult SetDefectResult(ResultItem aResult, string strWorkType, int anStripID)
        {
            DefectResult aDefectResult = new DefectResult();

            aDefectResult.SectionCode = aResult.SectionID.ToString("D4");
            aDefectResult.WorkType = strWorkType;
            aDefectResult.RoiCode = aResult.RoiID.ToString("D4");

            aDefectResult.DefectScore = aResult.DefectScore;
            aDefectResult.DefectBoundary = aResult.AbsoluteDefectRect.ToString();
            aDefectResult.DefectPosX = aResult.UnitPos.X;
            aDefectResult.DefectPosY = aResult.UnitPos.Y;
            aDefectResult.DefectCenterX = aResult.RelativeDefectCenter.X;
            aDefectResult.DefectCenterY = aResult.RelativeDefectCenter.Y;
            aDefectResult.DefectSize = aResult.DefectSize;
            aDefectResult.DefectCode = aResult.ResultType.ToString("D4");

            if (m_bAutoNG == false)
            {
                m_bAutoNG = InspectDataManager.IsAutoNG(aDefectResult.DefectCode);
            }

            aDefectResult.ResultID = aResult.ResultID.ToString("D4");
            aDefectResult.ResultCode = m_InspectResult.ResultCode;

            string strBasePath = MainWindow.Setting.General.ResultPath;

            string strImageFolderPath = String.Format("{0}/{1}/{2}/{3}/Image/", strBasePath, TableDataCtrl.GroupName, TableDataCtrl.ModelName, TableDataCtrl.LotNo);
            aDefectResult.DefectImagePath = strImageFolderPath;
            aDefectResult.DefaultImagePath = aDefectResult.DefectImagePath;

            return aDefectResult;
        }

        public bool LotExist()
        {
            TableDataCtrl.LoadData();
            if (TableDataCtrl.InspectStrip > 0) return true;
            else return false;
        }

        public InspectResultDetail SetInspectResultDetail(ResultItem aResult, string strWorkType)
        {
            InspectResultDetail aInspectResultDetail = new InspectResultDetail();
            aInspectResultDetail.SectionCode = aResult.SectionID.ToString("D4");
            aInspectResultDetail.RoiCode = aResult.RoiID.ToString("D4");
            aInspectResultDetail.UnitRow = aResult.UnitPos.Y;
            aInspectResultDetail.UnitCol = aResult.UnitPos.X;
            aInspectResultDetail.WorkType = strWorkType;
            aInspectResultDetail.StripCol = 1;
            aInspectResultDetail.StripRow = 1;
            aInspectResultDetail.IsDefect = true;

            Debug.WriteLine("{0}, {1}, {2}, {3:d}", aInspectResultDetail.SectionCode, aInspectResultDetail.RoiCode, strWorkType, aResult.ResultID);
            return aInspectResultDetail;
        }

        public void SetInspectResultDetailList(ResultInfo aResultInfo, string strWorkType, int anStripID)
        {
            SetInspectResultDetailKeyList(aResultInfo, strWorkType);

            int resultCount = aResultInfo.ResultItemCount;
            for (int i = 0; i < resultCount; i++)
            {
                DefectResult rDefectResult = SetDefectResult(aResultInfo.Results[i], strWorkType, anStripID);
                foreach (InspectResultDetail rInspectResultDetail in m_InspectResult.listInspectResultDetail)
                {
                    if (rInspectResultDetail.WorkType == rDefectResult.WorkType &&
                        rInspectResultDetail.SectionCode == rDefectResult.SectionCode &&
                        rInspectResultDetail.RoiCode == rDefectResult.RoiCode)
                    {
                        rInspectResultDetail.listDefectResult.Add(rDefectResult);
                    }
                }
            }
        }

        public void SetInspectResultDetailKeyList(ResultInfo aResultInfo, string strWorkType)
        {
            // 중복 걸르기
            int resultCount = aResultInfo.ResultItemCount;
            for (int i = 0; i < resultCount; i++)
            {
                if (i == 0)
                {
                    m_InspectResult.listInspectResultDetail.Add(SetInspectResultDetail(aResultInfo.Results[i], strWorkType));
                    // Debug.WriteLine("reg Section ID : {0:d} Roi ID : {1:d} Work type : {2} ", aResultInfo.Results[i].SectionID, aResultInfo.Results[i].RoiID, strWorkType);
                }
                else
                {
                    bool bUnique = true;
                    for (int j = 0; j < m_InspectResult.listInspectResultDetail.Count; j++)
                    {
                        if (m_InspectResult.listInspectResultDetail[j].WorkType == strWorkType &&
                            Convert.ToInt32(m_InspectResult.listInspectResultDetail[j].SectionCode) == aResultInfo.Results[i].SectionID &&
                            Convert.ToInt32(m_InspectResult.listInspectResultDetail[j].RoiCode) == aResultInfo.Results[i].RoiID)
                        {
                            bUnique = false;
                            break;
                        }
                    }

                    if (bUnique)
                    {
                        m_InspectResult.listInspectResultDetail.Add(SetInspectResultDetail(aResultInfo.Results[i], strWorkType));
                    }
                }
            }
        }
        #endregion


        private void View2DMark(Bitmap origin, List<string> result)
        {
            Action action;

            Mat matOrigin = BitmapConverter.ToMat(origin);

            action = delegate { this.reader2D.SetImage(matOrigin.ToBitmap()); };
            this.Dispatcher.Invoke(action);

            if (result != null && result.Count > 0)
            {
                action = delegate { this.lbl2D.Content = result[0]; };
                this.Dispatcher.Invoke(action);
            }
            else
            {
                action = delegate { this.lbl2D.Content = "인 식 실 패"; };
                this.Dispatcher.Invoke(action);
            }
        }

        public void SetPOPButton(bool visible)
        {
            //ReportCtrl.ButtonEnable(visible);
        }

        public void Set_ICS_Def_Data(int UnitX, int UnitY, int PosX, int PosY, int AlignOffsetX, int AlignOffsetY, string ImageName, double DefSize, string SectionType, int DefectPosX, int DefectPosY, MainWindow.IndexInfo indexInfo, System.Windows.Point BP2Section_info, int CamOffsetX, int CamOffsetY, ref List<List<DefInfo>> defs)
        {
            if (indexInfo.CategorySurface == CategorySurface.BP && indexInfo.Slave && (SectionType == SectionTypeCode.UNIT_REGION || SectionType == SectionTypeCode.PSR_REGION))
            {
                DefectPosX = DefectPosX + Convert.ToInt32(BP2Section_info.X);
                DefectPosY = DefectPosY + Convert.ToInt32(BP2Section_info.Y);
            }
            DefInfo TempdefInfo = new DefInfo();
            TempdefInfo.UnitX = UnitX;
            TempdefInfo.UnitY = UnitY;
            ///////X,Y 좌표계 반대
            TempdefInfo.PosX = PosY;
            TempdefInfo.PosY = PosX;
            TempdefInfo.UnitOffsetX = AlignOffsetY;
            TempdefInfo.UnitOffsetY = AlignOffsetX;
            TempdefInfo.DefectPosX = DefectPosY + CamOffsetY;
            TempdefInfo.DefectPosY = DefectPosX + CamOffsetX;
            ///////
            TempdefInfo.DefSize = DefSize;
            TempdefInfo.DefZone = "";
            TempdefInfo.ImageFullPathName = ImageName;
            TempdefInfo.AVI_AI_Code = "";
            TempdefInfo.AVI_AI_Name = "";
            TempdefInfo.AVI_AI_Score = "";
            TempdefInfo.ICS_AI_Code = "";
            TempdefInfo.ICS_AI_Name = "";
            TempdefInfo.ICS_AI_Score = "";
            TempdefInfo.ICS_ImageFullPathName1 = "";
            TempdefInfo.ICS_ImageFullPathName2 = "";
            TempdefInfo.ICS_ImageFullPathName3 = "";
            TempdefInfo.IVS_Operator = "";
            TempdefInfo.IVS_Name = "";
            TempdefInfo.IVS_Code = "";            

            if (SectionType == SectionTypeCode.UNIT_REGION || SectionType == SectionTypeCode.PSR_REGION)
            {
                defs[(int)SectionArea.Unit].Add(TempdefInfo);
            }
            else if (SectionType == SectionTypeCode.OUTER_REGION)
            {
                defs[(int)SectionArea.Outer].Add(TempdefInfo);
            }
            else if (SectionType == SectionTypeCode.RAW_REGION)
            {
                defs[(int)SectionArea.Material].Add(TempdefInfo);
            }
            else
                return;
        }

        public bool Write_ICS_Data(object obj1)
        {
            object[] param = (object[])obj1;
            int Index = (int)param[0];
            string LotFolderPath = (string)param[1];
            Modelinformation ModelInfo = (Modelinformation)param[2];
            List<List<SectionInfo>> SectionInfos = (List<List<SectionInfo>>)param[3];
            List<List<DefInfo>> DefectInfos = (List<List<DefInfo>>)param[4];
            List<SectionInfo> UnitAlignOffset = (List<SectionInfo>)param[5];

            MainWindow.IndexInfo IndexInfo = MainWindow.Convert_ViewIndexToVisionIndexs(Index);
            //if (IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave) return false;
            string SIDE = "TOP";
            if (IndexInfo.CategorySurface == CategorySurface.BA) SIDE = "BOTTOM";

            string curID = Current_ID ?? "";
            string path = MainWindow.Setting.General.ResultPath;
            string root = Path.GetPathRoot(path);
            string rest = path.Substring(root.Length);
            string NewRoot = Path.Combine(@"\\" + MainWindow.Setting.General.MachineIP, rest.TrimStart('\\'));
            string strImageFolderPath = Path.Combine(
                NewRoot,
                TableDataCtrl.GroupName,
                TableDataCtrl.ModelName,
                TableDataCtrl.LotNo,
                "Image",
                curID
            );
         
            foreach (var item in DefectInfos)
            {
                foreach (var item2 in item)
                {
                    item2.ImageFullPathName = Regex.Replace(item2.ImageFullPathName, @"\[\d{4}\]", "[" + curID + "]");
                    item2.ImageFullPathName = Path.Combine(strImageFolderPath, item2.ImageFullPathName);
                }
            }                

            var Data = new ICS_Data
            {
                ModelName = TableDataCtrl.ModelName,
                LOT_NO = TableDataCtrl.LotNo,
                Side = SIDE,
                MachineInfo = new MachineInfo
                {
                    Name = MainWindow.Setting.General.MachineName,//MachinName,
                    ResX = MainWindow.Setting.SubSystem.IS.CamResolutionY[(int)IndexInfo.CategorySurface],//ResX,
                    ResY = MainWindow.Setting.SubSystem.IS.CamResolutionX[(int)IndexInfo.CategorySurface],//ResY,
                },
                ModelInfo = new Modelinformation(),
                AlignOffset = new List<SectionInfo>(),
                UnitInfo = new List<SectionInfo>(),
                OuterInfo = new List<SectionInfo>(),
                MaterialInfo = new List<SectionInfo>(),
                UnitDefInfo = new List<DefInfo>(),
                OuterDefInfo = new List<DefInfo>(),
                MaterialDefInfo = new List<DefInfo>(),
            };
            Data.ModelInfo = ModelInfo;
            Data.AlignOffset = UnitAlignOffset.ToList();
            Data.UnitInfo = SectionInfos[(int)SectionArea.Unit].ToList();
            Data.OuterInfo = SectionInfos[(int)SectionArea.Outer].ToList();
            Data.MaterialInfo = SectionInfos[(int)SectionArea.Material].ToList();
            Data.UnitDefInfo = DefectInfos[(int)SectionArea.Unit].ToList();
            Data.OuterDefInfo = DefectInfos[(int)SectionArea.Outer].ToList();
            Data.MaterialDefInfo = DefectInfos[(int)SectionArea.Material].ToList();

            string strStripFolderPath = String.Format(@"{0}/{1}/{2}/", LotFolderPath, curID, IndexInfo.CategorySurface.ToString());
            if (!Directory.Exists(strStripFolderPath)) Common.DirectoryManager.CreateDirectory(strStripFolderPath);

            var option = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(
                   UnicodeRanges.BasicLatin, // 일반 영숫자
                   UnicodeRanges.HangulCompatibilityJamo, // 이하 한글 관련 문자
                   UnicodeRanges.HangulJamo,
                   UnicodeRanges.HangulJamoExtendedA,
                   UnicodeRanges.HangulJamoExtendedB,
                   UnicodeRanges.HangulSyllables
                   ),
                WriteIndented = true
            };
            string json = System.Text.Json.JsonSerializer.Serialize(Data, option);
            File.WriteAllText(strStripFolderPath + IndexInfo.Surface + ".json", json);
            return true;
        }
        void VisionInterface_ReceiveUnitOffset(int ID, int grabside, List<System.Windows.Point> UnitOffset)
        {// 출력 Image 기준으로 XY를 뒤집는다
            if (0 <= ID && ID < VID.CA1)
            {
                MainWindow.IndexInfo IndexInfo = MainWindow.Convert_Vision(CategorySurface.BP, ID, grabside);
                if (IndexInfo.CategorySurface == CategorySurface.BP && IndexInfo.Slave == false)
                {
                    MainWindow.AlignOffset_ICS[IndexInfo.ScanIndex].Clear();
                    SectionInfo temp = new SectionInfo();
                    temp.UnitX = 0;
                    temp.UnitY = CurrentModel.Strip.UnitRow - 1;
                    temp.UnitPosX = Convert.ToInt32(UnitOffset[0].Y);
                    temp.UnitPosY = Convert.ToInt32(UnitOffset[0].X);
                    MainWindow.AlignOffset_ICS[IndexInfo.ScanIndex].Add(temp);
                    SectionInfo temp1 = new SectionInfo();
                    temp1.UnitX = CurrentModel.Strip.UnitColumn - 1;
                    temp1.UnitY = CurrentModel.Strip.UnitRow - 1;
                    temp1.UnitPosX = Convert.ToInt32(UnitOffset[1].Y);
                    temp1.UnitPosY = Convert.ToInt32(UnitOffset[1].X);
                    MainWindow.AlignOffset_ICS[IndexInfo.ScanIndex].Add(temp1);
                }
            }
            else if (ID >= VID.CA1 && ID < VID.BA1)
            {
                MainWindow.IndexInfo IndexInfo = MainWindow.Convert_Vision(CategorySurface.CA, ID, grabside);
                MainWindow.AlignOffset_ICS[IndexInfo.ScanIndex].Clear();
                SectionInfo temp = new SectionInfo();
                temp.UnitX = 0;
                temp.UnitY = CurrentModel.Strip.UnitRow - 1;
                temp.UnitPosX = Convert.ToInt32(UnitOffset[0].Y);
                temp.UnitPosY = Convert.ToInt32(UnitOffset[0].X);
                MainWindow.AlignOffset_ICS[IndexInfo.ScanIndex].Add(temp);
                SectionInfo temp1 = new SectionInfo();
                temp1.UnitX = CurrentModel.Strip.UnitColumn - 1;
                temp1.UnitY = CurrentModel.Strip.UnitRow - 1;
                temp1.UnitPosX = Convert.ToInt32(UnitOffset[1].Y);
                temp1.UnitPosY = Convert.ToInt32(UnitOffset[1].X);
                MainWindow.AlignOffset_ICS[IndexInfo.ScanIndex].Add(temp1);
            }
            else
            {
                MainWindow.IndexInfo IndexInfo = MainWindow.Convert_Vision(CategorySurface.BA, ID, grabside);
                MainWindow.AlignOffset_ICS[IndexInfo.ScanIndex].Clear();
                SectionInfo temp = new SectionInfo();
                temp.UnitX = 0;
                temp.UnitY = 0;
                temp.UnitPosX = Convert.ToInt32(UnitOffset[0].Y);
                temp.UnitPosY = Convert.ToInt32(UnitOffset[0].X);
                MainWindow.AlignOffset_ICS[IndexInfo.ScanIndex].Add(temp);
                SectionInfo temp1 = new SectionInfo();
                temp1.UnitX = CurrentModel.Strip.UnitColumn - 1;
                temp1.UnitY = 0;
                temp1.UnitPosX = Convert.ToInt32(UnitOffset[1].Y);
                temp1.UnitPosY = Convert.ToInt32(UnitOffset[1].X);
                MainWindow.AlignOffset_ICS[IndexInfo.ScanIndex].Add(temp1);
            }
        }
    }
}
