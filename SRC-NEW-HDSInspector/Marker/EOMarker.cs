using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using Common;

namespace Marker
{
    public delegate void ClientConnectHandler(bool bConnect);
    public class EOTCPServer
    {
       // TcpListener Server;
        TcpClient Client;
        StreamReader Reader;
        StreamWriter Writer;
        NetworkStream Stream;
        int Port;
        string IP;
        string Stamp;
        int TimeStamp = 1;
        string[] rdata = new string[5];
        List<string> sl_Log;
        public event ClientConnectHandler connected;
        bool Connected = false;
        bool m_Debug = false;
        
        //Thread ListenThread;

        ManualResetEvent connectDone = new ManualResetEvent(false);
        IAsyncResult AR;

        public EOTCPServer(int port, bool Debug)
        {           
            sl_Log = new List<string>();
            m_Debug = !Debug;
        }

        public void ConnectCallback(IAsyncResult ar)
        {
            connectDone.Set();
            try
            {
                TcpClient t = (TcpClient)ar.AsyncState;
                t.EndConnect(ar);
                Connected = true;
            }
            catch
            {
                Connected = false;
            }
            AR = ar;
        }

        public bool Connect(string ip, int port)
        {
            IP = ip;
            Port = port;
            Connected = false;
            Client = new TcpClient(AddressFamily.InterNetwork);
            try
            {
                connectDone.Reset();
                Client.BeginConnect(ip, port, new AsyncCallback(ConnectCallback), Client);
                connectDone.WaitOne();
                Stream = Client.GetStream();
                Reader = new StreamReader(Stream);
                Writer = new StreamWriter(Stream);

                //Stream.ReadTimeout = 100;
                Connected = true;
                ClientConnectHandler er = connected;
                if (er != null) er(true);
                // buf_Size = Client.ReceiveBufferSize;
                return true;
                //Client.Connect(IP, port);
            }
            catch
            {
                ClientConnectHandler er = connected;
                if (er != null) er(false);
                return false;
            }

        }

        public void Delay(int ms)
        {
            int time = Environment.TickCount;

            do
            {
                if (Environment.TickCount - time >= ms)
                    return;
            } while (true);
        }

        ~EOTCPServer()
        {
            Close();
        }

        public void Close()
        {
            EndClient();
        }

        public void AddMessage(string str)
        {
            DateTime dt = DateTime.Now;
            sl_Log.Add(str + "  :  " + dt.ToString(@"M/d/yyyy hh:mm:ss tt"));
            sl_Log.Clear();
        }

        public bool Retry()
        {
            EndClient();
            Thread.Sleep(1000);
            return Connect(IP, Port);
        }

        public string ReadClient()
        {
            try
            {
                int cnt = 0;
                while(true)
                {
                    Thread.Sleep(100);
                    if (CheckClient())
                    {
                        Thread.Sleep(10);
                        if (Stream.DataAvailable)
                        {

                            byte[] bytes = new byte[Client.ReceiveBufferSize];
                            Stream.Read(bytes, 0, (int)Client.ReceiveBufferSize);
                            string returndata = Encoding.UTF8.GetString(bytes);
                            if (returndata.Length > 0)
                            {
                                return returndata;
                            }

                        }
                    }
                    else 
                        return "";
                    cnt++;
                    if (cnt > 3000) 
                        return "";
                }
             
            }
            catch (Exception)
            {
                return "";
            }
        }



        private void EndClient()
        {
            if (Reader != null)
                Reader.Close();
            if (Writer != null)
                Writer.Close();
            if (Client != null)
                Client.Close();
        }

        public bool CheckClient()
        {
            if (Client != null)
            {
                if (Client.Connected)
                {
                    return true;
                }
                else
                {
                    if (Connected)
                    {
                        ClientConnectHandler er = connected;
                        if (er != null) er(false);
                        EndClient();
                        Thread.Sleep(100);
                        Connect(IP, Port);
                    }
                    return false;
                }
            }
            return false;
        }

        public void SendClient(string msg)
        {
            Writer.Write(msg);
            Writer.Flush();
        }

