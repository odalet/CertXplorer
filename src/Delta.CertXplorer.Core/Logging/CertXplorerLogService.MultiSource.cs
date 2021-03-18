namespace Delta.CertXplorer.Logging
{
    partial class CertXplorerLogService : IMultiSourceLogService
    {
        private const string defaultSourceName = LogSource.RootSourceName;
        private string currentSourceName = defaultSourceName;

        public ILogService this[string sourceName]
        {
            get
            {
                if (string.IsNullOrEmpty(sourceName)) sourceName = currentSourceName;
                return sourceName == currentSourceName ? this : new CertXplorerLogService { currentSourceName = sourceName };
            }
        }

        public ILogService this[LogSource source]
        {
            get
            {
                if (source == null) return null;
                return source.Name == currentSourceName ? this : this[source.Name];
            }
        }

        public LogSource CurrentSource => new(currentSourceName);
    }
}
