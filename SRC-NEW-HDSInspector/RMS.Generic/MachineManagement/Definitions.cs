/*********************************************************************************
 * Copyright(c) 2011,2012,2013 by Samsung Techwin.
 * 
 * This software is copyrighted by, and is the sole property of Samsung Techwin.
 * All rigths, title, ownership, or other interests in the software remain the
 * property of Samsung Techwin. This software may only be used in accordance with
 * the corresponding license agreement. Any unauthorized use, duplication, 
 * transmission, distribution, or disclosure of this software is expressly 
 * forbidden.
 *
 * This Copyright notice may not be removed or modified without prior written
 * consent of Samsung Techwin.Samsung Techwin reserves the right to modify this 
 * software without notice.
 *
 * Samsung Techwin.
 * KOREA 
 * http://www.samsungtechwin.co.kr
 *********************************************************************************/
/**
 * @file  Definitions.cs
 * @brief 
 *  Definitions of Machine Management module.
 * 
 * @author : Minseok Hwang <h.min-suck@samsung.com>
 * @date : 2011.08.27
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.27 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMS.Generic.MachineManagement
{
    // 2011-08-27, Minseok Hwang
    public static class MachineDefinition
    {
        public static readonly string CODE = "00";
    }

    // 2011-08-27, Minseok Hwang
    public static class ELF_AVI_Definition
    {
        public static readonly string CODE = "0001";
        public static readonly string NAME = "ELF AVI";
    }

    // 2011-08-27, Minseok Hwang
    public static class BOC_AOI_Definition
    {
        public static readonly string CODE = "0002";
        public static readonly string NAME = "BOC AOI";
    }
}
