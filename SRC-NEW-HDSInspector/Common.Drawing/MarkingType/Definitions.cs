using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Drawing.MarkingInformation
{
    public enum eMarkingType
    {
        eMarkingNone =0,
        eMarkingRailCircle = 1,
        eMarkingRailRect = 2,
        eMarkingRailTri = 3,
        eMarkingRailSpecial = 4,
        eMarkingUnitCircle = 5,
        eMarkingUnitRect = 6,
        eMarkingUnitTri = 7,
        eMarkingUnitSpecial = 8,
        eMarkingNumber = 9,
        eMarkingWeek = 10,
        eMarkingIDMark = 11,
        eUnitGuide = 12,
        eMarkingTBD = 13,
    }

    public enum eMarkSectionType
    {
        eSecTypeNone = 0,
        eSecTypeRail = 1,
        eSecTypeUnit = 2,
        eSecTypeID = 3, 
        eSecTypeNum = 4,
        eSecTypeWeek = 5,
    }
}
