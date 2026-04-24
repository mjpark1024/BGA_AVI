using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using OpenCvSharp;
using System.IO;
using System.Collections.ObjectModel;
using Common;

namespace DNN
{
    /// <summary>
    /// Project File Control Class
    /// </summary>
    public class Project
    {
        public string Name;  //Project Name
        public string Path;  //Project Path
        public ObservableCollection<Net> Nets = new ObservableCollection<Net>(); //Projrct Dnn Networks List

        /// <summary>
        /// Load Project Form File
        /// </summary>
        /// <param name="project_file">Project File</param>
        /// <returns>Succes true Fial false</returns>
        public bool Load_Project(string project_file)
        {
            if (!File.Exists(project_file)) return false;
            IniFile ini = new IniFile(project_file);
            Nets.Clear();
            Name = ini.ReadString("PROJECT", "NAME", "");
            Path = ini.ReadString("PROJECT", "JOB_DIR", "");
            for (int i = 0; i < 256; i++)
            {
                string Net_Name = ini.ReadString(string.Format("NET_{0}", i + 1), "NAME", "");
                if (Net_Name == "") break;
                Net net = new Net();
                net.Name = Net_Name;
                net.Config = ini.ReadString(string.Format("NET_{0}", i + 1), "CONFIG_FILE", "");
                string[] BadNames = ini.ReadString(string.Format("NET_{0}", i + 1), "BAD_NAME", "").Split(';');
                net.Classes.Clear();
                for (int j = 0; j < 256; j++)
                {
                    string Class_Name = ini.ReadString(string.Format("NET_{0}", i + 1), string.Format("CLASS_NAME_{0}", j + 1), "");
                    if (Class_Name == "") break;
                    Class_Info classes = new Class_Info();
                    classes.Name = Class_Name;
                    if (BadNames.Length <= 1)
                    {
                        classes.BadName = "None";
                    }
                    else
                    {
                        try
                        {
                            classes.BadName = BadNames[j];
                        }
                        catch
                        {
                            classes.BadName = "None";
                        }
                    }
                    net.Classes.Add(classes);
                }
                Nets.Add(net);
            }
            return true;
        }

