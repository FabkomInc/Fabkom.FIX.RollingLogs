using QuickFix;

namespace Fabkom.FIX.RollingLogs
{
    public class NLogLog : ILog
    {
        private NLog.Logger logger_Messages;
        private NLog.Logger logger_Events;
  
        public NLogLog(string fileLogPath)
        {
            Init(fileLogPath, "GLOBAL");
        }
        public NLogLog(string fileLogPath, SessionID sessionID)
        {
            Init(fileLogPath, sessionID.Normalize()));
        }

        private void Init(string fileLogPath, string prefix)
        {
            if (!System.IO.Directory.Exists(fileLogPath))
                System.IO.Directory.CreateDirectory(fileLogPath);

            logger_Messages = NLog.LogManager.GetLogger($"Logger_{prefix}_Messages");
            logger_Events = NLog.LogManager.GetLogger($"Logger_{prefix}_Events");
        }

        public void Clear()
        {
        }
        public void Dispose()
        {
            this.Dispose();
        }
        public void OnEvent(string s)
        {
            logger_Events.Info(s);
        }
        public void OnIncoming(string msg)
        {
            logger_Messages.Info($"<In> {msg}");
        }
        public void OnOutgoing(string msg)
        {
            logger_Messages.Info($"<Out> {msg}");
        }
    }
}
