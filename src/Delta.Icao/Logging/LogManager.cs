using System;
using Delta.CapiNet.Logging;

namespace Delta.Icao.Logging
{
    internal static class LogManager
    {
        private sealed class LogServiceWrapper : ILogService
        {
            private readonly CapiNetLogger.ILogService originalLogger;

            public LogServiceWrapper(CapiNetLogger.ILogService logger) => originalLogger = logger;

            public Type Type => originalLogger.Type;

            public void Log(string level, string message, Exception exception)
            {
                try
                {
                    originalLogger.Log(level, message, exception);
                }
                catch { /* Nothing to do here */ }
            }
        }

        public static ILogService GetLogger<T>() => GetLogger(typeof(T));

        public static ILogService GetLogger(Type type)
        {
            var service = CapiNetLogger.LogServiceBuilder(type);
            return new LogServiceWrapper(service);
        }
    }
}
