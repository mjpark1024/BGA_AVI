using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using System.Windows.Media.Imaging;


namespace DNN
{
    public static class Defect_Info
    {

        /// <summary>
        /// File Parsing to Feature Information Create
        /// </summary>
        /// <param name="str">File path</param>
        /// <returns>Feature Information</returns>
        public static DefectInfo GetDefectInfo(string str)
        {
            DefectInfo di = new DefectInfo();
            try
            {
                string name = str.Replace(".png", "");
                string[] parse = name.Split(' ');

                if (parse.Length >= 13)
                {
                    di.CamID = Convert.ToInt32(parse[0].Substring(3, 2));
                    di.StripID = Convert.ToInt32(parse[1].Substring(3, 4));
                    di.UnitX = Convert.ToInt32(parse[2].Remove(0, 2));
                    di.UnitY = Convert.ToInt32(parse[3].Remove(0, 2));
                    string[] pos = parse[5].Split('P', '=', ',');
                    di.Pos = new System.Windows.Point(Convert.ToDouble(pos[2]), Convert.ToDouble(pos[3]));
                    di.Size = Convert.ToDouble(parse[6].Remove(0, 2));
                    int npos = 8;
                    if (parse[8] == "CA" || parse[8] == "BA")
                    {
                        di.BadName = parse[7];
                        npos = 8;
                    }
                    else
                    {
                        di.BadName = parse[7] + " " + parse[8];
                        npos = 9;
                    }
                    di.Side = parse[npos];
                    di.Surface = "";
                    di.Result = -1;
                    di.VisionID = Convert.ToInt32(parse[npos + 1]);
                    if (parse[npos + 2] == "R") di.Cahnnel = 0;
                    else if (parse[npos + 2] == "G") di.Cahnnel = 1;
                    else if (parse[npos + 2] == "B") di.Cahnnel = 2;
                    string[] p = parse[npos + 3].Split('_');

                    if (p.Length >= 4)
                    {
                        di.Inspect = p[0];
                        int n = Convert.ToInt32(p[1]);
                        if (n == 0)
                        {
                            di.ThresType = "Dark";
                            di.ThresHold = Convert.ToInt32(p[2]);
                        }
                        else if (n == 1)
                        {
                            di.ThresType = "Bright";
                            di.ThresHold = Convert.ToInt32(p[3]);
                        }
                        else
                        {
                            di.ThresType = "Other";
                            di.ThresHold = Convert.ToInt32(p[3]);
                        }
                        di.Index = Convert.ToInt32(parse[npos + 4]);
                    }
                    if (p.Length < 4)
                    {
                        p = parse[npos + 4].Split('_');
                        if (p.Length >= 4)
                        {
                            di.Inspect = parse[npos + 3] + p[0];
                            int n = Convert.ToInt32(p[1]);
                            if (n == 0)
                            {
                                di.ThresType = "Dark";
                                di.ThresHold = Convert.ToInt32(p[2]);
                            }
                            else if (n == 1)
                            {
                                di.ThresType = "Bright";
                                di.ThresHold = Convert.ToInt32(p[3]);
                            }
                            else
                            {
                                di.ThresType = "Other";
                                di.ThresHold = Convert.ToInt32(p[3]);
                            }
                            di.Index = Convert.ToInt32(parse[npos + 5]);
                        }
                        if (p.Length < 4)
                        {
                            p = parse[npos + 5].Split('_');
                            if (p.Length >= 4)
                            {
                                di.Inspect = parse[npos + 3] + parse[npos + 4] + p[0];
                                int n = Convert.ToInt32(p[1]);
                                if (n == 0)
                                {
                                    di.ThresType = "Dark";
                                    di.ThresHold = Convert.ToInt32(p[2]);
                                }
                                else if (n == 1)
                                {
                                    di.ThresType = "Bright";
                                    di.ThresHold = Convert.ToInt32(p[3]);
                                }
                                else
                                {
                                    di.ThresType = "Other";
                                    di.ThresHold = Convert.ToInt32(p[3]);
                                }
                                di.Index = Convert.ToInt32(parse[npos + 6]);
                            }
                        }
                    }
                    ////////불량명으로 먼저 필터링 하고
                    if (di.ThresType == "Dark")
                    {

                        /////표면 검사 일 경우 Pad결함
                        ///원소재 검사 인경우 원소재
                        ///
                        if (di.Inspect == "원소재검사")
                        {
                            di.Surface = Bad_Class.GetClass(7);
                        }
                        else
                        {
                            if (di.ThresHold > 125)
                            {
                                if (di.Inspect == "표면검사")
                                {
                                    if (di.Side == "CA")
                                        di.Surface = Bad_Class.GetClass(1);
                                    else
                                        di.Surface = Bad_Class.GetClass(0);
                                }
                                else
                                    di.Surface = Bad_Class.GetClass(0);
                            }
                            else
                            {
                                if (di.Inspect == "표면검사")
                                    di.Surface = Bad_Class.GetClass(3);
                                else
                                    di.Surface = Bad_Class.GetClass(6);
                            }
                            if (di.Size > 10000) di.Surface = Bad_Class.GetClass(8);
                        }
                    }
                    else if (di.ThresType == "Bright")
                    {
                        if (di.Inspect == "원소재검사")
                        {
                            di.Surface = Bad_Class.GetClass(7);
                        }
                        else if (di.Inspect == "인식키검사")
                        {
                            if (di.BadName == "인식키")
                                di.Surface = Bad_Class.GetClass(12);
                            else if (di.BadName == "PSR Shift")
                                di.Surface = Bad_Class.GetClass(11);
                        }
                        else if (di.Inspect == "Via검사")
                        {
                            di.Surface = Bad_Class.GetClass(9);
                        }
                        else
                        {
                            if (di.ThresHold > 125)
                            {
                                if (di.Inspect == "표면검사")
                                    di.Surface = Bad_Class.GetClass(4);
                                else
                                    di.Surface = Bad_Class.GetClass(5);
                            }
                            else
                            {
                                if (di.Inspect == "표면검사")
                                    di.Surface = Bad_Class.GetClass(3);
                                else
                                    di.Surface = Bad_Class.GetClass(5);
                            }
                            if (di.Size > 10000) di.Surface = Bad_Class.GetClass(8);
                        }
                    }
                    else
                    {
                        if (di.Inspect == "리드형상검사")
                        {
                            if (di.BadName.Contains("Align"))
                            {
                                di.Surface = Bad_Class.GetClass(13);
                            }
                            else di.Surface = Bad_Class.GetClass(1);
                        }
                        else if (di.Inspect == "공간형상검사")
                        {
                            if (di.BadName.Contains("Align"))
                            {
                                di.Surface = Bad_Class.GetClass(13);
                            }
                            else di.Surface = Bad_Class.GetClass(2);
                        }
                        else if (di.Inspect == "외곽검사")
                        {
                            di.Surface = Bad_Class.GetClass(10);
                        }
                        else if (di.Inspect == "Via검사")
                        {
                            di.Surface = Bad_Class.GetClass(9);
                        }
                        else if (di.Inspect == "인식키검사")
                        {
                            di.Surface = Bad_Class.GetClass(12);
                        }
                        else if (di.BadName.Contains("Align"))
                        {
                            di.Surface = Bad_Class.GetClass(13);
                        }

                    }
                }
            }
            catch
            {
            }
            return di;
        }
        public static int DI_STRING_NUM = 13;

