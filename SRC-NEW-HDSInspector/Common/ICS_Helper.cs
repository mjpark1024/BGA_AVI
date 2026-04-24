using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Common
{
    public enum SectionArea
    {
        Unit,
        Outer,
        Material,        
    }
    public class MachineInfo
    {
        public string Name { get; set; }
        public double ResX { get; set; }
        public double ResY { get; set; }
    }
    public class Modelinformation
    {
        public int UnitCountX { get; set; }
        public int UnitCountY { get; set; }
        public double UnitPitchX { get; set; }
        public double UnitPitchY { get; set; }
        public int BlockCountX { get; set; }
        public int BlockCountY { get; set; }
        public double BlockDistX { get; set; }
        public double BlockDistY { get; set; }
        public int StripSizeX { get; set; }
        public int StripSizeY { get; set; }
        public XoutInfo XOutInfo { get; set; }
        public List<AutoNGInformation> AutoNGInfo{ get; set; }
}
    public class DefInfo
    {
        public int UnitX { get; set; }
        public int UnitY { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int UnitOffsetX { get; set; }
        public int UnitOffsetY { get; set; }
        public string ImageFullPathName { get; set; }
        public string DefZone { get; set; }
        public double DefSize { get; set; }
        public int DefectPosX { get; set; }
        public int DefectPosY { get; set; }
        public string AVI_AI_Code { get; set; }
        public string AVI_AI_Name { get; set; }
        public string AVI_AI_Score { get; set; }
        public string ICS_ImageFullPathName1 { get; set; }
        public string ICS_ImageFullPathName2 { get; set; }
        public string ICS_ImageFullPathName3 { get; set; }
        public string ICS_AI_Code { get; set; }
        public string ICS_AI_Name { get; set; }
        public string ICS_AI_Score { get; set; }
        public string IVS_Operator { get; set; }
        public string IVS_Code { get; set; }
        public string IVS_Name { get; set; }
    }
    public class SectionInfo
    {
        public int UnitX { get; set; }
        public int UnitY { get; set; }
        public int UnitPosX { get; set; }
        public int UnitPosY { get; set; }
    }
    public class AlignOffsetInfo
    {
        public System.Windows.Point LeftTop { get; set; }
        public System.Windows.Point LeftBottom { get; set; }
        public AlignOffsetInfo()
        {
        }
        public AlignOffsetInfo(AlignOffsetInfo Other)
        {
            LeftTop = Other.LeftTop;
            LeftBottom = Other.LeftBottom;
        }
    }

    public class ICS_Data
    {
        public string ModelName { get; set; }
        public string LOT_NO { get; set; }
        public string Side { get; set; }
        public Modelinformation ModelInfo { get; set; }
        public MachineInfo MachineInfo { get; set; }
        public List<SectionInfo> AlignOffset { get; set; }
        public List<SectionInfo> UnitInfo { get; set; }
        public List<SectionInfo> OuterInfo { get; set; }
        public List<SectionInfo> MaterialInfo { get; set; }
        public List<DefInfo> UnitDefInfo { get; set; }
        public List<DefInfo> OuterDefInfo { get; set; }
        public List<DefInfo> MaterialDefInfo { get; set; }
    }
    public class XoutInfo
    {
        public int NGCount { get; set; }
        public int NGContinueX { get; set; }
        public int NGContinueY { get; set; }
        public int NGBlockCount { get; set; }
        public int NGOutUnitCount { get; set; }
        public int NGOutUnitXMode { get; set; }
        public int NGOutDivideUnitCount { get; set; }
        public List<System.Windows.Point> NGMatrix { get; set; }
    }
    public class AutoNGInformation
    {
        public string DefectName { get; set; }
        public string DefectCode { get; set; }
        public bool Enable { get; set; }
        public AutoNGInformation(string defectname, string defectcode, bool enable)
        { 
            DefectName = defectname;
            DefectCode = defectcode;
            Enable = enable;
        }
    }
}
