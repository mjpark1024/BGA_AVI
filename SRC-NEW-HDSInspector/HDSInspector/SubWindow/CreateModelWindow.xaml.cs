using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HDSInspector.SubWindow
{
    /// <summary>
    /// CreateModelWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CreateModelWindow : Window
    {
        public string model;
        public string code;
        List<string> lstModel;
        int type = 0;
        public CreateModelWindow(List<string> lstmodel, int type) //Type 0 : 신규 1 : 변경 
        {
            InitializeComponent();
            this.btnOK.Click += BtnOK_Click;
            this.btnCancel.Click += BtnCancel_Click;
            lstModel = lstmodel;
            this.type = type;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            model = txtName.Text.Trim();
            code = txtCode.Text.Trim();
            if (model =="")
            {
                MessageBox.Show("모델명을 입력 하세요..");
                txtName.Focus();
                return;
            }
            if (code=="")
            {
                MessageBox.Show("모델 코드를 입력 하세요.");
                txtCode.Focus();
                return;
            }
            if (type == 0)
            {
                if (lstModel.Contains(model))
                {
                    MessageBox.Show("이미 등록된 모델입니다.");
                    txtName.Focus();
                    return;
                }
            }
            this.DialogResult = true;
        }

        public void SetOldModel(string name, string description)
        {
            txtName.Text = name;
            this.model = name;
            txtCode.Text = description;
            this.code = description;
        }
    }
}
