/*********************************************************************************
 * Copyright(c) 2011,2012,2013 by Samsung Techwin.
 * 
 * This software is copyrighted by, and is the sole property of Samsung Techwin.
 * All rigths, title, ownership, or other interests in the software remain the
 * property of Samsung Techwin. This software may only be used in accordance with
 * the corresponding license agreement. Any unauthorized use, duplication, 
 * transmission, distribution, or disclosure of this software is expressly 
 * forbidden.
 *
 * This Copyright notice may not be removed or modified without prior written
 * consent of Samsung Techwin.Samsung Techwin reserves the right to modify this 
 * software without notice.
 *
 * Samsung Techwin.
 * KOREA 
 * http://www.samsungtechwin.co.kr
 *********************************************************************************/
/**
 * @file  ANTSManager.cs
 * @brief 
 *  Offline Teaching Manager Class.
 * 
 * @author : Minseok Hwang <h.min-suck@samsung.com>
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
    public class ANTSManager
    {
        public List<MachineInformation> MachineList = new List<MachineInformation>();
        private MultipleDBConnector m_MultipleDBConnector = new MultipleDBConnector();
        private SettingMTS m_Setting;

        public int MachineCount
        {
            get
            {
                if (MachineList != null)
                    return MachineList.Count;
                else
                    return 0;
            }
        }

        public ANTSManager(SettingMTS aSettingMTS)
        {
            m_Setting = aSettingMTS;
            Debug.Assert(m_Setting != null);

            SetMachineList();
            if (MachineList.Count > 0)
            {
                m_MultipleDBConnector.SetConnectStrings(ref MachineList);
            }
        }

        private void InitMachineList()
        {
            if (MachineList == null)
                MachineList = new List<MachineInformation>();

            if (MachineList.Count > 0)
                MachineList.Clear();
        }

        // machine_info 테이블로부터 장비 목록을 읽어온다.
        public void SetMachineList()
        {
            InitMachineList();

            IDataReader dataReader = null;
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string szQuery = "SELECT A.machine_code, A.machine_name, A.machine_type FROM inspdb.machine_info A WHERE A.use_yn='1'";
                    
                    dataReader = ConnectFactory.DBConnector().ExecuteQuery(szQuery);
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

        public void ConnectToSelectedMachine(string aszMachineName)
        {
            m_MultipleDBConnector.ChangeConnection(aszMachineName);
        }
    }
}
