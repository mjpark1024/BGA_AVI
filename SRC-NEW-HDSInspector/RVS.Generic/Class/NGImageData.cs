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
using Common;
using System.Windows.Media.Imaging;

namespace RVS.Generic
{
    /// <summary>   Ng image data.  </summary>
    [Serializable]
    public class NGImageData
    {
        /// <summary>   Initializes a new instance of the NGImageData class. </summary>
        public NGImageData()
        {
        }

        /// <summary>   Gets or sets zero-based index of the image. </summary>
        /// <value> The m image index. </value>
        public int ImageIndex
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the strip surface. </summary>
        /// <value> The m strip surface. </value>
        public Surface StripSurface
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the type of the ng. </summary>
        /// <value> The type of the ng. </value>
        //public RectFill NGType
        //{
        //    get;
        //    set;
        //}

        public int NG_ID
        {
            get;
            set;
        }

        public System.Windows.Media.Brush NG_Color
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the name of the string. </summary>
        /// <value> The name of the string. </value>
        public string NGName
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the full pathname of the string image file. </summary>
        /// <value> The full pathname of the string image file. </value>
        public string NGImagePath
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the full pathname of the string standard image file. </summary>
        /// <value> The full pathname of the string standard image file. </value>
        public string StandardImagePath
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the string result code. </summary>
        /// <value> The m string result code. </value>
        public string ResultCode
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the ng image source. </summary>
        /// <value> The m ng image source. </value>
        public BitmapSource NGImageSource
        {
            get;
            set;
        }

        /// <summary>   Gets or sets the standard image source. </summary>
        /// <value> The m standard image source. </value>
        public BitmapSource StandardImageSource
        {
            get;
            set;
        }

        public int StripID
        {
            get;
            set;
        }

        public int UnitX
        {
            get;
            set;
        }

        public int UnitY
        {
            get;
            set;
        }

        public string DefectCenterX
        {
            get;
            set;
        }

        public string DefectCenterY
        {
            get;
            set;
        }

        public string DefectSize
        {
            get;
            set;
        }

    }
}
