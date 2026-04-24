using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Common.DataBase;
using RMS.Generic.UserManagement;
using System.Data;
using Common;


namespace HDSInspector.SubWindow
{
    public partial class GoldenSaveWindow : Window
    {
        public bool SaveOK{ get; set; }

        public GoldenSaveWindow()
        {
            InitializeComponent();
            InitializeEvent();
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            txtGroup.Text = MainWindow.CurrentGroup.Name;
            txtModel.Text = MainWindow.CurrentModel.Name;
            txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            txtID.Text = UserManager.CurrentUser.ID;
            txtPassword.Focus();
        }

        private void InitializeEvent()
        {
            this.KeyDown += GoldenSaveWindow_KeyDown;
            
            this.btnSave.Click += btnSave_Click;
            this.btnCancel.Click += (s, e) =>
            {
                this.Close();
            };
            
            this.txtComment.GotFocus += txtComment_GotFocus;
            this.txtComment.LostFocus += txtComment_LostFocus;
            
        }

        void txtComment_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtComment.Text))
            {
                this.txtComment.Foreground = new SolidColorBrush(Colors.DarkGray);
                this.txtComment.Text = "Ex) 상부: 도금 임계값 수정";
            }
        }

        void txtComment_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.txtComment.Text.Equals("Ex) 상부: 도금 임계값 수정"))
            {
                this.txtComment.Clear();
            }
            this.txtComment.Foreground = new SolidColorBrush(Colors.Black);
        }

        void GoldenSaveWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                btnSave_Click(null, null);
        }

        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtComment.Text == null || txtComment.Text == "" || txtComment.Text == "Ex) 상부: 도금 임계값 수정")
            {
                MessageBox.Show("Comment를 입력 바랍니다.", "Information");
                txtComment.Focus();
            }
            else
            {
                if (LoginWindow.CheckLogin(txtID.Text, txtPassword.Password))
                {
                    if (SaveGoldenModel())
                    {
                        this.DialogResult = true;
                        SaveOK = true;
                        this.Close();
                    }
                    else
                        MessageBox.Show("저장에 실패하였습니다.", "Error");
                }
                else MessageBox.Show("Password를 확인해 주시기 바랍니다.", "Information");
            }
        }

        private bool SaveGoldenModel()
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select bgadb.`fn_Upload`('{0}', '{1}', now(), '{2}')",
                                  MainWindow.Setting.General.MachineCode, MainWindow.CurrentModel.Name, txtID.Text + "|" + txtComment.Text);
                
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                if (dataReader == null)
                    return false;

                if (dataReader.Read())
                {
                    int Ret = Convert.ToInt32(dataReader.GetValue(0).ToString());
                    dataReader.Close();

                    if (Ret > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }
    }
}
