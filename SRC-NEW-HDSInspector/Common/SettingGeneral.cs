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
 * @author : suoow <suoow.yeo@haesung.net>
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
    /// <summary>   Setting general.  </summary>
    /// <remarks>   suoow, 2014-05-25. </remarks>
    public class SettingGeneral
    {
        private readonly XmlSetting m_XmlSetting;
        public XmlSetting GetXmlSetting()
        {
            return m_XmlSetting;
        }

        public SettingGeneral(XmlSetting aXmlSetting)
        {
            if (aXmlSetting == null)
            {
                aXmlSetting = new XmlSetting();
                Load();
            }

            m_XmlSetting = aXmlSetting;
        }

        public void LoadMarkDefaultPos()
        {
            LaserBoat1 = m_XmlSetting.GetSettingDouble("General", "LaserBoat1", "0.00");
            LaserBoat2 = m_XmlSetting.GetSettingDouble("General", "LaserBoat2", "0.00");
            LaserCam = m_XmlSetting.GetSettingDouble("General", "LaserCam", "0.00");
        }

        public void Load()
        {
            MachineType = m_XmlSetting.GetSettingString("General", "MachineType", "BGA_AVI");
            MachineIP = m_XmlSetting.GetSettingString("General", "IP", "192.168.30.160");
            MachinePort = m_XmlSetting.GetSettingString("General", "Port", "50002");
            MachineCode = m_XmlSetting.GetSettingString("General", "MachineCode", "0001");
            MachineName = m_XmlSetting.GetSettingString("General", "MachineName", "CAI01");
            V3Reverse = m_XmlSetting.GetSettingBool("General", "V3Reverse", "0");
            ImageSaveAll = m_XmlSetting.GetSettingBool("General", "ImageSaveAll", "0");
            SaveDefectData = m_XmlSetting.GetSettingLong("General", "SaveDefectData", "1");
            TestInspectMode = m_XmlSetting.GetSettingBool("General", "SetupMode", "1");
            ScanVelocity1 = m_XmlSetting.GetSettingInt("General", "ScanVelocity1", "100");
            ScanVelocity2 = m_XmlSetting.GetSettingInt("General", "ScanVelocity2", "100");
            Customer = m_XmlSetting.GetSettingInt("General", "Customer", "1");
            UsePassword = m_XmlSetting.GetSettingBool("General", "UsePassword", "0");


            IDMarkPath = m_XmlSetting.GetSettingString("General", "IDMarkPath", @"D:\\IDMarkMap\\");
            ResultPath = m_XmlSetting.GetSettingString("General", "ResultPath", @"D:\\ResultPath\\");
            XMLMapPath = m_XmlSetting.GetSettingString("General", "XMLMapPath", @"D:\\XMLMapPath\\");
            ModelPath = m_XmlSetting.GetSettingString("General", "ModelPath", @"D:\\ModelPath\\");
            RejectRate = m_XmlSetting.GetSettingDouble("General", "Reject_Rate", "10");
            RejectValue = m_XmlSetting.GetSettingDouble("General", "Reject_Value", "10");
            AutoNG = m_XmlSetting.GetSettingString("General", "AutoNG", "0,0,0,0,0,0,0,0,0,0,0");
            LaserEnable = m_XmlSetting.GetSettingBool("General", "LaserEnable", "1");
            m_VisionIP[0] = m_XmlSetting.GetSettingString("General", "VisionIP1", "127.0.0.1");
            m_VisionIP[1] = m_XmlSetting.GetSettingString("General", "VisionIP2", "127.0.0.1");
            m_VisionIP[2] = m_XmlSetting.GetSettingString("General", "VisionIP3", "127.0.0.1");
            m_VisionIP[3] = m_XmlSetting.GetSettingString("General", "VisionIP4", "127.0.0.1");
            m_VisionIP[4] = m_XmlSetting.GetSettingString("General", "VisionIP5", "127.0.0.1");
            m_VisionPort[0] = m_XmlSetting.GetSettingInt("General", "VisionPort1", "15000");
            m_VisionPort[1] = m_XmlSetting.GetSettingInt("General", "VisionPort2", "15000");
            m_VisionPort[2] = m_XmlSetting.GetSettingInt("General", "VisionPort3", "15000");
            m_VisionPort[3] = m_XmlSetting.GetSettingInt("General", "VisionPort4", "15000");
            m_VisionPort[4] = m_XmlSetting.GetSettingInt("General", "VisionPort5", "15000");
            m_ResolutionX[0] = m_XmlSetting.GetSettingDouble("General", "ResolutionX1", "12");
            m_ResolutionX[1] = m_XmlSetting.GetSettingDouble("General", "ResolutionX2", "12");
            m_ResolutionX[2] = m_XmlSetting.GetSettingDouble("General", "ResolutionX3", "12");
            m_ResolutionY[0] = m_XmlSetting.GetSettingDouble("General", "ResolutionY1", "12");
            m_ResolutionY[1] = m_XmlSetting.GetSettingDouble("General", "ResolutionY2", "12");
            m_ResolutionY[2] = m_XmlSetting.GetSettingDouble("General", "ResolutionY3", "12");
            m_PageDelay[0] = m_XmlSetting.GetSettingInt("General", "PageDelay1", "100");
            m_PageDelay[1] = m_XmlSetting.GetSettingInt("General", "PageDelay2", "100");
            m_PageDelay[2] = m_XmlSetting.GetSettingInt("General", "PageDelay3", "100");
            AlignResolution = m_XmlSetting.GetSettingDouble("General", "AlignResolution", "9.5");
            Laser2OffsetY = m_XmlSetting.GetSettingDouble("General", "Laser2Offset", "0.003");
            Laser2OffsetX = m_XmlSetting.GetSettingDouble("General", "Laser2OffsetX", "0.02");
            Laser2Angle = m_XmlSetting.GetSettingDouble("General", "Laser2Angle", "0.00");
            LaserBoat1 = m_XmlSetting.GetSettingDouble("General", "LaserBoat1", "0.00");
            LaserBoat2 = m_XmlSetting.GetSettingDouble("General", "LaserBoat2", "0.00");
            LaserCam = m_XmlSetting.GetSettingDouble("General", "LaserCam", "0.00");
            LastSelectedGroup = m_XmlSetting.GetSettingInt("General", "LastSelectedGroup", "-1");
            LastSelectedModel = m_XmlSetting.GetSettingString("General", "LastSelectedModel", "");
            IsUseRejectRate = m_XmlSetting.GetSettingInt("General", "IsUseRejectRate", "1");
            LastLot = m_XmlSetting.GetSettingString("General", "LastLot", "Lot");
            LastUser = m_XmlSetting.GetSettingString("General", "LastUser", "DS");
            LastInspect = m_XmlSetting.GetSettingString("General", "LastInspect", "1,1,1,1");
            LastVerify = m_XmlSetting.GetSettingString("General", "LastVerify", "0");
            SAPPath = m_XmlSetting.GetSettingString("General", "SAPPath", @"D:\\loss\\");
            SaveFailLoss = m_XmlSetting.GetSettingBool("General", "SaveFailLoss", "0");
            ProcessCode = m_XmlSetting.GetSettingString("General", "ProcessCode", "VI21");
            PLCResultType = m_XmlSetting.GetSettingInt("General", "PLCResultType", "0");
            ContinueSize = m_XmlSetting.GetSettingInt("General", "ContinueSize", "8");
            Cam1Position = m_XmlSetting.GetSettingDouble("General", "Cam1Position", "60");
            Cam1Pitch = m_XmlSetting.GetSettingDouble("General", "Cam1Pitch", "40");
            VerifyInfoPath = m_XmlSetting.GetSettingString("General", "VerifyInfoPath", @"D:\\ModelPath\\");
            UnitBadCount = m_XmlSetting.GetSettingInt("General", "UnitBadCount", "5");
            BA_R_Gain = (float)m_XmlSetting.GetSettingDouble("SubSystem/IS", "RGain1", "1.0");
            BA_G_Gain = (float)m_XmlSetting.GetSettingDouble("SubSystem/IS", "GGain1", "1.0");
            BA_B_Gain = (float)m_XmlSetting.GetSettingDouble("SubSystem/IS", "BGain1", "1.0");
            CA_R_Gain = (float)m_XmlSetting.GetSettingDouble("SubSystem/IS", "RGain2", "1.0");
            CA_G_Gain = (float)m_XmlSetting.GetSettingDouble("SubSystem/IS", "GGain2", "1.0");
            CA_B_Gain = (float)m_XmlSetting.GetSettingDouble("SubSystem/IS", "BGain2", "1.0");
            Strenth = (float)m_XmlSetting.GetSettingDouble("SubSystem/IS", "SRGain", "1.0");
        }

        public void SaveBoat2Offset()
        {
            m_XmlSetting.SetSettingDouble("General", "Laser2Offset", Laser2OffsetY);
            m_XmlSetting.SetSettingDouble("General", "Laser2OffsetX", Laser2OffsetX);
            m_XmlSetting.SetSettingDouble("General", "Laser2Angle", Laser2Angle);
        }

        public void SaveMarkDefaultPos()
        {
            m_XmlSetting.SetSettingDouble("General", "LaserBoat1", LaserBoat1);
            m_XmlSetting.SetSettingDouble("General", "LaserBoat2", LaserBoat2);
            m_XmlSetting.SetSettingDouble("General", "LaserCam", LaserCam);
        }

        public void SaveRGBGain()
        {
            m_XmlSetting.SetSettingDouble("SubSystem/IS", "RGain1", BA_R_Gain);
            m_XmlSetting.SetSettingDouble("SubSystem/IS", "GGain1", BA_G_Gain);
            m_XmlSetting.SetSettingDouble("SubSystem/IS", "BGain1", BA_B_Gain);
            m_XmlSetting.SetSettingDouble("SubSystem/IS", "RGain2", CA_R_Gain);
            m_XmlSetting.SetSettingDouble("SubSystem/IS", "GGain2", CA_G_Gain);
            m_XmlSetting.SetSettingDouble("SubSystem/IS", "BGain2", CA_B_Gain);
            m_XmlSetting.SetSettingDouble("SubSystem/IS", "SRGain", Strenth);
        }

        public void Save()
        {
            m_XmlSetting.SetSettingString("General", "MachineType", MachineType);
            m_XmlSetting.SetSettingString("General", "IP", MachineIP);
            m_XmlSetting.SetSettingString("General", "Port", MachinePort);
            m_XmlSetting.SetSettingString("General", "MachineCode", MachineCode);
            m_XmlSetting.SetSettingString("General", "MachineName", MachineName);
            m_XmlSetting.SetSettingLong("General", "SaveDefectData", SaveDefectData);
            m_XmlSetting.SetSettingBool("General", "V3Reverse", V3Reverse);
            m_XmlSetting.SetSettingBool("General", "SetupMode", TestInspectMode);
            m_XmlSetting.SetSettingBool("General", "ImageSaveAll", ImageSaveAll);
            m_XmlSetting.SetSettingInt("General", "ScanVelocity1", ScanVelocity1);
            m_XmlSetting.SetSettingInt("General", "ScanVelocity2", ScanVelocity2);
            m_XmlSetting.SetSettingInt("General", "Customer", Customer);
            m_XmlSetting.SetSettingBool("General", "UsePassword", UsePassword);
            m_XmlSetting.SetSettingString("General", "IDMarkPath", IDMarkPath);
            m_XmlSetting.SetSettingString("General", "ResultPath", ResultPath);
            m_XmlSetting.SetSettingString("General", "XMLMapPath", XMLMapPath);
            m_XmlSetting.SetSettingString("General", "ModelPath", ModelPath);
            m_XmlSetting.SetSettingDouble("General", "Reject_Rate", RejectRate);
            m_XmlSetting.SetSettingDouble("General", "Reject_Value", RejectValue);
            m_XmlSetting.SetSettingString("General", "AutoNG", AutoNG);
            m_XmlSetting.SetSettingBool("General", "LaserEnable", LaserEnable);
            m_XmlSetting.SetSettingString("General", "VisionIP1", m_VisionIP[0]);
            m_XmlSetting.SetSettingString("General", "VisionIP2", m_VisionIP[1]);
            m_XmlSetting.SetSettingString("General", "VisionIP3", m_VisionIP[2]);
            m_XmlSetting.SetSettingString("General", "VisionIP4", m_VisionIP[3]);
            m_XmlSetting.SetSettingString("General", "VisionIP5", m_VisionIP[4]);
            m_XmlSetting.SetSettingInt("General", "VisionPort1", m_VisionPort[0]);
            m_XmlSetting.SetSettingInt("General", "VisionPort2", m_VisionPort[1]);
            m_XmlSetting.SetSettingInt("General", "VisionPort3", m_VisionPort[2]);
            m_XmlSetting.SetSettingInt("General", "VisionPort4", m_VisionPort[3]);
            m_XmlSetting.SetSettingInt("General", "VisionPort5", m_VisionPort[4]);
            m_XmlSetting.SetSettingDouble("General", "ResolutionX1", m_ResolutionX[0]);
            m_XmlSetting.SetSettingDouble("General", "ResolutionX2", m_ResolutionX[1]);
            m_XmlSetting.SetSettingDouble("General", "ResolutionX3", m_ResolutionX[2]);
            m_XmlSetting.SetSettingDouble("General", "ResolutionY1", m_ResolutionY[0]);
            m_XmlSetting.SetSettingDouble("General", "ResolutionY2", m_ResolutionY[1]);
            m_XmlSetting.SetSettingDouble("General", "ResolutionY3", m_ResolutionY[2]);
            m_XmlSetting.SetSettingInt("General", "PageDelay1", m_PageDelay[0]);
            m_XmlSetting.SetSettingInt("General", "PageDelay2", m_PageDelay[1]);
            m_XmlSetting.SetSettingInt("General", "PageDelay3", m_PageDelay[2]);
            m_XmlSetting.SetSettingDouble("General", "AlignResolution", AlignResolution);
            m_XmlSetting.SetSettingDouble("General", "Laser2Offset", Laser2OffsetY);
            m_XmlSetting.SetSettingDouble("General", "Laser2OffsetX", Laser2OffsetX);
            m_XmlSetting.SetSettingDouble("General", "Laser2Angle", Laser2Angle);
            m_XmlSetting.SetSettingDouble("General", "LaserBoat1", LaserBoat1);
            m_XmlSetting.SetSettingDouble("General", "LaserBoat2", LaserBoat2);
            m_XmlSetting.SetSettingDouble("General", "LaserCam", LaserCam);
            m_XmlSetting.SetSettingInt("General", "LastSelectedGroup", LastSelectedGroup);
            m_XmlSetting.SetSettingString("General", "LastSelectedModel", LastSelectedModel);
            m_XmlSetting.SetSettingInt("General", "IsUseRejectRate", IsUseRejectRate);
            m_XmlSetting.SetSettingString("General", "LastLot", LastLot);
            m_XmlSetting.SetSettingString("General", "LastUser", LastUser);
            m_XmlSetting.SetSettingString("General", "LastInspect", LastInspect);
            m_XmlSetting.SetSettingString("General", "LastVerify", LastVerify);
            m_XmlSetting.SetSettingString("General", "SAPPath", SAPPath);
            m_XmlSetting.SetSettingBool("General", "SaveFailLoss", SaveFailLoss);
            m_XmlSetting.SetSettingString("General", "ProcessCode", ProcessCode);
            m_XmlSetting.SetSettingInt("General", "PLCResultType", PLCResultType);
            m_XmlSetting.SetSettingInt("General", "ContinueSize", ContinueSize);
            m_XmlSetting.SetSettingInt("General", "UnitBadCount", UnitBadCount);
            m_XmlSetting.SetSettingDouble("General", " Cam1Position", Cam1Position);
            m_XmlSetting.SetSettingDouble("General", " Cam1Pitch", Cam1Pitch);
            m_XmlSetting.SetSettingString("General", "VerifyInfoPath", VerifyInfoPath);
            m_XmlSetting.SetSettingDouble("SubSystem/IS", "RGain1", BA_R_Gain);
            m_XmlSetting.SetSettingDouble("SubSystem/IS", "GGain1", BA_G_Gain);
            m_XmlSetting.SetSettingDouble("SubSystem/IS", "BGain1", BA_B_Gain);
            m_XmlSetting.SetSettingDouble("SubSystem/IS", "RGain2", CA_R_Gain);
            m_XmlSetting.SetSettingDouble("SubSystem/IS", "GGain2", CA_G_Gain);
            m_XmlSetting.SetSettingDouble("SubSystem/IS", "BGain2", CA_B_Gain);
            m_XmlSetting.SetSettingDouble("SubSystem/IS", "SRGain", Strenth);
        }

        #region Properties.
        public String MachineType { get; set; }
        public String MachineIP { get; set; }
        public String MachinePort { get; set; }
        public String MachineCode { get; set; }
        public String MachineName { get; set; }
        public String ModelPath { get; set; }
        public String ResultPath { get; set; }
        public String XMLMapPath { get; set; }
        public String SAPPath { get; set; }
        public String VerifyInfoPath { get; set; }
        public String ProcessCode { get; set; }
        public String AutoNG { get; set; }
        public String IDMarkPath { get; set; }
        public int Customer { get; set; }
        public bool UsePassword { get; set; }
        public bool SaveFailLoss { get; set; }
        public bool V3Reverse { get; set; }
        public long SaveDefectData { get; set; }
        public bool TestInspectMode { get; set; }
        public bool ImageSaveAll { get; set; }
        public int ScanVelocity1 { get; set; }
        public int ScanVelocity2 { get; set; }
        public int ContinueSize { get; set; }

        public int IsUseRejectRate { get; set; }
        public double RejectRate { get; set; }
        public double RejectValue { get; set; }

        public double Cam1Position { get; set; }
        public double Cam1Pitch { get; set; }

        public int UnitBadCount { get; set; }

        public string[] VisionIP
        {
            get { return m_VisionIP; }
            set { m_VisionIP = value; }
        }

        public int[] VisionPort
        {
            get { return m_VisionPort; }
            set { m_VisionPort = value; }
        }

        public double[] ResolutionX
        {
            get { return m_ResolutionX; }
            set { m_ResolutionX = value; }
        }

        public double[] ResolutionY
        {
            get { return m_ResolutionY; }
            set { m_ResolutionY = value; }
        }

        public int PLCResultType;

        public int[] PageDealy
        {
            get { return m_PageDelay; }
            set { m_PageDelay = value; }
        }
        public bool LaserEnable { get; set; }
        public double AlignResolution { get; set; }
        public double Laser2OffsetY { get; set; }
        public double Laser2OffsetX { get; set; }
        public double Laser2Angle { get; set; }

        public int LastSelectedGroup { get; set; }
        public string LastSelectedModel { get; set; }
        public string LastLot { get; set; }
        public string LastUser { get; set; }
        public string LastInspect { get; set; }
        public string LastVerify { get; set; }
        public double LaserBoat1 { get; set; }
        public double LaserBoat2 { get; set; }
        public double LaserCam { get; set; }
        public float BA_R_Gain { get; set; }
        public float BA_G_Gain { get; set; }
        public float BA_B_Gain { get; set; }
        public float CA_R_Gain { get; set; }
        public float CA_G_Gain { get; set; }
        public float CA_B_Gain { get; set; }
        public float Strenth { get; set; }
        #endregion

        #region Private member variables.
        private double[] m_ResolutionX = new double[3];
        private double[] m_ResolutionY = new double[3];
        private string[] m_VisionIP = new string[5];
        private int[] m_VisionPort = new int[5];
        private int[] m_PageDelay = new int[3];
        #endregion
    }
}
