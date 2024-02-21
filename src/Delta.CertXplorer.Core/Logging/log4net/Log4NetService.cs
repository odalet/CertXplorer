using System;
using System.IO;
using Delta.CertXplorer.UI;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

namespace Delta.CertXplorer.Logging.log4net
{
    /// <summary>
    /// Wraps the logging Log4Net logging framework into an <see cref="Delta.CertXplorer.Logging.ILogService"/>
    /// so that it can be used as any logging service of the Delta.CertXplorer framework.
    /// </summary>
    public sealed partial class Log4NetService : BaseLogService, ITextBoxAppendable
    {
        private static readonly Type thisServiceType = typeof(Log4NetService);
        private readonly FileInfo configurationFileInfo;

        public Log4NetService(FileInfo configurationFile) : this(configurationFile, true) { }
        private Log4NetService(FileInfo configurationFile, bool configure)
        {
            configurationFileInfo = configurationFile;
            if (configure) _ = XmlConfigurator.ConfigureAndWatch(configurationFileInfo);
        }

        private ILog CurrentLog => LogManager.GetLogger(
            string.IsNullOrEmpty(currentSourceName) ? DefaultSourceName : currentSourceName);

        public ILog GetLogger() => CurrentLog;

        public override void Log(LogEntry entry)
        {
            if (entry == null) throw new ArgumentNullException("entry");

            // Do we have a source? If yes ask-it to pre process the entry.
            if (CurrentSource != null) entry = CurrentSource.ProcessLogEntry(entry);
            if (entry == null) return; // The current source decided this entry should not be logged.

            CurrentLog.Logger.Log(thisServiceType, Helper.LogLevelToLog4NetLevel(entry.Level), entry.Message, entry.Exception);
        }

        public ITextBoxAppender AddLogBox(ThreadSafeTextBoxWrapper textboxWrapper) => AddLogBox(textboxWrapper, string.Empty);
        public ITextBoxAppender AddLogBox(ThreadSafeTextBoxWrapper textboxWrapper, string patternLayout)
        {
            if (textboxWrapper == null) throw new ArgumentNullException("textboxWrapper");
            if (CurrentLog == null) return null;

            if (CurrentLog.Logger is not IAppenderAttachable appenderAttachable)
                return null;

            var appender = string.IsNullOrEmpty(patternLayout) ?
                new TextBoxAppender(textboxWrapper) :
                new TextBoxAppender(textboxWrapper, new PatternLayout(patternLayout));
            appender.LogThreshold = LogLevel.All;
            appenderAttachable.AddAppender(appender);
            return appender;
        }
    }
}
