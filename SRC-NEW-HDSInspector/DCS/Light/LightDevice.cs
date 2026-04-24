using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using Common;
using System.IO;

namespace DCS.Light
{
    public class LightDevice
    {
        public int Port1 = 1;                  // "COM1" 이라면 m_iPort에는 1값이 할당된다.
        public int Port2 = 2;
        public int Port3 = 3;
        public bool IsOpen { get; set; }
        int[] m_LightType; // [1 : 16CH Control 8CH use] [2,4 : 12CH Control 6CH use] [5 : 6CH Control 6CH use]
        private SerialComm m_serialComm1;
        private SerialComm m_serialComm2;
        private SerialComm m_serialComm3;

        public int[,] m_arrLightValue = new int[10,8];
        private LightProtocol m_Protocol = new LightProtocol();

        // 초기화
        public bool Init()
        {
            return true;
        }

        // 초기화
        public int InitLightDevice(string[] aszPort, int[] anLightType, LightProtocol.LIGHT_TYPE anType = LightProtocol.LIGHT_TYPE.PLK_MODULE_6CH)
        {
            m_LightType = anLightType;
            try
            {
                if (aszPort[0].Length == 4)
                    Port1 = Convert.ToInt32(aszPort[0].Substring(3, 1));
                else
                    Port1 = Convert.ToInt32(aszPort[0].Substring(3, 2));
                if (aszPort[1].Length == 4)
                    Port2 = Convert.ToInt32(aszPort[1].Substring(3, 1));
                else
                    Port2 = Convert.ToInt32(aszPort[1].Substring(3, 2));
                if (aszPort[2].Length == 4)
                    Port3 = Convert.ToInt32(aszPort[2].Substring(3, 1));
                else
                    Port3 = Convert.ToInt32(aszPort[2].Substring(3, 2));

                int nResult;

                if (m_serialComm1 == null)
                    m_serialComm1 = new SerialComm(aszPort[0]);
                if (m_serialComm2 == null)
                    m_serialComm2 = new SerialComm(aszPort[1]);
                if (m_serialComm3 == null)
                    m_serialComm3 = new SerialComm(aszPort[2]);

                m_Protocol.LightType = anType;

                m_serialComm1.Open();
                m_serialComm2.Open();
                m_serialComm3.Open();
                nResult = 0;
                if (m_serialComm1.IsOpen)
                    nResult += 0;
                else
                    nResult += 1;
                if (m_serialComm2.IsOpen)
                    nResult += 0;
                else
                    nResult += 2;
                if (m_serialComm3.IsOpen)
                    nResult += 0;
                else
                    nResult += 4;


                IsOpen = m_serialComm1.IsOpen & m_serialComm2.IsOpen & m_serialComm3.IsOpen;
                return nResult;
            }
            catch
            {
                return -1;
            }
        }

        // 종료
        public void ClosePortAll()
        {
            try
            {
                if (m_serialComm1 != null)
                {
                    if (m_serialComm1.IsOpen)
                        SetOnOffEx(0, false);
                    ClosePort(0);
                }
                if (m_serialComm2 != null)
                {
                    if (m_serialComm2.IsOpen)
                        SetOnOffEx(1, false);
                    ClosePort(1);
                }
                if (m_serialComm3 != null)
                {
                    if (m_serialComm3.IsOpen)
                        SetOnOffEx(2, false);
                    ClosePort(2);
                }

            }
            catch
            {
                Debug.WriteLine("Exception occured in ClosePortAll(LightDevice.cs)");
            }
        }

