using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Net;

namespace Emc.Documentum.Rest.Utility
{
    /// <summary>
    /// Enum used for the LoggerFacade
    /// </summary>
    public enum LogLevel

    {
        /// <summary>
        /// Debug Level
        /// </summary>
        DEBUG,
        /// <summary>
        /// Info Level
        /// </summary>
        INFO,
        /// <summary>
        /// Warning Level
        /// </summary>
        WARNING,
        /// <summary>
        /// Error Level
        /// </summary>
        ERROR,
        /// <summary>
        /// Fatal Level
        /// </summary>
        FATAL
    }

    /// <summary>
    /// Logger facade stand-in while there is not access to the Process Logger. This will be switch during integration
    /// </summary>
    public class LoggerFacade 
    {
        // Private members that do not change after instantiation.
        private string _process;
        private string _processSubTag;
        private string _folderID;
        private string _userName;
        public LogLevel LogLevelThreshold {get; set;}
        public string TestDirectory { get; set; }
        public bool isPerformance { get; set; }
        // Shared private resources.
        // Disposable.

        /// <summary>
        /// Constructor for the logger facade
        /// </summary>
        /// <param name="process"></param>
        /// <param name="processSubTag"></param>
        /// <param name="folderID"></param>
        /// <param name="userName"></param>
        public LoggerFacade(string process, string processSubTag, string folderID, string userName)
        {
            if (string.IsNullOrWhiteSpace(process))
                throw new ArgumentException("The process parameter cannot be null or whitespace.");

            _process = process;
            _processSubTag = processSubTag;
            _folderID = folderID;
            _userName = userName;

            LogLevelThreshold = LogLevel.ERROR;
            _processSubTag = process + ":FACADE";
        }

        /// <summary>
        /// Facade method for WriteToLog
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="thread"></param>
        /// <param name="message"></param>
        /// <param name="verboseMessage"></param>
        public void WriteToLog(LogLevel logLevel, string thread, string message, string verboseMessage)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("The message parameter cannot be null or whitespace.");

