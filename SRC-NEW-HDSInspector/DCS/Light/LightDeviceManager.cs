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
 * @file  LightDeviceManager.cs
 * @brief 
 *  편이한 조명 구성에 효과적으로 대응하기 위해 작성함.
 * 
 * @author : suoow2
 * @date : 2012.08.08
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2012.08.08 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Diagnostics;
using System.Windows.Forms;

namespace DCS.Light
{
    public class LightDeviceManager
    {
        #region Singleton.
        private static LightDeviceManager _instance = null;
        public static LightDeviceManager Instance
        {
            get { return _instance; }
        }

        static LightDeviceManager()
        {
            _instance = new LightDeviceManager();
        }

        private LightDeviceManager()
        { }
        #endregion

        public LightDevice LightDevices { get; set; }
        public bool InitializeLight(string[] azsPort, int[] anLightType)
        {
            bool bRet = true;

            LightDevices = new LightDevice();
            bool bResult = LightDevices.Init();

            LightDevices.InitLightDevice(azsPort, anLightType);
            InitializeChannelSet();

            return bRet;
        }

        private void InitializeChannelSet()
        {
            if (_instance.LightDevices == null)
                return;
   
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < LightDevices.m_arrLightValue.GetLength(0); j ++)
                {
                    LightDevices.m_arrLightValue[j, i] = 0;
                    LightDevices.m_arrLightValue[j, i] = 0;
                    LightDevices.m_arrLightValue[j, i] = 0;
                    LightDevices.m_arrLightValue[j, i] = 0;
                    LightDevices.m_arrLightValue[j, i] = 0;
                }
            }
            
        }

        public void LightOn(int no, bool abIsOn)
        {
            if (_instance.LightDevices != null)
            {
                LightDevices.SetOnOffEx(no, abIsOn);
            }
        }

        public void SetLight(int no, int ValueIndex)
        {
            if (_instance.LightDevices != null)
            {
                LightDevices.SetLight(no, ValueIndex);
            }
        }

        public void LightOff()
        {
            if (_instance.LightDevices != null)
            {
                for(int i = 0; i < LightDevices.m_arrLightValue.GetLength(0); i++)
                    LightDevices.SetOnOffEx(i, false);
            }
        }

        public void SetLightValue(int no, int[] val)
        {
            try
            {
                for (int i = 0; i < 8; i++)
                    LightDevices.m_arrLightValue[no, i] = val[i];
            }
            catch { }
        }

        // Close Lights.
        public void Dispose()
        {
             LightDevices.ClosePortAll();
        }
    }
}
