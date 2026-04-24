using Common.DataBase;
using System;
using System.Data.Common;
using System.Drawing.Printing;

namespace Common.Drawing.InspectionInformation
{
    #region PSROdd
    public class PSROddProperty : InspectionAlgorithm
    {
        //Common
        public int ThreshType;                   // 0000

        //Core_RGB
        public int Core_Threshold;               //0060
        public int Core_ExceptionThreshold;      //0062
        public int Core_MinDefectSize;           //0063
         
        //Metal_채도
        public int Metal_LowerThreshold;         //0064
        public int Metal_UpperThreshold;         //0065
        public int Metal_MinDefectSize;          //0066

        //Circuit
        public int Circuit_Threshold;            //0067
        public int Circuit_MinDefectSize;        //0068

        // Nomal
        public int Summation_range;              //0069
        public int Summation_detection_size;     //0070
        public int Mask_Threshold;               //0071
        public int Mask_Extension;               //0072
        public int Step_Threshold;               //0073
        public int Step_Expansion;               //0074

        //필터
        public int HV_ratio_value;               //0075
        public int Min_Relative_size;            //0076
        public int Max_Relative_size;            //0077
        public int Area_Relative;                //0078

        public PSROddProperty()
        {
            Code = "3026";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
 
            //Core_RGB
            Core_Threshold = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0060"); 
            Core_ExceptionThreshold = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0062");       
            Core_MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0063");

            //Metal_채도
            Metal_LowerThreshold = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0064");
            Metal_UpperThreshold = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0065");          
            Metal_MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0066");

            //Circuit
            Circuit_Threshold = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0067");
            Circuit_MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0068");

                   
            // Nomal
            Summation_range = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0069");
            Summation_detection_size = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0070");
            Mask_Threshold = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0071");
            Mask_Extension = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0072");
            Step_Threshold = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0073");
            Step_Expansion = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0074");


            // 필터
            HV_ratio_value = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0075");
            Min_Relative_size = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0076");
            Max_Relative_size = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0077");
            Area_Relative = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0078");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            //UpdateProperty(strModelCode, strRoiCode, strInspectID);

            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery;

                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) 
                        return -1;

                    //Core_RGB
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0060", Core_Threshold);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) 
                        return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0062", Core_ExceptionThreshold);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) 
                        return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0063", Core_MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) 
                        return -1;


                    //Metal_채도
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0064", Metal_LowerThreshold);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) 
                        return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0065", Metal_UpperThreshold);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) 
                        return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0066", Metal_MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1)
                        return -1;


                    //Circuit
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0067", Circuit_Threshold);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) 
                        return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0068", Circuit_MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) 
                        return -1;


                    // Nomal
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0069", Summation_range);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) 
                        return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0070", Summation_detection_size);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) 
                        return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0071", Mask_Threshold);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) 
                        return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0072", Mask_Extension);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1)
                        return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0073", Step_Threshold);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1)
                        return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0074", Step_Expansion);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1)
                        return -1;