        public List<string[]> SetReadData(string str)
        {
            List<string[]> lstStr = new List<string[]>();
            if (str.Length <= 16)
            {
                return null;
            }
            string[] tmp = null;
            if (str.Length > 16)
            {
                tmp = str.Split(Convert.ToChar(0x03));
            }
            for (int i = 0; i < tmp.Length; i++)
            {
                if (tmp[i].Length > 16)
                {
                    string[] s = new string[5];
                    s[0] = tmp[i].Substring(0, 4);
                    if (s[0] != "0002") continue;
                    s[1] = tmp[i].Substring(4, 3);
                    s[2] = tmp[i].Substring(7, 5);
                    s[3] = tmp[i].Substring(12, 4);
                    int l = Convert.ToInt32(s[3]);
                    s[4] = "";
                    if (l > 0)
                    {
                        s[4] = tmp[i].Substring(17, tmp[i].Length-17);
                    }
                    lstStr.Add(s);
                }
            }
            return lstStr;
        }

        public void ReturnNAK()
        {
            if (CheckClient())
            {
                Stamp = TimeStamp.ToString("D5");
                string t = "0001NAK" + Stamp + "0000" + Convert.ToChar(0x02) + Convert.ToChar(0x03);
                TimeStamp++;
                if (TimeStamp > 99999) TimeStamp = 1;
                AddMessage("Server: " + t);
                SendClient(t);
                t = ReadClient();
                List<string[]> tmp = SetReadData(t);
                if ((tmp[0][0] != "0002") && (tmp[0][1] != "ACK") && (tmp[0][2] != Stamp))
                {
                 ///
                }
            }
        }

        public void ReturnACK(string stamp)
        {
            if (CheckClient())
            {
                string t = "0001ACK" + stamp + "0000" + Convert.ToChar(0x02) + Convert.ToChar(0x03);
                AddMessage("Server: " + t);
                SendClient(t);
            }
        }
        /// <param name="Markdata">마킹할 문자 배열</param>
        /// <param name="Count">숫자마킹</param>
        /// <param name="barcode">바코드 마킹</param>
        /// <param name="ax">Offset X</param>
        /// <param name="ay">Offset Y</param>
        /// <param name="angle">Angle</param>
        /// <param name="place">재검 횟수</param>
        /// <param name="mode">마킹 옵션</param>
        /// <param name="countmode">숫자마킹 위치</param>
        /// <param name="Step">스텝</param>
        /// <returns></returns>
        public int Marking(string Markdata, int Count, string barcode, double ax, double ay, double angle, int place, int mode, int countmode, int Step, string week, double addAngle, int ModelStep, bool bVerify, bool Rail, int nIDMode, bool ZeroMark, int WeekMarkPos)
        {
            Stamp = TimeStamp.ToString("D5");
            #region 숫자마킹
            string strPlace = "";  
            string bCode = "";
            if (nIDMode == 0 || place > 0)
            {
                bCode = "";
                if (nIDMode != 0 && place == 4)
                {
                    if (nIDMode == 1 && Step == 0)
                    {
                        bCode = barcode;
                    }
                    else if (nIDMode == 2 && Step == ModelStep - 1)
                    {
                        bCode = barcode;
                    }
                }
            }
            else if (nIDMode == 1 && Step == 0)
            {
                bCode = barcode;
            }
            else if (nIDMode == 2 && Step == ModelStep - 1)
            {
                bCode = barcode;
            }
            if (countmode == 0 || bVerify)
            {
                strPlace = "FRC";
            }
            else if (countmode == 1)
            {
                if (Step == 0)
                {
                    if (!ZeroMark && Count == 0)
                        strPlace = "FRC";
                    else
                    {
                        switch (place)
                        {
                            case 0: strPlace = "FRC" + Count.ToString(); break;
                            case 1: strPlace = "SRC" + Count.ToString(); break;
                            case 2: strPlace = "TRC" + Count.ToString(); break;
                            case 4: strPlace = "FRC" + Count.ToString(); break;
                        }
                    }
                }
                else
                {
                    strPlace = "FRC";
                }
            }
            else
            {
                if (Step == ModelStep - 1)
                {
                    if (!ZeroMark && Count == 0)
                        strPlace = "FRC";
                    else
                    {
                        switch (place)
                        {
                            case 0: strPlace = "FRC" + Count.ToString(); break;
                            case 1: strPlace = "SRC" + Count.ToString(); break;
                            case 2: strPlace = "TRC" + Count.ToString(); break;
                            case 4: strPlace = "FRC" + Count.ToString(); break;
                        }
                    }
                }
                else
                {
                    strPlace = "FRC";
                }
            }
            #endregion

            string strMode = "";
            switch (mode)
            {
                case 1: strMode = "SNG"; break;//// 레일 마킹만 할때
                case 2: strMode = "SNO"; break;//// 유닛 마킹만 할때
                case 3: strMode = "SNC"; break;//// 주차 마킹 센터 일때만 사용
                default: strMode = "SNA"; break;//// 레일, 유닛 모두 할때
            }
            string strWeek = "";
            switch (WeekMarkPos)
            {         
                case 0: if (Step == 0) strWeek = week; break;////좌측
                case 1: if (Step == ModelStep - 1) strWeek = week; break;////우측
                case 2: if (Step != 0 && Step != ModelStep - 1) strWeek = week; break;////센터
                default: strWeek = ""; break;
            }
            if (bVerify) strWeek = "";
            string str = Markdata + ";" + strPlace + ";TBD" + bCode + ";VAD" + ax.ToString("F3") + "," + ay.ToString("F3") + "," + (angle + addAngle).ToString("F3") + ";WWYY" + strWeek;
            string t = "0001" + strMode + Stamp + str.Length.ToString("D4") + Convert.ToChar(0x02) + str + Convert.ToChar(0x03);
            if (m_Debug) return 0;
            if (CheckClient())
            {        
                TimeStamp++;
                if (TimeStamp > 99999) TimeStamp = 1;
                AddMessage("Server: " + t);
                SendClient(t);
                LogWrite(t);
                Thread.Sleep(200);
                if (Rail) Thread.Sleep(200);
                return RunMark();
            }
            return -1;
        }

