using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace DCS
{
    public class SW_Trigger
    {
        SerialPort serial = new SerialPort();
        public SW_Trigger(string portName)
        {
            serial.PortName = portName;
            serial.BaudRate = 9600;
            serial.Handshake = System.IO.Ports.Handshake.None;
            serial.Parity = Parity.None;
            serial.DataBits = 8;
            serial.StopBits = StopBits.One;
            serial.ReadTimeout = 1000;
            serial.WriteTimeout = 100;
            serial.Open();
        }

        ~SW_Trigger()
        {
            serial.Close();
        }

        //트리거 초기화
        public void InitTrigger(int nDir, int nCount, int delay1, int delay2, int nWidth)
        {
            Thread.Sleep(500);
            SendData("%1A1\r");//축 선택
            Thread.Sleep(200);
            SendData(string.Format("%1V{0}\r",nDir));//엔코더 방향
            Thread.Sleep(200);
            SendData(string.Format("%1K{0}\r", nCount));//Cam 트리거 분할수
            Thread.Sleep(200);
            SendData(string.Format("%1P{0} {1} 100 100\r", delay1, delay2));//트리거 시작위치 (축 선택,Cam1,Cam2,Cam3,Cam4)
            Thread.Sleep(200);
            SendData("%1F50000\r");//타임 트리거 주파수
            Thread.Sleep(200);
            SendData(string.Format("%1W{0}\r", nWidth));//트리거 온 펄스 폭
            Thread.Sleep(200);
            SendData("%1S\r");//파라매터 저장
            Thread.Sleep(200);
        }

        //스캔 시작전 트리거 리셋
        public void ResetTrigger()
        {
            SendData("%1R\r");//리셋
            Thread.Sleep(200);
            SendData("%1T\r");//엔코더 트리거 run
            Thread.Sleep(100);
        }

        public void SendData(string data)
        {
            if (serial.IsOpen)
            {
                try
                {
                    byte[] hexstring = Encoding.ASCII.GetBytes(data);
                    foreach (byte hexval in hexstring)
                    {
                        byte[] _hexval = new byte[] { hexval }; // need to convert byte to byte[] to write
                        serial.Write(_hexval, 0, 1);
                        Thread.Sleep(1);
                    }
                }
#pragma warning disable CS0168 // 변수가 선언되었지만 사용되지 않았습니다.
                catch (Exception ex)
#pragma warning restore CS0168 // 변수가 선언되었지만 사용되지 않았습니다.
                {
                    // Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
