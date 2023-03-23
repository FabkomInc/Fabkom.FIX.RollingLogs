using QuickFix;
namespace Fabkom.FIX.RollingLogs
{
    public class NLogLogFactory : ILogFactory
    {
        SessionSettings settings_;

        public NLogLogFactory(SessionSettings settings)
        {
            settings_ = settings;
        }

        public ILog Create(SessionID sessionID)
        {
            return new NLogLog(settings_.Get(sessionID).GetString(SessionSettings.FILE_LOG_PATH), sessionID);
        }
    }
}
