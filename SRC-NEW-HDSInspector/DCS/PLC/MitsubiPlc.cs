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

using Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;

namespace DCS.PLC
{
    /// <summary>   Interface for mitsubi plc.  </summary>
    /// <remarks>   suoow2, 2014-11-22. </remarks>
    interface IMitsubiPlc
    {
        bool ConnectPLC(string ip, int port);
        int GetDevice(string szDeivce, out int lplData);
        int SetDevice(string szDeivce, int lData);
        int GetDeviceBlock(string szDevice, int nSize, out int lpdwData);
        int Write(string szDeivce, int nSize, ref int lpdwData);
        int WriteBuffer(int lStartIO, int lAddress, int lWriteSize, out short lpwData);
        int ReadBuffer(int lStartIO, int lAddress, int lWriteSize, out short lpwData);
        int Close();
    }

    /// <summary>   Mitsubi plc.  </summary>
    public class MitsubiPlc // : IPlcController 2012-04-24, Interface 삭제.
    {
        private IMitsubiPlc m_mitsubiPlc;
        private bool m_bOpen = false;
        private int m_Mode;
        Common.PLC_Address PLCConst;
        #region  Ctor & Destructor.
        public MitsubiPlc(int mode, Common.PLC_Address Address)
        {
            PLCConst = Address;
            if (mode == 0)
            {
                try
                {
                    m_mitsubiPlc = ActQJ71E71TCPDev.Instance();
                }
                catch (Exception ex)
                {
                    string[] str = ex.Message.Split('{', '}');
                    setReg64bit(str[1]);
                    Thread.Sleep(300);
                    m_mitsubiPlc = ActQJ71E71TCPDev.Instance();
                }
            }
            else if (mode == 2)
            {
                m_mitsubiPlc = ActClient.Instance();
            }
            else
            {
                m_mitsubiPlc = ActQNUDECPUTCPDev.Instance();
            }
            m_Mode = mode;
        }

        ~MitsubiPlc()
        {
            // m_mitsubiPlc.Close();
        }
        #endregion
        public void setReg64bit(string regVal)
        {
            RegistryKey reg = Registry.ClassesRoot;
            RegistryKey reg1 = Registry.ClassesRoot;
            RegistryKey reg2 = Registry.LocalMachine;

            RegistrySecurity userSecurity = new RegistrySecurity();
            RegistryAccessRule userRule = new RegistryAccessRule("Everyone", RegistryRights.FullControl, AccessControlType.Allow);
            userSecurity.AddAccessRule(userRule);

            reg = reg.CreateSubKey("WOW6432Node\\CLSID\\{" + regVal + "}", RegistryKeyPermissionCheck.ReadWriteSubTree, userSecurity);
            reg.SetValue("AppID", "{" + regVal + "}", RegistryValueKind.String);
            reg.Flush();

            reg1 = reg1.CreateSubKey("WOW6432Node\\AppID\\{" + regVal + "}", RegistryKeyPermissionCheck.ReadWriteSubTree, userSecurity);
            reg1.SetValue("DllSurrogate", "", RegistryValueKind.String);
            reg1.Flush();

            reg2 = reg2.OpenSubKey("Software\\Classes\\AppID\\{" + regVal + "}", RegistryKeyPermissionCheck.Default);
            if (reg2 == null)
            {
                reg2 = Registry.LocalMachine.CreateSubKey("Software\\Classes\\AppID\\{" + regVal + "}", RegistryKeyPermissionCheck.ReadWriteSubTree, userSecurity);

            }
            reg.Close();
            reg2.Close();
        }

        public bool ConnectPLC(string ip, int port)
        {
            m_bOpen = m_mitsubiPlc.ConnectPLC(ip, port);
            return m_bOpen;
        }

        public int ReadData(string address)
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(address, out lData);
            return lData;
        }

        public int WriteData(string address, int val)
        {
            m_mitsubiPlc.Write(address, 1, ref val);
            return 0;
        }

