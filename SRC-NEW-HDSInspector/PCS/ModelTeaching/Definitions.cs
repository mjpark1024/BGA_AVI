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
 * @file  Definitions.cs
 * @brief 
 *  Definitions concern about MTS.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.10.18
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.10.18 First creation.
 */

using System;
using Common;

namespace PCS.ModelTeaching
{
    /// <summary>   Section type code.  </summary>
    /// <remarks>   suoow2, 2014-10-18. </remarks>
    public static class SectionTypeCode
    {
        public static string STRIP_REGION = "0081"; // Strip Align Code.
        public static string UNIT_REGION = "0082"; // Unit Region Code.
        public static string OUTER_REGION = "0083"; // Outer Region Code.
        public static string RAW_REGION = "0084"; // Raw Region Code.
        public static string ID_REGION = "0085"; // ID Region Code.
        public static string PSR_REGION = "0086"; // PSR Region Code.
    }

    /// <summary>   Section type.  </summary>
    /// <remarks>   suoow2, 2014-09-09. </remarks>
    public class SectionType
    {
        public SectionType(string aszCode, string aszName) // 생성시점에 Code, Name을 결정하도록 강제한다.
        {
            Code = aszCode;
            Name = aszName;
        }

        // Section Code.
        public string Code
        {
            get;
            private set;
        }

        // Section Name.
        public string Name
        {
            get;
            private set;
        }
    }

    /// <summary>   Work type code.  </summary>
    /// <remarks>   suoow2, 2014-10-18. </remarks>
    public static class WorkTypeCode
    {
        public static string WORK_ALL = "9001"; // Work All Code.      
        public static string WORK_BONDPAD1 = "9011"; // Work BondPad1 Code.
        public static string WORK_BONDPAD2 = "9012"; // Work BondPad2 Code.
        public static string WORK_BONDPAD3 = "9013"; // Work BondPad1 Code.
        public static string WORK_BONDPAD4 = "9014"; // Work BondPad2 Code.
        public static string WORK_TOPSUR1 = "9021"; // Work TOP Code.
        public static string WORK_TOPSUR2 = "9022"; // Work TOP Code.
        public static string WORK_TOPSUR3 = "9023"; // Work TOP Code. 
        public static string WORK_TOPSUR4 = "9024"; // Work TOP Code.
        public static string WORK_BOTSUR1 = "9031"; // Work Bottom Code.
        public static string WORK_BOTSUR2 = "9032"; // Work Bottom Code. 
        public static string WORK_BOTSUR3 = "9033"; // Work Bottom Code. 
        public static string WORK_BOTSUR4 = "9034"; // Work Bottom Code. 

        public static string GetWorkTypeCode(Surface aSurface)
        {
            switch (aSurface)
            {
                case Surface.BA1:
                    return WORK_BOTSUR1;
                case Surface.BA2:
                    return WORK_BOTSUR2;
                case Surface.BA3:
                    return WORK_BOTSUR3;
                case Surface.BA4:
                    return WORK_BOTSUR4;
                case Surface.BP1:
                    return WORK_BONDPAD1;
                case Surface.BP2:
                    return WORK_BONDPAD2;
                case Surface.CA1:
                    return WORK_TOPSUR1;
                case Surface.CA2:
                    return WORK_TOPSUR2;
                case Surface.CA3:
                    return WORK_TOPSUR3;
                case Surface.CA4:
                    return WORK_TOPSUR4;
                default:
                    return WORK_ALL;
            }
        }
    }
}
