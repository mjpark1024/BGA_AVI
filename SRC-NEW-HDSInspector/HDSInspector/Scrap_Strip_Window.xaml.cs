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
using System.IO;
using Common;

namespace HDSInspector
{
    /// <summary>
    /// Scrap_Strip_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Scrap_Strip_Window : Window
    {
        int nScabCount = 0;
        Bads bad = new Bads();
        List<StripInfo> ScrabStrips = new List<StripInfo>();
        List<Strip_Xout> strinpXOut = new List<Strip_Xout>();
        int Page = 0;
        int CurrPage = 0;
        string ResultImagePath= "";
        int UnitNumX;
        int UnitNumY;
        int ScrabXOut;
        public Scrap_Strip_Window(string aResultImagePath, int aUnitNumX, int aUnitNumY, int aScrabXOut)
        {
            InitializeComponent();
            ResultImagePath = aResultImagePath;
            UnitNumX = aUnitNumX;
            UnitNumY = aUnitNumY;
            ScrabXOut = aScrabXOut;
            this.Loaded += Scrap_Strip_Window_Loaded;
            this.btnClose.Click += BtnClose_Click;
            this.btnFirst.Click += BtnFirst_Click;
            this.btnLast.Click += BtnLast_Click;
            this.btnNext.Click += BtnNext_Click;
            this.btnPrev.Click += BtnPrev_Click;
            svXout.PreviewMouseWheel += SvXout_PreviewMouseWheel;
        }

        private void SvXout_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double offset = e.Delta / 3;

            if (offset > 0)
            {
                offset = (svXout.VerticalOffset - offset < 0) ? svXout.VerticalOffset : offset;
                this.svXout.ScrollToVerticalOffset(svXout.VerticalOffset - offset);
            }
            else
            {
                offset = (svXout.VerticalOffset - offset > svXout.ScrollableHeight) ? svXout.VerticalOffset - svXout.ScrollableHeight : offset;
                this.svXout.ScrollToVerticalOffset(svXout.VerticalOffset - offset);
            }
        }

        private void ScrollMove()
        {
            ScrollViewer sv;
            sv = svImage;
            if (sv != null)
            {
                sv.ScrollToVerticalOffset(0);
            }
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            CurrPage--;
            if (CurrPage < 0) CurrPage = 0;
            Dispaly(CurrPage);
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            CurrPage++;
            if (CurrPage >= Page) CurrPage = Page - 1;
            Dispaly(CurrPage);
        }

        private void BtnLast_Click(object sender, RoutedEventArgs e)
        {
            CurrPage = Page - 1;
            Dispaly(CurrPage);
        }

        private void BtnFirst_Click(object sender, RoutedEventArgs e)
        {
            CurrPage = 0;
            Dispaly(CurrPage);
        }

