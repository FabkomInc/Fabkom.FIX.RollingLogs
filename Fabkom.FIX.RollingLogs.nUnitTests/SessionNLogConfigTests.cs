using Fabkom.FIX.RollingLogs;
using NUnit.Framework;
using QuickFix.DataDictionary;
using QuickFix.Fields;
using QuickFix;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework.Internal;
using System.Diagnostics;
using System.Xml.Linq;
using System.Runtime.Intrinsics.X86;

namespace Fabkom.FIX.RollingLogs.nUnitTests
{
    public class TestFIXSession
    {
        public string TargetCompID { get; set; }
        public string SenderCompID { get; set; }
        public int SocketAcceptPort { get; set; }
        public TestFIXSession(string targetCompID, string senderCompID, int socketAcceptPort)
        {
            TargetCompID = targetCompID;
            SenderCompID = senderCompID;
            SocketAcceptPort = socketAcceptPort;
        }
        public string SessionID
        {
            get
            {
                return new SessionID(ExtentionsTests.BEGIN_STRING, SenderCompID, TargetCompID).Normalize();
            }
        }
    }
    class TestData
    {
        public static List<TestFIXSession>? configSessions = new();
        public static List<ConfigTests>? configTests = new();
    }
    class ConfigFileLimits
    {
        public int? MessagesMaxFileSize { get; set; }
        public int? MessagesNumFilesToKeep { get; set; }
        public int? EventsMaxFileSize { get; set; }
        public int? EventsNumFilesToKeep { get; set; }

        public ConfigFileLimits(int? maxFileSize_Messages, int? numFilesToKeep_Messages, int? maxFileSize_Events, int? numFilesToKeep_Events)
        {
            MessagesMaxFileSize = maxFileSize_Messages;
            MessagesNumFilesToKeep = numFilesToKeep_Messages;
            EventsMaxFileSize = maxFileSize_Events;
            EventsNumFilesToKeep = numFilesToKeep_Events;
        }
    }
    class ConfigDefaultSection
    {
        public readonly bool? UseInternalLogger;
        public readonly bool? UseConsole;
        public readonly ConfigFileLimits? ConfigFileLimits;
        public ConfigDefaultSection(bool? useInternalLogger, bool? useConsole, ConfigFileLimits? configFilelimits)
        {
            UseInternalLogger = useInternalLogger;
            UseConsole = useConsole;
            ConfigFileLimits = configFilelimits;
        }
    }
    class ConfigSession
    {
        public readonly TestFIXSession? testFIXSession;
        public readonly ConfigFileLimits? configFileLimits;
        public ConfigSession(TestFIXSession? testFIXSession_, ConfigFileLimits? configFileLimits_)
        {
            configFileLimits = configFileLimits_;
            testFIXSession = testFIXSession_;
        }
    }
    class SessionsTest
    {
        public List<ConfigSession> sessions = new();
    }

    class ConfigTests
    {
        public readonly ConfigDefaultSection? configDefaultSection;
        public readonly SessionsTest? sessionsTests;
        public readonly string testFileName;
        public ConfigTests(ConfigDefaultSection? cds_, SessionsTest? sessionsTests_, string testFileName_)
        {
            configDefaultSection = cds_;
            sessionsTests = sessionsTests_;
            testFileName = testFileName_;
        }
    }

    [SetUpFixture]
    public class TestsSetupClass
    {

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            int testCount = 0;
            TestData.configTests ??= new List<ConfigTests>();
            TestData.configSessions ??= new List<TestFIXSession>();
            TestData.configSessions.Add(new TestFIXSession("FABKOM_1", "MDF_1", 12345));
            TestData.configSessions.Add(new TestFIXSession("FABKOM_2", "MDF_2", 23456));
            TestData.configSessions.Add(new TestFIXSession("FABKOM_3", "MDF_3", 34567));

            string testFileName;

            ConfigDefaultSection? configDefaultSession = null;
            ConfigFileLimits? configFileLimits = null;
            SessionsTest? sessionsTest = null;


            #region [Test]
            // No NLog configuration provided.
            // UseInternalLogger = false
            // UseConsole = false
            // Use all defaults for Messages and Events files.
            testFileName = $"Test_{++testCount:D2}.AllDefaults.cfg";

            sessionsTest = new SessionsTest
            {
                sessions = new List<ConfigSession>
                {
                    new ConfigSession(TestData.configSessions[0], configFileLimits),
                    new ConfigSession(TestData.configSessions[1], configFileLimits),
                    new ConfigSession(TestData.configSessions[2], configFileLimits)
                }
            };

