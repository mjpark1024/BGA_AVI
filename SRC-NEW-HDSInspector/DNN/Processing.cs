
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DNN
{
    public static class Processing
    {
        /// <summary>
        /// Color Image Load 
        /// </summary>
        /// <param name="strPath">File path</param>
        /// <returns>Color Image</returns>
        public static BitmapSource LoadColorImage(string strPath)
        {
            Mat def = new Mat(strPath, ImreadModes.Color);
            return OpenCvSharp.Extensions.BitmapSourceConverter.ToBitmapSource(def);
            //Mat dst = new Mat();
            //if (def.p == System.Windows.Media.PixelFormats.Gray8)
            //{
            //    Mat dst2 = OpenCvSharp.Extensions.BitmapSourceConverter.ToMat(def);
            //    Cv2.CvtColor(dst2, dst, ColorConversionCodes.GRAY2RGB);
            //}
            //else dst = OpenCvSharp.Extensions.BitmapSourceConverter.ToMat(def);
        }

        /// <summary>
        /// Load Image
        /// </summary>
        /// <param name="strPath">File Path</param>
        /// <returns>Image</returns>
        public static BitmapSource LoadImage(string strPath)
        {
            if (!System.IO.File.Exists(strPath)) return null;
            Mat def = new Mat(strPath);
            if (def.Empty() || def.Cols == 0 || def.Rows == 0) return null;
            return OpenCvSharp.Extensions.BitmapSourceConverter.ToBitmapSource(def);
            //Mat dst = new Mat();
            //if (def.p == System.Windows.Media.PixelFormats.Gray8)
            //{
            //    Mat dst2 = OpenCvSharp.Extensions.BitmapSourceConverter.ToMat(def);
            //    Cv2.CvtColor(dst2, dst, ColorConversionCodes.GRAY2RGB);
            //}
            //else dst = OpenCvSharp.Extensions.BitmapSourceConverter.ToMat(def);
        }

        /// <summary>
        /// Blob Image from Feature Information
        /// </summary>
        /// <param name="refimg">Reference Image</param>
        /// <param name="defimg">Defect Image</param>
        /// <param name="di">Feature Information</param>
        /// <returns>Blob Image Array RGB</returns>
        public static Mat[] GetBlobImage(Mat refimg, Mat defimg, ref DefectInfo di)
        {

            if (di.ThresType == null)
            {
                return null;
            }
            if (di.ThresType == "Other")
            {
                if (di.Inspect == "공간형상검사")
                {
                    di.ThresType = "Bright";
                }
                else if (di.Inspect == "리드형상검사")
                {
                    di.ThresType = "Dark";
                }
                else
                    return null;
            }


            Mat rimgs = refimg.Clone();
            Mat dimgs = defimg.Clone();
            Mat[] bsref = SetColorImage(rimgs);
            Mat[] bsdef = SetColorImage(dimgs);

            if (bsref == null || bsdef == null)
                return null;
            int mode = (di.ThresType == "Dark") ? 0 : 1;
            int Thres = di.ThresHold;
            int ch = di.Cahnnel;
            if (mode > 1)
                return null;

            BlobInfo blob = new BlobInfo();
            Mat[] ret = new Mat[7];
            try
            {
                Mat rimg = bsref[ch].Clone();
                if (mode == 0)
                    Cv2.Threshold(rimg, rimg, Thres, 255, ThresholdTypes.Binary);
                else if (mode == 1)
                    Cv2.Threshold(rimg, rimg, Thres, 255, ThresholdTypes.BinaryInv);

                Cv2.Erode(rimg, rimg, new Mat());
                Cv2.Erode(rimg, rimg, new Mat());
                Cv2.Erode(rimg, rimg, new Mat());

                Mat dimg = bsdef[ch].Clone();
                if (mode == 1) Cv2.Threshold(dimg, dimg, Thres, 255, ThresholdTypes.Binary);
                else if (mode == 0) Cv2.Threshold(dimg, dimg, Thres, 255, ThresholdTypes.BinaryInv);
                Mat img = new Mat();
                Cv2.BitwiseAnd(rimg, dimg, img);

                using (var labelsMat = new MatOfInt())
                using (var statsMat = new MatOfInt())
                using (var centroidsMat = new MatOfDouble())
                {
                    int nLabels = Cv2.ConnectedComponentsWithStats(
                        img, labelsMat, statsMat, centroidsMat);
                    var labels = labelsMat.ToRectangularArray();
                    var stats = statsMat.ToRectangularArray();
                    var centroids = centroidsMat.ToRectangularArray();
                    blob.Blobs = nLabels - 1;
                    if (blob.Blobs > 0)
                    {
                        int nMax = 0;
                        int nMaxArea = 0;
                        blob.Sum = 0;
                        for (int i = 1; i < nLabels; i++)
                        {
                            if (nMaxArea < stats[i, 4])
                            {
                                nMaxArea = stats[i, 4];
                                nMax = i;
                            }
                            blob.Sum += stats[i, 4];
                        }
                        blob.Width = stats[nMax, 2];
                        blob.Height = stats[nMax, 3];
                        blob.Pixels = stats[nMax, 4];
                        blob.Center = new System.Windows.Point(centroids[nMax, 0], centroids[nMax, 1]);
                    }
                }

                blob.Angle = 0;

                Mat[] tmp = new Mat[3];
                for (int i = 0; i < 3; i++)
                {
                    tmp[i] = bsdef[i].Clone();
                    Cv2.BitwiseAnd(tmp[i], img, tmp[i]);
                    Scalar s = Cv2.Mean(tmp[i], img);
                    blob.Average[i] = (int)s.Val0;
                    ret[i] = tmp[i];
                }
                ret[3] = img;
                Mat dst = new Mat();
                if (defimg.Channels() == 1)
                {
                    Mat dst2 = dimgs.Clone();
                    Cv2.CvtColor(dst2, dst, ColorConversionCodes.GRAY2RGB);
                }
                else dst = dimgs.Clone();

                blob.AvgI = (int)(Cv2.Mean(dst, img).Val0);
                Mat tmp2 = new Mat();
                Cv2.CvtColor(img, tmp2, ColorConversionCodes.GRAY2RGB);
                Cv2.BitwiseAnd(dst, tmp2, dst);
                // Cv2.ImShow("c", dst);
                // Cv2.WaitKey(0);
                ret[4] = dst;
                Mat hsv = new Mat();
                Cv2.CvtColor(dst, hsv, ColorConversionCodes.RGB2HSV);
                Mat[] m = Cv2.Split(hsv);
                blob.AvgH = (int)(Cv2.Mean(m[0], img).Val0);
                blob.AvgS = (int)(Cv2.Mean(m[1], img).Val0);
                ret[5] = m[0];
                ret[6] = m[1];
                Mat[] img_m = new Mat[3];
                img_m[0] = bsref[ch].Clone();
                img_m[1] = bsdef[ch] - bsref[ch];//.Clone();
                img_m[2] = bsdef[ch].Clone();
                //Cv2.BitwiseNot(img_m[1], img_m[1]);
                Mat mrg = new Mat(img_m[2].Size(), MatType.CV_8UC3);
                Cv2.Merge(img_m, mrg);
                ret[0] = mrg;
            }
            catch
            {
                return null;
            }

            di.Width = blob.Width;
            di.Height = blob.Height;
            di.Blobs = blob.Blobs;
            di.Pixels = blob.Pixels;
            di.Angle = blob.Angle;
            di.Average[0] = blob.Average[0];
            di.Average[1] = blob.Average[1];
            di.Average[2] = blob.Average[2];
            di.AvgI = blob.AvgI;
            di.AvgH = blob.AvgH;
            di.AvgS = blob.AvgS;
            di.Sum = blob.Sum;
            di.Center = new System.Windows.Point(blob.Center.X, blob.Center.Y);
            return ret;

            //SegRImage.Source = bbs[0].Clone();
            //SegGImage.Source = bbs[1].Clone();
            //SegBImage.Source = bbs[2].Clone();
            //TresRImage.Source = bbs[3].Clone();
            //TresGImage.Source = bbs[5].Clone();
            //TresBImage.Source = bbs[6].Clone();
            //MergeImage.Source = bbs[4].Clone();
        }

        /// <summary>
        /// Split RGB Image
        /// </summary>
        /// <param name="img">Color Image</param>
        /// <returns>RGB Split Image Array</returns>
        private static Mat[] SetColorImage(Mat img)
        {
            Mat[] bs = new Mat[3];
            try
            {
                //bs = img.Split();


                if (img.Channels() == 3)
                {
                    bs = img.Split();
                }
                else
                {
                    //System.Drawing.Color[][] matrix;
                    bs[0] = img.Clone();
                    bs[1] = img.Clone();
                    bs[2] = img.Clone();
                }
                return bs;
            }
            catch (Exception e)
            {
                return null;
            }


        }

        public static BitmapSource[] SetColorImageDef(string path)
        {
            BitmapSource[] bs = new BitmapSource[3];
            try
            {
                //DImage = ImageProcessing.LoadColorImage(path);
                //DImage.Freeze();
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(path);
                int height = bmp.Height;
                int width = bmp.Width;
                double dpi = 96;
                int stride = ((width * 8) + 7) / 8;
                if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    var bitmapData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                                     System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                    bs[0] = BitmapSource.Create(width, height, bmp.HorizontalResolution, bmp.VerticalResolution,
                                                System.Windows.Media.PixelFormats.Gray8, null, bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, stride);
                    bs[1] = BitmapSource.Create(width, height, bmp.HorizontalResolution, bmp.VerticalResolution,
                                               System.Windows.Media.PixelFormats.Gray8, null, bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, stride);
                    bs[2] = BitmapSource.Create(width, height, bmp.HorizontalResolution, bmp.VerticalResolution,
                                               System.Windows.Media.PixelFormats.Gray8, null, bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, stride);
                    bmp.UnlockBits(bitmapData);
                }
                else
                {
                    //System.Drawing.Color[][] matrix;

                    byte[] r = new byte[height * width];
                    byte[] g = new byte[height * width];
                    byte[] b = new byte[height * width];
                    int n = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            System.Drawing.Color c = bmp.GetPixel(j, i);
                            r[n] = c.R; g[n] = c.G; b[n] = c.B;
                            n++;
                        }
                    }

                    bs[0] = BitmapSource.Create(width, height, dpi, dpi, System.Windows.Media.PixelFormats.Gray8, null, r, stride);
                    bs[1] = BitmapSource.Create(width, height, dpi, dpi, System.Windows.Media.PixelFormats.Gray8, null, g, stride);
                    bs[2] = BitmapSource.Create(width, height, dpi, dpi, System.Windows.Media.PixelFormats.Gray8, null, b, stride);
                }
                // DefRImage.Source = bs[0].Clone();
                //  DefGImage.Source = bs[1].Clone();
                // DefBImage.Source = bs[2].Clone();
                return bs;
            }
            catch (Exception e)
            {
                // MessageBox.Show(e.Message);
                return null;
            }


        }

        public static BitmapSource[] SetColorImageRef(string path)
        {
            BitmapSource[] bs = new BitmapSource[3];
            try
            {
                // RImage = ImageProcessing.LoadColorImage(path);
                // RImage.Freeze();
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(path);
                //System.Drawing.Color[][] matrix;
                int height = bmp.Height;
                int width = bmp.Width;
                int stride = ((width * 8) + 7) / 8;
                if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    var bitmapData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                                     System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                    bs[0] = BitmapSource.Create(width, height, bmp.HorizontalResolution, bmp.VerticalResolution,
                                                System.Windows.Media.PixelFormats.Gray8, null, bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, stride);
                    bs[1] = BitmapSource.Create(width, height, bmp.HorizontalResolution, bmp.VerticalResolution,
                                               System.Windows.Media.PixelFormats.Gray8, null, bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, stride);
                    bs[2] = BitmapSource.Create(width, height, bmp.HorizontalResolution, bmp.VerticalResolution,
                                               System.Windows.Media.PixelFormats.Gray8, null, bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, stride);
                    bmp.UnlockBits(bitmapData);
                }
                else
                {
                    byte[] r = new byte[height * width];
                    byte[] g = new byte[height * width];
                    byte[] b = new byte[height * width];
                    int n = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            System.Drawing.Color c = bmp.GetPixel(j, i);
                            r[n] = c.R; g[n] = c.G; b[n] = c.B;
                            n++;
                        }
                    }
                    double dpi = 96;

                    bs[0] = BitmapSource.Create(width, height, dpi, dpi, System.Windows.Media.PixelFormats.Gray8, null, r, stride);
                    bs[1] = BitmapSource.Create(width, height, dpi, dpi, System.Windows.Media.PixelFormats.Gray8, null, g, stride);
                    bs[2] = BitmapSource.Create(width, height, dpi, dpi, System.Windows.Media.PixelFormats.Gray8, null, b, stride);
                }
                //RefRImage.Source = bs[0].Clone();
                //RefGImage.Source = bs[1].Clone();
                //RefBImage.Source = bs[2].Clone();
                return bs;
            }
            catch (Exception e)
            {
                // MessageBox.Show(e.Message);
                return null;
            }


        }

        /// <summary>
        /// Threshold Image from Array
        /// </summary>
        /// <param name="def">Original Reference Image</param>
        /// <param name="bsref">Reference RGB Array Image</param>
        /// <param name="bsdef">Defect RGB Array Image</param>
        /// <param name="nChannel">Colot Channels</param>
        /// <param name="nmode">Threshold Upper 0 Lower 1</param>
        /// <param name="nThres">Threshold Value</param>
        /// <param name="blob">Blob Information</param>
        /// <returns>Bitmap Source RBG Array</returns>
        public static BitmapSource[] ThresBitmapSource(BitmapSource def, BitmapSource[] bsref, BitmapSource[] bsdef, int nChannel, int nmode, int nThres, ref BlobInfo blob)
        {
            BitmapSource[] ret = new BitmapSource[8];
            int[] div = new int[] { 0, 100, 200, 255 };
            int[] div2 = new int[] { 0, 25, 50, 75, 100, 125, 150, 175, 200, 225, 255 };
            int[] divR = new int[] { 0, 0, 0, 0, 0, 0, 0, 100, 200, 255 };
            int[] divG = new int[] { 0, 0, 0, 0, 100, 200, 255, 0, 0, 0 };
            int[] divB = new int[] { 0, 255, 200, 100, 0, 0, 0, 0, 0, 0 };
            if (nmode > 1) return null;
            try
            {
                Mat rimg = OpenCvSharp.Extensions.BitmapSourceConverter.ToMat(bsref[nChannel]);
                if (nmode == 0)
                    Cv2.Threshold(rimg, rimg, nThres, 255, ThresholdTypes.Binary);
                else if (nmode == 1)
                    Cv2.Threshold(rimg, rimg, nThres, 255, ThresholdTypes.BinaryInv);

                // Cv2.ImShow("c", rimg);
                // Cv2.WaitKey(0);

                Cv2.Erode(rimg, rimg, new Mat());
                Cv2.Erode(rimg, rimg, new Mat());
                Cv2.Erode(rimg, rimg, new Mat());

                Mat dimg = OpenCvSharp.Extensions.BitmapSourceConverter.ToMat(bsdef[nChannel]);
                if (nmode == 1) Cv2.Threshold(dimg, dimg, nThres, 255, ThresholdTypes.Binary);
                else if (nmode == 0) Cv2.Threshold(dimg, dimg, nThres, 255, ThresholdTypes.BinaryInv);
                //  Cv2.ImShow("a", dimg);
                //  Cv2.WaitKey(0);
                Mat img = new Mat();
                Cv2.BitwiseAnd(rimg, dimg, img);
                // Cv2.ImShow("b", img);
                // Cv2.WaitKey(0);

                using (var labelsMat = new MatOfInt())
                using (var statsMat = new MatOfInt())
                using (var centroidsMat = new MatOfDouble())
                {
                    int nLabels = Cv2.ConnectedComponentsWithStats(
                        img, labelsMat, statsMat, centroidsMat);
                    var labels = labelsMat.ToRectangularArray();
                    var stats = statsMat.ToRectangularArray();
                    var centroids = centroidsMat.ToRectangularArray();
                    blob.Blobs = nLabels - 1;
                    if (blob.Blobs > 0)
                    {
                        int nMax = 0;
                        int nMaxArea = 0;
                        blob.Sum = 0;
                        for (int i = 1; i < nLabels; i++)
                        {
                            if (nMaxArea < stats[i, 4])
                            {
                                nMaxArea = stats[i, 4];
                                nMax = i;
                            }
                            blob.Sum += stats[i, 4];
                        }
                        blob.Width = stats[nMax, 2];
                        blob.Height = stats[nMax, 3];
                        blob.Pixels = stats[nMax, 4];
                        blob.Center = new System.Windows.Point(centroids[nMax, 0], centroids[nMax, 1]);
                    }
                }
                #region Not Used
                //var detectorParams = new SimpleBlobDetector.Params
                //{
                //    //MinDistBetweenBlobs = 10, // 10 pixels between blobs
                //    //MinRepeatability = 1,

                //    //MinThreshold = 100,
                //    //MaxThreshold = 255,
                //    //ThresholdStep = 5,

                //    FilterByArea = false,
                //    //FilterByArea = true,
                //    //MinArea = 0.001f, // 10 pixels squared
                //    //MaxArea = 500,

                //    FilterByCircularity = false,
                //    //FilterByCircularity = true,
                //    //MinCircularity = 0.001f,

                //    FilterByConvexity = false,
                //    //FilterByConvexity = true,
                //    //MinConvexity = 0.001f,
                //    //MaxConvexity = 10,

                //    FilterByInertia = false,
                //    //FilterByInertia = true,
                //    //MinInertiaRatio = 0.001f,

                //    FilterByColor = false
                //    //FilterByColor = true,
                //    //BlobColor = 255 // to extract light blobs
                //};

                //var simpleBlobDetector = SimpleBlobDetector.Create(detectorParams);
                //var keyPoints = simpleBlobDetector.Detect(img);
                //int nMaxsize = 0;
                //foreach (var keyPoint in keyPoints)
                //{
                //    if (nMaxsize < keyPoint.Size)
                //        blob.Angle = keyPoint.Angle;
                //}
                #endregion
                blob.Angle = 0;

                Mat[] tmp = new Mat[3];
                Mat[] tmp1 = new Mat[3];
                Mat mg = new Mat(img.Size(), MatType.CV_8UC3);
                mg.SetTo(new Scalar(0));
                for (int i = 0; i < 3; i++)
                {
                    tmp[i] = OpenCvSharp.Extensions.BitmapSourceConverter.ToMat(bsdef[i]);
                    #region red channel full hitmap
                    //if (i==0)
                    //{

                    //    Mat cvt2 = tmp[i].Clone();
                    //    for (int k = 0; k < 10; k++)
                    //    {
                    //        Mat tsr = new Mat();
                    //        tmp1[0] = new Mat();
                    //        tmp1[1] = new Mat();
                    //        tmp1[2] = new Mat();
                    //        Cv2.InRange(cvt2, new Scalar(div2[k]), new Scalar(div2[k + 1]), tsr);
                    //        Cv2.Threshold(tsr, tmp1[0], 1, divB[k], ThresholdTypes.Binary);
                    //        Cv2.Threshold(tsr, tmp1[1], 1, divG[k], ThresholdTypes.Binary);
                    //        Cv2.Threshold(tsr, tmp1[2], 1, divR[k], ThresholdTypes.Binary);
                    //        Mat mg2 = new Mat();
                    //        Cv2.Merge(tmp1, mg2);
                    //        Cv2.BitwiseOr(mg, mg2, mg);
                    //    }

                    //}
                    #endregion

                    Cv2.BitwiseAnd(tmp[i], img, tmp[i]);
                    Scalar s = Cv2.Mean(tmp[i], img);
                    blob.Average[i] = (int)s.Val0;
                    ret[i] = OpenCvSharp.Extensions.BitmapSourceConverter.ToBitmapSource(tmp[i]);
                    #region red channel bolb hitmap
                    if (i == 0)
                    {
                        Mat cvt2 = tmp[i].Clone();
                        for (int k = 0; k < 10; k++)
                        {
                            Mat tsr = new Mat();
                            tmp1[0] = new Mat();
                            tmp1[1] = new Mat();
                            tmp1[2] = new Mat();
                            Cv2.InRange(cvt2, new Scalar(div2[k]), new Scalar(div2[k + 1]), tsr);
                            Cv2.Threshold(tsr, tmp1[0], 1, divB[k], ThresholdTypes.Binary);
                            Cv2.Threshold(tsr, tmp1[1], 1, divG[k], ThresholdTypes.Binary);
                            Cv2.Threshold(tsr, tmp1[2], 1, divR[k], ThresholdTypes.Binary);
                            Mat mg2 = new Mat();
                            Cv2.Merge(tmp1, mg2);
                            Cv2.BitwiseOr(mg, mg2, mg);
                        }
                    }
                    #endregion

                }

                ret[3] = OpenCvSharp.Extensions.BitmapSourceConverter.ToBitmapSource(img);
                Mat dst = new Mat();
                if (def.Format == System.Windows.Media.PixelFormats.Gray8)
                {
                    Mat dst2 = OpenCvSharp.Extensions.BitmapSourceConverter.ToMat(def);
                    Cv2.CvtColor(dst2, dst, ColorConversionCodes.GRAY2RGB);
                }
                else dst = OpenCvSharp.Extensions.BitmapSourceConverter.ToMat(def);

                blob.AvgI = (int)(Cv2.Mean(dst, img).Val0);
                Mat tmp2 = new Mat();
                Cv2.CvtColor(img, tmp2, ColorConversionCodes.GRAY2RGB);
                Cv2.BitwiseAnd(dst, tmp2, dst);

                ret[4] = OpenCvSharp.Extensions.BitmapSourceConverter.ToBitmapSource(dst);
                Mat hsv = new Mat();
                Cv2.CvtColor(dst, hsv, ColorConversionCodes.RGB2HSV);
                Mat[] m = Cv2.Split(hsv);
                blob.AvgH = (int)(Cv2.Mean(m[0], img).Val0);
                blob.AvgS = (int)(Cv2.Mean(m[1], img).Val0);
                ret[5] = OpenCvSharp.Extensions.BitmapSourceConverter.ToBitmapSource(m[0]);
                ret[6] = OpenCvSharp.Extensions.BitmapSourceConverter.ToBitmapSource(m[1]);
                ret[7] = OpenCvSharp.Extensions.BitmapSourceConverter.ToBitmapSource(mg);
            }
            catch
            {
                return null;
            }

            return ret;
        }

        /// <summary>
        /// Image Blob Rect Create
        /// </summary>
        /// <param name="orgPath">Defect Origin Image</param>
        /// <param name="blobPath">Defect Blob Image</param>
        /// <param name="dix">Position X</param>
        /// <param name="diy">Position Y</param>
        /// <param name="diw">Width</param>
        /// <param name="dih">Height</param>
        /// <param name="path">Save Image Path</param>
        /// <param name="bSave">Usase Save Image</param>
        /// <returns></returns>
        public static BitmapSource Label_Train(string orgPath, string blobPath, ref double dix, ref double diy, ref double diw, ref double dih, string path, bool bSave = false)
        {
            double dx, dy;
            double dw, dh;
            double l, t, r, b;
            double w, h, x, y;
            Mat org = new Mat(orgPath);
            Mat blob1 = new Mat(blobPath);
            Mat blob = new Mat();
            Cv2.CvtColor(blob1, blob, ColorConversionCodes.BGR2GRAY);

            using (var labelsMat = new MatOfInt())
            using (var statsMat = new MatOfInt())
            using (var centroidsMat = new MatOfDouble())
            {
                int nLabels = Cv2.ConnectedComponentsWithStats(
                        blob, labelsMat, statsMat, centroidsMat);
                var labels = labelsMat.ToRectangularArray();
                var stats = statsMat.ToRectangularArray();
                var centroids = centroidsMat.ToRectangularArray();

                l = blob.Cols - 1;
                t = blob.Rows - 1;
                b = 1;
                r = 1;
                if (nLabels > 0)
                {

                    for (int i = 1; i < nLabels; i++)
                    {
                        x = centroids[i, 0];
                        y = centroids[i, 1];
                        w = stats[i, 2];
                        h = stats[i, 3];
                        if (w <= 1 || h <= 1 || x <= 1 || y <= 1) continue;
                        l = Math.Min(l, x - w / 2);
                        t = Math.Min(t, y - h / 2);
                        r = Math.Max(r, x + w / 2);
                        b = Math.Max(b, y + h / 2);

                    }
                    l = Math.Max(1, l - 7);
                    r = Math.Min(blob.Cols - 1, r + 7);
                    t = Math.Max(1, t - 7);
                    b = Math.Min(blob.Rows - 1, b + 7);
                    dw = r - l;
                    dh = b - t;
                    dx = l + dw / 2;
                    dy = t + dh / 2;
                    diw = dw / blob.Cols;
                    dih = dh / blob.Rows;
                    dix = dx / blob.Cols;
                    diy = dy / blob.Rows;
                }
                else return null;
            }

            Rect rct = new Rect((int)l, (int)t, (int)dw, (int)dh);
            Cv2.Rectangle(org, rct, Scalar.Red, 1);
            //Cv2.ImShow("fdgs", org);
            //Cv2.WaitKey(0);
            if (bSave)
            {
                string path2 = orgPath.Replace("\\Segment\\\\Defect\\", "\\tmp\\");
                System.IO.FileInfo fi = new System.IO.FileInfo(path2);
                if (!System.IO.Directory.Exists(fi.Directory.FullName)) System.IO.Directory.CreateDirectory(fi.Directory.FullName);
                Cv2.ImWrite(path2, org);
            }
            BitmapSource bs = OpenCvSharp.Extensions.BitmapSourceConverter.ToBitmapSource(org);
            bs.Freeze();
            return bs;
        }

        /// <summary>
        /// Save Image From BitmapSource
        /// </summary>
        /// <param name="bs">Bitmap Source Image</param>
        /// <param name="path">Save Path</param>
        /// <returns>Succes true Fail false</returns>
        public static bool SaveBitmapSource(BitmapSource bs, string path)
        {
            try
            {
                Mat img = OpenCvSharp.Extensions.BitmapSourceConverter.ToMat(bs);
                Cv2.ImWrite(path, img);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// PNG Image Load to Bitmap Source
        /// </summary>
        /// <param name="path">File Path</param>
        /// <returns>BitmapSource Image</returns>
        public static BitmapSource LoadPng(string path)
        {
            if (System.IO.File.Exists(path))
            {
                PngBitmapDecoder pngBitmapDecoder = new PngBitmapDecoder(new Uri(path), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                return pngBitmapDecoder.Frames[0];
            }
            return null;

        }
    }
}
