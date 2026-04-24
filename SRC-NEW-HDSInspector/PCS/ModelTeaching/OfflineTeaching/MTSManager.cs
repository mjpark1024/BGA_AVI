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
 * @file  MTSManager.cs
 * @brief 
 *  Offline Teaching Manager Class.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2012.04.03
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2012.04.03 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common.DataBase;
using System.Diagnostics;
using Common;

namespace PCS.ModelTeaching.OfflineTeaching
{
    public class MTSManager
    {
        public List<MachineInformation> MachineList = new List<MachineInformation>();
        public MachineInformation SelectedMachine { get; set; }
        public VisionInterface TheVision { private get; set; }

        private MultipleDBConnector m_MultipleDBConnector = new MultipleDBConnector();
        private SettingMTS m_Setting;

        public int LastSelectedMachine { get; set; }
        public int LastSelectedGroup { get; set; }
        public int LastSelectedModel { get; set; }

        // Vision EXE 경로.
        public string VisionLocation
        {
            get
            {
                if (m_Setting != null)
                    return m_Setting.VisionLocation;
                else
                    return @"C:\IS\IS.exe";
            }
            set
            {
                if (m_Setting != null)
                    m_Setting.VisionLocation = value;
            }
        }

        #region 기준 영상 경로 get/set
        public string[] VisionImagePath = new string[3];

        // 상부반사 기준영상 경로.
        public string Vision1ImagePath
        {
            get
            {
                if (m_Setting != null)
                    return m_Setting.Vision1ImagePath;
                else
                    return @"C:\IS\IS-1\GrabImage_00.bmp";
            }
            set
            {
                if (m_Setting != null)
                    m_Setting.Vision1ImagePath = value;
            }
        }

        // 하부반사 기준영상 경로.
        public string Vision2ImagePath
        {
            get
            {
                if (m_Setting != null)
                    return m_Setting.Vision2ImagePath;
                else
                    return @"C:\IS\IS-2\GrabImage_00.bmp";
            }
            set
            {
                if (m_Setting != null)
                    m_Setting.Vision2ImagePath = value;
            }
        }

        // 상부투과 기준영상 경로.
        public string Vision3ImagePath
        {
            get
            {
                if (m_Setting != null)
                    return m_Setting.Vision3ImagePath;
                else
                    return @"C:\IS\IS-3\GrabImage_00.bmp";
            }
            set
            {
                if (m_Setting != null)
                    m_Setting.Vision3ImagePath = value;
            }
        }
        #endregion

        public MTSManager(SettingMTS aSettingMTS)
        {
            m_Setting = aSettingMTS;
            m_Setting.Load();

            if (VisionImagePath == null || VisionImagePath.Length != 3)
                VisionImagePath = new string[3];
            VisionImagePath[0] = Vision1ImagePath;
            VisionImagePath[1] = Vision2ImagePath;
            VisionImagePath[2] = Vision3ImagePath;

            LoadLastState();
            SetMachineList();
            if (MachineList.Count > 0)
            {
                m_MultipleDBConnector.SetConnectStrings(ref MachineList);
            }
        }

        // machine_info 테이블로부터 장비 목록을 읽어온다.
        public void SetMachineList()
        {
            #region Initialize MachineList.
            if (MachineList == null)
                MachineList = new List<MachineInformation>();

            if (MachineList.Count > 0)
                MachineList.Clear();
            #endregion

            IDataReader dataReader = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string szQuery = "SELECT A.machine_code, A.machine_name, A.machine_type FROM bgadb.machine_info A WHERE A.use_yn='1'";

                    dataReader = ConnectFactory.DBConnector().ExecuteQuery19(szQuery);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            MachineInformation machine = new MachineInformation(
                                dataReader.GetValue(0).ToString(),  // Code.
                                dataReader.GetValue(1).ToString(),  // Name.
                                dataReader.GetValue(2).ToString()   // Type.
                                );
                            machine.IP = m_Setting.TryLoadMachineIP(machine.Name);

                            MachineList.Add(machine);
                        }
                        dataReader.Close();
                    }
                }
                Debug.WriteLine(string.Format("{0} Machines Loaded.", MachineList.Count));
            }
            catch
            {
                if (dataReader != null) dataReader.Close();
                Debug.WriteLine("Exception occured in GetCameraResolution(ResolutionHelper.cs");
            }
        }

        #region Load & Save Status.
        private void LoadLastState()
        {
            LastSelectedMachine = m_Setting.LastSelectedMachine;
            LastSelectedGroup = m_Setting.LastSelectedGroup;
            LastSelectedModel = m_Setting.LastSelectedModel;
        }

        public void SaveLastState()
        {
            m_Setting.LastSelectedMachine = LastSelectedMachine;
            m_Setting.LastSelectedGroup = LastSelectedGroup;
            m_Setting.LastSelectedModel = LastSelectedModel;

            foreach (MachineInformation machine in MachineList)
            {
                if (machine.IP.ToLower() == "localhost")
                    machine.IP = "127.0.0.1";

                string[] arrTokens = machine.IP.Split('.');
                if (arrTokens.Length == 4)
                {
                    m_Setting.TrySaveMachineIP(machine.Name, machine.IP);
                }
            }
        }
        #endregion Load & Save Status.

        public void ConnectToSelectedMachine(string aszMachineName)
        {
            m_MultipleDBConnector.ChangeConnection(aszMachineName);
        }

        public void Connect()
        {
            if (TheVision != null && !TheVision.Connected)
            {
                TheVision.Connect("127.0.0.1", 15000);
            }
        }

        public void InitVision(ISPara aVisionSetting, int anDesVision)
        {
            int tempDesVision = 0;
            if (anDesVision >= 3)
                tempDesVision = anDesVision - 1;
            else
                tempDesVision = anDesVision;

            if (TheVision != null && aVisionSetting != null)
            {
                int nGrabCount = 4;
                int nCropHeight = 500;
                TheVision.ID = tempDesVision;
                TheVision.InitVision(aVisionSetting.FGType[0], aVisionSetting.CameraWidth[0], aVisionSetting.CameraHeight[0], 8, nGrabCount, 0, aVisionSetting.DeviceName[0], VisionImagePath[tempDesVision], nCropHeight, nCropHeight, 2.5, 2);
            }
        }
    }
}
