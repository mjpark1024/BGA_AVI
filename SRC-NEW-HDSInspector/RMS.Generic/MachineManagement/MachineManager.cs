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
 * @file  MachineManager.cs
 * @brief 
 *  Machine Manager class.
 * 
 * @author : Minseok Hwang <h.min-suck@samsung.com>
 * @date : 2011.08.27
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.27 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

using System.Data;
using Common.DataBase;

namespace RMS.Generic.MachineManagement
{
    public class MachineManager
    {

        private static MachineManager _Instance;
        private MachineInformation m_Machine = new MachineInformation() ;

        private ObservableCollection<MachineInformation> m_listMachineInformation = new ObservableCollection<MachineInformation>();

        public ObservableCollection<MachineInformation> Machines
        {
            get
            {
                return m_listMachineInformation;
            }
        }

        public MachineInformation CurMachine
        {
            get { return m_Machine; }
            set { m_Machine = value; }
        }

        public static MachineManager Instance()
        {
            if (_Instance == null)
                _Instance = new MachineManager();
             return _Instance;
        }


        public MachineInformation SelectMachineInfo(string strCode)
        {
            MachineInformation SelectMachine = new MachineInformation();

            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select a.machine_code. a.machine_name, a.machine_type, b.com_dname,  a.send_yn from machine_info a, com_detail b where a.use_yn = 1 and a.machine_code = '{0}' and b.com_mcode = '01'", strCode);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery(strQuery);

                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        SelectMachine.Code = dataReader.GetValue(0).ToString();
                        SelectMachine.Name = dataReader.GetValue(1).ToString();
                        SelectMachine.Category.Code = dataReader.GetValue(2).ToString();
                        SelectMachine.Category.Name = dataReader.GetValue(3).ToString();
                        SelectMachine.IsSend = Convert.ToInt32(dataReader.GetValue(4).ToString()) > 0? true:false;
                    }
                    dataReader.Close();
                }
            }
            return SelectMachine;

        }


        public void LoadMachine()
        {
            // Load machine data from database.
        }

        public void AddUser(MachineInformation newMachineInformation)
        {
            m_listMachineInformation.Add(newMachineInformation);
        }

        public void ModifyUser(MachineInformation oldMachineInformation, MachineInformation newMachineInformation)
        {
            foreach (MachineInformation userInformation in m_listMachineInformation)
            {
                if (userInformation == oldMachineInformation)
                {

                }
            }
        }

        public void DeleteMachine(MachineInformation MachineInformation)
        {
            m_listMachineInformation.Remove(MachineInformation);
        }
    }
}