        /// <summary>
        /// Create Project File
        /// </summary>
        /// <param name="path">Project File PAth</param>
        /// <param name="name">Project Name</param>
        /// <returns>Succes true Fail false</returns>
        public bool Create_Project(string path, string name)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string project_file = path + "\\" + name + ".djt";
            IniFile ini = new IniFile(project_file);
            ini.WriteString("PROJECT", "NAME", name);
            ini.WriteString("PROJECT", "JOB_DIR", path);
            return true;
        }
    }

    /// <summary>
    /// Dnn Networks Structure
    /// </summary>
    public class Net : NotifyPropertyChanged
    {
        private string m_Name;
        public string Config;
        public string Name   // Network Name
        {
            get { return m_Name; }
            set { m_Name = value; Notify("Name"); }
        }
        public ObservableCollection<Class_Info> Classes = new ObservableCollection<Class_Info>(); // Classes
    }

    /// <summary>
    /// Classes Sutruture
    /// </summary>
    public class Class_Info : NotifyPropertyChanged
    {
        private string m_Name;
        private string m_Bad;
        public string Config;

        public string Name        // Class Name
        {
            get { return m_Name; }
            set { m_Name = value; Notify("Name"); }
        }
        public string BadName        // Class Name
        {
            get { return m_Bad; }
            set { m_Bad = value; Notify("BadName"); }
        }
    }

    /// <summary>
    /// Class vs Dnn Result Class
    /// </summary>
    public class Class_Result : NotifyPropertyChanged
    {
        private string m_name;
        private string m_result;

        public string Name
        {
            get { return m_name; }
            set { m_name = value; Notify("Name"); }
        }
        public string Result
        {
            get { return m_result; }
            set { m_result = value; Notify("Result"); }
        }

        public Class_Result()
        {
            Name = "";
            Result = "";
        }
        public Class_Result(string name, string result)
        {
            Name = name;
            Result = result;
        }
    }

    /// <summary>
    /// BGA 불량 Structure
    /// </summary>
    public static class Bad_Class
    {
        public static string[] m_Class = new string[]
        {
            "Ball핀홀",    //0
            "Pad리드",     //1
            "Pad공간",     //2
            "Pad이물",     //3
            "미도금",      //4
            "PSR핀홀",     //5
            "PSR이물",     //6
            "Rail마킹",    //7
            "Unit마킹",    //8
            "Via미충진",   //9
            "유닛외곽",    //10
            "PSRShift",    //11
            "인식키",      //12
            "Align",       //13
            "기타"         //14   
        };
        public static int Length { get { return m_Class.Length; } }
        public static string GetClass(int anClass)
        {
            try
            {
                if (anClass < m_Class.Length)
                    return m_Class[anClass];
                else
                    return "";
            }
            catch
            {
                return "";
            }
        }

        public static int GetIndex(string astrClass)
        {
            try
            {
                int nLength = m_Class.Length;
                for (int nIndex = 0; nIndex < nLength; nIndex++)
                {
                    if (astrClass == m_Class[nIndex])
                    {
                        return nIndex;
                    }
                }
                return -1;
            }
            catch
            {
                return -1;
            }
        }
    }

    public class Fix_Thresh : NotifyPropertyChanged
    {
        private string m_BadName;
        private double m_Thresh;

        public string BadName
        {
            get { return m_BadName; }
            set { m_BadName = value; Notify("BadName"); }
        }

        public double Threshold
        {
            get { return m_Thresh; }
            set { m_Thresh = value; Notify("Threshold"); }
        }

        public Fix_Thresh()
        {
            BadName = "";
            Threshold = 0.9;
        }

        public Fix_Thresh(string badname, double threshold)
        {
            BadName = badname;
            Threshold = threshold;
        }
    }

    /// <summary>
    /// Dnn Predict Parameters
    /// </summary>
    public class PredictInfo : NotifyPropertyChanged
    {
        private string m_class;
        private int m_index;
        private double m_threshold;
        private double m_weight;
        private double m_T_weight;
        public string ClassName
        {
            get { return m_class; }
            set { m_class = value; Notify("ClassName"); }
        }

        public int Index
        {
            get { return m_index; }
            set { m_index = value; Notify("Index"); }
        }

        public double Threshold
        {
            get { return m_threshold; }
            set { m_threshold = value; Notify("Threshold"); }
        }
        public double Bad_Weight
        {
            get { return m_weight; }
            set { m_weight = value; Notify("Weight"); }
        }
        public double Train_Weight
        {
            get { return m_T_weight; }
            set { m_T_weight = value; Notify("Train_Weight"); }
        }
    }

    public class TrainInfo : NotifyPropertyChanged
    {
        private string m_class;
        private int m_count;
        private int m_index;
        private int m_train;
        private int m_valid;
        private int m_test;
        public string ClassName
        {
            get { return m_class; }
            set { m_class = value; Notify("ClassName"); }
        }
        public int Count
        {
            get { return m_count; }
            set { m_count = value; Notify("Count"); }

        }

        public int Index
        {
            get { return m_index; }
            set { m_index = value; Notify("Index"); }

        }

        public int Train
        {
            get { return m_train; }
            set { m_train = value; Notify("Train"); }

        }
        public int Valid
        {
            get { return m_valid; }
            set { m_valid = value; Notify("Valid"); }

        }
        public int Test
        {
            get { return m_test; }
            set { m_test = value; Notify("Test"); }

        }


        public List<ClassInfo> Classes;
        public TrainInfo()
        {
            Classes = new List<ClassInfo>();
        }
    }

    public class ClassInfo
    {
        public string ClassName;
        public int ImageIndex;
        public bool IsGood;
        public int ClassIndex;
        public int TrainIndex;
    }

    public struct YoloResult
    {
        public Mat resultImg;
        public List<uint> classIds;
        public string filePath;
        public bool isDetection;
        public double prob;
        public int ClassID;
        public string ClassName;
        public string BadName;
        public int Layer;
        public int ModelIndex;

        public YoloResult Clone()
        {
            YoloResult result = new YoloResult();
            result.resultImg = resultImg.Clone();
            result.classIds = new List<uint>();
            for (int i = 0; i < classIds.Count; i++)
                result.classIds.Add(this.classIds[i]);
            result.filePath = this.filePath;
            result.isDetection = this.isDetection;
            result.prob = this.prob;
            result.ClassID = this.ClassID;
            result.ClassName = this.ClassName;
            result.Layer = this.Layer;
            result.ModelIndex = this.ModelIndex;
            result.BadName = this.BadName;
            return result;
        }
    }

    public class ClassResult : NotifyPropertyChanged
    {
        private int m_index;
        private string m_name;
        private string m_bad;
        private int m_count;
        private int m_ok;
        private int m_ng;
        private int m_nt;
        private double m_okrate;
        private double m_ngrate;
        private double m_ntrate;

        public string ClassName
        {
            get { return m_name; }
            set { m_name = value; Notify("ClassName"); }
        }
        public string BadName
        {
            get { return m_bad; }
            set { m_bad = value; Notify("BadName"); }
        }
        public int Index
        {
            get { return m_index; }
            set { m_index = value; Notify("Index"); }
        }

        public int Count
        {
            get { return m_count; }
            set { m_count = value; Notify("Count"); }
        }

        public int OK
        {
            get { return m_ok; }
            set
            {
                m_ok = value;
                if (m_ok == 0 || m_count == 0) OK_Rate = 0;
                else OK_Rate = ((double)m_ok / (double)m_count) * 100.0;
                Notify("OK");
            }
        }

        public int NG
        {
            get { return m_ng; }
            set
            {
                m_ng = value;
                if (m_ng == 0 || m_count == 0) NG_Rate = 0;
                else NG_Rate = ((double)m_ng / (double)m_count) * 100.0;
                Notify("NG");
            }
        }

        public int NT
        {
            get { return m_nt; }
            set
            {
                m_nt = value;
                if (m_nt == 0 || m_count == 0) NT_Rate = 0;
                else NT_Rate = ((double)m_nt / (double)m_count) * 100.0;
                Notify("NT");
            }
        }

        public double OK_Rate
        {
            get { return m_okrate; }
            set { m_okrate = value; Notify("OK_Rate"); }
        }

        public double NG_Rate
        {
            get { return m_ngrate; }
            set { m_ngrate = value; Notify("NG_Rate"); }
        }

        public double NT_Rate
        {
            get { return m_ntrate; }
            set { m_ntrate = value; Notify("NT_Rate"); }
        }
    }


    public class DefectInfo
    {
        public int CamID;
        public int StripID;
        public int UnitX;
        public int UnitY;
        public System.Windows.Point Pos;
        public double Size;
        public string BadName;
        public string Side;
        public string Surface;
        public int VisionID;
        public int Cahnnel;
        public string Inspect;
        public string ThresType;
        public int ThresHold;
        public int Index;
        public int Result;

        public int Width;
        public int Height;
        public int Blobs;
        public int Sum;
        public int Pixels;
        public float Angle;
        public int[] Average = new int[3];
        public int AvgI;
        public int AvgH;
        public int AvgS;
        public System.Windows.Point Center;
    }

    public class BlobInfo
    {
        public int Width;
        public int Height;
        public int Blobs;
        public int Sum;
        public int Pixels;
        public float Angle;
        public int[] Average = new int[3];
        public int AvgI;
        public int AvgH;
        public int AvgS;
        public System.Windows.Point Center;
    }


    public class Net_Cfg
    {
        private string path;
        public List<Net_Layer> Net;
        private bool bLoaded = false;
        public Net_Cfg(string cfg_filename)
        {
            path = cfg_filename;
            Net = new List<Net_Layer>();
            Load_Cfg();
        }

        private void AddLayer(List<string> lstLayer)
        {
            Net_Layer layer = new Net_Layer();
            foreach (string s in lstLayer)
            {
                if (s.Trim() == "") continue;
                if (s.Contains("[") && s.Contains("["))
                {
                    layer.Name = s.Trim(new char[] { '[', ']' });
                }
                else if (s.Contains("#"))
                {
                    layer.Header.Add(s);
                    layer.Value.Add("");
                }
                else
                {
                    string[] p = s.Split('=');
                    if (p.Length >= 2)
                    {
                        layer.Header.Add(p[0]);
                        layer.Value.Add(p[1]);
                    }
                }
            }
            Net.Add(layer);
        }

        public int Get_Batch()
        {
            if (!bLoaded) return 32;
            int nBatch = 32;
            #region Calc Config Values
            try
            {
                for (int i = 0; i < Net.Count; i++)
                {
                    if (Net[i].Name.Contains("net") || Net[i].Name.Contains("NET"))
                    {
                        for (int j = 0; j < Net[i].Header.Count; j++)
                        {
                            if (Net[i].Header[j].Contains("#"))
                            {
                                continue;
                            }
                            if (Net[i].Header[j].Trim().Equals("batch"))
                            {
                                nBatch = Convert.ToInt32(Net[i].Value[j]);
                            }
                        }
                    }
                }
            }
            catch { }
            #endregion
            return nBatch;
        }

        public int Get_Subdivisions()
        {
            if (!bLoaded) return 32;
            int nSubdivisions = 32;
            #region Calc Config Values
            try
            {
                for (int i = 0; i < Net.Count; i++)
                {
                    if (Net[i].Name.Contains("net") || Net[i].Name.Contains("NET"))
                    {
                        for (int j = 0; j < Net[i].Header.Count; j++)
                        {
                            if (Net[i].Header[j].Contains("#"))
                            {
                                continue;
                            }
                            if (Net[i].Header[j].Trim().Equals("subdivisions"))
                            {
                                nSubdivisions = Convert.ToInt32(Net[i].Value[j]);
                            }
                        }
                    }
                }
            }
            catch { }
            #endregion
            return nSubdivisions;
        }

        public bool Save_Cfg(string filename, int nClasses, int nBatch = 0, int nSub = 1, int nClass_iter = 2000, int nAngle = 0, bool bYolo = true)
        {
            if (!bLoaded) return false;
            #region Calc Config Values
            int max_batches = nClasses * nClass_iter + 200;
            for (int i = 0; i < Net.Count; i++)
            {
                if (Net[i].Name.Contains("net") || Net[i].Name.Contains("NET"))
                {
                    for (int j = 0; j < Net[i].Header.Count; j++)
                    {
                        if (Net[i].Header[j].Contains("#"))
                        {
                            continue;
                        }
                        if (nBatch > 0)
                        {
                            if (Net[i].Header[j].Trim().Equals("batch"))
                            {
                                Net[i].Value[j] = nBatch.ToString();
                            }
                        }

                        if (nAngle >= 0)
                        {
                            if (Net[i].Header[j].Trim().Equals("angle"))
                            {
                                Net[i].Value[j] = nAngle.ToString();
                            }
                        }

                        if (nSub > 0)
                        {
                            if (Net[i].Header[j].Trim().Equals("subdivisions"))
                            {
                                Net[i].Value[j] = nSub.ToString();
                            }
                        }

                        if (Net[i].Header[j].Trim().Equals("max_batches"))
                        {
                            Net[i].Value[j] = max_batches.ToString();
                        }
                        if (Net[i].Header[j].Trim().Equals("steps"))
                        {
                            Net[i].Value[j] = string.Format("{0},{1}", (int)((double)max_batches * 0.8), (int)((double)max_batches * 0.9));
                        }
                    }
                }
                if (bYolo)
                {
                    if (Net[i].Name.Contains("yolo"))
                    {
                        int nMask = 0;
                        for (int j = 0; j < Net[i].Header.Count; j++)
                        {
                            if (Net[i].Header[j].Contains("#"))
                            {
                                continue;
                            }
                            if (Net[i].Header[j].Trim().Equals("mask"))
                            {
                                string[] tmp = Net[i].Value[j].Split(',');

                                for (int k = 0; k < tmp.Length; k++)
                                {
                                    try
                                    {
                                        int n = Convert.ToInt32(tmp[k]);
                                        nMask++;
                                    }
                                    catch { }
                                }
                            }
                            if (Net[i].Header[j].Trim().Equals("classes"))
                            {
                                Net[i].Value[j] = nClasses.ToString();
                            }
                        }

                        if (i - 1 >= 0 && nMask > 0)
                        {
                            for (int j = 0; j < Net[i - 1].Header.Count; j++)
                            {
                                if (Net[i - 1].Header[j].Trim().Equals("filters"))
                                {
                                    Net[i - 1].Value[j] = ((nClasses + 5) * nMask).ToString();
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Save Config File

            List<string> lines = new List<string>();
            for (int i = 0; i < Net.Count; i++)
            {
                lines.Add(string.Format("[{0}]", Net[i].Name));
                for (int j = 0; j < Net[i].Header.Count; j++)
                {
                    if (Net[i].Header[j].Contains("#"))
                    {
                        lines.Add(string.Format("{0}", Net[i].Header[j]));
                    }
                    else lines.Add(string.Format("{0}={1}", Net[i].Header[j], Net[i].Value[j]));
                }
                lines.Add("");
            }
            if (File.Exists(filename)) File.Delete(filename);
            File.AppendAllLines(filename, lines);

            #endregion

            return true;
        }

        private bool Load_Cfg()
        {
            if (!File.Exists(path)) return false;
            try
            {
                string[] lines = File.ReadAllLines(path);
                List<string> tmpLayer = new List<string>();
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Trim() == "") continue;
                    if (lines[i].Contains("[") && lines[i].Contains("["))
                    {
                        if (tmpLayer.Count > 1)
                        {
                            AddLayer(tmpLayer);
                        }
                        tmpLayer.Clear();
                    }
                    tmpLayer.Add(lines[i]);
                }
                if (tmpLayer.Count > 1)
                {
                    AddLayer(tmpLayer);
                }
            }
            catch { return false; }
            bLoaded = true;
            return true;
        }
    }

    public class Node
    {
        public string HashID;
        public string Name;
        public List<string> model;
        public string parent_net;
        public string parent_class;
        public string[] cfg_file;
        public string[] weight;
        public string names;
        public double[][] thresholds;
        public double[][] Bad_Weights;
        public double[][] Train_Weights;
        public string[] classes;
        public string[] nodes;
        public string[] badname;
        public int[] NonClass;
        public int Layer;
        public bool bUse;
        public int CamID;
    }

    public class Net_Layer
    {
        public string Name;
        public List<string> Header;
        public List<string> Value;
        public Net_Layer()
        {
            Header = new List<string>();
            Value = new List<string>();
        }
    }

    /// <summary>   Notify property changed.  </summary>
    /// <remarks>   suoow2, 2014-10-25. </remarks>
    [Serializable]
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        /// <summary> Event queue for all listeners interested in PropertyChanged events. </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>   Notifies. </summary>
        /// <remarks>   suoow2, 2014-08-09. </remarks>
        /// <param name="strPropertyName">  Name of the property. </param>
        public void Notify(string strPropertyName)
        {
            PropertyChangedEventHandler p = PropertyChanged;
            if (p != null)
            {
                p(this, new PropertyChangedEventArgs(strPropertyName));
            }
        }
    }
}
