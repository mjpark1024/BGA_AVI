using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCSWrapper;
using System.Windows.Forms;

namespace DCS
{
    public static class NIDAQmx
    {
        //*** Value set AcquisitionType ***
        public const int Val_FiniteSamps = 10178;           // Finite Samples
        public const int Val_ContSamps = 10123;             // Continuous Samples
        public const int Val_HWTimedSinglePoint = 12522;    // Hardware Timed Single Point
        public const int Val_CountUp = 10128;

        //*** Value set for the state parameter of DAQmxSetDigitalPowerUpStates ***
        public const int Val_High = 10192;                  // High
        public const int Val_Low = 10214;                   // Low
        public const int Tristate = 10310;                  // Tristate

        //*** Value set for the ActiveEdge parameter of DAQmxCfgSampClkTiming and DAQmxCfgPipelinedSampClkTiming ***
        public const int Val_Rising = 10280;                // Rising
        public const int Val_Falling = 10171;               // Falling

        public const int Val_Hz = 10373;                    // Hz
        public const int Val_Meters = 10219;// Meters
        public const int Val_Inches = 10379; // Inches
        public const int Val_Ticks = 10304;// Ticks
        public const int Val_FromCustomScale = 10065; // From Custom Scale
        public const int Val_Degrees = 10146;// Degrees
        public const int Val_Radians = 10273; // Radians



        //*** Values for the Line Grouping parameter of DAQmxCreateDIChan and DAQmxCreateDOChan ***
        public const int Val_ChanPerLine = 0;   // One Channel For Each Line
        public const int Val_ChanForAllLines = 1;   // One Channel For All Lines


        //*** Values for the Fill Mode parameter of DAQmxReadAnalogF64, DAQmxReadBinaryI16, DAQmxReadBinaryU16, DAQmxReadBinaryI32, DAQmxReadBinaryU32,
        //    DAQmxReadDigitalU8, DAQmxReadDigitalU32, DAQmxReadDigitalLines ***
        //*** Values for the Data Layout parameter of DAQmxWriteAnalogF64, DAQmxWriteBinaryI16, DAQmxWriteDigitalU8, DAQmxWriteDigitalU32, DAQmxWriteDigitalLines ***
        public const int Val_GroupByChannel = 0;   // Group by Channel
        public const int Val_GroupByScanNumber = 1;   // Group by Scan Number

        public const int Val_X1 = 10090; // X1
        public const int Val_X2 = 10091; // X2
        public const int Val_X4 = 10092; // X4
        public const int Val_TwoPulseCounting = 10313; // Two Pulse Counting

        public const int Val_AHighBHigh = 10040; // A High B High
        public const int Val_AHighBLow = 10041; // A High B Low
        public const int Val_ALowBHigh = 10042;// A Low B High
        public const int Val_ALowBLow = 10043; // A Low B Low


        public const int Val_LowFreq1Ctr = 10105; // Low Frequency with 1 Counter
        public const int Val_HighFreq2Ctr = 10157; // High Frequency with 2 Counters
        public const int Val_LargeRng2Ctr = 10205; // Large Range with 2 Counters

