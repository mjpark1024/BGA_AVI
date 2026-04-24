using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Drawing.InspectionInformation
{
    /// <summary>   Interface for inspection type user interface commands.  </summary>
    /// <remarks>   suoow2, 2014-09-24. </remarks>
    public interface IInspectionTypeUICommands
    {
        void SetDialog(string strCaption, eVisInspectType inspectType);
        void TrySave(GraphicsBase graphic, int anInspectID, ParametersLocking paradata);
        void AllCircuitPartChange(GraphicsBase graphic, int anInspectID);
        void AllNonCircuitPartChange(GraphicsBase graphic, int anInspectID);
        void TryAdd(ref InspectList list, int anInspectID);
        void SetPreviewValue();
        void SetDefaultValue();
        void Display(InspectionItem settingValue, int MarginX, int MatginY);
    }
}
