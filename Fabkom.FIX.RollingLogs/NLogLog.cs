using QuickFix;

namespace Fabkom.FIX.RollingLogs
{
    public class NLogLog : ILog
    {
        private NLog.Logger logger_Messages;
        private NLog.Logger logger_Events;
        private NLog.Logger logger_Console;
        private bool LogToConsole { get; set; } = false;
        public NLogLog(string fileLogPath)
        {
            LogToConsole = false;
            Init(fileLogPath, "GLOBAL");
            
        }
        public NLogLog(string fileLogPath, SessionID sessionID, NLogConfig nLogConfig)
        {
            LogToConsole = nLogConfig?.internalLoggerConfig?.LogToConsole ?? false;
            Init(fileLogPath, sessionID.Normalize());
        }
        private void Init(string fileLogPath, string prefix)
        {
            if (!System.IO.Directory.Exists(fileLogPath))
                System.IO.Directory.CreateDirectory(fileLogPath);

            logger_Messages = NLog.LogManager.GetLogger($"Logger_{prefix}_Messages");
            logger_Events = NLog.LogManager.GetLogger($"Logger_{prefix}_Events");

            if(LogToConsole)
                logger_Console = NLog.LogManager.GetLogger("ColoredConsoleLog");
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
            if(LogToConsole)
                logger_Console.Info(s);
            logger_Events.Info(s);
        }
        public void OnIncoming(string msg)
        {
            if(LogToConsole)
                logger_Console.Info(msg);
            logger_Messages.Info($"<In> {msg}");
        }
        public void OnOutgoing(string msg)
        {
            if (LogToConsole)
                logger_Console.Info(msg);
            logger_Messages.Info($"<Out> {msg}");
        }
    }
}
