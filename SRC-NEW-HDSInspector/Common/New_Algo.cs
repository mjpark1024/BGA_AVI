using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using OpenCvSharp;

namespace Common
{
    public class New_Algo
    {
        public bool is_circle(int width, int height, int area)
        {
            //높이와 넓이의 차이가 크면 원x
            if ((double)Math.Min(width, height) / (double)Math.Max(width, height) < 0.75)
                return false;

            double c_area = Math.Pow(width / 2, 2) * Math.PI;

            //실제 넓이와 원으로 가정한 넓이의 차이가 크면 원x
            if (Math.Min(area, c_area) / Math.Max(area, c_area) < 0.75)
                return false;

            return true;
        }

        //원소재 찾기
        public int SearchRawmeterial(BitmapSource bmp, ref List<System.Windows.Point> points, ref List<int> sizes)
        {
            points = new List<System.Windows.Point>();
            sizes = new List<int>();
            points.Clear();
            sizes.Clear();
            int Raw_cnt = 0;

            Mat entireImage = OpenCvSharp.Extensions.BitmapSourceConverter.ToMat(bmp);
            Mat threshimage = new Mat();
            var mean_Val = entireImage.Mean();
            var thresh_Val = Math.Min(mean_Val.Val0 * 3, 150);//평균*3 , 150 - 이진화
            Cv2.Threshold(entireImage, threshimage, thresh_Val, 255, ThresholdTypes.Binary);
       
            using (var labelsMat = new MatOfInt())
            using (var statsMat = new MatOfInt())
            using (var centroidsMat = new MatOfDouble())
            {
                int nLabels = Cv2.ConnectedComponentsWithStats(threshimage, labelsMat, statsMat, centroidsMat);
                var labels = labelsMat.ToRectangularArray();
                var stats = statsMat.ToRectangularArray();
                var centroids = centroidsMat.ToRectangularArray();
                var blobs = new ConnectedComponents.Blob[nLabels];

                for (int j = 0; j < nLabels; j++)
                {
                    int left = stats[j, 0];
                    int top = stats[j, 1];
                    int width = stats[j, 2];
                    int height = stats[j, 3];
                    int area = stats[j, 4];

                    if (is_circle(width, height, area))//원 찾기
                    {
                        if (Math.Max(width, height) / 2 < 5)
                            continue;
                        System.Windows.Point center = new System.Windows.Point(left + width / 2, top + height / 2);
                        points.Add(center);
                        sizes.Add((int)(Math.Max(width, height) / 2 * 0.9));//반지름 저장
                        Raw_cnt++;
                    }
                }
            }
            return Raw_cnt;
        }
    }
}
