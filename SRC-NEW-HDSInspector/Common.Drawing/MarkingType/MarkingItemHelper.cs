using System;
using System.Collections.Generic;
using System.Linq;
using Common.Drawing.MarkingTypeUI;

namespace Common.Drawing.MarkingInformation
{
    public static class MarkingItemHelper
    {
        public static MarkItem CopyMarkItem(MarkItem markItem)
        {
            if (markItem != null)
            {
                MarkItem item = new MarkItem(){
                ID = markItem.ID,
                MarkType = markItem.MarkType,
                MarkInfo = markItem.MarkInfo.Clone()
                };
                return item;
            }
            return null;
        }

        public static MarkItem GetMarkingItem(double x, double y)
        {
            MarkingType markType = null;
            List<MarkingType> markTypeList = MarkingType.GetMarkTypeList();
            if (markTypeList != null)
            {
                foreach (MarkingType markTypeElement in markTypeList)
                {
                    if (eMarkingType.eUnitGuide == markTypeElement.MarkType)
                    {
                        markType = markTypeElement;
                        break;
                    }
                }
            }
            if (markType == null)
                return null; 
            UnitGuideProperty guideProperty = new UnitGuideProperty();
            guideProperty.StartX = x;
            guideProperty.StartY = y;
           return new MarkItem(markType, guideProperty.Clone(), 0);
        }

        public static MarkItem GetMarkingItem(Strip strip, HPGL hpgl, eMarkingType type)
        {
            MarkingType markType = null;
            List<MarkingType> markTypeList = MarkingType.GetMarkTypeList();
            if (markTypeList != null)
            {
                foreach (MarkingType markTypeElement in markTypeList)
                {
                    if (type == markTypeElement.MarkType)
                    {
                        markType = markTypeElement;
                        break;
                    }
                }
            }
            if (markType == null)
                return null;
            switch (markType.MarkType)
            {
                case eMarkingType.eMarkingRailCircle: // Strip Align
                    RailCircleProperty railCircleProperty = new RailCircleProperty();
                    railCircleProperty.LoadProperty(strip, hpgl);
                    return new MarkItem(markType, railCircleProperty.Clone(), 0);
                case eMarkingType.eMarkingRailRect: // Strip Align
                    RailRectProperty railRectProperty = new RailRectProperty();
                    railRectProperty.LoadProperty(strip, hpgl);
                    return new MarkItem(markType, railRectProperty.Clone(), 0);
                case eMarkingType.eMarkingRailTri: // Strip Align
                    RailTriProperty railTriProperty = new RailTriProperty();
                    railTriProperty.LoadProperty(strip, hpgl);
                    return new MarkItem(markType, railTriProperty.Clone(), 0);
                case eMarkingType.eMarkingRailSpecial: // Strip Align
                    RailSpecialProperty railSpecialProperty = new RailSpecialProperty();
                    railSpecialProperty.LoadProperty(strip, hpgl);
                    return new MarkItem(markType, railSpecialProperty.Clone(), 0);
                case eMarkingType.eMarkingUnitCircle: // Strip Align
                    UnitCircleProperty unitCircleProperty = new UnitCircleProperty();
                    unitCircleProperty.LoadProperty(strip, hpgl);
                    return new MarkItem(markType, unitCircleProperty.Clone(), 0);
                case eMarkingType.eMarkingUnitRect: // Strip Align
                    UnitRectProperty unitRectProperty = new UnitRectProperty();
                    unitRectProperty.LoadProperty(strip, hpgl);
                    return new MarkItem(markType, unitRectProperty.Clone(), 0);
                case eMarkingType.eMarkingUnitTri: // Strip Align
                    UnitTriProperty unitTriProperty = new UnitTriProperty();
                    unitTriProperty.LoadProperty(strip, hpgl);
                    return new MarkItem(markType, unitTriProperty.Clone(), 0);
                case eMarkingType.eMarkingUnitSpecial: // Strip Align
                    UnitSpecialProperty unitSpecialProperty = new UnitSpecialProperty();
                    unitSpecialProperty.LoadProperty(strip, hpgl);
                    return new MarkItem(markType, unitSpecialProperty.Clone(), 0);
                case eMarkingType.eMarkingNumber: // Strip Align
                    NumberProperty numberProperty = new NumberProperty();
                    numberProperty.LoadProperty(strip, hpgl);
                    return new MarkItem(markType, numberProperty.Clone(), 0);
                case eMarkingType.eMarkingWeek: // Strip Align
                    WeekProperty weekProperty = new WeekProperty();
                    weekProperty.LoadProperty(strip, hpgl);
                    return new MarkItem(markType, weekProperty.Clone(), 0);
                case eMarkingType.eMarkingIDMark: // Strip Align
                    IDMarkProperty idProperty = new IDMarkProperty();
                    idProperty.LoadProperty(strip, hpgl);
                    return new MarkItem(markType, idProperty.Clone(), 0);
                case eMarkingType.eUnitGuide: // Strip Align
                    UnitGuideProperty guideProperty = new UnitGuideProperty();
                    guideProperty.LoadProperty(strip, hpgl);
                    return new MarkItem(markType, guideProperty.Clone(), 0);
                default:
                    return null;
            }
        }

