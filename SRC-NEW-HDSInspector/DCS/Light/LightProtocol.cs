using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCS.Light
{
    public class LightProtocol
    {
        // "setbrightness channel 밝기"0x0D    channel 0번 Index 부터
        // "getbrightness channel"0X0D

        // "setonex 00000000"0xOD    , setonex ffffffff"0x0D  1~32

        // "getonex"OXOD
        // "getonex ffff"0X0D

        public enum LIGHT_TYPE
        {
            PLK_MODULE_2CH = 0,
            PLK_MODULE_4CH = 1,
            PLK_MODULE_8CH = 2,
            PLK_MODULE_16CH = 3,
            PLK_MODULE_6CH = 4
        };

        public LIGHT_TYPE LightType { get; set; }

        public int SelectChannelType()
        {
            int nValue = 0;
            switch (LightType)
            {

                case LIGHT_TYPE.PLK_MODULE_2CH:
                    nValue = (int)Math.Pow(2, 2) - 1;
                    break;

                case LIGHT_TYPE.PLK_MODULE_4CH:
                    nValue = (int)Math.Pow(2, 4) - 1;
                    break;

                case LIGHT_TYPE.PLK_MODULE_6CH:
                    nValue = (int)Math.Pow(2, 6) - 1;
                    break;

                case LIGHT_TYPE.PLK_MODULE_8CH:
                    nValue = (int)Math.Pow(2, 8) - 1;
                    break;

                case LIGHT_TYPE.PLK_MODULE_16CH:
                    nValue = (int)Math.Pow(2, 16) - 1;
                    break;
            }

            return nValue;

        }

        // nCahnnel 은 1인텍스
        public String SetBrightness(int nChannel, int anValue)
        {
            String strValue;

            if (anValue < 0)
                anValue = 0;
            if (anValue > 255)
                anValue = 255;

            strValue = String.Format("setbrightness {0:D} {1:D}\r", nChannel - 1, anValue);

            return strValue;
        }

        public String SetOnEx(int nValue)
        {
            String strValue = String.Format("setonex {0:X8}\r", nValue);
            return strValue;
        }

        public String GetOnex()
        {
            String strValue = String.Format("getonex {0:X8}\r", this.SelectChannelType());
            return strValue;
        }

        public String SetOnAll(bool bValue, int nType = 0)                 //nType = 1    CH 1, 2    // nType = 2  CH 3,4
        {
            String strValue = String.Empty;
            switch (nType)
            {
                case 1:
                    break;
                case 2:
                    break;
                default:
                    int nTemp = SelectChannelType();

                    if (bValue)
                    {
                        strValue = String.Format("setonex {0:X8}\r", nTemp);
                    }
                    else
                    {
                        strValue = String.Format("setonex {0:X8}\r", 0);
                    }
                    break;
            }

            return strValue;

        }

        public int GetBrightness(int nChannel)
        {
            int nValue = 0;

            return nValue;
        }
    }
}
