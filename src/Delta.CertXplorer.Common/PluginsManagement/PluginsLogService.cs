using System;
using Delta.CertXplorer.Logging;

namespace Delta.CertXplorer.PluginsManagement
{
    internal sealed class PluginsLogService : Extensibility.Logging.ILogService
    {
        private readonly ILogService log;
        private readonly string source;

        public PluginsLogService(string pluginName)
        {
            source = $"$.{pluginName}";
            log = This.Logger;
        }

        public void Log(Extensibility.Logging.LogLevel level, string message) => log.Log(GetLevel(level), message, source);
        public void Log(Extensibility.Logging.LogLevel level, string message, Exception exception) => log.Log(GetLevel(level), message, exception, source);
        public void Log(Extensibility.Logging.LogLevel level, Exception exception) => log.Log(GetLevel(level), exception, source);
        public void Log(Extensibility.Logging.LogEntry entry) => log.Log(GetEntry(entry, source));

        private static LogLevel GetLevel(Extensibility.Logging.LogLevel level) => level switch
        {
            Extensibility.Logging.LogLevel.All => LogLevel.All,
            Extensibility.Logging.LogLevel.Debug => LogLevel.Debug,
            Extensibility.Logging.LogLevel.Error => LogLevel.Error,
            Extensibility.Logging.LogLevel.Fatal => LogLevel.Fatal,
            Extensibility.Logging.LogLevel.Info => LogLevel.Info,
            Extensibility.Logging.LogLevel.Off => LogLevel.Off,
            Extensibility.Logging.LogLevel.Verbose => LogLevel.Verbose,
            Extensibility.Logging.LogLevel.Warning => LogLevel.Warning,
            _ => LogLevel.Info,// default
        };

        private static LogEntry GetEntry(Extensibility.Logging.LogEntry entry, string source) => new()
        {
            Exception = entry.Exception,
            Level = GetLevel(entry.Level),
            Message = entry.Message,
            SourceName = source,
            Tag = entry.Tag,
            TimeStamp = entry.TimeStamp
        };
    }
}
