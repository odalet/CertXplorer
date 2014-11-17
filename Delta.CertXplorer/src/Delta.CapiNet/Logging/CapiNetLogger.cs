using System;
using System.Diagnostics;

namespace Delta.CapiNet.Logging
{
    public static class CapiNetLogger
    {
        public interface ILogService
        {
            Type Type { get; }

            void Log(string level, string message, Exception exception);
        }

        public class DefaultLogService : ILogService
        {
            public DefaultLogService(Type type)
            {
                Type = type;
            }

            #region ILogService Members

            public Type Type { get; private set; }

            public void Log(string level, string message, Exception exception)
            {
                if (string.IsNullOrEmpty(level))
                    level = exception == null ? "INFO" : "ERROR";

                if (string.IsNullOrEmpty(message))
                {
                    if (exception == null)
                    {
                        Debug.WriteLine(level);
                        return;
                    }

                    message = exception.Message;
                }

                if (exception == null)
                    Debug.WriteLine(string.Format("{0} - {1}", level, message));
                else Debug.WriteLine(string.Format("{0} - {1}\r\n{2}", level, message, exception));
            }

            #endregion
        }

        private static Func<Type, ILogService> defaultLogServiceBuilder =
            t => new DefaultLogService(t);

        private static Func<Type, ILogService> currentLogServiceBuilder = null;

        public static Func<Type, ILogService> LogServiceBuilder
        {
            get { return currentLogServiceBuilder ?? defaultLogServiceBuilder; }
            set { currentLogServiceBuilder = value; }
        }
    }
}