        public static string[] Valid_DefectInfo(string[] str)
        {
            if (str.Length < 10)
            {
                string[] ret = new string[12];
                Array.Copy(str, ret, str.Length);
                ret[8] = "BA";
                ret[9] = "0";
                ret[10] = "R";
                ret[11] = "표면검사_0_100_255";
                return ret;
            }
            else
            {
                string[] ret = new string[str.Length];
                Array.Copy(str, ret, str.Length);
                int index = 9;
                int gap = 0;
                for (int i = 9; i < 12; i++)
                {
                    try
                    {
                        Convert.ToInt32(ret[i]);
                        index = i;
                        gap = i - 9;
                        break;

                    }
                    catch { }
                }
                try
                {
                    string bad = "";
                    for (int i = 7; i < index - 1; i++)
                    {
                        bad += ret[i];
                    }
                    ret[7] = bad;
                    int n = 0;
                    for (int i = index - 1; i < ret.Length; i++)
                    {
                        ret[8 + n] = ret[i];
                        n++;
                    }
                }
                catch
                {
                    int kk = 0;
                }
                Array.Resize(ref ret, ret.Length - gap);

                try
                {
                    string tmp = "";
                    for (int i = DI_STRING_NUM - 1; i < ret.Length; i++)
                    {
                        tmp += ret[i];
                    }
                    ret[DI_STRING_NUM - 1] = tmp;
                }
                catch
                {
                    return ret;
                }
                Array.Resize(ref ret, DI_STRING_NUM);
                return ret;
            }
        }

