/*********************************************************************************
 * Copyright(c) 2015 by Haesung DS.
 * 
 * This software is copyrighted by, and is the sole property of Haesung DS.
 * All rigths, title, ownership, or other interests in the software remain the
 * property of Haesung DS. This software may only be used in accordance with
 * the corresponding license agreement. Any unauthorized use, duplication, 
 * transmission, distribution, or disclosure of this software is expressly 
 * forbidden.
 *
 * This Copyright notice may not be removed or modified without prior written
 * consent of Haesung DS reserves the right to modify this 
 * software without notice.
 *
 * Haesung DS.
 * KOREA 
 * http://www.HaesungDS.com
 *********************************************************************************/
/**
 * @file  SettingOption.cs
 * @brief
 *  Setting about Option node in Settings.xml file
 * 
 * @author : Cheol Min <suoow.yeo@haesung.net>
 * @date : 2011.05.25
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.05.25 First creation.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    /// <summary>   Setting sub system.  </summary>
    /// <remarks>   suoow, 2014-05-25. </remarks>
    public class SettingSubSystem
    {
        private readonly XmlSetting m_XmlSetting;
        public XmlSetting GetXmlSetting()
        {
            return m_XmlSetting;
        }

        private const int KEYPADCOUNT = 11;
        private const int MAX_CONNECTION = 50;

        public SettingSubSystem(XmlSetting aXmlSetting)
        {
            if (aXmlSetting == null)
            {
                aXmlSetting = new XmlSetting();
                Load();
            }

            m_XmlSetting = aXmlSetting;
        }

        #region Properties.
        // Database
        public String UseDB { get; set; }
        public String DBIP { get; set; }
        public String DBPort { get; set; }

        public string UseVRSDB { get; set; }
        public string VRSDBIP { get; set; }
        public string VRSDBPort { get; set; }
        public string VRSAVITableName { get; set; }

        public string UseITSDB { get; set; }
        public string ITSDBIP { get; set; }
        public string ITSDBPort { get; set; }

        public string UseITS { get; set; }
        public string ITSPath1 { get; set; }
        public string ITSPath2 { get; set; }
        public string ITSPath3 { get; set; }

        public string ITSTableName { get; set; }
        public String VRSBinCodeTableName { get; set; }
        public string VRSMapPath { get; set; }
        // RVS
        public String UseRVS { get; set; }
        public String RVSIP { get; set; }
        public String RVSPort { get; set; }

        // IS Type.
        public int ISType { get; set; }
        public int ISType2 { get; set; }
        public int CameraWidth { get; set; }
        public int CameraWidth2 { get; set; }
        public int CameraHeight { get; set; }
        public String DeviceName { get; set; }
        public String CamFile { get; set; }
        public String CamFileSlave { get; set; }
        public String DeviceName2 { get; set; }
        public String CamFile2 { get; set; }
        public int TestID { get; set; }

        // File Server.
        public String FileServerIP { get; set; }
        public String FileServerPort { get; set; }

        // RVS Setting.
        public int ConnectionCount { get; set; }
        public int InnerResolution { get; set; }
        public int CenterResolution { get; set; }
        public int OuterResolution { get; set; }

        // Algorithm Setting
        public int PSRShiftType { get; set; }

        #region Equip Name
        private string[] m_arrEquipName = new string[MAX_CONNECTION];

        public void SetEquipName(int anIndex, string anValue)
        {
            m_arrEquipName = EquipName;
            m_arrEquipName[anIndex] = anValue;
            m_XmlSetting.SetSettingString("SubSystem/RVS", "EquipName" + (anIndex + 1).ToString("D2"), m_arrEquipName[anIndex]);
        }
        public string[] EquipName
        {
            get
            {
                return m_arrEquipName;
            }
            set
            {
                m_arrEquipName = value;
                for (int i = 0; i < m_arrEquipName.Length; i++)
                    m_XmlSetting.SetSettingString("SubSystem/RVS", "EquipName" + (i + 1).ToString("D2"), m_arrEquipName[i]);
            }
        }
        #endregion

        #region Equip IP
        private string[] m_arrEquipIP = new string[MAX_CONNECTION];

        public void SetEquipIP(int anIndex, string anValue)
        {
            m_arrEquipIP = EquipIP;
            m_arrEquipIP[anIndex] = anValue;
            m_XmlSetting.SetSettingString("SubSystem/RVS", "EquipIP" + (anIndex + 1).ToString("D2"), m_arrEquipIP[anIndex]);
        }
        public string[] EquipIP
        {
            get
            {
                return m_arrEquipIP;
            }
            set
            {
                m_arrEquipIP = value;
                for (int i = 0; i < m_arrEquipIP.Length; i++)
                    m_XmlSetting.SetSettingString("SubSystem/RVS", "EquipIP" + (i + 1).ToString("D2"), m_arrEquipIP[i]);
            }
        }
        #endregion

        #region KeyPad Name
        private string[] m_arrKeyPadName = new string[KEYPADCOUNT];

        public void SetKeyPadName(int anIndex, string anValue)
        {
            m_arrKeyPadName = KeyPadName;
            m_arrKeyPadName[anIndex] = anValue;
            m_XmlSetting.SetSettingString("SubSystem/RVS", "KeyPadName" + (anIndex + 1).ToString("D2"), m_arrKeyPadName[anIndex]);
        }
        public string[] KeyPadName
        {
            get
            {
                return m_arrKeyPadName;
            }
            set
            {
                m_arrKeyPadName = value;
                for (int i = 0; i < m_arrKeyPadName.Length; i++)
                    m_XmlSetting.SetSettingString("SubSystem/RVS", "KeyPadName" + (i + 1).ToString("D2"), m_arrKeyPadName[i]);
            }
        }
        #endregion

        #region DefectCode
        private string[] m_arrDefectCode = new string[KEYPADCOUNT + 1];           // +1은 AutoNG 땜에 가상으로 하나 추가함

        public void SetDefectCode(int anIndex, string anValue)
        {
            m_arrDefectCode = DefectCode;
            m_arrDefectCode[anIndex] = anValue;
            m_XmlSetting.SetSettingString("SubSystem/RVS", "DefectCode" + (anIndex + 1).ToString("D2"), m_arrDefectCode[anIndex]);
        }
        public string[] DefectCode
        {
            get
            {
                return m_arrDefectCode;
            }
            set
            {
                m_arrDefectCode = value;
                for (int i = 0; i < m_arrDefectCode.Length; i++)
                    m_XmlSetting.SetSettingString("SubSystem/RVS", "DefectCode" + (i + 1).ToString("D2"), m_arrDefectCode[i]);
            }
        }
        #endregion
        #endregion

        #region Load & Save
        public void Load()
        {
            UseVRSDB = m_XmlSetting.GetSettingString("SubSystem/VRSDatabase", "Use", "1");
            VRSDBIP = m_XmlSetting.GetSettingString("SubSystem/VRSDatabase", "IP", "192.168.20.29");
            VRSDBPort = m_XmlSetting.GetSettingString("SubSystem/VRSDatabase", "Port", "3306");
            VRSAVITableName = m_XmlSetting.GetSettingString("SubSystem/VRSDatabase", "VRSAVITableName", "avi_lot_info");
            VRSBinCodeTableName = m_XmlSetting.GetSettingString("SubSystem/VRSDatabase", "VRSBinCodeTableName", "avi_code");
            VRSMapPath = m_XmlSetting.GetSettingString("SubSystem/VRSDatabase", "MapPath", "MapPath");

            UseITSDB = m_XmlSetting.GetSettingString("SubSystem/ITSDatabase", "Use", "1");
            ITSDBIP = m_XmlSetting.GetSettingString("SubSystem/ITSDatabase", "IP", "192.168.20.29");
            ITSDBPort = m_XmlSetting.GetSettingString("SubSystem/ITSDatabase", "Port", "3306");
            ITSTableName = m_XmlSetting.GetSettingString("SubSystem/ITSDatabase", "ITSTableName", "itsdb_bga");

            UseITS = m_XmlSetting.GetSettingString("SubSystem/ITS", "Use", "1");
            ITSPath1 = m_XmlSetting.GetSettingString("SubSystem/ITS", "Path1", "d:\\Skipdata\\");
            ITSPath2 = m_XmlSetting.GetSettingString("SubSystem/ITS", "Path2", "d:\\ITS\\");
            ITSPath3 = m_XmlSetting.GetSettingString("SubSystem/ITS", "Path3", "d:\\ITStemp\\");

            UseDB = m_XmlSetting.GetSettingString("SubSystem/Database", "Use", "1");
            DBIP = m_XmlSetting.GetSettingString("SubSystem/Database", "IP", "localhost");
            DBPort = m_XmlSetting.GetSettingString("SubSystem/Database", "Port", "3306");
            UseRVS = m_XmlSetting.GetSettingString("SubSystem/RVS", "UseRVS", "0");
            RVSIP = m_XmlSetting.GetSettingString("SubSystem/RVS", "IP", "localhost");
            RVSPort = m_XmlSetting.GetSettingString("SubSystem/RVS", "Port", "500006");
            ISType = m_XmlSetting.GetSettingInt("SubSystem/IS", "Type", "0");
            ISType2 = m_XmlSetting.GetSettingInt("SubSystem/IS", "Type2", "0");
            CameraWidth = m_XmlSetting.GetSettingInt("SubSystem/IS", "CameraWidth", "12000");
            CameraWidth2 = m_XmlSetting.GetSettingInt("SubSystem/IS", "CameraWidth2", "8192");
            CameraHeight = m_XmlSetting.GetSettingInt("SubSystem/IS", "CameraHeight", "20000");
            DeviceName = m_XmlSetting.GetSettingString("SubSystem/IS", "DeviceName", "System");
            CamFile = m_XmlSetting.GetSettingString("SubSystem/IS", "CamFile", "C:\\");
            CamFileSlave = m_XmlSetting.GetSettingString("SubSystem/IS", "CamFileSlave", "C:\\");
            DeviceName2 = m_XmlSetting.GetSettingString("SubSystem/IS", "DeviceName2", "System");
            CamFile2 = m_XmlSetting.GetSettingString("SubSystem/IS", "CamFile2", "C:\\");
            TestID = m_XmlSetting.GetSettingInt("SubSystem/IS", "TestID", "0");
            FileServerIP = m_XmlSetting.GetSettingString("SubSystem/FileServer", "IP", "localhost");
            FileServerPort = m_XmlSetting.GetSettingString("SubSystem/FileServer", "Port", "500005");
            ConnectionCount = m_XmlSetting.GetSettingInt("SubSystem/RVS", "ConnectionCount", "10");
            InnerResolution = m_XmlSetting.GetSettingInt("SubSystem/RVS", "InnerResolution", "100");
            CenterResolution = m_XmlSetting.GetSettingInt("SubSystem/RVS", "CenterResolution", "200");
            OuterResolution = m_XmlSetting.GetSettingInt("SubSystem/RVS", "OuterResolution", "300");

            for (int i = 0; i < m_arrEquipName.Length; i++)
                m_arrEquipName[i] = m_XmlSetting.GetSettingString("SubSystem/RVS", "EquipName" + (i + 1).ToString("D2"), "Equip" + (i + 1).ToString("D2"));
            for (int i = 0; i < m_arrEquipIP.Length; i++)
                m_arrEquipIP[i] = m_XmlSetting.GetSettingString("SubSystem/RVS", "EquipIP" + (i + 1).ToString("D2"), "127.0.0.1");
            for (int i = 0; i < m_arrKeyPadName.Length; i++)
                m_arrKeyPadName[i] = m_XmlSetting.GetSettingString("SubSystem/RVS", "KeyPadName" + (i + 1).ToString("D2"), "No." + (i + 1).ToString("D2"));
            for (int i = 0; i < m_arrDefectCode.Length; i++)
                m_arrDefectCode[i] = m_XmlSetting.GetSettingString("SubSystem/RVS", "DefectCode" + (i + 1).ToString("D2"), "No." + (i + 1).ToString("D2"));

            PSRShiftType = m_XmlSetting.GetSettingInt("SubSystem/Algorithm", "PSRShiftType", "0");
        }

        public void Save()
        {
            m_XmlSetting.SetSettingString("SubSystem/VRSDatabase", "Use", UseVRSDB);
            m_XmlSetting.SetSettingString("SubSystem/VRSDatabase", "IP", VRSDBIP);
            m_XmlSetting.SetSettingString("SubSystem/VRSDatabase", "Port", VRSDBPort);

            m_XmlSetting.SetSettingString("SubSystem/VRSDatabase", "VRSAVITableName", VRSAVITableName);
            m_XmlSetting.SetSettingString("SubSystem/VRSDatabase", "VRSBinCodeTableName", VRSBinCodeTableName);
            m_XmlSetting.SetSettingString("SubSystem/VRSDatabase", "MapPath", VRSMapPath);

            m_XmlSetting.SetSettingString("SubSystem/ITSDatabase", "Use", UseITSDB);
            m_XmlSetting.SetSettingString("SubSystem/ITSDatabase", "IP", ITSDBIP);
            m_XmlSetting.SetSettingString("SubSystem/ITSDatabase", "Port", ITSDBPort);
            m_XmlSetting.SetSettingString("SubSystem/ITSDatabase", "ITSTableName", ITSTableName);

            m_XmlSetting.SetSettingString("SubSystem/ITS", "Use", UseITS);
            m_XmlSetting.SetSettingString("SubSystem/ITS", "Path1", ITSPath1);
            m_XmlSetting.SetSettingString("SubSystem/ITS", "Path2", ITSPath2);
            m_XmlSetting.SetSettingString("SubSystem/ITS", "Path3", ITSPath3);

            m_XmlSetting.SetSettingString("SubSystem/Database", "Use", UseDB);
            m_XmlSetting.SetSettingString("SubSystem/Database", "IP", DBIP);
            m_XmlSetting.SetSettingString("SubSystem/Database", "Port", DBPort);
            m_XmlSetting.SetSettingString("SubSystem/RVS", "UseRVS", UseRVS);
            m_XmlSetting.SetSettingString("SubSystem/RVS", "IP", RVSIP);
            m_XmlSetting.SetSettingString("SubSystem/RVS", "Port", RVSPort);
            m_XmlSetting.SetSettingInt("SubSystem/IS", "Type", ISType);
            m_XmlSetting.SetSettingInt("SubSystem/IS", "Type2", ISType2);
            m_XmlSetting.SetSettingInt("SubSystem/IS", "CameraWidth", CameraWidth);
            m_XmlSetting.SetSettingInt("SubSystem/IS", "CameraWidth2", CameraWidth2);
            m_XmlSetting.SetSettingInt("SubSystem/IS", "CameraHeight", CameraHeight);
            m_XmlSetting.SetSettingString("SubSystem/IS", "DeviceName", DeviceName);
            m_XmlSetting.SetSettingString("SubSystem/IS", "CamFile", CamFile);
            m_XmlSetting.SetSettingString("SubSystem/IS", "CamFileSlave", CamFileSlave);
            m_XmlSetting.SetSettingString("SubSystem/IS", "DeviceName2", DeviceName2);
            m_XmlSetting.SetSettingString("SubSystem/IS", "CamFile2", CamFile2);
            m_XmlSetting.SetSettingInt("SubSystem/IS", "TestID", TestID);
            m_XmlSetting.SetSettingString("SubSystem/FileServer", "IP", FileServerIP);
            m_XmlSetting.SetSettingString("SubSystem/FileServer", "Port", FileServerPort);
            m_XmlSetting.SetSettingInt("SubSystem/RVS", "ConnectionCount", ConnectionCount);
            m_XmlSetting.SetSettingInt("SubSystem/RVS", "InnerResolution", InnerResolution);
            m_XmlSetting.SetSettingInt("SubSystem/RVS", "CenterResolution", CenterResolution);
            m_XmlSetting.SetSettingInt("SubSystem/RVS", "OuterResolution", OuterResolution);
            m_XmlSetting.SetSettingInt("SubSystem/Algorithm", "PSRShiftType", PSRShiftType);
        }
        #endregion
    }
}
