using NLog.Layouts;
using System.IO;
using System.Reflection;
using System.Text;

namespace Fabkom.FIX.RollingLogs
{
    public class NLogInternalLoggerConfig
    {
        public readonly bool UseInternalLogger = false;
        public bool AutoReload { get; set; }
        public bool ThrowExceptions { get; set; }
        public bool ThrowConfigExceptions { get; set; }
        public bool LogToConsole { get; set; }
        public string LogFile { get; set; }
        public string LogLevel { get; set; }
        public NLogInternalLoggerConfig(bool _useInternalLogger)
        {
            UseInternalLogger = _useInternalLogger;
            AutoReload = false;
            ThrowExceptions = false;
            ThrowConfigExceptions = false;
        }
    }

    public static class NLogXMLConfig
    {
        public const string XMLTRUE = "true";
        public const string XMLFALSE = "false";
        public const string AUTORELOAD = "autoReload";
        public const string THROWEXCEPTIONS = "throwExceptions";
        public const string THROWCONFIGEXCEPTIONS = "throwConfigExceptions";
        public const string INTERNALLOGLEVEL = "internalLogLevel";
        public const string INTERNALLOGLEVEL_DEBUG = "Debug";
        public const string INTERNALLOGLEVEL_TRACE = "Trace";
        public const string INTERNALLOGFILE = "internalLogFile";
        public const string INTERNALLOGTOCONSOLE = "internalLogToConsole";

        public static string CreateNLogXMLConfigFromFIXSessionSettings(NLogConfig logConfig)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<nlog");
            sb.AppendLine("  xmlns =\"http://www.nlog-project.org/schemas/NLog.xsd\"");
            sb.AppendLine("  xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");

            var internalLoggerConfig = FormatInternalLoggerConfig(logConfig.internalLoggerConfig);
            sb.AppendLine(internalLoggerConfig + ">");

            sb.AppendLine("");
            sb.AppendLine("  <targets>");
            sb.AppendLine("    <!-- Useful for debugging -->");
            sb.AppendLine("    <target");
            sb.AppendLine("      xsi:type=\"ColoredConsole\"");
            sb.AppendLine("      name=\"consoleLog\"");
            sb.AppendLine("      layout =\"${longdate} ${message}\" />");

            foreach (var session in logConfig.sessionsNLog)
            {
                var prefix = session.SessionID;
                var logFileNameMessages = System.IO.Path.Combine(session.FileLogPath, prefix + ".messages.current.log");
                var logFileNameEvents = System.IO.Path.Combine(session.FileLogPath, prefix + ".event.current.log");
                var logFileNameMessagesArchive = System.IO.Path.Combine(session.FileLogPath, "Archive", prefix + ".messages.archive.{#####}.log");
                var logFileNameEventsArchive = System.IO.Path.Combine(session.FileLogPath, "Archive", prefix + ".event.archive.{#####}.log");

                sb.AppendLine("    <target");
                sb.AppendLine( "      xsi:type=\"File\"");
                sb.AppendLine($"      name=\"{prefix}_Messages\"");
                sb.AppendLine($"      fileName=\"{logFileNameMessages}\"");
                sb.AppendLine( "      layout =\"${longdate} ${message}\"");
                sb.AppendLine($"      archiveFileName=\"{logFileNameMessagesArchive}\"");
                sb.AppendLine($"      maxArchiveFiles =\"{session.MessagesNumFilesToKeep}\"");
                sb.AppendLine($"      archiveAboveSize =\"{session.MessagesMaxFileSize * 1024 * 1024}\"");
                sb.AppendLine($"      archiveNumbering =\"{NLog.Targets.ArchiveNumberingMode.Rolling}\"/>");


                sb.AppendLine("    <target");
                sb.AppendLine("      xsi:type=\"File\"");
                sb.AppendLine($"      name=\"{prefix}_Events\"");
                sb.AppendLine($"      fileName=\"{logFileNameEvents}\"");
                sb.AppendLine("      layout =\"${longdate} ${message}\"");
                sb.AppendLine($"      archiveFileName=\"{logFileNameEventsArchive}\"");
                sb.AppendLine($"      maxArchiveFiles =\"{session.EventsNumFilesToKeep}\"");
                sb.AppendLine($"      archiveAboveSize =\"{session.EventsMaxFileSize * 1024 * 1024}\"");
                sb.AppendLine($"      archiveNumbering =\"{NLog.Targets.ArchiveNumberingMode.Rolling}\"/>");
            }

            sb.AppendLine("  </targets>");
            sb.AppendLine("");
            sb.AppendLine("  <rules>");
            sb.AppendLine("    <logger");
            sb.AppendLine("      name=\"ColoredConsoleLog\"");
            sb.AppendLine("      minlevel=\"Trace\"");
            sb.AppendLine("      maxlevel=\"Fatal\"");
            sb.AppendLine("      writeTo=\"consoleLog\" />");

            foreach (var session in logConfig.sessionsNLog)
            {
                var prefix = session.SessionID;

                sb.AppendLine("    <logger");
                sb.AppendLine($"      name=\"Logger_{prefix}_Messages\"");
                sb.AppendLine("      minlevel=\"Trace\"");
                sb.AppendLine("      maxlevel=\"Fatal\"");
                sb.AppendLine($"      writeTo=\"{prefix}_Messages\" />");

                sb.AppendLine("    <logger");
                sb.AppendLine($"      name=\"Logger_{prefix}_Events\"");
                sb.AppendLine("      minlevel=\"Trace\"");
                sb.AppendLine("      maxlevel=\"Fatal\"");
                sb.AppendLine($"      writeTo=\"{prefix}_Events\" />");
            }
            sb.AppendLine("  </rules>");

            sb.AppendLine("</nlog>");

            sb.SaveToFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "nlog.config.generated"));

            return sb.ToString();
        }

        private static string FormatInternalLoggerConfig(NLogInternalLoggerConfig nLogInternalLoggerConfig)
        {
            StringBuilder sb = new StringBuilder();
            if (true == nLogInternalLoggerConfig?.UseInternalLogger)
            {
                if (true == nLogInternalLoggerConfig?.AutoReload)
                    sb.AppendLine($"  {AUTORELOAD}=\"{XMLTRUE}\"");

                if (true == nLogInternalLoggerConfig?.ThrowExceptions)
                    sb.AppendLine($"  {THROWEXCEPTIONS}=\"{XMLTRUE}\"");

                if (true == nLogInternalLoggerConfig?.ThrowConfigExceptions)
                    sb.AppendLine($"  {THROWCONFIGEXCEPTIONS}=\"{XMLTRUE}\"");

                if (false == string.IsNullOrEmpty(nLogInternalLoggerConfig?.LogLevel))
                    sb.AppendLine($"  {INTERNALLOGLEVEL}=\"{nLogInternalLoggerConfig?.LogLevel}\"");
                else
                    sb.AppendLine($"  {INTERNALLOGLEVEL}=\"{INTERNALLOGLEVEL_TRACE}\"");

                if (true == nLogInternalLoggerConfig?.LogToConsole)
                    sb.AppendLine($"  {INTERNALLOGTOCONSOLE}=\"{XMLTRUE}\"");

                if (false == string.IsNullOrEmpty(nLogInternalLoggerConfig?.LogFile))
                    sb.AppendLine($"  {INTERNALLOGFILE}=\"{nLogInternalLoggerConfig.LogFile}\"");
                else
                    sb.AppendLine($"  {INTERNALLOGFILE}=\"{"${basedir}/App_Data/nlog-internal.log"}\"");
            }
            return sb.ToString().Trim(new char[] { '\r', '\n' });
        }
    }
}