        private void Scrap_Strip_Window_Loaded(object sender, RoutedEventArgs e)
        {
            grdImage.Children.Clear();
            grdImage.RowDefinitions.Clear();
            grdImage.ColumnDefinitions.Clear();
            grdImage.Width = 1872;
            grdImage.Height = 104 * 40;
            for (int i = 0; i < 18; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                grdImage.ColumnDefinitions.Add(col);
            }
            for (int i = 0; i < 40; i++)
            {
                RowDefinition col = new RowDefinition();
                grdImage.RowDefinitions.Add(col);
            }
            LoadScrabInfo(ResultImagePath, UnitNumX, UnitNumY, ScrabXOut);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public bool LoadScrabInfo(string ResultImagePath, int UnitNumX, int UnitNumY, int ScrabXOut)
        {
            map.SetStripMap(UnitNumX, UnitNumY);
            strinpXOut.Clear();
            if (!Directory.Exists(ResultImagePath)) return false;
            string[] files = Directory.GetFiles(ResultImagePath);
            List<BadInfo> bi = new List<BadInfo>();
            int ScrabXOut1 = (int)(ScrabXOut * 0.8);
            int ScrabXOut2 = (int)(ScrabXOut * 0.6);
            svImage.ScrollToVerticalOffset(0);
            lblScrabText1.Content = String.Format("원소재 {0}개 이상", ScrabXOut);
            lblScrabText2.Content = String.Format("원소재 {0}개 이상", ScrabXOut1);
            lblScrabText3.Content = String.Format("원소재 {0}개 이상", ScrabXOut2);
            foreach (string f in files)
            {
                if (f.Contains(".png") && f.Contains("DEF"))
                {
                    BadInfo b = GetBadInfo(f);
                    if(b!=null) bi.Add(b);
                }
            }
            List<BadInfo> sort = bi.OrderByDescending(x => x.ID).ToList();
            StripInfo[] Strips = new StripInfo[sort[0].ID+1];
            for (int i = 0; i < Strips.Length; i++)
            {
                Strips[i] = new StripInfo(UnitNumX, UnitNumY);

            }
            for (int i=0; i<sort.Count; i++)
            {
                //0 Base 인지
                Strips[sort[i].ID].ID = sort[i].ID;
                Strips[sort[i].ID].Add(sort[i]);
            }
            int XOut, XOut1, XOut2;
            XOut = XOut1 = XOut2 = 0;
            for (int i = 0; i < Strips.Length; i++)
            {
                if(Strips[i].XOut >= ScrabXOut)
                {
                    Strips[i].Scrab = true; nScabCount++;
                    bad.Add(Strips[i].Map, Strips[i].X, Strips[i].Y);
                    ScrabStrips.Add(Strips[i]);
                    strinpXOut.Add(new Strip_Xout(Strips[i].ID, Strips[i].raw, Strips[i].XOut));
                    if (Strips[i].raw >= ScrabXOut)
                        XOut++;
                    else if (Strips[i].raw >= ScrabXOut1)
                        XOut1++;
                    else if (Strips[i].raw >= ScrabXOut2)
                        XOut2++;
                }
                if (Strips[i].Outer)
                {
                    nScabCount++;
                    Strips[i].Scrab = true;
                    bad.Add(Strips[i].Map, Strips[i].X, Strips[i].Y);
                    ScrabStrips.Add(Strips[i]);
                    strinpXOut.Add(new Strip_Xout(Strips[i].ID, Strips[i].raw, Strips[i].XOut));
                }
            }
            Page = ScrabStrips.Count;
            CurrPage = 0;
            Dispaly(0);
            lblScrab.Content = nScabCount.ToString();
            lblScrab1.Content = XOut.ToString();
            lblScrab2.Content = XOut1.ToString();
            lblScrab3.Content = XOut2.ToString();
            grdBads.DataContext = bad;
            lbXout.DataContext = strinpXOut;
            return true;
        }

        public void Dispaly(int n)
        {
            map.GoodColor();
            lbXout.SelectedIndex = n;
            grdImage.Children.Clear();
            ScrollMove();
            int index = 0;
            if (ScrabStrips[n].Outer)
            {
                map.ClearColor();
            }
            else
            {
                for (int x = 0; x < ScrabStrips[0].X; x++)
                {
                    for (int y = 0; y < ScrabStrips[0].Y; y++)
                    {    
                        if (ScrabStrips[n].Map[x, y].BadName.Contains("원소재"))
                        {
                            map.SetColor(x, y, Colors.Olive);
                        }
                        else if(ScrabStrips[n].Map[x, y].BadName.Count > 0)
                        {
                            map.SetColor(x, y, Colors.Red);
                            
                        }
                        for (int k = 0; k < ScrabStrips[n].Map[x, y].BadName.Count; k++)
                        {
                            AddImage(index, ScrabStrips[n].Map[x, y].DefImages[k]);
                            index++;
                        }
                    }
                }
            }
            lblXout.Content = string.Format("{0}/{1}", ScrabStrips[n].raw, ScrabStrips[n].XOut);
            lblIndex.Content = String.Format("{0}/{1}",n+1, Page);
        }

        public void AddImage(int index, string path)
        {
            Image img = new Image();
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(path);
            image.EndInit();
            img.Source = image;
            int nColumn = index % 18;
            Grid.SetColumn(img, nColumn);
            Grid.SetRow(img, (index - nColumn) / 18);
            img.Margin = new Thickness(2);
            grdImage.Children.Add(img);
        }

        public BadInfo GetBadInfo(string str)
        {
            BadInfo b = new BadInfo();
            int spos = 0;
            int epos = 0;
            string tmp = "";
            try
            {
                spos = str.IndexOf("ID[") +3;
                epos = str.IndexOf("] X=");
                tmp = str.Substring(spos, epos-spos);
                b.ID = Convert.ToInt32(tmp);
                spos = str.IndexOf("X=") + 2;
                epos = str.IndexOf("Y=");
                tmp = str.Substring(spos, epos - spos);
                b.X = Convert.ToInt32(tmp);
                b.X--;
                spos = str.IndexOf("Y=") + 2;
                epos = str.IndexOf("DEF");
                tmp = str.Substring(spos, epos - spos);
                b.Y = Convert.ToInt32(tmp);
                b.Y--;
                for (int i= str.Length-1; i>0; i--)
                {
                    if(str[i] == ' ')
                    {
                        spos = i+1;
                        break;
                    }
                }
                epos = str.IndexOf(".png");
                tmp = str.Substring(spos, epos - spos);
                b.BadName = tmp;
                b.DefFile = str;
            }
            catch { return null; }
            return b;
        }
    }

