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
 * @file  DefaultInspectionTypeValue.cs
 * @brief 
 *  검사 설정 아이템들에 대한 기본 값을 불러오는 역할을 수행한다.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.10.20
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.10.20 First creation.
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Drawing.InspectionInformation
{
    #region PSROdd.
    /// <summary>    PSROdd default value.  </summary>
    public static class PSROddDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        //Common
        public static int Default_ThreshType;                   // 0000


        //Core_RGB
        public static int Default_Core_Threshold;               //0060
        public static int Default_Core_ExceptionThreshold;      //0062
        public static int Default_Core_MinDefectSize;           //0063

        //Metal_채도
        public static int Default_Metal_LowerThreshold;         //0064
        public static int Default_Metal_UpperThreshold;         //0065
        public static int Default_Metal_MinDefectSize;          //0066

        //Circuit
        public static int Default_Circuit_Threshold;            //0067
        public static int Default_Circuit_MinDefectSize;        //0068

        // Nomal
        public static int Default_Summation_range;              //0069
        public static int Default_Summation_detection_size;     //0070
        public static int Default_Mask_Threshold;               //0071
        public static int Default_Mask_Extension;               //0072
        public static int Default_Step_Threshold;               //0073
        public static int Default_Step_Expansion;               //0074

        //필터
        public static int Default_HV_ratio_value;               //0075
        public static int Default_Min_Relative_size;            //0076
        public static int Default_Max_Relative_size;            //0077
        public static int Default_Area_Relative;                //0078

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0450";
            string PSROddStatecode = "3026";

            Default_ThreshType = 0;                     //0000
            //Core_RGB
            Default_Core_Threshold = 125;                //0060
            Default_Core_ExceptionThreshold = 20;       //0062
            Default_Core_MinDefectSize = 100;           //0063

            //Metal_채도
            Default_Metal_LowerThreshold = 40;          //0064
            Default_Metal_UpperThreshold = 50;          //0065
            Default_Metal_MinDefectSize = 150;          //0066

            //Circuit
            Default_Circuit_Threshold = 30;             //0067
            Default_Circuit_MinDefectSize = 200;        //0068

            // Nomal
            Default_Summation_range = 30;               //0069
            Default_Summation_detection_size = 200;     //0070
            Default_Mask_Threshold = 100;                //0071
            Default_Mask_Extension = 2;                 //0072
            Default_Step_Threshold = 40;               //0073
            Default_Step_Expansion = 2;                 //0074

            //필터
            Default_HV_ratio_value = 5;                 //0075
            Default_Min_Relative_size = 5;              //0076
            Default_Max_Relative_size = 4;              //0077
            Default_Area_Relative = 255;                //0078
        }

    }
    #endregion

    #region Fiducial Align type.
    /// <summary>   Strip align default value.  </summary>
    public static class StripAlignDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int AlignMarginX;     // 0043
        public static int AlignMarginY;     // 0044
        public static int AlignAcceptance;  // 0045

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0101";
            string fiducialAlignCode = "3001";
            AlignMarginX = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, fiducialAlignCode, "0043");
            AlignMarginY = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, fiducialAlignCode, "0044");
            AlignAcceptance = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, fiducialAlignCode, "0045");
        }
    }

    /// <summary>   Unit align default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class UnitAlignDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int AlignMarginX;     // 0043
        public static int AlignMarginY;     // 0044
        public static int AlignAcceptance;  // 0045

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0401";
            string fiducialAlignCode = "3001";
            AlignMarginX = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, fiducialAlignCode, "0043");
            AlignMarginY = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, fiducialAlignCode, "0044");
            AlignAcceptance = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, fiducialAlignCode, "0045");
        }
    }

    /// <summary>   Outer Fiducial default value.  </summary>
    /// <remarks>   suoow2, 2020-02-20. </remarks>
    public static class OuterFiducialDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int MarginX;     // 0043
        public static int MarginY;     // 0044
        public static int Acceptance;  // 0045

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0205";
            string fiducialAlignCode = "3025";
            MarginX = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, fiducialAlignCode, "0043");
            MarginY = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, fiducialAlignCode, "0044");
            Acceptance = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, fiducialAlignCode, "0045");
        }
    }
    #endregion

    #region Vent Hole Type.
    /// <summary>   Space shape with center line default value.  </summary>
    /// <remarks>   suoow2, 2012-03-30. </remarks>
    public static class VentHoleDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ThreshType;       // 0000
        public static int LowerThresh;      // 0001
        public static int MinDefectSize;    // 0015

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0434";
            string VentHoleCode = "3015";
            ThreshType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, VentHoleCode, "0000");
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, VentHoleCode, "0001");
            MinDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, VentHoleCode, "0015");
        }
    }
    #endregion


    #region Outre Vent Hole Type.
    /// <summary>   Space shape with center line default value.  </summary>
    /// <remarks>   suoow2, 2012-03-30. </remarks>
    public static class ReSizeVentHoleDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ThreshType;       // 0000
        public static int LowerThresh;      // 0001
        public static int MinDefectSize;    // 0015

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0434";
            string VentHoleCode = "3015";
            ThreshType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, VentHoleCode, "0000");
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, VentHoleCode, "0001");
            MinDefectSize = 40;
        }
    }
    #endregion

    #region Unit Pattern Type.
    /// <summary>   UnitPattern default value.  </summary>
    /// <remarks>   suoow2, 2012-03-30. </remarks>
    public static class UnitPatternDefaultValue
    {
        public static bool DefaultValueLoaded = false;
        public static int LowerThresh;      // 0001
        public static int MinDefectSize;    // 0030

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0407";
            string crossPatternCode = "3022";
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, crossPatternCode, "0001");
            MinDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, crossPatternCode, "0015");
        }
    }
    #endregion

    #region Cross Pattern Type.
    /// <summary>   Space shape with center line default value.  </summary>
    /// <remarks>   suoow2, 2012-03-30. </remarks>
    public static class CrossPatternDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ThreshType;       // 0000
        public static int LowerThresh;      // 0001
        public static int UpperThresh;      // 0002
        public static int MinDefectSize;    // 0030
        public static int MinHeightsize;    // 0035
        public static int PsrMarginX;       // 0049
        public static int PsrMarginY;       // 0050
        public static int UsePSRShift;      // 0048
        public static int UsePSRShiftBA;    // 0073 //jiwon
        public static int UsePunchShift;    // 0052

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0411";
            string crossPatternCode = "3003";
            ThreshType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, crossPatternCode, "0000");
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, crossPatternCode, "0001");
            UpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, crossPatternCode, "0002");
            UsePSRShift = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, crossPatternCode, "0048");
            UsePSRShiftBA = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, crossPatternCode, "0073"); // jiwon
            PsrMarginX = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, crossPatternCode, "0049");
            PsrMarginY = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, crossPatternCode, "0050");
            UsePunchShift = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, crossPatternCode, "0052");
            MinDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, crossPatternCode, "0015");
            MinHeightsize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, crossPatternCode, "0035");

        }
    }
    #endregion

    #region Window Punch Type.
    /// <summary>   Window Punch default value.  </summary>
    /// <remarks>   suoow2, 2016-03-06. </remarks>
    public static class WindowPunchDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ThreshType;       // 0000
        public static int LowerThresh;      // 0001
        public static int UpperThresh;      // 0002
        public static int MinDefectSize;    // 0030
        public static int MinHeightsize;    // 0035
        public static int PsrMarginX;   // 0038
        public static int PsrMarginY;   // 0039
        public static int UsePSRShift;   //0006
        public static int UsePunchShift;   //0006
        public static int ErosionTrainIter; //0011

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0428";
            string windowPunchCode = "3019";
            ThreshType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, windowPunchCode, "0000");
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, windowPunchCode, "0001");
            UpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, windowPunchCode, "0002");
            ErosionTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, windowPunchCode, "0011");
            UsePSRShift = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, windowPunchCode, "0048");
            PsrMarginX = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, windowPunchCode, "0049");
            PsrMarginY = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, windowPunchCode, "0050");
            UsePunchShift = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, windowPunchCode, "0052");
            MinDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, windowPunchCode, "0015");
            MinHeightsize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, windowPunchCode, "0035");

        }
    }
    #endregion

    #region Lead Shape With Center Line Type.
    /// <summary>   Lead shape with center line default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class LeadShapeWithCenterLineDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ThreshType;       // 0000
        public static int LowerThresh;      // 0001
        public static int UpperThresh;      // 0002
        public static int MaskLowerThresh;  // 0004
        public static int MinDefectSize;    // 0015
        public static int MinWidthRatio;    // 0030
        public static int MinWidthSize;     // 0031
        public static int MaxWidthRatio;    // 0032
        public static int MaxWidthSize;     // 0033
        public static int MinHeightsize;    // 0035
        public static int MinNormalRatio;   // 0038
        public static int MaxNormalRatio;   // 0039
        public static int RemoveTipSize;

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0415";
            string leadShapeWithCenterLineCode = "3008";
            ThreshType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0000");
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0001");
            UpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0002");
            MaskLowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0004");
            MinDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0015");
            MinWidthRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0030");
            MinWidthSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0031");
            MaxWidthRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0032");
            MaxWidthSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0033");
            MinHeightsize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0035");
            MinNormalRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0038");
            MaxNormalRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0039");
            RemoveTipSize = 0;
        }
    }
    #endregion

    #region Space Shape With Center Line Type.
    /// <summary>   Space shape with center line default value.  </summary>
    /// <remarks>   suoow2, 2012-03-30. </remarks>
    public static class SpaceShapeWithCenterLineDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ThreshType;       // 0000
        public static int LowerThresh;      // 0001
        public static int UpperThresh;      // 0002
        public static int MinWidthRatio;    // 0030
        public static int MinWidthSize;     // 0031
        public static int MaxWidthRatio;    // 0032
        public static int MaxWidthSize;     // 0033
        public static int MinHeightsize;    // 0035
        public static int MinNormalRatio;   // 0038
        public static int MaxNormalRatio;   // 0039
        public static int TipSearchSize;    // 0099

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0436";
            string leadShapeWithCenterLineCode = "3008";
            ThreshType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0000");
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0001");
            UpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0002");
            MinWidthRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0030");
            MinWidthSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0031");
            MaxWidthRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0032");
            MaxWidthSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0033");
            MinHeightsize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0035");
            MinNormalRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0038");
            MaxNormalRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0039");
            TipSearchSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, leadShapeWithCenterLineCode, "0099");
        }
    }
    #endregion

    #region Lead Gap Type.
    /// <summary>   Lead gap default value.  </summary>
    /// <remarks>   Shin.Cheol-min, 2014-11-16. </remarks>
    public static class LeadGapDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int LowerThresh;      // 0001
        public static int UpperThresh;      // 0002
        public static int MinWidthSize;     // 0031

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0435";
            string LeadGapCode = "3017";
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, LeadGapCode, "0001");
            UpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, LeadGapCode, "0002");
            MinWidthSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, LeadGapCode, "0031");
        }
    }
    #endregion

    #region Groove With Center Line Type.
    /// <summary>   Groove default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class GrooveDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ThreshType;           // 0000
        public static int LowerThresh;          // 0001
        public static int UpperThresh;          // 0002
        public static int ApplyAverDiff;        // 0003
        public static int AverMinMargin;        // 0007
        public static int AverMaxMargin;        // 0008
        public static int MinWidthRatio;        // 0030
        public static int MinWidthSize;         // 0031
        public static int MaxWidthRatio;        // 0032
        public static int MaxWidthSize;         // 0033
        public static int MinHeightSize;        // 0035
        public static int MaxHeightRatio;       // 0036
        public static int MaxHeightSize;        // 0037
        public static int CriterionSize;        // 0048
        public static int MinNormalRatio;       // 0038
        public static int MaxNormalRatio;       // 0039

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0416";
            string grooveCode = "3009";
            ThreshType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, grooveCode, "0000");
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, grooveCode, "0001");
            UpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, grooveCode, "0002");
            ApplyAverDiff = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, grooveCode, "0003");
            AverMinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, grooveCode, "0007");
            AverMaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, grooveCode, "0008");
            MinWidthRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, grooveCode, "0030");
            MinWidthSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, grooveCode, "0031");
            MaxWidthRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, grooveCode, "0032");
            MaxWidthSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, grooveCode, "0033");
            MinHeightSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, grooveCode, "0035");
            MaxHeightRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, grooveCode, "0036");
            MaxHeightSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, grooveCode, "0037");
            CriterionSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, grooveCode, "0048");
            MinNormalRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, grooveCode, "0038");
            MaxNormalRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, grooveCode, "0039");
        }
    }
    #endregion

    #region Figure Shape Type.
    /// <summary>   Shape half etching default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class ShapeHalfEtchingDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int InspRange;            // 0006
        public static int AverMinMargin;        // 0007
        public static int AverMaxMargin;        // 0008
        public static int MinMargin;            // 0009
        public static int MaxMargin;            // 0010
        public static int ErosionTrainIter;     // 0011
        public static int DilationTrainIter;    // 0012
        public static int ErosionInspIter;      // 0013
        public static int DilationInspIter;     // 0014
        public static int MinWidthSize;         // 0031
        public static int MinHeightSize;        // 0035
        public static int MinNormalRatio;       // 0038
        public static int MaxNormalRatio;       // 0039
        public static int AlignMaxDist;         // 0046
        public static int DarkAreaWidth;        // 0047

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0417";
            string figureShapeCode = "3010";
            InspRange = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, figureShapeCode, "0006");
            AverMinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, figureShapeCode, "0007");
            AverMaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, figureShapeCode, "0008");
            MinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, figureShapeCode, "0009");
            MaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, figureShapeCode, "0010");
            ErosionTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, figureShapeCode, "0011");
            DilationTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, figureShapeCode, "0012");
            ErosionInspIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, figureShapeCode, "0013");
            DilationInspIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, figureShapeCode, "0014");
            MinWidthSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, figureShapeCode, "0031");
            MinHeightSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, figureShapeCode, "0035");
            MinNormalRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, figureShapeCode, "0038");
            MaxNormalRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, figureShapeCode, "0039");
            AlignMaxDist = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, figureShapeCode, "0046");
            DarkAreaWidth = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, figureShapeCode, "0047");
        }
    }
    #endregion

    #region State Intensity type.
    /// <summary>   Down set default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class RailDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ThreshType;              // 0000
        public static int LowerThresh;             // 0001
        public static int UpperThresh;             // 0002
        public static int ApplyAverDiff;           // 0003
        public static int InspRange;               // 0006
        public static int AverMinMargin;           // 0007
        public static int AverMaxMargin;           // 0008
        public static int MinMargin;               // 0009
        public static int MaxMargin;               // 0010
        public static int ErosionTrainIter = 0;    // 0011
        public static int DilationTrainIter = 0;   // 0012
        public static int ErosionInspIter = 0;     // 0013
        public static int DilationInspIter = 0;    // 0014
        public static int MinDefectSize;           // 0015
        public static int Invert;                  // 0020

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0419";
            string stateIntensityCode = "3011";
            ThreshType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateIntensityCode, "0000");
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateIntensityCode, "0001");
            UpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateIntensityCode, "0002");
            ApplyAverDiff = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateIntensityCode, "0003");
            InspRange = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateIntensityCode, "0006");
            AverMinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateIntensityCode, "0007");
            AverMaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateIntensityCode, "0008");
            MinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateIntensityCode, "0009");
            MaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateIntensityCode, "0010");
            ErosionTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateIntensityCode, "0011");
            DilationTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateIntensityCode, "0012");
            ErosionInspIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateIntensityCode, "0013");
            DilationInspIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateIntensityCode, "0014");
            MinDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateIntensityCode, "0015");
            Invert = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateIntensityCode, "0020");
        }
    }
    #endregion

    #region Ball Pattern Mask type.
    /// <summary>   Surface half etching default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class BallPatternDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ThreshType;              // 0000
        public static int LowerThresh;             // 0001
        public static int UpperThresh;             // 0002
        public static int ApplyAverDiff;           // 0003
        public static int MaskLowerThresh;         // 0004
        public static int MaskUpperThresh;         // 0005
        public static int InspRange;               // 0006
        public static int AverMinMargin;           // 0007
        public static int AverMaxMargin;           // 0008
        public static int MinMargin;               // 0009
        public static int MaxMargin;               // 0010
        public static int ErosionTrainIter = 0;    // 0011
        public static int DilationTrainIter = 0;   // 0012
        public static int ErosionInspIter = 0;     // 0013
        public static int DilationInspIter = 0;    // 0014
        public static int MinDefectSize;           // 0015
        public static int MinSmallDefectSize;      // 0016
        public static int MinSmallDefectCount;     // 0017
        public static int MinWidthSize;
        public static int MinHeightSize;
        public static int MaxHeightSize;
        public static int MaxDefectSize;           // 0064
        public static int MaxSmallDefectSize;      // 0065
        public static int MaxSmallDefectCount;     // 0066
        public static int UsePSRShift;
        public static int UsePunchShift;
        public static int SameValue;

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0429";
            string stateAlignedMaskcode = "3013";
            ThreshType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0000");
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0001");
            UpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0002");
            ApplyAverDiff = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0003");
            MaskLowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0004");
            MaskUpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0005");
            InspRange = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0006");
            AverMinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0007");
            AverMaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0008");
            MinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0009");
            MaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0010");
            ErosionTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0011");
            DilationTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0012");
            ErosionInspIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0013");
            DilationInspIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0014");
            MinDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0015");
            MinSmallDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0016");
            MinSmallDefectCount = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0017");
            MinWidthSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0031");
            MinHeightSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0035");
            MaxHeightSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0037");
            UsePSRShift = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0048");
            UsePunchShift = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0052");
            MaxDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0064");
            MaxSmallDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0065");
            MaxSmallDefectCount = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0066");
            SameValue = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0999");
        }
    }
    #endregion

    #region State Aligned Mask type.
    /// <summary>   Surface half etching default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class SurfaceHalfEtchingDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ThreshType;              // 0000
        public static int LowerThresh;             // 0001
        public static int UpperThresh;             // 0002
        public static int ApplyAverDiff;           // 0003
        public static int MaskLowerThresh;         // 0004
        public static int MaskUpperThresh;         // 0005
        public static int InspRange;               // 0006
        public static int AverMinMargin;           // 0007
        public static int AverMaxMargin;           // 0008
        public static int MinMargin;               // 0009
        public static int MaxMargin;               // 0010
        public static int ErosionTrainIter = 0;    // 0011
        public static int DilationTrainIter = 0;   // 0012
        public static int ErosionInspIter = 0;     // 0013
        public static int DilationInspIter = 0;    // 0014
        public static int MinDefectSize;           // 0015
        public static int MinSmallDefectSize;      // 0016
        public static int MinSmallDefectCount;     // 0017
        public static int MaxDefectSize;           // 0064
        public static int MaxSmallDefectSize;      // 0065
        public static int MaxSmallDefectCount;     // 0066
        public static int SameValue;

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0418";
            string stateAlignedMaskcode = "3012";
            ThreshType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0000");
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0001");
            UpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0002");
            ApplyAverDiff = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0003");
            MaskLowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0004");
            MaskUpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0005");
            InspRange = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0006");
            AverMinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0007");
            AverMaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0008");
            MinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0009");
            MaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0010");
            ErosionTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0011");
            DilationTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0012");
            ErosionInspIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0013");
            DilationInspIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0014");
            MinDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0015");
            MinSmallDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0016");
            MinSmallDefectCount = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0017");
            MaxDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0064");
            MaxSmallDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0065");
            MaxSmallDefectCount = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0066");
            SameValue = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0999");
        }
    }

    /// <summary>   Surface default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class SurfaceDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ThreshType;              // 0000
        public static int LowerThresh;             // 0001
        public static int UpperThresh;             // 0002
        public static int ApplyAverDiff;           // 0003
        public static int MaskLowerThresh;         // 0004
        public static int MaskUpperThresh;         // 0005
        public static int InspRange;               // 0006
        public static int AverMinMargin;           // 0007
        public static int AverMaxMargin;           // 0008
        public static int MinMargin;               // 0009
        public static int MaxMargin;               // 0010
        public static int ErosionTrainIter = 0;    // 0011
        public static int DilationTrainIter = 0;   // 0012
        public static int ErosionInspIter = 0;     // 0013
        public static int DilationInspIter = 0;    // 0014
        public static int MinDefectSize;           // 0015
        public static int MinSmallDefectSize;      // 0016
        public static int MinSmallDefectCount;     // 0017
        public static int MaxDefectSize;           // 0064
        public static int MaxSmallDefectSize;      // 0065
        public static int MaxSmallDefectCount;     // 0066
        public static int SameValue;

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0422";
            string stateAlignedMaskcode = "3012";
            ThreshType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0000");
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0001");
            UpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0002");
            ApplyAverDiff = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0003");
            MaskLowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0004");
            MaskUpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0005");
            InspRange = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0006");
            AverMinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0007");
            AverMaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0008");
            MinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0009");
            MaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0010");
            ErosionTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0011");
            DilationTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0012");
            ErosionInspIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0013");
            DilationInspIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0014");
            MinDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0015");
            MinSmallDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0016");
            MinSmallDefectCount = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0017");
            MaxDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0064");
            MaxSmallDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0065");
            MaxSmallDefectCount = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0066");
            SameValue = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0999");
        }
    }

    /// <summary>   Space default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class SpaceDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ThreshType;              // 0000
        public static int LowerThresh;             // 0001
        public static int UpperThresh;             // 0002
        public static int ApplyAverDiff;           // 0003
        public static int MaskLowerThresh;         // 0004
        public static int MaskUpperThresh;         // 0005
        public static int InspRange;               // 0006
        public static int AverMinMargin;           // 0007
        public static int AverMaxMargin;           // 0008
        public static int MinMargin;               // 0009
        public static int MaxMargin;               // 0010
        public static int ErosionTrainIter = 0;    // 0011
        public static int DilationTrainIter = 0;   // 0012
        public static int ErosionInspIter = 0;     // 0013
        public static int DilationInspIter = 0;    // 0014
        public static int MinDefectSize;           // 0015
        public static int MinSmallDefectSize;      // 0016
        public static int MinSmallDefectCount;     // 0017
        public static int MaxDefectSize;           // 0064
        public static int MaxSmallDefectSize;      // 0065
        public static int MaxSmallDefectCount;     // 0066
        public static int SameValue;

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0423";
            string stateAlignedMaskcode = "3012";
            ThreshType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0000");
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0001");
            UpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0002");
            ApplyAverDiff = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0003");
            MaskLowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0004");
            MaskUpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0005");
            InspRange = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0006");
            AverMinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0007");
            AverMaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0008");
            MinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0009");
            MaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0010");
            ErosionTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0011");
            DilationTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0012");
            ErosionInspIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0013");
            DilationInspIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0014");
            MinDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0015");
            MinSmallDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0016");
            MinSmallDefectCount = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0017");
            MaxDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0064");
            MaxSmallDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0065");
            MaxSmallDefectCount = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0066");
            SameValue = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, stateAlignedMaskcode, "0999");
        }
    }
    #endregion

    #region Outer State type.
    /// <summary>   Outer State default value.  </summary>
    /// <remarks>   suoow2, 2020-01-04. </remarks>
    public static class OuterDefaultValue
    {
        public static bool DefaultValueLoaded = false;
        public static int LowerThresh;             // 0001
        public static int UpperThresh;             // 0002
        public static int MaskLowerThresh;         // 0004
        public static int MaskUpperThresh;         // 0005
        public static int ErosionTrainIter = 0;    // 0011
        public static int DilationTrainIter = 0;   // 0012
        public static int MinDefectSize;           // 0015
        public static int MaxDefectSize;           // 0064

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0211";
            string OuterStatecode = "3023";
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, OuterStatecode, "0001");
            UpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, OuterStatecode, "0002");
            MaskLowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, OuterStatecode, "0004");
            MaskUpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, OuterStatecode, "0005");
            ErosionTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, OuterStatecode, "0011");
            DilationTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, OuterStatecode, "0012");
            MinDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, OuterStatecode, "0015");
            MaxDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, OuterStatecode, "0064");
        }
    }
    #endregion

    #region Shape Shift type.
    /// <summary>   Tape default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class TapeDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ThreshType;              // 0000
        public static int LowerThresh;             // 0001
        public static int UpperThresh;             // 0002
        public static int ApplyAverDiff;           // 0003
        public static int InspRange;               // 0006
        public static int AverMinMargin;           // 0007
        public static int AverMaxMargin;           // 0008
        public static int MinMargin;               // 0009
        public static int MaxMargin;               // 0010
        public static int ErosionTrainIter;        // 0011
        public static int DilationTrainIter;       // 0012
        public static int MinDefectSize;           // 0015
        public static int ErosionShapeIter;        // 0021  
        public static int DilationShapeIter;       // 0022
        public static int GroundLowerThresh;       // 0023
        public static int GroundUpperThresh;       // 0024
        public static int ShapeLowerThresh;        // 0025
        public static int ShapeUpperThresh;        // 0026
        public static int CorrectMethod;           // 0027
        public static int ShapeShiftMarginX;       // 0028
        public static int ShapeShiftMarginY;       // 0029
        public static int ThreshType2;             // 0049
        public static int InspLowerThresh2;        // 0050
        public static int InspUpperThresh2;        // 0051
        public static int ApplyAverDiff2;          // 0052
        public static int AverMinMargin2;          // 0053
        public static int AverMaxMargin2;          // 0054
        public static int InspRange2;              // 0055
        public static int InspMinMargin2;          // 0056
        public static int InspMaxMargin2;          // 0057
        public static int MinDefectSize2;          // 0058
        public static int IsInspectMaster;         // 0059
        public static int IsInspectSlave;          // 0060
        public static int ShapeShiftType;          // 0061
        public static int ShapeOffsetX;            // 0062
        public static int ShapeOffsetY;            // 0063
        public static int MaxDefectSize;           // 0064
        public static int MaxSmallDefectSize;      // 0065
        public static int MaxSmallDefectCount;     // 0066
        public static int MinSmallDefectSize2;     // 0067
        public static int MinSmallDefectCount2;    // 0068
        public static int MaxDefectSize2;          // 0069
        public static int MaxSmallDefectSize2;     // 0070
        public static int MaxSmallDefectCount2;    // 0071

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0420";
            string shapeShiftCode = "3005";
            ThreshType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0000");
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0001");
            UpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0002");
            ApplyAverDiff = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0003");
            InspRange = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0006");
            AverMinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0007");
            AverMaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0008");
            MinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0009");
            MaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0010");
            ErosionTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0011");
            DilationTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0012");
            MinDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0015");
            ErosionShapeIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0021");
            DilationShapeIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0022");
            GroundLowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0023");
            GroundUpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0024");
            ShapeLowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0025");
            ShapeUpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0026");
            CorrectMethod = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0027");
            ShapeShiftMarginX = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0028");
            ShapeShiftMarginY = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0029");
            ThreshType2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0049");
            InspLowerThresh2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0050");
            InspUpperThresh2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0051");
            ApplyAverDiff2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0052");
            AverMinMargin2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0053");
            AverMaxMargin2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0054");
            InspRange2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0055");
            InspMinMargin2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0056");
            InspMaxMargin2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0057");
            MinDefectSize2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0058");
            IsInspectMaster = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0059");
            IsInspectSlave = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0060");
            ShapeShiftType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0061");
            ShapeOffsetX = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0062");
            ShapeOffsetY = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0063");
            MaxDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0064");
            MaxSmallDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0065");
            MaxSmallDefectCount = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0066");
            MinSmallDefectSize2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0067");
            MinSmallDefectCount2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0068");
            MaxDefectSize2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0069");
            MaxSmallDefectSize2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0070");
            MaxSmallDefectCount2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0071");
        }
    }

    /// <summary>   Plate default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class PlateDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ThreshType;              // 0000
        public static int LowerThresh;             // 0001
        public static int UpperThresh;             // 0002
        public static int ApplyAverDiff;           // 0003
        public static int InspRange;               // 0006
        public static int AverMinMargin;           // 0007
        public static int AverMaxMargin;           // 0008
        public static int MinMargin;               // 0009
        public static int MaxMargin;               // 0010
        public static int ErosionTrainIter;        // 0011
        public static int DilationTrainIter;       // 0012
        public static int MinDefectSize;           // 0015
        public static int ErosionShapeIter;        // 0021  
        public static int DilationShapeIter;       // 0022
        public static int GroundLowerThresh;       // 0023
        public static int GroundUpperThresh;       // 0024
        public static int ShapeLowerThresh;        // 0025
        public static int ShapeUpperThresh;        // 0026
        public static int CorrectMethod;           // 0027
        public static int ShapeShiftMarginX;       // 0028
        public static int ShapeShiftMarginY;       // 0029
        public static int ThreshType2;             // 0049
        public static int InspLowerThresh2;        // 0050
        public static int InspUpperThresh2;        // 0051
        public static int ApplyAverDiff2;          // 0052
        public static int AverMinMargin2;          // 0053
        public static int AverMaxMargin2;          // 0054
        public static int InspRange2;              // 0055
        public static int InspMinMargin2;          // 0056
        public static int InspMaxMargin2;          // 0057
        public static int MinDefectSize2;          // 0058
        public static int IsInspectMaster;         // 0059
        public static int IsInspectSlave;          // 0060
        public static int ShapeShiftType;          // 0061
        public static int ShapeOffsetX;            // 0062
        public static int ShapeOffsetY;            // 0063
        public static int MaxDefectSize;           // 0064
        public static int MaxSmallDefectSize;      // 0065
        public static int MaxSmallDefectCount;     // 0066
        public static int MinSmallDefectSize2;     // 0067
        public static int MinSmallDefectCount2;    // 0068
        public static int MaxDefectSize2;          // 0069
        public static int MaxSmallDefectSize2;     // 0070
        public static int MaxSmallDefectCount2;    // 0071

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0421";
            string shapeShiftCode = "3005";
            ThreshType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0000");
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0001");
            UpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0002");
            ApplyAverDiff = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0003");
            InspRange = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0006");
            AverMinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0007");
            AverMaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0008");
            MinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0009");
            MaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0010");
            ErosionTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0011");
            DilationTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0012");
            MinDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0015");
            ErosionShapeIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0021");
            DilationShapeIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0022");
            GroundLowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0023");
            GroundUpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0024");
            ShapeLowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0025");
            ShapeUpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0026");
            CorrectMethod = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0027");
            ShapeShiftMarginX = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0028");
            ShapeShiftMarginY = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0029");
            ThreshType2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0049");
            InspLowerThresh2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0050");
            InspUpperThresh2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0051");
            ApplyAverDiff2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0052");
            AverMinMargin2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0053");
            AverMaxMargin2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0054");
            InspRange2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0055");
            InspMinMargin2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0056");
            InspMaxMargin2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0057");
            MinDefectSize2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0058");
            IsInspectMaster = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0059");
            IsInspectSlave = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0060");
            ShapeShiftType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0061");
            ShapeOffsetX = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0062");
            ShapeOffsetY = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0063");
            MaxDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0064");
            MaxSmallDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0065");
            MaxSmallDefectCount = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0066");
            MinSmallDefectSize2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0067");
            MinSmallDefectCount2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0068");
            MaxDefectSize2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0069");
            MaxSmallDefectSize2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0070");
            MaxSmallDefectCount2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0071");
        }
    }
    #endregion

    #region Shape Shift With Center Line Type.
    /// <summary>   Plate with center line default value.  </summary>
    /// <remarks>   suoow2, 2012-09-12. </remarks>
    public static class PlateWithCLDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ThreshType;              // 0000
        public static int LowerThresh;             // 0001
        public static int UpperThresh;             // 0002
        public static int ApplyAverDiff;           // 0003
        public static int InspRange;               // 0006
        public static int AverMinMargin;           // 0007
        public static int AverMaxMargin;           // 0008
        public static int MinMargin;               // 0009
        public static int MaxMargin;               // 0010
        public static int ErosionTrainIter;        // 0011
        public static int DilationTrainIter;       // 0012
        public static int MinDefectSize;           // 0015
        public static int ErosionShapeIter;        // 0021  
        public static int DilationShapeIter;       // 0022
        public static int GroundLowerThresh;       // 0023
        public static int GroundUpperThresh;       // 0024
        public static int ShapeLowerThresh;        // 0025
        public static int ShapeUpperThresh;        // 0026
        public static int CorrectMethod;           // 0027
        public static int ShapeShiftMarginX;       // 0028
        public static int ShapeShiftMarginY;       // 0029

        // 형상 영역 중앙선 검사 2012-09-12
        public static int IsInspectCL;             // 0100
        public static int MinWidthRatio;           // 0030
        public static int MinWidthSize;            // 0031
        public static int MaxWidthRatio;           // 0032
        public static int MaxWidthSize;            // 0033
        public static int MinHeightSize;           // 0035
        public static int MinNormalRatio;          // 0038
        public static int MaxNormalRatio;          // 0039
        public static int LeadThresh;              // 0041

        public static int ThreshType2;             // 0049
        public static int InspLowerThresh2;        // 0050
        public static int InspUpperThresh2;        // 0051
        public static int ApplyAverDiff2;          // 0052
        public static int AverMinMargin2;          // 0053
        public static int AverMaxMargin2;          // 0054
        public static int InspRange2;              // 0055
        public static int InspMinMargin2;          // 0056
        public static int InspMaxMargin2;          // 0057
        public static int MinDefectSize2;          // 0058
        public static int IsInspectMaster;         // 0059
        public static int IsInspectSlave;          // 0060
        public static int ShapeShiftType;          // 0061
        public static int ShapeOffsetX;            // 0062
        public static int ShapeOffsetY;            // 0063
        public static int MaxDefectSize;           // 0064
        public static int MaxSmallDefectSize;      // 0065
        public static int MaxSmallDefectCount;     // 0066
        public static int MinSmallDefectSize2;     // 0067
        public static int MinSmallDefectCount2;    // 0068
        public static int MaxDefectSize2;          // 0069
        public static int MaxSmallDefectSize2;     // 0070
        public static int MaxSmallDefectCount2;    // 0071

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0437";
            string shapeShiftCode = "3018";
            ThreshType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0000");
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0001");
            UpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0002");
            ApplyAverDiff = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0003");
            InspRange = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0006");
            AverMinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0007");
            AverMaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0008");
            MinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0009");
            MaxMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0010");
            ErosionTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0011");
            DilationTrainIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0012");
            MinDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0015");
            ErosionShapeIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0021");
            DilationShapeIter = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0022");
            GroundLowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0023");
            GroundUpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0024");
            ShapeLowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0025");
            ShapeUpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0026");
            CorrectMethod = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0027");
            ShapeShiftMarginX = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0028");
            ShapeShiftMarginY = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0029");

            IsInspectCL = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0100");
            MinWidthRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0030");
            MinWidthSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0031");
            MaxWidthRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0032");
            MaxWidthSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0033");
            MinHeightSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0035");
            MinNormalRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0038");
            MaxNormalRatio = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0039");
            LeadThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0041");

            ThreshType2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0049");
            InspLowerThresh2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0050");
            InspUpperThresh2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0051");
            ApplyAverDiff2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0052");
            AverMinMargin2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0053");
            AverMaxMargin2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0054");
            InspRange2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0055");
            InspMinMargin2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0056");
            InspMaxMargin2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0057");
            MinDefectSize2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0058");
            IsInspectMaster = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0059");
            IsInspectSlave = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0060");
            ShapeShiftType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0061");
            ShapeOffsetX = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0062");
            ShapeOffsetY = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0063");
            MaxDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0064");
            MaxSmallDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0065");
            MaxSmallDefectCount = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0066");
            MinSmallDefectSize2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0067");
            MinSmallDefectCount2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0068");
            MaxDefectSize2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0069");
            MaxSmallDefectSize2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0070");
            MaxSmallDefectCount2 = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, shapeShiftCode, "0071");
        }
    }
    #endregion


    public static class GV_Inspection_DefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int Ball_Thresh;            // 0000
        public static int Core_Thresh;            // 0001
        public static int Mask;                   // 0002
        public static int Taget_GV;               // 0003
        public static int GV_Margin;              // 0004



        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0490";
            string damMaskcode = "3027";

            Ball_Thresh = 100;
            Core_Thresh = 0;
            Mask = 100;
            Taget_GV = 80;
            GV_Margin = 10;
        }
    }


    /// <summary>   Window Punch Dam default value.  </summary>
    /// <remarks>   suoow2, 2017-05-04. </remarks>
    public static class DamSizeDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ThreshType;              // 0000
        public static int LowerThresh;             // 0001
        public static int UpperThresh;             // 0002
        public static int MinMargin;               // 0009
        public static int MinDefectSize;           // 0015
        public static int MaxDefectSize;           // 0064
        public static int MinWidth;                // 0031

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0433";
            string damMaskcode = "3021";
            ThreshType = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, damMaskcode, "0000");
            LowerThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, damMaskcode, "0001");
            UpperThresh = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, damMaskcode, "0002");
            MinMargin = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, damMaskcode, "0009");
            MinDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, damMaskcode, "0015");
            MaxDefectSize = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, damMaskcode, "0064");
            MinWidth = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, damMaskcode, "0031");
        }
    }

    #region Exceptional Mask.
    /// <summary>   Exceptional Mask default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class ExceptionalMaskDefaultValue
    {
        public static bool DefaultValueLoaded = false;

        public static int ExceptionX;             // 0000
        public static int ExceptionY;             // 0001
        public static int UseShapeShift;          // 0002

        public static void LoadDefaultValue()
        {
            string inspectTypeCode = "0999";
            string exceptionalMaskCode = "3016";
            ExceptionX = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, exceptionalMaskCode, "0000");
            ExceptionY = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, exceptionalMaskCode, "0001");
            UseShapeShift = InspectionQueryHelper.GetDefaultParamValue(inspectTypeCode, exceptionalMaskCode, "0002");
        }
    }
    #endregion
}