        /// <summary>
        /// File to Feature Information Create
        /// </summary>
        /// <param name="str">File path</param>
        /// <param name="side">BGA Product Side</param>
        /// <param name="result">Inspect Result</param>
        /// <returns>Feature Information</returns>
        public static DefectInfo GetDefectInfo(string str, string side, int result, int no)
        {
            DefectInfo di = new DefectInfo();
            try
            {
                //string name = str.Replace(".png", "");
                string[] parse = str.Split(' ');
                parse = Valid_DefectInfo(parse);
                if (parse.Length >= DI_STRING_NUM)
                {
                    di.CamID = Convert.ToInt32(parse[0].Substring(3, 2));
                    di.StripID = Convert.ToInt32(parse[1].Substring(3, 4));
                    di.UnitX = Convert.ToInt32(parse[2].Remove(0, 2));
                    di.UnitY = Convert.ToInt32(parse[3].Remove(0, 2));
                    string[] pos = parse[5].Split('P', '=', ',');
                    di.Pos = new System.Windows.Point(Convert.ToDouble(pos[2]), Convert.ToDouble(pos[3]));
                    di.Size = Convert.ToDouble(parse[6].Remove(0, 2));
                    di.BadName = parse[7];
                    di.Side = parse[8];
                    di.Surface = side;
                    di.Result = result;
                    di.VisionID = Convert.ToInt32(parse[9]);
                    if (parse[10] == "R") di.Cahnnel = 0;
                    else if (parse[10] == "G") di.Cahnnel = 1;
                    else if (parse[10] == "B") di.Cahnnel = 2;
                    string[] p = parse[11].Split('_');

                    if (p.Length >= 4)
                    {
                        di.Inspect = p[0];
                        int n = Convert.ToInt32(p[1]);
                        if (n == 0)
                        {
                            di.ThresType = "Dark";
                            di.ThresHold = Convert.ToInt32(p[2]);
                        }
                        else if (n == 1)
                        {
                            di.ThresType = "Bright";
                            di.ThresHold = Convert.ToInt32(p[3]);
                        }
                        else
                        {
                            if (di.Inspect.Contains("공간"))
                            {
                                di.ThresType = "Dark";
                                di.ThresHold = Convert.ToInt32(p[2]);
                            }
                            else if (di.Inspect.Contains("리드"))
                            {
                                di.ThresType = "Bright";
                                di.ThresHold = Convert.ToInt32(p[3]);
                            }
                            else
                            {

                                di.ThresType = "Dark";
                                di.ThresHold = 125;
                            }
                        }
                        di.Index = no;// Convert.ToInt32(parse[12]);
                    }
                    if (p.Length < 4)
                    {
                        p = parse[12].Split('_');
                        if (p.Length >= 4)
                        {
                            di.Inspect = parse[11] + p[0];
                            int n = Convert.ToInt32(p[1]);
                            if (n == 0)
                            {
                                di.ThresType = "Dark";
                                di.ThresHold = Convert.ToInt32(p[2]);
                            }
                            else if (n == 1)
                            {
                                di.ThresType = "Bright";
                                di.ThresHold = Convert.ToInt32(p[3]);
                            }
                            else
                            {
                                if (di.Inspect.Contains("공간"))
                                {
                                    di.ThresType = "Dark";
                                    di.ThresHold = Convert.ToInt32(p[2]);
                                }
                                else if (di.Inspect.Contains("리드"))
                                {
                                    di.ThresType = "Bright";
                                    di.ThresHold = Convert.ToInt32(p[3]);
                                }
                                else
                                {

                                    di.ThresType = "Bright";
                                    di.ThresHold = 125;
                                }
                            }
                            di.Index = no;// Convert.ToInt32(parse[13]);
                        }
                        else
                        {
                            p = parse[13].Split('_');
                            if (p.Length >= 4)
                            {
                                di.Inspect = parse[11] + parse[12] + p[0];
                                int n = Convert.ToInt32(p[1]);
                                if (n == 0)
                                {
                                    di.ThresType = "Dark";
                                    di.ThresHold = Convert.ToInt32(p[2]);
                                }
                                else if (n == 1)
                                {
                                    di.ThresType = "Bright";
                                    di.ThresHold = Convert.ToInt32(p[3]);
                                }
                                else
                                {
                                    if (di.Inspect.Contains("공간"))
                                    {
                                        di.ThresType = "Dark";
                                        di.ThresHold = Convert.ToInt32(p[2]);
                                    }
                                    else if (di.Inspect.Contains("리드"))
                                    {
                                        di.ThresType = "Bright";
                                        di.ThresHold = Convert.ToInt32(p[3]);
                                    }
                                    else
                                    {

                                        di.ThresType = "Bright";
                                        di.ThresHold = 125;
                                    }
                                }
                                di.Index = no;// Convert.ToInt32(parse[14]);
                            }
                        }
                    }

                }
                else if (parse.Length >= 9)
                {
                    string cam = parse[0].Substring(0, 2);
                    if (cam == "CA")
                    {
                        cam = parse[0].Substring(0, 4);
                        if (cam == "CAM1") di.CamID = 11;
                        if (cam == "CAM2") di.CamID = 21;
                        if (cam == "CAM3") di.CamID = 31;
                        di.StripID = 0;
                        di.UnitX = 1;
                        di.UnitY = 1;
                        di.Pos = new System.Windows.Point(0, 0);
                        di.Size = 10;
                        di.BadName = "이물질";
                        di.Side = "CA";
                        di.Surface = side;
                        di.Result = result;
                        di.VisionID = 11;
                        di.Cahnnel = 0;
                        di.ThresType = "Dark";
                        di.ThresHold = 125;
                        di.Index = 1;
                        di.Inspect = "표면검사";
                    }
                    else
                    {
                        if (cam == "상부") di.CamID = 11;
                        if (cam == "하부") di.CamID = 21;
                        if (cam == "투과") di.CamID = 31;
                        di.StripID = Convert.ToInt32(parse[0].Substring(3, 5));
                        di.UnitX = 1;
                        di.UnitY = 1;
                        di.Pos = new System.Windows.Point(0, 0);
                        di.Size = Convert.ToDouble(parse[4].Remove(0, 5));
                        di.BadName = "이물질";
                        di.Side = "TS";
                        di.Surface = side;
                        di.Result = result;
                        di.VisionID = 11;
                        di.Cahnnel = 0;
                        di.ThresType = "Dark";
                        di.ThresHold = 125;
                        di.Index = 1;
                        di.Inspect = "표면검사";
                    }
                }
            }
            catch
            {
            }
            return di;
        }
    }
}
