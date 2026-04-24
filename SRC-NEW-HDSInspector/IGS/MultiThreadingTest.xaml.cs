using IGS.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IGS
{
    /// <summary>
    /// MultiThreadingTest.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MultiThreadingTest : Window
    {
        private ClientManager clientManager;
        private Stopwatch sw = new Stopwatch();

        private static string[] machines = new string[] {"EAV06", "EAV09", "EAV10", "EAV11", "EAV12", "EAV13", "EAV15", "EAV17", "EAV18", "EAV19", "EAV20", "EAV21", "EAV22",
        "EAV24", "EAV25", "EAV26", "EAV27", "EAV28", "EAV29", "EAV30", "EAV31", "EAV32", "EAV33", "EAV34", "EAV35", "EAV36", "EAV37", "EAV38", "EAV39", "EAV40"};

        private static string strLot = "105268328";

        public MultiThreadingTest(ref ClientManager cManager)
        {
            InitializeComponent();
            InitializeEvent();

            clientManager = cManager;
        }

        private void InitializeEvent()
        {
            this.btnRun.Click += BtnRun_Click;
        }

        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int nNum = Convert.ToInt32(tboxNum.Text);
                Task[] tasks = new Task[nNum];

                sw.Reset();
                sw.Start();
                for (int i = 0; i < nNum; i++)
                {
                    int nIndex = i;
                    tasks[i] = Task.Run(() => StartReport(nIndex));
                }

                Task.WaitAll(tasks);
                sw.Stop();
                tbTime.Text = sw.Elapsed.ToString();
            }
            catch(Exception ex)
            {
                string tmp = ex.Message;
            }
        }

        private void StartReport(int nIdx)
        {
            try
            {
                Console.WriteLine(string.Format("MC: {0}", machines[nIdx]));
                string curMC = machines[nIdx];

                POP_START_WINDOW_PARA para = new POP_START_WINDOW_PARA();
                para.strGroup = "양산";
                para.strModel = "TESTMODEL";
                para.strLot = strLot;
                para.strOperator = "200308";
                para.strVerifyOper = "김민우(190304)";
                para.ganji_spec = "";
                para.ganji_lot = "";

                string errMsg;
                int nErrCode;

                //if (clientManager.SetPOPStartReport(para, curMC, false, out nErrCode, out errMsg))
                //    Console.WriteLine(string.Format("{0} Done", curMC));
                //else
                //    Console.WriteLine(string.Format("{0} Fail", curMC));
            }
            catch(Exception ex)
            {
                string tmp = ex.Message;
                Console.WriteLine(string.Format("Exception: {0}", ex.Message));
            }
        }
    }
}
