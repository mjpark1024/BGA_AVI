using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace Sockets
{
    #region Event Arguments.
    // 소켓에서 발생한 에러 처리를 위한 이벤트 Argument Class
    public class SocketErrorEventArgs : EventArgs
    {
        private readonly Exception exception;
        private readonly int id;

        public SocketErrorEventArgs(int id, Exception exception)
        {
            this.id = id;
            this.exception = exception;
        }

        public Exception AsyncSocketException
        {
            get { return exception; }
        }

        public int ID
        {
            get { return id; }
        }
    }

    // 소켓의 연결 및 연결해제 이벤트 처리를 위한 Argument Class
    public class SocketConnectionEventArgs : EventArgs
    {
        private readonly int id;

        public SocketConnectionEventArgs(int id)
        {
            this.id = id;
        }

        public int ID
        {
            get { return id; }
        }
    }

    // 소캣의 데이터 전송 이벤트 처리를 위한 Argument Class
    public class SocketSendEventArgs : EventArgs
    {
        private readonly int id;
        private readonly int sendBytes;

        public SocketSendEventArgs(int id, int sendBytes)
        {
            this.id = id;
            this.sendBytes = sendBytes;
        }

        public int SendBytes
        {
            get { return sendBytes; }
        }

        public int ID
        {
            get { return id; }
        }
    }

    // 소켓의 데이터 수신 이벤트 처리를 위한 Argument Class
    public class SocketReceiveEventArgs : EventArgs
    {
        private readonly int id;
        private readonly int receiveBytes;
        private readonly byte[] receiveData;

        public SocketReceiveEventArgs(int id, int receiveBytes, byte[] receiveData)
        {
            this.id = id;
            this.receiveBytes = receiveBytes;
            this.receiveData = receiveData;
        }

        public int ReceiveBytes
        {
            get { return receiveBytes; }
        }

        public byte[] ReceiveData
        {
            get { return receiveData; }
        }

        public int ID
        {
            get { return id; }
        }
    }

    // 동기 서버의 Accept 이벤트를 위한 Argument Class
    public class SocketAcceptEventArgs : EventArgs//, IDisposable
    {
        private Socket conn;

        public SocketAcceptEventArgs(Socket conn)
        {
            this.conn = conn;
        }

        public Socket Worker
        {
            get { return conn; }
        }

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}
        
        //protected virtual void Dispose(bool disposing)
        //{
        //    if (disposing)
        //        if (conn != null)
        //        {
        //            conn.Dispose();
        //            conn = null;
        //        }
        //}

        //~SocketAcceptEventArgs()
        //{
        //    Dispose(false);
        //}
    }
    #endregion

    #region Delegates.
    public delegate void SocketDisconnectEventHandler(object sender, SocketConnectionEventArgs e);
    public delegate void SocketErrorEventHandler(object sender, SocketErrorEventArgs e);
    public delegate void SocketConnectEventHandler(object sender, SocketConnectionEventArgs e);
    public delegate void SocketCloseEventHandler(object sender, SocketConnectionEventArgs e);
    public delegate void SocketSendEventHandler(object sender, SocketSendEventArgs e);
    public delegate void SocketReceiveEventHandler(object sender, SocketReceiveEventArgs e);
    public delegate void SocketAcceptEventHandler(object sender, SocketAcceptEventArgs e);
    #endregion

    // 소켓 Base Class.
    public class SocketClass
    {
        // Event Handler
        public event SocketDisconnectEventHandler OnDisconnect;
        public event SocketErrorEventHandler OnError;
        public event SocketConnectEventHandler OnConnect;
        public event SocketCloseEventHandler OnClose;
        public event SocketSendEventHandler OnSend;
        public event SocketReceiveEventHandler OnReceive;
        public event SocketAcceptEventHandler OnAccept;

        public SocketClass()
        {
            this.ID = -1;
        }

        public SocketClass(int id)
        {
            this.ID = id;
        }

        public int ID { get; set; }

        protected virtual void ErrorOccured(SocketErrorEventArgs e)
        {
            SocketErrorEventHandler handler = OnError;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void Disconnected(SocketConnectionEventArgs e)
        {
            SocketDisconnectEventHandler handler = OnDisconnect;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void Connected(SocketConnectionEventArgs e)
        {
            SocketConnectEventHandler handler = OnConnect;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void Closed(SocketConnectionEventArgs e)
        {
            SocketCloseEventHandler handler = OnClose;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void Sent(SocketSendEventArgs e)
        {
            SocketSendEventHandler handler = OnSend;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void Received(SocketReceiveEventArgs e)
        {
            SocketReceiveEventHandler handler = OnReceive;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void Accepted(SocketAcceptEventArgs e)
        {
            SocketAcceptEventHandler handler = OnAccept;

            if (handler != null)
                handler(this, e);
        }
    }

    public class ClientSyncSocket : SocketClass //, IDisposable
    {
        private const int PACKET_HEADER_SIZE = 8;

        private Socket m_Socket;
        private Thread m_ReceiveThread;

        private IPAddress[] m_ipAddress;
        private IPEndPoint m_EndPoint;

        public bool RecvThread { get; set; }
        public bool IsConnected
        {
            get
            {
                if (m_Socket == null)
                    return false;
                else
                    return m_Socket.Connected;
            }
        }

        public ClientSyncSocket(int id)
        {
            this.ID = id; // Identifier.
        }

        // 연결을 시도한다.
        public bool Connect(string hostAddress, int port)
        {
            try
            {
                m_ipAddress = Dns.GetHostAddresses(hostAddress);
                m_EndPoint = new IPEndPoint(m_ipAddress[0], port);
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_Socket.Connect(m_EndPoint);
                m_Socket.SendTimeout = 1000 * 5;

                Thread.Sleep(100);

                m_ReceiveThread = new Thread(new ThreadStart(Receive));
                m_ReceiveThread.Start();
            }
            catch (System.Exception e)
            {
                ErrorOccured(new SocketErrorEventArgs(this.ID, e));
                return false;
            }
            return true;
        }

        // 데이터 수신을 동기적으로 처리 & Thread 내에서 수행
        public void Receive()
        {
            lock (this)
            {
                if (m_Socket == null)
                {
                    Debug.WriteLine("#### Receive : Connection is Null ID-{0}", ID);
                    return;
                }
                if (m_Socket.RemoteEndPoint == null)
                {
                    Debug.WriteLine("#### Receive : Remote End Point is Null ID-{0}", ID);
                    return;
                }

                int nRcvHeaderSize, nCurHeaderSize, nHeaderCode, nTtlBodySize, nRcvBodySize, nCurBodySize;
                byte[] arrHeader = new byte[PACKET_HEADER_SIZE];

                RecvThread = true;
                while (RecvThread)
                {
                    nRcvHeaderSize = 0;
                    nCurHeaderSize = 0;
                    do
                    {
                        try
                        {
                            nCurHeaderSize = m_Socket.Receive(arrHeader, nRcvHeaderSize, PACKET_HEADER_SIZE - nRcvHeaderSize, 0);
                        }
                        catch (System.Exception e)
                        {
                            Debug.WriteLine("####WARNING#### Receive Time Out ####");
                            if (m_Socket == null || !m_Socket.Connected)
                            {
                                RecvThread = false;
                                ErrorOccured(new SocketErrorEventArgs(this.ID, e));
                                Debug.WriteLine("#### Receive Thread Successfully Exited.");
                                return;
                            }
                            continue;
                        }

                        if (nCurHeaderSize == -1)
                        {
                            Disconnected(new SocketConnectionEventArgs(ID));
                            continue;
                        }
                        else
                        {
                            nRcvHeaderSize += nCurHeaderSize;
                        }
                    } while ((PACKET_HEADER_SIZE != nRcvHeaderSize) && RecvThread);

                    nHeaderCode = BitConverter.ToInt32(arrHeader, 0);
                    nTtlBodySize = BitConverter.ToInt32(arrHeader, 4);
                    if (!((nHeaderCode == 0) && (nTtlBodySize == 0)))
                    {
                        byte[] arrTtlPacket = new byte[nTtlBodySize + PACKET_HEADER_SIZE];
                        arrHeader.CopyTo(arrTtlPacket, 0);
                        if (nTtlBodySize != 0)
                        {
                            nRcvBodySize = 0;
                            nCurBodySize = 0;
                            do
                            {
                                try
                                {
                                    nCurBodySize = m_Socket.Receive(arrTtlPacket, PACKET_HEADER_SIZE + nRcvBodySize, nTtlBodySize - nRcvBodySize, SocketFlags.None);
                                }
                                catch (System.Exception e)
                                {
                                    Debug.WriteLine("####WARNING#### Receive Time Out ####");
                                    if (m_Socket == null || !m_Socket.Connected)
                                    {
                                        RecvThread = false;
                                        ErrorOccured(new SocketErrorEventArgs(this.ID, e));
                                        Debug.WriteLine("#### Receive Thread Successfully Exited.");
                                        return;
                                    }
                                    continue;
                                }

                                if (nCurBodySize == -1)
                                {
                                    Disconnected(new SocketConnectionEventArgs(ID));
                                    continue;
                                }
                                else
                                {
                                    nRcvBodySize += nCurBodySize;
                                }
                            }
                            while ((nTtlBodySize != nRcvBodySize) && RecvThread);
                        }
                        SocketReceiveEventArgs received = new SocketReceiveEventArgs(this.ID, PACKET_HEADER_SIZE + nTtlBodySize, arrTtlPacket);
                        Received(received);
                    }
                }
            }
            Debug.WriteLine("#### Receive Thread Successfully Exited.");
        }

        // 데이터 송신을 동기적으로 처리
        public bool Send(byte[] buffer)
        {
            if (m_Socket == null)
            {
                Debug.WriteLine("m_Socket is Null {0}", ID);
                return false;
            }
            if (m_Socket.RemoteEndPoint == null)
            {
                Debug.WriteLine("Remote End Point is Null {0}", ID);
                return false;
            }
            
            int nTtlPacketSize = buffer.Length;
            if (nTtlPacketSize <= 0)
            {
                ErrorOccured(new SocketErrorEventArgs(ID, new Exception("보낼 데이터 없음")));
                return false;
            }

            int nSndPacketSize = 0;
            int nCurPacketSize = 0;
            int nLoop = 100;
            do
            {
                try
                {
                    nCurPacketSize = m_Socket.Send(buffer, nSndPacketSize, nTtlPacketSize - nSndPacketSize, SocketFlags.None);
                }
                catch (System.Exception e)
                {
                    Debug.WriteLine("####WARNING#### Send Time Out ####");
                    if (m_Socket == null || !m_Socket.Connected)
                    {
                        SocketErrorEventArgs sendError = new SocketErrorEventArgs(this.ID, e);
                        ErrorOccured(sendError);
                        return false;
                    }
                    continue;
                }

                if (nCurPacketSize <= 0)
                {
                    // ErrorOccured(new SocketErrorEventArgs(id, new Exception("Packet 보내기 실패")));
                    Thread.Sleep(25);
                    continue;
                }
                else
                {
                    nSndPacketSize += nCurPacketSize;
                }
            } while ((nSndPacketSize != nTtlPacketSize) && (nLoop-- > 0));

            byte[] arrDummy = new byte[8 * 12];
            for (int nIndex = 0; nIndex < arrDummy.Length; nIndex++)
                arrDummy[nIndex] = 0;

            nTtlPacketSize = arrDummy.Length;
            nSndPacketSize = 0;
            nCurPacketSize = 0;
            do
            {
                try
                {
                    nCurPacketSize = m_Socket.Send(arrDummy, nSndPacketSize, nTtlPacketSize - nSndPacketSize, SocketFlags.None);
                }
                catch (System.Exception e)
                {
                    Debug.WriteLine("####WARNING#### Send Time Out ####");
                    if (m_Socket == null || !m_Socket.Connected)
                    {
                        SocketErrorEventArgs sendError = new SocketErrorEventArgs(this.ID, e);
                        ErrorOccured(sendError);
                        return false;
                    }
                    continue;
                }
                if (nCurPacketSize <= 0)
                {
                    // ErrorOccured(new SocketErrorEventArgs(id, new Exception("Packet 보내기 실패")));
                    Thread.Sleep(25);
                    continue;
                }
                else
                    nSndPacketSize += nCurPacketSize;
            } while ((nSndPacketSize != nTtlPacketSize));

            //if (nLoop <= 0)
            //    ErrorOccured(new SocketErrorEventArgs(id, new Exception("Packet 보내기 실패, 패킷 손실!!")));

            return true;
        }

        public void ShutDown()
        {
            try
            {
                RecvThread = false;
                m_Socket.Shutdown(SocketShutdown.Both);
            }
            catch (System.Exception e)
            {
                ErrorOccured(new SocketErrorEventArgs(ID, e));
            }
        }

        // 소켓 연결을 비동기적으로 종료
        public void Close()
        {
            try
            {
                RecvThread = false;
                if (m_Socket != null)
                {
                    m_Socket.Shutdown(SocketShutdown.Both);
                    m_Socket.Disconnect(true);
                }
            }
            catch (System.Exception e)
            {
                ErrorOccured(new SocketErrorEventArgs(ID, e));
            }
        }

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        //protected virtual void Dispose(bool disposing)
        //{
        //    Close();

        //    if (disposing)
        //        if (m_Socket != null)
        //        {
        //            m_Socket.Dispose();
        //            m_Socket = null;
        //        }
        //}

        //~ClientSyncSocket()
        //{
        //    Dispose(false);
        //}
    }
}
