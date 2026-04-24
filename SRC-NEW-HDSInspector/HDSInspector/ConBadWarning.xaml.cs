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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HDSInspector
{
    /// <summary>
    /// ConBadWarning.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public delegate void SendBadStop(int stop, int x, int y);


    public partial class ConBadWarning : UserControl
    {
        public event SendBadStop sendstop;
        private int nX, nY;
        public ConBadWarning()
        {
            InitializeComponent();
            this.btnstop.Click += btnstop_Click;
            this.btnContinue.Click += btnContinue_Click;
        }

        void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            SendBadStop er = sendstop;
            er(1, nX, nY);
        }

        void btnstop_Click(object sender, RoutedEventArgs e)
        {
            SendBadStop er = sendstop;
            er(0,0,0);
        }

        public void SetBadPoint(int x, int y)
        {
            nX = x;
            nY = y;
            map.SetBadPoint(x, y);
        }

        public void SetMap(int UnitX, int UnitY, int[,] val, int inspNum)
        {
            map.SetStripMap(UnitX, UnitY);
            for (int i = 0; i < UnitX; i++)
            {
                for (int j = 0; j < UnitY; j++)
                {
                    if (val[i, j] > 0)
                    {
                        map.SetColor(i, j, val[i, j], inspNum);
                    }
                }
            }
        }


    }
}
