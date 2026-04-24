using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DCS.NiDaq
{
    public class NIDAQ
    {
        [DllImport("nidaq32.dll")]
        public static extern short GPCTR_Control(short deviceNumber, uint gpCounterNumber, uint action);
        [DllImport("nidaq32.dll")]
        public static extern short GPCTR_Set_Application (short deviceNumber, uint gpCounterNumber, uint application);
        [DllImport("nidaq32.dll")]
        public static extern short Line_Change_Attribute(short deviceNumber, uint lineNumber, uint attribID, uint attribValue);
        [DllImport("nidaq32.dll")]
        public static extern short GPCTR_Change_Parameter(short deviceNumber, uint gpCounterNumber, uint paramID, uint paramValue);
        [DllImport("nidaq32.dll")]
        public static extern short Select_Signal(short deviceNumber, uint signal, uint source, uint sourceSpec);

        public static void StartCounter(uint anValue1, uint anValue2)
        {
            short nStatus = 0;
            short nDevice = 1;
            nStatus = GPCTR_Control(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_RESET); // Src0 - Power On Check
            nStatus = GPCTR_Set_Application(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_PULSE_TRAIN_GNR); // Src0 연속해서 트리거 발생하도록 설정
            nStatus = Line_Change_Attribute(nDevice, NIDAQConst.ND_PFI_39, NIDAQConst.ND_LINE_FILTER, NIDAQConst.ND_100_NANOSECONDS);
            nStatus = GPCTR_Change_Parameter(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_SOURCE, NIDAQConst.ND_PFI_39); // Src0의 블럭 Input Src
            nStatus = GPCTR_Change_Parameter(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_COUNT_1, anValue1); // 해상도에 맞게 Rate Convert
            nStatus = GPCTR_Change_Parameter(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_COUNT_2, anValue2);
            /* To output a counter pulse. */
            nStatus = Select_Signal(nDevice, NIDAQConst.ND_GPCTR0_OUTPUT, NIDAQConst.ND_GPCTR0_OUTPUT, NIDAQConst.ND_LOW_TO_HIGH);
            nStatus = GPCTR_Control(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_PROGRAM);

            #region From Legacy.
            //begin
            //  iDevice := 1;                       //Device Num
            //  iStatus := GPCTR_Control(iDevice, ND_COUNTER_0, ND_RESET); //Src0 - Power On Check
            //  iStatus := GPCTR_Set_Application(iDevice, ND_COUNTER_0, ND_PULSE_TRAIN_GNR);    //Src0 연속해서 트리거 발생하도록 설정
            //  iStatus := Line_Change_Attribute(iDevice, ND_PFI_39, ND_LINE_FILTER, ND_100_NANOSECONDS);
            //  iStatus := GPCTR_Change_Parameter(iDevice, ND_COUNTER_0, ND_SOURCE, ND_PFI_39); //Src0의 블럭 Input Src
            //  iStatus := GPCTR_Change_Parameter(iDevice, ND_COUNTER_0, ND_COUNT_1, 4); //해상도에 맞게 Rate Convert
            //  iStatus := GPCTR_Change_Parameter(iDevice, ND_COUNTER_0, ND_COUNT_2, 4); //
            //  iStatus := Select_Signal(iDevice, ND_GPCTR0_OUTPUT, ND_GPCTR0_OUTPUT, ND_LOW_TO_HIGH);     //
            //  iStatus := GPCTR_Control(iDevice, ND_COUNTER_0, ND_PROGRAM);
            //end;
            #endregion
        }

        public static void SCACounter()
        {
            short nDevice = 1;
            /* Reset GPCTR */
            short nStatus = GPCTR_Control(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_RESET);
        }

        //public static void ResetCounter(int anSettingType = 1)
        //{
        //    short nStatus = 0;
        //    short nDevice = 1;

        //    if (anSettingType == 1)
        //    {
        //        nStatus = GPCTR_Control(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_RESET); //Src0 - Power On Check
        //        nStatus = GPCTR_Set_Application(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_PULSE_TRAIN_GNR);    //Src0 연속해서 트리거 발생하도록 설정
        //        nStatus = Line_Change_Attribute(nDevice, NIDAQConst.ND_PFI_39, NIDAQConst.ND_LINE_FILTER, NIDAQConst.ND_100_NANOSECONDS);
        //        nStatus = GPCTR_Change_Parameter(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_SOURCE, NIDAQConst.ND_PFI_39); //Src0의 블럭 Input Src
        //        nStatus = GPCTR_Change_Parameter(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_COUNT_1, 3); //해상도에 맞게 Rate Convert
        //        nStatus = GPCTR_Change_Parameter(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_COUNT_2, 3); //
        //        nStatus = Select_Signal(nDevice, NIDAQConst.ND_GPCTR0_OUTPUT, NIDAQConst.ND_GPCTR0_OUTPUT, NIDAQConst.ND_LOW_TO_HIGH);
        //        nStatus = GPCTR_Control(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_PROGRAM);
        //    }
        //    else
        //    {
        //        nStatus = GPCTR_Control(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_RESET); //Src0 - Power On Check
        //        nStatus = GPCTR_Set_Application(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_PULSE_TRAIN_GNR);    //Src0 연속해서 트리거 발생하도록 설정
        //        nStatus = Line_Change_Attribute(nDevice, NIDAQConst.ND_PFI_35, NIDAQConst.ND_LINE_FILTER, NIDAQConst.ND_100_NANOSECONDS);
        //        nStatus = GPCTR_Change_Parameter(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_SOURCE, NIDAQConst.ND_PFI_35); //Src0의 블럭 Input Src
        //        nStatus = GPCTR_Change_Parameter(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_COUNT_1, 3); //해상도에 맞게 Rate Convert
        //        nStatus = GPCTR_Change_Parameter(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_COUNT_2, 3); //
        //        nStatus = Select_Signal(nDevice, NIDAQConst.ND_GPCTR0_OUTPUT, NIDAQConst.ND_GPCTR0_OUTPUT, NIDAQConst.ND_LOW_TO_HIGH);
        //        nStatus = GPCTR_Control(nDevice, NIDAQConst.ND_COUNTER_0, NIDAQConst.ND_PROGRAM);
        //    }

        //    #region From Legacy.
        //    //  begin
        //    //    iStatus := GPCTR_Control(iDevice, ND_COUNTER_0, ND_RESET); //Src0 - Power On Check
        //    //    iStatus := GPCTR_Set_Application(iDevice, ND_COUNTER_0, ND_PULSE_TRAIN_GNR);    //Src0 연속해서 트리거 발생하도록 설정
        //    //    iStatus := Line_Change_Attribute(iDevice, ND_PFI_39, ND_LINE_FILTER, ND_100_NANOSECONDS);
        //    //    iStatus := GPCTR_Change_Parameter(iDevice, ND_COUNTER_0, ND_SOURCE, ND_PFI_39); //Src0의 블럭 Input Src
        //    //    iStatus := GPCTR_Change_Parameter(iDevice, ND_COUNTER_0, ND_COUNT_1, 3); //해상도에 맞게 Rate Convert
        //    //    iStatus := GPCTR_Change_Parameter(iDevice, ND_COUNTER_0, ND_COUNT_2, 3); //
        //    //    iStatus := Select_Signal(iDevice, ND_GPCTR0_OUTPUT, ND_GPCTR0_OUTPUT, ND_LOW_TO_HIGH);     //
        //    //    iStatus := GPCTR_Control(iDevice, ND_COUNTER_0, ND_PROGRAM);
        //    //  end else
        //    //  begin
        //    //    iStatus := GPCTR_Control(iDevice, ND_COUNTER_0, ND_RESET); //Src0 - Power On Check
        //    //    iStatus := GPCTR_Set_Application(iDevice, ND_COUNTER_0, ND_PULSE_TRAIN_GNR);    //Src0 연속해서 트리거 발생하도록 설정
        //    //    iStatus := Line_Change_Attribute(iDevice, ND_PFI_35, ND_LINE_FILTER, ND_100_NANOSECONDS);
        //    //    iStatus := GPCTR_Change_Parameter(iDevice, ND_COUNTER_0, ND_SOURCE, ND_PFI_35); //Src0의 블럭 Input Src
        //    //    iStatus := GPCTR_Change_Parameter(iDevice, ND_COUNTER_0, ND_COUNT_1, 3); //해상도에 맞게 Rate Convert
        //    //    iStatus := GPCTR_Change_Parameter(iDevice, ND_COUNTER_0, ND_COUNT_2, 3); //
        //    //    iStatus := Select_Signal(iDevice, ND_GPCTR0_OUTPUT, ND_GPCTR0_OUTPUT, ND_LOW_TO_HIGH);     //
        //    //    iStatus := GPCTR_Control(iDevice, ND_COUNTER_0, ND_PROGRAM);
        //    //  end;
        //    #endregion
        //}
    }
}
