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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCS.PLC
{
    /// <summary>   Interface for plc controller.  </summary>
    /// <remarks>   suoow2, 2014-11-22. </remarks>
    //interface IPlcController
    //{
    //    /// <summary>   Connects a plc. </summary>
    //    bool ConnectPLC(string IP, int port);

    //    /// <summary>   Reads the mode. </summary>
    //    int ReadMode();

    //    /// <summary>   Sends a model data. </summary>
    //    void SendModelData(double Height, double Width, int Speed, double AlignPosX, double AlignPosY, int Good_Count);

    //    /// <summary>   Inspect ready. </summary>
    //    bool InspectReady();

    //    /// <summary>   Gets the request boat 1. </summary>
    //    bool RequestBoat1();

    //    /// <summary>   Gets the request boat 2. </summary>
    //    bool RequestBoat2();

    //    /// <summary>   Pass boat 1. </summary>
    //    void PassBoat1();

    //    /// <summary>   Pass boat 2. </summary>
    //    void PassBoat2();

    //    /// <summary>   Gets the request laser. </summary>
    //    bool RequestLaser();

    //    /// <summary>   Pass laser. </summary>
    //    void PassLaser();

    //    /// <summary>   Gets the request result. </summary>
    //    bool RequestResult();

    //    /// <summary>   Sends a result. </summary>
    //    void SendResult(int val);
    //}

    /// <summary>   Plc device.  </summary>
    public class PLCDevice
    {
        /// <summary> The plc device </summary>
        // private IPlcController m_PlcDevice;
        private MitsubiPlc m_PlcDevice; // 2012-04-24, Interface 삭제.

        /// <summary>   Initializes a new instance of the PLCDevice class. </summary>
        public PLCDevice(int plcMode, Common.PLC_Address Address)
        {
            // Mode
            // DEFAULT - 0
            // MXComponent = 1
            m_PlcDevice = new MitsubiPlc(plcMode, Address);
        }

        public int ReadData(string add)
        {
            return m_PlcDevice.ReadData(add);
        }

        public int WriteData(string add, int val)
        {
            return m_PlcDevice.WriteData(add, val);
        }

        /// <summary>   Connects a plc. </summary>
        public bool ConnectPLC(string IP, int port)
        {
            return m_PlcDevice.ConnectPLC(IP, port);
        }

        /// <summary>   Reads the mode. </summary>
        public int ReadMode()
        {
            return m_PlcDevice.ReadMode();
        }

        public double ReadBoatPos()
        {
            return m_PlcDevice.ReadBoatPos();
        }

        public double ReadCamPos(int mcType)
        {
            return m_PlcDevice.ReadCamPos(mcType);
        }
        /// <summary>   Sends a model data. </summary>
        public void SendModelData(double Height, double Width, int ScanVelocity1, int ScanVelocity2, double Cam1Pos, double CamPitch, int LaserStep, double LaserPitch, double AlignPosX1, double AlignPosX2, double AlignPosY, double LaserCorr, double Thickness, int CACount, int BACount, int LeftID, int mcType, MarkingBoatShift MarkingShift)
        {
            m_PlcDevice.SendModelData(Height, Width, ScanVelocity1, ScanVelocity2, Cam1Pos, CamPitch, LaserStep, LaserPitch, AlignPosX1, AlignPosX2, AlignPosY, LaserCorr, Thickness, CACount, BACount, LeftID, mcType, MarkingShift);
        }

        public int ReadIDUsed()
        {
            return m_PlcDevice.ReadIDUsed();
        }

        public void SendPitchData(double pitch)
        {
            m_PlcDevice.SendCamPitch(pitch);
        }

        public void SetWarning(bool isOn)
        {
            m_PlcDevice.SetWarning(isOn);
        }

        /// <summary>   Gets the inspect ready. </summary>
        public bool InspectReady()
        {
            return m_PlcDevice.InspectReady();
        }

        public bool InspectDone()
        {
            return m_PlcDevice.InspectDone();
        }

        public bool RequestBoat(int nID, int nMachineType)//, bool CAFirst)//검사 준비 완료 및 몇번째 스캔 반환
        {
            int n = 0;
           //if (CAFirst)
            {
                if(nID < 2)
                {
                    if(RequestBoatCA()>=1)
                    {
                        if (nMachineType == 0)
                            n = RequestBoatCA();
                        else n = ReadBoat2CamPos();
                        if (n == (nID + 1))
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else if(nID < 4)
                {
                    if(RequestBoatCA()>=1)
                        return true;
                    else
                        return false;
                }
                else
                {
                    if (RequestBoatBA() == 1)
                        return true;
                    else
                        return false;
                }
            }
            //else 
            //{
            //    if (nID < 2)
            //    {
            //        if (RequestBoatBA() == 1)
            //            return true;
            //        else
            //            return false;
            //    }
            //    else if (nID < 4)
            //    {
            //        if(RequestBoatBA() == 2)
            //        {
            //            n = ReadBoat2CamPos();
            //            if (n == (nID + 1))
            //                return true;
            //            else
            //                return false;
            //        }
            //        else
            //            return false;
            //    }
            //    else
            //    {
            //        if (RequestBoatCA() == 1)
            //            return true;
            //        else
            //            return false;
            //    }
            //}

        }

        public int ReadLaserBoat()
        {
            return m_PlcDevice.ReadLaserBoat();
        }

        /// <summary>   Gets the request boat 1. </summary>
        public int RequestBoatCA()
        {
            if (m_PlcDevice.RequestBoatCA())
            {
                return m_PlcDevice.ReadBoatCA();
            }
            return 0;
        }

        public int RequestBoatBA()
        {
            if (m_PlcDevice.RequestBoatBA())
            {
                return m_PlcDevice.ReadBoatBA();
            }
            return 0;
        }


        public bool RequestDone()
        {
            return m_PlcDevice.RequestDone();
        }

        /// <summary>   Gets the request boat 2. </summary>
       

        public bool RequestID()
        {
            return m_PlcDevice.RequestID();
        }

        public void PassID()
        {
            m_PlcDevice.PassID();
        }

        public int ReadBoat2CamPos()
        {
            return m_PlcDevice.ReadBoat2CamPos();
        }

        public void PassBoat(int id)
        {
            if (id == 0)
                PassBoatCA();
            else
                PassBoatBA();
        }

        /// <summary>   Pass boat 1. </summary>
        public void PassBoatCA()
        {
            m_PlcDevice.PassBoatCA();
        }

        /// <summary>   Pass boat 2. </summary>
        public void PassBoatBA()
        {
            m_PlcDevice.PassBoatBA();
        }

        /// <summary>   Gets the request laser. </summary>
        public bool RequestLaser(int no)
        {
            return m_PlcDevice.RequestLaser(no);
        }

        /// <summary>   Gets the request laser. </summary>
        public bool RequestAlign(int no)
        {
            return m_PlcDevice.RequestAlign(no);
        }

        /// <summary>   Pass laser. </summary>
        public void PassLaser(int no)
        {
            m_PlcDevice.PassLaser(no);
        }

        /// <summary>   Pass laser. </summary>
        public void PassAlign(int no)
        {
            m_PlcDevice.PassAlign(no);
        }

        /// <summary>   Pass laser. </summary>
        public void ByPass(int no)
        {
            m_PlcDevice.ByPass(no);
        }

        /// <summary>   Gets the request result. </summary>
        public bool RequestResult()
        {
            return m_PlcDevice.RequestResult();
        }

        /// <summary>   Sends a result. </summary>
        public void SendResult(int val)
        {
            m_PlcDevice.SendResult(val);
        }

        public void SetResult()
        {
            m_PlcDevice.SetResult();
        }

        /// <summary>   Sends a result. </summary>
        public void SendID(int id)
        {
            m_PlcDevice.SendID(id);
        }

        public int ReadBoat2ID()
        {
            return m_PlcDevice.ReadBoat2ID();
        }

        public int ReadResultID()
        {
            return m_PlcDevice.ReadResultID();
        }

        public int ReadLaser1ID()
        {
            return m_PlcDevice.ReadLaser1ID();
        }

        public int ReadLaser2ID()
        {
            return m_PlcDevice.ReadLaser2ID();
        }
    }
}
