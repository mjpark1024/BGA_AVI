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
using Common.Drawing.InspectionInformation;
using Common.Drawing.InspectionTypeUI;
using PCS.ModelTeaching;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using static System.Resources.ResXFileRef;

namespace PCS
{
    /// <summary>   Parameter.  </summary>
    public static class VisionParameter
    {
        private static byte[] m_arrInspItem;

        public static int VisionInfoSize = sizeof(int) * 18 + 256 * 2;
        public static int SectionSize = 4 + 4 + (4 * 4) + (4 * 2) + (4 * 2) + (4 * 2) + (4 * 2) + (4 * 2) + (4 * 2) + 4 + (8192 * 8) + (8192 * 8) + 4;

        public static int RoiSize = 4 + 4 + 4 + 4 * 4 + 4 + 4 + 4 + 4 + 4 * 2 * 512 + 4 * 2 * 512 + 4 + 4 * 2 * 4 + 4;
        public static int InspSize = 101 * 4;
        public static int ResultSize = 4 * 18;
        public static int StripAlignSize = sizeof(int) * 8;
        public static int ICS_UnitPos = sizeof(int) * (4 + 1);

        static VisionParameter()
        {
            m_arrInspItem = new byte[InspSize];
        }

        #region Make Section Packet.
        public static byte[] SectionToBytes(SectionInformation aSection)
        {
            byte[] arrResult = new Byte[SectionSize];

            // 전역 Fidu - 0 / Unit 영역 - 1 / 외곽 영역 - 2
            int nSectionType = 0;
            if (aSection.Type.Code == SectionTypeCode.STRIP_REGION) // Strip Align
            {
                nSectionType = (int)eSectionType.eSecTypeRegionGlobal;
                //nSectionType = (int)eSectionType.eSecTypeRegionUnit;
            }
            else if (aSection.Type.Code == SectionTypeCode.UNIT_REGION) // Unit Region
            {
                nSectionType = (int)eSectionType.eSecTypeRegionUnit;
            }
            else if (aSection.Type.Code == SectionTypeCode.PSR_REGION) // Psr Region
            {
                nSectionType = (int)eSectionType.eSecTypeRegionPsr;
            }
            else if (aSection.Type.Code == SectionTypeCode.RAW_REGION) // Raw Region
            {
                nSectionType = (int)eSectionType.eSecTypeRegionRaw;
            }
            else if (aSection.Type.Code == SectionTypeCode.ID_REGION) // ID Region
            {
                nSectionType = (int)eSectionType.eSecTypeRegionID;
            }
            else if (aSection.Type.Code == SectionTypeCode.OUTER_REGION)
            {
                nSectionType = (int)eSectionType.eSecTypeRegionOuter;
            }

            // C# - float 4byte / double 8byte
            float fIterationPitchX = (float)(aSection.IterationPitchX / VisionDefinition.GRAB_IMAGE_SCALE);
            float fIterationPitchY = (float)(aSection.IterationPitchY / VisionDefinition.GRAB_IMAGE_SCALE);
            float fBlockPitchX = (float)(aSection.BlockPitchX / VisionDefinition.GRAB_IMAGE_SCALE);
            float fBlockPitchY = (float)(aSection.BlockPitchY / VisionDefinition.GRAB_IMAGE_SCALE);

            int unitColumnsPerBlock = (int)Math.Round((double)aSection.IterationCountX / (double)aSection.BlockCountX);
            int unitRowsPerBlock = (int)Math.Round((double)aSection.IterationCountY / (double)aSection.BlockCountY);
            Int32Rect scaledSectionRegion = new Int32Rect((int)(aSection.Region.X / VisionDefinition.GRAB_IMAGE_SCALE),
                                                          (int)(aSection.Region.Y / VisionDefinition.GRAB_IMAGE_SCALE),
                                                          (int)(aSection.Region.Width / VisionDefinition.GRAB_IMAGE_SCALE),
                                                          (int)(aSection.Region.Height / VisionDefinition.GRAB_IMAGE_SCALE));

            BitConverter.GetBytes(aSection.IntCode).CopyTo(arrResult, 0); // Section ID
            BitConverter.GetBytes(nSectionType).CopyTo(arrResult, 4); // Section 종류 (eVisSecType)

            BitConverter.GetBytes(scaledSectionRegion.X).CopyTo(arrResult, 8); // not used in new version.
            BitConverter.GetBytes(scaledSectionRegion.Y).CopyTo(arrResult, 12); // not used in new version.
            BitConverter.GetBytes(scaledSectionRegion.Width).CopyTo(arrResult, 16); // not used in new version.
            BitConverter.GetBytes(scaledSectionRegion.Height).CopyTo(arrResult, 20); // not used in new version.
            BitConverter.GetBytes(aSection.IterationCountX).CopyTo(arrResult, 24); // not used in new version.
            BitConverter.GetBytes(aSection.IterationCountY).CopyTo(arrResult, 28); // not used in new version.
            BitConverter.GetBytes(fIterationPitchX).CopyTo(arrResult, 32); // not used in new version.
            BitConverter.GetBytes(fIterationPitchY).CopyTo(arrResult, 36); // not used in new version.
            BitConverter.GetBytes(unitColumnsPerBlock).CopyTo(arrResult, 40); // not used in new version.
            BitConverter.GetBytes(unitRowsPerBlock).CopyTo(arrResult, 44); // not used in new version.
            BitConverter.GetBytes(fBlockPitchX).CopyTo(arrResult, 48); // not used in new version.
            BitConverter.GetBytes(fBlockPitchY).CopyTo(arrResult, 52); // not used in new version.

            BitConverter.GetBytes(scaledSectionRegion.Width).CopyTo(arrResult, 56); // Section Region의 기준 크기
            BitConverter.GetBytes(scaledSectionRegion.Height).CopyTo(arrResult, 60); // Section Region의 기준 크기
            BitConverter.GetBytes(aSection.IterationXPosition).CopyTo(arrResult, 64); // 기준 Unit 위치
            BitConverter.GetBytes(aSection.IterationYPosition).CopyTo(arrResult, 68); // 기준 Unit 위치

            int all_Count = 0; // 색션 roi 중 검사제외인 경우는 전체 카운트에서 제거해야함
            for (int nIndex = 0; nIndex < aSection.SectionRegionList.Count; nIndex++) if (aSection.SectionRegionList[nIndex].IsInspection) all_Count++;
            BitConverter.GetBytes(all_Count).CopyTo(arrResult, 72); // 전체 영역 개수

            Debug.Assert(aSection.SectionRegionList.Count < 8192);

            int nByteIndex = 76;
            int nSectionRegionCount = aSection.SectionRegionList.Count;
            for (int nIndex = 0; nIndex < nSectionRegionCount; nIndex++)
            {
                if (aSection.SectionRegionList[nIndex].IsInspection)
                {
                    BitConverter.GetBytes((int)(aSection.SectionRegionList[nIndex].RegionPosition.X / VisionDefinition.GRAB_IMAGE_SCALE)).CopyTo(arrResult, nByteIndex);
                    BitConverter.GetBytes((int)(aSection.SectionRegionList[nIndex].RegionPosition.Y / VisionDefinition.GRAB_IMAGE_SCALE)).CopyTo(arrResult, nByteIndex + 4);

                    nByteIndex += 8; // next byte index.
                }
            }

            nByteIndex = 76 + 8192 * 8;

            // 76 + (4096 * 8) 부터 Region 시작 위치 정보
            for (int nIndex = 0; nIndex < nSectionRegionCount; nIndex++)
            {
                if (aSection.SectionRegionList[nIndex].IsInspection)
                {
                    BitConverter.GetBytes(aSection.SectionRegionList[nIndex].RegionIndex.X).CopyTo(arrResult, nByteIndex);
                    BitConverter.GetBytes(aSection.SectionRegionList[nIndex].RegionIndex.Y).CopyTo(arrResult, nByteIndex + 4);

                    nByteIndex += 8; // next byte index.
                }
            }
            nByteIndex = 76 + 8192 * 8 + 8192 * 8;
            BitConverter.GetBytes(0).CopyTo(arrResult, nByteIndex);

            return arrResult;
        }
        #endregion

