using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Common
{
    public class Setting
    {
        private string m_Path = "";
        private string m_GeneralPath;
        private string m_DevicePath;
        private string m_JobPath;
        public Generals General;
        public SubSystems SubSystem;
        public Jobs Job;
        public Setting(string astrPath)
        {
            m_Path = astrPath;
            m_GeneralPath = m_Path + "\\Setting.ini";
            m_DevicePath = m_Path + "\\SubSystem.ini";
            m_JobPath = m_Path + "\\Job.ini";
            General = new Generals(m_GeneralPath);
            SubSystem = new SubSystems(m_DevicePath);
            Job = new Jobs(m_JobPath);
        }

        public bool Exists()
        {
            if (File.Exists(m_GeneralPath)) return true;
            return false;
        }

        public int Load()
        {
            int nRet = 0;
            bool bCreate = false;
            DateTime dt = DateTime.Now;
            string dir = string.Format("{0}\\Setting_Backup\\{1}-{2}", m_Path, dt.Year.ToString(), dt.Month.ToString());
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                bCreate = true;
            }

            if (!General.Load())
            {
                nRet += 1;
            }
            else
            {
                if(bCreate)
                {
                    File.Copy(m_DevicePath, dir + "\\SubSystem.ini");
                }
            }
            if(!SubSystem.Load())
            {
                nRet += 2;
            }
            else
            {
                if (bCreate)
                {
                    File.Copy(m_JobPath, dir + "\\Job.ini");
                }
            }
            if (!Job.Load())
            {
                nRet += 4;
            }
            else
            {
                if (bCreate)
                {
                    File.Copy(m_GeneralPath, dir + "\\Setting.ini");
                }
            }
            return nRet;
        }

        public void Save()
        {
            General.Save();
            SubSystem.Save();
            Job.Save();
        }

        public void SettingConversion()
        {
            Settings settings = Settings.GetSettings();
            settings.Load();
            #region General
            General.MachineCode = settings.General.MachineCode;
            General.MachineName = settings.General.MachineName;

            if (General.MachineName == "CAI04")
                General.MachineName = "BAV01";
            else if (General.MachineName == "CAI06")
                General.MachineName = "BAV03";
            else if (General.MachineName == "CAI08")
                General.MachineName = "BAV05";
            else if (General.MachineName == "CAI09")
                General.MachineName = "BAV06";
            else if (General.MachineName == "CAI17")
                General.MachineName = "BAV11";
            else if (General.MachineName == "CAI18")
                General.MachineName = "BAV12";
            else if (General.MachineName == "CAI19")
                General.MachineName = "BAV13";
            ////

            if (General.MachineName == "BAV04" || General.MachineName == "BAV07" || General.MachineName == "BAV08" || General.MachineName == "BAV20" || General.MachineName == "BAV21" || General.MachineName == "BAV22" || General.MachineName == "BAV23" || General.MachineName == "BAV24")    //Type이 다른 거는 정리해서 넣어 주자.
                General.MachineType = 1;
            else General.MachineType = 0;
            General.MachineIP = settings.General.MachineIP;
            General.ModelPath = settings.General.ModelPath;
            General.ResultPath = settings.General.ResultPath;
            General.XMLMapPath = settings.General.XMLMapPath;
            General.POP_Path = settings.General.SAPPath;
            General.VerifyInfoPath = settings.General.VerifyInfoPath;
            General.IDMarkPath = settings.General.IDMarkPath;

            General.UsePassword = settings.General.UsePassword;
            General.RejectRate = settings.General.RejectRate;
            General.VRSNGUnitLimit = settings.General.UnitBadCount;
            if (General.MachineName == "BAV04")
                General.UseIDReader = true;
            else General.UseIDReader = false;
            General.IDReaderIP = "127.0.0.1";
            General.UseDB = settings.SubSystem.UseDB =="0" ? false: true;
            General.DBIP = settings.SubSystem.DBIP;
            General.DBPort = settings.SubSystem.DBPort;

            General.UseRVS = settings.SubSystem.UseRVS == "0" ? false : true;
            General.RVSIP = settings.SubSystem.RVSIP;
            General.RVSPort = settings.SubSystem.RVSPort;

            General.UseVRSDB = settings.SubSystem.UseVRSDB == "0" ? false : true;
            General.VRSDBIP = settings.SubSystem.VRSDBIP;
            General.VRSDBPort = settings.SubSystem.VRSDBPort;
            General.VRSAVITableName = settings.SubSystem.VRSAVITableName;
            General.VRSBinCodeTableName = settings.SubSystem.VRSBinCodeTableName;

            General.UseITS = settings.SubSystem.UseITS == "0" ? false : true;
            General.ITSPath1 = settings.SubSystem.ITSPath1;
            General.ITSPath2 = settings.SubSystem.ITSPath2;
            General.ITSPath3 = settings.SubSystem.ITSPath3;

            General.UseITSDB = settings.SubSystem.UseITSDB == "0" ? false : true;
            General.ITSDBIP = settings.SubSystem.ITSDBIP;
            General.ITSDBPort = settings.SubSystem.ITSDBPort;
            General.ITSTableName = settings.SubSystem.ITSTableName;
            General.LogSave = settings.Log.LocalSave == 0 ? false: true;
            General.LogLevel = (int)settings.Log.LocalSaveLevel;
            General.LogDPLevel = (int)settings.Log.UIDisplayLevel;
            General.LogKeepDate = (int)settings.Log.KeepDate;

            General.UsePOP = false;
            General.UseXML = false;
            General.POP_IP = "127.0.0.1";
            General.POP_BK_IP = "127.0.0.1";
            General.POP_Port = "5000";
            General.POP_Delay_Second = 300;
            General.MaxLimitDefect = 1024;
            #endregion
            #region SubSystems
            #region IS
            switch (General.MachineName)
            {
                case "BAV01":
                case "BAV05":
                    SubSystem.IS.UseCASlave = false;
                    SubSystem.IS.UseBASlave = true;
                    break;
                case "BAV04":
                case "BAV07":
                case "BAV08":
                case "BAV19":
                case "BAV20":
                    SubSystem.IS.UseCASlave = true;
                    SubSystem.IS.UseBASlave = true;
                    break;
                case "BAV03":
                case "BAV11":
                case "BAV12":
                case "BAV13":
                    SubSystem.IS.UseCASlave = false;
                    SubSystem.IS.UseBASlave = false;
                    break;
            }

            SubSystem.IS.ReScale = 2.0;
            SubSystem.IS.CameraHeight[0] = settings.SubSystem.CameraHeight;
            SubSystem.IS.CameraHeight[1] = settings.SubSystem.CameraHeight;
            SubSystem.IS.CameraHeight[2] = settings.SubSystem.CameraHeight;
            SubSystem.IS.VisionFlipX[0] = false;
            SubSystem.IS.VisionFlipX[1] = false;
            SubSystem.IS.VisionFlipX[2] = false;
            if (General.MachineName == "BAV04" || General.MachineName == "BAV19" || General.MachineName == "BAV20")
            {
                SubSystem.IS.CameraWidth[0] = settings.SubSystem.CameraWidth2;
                SubSystem.IS.CameraWidth[1] = settings.SubSystem.CameraWidth;
                SubSystem.IS.CameraWidth[2] = settings.SubSystem.CameraWidth;

                SubSystem.IS.CamResolutionX[0] = settings.General.ResolutionX[1];
                SubSystem.IS.CamResolutionX[1] = settings.General.ResolutionX[0];
                SubSystem.IS.CamResolutionX[2] = settings.General.ResolutionX[2];
                SubSystem.IS.CamResolutionY[0] = settings.General.ResolutionY[1];
                SubSystem.IS.CamResolutionY[1] = settings.General.ResolutionY[0];
                SubSystem.IS.CamResolutionY[2] = settings.General.ResolutionY[2];
                SubSystem.IS.CamPageDelay[0] = settings.General.PageDealy[1];
                SubSystem.IS.CamPageDelay[1] = settings.General.PageDealy[0];
                SubSystem.IS.CamPageDelay[2] = settings.General.PageDealy[2];
            }
            else
            {
                SubSystem.IS.CameraWidth[0] = settings.SubSystem.CameraWidth;
                SubSystem.IS.CameraWidth[1] = settings.SubSystem.CameraWidth2;
                SubSystem.IS.CameraWidth[2] = settings.SubSystem.CameraWidth2;

                SubSystem.IS.CamResolutionX[0] = settings.General.ResolutionX[0];
                SubSystem.IS.CamResolutionX[1] = settings.General.ResolutionX[1];
                SubSystem.IS.CamResolutionX[2] = settings.General.ResolutionX[2];
                SubSystem.IS.CamResolutionY[0] = settings.General.ResolutionY[0];
                SubSystem.IS.CamResolutionY[1] = settings.General.ResolutionY[1];
                SubSystem.IS.CamResolutionY[2] = settings.General.ResolutionY[2];
                SubSystem.IS.CamPageDelay[0] = settings.General.PageDealy[0];
                SubSystem.IS.CamPageDelay[1] = settings.General.PageDealy[1];
                SubSystem.IS.CamPageDelay[2] = settings.General.PageDealy[2];
            }
            SubSystem.IS.R_Gain[0] = 1;
            SubSystem.IS.G_Gain[0] = 1;
            SubSystem.IS.B_Gain[0] = 1;
            SubSystem.IS.R_Gain[1] = settings.General.CA_R_Gain;
            SubSystem.IS.G_Gain[1] = settings.General.CA_G_Gain;
            SubSystem.IS.B_Gain[1] = settings.General.CA_B_Gain;
            SubSystem.IS.R_Gain[2] = settings.General.BA_R_Gain;
            SubSystem.IS.G_Gain[2] = settings.General.BA_G_Gain;
            SubSystem.IS.B_Gain[2] = settings.General.BA_B_Gain;
            SubSystem.IS.Strenth[0] = settings.General.Strenth;
            SubSystem.IS.Strenth[1] = settings.General.Strenth;
            SubSystem.IS.Strenth[2] = settings.General.Strenth;

            if(General.MachineName == "BAV01" || General.MachineName == "BAV05")
            {
                SubSystem.IS.UseFocus = false;
                SubSystem.IS.FGType[0] = settings.SubSystem.ISType;
                SubSystem.IS.FGType[1] = settings.SubSystem.ISType;
                SubSystem.IS.FGType[2] = settings.SubSystem.ISType;
                SubSystem.IS.FGType[3] = settings.SubSystem.ISType;
                SubSystem.IS.FGType[4] = settings.SubSystem.ISType;
                SubSystem.IS.DeviceName[0] = settings.SubSystem.DeviceName;
                SubSystem.IS.DeviceName[1] = settings.SubSystem.DeviceName;
                SubSystem.IS.DeviceName[2] = settings.SubSystem.DeviceName;
                SubSystem.IS.DeviceName[3] = settings.SubSystem.DeviceName;
                SubSystem.IS.DeviceName[4] = settings.SubSystem.DeviceName;
                SubSystem.IS.CamFile[0] = settings.SubSystem.CamFile;
                SubSystem.IS.CamFile[1] = settings.SubSystem.CamFile;
                SubSystem.IS.CamFile[2] = settings.SubSystem.CamFile;
                SubSystem.IS.CamFile[3] = settings.SubSystem.CamFile;
                SubSystem.IS.CamFile[4] = settings.SubSystem.CamFileSlave;
                SubSystem.IS.IP[0] = settings.General.VisionIP[0];
                SubSystem.IS.IP[1] = settings.General.VisionIP[2];
                SubSystem.IS.IP[2] = settings.General.VisionIP[3];
                SubSystem.IS.IP[3] = settings.General.VisionIP[4];
                SubSystem.IS.IP[4] = settings.General.VisionIP[5];
                SubSystem.IS.Port[0] = settings.General.VisionPort[0];
                SubSystem.IS.Port[1] = settings.General.VisionPort[2];
                SubSystem.IS.Port[2] = settings.General.VisionPort[3];
                SubSystem.IS.Port[3] = settings.General.VisionPort[3];
                SubSystem.IS.Port[4] = settings.General.VisionPort[4];
            }
            else if (General.MachineName == "BAV07" || General.MachineName == "BAV08")
            {
                SubSystem.IS.UseFocus = true;
                SubSystem.IS.FGType[0] = settings.SubSystem.ISType;
                SubSystem.IS.FGType[1] = settings.SubSystem.ISType;
                SubSystem.IS.FGType[2] = settings.SubSystem.ISType;
                SubSystem.IS.FGType[3] = settings.SubSystem.ISType;
                SubSystem.IS.FGType[4] = settings.SubSystem.ISType;
                SubSystem.IS.DeviceName[0] = "0";
                SubSystem.IS.DeviceName[1] = "0";
                SubSystem.IS.DeviceName[2] = "1";
                SubSystem.IS.DeviceName[3] = "0";
                SubSystem.IS.DeviceName[4] = "1";
                SubSystem.IS.CamFile[0] = "1";
                SubSystem.IS.CamFile[1] = "1";
                SubSystem.IS.CamFile[2] = "1";
                SubSystem.IS.CamFile[3] = "1";
                SubSystem.IS.CamFile[4] = "1";
                SubSystem.IS.IP[0] = settings.General.VisionIP[0];
                SubSystem.IS.IP[1] = settings.General.VisionIP[2];
                SubSystem.IS.IP[2] = settings.General.VisionIP[2];
                SubSystem.IS.IP[3] = settings.General.VisionIP[3];
                SubSystem.IS.IP[4] = settings.General.VisionIP[4];
                SubSystem.IS.Port[0] = settings.General.VisionPort[0];
                SubSystem.IS.Port[1] = settings.General.VisionPort[2];
                SubSystem.IS.Port[2] = settings.General.VisionPort[3];
                SubSystem.IS.Port[3] = settings.General.VisionPort[3];
                SubSystem.IS.Port[4] = settings.General.VisionPort[4];
            }
            else if(General.MachineName == "BAV03")
            {
                SubSystem.IS.UseFocus = false;
                SubSystem.IS.FGType[0] = settings.SubSystem.ISType;
                SubSystem.IS.FGType[1] = settings.SubSystem.ISType2;
                SubSystem.IS.FGType[2] = settings.SubSystem.ISType2;
                SubSystem.IS.FGType[3] = settings.SubSystem.ISType2;
                SubSystem.IS.FGType[4] = settings.SubSystem.ISType2;
                SubSystem.IS.DeviceName[0] = settings.SubSystem.DeviceName;
                SubSystem.IS.DeviceName[1] = settings.SubSystem.DeviceName2;
                SubSystem.IS.DeviceName[2] = settings.SubSystem.DeviceName2;
                SubSystem.IS.DeviceName[3] = settings.SubSystem.DeviceName2;
                SubSystem.IS.DeviceName[4] = settings.SubSystem.DeviceName2;
                SubSystem.IS.CamFile[0] = settings.SubSystem.CamFile;
                SubSystem.IS.CamFile[1] = settings.SubSystem.CamFile2;
                SubSystem.IS.CamFile[2] = settings.SubSystem.CamFile2;
                SubSystem.IS.CamFile[3] = settings.SubSystem.CamFile2;
                SubSystem.IS.CamFile[4] = settings.SubSystem.CamFile2;
                SubSystem.IS.IP[0] = settings.General.VisionIP[0];
                SubSystem.IS.IP[1] = settings.General.VisionIP[2];
                SubSystem.IS.IP[2] = settings.General.VisionIP[2];
                SubSystem.IS.IP[3] = settings.General.VisionIP[3];
                SubSystem.IS.IP[4] = settings.General.VisionIP[3];
                SubSystem.IS.Port[0] = settings.General.VisionPort[0];
                SubSystem.IS.Port[1] = settings.General.VisionPort[2];
                SubSystem.IS.Port[2] = settings.General.VisionPort[2];
                SubSystem.IS.Port[3] = settings.General.VisionPort[3];
                SubSystem.IS.Port[4] = settings.General.VisionPort[3];
            }
            else if (General.MachineName == "BAV06" || General.MachineName == "BAV11" || General.MachineName == "BAV12" || General.MachineName == "BAV13")
            {
                SubSystem.IS.UseFocus = false;
                SubSystem.IS.FGType[0] = settings.SubSystem.ISType;
                SubSystem.IS.FGType[1] = settings.SubSystem.ISType2;
                SubSystem.IS.FGType[2] = settings.SubSystem.ISType2;
                SubSystem.IS.FGType[3] = settings.SubSystem.ISType2;
                SubSystem.IS.FGType[4] = settings.SubSystem.ISType2;
                SubSystem.IS.DeviceName[0] = "1";
                SubSystem.IS.DeviceName[1] = settings.SubSystem.DeviceName2;
                SubSystem.IS.DeviceName[2] = settings.SubSystem.DeviceName2;
                SubSystem.IS.DeviceName[3] = settings.SubSystem.DeviceName2;
                SubSystem.IS.DeviceName[4] = settings.SubSystem.DeviceName2;
                SubSystem.IS.CamFile[0] = "1";
                SubSystem.IS.CamFile[1] = settings.SubSystem.CamFile2;
                SubSystem.IS.CamFile[2] = settings.SubSystem.CamFile2;
                SubSystem.IS.CamFile[3] = settings.SubSystem.CamFile2;
                SubSystem.IS.CamFile[4] = settings.SubSystem.CamFile2;
                SubSystem.IS.IP[0] = settings.General.VisionIP[0];
                SubSystem.IS.IP[1] = settings.General.VisionIP[2];
                SubSystem.IS.IP[2] = settings.General.VisionIP[2];
                SubSystem.IS.IP[3] = settings.General.VisionIP[3];
                SubSystem.IS.IP[4] = settings.General.VisionIP[3];
                SubSystem.IS.Port[0] = settings.General.VisionPort[0];
                SubSystem.IS.Port[1] = settings.General.VisionPort[2];
                SubSystem.IS.Port[2] = settings.General.VisionPort[2];
                SubSystem.IS.Port[3] = settings.General.VisionPort[3];
                SubSystem.IS.Port[4] = settings.General.VisionPort[3];
            }
            else if (General.MachineName == "BAV04")
            {
                SubSystem.IS.UseFocus = true;
                SubSystem.IS.FGType[0] = settings.SubSystem.ISType2;
                SubSystem.IS.FGType[1] = settings.SubSystem.ISType;
                SubSystem.IS.FGType[2] = settings.SubSystem.ISType;
                SubSystem.IS.FGType[3] = settings.SubSystem.ISType;
                SubSystem.IS.FGType[4] = settings.SubSystem.ISType;
                SubSystem.IS.DeviceName[0] = "0";
                SubSystem.IS.DeviceName[1] = settings.SubSystem.DeviceName;
                SubSystem.IS.DeviceName[2] = settings.SubSystem.DeviceName;
                SubSystem.IS.DeviceName[3] = settings.SubSystem.DeviceName;
                SubSystem.IS.DeviceName[4] = settings.SubSystem.DeviceName;
                SubSystem.IS.CamFile[0] = "1";
                SubSystem.IS.CamFile[1] = settings.SubSystem.CamFile;
                SubSystem.IS.CamFile[2] = settings.SubSystem.CamFile;
                SubSystem.IS.CamFile[3] = settings.SubSystem.CamFile;
                SubSystem.IS.CamFile[4] = settings.SubSystem.CamFile;
                SubSystem.IS.IP[0] = settings.General.VisionIP[2];
                SubSystem.IS.IP[1] = settings.General.VisionIP[3];
                SubSystem.IS.IP[2] = settings.General.VisionIP[4];
                SubSystem.IS.IP[3] = settings.General.VisionIP[0];
                SubSystem.IS.IP[4] = settings.General.VisionIP[1];
                SubSystem.IS.Port[0] = settings.General.VisionPort[2];
                SubSystem.IS.Port[1] = settings.General.VisionPort[3];
                SubSystem.IS.Port[2] = settings.General.VisionPort[4];
                SubSystem.IS.Port[3] = settings.General.VisionPort[0];
                SubSystem.IS.Port[4] = settings.General.VisionPort[1];
            }
            else if (General.MachineName == "BAV19" || General.MachineName == "BAV20")
            {
                SubSystem.IS.UseFocus = true;
                SubSystem.IS.FGType[0] = settings.SubSystem.ISType2;
                SubSystem.IS.FGType[1] = settings.SubSystem.ISType;
                SubSystem.IS.FGType[2] = settings.SubSystem.ISType;
                SubSystem.IS.FGType[3] = settings.SubSystem.ISType;
                SubSystem.IS.FGType[4] = settings.SubSystem.ISType;
                SubSystem.IS.DeviceName[0] = settings.SubSystem.DeviceName;
                SubSystem.IS.DeviceName[1] = settings.SubSystem.DeviceName;
                SubSystem.IS.DeviceName[2] = settings.SubSystem.DeviceName;
                SubSystem.IS.DeviceName[3] = settings.SubSystem.DeviceName;
                SubSystem.IS.DeviceName[4] = settings.SubSystem.DeviceName;
                SubSystem.IS.CamFile[0] = settings.SubSystem.CamFile2;
                SubSystem.IS.CamFile[1] = settings.SubSystem.CamFile;
                SubSystem.IS.CamFile[2] = settings.SubSystem.CamFile;
                SubSystem.IS.CamFile[3] = settings.SubSystem.CamFile;
                SubSystem.IS.CamFile[4] = settings.SubSystem.CamFile;
                SubSystem.IS.IP[0] = settings.General.VisionIP[2];
                SubSystem.IS.IP[1] = settings.General.VisionIP[3];
                SubSystem.IS.IP[2] = settings.General.VisionIP[4];
                SubSystem.IS.IP[3] = settings.General.VisionIP[0];
                SubSystem.IS.IP[4] = settings.General.VisionIP[1];
                SubSystem.IS.Port[0] = settings.General.VisionPort[2];
                SubSystem.IS.Port[1] = settings.General.VisionPort[3];
                SubSystem.IS.Port[2] = settings.General.VisionPort[4];
                SubSystem.IS.Port[3] = settings.General.VisionPort[0];
                SubSystem.IS.Port[4] = settings.General.VisionPort[1];
            }
            SubSystem.IS.TestID = settings.SubSystem.TestID;
           
            #endregion
            #region PLC
            SubSystem.PLC.UsePLC = settings.Device.PlcUsed == 1 ? true : false;
            SubSystem.PLC.PLCType = settings.Device.PlcType;
            SubSystem.PLC.IP = settings.Device.PlcIP;
            SubSystem.PLC.Port = settings.Device.PlcPort;
            SubSystem.PLC.GoodID = settings.Device.GoodSlotNumber;
            SubSystem.PLC.NGID = settings.Device.NGSlotNumber;
            if (General.MachineName == "BAV04" || General.MachineName == "BAV20" || General.MachineName == "BAV21" || General.MachineName == "BAV22" || General.MachineName == "BAV23" || General.MachineName == "BAV24")    //Type이 다른 거는 정리해서 넣어 주자.
                SubSystem.PLC.MCType = 1;
            else SubSystem.PLC.MCType = 0;
            #endregion
            #region Laser
            SubSystem.Laser.UseLaser = settings.Device.MarkerUsed;
            if(General.MachineName == "BAV11" || General.MachineName == "BAV12" || General.MachineName == "BAV13")
                SubSystem.Laser.DualLaser = false;
            else
                SubSystem.Laser.DualLaser = true;
            SubSystem.Laser.IP = settings.Device.MarkerIP;
            SubSystem.Laser.Port = settings.Device.MarkerPort;
            SubSystem.Laser.CenterY = settings.Device.MarkCenterY;
            SubSystem.Laser.CamResolution = settings.General.AlignResolution;
            SubSystem.Laser.Boat2OffsetY = settings.General.Laser2OffsetX;
            SubSystem.Laser.Boat2OffsetX = settings.General.Laser2OffsetY;
            SubSystem.Laser.Boat2Angle = settings.General.Laser2Angle;
            SubSystem.Laser.BoatAlignPos1 = settings.General.LaserBoat1;
            SubSystem.Laser.BoatAlignPos2 = settings.General.LaserBoat2;
            SubSystem.Laser.CamPosY = settings.General.LaserCam;
            if (General.MachineName == "BAV04" || General.MachineName == "BAV19" || General.MachineName == "BAV20")
                SubSystem.Laser.CamType = 1;
            else
                SubSystem.Laser.CamType = 0;
            #endregion
            #region Light
            SubSystem.Light.UseLight = settings.Device.LightUse;
            //if (General.MachineName == "BAV01" || General.MachineName == "BAV02" || General.MachineName == "BAV03")
            //    SubSystem.Light.LightType = 0;
            //else if (General.MachineName == "BAV05" || General.MachineName == "BAV06")
            //    SubSystem.Light.LightType = 1;
            //else if (General.MachineName == "BAV07" || General.MachineName == "BAV08")
            //    SubSystem.Light.LightType = 2;
            //else if (General.MachineName == "BAV11" || General.MachineName == "BAV12" || General.MachineName == "BAV13")
            //    SubSystem.Light.LightType = 3;
            //else
            //    SubSystem.Light.LightType = 4;

            //if (SubSystem.Light.LightType == 4)
            //{
            //    SubSystem.Light.ComPort[0] = settings.Device.Lights[1];
            //    SubSystem.Light.ComPort[1] = settings.Device.Lights[2];
            //    SubSystem.Light.ComPort[2] = settings.Device.Lights[0];
            //}
            //else {
            //    SubSystem.Light.ComPort[0] = settings.Device.Lights[0];
            //    SubSystem.Light.ComPort[1] = settings.Device.Lights[1];
            //    SubSystem.Light.ComPort[2] = settings.Device.Lights[2];
            //}
            //if (SubSystem.Light.LightType == 0)
            //{
            //    SubSystem.Light.Channel[0, 0] = "동축1";       SubSystem.Light.Channel[1, 0] = "동축1";        SubSystem.Light.Channel[2, 0] = "동축1";
            //    SubSystem.Light.Channel[0, 1] = "동축2";       SubSystem.Light.Channel[1, 1] = "동축2";        SubSystem.Light.Channel[2, 1] = "동축2";
            //    SubSystem.Light.Channel[0, 2] = "측광상1";     SubSystem.Light.Channel[1, 2] = "측광상1";      SubSystem.Light.Channel[2, 2] = "측광상1";
            //    SubSystem.Light.Channel[0, 3] = "측광상2";     SubSystem.Light.Channel[1, 3] = "측광상2";      SubSystem.Light.Channel[2, 3] = "측광상2";
            //    SubSystem.Light.Channel[0, 4] = "측광하1";     SubSystem.Light.Channel[1, 4] = "측광하1";      SubSystem.Light.Channel[2, 4] = "측광하1";
            //    SubSystem.Light.Channel[0, 5] = "측광하2";     SubSystem.Light.Channel[1, 5] = "측광하2";      SubSystem.Light.Channel[2, 5] = "측광하2";
            //    SubSystem.Light.Channel[0, 6] = "";            SubSystem.Light.Channel[1, 6] = "";             SubSystem.Light.Channel[2, 6] = "";
            //    SubSystem.Light.Channel[0, 7] = "";            SubSystem.Light.Channel[1, 7] = "";             SubSystem.Light.Channel[2, 7] = "";
            //}
            //else if (SubSystem.Light.LightType == 1)
            //{
            //    SubSystem.Light.Channel[0, 0] = "동축RED"; SubSystem.Light.Channel[1, 0] = "동축외곽"; SubSystem.Light.Channel[2, 0] = "동축Green";
            //    SubSystem.Light.Channel[0, 1] = "동축BLUE"; SubSystem.Light.Channel[1, 1] = "동축중심"; SubSystem.Light.Channel[2, 1] = "동축BLUE";
            //    SubSystem.Light.Channel[0, 2] = "측광상RED"; SubSystem.Light.Channel[1, 2] = "측광1중"; SubSystem.Light.Channel[2, 2] = "측광상Green";
            //    SubSystem.Light.Channel[0, 3] = "측광상BLUE"; SubSystem.Light.Channel[1, 3] = "측광1외"; SubSystem.Light.Channel[2, 3] = "측광상BLUE";
            //    SubSystem.Light.Channel[0, 4] = "측광하RED"; SubSystem.Light.Channel[1, 4] = "측광2중"; SubSystem.Light.Channel[2, 4] = "측광하Green";
            //    SubSystem.Light.Channel[0, 5] = "측광하BLUE"; SubSystem.Light.Channel[1, 5] = "측광2외"; SubSystem.Light.Channel[2, 5] = "측광하BLUE";
            //    SubSystem.Light.Channel[0, 6] = ""; SubSystem.Light.Channel[1, 6] = "측광3중"; SubSystem.Light.Channel[2, 6] = "";
            //    SubSystem.Light.Channel[0, 7] = ""; SubSystem.Light.Channel[1, 7] = "측광3외"; SubSystem.Light.Channel[2, 7] = "";
            //}
            //else if (SubSystem.Light.LightType == 2)
            //{
            //    SubSystem.Light.Channel[0, 0] = "동축RED"; SubSystem.Light.Channel[1, 0] = "동축IR"; SubSystem.Light.Channel[2, 0] = "동축IR";
            //    SubSystem.Light.Channel[0, 1] = "동축BLUE"; SubSystem.Light.Channel[1, 1] = "동축백색"; SubSystem.Light.Channel[2, 1] = "동축백색";
            //    SubSystem.Light.Channel[0, 2] = "측광상RED"; SubSystem.Light.Channel[1, 2] = "측상IR"; SubSystem.Light.Channel[2, 2] = "측상IR";
            //    SubSystem.Light.Channel[0, 3] = "측광상BLUE"; SubSystem.Light.Channel[1, 3] = "측상백색"; SubSystem.Light.Channel[2, 3] = "측상백색";
            //    SubSystem.Light.Channel[0, 4] = "측광하RED"; SubSystem.Light.Channel[1, 4] = "측하IR"; SubSystem.Light.Channel[2, 4] = "측하IR";
            //    SubSystem.Light.Channel[0, 5] = "측광하BLUE"; SubSystem.Light.Channel[1, 5] = "측하백색"; SubSystem.Light.Channel[2, 5] = "측하백색";
            //    SubSystem.Light.Channel[0, 6] = ""; SubSystem.Light.Channel[1, 6] = ""; SubSystem.Light.Channel[2, 6] = "";
            //    SubSystem.Light.Channel[0, 7] = ""; SubSystem.Light.Channel[1, 7] = ""; SubSystem.Light.Channel[2, 7] = "";
            //}
            //else if (SubSystem.Light.LightType == 3)
            //{
            //    SubSystem.Light.Channel[0, 0] = "동축1"; SubSystem.Light.Channel[1, 0] = "돔"; SubSystem.Light.Channel[2, 0] = "동축1";
            //    SubSystem.Light.Channel[0, 1] = "동축2"; SubSystem.Light.Channel[1, 1] = "동축중심"; SubSystem.Light.Channel[2, 1] = "동축2";
            //    SubSystem.Light.Channel[0, 2] = "측광상1"; SubSystem.Light.Channel[1, 2] = "동축외곽"; SubSystem.Light.Channel[2, 2] = "측광상1";
            //    SubSystem.Light.Channel[0, 3] = "측광상2"; SubSystem.Light.Channel[1, 3] = ""; SubSystem.Light.Channel[2, 3] = "측광상2";
            //    SubSystem.Light.Channel[0, 4] = "측광하1"; SubSystem.Light.Channel[1, 4] = ""; SubSystem.Light.Channel[2, 4] = "측광하1";
            //    SubSystem.Light.Channel[0, 5] = "측광하2"; SubSystem.Light.Channel[1, 5] = ""; SubSystem.Light.Channel[2, 5] = "측광하2";
            //    SubSystem.Light.Channel[0, 6] = ""; SubSystem.Light.Channel[1, 6] = ""; SubSystem.Light.Channel[2, 6] = "";
            //    SubSystem.Light.Channel[0, 7] = ""; SubSystem.Light.Channel[1, 7] = ""; SubSystem.Light.Channel[2, 7] = "";
            //}
            //else if (SubSystem.Light.LightType == 4)
            //{
            //    SubSystem.Light.Channel[0, 0] = "동축RED"; SubSystem.Light.Channel[1, 0] = "동축중심"; SubSystem.Light.Channel[2, 0] = "동축중심";
            //    SubSystem.Light.Channel[0, 1] = "동축BLUE"; SubSystem.Light.Channel[1, 1] = "동축외곽"; SubSystem.Light.Channel[2, 1] = "동축외곽";
            //    SubSystem.Light.Channel[0, 2] = "측광상RED"; SubSystem.Light.Channel[1, 2] = "측상중심"; SubSystem.Light.Channel[2, 2] = "측상중심";
            //    SubSystem.Light.Channel[0, 3] = "측광상BLUE"; SubSystem.Light.Channel[1, 3] = "측상외곽"; SubSystem.Light.Channel[2, 3] = "측상외곽";
            //    SubSystem.Light.Channel[0, 4] = "측광하RED"; SubSystem.Light.Channel[1, 4] = "측하중심"; SubSystem.Light.Channel[2, 4] = "측하중심";
            //    SubSystem.Light.Channel[0, 5] = "측광하BLUE"; SubSystem.Light.Channel[1, 5] = "측하외곽"; SubSystem.Light.Channel[2, 5] = "측하외곽";
            //    SubSystem.Light.Channel[0, 6] = ""; SubSystem.Light.Channel[1, 6] = ""; SubSystem.Light.Channel[2, 6] = "";
            //    SubSystem.Light.Channel[0, 7] = ""; SubSystem.Light.Channel[1, 7] = ""; SubSystem.Light.Channel[2, 7] = "";
            //}

            #endregion
            #region ENCODER
            SubSystem.ENC.UseEncoder = settings.Device.CounterUsed == 1? true:false;
            //if(SubSystem.Light.LightType == 4)
            //    SubSystem.ENC.EncoderType = 1;
            //else
            //    SubSystem.ENC.EncoderType = 0; // 0 : NI 1: SW
            SubSystem.ENC.Low[0] = settings.Device.LowCtr0;
            SubSystem.ENC.Low[1] = settings.Device.LowCtr1;
            SubSystem.ENC.Low[2] = settings.Device.LowCtr2;
            SubSystem.ENC.High[0] = settings.Device.HighCtr0;
            SubSystem.ENC.High[1] = settings.Device.HighCtr1;
            SubSystem.ENC.High[2] = settings.Device.HighCtr2;
            SubSystem.ENC.UseFilter[0] = settings.Device.FilterUseCtr0;
            SubSystem.ENC.UseFilter[1] = settings.Device.FilterUseCtr1;
            SubSystem.ENC.UseFilter[2] = settings.Device.FilterUseCtr2;
            SubSystem.ENC.FilterTime[0] = settings.Device.FilterPulseCtr0;
            SubSystem.ENC.FilterTime[1] = settings.Device.FilterPulseCtr1;
            SubSystem.ENC.FilterTime[2] = settings.Device.FilterPulseCtr2;

            SubSystem.ENC.Port[0] = settings.Device.TriggerPort2;
            SubSystem.ENC.Port[1] = settings.Device.TriggerPort1;
            SubSystem.ENC.Direction[0] = settings.Device.TriggerDir2;
            SubSystem.ENC.Direction[1] = settings.Device.TriggerDir1;
            SubSystem.ENC.Count[0] = settings.Device.TriggerCount2;
            SubSystem.ENC.Count[1] = settings.Device.TriggerCount1;
            SubSystem.ENC.Delay1[0] = settings.Device.TriggerDelay21;
            SubSystem.ENC.Delay1[1] = settings.Device.TriggerDelay11;
            SubSystem.ENC.Delay2[0] = settings.Device.TriggerDelay22;
            SubSystem.ENC.Delay2[1] = settings.Device.TriggerDelay12;
            SubSystem.ENC.Width[0] = settings.Device.TriggerWidth2;
            SubSystem.ENC.Width[1] = settings.Device.TriggerWidth1;
            #endregion
            #endregion
            #region Job Conversion
            Job.ProcessCode = settings.General.ProcessCode;
            Job.Customer = settings.General.Customer;
            Job.LastGroup = settings.General.LastSelectedGroup;
            Job.LastModel = settings.General.LastSelectedModel;
            Job.LastLot = settings.General.LastLot;
            Job.LastUser = settings.General.LastUser;
            Job.LastVerify = settings.General.LastVerify;
            Job.AutoNG = settings.General.AutoNG;
            Job.Cam1Position = settings.General.Cam1Position;
            Job.Cam1Pitch = settings.General.Cam1Pitch;
            Job.ScanVelocity1 = settings.General.ScanVelocity1;
            Job.ScanVelocity2 = settings.General.ScanVelocity2;
            Job.PSRShiftType = settings.SubSystem.PSRShiftType;
            #endregion
            settings = null;
            Save();
        }
    }

    public class Generals
    {
        #region Members
        public string MachineCode;           //DB에서 사용되는 MC Code 나중에 삭제 하도록
        public string MachineName;          //설비 코드 
        public int MachineType;          //Boat1/Boat2의 순서가 CA/BA인지 BA/CA인지
        public string MachineIP;            //설비의 IP      
        public string ModelPath;            //모델의 정보를 저장하는 경로
        public string ResultPath;           //검사 결과를 저장하는 경로
        public string XMLMapPath;           //결과 표준화 XML Map 저장하는 경로
        public string POP_Path;              //Loss 저장 경로, POP 완료보고 시 경로 참조
        public string VerifyInfoPath;        //Verify 정보 저장 경로
        public string IDMarkPath;           //ID Mark 정보 저장 경로      
        public bool UsePassword;            //비밀번호 사용 
        public bool SaveFailLoss;           //폐기정보를 POP 정보에 사용 할지
        public double RejectRate;            //X-OUT 초과 율 Default 값 설정
        public int VRSNGUnitLimit;          //Verify 시 유닛 불량 수 초과 설정
        public bool UseRVS;                 //Online Verify 사용
        public string RVSIP;                //OnLine Verify IP
        public string RVSPort;                 //OnLine Verify Port
        public bool UseITS;                 //ITS 적용
        public string ITSPath1;             //ITS SkipData 경로
        public string ITSPath2;             //ITS 저장 경로
        public string ITSPath3;             //ITS 결과 데이터 위치
        public bool UseDB;                  //DB 사용
        public string DBIP;                 //DB IP
        public string DBPort;                  //DB Port
        public bool UseVRSDB;               //VRS  DB 사용
        public string VRSDBIP;              //VRS DB IP
        public string VRSDBPort;               //VRS DB Port
        public string VRSAVITableName;      //VRS Table Name
        public string VRSBinCodeTableName;  //VRS 불량 코드 Table Name    
        //ITS DB USE == AutoReceipt, ITS USE = JS System 2D 연동 2가지
        public bool UseITSDB;               //ITS DB 사용
        public string ITSDBIP;              //ITS DB IP
        public string ITSDBPort;               //ITS DB Port
        public string ITSTableName;         //ITS Table Name
        public bool UseIDReader;            //ID Reader 사용
        public string IDReaderIP;           //ID Reader IP
        public bool LogSave;
        public int LogLevel;
        public int LogDPLevel;
        public int LogKeepDate;
        public bool UsePOP;
        public bool UseXML;
        public string POP_IP;
        public string POP_BK_IP;
        public string POP_Port;
        public string POP_LastUser;
        public int POP_Delay_Second;
        public int MaxLimitDefect;
        public bool UseAI;
        public string PathAIWeights;
        public bool ResultImageSizeType;
        public int ResultImageSize1;                     //ResultImageSize
        public int ResultImageSize2;                     //ResultImageSize
        public int CornerImageSizeX_mm;                     //CornerImageSizeX
        public int CornerImageSizeY_mm;                     //CornerImageSizeY
        public string ICSFilePath;                   //ICSFilePath
        public int ICS_OFFSET_BP_X;
        public int ICS_OFFSET_BP_Y;
        public int ICS_OFFSET_CA_X;
        public int ICS_OFFSET_CA_Y;
        public int ICS_OFFSET_BA_X;
        public int ICS_OFFSET_BA_Y;
        #endregion

        private string m_Path;
        public Generals(string astrPath)
        {
            m_Path = astrPath;
        }
        public bool Load()
        {
            if (!File.Exists(m_Path))
            {
                FileStream fs = File.Create(m_Path);
                fs.Close();
                Save();
                return false;
            }
            IniFile ini = new IniFile(m_Path);
            MachineCode = ini.ReadString("MACHINE", "Code", "0001");
            MachineName = ini.ReadString("MACHINE", "Name", "BAV01");
            MachineType = ini.ReadInteger("MACHINE", "Type", 0);
            MachineIP = ini.ReadString("MACHINE", "IP", "127.0.0.1");
            ModelPath = ini.ReadString("PATH", "Model", "d:\\Model");
            ResultPath = ini.ReadString("PATH", "Result", "d:\\Result");
            XMLMapPath = ini.ReadString("PATH", "XMLMap", "d:\\XMLMapPath");
            POP_Path = ini.ReadString("PATH", "POP", "d:\\Loss");
            VerifyInfoPath = ini.ReadString("PATH", "VerifyInfo", "d:\\VerifyInfo");
            IDMarkPath = ini.ReadString("PATH", "IDMark", "IDMark");

            UsePassword = ini.ReadBool("PASSWORD", "Use", true);
            RejectRate = ini.ReadDouble("Reject", "Rate", 80);
            VRSNGUnitLimit = ini.ReadInteger("VERIFY", "UnitLimit", 10);

            UseIDReader = ini.ReadBool("ITS", "Use_IDReader", UseIDReader);
            IDReaderIP = ini.ReadString("ITS", "DB_IDReaderIP", IDReaderIP);

            UseDB = ini.ReadBool("DATABASE", "Use", true);
            DBIP = ini.ReadString("DATABASE", "IP", "127.0.0.1");
            DBPort = ini.ReadString("DATABASE", "Port", "5000");

            UseRVS = ini.ReadBool("VERIFY", "UseRVS", false);
            RVSIP = ini.ReadString("VERIFY", "RVS_IP", "127.0.0.1");
            RVSPort = ini.ReadString("VERIFY", "RVS_Port", "5000");

            UseVRSDB = ini.ReadBool("VERIFY", "UseVRS_DB", false);
            VRSDBIP = ini.ReadString("VERIFY", "VRS_DBIP", "127.0.0.1");
            VRSDBPort = ini.ReadString("VERIFY", "VRS_DBPort", "5000");
            VRSAVITableName = ini.ReadString("VERIFY", "AVI_TableName", "AVI");
            VRSBinCodeTableName = ini.ReadString("VERIFY", "BinCode_TableName", "Bin");

            UseITS = ini.ReadBool("ITS", "Use", false);
            ITSPath1 = ini.ReadString("ITS", "Path1", "d:\\ITS");
            ITSPath2 = ini.ReadString("ITS", "Path2", "d:\\ITS");
            ITSPath3 = ini.ReadString("ITS", "Path3", "d:\\ITS");

            UseITSDB = ini.ReadBool("ITS", "Use_DB", false);
            ITSDBIP = ini.ReadString("ITS", "DB_IP", "127.0.0.1");
            ITSDBPort = ini.ReadString("ITS", "DB_Port", "5000");
            ITSTableName = ini.ReadString("ITS", "TableName", "ITS");
            LogSave = ini.ReadBool("Log", "UseSave", true);
            LogLevel = ini.ReadInteger("Log", "SaveLevel", 1);
            LogDPLevel = ini.ReadInteger("Log", "DPLevel", 1);
            LogKeepDate = ini.ReadInteger("Log", "KeepDate", 60);

            UsePOP = ini.ReadBool("POP", "UsePOP", false);
            UseXML = ini.ReadBool("POP", "UseXML", false);
            POP_IP = ini.ReadString("POP", "IP", "127.0.0.1");
            POP_BK_IP = ini.ReadString("POP", "BK_IP", "127.0.0.1");
            POP_Port = ini.ReadString("POP", "PORT", "3306");
            POP_LastUser = ini.ReadString("POP", "LastUser", "");
            POP_Delay_Second = ini.ReadInteger("POP", "DelayTimeForSecond", 300);
            MaxLimitDefect = ini.ReadInteger("INSPECT", "MaxLimitDefect", 512);

            UseAI = ini.ReadBool("AI", "Use", false);
            PathAIWeights = ini.ReadString("AI", "WeightsPath", "D:\\main\\bin\\Dnn_Models");
            ResultImageSizeType = ini.ReadBool("VERIFY", "ResultImageSizeType", false);
            ResultImageSize1 = ini.ReadInteger("VERIFY", "ResultImageSize1", 96);
            ResultImageSize2 = ini.ReadInteger("VERIFY", "ResultImageSize2", 300);
            CornerImageSizeX_mm = ini.ReadInteger("ICS", "CornerImageSizeX_mm", 10);
            CornerImageSizeY_mm = ini.ReadInteger("ICS", "CornerImageSizeY_mm", 10);
            ICSFilePath = ini.ReadString("ICS", "ICSFilePath", "D:\\Model");

            ICS_OFFSET_BP_X = ini.ReadInteger("ICS", "ICS_OFFSET_BP_X", 0);
            ICS_OFFSET_BP_Y = ini.ReadInteger("ICS", "ICS_OFFSET_BP_Y", 0);
            ICS_OFFSET_CA_X = ini.ReadInteger("ICS", "ICS_OFFSET_CA_X", 0);
            ICS_OFFSET_CA_Y = ini.ReadInteger("ICS", "ICS_OFFSET_CA_Y", 0);
            ICS_OFFSET_BA_X = ini.ReadInteger("ICS", "ICS_OFFSET_BA_X", 0);
            ICS_OFFSET_BA_Y = ini.ReadInteger("ICS", "ICS_OFFSET_BA_Y", 0);
            return true;
        }

        public void Save()
        {
            if (!File.Exists(m_Path))
            {
                FileStream fs = File.Create(m_Path);
                fs.Close();
            }
            IniFile ini = new IniFile(m_Path);
            ini.WriteString("MACHINE", "Code", MachineCode);
            ini.WriteString("MACHINE", "Name", MachineName);
            ini.WriteInteger("MACHINE", "Type", MachineType);
            ini.WriteString("MACHINE", "IP", MachineIP);
            ini.WriteString("PATH", "Model", ModelPath);
            ini.WriteString("PATH", "Result", ResultPath);
            ini.WriteString("PATH", "XMLMap", XMLMapPath);
            ini.WriteString("PATH", "POP", POP_Path);
            ini.WriteString("PATH", "VerifyInfo", VerifyInfoPath);
            ini.WriteString("PATH", "IDMark", IDMarkPath);

            ini.WriteBool("PASSWORD", "Use", UsePassword);
            ini.WriteDouble("Reject", "Rate", RejectRate);
            ini.WriteInteger("VERIFY", "UnitLimit", VRSNGUnitLimit);

            ini.WriteBool("ITS", "Use_IDReader", UseIDReader);
            ini.WriteString("ITS", "DB_IDReaderIP", IDReaderIP);

            ini.WriteBool("DATABASE", "Use", UseDB);
            ini.WriteString("DATABASE", "IP", DBIP);
            ini.WriteString("DATABASE", "Port", DBPort);

            ini.WriteBool("VERIFY", "UseRVS", UseRVS);
            ini.WriteString("VERIFY", "RVS_IP", RVSIP);
            ini.WriteString("VERIFY", "RVS_Port", RVSPort);

            ini.WriteBool("VERIFY", "UseVRS_DB", UseVRSDB);
            ini.WriteString("VERIFY", "VRS_DBIP", VRSDBIP);
            ini.WriteString("VERIFY", "VRS_DBPort", VRSDBPort);
            ini.WriteString("VERIFY", "AVI_TableName", VRSAVITableName);
            ini.WriteString("VERIFY", "BinCode_TableName", VRSBinCodeTableName);
            ini.WriteBool("VERIFY", "ResultImageSizeType", ResultImageSizeType);
            ini.WriteInteger("VERIFY", "ResultImageSize1", ResultImageSize1);
            ini.WriteInteger("VERIFY", "ResultImageSize2", ResultImageSize2);

            ini.WriteBool("ITS", "Use", UseITS);
            ini.WriteString("ITS", "Path1", ITSPath1);
            ini.WriteString("ITS", "Path2", ITSPath2);
            ini.WriteString("ITS", "Path3", ITSPath3);

            ini.WriteBool("ITS", "Use_DB", UseITSDB);
            ini.WriteString("ITS", "DB_IP", ITSDBIP);
            ini.WriteString("ITS", "DB_Port", ITSDBPort);
            ini.WriteString("ITS", "TableName", ITSTableName);
            ini.WriteBool("Log", "UseSave", LogSave);
            ini.WriteInteger("Log", "SaveLevel", LogLevel);
            ini.WriteInteger("Log", "DPLevel", LogDPLevel);
            ini.WriteInteger("Log", "KeepDate", LogKeepDate);
            ini.WriteBool("POP", "UsePOP", UsePOP);
            ini.WriteBool("POP", "UseXML", UseXML);
            ini.WriteString("POP", "IP", POP_IP);
            ini.WriteString("POP", "BK_IP", POP_BK_IP);
            ini.WriteString("POP", "PORT", POP_Port);
            ini.WriteString("POP", "LastUser", POP_LastUser);
            ini.WriteInteger("POP", "DelayTimeForSecond", POP_Delay_Second);
            ini.WriteInteger("INSPECT", "MaxLimitDefect", MaxLimitDefect);

            ini.WriteBool("AI", "Use", UseAI);
            ini.WriteString("AI", "WeightsPath", PathAIWeights);

            ini.WriteInteger("ICS", "CornerImageSizeX_mm", CornerImageSizeX_mm);
            ini.WriteInteger("ICS", "CornerImageSizeY_mm", CornerImageSizeY_mm);
            ini.WriteString("ICS", "ICSFilePath", ICSFilePath);

            ini.WriteInteger("ICS", "ICS_OFFSET_BP_X", ICS_OFFSET_BP_X);
            ini.WriteInteger("ICS", "ICS_OFFSET_BP_Y", ICS_OFFSET_BP_Y);
            ini.WriteInteger("ICS", "ICS_OFFSET_CA_X", ICS_OFFSET_CA_X);
            ini.WriteInteger("ICS", "ICS_OFFSET_CA_Y", ICS_OFFSET_CA_Y);
            ini.WriteInteger("ICS", "ICS_OFFSET_BA_X", ICS_OFFSET_BA_X);
            ini.WriteInteger("ICS", "ICS_OFFSET_BA_Y", ICS_OFFSET_BA_Y);
        }
    }

    public class SubSystems
    {
        public ISPara IS;
        public PLCPara PLC;
        public LaserPara Laser;
        public LightPara Light;
        public Encoder ENC;
        private string m_Path;
        public SubSystems(string astrPath)
        {
            IS = new ISPara();
            PLC = new PLCPara();
            Laser = new LaserPara();
            Light = new LightPara();
            ENC = new Encoder();
            m_Path = astrPath;
        }

        public bool Load()
        {
            if (!File.Exists(m_Path))
            {
                FileStream fs = File.Create(m_Path);
                fs.Close();
                Save();
                return false;
            }
            IniFile ini = new IniFile(m_Path);
            #region IS
            IS.UseCASlave = ini.ReadBool("IS", "Use_CA_Slave", true);
            IS.UseBASlave = ini.ReadBool("IS", "Use_BA_Slave", true);
            IS.UseFocus = ini.ReadBool("IS", "Use_Focus", false);
            IS.ReScale = ini.ReadDouble("IS", "ReScale_18K", 2.0);
            for (int i=0;i< 3; i++)
            {
                IS.CameraWidth[i] = ini.ReadInteger("IS", "CameraWidth" + (i+1).ToString(), 0);
                IS.CameraHeight[i]= ini.ReadInteger("IS", "CameraHeight" + (i + 1).ToString(), 0);
                IS.VisionFlipX[i] = ini.ReadBool("IS", "VisionFlipX" + (i + 1).ToString(), false);
                IS.CamResolutionX[i] = ini.ReadDouble("IS", "CameRsolutionX" + (i + 1).ToString(), 0.0);
                IS.CamResolutionY[i] = ini.ReadDouble("IS", "CameRsolutionY" + (i + 1).ToString(), 0.0);
                IS.CamPageDelay[i] = ini.ReadInteger("IS", "CamPageDelay" + (i + 1).ToString(), 0);
                IS.R_Gain[i] = (float)ini.ReadDouble("IS", "R_Gain" + (i + 1).ToString(), 1.0);
                IS.G_Gain[i] = (float)ini.ReadDouble("IS", "G_Gain" + (i + 1).ToString(), 1.0);
                IS.B_Gain[i] = (float)ini.ReadDouble("IS", "B_Gain" + (i + 1).ToString(), 1.0);
                IS.Strenth[i] = (float)ini.ReadDouble("IS", "Strenth" + (i + 1).ToString(), 0.1);
            }
            for (int i = 0; i < 5; i++)
            {
                IS.FGType[i] = ini.ReadInteger("IS", "FG_Type" + (i + 1).ToString(), 0);
                IS.DeviceName[i] = ini.ReadString("IS", "DeviceName" + (i + 1).ToString(), "");
                IS.CamFile[i] = ini.ReadString("IS", "CamFile" + (i + 1).ToString(), "");
                IS.IP[i] = ini.ReadString("IS", "IP" + (i + 1).ToString(), "127.0.0.1");
                IS.Port[i] = ini.ReadInteger("IS", "Port" + (i + 1).ToString(), 0);
            }
            IS.TestID = ini.ReadInteger("IS", "Test_ID", 0);
            #endregion
            #region PLC
            PLC.UsePLC = ini.ReadBool("PLC", "Use", false);
            PLC.PLCType = ini.ReadInteger("PLC", "Type", 0);
            PLC.IP = ini.ReadString("PLC", "IP", "127.0.0.0"); 
            PLC.Port = ini.ReadInteger("PLC", "Port", 0);
            PLC.GoodID = ini.ReadInteger("PLC", "Good_ID", 0);
            PLC.NGID = ini.ReadInteger("PLC", "NG_ID", 0);
            PLC.MCType = ini.ReadInteger("PLC", "MCType", 0);
            #endregion
            #region Laser
            Laser.UseLaser = ini.ReadBool("Laser", "Use", true);
            Laser.DualLaser = ini.ReadBool("Laser", "Use_Dual", true);
            Laser.IP = ini.ReadString("Laser", "IP", "127.0.0.1");
            Laser.Port = ini.ReadString("Laser", "Port", "100");
            Laser.CenterY = ini.ReadDouble("Laser", "CenterY", 0);
            Laser.CamResolution = ini.ReadDouble("Laser", "CamResolution", 0);
            Laser.Boat2OffsetY = ini.ReadDouble("Laser", "Boat2OffsetY", 0);
            Laser.Boat2OffsetX = ini.ReadDouble("Laser", "Boat2OffsetX", 0);
            Laser.Boat2Angle = ini.ReadDouble("Laser", "Boat2Angle", 0);
            Laser.BoatAlignPos1 = ini.ReadDouble("Laser", "BoatAlignPos1", 0);
            Laser.BoatAlignPos2 = ini.ReadDouble("Laser", "BoatAlignPos2", 0);
            Laser.CamPosY = ini.ReadDouble("Laser", "CamPosY", 0);
            Laser.CamType = ini.ReadInteger("Laser", "AlignCamType", 0);
            #endregion
            #region Light
            Light.UseLight = ini.ReadBool("Light", "Use", true);
            Light.LightType[0] = ini.ReadInteger("Light", "Type1", 0);
            Light.LightType[1] = ini.ReadInteger("Light", "Type2", 0);
            Light.LightType[2] = ini.ReadInteger("Light", "Type3", 0);
            Light.ComPort[0] = ini.ReadString("Light", "Port1", "COM1");
            Light.ComPort[1] = ini.ReadString("Light", "Port2", "COM2");
            Light.ComPort[2] = ini.ReadString("Light", "Port3", "COM3");
            for(int i = 0; i< 3; i++)
            {
                string ch = ini.ReadString("Light", "Channels" + (i + 1).ToString(), "");
                string[] chs = ch.Split(';');
                int n = 0;
                foreach(string c in chs)
                {
                    Light.Channel[i, n] = c;
                    n++;
                    if (n == 8) break;
                }
            }
            #endregion
            #region Encoder
            ENC.UseEncoder = ini.ReadBool("Encoder", "Use", true);
            ENC.EncoderType = ini.ReadInteger("Encoder", "Type", 1);
            for (int i = 0; i < 3; i++)
            {
                ENC.Low[i] = ini.ReadInteger("Encoder", "Low_" + (i + 1).ToString(), 1);
                ENC.High[i] = ini.ReadInteger("Encoder", "High_" + (i + 1).ToString(), 1);
                ENC.UseFilter[i] = ini.ReadInteger("Encoder", "UseFilter_" + (i + 1).ToString(), 1);
                ENC.FilterTime[i] = ini.ReadDouble("Encoder", "FilterTime_" + (i + 1).ToString(), 1);
            }
            for (int i = 0; i < 2; i++)
            {
                ENC.Port[i] = ini.ReadString("Encoder", "Port_"+ (i+1).ToString(), "COM1");
                ENC.Direction[i] = ini.ReadInteger("Encoder", "Direction_" + (i + 1).ToString(), 0);
                ENC.Count[i] = ini.ReadInteger("Encoder", "Count_" + (i + 1).ToString(), 0);
                ENC.Delay1[i] = ini.ReadInteger("Encoder", "Delay1_" + (i + 1).ToString(), 0);
                ENC.Delay2[i] = ini.ReadInteger("Encoder", "Delay2_" + (i + 1).ToString(), 0);
                ENC.Width[i] = ini.ReadInteger("Encoder", "Width_" + (i + 1).ToString(), 0);
            }
            #endregion
            return true;
        }

        public void Save()
        {
            if (!File.Exists(m_Path))
            {
                FileStream fs = File.Create(m_Path);
                fs.Close();
            }
            IniFile ini = new IniFile(m_Path);
            #region IS
            ini.WriteBool("IS", "Use_CA_Slave", IS.UseCASlave);
            ini.WriteBool("IS", "Use_BA_Slave", IS.UseBASlave);
            ini.WriteBool("IS", "Use_Focus", IS.UseFocus);
            ini.WriteDouble("IS", "ReScale_18K", IS.ReScale);
            for (int i = 0; i < 3; i++)
            {
                 ini.WriteInteger("IS", "CameraWidth" + (i + 1).ToString(), IS.CameraWidth[i]);
                 ini.WriteInteger("IS", "CameraHeight" + (i + 1).ToString(), IS.CameraHeight[i]);
                 ini.WriteBool("IS", "VisionFlipX" + (i + 1).ToString(), IS.VisionFlipX[i]);
                 ini.WriteDouble("IS", "CameRsolutionX" + (i + 1).ToString(), IS.CamResolutionX[i]);
                 ini.WriteDouble("IS", "CameRsolutionY" + (i + 1).ToString(), IS.CamResolutionY[i]);
                 ini.WriteInteger("IS", "CamPageDelay" + (i + 1).ToString(), IS.CamPageDelay[i]);
                 ini.WriteDouble("IS", "R_Gain" + (i + 1).ToString(), IS.R_Gain[i]);
                 ini.WriteDouble("IS", "G_Gain" + (i + 1).ToString(), IS.G_Gain[i]);
                 ini.WriteDouble("IS", "B_Gain" + (i + 1).ToString(), IS.B_Gain[i]);
                 ini.WriteDouble("IS", "Strenth" + (i + 1).ToString(), IS.Strenth[i]);
            }
            for (int i = 0; i < 5; i++)
            {
                 ini.WriteInteger("IS", "FG_Type" + (i + 1).ToString(), IS.FGType[i]);
                 ini.WriteString("IS", "DeviceName" + (i + 1).ToString(), IS.DeviceName[i]);
                 ini.WriteString("IS", "CamFile" + (i + 1).ToString(), IS.CamFile[i]);
                 ini.WriteString("IS", "IP" + (i + 1).ToString(), IS.IP[i]);
                 ini.WriteInteger("IS", "Port" + (i + 1).ToString(), IS.Port[i]);
            }
            ini.WriteInteger("IS", "Test_ID", IS.TestID);
            
            #endregion
            #region PLC
            ini.WriteBool("PLC", "Use", PLC.UsePLC);
            ini.WriteInteger("PLC", "Type", PLC.PLCType);
            ini.WriteString("PLC", "IP", PLC.IP);
            ini.WriteInteger("PLC", "Port", PLC.Port);
            ini.WriteInteger("PLC", "Good_ID", PLC.GoodID);
            ini.WriteInteger("PLC", "NG_ID", PLC.NGID);
            ini.WriteInteger("PLC", "MCType", PLC.MCType);
            #endregion
            #region Laser
            ini.WriteBool("Laser", "Use", Laser.UseLaser);
            ini.WriteBool("Laser", "Use_Dual", Laser.DualLaser);
            ini.WriteString("Laser", "IP", Laser.IP);
            ini.WriteString("Laser", "Port", Laser.Port);
            ini.WriteDouble("Laser", "CenterY", Laser.CenterY);
            ini.WriteDouble("Laser", "CamResolution", Laser.CamResolution);
            ini.WriteDouble("Laser", "Boat2OffsetY", Laser.Boat2OffsetY);
            ini.WriteDouble("Laser", "Boat2OffsetX", Laser.Boat2OffsetX);
            ini.WriteDouble("Laser", "Boat2Angle", Laser.Boat2Angle);
            ini.WriteDouble("Laser", "BoatAlignPos1", Laser.BoatAlignPos1);
            ini.WriteDouble("Laser", "BoatAlignPos2", Laser.BoatAlignPos2);
            ini.WriteDouble("Laser", "CamPosY", Laser.CamPosY);
            ini.WriteInteger("Laser", "AlignCamType", Laser.CamType);
            #endregion
            #region Light
            ini.WriteBool("Light", "Use", Light.UseLight);
            ini.WriteInteger("Light", "Type1", Light.LightType[0]);
            ini.WriteInteger("Light", "Type2", Light.LightType[1]);
            ini.WriteInteger("Light", "Type3", Light.LightType[2]);
            ini.WriteString("Light", "Port1", Light.ComPort[0]);
            ini.WriteString("Light", "Port2", Light.ComPort[1]);
            ini.WriteString("Light", "Port3", Light.ComPort[2]);
            for (int i = 0; i < 3; i++)
            {
                string str = "";
                for(int n = 0; n < 8; n++)
                {
                    str += (Light.Channel[i, n] + ";");
                }

                ini.WriteString("Light", "Channels" + (i + 1).ToString(), str);
            }
            #endregion
            #region Encoder
            ini.WriteBool("Encoder", "Use", ENC.UseEncoder);
            ini.WriteInteger("Encoder", "Type", ENC.EncoderType);
            for (int i = 0; i < 3; i++)
            {
                ini.WriteInteger("Encoder", "Low_" + (i + 1).ToString(), (int)ENC.Low[i]);
                ini.WriteInteger("Encoder", "High_" + (i + 1).ToString(), (int)ENC.High[i]);
                ini.WriteInteger("Encoder", "UseFilter_" + (i + 1).ToString(), (int)ENC.UseFilter[i]);
                ini.WriteDouble("Encoder", "FilterTime_" + (i + 1).ToString(), ENC.FilterTime[i]);
            }
            for (int i = 0; i < 2; i++)
            {
                ini.WriteString("Encoder", "Port_" + (i + 1).ToString(), ENC.Port[i]);
                ini.WriteInteger("Encoder", "Direction_" + (i + 1).ToString(), ENC.Direction[i]);
                ini.WriteInteger("Encoder", "Count_" + (i + 1).ToString(), ENC.Count[i]);
                ini.WriteInteger("Encoder", "Delay1_" + (i + 1).ToString(), ENC.Delay1[i]);
                ini.WriteInteger("Encoder", "Delay2_" + (i + 1).ToString(), ENC.Delay2[i]);
                ini.WriteInteger("Encoder", "Width_" + (i + 1).ToString(), ENC.Width[i]);
            }
            #endregion
        }
    }

    public class Jobs
    {
        public string ProcessCode; //작업의 공정코드
        public int Customer; //작업의 고객코드
        public int LastGroup; //최근 작업 그룹
        public string LastModel;//최근 작업 모델 
        public string LastLot; //최근 작업 로트
        public string LastItsLot; //최근 작업 로트 ITS 번호
        public string LastUser; //최근 작업자
        public string LastVerify; //최근 Verify 옵션
        public string AutoNG; //AutoNG 정보 
        public string PriorityNG; //불량 우선순위
        public double Cam1Position; //BP 카메라의 시작위치
        public double Cam1Pitch; //BP 카메라의 이동 피치
        public int ScanVelocity1; //TOP 스캔속도
        public int ScanVelocity2; //BOT 스캔속도
        public int PSRShiftType; //PSR Shift Inspection Mode
        public int CACount; //TOP 스캔횟수
        public int BACount; //BOT 스캔횟수
        public int BPCount; //BOT 스캔횟수
        //public int FullCount; //전체 스캔횟수
        public int CAResultImageCount; //TopResultImage 갯수
        public int BAResultImageCount;
        public int BPResultImageCount;
        public int CA_PC_Count; //PC 갯수
        public int BA_PC_Count;
        public int BP_PC_Count;
      

        private string m_Path;
        public Jobs(string astrPath)
        {
            m_Path = astrPath;
        }

        public bool Load()
        {
            if (!File.Exists(m_Path))
            {
                FileStream fs = File.Create(m_Path);
                fs.Close();
                Save();
                return false;
            }
            IniFile ini = new IniFile(m_Path);
            ProcessCode = ini.ReadString("JOB", "ProcessCode", "VI92");
            Customer = ini.ReadInteger("JOB", "Customer", 0);
            LastGroup = ini.ReadInteger("JOB", "LastGroup", 0);
            LastModel = ini.ReadString("JOB", "LastModel", "");
            LastLot = ini.ReadString("JOB", "LastLot", "");
            LastItsLot = ini.ReadString("JOB", "LastItsLot", "");
            LastUser = ini.ReadString("JOB", "LastUser", "");
            LastVerify = ini.ReadString("JOB", "LastVerify", "");
            AutoNG = ini.ReadString("JOB", "AutoNG", "1,1,1,1,1,1,1,1");
            PriorityNG = ini.ReadString("JOB", "PriorityNG", "19,6,4,9,10,11,12,13,14,16,17,5,15,8,7,3,0,1,2,18");
            Cam1Position = ini.ReadDouble("JOB","Cam1Position", 10);
            Cam1Pitch = ini.ReadDouble("JOB", "Cam1Pitch", 10);
            ScanVelocity1 = ini.ReadInteger("JOB", "ScanVelocity1", 300);
            ScanVelocity2 = ini.ReadInteger("JOB", "ScanVelocity2", 300);
            PSRShiftType = ini.ReadInteger("JOB", "PSR_Shift_Type", 0);
            CACount = ini.ReadInteger("JOB", "CACount", 2);
            BACount = ini.ReadInteger("JOB", "BACount", 2);
            BPCount = ini.ReadInteger("JOB", "BPCount", 1);
            //FullCount = ini.ReadInteger("JOB", "FullCount", 2);
            CAResultImageCount = ini.ReadInteger("JOB", "CAResultImageCount", 6);
            BAResultImageCount = ini.ReadInteger("JOB", "BAResultImageCount", 6);
            BPResultImageCount = ini.ReadInteger("JOB", "BPResultImageCount", 6);
            CA_PC_Count = ini.ReadInteger("JOB", "CA_PC_Count", 2);
            BA_PC_Count = ini.ReadInteger("JOB", "BA_PC_Count", 2);
            BP_PC_Count = ini.ReadInteger("JOB", "BP_PC_Count", 1);
            return true;
        }

        public void Save()
        {
            if (!File.Exists(m_Path))
            {
                FileStream fs = File.Create(m_Path);
                fs.Close();
            }
            IniFile ini = new IniFile(m_Path);
            ini.WriteString("JOB", "ProcessCode", ProcessCode);
            ini.WriteInteger("JOB", "Customer", Customer);
            ini.WriteInteger("JOB", "LastGroup", LastGroup);
            ini.WriteString("JOB", "LastModel", LastModel);
            ini.WriteString("JOB", "LastLot", LastLot);
            ini.WriteString("JOB", "LastItsLot", LastItsLot);
            ini.WriteString("JOB", "LastUser", LastUser);
            ini.WriteString("JOB", "LastVerify", LastVerify);
            ini.WriteString("JOB", "AutoNG", AutoNG);
            ini.WriteString("JOB", "PriorityNG", PriorityNG);
            ini.WriteDouble("JOB", "Cam1Position", Cam1Position);
            ini.WriteDouble("JOB", "Cam1Pitch", Cam1Pitch);
            ini.WriteInteger("JOB", "ScanVelocity1", ScanVelocity1);
            ini.WriteInteger("JOB", "ScanVelocity2", ScanVelocity2);
            ini.WriteInteger("JOB", "PSR_Shift_Type", PSRShiftType);
            ini.WriteInteger("JOB", "CACount", CACount);
            ini.WriteInteger("JOB", "BACount", BACount);
            ini.WriteInteger("JOB", "BPCount", BPCount);            
            //ini.WriteInteger("JOB", "FullCount", FullCount);
            ini.WriteInteger("JOB", "CAResultImageCount", CAResultImageCount);
            ini.WriteInteger("JOB", "BAResultImageCount", BAResultImageCount);
            ini.WriteInteger("JOB", "BPResultImageCount", BPResultImageCount);
            ini.WriteInteger("JOB", "CA_PC_Count", CA_PC_Count);
            ini.WriteInteger("JOB", "BA_PC_Count", BA_PC_Count);
            ini.WriteInteger("JOB", "BP_PC_Count", BP_PC_Count);
        }
    }

    public class ISPara
    {
        public int[] FGType = new int[5];               //Camera Frame GrabberType
        public int[] CameraWidth = new int[3];          //Camera Sensor Size
        public int[] CameraHeight = new int[3];         //Camera Height
        public string[] DeviceName = new string[5];        //Frame Grabber Name
        public string[] CamFile = new string[5];           //Camera Config File Name
        public int TestID;                 //Test vision ID
        public bool UseCASlave;              //Camera Slave Use
        public bool UseBASlave;
        public bool UseFocus;
        public bool[] VisionFlipX = new bool[3];              //Camera  Flip X
        public string[] IP = new string[5];                //IS IP
        public int[] Port = new int[5];                 // IS Port
        public double[] CamResolutionX = new double[3];    //Camara Resolution X
        public double[] CamResolutionY = new double[3];    //Camara Resolution Y
        public int[] CamPageDelay = new int[3];         //Camera Grabing Delay
        public float[] R_Gain = new float[3];            //Camera Gsin
        public float[] G_Gain = new float[3];
        public float[] B_Gain = new float[3];
        public float[] Strenth = new float[3];           //Camera Strenth
        public double ReScale;
    }

    public class PLCPara
    {
        public bool UsePLC;               //PLC 연결 사용
        public int PLCType;               //PLC Ethernet Type
        public string IP;                 //PLC IP
        public int Port;                  //PLC Port
        public int GoodID;                //Good ID
        public int NGID;                  //NG ID
        public int MCType;         // 1 : 성우, 0 : JS
    }

    public class PLC_Address
    {
        #region Address
        public String READ_MODE = "D2000";        //운전모드:1(수동),2(설정),3(자동,TEST)//
        public String READ_WARNING = "M2020";     //plc 알람//
        public String REQ_READY = "M2008";//

        public String REQ_BOATCA = "M8100";        //검사보트1 스캔요구
        public String REQ_BOATBA = "M8101";        //검사보트2 스캔요구
        public String RED_BOATCA = "D8100";        //CA 스캔 위치
        public String RED_BOATBA = "D8102";        //BA 스캔 위치
        public String PASS_BOATCA = "M8110";       //보트1 스캔 시작
        public String PASS_BOATBA = "M8111";       //보트2 검사 시작

        public String RED_BOAT2_CAM = "D8152";         //캠 이송 위치

        public String CAM1_POS = "D8030";         //cam 이송 원점
        public String CAM1_PITCH = "D8032";       //cam 이송 pitch

        public String REQ_RESULT = "M8102";       //검사결과 요구
        public String WRITE_RESULT = "D8104";     //결과 1:마킹  5:폐기
        public String RESULT_DONE = "M8112";      //결과 전송완료

        public String REQ_ALIGN1 = "M8120";        //얼라인 보트1 요구 
        public String REQ_ALIGN2 = "M8121";        //얼라인 보트2 요구
        public String READ_CAM_1 = "D8120";         //얼라인 카메라 현재위치 (1자리)
        public String READ_CAM_2 = "D8121";         //얼라인 카메라 현재위치 (소숫점자리)
        public String LASER_LOC = "D8122";        //현재 얼라인 위치 확인(우측:1 좌측:2)
        public String PASS_ALIGN1 = "M8130";       //레이저 보트1 얼라인 완료
        public String PASS_ALIGN2 = "M8131";       //레이저 보트2 얼라인 완료

        public String REQ_LASER1 = "M8140";        //레이져 요구        
        public String REQ_LASER2 = "M8141";        //레이져 요구

        public String READ_BOAT1 = "D8140";      //레이저보트 현재 위치 (1)
        public String READ_BOAT2 = "D8142";      //레이저보트 현재 위치 (X1000)
        public String PASS_LASER1 = "M8150";       //레이저1 완료
        public String PASS_LASER2 = "M8151";       //레이저2 완료

        public String REQ_LIGHT = "M2008";        //조명대기 요구//
        public String READ_POS = "D2024";//

        public String LASER_BYPASS1 = "M8152";    //레이저1 마킹안하고 통과
        public String LASER_BYPASS2 = "M8153";    //레이저2 마킹안하고 통과

        public String WRITE_MODE = "D2030";       //운전 모드 변경 X//
        public String CONNECT_PC = "M2030";       //프로그램 시작 연결시 //
        public String SET_WARNING = "M1559";      //PC 알람//
        public String SET_ALIGN_POS = "M2042";    //얼라인 위치 저장//
        public String REQ_DONE = "M2106";       //검사결과 요구//

        public String ALIGN_POS_X = "D2044";      //레이저 얼라인 위치//
        public String ALIGN_POS_Y = "D2047";//

        public String MODEL_HEIGHT = "D8000";     //모델 길이
        public String MODEL_WIDTH = "D8002";      //모델 폭
        public String MODEL_THICK = "D8004";      //모델 두께
        public String TOP_COUNT = "D8006";      //상부 스캔횟수
        public String BOT_COUNT = "D8008";      //하부 스캔횟수
        public String LASER_STEP = "D8010";
        public String LASER_PITCH = "D8012";
        //위치 쓰기용
        public String ALIGN_YPOS = "D8014";       //얼라인 카메라 위치
        public String ALIGN_XPOS11 = "D8016";     //마킹 보트 오른쪽 얼라인 1자리 위치
        public String ALIGN_XPOS12 = "D8017";     //마킹 보트 오른쪽 얼라인 소숫점 아래자리 위치
        public String ALIGN_XPOS21 = "D8018";     //마킹 보트 왼쪽 얼라인 1자리 위치
        public String ALIGN_XPOS22 = "D8019";     //마킹 보트 왼쪽 얼라인 소숫점 아래자리 위치
        public String MODEL_SEND_DONE = "M8000";  //모델정보 전송 완료

        public String ID_USED = "D8026";          // ID Reader 사용 유무 0: 미사용 1: 좌측 2: 우측
        public String REQ_ID = "M8013";          // ID Reader 요구
        public String PASS_ID = "M8113";          // PASS

        public String WRITE_LASER = "M8010";      //레이저 마킹을 위한 얼라인 카메라 이동 요구
        public String LASER_CORR = "D2065"; //


        public String STRIP_ID = "D8050";     //검사 시작하는 제품의 ID
        public String BOAT2_ID = "D8052";     //검사보트2에 검사 진행하는 제품의 ID
        public String RESULT_ID = "D8054";    //검사완료후 대기중인 제품의 ID 넘버
        public String LASER1_ID = "D8056";    //마킹보트1에 마킹 진행하는 제품의 ID 넘버
        public String LASER2_ID = "D8058";    //마킹보트2에 마킹 진행하는 제품의 ID 넘버

        public String TOP_SPEED = "D8006";    //상부 스캔속도
        public String BOT_SPEED = "D8008";    //하부 스캔속도

        public String REQ_END = "M1410";   //로더 투입 완료 신호
        
        public String MARKING_STAGE_SHIFT1 = "D8060";   //마킹 보트 Offset
        public String MARKING_STAGE_SHIFT2 = "D8062";   //마킹 보트 Offset
        #endregion

        private string m_Path;
        private string m_MC;

        public int Mode;
        public PLC_Address(string astrPath, string aszMachineName)
        {
            m_Path = astrPath;
            m_MC = aszMachineName;
            if (m_MC == "BAV04") Mode = 0;
            else if (m_MC == "BAV11" || m_MC == "BAV12" || m_MC == "BAV13") Mode = 2;
            else Mode = 1;
            Load();
        }

        private void Load()
        {
            if (!File.Exists(m_Path))
            {
                FileStream fs = File.Create(m_Path);
                fs.Close();
                if (m_MC == "BAV04") Save04();
                else if (m_MC == "BAV11" || m_MC == "BAV12" || m_MC == "BAV13") Save11();
                else Save01();
            }
            IniFile ini = new IniFile(m_Path);

            READ_MODE = ini.ReadString("PLC", "READ_MODE", "D2000");        
            READ_WARNING = ini.ReadString("PLC", "READ_WARNING", "M2020");     
            REQ_READY = ini.ReadString("PLC", "REQ_READY", "M2008");

            REQ_BOATCA = ini.ReadString("PLC", "REQ_BOAT1", "M8100");        
            REQ_BOATBA = ini.ReadString("PLC", "REQ_BOAT2", "M8101");        
            RED_BOATCA = ini.ReadString("PLC", "RED_BOAT1", "D8100");        
            RED_BOATBA = ini.ReadString("PLC", "RED_BOAT2", "D8102");        
            PASS_BOATCA = ini.ReadString("PLC", "PASS_BOAT1", "M8110");       
            PASS_BOATBA = ini.ReadString("PLC", "PASS_BOAT2", "M8111");       

            RED_BOAT2_CAM = ini.ReadString("PLC", "RED_BOAT2_CAM", "D8152");

            CAM1_POS = ini.ReadString("PLC", "CAM1_POS", "D8030");         
            CAM1_PITCH = ini.ReadString("PLC", "CAM1_PITCH", "D8032");       

            REQ_RESULT = ini.ReadString("PLC", "REQ_RESULT", "M8102");            
            WRITE_RESULT = ini.ReadString("PLC", "WRITE_RESULT", "D8104");    
            RESULT_DONE = ini.ReadString("PLC", "RESULT_DONE", "M8112");     

            REQ_ALIGN1 = ini.ReadString("PLC", "REQ_ALIGN1", "M8120");      
            REQ_ALIGN2 = ini.ReadString("PLC", "REQ_ALIGN2", "M8121");      
            READ_CAM_1 = ini.ReadString("PLC", "READ_CAM_1", "D8120");
            READ_CAM_2 = ini.ReadString("PLC", "READ_CAM_2", "D8121");
            LASER_LOC = ini.ReadString("PLC", "LASER_LOC", "D8122");       
            PASS_ALIGN1 = ini.ReadString("PLC", "PASS_ALIGN1", "M8130");     
            PASS_ALIGN2 = ini.ReadString("PLC", "PASS_ALIGN2", "M8131");     

            REQ_LASER1 = ini.ReadString("PLC", "REQ_LASER1", "M8140");      
            REQ_LASER2 = ini.ReadString("PLC", "REQ_LASER2", "M8141");      

            READ_BOAT1 = ini.ReadString("PLC", "READ_BOAT1", "D8140");      
            READ_BOAT2 = ini.ReadString("PLC", "READ_BOAT2", "D8142");      
            PASS_LASER1 = ini.ReadString("PLC", "PASS_LASER1", "M8150");     
            PASS_LASER2 = ini.ReadString("PLC", "PASS_LASER2", "M8151");     

            REQ_LIGHT = ini.ReadString("PLC", "REQ_LIGHT", "M2008");       
            READ_POS = ini.ReadString("PLC", "READ_POS", "D2024");

            LASER_BYPASS1 = ini.ReadString("PLC", "LASER_BYPASS1", "M8152");   
            LASER_BYPASS2 = ini.ReadString("PLC", "LASER_BYPASS2", "M8153");   

            WRITE_MODE = ini.ReadString("PLC", "WRITE_MODE", "D2030");      
            CONNECT_PC = ini.ReadString("PLC", "CONNECT_PC", "M2030");      
            SET_WARNING = ini.ReadString("PLC", "SET_WARNING", "M1559");     
            SET_ALIGN_POS = ini.ReadString("PLC", "SET_ALIGN_POS", "M2042");   
            REQ_DONE = ini.ReadString("PLC", "REQ_DONE", "M2106");       

            ALIGN_POS_X = ini.ReadString("PLC", "ALIGN_POS_X", "D2044");     
            ALIGN_POS_Y = ini.ReadString("PLC", "ALIGN_POS_Y", "D2047");

            MODEL_HEIGHT = ini.ReadString("PLC", "MODEL_HEIGHT", "D8000");    
            MODEL_WIDTH = ini.ReadString("PLC", "MODEL_WIDTH", "D8002");     
            MODEL_THICK = ini.ReadString("PLC", "MODEL_THICK", "D8004");     
            TOP_COUNT = ini.ReadString("PLC", "TOP_COUNT", "D8006");     
            BOT_COUNT = ini.ReadString("PLC", "BOT_COUNT", "D8008");
            TOP_SPEED = ini.ReadString("PLC", "TOP_SPEED", "D0000");
            BOT_SPEED = ini.ReadString("PLC", "BOT_SPEED", "D0000");
            LASER_STEP = ini.ReadString("PLC", "LASER_STEP", "D8010");
            LASER_PITCH = ini.ReadString("PLC", "LASER_PITCH", "D8012");

            ALIGN_YPOS = ini.ReadString("PLC", "ALIGN_YPOS", "D8014");      
            ALIGN_XPOS11 = ini.ReadString("PLC", "ALIGN_XPOS11", "D8016");    
            ALIGN_XPOS12 = ini.ReadString("PLC", "ALIGN_XPOS12", "D8017");    
            ALIGN_XPOS21 = ini.ReadString("PLC", "ALIGN_XPOS21", "D8018");    
            ALIGN_XPOS22 = ini.ReadString("PLC", "ALIGN_XPOS22", "D8019");    
            MODEL_SEND_DONE = ini.ReadString("PLC", "MODEL_SEND_DONE", "M8000"); 

            ID_USED = ini.ReadString("PLC", "ID_USED", "D8026");         
            REQ_ID = ini.ReadString("PLC", "REQ_ID", "M8013");          
            PASS_ID = ini.ReadString("PLC", "PASS_ID", "M8113");         

            WRITE_LASER = ini.ReadString("PLC", "WRITE_LASER", "M8010");      
            LASER_CORR = ini.ReadString("PLC", "LASER_CORR", "D2065"); 


            STRIP_ID = ini.ReadString("PLC", "STRIP_ID", "D8050"); 
            BOAT2_ID = ini.ReadString("PLC", "BOAT2_ID", "D8052"); 
            RESULT_ID = ini.ReadString("PLC", "RESULT_ID", "D8054");
            LASER1_ID = ini.ReadString("PLC", "LASER1_ID", "D8056");
            LASER2_ID = ini.ReadString("PLC", "LASER2_ID", "D8058");

            REQ_END = ini.ReadString("PLC", "REQ_END", "M1410");

            MARKING_STAGE_SHIFT1 = ini.ReadString("PLC", "MARKING_STAGE_SHIFT1", "D8060");
            MARKING_STAGE_SHIFT2 = ini.ReadString("PLC", "MARKING_STAGE_SHIFT2", "D8062");
        }
        private void Save04()
        {
            if (!File.Exists(m_Path))
            {
                FileStream fs = File.Create(m_Path);
                fs.Close();
            }
            IniFile ini = new IniFile(m_Path);
            ini.WriteString("PLC", "READ_MODE", "D2000");
            ini.WriteString("PLC", "READ_WARNING", "M2020");
            ini.WriteString("PLC", "REQ_READY", "M2008");

            ini.WriteString("PLC", "REQ_BOAT2", "M8100");
            ini.WriteString("PLC", "REQ_BOAT1", "M8101");
            ini.WriteString("PLC", "RED_BOAT2", "D8100");
            ini.WriteString("PLC", "RED_BOAT1", "D8102");
            ini.WriteString("PLC", "PASS_BOAT2", "M8110");
            ini.WriteString("PLC", "PASS_BOAT1", "M8111");

            ini.WriteString("PLC", "RED_BOAT2_CAM", "D8152");

            ini.WriteString("PLC", "CAM1_POS", "D8030");
            ini.WriteString("PLC", "CAM1_PITCH", "D8032");

            ini.WriteString("PLC", "REQ_RESULT", "M8102");
            ini.WriteString("PLC", "WRITE_RESULT", "D8104");
            ini.WriteString("PLC", "RESULT_DONE", "M8112");

            ini.WriteString("PLC", "REQ_ALIGN1", "M8120");
            ini.WriteString("PLC", "REQ_ALIGN2", "M8121");
            ini.WriteString("PLC", "READ_CAM_1", "D8120");
            ini.WriteString("PLC", "READ_CAM_2", "D8121");
            ini.WriteString("PLC", "LASER_LOC", "D8122");
            ini.WriteString("PLC", "PASS_ALIGN1", "M8130");
            ini.WriteString("PLC", "PASS_ALIGN2", "M8131");

            ini.WriteString("PLC", "REQ_LASER1", "M8140");
            ini.WriteString("PLC", "REQ_LASER2", "M8141");

            ini.WriteString("PLC", "READ_BOAT1", "D8140");
            ini.WriteString("PLC", "READ_BOAT2", "D8142");
            ini.WriteString("PLC", "PASS_LASER1", "M8150");
            ini.WriteString("PLC", "PASS_LASER2", "M8151");

            ini.WriteString("PLC", "REQ_LIGHT", "M2008");
            ini.WriteString("PLC", "READ_POS", "D2024");

            ini.WriteString("PLC", "LASER_BYPASS1", "M8152");
            ini.WriteString("PLC", "LASER_BYPASS2", "M8153");

            ini.WriteString("PLC", "WRITE_MODE", "D2030");
            ini.WriteString("PLC", "CONNECT_PC", "M2030");
            ini.WriteString("PLC", "SET_WARNING", "M2021");
            ini.WriteString("PLC", "SET_ALIGN_POS", "M2042");
            ini.WriteString("PLC", "REQ_DONE", "M2106");

            ini.WriteString("PLC", "ALIGN_POS_X", "D2044");
            ini.WriteString("PLC", "ALIGN_POS_Y", "D2047");

            ini.WriteString("PLC", "MODEL_HEIGHT", "D8000");
            ini.WriteString("PLC", "MODEL_WIDTH", "D8002");
            ini.WriteString("PLC", "MODEL_THICK", "D8004");
            
            ini.WriteString("PLC", "LASER_STEP", "D8010");
            ini.WriteString("PLC", "LASER_PITCH", "D8012");

            ini.WriteString("PLC", "ALIGN_YPOS", "D8014");
            ini.WriteString("PLC", "ALIGN_XPOS11", "D8016");
            ini.WriteString("PLC", "ALIGN_XPOS12", "D8017");
            ini.WriteString("PLC", "ALIGN_XPOS21", "D8018");
            ini.WriteString("PLC", "ALIGN_XPOS22", "D8019");
            ini.WriteString("PLC", "MODEL_SEND_DONE", "M8000");

            ini.WriteString("PLC", "ID_USED", "D8026");
            ini.WriteString("PLC", "REQ_ID", "M8013");
            ini.WriteString("PLC", "PASS_ID", "M8113");

            ini.WriteString("PLC", "WRITE_LASER", "M8010");
            ini.WriteString("PLC", "LASER_CORR", "D2065");

            ini.WriteString("PLC", "STRIP_ID", "D8050");
            ini.WriteString("PLC", "BOAT2_ID", "D8052");
            ini.WriteString("PLC", "RESULT_ID", "D8054");
            ini.WriteString("PLC", "LASER1_ID", "D8056");
            ini.WriteString("PLC", "LASER2_ID", "D8058");

            ini.WriteString("PLC", "TOP_COUNT", "D8020");
            ini.WriteString("PLC", "BOT_COUNT", "D8022");

            ini.WriteString("PLC", "TOP_SPEED", "D8006");
            ini.WriteString("PLC", "BOT_SPEED", "D8008");

            ini.WriteString("PLC", "REQ_END", "M1410");

            ini.WriteString("PLC", "MARKING_STAGE_SHIFT1", "D8060");
            ini.WriteString("PLC", "MARKING_STAGE_SHIFT2", "D8062");
        }

        private void Save11()
        {
            if (!File.Exists(m_Path))
            {
                FileStream fs = File.Create(m_Path);
                fs.Close();
            }
            IniFile ini = new IniFile(m_Path);
            ini.WriteString("PLC", "READ_MODE", "D2020");
            ini.WriteString("PLC", "READ_WARNING", "M2020");
            ini.WriteString("PLC", "REQ_READY", "M2008");

            ini.WriteString("PLC", "REQ_BOAT1", "M2000");
            ini.WriteString("PLC", "REQ_BOAT2", "M2001");
            ini.WriteString("PLC", "RED_BOAT1", "D2028");
            ini.WriteString("PLC", "RED_BOAT2", "D2030");
            ini.WriteString("PLC", "PASS_BOAT1", "M2002");
            ini.WriteString("PLC", "PASS_BOAT2", "M2003");

            ini.WriteString("PLC", "RED_BOAT2_CAM", "D0000");

            ini.WriteString("PLC", "CAM1_POS", "D2090");
            ini.WriteString("PLC", "CAM1_PITCH", "D2091");

            ini.WriteString("PLC", "REQ_RESULT", "M2005");
            ini.WriteString("PLC", "WRITE_RESULT", "D2024");
            ini.WriteString("PLC", "RESULT_DONE", "M2006");

            ini.WriteString("PLC", "REQ_ALIGN1", "M2016");
            ini.WriteString("PLC", "REQ_ALIGN2", "M0000");
            ini.WriteString("PLC", "READ_CAM_1", "D2036");
            ini.WriteString("PLC", "READ_CAM_2", "D0000");
            ini.WriteString("PLC", "LASER_LOC", "D2037");
            ini.WriteString("PLC", "PASS_ALIGN1", "M2017");
            ini.WriteString("PLC", "PASS_ALIGN2", "M0000");

            ini.WriteString("PLC", "REQ_LASER1", "M2009");
            ini.WriteString("PLC", "REQ_LASER2", "M0000");

            ini.WriteString("PLC", "READ_BOAT1", "D2010");
            ini.WriteString("PLC", "READ_BOAT2", "D2011");
            ini.WriteString("PLC", "PASS_LASER1", "M2010");
            ini.WriteString("PLC", "PASS_LASER2", "M0000");

            ini.WriteString("PLC", "REQ_LIGHT", "M0000");
            ini.WriteString("PLC", "READ_POS", "D0000");

            ini.WriteString("PLC", "LASER_BYPASS1", "M0000");
            ini.WriteString("PLC", "LASER_BYPASS2", "M0000");

            ini.WriteString("PLC", "WRITE_MODE", "D0000");
            ini.WriteString("PLC", "CONNECT_PC", "M0000");
            ini.WriteString("PLC", "SET_WARNING", "M0000");
            ini.WriteString("PLC", "SET_ALIGN_POS", "M2013");
            ini.WriteString("PLC", "REQ_DONE", "M0000");

            ini.WriteString("PLC", "ALIGN_POS_X", "D0000");
            ini.WriteString("PLC", "ALIGN_POS_Y", "D0000");

            ini.WriteString("PLC", "MODEL_HEIGHT", "D2040");
            ini.WriteString("PLC", "MODEL_WIDTH", "D2042");
            ini.WriteString("PLC", "MODEL_THICK", "D0000");
            ini.WriteString("PLC", "TOP_COUNT", "D2058");
            ini.WriteString("PLC", "BOT_COUNT", "D2060");
            ini.WriteString("PLC", "LASER_STEP", "D2072");
            ini.WriteString("PLC", "LASER_PITCH", "D2074");

            ini.WriteString("PLC", "ALIGN_YPOS", "D2054");
            ini.WriteString("PLC", "ALIGN_XPOS11", "D2076");
            ini.WriteString("PLC", "ALIGN_XPOS12", "D2077");
            ini.WriteString("PLC", "ALIGN_XPOS21", "D2078");
            ini.WriteString("PLC", "ALIGN_XPOS22", "D2079");
            ini.WriteString("PLC", "MODEL_SEND_DONE", "M2011");

            ini.WriteString("PLC", "ID_USED", "D0000");
            ini.WriteString("PLC", "REQ_ID", "M0000");
            ini.WriteString("PLC", "PASS_ID", "M000");

            ini.WriteString("PLC", "WRITE_LASER", "M2013");
            ini.WriteString("PLC", "LASER_CORR", "D0000");

            ini.WriteString("PLC", "STRIP_ID", "D2102");
            ini.WriteString("PLC", "BOAT2_ID", "D2081");
            ini.WriteString("PLC", "RESULT_ID", "D2082");
            ini.WriteString("PLC", "LASER1_ID", "D2085");
            ini.WriteString("PLC", "LASER2_ID", "D0000");

            ini.WriteString("PLC", "TOP_COUNT", "D0000");
            ini.WriteString("PLC", "BOT_COUNT", "D0000");

            ini.WriteString("PLC", "TOP_SPEED", "D8006");
            ini.WriteString("PLC", "BOT_SPEED", "D8008");

            ini.WriteString("PLC", "REQ_END", "M1480");

            ini.WriteString("PLC", "MARKING_STAGE_SHIFT1", "D8060");
            ini.WriteString("PLC", "MARKING_STAGE_SHIFT2", "D8062");
        }

        private void Save01()
        {
            if (!File.Exists(m_Path))
            {
                FileStream fs = File.Create(m_Path);
                fs.Close();
            }
            IniFile ini = new IniFile(m_Path);
            ini.WriteString("PLC", "READ_MODE", "D2000");
            ini.WriteString("PLC", "READ_WARNING", "M2020");
            ini.WriteString("PLC", "REQ_READY", "M2008");

            ini.WriteString("PLC", "REQ_BOAT1", "M2100");
            ini.WriteString("PLC", "REQ_BOAT2", "M2110");
            ini.WriteString("PLC", "RED_BOAT1", "D2001");
            ini.WriteString("PLC", "RED_BOAT2", "D2002");
            ini.WriteString("PLC", "PASS_BOAT1", "M2500");
            ini.WriteString("PLC", "PASS_BOAT2", "M2510");

            ini.WriteString("PLC", "RED_BOAT2_CAM", "D0000");

            ini.WriteString("PLC", "CAM1_POS", "D2061");
            ini.WriteString("PLC", "CAM1_PITCH", "D2062");

            ini.WriteString("PLC", "REQ_RESULT", "M2005");
            ini.WriteString("PLC", "WRITE_RESULT", "D2100");
            ini.WriteString("PLC", "RESULT_DONE", "M2006");

            ini.WriteString("PLC", "REQ_ALIGN1", "M2025");
            ini.WriteString("PLC", "REQ_ALIGN2", "M2026");
            ini.WriteString("PLC", "READ_CAM_1", "D2005");
            ini.WriteString("PLC", "READ_CAM_2", "D0000");
            ini.WriteString("PLC", "LASER_LOC", "D2006");
            ini.WriteString("PLC", "PASS_ALIGN1", "M2525");
            ini.WriteString("PLC", "PASS_ALIGN2", "M2526");

            ini.WriteString("PLC", "REQ_LASER1", "M2120");
            ini.WriteString("PLC", "REQ_LASER2", "M2121");

            ini.WriteString("PLC", "READ_BOAT1", "D2003");
            ini.WriteString("PLC", "READ_BOAT2", "D2004");
            ini.WriteString("PLC", "PASS_LASER1", "M2520");
            ini.WriteString("PLC", "PASS_LASER2", "M2521");

            ini.WriteString("PLC", "REQ_LIGHT", "M2008");
            ini.WriteString("PLC", "READ_POS", "D2024");

            ini.WriteString("PLC", "LASER_BYPASS1", "M2527");
            ini.WriteString("PLC", "LASER_BYPASS2", "M2528");

            ini.WriteString("PLC", "WRITE_MODE", "D2030");
            ini.WriteString("PLC", "CONNECT_PC", "M2030");
            ini.WriteString("PLC", "SET_WARNING", "M2021");
            ini.WriteString("PLC", "SET_ALIGN_POS", "M2042");
            ini.WriteString("PLC", "REQ_DONE", "M2106");

            ini.WriteString("PLC", "ALIGN_POS_X", "D2044");
            ini.WriteString("PLC", "ALIGN_POS_Y", "D2047");

            ini.WriteString("PLC", "MODEL_HEIGHT", "D2050");
            ini.WriteString("PLC", "MODEL_WIDTH", "D2051");

            if(m_MC == "BAV07" || m_MC == "BAV08")
                ini.WriteString("PLC", "MODEL_THICK", "D2072");
            else
                ini.WriteString("PLC", "MODEL_THICK", "D0000");

            ini.WriteString("PLC", "TOP_COUNT", "D2060");
            ini.WriteString("PLC", "BOT_COUNT", "D2056");
            ini.WriteString("PLC", "LASER_STEP", "D2063");
            ini.WriteString("PLC", "LASER_PITCH", "D2064");

            ini.WriteString("PLC", "ALIGN_YPOS", "D2057");
            ini.WriteString("PLC", "ALIGN_XPOS11", "D2066");
            ini.WriteString("PLC", "ALIGN_XPOS12", "D2067");
            ini.WriteString("PLC", "ALIGN_XPOS21", "D2068");
            ini.WriteString("PLC", "ALIGN_XPOS22", "D2069");
            ini.WriteString("PLC", "MODEL_SEND_DONE", "M2002");

            if (m_MC == "BAV06")
            {
                ini.WriteString("PLC", "ID_USED", "M2610");
                ini.WriteString("PLC", "REQ_ID", "M2600");
                ini.WriteString("PLC", "PASS_ID", "M2601");
            }
            else
            {
                ini.WriteString("PLC", "ID_USED", "D0000");
                ini.WriteString("PLC", "REQ_ID", "M0000");
                ini.WriteString("PLC", "PASS_ID", "M000");
            }

            ini.WriteString("PLC", "WRITE_LASER", "M2015");
            ini.WriteString("PLC", "LASER_CORR", "D2065");

            ini.WriteString("PLC", "STRIP_ID", "D2102");
            ini.WriteString("PLC", "BOAT2_ID", "D2011");
            ini.WriteString("PLC", "RESULT_ID", "D2012");
            ini.WriteString("PLC", "LASER1_ID", "D2014");
            ini.WriteString("PLC", "LASER2_ID", "D2015");

            ini.WriteString("PLC", "TOP_SPEED", "D8006");
            ini.WriteString("PLC", "BOT_SPEED", "D8008");

            ini.WriteString("PLC", "REQ_END", "M1480");

            ini.WriteString("PLC", "MARKING_STAGE_SHIFT1", "D8060");
            ini.WriteString("PLC", "MARKING_STAGE_SHIFT2", "D8062");
        }
    }

    public class LaserPara
    {
        public bool UseLaser;            //Laser 연결 사용
        public bool DualLaser;           //2 Boat Laser 여부
        public string IP;                //Laser IP
        public string Port;                 //Laser Port
        public double CenterY;           //Laser Center Position Set
        public double CamResolution;     //Laser Align Camera Resolution
        public double Boat2OffsetY;      //Laser Second Boat to First Boat Y
        public double Boat2OffsetX;      //Laser Second Boat to First Boat X
        public double Boat2Angle;        //Laser Second Boat to First Boat Angle
        public double BoatAlignPos1;     //Laser Align Boat Right Position1
        public double BoatAlignPos2;     //Laser Align Boat Left Position2
        public double CamPosY;           //Laser Align Camera Y Position
        public int CamType;              //0: Picolo 1: GigE
    }

    public class LightPara
    {
        public bool UseLight;          //조명 사용 여부
        public int[] LightType = new int[3];          // 0: 1,3    1 : 5,6     2: 7,8    3: 11,12,13    4: 4,19,20,21,22,23,24
        public string[] ComPort = new string[3];
        public string[,] Channel = new string[3, 8];
    }

    public class Encoder
    {
        public bool UseEncoder;         //엔코더 사용 여부
        public int EncoderType;         //엔코더 타입 0: NI 1: SW
        public long[] Low = new long[3];               //NI Low Pulse 
        public long[] High = new long[3];              //NI Hight Pulse
        public long[] UseFilter = new long[3];        //NI Filter 사용 
        public double[] FilterTime = new double[3];     //NI Filter 간격
        public string[] Port = new string[2];           //SW Port CA 
        public int[] Direction = new int[2];         //SW Encoder CA Direction
        public int[] Count = new int[2];             //SW Encoder Count CA
        public int[] Delay1 = new int[2];             //SW Encoder Delay CA
        public int[] Delay2 = new int[2];             //SW Encoder Delay CA
        public int[] Width = new int[2];             //SW Encoder Width CA

    }

    public class DNN
    {
        public bool UseDNN;
        public string[] UseModels;
        public string Net_Path;
    }

}
