using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using System.Data;
using Common.DataBase;

namespace RMS.Generic
{

    public class Auth
    {
        private string m_AuthCode;
        private string m_AuthName;

        public string AuthCode
        {
            get { return m_AuthCode;}
            set { m_AuthCode = value;}
        }

        public string AuthName
        {
            get { return m_AuthCode;}
            set { m_AuthName = value;}
        }

    }

    public class User
    {
        private string m_UserID;

        private string m_UserName;

        private string m_passwd;

        private Auth m_Auth = new Auth();


        public String UserID
        {
            get { return m_UserID; }
            set { m_UserID = value; }
        }

        public String UserName
        {
            get { return m_UserName; }
            set { m_UserName = value; }
        }

        public String PassWd
        {
            get { return m_passwd; }
            set { m_passwd = value; }
        }

        public Auth Authority
        {
            get { return m_Auth; }
            set { m_Auth = value; }
        }
    }

    public class UserManager
    {
        ObservableCollection<User> m_UserList;
        
        private static UserManager _Instance;
        private User m_CurUser;

        public User CurUser
        {
            get { return m_CurUser; }
            set { m_CurUser = value; }
        }

        public static UserManager Instance
        {
            get { return _Instance; }
            set { _Instance = value; }
        }

        public User SelectUserInfo(string strUserId)
        {
             String strQuery;

             User UserIns = new User();

            if ( ConnectFactory.DBConnector() != null)
            {
                strQuery = String.Format("Select a.user_id. a.user_name, a.user_passwd, a.user_auth, b.com_dname from user_info a, com_detail b where a.user_auth = b.com_dcode and a.use_yn = 1 and a.user_id = '{0}' and b.com_mcode = '06'", strUserId);

               IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery(strQuery);

               if (dataReader != null)
               {
                   while (dataReader.Read())
                   {
                       UserIns.UserID = dataReader.GetValue(0).ToString();
                       UserIns.UserName = dataReader.GetValue(1).ToString();
                       UserIns.PassWd = dataReader.GetValue(2).ToString();
                       UserIns.Authority.AuthCode = dataReader.GetValue(3).ToString();
                       UserIns.Authority.AuthName = dataReader.GetValue(4).ToString();
                   }
                   dataReader.Close();
               }
            }
            return UserIns;
        }

    }
}