        public static byte[] StripAlignToBytes(StripAlignInfo roiinfo)
        {
            byte[] arrResult = new Byte[StripAlignSize]; // 

            BitConverter.GetBytes(roiinfo.RoiID).CopyTo(arrResult, 0);
            BitConverter.GetBytes(roiinfo.BndRect.X).CopyTo(arrResult, 4);
            BitConverter.GetBytes(roiinfo.BndRect.Y).CopyTo(arrResult, 8);
            BitConverter.GetBytes(roiinfo.BndRect.Width).CopyTo(arrResult, 12);
            BitConverter.GetBytes(roiinfo.BndRect.Height).CopyTo(arrResult, 16);

            BitConverter.GetBytes(roiinfo.SearchMarginX).CopyTo(arrResult, 20);
            BitConverter.GetBytes(roiinfo.SearchMarginY).CopyTo(arrResult, 24);
            BitConverter.GetBytes(roiinfo.Match).CopyTo(arrResult, 28);
            return arrResult;
        }


        #region Make ROI Packet.
        public static byte[] RoiToBytes(RoiInfo roiinfo, int anChannel)
        {
            byte[] arrResult = new Byte[RoiSize]; // 
            int nOuterPtCnt = (roiinfo.OuterPoints == null) ? 0 : (int)roiinfo.OuterPoints.Count();
            int nInnerPtCnt = (roiinfo.InnerPoints == null) ? 0 : (int)roiinfo.InnerPoints.Count();
            int nLocalAlignCnt = roiinfo.LocalAlignPoints.Count;

            BitConverter.GetBytes(roiinfo.RoiID).CopyTo(arrResult, 0);
            BitConverter.GetBytes(roiinfo.SectionID).CopyTo(arrResult, 4);
            BitConverter.GetBytes(roiinfo.UnitRow).CopyTo(arrResult, 8);



            BitConverter.GetBytes(anChannel).CopyTo(arrResult, 12);//color channel

            // BndRect - 16 Bytes. IS에서 연산하므로 건너 뛰어준다.

            BitConverter.GetBytes(roiinfo.OuterDrawType).CopyTo(arrResult, 32);
            BitConverter.GetBytes(roiinfo.InnerDrawType).CopyTo(arrResult, 36);

            BitConverter.GetBytes(nOuterPtCnt).CopyTo(arrResult, 40);
            BitConverter.GetBytes(nInnerPtCnt).CopyTo(arrResult, 44);

            for (int i = 0; i < nOuterPtCnt; i++)
            {
                BitConverter.GetBytes(roiinfo.OuterPoints[i].X).CopyTo(arrResult, 48 + 8 * i);
                BitConverter.GetBytes(roiinfo.OuterPoints[i].Y).CopyTo(arrResult, 52 + 8 * i);
            }

            for (int i = 0; i < nInnerPtCnt; i++)
            {
                BitConverter.GetBytes(roiinfo.InnerPoints[i].X).CopyTo(arrResult, 48 + 8 * 512 + 8 * i);
                BitConverter.GetBytes(roiinfo.InnerPoints[i].Y).CopyTo(arrResult, 52 + 8 * 512 + 8 * i);
            }

            BitConverter.GetBytes(nLocalAlignCnt).CopyTo(arrResult, 48 + 8 * 1024);

            for (int i = 0; i < nLocalAlignCnt; i++)
            {
                BitConverter.GetBytes(roiinfo.LocalAlignPoints[i].X).CopyTo(arrResult, 52 + 8 * 1024 + 8 * i);
                BitConverter.GetBytes(roiinfo.LocalAlignPoints[i].Y).CopyTo(arrResult, 56 + 8 * 1024 + 8 * i);
            }

            return arrResult;
        }
        #endregion

