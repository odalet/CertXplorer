using System;
using Delta.CapiNet.Logging;

namespace Delta.Icao.Logging
{
    internal static class LogManager
    {
        private class LogServiceWrapper : ILogService
        {
            private readonly CapiNetLogger.ILogService originalLogger;

            public LogServiceWrapper(CapiNetLogger.ILogService logger)
            {
                originalLogger = logger;
            }

            #region ILogService Members

            public Type Type
            {
                get { return originalLogger.Type; }
            }

            public void Log(string level, string message, Exception exception)
            {
                try
                {
                    originalLogger.Log(level, message, exception);
                }
                catch { }
            }

            #endregion
        }

        public static ILogService GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        public static ILogService GetLogger(Type type)
        {
            var service = CapiNetLogger.LogServiceBuilder(type);
            return new LogServiceWrapper(service);
        }
    }
}
