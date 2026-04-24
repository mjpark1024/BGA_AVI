using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using Common;

namespace DNN
{
    public class Networks
    {
        public bool IsCreate;
        public bool IsLoaded;
        public string Site = "";
        public string Group = "";
        YoloV4 yolo = new YoloV4();
        public List<Node> models = new List<Node>();
        private List<string> Fix_Bad_Name = new List<string>();
        private List<Fix_Thresh> fix_thres = new List<Fix_Thresh>();
        public Node CurrNode = null;

        public Networks(string filePath)
        {
            IsCreate = false;
            IsLoaded = false;
            models = Load_Yolo_Net(filePath, out Site, out Group);
            if (models == null) IsLoaded = false;
            else IsLoaded = true;
            if (IsLoaded) IsCreate = Create_Yolo_Net();
        }

        public Networks(List<Node> nodes)
        {
            IsCreate = false;
            models = nodes;
            IsLoaded = true;
            if (IsLoaded) IsCreate = Create_Yolo_Net();
        }

        public void Save_Fix_Thres(string path)
        {
            string names = "";
            string thres = "";
            for (int i = 0; i < fix_thres.Count; i++)
            {
                try
                {
                    names += fix_thres[i].BadName + ";";
                    thres += fix_thres[i].Threshold.ToString() + ";";
                }
                catch { }
            }
            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Close();
            }
            IniFile ini = new IniFile(path);
            ini.WriteInteger("FIX", "Count", fix_thres.Count);
            ini.WriteString("FIX", "BAD_NAME", names);
            ini.WriteString("FIX", "THRESHOLD", thres);
        }

        public void Load_Fix_Thres(string path)
        {
            if (!File.Exists(path))
            {
                Set_Fix_Thres(null);
            }
            IniFile ini = new IniFile(path);
            int cnt = ini.ReadInteger("FIX", "Count", 0);
            string names = ini.ReadString("FIX", "BAD_NAME", "");
            string thres = ini.ReadString("FIX", "THRESHOLD", "");
            string[] name = names.Split(';');
            string[] thre = thres.Split(';');
            List<Fix_Thresh> fix = new List<Fix_Thresh>();
            for (int i = 0; i < cnt; i++)
            {
                try
                {
                    Fix_Thresh tmp = new Fix_Thresh(name[i], Convert.ToDouble(thre[i]));
                    fix.Add(tmp);
                }
                catch { }
            }
            Set_Fix_Thres(fix);
        }

        public void Set_Fix_Thres(List<Fix_Thresh> fix)
        {
            List<string> bads = GetClasses();
            if (fix == null)
            {
                for (int i = 0; i < bads.Count; i++)
                {
                    Fix_Thresh tmp = new Fix_Thresh(bads[i], 0.95);
                    fix_thres.Add(tmp);
                }
                return;
            }
            bool bEach = true;
            for (int i = 0; i < bads.Count; i++)
            {
                try
                {
                    Fix_Thresh th = fix_thres.First(x => x.BadName == bads[i]);
                    if (th == null)
                    {
                        bEach = false;
                        break;
                    }
                }
                catch
                {
                    bEach = false;
                    break;
                }
            }
            fix_thres.Clear();
            if (bEach)
            {
                for (int i = 0; i < fix.Count; i++)
                {
                    Fix_Thresh tmp = new Fix_Thresh(fix[i].BadName, fix[i].Threshold);
                    fix_thres.Add(tmp);
                }
            }
            else
            {
                for (int i = 0; i < bads.Count; i++)
                {
                    Fix_Thresh tmp = new Fix_Thresh(bads[i], 0.95);
                    fix_thres.Add(tmp);
                }
            }
        }

        private List<string> GetClasses()
        {
            List<string> classes = new List<string>();
            for (int i = 0; i < models.Count; i++)
            {
                for (int j = 0; j < models[i].badname.Length; j++)
                {
                    if (!classes.Contains(models[i].badname[j]))
                        classes.Add(models[i].badname[j]);
                }
            }
            return classes;
        }