   // string strREFImagePath = strImageFolerPath + String.Format("CAM{0} ID[{1:D4}] X={2:D2} Y={3:D2} REF P={4:D5},{5:D5} S={6:F2} {7}.png",
  //                                               anCamID, anStripID, x + 1, y + 1, result.RelativeDefectCenter.X, result.RelativeDefectCenter.Y, result.DefectSize, strResult);


    public class StripInfo
    {
        public int ID;
        public bool Scrab = false;
        public bool Outer = false;
        public int raw = 0;
        public int XOut
        { get
            {
                int n = 0;
                raw = 0;
                for (int i = 0;  i < X; i++)
                {
                    for(int j =0; j< Y; j++)
                    {
                        if (Map[i, j].BadName.Count > 0)
                        {
                            n++;
                            if (Map[i, j].BadName.Contains("원소재")) this.raw++;
                            if (Map[i, j].BadName.Contains("외곽")) this.Outer=true;
                        }
                    }
                }
                return n;
            }
        }
        public int X;
        public int Y;
        public UnitInfo[,] Map;
        public StripInfo(int x, int y)
        {
            Map = new UnitInfo[x,y];
            for(int i=0; i< x; i++)
            {
                for(int j=0; j< y; j++)
                {
                    Map[i, j] = new UnitInfo();
                    Map[i, j].BadName = new List<string>();
                    Map[i, j].DefImages = new List<string>();
                }
            }
            X = x;
            Y = y;
        }
        public void Add(BadInfo b)
        {
            if(b.X < 0 || b.Y < 0)
            {
                Outer = true;
                return;
            }
            Map[b.X, b.Y].BadName.Add(b.BadName);
            Map[b.X, b.Y].DefImages.Add(b.DefFile);
        }

    }

    public class UnitInfo
    {
        public List<string> BadName;
        public List<string> DefImages;
    }

    public class BadInfo
    {
        public int X;
        public int Y;
        public int ID;
        public string BadName;
        public string DefFile;
    }

    public class Strip_Xout : NotifyPropertyChanged
    {
        private int nID;
        private int nRaw;
        private int nXOut;
        public int ID
        {
            get { return nID; }
            set
            {
                nID = value;
                Notify("ID");
            }
        }

        public int Raw
        {
            get { return nRaw; }
            set
            {
                nRaw = value;
                Notify("Raw");
            }
        }

        public int XOut
        {
            get { return nXOut; }
            set
            {
                nXOut = value;
                Notify("XOut");
            }
        }