        private void LogWrite(string str)
        {
            string DirPath = "E:\\ResultPath\\MarkerTEST\\Log\\";
            string FilePath = DirPath + "\\Log_" + DateTime.Today.ToString("YYMMdd") + ".log";
            string temp;

            DirectoryInfo di = new DirectoryInfo(DirPath);
            FileInfo fi = new FileInfo(FilePath);

            try
            {
                if (!di.Exists) Directory.CreateDirectory(DirPath);
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))
                    {
                        temp = string.Format("[{0}] {1}", DateTime.Now, str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(FilePath))
                    {
                        temp = string.Format("[{0}] {1}", DateTime.Now, str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
            }
            catch (Exception e)
            {

            }
        }


        public int RunMark()
        {
            if (CheckClient())
            {
                Stamp = TimeStamp.ToString("D5");
                string t = "0001TMS" + Stamp + "0000" + Convert.ToChar(0x02) + Convert.ToChar(0x03);
                TimeStamp++;
                if (TimeStamp > 99999) TimeStamp = 1;
                AddMessage("Server: " + t);
                SendClient(t);
                Thread.Sleep(100);
                t = ReadClient();
                List<string[]> tmp = SetReadData(t);
                if (tmp.Count == 0)
                {
                    return -1;
                }
                AddMessage("Laser: " + t);
                if (tmp.Count >= 1)
                {
                    if ((tmp[0][0] != "0002") && (tmp[0][2] != Stamp))
                    {
                        ReturnNAK();
                        return 1;
                        // 메세지 전달실패
                    }
                    else
                    {
                        if (tmp[0][1] != "ACK")
                        {
                            return -1;
                        }
                    }
                }
                if (tmp.Count >= 2)
                {
                    if (tmp[1][0] != "0002")
                    {
                        ReturnNAK();
                        return 1;
                        // 메세지 전달실패
                    }
                    else
                    {
                        if (tmp[1][1] != "MPS")
                        {
                            return -1;
                        }
                        else
                        {
                            Stamp = tmp[1][2];
                            if (tmp[1][4] == "ON")
                            {
                            
                               // ReturnACK(Stamp);
                            }
                            else
                            {
                              //  ReturnACK(Stamp);
                              //  return 1;        // 프로젝트 로딩 실패
                            }
                        }
                    }
                }
                else
                {
                    t = ReadClient();
                    tmp = SetReadData(t);
                    if (tmp.Count == 0) return -1;
                    AddMessage("Laser: " + t);
                    if (tmp.Count > 0)
                    {
                        if (tmp[0][0] != "0002")
                        {
                            ReturnNAK();
                            return 1;
                            // 메세지 전달실패
                        }
                        else
                        {
                            if (tmp[0][1] != "MPS")
                            {
                                return -1;
                            }
                            else
                            {
                                Stamp = tmp[0][2];
                                if (tmp[0][4] == "ON")
                                {
                                    //Thread.Sleep(30);
                                   // ReturnACK(Stamp);
                                }
                                else
                                {
                                   // Thread.Sleep(30);
                                   // ReturnACK(Stamp);
                                   // return 1;        // 프로젝트 로딩 실패
                                }
                            }
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
                t = ReadClient();
                tmp = SetReadData(t);
                if(tmp == null) return -1;
                AddMessage("Laser: " + t);
                if (tmp.Count > 0)
                {
                    if (tmp[0][0] != "0002")
                    {
                        ReturnNAK();
                        return 1;
                        // 메세지 전달실패
                    }
                    else
                    {
                        if (tmp[0][1] != "MPS")
                        {
                            return -1;
                        }
                        else
                        {
                            Stamp = tmp[0][2];
                            if (tmp[0][4] == "OFF")
                            {
                                ReturnACK(Stamp);
                                //Thread.Sleep(100);
                                return 0;        // 프로젝트 로딩 성공
                            }
                            else
                            {
                                ReturnACK(Stamp);
                               // Thread.Sleep(100);
                                return 1;        // 프로젝트 로딩 실패
                            }
                        }
                    }
                }
                else
                {
                    return -1;
                }

            }
            return -1;
        }

        public int MarkStatus(string str)
        {
            if (CheckClient())
            {
                Stamp = TimeStamp.ToString("D5");
                string t = "0001CHM" + Stamp + str.Length.ToString("D4") + Convert.ToChar(0x02) + str + Convert.ToChar(0x03);
                TimeStamp++;
                if (TimeStamp > 99999) TimeStamp = 1;
                AddMessage("Server: " + t);
                SendClient(t);
                Thread.Sleep(100);
                t = ReadClient();
                List<string[]> tmp = SetReadData(t);
                AddMessage("Laser: " + t);
                if (tmp.Count == 0)
                {
                    return -1;
                }
                if ((tmp[0][0] != "0002") && (tmp[0][2] != Stamp))
                {
                    ReturnNAK();
                    return 1;
                    // 메세지 전달실패
                }
                else
                {
                    if (tmp[0][1] != "ACK")
                    {
                        System.Windows.MessageBox.Show(t);
                        return -1;
                    }
                }
                if (tmp.Count >= 2)
                {
                    if (tmp[1][0] != "0002")
                    {
                        ReturnNAK();
                        return 1;
                        // 메세지 전달실패
                    }
                    else
                    {
                        if (tmp[1][1] != "ACM")
                        {
                            return -1;
                        }
                        else
                        {
                            Stamp = tmp[1][2];
                            if (tmp[1][4] == "SUCCESS")
                            {
                                ReturnACK(Stamp);
                                return 0;        // 프로젝트 로딩 성공
                            }
                            else
                            {
                                ReturnACK(Stamp);
                                return 1;        // 프로젝트 로딩 실패
                            }
                        }
                    }
                }
                else
                {
                    t = ReadClient();
                    tmp = SetReadData(t);
                    AddMessage("Laser: " + t);
                    if (tmp.Count > 0)
                    {
                        if (tmp[0][0] != "0002")
                        {
                            ReturnNAK();
                            return 1;
                            // 메세지 전달실패
                        }
                        else
                        {
                            if (tmp[0][1] != "ACM")
                            {
                                return -1;
                            }
                            else
                            {
                                Stamp = tmp[0][2];
                                if (tmp[0][4] == "SUCCESS")
                                {
                                    ReturnACK(Stamp);
                                    return 0;        // 프로젝트 로딩 성공
                                }
                                else
                                {
                                    ReturnACK(Stamp);
                                    return 1;        // 프로젝트 로딩 실패
                                }
                            }
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            else return -1;
        }

        public int LoadLaser(string filename)
        {
            if (CheckClient())
            {
                Thread.Sleep(10);
                if (Stream.DataAvailable)
                {

                    byte[] bytes = new byte[Client.ReceiveBufferSize];
                    Stream.Read(bytes, 0, (int)Client.ReceiveBufferSize);
                    string returndata = Encoding.UTF8.GetString(bytes);
                }
                Thread.Sleep(10);
                if (Stream.DataAvailable)
                {

                    byte[] bytes = new byte[Client.ReceiveBufferSize];
                    Stream.Read(bytes, 0, (int)Client.ReceiveBufferSize);
                    string returndata = Encoding.UTF8.GetString(bytes);
                }
                Thread.Sleep(10);
                if (Stream.DataAvailable)
                {

                    byte[] bytes = new byte[Client.ReceiveBufferSize];
                    Stream.Read(bytes, 0, (int)Client.ReceiveBufferSize);
                    string returndata = Encoding.UTF8.GetString(bytes);
                }
            }
            Thread.Sleep(50);
            if (MarkStatus("MARKSTOP") != 0) return -1;
            Thread.Sleep(100);
            if (CheckClient())
            {
                Stamp = TimeStamp.ToString("D5");
                string t = "0001LDP" + Stamp;
                TimeStamp++;
                if (TimeStamp > 99999) TimeStamp = 1;
                string d = Convert.ToChar(0x02) + filename + Convert.ToChar(0x03);
                string l = filename.Length.ToString("D4");
                string s = t + l + d;
                AddMessage("Server: " + t);
                SendClient(s);
                t = ReadClient();
                List<string[]> tmp = SetReadData(t);
                AddMessage("Laser: " + t);
                if (tmp.Count == 0)
                {
                    ClientConnectHandler er = connected;
                    if (er != null) er(false);
                    EndClient();
                   // Thread ListenThread = new Thread(new ThreadStart(Listen));
                   // ListenThread.Start();
                    return -1;
                }
                if ((tmp[0][0] != "0002") && (tmp[0][2] != Stamp))
                {
                    ReturnNAK();
                    return 1;
                    // 메세지 전달실패
                }
                else
                {
                    if (tmp[0][1] != "ACK")
                    {
                        return -1;
                    }
                }
                if (tmp.Count >= 2)
                {
                    if (tmp[1][0] != "0002")
                    {
                        ReturnNAK();
                        return 1;
                        // 메세지 전달실패
                    }
                    else
                    {
                        if (tmp[1][1] != "LPR")
                        {
                            return -1;
                        }
                        else
                        {
                            Stamp = tmp[1][2];
                            if (tmp[1][4] == "SUCCESS")
                            {
                                ReturnACK(Stamp);
                                return 0;        // 프로젝트 로딩 성공
                            }
                            else
                            {
                                ReturnACK(Stamp);
                                return 1;        // 프로젝트 로딩 실패
                            }
                        }
                    }
                }
                else
                {

                    t = ReadClient();
                    tmp = SetReadData(t);
                    AddMessage("Laser: " + t);
                    if (tmp.Count > 0)
                    {
                        if (tmp[0][0] != "0002")
                        {
                            ReturnNAK();
                            return 1;
                            // 메세지 전달실패
                        }
                        else
                        {
                            if (tmp[0][1] != "LPR")
                            {
                                return -1;
                            }
                            else
                            {
                                Stamp = tmp[0][2];
                                if (tmp[0][4] == "SUCCESS")
                                {
                                    ReturnACK(Stamp);
                                    return 0;        // 프로젝트 로딩 성공
                                }
                                else
                                {
                                    ReturnACK(Stamp);
                                    return 1;        // 프로젝트 로딩 실패
                                }
                            }
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            else return -1;
        }

        public int MarkReady()
        {
            return MarkStatus("MARKREADY");
           
        }
    }

}