        public static List<Node> Load_Yolo_Net(string path, out string site, out string group)
        {
            site = "";
            group = "";
            if (!File.Exists(path)) return null;
            IniFile ini = new IniFile(path);
            site = ini.ReadString("NET", "SITE", "");
            group = ini.ReadString("NET", "GROUP", "");
            int cnt = ini.ReadInteger("NET", "COUNT", 0);
            List<Node> lstNode = new List<Node>();
            FileInfo fi = new FileInfo(path);
            for (int i = 0; i < cnt; i++)
            {
                Node node = new Node();
                node.Name = ini.ReadString("Node" + (i + 1).ToString(), "Name", "");
                string m = ini.ReadString("Node" + (i + 1).ToString(), "model", "");
                node.model = m.Split(',').ToList();
                node.parent_net = ini.ReadString("Node" + (i + 1).ToString(), "parent_net", "");
                node.parent_class = ini.ReadString("Node" + (i + 1).ToString(), "parent_class", "");

                node.names = ini.ReadString("Node" + (i + 1).ToString(), "names_File", "");

                node.Layer = ini.ReadInteger("Node" + (i + 1).ToString(), "Layer", 0);

                node.CamID = ini.ReadInteger("Node" + (i + 1).ToString(), "CamID", 0);
                node.NonClass = new int[node.model.Count];
                node.cfg_file = new string[node.model.Count];
                node.weight = new string[node.model.Count];



                string[] non = ini.ReadString("Node" + (i + 1).ToString(), "None_Class", "").Split(',');
                string[] wei = ini.ReadString("Node" + (i + 1).ToString(), "weight_File", "").Split(',');
                string[] cfg = ini.ReadString("Node" + (i + 1).ToString(), "cfg_file", "").Split(',');

                for (int k = 0; k < node.model.Count; k++)
                {
                    node.cfg_file[k] = cfg[k];
                    node.weight[k] = wei[k];
                    node.NonClass[k] = Convert.ToInt32(non[k]);
                    node.cfg_file[k] = fi.DirectoryName + "\\" + node.cfg_file[k];
                    node.weight[k] = fi.DirectoryName + "\\" + node.weight[k];
                }
                node.names = fi.DirectoryName + "\\" + node.names;
                node.classes = File.ReadAllLines(node.names);

                m = ini.ReadString("Node" + (i + 1).ToString(), "Thresholds", "");
                node.thresholds = new double[node.model.Count][];
                for (int x = 0; x < node.model.Count; x++)
                    node.thresholds[x] = new double[node.classes.Length];
                string[] tmp = m.Split(',');
                for (int x = 0; x < node.model.Count; x++)
                {
                    for (int y = 0; y < node.classes.Length; y++)
                    {
                        if (tmp.Length < node.model.Count * node.classes.Length)
                        {
                            node.thresholds[x][y] = 0.7;
                        }
                        else
                            node.thresholds[x][y] = Convert.ToDouble(tmp[y + x * node.classes.Length]);
                    }
                }

                string[] bads = ini.ReadString("Node" + (i + 1).ToString(), "Bad_Name", "").Split(';');
                node.badname = new string[node.classes.Length];
                for (int y = 0; y < node.classes.Length; y++)
                {
                    if (bads.Length < node.classes.Length)
                    {
                        node.badname[y] = "None";
                    }
                    else
                        node.badname[y] = bads[y];
                }
                m = ini.ReadString("Node" + (i + 1).ToString(), "Weights", "");
                node.Bad_Weights = new double[node.model.Count][];
                for (int x = 0; x < node.model.Count; x++)
                    node.Bad_Weights[x] = new double[node.classes.Length];

                tmp = m.Split(',');
                for (int x = 0; x < node.model.Count; x++)
                {
                    for (int y = 0; y < node.classes.Length; y++)
                    {
                        if (tmp.Length < node.model.Count * node.classes.Length)
                        {
                            node.Bad_Weights[x][y] = 1.0;
                        }
                        else
                            node.Bad_Weights[x][y] = Convert.ToDouble(tmp[y + x * node.classes.Length]);
                    }
                }
                node.Train_Weights = new double[node.model.Count][];
                for (int x = 0; x < node.model.Count; x++)
                    node.Train_Weights[x] = new double[node.classes.Length];
                m = ini.ReadString("Node" + (i + 1).ToString(), "Train_Weights", "");
                tmp = m.Split(',');
                for (int x = 0; x < node.model.Count; x++)
                {
                    for (int y = 0; y < node.classes.Length; y++)
                    {
                        if (tmp.Length < node.model.Count * node.classes.Length)
                        {
                            node.Train_Weights[x][y] = 1.0;
                        }
                        else
                            node.Train_Weights[x][y] = Convert.ToDouble(tmp[y + x * node.classes.Length]);
                    }
                }

                string nds = ini.ReadString("Node" + (i + 1).ToString(), "nodes", "");
                node.nodes = nds.Split(';');
                node.bUse = false;
                lstNode.Add(node);
            }
            return lstNode;
        }

