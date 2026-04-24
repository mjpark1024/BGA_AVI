using System;
using System.Windows;
using System.Windows.Input;
using Common;
using Common.DataBase;
using RMS.Generic.UserManagement;

namespace HDSInspector.SubWindow
{
    /// <summary>
    /// Interaction logic for ChangeUserWindow.xaml
    /// </summary>
    public partial class ChangeUserWindow : Window
    {
        public ChangeUserWindow()
        {
            InitializeComponent();
            InitializeEvent();

            this.txtID.Focus();
        }

        private void InitializeEvent()
        {
            this.btnOK.Click += (s, e) => CloseWithOK();
            this.btnCancel.Click += (s, e) => CloseWithCancel();
            this.KeyDown += ChangeUserWindow_KeyDown;
        }

        private void ChangeUserWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                CloseWithOK();
            }
            else if (e.Key == Key.Escape)
            {
                CloseWithCancel();
            }
        }

        private void CloseWithOK()
        {
            bool bValidLogin = false;
            this.btnOK.IsEnabled = false;
            this.btnCancel.IsEnabled = false;

            MainWindow ptrMainWindow = Application.Current.MainWindow as MainWindow;
            Setting setting = MainWindow.Setting;
            if (setting != null && setting.General.UseDB)
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    if (LoginWindow.CheckLogin(txtID.Text, txtPW.Password))
                    {
                        MainWindow.Setting.Job.LastUser = txtID.Text;
                        if (ptrMainWindow != null)
                        {
                           // ptrMainWindow.StatusBarCtrl.DBStatus = true;
                           // ptrMainWindow.InspectionMonitoringCtrl.ResultTable.txtOperator.Text = UserManager.CurrentUser.Name;
                            bValidLogin = true;
                        }
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
                    if (ptrMainWindow != null)
                    {
                        ptrMainWindow.ConnectDb();
                    }
                }
                this.btnOK.IsEnabled = true;
                this.btnCancel.IsEnabled = true;
            }

            if (bValidLogin)
            {
                DialogResult = true;
            }
        }

        private void CloseWithCancel()
        {
            DialogResult = false;
        }
    }
}