        public void ClosePort(int no)
        {
            try
            {
                if (no == 0)
                {
                    if (m_serialComm1 != null)
                    {
                        if (m_serialComm1.IsOpen)
                            m_serialComm1.Close();
                        m_serialComm1.Dispose();
                    }
                }
                if (no == 1)
                {
                    if (m_serialComm2 != null)
                    {
                        if (m_serialComm2.IsOpen)
                            m_serialComm2.Close();
                        m_serialComm2.Dispose();
                    }
                }
                if (no == 2)
                {
                    if (m_serialComm3 != null)
                    {
                        if (m_serialComm3.IsOpen)
                            m_serialComm3.Close();
                        m_serialComm3.Dispose();
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Exception occured in ClosePort(LightDevice.cs)");
            }
            IsOpen = false;
        }

        // Sets light value.
        public void SetLightValue(int no, int index, int[] arrLightValue)
        {
            try
            {
                for (int i = 0; i < 8; i++)
                {
                    m_arrLightValue[index, i] = arrLightValue[i];
                }
            }
            catch{ }

            SetLight(no, index);
        }

        public void SetBrightness(int no, int anChannel, int anValue)
        {
            String strData = "";
            if (no == 0)
            {
                if (m_serialComm1 != null)
                {
                    if (m_LightType[0] == 5) // 컨트롤러 채널 개수와 UI 조명 채널 개수가 같을 경우
                    {
                        strData = m_Protocol.SetBrightness(anChannel, anValue);
                        m_serialComm1.SendData(strData);
                    }
                    else if (m_LightType[0] == 2 || m_LightType[0] == 4) // 컨트롤러 12ch을 UI 6개 채널로 제어할 경우
                    {
                        if (anChannel > 6) return;
                        int[] nCh = new int[6] { 1, 2, 5, 6, 9, 10 };
                        strData = m_Protocol.SetBrightness(nCh[anChannel - 1], anValue);
                        m_serialComm1.SendData(strData);
                        Thread.Sleep(10);
                        strData = m_Protocol.SetBrightness(nCh[anChannel - 1] + 2, anValue);
                        m_serialComm1.SendData(strData);
                    }
                    else if (m_LightType[0] == 1) // 16CH Control 9~16CH 사용
                    {
                        if (anChannel > 3) return;

                        int[] nCH = new int[3] { 0, 15, 16 };// 돔은 UI 3개 채널 사용

                        if (anChannel == 1) // 돔
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                strData = m_Protocol.SetBrightness(9 + i, anValue);
                                m_serialComm1.SendData(strData);
                                Thread.Sleep(10);
                            }
                        }
                        else // 동축 외곽/중심
                        {
                            strData = m_Protocol.SetBrightness(nCH[anChannel - 1], anValue);
                            m_serialComm1.SendData(strData);
                        }
                    }
                }
            }

            if (no == 1 || no == 2)
            {
                if (m_serialComm2 != null)
                {
                    if (m_LightType[1] == 5) // 6ch
                    {
                        strData = m_Protocol.SetBrightness(anChannel, anValue);
                        m_serialComm2.SendData(strData);
                    }
                    else if (m_LightType[1] == 4 || m_LightType[1] == 2) // 12ch
                    {
                        if (anChannel > 6) return;
                        int[] nCh = new int[6] { 1, 2, 5, 6, 9, 10 };
                        strData = m_Protocol.SetBrightness(nCh[anChannel - 1], anValue);
                        m_serialComm2.SendData(strData);
                        Thread.Sleep(10);
                        strData = m_Protocol.SetBrightness(nCh[anChannel - 1] + 2, anValue);
                        m_serialComm2.SendData(strData);
                    }
                    else if (m_LightType[1] == 1) // 16CH Control 9~16CH 사용
                    {
                        if (anChannel > 3) return;// 돔은 UI 3개 채널 사용

                        int[] nCH = new int[3] { 0, 15, 16 };

                        if (anChannel == 1) // 돔
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                strData = m_Protocol.SetBrightness(9 + i, anValue);
                                m_serialComm2.SendData(strData);
                                Thread.Sleep(10);
                            }
                        }
                        else // 동축 외곽/중심
                        {
                            strData = m_Protocol.SetBrightness(nCH[anChannel - 1], anValue);
                            m_serialComm2.SendData(strData);
                        }
                    }
                }
            }

