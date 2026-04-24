using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;
using System.IO;
using Setting_INI.Properties;

namespace Setting_INI
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        Setting setting;
        public MainWindow()
        {
            InitializeComponent();
            InitEvent();

        }

        private void InitEvent()
        {
            this.Loaded += MainWindow_Loaded;
            this.btnClose.Click += BtnClose_Click;
            this.btnSave.Click += BtnSave_Click;
            this.btnConvert.Click += BtnConvert_Click;
        }

        private void BtnConvert_Click(object sender, RoutedEventArgs e)
        {
            string path = setting.General.ModelPath;
           // path = "d:\\Model";
            string[] groups = Directory.GetDirectories(path);
            foreach(string group in groups)
            {
                string[] models = Directory.GetDirectories(group);
                foreach(string model in models)
                {
                    string[] files = Directory.GetFiles(model);
                    foreach(string file in files)
                    {
                        FileInfo f = new FileInfo(file);
                        string fn = f.Name;
                        if(fn.Contains("11-"))
                            File.Move(file, f.Directory + "\\" + fn.Replace("11-", "131-"));
                        if (fn.Contains("12-"))
                            File.Move(file, f.Directory + "\\" + fn.Replace("12-", "132-"));
                        if (fn.Contains("21-"))
                            File.Move(file, f.Directory + "\\" + fn.Replace("21-", "111-"));
                        if (fn.Contains("22-"))
                            File.Move(file, f.Directory + "\\" + fn.Replace("22-", "112-"));
                        if (fn.Contains("31-"))
                            File.Move(file, f.Directory + "\\" + fn.Replace("31-", "121-"));
                        if (fn.Contains("32-"))
                            File.Move(file, f.Directory + "\\" + fn.Replace("32-", "122-"));
                    }
                    files = Directory.GetFiles(model);
                    foreach (string file in files)
                    {
                        FileInfo f = new FileInfo(file);
                        string fn = f.Name;
                        if (fn.Contains("111-"))
                        {
                            string rs = fn.Replace("111-", "11-");
                            rs = rs.Replace("-R", "");
                            File.Move(file, f.Directory + "\\" + rs);
                        }
                        if (fn.Contains("112-"))
                        {
                            string rs = fn.Replace("112-", "12-");
                            rs = rs.Replace("-R", "");
                            File.Move(file, f.Directory + "\\" + rs);
                        }
                        if (fn.Contains("121-"))
                            File.Move(file, f.Directory + "\\" + fn.Replace("121-", "21-"));
                        if (fn.Contains("122-"))
                            File.Move(file, f.Directory + "\\" + fn.Replace("122-", "22-"));
                        if (fn.Contains("131-"))
                            File.Move(file, f.Directory + "\\" + fn.Replace("131-", "31-"));
                        if (fn.Contains("132-"))
                            File.Move(file, f.Directory + "\\" + fn.Replace("132-", "32-"));
                    }
                    try
                    {
                        files = Directory.GetFiles(model + "\\CenterLine");
                        foreach (string file in files)
                        {
                            if (file.Contains("11.txt"))
                                File.Move(file, file.Replace("11.txt", "131.txt"));
                            if (file.Contains("12.txt"))
                                File.Move(file, file.Replace("12.txt", "132.txt"));
                            if (file.Contains("21.txt"))
                                File.Move(file, file.Replace("21.txt", "111.txt"));
                            if (file.Contains("22.txt"))
                                File.Move(file, file.Replace("22.txt", "112.txt"));
                            if (file.Contains("31.txt"))
                                File.Move(file, file.Replace("31.txt", "121.txt"));
                            if (file.Contains("32.txt"))
                                File.Move(file, file.Replace("32.txt", "122.txt"));
                        }
                        files = Directory.GetFiles(model + "\\CenterLine");
                        foreach (string file in files)
                        {
                            if (file.Contains("111.txt"))
                                File.Move(file, file.Replace("111.txt", "11.txt"));
                            if (file.Contains("112.txt"))
                                File.Move(file, file.Replace("112.txt", "12.txt"));
                            if (file.Contains("121.txt"))
                                File.Move(file, file.Replace("121.txt", "21.txt"));
                            if (file.Contains("122.txt"))
                                File.Move(file, file.Replace("122.txt", "22.txt"));
                            if (file.Contains("131.txt"))
                                File.Move(file, file.Replace("131.txt", "31.txt"));
                            if (file.Contains("132.txt"))
                                File.Move(file, file.Replace("132.txt", "32.txt"));
                        }
                    }
                    catch { }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            setting = new Setting(Directory.GetCurrentDirectory() + "\\..\\Config");
            bool bFirst = false;
            if (!setting.Exists())
            {
                setting.SettingConversion();
                bFirst = true;
            }
            setting.Load();
            if(bFirst) 
                PCS.ELF.AVI.ModelManager.LightConversion(setting.General.MachineName, setting.General.DBIP, setting.General.DBPort);            
            Display();
        }

        private void Display()
        {
            #region MC
            txtMCCode.Text = setting.General.MachineCode;
            txtMCName.Text = setting.General.MachineName;
            txtMCType.Text = setting.General.MachineType.ToString();
            txtMCIP.Text = setting.General.MachineIP;
            txtModelPath.Text = setting.General.ModelPath;
            txtResultPath.Text = setting.General.ResultPath;
            txtPopPath.Text = setting.General.POP_Path;
            txtVerifyPath.Text = setting.General.VerifyInfoPath;
            txtIDPath.Text = setting.General.IDMarkPath;
            txtUsePW.Text = setting.General.UsePassword ? "1":"0";
            txtRejectRate.Text = setting.General.RejectRate.ToString();
            txtUnitLimit.Text = setting.General.VRSNGUnitLimit.ToString();
            txtuseIDReader.Text = setting.General.UseIDReader ? "1" : "0";
            txtIDReaderIP.Text = setting.General.IDReaderIP;
            txtResultImageSizeType.Text = setting.General.ResultImageSizeType ? "1" : "0";
            txtResultImageSize1.Text = setting.General.ResultImageSize1.ToString();
            txtResultImageSize2.Text = setting.General.ResultImageSize2.ToString();
            txtCornerImageSizeX.Text = setting.General.CornerImageSizeX_mm.ToString();
            txtCornerImageSizeY.Text = setting.General.CornerImageSizeY_mm.ToString();
            txtICSFilePath.Text = setting.General.ICSFilePath;

            txtICS_OFFSET_BP_X.Text = setting.General.ICS_OFFSET_BP_X.ToString();
            txtICS_OFFSET_BP_Y.Text = setting.General.ICS_OFFSET_BP_Y.ToString();
            txtICS_OFFSET_CA_X.Text = setting.General.ICS_OFFSET_CA_X.ToString();
            txtICS_OFFSET_CA_Y.Text = setting.General.ICS_OFFSET_CA_Y.ToString();
            txtICS_OFFSET_BA_X.Text = setting.General.ICS_OFFSET_BA_X.ToString();
            txtICS_OFFSET_BA_Y.Text = setting.General.ICS_OFFSET_BA_Y.ToString();

            #endregion
            #region DB
            txtUseDB.Text = setting.General.UseDB ? "1" : "0";
            txtDBIP.Text = setting.General.DBIP;
            txtDBPort.Text = setting.General.DBPort;
            txtUseRVS.Text = setting.General.UseRVS ? "1" : "0";
            txtRVSIP.Text = setting.General.RVSIP;
            txtRVSPort.Text = setting.General.RVSPort;
            txtVRSDB.Text = setting.General.UseVRSDB ? "1" : "0";
            txtVRSDBIP.Text = setting.General.VRSDBIP;
            txtVRSDBPort.Text = setting.General.VRSDBPort;
            txtAVITN.Text = setting.General.VRSAVITableName;
            txtBCTN.Text = setting.General.VRSBinCodeTableName;
            #endregion
            #region ITS
            txtUseITS.Text = setting.General.UseITS ? "1" : "0";
            txtITSPath1.Text = setting.General.ITSPath1;
            txtITSPath2.Text = setting.General.ITSPath2;
            txtITSPath3.Text = setting.General.ITSPath3;
            txtUseITSDB.Text = setting.General.UseITSDB ? "1" : "0";
            txtITSDBIP.Text = setting.General.ITSDBIP;
            txtITSDBPort.Text = setting.General.ITSDBPort;
            txtITSTN.Text = setting.General.ITSTableName;
            #endregion
            #region Log
            txtLogSave.Text = setting.General.LogSave ? "1" : "0";
            txtLevel.Text = setting.General.LogLevel.ToString();
            txtDisplay.Text = setting.General.LogDPLevel.ToString();
            txtDate.Text = setting.General.LogKeepDate.ToString();
            #endregion
            #region IS
            txtUseCASlave.Text = setting.SubSystem.IS.UseCASlave ? "1" : "0";
            txtUseBASlave.Text = setting.SubSystem.IS.UseCASlave ? "1" : "0";
            txtXF1.Text = setting.SubSystem.IS.VisionFlipX[0] ? "1" : "0";
            txtXF2.Text = setting.SubSystem.IS.VisionFlipX[1] ? "1" : "0";
            txtXF3.Text = setting.SubSystem.IS.VisionFlipX[2] ? "1" : "0";
            txtCS1.Text = setting.SubSystem.IS.CameraWidth[0].ToString();
            txtCS2.Text = setting.SubSystem.IS.CameraWidth[1].ToString();
            txtCS3.Text = setting.SubSystem.IS.CameraWidth[2].ToString();
            txtRX1.Text = setting.SubSystem.IS.CamResolutionX[0].ToString();
            txtRX2.Text = setting.SubSystem.IS.CamResolutionX[1].ToString();
            txtRX3.Text = setting.SubSystem.IS.CamResolutionX[2].ToString();
            txtRY1.Text = setting.SubSystem.IS.CamResolutionY[0].ToString();
            txtRY2.Text = setting.SubSystem.IS.CamResolutionY[1].ToString();
            txtRY3.Text = setting.SubSystem.IS.CamResolutionY[2].ToString();
            txtPD1.Text = setting.SubSystem.IS.CamPageDelay[0].ToString();
            txtPD2.Text = setting.SubSystem.IS.CamPageDelay[1].ToString();
            txtPD3.Text = setting.SubSystem.IS.CamPageDelay[2].ToString();
            txtRG1.Text = setting.SubSystem.IS.R_Gain[0].ToString();
            txtRG2.Text = setting.SubSystem.IS.R_Gain[1].ToString();
            txtRG3.Text = setting.SubSystem.IS.R_Gain[2].ToString();
            txtGG1.Text = setting.SubSystem.IS.G_Gain[0].ToString();
            txtGG2.Text = setting.SubSystem.IS.G_Gain[1].ToString();
            txtGG3.Text = setting.SubSystem.IS.G_Gain[2].ToString();
            txtBG1.Text = setting.SubSystem.IS.B_Gain[0].ToString();
            txtBG2.Text = setting.SubSystem.IS.B_Gain[1].ToString();
            txtBG3.Text = setting.SubSystem.IS.B_Gain[2].ToString();
            txtST1.Text = setting.SubSystem.IS.Strenth[0].ToString();
            txtST2.Text = setting.SubSystem.IS.Strenth[1].ToString();
            txtST3.Text = setting.SubSystem.IS.Strenth[2].ToString();
            txtFT1.Text = setting.SubSystem.IS.FGType[0].ToString();
            txtFT2.Text = setting.SubSystem.IS.FGType[1].ToString();
            txtFT3.Text = setting.SubSystem.IS.FGType[2].ToString();
            txtFT4.Text = setting.SubSystem.IS.FGType[3].ToString();
            txtFT5.Text = setting.SubSystem.IS.FGType[4].ToString();
            txtDN1.Text = setting.SubSystem.IS.DeviceName[0].ToString();
            txtDN2.Text = setting.SubSystem.IS.DeviceName[1].ToString();
            txtDN3.Text = setting.SubSystem.IS.DeviceName[2].ToString();
            txtDN4.Text = setting.SubSystem.IS.DeviceName[3].ToString();
            txtDN5.Text = setting.SubSystem.IS.DeviceName[4].ToString();
            txtCF1.Text = setting.SubSystem.IS.CamFile[0].ToString();
            txtCF2.Text = setting.SubSystem.IS.CamFile[1].ToString();
            txtCF3.Text = setting.SubSystem.IS.CamFile[2].ToString();
            txtCF4.Text = setting.SubSystem.IS.CamFile[3].ToString();
            txtCF5.Text = setting.SubSystem.IS.CamFile[4].ToString();
            txtVI1.Text = setting.SubSystem.IS.IP[0].ToString();
            txtVI2.Text = setting.SubSystem.IS.IP[1].ToString();
            txtVI3.Text = setting.SubSystem.IS.IP[2].ToString();
            txtVI4.Text = setting.SubSystem.IS.IP[3].ToString();
            txtVI5.Text = setting.SubSystem.IS.IP[4].ToString();
            txtVP1.Text = setting.SubSystem.IS.Port[0].ToString();
            txtVP2.Text = setting.SubSystem.IS.Port[1].ToString();
            txtVP3.Text = setting.SubSystem.IS.Port[2].ToString();
            txtVP4.Text = setting.SubSystem.IS.Port[3].ToString();
            txtVP5.Text = setting.SubSystem.IS.Port[4].ToString();
            #endregion
            #region PLC
            txtUsePLC.Text = setting.SubSystem.PLC.UsePLC ? "1" : "0";
            txtPLCType.Text = setting.SubSystem.PLC.PLCType.ToString();
            txtPLCIP.Text = setting.SubSystem.PLC.IP;
            txtPLCPort.Text = setting.SubSystem.PLC.Port.ToString();
            txtGood.Text = setting.SubSystem.PLC.GoodID.ToString();
            txtBad.Text = setting.SubSystem.PLC.NGID.ToString();
            #endregion
            #region Laser
            txtUseLaser.Text = setting.SubSystem.Laser.UseLaser ? "1" : "0";
            txtDualBoat.Text = setting.SubSystem.Laser.DualLaser ? "1" : "0";
            txtLaserIP.Text = setting.SubSystem.Laser.IP;
            txtLaserPort.Text = setting.SubSystem.Laser.Port.ToString();
            txtAlignCamType.Text = setting.SubSystem.Laser.CamType.ToString();
            #endregion
            #region Light
            txtUseLight.Text = setting.SubSystem.Light.UseLight ? "1" : "0";
            txtLightType.Text = setting.SubSystem.Light.LightType[0].ToString();
            txtLightType2.Text = setting.SubSystem.Light.LightType[1].ToString();
            txtLightType3.Text = setting.SubSystem.Light.LightType[2].ToString();
            txtPT1.Text = setting.SubSystem.Light.ComPort[0];
            txtPT2.Text = setting.SubSystem.Light.ComPort[1];
            txtPT3.Text = setting.SubSystem.Light.ComPort[2];
            txtBP1.Text = setting.SubSystem.Light.Channel[0, 0];
            txtBP2.Text = setting.SubSystem.Light.Channel[0, 1];
            txtBP3.Text = setting.SubSystem.Light.Channel[0, 2];
            txtBP4.Text = setting.SubSystem.Light.Channel[0, 3];
            txtBP5.Text = setting.SubSystem.Light.Channel[0, 4];
            txtBP6.Text = setting.SubSystem.Light.Channel[0, 5];
            txtBP7.Text = setting.SubSystem.Light.Channel[0, 6];
            txtBP8.Text = setting.SubSystem.Light.Channel[0, 7];
            txtCA1.Text = setting.SubSystem.Light.Channel[1, 0];
            txtCA2.Text = setting.SubSystem.Light.Channel[1, 1];
            txtCA3.Text = setting.SubSystem.Light.Channel[1, 2];
            txtCA4.Text = setting.SubSystem.Light.Channel[1, 3];
            txtCA5.Text = setting.SubSystem.Light.Channel[1, 4];
            txtCA6.Text = setting.SubSystem.Light.Channel[1, 5];
            txtCA7.Text = setting.SubSystem.Light.Channel[1, 6];
            txtCA8.Text = setting.SubSystem.Light.Channel[1, 7];
            txtBA1.Text = setting.SubSystem.Light.Channel[2, 0];
            txtBA2.Text = setting.SubSystem.Light.Channel[2, 1];
            txtBA3.Text = setting.SubSystem.Light.Channel[2, 2];
            txtBA4.Text = setting.SubSystem.Light.Channel[2, 3];
            txtBA5.Text = setting.SubSystem.Light.Channel[2, 4];
            txtBA6.Text = setting.SubSystem.Light.Channel[2, 5];
            txtBA7.Text = setting.SubSystem.Light.Channel[2, 6];
            txtBA8.Text = setting.SubSystem.Light.Channel[2, 7];
            #endregion
            #region Encoder
            txtUseENC.Text = setting.SubSystem.ENC.UseEncoder ? "1" : "0";
            txtENCType.Text = setting.SubSystem.ENC.EncoderType.ToString();
            txtLow1.Text = setting.SubSystem.ENC.Low[0].ToString();
            txtLow2.Text = setting.SubSystem.ENC.Low[1].ToString();
            txtLow3.Text = setting.SubSystem.ENC.Low[2].ToString();
            txtHigh1.Text = setting.SubSystem.ENC.High[0].ToString();
            txtHigh2.Text = setting.SubSystem.ENC.High[1].ToString();
            txtHigh3.Text = setting.SubSystem.ENC.High[2].ToString();
            txtFilter1.Text = setting.SubSystem.ENC.UseFilter[0].ToString();
            txtFilter2.Text = setting.SubSystem.ENC.UseFilter[1].ToString();
            txtFilter3.Text = setting.SubSystem.ENC.UseFilter[2].ToString();
            txtTime1.Text = setting.SubSystem.ENC.FilterTime[0].ToString();
            txtTime2.Text = setting.SubSystem.ENC.FilterTime[1].ToString();
            txtTime3.Text = setting.SubSystem.ENC.FilterTime[2].ToString();

            txtENCPort1.Text = setting.SubSystem.ENC.Port[0];
            txtENCPort2.Text = setting.SubSystem.ENC.Port[1];
            txtENCDir1.Text = setting.SubSystem.ENC.Direction[0].ToString();
            txtENCDir2.Text = setting.SubSystem.ENC.Direction[1].ToString();
            txtENCCnt1.Text = setting.SubSystem.ENC.Count[0].ToString();
            txtENCCnt2.Text = setting.SubSystem.ENC.Count[1].ToString();
            txtENCDelay11.Text = setting.SubSystem.ENC.Delay1[0].ToString();
            txtENCDelay12.Text = setting.SubSystem.ENC.Delay1[1].ToString();
            txtENCDelay21.Text = setting.SubSystem.ENC.Delay2[0].ToString();
            txtENCDelay22.Text = setting.SubSystem.ENC.Delay2[1].ToString();
            txtENCWidth1.Text = setting.SubSystem.ENC.Width[0].ToString();
            txtENCWidth2.Text = setting.SubSystem.ENC.Width[1].ToString();
            #endregion
        }

        private void Save()
        {
            try
            {
                #region MC
                setting.General.MachineCode = txtMCCode.Text;
                setting.General.MachineName = txtMCName.Text;
                setting.General.MachineType = Convert.ToInt32(txtMCType.Text);
                setting.General.MachineIP = txtMCIP.Text;
                setting.General.ModelPath = txtModelPath.Text;
                setting.General.ResultPath = txtResultPath.Text;
                setting.General.POP_Path = txtPopPath.Text;
                setting.General.VerifyInfoPath = txtVerifyPath.Text;
                setting.General.IDMarkPath = txtIDPath.Text;
                txtUsePW.Text = setting.General.UsePassword ? "1" : "0";
                setting.General.RejectRate = Convert.ToDouble(txtRejectRate.Text);
                setting.General.VRSNGUnitLimit = Convert.ToInt32(txtUnitLimit.Text);
                setting.General.UseIDReader = (txtuseIDReader.Text == "1") ? true : false;
                setting.General.IDReaderIP = txtIDReaderIP.Text;
                setting.General.ResultImageSizeType = (txtResultImageSizeType.Text == "1") ? true : false;
                setting.General.ResultImageSize1 = Convert.ToInt32(txtResultImageSize1.Text);
                setting.General.ResultImageSize1 = Convert.ToInt32(txtResultImageSize1.Text);
                setting.General.CornerImageSizeX_mm = Convert.ToInt32(txtCornerImageSizeX.Text);
                setting.General.CornerImageSizeY_mm = Convert.ToInt32(txtCornerImageSizeY.Text);
                setting.General.ICSFilePath = txtICSFilePath.Text;
                setting.General.ICS_OFFSET_BP_X = Convert.ToInt32(txtICS_OFFSET_BP_X.Text);
                setting.General.ICS_OFFSET_BP_Y = Convert.ToInt32(txtICS_OFFSET_BP_Y.Text);
                setting.General.ICS_OFFSET_CA_X = Convert.ToInt32(txtICS_OFFSET_CA_X.Text);
                setting.General.ICS_OFFSET_CA_Y = Convert.ToInt32(txtICS_OFFSET_CA_Y.Text);
                setting.General.ICS_OFFSET_BA_X = Convert.ToInt32(txtICS_OFFSET_BA_X.Text);
                setting.General.ICS_OFFSET_BA_Y = Convert.ToInt32(txtICS_OFFSET_BA_Y.Text);
                #endregion
                #region DB
                setting.General.UseDB = (txtUseDB.Text == "1") ? true : false;
                setting.General.DBIP = txtDBIP.Text;
                setting.General.DBPort = txtDBPort.Text;
                setting.General.UseRVS = (txtUseRVS.Text == "1") ? true : false;
                setting.General.RVSIP = txtRVSIP.Text;
                setting.General.RVSPort = txtRVSPort.Text;
                setting.General.UseVRSDB = (txtVRSDB.Text == "1") ? true : false;
                setting.General.VRSDBIP = txtVRSDBIP.Text;
                setting.General.VRSDBPort = txtVRSDBPort.Text;
                setting.General.VRSAVITableName = txtAVITN.Text;
                setting.General.VRSBinCodeTableName = txtBCTN.Text;
                #endregion
                #region ITS
                setting.General.UseITS = (txtUseITS.Text == "1") ? true: false;
                setting.General.ITSPath1 = txtITSPath1.Text;
                setting.General.ITSPath2 = txtITSPath2.Text;
                setting.General.ITSPath3 = txtITSPath3.Text;
                setting.General.UseITSDB = (txtUseITSDB.Text == "1") ? true : false;
                setting.General.ITSDBIP = txtITSDBIP.Text;
                setting.General.ITSDBPort = txtITSDBPort.Text;
                setting.General.ITSTableName = txtITSTN.Text;
                #endregion
                #region Log
                setting.General.LogSave = (txtLogSave.Text == "1") ? true : false;
                setting.General.LogLevel = Convert.ToInt32(txtLevel.Text);
                setting.General.LogDPLevel = Convert.ToInt32(txtDisplay.Text);
                setting.General.LogKeepDate = Convert.ToInt32(txtDate.Text);
                #endregion
                #region IS
                setting.SubSystem.IS.UseCASlave = (txtUseCASlave.Text == "1") ? true : false;
                setting.SubSystem.IS.UseBASlave = (txtUseBASlave.Text == "1") ? true : false;
                setting.SubSystem.IS.VisionFlipX[0] = (txtXF1.Text == "1") ? true : false;
                setting.SubSystem.IS.VisionFlipX[1] = (txtXF2.Text == "1") ? true : false;
                setting.SubSystem.IS.VisionFlipX[2] = (txtXF3.Text == "1") ? true : false;

                setting.SubSystem.IS.CameraWidth[0] = Convert.ToInt32(txtCS1.Text);
                setting.SubSystem.IS.CameraWidth[1] = Convert.ToInt32(txtCS2.Text);
                setting.SubSystem.IS.CameraWidth[2] = Convert.ToInt32(txtCS3.Text);
                setting.SubSystem.IS.CamResolutionX[0] = Convert.ToDouble(txtRX1.Text);
                setting.SubSystem.IS.CamResolutionX[1] = Convert.ToDouble(txtRX2.Text);
                setting.SubSystem.IS.CamResolutionX[2] = Convert.ToDouble(txtRX3.Text);
                setting.SubSystem.IS.CamResolutionY[0] = Convert.ToDouble(txtRY1.Text);
                setting.SubSystem.IS.CamResolutionY[1] = Convert.ToDouble(txtRY2.Text);
                setting.SubSystem.IS.CamResolutionY[2] = Convert.ToDouble(txtRY3.Text);
                setting.SubSystem.IS.CamPageDelay[0] = Convert.ToInt32(txtPD1.Text);
                setting.SubSystem.IS.CamPageDelay[1] = Convert.ToInt32(txtPD2.Text);
                setting.SubSystem.IS.CamPageDelay[2] = Convert.ToInt32(txtPD3.Text);
                setting.SubSystem.IS.R_Gain[0] = (float)Convert.ToDouble(txtRG1.Text);
                setting.SubSystem.IS.R_Gain[1] = (float)Convert.ToDouble(txtRG2.Text);
                setting.SubSystem.IS.R_Gain[2] = (float)Convert.ToDouble(txtRG3.Text);
                setting.SubSystem.IS.G_Gain[0] = (float)Convert.ToDouble(txtGG1.Text);
                setting.SubSystem.IS.G_Gain[1] = (float)Convert.ToDouble(txtGG2.Text);
                setting.SubSystem.IS.G_Gain[2] = (float)Convert.ToDouble(txtGG3.Text);
                setting.SubSystem.IS.B_Gain[0] = (float)Convert.ToDouble(txtBG1.Text);
                setting.SubSystem.IS.B_Gain[1] = (float)Convert.ToDouble(txtBG2.Text);
                setting.SubSystem.IS.B_Gain[2] = (float)Convert.ToDouble(txtBG3.Text);
                setting.SubSystem.IS.Strenth[0] = (float)Convert.ToDouble(txtST1.Text);
                setting.SubSystem.IS.Strenth[1] = (float)Convert.ToDouble(txtST2.Text);
                setting.SubSystem.IS.Strenth[2] = (float)Convert.ToDouble(txtST3.Text);

                setting.SubSystem.IS.FGType[0] = Convert.ToInt32(txtFT1.Text);
                setting.SubSystem.IS.FGType[1] = Convert.ToInt32(txtFT2.Text);
                setting.SubSystem.IS.FGType[2] = Convert.ToInt32(txtFT3.Text);
                setting.SubSystem.IS.FGType[3] = Convert.ToInt32(txtFT4.Text);
                setting.SubSystem.IS.FGType[4] = Convert.ToInt32(txtFT5.Text);
                setting.SubSystem.IS.DeviceName[0] = txtDN1.Text;
                setting.SubSystem.IS.DeviceName[1] = txtDN2.Text;
                setting.SubSystem.IS.DeviceName[2] = txtDN3.Text;
                setting.SubSystem.IS.DeviceName[3] = txtDN4.Text;
                setting.SubSystem.IS.DeviceName[4] = txtDN5.Text;
                setting.SubSystem.IS.CamFile[0] = txtCF1.Text;
                setting.SubSystem.IS.CamFile[1] = txtCF2.Text;
                setting.SubSystem.IS.CamFile[2] = txtCF3.Text;
                setting.SubSystem.IS.CamFile[3] = txtCF4.Text;
                setting.SubSystem.IS.CamFile[4] = txtCF5.Text;
                setting.SubSystem.IS.IP[0] = txtVI1.Text;
                setting.SubSystem.IS.IP[1] = txtVI2.Text;
                setting.SubSystem.IS.IP[2] = txtVI3.Text;
                setting.SubSystem.IS.IP[3] = txtVI4.Text;
                setting.SubSystem.IS.IP[4] = txtVI5.Text;
                setting.SubSystem.IS.Port[0] = Convert.ToInt32(txtVP1.Text);
                setting.SubSystem.IS.Port[1] = Convert.ToInt32(txtVP2.Text);
                setting.SubSystem.IS.Port[2] = Convert.ToInt32(txtVP3.Text);
                setting.SubSystem.IS.Port[3] = Convert.ToInt32(txtVP4.Text);
                setting.SubSystem.IS.Port[4] = Convert.ToInt32(txtVP5.Text);
                #endregion
                #region PLC
                setting.SubSystem.PLC.UsePLC =  (txtUsePLC.Text == "1") ? true : false;
                setting.SubSystem.PLC.PLCType = Convert.ToInt32(txtPLCType.Text);
                setting.SubSystem.PLC.IP = txtPLCIP.Text;
                setting.SubSystem.PLC.Port = Convert.ToInt32(txtPLCPort.Text);
                setting.SubSystem.PLC.GoodID = Convert.ToInt32(txtGood.Text);
                setting.SubSystem.PLC.NGID = Convert.ToInt32(txtBad.Text);
                #endregion
                #region Laser
                setting.SubSystem.Laser.UseLaser = (txtUseLaser.Text == "1") ? true : false;
                setting.SubSystem.Laser.DualLaser = (txtDualBoat.Text == "1") ? true : false;
                setting.SubSystem.Laser.IP = txtLaserIP.Text;
                setting.SubSystem.Laser.Port = txtLaserPort.Text;
                setting.SubSystem.Laser.CamType = Convert.ToInt32(txtAlignCamType.Text);
                #endregion
                #region Light
                setting.SubSystem.Light.UseLight = (txtUseLight.Text == "1") ? true : false;
                setting.SubSystem.Light.LightType[0] = Convert.ToInt32(txtLightType.Text);
                setting.SubSystem.Light.LightType[1] = Convert.ToInt32(txtLightType2.Text);
                setting.SubSystem.Light.LightType[2] = Convert.ToInt32(txtLightType3.Text);
                setting.SubSystem.Light.ComPort[0] = txtPT1.Text;
                setting.SubSystem.Light.ComPort[1] = txtPT2.Text;
                setting.SubSystem.Light.ComPort[2] = txtPT3.Text;
                setting.SubSystem.Light.Channel[0, 0] = txtBP1.Text;
                setting.SubSystem.Light.Channel[0, 1] = txtBP2.Text;
                setting.SubSystem.Light.Channel[0, 2] = txtBP3.Text;
                setting.SubSystem.Light.Channel[0, 3] = txtBP4.Text;
                setting.SubSystem.Light.Channel[0, 4] = txtBP5.Text;
                setting.SubSystem.Light.Channel[0, 5] = txtBP6.Text;
                setting.SubSystem.Light.Channel[0, 6] = txtBP7.Text;
                setting.SubSystem.Light.Channel[0, 7] = txtBP8.Text;
                setting.SubSystem.Light.Channel[1, 0] = txtCA1.Text;
                setting.SubSystem.Light.Channel[1, 1] = txtCA2.Text;
                setting.SubSystem.Light.Channel[1, 2] = txtCA3.Text;
                setting.SubSystem.Light.Channel[1, 3] = txtCA4.Text;
                setting.SubSystem.Light.Channel[1, 4] = txtCA5.Text;
                setting.SubSystem.Light.Channel[1, 5] = txtCA6.Text;
                setting.SubSystem.Light.Channel[1, 6] = txtCA7.Text;
                setting.SubSystem.Light.Channel[1, 7] = txtCA8.Text;
                setting.SubSystem.Light.Channel[2, 0] = txtBA1.Text;
                setting.SubSystem.Light.Channel[2, 1] = txtBA2.Text;
                setting.SubSystem.Light.Channel[2, 2] = txtBA3.Text;
                setting.SubSystem.Light.Channel[2, 3] = txtBA4.Text;
                setting.SubSystem.Light.Channel[2, 4] = txtBA5.Text;
                setting.SubSystem.Light.Channel[2, 5] = txtBA6.Text;
                setting.SubSystem.Light.Channel[2, 6] = txtBA7.Text;
                setting.SubSystem.Light.Channel[2, 7] = txtBA8.Text;
                #endregion
                #region Encoder
                setting.SubSystem.ENC.UseEncoder = (txtUseENC.Text == "1") ? true : false;
                setting.SubSystem.ENC.EncoderType = Convert.ToInt32(txtENCType.Text);
                setting.SubSystem.ENC.Low[0] = Convert.ToInt32(txtLow1.Text);
                setting.SubSystem.ENC.Low[1] = Convert.ToInt32(txtLow2.Text);
                setting.SubSystem.ENC.Low[2] = Convert.ToInt32(txtLow3.Text);
                setting.SubSystem.ENC.High[0] = Convert.ToInt32(txtHigh1.Text);
                setting.SubSystem.ENC.High[1] = Convert.ToInt32(txtHigh2.Text);
                setting.SubSystem.ENC.High[2] = Convert.ToInt32(txtHigh3.Text);
                setting.SubSystem.ENC.UseFilter[0] = Convert.ToInt32(txtFilter1.Text);
                setting.SubSystem.ENC.UseFilter[1] = Convert.ToInt32(txtFilter2.Text);
                setting.SubSystem.ENC.UseFilter[2] = Convert.ToInt32(txtFilter3.Text);
                setting.SubSystem.ENC.FilterTime[0] = Convert.ToDouble(txtTime1.Text);
                setting.SubSystem.ENC.FilterTime[1] = Convert.ToDouble(txtTime2.Text);
                setting.SubSystem.ENC.FilterTime[2] = Convert.ToDouble(txtTime3.Text);

                setting.SubSystem.ENC.Port[0] = txtENCPort1.Text;
                setting.SubSystem.ENC.Port[1] = txtENCPort2.Text;

                setting.SubSystem.ENC.Direction[0] = Convert.ToInt32(txtENCDir1.Text);
                setting.SubSystem.ENC.Direction[1] = Convert.ToInt32(txtENCDir2.Text);
                setting.SubSystem.ENC.Count[0] = Convert.ToInt32(txtENCCnt1.Text);
                setting.SubSystem.ENC.Count[1] = Convert.ToInt32(txtENCCnt2.Text);
                setting.SubSystem.ENC.Delay1[0] = Convert.ToInt32(txtENCDelay11.Text);
                setting.SubSystem.ENC.Delay1[1] = Convert.ToInt32(txtENCDelay12.Text);
                setting.SubSystem.ENC.Delay2[0] = Convert.ToInt32(txtENCDelay21.Text);
                setting.SubSystem.ENC.Delay2[1] = Convert.ToInt32(txtENCDelay22.Text);
                setting.SubSystem.ENC.Width[0] = Convert.ToInt32(txtENCWidth1.Text);
                setting.SubSystem.ENC.Width[1] = Convert.ToInt32(txtENCWidth2.Text);
                #endregion
            }
            catch
            {
                MessageBox.Show("잘못된 값을 설정 하였습니다.");
                return;
            }
            setting.General.Save();
            setting.SubSystem.Save();
        }
    }
}
