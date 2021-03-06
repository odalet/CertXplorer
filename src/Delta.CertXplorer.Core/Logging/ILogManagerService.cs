using System;

namespace Delta.CertXplorer.Logging
{
    public interface ILogManagerService
    {
        ILogService DefaultLogger { get; }
        ILogService this[Type type] { get; }
        ILogService this[string loggerName] { get; }

        ILogService GetDefaultLogService();
        ILogService GetLogService(Type type);
        ILogService GetLogService(string name);
        void AddLogService(ILogService service);
        void AddLogService(string name, ILogService service);
        ILogService RemoveLogService(Type type);
        ILogService RemoveLogService(string name);
        ILogService SetDefaultLogService(Type type);
        ILogService SetDefaultLogService(string name);
    }
}
