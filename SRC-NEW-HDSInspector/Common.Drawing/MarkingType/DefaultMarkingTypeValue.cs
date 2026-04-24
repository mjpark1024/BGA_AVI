using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Drawing.MarkingInformation
{
    #region Rail Circle type.
    /// <summary>   Rail Circle default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class RailCircleDefaultValue
    {
        public static bool DefaultValueLoaded = false;
        public static int ParamID;     // 0000
        public static string PLTFile;  // 0001
        public static int MarkType;    // 0002

        public static void LoadDefaultValue()
        {
            string markTypeCode = "0101";
            string groupCode = "4001";
            ParamID = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0000");
            PLTFile = MarkingQueryHelper.GetDefaultParamValueStr(markTypeCode, groupCode, "0001");
            MarkType = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0002");
        }
    }
    #endregion

    #region Rail Rect type.
    /// <summary>   Rail Rect default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class RailRectDefaultValue
    {
        public static bool DefaultValueLoaded = false;
        public static int ParamID;     // 0000
        public static string PLTFile;  // 0001
        public static int MarkType;    // 0002

        public static void LoadDefaultValue()
        {
            string markTypeCode = "0102";
            string groupCode = "4002";
            ParamID = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0000");
            PLTFile = MarkingQueryHelper.GetDefaultParamValueStr(markTypeCode, groupCode, "0001");
            MarkType = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0002");
        }
    }
    #endregion

    #region Rail Triangle type.
    /// <summary>   Rail Triangle default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class RailTriDefaultValue
    {
        public static bool DefaultValueLoaded = false;
        public static int ParamID;     // 0000
        public static string PLTFile;  // 0001
        public static int MarkType;    // 0002

        public static void LoadDefaultValue()
        {
            string markTypeCode = "0103";
            string groupCode = "4003";
            ParamID = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0000");
            PLTFile = MarkingQueryHelper.GetDefaultParamValueStr(markTypeCode, groupCode, "0001");
            MarkType = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0002");
        }
    }
    #endregion

    #region Rail Special type.
    /// <summary>   Rail Special default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class RailSpecialDefaultValue
    {
        public static bool DefaultValueLoaded = false;
        public static int ParamID;     // 0000
        public static string PLTFile;  // 0001
        public static int MarkType;    // 0002

        public static void LoadDefaultValue()
        {
            string markTypeCode = "0104";
            string groupCode = "4004";
            ParamID = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0000");
            PLTFile = MarkingQueryHelper.GetDefaultParamValueStr(markTypeCode, groupCode, "0001");
            MarkType = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0002");
        }
    }
    #endregion

    #region Unit Circle type.
    /// <summary>   Unit Circle default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class UnitCircleDefaultValue
    {
        public static bool DefaultValueLoaded = false;
        public static int ParamID;     // 0000
        public static string PLTFile;  // 0001
        public static int MarkType;    // 0002

        public static void LoadDefaultValue()
        {
            string markTypeCode = "0201";
            string groupCode = "4011";
            ParamID = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0000");
            PLTFile = MarkingQueryHelper.GetDefaultParamValueStr(markTypeCode, groupCode, "0001");
            MarkType = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0002");
        }
    }
    #endregion

    #region Unit Rect type.
    /// <summary>   Unit Rect default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class UnitRectDefaultValue
    {
        public static bool DefaultValueLoaded = false;
        public static int ParamID;     // 0000
        public static string PLTFile;  // 0001
        public static int MarkType;    // 0002

        public static void LoadDefaultValue()
        {
            string markTypeCode = "0202";
            string groupCode = "4012";
            ParamID = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0000");
            PLTFile = MarkingQueryHelper.GetDefaultParamValueStr(markTypeCode, groupCode, "0001");
            MarkType = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0002");
        }
    }
    #endregion

    #region Unit Triangle type.
    /// <summary>   Unit Triangle default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class UnitTriDefaultValue
    {
        public static bool DefaultValueLoaded = false;
        public static int ParamID;     // 0000
        public static string PLTFile;  // 0001
        public static int MarkType;    // 0002

        public static void LoadDefaultValue()
        {
            string markTypeCode = "0203";
            string groupCode = "4013";
            ParamID = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0000");
            PLTFile = MarkingQueryHelper.GetDefaultParamValueStr(markTypeCode, groupCode, "0001");
            MarkType = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0002");
        }
    }
    #endregion

    #region Unit Special type.
    /// <summary>   Unit Special default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class UnitSpecialDefaultValue
    {
        public static bool DefaultValueLoaded = false;
        public static int ParamID;     // 0000
        public static string PLTFile;  // 0001
        public static int MarkType;    // 0002

        public static void LoadDefaultValue()
        {
            string markTypeCode = "0204";
            string groupCode = "4014";
            ParamID = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0000");
            PLTFile = MarkingQueryHelper.GetDefaultParamValueStr(markTypeCode, groupCode, "0001");
            MarkType = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0002");
        }
    }
    #endregion

    #region Number type.
    /// <summary>   Number default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class NumberDefaultValue
    {
        public static bool DefaultValueLoaded = false;
        public static int ParamID;     // 0000
        public static string PLTFile;  // 0001
        public static int MarkType;    // 0002

        public static void LoadDefaultValue()
        {
            string markTypeCode = "0301";
            string groupCode = "4021";
            ParamID = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0000");
            PLTFile = MarkingQueryHelper.GetDefaultParamValueStr(markTypeCode, groupCode, "0001");
            MarkType = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0002");
        }
    }
    #endregion

    #region Week type.
    /// <summary>   Week default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class WeekDefaultValue
    {
        public static bool DefaultValueLoaded = false;
        public static int ParamID;     // 0000
        public static string PLTFile;  // 0001
        public static int MarkType;    // 0002

        public static void LoadDefaultValue()
        {
            string markTypeCode = "0302";
            string groupCode = "4022";
            ParamID = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0000");
            PLTFile = MarkingQueryHelper.GetDefaultParamValueStr(markTypeCode, groupCode, "0001");
            MarkType = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0002");
        }
    }
    #endregion

    #region ID Mark type.
    /// <summary>   ID MArk default value.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public static class IDMarkDefaultValue
    {
        public static bool DefaultValueLoaded = false;
        public static int ParamID;     // 0000
        public static string PLTFile;  // 0001
        public static int MarkType;    // 0002

        public static void LoadDefaultValue()
        {
            string markTypeCode = "0303";
            string groupCode = "4023";
            ParamID = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0000");
            PLTFile = MarkingQueryHelper.GetDefaultParamValueStr(markTypeCode, groupCode, "0001");
            MarkType = MarkingQueryHelper.GetDefaultParamValueInt(markTypeCode, groupCode, "0002");
        }
    }
    #endregion
}
