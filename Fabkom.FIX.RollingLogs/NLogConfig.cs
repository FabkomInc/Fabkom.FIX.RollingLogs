using QuickFix;
using QuickFix.Fields;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fabkom.FIX.RollingLogs
{

    public class SessionNLogConfig
    {
        public readonly string FileLogPath;
        public readonly int MessagesMaxFileSize;
        public readonly int MessagesNumFilesToKeep;
        public readonly int EventsMaxFileSize;
        public readonly int EventsNumFilesToKeep;
        public readonly string SessionID;

        public SessionNLogConfig(string sessionID, string fileLogPath, int messagesMaxFileSize, int messagesNumFilesToKeep, int eventsMaxFileSize, int eventsNumFilesToKeep)
        {
            SessionID = sessionID;
            FileLogPath = fileLogPath;
            MessagesMaxFileSize = messagesMaxFileSize;
            MessagesNumFilesToKeep = messagesNumFilesToKeep;
            EventsMaxFileSize = eventsMaxFileSize;
            EventsNumFilesToKeep = eventsNumFilesToKeep;
        }
    }


    public class NLogConfig
    {
        public static string NLOG_CONFIG_FILE = "NLog.ConfigFile";
        public static string NLOG_DEBUG_USEINTERNALLOGGER = "NLog.Debug.UseInternalLogger";
        public static string NLOG_DEBUG_USECONSOLE = "NLog.Debug.UseConsole";

        public static string NLOG_MAXFILESIZE_MESSAGES = "NLog.MaxFileSize.Messages";
        public static string NLOG_NUMFILESTOKEEP_MESSAGES = "NLog.NumFilesToKeep_Messages";
        public static string NLOG_MAXFILESIZE_EVENTS = "NLog.MaxFileSize_Events";
        public static string NLOG_NUMFILESTOKEEP_EVENTS = "NLog.NumFilesToKeep_Events";

        public string NLogConfigFile { get; private set; }

        public List<SessionNLogConfig> sessionsNLog = new List<SessionNLogConfig>();

        public NLogInternalLoggerConfig internalLoggerConfig; 
        
        public NLogConfig(string configFile)
        {
            if (!File.Exists(configFile))
            {
                throw new ConfigError("File " + configFile + " not found.");
            }

            try
            {
                using (FileStream fileStream = File.Open(configFile, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream))
                    {
                        Settings settings = new Settings(streamReader);
                        LinkedList<Dictionary> dicts = settings.Get("DEFAULT");
                        var dict = dicts.First.Value;

                        NLogConfigFile = GetFIXDictionaryString(dict, NLOG_CONFIG_FILE);
                        if (!string.IsNullOrEmpty(NLogConfigFile))
                        {
                            if (File.Exists(NLogConfigFile))
                                return;
                            //else
                            //    throw new ConfigError($"NLog ConfigurationFile {NLogConfigFile} not found.");
                        }

                        if(GetFIXDictionaryBool(dict, NLOG_DEBUG_USEINTERNALLOGGER))
                            internalLoggerConfig = new NLogInternalLoggerConfig(true)
                            {
                                AutoReload = true,
                                ThrowExceptions = true,
                                ThrowConfigExceptions = true,
                                LogToConsole = GetFIXDictionaryBool(dict, NLOG_DEBUG_USECONSOLE),
                                LogFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "nlog-internal.log"),
                                LogLevel = NLogXMLConfig.INTERNALLOGLEVEL_DEBUG,
                            };

                        LinkedList<Dictionary> sessions = settings.Get("SESSION");
                        foreach (Dictionary session in sessions)
                        {
                            session.Merge(dict);
                            
                            var fileLogPath = GetFIXDictionaryString(session, NLOG_MAXFILESIZE_MESSAGES);
                            var nLogMeggagesMaxFileSize = GetFIXDictionaryInt(session, NLOG_MAXFILESIZE_MESSAGES, 30);
                            var nLogMessagesNumFilesToKeep = GetFIXDictionaryInt(session, NLOG_NUMFILESTOKEEP_MESSAGES, 10);
                            var nLogEventsMaxFileSize = GetFIXDictionaryInt(session, NLOG_MAXFILESIZE_EVENTS, 10);
                            var nLogEventsNumFilesToKeep = GetFIXDictionaryInt(session, NLOG_NUMFILESTOKEEP_EVENTS, 5);

                            SessionID sessionID = GetSessionID(session);
                            sessionsNLog.Add(new SessionNLogConfig(sessionID.Normalize(), fileLogPath, nLogMeggagesMaxFileSize, nLogMessagesNumFilesToKeep, nLogEventsMaxFileSize, nLogEventsNumFilesToKeep));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ConfigError($"ConfigFile {configFile}\r\n{ex.Message}");
            }

            bool GetFIXDictionaryBool(Dictionary dict, string key)
            {
                bool result = false;
                string buff = dict.Has(key) ? dict.GetString(key) : null;
                if (bool.TryParse(buff, out bool _result))
                    result = _result;
                return result;
            }

            string GetFIXDictionaryString(Dictionary dict, string key)
            {
                return dict.Has(key) ? dict.GetString(key) : null;
            }

            int GetFIXDictionaryInt(Dictionary dict, string key, int defaultValue)
            {
                int result = defaultValue;
                string buff = dict.Has(key) ? dict.GetString(key) : null;
                if (int.TryParse(buff, out int _result))
                    result = _result;
                return result;
            }

            SessionID GetSessionID(Dictionary session)
            {
                var beginString = GetFIXDictionaryString(session, "BeginString");
                var senderCompID = GetFIXDictionaryString(session, "SenderCompID");
                var sessionQualifier = GetFIXDictionaryString(session, "SessionQualifier");
                var senderSubID = GetFIXDictionaryString(session, "SenderSubID");
                var senderLocationID = GetFIXDictionaryString(session, "SenderLocationID");
                var targetCompID = GetFIXDictionaryString(session, "TargetCompID");
                var targetSubID = GetFIXDictionaryString(session, "TargetSubID");
                var targetLocationID = GetFIXDictionaryString(session, "TargetLocationID");
                SessionID sessionID = new SessionID(beginString, senderCompID, senderSubID, senderLocationID, targetCompID, targetSubID, targetLocationID, sessionQualifier);
                return sessionID;
            }

            //var dict = settings.Get();
            //var nLogConfigFile = dict.Has(NLOG_CONFIG_FILE) ? dict.GetString(NLOG_CONFIG_FILE) : null;
            //if (System.IO.File.Exists(nLogConfigFile))
            //    return;

            //            //    var MessagesMaxFileSize = dict.Has(LOG4NET_MAXFILESIZE_MSGS) ? dict.GetString(LOG4NET_MAXFILESIZE_MSGS) : "10MB";
            //            //    var MessagesNumFilesToKeep = dict.Has(LOG4NET_NUMFILESTOKEEP_MSGS) ? dict.GetInt(LOG4NET_NUMFILESTOKEEP_MSGS) : 10;
            //            //    var EventsMaxFileSize = dict.Has(LOG4NET_MAXFILESIZE_EVTS) ? dict.GetString(LOG4NET_MAXFILESIZE_EVTS) : "5MB";
            //            //    var EventsNumFilesToKeep = dict.Has(LOG4NET_NUMFILESTOKEEP_EVTS) ? dict.GetInt(LOG4NET_NUMFILESTOKEEP_EVTS) : 10;
            //            //    var CreateXmlConfigFile = dict.Has(LOG4NET_DEBUG_CREATEXMLCONFIGFILE) ? dict.GetBool(LOG4NET_DEBUG_CREATEXMLCONFIGFILE) : false;
            //            //    var UseConsole = dict.Has(LOG4NET_DEBUG_USECONSOLE) ? dict.GetBool(LOG4NET_DEBUG_USECONSOLE) : false;
            //            //    var ConversionPattern = dict.Has(LOG4NET_CONVERSIONPATTERN) ? dict.GetString(LOG4NET_CONVERSIONPATTERN) : "%date% %message%newline";
            //            //    var sessions = settings.GetSessions();

            //            //    foreach (var sessionID in sessions)
            //            //    {
            //            //        Appenders.Add(new AppenderConfig(sessionID.ToString())
            //            //        {
            //            //            LogDirectory = LogDirectory,
            //            //            MessagesMaxFileSize = MessagesMaxFileSize,
            //            //            MessagesNumFilesToKeep = MessagesNumFilesToKeep,
            //            //            EventsMaxFileSize = EventsMaxFileSize,
            //            //            EventsNumFilesToKeep = EventsNumFilesToKeep,
            //            //            ConversionPattern = ConversionPattern
            //            //        });
            //            //    }

            //            //    SetLog4NetXmlElement(Log4NetConfigBuilder.Build(Appenders, UseConsole, CreateXmlConfigFile).ToString());
            //            //}

            //            //private void SetLog4NetXmlElement(string strXml)
            //            //{
            //            //    var doc = new XmlDocument();
            //            //    doc.LoadXml(strXml);
            //            //    var root = doc.DocumentElement;
            //            //    XML = (XmlElement)root.GetElementsByTagName("log4net")[0];
            //            //}
        }
    }
}




//using QuickFix;
//using System.Collections.Generic;
//using System.Xml;

//namespace AD.FIX.Log4Net
//{
//    public class LoggerConfig
//    {


//        public XmlElement XML;

//        public LoggerConfig(SessionSettings settings)
//        {
//            List<AppenderConfig> Appenders = new List<AppenderConfig>();

//            var dict = settings.Get();
//            var LogDirectory = dict.Has(LOG4NET_FILELOGPATH) ? dict.GetString(LOG4NET_FILELOGPATH) : null;
//            if (LogDirectory == null)
//                return;

//            var MessagesMaxFileSize = dict.Has(LOG4NET_MAXFILESIZE_MSGS) ? dict.GetString(LOG4NET_MAXFILESIZE_MSGS) : "10MB";
//            var MessagesNumFilesToKeep = dict.Has(LOG4NET_NUMFILESTOKEEP_MSGS) ? dict.GetInt(LOG4NET_NUMFILESTOKEEP_MSGS) : 10;
//            var EventsMaxFileSize = dict.Has(LOG4NET_MAXFILESIZE_EVTS) ? dict.GetString(LOG4NET_MAXFILESIZE_EVTS) : "5MB";
//            var EventsNumFilesToKeep = dict.Has(LOG4NET_NUMFILESTOKEEP_EVTS) ? dict.GetInt(LOG4NET_NUMFILESTOKEEP_EVTS) : 10;
//            var CreateXmlConfigFile = dict.Has(LOG4NET_DEBUG_CREATEXMLCONFIGFILE) ? dict.GetBool(LOG4NET_DEBUG_CREATEXMLCONFIGFILE) : false;
//            var UseConsole = dict.Has(LOG4NET_DEBUG_USECONSOLE) ? dict.GetBool(LOG4NET_DEBUG_USECONSOLE) : false;
//            var ConversionPattern = dict.Has(LOG4NET_CONVERSIONPATTERN) ? dict.GetString(LOG4NET_CONVERSIONPATTERN) : "%date% %message%newline";
//            var sessions = settings.GetSessions();

//            foreach (var sessionID in sessions)
//            {
//                Appenders.Add(new AppenderConfig(sessionID.ToString())
//                {
//                    LogDirectory = LogDirectory,
//                    MessagesMaxFileSize = MessagesMaxFileSize,
//                    MessagesNumFilesToKeep = MessagesNumFilesToKeep,
//                    EventsMaxFileSize = EventsMaxFileSize,
//                    EventsNumFilesToKeep = EventsNumFilesToKeep,
//                    ConversionPattern = ConversionPattern
//                });
//            }

//            SetLog4NetXmlElement(Log4NetConfigBuilder.Build(Appenders, UseConsole, CreateXmlConfigFile).ToString());
//        }

//        private void SetLog4NetXmlElement(string strXml)
//        {
//            var doc = new XmlDocument();
//            doc.LoadXml(strXml);
//            var root = doc.DocumentElement;
//            XML = (XmlElement)root.GetElementsByTagName("log4net")[0];
//        }
//    }
//}
