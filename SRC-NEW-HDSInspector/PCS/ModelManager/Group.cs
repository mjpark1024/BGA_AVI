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
/**
 * @file  Group.cs
 * @brief 
 *  Group definition of ELF AVI.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.08.10
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.10 First creation.
 */

using System;
using System.Xml.Serialization;

namespace PCS.ELF.AVI
{
    /// <summary>   Group.  </summary>
    /// <remarks>   suoow2, 2014-08-03. </remarks>
    [XmlRoot("Group")]
    public class Group : ProfileNotifyChanged
    {
        [XmlIgnore]
        public int Index
        {
            get
            {
                return m_nIndex;
            }
            set
            {
                m_nIndex = value;
                Notify("Index");
            }
        }
        private int m_nIndex = -1;

        #region Constructors.
        public Group() : this(-1, string.Empty, string.Empty) { }

        public Group(int nIndex, string strCode, string strName)
        {
            Index = nIndex;
            Code = strCode;
            Name = strName;
        }
        #endregion

        // Deep Copy.
        public Group Clone()
        {
            return new Group()
            {
                Index = this.Index,
                Code = this.Code,
                Name = this.Name
            };
        }
    }
}