            TestData.configTests.Add(new ConfigTests(configDefaultSession, sessionsTest, testFileName));
            #endregion

            #region [Test]
            // UseInternalLogger = true
            // UseConsole = false
            // Use all defaults for Messages and Events files.
            testFileName = $"Test_{++testCount:D2}.AllDefaults_UseInternalLogger_NoConsole.cfg";
            bool useInternalLogger = true;
            bool useConsole = false;
            configDefaultSession = new ConfigDefaultSection(useInternalLogger, useConsole, configFileLimits);
            sessionsTest = new SessionsTest
            {
                sessions = new List<ConfigSession>
                {
                    new ConfigSession(TestData.configSessions[0], configFileLimits),
                    new ConfigSession(TestData.configSessions[1], configFileLimits),
                    new ConfigSession(TestData.configSessions[2], configFileLimits)
                }
            };

            TestData.configTests.Add(new ConfigTests(configDefaultSession, sessionsTest, testFileName));
            #endregion


            #region [Test]
            // UseInternalLogger = true
            // UseConsole = true
            // Use all defaults for Messages and Events files.
            testFileName = $"Test_{++testCount:D2}.AllDefaults_UseInternalLogger_UseConsole.cfg";
            useInternalLogger = true;
            useConsole = true;
            configDefaultSession = new ConfigDefaultSection(useInternalLogger, useConsole, configFileLimits);
            sessionsTest = new SessionsTest
            {
                sessions = new List<ConfigSession>
                {
                    new ConfigSession(TestData.configSessions[0], configFileLimits),
                    new ConfigSession(TestData.configSessions[1], configFileLimits),
                    new ConfigSession(TestData.configSessions[2], configFileLimits)
                }
            };

            TestData.configTests.Add(new ConfigTests(configDefaultSession, sessionsTest, testFileName));
            #endregion

            #region [Test]
            // UseInternalLogger = true
            // UseConsole = false
            // Use custom values for Messages and Events files.
            testFileName = $"Test_{++testCount:D2}.UseInternalLogger_NoConsole_GlobalFileLimits.cfg";
            useInternalLogger = true;
            useConsole = false;
            configDefaultSession = new ConfigDefaultSection(useInternalLogger, useConsole, new ConfigFileLimits(50, 10, 5, 3));

            sessionsTest = new SessionsTest
            {
                sessions = new List<ConfigSession>
                {
                    new ConfigSession(TestData.configSessions[0], configFileLimits),
                    new ConfigSession(TestData.configSessions[1], configFileLimits),
                    new ConfigSession(TestData.configSessions[2], configFileLimits)
                }
            };

            TestData.configTests.Add(new ConfigTests(configDefaultSession, sessionsTest, testFileName));
            #endregion


            #region [Test]
            // UseInternalLogger = false
            // UseConsole = false
            // Use custom global values for Messages and Events files.
            testFileName = $"Test_{++testCount:D2}.UseInternalLogger_NoConsole_GlobalFileLimits.cfg";
            useInternalLogger = true;
            useConsole = false;
            configDefaultSession = new ConfigDefaultSection(useInternalLogger, useConsole, new ConfigFileLimits(50, 10, 5, 3));

            sessionsTest = new SessionsTest
            {
                sessions = new List<ConfigSession>
                {
                    new ConfigSession(TestData.configSessions[0], configFileLimits),
                    new ConfigSession(TestData.configSessions[1], configFileLimits),
                    new ConfigSession(TestData.configSessions[2], configFileLimits)
                }
            };

            TestData.configTests.Add(new ConfigTests(configDefaultSession, sessionsTest, testFileName));
            #endregion

            #region [Test]
            // UseInternalLogger = false
            // UseConsole = false
            // Use custom values for Session Messages and Events files.
            testFileName = $"Test_{++testCount:D2}.UseInternalLogger_NoConsole_GlobalFileLimits.cfg";
            useInternalLogger = true;
            useConsole = false;
            configDefaultSession = new ConfigDefaultSection(useInternalLogger, useConsole, configFileLimits);

            sessionsTest = new SessionsTest
            {
                sessions = new List<ConfigSession>
                {
                    new ConfigSession(TestData.configSessions[0], new ConfigFileLimits(50, 10, 5, 3)),
                    new ConfigSession(TestData.configSessions[1], new ConfigFileLimits(25, 5, 10, 5)),
                    new ConfigSession(TestData.configSessions[2], new ConfigFileLimits(10, 20, 15, 10))
                }
            };

