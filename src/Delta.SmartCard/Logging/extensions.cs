using System;

namespace Delta.SmartCard.Logging
{
    /// <summary>
    /// Adds shortcut methods to the <see cref="Siti.Logging.ILogService"/> interface.
    /// </summary>
    internal static class LoggingExtensions
    {
        public static void Log(this ILogService logService, string level, string message)
        {
            logService.Log(level, message, null);
        }

        public static void Log(this ILogService logService, string level, Exception exception)
        {
            logService.Log(level, string.Empty, exception);
        }

        public static void Verbose(this ILogService logService, string message) { logService.Log("VERBOSE", message); }
        public static void Verbose(this ILogService logService, Exception exception) { logService.Log("VERBOSE", exception); }
        public static void Verbose(this ILogService logService, string message, Exception exception) { logService.Log("VERBOSE", message, exception); }

        public static void Debug(this ILogService logService, string message) { logService.Log("DEBUG", message); }
        public static void Debug(this ILogService logService, Exception exception) { logService.Log("DEBUG", exception); }
        public static void Debug(this ILogService logService, string message, Exception exception) { logService.Log("DEBUG", message, exception); }

        public static void Info(this ILogService logService, string message) { logService.Log("INFO", message); }
        public static void Info(this ILogService logService, Exception exception) { logService.Log("INFO", exception); }
        public static void Info(this ILogService logService, string message, Exception exception) { logService.Log("INFO", message, exception); }
        
        public static void Warning(this ILogService logService, string message) { logService.Log("WARNING", message); }
        public static void Warning(this ILogService logService, Exception exception) { logService.Log("WARNING", exception); }
        public static void Warning(this ILogService logService, string message, Exception exception) { logService.Log("WARNING", message, exception); }
        
        public static void Error(this ILogService logService, string message) { logService.Log("ERROR", message); }
        public static void Error(this ILogService logService, Exception exception) { logService.Log("ERROR", exception); }
        public static void Error(this ILogService logService, string message, Exception exception) { logService.Log("ERROR", message, exception); }

        public static void Fatal(this ILogService logService, string message) { logService.Log("ERROR", message); }
        public static void Fatal(this ILogService logService, Exception exception) { logService.Log("ERROR", exception); }
        public static void Fatal(this ILogService logService, string message, Exception exception) { logService.Log("ERROR", message, exception); }
    }
}