        public Strip_Xout(int id, int raw ,int xout)
        {
            ID = id;
            Raw = raw;
            XOut = xout;
        }
    }

    public class Bads: NotifyPropertyChanged
    {
        private int m_nAlign;
        private int m_nRaw;
        private int m_nBP;
        private int m_nBall;
        private int m_nPSR;
        private int m_nNonPSR;
        private int m_nOpen;
        private int m_nShort;
        private int m_nCrack;
        private int m_nBurr;
        private int m_nVenthole;
        private int m_nVia;
        #region NG count variable ELF
        // 얼라인 불량 수
        public int Align
        {
            get { return m_nAlign; }
            set
            {
                m_nAlign = value;
                Notify("Align");
            }
        }

        // 다운-셋 불량 수
        public int Raw
        {
            get { return m_nRaw; }
            set
            {
                m_nRaw = value;
                Notify("Raw");
            }
        }

        // 리드 불량 수
        public int Open
        {
            get { return m_nOpen; }
            set
            {
                m_nOpen = value;
                Notify("Open");
            }
        }

        // 리드 불량 수
        public int Short
        {
            get { return m_nShort; }
            set
            {
                m_nShort = value;
                Notify("Short");
            }
        }

        // 리드 불량 수
        public int BP
        {
            get { return m_nBP; }
            set
            {
                m_nBP = value;
                Notify("BP");
            }
        }

        // 그루브 불량 수
        public int Ball
        {
            get { return m_nBall; }
            set
            {
                m_nBall = value;
                Notify("Ball");
            }
        }

        // 표면 불량 수
        public int NonPSR
        {
            get { return m_nNonPSR; }
            set
            {
                m_nNonPSR = value;
                Notify("NonPSR");
            }
        }

        // 표면 불량 수
        public int PSR
        {
            get { return m_nPSR; }
            set
            {
                m_nPSR = value;
                Notify("PSR");
            }
        }


        // 테이프 불량 수
        public int Crack
        {
            get { return m_nCrack; }
            set
            {
                m_nCrack = value;
                Notify("Crack");
            }
        }

        // 테이프 불량 수
        public int Burr
        {
            get { return m_nBurr; }
            set
            {
                m_nBurr = value;
                Notify("Burr");
            }
        }

        // 플레이트 불량 수
        public int Venthole
        {
            get { return m_nVenthole; }
            set
            {
                m_nVenthole = value;
                Notify("Venthole");
            }
        }

        // 리드 불량 수
        public int Via
        {
            get { return m_nVia; }
            set
            {
                m_nVia = value;
                Notify("Via");
            }
        }

        // 불량 개수 초기화.
        private void InitNGInfo()
        {
            Align = 0;
            Raw = 0;
            Open = 0;
            Short = 0;
            BP = 0;
            Ball = 0;
            PSR = 0;
            NonPSR = 0;
            Crack = 0;
            Burr = 0;
            Venthole = 0;
            Via = 0;
        }

        public void Add(UnitInfo[,] info, int X, int Y)
        {
            try
            {
                for (int i = 0; i < X; i++)
                {
                    for (int j = 0; j < Y; j++)
                    {
                        for (int k = 0; k < info[i,j].BadName.Count; i++)
                        {
                            if (info[i, j].BadName.Contains("원소재"))
                            {
                                Raw++;
                            }
                            else
                            {
                                switch (info[i, j].BadName[k])
                                {
                                    case "원소재": Raw++; break;
                                    case "Open": Open++; break;
                                    case "Align": Align++; break;
                                    case "Short": Short++; break;
                                    case "본드패드": BP++; break;
                                    case "Ball": Ball++; break;
                                    case "PSR이물질": PSR++; break;
                                    case "PSR핀홀": case "이물질": NonPSR++; break;
                                    case "VentHole": Venthole++; break;
                                    case "Via": Via++; break;
                                    case "Shift": PSR++; break;
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }
        #endregion
    }
}
