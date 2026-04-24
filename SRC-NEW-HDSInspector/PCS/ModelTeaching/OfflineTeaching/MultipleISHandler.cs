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
 * @file  MultipleISHandler.cs
 * @brief 
 *  1개의 IS Program으로 상부 반사, 하부 반사, 상부 투과 모두 티칭할 수 있도록 지원하는 클래스.
 * 
 * @author : Minseok Hwang <h.min-suck@samsung.com>
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

namespace PCS.ModelTeaching.OfflineTeaching
{
    public class MultipleISHandler
    {
        public VisionInterface Vision   { get; private set; }
        public bool VisionConnected     { get; private set;}

        public MultipleISHandler()
        {
            CreateVision();
        }

        #region Handling Visions.
        public void CreateVision()
        {
            Vision = new VisionInterface(0);
            VisionConnected = false;
        }

        public bool ConnectVision(string aszIP, int anPort)
        {
            return Vision.Connect(aszIP, anPort);
        }

        public void SetPageDelay(int anPageDelay)
        {
            Vision.SetPageDelay(anPageDelay);
        }

        public void SetAutoInspection(bool boolValue)
        {
            Vision.AutoInspect = boolValue;
        }

        public void ReleaseVision()
        {
            Vision.DisConnect();
        }
        #endregion Handling Visions.

        #region Finalizer.

        ~MultipleISHandler()
        {
            ReleaseVision();
        }
        #endregion Finalizer.
    }
}
