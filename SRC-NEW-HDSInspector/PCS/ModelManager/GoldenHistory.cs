using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace PCS.ELF.AVI
{
    public class GoldenHistory : NotifyPropertyChanged
    {
        public string Author
        {
            get
            {
                return m_szAuthor;
            }
            set
            {
                m_szAuthor = value;
                Notify("Author");
            }
        }
        public string Comment
        {
            get
            {
                return m_szComment;
            }
            set
            {
                m_szComment = value;
                Notify("Comment");
            }
        }
        public DateTime Date
        {
            get
            {
                return m_dtDate;
            }
            set
            {
                m_dtDate = value;
                Notify("Date");
            }
        }

        private string m_szAuthor;
        private string m_szComment;
        private DateTime m_dtDate;
    }
}