            if (!string.IsNullOrWhiteSpace(thread)) thread = thread.Trim();
            if (!string.IsNullOrWhiteSpace(message)) message = message.Trim();
            if (!string.IsNullOrWhiteSpace(verboseMessage)) verboseMessage = verboseMessage.Trim();
            ConsoleColor fgOriginal = Console.ForegroundColor;
            ConsoleColor bgOriginal = Console.BackgroundColor;
            try
            {
                if (logLevel >= LogLevelThreshold)
                {
                    if (isPerformance)
                    {
                        if (thread.Equals("NORMAL"))
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                        }
                        if (thread.Equals("SLOW"))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.BackgroundColor = ConsoleColor.White;
                        }
                    
                    
                        Console.WriteLine(
                        logLevel.ToString() + "|" + thread  + "|" +
                            //_process+","+
                            //_processSubTag+","+
                            //_folderID+","+
                            //_userName+","+
                            //thread+","+
                            //message+","+
                         verboseMessage + "|" + message);
                    }
                    File.AppendAllText(TestDirectory + Path.DirectorySeparatorChar + "Performance.txt", DateTime.Now + "|" + "TYPE: [" + verboseMessage + "]|" + message +"\n");
                    }
            }
            catch (Exception ex)
            {
                WriteToEventLog(EventLogEntryType.Error, new Exception("Unable to write to log table.", ex));
            }
            Console.ForegroundColor = fgOriginal;
            Console.BackgroundColor = bgOriginal;
        }

        /// <summary>
        /// Facade method for WriteToLog
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="thread"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void WriteToLog(LogLevel logLevel, string thread, string message, Exception exception)
        {
            WriteToLog(logLevel, thread, message, ConvertExceptionToString(exception));
        }

        /// <summary>
        /// Facade method for WriteToLogWithXmlMessage
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="thread"></param>
        /// <param name="message"></param>
        /// <param name="xmlMessage"></param>
        public void WriteToLogWithXmlMessage(LogLevel logLevel, string thread, string message, string xmlMessage)
        {
            WriteToLog(logLevel, thread, message, ConvertXmlToString(xmlMessage));
        }

        /// <summary>
        /// Facade method for WriteToEventLog
        /// </summary>
        /// <param name="eventLogEntryType"></param>
        /// <param name="message"></param>
        public void WriteToEventLog(System.Diagnostics.EventLogEntryType eventLogEntryType, string message)
        {
           // Unimplemented
        }

        /// <summary>
        /// Facade method for WriteToEventLog
        /// </summary>
        /// <param name="eventLogEntryType"></param>
        /// <param name="exception"></param>
        public void WriteToEventLog(System.Diagnostics.EventLogEntryType eventLogEntryType, Exception exception)
        {
            WriteToEventLog(eventLogEntryType, ConvertExceptionToString(exception));
        }

        /// <summary>
        /// Facade method for WriteToServiceTransactionLog
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="action"></param>
        /// <param name="description"></param>
        /// <param name="xmlMessage"></param>
        public void WriteToServiceTransactionLog(string thread, string action, string description, string xmlMessage)
        {
            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("The action parameter cannot be null or whitespace.");
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("The description parameter cannot be null or whitespace.");
            if (string.IsNullOrWhiteSpace(xmlMessage))
                throw new ArgumentException("The xmlMessage parameter cannot be null or whitespace.");

            if (!string.IsNullOrWhiteSpace(thread)) thread = thread.Trim();
            if (!string.IsNullOrWhiteSpace(action)) action = action.Trim();
            if (!string.IsNullOrWhiteSpace(description)) description = description.Trim();
            if (!string.IsNullOrWhiteSpace(xmlMessage)) xmlMessage = xmlMessage.Trim();

            const int maxLength = 255;
            if (action.Length > maxLength) action = action.Substring(0, maxLength);
            if (description.Length > maxLength) description = description.Substring(0, maxLength);

            try
            {
                Console.WriteLine("FACADELOGGER:" +
                    _process+","+
                    _processSubTag+","+
                    _folderID+","+
                    _userName+","+ 
                    thread+","+
                    action+","+
                    description+","+
                    xmlMessage);

            }
            catch (Exception ex)
            {
                WriteToEventLog(EventLogEntryType.Error, new Exception("Unable to write to log table.", ex));
            }
        }

        internal string ConvertExceptionToString(Exception exception)
        {
            var exceptionString = string.Empty;
            if (exception != null)
                exceptionString = string.Format("{0}{1}Stack Trace:{2}{3}",
                    exception.ToString(), System.Environment.NewLine, System.Environment.NewLine, exception.StackTrace);
            return exceptionString;
        }

        internal string ConvertXmlToString(string xml)
        {
            var xmlString = string.Empty;
            if (!string.IsNullOrWhiteSpace(xml))
                xmlString = WebUtility.HtmlEncode(xml); 
            return xmlString;
        }

        internal string ConvertMessageToEventViewerMessage(string message, string process, string processSubTag, string eFolderID, string eUserName)
        {
            const int maxLength = 31000;

            if (string.IsNullOrWhiteSpace(message)) message = "Message parameter was null or whitespace";

            var sb = new StringBuilder();
            sb.AppendFormat("Process: [{0}]{1}", process, System.Environment.NewLine);
            sb.AppendFormat("Process Subtag: [{0}]{1}", processSubTag, System.Environment.NewLine);
            sb.AppendFormat("FolderID: [{0}]{1}", eFolderID, System.Environment.NewLine);
            sb.AppendFormat("UserName: [{0}]{1}", eUserName, System.Environment.NewLine);
            sb.AppendFormat("Error: {0}", message);

            if (sb.ToString().Length > maxLength)
                return string.Format("(TRUNCATED): {0}", sb.ToString()).Substring(0, maxLength);
            else
                return sb.ToString();
        }
    }
}
