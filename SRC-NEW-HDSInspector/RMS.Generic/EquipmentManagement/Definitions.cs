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
 *  Definitions of Equipment Management module.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
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
using Common;

namespace RMS.Generic.EquipmentManagement
{
    public class EquipmentType : NotifyPropertyChanged
    {
        private string m_TypeCode;
        private string m_TypeName;

        public string TypeCode
        {
            get { return m_TypeCode; }
            set { m_TypeCode = value; Notify("TypeCode"); }
        }

        public string TypeName
        {
            get { return m_TypeName; }
            set { m_TypeName = value; Notify("TypeName"); }
        }
    }
}
