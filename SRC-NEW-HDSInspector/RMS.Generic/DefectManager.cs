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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using Common.DataBase;

namespace RMS.Generic
{
    public class DefectManager
    {
        public static List<DefectGroup> DefectGroups = new List<DefectGroup>();

        public static List<int> AutoNGCodeList = new List<int>();

        public static void LoadDefectGroup()
        {
        }

        public static void UpdateAutoNGState()
        {
        }

        public static void LoadAutoNGCodeList()
        {
            AutoNGCodeList.Clear();

            // auto ng 1조회

            // 리스트 업데이트.
        }

        public static bool IsAutoNGState(int anResultType)
        {
            bool bIsAutoNG = false;

            foreach (int nCode in AutoNGCodeList)
            {
                if (nCode == anResultType)
                {
                    bIsAutoNG = true;
                    break;
                }
            }

            return bIsAutoNG;
        }

        public DefectManager()
        {
        }
    }

    public class DefectGroup
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public List<DefectItem> DefectItems { get; set; }

        public DefectGroup()
        {
            DefectItems = new List<DefectItem>();
        }
    }

    public class DefectItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool AutoNG { get; set; }
    }

    /// <summary>   Defect.  </summary>
    //public class Defect : BaseCode
    //{
    //    /// <summary>   Gets or sets the name of the defect group. </summary>
    //    /// <value> The name of the defect group. </value>
    //    public string DefectGroupName
    //    {
    //        get;
    //        set;
    //    }

    //    /// <summary>   Gets or sets the type of the inspect. </summary>
    //    /// <value> The type of the inspect. </value>
    //    public BaseCode InspectType
    //    {
    //        get 
    //        {
    //            if (m_InspectType == null)
    //            {
    //                m_InspectType = new BaseCode();
    //            }
    //            return m_InspectType; 
    //        }
    //        set
    //        {
    //            m_InspectType = value; 
    //        }
    //    }
    //    private BaseCode m_InspectType;
    //}

    /// <summary>   Manager for defects.  </summary>
    //public class DefectManager
    //{
    //    /// <summary>   Select defect information. </summary>
    //    /// <param name="strCode">  The string code. </param>
    //    public Defect SelectDefectInfo(string strCode)
    //    {
    //        Defect DefectIns = new Defect();

    //        if (ConnectFactory.DBConnector() != null)
    //        {
    //            string strQuery = string.Format("SELECT a.defect_type. a.defect_name, a.inspect_type, a.defect_group, b.com_dname FROM defect_info a, com_detail b WHERE a.defect_group = b.com_dcode and a.defect_type = '{0}' and b.com_mcode = '05' and a.use_yn = 1", strCode);

    //            IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery(strQuery);
    //            if (dataReader != null)
    //            {
    //                while (dataReader.Read())
    //                {
    //                    DefectIns.Code = dataReader.GetValue(0).ToString();
    //                    DefectIns.Name = dataReader.GetValue(1).ToString();
    //                    DefectIns.InspectType.Code = dataReader.GetValue(2).ToString();
    //                    DefectIns.MasterCode = dataReader.GetValue(3).ToString();
    //                    DefectIns.DefectGroupName = dataReader.GetValue(4).ToString();
    //                }
    //                dataReader.Close();
    //            }
    //        }

    //        return DefectIns;
    //    }
    //}
}
