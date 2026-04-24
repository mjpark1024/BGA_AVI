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

namespace RVS.Generic.Insp
{
    /// <summary>   Defect result.  </summary>
    public class DefectResult
    {
        /// <summary>   Gets or sets the serial. </summary>
        /// <value> The serial. </value>
        public int Serial
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the result code. </summary>
        /// <value> The result code. </value>
        public string ResultCode
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the type of the work. </summary>
        /// <value> The type of the work. </value>
        public string WorkType
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the section code. </summary>
        /// <value> The section code. </value>
        public string SectionCode
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the roi code. </summary>
        /// <value> The roi code. </value>
        public string RoiCode
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the defect code. </summary>
        /// <value> The defect code. </value>
        public string DefectCode
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the default image path. </summary>
        /// <value> The default image path. </value>
        public string DefaultImagePath
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the full pathname of the defect image file. </summary>
        /// <value> The full pathname of the defect image file. </value>
        public string DefectImagePath
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the full pathname of the live image file. </summary>
        /// <value> The full pathname of the live image file. </value>
        public string LiveImagePath
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the defect score. </summary>
        /// <value> The defect score. </value>
        public Double DefectScore
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the defect boundary. </summary>
        /// <value> The defect boundary. </value>
        public string DefectBoundary
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the defect center x coordinate. </summary>
        /// <value> The defect center x coordinate. </value>
        public Double DefectCenterX
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the defect center y coordinate. </summary>
        /// <value> The defect center y coordinate. </value>
        public Double DefectCenterY
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the defect position x coordinate. </summary>
        /// <value> The defect position x coordinate. </value>
        public Double DefectPosX
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the defect position y coordinate. </summary>
        /// <value> The defect position y coordinate. </value>
        public Double DefectPosY
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the size of the defect. </summary>
        /// <value> The size of the defect. </value>
        public Double DefectSize
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the identifier of the result. </summary>
        /// <value> The identifier of the result. </value>
        public string ResultID
        {
            get;
            set;
        }
    }
}
