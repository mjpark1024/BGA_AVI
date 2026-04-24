using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IGS.Classes
{
    public class ClientSocket
    {
        #region Member Variables.
        private const int PACKET_HEADER_SIZE = 8;
        private string m_serverIP;                      //Server IP
        private int m_nPort;                            //Server Port

        private Socket m_Socket = null;                 //Connection Socket
        private Thread m_thdWaitMsg = null;             //Receive Thread
        private bool b_thdWaitMsg = true;               //Receive Thread Alive Flag

        private bool bReceived = false;                 //Receive Flag
        private bool bException = false;                //Exception Flag

        private byte[] m_rcvData = null;
        #endregion

        public ClientSocket(string strIP, int anPort)
        {
            m_serverIP = strIP;
            m_nPort = anPort;
        }

        public void Close()
        {
            try
            {
                if(m_Socket != null)
                {
                    SendPacket(ProtocolDefine.CLOSE);
                    m_Socket.Close();
                }
            }
            catch(Exception ex)
            {
                Log(string.Format("IGS Client Close Exception: {0}", ex.Message));
            }
            b_thdWaitMsg = false;

            m_Socket = null;
            m_thdWaitMsg.Abort();
            m_thdWaitMsg = null;
        }

        public bool Connect()
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(m_serverIP), m_nPort);
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_Socket.Connect(ep);

                m_Socket.SendTimeout = 5000;
                Thread.Sleep(500);

                m_thdWaitMsg = new Thread(Receive);
                m_thdWaitMsg.Start();

                bReceived = false;
                bException = false;
                int nCnt = 0;

                SendPacket(ProtocolDefine.PIN);
                while (!bReceived)
                {
                    Thread.Sleep(100);
                    if (bException || nCnt > 50)
                    {
                        m_Socket.Close();
                        Log("IGS Client Connect Fail or Server Database Connection Fail.");
                        return false;
                    }
                    nCnt++;
                }
                
                Log("IGS Client Connect Success.");
                return true;
            }
            catch(Exception ex)
            {
                Log(string.Format("IGS Client Connect Exception: {0}", ex.Message));
                return false;
            }
        }

        public bool RecoveryConnection()
        {
            try
            {
                Close();
                Thread.Sleep(100);
                if(!Connect())
                {
                    Close();
                    return false;
                }

                GC.Collect();
                Thread.Sleep(200);

                return true;
            }
            catch(Exception ex)
            {
                Log(string.Format("IGS Client RecoveryConnection Exception: {0}", ex.Message));
                return false;
            }
        }

        public void Receive()
        {
            lock(this)
            {
                if (m_Socket == null || m_Socket.RemoteEndPoint == null)
                    return;

                int nRcvHeaderSize, nCurHeaderSize, nHeaderCode, nTtlBodySize, nRcvBodySize, nCurBodySize;
                byte[] arrHeader = new byte[PACKET_HEADER_SIZE];

                while(b_thdWaitMsg)
                {
                    nRcvHeaderSize = 0;
                    nCurHeaderSize = 0;

                    do
                    {
                        try
                        {
                            nCurHeaderSize = m_Socket.Receive(arrHeader, nRcvHeaderSize, PACKET_HEADER_SIZE - nRcvHeaderSize, 0);
                        }
                        catch (Exception ex)
                        {
                            string tmp = ex.Message;
                            if (m_Socket == null || !m_Socket.Connected)
                            {
                                b_thdWaitMsg = false;
                                Log(string.Format("IGS Client Receive Header Exception: {0}", tmp));
                                return;
                            }
                            continue;
                        }

                        if (nCurHeaderSize == -1)
                            continue;
                        else
                            nRcvHeaderSize += nCurHeaderSize;
                    } while ((PACKET_HEADER_SIZE != nRcvHeaderSize) && b_thdWaitMsg);

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
                                catch (Exception ex)
                                {
                                    string tmp = ex.Message;
                                    if (m_Socket == null || !m_Socket.Connected)
                                    {
                                        b_thdWaitMsg = false;
                                        Log(string.Format("IGS Client Receive Body Exception: {0}", tmp));
                                        return;
                                    }
                                    continue;
                                }

                                if (nCurBodySize == -1)
                                    continue;
                                else
                                    nRcvBodySize += nCurBodySize;
                            } while ((nTtlBodySize != nRcvBodySize) && b_thdWaitMsg);
                        }

                        ClientSocketReceiveEventArgs received = new ClientSocketReceiveEventArgs(PACKET_HEADER_SIZE + nTtlBodySize, arrTtlPacket);
                        OnReceived(received);
                    }
                }
            }
        }

        public void OnReceived(ClientSocketReceiveEventArgs e)
        {
            try
            {
                int nCode = BitConverter.ToInt32(e.ReceivedData, sizeof(int) * 0);
                int nSize = BitConverter.ToInt32(e.ReceivedData, sizeof(int) * 1);

                switch(nCode)
                {
                    case ProtocolDefine.PIN:
                    case ProtocolDefine.ACK:
                    case ProtocolDefine.REQ:
                        bReceived = true;
                        break;
                    case ProtocolDefine.NAK:
                        Log("IGS Client NAK : 요청 실패");
                        bException = true;
                        break;
                    case ProtocolDefine.ACK_LOT_PROC:
                        Array.Copy(e.ReceivedData, sizeof(int) * 2, m_rcvData, 0, nSize);
                        bReceived = true;
                        break;
                    default:
                        Log("IGS Client TCP: 잘못된 코드의 패킷이 수신되었습니다.");
                        bException = true;
                        break;
                }
            }
            catch(Exception ex)
            {
                Log(string.Format("IGS Client OnReceived Exception: {0}", ex.Message));
                bException = true;
            }
        }

        public void SendPacket(int nSendCode, int nSendSize = 0, byte[] pSendData = null)
        {
            byte[] pPacket = new byte[sizeof(int) * 2 + nSendSize];
            byte[] arrSendCode = BitConverter.GetBytes(nSendCode);
            byte[] arrSendSize = BitConverter.GetBytes(nSendSize);

            arrSendCode.CopyTo(pPacket, sizeof(int) * 0);
            arrSendSize.CopyTo(pPacket, sizeof(int) * 1);

            if ((nSendSize) != 0)
                Array.Copy(pSendData, 0, pPacket, sizeof(int) * 2, nSendSize);

            int nLoop = 0;
            bool bSend;
            do
            {
                bSend = Send(pPacket);
                if (!bSend && !string.IsNullOrEmpty(m_serverIP) && m_nPort > 0)
                {
                    RecoveryConnection();
                }
                if (nLoop++ == 2)
                    break;
            } while (!bSend);
        }

        public bool Send(byte[] buffer)
        {
            if (m_Socket == null || m_Socket.RemoteEndPoint == null)
                return false;

            int nTtlPacketSize = buffer.Length;
            if (nTtlPacketSize <= 0)
            {
                Log("IGS Client TCP 보낼 데이터 없음");
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
                catch (Exception ex)
                {
                    if (m_Socket == null || !m_Socket.Connected)
                    {
                        Log(string.Format("IGS Client TCP Send Exception: {0}", ex.Message));
                        return false;
                    }
                    continue;
                }

                if (nCurPacketSize <= 0)
                {
                    Thread.Sleep(25);
                    continue;
                }
                else
                    nSndPacketSize += nCurPacketSize;
            } while ((nSndPacketSize != nTtlPacketSize) && (nLoop-- > 0));

            return true;
        }

        public void Log(string msg)
        {
            ClientManager.Log(msg);
        }
    }
}
