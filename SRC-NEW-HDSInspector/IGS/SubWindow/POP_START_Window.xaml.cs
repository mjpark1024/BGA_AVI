using IGS.Classes;
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

namespace IGS.SubWindow
{
    /// <summary>
    /// POP_Start_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class POP_START_Window : Window
    {
        public POP_START_Window()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.btnOK.Click += BtnOK_Click;
            this.btnCancel.Click += BtnCancel_Click;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            string strLot = txtLot.Text.Trim();

            POP_START_WINDOW_PARA para = new POP_START_WINDOW_PARA();
            para.strGroup = "양산";                   //CurrentModel.Group
            para.strModel = "L230629S31";             //CurrentModel.Model
            para.strLot = strLot;
            bool bRestart = false;                      //재검 여부

            string errMsg;
            if (!ReportMenuCtrl.clientManager.SetPOPStartReport(ref para, bRestart, out errMsg))
            {
                //시작 보고 실패
                MessageBox.Show(string.Format("시작 보고를 하지 못했습니다.\n실패 사유:{0}", errMsg));
                ClientManager.Log(string.Format("IGS - POP_START_Window StartReport Fail. ErrMsg: {0}", errMsg));

                /////보고 실패시 개별 루틴 설정

            }
            else
                ClientManager.Log(string.Format("IGS - POP Start Report Success. Lot: {0}", strLot));

            this.DialogResult = true;
        }
    }
}
