namespace Delta.CertXplorer.Logging.log4net
{
    partial class Log4NetService : IMultiSourceLogService
    {
        // In order for other sources to inherit this logger properties, they should begin with "$."
        public const string DefaultSourceName = LogSource.RootSourceName;

        private string currentSourceName = DefaultSourceName;

        public ILogService this[LogSource source] => this[source?.Name];
        public ILogService this[string sourceName] => 
            string.IsNullOrEmpty(sourceName) || sourceName == currentSourceName 
            ? this 
            : new Log4NetService(configurationFileInfo, false) { currentSourceName = sourceName };

        public LogSource CurrentSource => new(currentSourceName);
    }
}