            if (no == 3 || no == 4)
            {
                if (m_serialComm3 != null)
                {
                    if (m_LightType[2] == 5) // 6ch
                    {
                        strData = m_Protocol.SetBrightness(anChannel, anValue);
                        m_serialComm3.SendData(strData);
                    }
                    else if (m_LightType[2] == 4 || m_LightType[2] == 2) // 12ch
                    {
                        if (anChannel > 6) return;
                        int[] nCh = new int[6] { 1, 2, 5, 6, 9, 10 };
                        strData = m_Protocol.SetBrightness(nCh[anChannel - 1], anValue);
                        m_serialComm3.SendData(strData);
                        Thread.Sleep(10);
                        strData = m_Protocol.SetBrightness(nCh[anChannel - 1] + 2, anValue);
                        m_serialComm3.SendData(strData);
                    }
                    else if (m_LightType[2] == 1) // 16CH Control 9~16CH 사용
                    {
                        if (anChannel > 3) return; // 돔은 UI 3개 채널 사용

                        int[] nCH = new int[3] { 0, 15, 16 };

                        if (anChannel == 1) // 돔
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                strData = m_Protocol.SetBrightness(9 + i, anValue);
                                m_serialComm3.SendData(strData);
                                Thread.Sleep(10);
                            }
                        }
                        else // 동축 외곽/중심
                        {
                            strData = m_Protocol.SetBrightness(nCH[anChannel - 1], anValue);
                            m_serialComm3.SendData(strData);
                        }
                    }
                }
            }
        }

        // Light On/ Off
        public void SetOnOffEx(int no, bool bIsOn)
        {
            if (no == 0)
            {
                if (m_serialComm1 != null)
                {
                    if (bIsOn) m_serialComm1.SendData(String.Format("setonex {0:X8}\r", (int)Math.Pow(2, 16) - 1));
                    else m_serialComm1.SendData(String.Format("setonex {0:X8}\r", 0));
                }
            }
            if (no == 1 || no == 2)
            {
                if (m_serialComm2 != null)
                    if (bIsOn) m_serialComm2.SendData(String.Format("setonex {0:X8}\r", (int)Math.Pow(2, 16) - 1)); // 16ch 반영
                    else m_serialComm2.SendData(String.Format("setonex {0:X8}\r", 0));
            }
            if (no == 3 || no == 4)
            {
                if (m_serialComm3 != null)
                    if (bIsOn) m_serialComm3.SendData(String.Format("setonex {0:X8}\r", (int)Math.Pow(2, 16) - 1));
                    else m_serialComm3.SendData(String.Format("setonex {0:X8}\r", 0));
            }
        }

        private string GetOnOffEx(int no)
        {
             String receive = "";
            if (no == 0)
            {
                if (m_serialComm1 == null)
                    return "";

                String strData = m_Protocol.GetOnex();
                m_serialComm1.SendData(strData);

                while (m_serialComm1.WaitReceivingFlag == true)
                {
                    Thread.Sleep(1);
                }

                receive = m_serialComm1.recieved_data;
            }
            if (no == 1 || no == 2)
            {
                if (m_serialComm2 == null)
                    return "";

                String strData = m_Protocol.GetOnex();
                m_serialComm2.SendData(strData);

                while (m_serialComm2.WaitReceivingFlag == true)
                {
                    Thread.Sleep(1);
                }

                receive = m_serialComm2.recieved_data;
            }
            if (no == 3 || no == 4)
            {
                if (m_serialComm3 == null)
                    return "";

                String strData = m_Protocol.GetOnex();
                m_serialComm3.SendData(strData);

                while (m_serialComm3.WaitReceivingFlag == true)
                {
                    Thread.Sleep(1);
                }

                receive = m_serialComm3.recieved_data;
            }
            String strTmp = "0";

            String strGetProtocol = "getonex 0x";
            String strSub = receive.Substring(0, strGetProtocol.Length);
            if (strSub == strGetProtocol)
            {
                strTmp = receive.Substring(strGetProtocol.Length, receive.Length - strGetProtocol.Length);
            }
            return strTmp;
        }

        // 여기서 currentValues는 1~15사이의 값으로, this.GetOnOffEx()의 반환값이다.
        // 이 값은 시리얼통신에서 받아오고, 4개 채널의 On/Off가 통합되어 있는 값이다.
        // 따라서 bit masking으로 각 채널의 On/Off를 구분한다.
        private bool[] GetEachChannelValues(int currentValues)
        {
            // c#에서 bool은 기본적으로 false로 초기화.
            bool[] eachvalues = new bool[6];

            // ch1 => 0001 // ch2 => 0010 // ch4 => 0100 // ch8 => 1000
            if ((currentValues & 1) == 1)
            {
                eachvalues[0] = true;
            }

            if ((currentValues & 2) == 2)
            {
                eachvalues[1] = true;
            }

            if ((currentValues & 4) == 4)
            {
                eachvalues[2] = true;
            }

            if ((currentValues & 8) == 8)
            {
                eachvalues[3] = true;
            }

            if ((currentValues & 16) == 16)
            {
                eachvalues[4] = true;
            }

            if ((currentValues & 32) == 32)
            {
                eachvalues[5] = true;
            }

            return eachvalues;
        }

        // 여기서 1번 채널 이란,, 2진수 0000 중에 첫 번째 자리를 말한다.
        private int SetEachChannelValues(bool channel1, bool channel2, bool channel3, bool channel4, bool channel5, bool channel6)
        {
            int onoffvalues = 0;

            // ch1 => 0001 // ch2 => 0010 // ch4 => 0100 // ch8 => 1000
            if (channel1 == true)
            {
                onoffvalues += 1;
            }

            if (channel2 == true)
            {
                onoffvalues += 2;
            }

            if (channel3 == true)
            {
                onoffvalues += 4;
            }

            if (channel4 == true)
            {
                onoffvalues += 8;
            }

            if (channel5 == true)
            {
                onoffvalues += 16;
            }

            if (channel6 == true)
            {
                onoffvalues += 32;
            }

            return onoffvalues;
        }

        public void SetLight(int no, int ValueIndex)
        {
            for (int nIndex = 0; nIndex < 8; nIndex++)
                SetBrightness(no, nIndex + 1, m_arrLightValue[ValueIndex, nIndex]);
        }
    }
}
