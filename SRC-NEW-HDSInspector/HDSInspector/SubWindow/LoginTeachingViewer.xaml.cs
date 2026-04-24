using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common;
using System.Data;
using Common.DataBase;

namespace HDSInspector.SubWindow
{
    /// <summary>
    /// LoginTeachingViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginTeachingViewer : Window
    {
        public LoginTeachingViewer()
        {
            InitializeComponent();
            InitializeEvent();
        }

        void InitializeEvent()
        {
            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;
            Loaded += LoginTeachingViewer_Loaded;
            txtbusinessnumber.KeyDown += Txtbusinessnumber_KeyDown;
        }

        private void Txtbusinessnumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                EnterKeyDown();
        }

        private void LoginTeachingViewer_Loaded(object sender, RoutedEventArgs e)
        {
            txtbusinessnumber.Focus();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckID())
            {
                MessageBox.Show("없는 사번 입니다.");
            }
            else
            {
                MainWindow.Log("PCS", SeverityLevel.DEBUG, String.Format("사번 {0}이(가) 티칭화면에 접근했습니다.", txtbusinessnumber.Text), true);
                this.DialogResult = true;
            }
        }
        void EnterKeyDown()
        {
            if (!CheckID())
            {
                MessageBox.Show("없는 사번 입니다.");
            }
            else
            {
                MainWindow.Log("PCS", SeverityLevel.DEBUG, String.Format("사번 {0}이(가) 티칭화면에 접근했습니다.", txtbusinessnumber.Text), true);
                this.DialogResult = true;
            }
        }
        private bool CheckID()
        {
            try
            {
                if (txtbusinessnumber.Text != "")
                {
                    string strQuery = string.Format("SELECT user_id, user_name, user_auth FROM user_info WHERE user_id = '{0}'", txtbusinessnumber.Text);
                    int n = -1;
                    IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery(strQuery);

                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            MainWindow.CurrentUser_HDS.BusinessNumber = dataReader.GetValue(0).ToString();
                            MainWindow.CurrentUser_HDS.Name = dataReader.GetValue(1).ToString();
                            MainWindow.CurrentUser_HDS.Author = (dataReader.GetValue(2).ToString() == "0064") ? true : false;
                            n = 0;
                        }
                        dataReader.Close();
                        if (n == -1)
                            return false;
                        else
                            return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
