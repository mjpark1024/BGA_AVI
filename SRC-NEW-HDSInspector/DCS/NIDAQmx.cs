using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCSWrapper;

namespace DCS
{
    /// <summary>   Nidaqmx.  </summary>
    public static class NIDAQmx
    {
        //*** Value set AcquisitionType ***
        public const int Val_FiniteSamps = 10178; // Finite Samples
        public const int Val_ContSamps = 10123; // Continuous Samples
        public const int Val_HWTimedSinglePoint = 12522; // Hardware Timed Single Point

        //*** Value set for the state parameter of DAQmxSetDigitalPowerUpStates ***
        public const int Val_High = 10192;// High
        public const int Val_Low = 10214;// Low
        public const int Tristate = 10310;// Tristate

        //*** Value set for the ActiveEdge parameter of DAQmxCfgSampClkTiming and DAQmxCfgPipelinedSampClkTiming ***
        public const int Val_Rising = 10280; // Rising
        public const int Val_Falling = 10171; // Falling

        public const int Val_Hz = 10373; // Hz

        /// <summary>   Starts a count. </summary>
        public unsafe static IntPtr StartCount(IntPtr pTaskHandle, string dev, string strInput, int nValue1, int nValue2)
        {
            DCSWrapper.NiDAQmxWrapper NiDaq = new NiDAQmxWrapper();

            string strTaskName = "";
            void* taskHandle = (void*)pTaskHandle;
            sbyte* taskName = stackalloc sbyte[strTaskName.Length + 1];
            taskName[strTaskName.Length] = 0;
            fixed (char* sTaskName = strTaskName)
            {
                for (int i = 0; i < strTaskName.Length; i++)
                {
                    taskName[i] = (sbyte)sTaskName[i]; // Casting unicode! Pay attention
                }
            }

            // int taskHandle;
            int nRet = NiDaq.CreateTask(taskName, &taskHandle);

            sbyte* result = stackalloc sbyte[dev.Length + 1];
            result[dev.Length] = 0; //nullterminated
            fixed (char* p = dev)
            {
                // Copying bytewise
                for (int i = 0; i < dev.Length; i++)
                {
                    result[i] = (sbyte)p[i]; // Casting unicode! Pay attention
                }
            }
            sbyte* error = stackalloc sbyte[2048];

            sbyte* input = stackalloc sbyte[strInput.Length + 1];
            result[strInput.Length] = 0; // null terminated
            fixed (char* p = strInput)
            {
                // Copying bytewise
                for (int i = 0; i < strInput.Length; i++)
                {
                    input[i] = (sbyte)p[i]; // Casting unicode! Pay attention
                }
            }

            // nValue1 = 4
            // nValue2 = 4;

            nRet = NiDaq.CreateCOPulseChanTicks(taskHandle, result, taskName, input, NIDAQmx.Val_Low, 0, nValue1, nValue2);
            NiDaq.GetExtendedErrorInfo(error, 2048);

            // nRet = NiDaq.CfgDigEdgeStartTrig(taskHandle, input, NIDAQmx.Val_Rising);
            // NiDaq.GetExtendedErrorInfo(error, 2048);

            // nRet=  NiDaq.CreateCOPulseChanFreq(taskHandle, result, taskName, NIDAQmx.Val_Hz, NIDAQmx.Val_Low, 0.0, 1.00, dValue2);
            // NiDaq.CfgDigEdgeStartTrig(taskHandle, "/Dev1/PFI39", NIDAQmx.Val_Rising);
            // ctr0     PFI39, ctr2 PFI 31
            // NiDaq.GetExtendedErrorInfo(error, 2048);

            // nRet = NiDaq.SetCIFreqDigFltrMinPulseWidth(taskHandle, result, 0.0000001);//1.0e-7);
            // NiDaq.GetExtendedErrorInfo(error, 2048);

            nRet = NiDaq.CfgImplicitTiming(taskHandle, NIDAQmx.Val_ContSamps, 1000);
            NiDaq.GetExtendedErrorInfo(error, 2048);
            nRet = NiDaq.StartTask(taskHandle);
            NiDaq.GetExtendedErrorInfo(error, 2048);
            NiDaq.Dispose();

            return (IntPtr)taskHandle;
        }

        /// <summary>   Stop count. </summary>
        /// <param name="pTaskHandle">  Handle of the task. </param>
        public unsafe static void StopCount(IntPtr pTaskHandle)
        {
            DCSWrapper.NiDAQmxWrapper NiDaq = new NiDAQmxWrapper();
            void* taskHandle = (void*)pTaskHandle;
            NiDaq.StopTask(taskHandle);
            NiDaq.ClearTask(taskHandle);
            NiDaq.Dispose();
        }
    }

    /// <summary>   NIDAQmx factory.  </summary>
    public class NIDAQmxFactory
    {
        /// <summary> The first m task handle </summary>
        private IntPtr m_TaskHandle1;

        /// <summary> The second m task handle </summary>
        private IntPtr m_TaskHandle2;

        /// <summary> The third m task handle </summary>
        private IntPtr m_TaskHandle3;

        /// <summary> The fourth m task handle </summary>
        private IntPtr m_TaskHandle4;

        /// <summary> The ni daq wrapper </summary>
        private NiDAQmxWrapper m_NIDaqWrapper;

        /// <summary>   Gets or sets the task handle 1. </summary>
        /// <value> The task handle 1. </value>
        public IntPtr TaskHandle1
        {
            get { return m_TaskHandle1; }
            set { m_TaskHandle1 = value; }
        }

        /// <summary>   Gets or sets the task handle 2. </summary>
        /// <value> The task handle 2. </value>
        public IntPtr TaskHandle2
        {
            get { return m_TaskHandle2; }
            set { m_TaskHandle2 = value; }
        }

        /// <summary>   Gets or sets the task handle 3. </summary>
        /// <value> The task handle 3. </value>
        public IntPtr TaskHandle3
        {
            get { return m_TaskHandle3; }
            set { m_TaskHandle3 = value; }
        }

        /// <summary>   Gets or sets the task handle 4. </summary>
        /// <value> The task handle 4. </value>
        public IntPtr TaskHandle4
        {
            get { return m_TaskHandle4; }
            set { m_TaskHandle4 = value; }
        }

        /// <summary>   Gets or sets the ni daq wrapper. </summary>
        /// <value> The ni daq wrapper. </value>
        public NiDAQmxWrapper NIDaqWrapper
        {
            get { return m_NIDaqWrapper = new NiDAQmxWrapper(); }
            set { m_NIDaqWrapper = value; }
        }
    }
}
