using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ACTETHERLib;
using System.ComponentModel;

namespace DCS.PLC
{
    /// <summary>   Plc constant.  </summary>


    public class ActClient : IMitsubiPlc
    {
        private static TCPClient m_PlcInstance = null;
        /// <summary> The instance </summary>
        private static ActClient _Instance = null;
        /// <summary>   Gets the instance. </summary>
        public static ActClient Instance()
        {
            if (_Instance == null)
            {
                _Instance = new ActClient();
                m_PlcInstance = new TCPClient();
            }

            return _Instance;
        }

        /// <summary>   Gets or sets the plc instance. </summary>
        /// <value> The plc instance. </value>
        public TCPClient PlcInstance
        {
            get { return m_PlcInstance; }
            set { m_PlcInstance = value; }
        }

        public bool ConnectPLC(string ip, int port)
        {
            return PlcInstance.Connect(ip, port);
        }
        #region Open & Close.
        public int Open()
        {
            return 0;
        }

        public int Close()
        {
            return PlcInstance.Close();
        }
        #endregion
        #region Get / Set Device.
        public int Read(string szDevice, int nSize, out int lpdwData)
        {
            lpdwData = 0;
            for (int i=0; i<5; i++)
            {
                if (PlcInstance.ReadDeviceBlock(szDevice, nSize, out lpdwData) == 0) break;
            }
            return 0;
        }

        public int GetDevice(string szDeivce, out int lplData)
        {
            lplData = 0;
            for (int i = 0; i < 5; i++)
            {
                if (PlcInstance.GetDevice(szDeivce, 1, out lplData) == 0) break;
            }
            return 0;
        }

        public int SetDevice(string szDeivce, int lData)
        {
            return PlcInstance.SetDevice(szDeivce, 1, lData);
        }
        public int Write(string szDeivce, int nSize, ref int lpdwData)
        {
            return PlcInstance.WriteDeviceBlock(szDeivce, nSize, ref lpdwData);
        }
        public int GetDeviceBlock(string szDevice, int nSize, out int lpdwData)
        {
            return PlcInstance.GetDevice(szDevice, nSize, out lpdwData);
        }
        public int ReadBuffer(int lStartIO, int lAddress, int lReadSize, out short lpwData)
        {
            lpwData = 0;
            return 0;
        }

        public int WriteBuffer(int lStartIO, int lAddress, int lWriteSize, out short lpwData)
        {
            lpwData = 0;
            return 0;
        }
        #endregion
    }

    /// <summary>   Type : ActQJ71E71TCPDev.  </summary>
    public class ActQJ71E71TCPDev : IMitsubiPlc
    {
        /// <summary> The plc instance </summary>
        private static ActQJ71E71TCP m_PlcInstance = null;

        /// <summary> The instance </summary>
        private static ActQJ71E71TCPDev _Instance = null;

        /// <summary>   Gets the instance. </summary>
        public static ActQJ71E71TCPDev Instance()
        {
            if (_Instance == null)
            {
                _Instance = new ActQJ71E71TCPDev();
                m_PlcInstance = new ActQJ71E71TCP();
            }

            return _Instance;
        }

        /// <summary>   Gets or sets the plc instance. </summary>
        /// <value> The plc instance. </value>
        public ActQJ71E71TCP PlcInstance
        {
            get { return m_PlcInstance; }
            set { m_PlcInstance = value; }
        }

