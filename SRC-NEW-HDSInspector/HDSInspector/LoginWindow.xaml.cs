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
 * @file  LoginWindow.xaml.cs
 * @brief . 
 *  behind code of LoginWindow.xaml
 * 
 * @author : suoow 
 * @date : 2011.05.09
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.05.09 First creation.
 * - 2011.05.15 ProgressBar Add.
 * - 2011.05.26 Xml reading Add.
 */
using Common;
using Common.Control;
using Common.DataBase;
using HDSInspector.SubWindow;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using PCS.ModelTeaching.OfflineTeaching;
using RMS.Generic.UserManagement;
using RVS.Generic.Insp;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace HDSInspector
{
    /// <summary>   Form for viewing the login.  </summary>
    /// <remarks>   suoow, 2014-05-09. </remarks>
    public partial class LoginWindow : Window
    {
        /// <summary> The logger </summary>
        private static Logger m_Logger = Logger.GetLogger();

        /// <summary> The main window </summary>
        private MainWindow m_MainWindow;

        /// <summary>   Initializes a new instance of the LoginWindow class. </summary>
        /// <remarks>   suoow, 2014-05-09. </remarks>
        public LoginWindow()
        {            
            InitializeComponent();
            InitializeEvent();
        }
        #region Add DataBase Para
        private static int AddAutoColumn()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                int n = -1;
                String strQuery = String.Format("select AutoNGX from model_info");
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                try
                {
                    if (dataReader.Read())
                    {
                        string code = dataReader.GetValue(0).ToString();
                        n = 0;
                    }
                }
                catch
                {
                }
                if (dataReader != null) dataReader.Close();
                if (n == -1)
                {
                    strQuery = String.Format("alter table model_info add column AutoNGX INT(4) default 0");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("alter table model_info add column AutoNGY INT(4) default 0");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }
            return 0;
        }

        private static int AddMachineState()
        {
            int nRet = -1;
            if (ConnectFactory.DBConnector() != null)
            {
                nRet = -1;
                string strQuery = String.Format("Select * from machine_status");

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                try
                {
                    if (dataReader.Read())
                    {
                        string code = dataReader.GetValue(0).ToString();
                        dataReader.Close();
                        return 0;
                    }
                }
                catch
                {
                }
                if (dataReader != null) dataReader.Close();
                if (nRet == -1)
                {
                    strQuery = String.Format("create table machine_status(machine varchar(20) not null primary key, order_no varchar(20), model varchar(100), start_time datetime, end_time datetime, use_verify int, use_setting int, running int)");
                    nRet = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into machine_status(machine, order_no, model, start_time, end_time, use_verify, use_setting, running) values('{0}', '', '', now(), now(), 0, 0, 0)", MainWindow.Setting.General.MachineName);
                    nRet = ConnectFactory.DBConnector().Execute(strQuery);
                }

            }
            return nRet;
        }

        private static int UpdateDefectColumn()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                int n = -1;
                String strQuery = String.Format("select defect_name from defect_info where defect_code='0460' and defect_name='다운셋'");
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                try
                {
                    if (dataReader.Read())
                    {
                        string code = dataReader.GetValue(0).ToString();
                        n = 0;
                    }
                }
                catch
                {
                }
                if (dataReader != null) dataReader.Close();
                if (n == 0)
                {
                    strQuery = String.Format("update defect_info set defect_name = 'Via 미충진' where defect_code='0460'");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }
            return 0;
        }

        private static int AddViaColumn()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                int n = -1;
                String strQuery = String.Format("select inspect_code from inspect_info where inspect_code='0460'");
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                try
                {
                    if (dataReader.Read())
                    {
                        string code = dataReader.GetValue(0).ToString();
                        n = 0;
                    }
                }
                catch
                {
                }
                if (dataReader != null) dataReader.Close();
                if (n != 0)
                {
                    strQuery = String.Format("insert into inspect_info(inspect_code, inspect_name, use_yn, reg_date, user_id) values('0460', 'Via 검사', 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }
            return 0;
        }

        private static int AddNewVia()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                int n = -1;
                String strQuery = String.Format("select inspect_code from inspect_property where inspect_code='0460'");
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                try
                {
                    if (dataReader.Read())
                    {
                        string code = dataReader.GetValue(0).ToString();
                        n = 0;
                    }
                }
                catch
                {
                }
                if (dataReader != null) dataReader.Close();
                if (n == -1)
                {
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0000', 2, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0001', 80, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0002', 140, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0003', 1, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0004', 0, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0005', 0, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0006', 100, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0007', 30, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0008', 30, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0009', 10, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0010', 10, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0011', 1, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0012', 0, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0013', 0, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0014', 0, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0015', 30, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0016', 10, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0017', 3, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0064', 30, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0065', 0, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0066', 0, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }
            return 0;
        }

        private static int AddSameValue()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                int n = -1;
                String strQuery = String.Format("select param_group from inspect_param where param_code='0999'");
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                try
                {
                    if (dataReader.Read())
                    {
                        string code = dataReader.GetValue(0).ToString();
                        n = 0;
                    }
                }
                catch
                {
                }
                if (dataReader != null) dataReader.Close();
                if (n == -1)
                {
                    strQuery = String.Format("Insert into inspect_param(param_group, param_code, param_name, use_yn, reg_date, user_id) values('3012', '0999', 'SameValue', 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_param(param_group, param_code, param_name, use_yn, reg_date, user_id) values('3013', '0999', 'SameValue', 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                }

                n = -1;
                strQuery = String.Format("select inspect_code from inspect_property where param_code='0999'");
                dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                try
                {
                    if (dataReader.Read())
                    {
                        string code = dataReader.GetValue(0).ToString();
                        n = 0;
                    }
                }
                catch
                {
                }
                if (dataReader != null) dataReader.Close();
                if (n == -1)
                {
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0422', '3012', '0999', 1, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0423', '3012', '0999', 1, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0431', '3012', '0999', 1, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0432', '3012', '0999', 1, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0440', '3012', '0999', 1, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0460', '3012', '0999', 1, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0429', '3013', '0999', 1, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }
            return 0;
        }

        private static bool AddUnitColumn()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                ConnectFactory.DBConnector().StartTrans();
                String strQuery = String.Format("select count(com_dcode) from com_detail where com_dcode='3022'");
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                while (dataReader.Read())
                {
                    if (dataReader.GetInt32(0) == 0)
                    {
                        strQuery = String.Format("insert into com_detail(com_dcode, com_dname, com_desc, com_mcode, use_yn, reg_date, user_id) values('3022', '유닛인식키 검사', 'param group(Algorithm)', 30, 1, now(), 'system')");

                        int val = ConnectFactory.DBConnector().Execute(strQuery);

                        if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                        {
                            ConnectFactory.DBConnector().Commit();
                            return true;
                        }
                        else
                        {
                            ConnectFactory.DBConnector().Rollback();
                            return false;
                        }
                    }
                }
                if (dataReader != null) dataReader.Close();
            }
            return true;
        }
        private static int AddinfoColumn()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                int n = -1;
                String strQuery = String.Format("select inspect_code from inspect_info where inspect_code='0407'");
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                try
                {
                    if (dataReader.Read())
                    {
                        string code = dataReader.GetValue(0).ToString();
                        n = 0;
                    }
                }
                catch
                {
                }
                if (dataReader != null) dataReader.Close();
                if (n != 0)
                {
                    strQuery = String.Format("insert into inspect_info(inspect_code, inspect_name, use_yn, reg_date, user_id) values('0407', '유닛인식키 검사', 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }
            return 0;
        }
        private static int AdddefectColumn()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                int n = -1;
                String strQuery = String.Format("select defect_code from defect_info where defect_code='0489'");
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                try
                {
                    if (dataReader.Read())
                    {
                        string code = dataReader.GetValue(0).ToString();
                        n = 0;
                    }
                }
                catch
                {
                }
                if (dataReader != null) dataReader.Close();
                if (n != 0)
                {
                    strQuery = String.Format("insert into defect_info(defect_code, defect_name, inspect_code, defect_group, autong_yn, use_yn, reg_date, user_id) values('0489', '유닛인식키', '0407', '2001', 1, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }
            return 0;
        }
        private static int AddNewUnitPattern()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                int n = -1;
                String strQuery = String.Format("select param_group from inspect_param where param_group='3022'");
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                try
                {
                    if (dataReader.Read())
                    {
                        string code = dataReader.GetValue(0).ToString();
                        n = 0;
                    }
                }
                catch
                {
                }
                if (dataReader != null) dataReader.Close();
                if (n == -1)
                {
                    strQuery = String.Format("Insert into inspect_param(param_group, param_code, param_name, use_yn, reg_date, user_id) values('3022', '0001', 'LowerThresh', 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);

                    strQuery = String.Format("Insert into inspect_param(param_group, param_code, param_name, use_yn, reg_date, user_id) values('3022', '0015', 'MinDefectSize', 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }
            return 0;
        }
        private static int AddNewUnitPattern2()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                int n = -1;
                String strQuery = String.Format("select inspect_code from inspect_property where inspect_code='0407'");
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                try
                {
                    if (dataReader.Read())
                    {
                        string code = dataReader.GetValue(0).ToString();
                        n = 0;
                    }
                }
                catch
                {
                }
                if (dataReader != null) dataReader.Close();
                if (n == -1)
                {
                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0407', '3022', '0001', 100, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);

                    strQuery = String.Format("Insert into inspect_property(inspect_code, param_group, param_code, param_value, use_yn, reg_date, user_id) values('0407', '3022', '0015', 80, 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }
            return 0;
        }


        private static int Add_PSR()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                int n = -1;
                String strQuery;
                String msg = "set sql_safe_updates=0;SET FOREIGN_KEY_CHECKS = 0;";

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0000','0','1', now(), 'system');");              
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0060','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0062','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0063','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0064','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0065','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0066','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0067','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0068','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0069','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0070','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0071','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0072','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0073','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0074','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0075','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0076','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0077','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0450','3026','0078','0','1', now(), 'system');");
                n = ConnectFactory.DBConnector().Execute(strQuery);

                return 0;
            }
            return -1;
        }

        #endregion
        /// <summary>   Gets the check login. </summary>
        /// <returns>   true if it succeeds, false if it fails. </returns>
        /// 

        private static int AddGV_inspection()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                int n = -1;
                string strQuery = String.Format("select inspect_code from bgadb.inspect_info where inspect_code='0490'");         
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
        
                try
                {
                    if (dataReader.Read())
                    {
                        string code = dataReader.GetValue(0).ToString();
                        n = 0;
                    }
                }
                catch
                {
                    if(dataReader != null) dataReader.Close();
                }

                if (n == -1)
                {
                    String msg = "set sql_safe_updates = 0;SET FOREIGN_KEY_CHECKS = 0;";

                    // inspect_info
                    strQuery = msg + String.Format("Insert into bgadb.inspect_info(inspect_code, inspect_name, use_yn, reg_date, user_id) values('0490', 'GV검사', 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    
                    // defect_info
                    strQuery = msg + String.Format("Insert into bgadb.defect_info(defect_code, defect_name, inspect_code, defect_group, autong_yn, use_yn, reg_date, user_id) values('0490', 'GV불량', '0490','3027' , 0 , 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);

                    // inspect_property
                    strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0490','3027','0000','0','1', now(), 'system');");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0490','3027','0001','0','1', now(), 'system');");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0490','3027','0002','0','1', now(), 'system');");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0490','3027','0003','0','1', now(), 'system');");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0490','3027','0004','0','1', now(), 'system');");
                    n = ConnectFactory.DBConnector().Execute(strQuery);

                    // inspect_param
                    //strQuery = String.Format("Insert into bgadb.inspect_param(param_group, param_code, param_name, use_yn, reg_date, user_id) values('3027', '0000', 'Threshold', 1, now(), 'system')");
                    //n = ConnectFactory.DBConnector().Execute(strQuery);
                    //strQuery = String.Format("Insert into bgadb.inspect_param(param_group, param_code, param_name, use_yn, reg_date, user_id) values('3027', '0000', 'MinDefectSize', 1, now(), 'system')");
                    //n = ConnectFactory.DBConnector().Execute(strQuery);
                    //strQuery = String.Format("Insert into bgadb.inspect_param(param_group, param_code, param_name, use_yn, reg_date, user_id) values('3027', '0000', 'MinDefectSize', 1, now(), 'system')");
                    //n = ConnectFactory.DBConnector().Execute(strQuery);
                    //strQuery = String.Format("Insert into bgadb.inspect_param(param_group, param_code, param_name, use_yn, reg_date, user_id) values('3027', '0000', 'MinDefectSize', 1, now(), 'system')");
                    //n = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }
            return 0;
        }

        private static int Add_VentHole2()
        {

            if (ConnectFactory.DBConnector() != null)
            {
                int n = -1;
                string strQuery = String.Format("select inspect_code from bgadb.inspect_info where inspect_code='0441'");
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                try
                {
                    if (dataReader.Read())
                    {
                        string code = dataReader.GetValue(0).ToString();
                        n = 0;
                    }
                }
                catch
                {
                    if (dataReader != null) dataReader.Close();
                }

                if (n == -1)
                {
                    String msg = "set sql_safe_updates = 0;SET FOREIGN_KEY_CHECKS = 0;";

                    // inspect_info
                    strQuery = msg + String.Format("Insert into bgadb.inspect_info(inspect_code, inspect_name, use_yn, reg_date, user_id) values('0441', 'Vent-Hole2', 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);

                    // defect_info
                    strQuery = msg + String.Format("Insert into bgadb.defect_info(defect_code, defect_name, inspect_code, defect_group, autong_yn, use_yn, reg_date, user_id) values('0491', '외곽 Vent-Hole', '0491','2002' , 0 , 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);

                    // inspect_property
                    strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0491','3015','0000','0','1', now(), 'system');");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0491','3015','0001','0','1', now(), 'system');");
                    n = ConnectFactory.DBConnector().Execute(strQuery);
                    strQuery = msg + String.Format("Insert into inspect_property(inspect_code,param_group,param_code,param_value,use_yn,reg_date,user_id) values('0491','3015','0015','0','1', now(), 'system');");
                    n = ConnectFactory.DBConnector().Execute(strQuery);

                    // inspect_param
                    //strQuery = String.Format("Insert into bgadb.inspect_param(param_group, param_code, param_name, use_yn, reg_date, user_id) values('3027', '0000', 'Threshold', 1, now(), 'system')");
                    //n = ConnectFactory.DBConnector().Execute(strQuery);
                    //strQuery = String.Format("Insert into bgadb.inspect_param(param_group, param_code, param_name, use_yn, reg_date, user_id) values('3027', '0000', 'MinDefectSize', 1, now(), 'system')");
                    //n = ConnectFactory.DBConnector().Execute(strQuery);
                    //strQuery = String.Format("Insert into bgadb.inspect_param(param_group, param_code, param_name, use_yn, reg_date, user_id) values('3027', '0000', 'MinDefectSize', 1, now(), 'system')");
                    //n = ConnectFactory.DBConnector().Execute(strQuery);
                    //strQuery = String.Format("Insert into bgadb.inspect_param(param_group, param_code, param_name, use_yn, reg_date, user_id) values('3027', '0000', 'MinDefectSize', 1, now(), 'system')");
                    //n = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }
            return 0;




            if (ConnectFactory.DBConnector() != null)
            {
                int n = -1;
                string strQuery = String.Format("select inspect_code from bgadb.defect_info where defect_code='0491'");
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                try
                {
                    if (dataReader.Read())
                    {
                        string code = dataReader.GetValue(0).ToString();
                        n = 0;
                    }
                }
                catch
                {
                    if (dataReader != null) dataReader.Close();
                }

                if (n == -1)
                {
                    String msg = "set sql_safe_updates = 0;SET FOREIGN_KEY_CHECKS = 0;";
     
                    // defect_info
                    strQuery = msg + String.Format("Insert into bgadb.defect_info(defect_code, defect_name, inspect_code, defect_group, autong_yn, use_yn, reg_date, user_id) values('0491', '외곽 VentHole', '0412','2002' , 0 , 1, now(), 'system')");
                    n = ConnectFactory.DBConnector().Execute(strQuery);

                }
            }
            return 0;
        }


        public static bool CheckLogin(string aszID, string aszPassword)
        {
            #region Add DB
            //if (!AddUnitColumn()) return false; ;
            //AddinfoColumn();
            //AdddefectColumn();
            //AddNewUnitPattern();
            //AddNewUnitPattern2();
            //AddMachineState();
            //AddAutoColumn();
            //UpdateDefectColumn();
            //AddViaColumn();
            //AddNewVia();
            //AddSameValue();
            AddGV_inspection();
            Add_VentHole2();
            Add_PSR();
            

            #endregion
            if (aszID == "hds" && aszPassword == "1")
            {
                UserManager.CurrentUser.ID = aszID;
                UserManager.CurrentUser.Name = "관리자";
                UserManager.CurrentUser.Password = "1";
                UserManager.CurrentUser.Authority.AuthCode = "0061";
                UserManager.CurrentUser.Authority.AuthName = "";
                Log("PCS", SeverityLevel.DEBUG, String.Format("\"{0}\"님이 로그인하였습니다.", aszID));

                return true;
            }
            if (ConnectFactory.DBConnector() != null)
            {
                String txtPasswd = MD5Core.GetHashString(aszPassword + MD5Core.GetHashString(aszID));
                String strQuery = String.Format("Select a.user_id, a.user_name, a.user_passwd, a.user_auth, b.com_dname from user_info a, com_detail b where a.user_auth = b.com_dcode and a.use_yn = 1 and a.user_id = '{0}' and b.com_mcode = '06' and a.user_passwd = '{1}'", aszID, txtPasswd);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                if (dataReader == null)
                {
                    return false;
                }

                if (dataReader.Read())
                {
                    UserManager.CurrentUser.ID = dataReader.GetValue(0).ToString();
                    UserManager.CurrentUser.Name = dataReader.GetValue(1).ToString();
                    UserManager.CurrentUser.Password = dataReader.GetValue(2).ToString();
                    UserManager.CurrentUser.Authority.AuthCode = dataReader.GetValue(3).ToString();
                    UserManager.CurrentUser.Authority.AuthName = dataReader.GetValue(4).ToString();

                    dataReader.Close();
                    Log("PCS", SeverityLevel.DEBUG, String.Format("\"{0}\"님이 로그인하였습니다.", aszID));

                    return true;
                }
            }
            Log("PCS", SeverityLevel.DEBUG, String.Format("\"{0}\"님이 로그인에 실패하였습니다.", aszID));
            return false;
        }

        private void InitializeEvent()
        {
            this.btnLogin.Click += btnLogin_Click;
            this.btnAddUser.Click += btnAddUser_Click;
            this.btnLogin.MouseEnter += btn_MouseEnter;
            this.btnLogin.MouseLeave += btn_MouseLeave;
            this.btnClose.Click += btnClose_Click;
            this.KeyDown += LoginWindow_KeyDown;
            this.Loaded += LoginWindow_Loaded;
        }

        void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.prog.Value = 0;
            this.IsEnabled = false;

            
            m_MainWindow = new MainWindow(m_Logger);
            
            Application.Current.MainWindow = m_MainWindow;

            string szUserID = MainWindow.Setting.Job.LastUser;
            string szUserPW = string.Empty;
            if (string.IsNullOrEmpty(szUserID))
                szUserID = "hds";
            if (szUserID == "hds")
                szUserPW = "1";

            this.txtID.Text = szUserID;
            this.txtPW.Password = szUserPW;

            if (MainWindow.Setting.SubSystem.PLC.UsePLC)
            {
                Thread thdVisionRestart = new Thread(new ThreadStart(RestartVision));
                thdVisionRestart.Start();
            }
            else
            {
                this.prog.Value = 100;
                this.IsEnabled = true;
            }

            MainWindow.CommandExcuteISUpdate(); // IS업데이트 실행
        }

        public void RestartVision()
        {
            RemoteVisionProcessKill();
            Action action = delegate
            {
                this.prog.Value = 30;
            }; this.Dispatcher.Invoke(action);

            RemoteVisionProcessStart();
            action = delegate
            {
                this.prog.Value = 60;
            }; this.Dispatcher.Invoke(action);
            int cnt = 0;
            while(!m_MainWindow.PCSInstance.bInit)
            {
                Thread.Sleep(100);
                cnt++;
                if (cnt > 100) break;
            }

            action = delegate
            {
                this.prog.Value = 100;
                this.IsEnabled = true;
            }; this.Dispatcher.Invoke(action);
        }

        public static void RemoteVisionProcessKill()
        {
            try
            {
                for (int i = 0; i < MainWindow.Setting.SubSystem.IS.IP.Length; i++)
                {
                    if ((!MainWindow.Setting.SubSystem.IS.UseBASlave && i == 4) || (!MainWindow.Setting.SubSystem.IS.UseCASlave && i == 2)) continue;
                    string remoteName = @"\\" + MainWindow.Setting.SubSystem.IS.IP[i] + @"\root\cimv2";

                    ConnectionOptions con = new ConnectionOptions();
                    con.Impersonation = ImpersonationLevel.Impersonate;
                    con.Authentication = AuthenticationLevel.PacketPrivacy;
                    con.EnablePrivileges = true;
                    con.Username = "bga";
                    con.Password = "vision";

                    ManagementPath ms = new ManagementPath();
                    ms.Path = remoteName;

                    ManagementScope managementScope = new ManagementScope();
                    managementScope.Options = con;
                    managementScope.Path = ms;
                    managementScope.Connect();

                    ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_Process Where Name = 'IS.exe'");
                    ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(managementScope, objectQuery);
                    ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
                    foreach (ManagementObject managementObject in managementObjectCollection)
                    {
                        managementObject.InvokeMethod("Terminate", null);
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Vision 원격 종료 실패");
            }
        }

        public static void RemoteVisionProcessKillTaskKill()
        {
            try
            {
                for (int i = 0; i < MainWindow.Setting.SubSystem.IS.IP.Length; i++)
                {
                    if (!MainWindow.Setting.SubSystem.IS.UseBASlave && i == 4 && (MainWindow.Setting.General.MachineName == "BAV07" || MainWindow.Setting.General.MachineName == "BAV08")) continue;
                    string remoteName = MainWindow.Setting.SubSystem.IS.IP[i];

                    ProcessStartInfo si = new ProcessStartInfo();
                    Process run = new Process();
                    si.FileName = "taskkill.exe";
                    si.UseShellExecute = false;
                    si.WindowStyle = ProcessWindowStyle.Hidden;
                    si.CreateNoWindow = true;
                    si.RedirectStandardInput = true;
                    si.Arguments = string.Format("/f /im is.exe /s {0} /u {1} /p {2}", remoteName, "bga", "vision");

                    run.StartInfo = si;
                    run.Start();
                    Thread.Sleep(500);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Vision 원격 종료 실패");
            }
        }


        public static void RemoteVisionProcessStart()
        {
            try
            {
                for (int i = 0; i < MainWindow.Setting.SubSystem.IS.IP.Length; i++)
                {
                    if ((!MainWindow.Setting.SubSystem.IS.UseBASlave && i == 4) || (!MainWindow.Setting.SubSystem.IS.UseCASlave && i == 2)) continue;
                    string szPCName = MainWindow.Setting.SubSystem.IS.IP[i];

                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = "schtasks.exe";
                    si.UseShellExecute = false;
                    si.WindowStyle = ProcessWindowStyle.Hidden;
                    si.CreateNoWindow = true;
                    si.RedirectStandardInput = true;
                    Process run = new Process();

                    si.Arguments = string.Format("/run /tn IS /s {0} /u {1} /p {2}", szPCName, "bga", "vision");
                    run.StartInfo = si;
                    run.Start();
                    Thread.Sleep(300);
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Vision 원격 종료 실패, 서비스 Remote Registy Service 'On' 을 확인하세요.");
            }
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow addUserWindow = new AddUserWindow();
            if ((bool)addUserWindow.ShowDialog())
            {
                this.txtID.Text = addUserWindow.NewUser.ID;
                this.txtPW.Password = addUserWindow.txtNewPassword.Password;
            }
        }

        /// <summary>   Event handler. Called by btn for mouse enter events. </summary>
        private void btn_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        /// <summary>   Event handler. Called by btn for mouse leave events. </summary>
        private void btn_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>   Event handler. Called by LoginWindow for key down events. </summary>
        /// <remarks>   suoow2, 2014-08-22. </remarks>
        private void LoginWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter || e.Key == Key.Space) && this.txtID.Text != string.Empty && this.txtPW.Password != string.Empty)
            {
                btnLogin_Click(btnLogin, null);
            }
        }

        /// <summary>   Event handler. Called by titlebar for mouse left button down events. </summary>
        /// <remarks>   suoow, 2014-05-09. </remarks>
        private void titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// <summary>   Event handler. Called by btnClose for click events. </summary>
        /// <remarks>   suoow, 2014-05-09. </remarks>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (m_MainWindow != null)
            {
                m_MainWindow.Close();

                if (MainWindow.MainLogger == null)
                {
                    this.Close();
                }
            }
        }

        #region handle log.
        /// <summary>   Logs. </summary>
        /// <remarks>   suoow2, 2014-05-15. </remarks>
        /// <param name="astrSubSystem">    The astr sub system. </param>
        /// <param name="aSeverityLevel">   The severity level. </param>
        /// <param name="astrLogMessage">   Message describing the astr log. </param>
        public static void Log(string astrSubSystem, SeverityLevel aSeverityLevel, string astrLogMessage)
        {
            try
            {
                if (MainWindow.Setting.General.LogSave && Convert.ToInt32(aSeverityLevel)
                    >= Convert.ToInt32(MainWindow.Setting.General.LogLevel))
                {
                    m_Logger.Log(astrSubSystem, aSeverityLevel, astrLogMessage);
                }
            }
            catch
            {
            }
        }

        /// <summary>   Clean log. </summary>
        /// <remarks>   suoow2, 2014-05-15. </remarks>
        public void CleanLog()
        {
            try
            {
                int nDeleteLogCount = Logger.CleanLog(MainWindow.Setting.General.LogKeepDate);
                if (nDeleteLogCount > 0)
                {
                    MessageBox.Show(String.Format("최근에 기록된 {0}개의 로그 파일을 정리하였습니다.", nDeleteLogCount), "Information");
                }
            }
            catch
            {
                Log("PCS", SeverityLevel.WARN, "로그 파일을 정리하는데 실패하였습니다.");
            }
        }
        #endregion

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            this.btnLogin.IsEnabled = false;
            this.btnAddUser.IsEnabled = false;
            this.txtStatusBoxScroll.ScrollToVerticalOffset(txtStatusBoxScroll.MaxHeight);

            RoundRectProgressBar progress = new RoundRectProgressBar { Owner = this };
            progress.Show();

            this.txtStatusBox.Text += "로그인...\n";

            int i = 0;
            while (i != 10000)
            {
                i++;
                System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                                       new System.Threading.ThreadStart(delegate { }));
            }
            progress.Stop();
            progress.Close();

            this.txtStatusBox.Text += "초기화...\n";
            this.txtStatusBox.Refresh();

            if (this.txtID.Text == "nodb" && this.txtPW.Password == "nodb")
            {
                this.Close();
                m_MainWindow.Show();
                return;
            }

            if (MainWindow.Setting.General.UseDB)
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    if (CheckLogin(txtID.Text, txtPW.Password))
                    {
                        MainWindow.Setting.Job.LastUser = txtID.Text;

                        this.txtStatusBox.Text += String.Format("DB : {0}succeed! \n", MainWindow.Setting.General.DBIP);
                        this.txtStatusBox.Refresh();
                        this.Close();

                        m_MainWindow.DBStatus = true;

                        RunApplication();
                        return;
                    }
                    else
                    {
                        // ID/PW 잘못된 입력시 에러 메시지
                        MessageBox.Show(ResourceStringHelper.GetErrorMessage("C003"), "Error");
                    }
                }
                else
                {
                    // DB 접속 실패 에러 메시지
                    MessageBox.Show(ResourceStringHelper.GetErrorMessage("C004"), "Error");
                }

                // DB 접속 또는 ID / PW 잘못된 입력 시
                this.btnLogin.IsEnabled = true;
                this.btnAddUser.IsEnabled = true;
                this.txtStatusBox.Text += "로그인 실패\n";
            }
            else
            {
                this.txtStatusBox.Text += "Not used DB! \n";
                this.txtStatusBox.Refresh();
                this.Close();

                RunApplication();
                return;
            }
        }

        private void RunApplication()
        {
            m_MainWindow.Show();
        }

        
    }
}