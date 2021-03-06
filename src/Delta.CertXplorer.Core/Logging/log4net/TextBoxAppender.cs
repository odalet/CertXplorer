using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Delta.CertXplorer.UI;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;

namespace Delta.CertXplorer.Logging.log4net
{
    internal sealed class TextBoxAppender : AppenderSkeleton, ITextBoxAppender
    {
        private sealed class TrivialDisposable : IDisposable 
        {
            public void Dispose() 
            {
                // Nothing to do
            } 
        }

        private sealed class DisposableList : IDisposable
        {
            private readonly List<IDisposable> list;

            public DisposableList(params IDisposable[] disposableItems) =>
                list = new List<IDisposable>(disposableItems.Where(item => item != null));

            public void Dispose()
            {
                foreach (var disposable in list) 
                    disposable.Dispose();
            }
        }

        private const string defaultLayout = "%date [%thread] %-8level- %message%newline";
        private readonly bool isRichTextBox;
        private bool closed = false;

        public TextBoxAppender(ThreadSafeTextBoxWrapper textboxWrapper) :
            this(textboxWrapper, new PatternLayout(defaultLayout)) { }

        public TextBoxAppender(ThreadSafeTextBoxWrapper textboxWrapper, ILayout layout)
        {
            TextBoxWrapper = textboxWrapper ?? throw new ArgumentNullException(nameof(textboxWrapper));
            isRichTextBox = TextBoxWrapper is ThreadSafeRichTextBoxWrapper;

            Layout = layout ?? new PatternLayout(defaultLayout);
        }

        public ThreadSafeTextBoxWrapper TextBoxWrapper { get; }

        public LogLevel LogThreshold
        {
            get => Helper.Log4NetLevelToLogLevel(Threshold);
            set => Threshold = Helper.LogLevelToLog4NetLevel(value);
        }

        public void Dispose() => Close();

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (closed) return;

            if (isRichTextBox)
                AppendRich((ThreadSafeRichTextBoxWrapper)TextBoxWrapper, loggingEvent);
            else 
                AppendNormal(TextBoxWrapper, loggingEvent);
        }

        protected override void OnClose()
        {
            base.OnClose();
            closed = true;
        }

        private void AppendRich(ThreadSafeRichTextBoxWrapper textboxWrapper, LoggingEvent loggingEvent)
        {
            using (GetFormat(textboxWrapper, loggingEvent))
                AppendNormal(textboxWrapper, loggingEvent);
        }

        private void AppendNormal(ThreadSafeTextBoxWrapper textboxWrapper, LoggingEvent loggingEvent)
        {
            var text = RenderLoggingEvent(loggingEvent);
            textboxWrapper.AppendText(text);
            textboxWrapper.ScrollToCaret();
        }

        private IDisposable GetFormat(ThreadSafeRichTextBoxWrapper textboxWrapper, LoggingEvent loggingEvent)
        {
            if (loggingEvent.Level == Level.All)  return new TrivialDisposable();

            if (loggingEvent.Level == Level.Verbose || loggingEvent.Level == Level.Finest)
                return CreateRichTextBoxFormatter(textboxWrapper, Color.Blue);
            if (loggingEvent.Level == Level.Trace || loggingEvent.Level == Level.Finer)
                return CreateRichTextBoxFormatter(textboxWrapper, Color.Blue, FontStyle.Bold | FontStyle.Underline);
            if (loggingEvent.Level == Level.Debug || loggingEvent.Level == Level.Fine)
                return CreateRichTextBoxFormatter(textboxWrapper, Color.Blue, FontStyle.Bold);                
            
            if (loggingEvent.Level == Level.Info) // Black on White
                return new TrivialDisposable();
            
            if (loggingEvent.Level == Level.Notice)
                return CreateRichTextBoxFormatter(textboxWrapper, Color.Orange);
            if (loggingEvent.Level == Level.Warn)
                return CreateRichTextBoxFormatter(textboxWrapper, Color.Orange, FontStyle.Bold);
            
            if (loggingEvent.Level == Level.Error)
                return CreateRichTextBoxFormatter(textboxWrapper, Color.Red);
            if (loggingEvent.Level == Level.Severe)
                return CreateRichTextBoxFormatter(textboxWrapper, Color.Red, FontStyle.Bold);
            if (loggingEvent.Level == Level.Critical)
                return CreateRichTextBoxFormatter(textboxWrapper, Color.Red, FontStyle.Bold|FontStyle.Underline);
            
            if (loggingEvent.Level == Level.Alert)
                return CreateRichTextBoxFormatter(textboxWrapper, Color.White, Color.Red);
            if (loggingEvent.Level == Level.Fatal)                
                return CreateRichTextBoxFormatter(textboxWrapper, Color.White, Color.Red, FontStyle.Bold);
            if (loggingEvent.Level == Level.Emergency)
                return CreateRichTextBoxFormatter(textboxWrapper, Color.White, Color.Red, FontStyle.Bold|FontStyle.Underline);
            
            return new TrivialDisposable();
        }

        private IDisposable CreateRichTextBoxFormatter(ThreadSafeRichTextBoxWrapper textboxWrapper, Color foreColor, FontStyle style) => 
            CreateRichTextBoxFormatter(textboxWrapper, foreColor, Color.Empty, style);

        private IDisposable CreateRichTextBoxFormatter(ThreadSafeRichTextBoxWrapper textboxWrapper, Color foreColor) => 
            CreateRichTextBoxFormatter(textboxWrapper, foreColor, Color.Empty);

        private IDisposable CreateRichTextBoxFormatter(ThreadSafeRichTextBoxWrapper textboxWrapper, Color foreColor, Color backColor) => 
            new RichTextBoxFormatter(textboxWrapper, foreColor, backColor);

        private IDisposable CreateRichTextBoxFormatter(ThreadSafeRichTextBoxWrapper textboxWrapper, Color foreColor, Color backColor, FontStyle style)
        {
            var font = new Font(textboxWrapper.Font, style);
            var formatter = new RichTextBoxFormatter(textboxWrapper, foreColor, backColor, font);

            return new DisposableList(formatter, font);
        }
    }
}
