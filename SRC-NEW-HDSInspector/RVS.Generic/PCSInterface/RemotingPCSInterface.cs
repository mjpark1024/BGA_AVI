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
using System.Windows.Media.Imaging;

namespace RVS.Generic
{
    #region Delegates.
    public delegate void SetAutoNGEventHandler(string aEquipName);
    public delegate bool EnqueuVerifyDBDataHandler(string aEquipName, int anStripID, int[,] aMap);
    public delegate void SetVerifyDBDataEventHandler(string aEquipName);
    public delegate void DisconnectEquipmentEventHandler(string aEquipName);
    public delegate bool CheckDisconnectEventHandler(string aEquipName);
    public delegate bool ConnectEquipmentEventHandler(InspectionEquipDataControl aEquip, int UnitBadCount);
    public delegate void SetVerifyDataEventHandler(string aEquipName, ResultData aData);
    #endregion

    /// <summary>   Remoting pcs interface.  </summary>
    public class RemotingPCSInterface : MarshalByRefObject
    {
        #region Events.
        public static event SetAutoNGEventHandler SetAutoNGEvent;
        public static event EnqueuVerifyDBDataHandler EnqueueVerifyDataEvent;
        public static event DisconnectEquipmentEventHandler DisconnectEquipEvent;
        public static event CheckDisconnectEventHandler CheckDisconnectEvent;
        public static event ConnectEquipmentEventHandler ConnectEquipEvent;
        public static event SetVerifyDataEventHandler SetVerifyDataEvent;
        #endregion

        public void SetAutoNG(string aEquipName)
        {
            SetAutoNGEventHandler autoNG = SetAutoNGEvent;

            if (autoNG != null)
            {
                autoNG(aEquipName);
            }
        }

        /// <summary>   Enqueue verify database data. </summary>
        public bool EnqueueVerifyDBData(string aEquipName, int anStripID, int[,] aMap)
        {
            EnqueuVerifyDBDataHandler verifyDBData = EnqueueVerifyDataEvent;

            if (verifyDBData != null)
            {
                return verifyDBData(aEquipName, anStripID, aMap);
            }
            return false;
        }

        /// <summary>   Disconnects the equipment described by aEquipName. </summary>
        public void DisconnectEquipment(string aEquipName)
        {
            DisconnectEquipmentEventHandler disconnectEquip = DisconnectEquipEvent;

            if (disconnectEquip != null)
            {
                disconnectEquip(aEquipName);
            }
        }

        /// <summary>   Check disconnect. </summary>
        public bool CheckDisconnect(string aEquipName)
        {
            CheckDisconnectEventHandler checkConnect = CheckDisconnectEvent;

            if (checkConnect != null)
            {
                return checkConnect(aEquipName);
            }
            return false;
        }

        /// <summary>   Sets a verify data. </summary>
        public void SetVerifyData(string aEquipName, ResultData aData)
        {
            SetVerifyDataEventHandler setVerifyData = SetVerifyDataEvent;

            if (setVerifyData != null)
            {
                setVerifyData(aEquipName, aData);
            }
        }
        
        /// <summary>   Connects an equipment. </summary>
        public bool ConnectEquipment(InspectionEquipDataControl aEquip, int UnitBadCount)
        {
            ConnectEquipmentEventHandler connectEquip = ConnectEquipEvent;

            if (connectEquip != null)
            {
                return connectEquip(aEquip, UnitBadCount);
            }
            return false;
        }
    }
}