        /// <summary>   Connects a plc. </summary>
        /// <remarks>   suoow2, 2014-11-22. </remarks>
        public bool ConnectPLC(string ip, int port)
        {
            this.Close();
            this.SetConnectUnitNumber(0);
            this.SetCpuType(34);
            this.SetDestinationIONumber(0);
            this.SetDidPropertyBit(1);
            this.SetDsidPropertyBit(1);
            this.SetAddress(ip);
            this.SetActIONumber(1023);
            this.SetMultiDropChannelNumer(0);
            this.SetNetWorkNumber(1);
            this.SetSourceNetworkNumber(1);
            this.SetSourceStationNumber(2);
            this.SetStationNumber(1);
            this.SetThroughNetworkType(0);
            this.SetTimeOut(10000);
            this.SetUnitNumber(0);

            if (this.Open() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Setters.
        public void SetAddress(String strAddress)
        {
            PlcInstance.ActHostAddress = strAddress;
        }

        public void SetConnectUnitNumber(int nNumber)
        {
            PlcInstance.ActConnectUnitNumber = nNumber;
        }

        public void SetDestinationIONumber(int nNumber)
        {
            //0x00
            PlcInstance.ActDestinationIONumber = nNumber;
        }

        public void SetDidPropertyBit(int nNumber)
        {
            //0x01
            PlcInstance.ActDidPropertyBit = nNumber;
        }

        public void SetDsidPropertyBit(int nNumber)
        {
            //0x01
            PlcInstance.ActDsidPropertyBit = nNumber;
        }

        public void SetActIONumber(int nNumber)
        {
            //0x3FF
            PlcInstance.ActIONumber = nNumber;
        }

        public void SetMultiDropChannelNumer(int nNumber)
        {
            PlcInstance.ActMultiDropChannelNumber = nNumber;
        }

        public void SetNetWorkNumber(int nNumber)
        {
            PlcInstance.ActNetworkNumber = nNumber;
        }

        public void SetPassword(string strPassword)
        {
            PlcInstance.ActPassword = strPassword;
        }

        public void SetSourceNetworkNumber(int nNumber)
        {
            PlcInstance.ActSourceNetworkNumber = nNumber;
        }

        public void SetSourceStationNumber(int nNumber)
        {
            PlcInstance.ActSourceStationNumber = nNumber;
        }

        public void SetStationNumber(int nNumber)
        {
            PlcInstance.ActStationNumber = nNumber;
        }

        public void SetThroughNetworkType(int nType)
        {
            // MELSECNET/H only 0x00      include MELSECNTE/10 :0x01
            PlcInstance.ActThroughNetworkType = nType;
        }

        public void SetTimeOut(int nTime)
        {
            PlcInstance.ActTimeOut = nTime;
        }

        public void SetUnitNumber(int nNumber)
        {
            PlcInstance.ActUnitNumber = nNumber;
        }
        #endregion

        #region Open & Close.
        public int Open()
        {
            return PlcInstance.Open();
        }

        public int Close()
        {
            return PlcInstance.Close();
        }
        #endregion

        #region Get / Set Cpu Type.
        public int GetCpuType(out string szCpuName, ref int lplCpuType)
        {
            return PlcInstance.GetCpuType(out szCpuName, ref lplCpuType);
        }

        public void SetCpuType(int nCpuType)
        {
            PlcInstance.ActCpuType = nCpuType;
        }
        #endregion

        #region Get / Set Device.
        public int GetDeviceBlock(string szDevice, int nSize, out int lpdwData)
        {
            int val = PlcInstance.ReadDeviceBlock(szDevice, nSize, out lpdwData);
            //lpdwData = val;
            return lpdwData;
        }

        public int GetDevice(string szDeivce, out int lplData)
        {
            return PlcInstance.GetDevice(szDeivce, out lplData);
        }

        public int SetDevice(string szDeivce, int lData)
        {
            return PlcInstance.SetDevice(szDeivce, lData);
        }
        #endregion

        #region Read, Write, ReadBuffer, WriteBuffer.
        public int Read(string szDeivce, int nSize, out int lpdwData)
        {
            return PlcInstance.ReadDeviceBlock(szDeivce, nSize, out lpdwData);
        }

        public int Write(string szDeivce, int nSize, ref int lpdwData)
        {
            return PlcInstance.SetDevice(szDeivce, lpdwData);
            //return PlcInstance.WriteDeviceBlock(szDeivce, nSize, ref lpdwData);
        }

        public int ReadBuffer(int lStartIO, int lAddress, int lReadSize, out short lpwData)
        {
            return PlcInstance.ReadBuffer(lStartIO, lAddress, lReadSize, out lpwData);
        }

        public int WriteBuffer(int lStartIO, int lAddress, int lWriteSize, out short lpwData)
        {
            return PlcInstance.WriteBuffer(lStartIO, lAddress, lWriteSize, out lpwData);
        }
        #endregion
    }

    /// <summary>   Type : ActQNUDECPUTCPDev.  </summary>
    public class ActQNUDECPUTCPDev : IMitsubiPlc
    {
        /// <summary> The plc instance </summary>
        private static ActQNUDECPUTCP m_PlcInstance = null;
        
        /// <summary> The instance </summary>
        private static ActQNUDECPUTCPDev _Instance = null;

        /// <summary>   Gets the instance. </summary>
        public static ActQNUDECPUTCPDev Instance()
        {
            if (_Instance == null)
            {
                _Instance = new ActQNUDECPUTCPDev();
                m_PlcInstance = new ActQNUDECPUTCP();
            }

            return _Instance;
        }

        /// <summary>   Gets or sets the plc instance. </summary>
        /// <value> The plc instance. </value>
        public ActQNUDECPUTCP PlcInstance
        {
            get { return m_PlcInstance; }
            set { m_PlcInstance = value; }
        }

        /// <summary>   Connects a plc. </summary>
        /// <remarks>   suoow2, 2014-11-22. </remarks>
        public bool ConnectPLC(string ip, int port)
        {
            this.Close();
            this.SetCpuType(144);
            this.SetDestinationIONumber(0);
            this.SetDidPropertyBit(1);
            this.SetDsidPropertyBit(1);
            this.SetAddress(ip);
            this.SetIntelligentPreference(0);
            this.SetActIONumber(1023);
            this.SetMultiDropChannelNumer(0);
            this.SetNetWorkNumber(0);
            this.SetStationNumber(255);
            this.SetThroughNetworkType(0);
            this.SetTimeOut(10000);
            this.SetUnitNumber(0);

            if (this.Open() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Setters.
        public void SetAddress(String strAddress)
        {
            PlcInstance.ActHostAddress = strAddress;
        }

        public void SetIntelligentPreference(int nNumber)
        {
            PlcInstance.ActIntelligentPreferenceBit = nNumber;

        }
        
        public void SetDestinationIONumber(int nNumber)
        {
            //0x00
            PlcInstance.ActDestinationIONumber = nNumber;
        }

        public void SetDidPropertyBit(int nNumber)
        {
            //0x01
            PlcInstance.ActDidPropertyBit = nNumber;
        }

        public void SetDsidPropertyBit(int nNumber)
        {   
            //0x01
            PlcInstance.ActDsidPropertyBit = nNumber;
        }
        public void SetActIONumber(int nNumber)
        {
            //0x3FF
            PlcInstance.ActIONumber = nNumber;
        }

        public void SetMultiDropChannelNumer(int nNumber)
        {
            PlcInstance.ActMultiDropChannelNumber = nNumber;
        }

        public void SetNetWorkNumber(int nNumber)
        {
            PlcInstance.ActNetworkNumber = nNumber;
        }

        public void SetPassword(string strPassword)
        {
            PlcInstance.ActPassword = strPassword;
        }

        public void SetSourceNetworkNumber(int nNumber)
        {
            //PlcInstance.ActSourceNetworkNumber = nNumber;
        }

        public void SetSourceStationNumber(int nNumber)
        {
            //PlcInstance.ActSourceStationNumber = nNumber;
        }

        public void SetStationNumber(int nNumber)
        {
            PlcInstance.ActStationNumber = nNumber;
        }

        public void SetThroughNetworkType(int nType)
        {
            // MELSECNET/H only 0x00      include MELSECNTE/10 :0x01
            PlcInstance.ActThroughNetworkType = nType;
        }

        public void SetTimeOut(int nTime)
        {
            PlcInstance.ActTimeOut = nTime;
        }

        public void SetUnitNumber(int nNumber)
        {
            PlcInstance.ActUnitNumber = nNumber;
        }
        #endregion

        #region Open & Close.
        public int Open()
        {
            return PlcInstance.Open();
        }

        public int Close()
        {
            return PlcInstance.Close();
        }
        #endregion

        #region Get / Set Cpu Type.
        public int GetCpuType(out string szCpuName, out int lplCpuType)
        {
            return PlcInstance.GetCpuType(out szCpuName, out lplCpuType);
        }

        public void SetCpuType(int nCpuType)
        {
            PlcInstance.ActCpuType = nCpuType;
        }
        #endregion

        #region Get / Set Device.
        public int GetDeviceBlock(string szDevice, int nSize, out int lpdwData)
        {
            return PlcInstance.ReadDeviceBlock(szDevice, nSize, out lpdwData);
        }

        public int GetDevice(string szDeivce, out int lplData)
        {
            return PlcInstance.GetDevice(szDeivce, out lplData);
        }

        public int SetDevice(string szDeivce, int lData)
        {
            return PlcInstance.SetDevice(szDeivce, lData);
        }
        #endregion

        #region Read, Write, ReadBuffer, WriteBuffer.
        public int Read(string szDeivce, int nSize, out int lpdwData)
        {
            return PlcInstance.ReadDeviceBlock(szDeivce, nSize, out lpdwData);
        }

        public int Write(string szDeivce, int nSize, ref int lpdwData)
        {
            return PlcInstance.WriteDeviceBlock(szDeivce, nSize, ref lpdwData);
        }

        public int ReadBuffer(int lStartIO, int lAddress, int lReadSize, out short lpwData)
        {
            return PlcInstance.ReadBuffer(lStartIO, lAddress, lReadSize, out lpwData);
        }

       // public int WriteBuffer(int lStartIO, int lAddress, int lWriteSize, ref short lpwData)
       // {
       //     return PlcInstance.WriteBuffer(lStartIO, lAddress, lWriteSize, ref lpwData);
       // }
        public int WriteBuffer(int lStartIO, int lAddress, int lWriteSize, out short lpwData)
        {
            lpwData = 0;
            return 0;
        }
        #endregion
    }
}