        public unsafe static IntPtr StartCount(IntPtr pTaskHandle, string aszDevice, string aszInput, int nValue1, int nValue2, long lSampleTime = 1000, double lMinPulse = 0.0000001, bool bFilterEnable = true, int nDelay = 0)
        {
            DCSWrapper.NiDAQmxWrapper NiDaq = new NiDAQmxWrapper();

            string szTaskNameInput = "";

            // void* taskHandleInput  = (void*)pTaskHandleInput;
            void* taskHandle = (void*)pTaskHandle;

            sbyte* error = stackalloc sbyte[2048]; // For get error msg.
            sbyte* taskNameInput = stackalloc sbyte[szTaskNameInput.Length + 1];


            sbyte* result = stackalloc sbyte[aszDevice.Length + 1];
            sbyte* input = stackalloc sbyte[aszInput.Length + 1];


            #region Sets sbyte*
            //////////////////////////////////////////////////////////////////////////
            taskNameInput[szTaskNameInput.Length] = 0; // null terminated
            fixed (char* ch = szTaskNameInput)
            {
                for (int i = 0; i < szTaskNameInput.Length; i++)
                    taskNameInput[i] = (sbyte)szTaskNameInput[i];
            }

            //ctr0
            result[aszDevice.Length] = 0;
            fixed (char* ch = aszDevice)
            {
                for (int i = 0; i < aszDevice.Length; i++)
                    result[i] = (sbyte)ch[i];
            }

            //PFI39 source
            //////////////////////////////////////////////////////////////////////////
            input[aszInput.Length] = 0;
            fixed (char* ch = aszInput)
            {
                for (int i = 0; i < aszInput.Length; i++)
                    input[i] = (sbyte)ch[i];
            }

            error[2048 - 1] = 0;
            #endregion Sets sbyte*

            int nRet = 0;
            //////////////////////////////////////////////////////////////////////////
            // Create Input Task
            //////////////////////////////////////////////////////////////////////////
            // int taskHandle;
            nRet = NiDaq.CreateTask(taskNameInput, &taskHandle);
            if (nRet != 0)
            {
                NiDaq.GetExtendedErrorInfo(error, 2048);
                NiDaq.Dispose();
                return (IntPtr)0;
            }


            nRet = NiDaq.CreateCOPulseChanTicks(taskHandle, result, taskNameInput, input, NIDAQmx.Val_Low, nDelay, nValue1, nValue2);
            if (nRet != 0)
            {
                NiDaq.GetExtendedErrorInfo(error, 2048);
                NiDaq.Dispose();
                return (IntPtr)0;
            }
            nRet = NiDaq.CfgImplicitTiming(taskHandle, NIDAQmx.Val_ContSamps, (ulong)lSampleTime);//1000,100

           // nRet = NiDaq.CfgImplicitTiming(taskHandle, NIDAQmx.Val_FiniteSamps, (ulong)8);//1000,100
                                                                                                  // nRet = NiDaq.CfgSampClkTiming(taskHandle, input, 100000, NIDAQmx.Val_Rising, NIDAQmx.Val_ContSamps, (ulong)lSampleTime);//1000,100


            //HSB-ADD-20120514
            if (bFilterEnable)
            {

                nRet = NiDaq.SetCOCtrTimebaseDigFltrEnable(taskHandle, result, 1);
                if (nRet != 0)
                {
                    NiDaq.GetExtendedErrorInfo(error, 2048);
                    NiDaq.Dispose();
                    return (IntPtr)0;
                }

                nRet = NiDaq.SetCOCtrTimebaseDigFltrMinPulseWidth(taskHandle, result, lMinPulse);//1.0e-7); // 0.0000001
                if (nRet != 0)
                {
                    NiDaq.GetExtendedErrorInfo(error, 2048);
                    NiDaq.Dispose();
                    return (IntPtr)0;
                }
            }
            else
            {
                nRet = NiDaq.SetCOCtrTimebaseDigFltrEnable(taskHandle, result, 0);
                if (nRet != 0)
                {
                    NiDaq.GetExtendedErrorInfo(error, 2048);
                    NiDaq.Dispose();
                    return (IntPtr)0;
                }
            }

            if (nRet != 0)
            {
                NiDaq.GetExtendedErrorInfo(error, 2048);
                NiDaq.Dispose();
                return (IntPtr)0;
            }

  

            nRet = NiDaq.StartTask(taskHandle);
            if (nRet != 0)
            {
                NiDaq.GetExtendedErrorInfo(error, 2048);
                NiDaq.Dispose();
                return (IntPtr)0;
            }
            NiDaq.Dispose();

            return (IntPtr)taskHandle;
        }


        public unsafe static int SCACount(IntPtr pTaskHandle)
        {
            int nRet = 0;
            void* taskHandle = (void*)pTaskHandle;
            sbyte* error = stackalloc sbyte[2048]; // For get error msg.

            DCSWrapper.NiDAQmxWrapper NiDaq = new NiDAQmxWrapper();
            nRet = NiDaq.StopTask(taskHandle);
            if (nRet != 0)
            {
                NiDaq.GetExtendedErrorInfo(error, 2048);
                NiDaq.Dispose();
                return nRet;
            }

            nRet = NiDaq.ClearTask(taskHandle);
            if (nRet != 0)
            {
                NiDaq.GetExtendedErrorInfo(error, 2048);
                NiDaq.Dispose();
                return nRet;
            }
            NiDaq.Dispose();
            return nRet;

        }
    }
}
