using NLog.Config;
using NLog.Targets;
using NLog;
using QuickFix;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using NLog.Fluent;
using QuickFix.Fields;
using System.Configuration;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;

namespace Fabkom.FIX.RollingLogs.Cnsl
{

    public class NLogTest
    {
        //public NLog.Logger logger_Messages_1;
        //public NLog.Logger logger_Events_1;

        //public NLog.Logger logger_Messages_2;
        //public NLog.Logger logger_Events_2;

        public NLog.Logger logger_Messages;
        public NLog.Logger logger_Events;

        const long ARCHIVEABOVESIZE_MESSAGES = 10 * 1024;
        const int MAXARCHIVEFILES_MESSAGES = 10;

        const long ARCHIVEABOVESIZE_EVENTS = 1 * 1024;
        const int MAXARCHIVEFILES_EVENTS = 5;

        public void Init(string fileLogPath, string prefix)
        {
            var logFileNameMessages = System.IO.Path.Combine(fileLogPath, prefix + ".msg.log");
            var logFileNameEvents = System.IO.Path.Combine(fileLogPath, prefix + ".evt.log");
            var logArchiveFileNameMessages = System.IO.Path.Combine(fileLogPath, "Archive", prefix + ".msg.{#####}.log");
            var logArchiveFileNameEvents = System.IO.Path.Combine(fileLogPath, "Archive", prefix + ".evt.{#####}.log");

            var targetNameMessages = $"Messages_{prefix}";
            var targetNameEvents = $"Events_{prefix}";
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

            //var targetEvents = new NLog.Targets.FileTarget(targetNameEvents)
            //{
            //    FileName = logFileNameEvents,
            //    Layout = new NLog.Layouts.SimpleLayout("${longdate} ${message}"),
            //    ArchiveFileName = logArchiveFileNameEvents,
            //    ArchiveAboveSize = ARCHIVEABOVESIZE_EVENTS,
            //    ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Rolling,
            //    ConcurrentWrites = true,
            //    KeepFileOpen = false,
            //    MaxArchiveFiles = MAXARCHIVEFILES_EVENTS
            //};
            //config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, targetEvents);
            //config.AddTarget(targetNameEvents, targetEvents);

            NLog.LogManager.Configuration.AddTarget(targetNameMessages, targetMessages);
            //NLog.LogManager.Configuration.AddTarget(targetNameEvents, targetEvents);

            NLog.LogManager.Configuration.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, targetMessages);
            //NLog.LogManager.Configuration.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, targetEvents);

