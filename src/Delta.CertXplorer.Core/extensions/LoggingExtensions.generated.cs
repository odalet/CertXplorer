﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using Delta.CertXplorer.Logging;

namespace Delta.CertXplorer
{
	/// <summary>
    /// Adds shortcut methods to the <see cref="Delta.CertXplorer.Logging.ILogService"/> interface.
	/// </summary>
	public static class LoggingExtensions
	{
        #region ILogService <--> IMultiSourceLogService

        /// <summary>
        /// Gets the log source of the specified service (if it is a <see cref="IMultiSourceLogService"/>).
        /// </summary>
        /// <param name="log">The logging service.</param>
        /// <returns>The logging service source, or <c>null</c> if it is not a <see cref="IMultiSourceLogService"/>.</returns>
        public static LogSource GetLogSource(this ILogService log)
        {
            if (log is IMultiSourceLogService)
                return ((IMultiSourceLogService)log).CurrentSource;
            else return null;
        }

        #endregion

        #region Log

        /// <summary>
        /// Logs the specified message with the specified log level on the specified source.
        /// </summary>
        /// <param name="log">The logging service.</param>
        /// <param name="source">The source.</param>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        public static void Log(this ILogService log, LogLevel level, string message, string source)
        {
            if (log is IMultiSourceLogService)
                ((IMultiSourceLogService)log)[source].Log(level, message);
            else log.Log(level, message); 
        }

        /// <summary>
        /// Logs the specified exception with the specified log level on the specified source.
        /// </summary>
        /// <param name="log">The logging service.</param>
        /// <param name="source">The source.</param>
        /// <param name="level">The level.</param>
        /// <param name="exception">The exception.</param>
        public static void Log(this ILogService log, LogLevel level, Exception exception, string source)
        {
            if (log is IMultiSourceLogService)
                ((IMultiSourceLogService)log)[source].Log(level, exception);
            else log.Log(level, exception);
        }

        /// <summary>
        /// Logs the specified exception and message with the specified log level on the specified source.
        /// </summary>
        /// <param name="log">The logging service.</param>
        /// <param name="source">The source.</param>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public static void Log(this ILogService log, LogLevel level, string message, Exception exception, string source)
        {
            if (log is IMultiSourceLogService)
                ((IMultiSourceLogService)log)[source].Log(level, message, exception);
            else log.Log(level, message, exception);
        }

        /// <summary>
        /// Logs the specified message with the specified log level on the specified source.
        /// </summary>
        /// <param name="log">The logging service.</param>
        /// <param name="source">The source.</param>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        public static void Log(this ILogService log, LogLevel level, string message, LogSource source)
        {
            if (log is IMultiSourceLogService)
                ((IMultiSourceLogService)log)[source].Log(level, message);
            else log.Log(level, message);
        }

        /// <summary>
        /// Logs the specified exception with the specified log level on the specified source.
        /// </summary>
        /// <param name="log">The logging service.</param>
        /// <param name="source">The source.</param>
        /// <param name="level">The level.</param>
        /// <param name="exception">The exception.</param>
        public static void Log(this ILogService log, LogLevel level, Exception exception, LogSource source)
        {
            if (log is IMultiSourceLogService)
                ((IMultiSourceLogService)log)[source].Log(level, exception);
            else log.Log(level, exception);
        }
        
        /// <summary>
        /// Logs the specified exception and message with the specified log level on the specified source.
        /// </summary>
        /// <param name="log">The logging service.</param>
        /// <param name="source">The source.</param>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public static void Log(this ILogService log, LogLevel level, string message, Exception exception, LogSource source)
        {
            if (log is IMultiSourceLogService)
                ((IMultiSourceLogService)log)[source].Log(level, message, exception);
            else log.Log(level, message, exception);
        }

        #endregion
	

		#region Verbose