        public int ReadMode()
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(PLCConst.READ_MODE, out lData);
            return lData;
        }



        public double ReadBoatPos() {
            int lData1 = 0;
            int lData2 = 0;
            m_mitsubiPlc.GetDevice(PLCConst.READ_BOAT1, out lData1);
            m_mitsubiPlc.GetDevice(PLCConst.READ_BOAT2, out lData2);
            return lData1 + (lData2 / 1000.0);
        }

        public double ReadCamPos(int mcType)
        {
            int lData1 = 0;
            int lData2 = 0;
            m_mitsubiPlc.GetDevice(PLCConst.READ_CAM_1, out lData1);
            m_mitsubiPlc.GetDevice(PLCConst.READ_CAM_2, out lData2);

            if (mcType == 1) return lData1 + (lData2 / 1000.0);
            else if (mcType == 0) return lData1;
            else return lData1;
        }

        public void SendCamPitch(double cam1pitch)
        {
            int val = System.Convert.ToInt32(cam1pitch * 100.0);//cam 이송 pitch
            m_mitsubiPlc.Write(PLCConst.CAM1_PITCH, 1, ref val);
            Thread.Sleep(100);
        }

        public void SetWarning(bool isOn)
        {
            int val = isOn ? 1 : 0;
            m_mitsubiPlc.SetDevice(PLCConst.SET_WARNING, val);
        }

        // 모델 정보 전송 : 길이, 폭, 속도, 레이저 얼라인 위치, 양품적재수
        public void SendModelData(double Height, double Width, int ScanVelocity1, int ScanVelocity2, double Cam1Pos, double CamPitch, int LaserStep, double LaserPitch, double AlignPosX1, double AlignPosX2, double AlignPosY, double LaserCorr, double Thickness, int CACount, int BACount, int LeftID, int mcType, MarkingBoatShift MarkingShift)
        {
            ResetIO(); Thread.Sleep(100);

            int h = System.Convert.ToInt32(Height * 100);
            int w = System.Convert.ToInt32(Width * 100);
            int t = System.Convert.ToInt32(Thickness * 1000);
            m_mitsubiPlc.Write(PLCConst.MODEL_HEIGHT, 1, ref w);
            Thread.Sleep(100);
            m_mitsubiPlc.Write(PLCConst.MODEL_WIDTH, 1, ref h);
            Thread.Sleep(100);

            if (Thickness > 0.0)
            {
                m_mitsubiPlc.Write(PLCConst.MODEL_THICK, 1, ref t);
                Thread.Sleep(100);
            }
            if (ScanVelocity1 > 0)
            {
                m_mitsubiPlc.Write(PLCConst.TOP_SPEED, 1, ref ScanVelocity1);
                Thread.Sleep(100);
                m_mitsubiPlc.Write(PLCConst.BOT_SPEED, 1, ref ScanVelocity2);
                Thread.Sleep(100);
            }
            int val = System.Convert.ToInt32(Cam1Pos * 100.0);//cam 이송 원점
            m_mitsubiPlc.Write(PLCConst.CAM1_POS, 1, ref val);
            Thread.Sleep(100);

            val = System.Convert.ToInt32(CamPitch * 100.0);//cam 이송 pitch
            m_mitsubiPlc.Write(PLCConst.CAM1_PITCH, 1, ref val);
            Thread.Sleep(100);

            m_mitsubiPlc.Write(PLCConst.LASER_STEP, 1, ref LaserStep);
            Thread.Sleep(100);

            val = System.Convert.ToInt32(LaserPitch * 100.0);
            m_mitsubiPlc.Write(PLCConst.LASER_PITCH, 1, ref val);
            Thread.Sleep(100);

            if (mcType == 1) val = System.Convert.ToInt32(AlignPosY * 1000.0);
            else if (mcType == 0) val = System.Convert.ToInt32(AlignPosY);

            m_mitsubiPlc.Write(PLCConst.ALIGN_YPOS, 1, ref val);
            Thread.Sleep(100);

            val = System.Convert.ToInt32(Math.Floor(AlignPosX1));
            m_mitsubiPlc.Write(PLCConst.ALIGN_XPOS11, 1, ref val);
            Thread.Sleep(100);
            val = System.Convert.ToInt32(AlignPosX1 * 1000.0) % 1000;
            m_mitsubiPlc.Write(PLCConst.ALIGN_XPOS12, 1, ref val);
            Thread.Sleep(100);

            val = System.Convert.ToInt32(Math.Floor(AlignPosX2));
            m_mitsubiPlc.Write(PLCConst.ALIGN_XPOS21, 1, ref val);
            Thread.Sleep(100);
            val = System.Convert.ToInt32(AlignPosX2 * 1000.0) % 1000;
            m_mitsubiPlc.Write(PLCConst.ALIGN_XPOS22, 1, ref val);
            Thread.Sleep(100);

            if (mcType == 1)
            {
                val = System.Convert.ToInt32(BACount);
                m_mitsubiPlc.Write(PLCConst.TOP_COUNT, 1, ref val);
                Thread.Sleep(100);

                val = System.Convert.ToInt32(CACount);
                m_mitsubiPlc.Write(PLCConst.BOT_COUNT, 1, ref val);
                Thread.Sleep(100);
            }
            else
            {
                val = System.Convert.ToInt32(CACount);
                m_mitsubiPlc.Write(PLCConst.TOP_COUNT, 1, ref val);
                Thread.Sleep(100);

                val = System.Convert.ToInt32(BACount);
                m_mitsubiPlc.Write(PLCConst.BOT_COUNT, 1, ref val);
                Thread.Sleep(100);
            }


            if (LeftID >= 0)
            {
                val = System.Convert.ToInt32(LeftID);
                m_mitsubiPlc.Write(PLCConst.ID_USED, 1, ref val);
                Thread.Sleep(100);
            }

            val = System.Convert.ToInt32(MarkingShift.Offset1 * 100.0);
            m_mitsubiPlc.Write(PLCConst.MARKING_STAGE_SHIFT1, 1, ref val);
            Thread.Sleep(100);
            val = System.Convert.ToInt32(MarkingShift.Offset2 * 100.0);
            m_mitsubiPlc.Write(PLCConst.MARKING_STAGE_SHIFT2, 1, ref val);
            Thread.Sleep(100);

            Thread.Sleep(300);
            m_mitsubiPlc.SetDevice(PLCConst.MODEL_SEND_DONE, 1);
            Thread.Sleep(300);
            m_mitsubiPlc.SetDevice(PLCConst.MODEL_SEND_DONE, 0);
            Thread.Sleep(50);
        }

        public int ReadIDUsed()
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(PLCConst.ID_USED, out lData);
            return lData;
        }

        public int ReadBoat2ID()
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(PLCConst.BOAT2_ID, out lData);
            return lData;
        }

        public int ReadResultID()
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(PLCConst.RESULT_ID, out lData);
            return lData;
        }

        public int ReadLaserBoat()
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(PLCConst.LASER_LOC, out lData);
            return lData;
        }

        public int ReadLaser1ID()
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(PLCConst.LASER1_ID, out lData);
            return lData;
        }

        public int ReadLaser2ID()
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(PLCConst.LASER2_ID, out lData);
            return lData;
        }

        public void SendModelName(string aszModelName)
        {
            /////

        }

        public bool InspectReady()
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(PLCConst.REQ_READY, out lData);

            return (lData == 0) ? false : true;
        }

        public int ReadBoatCA()
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(PLCConst.RED_BOATCA, out lData);
            return lData;
        }

        public int ReadBoatBA()
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(PLCConst.RED_BOATBA, out lData);
            return lData;
        }

        public int ReadBoat2CamPos()
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(PLCConst.RED_BOAT2_CAM, out lData);
            return lData;
        }

        // 보트1 스캔요구 확인
        public bool RequestBoatCA()
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(PLCConst.REQ_BOATCA, out lData);


            return (lData == 0) ? false : true;
        }

        // 보트2 스캔요구 확인
        public bool RequestBoatBA()
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(PLCConst.REQ_BOATBA, out lData);

            return (lData == 0) ? false : true;
        }

        // 보트1 스캔 시작
        public void PassBoatCA()
        {
            m_mitsubiPlc.SetDevice(PLCConst.PASS_BOATCA, 1);
            Thread.Sleep(100);
            m_mitsubiPlc.SetDevice(PLCConst.PASS_BOATCA, 0);
        }

        // 보트2 스캔시작
        public void PassBoatBA()
        {
            m_mitsubiPlc.SetDevice(PLCConst.PASS_BOATBA, 1);
            Thread.Sleep(100);
            m_mitsubiPlc.SetDevice(PLCConst.PASS_BOATBA, 0);
        }

        // 레이저 요구
        public bool RequestLaser(int no) {
            int lData = 0;
            if (no == 0)
                m_mitsubiPlc.GetDevice(PLCConst.REQ_LASER1, out lData);
            else
                m_mitsubiPlc.GetDevice(PLCConst.REQ_LASER2, out lData);
            return (lData == 0) ? false : true;
        }

        public bool RequestDone()
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(PLCConst.REQ_END, out lData);


            return (lData == 0) ? false : true;
        }

        public bool RequestID()
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(PLCConst.REQ_ID, out lData);


            return (lData == 0) ? false : true;
        }

        // Align 요구
        public bool RequestAlign(int no) {
            int lData = 0;
            if (no == 0)
                m_mitsubiPlc.GetDevice(PLCConst.REQ_ALIGN1, out lData);
            else
                m_mitsubiPlc.GetDevice(PLCConst.REQ_ALIGN2, out lData);
            return (lData == 0) ? false : true;
        }

        public bool InspectDone()
        {
            int lData = 0;
            m_mitsubiPlc.GetDevice(PLCConst.REQ_DONE, out lData);

            return (lData == 0) ? false : true;
        }

        public void PassID()
        {
            m_mitsubiPlc.SetDevice(PLCConst.PASS_ID, 1);
            Thread.Sleep(100);
            m_mitsubiPlc.SetDevice(PLCConst.PASS_ID, 0);
        }

        // 레이저 완료
        public void PassLaser(int no)
        {
            if (no == 0)
            {
                m_mitsubiPlc.SetDevice(PLCConst.PASS_LASER1, 1);
                Thread.Sleep(100);
                m_mitsubiPlc.SetDevice(PLCConst.PASS_LASER1, 0);
            }
            else
            {
                m_mitsubiPlc.SetDevice(PLCConst.PASS_LASER2, 1);
                Thread.Sleep(100);
                m_mitsubiPlc.SetDevice(PLCConst.PASS_LASER2, 0);
            }
        }

        // 레이저 완료
        public void PassAlign(int no)
        {
            if (no == 0)
            {
                m_mitsubiPlc.SetDevice(PLCConst.PASS_ALIGN1, 1);
                Thread.Sleep(100);
                m_mitsubiPlc.SetDevice(PLCConst.PASS_ALIGN1, 0);
            }
            else
            {
                m_mitsubiPlc.SetDevice(PLCConst.PASS_ALIGN2, 1);
                Thread.Sleep(100);
                m_mitsubiPlc.SetDevice(PLCConst.PASS_ALIGN2, 0);
            }
        }

        // 레이저 완료
        public void ByPass(int no)
        {
            if (no == 0)
            {
                m_mitsubiPlc.SetDevice(PLCConst.LASER_BYPASS1, 1);
                Thread.Sleep(100);
                m_mitsubiPlc.SetDevice(PLCConst.LASER_BYPASS1, 0);
            }
            else
            {
                m_mitsubiPlc.SetDevice(PLCConst.LASER_BYPASS2, 1);
                Thread.Sleep(100);
                m_mitsubiPlc.SetDevice(PLCConst.LASER_BYPASS2, 0);
            }
        }

        // 검사 결과 요구 확인
        public bool RequestResult()
        {
            int lData = 0;
            if (m_Mode != 3)
                m_mitsubiPlc.GetDevice(PLCConst.REQ_RESULT, out lData);
            else
            {
                short s;
                m_mitsubiPlc.ReadBuffer(22, 9872, 2, out s);
                lData = s & 2;
            }

            return (lData == 0) ? false : true;
        }

        // 검사 결과 전송
        public void SendResult(int Res)
        {
            //id 확인 하는 거 추가 할 것    
            m_mitsubiPlc.Write(PLCConst.WRITE_RESULT, 1, ref Res);
            Thread.Sleep(20);
            m_mitsubiPlc.SetDevice(PLCConst.RESULT_DONE, 1);
        }

        // 검사 결과 전송
        public void SetResult()
        {
            //id 확인 하는 거 추가 할 것    
            m_mitsubiPlc.SetDevice(PLCConst.RESULT_DONE, 0);
        }

        public void SendID(int id) {
            m_mitsubiPlc.Write(PLCConst.STRIP_ID, 1, ref id);
        }

        public void ResetIO()
        {
            m_mitsubiPlc.SetDevice(PLCConst.LASER_BYPASS1, 0);
            m_mitsubiPlc.SetDevice(PLCConst.LASER_BYPASS2, 0);
            m_mitsubiPlc.SetDevice(PLCConst.PASS_ALIGN1, 0);      
            m_mitsubiPlc.SetDevice(PLCConst.PASS_ALIGN2, 0);
            m_mitsubiPlc.SetDevice(PLCConst.PASS_LASER1, 0);
            m_mitsubiPlc.SetDevice(PLCConst.PASS_LASER2, 0);
            m_mitsubiPlc.SetDevice(PLCConst.PASS_BOATBA, 0);
            m_mitsubiPlc.SetDevice(PLCConst.PASS_BOATCA, 0);
            m_mitsubiPlc.SetDevice(PLCConst.MODEL_SEND_DONE, 0);
        }
    }
}
