using Common;
using Common.Drawing.MarkingInformation;
using DCS.PLC;
using Marker;
using Marker.Class;
using PCS;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HDSInspector
{
    public class WeekClient
    {
        TcpClient Client;
        NetworkStream Stream;
        bool Connected = false;
        Thread ReadThread;
        ManualResetEvent connectDone = new ManualResetEvent(false);
        IAsyncResult AR;

        public void ConnectCallback(IAsyncResult ar)
        {
            connectDone.Set();
            TcpClient t = (TcpClient)ar.AsyncState;
            t.EndConnect(ar);
            AR = ar;
        }
        public WeekClient(string IP, int port, string order, out string week)
        {

            Client = new TcpClient(AddressFamily.InterNetwork);
            IPAddress ip = IPAddress.Parse(IP);
            try
            {
                connectDone.Reset();
                Client.BeginConnect(ip, port, new AsyncCallback(ConnectCallback), Client);
                connectDone.WaitOne();
                //Client.Connect(IP, port);
            }
            catch
            {
                Close();
                week = "";
                return;
            }
            Stream = Client.GetStream();
            Stream.ReadTimeout = 100;
            Connected = true;

            SendData(Convert.ToChar(0x02) + "^B100^" + order + "^VI91^" + Convert.ToChar(0x02));
            bool bReturn = false;
            int cnt = 0;
            Thread.Sleep(50);
            while (!bReturn)
            {
                Thread.Sleep(10);
                string rdata = ReadData();
                string[] str = rdata.Split('^');
                if (str.Length >= 5)
                {
                    Close();
                    week = str[4];
                    return;
                }

                if (cnt > 100)
                {
                    Close();
                    week = "";
                    return;
                }
                cnt++;
            }
            Close();
            week = "";
        }

        public string ReadData()
        {
            if (!Connected) return "";
            if (Stream.DataAvailable)
            {
                byte[] bytes = new byte[Client.ReceiveBufferSize];
                Stream.Read(bytes, 0, (int)Client.ReceiveBufferSize);
                string returndata = Encoding.UTF8.GetString(bytes);

                if (returndata.Length > 10)
                {
                    return returndata;
                }
                else return "";

            }
            return "";
        }

        //보내는거
        public void SendData(string str)
        {
            if (Stream.CanWrite)
            {
                Byte[] sendBytes = Encoding.UTF8.GetBytes(str);
                Stream.Write(sendBytes, 0, sendBytes.Length);
            }
        }

        //연결 종료 시
        public int Close()
        {
            Connected = false;
            if (ReadThread != null)
                ReadThread = null;

            if (Client != null)
            {
                Client.Client.Shutdown(SocketShutdown.Both);

                Client.Close();

            }
            if (Stream != null)
                Stream.Close();
            if (!Client.Connected)
            {

            }
            else return 0;
            return 1;
        }
    }


    /// <summary>
    /// LaserWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public delegate void ClientConnectHandler(bool bConnect);

    public partial class LaserWindow : Window
    {
        const string HOST_ID = "0001";
        const string LASER_ID = "0002";
        const double LASER_SCALE = 1;
        MainWindow pMain = null;
        PCS.ELF.AVI.ModelInformation Model;
        Thread ReadThread = null;
        Picolo64 picolo = null;
        Picolo64Ch pc = null;

        PylonCam pylonCam = null;

        EOTCPServer server = null;
        private readonly DeviceController PCSInstance;
        string ModelPath = "";
        double currBoatPos = 0;
        double currCamPos = 0;
        bool m_bHW = false;
        bool m_bAlignPos1 = false;
        bool m_bAlignPos2 = false;
        bool m_bDraw = false;
        public string week = "";
        public bool Connected = false;
        //private System.Timers.Timer timer;



        //public event ClientConnectHandler ClientConnect;

        private Point Boat2Offset = new Point(0, 0);
        private int CamType;
        private bool DualBoat;
        private double Resolution;
        private Point Scale;
        #region Align Value Window
        Point Offset1;
        Point Offset2;
        Point Offset;
        double Angle;
        #endregion

        #region Align Value Thread
        Point OffsetL1;
        Point OffsetL2;
        Point OffsetR1;
        Point OffsetR2;
        Point OffsetT1;
        Point OffsetT2;
        double Angle1;
        double Angle2;

        float MarginY;
        float MarginY1;
        float MarginY2;
        #endregion

        public LaserWindow(MainWindow main)
        {
            InitializeComponent();
            pMain = main;
            m_bHW = MainWindow.Setting.SubSystem.PLC.UsePLC;
            PCSInstance = pMain.PCSInstance;

            InitEvent();
            Init();
        }



        private void Init()
        {
            DualBoat = MainWindow.Setting.SubSystem.Laser.DualLaser;
            CamType = MainWindow.Setting.SubSystem.Laser.CamType;
            Resolution = MainWindow.Setting.SubSystem.Laser.CamResolution;

            if (CamType == 0) Scale = new Point(512.0 / 640.0, 384.0 / 480.0);
            else Scale = new Point(512.0 / 1280, 384.0 / 1024.0);
            if (m_bHW)
            {
                if (CamType == 0)
                {
                    picolo = new Picolo64();
                    picolo.Init();
                    pc = new Picolo64Ch("VID2");
                }
                else
                {
                    pylonCam = new PylonCam();
                }                
            }
            if (MainWindow.Setting.SubSystem.Laser.UseLaser)
            {
                server = new EOTCPServer(2001, m_bHW);
                this.server.connected += server_connected;
                if (m_bHW)
                {
                    if (!server.Connect(MainWindow.Setting.SubSystem.Laser.IP, Convert.ToInt32(MainWindow.Setting.SubSystem.Laser.Port)))
                    {
                        MessageBox.Show("Laser Mime 접속 실패. Mime 실행 상태를 확인 하세요.");
                    }
                }
            }
        }

        public void UnInit()
        {
            if (m_bHW)
            {
                if (CamType == 0 && picolo != null) picolo.UnInit();
                if (MainWindow.Setting.SubSystem.Laser.UseLaser)
                {
                    server.Close();
                }
            }
        }

        public void InitModelData()
        {
            viewer.SetMarkPos();
            viewer.InitMark(Connected);         
            if (m_bHW) LoadProject(viewer.markparam.filename);
            viewer.MarkingTypeCtrl.DisplayChange();
            //SetMarkPos();
        }

        public void InitModel(bool bAuto, Image img1, Image img2)
        {
            ModelPath = MainWindow.Setting.General.ModelPath + "\\" + MainWindow.CurrentGroup.Name + "\\" + MainWindow.CurrentModel.Name + "\\";
            lblAlignPos1.Content = MainWindow.CurrentModel.Marker.BoatPos1.ToString("0000.000");
            lblAlignPos2.Content = MainWindow.CurrentModel.Marker.BoatPos2.ToString("0000.000");
            lblAlignPos.Content = MainWindow.CurrentModel.Marker.CamPosY.ToString("00.000");
            txtAlignMatch.Text = MainWindow.CurrentModel.Marker.MatchRate.ToString();
            viewer.map.SetUnitNum(MainWindow.CurrentModel.Strip.UnitRow, MainWindow.CurrentModel.Strip.UnitColumn);
            Model = MainWindow.CurrentModel;
            if (DualBoat) ShowBoat2Offset();

            Canvas.SetLeft(rctAlign1, MainWindow.CurrentModel.Marker.PosX1 * Scale.X - 16);
            Canvas.SetTop(rctAlign1, MainWindow.CurrentModel.Marker.PosY1 * Scale.Y - 16);
            Canvas.SetLeft(rctAlign2, MainWindow.CurrentModel.Marker.PosX2 * Scale.X - 16);
            Canvas.SetTop(rctAlign2, MainWindow.CurrentModel.Marker.PosY2 * Scale.Y - 16);

            if (m_bHW)
            {
                if (!server.Retry())
                {
                    MessageBox.Show("Laser Mime 접속 실패. Mime 실행 상태를 확인 하세요.");
                }
                if (bAuto)
                {
                    if (CamType == 0) pc.SetAuto(bAuto, img1, img2);
                    else pylonCam.SetAuto(bAuto, img1, img2);
                }
                else
                {
                    if (CamType == 0) pc.bAuto = false;
                    else pylonCam.bAuto = false;
                }
                if (CamType == 0)
                    pc.Match = MainWindow.CurrentModel.Marker.MatchRate;
                else pylonCam.Match = MainWindow.CurrentModel.Marker.MatchRate;
                if (File.Exists(ModelPath + "align1.dat"))
                {
                    if (CamType == 0)
                    {
                        pc.ImageToAlign1(ModelPath + "align1.dat", MainWindow.CurrentModel.Marker.PosX1, MainWindow.CurrentModel.Marker.PosY1);
                        imgAlignRight.Source = pc.AlignBmp1;
                    }
                    else
                    {
                        pylonCam.ImageToAlign1(ModelPath + "align1.dat", MainWindow.CurrentModel.Marker.PosX1, MainWindow.CurrentModel.Marker.PosY1);
                        imgAlignRight.Source = pylonCam.AlignBmp1;
                    }
                }
                if (File.Exists(ModelPath + "align2.dat"))
                {
                    if (CamType == 0)
                    {
                        pc.ImageToAlign2(ModelPath + "align2.dat", MainWindow.CurrentModel.Marker.PosX2, MainWindow.CurrentModel.Marker.PosY2);
                        imgAlignLeft.Source = pc.AlignBmp2;
                    }
                    else
                    {
                        pylonCam.ImageToAlign2(ModelPath + "align2.dat", MainWindow.CurrentModel.Marker.PosX2, MainWindow.CurrentModel.Marker.PosY2);
                        imgAlignLeft.Source = pylonCam.AlignBmp2;
                    }
                }
                if (CamType == 0)
                {
                    pc.SetImage(imgLive); // 수정 20221103
                    pc.Grab(0);
                }
                else
                    pylonCam.Grab();
                Thread.Sleep(100);
            }
            btnROI.Visibility = Visibility.Hidden;
        }

        private void InitEvent()
        {
            this.btnClose.Click += btnClose_Click;
            this.IsVisibleChanged += LaserWindow_IsVisibleChanged;
            this.btnSave.Click += btnSave_Click;
            this.btnAlign.Click += btnAlign_Click;
            this.btnInspectAlignLeft.Click += btnInspectAlignLeft_Click;
            this.btnInspectAlignRight.Click += btnInspectAlignRight_Click;
            this.btnROI.Click += btnROI_Click;
            this.LiveCanvas.MouseMove += LiveCanvas_MouseMove;
            this.LiveCanvas.MouseUp += LiveCanvas_MouseUp;
            this.btnMarking.Click += btnMarking_Click;
            this.btnSaveMatch.Click += btnSaveMatch_Click;
            cbPos.SelectionChanged += cbPos_SelectionChanged;
            this.btnRegistRef.Click += btnRegistRef_Click;
            this.btnSaveDefaultPos.Click += btnSaveDefaultPos_Click;
            this.viewer.btnBoat2Offet.Click += btnBoat2Offet_Click;
            this.btnSave.Click += btnSaveDefaultPos_Click;
        }

        void ShowBoat2Offset()
        {
            viewer.txtBoat2OffsetX.Text = MainWindow.Setting.SubSystem.Laser.Boat2OffsetX.ToString();
            viewer.txtBoat2OffsetY.Text = MainWindow.Setting.SubSystem.Laser.Boat2OffsetY.ToString();
            viewer.txtBoat2Angle.Text = MainWindow.Setting.SubSystem.Laser.Boat2Angle.ToString();
        }

        void btnBoat2Offet_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("레이저 보트2의 Offset를 변경 할 경우 보트간 위치 차이가 발생 될 수 있습니다. 그래도 변경 하시겠습니까?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                MainWindow.Setting.SubSystem.Laser.Boat2OffsetX = Convert.ToDouble(viewer.txtBoat2OffsetX.Text);
                MainWindow.Setting.SubSystem.Laser.Boat2OffsetY = Convert.ToDouble(viewer.txtBoat2OffsetY.Text);
                MainWindow.Setting.SubSystem.Laser.Boat2Angle = Convert.ToDouble(viewer.txtBoat2Angle.Text);
                MainWindow.Setting.SubSystem.Save();
            }
            else
            {
                ShowBoat2Offset();
            }
        }

        void btnSaveDefaultPos_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Setting.SubSystem.Laser.BoatAlignPos1 = Convert.ToDouble(lblAlignPos1.Content);
            MainWindow.Setting.SubSystem.Laser.BoatAlignPos2 = Convert.ToDouble(lblAlignPos2.Content);
            MainWindow.Setting.SubSystem.Laser.CamPosY = Convert.ToDouble(lblAlignPos.Content);
            MainWindow.Setting.SubSystem.Save();
        }

        void btnRegistRef_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("검사 설정의 최신 이미지로 영상을 변경 하시겠습니까?", "경고", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                viewer.ChangeRefImage();

        }

        void btnAlign_Click(object sender, RoutedEventArgs e)
        {
            if (PCSInstance.PlcDevice == null) return;
            if (!PCSInstance.PlcDevice.RequestAlign(0) && !PCSInstance.PlcDevice.RequestAlign(1))
            {
                MessageBox.Show("레이저 요구가 없습니다.");
                return;
            }
            if (PCSInstance.PlcDevice.ReadLaserBoat() != 1)
            {
                MessageBox.Show("우측 얼라인 위치가 아닙니다.");
                return;
            }
            this.Cursor = Cursors.Wait;

            if (CamType == 0)
            {
                float x, y;
                int match;
                int n = pc.SearchAlign1(out x, out y, out match);
                MarginY = y;
                Offset2.X = (double)(x) * Resolution / 1000.0;
                Offset2.Y = (double)(y) * Resolution / 1000.0;
                lblOffsetXRight.Content = x.ToString();
                lblOffsetYRight.Content = y.ToString();
                lblMatchRight.Content = match.ToString();
                if (n != 0)
                {
                    this.Cursor = Cursors.Arrow;
                    MessageBox.Show("얼라인에 실패 했습니다.");
                    return;
                }
                pc.LiveStop();
                if (PCSInstance.PlcDevice.RequestAlign(0)) PCSInstance.PlcDevice.PassAlign(0);
                if (MainWindow.Setting.SubSystem.Laser.DualLaser)
                    if (PCSInstance.PlcDevice.RequestAlign(1)) PCSInstance.PlcDevice.PassAlign(1);
                Thread.Sleep(100);
                int cnt = 0;
                while (true)
                {
                    if (PCSInstance.PlcDevice.RequestAlign(0)) break;
                    if (MainWindow.Setting.SubSystem.Laser.DualLaser)
                        if (PCSInstance.PlcDevice.RequestAlign(1)) break;
                    cnt++;
                    Thread.Sleep(200);
                    if (cnt > 50)
                    {
                        this.Cursor = Cursors.Arrow;
                        MessageBox.Show("얼라인 위치 이동 실패");
                        return;
                    }
                }
                Thread.Sleep(100);
                for (int i = 0; i < 5; i++)
                {
                    pc.Grab(1);
                    int cnt1 = 0;
                    Thread.Sleep(30);
                    while (pc.nGrabDone == 0)
                    {
                        Thread.Sleep(30);
                        cnt1++;
                        if (cnt > 5) break;
                    }
                    if (pc.nGrabDone == 1 && i >= 1)
                    {
                        break;
                    }
                }
                n = pc.SearchAlign2(out x, out y, out match);
                pc.LiveStart();
                Offset1.X = (double)(x) * Resolution / 1000.0;
                Offset1.Y = (double)(y) * Resolution / 1000.0;
                lblOffsetXLeft.Content = x.ToString();
                lblOffsetYLeft.Content = y.ToString();
                lblMatchLeft.Content = match.ToString();

                if (n != 0)
                {
                    this.Cursor = Cursors.Arrow;
                    MessageBox.Show("얼라인에 실패 했습니다.");
                    return;
                }

                Offset.X = Offset1.X;
                double osx = (Offset2.X - Offset1.X) + Math.Abs(MainWindow.CurrentModel.Marker.BoatPos1 - MainWindow.CurrentModel.Marker.BoatPos2);
                double osy = ((MainWindow.CurrentModel.Marker.PosY1 + MarginY) - (MainWindow.CurrentModel.Marker.PosY2 + y)) * Resolution / 1000.0;

                double tan = 0;
                if (osx != 0)
                {
                    tan = osy / osx;
                }
                Angle = Math.Atan(tan) * (180.0 / Math.PI);
                Offset.Y = Offset1.Y;
                lblAngle.Content = Angle.ToString("0.0000");
                if (PCSInstance.PlcDevice.RequestAlign(0)) PCSInstance.PlcDevice.PassAlign(0);
                if (MainWindow.Setting.SubSystem.Laser.DualLaser)
                    if (PCSInstance.PlcDevice.RequestAlign(1)) PCSInstance.PlcDevice.PassAlign(1);
            }
            else
            {
                int cnt = 0;
                align_right();
                while (true)
                {
                    if (PCSInstance.PlcDevice.RequestAlign(0)) break;
                    if (MainWindow.Setting.SubSystem.Laser.DualLaser)
                        if (PCSInstance.PlcDevice.RequestAlign(1)) break;
                    cnt++;
                    Thread.Sleep(200);
                    if (cnt > 50)
                    {
                        this.Cursor = Cursors.Arrow;
                        MessageBox.Show("얼라인 위치 이동 실패");
                        return;
                    }
                }

                int cnt1 = 0;
                while (true)
                {
                    if (pylonCam.nGrabDone == 1)
                    {
                        this.Dispatcher.Invoke((ThreadStart)(() => { }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                        Thread.Sleep(5);
                        cnt1++;
                        if (cnt1 > 60)
                            break;
                    }
                }
                align_left();
            }

            this.Cursor = Cursors.Arrow;
        }

        void btnSaveMatch_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.CurrentModel.Marker.MatchRate = Convert.ToInt32(txtAlignMatch.Text);
            PCS.ELF.AVI.ModelManager.UpdateModelMarkInfo(MainWindow.CurrentModel);
        }

        void cbPos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPos.SelectedIndex == 0)
            {
                m_bAlignPos1 = true;
                m_bAlignPos2 = false;
            }
            else
            {
                m_bAlignPos1 = false;
                m_bAlignPos2 = true;
            }
        }

        void LaserWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                if (MainWindow.Setting.SubSystem.Laser.DualLaser)
                    viewer.grdBoat2Offset.Visibility = Visibility.Visible;
                else
                    viewer.grdBoat2Offset.Visibility = Visibility.Hidden;
                if (m_bHW)
                {
                    if (CamType == 0)
                    {
                        pc.SetImage(imgLive);
                        pc.LiveStart();
                    }
                    else
                    {
                        pylonCam.SetImage(imgLive);
                        pylonCam.LiveStart(true);
                    }
                    ReadThread = new Thread(ReadPosition);
                    ReadThread.Start();
                    m_bAlignPos1 = true;
                    m_bAlignPos2 = false;
                    cbPos.SelectedIndex = 0;
                    btnROI.Visibility = Visibility.Visible;
                }
            }
        }

        void btnMarking_Click(object sender, RoutedEventArgs e)
        {
            if(m_bHW)
            {
                if (!Connected)
                {
                    MessageBox.Show("레이저 연결 상태를 확인 하세요.");
                    return;
                }
            }          

            MarkingGenaral();

        }

        void MarkingGenaral()
        {
            int id = 0;
            if (PCSInstance.PlcDevice != null && PCSInstance.PlcDevice.RequestLaser(0))
                id = 1;
            if (PCSInstance.PlcDevice != null && PCSInstance.PlcDevice.RequestLaser(1))
                id = 2; 
            if (m_bHW)
            {
                if (!Connected || id == 0)
                {
                    MessageBox.Show("레이저 마킹 가능 상태가 아닙니다.");
                    return;
                }
            }
            else id = 1;

            this.Cursor = Cursors.Wait;
            int BadCount = 0;
            int tCount = viewer.map.GetBadCount();
            int n = 0;
            if (MainWindow.CurrentModel.Marker.WeekMark > 0)
                week = txtWeek.Text;
            else
                week = "";
            double AddAngle = 0;
            string barcode = "H19A1234000";
            if (MainWindow.CurrentModel.Marker.IDMark == 0) barcode = "";
            if (id == 1)
            {
                AddAngle = MainWindow.CurrentModel.Marker.LaserAngle;
            }
            else if (id == 2)
            {
                Angle += MainWindow.Setting.SubSystem.Laser.Boat2Angle;
                AddAngle = MainWindow.CurrentModel.Marker.LaserAngle;
            }
            int nNo = 0;
            if (MainWindow.CurrentModel.Marker.NumMark > 0)
            {
                nNo = MainWindow.CurrentModel.Marker.NumLeft ? 1 : 2;
            }
            int nStep = MainWindow.CurrentModel.Strip.MarkStep;
            for (int i = 0; i < nStep; i++)
            {             
                if (PCSInstance.PlcDevice != null)
                {
                    bool b = false;
                    while (!b)
                    {
                        if (id == 1 && PCSInstance.PlcDevice.RequestLaser(0)) break;
                        if (id == 2 && PCSInstance.PlcDevice.RequestLaser(1)) break;
                        Thread.Sleep(50);
                        n++;
                        if (i > 100)
                        {
                            this.Cursor = Cursors.Arrow;
                            MessageBox.Show("레이저 마킹 가능 상태가 아닙니다.");
                            return;
                        }
                    }
                }
                Thread.Sleep(100);
                string s = viewer.map.GetLaserString(out BadCount, (nStep - 1) - i, MainWindow.CurrentModel.Strip.StepUnits, MainWindow.CurrentModel.Strip.UnitColumn, MainWindow.CurrentModel.Strip.MarkStep);
       
                if (IsPassLaser(i, BadCount, nStep, tCount))
                {
                    if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                    if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                    continue;
                }
                #region 이전 코드
                /*
                if (m_bHW)
                {
                    if (MainWindow.CurrentModel.Marker.IDMark > 0)
                    {
                        if (MainWindow.CurrentModel.Marker.IDMark == 1)
                        {
                            if (i != nStep - 1)
                            {
                                if ((BadCount == 0))
                                {
                                    if (i != nStep - 1 && i != 0)//////센터
                                    {
                                        if (MainWindow.CurrentModel.Marker.WeekPos != 2)
                                        {
                                            if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                            if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                        if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                        continue;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (i != 0)
                            {
                                if ((BadCount == 0))
                                {
                                    if (i != nStep - 1 && i != 0)//////센터
                                    {
                                        if (MainWindow.CurrentModel.Marker.WeekPos != 2)
                                        {
                                            if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                            if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                        if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                        continue;
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        if (MainWindow.CurrentModel.Marker.NumMark > 0)
                        {
                            if (MainWindow.CurrentModel.Marker.NumLeft)
                            {
                                if (MainWindow.CurrentModel.Marker.WeekMark > 0)
                                {
                                    if (i != nStep - 1 && i != 0)///////// 가운데 아무것도 없으면 패스
                                    {
                                        if (BadCount == 0)
                                        {
                                            if (MainWindow.CurrentModel.Marker.WeekPos != 2)
                                            {
                                                if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                                if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                                continue;
                                            }

                                        }
                                    }
                                    if (i == nStep - 1)///////////마지막 스탭에서 특정 조건이면 패스
                                    {
                                        if (BadCount == 0)
                                        {
                                            if (MainWindow.CurrentModel.Marker.WeekPos != 0)
                                            {
                                                if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                                if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                                continue;
                                            }
                                        }
                                    }
                                    if (i == 0)///////////첫번째 스탭에서 특정 조건이면 패스
                                    {
                                        if (BadCount == 0)
                                        {
                                            if (MainWindow.CurrentModel.Marker.WeekPos != 1)
                                            {
                                                if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                                if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                                continue;
                                            }
                                        }
                                    }
                                    //if ((i == nStep - 1) && !MainWindow.CurrentModel.Marker.ZeroMark && tCount == 0)///////////마지막 스탭에서 특정 조건이면 패스
                                    //{
                                    //    if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                    //    if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                    //    continue;
                                    //}                           
                                }
                                else
                                {
                                    if ((BadCount == 0) && (i < (nStep - 1)))
                                    {
                                        if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                        if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                        continue;
                                    }
                                    if (i == (nStep - 1) && (tCount == 0) && !MainWindow.CurrentModel.Marker.ZeroMark)
                                    {
                                        if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                        if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                if (MainWindow.CurrentModel.Marker.WeekMark > 0)
                                {
                                    if (i != nStep - 1 && i != 0)/////센터
                                    {
                                        if (BadCount == 0)
                                        {
                                            if (MainWindow.CurrentModel.Marker.WeekPos != 2)
                                            {
                                                if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                                if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                                continue;
                                            }

                                        }
                                    }
                                    if (i == nStep - 1)///////////마지막 스탭에서 특정 조건이면 패스
                                    {
                                        if (BadCount == 0)
                                        {
                                            if (MainWindow.CurrentModel.Marker.WeekPos != 0)
                                            {
                                                if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                                if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                                continue;
                                            }
                                        }
                                    }
                                    if (i == 0)///////////첫번째 스탭에서 특정 조건이면 패스
                                    {
                                        if (BadCount == 0)
                                        {
                                            if (MainWindow.CurrentModel.Marker.WeekPos != 1)
                                            {
                                                if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                                if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                                continue;
                                            }
                                        }
                                    }
                                    //if ((i == 0) && !MainWindow.CurrentModel.Marker.ZeroMark && tCount == 0)
                                    //{
                                    //    if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                    //    if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                    //    continue;
                                    //}
                                }
                                else
                                {
                                    if ((BadCount == 0) && (i > 0))
                                    {
                                        if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                        if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                        continue;
                                    }
                                    if (i == 0 && (tCount == 0) && !MainWindow.CurrentModel.Marker.ZeroMark)
                                    {
                                        if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                        if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                        continue;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.WeekMark > 0)
                            {
                                if (MainWindow.CurrentModel.Marker.WeekPos == 0)////// 왼쪽
                                {
                                    if (i != nStep - 1)
                                    {
                                        if ((BadCount == 0))
                                        {
                                            if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                            if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                            continue;
                                        }
                                    }
                                }
                                else if (MainWindow.CurrentModel.Marker.WeekPos == 1)////// 오른쪽
                                {
                                    if (i != 0)
                                    {
                                        if ((BadCount == 0))
                                        {
                                            if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                            if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                            continue;
                                        }
                                    }
                                }
                                else////// 센터
                                {
                                    if (!(i != nStep - 1 && i != 0))///////// 가운데가 아닐때
                                    {
                                        if (BadCount == 0)
                                        {
                                            if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                            if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                            continue;
                                        }
                                    }
                                }
                            }
                            //else if (MainWindow.CurrentModel.Marker.IDMark > 0)
                            //{
                            //    if (MainWindow.CurrentModel.Marker.IDMark == 1)
                            //    {
                            //        if (i != nStep - 1)
                            //        {
                            //            if ((BadCount == 0))
                            //            {
                            //                if (id == 1) PCSInstance.PlcDevice.PassLaser(0);
                            //                if (id == 2) PCSInstance.PlcDevice.PassLaser(1);
                            //                continue;
                            //            }
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (i != 0)
                            //        {
                            //            if ((BadCount == 0))
                            //            {
                            //                if (id == 1) PCSInstance.PlcDevice.PassLaser(0);
                            //                if (id == 2) PCSInstance.PlcDevice.PassLaser(1);
                            //                continue;
                            //            }
                            //        }
                            //    }
                            //}
                            else
                            {
                                if (BadCount == 0)
                                {
                                    if (id == 1 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(0);
                                    if (id == 2 && PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(1);
                                    continue;
                                }
                            }
                        }
                    }
                }*/
                 #endregion 이전 코드

                Point tmpPos = new Point(0, 0);
                if (i == (nStep - 1))
                {
                    tmpPos = Offset;
                }
                else
                {
                    tmpPos = GetPositionManual((nStep - 1) - i, id);
                    tmpPos.X += Offset.X;
                    tmpPos.Y += Offset.Y;
                }
                if (id == 2)
                {
                    tmpPos.X += MainWindow.Setting.SubSystem.Laser.Boat2OffsetX;
                    tmpPos.Y += MainWindow.Setting.SubSystem.Laser.Boat2OffsetY;
                }

                string file = "";
                if (MainWindow.CurrentModel.Marker.RailIrr)
                {
                    file = MainWindow.CurrentModel.Name + "_" + (MainWindow.CurrentModel.Strip.MarkStep - i).ToString() + "STEP.mrk";

                    if (!LoadProject(file))
                    {
                        this.Cursor = Cursors.Arrow;
                        MessageBox.Show("마크 파일 로딩 실패");
                        return;
                    }
                    Thread.Sleep(300);
                }

                int nMark = 0;
                if (MainWindow.CurrentModel.Marker.UnitMark > 0 && MainWindow.CurrentModel.Marker.RailMark > 0)  nMark = 0;
                if (MainWindow.CurrentModel.Marker.UnitMark == 0 && MainWindow.CurrentModel.Marker.RailMark > 0) nMark = 1;
                if (MainWindow.CurrentModel.Marker.UnitMark > 0 && MainWindow.CurrentModel.Marker.RailMark == 0) nMark = 2;
                //if (MainWindow.CurrentModel.Marker.WeekPos == 2 && (i != nStep - 1 && i != 0)) nMark = 3;/////센터마킹만 템플릿을 예외로 한다//패치후 불필요

                if (MainWindow.CurrentModel.Marker.RailMark == 2)/////역방향 마킹
                {
                    if (MainWindow.CurrentModel.Marker.UnitMark > 0)
                    {
                        int Mode = 2;
                        MainWindow.Log("LASER", SeverityLevel.DEBUG, "marking", true);
                        if (0 != server.Marking(s, tCount, barcode, tmpPos.X, tmpPos.Y, Angle, MainWindow.CurrentModel.InspectMode, Mode, 0, 0, week, AddAngle,
                            MainWindow.CurrentModel.Strip.MarkStep, false, MainWindow.CurrentModel.Marker.RailIrr, MainWindow.CurrentModel.Marker.IDMark, 
                            MainWindow.CurrentModel.Marker.ZeroMark, MainWindow.CurrentModel.Marker.WeekPos))
                        {
                            this.Cursor = Cursors.Arrow;
                            MainWindow.Log("LASER", SeverityLevel.DEBUG, "Genaral marking fail UnitMark 1", true);
                            MessageBox.Show("마킹 실패");
                            return;
                        }
                        Thread.Sleep(500);
                        s = viewer.map.GetLaserStringRev(out BadCount, (nStep - 1) - i, MainWindow.CurrentModel.Strip.StepUnits, MainWindow.CurrentModel.Strip.UnitColumn, MainWindow.CurrentModel.Strip.MarkStep);
                        MainWindow.Log("LASER", SeverityLevel.DEBUG, "marking", true);

                        Mode = 1;
                        if (0 != server.Marking(s, tCount, "", tmpPos.X, tmpPos.Y, Angle, MainWindow.CurrentModel.InspectMode, Mode, nNo, (nStep - 1) - i, week, AddAngle, 
                            MainWindow.CurrentModel.Strip.MarkStep, false, MainWindow.CurrentModel.Marker.RailIrr, MainWindow.CurrentModel.Marker.IDMark, 
                            MainWindow.CurrentModel.Marker.ZeroMark, MainWindow.CurrentModel.Marker.WeekPos))
                        {
                            this.Cursor = Cursors.Arrow;
                            MainWindow.Log("LASER", SeverityLevel.DEBUG, "Genaral marking fail UnitMark 2", true);
                            MessageBox.Show("마킹 실패");
                            return;
                        }

                    }
                    else
                    {
                        s = viewer.map.GetLaserStringRev(out BadCount, (nStep - 1) - i, MainWindow.CurrentModel.Strip.StepUnits, MainWindow.CurrentModel.Strip.UnitColumn, MainWindow.CurrentModel.Strip.MarkStep);
                        MainWindow.Log("LASER", SeverityLevel.DEBUG, "marking", true);
                        if (0 != server.Marking(s, tCount, barcode, tmpPos.X, tmpPos.Y, Angle, MainWindow.CurrentModel.InspectMode, nMark, nNo, (nStep - 1) - i, week, AddAngle, 
                            MainWindow.CurrentModel.Strip.MarkStep, false, MainWindow.CurrentModel.Marker.RailIrr, MainWindow.CurrentModel.Marker.IDMark, 
                            MainWindow.CurrentModel.Marker.ZeroMark, MainWindow.CurrentModel.Marker.WeekPos))
                        {
                            this.Cursor = Cursors.Arrow;
                            MainWindow.Log("LASER", SeverityLevel.DEBUG, "Genaral marking fail UnitMark <= 0", true);
                            MessageBox.Show("마킹 실패");
                            return;
                        }
                    }
                }
                else
                {
                    MainWindow.Log("LASER", SeverityLevel.DEBUG, "marking", true);
                    if (0 != server.Marking(s, tCount, barcode, tmpPos.X, tmpPos.Y, Angle, MainWindow.CurrentModel.InspectMode, nMark, nNo, (nStep - 1) - i, week, AddAngle, 
                        MainWindow.CurrentModel.Strip.MarkStep, false, MainWindow.CurrentModel.Marker.RailIrr, MainWindow.CurrentModel.Marker.IDMark, 
                        MainWindow.CurrentModel.Marker.ZeroMark, MainWindow.CurrentModel.Marker.WeekPos))
                    {
                        this.Cursor = Cursors.Arrow;
                        MainWindow.Log("LASER", SeverityLevel.DEBUG, "Genaral marking fail RailMark != 2", true);
                        MessageBox.Show("마킹 실패");
                        return;
                    }
                }

                if (id == 1 && PCSInstance.PlcDevice != null)
                    PCSInstance.PlcDevice.PassLaser(0);
                if (id == 2 && PCSInstance.PlcDevice != null)
                    PCSInstance.PlcDevice.PassLaser(1);
            }
            this.Cursor = Cursors.Arrow;
        }

        void LiveCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (m_bDraw)
            {
                m_bDraw = false;
                Point pos = e.GetPosition(this.LiveCanvas);

                //Align 1
                if (m_bAlignPos1)
                {
                    Canvas.SetLeft(rctAlign1, pos.X - 16);
                    Canvas.SetTop(rctAlign1, pos.Y - 16);
                    MainWindow.CurrentModel.Marker.PosX1 = (int)(pos.X / Scale.X);
                    MainWindow.CurrentModel.Marker.PosY1 = (int)(pos.Y / Scale.Y);

                    if (CamType == 0)
                    {
                        pc.resultToAlign1(ModelPath + "align1.dat", new Point(MainWindow.CurrentModel.Marker.PosX1, MainWindow.CurrentModel.Marker.PosY1));
                        imgAlignRight.Source = pc.AlignBmp1;
                    }
                    else
                    {
                        pylonCam.resultToAlign1(ModelPath + "align1.dat", new Point(MainWindow.CurrentModel.Marker.PosX1, MainWindow.CurrentModel.Marker.PosY1));
                        imgAlignRight.Source = pylonCam.AlignBmp1;
                    }

                    MainWindow.CurrentModel.Marker.BoatPos1 = currBoatPos;
                    MainWindow.CurrentModel.Marker.CamPosY = currCamPos;

                    if (MainWindow.CurrentModel.Marker.BoatPos2 < 500)
                        MainWindow.CurrentModel.Marker.BoatPos2 = MainWindow.CurrentModel.Marker.BoatPos1 + MainWindow.CurrentModel.Strip.Width - 10;

                    PCS.ELF.AVI.ModelManager.UpdateModelMarkInfo(MainWindow.CurrentModel);
                    lblAlignPos1.Content = MainWindow.CurrentModel.Marker.BoatPos1.ToString("0000.000");
                    lblAlignPos.Content = MainWindow.CurrentModel.Marker.CamPosY.ToString("00.000");
                    int val = Convert.ToInt32(Math.Floor(MainWindow.CurrentModel.Marker.BoatPos1));
                    PCSInstance.PlcDevice.WriteData(PCSInstance.Address.ALIGN_XPOS11, val);
                    Thread.Sleep(100);
                    val = Convert.ToInt32(MainWindow.CurrentModel.Marker.BoatPos1 * 1000.0) % 1000;
                    PCSInstance.PlcDevice.WriteData(PCSInstance.Address.ALIGN_XPOS12, val);
                    Thread.Sleep(100);
                    val = Convert.ToInt32(Math.Floor(MainWindow.CurrentModel.Marker.BoatPos2));
                    PCSInstance.PlcDevice.WriteData(PCSInstance.Address.ALIGN_XPOS21, val);
                    Thread.Sleep(100);
                    val = Convert.ToInt32(MainWindow.CurrentModel.Marker.BoatPos2 * 1000.0) % 1000;
                    PCSInstance.PlcDevice.WriteData(PCSInstance.Address.ALIGN_XPOS22, val);
                    Thread.Sleep(100);

                    if (MainWindow.Setting.SubSystem.PLC.MCType == 1) val = Convert.ToInt32(MainWindow.CurrentModel.Marker.CamPosY * 1000.0);
                    else val = Convert.ToInt32(MainWindow.CurrentModel.Marker.CamPosY);

                    PCSInstance.PlcDevice.WriteData(PCSInstance.Address.ALIGN_YPOS, val);
                    Thread.Sleep(100);
                    PCSInstance.PlcDevice.WriteData(PCSInstance.Address.WRITE_LASER, 1);
                    Thread.Sleep(100);
                    PCSInstance.PlcDevice.WriteData(PCSInstance.Address.WRITE_LASER, 0);
                }

                //Align 2
                if (m_bAlignPos2)
                {
                    Canvas.SetLeft(rctAlign2, pos.X - 16);
                    Canvas.SetTop(rctAlign2, pos.Y - 16);
                    MainWindow.CurrentModel.Marker.PosX2 = (int)(pos.X / Scale.X);
                    MainWindow.CurrentModel.Marker.PosY2 = (int)(pos.Y / Scale.Y);

                    if (CamType == 0)
                    {
                        pc.resultToAlign2(ModelPath + "align2.dat", new Point(MainWindow.CurrentModel.Marker.PosX2, MainWindow.CurrentModel.Marker.PosY2));
                        imgAlignLeft.Source = pc.AlignBmp2;
                    }
                    else
                    {
                        pylonCam.resultToAlign2(ModelPath + "align2.dat", new Point(MainWindow.CurrentModel.Marker.PosX2, MainWindow.CurrentModel.Marker.PosY2));
                        imgAlignLeft.Source = pylonCam.AlignBmp2;
                    }

                    MainWindow.CurrentModel.Marker.BoatPos2 = currBoatPos;
                    if (MainWindow.CurrentModel.Marker.CamPosY != currCamPos)
                    {
                        MessageBox.Show("레이저 Align 카메라 위치는 변경할 수 없습니다.\n처음부터 다시 시작해 주세요.");
                        return;
                    }
                    PCS.ELF.AVI.ModelManager.UpdateModelMarkInfo(MainWindow.CurrentModel);
                    lblAlignPos2.Content = MainWindow.CurrentModel.Marker.BoatPos2.ToString("0000.000");
                    int val = Convert.ToInt32(Math.Floor(MainWindow.CurrentModel.Marker.BoatPos1));
                    PCSInstance.PlcDevice.WriteData(PCSInstance.Address.ALIGN_XPOS11, val);
                    val = Convert.ToInt32(MainWindow.CurrentModel.Marker.BoatPos1 * 1000.0) % 1000;
                    PCSInstance.PlcDevice.WriteData(PCSInstance.Address.ALIGN_XPOS12, val);
                    val = Convert.ToInt32(Math.Floor(MainWindow.CurrentModel.Marker.BoatPos2));
                    PCSInstance.PlcDevice.WriteData(PCSInstance.Address.ALIGN_XPOS21, val);
                    val = Convert.ToInt32(MainWindow.CurrentModel.Marker.BoatPos2 * 1000.0) % 1000;
                    PCSInstance.PlcDevice.WriteData(PCSInstance.Address.ALIGN_XPOS22, val);

                    //if (MainWindow.Setting.SubSystem.PLC.MCType == 1) val = Convert.ToInt32(MainWindow.CurrentModel.Marker.CamPosY * 1000.0);
                    //else val = Convert.ToInt32(MainWindow.CurrentModel.Marker.CamPosY);

                    //PCSInstance.PlcDevice.WriteData(PCSInstance.Address.ALIGN_YPOS, val);
                    //Thread.Sleep(100);
                    PCSInstance.PlcDevice.WriteData(PCSInstance.Address.WRITE_LASER, 1);
                    Thread.Sleep(100);
                    PCSInstance.PlcDevice.WriteData(PCSInstance.Address.WRITE_LASER, 0);
                }
            }
        }

        void LiveCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_bDraw)
            {
                Point pos = e.GetPosition(this.LiveCanvas);
                if (m_bAlignPos1)
                {
                    Canvas.SetLeft(rctAlign1, pos.X - 16);
                    Canvas.SetTop(rctAlign1, pos.Y - 16);
                }
                if (m_bAlignPos2)
                {
                    Canvas.SetLeft(rctAlign2, pos.X - 16);
                    Canvas.SetTop(rctAlign2, pos.Y - 16);
                }
            }
        }

        void btnROI_Click(object sender, RoutedEventArgs e)
        {
            this.btnROI.Visibility = Visibility.Hidden;
            m_bDraw = true;
        }

        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (m_bHW)
            {
                if (CamType == 0) pc.LiveStop();
                else pylonCam.LiveStop();
                if (ReadThread != null)
                {
                    ReadThread.Abort();
                    Thread.Sleep(100);
                    ReadThread = null;
                }
            }
            viewer.CloseDialog();
            this.Hide();
        }

        void server_connected(bool bConnect)
        {
            Connected = bConnect;
            Action action = delegate
            {
                lampConnect.Status = bConnect;
                pMain.InspectionMonitoringCtrl.lampMark.Status = bConnect;
            }; this.Dispatcher.Invoke(action);
        }

        void align_right()
        {

            float x, y;
            int match;
            byte[] tmp = new byte[1280 * 1024];
            int n = pylonCam.SearchAlign1(out x, out y, out match, ref tmp);
            MarginY = y;
            Offset2.X = (double)(x) * Resolution / 1000.0;
            Offset2.Y = (double)(y) * Resolution / 1000.0;
            lblOffsetXRight.Content = x.ToString();
            lblOffsetYRight.Content = y.ToString();
            lblMatchRight.Content = match.ToString();
            if (n != 0)
            {
                MessageBox.Show("얼라인에 실패 했습니다.");
                return;
            }
            if (PCSInstance.PlcDevice.RequestAlign(0))
                PCSInstance.PlcDevice.PassAlign(0);
            if (MainWindow.Setting.SubSystem.Laser.DualLaser)
            {
                if (PCSInstance.PlcDevice.RequestAlign(1))
                    PCSInstance.PlcDevice.PassAlign(1);
            }
        }

        void align_left()
        {
            float x, y;
            int match;
            byte[] tmp = new byte[1280 * 1024];
            int n = pylonCam.SearchAlign2(out x, out y, out match, ref tmp);
            Offset1.X = (double)(x) * Resolution / 1000.0;
            Offset1.Y = (double)(y) * Resolution / 1000.0;
            lblOffsetXLeft.Content = x.ToString();
            lblOffsetYLeft.Content = y.ToString();
            lblMatchLeft.Content = match.ToString();
            if (n != 0)
            {
                MessageBox.Show("얼라인에 실패 했습니다.");
                return;
            }

            Offset.X = Offset1.X;
            double osx = (Offset2.X - Offset1.X) + Math.Abs(MainWindow.CurrentModel.Marker.BoatPos1 - MainWindow.CurrentModel.Marker.BoatPos2);
            double osy = ((MainWindow.CurrentModel.Marker.PosY1 + MarginY) - (MainWindow.CurrentModel.Marker.PosY2 + y)) * MainWindow.Setting.SubSystem.Laser.CamResolution / 1000.0;
            double tan = 0;
            if (osx != 0)
            {
                tan = osy / osx;
            }
            Angle = Math.Atan(tan) * (180 / Math.PI);
            Offset.Y = Offset1.Y;
            lblAngle.Content = Angle.ToString("0.0000");
            if (PCSInstance.PlcDevice.RequestAlign(0))
                PCSInstance.PlcDevice.PassAlign(0);
            if (MainWindow.Setting.SubSystem.Laser.DualLaser)
            {
                if (PCSInstance.PlcDevice.RequestAlign(1))
                    PCSInstance.PlcDevice.PassAlign(1);
            }
        }

        void btnInspectAlignRight_Click(object sender, RoutedEventArgs e)
        {
            if (PCSInstance.PlcDevice == null) return;
            if (!m_bAlignPos1 && !PCSInstance.PlcDevice.RequestAlign(1))
            {
                MessageBox.Show("우측 얼라인 위치가 아닙니다.");
                return;
            }
            if (PCSInstance.PlcDevice.ReadLaserBoat() != 1)
            {
                MessageBox.Show("우측 얼라인 위치가 아닙니다.");
                return;
            }
            float x, y;
            int match;
            int n = 0;
            byte[] tmp;
            if (CamType == 0)
            {
                n = pc.SearchAlign1(out x, out y, out match);
            }
            else
            {
                tmp = new byte[1280 * 1024];
                n = pylonCam.SearchAlign1(out x, out y, out match, ref tmp);
            }
            MarginY = y;
            Offset2.X = (double)(x) * Resolution / 1000.0;
            Offset2.Y = (double)(y) * Resolution / 1000.0;
            lblOffsetXRight.Content = x.ToString();
            lblOffsetYRight.Content = y.ToString();
            lblMatchRight.Content = match.ToString();
            if (n != 0)
            {
                MessageBox.Show("얼라인에 실패 했습니다.");
                return;
            }
            if (PCSInstance.PlcDevice.RequestAlign(0))
                PCSInstance.PlcDevice.PassAlign(0);

            if (MainWindow.Setting.SubSystem.Laser.DualLaser) // 미사용 PLC주소값에 EX) M0000 값 넣을 시 다른 정상 주소의 값이 바뀜..
            {
                if (PCSInstance.PlcDevice.RequestAlign(1))
                    PCSInstance.PlcDevice.PassAlign(1);
            }

        }

        void btnInspectAlignLeft_Click(object sender, RoutedEventArgs e)
        {
            if (PCSInstance.PlcDevice == null) return;
            if (!m_bAlignPos2 && !PCSInstance.PlcDevice.RequestAlign(1))
            {
                MessageBox.Show("좌측 얼라인 위치가 아닙니다.");
                return;
            }
            if (PCSInstance.PlcDevice.ReadLaserBoat() != 2)
            {
                MessageBox.Show("좌측 얼라인 위치가 아닙니다.");
                return;
            }
            float x, y;
            int match;
            int n = 0;
            byte[] tmp;
            if (CamType == 0)
            {
                n = pc.SearchAlign2(out x, out y, out match);
            }
            else
            {
                tmp = new byte[1280 * 1024];
                n = pylonCam.SearchAlign2(out x, out y, out match, ref tmp);
            }
            Offset1.X = (double)(x) * Resolution / 1000.0;
            Offset1.Y = (double)(y) * Resolution / 1000.0;
            lblOffsetXLeft.Content = x.ToString();
            lblOffsetYLeft.Content = y.ToString();
            lblMatchLeft.Content = match.ToString();
            if (n != 0)
            {
                MessageBox.Show("얼라인에 실패 했습니다.");
                return;
            }

            Offset.X = Offset1.X;
            double osx = (Offset2.X - Offset1.X) + Math.Abs(MainWindow.CurrentModel.Marker.BoatPos1 - MainWindow.CurrentModel.Marker.BoatPos2);
            double osy = ((MainWindow.CurrentModel.Marker.PosY1 + MarginY) - (MainWindow.CurrentModel.Marker.PosY2 + y)) * MainWindow.Setting.SubSystem.Laser.CamResolution / 1000.0;
            double tan = 0;
            if (osx != 0)
            {
                tan = osy / osx;
            }
            Angle = Math.Atan(tan) * (180 / Math.PI);
            Offset.Y = Offset1.Y;
            lblAngle.Content = Angle.ToString("0.0000");
            if (PCSInstance.PlcDevice.RequestAlign(0))
                PCSInstance.PlcDevice.PassAlign(0);

            if (MainWindow.Setting.SubSystem.Laser.DualLaser) // 미사용 PLC주소값에 EX) M0000 값 넣을 시 다른 정상 주소의 값이 바뀜..
            {
                if (PCSInstance.PlcDevice.RequestAlign(1))
                    PCSInstance.PlcDevice.PassAlign(1);
            }

        }

        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    InitModelData();
            //}
            //catch
            //{
            //    MessageBox.Show("MRK 파일 이상입니다. 마크 파일을 재생성 해 주세요.");
            //}

            viewer.SaveTeachingData();
            viewer.ReloadMark();
            LoadProject(viewer.markparam.filename);
            MessageBox.Show("전체 적용 완료");
        }

        #region Align
        private Point GetPositionManual(int block, int id)
        {
            Point tmp = new Point();
            double t = 0;
            double l = MainWindow.CurrentModel.Strip.StepPitch * block;
            //if (id == 1)
            // {
            t = (Angle + MainWindow.CurrentModel.Marker.BoatAngle) / (180.0 / Math.PI);
            tmp.X = l * Math.Cos(t);
            tmp.X -= l;
            tmp.Y = (l * Math.Sin(t));
            //}
            //else
            //{
            //    t = (Angle + MainWindow.Setting.General.BoatAngle) / (180.0 / Math.PI);
            //    tmp.X = l * Math.Cos(t);
            //    tmp.X -= l;
            //    tmp.Y = (l * Math.Sin(t));
            //}
            return tmp;
        }

        private Point GetPosition(int block, int ID)
        {
            Point tmp = new Point();
            double t = 0;
            double l = MainWindow.CurrentModel.Strip.StepPitch * block;
            if (ID == 0)
            {
                t = (Angle1 + MainWindow.CurrentModel.Marker.BoatAngle) / (180.0 / Math.PI);
                tmp.X = l * Math.Cos(t);
                tmp.X -= l;
                tmp.Y = l * Math.Sin(t);

            }
            else
            {
                t = (Angle2 + MainWindow.CurrentModel.Marker.BoatAngle) / (180.0 / Math.PI);
                tmp.X = l * Math.Cos(t);
                tmp.X -= l;
                tmp.Y = l * Math.Sin(t);
            }
            return tmp;
        }

        //public bool InspcetAlignLeft(int anID)
        //{
        //    float x, y;
        //    int match;
        //    x = y = match = 0;
        //    bool ret = true;
        //    Action action = null;
        //    for (int j = 0; j < 3; j++)
        //    {
        //        if (CamType == 0)
        //        {
        //            for (int i = 0; i < 5; i++)
        //            {
        //                pc.Grab(1);
        //                int cnt = 0;

        //                while (pc.nGrabDone == 0)
        //                {
        //                    Thread.Sleep(20);
        //                    cnt++;
        //                    if (cnt > 5) break;
        //                }

        //                if (pc.nGrabDone == 1 && i>=1)
        //                {
        //                    break;
        //                }
        //            }

        //            if (pc.nGrabDone != 1)
        //            {
        //                if (j != 2)
        //                    continue;
        //                else
        //                    return false;
        //            }
        //        }
        //        else
        //        {
        //            int cnt1 = 0;

        //            if (pylonCam.nGrabDone == 1)
        //            {
        //                this.Dispatcher.Invoke((ThreadStart)(() => { }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        //                Thread.Sleep(10);
        //                cnt1++;
        //                if (cnt1 > 60)
        //                    break;
        //            }
        //            else
        //            {
        //                Thread.Sleep(100);
        //                if (j == 2)
        //                {
        //                    ret = false;
        //                    break;
        //                }
        //                continue;
        //            }
        //        }

        //        MainWindow.Log("Laser", SeverityLevel.INFO, "Find AlignLeft 성공 ! ");

        //        int n = 0;
        //        byte[] tmp = new byte[1280 * 1024];
        //        if (CamType == 0)
        //        {
        //            n = pc.SearchAlign2(out x, out y, out match);
        //            if (n == -1)
        //            {
        //                if (j != 2) continue;
        //                else
        //                {
        //                    return false;
        //                }
        //            }

        //            action = delegate
        //            {
        //                PixelFormat pf = PixelFormats.Gray8;
        //                int r = (640 * pf.BitsPerPixel + 7) / 8;
        //                BitmapSource bmp = BitmapSource.Create(640, 480, 96, 96, pf, null, pc.result, r);
        //                bmp.Freeze();
        //                pMain.InspectionMonitoringCtrl.imgAlign2.Source = bmp;

        //            }; pMain.Dispatcher.Invoke(action);
        //        }
        //        else
        //        {
        //            n = pylonCam.SearchAlign2(out x, out y, out match, ref tmp);
        //            if (n == -1)
        //            {
        //                if (j != 2) continue;
        //                else
        //                {
        //                    return false;
        //                }
        //            }

        //            action = delegate
        //            {
        //                PixelFormat pf = PixelFormats.Gray8;
        //                int r = (1280 * pf.BitsPerPixel + 7) / 8;
        //                BitmapSource bmp = BitmapSource.Create(1280, 1024, 96, 96, pf, null, tmp, r);
        //                bmp.Freeze();
        //                pMain.InspectionMonitoringCtrl.imgAlign2.Source = bmp;

        //            }; pMain.Dispatcher.Invoke(action);
        //        }

        //        MainWindow.Log("Laser", SeverityLevel.INFO, "AlignLeft 결과 : " + n + " " + j);


        //        if (n == 0) break;
        //    }


        //    double osx, osy;
        //    if (anID == 0)
        //    {
        //        OffsetL1.X = (double)(x) * Resolution / 1000.0;
        //        OffsetL1.Y = (double)(y) * Resolution / 1000.0;
        //        OffsetT1.X = OffsetL1.X * LASER_SCALE;
        //        osx = (OffsetR1.X - OffsetL1.X) + Math.Abs(MainWindow.CurrentModel.Marker.BoatPos1 - MainWindow.CurrentModel.Marker.BoatPos2);
        //        osy = ((MainWindow.CurrentModel.Marker.PosY1 + MarginY1) - (MainWindow.CurrentModel.Marker.PosY2 + y)) * MainWindow.Setting.SubSystem.Laser.CamResolution / 1000.0;
        //    }
        //    else
        //    {
        //        OffsetL2.X = (double)(x) * Resolution / 1000.0;
        //        OffsetL2.Y = (double)(y) * Resolution / 1000.0;
        //        OffsetT2.X = OffsetL2.X * LASER_SCALE;
        //        osx = (OffsetR2.X - OffsetL2.X) + Math.Abs(MainWindow.CurrentModel.Marker.BoatPos1 - MainWindow.CurrentModel.Marker.BoatPos2);
        //        osy = ((MainWindow.CurrentModel.Marker.PosY1 + MarginY2) - (MainWindow.CurrentModel.Marker.PosY2 + y)) * MainWindow.Setting.SubSystem.Laser.CamResolution / 1000.0;
        //    }
        //    double tan = 0;
        //    if (osx != 0)
        //    {
        //        tan = (osy) / osx;
        //    }

        //    if (anID == 0)
        //    {
        //        Angle1 = Math.Atan(tan) * (180 / Math.PI);
        //        Angle = Angle1;
        //        OffsetT1.Y = OffsetL1.Y;
        //        action = delegate
        //        {
        //            pMain.InspectionMonitoringCtrl.lblX1.Content = OffsetL1.X.ToString("F3");
        //            pMain.InspectionMonitoringCtrl.lblY1.Content = OffsetL1.Y.ToString("F3");
        //            pMain.InspectionMonitoringCtrl.lblM1.Content = match.ToString();
        //            pMain.InspectionMonitoringCtrl.lblAngle.Content = Angle.ToString("F3");
        //        }; pMain.Dispatcher.Invoke(action);
        //    }
        //    else
        //    {
        //        Angle2 = Math.Atan(tan) * (180 / Math.PI);
        //        Angle = Angle2;
        //        OffsetT2.Y = OffsetL2.Y;
        //        action = delegate
        //        {
        //            pMain.InspectionMonitoringCtrl.lblX2.Content = OffsetL2.X.ToString("F3");
        //            pMain.InspectionMonitoringCtrl.lblY2.Content = OffsetL2.Y.ToString("F3");
        //            pMain.InspectionMonitoringCtrl.lblM2.Content = match.ToString();
        //            pMain.InspectionMonitoringCtrl.lblAngle.Content = Angle.ToString("F3");
        //        }; pMain.Dispatcher.Invoke(action);
        //    }

        //    MainWindow.Log("Laser", SeverityLevel.INFO, "AlignLeft 계산 성공 리턴 : " + ret);

        //    return ret;
        //}
        public bool InspcetAlignLeft(int anID)
        {
            float x, y;
            int match;
            x = y = match = 0;
            bool ret = true;
            Action action = null;
            for (int j = 0; j < 3; j++)
            {
                if (CamType == 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        pc.Grab(1);
                        int cnt = 0;
                        Thread.Sleep(10);
                        while (pc.nGrabDone == 0)
                        {
                            Thread.Sleep(10);
                            cnt++;
                            if (cnt > 5) break;
                        }
                        if (pc.nGrabDone == 1 && i >= 1)
                        {
                            break;
                        }
                    }
                    if (pc.nGrabDone != 1) return false;
                }
                else
                {
                    while (true)
                    {
                        if (pylonCam.nGrabDone == 1)
                        {
                            Thread.Sleep(100);
                            break;
                        }
                        else
                        {
                            Thread.Sleep(100);
                            if (j == 2)
                            {
                                return false;
                            }
                            continue;
                        }
                    }
                }
                int n = 0;
                byte[] tmp = new byte[1280 * 1024];
                if (CamType == 0)
                {
                    n = pc.SearchAlign2(out x, out y, out match);

                    if (n == -1)
                    {
                        if (j == 2)
                        {
                            return false;
                        }
                        continue;
                    }

                    action = delegate
                    {
                        PixelFormat pf = PixelFormats.Gray8;
                        int r = (640 * pf.BitsPerPixel + 7) / 8;
                        BitmapSource bmp = BitmapSource.Create(640, 480, 96, 96, pf, null, pc.result, r);
                        bmp.Freeze();
                        pMain.InspectionMonitoringCtrl.imgAlign2.Source = bmp;

                    }; pMain.Dispatcher.Invoke(action);
                }
                else
                {
                    n = pylonCam.SearchAlign2(out x, out y, out match, ref tmp);
                    if (n == -1)
                    {
                        if (j == 2)
                        {
                            return false;
                        }
                        continue;
                    }
                    action = delegate
                    {
                        PixelFormat pf = PixelFormats.Gray8;
                        int r = (1280 * pf.BitsPerPixel + 7) / 8;
                        BitmapSource bmp = BitmapSource.Create(1280, 1024, 96, 96, pf, null, tmp, r);
                        bmp.Freeze();
                        pMain.InspectionMonitoringCtrl.imgAlign2.Source = bmp;

                    }; pMain.Dispatcher.Invoke(action);
                }

                if (n == 0) break;
            }
            double osx, osy;
            if (anID == 0)
            {
                OffsetL1.X = (double)(x) * Resolution / 1000.0;
                OffsetL1.Y = (double)(y) * Resolution / 1000.0;
                OffsetT1.X = OffsetL1.X * LASER_SCALE;
                osx = (OffsetR1.X - OffsetL1.X) + Math.Abs(MainWindow.CurrentModel.Marker.BoatPos1 - MainWindow.CurrentModel.Marker.BoatPos2);
                osy = ((MainWindow.CurrentModel.Marker.PosY1 + MarginY1) - (MainWindow.CurrentModel.Marker.PosY2 + y)) * MainWindow.Setting.SubSystem.Laser.CamResolution / 1000.0;
            }
            else
            {
                OffsetL2.X = (double)(x) * Resolution / 1000.0;
                OffsetL2.Y = (double)(y) * Resolution / 1000.0;
                OffsetT2.X = OffsetL2.X * LASER_SCALE;
                osx = (OffsetR2.X - OffsetL2.X) + Math.Abs(MainWindow.CurrentModel.Marker.BoatPos1 - MainWindow.CurrentModel.Marker.BoatPos2);
                osy = ((MainWindow.CurrentModel.Marker.PosY1 + MarginY2) - (MainWindow.CurrentModel.Marker.PosY2 + y)) * MainWindow.Setting.SubSystem.Laser.CamResolution / 1000.0;
            }
            double tan = 0;
            if (osx != 0)
            {
                tan = (osy) / osx;
            }
            if (anID == 0)
            {
                Angle1 = Math.Atan(tan) * (180 / Math.PI);
                Angle = Angle1;
                OffsetT1.Y = OffsetL1.Y;
                action = delegate
                {
                    pMain.InspectionMonitoringCtrl.lblX1.Content = OffsetL1.X.ToString("F3");
                    pMain.InspectionMonitoringCtrl.lblY1.Content = OffsetL1.Y.ToString("F3");
                    pMain.InspectionMonitoringCtrl.lblM1.Content = match.ToString();
                    pMain.InspectionMonitoringCtrl.lblAngle.Content = Angle.ToString("F3");
                }; pMain.Dispatcher.Invoke(action);
            }
            else
            {
                Angle2 = Math.Atan(tan) * (180 / Math.PI);
                Angle = Angle2;
                OffsetT2.Y = OffsetL2.Y;
                action = delegate
                {
                    pMain.InspectionMonitoringCtrl.lblX2.Content = OffsetL2.X.ToString("F3");
                    pMain.InspectionMonitoringCtrl.lblY2.Content = OffsetL2.Y.ToString("F3");
                    pMain.InspectionMonitoringCtrl.lblM2.Content = match.ToString();
                    pMain.InspectionMonitoringCtrl.lblAngle.Content = Angle.ToString("F3");
                }; pMain.Dispatcher.Invoke(action);
            }

            return ret;
        }
        public int index = 0;
        public bool InspcetAlignRight(int anID)
        {
            float x, y;
            int match;
            x = y = match = 0;
            bool ret = true;
            Action action = null;

            //Grab And Search Align
            for (int j = 0; j < 3; j++)
            {
                if (CamType == 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        pc.Grab(0);
                        int cnt = 0;
                        Thread.Sleep(10);
                        while (pc.nGrabDone == 0)
                        {
                            Thread.Sleep(10);
                            cnt++;
                            if (cnt > 5) break;
                        }
                        if (pc.nGrabDone == 1 && i >= 1)
                        {
                            break;
                        }

                        if (pc.nGrabDone != 1)
                        {
                            if (j == 2)
                                return false;
                            else
                                continue;
                        }
                    }

                    int n = pc.SearchAlign1(out x, out y, out match);

                    if (n == -1)
                    {
                        if (j == 2)
                        {
                            return false;
                        }
                        continue;
                    }

                    action = delegate
                    {
                        PixelFormat pf = PixelFormats.Gray8;
                        int r = (640 * pf.BitsPerPixel + 7) / 8;
                        BitmapSource bmp = BitmapSource.Create(640, 480, 96, 96, pf, null, pc.result, r);
                        bmp.Freeze();
                        pMain.InspectionMonitoringCtrl.imgAlign1.Source = bmp;
                    }; pMain.Dispatcher.Invoke(action);
                    if (n == 0) break;
                }
                else
                {

                    while (true)
                    {
                        if (pylonCam.nGrabDone == 1)
                        {
                            Thread.Sleep(100);
                            break;
                        }
                        else
                        {
                            Thread.Sleep(100);
                            if (j == 2)
                            {
                                return false;
                            }
                            continue;
                        }
                    }


                    byte[] tmp = new byte[1280 * 1024];
                    int n = pylonCam.SearchAlign1(out x, out y, out match, ref tmp);

                    if (n == -1)
                    {
                        if (j == 2)
                        {
                            return false;
                        }
                        continue;
                    }

                    action = delegate
                    {
                        PixelFormat pf = PixelFormats.Gray8;
                        int r = (1280 * pf.BitsPerPixel + 7) / 8;
                        BitmapSource bmp = BitmapSource.Create(1280, 1024, 96, 96, pf, null, tmp, r);
                        bmp.Freeze();
                        pMain.InspectionMonitoringCtrl.imgAlign1.Source = bmp;
                    }; pMain.Dispatcher.Invoke(action);

                    if (n == 0) break;
                }
            }


            if (anID == 0)
            {
                OffsetR1.X = (double)(x) * Resolution / 1000.0;
                OffsetR1.Y = (double)(y) * Resolution / 1000.0;
                MarginY1 = y;
                //action = delegate
                //{
                //    pMain.InspectionMonitoringCtrl.lblX1.Content = OffsetR1.X.ToString();
                //    pMain.InspectionMonitoringCtrl.lblY1.Content = OffsetR1.Y.ToString();
                //    pMain.InspectionMonitoringCtrl.lblM1.Content = match.ToString();
                //    pMain.InspectionMonitoringCtrl.lblX2.Content = "";
                //    pMain.InspectionMonitoringCtrl.lblY2.Content = "";
                //    pMain.InspectionMonitoringCtrl.lblAngle.Content = "";
                //}; pMain.Dispatcher.Invoke(action);
            }
            else
            {
                OffsetR2.X = (double)(x) * Resolution / 1000.0;
                OffsetR2.Y = (double)(y) * Resolution / 1000.0;
                MarginY2 = y;
                //action = delegate
                //{
                //    pMain.InspectionMonitoringCtrl.lblX1.Content = OffsetR2.X.ToString();
                //    pMain.InspectionMonitoringCtrl.lblY1.Content = OffsetR2.Y.ToString();
                //    pMain.InspectionMonitoringCtrl.lblM1.Content = match.ToString();
                //    pMain.InspectionMonitoringCtrl.lblX2.Content = "";
                //    pMain.InspectionMonitoringCtrl.lblY2.Content = "";
                //    pMain.InspectionMonitoringCtrl.lblAngle.Content = "";
                //}; pMain.Dispatcher.Invoke(action);
            }

            return ret;
        }
        #endregion Align

        #region Laser Function
        public bool InitLaser()
        {
            return true;
            //    return LoadProject("aaa.mrk");
            // return LoadProject(MainWindow.CurrentModel.MarkingFilePath);
        }
        #endregion Laser Function

        private void ReadPosition()
        {
            bool bAlignPos1, bAlignPos2;
            bAlignPos1 = false;
            bAlignPos2 = false;
            while (true)
            {
                Thread.Sleep(100);
                try
                {
                    currBoatPos = PCSInstance.PlcDevice.ReadBoatPos();
                    currCamPos = PCSInstance.PlcDevice.ReadCamPos(MainWindow.Setting.SubSystem.PLC.MCType);
                    if (PCSInstance.PlcDevice.RequestAlign(0))
                    {
                        int pos = PCSInstance.PlcDevice.ReadLaserBoat();
                        if (pos == 1)
                            bAlignPos1 = true;
                        else
                            bAlignPos1 = false;
                        if (pos == 2)
                            bAlignPos2 = true;
                        else
                            bAlignPos2 = false;
                    }
                    else
                    {
                        bAlignPos1 = false;
                        bAlignPos2 = false;
                    }
                }
                catch
                {
                    bAlignPos1 = false;
                    bAlignPos2 = false;
                }
                if (m_bAlignPos1 != bAlignPos1)
                    m_bAlignPos1 = bAlignPos1;
                if (m_bAlignPos2 != bAlignPos2)
                    m_bAlignPos2 = bAlignPos2;
                Action a = delegate
                {
                    if (m_bAlignPos1 || m_bAlignPos2)
                    {
                        btnROI.Visibility = Visibility.Visible;
                        if (m_bAlignPos1)
                            btnROI.Content = "Right Set";
                        else
                            btnROI.Content = "Left Set";
                    }
                    else
                        btnROI.Visibility = Visibility.Hidden;
                    lblCurrentBoatPos.Content = currBoatPos.ToString("0000.000");
                    lblCurrentCamPos.Content = currCamPos.ToString("00.000");
                }; this.Dispatcher.Invoke(a);
            }
        }

        public bool LoadProject(string filename)
        {
            //return true;
            // int val = SetAlign(AlignFile);
            // if (val != 0) return val;
            if (server != null)
            {
                int val = server.LoadLaser(filename);
                if (val != 0) return false;
                Thread.Sleep(100);
                val = server.MarkReady();
                if (val != 0) return false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="res">해당 검사 결과</param>
        /// <param name="anMarkMode">전체, 레일, 유닛</param>
        /// <param name="anCountMode">숫자 표시</param>
        /// <param name="anInspType">재검 횟수</param>
        /// <param name="abMirror">레일 반전</param>
        /// <param name="anMarkID">마크 ID</param>
        /// <returns></returns>
        public bool RunLaser(InspectBuffer res, int BoatID, int anMarkID, bool bVerify)
        {
            int InspType = MainWindow.CurrentModel.InspectMode;
            int BadCount = 0;
            int tCount = res.BadCount();
            int n = 0;
            bool good = (tCount == 0) ? true : false;
            int nStep = Model.Strip.MarkStep;

            ////////Mark ID에 따라 정리 요망
            double angle = 0;
            double AddAngle = 0;
            string barcode = MainWindow.IDString;

            if (MainWindow.historyID > 0)
            {
                int order = Convert.ToInt32(MainWindow.IDString.Substring(MainWindow.IDString.Length - 5, 2));
                barcode = MainWindow.IDString.Substring(0, 12) + (MainWindow.historyID + (order % 4)).ToString("D2") + MainWindow.IDString.Substring(MainWindow.IDString.Length - 3, 3);
            }

            if (BoatID == 0)
            {
                AddAngle = MainWindow.CurrentModel.Marker.LaserAngle;
                angle = Angle1;
            }
            else
            {
                AddAngle = MainWindow.CurrentModel.Marker.LaserAngle;
                angle = Angle2 + MainWindow.Setting.SubSystem.Laser.Boat2Angle;
            }

            int nNo = 0;
            if (MainWindow.CurrentModel.Marker.NumMark > 0)
            {
                if (MainWindow.CurrentModel.Marker.NumLeft) nNo = 1;
                else nNo = 2;
            }

            for (int i = 0; i < nStep; i++)
            {
                bool b = false;
                while (!b)
                {
                    if (PCSInstance.PlcDevice.RequestLaser(BoatID)) break;
                    Thread.Sleep(50);
                    n++;
                    if (n > 100)
                    {
                        MainWindow.Log("LASER", SeverityLevel.DEBUG, "RequestLaser TimeOut. (Marking Fail)", true);
                        return false;
                    }
                }

                ///////////////////////////
                ///////////////////////////
                //마킹 스킵 테스트
                //PCSInstance.PlcDevice.PassLaser(BoatID);
                // continue;
                ///////////////////////
                /////////////////////////
                //////////////////////////
                Action action = delegate
                {
                    if (BoatID == 0)
                        pMain.InspectionMonitoringCtrl.Mark1Map.SetStep(nStep - i - 1);
                    else
                        pMain.InspectionMonitoringCtrl.Mark2Map.SetStep(nStep - i - 1);
                };
                pMain.Dispatcher.Invoke(action);

                Point tmpPos = new Point(0, 0);

                if (i == (nStep - 1))
                {
                    if (BoatID == 0)
                    {
                        tmpPos = OffsetT1;
                    }
                    else
                    {
                        tmpPos = OffsetT2;
                        tmpPos.X += MainWindow.Setting.SubSystem.Laser.Boat2OffsetX;
                        tmpPos.Y += MainWindow.Setting.SubSystem.Laser.Boat2OffsetY;
                    }
                }
                else
                {
                    if (BoatID == 0)
                    {
                        tmpPos = GetPosition((nStep - 1) - i, 0);
                        tmpPos.X += OffsetT1.X;
                        tmpPos.Y += OffsetT1.Y;
                    }
                    else
                    {
                        tmpPos = GetPosition((nStep - 1) - i, 1);
                        tmpPos.X += OffsetT2.X;
                        tmpPos.Y += OffsetT2.Y;
                        tmpPos.X += MainWindow.Setting.SubSystem.Laser.Boat2OffsetX;
                        tmpPos.Y += MainWindow.Setting.SubSystem.Laser.Boat2OffsetY;
                    }
                }
                int nAllMarkingcount = 0;
                int nOrigincount = 0;
                int nSkipRemarkCnt = 0;
                string s = res.GetLaserString(out BadCount, (nStep - 1) - i, Model.Strip.StepUnits, Model.Strip.UnitColumn, InspType, Model.Strip.MarkStep, bVerify, BoatID, ref nAllMarkingcount, ref nOrigincount, ref nSkipRemarkCnt);

                if (InspType > 0 && nOrigincount > 0 && nAllMarkingcount <= nSkipRemarkCnt && nAllMarkingcount > 0)
                {
                    MainWindow.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, string.Format("InspType = {0}, nOrigincount = {1}, nAllMarkingcount = {2}, nSkipRemarkCnt = {3}", InspType, nOrigincount, nAllMarkingcount, nSkipRemarkCnt));
                    MessageBox.Show(string.Format("재마킹 위험이 있습니다 정지합니다. StripID = {0}", res.IDString));
                    //if (MessageBoxResult.Yes != MessageBox.Show(string.Format("마킹 수량 {0}개 계속 진행 하시겠습니까? ", BadCount), "Information", MessageBoxButton.YesNo))
                    return false;
                }

                if (IsPassLaser(i, BadCount, nStep, tCount))
                {
                    if (PCSInstance.PlcDevice != null) PCSInstance.PlcDevice.PassLaser(BoatID);
                    continue;
                }
                #region 이전 코드
                /*
                if (MainWindow.CurrentModel.Marker.IDMark > 0)
                {
                    if (MainWindow.CurrentModel.Marker.IDMark == 1)
                    {
                        if (i != nStep - 1)
                        {
                            if ((BadCount == 0))
                            {
                                PCSInstance.PlcDevice.PassLaser(BoatID);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (i != 0)
                        {
                            if ((BadCount == 0))
                            {
                                PCSInstance.PlcDevice.PassLaser(BoatID);
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    if (MainWindow.CurrentModel.Marker.NumMark > 0)
                    {
                        if (MainWindow.CurrentModel.Marker.NumLeft)
                        {
                            if (MainWindow.CurrentModel.Marker.WeekMark > 0)
                            {
                                if (i != nStep - 1 && i != 0)/////////두번째 스탭에서 count가 0이면 패스
                                {
                                    if (BadCount == 0)
                                    {
                                        if (MainWindow.CurrentModel.Marker.IDMark == 1 && i == nStep - 1)
                                        { }
                                        else if (MainWindow.CurrentModel.Marker.IDMark == 2 && i == 0)
                                        { }
                                        else
                                        {
                                            if (MainWindow.CurrentModel.Marker.WeekPos != 2)
                                            {
                                                PCSInstance.PlcDevice.PassLaser(BoatID);
                                                continue;
                                            }
                                        }
                                    }

                                    if (BadCount == 0)
                                    {
                                        if (MainWindow.CurrentModel.Marker.IDMark == 1 && i == nStep - 1)
                                        { }
                                        else if (MainWindow.CurrentModel.Marker.IDMark == 2 && i == 0)
                                        { }
                                        else
                                        {
                                            if (MainWindow.CurrentModel.Marker.WeekPos != 2)
                                            {
                                                PCSInstance.PlcDevice.PassLaser(BoatID);
                                                continue;
                                            }
                                        }
                                    }
                                }
                                if ((i == nStep - 1) && (!MainWindow.CurrentModel.Marker.ZeroMark && tCount == 0))
                                {
                                    if (MainWindow.CurrentModel.Marker.IDMark == 1 && i == nStep - 1)
                                    { }
                                    else if (MainWindow.CurrentModel.Marker.IDMark == 2 && i == 0)
                                    { }
                                    else
                                    {
                                        PCSInstance.PlcDevice.PassLaser(BoatID);
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                if ((BadCount == 0) && (i < (nStep - 1)))
                                {
                                    if (MainWindow.CurrentModel.Marker.IDMark == 1 && i == nStep - 1)
                                    { }
                                    else if (MainWindow.CurrentModel.Marker.IDMark == 2 && i == 0)
                                    { }
                                    else
                                    {
                                        PCSInstance.PlcDevice.PassLaser(BoatID);
                                        continue;
                                    }
                                }
                                if (i == (nStep - 1) && (tCount == 0) && (!MainWindow.CurrentModel.Marker.ZeroMark))
                                {
                                    if (MainWindow.CurrentModel.Marker.IDMark == 1 && i == nStep - 1)
                                    { }
                                    else if (MainWindow.CurrentModel.Marker.IDMark == 2 && i == 0)
                                    { }
                                    else
                                    {
                                        PCSInstance.PlcDevice.PassLaser(BoatID);
                                        continue;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.WeekMark > 0)
                            {
                                if (i != nStep - 1 && i != 0)
                                {
                                    if (BadCount == 0)
                                    {
                                        if (MainWindow.CurrentModel.Marker.IDMark == 1 && i == nStep - 1)
                                        { }
                                        else if (MainWindow.CurrentModel.Marker.IDMark == 2 && i == 0)
                                        { }
                                        else
                                        {
                                            if (MainWindow.CurrentModel.Marker.WeekPos != 2)
                                            {
                                                PCSInstance.PlcDevice.PassLaser(BoatID);
                                                continue;
                                            }
                                        }
                                    }
                                }
                                if ((i == 0) && (!MainWindow.CurrentModel.Marker.ZeroMark && tCount == 0))
                                {
                                    if (MainWindow.CurrentModel.Marker.IDMark == 1 && i == nStep - 1)
                                    { }
                                    else if (MainWindow.CurrentModel.Marker.IDMark == 2 && i == 0)
                                    { }
                                    else
                                    {
                                        PCSInstance.PlcDevice.PassLaser(BoatID);
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                if ((BadCount == 0) && (i > 0))
                                {
                                    if (MainWindow.CurrentModel.Marker.IDMark == 1 && i == nStep - 1)
                                    { }
                                    else if (MainWindow.CurrentModel.Marker.IDMark == 2 && i == 0)
                                    { }
                                    else
                                    {
                                        PCSInstance.PlcDevice.PassLaser(BoatID);
                                        continue;
                                    }
                                }
                                if (i == 0 && (tCount == 0) && !MainWindow.CurrentModel.Marker.ZeroMark)
                                {
                                    if (MainWindow.CurrentModel.Marker.IDMark == 1 && i == nStep - 1)
                                    { }
                                    else if (MainWindow.CurrentModel.Marker.IDMark == 2 && i == 0)
                                    { }
                                    else
                                    {
                                        PCSInstance.PlcDevice.PassLaser(BoatID);
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (MainWindow.CurrentModel.Marker.WeekMark > 0)
                        {
                            if (MainWindow.CurrentModel.Marker.WeekPos == 0)
                            {
                                if (i != nStep - 1)
                                {
                                    if (BadCount == 0)
                                    {
                                        if (MainWindow.CurrentModel.Marker.IDMark == 1 && i == nStep - 1)
                                        { }
                                        else if (MainWindow.CurrentModel.Marker.IDMark == 2 && i == 0)
                                        { }
                                        else
                                        {
                                            PCSInstance.PlcDevice.PassLaser(BoatID);
                                            continue;
                                        }
                                    }
                                }
                            }
                            else if (MainWindow.CurrentModel.Marker.WeekPos == 1)
                            {
                                if (i != 0)
                                {
                                    if ((BadCount == 0))
                                    {
                                        if (MainWindow.CurrentModel.Marker.IDMark == 1 && i == nStep - 1)
                                        { }
                                        else if (MainWindow.CurrentModel.Marker.IDMark == 2 && i == 0)
                                        { }
                                        else
                                        {
                                            PCSInstance.PlcDevice.PassLaser(BoatID);
                                            continue;
                                        }
                                    }
                                }
                            }
                            else/////센터
                            {
                                if (!(i != nStep - 1 && i != 0))///////// 가운데가 아닐때
                                {
                                    if (BadCount == 0)
                                    {
                                        //if (MainWindow.CurrentModel.Marker.IDMark == 1)
                                        //{ }
                                        //else if (MainWindow.CurrentModel.Marker.IDMark == 2)
                                        //{ }
                                        //else
                                        //{
                                        PCSInstance.PlcDevice.PassLaser(BoatID);
                                        continue;
                                        //}
                                    }
                                }
                            }
                        }
                        //else if (MainWindow.CurrentModel.Marker.IDMark > 0)
                        //{
                        //    if (MainWindow.CurrentModel.Marker.IDMark == 1)
                        //    {
                        //        if (i != nStep - 1)
                        //        {
                        //            if ((BadCount == 0))
                        //            {
                        //                PCSInstance.PlcDevice.PassLaser(BoatID);
                        //                continue;
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        if (i != 0)
                        //        {
                        //            if ((BadCount == 0))
                        //            {
                        //                PCSInstance.PlcDevice.PassLaser(BoatID);
                        //                continue;
                        //            }
                        //        }
                        //    }
                        //}
                        else
                        {
                            if (BadCount == 0)
                            {
                                PCSInstance.PlcDevice.PassLaser(BoatID);
                                continue;
                            }
                        }
                    }
                }
                */
                #endregion 이전 코드
                if (MainWindow.CurrentModel.Marker.RailIrr)
                {
                    string file = "";
                    file = MainWindow.CurrentModel.Name + "_" + (MainWindow.CurrentModel.Strip.MarkStep - i).ToString() + "STEP.mrk";
                    //if (i == 0) file = MainWindow.CurrentModel.MarkingFilePath;
                    // else if (i == 1) file = MainWindow.CurrentModel.MarkingFilePath2;
                    // else file = MainWindow.CurrentModel.MarkingFilePath3;
                    if (!LoadProject(file))
                    {
                        return false;
                    }
                    Thread.Sleep(1000);
                }

                int nMark = 0;
                if (MainWindow.CurrentModel.Marker.UnitMark > 0 && MainWindow.CurrentModel.Marker.RailMark > 0) nMark = 0;
                if (MainWindow.CurrentModel.Marker.UnitMark == 0 && MainWindow.CurrentModel.Marker.RailMark > 0) nMark = 1;
                if (MainWindow.CurrentModel.Marker.UnitMark > 0 && MainWindow.CurrentModel.Marker.RailMark == 0) nMark = 2;
                //if (MainWindow.CurrentModel.Marker.WeekPos == 2 && (i != nStep - 1 && i != 0)) nMark = 3;/////주차 센터 마킹만 템플릿을 예외로 한다//패치후 불필요

                if (MainWindow.CurrentModel.Marker.RailMark == 2)
                {
                    if (MainWindow.CurrentModel.Marker.UnitMark > 0)
                    {
                        if (BadCount > 0)
                        {
                            MainWindow.Log("LASER", SeverityLevel.DEBUG, "marking", true);
                            if (0 != server.Marking(s, tCount, barcode, tmpPos.X, tmpPos.Y, angle, InspType, 2, 0, 0, week, AddAngle, 
                                MainWindow.CurrentModel.Strip.MarkStep, bVerify, MainWindow.CurrentModel.Marker.RailIrr, MainWindow.CurrentModel.Marker.IDMark, 
                                MainWindow.CurrentModel.Marker.ZeroMark, MainWindow.CurrentModel.Marker.WeekPos))
                            {
                                MainWindow.Log("LASER", SeverityLevel.DEBUG, "Run marking fail BadCount > 0", true);
                                return false;
                            }
                            Thread.Sleep(500);
                        }
                        nAllMarkingcount = 0;
                        nOrigincount = 0;
                        nSkipRemarkCnt = 0;
                        s = res.GetLaserStringRev(out BadCount, (nStep - 1) - i, Model.Strip.StepUnits, Model.Strip.UnitColumn, InspType, Model.Strip.MarkStep, bVerify, BoatID, ref nAllMarkingcount, ref nOrigincount, ref nSkipRemarkCnt);
                        if (InspType > 0 && nOrigincount > 0 && nAllMarkingcount <= nSkipRemarkCnt && nAllMarkingcount > 0)
                        {
                            //if (MessageBoxResult.Yes != MessageBox.Show(string.Format("마킹 수량 {0}개 계속 진행 하시겠습니까? ", BadCount), "Information", MessageBoxButton.YesNo))
                            MainWindow.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, string.Format("InspType = {0}, nOrigincount = {1}, nAllMarkingcount = {2}, nSkipRemarkCnt = {3}", InspType, nOrigincount, nAllMarkingcount, nSkipRemarkCnt));
                            MessageBox.Show(string.Format("재마킹 위험이 있습니다 정지합니다. StripID = {0}", res.IDString));
                            return false;
                        }
                        MainWindow.Log("LASER", SeverityLevel.DEBUG, "marking", true);
                        if (0 != server.Marking(s, tCount, "", tmpPos.X, tmpPos.Y, angle, InspType, 1, nNo, (nStep - 1) - i, week, AddAngle, 
                            MainWindow.CurrentModel.Strip.MarkStep, bVerify, MainWindow.CurrentModel.Marker.RailIrr, MainWindow.CurrentModel.Marker.IDMark,
                            MainWindow.CurrentModel.Marker.ZeroMark, MainWindow.CurrentModel.Marker.WeekPos))
                        {
                            MainWindow.Log("LASER", SeverityLevel.DEBUG, "Run marking fail UnitMark", true);
                            return false;
                        }
                    }
                    else
                    {
                        nAllMarkingcount = 0;
                        nOrigincount = 0;
                        nSkipRemarkCnt = 0;
                        s = res.GetLaserStringRev(out BadCount, (nStep - 1) - i, Model.Strip.StepUnits, Model.Strip.UnitColumn, InspType, Model.Strip.MarkStep, bVerify, BoatID, ref nAllMarkingcount, ref nOrigincount, ref nSkipRemarkCnt);
                        if (InspType > 0 && nOrigincount > 0 && nAllMarkingcount <= nSkipRemarkCnt && nAllMarkingcount > 0)
                        {
                            //if (MessageBoxResult.Yes != MessageBox.Show(string.Format("마킹 수량 {0}개 계속 진행 하시겠습니까? ", BadCount), "Information", MessageBoxButton.YesNo))
                            MainWindow.Log(string.Format("Marking{0}", BoatID), SeverityLevel.DEBUG, string.Format("InspType = {0}, nOrigincount = {1}, nAllMarkingcount = {2}, nSkipRemarkCnt = {3}", InspType, nOrigincount, nAllMarkingcount, nSkipRemarkCnt));
                            MessageBox.Show(string.Format("재마킹 위험이 있습니다 정지합니다. StripID = {0}", res.IDString));
                            return false;
                        }
                        MainWindow.Log("LASER", SeverityLevel.DEBUG, "marking", true);
                        if (0 != server.Marking(s, tCount, barcode, tmpPos.X, tmpPos.Y, angle, InspType, nMark, nNo, (nStep - 1) - i, week, AddAngle, 
                            MainWindow.CurrentModel.Strip.MarkStep, bVerify, MainWindow.CurrentModel.Marker.RailIrr, MainWindow.CurrentModel.Marker.IDMark, 
                            MainWindow.CurrentModel.Marker.ZeroMark, MainWindow.CurrentModel.Marker.WeekPos))
                        {
                            MainWindow.Log("LASER", SeverityLevel.DEBUG, "Run marking fail UnitMark <= 0", true);
                            return false;
                        }
                    }
                }
                else
                {
                    MainWindow.Log("LASER", SeverityLevel.DEBUG, "marking", true);
                    if (0 != server.Marking(s, tCount, barcode, tmpPos.X, tmpPos.Y, angle, InspType, nMark, nNo, (nStep - 1) - i, week, AddAngle, 
                        MainWindow.CurrentModel.Strip.MarkStep, bVerify, MainWindow.CurrentModel.Marker.RailIrr, MainWindow.CurrentModel.Marker.IDMark, 
                        MainWindow.CurrentModel.Marker.ZeroMark, MainWindow.CurrentModel.Marker.WeekPos))
                    {
                        MainWindow.Log("LASER", SeverityLevel.DEBUG, "Run marking fail RailMark != 2", true);
                        return false;
                    }
                }
                Thread.Sleep(100);
                PCSInstance.PlcDevice.PassLaser(BoatID);
            }
            Action action1 = delegate
            {
                if (BoatID == 0)
                    pMain.InspectionMonitoringCtrl.Mark1Map.MarkDone();
                else
                    pMain.InspectionMonitoringCtrl.Mark2Map.MarkDone();
            };
            pMain.Dispatcher.Invoke(action1);
            return true;
        }

        public void close_vision()
        {
            if (pylonCam != null)
            {
                pylonCam.ClearVisionData();
                pylonCam.close_vision();
            }
        }

        public bool IsPassLaser(int Step, int BadCount, int MaxStep, int tCount)
        {
            if (BadCount == 0)
            {
                if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 안할 때, 넘버 좌측, 주차 좌측
                    if (Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 안할 때, 넘버 좌측, 주차 우측

                    if (Step == MaxStep - 1)
                    {
                        if (tCount != 0)
                        {
                            return false;
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                    else if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 안할 때, 넘버 좌측, 주차 중앙
                    if (Step == MaxStep - 1)
                    {
                        if (tCount != 0)
                        {
                            return false;
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                    else if (Step != 0 && Step != MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 안할 때, 넘버 우측, 주차 좌측
                    if (Step == 0)
                    {
                        if (tCount != 0)
                        {
                            return false;
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                    else if (Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 안할 때, 넘버 우측, 주차 우측
                    if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 안할 때, 넘버 우측, 주차 중앙
                    if (Step == 0)
                    {
                        if (tCount != 0)
                        {
                            return false;
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                    else if (Step != 0 && Step != MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 안할 때, 넘버 좌측, 주차 안할 때
                    if (Step == MaxStep - 1)
                    {
                        if (tCount != 0)
                        { 
                            return false;
                        }
                        else
                        { 
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 안할 때, 넘버 좌측, 주차 안할 때
                    if (Step == MaxStep - 1)
                    {
                        if (tCount != 0)
                        { 
                            return false;
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 안할 때, 넘버 좌측, 주차 안할 때
                    if (Step == MaxStep - 1)
                    {
                        if (tCount != 0)
                        {
                            return false;
                        }
                        else 
                        { 
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 안할 때, 넘버 우측, 주차 안할 때
                    if (Step == 0)
                    {
                        if (tCount != 0)
                        {
                            return false;
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 안할 때, 넘버 우측, 주차 안할 때
                    if (Step == 0)
                    {
                        if (tCount != 0)
                        {
                            return false;
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 안할 때, 넘버 우측, 주차 안할 때
                    if (Step == 0)
                    {
                        if (tCount != 0)
                        { 
                            return false;
                        }
                        else
                        { 
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 안할 때, 넘버 안할때, 주차 좌측
                    if (Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 안할 때, 넘버 안할때, 주차 우측
                    if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 안할 때, 넘버 안할때, 주차 중앙
                    if (Step != 0 && Step != MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 안할 때, 넘버 안할때, 주차 좌측
                    if (Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 안할 때, 넘버 안할때, 주차 우측
                    if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 안할 때, 넘버 안할때, 주차 중앙
                    if (Step != 0 && Step != MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking, 넘버 안할때, 주차 모두 안할때
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking, 넘버 안할때, 주차 모두 안할때
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking, 넘버 안할때, 주차 모두 안할때
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking, 넘버 안할때, 주차 모두 안할때
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking, 넘버 안할때, 주차 모두 안할때
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 0 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking, 넘버 안할때, 주차 모두 안할때
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 좌측, 넘버 좌측, 주차 좌측
                    if (Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 좌측, 넘버 좌측, 주차 우측
                    if (Step == 0 && Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 좌측, 넘버 좌측, 주차 중앙
                    if (Step != 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 좌측, 넘버 우측, 주차 좌측
                    if (Step == 0 )
                    {
                        if (tCount != 0)
                        {
                            return false;
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }                 
                    }
                    else if(Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 좌측, 넘버 우측, 주차 우측
                    if (Step == 0 || Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 좌측, 넘버 우측, 주차 중앙
                    if (Step == 0)
                    {
                        if (tCount != 0)
                        {
                            return false;
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                    else
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 좌측, 넘버 좌측, 주차 안함
                    if (Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 좌측, 넘버 좌측, 주차 안함
                    if (Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 좌측, 넘버 좌측, 주차 안함
                    if (Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 좌측, 넘버 우측, 주차 안함
                    if (Step == 0)
                    {
                        if (tCount != 0)
                        { 
                            return false;
                        }
                        else 
                        { 
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                    else if (Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 좌측, 넘버 우측, 주차 안함
                    if (Step == 0)
                    {
                        if (tCount != 0) { return false; }
                        else 
                        { 
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false; 
                        }
                    }
                    else if (Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 좌측, 넘버 우측, 주차 안함
                    if (Step == 0)
                    {
                        if (tCount != 0) 
                        {
                            return false; 
                        }
                        else 
                        { 
                            if (MainWindow.CurrentModel.Marker.ZeroMark) 
                                return false; 
                        }
                    }
                    else if (Step == MaxStep - 1)
                        return false; 
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 좌측, 넘버 안함, 주차 좌측
                    if (Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 좌측, 넘버 안함, 주차 우측
                    if (Step == 0 || Step == MaxStep - 1) 
                        return false; 
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 좌측, 넘버 안함, 주차 중앙
                    if (Step != 0) 
                        return false; 
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 좌측, 넘버 안함, 주차 좌측
                    if (Step == MaxStep - 1) 
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 좌측, 넘버 안함, 주차 우측
                    if (Step == 0 && Step == MaxStep - 1) 
                        return false; 
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 좌측, 넘버 안함, 주차 중앙
                    if (Step != 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 좌측, 넘버 안함, 주차 좌측
                    if (Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 좌측, 넘버 안함, 주차 우측
                    if (Step == 0 || Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 좌측, 넘버 안함, 주차 중앙
                    if (Step != 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 좌측, 넘버 안함, 주차 안함
                    if (Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 좌측, 넘버 안함, 주차 안함
                    if (Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 1 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 좌측, 넘버 안함, 주차 안함
                    if (Step == MaxStep - 1)
                        return false;
                }

                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 우측, 넘버 좌측, 주차 좌측
                    if (Step == 0 || Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 우측, 넘버 좌측, 주차 우측
                    if (Step == 0 || Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 우측, 넘버 좌측, 주차 중앙
                    if(Step == MaxStep - 1)
                    {
                        if (tCount != 0)
                        {
                            return false;
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                    else
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 우측, 넘버 우측 , 주차 좌측
                    if (Step == 0)
                    {
                        if (tCount != 0)
                        {
                            return false;
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                    else if(Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 우측, 넘버 우측 , 주차 우측
                    if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 우측, 넘버 우측 , 주차 중앙
                    if (Step == 0)
                    {
                        if (tCount != 0)
                        {
                            return false;
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                    else if (Step != 0 && Step != MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 우측, 넘버 좌측 , 주차 안함
                    if (Step == MaxStep - 1)
                    {
                        if (tCount != 0)
                        {
                            return false;
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                    else if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 우측, 넘버 좌측 , 주차 안함
                    if (Step == MaxStep - 1)
                    {
                        if (tCount != 0)
                        {
                            return false;
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                    else if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 우측, 넘버 좌측 , 주차 중앙
                    if (Step == MaxStep - 1)
                    {
                        if (tCount != 0)
                        {
                            return false;
                        }
                        else
                        {
                            if (MainWindow.CurrentModel.Marker.ZeroMark)
                                return false;
                        }
                    }
                    else
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 우측, 넘버 우측 , 주차 안함
                    if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 우측, 넘버 우측 , 주차 안함
                    if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark > 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 우측, 넘버 우측 , 주차 안함
                    if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 우측, 넘버 안함 , 주차 좌측
                    if (Step == 0 || Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 우측, 넘버 안함 , 주차 우측
                    if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 우측, 넘버 안함 , 주차 중앙
                    if (Step != MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 우측, 넘버 안함 , 주차 좌측
                    if (Step == 0 || Step == MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 우측, 넘버 안함 , 주차 우측
                    if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark > 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 우측, 넘버 안함 , 주차 중앙
                    if (Step != MaxStep - 1)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 우측, 넘버 안함 , 주차 안함
                    if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 우측, 넘버 안함 , 주차 안함
                    if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 우측, 넘버 안함 , 주차 안함
                    if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 0)
                {////ID Marking 우측, 넘버 안함 , 주차 안함
                    if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 1)
                {////ID Marking 우측, 넘버 안함 , 주차 안함
                    if (Step == 0)
                        return false;
                }
                else if (MainWindow.CurrentModel.Marker.IDMark == 2 && MainWindow.CurrentModel.Marker.NumMark == 0 && MainWindow.CurrentModel.Marker.WeekMark == 0 && !MainWindow.CurrentModel.Marker.NumLeft && MainWindow.CurrentModel.Marker.WeekPos == 2)
                {////ID Marking 우측, 넘버 안함 , 주차 안함
                    if (Step == 0)
                        return false;
                }
                return true;
            }
            return false;
        }
    }
}
