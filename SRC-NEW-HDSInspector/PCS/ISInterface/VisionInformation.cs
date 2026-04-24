/*********************************************************************************
 * Copyright(c) 2015 by Haesung DS.
 * 
 * This software is copyrighted by, and is the sole property of Haesung DS.
 * All rigths, title, ownership, or other interests in the software remain the
 * property of Haesung DS. This software may only be used in accordance with
 * the corresponding license agreement. Any unauthorized use, duplication, 
 * transmission, distribution, or disclosure of this software is expressly 
 * forbidden.
 *
 * This Copyright notice may not be removed or modified without prior written
 * consent of Haesung DS reserves the right to modify this 
 * software without notice.
 *
 * Haesung DS.
 * KOREA 
 * http://www.HaesungDS.com
 *********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCS
{
    // 2012-03-22, suoow2 Added.
    public class VisionInformation
    {
        public int GrabberType;                // [0] +GrabberType (Define : eVisGrabber)
        public int DeviceNum;                  // [1] +Device Number
        public string DeviceName;              // [2] +Device Name
        public string CamFile;                 // [3] +Cam File Name or Bitmap File Name
        public int GrabWidth;                  // [4] +Grab Image Size(Width)
        public int GrabHeight;                 // [5] +Grab Image Size(Height)
        public int GrabDepth;                  // [6] +Image Color Depth(Define : eVisImageDepth)
        public int GrabCount;                  // [7] +Grab Buffer Count
        public int CropHeight;                 // [8] +Crop Image Height
        public int PageDelay;                  // [9] +PageDelay(=Trigger Delay)
        public int InspWidth;                  // [10] +Inspection Buffer Width
        public int InspHeight;                 // [11] +Inspection Buffer Height
        public int InspDepth;                  // [12] +Inspection Buffer Depth
        public int InspCount;                  // [13] +Inspection Buffer Count
        public int Resolution;
        public int LimitResultImages;          // [14] +최대 불량 개수
        public double FilterEnable;
        public int RGBIndex;

        public bool SetVisionInfo(int anGrabberType, int anGrabWidth, int anGrabHeight, int anGrabDepth, int anGrabCount, int anDeviceNum, string aszDeviceName, string aszCamFile, int anCropHeight, int anPageDelay, double afResolution, double anFilterEnable, int anRGBIndex = 0, int anLimitResultImages = 64)
        {
            GrabberType = anGrabberType;
            GrabWidth = anGrabWidth;
            GrabHeight = (anGrabHeight + anCropHeight * 2) / anGrabCount;
            GrabDepth = anGrabDepth;
            GrabCount = anGrabCount;

            // ELF AVI = GrabXXXX와 InspXXXX 동일 값.
            InspWidth = anGrabWidth;
            InspHeight = (anGrabHeight + anCropHeight * 2);
            InspDepth = anGrabDepth;
            InspCount = 1;

            DeviceNum = anDeviceNum;
            DeviceName = aszDeviceName;
            CamFile = aszCamFile;
            CropHeight = anCropHeight;
            PageDelay = anPageDelay;
            Resolution = (int)(afResolution * 10);
            FilterEnable = anFilterEnable;
            RGBIndex = anRGBIndex;
            LimitResultImages = anLimitResultImages;

            return true;
        }

        public byte[] GetVisionInfo()
        {
            byte[] arrResult = new Byte[VisionParameter.VisionInfoSize];

            if (string.IsNullOrEmpty(DeviceName))
                DeviceName = "System";
            if (string.IsNullOrEmpty(CamFile))
                CamFile = "C:\\";

            //  BitConverter.GetBytes(CameraType).CopyTo(arrResult, sizeof(int) * 0);
            BitConverter.GetBytes(GrabberType).CopyTo(arrResult, sizeof(int) * 0);
            BitConverter.GetBytes(DeviceNum).CopyTo(arrResult, sizeof(int) * 1);
            Encoding.Default.GetBytes(DeviceName).CopyTo(arrResult, sizeof(int) * 2);
            Encoding.Default.GetBytes(CamFile).CopyTo(arrResult, sizeof(int) * 2 + 256);
            BitConverter.GetBytes(GrabWidth).CopyTo(arrResult, sizeof(int) * 2 + 256 * 2);
            BitConverter.GetBytes(GrabHeight).CopyTo(arrResult, sizeof(int) * 3 + 256 * 2);
            BitConverter.GetBytes(GrabDepth).CopyTo(arrResult, sizeof(int) * 4 + 256 * 2);
            BitConverter.GetBytes(GrabCount).CopyTo(arrResult, sizeof(int) * 5 + 256 * 2);
            BitConverter.GetBytes(CropHeight).CopyTo(arrResult, sizeof(int) * 6 + 256 * 2);
            BitConverter.GetBytes(PageDelay).CopyTo(arrResult, sizeof(int) * 7 + 256 * 2);
            BitConverter.GetBytes(InspWidth).CopyTo(arrResult, sizeof(int) * 8 + 256 * 2);
            BitConverter.GetBytes(InspHeight).CopyTo(arrResult, sizeof(int) * 9 + 256 * 2);
            BitConverter.GetBytes(InspDepth).CopyTo(arrResult, sizeof(int) * 10 + 256 * 2);
            BitConverter.GetBytes(InspCount).CopyTo(arrResult, sizeof(int) * 11 + 256 * 2);
            BitConverter.GetBytes(Resolution).CopyTo(arrResult, sizeof(int) * 12 + 256 * 2);
            BitConverter.GetBytes(LimitResultImages).CopyTo(arrResult, sizeof(int) * 13 + 256 * 2);
            BitConverter.GetBytes(FilterEnable).CopyTo(arrResult, sizeof(int) * 14 + 256 * 2);
            BitConverter.GetBytes(RGBIndex).CopyTo(arrResult, sizeof(int) * 16 + 256 * 2);

            return arrResult;
        }
    }
}