        private bool Create_Yolo_Net()
        {
            yolo.Dispose();
            try
            {
                int n = 0;
                for (int i = 0; i < models.Count; i++)
                {
                    for (int j = 0; j < models[i].model.Count; j++)
                    {
                        yolo.Init(n, models[i].cfg_file[j], models[i].weight[j], 0, 1);
                        n++;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Add_Fix_Bad_Name(string name)
        {
            Fix_Bad_Name.Add(name);
        }

        public bool Fix_Bad(YoloResult result)
        {
            try
            {
                Fix_Thresh th = fix_thres.First(x => x.BadName == result.BadName);
                if (th == null)
                {
                    return false;
                }
                if (result.isDetection && result.prob > th.Threshold)
                    return true;
                else return false;
            }
            catch { return false; }
        }

        private bool IsFixBad(string BadType)
        {
            if (Fix_Bad_Name.Contains(BadType)) return true;
            else return false;
        }

        public YoloResult Predict_Yolo_BGA(string info, System.Windows.Media.Imaging.BitmapSource refimg, System.Windows.Media.Imaging.BitmapSource defimg, int nCamID, int no, out string inspName)
        {
            DefectInfo di = Defect_Info.GetDefectInfo(info, "CA", 0, no);
            inspName = di.BadName;

            Mat[] Feature;
            Mat dimg;
            try
            {
                Mat rimg = OpenCvSharp.Extensions.BitmapSourceConverter.ToMat(refimg);
                dimg = OpenCvSharp.Extensions.BitmapSourceConverter.ToMat(defimg);
                Feature = Processing.GetBlobImage(rimg, dimg, ref di);
            }
            catch
            {
                return new YoloResult();
            }
            if (Feature == null) return new YoloResult();

            YoloResult result = new YoloResult();
            result.Layer = 0;
            if (!IsCreate)
            {
                result.ClassName = "None";
                result.isDetection = false;
                result.ClassID = -1;
                result.BadName = "None";
                return result;
            }
            int nLayer = 1;
            bool bDone = false;
            string NextNet = "";
            int MaxLayer = 0;
            for (int i = 0; i < models.Count; i++)
            {
                MaxLayer = Math.Max(MaxLayer, models[i].Layer);
            }
            while (!bDone)
            {
                if (MaxLayer < nLayer)
                {
                    return new YoloResult();
                }
                bool bNext = false;
                int n = 0;
                for (int i = 0; i < models.Count; i++)
                {
                    n = 0;
                    for (int k = 0; k < i; k++)
                    {
                        n += models[k].model.Count;
                    }
                    if (models[i].CamID != nCamID && models[i].CamID != 0) continue;
                    if ((models[i].Layer == nLayer && NextNet == models[i].Name) || (models[i].Layer == nLayer && nLayer == 1))
                    {
                        YoloResult[] results = new YoloResult[models[i].model.Count];
                        int pdtIndex = 0;
                        for (int j = 0; j < models[i].model.Count; j++)
                        {
                            results[j] = new YoloResult();
                            results[j].Layer = nLayer;
                            switch (models[i].model[j])
                            {
                                case "Defect":
                                case "Feature":
                                    pdtIndex = -1; break;
                                case "MIX": pdtIndex = 0; break;
                                //case "Saturation": pdtIndex = 6; break;
                                //case "Segment_Blue": pdtIndex = 2; break;
                                //case "Segment_Green": pdtIndex = 1; break;
                                //case "Segment_Red": pdtIndex = 0; break;
                                case "Segment_Merge": pdtIndex = 4; break;
                                //case "Threshold": pdtIndex = 3; break;
                                case "RGB_Convert": pdtIndex = 7; break;

                            }

                            Mat dect;
                            try
                            {
                                if (pdtIndex >= 0)
                                    dect = Feature[pdtIndex].Clone();
                                else
                                    dect = dimg;
                            }
                            catch
                            {
                                bDone = true;
                                return new YoloResult();
                            }
                            bbox_t[] tmp = null;
                            lock (this)
                            {
                                tmp = yolo.Detect(n + j, dect.ToBytes());
                            }
                            results[j] = Networks.GetV4Result(tmp, dect.Clone(), models[i].thresholds[j].ToList(), models[i].classes);
                            results[j].ModelIndex = j;
                            results[j].BadName = models[i].badname[results[j].ClassID];
                            if (results[j].isDetection)
                            {
                                NextNet = models[i].nodes[results[j].ClassID];
                                CurrNode = models[i];
                            }
                            else
                            {
                                if (models[i].NonClass[j] > 0)
                                {
                                    results[j].prob = 0.1;
                                    results[j].isDetection = true;

                                    results[j].ClassID = models[i].NonClass[j] - 1;
                                    results[j].ClassName = models[i].classes[results[j].ClassID];
                                    NextNet = models[i].nodes[results[j].ClassID];
                                    results[j].BadName = models[i].badname[results[j].ClassID];
                                    CurrNode = models[i];
                                }
                            }
                        }

                        double max = 0.0;
                        for (int j = 0; j < models[i].model.Count; j++)
                        {

                            if (results[j].isDetection)
                            {
                                double cmax = results[j].prob * models[i].Bad_Weights[j][results[j].ClassID] * models[i].Train_Weights[j][results[j].ClassID];
                                if(result.Layer < results[j].Layer)
                                {
                                    max = cmax;
                                    result = results[j].Clone();
                                }
                                else if (cmax > max)
                                {
                                    max = cmax;
                                    result = results[j].Clone();
                                }
                                
                            }
                        }
                        if (result.isDetection)
                        {
                            bNext = false;
                            NextNet = models[i].nodes[result.ClassID];
                            CurrNode = models[i];
                            if (NextNet != "")
                                bNext = true;
                        }
                        else
                            bNext = true;
                    }
                }
                nLayer++;
                if (!bNext) bDone = true;
            }

            return result;
        }

        public static YoloResult GetResult(bbox_t[] bbox_Ts, Mat image, List<double> minProb, string[] Labels)
        {
            return GetV4Result(bbox_Ts, image, minProb, Labels);
        }

        private static YoloResult GetV4Result(bbox_t[] bbox_Ts, Mat image, List<double> minProb, string[] Labels)
        {
            var classIds = new List<uint>();
            var probabilities = new List<float>();
            var boxes = new List<Rect2d>();
            YoloResult yoloResult = new YoloResult();
            yoloResult.isDetection = false;
            yoloResult.prob = 0;
            int countDetectedObj = 0;

            foreach (var bbox in bbox_Ts)
            {
                if (bbox.prob >= minProb[(int)bbox.obj_id])
                {
                    yoloResult.prob = bbox.prob;
                    yoloResult.ClassID = (int)bbox.obj_id;
                    yoloResult.ClassName = Labels[yoloResult.ClassID];
                    classIds.Add(bbox.obj_id);
                    probabilities.Add(bbox.prob);
                    boxes.Add(new Rect2d(bbox.x, bbox.y, bbox.w, bbox.h));
                    countDetectedObj++;
                    yoloResult.isDetection = true;
                }
                else if (bbox.prob > 0.5 && bbox.prob < minProb[(int)bbox.obj_id])
                {
                    yoloResult.prob = bbox.prob;
                }
            }

            while (countDetectedObj != 0)
            {
                countDetectedObj--;
                var box = boxes[countDetectedObj];
                DrawV4(image, classIds[countDetectedObj], probabilities[countDetectedObj], box.X, box.Y, box.Width, box.Height, Labels);
            }

            yoloResult.resultImg = image;
            yoloResult.classIds = classIds;

            return yoloResult;
        }


        private static void DrawV4(Mat image, uint classes, float probability, double x, double y, double width, double height, string[] Labels)
        {
            var label = $"{Labels[classes]} {probability * 100:0.00}%";

            Cv2.Rectangle(image, new OpenCvSharp.Point(x, y), new OpenCvSharp.Point(x + width, y + height), new Scalar(252, 211, 20), 1);
            Cv2.PutText(image, label, new OpenCvSharp.Point(x, y - 1), HersheyFonts.Italic, 0.35, new Scalar(252, 211, 20), 1, LineTypes.AntiAlias);
        }

    }
}
