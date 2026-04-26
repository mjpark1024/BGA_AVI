using Common;
using Common.Drawing.InspectionTypeUI;
using DataMatrix.net;
using IGS.Classes;
using Keyence.AR.Communication;
using MySqlX.XDevAPI.Common;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using RVS.Generic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace HDSInspector
{
    public static class DataMatrixCode
    {
        static Mat mask1 = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(1, 1));
        static Mat mask2 = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));
        static DmtxImageDecoder decoder = new DmtxImageDecoder();
        static Regex regex = new Regex("^[a-zA-Z0-9]*$");

        static ZXing.IBarcodeReader zxing_reader = new BarcodeReader()
        {
            AutoRotate = true,
            TryInverted = true,
            Options = new ZXing.Common.DecodingOptions()
            {
                TryHarder = true,
                PossibleFormats = new List<BarcodeFormat>()
                {
                    BarcodeFormat.DATA_MATRIX
                }
            }
        };

        private static Rect GetBoundary(Mat src, int offset, int brightAndDark)
        {
            Scalar s = src.Mean();
            src = src.Threshold(s.Val0 - (s.Val0 * 0.1) + offset, 255, ThresholdTypes.Binary);

            Mat tmpMat = src.Clone();

            Mat ele = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));

            if (brightAndDark == 0)
            {
                Cv2.Erode(src, src, ele, new Point(-1, -1), 3);
                Cv2.Dilate(src, src, ele, new Point(-1, -1), 10);
            }
            else
            {
                Cv2.Dilate(src, src, ele, new Point(-1, -1), 10);
                Cv2.Erode(src, src, ele, new Point(-1, -1), 3);
            }

            Mat label, stats, cent;
            label = new Mat(); stats = new Mat(); cent = new Mat();
            int n = Cv2.ConnectedComponentsWithStats(src, label, stats, cent);
            Rect r = new Rect(0, 0, 0, 0);
            int offsetX = 0; int offsetY = 0; int gap = 0; int cmpGap = 0; int pivot = 0;
            Point center = new Point(src.Width / 2, src.Height / 2);

            int maxArea = 0;

            for (int i = 0; i < n; i++)
            {
                r.X = stats.At<int>(i, 0);
                r.Y = stats.At<int>(i, 1);
                r.Width = stats.At<int>(i, 2);
                r.Height = stats.At<int>(i, 3);
                int area = stats.At<int>(i, 4);
                if (r.X < 2 || r.Y < 2 || r.X + r.Width > src.Cols - 2 || r.Y + r.Height > src.Rows - 2) continue;
                if (area > 20000)
                {
                    if (maxArea < area)
                    {
                        maxArea = area;
                        offsetX = Math.Abs(center.X - r.X);
                        offsetY = Math.Abs(center.Y - r.Y);
                        gap = offsetX + offsetY;
                        if (cmpGap == 0 || cmpGap > gap)
                        {
                            cmpGap = gap;
                            pivot = i;
                        }
                    }
                }
            }

            r.X = stats.At<int>(pivot, 0);
            r.Y = stats.At<int>(pivot, 1);
            r.Width = stats.At<int>(pivot, 2);
            r.Height = stats.At<int>(pivot, 3);

            if (r.X < 2 || r.Y < 2 || r.X + r.Width > src.Cols - 2 || r.Y + r.Height > src.Rows - 2) return new Rect(0, 0, 0, 0);

            return r;
        }

        public static List<string> GetDataMatrixMJ(Bitmap srcBitmap, string lot, string type, int plcIndex)
        {
            try
            {
                List<string> codes = new List<string>();

                Mat origin_mat = BitmapConverter.ToMat(srcBitmap);
                Mat saveMat = origin_mat.Clone();

                if (origin_mat.Channels() > 1)
                    Cv2.CvtColor(origin_mat, origin_mat, ColorConversionCodes.BGR2GRAY);

                origin_mat = origin_mat.GaussianBlur(new Size(9, 9), 0.5);

                int offset = 60;
                int brightAndDark = 0;
                while (offset > -60 && brightAndDark < 2)
                {
                    offset -= 5;
                    Cv2.Normalize(origin_mat, origin_mat, 0, 255, NormTypes.MinMax);
                    Rect r = GetBoundary(origin_mat.Clone(), offset, brightAndDark);

                    if (offset == -60)
                    {
                        brightAndDark += 1;
                        offset = 60;
                    }

                    if (r.X == 0 && r.Y == 0 && r.Width == 0 && r.Height == 0) continue;

                    int margin = origin_mat.Width / 25;
                    r = new Rect(Math.Max(r.X - margin, 0), Math.Max(r.Y - margin, 0), Math.Min(r.Width + margin * 2, origin_mat.Cols), Math.Min(r.Height + margin * 2, origin_mat.Rows));

                    if (Math.Abs(r.Width + r.X) >= origin_mat.Cols || r.Width + r.X < 0) continue;
                    if (Math.Abs(r.Height + r.Y) >= origin_mat.Rows || r.Width + r.X < 0) continue;

                    Mat roiMat = origin_mat.SubMat(r);
                    Scalar s = roiMat.Mean();

                    //debug
                    //Cv2.ImShow("d", roiMat);
                    //Cv2.WaitKey(0);

                    for (int eleOffset = 1; eleOffset < 8; eleOffset += 2)
                    {
                        for (int roiOffset = 60; roiOffset >= -45; roiOffset -= 15)
                        {
                            Mat roiMatThresh = roiMat.Threshold(s.Val0 - (s.Val0 * 0.1) + roiOffset, 255, ThresholdTypes.Binary);

                            if (type != "AOI")
                            {
                                Mat ele = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(eleOffset, eleOffset));
                                Cv2.Erode(roiMatThresh, roiMatThresh, ele, new Point(-1, -1), 2);
                                Cv2.Dilate(roiMatThresh, roiMatThresh, ele, new Point(-1, -1), 5);
                                Cv2.Erode(roiMatThresh, roiMatThresh, ele, new Point(-1, -1), 3);
                            }
                            else
                            {
                                Mat ele = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(eleOffset, eleOffset));
                                Cv2.Erode(roiMatThresh, roiMatThresh, ele, new Point(-1, -1), 2);
                                Cv2.Dilate(roiMatThresh, roiMatThresh, ele, new Point(-1, -1), 5);
                                Cv2.Erode(roiMatThresh, roiMatThresh, ele, new Point(-1, -1), 2);
                            }

                            //resize (1.Decode 속도향상, 2.ㄴ 인식키 약간 틀어저 있는 것 보정, 잘못된 이미지 들어오더라도 속도빨라짐)
                            if (roiMatThresh.Width > 100 || roiMatThresh.Height > 100)
                                Cv2.Resize(roiMatThresh, roiMatThresh, new Size(100, 100));

                            var zxresult = RecognitionMatrix(roiMatThresh.Clone(), type);

                            if (zxresult != "")
                            {
                                codes.Add(zxresult);
                                //SaveMCRImg(roiMatThresh.Clone(), type, plcIndex, lot + "_기존_" + zxresult); // 인식 성공
                                //SaveMCRImg(roiMat.Clone(), type, plcIndex, lot + "_기존_Success_Origin"); // 인식 실패
                                Log2DMark(saveMat, lot, plcIndex, codes);

                                return check_datamatrix(codes, lot) ? codes : null;
                            }
                            else
                            {
                                //SaveMCRImg(roiMat.Clone(), type, plcIndex, lot + "_기존_FailOrigin"); // 인식 실패
                                //SaveMCRImg(roiMatThresh.Clone(), type, plcIndex, lot + "_기존_FailThresh"); // 인식 실패
                            }
                        }
                    }
                }
                Log2DMark(saveMat, lot, plcIndex, codes);
                return new List<string>();
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }

        public static string RecognitionMatrix(Mat roiMat, string Type)
        {
            try
            {
                List<string> lstResult = new List<string>();
                string findResult = "";

                //Cv2.ImShow("d", roiMat);
                //Cv2.WaitKey(0);

                var result = zxing_reader.Decode(roiMat.ToBitmap());
                if (result != null) lstResult.Add(result.Text);

                //flip
                Mat flipMat = new Mat();
                if (lstResult.Count == 0)
                {
                    for (int i = -1; i < 2; i++)
                    {
                        Cv2.Flip(roiMat, flipMat, (FlipMode)i);
                        result = zxing_reader.Decode(flipMat.ToBitmap());
                        if (result != null)
                        {
                            lstResult.Add(result.Text);
                            break;
                        }
                    }
                }

                var group = lstResult.GroupBy(i => i);
                int maxCount = 0;
                foreach (var g in group)
                {
                    if (maxCount <= g.Count())
                    {
                        maxCount = g.Count();
                        findResult = g.Key;
                    }
                }

                return findResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private static void Log2DMark(Mat origin, string lot, int plcIndex, List<string> id)
        {
            string path = MainWindow.Setting.General.ResultPath + "\\" + MainWindow.CurrentGroup.Name + "\\"
                + MainWindow.CurrentModel.Name + "\\" + lot + "\\2D Mark log\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //Origin123456789 0025_Recog
            string info = "Origin" + lot + " " + plcIndex.ToString("D4") + "_Recog";

            //Origin123456789 0025_Recog123456789 0025
            if (id.Count > 0)
            {
                string[] ids = id[0].Split(' ');
                if (ids.Length >= 2)
                    info += ids[0] + " " + ids[1];

                string[] result = info.Split('_');

                if (MainWindow.curLotData == null)
                {
                    if (lot == "101111111" || lot == "Teaching")
                        info += "_[테스트]";
                    else
                        info += "_[정상]";
                }
                else
                {
                    //Origin123456789 0025_Recog123456789 0025_일치
                    if (lot == "101111111" || lot == "Teaching")
                        info += "_[테스트]";
                    else if (MainWindow.curLotData.ITS_ORDER != ids[0])
                        info += "_[오인식]";
                    else
                        info += "_[정상]";
                }
            }
            else
            {
                info += "_[미인식]";
            }

            origin.SaveImage(path + info + ".png");
        }

        public static bool check_datamatrix(List<string> data_list, string lot)
        {
            Regex regex = new Regex("[^0-9]"); // Allow

            if (data_list.Count < 1) return false;

            if (MainWindow.CurrentModel.Marker.IDMark > 0) return true;

            if (lot == "Teaching") return true;

            if (lot == "101111111") return true;

            //if (!data_list[0].StartsWith(MainWindow.curLotData.ITS_ORDER)) return false;

            string[] result = data_list[0].Split(' ');

            if (regex.IsMatch(result[1])) return false;

            if (Convert.ToInt32(result[1]) > 999) return false;

            return true;
        }

        public static bool checkAndLogReaderData(Mat img, string readerData, string lot, int plcIndex)
        {
            //Log
            string path = MainWindow.Setting.General.ResultPath + "\\" + MainWindow.CurrentGroup.Name + "\\"
                + MainWindow.CurrentModel.Name + "\\" + lot + "\\2D Mark log\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //Origin123456789 0025_Recog
            string info = "Origin" + lot + " " + plcIndex.ToString("D4") + "_Recog";

            //Origin123456789 0025_Recog123456789 0025
            if (readerData != "")
            {
                string[] ids = readerData.Split(' ');
                if (ids.Length >= 2)
                    info += ids[0] + " " + ids[1];

                //Origin123456789 0025_Recog123456789 0025_일치
                if (lot == "101111111" && lot == "Teaching")
                    info += "_[테스트]";
                else if (MainWindow.curLotData.ITS_ORDER != ids[0])
                    info += "_[오인식]";
                else
                    info += "_[정상]";
            }
            else
            {
                info += "_[미인식]";
            }

            img.SaveImage(path + info + "_Reader" + ".png");

            //Valid Check
            Regex regex = new Regex("[^0-9]"); // Allow

            if (lot == "Teaching") return true;

            if (!readerData.StartsWith(MainWindow.curLotData.ITS_ORDER)) return false;

            string[] result = readerData.Split(' ');

            if (regex.IsMatch(result[1])) return false;

            if (Convert.ToInt32(result[1]) > 999) return false;

            return true;
        }
        enum MCR_Simbols
        {
            MCR_Simbols_Auto = 0,
            MCR_Simbols_10x10 = 10,
            MCR_Simbols_11x11,
            MCR_Simbols_12x12,
            MCR_Simbols_13x13,
            MCR_Simbols_14x14,
            MCR_Simbols_15x15,
            MCR_Simbols_16x16,
            MCR_Simbols_17x17,
            MCR_Simbols_18x18,
            MCR_Simbols_19x19,
            MCR_Simbols_20x20,
            MCR_Simbols_21x21,
            MCR_Simbols_22x22,
            MCR_Simbols_23x23,
            MCR_Simbols_24x24,
            MCR_Simbols_25x25,
            MCR_Simbols_26x26,
        };
        static int m_iMCROrigin;
        static Rect m_RectCode = new Rect();
        static SortedDictionary<float, int> mapPeakInfo;
        static List<int>[] ListEdgePoints = new List<int>[2]; // 0 = x; 1 = y;
        static bool[] m_bMCROrigin = new bool[2];

        public static List<string> Algo_Conv_Square_DataMatrix(Bitmap srcBitmap, string lot, string Type, int plcIndex)
        {
            try
            {
                Mat origin_mat = BitmapConverter.ToMat(srcBitmap);
                Mat image = origin_mat.Clone();
                Mat saveMat = origin_mat.Clone();
                Mat DstImg = origin_mat.Clone();
                List<string> codes = new List<string>();
                Mat label_box = new Mat();

                m_bMCROrigin[0] = false;
                m_bMCROrigin[1] = false;
                origin_mat.CopyTo(image);

                Mat image_gray = new Mat();
                Mat image_bi = new Mat();

                for (int i = 0; i < 2; i++)
                    ListEdgePoints[i] = new List<int>();

                // 라벨 레이어 변수
                Mat img_label = new Mat();
                Mat stats = new Mat();
                Mat centroids = new Mat();
                Mat mask = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3), new Point(1, 1));

                int label;
                // 복사
                label_box = image.Clone();

                //그레이스케일
                if (origin_mat.Channels() > 1)
                    Cv2.CvtColor(origin_mat, image_gray, ColorConversionCodes.BGR2GRAY);
                else
                    origin_mat.CopyTo(image_gray);

                Mat MatRoiImg = new Mat();
                Mat tempImg = new Mat();
                Mat MCRResultImg;
                Mat MatRoiImgBi = new Mat(); ;
                Mat MatRoiImgBiLine = new Mat();
                Mat MatRoiImgDot = new Mat();

                int iboraderLength = 0;
                List<Mat> ListMatRoiImg = new List<Mat>();
                List<Rect> ListFindRect = new List<Rect>();
                bool bFind = false;
                bool bReverse = false;
                List<bool> ListReverseFlag = new List<bool>();

                bool bRetry = false;
                bool bFindFlag = true;
                ListMatRoiImg.Clear();
                bool AutoSizeChk = true;
                for (int i = 0; i < 1; i++)
                {
                    //if (bFind == true) break;
                    for (int z = 0; z < 10; z++)
                    {
                        //if (bFind == true) break;;
                        if (i == 0) //이진화
                        {
                            //OTSU 알고리즘
                            if (z == 0) Cv2.Threshold(image_gray, image_bi, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
                            Cv2.Dilate(image_bi, image_bi, mask, new Point(1, 1), 1, BorderTypes.Replicate); //노이즈 제거            
                        }
                        //else if (i == 1) //(첫번째 이진화 실패할 경우 반전)
                        //{
                        //    bReverse = true;
                        //
                        //    //OTSU 알고리즘
                        //    if(z == 0) Cv2.Threshold(image_gray, image_bi, 0, 255, ThresholdTypes.BinaryInv | ThresholdTypes.Otsu);
                        //    Cv2.Erode(image_bi, image_bi, mask, new Point(1, 1), 1, BorderTypes.Replicate);
                        //}                   

                        Mat cent = new Mat();
                        label = Cv2.ConnectedComponentsWithStats(image_bi, img_label, stats, centroids);

                        int iCutSize = 1;

                        if (!AutoSizeChk) iCutSize = 0;

                        int area, left, top, width, height;
                        for (int j = 1; j < label; j++)
                        {
                            if (bFind == true && i != 0) break;
                            left = stats.At<int>(j, (int)ConnectedComponentsTypes.Left);
                            top = stats.At<int>(j, (int)ConnectedComponentsTypes.Top);
                            width = stats.At<int>(j, (int)ConnectedComponentsTypes.Width);
                            height = stats.At<int>(j, (int)ConnectedComponentsTypes.Height);
                            area = stats.At<int>(j, (int)ConnectedComponentsTypes.Area);

                            // 찾은 덩어리 중 큰것부터 작은 것 순으로 검색하여. 정사각형과 유사하고, 사이즈가 적당히 큰 것으로 리턴한다. 큰 사각형 테두리를 찾더라도 더 작은 것이 우선
                            if (Math.Abs(width - height) < 10 && width > 150 && height > 150)
                            {
                                // 라벨링 박스
                                if (left < iCutSize || top < iCutSize || image.Cols < left + width + (iCutSize) || image.Rows < top + height + (iCutSize)) continue;

                                Cv2.Rectangle(label_box, new Rect(left - iCutSize, top - iCutSize, width + (iCutSize * 2), height + (iCutSize * 2)), new Scalar(0, 0, 255), 3);

                                Rect roi = new Rect(left - iCutSize, top - iCutSize, width + (iCutSize * 2), height + (iCutSize * 2));
                                MatRoiImg = image_gray.SubMat(roi);
                                Rect biroi = new Rect(left - iCutSize, top - iCutSize, width + (iCutSize * 2), height + (iCutSize * 2));
                                tempImg = image_bi.SubMat(biroi);

                                int Size = 0;
                                FindCountour(tempImg, ref Size);
                                if (Size < 15)//찾은 뭉티기 개수가 15개 미만(MCR Image를 제대로 커트했으면 15개 이상 나옴)
                                {
                                    continue;
                                }
                                ListMatRoiImg.Add(MatRoiImg);
                                ListFindRect.Add(new Rect(new Point(top, left), new Size(height, width)));
                                ListReverseFlag.Add(bReverse);
                                bFind = true;
                            }
                        }
                    }
                }

                if (ListMatRoiImg.Count() == 0)
                    bFindFlag = false;

                for (int k = 0; k < ListMatRoiImg.Count(); k++) //찾은 후보들을 전부 Search
                {
                    bFindFlag = true;
                    if (ListMatRoiImg == null)
                    {
                        //AfxMessageBox("Cannot Find MCR Image! : label Fail");
                        bFindFlag = false;
                        break;
                    }
                    if (ListMatRoiImg[k].Rows < 100)// 100pixel 미만인것들은 Resize 한다.
                    {
                        //double zoom =(int)(100.0 / vecMatRoiImg[k].Rows+0.5);
                        Cv2.Resize(ListMatRoiImg[k], ListMatRoiImg[k], new Size(), 3.0, 3.0, InterpolationFlags.Lanczos4);
                    }
                    else
                    {
                        if (!bRetry)
                        { //후보의 노이즈 제거
                            if (ListReverseFlag[k])
                            {
                                Cv2.Erode(ListMatRoiImg[k], ListMatRoiImg[k], mask, new Point(-1, -1), 1, BorderTypes.Replicate);
                                Cv2.Dilate(ListMatRoiImg[k], ListMatRoiImg[k], mask, new Point(-1, -1), 1, BorderTypes.Replicate);

                            }
                            else
                            {
                                Cv2.Dilate(ListMatRoiImg[k], ListMatRoiImg[k], mask, new Point(-1, -1), 1, BorderTypes.Replicate);
                                Cv2.Erode(ListMatRoiImg[k], ListMatRoiImg[k], mask, new Point(-1, -1), 1, BorderTypes.Replicate);
                            }
                        }
                        else
                        {
                            if (ListReverseFlag[k])
                            {
                                Cv2.Dilate(ListMatRoiImg[k], ListMatRoiImg[k], mask, new Point(-1, -1), 1, BorderTypes.Replicate);
                                Cv2.Erode(ListMatRoiImg[k], ListMatRoiImg[k], mask, new Point(-1, -1), 1, BorderTypes.Replicate);
                            }
                            else
                            {
                                Cv2.Erode(ListMatRoiImg[k], ListMatRoiImg[k], mask, new Point(-1, -1), 1, BorderTypes.Replicate);
                                Cv2.Dilate(ListMatRoiImg[k], ListMatRoiImg[k], mask, new Point(-1, -1), 1, BorderTypes.Replicate);
                            }
                        }
                    }
                    int iMeanPeak = 0, iPeakCnt = 0;
                    mapPeakInfo = new SortedDictionary<float, int>();
                    FindHistMeanPeak(ListMatRoiImg[k], out iMeanPeak);//찾은 후보를 히스토그램을 구해 가장 높은 Peak 와 그 다음 Peaak의 평균 값으로 threshold
                    if (iMeanPeak < 60) iMeanPeak = 80;
                    Cv2.Threshold(ListMatRoiImg[k], MatRoiImgBi, iMeanPeak, 255, ThresholdTypes.Binary);

                    iboraderLength = (int)(MatRoiImgBi.Rows / 100); //가장자리에 여백 만들기(여백이 어느정도 있어야함)
                    if (!bRetry)
                    {
                        if (ListReverseFlag[k])
                        {
                            Cv2.CopyMakeBorder(MatRoiImgBi, MatRoiImgBi, iboraderLength, iboraderLength, iboraderLength, iboraderLength, BorderTypes.Constant, 255);
                            Cv2.Dilate(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                            Cv2.Erode(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                        }
                        else
                        {
                            Cv2.CopyMakeBorder(MatRoiImgBi, MatRoiImgBi, iboraderLength, iboraderLength, iboraderLength, iboraderLength, BorderTypes.Constant, 0);
                            Cv2.Erode(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                            Cv2.Dilate(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                        }
                    }
                    else
                    {
                        if (ListReverseFlag[k])
                        {
                            Cv2.CopyMakeBorder(MatRoiImgBi, MatRoiImgBi, iboraderLength, iboraderLength, iboraderLength, iboraderLength, BorderTypes.Constant, 0);
                            Cv2.Erode(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                            Cv2.Dilate(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                        }
                        else
                        {
                            Cv2.CopyMakeBorder(MatRoiImgBi, MatRoiImgBi, iboraderLength, iboraderLength, iboraderLength, iboraderLength, BorderTypes.Constant, 255);
                            Cv2.Dilate(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                            Cv2.Erode(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                        }
                    }

                    int[] iindexChk = new int[2];
                    int iMatrixCnt = 0;
                    if (AutoSizeChk)
                    {
                        iMatrixCnt = 0;
                        int iCntTmp = (int)MCR_Simbols.MCR_Simbols_Auto;

                        for (int rotate = 0; rotate < 2; rotate++) //상면, 하면 검사하여 Matrix Dot Count Check
                        {
                            //OriginMake 끊어진 LINE 이어주기 
                            //FindMCRCnt X by X MCR 인지 검색
                            Size DotSize = new Size();
                            while (OriginMake(MatRoiImgBi.Clone(), ref DotSize) ? !FindMCRCnt(MatRoiImgBi, DotSize, ref iCntTmp, rotate) : true)
                            {
                                iPeakCnt++;
                                if (!FindHistMeanPeak(ListMatRoiImg[k], out iMeanPeak, iPeakCnt) || iPeakCnt > 5)
                                {
                                    //AfxMessageBox("Cannot Find MCR Image!");
                                    bFindFlag = false;
                                    break;
                                }

                                rotate = 0;
                                Cv2.Threshold(ListMatRoiImg[k], MatRoiImgBi, iMeanPeak, 255, ThresholdTypes.Binary);
                                if (!bRetry)
                                {
                                    if (ListReverseFlag[k])
                                    {
                                        Cv2.CopyMakeBorder(MatRoiImgBi, MatRoiImgBi, iboraderLength, iboraderLength, iboraderLength, iboraderLength, BorderTypes.Constant, 255);
                                        Cv2.Dilate(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                                        Cv2.Erode(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                                    }
                                    else
                                    {
                                        Cv2.CopyMakeBorder(MatRoiImgBi, MatRoiImgBi, iboraderLength, iboraderLength, iboraderLength, iboraderLength, BorderTypes.Constant, 0);
                                        Cv2.Erode(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                                        Cv2.Dilate(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                                    }
                                }
                                else
                                {
                                    if (ListReverseFlag[k])
                                    {
                                        Cv2.CopyMakeBorder(MatRoiImgBi, MatRoiImgBi, iboraderLength, iboraderLength, iboraderLength, iboraderLength, BorderTypes.Constant, 0);
                                        Cv2.Erode(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                                        Cv2.Dilate(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                                    }
                                    else
                                    {
                                        Cv2.CopyMakeBorder(MatRoiImgBi, MatRoiImgBi, iboraderLength, iboraderLength, iboraderLength, iboraderLength, BorderTypes.Constant, 255);
                                        Cv2.Dilate(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                                        Cv2.Erode(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                                    }
                                }
                            }

                            if (iCntTmp >= iMatrixCnt)
                                iMatrixCnt = iCntTmp;
                            if (bFindFlag == false && bRetry == true) continue;

                            if (iMatrixCnt < 10)
                            {
                                continue;
                            }
                        }

                        if (iMatrixCnt < 10 || ListEdgePoints[0].Count() < 0 || ListEdgePoints[1].Count() < 0 ||
                            ListEdgePoints[0].Count() % 2 == 1 ||
                            ListEdgePoints[1].Count() % 2 == 1 ||
                            ListEdgePoints[0].Count() != ListEdgePoints[1].Count())
                        {
                            bFindFlag = false;
                        }

                        if (bFindFlag == true)
                        {
                            MatRoiImgBi.CopyTo(MatRoiImgBiLine);
                            MatRoiImgBi.CopyTo(MatRoiImgDot);
                            //if (ListMatRoiImg[k].Channels() > 1)
                            Cv2.CvtColor(ListMatRoiImg[k], ListMatRoiImg[k], ColorConversionCodes.GRAY2BGR);
                            //if (MatRoiImgBiLine.Channels() > 1)
                            Cv2.CvtColor(MatRoiImgBiLine, MatRoiImgBiLine, ColorConversionCodes.GRAY2BGR);
                            //kbs 라인 search
                            //for (int rotate = 0; rotate < ListEdgePoints[0].Count; rotate++) //검증용 Line 그리기
                            //{
                            //    Cv2.Line(MatRoiImgBiLine, ListEdgePoints[0][rotate], 0, ListEdgePoints[0][rotate], MatRoiImgBi.Rows, new Scalar(0, 255, 0));
                            //}
                            //for (int rotate = 0; rotate < ListEdgePoints[1].Count; rotate++)
                            //{
                            //    Cv2.Line(MatRoiImgBiLine, 0, ListEdgePoints[1][rotate], MatRoiImgBi.Cols, ListEdgePoints[1][rotate], new Scalar(0, 255, 0));
                            //}
                            Mat matCrossLineImg = new Mat();
                            MatRoiImgBiLine.CopyTo(matCrossLineImg);
                            Cv2.Resize(matCrossLineImg, matCrossLineImg, new Size(300, 300), 0, 0, InterpolationFlags.Linear);
                            //MatRoiImgBiLine
                            int ix, iy;
                            if (!bRetry)
                            {
                                if (ListReverseFlag[k])
                                    MCRResultImg = new Mat(ListEdgePoints[1].Count + 3, ListEdgePoints[0].Count + 3, MatType.CV_8U, new Scalar(255));
                                else
                                    MCRResultImg = new Mat(ListEdgePoints[1].Count + 3, ListEdgePoints[0].Count + 3, MatType.CV_8U, new Scalar(0));
                            }
                            else
                            {
                                if (!ListReverseFlag[k])
                                    MCRResultImg = new Mat(ListEdgePoints[1].Count + 3, ListEdgePoints[0].Count + 3, MatType.CV_8U, new Scalar(255));
                                else
                                    MCRResultImg = new Mat(ListEdgePoints[1].Count + 3, ListEdgePoints[0].Count + 3, MatType.CV_8U, new Scalar(0));
                            }
                            unsafe
                            {
                                //바이너리  IMAGE 생성 
                                Mat MCRResult = MCRResultImg.SubMat(new Rect(1, 1, ListEdgePoints[1].Count + 1, ListEdgePoints[0].Count + 1));
                                byte* dotPixel;
                                byte[] mcrPixel = new byte[MatRoiImgBi.Rows];
                                for (int rotate = 0; rotate <= ListEdgePoints[1].Count; rotate++)
                                {
                                    if (rotate == 0)
                                    {
                                        if (m_iMCROrigin == 3 || m_iMCROrigin == 4)
                                            continue;
                                        ix = Convert.ToInt32((double)(ListEdgePoints[1][rotate] / 2) + 0.5);

                                    }
                                    else if (rotate == ListEdgePoints[1].Count)
                                    {
                                        if (m_iMCROrigin == 1 || m_iMCROrigin == 2)
                                            continue;
                                        //ix = (double)(MatRoiImgBi.Rows + vecEdgePoints[1][i - 1]) / 2 + 0.5;
                                        ix = Convert.ToInt32((double)(MatRoiImgBi.Rows + ListEdgePoints[1][rotate - 1]) / 2 - 0.5);
                                    }
                                    else
                                        if (m_iMCROrigin == 1 || m_iMCROrigin == 2)
                                        ix = Convert.ToInt32((double)(ListEdgePoints[1][rotate - 1] + ListEdgePoints[1][rotate]) / 2 - 0.5);
                                    else
                                        ix = Convert.ToInt32((double)(ListEdgePoints[1][rotate - 1] + ListEdgePoints[1][rotate]) / 2 + 0.5);

                                    dotPixel = (byte*)MatRoiImgDot.Ptr(ix);
                                    for (int j = 0; j <= ListEdgePoints[0].Count; j++)
                                    {
                                        if (j == 0)
                                        {
                                            if (m_iMCROrigin == 1 || m_iMCROrigin == 3)
                                                continue;
                                            iy = Convert.ToInt32((double)(ListEdgePoints[0][j] / 2) + 0.5);

                                        }
                                        else if (j == ListEdgePoints[0].Count)
                                        {
                                            if (m_iMCROrigin == 2 || m_iMCROrigin == 4)
                                                continue;
                                            iy = Convert.ToInt32((double)(MatRoiImgBi.Cols + ListEdgePoints[0][j - 1]) / 2 + 0.5);
                                        }
                                        else
                                            iy = Convert.ToInt32((double)(ListEdgePoints[0][j - 1] + ListEdgePoints[0][j]) / 2 + 0.5);


                                        MCRResult.SetArray(rotate, j, dotPixel[iy]);
                                        //MCRResult.DataPointer[(MatRoiImgBi.Cols)]= dotPixel[iy];
                                        dotPixel[iy] = 127;
                                    }
                                }
                                //가장자리 영역 재구성 (X x X)
                                //상                            
                                //for (int rotate = 1; rotate < MCRResult.Cols; rotate++)
                                //{
                                //    if (m_iMCROrigin == 1 || m_iMCROrigin == 2) break;
                                //    byte value = MCRResult.At<byte>(0, rotate - 1);
                                //    value = (value == 255) ? (byte)0 : (byte)255;
                                //    MCRResult.Set(0, rotate, value);
                                //}
                                ////하
                                //for (int rotate = 1; rotate < MCRResult.Cols; rotate++)
                                //{
                                //    if (m_iMCROrigin == 1 || m_iMCROrigin == 2) break;
                                //    byte value = MCRResult.At<byte>(MCRResult.Rows - 1, rotate - 1);
                                //    MCRResult.Set(MCRResult.Rows - 1, rotate, (value == 255) ? (byte)0 : (byte)255);
                                //}
                                ////좌
                                //for (int rotate = 1; rotate < MCRResult.Rows; rotate++)
                                //{
                                //    if (m_iMCROrigin == 1 || m_iMCROrigin == 2) break;
                                //    byte value = MCRResult.At<byte>(rotate - 1, 0);
                                //    MCRResult.Set(rotate, 0, (value == 255) ? (byte)0 : (byte)255);
                                //}
                                ////우
                                //for (int rotate = 1; rotate < MCRResult.Rows; rotate++)
                                //{
                                //    if (m_iMCROrigin == 1 || m_iMCROrigin == 2) break;
                                //    byte value = MCRResult.At<byte>(rotate - 1, MCRResult.Cols - 1);
                                //    MCRResult.Set(rotate, MCRResult.Cols - 1, (value == 255) ? (byte)0 : (byte)255);
                                //}

                                switch (m_iMCROrigin)
                                {
                                    case 3:
                                        MCRResultImg = new Mat(MCRResultImg, new Rect(1, 1, MCRResultImg.Cols - 1, MCRResultImg.Rows - 1));
                                        break;
                                    case 4:
                                        MCRResultImg = new Mat(MCRResultImg, new Rect(0, 1, MCRResultImg.Cols - 1, MCRResultImg.Rows - 1));
                                        break;
                                    case 1:
                                        MCRResultImg = new Mat(MCRResultImg, new Rect(1, 0, MCRResultImg.Cols - 1, MCRResultImg.Rows - 1));
                                        break;
                                    case 2:
                                        MCRResultImg = new Mat(MCRResultImg, new Rect(0, 0, MCRResultImg.Cols - 1, MCRResultImg.Rows - 1));
                                        break;
                                    default:
                                        MCRResultImg = new Mat(MCRResultImg, new Rect(1, 0, MCRResultImg.Cols - 1, MCRResultImg.Rows - 1));
                                        break;
                                }
                            }
                            Cv2.Resize(MCRResultImg, MCRResultImg, new Size(MCRResultImg.Cols * 10, MCRResultImg.Rows * 10), 0, 0, InterpolationFlags.Nearest);
                            MCRResultImg.CopyTo(DstImg);
                        }
                        m_RectCode = ListFindRect[k];
                    }
                    //CString str_ttmp;
                    //str_ttmp.Format("Matrix : %d x %d ", iMatrixCnt, iMatrixCnt);
                    if (!bFindFlag)
                    {
                        if (k == ListMatRoiImg.Count - 1 && bRetry == false)
                        {
                            bRetry = true;
                            k = -1;
                        }
                        continue;
                    }
                    if (bFindFlag)
                    {
                        var zxresult = RecognitionMatrix(DstImg.Clone(), Type);
                        if (zxresult != "")
                        {
                            codes.Add(zxresult);
                            Log2DMark(DstImg, lot, plcIndex, codes);
                            if (MainWindow.bMCR_TEST_SAVE)
                            {
                                SaveMCRImg(origin_mat.Clone(), Type, plcIndex, lot, "Success", codes); // 인식 성공
                                SaveMCRImg(DstImg.Clone(), Type, plcIndex, lot, "Convert", codes); // 인식 성공
                            }
                            return check_datamatrix(codes, lot) ? codes : null;
                        }
                    }

                }
                //else
                {
                    Log2DMark(saveMat, lot, plcIndex, codes);
                    if (MainWindow.bMCR_TEST_SAVE) SaveMCRImg(origin_mat.Clone(), Type, plcIndex, lot, "Fail", codes); // 인식 실패  
                    return codes;
                }
            }
            catch
            {
                List<string> result = new List<string>();
                return result;
            }
            List<string> error = new List<string>();
            return error;
        }
        public static List<string> Algo_Conv_Square_DataMatrix2(Bitmap srcBitmap, string lot, string Type, int plcIndex)
        {
            try
            {
                Mat origin_mat = BitmapConverter.ToMat(srcBitmap);
                Mat image = origin_mat.Clone();
                Mat saveMat = origin_mat.Clone();
                Mat DstImg = origin_mat.Clone();
                List<string> codes = new List<string>();
                Mat label_box = new Mat();

                m_bMCROrigin[0] = false;
                m_bMCROrigin[1] = false;
                origin_mat.CopyTo(image);

                Mat image_gray = new Mat();
                Mat image_bi = new Mat();

                for (int i = 0; i < 2; i++)
                    ListEdgePoints[i] = new List<int>();

                // 라벨 레이어 변수
                Mat img_label = new Mat();
                Mat stats = new Mat();
                Mat centroids = new Mat();
                Mat mask = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3), new Point(1, 1));

                int label;
                // 복사
                label_box = image.Clone();

                //그레이스케일
                if (origin_mat.Channels() > 1)
                    Cv2.CvtColor(origin_mat, image_gray, ColorConversionCodes.BGR2GRAY);
                else
                    origin_mat.CopyTo(image_gray);

                Mat MatRoiImg = new Mat();
                Mat tempImg = new Mat();
                Mat MCRResultImg;
                Mat MatRoiImgBi = new Mat(); ;
                Mat MatRoiImgBiLine = new Mat();
                Mat MatRoiImgDot = new Mat();

                int iboraderLength = 0;
                List<Mat> ListMatRoiImg = new List<Mat>();
                List<Rect> ListFindRect = new List<Rect>();
                bool bFind = false;
                bool bReverse = false;
                List<bool> ListReverseFlag = new List<bool>();

                bool bRetry = false;
                bool bFindFlag = true;
                ListMatRoiImg.Clear();
                bool AutoSizeChk = true;
                for (int i = 0; i < 1; i++)
                {
                    //if (bFind == true) break;
                    for (int z = 0; z < 10; z++)
                    {
                        //if (bFind == true) break;;
                        if (i == 0) //이진화
                        {
                            //OTSU 알고리즘
                            if (z == 0) Cv2.Threshold(image_gray, image_bi, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
                            Cv2.Dilate(image_bi, image_bi, mask, new Point(1, 1), 1, BorderTypes.Replicate); //노이즈 제거
                        }

                        Mat cent = new Mat();
                        label = Cv2.ConnectedComponentsWithStats(image_bi, img_label, stats, centroids);
                        int iCutSize = 1;

                        if (!AutoSizeChk) iCutSize = 0;

                        int area, left, top, width, height;
                        for (int j = 1; j < label; j++)
                        {
                            //if (bFind == true && i != 0)
                            //    break;
                            left = stats.At<int>(j, (int)ConnectedComponentsTypes.Left);
                            top = stats.At<int>(j, (int)ConnectedComponentsTypes.Top);
                            width = stats.At<int>(j, (int)ConnectedComponentsTypes.Width);
                            height = stats.At<int>(j, (int)ConnectedComponentsTypes.Height);
                            area = stats.At<int>(j, (int)ConnectedComponentsTypes.Area);

                            // 찾은 덩어리 중 큰것부터 작은 것 순으로 검색하여. 정사각형과 유사하고, 사이즈가 적당히 큰 것으로 리턴한다. 큰 사각형 테두리를 찾더라도 더 작은 것이 우선
                            if (Math.Abs(width - height) < 10 && width > 150 && height > 150)
                            {
                                // 라벨링 박스
                                if (left < iCutSize || top < iCutSize || image.Cols < left + width + (iCutSize) || image.Rows < top + height + (iCutSize)) continue;

                                Cv2.Rectangle(label_box, new Rect(left - iCutSize, top - iCutSize, width + (iCutSize * 2), height + (iCutSize * 2)), new Scalar(0, 0, 255), 3);

                                Rect roi = new Rect(left - iCutSize, top - iCutSize, width + (iCutSize * 2), height + (iCutSize * 2));
                                MatRoiImg = image_gray.SubMat(roi);
                                Rect biroi = new Rect(left - iCutSize, top - iCutSize, width + (iCutSize * 2), height + (iCutSize * 2));
                                tempImg = image_bi.SubMat(biroi);

                                int Size = 0;
                                FindCountour(tempImg, ref Size);
                                if (Size < 15)//찾은 뭉티기 개수가 15개 미만(MCR Image를 제대로 커트했으면 15개 이상 나옴)
                                {
                                    continue;
                                }
                                ListMatRoiImg.Add(MatRoiImg);
                                ListFindRect.Add(new Rect(new Point(top, left), new Size(height, width)));
                                ListReverseFlag.Add(bReverse);
                                bFind = true;
                            }
                        }
                    }
                }

                if (ListMatRoiImg.Count() == 0)
                    bFindFlag = false;

                for (int k = 0; k < ListMatRoiImg.Count(); k++) //찾은 후보들을 전부 Search
                {
                    //Bitmap dddd = BitmapConverter.ToBitmap(ListMatRoiImg[k]);
                    if (bFindFlag)
                    {
                        Mat dstimage = NewConvertMatrix(ListMatRoiImg[k], 0, FindEdge(ListMatRoiImg[k]));
                        if (dstimage == null) continue;

                        var zxresult = RecognitionMatrix(dstimage.Clone(), Type);
                        if (zxresult != "")
                        {
                            codes.Add(zxresult);
                            Log2DMark(dstimage.Clone(), lot, plcIndex, codes);
                            if (MainWindow.bMCR_TEST_SAVE)
                            {
                                SaveMCRImg(origin_mat.Clone(), Type, plcIndex, lot, "Success", codes); // 인식 성공
                                SaveMCRImg(dstimage.Clone(), Type, plcIndex, lot, "Convert", codes); // 인식 성공
                            }
                            return check_datamatrix(codes, lot) ? codes : null;
                        }
                        else
                        {
                            dstimage = NewConvertMatrix(ListMatRoiImg[k], 1, FindEdge(ListMatRoiImg[k]));
                            if (dstimage == null) continue;
                            zxresult = RecognitionMatrix(dstimage.Clone(), Type);
                            if (zxresult != "")
                            {
                                codes.Add(zxresult);
                                Log2DMark(dstimage.Clone(), lot, plcIndex, codes);
                                if (MainWindow.bMCR_TEST_SAVE)
                                {
                                    SaveMCRImg(origin_mat.Clone(), Type, plcIndex, lot, "Success", codes); // 인식 성공
                                    SaveMCRImg(dstimage.Clone(), Type, plcIndex, lot, "Convert", codes); // 인식 성공
                                }
                                return check_datamatrix(codes, lot) ? codes : null;
                            }
                        }
                    }
                }
            }
            catch
            {
                List<string> result = new List<string>();
                return result;
            }
            List<string> error = new List<string>();
            return error;
        }
        public static int FindEdge(Mat src, int thickness = 20)
        {
            int H = src.Rows;
            int W = src.Cols;

            // 흑백 변환
            Mat gray = new Mat();
            if (src.Channels() > 1)
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            else
                gray = src.Clone();

            // 이진화
            Mat binary = new Mat();
            Cv2.Threshold(gray, binary, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

            // 각 변 카운트
            int top = Cv2.CountNonZero(binary[new Rect(0, 0, W, thickness)]);
            int bottom = Cv2.CountNonZero(binary[new Rect(0, H - thickness, W, thickness)]);
            int left = Cv2.CountNonZero(binary[new Rect(0, 0, thickness, H)]);
            int right = Cv2.CountNonZero(binary[new Rect(W - thickness, 0, thickness, H)]);
            // 딕셔너리 저장
            var edgeCounts = new Dictionary<string, int>
    {
        {"TOP", top},
        {"LEFT", left},
        {"BOTTOM", bottom},
        {"RIGHT", right}
    };

            // 내림차순 정렬
            var sorted = edgeCounts.OrderByDescending(kv => kv.Value).ToList();

            // 상위 2개 이름 추출
            var top2 = sorted.Take(2).Select(kv => kv.Key).ToArray();

            // 순서쌍과 인덱스 정의
            string[][] pairs = new string[][]
            {
                new string[]{ "TOP", "LEFT" },
                new string[]{ "LEFT", "BOTTOM" },
                new string[]{ "BOTTOM", "RIGHT" },
                new string[]{ "RIGHT", "TOP" }
            };

            // 상위 2개 쌍이 어떤 인덱스에 속하는지 확인
            for (int i = 0; i < pairs.Length; i++)
            {
                if ((top2.Contains(pairs[i][0]) && top2.Contains(pairs[i][1])))
                    return i;
            }


            return -1;
        }
        public static Mat NewConvertMatrix(Mat src, int plcIndex, int rotate)
        {
            try
            {
                // 블러로 노이즈 제거
                Mat origin_mat = src.Clone();
                switch (rotate)
                {
                    case 0: break;
                    case 1: origin_mat = origin_mat.Flip(FlipMode.X); break;
                    case 2: origin_mat = origin_mat.Flip(FlipMode.XY); break;
                    case 3: origin_mat = origin_mat.Flip(FlipMode.Y); break;
                }
                Mat image = new Mat();
                if (origin_mat.Channels() > 1)
                    Cv2.CvtColor(origin_mat, image, ColorConversionCodes.BGR2GRAY);
                else
                    image = origin_mat.Clone();
                Mat blur = new Mat();

                Cv2.GaussianBlur(image, blur, new Size(5, 5), 0);

                // Otsu 이진화
                Mat binary = new Mat();
                if (plcIndex == 0) Cv2.Threshold(blur, binary, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
                else
                {
                    int iMeanPeak = 0;
                    mapPeakInfo = new SortedDictionary<float, int>();
                    FindHistMeanPeak(blur, out iMeanPeak);
                    Cv2.Threshold(blur, binary, iMeanPeak, 255, ThresholdTypes.Binary);
                }

                // 반전 (점들을 흰색으로)
                //Bitmap check3 = BitmapConverter.ToBitmap(binary);
                //string filename21 = "C:Users\\admin\\Desktop\\양산\\test\\원본이미지.png";
                //check3.Save(filename21);
                // === Matrix Code 사이즈 (예: 14x14) ===

                // 세로 방향 흰 픽셀 개수 합
                Mat verticalSum = new Mat();
                Cv2.Reduce(binary, verticalSum, ReduceDimension.Row, ReduceTypes.Sum, MatType.CV_32S);
                int[] colSums = new int[verticalSum.Cols];
                verticalSum.GetArray(0, 0, colSums);

                // 가로 방향 흰 픽셀 개수 합
                Mat horizontalSum = new Mat();
                Cv2.Reduce(binary, horizontalSum, ReduceDimension.Column, ReduceTypes.Sum, MatType.CV_32S);
                int[] rowSums = new int[horizontalSum.Rows];
                horizontalSum.GetArray(0, 0, rowSums);

                // 봉우리 검출 (연속된 값이 기준 이상인 부분을 카운트)
                int gridCols = CountPeaks(colSums);
                int gridRows = CountPeaks(rowSums);
                int h = binary.Rows;
                int w = binary.Cols;
                double cellH = 0;
                double cellW = 0;
                cellH = Convert.ToInt32(h / gridRows);
                cellW = Convert.ToInt32(w / gridCols);

                // 체스판 배열 생성
                int[,] chessboard = new int[gridRows, gridCols];
                Point2f Edge = FindTopLeftWhiteCenter(binary);
                Point AlignOffset = new Point(Edge.X - cellW / 2, Edge.Y - cellH / 2);
                if (AlignOffset.X < 0 || AlignOffset.Y < 0) return null;
                //int count = 0;
                for (int i = 0; i < gridRows; i++)
                {
                    for (int j = 0; j < gridCols; j++)
                    {
                        binary = binary.Resize(new Size(cellW * gridCols + AlignOffset.X * 2, cellH * gridRows + AlignOffset.Y * 2));
                        Rect roi = new Rect();
                        // 셀 영역 추출
                        if (j * cellW + AlignOffset.X + cellW <= image.Width && i * cellH + AlignOffset.Y + cellH <= image.Height)
                            roi = new Rect((int)(j * cellW) + AlignOffset.X, (int)(i * cellH) + AlignOffset.Y, (int)cellW, (int)cellH);
                        else
                        {
                            int Width = (int)cellW; int Height = (int)cellH;
                            if (j * cellW + AlignOffset.X + cellW > image.Width)
                                Width = image.Width - ((int)(j * cellW) + AlignOffset.X);
                            if (i * cellH + AlignOffset.Y + cellH > image.Height)
                                Height = image.Height - ((int)(i * cellH) + AlignOffset.Y);
                            roi = new Rect((int)(j * cellW) + AlignOffset.X, (int)(i * cellH) + AlignOffset.Y, Width, Height);
                        }
                        Mat cell = new Mat(binary, roi);
                        Bitmap fdfd1 = BitmapConverter.ToBitmap(cell);

                        //string filename = "C:Users\\admin\\Desktop\\양산\\test\\count" + count+ ".png";
                        //fdfd1.Save(filename);
                        //count++;
                        // 흰색 픽셀 비율 계산
                        double whiteRatio = Cv2.CountNonZero(cell) / (double)(cell.Total());

                        // 기준값 0.07 이상이면 1, 아니면 0
                        chessboard[i, j] = (whiteRatio > 0.07) ? 1 : 0;
                    }
                }

                // 체스판 결과 출력 (콘솔)
                //for (int i = 0; i < gridRows; i++)
                //{
                //    for (int j = 0; j < gridCols; j++)
                //    {
                //        Console.Write(chessboard[i, j] + " ");
                //    }
                //    Console.WriteLine();
                //}

                // 결과 이미지 시각화
                Mat vis = new Mat(gridRows, gridCols, MatType.CV_8UC1);
                for (int i = 0; i < gridRows; i++)
                {
                    for (int j = 0; j < gridCols; j++)
                    {
                        vis.Set(i, j, (byte)(chessboard[i, j] * 255));
                    }
                }
                Cv2.Resize(vis, vis, new Size(300, 300), 0, 0, InterpolationFlags.Nearest);
                int Rect_padding = gridRows;
                Mat gridPaddMat = new Mat();
                Cv2.CopyMakeBorder(vis, gridPaddMat, Rect_padding, Rect_padding, Rect_padding, Rect_padding, BorderTypes.Constant, Scalar.All(0));
                //Bitmap fdfd = BitmapConverter.ToBitmap(gridPaddMat);
                switch (rotate)
                {
                    case 0: break;
                    case 1: gridPaddMat = gridPaddMat.Flip(FlipMode.X); break;
                    case 2: gridPaddMat = gridPaddMat.Flip(FlipMode.XY); break;
                    case 3: gridPaddMat = gridPaddMat.Flip(FlipMode.Y); break;
                }
                return gridPaddMat;
            }
            catch (Exception e)
            { return null; }
        }
        public static Point2f FindTopLeftWhiteCenter(Mat src)
        {
            int H = src.Rows;
            int W = src.Cols;

            // 좌상단 20% 영역 계산
            int roiWidth = (int)(W * 0.2);
            int roiHeight = (int)(H * 0.2);
            Rect roiRect = new Rect(0, 0, roiWidth, roiHeight);

            // 흑백 변환
            Mat gray = new Mat();
            if (src.Channels() > 1)
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            else
                gray = src.Clone();

            // 이진화 (Otsu)
            Mat binary = new Mat();
            Cv2.Threshold(gray, binary, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

            // 좌상단 ROI 추출
            Mat roi = new Mat(binary, roiRect);

            // 컨투어 찾기
            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(roi, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            if (contours.Length == 0)
                return new Point2f(0, 0); // 원 없음


            // 좌상단 기준 가장 가까운 원 찾기
            Point topLeftPixel = new Point(int.MaxValue, int.MaxValue);
            int selectedContourIndex = -1;
            int minSum = int.MaxValue;
            for (int i = 0; i < contours.Length; i++)
            {
                foreach (var p in contours[i])
                {
                    int sum = p.X + p.Y;
                    if (sum < minSum)
                    {
                        minSum = sum;
                        topLeftPixel = p;
                        selectedContourIndex = i;
                    }
                }
            }

            var contour = contours[selectedContourIndex];

            // 모멘트 계산
            Moments m = Cv2.Moments(contour);

            // 면적이 0인 경우 예외 처리
            if (m.M00 == 0)
                return new Point2f(-1, -1);

            // 중심 좌표 계산
            float cx = (float)(m.M10 / m.M00);
            float cy = (float)(m.M01 / m.M00);

            // ROI 기준 → 원본 영상 좌표로 변환 필요 시
            cx += roiRect.X;
            cy += roiRect.Y;

            Point2f center = new Point2f(cx, cy);
            return center;
        }
        static int CountPeaks(int[] sums, int thresholdRatio = 30)
        {
            int maxVal = sums.Max();
            int threshold = maxVal / thresholdRatio; // 전체 최대치의 1/30 이상이면 유효 피크로 간주
            int count = 0;
            bool inPeak = false;

            foreach (int v in sums)
            {
                if (v > threshold)
                {
                    if (!inPeak)
                    {
                        count++;
                        inPeak = true;
                    }
                }
                else
                {
                    inPeak = false;
                }
            }
            return count;
        }

        public static Size FindDotSize(Mat MatImage)
        {
            int label;
            int cnt = 0;
            bool bFind;
            bool bReverse = false;
            Mat image = MatImage.Clone();
            Mat image_gray = MatImage.Clone();
            Mat image_bi = new Mat();
            Mat MatRoiImg = new Mat();
            MatImage.CopyTo(image);
            MatImage.CopyTo(image_gray);
            bool AutoSizeChk = true;
            // 라벨 레이어 변수
            Mat img_label = new Mat();
            Mat stats = new Mat();
            Mat centroids = new Mat();
            Mat label_box = new Mat();
            Mat tempImg = new Mat();
            Size DotSize = new Size(0, 0);

            Mat tempmask = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3), new Point(1, 1));

            //OTSU 알고리즘                   
            Cv2.Threshold(image_gray, image_bi, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
            Cv2.Dilate(image_bi, image_bi, tempmask, new Point(-1, -1), 1, BorderTypes.Replicate); //노이즈 제거
            Cv2.Erode(image_bi, image_bi, tempmask, new Point(-1, -1), 1, BorderTypes.Replicate);
            Mat cent = new Mat();
            label = Cv2.ConnectedComponentsWithStats(image_bi, img_label, stats, centroids);

            int iCutSize = 1;

            if (!AutoSizeChk) iCutSize = 0;

            int area, left, top, width, height;

            for (int j = 1; j < label; j++)
            {
                left = stats.At<int>(j, (int)ConnectedComponentsTypes.Left);
                top = stats.At<int>(j, (int)ConnectedComponentsTypes.Top);
                width = stats.At<int>(j, (int)ConnectedComponentsTypes.Width);
                height = stats.At<int>(j, (int)ConnectedComponentsTypes.Height);
                area = stats.At<int>(j, (int)ConnectedComponentsTypes.Area);

                // 찾은 덩어리 중 큰것부터 작은 것 순으로 검색하여. 정사각형과 유사하고, 사이즈가 적당히 큰 것으로 리턴한다. 큰 사각형 테두리를 찾더라도 더 작은 것이 우선
                if (Math.Abs(width - height) < 10 && width > 2 && height > 2 && width < 25 && height < 25)//  && width < 500 && height < 500)
                {
                    // 라벨링 박스
                    if (left < iCutSize || top < iCutSize || image.Cols < left + width + (iCutSize) || image.Rows < top + height + (iCutSize)) continue;

                    Cv2.Rectangle(label_box, new Rect(left - iCutSize, top - iCutSize, width + (iCutSize * 2), height + (iCutSize * 2)), new Scalar(0, 0, 255), 3);

                    Rect roi = new Rect(left - iCutSize, top - iCutSize, width + (iCutSize * 2), height + (iCutSize * 2));
                    MatRoiImg = image_gray.SubMat(roi);
                    Rect biroi = new Rect(left - iCutSize, top - iCutSize, width + (iCutSize * 2), height + (iCutSize * 2));
                    tempImg = image_bi.SubMat(biroi);

                    int Size = 0;
                    FindCountour(tempImg, ref Size);
                    DotSize = roi.Size;
                    return DotSize;
                }
            }
            return DotSize;
        }
        public static bool OriginMake(Mat tempByImg, ref Size DotSize)
        {
            Mat matRoiImgGrayRoi;
            Mat ByImg = new Mat();
            tempByImg.CopyTo(ByImg);
            int iRisingCnt = 0;
            int risingPos = 0, fallingPos = 0;
            byte ucPixel, ucPixelBack = 0;
            byte pixelData;
            int borderLength = (int)(ByImg.Rows / 100.0 + 0.5);
            int linePos = -999;
            double edgePer = 0.8;
            bool width = false, height = false;
            DotSize = FindDotSize(ByImg.Clone());

            if (DotSize.Width == 0 || DotSize.Height == 0) return false;
            int offset = 0;
            DotSize.Width += offset;
            DotSize.Height += offset;
            // Top
            for (int i = 0; i < ByImg.Rows / 10; i++)
            {
                risingPos = fallingPos = 0;
                matRoiImgGrayRoi = new Mat(ByImg, new Rect(0, i, ByImg.Cols, 1));
                iRisingCnt = 0;
                for (int j = 0; j < ByImg.Cols; j++)
                {
                    ucPixel = matRoiImgGrayRoi.At<byte>(0, j);
                    if (i == 0 && j == 0)
                        ucPixelBack = ucPixel;
                    if (ucPixel != ucPixelBack)
                    {
                        if (ucPixel > ucPixelBack)
                        {
                            iRisingCnt++;
                            risingPos = j;
                        }
                        else
                            fallingPos = j;
                    }
                    ucPixelBack = ucPixel;
                    //if (iRisingCnt > 2)
                    //    break;
                }
                if ((ByImg.Cols / DotSize.Width / 2) < iRisingCnt)
                //if (Math.Abs(fallingPos - risingPos) > ByImg.Cols * edgePer)
                {
                    if (linePos == -999)
                        linePos = i;
                    else
                    {
                        if (Math.Abs(linePos - i) < 1) break;
                        else linePos = i;
                    }
                    pixelData = fallingPos > risingPos ? (byte)255 : (byte)0;
                    Cv2.Line(ByImg, new Point(borderLength, i), new Point(ByImg.Cols - 1 - borderLength, i), pixelData, 1, LineTypes.Link8);
                    width = true;
                }
            }
            ucPixelBack = 0;
            linePos = -999;
            // Bottom
            for (int i = ByImg.Rows - 2; i > ByImg.Rows * 9 / 10; i--)
            {
                iRisingCnt = 0;
                risingPos = fallingPos = 0;
                matRoiImgGrayRoi = new Mat(ByImg, new Rect(0, i, ByImg.Cols, 1));
                for (int j = 0; j < ByImg.Cols; j++)
                {
                    ucPixel = matRoiImgGrayRoi.At<byte>(0, j);
                    if (i == 0 && j == 0)
                        ucPixelBack = ucPixel;
                    if (ucPixel != ucPixelBack)
                    {
                        if (ucPixel > ucPixelBack)
                        {
                            iRisingCnt++;
                            risingPos = j;
                        }
                        else
                            fallingPos = j;
                    }
                    ucPixelBack = ucPixel;
                }
                if ((ByImg.Cols / DotSize.Width / 2) < iRisingCnt)
                //if (Math.Abs(fallingPos - risingPos) > ByImg.Cols * edgePer)
                {
                    if (linePos == -999)
                        linePos = i;
                    else
                    {
                        if (Math.Abs(linePos - i) != 1) break;
                        else linePos = i;
                    }
                    pixelData = fallingPos > risingPos ? (byte)255 : (byte)0;
                    Cv2.Line(ByImg, new Point(borderLength, i), new Point(ByImg.Cols - 1 - borderLength, i), pixelData, 1, LineTypes.Link8);
                    width = true;
                }
            }

            linePos = -999;
            ucPixelBack = 0;
            // Left
            for (int i = 0; i < ByImg.Cols / 10; i++)
            {
                iRisingCnt = 0;
                risingPos = fallingPos = 0;
                matRoiImgGrayRoi = new Mat(ByImg, new Rect(i, 0, 1, ByImg.Rows));
                for (int j = 0; j < ByImg.Rows; j++)
                {
                    ucPixel = matRoiImgGrayRoi.At<byte>(j, 0);
                    if (i == 0 && j == 0)
                        ucPixelBack = ucPixel;
                    if (ucPixel != ucPixelBack)
                    {
                        if (ucPixel > ucPixelBack)
                        {
                            iRisingCnt++;
                            risingPos = j;
                        }
                        else
                            fallingPos = j;
                    }
                    ucPixelBack = ucPixel;
                    //if (iRisingCnt > 2)
                    //    break;
                }
                if ((ByImg.Rows / DotSize.Height / 2) < iRisingCnt)
                //if ( Math.Abs(fallingPos - risingPos) > ByImg.Rows * edgePer)
                {
                    if (linePos == -999)
                        linePos = i;
                    else
                    {
                        if (Math.Abs(linePos - i) != 1) break;
                        else linePos = i;
                    }
                    pixelData = fallingPos > risingPos ? (byte)255 : (byte)0;
                    Cv2.Line(ByImg, new Point(i, borderLength), new Point(i, ByImg.Rows - 1 - borderLength), pixelData, 1, LineTypes.Link8);
                    height = true;
                }
            }

            linePos = -999;
            ucPixelBack = 0;

            // Right
            for (int i = ByImg.Cols - 2; i > ByImg.Cols * 9 / 10; i--)
            {
                iRisingCnt = 0;
                risingPos = fallingPos = 0;
                matRoiImgGrayRoi = new Mat(ByImg, new Rect(i, 0, 1, ByImg.Rows));
                for (int j = 0; j < ByImg.Rows; j++)
                {
                    ucPixel = matRoiImgGrayRoi.At<byte>(j, 0);
                    if (i == 0 && j == 0)
                        ucPixelBack = ucPixel;
                    if (ucPixel != ucPixelBack)
                    {
                        if (ucPixel > ucPixelBack)
                        {
                            iRisingCnt++;
                            risingPos = j;
                        }
                        else
                            fallingPos = j;
                    }
                    ucPixelBack = ucPixel;
                    //if (iRisingCnt > 2)
                    //    break;
                }
                if ((ByImg.Rows / DotSize.Height / 2) < iRisingCnt)
                //if ( Math.Abs(fallingPos - risingPos) > ByImg.Rows * edgePer)
                {
                    if (linePos == -999)
                        linePos = i;
                    else
                    {
                        if (Math.Abs(linePos - i) != 1) break;
                        else linePos = i;
                    }
                    pixelData = fallingPos > risingPos ? (byte)255 : (byte)0;
                    Cv2.Line(ByImg, new Point(i, borderLength), new Point(i, ByImg.Rows - 1 - borderLength), pixelData, 1, LineTypes.Link8);
                    height = true;
                }
            }
            return height && width;
        }

        public static bool FindHistMeanPeak(Mat img, out int iMeanPeak, int repeatcnt = 0, int iSensitive = 7)
        {
            iMeanPeak = 0;

            // 히스토그램 피크 구하기
            if (img.Empty())
                return false;

            int[] iFisrtSecondPeak = new int[2];

            if (repeatcnt != 0)
            {
                var keys = mapPeakInfo.Keys.Reverse().ToList();

                if (keys.Count <= repeatcnt + 1)
                    return false;

                iFisrtSecondPeak[0] = mapPeakInfo[keys[0]];
                iFisrtSecondPeak[1] = mapPeakInfo[keys[repeatcnt + 1]];

                iMeanPeak = (iFisrtSecondPeak[0] + iFisrtSecondPeak[1]) / 2;
            }
            else
            {
                Size szHistImg = new Size(512 * 2, 300 * 2);
                double textScale = 1.0;
                Size divNum = new Size(10, 10);
                int guideLineClr = 150;
                int backClr = 1;
                bool bFill = false;
                mapPeakInfo.Clear();

                // 히스토그램 계산
                int histSize = 256;
                Rangef histRange = new Rangef(0, histSize);
                Mat b_hist = new Mat();

                Cv2.CalcHist(new Mat[] { img }, new int[] { 0 }, null, b_hist, 1, new int[] { histSize }, new Rangef[] { histRange }, true, false);

                // 히스토그램 정규화
                Mat selectedHist = b_hist;

                double maxVal = 0, minVal = 0;
                Cv2.MinMaxLoc(selectedHist, out minVal, out maxVal);

                // presetting
                Scalar bgClr = new Scalar(255, 255, 255);
                Scalar bkClr = new Scalar(0, 0, 0);
                Scalar grClr = new Scalar(guideLineClr, guideLineClr, guideLineClr);
                if (backClr == 1)
                {
                    bkClr = new Scalar(255, 255, 255);
                    bgClr = new Scalar(0, 0, 0);
                }

                int temp = 0;
                Size szTextHor = Cv2.GetTextSize("000", HersheyFonts.HersheyPlain, textScale, 1, out temp);
                Size szTextMaxVal = Cv2.GetTextSize(((int)maxVal).ToString(), HersheyFonts.HersheyPlain, textScale, 1, out temp);

                int marginText = 15;
                int marginBtm = szTextHor.Height + marginText * 2;
                int marginRig = 30;
                int marginLef = szTextMaxVal.Width + marginText * 2;
                int marginTop = 30;

                int hist_w = szHistImg.Width;
                int hist_h = szHistImg.Height;

                float bin_w = (float)(hist_w - marginLef - marginRig) / (float)histSize;

                Mat MaskedHist = new Mat();

                Mat histImage = new Mat(hist_h, hist_w, MatType.CV_8UC3, bgClr);

                // Draw frame
                Cv2.ArrowedLine(histImage, new Point(marginLef, hist_h - marginBtm), new Point(hist_w - marginRig * 0.5, hist_h - marginBtm), bkClr, 2, LineTypes.Link8, 0, 0.008);   // bottom line
                Cv2.ArrowedLine(histImage, new Point(marginLef, hist_h - marginBtm), new Point(marginLef, marginTop * 0.5), bkClr, 2, LineTypes.Link8, 0, 0.015); // left

                int repValNumHori = divNum.Width;
                int interHori = Convert.ToInt32((double)((hist_w - marginLef - marginRig) / repValNumHori));
                float interValHori = (float)(histSize - 1) / (float)repValNumHori;
                for (int ii = 0; ii < repValNumHori + 1; ii++)
                {
                    string textHori = ((int)(interValHori * ii)).ToString();
                    Size szTextHori = Cv2.GetTextSize(textHori, HersheyFonts.HersheyPlain, textScale, 1, out temp);
                    Cv2.PutText(histImage, textHori, new Point(marginLef + interHori * ii - szTextHori.Width * 0.5, hist_h - marginBtm + szTextHor.Height + marginText), HersheyFonts.HersheyPlain, textScale, bkClr, 1, LineTypes.Link8, false);
                }

                // Draw vertical value
                int repValNumVert = divNum.Height;
                int interVert = Convert.ToInt32((hist_h - marginTop - marginBtm) / repValNumVert);
                float interValVert = (float)(maxVal) / (float)repValNumVert;
                for (int ii = 0; ii < repValNumVert; ii++)
                {
                    string textVer = ((int)(interValVert * (repValNumVert - ii))).ToString();
                    Size szTextVer = Cv2.GetTextSize(textVer, HersheyFonts.HersheyPlain, textScale, 1, out temp);
                    int rightAlig = szTextMaxVal.Width - szTextVer.Width;
                    Cv2.PutText(histImage, textVer, new Point(marginLef - szTextMaxVal.Width + rightAlig - marginText, marginTop + interVert * ii + szTextVer.Height * 0.5), HersheyFonts.HersheyPlain, textScale, bkClr, 1, LineTypes.Link8, false);
                }

                // Draw horizontal guide line
                for (int ii = 1; ii <= repValNumHori; ii++)
                {
                    Cv2.Line(histImage, Convert.ToInt32(marginLef + interHori * ii), Convert.ToInt32(marginTop * 0.5), Convert.ToInt32(marginLef + interHori * ii), Convert.ToInt32(hist_h - marginBtm), grClr, 1, LineTypes.Link8, 0);
                    Cv2.Line(histImage, Convert.ToInt32(marginLef + interHori * ii), Convert.ToInt32(hist_h - marginBtm), Convert.ToInt32(marginLef + interHori * ii), Convert.ToInt32(hist_h - marginBtm + 10), bkClr, 1, LineTypes.Link8, 0);
                }

                // Draw vertical guide line
                for (int ii = 0; ii < repValNumVert; ii++)
                {
                    Cv2.Line(histImage, Convert.ToInt32(marginLef - 10), Convert.ToInt32(marginTop + interVert * ii), Convert.ToInt32(hist_w - marginRig), Convert.ToInt32(marginTop + interVert * ii), grClr, 1, LineTypes.Link8, 0);
                    Cv2.Line(histImage, Convert.ToInt32(marginLef - 10), Convert.ToInt32(marginTop + interVert * ii), Convert.ToInt32(marginLef), Convert.ToInt32(marginTop + interVert * ii), bkClr, 1, LineTypes.Link8, 0);
                }
                /// Normalize the result to [ 0, histImage.rows-margin of top & bottom ]
                Cv2.Normalize(selectedHist, selectedHist, 0, histImage.Rows - marginTop - marginBtm, NormTypes.MinMax, -1);
                selectedHist.CopyTo(MaskedHist);
                //blur(selectedHist, MaskedHist, Size(1,20));
                //cv::medianBlur(selectedHist, MaskedHist, 3);
                Cv2.GaussianBlur(selectedHist, MaskedHist, new Size(iSensitive, iSensitive), 0);
                /// Draw for each channel
                if (!bFill)
                {
                    for (int i = 1; i < histSize; i++)
                    {
                        Cv2.Line(histImage, Convert.ToInt32(marginLef + (bin_w * (i - 1))), Convert.ToInt32(hist_h - marginBtm - MaskedHist.At<float>(i - 1)), Convert.ToInt32(marginLef + (bin_w * (i))), Convert.ToInt32((hist_h - marginBtm - MaskedHist.At<float>(i))), bkClr, 2, LineTypes.Link8, 0);
                    }
                }

                else if (bFill)
                {
                    for (int i = 1; i < histSize; i++)
                    {
                        Point prePtR = new Point(Convert.ToInt32(marginLef + (bin_w * (i - 1))), Convert.ToInt32(hist_h - marginBtm));
                        Point postPtR = new Point(Convert.ToInt32(marginLef + (bin_w * (i))), Convert.ToInt32(hist_h - marginBtm - MaskedHist.At<float>(i)));
                        Cv2.Rectangle(histImage, prePtR, postPtR, bkClr, -1, LineTypes.Link8, 0);
                    }
                }
                // 피크 찾기
                bool bPlus = false, bPlusBack = true;
                for (int i = 1; i < histSize; i++)
                {
                    float iDiff = MaskedHist.At<float>(i) - MaskedHist.At<float>(i - 1);
                    if (iDiff > 0)
                    {
                        bPlus = true;
                    }
                    else
                    {
                        bPlus = false;
                    }

                    if (bPlusBack && !bPlus)
                    {
                        mapPeakInfo[MaskedHist.At<float>(i - 1)] = i - 1;
                    }

                    bPlusBack = bPlus;
                }

                mapPeakInfo[MaskedHist.At<float>(255)] = 255;
                mapPeakInfo[MaskedHist.At<float>(0)] = 0;

                // 첫 번째, 두 번째 피크값 추출
                var keys = new List<float>(mapPeakInfo.Keys);
                if (keys.Count >= 2)
                {
                    float filstKey = keys[keys.Count - 1];
                    float secondKey = keys[keys.Count - 2];
                    iFisrtSecondPeak[0] = mapPeakInfo[filstKey];
                    iFisrtSecondPeak[1] = mapPeakInfo[secondKey];
                }
                iMeanPeak = (iFisrtSecondPeak[0] + iFisrtSecondPeak[1]) / 2;
            }

            return true;
        }

        public static bool FindMCRCnt(Mat img, Size DotSize, ref int MatrixCnt, int iType)
        {
            Mat matTmp = new Mat();
            Mat src = new Mat();
            img.CopyTo(src);
            if (src.Channels() != 1)
            {
                Cv2.CvtColor(src, src, ColorConversionCodes.BGR2GRAY);
            }
            if (iType == 1)
            {
                Mat matrix;
                int Width = src.Width, Height = src.Height;

                if (src.Width % 2 != 0)
                    Width += 1;
                if (src.Height % 2 != 0)
                    Height += 1;
                Point2f Center = new Point2f(Width / 2, Height / 2);
                matrix = Cv2.GetRotationMatrix2D(Center, 90, 1);

                Cv2.WarpAffine(src, matTmp, matrix, new OpenCvSharp.Size(src.Height, src.Width), InterpolationFlags.Linear);
            }
            else src.CopyTo(matTmp);

            int[] iIndex = new int[2];
            int iRowSize = (int)(DotSize.Height * 1.2);//(int)((matTmp.Rows / 12.0) + 0.5);
            byte ucPixel = 0, ucPixel_back = 0;
            int iRisingCnt = 0, iFallingCnt = 0;
            List<int> ListRisingCnt = new List<int>();
            List<int> ListFallingCnt = new List<int>();
            bool bDotPos;

            List<SortedSet<int>> mapEdgeSearchKey = new List<SortedSet<int>>();
            SortedTuple<int, int>[] mapEdgeSearch = new SortedTuple<int, int>[2];
            SortedTuple<int, int>[] mapRisingPos = new SortedTuple<int, int>[2];
            SortedTuple<int, int>[] mapFallingPos = new SortedTuple<int, int>[2];

            for (int i = 0; i < 2; i++)
            {
                mapEdgeSearchKey.Add(new SortedSet<int>());
                mapEdgeSearch[i] = new SortedTuple<int, int>();
                mapRisingPos[i] = new SortedTuple<int, int>();
                mapFallingPos[i] = new SortedTuple<int, int>();
            }

            if (ListRisingCnt != null) ListRisingCnt.Clear();

            //int pos = 0; // 상하좌우
            int key = 0, MaxKeyCnt = 0, map_value, map_value_old;
            int[] MaxKey = new int[2];
            int repeatcnt = 0, iFindCutPos = -1; // 반복횟수
            int[] repeatTar = new int[2];
            int mcr_version = 0; // N x N

            int[] iEdgePointCnt;
            int[] sortint;
            var Sortedtuples = new SortedTuple<int, int>();
            try
            {
                for (int updown = 0; updown < 2; updown++)
                {
                    if (updown == 1) { Cv2.Flip(matTmp, matTmp, FlipMode.XY); }

                    key = 0;
                    MaxKeyCnt = 0;
                    MaxKey[updown] = 0;
                    map_value = 0;
                    map_value_old = -9999;
                    repeatcnt = 0;
                    iFindCutPos = -1;
                    repeatTar[updown] = 0;
                    for (int i = 0; i < iRowSize; i++)
                    {
                        iRisingCnt = 0;
                        iFallingCnt = 0;

                        Mat matroiimg_gray_roi = matTmp.RowRange(i, i + 1);
                        for (int j = 0; j < matTmp.Cols; j++)
                        {
                            ucPixel = matroiimg_gray_roi.At<byte>(0, j);
                            if (i == 0 && j == 0)
                                ucPixel_back = ucPixel;

                            if (ucPixel != ucPixel_back)
                            {
                                if (ucPixel > ucPixel_back)
                                {
                                    iRisingCnt++;
                                    mapRisingPos[updown].Add(i, j);
                                }
                                else
                                {
                                    iFallingCnt++;
                                    mapFallingPos[updown].Add(i, j);
                                }
                            }
                            ucPixel_back = ucPixel;
                        }

                        if (iRisingCnt != 0)
                        {
                            if (iRisingCnt == 1 || iRisingCnt >= 5)
                            {
                                mapEdgeSearchKey[updown].Add(iRisingCnt);
                                mapEdgeSearch[updown].Add(iRisingCnt, i);
                            }
                        }
                    }

                    MaxKeyCnt = 0;

                    foreach (var keyEntry in mapEdgeSearchKey[updown])
                    {
                        key = keyEntry;
                        int count = mapEdgeSearch[updown].ContainsKey(key) ? mapEdgeSearch[updown].EqualKeyCount(key) : 0;

                        if (MaxKeyCnt < count)
                        {
                            MaxKey[updown] = key;
                            MaxKeyCnt = count;
                        }
                    }
                    if (MaxKeyCnt * MaxKeyCnt / iRowSize + 0.5 > 1.0)
                        repeatTar[updown] = (int)((double)(MaxKeyCnt * MaxKeyCnt / iRowSize) + 0.5);
                    else repeatTar[updown] = 1;
                    repeatTar[updown] = Math.Max(repeatTar[updown], 1);
                    if (repeatTar[updown] > Math.Sqrt(iRowSize))
                    {
                        repeatTar[updown] = Convert.ToInt32(Math.Max(Math.Sqrt(iRowSize), 1));
                    }

                    foreach (var item in mapEdgeSearch[updown])
                    {
                        if (item.Item1 < MaxKey[updown]) continue;
                        if (item.Item1 > MaxKey[updown]) break;

                        map_value = item.Item2; // MaxKey의 value값

                        if (Math.Abs(map_value - map_value_old) == 1)//연속된다면
                            repeatcnt++;
                        else
                            repeatcnt = 0;

                        if (repeatcnt >= repeatTar[updown])
                        {
                            iFindCutPos = map_value - repeatcnt;
                            break;
                        }
                        map_value_old = map_value;
                    }

                    if (iFindCutPos == -1)
                    {
                        MaxKeyCnt = 0;
                        mapEdgeSearchKey[updown].Remove(MaxKey[updown]);
                        foreach (var keyEntry in mapEdgeSearchKey[updown])
                        {
                            key = keyEntry;
                            int count = mapEdgeSearch[updown].ContainsKey(key) ? mapEdgeSearch[updown].EqualKeyCount(key) : 0;

                            if (MaxKeyCnt < count)
                            {
                                MaxKey[updown] = key;
                                MaxKeyCnt = count;
                            }
                        }

                        if (MaxKeyCnt * MaxKeyCnt / iRowSize + 0.5 > 1.0)
                            repeatTar[updown] = (int)((double)(MaxKeyCnt * MaxKeyCnt / iRowSize) + 0.5);
                        else repeatTar[updown] = 1;
                        repeatTar[updown] = Convert.ToInt32(Math.Max(repeatTar[updown], 1));
                        map_value = 0; map_value_old = -9999;
                        if (repeatTar[updown] > Math.Sqrt(iRowSize))
                        {
                            repeatTar[updown] = Convert.ToInt32(Math.Max(Math.Sqrt(iRowSize), 1));
                        }
                        foreach (var item in mapEdgeSearch[updown])
                        {
                            if (item.Item1 < MaxKey[updown]) continue;
                            if (item.Item1 > MaxKey[updown]) break;

                            map_value = item.Item2; // MaxKey의 value값

                            if (Math.Abs(map_value - map_value_old) == 1)//연속된다면
                                repeatcnt++;
                            else
                                repeatcnt = 0;

                            if (repeatcnt >= repeatTar[updown])
                            {
                                iFindCutPos = map_value - repeatcnt;
                                break;
                            }
                            map_value_old = map_value;
                        }

                        if (iFindCutPos == -1)
                        {
                            iIndex[updown] = 0;
                            return false;
                        }
                    }

                    iIndex[updown] = iFindCutPos;

                    if (mcr_version < MaxKey[updown])
                    {
                        mcr_version = MaxKey[updown];
                    }

                    if (updown == 1)
                    {
                        if (!mapFallingPos[0].Any() || !mapFallingPos[1].Any() || !mapRisingPos[1].Any() || !mapRisingPos[0].Any())
                        {
                            return false;
                        }

                        for (int n = 0; n < 2; n++)
                        {
                            if (MaxKey[n] == 1)
                            {
                                mcr_version = MaxKey[1 - n];
                                break;
                                /*
                                bool b1 = mapRisingPos[n].ContainsKey(iIndex[n] + (int)repeatTar[n]);
                                bool b2 = mapFallingPos[n].ContainsKey(iIndex[n] + (int)repeatTar[n]);

                                if (!(b1 && b2))
                                {
                                    return false;
                                }

                                int MaxLineSize1 = 0;
                                foreach (var item in mapEdgeSearch[n])
                                {
                                    if (item.Item1 < MaxKey[n]) continue;
                                    if (item.Item1 > MaxKey[n]) break;

                                    if (mapRisingPos[n].FindValue(item.Item1) == mapRisingPos[n].End() || mapFallingPos[n].FindValue(item.Item1) == mapFallingPos[n].End() ||
                                        mapRisingPos[n].FindValue(item.Item1) == null || mapFallingPos[n].FindValue(item.Item1) == null)
                                        continue;

                                    Tuple<int, int> RisingTemp = mapRisingPos[n].FindValue(item.Item2);
                                    Tuple<int, int> FallingTemp = mapFallingPos[n].FindValue(item.Item2);

                                    int dist0 = Math.Abs(RisingTemp.Item2 - FallingTemp.Item2);
                                    MaxLineSize1 = Math.Max(MaxLineSize1, dist0);
                                }

                                if (MaxLineSize1 < (matTmp.Cols / 2))
                                {
                                    mapEdgeSearchKey[n].Remove(1);
                                    MaxKeyCnt = 0;
                                    mapEdgeSearchKey[n].Remove(MaxKey[n]);
                                    foreach (var keyEntry in mapEdgeSearchKey[n])
                                    {
                                        key = keyEntry;
                                        int count = mapEdgeSearch[n].ContainsKey(key) ? mapEdgeSearch[n].EqualKeyCount(key) : 0;

                                        if (MaxKeyCnt < count)
                                        {
                                            MaxKey[n] = key;
                                            MaxKeyCnt = count;
                                        }
                                    }

                                    if (MaxKeyCnt * MaxKeyCnt / iRowSize + 0.5 > 1.0)
                                        repeatTar[n] = (int)((double)(MaxKeyCnt * MaxKeyCnt / iRowSize) + 0.5);
                                    else repeatTar[n] = 1;
                                    repeatTar[n] = Convert.ToInt32(Math.Max(repeatTar[n], 1));
                                    map_value = 0; map_value_old = -9999;
                                    if (repeatTar[n] > Math.Sqrt(iRowSize))
                                    {
                                        repeatTar[n] = Convert.ToInt32(Math.Max(Math.Sqrt(iRowSize), 1));
                                    }

                                    foreach (var item in mapEdgeSearch[n])
                                    {
                                        if (item.Item1 < MaxKey[n]) continue;
                                        if (item.Item1 > MaxKey[n]) break;

                                        map_value = item.Item2; // MaxKey의 value값

                                        if (Math.Abs(map_value - map_value_old) == 1)//연속된다면
                                            repeatcnt++;
                                        else
                                            repeatcnt = 0;

                                        if (repeatcnt >= repeatTar[n])
                                        {
                                            iFindCutPos = map_value - repeatcnt;
                                            break;
                                        }
                                        map_value_old = map_value;
                                    }

                                    if (iFindCutPos == -1)
                                    {
                                        iIndex[updown] = 0;
                                        return false;
                                    }

                                    iIndex[n] = iFindCutPos;

                                    mcr_version = MaxKey[n];
                                }
                                */
                            }
                        }

                        if (!(MaxKey[0] == 1 || MaxKey[1] == 1))
                        {
                            bool bLineSearch = MaxKey[0] > MaxKey[1] ? true : false;
                            //int MaxLineSize1 = 0, MaxLineSize2 = 0, lineSize = 0;

                            //foreach (var item in mapEdgeSearch[0])
                            //{
                            //    if (item.Item1 < MaxKey[0]) continue;
                            //    if (item.Item1 > MaxKey[0]) break;
                            //
                            //    if (mapRisingPos[0].FindValue(item.Item1) == mapRisingPos[0].End() || mapFallingPos[0].FindValue(item.Item1) == mapFallingPos[0].End() ||
                            //        mapRisingPos[0].FindValue(item.Item1) == null || mapFallingPos[0].FindValue(item.Item1) == null)
                            //        continue;
                            //
                            //    Tuple<int, int> RisingTemp = mapRisingPos[0].FindValue(item.Item1);
                            //    Tuple<int, int> FallingTemp = mapFallingPos[0].FindValue(item.Item1);
                            //
                            //    lineSize = Math.Abs(RisingTemp.Item2 - FallingTemp.Item2);
                            //    MaxLineSize1 = Math.Max(MaxLineSize1, lineSize);
                            //}
                            //
                            //lineSize = 0;
                            //
                            //foreach (var item in mapEdgeSearch[1])
                            //{
                            //    if (item.Item1 < MaxKey[1]) continue;
                            //    if (item.Item1 > MaxKey[1]) break;
                            //
                            //    if (mapRisingPos[1].FindValue(item.Item1) == mapRisingPos[1].End() || mapFallingPos[1].FindValue(item.Item1) == mapFallingPos[1].End()||
                            //        mapRisingPos[1].FindValue(item.Item1) == null || mapFallingPos[1].FindValue(item.Item1) == null)
                            //        continue;
                            //
                            //    Tuple<int, int> RisingTemp = mapRisingPos[1].FindValue(item.Item1);
                            //    Tuple<int, int> FallingTemp = mapFallingPos[1].FindValue(item.Item1);
                            //
                            //    lineSize = Math.Abs(RisingTemp.Item2 - FallingTemp.Item2);
                            //    MaxLineSize2 = Math.Max(MaxLineSize2, lineSize);
                            //}
                            if (MaxKey[0] == MaxKey[1])
                            {
                                bLineSearch = mapEdgeSearch[0].EqualKeyCount(key) < mapEdgeSearch[1].EqualKeyCount(key) ? false : true;
                            }
                            else
                            {
                                bLineSearch = MaxKey[0] > MaxKey[1] ? false : true;
                            }

                            //mapEdgeSearch[Convert.ToInt32(bLineSearch)].Remove(mapEdgeSearch[Convert.ToInt32(bLineSearch)].FindKey(MaxKey[Convert.ToInt32(bLineSearch)]));
                            //MaxKey[Convert.ToInt32(bLineSearch)] = 1;// Max Key
                            MaxKeyCnt = 0;
                            foreach (var keyEntry in mapEdgeSearch[Convert.ToInt32(bLineSearch)])
                            {
                                if (keyEntry.Item1 < MaxKey[Convert.ToInt32(bLineSearch)]) continue;
                                if (keyEntry.Item1 > MaxKey[Convert.ToInt32(bLineSearch)]) break;
                                key = keyEntry.Item1;
                                int count = mapEdgeSearch[Convert.ToInt32(bLineSearch)].ContainsKey(key) ? mapEdgeSearch[Convert.ToInt32(bLineSearch)].EqualKeyCount(key) : 0;

                                if (MaxKeyCnt < count)
                                {
                                    MaxKeyCnt = count;
                                }
                            }
                            //MaxKeyCnt = mapEdgeSearch[Convert.ToInt32(bLineSearch)].ContainsKey(MaxKey[Convert.ToInt32(bLineSearch)]);//.Count();// Max key 갯수

                            //}
                            //}
                            if (MaxKeyCnt == 0)
                                continue;

                            map_value = 0; map_value_old = -9999;
                            if (MaxKeyCnt * MaxKeyCnt / iRowSize + 0.5 > 1.0)
                                repeatTar[Convert.ToInt32(bLineSearch)] = (int)((double)(MaxKeyCnt * MaxKeyCnt / iRowSize) + 0.5);
                            else repeatTar[Convert.ToInt32(bLineSearch)] = 1;
                            if (repeatTar[Convert.ToInt32(bLineSearch)] > Math.Sqrt(iRowSize))
                                repeatTar[Convert.ToInt32(bLineSearch)] = Convert.ToInt32(Math.Sqrt(iRowSize) > 1 ? Math.Sqrt(iRowSize) : 1);

                            foreach (var item in mapEdgeSearch[Convert.ToInt32(bLineSearch)])
                            {
                                if (item.Item1 < MaxKey[Convert.ToInt32(bLineSearch)]) continue;
                                if (item.Item1 > MaxKey[Convert.ToInt32(bLineSearch)]) break;

                                map_value = item.Item2;
                                if (Math.Abs(map_value - map_value_old) == 1)//연속된다면
                                    repeatcnt++;
                                else
                                    repeatcnt = 0;

                                if (repeatcnt >= repeatTar[Convert.ToInt32(bLineSearch)])
                                {
                                    iFindCutPos = map_value - repeatcnt;
                                    break;
                                }
                                map_value_old = map_value;
                            }
                            iIndex[Convert.ToInt32(bLineSearch)] = iFindCutPos;
                            mcr_version = MaxKey[Convert.ToInt32(!bLineSearch)];
                        }
                    }
                }

                if (MatrixCnt == 0)
                {
                    MatrixCnt = mcr_version * 2;
                }
                else
                {
                    if (MatrixCnt != mcr_version * 2)
                    {
                        iFindCutPos = -1;
                        bool bDotSearch = MaxKey[0] > MaxKey[1] ? false : true;

                        MaxKeyCnt = 0;

                        foreach (var keyEntry in mapEdgeSearchKey[Convert.ToInt32(bDotSearch)])
                        {
                            key = keyEntry;
                            int count = mapEdgeSearch[Convert.ToInt32(bDotSearch)].ContainsKey(key) ? mapEdgeSearch[Convert.ToInt32(bDotSearch)].EqualKeyCount(key) : 0;

                            if (MaxKeyCnt < count)
                            {
                                MaxKey[Convert.ToInt32(bDotSearch)] = key;
                                MaxKeyCnt = count;
                            }
                        }

                        if (MaxKeyCnt <= 0)
                            return false;
                        map_value = 0; map_value_old = -9999;
                        if (MaxKeyCnt * MaxKeyCnt / iRowSize > Math.Sqrt(iRowSize) / 2)
                            repeatTar[Convert.ToInt32(bDotSearch)] = (int)(MaxKeyCnt * MaxKeyCnt / iRowSize);
                        else
                            repeatTar[Convert.ToInt32(bDotSearch)] = (int)(Math.Sqrt(iRowSize) / 2);

                        if (Math.Sqrt(iRowSize) >= 1)
                            repeatTar[Convert.ToInt32(bDotSearch)] = (int)Math.Sqrt(iRowSize);
                        else
                            repeatTar[Convert.ToInt32(bDotSearch)] = 1;

                        foreach (var item in mapEdgeSearch[Convert.ToInt32(bDotSearch)])
                        {
                            if (item.Item1 < MaxKey[Convert.ToInt32(bDotSearch)]) continue;
                            if (item.Item1 > MaxKey[Convert.ToInt32(bDotSearch)]) break;

                            map_value = item.Item2;
                            if (Math.Abs(map_value - map_value_old) == 1)//연속된다면
                                repeatcnt++;
                            else
                                repeatcnt = 0;

                            if (repeatcnt >= repeatTar[Convert.ToInt32(bDotSearch)])
                            {
                                iFindCutPos = map_value - repeatcnt;
                                break;
                            }
                            map_value_old = map_value;
                        }
                        if (iFindCutPos == -1)
                            return false;
                        iIndex[Convert.ToInt32(bDotSearch)] = iFindCutPos;

                        mcr_version = MaxKey[Convert.ToInt32(bDotSearch)];
                        if (MatrixCnt != mcr_version * 2)
                            return false;
                    }
                }

                m_bMCROrigin[iType] = (MaxKey[0] > MaxKey[1] ? true : false);

                if (iType == 1)
                {
                    if (!m_bMCROrigin[0])
                    {
                        if (m_bMCROrigin[1])
                            m_iMCROrigin = 4;//	」 
                        else
                            m_iMCROrigin = 3; // ㄴ                                          
                    }
                    else
                    {
                        if (m_bMCROrigin[1])
                            m_iMCROrigin = 2;//	ㄱ 
                        else
                            m_iMCROrigin = 1; //「   
                    }
                }
                if (!(mcr_version >= 5 && !(MaxKey[0] == 1 || MaxKey[1] == 1)))
                    return false;


                if (ListEdgePoints[iType] != null) ListEdgePoints[iType].Clear();
                bDotPos = MaxKey[0] > MaxKey[1] ? true : false; // 점 위치 판단(라인 반대쪽)

                int[][] iFindEdgePoints = new int[repeatTar[Convert.ToInt32(bDotPos)]][];
                iEdgePointCnt = new int[repeatTar[Convert.ToInt32(bDotPos)]];

                for (int i = 0; i < repeatTar[Convert.ToInt32(bDotPos)]; i++)
                {
                    iFindEdgePoints[i] = new int[MatrixCnt + 4];
                    for (int j = 0; j < MatrixCnt + 4; j++)
                        iFindEdgePoints[i][j] = 0;
                }

                for (int i = 0; i < repeatTar[Convert.ToInt32(bDotPos)]; i++)
                {
                    int cnt = 0;
                    foreach (var item in mapRisingPos[Convert.ToInt32(bDotPos)])
                    {
                        if (item.Item1 < iIndex[Convert.ToInt32(bDotPos)] + i) continue;
                        if (item.Item1 > iIndex[Convert.ToInt32(bDotPos)] + i) break;

                        if (cnt >= mcr_version + 2)  //1배
                            break;
                        iFindEdgePoints[i][cnt++] = item.Item2;
                    }
                    foreach (var item in mapFallingPos[Convert.ToInt32(bDotPos)])
                    {
                        if (item.Item1 < iIndex[Convert.ToInt32(bDotPos)] + i) continue;
                        if (item.Item1 > iIndex[Convert.ToInt32(bDotPos)] + i) break;

                        if (cnt >= MatrixCnt + 4) //2배
                            break;
                        iFindEdgePoints[i][cnt++] = item.Item2;
                    }
                    sortint = iFindEdgePoints[i];
                    Array.Sort(sortint, 0, cnt);
                    iEdgePointCnt[i] = cnt;// sortint.Count();
                }

                int avgPos, repeat = 0;
                long sumPos = 0;
                for (int j = 0; j < MatrixCnt + 4; j++)
                {
                    sumPos = 0;
                    repeat = 0;
                    for (int i = 0; i < repeatTar[Convert.ToInt32(bDotPos)]; i++)
                    {
                        if (iEdgePointCnt[i] < j)
                            break;
                        repeat++;
                        sumPos += iFindEdgePoints[i][j];
                    }
                    if (repeat == 0)
                        continue;
                    avgPos = Convert.ToInt32((double)sumPos / (double)repeat);
                    if (avgPos == 0)
                        continue;
                    if (Convert.ToBoolean(iType))
                    {
                        if (bDotPos)
                            ListEdgePoints[iType].Add(avgPos);
                        else
                            ListEdgePoints[iType].Add(matTmp.Cols - avgPos);
                    }
                    else
                    {
                        if (bDotPos)
                            ListEdgePoints[iType].Add(matTmp.Cols - avgPos);
                        else
                            ListEdgePoints[iType].Add(avgPos);
                    }
                }

                ListEdgePoints[iType].Sort();

                //for (int i = 0; i < repeatTar[Convert.ToInt32(bDotPos)]; i++)
                //    delete(iFindEdgePoints[i]);
                //delete(iFindEdgePoints);

            }
            catch
            {
                iIndex = new int[2];
                return false;
            }

            if (MatrixCnt != 0 && MatrixCnt < mcr_version)
            {
                MatrixCnt = mcr_version;
            }

            return true;
        }

        private static void SaveMCRImg(Mat src, string type, int plcIndex, string lot, string recogResult, List<string> id)
        {
            DateTime now = DateTime.Now;
            int year = now.Year;
            int month = now.Month;
            int day = now.Day;
            int hour = now.Hour;
            int minute = now.Minute;
            int second = now.Second;
            string date = $"{year:D2}{month:D2}{day:D2}{hour:D2}{minute:D2}";

            if (MainWindow.curLotData == null)
            {
                lot = "";
            }
            else lot = MainWindow.curLotData.ITS_ORDER + "\\";
            string dir = MainWindow.Setting.General.ResultPath + "\\MCR_TEST\\" + MainWindow.CurrentModel.Name + "\\" + lot;
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (dirInfo.Exists == false)
                Directory.CreateDirectory(dir);
            //string info = "Origin" + lot + " " + plcIndex.ToString("D4") + "_Recog";
            //if (id.Count > 0)
            //{
            //    string[] ids = id[0].Split(' ');
            //    if (ids.Length >= 2)
            //        recogResult += ids[0] + " " + ids[1];
            //
            //    string[] result = info.Split('_');
            //
            //    if (MainWindow.curLotData == null)
            //    {
            //        if (lot == "101111111" || lot == "Teaching")
            //            recogResult += "_[테스트]";
            //        else
            //            recogResult += "_[정상]";
            //    }
            //    else
            //    {
            //        //Origin123456789 0025_Recog123456789 0025_일치
            //        if (lot == "101111111" || lot == "Teaching")
            //            recogResult += "_[테스트]";
            //        else if (MainWindow.curLotData.ITS_ORDER != ids[0])
            //            recogResult += "_[오인식]";
            //        else
            //            recogResult += "_[정상]";
            //    }
            //}
            //else
            //{
            //    recogResult += "_[미인식]";
            //}

            //string dir = MainWindow.Setting.General.ResultPath + "\\" + MainWindow.CurrentGroup.Name + "\\" + MainWindow.CurrentModel.Name + "\\" + lot + "\\2D Mark log\\";

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            Cv2.ImWrite(dir + plcIndex + "_" + recogResult + "_" + date + ".png", src);
        }

        public static void FindCountour(Mat img, ref int iSize)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            Mat gray = new Mat();
            img.CopyTo(gray);

            // 방법 1 : 캐니 에지.    주의 : 에지 처리를 하면 두개의 윤곽선이 나오게 된다.
            Cv2.Canny(gray, gray, 200, 300, 3); //cv::namedWindow("edge Image");	cv::imshow("edge Image", gray);

            Point[][] contours;// 윤곽선 한개는 vector<Point> 로 충분. 한 화면에 윤곽선이 많기 때문에 이들의 벡터 표현으로 윤곽선 집합이 표현된다.
            HierarchyIndex[] hierarchy;

            RNG rng = new RNG(12345);

            Cv2.FindContours(gray,  // Source, an 8-bit single-channel image. Non-zero pixels are treated as 1’s. 
                out contours,       // output. Detected contours. Each contour is stored as a vector of points.
                out hierarchy,

                 //CV_RETR_EXTERNAL,	// retrieves only the extreme outer contours. It sets hierarchy[i][2]=hierarchy[i][3]=-1 for all the contours.
                 RetrievalModes.Tree,       // retrieves all of the contours and reconstructs a full hierarchy of nested contours.
                                            //CV_RETR_CCOMP,		// retrieves all of the contours and organizes them into a two-level hierarchy

                ContourApproximationModes.ApproxSimple // Contour approximation method.  compresses horizontal, vertical, and diagonal segments and leaves only their end points.
              );       // Optional offset by which every contour point is shifted. This is useful if the contours are extracted from the image ROI and then they should be analyzed in the whole image context.

            // Draw contours 렉트 확인용
            iSize = contours.Count();
            // 	Mat drawing = Mat::zeros(gray.size(), CV_8UC3);
            // 	for (int i = 0; i < contours.size(); i++)
            // 	{
            // 		Scalar color = Scalar(rng.uniform(0, 255), rng.uniform(0, 255), rng.uniform(0, 255));
            // 		drawContours(drawing,	// Destination image.
            // 			contours,			// All the input contours. Each contour is stored as a point vector
            // 			i,					// contourIdx – Parameter indicating a contour to draw. If it is negative, all the contours are drawn.
            // 			color,				// color – Color of the contours.
            // 			2,					// thickness – Thickness of lines the contours are drawn with.
            // 			8,					// lineType – Line connectivity. 
            // 			hierarchy,			// hierarchy – Optional information about hierarchy. It is only needed if you want to draw only some of the contours (see maxLevel ).
            // 			0,					// maxLevel – Maximal level for drawn contours. If it is 0, only the specified contour is drawn.
            // 			Point());			// offset – Optional contour shift parameter.
            // 								//imshow("contour window", drawing);
            // 	}
        }

        public static List<string> Decode(Bitmap srcBitmap, string lot, string type, int plcIndex)
        {
            try
            {
                bool bChk = false;
                List<string> codes = new List<string>();
                Mat srcImage = BitmapConverter.ToMat(srcBitmap);
                Mat ConvertImg = srcImage.Clone();
                Cv2.ImWrite("D:\\TEST.png", srcImage);
                bChk = ConvertDataMatrix(srcImage, ref ConvertImg, (int)MCR_Simbols.MCR_Simbols_Auto);
                var zxresult = RecognitionMatrix(ConvertImg, type);           
                Cv2.ImWrite("D:\\DstImg.png", ConvertImg);
                if (zxresult != "")
                {
                    codes.Add(zxresult);
                    //SaveMCRImg(roiMatThresh.Clone(), type, plcIndex, lot + "_기존_" + zxresult); // 인식 성공
                    //SaveMCRImg(roiMat.Clone(), type, plcIndex, lot + "_기존_Success_Origin"); // 인식 실패

                    //return check_datamatrix(codes, lot) ? codes : null;
                }
                //Log2DMark(srcImage, lot, plcIndex, codes);
                //if (MainWindow.bMCR_TEST_SAVE) SaveMCRImg(srcImage.Clone(), type, plcIndex, lot, "Fail", codes); // 인식 실패  
                return codes;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                List<string> result = new List<string>();
                return result;
            }
        }
        public static bool ConvertDataMatrix(Mat srcImage, ref Mat DstImg, int simbol, int ithreshold = -1)
        {
            //try
            {
                Mat image = srcImage.Clone();
                List<string> codes = new List<string>();
                Mat label_box = new Mat();

                Mat image_gray = new Mat();
                Mat image_bi = new Mat();
                // 라벨 레이어 변수
                Mat img_label = new Mat();
                Mat stats = new Mat();

                Mat centroids = new Mat();
                Mat mask = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3), new Point(1, 1));

                int label;
                // 복사
                label_box = image.Clone();

                //그레이스케일
                if (image.Channels() > 1)
                    Cv2.CvtColor(image, image_gray, ColorConversionCodes.BGR2GRAY);
                else
                    image.CopyTo(image_gray);

                Mat MatRoiImg = new Mat();
                Mat tempImg = new Mat();
                Mat MCRResultImg;
                Mat MatRoiImgBi = new Mat(); ;
                Mat MatRoiImgBiLine = new Mat();
                Mat MatRoiImgDot = new Mat();
                Mat CrossLineImg = new Mat();

                int iboraderLength = 0;
                List<Mat> ListMatRoiImg = new List<Mat>();
                List<Rect> ListFindRect = new List<Rect>();
                bool bFind = false;
                bool bReverse = false;
                List<bool> ListReverseFlag = new List<bool>();

                bool bRetry = false;
                bool bFindFlag = true;
                ListMatRoiImg.Clear();
                bool AutoSizeChk = true;
                int iMatrixCnt = 16;

                for (int i = 0; i < 5; i++)
                {
                    if (i == 0) //이진화
                    {
                        if (ithreshold != -1)
                            Cv2.Threshold(image_gray, image_bi, ithreshold, 255, ThresholdTypes.Binary);
                        else//OTSU 알고리즘
                            Cv2.Threshold(image_gray, image_bi, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
                        Cv2.Dilate(image_bi, image_bi, mask, new Point(-1, -1), 1, BorderTypes.Replicate); //노이즈 제거
                        Cv2.Erode(image_bi, image_bi, mask, new Point(-1, -1), 1, BorderTypes.Replicate); //노이즈 제거     
                    }
                    else if (i == 1) //(첫번째 이진화 실패할 경우 반전)
                    {
                        bReverse = true;
                        if (ithreshold != -1)
                            Cv2.Threshold(image_gray, image_bi, ithreshold, 255, ThresholdTypes.BinaryInv);
                        else//OTSU 알고리즘
                            Cv2.Threshold(image_gray, image_bi, 0, 255, ThresholdTypes.BinaryInv | ThresholdTypes.Otsu);
                        Cv2.Erode(image_bi, image_bi, mask, new Point(-1, -1), 1, BorderTypes.Replicate); //노이즈 제거  
                        Cv2.Dilate(image_bi, image_bi, mask, new Point(-1, -1), 1, BorderTypes.Replicate); //노이즈 제거       
                    }
                    else if (i == 2) //(첫번째 이진화 실패할 경우 반전)
                    {
                        bReverse = false;
                        Cv2.Threshold(image_gray, image_bi, 150, 255, ThresholdTypes.Binary);
                        Cv2.Dilate(image_bi, image_bi, mask, new Point(-1, -1), 1, BorderTypes.Replicate); //노이즈 제거
                        Cv2.Erode(image_bi, image_bi, mask, new Point(-1, -1), 1, BorderTypes.Replicate); //노이즈 제거    
                    }

                    else if (i == 3) //(첫번째 이진화 실패할 경우 반전)
                    {
                        bReverse = true;
                        Cv2.Threshold(image_gray, image_bi, 150, 255, ThresholdTypes.BinaryInv);
                        Cv2.Dilate(image_bi, image_bi, mask, new Point(-1, -1), 1, BorderTypes.Replicate); //노이즈 제거
                        Cv2.Erode(image_bi, image_bi, mask, new Point(-1, -1), 1, BorderTypes.Replicate); //노이즈 제거    
                    }
                    else if (i == 4) //(첫번째 이진화 실패할 경우 반전)
                    {
                        bReverse = false;
                        Cv2.GaussianBlur(image_gray, image_gray, new Size(3, 3), 0);
                        Cv2.Canny(image_gray, image_bi, 0, 50, 3);
                    }

                    Mat cent = new Mat();
                    label = Cv2.ConnectedComponentsWithStats(image_bi, img_label, stats, centroids, PixelConnectivity.Connectivity8, MatType.CV_32S);

                    int iCutSize = 1;
                    //if (!AutoSizeChk) iCutSize = 0;

                    int area, left, top, width, height;
                    for (int j = 1; j < label; j++)
                    {
                        left = stats.At<int>(j, (int)ConnectedComponentsTypes.Left);
                        top = stats.At<int>(j, (int)ConnectedComponentsTypes.Top);
                        width = stats.At<int>(j, (int)ConnectedComponentsTypes.Width);
                        height = stats.At<int>(j, (int)ConnectedComponentsTypes.Height);
                        area = stats.At<int>(j, (int)ConnectedComponentsTypes.Area);

                        // 찾은 덩어리 중 큰것부터 작은 것 순으로 검색하여. 정사각형과 유사하고, 사이즈가 적당히 큰 것으로 리턴한다. 큰 사각형 테두리를 찾더라도 더 작은 것이 우선
                        if (/*Math.Abs(width - height) < 30 &&*/ width > 40 && height > 40)
                        {
                            // 라벨링 박스
                            if (left < iCutSize || top < iCutSize || image.Cols < left + width + (iCutSize) || image.Rows < top + height + (iCutSize)) continue;

                            Cv2.Rectangle(label_box, new Rect(left - iCutSize, top - iCutSize, width + (iCutSize * 2), height + (iCutSize * 2)), new Scalar(0, 0, 255), 3);

                            Rect roi = new Rect(left - iCutSize, top - iCutSize, width + (iCutSize * 2), height + (iCutSize * 2));
                            MatRoiImg = image_gray.SubMat(roi);
                            Rect biroi = new Rect(left - iCutSize, top - iCutSize, width + (iCutSize * 2), height + (iCutSize * 2));
                            tempImg = image_bi.SubMat(biroi);

                            int Size = 0;
                            FindCountour(tempImg, ref Size);
                            if (Size < 15)//찾은 뭉티기 개수가 15개 미만(MCR Image를 제대로 커트했으면 15개 이상 나옴)
                            {
                                continue;
                            }
                            ListMatRoiImg.Add(MatRoiImg);
                            ListFindRect.Add(new Rect(new Point(top, left), new Size(height, width)));
                            ListReverseFlag.Add(bReverse);
                            bFind = true;
                        }
                    }
                    if (bFind && i != 0) break;
                }


                if (ListMatRoiImg.Count() == 0)
                    bFindFlag = false;

                for (int k = 0; k < ListMatRoiImg.Count(); k++) //찾은 후보들을 전부 Search
                {
                    bFindFlag = true;
                    if (ListMatRoiImg == null)
                    {
                        bFindFlag = false;
                        break;
                    }
                    if (ListMatRoiImg[k].Rows < 100)// 100pixel 미만인것들은 Resize 한다.
                    {
                        Cv2.Resize(ListMatRoiImg[k], ListMatRoiImg[k], new Size(), 3.0, 3.0, InterpolationFlags.Lanczos4);
                    }
                    else
                    {
                        if (!bRetry)//후보의 노이즈 제거
                        {
                            if (ListReverseFlag[k])
                            {
                                Cv2.Erode(ListMatRoiImg[k], ListMatRoiImg[k], mask, new Point(-1, -1), 1, BorderTypes.Replicate);
                                Cv2.Dilate(ListMatRoiImg[k], ListMatRoiImg[k], mask, new Point(-1, -1), 1, BorderTypes.Replicate);

                            }
                            else
                            {
                                Cv2.Dilate(ListMatRoiImg[k], ListMatRoiImg[k], mask, new Point(-1, -1), 1, BorderTypes.Replicate);
                                Cv2.Erode(ListMatRoiImg[k], ListMatRoiImg[k], mask, new Point(-1, -1), 1, BorderTypes.Replicate);
                            }
                        }
                        else
                        {
                            if (ListReverseFlag[k])
                            {
                                Cv2.Dilate(ListMatRoiImg[k], ListMatRoiImg[k], mask, new Point(-1, -1), 1, BorderTypes.Replicate);
                                Cv2.Erode(ListMatRoiImg[k], ListMatRoiImg[k], mask, new Point(-1, -1), 1, BorderTypes.Replicate);
                            }
                            else
                            {
                                Cv2.Erode(ListMatRoiImg[k], ListMatRoiImg[k], mask, new Point(-1, -1), 1, BorderTypes.Replicate);
                                Cv2.Dilate(ListMatRoiImg[k], ListMatRoiImg[k], mask, new Point(-1, -1), 1, BorderTypes.Replicate);
                            }
                        }
                    }
                    int iMeanPeak = 0, iPeakCnt = 0;
                    mapPeakInfo = new SortedDictionary<float, int>();
                    if (ithreshold != -1)//찾은 후보를 고정 threshold로 변환
                    {
                        Cv2.Threshold(ListMatRoiImg[k], MatRoiImgBi, ithreshold, 255, ThresholdTypes.Binary);
                        iPeakCnt = -1;
                    }
                    else//찾은 후보를 히스토그램을 구해 가장 높은 Peak 와 그 다음 Peaak의 평균 값으로 threshold
                    {
                        FindHistMeanPeak(ListMatRoiImg[k], out iMeanPeak);
                        Cv2.Threshold(ListMatRoiImg[k], MatRoiImgBi, iMeanPeak, 255, ThresholdTypes.Binary);
                    }

                    iboraderLength = (int)(MatRoiImgBi.Rows / 100); //가장자리에 여백 만들기(여백이 어느정도 있어야함)
                    if (!bRetry)
                    {
                        if (ListReverseFlag[k])
                        {
                            Cv2.CopyMakeBorder(MatRoiImgBi, MatRoiImgBi, iboraderLength, iboraderLength, iboraderLength, iboraderLength, BorderTypes.Constant, 255);
                            Cv2.Dilate(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                            Cv2.Erode(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                        }
                        else
                        {
                            Cv2.CopyMakeBorder(MatRoiImgBi, MatRoiImgBi, iboraderLength, iboraderLength, iboraderLength, iboraderLength, BorderTypes.Constant, 0);
                            Cv2.Erode(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                            Cv2.Dilate(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                        }
                    }
                    else
                    {
                        if (ListReverseFlag[k])
                        {
                            Cv2.CopyMakeBorder(MatRoiImgBi, MatRoiImgBi, iboraderLength, iboraderLength, iboraderLength, iboraderLength, BorderTypes.Constant, 0);
                            Cv2.Erode(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                            Cv2.Dilate(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                        }
                        else
                        {
                            Cv2.CopyMakeBorder(MatRoiImgBi, MatRoiImgBi, iboraderLength, iboraderLength, iboraderLength, iboraderLength, BorderTypes.Constant, 255);
                            Cv2.Dilate(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                            Cv2.Erode(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                        }
                    }
                    int[] iindexChk = new int[2];
                    List<int>[] ListEdgePoints = new List<int>[2];
                    for (int i = 0; i < 2; i++) ListEdgePoints[i] = new List<int>();
                    bool[] m_bMCROrigin = new bool[2];
                    int m_iMCROrigin = 0;
                    if (AutoSizeChk)
                    {
                        iMatrixCnt = 0;
                        int iCntTmp = simbol;

                        for (int rotate = 0; rotate < 2; rotate++) //상면, 하면 검사하여 Matrix Dot Count Check
                        {
                            while (OriginMake(MatRoiImgBi.Clone()) ? !FindMCRCnt(MatRoiImgBi, rotate, ref ListEdgePoints, ref iindexChk, ref iCntTmp, ref m_bMCROrigin, ref m_iMCROrigin) : true)
                            {
                                iPeakCnt++;
                                if (!FindHistMeanPeak(ListMatRoiImg[k], out iMeanPeak, iPeakCnt) || iPeakCnt > 5)
                                {
                                    bFindFlag = false;
                                    break;
                                }

                                iCntTmp = simbol;
                                rotate = 0;
                                Cv2.Threshold(ListMatRoiImg[k], MatRoiImgBi, iMeanPeak, 255, ThresholdTypes.Binary);
                                if (!bRetry)
                                {
                                    if (ListReverseFlag[k])
                                    {
                                        Cv2.CopyMakeBorder(MatRoiImgBi, MatRoiImgBi, iboraderLength, iboraderLength, iboraderLength, iboraderLength, BorderTypes.Constant, 255);
                                        Cv2.Dilate(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                                        Cv2.Erode(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                                    }
                                    else
                                    {
                                        Cv2.CopyMakeBorder(MatRoiImgBi, MatRoiImgBi, iboraderLength, iboraderLength, iboraderLength, iboraderLength, BorderTypes.Constant, 0);
                                        Cv2.Erode(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                                        Cv2.Dilate(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                                    }
                                }
                                else
                                {
                                    if (ListReverseFlag[k])
                                    {
                                        Cv2.CopyMakeBorder(MatRoiImgBi, MatRoiImgBi, iboraderLength, iboraderLength, iboraderLength, iboraderLength, BorderTypes.Constant, 0);
                                        Cv2.Erode(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                                        Cv2.Dilate(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                                    }
                                    else
                                    {
                                        Cv2.CopyMakeBorder(MatRoiImgBi, MatRoiImgBi, iboraderLength, iboraderLength, iboraderLength, iboraderLength, BorderTypes.Constant, 255);
                                        Cv2.Dilate(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                                        Cv2.Erode(MatRoiImgBi, MatRoiImgBi, mask, new Point(-1, -1), iboraderLength, BorderTypes.Replicate);
                                    }
                                }
                            }

                            if (iCntTmp >= iMatrixCnt)
                                iMatrixCnt = iCntTmp;
                            if (bFindFlag == false && bRetry == true) continue;

                            if (iMatrixCnt < 10)
                            {
                                continue;
                            }
                        }

                        if (iMatrixCnt < 10 ||
                            (ListEdgePoints[0] == null || ListEdgePoints[0].Count == 0) ||
                            (ListEdgePoints[1] == null || ListEdgePoints[1].Count == 0) ||
                            ListEdgePoints[0].Count() % 2 == 1 ||
                            ListEdgePoints[1].Count() % 2 == 1)// ||
                            //ListEdgePoints[0].Count() != ListEdgePoints[1].Count())
                        {
                            bFindFlag = false;
                        }

                        if (bFindFlag == true)
                        {
                            /////////////////////검증용/////////////////////////
                            MatRoiImgBi.CopyTo(MatRoiImgBiLine);
                            MatRoiImgBi.CopyTo(MatRoiImgDot);
                            Cv2.CvtColor(ListMatRoiImg[k], ListMatRoiImg[k], ColorConversionCodes.GRAY2BGR);
                            Cv2.CvtColor(MatRoiImgBiLine, MatRoiImgBiLine, ColorConversionCodes.GRAY2BGR);
                            //라인 search
                            for (int rotate = 0; rotate < ListEdgePoints[0].Count; rotate++) //검증용 Line 그리기
                            {
                                Cv2.Line(MatRoiImgBiLine, ListEdgePoints[0][rotate], 0, ListEdgePoints[0][rotate], MatRoiImgBi.Rows, new Scalar(0, 255, 0));
                            }
                            for (int rotate = 0; rotate < ListEdgePoints[1].Count; rotate++)
                            {
                                Cv2.Line(MatRoiImgBiLine, 0, ListEdgePoints[1][rotate], MatRoiImgBi.Cols, ListEdgePoints[1][rotate], new Scalar(0, 255, 0));
                            }
                            MatRoiImgBiLine.CopyTo(CrossLineImg);

                            int numerator, denominator, multColum, multRow;
                            if (ListEdgePoints[0].Count > ListEdgePoints[1].Count)
                            {
                                numerator = ListEdgePoints[0].Count;
                                denominator = ListEdgePoints[1].Count;
                                multColum = (int)Math.Ceiling((double)(numerator / denominator));
                                multRow = 1;

                            }
                            else
                            {
                                numerator = ListEdgePoints[1].Count;
                                denominator = ListEdgePoints[0].Count;
                                multColum = 1;
                                multRow = (int)Math.Ceiling((double)(numerator / denominator));
                            }
                            Cv2.Resize(CrossLineImg, CrossLineImg, new Size(300 * multColum, 300 * multRow), 0, 0, InterpolationFlags.Linear);
                            Cv2.ImWrite("D:\\cross.png", CrossLineImg);
                            //////////////////////////////////////////////////////
                            //MatRoiImgBiLine                        
                            if (!bRetry)
                            {
                                if (ListReverseFlag[k])
                                    MCRResultImg = new Mat(ListEdgePoints[1].Count + 3, ListEdgePoints[0].Count + 3, MatType.CV_8U, new Scalar(255));
                                else
                                    MCRResultImg = new Mat(ListEdgePoints[1].Count + 3, ListEdgePoints[0].Count + 3, MatType.CV_8U, new Scalar(0));
                            }
                            else
                            {
                                if (!ListReverseFlag[k])
                                    MCRResultImg = new Mat(ListEdgePoints[1].Count + 3, ListEdgePoints[0].Count + 3, MatType.CV_8U, new Scalar(255));
                                else
                                    MCRResultImg = new Mat(ListEdgePoints[1].Count + 3, ListEdgePoints[0].Count + 3, MatType.CV_8U, new Scalar(0));
                            }
                            unsafe
                            {
                                int ix, iy;
                                //바이너리  IMAGE 생성 
                                Mat MCRResult = MCRResultImg.SubMat(new Rect(1, 1, ListEdgePoints[0].Count + 1, ListEdgePoints[1].Count + 1));

                                for (int rotate = 0; rotate <= ListEdgePoints[1].Count; rotate++)
                                {
                                    if (rotate == 0)
                                    {
                                        if (m_iMCROrigin == 1 || m_iMCROrigin == 2)
                                            continue;

                                        ix = (int)((ListEdgePoints[1][rotate] / 2) + 0.5);
                                    }
                                    else if (rotate == ListEdgePoints[1].Count)
                                    {
                                        if (m_iMCROrigin == 3 || m_iMCROrigin == 4)
                                            continue;

                                        ix = (int)((MatRoiImgBi.Rows + ListEdgePoints[1][rotate - 1]) / 2 - 0.5);
                                    }
                                    else
                                    {
                                        if (m_iMCROrigin == 1 || m_iMCROrigin == 2)
                                            ix = (int)((ListEdgePoints[1][rotate - 1] + ListEdgePoints[1][rotate]) / 2 - 0.5);
                                        else
                                            ix = (int)((ListEdgePoints[1][rotate - 1] + ListEdgePoints[1][rotate]) / 2 + 0.5);
                                    }

                                    byte* dotPixel = (byte*)MatRoiImgDot.Ptr(ix);
                                    byte* mcrPtr = (byte*)MCRResult.Ptr(rotate);

                                    for (int j = 0; j <= ListEdgePoints[0].Count; j++)
                                    {
                                        if (j == 0)
                                        {
                                            if (m_iMCROrigin == 1 || m_iMCROrigin == 3)
                                                continue;

                                            iy = (int)((ListEdgePoints[0][j] / 2) + 0.5);
                                        }
                                        else if (j == ListEdgePoints[0].Count)
                                        {
                                            if (m_iMCROrigin == 2 || m_iMCROrigin == 4)
                                                continue;

                                            iy = (int)((MatRoiImgBi.Cols + ListEdgePoints[0][j - 1]) / 2 + 0.5);
                                        }
                                        else
                                        {
                                            iy = (int)((ListEdgePoints[0][j - 1] + ListEdgePoints[0][j]) / 2 + 0.5);
                                        }

                                        mcrPtr[j] = dotPixel[iy];
                                        dotPixel[iy] = 127;
                                    }
                                }
                                //가장자리 영역 재구성 (X x X)
                                //상                            
                                for (int rotate = 1; rotate < MCRResult.Cols; rotate++)
                                {
                                    if (m_iMCROrigin == 1 || m_iMCROrigin == 2) break;
                                    byte value = MCRResult.At<byte>(0, rotate - 1);
                                    value = (value == 255) ? (byte)0 : (byte)255;
                                    MCRResult.Set(0, rotate, value);
                                }
                                //하
                                for (int rotate = 1; rotate < MCRResult.Cols; rotate++)
                                {
                                    if (m_iMCROrigin == 3 || m_iMCROrigin == 4) break;
                                    byte value = MCRResult.At<byte>(MCRResult.Rows - 1, rotate - 1);
                                    MCRResult.Set(MCRResult.Rows - 1, rotate, (value == 255) ? (byte)0 : (byte)255);
                                }
                                //좌
                                for (int rotate = 1; rotate < MCRResult.Rows; rotate++)
                                {
                                    if (m_iMCROrigin == 1 || m_iMCROrigin == 3) break;
                                    byte value = MCRResult.At<byte>(rotate - 1, 0);
                                    MCRResult.Set(rotate, 0, (value == 255) ? (byte)0 : (byte)255);
                                }
                                //우
                                for (int rotate = 1; rotate < MCRResult.Rows; rotate++)
                                {
                                    if (m_iMCROrigin == 2 || m_iMCROrigin == 4) break;
                                    byte value = MCRResult.At<byte>(rotate - 1, MCRResult.Cols - 1);
                                    MCRResult.Set(rotate, MCRResult.Cols - 1, (value == 255) ? (byte)0 : (byte)255);
                                }

                                switch (m_iMCROrigin)
                                {
                                    case 1:
                                        MCRResultImg = new Mat(MCRResultImg, new Rect(1, 1, MCRResultImg.Cols - 1, MCRResultImg.Rows - 1));
                                        break;
                                    case 2:
                                        MCRResultImg = new Mat(MCRResultImg, new Rect(0, 1, MCRResultImg.Cols - 1, MCRResultImg.Rows - 1));
                                        break;
                                    case 3:
                                        MCRResultImg = new Mat(MCRResultImg, new Rect(1, 0, MCRResultImg.Cols - 1, MCRResultImg.Rows - 1));
                                        break;
                                    case 4:
                                        MCRResultImg = new Mat(MCRResultImg, new Rect(0, 0, MCRResultImg.Cols - 1, MCRResultImg.Rows - 1));
                                        break;
                                    default:
                                        MCRResultImg = new Mat(MCRResultImg, new Rect(1, 0, MCRResultImg.Cols - 1, MCRResultImg.Rows - 1));
                                        break;
                                }
                            }
                            Cv2.Resize(MCRResultImg, MCRResultImg, new Size(MCRResultImg.Cols * 10, MCRResultImg.Rows * 10), 0, 0, InterpolationFlags.Nearest);
                            MCRResultImg.CopyTo(DstImg);
                        }
                    }
                    if (bFindFlag)
                    {
                        return true;
                    }
                    else
                    {
                        if (k == ListMatRoiImg.Count - 1 && bRetry == false)
                        {
                            bRetry = true;
                            k = -1;
                        }
                        continue;
                    }
                }
            }
            //catch (Exception e)
            //{
            //    MainWindow.Log("PCS", SeverityLevel.DEBUG, string.Format("MatrixCode Read Fail, Error: {0}", e.Message), true);
            //    return false;
            //}
            return false;
        }
        public static bool OriginMake(Mat Byimg)
        {
            Mat matroiimg_gray_roi;
            int iRisingCnt = 0;
            int RisingPos = 0, FallingPos = 0;
            byte ucPixel = 0, ucPixel_back = 0;
            byte PixelData = 0;

            int BorderLength = (int)(Byimg.Rows / 100.0 + 0.5);
            int linePos = -999;
            double edgePer = 0.8;

            bool width = false, height = false;

            // 상단
            for (int i = 0; i < Byimg.Rows / 10; i++)
            {
                matroiimg_gray_roi = new Mat(Byimg, new Rect(0, i, Byimg.Cols, 1));
                iRisingCnt = 0;

                for (int j = 0; j < Byimg.Cols; j++)
                {
                    ucPixel = matroiimg_gray_roi.At<byte>(0, j);

                    if (i == 0 && j == 0)
                        ucPixel_back = ucPixel;

                    if (ucPixel != ucPixel_back)
                    {
                        if (ucPixel > ucPixel_back)
                        {
                            iRisingCnt++;
                            RisingPos = j;
                        }
                        else
                            FallingPos = j;
                    }

                    ucPixel_back = ucPixel;

                    if (iRisingCnt > 2)
                        break;
                }

                if (iRisingCnt == 1 && Math.Abs(FallingPos - RisingPos) > Byimg.Cols * edgePer)
                {
                    if (linePos == -999)
                        linePos = i;
                    else
                    {
                        if (Math.Abs(linePos - i) < 1) break;
                        else linePos = i;
                    }

                    PixelData = (byte)(FallingPos > RisingPos ? 255 : 0);

                    Cv2.Line(Byimg,
                        new Point(BorderLength, i),
                        new Point(Byimg.Cols - 1 - BorderLength, i),
                        new Scalar(PixelData), 1);
                    //Cv2.ImWrite("D:\\originmake1.png", Byimg);
                    width = true;
                }
            }

            // 하단
            linePos = -999;
            for (int i = Byimg.Rows - 2; i > Byimg.Rows * 9 / 10; i--)
            {
                iRisingCnt = 0;
                matroiimg_gray_roi = new Mat(Byimg, new Rect(0, i, Byimg.Cols, 1));

                for (int j = 0; j < Byimg.Cols; j++)
                {
                    ucPixel = matroiimg_gray_roi.At<byte>(0, j);

                    if (i == 0 && j == 0)
                        ucPixel_back = ucPixel;

                    if (ucPixel != ucPixel_back)
                    {
                        if (ucPixel > ucPixel_back)
                        {
                            iRisingCnt++;
                            RisingPos = j;
                        }
                        else
                            FallingPos = j;
                    }

                    ucPixel_back = ucPixel;

                    if (iRisingCnt > 2)
                        break;
                }

                if (iRisingCnt == 1 && Math.Abs(FallingPos - RisingPos) > Byimg.Cols * edgePer)
                {
                    if (linePos == -999)
                        linePos = i;
                    else
                    {
                        if (Math.Abs(linePos - i) != 1) break;
                        else linePos = i;
                    }

                    PixelData = (byte)(FallingPos > RisingPos ? 255 : 0);

                    Cv2.Line(Byimg,
                        new Point(BorderLength, i),
                        new Point(Byimg.Cols - 1 - BorderLength, i),
                        new Scalar(PixelData), 1);
                    //Cv2.ImWrite("D:\\originmake2.png", Byimg);
                    width = true;
                }
            }

            // 좌측
            linePos = -999;
            for (int i = 0; i < Byimg.Cols / 10; i++)
            {
                iRisingCnt = 0;
                matroiimg_gray_roi = new Mat(Byimg, new Rect(i, 0, 1, Byimg.Rows));

                for (int j = 0; j < Byimg.Rows; j++)
                {
                    ucPixel = matroiimg_gray_roi.At<byte>(j, 0);

                    if (i == 0 && j == 0)
                        ucPixel_back = ucPixel;

                    if (ucPixel != ucPixel_back)
                    {
                        if (ucPixel > ucPixel_back)
                        {
                            iRisingCnt++;
                            RisingPos = j;
                        }
                        else
                            FallingPos = j;
                    }

                    ucPixel_back = ucPixel;

                    if (iRisingCnt > 2)
                        break;
                }

                if (iRisingCnt == 1 && Math.Abs(FallingPos - RisingPos) > Byimg.Rows * edgePer)
                {
                    if (linePos == -999)
                        linePos = i;
                    else
                    {
                        if (Math.Abs(linePos - i) != 1) break;
                        else linePos = i;
                    }

                    PixelData = (byte)(FallingPos > RisingPos ? 255 : 0);

                    Cv2.Line(Byimg,
                        new Point(i, BorderLength),
                        new Point(i, Byimg.Rows - 1 - BorderLength),
                        new Scalar(PixelData), 1);
                    //Cv2.ImWrite("D:\\originmake3.png", Byimg);
                    height = true;
                }
            }

            // 우측
            linePos = -999;
            for (int i = Byimg.Cols - 2; i > Byimg.Cols * 9 / 10; i--)
            {
                iRisingCnt = 0;
                matroiimg_gray_roi = new Mat(Byimg, new Rect(i, 0, 1, Byimg.Rows));

                for (int j = 0; j < Byimg.Rows; j++)
                {
                    ucPixel = matroiimg_gray_roi.At<byte>(j, 0);

                    if (i == 0 && j == 0)
                        ucPixel_back = ucPixel;

                    if (ucPixel != ucPixel_back)
                    {
                        if (ucPixel > ucPixel_back)
                        {
                            iRisingCnt++;
                            RisingPos = j;
                        }
                        else
                            FallingPos = j;
                    }

                    ucPixel_back = ucPixel;

                    if (iRisingCnt > 2)
                        break;
                }

                if (iRisingCnt == 1 && Math.Abs(FallingPos - RisingPos) > Byimg.Rows * edgePer)
                {
                    if (linePos == -999)
                        linePos = i;
                    else
                    {
                        if (Math.Abs(linePos - i) != 1) break;
                        else linePos = i;
                    }

                    PixelData = (byte)(FallingPos > RisingPos ? 255 : 0);

                    Cv2.Line(Byimg,
                        new Point(i, BorderLength),
                        new Point(i, Byimg.Rows - 1 - BorderLength),
                        new Scalar(PixelData), 1);
                    //Cv2.ImWrite("D:\\originmake4.png", Byimg);
                    height = true;
                }
            }

            return height && width;
        }
        unsafe public static bool FindMCRCnt(Mat img, int iType, ref List<int>[] vecEdgePoints, ref int[] iIndex, ref int MatrixCnt, ref bool[] m_bMCROrigin, ref int m_iMCROrigin)
        {
            if (img.Empty()) return false;

            Mat matTmp = new Mat();

            img.CopyTo(matTmp);
            if (matTmp.Channels() != 1)
                Cv2.CvtColor(matTmp, matTmp, ColorConversionCodes.BGR2GRAY);

            if (iType == 1)
            {
                matTmp = matTmp.T();
                Cv2.Flip(matTmp, matTmp, FlipMode.Y);
            }
            else if (iType != 0)
            {
                return false;
            }
            Size DotSize = FindDotSize(img.Clone());
            int Count = (int)(matTmp.Rows / DotSize.Width);
            int iRowSize = (int)(matTmp.Rows / Count);  
            MultiMap<int, int>[] mapEdgeSearch = new MultiMap<int, int>[2];
            MultiMap<int, int>[] mapRisingPos = new MultiMap<int, int>[2];
            MultiMap<int, int>[] mapFallingPos = new MultiMap<int, int>[2];
            HashSet<int>[] mapEdgeSearchKey = new HashSet<int>[2];

            for (int i = 0; i < 2; i++)
            {
                mapEdgeSearch[i] = new MultiMap<int, int>();
                mapRisingPos[i] = new MultiMap<int, int>();
                mapFallingPos[i] = new MultiMap<int, int>();
                mapEdgeSearchKey[i] = new HashSet<int>();
            }

            int[] MaxKey = new int[2];
            int[] repeatTar = new int[2];
            int mcr_version = 0;

            for (int updown = 0; updown < 2; updown++)
            {
                if (updown == 1)
                    Cv2.Flip(matTmp, matTmp, FlipMode.XY);
                int MaxKeyCnt = 0;
                int iFindCutPos = -1;

                for (int i = 0; i < iRowSize; i++)
                {
                    int iRisingCnt = 0;
                    int iFallingCnt = 0;

                    byte* ptr = (byte*)matTmp.Ptr(i).ToPointer();
                    byte prev = ptr[0];

                    for (int j = 1; j < matTmp.Cols; j++)
                    {
                        byte cur = ptr[j];

                        if (cur != prev)
                        {
                            if (cur > prev)
                            {
                                iRisingCnt++;
                                mapRisingPos[updown].Add(i, j);
                            }
                            else
                            {
                                iFallingCnt++;
                                mapFallingPos[updown].Add(i, j);
                            }
                        }
                        prev = cur;
                    }

                    if (iRisingCnt != 0 && (iRisingCnt == 1 || iRisingCnt >= 5))
                    {
                        mapEdgeSearchKey[updown].Add(iRisingCnt);
                        mapEdgeSearch[updown].Add(iRisingCnt, i);
                    }
                }

                foreach (var key in mapEdgeSearchKey[updown])
                {
                    int cnt = mapEdgeSearch[updown].Count(key);
                    if (cnt > MaxKeyCnt)
                    {
                        MaxKeyCnt = cnt;
                        MaxKey[updown] = key;
                    }
                }

                repeatTar[updown] =
                    (int)(MaxKeyCnt * MaxKeyCnt / (double)iRowSize + 0.5);

                if (repeatTar[updown] < 1)
                    repeatTar[updown] = 1;

                int sqrt = (int)Math.Sqrt(iRowSize);
                if (repeatTar[updown] > sqrt)
                    repeatTar[updown] = sqrt;

                var list = mapEdgeSearch[updown].GetValues(MaxKey[updown]);
                if (list == null) return false;

                int prevVal = -9999;
                int repeatcnt = 0;

                foreach (var v in list)
                {
                    if (Math.Abs(v - prevVal) == 1)
                        repeatcnt++;
                    else
                        repeatcnt = 0;

                    if (repeatcnt >= repeatTar[updown])
                    {
                        iFindCutPos = v - repeatcnt;
                        break;
                    }

                    prevVal = v;
                }

                if (iFindCutPos == -1)
                    return false;

                iIndex[updown] = iFindCutPos;

                if (mcr_version < MaxKey[updown])
                    mcr_version = MaxKey[updown];
            }
                      
            MatrixCnt = mcr_version * 2;           
            //if (img.Cols > matTmp.Cols)
            //    MatrixCnt = mcr_version * 2;
            if (MatrixCnt == 0)
                return false;

            vecEdgePoints[iType].Clear();

            bool bDotPos = MaxKey[0] < MaxKey[1];
            int idx = bDotPos ? 1 : 0;

            int repeat = repeatTar[idx];

            int[][] pts = new int[repeat][];
            int[] cnts = new int[repeat];

            for (int i = 0; i < repeat; i++)
                pts[i] = new int[MatrixCnt + 4];

            for (int i = 0; i < repeat; i++)
            {
                int cnt = 0;
                int row = iIndex[idx] + i;

                var rising = mapRisingPos[idx].GetValues(row);
                if (rising != null)
                {
                    foreach (var v in rising)
                    {
                        if (cnt > mcr_version + 2) break;
                        pts[i][cnt++] = v;
                    }
                }

                var falling = mapFallingPos[idx].GetValues(row);
                if (falling != null)
                {
                    foreach (var v in falling)
                    {
                        if (cnt > MatrixCnt + 4) break;
                        pts[i][cnt++] = v;
                    }
                }

                Array.Sort(pts[i], 0, cnt);
                cnts[i] = cnt;
            }

            for (int j = 0; j < MatrixCnt + 4; j++)
            {
                long sum = 0;
                int cnt = 0;

                for (int i = 0; i < repeat; i++)
                {
                    if (cnts[i] <= j) break;
                    sum += pts[i][j];
                    cnt++;
                }

                if (cnt == 0) continue;

                int avg = (int)(sum / (double)cnt + 0.5);
                if (avg == 0) continue;

                if (iType == 1)
                    vecEdgePoints[iType].Add(bDotPos ? avg : matTmp.Cols - avg);
                else
                    vecEdgePoints[iType].Add(bDotPos ? matTmp.Cols - avg : avg);
            }

            vecEdgePoints[iType].Sort();

            m_bMCROrigin[iType] = (MaxKey[0] != 1);

            if (iType == 1)
            {
                if (m_bMCROrigin[0])
                    m_iMCROrigin = m_bMCROrigin[1] ? 4 : 3;
                else
                    m_iMCROrigin = m_bMCROrigin[1] ? 2 : 1;
            }

            return true;
        }        
    }

    public class SortedTuple<TKey, TValue> : SortedSet<Tuple<TKey, TValue>>, IEnumerable<Tuple<TKey, TValue>> where TKey : IComparable
    {
        private class TupleComparer : Comparer<Tuple<TKey, TValue>>
        {
            public override int Compare(Tuple<TKey, TValue> x, Tuple<TKey, TValue> y)
            {
                if (x == null || y == null) return 0;
                return x.Item1.Equals(y.Item1) ? 1 : Comparer<TKey>.Default.Compare(x.Item1, y.Item1);
            }

        }
        public SortedTuple() : base(new TupleComparer()) { }
        public void Add(TKey key, TValue value)
        {
            Add(new Tuple<TKey, TValue>(key, value));
        }

        public Tuple<TKey, TValue> Find(TKey key, TValue value)
        {
            foreach (var item in this)
            {
                if (item.Item2.Equals(key) && item.Item1.Equals(value))
                {
                    return item;
                }
            }
            return null;
        }

        public Tuple<TKey, TValue> FindKey(TKey key)
        {
            foreach (var item in this)
            {
                if (item.Item2.Equals(key))
                {
                    return item;
                }
            }
            return null;
        }

        public Tuple<TKey, TValue> FindValue(TValue value)
        {
            foreach (var item in this)
            {
                if (item.Item2.Equals(value))
                {
                    return item;
                }
            }
            return null;
        }

        public bool ContainsKey(TKey key)
        {
            foreach (var item in this)
            {
                if (item.Item1.Equals(key)) return true;
            }
            return false;
        }
        public bool ContainsValue(TValue value)
        {
            foreach (var item in this)
            {
                if (item.Item2.Equals(value)) return true;
            }
            return false;
        }
        public bool Contains(TKey key, TValue value)
        {
            return Contains(new Tuple<TKey, TValue>(key, value));
        }

        public int EqualKeyCount(TKey key)
        {
            int count = 0;
            foreach (var item in this)
            {
                if (item.Item1.Equals(key)) count++;
            }
            return count;
        }
        public Tuple<TKey, TValue> LowerBound(int key)
        {
            foreach (var item in this)
            {
                if (item.Item1.CompareTo(key) >= 0)
                    return item;
            }
            return null;
        }
        public Tuple<TKey, TValue> UpperBound(int key)
        {
            foreach (var item in this)
            {
                if (item.Item1.CompareTo(key) > 0)
                    return item;
            }
            return null;
        }

        public int LowerUpperBoundCount(int key)
        {
            int i = 0;
            foreach (var item in this)
            {
                if (item.Item1.CompareTo(key) > 0)
                {
                    break;
                }
                i++;
            }
            return i;
        }

        public Tuple<TKey, TValue> Begin()
        {
            if (Count > 0)
            {
                return this.Min;
            }
            return null;
        }
        public Tuple<TKey, TValue> End()
        {
            if (Count > 0)
            {
                return this.Max;
            }
            return null;
        }
    }
    public class MultiMap<TKey, TValue> where TKey : IComparable<TKey>
    {
        private List<KeyValuePair<TKey, TValue>> data = new List<KeyValuePair<TKey, TValue>>();
        private bool sorted = false;

        public void Add(TKey key, TValue value)
        {
            data.Add(new KeyValuePair<TKey, TValue>(key, value));
            sorted = false;
        }

        private void SortIfNeeded()
        {
            if (sorted) return;

            data.Sort((a, b) =>
            {
                int cmp = a.Key.CompareTo(b.Key);
                if (cmp != 0) return cmp;
                return 0; // C++ multimap도 value sort 보장 없음
            });

            sorted = true;
        }

        public int Count(TKey key)
        {
            int c = 0;
            for (int i = 0; i < data.Count; i++)
                if (data[i].Key.CompareTo(key) == 0)
                    c++;
            return c;
        }

        public IEnumerable<TValue> GetValues(TKey key)
        {
            SortIfNeeded();

            for (int i = 0; i < data.Count; i++)
                if (data[i].Key.CompareTo(key) == 0)
                    yield return data[i].Value;
        }

        public void RemoveKey(TKey key)
        {
            data.RemoveAll(x => x.Key.CompareTo(key) == 0);
            sorted = false;
        }

        public bool Empty()
        {
            return data.Count == 0;
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                SortIfNeeded();

                TKey prev = default(TKey);
                bool first = true;

                for (int i = 0; i < data.Count; i++)
                {
                    if (first || data[i].Key.CompareTo(prev) != 0)
                    {
                        yield return data[i].Key;
                        prev = data[i].Key;
                        first = false;
                    }
                }
            }
        }
    }
}
