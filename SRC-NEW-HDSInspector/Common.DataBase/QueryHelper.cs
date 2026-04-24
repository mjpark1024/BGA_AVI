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
 * @file  QueryHelper.cs
 * @brief
 *  Query Helper.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.09.11
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.09.11 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.DataBase
{
    /// <summary>   Query helper.  </summary>
    /// <remarks>   suoow2, 2014-09-11. </remarks>
    public static class QueryHelper
    {
        /// <summary>   Gets a code. </summary>
        /// <remarks>   suoow2, 2014-09-11. </remarks>
        /// <param name="codeNumber">   The code number. </param>
        /// <param name="codeLength">   Length of the code. </param>
        /// <returns>   The code. </returns>
        public static string GetCode(int codeNumber, int codeLength)
        {
            // codeNumber - 생성하려는 코드의 숫자 값.
            // codeLength - 생성하고자 하는 코드의 길이.

            if (codeNumber < 0)
            {
                return string.Empty;
            }

            string strCode = codeNumber.ToString("0000");
            //while (strCode.Length < codeLength)
            //{
            //    strCode = "0" + strCode;
            //}

            if (strCode.Length != codeLength)
            {
                return string.Empty;
            }

            // 반환되는 코드의 형태
            // "0001", "10010", ...
            return strCode;
        }
    }
}
