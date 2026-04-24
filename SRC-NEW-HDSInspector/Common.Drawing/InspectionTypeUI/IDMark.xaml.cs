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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Common.Drawing.InspectionInformation;

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>
    /// IDMark.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class IDMark : UserControl, IInspectionTypeUICommands
    {

        public eVisInspectType InspectType
        {
            get
            {
                return m_enumInspectType;
            }
        }
        private eVisInspectType m_enumInspectType;

        public IDMark()
        {
            InitializeComponent();
        }

        public void SetDialog(string strCaption, eVisInspectType inspectType)
        {
            this.txtCaption.Text = strCaption;
            this.m_enumInspectType = inspectType;
        }

        public void TrySave(GraphicsBase graphic, int anInspectID, ParametersLocking paradata)
        {
            try
            {
                IDAreaProperty oldFiducialAlignValue = null;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (element.InspectionType != null && element.ID == anInspectID)
                    {
                        oldFiducialAlignValue = element.InspectionAlgorithm as IDAreaProperty; // 기존에 저장된 값이 있는 경우.
                        break;
                    }
                }
                int nMaxInspectID = -1;
                foreach (InspectionItem element in graphic.InspectionList)
                {
                    if (nMaxInspectID < element.ID)
                        nMaxInspectID = element.ID;
                }
                if (oldFiducialAlignValue == null) // 기존에 저장된 값이 없는 경우, 새로 리스트에 추가시킨다.
                {
                    IDAreaProperty fiducialAlignValue = new IDAreaProperty();
                    InspectionItem inspectionItem = new InspectionItem();
                    inspectionItem.ID = nMaxInspectID + 1;
                    inspectionItem.InspectionType = InspectionType.GetInspectionType(m_enumInspectType);
                    inspectionItem.InspectionAlgorithm = fiducialAlignValue;

                    if (inspectionItem.InspectionType != null)
                    {
                        graphic.InspectionList.Add(inspectionItem);
                    }
                }
                else
                {
                    oldFiducialAlignValue.ThreshType = 0;
                }
            }
            catch
            {

            }
        }

        // 검사 설정 저장.
        public void TryAdd(ref InspectList list, int anInspectID)
        {

        }

        // 검사 설정 직전 값 표시.
        public void SetPreviewValue()
        {

        }

        // 검사 설정 기본 값 표시.
        public void SetDefaultValue()
        {

        }

        // 검사 설정 표시.
        public void Display(InspectionItem inspectionItem, int MarginX, int MatginY)
        {

        }

        public void AllCircuitPartChange(GraphicsBase graphic, int anInspectID)
        {
        }
        public void AllNonCircuitPartChange(GraphicsBase graphic, int anInspectID)
        {
        }
    }
}
