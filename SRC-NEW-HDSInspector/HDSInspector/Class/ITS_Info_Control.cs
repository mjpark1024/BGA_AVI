using System;
using System.Collections.Generic;
using Common.DataBase;
using System.Diagnostics;
using System.Data.SqlClient;
using System.IO;
using Common;
using System.Xml;
using System.Linq;

namespace HDSInspector
{
    public class ResultData
    {
        public string ID;
        public InspectBuffer result;
        public int good;

        public ResultData()
        {
            result = new InspectBuffer();
        }
        public ResultData(string ID, int model_x, int model_y, int x, int y)
        {
            this.ID = ID;
            result = new InspectBuffer();
            result.Init(model_x, model_y, MainWindow.NG_Info.Priority);
            int val = MainWindow.NG_Info.BadNameToID("Skip불량");
            result.Set(x, y, val);
        }
    }

    public class ITS_Info_Control
    {
        DBConnector ITSConnection = null;

        public string Management_Code = "0";
        public List<ResultData> skip_datas;
        public List<ResultData> marking_datas;

        //처음부터 시작시 기존에 있던 결과 파일을 삭제
        public bool DeleteResultFiles(string path, string lotNum)
        {
            try
            {
                string strFolderPath = String.Format(@"{0}/{1}/", path, lotNum);//경로 변경
                if (Directory.Exists(strFolderPath))
                {
                    DirectoryInfo directory = new DirectoryInfo(strFolderPath);
                    directory.EnumerateFiles().ToList().ForEach(f => f.Delete());
                    directory.EnumerateDirectories().ToList().ForEach(d => d.Delete(true));
                    Directory.Delete(strFolderPath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region AVI
        //검사 시작전 skipdata를 gathering pc로부터 불러온다.
        public bool ReadSkipData_AVI(string path, string lot, int model_x, int model_y)
        {
            string lot_number = null;
            string process_code = null;
            int total_count = 0;
            skip_datas = new List<ResultData>();

            try
            {
                string file_path = Path.Combine(path, lot.Substring(0, 5));
                string[] SearchFiles = Directory.GetFiles(file_path, "*" + lot + ".ski");//file 형식에 맞는 file 검색
                string[] lines = File.ReadAllLines(SearchFiles[0]);

                foreach (string line in lines)
                {
                    if (line.Contains("Management Code"))//관리번호(Item Number)
                    {
                        Management_Code = line.Split(':')[1].Trim();
                    }
                    else if (line.Contains("Lot Number"))
                    {
                        lot_number = line.Split(':')[1].Trim();
                    }
                    else if (line.Contains("Process Code"))//MES 공정 코드
                    {
                        process_code = line.Split(':')[1].Trim();
                    }
                    else if (line.Contains("Total Count"))//총 불량 수량
                    {
                        total_count = Convert.ToInt32(line.Split(':')[1].Trim());
                    }
                    else if (lot_number != null && line.Contains(lot_number))
                    {
                        var data = line.Split(',');
                        int index = skip_datas.FindIndex(x => x.ID == data[0]);

                        if (index < 0)//해당 lot 못찾음
                        {
                            skip_datas.Add(new ResultData(data[0], model_x, model_y, Convert.ToInt32(data[1]) - 1, Convert.ToInt32(data[2]) - 1));
                        }
                        else//해당 lot 찾음
                        {
                            int val = MainWindow.NG_Info.BadNameToID("Skip불량");
                            skip_datas[index].result.Set(Convert.ToInt32(data[1]) - 1, Convert.ToInt32(data[2]) - 1, val);
                        }
                    }
                    else if (line.Equals("EOL"))
                        break;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        //data file를 위한 strip data 생성
        public bool SaveResultFile(string path, InspectBuffer TopBuffer, InspectBuffer BotBuffer, string lotNum, NGInformationHelper badinfo, string IDString)
        {
            string strID = null;
            string side_str = "";
            string strFile = "";
            List<string> lines = new List<string>();

            try
            {
                string strFolderPath = string.Format(@"{0}/{1}/", path, lotNum);//경로 변경
                if (!Directory.Exists(strFolderPath))
                    Directory.CreateDirectory(strFolderPath);

                strID = IDString;

                if (strID == "" || strID == null)///ID mark 인식 오류
                    return false;

                strFile = strFolderPath + strID + ".txt";
                if (File.Exists(strFile))
                    File.Delete(strFile);

                List<string> stripstr = new List<string>();

                #region Top Side
                side_str = "T";
                string defect_code = "";
                int defect_cnt = 0;

                if (TopBuffer.OuterNG)//외곽불량 처리 0,0
                {
                    defect_cnt++;
                    //stripstr.Add(lotNum + "," + strID + "," + "0" + "," + "0" + "," + side_str + "," + "B280");
                    //stripstr.Add(lotNum + "," + strID + "," + "0" + "," + "0" + "," + side_str + "," + badinfo.IDtoMesCode(2));
                    stripstr.Add(lotNum + "," + strID + "," + "0" + "," + "0" + "," + side_str + "," + badinfo.IDtoMesCode(TopBuffer.Buffer[0, 0]));
                }
                else
                {
                    for (int i = TopBuffer.SizeY - 1; i >= 0; i--)
                    {
                        for (int j = TopBuffer.SizeX - 1; j >= 0; j--)
                        {
                            if (TopBuffer.Buffer[j, i] != 0)
                            {
                                defect_code = badinfo.IDtoMesCode(TopBuffer.Buffer[j, i]);
                                //switch (TopBuffer.Buffer[j, i])
                                //{
                                //    //case 1://align
                                //    //    break;
                                //    case 2://원소재
                                //        defect_code = "B106";
                                //        break;
                                //    case 3://Open
                                //        defect_code = "B102";
                                //        break;
                                //    case 4://Short
                                //        defect_code = "B129";
                                //        break;
                                //    case 5://BondPad
                                //        defect_code = "B282";
                                //        break;
                                //    case 6://Ball
                                //        defect_code = "B283";
                                //        break;
                                //    case 7://PSR핀홀
                                //        defect_code = "B227";
                                //        break;
                                //    case 8://PSR이물질
                                //        defect_code = "B227";
                                //        break;
                                //    case 9://Crack
                                //        defect_code = "B284";
                                //        break;
                                //    case 10://Burr
                                //        defect_code = "B284";
                                //        break;
                                //    case 11://Venthole
                                //        defect_code = "B102";
                                //        break;
                                //    case 12://Viahole
                                //        defect_code = "B227";
                                //        break;
                                //    case 13://Dam-psr
                                //        defect_code = "B227";
                                //        break;
                                //    default://align 
                                //        defect_code = "B280";
                                //        break;
                                //}
                                defect_cnt++;
                                stripstr.Add(lotNum + "," + strID + "," + (j + 1).ToString() + "," + (i + 1).ToString() + "," + side_str + "," + defect_code);
                            }
                        }
                    }
                }
                #endregion

                #region Bot Side
                side_str = "B";

                if (BotBuffer.OuterNG)//외곽불량 처리 0,0
                {
                    defect_cnt++;
                    //stripstr.Add(lotNum + "," + strID + "," + "0" + "," + "0" + "," + side_str + "," + "B280");
                    //stripstr.Add(lotNum + "," + strID + "," + "0" + "," + "0" + "," + side_str + "," + badinfo.IDtoMesCode(2));
                    stripstr.Add(lotNum + "," + strID + "," + "0" + "," + "0" + "," + side_str + "," + badinfo.IDtoMesCode(BotBuffer.Buffer[0, 0]));
                }
                else
                {
                    for (int i = BotBuffer.SizeX - 1; i >= 0; i--)
                    {
                        for (int j = BotBuffer.SizeY - 1; j >= 0; j--)
                        {
                            if (BotBuffer.Buffer[i, j] != 0)
                            {
                                defect_code = badinfo.IDtoMesCode(BotBuffer.Buffer[i, j]);
                                //switch (BotBuffer.Buffer[i, j])
                                //{
                                //    //case 1://align
                                //    //    break;
                                //    case 2://원소재
                                //        defect_code = "B106";
                                //        break;
                                //    case 3://Open
                                //        defect_code = "B102";
                                //        break;
                                //    case 4://Short
                                //        defect_code = "B129";
                                //        break;
                                //    case 5://BondPad
                                //        defect_code = "B282";
                                //        break;
                                //    case 6://Ball
                                //        defect_code = "B283";
                                //        break;
                                //    case 7://PSR핀홀
                                //        defect_code = "B227";
                                //        break;
                                //    case 8://PSR이물질
                                //        defect_code = "B227";
                                //        break;
                                //    case 9://Crack
                                //        defect_code = "B284";
                                //        break;
                                //    case 10://Burr
                                //        defect_code = "B284";
                                //        break;
                                //    case 11://Venthole
                                //        defect_code = "B102";
                                //        break;
                                //    case 12://Viahole
                                //        defect_code = "B227";
                                //        break;
                                //    case 13://Dam-psr
                                //        defect_code = "B227";
                                //        break;
                                //    default://align 
                                //        defect_code = "B280";
                                //        break;
                                //}
                                defect_cnt++;
                                stripstr.Add(lotNum + "," + strID + "," + (i + 1).ToString() + "," + (j + 1).ToString() + "," + side_str + "," + defect_code);
                            }
                        }
                    }
                }
                #endregion

                lines.Add(defect_cnt.ToString() + "," + lotNum + "," + strID);
                lines.AddRange(stripstr);
                lines.Add(lotNum + "," + strID + ",EOS");

                using (StreamWriter stream = new StreamWriter(strFile))//text 쓰기
                {
                    foreach (string line in lines)
                    {
                        stream.WriteLine(line);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        //검사후 불량 위치와 불량코드 data를 만들어서 gathering pc에 저장한다.
        public bool WriteOutputFile_AVI(string path1, string path2, string lotNum, string worker, int x_out, DateTime EndTime)
        {
            try
            {
                string strResultFolderPath = string.Format(@"{0}/{1}/", path1, lotNum);//temp파일
                string[] files = Directory.GetFiles(strResultFolderPath, "*.txt");

                if (files.Length < 1)//temp파일 없음
                    return false;

                List<string> file_lines = new List<string>();

                for (int i = 0; i < files.Length; i++)
                {
                    string name = Path.GetFileNameWithoutExtension(files[i]);
                    if (name.Length < 3)
                        continue;

                    string[] lines = File.ReadAllLines(files[i]);
                    var str_array = lines[0].Split(',');

                    file_lines.AddRange(lines);
                }
                file_lines.Add(lotNum + ",EOL");//end_of_lot

                string txtFileName = Management_Code + "_" + lotNum + "_" + worker + "_" /*+ x_out.ToString() + "_"*/ + EndTime.ToString("yyyyMMddHHmmss") + ".txt";

                string strFolderPath = string.Format(@"{0}/{1}/", path2, lotNum);

                if (!Directory.Exists(strFolderPath))
                {
                    Directory.CreateDirectory(strFolderPath);
                }
                else
                {
                    DirectoryInfo directory = new DirectoryInfo(strFolderPath);
                    directory.EnumerateFiles().ToList().ForEach(f => f.Delete());
                    directory.EnumerateDirectories().ToList().ForEach(d => d.Delete(true));
                    Directory.Delete(strFolderPath);
                    Directory.CreateDirectory(strFolderPath);
                }
                string output_file_path = string.Format(@"{0}/{1}", strFolderPath, txtFileName);
                if (File.Exists(output_file_path))
                    File.Delete(output_file_path);

                using (StreamWriter stream = new StreamWriter(output_file_path))//text 쓰기
                {
                    foreach (string line in file_lines)
                    {
                        stream.WriteLine(line);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        //불량 정보 string 만들기
        public void WriteBadUnits_AVI(string bincode, int x, string lot, string id, string side, ref int x_out, ref List<string> line)
        {
            int iSplit = 2;
            int indexStart = bincode.Length - iSplit;
            int forCount = bincode.Length / iSplit;//글자 수
            int x_index = 1, y_index = 1, defects = 0;
            string txt = "";
            List<string> temp_lines = new List<string>();

            for (int i = forCount - 1; i >= 0; i--)
            {
                if (indexStart < 0)
                    break;

                string str = bincode.Substring(indexStart, iSplit);

                if (x_index >= x)
                {
                    x_index = 0;
                    y_index++;
                }

                if (str != "FF")
                {
                    txt = lot + "," + id + "," + y_index.ToString() + "," + x_index.ToString() + "," + side/*+","+불량코드*/;
                    temp_lines.Add(txt);
                    defects++;
                }

                x_index++;
                indexStart -= iSplit;
            }

            x_out += defects;
            //불량 갯수 + lot number+strip id
            txt = defects.ToString() + "," + lot + "," + id;
            line.Add(txt);
            line.AddRange(temp_lines);

            //end of strip
            txt = lot + "," + id + ",EOS";
            line.Add(txt);
        }
        #endregion

        #region Server DB - Not Used
        //DB 연결
        public bool ConnectServerDB(string ip, string port)
        {
            string strCon = string.Format("server={0}; user id={1}; password={2}; database=igsdb_bga; port={3}; pooling=false", ip, "igsClient", "mysql", port);
            int connectionType = 0;

            ITSConnection = new DBConnector();

            if (strCon.IndexOf("server") > -1)
            {
                connectionType = 1;
            }
            else if (strCon.IndexOf("DSN") > -1)
            {
                connectionType = 2;
            }

            try
            {
                switch (connectionType)
                {
                    case 1:
                        ITSConnection = new ADOConnector();
                        break;
                    case 2:
                        ITSConnection = new ODBCConnector();
                        break;
                    default:
                        ITSConnection = new NullConnector();
                        break;
                }
                ITSConnection.Connector(strCon);
                if (ITSConnection.SqlConnection == null)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public bool InsertITSDB(string LotNo, string MCcode, DateTime StartTime, DateTime EndTime, int CustomCode)
        {
            try
            {
                if (ITSConnection != null)
                {
                    string strQuery = "INSERT INTO its_lot_info(Lot_No, MC_Code, Start_Time, End_Time, Custom_Code) ";
                    strQuery += string.Format("VALUES('{0}','{1}','{2}','{3}','{4}') ON DUPLICATE KEY UPDATE " +
                        "Lot_No='{0}', MC_Code='{1}', Start_Time='{2}', End_Time='{3}', Custom_Code='{4}'"
                        , LotNo, MCcode, StartTime.ToString("yyyy-MM-dd HH:mm:ss"), EndTime.ToString("yyyy-MM-dd HH:mm:ss"), CustomCode);

                    int queryResult = 1;
                    queryResult = ITSConnection.Execute(strQuery);

                    if (queryResult > 0)
                    {
                        ITSConnection.Commit();
                        return true;
                    }
                    else
                    {
                        ITSConnection.Rollback();
                        return false;
                    }
                }
            }
            catch
            {
                if (ITSConnection != null)
                    ITSConnection.Rollback();
                Debug.WriteLine("Exception occured in Insert ITS DB");
                return false;
            }
            return false;
        }

        public bool DeleteITSDB(string LotNo)
        {
            try
            {
                if (ITSConnection != null)
                {
                    string strQuery = String.Format(" DELETE FROM its_lot_info WHERE Lot_No = '{0}' ", LotNo);
                    int queryResult = 1;
                    queryResult = ITSConnection.Execute(strQuery);
                    if (queryResult >= 0)
                    {
                        ITSConnection.Commit();
                        return true;
                    }
                    else
                    {
                        ITSConnection.Rollback();
                        return false;
                    }
                }
            }
            catch
            {
                if (ITSConnection != null)
                    ITSConnection.Rollback();
                Debug.WriteLine("Exception occured in Insert ITS DB");
                return false;
            }
            return true;
        }

        public bool CloseITSDB()
        {
            if (ITSConnection != null)
                ITSConnection.Close();

            return true;
        }
        #endregion
    }
}
