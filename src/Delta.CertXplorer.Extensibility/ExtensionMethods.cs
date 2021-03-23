using System;
using System.Text;
using Delta.CertXplorer.Extensibility.Logging;

namespace Delta.CertXplorer.Extensibility
{
    public static class ExtensionMethods
    {
        public static string ToFormattedString(this Exception exception)
        {
            if (exception == null) return string.Empty;

            const string tab = "   ";
            var indent = string.Empty;

            var builder = new StringBuilder();
            for (var currentException = exception; currentException != null; currentException = currentException.InnerException)
            {
                _ = builder
                    .Append(indent)
                    .Append(" + ")
                    .Append("[")
                    .Append(currentException.GetType().ToString())
                    .Append("] ")
                    .Append(currentException.Message)
                    .Append(Environment.NewLine)
                    ;

                indent += tab;

                if (currentException.StackTrace != null)
                {
                    var stackTrace = currentException.StackTrace
                        .Replace(Environment.NewLine, "\n")
                        .Split('\n');

                    for (var i = 0; i < stackTrace.Length; i++) _ = builder
                            .Append(indent)
                            .Append(" | ")
                            .Append(stackTrace[i].Trim())
                            .Append(Environment.NewLine)
                            ;
                }
            }

            return builder.ToString();
        }

        public static T GetService<T>(this IServiceProvider serviceProvider) where T : class => serviceProvider.GetService(typeof(T)) as T;

        public static T GetService<T>(this IServiceProvider serviceProvider, bool mandatory) where T : class
        {
            var t = serviceProvider.GetService<T>();
            if (t == null && mandatory)
                throw new ServiceNotFoundException<T>();

            return t;
        }

        public static void Verbose(this ILogService log, string message) => log.Log(LogLevel.Verbose, message);
        public static void Verbose(this ILogService log, Exception exception) => log.Log(LogLevel.Verbose, exception);
        public static void Verbose(this ILogService log, string message, Exception exception) => log.Log(LogLevel.Verbose, message, exception);
     
        public static void Debug(this ILogService log, string message) => log.Log(LogLevel.Debug, message);
        public static void Debug(this ILogService log, Exception exception) => log.Log(LogLevel.Debug, exception);
        public static void Debug(this ILogService log, string message, Exception exception) => log.Log(LogLevel.Debug, message, exception);
        
        public static void Info(this ILogService log, string message) => log.Log(LogLevel.Info, message);
        public static void Info(this ILogService log, Exception exception) => log.Log(LogLevel.Info, exception);
        public static void Info(this ILogService log, string message, Exception exception) => log.Log(LogLevel.Info, message, exception);
        
        public static void Warning(this ILogService log, string message) => log.Log(LogLevel.Warning, message);
        public static void Warning(this ILogService log, Exception exception) => log.Log(LogLevel.Warning, exception);
        public static void Warning(this ILogService log, string message, Exception exception) => log.Log(LogLevel.Warning, message, exception);
                
        public static void Error(this ILogService log, string message) => log.Log(LogLevel.Error, message);
        public static void Error(this ILogService log, Exception exception) => log.Log(LogLevel.Error, exception);
        public static void Error(this ILogService log, string message, Exception exception) => log.Log(LogLevel.Error, message, exception);

        public static void Fatal(this ILogService log, string message) => log.Log(LogLevel.Fatal, message);
        public static void Fatal(this ILogService log, Exception exception) => log.Log(LogLevel.Fatal, exception);
        public static void Fatal(this ILogService log, string message, Exception exception) => log.Log(LogLevel.Fatal, message, exception);
    }
}
