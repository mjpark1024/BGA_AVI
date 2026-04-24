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
 * @file  EquipmentManager.cs
 * @brief 
 *  Equipment Manager class.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
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
using RMS.Generic.UserManagement;

namespace RMS.Generic.EquipmentManagement
{
    public class EquipmentManager
    {
        /// <summary> Information describing the list equipment </summary>
        private ObservableCollection<EquipmentInformation> m_listEquipmentInformation = new ObservableCollection<EquipmentInformation>();
        
        /// <summary> Type of the list equipment </summary>
        private ObservableCollection<EquipmentType> m_listEquipmentType = new ObservableCollection<EquipmentType>();

        /// <summary>   Gets the equipments. </summary>
        /// <value> The equipments. </value>
        public ObservableCollection<EquipmentInformation> Equipments
        {
            get
            {
                return m_listEquipmentInformation;
            }
        }

        /// <summary>   Gets the type of the list equipment. </summary>
        /// <value> The type of the list equipment. </value>
        public ObservableCollection<EquipmentType> listEquipmentType
        {
            get
            {
                return m_listEquipmentType;
            }
        }

        /// <summary>   Loads the equipment type. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        public void LoadEquipmentType()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("SELECT com_dcode, com_dname FROM  com_detail WHERE com_mcode = '00'");

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {                    
                    m_listEquipmentType.Clear();

                    while (dataReader.Read())
                    {
                        EquipmentType listEquipmentType = new EquipmentType();
                        listEquipmentType.TypeCode = dataReader.GetValue(0).ToString();
                        listEquipmentType.TypeName = dataReader.GetValue(1).ToString();
                        m_listEquipmentType.Add(listEquipmentType);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Loads the equipment type search option. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        public void LoadEquipmentType_SearchOption()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("SELECT com_dcode, com_dname FROM  com_detail WHERE com_mcode = '00'");

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listEquipmentType.Clear();

                    EquipmentType listEquipmentType = new EquipmentType();
                    listEquipmentType.TypeCode = "-99";
                    listEquipmentType.TypeName = "전체 검색";
                    m_listEquipmentType.Add(listEquipmentType);

                    while (dataReader.Read())
                    {
                        listEquipmentType = new EquipmentType();
                        listEquipmentType.TypeCode = dataReader.GetValue(0).ToString();
                        listEquipmentType.TypeName = dataReader.GetValue(1).ToString();
                        m_listEquipmentType.Add(listEquipmentType);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Loads the equipment. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        public void LoadEquipment()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select a.machine_code, a.machine_name, a.machine_type, b.com_dname,  a.use_yn, a.Reg_date, a.user_id from machine_info a, com_detail b where a.machine_type = b.com_dcode and b.com_mcode = '00'");

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listEquipmentInformation.Clear();
                    while (dataReader.Read())
                    {
                        EquipmentInformation MachineEquip = new EquipmentInformation();
                        MachineEquip.Code = dataReader.GetValue(0).ToString();
                        MachineEquip.Name = dataReader.GetValue(1).ToString();
                        MachineEquip.EquipmentType.TypeCode = dataReader.GetValue(2).ToString();
                        MachineEquip.EquipmentType.TypeName = dataReader.GetValue(3).ToString();
                        MachineEquip.IsUse = Convert.ToBoolean(dataReader.GetValue(4).ToString());
                        MachineEquip.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(5).ToString());
                        MachineEquip.RegistrationID = dataReader.GetValue(6).ToString();
                        m_listEquipmentInformation.Add(MachineEquip);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Generates a machine code. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-14. </remarks>
        /// <returns>   The machine code. </returns>
        public string GenerateMachineCode()
        {
            // 가장 큰 값의 Machine Index가 생성된다.
            string strQuery = "SELECT ifnull(max(machine_code), 0) + 1 from machine_info";

            int nMaxStripCode = ConnectFactory.DBConnector().ExecuteScalarByInt(strQuery);

            return QueryHelper.GetCode(nMaxStripCode, 4);
        }

        /// <summary>   Select equip by name. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="astrEquipName">    Name of the astr equip. </param>
        public void SelectEquipByName(string astrEquipName)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select a.machine_code, a.machine_name, a.machine_type, b.com_dname,  a.use_yn, a.Reg_date, a.user_id from machine_info a, com_detail b where a.machine_type = b.com_dcode and a.machine_name LIKE '%{0}%' and b.com_mcode = '00'", astrEquipName);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listEquipmentInformation.Clear();
                    while (dataReader.Read())
                    {
                        EquipmentInformation MachineEquip = new EquipmentInformation();
                        MachineEquip.Code = dataReader.GetValue(0).ToString();
                        MachineEquip.Name = dataReader.GetValue(1).ToString();
                        MachineEquip.EquipmentType.TypeCode = dataReader.GetValue(2).ToString();
                        MachineEquip.EquipmentType.TypeName = dataReader.GetValue(3).ToString();
                        MachineEquip.IsUse = Convert.ToBoolean(dataReader.GetValue(4).ToString());
                        MachineEquip.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(5));
                        MachineEquip.RegistrationID = dataReader.GetValue(6).ToString();
                        m_listEquipmentInformation.Add(MachineEquip);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Select equip by type. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="astrEquipType">    Type of the astr equip. </param>
        public void SelectEquipByType(string astrEquipType)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select a.machine_code, a.machine_name, a.machine_type, b.com_dname,  a.use_yn, a.Reg_date, a.user_id from machine_info a, com_detail b where a.machine_type = b.com_dcode and a.machine_type = '{0}' and b.com_mcode = '00'", astrEquipType);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listEquipmentInformation.Clear();
                    while (dataReader.Read())
                    {
                        EquipmentInformation MachineEquip = new EquipmentInformation();
                        MachineEquip.Code = dataReader.GetValue(0).ToString();
                        MachineEquip.Name = dataReader.GetValue(1).ToString();
                        MachineEquip.EquipmentType.TypeCode = dataReader.GetValue(2).ToString();
                        MachineEquip.EquipmentType.TypeName = dataReader.GetValue(3).ToString();
                        MachineEquip.IsUse = Convert.ToBoolean(dataReader.GetValue(4).ToString());
                        MachineEquip.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(5));
                        MachineEquip.RegistrationID = dataReader.GetValue(6).ToString();
                        m_listEquipmentInformation.Add(MachineEquip);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Select equip by name type. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="astrEquipType">    Type of the astr equip. </param>
        /// <param name="astrEquipName">    Name of the astr equip. </param>
        public void SelectEquipByName_Type(string astrEquipType, string astrEquipName)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select a.machine_code, a.machine_name, a.machine_type, b.com_dname, a.use_yn, a.Reg_date, a.user_id from machine_info a, com_detail b where a.machine_type = b.com_dcode and a.machine_type = '{0}' and a.machine_name LIKE '%{1}%' and b.com_mcode = '00'", astrEquipType, astrEquipName);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listEquipmentInformation.Clear();
                    while (dataReader.Read())
                    {
                        EquipmentInformation MachineEquip = new EquipmentInformation();
                        MachineEquip.Code = dataReader.GetValue(0).ToString();
                        MachineEquip.Name = dataReader.GetValue(1).ToString();
                        MachineEquip.EquipmentType.TypeCode = dataReader.GetValue(2).ToString();
                        MachineEquip.EquipmentType.TypeName = dataReader.GetValue(3).ToString();
                        MachineEquip.IsUse = Convert.ToBoolean(dataReader.GetValue(4).ToString());
                        MachineEquip.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(5));
                        MachineEquip.RegistrationID = dataReader.GetValue(6).ToString();
                        m_listEquipmentInformation.Add(MachineEquip);
                    }
                    dataReader.Close();
                }
            }
        }

