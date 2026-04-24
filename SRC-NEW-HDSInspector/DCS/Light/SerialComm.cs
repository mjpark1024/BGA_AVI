using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;

namespace DCS.Light
{
    public class SerialComm
    {
        private SerialPort serial = new SerialPort();

        public bool WaitReceivingFlag { get; set; }
        public bool IsOpen { get; set; }

        public string recieved_data;

        public SerialComm(string strPort)
        {
            serial.PortName = strPort;
            serial.BaudRate = 38400;
            serial.Handshake = System.IO.Ports.Handshake.None;
            serial.Parity = Parity.None;
            serial.DataBits = 8;
            serial.StopBits = StopBits.One;
            serial.ReadTimeout = 200;
            serial.WriteTimeout = 50;

            serial.DataReceived += new SerialDataReceivedEventHandler(Recieve);
        }

        public bool Open()
        {
            try
            {
                serial.Open();
                return IsOpen = serial.IsOpen;
            }
            catch
            {
                return IsOpen = serial.IsOpen;
            }

        }

        public bool Close()
        {
            try
            {
                serial.Close();
                return IsOpen = serial.IsOpen;
            }
            catch
            {
                return IsOpen = serial.IsOpen;
            }
        }

        public void Dispose()
        {
            serial.Dispose();
        }

        private void Recieve(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // Collecting the characters received to our 'buffer' (string).
            try
            {
                recieved_data = serial.ReadLine();

                // Console.WriteLine(recieved_data);
                WaitReceivingFlag = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void SendData(string data)
        {
            if (serial.IsOpen)
            {
                try
                {
                    // Send the binary data out the port
                    byte[] hexstring = Encoding.ASCII.GetBytes(data);
                    //There is a intermitant problem that I came across
                    //If I write more than one byte in succesion without a 
                    //delay the PIC i'm communicating with will Crash
                    //I expect this id due to PC timing issues ad they are
                    //not directley connected to the COM port the solution
                    //Is a ver small 1 millisecound delay between chracters
                    foreach (byte hexval in hexstring)
                    {
                        byte[] _hexval = new byte[] { hexval }; // need to convert byte to byte[] to write
                        serial.Write(_hexval, 0, 1);
                        Thread.Sleep(1);
                    }
                    WaitReceivingFlag = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