		#region No source

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Verbose"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="message">The message to trace.</param>
        public static void Verbose(this ILogService log, string message) { log.Log(LogLevel.Verbose, message); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Verbose"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Verbose(this ILogService log, Exception exception) { log.Log(LogLevel.Verbose, exception); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Verbose"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Verbose(this ILogService log, string message, Exception exception) { log.Log(LogLevel.Verbose, message, exception); }
        
        #endregion
        
		#region Source as string

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Verbose"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        public static void Verbose(this ILogService log, string message, string source) { Log(log, LogLevel.Verbose, message, source); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Verbose"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Verbose(this ILogService log, Exception exception, string source) { Log(log, LogLevel.Verbose, exception, source); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Verbose"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Verbose(this ILogService log, string message, Exception exception, string source) { Log(log, LogLevel.Verbose, message, exception, source); }
        
        #endregion
        
		#region Source as LogSource

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Verbose"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        public static void Verbose(this ILogService log, string message, LogSource source) { Log(log, LogLevel.Verbose, message, source); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Verbose"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Verbose(this ILogService log, Exception exception, LogSource source) { log.Log(LogLevel.Verbose, exception, source); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Verbose"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Verbose(this ILogService log, string message, Exception exception, LogSource source) { log.Log(LogLevel.Verbose, message, exception, source); }
        
        #endregion
        
        #endregion

		#region Debug

		#region No source

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Debug"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="message">The message to trace.</param>
        public static void Debug(this ILogService log, string message) { log.Log(LogLevel.Debug, message); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Debug"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Debug(this ILogService log, Exception exception) { log.Log(LogLevel.Debug, exception); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Debug"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Debug(this ILogService log, string message, Exception exception) { log.Log(LogLevel.Debug, message, exception); }
        
        #endregion
        
		#region Source as string

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Debug"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        public static void Debug(this ILogService log, string message, string source) { Log(log, LogLevel.Debug, message, source); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Debug"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Debug(this ILogService log, Exception exception, string source) { Log(log, LogLevel.Debug, exception, source); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Debug"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Debug(this ILogService log, string message, Exception exception, string source) { Log(log, LogLevel.Debug, message, exception, source); }
        
        #endregion
        
		#region Source as LogSource

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Debug"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        public static void Debug(this ILogService log, string message, LogSource source) { Log(log, LogLevel.Debug, message, source); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Debug"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Debug(this ILogService log, Exception exception, LogSource source) { log.Log(LogLevel.Debug, exception, source); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Debug"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Debug(this ILogService log, string message, Exception exception, LogSource source) { log.Log(LogLevel.Debug, message, exception, source); }
        
        #endregion
        
        #endregion

		#region Info

		#region No source

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Info"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="message">The message to trace.</param>
        public static void Info(this ILogService log, string message) { log.Log(LogLevel.Info, message); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Info"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Info(this ILogService log, Exception exception) { log.Log(LogLevel.Info, exception); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Info"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Info(this ILogService log, string message, Exception exception) { log.Log(LogLevel.Info, message, exception); }
        
        #endregion
        
		#region Source as string

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Info"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        public static void Info(this ILogService log, string message, string source) { Log(log, LogLevel.Info, message, source); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Info"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Info(this ILogService log, Exception exception, string source) { Log(log, LogLevel.Info, exception, source); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Info"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Info(this ILogService log, string message, Exception exception, string source) { Log(log, LogLevel.Info, message, exception, source); }
        
        #endregion
        
		#region Source as LogSource

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Info"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        public static void Info(this ILogService log, string message, LogSource source) { Log(log, LogLevel.Info, message, source); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Info"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Info(this ILogService log, Exception exception, LogSource source) { log.Log(LogLevel.Info, exception, source); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Info"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Info(this ILogService log, string message, Exception exception, LogSource source) { log.Log(LogLevel.Info, message, exception, source); }
        
        #endregion
        
        #endregion

		#region Warning

		#region No source

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Warning"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="message">The message to trace.</param>
        public static void Warning(this ILogService log, string message) { log.Log(LogLevel.Warning, message); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Warning"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Warning(this ILogService log, Exception exception) { log.Log(LogLevel.Warning, exception); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Warning"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Warning(this ILogService log, string message, Exception exception) { log.Log(LogLevel.Warning, message, exception); }
        
        #endregion
        
		#region Source as string

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Warning"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        public static void Warning(this ILogService log, string message, string source) { Log(log, LogLevel.Warning, message, source); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Warning"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Warning(this ILogService log, Exception exception, string source) { Log(log, LogLevel.Warning, exception, source); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Warning"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Warning(this ILogService log, string message, Exception exception, string source) { Log(log, LogLevel.Warning, message, exception, source); }
        
        #endregion
        
		#region Source as LogSource

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Warning"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        public static void Warning(this ILogService log, string message, LogSource source) { Log(log, LogLevel.Warning, message, source); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Warning"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Warning(this ILogService log, Exception exception, LogSource source) { log.Log(LogLevel.Warning, exception, source); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Warning"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Warning(this ILogService log, string message, Exception exception, LogSource source) { log.Log(LogLevel.Warning, message, exception, source); }
        
        #endregion
        
        #endregion

		#region Error

		#region No source

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Error"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="message">The message to trace.</param>
        public static void Error(this ILogService log, string message) { log.Log(LogLevel.Error, message); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Error"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Error(this ILogService log, Exception exception) { log.Log(LogLevel.Error, exception); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Error"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Error(this ILogService log, string message, Exception exception) { log.Log(LogLevel.Error, message, exception); }
        
        #endregion
        
		#region Source as string

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Error"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        public static void Error(this ILogService log, string message, string source) { Log(log, LogLevel.Error, message, source); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Error"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Error(this ILogService log, Exception exception, string source) { Log(log, LogLevel.Error, exception, source); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Error"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Error(this ILogService log, string message, Exception exception, string source) { Log(log, LogLevel.Error, message, exception, source); }
        
        #endregion
        
		#region Source as LogSource

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Error"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        public static void Error(this ILogService log, string message, LogSource source) { Log(log, LogLevel.Error, message, source); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Error"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Error(this ILogService log, Exception exception, LogSource source) { log.Log(LogLevel.Error, exception, source); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Error"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Error(this ILogService log, string message, Exception exception, LogSource source) { log.Log(LogLevel.Error, message, exception, source); }
        
        #endregion
        
        #endregion

		#region Fatal

		#region No source

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Fatal"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="message">The message to trace.</param>
        public static void Fatal(this ILogService log, string message) { log.Log(LogLevel.Fatal, message); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Fatal"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Fatal(this ILogService log, Exception exception) { log.Log(LogLevel.Fatal, exception); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Fatal"/> trace level.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Fatal(this ILogService log, string message, Exception exception) { log.Log(LogLevel.Fatal, message, exception); }
        
        #endregion
        
		#region Source as string

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Fatal"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        public static void Fatal(this ILogService log, string message, string source) { Log(log, LogLevel.Fatal, message, source); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Fatal"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Fatal(this ILogService log, Exception exception, string source) { Log(log, LogLevel.Fatal, exception, source); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Fatal"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Fatal(this ILogService log, string message, Exception exception, string source) { Log(log, LogLevel.Fatal, message, exception, source); }
        
        #endregion
        
		#region Source as LogSource

		/// <summary>
        /// Logs the specified message with the <see cref="LogLevel.Fatal"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        public static void Fatal(this ILogService log, string message, LogSource source) { Log(log, LogLevel.Fatal, message, source); }
        
        /// <summary>
        /// Logs the specified exception with the <see cref="LogLevel.Fatal"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Fatal(this ILogService log, Exception exception, LogSource source) { log.Log(LogLevel.Fatal, exception, source); }

        /// <summary>
        /// Logs the specified message and exception with the <see cref="LogLevel.Fatal"/> trace level on the specified source.
        /// </summary>
        /// <param name="log">The logging service used to output the trace.</param>
        /// <param name="source">The source.</param>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Fatal(this ILogService log, string message, Exception exception, LogSource source) { log.Log(LogLevel.Fatal, message, exception, source); }
        
        #endregion
        
        #endregion
	}
}
