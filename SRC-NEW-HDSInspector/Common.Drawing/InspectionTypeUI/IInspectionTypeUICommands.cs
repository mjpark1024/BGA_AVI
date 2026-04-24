using System;
using System.Collections.Generic;
using System.Linq;
using Common.Drawing.InspectionInformation;

namespace Common.Drawing.InspectionTypeUI
{
    /// <summary>   Interface for inspection type user interface commands.  </summary>
    /// <remarks>   Minseok, Hwang, 2011-09-24. </remarks>
    public interface IInspectionTypeUICommands
    {
        void SetDialog(string strCaption, eVisInspectType inspectType);
        void TrySave(GraphicsBase graphic, int anInspectID);
        void SetDefaultValue();
        void Display(InspectionItem settingValue);
    }
}
