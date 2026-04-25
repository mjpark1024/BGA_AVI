using Common;
using Common.Drawing.InspectionInformation;
using PCS;
using RVS.Generic;
using RVS.Generic.Class;
using RVS.Generic.Insp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace HDSInspector
{
    /// <summary>   Interaction logic for InspectionResultTable.xaml. </summary>
    public partial class InspectionResultTable : UserControl
    {
        private InspectionResultDataControl m_TableData;
        private InspectDataManager m_InspectDataManager = new InspectDataManager();  
        #region Properties

        public InspectionResultDataControl TableData
        {
            get
            {
                if (m_TableData == null)
                {
                    //m_TableData = new InspectionResultDataControl(MainWindow.NG_Info);
                    //ResultTable.DataContext = m_TableData;
                    //for (int i = 1; i < MainWindow.NG_Info.Size; i++)
                    //{
                    //    Binding b1 = new Binding(string.Format("UnitBad[{0}]", i-1));
                    //    b1.Source = m_TableData;
                    //    BadCount[i-1].SetBinding(TextBlock.TextProperty, b1);

                    //    Binding b2 = new Binding(string.Format("FailBad[{0}]", i - 1));
                    //    b2.Source = m_TableData;
                    //    FailCount[i - 1].SetBinding(TextBlock.TextProperty, b2);

                    //    Binding b3 = new Binding(string.Format("DnnBad[{0}]", i - 1));
                    //    b3.Source = m_TableData;
                    //    DnnCount[i - 1].SetBinding(TextBlock.TextProperty, b3);
                    //}
                    return null;
                }
                else return m_TableData;
            }
            set { m_TableData = value; }
        }

        #endregion

        public InspectionResultTable()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            ModelManager.Send_PSR_Shift_Margin_Event += ModelManager_Send_PSR_Shift_Margin_Event;
        }

        private void ModelManager_Send_PSR_Shift_Margin_Event(int PSR_Shift_Margin)
        {
            this.txtPSR_Shift.Text = PSR_Shift_Margin.ToString();
        }

        public void Init()
        {
            if (MainWindow.NG_Info.Size == 0) return;
            m_TableData = new InspectionResultDataControl(MainWindow.NG_Info);
            m_TableData.InitNGInfo();
            ResultTable.DataContext = m_TableData;
            int ColumnCount = 3;
            int RowCount = (int)(Math.Ceiling(((double)MainWindow.NG_Info.Size) / ColumnCount));////불량 코드중 양품이 들어있어 -1, 수율창을 위해 + 1 결과 0
            
            for (int i = 0; i < ColumnCount; i++)
            {
                GridBadResult.ColumnDefinitions.Add(new ColumnDefinition());
                GridFailResult.ColumnDefinitions.Add(new ColumnDefinition());
                GridAIResult.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < RowCount; i++)
            {
                GridBadResult.RowDefinitions.Add(new RowDefinition());
                GridFailResult.RowDefinitions.Add(new RowDefinition());
                GridAIResult.RowDefinitions.Add(new RowDefinition());
            }

            int RowArea = 0;
            int ColumArea = 0;
            for (int i = 1; i < MainWindow.NG_Info.Size; i++)
            {
                Bad_Info bad = MainWindow.NG_Info.GetItem(i);
                if (RowCount == RowArea)
                {
                    RowArea = 0;
                    ColumArea++;
                }
               
                Binding BadBindingdata = new Binding(string.Format("UnitBad[{0}]", i));
                BadBindingdata.Source = TableData;
                InspectionResultItem BadResultItem = new InspectionResultItem(bad.Name, bad.Color, BadBindingdata);
                Grid.SetRow(BadResultItem, RowArea);
                Grid.SetColumn(BadResultItem, ColumArea);
                GridBadResult.Children.Add(BadResultItem);

                Binding FileBindingdata = new Binding(string.Format("FailBad[{0}]", i));
                FileBindingdata.Source = TableData;
                InspectionResultItem FailResultItem = new InspectionResultItem(bad.Name, bad.Color, FileBindingdata);
                Grid.SetRow(FailResultItem, RowArea);
                Grid.SetColumn(FailResultItem, ColumArea);
                GridFailResult.Children.Add(FailResultItem);

                Binding AiBindingdata = new Binding(string.Format("DnnBad[{0}]", i));
                AiBindingdata.Source = TableData;
                InspectionResultItem AiResultItem = new InspectionResultItem(bad.Name, bad.Color, AiBindingdata);
                Grid.SetRow(AiResultItem, RowArea);
                Grid.SetColumn(AiResultItem, ColumArea);
                GridAIResult.Children.Add(AiResultItem);

                RowArea++;
            }
            Binding FileBindingdataPercent = new Binding(string.Format("FYield")) { StringFormat = "F2" };
            FileBindingdataPercent.Source = TableData;
            InspectionResultItem TempResultItem = new InspectionResultItem("폐기 수율", new SolidColorBrush(Colors.Blue), FileBindingdataPercent, true);
            Grid.SetRow(TempResultItem, RowCount - 1);
            Grid.SetColumn(TempResultItem, ColumArea);
            GridFailResult.Children.Add(TempResultItem);
            if (!MainWindow.Setting.General.UseAI) tbAIResult.Visibility = Visibility.Hidden;
        }
        public void ResetFailData()
        {
            m_TableData.ResetFail();
        }
        #region SAP File

        public void WriteResultLoss(bool useVerify)
        {
            string szSAPPath = "";
            if (!useVerify)
            {
                szSAPPath = MainWindow.Setting.General.POP_Path;
                try
                {
                    DirectoryManager.CreateDirectory(szSAPPath);
                }
                catch
                {
                    MessageBox.Show("SAP파일 폴더를 생성할 수 없습니다. (" + szSAPPath + ")");
                }
            }
            else
            {
                szSAPPath = MainWindow.Setting.General.ResultPath + "\\" + MainWindow.CurrentGroup.Name + "\\" + MainWindow.CurrentModel.Name + "\\" + TableData.LotNo + "\\" + "VRS" + "\\";
                try
                {
                    DirectoryManager.CreateDirectory(szSAPPath);
                }
                catch
                {
                    MessageBox.Show("SAP파일 폴더를 생성할 수 없습니다. (" + szSAPPath + ")");
                }

            }

            int nUnitCount = MainWindow.CurrentModel.Strip.UnitColumn * MainWindow.CurrentModel.Strip.UnitRow;

            // Loss 파일 -- 재검 분류명 제거
            string szLotName = TableData.LotNo;
            string szSapFile;

            #region 폐기 스트립 정보 추가, 기존 정보 삭제
            szSapFile = String.Format("{0}{1}_{2}_{3}.txt", szSAPPath, szLotName, MainWindow.Setting.Job.ProcessCode, MainWindow.Setting.General.MachineName);
            if (File.Exists(szSapFile))
            {
                File.Delete(szSapFile);
            }
            
            string szContents = "1Q,O\r\n";
            szContents += String.Format("{0},{1},{2},{3}\r\n", TableData.TotalUnits, TableData.TotalUnits - TableData.BadUnits, TableData.BeforeYield.ToString("00.00"), TableData.Yield.ToString("00.00"));
            szContents += "1X\r\n";
            List<string> mes_codes = new List<string>();
            List<int> bad_count = new List<int>();

            //불량 수량 카운트
            for(int i = 1; i < TableData.Infos.Size; i++)
            {
                int n = mes_codes.IndexOf(TableData.Infos.GetItem(i).MES_Code);
                if (n >= 0)
                {
                    bad_count[n] += TableData.UnitBad[i];
                }
                else
                {
                    if (TableData.Infos.GetItem(i).MES_Code == "A000") continue;
                    mes_codes.Add(TableData.Infos.GetItem(i).MES_Code);
                    bad_count.Add(TableData.UnitBad[i]);
                }
            }

            //Text 생성
            for(int i=0; i< mes_codes.Count; i++)
            {
                szContents += String.Format("{0},{1}\r\n", mes_codes[i], bad_count[i]);
            }
            szContents += String.Format("{0},{1}\r\n", "B108", 0);

            // AutoNG 갯수도 Unit 단위로 수정
            szContents += String.Format("1M\r\n{0}\r\n", 0); //원소재
            szContents += String.Format("S\r\n{0:f}\r\n", 17.00); //원소재

            szContents += String.Format("\r\n");
            szContents += String.Format("\r\n"); 

            szContents += "1Q,X\r\n";
            szContents += String.Format("{0},{1},{2},{3}\r\n", TableData.FTotalUnits, TableData.FTotalUnits - TableData.FailUnits, TableData.FYield.ToString("00.00"), TableData.FYield.ToString("00.00"));
            szContents += "1X\r\n";

            mes_codes.Clear();
            bad_count.Clear();
            for (int i = 1; i < TableData.Infos.Size; i++)
            {
                int n = mes_codes.IndexOf(TableData.Infos.GetItem(i).MES_Fail);
                if (n >= 0)
                {
                    bad_count[n] += TableData.FailBad[i];
                }
                else
                {
                    mes_codes.Add(TableData.Infos.GetItem(i).MES_Fail);
                    bad_count.Add(TableData.FailBad[i]);
                }
            }
            for (int i = 0; i < mes_codes.Count; i++)
            {
                szContents += String.Format("{0},{1}\r\n", mes_codes[i], bad_count[i]);
            }
            szContents += String.Format("{0},{1}\r\n", "B108", TableData.FTotalUnits - TableData.FailUnits); //폐기 양품 코드를 확인 할 것
            // AutoNG 갯수도 Unit 단위로 수정
            szContents += String.Format("1M\r\n{0}\r\n", 0); //원소재
            szContents += String.Format("S\r\n{0:f}\r\n", 17.00); //원소재

            try
            {
                System.IO.File.WriteAllText(szSapFile, szContents);
            }
            catch
            {
                MessageBox.Show("SAP Text File 쓰기 실패 (" + szSapFile + ")");
            }
            #endregion


            if (MainWindow.Setting.General.UseAI)
            {
                #region AI Loss 정보 추가, Unit 베이스

                szSapFile = String.Format("{0}A_{1}_{2}_{3}.txt", szSAPPath, szLotName, MainWindow.Setting.Job.ProcessCode, MainWindow.Setting.General.MachineName);
                if (File.Exists(szSapFile))
                {
                    File.Delete(szSapFile);
                }

                szContents = "1U,O\r\n";
                szContents += String.Format("{0},{1},{2},{3}\r\n", TableData.TotalUnits, TableData.TotalUnits - TableData.BadUnits, TableData.BeforeYield.ToString("00.00"), TableData.Yield.ToString("00.00"));
                szContents += "1X\r\n";

                mes_codes.Clear();
                bad_count.Clear();
                for (int i = 1; i < TableData.Infos.Size; i++)
                {
                    int n = mes_codes.IndexOf(TableData.Infos.GetItem(i).MES_Code);
                    if (n >= 0)
                    {
                        bad_count[n] += TableData.DnnTUnitTbl[i];
                    }
                    else
                    {
                        if (TableData.Infos.GetItem(i).MES_Code == "A000") continue;
                        mes_codes.Add(TableData.Infos.GetItem(i).MES_Code);
                        bad_count.Add(TableData.DnnTUnitTbl[i]);
                    }
                }

                for (int i = 0; i < mes_codes.Count; i++)
                {
                    szContents += String.Format("{0},{1}\r\n", mes_codes[i], bad_count[i]);
                }
                szContents += String.Format("{0},{1}\r\n", "B108", 0);

                szContents += String.Format("1M\r\n{0}\r\n", 0); //원소재
                szContents += String.Format("S\r\n{0:f}\r\n", 17.00); //원소재

                szContents += String.Format("\r\n");
                szContents += String.Format("\r\n");

                szContents += "1U,X\r\n";
                szContents += String.Format("{0},{1},{2},{3}\r\n", TableData.FTotalUnits, TableData.FTotalUnits - TableData.FailUnits, TableData.FYield.ToString("00.00"), TableData.FYield.ToString("00.00"));
                szContents += "1X\r\n";

                mes_codes.Clear();
                bad_count.Clear();
                for (int i = 1; i < TableData.Infos.Size; i++)
                {
                    int n = mes_codes.IndexOf(TableData.Infos.GetItem(i).MES_Fail);
                    if (n >= 0)
                    {
                        bad_count[n] += TableData.DnnFUnitTbl[i];
                    }
                    else
                    {
                        mes_codes.Add(TableData.Infos.GetItem(i).MES_Fail);
                        bad_count.Add(TableData.DnnFUnitTbl[i]);
                    }
                }
                for (int i = 0; i < mes_codes.Count; i++)
                {
                    szContents += String.Format("{0},{1}\r\n", mes_codes[i], bad_count[i]);
                }
                szContents += String.Format("{0},{1}\r\n", "B108", TableData.FTotalUnits - TableData.FailUnits); //폐기 양품 코드를 확인 할 것

                szContents += String.Format("1M\r\n{0}\r\n", 0); //원소재
                szContents += String.Format("S\r\n{0:f}\r\n", 17.00); //원소재
                #endregion

                szContents += String.Format("\r\n");
                szContents += String.Format("\r\n");

                #region AI Loss 정보 추가, Image 베이스
                szContents += "2I,O\r\n";
                szContents += String.Format("{0},{1},{2},{3}\r\n", TableData.TotalUnits, TableData.TotalUnits - TableData.BadUnits, TableData.BeforeYield.ToString("00.00"), TableData.Yield.ToString("00.00"));
                szContents += "2X\r\n";

                mes_codes.Clear();
                bad_count.Clear();
                for (int i = 1; i < TableData.Infos.Size; i++)
                {
                    int n = mes_codes.IndexOf(TableData.Infos.GetItem(i).MES_Code);
                    if (n >= 0)
                    {
                        bad_count[n] += TableData.DnnTImageTbl[i];
                    }
                    else
                    {
                        if (TableData.Infos.GetItem(i).MES_Code == "A000") continue;
                        mes_codes.Add(TableData.Infos.GetItem(i).MES_Code);
                        bad_count.Add(TableData.DnnTImageTbl[i]);
                    }
                }

                for (int i = 0; i < mes_codes.Count; i++)
                {
                    szContents += String.Format("{0},{1}\r\n", mes_codes[i], bad_count[i]);
                }
                szContents += String.Format("{0},{1}\r\n", "B108", 0);

                szContents += String.Format("2M\r\n{0}\r\n", 0); //원소재
                szContents += String.Format("S\r\n{0:f}\r\n", 17.00); //원소재

                szContents += String.Format("\r\n");
                szContents += String.Format("\r\n");

                szContents += "2I,X\r\n";
                szContents += String.Format("{0},{1},{2},{3}\r\n", TableData.FTotalUnits, TableData.FTotalUnits - TableData.FailUnits, TableData.FYield.ToString("00.00"), TableData.FYield.ToString("00.00"));
                szContents += "2X\r\n";

                mes_codes.Clear();
                bad_count.Clear();
                for (int i = 1; i < TableData.Infos.Size; i++)
                {
                    int n = mes_codes.IndexOf(TableData.Infos.GetItem(i).MES_Fail);
                    if (n >= 0)
                    {
                        bad_count[n] += TableData.DnnFImageTbl[i];
                    }
                    else
                    {
                        mes_codes.Add(TableData.Infos.GetItem(i).MES_Fail);
                        bad_count.Add(TableData.DnnFImageTbl[i]);
                    }
                }
                for (int i = 0; i < mes_codes.Count; i++)
                {
                    szContents += String.Format("{0},{1}\r\n", mes_codes[i], bad_count[i]);
                }
                szContents += String.Format("{0},{1}\r\n", "B108", TableData.FTotalUnits - TableData.FailUnits); //폐기 양품 코드를 확인 할 것
                szContents += String.Format("2M\r\n{0}\r\n", 0); //원소재
                szContents += String.Format("S\r\n{0:f}\r\n", 17.00); //원소재
                try
                {
                    System.IO.File.WriteAllText(szSapFile, szContents);
                }
                catch
                {
                    MessageBox.Show("SAP Text File 쓰기 실패 (" + szSapFile + ")");
                }
                #endregion
            }
        }

        public List<string> ReadSapCode()
        {
            List<string> code = new List<string>();
            string szSAPPath = MainWindow.Setting.General.POP_Path;
            string path = szSAPPath + "SapCode.ini";
            IniFile ini;
            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Close();
                ini = new IniFile(path);
                ini.WriteString("Code", "Open", "B102");  //0
                code.Add("B102");
                ini.WriteString("Code", "Short", "B129");        //1
                code.Add("B129");
                ini.WriteString("Code", "PSR", "B227");          //2
                code.Add("B227");
                ini.WriteString("Code", "BondPad", "B282");      //3
                code.Add("B282");
                ini.WriteString("Code", "Ball", "B283");      //4
                code.Add("B283");
                ini.WriteString("Code", "VentHole", "B377");     //6
                code.Add("B377");
                ini.WriteString("Code", "Align", "B280"); // 7
                code.Add("B280");
                ini.WriteString("Code", "Raw", "B900"); // 8
                code.Add("B900");
                ini.WriteString("Code", "BBT", "B320"); // 9
                code.Add("B320");
                ini.WriteString("Code", "DAM", "B304"); // 10
                code.Add("B304");
                ini.WriteString("Code", "PSRPinhole", "B257"); //11
                code.Add("B257");
                ini.WriteString("Code", "Via", "B329");
                code.Add("B329");
                ini.WriteString("Code", "Crack", "B284");
                code.Add("B284");
                ini.WriteString("Code", "Burr", "B181");
                code.Add("B181");
                ini.WriteString("Code", "StaticNG", "B999");
                code.Add("B999");

                ini.WriteString("Code", "XOpen", "B102");         
                code.Add("B102");
                ini.WriteString("Code", "XShort", "B129");  
                code.Add("B129");
                ini.WriteString("Code", "XPSR", "B227");    
                code.Add("B227");
                ini.WriteString("Code", "XBondPad", "B282");    
                code.Add("B282");
                ini.WriteString("Code", "XBall", "B283");    
                code.Add("B283");
                ini.WriteString("Code", "XVentHole", "B377");   
                code.Add("B377");
                ini.WriteString("Code", "XAlign", "B280"); // 7
                code.Add("B280");
                ini.WriteString("Code", "XRaw", "B900");
                code.Add("B900");
                ini.WriteString("Code", "XBBT", "B320");
                code.Add("B320");
                ini.WriteString("Code", "XDAM", "B304");
                code.Add("B304");
                ini.WriteString("Code", "XPSRPinhole", "B257");
                code.Add("B257");
                ini.WriteString("Code", "XVia", "B329");
                code.Add("B329");
                ini.WriteString("Code", "XCrack", "B284");
                code.Add("B284");
                ini.WriteString("Code", "XBurr", "B181");
                code.Add("B181");
                ini.WriteString("Code", "XStaticNG", "B999");
                code.Add("B999");

                ini.WriteString("Code", "XOuter", "B373");  
                code.Add("B373");
                ini.WriteString("Code", "XOUTGood", "B108");   
                code.Add("B108");
                ini.WriteString("Code", "XPSRShift", "B229");   
                code.Add("B229");
                ini.WriteString("Code", "XConbad", "B375");   
                code.Add("B375");
                ini.WriteString("Code", "XMark2D", "B376");  
                code.Add("B376");
                return code;
            }
            ini = new IniFile(path);
            string c = ini.ReadString("Code", "Open", "B102");  //0
            code.Add(c);
            c = ini.ReadString("Code", "Short", "B129");        //1
            code.Add(c);
            c = ini.ReadString("Code", "PSR", "B227");          //2
            code.Add(c);
            c = ini.ReadString("Code", "BondPad", "B282");      //3
            code.Add(c);
            c = ini.ReadString("Code", "Ball", "B283");      //4
            code.Add(c);
            c = ini.ReadString("Code", "VentHole", "B377");     //6
            code.Add(c);
            c = ini.ReadString("Code", "Align", "B280"); // 7
            code.Add("B280");
            c = ini.ReadString("Code", "Raw", "B900"); // 8
            code.Add("B900");
            c = ini.ReadString("Code", "BBT", "B320"); // 9
            code.Add("B320");
            c = ini.ReadString("Code", "DAM", "B304"); // 10
            code.Add("B304");
            c = ini.ReadString("Code", "PSRPinhole", "B257"); //11
            code.Add("B257");
            c = ini.ReadString("Code", "Via", "B329");
            code.Add("B329");
            c = ini.ReadString("Code", "Crack", "B284");
            code.Add("B284");
            c = ini.ReadString("Code", "Burr", "B181");
            code.Add("B181");
            c = ini.ReadString("Code", "StaticNG", "B999");
            code.Add("B999");

            c = ini.ReadString("Code", "XOpen", "B102");          //7
            code.Add(c);
            c = ini.ReadString("Code", "XShort", "B129");   //8
            code.Add(c);
            c = ini.ReadString("Code", "XPSR", "B227"); //9
            code.Add(c);
            c = ini.ReadString("Code", "XBondPad", "B282"); //10
            code.Add(c);
            c = ini.ReadString("Code", "XBallPad", "B283"); //11
            code.Add(c);
            c = ini.ReadString("Code", "XVentHole", "B377");    //13
            code.Add(c);
            c = ini.ReadString("Code", "XAlign", "B280"); // 7
            code.Add("B280");
            c = ini.ReadString("Code", "XRaw", "B900");
            code.Add("B900");
            c = ini.ReadString("Code", "XBBT", "B320");
            code.Add("B320");
            c = ini.ReadString("Code", "XDAM", "B304");
            code.Add("B304");
            c = ini.ReadString("Code", "XPSRPinhole", "B257");
            code.Add("B257");
            c = ini.ReadString("Code", "XVia", "B329");
            code.Add("B329");
            c = ini.ReadString("Code", "XCrack", "B284");
            code.Add("B284");
            c = ini.ReadString("Code", "XBurr", "B181");
            code.Add("B181");
            c = ini.ReadString("Code", "XStaticNG", "B999");
            code.Add("B999");

            c = ini.ReadString("Code", "XOuter", "B373");   //14
            code.Add(c);
            c = ini.ReadString("Code", "XOUTGood", "B108"); //16
            code.Add(c);
            c = ini.ReadString("Code", "XPSRShift", "B229");    //17
            code.Add(c);
            c = ini.ReadString("Code", "XConbad", "B375");    //18
            code.Add(c);
            c = ini.ReadString("Code", "XMark2D", "B376");    //19
            code.Add(c);
            return code;
        }
        #endregion
    }
}