            logger_Messages = NLog.LogManager.GetLogger(targetNameMessages);
            //logger_Events = NLog.LogManager.GetLogger(targetNameEvents);
        }
    }

    internal class Program
    {

        public static string FileToReplay = @"C:\DEV\Fabkom.MDF\Fabkom.MDF.Replay\ReplayData\FIX.4.4-NORTHERNCAPFBMD-VBOND.messages.current.log";
        public static string FIXConfigFile = @"C:\DEV\FIX\Fabkom.FIX.RollingLogs\Fabkom.FIX.RollingLogs.Cnsl\FIX\FIXConfig\Fabkom.FIX.RollingLogs.cfg";

        private static FIXLogReplayer fixLogReplayer;

        private static void TestNLog()
        {
            var path = @"C:\DEV\FIX\Fabkom.FIX.RollingLogs\Fabkom.FIX.RollingLogs.Cnsl\Logs\";
            var messageLogFileName = Path.Combine(path, "Messages.log");
            var messageLogArchiveFileName = Path.Combine(path, @"Archive", "Messages.{#####}.log");

            var eventLogFileName = Path.Combine(path, "Events.log");
            var eventLogArchiveFileName = Path.Combine(path, @"Archive", "Events.{#####}.log");

            var config = new NLog.Config.LoggingConfiguration();

            var targetMessages = new NLog.Targets.FileTarget("Messages")
            {
                FileName = messageLogFileName,
                Layout = new NLog.Layouts.SimpleLayout("${longdate} ${message}"),
                ArchiveFileName = messageLogArchiveFileName,
                ArchiveAboveSize = 500_000,
                ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Rolling,
                ConcurrentWrites = true,
                KeepFileOpen = false,
                MaxArchiveFiles = 10
            };
            config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, targetMessages);
            config.AddTarget("Messages", targetMessages);

            var targetEvents = new NLog.Targets.FileTarget("Events")
            {
                FileName = eventLogFileName,
                Layout = new NLog.Layouts.SimpleLayout("${longdate} ${message}"),
                ArchiveFileName = eventLogArchiveFileName,
                ArchiveAboveSize = 500_000,
                ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Rolling,
                ConcurrentWrites = true,
                KeepFileOpen = false,
                MaxArchiveFiles = 10
            };
            config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, targetEvents);
            config.AddTarget("Events", targetEvents);

            NLog.LogManager.Configuration = config;

            var logger_Messages = NLog.LogManager.GetLogger("Messages");
            var logger_Events = NLog.LogManager.GetLogger("Events");

            Task.Run(() =>
            {
                while (true)
                {
                    var s = $"{DateTime.Now.ToFileTimeUtc()} {new string('A', 1024)}";
                    logger_Messages.Debug(s);
                    logger_Messages.Info(s);
                    logger_Messages.Warn(s);
                    logger_Messages.Error(s);
                    logger_Messages.Fatal(s);
                    Thread.Sleep(50);
                }
            });
            Task.Run(() =>
            {
                while (true)
                {
                    var s = $"{DateTime.Now.ToFileTimeUtc()} {new string('A', 1024)}";
                    logger_Events.Debug(s);
                    logger_Events.Info(s);
                    logger_Events.Warn(s);
                    logger_Events.Error(s);
                    logger_Events.Fatal(s);
                    Thread.Sleep(50);
                }
            });
        }
        //private static void EnableNLogSelfDiagLogging()
        //{
        //    NLog.LogManager.ThrowExceptions = true;
        //    NLog.LogManager.ThrowConfigExceptions = true;
        //    // enable internal logging to the console
        //    NLog.Common.InternalLogger.LogToConsole = true;

        //    // enable internal logging to a file
        //    NLog.Common.InternalLogger.LogFile = @"C:\DEV\Fabkom.MDF.Replay\FIX\Logs\NLog-internal.log"; // On Linux one can use "/home/nlog-internal.txt"

        //    // enable internal logging to a custom TextWriter
        //    //NLog.Common.InternalLogger.LogWriter = new StringWriter(); //e.g. TextWriter writer = File.CreateText("C:\\perl.txt")

        //    // set internal log level
        //    NLog.Common.InternalLogger.LogLevel = NLog.LogLevel.Debug;

        //    // Perform test output, ensure first NLog Logger is created after InternalLogger is enabled.
        //    //NLog.LogManager.GetLogger("Test").Info("Hello World");
        //}

        private static System.Timers.Timer StatsTimer;
        public static Stopwatch watch = new Stopwatch();
        private static void StartStatsTimer()
        {
            StatsTimer = new System.Timers.Timer(1000);
            StatsTimer.Elapsed += OnStatsTimer;
            StatsTimer.AutoReset = true;
            StatsTimer.Enabled = true;
        }
        private static void OnStatsTimer(object sender, ElapsedEventArgs e)
        {
            long elapsedSec = 1 + (watch.ElapsedMilliseconds / 1000);
            long replRecs = Interlocked.Read(ref fixLogReplayer.ReplayedRecordCount);
            //Console.SetCursorPosition(0, Console.CursorTop == 0 ? 0 : 12);
            Console.WriteLine($"Records Replayed: {replRecs}  Total Run Time: {watch.Elapsed:hh\\:mm\\:ss} Speed {(int)(replRecs / (elapsedSec * 1.0))} rec/sec.");
        }
        private static void StopStatsTimer()
        {
            StatsTimer.Stop();
            StatsTimer.Dispose();
        }


        /*

namespace Fabkom.FIX.RollingLogs
{
    public class NLogLog : ILog
    {
  

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

         
         
         
         */

        private static LoggingConfiguration config;

        static void Main(string[] args)
        {
            //// TestNLog();
            ////EnableNLogSelfDiagLogging();
            ////InitNLog_Use_LoggingConfiguration();
            ////InitNLog_Use_LogManager_Configuration();

            //SessionSettings settings = new SessionSettings(FIXConfigFile);
            //NLogInternalLoggerConfig internalLoggerConfig = new NLogInternalLoggerConfig(true)
            //{
            //    AutoReload = true,
            //    ThrowExceptions = true,
            //    ThrowConfigExceptions = true,
            //    LogToConsole = true,
            //    LogFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "nlog-internal.log"),
            //    LogLevel = NLogXMLConfig.INTERNALLOGLEVEL_DEBUG,
            //};

            //string xmlNLogConfig;
            //xmlNLogConfig = NLogXMLConfig.CreateNLogXMLConfigFromFIXSessionSettings(internalLoggerConfig, settings);
            ////xmlNLogConfig = File.ReadAllText(@"C:\DEV\Fabkom.MDF.Replay\NLog\nlog.config");
            //NLog.LogManager.Configuration = XmlLoggingConfiguration.CreateFromXmlString(xmlNLogConfig);

            //var loger_CCL = NLog.LogManager.GetLogger("ColoredConsoleLog");
            //var logger1_Messages = NLog.LogManager.GetLogger("Logger_FIX.4.4-MDF-FABKOM_Messages");
            //var logger1_Events = NLog.LogManager.GetLogger("Logger_FIX.4.4-MDF-FABKOM_Events");
            //var logger2_Messages = NLog.LogManager.GetLogger("Logger_FIX.4.4-MDF2-FABKOM2_Messages");
            //var logger2_Events = NLog.LogManager.GetLogger("Logger_FIX.4.4-MDF2-FABKOM2_Events");

            ////var nLogTest = new NLogTest();
            ////nLogTest.Init(@"C:\DEV\Fabkom.MDF.Replay\FIX\Logs", "1111");
            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        logger1_Messages.Debug("Only <<<Messages>>> for 11111111111111 go here.");
            //        logger1_Events.Debug("Only <<<Events>>> for 11111111111111 go here.");
            //        logger2_Messages.Debug("Only <<<Messages>>> for 22222222222222 go here.");
            //        logger2_Events.Debug("Only <<<Events>>> for 22222222222222 go here.");

            //        loger_CCL.Trace("Trace");
            //        loger_CCL.Debug("Debug");
            //        loger_CCL.Info("Info");
            //        loger_CCL.Warn("Warn");
            //        loger_CCL.Error("Error");
            //        loger_CCL.Fatal("Fatal");
            //        Thread.Sleep(1000);
            //    }
            //});

            //FileStream fileStream = File.Open(FIXConfigFile, FileMode.Open, FileAccess.Read);
            //Settings mysettings = new Settings(new StreamReader(fileStream));
            //LinkedList<Dictionary> linkedList = mysettings.Get("DEFAULT");


            SessionSettings settings = new SessionSettings(FIXConfigFile);
            MyQuickFixApp myApp = new MyQuickFixApp();
            IMessageStoreFactory storeFactory = new FileStoreFactory(settings);
            //ILogFactory logFactory = new FileLogFactory(settings);
            ILogFactory logFactory = new NLogLogFactory(settings, FIXConfigFile);

            var _settings = settings.Get();

            string dd = _settings.GetString(SessionSettings.DATA_DICTIONARY);
            FIXUtils.LoadDictionary(dd);
            ThreadedSocketAcceptor acceptor = new ThreadedSocketAcceptor(myApp, storeFactory, settings, logFactory);
            acceptor.Start();

            while (!myApp.isLoggedIn)
            {
                Console.WriteLine($"Checking for LoggedOn every 5000 ms. {DateTime.Now:HH:mm:ss}.");
                Thread.Sleep(5000);
            }

            fixLogReplayer = new FIXLogReplayer(FileToReplay, dd, myApp.sessions_[0]);
            Console.Clear();

            StartStatsTimer();
            Task.Factory.StartNew(() =>
            {
                watch = Stopwatch.StartNew();
                //fixLogReplayer.ReadFile_AllLines_Convert();
                fixLogReplayer.ReadFile_AllLines_PLINQ_Convert();
                watch.Stop();
            }).Wait();
            StopStatsTimer();

            Console.WriteLine("All Done. Hit Esc.");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                Thread.Sleep(100);
            }

            //acceptor.Stop();
            //acceptor = null;

            return;

        }

       

        private static void InitNLog_Use_LoggingConfiguration()
        {
            var config = new LoggingConfiguration();

            var fileTargetMessages = new FileTarget("TargetMessages")
            {
                FileName = @"C:\DEV\Fabkom.MDF.Replay\FIX\Logs\msg.log",
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}",
            };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTargetMessages, "*Message*", true);

            var fileTargetEvents = new FileTarget("TargetEvents")
            {
                FileName = @"C:\DEV\Fabkom.MDF.Replay\FIX\Logs\evt.log",
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}",
            };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTargetEvents, "*Event*", true);

            LogManager.Configuration = config;
        }


        private static void InitNLog_Use_LogManager_Configuration()
        {
            LogManager.Configuration = LogManager.Configuration ?? new LoggingConfiguration();

            var fileTargetMessages = new FileTarget("TargetMessages")
            {
                FileName = @"C:\DEV\Fabkom.MDF.Replay\FIX\Logs\msg.log",
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}",
            };
            LogManager.Configuration.AddTarget("fileTargetMessages", fileTargetMessages);
            LogManager.Configuration.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTargetMessages, "*Message*");
            LogManager.Configuration.Reload();

            var fileTargetEvents = new FileTarget("TargetEvents")
            {
                FileName = @"C:\DEV\Fabkom.MDF.Replay\FIX\Logs\evt.log",
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}",
            };
            LogManager.Configuration.AddTarget("fileTargetEvents", fileTargetEvents);
            LogManager.Configuration.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTargetEvents, "*Event*");
            LogManager.Configuration.Reload();
        }
    }
}