        public static MarkItem GetMarkingItem(Strip strip, IDMARK text, eMarkingType type)
        {
            MarkingType markType = null;
            List<MarkingType> markTypeList = MarkingType.GetMarkTypeList();
            if (markTypeList != null)
            {
                foreach (MarkingType markTypeElement in markTypeList)
                {
                    if (type == markTypeElement.MarkType)
                    {
                        markType = markTypeElement;
                        break;
                    }
                }
            }
            if (markType == null)
                return null;

            TBDProperty railCircleProperty = new TBDProperty();
            railCircleProperty.LoadProperty(strip, text);
            return new MarkItem(markType, railCircleProperty.Clone(), 0);
        }

        public static MarkItem GetMarkingItem(Strip strip, TEXT text, eMarkingType type, int IDLoc, int CountLoc, int Weekloc)
        {
            MarkingType markType = null;
            List<MarkingType> markTypeList = MarkingType.GetMarkTypeList();
            if (markTypeList != null)
            {
                foreach (MarkingType markTypeElement in markTypeList)
                {
                    if (type == markTypeElement.MarkType)
                    {
                        markType = markTypeElement;
                        break;
                    }
                }
            }
            if (markType == null)
                return null;
            switch (markType.MarkType)
            {
                case eMarkingType.eMarkingRailCircle: // Strip Align
                    RailCircleProperty railCircleProperty = new RailCircleProperty();
                    railCircleProperty.LoadProperty(strip, text);
                    return new MarkItem(markType, railCircleProperty.Clone(), 0);
                case eMarkingType.eMarkingRailRect: // Strip Align
                    RailRectProperty railRectProperty = new RailRectProperty();
                    railRectProperty.LoadProperty(strip, text);
                    return new MarkItem(markType, railRectProperty.Clone(), 0);
                case eMarkingType.eMarkingRailTri: // Strip Align
                    RailTriProperty railTriProperty = new RailTriProperty();
                    railTriProperty.LoadProperty(strip, text);
                    return new MarkItem(markType, railTriProperty.Clone(), 0);
                case eMarkingType.eMarkingRailSpecial: // Strip Align
                    RailSpecialProperty railSpecialProperty = new RailSpecialProperty();
                    railSpecialProperty.LoadProperty(strip, text);
                    return new MarkItem(markType, railSpecialProperty.Clone(), 0);
                case eMarkingType.eMarkingUnitCircle: // Strip Align
                    UnitCircleProperty unitCircleProperty = new UnitCircleProperty();
                    unitCircleProperty.LoadProperty(strip, text);
                    return new MarkItem(markType, unitCircleProperty.Clone(), 0);
                case eMarkingType.eMarkingUnitRect: // Strip Align
                    UnitRectProperty unitRectProperty = new UnitRectProperty();
                    unitRectProperty.LoadProperty(strip, text);
                    return new MarkItem(markType, unitRectProperty.Clone(), 0);
                case eMarkingType.eMarkingUnitTri: // Strip Align
                    UnitTriProperty unitTriProperty = new UnitTriProperty();
                    unitTriProperty.LoadProperty(strip, text);
                    return new MarkItem(markType, unitTriProperty.Clone(), 0);
                case eMarkingType.eMarkingUnitSpecial: // Strip Align
                    UnitSpecialProperty unitSpecialProperty = new UnitSpecialProperty();
                    unitSpecialProperty.LoadProperty(strip, text);
                    return new MarkItem(markType, unitSpecialProperty.Clone(), 0);
                case eMarkingType.eMarkingNumber: // Strip Align
                    NumberProperty numberProperty = new NumberProperty();
                    numberProperty.LoadProperty(strip, text, CountLoc);
                    return new MarkItem(markType, numberProperty.Clone(), 0);
                case eMarkingType.eMarkingWeek: // Strip Align
                    WeekProperty weekProperty = new WeekProperty();
                    weekProperty.LoadProperty(strip, text, Weekloc);
                    return new MarkItem(markType, weekProperty.Clone(), 0);
                case eMarkingType.eMarkingIDMark: // Strip Align
                    IDMarkProperty idProperty = new IDMarkProperty();
                    idProperty.LoadProperty(strip, text, IDLoc);
                    return new MarkItem(markType, idProperty.Clone(), 0);
                case eMarkingType.eUnitGuide: // Strip Align
                    UnitGuideProperty unitGuideProperty = new UnitGuideProperty();
                    return new MarkItem(markType, unitGuideProperty.Clone(), 0);
                default:
                    return null;
            }
        }
    }
}