        #region Make Insp Item Packet.
        // anSurfaceID :
        // 0 - 상부 반사 / 1 - 하부 반사 / 2 - 상부 투과
        public static byte[] InspItemToBytes(int anSurfaceID, int anInspID, int anSecID, int anRoiID, InspectionItem aInspItem, double afRes, Point PSRMargin, Point WPMargin, int anChannel)
        {
            for (int i = 0; i < InspSize; i++)
            {
                m_arrInspItem[i] = 0;
            }
      
            eVisInspectType inspectType = aInspItem.InspectionType.InspectType;
            InspectionResultTypeHelper.SetResultType(anSurfaceID, aInspItem);

            BitConverter.GetBytes(anInspID).CopyTo(m_arrInspItem, 0 * sizeof(int));
            BitConverter.GetBytes(anSecID).CopyTo(m_arrInspItem, 1 * sizeof(int));
            BitConverter.GetBytes(anRoiID).CopyTo(m_arrInspItem, 2 * sizeof(int));
            BitConverter.GetBytes(anChannel).CopyTo(m_arrInspItem, 3 * sizeof(int));
            BitConverter.GetBytes((int)aInspItem.InspectionType.InspectType).CopyTo(m_arrInspItem, 4 * sizeof(int));
            BitConverter.GetBytes((int)aInspItem.InspectionType.InspectAlgorithm).CopyTo(m_arrInspItem, 5 * sizeof(int));
            BitConverter.GetBytes((int)aInspItem.InspectionType.Priority).CopyTo(m_arrInspItem, 6 * sizeof(int));
            BitConverter.GetBytes(aInspItem.InspectionType.ResultType).CopyTo(m_arrInspItem, 7 * sizeof(int));
            BitConverter.GetBytes(aInspItem.InspectionType.ResultTypeMin).CopyTo(m_arrInspItem, 8 * sizeof(int));
            BitConverter.GetBytes(aInspItem.InspectionType.ResultTypeMax).CopyTo(m_arrInspItem, 9 * sizeof(int));
            BitConverter.GetBytes(aInspItem.InspectionType.BreakType).CopyTo(m_arrInspItem, 10 * sizeof(int));

            switch (inspectType)
            {
                case eVisInspectType.eInspTypeGlobalAlign:
                case eVisInspectType.eInspTypeUnitAlign:
                    FiducialAlignProperty fiducialAlignProperty = aInspItem.InspectionAlgorithm as FiducialAlignProperty;
                    if (fiducialAlignProperty != null)
                    {
                        BitConverter.GetBytes(fiducialAlignProperty.AlignMarginX).CopyTo(m_arrInspItem, 57 * sizeof(int));
                        BitConverter.GetBytes(fiducialAlignProperty.AlignMarginY).CopyTo(m_arrInspItem, 58 * sizeof(int));
                        BitConverter.GetBytes(fiducialAlignProperty.AlignAcceptance).CopyTo(m_arrInspItem, 59 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeRailFiducial:
                    OuterFiducialProperty outerfiducialProperty = aInspItem.InspectionAlgorithm as OuterFiducialProperty;
                    if (outerfiducialProperty != null)
                    {
                        BitConverter.GetBytes(outerfiducialProperty.MarginX).CopyTo(m_arrInspItem, 57 * sizeof(int));
                        BitConverter.GetBytes(outerfiducialProperty.MarginY).CopyTo(m_arrInspItem, 58 * sizeof(int));
                        BitConverter.GetBytes(outerfiducialProperty.Acceptance).CopyTo(m_arrInspItem, 59 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeLeadShapeWithCL: // 리드 형상 검사
                    LeadShapeWithCenterLineProperty leadShapeWithCenterLineProperty = aInspItem.InspectionAlgorithm as LeadShapeWithCenterLineProperty;
                    if (leadShapeWithCenterLineProperty != null)
                    {
                        BitConverter.GetBytes(leadShapeWithCenterLineProperty.ThreshType).CopyTo(m_arrInspItem, 12 * sizeof(int));
                        BitConverter.GetBytes(leadShapeWithCenterLineProperty.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(leadShapeWithCenterLineProperty.UpperThresh).CopyTo(m_arrInspItem, 14 * sizeof(int));
                        BitConverter.GetBytes(leadShapeWithCenterLineProperty.MaskLowerThresh).CopyTo(m_arrInspItem, 16 * sizeof(int));
                        BitConverter.GetBytes(leadShapeWithCenterLineProperty.MinDefectSize).CopyTo(m_arrInspItem, 27 * sizeof(int));

                        // 2012-03-30 suoow2. : 리드 형상 검사일 때 반사와 투과 조명에 대한 구분을 둔다.
                        //if (anSurfaceID < 2) // 상부, 하부 반사 : 밝은 불량 검출. (0 or 1)
                        //{
                        BitConverter.GetBytes((int)2).CopyTo(m_arrInspItem, 17 * sizeof(int));
                        //}
                        //else if (anSurfaceID == 2) // 상부 투과 : 어두운 불량 검출. (2)
                        //{
                        //   BitConverter.GetBytes((int)1).CopyTo(m_arrInspItem, 17 * sizeof(int));
                        //}
                        BitConverter.GetBytes(leadShapeWithCenterLineProperty.MinWidthRatio).CopyTo(m_arrInspItem, 42 * sizeof(int));
                        BitConverter.GetBytes(leadShapeWithCenterLineProperty.MinWidthSize).CopyTo(m_arrInspItem, 43 * sizeof(int));
                        BitConverter.GetBytes(leadShapeWithCenterLineProperty.MaxWidthRatio).CopyTo(m_arrInspItem, 44 * sizeof(int));
                        BitConverter.GetBytes(leadShapeWithCenterLineProperty.MaxWidthSize).CopyTo(m_arrInspItem, 45 * sizeof(int));
                        BitConverter.GetBytes(leadShapeWithCenterLineProperty.MinHeightsize).CopyTo(m_arrInspItem, 47 * sizeof(int));
                        BitConverter.GetBytes(leadShapeWithCenterLineProperty.MinNormalRatio).CopyTo(m_arrInspItem, 50 * sizeof(int));
                        BitConverter.GetBytes(leadShapeWithCenterLineProperty.MaxNormalRatio).CopyTo(m_arrInspItem, 51 * sizeof(int));
                        BitConverter.GetBytes(leadShapeWithCenterLineProperty.RemoveTipSize).CopyTo(m_arrInspItem, 100 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeSpaceShapeWithCL: // 공간 형상 검사
                    SpaceShapeWithCenterLineProperty spaceShapeWithCenterLineProperty = aInspItem.InspectionAlgorithm as SpaceShapeWithCenterLineProperty;
                    if (spaceShapeWithCenterLineProperty != null)
                    {
                        BitConverter.GetBytes(spaceShapeWithCenterLineProperty.ThreshType).CopyTo(m_arrInspItem, 12 * sizeof(int));
                        BitConverter.GetBytes(spaceShapeWithCenterLineProperty.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(spaceShapeWithCenterLineProperty.UpperThresh).CopyTo(m_arrInspItem, 14 * sizeof(int));
                        BitConverter.GetBytes((int)0).CopyTo(m_arrInspItem, 16 * sizeof(int));
                        BitConverter.GetBytes((int)0).CopyTo(m_arrInspItem, 27 * sizeof(int));
                        // 2012-03-30 suoow2. : 공간 형상 검사일 때 반사와 투과 조명에 대한 구분을 둔다.
                        //if (anSurfaceID < 2) // 상부, 하부 반사 : 어두운 불량 검출. (0 or 1)
                        //{
                        BitConverter.GetBytes((int)1).CopyTo(m_arrInspItem, 18 * sizeof(int));
                        //}
                        //else if (anSurfaceID == 2) // 상부 투과 : 밝은 불량 검출. (2)
                        //{
                        //   BitConverter.GetBytes((int)2).CopyTo(m_arrInspItem, 17 * sizeof(int));
                        //}
                        BitConverter.GetBytes(spaceShapeWithCenterLineProperty.MinWidthRatio).CopyTo(m_arrInspItem, 42 * sizeof(int));
                        BitConverter.GetBytes(spaceShapeWithCenterLineProperty.MinWidthSize).CopyTo(m_arrInspItem, 43 * sizeof(int));
                        BitConverter.GetBytes(spaceShapeWithCenterLineProperty.MaxWidthRatio).CopyTo(m_arrInspItem, 44 * sizeof(int));
                        BitConverter.GetBytes(spaceShapeWithCenterLineProperty.MaxWidthSize).CopyTo(m_arrInspItem, 45 * sizeof(int));
                        BitConverter.GetBytes(spaceShapeWithCenterLineProperty.MinHeightsize).CopyTo(m_arrInspItem, 47 * sizeof(int));
                        BitConverter.GetBytes(spaceShapeWithCenterLineProperty.MinNormalRatio).CopyTo(m_arrInspItem, 50 * sizeof(int));
                        BitConverter.GetBytes(spaceShapeWithCenterLineProperty.MaxNormalRatio).CopyTo(m_arrInspItem, 51 * sizeof(int));
                        BitConverter.GetBytes(spaceShapeWithCenterLineProperty.TipSearchSize).CopyTo(m_arrInspItem, 100 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeGroove: // 중앙선을 이용한 그루브 검사
                    GrooveProperty grooveProperty = aInspItem.InspectionAlgorithm as GrooveProperty;
                    if (grooveProperty != null)
                    {
                        BitConverter.GetBytes(grooveProperty.ThreshType).CopyTo(m_arrInspItem, 12 * sizeof(int));
                        BitConverter.GetBytes(grooveProperty.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(grooveProperty.UpperThresh).CopyTo(m_arrInspItem, 14 * sizeof(int));
                        BitConverter.GetBytes(grooveProperty.ApplyAverDiff).CopyTo(m_arrInspItem, 15 * sizeof(int));
                        BitConverter.GetBytes(grooveProperty.AverMinMargin).CopyTo(m_arrInspItem, 19 * sizeof(int));
                        BitConverter.GetBytes(grooveProperty.AverMaxMargin).CopyTo(m_arrInspItem, 20 * sizeof(int));
                        BitConverter.GetBytes(grooveProperty.MinWidthRatio).CopyTo(m_arrInspItem, 42 * sizeof(int));
                        BitConverter.GetBytes(grooveProperty.MinWidthSize).CopyTo(m_arrInspItem, 43 * sizeof(int));
                        BitConverter.GetBytes(grooveProperty.MaxWidthRatio).CopyTo(m_arrInspItem, 44 * sizeof(int));
                        BitConverter.GetBytes(grooveProperty.MaxWidthSize).CopyTo(m_arrInspItem, 45 * sizeof(int));
                        BitConverter.GetBytes(grooveProperty.MinHeightSize).CopyTo(m_arrInspItem, 47 * sizeof(int));
                        BitConverter.GetBytes(grooveProperty.MaxHeightRatio).CopyTo(m_arrInspItem, 48 * sizeof(int));
                        BitConverter.GetBytes(grooveProperty.MaxHeightSize).CopyTo(m_arrInspItem, 49 * sizeof(int));
                        BitConverter.GetBytes(grooveProperty.MinNormalRatio).CopyTo(m_arrInspItem, 50 * sizeof(int));
                        BitConverter.GetBytes(grooveProperty.MaxNormalRatio).CopyTo(m_arrInspItem, 51 * sizeof(int));
                        BitConverter.GetBytes(grooveProperty.CriterionSize).CopyTo(m_arrInspItem, 76 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeShapeHalfEtching: // 형상 Half Etching 검사
                    FigureShapeProperty figureShapeProperty = aInspItem.InspectionAlgorithm as FigureShapeProperty;
                    if (figureShapeProperty != null)
                    {
                        BitConverter.GetBytes(figureShapeProperty.InspRange).CopyTo(m_arrInspItem, 18 * sizeof(int));
                        BitConverter.GetBytes(figureShapeProperty.AverMinMargin).CopyTo(m_arrInspItem, 19 * sizeof(int));
                        BitConverter.GetBytes(figureShapeProperty.AverMaxMargin).CopyTo(m_arrInspItem, 20 * sizeof(int));
                        BitConverter.GetBytes(figureShapeProperty.MinMargin).CopyTo(m_arrInspItem, 21 * sizeof(int));
                        BitConverter.GetBytes(figureShapeProperty.MaxMargin).CopyTo(m_arrInspItem, 22 * sizeof(int));
                        BitConverter.GetBytes(figureShapeProperty.ErosionTrainIter).CopyTo(m_arrInspItem, 23 * sizeof(int));
                        BitConverter.GetBytes(figureShapeProperty.DilationTrainIter).CopyTo(m_arrInspItem, 24 * sizeof(int));
                        BitConverter.GetBytes(figureShapeProperty.ErosionInspIter).CopyTo(m_arrInspItem, 25 * sizeof(int));
                        BitConverter.GetBytes(figureShapeProperty.DilationInspIter).CopyTo(m_arrInspItem, 26 * sizeof(int));

                        BitConverter.GetBytes(figureShapeProperty.MinWidthSize).CopyTo(m_arrInspItem, 43 * sizeof(int));
                        BitConverter.GetBytes(figureShapeProperty.MinHeightSize).CopyTo(m_arrInspItem, 47 * sizeof(int));
                        BitConverter.GetBytes(figureShapeProperty.MinNormalRatio).CopyTo(m_arrInspItem, 50 * sizeof(int));
                        BitConverter.GetBytes(figureShapeProperty.MaxNormalRatio).CopyTo(m_arrInspItem, 51 * sizeof(int));
                        BitConverter.GetBytes(figureShapeProperty.AlignMaxDist).CopyTo(m_arrInspItem, 55 * sizeof(int));
                        BitConverter.GetBytes(figureShapeProperty.DarkAreaWidth).CopyTo(m_arrInspItem, 56 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeRailIntensity: // 다운셋 검사
                    StateIntensityProperty stateIntensityProperty = aInspItem.InspectionAlgorithm as StateIntensityProperty;
                    if (stateIntensityProperty != null)
                    {
                        BitConverter.GetBytes(stateIntensityProperty.ThreshType).CopyTo(m_arrInspItem, 12 * sizeof(int));
                        BitConverter.GetBytes(stateIntensityProperty.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(stateIntensityProperty.UpperThresh).CopyTo(m_arrInspItem, 14 * sizeof(int));
                        BitConverter.GetBytes(stateIntensityProperty.ApplyAverDiff).CopyTo(m_arrInspItem, 15 * sizeof(int));
                        BitConverter.GetBytes(stateIntensityProperty.InspRange).CopyTo(m_arrInspItem, 18 * sizeof(int));
                        BitConverter.GetBytes(stateIntensityProperty.AverMinMargin).CopyTo(m_arrInspItem, 19 * sizeof(int));
                        BitConverter.GetBytes(stateIntensityProperty.AverMaxMargin).CopyTo(m_arrInspItem, 20 * sizeof(int));
                        BitConverter.GetBytes(stateIntensityProperty.MinMargin).CopyTo(m_arrInspItem, 21 * sizeof(int));
                        BitConverter.GetBytes(stateIntensityProperty.MaxMargin).CopyTo(m_arrInspItem, 22 * sizeof(int));
                        BitConverter.GetBytes(stateIntensityProperty.ErosionTrainIter).CopyTo(m_arrInspItem, 23 * sizeof(int));
                        BitConverter.GetBytes(stateIntensityProperty.DilationTrainIter).CopyTo(m_arrInspItem, 24 * sizeof(int));
                        BitConverter.GetBytes(stateIntensityProperty.ErosionInspIter).CopyTo(m_arrInspItem, 25 * sizeof(int));
                        BitConverter.GetBytes(stateIntensityProperty.DilationInspIter).CopyTo(m_arrInspItem, 26 * sizeof(int));
                        BitConverter.GetBytes(stateIntensityProperty.MinDefectSize).CopyTo(m_arrInspItem, 27 * sizeof(int));
                        BitConverter.GetBytes(stateIntensityProperty.Invert).CopyTo(m_arrInspItem, 32 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeTape:     // 테이프 검사
                case eVisInspectType.eInspTypePlate:    // 도금 검사
                    ShapeShiftProperty shapeShiftProperty = aInspItem.InspectionAlgorithm as ShapeShiftProperty;
                    if (shapeShiftProperty != null)
                    {
                        BitConverter.GetBytes(shapeShiftProperty.ThreshType).CopyTo(m_arrInspItem, 12 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.UpperThresh).CopyTo(m_arrInspItem, 14 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.ApplyAverDiff).CopyTo(m_arrInspItem, 15 * sizeof(int));
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 16 * sizeof(int)); // 삭제됨 : 마스크 임계 하한, MaskLowerThresh
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 17 * sizeof(int)); // 삭제됨 : 마스크 임계 상한, MaskUpperThresh
                        BitConverter.GetBytes(shapeShiftProperty.InspRange).CopyTo(m_arrInspItem, 18 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.AverMinMargin).CopyTo(m_arrInspItem, 19 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.AverMaxMargin).CopyTo(m_arrInspItem, 20 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.MinMargin).CopyTo(m_arrInspItem, 21 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.MaxMargin).CopyTo(m_arrInspItem, 22 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.ErosionTrainIter).CopyTo(m_arrInspItem, 23 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.DilationTrainIter).CopyTo(m_arrInspItem, 24 * sizeof(int));
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 25 * sizeof(int)); // 삭제됨 : 결과 축소, ErosionInspIter
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 26 * sizeof(int)); // 삭제됨 : 결과 확장, DilationInspIter
                        BitConverter.GetBytes(shapeShiftProperty.MinDefectSize).CopyTo(m_arrInspItem, 27 * sizeof(int));
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 28 * sizeof(int)); // 삭제됨 : 미세불량 사이즈, MinSmallDefectSize
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 29 * sizeof(int)); // 삭제됨 : 미세불량 개수, MinSmallDefectCount
                        BitConverter.GetBytes(shapeShiftProperty.ErosionShapeIter).CopyTo(m_arrInspItem, 33 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.DilationShapeIter).CopyTo(m_arrInspItem, 34 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.GroundLowerThresh).CopyTo(m_arrInspItem, 35 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.GroundUpperThresh).CopyTo(m_arrInspItem, 36 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.ShapeLowerThresh).CopyTo(m_arrInspItem, 37 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.ShapeUpperThresh).CopyTo(m_arrInspItem, 38 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.CorrectMethod).CopyTo(m_arrInspItem, 39 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.ShapeShiftMarginX).CopyTo(m_arrInspItem, 40 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.ShapeShiftMarginY).CopyTo(m_arrInspItem, 41 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.ThreshType2).CopyTo(m_arrInspItem, 77 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.InspLowerThresh2).CopyTo(m_arrInspItem, 78 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.InspUpperThresh2).CopyTo(m_arrInspItem, 79 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.ApplyAverDiff2).CopyTo(m_arrInspItem, 80 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.AverMinMargin2).CopyTo(m_arrInspItem, 81 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.AverMaxMargin2).CopyTo(m_arrInspItem, 82 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.InspRange2).CopyTo(m_arrInspItem, 83 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.InspMinMargin2).CopyTo(m_arrInspItem, 84 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.InspMaxMargin2).CopyTo(m_arrInspItem, 85 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.MinDefectSize2).CopyTo(m_arrInspItem, 86 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.IsInspectMaster).CopyTo(m_arrInspItem, 87 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.IsInspectSlave).CopyTo(m_arrInspItem, 88 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.ShapeShiftType).CopyTo(m_arrInspItem, 89 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.ShapeOffsetX).CopyTo(m_arrInspItem, 90 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.ShapeOffsetY).CopyTo(m_arrInspItem, 91 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.MaxDefectSize).CopyTo(m_arrInspItem, 92 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftProperty.MaxDefectSize2).CopyTo(m_arrInspItem, 97 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypePlateWithCL:        // 도금 선폭 검사
                    ShapeShiftWithCLProperty shapeShiftWithCLProperty = aInspItem.InspectionAlgorithm as ShapeShiftWithCLProperty;
                    if (shapeShiftWithCLProperty != null)
                    {
                        BitConverter.GetBytes(shapeShiftWithCLProperty.ThreshType).CopyTo(m_arrInspItem, 12 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.UpperThresh).CopyTo(m_arrInspItem, 14 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.ApplyAverDiff).CopyTo(m_arrInspItem, 15 * sizeof(int));
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 16 * sizeof(int)); // 삭제됨 : 마스크 임계 하한, MaskLowerThresh
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 17 * sizeof(int)); // 삭제됨 : 마스크 임계 상한, MaskUpperThresh
                        BitConverter.GetBytes(shapeShiftWithCLProperty.InspRange).CopyTo(m_arrInspItem, 18 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.AverMinMargin).CopyTo(m_arrInspItem, 19 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.AverMaxMargin).CopyTo(m_arrInspItem, 20 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.MinMargin).CopyTo(m_arrInspItem, 21 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.MaxMargin).CopyTo(m_arrInspItem, 22 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.ErosionTrainIter).CopyTo(m_arrInspItem, 23 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.DilationTrainIter).CopyTo(m_arrInspItem, 24 * sizeof(int));
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 25 * sizeof(int)); // 삭제됨 : 결과 축소, ErosionInspIter
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 26 * sizeof(int)); // 삭제됨 : 결과 확장, DilationInspIter
                        BitConverter.GetBytes(shapeShiftWithCLProperty.MinDefectSize).CopyTo(m_arrInspItem, 27 * sizeof(int));
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 28 * sizeof(int)); // 삭제됨 : 미세불량 사이즈, MinSmallDefectSize
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 29 * sizeof(int)); // 삭제됨 : 미세불량 개수, MinSmallDefectCount
                        BitConverter.GetBytes(shapeShiftWithCLProperty.ErosionShapeIter).CopyTo(m_arrInspItem, 33 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.DilationShapeIter).CopyTo(m_arrInspItem, 34 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.GroundLowerThresh).CopyTo(m_arrInspItem, 35 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.GroundUpperThresh).CopyTo(m_arrInspItem, 36 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.ShapeLowerThresh).CopyTo(m_arrInspItem, 37 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.ShapeUpperThresh).CopyTo(m_arrInspItem, 38 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.CorrectMethod).CopyTo(m_arrInspItem, 39 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.ShapeShiftMarginX).CopyTo(m_arrInspItem, 40 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.ShapeShiftMarginY).CopyTo(m_arrInspItem, 41 * sizeof(int));

                        // 형상 영역 중앙선 검사 2012-09-12
                        BitConverter.GetBytes(shapeShiftWithCLProperty.IsInspectCL).CopyTo(m_arrInspItem, 72 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.MinWidthRatio).CopyTo(m_arrInspItem, 42 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.MinWidthSize).CopyTo(m_arrInspItem, 43 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.MaxWidthRatio).CopyTo(m_arrInspItem, 44 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.MaxWidthSize).CopyTo(m_arrInspItem, 45 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.MinHeightSize).CopyTo(m_arrInspItem, 47 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.MinNormalRatio).CopyTo(m_arrInspItem, 50 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.MaxNormalRatio).CopyTo(m_arrInspItem, 51 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.LeadThresh).CopyTo(m_arrInspItem, 53 * sizeof(int));

                        BitConverter.GetBytes(shapeShiftWithCLProperty.ThreshType2).CopyTo(m_arrInspItem, 77 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.InspLowerThresh2).CopyTo(m_arrInspItem, 78 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.InspUpperThresh2).CopyTo(m_arrInspItem, 79 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.ApplyAverDiff2).CopyTo(m_arrInspItem, 80 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.AverMinMargin2).CopyTo(m_arrInspItem, 81 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.AverMaxMargin2).CopyTo(m_arrInspItem, 82 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.InspRange2).CopyTo(m_arrInspItem, 83 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.InspMinMargin2).CopyTo(m_arrInspItem, 84 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.InspMaxMargin2).CopyTo(m_arrInspItem, 85 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.MinDefectSize2).CopyTo(m_arrInspItem, 86 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.IsInspectMaster).CopyTo(m_arrInspItem, 87 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.IsInspectSlave).CopyTo(m_arrInspItem, 88 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.ShapeShiftType).CopyTo(m_arrInspItem, 89 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.ShapeOffsetX).CopyTo(m_arrInspItem, 90 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.ShapeOffsetY).CopyTo(m_arrInspItem, 91 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.MaxDefectSize).CopyTo(m_arrInspItem, 92 * sizeof(int));
                        BitConverter.GetBytes(shapeShiftWithCLProperty.MaxDefectSize2).CopyTo(m_arrInspItem, 97 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeSurfaceHalfEtching: // 표면 Half Etching 검사
                case eVisInspectType.eInspTypeSurface:            // 표면 검사
                case eVisInspectType.eInspTypeSpace:              // 공간 검사
                case eVisInspectType.eInspTypePSR:          // 테이프 검사
                case eVisInspectType.eInspTypePunchBurr:
                case eVisInspectType.eInspTypeDownSet:
                    StateAlignedMaskProperty stateAlignedMaskProperty = aInspItem.InspectionAlgorithm as StateAlignedMaskProperty;
                    if (stateAlignedMaskProperty != null)
                    {
                        BitConverter.GetBytes(stateAlignedMaskProperty.ThreshType).CopyTo(m_arrInspItem, 12 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.UpperThresh).CopyTo(m_arrInspItem, 14 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.ApplyAverDiff).CopyTo(m_arrInspItem, 15 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.MaskLowerThresh).CopyTo(m_arrInspItem, 16 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.MaskUpperThresh).CopyTo(m_arrInspItem, 17 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.InspRange).CopyTo(m_arrInspItem, 18 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.AverMinMargin).CopyTo(m_arrInspItem, 19 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.AverMaxMargin).CopyTo(m_arrInspItem, 20 * sizeof(int));
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 21 * sizeof(int));
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 22 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.ErosionTrainIter).CopyTo(m_arrInspItem, 23 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.DilationTrainIter).CopyTo(m_arrInspItem, 24 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.ErosionInspIter).CopyTo(m_arrInspItem, 25 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.DilationInspIter).CopyTo(m_arrInspItem, 26 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.MinDefectSize).CopyTo(m_arrInspItem, 27 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.MinSmallDefectSize).CopyTo(m_arrInspItem, 28 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.MinSmallDefectCount).CopyTo(m_arrInspItem, 29 * sizeof(int));
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 31 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.MaxDefectSize).CopyTo(m_arrInspItem, 92 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.MaxSmallDefectSize).CopyTo(m_arrInspItem, 93 * sizeof(int));
                        BitConverter.GetBytes(stateAlignedMaskProperty.MaxSmallDefectCount).CopyTo(m_arrInspItem, 94 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeDamBar:          // Ball 검사
                    DamSizeProperty damProperty = aInspItem.InspectionAlgorithm as DamSizeProperty;
                    if (damProperty != null)
                    {
                        BitConverter.GetBytes(damProperty.ThreshType).CopyTo(m_arrInspItem, 12 * sizeof(int));
                        BitConverter.GetBytes(damProperty.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(damProperty.UpperThresh).CopyTo(m_arrInspItem, 14 * sizeof(int));
                        BitConverter.GetBytes((int)(afRes * 10.0)).CopyTo(m_arrInspItem, 21 * sizeof(int));
                        BitConverter.GetBytes(damProperty.MinDefectSize).CopyTo(m_arrInspItem, 27 * sizeof(int));
                        BitConverter.GetBytes(damProperty.MinWidth).CopyTo(m_arrInspItem, 43 * sizeof(int));
                        BitConverter.GetBytes(damProperty.MaxDefectSize).CopyTo(m_arrInspItem, 92 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeBallPattern:          // Ball 검사
                    BallPatternProperty ballProperty = aInspItem.InspectionAlgorithm as BallPatternProperty;
                    if (ballProperty != null)
                    {
                        BitConverter.GetBytes(ballProperty.ThreshType).CopyTo(m_arrInspItem, 12 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.UpperThresh).CopyTo(m_arrInspItem, 14 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.ApplyAverDiff).CopyTo(m_arrInspItem, 15 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.MaskLowerThresh).CopyTo(m_arrInspItem, 16 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.MaskUpperThresh).CopyTo(m_arrInspItem, 17 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.InspRange).CopyTo(m_arrInspItem, 18 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.AverMinMargin).CopyTo(m_arrInspItem, 19 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.AverMaxMargin).CopyTo(m_arrInspItem, 20 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.MinMargin).CopyTo(m_arrInspItem, 21 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.MaxMargin).CopyTo(m_arrInspItem, 22 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.ErosionTrainIter).CopyTo(m_arrInspItem, 23 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.DilationTrainIter).CopyTo(m_arrInspItem, 24 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.ErosionInspIter).CopyTo(m_arrInspItem, 25 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.DilationInspIter).CopyTo(m_arrInspItem, 26 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.MinDefectSize).CopyTo(m_arrInspItem, 27 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.MinSmallDefectSize).CopyTo(m_arrInspItem, 28 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.MinSmallDefectCount).CopyTo(m_arrInspItem, 29 * sizeof(int));

                        BitConverter.GetBytes((int)((double)ballProperty.MinWidthSize)).CopyTo(m_arrInspItem, 43 * sizeof(int));
                        BitConverter.GetBytes((int)((double)ballProperty.MinHeightSize)).CopyTo(m_arrInspItem, 47 * sizeof(int));
                        BitConverter.GetBytes((int)((double)ballProperty.MaxHeightSize)).CopyTo(m_arrInspItem, 49 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.UsePSRShift).CopyTo(m_arrInspItem, 60 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.UsePunchShift).CopyTo(m_arrInspItem, 64 * sizeof(int));
                        BitConverter.GetBytes((int)(((double)PSRMargin.X / afRes) * 100.0)).CopyTo(m_arrInspItem, 61 * sizeof(int));
                        BitConverter.GetBytes((int)(((double)PSRMargin.Y / afRes) * 100.0)).CopyTo(m_arrInspItem, 62 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.MaxDefectSize).CopyTo(m_arrInspItem, 92 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.MaxSmallDefectSize).CopyTo(m_arrInspItem, 93 * sizeof(int));
                        BitConverter.GetBytes(ballProperty.MaxSmallDefectCount).CopyTo(m_arrInspItem, 94 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeOuter:
                    OuterProperty outerProperty = aInspItem.InspectionAlgorithm as OuterProperty;
                    if (outerProperty != null)
                    {
                        BitConverter.GetBytes(outerProperty.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(outerProperty.UpperThresh).CopyTo(m_arrInspItem, 14 * sizeof(int));
                        BitConverter.GetBytes(outerProperty.MaskLowerThresh).CopyTo(m_arrInspItem, 16 * sizeof(int));
                        BitConverter.GetBytes(outerProperty.MaskUpperThresh).CopyTo(m_arrInspItem, 17 * sizeof(int));
                        BitConverter.GetBytes(outerProperty.ErosionTrainIter).CopyTo(m_arrInspItem, 23 * sizeof(int));
                        BitConverter.GetBytes(outerProperty.DilationTrainIter).CopyTo(m_arrInspItem, 24 * sizeof(int));
                        BitConverter.GetBytes(outerProperty.MinDefectSize).CopyTo(m_arrInspItem, 27 * sizeof(int));
                        BitConverter.GetBytes(outerProperty.MaxDefectSize).CopyTo(m_arrInspItem, 92 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;
                case eVisInspectType.eInspTypePSROdd: // PSR 하지이물
                    PSROddProperty psrOddProperty = aInspItem.InspectionAlgorithm as PSROddProperty;
                    if (psrOddProperty != null)
                    {
                        BitConverter.GetBytes(psrOddProperty.ThreshType).CopyTo(m_arrInspItem, 12 * sizeof(int));

                        // Core (RGB)
                        BitConverter.GetBytes(psrOddProperty.Core_Threshold).CopyTo(m_arrInspItem, 78 * sizeof(int));
                        BitConverter.GetBytes(psrOddProperty.Core_ExceptionThreshold).CopyTo(m_arrInspItem, 21 * sizeof(int));
                        BitConverter.GetBytes(psrOddProperty.Core_MinDefectSize).CopyTo(m_arrInspItem, 98 * sizeof(int));

                        //Metal_채도
                        BitConverter.GetBytes(psrOddProperty.Metal_LowerThreshold).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(psrOddProperty.Metal_UpperThreshold).CopyTo(m_arrInspItem, 14 * sizeof(int));
                        BitConverter.GetBytes(psrOddProperty.Metal_MinDefectSize).CopyTo(m_arrInspItem, 93 * sizeof(int));

                        // Nomal
                        BitConverter.GetBytes(psrOddProperty.Summation_range).CopyTo(m_arrInspItem, 31 * sizeof(int));
                        BitConverter.GetBytes(psrOddProperty.Summation_detection_size).CopyTo(m_arrInspItem, 27 * sizeof(int));
                        BitConverter.GetBytes(psrOddProperty.Mask_Threshold).CopyTo(m_arrInspItem, 16 * sizeof(int));
                        BitConverter.GetBytes(psrOddProperty.Mask_Extension).CopyTo(m_arrInspItem, 24 * sizeof(int));
                        BitConverter.GetBytes(psrOddProperty.Step_Threshold).CopyTo(m_arrInspItem, 37 * sizeof(int));
                        BitConverter.GetBytes(psrOddProperty.Step_Expansion).CopyTo(m_arrInspItem, 26 * sizeof(int));

                        // 필터
                        BitConverter.GetBytes(psrOddProperty.HV_ratio_value).CopyTo(m_arrInspItem, 46 * sizeof(int));
                        BitConverter.GetBytes(psrOddProperty.Min_Relative_size).CopyTo(m_arrInspItem, 50 * sizeof(int));
                        BitConverter.GetBytes(psrOddProperty.Max_Relative_size).CopyTo(m_arrInspItem, 51 * sizeof(int));
                        BitConverter.GetBytes(psrOddProperty.Area_Relative).CopyTo(m_arrInspItem, 44 * sizeof(int));

                        //Circuit
                        BitConverter.GetBytes(psrOddProperty.Circuit_Threshold).CopyTo(m_arrInspItem, 42 * sizeof(int));
                        BitConverter.GetBytes(psrOddProperty.Circuit_MinDefectSize).CopyTo(m_arrInspItem, 43 * sizeof(int));



                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspResultGV: // GV 검사
                    GV_Inspection_Property gv_Inspection_Property = aInspItem.InspectionAlgorithm as GV_Inspection_Property;
                    if (gv_Inspection_Property != null)
                    {
                        // Ball 임계값이 0이면 사용안함을 의미!!! 그리고 비전에 255를 보내줘야 사용 안함으로 처리됨
                        if (gv_Inspection_Property.Ball_Thresh == 0)
                            BitConverter.GetBytes(255).CopyTo(m_arrInspItem, 16 * sizeof(int));
                        else
                            BitConverter.GetBytes(gv_Inspection_Property.Ball_Thresh).CopyTo(m_arrInspItem, 16 * sizeof(int));

                        BitConverter.GetBytes(gv_Inspection_Property.Core_Thresh).CopyTo(m_arrInspItem, 17 * sizeof(int));
                        BitConverter.GetBytes(gv_Inspection_Property.Mask).CopyTo(m_arrInspItem, 25 * sizeof(int));
                        BitConverter.GetBytes(gv_Inspection_Property.Taget_GV).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(gv_Inspection_Property.GV_Margin).CopyTo(m_arrInspItem, 12 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeIDMark:
                    break;

                case eVisInspectType.eInspTypeUnitFidu:          // 인식키 검사
                    CrossPatternProperty crossPatternProperty = aInspItem.InspectionAlgorithm as CrossPatternProperty;
                    if (crossPatternProperty != null)
                    {
                        BitConverter.GetBytes(crossPatternProperty.ThreshType).CopyTo(m_arrInspItem, 12 * sizeof(int));
                        BitConverter.GetBytes(crossPatternProperty.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(crossPatternProperty.UpperThresh).CopyTo(m_arrInspItem, 14 * sizeof(int));
                        BitConverter.GetBytes(crossPatternProperty.LowerThresh).CopyTo(m_arrInspItem, 16 * sizeof(int));
                        BitConverter.GetBytes(crossPatternProperty.UpperThresh).CopyTo(m_arrInspItem, 17 * sizeof(int));
                        BitConverter.GetBytes(crossPatternProperty.UsePSRShift).CopyTo(m_arrInspItem, 60 * sizeof(int));
                        BitConverter.GetBytes(crossPatternProperty.UsePSRShiftBA).CopyTo(m_arrInspItem, 63 * sizeof(int));
                        BitConverter.GetBytes((int)(((double)PSRMargin.X / afRes) * 100.0)).CopyTo(m_arrInspItem, 61 * sizeof(int));
                        BitConverter.GetBytes((int)(((double)PSRMargin.Y / afRes) * 100.0)).CopyTo(m_arrInspItem, 62 * sizeof(int));
                        BitConverter.GetBytes(crossPatternProperty.UsePunchShift).CopyTo(m_arrInspItem, 64 * sizeof(int));

                        BitConverter.GetBytes(crossPatternProperty.MinDefectSize).CopyTo(m_arrInspItem, 27 * sizeof(int));
                        BitConverter.GetBytes(crossPatternProperty.MinHeightsize).CopyTo(m_arrInspItem, 47 * sizeof(int));

                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeUnitPattern:          // Ball 검사
                    UnitPatternProperty unitPatternProperty = aInspItem.InspectionAlgorithm as UnitPatternProperty;
                    if (unitPatternProperty != null)
                    {
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 12 * sizeof(int));
                        BitConverter.GetBytes(unitPatternProperty.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(unitPatternProperty.MinDefectSize).CopyTo(m_arrInspItem, 27 * sizeof(int));

                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeVentHole:          // 벤트 홀 검사
                    VentHoleProperty ventHoleProperty = aInspItem.InspectionAlgorithm as VentHoleProperty;
                    if (ventHoleProperty != null)
                    {
                        BitConverter.GetBytes(ventHoleProperty.ThreshType).CopyTo(m_arrInspItem, 12 * sizeof(int));
                        BitConverter.GetBytes(ventHoleProperty.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(ventHoleProperty.MinDefectSize).CopyTo(m_arrInspItem, 27 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeNoResizeVentHole:          // 벤트 홀2 검사
                    VentHoleProperty2 ventHoleProperty2 = aInspItem.InspectionAlgorithm as VentHoleProperty2;
                    if (ventHoleProperty2 != null)
                    {
                        BitConverter.GetBytes(ventHoleProperty2.ThreshType).CopyTo(m_arrInspItem, 12 * sizeof(int));
                        BitConverter.GetBytes(ventHoleProperty2.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(ventHoleProperty2.MinDefectSize).CopyTo(m_arrInspItem, 27 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeCoreCrack:          // Ball 검사
                    WindowPunchProperty windowPunchProperty = aInspItem.InspectionAlgorithm as WindowPunchProperty;
                    if (windowPunchProperty != null)
                    {
                        BitConverter.GetBytes(windowPunchProperty.ThreshType).CopyTo(m_arrInspItem, 12 * sizeof(int));
                        BitConverter.GetBytes(windowPunchProperty.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(windowPunchProperty.UpperThresh).CopyTo(m_arrInspItem, 14 * sizeof(int));
                        BitConverter.GetBytes(windowPunchProperty.LowerThresh).CopyTo(m_arrInspItem, 16 * sizeof(int));
                        BitConverter.GetBytes(windowPunchProperty.UpperThresh).CopyTo(m_arrInspItem, 17 * sizeof(int));
                        BitConverter.GetBytes(windowPunchProperty.ErosionTrainIter).CopyTo(m_arrInspItem, 23 * sizeof(int));
                        BitConverter.GetBytes(windowPunchProperty.UsePSRShift).CopyTo(m_arrInspItem, 60 * sizeof(int));
                        BitConverter.GetBytes(windowPunchProperty.PsrMarginX).CopyTo(m_arrInspItem, 61 * sizeof(int));
                        BitConverter.GetBytes(windowPunchProperty.PsrMarginY).CopyTo(m_arrInspItem, 62 * sizeof(int));
                        BitConverter.GetBytes(windowPunchProperty.UsePunchShift).CopyTo(m_arrInspItem, 64 * sizeof(int));

                        BitConverter.GetBytes(windowPunchProperty.MinDefectSize).CopyTo(m_arrInspItem, 27 * sizeof(int));
                        BitConverter.GetBytes(windowPunchProperty.MinHeightsize).CopyTo(m_arrInspItem, 47 * sizeof(int));

                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeUnitRawMaterial:          // 테이프 검사
                    RawMetrialProperty rawmetrialProperty = aInspItem.InspectionAlgorithm as RawMetrialProperty;
                    if (rawmetrialProperty != null)
                    {
                        BitConverter.GetBytes(rawmetrialProperty.ThreshType).CopyTo(m_arrInspItem, 12 * sizeof(int));
                        if (rawmetrialProperty.ThreshType == 0)
                        {
                            rawmetrialProperty.UpperThresh = 255;
                        }
                        else if (rawmetrialProperty.ThreshType == 1)
                        {
                            rawmetrialProperty.LowerThresh = 0;
                        }
                        BitConverter.GetBytes(rawmetrialProperty.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.UpperThresh).CopyTo(m_arrInspItem, 14 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.ApplyAverDiff).CopyTo(m_arrInspItem, 15 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.MaskLowerThresh).CopyTo(m_arrInspItem, 16 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.MaskUpperThresh).CopyTo(m_arrInspItem, 17 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.InspRange).CopyTo(m_arrInspItem, 18 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.AverMinMargin).CopyTo(m_arrInspItem, 19 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.AverMaxMargin).CopyTo(m_arrInspItem, 20 * sizeof(int));
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 21 * sizeof(int));
                        BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 22 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.ErosionTrainIter).CopyTo(m_arrInspItem, 23 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.DilationTrainIter).CopyTo(m_arrInspItem, 24 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.ErosionInspIter).CopyTo(m_arrInspItem, 25 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.DilationInspIter).CopyTo(m_arrInspItem, 26 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.MinDefectSize).CopyTo(m_arrInspItem, 27 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.MinSmallDefectSize).CopyTo(m_arrInspItem, 28 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.MinSmallDefectCount).CopyTo(m_arrInspItem, 29 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.SumDefectSize).CopyTo(m_arrInspItem, 31 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.MaxDefectSize).CopyTo(m_arrInspItem, 92 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.MaxSmallDefectSize).CopyTo(m_arrInspItem, 93 * sizeof(int));
                        BitConverter.GetBytes(rawmetrialProperty.MaxSmallDefectCount).CopyTo(m_arrInspItem, 94 * sizeof(int));
                    }
                    BitConverter.GetBytes(aInspItem.IsExceptionSkip).CopyTo(m_arrInspItem, 73 * sizeof(int));
                    break;

                case eVisInspectType.eInspTypeLeadGap:
                    LeadGapProperty leadGapProperty = aInspItem.InspectionAlgorithm as LeadGapProperty;
                    if (leadGapProperty != null)
                    {
                        // 2012-04-02 suoow2. : 리드 간격 검사일 때 반사와 투과 조명에 대한 구분을 둔다.
                        if (anSurfaceID < 2) // 상부, 하부 반사 : 밝은 불량 검출. (0 or 1)
                        {
                            BitConverter.GetBytes((int)2).CopyTo(m_arrInspItem, 18 * sizeof(int));
                        }
                        else if (anSurfaceID == 2) // 상부 투과 : 어두운 불량 검출. (2)
                        {
                            BitConverter.GetBytes((int)1).CopyTo(m_arrInspItem, 18 * sizeof(int));
                        }

                        BitConverter.GetBytes(leadGapProperty.LowerThresh).CopyTo(m_arrInspItem, 13 * sizeof(int));
                        BitConverter.GetBytes(leadGapProperty.UpperThresh).CopyTo(m_arrInspItem, 14 * sizeof(int));
                        BitConverter.GetBytes(leadGapProperty.MinWidthSize).CopyTo(m_arrInspItem, 43 * sizeof(int));
                    }
                    break;

                case eVisInspectType.eInspTypeExceptionalMask: // 검사 제외
                    ExceptionalMaskProperty exceptionalMaskProperty = aInspItem.InspectionAlgorithm as ExceptionalMaskProperty;
                    if (exceptionalMaskProperty != null)
                    {
                        BitConverter.GetBytes(exceptionalMaskProperty.ExceptionX).CopyTo(m_arrInspItem, 74 * sizeof(int));
                        BitConverter.GetBytes(exceptionalMaskProperty.ExceptionY).CopyTo(m_arrInspItem, 75 * sizeof(int));
                        BitConverter.GetBytes(exceptionalMaskProperty.UseShapeShift).CopyTo(m_arrInspItem, 72 * sizeof(int));
                    }
                    break;
            }

            return m_arrInspItem;
        }
        #endregion Make Insp Item Packet.

        public static byte[] StripAlignToBytes(int anSurfaceID, int anInspID, int anSecID, int anRoiID)
        {
            for (int i = 0; i < InspSize; i++)
            {
                m_arrInspItem[i] = 0;
            }

            BitConverter.GetBytes(anInspID).CopyTo(m_arrInspItem, 0 * sizeof(int));
            BitConverter.GetBytes(anSecID).CopyTo(m_arrInspItem, 1 * sizeof(int));
            BitConverter.GetBytes(anRoiID).CopyTo(m_arrInspItem, 2 * sizeof(int));
            BitConverter.GetBytes((int)eVisInspectType.eInspTypeGlobalAlign).CopyTo(m_arrInspItem, 3 * sizeof(int));
            BitConverter.GetBytes((int)eVisInspectAlgo.eInspAlgoStripAlign).CopyTo(m_arrInspItem, 4 * sizeof(int));
            BitConverter.GetBytes((int)eInspectPriority.eInspPriorityGlobalAlign).CopyTo(m_arrInspItem, 5 * sizeof(int));
            BitConverter.GetBytes(101).CopyTo(m_arrInspItem, 6 * sizeof(int));
            BitConverter.GetBytes(101).CopyTo(m_arrInspItem, 7 * sizeof(int));
            BitConverter.GetBytes(101).CopyTo(m_arrInspItem, 8 * sizeof(int));
            BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 9 * sizeof(int));

            BitConverter.GetBytes(300).CopyTo(m_arrInspItem, 56 * sizeof(int));
            BitConverter.GetBytes(300).CopyTo(m_arrInspItem, 57 * sizeof(int));
            BitConverter.GetBytes(70).CopyTo(m_arrInspItem, 58 * sizeof(int));
            BitConverter.GetBytes(0).CopyTo(m_arrInspItem, 72 * sizeof(int));

            return m_arrInspItem;
        }
        public static byte[] ICS_UNITToBytes(List<Point> UnitPos, int Grabside)
        {
            byte[] arrResult = new Byte[ICS_UnitPos]; 
            int Count = 0;
            BitConverter.GetBytes((int)Grabside).CopyTo(arrResult, 0 + (sizeof(int) * Count++));         
            foreach (Point p in UnitPos) {        
                BitConverter.GetBytes((int)(p.X / VisionDefinition.GRAB_IMAGE_SCALE)).CopyTo(arrResult, 0 + (sizeof(int) * Count++));
                BitConverter.GetBytes((int)(p.Y / VisionDefinition.GRAB_IMAGE_SCALE)).CopyTo(arrResult, 0 + (sizeof(int) * Count++));
            }
            return arrResult;
        }
    }
}
