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
}
