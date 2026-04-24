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
 * @file  ColorPalette.cs
 * @brief
 *  Color palette class.
 * 
 * @author : suoow <suoow.yeo@haesung.net>
 * @date : 2011.07.30
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.07.30 First creation.
 */

using System;
using System.Windows.Media;

namespace Common
{
    /// <summary>   Color palette.  </summary>
    public static class ColorPalette
    {
        public static Color[] m_colors = new Color[]
        {
            //0
            Colors.White,                   /* 양품 */
            Colors.Black,                   /* 인식마크 불량(ALIGN 인식오류) */
            Colors.Olive,                    /* 원소재 */
            Colors.Red,                      /* Open */
            Colors.Blue,                     /* Short */
            //5
            Colors.Goldenrod,          /* 본드패드 */   
            Colors.SkyBlue,              /* Ball */
            Colors.Fuchsia,              /* PSR PIN HOLE */
            Colors.Green,                 /* PSR 이물질 */
            Colors.LightSteelBlue, /* Crack */
            //10
            Colors.IndianRed,         /* Burr */       
            Colors.Indigo,                /* VentHole */
            Colors.DodgerBlue,     /* Via Hole 미충진 */
            Colors.Tan,                    /* PSR Dam 탈락 */
            Colors.Teal,                   /* BBT Both */
            //15
            Colors.Yellow,               /* PSR Shift */
            Colors.Brown,               /* 외각 */
            Colors.Silver,                /* 연속불량폐기 */
            Colors.Beige,                /* 2D인식불량 */
            Colors.Magenta,         /* STATIC */
            Colors.DarkGray        /* UI 초기화 */
        };

        public static Color GetColor(int anColor)
        {
            try
            {
                if (anColor < m_colors.Length)
                    return m_colors[anColor];
                else
                    return Colors.DarkGray;
            }
            catch
            {
                return Color.FromArgb(120, 192, 192, 192);
            }
        }

        public static int GetIndex(Color anColor)
        {
            try
            {
                int nLength = m_colors.Length;
                for (int nIndex = 0; nIndex < nLength; nIndex++)
                {
                    if (anColor == m_colors[nIndex])
                    {
                        return nIndex;
                    }
                }
                return -1;
            }
            catch
            {
                return -1;
            }
        }
    }
}
