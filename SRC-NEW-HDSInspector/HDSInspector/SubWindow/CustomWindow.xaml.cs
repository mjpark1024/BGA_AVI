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
    /// CustomWIndow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CustomWIndow : Window
    {
        public CustomWIndow()
        {
            InitializeComponent();
            InitializeEvent();
        }
        void InitializeEvent()
        {
            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;
            Loaded += MainLoaded;
            txtpassword.KeyDown += txtpassword_KeyDown;
        }

        private void txtpassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                EnterKeyDown();
        }

        private void MainLoaded(object sender, RoutedEventArgs e)
        {
            txtpassword.Focus();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (txtpassword.Password != "hds")
            {
                MessageBox.Show("비밀번호가 틀렸습니다.");
            }
            else
                this.DialogResult = true;

        }
        void EnterKeyDown()
        {
            if (txtpassword.Password != "hds")
            {
                MessageBox.Show("비밀번호가 틀렸습니다.");
            }
            else
                this.DialogResult = true;
        }
    }
}
