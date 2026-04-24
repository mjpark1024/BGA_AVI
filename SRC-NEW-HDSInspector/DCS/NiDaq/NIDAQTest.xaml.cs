using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DCSWrapper;

namespace DCS.NIDAQ
{
    public partial class NIDAQTest : UserControl
    {
        #region Private member variables.
        /// <summary> The first m task handle </summary>
        private IntPtr m_TaskHandle1 = new IntPtr();

        /// <summary> The second m task handle </summary>
        private IntPtr m_TaskHandle2 = new IntPtr();

        /// <summary> The third m task handle </summary>
        private IntPtr m_TaskHandle3 = new IntPtr();

        /// <summary> The fourth m task handle </summary>
        private IntPtr m_TaskHandle4 = new IntPtr();

        #endregion

        /// <summary>   Initializes a new instance of the ucNIDAQTest class. </summary>
        public NIDAQTest()
        {
            InitializeComponent();
        }

        /// <summary>   Event handler. Called by btnStart for click events. </summary>
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //if (chkCtr0.IsChecked == true)
            //{
            //    m_TaskHandle1 = NIDAQmx.StartCount( m_TaskHandle1, "Dev1/ctr0", "/Dev1/PFI39", Convert.ToInt32(txtDelay0.Text), Convert.ToInt32(txtLow0.Text), Convert.ToInt32(txtHigh0.Text));
            //}

            //if (chkCtr1.IsChecked == true)
            //{
            //    m_TaskHandle2 = NIDAQmx.StartCount( m_TaskHandle2, "Dev1/ctr1", "/Dev1/PFI35", Convert.ToInt32(txtDelay1.Text), Convert.ToInt32(txtLow1.Text), Convert.ToInt32(txtHigh1.Text));
            //}

            //if (chkCtr2.IsChecked == true)
            //{
            //    m_TaskHandle3 = NIDAQmx.StartCount( m_TaskHandle3, "Dev1/ctr2", "/Dev1/PFI31", Convert.ToInt32(txtDelay2.Text), Convert.ToInt32(txtLow2.Text), Convert.ToInt32(txtHigh2.Text));
            //}

            //if (chkCtr3.IsChecked == true)
            //{
            //    m_TaskHandle4 = NIDAQmx.StartCount( m_TaskHandle4, "Dev1/ctr3", "/Dev1/PFI27", Convert.ToInt32(txtDelay3.Text), Convert.ToInt32(txtLow3.Text), Convert.ToInt32(txtHigh3.Text));
            //}
        }

        //unsafe static IntPtr StartCount(NiDAQmxWrapper NiDaq, IntPtr pTaskHandle, string dev, string strInput, int nDelay, int nValue1, int nValue2)
        //{
        //    string strTaskName = "";
        //    void* taskHandle = (void*)pTaskHandle;
        //    sbyte* taskName = stackalloc sbyte[strTaskName.Length + 1];
        //    taskName[strTaskName.Length] = 0;
        //    fixed (char* sTaskName = strTaskName)
        //    {
        //        for (int i = 0; i < strTaskName.Length; i++)
        //            taskName[i] = (sbyte)sTaskName[i]; //Casting unicode! Pay attention
        //    }
        //    // int taskHandle;
        //    int nRet = NiDaq.CreateTask(taskName, &taskHandle);

        //    sbyte* result = stackalloc sbyte[dev.Length + 1];
        //    result[dev.Length] = 0; //nullterminated
        //    fixed (char* p = dev)
        //    {
        //        //copying bytewise
        //        for (int i = 0; i < dev.Length; i++)
        //            result[i] = (sbyte)p[i]; //Casting unicode! Pay attention
        //    }
        //    sbyte* error = stackalloc sbyte[2048];

        //    sbyte* input = stackalloc sbyte[strInput.Length + 1];
        //    result[strInput.Length] = 0; //nullterminated
        //    fixed (char* p = strInput)
        //    {
        //        //copying bytewise
        //        for (int i = 0; i < strInput.Length; i++)
        //            input[i] = (sbyte)p[i]; //Casting unicode! Pay attention
        //    }

        //    // nValue1 = 4
        //    // nValue2 = 4;

        //    nRet = NiDaq.CreateCOPulseChanTicks(taskHandle, result, taskName, input, NIDAQmx.Val_Low, nDelay, nValue1, nValue2);
        //    NiDaq.GetExtendedErrorInfo(error, 2048);
        //    // nRet = NiDaq.CfgDigEdgeStartTrig(taskHandle, input, NIDAQmx.Val_Rising);
        //    // NiDaq.GetExtendedErrorInfo(error, 2048);

        //    // nRet=  NiDaq.CreateCOPulseChanFreq(taskHandle, result, taskName, NIDAQmx.Val_Hz, NIDAQmx.Val_Low, 0.0, 1.00, dValue2);
        //    // NiDaq.CfgDigEdgeStartTrig(taskHandle, "/Dev1/PFI39", NIDAQmx.Val_Rising);
        //    // ctr0     PFI39, ctr2 PFI 31
        //    // NiDaq.GetExtendedErrorInfo(error, 2048);

        //    // nRet = NiDaq.SetCIFreqDigFltrMinPulseWidth(taskHandle, result, 0.0000001);//1.0e-7);
        //    // NiDaq.GetExtendedErrorInfo(error, 2048);
        //    nRet = NiDaq.CfgImplicitTiming(taskHandle, NIDAQmx.Val_ContSamps, 1000);
        //    NiDaq.GetExtendedErrorInfo(error, 2048);
        //    nRet = NiDaq.StartTask(taskHandle);
        //    NiDaq.GetExtendedErrorInfo(error, 2048);

        //    return (IntPtr)taskHandle;
        //}

        /// <summary>   Event handler. Called by btnStop for click events. </summary>
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            //if (chkCtr0.IsChecked == true)
            //{
            //    NIDAQmx.StopCount( m_TaskHandle1);
            //}

            //if (chkCtr1.IsChecked == true)
            //{
            //    NIDAQmx.StopCount(m_TaskHandle2);
            //}

            //if (chkCtr2.IsChecked == true)
            //{
            //    NIDAQmx.StopCount( m_TaskHandle3);
            //}

            //if (chkCtr3.IsChecked == true)
            //{
            //    NIDAQmx.StopCount(m_TaskHandle4);
            //}
        }
    }
}
