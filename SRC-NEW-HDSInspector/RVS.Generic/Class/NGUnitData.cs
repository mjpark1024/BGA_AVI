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
using System.Linq;
using System.Text;
using Common;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace RVS.Generic
{
    /// <summary>   Ng unit data.  </summary>
    [Serializable]
    public class NGUnitData
    {
        /// <summary>   Initializes a new instance of the NGUnitData class. </summary>
        public NGUnitData()
        {
            ImageData = new List<NGImageData>();
        }

        /// <summary>   Gets or sets the information describing the image. </summary>
        /// <value> Information describing the image. </value>
        public List<NGImageData> ImageData
        {
            get;
            set;
        }

        /// <summary>   Gets or sets a value indicating whether this object is good. </summary>
        /// <value> true if this object is good, false if not. </value>
        [DefaultValue(false)]
        public bool IsGood
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the n strip number. </summary>
        /// <value> The m n strip number. </value>
        public int StripNumber
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the n unit row. </summary>
        /// <value> The m n unit row. </value>
        public int UnitRow
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the n unit col. </summary>
        /// <value> The m n unit col. </value>
        public int UnitCol
        {
            get;
            set;
        }
    }
}
