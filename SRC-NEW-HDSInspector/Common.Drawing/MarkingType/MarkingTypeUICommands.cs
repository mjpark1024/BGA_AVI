using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Drawing.MarkingInformation
{
    public delegate void MoveEventHandler(int id, double pitch, eMarkingType type);
    public delegate void SizeChangeEventHandler(double X, double Y, double Width, double Height, double Angle, eMarkingType type);
    public delegate void LocationChangeEventHandler(int Location);    
    public delegate void PenParamChanged(int nParam);
    public delegate void SaveChanged();
    public interface IMarkingTypeUICommands
    {
        event MoveEventHandler MoveClick;
        event SizeChangeEventHandler MarkSizeChanged;
        event LocationChangeEventHandler LocationChanged;
        event PenParamChanged ParamChange;
        event SaveChanged SaveChange;
        void SetDialog(double resolution, eMarkingType markType);
        void TrySave(GraphicsBase graphic);
        void TryAdd(ref MarkItem item);
        void SetPreviewValue();
        void SetDefaultValue();
        void Display(GraphicsBase settingValue, bool isdraw = false);
        void LocationEnabled(bool IsEnable);
    }
}
