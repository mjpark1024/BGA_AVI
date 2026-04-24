using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using PCS;

namespace HDSInspector
{
    /// <summary>
    /// PLCSetupWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PLCSetupWindow : Window
    {
        bool bManual = false;
        MainWindow main;
        private readonly DeviceController PCSInstance;
        bool bStart = false;
        int ID = 0;
        Thread tBoat1;
        Thread tBoat2;
        Thread tLaser1;
        Thread tLaser2;
        Thread tResult;
        public PLCSetupWindow(MainWindow aMainWindow)
        {
            
            InitializeComponent();
            main = aMainWindow;
            PCSInstance = main.PCSInstance;
            InitEvent();
        }

        void InitEvent()
        {
            this.Loaded += PLCSetupWindow_Loaded;
            this.btnGet.Click += btnGet_Click;
            this.btnSet.Click += btnSet_Click;
            this.Closed += PLCSetupWindow_Closed;
            this.btnStart.Click += btnStart_Click;
        }

        void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!bStart)
            {
                btnStart.Content = "검사종료";
                tBoat1 = new Thread(StartBoat1);
                tBoat2 = new Thread(StartBoat2);
                tLaser1 = new Thread(Laser1);
                tLaser2 = new Thread(Laser2);
                tResult = new Thread(RequestResult);
                bStart = true;
                tBoat1.Start();
                tBoat2.Start();
                tLaser1.Start();
                tLaser2.Start();
                tResult.Start();
            }
            else
            {
                CloseThread();
                btnStart.Content = "검사시작";
            }
        }

        void CloseThread()
        {
            bStart = false;
            if (tBoat1 != null)
            {
                tBoat1.Abort(); Thread.Sleep(200); tBoat1 = null;
            }
            if (tBoat2 != null)
            {
                tBoat2.Abort(); Thread.Sleep(200); tBoat2 = null;
            }
            if (tLaser1 != null)
            {
                tLaser1.Abort(); Thread.Sleep(200); tLaser1 = null;
            }
            if (tLaser2 != null)
            {
                tLaser2.Abort(); Thread.Sleep(200); tLaser2 = null;
            }
            if (tResult != null)
            {
                tResult.Abort(); Thread.Sleep(200); tResult = null;
            }

        }

        void PLCSetupWindow_Closed(object sender, EventArgs e)
        {
            CloseThread();
        }

        void btnSet_Click(object sender, RoutedEventArgs e)
        {
            PCSInstance.PlcDevice.WriteData(txtAdd.Text, Convert.ToInt32(txtVal.Text));
        }

        void btnGet_Click(object sender, RoutedEventArgs e)
        {
            int val = PCSInstance.PlcDevice.ReadData(txtAdd.Text);
            txtVal.Text = val.ToString();
        }

        void PLCSetupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnStart.Content = "검사시작";
            bStart = false;
        }

        void StartBoat1()
        {
            Action action = null;
            int iPos = 0;
            action = delegate { lmpRI11.Status = false; lmpRI12.Status = false; }; this.Dispatcher.Invoke(action);
            while (bStart)
            {
                iPos = 0;
                while (iPos==0)
                {
                    action = () =>
                    {
                        iPos = PCSInstance.PlcDevice.RequestBoatCA();
                        if (iPos == 0)
                        {
                            lmpRI11.Status = false; lmpRI12.Status = false;
                        }
                        else if (iPos == 1)
                        {
                            lmpRI11.Status = true; lmpRI12.Status = false;
                        }
                        else
                        {
                            lmpRI11.Status = false; lmpRI12.Status = true;
                        }
                        ///////////////////////ID와 회차 읽어오는거 추가
                    };
                    this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                if (!bManual)
                {
                    action = delegate {
                        if (iPos == 1)
                        {
                            PCSInstance.PlcDevice.SendID(ID);
                            ID++;
                            lblID1.Content = ID.ToString("0000");
                        }
                        Thread.Sleep(50);
                        PCSInstance.PlcDevice.PassBoatCA();
                        lmpRI11.Status = false; lmpRI12.Status = false;
                    }; this.Dispatcher.Invoke(action);
                    Thread.Sleep(500);
                }
                Thread.Sleep(50);
            }
        }

        void StartBoat2()
        {
            Action action = null;
            int iPos = 0;
            action = delegate { lmpRI21.Status = false; lmpRI22.Status = false; }; this.Dispatcher.Invoke(action);

            while (bStart)
            {
                iPos = 0;
                while (iPos==0)
                {
                    
                    action = () =>
                    {
                        iPos = PCSInstance.PlcDevice.RequestBoatBA();
                        ///////////////////////ID와 회차 읽어오는거 추가
                        if (iPos == 0)
                        {
                            lmpRI21.Status = false; lmpRI22.Status = false;
                        }
                        else if (iPos == 1)
                        {
                            lmpRI21.Status = true; lmpRI22.Status = false;
                        }
                        else
                        {
                            lmpRI21.Status = false; lmpRI22.Status = true;
                        }
                    };
                    this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                if (!bManual)
                {
                    action = delegate {
                        if (iPos == 1)
                        {
                            int id = PCSInstance.PlcDevice.ReadBoat2ID();
                            lblID2.Content = id.ToString("0000");
                        }
                        Thread.Sleep(50);
                        PCSInstance.PlcDevice.PassBoatBA();
                        lmpRI21.Status = false; lmpRI22.Status = false;
                    }; this.Dispatcher.Invoke(action);
                    Thread.Sleep(500);
                }
                Thread.Sleep(50);
            }
        }

        void RequestResult()
        {
            Action action = null;
            bool bIsTrue = false;
            int cnt = 0;
            action = delegate { lmpRR.Status = false; }; this.Dispatcher.Invoke(action);
            while (bStart)
            {
                bIsTrue = false;
                while (!bIsTrue)
                {
                    action = () =>
                    {
                        bIsTrue = PCSInstance.PlcDevice.RequestResult();
                        
                        ///////////////////////ID와 
                    };
                    this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                if (!bManual)
                {
                    action = delegate {
                        lmpRR.Status = true;
                        Thread.Sleep(50);
                        int id = PCSInstance.PlcDevice.ReadResultID();
                        lblResID.Content = id.ToString("0000");
                        if (cnt==2)
                            PCSInstance.PlcDevice.SendResult(2);
                        else
                            PCSInstance.PlcDevice.SendResult(1);
                        lmpRR.Status = false;
                    }; this.Dispatcher.Invoke(action);
                    cnt++;
                    if (cnt == 9) cnt = 0;
                    Thread.Sleep(500);
                }
                Thread.Sleep(50);
            }
        }

        void Laser1()
        {
            Action action = null;
            bool bIsTrue = false;
            int cnt = 0;
            action = delegate { lmpRA11.Status = false; lmpRA12.Status = false; }; this.Dispatcher.Invoke(action);
            action = delegate { lmpRL1.Status = false; }; this.Dispatcher.Invoke(action);
            while (bStart)
            {
                bIsTrue = false;
                while (!bIsTrue)
                {
                    action = () =>
                    {
                        bIsTrue = PCSInstance.PlcDevice.RequestAlign(0);
                        ///////////////////////ID와 회차 읽어오는거 추가
                    };
                    this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                if (!bManual)
                {
                    cnt++;
                    if (cnt == 10) cnt = 0;
                    if (cnt == 3)
                    {
                        PCSInstance.PlcDevice.ByPass(0);
                        continue;
                    }
                    action = delegate {
                        lmpRA11.Status = true;
                        Thread.Sleep(500);
                        int id = PCSInstance.PlcDevice.ReadLaser1ID();
                        lblLBID1.Content = id.ToString("0000");
                        PCSInstance.PlcDevice.PassAlign(0);
                        lmpRA11.Status = false;
                    }; this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                bIsTrue = false;
                while (!bIsTrue)
                {
                    action = () =>
                    {
                        bIsTrue = PCSInstance.PlcDevice.RequestAlign(0);
                        ///////////////////////ID와 회차 읽어오는거 추가
                    };
                    this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                if (!bManual)
                {
                    action = delegate
                    {
                        lmpRA12.Status = true;
                        Thread.Sleep(50);
                        PCSInstance.PlcDevice.PassAlign(0);
                        lmpRA12.Status = false;
                    }; this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                bIsTrue = false;
                while (!bIsTrue)
                {
                    action = () =>
                    {
                        bIsTrue = PCSInstance.PlcDevice.RequestLaser(0);
                        ///////////////////////ID와 회차 읽어오는거 추가
                    };
                    this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                if (!bManual)
                {
                    action = delegate {
                        lmpRL1.Status = true; 
                        PCSInstance.PlcDevice.PassLaser(0); 
                    }; this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                bIsTrue = false;
                while (!bIsTrue)
                {
                    action = () =>
                    {
                        bIsTrue = PCSInstance.PlcDevice.RequestLaser(0);
                        ///////////////////////ID와 회차 읽어오는거 추가
                    };
                    this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                if (!bManual)
                {
                    action = delegate
                    {
                        PCSInstance.PlcDevice.PassLaser(0);
                    }; this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                bIsTrue = false;
                while (!bIsTrue)
                {
                    action = () =>
                    {
                        bIsTrue = PCSInstance.PlcDevice.RequestLaser(0);
                        ///////////////////////ID와 회차 읽어오는거 추가
                    };
                    this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                if (!bManual)
                {
                    action = delegate
                    {
                        PCSInstance.PlcDevice.PassLaser(0);
                        lmpRL1.Status = false; 
                    }; this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                Thread.Sleep(50);
            }
        }
        void Laser2()
        {
            Action action = null;
            bool bIsTrue = false;
            action = delegate { lmpRA21.Status = false; lmpRA22.Status = false; }; this.Dispatcher.Invoke(action);
            action = delegate { lmpRL2.Status = false; }; this.Dispatcher.Invoke(action);
            int cnt = 0;
            while (bStart)
            {
                bIsTrue = false;
                while (!bIsTrue)
                {
                    action = () =>
                    {
                        bIsTrue = PCSInstance.PlcDevice.RequestAlign(1);
                        ///////////////////////ID와 회차 읽어오는거 추가
                    };
                    this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                if (!bManual)
                {
                    cnt++;
                    if (cnt == 10) cnt = 0;
                    if (cnt == 2)
                    {
                        PCSInstance.PlcDevice.ByPass(1);
                        continue;
                    }
                    action = delegate
                    {
                        lmpRA21.Status = true;
                        Thread.Sleep(50);
                        int id = PCSInstance.PlcDevice.ReadLaser2ID();
                        lblLBID2.Content = id.ToString("0000");
                        if (MainWindow.Setting.SubSystem.Laser.DualLaser)
                            PCSInstance.PlcDevice.PassAlign(1);
                        lmpRA21.Status = false;
                    }; this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                bIsTrue = false;
                while (!bIsTrue)
                {
                    action = () =>
                    {
                        bIsTrue = PCSInstance.PlcDevice.RequestAlign(1);
                        ///////////////////////ID와 회차 읽어오는거 추가
                    };
                    this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                if (!bManual)
                {

                    action = delegate
                    {
                        lmpRA22.Status = true;
                        Thread.Sleep(50);
                        if (MainWindow.Setting.SubSystem.Laser.DualLaser)
                            PCSInstance.PlcDevice.PassAlign(1);
                        lmpRA22.Status = false;
                    }; this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                bIsTrue = false;
                while (!bIsTrue)
                {
                    action = () =>
                    {
                        bIsTrue = PCSInstance.PlcDevice.RequestLaser(1);
                        ///////////////////////ID와 회차 읽어오는거 추가
                    };
                    this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                if (!bManual)
                {
                    action = delegate
                    {
                        lmpRL2.Status = true;
                        PCSInstance.PlcDevice.PassLaser(1);
                    }; this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                bIsTrue = false;
                while (!bIsTrue)
                {
                    action = () =>
                    {
                        bIsTrue = PCSInstance.PlcDevice.RequestLaser(1);
                        ///////////////////////ID와 회차 읽어오는거 추가
                    };
                    this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                if (!bManual)
                {
                    action = delegate
                    {
                        PCSInstance.PlcDevice.PassLaser(1);
                    }; this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                bIsTrue = false;
                while (!bIsTrue)
                {
                    action = () =>
                    {
                        bIsTrue = PCSInstance.PlcDevice.RequestLaser(1);
                        ///////////////////////ID와 회차 읽어오는거 추가
                    };
                    this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                if (!bManual)
                {
                    action = delegate
                    {
                        PCSInstance.PlcDevice.PassLaser(1);
                        lmpRL2.Status = false;
                    }; this.Dispatcher.Invoke(action);
                    Thread.Sleep(50);
                }
                Thread.Sleep(50);
            }
        }
    }
}