            TestData.configTests.Add(new ConfigTests(configDefaultSession, sessionsTest, testFileName));
            #endregion
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            TestData.configTests = null;
            TestData.configSessions = null;
        }
    }


    [TestFixture]
    public class SessionNLogConfigTests
    {
        [Test]
        public void Test_TestAllConfigs_ShouldPass()
        {
            // Arrange
            // Act
            // Assert

            Directory.GetFiles(GetTestFilesDir()).ToList().ForEach(filePath => { File.Delete(filePath); });

            Assert.Multiple(() =>
            {
                var configTests = TestData.configTests ?? Enumerable.Empty<ConfigTests>();
                foreach (var configTest in configTests)
                {
                    var testFile = CreateTempFIXConfigFile(configTest);
                    Debug.WriteLine(testFile);
                    var nLogConfig = new NLogConfig(testFile);
                    var result = Compare(configTest, nLogConfig, out StringBuilder sb);
                    Assert.That(result, Is.True, sb.ToString());
                }
            });
        }

        private bool Compare(ConfigTests configTest, NLogConfig nLogConfig, out StringBuilder sb)
        {
            bool result = true;
            sb = new StringBuilder();

            var configDefaultSection_Test = configTest.configDefaultSection ?? new ConfigDefaultSection(false, false, null);
            var useInternalLogger_Test = configDefaultSection_Test.UseInternalLogger ?? false;
            var useConsole_Test = configDefaultSection_Test.UseConsole ?? false;

            var internalLogger_Processed = nLogConfig.internalLoggerConfig ?? new NLogInternalLoggerConfig(false);
            var useInternalLogger_Processed = internalLogger_Processed.UseInternalLogger;
            var autoReload_Processed = internalLogger_Processed.AutoReload;
            var throwExceptions_Processed = internalLogger_Processed.ThrowExceptions;
            var throwConfigExceptions_Processed = internalLogger_Processed.ThrowConfigExceptions;
            var logToConsole_Processed = internalLogger_Processed.LogToConsole;
            var logFile_Processed = internalLogger_Processed.LogFile;
            var logLevel_Processed = internalLogger_Processed.LogLevel;

            if (useInternalLogger_Test != useInternalLogger_Processed)
            {
                sb.AppendLine("<UseInternalLogger> mismatch.");
                result = false;
            }

            if (useConsole_Test != logToConsole_Processed)
            {
                sb.AppendLine("<UseConsole> mismatch.");
                result = false;
            }

            if (useInternalLogger_Test == true)
            {
                if (autoReload_Processed != true)
                {
                    sb.AppendLine("<AutoReload> mismatch.");
                    result = false;
                }
                if (throwExceptions_Processed != true)
                {
                    sb.AppendLine("<ThrowExceptions> mismatch.");
                    result = false;
                }
                if (throwConfigExceptions_Processed != true)
                {
                    sb.AppendLine("<ThrowConfigExceptions> mismatch.");
                    result = false;
                }
                if (Path.GetFileName(logFile_Processed ?? "").ToLower() != NLogConfig.NLOG_INTERNAL_LOG)
                {
                    sb.AppendLine($"<LogFile> mismatch. {(logFile_Processed ?? "")} does not contain `{NLogConfig.NLOG_INTERNAL_LOG}`");
                    result = false;
                }
                if ((logLevel_Processed ?? "").ToUpper() != "TRACE")
                {
                    sb.AppendLine($"<LogLevel> mismatch. {(logLevel_Processed ?? "").ToUpper()} != `TRACE`");
                    result = false;
                }
            }

            var configFileLimitsGlobal_Test = configDefaultSection_Test.ConfigFileLimits ?? new ConfigFileLimits(NLogConfig.NLOG_MAXFILESIZE_MESSAGES_DEFAULT, NLogConfig.NLOG_NUMFILESTOKEEP_MESSAGES_DEFAULT, NLogConfig.NLOG_MAXFILESIZE_EVENTS_DEFAULT, NLogConfig.NLOG_NUMFILESTOKEEP_EVENTS_DEFAULT);
            configFileLimitsGlobal_Test.MessagesMaxFileSize ??= NLogConfig.NLOG_MAXFILESIZE_MESSAGES_DEFAULT;
            configFileLimitsGlobal_Test.MessagesNumFilesToKeep ??= NLogConfig.NLOG_NUMFILESTOKEEP_MESSAGES_DEFAULT;
            configFileLimitsGlobal_Test.EventsMaxFileSize ??= NLogConfig.NLOG_MAXFILESIZE_EVENTS_DEFAULT;
            configFileLimitsGlobal_Test.MessagesNumFilesToKeep ??= NLogConfig.NLOG_NUMFILESTOKEEP_EVENTS_DEFAULT;

            if (configTest.sessionsTests is null)
            {
                sb.AppendLine($"<{nameof(configTest.sessionsTests)}> is null.");
                result = false;
                return result;
            }

            var configSessions_Test = configTest.sessionsTests?.sessions ?? new List<ConfigSession>();
            foreach (var configSession_Test in configSessions_Test)
            {
                var configFileLimitsSession_Test = configSession_Test.configFileLimits ?? new ConfigFileLimits(null, null, null, null);
                Merge(configFileLimitsGlobal_Test, configFileLimitsSession_Test);

                var sessionID_Test = configSession_Test?.testFIXSession?.SessionID ?? "";
                var sessionProcessed = nLogConfig.sessionsNLog.FirstOrDefault(s => s.SessionID == sessionID_Test);
                if (sessionProcessed is null)
                {
                    sb.AppendLine($"Unable to find <SessionID> '{sessionID_Test ?? ""} in processed data'.");
                    result = false;
                }
                else
                {
                    if (configFileLimitsSession_Test.MessagesMaxFileSize != sessionProcessed.MessagesMaxFileSize)
                    {
                        sb.AppendLine($"<SessionID> mismatch '{sessionID_Test ?? ""}' MessagesMaxFileSize Test:{configFileLimitsSession_Test.MessagesMaxFileSize} != Processed:{sessionProcessed.MessagesMaxFileSize}.");
                        result = false;
                    }
                    if (configFileLimitsSession_Test.MessagesNumFilesToKeep != sessionProcessed.MessagesNumFilesToKeep)
                    {
                        sb.AppendLine($"<SessionID> mismatch '{sessionID_Test ?? ""}' MessagesNumFilesToKeep Test:{configFileLimitsSession_Test.MessagesNumFilesToKeep} != Processed:{sessionProcessed.MessagesNumFilesToKeep}.");
                        result = false;
                    }
                    if (configFileLimitsSession_Test.EventsMaxFileSize != sessionProcessed.EventsMaxFileSize)
                    {
                        sb.AppendLine($"<SessionID> mismatch '{sessionID_Test ?? ""}' EventsMaxFileSize Test:{configFileLimitsSession_Test.EventsMaxFileSize} != Processed:{sessionProcessed.EventsMaxFileSize}.");
                        result = false;
                    }
                    if (configFileLimitsSession_Test.EventsNumFilesToKeep != sessionProcessed.EventsNumFilesToKeep)
                    {
                        sb.AppendLine($"<SessionID> mismatch '{sessionID_Test ?? ""}' EventsNumFilesToKeep Test:{configFileLimitsSession_Test.EventsNumFilesToKeep} != Processed:{sessionProcessed.EventsNumFilesToKeep}.");
                        result = false;
                    }
                }
            }
            return result;
        }

        private static void Merge(ConfigFileLimits configFileLimitsGlobal_Test, ConfigFileLimits configFileLimitsSession_Test)
        {
            configFileLimitsSession_Test.MessagesMaxFileSize ??= configFileLimitsGlobal_Test.MessagesMaxFileSize;
            configFileLimitsSession_Test.MessagesNumFilesToKeep ??= configFileLimitsGlobal_Test.MessagesNumFilesToKeep;
            configFileLimitsSession_Test.EventsMaxFileSize ??= configFileLimitsGlobal_Test.EventsMaxFileSize;
            configFileLimitsSession_Test.EventsNumFilesToKeep ??= configFileLimitsGlobal_Test.EventsNumFilesToKeep;
        }

        private string CreateTempFIXConfigFile(ConfigTests configTest)
        {
            var testFile = Path.Combine(GetTestFilesDir(), configTest.testFileName);
            var sb = new StringBuilder();
            GetDefault(configTest, sb);
            GetSessions(configTest, sb);
            File.WriteAllText(testFile, sb.ToString());
            return testFile;
        }

        private static void GetDefault(ConfigTests configTest, StringBuilder sb)
        {
            #region [Boilerplate Defaut FIX]
            sb.AppendLine("[DEFAULT]");
            sb.AppendLine("ConnectionType=acceptor");
            sb.AppendLine("ReconnectInterval=1");
            sb.AppendLine("HeartBtInt=60");
            sb.AppendLine("UseLocalTime=Y");
            sb.AppendLine("StartTime=6:30:00");
            sb.AppendLine("EndTime=23:30:00");
            sb.AppendLine("ResetOnLogon=Y");
            sb.AppendLine("ResetOnLogout=Y");
            sb.AppendLine("ResetOnDisconnect=Y");

            sb.AppendLine(@"DataDictionary=.\FIX44-BP.xml");
            sb.AppendLine(@"FileStorePath=.\Store");
            sb.AppendLine(@"FileLogPath=.\Logs");
            sb.AppendLine(@"FileLogBackup=.\LogsBackup");
            sb.AppendLine(@"FileLogDebug=.\LogsDebug");
            #endregion

            if (configTest == null)
                return;

            if (configTest.configDefaultSection == null)
                return;

            var configDefaultSection = configTest.configDefaultSection;
            if (true == configDefaultSection.UseInternalLogger)
            {
                sb.AppendLine(@$"{NLogConfig.NLOG_CONFIGFILE}=.\nlog.config");
                sb.AppendLine($"{NLogConfig.NLOG_DEBUG_USEINTERNALLOGGER}=true");
            }

            if (true == configDefaultSection.UseConsole)
            {
                sb.AppendLine("NLog.Debug.UseConsole=true");
            }

            if (configDefaultSection.ConfigFileLimits == null)
                return;

            var configFileLimits = configDefaultSection.ConfigFileLimits;
            if (configFileLimits.MessagesMaxFileSize.HasValue)
                sb.AppendLine($"{NLogConfig.NLOG_MAXFILESIZE_MESSAGES}={configFileLimits.MessagesMaxFileSize.Value}");
            if (configFileLimits.MessagesNumFilesToKeep.HasValue)
                sb.AppendLine($"{NLogConfig.NLOG_NUMFILESTOKEEP_MESSAGES}={configFileLimits.MessagesNumFilesToKeep.Value}");
            if (configFileLimits.EventsMaxFileSize.HasValue)
                sb.AppendLine($"{NLogConfig.NLOG_MAXFILESIZE_EVENTS}={configFileLimits.EventsMaxFileSize.Value}");
            if (configFileLimits.EventsNumFilesToKeep.HasValue)
                sb.AppendLine($"{NLogConfig.NLOG_NUMFILESTOKEEP_EVENTS}={configFileLimits.EventsNumFilesToKeep.Value}");
        }

        private static void GetSessions(ConfigTests? configTest, StringBuilder sb)
        {
            if (configTest is null)
                throw new ArgumentNullException(nameof(configTest), $"{nameof(ConfigTests)} cannot be null");

            if (configTest.sessionsTests is null)
                throw new ArgumentNullException(nameof(configTest.sessionsTests));

            foreach (var configSession in configTest.sessionsTests.sessions)
            {
                sb.AppendLine();
                sb.AppendLine("[SESSION]");
                sb.AppendLine($"BeginString = {ExtentionsTests.BEGIN_STRING}");

                if (configSession.testFIXSession is null)
                    throw new ArgumentNullException(nameof(configSession.testFIXSession));
                var testFIXSession = configSession.testFIXSession;
                sb.AppendLine($"TargetCompID = {testFIXSession.TargetCompID}");
                sb.AppendLine($"SenderCompID = {testFIXSession.SenderCompID}");
                sb.AppendLine($"SocketAcceptPort = {testFIXSession.SocketAcceptPort}");
                if (configSession.configFileLimits is not null)
                {
                    var configFileLimits = configSession.configFileLimits;
                    if (configFileLimits.MessagesMaxFileSize.HasValue)
                        sb.AppendLine($"{NLogConfig.NLOG_MAXFILESIZE_MESSAGES}={configFileLimits.MessagesMaxFileSize.Value}");
                    if (configFileLimits.MessagesNumFilesToKeep.HasValue)
                        sb.AppendLine($"{NLogConfig.NLOG_NUMFILESTOKEEP_MESSAGES}={configFileLimits.MessagesNumFilesToKeep.Value}");
                    if (configFileLimits.EventsMaxFileSize.HasValue)
                        sb.AppendLine($"{NLogConfig.NLOG_MAXFILESIZE_EVENTS}={configFileLimits.EventsMaxFileSize.Value}");
                    if (configFileLimits.EventsNumFilesToKeep.HasValue)
                        sb.AppendLine($"{NLogConfig.NLOG_NUMFILESTOKEEP_EVENTS}={configFileLimits.EventsNumFilesToKeep.Value}");
                }
            }
        }

        private static string GetTestFilesDir()
        {
            var currDir = Directory.GetCurrentDirectory();
            var testConfigFilesDir = Path.Combine(currDir, "TestConfigFiles");

            if (!Directory.Exists(testConfigFilesDir))
            {
                Directory.CreateDirectory(testConfigFilesDir);
            }
            return testConfigFilesDir;
        }
    }
}