        /// <summary>   Searches for the first by name. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="astrEquipName">    Name of the astr equip. </param>
        /// <param name="astrEquipType">    Type of the astr equip. </param>
        /// <returns>   true if it succeeds, false if it fails. </returns>
        public bool SearchByName(string astrEquipName, string astrEquipType)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select machine_name from machine_info where machine_name = '{0}' AND machine_type = '{1}'", astrEquipName, astrEquipType);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    if (dataReader.Read())
                    {
                        dataReader.Close();
                        return true;
                    }
                    dataReader.Close();
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>   Adds an equipment.  </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="aEquipInformation">    Information describing the equip. </param>
        public void AddEquipment(EquipmentInformation aEquipInformation)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                ConnectFactory.DBConnector().StartTrans();

                // code 번호를 획득한다.
                aEquipInformation.Code = GenerateMachineCode();
                
                String strQuery = "INSERT INTO machine_info(machine_code, machine_name, machine_type, use_yn, reg_date, user_id, send_yn) ";
                strQuery += String.Format("VALUES('{0}', '{1}', '{2}', {3}, now(), '{4}', 0 ) ", aEquipInformation.Code, aEquipInformation.Name, aEquipInformation.EquipmentType.TypeCode, (aEquipInformation.IsUse == true) ? 1 : 0, UserManager.CurrentUser.ID);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                {
                    ConnectFactory.DBConnector().Commit();
                    m_listEquipmentInformation.Add(aEquipInformation);
                }
                else
                {
                    ConnectFactory.DBConnector().Rollback();
                }
            }
        }

        /// <summary>   Modify equipment. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="aEquipInformation">    Information describing the equip. </param>
        public void ModifyEquipment(EquipmentInformation aEquipInformation)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                ConnectFactory.DBConnector().StartTrans();

                String strQuery = String.Format("UPDATE machine_info SET machine_name ='{0}', machine_type ='{1}', use_yn ={2}, reg_date = now(), user_id='{3}' WHERE machine_code = '{4}'",
                   aEquipInformation.Name, aEquipInformation.EquipmentType.TypeCode, aEquipInformation.IsUse == true ? 1 : 0, UserManager.CurrentUser.ID, aEquipInformation.Code);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                {
                    ConnectFactory.DBConnector().Commit();
                }
                else
                {
                    ConnectFactory.DBConnector().Rollback();
                }
            }

        }

        /// <summary>   Deletes the equipment described by aEquipInformation. </summary>
        /// <remarks>   Shin.Cheol-min, 2014-09-17. </remarks>
        /// <param name="aEquipInformation">    Information describing the equip. </param>
        public void DeleteEquipment(EquipmentInformation aEquipInformation)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                ConnectFactory.DBConnector().StartTrans();

                String strQuery = String.Format("DELETE FROM machine_info WHERE machine_code ='{0}' ", aEquipInformation.Code);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                {
                    ConnectFactory.DBConnector().Commit();
                    m_listEquipmentInformation.Remove(aEquipInformation);
                }
                else
                {
                    ConnectFactory.DBConnector().Rollback();
                }
            }
            
        }

        public ObservableCollection<EquipmentInformation> LoadAllEquipmentList()
        {
            ObservableCollection<EquipmentInformation> listEquipment = new ObservableCollection<EquipmentInformation>();
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("SELECT '*', '*' "
                                               + " FROM com_master "
                                               + " WHERE com_mcode = '00' "
                                               + " UNION ALL "
                                               + " SELECT machine_code, machine_name "
                                               + " FROM machine_info "
                                               + " WHERE use_yn = 1 ");
                    

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    m_listEquipmentInformation.Clear();
                    while (dataReader.Read())
                    {
                        EquipmentInformation MachineEquip = new EquipmentInformation();
                        MachineEquip.Code = dataReader.GetValue(0).ToString();
                        MachineEquip.Name = dataReader.GetValue(1).ToString();
                        listEquipment.Add(MachineEquip);
                    }
                    dataReader.Close();
                }
            }

            return listEquipment;
        }
    }
}
