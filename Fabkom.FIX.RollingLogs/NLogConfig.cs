using QuickFix;


namespace Fabkom.FIX.RollingLogs
{
    public class NLogConfig
    {
        public static string NLOG_CONFIG_FILE = "NLog.ConfigFile";

        //public static string LOG4NET_DEBUG_CREATEXMLCONFIGFILE = "Log4Net.Debug.CreateXmlConfigFile";
        //public static string LOG4NET_DEBUG_USECONSOLE = "Log4Net.Debug.UseConsole";
        //public static string LOG4NET_FILELOGPATH = "Log4Net.FileLogPath";
        //public static string LOG4NET_MAXFILESIZE_MSGS = "Log4Net.MaxFileSize_MSGS";
        //public static string LOG4NET_NUMFILESTOKEEP_MSGS = "Log4Net.NumFilesToKeep_MSGS";
        //public static string LOG4NET_MAXFILESIZE_EVTS = "Log4Net.MaxFileSize_EVTS";
        //public static string LOG4NET_NUMFILESTOKEEP_EVTS = "Log4Net.NumFilesToKeep_EVTS";
        //public static string LOG4NET_CONVERSIONPATTERN = "Log4Net.ConversionPattern";
        //public static string LOG4NET_ID = "d4610bdacdddde2b4e0a15761c040109";

        //public XmlElement XML;

        public NLogConfig(SessionSettings settings)
        {
            //    List<AppenderConfig> Appenders = new List<AppenderConfig>();

            //    var dict = settings.Get();
            //    var LogDirectory = dict.Has(LOG4NET_FILELOGPATH) ? dict.GetString(LOG4NET_FILELOGPATH) : null;
            //    if (LogDirectory == null)
            //        return;

            //    var MessagesMaxFileSize = dict.Has(LOG4NET_MAXFILESIZE_MSGS) ? dict.GetString(LOG4NET_MAXFILESIZE_MSGS) : "10MB";
            //    var MessagesNumFilesToKeep = dict.Has(LOG4NET_NUMFILESTOKEEP_MSGS) ? dict.GetInt(LOG4NET_NUMFILESTOKEEP_MSGS) : 10;
            //    var EventsMaxFileSize = dict.Has(LOG4NET_MAXFILESIZE_EVTS) ? dict.GetString(LOG4NET_MAXFILESIZE_EVTS) : "5MB";
            //    var EventsNumFilesToKeep = dict.Has(LOG4NET_NUMFILESTOKEEP_EVTS) ? dict.GetInt(LOG4NET_NUMFILESTOKEEP_EVTS) : 10;
            //    var CreateXmlConfigFile = dict.Has(LOG4NET_DEBUG_CREATEXMLCONFIGFILE) ? dict.GetBool(LOG4NET_DEBUG_CREATEXMLCONFIGFILE) : false;
            //    var UseConsole = dict.Has(LOG4NET_DEBUG_USECONSOLE) ? dict.GetBool(LOG4NET_DEBUG_USECONSOLE) : false;
            //    var ConversionPattern = dict.Has(LOG4NET_CONVERSIONPATTERN) ? dict.GetString(LOG4NET_CONVERSIONPATTERN) : "%date% %message%newline";
            //    var sessions = settings.GetSessions();

            //    foreach (var sessionID in sessions)
            //    {
            //        Appenders.Add(new AppenderConfig(sessionID.ToString())
            //        {
            //            LogDirectory = LogDirectory,
            //            MessagesMaxFileSize = MessagesMaxFileSize,
            //            MessagesNumFilesToKeep = MessagesNumFilesToKeep,
            //            EventsMaxFileSize = EventsMaxFileSize,
            //            EventsNumFilesToKeep = EventsNumFilesToKeep,
            //            ConversionPattern = ConversionPattern
            //        });
            //    }

            //    SetLog4NetXmlElement(Log4NetConfigBuilder.Build(Appenders, UseConsole, CreateXmlConfigFile).ToString());
            //}

            //private void SetLog4NetXmlElement(string strXml)
            //{
            //    var doc = new XmlDocument();
            //    doc.LoadXml(strXml);
            //    var root = doc.DocumentElement;
            //    XML = (XmlElement)root.GetElementsByTagName("log4net")[0];
            //}
        }
    }
}
