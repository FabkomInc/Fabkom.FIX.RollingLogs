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
}

