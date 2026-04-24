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
 * @file  UserManager.cs
 * @brief 
 *  User Manager class.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.08.27
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.27 First creation.
 */

using System;
using System.Collections.ObjectModel;
using System.Data;
using Common.DataBase;


namespace RMS.Generic.UserManagement
{
    public class UserManager
    {
        #region Static User.
        public static UserInformation CurrentUser
        {
            get { return m_CurrentUser; }
            set { m_CurrentUser = value; }
        }
        private static UserInformation m_CurrentUser = new UserInformation();
        #endregion

        #region User and Authority information.
        public ObservableCollection<UserInformation> UserList
        {
            get
            {
                return m_listUserInformation;
            }
        }
        private ObservableCollection<UserInformation> m_listUserInformation = new ObservableCollection<UserInformation>();

        public ObservableCollection<UserAuthority> UserAuthorityList
        {
            get
            {
                return m_listUserAuthority;
            }
        }
        private ObservableCollection<UserAuthority> m_listUserAuthority = new ObservableCollection<UserAuthority>();
        #endregion

        public void LoadUserAutority()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                string strQuery = "SELECT com_dcode, com_dname FROM com_detail WHERE use_yn = 1 and com_mcode = '06'";

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                if (dataReader != null)
                {
                    m_listUserAuthority.Clear();
                    while (dataReader.Read())
                    {
                        UserAuthority SelectUserAuthority = new UserAuthority();
                        SelectUserAuthority.AuthCode = dataReader.GetValue(0).ToString();
                        SelectUserAuthority.AuthName = dataReader.GetValue(1).ToString();

                        m_listUserAuthority.Add(SelectUserAuthority);
                    }
                    dataReader.Close();
                }
            }
        }

        public void LoadUser()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                string strQuery = "Select a.user_id, a.user_name, a.user_passwd, a.user_auth, b.com_dname, a.reg_date from user_info a, com_detail b where a.user_auth = b.com_dcode and a.use_yn = 1  and b.com_mcode = '06'";

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                if (dataReader != null)
                {
                    m_listUserInformation.Clear();
                    while (dataReader.Read())
                    {
                        UserInformation SelectUser = new UserInformation();
                        SelectUser.ID = dataReader.GetValue(0).ToString();
                        SelectUser.Name = dataReader.GetValue(1).ToString();
                        SelectUser.Password = dataReader.GetValue(2).ToString();
                        SelectUser.Authority.AuthCode = dataReader.GetValue(3).ToString();
                        SelectUser.Authority.AuthName = dataReader.GetValue(4).ToString();
                        SelectUser.RegistrationDate = Convert.ToDateTime(dataReader.GetValue(5));
                        m_listUserInformation.Add(SelectUser);
                    }
                    dataReader.Close();
                }
            }
        }

        public void SelectUserByID(string astrUserId)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                m_listUserInformation.Clear();

                string strQuery = String.Format("Select a.user_id, a.user_name, a.user_passwd, a.user_auth, b.com_dname, a.reg_date from user_info a, com_detail b where a.user_auth = b.com_dcode and a.use_yn = 1 and a.user_id LIKE '%{0}%' and b.com_mcode = '06'", astrUserId);
                
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                if (dataReader != null)
                {
                    if (dataReader.Read())
                    {
                        UserInformation SelectUser = new UserInformation();
                        SelectUser.ID = dataReader.GetValue(0).ToString();
                        SelectUser.Name = dataReader.GetValue(1).ToString();
                        SelectUser.Password = dataReader.GetValue(2).ToString();
                        SelectUser.Authority.AuthCode = dataReader.GetValue(3).ToString();
                        SelectUser.Authority.AuthName = dataReader.GetValue(4).ToString();
                        SelectUser.RegistrationDate = (DateTime)dataReader.GetValue(5);
                        m_listUserInformation.Add(SelectUser);
                    }

                    dataReader.Close();
                }
            }
        }

        public bool SearchByID(string astrUserId)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                string strQuery = String.Format("select user_id from user_info where user_id = '{0}'", astrUserId);

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

        public void SelectUserByName(string astrUserName)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                UserInformation SelectUser = new UserInformation();
                m_listUserInformation.Clear();

                string strQuery = string.Format("Select a.user_id, a.user_name, a.user_passwd, a.user_auth, b.com_dname, a.reg_date from user_info a, com_detail b where a.user_auth = b.com_dcode and a.use_yn = 1 and a.user_name LIKE '%{0}%' and b.com_mcode = '06'", astrUserName);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        SelectUser.ID = dataReader.GetValue(0).ToString();
                        SelectUser.Name = dataReader.GetValue(1).ToString();
                        SelectUser.Password = dataReader.GetValue(2).ToString();
                        SelectUser.Authority.AuthCode = dataReader.GetValue(3).ToString();
                        SelectUser.Authority.AuthName = dataReader.GetValue(4).ToString();
                        SelectUser.RegistrationDate = (DateTime)dataReader.GetValue(5);
                        m_listUserInformation.Add(SelectUser);
                    }
                    dataReader.Close();
                }
            }
        }

        public void AddUser(UserInformation newUserInformation)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                ConnectFactory.DBConnector().StartTrans();

                string strQuery = string.Format("INSERT INTO user_info(user_id, user_name, user_passwd, user_auth, use_yn, reg_date)" +
                                                " VALUES('{0}', '{1}', '{2}', '{3}', 1, now())",
                                                newUserInformation.ID,
                                                newUserInformation.Name,
                                                newUserInformation.Password,
                                                newUserInformation.Authority.AuthCode);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                {
                    ConnectFactory.DBConnector().Commit();
                    m_listUserInformation.Add(newUserInformation);
                }
                else
                {
                    ConnectFactory.DBConnector().Rollback();
                }
            }
        }

        public void ModifyUser(UserInformation userInformation)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                ConnectFactory.DBConnector().StartTrans();

                string strQuery = String.Format("UPDATE user_info SET user_name ='{0}', user_passwd='{1}', user_auth = '{2}' WHERE user_id = '{3}'",
                    userInformation.Name, userInformation.Password, userInformation.Authority.AuthCode, userInformation.ID);

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

        public void DeleteUser(UserInformation userInformation)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                ConnectFactory.DBConnector().StartTrans();

                String strQuery = String.Format("DELETE FROM user_info WHERE user_id = '{0}'", userInformation.ID);

                if (ConnectFactory.DBConnector().Execute(strQuery) > 0)
                {
                    ConnectFactory.DBConnector().Commit();
                    m_listUserInformation.Remove(userInformation);
                }
                else
                {
                    ConnectFactory.DBConnector().Rollback();
                }
            }
        }
    }
}
