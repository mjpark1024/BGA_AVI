using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Net.Sockets;
using System.Threading;

namespace Maker_Client
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpClient Client;
        NetworkStream Stream;
        bool Connected = false;
        Thread ReadThread;
        ManualResetEvent connectDone = new ManualResetEvent(false);
        IAsyncResult AR;
        int buf_Size = 0;
        Thread thd;

        public MainWindow()
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed;
            this.Loaded += MainWindow_Loaded;
            this.lbLog.PreviewMouseWheel += new MouseWheelEventHandler(lbLog_PreviewMouseWheel);
            this.btnConnect.Click += btnConnect_Click;
        }

        void btnConnect_Click(object sender, RoutedEventArgs e)
        {
           
        }

        void lbLog_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double m_MouseScroll = this.svLogViewer.VerticalOffset;

            m_MouseScroll -= e.Delta / 3;
            // At Start Point
            if (m_MouseScroll < 0)
            {
                m_MouseScroll = 0;
                this.svLogViewer.ScrollToVerticalOffset(m_MouseScroll);
            }
            // At End Point
            else if (m_MouseScroll > this.svLogViewer.ScrollableHeight)
            {
                m_MouseScroll = this.svLogViewer.ScrollableHeight;
                this.svLogViewer.ScrollToEnd();
            }
            // Middle
            else
            {
                this.svLogViewer.ScrollToVerticalOffset(m_MouseScroll);
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Connect("192.168.20.20", 2001);
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            Close();
        }

        private void AddLog(LogType log)
        {
            try
            {
                Action a = delegate
                {
                    this.lbLog.Items.Add(log);
                    this.svLogViewer.ScrollToVerticalOffset(svLogViewer.MaxHeight);
                }; this.Dispatcher.Invoke(a);
            }
            catch { }
        }

        public void ConnectCallback(IAsyncResult ar)
        {
            connectDone.Set();
            try
            {
                TcpClient t = (TcpClient)ar.AsyncState;
                t.EndConnect(ar);
                AR = ar;
                Stream = Client.GetStream();
                Stream.ReadTimeout = 100;
                Connected = true;
                buf_Size = Client.ReceiveBufferSize;
                thd = new Thread(Read);
                thd.Start();
            }
            catch
            {
                Thread.Sleep(1000);
                Connect("", 0);
            }
        }



        public bool Connect(string IP, int port)
        {
            Client = new TcpClient(AddressFamily.InterNetwork);
            try
            {
                connectDone.Reset();
                Client.BeginConnect("127.0.0.1", 2001, new AsyncCallback(ConnectCallback), Client);
                connectDone.WaitOne();
                //Client.Connect(IP, port);
            }
            catch
            {
                return false;
            }
            
            return true;
        }

        public void Read()
        {
            while (Connected)
            {
                Thread.Sleep(100);
                ReadData();
            }
        }

        private void ReadData()
        {
            if (!Connected) return;

            if (Stream.DataAvailable)
            {
                byte[] bytes = new byte[buf_Size];
                Stream.Read(bytes, 0, buf_Size);
                string returndata = Encoding.UTF8.GetString(bytes);
                if (returndata.Length > 10)
                {
                    ParseData(returndata);
                }
            }

        }

        private void ParseData(string str)
        {
            AddLog(new LogType(str));
            string[] rdata = new string[5];
            rdata[0] = str.Substring(0, 4);  //ID
            rdata[1] = str.Substring(4, 3);  //구분
            rdata[2] = str.Substring(7, 5);  //STAMP
            rdata[3] = str.Substring(11, 4); //DATA LENGTH
            int i = str.IndexOf(Convert.ToChar(0x03));
            int v = i - 16 - 1;
            if (v > 0)
            {
                rdata[4] = str.Substring(17, v); //DATA
            }
            else rdata[4] = "";
            switch (rdata[1])
            {
                case "SNA":
                case "SNG":
                case "SNO":
                    SetData(rdata);
                    break;
                case "TMS":
                    Marking(rdata);
                    break;
                case "CHM":
                    Check(rdata);
                    break;
                case "LDP":
                    LoadProject(rdata);
                    break;
                case "ACK":
                    break;
                default:
                    AddLog(new LogType("0002NAK" + rdata[2] + "0000" + Convert.ToChar(0x02) + Convert.ToChar(0x03)));
                    SendData("0002NAK" + rdata[2] + "0000" + Convert.ToChar(0x02) + Convert.ToChar(0x03));
                    break;
            }
        }

        private void SetData(string[] astr)
        {
            string str = "0002ACK" + astr[2] + "0000" + Convert.ToChar(0x02) + Convert.ToChar(0x03);
            SendData(str);
            AddLog(new LogType(str));
        }

        private void Marking(string[] astr)
        {
            string str = "0002ACK" + astr[2] + "0000" + Convert.ToChar(0x02) + Convert.ToChar(0x03);
            SendData(str);
            AddLog(new LogType(str));
            Thread.Sleep(10);
            str = "0002MPS000010002" + Convert.ToChar(0x02) + "ON" + Convert.ToChar(0x03);
            SendData(str);
            AddLog(new LogType(str));
            Thread.Sleep(4000);
            str = "0002MPS000020003" + Convert.ToChar(0x02) + "OFF" + Convert.ToChar(0x03);
            SendData(str);
            AddLog(new LogType(str));
        }

        private void Check(string[] astr)
        {
            string str = "0002ACK" + astr[2] + "0000" + Convert.ToChar(0x02) + Convert.ToChar(0x03);
            SendData(str);
            AddLog(new LogType(str));
            Thread.Sleep(100);
            str = "0002ACM000010007" + Convert.ToChar(0x02) + "SUCCESS" + Convert.ToChar(0x03);
            SendData(str);
            AddLog(new LogType(str));
        }

        private void LoadProject(string[] astr)
        {
            string str = "0002ACK" + astr[2] + "0000" + Convert.ToChar(0x02) + Convert.ToChar(0x03);
            SendData(str);
            AddLog(new LogType(str));
            Thread.Sleep(100);
            str = "0002LPR000010007" + Convert.ToChar(0x02) + "SUCCESS" + Convert.ToChar(0x03);
            SendData(str);
            AddLog(new LogType(str));
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
        public new int Close()
        {
            Connected = false;
            if (ReadThread != null)
                ReadThread = null;

            if (thd != null)
            {
                thd.Abort();
                thd = null;
            }

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

    public class LogType
    {
        public LogType(string aszMessage) : this(DateTime.Now, aszMessage) { }
        public LogType(DateTime aTime, string aszMessage)
        {
            this.Time = aTime;
            this.Message = aszMessage;
        }

        public DateTime Time { get; set; }
        public string Message { get; set; }
    }
}
