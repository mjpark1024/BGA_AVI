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
/**
 * @file  ResolutionHelper.cs
 * @brief 
 *  ELF 설비의 경우 장비 별로 카메라 해상도가 다르다. 
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2012.04.03
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2012.04.03 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.DataBase;
using System.Data;
using System.Diagnostics;

// ELF AVI 설비 사양
// Type 1 : 16 Micron 해상도
// 대상 설비 : EAV001, EAV002, EAV003, EAV004, EAV005, EAV006, EAV007, EAV009, EAV014, EAV015, EAV016

// Type 2 : 10 Micron 해상도p
// 대상 설비 : EAV010, EAV011

// Type 3 : 12 Micron 해상도 (레이저 마킹 설비)
// 대상 설비 : EAV013, EAV017, EAV018, EAV019

// Type 4 : 12 Micron 해상도 (8 Micron 가변)
// 대상 설비 : EAV012, EAV020, EAV021, EAV022

// Type 5 : 12 Micron 해상도 
// 대상 설비 : EAV008(투과 카메라 없음)

namespace PCS.ModelTeaching.OfflineTeaching
{
    public static class ResolutionHelper
    {
        private static readonly double DEFAULT_RESOLUTION = 12.0;

        public static double GetCameraResolution(string aszMachineCode)
        {
            int nMachineType = 0;
            if (int.TryParse(aszMachineCode, out nMachineType))
            {
                nMachineType = Math.Min(nMachineType, 5); // Max Machine Type is 5.
                // Camera 해상도를 반환한다.
                return GetCameraResolution(nMachineType);
            }
            else
            {
                return DEFAULT_RESOLUTION; // Default Value;
            }
        }

        // Machine Type 구분에 따른 카메라 해상도를 반환한다.
        private static double GetCameraResolution(int anMachineType)
        {
            double fResolution = DEFAULT_RESOLUTION; // Default Value.
            switch (anMachineType)
            {
                case 1:
                    fResolution = 16.0;
                    break;
                case 2:
                    fResolution = 10.0;
                    break;
                case 3:
                    fResolution = 12.0;
                    break;
                case 4:
                    fResolution = 12.0;
                    break;
                case 5:
                    fResolution = 12.0;
                    break;
            }

            return fResolution;
        }
    }
}
