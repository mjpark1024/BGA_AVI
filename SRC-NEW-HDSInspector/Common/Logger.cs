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
 * @file  Logger.cs
 * @brief 
 *  logger module.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.10.06
 * @version : 1.1
 * 
 * <b> Revision Histroy </b>
 * - 2011.05.02 First creation.
 * - 2011.10.06 Re-structuring.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace Common
{
    /// <summary>   Values that represent SeverityLevel.  </summary>
    /// <remarks>   suoow2, 2014-10-06. </remarks>
    public enum SeverityLevel
    {
        DEBUG = 0,  // debug purpose.
        INFO = 1,   // significant events in the normal flow of the application.
        WARN = 2,   // minor problems or potential errors.
        ERROR = 3,  // exception or error condition.
        FATAL = 4   // 
    }
       
    /// <summary>   Logger.  </summary>
    /// <remarks>   suoow2, 2014-10-06. </remarks>
    public class Logger
    {
        private const int MAX_LOG_COUNT = 10;
        private static Logger m_Logger = null;
        private static StreamWriter m_StreamWriter = null;

        private List<string> m_LogMessageList = new List<string>();

        private static string m_strLogPath = "log";
        private static readonly string m_strStartUpPath = ((DirectoryInfo)Directory.GetParent(Directory.GetCurrentDirectory())).FullName;
        private static DateTime m_CurDate = DateTime.Now.Date;

        private Logger(string strFileName)
        {
            FileStream fileStream = new FileStream(strFileName, FileMode.Append, FileAccess.Write, FileShare.Read);
            m_StreamWriter = new StreamWriter(fileStream, System.Text.Encoding.Unicode, 4096);
        }

        public static Logger GetLogger()
        {
            SetLogger();
            //CleanLog(nDay);
            return m_Logger;
        }

        // Set Logger state.
        private static void SetLogger()
        {
            if (m_Logger == null || m_CurDate != DateTime.Now.Date)
            {
                if (m_Logger != null)
                {
                    m_Logger.Close();
                }
                m_CurDate = DateTime.Now.Date; // update current date.

                m_strLogPath = Path.Combine(m_strStartUpPath, m_strLogPath);
                if (Directory.Exists(m_strLogPath) == false)
                {
                    Directory.CreateDirectory(m_strLogPath);
                }

                // string strFileName = DateTime.Now.ToShortDateString().Replace(@"/", @"-").Replace(@"\", @"-") + ".log";
                m_Logger = new Logger(Path.Combine(m_strLogPath, DateTime.Now.ToShortDateString().Replace(@"/", @"-").Replace(@"\", @"-") + ".log"));
            }
        }

        public static int CleanLog(int nDay)
        {
            if (Directory.Exists(m_strLogPath) == false)
            {
                Directory.CreateDirectory(m_strLogPath);
            }

            string[] files = Directory.GetFiles(m_strLogPath, "*.log");

            int nDeleteCount = 0;
            foreach (string file in files)
            {
                DateTime dateTime = File.GetLastWriteTime(file);

                try
                {
                    int nDays = (DateTime.Now.Year - dateTime.Year) * 365;
                    nDays += DateTime.Now.DayOfYear - dateTime.DayOfYear;

                    if (nDays > nDay)
                    {
                        File.Delete(file);
                        nDeleteCount++;
                    }
                }
                catch
                {
                    Debug.WriteLine("Exception occured in CleanLog(SettingLog.cs)");
                }
            }

            return nDeleteCount;
        }

        // Do Log.
        public void Log(string strSubSystem, SeverityLevel severityLevel, string strMessage, bool isDirectLog = false)
        {
            lock (this)
            {
                if (m_StreamWriter != null)
                {
                    SetLogger();

                    string strLogMessage = String.Format("[{0}][{1}][{2}] {3}\r\n", strSubSystem, severityLevel, DateTime.Now, strMessage);
                    m_LogMessageList.Add(strLogMessage);

                    //if (isDirectLog || (m_LogMessageList.Count >= MAX_LOG_COUNT))
                    //{
                        try
                        {
                            foreach (string _strLogMessage in m_LogMessageList)
                            {
                                m_StreamWriter.Write(_strLogMessage);
                            }

                            //if (isDirectLog) // 즉시 로그를 남길 필요가 있는 경우 파일에 바로 기록한다.
                            //{
                                if (m_StreamWriter.BaseStream != null)
                                {
                                    m_StreamWriter.Flush();
                                }
                            //}
                            m_LogMessageList.Clear();
                        }
                        catch
                        {
                            Debug.WriteLine("Exception occured in Log(Logger.cs)");
                        }
                    //}
                }
            }            
        }

        // Close.
        public void Close()
        {
            if (m_StreamWriter != null)
            {
                try
                {
                    foreach (string _strLogMessage in m_LogMessageList)
                    {
                        m_StreamWriter.Write(_strLogMessage);
                    }
                    m_LogMessageList.Clear();

                    if (m_StreamWriter.BaseStream != null)
                    {
                        m_StreamWriter.Flush();
                    }
                    m_StreamWriter.Close();
                    m_StreamWriter = null;
                }
                catch
                {
                    Debug.WriteLine("Exception occured in Close(Logger.cs)");
                }
            }
        }
    }

    // Log Type class. { Time | Message }
    public class LogType
    {
        public LogType(string aszMessage) : this(DateTime.Now, aszMessage) { }
        public LogType(DateTime aTime, string aszMessage)
        {
            this.Time = aTime;
            this.Message = aszMessage;
        }

        public DateTime Time  { get; set; }
        public string Message { get; set; }
    }
}
