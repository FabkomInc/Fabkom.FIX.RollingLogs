using NLog.Config;
using QuickFix;
using System.IO;

using System.Reflection;

namespace Fabkom.FIX.RollingLogs
{
    public class NLogLogFactory : ILogFactory
    {
        SessionSettings settings_;

        public NLogLogFactory(SessionSettings settings, string configFile)
        {
            settings_ = settings;

            NLogConfig nLogConfig = new NLogConfig(configFile);

            NLogInternalLoggerConfig internalLoggerConfig = new NLogInternalLoggerConfig(true)
            {
                AutoReload = true,
                ThrowExceptions = true,
                ThrowConfigExceptions = true,
                LogToConsole = true,
                LogFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "nlog-internal.log"),
                LogLevel = NLogXMLConfig.INTERNALLOGLEVEL_DEBUG,
            };


            string xmlNLogConfig = NLogXMLConfig.CreateNLogXMLConfigFromFIXSessionSettings(internalLoggerConfig, settings);
            NLog.LogManager.Configuration = XmlLoggingConfiguration.CreateFromXmlString(xmlNLogConfig);
        }

        public ILog Create(SessionID sessionID)
        {
            return new NLogLog(settings_.Get(sessionID).GetString(SessionSettings.FILE_LOG_PATH), sessionID);
        }
    }
}
