using NLog.Config;
using QuickFix;
using System.IO;

using System.Reflection;

namespace Fabkom.FIX.RollingLogs
{
    public class NLogLogFactory : ILogFactory
    {
        SessionSettings settings_;
        NLogConfig nLogConfig;

        public NLogLogFactory(SessionSettings settings, string configFile)
        {
            settings_ = settings;
            nLogConfig = new NLogConfig(configFile);
            string xmlNLogConfig = NLogXMLConfig.CreateNLogXMLConfigFromFIXSessionSettings(nLogConfig);
            NLog.LogManager.Configuration = XmlLoggingConfiguration.CreateFromXmlString(xmlNLogConfig);
        }

        public ILog Create(SessionID sessionID)
        {
            return new NLogLog(settings_.Get(sessionID).GetString(SessionSettings.FILE_LOG_PATH), sessionID, nLogConfig);
        }
    }
}
