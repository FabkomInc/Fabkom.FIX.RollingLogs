using NLog;
using QuickFix;
using System;
using System.IO;

namespace Fabkom.FIX.RollingLogs
{
    public class NLogLog : ILog
    {
        private NLog.Logger logger_Messages;
        private NLog.Logger logger_Events;

        private string logFileNameMessages;
        private string logFileNameEvents;
        private string logArchiveFileNameMessages;
        private string logArchiveFileNameEvents;

        const long ARCHIVEABOVESIZE_MESSAGES = 100 * 1024 * 1024;
        const int MAXARCHIVEFILES_MESSAGES = 300;

        const long ARCHIVEABOVESIZE_EVENTS = 2 * 1024 * 1024;
        const int MAXARCHIVEFILES_EVENTS = 30;

        public NLogLog(string fileLogPath)
        {
            Init(fileLogPath, "GLOBAL");
        }
        public NLogLog(string fileLogPath, SessionID sessionID)
        {
            Init(fileLogPath, Prefix(sessionID));
        }

        private void Init(string fileLogPath, string prefix)
        {


            if (!System.IO.Directory.Exists(fileLogPath))
                System.IO.Directory.CreateDirectory(fileLogPath);

            logFileNameMessages = System.IO.Path.Combine(fileLogPath, prefix + ".messages.current.log");
            logFileNameEvents = System.IO.Path.Combine(fileLogPath, prefix + ".event.current.log");

            logArchiveFileNameMessages = System.IO.Path.Combine(fileLogPath, "Archive", prefix + ".messages.archived.{#####}.log");
            logArchiveFileNameEvents = System.IO.Path.Combine(fileLogPath, "Archive", prefix + ".event.archived.{#####}.log");

            var targetNameMessages = $"Messages_{prefix}";
            var targetNameEvents = $"Events_{prefix}";

            //var config = new NLog.Config.LoggingConfiguration();
            NLog.LogManager.Configuration = NLog.LogManager.Configuration ?? new NLog.Config.LoggingConfiguration();

            var targetMessages = new NLog.Targets.FileTarget(targetNameMessages)
            {
                FileName = logFileNameMessages,
                Layout = new NLog.Layouts.SimpleLayout("${longdate} ${message}"),
                ArchiveFileName = logArchiveFileNameMessages,
                ArchiveAboveSize = ARCHIVEABOVESIZE_MESSAGES,
                ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Rolling,
                ConcurrentWrites = true,
                KeepFileOpen = false,
                MaxArchiveFiles = MAXARCHIVEFILES_MESSAGES
            };
            //config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, targetMessages);
            //config.AddTarget(targetNameMessages, targetMessages);

            var targetEvents = new NLog.Targets.FileTarget(targetNameEvents)
            {
                FileName = logFileNameEvents,
                Layout = new NLog.Layouts.SimpleLayout("${longdate} ${message}"),
                ArchiveFileName = logArchiveFileNameEvents,
                ArchiveAboveSize = ARCHIVEABOVESIZE_EVENTS,
                ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Rolling,
                ConcurrentWrites = true,
                KeepFileOpen = false,
                MaxArchiveFiles = MAXARCHIVEFILES_EVENTS
            };
            //config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, targetEvents);
            //config.AddTarget(targetNameEvents, targetEvents);

            NLog.LogManager.Configuration.AddTarget(targetNameMessages, targetMessages);
            NLog.LogManager.Configuration.AddTarget(targetNameEvents, targetEvents);

            NLog.LogManager.Configuration.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, targetMessages);
            NLog.LogManager.Configuration.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, targetEvents);

            logger_Messages = NLog.LogManager.GetLogger(targetNameMessages);
            logger_Events = NLog.LogManager.GetLogger(targetNameEvents);
        }

        public static string Prefix(SessionID sessionID)
        {
            System.Text.StringBuilder prefix = new System.Text.StringBuilder(sessionID.BeginString)
                .Append('-').Append(sessionID.SenderCompID);
            if (SessionID.IsSet(sessionID.SenderSubID))
                prefix.Append('_').Append(sessionID.SenderSubID);
            if (SessionID.IsSet(sessionID.SenderLocationID))
                prefix.Append('_').Append(sessionID.SenderLocationID);
            prefix.Append('-').Append(sessionID.TargetCompID);
            if (SessionID.IsSet(sessionID.TargetSubID))
                prefix.Append('_').Append(sessionID.TargetSubID);
            if (SessionID.IsSet(sessionID.TargetLocationID))
                prefix.Append('_').Append(sessionID.TargetLocationID);

            if (SessionID.IsSet(sessionID.SessionQualifier))
                prefix.Append('-').Append(sessionID.SessionQualifier);

            return prefix.ToString();
        }

        public void Clear()
        {
            throw new NotImplementedException("Clear - not implemented.");
        }
        public void Dispose()
        {
            this.Dispose();
        }
        public void OnEvent(string s)
        {
            //Console.WriteLine($"OnEvent {s}");
            logger_Events.Info(s);
        }
        public void OnIncoming(string msg)
        {
            //Console.WriteLine($"OnIncoming {msg}");
            logger_Messages.Info($"<In> {msg}");

        }
        public void OnOutgoing(string msg)
        {
            //Console.WriteLine($"OnOutgoing {msg}");
            logger_Messages.Info($"<Out> {msg}");
        }
    }
}
