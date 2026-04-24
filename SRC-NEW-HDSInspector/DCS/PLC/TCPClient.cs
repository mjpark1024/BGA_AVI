using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace DCS.PLC
{
    public class TCPClient
    {
        TcpClient Client;
        NetworkStream Stream;
        bool Connected = false;
        Thread ReadThread;
        ManualResetEvent connectDone = new ManualResetEvent(false);
        IAsyncResult AR;
        int buf_Size = 0;
        public void ConnectCallback(IAsyncResult ar)
        {
            connectDone.Set();
            TcpClient t = (TcpClient)ar.AsyncState;
            t.EndConnect(ar);
            AR = ar;
        }



        public bool Connect(string IP, int port)
        {
            Client = new TcpClient(AddressFamily.InterNetwork);
            try
            {
                connectDone.Reset();
                Client.BeginConnect("127.0.0.1", 3000, new AsyncCallback(ConnectCallback), Client);
                connectDone.WaitOne();
                //Client.Connect(IP, port);
            }
            catch
            {
                return false;
            }
            Stream = Client.GetStream();
            Stream.ReadTimeout = 100;
            Connected = true;
            buf_Size = Client.ReceiveBufferSize;
            SendData("C;" + IP + ";" + port.ToString() + ";E;");
            bool bReturn = false;
            int cnt = 0;
            while (!bReturn)
            {
                Thread.Sleep(10);
                string rdata = ReadData();
                string[] str = rdata.Split(';');
                if (str.Length >= 2)
                {
                    if (rdata[1].Equals("N")) return false;
                    else
                    {
                        return true;
                    }
                }

                if (cnt > 100)
                    return false;
                cnt++;
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


        public string ReadData()
        {
            if (!Connected) return "";
            string ret = "";
            
                if (Stream.DataAvailable)
                {
                    byte[] bytes = new byte[buf_Size];
                    Stream.Read(bytes, 0, buf_Size);
                    string returndata = Encoding.UTF8.GetString(bytes);
                    if (returndata.Length > 10)
                    {
                        ret = returndata;
                    }
                }
            
            return ret;
        }

        public int ReadDeviceBlock(string szDevice, int nSize, out int lpdwData)
        {
            lpdwData = 0;
            lock (this)
            {
                SendData("R;" + szDevice + ";" + nSize.ToString() + ";E;");
                bool bReturn = false;
                int cnt = 0;
                while (!bReturn)
                {
                    Thread.Sleep(10);
                    string rdata = ReadData();
                    string[] str = rdata.Split(';');
                    if (str.Length >= 2)
                    {
                        if (str[1].Equals("N")) return -1;
                        else
                        {
                            try
                            {
                                lpdwData = Convert.ToInt32(str[0]);
                                return 0;
                            }
                            catch
                            {
                                return -1;
                            }
                        }
                    }

                    if (cnt > 100)
                        return -1;
                    cnt++;
                }
            }
            return 0;
        }

        public int GetDevice(string szDevice, int nSize, out int lpdwData)
        {
            lpdwData = 0;
            lock (this)
            {
                SendData("R;" + szDevice + ";" + nSize.ToString() + ";E;");
                bool bReturn = false;
                int cnt = 0;
                while (!bReturn)
                {
                    Thread.Sleep(10);
                    string rdata = ReadData();
                    string[] str = rdata.Split(';');
                    if (str.Length >= 2)
                    {
                        if (str[1].Equals("N")) return -1;
                        else
                        {
                            try
                            {
                                lpdwData = Convert.ToInt32(str[0]);
                                return 0;
                            }
                            catch
                            {
                                return -1;
                            }
                        }
                    }

                    if (cnt > 100)
                        return -1;
                    cnt++;
                }
            }
            return 0;
        }

        public int SetDevice(string szDeivce, int nSize, int lData)
        {
            lock (this)
            {
                SendData("W;" + szDeivce + ";" + lData.ToString("0000000") + ";" + nSize.ToString() + ";E;");
                bool bReturn = false;
                int cnt = 0;
                while (!bReturn)
                {
                    Thread.Sleep(10);
                    string rdata = ReadData();
                    string[] str = rdata.Split(';');
                    if (str.Length >= 2)
                    {
                        if (str[1].Equals("N")) return -1;
                        else
                        {
                            return 0;
                        }
                    }

                    if (cnt > 100)
                        return -1;
                    cnt++;
                }
            }
            return 0;
        }

        public int WriteDeviceBlock(string szDeivce, int nSize, ref int lData)
        {
            lock(this)
            {
                SendData("W;" + szDeivce + ";" + lData.ToString("0000000") + ";" + nSize.ToString() + ";E;");
                bool bReturn = false;
                int cnt = 0;
                while (!bReturn)
                {
                    Thread.Sleep(10);
                    string rdata = ReadData();
                    string[] str = rdata.Split(';');
                    if (str.Length >= 2)
                    {
                        if (str[1].Equals("N")) return -1;
                        else
                        {
                            return 0;
                        }
                    }

                    if (cnt > 100)
                        return -1;
                    cnt++;
                }
            }
            return 0;
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
}
