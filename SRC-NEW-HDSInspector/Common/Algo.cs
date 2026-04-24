using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Microsoft.SqlServer.Server;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Common
{
    public static class BitmapHelper
    {
        // Bitmap : C# style.
        // BitmapSource : WPF style.

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
        public static Bitmap Bitmap2Bitmap(Bitmap aBitmap, double angle)
        {
            Image<Gray, Byte> bmp = new Image<Gray, Byte>(aBitmap);
            bmp = bmp.Not();
            Gray g = bmp.GetAverage();
            bmp = bmp.ThresholdBinary(new Gray(g.Intensity * 0.95), new Gray(255));
            bmp = bmp.Erode(2);
            bmp = bmp.Dilate(2);
            bmp = bmp.Rotate(angle, new Gray(255));
            return bmp.Bitmap;
        }

        public static Bitmap BitmapSource2Bitmap(BitmapSource aBitmapSource)
        {
            if (aBitmapSource == null) return null;

            Bitmap bitmap;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(aBitmapSource));
                bitmapEncoder.Save(memoryStream);
                bitmap = new Bitmap(memoryStream);
            }

            return bitmap;
        }

        public static BitmapSource ResizeBitmapSource(BitmapSource bs, double ratio)
        {
            Mat src = bs.ToMat();
            //Image<Gray, byte> src = new Image<Gray, byte>(BitmapHelper.BitmapSource2Bitmap(bs));
            //src = src.Resize(ratio, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            //src.ToBitmap().ToBitmapSource();
            Cv2.Resize(src, src, new OpenCvSharp.Size(src.Width*ratio, src.Height*ratio));

            BitmapSource result = src.ToBitmapSource();
            return result;
        }

        public static Bitmap BitmapSource2Bitmap(BitmapSource aBitmapSource, int rotate)
        {
            if (aBitmapSource == null) return null;

            Bitmap bitmap;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(aBitmapSource));
                bitmapEncoder.Save(memoryStream);
                bitmap = new Bitmap(memoryStream);
            }

            return bitmap;
        }



        public static BitmapSource CVImageToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap

                return bs;
            }
        }
        static public int index = 0;
        public static bool SearchAlign(byte[] Image, byte[] Template, int thresh, out float x, out float y, out int match, int mode)
        {
            try
            {
                BitmapSource bs;
                System.Windows.Media.PixelFormat pf = System.Windows.Media.PixelFormats.Gray8;
                int r = (1280 * pf.BitsPerPixel + 7) / 8;
                if (mode == 0)
                {
                    r = (640 * pf.BitsPerPixel + 7) / 8;
                    bs = System.Windows.Media.Imaging.BitmapSource.Create(640, 480, 96, 96, pf, null, Image, r);
                }
                else
                {
                    bs = System.Windows.Media.Imaging.BitmapSource.Create(1280, 1024, 96, 96, pf, null, Image, r);
                }
                bs.Freeze();
                Image<Gray, byte> src = new Image<Gray, byte>(BitmapHelper.BitmapSource2Bitmap(bs));
                Rectangle rect = new Rectangle(src.Width/2 - 200 , src.Height/2 - 200, 400, 400);
                src.ROI = rect;
                //src.Save("img" + index + ".bmp");
                //index++;
                BitmapSource bs2;
                r = (40 * pf.BitsPerPixel + 7) / 8;
                bs2 = System.Windows.Media.Imaging.BitmapSource.Create(40, 40, 96, 96, pf, null, Template, r);
                bs2.Freeze();
                Image<Gray, byte> template = new Image<Gray, byte>(BitmapHelper.BitmapSource2Bitmap(bs2));
                Gray g = src.GetAverage();
                //index++;
                //src.Save("img" + index + ".bmp");
                //index++;
                //template.Save("img" + index + ".bmp");

                //CvInvoke.cvThreshold(src, src, Math.Min(180, g.Intensity + 20), 255, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY);
                //CvInvoke.cvThreshold(template, template, Math.Min(180, g.Intensity + 20), 255, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY);

                using (Image<Gray, float> result = src.MatchTemplate(template, Emgu.CV.CvEnum.TM_TYPE.CV_TM_CCORR_NORMED))
                {
                    double[] minValues, maxValues;
                    System.Drawing.Point[] minLocations, maxLocations;
                    result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                    // 맥스 밸류 값을 변경해가면서 TEST 할 것
                    x = (float)((float)(maxLocations[0].X + template.Width / 2 - src.Width / 2));
                    y = (float)((float)(maxLocations[0].Y + template.Height / 2 - src.Height / 2));
                    match = (int)(maxValues[0] * 100.0);
                }
                return true;

            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                x = y = 0;
                match = 0;
                return false;
            }
        }
    }

    public class Algo
    {
        private Image<Gray, Byte> m_imgEntireImage;
        private Image<Gray, Byte> m_imgBufferImage;
        private Image<Gray, Byte> m_imgTemplateImage;

        Rectangle m_rcImageROI;

        private double m_fMaxScore = 0;

        private System.Drawing.Point m_ptMaxPos = new System.Drawing.Point();
        private System.Drawing.Point m_ptOffset = new System.Drawing.Point();

        public System.Drawing.Point m_ptStartOffset = new System.Drawing.Point();

        #region Property (MaxScore, Offset)
        public double MaxScore
        {
            get
            {
                return m_fMaxScore;
            }
            set
            {
                m_fMaxScore = value;
            }
        }

        public System.Drawing.Point Offset
        {
            get
            {
                return m_ptOffset;
            }
            set
            {
                m_ptOffset = value;
            }
        }
        #endregion

        public void XOffsetSetZero()
        {
            m_ptOffset.X = 0;
        }

        public void YOffsetSetZero()
        {
            m_ptOffset.Y = 0;
        }

        public bool SetImage(Bitmap aBitmap)
        {
            if (aBitmap == null)
                return false;
            try
            {
                if (m_imgEntireImage != null)
                    m_imgEntireImage.Dispose();
                m_imgEntireImage = new Image<Gray, Byte>(aBitmap);
                m_imgBufferImage = m_imgEntireImage.Clone();

                return (m_imgEntireImage != null) && (m_imgBufferImage != null);
            }
            catch
            {
                return false;
            }
        }

        public BitmapSource GetROIImage(BitmapSource bs, Rectangle aROI)
        {
            Bitmap aBitmap = BitmapHelper.BitmapSource2Bitmap(bs);
            m_imgEntireImage = new Image<Gray, Byte>(aBitmap);
            Image<Gray, byte> tmp = new Image<Gray, byte>(aROI.Width, aROI.Height);
            Emgu.CV.CvInvoke.cvSetImageROI(m_imgEntireImage, aROI);
            Emgu.CV.CvInvoke.cvCopy(m_imgEntireImage, tmp, System.IntPtr.Zero);
            return BitmapHelper.CVImageToBitmapSource(tmp);

        }

        public BitmapSource SetROIImage(BitmapSource bs, BitmapSource ds, Rectangle aROI)
        {
            /////붙여넣기로 수정
            Bitmap bmpSrc = BitmapHelper.BitmapSource2Bitmap(bs);
            Bitmap bmpDst = BitmapHelper.BitmapSource2Bitmap(ds);
            m_imgEntireImage = new Image<Gray, Byte>(bmpSrc);
            m_imgBufferImage = m_imgEntireImage.Clone();
            m_imgTemplateImage = new Image<Gray, Byte>(bmpDst);

            #region Search
            Rectangle rectSearchROI = new Rectangle((int)Math.Max(aROI.X - 20, 0), (int)Math.Max(aROI.Y - 20, 0),
                                                   (int)Math.Min(aROI.Width + 40, bs.Width), (int)Math.Min(aROI.Height + 40, bs.Height));

            Image<Gray, float> imgResult = null;
            try
            {
                System.Drawing.Size nResultSize = new System.Drawing.Size((int)(rectSearchROI.Width - m_imgTemplateImage.Bitmap.Width + 1), (int)(rectSearchROI.Height - m_imgTemplateImage.Bitmap.Height + 1));


                imgResult = new Image<Gray, float>(nResultSize);
                Emgu.CV.CvInvoke.cvSetImageROI(m_imgBufferImage, rectSearchROI);
                Emgu.CV.CvInvoke.cvMatchTemplate(m_imgBufferImage, m_imgTemplateImage, imgResult, Emgu.CV.CvEnum.TM_TYPE.CV_TM_CCORR_NORMED);
                Emgu.CV.CvInvoke.cvResetImageROI(m_imgBufferImage);
                double[] fMinVal;
                double[] fMaxVal;
                System.Drawing.Point[] ptMinLocation;
                System.Drawing.Point[] ptMaxLocation;
                imgResult.MinMax(out fMinVal, out fMaxVal, out ptMinLocation, out ptMaxLocation);

                m_fMaxScore = fMaxVal[0];
                m_ptMaxPos = ptMaxLocation[0];

                if (m_ptMaxPos.X <= 0) m_ptMaxPos.X = 1;
                if (m_ptMaxPos.Y <= 0) m_ptMaxPos.Y = 1;
                if (m_ptMaxPos.X >= imgResult.Width - 1) m_ptMaxPos.X = imgResult.Width - 2;
                if (m_ptMaxPos.Y >= imgResult.Height - 1) m_ptMaxPos.Y = imgResult.Height - 2;

                //m_imgEntireImage.ROI = rectOldROI;
                m_ptStartOffset.X = m_ptMaxPos.X;

                m_ptOffset.X = m_ptMaxPos.X - imgResult.Width / 2;
                m_ptOffset.Y = m_ptMaxPos.Y - imgResult.Height / 2;
                m_ptStartOffset.Y = m_ptOffset.Y;
                // Resource 반환.
                imgResult.Dispose();
                imgResult = null;

                if (m_fMaxScore >= 0.85)
                {
                    aROI.X += m_ptOffset.X;
                    aROI.Y += m_ptOffset.Y;
                }
                m_imgBufferImage.ROI = aROI;
                Image<Gray, Byte> tmp = m_imgBufferImage.Add(m_imgTemplateImage);
                m_imgTemplateImage.CopyTo(m_imgBufferImage);
                m_imgBufferImage.ROI = Rectangle.Empty;
                // Emgu.CV.CvInvoke.cvShowImage("", m_imgBufferImage);
                // Emgu.CV.CvInvoke.cvWaitKey(0);
                return BitmapHelper.CVImageToBitmapSource(m_imgBufferImage);
            }
            #endregion
            catch
            {
                if (imgResult != null)
                {
                    imgResult.Dispose();
                }
                return null;
            }
        }

        public bool SetImageROI(Rectangle aROI)
        {
            if ((m_imgEntireImage == null) || (m_imgBufferImage == null))
                return false;

            m_rcImageROI = aROI;

            return true;
        }

        public bool ResetImageROI()
        {
            if (m_imgBufferImage == null)
                return false;

            // m_rcImageROI = new Rectangle(0, 0, m_imgBufferImage.MIplImage.width, m_imgBufferImage.MIplImage.height);
            Emgu.CV.CvInvoke.cvResetImageROI(m_imgBufferImage);

            return true;
        }

        public int SearchRawmeterial(BitmapSource bmp, ref List<System.Windows.Point> points, ref List<int> sizes)
        {
            int nCnt = 0;
            Gray g = new Gray();
            MCvScalar s = new MCvScalar();
            SetImage(BitmapHelper.BitmapSource2Bitmap(bmp));
            m_imgBufferImage.AvgSdv(out g, out s);
            int th = 100;// (int)g.Intensity + 30;
            int stride = bmp.PixelWidth * ((bmp.Format.BitsPerPixel + 7) / 8);
            byte[] data = new byte[bmp.PixelWidth * bmp.PixelHeight];
            bmp.CopyPixels(data, stride, 0);
            points = new List<System.Windows.Point>();
            sizes = new List<int>();
            points.Clear();
            sizes.Clear();
            int searchX = 700;
            int searchY = 30;
            for (int y = searchY; y < bmp.PixelHeight; y++)
            {
                for (int x = 0; x < searchX; x++)
                {
                    if (data[x + (y * bmp.PixelWidth)] > th)
                    {
                        int j = y;
                        int w1 = 0;
                        for (int i = x; i < searchX; i++)
                        {
                            if (j >= bmp.PixelHeight) break;
                            if (data[i + j * bmp.PixelWidth] > th)
                                w1++;
                            else break;
                            j++;
                        }
                        j = y;
                        int w2 = 0;
                        for (int i = x; i >= 0; i--)
                        {
                            if (j < 0) break;
                            if (data[i + j * bmp.PixelWidth] > th)
                                w2++;
                            else break;
                            j--;
                        }
                        if (Math.Abs(w1 - w2) < 4 && w1 > 30)
                        {
                            j = x;
                            int h1 = 0;
                            for (int i = y; i < bmp.PixelHeight; i++)
                            {
                                if (j < 0) break;
                                if (data[j + i * bmp.PixelWidth] > th)
                                    h1++;
                                else break;
                                j--;
                            }
                            j = x;
                            int h2 = 0;
                            for (int i = y; i >= 0; i--)
                            {
                                if (j >= searchX) break;
                                if (data[j + i * bmp.PixelWidth] > th)
                                    h2++;
                                else break;
                                j++;
                            }
                            if (Math.Abs(h1 - h2) < 4)
                            {
                                int min = Math.Min(Math.Min(Math.Min(w1, w2), h1), h2);
                                int max = Math.Max(Math.Max(Math.Max(w1, w2), h1), h2);
                                int r = (int)(min * Math.Sqrt(2.0));
                                if (Math.Abs(min - max) < 8 && r > 40)
                                {
                                    System.Windows.Point p = new System.Windows.Point(x, y);
                                    points.Add(p);
                                    sizes.Add(r);
                                    nCnt++;
                                    y += r + 10;
                                    break;
                                }
                            }
                        }
                    }
                    //m_imgBufferImage.AvgSdv(
                }
            }
            // m_imgBufferImage.Bitmap.GetPixel(
            return nCnt;
        }

        public byte[] GetAlignImage(Rectangle aROI)//, ref int w, ref int h)
        {
            Image<Gray, byte> tmp = new Image<Gray, byte>(aROI.Width, aROI.Height);
            Emgu.CV.CvInvoke.cvSetImageROI(m_imgBufferImage, aROI);
            Emgu.CV.CvInvoke.cvCopy(m_imgBufferImage, tmp, System.IntPtr.Zero);
            Emgu.CV.CvInvoke.cvResetImageROI(m_imgBufferImage);
            //BitmapSource bmp = BitmapHelper.CVImageToBitmapSource(tmp);
            //tmp.Save("d:\\a-1.bmp");
            //return bmp;
            byte[] b = tmp.Bytes;
            //w = tmp.Width;
            //h = tmp.Height;
            return b;
        }

        public bool SetTemplateImage(BitmapSource src, double scale)
        {
            try
            {
                Image<Gray, Byte> image;
                Bitmap bmp = BitmapHelper.BitmapSource2Bitmap(src);
                image = new Image<Gray, Byte>(bmp);
                m_imgTemplateImage = image.Resize(scale, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool SetTemplateImage(Rectangle aROI)
        {
            if (m_imgBufferImage == null)
                return false;

            if (!SetImageROI(aROI))
                return false;

            m_imgTemplateImage = new Image<Gray, byte>(m_rcImageROI.Width, m_rcImageROI.Height);
            Emgu.CV.CvInvoke.cvSetImageROI(m_imgBufferImage, m_rcImageROI);
            Emgu.CV.CvInvoke.cvCopy(m_imgBufferImage, m_imgTemplateImage, System.IntPtr.Zero);
            Emgu.CV.CvInvoke.cvResetImageROI(m_imgBufferImage);

            ResetImageROI();

            return true;
        }

        // Template Image를 기준으로 Matching 작업을 수행한다.
        public bool SearchTemplateImage(System.Drawing.Point aptPosition /* Search 시작 좌표 */,
                                        System.Drawing.Point aptSearchMargin /* Search Margin */,
                                        double afMinCorr /* 최소 허용 일치율 */,
                                        System.Drawing.Point aptOffset = new System.Drawing.Point() /* Offset, 기본값 (0,0) */)
        {
            Rectangle rectSearchROI = new Rectangle(aptPosition.X - aptSearchMargin.X, aptPosition.Y - aptSearchMargin.Y,
                                                    m_imgTemplateImage.Width + aptSearchMargin.X * 2, m_imgTemplateImage.Height + aptSearchMargin.Y * 2);

            return SearchTemplateImage(rectSearchROI, afMinCorr, aptOffset);
        }



        public bool SearchTemplateImage(Rectangle aRectSearchROI, double afMinCorr, System.Drawing.Point aptOffset = new System.Drawing.Point())
        {
            Image<Gray, float> imgResult = null;
            try
            {
                aRectSearchROI.X += aptOffset.X;
                aRectSearchROI.Y += aptOffset.Y;
                int tmpOffset = 0;
                if (aRectSearchROI.X < 0)
                {
                    tmpOffset = aRectSearchROI.X;
                    aRectSearchROI.X = 0;
                }
                System.Drawing.Size nResultSize = new System.Drawing.Size(aRectSearchROI.Width - m_imgTemplateImage.Bitmap.Width + 1, aRectSearchROI.Height - m_imgTemplateImage.Bitmap.Height + 1);

                //                 System.Drawing.Point ptCropSize = new System.Drawing.Point();
                //                 System.Drawing.Point ptMinusPos = new System.Drawing.Point();
                //                 ptCropSize.X = aRectSearchROI.Width;
                //                 ptCropSize.Y = aRectSearchROI.Height;
                //                 ptMinusPos.X = -aRectSearchROI.Location.X;
                //                 ptMinusPos.Y = -aRectSearchROI.Location.Y;
                // 
                //                 Rectangle rectOldROI = m_imgEntireImage.ROI;
                //                 aRectSearchROI.Intersect(rectOldROI);
                // 
                //                 ptCropSize.X -= aRectSearchROI.Width;
                //                 ptCropSize.Y -= aRectSearchROI.Height;
                //                 ptMinusPos.X += aRectSearchROI.Location.X;
                //                 ptMinusPos.Y += aRectSearchROI.Location.Y;

                imgResult = new Image<Gray, float>(nResultSize);
                Emgu.CV.CvInvoke.cvSetImageROI(m_imgBufferImage, aRectSearchROI);
                Emgu.CV.CvInvoke.cvMatchTemplate(m_imgBufferImage, m_imgTemplateImage, imgResult, Emgu.CV.CvEnum.TM_TYPE.CV_TM_CCORR_NORMED);
                Emgu.CV.CvInvoke.cvResetImageROI(m_imgBufferImage);

                //                 m_imgEntireImage.ROI = aRectSearchROI;
                // 
                //                 imgResult = m_imgBufferImage.MatchTemplate(m_imgTemplateImage, Emgu.CV.CvEnum.TM_TYPE.CV_TM_CCORR_NORMED);

                double[] fMinVal;
                double[] fMaxVal;
                System.Drawing.Point[] ptMinLocation;
                System.Drawing.Point[] ptMaxLocation;
                imgResult.MinMax(out fMinVal, out fMaxVal, out ptMinLocation, out ptMaxLocation);

                m_fMaxScore = fMaxVal[0];
                m_ptMaxPos = ptMaxLocation[0];

                if (m_ptMaxPos.X <= 0) m_ptMaxPos.X = 1;
                if (m_ptMaxPos.Y <= 0) m_ptMaxPos.Y = 1;
                if (m_ptMaxPos.X >= imgResult.Width - 1) m_ptMaxPos.X = imgResult.Width - 2;
                if (m_ptMaxPos.Y >= imgResult.Height - 1) m_ptMaxPos.Y = imgResult.Height - 2;

                //m_imgEntireImage.ROI = rectOldROI;
                m_ptStartOffset.X = m_ptMaxPos.X;

                m_ptOffset.X = m_ptMaxPos.X - imgResult.Width / 2 + aptOffset.X - tmpOffset;
                m_ptOffset.Y = m_ptMaxPos.Y - imgResult.Height / 2 + aptOffset.Y;
                m_ptStartOffset.Y = m_ptOffset.Y;
                // Resource 반환.
                imgResult.Dispose();
                imgResult = null;

                return (m_fMaxScore >= afMinCorr);
            }
            catch
            {
                if (imgResult != null)
                {
                    imgResult.Dispose();
                }
                return false;
            }
        }

        // Y축 기준의 Profile을 추출한다.
        public float[] GetVerticalProfile(int anStartLine, int anEndLine)
        {
            try
            {
                if (m_imgBufferImage == null)
                    return null;

                int nHeight = anEndLine - anStartLine + 1;
                float[] arrVerticalProfile = new float[nHeight];

                if (nHeight < 1 || nHeight > m_imgBufferImage.Height || anStartLine < 0 ||
                    anEndLine < anStartLine || anStartLine > m_imgBufferImage.Height || anEndLine > m_imgBufferImage.Height)
                    return null;

                Rectangle rectROI = new Rectangle(0, 0, m_imgBufferImage.Width, 1);
                Image<Gray, Byte> imgLine = new Image<Gray, Byte>(m_imgBufferImage.Width, 1);
                for (int y = anStartLine; y < anEndLine; y++)
                {
                    rectROI.Y = y;
                    Emgu.CV.CvInvoke.cvGetSubRect(m_imgBufferImage, imgLine, rectROI);
                    MCvScalar sum = Emgu.CV.CvInvoke.cvSum(imgLine);
                    arrVerticalProfile[y - anStartLine] = (float)sum.v0;
                }
                return arrVerticalProfile;
            }
            catch
            {
                return null;
            }
        }

        public float[] GetVerticalProfile(int anStartX, int anEndX, int anStartY, int anEndY)
        {
            try
            {
                if (m_imgBufferImage == null)
                    return null;

                int nHeight = anEndY - anStartY + 1;
                float[] arrVerticalProfile = new float[nHeight];

                if (nHeight < 1 || nHeight > m_imgBufferImage.Height ||
                    anStartX < 0 || anStartX < 0 ||
                    anEndX < anStartX || anEndY < anStartY ||
                    anStartX > m_imgBufferImage.Width ||
                    anStartY > m_imgBufferImage.Height ||
                    anEndX > m_imgBufferImage.Width ||
                    anEndY > m_imgBufferImage.Height)
                    return null;

                Rectangle rectROI = new Rectangle(anStartX, anStartY, anEndX - anStartX, 1);
                Image<Gray, Byte> imgLine = new Image<Gray, Byte>(anEndX - anStartX, 1);
                for (int y = anStartY; y <= anEndY; y++)
                {
                    rectROI.Y = y;
                    Emgu.CV.CvInvoke.cvGetSubRect(m_imgBufferImage, imgLine, rectROI);
                    MCvScalar sum = Emgu.CV.CvInvoke.cvSum(imgLine);
                    arrVerticalProfile[y - anStartY] = (float)sum.v0;
                }
                return arrVerticalProfile;
            }
            catch
            {
                return null;
            }
        }

        // X축 기준의 Profile을 추출한다.
        public float[] GetHorizontalProfile(int anStartLine, int anEndLine)
        {
            try
            {
                if (m_imgBufferImage == null)
                    return null;

                int nWidth = anEndLine - anStartLine + 1;
                float[] arrHorizontalProfile = new float[nWidth];

                if (nWidth < 1 || nWidth > m_imgBufferImage.Width || anStartLine < 0 ||
                    anEndLine < anStartLine || anStartLine > m_imgBufferImage.Width || anEndLine > m_imgBufferImage.Width)
                    return null;

                Rectangle rectROI = new Rectangle(0, 0, 1, m_imgBufferImage.Height);
                Image<Gray, Byte> imgLine = new Image<Gray, Byte>(1, m_imgBufferImage.Height);
                for (int x = anStartLine; x < anEndLine; x++)
                {
                    rectROI.X = x;
                    Emgu.CV.CvInvoke.cvGetSubRect(m_imgBufferImage, imgLine, rectROI);
                    MCvScalar sum = Emgu.CV.CvInvoke.cvSum(imgLine);
                    arrHorizontalProfile[x - anStartLine] = (float)sum.v0;
                }
                return arrHorizontalProfile;
            }
            catch
            {
                return null;
            }
        }

        public float[] GetHorizontalProfile(int anStartX, int anEndX, int anStartY, int anEndY)
        {
            try
            {
                if (m_imgBufferImage == null)
                    return null;

                int nWidth = anEndX - anStartX + 1;
                float[] arrHorizontalProfile = new float[nWidth];

                if (nWidth < 1 || nWidth > m_imgBufferImage.Width || anStartX < 0 || anStartX < 0 ||
                    anEndX < anStartX || anEndY < anStartY || anStartX > m_imgBufferImage.Width ||
                    anStartY > m_imgBufferImage.Height || anEndX > m_imgBufferImage.Width || anEndY > m_imgBufferImage.Height)
                    return null;

                Rectangle rectROI = new Rectangle(anStartX, anStartY, 1, anEndY - anStartY);
                Image<Gray, Byte> imgLine = new Image<Gray, Byte>(1, anEndY - anStartY);
                for (int x = anStartX; x <= anEndX; x++)
                {
                    rectROI.X = x;
                    Emgu.CV.CvInvoke.cvGetSubRect(m_imgBufferImage, imgLine, rectROI);
                    MCvScalar sum = Emgu.CV.CvInvoke.cvSum(imgLine);
                    arrHorizontalProfile[x - anStartX] = (float)sum.v0;
                }
                return arrHorizontalProfile;
            }
            catch
            {
                return null;
            }
        }

        public void DoProcessing(int anLowerThreshold, int anUpperThreshold, int anErodeIteration, int anDilateIteration)
        {
            // sync : Emgu.CV.CvInvoke.cvCopy(m_imgEntireImage, m_imgBufferImage, IntPtr.Zero);
            Emgu.CV.CvInvoke.cvSetImageROI(m_imgBufferImage, m_rcImageROI);
            Emgu.CV.CvInvoke.cvInRangeS(m_imgBufferImage, new MCvScalar(anLowerThreshold), new MCvScalar(anUpperThreshold), m_imgBufferImage);
            if (anErodeIteration > 0)
                Emgu.CV.CvInvoke.cvErode(m_imgBufferImage, m_imgBufferImage, IntPtr.Zero, anErodeIteration);
            if (anDilateIteration > 0)
                Emgu.CV.CvInvoke.cvDilate(m_imgBufferImage, m_imgBufferImage, IntPtr.Zero, anDilateIteration);
        }

        //public BitmapSource Threshold(int anLowerThreshold, int anUpperThreshold)
        //{
        //    Emgu.CV.CvInvoke.cvSetImageROI(m_imgBufferImage, m_rcImageROI);
        //    Emgu.CV.CvInvoke.cvInRangeS(m_imgBufferImage, new MCvScalar(anLowerThreshold), new MCvScalar(anUpperThreshold), m_imgBufferImage);
        //    Emgu.CV.CvInvoke.cvResetImageROI(m_imgBufferImage);

        //    //return BitmapHelper.CVImageToBitmapSource(m_imgBufferImage);
        //}

        //public BitmapSource Erode(int anIteration)
        //{
        //    Emgu.CV.CvInvoke.cvSetImageROI(m_imgBufferImage, m_rcImageROI);
        //    Emgu.CV.CvInvoke.cvErode(m_imgBufferImage, m_imgBufferImage, IntPtr.Zero, anIteration);
        //    Emgu.CV.CvInvoke.cvResetImageROI(m_imgBufferImage);

        //    return BitmapHelper.CVImageToBitmapSource(m_imgBufferImage);
        //}

        //public BitmapSource Dilate(int anIteration)
        //{
        //    Emgu.CV.CvInvoke.cvSetImageROI(m_imgBufferImage, m_rcImageROI);
        //    Emgu.CV.CvInvoke.cvDilate(m_imgBufferImage, m_imgBufferImage, IntPtr.Zero, anIteration);
        //    Emgu.CV.CvInvoke.cvResetImageROI(m_imgBufferImage);

        //    return BitmapHelper.CVImageToBitmapSource(m_imgBufferImage);
        //}

        public BitmapSource GetImage()
        {
            return BitmapHelper.CVImageToBitmapSource(m_imgBufferImage);
        }

        public static BitmapSource LoadImage(string filepath)
        {
            PixelFormat format;
            using (Bitmap bmp = new Bitmap(filepath))
            {
                format = bmp.PixelFormat;
            }

            if(format == PixelFormat.Format24bppRgb)
                return OpenCvSharp.Extensions.BitmapSourceConverter.ToBitmapSource(Cv2.ImRead(filepath, ImreadModes.Color));
            
            return OpenCvSharp.Extensions.BitmapSourceConverter.ToBitmapSource(Cv2.ImRead(filepath, ImreadModes.Grayscale));

        }

        public bool Save(String aszFilePath)
        {
            m_imgBufferImage.Save(aszFilePath);

            return true;
        }
    }
}