                    //필터
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0075", HV_ratio_value);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1)
                        return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0076", Min_Relative_size);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1)
                        return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0077", Max_Relative_size);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1)
                        return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0078", Area_Relative);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1)
                        return -1;

                }
                else return -1;
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }

        public override int UpdateProperty(string strModelCode, string strRoiCode, string strInspectID)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = "SET SQL_SAFE_UPDATES = 0; ";
                    strQuery += string.Format(
                    "UPDATE bgadb.roi_param " +
                    "SET param_value = " +
                    "   CASE param_code " +
                    "       WHEN '0000' THEN '{0}' " +
                    "       WHEN '0060' THEN '{1}' " +
                    "       WHEN '0062' THEN '{2}' " +
                    "       WHEN '0063' THEN '{3}' " +
                    "       WHEN '0064' THEN '{4}' " +
                    "       WHEN '0065' THEN '{5}' " +
                    "       WHEN '0066' THEN '{6}' " +
                    "       WHEN '0067' THEN '{7}' " +
                    "       WHEN '0068' THEN '{8}' " +
                    "       WHEN '0069' THEN '{9}' " +
                    "       WHEN '0070' THEN '{10}' " +
                    "       WHEN '0071' THEN '{11}' " +
                    "       WHEN '0072' THEN '{12}' " +
                    "       WHEN '0073' THEN '{13}' " +
                    "       WHEN '0074' THEN '{14}' " +
                    "       WHEN '0075' THEN '{15}' " +
                    "       WHEN '0076' THEN '{16}' " +
                    "       WHEN '0077' THEN '{17}' " +
                    "       WHEN '0078' THEN '{18}' " +
                    "       ELSE param_value " +
                    "   END " +
                    "WHERE model_code = '{19}' " +
                    "AND roi_code = '{20}' " +
                    "AND inspect_id = '{21}' " +
                    "AND param_code IN ('0000','0060', '0062', '0063', '0064', '0065', '0066','0067','0068','0069','0070','0071','0072', '0073','0074','0075', '0076','0077','0078') "
                    , ThreshType, 
                    Core_Threshold, Core_ExceptionThreshold, Core_MinDefectSize,
                    Metal_LowerThreshold, Metal_UpperThreshold, Metal_MinDefectSize,
                    Circuit_Threshold, Circuit_MinDefectSize, 
                    Summation_range, Summation_detection_size, Mask_Threshold, Mask_Extension, Step_Threshold, Step_Expansion, HV_ratio_value, Min_Relative_size, Max_Relative_size, Area_Relative
                    , strModelCode, strRoiCode, strInspectID);

                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) 
                        return -1;
                }
                else return -1;
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            PSROddProperty clonePSROddProperty = new PSROddProperty();

            //Common
            clonePSROddProperty.ThreshType = this.ThreshType;

            //Core_RGB
            clonePSROddProperty.Core_Threshold = this.Core_Threshold;
            clonePSROddProperty.Core_ExceptionThreshold = this.Core_ExceptionThreshold;
            clonePSROddProperty.Core_MinDefectSize = this.Core_MinDefectSize;

            //Metal_채도
            clonePSROddProperty.Metal_LowerThreshold = this.Metal_LowerThreshold;
            clonePSROddProperty.Metal_UpperThreshold = this.Metal_UpperThreshold;
            clonePSROddProperty.Metal_MinDefectSize = this.Metal_MinDefectSize;

            //Circuit
            clonePSROddProperty.Circuit_Threshold = this.Circuit_Threshold;
            clonePSROddProperty.Circuit_MinDefectSize = this.Circuit_MinDefectSize;

            // Nomal
            clonePSROddProperty.Summation_range = this.Summation_range;
            clonePSROddProperty.Summation_detection_size = this.Summation_detection_size;
            clonePSROddProperty.Mask_Threshold = this.Mask_Threshold;
            clonePSROddProperty.Mask_Extension = this.Mask_Extension;
            clonePSROddProperty.Step_Threshold = this.Step_Threshold;
            clonePSROddProperty.Step_Expansion = this.Step_Expansion;

            // 필터
            clonePSROddProperty.HV_ratio_value = this.HV_ratio_value;
            clonePSROddProperty.Min_Relative_size = this.Min_Relative_size;
            clonePSROddProperty.Max_Relative_size = this.Max_Relative_size;
            clonePSROddProperty.Area_Relative = this.Area_Relative;


            return clonePSROddProperty;
        }
    }
    #endregion

    #region ID Mark Type.
    /// <summary>   ID Mark property.  </summary>
    /// <remarks>   suoow2, 2014-10-09. </remarks>
    public class IDAreaProperty : InspectionAlgorithm
    {
        public int ThreshType; 
        public IDAreaProperty()
        {
            Code = "3024";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else return -1;
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }

        public override int UpdateProperty(string strModelCode, string strRoiCode, string strInspectID)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = "SET SQL_SAFE_UPDATES = 0; ";
                    strQuery += string.Format(
                    "UPDATE bgadb.roi_param " +
                    "SET param_value = " +
                    "   CASE param_code " +
                    "       WHEN '0000' THEN '{0}' " +
                    "       ELSE param_value " +
                    "   END " +
                    "WHERE model_code = '{1}' " +
                    "AND roi_code = '{2}' " +
                    "AND inspect_id = '{3}' " +
                    "AND param_code IN ('0000') "
                    , ThreshType
                    , strModelCode, strRoiCode, strInspectID);

                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else return -1;
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            IDAreaProperty cloneIDMarkProperty = new IDAreaProperty();

            cloneIDMarkProperty.ThreshType = this.ThreshType;

            return cloneIDMarkProperty;
        }
    }
    #endregion

    #region Fiducial Align Type.
    /// <summary>   Fiducial align property.  </summary>
    /// <remarks>   suoow2, 2014-10-09. </remarks>
    public class FiducialAlignProperty : InspectionAlgorithm
    {
        public int AlignMarginX;    // 0043
        public int AlignMarginY;    // 0044
        public int AlignAcceptance; // 0045

        public FiducialAlignProperty()
        {
            Code = "3001";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            AlignMarginX = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0043");
            AlignMarginY = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0044");
            AlignAcceptance = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0045");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0043", AlignMarginX);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0044", AlignMarginY);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0045", AlignAcceptance);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else return -1;
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }

        public override int UpdateProperty(string strModelCode, string strRoiCode, string strInspectID)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = "SET SQL_SAFE_UPDATES = 0; ";

                    strQuery += string.Format(
                    "UPDATE bgadb.roi_param " +
                    "SET param_value = " +
                    "   CASE param_code " +
                    "       WHEN '0043' THEN '{0}' " +
                    "       WHEN '0044' THEN '{1}' " +
                    "       WHEN '0045' THEN '{2}' " +
                    "       ELSE param_value " +
                    "   END " +
                    "WHERE model_code = '{3}' " +
                    "AND roi_code = '{4}' " +
                    "AND inspect_id = '{5}' " +
                    "AND param_code IN ('0043', '0044', '0045') "
                    , AlignMarginX, AlignMarginY, AlignAcceptance
                    , strModelCode, strRoiCode, strInspectID);

                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else return -1;
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            FiducialAlignProperty cloneFiducialAlignProperty = new FiducialAlignProperty();

            cloneFiducialAlignProperty.AlignMarginX = this.AlignMarginX;
            cloneFiducialAlignProperty.AlignMarginY = this.AlignMarginY;
            cloneFiducialAlignProperty.AlignAcceptance = this.AlignAcceptance;

            return cloneFiducialAlignProperty;
        }
    }
    #endregion

    #region Outer Fiducial Type.
    /// <summary>   Fiducial align property.  </summary>
    /// <remarks>   suoow2, 2014-10-09. </remarks>
    public class OuterFiducialProperty : InspectionAlgorithm
    {
        public int MarginX;    // 0043
        public int MarginY;    // 0044
        public int Acceptance; // 0045

        public OuterFiducialProperty()
        {
            Code = "3025";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            MarginX = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0043");
            MarginY = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0044");
            Acceptance = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0045");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0043", MarginX);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0044", MarginY);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0045", Acceptance);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else return -1;
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            OuterFiducialProperty cloneFiducialAlignProperty = new OuterFiducialProperty();

            cloneFiducialAlignProperty.MarginX = this.MarginX;
            cloneFiducialAlignProperty.MarginY = this.MarginY;
            cloneFiducialAlignProperty.Acceptance = this.Acceptance;

            return cloneFiducialAlignProperty;
        }
    }
    #endregion

    #region Lead Shape With Center Line Type.
    /// <summary>   Lead shape with center line property.  </summary>
    /// <remarks>   suoow2, 2014-10-11. </remarks>
    public class LeadShapeWithCenterLineProperty : InspectionAlgorithm
    {
        public int ThreshType;      // 0000
        public int LowerThresh;     // 0001
        public int UpperThresh;     // 0002
        public int MaskLowerThresh; // 0004
        public int MinDefectSize;   // 0015
        public int MinWidthRatio;   // 0030
        public int MinWidthSize;    // 0031
        public int MaxWidthRatio;   // 0032
        public int MaxWidthSize;    // 0033
        public int MinHeightsize;   // 0035
        public int MinNormalRatio;  // 0038
        public int MaxNormalRatio;  // 0039
        public int RemoveTipSize;   // 0099

        /// <summary>   Initializes a new instance of the ShapeShiftProperty class. </summary>
        /// <remarks>   suoow2, 2014-10-10. </remarks>
        public LeadShapeWithCenterLineProperty()
        {
            Code = "3008";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            MaskLowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0004");
            MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0015");
            MinWidthRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0030");
            MinWidthSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0031");
            MaxWidthRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0032");
            MaxWidthSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0033");
            MinHeightsize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0035");
            MinNormalRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0038");
            MaxNormalRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0039");
            RemoveTipSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0099");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002", UpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0004", MaskLowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0015", MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0030", MinWidthRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0031", MinWidthSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0032", MaxWidthRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0033", MaxWidthSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0035", MinHeightsize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0038", MinNormalRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0039", MaxNormalRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0099", RemoveTipSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        public override int UpdateProperty(string strModelCode, string strRoiCode, string strInspectID)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = "SET SQL_SAFE_UPDATES = 0; ";

                    strQuery += string.Format(
                        "UPDATE bgadb.roi_param " +
                        "SET param_value = " +
                        "   CASE param_code " +
                        "       WHEN '0000' THEN '{0}' " +
                        "       WHEN '0001' THEN '{1}' " +
                        "       WHEN '0002' THEN '{2}' " +
                        "       WHEN '0004' THEN '{3}' " +
                        "       WHEN '0015' THEN '{4}' " +
                        "       WHEN '0030' THEN '{5}' " +
                        "       WHEN '0031' THEN '{6}' " +
                        "       WHEN '0032' THEN '{7}' " +
                        "       WHEN '0033' THEN '{8}' " +
                        "       WHEN '0035' THEN '{9}' " +
                        "       WHEN '0038' THEN '{10}' " +
                        "       WHEN '0039' THEN '{11}' " +
                        "       WHEN '0099' THEN '{12}' " +
                        "       ELSE param_value " +
                        "   END " +
                        "WHERE model_code = '{13}' " +
                        "AND roi_code = '{14}' " +
                        "AND inspect_id = '{15}' " +
                        "AND param_code IN ('0000', '0001', '0002', '0004', '0015', '0030', '0031', '0032', '0033', '0035', '0038', '0039', '0099') "
                        , ThreshType, LowerThresh, UpperThresh, MaskLowerThresh, MinDefectSize, MinWidthRatio, MinWidthSize, MaxWidthRatio, MaxWidthSize, MinHeightsize, MinNormalRatio, MaxNormalRatio, RemoveTipSize
                        , strModelCode, strRoiCode, strInspectID);

                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            LeadShapeWithCenterLineProperty cloneLeadShapeWithCenterLineProperty = new LeadShapeWithCenterLineProperty();

            cloneLeadShapeWithCenterLineProperty.ThreshType = this.ThreshType;
            cloneLeadShapeWithCenterLineProperty.LowerThresh = this.LowerThresh;
            cloneLeadShapeWithCenterLineProperty.UpperThresh = this.UpperThresh;
            cloneLeadShapeWithCenterLineProperty.MinWidthRatio = this.MinWidthRatio;
            cloneLeadShapeWithCenterLineProperty.MinWidthSize = this.MinWidthSize;
            cloneLeadShapeWithCenterLineProperty.MaxWidthRatio = this.MaxWidthRatio;
            cloneLeadShapeWithCenterLineProperty.MaxWidthSize = this.MaxWidthSize;
            cloneLeadShapeWithCenterLineProperty.MinHeightsize = this.MinHeightsize;
            cloneLeadShapeWithCenterLineProperty.MinNormalRatio = this.MinNormalRatio;
            cloneLeadShapeWithCenterLineProperty.MaxNormalRatio = this.MaxNormalRatio;
            cloneLeadShapeWithCenterLineProperty.RemoveTipSize = this.RemoveTipSize;
            return cloneLeadShapeWithCenterLineProperty;
        }
    }
    #endregion

    #region Space Shape With Center Line Type.
    /// <summary>   Shape shape with center line property.  </summary>
    /// <remarks>   suoow2, 2012-03-30. </remarks>
    public class SpaceShapeWithCenterLineProperty : InspectionAlgorithm
    {
        public int ThreshType;      // 0000
        public int LowerThresh;     // 0001
        public int UpperThresh;     // 0002
        public int MinWidthRatio;   // 0030
        public int MinWidthSize;    // 0031
        public int MaxWidthRatio;   // 0032
        public int MaxWidthSize;    // 0033
        public int MinHeightsize;   // 0035
        public int MinNormalRatio;  // 0038
        public int MaxNormalRatio;  // 0039
        public int TipSearchSize;   // 0099
        public SpaceShapeWithCenterLineProperty()
        {
            Code = "3008";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            MinWidthRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0030");
            MinWidthSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0031");
            MaxWidthRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0032");
            MaxWidthSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0033");
            MinHeightsize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0035");
            MinNormalRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0038");
            MaxNormalRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0039");
            TipSearchSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0099");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002", UpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0030", MinWidthRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0031", MinWidthSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0032", MaxWidthRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0033", MaxWidthSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0035", MinHeightsize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0038", MinNormalRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0039", MaxNormalRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0099", TipSearchSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }

        public override int UpdateProperty(string strModelCode, string strRoiCode, string strInspectID)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = "SET SQL_SAFE_UPDATES = 0; ";

                    strQuery += string.Format(
                    "UPDATE bgadb.roi_param " +
                    "SET param_value = " +
                    "   CASE param_code " +
                    "       WHEN '0000' THEN '{0}' " +
                    "       WHEN '0001' THEN '{1}' " +
                    "       WHEN '0002' THEN '{2}' " +
                    "       WHEN '0030' THEN '{3}' " +
                    "       WHEN '0031' THEN '{4}' " +
                    "       WHEN '0032' THEN '{5}' " +
                    "       WHEN '0033' THEN '{6}' " +
                    "       WHEN '0035' THEN '{7}' " +
                    "       WHEN '0038' THEN '{8}' " +
                    "       WHEN '0039' THEN '{9}' " +
                    "       WHEN '0099' THEN '{10}' " +
                    "       ELSE param_value " +
                    "   END " +
                    "WHERE model_code = '{11}' " +
                    "AND roi_code = '{12}' " +
                    "AND inspect_id = '{13}' " +
                    "AND param_code IN ('0000', '0001', '0002', '0030', '0031', '0032', '0033', '0035', '0038', '0039', '0099') "
                    , ThreshType, LowerThresh, UpperThresh, MinWidthRatio, MinWidthSize, MaxWidthRatio, MaxWidthSize, MinHeightsize, MinNormalRatio, MaxNormalRatio, TipSearchSize
                    , strModelCode, strRoiCode, strInspectID);

                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            SpaceShapeWithCenterLineProperty cloneSpaceShapeWithCenterLineProperty = new SpaceShapeWithCenterLineProperty();

            cloneSpaceShapeWithCenterLineProperty.ThreshType = this.ThreshType;
            cloneSpaceShapeWithCenterLineProperty.LowerThresh = this.LowerThresh;
            cloneSpaceShapeWithCenterLineProperty.UpperThresh = this.UpperThresh;
            cloneSpaceShapeWithCenterLineProperty.MinWidthRatio = this.MinWidthRatio;
            cloneSpaceShapeWithCenterLineProperty.MinWidthSize = this.MinWidthSize;
            cloneSpaceShapeWithCenterLineProperty.MaxWidthRatio = this.MaxWidthRatio;
            cloneSpaceShapeWithCenterLineProperty.MaxWidthSize = this.MaxWidthSize;
            cloneSpaceShapeWithCenterLineProperty.MinHeightsize = this.MinHeightsize;
            cloneSpaceShapeWithCenterLineProperty.MinNormalRatio = this.MinNormalRatio;
            cloneSpaceShapeWithCenterLineProperty.MaxNormalRatio = this.MaxNormalRatio;
            cloneSpaceShapeWithCenterLineProperty.TipSearchSize = this.TipSearchSize;
            return cloneSpaceShapeWithCenterLineProperty;
        }
    }
    #endregion

    #region Lead Gap Type.
    public class LeadGapProperty : InspectionAlgorithm
    {
        public int LowerThresh;             // 0001
        public int UpperThresh;             // 0002
        public int MinWidthSize;            // 0031

        public LeadGapProperty()
        {
            Code = "3017";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            MinWidthSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0031");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001 ", LowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002 ", UpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0031 ", MinWidthSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            LeadGapProperty cloneLeadGap = new LeadGapProperty();

            cloneLeadGap.LowerThresh = this.LowerThresh;
            cloneLeadGap.UpperThresh = this.UpperThresh;
            cloneLeadGap.MinWidthSize = this.MinWidthSize;

            return cloneLeadGap;
        }
    }
    #endregion

    #region Groove With Center Line Type.
    /// <summary>   Groove property.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public class GrooveProperty : InspectionAlgorithm
    {
        public int ThreshType;              // 0000
        public int LowerThresh;             // 0001
        public int UpperThresh;             // 0002
        public int ApplyAverDiff;           // 0003
        public int AverMinMargin;           // 0007
        public int AverMaxMargin;           // 0008
        public int MinWidthRatio;           // 0030
        public int MinWidthSize;            // 0031
        public int MaxWidthRatio;           // 0032
        public int MaxWidthSize;            // 0033
        public int MinHeightSize;           // 0035
        public int MaxHeightRatio;          // 0036
        public int MaxHeightSize;           // 0037
        public int CriterionSize;           // 0048
        public int MinNormalRatio;          // 0038
        public int MaxNormalRatio;          // 0039

        public GrooveProperty()
        {
            Code = "3009";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            ApplyAverDiff = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0003");
            AverMinMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0007");
            AverMaxMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0008");
            MinWidthRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0030");
            MinWidthSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0031");
            MaxWidthRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0032");
            MaxWidthSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0033");
            MinHeightSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0035");
            MaxHeightRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0036");
            MaxHeightSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0037");
            CriterionSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0048");
            MinNormalRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0038");
            MaxNormalRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0039");

        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001 ", LowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002 ", UpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0003", ApplyAverDiff);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0007", AverMinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0008", AverMaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0030 ", MinWidthRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0031 ", MinWidthSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0032 ", MaxWidthRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0033 ", MaxWidthSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0035 ", MinHeightSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0036 ", MaxHeightRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0037 ", MaxHeightSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0048 ", CriterionSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0038 ", MinNormalRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0039 ", MaxNormalRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            GrooveProperty cloneGroove = new GrooveProperty();

            cloneGroove.ThreshType = this.ThreshType;
            cloneGroove.LowerThresh = this.LowerThresh;
            cloneGroove.UpperThresh = this.UpperThresh;
            cloneGroove.ApplyAverDiff = this.ApplyAverDiff;
            cloneGroove.AverMinMargin = this.AverMinMargin;
            cloneGroove.AverMaxMargin = this.AverMaxMargin;
            cloneGroove.MinWidthRatio = this.MinWidthRatio;
            cloneGroove.MinWidthSize = this.MinWidthSize;
            cloneGroove.MaxWidthRatio = this.MaxWidthRatio;
            cloneGroove.MaxWidthSize = this.MaxWidthSize;
            cloneGroove.MinHeightSize = this.MinHeightSize;
            cloneGroove.MaxHeightRatio = this.MaxHeightRatio;
            cloneGroove.MaxHeightSize = this.MaxHeightSize;
            cloneGroove.CriterionSize = this.CriterionSize;
            cloneGroove.MinNormalRatio = this.MinNormalRatio;
            cloneGroove.MaxNormalRatio = this.MaxNormalRatio;

            return cloneGroove;
        }
    }
    #endregion

    #region Figure Shape Type.
    /// <summary>   Figure shape property.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public class FigureShapeProperty : InspectionAlgorithm
    {
        public int InspRange;               // 0006
        public int AverMinMargin;           // 0007
        public int AverMaxMargin;           // 0008
        public int MinMargin;               // 0009
        public int MaxMargin;               // 0010
        public int ErosionTrainIter;        // 0011
        public int DilationTrainIter;       // 0012
        public int ErosionInspIter;         // 0013
        public int DilationInspIter;        // 0014
        public int MinWidthSize;            // 0031
        public int MinHeightSize;           // 0035
        public int MinNormalRatio;          // 0038
        public int MaxNormalRatio;          // 0039
        public int AlignMaxDist;            // 0046
        public int DarkAreaWidth;           // 0047

        public FigureShapeProperty()
        {
            Code = "3010";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            InspRange = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0006");
            AverMinMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0007");
            AverMaxMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0008");
            MinMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0009");
            MaxMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0010");
            ErosionTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0011");
            DilationTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0012");
            ErosionInspIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0013");
            DilationInspIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0014");
            MinWidthSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0031");
            MinHeightSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0035");
            MinNormalRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0038");
            MaxNormalRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0039");
            AlignMaxDist = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0046");
            DarkAreaWidth = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0047");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0006", InspRange);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0007", AverMinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0008", AverMaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0009", MinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0010", MaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0011", ErosionTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0012", DilationTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0013", ErosionInspIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0014", DilationInspIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0031", MinWidthSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0035", MinHeightSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0038", MinNormalRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0039", MaxNormalRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0046", AlignMaxDist);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0047", DarkAreaWidth);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            FigureShapeProperty cloneFigureShape = new FigureShapeProperty();

            cloneFigureShape.InspRange = this.InspRange;
            cloneFigureShape.AverMinMargin = this.AverMinMargin;
            cloneFigureShape.AverMaxMargin = this.AverMaxMargin;
            cloneFigureShape.MinMargin = this.MinMargin;
            cloneFigureShape.MaxMargin = this.MaxMargin;
            cloneFigureShape.ErosionTrainIter = this.ErosionTrainIter;
            cloneFigureShape.DilationTrainIter = this.DilationTrainIter;
            cloneFigureShape.ErosionInspIter = this.ErosionInspIter;
            cloneFigureShape.DilationInspIter = this.DilationInspIter;
            cloneFigureShape.MinWidthSize = this.MinWidthSize;
            cloneFigureShape.MinHeightSize = this.MinHeightSize;
            cloneFigureShape.MinNormalRatio = this.MinNormalRatio;
            cloneFigureShape.MaxNormalRatio = this.MaxNormalRatio;
            cloneFigureShape.AlignMaxDist = this.AlignMaxDist;
            cloneFigureShape.DarkAreaWidth = this.DarkAreaWidth;

            return cloneFigureShape;
        }
    }
    #endregion

    #region State Intensity Type.
    /// <summary>   State intensity property.  </summary>
    /// <remarks>   suoow2, 2014-10-20. </remarks>
    public class StateIntensityProperty : InspectionAlgorithm
    {
        public int ThreshType;              // 0000
        public int LowerThresh;             // 0001
        public int UpperThresh;             // 0002
        public int ApplyAverDiff;           // 0003
        public int InspRange;               // 0006
        public int AverMinMargin;           // 0007
        public int AverMaxMargin;           // 0008
        public int MinMargin;               // 0009
        public int MaxMargin;               // 0010
        public int ErosionTrainIter;        // 0011
        public int DilationTrainIter;       // 0012
        public int ErosionInspIter;         // 0013
        public int DilationInspIter;        // 0014
        public int MinDefectSize;           // 0015
        public int Invert;                  // 0020

        public StateIntensityProperty()
        {
            Code = "3011";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            ApplyAverDiff = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0003");
            InspRange = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0006");
            AverMinMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0007");
            AverMaxMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0008");
            MinMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0009");
            MaxMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0010");
            ErosionTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0011");
            DilationTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0012");
            ErosionInspIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0013");
            DilationInspIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0014");
            MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0015");
            Invert = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0020");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    if (ThreshType == 1 || ThreshType == 2) // LowerThreshold 값은 임계이하, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    if (ThreshType == 0 || ThreshType == 2) // Upperthreshold 값은 임계이상, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002", UpperThresh);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0003", ApplyAverDiff);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0006", InspRange);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0007", AverMinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0008", AverMaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0009", MinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0010", MaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0011", ErosionTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0012", DilationTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0013", ErosionInspIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0014", DilationInspIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0015", MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0020", Invert);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            StateIntensityProperty cloneStateIntensity = new StateIntensityProperty();

            cloneStateIntensity.ThreshType = this.ThreshType;
            cloneStateIntensity.LowerThresh = this.LowerThresh;
            cloneStateIntensity.UpperThresh = this.UpperThresh;
            cloneStateIntensity.ApplyAverDiff = this.ApplyAverDiff;
            cloneStateIntensity.InspRange = this.InspRange;
            cloneStateIntensity.AverMinMargin = this.AverMinMargin;
            cloneStateIntensity.AverMaxMargin = this.AverMaxMargin;
            cloneStateIntensity.MinMargin = this.MinMargin;
            cloneStateIntensity.MaxMargin = this.MaxMargin;
            cloneStateIntensity.ErosionTrainIter = this.ErosionTrainIter;
            cloneStateIntensity.DilationTrainIter = this.DilationTrainIter;
            cloneStateIntensity.ErosionInspIter = this.ErosionInspIter;
            cloneStateIntensity.DilationInspIter = this.DilationInspIter;
            cloneStateIntensity.MinDefectSize = this.MinDefectSize;
            cloneStateIntensity.Invert = this.Invert;

            return cloneStateIntensity;
        }
    }
    #endregion

    #region Window Punch Type.
    /// <summary>   WindowPunch property.  </summary>
    /// <remarks>   suoow2, 2016-01-20. </remarks>
    public class WindowPunchProperty : InspectionAlgorithm
    {
        public int ThreshType;      // 0000
        public int LowerThresh;     // 0001
        public int UpperThresh;     // 0002
        public int PsrMarginX;               // 0049
        public int PsrMarginY;               // 0050
        public int MinDefectSize;   // 0015
        public int MinHeightsize;   // 0035
        public int UsePSRShift;       //0048 //////인식키 검사여부
        public int UsePunchShift;       //0052 //////인식키 검사여부
        public int ErosionTrainIter; //0011

        public WindowPunchProperty()
        {
            Code = "3003";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            ErosionTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0011");
            UsePSRShift = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0048");
            PsrMarginX = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0049");
            PsrMarginY = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0050");
            UsePunchShift = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0052");
            MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0015");
            MinHeightsize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0035");

        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002", UpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0011", ErosionTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0048", UsePSRShift);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0049", PsrMarginX);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0050", PsrMarginY);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0052", UsePunchShift);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0015", MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0035", MinHeightsize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            WindowPunchProperty cloneWindowPunchProperty = new WindowPunchProperty();

            cloneWindowPunchProperty.ThreshType = this.ThreshType;
            cloneWindowPunchProperty.LowerThresh = this.LowerThresh;
            cloneWindowPunchProperty.UpperThresh = this.UpperThresh;
            cloneWindowPunchProperty.UsePSRShift = this.UsePSRShift;
            cloneWindowPunchProperty.PsrMarginX = this.PsrMarginX;
            cloneWindowPunchProperty.PsrMarginY = this.PsrMarginY;
            cloneWindowPunchProperty.UsePunchShift = this.UsePunchShift;
            cloneWindowPunchProperty.MinDefectSize = this.MinDefectSize;
            cloneWindowPunchProperty.MinHeightsize = this.MinHeightsize;
            cloneWindowPunchProperty.ErosionTrainIter = this.ErosionTrainIter;
            return cloneWindowPunchProperty;
        }
    }
    #endregion

    #region PatternUnit Type
    /// <summary>   PatternUnit property.  </summary>
    /// <remarks>   suoow2, 2017-07-10. </remarks>
    ///  public int LowerThresh;     // 0001
    public class UnitPatternProperty : InspectionAlgorithm
    {
        public int LowerThresh;     // 0001
        public int MinDefectSize;   // 0015
        public UnitPatternProperty()
        {
            Code = "3022";
        }
        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");         
            MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0015");

        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0015", MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            UnitPatternProperty cloneUnitPatternProperty = new UnitPatternProperty();

            cloneUnitPatternProperty.LowerThresh = this.LowerThresh;
            cloneUnitPatternProperty.MinDefectSize = this.MinDefectSize;
            return cloneUnitPatternProperty;
        }
    }
    #endregion

    #region CrossPatern Type.
    /// <summary>   CrossPattern property.  </summary>
    /// <remarks>   suoow2, 2016-01-20. </remarks>
    public class CrossPatternProperty : InspectionAlgorithm
    {
        public int ThreshType;      // 0000
        public int LowerThresh;     // 0001
        public int UpperThresh;     // 0002
        public int PsrMarginX;               // 0049
        public int PsrMarginY;               // 0050
        public int MinDefectSize;   // 0015
        public int MinHeightsize;   // 0035
        public int UsePunchShift;       //0052 //인식키 검사여부
        public int UsePSRShift;         //0048 //인식키 검사여부
        public int UsePSRShiftBA;       //0073 //PSR Shift BA면 검사여부

        public CrossPatternProperty()
        {
            Code = "3003";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            UsePSRShift = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0048");
            UsePSRShiftBA = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0073");  // jiwon
            PsrMarginX = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0049");
            PsrMarginY = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0050");
            UsePunchShift = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0052");
            MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0015");
            MinHeightsize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0035");

        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002", UpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0048", UsePSRShift);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0073", UsePSRShiftBA);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0049", PsrMarginX);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0050", PsrMarginY);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0052", UsePunchShift);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0015", MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0035", MinHeightsize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }

        public override int UpdateProperty(string strModelCode, string strRoiCode, string strInspectID)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = "SET SQL_SAFE_UPDATES = 0; ";

                    strQuery += string.Format(
                    "UPDATE bgadb.roi_param " +
                    "SET param_value = " +
                    "   CASE param_code " +
                    "       WHEN '0000' THEN '{0}' " +
                    "       WHEN '0001' THEN '{1}' " +
                    "       WHEN '0002' THEN '{2}' " +
                    "       WHEN '0048' THEN '{3}' " +
                    "       WHEN '0073' THEN '{4}' " +          // jiwon DB 변경
                    "       WHEN '0049' THEN '{5}' " +
                    "       WHEN '0050' THEN '{6}' " +
                    "       WHEN '0052' THEN '{7}' " +
                    "       WHEN '0015' THEN '{8}' " +
                    "       WHEN '0035' THEN '{9}' " +
                    "       ELSE param_value " +
                    "   END " +
                    "WHERE model_code = '{10}' " +
                    "AND roi_code = '{11}' " +
                    "AND inspect_id = '{12}' " +
                    "AND param_code IN ('0000', '0001', '0002', '0048', '0073', '0049', '0050', '0052', '0015', '0035') "
                    , ThreshType, LowerThresh, UpperThresh, UsePSRShift, UsePSRShiftBA, PsrMarginX, PsrMarginY, UsePunchShift, MinDefectSize, MinHeightsize
                    , strModelCode, strRoiCode, strInspectID);

                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            CrossPatternProperty cloneCrossPatternProperty = new CrossPatternProperty();

            cloneCrossPatternProperty.ThreshType = this.ThreshType;
            cloneCrossPatternProperty.LowerThresh = this.LowerThresh;
            cloneCrossPatternProperty.UpperThresh = this.UpperThresh;
            cloneCrossPatternProperty.UsePSRShift = this.UsePSRShift;
            cloneCrossPatternProperty.UsePSRShiftBA = this.UsePSRShiftBA;
            cloneCrossPatternProperty.PsrMarginX = this.PsrMarginX;
            cloneCrossPatternProperty.PsrMarginY = this.PsrMarginY;
            cloneCrossPatternProperty.UsePunchShift = this.UsePunchShift;
            cloneCrossPatternProperty.MinDefectSize = this.MinDefectSize;
            cloneCrossPatternProperty.MinHeightsize = this.MinHeightsize;
            return cloneCrossPatternProperty;
        }
    }
    #endregion

    #region RawMetrial Type.
    /// <summary>   State aligned mask property.  </summary>
    /// <remarks>   suoow2, 2014-10-11. </remarks>
    public class RawMetrialProperty : InspectionAlgorithm
    {
        public int ThreshType;              // 0000
        public int LowerThresh;             // 0001
        public int UpperThresh;             // 0002
        public int ApplyAverDiff;           // 0003
        public int MaskLowerThresh;         // 0004
        public int MaskUpperThresh;         // 0005
        public int InspRange;               // 0006
        public int AverMinMargin;           // 0007
        public int AverMaxMargin;           // 0008
        public int MinMargin;               // 0009
        public int MaxMargin;               // 0010
        public int ErosionTrainIter = 0;    // 0011
        public int DilationTrainIter = 0;   // 0012
        public int ErosionInspIter = 0;     // 0013
        public int DilationInspIter = 0;    // 0014
        public int MinDefectSize;           // 0015
        public int MinSmallDefectSize;      // 0016
        public int MinSmallDefectCount;     // 0017
        public int MaxDefectSize;           // 0064
        public int MaxSmallDefectSize;      // 0065
        public int MaxSmallDefectCount;     // 0066
        public int SumDefectSize;           // 0041
        public int SameValue;

        public RawMetrialProperty()
        {
            Code = "3020";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            ApplyAverDiff = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0003");
            MaskLowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0004");
            MaskUpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0005");
            InspRange = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0006");
            AverMinMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0007");
            AverMaxMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0008");
            MinMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0009");
            MaxMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0010");
            ErosionTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0011");
            DilationTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0012");
            ErosionInspIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0013");
            DilationInspIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0014");
            MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0015");
            MinSmallDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0016");
            MinSmallDefectCount = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0017");
            MaxDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0064");
            MaxSmallDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0065");
            MaxSmallDefectCount = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0066");
            SumDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0041");
            // SameValue = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0999");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    if (ThreshType == 0 || ThreshType == 2) // LowerThreshold 값은 임계이상, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    if (ThreshType == 1 || ThreshType == 2) // Upperthreshold 값은 임계이하, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002", UpperThresh);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0003", ApplyAverDiff);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0004", MaskLowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0005", MaskUpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0006", InspRange);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0007", AverMinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0008", AverMaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0009", MinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0010", MaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0011", ErosionTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0012", DilationTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0013", ErosionInspIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0014", DilationInspIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0015", MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0016", MinSmallDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0017", MinSmallDefectCount);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0064", MaxDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0065", MaxSmallDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0066", MaxSmallDefectCount);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0041", SumDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    // strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0999", SameValue);
                    //  if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        public override int UpdateProperty(string strModelCode, string strRoiCode, string strInspectID)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = "SET SQL_SAFE_UPDATES = 0; ";
                    if (ThreshType == 0)
                    {
                        strQuery +=
                        string.Format(
                                    "UPDATE bgadb.roi_param " +
                                    "SET param_value = " +
                                    "   CASE param_code " +
                                    "       WHEN '0000' THEN '{0}' " +
                                    "       WHEN '0001' THEN '{1}' " +
                                    "       WHEN '0003' THEN '{2}' " +
                                    "       WHEN '0004' THEN '{3}' " +
                                    "       WHEN '0005' THEN '{4}' " +
                                    "       WHEN '0006' THEN '{5}' " +
                                    "       WHEN '0007' THEN '{6}' " +
                                    "       WHEN '0008' THEN '{7}' " +
                                    "       WHEN '0009' THEN '{8}' " +
                                    "       WHEN '0010' THEN '{9}' " +
                                    "       WHEN '0011' THEN '{10}' " +
                                    "       WHEN '0012' THEN '{11}' " +
                                    "       WHEN '0013' THEN '{12}' " +
                                    "       WHEN '0014' THEN '{13}' " +
                                    "       WHEN '0015' THEN '{14}' " +
                                    "       WHEN '0016' THEN '{15}' " +
                                    "       WHEN '0017' THEN '{16}' " +
                                    "       WHEN '0064' THEN '{17}' " +
                                    "       WHEN '0065' THEN '{18}' " +
                                    "       WHEN '0066' THEN '{19}' " +
                                    "       WHEN '0041' THEN '{20}' " +
                                    "       ELSE param_value " +
                                    "   END " +
                                    "WHERE model_code = '{21}' " +
                                    "AND roi_code = '{22}' " +
                                    "AND inspect_id = '{23}' " +
                                    "AND param_code IN ('0000', '0001', '0003', '0004', '0005', '0006', '0007', '0008', '0009', '0010', '0011', '0012', '0013', '0014', '0015', '0016', '0017', '0064', '0065', '0066', '0041') "
                                    , ThreshType, LowerThresh, ApplyAverDiff, MaskLowerThresh, MaskUpperThresh, InspRange, AverMinMargin, AverMaxMargin, MinMargin, MaxMargin, ErosionTrainIter
                                    , DilationTrainIter, ErosionInspIter, DilationInspIter, MinDefectSize, MinSmallDefectSize, MinSmallDefectCount, MaxDefectSize, MaxSmallDefectSize, MaxSmallDefectCount, SumDefectSize
                                    , strModelCode, strRoiCode, strInspectID);
                    }
                    else if (ThreshType == 1)
                    {
                        strQuery +=
                        string.Format(
                                    "UPDATE bgadb.roi_param " +
                                    "SET param_value = " +
                                    "   CASE param_code " +
                                    "       WHEN '0000' THEN '{0}' " +
                                    "       WHEN '0002' THEN '{1}' " +
                                    "       WHEN '0003' THEN '{2}' " +
                                    "       WHEN '0004' THEN '{3}' " +
                                    "       WHEN '0005' THEN '{4}' " +
                                    "       WHEN '0006' THEN '{5}' " +
                                    "       WHEN '0007' THEN '{6}' " +
                                    "       WHEN '0008' THEN '{7}' " +
                                    "       WHEN '0009' THEN '{8}' " +
                                    "       WHEN '0010' THEN '{9}' " +
                                    "       WHEN '0011' THEN '{10}' " +
                                    "       WHEN '0012' THEN '{11}' " +
                                    "       WHEN '0013' THEN '{12}' " +
                                    "       WHEN '0014' THEN '{13}' " +
                                    "       WHEN '0015' THEN '{14}' " +
                                    "       WHEN '0016' THEN '{15}' " +
                                    "       WHEN '0017' THEN '{16}' " +
                                    "       WHEN '0064' THEN '{17}' " +
                                    "       WHEN '0065' THEN '{18}' " +
                                    "       WHEN '0066' THEN '{19}' " +
                                    "       WHEN '0041' THEN '{20}' " +
                                    "       ELSE param_value " +
                                    "   END " +
                                    "WHERE model_code = '{21}' " +
                                    "AND roi_code = '{22}' " +
                                    "AND inspect_id = '{23}' " +
                                    "AND param_code IN ('0000', '0002', '0003', '0004', '0005', '0006', '0007', '0008', '0009', '0010', '0011', '0012', '0013', '0014', '0015', '0016', '0017', '0064', '0065', '0066', '0041') "
                                    , ThreshType, UpperThresh, ApplyAverDiff, MaskLowerThresh, MaskUpperThresh, InspRange, AverMinMargin, AverMaxMargin, MinMargin, MaxMargin, ErosionTrainIter
                                    , DilationTrainIter, ErosionInspIter, DilationInspIter, MinDefectSize, MinSmallDefectSize, MinSmallDefectCount, MaxDefectSize, MaxSmallDefectSize, MaxSmallDefectCount, SumDefectSize
                                    , strModelCode, strRoiCode, strInspectID);
                    }
                    else if (ThreshType == 2)
                    {
                        strQuery +=
                        string.Format(
                                    "UPDATE bgadb.roi_param " +
                                    "SET param_value = " +
                                    "   CASE param_code " +
                                    "       WHEN '0000' THEN '{0}' " +
                                    "       WHEN '0001' THEN '{1}' " +
                                    "       WHEN '0002' THEN '{2}' " +
                                    "       WHEN '0003' THEN '{3}' " +
                                    "       WHEN '0004' THEN '{4}' " +
                                    "       WHEN '0005' THEN '{5}' " +
                                    "       WHEN '0006' THEN '{6}' " +
                                    "       WHEN '0007' THEN '{7}' " +
                                    "       WHEN '0008' THEN '{8}' " +
                                    "       WHEN '0009' THEN '{9}' " +
                                    "       WHEN '0010' THEN '{10}' " +
                                    "       WHEN '0011' THEN '{11}' " +
                                    "       WHEN '0012' THEN '{12}' " +
                                    "       WHEN '0013' THEN '{13}' " +
                                    "       WHEN '0014' THEN '{14}' " +
                                    "       WHEN '0015' THEN '{15}' " +
                                    "       WHEN '0016' THEN '{16}' " +
                                    "       WHEN '0017' THEN '{17}' " +
                                    "       WHEN '0064' THEN '{18}' " +
                                    "       WHEN '0065' THEN '{19}' " +
                                    "       WHEN '0066' THEN '{20}' " +
                                    "       WHEN '0041' THEN '{21}' " +
                                    "       ELSE param_value " +
                                    "   END " +
                                    "WHERE model_code = '{22}' " +
                                    "AND roi_code = '{23}' " +
                                    "AND inspect_id = '{24}' " +
                                    "AND param_code IN ('0000', '0001', '0002', '0003', '0004', '0005', '0006', '0007', '0008', '0009', '0010', '0011', '0012', '0013', '0014', '0015', '0016', '0017', '0064', '0065', '0066', '0041') "
                                    , ThreshType, LowerThresh, UpperThresh, ApplyAverDiff, MaskLowerThresh, MaskUpperThresh, InspRange, AverMinMargin, AverMaxMargin, MinMargin, MaxMargin, ErosionTrainIter
                                    , DilationTrainIter, ErosionInspIter, DilationInspIter, MinDefectSize, MinSmallDefectSize, MinSmallDefectCount, MaxDefectSize, MaxSmallDefectSize, MaxSmallDefectCount, SumDefectSize
                                    , strModelCode, strRoiCode, strInspectID);
                    }

                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            RawMetrialProperty cloneRawMetrialMask = new RawMetrialProperty();

            cloneRawMetrialMask.ThreshType = this.ThreshType;
            cloneRawMetrialMask.LowerThresh = this.LowerThresh;
            cloneRawMetrialMask.UpperThresh = this.UpperThresh;
            cloneRawMetrialMask.ApplyAverDiff = this.ApplyAverDiff;
            cloneRawMetrialMask.MaskLowerThresh = this.MaskLowerThresh;
            cloneRawMetrialMask.MaskUpperThresh = this.MaskUpperThresh;
            cloneRawMetrialMask.InspRange = this.InspRange;
            cloneRawMetrialMask.AverMinMargin = this.AverMinMargin;
            cloneRawMetrialMask.AverMaxMargin = this.AverMaxMargin;
            cloneRawMetrialMask.MinMargin = this.MinMargin;
            cloneRawMetrialMask.MaxMargin = this.MaxMargin;
            cloneRawMetrialMask.ErosionTrainIter = this.ErosionTrainIter;
            cloneRawMetrialMask.DilationTrainIter = this.DilationTrainIter;
            cloneRawMetrialMask.ErosionInspIter = this.ErosionInspIter;
            cloneRawMetrialMask.DilationInspIter = this.DilationInspIter;
            cloneRawMetrialMask.MinDefectSize = this.MinDefectSize;
            cloneRawMetrialMask.MinSmallDefectSize = this.MinSmallDefectSize;
            cloneRawMetrialMask.MinSmallDefectCount = this.MinSmallDefectCount;
            cloneRawMetrialMask.MaxDefectSize = this.MaxDefectSize;
            cloneRawMetrialMask.MaxSmallDefectSize = this.MaxSmallDefectSize;
            cloneRawMetrialMask.MaxSmallDefectCount = this.MaxSmallDefectCount;
            cloneRawMetrialMask.SumDefectSize = this.SumDefectSize;
            cloneRawMetrialMask.SameValue = this.SameValue;

            return cloneRawMetrialMask;
        }
    }
    #endregion

    #region State Aligned Mask Type.
    /// <summary>   State aligned mask property.  </summary>
    /// <remarks>   suoow2, 2014-10-11. </remarks>
    public class StateAlignedMaskProperty : InspectionAlgorithm
    {
        public int ThreshType;              // 0000
        public int LowerThresh;             // 0001
        public int UpperThresh;             // 0002
        public int ApplyAverDiff;           // 0003
        public int MaskLowerThresh;         // 0004
        public int MaskUpperThresh;         // 0005
        public int InspRange;               // 0006
        public int AverMinMargin;           // 0007
        public int AverMaxMargin;           // 0008
        public int MinMargin;               // 0009
        public int MaxMargin;               // 0010
        public int ErosionTrainIter = 0;    // 0011
        public int DilationTrainIter = 0;   // 0012
        public int ErosionInspIter = 0;     // 0013
        public int DilationInspIter = 0;    // 0014
        public int MinDefectSize;           // 0015
        public int MinSmallDefectSize;      // 0016
        public int MinSmallDefectCount;     // 0017
        public int MaxDefectSize;           // 0064
        public int MaxSmallDefectSize;      // 0065
        public int MaxSmallDefectCount;     // 0066
        public int SameValue;               //체크 박스만 컨트롤

        public StateAlignedMaskProperty()
        {
            Code = "3012";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            ApplyAverDiff = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0003");
            MaskLowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0004");
            MaskUpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0005");
            InspRange = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0006");
            AverMinMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0007");
            AverMaxMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0008");
            MinMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0009");
            MaxMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0010");
            ErosionTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0011");
            DilationTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0012");
            ErosionInspIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0013");
            DilationInspIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0014");
            MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0015");
            MinSmallDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0016");
            MinSmallDefectCount = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0017");
            MaxDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0064");
            MaxSmallDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0065");
            MaxSmallDefectCount = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0066");
            SameValue = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0999");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    if (ThreshType == 0 || ThreshType == 2) // LowerThreshold 값은 임계이상, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    if (ThreshType == 1 || ThreshType == 2) // Upperthreshold 값은 임계이하, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002", UpperThresh);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0003", ApplyAverDiff);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0004", MaskLowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0005", MaskUpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0006", InspRange);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0007", AverMinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0008", AverMaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0009", MinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0010", MaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0011", ErosionTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0012", DilationTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0013", ErosionInspIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0014", DilationInspIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0015", MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0016", MinSmallDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0017", MinSmallDefectCount);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0064", MaxDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0065", MaxSmallDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0066", MaxSmallDefectCount);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0999", SameValue);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }

        public override int UpdateProperty(string strModelCode, string strRoiCode, string strInspectID)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = "SET SQL_SAFE_UPDATES = 0; ";

                    if (ThreshType == 0)
                    {            
                        strQuery +=
                        string.Format(
                                    "UPDATE bgadb.roi_param " +
                                    "SET param_value = " +
                                    "   CASE param_code " +
                                    "       WHEN '0000' THEN '{0}' " +
                                    "       WHEN '0001' THEN '{1}' " +
                                    "       WHEN '0003' THEN '{2}' " +
                                    "       WHEN '0004' THEN '{3}' " +
                                    "       WHEN '0005' THEN '{4}' " +
                                    "       WHEN '0006' THEN '{5}' " +
                                    "       WHEN '0007' THEN '{6}' " +
                                    "       WHEN '0008' THEN '{7}' " +
                                    "       WHEN '0009' THEN '{8}' " +
                                    "       WHEN '0010' THEN '{9}' " +
                                    "       WHEN '0011' THEN '{10}' " +
                                    "       WHEN '0012' THEN '{11}' " +
                                    "       WHEN '0013' THEN '{12}' " +
                                    "       WHEN '0014' THEN '{13}' " +
                                    "       WHEN '0015' THEN '{14}' " +
                                    "       WHEN '0016' THEN '{15}' " +
                                    "       WHEN '0017' THEN '{16}' " +
                                    "       WHEN '0064' THEN '{17}' " +
                                    "       WHEN '0065' THEN '{18}' " +
                                    "       WHEN '0066' THEN '{19}' " +
                                    "       WHEN '0999' THEN '{20}' " +
                                    "       ELSE param_value " +
                                    "   END " +
                                    "WHERE model_code = '{21}' " +
                                    "AND roi_code = '{22}' " +
                                    "AND inspect_id = '{23}' " +
                                    "AND param_code IN ('0000', '0001', '0003', '0004', '0005', '0006', '0007', '0008', '0009', '0010', '0011', '0012', '0013', '0014', '0015', '0016', '0017', '0064', '0065', '0066', '0999') "
                                    , ThreshType, LowerThresh, ApplyAverDiff, MaskLowerThresh, MaskUpperThresh, InspRange, AverMinMargin, AverMaxMargin, MinMargin, MaxMargin, ErosionTrainIter
                                    , DilationTrainIter, ErosionInspIter, DilationInspIter, MinDefectSize, MinSmallDefectSize, MinSmallDefectCount, MaxDefectSize, MaxSmallDefectSize, MaxSmallDefectCount, SameValue
                                    , strModelCode, strRoiCode, strInspectID);
                    }
                    else if (ThreshType == 1)
                    {
                        strQuery +=
                        string.Format(
                                    "UPDATE bgadb.roi_param " +
                                    "SET param_value = " +
                                    "   CASE param_code " +
                                    "       WHEN '0000' THEN '{0}' " +
                                    "       WHEN '0002' THEN '{1}' " +
                                    "       WHEN '0003' THEN '{2}' " +
                                    "       WHEN '0004' THEN '{3}' " +
                                    "       WHEN '0005' THEN '{4}' " +
                                    "       WHEN '0006' THEN '{5}' " +
                                    "       WHEN '0007' THEN '{6}' " +
                                    "       WHEN '0008' THEN '{7}' " +
                                    "       WHEN '0009' THEN '{8}' " +
                                    "       WHEN '0010' THEN '{9}' " +
                                    "       WHEN '0011' THEN '{10}' " +
                                    "       WHEN '0012' THEN '{11}' " +
                                    "       WHEN '0013' THEN '{12}' " +
                                    "       WHEN '0014' THEN '{13}' " +
                                    "       WHEN '0015' THEN '{14}' " +
                                    "       WHEN '0016' THEN '{15}' " +
                                    "       WHEN '0017' THEN '{16}' " +
                                    "       WHEN '0064' THEN '{17}' " +
                                    "       WHEN '0065' THEN '{18}' " +
                                    "       WHEN '0066' THEN '{19}' " +
                                    "       WHEN '0999' THEN '{20}' " +
                                    "       ELSE param_value " +
                                    "   END " +
                                    "WHERE model_code = '{21}' " +
                                    "AND roi_code = '{22}' " +
                                    "AND inspect_id = '{23}' " +
                                    "AND param_code IN ('0000', '0002', '0003', '0004', '0005', '0006', '0007', '0008', '0009', '0010', '0011', '0012', '0013', '0014', '0015', '0016', '0017', '0064', '0065', '0066', '0999') "
                                    , ThreshType, UpperThresh, ApplyAverDiff, MaskLowerThresh, MaskUpperThresh, InspRange, AverMinMargin, AverMaxMargin, MinMargin, MaxMargin, ErosionTrainIter
                                    , DilationTrainIter, ErosionInspIter, DilationInspIter, MinDefectSize, MinSmallDefectSize, MinSmallDefectCount, MaxDefectSize, MaxSmallDefectSize, MaxSmallDefectCount, SameValue
                                    , strModelCode, strRoiCode, strInspectID);
                    }
                    else if (ThreshType == 2)
                    {
                        strQuery +=
                        string.Format(
                                    "UPDATE bgadb.roi_param " +
                                    "SET param_value = " +
                                    "   CASE param_code " +
                                    "       WHEN '0000' THEN '{0}' " +
                                    "       WHEN '0001' THEN '{1}' " +
                                    "       WHEN '0002' THEN '{2}' " +
                                    "       WHEN '0003' THEN '{3}' " +
                                    "       WHEN '0004' THEN '{4}' " +
                                    "       WHEN '0005' THEN '{5}' " +
                                    "       WHEN '0006' THEN '{6}' " +
                                    "       WHEN '0007' THEN '{7}' " +
                                    "       WHEN '0008' THEN '{8}' " +
                                    "       WHEN '0009' THEN '{9}' " +
                                    "       WHEN '0010' THEN '{10}' " +
                                    "       WHEN '0011' THEN '{11}' " +
                                    "       WHEN '0012' THEN '{12}' " +
                                    "       WHEN '0013' THEN '{13}' " +
                                    "       WHEN '0014' THEN '{14}' " +
                                    "       WHEN '0015' THEN '{15}' " +
                                    "       WHEN '0016' THEN '{16}' " +
                                    "       WHEN '0017' THEN '{17}' " +
                                    "       WHEN '0064' THEN '{18}' " +
                                    "       WHEN '0065' THEN '{19}' " +
                                    "       WHEN '0066' THEN '{20}' " +
                                    "       WHEN '0999' THEN '{21}' " +
                                    "       ELSE param_value " +
                                    "   END " +
                                    "WHERE model_code = '{22}' " +
                                    "AND roi_code = '{23}' " +
                                    "AND inspect_id = '{24}' " +
                                    "AND param_code IN ('0000', '0001', '0002', '0003', '0004', '0005', '0006', '0007', '0008', '0009', '0010', '0011', '0012', '0013', '0014', '0015', '0016', '0017', '0064', '0065', '0066', '0999') "
                                    , ThreshType, LowerThresh, UpperThresh, ApplyAverDiff, MaskLowerThresh, MaskUpperThresh, InspRange, AverMinMargin, AverMaxMargin, MinMargin, MaxMargin, ErosionTrainIter
                                    , DilationTrainIter, ErosionInspIter, DilationInspIter, MinDefectSize, MinSmallDefectSize, MinSmallDefectCount, MaxDefectSize, MaxSmallDefectSize, MaxSmallDefectCount, SameValue
                                    , strModelCode, strRoiCode, strInspectID);
                    }

                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            StateAlignedMaskProperty cloneStateAlignedMask = new StateAlignedMaskProperty();

            cloneStateAlignedMask.ThreshType = this.ThreshType;
            cloneStateAlignedMask.LowerThresh = this.LowerThresh;
            cloneStateAlignedMask.UpperThresh = this.UpperThresh;
            cloneStateAlignedMask.ApplyAverDiff = this.ApplyAverDiff;
            cloneStateAlignedMask.MaskLowerThresh = this.MaskLowerThresh;
            cloneStateAlignedMask.MaskUpperThresh = this.MaskUpperThresh;
            cloneStateAlignedMask.InspRange = this.InspRange;
            cloneStateAlignedMask.AverMinMargin = this.AverMinMargin;
            cloneStateAlignedMask.AverMaxMargin = this.AverMaxMargin;
            cloneStateAlignedMask.MinMargin = this.MinMargin;
            cloneStateAlignedMask.MaxMargin = this.MaxMargin;
            cloneStateAlignedMask.ErosionTrainIter = this.ErosionTrainIter;
            cloneStateAlignedMask.DilationTrainIter = this.DilationTrainIter;
            cloneStateAlignedMask.ErosionInspIter = this.ErosionInspIter;
            cloneStateAlignedMask.DilationInspIter = this.DilationInspIter;
            cloneStateAlignedMask.MinDefectSize = this.MinDefectSize;
            cloneStateAlignedMask.MinSmallDefectSize = this.MinSmallDefectSize;
            cloneStateAlignedMask.MinSmallDefectCount = this.MinSmallDefectCount;
            cloneStateAlignedMask.MaxDefectSize = this.MaxDefectSize;
            cloneStateAlignedMask.MaxSmallDefectSize = this.MaxSmallDefectSize;
            cloneStateAlignedMask.MaxSmallDefectCount = this.MaxSmallDefectCount;
            cloneStateAlignedMask.SameValue = this.SameValue;

            return cloneStateAlignedMask;
        }
    }
    #endregion

    #region Shape Shift Type.
    /// <summary>   Shape shift property.  </summary>
    /// <remarks>   suoow2, 2014-10-10. </remarks>
    public class ShapeShiftProperty : InspectionAlgorithm
    {
        public int ThreshType;              // 0000
        public int LowerThresh;             // 0001
        public int UpperThresh;             // 0002
        public int ApplyAverDiff;           // 0003
        public int InspRange;               // 0006
        public int AverMinMargin;           // 0007
        public int AverMaxMargin;           // 0008
        public int MinMargin;               // 0009
        public int MaxMargin;               // 0010
        public int ErosionTrainIter;        // 0011
        public int DilationTrainIter;       // 0012
        public int MinDefectSize;           // 0015
        public int ErosionShapeIter;        // 0021  
        public int DilationShapeIter;       // 0022
        public int GroundLowerThresh;       // 0023
        public int GroundUpperThresh;       // 0024
        public int ShapeLowerThresh;        // 0025
        public int ShapeUpperThresh;        // 0026
        public int CorrectMethod;           // 0027
        public int ShapeShiftMarginX;       // 0028
        public int ShapeShiftMarginY;       // 0029

        public int ThreshType2;             // 0049
        public int InspLowerThresh2;        // 0050
        public int InspUpperThresh2;        // 0051
        public int ApplyAverDiff2;          // 0052
        public int AverMinMargin2;          // 0053
        public int AverMaxMargin2;          // 0054
        public int InspRange2;              // 0055
        public int InspMinMargin2;          // 0056
        public int InspMaxMargin2;          // 0057
        public int MinDefectSize2;          // 0058
        public int IsInspectMaster;         // 0059
        public int IsInspectSlave;          // 0060
        public int ShapeShiftType;          // 0061
        public int ShapeOffsetX;            // 0062
        public int ShapeOffsetY;            // 0063
        public int MaxDefectSize;           // 0064
        public int MaxSmallDefectSize;      // 0065
        public int MaxSmallDefectCount;     // 0066
        public int MinSmallDefectSize2;     // 0067
        public int MinSmallDefectCount2;    // 0068
        public int MaxDefectSize2;          // 0069
        public int MaxSmallDefectSize2;     // 0070
        public int MaxSmallDefectCount2;    // 0071

        public ShapeShiftProperty()
        {
            Code = "3005";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            ApplyAverDiff = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0003");
            InspRange = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0006");
            AverMinMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0007");
            AverMaxMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0008");
            MinMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0009");
            MaxMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0010");
            ErosionTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0011");
            DilationTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0012");
            MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0015");
            ErosionShapeIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0021");
            DilationShapeIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0022");
            GroundLowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0023");
            GroundUpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0024");
            ShapeLowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0025");
            ShapeUpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0026");
            CorrectMethod = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0027");
            ShapeShiftMarginX = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0028");
            ShapeShiftMarginY = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0029");
            ThreshType2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0049");
            InspLowerThresh2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0050");
            InspUpperThresh2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0051");
            ApplyAverDiff2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0052");
            AverMinMargin2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0053");
            AverMaxMargin2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0054");
            InspRange2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0055");
            InspMinMargin2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0056");
            InspMaxMargin2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0057");
            MinDefectSize2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0058");
            IsInspectMaster = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0059");
            IsInspectSlave = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0060");
            ShapeShiftType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0061");
            ShapeOffsetX = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0062");
            ShapeOffsetY = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0063");
            MaxDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0064");
            MaxSmallDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0065");
            MaxSmallDefectCount = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0066");
            MinSmallDefectSize2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0067");
            MinSmallDefectCount2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0068");
            MaxDefectSize2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0069");
            MaxSmallDefectSize2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0070");
            MaxSmallDefectCount2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0071");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    if (ThreshType == 0 || ThreshType == 2) // LowerThreshold 값은 임계이상, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    if (ThreshType == 1 || ThreshType == 2) // Upperthreshold 값은 임계이하, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002", UpperThresh);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0003", ApplyAverDiff);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0006", InspRange);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0007", AverMinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0008", AverMaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0009", MinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0010", MaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0011", ErosionTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0012", DilationTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0015", MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0021", ErosionShapeIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0022", DilationShapeIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0023", GroundLowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0024", GroundUpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0025", ShapeLowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0026", ShapeUpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0027", CorrectMethod);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0028", ShapeShiftMarginX);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0029", ShapeShiftMarginY);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0049", ThreshType2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    if (ThreshType2 == 0 || ThreshType2 == 2) // InspLowerThreshold 값은 임계이상, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0050", InspLowerThresh2);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    if (ThreshType2 == 1 || ThreshType2 == 2) // InspUpperthreshold 값은 임계이하, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0051", InspUpperThresh2);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0052", ApplyAverDiff2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0053", AverMinMargin2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0054", AverMaxMargin2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0055", InspRange2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0056", InspMinMargin2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0057", InspMaxMargin2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0058", MinDefectSize2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0059", IsInspectMaster);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0060", IsInspectSlave);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0061", ShapeShiftType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0062", ShapeOffsetX);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0063", ShapeOffsetY);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0064", MaxDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0065", MaxSmallDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0066", MaxSmallDefectCount);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0067", MinSmallDefectSize2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0068", MinSmallDefectCount2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0069", MaxDefectSize2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0070", MaxSmallDefectSize2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0071", MaxSmallDefectCount2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            ShapeShiftProperty cloneShapeShift = new ShapeShiftProperty();

            cloneShapeShift.ThreshType = this.ThreshType;
            cloneShapeShift.LowerThresh = this.LowerThresh;
            cloneShapeShift.UpperThresh = this.UpperThresh;
            cloneShapeShift.ApplyAverDiff = this.ApplyAverDiff;
            cloneShapeShift.InspRange = this.InspRange;
            cloneShapeShift.AverMinMargin = this.AverMinMargin;
            cloneShapeShift.AverMaxMargin = this.AverMaxMargin;
            cloneShapeShift.MinMargin = this.MinMargin;
            cloneShapeShift.MaxMargin = this.MaxMargin;
            cloneShapeShift.ErosionTrainIter = this.ErosionTrainIter;
            cloneShapeShift.DilationTrainIter = this.DilationTrainIter;
            cloneShapeShift.MinDefectSize = this.MinDefectSize;
            cloneShapeShift.ErosionShapeIter = this.ErosionShapeIter;
            cloneShapeShift.DilationShapeIter = this.DilationShapeIter;
            cloneShapeShift.GroundLowerThresh = this.GroundLowerThresh;
            cloneShapeShift.GroundUpperThresh = this.GroundUpperThresh;
            cloneShapeShift.ShapeLowerThresh = this.ShapeLowerThresh;
            cloneShapeShift.ShapeUpperThresh = this.ShapeUpperThresh;
            cloneShapeShift.CorrectMethod = this.CorrectMethod;
            cloneShapeShift.ShapeShiftMarginX = this.ShapeShiftMarginX;
            cloneShapeShift.ShapeShiftMarginY = this.ShapeShiftMarginY;
            cloneShapeShift.ThreshType2 = this.ThreshType2;
            cloneShapeShift.InspLowerThresh2 = this.InspLowerThresh2;
            cloneShapeShift.InspUpperThresh2 = this.InspUpperThresh2;
            cloneShapeShift.ApplyAverDiff2 = this.ApplyAverDiff2;
            cloneShapeShift.AverMinMargin2 = this.AverMinMargin2;
            cloneShapeShift.AverMaxMargin2 = this.AverMaxMargin2;
            cloneShapeShift.InspRange2 = this.InspRange2;
            cloneShapeShift.InspMinMargin2 = this.InspMinMargin2;
            cloneShapeShift.InspMaxMargin2 = this.InspMaxMargin2;
            cloneShapeShift.MinDefectSize2 = this.MinDefectSize2;
            cloneShapeShift.IsInspectMaster = this.IsInspectMaster;
            cloneShapeShift.IsInspectSlave = this.IsInspectSlave;
            cloneShapeShift.ShapeShiftType = this.ShapeShiftType;
            cloneShapeShift.ShapeOffsetX = this.ShapeOffsetX;
            cloneShapeShift.ShapeOffsetY = this.ShapeOffsetY;
            cloneShapeShift.MaxDefectSize = this.MaxDefectSize;
            cloneShapeShift.MaxSmallDefectSize = this.MaxSmallDefectSize;
            cloneShapeShift.MaxSmallDefectCount = this.MaxSmallDefectCount;
            cloneShapeShift.MinSmallDefectSize2 = this.MinSmallDefectSize2;
            cloneShapeShift.MinSmallDefectCount2 = this.MinSmallDefectCount2;
            cloneShapeShift.MaxDefectSize2 = this.MaxDefectSize2;
            cloneShapeShift.MaxSmallDefectSize2 = this.MaxSmallDefectSize2;
            cloneShapeShift.MaxSmallDefectCount2 = this.MaxSmallDefectCount2;

            return cloneShapeShift;
        }
    }
    #endregion

    #region Shape Shift With CL Type.
    /// <summary>   Shape shift with cl property.  </summary>
    /// <remarks>   suoow2, 2012-09-12. </remarks>
    public class ShapeShiftWithCLProperty : InspectionAlgorithm
    {
        public int ThreshType;              // 0000
        public int LowerThresh;             // 0001
        public int UpperThresh;             // 0002
        public int ApplyAverDiff;           // 0003
        public int InspRange;               // 0006
        public int AverMinMargin;           // 0007
        public int AverMaxMargin;           // 0008
        public int MinMargin;               // 0009
        public int MaxMargin;               // 0010
        public int ErosionTrainIter;        // 0011
        public int DilationTrainIter;       // 0012
        public int MinDefectSize;           // 0015
        public int ErosionShapeIter;        // 0021  
        public int DilationShapeIter;       // 0022
        public int GroundLowerThresh;       // 0023
        public int GroundUpperThresh;       // 0024
        public int ShapeLowerThresh;        // 0025
        public int ShapeUpperThresh;        // 0026
        public int CorrectMethod;           // 0027
        public int ShapeShiftMarginX;       // 0028
        public int ShapeShiftMarginY;       // 0029

        // 형상 영역 중앙선 검사 2012-09-12
        public int IsInspectCL;             // 0100
        public int MinWidthRatio;           // 0030
        public int MinWidthSize;            // 0031
        public int MaxWidthRatio;           // 0032
        public int MaxWidthSize;            // 0033
        public int MinHeightSize;           // 0035
        public int MinNormalRatio;          // 0038
        public int MaxNormalRatio;          // 0039
        public int LeadThresh;              // 0041
        
        public int ThreshType2;             // 0049
        public int InspLowerThresh2;        // 0050
        public int InspUpperThresh2;        // 0051
        public int ApplyAverDiff2;          // 0052
        public int AverMinMargin2;          // 0053
        public int AverMaxMargin2;          // 0054
        public int InspRange2;              // 0055
        public int InspMinMargin2;          // 0056
        public int InspMaxMargin2;          // 0057
        public int MinDefectSize2;          // 0058
        public int IsInspectMaster;         // 0059
        public int IsInspectSlave;          // 0060
        public int ShapeShiftType;          // 0061
        public int ShapeOffsetX;            // 0062
        public int ShapeOffsetY;            // 0063
        public int MaxDefectSize;           // 0064
        public int MaxSmallDefectSize;      // 0065
        public int MaxSmallDefectCount;     // 0066
        public int MinSmallDefectSize2;     // 0067
        public int MinSmallDefectCount2;    // 0068
        public int MaxDefectSize2;          // 0069
        public int MaxSmallDefectSize2;     // 0070
        public int MaxSmallDefectCount2;    // 0071

        public ShapeShiftWithCLProperty()
        {
            Code = "3018";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            ApplyAverDiff = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0003");
            InspRange = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0006");
            AverMinMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0007");
            AverMaxMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0008");
            MinMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0009");
            MaxMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0010");
            ErosionTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0011");
            DilationTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0012");
            MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0015");
            ErosionShapeIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0021");
            DilationShapeIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0022");
            GroundLowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0023");
            GroundUpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0024");
            ShapeLowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0025");
            ShapeUpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0026");
            CorrectMethod = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0027");
            ShapeShiftMarginX = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0028");
            ShapeShiftMarginY = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0029");

            // 형상 영역 중앙선 검사 2012-09-12
            IsInspectCL = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0100");
            MinWidthRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0030");
            MinWidthSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0031");
            MaxWidthRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0032");
            MaxWidthSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0033");
            MinHeightSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0035");
            MinNormalRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0038");
            MaxNormalRatio = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0039");
            LeadThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0041");

            ThreshType2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0049");
            InspLowerThresh2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0050");
            InspUpperThresh2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0051");
            ApplyAverDiff2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0052");
            AverMinMargin2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0053");
            AverMaxMargin2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0054");
            InspRange2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0055");
            InspMinMargin2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0056");
            InspMaxMargin2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0057");
            MinDefectSize2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0058");
            IsInspectMaster = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0059");
            IsInspectSlave = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0060");
            ShapeShiftType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0061");
            ShapeOffsetX = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0062");
            ShapeOffsetY = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0063");
            MaxDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0064");
            MaxSmallDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0065");
            MaxSmallDefectCount = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0066");
            MinSmallDefectSize2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0067");
            MinSmallDefectCount2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0068");
            MaxDefectSize2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0069");
            MaxSmallDefectSize2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0070");
            MaxSmallDefectCount2 = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0071");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    if (ThreshType == 0 || ThreshType == 2) // LowerThreshold 값은 임계이상, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    if (ThreshType == 1 || ThreshType == 2) // Upperthreshold 값은 임계이하, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002", UpperThresh);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0003", ApplyAverDiff);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0006", InspRange);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0007", AverMinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0008", AverMaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0009", MinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0010", MaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0011", ErosionTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0012", DilationTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0015", MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0021", ErosionShapeIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0022", DilationShapeIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0023", GroundLowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0024", GroundUpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0025", ShapeLowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0026", ShapeUpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0027", CorrectMethod);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0028", ShapeShiftMarginX);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0029", ShapeShiftMarginY);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;

                    // 형상 영역 중앙선 검사 2012-09-12
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0100", IsInspectCL);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0030", MinWidthRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0031", MinWidthSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0032", MaxWidthRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0033", MaxWidthSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0035", MinHeightSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0038", MinNormalRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0039", MaxNormalRatio);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0041", LeadThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0049", ThreshType2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    if (ThreshType2 == 0 || ThreshType2 == 2) // InspLowerThreshold 값은 임계이상, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0050", InspLowerThresh2);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    if (ThreshType2 == 1 || ThreshType2 == 2) // InspUpperthreshold 값은 임계이하, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0051", InspUpperThresh2);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0052", ApplyAverDiff2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0053", AverMinMargin2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0054", AverMaxMargin2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0055", InspRange2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0056", InspMinMargin2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0057", InspMaxMargin2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0058", MinDefectSize2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0059", IsInspectMaster);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0060", IsInspectSlave);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0061", ShapeShiftType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0062", ShapeOffsetX);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0063", ShapeOffsetY);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0064", MaxDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0065", MaxSmallDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0066", MaxSmallDefectCount);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0067", MinSmallDefectSize2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0068", MinSmallDefectCount2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0069", MaxDefectSize2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0070", MaxSmallDefectSize2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0071", MaxSmallDefectCount2);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            ShapeShiftWithCLProperty cloneShapeShift = new ShapeShiftWithCLProperty();

            cloneShapeShift.ThreshType = this.ThreshType;
            cloneShapeShift.LowerThresh = this.LowerThresh;
            cloneShapeShift.UpperThresh = this.UpperThresh;
            cloneShapeShift.ApplyAverDiff = this.ApplyAverDiff;
            cloneShapeShift.InspRange = this.InspRange;
            cloneShapeShift.AverMinMargin = this.AverMinMargin;
            cloneShapeShift.AverMaxMargin = this.AverMaxMargin;
            cloneShapeShift.MinMargin = this.MinMargin;
            cloneShapeShift.MaxMargin = this.MaxMargin;
            cloneShapeShift.ErosionTrainIter = this.ErosionTrainIter;
            cloneShapeShift.DilationTrainIter = this.DilationTrainIter;
            cloneShapeShift.MinDefectSize = this.MinDefectSize;
            cloneShapeShift.ErosionShapeIter = this.ErosionShapeIter;
            cloneShapeShift.DilationShapeIter = this.DilationShapeIter;
            cloneShapeShift.GroundLowerThresh = this.GroundLowerThresh;
            cloneShapeShift.GroundUpperThresh = this.GroundUpperThresh;
            cloneShapeShift.ShapeLowerThresh = this.ShapeLowerThresh;
            cloneShapeShift.ShapeUpperThresh = this.ShapeUpperThresh;
            cloneShapeShift.CorrectMethod = this.CorrectMethod;
            cloneShapeShift.ShapeShiftMarginX = this.ShapeShiftMarginX;
            cloneShapeShift.ShapeShiftMarginY = this.ShapeShiftMarginY;

            // 형상 영역 중앙선 검사 2012-09-12
            cloneShapeShift.IsInspectCL = this.IsInspectCL;
            cloneShapeShift.MinWidthRatio = this.MinWidthRatio;
            cloneShapeShift.MinWidthSize = this.MinWidthSize;
            cloneShapeShift.MaxWidthRatio = this.MaxWidthRatio;
            cloneShapeShift.MaxWidthSize = this.MaxWidthSize;
            cloneShapeShift.MinHeightSize = this.MinHeightSize;
            cloneShapeShift.MinNormalRatio = this.MinNormalRatio;
            cloneShapeShift.MaxNormalRatio = this.MaxNormalRatio;
            cloneShapeShift.LeadThresh = this.LeadThresh;

            cloneShapeShift.ThreshType2 = this.ThreshType2;
            cloneShapeShift.InspLowerThresh2 = this.InspLowerThresh2;
            cloneShapeShift.InspUpperThresh2 = this.InspUpperThresh2;
            cloneShapeShift.ApplyAverDiff2 = this.ApplyAverDiff2;
            cloneShapeShift.AverMinMargin2 = this.AverMinMargin2;
            cloneShapeShift.AverMaxMargin2 = this.AverMaxMargin2;
            cloneShapeShift.InspRange2 = this.InspRange2;
            cloneShapeShift.InspMinMargin2 = this.InspMinMargin2;
            cloneShapeShift.InspMaxMargin2 = this.InspMaxMargin2;
            cloneShapeShift.MinDefectSize2 = this.MinDefectSize2;
            cloneShapeShift.IsInspectMaster = this.IsInspectMaster;
            cloneShapeShift.IsInspectSlave = this.IsInspectSlave;
            cloneShapeShift.ShapeShiftType = this.ShapeShiftType;
            cloneShapeShift.ShapeOffsetX = this.ShapeOffsetX;
            cloneShapeShift.ShapeOffsetY = this.ShapeOffsetY;
            cloneShapeShift.MaxDefectSize = this.MaxDefectSize;
            cloneShapeShift.MaxSmallDefectSize = this.MaxSmallDefectSize;
            cloneShapeShift.MaxSmallDefectCount = this.MaxSmallDefectCount;
            cloneShapeShift.MinSmallDefectSize2 = this.MinSmallDefectSize2;
            cloneShapeShift.MinSmallDefectCount2 = this.MinSmallDefectCount2;
            cloneShapeShift.MaxDefectSize2 = this.MaxDefectSize2;
            cloneShapeShift.MaxSmallDefectSize2 = this.MaxSmallDefectSize2;
            cloneShapeShift.MaxSmallDefectCount2 = this.MaxSmallDefectCount2;

            return cloneShapeShift;
        }
    }
    #endregion

    #region Ball Pattern Type.
    /// <summary>   Ball Pattern property.  </summary>
    /// <remarks>   Minseok, Hwang, 2012-07-22. </remarks>
    public class BallPatternProperty : InspectionAlgorithm
    {
        public int ThreshType;              // 0000
        public int LowerThresh;             // 0001
        public int UpperThresh;             // 0002
        public int ApplyAverDiff;           // 0003
        public int MaskLowerThresh;         // 0004
        public int MaskUpperThresh;         // 0005
        public int InspRange;               // 0006
        public int AverMinMargin;           // 0007
        public int AverMaxMargin;           // 0008
        public int MinMargin;               // 0009
        public int MaxMargin;               // 0010
        public int ErosionTrainIter = 0;    // 0011
        public int DilationTrainIter = 0;   // 0012
        public int ErosionInspIter = 0;     // 0013
        public int DilationInspIter = 0;    // 0014
        public int MinDefectSize;           // 0015
        public int MinSmallDefectSize;      // 0016
        public int MinSmallDefectCount;     // 0017

        public int MinWidthSize;            // 0031
        public int MinHeightSize;           // 0035
        public int MaxHeightSize;           // 0037

        public int MaxDefectSize;           // 0064
        public int MaxSmallDefectSize;      // 0065
        public int MaxSmallDefectCount;     // 0066
        public int UsePSRShift;             // 0048
        public int UsePunchShift;           // 0052
        public int SameValue;

        public BallPatternProperty()
        {
            Code = "3013";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            ApplyAverDiff = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0003");
            MaskLowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0004");
            MaskUpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0005");
            InspRange = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0006");
            AverMinMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0007");
            AverMaxMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0008");
            MinMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0009");
            MaxMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0010");
            ErosionTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0011");
            DilationTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0012");
            ErosionInspIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0013");
            DilationInspIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0014");
            MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0015");
            MinSmallDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0016");
            MinSmallDefectCount = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0017");
            MinWidthSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0031");
            MinHeightSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0035");
            MaxHeightSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0037");
            UsePSRShift = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0048");
            UsePunchShift = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0052");
            MaxDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0064");
            MaxSmallDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0065");
            MaxSmallDefectCount = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0066");
            SameValue = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0999");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    if (ThreshType == 0 || ThreshType == 2) // LowerThreshold 값은 임계이상, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    if (ThreshType == 1 || ThreshType == 2) // Upperthreshold 값은 임계이하, 임계범위인 경우에만 저장한다.
                    {
                        strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002", UpperThresh);
                        if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    }
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0003", ApplyAverDiff);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0004", MaskLowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0005", MaskUpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0006", InspRange);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0007", AverMinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0008", AverMaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0009", MinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0010", MaxMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0011", ErosionTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0012", DilationTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0013", ErosionInspIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0014", DilationInspIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0015", MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0016", MinSmallDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0017", MinSmallDefectCount);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0031", MinWidthSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0035", MinHeightSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0037", MaxHeightSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0048", UsePSRShift);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0052", UsePunchShift);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0064", MaxDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0065", MaxSmallDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0066", MaxSmallDefectCount);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0999", SameValue);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            BallPatternProperty cloneBallPatternMask = new BallPatternProperty();

            cloneBallPatternMask.ThreshType = this.ThreshType;
            cloneBallPatternMask.LowerThresh = this.LowerThresh;
            cloneBallPatternMask.UpperThresh = this.UpperThresh;
            cloneBallPatternMask.ApplyAverDiff = this.ApplyAverDiff;
            cloneBallPatternMask.MaskLowerThresh = this.MaskLowerThresh;
            cloneBallPatternMask.MaskUpperThresh = this.MaskUpperThresh;
            cloneBallPatternMask.InspRange = this.InspRange;
            cloneBallPatternMask.AverMinMargin = this.AverMinMargin;
            cloneBallPatternMask.AverMaxMargin = this.AverMaxMargin;
            cloneBallPatternMask.MinMargin = this.MinMargin;
            cloneBallPatternMask.MaxMargin = this.MaxMargin;
            cloneBallPatternMask.ErosionTrainIter = this.ErosionTrainIter;
            cloneBallPatternMask.DilationTrainIter = this.DilationTrainIter;
            cloneBallPatternMask.ErosionInspIter = this.ErosionInspIter;
            cloneBallPatternMask.DilationInspIter = this.DilationInspIter;
            cloneBallPatternMask.MinDefectSize = this.MinDefectSize;
            cloneBallPatternMask.MinSmallDefectSize = this.MinSmallDefectSize;
            cloneBallPatternMask.MinSmallDefectCount = this.MinSmallDefectCount;
            cloneBallPatternMask.MinWidthSize = this.MinWidthSize;
            cloneBallPatternMask.MinHeightSize = this.MinHeightSize;
            cloneBallPatternMask.MaxHeightSize = this.MaxHeightSize;
            cloneBallPatternMask.MaxDefectSize = this.MaxDefectSize;
            cloneBallPatternMask.MaxSmallDefectSize = this.MaxSmallDefectSize;
            cloneBallPatternMask.MaxSmallDefectCount = this.MaxSmallDefectCount;
            cloneBallPatternMask.UsePSRShift = this.UsePSRShift;
            cloneBallPatternMask.UsePunchShift = this.UsePunchShift;
            cloneBallPatternMask.SameValue = this.SameValue;

            return cloneBallPatternMask;
        }
    }
    #endregion

    #region VentHole Type.
    /// <summary>   VentHole property.  </summary>
    /// <remarks>   suoow2, 2016-03-10. </remarks>
    public class VentHoleProperty : InspectionAlgorithm
    {
        public int ThreshType;      // 0000
        public int LowerThresh;     // 0001
        public int MinDefectSize;   // 0015

        public VentHoleProperty()
        {
            Code = "3015";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0015");

        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0015", MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                    return -1;
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        public override int UpdateProperty(string strModelCode, string strRoiCode, string strInspectID)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = "SET SQL_SAFE_UPDATES = 0; ";
                    strQuery += string.Format(
                    "UPDATE bgadb.roi_param " +
                    "SET param_value = " +
                    "   CASE param_code " +
                    "       WHEN '0000' THEN '{0}' " +
                    "       WHEN '0001' THEN '{1}' " +
                    "       WHEN '0015' THEN '{2}' " +
                    "       ELSE param_value " +
                    "   END " +
                    "WHERE model_code = '{3}' " +
                    "AND roi_code = '{4}' " +
                    "AND inspect_id = '{5}' " +
                    "AND param_code IN ('0000', '0001', '0015') "
                    , ThreshType, LowerThresh, MinDefectSize
                    , strModelCode, strRoiCode, strInspectID);

                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                    return -1;
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            VentHoleProperty cloneVentHoleProperty = new VentHoleProperty();

            cloneVentHoleProperty.ThreshType = this.ThreshType;
            cloneVentHoleProperty.LowerThresh = this.LowerThresh;
            cloneVentHoleProperty.MinDefectSize = this.MinDefectSize;
            return cloneVentHoleProperty;
        }
    }
    #endregion

    #region VentHole2 Type.
    /// <summary>   VentHole property.  </summary>
    /// <remarks>   suoow2, 2016-03-10. </remarks>
    public class VentHoleProperty2 : InspectionAlgorithm
    {
        public int ThreshType;      // 0000
        public int LowerThresh;     // 0001
        public int MinDefectSize;   // 0015

        public VentHoleProperty2()
        {
            Code = "3015";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0015");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0015", MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                    return -1;
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        public override int UpdateProperty(string strModelCode, string strRoiCode, string strInspectID)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = "SET SQL_SAFE_UPDATES = 0; ";
                    strQuery += string.Format(
                    "UPDATE bgadb.roi_param " +
                    "SET param_value = " +
                    "   CASE param_code " +
                    "       WHEN '0000' THEN '{0}' " +
                    "       WHEN '0001' THEN '{1}' " +
                    "       WHEN '0015' THEN '{2}' " +
                    "       ELSE param_value " +
                    "   END " +
                    "WHERE model_code = '{3}' " +
                    "AND roi_code = '{4}' " +
                    "AND inspect_id = '{5}' " +
                    "AND param_code IN ('0000', '0001', '0015') "
                    , ThreshType, LowerThresh, MinDefectSize
                    , strModelCode, strRoiCode, strInspectID);

                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                    return -1;
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            VentHoleProperty2 cloneVentHoleProperty2 = new VentHoleProperty2();

            cloneVentHoleProperty2.ThreshType = this.ThreshType;
            cloneVentHoleProperty2.LowerThresh = this.LowerThresh;
            cloneVentHoleProperty2.MinDefectSize = this.MinDefectSize;
            return cloneVentHoleProperty2;
        }
    }
    #endregion

    #region Ball Pattern Type.
    /// <summary>   Outer State property.  </summary>
    /// <remarks>   suoow, 2020-01-04. </remarks>
    public class OuterProperty : InspectionAlgorithm
    {
        public int LowerThresh;             // 0001
        public int UpperThresh;             // 0002
        public int MaskLowerThresh;         // 0004
        public int MaskUpperThresh;         // 0005
        public int ErosionTrainIter = 0;    // 0011
        public int DilationTrainIter = 0;   // 0012
        public int MinDefectSize;           // 0015
        public int MaxDefectSize;           // 0064

        public OuterProperty()
        {
            Code = "3023";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            MaskLowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0004");
            MaskUpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0005");
            ErosionTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0011");
            DilationTrainIter = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0012");
            MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0015");
            MaxDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0064");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002", UpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0004", MaskLowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0005", MaskUpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0011", ErosionTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0012", DilationTrainIter);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0015", MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0064", MaxDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        public override int UpdateProperty(string strModelCode, string strRoiCode, string strInspectID)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = "SET SQL_SAFE_UPDATES = 0; ";
                    strQuery += string.Format(
                    "UPDATE bgadb.roi_param " +
                    "SET param_value = " +
                    "   CASE param_code " +
                    "       WHEN '0001' THEN '{0}' " +
                    "       WHEN '0002' THEN '{1}' " +
                    "       WHEN '0004' THEN '{2}' " +
                    "       WHEN '0005' THEN '{3}' " +
                    "       WHEN '0011' THEN '{4}' " +
                    "       WHEN '0012' THEN '{5}' " +
                    "       WHEN '0015' THEN '{6}' " +
                    "       WHEN '0064' THEN '{7}' " +
                    "       ELSE param_value " +
                    "   END " +
                    "WHERE model_code = '{8}' " +
                    "AND roi_code = '{9}' " +
                    "AND inspect_id = '{10}' " +
                    "AND param_code IN ('0001', '0002', '0004', '0005', '0011', '0012', '0015', '0064') "
                    , LowerThresh, UpperThresh, MaskLowerThresh, MaskUpperThresh, ErosionTrainIter, DilationTrainIter, MinDefectSize, MaxDefectSize
                    , strModelCode, strRoiCode, strInspectID);

                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            OuterProperty cloneOuterMask = new OuterProperty();

            cloneOuterMask.LowerThresh = this.LowerThresh;
            cloneOuterMask.UpperThresh = this.UpperThresh;
            cloneOuterMask.MaskLowerThresh = this.MaskLowerThresh;
            cloneOuterMask.MaskUpperThresh = this.MaskUpperThresh;
            cloneOuterMask.ErosionTrainIter = this.ErosionTrainIter;
            cloneOuterMask.DilationTrainIter = this.DilationTrainIter;
            cloneOuterMask.MinDefectSize = this.MinDefectSize;
            cloneOuterMask.MaxDefectSize = this.MaxDefectSize;

            return cloneOuterMask;
        }
    }
    #endregion

    #region WindowPunch Dam Size Type.
    /// <summary>   WindowPunch Dam Size property.  </summary>
    /// <remarks>   suoow2, 2017-05-04. </remarks>
    public class DamSizeProperty : InspectionAlgorithm
    {
        public int ThreshType;              // 0000
        public int LowerThresh;             // 0001
        public int UpperThresh;             // 0002
        public int MinMargin;               // 0009
        public int MinDefectSize;           // 0015
        public int MaxDefectSize;           // 0064
        public int MinWidth;      // 0065

        public DamSizeProperty()
        {
            Code = "3021";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ThreshType = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            LowerThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UpperThresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            MinMargin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0009");
            MinDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0015");
            MaxDefectSize = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0064");
            MinWidth = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0031");
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", ThreshType);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", LowerThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002", UpperThresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0009", MinMargin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0015", MinDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0064", MaxDefectSize);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0031", MinWidth);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            DamSizeProperty cloneDamMask = new DamSizeProperty();

            cloneDamMask.ThreshType = this.ThreshType;
            cloneDamMask.LowerThresh = this.LowerThresh;
            cloneDamMask.UpperThresh = this.UpperThresh;
            cloneDamMask.MinMargin = this.MinMargin;
            cloneDamMask.MinDefectSize = this.MinDefectSize;
            cloneDamMask.MaxDefectSize = this.MaxDefectSize;
            cloneDamMask.MinWidth = this.MinWidth;

            return cloneDamMask;
        }
    }
    #endregion


    #region WindowPunch Dam Size Type.
    /// <summary>   WindowPunch Dam Size property.  </summary>
    /// <remarks>   suoow2, 2017-05-04. </remarks>
    public class GV_Inspection_Property : InspectionAlgorithm
    {
        public int Ball_Thresh;             // 0000
        public int Core_Thresh;             // 0001
        public int Mask;                    // 0002
        public int Taget_GV;                // 0003
        public int GV_Margin;               // 0004


        public GV_Inspection_Property()
        {
            Code = "3027";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            if (!GV_Inspection_DefaultValue.DefaultValueLoaded)
            {
                GV_Inspection_DefaultValue.DefaultValueLoaded = true;
                GV_Inspection_DefaultValue.LoadDefaultValue();
            }
       
            Ball_Thresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            if (Ball_Thresh == 0) Ball_Thresh = GV_Inspection_DefaultValue.Ball_Thresh;

            Core_Thresh = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            if (Core_Thresh == 0) Core_Thresh = GV_Inspection_DefaultValue.Core_Thresh;

            Mask = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");
            if (Mask == 0) Mask = GV_Inspection_DefaultValue.Mask;

            Taget_GV = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0003");
            if (Taget_GV == 0) Taget_GV = GV_Inspection_DefaultValue.Taget_GV;

            GV_Margin = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0004");
            if (GV_Margin == 0) GV_Margin = GV_Inspection_DefaultValue.GV_Margin;
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", Ball_Thresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", Core_Thresh);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002", Mask);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0003", Taget_GV);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0004", GV_Margin);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        public override int UpdateProperty(string strModelCode, string strRoiCode, string strInspectID)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    string strQuery = "SET SQL_SAFE_UPDATES = 0; ";
                    strQuery += string.Format(
                    "UPDATE bgadb.roi_param " +
                    "SET param_value = " +
                    "   CASE param_code " +
                    "       WHEN '0000' THEN '{0}' " +
                    "       WHEN '0001' THEN '{1}' " +
                    "       WHEN '0002' THEN '{2}' " +
                    "       WHEN '0003' THEN '{3}' " +
                    "       WHEN '0004' THEN '{4}' " +
                    "       ELSE param_value " +
                    "   END " +
                    "WHERE model_code = '{5}' " +
                    "AND roi_code = '{6}' " +
                    "AND inspect_id = '{7}' " +
                    "AND param_code IN ('0000', '0001', '0002', '0003', '0004') "
                    , Ball_Thresh, Core_Thresh, Mask, Taget_GV, Taget_GV, GV_Margin
                    , strModelCode, strRoiCode, strInspectID);

                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            GV_Inspection_Property clone_GV_Inspection = new GV_Inspection_Property();
            clone_GV_Inspection.Ball_Thresh = this.Ball_Thresh;
            clone_GV_Inspection.Core_Thresh = this.Core_Thresh;
            clone_GV_Inspection.Mask = this.Mask;
            clone_GV_Inspection.Taget_GV = this.Taget_GV;
            clone_GV_Inspection.GV_Margin = this.GV_Margin;

            return clone_GV_Inspection;
        }

    }
    #endregion

    #region Exceptional Mask.
    /// <summary>   Exceptional mask property.  </summary>
    /// <remarks>   suoow2, 2014-10-09. </remarks>
    public class ExceptionalMaskProperty : InspectionAlgorithm
    {
        public int ExceptionX;    // 0000
        public int ExceptionY;    // 0001
        public int UseShapeShift; // 0002

        /// <summary>   Initializes a new instance of the FiducialAlignProperty class. </summary>
        /// <remarks>   suoow2, 2014-10-09. </remarks>
        public ExceptionalMaskProperty()
        {
            Code = "3016";
        }

        #region Load & Save Properties.
        // Load.
        public override void LoadProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            ExceptionX = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0000");
            ExceptionY = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0001");
            UseShapeShift = InspectionQueryHelper.GetParamValue(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, this.Code, "0002");

            ExceptionX = (ExceptionX == 9999) ? -1 : ExceptionX;
            ExceptionY = (ExceptionY == 9999) ? -1 : ExceptionY;
        }

        // Save.
        public override int SaveProperty(string strMachineCode, string strModelCode, string strWorkType, string strSectionCode, string strRoiCode, string strInspectID, string strInspectCode)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    int nDbExceptionX = (ExceptionX == -1) ? 9999 : ExceptionX;
                    int nDbExceptionY = (ExceptionY == -1) ? 9999 : ExceptionY;
                    string strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0000", nDbExceptionX);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0001", nDbExceptionY);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                    // UseShapeShift = 0 | 1 | 2 | 3;
                    strQuery = InspectionQueryHelper.GetInsertQuery(strMachineCode, strModelCode, strWorkType, strSectionCode, strRoiCode, strInspectID, strInspectCode, Code, "0002", UseShapeShift);
                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else return -1;
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        public override int UpdateProperty(string strModelCode, string strRoiCode, string strInspectID)
        {
            try
            {
                if (ConnectFactory.DBConnector() != null)
                {
                    int nDbExceptionX = (ExceptionX == -1) ? 9999 : ExceptionX;
                    int nDbExceptionY = (ExceptionY == -1) ? 9999 : ExceptionY;

                    string strQuery = "SET SQL_SAFE_UPDATES = 0; ";
                    strQuery += string.Format(
                    "UPDATE bgadb.roi_param " +
                    "SET param_value = " +
                    "   CASE param_code " +
                    "       WHEN '0000' THEN '{0}' " +
                    "       WHEN '0001' THEN '{1}' " +
                    "       WHEN '0002' THEN '{2}' " +
                    "       ELSE param_value " +
                    "   END " +
                    "WHERE model_code = '{3}' " +
                    "AND roi_code = '{4}' " +
                    "AND inspect_id = '{5}' " +
                    "AND param_code IN ('0000', '0001', '0002') "
                    , nDbExceptionX, nDbExceptionY, UseShapeShift
                    , strModelCode, strRoiCode, strInspectID);

                    if (ConnectFactory.DBConnector().Execute(strQuery) < 1) return -1;
                }
                else return -1;
            }
            catch
            {
                return -1;
            }

            return 1; // Save Success !!!
        }
        #endregion

        // Deep Copy.
        public override InspectionAlgorithm Clone()
        {
            ExceptionalMaskProperty exceptionalMaskProperty = new ExceptionalMaskProperty();
            exceptionalMaskProperty.ExceptionX = this.ExceptionX;
            exceptionalMaskProperty.ExceptionY = this.ExceptionY;
            exceptionalMaskProperty.UseShapeShift = this.UseShapeShift;

            return exceptionalMaskProperty;
        }
    }
    #endregion



}
