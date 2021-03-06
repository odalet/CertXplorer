using System;

namespace Delta.CertXplorer.Logging
{
    public interface ILogService
    {
        void Log(LogLevel level, string message);
        void Log(LogLevel level, string message, Exception exception);
        void Log(LogLevel level, Exception exception);
        void Log(LogEntry entry);
    }
}
