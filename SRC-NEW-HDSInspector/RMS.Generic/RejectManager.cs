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
    /// <summary>   Reject.  </summary>
    public class Reject : BaseCode
    {
        /// <summary>   Gets or sets the strip defect rate. </summary>
        /// <value> The strip defect rate. </value>
        public Double StripDefectRate
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the unit defect rate. </summary>
        /// <value> The unit defect rate. </value>
        public Double UnitDefectRate
        {
            get;
            set;
        }

        /// <summary>   Makes a deep copy of this object. </summary>
        /// <remarks>   suoow2, 2014-09-14. </remarks>
        /// <returns>   A copy of this object. </returns>
        public Reject Clone()
        {
            Reject clonedReject = new Reject();

            clonedReject.Code = this.Code;
            clonedReject.Name = this.Name;
            clonedReject.MasterCode = this.MasterCode;
            clonedReject.StripDefectRate = this.StripDefectRate;
            clonedReject.UnitDefectRate = this.UnitDefectRate;

            return clonedReject;
        }
    }

    /// <summary>   Manager for rejects.  </summary>
    public class RejectManager
    {
        /// <summary>   Select reject type. </summary>
        public Reject SelectRejectType(string strCode)
        {
            Reject SelectReject = new Reject();
            if (ConnectFactory.DBConnector() != null)
            {
                string strQuery = string.Format("SELECT a.reject_type. a.reject_name, a.stripdefect_rate, a.unitdefect_rate FROM reject a WHERE a.use_yn = 1 and a.reject_type = '{0}' ", strCode);
                
                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        SelectReject.Code = dataReader.GetValue(0).ToString();
                        SelectReject.Name = dataReader.GetValue(1).ToString();
                        SelectReject.StripDefectRate = Convert.ToDouble(dataReader.GetValue(2).ToString());
                        SelectReject.UnitDefectRate = Convert.ToDouble(dataReader.GetValue(3).ToString());
                    }
                    dataReader.Close();
                }
            }

            return SelectReject;
        }
    }
    


}
