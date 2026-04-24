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
using Keyence.AutoID;
using OpenCvSharp;

namespace HDSInspector.SubControl
{
    /// <summary>
    /// ID_Reader.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ID_Reader : UserControl
    {
        private Keyence.AutoID.SDK.ReaderAccessor acc;// = new Keyence.AutoID.SDK.ReaderAccessor();
        private Keyence.AutoID.SDK.LiveviewForm live = new Keyence.AutoID.SDK.LiveviewForm();
        private string tempImagePath;

        public ID_Reader()
        {
            InitializeComponent();
        }

        public bool Connect(string ip)
        {
            bool isOpen = false;
            acc = new Keyence.AutoID.SDK.ReaderAccessor(ip);
            isOpen = acc.Connect();
            acc.CommandPort = 9003;
            acc.DataPort = 9004;
            imgLive.Child = live;
            live.AutoSize = true;
            live.BackColor = System.Drawing.Color.Black;
            this.live.BinningType = Keyence.AutoID.SDK.LiveviewForm.ImageBinningType.OneQuarter;
            this.live.ImageFormat = Keyence.AutoID.SDK.LiveviewForm.ImageFormatType.Jpeg;
            this.live.ImageQuality = 5;
            this.live.Location = new System.Drawing.Point(0, 0);
            this.live.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.live.Name = "liveviewForm1";
            this.live.PullTimeSpan = 100;
            this.live.Size = new System.Drawing.Size((int)this.ActualWidth - 1, (int)this.ActualHeight - 1);
            this.live.TabIndex = 0;
            this.live.TimeoutMs = 2000;
            this.live.IpAddress = ip;
            live.BeginReceive();
            return isOpen;
        }

        public void CloseID()
        {
            acc.Disconnect();
            acc.Dispose();
            live.EndReceive();
            live.Dispose();
        }

        public System.Drawing.Image GetImage()
        {
            return live.BackgroundImage;
        }

        public void SetImage(System.Drawing.Image src)
        {
            live.BackgroundImage = src;
        }

        public string ReadID()
        {
            string str = acc.ExecCommand("LON");
            //acc.ExecCommand("LOFF");
            return str;
        }

        public void DownloadRecentImg(string order, string id)
        {
            tempImagePath = MainWindow.Setting.General.ResultPath + "\\" + MainWindow.CurrentGroup.Name + "\\"
+ MainWindow.CurrentModel.Name + "\\" + order + "\\2D Mark log(temp)\\" + id + ".png";

            live.DownloadRecentImage(tempImagePath);
        }

        public Mat GetRecentMatImage()
        {
            Mat img = new Mat(tempImagePath);

            return img;
        }

        public bool Tune()
        {
            string str = acc.ExecCommand("FTUNE");
            if (str.Contains("SUCC"))
            {
                str = acc.ExecCommand("FTUNE");
                if (str.Contains("SUCC"))
                {
                    acc.ExecCommand("TQUIT");
                    return true;
                }
                else return false;
            }
            else return false;
        }

        public bool Grab()
        {
            string str = acc.ExecCommand("SHOT01");

            if (str.Contains("OK"))
            {
                return true;
            }
            return false;
        }
    }
}
